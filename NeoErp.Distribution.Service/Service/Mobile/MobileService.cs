using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.html;
using NeoErp.Core.Helpers;
using NeoErp.Core.Models;
using NeoErp.Core.Services;
using NeoErp.Distribution.Service.Model.Mobile;
using NepaliDateConverter.Net;
using Oracle.ManagedDataAccess.Client;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.Pkcs;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
//using NepaliDateConverter.Net;

namespace NeoErp.Distribution.Service.Service.Mobile
{
    public class MobileService : IMobileService
    {
        private const string CATEGORY_CODE = "FG";
        private const string GROUP_SKU_FLAG = "I";
        private IMessageService _MessageService;
        private readonly string UploadPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + @"Areas\NeoErp.Distribution\Images";

        public NeoErpCoreEntity _objectEntity = new NeoErpCoreEntity();
        public MobileService(IMessageService messageService)
        {
            _MessageService = messageService;
        }

        #region Private Functions
        private int GetMaxId(string table, string column, NeoErpCoreEntity dbContext)
        {
            var query = $"SELECT nvl(max({column}),0) +1 as p_key FROM {table}";
            var result = dbContext.SqlQuery<int>(query).FirstOrDefault();
            return result;
        }
        private int checkGlobalCompany(NeoErpCoreEntity dbContext)
        {
            return dbContext.SqlQuery<int>(
                    "SELECT COUNT(*) FROM company_setup WHERE NVL(consolidate_flag, 'N') <> 'Y'"
                ).FirstOrDefault();
        }

        //private bool IsDistanceGreater(string lat1, string lon1, string lat2, string lon2, int meters)
        //{
        //    const double R = 6371000; // Earth's radius in meters
        //    //double ToRad(double angle) => (angle * Math.PI) / 180;

        //    //// Try to parse the latitude and longitude inputs
        //    //if (!double.TryParse(lat1, out double lat1Double) ||
        //    //    !double.TryParse(lon1, out double lon1Double) ||
        //    //    !double.TryParse(lat2, out double lat2Double) ||
        //    //    !double.TryParse(lon2, out double lon2Double))
        //    //{
        //    //    throw new ArgumentException("Invalid latitude or longitude values.");
        //    //}

        //    //// Convert degrees to radians
        //    //double lat1Rad = ToRad(lat1Double);
        //    //double lon1Rad = ToRad(lon1Double);
        //    //double lat2Rad = ToRad(lat2Double);
        //    //double lon2Rad = ToRad(lon2Double);

        //    //// Differences
        //    //double dLat = lat2Rad - lat1Rad;
        //    //double dLon = lon2Rad - lon1Rad;

        //    //// Haversine formula
        //    //double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
        //    //            Math.Cos(lat1Rad) * Math.Cos(lat2Rad) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        //    //double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        //    //double distance = R * c;

        //    //Debug.WriteLine(distance > meters);
        //    //return distance > meters;
        //    return R > 2;
        //}

        private List<ItemModel> FetchAllCompanyItems(string companyCode, NeoErpCoreEntity dbContext)
        {
            var pref = FetchPreferences(companyCode, dbContext);
            string conversionClause = "";
            if (pref.SQL_NN_CONVERSION_UNIT_FACTOR == "Y")
                conversionClause = "AND IUS.MU_CODE IS NOT NULL AND IUS.CONVERSION_FACTOR IS NOT NULL";
            var Query = $@"SELECT IM.ITEM_CODE, IM.ITEM_EDESC, ISS.BRAND_NAME, IM.INDEX_MU_CODE AS UNIT, IM.INDEX_MU_CODE AS MU_CODE, MC.MU_EDESC, IUS.MU_CODE CONVERSION_UNIT,TO_CHAR(IUS.CONVERSION_FACTOR) CONVERSION_FACTOR, IM.COMPANY_CODE, IM.BRANCH_CODE
				FROM IP_ITEM_MASTER_SETUP IM
				  INNER JOIN IP_MU_CODE MC ON MC.MU_CODE = IM.INDEX_MU_CODE AND MC.COMPANY_CODE = IM.COMPANY_CODE
				  INNER JOIN IP_ITEM_SPEC_SETUP ISS ON ISS.ITEM_CODE = IM.ITEM_CODE AND ISS.COMPANY_CODE = IM.COMPANY_CODE AND TRIM(ISS.BRAND_NAME) IS NOT NULL
				  LEFT JOIN IP_ITEM_UNIT_SETUP IUS ON IUS.ITEM_CODE = ISS.ITEM_CODE AND IUS.COMPANY_CODE = ISS.COMPANY_CODE AND IUS.MU_CODE IS NOT NULL AND IUS.CONVERSION_FACTOR IS NOT NULL
				WHERE 1 = 1
				AND IM.COMPANY_CODE IN (SELECT COMPANY_CODE FROM COMPANY_SETUP) AND IM.CATEGORY_CODE = '{CATEGORY_CODE}' AND IM.GROUP_SKU_FLAG = '{GROUP_SKU_FLAG}' AND IM.DELETED_FLAG = 'N'
                {conversionClause}
				ORDER BY IM.COMPANY_CODE, IM.BRANCH_CODE, UPPER(IM.ITEM_EDESC) ASC";
            var data = dbContext.SqlQuery<ItemModel>(Query).ToList();
            return data;
        }

        private List<ItemModel> FetchAllCompanyBranchItemRate(NeoErpCoreEntity dbContext)
        {
            var Query = $@"SELECT B.COMPANY_CODE, B.BRANCH_CODE, A.ITEM_CODE, TO_CHAR(NVL(B.SALES_RATE, 0)) SALES_RATE, A.APPLY_DATE
                  FROM (SELECT ITEM_CODE, COMPANY_CODE, BRANCH_CODE,TO_CHAR(MAX(APP_DATE)) APPLY_DATE 
                    FROM IP_ITEM_RATE_APPLICAT_SETUP
                    WHERE 1 = 1
                    GROUP BY ITEM_CODE, COMPANY_CODE, BRANCH_CODE) A
                  INNER JOIN IP_ITEM_RATE_APPLICAT_SETUP B
                    ON B.ITEM_CODE = A.ITEM_CODE
                    AND B.APP_DATE = A.APPLY_DATE
                    AND B.COMPANY_CODE = A.COMPANY_CODE
                    AND B.COMPANY_CODE IN (SELECT COMPANY_CODE FROM COMPANY_SETUP)
                    AND B.BRANCH_CODE = A.BRANCH_CODE
                  WHERE 1 = 1
                  AND SALES_RATE <> 0
                  ORDER BY B.COMPANY_CODE, B.BRANCH_CODE, TO_NUMBER(A.ITEM_CODE)";
            var data = dbContext.SqlQuery<ItemModel>(Query).ToList();
            return data;
        }

        private List<SubLedgerMapModel> FetchAllCompanySubLedgerMap(NeoErpCoreEntity dbContext)
        {
            var Query = $@"SELECT SLM.ACC_CODE, SLM.SUB_CODE, CS.PARTY_TYPE_CODE AS DEFAULT_PARTY_TYPE_CODE, SLM.COMPANY_CODE
                FROM FA_SUB_LEDGER_MAP SLM
                INNER JOIN SA_CUSTOMER_SETUP CS ON TRIM(CS.LINK_SUB_CODE) = TRIM(SLM.SUB_CODE) AND CS.GROUP_SKU_FLAG = 'I' AND CS.COMPANY_CODE = SLM.COMPANY_CODE
                WHERE SUBSTR(SLM.SUB_CODE, 1, 1) = 'C'
                ORDER BY TO_NUMBER(SLM.ACC_CODE), TO_NUMBER(SUBSTR(SLM.SUB_CODE, 2)), SLM.COMPANY_CODE";
            var data = dbContext.SqlQuery<SubLedgerMapModel>(Query).ToList();
            return data;
        }

        private List<PartyTypeModel> FetchAllCompanyPartyType(NeoErpCoreEntity dbContext)
        {
            var data = dbContext.SqlQuery<PartyTypeModel>("SELECT PARTY_TYPE_CODE, PARTY_TYPE_EDESC PARTY_TYPE_NAME, TO_CHAR(CREDIT_DAYS) CREDIT_DAYS, TO_CHAR(CREDIT_LIMIT) CREDIT_LIMIT, COMPANY_CODE FROM IP_PARTY_TYPE_CODE WHERE DELETED_FLAG = 'N'").ToList();
            return data;
        }

        private List<CustomerModel> FetchAllCompanySaCustomer(NeoErpCoreEntity dbContext)
        {
            var Query = $@"SELECT CUSTOMER_CODE, CUSTOMER_EDESC CUSTOMER_NAME, REGD_OFFICE_EADDRESS ADDRESS, PARTY_TYPE_CODE, LINK_SUB_CODE, ACC_CODE,
                 TO_CHAR(CREDIT_DAYS) CREDIT_DAYS, To_CHAR(CREDIT_LIMIT) CREDIT_LIMIT, COMPANY_CODE, BRANCH_CODE
                FROM SA_CUSTOMER_SETUP 
                WHERE GROUP_SKU_FLAG = 'I'
                AND DELETED_FLAG = 'N'";
            var data = dbContext.SqlQuery<CustomerModel>(Query).ToList();
            return data;
        }
        private List<CalenderDataModel> FetchCalendarData(ProfileDetailsModel model, NeoErpCoreEntity dbContext)
        {
            try
            {
                var query = $@"SELECT TO_CHAR(ATN.ATTENDANCE_DT, 'YYYY-MM-DD') MONTH_DAY,
                   ATN.EMPLOYEE_ID,
                   TO_CHAR(ATN.ATTENDANCE_DT, 'YYYY-MM-DD') ATTENDANCE_DT,
                   TO_CHAR(ATN.IN_TIME, 'HH24:MI') IN_TIME,
                   TO_CHAR(ATN.OUT_TIME, 'HH24:MI') OUT_TIME,ATN.LATE_STATUS,
                   CASE
                       WHEN ATN.OVERALL_STATUS = 'DO' THEN 'Day Off'
                       WHEN ATN.OVERALL_STATUS = 'HD' THEN 'On Holiday (' || HMS.HOLIDAY_ENAME || ')'
                       WHEN ATN.OVERALL_STATUS = 'LV' THEN 'On Leave (' || LMS.LEAVE_ENAME || ')'
                       WHEN ATN.OVERALL_STATUS = 'TV' THEN 'On Travel (' || ETR.DESTINATION || ')' || LATE_STATUS_DESC(ATN.LATE_STATUS)
                       WHEN ATN.OVERALL_STATUS = 'TN' THEN
                           CASE
                               WHEN TMS.SHOW_AS_TRAINING = 'Y' THEN 'On Training (' || 
                                   CASE
                                       WHEN ATN.TRAINING_TYPE = 'A' THEN TMS.TRAINING_NAME
                                       ELSE ETN.TITLE
                                   END || ')' || LATE_STATUS_DESC(ATN.LATE_STATUS)
                                WHEN ATN.LATE_STATUS IS NOT NULL THEN 'On Training ' || LATE_STATUS_DESC(ATN.LATE_STATUS)
                                ELSE CASE
                                   WHEN ATN.TRAINING_TYPE = 'A' THEN TMS.TRAINING_NAME
                                   ELSE ETN.TITLE
                               END
                           END
                       WHEN ATN.OVERALL_STATUS = 'EC' THEN
                           CASE
                               WHEN EMS.SHOW_AS_EVENT = 'Y' THEN 'On Event and Conference (' || 
                                   CASE
                                       WHEN ATN.EVENT_TYPE = 'A' THEN EMS.EVENT_NAME
                                       ELSE EEN.TITLE
                                   END || ')'
                                WHEN ATN.LATE_STATUS IS NOT NULL THEN 'On Event and Conference ' || LATE_STATUS_DESC(ATN.LATE_STATUS)
                               ELSE 'On Event and Conference (' || 
                                   CASE
                                       WHEN ATN.EVENT_TYPE = 'R' THEN EEN.TITLE
                                       ELSE EMS.EVENT_NAME
                                   END || ')'
                           END
                       WHEN ATN.OVERALL_STATUS = 'WD' THEN 'Work On Dayoff' || LATE_STATUS_DESC(ATN.LATE_STATUS)
                       WHEN ATN.OVERALL_STATUS = 'WH' THEN 'Work on Holiday (' || HMS.HOLIDAY_ENAME || ')' || LATE_STATUS_DESC(ATN.LATE_STATUS)
                       WHEN ATN.OVERALL_STATUS = 'LP' THEN 'On Partial Leave (' || LMS.LEAVE_ENAME || ') ' || LATE_STATUS_DESC(ATN.LATE_STATUS)
                       WHEN ATN.OVERALL_STATUS = 'VP' THEN 'Work on Travel (' || ETR.DESTINATION || ') ' || LATE_STATUS_DESC(ATN.LATE_STATUS)
                       WHEN ATN.OVERALL_STATUS = 'TP' THEN 'Present (' || TMS.TRAINING_NAME || ') ' || LATE_STATUS_DESC(ATN.LATE_STATUS)
                       WHEN ATN.OVERALL_STATUS = 'PR' THEN 'Present ' || LATE_STATUS_DESC(ATN.LATE_STATUS)
                       WHEN ATN.OVERALL_STATUS = 'AB' THEN 'Absent'
                       WHEN ATN.OVERALL_STATUS = 'LA' THEN 'Present Late Penalty '|| LATE_STATUS_DESC(ATN.LATE_STATUS)
                   END AS ATTENDANCE_STATUS,
                   ATN.OVERALL_STATUS
            FROM HRIS_ATTENDANCE_DETAIL ATN
            LEFT JOIN HRIS_LEAVE_MASTER_SETUP LMS ON LMS.LEAVE_ID = ATN.LEAVE_ID
            LEFT JOIN HRIS_HOLIDAY_MASTER_SETUP HMS ON HMS.HOLIDAY_ID = ATN.HOLIDAY_ID
            LEFT JOIN USER_PREFERENCE_CALENDAR UPC ON ATN.EMPLOYEE_ID = UPC.EMPLOYEE_ID
            LEFT JOIN HRIS_TRAINING_MASTER_SETUP TMS ON (TMS.TRAINING_ID = ATN.TRAINING_ID AND ATN.TRAINING_TYPE='A')
            LEFT JOIN HRIS_EMPLOYEE_TRAINING_REQUEST ETN ON (ETN.REQUEST_ID=ATN.TRAINING_ID AND ATN.TRAINING_TYPE ='R')
            LEFT JOIN HRIS_EMPLOYEE_TRAVEL_REQUEST ETR ON ETR.TRAVEL_ID = ATN.TRAVEL_ID
            LEFT JOIN HRIS_EVENT_MASTER_SETUP EMS ON (EMS.EVENT_ID = ATN.EVENT_ID AND ATN.EVENT_TYPE='A')
            LEFT JOIN HRIS_EMPLOYEE_EVENT_REQUEST EEN ON (EEN.REQUEST_ID=ATN.EVENT_ID AND ATN.EVENT_TYPE ='R')
            WHERE ATN.ATTENDANCE_DT BETWEEN TO_DATE('{model.start_date}', 'DD-MON-YYYY') AND TO_DATE('{model.end_date}', 'DD-MON-YYYY')
            AND ATN.EMPLOYEE_ID = {model.sp_code} 
            ORDER BY ATN.ATTENDANCE_DT ASC";
                var list = dbContext.SqlQuery<CalenderDataModel>(query).ToList();
                return list.Count > 0 ? list : new List<CalenderDataModel>();
            }
            catch (Exception ex)
            {
                return new List<CalenderDataModel>();
            }

        }
        private List<string> FetchAttendanceStatus(ProfileDetailsModel model, NeoErpCoreEntity dbContext)
        {
            try
            {
                var query = $@"    SELECT 'Day Off' AS ATTENDANCE_STATUS FROM DUAL
                        UNION ALL
                        SELECT 'On Holiday'  FROM DUAL
                        UNION ALL
                        SELECT 'On Leave' FROM DUAL
                        UNION ALL
                        SELECT 'On Travel' FROM DUAL
                        UNION ALL
                        SELECT 'Late In'  FROM DUAL
                        UNION ALL
                        SELECT 'Early Out' FROM DUAL
                        UNION ALL
                        SELECT 'Missed Punch' FROM DUAL
                        UNION ALL
                        SELECT 'On Training' FROM DUAL
                        UNION ALL
                        SELECT 'On Event and Conference' FROM DUAL
                        UNION ALL
                        SELECT 'Work On Dayoff' FROM DUAL
                        UNION ALL
                        SELECT 'Work on Holiday' FROM DUAL
                        UNION ALL
                        SELECT 'On Partial Leave' FROM DUAL
                        UNION ALL
                        SELECT 'Work on Travel' FROM DUAL
                        UNION ALL
                        SELECT 'Present' FROM DUAL
                        UNION ALL
                        SELECT 'Absent' FROM DUAL
                        UNION ALL
                        SELECT 'Late Penalty' FROM DUAL";
                var list = dbContext.SqlQuery<string>(query).ToList();
                return list.Count > 0 ? list : new List<string>();
            }
            catch (Exception ex)
            {
                return new List<string>();
            }
        }

        private List<AreaResponseModel> FetchAllCompanyArea(NeoErpCoreEntity dbContext)
        {
            string Query = $@"SELECT A.AREA_CODE,A.AREA_NAME,A.ZONE_CODE,A.DISTRICT_CODE,A.VDC_CODE,A.REG_CODE,
                       B.ZONE_NAME,B.DISTRICT_NAME,B.VDC_NAME,B.REG_NAME,A.COMPANY_CODE
                FROM DIST_AREA_MASTER A,DIST_ADDRESS_MASTER B
                WHERE (A.DISTRICT_CODE = B.DISTRICT_CODE
                AND  A.REG_CODE = B.REG_CODE
                AND  A.ZONE_CODE = B.ZONE_CODE
                AND  A.VDC_CODE = B.VDC_CODE) ORDER BY UPPER(A.AREA_NAME) ASC";
            var data = dbContext.SqlQuery<AreaResponseModel>(Query).ToList();
            return data;
        }

        private List<SalesTypeModel> FetchAllCompanySaSalesType(NeoErpCoreEntity dbContext)
        {
            string Query = $@"SELECT SALES_TYPE_CODE, SALES_TYPE_EDESC, COMPANY_CODE FROM SA_SALES_TYPE
                            WHERE GROUP_SKU_FLAG = '{GROUP_SKU_FLAG}' AND DELETED_FLAG = 'N'
                            ORDER BY COMPANY_CODE, UPPER(TRIM(SALES_TYPE_EDESC))";
            var data = dbContext.SqlQuery<SalesTypeModel>(Query).ToList();
            return data;
        }

        private List<ShippingAddressModel> FetchShippingAddress(NeoErpCoreEntity dbContext)
        {
            string Query = $@"SELECT TRIM(CC.CITY_CODE) CITY_CODE, TRIM(CC.CITY_EDESC) CITY_EDESC, TRIM(DC.DISTRICT_EDESC) DISTRICT_EDESC,
                (CASE WHEN TRIM(CC.CITY_EDESC) = TRIM(DC.DISTRICT_EDESC) THEN TRIM(CC.CITY_EDESC) ELSE TRIM(CC.CITY_EDESC) || ', ' || TRIM(DC.DISTRICT_EDESC) END) CITY
                FROM CITY_CODE CC
                INNER JOIN DISTRICT_CODE DC ON TRIM(DC.DISTRICT_CODE) = TRIM(CC.DISTRICT_CODE)
                WHERE 1 = 1 
                  AND DELETED_FLAG = 'N'
                ORDER BY UPPER(TRIM(CITY_EDESC))";
            var data = dbContext.SqlQuery<ShippingAddressModel>(Query).ToList();
            data = data == null ? new List<ShippingAddressModel>() : data;
            return data;
        }

        public PreferenceModel FetchPreferences(string comp_code, NeoErpCoreEntity dbContext)
        {
            var PreferenceQuery = $"SELECT * FROM DIST_PREFERENCE_SETUP WHERE COMPANY_CODE='{comp_code}'";
            var preferences = dbContext.SqlQuery<PreferenceModel>(PreferenceQuery).FirstOrDefault();
            string tableCheckQuery = @"SELECT COUNT(*) FROM USER_TABLES WHERE TABLE_NAME = 'DIST_AUTO_TIME'";
            int tableCount = dbContext.SqlQuery<int>(tableCheckQuery).FirstOrDefault();
            if (tableCount <= 0)
            {
                string createTableQuery = @"
                CREATE TABLE dist_auto_time (
                id           NUMBER PRIMARY KEY,
                time         VARCHAR2(255),
                created_date DATE DEFAULT SYSDATE,
                Deleted_flag char(1),
                company_code VARCHAR2(255)
                )";
                dbContext.Database.ExecuteSqlCommand(createTableQuery);
            }
            var Query = $"SELECT time FROM dist_auto_time WHERE Deleted_flag='N'";
            var time = dbContext.SqlQuery<AutoTimeModel>(Query).FirstOrDefault();
            if (time != null)
            {
                preferences.AUTO_TIME = time.TIME;
            }
            return preferences;
        }

        #endregion Private Functions

        #region Fetching Data
        public List<LoginResponseModel> Login(LoginModel model, NeoErpCoreEntity dbContext)
        {
            LoginResponseModel result = new LoginResponseModel();
            if (string.IsNullOrEmpty(model.UserName as string))
                throw new Exception("Username is empty.");

            if (string.IsNullOrEmpty(model.Password as string))
                throw new Exception("Password is empty.");

            if (string.IsNullOrEmpty(model.Imei as string))
                model.Imei = "EMPTY";

            //user validation
            string UserQuery = $@"SELECT TO_CHAR(LU.USERID) AS USER_ID, TO_CHAR(LU.GROUPID) AS GROUP_ID, trim(LU.USER_NAME) as USER_NAME, LU.IS_MOBILE, LU.ATTENDANCE,
                    ES.EMPLOYEE_EDESC AS FULL_NAME, LU.PASS_WORD, LU.CONTACT_NO, LU.SP_CODE, LU.GROUPID,
                    LU.USER_TYPE, LU.SUPER_USER, LU.MGR_USER, TO_CHAR(LU.EXPIRY_DATE) EXPIRY_DATE, TO_CHAR(LU.LINK_SYN_USER_NO) AS LINK_SYN_USER_NO,TO_CHAR(DRU.ROLE_CODE) AS ROLE_CODE,
                    LU.COMPANY_CODE,LU.BRANCH_CODE,LU.AREA_CODE,LU.BRANDING
                FROM DIST_LOGIN_USER LU
                INNER JOIN DIST_SALESPERSON_MASTER SM ON SM.SP_CODE = LU.SP_CODE
                INNER JOIN HR_EMPLOYEE_SETUP ES ON ES.EMPLOYEE_CODE = SM.SP_CODE AND ES.COMPANY_CODE = LU.COMPANY_CODE --for user's actual company code in synergy
                INNER JOIN DIST_ROLE_USER DRU ON DRU.USERID = LU.USERID
                WHERE 1 = 1
                AND trim(LU.USER_NAME) = trim('{model.UserName}') 
                AND LU.PASS_WORD = '{model.Password}'
                AND LU.ACTIVE = 'Y'";
            result = dbContext.SqlQuery<LoginResponseModel>(UserQuery).FirstOrDefault();


            if (result != null)
            {
                string isPharma = dbContext.SqlQuery<string>("select is_pharma from dist_preference_setup").FirstOrDefault();

                result.IS_PHARMA = isPharma;

                string isAgro = dbContext.SqlQuery<string>("select is_agro from dist_preference_setup").FirstOrDefault();

                result.IS_AGRO = isAgro;

                int notY = dbContext.SqlQuery<int>(
                    "SELECT COUNT(*) FROM company_setup WHERE NVL(consolidate_flag, 'N') <> 'Y'"
                ).FirstOrDefault();

                result.IS_GLOBAL = (notY == 0) ? "Y" : "N";
            }


            if (result == null)
                throw new Exception("Incorrect username or password.");
            DateTime userDate;
            DateTime.TryParse(result.EXPIRY_DATE, out userDate);
            if (userDate < DateTime.Now)
                throw new Exception("User is expired.");
            if (result.IS_MOBILE == "N")
                throw new Exception("Not a Mobile User.");

            //imei validation
            var companyName = dbContext.SqlQuery<string>($"SELECT COMPANY_EDESC FROM COMPANY_SETUP WHERE COMPANY_CODE='{result.COMPANY_CODE}'").FirstOrDefault();
            if (!companyName.Equals("JGI Distribution Pvt. Ltd."))
            {
                List<string> SavedImei = dbContext.SqlQuery<string>($"SELECT IMEI_NO FROM DIST_LOGIN_DEVICE WHERE USERID='{result.USER_ID}' AND APPROVED_FLAG='Y' AND ACTIVE='Y'").ToList();
                var imei = dbContext.SqlQuery<string>($"SELECT IMEI_NO FROM DIST_LOGIN_DEVICE WHERE IMEI_NO='{model.Imei}' ").ToList();
                if (SavedImei.Count == 0)
                {
                    if (imei.Count > 0 && model.Imei != "EMPTY")
                        throw new Exception("Device already in use by another user");
                    string imeiInsert = $@"INSERT INTO DIST_LOGIN_DEVICE (USERID,IMEI_NO,DEVICE_NAME,CREATED_BY,APPROVED_FLAG,ACTIVE,APP_VERSION,FIREBASE_ID,CURRENT_LOGIN)
                VALUES ('{result.USER_ID}','{model.Imei}','{model.Device_Name}','{result.USER_ID}','Y','Y','{model.App_Version}','{model.Firebase_key}','Y')";
                    var rowNum = dbContext.ExecuteSqlCommand(imeiInsert);
                }
                else if (!SavedImei.Contains(model.Imei.Trim()))
                {
                    if (imei.Count > 0 && model.Imei != "EMPTY")
                        throw new Exception("Device already in use by another user");
                    string imeiInsert = $@"INSERT INTO DIST_LOGIN_DEVICE (USERID,IMEI_NO,DEVICE_NAME,CREATED_BY,APP_VERSION,FIREBASE_ID,ACTIVE,APPROVED_FLAG)
                VALUES ('{result.USER_ID}','{model.Imei}','{model.Device_Name}','{result.USER_ID}','{model.App_Version}','{model.Firebase_key}','N','N')";
                    var rowNum = dbContext.ExecuteSqlCommand(imeiInsert);
                    throw new Exception("IMEI_REG_ERROR");
                }
            }
            else
            {
                List<string> SavedImei = dbContext.SqlQuery<string>($"SELECT IMEI_NO FROM DIST_LOGIN_DEVICE WHERE USERID='{result.USER_ID}'").ToList();
                var imei = dbContext.SqlQuery<string>($"SELECT IMEI_NO FROM DIST_LOGIN_DEVICE WHERE IMEI_NO='{model.Imei}' ").ToList();
                if (SavedImei.Count == 0)
                {
                    if (imei.Count > 0 && model.Imei != "EMPTY")
                        throw new Exception("Device already in use by another user");
                    string imeiInsert = $@"INSERT INTO DIST_LOGIN_DEVICE (USERID,IMEI_NO,DEVICE_NAME,CREATED_BY,APPROVED_FLAG,ACTIVE,APP_VERSION,FIREBASE_ID,CURRENT_LOGIN)
                VALUES ('{result.USER_ID}','{model.Imei}','{model.Device_Name}','{result.USER_ID}','Y','Y','{model.App_Version}','{model.Firebase_key}','Y')";
                    var rowNum = dbContext.ExecuteSqlCommand(imeiInsert);
                }
                else if (!SavedImei.Contains(model.Imei.Trim()))
                {

                    if (imei.Count > 0 && model.Imei != "EMPTY")
                        throw new Exception("Device already in use by another user");
                    string imeiInsert = $@"INSERT INTO DIST_LOGIN_DEVICE (USERID,IMEI_NO,DEVICE_NAME,CREATED_BY,APP_VERSION,FIREBASE_ID)
                VALUES ('{result.USER_ID}','{model.Imei}','{model.Device_Name}','{result.USER_ID}','{model.App_Version}','{model.Firebase_key}')";
                    var rowNum = dbContext.ExecuteSqlCommand(imeiInsert);
                    throw new Exception("IMEI_REG_ERROR");
                }
            }



            //make all devices as not current login
            var row = dbContext.ExecuteSqlCommand($"UPDATE DIST_LOGIN_DEVICE SET CURRENT_LOGIN='N' WHERE USERID='{result.USER_ID}' AND IMEI_NO !='{model.Imei}'");

            //update App version
            row = dbContext.ExecuteSqlCommand($"UPDATE DIST_LOGIN_DEVICE SET APP_VERSION='{model.App_Version}',FIREBASE_ID='{model.Firebase_key}',CURRENT_LOGIN='Y',INSTALLED_APPS='{model.Installed_Apps}' WHERE USERID='{result.USER_ID}' AND IMEI_NO='{model.Imei}'");

            //inserting location
            //var insertQuery = $@"INSERT INTO DIST_LM_LOCATION_TRACKING (SP_CODE, SUBMIT_DATE, LATITUDE, LONGITUDE,COMPANY_CODE,BRANCH_CODE,TRACK_TYPE) VALUES 
            //                ('{result.SP_CODE}',SYSDATE,'{model.latitude}','{model.longitude}','{result.COMPANY_CODE}','{result.BRANCH_CODE}','TRK')";
            //row = dbContext.ExecuteSqlCommand(insertQuery);
            //inserting location

            var compQuery = $@"SELECT CC.COMPANY_CODE, INITCAP(TRIM(CS.COMPANY_EDESC)) COMPANY_EDESC,
                            BC.BRANCH_CODE, TRIM(BS.BRANCH_EDESC) BRANCH_EDESC,
                            TO_CHAR(PS.FY_START_DATE, 'YYYY-MM-DD') AS FISCAL_START, TO_CHAR(PS.FY_END_DATE, 'YYYY-MM-DD') AS FISCAL_END
                    FROM SC_APPLICATION_USERS AU
                    LEFT JOIN SC_COMPANY_CONTROL CC ON CC.USER_NO = AU.USER_NO
                    INNER JOIN COMPANY_SETUP CS ON CS.COMPANY_CODE = CC.COMPANY_CODE
                    LEFT JOIN SC_BRANCH_CONTROL BC ON BC.USER_NO = AU.USER_NO AND BC.COMPANY_CODE = CC.COMPANY_CODE
                    INNER JOIN FA_BRANCH_SETUP BS ON BS.BRANCH_CODE = BC.BRANCH_CODE AND BS.COMPANY_CODE = BC.COMPANY_CODE AND BS.GROUP_SKU_FLAG = 'I' AND BS.DELETED_FLAG = 'N'
                    LEFT JOIN (SELECT DISTINCT COMPANY_CODE, FY_START_DATE, FY_END_DATE FROM PREFERENCE_SETUP) PS ON PS.COMPANY_CODE = CC.COMPANY_CODE
                    WHERE 1 = 1
                      AND AU.EMPLOYEE_CODE = '{result.SP_CODE}'
                       AND AU.DELETED_FLAG='N'
                    GROUP BY CC.COMPANY_CODE, INITCAP(TRIM(CS.COMPANY_EDESC)),
                              BC.BRANCH_CODE, TRIM(BS.BRANCH_EDESC),
                              TO_CHAR(PS.FY_START_DATE, 'YYYY-MM-DD'), TO_CHAR(PS.FY_END_DATE, 'YYYY-MM-DD')
                    ORDER BY '', INITCAP(TRIM(CS.COMPANY_EDESC)),TRIM(BS.BRANCH_EDESC)";

            var allCompany = dbContext.SqlQuery<TempCompanyModel>(compQuery).GroupBy(x => x.COMPANY_CODE);
            var companies = new List<CompanyModel>();
            foreach (var group in allCompany)
            {
                var compTemp = group.FirstOrDefault();
                var comp = new CompanyModel()
                {
                    COMPANY_CODE = compTemp.COMPANY_CODE,
                    COMPANY_EDESC = compTemp.COMPANY_EDESC,
                    FISCAL_START = compTemp.FISCAL_START,
                    FISCAL_END = compTemp.FISCAL_END
                };
                foreach (var branch in group)
                {
                    comp.BRANCH.Add(branch.BRANCH_CODE, new BranchModel
                    {
                        BRANCH_CODE = branch.BRANCH_CODE,
                        BRANCH_EDESC = branch.BRANCH_EDESC
                    });
                }
                comp.PREFERENCE = FetchPreferences(comp.COMPANY_CODE, dbContext);
                result.COMPANY.Add(comp.COMPANY_CODE, comp);
            }

            var userAccessQuery = $@"SELECT 
    m.MENU_NO,
    m.MODULE_CODE,
    m.MENU_EDESC,
    m.MENU_NDESC,
    m.COMPANY_CODE,
    m.BRANCH_CODE,
    m.GROUP_SKU_FLAG,
    m.ICON,
    CASE
        WHEN NOT EXISTS (
            SELECT 1 
            FROM DIST_USER_ACCESS_SETUP u 
            WHERE u.USER_ID = '{result.USER_ID}'
              AND u.COMPANY_CODE = m.COMPANY_CODE
              AND u.BRANCH_CODE = m.BRANCH_CODE
        ) THEN m.ACCESS_FLAG
        WHEN m.GROUP_SKU_FLAG = 'I' AND EXISTS (
            SELECT 1 
            FROM DIST_USER_ACCESS_SETUP u_mod
            WHERE u_mod.USER_ID = '{result.USER_ID}'
              AND u_mod.COMPANY_CODE = m.COMPANY_CODE
              AND u_mod.BRANCH_CODE = m.BRANCH_CODE
              AND u_mod.MODULE_CODE = m.MODULE_CODE
              AND u_mod.MENU_NO = m.MENU_NO
              AND u_mod.ACCESS_FLAG = 'N'
        ) THEN 'N'
        WHEN m.GROUP_SKU_FLAG = 'G' AND EXISTS (
            SELECT 1 
            FROM DIST_USER_ACCESS_SETUP u_mod
            WHERE u_mod.USER_ID = '{result.USER_ID}'
              AND u_mod.COMPANY_CODE = m.COMPANY_CODE
              AND u_mod.BRANCH_CODE = m.BRANCH_CODE
              AND u_mod.MODULE_CODE = m.MODULE_CODE
              AND u_mod.MENU_NO = m.MENU_NO
              AND u_mod.ACCESS_FLAG = 'N'
        ) THEN 'N'
WHEN m.GROUP_SKU_FLAG = 'G' AND EXISTS (
            SELECT 1 
            FROM DIST_USER_ACCESS_SETUP u_mod
            WHERE u_mod.USER_ID = '{result.USER_ID}'
              AND u_mod.COMPANY_CODE = m.COMPANY_CODE
              AND u_mod.BRANCH_CODE = m.BRANCH_CODE
              AND u_mod.MODULE_CODE = m.MODULE_CODE
              AND u_mod.MENU_NO is null
              AND u_mod.ACCESS_FLAG = 'N'
        ) THEN 'N'
        WHEN u.ACCESS_FLAG IS NOT NULL THEN u.ACCESS_FLAG
        ELSE m.ACCESS_FLAG
    END AS ACCESS_FLAG
FROM 
    DIST_MODULE_SETUP m
LEFT JOIN 
    DIST_USER_ACCESS_SETUP u 
    ON u.USER_ID = '{result.USER_ID}'
   AND u.COMPANY_CODE = '{result.COMPANY_CODE}'
   AND u.COMPANY_CODE = m.COMPANY_CODE
   AND u.BRANCH_CODE = m.BRANCH_CODE
   AND u.MODULE_CODE = m.MODULE_CODE
   AND u.MENU_NO = m.MENU_NO order by module_code,menu_no";

            var rawAccess = dbContext.SqlQuery<UserAccessPayload>(userAccessQuery).ToList();

            var groupedAccess = rawAccess
                .GroupBy(x => x.MODULE_CODE)
                .Select(g =>
                {
                    var moduleItem = g.FirstOrDefault(x => x.MENU_NO == null);

                    return new UserAccessModule
                    {
                        MODULE_CODE = g.Key,
                        MODULE_NAME = moduleItem.MENU_EDESC,
                        ALTERNATE_NAME = moduleItem?.MENU_NDESC ?? "",
                        ACCESS_FLAG = moduleItem?.ACCESS_FLAG ?? "N",
                        ICON = "/Areas/NeoErp.Distribution/Images/Icons/" + moduleItem.ICON,
                        MENU = g
                            .Where(x => x.MENU_NO != null)
                            .Select(m => new UserAccessMenu
                            {
                                MENU_NO = m.MENU_NO,
                                ACCESS_FLAG = m.ACCESS_FLAG,
                                MENU_NAME = m.MENU_EDESC,
                                ALTERNATE_NAME = m.MENU_NDESC ?? "",
                                ICON = "/Areas/NeoErp.Distribution/Images/Icons/" + m.ICON
                            }).ToList()
                    };
                }).ToList();

            result.USER_ACCESS = groupedAccess;


            var list = new List<LoginResponseModel>();
            list.Add(result);
            return list;
        }

        public Dictionary<string, string> Logout(LogoutRequestModel model, NeoErpCoreEntity dbContext)
        {
            //string AttendanceQuery = $@"INSERT INTO HRIS_ATTENDANCE (EMPLOYEE_ID,ATTENDANCE_DT,ATTENDANCE_FROM,ATTENDANCE_TIME) VALUES ('{model.SP_CODE}',TRUNC(SYSDATE),'MOBILE',CURRENT_TIMESTAMP)";
            //var row = dbContext.ExecuteSqlCommand(AttendanceQuery);
            //var time = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss.fffffff tt", CultureInfo.InvariantCulture);
            //try
            //{
            //    var thumbId = dbContext.SqlQuery<string>($"SELECT ID_THUMB_ID FROM HRIS_EMPLOYEES WHERE EMPLOYEE_ID = '{model.SP_CODE}'").FirstOrDefault();
            //    if (!string.IsNullOrWhiteSpace(thumbId))
            //    {
            //        var hris_procedure = $"BEGIN HRIS_ATTENDANCE_INSERT ({thumbId}, TRUNC(SYSDATE), NULL, 'MOBILE', TO_TIMESTAMP('{time}')); END;";
            //        dbContext.ExecuteSqlCommand(hris_procedure);
            //    }
            //}
            //catch (Exception ex)
            //{
            //}
            //if (row <= 0)
            //    throw new Exception("Attendance could not be made!!!");
            //else
            //{
            //    Dictionary<string, string> result = new Dictionary<string, string>();
            //    result.Add("msg", "Your logout attendance has been made successfully");
            //    return result;
            //}
            Dictionary<string, string> result = new Dictionary<string, string>();
            result.Add("msg", "Your logout attendance has been made successfully");
            return result;
        }

        public Dictionary<string, VisitPlanResponseModel> GetVisitPlan(VisitPlanRequestModel model, NeoErpCoreEntity dbContext)
        {
            string Superuser;
            string today = string.Empty;
            if (string.IsNullOrEmpty(model.date))
                today = DateTime.Now.ToString("dd-MMM-yyyy");
            else
                today = model.date;
            Dictionary<string, VisitPlanResponseModel> Result = new Dictionary<string, VisitPlanResponseModel>();
            VisitPlanResponseModel VisitPlans = new VisitPlanResponseModel();

            if (string.IsNullOrEmpty(model.spcode as string))
                throw new Exception("User code is empty.");
            string VisitQuery = string.Empty;
            string UserQuery = $@"select super_user from dist_login_user where sp_code = '{model.spcode}'";
            var users = dbContext.SqlQuery<string>(UserQuery).ToList();

            if (users.Count == 0)
                throw new Exception("Invalid user");
            else
                Superuser = users[0];//for future use. if the user is super user, different query will be used

            VisitQuery = $@"SELECT * FROM (
                (SELECT
                DM.DEALER_CODE AS CODE,DM.EMAIL,
                PT.PARTY_TYPE_EDESC AS NAME,
                PT.ACC_CODE,
                DM.CONTACT_NO AS P_CONTACT_NO, DM.REG_OFFICE_ADDRESS AS ADDRESS,
                RM.ROUTE_CODE, TO_CHAR(RD.ASSIGN_DATE,'DD-MON-RRRR') AS ASSIGN_DATE, RM.ROUTE_NAME,
                AM.AREA_CODE, AM.AREA_NAME,
                TO_CHAR(RE.ORDER_NO) AS ORDER_NO,
                RM.COMPANY_CODE,
                'dealer' AS TYPE,
                'N' AS WHOLESELLER,
                '' DEFAULT_PARTY_TYPE_CODE,
                '' PARENT_DISTRIBUTOR_CODE,
                '' PARENT_DISTRIBUTOR_NAME,
                NVL(DM.LATITUDE,0) LATITUDE, NVL(DM.LONGITUDE,0) LONGITUDE,
                LT.LAST_VISIT_DATE, LT.LAST_VISIT_BY, LT.LAST_VISIT_STATUS, NVL(LT.IS_VISITED, 'X') AS IS_VISITED, LT.REMARKS
                FROM DIST_ROUTE_MASTER RM
                INNER JOIN DIST_ROUTE_DETAIL RD ON RD.ROUTE_CODE = RM.ROUTE_CODE AND RD.COMPANY_CODE = RM.COMPANY_CODE
                INNER JOIN DIST_ROUTE_ENTITY RE ON RE.ROUTE_CODE = RD.ROUTE_CODE AND RE.ENTITY_TYPE = 'P' AND RE.COMPANY_CODE = RM.COMPANY_CODE
                INNER JOIN DIST_DEALER_MASTER DM ON DM.DEALER_CODE = RE.ENTITY_CODE AND DM.COMPANY_CODE = RM.COMPANY_CODE
                INNER JOIN IP_PARTY_TYPE_CODE PT ON PT.PARTY_TYPE_CODE = DM.DEALER_CODE AND PT.COMPANY_CODE = DM.COMPANY_CODE
                INNER JOIN DIST_AREA_MASTER AM ON AM.AREA_CODE = DM.AREA_CODE AND AM.COMPANY_CODE = DM.COMPANY_CODE
                LEFT JOIN (SELECT A.SP_CODE, A.CUSTOMER_CODE, A.CUSTOMER_TYPE, A.COMPANY_CODE, C.EMPLOYEE_EDESC AS LAST_VISIT_BY, A.IS_VISITED AS LAST_VISIT_STATUS, 
                          (CASE 
                            WHEN A.IS_VISITED IS NULL THEN 'X' 
                              ELSE
                                CASE WHEN TO_CHAR(SYSDATE, 'DD-MON-RRRR') = TO_CHAR(A.UPDATE_DATE, 'DD-MON-RRRR') THEN A.IS_VISITED ELSE 'X' END
                           END
                          ) IS_VISITED, 
                          A.REMARKS,
                          TO_CHAR(MAX(A.UPDATE_DATE), 'RRRR-MM-DD HH24:MI A.M.') LAST_VISIT_DATE
                          FROM DIST_LOCATION_TRACK A
                          INNER JOIN IP_PARTY_TYPE_CODE B ON B.PARTY_TYPE_CODE = A.CUSTOMER_CODE AND B.COMPANY_CODE = A.COMPANY_CODE
                          INNER JOIN HR_EMPLOYEE_SETUP C ON C.EMPLOYEE_CODE = A.SP_CODE AND C.COMPANY_CODE = A.COMPANY_CODE
                          WHERE 1 = 1
                          -- AND SP_CODE = '1000097' -- Commented out because customer can be visited by another SP_CODE
                          AND A.CUSTOMER_TYPE = 'P'
                          AND A.UPDATE_DATE = (SELECT MAX(UPDATE_DATE) FROM DIST_LOCATION_TRACK WHERE CUSTOMER_CODE = A.CUSTOMER_CODE AND CUSTOMER_TYPE = A.CUSTOMER_TYPE)
                          GROUP BY A.SP_CODE, A.CUSTOMER_CODE, A.CUSTOMER_TYPE, A.COMPANY_CODE, C.EMPLOYEE_EDESC, A.IS_VISITED, 
                              (CASE 
                                WHEN A.IS_VISITED IS NULL THEN 'X' 
                                  ELSE
                                    CASE WHEN TO_CHAR(SYSDATE, 'DD-MON-RRRR') = TO_CHAR(A.UPDATE_DATE, 'DD-MON-RRRR') THEN A.IS_VISITED ELSE 'X' END
                               END
                              ), 
                              A.REMARKS
                          ) LT
                          ON LT.CUSTOMER_CODE = DM.DEALER_CODE AND LT.COMPANY_CODE = RM.COMPANY_CODE
                WHERE 1 = 1
                AND TO_CHAR(RD.ASSIGN_DATE, 'DD-MON-RRRR') = UPPER('{today}')
                AND RD.EMP_CODE = '{model.spcode}'
                AND RD.COMPANY_CODE = '{model.COMPANY_CODE}'
                )
              UNION
                (SELECT
                DM.DISTRIBUTOR_CODE AS CODE,DM.EMAIL,
                CS.CUSTOMER_EDESC AS NAME,
                '' AS ACC_CODE,
                DM.CONTACT_NO AS P_CONTACT_NO, NVL(DM.REG_OFFICE_ADDRESS, CS.REGD_OFFICE_EADDRESS) AS ADDRESS,
                RM.ROUTE_CODE, TO_CHAR(RD.ASSIGN_DATE,'DD-MON-RRRR') AS ASSIGN_DATE, RM.ROUTE_NAME,
                AM.AREA_CODE, AM.AREA_NAME,
                TO_CHAR(RE.ORDER_NO) AS ORDER_NO,
                RM.COMPANY_CODE,
                'distributor' AS TYPE,
                'N' AS WHOLESELLER,
                CS.PARTY_TYPE_CODE AS DEFAULT_PARTY_TYPE_CODE,
                '' PARENT_DISTRIBUTOR_CODE,
                '' PARENT_DISTRIBUTOR_NAME,
                NVL(DM.LATITUDE,0) LATITUDE, NVL(DM.LONGITUDE,0) LONGITUDE,
                LT.LAST_VISIT_DATE, LT.LAST_VISIT_BY, LT.LAST_VISIT_STATUS, NVL(LT.IS_VISITED, 'X') AS IS_VISITED, LT.REMARKS
                FROM DIST_ROUTE_MASTER RM
                INNER JOIN DIST_ROUTE_DETAIL RD ON RD.ROUTE_CODE = RM.ROUTE_CODE AND RD.COMPANY_CODE = RM.COMPANY_CODE
                INNER JOIN DIST_ROUTE_ENTITY RE ON RE.ROUTE_CODE = RD.ROUTE_CODE AND RE.ENTITY_TYPE = 'D' AND RE.COMPANY_CODE = RM.COMPANY_CODE
                INNER JOIN DIST_DISTRIBUTOR_MASTER DM ON DM.DISTRIBUTOR_CODE = RE.ENTITY_CODE AND DM.COMPANY_CODE = RM.COMPANY_CODE
                INNER JOIN SA_CUSTOMER_SETUP CS ON CS.CUSTOMER_CODE = DM.DISTRIBUTOR_CODE AND CS.COMPANY_CODE = DM.COMPANY_CODE
                INNER JOIN DIST_AREA_MASTER AM ON AM.AREA_CODE = DM.AREA_CODE AND AM.COMPANY_CODE = DM.COMPANY_CODE
                LEFT JOIN (SELECT A.SP_CODE, A.CUSTOMER_CODE, A.CUSTOMER_TYPE, A.COMPANY_CODE, C.EMPLOYEE_EDESC AS LAST_VISIT_BY, A.IS_VISITED AS LAST_VISIT_STATUS, 
                          (CASE 
                            WHEN A.IS_VISITED IS NULL THEN 'X' 
                              ELSE
                                CASE WHEN TO_CHAR(SYSDATE, 'DD-MON-RRRR') = TO_CHAR(A.UPDATE_DATE, 'DD-MON-RRRR') THEN A.IS_VISITED ELSE 'X' END
                           END
                          ) IS_VISITED, 
                          A.REMARKS,
                          TO_CHAR(MAX(A.UPDATE_DATE), 'RRRR-MM-DD HH24:MI A.M.') LAST_VISIT_DATE
                          FROM DIST_LOCATION_TRACK A
                          INNER JOIN SA_CUSTOMER_SETUP B ON B.CUSTOMER_CODE = A.CUSTOMER_CODE AND B.COMPANY_CODE = A.COMPANY_CODE
                          INNER JOIN HR_EMPLOYEE_SETUP C ON C.EMPLOYEE_CODE = A.SP_CODE AND C.COMPANY_CODE = A.COMPANY_CODE
                          WHERE 1 = 1
                          -- AND SP_CODE = '1000097' -- Commented out because customer can be visited by another SP_CODE
                          AND A.CUSTOMER_TYPE = 'D'
                          AND A.UPDATE_DATE = (SELECT MAX(UPDATE_DATE) FROM DIST_LOCATION_TRACK WHERE CUSTOMER_CODE = A.CUSTOMER_CODE AND CUSTOMER_TYPE = A.CUSTOMER_TYPE)
                          GROUP BY A.SP_CODE, A.CUSTOMER_CODE, A.CUSTOMER_TYPE, A.COMPANY_CODE, C.EMPLOYEE_EDESC, A.IS_VISITED, 
                              (CASE 
                                WHEN A.IS_VISITED IS NULL THEN 'X' 
                                  ELSE
                                    CASE WHEN TO_CHAR(SYSDATE, 'DD-MON-RRRR') = TO_CHAR(A.UPDATE_DATE, 'DD-MON-RRRR') THEN A.IS_VISITED ELSE 'X' END
                               END
                              ), 
                              A.REMARKS
                          ) LT
                          ON LT.CUSTOMER_CODE = DM.DISTRIBUTOR_CODE AND LT.COMPANY_CODE = RM.COMPANY_CODE
                WHERE 1 = 1
                AND TO_CHAR(RD.ASSIGN_DATE, 'DD-MON-RRRR') = UPPER('{today}')
                AND RD.EMP_CODE = '{model.spcode}'
                AND RD.COMPANY_CODE = '{model.COMPANY_CODE}'
                )
            UNION
                (SELECT
                RES.RESELLER_CODE AS CODE,RES.EMAIL,
                RES.RESELLER_NAME AS NAME,
                '' AS ACC_CODE,
                RES.CONTACT_NO AS P_CONTACT_NO, RES.REG_OFFICE_ADDRESS AS ADDRESS,
                RM.ROUTE_CODE, TO_CHAR(RD.ASSIGN_DATE,'DD-MON-RRRR') AS ASSIGN_DATE, RM.ROUTE_NAME,
                AM.AREA_CODE, AM.AREA_NAME,
                TO_CHAR(RE.ORDER_NO) AS ORDER_NO,
                RM.COMPANY_CODE,
                'reseller' AS TYPE,
                RES.WHOLESELLER AS WHOLESELLER,
                '' AS DEFAULT_PARTY_TYPE_CODE,
                DISTRIBUTOR_CODE AS PARENT_DISTRIBUTOR_CODE,
                CS.CUSTOMER_EDESC AS PARENT_DISTRIBUTOR_NAME,
                NVL(RES.LATITUDE,0) LATITUDE, NVL(RES.LONGITUDE,0) LONGITUDE,
                LT.LAST_VISIT_DATE, LT.LAST_VISIT_BY, LT.LAST_VISIT_STATUS, NVL(LT.IS_VISITED, 'X') AS IS_VISITED, LT.REMARKS               
                FROM DIST_ROUTE_MASTER RM
                INNER JOIN DIST_ROUTE_DETAIL RD ON RD.ROUTE_CODE = RM.ROUTE_CODE AND RD.COMPANY_CODE = RM.COMPANY_CODE
                INNER JOIN DIST_ROUTE_ENTITY RE ON RE.ROUTE_CODE = RD.ROUTE_CODE AND RE.ENTITY_TYPE = 'R' AND RE.COMPANY_CODE = RM.COMPANY_CODE
                INNER JOIN DIST_RESELLER_MASTER RES ON RES.RESELLER_CODE = RE.ENTITY_CODE AND RES.COMPANY_CODE = RM.COMPANY_CODE
                LEFT JOIN DIST_AREA_MASTER AM ON AM.AREA_CODE = RES.AREA_CODE AND AM.COMPANY_CODE = RM.COMPANY_CODE
                LEFT JOIN SA_CUSTOMER_SETUP CS ON CS.CUSTOMER_CODE = RES.DISTRIBUTOR_CODE AND CS.COMPANY_CODE = RM.COMPANY_CODE
                LEFT JOIN (SELECT A.SP_CODE, A.CUSTOMER_CODE, A.CUSTOMER_TYPE, A.COMPANY_CODE, C.EMPLOYEE_EDESC AS LAST_VISIT_BY, A.IS_VISITED AS LAST_VISIT_STATUS, 
                          (CASE 
                            WHEN A.IS_VISITED IS NULL THEN 'X' 
                              ELSE
                                CASE WHEN TO_CHAR(SYSDATE, 'DD-MON-RRRR') = TO_CHAR(A.UPDATE_DATE, 'DD-MON-RRRR') THEN A.IS_VISITED ELSE 'X' END
                           END
                          ) IS_VISITED, 
                          A.REMARKS,
                          TO_CHAR(MAX(A.UPDATE_DATE), 'RRRR-MM-DD HH24:MI A.M.') LAST_VISIT_DATE
                          FROM DIST_LOCATION_TRACK A
                          INNER JOIN DIST_RESELLER_MASTER B ON B.RESELLER_CODE = A.CUSTOMER_CODE AND B.COMPANY_CODE = A.COMPANY_CODE
                          INNER JOIN HR_EMPLOYEE_SETUP C ON C.EMPLOYEE_CODE = A.SP_CODE AND C.COMPANY_CODE = A.COMPANY_CODE
                          WHERE 1 = 1
                          AND A.CUSTOMER_TYPE = 'R'
                          AND A.UPDATE_DATE = (SELECT MAX(UPDATE_DATE) FROM DIST_LOCATION_TRACK WHERE CUSTOMER_CODE = A.CUSTOMER_CODE AND CUSTOMER_TYPE = A.CUSTOMER_TYPE)
                          GROUP BY A.SP_CODE, A.CUSTOMER_CODE, A.CUSTOMER_TYPE, A.COMPANY_CODE, C.EMPLOYEE_EDESC, A.IS_VISITED, 
                              (CASE 
                                WHEN A.IS_VISITED IS NULL THEN 'X' 
                                  ELSE
                                    CASE WHEN TO_CHAR(SYSDATE, 'DD-MON-RRRR') = TO_CHAR(A.UPDATE_DATE, 'DD-MON-RRRR') THEN A.IS_VISITED ELSE 'X' END
                               END
                              ), 
                              A.REMARKS
                          ) LT
                          ON LT.CUSTOMER_CODE = RES.RESELLER_CODE AND LT.COMPANY_CODE = RM.COMPANY_CODE
                WHERE 1 = 1
                AND TO_CHAR(RD.ASSIGN_DATE, 'DD-MON-RRRR') = UPPER('{today}')
                AND RD.EMP_CODE = '{model.spcode}'
                AND RD.COMPANY_CODE = '{model.COMPANY_CODE}'
                )
            )
            ORDER BY UPPER(ROUTE_NAME), ORDER_NO, UPPER(AREA_NAME), UPPER(NAME), LAST_VISIT_DATE DESC";


            var Visitlist = dbContext.SqlQuery<VisitEntityModel>(VisitQuery).ToList();
            if (Visitlist.Count <= 0)
                throw new Exception("No records found");

            var RouteCode = Visitlist[0].Route_Code;
            var RouteName = Visitlist[0].Route_Name;
            foreach (var visit in Visitlist)
            {
                var code = visit.Code;
                VisitPlans.entity.Add(code, visit);
            }
            VisitPlans.code = RouteCode;
            Result.Add(RouteName, VisitPlans);

            return Result;
        }

        public Dictionary<string, List<EntityResponseModel>> FetchEntity(CommonRequestModel model, NeoErpCoreEntity dbContext)
        {
            var result = new Dictionary<string, List<EntityResponseModel>>();
            string EntityQuery = string.Empty;
            EntityQuery = $@"SELECT * FROM (
                  (SELECT
                     DM.DEALER_CODE AS CODE,
                     PT.PARTY_TYPE_EDESC AS NAME,
                     PT.ACC_CODE,
                     DM.CONTACT_NO AS P_CONTACT_NO, '' AS P_CONTACT_NAME, DM.REG_OFFICE_ADDRESS AS ADDRESS,DM.EMAIL,
                     RM.ROUTE_CODE, RM.ROUTE_NAME,
                     AM.AREA_CODE, AM.AREA_NAME,
                     NVL(DM.LATITUDE,0) LATITUDE, NVL(DM.LONGITUDE,0) LONGITUDE,
                     'dealer' AS TYPE,
                     'N' AS WHOLESELLER,
                     '' DEFAULT_PARTY_TYPE_CODE,
                     '' PARENT_DISTRIBUTOR_CODE,
                     '' PARENT_DISTRIBUTOR_NAME,
                     LT.LAST_VISIT_DATE, LT.LAST_VISIT_BY, LT.LAST_VISIT_STATUS, NVL(LT.IS_VISITED, 'X') AS IS_VISITED, LT.REMARKS,
                     DM.COMPANY_CODE, DM.BRANCH_CODE,'' TYPE_EDESC,'' SUBTYPE_EDESC
                   FROM
                     DIST_DEALER_MASTER DM
                     LEFT JOIN DIST_ROUTE_ENTITY RE ON RE.ENTITY_CODE = DM.DEALER_CODE AND RE.ENTITY_TYPE = 'P' AND RE.COMPANY_CODE = DM.COMPANY_CODE
                     LEFT JOIN DIST_ROUTE_MASTER RM ON RM.ROUTE_CODE = RE.ROUTE_CODE AND RM.COMPANY_CODE = DM.COMPANY_CODE
                     INNER JOIN IP_PARTY_TYPE_CODE PT ON PT.PARTY_TYPE_CODE = DM.DEALER_CODE AND PT.COMPANY_CODE = DM.COMPANY_CODE
                     INNER JOIN DIST_AREA_MASTER AM ON AM.AREA_CODE = DM.AREA_CODE AND AM.COMPANY_CODE = DM.COMPANY_CODE
                     LEFT JOIN (SELECT A.SP_CODE, A.CUSTOMER_CODE, A.CUSTOMER_TYPE, A.COMPANY_CODE, C.EMPLOYEE_EDESC AS LAST_VISIT_BY, A.IS_VISITED AS LAST_VISIT_STATUS,
                                  (CASE 
                                    WHEN A.IS_VISITED IS NULL THEN 'X' 
                                      ELSE
                                        CASE WHEN TO_CHAR(SYSDATE, 'DD-MON-RRRR') = TO_CHAR(A.UPDATE_DATE, 'DD-MON-RRRR') THEN A.IS_VISITED ELSE 'X' END
                                   END
                                  ) IS_VISITED, 
                                  A.REMARKS,
                                  TO_CHAR(MAX(A.UPDATE_DATE), 'RRRR-MM-DD HH24:MI A.M.') LAST_VISIT_DATE
                                FROM DIST_LOCATION_TRACK A
                                  INNER JOIN IP_PARTY_TYPE_CODE B ON B.PARTY_TYPE_CODE = A.CUSTOMER_CODE AND B.COMPANY_CODE = A.COMPANY_CODE
                                  INNER JOIN HR_EMPLOYEE_SETUP C ON C.EMPLOYEE_CODE = A.SP_CODE AND C.COMPANY_CODE = A.COMPANY_CODE
                                WHERE 1 = 1
                                      AND A.CUSTOMER_TYPE = 'P'
                                      AND A.UPDATE_DATE = (SELECT MAX(UPDATE_DATE) FROM DIST_LOCATION_TRACK WHERE CUSTOMER_CODE = A.CUSTOMER_CODE AND CUSTOMER_TYPE = A.CUSTOMER_TYPE)
                                GROUP BY A.SP_CODE, A.CUSTOMER_CODE, A.CUSTOMER_TYPE, A.COMPANY_CODE, C.EMPLOYEE_EDESC, A.IS_VISITED, 
                                  (CASE 
                                    WHEN A.IS_VISITED IS NULL THEN 'X' 
                                      ELSE
                                        CASE WHEN TO_CHAR(SYSDATE, 'DD-MON-RRRR') = TO_CHAR(A.UPDATE_DATE, 'DD-MON-RRRR') THEN A.IS_VISITED ELSE 'X' END
                                   END
                                  ), 
                                  A.REMARKS
                               ) LT
                       ON LT.CUSTOMER_CODE = DM.DEALER_CODE AND LT.COMPANY_CODE = DM.COMPANY_CODE
                   WHERE 1 = 1
                         AND DM.COMPANY_CODE = '{model.COMPANY_CODE}'
                  )
                UNION
                  (SELECT
                     DM.DISTRIBUTOR_CODE AS CODE,
                     CS.CUSTOMER_EDESC AS NAME,
                     CS.ACC_CODE,
                     DM.CONTACT_NO AS P_CONTACT_NO, '' AS P_CONTACT_NAME, NVL(DM.REG_OFFICE_ADDRESS, CS.REGD_OFFICE_EADDRESS) AS ADDRESS,DM.EMAIL,
                     RM.ROUTE_CODE, RM.ROUTE_NAME,
                     AM.AREA_CODE, AM.AREA_NAME,
                     NVL(DM.LATITUDE,0) LATITUDE, NVL(DM.LONGITUDE,0) LONGITUDE,
                     'distributor' AS TYPE,
                     'N' AS WHOLESELLER,
                     CS.PARTY_TYPE_CODE AS DEFAULT_PARTY_TYPE_CODE,
                     '' PARENT_DISTRIBUTOR_CODE,
                     '' PARENT_DISTRIBUTOR_NAME,
                     LT.LAST_VISIT_DATE, LT.LAST_VISIT_BY, LT.LAST_VISIT_STATUS, NVL(LT.IS_VISITED, 'X') AS IS_VISITED, LT.REMARKS,
                     DM.COMPANY_CODE, DM.BRANCH_CODE,'' TYPE_EDESC,'' SUBTYPE_EDESC
                   FROM
                     DIST_DISTRIBUTOR_MASTER DM
                     LEFT JOIN DIST_ROUTE_ENTITY RE ON RE.ENTITY_CODE = DM.DISTRIBUTOR_CODE AND RE.ENTITY_TYPE = 'D' AND RE.COMPANY_CODE = DM.COMPANY_CODE
                     LEFT JOIN DIST_ROUTE_MASTER RM ON RM.ROUTE_CODE = RE.ROUTE_CODE AND RM.COMPANY_CODE = DM.COMPANY_CODE
                     INNER JOIN SA_CUSTOMER_SETUP CS ON CS.CUSTOMER_CODE = DM.DISTRIBUTOR_CODE AND CS.COMPANY_CODE = DM.COMPANY_CODE
                     INNER JOIN DIST_AREA_MASTER AM ON AM.AREA_CODE = DM.AREA_CODE AND AM.COMPANY_CODE = DM.COMPANY_CODE
                     LEFT JOIN (SELECT A.SP_CODE, A.CUSTOMER_CODE, A.CUSTOMER_TYPE, A.COMPANY_CODE, C.EMPLOYEE_EDESC AS LAST_VISIT_BY, A.IS_VISITED AS LAST_VISIT_STATUS,
                                  (CASE 
                                    WHEN A.IS_VISITED IS NULL THEN 'X' 
                                      ELSE
                                        CASE WHEN TO_CHAR(SYSDATE, 'DD-MON-RRRR') = TO_CHAR(A.UPDATE_DATE, 'DD-MON-RRRR') THEN A.IS_VISITED ELSE 'X' END
                                   END
                                  ) IS_VISITED, 
                                  A.REMARKS,
                                  TO_CHAR(MAX(A.UPDATE_DATE), 'RRRR-MM-DD HH24:MI A.M.') LAST_VISIT_DATE
                                FROM DIST_LOCATION_TRACK A
                                  INNER JOIN SA_CUSTOMER_SETUP B ON B.CUSTOMER_CODE = A.CUSTOMER_CODE AND B.COMPANY_CODE = A.COMPANY_CODE
                                  INNER JOIN HR_EMPLOYEE_SETUP C ON C.EMPLOYEE_CODE = A.SP_CODE AND C.COMPANY_CODE = A.COMPANY_CODE
                                WHERE 1 = 1
                                      AND A.CUSTOMER_TYPE = 'D'
                                      AND A.UPDATE_DATE = (SELECT MAX(UPDATE_DATE) FROM DIST_LOCATION_TRACK WHERE CUSTOMER_CODE = A.CUSTOMER_CODE AND CUSTOMER_TYPE = A.CUSTOMER_TYPE)
                                GROUP BY A.SP_CODE, A.CUSTOMER_CODE, A.CUSTOMER_TYPE, A.COMPANY_CODE, C.EMPLOYEE_EDESC, A.IS_VISITED, 
                                  (CASE 
                                    WHEN A.IS_VISITED IS NULL THEN 'X' 
                                      ELSE
                                        CASE WHEN TO_CHAR(SYSDATE, 'DD-MON-RRRR') = TO_CHAR(A.UPDATE_DATE, 'DD-MON-RRRR') THEN A.IS_VISITED ELSE 'X' END
                                   END
                                  ), 
                                  A.REMARKS
                               ) LT
                       ON LT.CUSTOMER_CODE = DM.DISTRIBUTOR_CODE AND LT.COMPANY_CODE = DM.COMPANY_CODE
                   WHERE 1 = 1
                         AND DM.COMPANY_CODE = '{model.COMPANY_CODE}'
                  )
                  UNION
                  (SELECT
                     REM.RESELLER_CODE AS CODE,
                     REM.RESELLER_NAME AS NAME,
                     '' AS ACC_CODE,
                     REM.CONTACT_NO AS P_CONTACT_NO, REM.CONTACT_NAME AS P_CONTACT_NAME, REM.REG_OFFICE_ADDRESS AS ADDRESS,REM.EMAIL,
                     RM.ROUTE_CODE, RM.ROUTE_NAME,
                     AM.AREA_CODE, AM.AREA_NAME,
                     NVL(REM.LATITUDE,0) LATITUDE, NVL(REM.LONGITUDE,0) LONGITUDE,
                     'reseller' AS TYPE,
                     REM.WHOLESELLER AS WHOLESELLER,
                     '' AS DEFAULT_PARTY_TYPE_CODE,
                     DISTRIBUTOR_CODE AS PARENT_DISTRIBUTOR_CODE,
                     CS.CUSTOMER_EDESC AS PARENT_DISTRIBUTOR_NAME,
                     LT.LAST_VISIT_DATE, LT.LAST_VISIT_BY, LT.LAST_VISIT_STATUS, NVL(LT.IS_VISITED, 'X') AS IS_VISITED, LT.REMARKS,
                     REM.COMPANY_CODE, REM.BRANCH_CODE,
                     DOT.TYPE_EDESC,DOS.SUBTYPE_EDESC
                   FROM DIST_RESELLER_MASTER REM

                     LEFT JOIN DIST_ROUTE_ENTITY RE ON RE.ENTITY_CODE = REM.RESELLER_CODE AND RE.ENTITY_TYPE = 'R' AND RE.COMPANY_CODE = REM.COMPANY_CODE
                     LEFT JOIN DIST_ROUTE_MASTER RM ON RM.ROUTE_CODE = RE.ROUTE_CODE AND RM.COMPANY_CODE = REM.COMPANY_CODE
                     INNER JOIN DIST_AREA_MASTER AM ON AM.AREA_CODE = REM.AREA_CODE AND AM.COMPANY_CODE = REM.COMPANY_CODE
                     LEFT JOIN SA_CUSTOMER_SETUP CS ON CS.CUSTOMER_CODE = REM.DISTRIBUTOR_CODE AND CS.COMPANY_CODE = REM.COMPANY_CODE
                     LEFT JOIN (SELECT A.SP_CODE, A.CUSTOMER_CODE, A.CUSTOMER_TYPE, A.COMPANY_CODE, C.EMPLOYEE_EDESC AS LAST_VISIT_BY, A.IS_VISITED AS LAST_VISIT_STATUS,
                                 (CASE 
                                        WHEN A.IS_VISITED IS NULL THEN 'X' 
                                          ELSE
                                            CASE WHEN TO_CHAR(SYSDATE, 'DD-MON-RRRR') = TO_CHAR(A.UPDATE_DATE, 'DD-MON-RRRR') THEN A.IS_VISITED ELSE 'X' END
                                       END
                                      ) IS_VISITED, 
                                 A.REMARKS,
                                 TO_CHAR(MAX(A.UPDATE_DATE), 'RRRR-MM-DD HH24:MI A.M.') LAST_VISIT_DATE
                                FROM DIST_LOCATION_TRACK A
                                  INNER JOIN DIST_RESELLER_MASTER B ON B.RESELLER_CODE = A.CUSTOMER_CODE AND B.COMPANY_CODE = A.COMPANY_CODE
                                  INNER JOIN HR_EMPLOYEE_SETUP C ON C.EMPLOYEE_CODE = A.SP_CODE AND C.COMPANY_CODE = A.COMPANY_CODE
                                WHERE 1 = 1
                                      AND A.CUSTOMER_TYPE = 'R'
                                      AND A.UPDATE_DATE = (SELECT MAX(UPDATE_DATE) FROM DIST_LOCATION_TRACK WHERE CUSTOMER_CODE = A.CUSTOMER_CODE AND CUSTOMER_TYPE = A.CUSTOMER_TYPE)
                                GROUP BY A.SP_CODE, A.CUSTOMER_CODE, A.CUSTOMER_TYPE, A.COMPANY_CODE, C.EMPLOYEE_EDESC, A.IS_VISITED, 
                                  (CASE 
                                    WHEN A.IS_VISITED IS NULL THEN 'X' 
                                      ELSE
                                        CASE WHEN TO_CHAR(SYSDATE, 'DD-MON-RRRR') = TO_CHAR(A.UPDATE_DATE, 'DD-MON-RRRR') THEN A.IS_VISITED ELSE 'X' END
                                   END
                                  ), 
                                  A.REMARKS
                               ) LT
                       ON LT.CUSTOMER_CODE = REM.RESELLER_CODE AND LT.COMPANY_CODE = REM.COMPANY_CODE
               LEFT JOIN DIST_OUTLET_TYPE DOT ON REM.OUTLET_TYPE_ID=DOT.TYPE_ID AND REM.COMPANY_CODE=DOT.COMPANY_CODE
               LEFT JOIN DIST_OUTLET_SUBTYPE DOS ON REM.OUTLET_SUBTYPE_ID=DOS.SUBTYPE_ID AND REM.COMPANY_CODE=DOS.COMPANY_CODE
                   WHERE 1 = 1
                         AND REM.COMPANY_CODE = '{model.COMPANY_CODE}'
                  )
                )
                ORDER BY UPPER(NAME)";
            var Entities = dbContext.SqlQuery<EntityResponseModel>(EntityQuery).GroupBy(x => x.TYPE);
            if (Entities.Count() <= 0)
                throw new Exception("No records found");
            foreach (var EntGroup in Entities)
            {
                result.Add(EntGroup.Key, EntGroup.ToList());
            }
            return (result);
        }

        public Dictionary<string, List<EntityResponseModel>> FetchAllCompanyEntity(NeoErpCoreEntity dbContext)
        {
            var result = new Dictionary<string, List<EntityResponseModel>>();
            string EntityQuery = string.Empty;
            EntityQuery = $@"SELECT * FROM (
                  (SELECT
                     DM.DEALER_CODE AS CODE,
                     TRIM(PT.PARTY_TYPE_EDESC) AS NAME,
                     PT.ACC_CODE,
                     DM.CONTACT_NO AS P_CONTACT_NO, '' AS P_CONTACT_NAME, DM.REG_OFFICE_ADDRESS AS ADDRESS,
                     --RM.ROUTE_CODE, RM.ROUTE_NAME,
                     AM.AREA_CODE, AM.AREA_NAME,
                     NVL(DM.LATITUDE,0) LATITUDE, NVL(DM.LONGITUDE,0) LONGITUDE,
                     'dealer' AS TYPE,
                     'N' AS WHOLESELLER,
                     '' DEFAULT_PARTY_TYPE_CODE,
                     '' PARENT_DISTRIBUTOR_CODE,
                     '' PARENT_DISTRIBUTOR_NAME,
                     LT.LAST_VISIT_DATE, LT.LAST_VISIT_BY, LT.LAST_VISIT_STATUS, NVL(LT.IS_VISITED, 'X') AS IS_VISITED, LT.REMARKS,
                     DM.COMPANY_CODE, DM.BRANCH_CODE,'' TYPE_EDESC,'' SUBTYPE_EDESC
                     FROM DIST_DEALER_MASTER DM
                     LEFT JOIN DIST_ROUTE_ENTITY RE ON RE.ENTITY_CODE = DM.DEALER_CODE AND RE.ENTITY_TYPE = 'P' AND RE.COMPANY_CODE = DM.COMPANY_CODE
                     LEFT JOIN DIST_ROUTE_MASTER RM ON RM.ROUTE_CODE = RE.ROUTE_CODE AND RM.COMPANY_CODE = DM.COMPANY_CODE
                     INNER JOIN IP_PARTY_TYPE_CODE PT ON PT.PARTY_TYPE_CODE = DM.DEALER_CODE AND PT.COMPANY_CODE = DM.COMPANY_CODE
                     INNER JOIN DIST_AREA_MASTER AM ON AM.AREA_CODE = DM.AREA_CODE AND AM.COMPANY_CODE = DM.COMPANY_CODE
                     LEFT JOIN (SELECT A.SP_CODE, A.CUSTOMER_CODE, A.CUSTOMER_TYPE, A.COMPANY_CODE, C.EMPLOYEE_EDESC AS LAST_VISIT_BY, A.IS_VISITED AS LAST_VISIT_STATUS,
                                  (CASE 
                                    WHEN A.IS_VISITED IS NULL THEN 'X' 
                                      ELSE
                                        CASE WHEN TO_CHAR(SYSDATE, 'DD-MON-RRRR') = TO_CHAR(A.UPDATE_DATE, 'DD-MON-RRRR') THEN A.IS_VISITED ELSE 'X' END
                                   END
                                  ) IS_VISITED, 
                                  A.REMARKS,
                                  TO_CHAR(MAX(A.UPDATE_DATE), 'RRRR-MM-DD HH24:MI A.M.') LAST_VISIT_DATE
                                FROM DIST_LOCATION_TRACK A
                                  INNER JOIN IP_PARTY_TYPE_CODE B ON B.PARTY_TYPE_CODE = A.CUSTOMER_CODE AND B.COMPANY_CODE = A.COMPANY_CODE
                                  INNER JOIN HR_EMPLOYEE_SETUP C ON C.EMPLOYEE_CODE = A.SP_CODE AND C.COMPANY_CODE = A.COMPANY_CODE
                                WHERE 1 = 1
                                      AND A.CUSTOMER_TYPE = 'P'
                                      AND A.UPDATE_DATE = (SELECT MAX(UPDATE_DATE) FROM DIST_LOCATION_TRACK WHERE CUSTOMER_CODE = A.CUSTOMER_CODE AND CUSTOMER_TYPE = A.CUSTOMER_TYPE)
                                GROUP BY A.SP_CODE, A.CUSTOMER_CODE, A.CUSTOMER_TYPE, A.COMPANY_CODE, C.EMPLOYEE_EDESC, A.IS_VISITED, 
                                  (CASE 
                                    WHEN A.IS_VISITED IS NULL THEN 'X' 
                                      ELSE
                                        CASE WHEN TO_CHAR(SYSDATE, 'DD-MON-RRRR') = TO_CHAR(A.UPDATE_DATE, 'DD-MON-RRRR') THEN A.IS_VISITED ELSE 'X' END
                                   END
                                  ), 
                                  A.REMARKS
                               ) LT
                       ON LT.CUSTOMER_CODE = DM.DEALER_CODE AND LT.COMPANY_CODE = DM.COMPANY_CODE)
                UNION
                  (SELECT
                     DM.DISTRIBUTOR_CODE AS CODE,
                     TRIM(CS.CUSTOMER_EDESC) AS NAME,
                     CS.ACC_CODE,
                     DM.CONTACT_NO AS P_CONTACT_NO, '' AS P_CONTACT_NAME, NVL(DM.REG_OFFICE_ADDRESS, CS.REGD_OFFICE_EADDRESS) AS ADDRESS,
                     --RM.ROUTE_CODE, RM.ROUTE_NAME,
                     AM.AREA_CODE, AM.AREA_NAME,
                     NVL(DM.LATITUDE,0) LATITUDE, NVL(DM.LONGITUDE,0) LONGITUDE,
                     'distributor' AS TYPE,
                     'N' AS WHOLESELLER,
                     CS.PARTY_TYPE_CODE AS DEFAULT_PARTY_TYPE_CODE,
                     '' PARENT_DISTRIBUTOR_CODE,
                     '' PARENT_DISTRIBUTOR_NAME,
                     LT.LAST_VISIT_DATE, LT.LAST_VISIT_BY, LT.LAST_VISIT_STATUS, NVL(LT.IS_VISITED, 'X') AS IS_VISITED, LT.REMARKS,
                     DM.COMPANY_CODE, DM.BRANCH_CODE,'' TYPE_EDESC,'' SUBTYPE_EDESC
                     FROM DIST_DISTRIBUTOR_MASTER DM
                     LEFT JOIN DIST_ROUTE_ENTITY RE ON RE.ENTITY_CODE = DM.DISTRIBUTOR_CODE AND RE.ENTITY_TYPE = 'D' AND RE.COMPANY_CODE = DM.COMPANY_CODE
                     LEFT JOIN DIST_ROUTE_MASTER RM ON RM.ROUTE_CODE = RE.ROUTE_CODE AND RM.COMPANY_CODE = DM.COMPANY_CODE
                     LEFT JOIN SA_CUSTOMER_SETUP CS ON CS.CUSTOMER_CODE = DM.DISTRIBUTOR_CODE AND CS.COMPANY_CODE = DM.COMPANY_CODE
                     INNER JOIN DIST_AREA_MASTER AM ON AM.AREA_CODE = DM.AREA_CODE AND AM.COMPANY_CODE = DM.COMPANY_CODE
                     LEFT JOIN (SELECT A.SP_CODE, A.CUSTOMER_CODE, A.CUSTOMER_TYPE, A.COMPANY_CODE, C.EMPLOYEE_EDESC AS LAST_VISIT_BY, A.IS_VISITED AS LAST_VISIT_STATUS,
                                  (CASE 
                                    WHEN A.IS_VISITED IS NULL THEN 'X' 
                                      ELSE
                                        CASE WHEN TO_CHAR(SYSDATE, 'DD-MON-RRRR') = TO_CHAR(A.UPDATE_DATE, 'DD-MON-RRRR') THEN A.IS_VISITED ELSE 'X' END
                                   END
                                  ) IS_VISITED, 
                                  A.REMARKS,
                                  TO_CHAR(MAX(A.UPDATE_DATE), 'RRRR-MM-DD HH24:MI A.M.') LAST_VISIT_DATE
                                FROM DIST_LOCATION_TRACK A
                                  INNER JOIN SA_CUSTOMER_SETUP B ON B.CUSTOMER_CODE = A.CUSTOMER_CODE AND B.COMPANY_CODE = A.COMPANY_CODE
                                  INNER JOIN HR_EMPLOYEE_SETUP C ON C.EMPLOYEE_CODE = A.SP_CODE AND C.COMPANY_CODE = A.COMPANY_CODE
                                WHERE 1 = 1
                                      AND A.CUSTOMER_TYPE = 'D'
                                      AND A.UPDATE_DATE = (SELECT MAX(UPDATE_DATE) FROM DIST_LOCATION_TRACK WHERE CUSTOMER_CODE = A.CUSTOMER_CODE AND CUSTOMER_TYPE = A.CUSTOMER_TYPE)
                                GROUP BY A.SP_CODE, A.CUSTOMER_CODE, A.CUSTOMER_TYPE, A.COMPANY_CODE, C.EMPLOYEE_EDESC, A.IS_VISITED, 
                                  (CASE 
                                    WHEN A.IS_VISITED IS NULL THEN 'X' 
                                      ELSE
                                        CASE WHEN TO_CHAR(SYSDATE, 'DD-MON-RRRR') = TO_CHAR(A.UPDATE_DATE, 'DD-MON-RRRR') THEN A.IS_VISITED ELSE 'X' END
                                   END
                                  ), 
                                  A.REMARKS
                               ) LT
                       ON LT.CUSTOMER_CODE = DM.DISTRIBUTOR_CODE AND LT.COMPANY_CODE = DM.COMPANY_CODE)
                  UNION
                  (SELECT
                     REM.RESELLER_CODE AS CODE,
                     TRIM(REM.RESELLER_NAME) AS NAME,
                     '' AS ACC_CODE,
                     REM.CONTACT_NO AS P_CONTACT_NO, REM.CONTACT_NAME AS P_CONTACT_NAME, REM.REG_OFFICE_ADDRESS AS ADDRESS,
                     --RM.ROUTE_CODE, RM.ROUTE_NAME,
                     AM.AREA_CODE, AM.AREA_NAME,
                     NVL(REM.LATITUDE,0) LATITUDE, NVL(REM.LONGITUDE,0) LONGITUDE,
                     'reseller' AS TYPE,
                     REM.WHOLESELLER AS WHOLESELLER,
                     '' AS DEFAULT_PARTY_TYPE_CODE,
                     DISTRIBUTOR_CODE AS PARENT_DISTRIBUTOR_CODE,
                     CS.CUSTOMER_EDESC AS PARENT_DISTRIBUTOR_NAME,
                     LT.LAST_VISIT_DATE, LT.LAST_VISIT_BY, LT.LAST_VISIT_STATUS, NVL(LT.IS_VISITED, 'X') AS IS_VISITED, LT.REMARKS,
                     REM.COMPANY_CODE, REM.BRANCH_CODE,
                     DOT.TYPE_EDESC,DOS.SUBTYPE_EDESC
                   FROM DIST_RESELLER_MASTER REM

                     LEFT JOIN DIST_ROUTE_ENTITY RE ON RE.ENTITY_CODE = REM.RESELLER_CODE AND RE.ENTITY_TYPE = 'R' AND RE.COMPANY_CODE = REM.COMPANY_CODE
                     LEFT JOIN DIST_ROUTE_MASTER RM ON RM.ROUTE_CODE = RE.ROUTE_CODE AND RM.COMPANY_CODE = REM.COMPANY_CODE
                     INNER JOIN DIST_AREA_MASTER AM ON AM.AREA_CODE = REM.AREA_CODE AND AM.COMPANY_CODE = REM.COMPANY_CODE
                     LEFT JOIN SA_CUSTOMER_SETUP CS ON CS.CUSTOMER_CODE = REM.DISTRIBUTOR_CODE AND CS.COMPANY_CODE = REM.COMPANY_CODE
                     LEFT JOIN (SELECT A.SP_CODE, A.CUSTOMER_CODE, A.CUSTOMER_TYPE, A.COMPANY_CODE, C.EMPLOYEE_EDESC AS LAST_VISIT_BY, A.IS_VISITED AS LAST_VISIT_STATUS,
                                 (CASE 
                                        WHEN A.IS_VISITED IS NULL THEN 'X' 
                                          ELSE
                                            CASE WHEN TO_CHAR(SYSDATE, 'DD-MON-RRRR') = TO_CHAR(A.UPDATE_DATE, 'DD-MON-RRRR') THEN A.IS_VISITED ELSE 'X' END
                                       END
                                      ) IS_VISITED, 
                                 A.REMARKS,
                                 TO_CHAR(MAX(A.UPDATE_DATE), 'RRRR-MM-DD HH24:MI A.M.') LAST_VISIT_DATE
                                FROM DIST_LOCATION_TRACK A
                                  INNER JOIN DIST_RESELLER_MASTER B ON B.RESELLER_CODE = A.CUSTOMER_CODE AND B.COMPANY_CODE = A.COMPANY_CODE
                                  INNER JOIN HR_EMPLOYEE_SETUP C ON C.EMPLOYEE_CODE = A.SP_CODE AND C.COMPANY_CODE = A.COMPANY_CODE
                                WHERE 1 = 1
                                      AND A.CUSTOMER_TYPE = 'R'
                                      AND A.UPDATE_DATE = (SELECT MAX(UPDATE_DATE) FROM DIST_LOCATION_TRACK WHERE CUSTOMER_CODE = A.CUSTOMER_CODE AND CUSTOMER_TYPE = A.CUSTOMER_TYPE)
                                GROUP BY A.SP_CODE, A.CUSTOMER_CODE, A.CUSTOMER_TYPE, A.COMPANY_CODE, C.EMPLOYEE_EDESC, A.IS_VISITED, 
                                  (CASE 
                                    WHEN A.IS_VISITED IS NULL THEN 'X' 
                                      ELSE
                                        CASE WHEN TO_CHAR(SYSDATE, 'DD-MON-RRRR') = TO_CHAR(A.UPDATE_DATE, 'DD-MON-RRRR') THEN A.IS_VISITED ELSE 'X' END
                                   END
                                  ), 
                                  A.REMARKS
                               ) LT ON LT.CUSTOMER_CODE = REM.RESELLER_CODE AND LT.COMPANY_CODE = REM.COMPANY_CODE
               LEFT JOIN DIST_OUTLET_TYPE DOT ON REM.OUTLET_TYPE_ID=DOT.TYPE_ID AND REM.COMPANY_CODE=DOT.COMPANY_CODE
               LEFT JOIN DIST_OUTLET_SUBTYPE DOS ON REM.OUTLET_SUBTYPE_ID=DOS.SUBTYPE_ID AND REM.COMPANY_CODE=DOS.COMPANY_CODE)
                )
                ORDER BY TYPE";
            var Entities = dbContext.SqlQuery<EntityResponseModel>(EntityQuery).GroupBy(x => x.TYPE);
            if (Entities.Count() <= 0)
                throw new Exception("No records found");
            foreach (var EntGroup in Entities)
            {
                result.Add(EntGroup.Key, EntGroup.ToList());
            }
            return (result);
        }

        //public List<ItemModel> FetchItems(CommonRequestModel model, NeoErpCoreEntity dbContext)
        //{
        //    var pref = FetchPreferences(model.COMPANY_CODE, dbContext);
        //    string salesClause = "", conversionClause = "";
        //    if ("Y" == pref.PO_SYN_RATE)
        //        salesClause = "AND SALES_RATE IS NOT NULL AND SALES_RATE <> 0";
        //    if ("Y" == pref.SQL_NN_CONVERSION_UNIT_FACTOR)
        //        conversionClause = "AND IUS.MU_CODE IS NOT NULL AND IUS.CONVERSION_FACTOR IS NOT NULL";

        //    string ItemsQuery = string.Empty;
        //    ItemsQuery = $@"SELECT IM.ITEM_CODE, IM.ITEM_EDESC, ISS.BRAND_NAME, IM.INDEX_MU_CODE AS UNIT, MC.MU_EDESC, IUS.MU_CODE CONVERSION_UNIT,
        //        TO_CHAR(IUS.CONVERSION_FACTOR) AS CONVERSION_FACTOR, TO_CHAR(NVL(IR.SALES_RATE, 0)) SALES_RATE, TO_CHAR(IR.APPLY_DATE) AS APPLY_DATE
        //        FROM IP_ITEM_MASTER_SETUP IM
        //          INNER JOIN IP_MU_CODE MC ON MC.MU_CODE = IM.INDEX_MU_CODE AND MC.COMPANY_CODE = IM.COMPANY_CODE
        //          INNER JOIN IP_ITEM_SPEC_SETUP ISS ON ISS.ITEM_CODE = IM.ITEM_CODE AND ISS.COMPANY_CODE = IM.COMPANY_CODE AND TRIM(ISS.BRAND_NAME) IS NOT NULL
        //          LEFT JOIN IP_ITEM_UNIT_SETUP IUS ON IUS.ITEM_CODE = ISS.ITEM_CODE AND IUS.COMPANY_CODE = ISS.COMPANY_CODE
        //          LEFT JOIN (SELECT A.ITEM_CODE, A.APPLY_DATE, B.SALES_RATE, B.COMPANY_CODE
        //                      FROM (SELECT ITEM_CODE, COMPANY_CODE, MAX(APP_DATE) APPLY_DATE 
        //                        FROM IP_ITEM_RATE_APPLICAT_SETUP
        //                        WHERE COMPANY_CODE = '{model.COMPANY_CODE}' 
        //                        AND BRANCH_CODE = '{model.BRANCH_CODE}'
        //                        GROUP BY ITEM_CODE, COMPANY_CODE) A
        //                      INNER JOIN IP_ITEM_RATE_APPLICAT_SETUP B
        //                        ON B.ITEM_CODE = A.ITEM_CODE
        //                        AND B.APP_DATE = A.APPLY_DATE
        //                        AND B.COMPANY_CODE = '{model.COMPANY_CODE}'
        //                        AND B.BRANCH_CODE = '{model.BRANCH_CODE}') IR 
        //            ON IR.ITEM_CODE = IM.ITEM_CODE AND IR.COMPANY_CODE = IM.COMPANY_CODE
        //        WHERE IM.COMPANY_CODE = '{model.COMPANY_CODE}' AND IM.CATEGORY_CODE = '{CATEGORY_CODE}' AND IM.GROUP_SKU_FLAG = '{GROUP_SKU_FLAG}' AND IM.DELETED_FLAG = 'N'
        //        {salesClause}
        //        {conversionClause}
        //        ORDER BY UPPER(IM.ITEM_EDESC) ASC";
        //    var Items = dbContext.SqlQuery<ItemModel>(ItemsQuery).ToList();
        //    if (Items.Count <= 0)
        //        throw new Exception("No records found");
        //    return Items;
        //}

        /*SASHI* no value for mu_code*/
        public List<ItemModel> FetchItems(CommonRequestModel model, NeoErpCoreEntity dbContext)
        {
            var pref = FetchPreferences(model.COMPANY_CODE, dbContext);
            string salesClause = "", conversionClause = "";
            if ("Y" == pref.PO_SYN_RATE)
                salesClause = "AND SALES_RATE IS NOT NULL AND SALES_RATE <> 0";
            if ("Y" == pref.SQL_NN_CONVERSION_UNIT_FACTOR)
                conversionClause = "AND IUS.MU_CODE IS NOT NULL AND IUS.CONVERSION_FACTOR IS NOT NULL";

            string ItemsQuery = string.Empty;
            ItemsQuery = $@"SELECT IM.ITEM_CODE, IM.ITEM_EDESC, ISS.BRAND_NAME, IM.INDEX_MU_CODE AS UNIT, MC.MU_EDESC, IUS.MU_CODE CONVERSION_UNIT,
                TO_CHAR(IUS.CONVERSION_FACTOR) AS CONVERSION_FACTOR, TO_CHAR(NVL(IR.SALES_RATE, 0)) SALES_RATE, TO_CHAR(IR.APPLY_DATE) AS APPLY_DATE
                FROM IP_ITEM_MASTER_SETUP IM
                  INNER JOIN IP_MU_CODE MC ON MC.MU_CODE = IM.INDEX_MU_CODE AND MC.COMPANY_CODE = IM.COMPANY_CODE
                  INNER JOIN IP_ITEM_SPEC_SETUP ISS ON ISS.ITEM_CODE = IM.ITEM_CODE AND ISS.COMPANY_CODE = IM.COMPANY_CODE AND TRIM(ISS.BRAND_NAME) IS NOT NULL
                  LEFT JOIN IP_ITEM_UNIT_SETUP IUS ON IUS.ITEM_CODE = ISS.ITEM_CODE AND IUS.COMPANY_CODE = ISS.COMPANY_CODE
                  LEFT JOIN (SELECT A.ITEM_CODE, A.APPLY_DATE, B.SALES_RATE, B.COMPANY_CODE
                              FROM (SELECT ITEM_CODE, COMPANY_CODE, MAX(APP_DATE) APPLY_DATE 
                                FROM IP_ITEM_RATE_APPLICAT_SETUP
                                WHERE COMPANY_CODE = '{model.COMPANY_CODE}' 
                                AND BRANCH_CODE = '{model.BRANCH_CODE}'
                                GROUP BY ITEM_CODE, COMPANY_CODE) A
                              INNER JOIN IP_ITEM_RATE_APPLICAT_SETUP B
                                ON B.ITEM_CODE = A.ITEM_CODE
                                AND B.APP_DATE = A.APPLY_DATE
                                AND B.COMPANY_CODE = '{model.COMPANY_CODE}'
                                AND B.BRANCH_CODE = '{model.BRANCH_CODE}') IR 
                    ON IR.ITEM_CODE = IM.ITEM_CODE AND IR.COMPANY_CODE = IM.COMPANY_CODE
                WHERE IM.COMPANY_CODE = '{model.COMPANY_CODE}' AND IM.CATEGORY_CODE = '{CATEGORY_CODE}' AND IM.GROUP_SKU_FLAG = '{GROUP_SKU_FLAG}' AND IM.DELETED_FLAG = 'N'
                {salesClause}
                {conversionClause}
                ORDER BY UPPER(IM.ITEM_EDESC) ASC";
            var Items = dbContext.SqlQuery<ItemModel>(ItemsQuery).ToList();
            if (Items.Count <= 0)
                throw new Exception("No records found");
            return Items;
        }

        public QuestionResponseModel FetchAllQuestions(QuestionRequestModel model, NeoErpCoreEntity dbContext)
        {
            QuestionResponseModel Result = new QuestionResponseModel();
            //general Questions
            var generalQuery = string.Empty;
            generalQuery = $@"SELECT TO_CHAR(A.QA_CODE) AS QA_CODE, A.QA_TYPE, A.QUESTION FROM DIST_QA_MASTER A
                    WHERE A.DELETED_FLAG = 'N' AND 
                    A.SET_CODE = (SELECT B.SET_CODE FROM DIST_QA_SET B WHERE B.COMPANY_CODE=A.COMPANY_CODE AND
                                B.QA_TYPE='{model.SetType}' AND
                                B.SET_CODE=(SELECT C.SET_CODE FROM DIST_QA_SET_SALESPERSON_MAP C WHERE C.SP_CODE='{model.sp_code}' AND 
                                C.COMPANY_CODE=B.COMPANY_CODE AND C.DELETED_FLAG='N'))
                    AND A.COMPANY_CODE='{model.COMPANY_CODE}'";
            var generalQuestion = dbContext.SqlQuery<GeneralModel>(generalQuery).ToList();
            for (int i = 0; i < generalQuestion.Count; i++)
            {
                string AnswerQuery = string.Empty;
                string[] types = { "MCR", "MCC" };
                if (types.Contains(generalQuestion[i].QA_TYPE))
                {
                    AnswerQuery = $@"SELECT ANSWERS FROM DIST_QA_DETAIL WHERE QA_CODE='{generalQuestion[i].QA_CODE}' AND COMPANY_CODE='{model.COMPANY_CODE}'";
                    var answers = dbContext.SqlQuery<string>(AnswerQuery).ToList();
                    generalQuestion[i].ANSWERS = answers;
                }
            }

            //tabular questions
            string TabularQuery = string.Empty;
            TabularQuery = $@"SELECT TBL.*, TC.CELL_ID, TC.ROW_NO, TC.CELL_NO, TC.CELL_TYPE, TC.CELL_LABEL
                FROM DIST_QA_TAB_TABLE TBL
                LEFT JOIN DIST_QA_TAB_CELL TC ON TBL.TABLE_ID = TC.TABLE_ID
                WHERE TBL.COMPANY_CODE = '{model.COMPANY_CODE}'
                ORDER BY TBL.TABLE_ID, TC.ROW_NO, TC.CELL_NO";
            var tempData = dbContext.SqlQuery<TabularTemp>(TabularQuery).GroupBy(x => x.TABLE_ID);
            var tabularQuestions = new Dictionary<string, TabularModel>();
            foreach (var table in tempData)
            {
                var tables = table.ToList();
                var resultTable = new TabularModel
                {
                    TABLE_ID = tables[0].TABLE_ID.ToString(),
                    TABLE_TITLE = tables[0].TABLE_TITLE,
                    CREATED_DATE = tables[0].CREATED_DATE == null ? null : tables[0].CREATED_DATE.Value.ToString("dd-MMM-yy"),
                    DELETED_FLAG = tables[0].DELETED_FLAG
                };
                var groupTables = tables.GroupBy(x => x.ROW_NO);
                foreach (var rowGroup in groupTables)
                {
                    var CellGroup = (from r in rowGroup
                                     select new TabularCellModel
                                     {
                                         CELL_ID = r.CELL_ID.ToString(),
                                         CELL_LABEL = r.CELL_LABEL,
                                         CELL_NO = r.CELL_NO.ToString(),
                                         CELL_TYPE = r.CELL_TYPE,
                                         ROW_NO = r.ROW_NO.ToString()
                                     }).ToList();
                    resultTable.CELL_DATA.Add(CellGroup);
                }
                tabularQuestions.Add(table.Key.ToString(), resultTable);
            }
            Result.general = generalQuestion;
            Result.tabular = tabularQuestions;
            if (Result.general.Count <= 0 && Result.tabular.Count <= 0)
                throw new Exception("No records found");
            return Result;
        }

        public List<AreaResponseModel> FetchArea(CommonRequestModel model, NeoErpCoreEntity dbContext)
        {
            var AreaQuery = $@"SELECT A.AREA_CODE,A.AREA_NAME,A.ZONE_CODE,A.DISTRICT_CODE,A.VDC_CODE,A.REG_CODE,
                       B.ZONE_NAME,B.DISTRICT_NAME,B.VDC_NAME,B.REG_NAME
                FROM DIST_AREA_MASTER A,DIST_ADDRESS_MASTER B
                WHERE (A.DISTRICT_CODE = B.DISTRICT_CODE
                AND  A.REG_CODE = B.REG_CODE
                AND  A.ZONE_CODE = B.ZONE_CODE
                AND  A.VDC_CODE = B.VDC_CODE) AND A.COMPANY_CODE = '{model.COMPANY_CODE}' ORDER BY UPPER(A.AREA_NAME) ASC";

            var result = dbContext.SqlQuery<AreaResponseModel>(AreaQuery).ToList();
            if (result.Count <= 0)
                throw new Exception("No records found");
            return result;
        }

        public Dictionary<string, OutletResponseModel> FetchOutlets(CommonRequestModel model, NeoErpCoreEntity dbContext)
        {
            var result = new Dictionary<string, OutletResponseModel>();
            string OutletsQuery = $@"SELECT TO_CHAR(T.TYPE_ID) AS TYPE_ID, T.TYPE_CODE, T.TYPE_EDESC, T.DELETED_FLAG, TO_CHAR(NVL(ST.SUBTYPE_ID,0)) AS SUBTYPE_ID, ST.SUBTYPE_CODE, ST.SUBTYPE_EDESC FROM DIST_OUTLET_TYPE T
                LEFT JOIN DIST_OUTLET_SUBTYPE ST ON ST.TYPE_ID = T.TYPE_ID AND ST.COMPANY_CODE = T.COMPANY_CODE
                WHERE T.COMPANY_CODE='{model.COMPANY_CODE}' AND  T.DELETED_FLAG='N'
                ORDER BY UPPER(T.TYPE_EDESC), ST.SUBTYPE_EDESC ASC";
            var tempOutlets = dbContext.SqlQuery<OutletTemp>(OutletsQuery).GroupBy(x => x.TYPE_ID);
            if (tempOutlets.Count() <= 0)
                throw new Exception("No records found");
            foreach (var typeGroup in tempOutlets)
            {
                var outlet = new OutletResponseModel();
                foreach (var subType in typeGroup)
                {
                    var SubObj = new SubTypeModel
                    {
                        SUBTYPE_ID = subType.SUBTYPE_ID,
                        SUBTYPE_CODE = subType.SUBTYPE_CODE,
                        SUBTYPE_EDESC = subType.SUBTYPE_EDESC,
                        TYPE_ID = typeGroup.Key
                    };
                    outlet.SIZE.Add(subType.SUBTYPE_ID, SubObj);
                }
                var typeObjTemp = typeGroup.FirstOrDefault();
                outlet.TYPE_ID = typeObjTemp.TYPE_ID;
                outlet.TYPE_CODE = typeObjTemp.TYPE_CODE;
                outlet.TYPE_EDESC = typeObjTemp.TYPE_EDESC;
                outlet.DELETED_FLAG = typeObjTemp.DELETED_FLAG;

                result.Add(typeGroup.Key, outlet);
            }
            return result;
        }

        public ClosingStockResponseModel GetEntityItemByBrand(ClosingStockRequestModel model, NeoErpCoreEntity dbContext)
        {
            ClosingStockResponseModel result = new ClosingStockResponseModel();
            var ItemsQuery = string.Empty;
            if (model.entity_type.ToUpper() == "R" || model.entity_type.ToUpper() == "RESELLER")
            {
                ItemsQuery = $@"SELECT A.BRAND_NAME,
                  B.ITEM_CODE,
                  B.ITEM_EDESC,
                  B.INDEX_MU_CODE,
                  CONCAT(CONCAT(NVL(C.LVS, 0), ' '), NVL(B.INDEX_MU_CODE, C.MU_CODE)) AS LVS
                FROM IP_ITEM_SPEC_SETUP A
                  INNER JOIN IP_ITEM_MASTER_SETUP B
                    ON B.ITEM_CODE = A.ITEM_CODE
                       AND B.CATEGORY_CODE = '{CATEGORY_CODE}'
                       AND B.GROUP_SKU_FLAG = '{GROUP_SKU_FLAG}'
                       AND B.DELETED_FLAG = A.DELETED_FLAG
                       AND A.COMPANY_CODE = B.COMPANY_CODE
                  LEFT JOIN (
                    SELECT TO_CHAR(T.CREATED_DATE, 'DD-MON-RRRR') CREATED_DATE, T.RESELLER_CODE, T.ITEM_CODE, T.MU_CODE, T.CURRENT_STOCK AS LVS
                    FROM DIST_RESELLER_STOCK T
                    WHERE T.RESELLER_CODE='{model.entity_code}'
                          AND T.CREATED_DATE = (SELECT MAX(CREATED_DATE) FROM DIST_RESELLER_STOCK WHERE RESELLER_CODE='{model.entity_code}' AND DELETED_FLAG='N' AND COMPANY_CODE='{model.COMPANY_CODE}')
                    ) C
                      ON C.ITEM_CODE = B.ITEM_CODE
                WHERE TRIM(A.BRAND_NAME) IS NOT NULL
                      AND A.COMPANY_CODE = '{model.COMPANY_CODE}'
                      AND A.DELETED_FLAG = 'N'
                ORDER BY UPPER(A.BRAND_NAME), UPPER(B.ITEM_EDESC) ASC";
            }
            else if (model.entity_type.ToUpper() == "D" || model.entity_type.ToUpper() == "DISTRIBUTOR")
            {
                ItemsQuery = $@"SELECT A.BRAND_NAME,
                  B.ITEM_CODE,
                  B.ITEM_EDESC,
                  B.INDEX_MU_CODE,
                  CONCAT(CONCAT(NVL(C.LVS, 0), ' '), NVL(B.INDEX_MU_CODE, C.MU_CODE)) AS LVS
                FROM IP_ITEM_SPEC_SETUP A
                  INNER JOIN IP_ITEM_MASTER_SETUP B
                    ON A.ITEM_CODE = B.ITEM_CODE
                       AND B.CATEGORY_CODE = '{CATEGORY_CODE}'
                       AND B.GROUP_SKU_FLAG = '{GROUP_SKU_FLAG}'
                       AND B.DELETED_FLAG = A.DELETED_FLAG
                       AND A.COMPANY_CODE = B.COMPANY_CODE
                  LEFT JOIN (
                    SELECT TO_CHAR(T.CREATED_DATE, 'DD-MON-RRRR') CREATED_DATE, T.DISTRIBUTOR_CODE, T.ITEM_CODE, T.MU_CODE, T.CURRENT_STOCK AS LVS
                    FROM DIST_DISTRIBUTOR_STOCK T
                    WHERE T.DISTRIBUTOR_CODE='{model.entity_type}'
                          AND T.CREATED_DATE = (SELECT MAX(CREATED_DATE) FROM DIST_DISTRIBUTOR_STOCK WHERE DISTRIBUTOR_CODE='{model.entity_type}' AND DELETED_FLAG='N' AND COMPANY_CODE='{model.COMPANY_CODE}')
                    ) C
                      ON C.ITEM_CODE = B.ITEM_CODE
                WHERE TRIM(A.BRAND_NAME) IS NOT NULL
                      AND A.COMPANY_CODE = '{model.COMPANY_CODE}'
                      AND A.DELETED_FLAG = 'N'
                      --AND UPPER(A.BRAND_NAME) = 'SUJI'
                ORDER BY UPPER(A.BRAND_NAME), UPPER(B.ITEM_EDESC) ASC";
            }
            else
                throw new Exception("Entity type not specified");
            var items = dbContext.SqlQuery<ClosingStockItemModel>(ItemsQuery).GroupBy(x => x.BRAND_NAME);

            foreach (var brand in items)
            {
                var temp = new Dictionary<string, ClosingStockItemModel>();
                foreach (var item in brand)
                {
                    temp.Add(item.ITEM_CODE, item);
                }
                result.item.Add(brand.Key, temp);
            }
            if (result.item.Count <= 0)
                throw new Exception("No records found");
            result.mu_code = this.FetchMU(model, dbContext);
            return result;
        }

        public Dictionary<string, Dictionary<string, MuCodeResponseModel>> FetchMU(CommonRequestModel model, NeoErpCoreEntity dbContext)
        {
            Dictionary<string, MuCodeResponseModel> result = new Dictionary<string, MuCodeResponseModel>();
            var pref = FetchPreferences(model.COMPANY_CODE, dbContext);
            var conversionClause = "";
            if ("Y" == pref.SQL_NN_CONVERSION_UNIT_FACTOR)
            {
                conversionClause = "AND IUS.MU_CODE IS NOT NULL AND IUS.CONVERSION_FACTOR IS NOT NULL";
            }
            string MuQuery = $@"SELECT IMS.ITEM_CODE, IMS.ITEM_EDESC, IMS.INDEX_MU_CODE, IUS.MU_CODE, TO_CHAR(IUS.CONVERSION_FACTOR) AS CONVERSION_FACTOR
                FROM IP_ITEM_MASTER_SETUP IMS 
                LEFT JOIN IP_ITEM_UNIT_SETUP IUS
                ON IUS.ITEM_CODE = IMS.ITEM_CODE AND IUS.COMPANY_CODE = IMS.COMPANY_CODE
                WHERE 1 = 1
                AND IMS.COMPANY_CODE = '{model.COMPANY_CODE}'
                AND IMS.CATEGORY_CODE = 'FG'
                AND IMS.GROUP_SKU_FLAG = 'I'
                AND IMS.DELETED_FLAG = 'N'
                {conversionClause}
                ORDER BY UPPER(IMS.ITEM_EDESC), UPPER(IMS.INDEX_MU_CODE), UPPER(MU_CODE)";
            var allMu = dbContext.SqlQuery<MuCodeResponseModel>(MuQuery);
            foreach (var Mu in allMu)
            {
                if (Mu.MU_CODE != null)
                    Mu.CONVERSION_UNIT_FACTOR.Add(Mu.MU_CODE, Mu.CONVERSION_FACTOR);
                result.Add(Mu.ITEM_CODE, Mu);
            }
            var finalResult = new Dictionary<string, Dictionary<string, MuCodeResponseModel>>();
            finalResult.Add("UNIT", result);
            return finalResult;
        }




        public class FORAGEING
        {
            public DateTime VOUCHER_DATE { get; set; }
            public string VOUCHER_NO { get; set; }
            public int SUB { get; set; }
            public decimal? PENDING_BAL { get; set; }
            public decimal? AGE { get; set; }
            public decimal? CLOSING_BALANCE { get; set; }
            public decimal? AMM { get; set; }
        }

        public class CustomerModel
        {
            public int CUSTOMER_CODE { get; set; }
            public string CUSTOMER_EDESC { get; set; }
            public string GROUP_SKU_FLAG { get; set; }
            public string MASTER_CUSTOMER_CODE { get; set; }
            public string PRE_CUSTOMER_CODE { get; set; }
        }
        private void AddCell(PdfPTable table, string text, bool isHeader)
        {
            Font font = isHeader
                ? FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9)
                : FontFactory.GetFont(FontFactory.HELVETICA, 9);

            PdfPCell cell = new PdfPCell(new Phrase(text ?? "", font));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;

            if (isHeader)
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;

            table.AddCell(cell);
        }
        public dynamic ageingTransactionsPdf(TransactionRequestModel model, NeoErpCoreEntity dbContext)
        {
            var data = ageingTransactions(model, dbContext);

            var detail = (List<Dictionary<string, object>>)data["detail"];
            var totals = (Dictionary<string, object>)data["total"];

            string folderPath = HttpContext.Current.Server.MapPath("~/Areas/NeoErp.Distribution/Images/Uploads/Reports/AgeingReport/");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string fileName = $"AgeingReport_{model.COMPANY_CODE}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            string fullPath = Path.Combine(folderPath, fileName);

            using (FileStream fs = new FileStream(fullPath, FileMode.Create))
            {
                Document document = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
                PdfWriter.GetInstance(document, fs);
                document.Open();

                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
                Font normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);

                document.Add(new Paragraph("Customer Ageing Report", headerFont));
                document.Add(new Paragraph(" "));

                PdfPTable table = new PdfPTable(7);
                table.WidthPercentage = 100;

                AddCell(table, "Customer", true);
                AddCell(table, "Total", true);
                AddCell(table, "0-30", true);
                AddCell(table, "31-60", true);
                AddCell(table, "61-90", true);
                AddCell(table, "91-120", true);
                AddCell(table, "120+", true);

                foreach (var row in detail)
                {
                    AddCell(table, row["item"]?.ToString(), false);
                    AddCell(table, row["total"]?.ToString(), false);
                    AddCell(table, row["0-30"]?.ToString(), false);
                    AddCell(table, row["31-60"]?.ToString(), false);
                    AddCell(table, row["61-90"]?.ToString(), false);
                    AddCell(table, row["91-120"]?.ToString(), false);
                    AddCell(table, row["120+"]?.ToString(), false);
                }

                document.Add(table);
                document.Close();
            }

            string returnPath = "/Areas/NeoErp.Distribution/Images/Uploads/Reports/AgeingReport/" + fileName;

            return returnPath;
        }
        public dynamic ageingTransactions(TransactionRequestModel model, NeoErpCoreEntity dbContext)
        {

            Console.WriteLine(model);

            var slData = new List<Dictionary<string, object>>();

            var opening = new Dictionary<string, object>();

            List<double> drsArray = new List<double>();

            List<double> crsArray = new List<double>();

            Dictionary<string, float> ageingVals = new Dictionary<string, float>();

            string branchCode = model.BRANCH_CODE;
            string company_code = model.COMPANY_CODE;

            string formatedBranchCodes;
            if (branchCode.Contains("[") && branchCode.Contains("]"))
            {
                formatedBranchCodes = branchCode.Replace("[", "'").Replace("]", "'").Replace(", ", "','");
            }
            else
            {
                formatedBranchCodes = $"'{branchCode}'";
            }

            string formatedCompanyCodes;
            if (company_code.Contains("[") && company_code.Contains("]"))
            {
                formatedCompanyCodes = company_code.Replace("[", "'").Replace("]", "'").Replace(", ", "','");
            }
            else
            {
                formatedCompanyCodes = $"'{company_code}'";
            }

            Debug.WriteLine(formatedBranchCodes);
            var dateData = dbContext.SqlQuery<DateModel>($"select startdate as START_DATE, enddate as END_DATE from v_date_range where rangename='This Year'").ToList();


            //string ss = model.user_id;

            if (string.IsNullOrEmpty(model.from_date))
            {
                model.from_date = dateData[0].START_DATE.ToString("dd-MMM-yyyy");
            }
            if (string.IsNullOrEmpty(model.to_date))
            {
                model.to_date = dateData[0].END_DATE.ToString("dd-MMM-yyyy");
            }
            // First query to fetch transaction details
            string query1 = $@"
        SELECT voucher_date, voucher_no, dr_amount * exchange_rate AS dr_amount, 
               cr_amount * exchange_rate AS cr_amount, particulars, created_by, 
               currency_code, exchange_rate, BS_DATE(voucher_date) AS bs_date,
               remarks, manual_no
        FROM V$VIRTUAL_SUB_LEDGER
        WHERE sub_code = '{model.sub_code}'
          AND company_code in ({formatedCompanyCodes})
          AND branch_code in ({formatedBranchCodes})
          AND trunc(voucher_date) BETWEEN '{model.from_date}' and 
          '{model.to_date}'
          AND deleted_flag = 'N'
          AND form_code != 0
        ORDER BY voucher_date, voucher_no";



            string sConnStr1 = ConfigurationManager.ConnectionStrings["NeoErpCoreEntity"].ToString();
            string[] tokens = sConnStr1.Split('"');

            using (OracleConnection objConn = new OracleConnection(tokens[1]))
            {
                objConn.Open();

                using (OracleCommand cmd = new OracleCommand(query1, objConn))
                {
                    cmd.Parameters.Add(":SubCode", model.sub_code);
                    cmd.Parameters.Add(":CompanyCode", model.COMPANY_CODE);
                    cmd.Parameters.Add(":BranchCode", model.BRANCH_CODE);
                    cmd.Parameters.Add(":FromDate", model.from_date);
                    cmd.Parameters.Add(":ToDate", model.to_date);

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            double debitAmt = reader["dr_amount"] != DBNull.Value ? Convert.ToDouble(reader["dr_amount"]) : 0;
                            double creditAmt = reader["cr_amount"] != DBNull.Value ? Convert.ToDouble(reader["cr_amount"]) : 0;

                            drsArray.Add(debitAmt);
                            crsArray.Add(creditAmt);

                            var vals = new Dictionary<string, object>
                            {
                                ["voucher_date"] = Convert.ToDateTime(reader["voucher_date"]).ToString("dd-MMM-yyyy"),
                                ["voucher_no"] = reader["voucher_no"].ToString(),
                                ["dr_amount"] = debitAmt,
                                ["cr_amount"] = creditAmt,
                                ["particulars"] = reader["particulars"].ToString(),
                                ["created_by"] = reader["created_by"].ToString(),
                                ["currency_code"] = reader["currency_code"].ToString(),
                                ["exchange_rate"] = reader["exchange_rate"] != DBNull.Value ? Convert.ToDouble(reader["exchange_rate"]) : 0,
                                ["miti"] = reader["bs_date"].ToString(),
                                ["remarks"] = reader["remarks"].ToString(),
                                ["manual_no"] = reader["manual_no"].ToString()
                            };

                            slData.Add(vals);
                        }
                    }
                }
            }



            string query = $@"select CAST(NVL(sub, 0) AS NUMBER(20,2)) sub ,voucher_date,
                voucher_no,
                case when amm =1 then closing_balance else  pending_bal end as   pending_bal,
                age,
                closing_balance,
                amm
                from
                (SELECT sub,voucher_date,voucher_no, pending_bal,age, closing_balance,
                            ROW_NUMBER() OVER (PARTITION BY sub ORDER BY voucher_date,sub) amm  FROM
                             (
                             select * from
                            (select to_number(substr(sub_code,2,99)) sub ,voucher_date,voucher_no,
                            case when qw=1 then net_amount else dr_amount end as pending_bal ,
                            trunc(sysdate)-trunc(voucher_date) age,closing_balance from(select sub_code,voucher_no,voucher_date,dr_amount,cr_amount,qw,(dr_amount - nvl(cr_amount,0)) AS net_amount
                            ,SUM(dr_amount - nvl(cr_amount,0)) OVER (PARTITION BY to_number(substr(sub_code,2,99)) ORDER BY voucher_date,voucher_no,serial_no) AS closing_balance
                            from(select sub_code,voucher_no,voucher_date,dr_amount,qw,case when qw=1 then cr_amount else 0 end as cr_amount,serial_no
                            from(select sub_code,voucher_no,voucher_date,dr_amount,cr_amount, ROW_NUMBER() OVER (PARTITION BY to_number(substr(sub_code,2,9999)) ORDER BY to_number(substr(sub_code,2,9999)),voucher_date,voucher_no,serial_no) AS qw,serial_no
                            from (
                            select sub_code,voucher_no,voucher_date,dr_amount, SUM(cr_amount) OVER (PARTITION BY sub_code) cr_amount ,serial_no
                            from (
                            SELECT TO_CHAR(sub_code) sub_code, TO_CHAR(voucher_no) voucher_no, voucher_date, dr_amount, cr_amount ,serial_no
                            FROM V$VIRTUAL_SUB_LEDGER WHERE  
                            1=1 AND company_code in ({formatedCompanyCodes})  and sub_code in (select 'C'||customer_code from DIST_USER_AREAS  where user_id='{model.user_id}')
                            --AND branch_code in ({formatedBranchCodes})
                            and substr(sub_code,1,1)='C' 
                            AND VOUCHER_NO NOT IN(SELECT DISTINCT NVL(BOUNCE_VC_NO,'A000000') FROM FA_PDC_RECEIPTS WHERE (ENCASH_DATE IS NOT NULL OR BOUNCE_DATE IS NOT NULL) AND BOUNCE_FLAG='Y' )
                            and form_code not in (select distinct form_code from form_detail_setup where table_name='FA_PAY_ORDER' AND company_code in ({formatedCompanyCodes})) 
                            )
                            order by to_number(substr(sub_code,2,99)),voucher_date,voucher_no
                            ) where dr_amount !=0)) ) where closing_balance>0 )
                             ) )
                union all
                             select sub,voucher_date,
                voucher_no,
                case when amm =1 then closing_balance *-1 else  pending_bal *-1 end as   pending_bal,
                age,
                closing_balance,
                amm
                from
                (SELECT sub,voucher_date,voucher_no, pending_bal,age, closing_balance ,
                            ROW_NUMBER() OVER (PARTITION BY sub ORDER BY voucher_date,sub) amm  FROM
                             (
                             select * from
                            (select to_number(substr(sub_code,2,99)) sub ,voucher_date,voucher_no,
                            case when qw=1 then net_amount else dr_amount end as pending_bal ,
                            trunc(sysdate)-trunc(voucher_date) age,closing_balance from(select sub_code,voucher_no,voucher_date,dr_amount,cr_amount,qw,(dr_amount - nvl(cr_amount,0)) AS net_amount
                            ,SUM(dr_amount - nvl(cr_amount,0)) OVER (PARTITION BY to_number(substr(sub_code,2,99)) ORDER BY voucher_date,voucher_no) AS closing_balance
                            from(select sub_code,voucher_no,voucher_date,dr_amount,qw,case when qw=1 then cr_amount else 0 end as cr_amount
                            from(select sub_code,voucher_no,voucher_date,dr_amount,cr_amount, ROW_NUMBER() OVER (PARTITION BY to_number(substr(sub_code,2,9999)) ORDER BY to_number(substr(sub_code,2,9999)),voucher_date,voucher_no) AS qw
                            from (
                            select sub_code,voucher_no,voucher_date,dr_amount, SUM(cr_amount) OVER (PARTITION BY sub_code) cr_amount
                            from (
                            SELECT TO_CHAR(sub_code) sub_code, TO_CHAR(voucher_no) voucher_no, voucher_date,cr_amount dr_amount,dr_amount  cr_amount
                            FROM V$VIRTUAL_SUB_LEDGER WHERE  
                            1=1 AND company_code in ({formatedCompanyCodes})  and sub_code in (select 'C'||customer_code from DIST_USER_AREAS  where user_id='{model.user_id}')
                            --AND branch_code in ({formatedBranchCodes}) 
                            and substr(sub_code,1,1)='C'
                            AND VOUCHER_NO NOT IN(SELECT DISTINCT NVL(BOUNCE_VC_NO,'A000000') FROM FA_PDC_RECEIPTS WHERE (ENCASH_DATE IS NOT NULL OR BOUNCE_DATE IS NOT NULL) AND BOUNCE_FLAG='Y' )
                            and form_code not in (select distinct form_code from form_detail_setup where table_name='FA_PAY_ORDER' AND company_code in ({formatedCompanyCodes})) )
                            order by to_number(substr(sub_code,2,99)),voucher_date,voucher_no
                            ) where dr_amount !=0)) ) where closing_balance>0 )
                             ) ) ";



            List<FORAGEING> all_codes = new List<FORAGEING>();

            using (OracleConnection objConn = new OracleConnection(tokens[1]))
            {
                objConn.Open();
                using (OracleCommand objCmd = new OracleCommand(query, objConn))
                {
                    using (OracleDataReader reader = objCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = new FORAGEING
                            {
                                SUB = reader["SUB"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SUB"]),
                                VOUCHER_DATE = reader["VOUCHER_DATE"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["VOUCHER_DATE"]),
                                VOUCHER_NO = reader["VOUCHER_NO"] == DBNull.Value ? "" : reader["VOUCHER_NO"].ToString(),
                                PENDING_BAL = reader["PENDING_BAL"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PENDING_BAL"]),
                                AGE = reader["AGE"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["AGE"]),
                                CLOSING_BALANCE = reader["CLOSING_BALANCE"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["CLOSING_BALANCE"]),
                                AMM = reader["AMM"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["AMM"])
                            };

                            all_codes.Add(item);
                        }
                    }
                }
            }


            int interval = 30;


            List<Dictionary<string, object>> first030 = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> third3160 = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> fourth6190 = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> sixth91120 = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> seventh120 = new List<Dictionary<string, object>>();

            List<decimal> allBal = new List<decimal>();
            Dictionary<int, Dictionary<string, object>> allData = new Dictionary<int, Dictionary<string, object>>();


            int? subCode = null;
            int x = 0;
            decimal bal = 0;
            string vNo = "";
            int days = 0;

            foreach (var i in all_codes)
            {
                // Assuming i.SUB, i.VOUCHER_DATE, i.VOUCHER_NO, i.PENDING_BAL, i.AGE
                if (i.VOUCHER_DATE != null)
                {
                    int currentSubCode = Convert.ToInt32(i.SUB);
                    //int currentSubCode = Convert.ToInt32(i.SUB);

                    if (x == 0)
                    {
                        subCode = currentSubCode;
                        allBal = new List<decimal>();
                    }
                    else
                    {
                        allBal.Add(bal);

                        var entry = new Dictionary<string, object>
                            {
                                { "amt", bal },
                                { "v_no", vNo },
                                { "days", days }
                            };

                        if (days <= interval)
                            first030.Add(entry);
                        else if (days <= interval * 2)
                            third3160.Add(entry);
                        else if (days <= interval * 3)
                            fourth6190.Add(entry);
                        else if (days <= interval * 4)
                            sixth91120.Add(entry);
                        else
                            seventh120.Add(entry);
                    }

                    if (subCode != currentSubCode)
                    {
                        allData[(int)subCode] = new Dictionary<string, object>
                            {
                                { "sub_code", subCode },
                                { "total", allBal.Sum() },
                                { "0-30", first030.Sum(e => Convert.ToDecimal(e["amt"])) },
                                { "0-31_detail", new List<Dictionary<string, object>>(first030) },
                                { "31-60", third3160.Sum(e => Convert.ToDecimal(e["amt"])) },
                                { "31-60_detail", new List<Dictionary<string, object>>(third3160) },
                                { "61-90", fourth6190.Sum(e => Convert.ToDecimal(e["amt"])) },
                                { "61-90_detail", new List<Dictionary<string, object>>(fourth6190) },
                                { "91-120", sixth91120.Sum(e => Convert.ToDecimal(e["amt"])) },
                                { "91-120_detail", new List<Dictionary<string, object>>(sixth91120) },
                                { "120+", seventh120.Sum(e => Convert.ToDecimal(e["amt"])) },
                                { "120+_detail", new List<Dictionary<string, object>>(seventh120) }
                            };
                        // Reset data
                        allBal.Clear();
                        first030.Clear();
                        third3160.Clear();
                        fourth6190.Clear();
                        sixth91120.Clear();
                        seventh120.Clear();
                    }

                    // Assign current values for next iteration
                    subCode = currentSubCode;
                    bal = Convert.ToDecimal(i.PENDING_BAL);
                    vNo = i.VOUCHER_NO;
                    days = Convert.ToInt32(i.AGE);
                    x++;
                }
            }


            // Add final balance after loop ends

            allBal.Add(bal);


            Console.WriteLine("step2");


            var lastEntry = new Dictionary<string, object>
                {
                    { "amt", bal },
                    { "v_no", vNo },
                    { "days", days }
                };

            if (days <= interval)
                first030.Add(lastEntry);
            else if (days <= interval * 2)
                third3160.Add(lastEntry);
            else if (days <= interval * 3)
                fourth6190.Add(lastEntry);
            else if (days <= interval * 4)
                sixth91120.Add(lastEntry);
            else
                seventh120.Add(lastEntry);

            if (subCode.HasValue)
            {
                allData[(int)subCode] = new Dictionary<string, object>
                    {
                        { "sub_code", subCode },
                        { "total", allBal.Sum() },
                        { "0-30", first030.Sum(e => Convert.ToDecimal(e["amt"])) },
                        { "0-31_detail", new List<Dictionary<string, object>>(first030) },
                        { "31-60", third3160.Sum(e => Convert.ToDecimal(e["amt"])) },
                        { "31-60_detail", new List<Dictionary<string, object>>(third3160) },
                        { "61-90", fourth6190.Sum(e => Convert.ToDecimal(e["amt"])) },
                        { "61-90_detail", new List<Dictionary<string, object>>(fourth6190) },
                        { "91-120", sixth91120.Sum(e => Convert.ToDecimal(e["amt"])) },
                        { "91-120_detail", new List<Dictionary<string, object>>(sixth91120) },
                        { "120+", seventh120.Sum(e => Convert.ToDecimal(e["amt"])) },
                        { "120+_detail", new List<Dictionary<string, object>>(seventh120) }
                    };
            }

            // Fetch customer details

            List<CustomerModel> customerList = new List<CustomerModel>();

            string custQuery = $@"
    SELECT 
        CAST(NVL(customer_code, 0) AS NUMBER(20,2)) customer_code,
        CUSTOMER_EDESC,
        GROUP_SKU_FLAG,
        MASTER_CUSTOMER_CODE,
        PRE_CUSTOMER_CODE 
    FROM SA_CUSTOMER_SETUP 
    WHERE 1=1 and company_code in ({formatedCompanyCodes})  AND deleted_flag = 'N' 
    GROUP BY CUSTOMER_CODE, CUSTOMER_EDESC, GROUP_SKU_FLAG, MASTER_CUSTOMER_CODE, PRE_CUSTOMER_CODE 
    ORDER BY MASTER_CUSTOMER_CODE";

            using (OracleConnection objConn = new OracleConnection(tokens[1]))
            {
                objConn.Open();
                using (OracleCommand objCmd = new OracleCommand(custQuery, objConn))
                {
                    using (OracleDataReader reader = objCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = new CustomerModel
                            {
                                CUSTOMER_CODE = reader["CUSTOMER_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CUSTOMER_CODE"]),
                                CUSTOMER_EDESC = reader["CUSTOMER_EDESC"] == DBNull.Value ? "" : reader["CUSTOMER_EDESC"].ToString(),
                                GROUP_SKU_FLAG = reader["GROUP_SKU_FLAG"] == DBNull.Value ? "" : reader["GROUP_SKU_FLAG"].ToString(),
                                MASTER_CUSTOMER_CODE = reader["MASTER_CUSTOMER_CODE"] == DBNull.Value ? "" : reader["MASTER_CUSTOMER_CODE"].ToString(),
                                PRE_CUSTOMER_CODE = reader["PRE_CUSTOMER_CODE"] == DBNull.Value ? "" : reader["PRE_CUSTOMER_CODE"].ToString()
                            };

                            customerList.Add(item);
                        }
                    }
                }
            }



            var resultList = new List<Dictionary<string, object>>();

            foreach (var customer in customerList)
            {
                int customerCode = Convert.ToInt32(customer.CUSTOMER_CODE);
                var totalData = allData.ContainsKey(customerCode) ? allData[customerCode] : new Dictionary<string, object>();

                resultList.Add(new Dictionary<string, object>
                {
                    { "CUSTOMER_CODE", customer.CUSTOMER_CODE },
                    { "CUSTOMER_EDESC", customer.CUSTOMER_EDESC },
                    { "GROUP_SKU_FLAG", customer.GROUP_SKU_FLAG },
                    { "MASTER_CUSTOMER_CODE", customer.MASTER_CUSTOMER_CODE },
                    { "PRE_CUSTOMER_CODE", customer.PRE_CUSTOMER_CODE },
                    { "total", totalData.ContainsKey("total") ? totalData["total"] : 0 },
                    { "0-30", totalData.ContainsKey("0-30") ? totalData["0-30"] : 0 },
                    { "0-31_detail", totalData.ContainsKey("0-31_detail") ? totalData["0-31_detail"] : new List<Dictionary<string, object>>() },
                    { "31-60", totalData.ContainsKey("31-60") ? totalData["31-60"] : 0 },
                    { "31-60_detail", totalData.ContainsKey("31-60_detail") ? totalData["31-60_detail"] : new List<Dictionary<string, object>>() },
                    { "61-90", totalData.ContainsKey("61-90") ? totalData["61-90"] : 0 },
                    { "61-90_detail", totalData.ContainsKey("61-90_detail") ? totalData["61-90_detail"] : new List<Dictionary<string, object>>() },
                    { "91-120", totalData.ContainsKey("91-120") ? totalData["91-120"] : 0 },
                    { "91-120_detail", totalData.ContainsKey("91-120_detail") ? totalData["91-120_detail"] : new List<Dictionary<string, object>>() },
                    { "120+", totalData.ContainsKey("120+") ? totalData["120+"] : 0 },
                    { "120+_detail", totalData.ContainsKey("120+_detail") ? totalData["120+_detail"] : new List<Dictionary<string, object>>() },
                });
            }


            var valsqq = new List<Dictionary<string, object>>();

            decimal total1 = 0;

            var Arrtotal1 = new List<decimal>();
            var Arrtotal2 = new List<decimal>();
            var Arrtotal3 = new List<decimal>();
            var Arrtotal4 = new List<decimal>();
            var Arrtotal5 = new List<decimal>();
            var Arrtotal6 = new List<decimal>();

            Console.WriteLine("step4");


            foreach (var i in resultList)
            {
                string groupFlag = i["GROUP_SKU_FLAG"]?.ToString();
                string masterCode = i["MASTER_CUSTOMER_CODE"]?.ToString() ?? "";
                string preCode = i["PRE_CUSTOMER_CODE"]?.ToString() ?? "";

                if (groupFlag == "G")
                {
                    bool add_on = false;
                    total1 = 0;
                    decimal total2 = 0, total3 = 0, total4 = 0, total5 = 0, total6 = 0;

                    foreach (var z in resultList)
                    {
                        if (z["GROUP_SKU_FLAG"]?.ToString() == "I")
                        {
                            string zMaster = z["MASTER_CUSTOMER_CODE"]?.ToString() ?? "";
                            string match = "";

                            try
                            {
                                match = zMaster.Length >= masterCode.Length ? zMaster.Substring(0, masterCode.Length) : "";
                            }
                            catch { match = ""; }

                            if (masterCode != "" && match.Contains(masterCode))
                            {
                                add_on = true;

                                total1 += Convert.ToDecimal(z["total"]);
                                total2 += Convert.ToDecimal(z["0-30"]);
                                total3 += Convert.ToDecimal(z["31-60"]);
                                total4 += Convert.ToDecimal(z["61-90"]);
                                total5 += Convert.ToDecimal(z["91-120"]);
                                total6 += Convert.ToDecimal(z["120+"]);
                            }
                            else if (add_on)
                            {
                                break;
                            }
                        }
                    }

                    if (total1 > 0)
                    {
                        valsqq.Add(new Dictionary<string, object>
                {
                    { "item", i["CUSTOMER_EDESC"] },
                    { "group_flag", groupFlag },
                    { "total", total1 },
                    { "level", masterCode.Count(c => c == '.') },
                    { "master", masterCode },
                    { "pre", preCode },
                    { "code", i["CUSTOMER_CODE"] },
                    { "0-30", total2 },
                    { "31-60", total3 },
                    { "61-90", total4 },
                    { "91-120", total5 },
                    { "120+", total6 }
                });
                    }
                }

                if (groupFlag == "I" && Convert.ToDecimal(i["total"]) > 0)
                {
                    Arrtotal1.Add(Convert.ToDecimal(i["total"]));
                    Arrtotal2.Add(Convert.ToDecimal(i["0-30"]));
                    Arrtotal3.Add(Convert.ToDecimal(i["31-60"]));
                    Arrtotal4.Add(Convert.ToDecimal(i["61-90"]));
                    Arrtotal5.Add(Convert.ToDecimal(i["91-120"]));
                    Arrtotal6.Add(Convert.ToDecimal(i["120+"]));

                    valsqq.Add(new Dictionary<string, object>
            {
                { "item", i["CUSTOMER_EDESC"] },
                { "group_flag", groupFlag },
                { "total", Convert.ToDecimal(i["total"]) },
                { "level", masterCode.Count(c => c == '.') },
                { "master", masterCode },
                { "pre", preCode },
                { "code", i["CUSTOMER_CODE"] },
                { "0-30", Convert.ToDecimal(i["0-30"]) },
                { "31-60", Convert.ToDecimal(i["31-60"]) },
                { "61-90", Convert.ToDecimal(i["61-90"]) },
                { "91-120", Convert.ToDecimal(i["91-120"]) },
                { "120+", Convert.ToDecimal(i["120+"]) },
                { "0-30_detail", i["0-31_detail"] },
                { "31-60_detail", i["31-60_detail"] },
                { "61-90_detail", i["61-90_detail"] },
                { "91-120_detail", i["91-120_detail"] },
                { "120+_detail", i["120+_detail"] }
            });
                }
            }

            var response = new Dictionary<string, object>
            {
                ["detail"] = valsqq,
                ["total"] = new Dictionary<string, object>
                {
                    ["0-30"] = Arrtotal2.Sum(),
                    ["31-60"] = Arrtotal3.Sum(),
                    ["61-90"] = Arrtotal4.Sum(),
                    ["91-120"] = Arrtotal5.Sum(),
                    ["120+"] = Arrtotal6.Sum(),
                    ["total"] = Arrtotal1.Sum()
                }
            };

            return response;

            //return allData;
        }

        public dynamic billwiseAgeingTransactions(Dictionary<string, string> model, NeoErpCoreEntity dbContext)
        {
            string sConnStr1 = ConfigurationManager.ConnectionStrings["NeoErpCoreEntity"].ToString();
            string[] tokens = sConnStr1.Split('"');
            using (OracleConnection conn = new OracleConnection(tokens[1]))
            {
                string cusCode = model.ContainsKey("cus_code") ? model["cus_code"] : "";
                string cusCondition = "";
                string cu_code = "";
                if (!string.IsNullOrWhiteSpace(cusCode))
                {
                    cusCondition = $" and SUB_CODE= 'C{cusCode.Replace("'", "''")}' ";
                    cu_code = $" and customer_code='{cusCode}' ";
                }

                string branchCode = model["BRANCH_CODE"];
                string company_code = model["COMPANY_CODE"];

                string formatedBranchCodes;
                if (branchCode.Contains("[") && branchCode.Contains("]"))
                {
                    formatedBranchCodes = branchCode.Replace("[", "'").Replace("]", "'").Replace(", ", "','");
                }
                else
                {
                    formatedBranchCodes = $"'{branchCode}'";
                }

                string formatedCompanyCodes;
                if (company_code.Contains("[") && company_code.Contains("]"))
                {
                    formatedCompanyCodes = company_code.Replace("[", "'").Replace("]", "'").Replace(", ", "','");
                }
                else
                {
                    formatedCompanyCodes = $"'{company_code}'";
                }


                //string formatedBranchCodes = model["BRANCH_CODE"].Replace("[", "'").Replace("]", "'").Replace(", ", "','");
                //string formatedCompanyCodes = model["COMPANY_CODE"].Replace("[", "'").Replace("]", "'").Replace(", ", "','");

                string openingStr1 = " voucher_no != '0' and   ";
                string openingStr2 = $@"
             UNION ALL
             SELECT TO_CHAR('C' || CUSTOMER_CODE), TO_CHAR(REFERENCE_NO), INVOICE_DATE,0 AS DR_AMOUNT, SUM(BALANCE_AMOUNT)  AS CR_AMOUNT ,1
             FROM SA_CUSTOMER_OPENING_SETUP where company_code in ({formatedCompanyCodes}) {cu_code}
             AND TRANSACTION_TYPE='CR'
             GROUP BY CUSTOMER_CODE, REFERENCE_NO, INVOICE_DATE
             UNION ALL
             SELECT TO_CHAR('C' || CUSTOMER_CODE), TO_CHAR(REFERENCE_NO), INVOICE_DATE,SUM(BALANCE_AMOUNT) AS DR_AMOUNT,0 AS CR_AMOUNT,1
             FROM SA_CUSTOMER_OPENING_SETUP where company_code in ({formatedCompanyCodes}) {cu_code}
             AND TRANSACTION_TYPE='DR'
             GROUP BY CUSTOMER_CODE, REFERENCE_NO, INVOICE_DATE";

                string query = $@"
            select a.sub,a.voucher_date,
                a.voucher_no,
                case when amm =1 then a.closing_balance else  a.pending_bal end as   pending_bal,
                a.age,
                a.closing_balance,
                a.amm,a.dr_amount,
                --(SELECT reference_no FROM FINANCIAL_REFERENCE_DETAIL
                        --WHERE company_code in ({formatedCompanyCodes}) and a.voucher_no=voucher_no
                        --GROUP BY form_code, reference_form_code, voucher_no, reference_no) manual_no,
                b.REFERENCE_NO manual_no,
                        BS_DATE(a.voucher_date) MITI
            from
            (SELECT sub,voucher_date,voucher_no, pending_bal,age, closing_balance,
                        ROW_NUMBER() OVER (PARTITION BY sub ORDER BY voucher_date,sub) amm,dr_amount  FROM
                         (
                         select * from 
                        (
                        select to_number(substr(sub_code,2,99)) sub ,voucher_date,voucher_no,
                        case when qw=1 then net_amount else dr_amount end as pending_bal ,
                        trunc(sysdate)-trunc(voucher_date) age,closing_balance,dr_amount 
                            from(select sub_code,voucher_no,voucher_date,dr_amount,cr_amount,qw,(dr_amount - nvl(cr_amount,0)) AS net_amount
                        ,SUM(dr_amount - nvl(cr_amount,0)) OVER (PARTITION BY to_number(substr(sub_code,2,99)) ORDER BY voucher_date,voucher_no,serial_no) AS closing_balance 
                        from(select sub_code,voucher_no,voucher_date,dr_amount,qw,case when qw=1 then cr_amount else 0 end as cr_amount,serial_no
                        from(select sub_code,voucher_no,voucher_date,dr_amount,cr_amount, ROW_NUMBER() OVER (PARTITION BY to_number(substr(sub_code,2,9999)) ORDER BY to_number(substr(sub_code,2,9999)),voucher_date,voucher_no,serial_no) AS qw,serial_no
                        from (                            
                        select sub_code,voucher_no,voucher_date,dr_amount, SUM(cr_amount) OVER (PARTITION BY sub_code) cr_amount ,serial_no
                        from (
                        SELECT TO_CHAR(sub_code) sub_code, TO_CHAR(voucher_no) voucher_no, voucher_date, dr_amount, cr_amount ,serial_no
                        FROM V$VIRTUAL_SUB_LEDGER WHERE  {openingStr1}                            
                        company_code in ({formatedCompanyCodes}) and substr(sub_code,1,1)='C' {cusCondition}
                        AND VOUCHER_NO NOT IN(SELECT DISTINCT NVL(BOUNCE_VC_NO,'A000000') FROM FA_PDC_RECEIPTS WHERE (ENCASH_DATE IS NOT NULL OR BOUNCE_DATE IS NOT NULL) AND BOUNCE_FLAG='Y' )  
                        and form_code not in (select distinct form_code from form_detail_setup where table_name='FA_PAY_ORDER' and company_code in ({formatedCompanyCodes})) {openingStr2}
                        )                            
                        order by to_number(substr(sub_code,2,99)),voucher_date,voucher_no
                        )WHERE dr_amount != 0
                        )) ) 
                        )                            
                         ) ) a  , FINANCIAL_REFERENCE_DETAIL b
                        WHERE a.VOUCHER_NO = b.VOUCHER_NO and b.company_code in ({formatedCompanyCodes}) order by age desc
        ";

                var resultList = new List<dynamic>();
                decimal billAmtTotal = 0m;
                decimal adjustedTotal = 0m;
                decimal balanceTotal = 0m;
                bool getAdjust = false;
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = query;
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            DateTime? voucherDate = rdr.IsDBNull(1) ? (DateTime?)null : rdr.GetDateTime(1);
                            string vDateStr = voucherDate?.ToString("dd-MMM-yyyy") ?? "";

                            decimal pendingBal = rdr.IsDBNull(3) ? 0 : Convert.ToDecimal(rdr.GetValue(3));
                            decimal closingBal = rdr.IsDBNull(5) ? 0 : Convert.ToDecimal(rdr.GetValue(5));
                            decimal drAmt = rdr.IsDBNull(7) ? 0 : Convert.ToDecimal(rdr.GetValue(7));

                            decimal adj = 0;
                            if (closingBal < 0)
                            {
                                adj = 0;
                            }
                            else
                            {
                                if (!getAdjust)
                                {
                                    adj = closingBal;
                                    getAdjust = true;
                                }
                                else
                                {
                                    adj = pendingBal;
                                }
                            }
                            if (adj == 0) continue;
                            resultList.Add(new
                            {
                                v_no = rdr.IsDBNull(2) ? "" : rdr.GetString(2),
                                v_date = vDateStr,
                                days = rdr.IsDBNull(4) ? 0 : Convert.ToInt32(rdr.GetValue(4)),
                                balance = Math.Round(adj, 2),
                                bill_amt = Math.Round(drAmt, 2),
                                status = Math.Round(pendingBal, 2) < 0 ? "adjusted" : "pending",
                                due_date = vDateStr,
                                due_miti = rdr.IsDBNull(9) ? "" : rdr.GetValue(9).ToString(),
                                manual_no = rdr.IsDBNull(8) ? "" : rdr.GetValue(8).ToString(),
                                miti = rdr.IsDBNull(9) ? "" : rdr.GetValue(9).ToString(),
                                adjusted_amt = Math.Round(drAmt - adj, 2)
                            });
                            billAmtTotal += drAmt;
                            adjustedTotal += drAmt - adj;
                            balanceTotal += adj;

                        }
                    }
                }

                var response = new
                {
                    transactions = resultList,
                    balance_total = Math.Round(balanceTotal, 2),
                    bill_amt_total = Math.Round(billAmtTotal, 2),
                    adjusted_total = Math.Round(adjustedTotal, 2)
                };

                return response;
            }
        }
        public dynamic billwiseAgeingTransactionsPdf(Dictionary<string, string> model, NeoErpCoreEntity dbContext)
        {
            var data = billwiseAgeingTransactions(model, dbContext);
            string customerName = "";
            string mobile = "";
            string cityCode = "";
            string address = "";
            string formatedCompanyCodes;
            string company_code = model["COMPANY_CODE"];
            if (company_code.Contains("[") && company_code.Contains("]"))
            {
                formatedCompanyCodes = company_code.Replace("[", "'").Replace("]", "'").Replace(", ", "','");
            }
            else
            {
                formatedCompanyCodes = $"'{company_code}'";
            }
            using (var conn = new OracleConnection(ConfigurationManager.ConnectionStrings["NeoErpCoreEntity"].ToString().Split('"')[1]))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $@"
                SELECT customer_edesc, 
                       NVL(tel_mobile_no1, NVL(tel_mobile_no2, tel_mobile_no3)) AS mobile, 
                       a.city_code, 
                       NVL(b.city_edesc, regd_office_eaddress) AS address
                FROM sa_customer_setup a
                JOIN city_code b 
                  ON a.city_code = b.city_code AND b.deleted_flag = 'N'
                WHERE customer_code = '{model["cus_code"]}' AND company_code in ({formatedCompanyCodes})";

                    using (var rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            customerName = rdr.IsDBNull(0) ? "" : rdr.GetString(0);
                            mobile = rdr.IsDBNull(1) ? "" : rdr.GetString(1);
                            cityCode = rdr.IsDBNull(2) ? "" : rdr.GetString(2);
                            address = rdr.IsDBNull(3) ? "" : rdr.GetString(3);
                        }
                    }
                }
            }
            var transactions = ((IEnumerable<dynamic>)data.transactions).ToList();
            decimal billTotal = data.bill_amt_total;
            decimal adjustedTotal = data.adjusted_total;

            string toGivePath = @"\Areas\NeoErp.Distribution\Images\Uploads\Reports\BillwiseAgeing";
            string folderPath = Path.Combine(UploadPath, "Uploads", "Reports", "BillwiseAgeing");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string fileName = $"BillwiseAgeing_{model["SP_CODE"]}_{model["cus_code"]}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            string filePath = Path.Combine(folderPath, fileName);
            string returnPath = Path.Combine(toGivePath, fileName);

            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (Document doc = new Document(PageSize.A4.Rotate(), 40, 40, 25, 25))
            {
                PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                doc.Open();

                Paragraph title = new Paragraph("Billwise Ageing Transactions Report",
                    FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14))
                {
                    Alignment = Element.ALIGN_CENTER
                };
                doc.Add(title);
                doc.Add(new Paragraph(" "));

                doc.Add(new Paragraph($"Generated On: {DateTime.Now:yyyy-MM-dd hh:mm tt}",
                    FontFactory.GetFont(FontFactory.HELVETICA, 10)));
                doc.Add(new Paragraph(" "));

                PdfPTable custTable = new PdfPTable(2);
                custTable.WidthPercentage = 100;
                custTable.SetWidths(new float[] { 2f, 6f });

                custTable.AddCell(new PdfPCell(new Phrase("Customer Name:", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10))) { Border = 0 });
                custTable.AddCell(new PdfPCell(new Phrase(customerName, FontFactory.GetFont(FontFactory.HELVETICA, 10))) { Border = 0 });

                custTable.AddCell(new PdfPCell(new Phrase("Mobile:", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10))) { Border = 0 });
                custTable.AddCell(new PdfPCell(new Phrase(mobile, FontFactory.GetFont(FontFactory.HELVETICA, 10))) { Border = 0 });

                custTable.AddCell(new PdfPCell(new Phrase("City Code:", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10))) { Border = 0 });
                custTable.AddCell(new PdfPCell(new Phrase(cityCode, FontFactory.GetFont(FontFactory.HELVETICA, 10))) { Border = 0 });

                custTable.AddCell(new PdfPCell(new Phrase("Address:", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10))) { Border = 0 });
                custTable.AddCell(new PdfPCell(new Phrase(address, FontFactory.GetFont(FontFactory.HELVETICA, 10))) { Border = 0 });

                doc.Add(custTable);
                doc.Add(new Paragraph(" "));

                List<string> headers = new List<string>
                {
                    "Voucher No",
                    "Bill No",
                    "Voucher Date",
                    "Bill Amount",
                    "Adjusted Amount",
                    "Balance",
                    "Due Days",
                };

                PdfPTable table = new PdfPTable(headers.Count);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 2f, 2f, 1.5f, 1.75f, 1.75f, 1.75f, 1f });

                foreach (var h in headers)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(h,
                        FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE)))
                    {
                        BackgroundColor = new BaseColor(40, 167, 69),
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 5
                    };
                    table.AddCell(cell);
                }

                foreach (var item in transactions)
                {
                    table.AddCell(new PdfPCell(new Phrase(item.v_no ?? ""))
                    { HorizontalAlignment = Element.ALIGN_LEFT });

                    table.AddCell(new PdfPCell(new Phrase(item.manual_no ?? ""))
                    { HorizontalAlignment = Element.ALIGN_LEFT });

                    table.AddCell(new PdfPCell(new Phrase(item.v_date ?? ""))
                    { HorizontalAlignment = Element.ALIGN_CENTER });

                    table.AddCell(new PdfPCell(new Phrase(
                        Convert.ToDecimal(item.bill_amt).ToString("N2")))
                    { HorizontalAlignment = Element.ALIGN_RIGHT });

                    table.AddCell(new PdfPCell(new Phrase(
                        Convert.ToDecimal(item.adjusted_amt).ToString("N2")))
                    { HorizontalAlignment = Element.ALIGN_RIGHT });

                    table.AddCell(new PdfPCell(new Phrase(
                        Convert.ToDecimal(item.balance).ToString("N2")))
                    { HorizontalAlignment = Element.ALIGN_RIGHT });

                    table.AddCell(new PdfPCell(new Phrase(item.days.ToString()))
                    { HorizontalAlignment = Element.ALIGN_RIGHT });
                }

                doc.Add(table);

                doc.Add(new Paragraph(" "));

                Paragraph billPara = new Paragraph($"Total Bill Amount: {billTotal:N2}",
                    FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11))
                {
                    Alignment = Element.ALIGN_RIGHT
                };

                Paragraph adjPara = new Paragraph($"Total Adjusted Amount: {adjustedTotal:N2}",
                    FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11))
                {
                    Alignment = Element.ALIGN_RIGHT
                };

                doc.Add(billPara);
                doc.Add(adjPara);

                doc.Close();
                writer.Close();
            }

            return returnPath.Replace("\\", "/");
        }
        public dynamic fetchOrderList(Dictionary<string, string> model, NeoErpCoreEntity dbContext)
        {
            string sConnStr1 = ConfigurationManager.ConnectionStrings["NeoErpCoreEntity"].ToString();
            string[] tokens = sConnStr1.Split('"');
            using (OracleConnection conn = new OracleConnection(tokens[1]))
            {
                string branchCode = model["BRANCH_CODE"];
                string company_code = model["COMPANY_CODE"];

                string formatedBranchCodes;
                if (branchCode.Contains("[") && branchCode.Contains("]"))
                {
                    formatedBranchCodes = branchCode.Replace("[", "'").Replace("]", "'").Replace(", ", "','");
                }
                else
                {
                    formatedBranchCodes = $"'{branchCode}'";
                }

                string formatedCompanyCodes;
                if (company_code.Contains("[") && company_code.Contains("]"))
                {
                    formatedCompanyCodes = company_code.Replace("[", "'").Replace("]", "'").Replace(", ", "','");
                }
                else
                {
                    formatedCompanyCodes = $"'{company_code}'";
                }
                var query = $@"select c.customer_edesc, d.item_edesc, a.* from dist_ip_ssd_purchase_order a, dist_login_user b, sa_customer_setup c, ip_item_master_setup d
                    where a.created_by = b.USERID and a.company_code = b.company_code and a.company_code = c.company_code and a.customer_code = c.CUSTOMER_CODE
                    and a.company_code = d.company_code and a.item_code = d.item_code
                    and a.REJECT_FLAG = 'N' and a.DELETED_FLAG = 'N' and (a.APPROVE_QTY is NULL or a.approve_qty = 0)
                    and b.sp_code = '{model["SP_CODE"]}' and a.company_code in ({formatedCompanyCodes})";

                var resultList = new List<dynamic>();
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = query;
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            resultList.Add(new
                            {
                                ORDER_NO = rdr["ORDER_NO"].ToString(),
                                ORDER_DATE = rdr["ORDER_DATE"].ToString(),
                                CUSTOMER_CODE = rdr["CUSTOMER_CODE"].ToString(),
                                CUSTOMER_NAME = rdr["CUSTOMER_EDESC"].ToString(),
                                ITEM_CODE = rdr["ITEM_CODE"].ToString(),
                                ITEM_NAME = rdr["ITEM_EDESC"].ToString(),
                                MU_CODE = rdr["MU_CODE"].ToString(),
                                QUNATITY = rdr["QUNATITY"].ToString(),
                                UNIT_PRICE = rdr["UNIT_PRICE"].ToString(),
                                TOTAL_PRICE = rdr["TOTAL_PRICE"].ToString(),
                                BILLING_NAME = rdr["BILLING_NAME"].ToString(),
                                REMARKS = rdr["REMARKS"].ToString(),
                                DISCOUNT = rdr["DISCOUNT"].ToString(),
                                DISCOUNT_RATE = rdr["DISCOUNT_RATE"].ToString(),
                                DISCOUNT_PERCENTAGE = rdr["DISCOUNT_PERCENTAGE"].ToString()
                            });
                        }
                    }
                }

                return resultList;
            }
        }
        public dynamic updatePurchaseOrder(Dictionary<string, string> model, NeoErpCoreEntity dbContext)
        {
            string sConnStr1 = ConfigurationManager.ConnectionStrings["NeoErpCoreEntity"].ToString();
            string[] tokens = sConnStr1.Split('"');
            string branchCode = model["BRANCH_CODE"];
            string company_code = model["COMPANY_CODE"];

            string formatedBranchCodes;
            if (branchCode.Contains("[") && branchCode.Contains("]"))
            {
                formatedBranchCodes = branchCode.Replace("[", "'").Replace("]", "'").Replace(", ", "','");
            }
            else
            {
                formatedBranchCodes = $"'{branchCode}'";
            }

            string formatedCompanyCodes;
            if (company_code.Contains("[") && company_code.Contains("]"))
            {
                formatedCompanyCodes = company_code.Replace("[", "'").Replace("]", "'").Replace(", ", "','");
            }
            else
            {
                formatedCompanyCodes = $"'{company_code}'";
            }
            if (string.IsNullOrWhiteSpace(model["ORDER_NO"]))
                throw new Exception("Order No not found!");

            var query = $@"update dist_ip_ssd_purchase_order set deleted_flag = 'Y' where order_no = '{model["ORDER_NO"]}' and company_code in ({formatedCompanyCodes})";

            var result = dbContext.ExecuteSqlCommand(query);
            if (result > 0)
                return "Successfully updated!";
            else
                throw new Exception("Something went wrong!");
        }
        private class FarmProblemModel
        {
            public string FARMER_ID { get; set; }
            public string FARMER_EDESC { get; set; }
            public string FARM_EDESC { get; set; }
            public string ADDRESS { get; set; }
            public string CONTACT_NO { get; set; }
            public string AREA_CODE { get; set; }
            public string FARM_LONGITUDE { get; set; }
            public string FARM_LATITUDE { get; set; }
            public string FARM_AREA { get; set; }
            public string FARMING_CROPS { get; set; }
            public string EXPERIENCE { get; set; }
            public string REMARKS { get; set; }

            public string PROFILE_IMG { get; set; }

        }
        public dynamic saveFarmProblems(Dictionary<string, dynamic> model, NeoErpCoreEntity dbContext)
        {
            string farmerId = model["farmerId"];
            string companyCode = model["COMPANY_CODE"];
            string branchCode = model["BRANCH_CODE"];
            string sp_code = model["SP_CODE"];
            string altitude = model["altitude"] ?? string.Empty;

            var farmer = dbContext.SqlQuery<FarmProblemModel>($@" SELECT *  FROM DIST_FARMER_MASTER  WHERE FARMER_ID = '{farmerId}' AND ROWNUM = 1").FirstOrDefault();

            if (farmer == null)
                throw new Exception("Farmer not found");

            var farmProblems = model["farmProblems"] as List<Dictionary<string, object>>;
            int insertCount = 0;

            foreach (var crop in farmProblems)
            {
                string cropName = crop["cropName"].ToString();
                var problems = crop["problems"] as List<Dictionary<string, object>>;

                foreach (var problem in problems)
                {
                    string sql = $@"
                INSERT INTO DIST_FARMER_RECOMMENDATIONS
                (
                    FARMER_ID, FARMER_NAME, FARM_NAME, ADDRESS, LAND_SIZE, LONGITUDE, LATITUDE, ALTITUDE, PROBLEM_CODE, CROP_NAME, CREATED_DATE, CREATED_BY, COMPANY_CODE, REMARKS, RECOMMENDATION
                )
                VALUES
                (
                    '{farmerId}', '{farmer.FARMER_EDESC}', '{farmer.FARM_EDESC}', '{farmer.ADDRESS}', '{farmer.FARM_AREA}',
                    '{farmer.FARM_LONGITUDE}', '{farmer.FARM_LATITUDE}', '{altitude}', {problem["ID"]}, '{cropName}', SYSDATE, '{sp_code}', '{companyCode}', '{problem["description"]}','{problem["recommendation"]}'
                )";

                    insertCount += dbContext.ExecuteSqlCommand(sql);
                }
            }

            if (insertCount > 0)
                return "Successfully saved farmer recommendations";
            else
                throw new Exception("Insert failed");
        }
        public dynamic FetchVoucherPdf(TransactionRequestModel model, NeoErpCoreEntity dbContext, dynamic tok)
        {
            dynamic data = FetchVoucher(model, dbContext, tok);
            if (data == null || data.ToString().Contains("Voucher doesn't exist"))
                throw new Exception("No voucher data found.");

            string toGivePath = @"Areas\NeoErp.Distribution\Images\Uploads\Vouchers";
            string folderPath = Path.Combine(UploadPath, "Uploads", "Vouchers");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string voucherNo = data["voucher_no"] != null ? data["voucher_no"].ToString() : "Unknown";
            string safeVoucherNo = string.Join("_", voucherNo.Split(Path.GetInvalidFileNameChars()));
            string fileName = $"Voucher_{safeVoucherNo}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            string filePath = Path.Combine(folderPath, fileName);
            string filePath1 = Path.Combine(toGivePath, fileName);

            string htmlContent = GenerateVoucherHtml(data);

            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (Document doc = new Document(PageSize.A4, 50, 50, 35, 35))
            {
                PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                doc.Open();
                doc.Add(new Paragraph(" "));
                using (var srHtml = new StringReader(htmlContent))
                {
                    XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, srHtml);
                }

                doc.Close();
                writer.Close();
            }

            return filePath1.Replace("\\", "/");
        }
        private string GenerateVoucherHtml(dynamic data)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(@"
<!DOCTYPE html>
<html><head><meta charset=""utf-8"" />
<title>Voucher</title>
<style>
    body{font-family:Arial,Helvetica,sans-serif;margin:30px;font-size:13px;}
    table{width:100%;border-collapse:collapse;margin-top:15px;}
    th,td{border:1px solid #000;padding:6px;white-space: nowrap;}
    th{background:#f0f0f0;}
    .right{text-align:right;}
    .center{text-align:center;}
    .header{font-weight:bold;font-size:16px;margin-bottom:20px;}
    .footer{margin-top:30px;font-size:11px;}
    .amt{font-weight:bold;}
</style></head><body>");

            // ---------- COMMON HEADER ----------
            sb.Append($@"
<div class=""header"">
    Voucher No: {data["voucher_no"]} &nbsp;&nbsp;&nbsp; <br />
    Voucher Date: {data["voucher_date"]}<br/>
    Created By: {data["created_by"]} ({data["created_date"]})
    &nbsp;&nbsp; Authorised By: {(data["authorised_by"] ?? "-")}
</div>");

            string formType = data["form_type"] ?? data["type"] ?? "";

            // ------------------------------------------------------------
            // 1. OT (Sales / Purchase / Requisition)
            // ------------------------------------------------------------
            if (formType == "OT")
            {
                sb.Append($@"
<h3>{formType} – {(data["party"] ?? "")}</h3>
<table>
<tr><th>S.N.</th><th>Item</th><th>Unit</th><th class=""right"">Qty</th><th class=""right"">Rate</th><th class=""right"">Amount</th></tr>");

                int sn = 1;
                foreach (var d in (IEnumerable<dynamic>)data["detail"])
                {
                    sb.Append($@"
                    <tr>
                        <td>{sn++}</td>
                        <td>{d["item"]}</td>
                        <td>{d["unit"]}</td>
                        <td class=""right"">{d["qty"]}</td>
                        <td class=""right"">{d["rate"]}</td>
                        <td class=""right"">{d["amt"]}</td>
                    </tr>");
                }

                sb.Append($@"
<tr><td colspan=""3"" class=""right""><b>Total Qty</b></td><td class=""right amt"">{data["total_qty"]}</td><td colspan=""2""></td></tr>
<tr><td colspan=""5"" class=""right""><b>Total Amount</b></td><td class=""right amt"">{data["total_net_amt"]}</td></tr>");

                // ----- charges -----
                if (data["charge"] != null && ((IEnumerable<dynamic>)data["charge"]).Any())
                {
                    sb.Append("<tr><td colspan='6'><b>Charges</b></td></tr>");
                    foreach (var c in (IEnumerable<dynamic>)data["charge"])
                    {
                        string sign = c["charge_type"] == "A" ? "+" : "-";
                        sb.Append($@"<tr><td colspan=""4"">{c["name"]}</td><td class=""right"">{sign}{c["amt"]}</td></tr>");
                    }
                    sb.Append($@"<tr><td colspan=""4"" class=""right""><b>Net Amount</b></td><td class=""right amt"">{data["total_amt"]}</td></tr>");
                }

                // ----- special requisition fields -----
                //if (data.from_loc != null)
                //    sb.Append($@"<div><b>From:</b> {data["from_loc"]} &nbsp;&nbsp; <b>To:</b> {data["to_loc"]}</div>");

                sb.Append("</table>");
            }

            // ------------------------------------------------------------
            // 2. CH / BK (single-voucher)
            // ------------------------------------------------------------
            else if (formType == "CH" || formType == "BK")
            {
                sb.Append("<h3>Single Voucher</h3><table>");
                sb.Append("<tr><th>S.N.</th><th>Account</th><th class=\"right\">Dr</th><th class=\"right\">Cr</th></tr>");

                int sn = 1;
                foreach (var d in (IEnumerable<dynamic>)data["detail"])
                {
                    string accName = System.Net.WebUtility.HtmlEncode((string)d["acc_edesc"]);
                    string accCode = System.Net.WebUtility.HtmlEncode((string)d["acc_code"]);
                    sb.Append($@"
                        <tr>
                            <td>{sn++}</td>
                            <td>{accName} ({accCode})</td>
                            <td class=""right""><span>{d["dr_amt"]}</span></td>
                            <td class=""right""><span>{d["cr_amt"]}</span></td>
                        </tr>");

                    // sub-ledger
                    if (d["sl"] != null && ((IEnumerable<dynamic>)d["sl"]).Any())
                    {
                        foreach (var s in (IEnumerable<dynamic>)d["sl"])
                            sb.Append($@"<tr><td></td><td style=""padding-left:30px;"">{s["name"]}</td><td></td><td class=""right"">{s["amt"]}</td></tr>");
                    }
                    // budget centre
                    if (d["cc"] != null && ((IEnumerable<dynamic>)d["cc"]).Any())
                    {
                        foreach (var c in (IEnumerable<dynamic>)d["cc"])
                            sb.Append($@"<tr><td></td><td style=""padding-left:30px;"">{c["name"]}</td><td></td><td class=""right"">{c["amt"]}</td></tr>");
                    }
                }

                sb.Append($@"
                    <tr><td colspan=""2"" class=""right""><b>Total</b></td>
                        <td class=""right amt"">{data["dr_total"]}</td>
                        <td class=""right amt"">{data["cr_total"]}</td></tr>");
                sb.Append("</table>");
            }

            // ------------------------------------------------------------
            // 3. JV (double-voucher)
            // ------------------------------------------------------------
            else if (formType == "JV")
            {
                sb.Append("<h3>Journal Voucher</h3><table>");
                sb.Append("<tr><th>S.N.</th><th>Account</th><th class=\"right\">Dr</th><th class=\"right\">Cr</th></tr>");

                int sn = 1;
                foreach (var d in (IEnumerable<dynamic>)data["detail"])
                {
                    string accName = System.Net.WebUtility.HtmlEncode((string)d["acc_edesc"]);
                    string accCode = System.Net.WebUtility.HtmlEncode((string)d["acc_code"]);
                    sb.Append($@"
                        <tr>
                            <td>{sn++}</td>
                            <td>{accName} ({accCode})</td>
                            <td class=""right""><span>{d["dr_amt"]}</span></td>
                            <td class=""right""><span>{d["cr_amt"]}</span></td>
                        </tr>");

                    // sub-ledger / budget same as CH/BK
                    if (d["sl"] != null && ((IEnumerable<dynamic>)d["sl"]).Any())
                    {
                        foreach (var s in (IEnumerable<dynamic>)d["sl"])
                            sb.Append($@"<tr><td></td><td style=""padding-left:30px;"">{s["name"]}</td><td></td><td class=""right"">{s["amt"]}</td></tr>");
                    }
                    if (d["cc"] != null && ((IEnumerable<dynamic>)d["cc"]).Any())
                    {
                        foreach (var c in (IEnumerable<dynamic>)d["cc"])
                            sb.Append($@"<tr><td></td><td style=""padding-left:30px;"">{c["name"]}</td><td></td><td class=""right"">{c["amt"]}</td></tr>");
                    }
                }

                sb.Append($@"
                    <tr><td colspan=""2"" class=""right""><b>Total</b></td>
                        <td class=""right amt"">{data["dr_total"]}</td>
                        <td class=""right amt"">{data["cr_total"]}</td></tr>");
                sb.Append("</table>");
            }

            // ------------------------------------------------------------
            // Remarks (common)
            // ------------------------------------------------------------
            //if (!string.IsNullOrWhiteSpace((string)data["remarks"]))
            //    sb.Append($@"<div style=""margin-top:20px;""><b>Remarks:</b> {data["remarks"]}</div>");

            sb.Append($@"
<div class=""footer"">
    Printed on: {DateTime.Now:dd-MMM-yyyy HH:mm}
</div>
</body></html>");
            return sb.ToString();
        }
        public dynamic FetchVoucher(TransactionRequestModel model, NeoErpCoreEntity dbContext, dynamic tok)
        {

            Console.WriteLine(model);

            var slData = new List<Dictionary<string, object>>();

            var opening = new Dictionary<string, object>();

            List<double> drsArray = new List<double>();

            List<double> crsArray = new List<double>();

            Dictionary<string, float> ageingVals = new Dictionary<string, float>();

            string branchCode = model.BRANCH_CODE;
            string company_code = model.COMPANY_CODE;

            string formatedBranchCodes;
            if (branchCode.Contains("[") && branchCode.Contains("]"))
            {
                formatedBranchCodes = branchCode.Replace("[", "'").Replace("]", "'").Replace(", ", "','");
            }
            else
            {
                formatedBranchCodes = $"'{branchCode}'";
            }

            string formatedCompanyCodes;
            if (company_code.Contains("[") && company_code.Contains("]"))
            {
                formatedCompanyCodes = company_code.Replace("[", "'").Replace("]", "'").Replace(", ", "','");
            }
            else
            {
                formatedCompanyCodes = $"'{company_code}'";
            }

            Debug.WriteLine(formatedBranchCodes);
            var dateData = dbContext.SqlQuery<DateModel>($"select startdate as START_DATE, enddate as END_DATE from v_date_range where rangename='This Year'").ToList();


            //string ss = model.user_id;

            if (string.IsNullOrEmpty(model.from_date))
            {
                model.from_date = dateData[0].START_DATE.ToString("dd-MMM-yyyy");
            }
            if (string.IsNullOrEmpty(model.to_date))
            {
                model.to_date = dateData[0].END_DATE.ToString("dd-MMM-yyyy");
            }



            string voucherNo = "";
            string voucherDate = "";
            string createdDate = "";
            string createdBy = "";
            string tableName = "";
            string formCode = "";
            string authorisedBy = "";
            string authorisedDate = "";
            string postedBy = "";
            string postedDate = "";
            string checkedBy = "";
            string checkedDate = "";
            string formType = "";
            string rem = "";
            bool exist = false;

            string vNo = "";

            vNo = (string)tok.SelectToken("voucher_no");
            string companyCode = company_code;


            string sConnStr1 = ConfigurationManager.ConnectionStrings["NeoErpCoreEntity"].ToString();
            string[] tokens = sConnStr1.Split('"');


            //using (var connection = new OracleConnection(_connectionString))
            using (OracleConnection connection = new OracleConnection(tokens[1]))
            {
                connection.Open();

                var command = connection.CreateCommand();

                string query1 = $@"
                SELECT a.form_code, b.form_type, a.voucher_no, TRUNC(a.voucher_date), 
                       TRUNC(a.created_date), a.created_by, 
                       (SELECT table_name FROM FORM_DETAIL_SETUP WHERE form_code = a.form_code 
                        AND company_code = '{companyCode}' GROUP BY table_name) tab_name, 
                       authorised_by, NVL(TRUNC(authorised_date), ''), 
                       posted_by, NVL(TRUNC(posted_date), ''), 
                       CHECKED_BY, NVL(TRUNC(CHECKED_date), '') 
                FROM master_transaction a, form_setup b 
                WHERE a.form_code = b.form_code 
                AND a.company_code = b.company_code 
                AND a.voucher_no = '{vNo}' 
                AND a.company_code = '{companyCode}'";

                command.CommandText = query1;
                var result = command.ExecuteReader();

                while (result.Read())
                {
                    formCode = result.IsDBNull(0) ? string.Empty : result.GetString(0);
                    formType = result.IsDBNull(1) ? string.Empty : result.GetString(1);
                    voucherNo = result.IsDBNull(2) ? string.Empty : result.GetString(2);
                    voucherDate = result.IsDBNull(3) ? string.Empty : result.GetDateTime(3).ToString("yyyy-MM-dd");
                    createdBy = result.IsDBNull(5) ? string.Empty : result.GetString(5);
                    createdDate = result.IsDBNull(4) ? string.Empty : result.GetDateTime(4).ToString("yyyy-MM-dd");
                    tableName = result.IsDBNull(6) ? string.Empty : result.GetString(6);
                    authorisedBy = result.IsDBNull(7) ? null : result.GetString(7);
                    authorisedDate = result.IsDBNull(8) ? string.Empty : result.GetDateTime(8).ToString("yyyy-MM-dd");
                    postedBy = result.IsDBNull(9) ? null : result.GetString(9);
                    postedDate = result.IsDBNull(10) ? string.Empty : result.GetDateTime(10).ToString("yyyy-MM-dd");
                    checkedBy = result.IsDBNull(11) ? null : result.GetString(11);
                    checkedDate = result.IsDBNull(12) ? string.Empty : result.GetDateTime(12).ToString("yyyy-MM-dd");
                    exist = true;
                }

                if (tableName == "FA_DOUBLE_VOUCHER")
                {
                    formType = "JV";
                }

                if (exist)
                {

                    if (formType == "OT")
                    {
                        decimal totalQty = 0;
                        decimal totalAmt = 0;
                        string type = "bill";


                        var distinctColumns = new Dictionary<string, Dictionary<string, string>>()
                    {
                        { "SA_QUOTATION_DETAILS", new Dictionary<string, string> { { "V_NO", "QUOTATION_NO" }, { "V_DATE", "QUOTATION_DATE" }, { "PARTY_NAME", "customer_edesc" }, { "PARTY_TABLE_NAME", "sa_customer_setup" }, { "PARTY_CODE", "customer_code" }, { "SUB_CODE", "C" } } },
                        { "SA_SALES_ORDER", new Dictionary<string, string> { { "V_NO", "ORDER_NO" }, { "V_DATE", "ORDER_DATE" }, { "PARTY_NAME", "customer_edesc" }, { "PARTY_TABLE_NAME", "sa_customer_setup" }, { "PARTY_CODE", "customer_code" }, { "SUB_CODE", "C" } } },
                        { "SA_SALES_INVOICE", new Dictionary<string, string> { { "V_NO", "SALES_NO" }, { "V_DATE", "SALES_DATE" }, { "PARTY_NAME", "customer_edesc" }, { "PARTY_TABLE_NAME", "sa_customer_setup" }, { "PARTY_CODE", "customer_code" }, { "SUB_CODE", "C" } } },
                        { "SA_SALES_CHALAN", new Dictionary<string, string> { { "V_NO", "CHALAN_NO" }, { "V_DATE", "CHALAN_DATE" }, { "PARTY_NAME", "customer_edesc" }, { "PARTY_TABLE_NAME", "sa_customer_setup" }, { "PARTY_CODE", "customer_code" }, { "SUB_CODE", "C " } } },
                        { "SA_SALES_RETURN", new Dictionary<string, string> { { "V_NO", "RETURN_NO" }, { "V_DATE", "RETURN_DATE" }, { "PARTY_NAME", "customer_edesc" }, { "PARTY_TABLE_NAME", "sa_customer_setup" }, { "PARTY_CODE", "customer_code" }, { "SUB_CODE", "C" } } },
                        { "IP_PURCHASE_ORDER", new Dictionary<string, string> { { "V_NO", "ORDER_NO" }, { "V_DATE", "ORDER_DATE" }, { "PARTY_NAME", "SUPPLIER_EDESC" }, { "PARTY_TABLE_NAME", "IP_SUPPLIER_SETUP" }, { "PARTY_CODE", "SUPPLIER_CODE" }, { "SUB_CODE", "S" } } },
                        { "IP_PURCHASE_MRR", new Dictionary<string, string> { { "V_NO", "MRR_NO" }, { "V_DATE", "MRR_DATE" }, { "PARTY_NAME", "SUPPLIER_EDESC" }, { "PARTY_TABLE_NAME", "IP_SUPPLIER_SETUP" }, { "PARTY_CODE", "SUPPLIER_CODE" }, { "SUB_CODE", "S" } } },
                        { "IP_PURCHASE_INVOICE", new Dictionary<string, string> { { "V_NO", "INVOICE_NO" }, { "V_DATE", "INVOICE_DATE" }, { "PARTY_NAME", "SUPPLIER_EDESC" }, { "PARTY_TABLE_NAME", "IP_SUPPLIER_SETUP" }, { "PARTY_CODE", "SUPPLIER_CODE" }, { "SUB_CODE", "S" } } },
                        { "IP_PURCHASE_RETURN", new Dictionary<string, string> { { "V_NO", "RETURN_NO" }, { "V_DATE", "RETURN_DATE" }, { "PARTY_NAME", "SUPPLIER_EDESC" }, { "PARTY_TABLE_NAME", "IP_SUPPLIER_SETUP" }, { "PARTY_CODE", "SUPPLIER_CODE" }, { "SUB_CODE", "S" } } },

                        { "IP_ADVICE_MRR", new Dictionary<string, string> { { "V_NO", "MRR_NO" }, { "V_DATE", "MRR_DATE" }, { "PARTY_NAME", "SUPPLIER_EDESC" }, { "PARTY_TABLE_NAME", "IP_SUPPLIER_SETUP" }, { "SUB_CODE", "S" } } },
                        { "IP_TRANSFER_ISSUE", new Dictionary<string, string> { { "V_NO", "ISSUE_NO" }, { "V_DATE", "ISSUE_DATE" }, { "PARTY_NAME", "SUPPLIER_EDESC" }, { "PARTY_TABLE_NAME", "IP_SUPPLIER_SETUP" }, { "SUB_CODE", "S" } } },
                        { "IP_PRODUCTION_ISSUE", new Dictionary<string, string> { { "V_NO", "ISSUE_NO" }, { "V_DATE", "ISSUE_DATE" }, { "PARTY_NAME", "SUPPLIER_EDESC" }, { "PARTY_TABLE_NAME", "IP_SUPPLIER_SETUP" },  { "SUB_CODE", "S" } } },
                        { "IP_PRODUCTION_MRR", new Dictionary<string, string> { { "V_NO", "MRR_NO" }, { "V_DATE", "MRR_DATE" }, { "PARTY_NAME", "SUPPLIER_EDESC" }, { "PARTY_TABLE_NAME", "IP_SUPPLIER_SETUP" },  { "SUB_CODE", "S" } } },
                        { "FA_PAY_ORDER", new Dictionary<string, string> { { "V_NO", "VOUCHER_NO" }, { "V_DATE", "VOUCHER_DATE" }, { "PARTY_NAME", "SUPPLIER_EDESC" }, { "PARTY_TABLE_NAME", "IP_SUPPLIER_SETUP" },  { "SUB_CODE", "S" } } },
                    };

                        string query;
                        if (tableName == "IP_GOODS_REQUISITION")
                        {
                            query = $@"SELECT A.serial_no, B.ITEM_EDESC, A.MU_CODE, NVL(A.QUANTITY, 0), NVL(A.UNIT_PRICE, 0), 
                            NVL(A.TOTAL_PRICE, 0), A.item_code, C.LOCATION_EDESC FROM_LOC, 
                            D.LOCATION_EDESC TO_LOC, A.remarks 
                            FROM IP_GOODS_REQUISITION A
                            LEFT OUTER JOIN 
                            (SELECT ITEM_CODE CC, ITEM_EDESC FROM IP_ITEM_MASTER_SETUP WHERE COMPANY_CODE = '{companyCode}') B
                            ON A.ITEM_CODE = B.CC
                            LEFT OUTER JOIN 
                            (SELECT LOCATION_CODE CS, LOCATION_EDESC FROM IP_LOCATION_SETUP WHERE COMPANY_CODE = '{companyCode}') C
                            ON A.FROM_LOCATION_CODE = C.CS
                            LEFT OUTER JOIN 
                            (SELECT LOCATION_CODE CQ, LOCATION_EDESC FROM IP_LOCATION_SETUP WHERE COMPANY_CODE = '{companyCode}') D
                            ON A.TO_LOCATION_CODE = D.CQ
                            WHERE A.REQUISITION_NO = '{vNo}'";
                        }

                        else
                        {
                            bool partyCodeExists = distinctColumns[tableName].ContainsKey("PARTY_CODE");
                            string partyTableName = partyCodeExists ? distinctColumns[tableName]["PARTY_TABLE_NAME"] : null;
                            string partyCodeCondition = partyCodeExists
                                ? $"AND a.{distinctColumns[tableName]["PARTY_CODE"]} = b.{distinctColumns[tableName]["PARTY_CODE"]}"
                                : string.Empty;

                            query = @"
                                SELECT 
                                    " + (partyCodeExists ? "b." + distinctColumns[tableName]["PARTY_NAME"] + "," : "' ' AS PARTY_NAME,") + @"
                                    a." + distinctColumns[tableName]["V_NO"] + @",
                                    TRUNC(a." + distinctColumns[tableName]["V_DATE"] + @"),
                                    to_char(a.serial_no) serial_no,
                                    a.item_code,
                                    c.item_edesc,
                                    a.mu_code,
                                    a.QUANTITY,
                                    a.UNIT_PRICE,
                                    NVL(a.total_price, 1) * NVL(a.exchange_rate, 1) AS total_price,
                                    '" + distinctColumns[tableName]["SUB_CODE"] + "' || " + (partyCodeExists ? "a." + distinctColumns[tableName]["PARTY_CODE"] : "' '") + @" AS sub_code,
                                    a.remarks
                                FROM
                                    " + tableName + @"
                                    a
                                    " + (partyTableName != null ? ", " + partyTableName + " b" : "") + @",
                                    ip_item_master_setup c
                                WHERE
                                    a.company_code = '" + companyCode + @"'
                                    AND a.company_code = c.company_code
                                    " + (partyTableName != null ? "AND a.company_code = b.company_code" : "") + @"
                                    AND a.item_code = c.item_code
                                    " + partyCodeCondition + @"
                                    AND a." + distinctColumns[tableName]["V_NO"] + @" = '" + vNo + @"'
                                ORDER BY
                                    serial_no";
                        }
                        command.CommandText = query;
                        var allCodes = command.ExecuteReader();
                        var vals = new List<Dictionary<string, object>>();
                        string partyCode = "";
                        string partyName = "";
                        totalQty = 0;

                        string ClQtyQuery;
                        if (tableName == "IP_GOODS_REQUISITION")
                        {
                            string fromLoc = allCodes.IsDBNull(7) ? string.Empty : allCodes.GetString(7);
                            string toLoc = allCodes.IsDBNull(8) ? string.Empty : allCodes.GetString(8);
                            string rate = allCodes.IsDBNull(8) ? "0" : allCodes.GetDecimal(8).ToString("N2");
                            vals.Add(new Dictionary<string, object>
                            {
                                { "sn", allCodes.IsDBNull(3) ? 0 : allCodes.GetInt32(3) },
                                { "item", allCodes.IsDBNull(5) ? string.Empty : allCodes.GetString(5) },
                                { "unit", allCodes.IsDBNull(6) ? string.Empty : allCodes.GetString(6) },
                                { "qty", Math.Round(allCodes.IsDBNull(7) ? 0 : allCodes.GetDecimal(7), 2) },
                                { "rate", rate },
                                { "amt", allCodes.IsDBNull(9) ? 0 : allCodes.GetDecimal(9) },
                                { "code", allCodes.IsDBNull(4) ? string.Empty : allCodes.GetString(4) },
                                { "remarks", allCodes.IsDBNull(11) ? string.Empty : allCodes.GetString(11) },
                            });

                            rem = allCodes.IsDBNull(9) ? string.Empty : allCodes.GetString(9);
                        }
                        else
                        {
                            while (allCodes.Read())
                            {
                                totalAmt += allCodes.IsDBNull(9) ? 0 : allCodes.GetDecimal(9);
                                totalQty -= allCodes.IsDBNull(7) ? 0 : allCodes.GetDecimal(7);
                                string serialNoString = allCodes.IsDBNull(3) ? string.Empty : allCodes.GetValue(3).ToString();
                                //int serialNo = allCodes.IsDBNull(3) ? 0 : allCodes.GetInt32(3);
                                //string serialNoString = allCodes.IsDBNull(3) ? "0" : int.Parse(allCodes.GetString(3)).ToString();

                                string item = allCodes.IsDBNull(5) ? string.Empty : allCodes.GetString(5);
                                string unit = allCodes.IsDBNull(6) ? string.Empty : allCodes.GetString(6);
                                partyCode = allCodes.IsDBNull(10) ? string.Empty : allCodes.GetString(10);
                                partyName = allCodes.IsDBNull(0) ? string.Empty : allCodes.GetString(0);
                                decimal qty = allCodes.IsDBNull(7) ? 0 : allCodes.GetDecimal(7);

                                decimal rate;
                                if (allCodes.IsDBNull(8))
                                {
                                    rate = 0;
                                }

                                else
                                {
                                    string rateString = allCodes.GetValue(8).ToString();
                                    if (!decimal.TryParse(rateString, out rate))
                                    {
                                        rate = 0;
                                    }
                                }

                                decimal amt;
                                if (allCodes.IsDBNull(9))
                                {
                                    amt = 0;
                                }

                                else
                                {
                                    string amtString = allCodes.GetValue(9).ToString();
                                    if (!decimal.TryParse(amtString, out amt))
                                    {
                                        amt = 0;
                                    }
                                }

                                string code = allCodes.IsDBNull(4) ? string.Empty : allCodes.GetString(4);

                                string remarks = allCodes.IsDBNull(11) ? string.Empty : allCodes.GetString(11);
                                rem = allCodes.IsDBNull(11) ? string.Empty : allCodes.GetString(11);
                                totalQty += qty;


                                decimal serialNo;
                                if (!decimal.TryParse(serialNoString, out serialNo))
                                {
                                    serialNo = 0;
                                }

                                totalQty += qty;

                                ClQtyQuery = $@"
                                    SELECT SUM(NVL(IN_QUANTITY, 0) - NVL(OUT_QUANTITY, 0)) AS CLOSE_QTY  
                                    FROM V$VIRTUAL_STOCK_WIP_LEDGER1 A  
                                    WHERE A.COMPANY_CODE = '{companyCode}' 
                                      AND item_code = '{code}' 
                                      AND A.DELETED_FLAG = 'N'    
                                    GROUP BY A.ITEM_CODE
                                ";
                                var command2 = new OracleCommand(ClQtyQuery, connection);
                                var closeQty = Convert.ToDecimal(command2.ExecuteScalar() ?? 0);


                                vals.Add(new Dictionary<string, object>
                            {
                                { "sn", serialNo },
                                { "item", item },
                                { "unit", unit },
                                { "qty", qty.ToString("N4") },
                                { "rate", rate.ToString("N2") },
                                { "amt", amt.ToString("N2") },
                                { "code", code },
                                { "remarks", remarks },
                                {"close_qty", closeQty }
                            });
                            }
                        }

                        ClQtyQuery = $@"
                            SELECT SUM(NVL(IN_QUANTITY, 0) - NVL(OUT_QUANTITY, 0)) AS CLOSE_QTY  
                            FROM V$VIRTUAL_STOCK_WIP_LEDGER1 A  
                            WHERE A.COMPANY_CODE = '{companyCode}' 
                              AND item_code = '1537' 
                              AND A.DELETED_FLAG = 'N'    
                            GROUP BY A.ITEM_CODE
                        ";

                        List<Dictionary<string, object>> charge = new List<Dictionary<string, object>>(); decimal totalChargeAmt = 0;

                        query = $@"
                        SELECT c.charge_edesc, a.form_code, a.charge_code, a.charge_type_flag, 
                               a.PRIORITY_INDEX_NO, NVL(b.charge_amount, 0) charge_amount 
                        FROM charge_setup a
                        LEFT OUTER JOIN 
                        (SELECT a.charge_type_flag, a.charge_code, a.acc_code, SUM(a.charge_amount) charge_amount 
                         FROM CHARGE_TRANSACTION a 
                         WHERE a.reference_no = '{vNo}' AND a.form_code = '{formCode}' AND a.company_code = '{companyCode}' 
                         GROUP BY a.charge_type_flag, a.charge_code, a.acc_code) b 
                        ON a.charge_code = b.charge_code   
                        LEFT OUTER JOIN 
                        (SELECT * FROM IP_CHARGE_CODE WHERE company_code = '{companyCode}') c
                        ON a.charge_code = c.charge_code                                                                        
                        WHERE a.form_code = '{formCode}' AND a.company_code = '{companyCode}' 
                        AND NVL(b.charge_amount, 0) != 0 
                        ORDER BY PRIORITY_INDEX_NO";

                        command.CommandText = query;
                        allCodes = command.ExecuteReader();

                        string charge_type = "";
                        while (allCodes.Read())
                        {
                            if (allCodes[3].ToString() == "A")
                            {
                                totalAmt += Math.Round(allCodes.IsDBNull(5) ? 0 : allCodes.GetDecimal(5), 2);
                                totalChargeAmt += Math.Round(allCodes.IsDBNull(5) ? 0 : allCodes.GetDecimal(5), 2);
                            }
                            else if (allCodes[3].ToString() == "D")
                            {
                                totalAmt -= Math.Round(allCodes.IsDBNull(5) ? 0 : allCodes.GetDecimal(5), 2);
                                totalChargeAmt -= Math.Round(allCodes.IsDBNull(5) ? 0 : allCodes.GetDecimal(5), 2);
                            }
                            charge_type = allCodes.IsDBNull(3) ? string.Empty : allCodes.GetString(3);
                            string amt = allCodes.IsDBNull(5) ? "0" : allCodes.GetDecimal(5).ToString("N2");
                            charge.Add(new Dictionary<string, object>
                                {
                                    { "sn", allCodes.IsDBNull(4) ? 0 : allCodes.GetInt32(4) },
                                    { "name", allCodes.IsDBNull(0) ? string.Empty : allCodes.GetString(0) },
                                    { "amt", amt },
                                    { "charge_type", allCodes.IsDBNull(3) ? string.Empty : allCodes.GetString(3) }
                                });
                        }

                        decimal total_net_amt = charge_type == "A" ? totalAmt - totalChargeAmt : totalAmt + totalChargeAmt;
                        Dictionary<string, object> data;

                        if (tableName == "IP_GOODS_REQUISITION")
                        {

                            string rem2 = allCodes.GetString(9);
                            string fromLoc = allCodes.GetString(7);
                            string toLoc = allCodes.GetString(8);


                            data = new Dictionary<string, object>
                            {
                                { "authorised_by", authorisedBy },
                                { "authorised_date", authorisedDate },
                                { "charge", charge },
                                { "checked_by", checkedBy },
                                { "checked_date", checkedDate },
                                { "created_by", createdBy },
                                { "created_date", createdDate.ToString() },
                                { "detail", vals },
                                { "party_code", partyCode },
                                {"party", partyName },
                                { "posted_by", postedBy },
                                { "posted_date", postedDate },
                                { "remarks", rem2 },
                                { "total_qty", totalQty.ToString("N4") },
                                { "total_amt", totalAmt.ToString("N2") },
                                { "total_charge_amt", totalChargeAmt.ToString("N2") },
                                { "total_net_amt", total_net_amt.ToString("N2") },
                                { "type", type },
                                { "voucher_no", voucherNo },
                                { "voucher_date", voucherDate.ToString() },
                                { "from_loc", fromLoc },
                                { "to_loc", toLoc },
                            };
                        }
                        else
                        {


                            data = new Dictionary<string, object>
                            {
                                { "authorised_by", authorisedBy },
                                { "authorised_date", authorisedDate },
                                { "charge", charge },
                                { "checked_by", checkedBy },
                                { "checked_date", checkedDate },
                                { "created_by", createdBy },
                                { "created_date", createdDate.ToString() },
                                { "detail", vals },
                                { "party_code", partyCode },
                                {"party", partyName },
                                { "posted_by", postedBy },
                                { "posted_date", postedDate },
                                { "remarks", rem },
                                { "total_qty", totalQty.ToString("N4") },
                                { "total_amt", totalAmt.ToString("N2") },
                                { "total_charge_amt", totalChargeAmt.ToString("N2") },
                                { "total_net_amt",  total_net_amt.ToString("N2") },
                                { "type", type },
                                { "voucher_no", voucherNo },
                                { "voucher_date", voucherDate.ToString() },
                                {"form_type", formType }
                            };
                        }
                        return data;
                    }

                    if (formType == "CH" || formType == "BK")
                    {
                        string query = $@"
                        SELECT f.form_code, f.form_edesc, a.voucher_no, a.master_transaction_type, 
                               a.master_acc_code, b.acc_edesc master_acc_edesc, 
                               a.master_amount, a.serial_no, a.acc_code, c.acc_edesc acc_edesc, 
                               a.transaction_type, a.amount, a.particulars 
                        FROM fa_single_voucher a, FA_CHART_OF_ACCOUNTS_SETUP b, 
                             FA_CHART_OF_ACCOUNTS_SETUP c, form_setup f 
                        WHERE a.master_acc_code = b.acc_code 
                        AND a.company_code = b.company_code 
                        AND a.acc_code = c.acc_code 
                        AND a.company_code = c.company_code  
                        AND a.company_code = F.company_code 
                        AND a.form_code = f.form_code 
                        AND a.company_code = c.company_code   
                        AND a.voucher_no = '{vNo}'  
                        AND a.company_code = '{companyCode}' 
                        ORDER BY serial_no";

                        command.CommandText = query;

                        OracleDataReader allCodes = command.ExecuteReader();

                        var vals = new List<Dictionary<string, object>>();


                        decimal drTotal = 0;
                        decimal crTotal = 0;

                        int x = 0;
                        while (allCodes.Read())
                        {
                            var sl = new List<Dictionary<string, object>>();
                            var cd = new List<Dictionary<string, object>>();
                            x++;

                            if (x == 1)
                            {
                                rem = allCodes.IsDBNull(12) ? string.Empty : allCodes.GetString(12);
                                decimal drVal = allCodes.GetString(3) == "DR" ? allCodes.GetDecimal(6) : 0;
                                decimal crVal = allCodes.GetString(3) == "CR" ? allCodes.GetDecimal(6) : 0;

                                drTotal += drVal;
                                crTotal += crVal;


                                string v_no = allCodes.IsDBNull(2) ? string.Empty : allCodes.GetString(2);
                                string acc_code = allCodes.IsDBNull(4) ? string.Empty : allCodes.GetString(4);
                                string acc_edesc = allCodes.IsDBNull(5) ? string.Empty : allCodes.GetString(5);
                                string remarks = allCodes.IsDBNull(12) ? string.Empty : allCodes.GetString(12);
                                string serialNoString = allCodes.IsDBNull(0) ? string.Empty : allCodes.GetValue(0).ToString();

                                decimal serial_no;
                                if (!decimal.TryParse(serialNoString, out serial_no))
                                {
                                    serial_no = 0;
                                }
                                decimal drAmt = Math.Round(drVal, 3);
                                decimal crAmt = Math.Round(crVal, 3);

                                vals.Add(new Dictionary<string, object>
                                {
                                    { "v_no", v_no },
                                    { "acc_code", acc_code },
                                    { "acc_edesc", acc_edesc },
                                    { "dr_amt", Math.Round(drAmt, 2) },
                                    { "cr_amt", Math.Round(crAmt, 2) },
                                    //{ "dr_amt", drAmt.ToString("N2") },
                                    //{ "cr_amt", crAmt.ToString("N2") },
                                    { "sl",  new List<Dictionary<string, object>>() },
                                    { "cc", new List<Dictionary<string, object>>() },
                                    { "serial_no", serial_no },
                                    {"remarks",remarks },
                                });
                            }

                            string query2 = $@"
                            SELECT a.sub_code, b.sub_edesc, a.dr_amount, a.cr_amount 
                            FROM FA_VOUCHER_SUB_DETAIL a, FA_SUB_LEDGER_SETUP b
                            WHERE a.sub_code = b.sub_code 
                            AND b.company_code = '{companyCode}'
                            AND a.company_code = b.company_code 
                            AND a.voucher_no = '{allCodes.GetString(2)}' 
                            AND a.serial_no = '{allCodes.GetDecimal(7)}'";

                            command.CommandText = query2;
                            var subResult = command.ExecuteReader();

                            while (subResult.Read())
                            {
                                sl.Add(new Dictionary<string, object>
                            {
                                { "name", subResult.GetString(1) },
                                { "amt", (subResult.GetDecimal(3) - subResult.GetDecimal(2)).ToString("N2") }
                            });
                            }

                            query1 = $@"
                            SELECT b.budget_edesc, a.BUDGET_AMOUNT 
                            FROM BUDGET_TRANSACTION a, BC_BUDGET_CENTER_SETUP b 
                            WHERE a.company_code = b.company_code 
                            AND a.budget_code = b.budget_code 
                            AND a.company_code = '{companyCode}'  
                            AND a.reference_no = '{allCodes.GetString(2)}' 
                            AND a.serial_no = '{allCodes.GetDecimal(7)}' 
                            AND a.BUDGET_AMOUNT != 0";

                            command.CommandText = query1;
                            subResult = command.ExecuteReader();

                            while (subResult.Read())
                            {
                                cd.Add(new Dictionary<string, object>
                            {
                                { "name", subResult.GetString(0) },
                                { "amt", (subResult.GetDecimal(1)).ToString("N2") }
                            });
                            }

                            //variable names for the second entry
                            decimal drVal2 = allCodes.GetString(10) == "DR" ? allCodes.GetDecimal(11) : 0;
                            decimal crVal2 = allCodes.GetString(10) == "CR" ? allCodes.GetDecimal(11) : 0;

                            //From here....
                            string v_no2 = allCodes.IsDBNull(2) ? string.Empty : allCodes.GetString(2);
                            string acc_code2 = allCodes.IsDBNull(8) ? string.Empty : allCodes.GetString(8);
                            string acc_edesc2 = allCodes.IsDBNull(9) ? string.Empty : allCodes.GetString(9);
                            string remarks2 = allCodes.IsDBNull(12) ? string.Empty : allCodes.GetString(12);
                            string serialNoString2 = allCodes.IsDBNull(7) ? string.Empty : allCodes.GetValue(7).ToString();

                            decimal serial_no2;
                            if (!decimal.TryParse(serialNoString2, out serial_no2))
                            {
                                serial_no2 = 0;
                            }
                            decimal drAmt2 = Math.Round(drVal2, 3);
                            decimal crAmt2 = Math.Round(crVal2, 3);

                            vals.Add(new Dictionary<string, object>
                        {
                            { "v_no", v_no2 },
                            { "acc_code", acc_code2 },
                            { "acc_edesc", acc_edesc2 },
                            { "dr_amt",  Math.Round(drAmt2, 2) },
                            { "cr_amt",  Math.Round(crAmt2, 2) },
                            //{ "dr_amt", drAmt2.ToString("N2") },
                            //{ "cr_amt", crAmt2.ToString("N2") },
                            { "sl", sl },
                            { "cc", cd },
                            { "serial_no", serial_no2 },
                            {"remarks",remarks2 },
                        });
                            //To here....
                        }

                        var data = new Dictionary<string, object>
                    {
                        { "authorised_by", authorisedBy },
                        { "authorised_date", authorisedDate },
                        { "checked_by", checkedBy },
                        { "checked_date", checkedDate },
                        { "detail", vals },
                        { "dr_total", drTotal.ToString("N2") },
                        { "cr_total", crTotal.ToString("N2") },
                        { "voucher_no", voucherNo },
                        { "voucher_date", voucherDate },
                        { "created_by", createdBy },
                        { "created_date", createdDate },
                        { "remarks", rem },
                        { "type", formType },
                        { "posted_by", postedBy },
                        { "posted_date", postedDate },
                        {"form_type", formType }

                    };


                        return data;
                    }

                    if (formType == "JV")
                    {
                        string query = $@"
                        SELECT f.form_code, f.form_edesc, a.voucher_no, '', '', '', 
                               '', a.serial_no, a.acc_code, c.acc_edesc acc_edesc, 
                               a.transaction_type, a.amount 
                        FROM fa_double_voucher a, FA_CHART_OF_ACCOUNTS_SETUP c, form_setup f 
                        WHERE a.acc_code = c.acc_code 
                        AND a.company_code = c.company_code  
                        AND a.company_code = F.company_code 
                        AND a.form_code = f.form_code 
                        AND a.company_code = c.company_code   
                        AND a.voucher_no = '{vNo}'  
                        AND a.company_code = '{companyCode}' 
                        ORDER BY serial_no";

                        command.CommandText = query;
                        OracleDataReader allCodes = command.ExecuteReader(); var vals = new List<Dictionary<string, object>>();
                        decimal drTotal = 0;
                        decimal crTotal = 0;

                        while (allCodes.Read())
                        {
                            var sl = new List<Dictionary<string, object>>();
                            var cd = new List<Dictionary<string, object>>();
                            decimal drVal = allCodes.GetString(10) == "DR" ? allCodes.GetDecimal(11) : 0;
                            decimal crVal = allCodes.GetString(10) == "CR" ? allCodes.GetDecimal(11) : 0;

                            drTotal += drVal;
                            crTotal += crVal;

                            vals.Add(new Dictionary<string, object>
                        {
                            { "v_no", allCodes.GetString(2) },
                            { "acc_code", allCodes.GetString(8) },
                            { "acc_edesc", allCodes.GetString(9) },
                            { "dr_amt", drVal },
                            { "cr_amt", crVal },
                            { "sl", sl },
                            { "cc", cd },
                            { "serial_no", allCodes.GetDecimal(7) }
                        });

                            string query2 = $@"
                            SELECT a.sub_code, b.sub_edesc, a.dr_amount, a.cr_amount 
                            FROM FA_VOUCHER_SUB_DETAIL a, FA_SUB_LEDGER_SETUP b
                            WHERE a.sub_code = b.sub_code 
                            AND b.company_code = '{companyCode}'
                            AND a.company_code = b.company_code 
                            AND a.voucher_no = '{allCodes.GetString(2)}' 
                            AND a.serial_no = '{allCodes.GetDecimal(7)}'";

                            command.CommandText = query2;
                            var subResult = command.ExecuteReader();

                            while (subResult.Read())
                            {
                                sl.Add(new Dictionary<string, object>
                            {
                                { "name", subResult.GetString(1) },
                                { "amt", (subResult.GetDecimal(3) - subResult.GetDecimal(2)).ToString("N2") }
                            });
                            }

                            query1 = $@"
                            SELECT b.budget_edesc, a.BUDGET_AMOUNT 
                            FROM BUDGET_TRANSACTION a, BC_BUDGET_CENTER_SETUP b 
                            WHERE a.company_code = b.company_code 
                            AND a.budget_code = b.budget_code 
                            AND a.company_code = '{companyCode}'  
                            AND a.reference_no = '{allCodes.GetString(2)}' 
                            AND a.serial_no = '{allCodes.GetDecimal(7)}' 
                            AND a.BUDGET_AMOUNT != 0";


                            command.CommandText = query1;
                            subResult = command.ExecuteReader();

                            while (subResult.Read())
                            {
                                cd.Add(new Dictionary<string, object>
                            {
                                { "name", subResult.GetString(0) },
                                { "amt", (subResult.GetDecimal(1)).ToString("N2") }
                            });
                            }
                        }

                        var data = new Dictionary<string, object>
                    {
                        { "authorised_by", authorisedBy },
                        { "authorised_date", authorisedDate },
                        { "checked_by", checkedBy },
                        { "checked_date", checkedDate },
                        { "created_by", createdBy },
                        { "created_date", createdDate },
                        { "detail", vals },
                        { "dr_total", drTotal.ToString("N2") },
                        { "cr_total", crTotal.ToString("N2") },
                        { "voucher_no", voucherNo },
                        { "voucher_date", voucherDate },
                        { "type", formType },
                        { "posted_by", postedBy },
                        { "posted_date", postedDate },
                        {"form_type", formType }

                    };

                        return data;
                    }
                    return "Voucher doesn't exist";
                }
                else
                {
                    return "Voucher doesn't exist";
                }
            }
        }


        public List<TransactionResponseModel> FetchTransactions(TransactionRequestModel model, NeoErpCoreEntity dbContext)
        {

            var result = new List<TransactionResponseModel>();
            DateTime fromDate, toDate;
            if (!DateTime.TryParseExact(model.from_date, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDate) || !DateTime.TryParseExact(model.to_date, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out toDate))
                throw new Exception("Invalid Date");

            //opening balance
            string OpeningQuery = string.Empty;
            string NonOpeningQuery = string.Empty;
            model.COMPANY_CODE = model.COMPANY_CODE.Replace(" ", string.Empty);
            model.COMPANY_CODE = model.COMPANY_CODE.Replace(",", "','");
            model.BRANCH_CODE = model.BRANCH_CODE.Replace(" ", string.Empty);
            model.BRANCH_CODE = model.BRANCH_CODE.Replace(",", "','");

            if (!string.IsNullOrWhiteSpace(model.acc_code))
            {
                OpeningQuery = $@"SELECT VGL.VOUCHER_NO, TO_CHAR(VGL.VOUCHER_DATE, 'DD-MON-RRRR') VOUCHER_DATE, VGL.PARTICULARS, TO_CHAR(NVL(VGL.DR_AMOUNT, 0)) DR_AMOUNT, TO_CHAR(NVL(VGL.CR_AMOUNT, 0)) CR_AMOUNT, VGL.TRANSACTION_TYPE
                    FROM V$VIRTUAL_GENERAL_LEDGER VGL
                    WHERE 1=1
                    AND VGL.ACC_CODE = '{model.acc_code}'
                    AND VGL.VOUCHER_DATE < '{fromDate.ToString("dd-MMM-yyyy")}'
                    AND VGL.COMPANY_CODE IN ('{model.COMPANY_CODE}')
                    AND VGL.BRANCH_CODE IN ('{model.BRANCH_CODE}')
                    AND VGL.DELETED_FLAG = 'N'
                    ORDER BY VGL.VOUCHER_DATE, UPPER(VGL.VOUCHER_NO) ASC";

                //NonOpeningQuery = $@"SELECT VGL.VOUCHER_NO, TO_CHAR(VGL.VOUCHER_DATE, 'DD-MON-RRRR') VOUCHER_DATE, VGL.PARTICULARS,
                //    TO_CHAR(NVL(VGL.DR_AMOUNT, 0)) DR_AMOUNT, TO_CHAR(NVL(VGL.CR_AMOUNT, 0)) CR_AMOUNT, VGL.TRANSACTION_TYPE
                //    FROM V$VIRTUAL_GENERAL_LEDGER1 VGL
                //    WHERE 1=1
                //    AND VGL.ACC_CODE = '{model.acc_code}'
                //    AND VGL.VOUCHER_DATE BETWEEN '{fromDate.ToString("dd-MMM-yyyy")}' AND '{toDate.ToString("dd-MMM-yyyy")}'
                //    AND VGL.COMPANY_CODE = '{model.COMPANY_CODE}'
                //    ORDER BY VGL.VOUCHER_DATE, UPPER(VGL.VOUCHER_NO) ASC";
                NonOpeningQuery = $@"SELECT VGL.VOUCHER_NO, TO_CHAR(VGL.VOUCHER_DATE, 'DD-MON-RRRR') VOUCHER_DATE, VGL.PARTICULARS, TO_CHAR(NVL(VGL.DR_AMOUNT, 0)) DR_AMOUNT, TO_CHAR(NVL(VGL.CR_AMOUNT, 0)) CR_AMOUNT, VGL.TRANSACTION_TYPE
                    FROM V$VIRTUAL_GENERAL_LEDGER VGL
                    WHERE 1=1
                    AND VGL.ACC_CODE = '{model.acc_code}'
                    AND TRUNC(VGL.VOUCHER_DATE) BETWEEN TO_DATE('{fromDate.ToString("dd-MMM-yyyy")}','DD-MON-RRRR') AND TO_DATE('{toDate.ToString("dd-MMM-yyyy")}','DD-MON-RRRR')
                    AND VGL.COMPANY_CODE IN ('{model.COMPANY_CODE}')
                    AND VGL.BRANCH_CODE IN ('{model.BRANCH_CODE}')
                    AND VGL.DELETED_FLAG = 'N'
                    ORDER BY VGL.VOUCHER_DATE, UPPER(VGL.VOUCHER_NO) ASC";
            }
            else
            {
                //OpeningQuery = $@"SELECT VSL.VOUCHER_NO, TO_CHAR(VSL.VOUCHER_DATE, 'DD-MON-RRRR') VOUCHER_DATE, VSL.PARTICULARS,
                //    TO_CHAR(NVL(VSL.DR_AMOUNT, 0)) DR_AMOUNT, TO_CHAR(NVL(VSL.CR_AMOUNT, 0)) CR_AMOUNT, VSL.TRANSACTION_TYPE
                //    FROM V$VIRTUAL_SUB_LEDGER VSL
                //    WHERE 1=1
                //    AND VSL.SUB_CODE = '{model.sub_code}'
                //    AND VSL.VOUCHER_DATE < '{fromDate.ToString("dd-MMM-yyyy")}'
                //    AND VSL.COMPANY_CODE = '{model.COMPANY_CODE}'
                //    ORDER BY VSL.VOUCHER_DATE, UPPER(VSL.VOUCHER_NO) ASC";
                OpeningQuery = $@"SELECT VSL.VOUCHER_NO, TO_CHAR(VSL.VOUCHER_DATE, 'DD-MON-RRRR') VOUCHER_DATE, VSL.PARTICULARS, TO_CHAR(NVL(VSL.DR_AMOUNT, 0)) DR_AMOUNT, TO_CHAR(NVL(VSL.CR_AMOUNT, 0)) CR_AMOUNT, VSL.TRANSACTION_TYPE
                    FROM V$VIRTUAL_SUB_LEDGER VSL
                    WHERE 1 = 1
                    AND VSL.SUB_CODE = '{model.sub_code}'
                    AND VSL.VOUCHER_DATE < '{fromDate.ToString("dd-MMM-yyyy")}'
                    AND VSL.COMPANY_CODE IN ('{model.COMPANY_CODE}')
                    AND VSL.BRANCH_CODE IN ('{model.BRANCH_CODE}')
                    AND VSL.DELETED_FLAG = 'N'
                    ORDER BY VSL.VOUCHER_DATE, UPPER(VSL.VOUCHER_NO) ASC";

                //NonOpeningQuery = $@"SELECT VSL.VOUCHER_NO, TO_CHAR(VSL.VOUCHER_DATE, 'DD-MON-RRRR') VOUCHER_DATE, VSL.PARTICULARS,
                //    TO_CHAR(NVL(VSL.DR_AMOUNT, 0)) DR_AMOUNT, TO_CHAR(NVL(VSL.CR_AMOUNT, 0)) CR_AMOUNT, VSL.TRANSACTION_TYPE
                //    FROM V$VIRTUAL_SUB_LEDGER VSL
                //    WHERE 1=1
                //    AND VSL.SUB_CODE = '{model.sub_code}'
                //    AND VSL.VOUCHER_DATE BETWEEN '{fromDate.ToString("dd-MMM-yyyy")}' AND '{toDate.ToString("dd-MMM-yyyy")}'
                //    AND VSL.COMPANY_CODE = '{model.COMPANY_CODE}'
                //    ORDER BY VSL.VOUCHER_DATE, UPPER(VSL.VOUCHER_NO) ASC";

                NonOpeningQuery = $@"SELECT VSL.VOUCHER_NO, TO_CHAR(VSL.VOUCHER_DATE, 'DD-MON-RRRR') VOUCHER_DATE, VSL.PARTICULARS, TO_CHAR(NVL(VSL.DR_AMOUNT, 0)) DR_AMOUNT, TO_CHAR(NVL(VSL.CR_AMOUNT, 0)) CR_AMOUNT, VSL.TRANSACTION_TYPE
                    FROM V$VIRTUAL_SUB_LEDGER VSL
                    WHERE 1=1
                    AND VSL.SUB_CODE = '{model.sub_code}'
                    AND TRUNC(VSL.VOUCHER_DATE) BETWEEN TO_DATE('{fromDate.ToString("dd-MMM-yyyy")}','DD-MON-RRRR') AND TO_DATE('{toDate.ToString("dd-MMM-yyyy")}','DD-MON-RRRR')
                    AND VSL.COMPANY_CODE IN ('{model.COMPANY_CODE}')
                    AND VSL.BRANCH_CODE IN ('{model.BRANCH_CODE}')
                    AND VSL.DELETED_FLAG = 'N'
                    ORDER BY VSL.VOUCHER_DATE, UPPER(VSL.VOUCHER_NO) ASC";
            }
            var openingData = dbContext.SqlQuery<TransactionResponseModel>(OpeningQuery).ToList();
            decimal OpeningBalance = 0;
            foreach (var item in openingData)
            {
                decimal amount;
                if (item.TRANSACTION_TYPE == "DR")
                {
                    decimal.TryParse(item.DR_AMOUNT, out amount);
                    OpeningBalance += amount;
                }
                else
                {
                    decimal.TryParse(item.CR_AMOUNT, out amount);
                    OpeningBalance -= amount;
                }
            }
            var OpeningObject = new TransactionResponseModel
            {
                VOUCHER_NO = "0",
                PARTICULARS = "Opening Balance",
                VOUCHER_DATE = fromDate.ToString("dd-MMM-yyyy")
            };
            if (OpeningBalance >= 0)
            {
                OpeningObject.DR_AMOUNT = OpeningBalance.ToString();
                OpeningObject.CR_AMOUNT = "0";
                OpeningObject.TRANSACTION_TYPE = "DR";
            }
            else
            {
                OpeningObject.CR_AMOUNT = (-1 * OpeningBalance).ToString();
                OpeningObject.DR_AMOUNT = "0";
                OpeningObject.TRANSACTION_TYPE = "CR";
            }

            //non opening balance
            result = dbContext.SqlQuery<TransactionResponseModel>(NonOpeningQuery).ToList();
            var itemsToRemove = new List<TransactionResponseModel>();
            OpeningBalance = 0;
            bool hasOpening = false;
            foreach (var item in result)
            {
                if (item.VOUCHER_NO == "0")
                {
                    decimal amount;
                    if (item.TRANSACTION_TYPE == "DR")
                    {
                        decimal.TryParse(item.DR_AMOUNT, out amount);
                        OpeningBalance += amount;
                    }
                    else
                    {
                        decimal.TryParse(item.CR_AMOUNT, out amount);
                        OpeningBalance -= amount;
                    }
                    hasOpening = true;
                    itemsToRemove.Add(item);
                }
            }
            foreach (var item in itemsToRemove)
                result.Remove(item);
            var OpeningObject2 = new TransactionResponseModel
            {
                VOUCHER_NO = "0",
                PARTICULARS = "Opening Balance",
                VOUCHER_DATE = fromDate.ToString("dd-MMM-yyyy")
            };
            if (OpeningBalance >= 0)
            {
                OpeningObject2.DR_AMOUNT = OpeningBalance.ToString();
                OpeningObject2.CR_AMOUNT = "0";
                OpeningObject2.TRANSACTION_TYPE = "DR";
            }
            else
            {
                OpeningObject2.CR_AMOUNT = (-1 * OpeningBalance).ToString();
                OpeningObject2.DR_AMOUNT = "0";
                OpeningObject2.TRANSACTION_TYPE = "CR";
            }
            if (openingData.Count != 0)
                result.Insert(0, OpeningObject);
            if (hasOpening)
                result.Insert(0, OpeningObject2);
            if (result.Count <= 0)
                throw new Exception("No records found");
            return result;
        }
        public CustomerModels fetchCustomerDetails(TransactionRequestModel model, NeoErpCoreEntity dbContext)
        {
            string company_code = model.COMPANY_CODE;
            string formatedCompanyCodes = company_code.Replace("[", "'").Replace("]", "'").Replace(", ", "','");
            string query = $@"select NVL(a.TEL_MOBILE_NO1,NVL(a.TEL_MOBILE_NO2,a.TEL_MOBILE_NO3)) as CONTACT,a.customer_edesc,a.regd_office_eaddress,c.branch_edesc as branch_name,b.company_edesc as company_name, b.address from sa_customer_setup a
           left join company_setup b  on (a.company_code=b.company_code)
           left join fa_branch_setup c on (a.branch_code=c.branch_code and b.company_code=c.company_code)
           where trim(a.link_sub_code)='{model.sub_code}' and a.company_code in ({formatedCompanyCodes})";
            var result = dbContext.SqlQuery<CustomerModels>(query).FirstOrDefault();
            return result;
        }
        public List<NotificationDataModel> fetchNotificationData(ProfileDetails model, NeoErpCoreEntity dbContext)
        {
            try
            {
                var data = new List<NotificationDataModel>();

                string query1 = $@"SELECT NOTIFICATION_TITLE, NOTIFICATION_TEXT,NOTIFICATION_TYPE, STATUS  FROM DIST_NOTIFICATIONS WHERE SP_CODE = '{model.SP_CODE}' AND COMPANY_CODE in ('{model.COMPANY_CODE}') AND DELETED_FLAG='N' ORDER BY NOTIFICATION_ID DESC";
                data = dbContext.SqlQuery<NotificationDataModel>(query1).ToList();
                if (data == null || data.Count == 0)
                {
                    return new List<NotificationDataModel>
               {
                   new NotificationDataModel
                   {
                       NOTIFICATION_TITLE = null,
                       NOTIFICATION_TEXT = null,
                       NOTIFICATION_TYPE = null,
                       STATUS = null
                   }
               };
                }
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception("Data not found!!!", ex);
            }
        }
        public List<POStatusModel> fetchPoStatus(PurchaseOrderStatus model, NeoErpCoreEntity dbContext)
        {
            try
            {
                var data = new List<POStatusModel>();

                var dateFilter = "";
                var distributorQuery = "";

                //}
                if (model.FROM_DATE != null && model.TO_DATE != null)
                {
                    dateFilter = $@" trunc(order_date) between trunc(to_date('{model.FROM_DATE?.ToString("yyyy-MM-dd")}','yyyy-MM-dd')) and trunc(to_date('{model.TO_DATE?.ToString("yyyy-MM-dd")}','yyyy-MM-dd')) AND ";
                    //dateFilter = $@" AND trunc(order_date) between trunc(to_date('{model.FROM_DATE?.ToString("yyyy-MM-dd")}','yyyy-MM-dd')) and trunc(to_date('{model.TO_DATE?.ToString("yyyy-MM-dd")}','yyyy-MM-dd')) ";
                }
                //if (!string.IsNullOrEmpty(model.FORM_CODE))
                //{
                //    formFilter = $@" and form_code = '{model.FORM_CODE}'";
                //}

                //if(model.ENTITY_CODE.ToUpper() == "D" || string.IsNullOrEmpty(model.ENTITY_CODE))
                //{
                //distributorQuery = $@"
                //                    SELECT DISTINCT
                //                     po.order_no,
                //                     po.sales_order_no,
                //                     rd.voucher_no AS chalan_no,
                //                     (select distinct voucher_no from reference_detail where reference_no = rd.voucher_no and rownum = 1) as BILL_NO,
                //                     po.created_date AS order_date,
                //                              (select checked_date from master_transaction where voucher_no = po.sales_order_no) as so_approve_date,
                //                              (SELECT created_date
                //                                 FROM reference_detail
                //                                WHERE reference_no = po.sales_order_no AND ROWNUM = 1)
                //                                 AS bill_date,
                //                              (select CREATED_DATE from chalan_delivery_status_update where voucher_no = RD.VOUCHER_NO AND ROWNUM = 1) as delivered_date,
                //                              (SELECT created_date
                //                        FROM order_dispatch_schedule a
                //                       WHERE a.order_no = po.sales_order_no and rownum = 1)
                //                        AS allocation_date,
                //                        CASE
                //                      WHEN EXISTS
                //                              (SELECT 1
                //                                 FROM order_dispatch_schedule ods
                //                                WHERE     ods.order_no = po.sales_order_no
                //                                      AND ods.company_code = po.company_code
                //                                      AND ods.item_code = po.item_code)
                //                      THEN
                //                         'Y'
                //                      ELSE
                //                         'N'
                //                   END
                //                      AS is_order_allocated,
                //                     cs.customer_edesc AS party_name,    
                //                     --SO Status Logic
                //                     CASE
                //                        WHEN fs.form_action_flag = '001' AND mt.posted_by IS NOT NULL
                //                        THEN
                //                           'Approved'
                //                        WHEN fs.form_action_flag = '010' AND mt.authorised_by IS NOT NULL
                //                        THEN
                //                           'Approved'
                //                        WHEN fs.form_action_flag = '011'
                //                             AND mt.posted_by IS NOT NULL
                //                             AND mt.authorised_by IS NOT NULL
                //                        THEN
                //                           'Approved'
                //                        WHEN fs.form_action_flag = '100' AND mt.checked_by IS NOT NULL
                //                        THEN
                //                           'Approved'
                //                        WHEN fs.form_action_flag = '101'
                //                             AND mt.checked_by IS NOT NULL
                //                             AND mt.posted_by IS NOT NULL
                //                        THEN
                //                           'Approved'
                //                        WHEN fs.form_action_flag = '110'
                //                             AND mt.authorised_by IS NOT NULL
                //                             AND mt.checked_by IS NOT NULL
                //                        THEN
                //                           'Approved'
                //                        WHEN fs.form_action_flag = '111'
                //                             AND mt.checked_by IS NOT NULL
                //                             AND mt.authorised_by IS NOT NULL
                //                             AND mt.posted_by IS NOT NULL
                //                        THEN
                //                           'Approved'
                //                        ELSE
                //                           'Pending'
                //                     END
                //                        AS so_status,
                //                     CASE
                //                       WHEN EXISTS (
                //                             SELECT 1
                //                               FROM chalan_delivery_status_update
                //                                WHERE voucher_no = RD.VOUCHER_NO
                //                                        )
                //                                    THEN 'Delivered'
                //                        WHEN EXISTS
                //                                (SELECT 1
                //                                   FROM reference_detail rd2
                //                                  WHERE rd2.reference_no = po.sales_order_no)
                //                        THEN
                //                           'Dispatch'
                //                        WHEN EXISTS
                //                                (SELECT 1
                //                                   FROM order_dispatch_schedule ods
                //                                  WHERE     ods.order_no = po.sales_order_no
                //                                        AND ods.company_code = po.company_code
                //                                        AND ods.item_code = po.item_code)
                //                        THEN
                //                           'Order Allocated'
                //                        WHEN po.sales_order_no IS NOT NULL
                //                        THEN
                //                           'SO Created'
                //                        ELSE
                //                           'Pending'
                //                     END
                //                        AS status,
                //                     (SELECT CASE
                //                                WHEN COUNT(*) =
                //                                        SUM(
                //                                           CASE
                //                                              WHEN(a.order_qty - a.quantity) = 0 THEN 1
                //                                              ELSE 0
                //                                           END)
                //                                THEN
                //                                   'Y'
                //                                ELSE
                //                                   'N'
                //                             END
                //                        FROM order_dispatch_schedule a
                //                       WHERE a.order_no = po.sales_order_no)
                //                        AS is_full_reference
                //                FROM dist_ip_ssd_purchase_order po
                //                     LEFT JOIN sa_customer_setup cs
                //                        ON     po.customer_code = cs.customer_code
                //                           AND po.company_code = cs.company_code
                //                     LEFT JOIN reference_detail rd
                //                        ON po.sales_order_no = rd.reference_no
                //                     LEFT JOIN master_transaction mt
                //                        ON po.sales_order_no = mt.voucher_no
                //                     LEFT JOIN form_setup fs
                //                        ON mt.form_code = fs.form_code
                //                     LEFT JOIN order_dispatch_schedule ods
                //                        ON po.sales_order_no = ods.order_no
                //            where {dateFilter}
                //            po.customer_code in (select customer_code from dist_user_areas where SP_CODE = '{model.SP_CODE}')
                //            ORDER BY 1";

                //distributorQuery = $@"SELECT * FROM VW_PO_STATUS 
                //                        WHERE customer_code IN (SELECT customer_code 
                //                                                  FROM dist_user_areas 
                //                                                 WHERE user_id = (SELECT userid 
                //                                                                    FROM dist_login_user 
                //                                                                   WHERE sp_code = '{model.SP_CODE}')) {dateFilter} order by order_date desc";

                //distributorQuery = $@"
                //    select a.*
                //     ,CASE
                //                 WHEN EXISTS
                //                         (SELECT 1
                //                            FROM order_dispatch_schedule ods
                //                           WHERE     ods.order_no = sales_order_no
                //                                 AND ods.company_code = company_code
                //                                 AND ods.item_code = item_code)
                //                 THEN
                //                    'Y'
                //                 ELSE
                //                    'N'
                //              END
                //                 AS is_order_allocated,
                //              CASE
                //                 WHEN form_action_flag = '001' AND posted_by IS NOT NULL
                //                 THEN
                //                    'Approved'
                //                 WHEN     form_action_flag = '010'
                //                      AND authorised_by IS NOT NULL
                //                 THEN
                //                    'Approved'
                //                 WHEN     form_action_flag = '011'
                //                      AND posted_by IS NOT NULL
                //                      AND authorised_by IS NOT NULL
                //                 THEN
                //                    'Approved'
                //                 WHEN form_action_flag = '100' AND checked_by IS NOT NULL
                //                 THEN
                //                    'Approved'
                //                 WHEN     form_action_flag = '101'
                //                      AND checked_by IS NOT NULL
                //                      AND posted_by IS NOT NULL
                //                 THEN
                //                    'Approved'
                //                 WHEN     form_action_flag = '110'
                //                      AND authorised_by IS NOT NULL
                //                      AND checked_by IS NOT NULL
                //                 THEN
                //                    'Approved'
                //                 WHEN     form_action_flag = '111'
                //                      AND checked_by IS NOT NULL
                //                      AND authorised_by IS NOT NULL
                //                      AND posted_by IS NOT NULL
                //                 THEN
                //                    'Approved'
                //                 ELSE
                //                    'Pending'
                //              END
                //                 AS so_status,
                //              CASE
                //                 WHEN EXISTS
                //                         (SELECT 1
                //                            FROM chalan_delivery_status_update
                //                           WHERE voucher_no = chalan_no)
                //                 THEN
                //                    'Delivered'
                //                 WHEN EXISTS
                //                         (SELECT 1
                //                            FROM reference_detail rd2
                //                           WHERE rd2.reference_no = chalan_no)
                //                 THEN
                //                    'Billed'
                //                 WHEN EXISTS
                //                         (SELECT 1
                //                            FROM reference_detail rd2
                //                           WHERE rd2.reference_no = sales_order_no)
                //                 THEN
                //                    'Dispatch'
                //                 WHEN EXISTS
                //                         (SELECT 1
                //                            FROM order_dispatch_schedule ods
                //                           WHERE     ods.order_no = sales_order_no
                //                                 AND ods.company_code = company_code
                //                                 AND ods.item_code = item_code)
                //                 THEN
                //                    'Order Allocated'
                //                 WHEN sales_order_no IS NOT NULL
                //                 THEN
                //                    'SO Created'
                //                 ELSE
                //                    'Pending'
                //              END
                //                 AS status,
                //              (SELECT CASE
                //                         WHEN COUNT (*) =
                //                                 SUM (
                //                                    CASE
                //                                       WHEN (aa.order_qty - aa.quantity) = 0 THEN 1
                //                                       ELSE 0
                //                                    END)
                //                         THEN
                //                            'Y'
                //                         ELSE
                //                            'N'
                //                      END
                //                 FROM order_dispatch_schedule aa
                //                WHERE aa.order_no = sales_order_no)
                //                 AS is_full_reference
                //    from (
                //    SELECT 
                //              po.order_no,
                //              po.item_code,
                //              po.company_code,
                //              po.customer_code,
                //              po.sales_order_no,
                //              rd.voucher_no AS chalan_no,
                //              rd2.voucher_no As bill_no,
                //              po.created_date As order_date,
                //              mt.checked_date AS so_approve_date,
                //              rd.created_date As bill_date,
                //              cdsu.created_date As delivered_date,
                //              cs.customer_edesc AS party_name,
                //              fs.form_action_flag, mt.posted_by, mt.checked_by, mt.authorised_by
                //         FROM dist_ip_ssd_purchase_order po,
                //         sa_customer_setup cs,
                //         reference_detail rd,
                //         reference_detail rd2,
                //         master_transaction mt,
                //         chalan_delivery_status_update cdsu,
                //         form_setup fs,
                //         order_dispatch_schedule ods         
                //         where     po.customer_code = cs.customer_code(+)
                //              and po.company_code = cs.company_code(+)
                //              and po.sales_order_no = rd.reference_no(+)
                //              and po.sales_order_no = mt.voucher_no(+)
                //              and mt.form_code = fs.form_code(+)
                //              and rd.voucher_no = cdsu.voucher_no(+)
                //              and po.sales_order_no = rd2.reference_no(+)
                //              and po.sales_order_no = ods.order_no(+) 
                //              group by po.order_no, po.customer_code, po.company_code,fs.form_action_flag,cs.customer_edesc, po.item_code,po.sales_order_no,rd.voucher_no,rd2.voucher_no,po.created_date,mt.checked_date,rd.created_date,cdsu.created_date,
                //              mt.posted_by, mt.checked_by, mt.authorised_by) a
                //            where {dateFilter}
                //                customer_code in (select customer_code from dist_user_areas where SP_CODE = '{model.SP_CODE}')
                //                ORDER BY 1
                //";

                distributorQuery = $@"
                    WITH ref_first AS (
                        SELECT distinct reference_no,
                               voucher_no,
                               created_date,
                               ROW_NUMBER() OVER (PARTITION BY reference_no ORDER BY created_date) rn
                          FROM reference_detail
                    ),
                    delivered AS (
                        SELECT voucher_no,
                               created_date AS delivered_date,
                               ROW_NUMBER() OVER (PARTITION BY voucher_no ORDER BY created_date) rn
                          FROM chalan_delivery_status_update
                    ),
                    allocation AS (
                        SELECT order_no,
                               created_date AS allocation_date,
                               ROW_NUMBER() OVER (PARTITION BY order_no ORDER BY created_date) rn
                          FROM order_dispatch_schedule
                    ),
                    full_reference AS (
                        SELECT order_no,
                               CASE
                                 WHEN COUNT(*) = SUM(CASE WHEN (order_qty - quantity) = 0 THEN 1 ELSE 0 END) THEN 'Y'
                                 ELSE 'N'
                               END AS is_full_reference
                          FROM order_dispatch_schedule
                         GROUP BY order_no
                    )
                    SELECT DISTINCT
                           po.order_no,
                           po.sales_order_no,
                           rd.voucher_no AS chalan_no,
                           rf_chalan.voucher_no AS bill_no,
                           po.created_date AS order_date,
                           mt.checked_date AS so_approve_date,
                           rf_chalan.created_date AS bill_date,
                           d.delivered_date,
                           al.allocation_date,
                           CASE WHEN ods.order_no IS NOT NULL THEN 'Y' ELSE 'N' END AS is_order_allocated,
                           cs.customer_edesc AS party_name,
                           -- SO Status Logic (unchanged)
                           CASE
                             WHEN fs.form_action_flag = '001' AND mt.posted_by IS NOT NULL THEN 'Approved'
                             WHEN fs.form_action_flag = '010' AND mt.authorised_by IS NOT NULL THEN 'Approved'
                             WHEN fs.form_action_flag = '011' AND mt.posted_by IS NOT NULL AND mt.authorised_by IS NOT NULL THEN 'Approved'
                             WHEN fs.form_action_flag = '100' AND mt.checked_by IS NOT NULL THEN 'Approved'
                             WHEN fs.form_action_flag = '101' AND mt.checked_by IS NOT NULL AND mt.posted_by IS NOT NULL THEN 'Approved'
                             WHEN fs.form_action_flag = '110' AND mt.authorised_by IS NOT NULL AND mt.checked_by IS NOT NULL THEN 'Approved'
                             WHEN fs.form_action_flag = '111' AND mt.checked_by IS NOT NULL AND mt.authorised_by IS NOT NULL AND mt.posted_by IS NOT NULL THEN 'Approved'
                             ELSE 'Pending'
                           END AS so_status,
                           CASE
                             WHEN d.voucher_no IS NOT NULL THEN 'Delivered'
                             WHEN rd.voucher_no = rf_chalan.reference_no THen 'Billed'
                             WHEN rf_sales.voucher_no IS NOT NULL THEN 'Dispatch'
                             WHEN ods.order_no IS NOT NULL THEN 'Order Allocated'
                             WHEN po.sales_order_no IS NOT NULL THEN 'SO Created'
                             WHEN po.reject_flag = 'Y' THEN 'Rejected'
                             ELSE 'Pending'
                           END AS status,
                           fr.is_full_reference
                    FROM dist_ip_ssd_purchase_order po
                         LEFT JOIN sa_customer_setup cs
                           ON po.customer_code = cs.customer_code
                          AND po.company_code = cs.company_code
                         LEFT JOIN reference_detail rd
                           ON po.sales_order_no = rd.reference_no
                            and po.item_code = rd.REFERENCE_ITEM_CODE
                         LEFT JOIN master_transaction mt
                           ON po.sales_order_no = mt.voucher_no
                         LEFT JOIN form_setup fs
                           ON mt.form_code = fs.form_code
                         LEFT JOIN order_dispatch_schedule ods
                           ON po.sales_order_no = ods.order_no
                          AND po.company_code = ods.company_code
                          AND po.item_code = ods.item_code
                         LEFT JOIN ref_first rf_sales
                           ON rf_sales.reference_no = po.sales_order_no
                          AND rf_sales.rn = 1
                         LEFT JOIN ref_first rf_chalan
                           ON rf_chalan.reference_no = rd.voucher_no
                          AND rf_chalan.rn = 1
                         LEFT JOIN delivered d
                           ON d.voucher_no = rd.voucher_no
                          AND d.rn = 1
                         LEFT JOIN allocation al
                           ON al.order_no = po.sales_order_no
                          AND al.rn = 1
                         LEFT JOIN full_reference fr
                           ON fr.order_no = po.sales_order_no
                        where {dateFilter}
                                po.customer_code in (select customer_code from dist_user_areas where SP_CODE = '{model.SP_CODE}')
                                ORDER BY 1 desc
                        ";

                data = dbContext.SqlQuery<POStatusModel>(distributorQuery).ToList();



                return data;
            }
            catch (Exception ex)
            {
                throw new Exception("Data not found!!!", ex);
            }
        }
        public List<POStatusModel> fetchPoStatusDate(PurchaseOrderStatus model, NeoErpCoreEntity dbContext)
        {
            try
            {
                var data = new List<POStatusModel>();

                var distributorQuery = $@"
                                    SELECT DISTINCT
                                     po.order_no,
                                     po.sales_order_no,
                                     rd.voucher_no AS chalan_no,
                                     po.created_date AS order_date,
                                              (select checked_date from master_transaction where voucher_no = po.sales_order_no) as so_approve_date,
                                              (SELECT created_date
                                                 FROM reference_detail
                                                WHERE reference_no = po.sales_order_no AND ROWNUM = 1)
                                                 AS bill_date,
                                              (select CREATED_DATE from chalan_delivery_status_update where voucher_no = RD.VOUCHER_NO AND ROWNUM = 1) as delivered_date,
                                              (SELECT created_date
                                        FROM order_dispatch_schedule a
                                       WHERE a.order_no = po.sales_order_no and rownum = 1)
                                        AS allocation_date,
                                        CASE
                                      WHEN EXISTS
                                              (SELECT 1
                                                 FROM order_dispatch_schedule ods
                                                WHERE     ods.order_no = po.sales_order_no
                                                      AND ods.company_code = po.company_code
                                                      AND ods.item_code = po.item_code)
                                      THEN
                                         'Y'
                                      ELSE
                                         'N'
                                   END
                                      AS is_order_allocated,
                                     cs.customer_edesc AS party_name,    
                                     --SO Status Logic
                                     CASE
                                        WHEN fs.form_action_flag = '001' AND mt.posted_by IS NOT NULL
                                        THEN
                                           'Approved'
                                        WHEN fs.form_action_flag = '010' AND mt.authorised_by IS NOT NULL
                                        THEN
                                           'Approved'
                                        WHEN fs.form_action_flag = '011'
                                             AND mt.posted_by IS NOT NULL
                                             AND mt.authorised_by IS NOT NULL
                                        THEN
                                           'Approved'
                                        WHEN fs.form_action_flag = '100' AND mt.checked_by IS NOT NULL
                                        THEN
                                           'Approved'
                                        WHEN fs.form_action_flag = '101'
                                             AND mt.checked_by IS NOT NULL
                                             AND mt.posted_by IS NOT NULL
                                        THEN
                                           'Approved'
                                        WHEN fs.form_action_flag = '110'
                                             AND mt.authorised_by IS NOT NULL
                                             AND mt.checked_by IS NOT NULL
                                        THEN
                                           'Approved'
                                        WHEN fs.form_action_flag = '111'
                                             AND mt.checked_by IS NOT NULL
                                             AND mt.authorised_by IS NOT NULL
                                             AND mt.posted_by IS NOT NULL
                                        THEN
                                           'Approved'
                                        ELSE
                                           'Pending'
                                     END
                                        AS so_status,
                                     CASE
                                         WHEN EXISTS (
                                             SELECT 1
                                               FROM chalan_delivery_status_update
                                                WHERE voucher_no = RD.VOUCHER_NO
                                                        )
                                                    THEN 'Delivered'
                                        WHEN EXISTS
                                                (SELECT 1
                                                   FROM reference_detail rd2
                                                  WHERE rd2.reference_no = po.sales_order_no)
                                        THEN
                                           'Dispatch'
                                        WHEN EXISTS
                                                (SELECT 1
                                                   FROM order_dispatch_schedule ods
                                                  WHERE     ods.order_no = po.sales_order_no
                                                        AND ods.company_code = po.company_code
                                                        AND ods.item_code = po.item_code)
                                        THEN
                                           'Order Allocated'
                                        WHEN po.sales_order_no IS NOT NULL
                                        THEN
                                           'SO Created'
                                        ELSE
                                           'Pending'
                                     END
                                        AS status,
                                     (SELECT CASE
                                                WHEN COUNT(*) =
                                                        SUM(
                                                           CASE
                                                              WHEN(a.order_qty - a.quantity) = 0 THEN 1
                                                              ELSE 0
                                                           END)
                                                THEN
                                                   'Y'
                                                ELSE
                                                   'N'
                                             END
                                        FROM order_dispatch_schedule a
                                       WHERE a.order_no = po.sales_order_no)
                                        AS is_full_reference
                                FROM dist_ip_ssd_purchase_order po
                                     LEFT JOIN sa_customer_setup cs
                                        ON     po.customer_code = cs.customer_code
                                           AND po.company_code = cs.company_code
                                     LEFT JOIN reference_detail rd
                                        ON po.sales_order_no = rd.reference_no
                                     LEFT JOIN master_transaction mt
                                        ON po.sales_order_no = mt.voucher_no
                                     LEFT JOIN form_setup fs
                                        ON mt.form_code = fs.form_code
                                     LEFT JOIN order_dispatch_schedule ods
                                        ON po.sales_order_no = ods.order_no
                            where po.ORDER_NO = '{model.ORDER_NO}'
                            ORDER BY 1";

                data = dbContext.SqlQuery<POStatusModel>(distributorQuery).ToList();

                return data;
            }
            catch (Exception ex)
            {
                throw new Exception("Data not found!!!", ex);
            }
        }
        public dynamic GetTimeline(PurchaseOrderStatus model, NeoErpCoreEntity dbContext)
        {
            var data = new PoTimelineModel();
            string salesOrderNo = "";

            if (!string.IsNullOrEmpty(model.SALES_ORDER_NO))
            {
                salesOrderNo = $@" and SSD.SALES_ORDER_NO = '{model.SALES_ORDER_NO}'";
            }

            string query = $@"
                            SELECT distinct ssd.order_no,
                                   CASE WHEN ssd.sales_order_no IS NOT NULL THEN 'Y' ELSE 'N' END
                                      AS is_so_created,
                                   (select
                                   distinct
                                   CASE
                                      WHEN fs.form_action_flag = '001' AND mt.posted_by IS NOT NULL
                                      THEN
                                         'Approved'
                                      WHEN fs.form_action_flag = '010' AND mt.authorised_by IS NOT NULL
                                      THEN
                                         'Approved'
                                      WHEN fs.form_action_flag = '011'
                                           AND mt.posted_by IS NOT NULL
                                           AND mt.authorised_by IS NOT NULL
                                      THEN
                                         'Approved'
                                      WHEN fs.form_action_flag = '100' AND mt.checked_by IS NOT NULL
                                      THEN
                                         'Approved'
                                      WHEN fs.form_action_flag = '101'
                                           AND mt.checked_by IS NOT NULL
                                           AND mt.posted_by IS NOT NULL
                                      THEN
                                         'Approved'
                                      WHEN fs.form_action_flag = '110'
                                           AND mt.authorised_by IS NOT NULL
                                           AND mt.checked_by IS NOT NULL
                                      THEN
                                         'Approved'
                                      WHEN fs.form_action_flag = '111'
                                           AND mt.checked_by IS NOT NULL
                                           AND mt.authorised_by IS NOT NULL
                                           AND mt.posted_by IS NOT NULL
                                      THEN
                                         'Approved'
                                      ELSE
                                         'Pending'
                                   END
                                      AS so_status
                                      from master_transaction mt join form_setup fs on mt.form_code = fs.form_code
                                      where mt.voucher_no = ssd.sales_order_no) as so_status,
                                   CASE
                                      WHEN EXISTS
                                              (SELECT 1
                                                 FROM order_dispatch_schedule ods
                                                WHERE     ods.order_no = ssd.sales_order_no
                                                      AND ods.company_code = ssd.company_code
                                                      AND ods.item_code = ssd.item_code)
                                      THEN
                                         'Y'
                                      ELSE
                                         'N'
                                   END
                                      AS is_order_allocated,
                                   CASE
                                      WHEN EXISTS
                                              (SELECT 1
                                                 FROM reference_detail rd
                                                WHERE rd.reference_no = ssd.sales_order_no)
                                      THEN
                                         'Y'
                                      ELSE
                                         'N'
                                   END
                                    AS is_billed,
                                    (SELECT CASE
                                                 WHEN EXISTS(
                                                   SELECT 1
                                                   FROM chalan_delivery_status_update
                                                   WHERE voucher_no = '{model.CHALAN_NO}'
                                                 )
                                                 THEN 'Y'
                                                 ELSE 'N'
                                               END AS result
                                        FROM dual)  as delivered,
                                       (SELECT CASE
                                                WHEN COUNT(*) =
                                                        SUM(
                                                           CASE
                                                              WHEN(a.order_qty - a.quantity) = 0 THEN 1
                                                              ELSE 0
                                                           END)
                                                THEN
                                                   'Y'
                                                ELSE
                                                   'N'
                                             END
                                        FROM order_dispatch_schedule a
                                       WHERE a.order_no = ssd.sales_order_no)
                                        AS is_full_reference
                              FROM dist_ip_ssd_purchase_order ssd
                                   LEFT JOIN reference_detail rd
                                      ON ssd.sales_order_no = rd.reference_no
                                   LEFT JOIN master_transaction mt
                                      ON rd.voucher_no = mt.voucher_no
                                   LEFT JOIN form_setup fs
                                      ON mt.form_code = fs.form_code
                             WHERE ssd.order_no = '{model.ORDER_NO}'
                             {salesOrderNo}";
            try
            {
                data = dbContext.SqlQuery<PoTimelineModel>(query).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception("Data not found!!!", ex);
            }
            return data;
        }

        /*public dynamic GetPurchaseOrderDetailItems(PurchaseOrderStatus model, NeoErpCoreEntity dbContext)
        {
           var data = new List<PurchaseOrderItem>();

           string query = $@"SELECT
                             i.item_edesc AS item,
                               i.item_code as code,
                              NVL(NVL(ssd.quantity, 0) + NVL(ssd.approve_qty, 0), 0) AS order_quantity,
                              NVL(ssd.approve_qty, 0) AS approved_quantity,
                               NVL(d.quantity, 0) AS allocated_quantity
                           FROM dist_ip_ssd_purchase_order ssd
                           LEFT JOIN order_dispatch_schedule d
                             ON ssd.sales_order_no = d.order_no
                             AND ssd.item_code = d.item_code
                             AND ssd.company_code = d.company_code
                           LEFT JOIN ip_item_master_setup i
                             ON ssd.item_code = i.item_code
                             AND ssd.company_code = i.company_code
                           WHERE ssd.order_no = '{model.ORDER_NO}'";
           try
           {
               data = dbContext.SqlQuery<PurchaseOrderItem>(query).ToList();
           }
           catch (Exception ex)
           {
               throw new Exception("Data not found!!!", ex);
           }
           return data;
        }*/
        public dynamic GetPurchaseOrderDetailItems(PurchaseOrderStatus model, NeoErpCoreEntity dbContext)
        {
            var data = new List<PurchaseOrderItem>();
            var sales = "";
            var chalan = "";

            if (!string.IsNullOrEmpty(model.SALES_ORDER_NO))
            {
                sales = $@" and SALES_ORDER_NO = '{model.SALES_ORDER_NO}' ";
            }
            if (!string.IsNullOrWhiteSpace(model.CHALAN_NO))
            {
                chalan = $@" and voucher_no = '{model.CHALAN_NO}'";
            }

            string query = $@"select item, code, order_quantity, approved_quantity, allocated_quantity, 
                            (select calc_quantity from sa_sales_chalan where chalan_no = '{model.CHALAN_NO}' and item_code = code) as Delivered_Quantity,
                            (select calc_quantity from sa_sales_chalan where chalan_no = '{model.CHALAN_NO}' and item_code = code) as Dispatch_Quantity
                            from (SELECT
                              i.item_edesc AS item,
                                i.item_code as code,
                               NVL(NVL(ssd.quantity, 0) + NVL(ssd.approve_qty, 0), 0) AS order_quantity,
                               NVL(ssd.approve_qty, 0) AS approved_quantity,
                                NVL(d.quantity, 0) AS allocated_quantity
                            FROM dist_ip_ssd_purchase_order ssd
                            LEFT JOIN order_dispatch_schedule d
                              ON ssd.sales_order_no = d.order_no
                              AND ssd.item_code = d.item_code
                              AND ssd.company_code = d.company_code
                            LEFT JOIN ip_item_master_setup i
                              ON ssd.item_code = i.item_code
                              AND ssd.company_code = i.company_code
                            Left Join reference_detail rd
                                On ssd.sales_order_no = rd.reference_no
                                and rd.reference_item_code = ssd.item_code
                                AND SSD.COMPANY_CODE = RD.COMPANY_CODE
                            WHERE ssd.order_no = '{model.ORDER_NO}' {sales} {chalan}
                            )
                            ";
            //string query = $@"
            //        SELECT a.*
            //        ,(select calc_quantity from sa_sales_chalan where chalan_no = '{model.CHALAN_NO}' and item_code = code) as Delivered_Quantity,
            //                                    (select calc_quantity from sa_sales_chalan where chalan_no = '{model.CHALAN_NO}' and item_code = code) as Dispatch_Quantity
            //         FROM VW_PO_STATUS_ITEM a where order_no = '{model.ORDER_NO}' {sales}
            //    ";
            try
            {
                data = dbContext.SqlQuery<PurchaseOrderItem>(query).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Data not found!!!", ex);
            }
            return data;
        }
        public dynamic GetPurchaseOrderDeliveryDetailItems(PurchaseOrderStatus model, NeoErpCoreEntity dbContext)
        {
            var data = new List<PurchaseOrderItem>();
            var sales = "";
            var chalan = "";

            if (!string.IsNullOrEmpty(model.SALES_ORDER_NO))
            {
                sales = $@" and ssd.SALES_ORDER_NO = '{model.SALES_ORDER_NO}' ";
                chalan = $@" and ssd.item_code in (select item_code from sa_sales_order where order_no = '{model.SALES_ORDER_NO}')";

                if (!string.IsNullOrEmpty(model.CHALAN_NO))
                {
                    chalan = $@" and ssd.item_code in (select item_code from sa_sales_chalan where chalan_no = '{model.CHALAN_NO}')";
                }
            }

            string query = $@"select item, code, order_quantity, approved_quantity, allocated_quantity, 
                            --(select SUM(calc_quantity) from sa_sales_chalan where chalan_no = '{model.CHALAN_NO}' and item_code = code) as Delivered_Quantity,
                            --(select SUM(calc_quantity) from sa_sales_chalan where chalan_no = '{model.CHALAN_NO}' and item_code = code) as Dispatch_Quantity
                            (select calc_quantity from sa_sales_chalan where chalan_no = '{model.CHALAN_NO}' and item_code = code) as Delivered_Quantity,
                            (select calc_quantity from sa_sales_chalan where chalan_no = '{model.CHALAN_NO}' and item_code = code) as Dispatch_Quantity
                            from (SELECT
                              i.item_edesc AS item,
                                i.item_code as code,
                               NVL(NVL(ssd.quantity, 0) + NVL(ssd.approve_qty, 0), 0) AS order_quantity,
                               NVL(ssd.approve_qty, 0) AS approved_quantity,
                                NVL(d.quantity, 0) AS allocated_quantity
                            FROM dist_ip_ssd_purchase_order ssd
                            LEFT JOIN order_dispatch_schedule d
                              ON ssd.sales_order_no = d.order_no
                              AND ssd.item_code = d.item_code
                              AND ssd.company_code = d.company_code
                            LEFT JOIN ip_item_master_setup i
                              ON ssd.item_code = i.item_code
                              AND ssd.company_code = i.company_code
                            WHERE ssd.order_no = '{model.ORDER_NO}' {sales} {chalan}
                            )
                            ";
            try
            {
                data = dbContext.SqlQuery<PurchaseOrderItem>(query).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Data not found!!!", ex);
            }
            return data;
        }
        public dynamic GetCurrentOrderStocks(OrderStocksModel model, NeoErpCoreEntity dbContext)
        {
            var result = new Dictionary<string, List<object>>();
            var code = "";
            if (model.ENTITY_TYPE == "R")
            {
                code = $@"select entity_code from dist_reseller_entity where reseller_code = '{model.CODE}'";
            }
            else
            {
                code = $@"'{model.CODE}'";
            }

            string query = $@"
                            SELECT DISTINCT
                                     INITCAP (b.LOCATION_EDESC) as LOCATION_EDESC,
                                     NVL (SUM (NVL (a.IN_QUANTITY, 0)) - SUM (NVL (a.OUT_QUANTITY, 0)), 0) STOCK_BALANCE, INITCAP(ITEM_CODE) as ITEM_CODE
                                FROM V$VIRTUAL_STOCK_WIP_LEDGER1 a, IP_LOCATION_SETUP b
                               WHERE     a.LOCATION_CODE = b.LOCATION_CODE
                                     AND a.COMPANY_CODE = b.COMPANY_CODE
                                     --AND a.ITEM_CODE = '1664'
                                     AND a.COMPANY_CODE = '{model.COMPANY_CODE}'
                            GROUP BY INITCAP (b.LOCATION_EDESC), INITCAP(ITEM_CODE)
                            ORDER BY INITCAP (b.LOCATION_EDESC)
                            ";
            try
            {
                var data = dbContext.SqlQuery<OrderStocksModel>(query).ToList();

                foreach (var item in data)
                {
                    string itemCode = item.ITEM_CODE;

                    if (!result.ContainsKey(itemCode))
                    {
                        result[itemCode] = new List<object>();
                    }

                    result[itemCode].Add(new
                    {
                        name = item.LOCATION_EDESC,
                        qty = item.STOCK_BALANCE
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Data not found!!!", ex);
            }
        }
        public dynamic GetCustomerLastRate(LastRateModel model, NeoErpCoreEntity dbContext)
        {
            var data = new List<LastRateModel>();

            string query = $@"
        SELECT item_code,
               item_edesc,
               customer_code,
               calc_unit_price AS LAST_RATE
        FROM (
            SELECT ssi.item_code,
                   iims.item_edesc,
                   ssi.customer_code,
                   ssi.calc_unit_price,
                   ssi.created_date,
                   ROW_NUMBER() OVER (
                       PARTITION BY ssi.customer_code, ssi.item_code
                       ORDER BY ssi.created_date DESC
                   ) AS rn
            FROM sa_sales_invoice ssi
            JOIN ip_item_master_setup iims
                ON ssi.item_code = iims.item_code
               AND ssi.company_code = iims.company_code
            WHERE ssi.company_code = '{model.COMPANY_CODE}'
        ) sub
        WHERE rn = 1 AND customer_code = '{model.CUSTOMER_CODE}'
    ";

            try
            {
                data = dbContext.SqlQuery<LastRateModel>(query).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Data not found!!!", ex);
            }

            return data.ToDictionary(x => x.ITEM_CODE, x => x.LAST_RATE);
        }
        public dynamic GetOdoMeterModeType(Dictionary<string, string> model, NeoErpCoreEntity dbContext)
        {
            try
            {
                string query = @"SELECT MODE_CODE, MODE_NAME 
                         FROM DIST_TRAVEL_MODE 
                         WHERE IS_ACTIVE = 'Y'";

                var conn = dbContext.Database.Connection;
                if (conn.State != System.Data.ConnectionState.Open)
                    conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = query;
                    using (var reader = cmd.ExecuteReader())
                    {
                        var result = new List<ExpandoObject>();

                        while (reader.Read())
                        {
                            dynamic row = new ExpandoObject();
                            row.ID = reader["MODE_CODE"].ToString();
                            row.NAME = reader["MODE_NAME"].ToString();
                            result.Add(row);
                        }

                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Data not found!!!", ex);
            }
        }
        public dynamic GetOdoMeterClaimType(Dictionary<string, string> model, NeoErpCoreEntity dbContext)
        {
            try
            {
                string query = @"SELECT CLAIM_CODE, CLAIM_NAME 
                         FROM DIST_CLAIM_TYPE 
                         WHERE IS_ACTIVE = 'Y'";

                var conn = dbContext.Database.Connection;
                if (conn.State != System.Data.ConnectionState.Open)
                    conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = query;
                    using (var reader = cmd.ExecuteReader())
                    {
                        var result = new List<ExpandoObject>();

                        while (reader.Read())
                        {
                            dynamic row = new ExpandoObject();
                            row.ID = reader["CLAIM_CODE"].ToString();
                            row.NAME = reader["CLAIM_NAME"].ToString();
                            result.Add(row);
                        }

                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Data not found!!!", ex);
            }
        }
        public dynamic GetVehicleSetup(Dictionary<string, string> model, NeoErpCoreEntity dbContext)
        {
            try
            {
                string query = $@"
                    SELECT  ID, SP_CODE, VEHICLE_CODE, VEHICLE_NO, VEHICLE_MODEL, MILAGE, FUEL_PRICE, YEAR_MADE, IMAGE, DELETED_FLAG, CREATED_DATE, CREATED_BY, RECOMMEND_FLAG, RECOMMEND_DATE, RECOMMEND_BY, APPROVE_FLAG, APPROVE_DATE, APPROVE_BY 
                    ,(select * from (SELECT END_KM_READING
                        FROM dist_claim_report
                        WHERE sp_code = '{model["SP_CODE"]}' and trunc(check_in) < trunc(sysdate)
                          AND claim_code = 'VA' ORDER BY claim_id DESC) where rownum = 1) START_KM,
                        (select mode_name from dist_travel_mode where mode_code = VEHICLE_CODE) mode_name
                    FROM DIST_VEHICLE_SETUP WHERE DELETED_FLAG = 'N' and sp_code = '{model["SP_CODE"]}'
                ";

                var conn = dbContext.Database.Connection;
                if (conn.State != System.Data.ConnectionState.Open)
                    conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = query;
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            dynamic row = new ExpandoObject();
                            row.ID = SafeGetString(reader, "ID");
                            row.MODE_CODE = SafeGetString(reader, "VEHICLE_CODE");
                            row.MODE_NAME = SafeGetString(reader, "MODE_NAME");
                            row.VEHICLE_NO = SafeGetString(reader, "VEHICLE_NO");
                            row.MODEL = SafeGetString(reader, "VEHICLE_MODEL");
                            row.MILEAGE = SafeGetString(reader, "MILAGE");
                            row.FUEL_PRICE = SafeGetString(reader, "FUEL_PRICE");
                            row.YEAR_MADE = SafeGetString(reader, "YEAR_MADE");
                            row.APPROVE_FLAG = SafeGetString(reader, "APPROVE_FLAG");
                            row.DATE = SafeGetString(reader, "CREATED_DATE");
                            row.START_KM = SafeGetString(reader, "START_KM");
                            return row;
                        }
                        else
                        {
                            dynamic row = new ExpandoObject();
                            row.ID = row.MODE_CODE = row.MODE_NAME = row.VEHICLE_NO = row.MODEL =
                            row.MILEAGE = row.FUEL_PRICE = row.YEAR_MADE = row.APPROVE_FLAG =
                            row.DATE = row.START_KM = string.Empty;

                            return row;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Data not found!!!", ex);
            }
        }
        private string SafeGetString(IDataReader reader, string column)
        {
            int idx = reader.GetOrdinal(column);
            return reader.IsDBNull(idx) ? string.Empty : reader.GetValue(idx).ToString();
        }
        public dynamic GetClaimId(Dictionary<string, string> model, NeoErpCoreEntity dbContext)
        {
            try
            {
                //string query = $@"select * from DIST_VEHICLE_SETUP where deleted_flag = 'N' and sp_code = '{model["SP_CODE"]}'";
                string claimQuery = $@"
                    SELECT  * FROM dist_claim_report WHERE TRUNC(CHECK_IN) = TRUNC(SYSDATE) and sp_code = '{model["SP_CODE"]}' and rownum = 1 order by claim_id desc
                ";

                string vehicleQuery = $@"
                    SELECT  ID, SP_CODE, VEHICLE_CODE, VEHICLE_NO, VEHICLE_MODEL, MILAGE, FUEL_PRICE, YEAR_MADE, IMAGE, DELETED_FLAG, CREATED_DATE, CREATED_BY, RECOMMEND_FLAG, RECOMMEND_DATE, RECOMMEND_BY, APPROVE_FLAG, APPROVE_DATE, APPROVE_BY 
                    ,(select * from (SELECT END_KM_READING
                        FROM dist_claim_report
                        WHERE sp_code = '{model["SP_CODE"]}' and trunc(check_in) < trunc(sysdate)
                          AND claim_code = 'VA' ORDER BY claim_id DESC) where rownum = 1) START_KM,
                        (select mode_name from dist_travel_mode where mode_code = VEHICLE_CODE) mode_name
                    FROM DIST_VEHICLE_SETUP WHERE DELETED_FLAG = 'N' and sp_code = '{model["SP_CODE"]}'
                ";

                var conn = dbContext.Database.Connection;
                if (conn.State != System.Data.ConnectionState.Open)
                    conn.Open();

                dynamic row = new ExpandoObject();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = vehicleQuery;
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            row.MODE_CODE = SafeGetString(reader, "VEHICLE_CODE");
                            row.MODE_NAME = SafeGetString(reader, "MODE_NAME");
                            row.VEHICLE_NO = SafeGetString(reader, "VEHICLE_NO");
                            row.MODEL = SafeGetString(reader, "VEHICLE_MODEL");
                            row.MILEAGE = SafeGetString(reader, "MILAGE");
                            row.FUEL_PRICE = SafeGetString(reader, "FUEL_PRICE");
                            row.YEAR_MADE = SafeGetString(reader, "YEAR_MADE");
                            row.APPROVE_FLAG = SafeGetString(reader, "APPROVE_FLAG");
                            row.DATE = SafeGetString(reader, "CREATED_DATE");
                            row.START_KM = SafeGetString(reader, "START_KM");
                        }
                        else
                        {
                            row.MODE_CODE = row.MODE_NAME = row.VEHICLE_NO = row.MODEL =
                            row.MILEAGE = row.FUEL_PRICE = row.YEAR_MADE = row.APPROVE_FLAG =
                            row.DATE = row.START_KM = string.Empty;
                        }
                    }
                }

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = claimQuery;
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            row.CLAIM_ID = SafeGetString(reader, "CLAIM_ID");
                            row.CLAIM_CODE = SafeGetString(reader, "CLAIM_CODE");
                            row.TRAVEL_MODE = SafeGetString(reader, "TRAVEL_MODE");
                            row.TRAVEL_PURPOSE = SafeGetString(reader, "TRAVEL_PURPOSE");
                            row.START_KM_READING = SafeGetString(reader, "START_KM_READING");
                            row.END_KM_READING = SafeGetString(reader, "END_KM_READING");
                            row.CHECK_IN = SafeGetString(reader, "CHECK_IN");
                            row.CHECK_OUT = SafeGetString(reader, "CHECK_OUT");
                            row.CHECK_IN_IMAGE = SafeGetString(reader, "CHECK_IN_IMAGE");
                            row.CHECK_OUT_IMAGE = SafeGetString(reader, "CHECK_OUT_IMAGE");
                            row.TOTAL_EXPENSE = SafeGetString(reader, "TOTAL_EXPENSE");
                            row.IN_REMARKS = SafeGetString(reader, "IN_REMARKS");
                            row.OUT_REMARKS = SafeGetString(reader, "OUT_REMARKS");
                        }
                        else
                        {
                            row.CLAIM_ID = row.CLAIM_CODE = row.TRAVEL_MODE = row.TRAVEL_PURPOSE =
                            row.START_KM_READING = row.END_KM_READING = row.CHECK_IN = row.CHECK_OUT =
                            row.CHECK_IN_IMAGE = row.CHECK_OUT_IMAGE = row.TOTAL_EXPENSE =
                            row.IN_REMARKS = row.OUT_REMARKS = string.Empty;
                        }
                    }
                }

                return row;
            }
            catch (Exception ex)
            {
                throw new Exception("Data not found!!!", ex);
            }
        }
        public dynamic GetOdoMeterReport(Dictionary<string, string> model, NeoErpCoreEntity dbContext)
        {
            try
            {
                var sp_code = model["SP_CODE"]?.ToString();
                var company_code = model["COMPANY_CODE"]?.ToString();
                var branch_code = model["BRANCH_CODE"]?.ToString();
                var claim_code = model["CLAIM_CODE"]?.ToString() ?? "VA";
                var from_date = model["FROM_DATE"]?.ToString() ?? "";
                var to_date = model["TO_DATE"]?.ToString() ?? "";

                string query = $@"
                    SELECT 
                        CLAIM_ID,
                        SP_CODE,
                        CLAIM_CODE,
                        TRAVEL_MODE,
                        TRAVEL_PURPOSE,
                        VEHICLE_MILEAGE,
                        FUEL_PRICE,
                        START_KM_READING,
                        END_KM_READING,
                        (NVL(END_KM_READING,0) - NVL(START_KM_READING,0)) AS KM_RUN,
                        CHECK_IN,
                        CHECK_OUT,
                        TOTAL_EXPENSE,
                        IN_REMARKS,
                        OUT_REMARKS,
                        CREATED_BY,
                        CREATED_AT,
                        CHECK_IN_IMAGE,
                        CHECK_OUT_IMAGE
                    FROM DIST_CLAIM_REPORT
                    WHERE SP_CODE = '{sp_code}' AND END_KM_READING IS NOT NULL
                      AND CLAIM_CODE = '{claim_code}'
                      AND TRUNC(CHECK_IN) BETWEEN TO_DATE('{from_date}', 'YYYY-MM-DD') 
                                              AND TO_DATE('{to_date}', 'YYYY-MM-DD')
                    ORDER BY CREATED_AT DESC
                ";
                //string query = $@"select * from DIST_VEHICLE_SETUP where deleted_flag = 'N' and sp_code = '{model["SP_CODE"]}'";

                var conn = dbContext.Database.Connection;
                if (conn.State != System.Data.ConnectionState.Open)
                    conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = query;
                    using (var reader = cmd.ExecuteReader())
                    {
                        var result = new List<ExpandoObject>();

                        while (reader.Read())
                        {
                            dynamic row = new ExpandoObject();
                            row.CLAIM_ID = SafeGetString(reader, "CLAIM_ID");
                            row.SP_CODE = SafeGetString(reader, "SP_CODE");
                            row.CLAIM_CODE = SafeGetString(reader, "CLAIM_CODE");
                            row.TRAVEL_MODE = SafeGetString(reader, "TRAVEL_MODE");
                            row.TRAVEL_PURPOSE = SafeGetString(reader, "TRAVEL_PURPOSE");
                            row.VEHICLE_MILEAGE = SafeGetString(reader, "VEHICLE_MILEAGE");
                            row.FUEL_PRICE = SafeGetString(reader, "FUEL_PRICE");
                            row.START_KM_READING = SafeGetString(reader, "START_KM_READING");
                            row.END_KM_READING = SafeGetString(reader, "END_KM_READING");
                            row.KM_RUN = SafeGetString(reader, "KM_RUN");
                            row.CHECK_IN = SafeGetString(reader, "CHECK_IN");
                            row.CHECK_OUT = SafeGetString(reader, "CHECK_OUT");
                            row.TOTAL_EXPENSE = SafeGetString(reader, "TOTAL_EXPENSE");
                            row.IN_REMARKS = SafeGetString(reader, "IN_REMARKS");
                            row.OUT_REMARKS = SafeGetString(reader, "OUT_REMARKS");
                            row.CREATED_BY = SafeGetString(reader, "CREATED_BY");
                            row.CREATED_AT = SafeGetString(reader, "CREATED_AT");
                            row.CHECK_IN_IMAGE = SafeGetString(reader, "CHECK_IN_IMAGE");
                            row.CHECK_OUT_IMAGE = SafeGetString(reader, "CHECK_OUT_IMAGE");

                            result.Add(row);
                        }
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Data not found!!!", ex);
            }
        }

        public string fetchTransactionData(TransactionRequestModel model, NeoErpCoreEntity dbContext)
        {
            DateTime currentDateTime = DateTime.Now;
            string fileName = "Transactions_" + currentDateTime.ToString("yyyyMMdd_HHmmss") + ".pdf";
            string filePath = Path.Combine(UploadPath, "Document", fileName);
            if(!Directory.Exists(Path.Combine(UploadPath, "Document"))) Directory.CreateDirectory(Path.Combine(UploadPath, "Document"));
            dynamic transactions = FetchSubLedgers(model, dbContext);
            CustomerModels customerData = fetchCustomerDetails(model, dbContext);
            // HTML content with inline CSS
            string htmlContent = $@"
        <html>
            <head>
                <style>
                    .header {{
                        text-align: center;
                    }}
                .head{{
                        font-size:14px; 
                    font-weight: bold;
                    }}
                    .style{{
                        font-size:12px; 
                    font-weight: bold;
                    }}
                .data{{
                        font-size:12px; 
                    }}
                    .transaction-table {{
                        width: 100%;
                        border-collapse: collapse;
                        margin-top: 18px;
                    }}
                    .transaction-table th, .transaction-table td {{
                        border: 1px solid #ccc;
                        padding: 8px;
                    }}
                    .transaction-table th {{
                        background-color: #28a745;
                        color: white;
                        font-size:12px; 
                    }}
                .transaction-table td {{
                        font-size:12px; 
                    }}
                    .ageing-table {{
                        width: 100%;
                        border-collapse: collapse;
                        margin-top: 18px;
                    }}
                    .ageing-table th, .ageing-table td {{
                        border: 1px solid #ccc;
                        padding: 8px;
                        text-align: center;
                    }}
                    .rightAlign{{
                        text-align: right;
                    }}
                    .ageing-table td {{
                        font-size:12px; 
                    }}
                    .ageing-table th {{
                        background-color: #28a745;
                        color: white;
                        font-size:12px; 
                    }}
                </style>
            </head>
            <body>
                <div class='header'>
                    <p class='head'>{customerData.COMPANY_NAME}</p>
                    <p  class='style' >{customerData.ADDRESS}</p>
                    <p  class='data' >Branch:{customerData.BRANCH_NAME}</p>
                </div>
                <div>
                    <p class='data'>Customer: {customerData.CUSTOMER_EDESC}</p>
                    <p class='data'>Address: {customerData.REGD_OFFICE_EADDRESS}</p>
                    <p class='data'>Contact: {customerData.CONTACT}</p>
                    <p class='data'>Transaction From: {model.from_date} to {model.to_date}</p>
                </div>

                <table class='transaction-table'>
                    <thead>
                        <tr>
                            <th>Date</th>
                            <th>Miti</th>
                            <th>Description</th>
                            <th>Debit Amount</th>
                            <th>Credit Amount</th>
                            <th>Balance</th>
                        </tr>
                    </thead>
                    <tbody>";

            //// Create a dictionary to store opening values
            var openingData = new Dictionary<string, string>();

            // Check if 'opening' data exists in transactions and convert it
            if (transactions != null && transactions.opening != null)
            {
                foreach (var kvp in (Dictionary<string, object>)transactions.opening)
                {
                    // Convert value to string, if it's a numeric value
                    if (kvp.Value is float || kvp.Value is double || kvp.Value is int)
                    {
                        openingData[kvp.Key] = kvp.Value.ToString();
                    }
                    else
                    {
                        openingData[kvp.Key] = kvp.Value?.ToString() ?? "0";
                    }
                }
            }

            // Display Opening data
            htmlContent += "<tr><td></td><td></td><td>Opening</td>";
            htmlContent += openingData.ContainsKey("DR") ? $"<td class='rightAlign'>{openingData["DR"]}</td>" : "<td class='rightAlign'>0.0</td>";
            htmlContent += openingData.ContainsKey("CR") ? $"<td class='rightAlign'>{openingData["CR"]}</td>" : "<td class='rightAlign'>0.0</td>";
            htmlContent += openingData.ContainsKey("DR") && openingData.ContainsKey("CR")
                            ? $"<td  class='rightAlign'>{(float.Parse(openingData["DR"]) - float.Parse(openingData["CR"]))}</td>"
                            : "<td  class='rightAlign'>0.0</td>";
            htmlContent += "</tr>";
            // Add the transaction data to the HTML table

            //var opening = openingData.ContainsKey("cr_amount")
            //    ? -float.Parse(openingData["cr_amount"])
            //    : openingData.ContainsKey("dr_amount")
            //        ? float.Parse(openingData["dr_amount"])
            //        : 0.0;

            var opening = openingData.ContainsKey("DR") && float.Parse(openingData["DR"]) > 0 ? float.Parse(openingData["DR"]) : float.Parse(openingData["CR"]);

            opening = float.Parse(opening.ToString("F2"));

            var balance = opening;
            foreach (var transaction in transactions.transactions)
            {
                var transactionData = new Dictionary<string, string>();



                foreach (var kvp in (Dictionary<string, object>)transaction)
                {
                    transactionData[kvp.Key] = kvp.Value.ToString();
                }

                //balance = transactionData.ContainsKey("dr_amount")
                //              ? balance + float.Parse(transactionData["dr_amount"])
                //              : transactionData.ContainsKey("cr_amount") ? balance - float.Parse(transactionData["cr_amount"]) : balance;

                //if (transactionData.ContainsKey("dr_amount") && float.TryParse(transactionData["dr_amount"], out float drAmount))
                //{
                //    balance += drAmount;
                //}
                //else if (transactionData.ContainsKey("cr_amount") && float.TryParse(transactionData["cr_amount"], out float crAmount))
                //{
                //    balance -= crAmount;
                //}

                var drAmount = float.Parse(transactionData["dr_amount"]);
                var crAmount = float.Parse(transactionData["cr_amount"]);


                balance += drAmount;
                balance -= crAmount;


                htmlContent += transactionData.ContainsKey("voucher_date") ? $"<tr><td>{transactionData["voucher_date"]}</td>" : "<td></td>";
                htmlContent += transactionData.ContainsKey("miti") ? $"<td>{transactionData["miti"]}</td>" : "<td></td>";
                htmlContent += transactionData.ContainsKey("particulars") ? $"<td>{transactionData["particulars"]}</td>" : "<td></td>";
                htmlContent += transactionData.ContainsKey("dr_amount") ? $"<td class='rightAlign'>{transactionData["dr_amount"]}</td>" : "<td class='rightAlign'>0.0</td>";
                htmlContent += transactionData.ContainsKey("cr_amount") ? $"<td class='rightAlign'>{transactionData["cr_amount"]}</td>" : "<td class='rightAlign'>0.0</td>";
                htmlContent += $"<td class='rightAlign'>{balance.ToString("F2")}</td></tr>";

                //htmlContent += transactionData.ContainsKey("dr_amount") && openingData.ContainsKey("cr_amount")
                //               ? $"<td class='rightAlign'>{float.Parse(transactionData["dr_amount"]) - float.Parse(openingData["cr_amount"])}</td>"
                //               : "<td class='rightAlign'>0.0</td></tr>";


            }
            htmlContent += "<tr><td></td>";
            htmlContent += "<td></td>";
            htmlContent += "<td>Total</td>";
            htmlContent += (transactions.debit_total != null && transactions.debit_total != 0)
                ? $"<td class='rightAlign'>{transactions.debit_total}</td>"
                : "<td>0.0</td>";

            htmlContent += (transactions.credit_total != null && transactions.credit_total != 0)
                ? $"<td class='rightAlign'>{transactions.credit_total}</td>"
                : "<td class='rightAlign'>0.0</td>";

            htmlContent += (transactions.closing_balance != null && transactions.closing_balance != 0)
                ? $"<td class='rightAlign'>{transactions.closing_balance}</td>"
                : "<td  class='rightAlign'>0.0</td>";
            htmlContent += "</tr></tbody></table>";
            // Add the ageing report table
            htmlContent += "<table class='ageing-table'><thead><tr>";
            string[] headers = { "Ageing", "0-30", "31-60", "61-90", "91-120", "120++", "Total" };
            foreach (var headerText in headers)
            {
                htmlContent += $"<th>{headerText}</th>";
            }
            htmlContent += "</tr></thead><tbody>";
            // Store ageing report in a single variable (Dictionary)
            var ageingReportData = new Dictionary<string, string>();

            foreach (var kvp in (IEnumerable<KeyValuePair<string, float>>)transactions.ageingReport)
            {
                ageingReportData[kvp.Key] = kvp.Value.ToString();
            }
            // Generate HTML using the stored variable
            htmlContent += "<tr><td>Outstanding Amount</td>";
            htmlContent += ageingReportData.ContainsKey("0-30") ? $"<td class='rightAlign'>{ageingReportData["0-30"]}</td>" : "<td class='rightAlign'>0.0</td>";
            htmlContent += ageingReportData.ContainsKey("30-60") ? $"<td class='rightAlign'>{ageingReportData["30-60"]}</td>" : "<td class='rightAlign'>0.0</td>";
            htmlContent += ageingReportData.ContainsKey("61-90") ? $"<td class='rightAlign'>{ageingReportData["61-90"]}</td>" : "<td class='rightAlign'>0.0</td>";
            htmlContent += ageingReportData.ContainsKey("91-120") ? $"<td class='rightAlign'>{ageingReportData["91-120"]}</td>" : "<td class='rightAlign'>0.0</td>";
            htmlContent += ageingReportData.ContainsKey("120++") ? $"<td class='rightAlign'>{ageingReportData["120++"]}</td>" : "<td class='rightAlign'>0.0</td>";
            htmlContent += ageingReportData.ContainsKey("total") ? $"<td class='rightAlign'>{ageingReportData["total"]}</td>" : "<td class='rightAlign'>0.0</td>";
            htmlContent += "</tr>";
            htmlContent += "</tbody></table></body></html>";
            try
            {
                // Create the PDF file
                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    var document = new Document();
                    try
                    {
                        var writer = PdfWriter.GetInstance(document, fs);
                        document.Open();

                        // Parse the HTML content and add it to the document
                        using (var stringReader = new StringReader(htmlContent))
                        {
                            XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, stringReader);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error while generating the PDF: " + ex.Message, ex);
                    }
                    finally
                    {
                        document.Close();
                    }
                }

                // Construct file URL
                string fileUrl = "http://" + HttpContext.Current.Request.Url.Host + ":" + HttpContext.Current.Request.Url.Port +
                                 "/Areas/NeoErp.Distribution/Images/Document/" + fileName;
                //Debug.WriteLine(fileUrl);
                return fileUrl;
            }
            catch (Exception ex)
            {
                // Log and return error
                return "Error: " + ex.Message;
            }

        }
        public int checkGlobalCompany()
        {
            return _objectEntity.SqlQuery<int>(
                    "SELECT COUNT(*) FROM company_setup WHERE NVL(consolidate_flag, 'N') <> 'Y'"
                ).FirstOrDefault();
        }
        public dynamic FetchSubLedgers(TransactionRequestModel model, NeoErpCoreEntity dbContext)
        {
            var slData = new List<Dictionary<string, object>>();
            var opening = new Dictionary<string, object>();
            List<double> drsArray = new List<double>();
            List<double> crsArray = new List<double>();
            Dictionary<string, float> ageingVals = new Dictionary<string, float>();
            string branchCode = model.BRANCH_CODE;
            string company_code = model.COMPANY_CODE;

            string formatedBranchCodes;
            if (branchCode.Contains("[") && branchCode.Contains("]"))
            {
                formatedBranchCodes = branchCode.Replace("[", "'").Replace("]", "'").Replace(", ", "','");
            }
            else
            {
                formatedBranchCodes = $"'{branchCode}'";
            }

            string formatedCompanyCodes;
            if (company_code.Contains("[") && company_code.Contains("]"))
            {
                formatedCompanyCodes = company_code.Replace("[", "'").Replace("]", "'").Replace(", ", "','");
            }
            else
            {
                formatedCompanyCodes = $"'{company_code}'";
            }

            Debug.WriteLine(formatedBranchCodes);
            var dateData = dbContext.SqlQuery<DateModel>($"select startdate as START_DATE, enddate as END_DATE from v_date_range where rangename='This Year'").ToList();
            if (string.IsNullOrEmpty(model.from_date))
            {
                model.from_date = dateData[0].START_DATE.ToString("dd-MMM-yyyy");
            }
            if (string.IsNullOrEmpty(model.to_date))
            {
                model.to_date = dateData[0].END_DATE.ToString("dd-MMM-yyyy");
            }
            // First query to fetch transaction details

            string query1 = $@"
                        SELECT a.voucher_date, a.voucher_no, a.dr_amount * a.exchange_rate AS dr_amount, 
                               a.cr_amount * a.exchange_rate AS cr_amount, a.particulars, a.created_by, 
                               a.currency_code, a.exchange_rate, BS_DATE(a.voucher_date) AS bs_date,
                               remarks, 
                         (SELECT 
                                reference_no
                            FROM FINANCIAL_REFERENCE_DETAIL
                        WHERE  company_code in ({formatedCompanyCodes}) and a.voucher_no=voucher_no 
                        GROUP BY form_code,
                                reference_form_code,
                                voucher_no,
                                reference_no)  manual_no
                        FROM V$VIRTUAL_SUB_LEDGER a
                        WHERE a.sub_code = '{model.sub_code}'
                          AND a.company_code in ({formatedCompanyCodes})
                          AND a.branch_code in ({formatedBranchCodes})
                          AND trunc(a.voucher_date) BETWEEN '{model.from_date}' and 
                          '{model.to_date}'
                          AND a.deleted_flag = 'N'
                          AND a.form_code != 0
                        ORDER BY a.voucher_date, voucher_no";

            int global = checkGlobalCompany();
            if (global == 0)
            {
                query1 = $@"
                        SELECT a.voucher_date, a.voucher_no, a.dr_amount * a.exchange_rate AS dr_amount, 
                               a.cr_amount * a.exchange_rate AS cr_amount, a.particulars, a.created_by, 
                               a.currency_code, a.exchange_rate, BS_DATE(a.voucher_date) AS bs_date,
                               remarks, 
                         b.reference_no manual_no
                        FROM V$VIRTUAL_SUB_LEDGER a, FINANCIAL_REFERENCE_DETAIL b
                        WHERE a.sub_code = '{model.sub_code}' AND a.voucher_no = b.voucher_no
                          AND a.company_code in ({formatedCompanyCodes})
                          AND a.branch_code in ({formatedBranchCodes})
                          AND b.company_code in ({formatedCompanyCodes})
                          AND b.branch_code in ({formatedBranchCodes})
                          AND trunc(a.voucher_date) BETWEEN '{model.from_date}' and 
                          '{model.to_date}'
                          AND a.deleted_flag = 'N'
                          AND a.form_code != 0
                        ORDER BY a.voucher_date, voucher_no";
            }

            string sConnStr1 = ConfigurationManager.ConnectionStrings["NeoErpCoreEntity"].ToString();
            string[] tokens = sConnStr1.Split('"');

            using (OracleConnection objConn = new OracleConnection(tokens[1]))
            {
                objConn.Open();

                using (OracleCommand cmd = new OracleCommand(query1, objConn))
                {
                    cmd.Parameters.Add(":SubCode", model.sub_code);
                    cmd.Parameters.Add(":CompanyCode", model.COMPANY_CODE);
                    cmd.Parameters.Add(":BranchCode", model.BRANCH_CODE);
                    cmd.Parameters.Add(":FromDate", model.from_date);
                    cmd.Parameters.Add(":ToDate", model.to_date);

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            double debitAmt = reader["dr_amount"] != DBNull.Value ? Convert.ToDouble(reader["dr_amount"]) : 0;
                            double creditAmt = reader["cr_amount"] != DBNull.Value ? Convert.ToDouble(reader["cr_amount"]) : 0;

                            drsArray.Add(debitAmt);
                            crsArray.Add(creditAmt);

                            var vals = new Dictionary<string, object>
                            {
                                ["voucher_date"] = Convert.ToDateTime(reader["voucher_date"]).ToString("dd-MMM-yyyy"),
                                ["voucher_no"] = reader["voucher_no"].ToString(),
                                ["dr_amount"] = debitAmt,
                                ["cr_amount"] = creditAmt,
                                ["particulars"] = reader["particulars"].ToString(),
                                ["created_by"] = reader["created_by"].ToString(),
                                ["currency_code"] = reader["currency_code"].ToString(),
                                ["exchange_rate"] = reader["exchange_rate"] != DBNull.Value ? Convert.ToDouble(reader["exchange_rate"]) : 0,
                                ["miti"] = reader["bs_date"].ToString(),
                                ["remarks"] = reader["remarks"].ToString(),
                                ["manual_no"] = reader["manual_no"].ToString()
                            };

                            slData.Add(vals);
                        }
                    }
                }
            }
            opening["DR"] = 0.0;
            opening["CR"] = 0.0;
            // Second query for opening balance
            string query2 = $@"
        SELECT to_date('{model.from_date}', 'DD-MM-YYYY') AS voucher_date, ' Opening Balance' AS voucher_no,
               CASE WHEN SUM(dr_amount) - SUM(cr_amount) > 0 THEN SUM(dr_amount) - SUM(cr_amount) END AS dr_amount,
               CASE WHEN SUM(cr_amount) - SUM(dr_amount) > 0 THEN SUM(cr_amount) - SUM(dr_amount) END AS cr_amount
        FROM V$VIRTUAL_SUB_LEDGER
        WHERE sub_code = '{model.sub_code}'
          AND company_code in ({formatedCompanyCodes})
          AND branch_code in ({formatedBranchCodes})
          AND deleted_flag = 'N'
          AND (form_code = '0' OR trunc(voucher_date) < '{model.from_date}')
        GROUP BY sub_code";

            using (OracleConnection objConn = new OracleConnection(tokens[1]))
            {
                objConn.Open();

                using (OracleCommand cmd = new OracleCommand(query2, objConn))
                {
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            double openingDr = reader["dr_amount"] != DBNull.Value ? Convert.ToDouble(reader["dr_amount"]) : 0.0;
                            double openingCr = reader["cr_amount"] != DBNull.Value ? Convert.ToDouble(reader["cr_amount"]) : 0.0;

                            opening["DR"] = openingDr;
                            opening["CR"] = openingCr;
                        }
                    }
                }
            }
            var openingDetails = new List<Dictionary<string, object>>();

            if (!string.IsNullOrEmpty(model.sub_code) && model.sub_code.Length > 1)
            {
                string openingQuery = "";
                string subType = model.sub_code.Substring(0, 1);
                string subCodeValue = model.sub_code.Substring(1);

                if (subType == "C")
                {
                    openingQuery = $@"
            SELECT 
                TO_CHAR(TRUNC(INVOICE_DATE)) AS INVOICE_DATE,
                BS_DATE(INVOICE_DATE) AS MITI,
                REFERENCE_NO,
                BALANCE_AMOUNT,
                (TRUNC(SYSDATE) - INVOICE_DATE) AS DUE_DATE
            FROM SA_CUSTOMER_OPENING_SETUP 
            WHERE COMPANY_CODE IN ({formatedCompanyCodes})
              AND CUSTOMER_CODE = '{subCodeValue}'
              AND DELETED_FLAG = 'N'
              AND BRANCH_CODE IN ({formatedBranchCodes})
            ORDER BY REFERENCE_NO, MITI";
                }
                else if (subType == "S")
                {
                    openingQuery = $@"
            SELECT 
                TO_CHAR(TRUNC(INVOICE_DATE)) AS INVOICE_DATE,
                BS_DATE(INVOICE_DATE) AS MITI,
                REFERENCE_NO,
                BALANCE_AMOUNT,
                (TRUNC(SYSDATE) - INVOICE_DATE) AS DUE_DATE
            FROM IP_SUPPLIER_OPENING_SETUP 
            WHERE COMPANY_CODE IN ({formatedCompanyCodes})
              AND SUPPLIER_CODE = '{subCodeValue}'
              AND DELETED_FLAG = 'N'
              AND BRANCH_CODE IN ({formatedBranchCodes})
            ORDER BY REFERENCE_NO, MITI";
                }

                if (!string.IsNullOrWhiteSpace(openingQuery))
                {
                    using (OracleConnection conn = new OracleConnection(tokens[1]))
                    {
                        conn.Open();
                        using (OracleCommand cmd = new OracleCommand(openingQuery, conn))
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var entry = new Dictionary<string, object>
                                {
                                    ["INVOICE_DATE"] = reader["INVOICE_DATE"].ToString(),
                                    ["MITI"] = reader["MITI"].ToString(),
                                    ["REFERENCE_NO"] = reader["REFERENCE_NO"].ToString(),
                                    ["BALANCE_AMOUNT"] = Convert.ToDouble(reader["BALANCE_AMOUNT"]),
                                    ["DUE_DATE"] = Convert.ToInt32(reader["DUE_DATE"])
                                };

                                openingDetails.Add(entry);
                            }
                        }
                    }
                }
            }

            opening["opening"] = openingDetails;


            /*opening["opening"] = [
                {
                "INVOICE_DATE":i[0],
                    "MITI":i[1],
                    "REFERENCE_NO":i[2],
                    "BALANCE_AMOUNT":i[3],
                    "DUE_DATE":i[4]
                }
                ]
                ;*/



            double debitTotal = drsArray.Sum();
            double creditTotal = crsArray.Sum() + Convert.ToDouble(opening["CR"]);
            double totalClosing = debitTotal - creditTotal + Convert.ToDouble(opening["DR"]);



            //string cust_code = $@"select link_sub_code from sa_customer_setup where customer_code ='{model.sub_code}'";
            var query_cust_code = model.sub_code;

            string query3 = $@"
        SELECT NVL(SUM(DR_AMOUNT)-SUM(CR_AMOUNT),0) FROM V$VIRTUAL_SUB_LEDGER 
        WHERE SUB_CODE IN('{query_cust_code}') AND COMPANY_CODE in ({formatedCompanyCodes})";

            bool for_debit_balance = false;

            // Execute the query to check the debit balance
            //string sConnStr1 = ConfigurationManager.ConnectionStrings["NeoErpCoreEntity"].ToString();
            //string[] tokens = sConnStr1.Split('"');

            using (OracleConnection objConn = new OracleConnection(tokens[1]))
            {
                objConn.Open();  // Ensure the connection is opened here

                using (OracleCommand objCmd = new OracleCommand(query3, objConn))
                {
                    OracleDataReader reader = objCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        if (reader.GetDecimal(0) > 0)
                        {
                            for_debit_balance = true;
                        }
                    }
                    reader.Close();
                }
            }
            // Query for ageing data based on debit balance
            if (for_debit_balance)
            {
                query3 = $@"
            SELECT SUM(CASE WHEN (trunc(TO_DATE('{model.Saved_Date}','MM/dd/yyyy hh24:mi:ss')) - trunc(VOUCHER_DATE)) BETWEEN 0 AND 30 THEN REAL_BALANCE ELSE 0 END) A30, 
            SUM(CASE WHEN (trunc(TO_DATE('{model.Saved_Date}','MM/dd/yyyy hh24:mi:ss')) - trunc(VOUCHER_DATE)) BETWEEN 31 AND 60 THEN REAL_BALANCE ELSE 0 END) A60, 
            SUM(CASE WHEN (trunc(TO_DATE('{model.Saved_Date}','MM/dd/yyyy hh24:mi:ss')) - trunc(VOUCHER_DATE)) BETWEEN 61 AND 90 THEN REAL_BALANCE ELSE 0 END) A90, 
            SUM(CASE WHEN (trunc(TO_DATE('{model.Saved_Date}','MM/dd/yyyy hh24:mi:ss')) - trunc(VOUCHER_DATE)) BETWEEN 91 AND 120 THEN REAL_BALANCE ELSE 0 END) A120, 
            SUM(CASE WHEN (trunc(TO_DATE('{model.Saved_Date}','MM/dd/yyyy hh24:mi:ss')) - trunc(VOUCHER_DATE)) > 120 THEN REAL_BALANCE ELSE 0 END) A121 
            FROM (
                SELECT VOUCHER_NO, VOUCHER_DATE, DR_AMOUNT, BALANCE_AMOUNT, CR_AMOUNT, 
                DECODE(ROWNUM,1,ABS(BALANCE_AMOUNT), DR_AMOUNT) REAL_BALANCE 
                FROM (
                   SELECT  VOUCHER_NO,  VOUCHER_DATE, DR_AMOUNT, CR_AMOUNT, SUM (CR_AMOUNT - DR_AMOUNT ) OVER ( ORDER BY VOUCHER_DATE, REGEXP_REPLACE(VOUCHER_NO, '[^0-9]', ''), COMPANY_CODE ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW)   BALANCE_AMOUNT FROM (  
                SELECT VOUCHER_NO,  VOUCHER_DATE, FORM_CODE, A.COMPANY_CODE, DR_AMOUNT  * NVL(EXCHANGE_RATE,1)  - NVL((SELECT SUM(NVL(REFERENCE_AMOUNT,0)) FROM  
                   FA_REFERENCE_TRANSACTION WHERE REFERENCE_NO = A.MANUAL_NO AND COMPANY_CODE = A.COMPANY_CODE AND 
                SUB_CODE = A.SUB_CODE AND DELETED_FLAG = 'N'),0)  DR_AMOUNT , DECODE( ROW_NUMBER() OVER ( ORDER BY  VOUCHER_DATE,  REGEXP_REPLACE(VOUCHER_NO, '[^0-9]', ''),COMPANY_CODE ),1,  
                (SELECT  NVL(SUM(NVL(CR_AMOUNT ,0)  * NVL(EXCHANGE_RATE,1) ),0) TOTALPAIDAMT FROM V$VIRTUAL_SUB_LEDGER  
                WHERE DELETED_FLAG = 'N'  
                AND COMPANY_CODE in ({formatedCompanyCodes})
                AND BRANCH_CODE IN ({formatedBranchCodes})
                AND TO_DATE(VOUCHER_DATE) <= TO_DATE('{model.Saved_Date}','MM/dd/yyyy hh24:mi:ss')  
                AND SUB_CODE IN('{query_cust_code}')  
                AND (VOUCHER_NO, SUB_CODE, COMPANY_CODE) NOT IN (SELECT DISTINCT NVL(VOUCHER_NO,'A000000'),SUB_CODE, COMPANY_CODE FROM FA_REFERENCE_TRANSACTION WHERE 
                COMPANY_CODE in ({formatedCompanyCodes}) AND DELETED_FLAG = 'N' AND SUB_CODE IN('{query_cust_code}') )),0) CR_AMOUNT FROM V$VIRTUAL_SUB_LEDGER A  
                WHERE TO_DATE(VOUCHER_DATE) <= TO_DATE('{model.Saved_Date}','MM/dd/yyyy hh24:mi:ss')  
                AND A.DELETED_FLAG='N'  
                AND COMPANY_CODE in ({formatedCompanyCodes})
                AND BRANCH_CODE IN ({formatedBranchCodes})  and 
                SUB_CODE IN('{query_cust_code}')  
                ORDER BY VOUCHER_DATE, VOUCHER_DATE, REGEXP_REPLACE(VOUCHER_NO, '[^0-9]', '')  
                ) A
            ) WHERE BALANCE_AMOUNT < 0)";
            }
            else
            {
                query3 = $@"
            SELECT SUM(CASE WHEN (trunc(TO_DATE('{model.Saved_Date}','MM/dd/yyyy hh24:mi:ss')) - trunc(VOUCHER_DATE)) BETWEEN 0 AND 30 THEN REAL_BALANCE ELSE 0 END) A30, 
            SUM(CASE WHEN (trunc(TO_DATE('{model.Saved_Date}','MM/dd/yyyy hh24:mi:ss')) - trunc(VOUCHER_DATE)) BETWEEN 31 AND 60 THEN REAL_BALANCE ELSE 0 END) A60, 
            SUM(CASE WHEN (trunc(TO_DATE('{model.Saved_Date}','MM/dd/yyyy hh24:mi:ss')) - trunc(VOUCHER_DATE)) BETWEEN 61 AND 90 THEN REAL_BALANCE ELSE 0 END) A90, 
            SUM(CASE WHEN (trunc(TO_DATE('{model.Saved_Date}','MM/dd/yyyy hh24:mi:ss')) - trunc(VOUCHER_DATE)) BETWEEN 91 AND 120 THEN REAL_BALANCE ELSE 0 END) A120, 
            SUM(CASE WHEN (trunc(TO_DATE('{model.Saved_Date}','MM/dd/yyyy hh24:mi:ss')) - trunc(VOUCHER_DATE)) > 120 THEN REAL_BALANCE ELSE 0 END) A121 
            FROM (
                 SELECT  VOUCHER_NO,  VOUCHER_DATE ,  CR_AMOUNT , BALANCE_AMOUNT, DR_AMOUNT, DECODE(ROWNUM,1,ABS(BALANCE_AMOUNT), CR_AMOUNT) REAL_BALANCE 
                FROM (
                    SELECT  VOUCHER_NO,  VOUCHER_DATE, CR_AMOUNT, DR_AMOUNT, SUM (DR_AMOUNT - CR_AMOUNT ) OVER ( ORDER BY VOUCHER_DATE, REGEXP_REPLACE(VOUCHER_NO, '[^0-9]', ''), 
                    COMPANY_CODE ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW)   BALANCE_AMOUNT
                    FROM (  
                    SELECT VOUCHER_NO,  VOUCHER_DATE, FORM_CODE, A.COMPANY_CODE
                    , CR_AMOUNT  * NVL(EXCHANGE_RATE,1) CR_AMOUNT , DECODE( ROW_NUMBER() OVER
                    ( ORDER BY  VOUCHER_DATE,  REGEXP_REPLACE(VOUCHER_NO, '[^0-9]', ''),COMPANY_CODE ),1,  
                    (
                    SELECT  NVL(SUM(NVL(DR_AMOUNT ,0)  * NVL(EXCHANGE_RATE,1) ),0) TOTALPAIDAMT FROM V$VIRTUAL_SUB_LEDGER  
                    WHERE DELETED_FLAG = 'N'
                    AND COMPANY_CODE in ({formatedCompanyCodes})
                    AND BRANCH_CODE IN ({formatedBranchCodes})  AND TO_DATE(VOUCHER_DATE) <= TO_DATE('{model.Saved_Date}','MM/dd/yyyy hh24:mi:ss')
                    AND SUB_CODE IN('{query_cust_code}')  
                    ),0) DR_AMOUNT FROM V$VIRTUAL_SUB_LEDGER A  
                    WHERE TO_DATE(VOUCHER_DATE) <= TO_DATE('{model.Saved_Date}','MM/dd/yyyy hh24:mi:ss')
                    AND A.DELETED_FLAG='N'  
                    AND COMPANY_CODE in ({formatedCompanyCodes})
                    AND BRANCH_CODE IN ({formatedBranchCodes})  AND SUB_CODE IN('{query_cust_code}')  
                    ORDER BY VOUCHER_DATE, VOUCHER_DATE, REGEXP_REPLACE(VOUCHER_NO, '[^0-9]', '')  
                ) A
            ) WHERE BALANCE_AMOUNT < 0)";
            }

            // Execute the query to get ageing data
            //Dictionary<string, float> vals = new Dictionary<string, float>();
            using (OracleConnection objConn = new OracleConnection(tokens[1]))
            {
                objConn.Open();  // Ensure the connection is opened here

                using (OracleCommand objCmd = new OracleCommand(query3, objConn))
                {
                    OracleDataReader reader = objCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        float d0 = reader.IsDBNull(0) ? 0 : reader.GetFloat(0);
                        float d1 = reader.IsDBNull(1) ? 0 : reader.GetFloat(1);
                        float d2 = reader.IsDBNull(2) ? 0 : reader.GetFloat(2);
                        float d3 = reader.IsDBNull(3) ? 0 : reader.GetFloat(3);
                        float d4 = reader.IsDBNull(4) ? 0 : reader.GetFloat(4);

                        ageingVals["0-30"] = d0;
                        ageingVals["30-60"] = d1;
                        ageingVals["61-90"] = d2;
                        ageingVals["91-120"] = d3;
                        ageingVals["120++"] = d4;
                        ageingVals["total"] = d0 + d1 + d2 + d3 + d4;
                    }
                    reader.Close();
                }
            }



            var transaction = new
            {
                transactions = slData,
                debit_total = debitTotal,
                credit_total = creditTotal,
                opening = opening,
                closing_balance = totalClosing,
                ageingReport = ageingVals
                // You can extend this to include ageing_report, company_info, etc.
            };

            return transaction;
        }
        /*sashi Subledger module*/
        public List<MoveTransactionResponseModel> FetchMovementTransactions(TransactionRequestModel model, NeoErpCoreEntity dbContext)
        {
            var result = new List<MoveTransactionResponseModel>();
            DateTime fromDate, toDate;
            if (!DateTime.TryParseExact(model.from_date, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDate) || !DateTime.TryParseExact(model.to_date, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out toDate))
                throw new Exception("Invalid Date");

            //opening balance
            string OpeningQuery = string.Empty;
            string NonOpeningQuery = string.Empty;
            //model.COMPANY_CODE = model.COMPANY_CODE.Replace(" ", string.Empty);
            //model.COMPANY_CODE = model.COMPANY_CODE.Replace(",", "','");
            model.BRANCH_CODE = model.BRANCH_CODE.Replace(" ", string.Empty);
            model.BRANCH_CODE = model.BRANCH_CODE.Replace(",", "','");
            if (string.IsNullOrWhiteSpace(model.acc_code))
                throw new Exception("No records found");

            //var query = $@"SELECT DISTINCT SUB_CODE,SUB_EDESC, VOUCHER_NO,MANUAL_NO,TO_CHAR(VOUCHER_DATE) AS VOUCHER_DATE, CREDIT_LIMIT, CREDIT_DAYS, VOUCHER_DATE+CREDIT_DAYS DUE_DATE,
            //                 COALESCE( SUM(DR_AMOUNT - CR_AMOUNT) OVER (PARTITION BY SUB_EDESC ORDER BY VOUCHER_DATE
            //                     RANGE BETWEEN UNBOUNDED PRECEDING AND 1 PRECEDING), 0 )       AS OP_BAL,
            //                 SUM(DR_AMOUNT) OVER (PARTITION BY SUB_EDESC, VOUCHER_DATE)                  AS DAILY_DR,
            //                 SUM(CR_AMOUNT) OVER (PARTITION BY SUB_EDESC, VOUCHER_DATE)                  AS DAILY_CR,
            //                 SUM(DR_AMOUNT - CR_AMOUNT) OVER (PARTITION BY SUB_EDESC ORDER BY VOUCHER_DATE) AS CL_BAL
            //            FROM     V$VIRTUAL_SUB_DEALER_LEDGER
            //        WHERE 1=1
            //        AND COMPANY_CODE IN ('{model.COMPANY_CODE}')
            //        AND  TRUNC(VOUCHER_DATE) BETWEEN TO_DATE('{fromDate.ToString("dd-MMM-yyyy")}','DD-MON-RRRR') AND TO_DATE('{toDate.ToString("dd-MMM-yyyy")}','DD-MON-RRRR')
            //        AND SUB_CODE in (select distinct link_sub_code from sa_customer_setup  where deleted_flag='N' and company_code in ('{model.COMPANY_CODE}') and deleted_flag='N' and group_sku_flag='I'  and customer_code='{model.acc_code}')
            //        ORDER BY SUB_EDESC, VOUCHER_DATE";
            //var query = $@"SELECT * FROM M$V_MOVEMENT_ANALYSIS WHERE 'C'|| CUSTOMER_CODE ='{model.acc_code}' AND COMPANY_CODE IN('{model.COMPANY_CODE}')";
            //var query1 = $@"SELECT VOUCHER_NO,VOUCHER_DATE,CREDIT_LIMIT,CREDIT_DAYS,DUE_DAYS,SALES_AMT,REC_AMT,BALANCE FROM M$V_MOVEMENT_ANALYSIS WHERE CUSTOMER_CODE='2836'";
            //var query2 = $@"SELECT CUSTOMER_EDESC,VOUCHER_NO,VOUCHER_DATE,CREDIT_LIMIT,CREDIT_DAYS,DUE_DAYS,SALES_AMT,REC_AMT,BALANCE FROM M$V_MOVEMENT_ANALYSIS WHERE 'C' || CUSTOMER_CODE ='{model.sub_code}' AND COMPANY_CODE IN('{model.COMPANY_CODE}')";

            string sConnStr1 = ConfigurationManager.ConnectionStrings["NeoErpCoreEntity"].ToString();
            string[] tokens = sConnStr1.Split('"');

            using (OracleConnection objConn = new OracleConnection(tokens[1]))
            {
                objConn.Open();  // Ensure the connection is opened here

                using (OracleCommand objCmd = new OracleCommand("DIST_MOVEMENT_ANALYSIS", objConn))
                {
                    objCmd.CommandType = CommandType.StoredProcedure;
                    objCmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("p_fromDate", OracleDbType.Varchar2).Value = fromDate.ToString("dd-MMM-yyyy");
                    objCmd.Parameters.Add("p_toDate", OracleDbType.Varchar2).Value = toDate.ToString("dd-MMM-yyyy");
                    objCmd.Parameters.Add("p_company_codes", OracleDbType.Varchar2).Value = model.COMPANY_CODE;
                    objCmd.Parameters.Add("p_sub_code", OracleDbType.Varchar2).Value = model.sub_code;

                    using (OracleDataReader reader = objCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MoveTransactionResponseModel item = new MoveTransactionResponseModel
                            {
                                CUSTOMER_EDESC = reader.GetString(reader.GetOrdinal("CUSTOMER_EDESC")),
                                VOUCHER_NO = reader.GetString(reader.GetOrdinal("VOUCHER_NO")),
                                MANUAL_NO = reader.GetString(reader.GetOrdinal("MANUAL_NO")),
                                DR_AMOUNT = reader.GetDecimal(reader.GetOrdinal("VSL_DR_AMOUNT")),
                                CR_AMOUNT = reader.GetDecimal(reader.GetOrdinal("VSL_CR_AMOUNT")),
                                VOUCHER_DATE = reader.GetString(reader.GetOrdinal("VOUCHER_DATE")),
                                CREDIT_DAYS = reader.GetString(reader.GetOrdinal("CREDIT_DAYS")),
                                CREDIT_LIMIT = reader.GetString(reader.GetOrdinal("CREDIT_LIMIT")),
                                SALES_AMT = reader.GetString(reader.GetOrdinal("SALES_AMT")),
                                SUB_CODE = reader.GetString(reader.GetOrdinal("SUB_CODE")),
                                BALANCE = reader.GetString(reader.GetOrdinal("BALANCE")),
                                REC_AMT = reader.GetString(reader.GetOrdinal("REC_AMT"))
                            };

                            result.Add(item);
                        }
                    }
                }
            }
            if (result.Count <= 0)
            {
                throw new Exception("No records found");
            }
            return result;
        }

        //public List<MoveTransactionResponseModel> FetchMovementTransactions(TransactionRequestModel model, NeoErpCoreEntity dbContext)
        //{
        //    var result = new List<MoveTransactionResponseModel>();
        //    DateTime fromDate, toDate;
        //    if (!DateTime.TryParseExact(model.from_date, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDate) || !DateTime.TryParseExact(model.to_date, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out toDate))
        //        throw new Exception("Invalid Date");



        //    //opening balance
        //    string OpeningQuery = string.Empty;
        //    string NonOpeningQuery = string.Empty;
        //    model.COMPANY_CODE = model.COMPANY_CODE.Replace(" ", string.Empty);
        //    model.COMPANY_CODE = model.COMPANY_CODE.Replace(",", "','");
        //    model.BRANCH_CODE = model.BRANCH_CODE.Replace(" ", string.Empty);
        //    model.BRANCH_CODE = model.BRANCH_CODE.Replace(",", "','");
        //    if (string.IsNullOrWhiteSpace(model.acc_code))
        //        throw new Exception("No records found");

        //    //var query = $@"SELECT DISTINCT SUB_CODE,SUB_EDESC, VOUCHER_NO,MANUAL_NO,TO_CHAR(VOUCHER_DATE) AS VOUCHER_DATE, CREDIT_LIMIT, CREDIT_DAYS, VOUCHER_DATE+CREDIT_DAYS DUE_DATE,
        //    //                 COALESCE( SUM(DR_AMOUNT - CR_AMOUNT) OVER (PARTITION BY SUB_EDESC ORDER BY VOUCHER_DATE
        //    //                     RANGE BETWEEN UNBOUNDED PRECEDING AND 1 PRECEDING), 0 )       AS OP_BAL,
        //    //                 SUM(DR_AMOUNT) OVER (PARTITION BY SUB_EDESC, VOUCHER_DATE)                  AS DAILY_DR,
        //    //                 SUM(CR_AMOUNT) OVER (PARTITION BY SUB_EDESC, VOUCHER_DATE)                  AS DAILY_CR,
        //    //                 SUM(DR_AMOUNT - CR_AMOUNT) OVER (PARTITION BY SUB_EDESC ORDER BY VOUCHER_DATE) AS CL_BAL
        //    //            FROM     V$VIRTUAL_SUB_DEALER_LEDGER
        //    //        WHERE 1=1
        //    //        AND COMPANY_CODE IN ('{model.COMPANY_CODE}')
        //    //        AND  TRUNC(VOUCHER_DATE) BETWEEN TO_DATE('{fromDate.ToString("dd-MMM-yyyy")}','DD-MON-RRRR') AND TO_DATE('{toDate.ToString("dd-MMM-yyyy")}','DD-MON-RRRR')
        //    //        AND SUB_CODE in (select distinct link_sub_code from sa_customer_setup  where deleted_flag='N' and company_code in ('{model.COMPANY_CODE}') and deleted_flag='N' and group_sku_flag='I'  and customer_code='{model.acc_code}')
        //    //        ORDER BY SUB_EDESC, VOUCHER_DATE";
        //    //var query = $@"SELECT * FROM M$V_MOVEMENT_ANALYSIS WHERE 'C'|| CUSTOMER_CODE ='{model.acc_code}' AND COMPANY_CODE IN('{model.COMPANY_CODE}')";
        //    //var query1 = $@"SELECT VOUCHER_NO,VOUCHER_DATE,CREDIT_LIMIT,CREDIT_DAYS,DUE_DAYS,SALES_AMT,REC_AMT,BALANCE FROM M$V_MOVEMENT_ANALYSIS WHERE CUSTOMER_CODE='2836'";
        //    var query1 = $@"SELECT CUSTOMER_EDESC,VOUCHER_NO,VOUCHER_DATE,CREDIT_LIMIT,CREDIT_DAYS,DUE_DAYS,SALES_AMT,REC_AMT,BALANCE FROM M$V_MOVEMENT_ANALYSIS WHERE 'C' || CUSTOMER_CODE ='{model.sub_code}' AND COMPANY_CODE IN('{model.COMPANY_CODE}')";
        //    result = dbContext.SqlQuery<MoveTransactionResponseModel>(query1).ToList();




        //    if (result.Count <= 0)
        //        throw new Exception("No records found");
        //    return result;
        //}

        public Dictionary<string, List<PurchaseOrderResponseModel>> FetchPurchaseOrder(PurchaseOrderRequestModel model, NeoErpCoreEntity dbContext)
        {
            var result = new Dictionary<string, List<PurchaseOrderResponseModel>>();
            string PurchaseQuery = string.Empty;
            string DateQuery = string.Empty;

            DateQuery = $@"select FY_START_DATE from PREFERENCE_SETUP where company_code = '{model.COMPANY_CODE}'";
            var startDate = dbContext.SqlQuery<DateTime>(DateQuery).FirstOrDefault();
            var endDate = DateTime.Now;
            if (model.type.Equals("distributor", StringComparison.OrdinalIgnoreCase))
                PurchaseQuery = $@"select a.distributor_code as code,TO_CHAR(c.order_no) order_no, to_char(c.order_date, 'DD-MON-YYYY') as order_date, c.item_code, b.item_edesc, c.mu_code,
             TO_CHAR(c.quantity) quantity, TO_CHAR(c.unit_price) unit_price,TO_CHAR(c.total_price) total_price, c.remarks, c.approved_flag, c.dispatch_flag, c.acknowledge_flag,
             c.reject_flag, TO_CHAR(NVL(IR.SALES_RATE, 0)) SALES_RATE, To_CHAR(IR.APPLY_DATE) APPLY_DATE
                    from dist_distributor_master a, ip_item_master_setup b, dist_ip_ssd_purchase_order c,
                    (SELECT A.ITEM_CODE, A.APPLY_DATE, B.SALES_RATE, B.COMPANY_CODE
                              FROM (SELECT ITEM_CODE, COMPANY_CODE, MAX(APP_DATE) APPLY_DATE 
                                FROM IP_ITEM_RATE_APPLICAT_SETUP
                                WHERE COMPANY_CODE = '{model.COMPANY_CODE}' 
                                AND BRANCH_CODE = '{model.BRANCH_CODE}'
                                GROUP BY ITEM_CODE, COMPANY_CODE) A
                              INNER JOIN IP_ITEM_RATE_APPLICAT_SETUP B
                                ON B.ITEM_CODE = A.ITEM_CODE
                                AND B.APP_DATE = A.APPLY_DATE
                                AND B.COMPANY_CODE = '{model.COMPANY_CODE}'
                                AND B.BRANCH_CODE = '{model.BRANCH_CODE}') IR
                    where a.distributor_code = '{model.code}'
                    and a.distributor_code = c.customer_code
                    and b.item_code = c.item_code
                    and b.group_sku_flag='{GROUP_SKU_FLAG}'
                    and b.category_code='{CATEGORY_CODE}'
                    and TRUNC(c.order_date) between TO_DATE('{startDate.ToString("dd-MMM-yyyy")}','DD-MON-RRRR') and TO_DATE('{endDate.ToString("dd-MMM-yyyy")}','DD-MON-RRRR')
                    and a.company_code='{model.COMPANY_CODE}'
                    and a.company_code=b.company_code
                    and b.company_code=c.company_code
                    AND IR.ITEM_CODE(+) = c.ITEM_CODE AND IR.COMPANY_CODE(+) = c.COMPANY_CODE
                    order by c.order_no DESC";
            else if (model.type.Equals("reseller", StringComparison.OrdinalIgnoreCase))
                PurchaseQuery = $@"select a.distributor_code as code,TO_CHAR(c.order_no) order_no, to_char(c.order_date, 'DD-MON-YYYY') as order_date, c.item_code, b.item_edesc, c.mu_code,
             TO_CHAR(c.quantity) quantity, TO_CHAR(c.unit_price) unit_price,TO_CHAR(c.total_price) total_price, c.remarks, c.approved_flag, c.dispatch_flag, c.acknowledge_flag,
             c.reject_flag, TO_CHAR(NVL(IR.SALES_RATE, 0)) SALES_RATE, To_CHAR(IR.APPLY_DATE) APPLY_DATE
                    from dist_reseller_master a, ip_item_master_setup b, dist_ip_ssr_purchase_order c,
                    (SELECT A.ITEM_CODE, A.APPLY_DATE, B.SALES_RATE, B.COMPANY_CODE
                              FROM (SELECT ITEM_CODE, COMPANY_CODE, MAX(APP_DATE) APPLY_DATE 
                                FROM IP_ITEM_RATE_APPLICAT_SETUP
                                WHERE COMPANY_CODE = '{model.COMPANY_CODE}' 
                                AND BRANCH_CODE = '{model.BRANCH_CODE}'
                                GROUP BY ITEM_CODE, COMPANY_CODE) A
                              INNER JOIN IP_ITEM_RATE_APPLICAT_SETUP B
                                ON B.ITEM_CODE = A.ITEM_CODE
                                AND B.APP_DATE = A.APPLY_DATE
                                AND B.COMPANY_CODE = '{model.COMPANY_CODE}'
                                AND B.BRANCH_CODE = '{model.BRANCH_CODE}') IR
                    where a.reseller_code = '{model.code}'
                    and a.reseller_code = c.reseller_code
                    and b.item_code = c.item_code
                    and b.group_sku_flag='{GROUP_SKU_FLAG}'
                    and b.category_code='{CATEGORY_CODE}'
                    and TRUNC(c.order_date) between TO_DATE('{startDate.ToString("dd-MMM-yyyy")}','DD-MON-RRRR') and TO_DATE('{endDate.ToString("dd-MMM-yyyy")}','DD-MON-RRRR')
                    and a.company_code='{model.COMPANY_CODE}'
                    and a.company_code=b.company_code
                    and b.company_code=c.company_code
                    AND IR.ITEM_CODE(+) = c.ITEM_CODE AND IR.COMPANY_CODE(+) = c.COMPANY_CODE
                    order by c.order_no DESC";
            else
                throw new Exception("Type Not specified");

            var data = dbContext.SqlQuery<PurchaseOrderResponseModel>(PurchaseQuery).ToList();
            if (data.Count <= 0)
                throw new Exception("No records found");
            var groups = data.GroupBy(x => x.ORDER_NO);
            foreach (var group in groups)
            {
                result.Add(group.Key, group.ToList());
            }
            //foreach (var item in data)
            //{
            //    var list = new List<PurchaseOrderResponseModel>();
            //    list.Add(item);
            //    result.Add(item.ORDER_NO, list);
            //}
            return result;
        }

        public SalesAgeReportResponseModel SalesAgingReport(ReportRequestModel model, NeoErpCoreEntity dbContext)
        {
            var result = new SalesAgeReportResponseModel();
            result.sales = this.MonthWiseSales(model, dbContext);
            result.age = this.AgingReport(model, dbContext);
            return result;
        }

        public Dictionary<string, string> MonthWiseSales(ReportRequestModel model, NeoErpCoreEntity dbContext)
        {
            if (string.IsNullOrWhiteSpace(model.code) || string.IsNullOrEmpty(model.code))
                throw new Exception("Code cannot be empty");

            var result = new Dictionary<string, string>();
            var SalesQuery = string.Empty;
            SalesQuery = $@"SELECT 
                  UPPER(trim(substr(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SI.sales_date),6,2)),5,20))) MONTH, 
                  TO_CHAR(NVL(SUM(NVL(SI.CALC_TOTAL_PRICE,0))/1,0)) AS AMOUNT,
                  TO_CHAR(SUM(NVL(SI.CALC_QUANTITY,0))/'1') AS QUANTITY,
                  TO_CHAR(SUBSTR(BS_DATE(SI.SALES_DATE),1,7)) AS MONTHINT
                FROM SA_SALES_INVOICE SI
                WHERE 1 = 1
                  AND  SI.DELETED_FLAG = 'N'
                  AND SI.COMPANY_CODE IN ('{model.COMPANY_CODE}')
                  AND SI.CUSTOMER_CODE = '{model.code}'
              GROUP BY FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SI.SALES_DATE),6,2)),SUBSTR(BS_DATE(SI.SALES_DATE),1,7)
              ORDER BY SUBSTR(BS_DATE(SI.SALES_DATE),1,7)";
            var data = dbContext.SqlQuery<SalesReportResponseModel>(SalesQuery).ToList();
            string[] Months = { "BAISAKH", "JESTHA", "ASHADH", "SHRAWAN", "BHADRA", "ASHOJ", "KARTIK", "MANGSIR", "POUSH", "MAGH", "FALGUN", "CHAITRA" };

            decimal Netsales = 0;
            foreach (var m in Months)
            {
                decimal amount = 0;
                var monthData = data.FirstOrDefault(x => x.MONTH == m);
                if (monthData == null)
                    result.Add(m, "0");
                else
                {
                    decimal.TryParse(monthData.AMOUNT, out amount);
                    Netsales += amount;
                    result.Add(m, monthData.AMOUNT);
                }
            }
            result.Add("NETSALES", Netsales.ToString());
            return result;
        }

        public Dictionary<string, string> AgingReport(ReportRequestModel model, NeoErpCoreEntity dbContext)
        {
            model.code = model.code.FirstOrDefault() == 'C' ? model.code : "C" + model.code;
            var result = new Dictionary<string, string>();
            string AgingQuery = string.Empty;
            var Dates = new List<AgingDateRange>();
            Dates.Add(new AgingDateRange { StartDate = DateTime.Now.AddDays(-30), EndDate = DateTime.Now });
            for (int i = 1; i < 5; i++)
            {
                var date = new AgingDateRange();
                date.StartDate = Dates[i - 1].StartDate.AddDays(-30);
                date.EndDate = Dates[i - 1].StartDate.AddDays(-1);
                Dates.Add(date);
            }
            AgingQuery += "SELECT DISTINCT A.SUB_CODE, ";
            for (int i = 5; i >= 0; i--)
            {
                if (i == 5)
                    AgingQuery += $@"
                    SUM((SELECT SUM(ROUND(NVL(C.CR_AMOUNT,0),2)) FROM V$CUSTOMER_BILLAGE_LEDGER C
                    WHERE C.COMPANY_CODE = A.COMPANY_CODE AND C.BRANCH_CODE = A.BRANCH_CODE AND C.SERIAL_NO = A.SERIAL_NO AND C.SUB_CODE = A.SUB_CODE
                    AND C.ACC_CODE = A.ACC_CODE AND C.FORM_CODE = A.FORM_CODE AND C.VOUCHER_NO = A.VOUCHER_NO
                    AND TO_DATE(TO_CHAR(C.VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}')
                    AND C.COMPANY_CODE IN ('{model.COMPANY_CODE}') )) CR_AMOUNT, ";
                else
                {
                    if (i == 4)
                        AgingQuery += $@"
                    SUM((SELECT SUM(ROUND(NVL(C.DR_AMOUNT,0),2)) FROM V$CUSTOMER_BILLAGE_LEDGER C WHERE C.COMPANY_CODE = A.COMPANY_CODE
                    AND C.BRANCH_CODE = A.BRANCH_CODE AND C.SERIAL_NO = A.SERIAL_NO AND C.SUB_CODE = A.SUB_CODE AND C.ACC_CODE = A.ACC_CODE
                    AND C.FORM_CODE = A.FORM_CODE AND C.VOUCHER_NO = A.VOUCHER_NO 
                    AND TO_DATE(TO_CHAR(C.VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{Dates[i].EndDate.ToString("dd-MMM-yyyy")}')
                    AND C.COMPANY_CODE IN ('{model.COMPANY_CODE}')  )) DR_AMOUNT{i}, ";
                    else
                    {
                        AgingQuery += $@"
                    SUM((SELECT SUM(ROUND(NVL(C.DR_AMOUNT,0),2)) FROM V$CUSTOMER_BILLAGE_LEDGER C WHERE C.COMPANY_CODE = A.COMPANY_CODE
                    AND C.BRANCH_CODE = A.BRANCH_CODE AND C.SERIAL_NO = A.SERIAL_NO AND C.SUB_CODE = A.SUB_CODE AND C.ACC_CODE = A.ACC_CODE
                    AND C.FORM_CODE = A.FORM_CODE AND C.VOUCHER_NO = A.VOUCHER_NO AND TO_DATE(TO_CHAR(C.VOUCHER_DATE,'DD-MON-YYYY')) >= TO_DATE('{Dates[i].StartDate.ToString("dd-MMM-yyyy")}')
                    AND TO_DATE(TO_CHAR(C.VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{Dates[i].EndDate.ToString("dd-MMM-yyyy")}')
                    AND C.COMPANY_CODE IN ('{model.COMPANY_CODE}')   )) DR_AMOUNT";
                        AgingQuery += i;
                        if (i != 0)
                            AgingQuery += ", ";
                    }
                }
            }
            AgingQuery += $@"
            FROM V$CUSTOMER_BILLAGE_LEDGER A
            WHERE A.DELETED_FLAG='N' AND A.SUB_CODE ='{model.code}'
            AND TO_DATE(TO_CHAR(A.VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}')
            GROUP BY A.SUB_CODE";
            var data = dbContext.SqlQuery<AgingReportModel>(AgingQuery).FirstOrDefault();

            result.Add("SUB_CODE", model.code);
            result.Add("120+", "0");
            result.Add("91-120", "0");
            result.Add("61-90", "0");
            result.Add("31-60", "0");
            result.Add("0-30", "0");
            result.Add("total", "0");

            if (data != null)
            {
                result["SUB_CODE"] = data.SUB_CODE;
                decimal total = 0;
                var CRAmount = data.CR_AMOUNT == null ? 0 : data.CR_AMOUNT.Value;
                //DR_AMOUNT4
                if (data.DR_AMOUNT4 != null)
                {
                    if (CRAmount >= data.DR_AMOUNT4)
                        CRAmount = CRAmount - data.DR_AMOUNT4.Value;
                    else
                    {
                        result["120+"] = (data.DR_AMOUNT4 - CRAmount).ToString();
                        total += data.DR_AMOUNT4.Value - CRAmount;
                        CRAmount = 0;
                    }
                }
                //DR_AMOUNT3
                if (data.DR_AMOUNT3 != null)
                {
                    if (CRAmount >= data.DR_AMOUNT3)
                        CRAmount = CRAmount - data.DR_AMOUNT3.Value;
                    else
                    {
                        result["91-120"] = (data.DR_AMOUNT3 - CRAmount).ToString();
                        total += data.DR_AMOUNT3.Value - CRAmount;
                        CRAmount = 0;
                    }
                }
                //DR_AMOUNT2
                if (data.DR_AMOUNT2 != null)
                {
                    if (CRAmount >= data.DR_AMOUNT2)
                        CRAmount = CRAmount - data.DR_AMOUNT2.Value;
                    else
                    {
                        result["61-90"] = (data.DR_AMOUNT2 - CRAmount).ToString();
                        total += data.DR_AMOUNT2.Value - CRAmount;
                        CRAmount = 0;
                    }
                }
                //DR_AMOUNT1
                if (data.DR_AMOUNT1 != null)
                {
                    if (CRAmount >= data.DR_AMOUNT1)
                        CRAmount = CRAmount - data.DR_AMOUNT1.Value;
                    else
                    {
                        result["31-60"] = (data.DR_AMOUNT1 - CRAmount).ToString();
                        total += data.DR_AMOUNT1.Value - CRAmount;
                        CRAmount = 0;
                    }
                }
                //DR_AMOUNT0
                if (data.DR_AMOUNT0 != null)
                {
                    if (CRAmount >= data.DR_AMOUNT0)
                        CRAmount = CRAmount - data.DR_AMOUNT0.Value;
                    else
                    {
                        result["0-30"] = (data.DR_AMOUNT0 - CRAmount).ToString();
                        total += data.DR_AMOUNT0.Value - CRAmount;
                        CRAmount = 0;
                    }
                }
                result["total"] = total.ToString();
            }
            return result;
        }

        public List<Dictionary<string, string>> AgingReportGroup(ReportRequestModel model, NeoErpCoreEntity dbContext)
        {
            //model.code = model.code.FirstOrDefault() == 'C' ? model.code : "C" + model.code;
            var listResult = new List<Dictionary<string, string>>();
            string AgingQuery = string.Empty;
            var Dates = new List<AgingDateRange>();
            Dates.Add(new AgingDateRange { StartDate = DateTime.Now.AddDays(-30), EndDate = DateTime.Now });
            for (int i = 1; i < 4; i++)
            {
                var date = new AgingDateRange();
                date.StartDate = Dates[i - 1].StartDate.AddDays(-30);
                date.EndDate = Dates[i - 1].StartDate.AddDays(-1);
                Dates.Add(date);
            }
            AgingQuery += "SELECT DISTINCT A.SUB_CODE,A.SUB_EDESC,";
            for (int i = 4; i >= 0; i--)
            {
                if (i == 4)
                    AgingQuery += $@"
                    SUM((SELECT SUM(ROUND(NVL(C.CR_AMOUNT,0),2)) FROM V$CUSTOMER_BILLAGE_LEDGER C
                    WHERE C.COMPANY_CODE = A.COMPANY_CODE AND C.BRANCH_CODE = A.BRANCH_CODE AND C.SERIAL_NO = A.SERIAL_NO AND C.SUB_CODE = A.SUB_CODE
                    AND C.ACC_CODE = A.ACC_CODE AND C.FORM_CODE = A.FORM_CODE AND C.VOUCHER_NO = A.VOUCHER_NO
                    AND TO_DATE(TO_CHAR(C.VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}')
                    AND C.COMPANY_CODE='{model.COMPANY_CODE}'  AND C.BRANCH_CODE='{model.BRANCH_CODE}' )) CR_AMOUNT, ";
                else
                {
                    if (i == 3)
                        AgingQuery += $@"
                    SUM((SELECT SUM(ROUND(NVL(C.DR_AMOUNT,0),2)) FROM V$CUSTOMER_BILLAGE_LEDGER C WHERE C.COMPANY_CODE = A.COMPANY_CODE
                    AND C.BRANCH_CODE = A.BRANCH_CODE AND C.SERIAL_NO = A.SERIAL_NO AND C.SUB_CODE = A.SUB_CODE AND C.ACC_CODE = A.ACC_CODE
                    AND C.FORM_CODE = A.FORM_CODE AND C.VOUCHER_NO = A.VOUCHER_NO 
                    AND TO_DATE(TO_CHAR(C.VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{Dates[i].EndDate.ToString("dd-MMM-yyyy")}')
                    AND C.COMPANY_CODE='{model.COMPANY_CODE}' AND C.BRANCH_CODE='{model.BRANCH_CODE}' )) DR_AMOUNT{i}, ";
                    else
                    {
                        AgingQuery += $@"
                    SUM((SELECT SUM(ROUND(NVL(C.DR_AMOUNT,0),2)) FROM V$CUSTOMER_BILLAGE_LEDGER C WHERE C.COMPANY_CODE = A.COMPANY_CODE
                    AND C.BRANCH_CODE = A.BRANCH_CODE AND C.SERIAL_NO = A.SERIAL_NO AND C.SUB_CODE = A.SUB_CODE AND C.ACC_CODE = A.ACC_CODE
                    AND C.FORM_CODE = A.FORM_CODE AND C.VOUCHER_NO = A.VOUCHER_NO AND TO_DATE(TO_CHAR(C.VOUCHER_DATE,'DD-MON-YYYY')) >= TO_DATE('{Dates[i].StartDate.ToString("dd-MMM-yyyy")}')
                    AND TO_DATE(TO_CHAR(C.VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{Dates[i].EndDate.ToString("dd-MMM-yyyy")}')
                    AND C.COMPANY_CODE='{model.COMPANY_CODE}'  AND C.BRANCH_CODE='{model.BRANCH_CODE}' )) DR_AMOUNT";
                        AgingQuery += i;
                        if (i != 0)
                            AgingQuery += ", ";
                    }
                }
            }
            AgingQuery += $@"
            FROM V$CUSTOMER_BILLAGE_LEDGER A
            WHERE A.DELETED_FLAG='N' AND A.SUB_CODE IN (SELECT SUB_CODE FROM  FA_SUB_LEDGER_DEALER_MAP WHERE PARTY_TYPE_CODE='{model.party_type_code}')
            AND TO_DATE(TO_CHAR(A.VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}')
            GROUP BY A.SUB_CODE,A.SUB_EDESC";
            var AllData = dbContext.SqlQuery<AgingReportModel>(AgingQuery).ToList();


            if (AllData.Count > 0)
            {
                foreach (var data in AllData)
                {
                    var result = new Dictionary<string, string>();
                    result.Add("SUB_CODE", data.SUB_CODE);
                    result.Add("SUB_EDESC", data.SUB_EDESC);
                    //result.Add("120+", "0");
                    result.Add("90+", "0");
                    result.Add("61-90", "0");
                    result.Add("31-60", "0");
                    result.Add("0-30", "0");
                    result.Add("total", "0");
                    decimal total = 0;
                    var CRAmount = data.CR_AMOUNT == null ? 0 : data.CR_AMOUNT.Value;
                    //DR_AMOUNT4
                    //if (data.DR_AMOUNT4 != null)
                    //{
                    //    if (CRAmount >= data.DR_AMOUNT4)
                    //        CRAmount = CRAmount - data.DR_AMOUNT4.Value;
                    //    else
                    //    {
                    //        result["120+"] = (data.DR_AMOUNT4 - CRAmount).ToString();
                    //        total += data.DR_AMOUNT4.Value - CRAmount;
                    //        CRAmount = 0;
                    //    }
                    //}

                    //DR_AMOUNT3
                    if (data.DR_AMOUNT3 != null)
                    {
                        if (CRAmount >= data.DR_AMOUNT3)
                            CRAmount = CRAmount - data.DR_AMOUNT3.Value;
                        else
                        {
                            result["90+"] = (data.DR_AMOUNT3 - CRAmount).ToString();
                            total += data.DR_AMOUNT3.Value - CRAmount;
                            CRAmount = 0;
                        }
                    }
                    //DR_AMOUNT2
                    if (data.DR_AMOUNT2 != null)
                    {
                        if (CRAmount >= data.DR_AMOUNT2)
                            CRAmount = CRAmount - data.DR_AMOUNT2.Value;
                        else
                        {
                            result["61-90"] = (data.DR_AMOUNT2 - CRAmount).ToString();
                            total += data.DR_AMOUNT2.Value - CRAmount;
                            CRAmount = 0;
                        }
                    }
                    //DR_AMOUNT1
                    if (data.DR_AMOUNT1 != null)
                    {
                        if (CRAmount >= data.DR_AMOUNT1)
                            CRAmount = CRAmount - data.DR_AMOUNT1.Value;
                        else
                        {
                            result["31-60"] = (data.DR_AMOUNT1 - CRAmount).ToString();
                            total += data.DR_AMOUNT1.Value - CRAmount;
                            CRAmount = 0;
                        }
                    }
                    //DR_AMOUNT0
                    if (data.DR_AMOUNT0 != null)
                    {
                        if (CRAmount >= data.DR_AMOUNT0)
                            CRAmount = CRAmount - data.DR_AMOUNT0.Value;
                        else
                        {
                            result["0-30"] = (data.DR_AMOUNT0 - CRAmount).ToString();
                            total += data.DR_AMOUNT0.Value - CRAmount;
                            CRAmount = 0;
                        }
                    }
                    result["total"] = total.ToString();
                    listResult.Add(result);
                }
            }
            return listResult;
        }

        public DistributorItemResponseModel FetchEntityPartyTypeAndMu(EntityRequestModel model, NeoErpCoreEntity dbContext)
        {
            var result = new DistributorItemResponseModel();
            model.entity_code = model.entity_code.FirstOrDefault() == 'C' ? model.entity_code : "C" + model.entity_code;
            var PartyQuery = string.Empty;
            switch (model.entity_type.ToUpper())
            {
                case "D":
                case "DISTRIBUTOR":
                    PartyQuery = $@"SELECT 
                            PT.PARTY_TYPE_CODE AS CODE, 
                            PT.PARTY_TYPE_EDESC AS NAME, 
                            PT.ACC_CODE,
                            DM.CONTACT_NO AS P_CONTACT_NO, '' AS P_CONTACT_NAME, DM.REG_OFFICE_ADDRESS AS ADDRESS,
                            RM.ROUTE_CODE, RM.ROUTE_NAME,
                            AM.AREA_CODE, AM.AREA_NAME,
                            NVL(DM.LATITUDE, 0) LATITUDE, NVL(DM.LONGITUDE, 0) LONGITUDE,
                            'dealer' AS TYPE,
                            '' DEFAULT_PARTY_TYPE_CODE,
                            '' PARENT_DISTRIBUTOR_CODE,
                            '' PARENT_DISTRIBUTOR_NAME,
                            LT.LAST_VISIT_DATE, LT.LAST_VISIT_BY, LT.LAST_VISIT_STATUS, NVL(LT.IS_VISITED, 'X') AS IS_VISITED, LT.REMARKS,
                            DM.COMPANY_CODE, DM.BRANCH_CODE
                            FROM IP_PARTY_TYPE_CODE PT
                            LEFT JOIN DIST_DEALER_MASTER DM ON DM.DEALER_CODE = PT.PARTY_TYPE_CODE AND DM.COMPANY_CODE = PT.COMPANY_CODE
                            LEFT JOIN DIST_ROUTE_ENTITY RE ON RE.ENTITY_CODE = DM.DEALER_CODE AND RE.ENTITY_TYPE = 'P' AND RE.COMPANY_CODE = DM.COMPANY_CODE
                            LEFT JOIN DIST_ROUTE_MASTER RM ON RM.ROUTE_CODE = RE.ROUTE_CODE AND RM.COMPANY_CODE = DM.COMPANY_CODE
                            LEFT JOIN DIST_AREA_MASTER AM ON AM.AREA_CODE = DM.AREA_CODE AND AM.COMPANY_CODE = DM.COMPANY_CODE
                            LEFT JOIN (SELECT A.SP_CODE, A.CUSTOMER_CODE, A.CUSTOMER_TYPE, A.COMPANY_CODE, C.EMPLOYEE_EDESC AS LAST_VISIT_BY, A.IS_VISITED AS LAST_VISIT_STATUS,
                                              (CASE 
                                                WHEN A.IS_VISITED IS NULL THEN 'X' 
                                                  ELSE
                                                    CASE WHEN TO_CHAR(SYSDATE, 'DD-MON-RRRR') = TO_CHAR(A.UPDATE_DATE, 'DD-MON-RRRR') THEN A.IS_VISITED ELSE 'X' END
                                               END
                                              ) IS_VISITED, 
                                              A.REMARKS,
                                              TO_CHAR(MAX(A.UPDATE_DATE), 'RRRR-MM-DD HH24:MI A.M.') LAST_VISIT_DATE
                                            FROM DIST_LOCATION_TRACK A
                                              INNER JOIN IP_PARTY_TYPE_CODE B ON B.PARTY_TYPE_CODE = A.CUSTOMER_CODE AND B.COMPANY_CODE = A.COMPANY_CODE
                                              INNER JOIN HR_EMPLOYEE_SETUP C ON C.EMPLOYEE_CODE = A.SP_CODE AND C.COMPANY_CODE = A.COMPANY_CODE
                                            WHERE 1 = 1
                                                  AND A.CUSTOMER_TYPE = 'P'
                                                  AND A.UPDATE_DATE = (SELECT MAX(UPDATE_DATE) FROM DIST_LOCATION_TRACK WHERE CUSTOMER_CODE = A.CUSTOMER_CODE AND CUSTOMER_TYPE = A.CUSTOMER_TYPE)
                                            GROUP BY A.SP_CODE, A.CUSTOMER_CODE, A.CUSTOMER_TYPE, A.COMPANY_CODE, C.EMPLOYEE_EDESC, A.IS_VISITED, 
                                              (CASE 
                                                WHEN A.IS_VISITED IS NULL THEN 'X' 
                                                  ELSE
                                                    CASE WHEN TO_CHAR(SYSDATE, 'DD-MON-RRRR') = TO_CHAR(A.UPDATE_DATE, 'DD-MON-RRRR') THEN A.IS_VISITED ELSE 'X' END
                                               END
                                              ), 
                                              A.REMARKS
                                           ) LT
                                   ON LT.CUSTOMER_CODE = DM.DEALER_CODE AND LT.COMPANY_CODE = DM.COMPANY_CODE
                            WHERE TRIM(PT.ACC_CODE) IN (SELECT TRIM(ACC_CODE) FROM FA_SUB_LEDGER_MAP WHERE COMPANY_CODE = '{model.COMPANY_CODE}' AND SUB_CODE = TRIM('{model.entity_code}'))
                              AND PT.COMPANY_CODE = '{model.COMPANY_CODE}'
                              AND PT.DELETED_FLAG = 'N'
                            ORDER BY UPPER(PT.PARTY_TYPE_EDESC)";
                    break;
            }

            if (!string.IsNullOrWhiteSpace(PartyQuery))
            {
                var allParty = dbContext.SqlQuery<EntityResponseModel>(PartyQuery).ToList();
                if (allParty.Count > 0)
                {
                    foreach (var party in allParty)
                    {
                        result.PARTY.Add(party.CODE, party);
                    }
                }
            }

            //preferences
            var pref = this.FetchPreferences(model.COMPANY_CODE, dbContext);
            string salesRateClause = string.Empty;
            string conversionClause = string.Empty;
            if (pref.PO_SYN_RATE.Trim().ToUpper() == "Y")
                salesRateClause = "AND SALES_RATE IS NOT NULL AND SALES_RATE <> 0";
            if (pref.SQL_NN_CONVERSION_UNIT_FACTOR.Trim().ToUpper() == "Y")
                conversionClause = "AND IUS.MU_CODE IS NOT NULL AND IUS.CONVERSION_FACTOR IS NOT NULL";

            //Unit
            Dictionary<string, MuCodeResponseModel> MuResult = new Dictionary<string, MuCodeResponseModel>();
            string MuQuery = $@"SELECT IMS.ITEM_CODE, IMS.ITEM_EDESC, IMS.INDEX_MU_CODE, IUS.MU_CODE, IUS.CONVERSION_FACTOR, IMS.COMPANY_CODE
                FROM IP_ITEM_MASTER_SETUP IMS 
                LEFT JOIN IP_ITEM_UNIT_SETUP IUS
                ON IUS.ITEM_CODE = IMS.ITEM_CODE AND IUS.COMPANY_CODE = IMS.COMPANY_CODE
                WHERE 1 = 1
                AND IMS.COMPANY_CODE IN (SELECT COMPANY_CODE FROM COMPANY_SETUP)
                AND IMS.CATEGORY_CODE = '{CATEGORY_CODE}'
                AND IMS.GROUP_SKU_FLAG = '{GROUP_SKU_FLAG}'
                AND IMS.DELETED_FLAG = 'N'
                {conversionClause}
                ORDER BY IMS.COMPANY_CODE, UPPER(IMS.ITEM_EDESC), UPPER(IMS.INDEX_MU_CODE), UPPER(MU_CODE)";
            var allMu = dbContext.SqlQuery<MuCodeResponseModel>(MuQuery).GroupBy(x => x.COMPANY_CODE);
            foreach (var Mu in allMu)
            {
                var tempResult = new Dictionary<string, MuCodeResponseModel>();
                var CompanyMu = Mu.ToList();
                foreach (var CM in CompanyMu)
                {
                    if (CM.MU_CODE != null)
                        CM.CONVERSION_UNIT_FACTOR.Add(CM.MU_CODE, CM.CONVERSION_FACTOR);
                    tempResult.Add(CM.ITEM_CODE, CM);
                }
                result.UNIT[Mu.Key] = tempResult;
            }

            //sales types
            var salesTypes = FetchAllCompanySaSalesType(dbContext).GroupBy(x => x.COMPANY_CODE);
            foreach (var type in salesTypes)
            {
                result.SALES_TYPE.Add(type.Key, type.ToList());
            }
            result.SHIPPING_ADDRESS = FetchShippingAddress(dbContext); //shipping addresses
            return result;
        }

        public Dictionary<string, List<EntityResponseModel>> FetchPartyTypeBillingEntity(EntityRequestModel model, NeoErpCoreEntity dbContext)
        {
            if (string.IsNullOrWhiteSpace(model.COMPANY_CODE))
                throw new Exception("Company code not found!!!");
            if (string.IsNullOrWhiteSpace(model.ACC_CODE))
                throw new Exception("Account code not found!!!");

            string Query = $@"SELECT CS.CUSTOMER_CODE AS CODE,
                  CS.CUSTOMER_EDESC AS NAME,
                  CS.ACC_CODE AS ACC_CODE,
                  CS.PARTY_TYPE_CODE AS DEFAULT_PARTY_TYPE_CODE,
                  DM.CONTACT_NO AS P_CONTACT_NO, '' AS P_CONTACT_NAME, NVL(DM.REG_OFFICE_ADDRESS, CS.REGD_OFFICE_EADDRESS) AS ADDRESS,
                  AM.AREA_CODE, AM.AREA_NAME,
                  NVL(DM.LATITUDE, '0') AS LATITUDE,
                  NVL(DM.LONGITUDE, '0') AS LONGITUDE,
                  'distributor' AS TYPE,
                   '' AS PARENT_DISTRIBUTOR_CODE,
                   '' AS PARENT_DISTRIBUTOR_NAME,
                   LT.LAST_VISIT_DATE, LT.LAST_VISIT_BY, LT.LAST_VISIT_STATUS, NVL(LT.IS_VISITED, 'X') AS IS_VISITED, LT.REMARKS,
                   CS.COMPANY_CODE, CS.BRANCH_CODE
                FROM SA_CUSTOMER_SETUP CS
                LEFT JOIN DIST_DISTRIBUTOR_MASTER DM ON DM.DISTRIBUTOR_CODE = CS.CUSTOMER_CODE AND DM.COMPANY_CODE = CS.COMPANY_CODE
                LEFT JOIN DIST_AREA_MASTER AM ON AM.AREA_CODE = DM.AREA_CODE AND AM.COMPANY_CODE = CS.COMPANY_CODE
                LEFT JOIN (SELECT A.SP_CODE, A.CUSTOMER_CODE, A.CUSTOMER_TYPE, A.COMPANY_CODE, C.EMPLOYEE_EDESC AS LAST_VISIT_BY, A.IS_VISITED AS LAST_VISIT_STATUS,
                              (CASE 
                                WHEN A.IS_VISITED IS NULL THEN 'X' 
                                  ELSE
                                    CASE WHEN TO_CHAR(SYSDATE, 'DD-MON-RRRR') = TO_CHAR(A.UPDATE_DATE, 'DD-MON-RRRR') THEN A.IS_VISITED ELSE 'X' END
                               END
                              ) IS_VISITED, 
                              A.REMARKS,
                              TO_CHAR(MAX(A.UPDATE_DATE), 'RRRR-MM-DD HH24:MI A.M.') LAST_VISIT_DATE
                          FROM DIST_LOCATION_TRACK A
                          INNER JOIN SA_CUSTOMER_SETUP B ON B.CUSTOMER_CODE = A.CUSTOMER_CODE AND B.COMPANY_CODE = A.COMPANY_CODE
                          INNER JOIN HR_EMPLOYEE_SETUP C ON C.EMPLOYEE_CODE = A.SP_CODE AND C.COMPANY_CODE = A.COMPANY_CODE
                          WHERE 1 = 1
                          AND A.CUSTOMER_TYPE = 'D'
                          AND A.UPDATE_DATE = (SELECT MAX(UPDATE_DATE) FROM DIST_LOCATION_TRACK WHERE CUSTOMER_CODE = A.CUSTOMER_CODE AND CUSTOMER_TYPE = A.CUSTOMER_TYPE)
                          GROUP BY A.SP_CODE, A.CUSTOMER_CODE, A.CUSTOMER_TYPE, A.COMPANY_CODE, C.EMPLOYEE_EDESC, A.IS_VISITED, 
                              (CASE 
                                WHEN A.IS_VISITED IS NULL THEN 'X' 
                                  ELSE
                                    CASE WHEN TO_CHAR(SYSDATE, 'DD-MON-RRRR') = TO_CHAR(A.UPDATE_DATE, 'DD-MON-RRRR') THEN A.IS_VISITED ELSE 'X' END
                               END
                              ), 
                              A.REMARKS
                          ) LT
                       ON LT.CUSTOMER_CODE = CS.CUSTOMER_CODE AND LT.COMPANY_CODE = CS.COMPANY_CODE                       
                WHERE CS.GROUP_SKU_FLAG = 'I'
                AND TRIM(CS.LINK_SUB_CODE) IN (SELECT TRIM(SUB_CODE) FROM FA_SUB_LEDGER_MAP WHERE COMPANY_CODE = '{model.COMPANY_CODE}' AND ACC_CODE ='{model.ACC_CODE}')
                AND CS.COMPANY_CODE = '{model.COMPANY_CODE}'
                ORDER BY UPPER(CS.CUSTOMER_EDESC)";
            var AllParty = dbContext.SqlQuery<EntityResponseModel>(Query).ToList();
            var result = new Dictionary<string, List<EntityResponseModel>>();
            if (AllParty != null)
                result.Add("distributor", AllParty);
            return result;
        }

        public List<EntityResponseModel> FetchEntityById(EntityRequestModel model, NeoErpCoreEntity dbContext)
        {
            string Query = string.Empty;
            if (model.entity_type.Equals("P", StringComparison.OrdinalIgnoreCase) || model.entity_type.Equals("DEALER", StringComparison.OrdinalIgnoreCase))
            {
                Query = $@"SELECT
                     DM.DEALER_CODE AS CODE,
                     PT.PARTY_TYPE_EDESC AS NAME,
                     PT.ACC_CODE,
                     DM.CONTACT_NO AS P_CONTACT_NO, '' AS P_CONTACT_NAME, DM.REG_OFFICE_ADDRESS AS ADDRESS,
                     RM.ROUTE_CODE, RM.ROUTE_NAME,
                     AM.AREA_CODE, AM.AREA_NAME,
                     NVL(DM.LATITUDE,0) LATITUDE, NVL(DM.LONGITUDE,0) LONGITUDE,
                     'dealer' AS TYPE,
                     '' DEFAULT_PARTY_TYPE_CODE,
                     '' PARENT_DISTRIBUTOR_CODE,
                     '' PARENT_DISTRIBUTOR_NAME,
                     LT.LAST_VISIT_DATE, LT.LAST_VISIT_BY, LT.LAST_VISIT_STATUS, NVL(LT.IS_VISITED, 'X') AS IS_VISITED, LT.REMARKS,
                     DM.COMPANY_CODE, DM.BRANCH_CODE
                   FROM
                     DIST_DEALER_MASTER DM
                     LEFT JOIN DIST_ROUTE_ENTITY RE ON RE.ENTITY_CODE = DM.DEALER_CODE AND RE.ENTITY_TYPE = 'P' AND RE.COMPANY_CODE = DM.COMPANY_CODE
                     LEFT JOIN DIST_ROUTE_MASTER RM ON RM.ROUTE_CODE = RE.ROUTE_CODE AND RM.COMPANY_CODE = DM.COMPANY_CODE
                     INNER JOIN IP_PARTY_TYPE_CODE PT ON PT.PARTY_TYPE_CODE = DM.DEALER_CODE AND PT.COMPANY_CODE = DM.COMPANY_CODE
                     INNER JOIN DIST_AREA_MASTER AM ON AM.AREA_CODE = DM.AREA_CODE AND AM.COMPANY_CODE = DM.COMPANY_CODE
                     LEFT JOIN (SELECT A.SP_CODE, A.CUSTOMER_CODE, A.CUSTOMER_TYPE, A.COMPANY_CODE, C.EMPLOYEE_EDESC AS LAST_VISIT_BY, A.IS_VISITED AS LAST_VISIT_STATUS,
                                  (CASE 
                                    WHEN A.IS_VISITED IS NULL THEN 'X' 
                                      ELSE
                                        CASE WHEN TO_CHAR(SYSDATE, 'DD-MON-RRRR') = TO_CHAR(A.UPDATE_DATE, 'DD-MON-RRRR') THEN A.IS_VISITED ELSE 'X' END
                                   END
                                  ) IS_VISITED, 
                                  A.REMARKS,
                                  TO_CHAR(MAX(A.UPDATE_DATE), 'RRRR-MM-DD HH24:MI A.M.') LAST_VISIT_DATE
                                FROM DIST_LOCATION_TRACK A
                                  INNER JOIN SA_CUSTOMER_SETUP B ON B.CUSTOMER_CODE = A.CUSTOMER_CODE AND B.COMPANY_CODE = A.COMPANY_CODE
                                  INNER JOIN HR_EMPLOYEE_SETUP C ON C.EMPLOYEE_CODE = A.SP_CODE AND C.COMPANY_CODE = A.COMPANY_CODE
                                WHERE 1 = 1
                                      AND A.CUSTOMER_TYPE = 'P'
                                      AND A.CUSTOMER_CODE = '{model.entity_code}'
                                      AND A.UPDATE_DATE = (SELECT MAX(UPDATE_DATE) FROM DIST_LOCATION_TRACK WHERE CUSTOMER_CODE = A.CUSTOMER_CODE AND CUSTOMER_TYPE = A.CUSTOMER_TYPE)
                                GROUP BY A.SP_CODE, A.CUSTOMER_CODE, A.CUSTOMER_TYPE, A.COMPANY_CODE, C.EMPLOYEE_EDESC, A.IS_VISITED, 
                                  (CASE 
                                    WHEN A.IS_VISITED IS NULL THEN 'X' 
                                      ELSE
                                        CASE WHEN TO_CHAR(SYSDATE, 'DD-MON-RRRR') = TO_CHAR(A.UPDATE_DATE, 'DD-MON-RRRR') THEN A.IS_VISITED ELSE 'X' END
                                   END
                                  ), 
                                  A.REMARKS
                               ) LT
                       ON LT.CUSTOMER_CODE = DM.DEALER_CODE AND LT.COMPANY_CODE = DM.COMPANY_CODE
                   WHERE 1 = 1
                         AND DM.COMPANY_CODE = '{model.COMPANY_CODE}'
                         AND DM.DEALER_CODE = '{model.entity_code}'";
            }
            else if (model.entity_type.Equals("D", StringComparison.OrdinalIgnoreCase) || model.entity_type.Equals("DISTRIBUTOR", StringComparison.OrdinalIgnoreCase))
            {
                Query = $@"SELECT
                     DM.DISTRIBUTOR_CODE AS CODE,
                     CS.CUSTOMER_EDESC AS NAME,
                     CS.ACC_CODE,
                     DM.CONTACT_NO AS P_CONTACT_NO, '' AS P_CONTACT_NAME, NVL(DM.REG_OFFICE_ADDRESS, CS.REGD_OFFICE_EADDRESS) AS ADDRESS,
                     RM.ROUTE_CODE, RM.ROUTE_NAME,
                     AM.AREA_CODE, AM.AREA_NAME,
                     NVL(DM.LATITUDE,0) LATITUDE, NVL(DM.LONGITUDE,0) LONGITUDE,
                     'distributor' AS TYPE,
                     CS.PARTY_TYPE_CODE AS DEFAULT_PARTY_TYPE_CODE,
                     '' PARENT_DISTRIBUTOR_CODE,
                     '' PARENT_DISTRIBUTOR_NAME,
                     LT.LAST_VISIT_DATE, LT.LAST_VISIT_BY, LT.LAST_VISIT_STATUS, NVL(LT.IS_VISITED, 'X') AS IS_VISITED, LT.REMARKS,
                     DM.COMPANY_CODE, DM.BRANCH_CODE
                   FROM
                     DIST_DISTRIBUTOR_MASTER DM
                     LEFT JOIN DIST_ROUTE_ENTITY RE ON RE.ENTITY_CODE = DM.DISTRIBUTOR_CODE AND RE.ENTITY_TYPE = 'D' AND RE.COMPANY_CODE = DM.COMPANY_CODE
                     LEFT JOIN DIST_ROUTE_MASTER RM ON RM.ROUTE_CODE = RE.ROUTE_CODE AND RM.COMPANY_CODE = DM.COMPANY_CODE
                     INNER JOIN SA_CUSTOMER_SETUP CS ON CS.CUSTOMER_CODE = DM.DISTRIBUTOR_CODE AND CS.COMPANY_CODE = DM.COMPANY_CODE
                     INNER JOIN DIST_AREA_MASTER AM ON AM.AREA_CODE = DM.AREA_CODE AND AM.COMPANY_CODE = DM.COMPANY_CODE
                     LEFT JOIN (SELECT A.SP_CODE, A.CUSTOMER_CODE, A.CUSTOMER_TYPE, A.COMPANY_CODE, C.EMPLOYEE_EDESC AS LAST_VISIT_BY, A.IS_VISITED AS LAST_VISIT_STATUS,
                                  (CASE 
                                    WHEN A.IS_VISITED IS NULL THEN 'X' 
                                      ELSE
                                        CASE WHEN TO_CHAR(SYSDATE, 'DD-MON-RRRR') = TO_CHAR(A.UPDATE_DATE, 'DD-MON-RRRR') THEN A.IS_VISITED ELSE 'X' END
                                   END
                                  ) IS_VISITED, 
                                  A.REMARKS,
                                  TO_CHAR(MAX(A.UPDATE_DATE), 'RRRR-MM-DD HH24:MI A.M.') LAST_VISIT_DATE
                                FROM DIST_LOCATION_TRACK A
                                  INNER JOIN SA_CUSTOMER_SETUP B ON B.CUSTOMER_CODE = A.CUSTOMER_CODE AND B.COMPANY_CODE = A.COMPANY_CODE
                                  INNER JOIN HR_EMPLOYEE_SETUP C ON C.EMPLOYEE_CODE = A.SP_CODE AND C.COMPANY_CODE = A.COMPANY_CODE
                                WHERE 1 = 1
                                      AND A.CUSTOMER_TYPE = 'D'
                                      AND A.CUSTOMER_CODE = '{model.entity_code}'
                                      AND A.UPDATE_DATE = (SELECT MAX(UPDATE_DATE) FROM DIST_LOCATION_TRACK WHERE CUSTOMER_CODE = A.CUSTOMER_CODE AND CUSTOMER_TYPE = A.CUSTOMER_TYPE)
                                GROUP BY A.SP_CODE, A.CUSTOMER_CODE, A.CUSTOMER_TYPE, A.COMPANY_CODE, C.EMPLOYEE_EDESC, A.IS_VISITED, 
                                  (CASE 
                                    WHEN A.IS_VISITED IS NULL THEN 'X' 
                                      ELSE
                                        CASE WHEN TO_CHAR(SYSDATE, 'DD-MON-RRRR') = TO_CHAR(A.UPDATE_DATE, 'DD-MON-RRRR') THEN A.IS_VISITED ELSE 'X' END
                                   END
                                  ), 
                                  A.REMARKS
                               ) LT
                       ON LT.CUSTOMER_CODE = DM.DISTRIBUTOR_CODE AND LT.COMPANY_CODE = DM.COMPANY_CODE
                   WHERE 1 = 1
                         AND DM.COMPANY_CODE = '{model.COMPANY_CODE}'
                         AND DM.DISTRIBUTOR_CODE = '{model.entity_code}'";
            }
            else if (model.entity_type.Equals("R", StringComparison.OrdinalIgnoreCase) || model.entity_type.Equals("RESELLER", StringComparison.OrdinalIgnoreCase))
            {
                Query = $@"SELECT
                     REM.RESELLER_CODE AS CODE,
                     REM.RESELLER_NAME AS NAME,
                     '' AS ACC_CODE,
                     REM.CONTACT_NO AS P_CONTACT_NO, REM.CONTACT_NAME AS P_CONTACT_NAME, REM.REG_OFFICE_ADDRESS AS ADDRESS,
                     RM.ROUTE_CODE, RM.ROUTE_NAME,
                     AM.AREA_CODE, AM.AREA_NAME,
                     NVL(REM.LATITUDE,0) LATITUDE, NVL(REM.LONGITUDE,0) LONGITUDE,
                     'reseller' AS TYPE,
                     '' AS DEFAULT_PARTY_TYPE_CODE,
                     DISTRIBUTOR_CODE AS PARENT_DISTRIBUTOR_CODE,
                     CS.CUSTOMER_EDESC AS PARENT_DISTRIBUTOR_NAME,
                     LT.LAST_VISIT_DATE, LT.LAST_VISIT_BY, LT.LAST_VISIT_STATUS, NVL(LT.IS_VISITED, 'X') AS IS_VISITED, LT.REMARKS,
                     REM.COMPANY_CODE, REM.BRANCH_CODE
                   FROM DIST_RESELLER_MASTER REM
                     LEFT JOIN DIST_ROUTE_ENTITY RE ON RE.ENTITY_CODE = REM.RESELLER_CODE AND RE.ENTITY_TYPE = 'R' AND RE.COMPANY_CODE = REM.COMPANY_CODE
                     LEFT JOIN DIST_ROUTE_MASTER RM ON RM.ROUTE_CODE = RE.ROUTE_CODE AND RM.COMPANY_CODE = REM.COMPANY_CODE
                     INNER JOIN DIST_AREA_MASTER AM ON AM.AREA_CODE = REM.AREA_CODE AND AM.COMPANY_CODE = REM.COMPANY_CODE
                     LEFT JOIN SA_CUSTOMER_SETUP CS ON CS.CUSTOMER_CODE = REM.DISTRIBUTOR_CODE AND CS.COMPANY_CODE = REM.COMPANY_CODE
                     LEFT JOIN (SELECT A.SP_CODE, A.CUSTOMER_CODE, A.CUSTOMER_TYPE, A.COMPANY_CODE, C.EMPLOYEE_EDESC AS LAST_VISIT_BY, A.IS_VISITED AS LAST_VISIT_STATUS,
                                  (CASE 
                                    WHEN A.IS_VISITED IS NULL THEN 'X' 
                                      ELSE
                                        CASE WHEN TO_CHAR(SYSDATE, 'DD-MON-RRRR') = TO_CHAR(A.UPDATE_DATE, 'DD-MON-RRRR') THEN A.IS_VISITED ELSE 'X' END
                                   END
                                  ) IS_VISITED, 
                                  A.REMARKS,
                                  TO_CHAR(MAX(A.UPDATE_DATE), 'RRRR-MM-DD HH24:MI A.M.') LAST_VISIT_DATE
                                FROM DIST_LOCATION_TRACK A
                                  INNER JOIN DIST_RESELLER_MASTER B ON B.RESELLER_CODE = A.CUSTOMER_CODE AND B.COMPANY_CODE = A.COMPANY_CODE
                                  INNER JOIN HR_EMPLOYEE_SETUP C ON C.EMPLOYEE_CODE = A.SP_CODE AND C.COMPANY_CODE = A.COMPANY_CODE
                                WHERE 1 = 1
                                      AND A.CUSTOMER_TYPE = 'R'
                                      AND A.CUSTOMER_CODE = '{model.entity_code}'
                                      AND A.UPDATE_DATE = (SELECT MAX(UPDATE_DATE) FROM DIST_LOCATION_TRACK WHERE CUSTOMER_CODE = A.CUSTOMER_CODE AND CUSTOMER_TYPE = A.CUSTOMER_TYPE)
                                GROUP BY A.SP_CODE, A.CUSTOMER_CODE, A.CUSTOMER_TYPE, A.COMPANY_CODE, C.EMPLOYEE_EDESC, A.IS_VISITED, 
                                  (CASE 
                                    WHEN A.IS_VISITED IS NULL THEN 'X' 
                                      ELSE
                                        CASE WHEN TO_CHAR(SYSDATE, 'DD-MON-RRRR') = TO_CHAR(A.UPDATE_DATE, 'DD-MON-RRRR') THEN A.IS_VISITED ELSE 'X' END
                                   END
                                  ), 
                                  A.REMARKS
                               ) LT
                       ON LT.CUSTOMER_CODE = REM.RESELLER_CODE AND LT.COMPANY_CODE = REM.COMPANY_CODE
                   WHERE 1 = 1
                         AND REM.COMPANY_CODE = '{model.COMPANY_CODE}'
                         AND REM.RESELLER_CODE = '{model.entity_code}'";
            }
            else
                throw new Exception("Invalid entity type");
            if (string.IsNullOrWhiteSpace(Query))
                return new List<EntityResponseModel>();
            var result = dbContext.SqlQuery<EntityResponseModel>(Query).ToList();
            return result;
        }

        public Dictionary<string, List<EntityResponseModel>> FetchDistributorWithConstraint(CommonRequestModel model, NeoErpCoreEntity dbContext)
        {
            string Query = string.Empty;
            Query = $@"SELECT
                     DM.DISTRIBUTOR_CODE AS CODE,
                     CS.CUSTOMER_EDESC AS NAME,
                     CS.ACC_CODE,
                     DM.CONTACT_NO AS P_CONTACT_NO, '' AS P_CONTACT_NAME, NVL(DM.REG_OFFICE_ADDRESS, CS.REGD_OFFICE_EADDRESS) AS ADDRESS,
                     RM.ROUTE_CODE, RM.ROUTE_NAME,
                     AM.AREA_CODE, AM.AREA_NAME,
                     NVL(DM.LATITUDE,0) LATITUDE, NVL(DM.LONGITUDE,0) LONGITUDE,
                     'distributor' AS TYPE,
                     CS.PARTY_TYPE_CODE AS DEFAULT_PARTY_TYPE_CODE,
                     '' PARENT_DISTRIBUTOR_CODE,
                     '' PARENT_DISTRIBUTOR_NAME,
                     LT.LAST_VISIT_DATE, LT.LAST_VISIT_BY, LT.LAST_VISIT_STATUS, NVL(LT.IS_VISITED, 'X') AS IS_VISITED, LT.REMARKS,
                     DM.COMPANY_CODE, DM.BRANCH_CODE
                   FROM
                     DIST_DISTRIBUTOR_MASTER DM
                     LEFT JOIN DIST_ROUTE_ENTITY RE ON RE.ENTITY_CODE = DM.DISTRIBUTOR_CODE AND RE.ENTITY_TYPE = 'D' AND RE.COMPANY_CODE = DM.COMPANY_CODE
                     LEFT JOIN DIST_ROUTE_MASTER RM ON RM.ROUTE_CODE = RE.ROUTE_CODE AND RM.COMPANY_CODE = DM.COMPANY_CODE
                     INNER JOIN SA_CUSTOMER_SETUP CS ON CS.CUSTOMER_CODE = DM.DISTRIBUTOR_CODE AND CS.COMPANY_CODE = DM.COMPANY_CODE
                     INNER JOIN DIST_AREA_MASTER AM ON AM.AREA_CODE = DM.AREA_CODE AND AM.COMPANY_CODE = DM.COMPANY_CODE
                     LEFT JOIN (SELECT A.SP_CODE, A.CUSTOMER_CODE, A.CUSTOMER_TYPE, A.COMPANY_CODE, C.EMPLOYEE_EDESC AS LAST_VISIT_BY, A.IS_VISITED AS LAST_VISIT_STATUS,
                                  (CASE 
                                    WHEN A.IS_VISITED IS NULL THEN 'X' 
                                      ELSE
                                        CASE WHEN TO_CHAR(SYSDATE, 'DD-MON-RRRR') = TO_CHAR(A.UPDATE_DATE, 'DD-MON-RRRR') THEN A.IS_VISITED ELSE 'X' END
                                   END
                                  ) IS_VISITED, 
                                  A.REMARKS,
                                  TO_CHAR(MAX(A.UPDATE_DATE), 'RRRR-MM-DD HH24:MI A.M.') LAST_VISIT_DATE
                                FROM DIST_LOCATION_TRACK A
                                  INNER JOIN SA_CUSTOMER_SETUP B ON B.CUSTOMER_CODE = A.CUSTOMER_CODE AND B.COMPANY_CODE = A.COMPANY_CODE
                                  INNER JOIN HR_EMPLOYEE_SETUP C ON C.EMPLOYEE_CODE = A.SP_CODE AND C.COMPANY_CODE = A.COMPANY_CODE
                                WHERE 1 = 1
                                      AND A.CUSTOMER_TYPE = 'D'
                                      AND A.UPDATE_DATE = (SELECT MAX(UPDATE_DATE) FROM DIST_LOCATION_TRACK WHERE CUSTOMER_CODE = A.CUSTOMER_CODE AND CUSTOMER_TYPE = A.CUSTOMER_TYPE)
                                GROUP BY A.SP_CODE, A.CUSTOMER_CODE, A.CUSTOMER_TYPE, A.COMPANY_CODE, C.EMPLOYEE_EDESC, A.IS_VISITED, 
                                  (CASE 
                                    WHEN A.IS_VISITED IS NULL THEN 'X' 
                                      ELSE
                                        CASE WHEN TO_CHAR(SYSDATE, 'DD-MON-RRRR') = TO_CHAR(A.UPDATE_DATE, 'DD-MON-RRRR') THEN A.IS_VISITED ELSE 'X' END
                                   END
                                  ), 
                                  A.REMARKS
                               ) LT
                       ON LT.CUSTOMER_CODE = DM.DISTRIBUTOR_CODE AND LT.COMPANY_CODE = DM.COMPANY_CODE
                   WHERE 1 = 1
                         AND DM.COMPANY_CODE = '{model.COMPANY_CODE}'";
            var distributors = dbContext.SqlQuery<EntityResponseModel>(Query).ToList();
            var result = new Dictionary<string, List<EntityResponseModel>>();
            result.Add("distributor", distributors);
            return result;
        }

        public List<SPEntityModel> FetchSpPartyType(VisitPlanRequestModel model, NeoErpCoreEntity dbContext)
        {
            var Query = $@"SELECT
                DM.DEALER_CODE AS CODE,
                TRIM(PT.PARTY_TYPE_EDESC) AS NAME,
                TRIM(PT.ACC_CODE) ACC_CODE,
                DM.CONTACT_NO AS P_CONTACT_NO, DM.REG_OFFICE_ADDRESS AS ADDRESS,
                RM.ROUTE_CODE, RM.ROUTE_NAME,
                AM.AREA_CODE, AM.AREA_NAME,
                RE.ORDER_NO,
                RM.COMPANY_CODE,
                'dealer' AS TYPE,
                '' DEFAULT_PARTY_TYPE_CODE,
                '' PARENT_DISTRIBUTOR_CODE,
                '' PARENT_DISTRIBUTOR_NAME,
                NVL(DM.LATITUDE,0) LATITUDE, NVL(DM.LONGITUDE,0) LONGITUDE,
                LT.LAST_VISIT_DATE, LT.LAST_VISIT_BY, NVL(LT.LAST_VISIT_STATUS, 'X') LAST_VISIT_STATUS, NVL(LT.IS_VISITED, 'X') AS IS_VISITED, LT.REMARKS
                FROM DIST_ROUTE_MASTER RM
                INNER JOIN (SELECT ROUTE_CODE, EMP_CODE, COMPANY_CODE FROM DIST_ROUTE_DETAIL WHERE DELETED_FLAG = 'N' GROUP BY ROUTE_CODE, EMP_CODE, COMPANY_CODE) RD ON RD.ROUTE_CODE = RM.ROUTE_CODE AND RD.COMPANY_CODE = RM.COMPANY_CODE
                INNER JOIN DIST_ROUTE_ENTITY RE ON RE.ROUTE_CODE = RD.ROUTE_CODE AND RE.ENTITY_TYPE = 'P' AND RE.COMPANY_CODE = RM.COMPANY_CODE
                INNER JOIN DIST_DEALER_MASTER DM ON DM.DEALER_CODE = RE.ENTITY_CODE AND DM.COMPANY_CODE = RM.COMPANY_CODE
                INNER JOIN IP_PARTY_TYPE_CODE PT ON PT.PARTY_TYPE_CODE = DM.DEALER_CODE AND PT.COMPANY_CODE = DM.COMPANY_CODE
                INNER JOIN DIST_AREA_MASTER AM ON AM.AREA_CODE = DM.AREA_CODE AND AM.COMPANY_CODE = DM.COMPANY_CODE
                LEFT JOIN (SELECT A.SP_CODE, A.CUSTOMER_CODE, A.CUSTOMER_TYPE, A.COMPANY_CODE, C.EMPLOYEE_EDESC AS LAST_VISIT_BY, A.IS_VISITED AS LAST_VISIT_STATUS, 
                          (CASE 
                            WHEN A.IS_VISITED IS NULL THEN 'X' 
                              ELSE
                                CASE WHEN TO_CHAR(SYSDATE, 'DD-MON-RRRR') = TO_CHAR(A.UPDATE_DATE, 'DD-MON-RRRR') THEN A.IS_VISITED ELSE 'X' END
                           END
                          ) IS_VISITED, 
                          A.REMARKS,
                          TO_CHAR(MAX(A.UPDATE_DATE), 'DD-MON-RRRR HH24:MI A.M.') LAST_VISIT_DATE
                          FROM DIST_LOCATION_TRACK A
                          INNER JOIN IP_PARTY_TYPE_CODE B ON B.PARTY_TYPE_CODE = A.CUSTOMER_CODE AND B.COMPANY_CODE = A.COMPANY_CODE
                          INNER JOIN HR_EMPLOYEE_SETUP C ON C.EMPLOYEE_CODE = A.SP_CODE AND C.COMPANY_CODE = A.COMPANY_CODE
                          WHERE 1 = 1
                          -- AND SP_CODE = '1000097' -- Commented out because customer can be visited by another SP_CODE
                          AND A.CUSTOMER_TYPE = 'P'
                          AND A.UPDATE_DATE = (SELECT MAX(UPDATE_DATE) FROM DIST_LOCATION_TRACK WHERE CUSTOMER_CODE = A.CUSTOMER_CODE AND CUSTOMER_TYPE = A.CUSTOMER_TYPE)
                          GROUP BY A.SP_CODE, A.CUSTOMER_CODE, A.CUSTOMER_TYPE, A.COMPANY_CODE, C.EMPLOYEE_EDESC, A.IS_VISITED, 
                              (CASE 
                                WHEN A.IS_VISITED IS NULL THEN 'X' 
                                  ELSE
                                    CASE WHEN TO_CHAR(SYSDATE, 'DD-MON-RRRR') = TO_CHAR(A.UPDATE_DATE, 'DD-MON-RRRR') THEN A.IS_VISITED ELSE 'X' END
                               END
                              ), 
                              A.REMARKS
                          ) LT
                          ON LT.CUSTOMER_CODE = DM.DEALER_CODE AND LT.COMPANY_CODE = RM.COMPANY_CODE
                WHERE 1 = 1
                --AND TO_CHAR(RD.ASSIGN_DATE, 'DD-MON-RRRR') = UPPER('23-MAY-2017')
                AND RD.EMP_CODE = '{model.spcode}'
                AND RD.COMPANY_CODE = '{model.COMPANY_CODE}'
                GROUP BY 
                  DM.DEALER_CODE,
                  TRIM(PT.PARTY_TYPE_EDESC),
                  TRIM(PT.ACC_CODE),
                  DM.CONTACT_NO, DM.REG_OFFICE_ADDRESS,
                  RM.ROUTE_CODE, RM.ROUTE_NAME,
                  AM.AREA_CODE, AM.AREA_NAME,
                  RE.ORDER_NO,
                  RM.COMPANY_CODE,
                  'dealer',
                  '',
                  '',
                  '',
                  NVL(DM.LATITUDE,0), NVL(DM.LONGITUDE,0),
                  LT.LAST_VISIT_DATE, LT.LAST_VISIT_BY, NVL(LT.LAST_VISIT_STATUS, 'X'), NVL(LT.IS_VISITED, 'X'), LT.REMARKS
                ORDER BY UPPER(TRIM(PT.PARTY_TYPE_EDESC)) ASC";

            var data = dbContext.SqlQuery<SPEntityModel>(Query).ToList();
            return data;
        }

        public List<SPEntityModel> FetchSpCustomer(VisitPlanRequestModel model, NeoErpCoreEntity dbContext)
        {
            var Query = $@"SELECT
                DM.DISTRIBUTOR_CODE AS CODE,
                TRIM(CS.CUSTOMER_EDESC) AS NAME,
                TRIM(CS.ACC_CODE) AS ACC_CODE,
                DM.CONTACT_NO AS P_CONTACT_NO, NVL(DM.REG_OFFICE_ADDRESS, CS.REGD_OFFICE_EADDRESS) AS ADDRESS,
                RM.ROUTE_CODE, RM.ROUTE_NAME,
                AM.AREA_CODE, AM.AREA_NAME,
                RE.ORDER_NO,
                RM.COMPANY_CODE,
                'distributor' AS TYPE,
                CS.PARTY_TYPE_CODE AS DEFAULT_PARTY_TYPE_CODE,
                '' PARENT_DISTRIBUTOR_CODE,
                '' PARENT_DISTRIBUTOR_NAME,
                NVL(DM.LATITUDE,0) LATITUDE, NVL(DM.LONGITUDE,0) LONGITUDE,
                LT.LAST_VISIT_DATE, LT.LAST_VISIT_BY, NVL(LT.LAST_VISIT_STATUS, 'X') LAST_VISIT_STATUS, NVL(LT.IS_VISITED, 'X') AS IS_VISITED, LT.REMARKS
                FROM DIST_ROUTE_MASTER RM
                INNER JOIN DIST_ROUTE_DETAIL RD ON RD.ROUTE_CODE = RM.ROUTE_CODE AND RD.COMPANY_CODE = RM.COMPANY_CODE
                INNER JOIN DIST_ROUTE_ENTITY RE ON RE.ROUTE_CODE = RD.ROUTE_CODE AND RE.ENTITY_TYPE = 'D' AND RE.COMPANY_CODE = RM.COMPANY_CODE
                INNER JOIN DIST_DISTRIBUTOR_MASTER DM ON DM.DISTRIBUTOR_CODE = RE.ENTITY_CODE AND DM.COMPANY_CODE = RM.COMPANY_CODE
                INNER JOIN SA_CUSTOMER_SETUP CS ON CS.CUSTOMER_CODE = DM.DISTRIBUTOR_CODE AND CS.COMPANY_CODE = DM.COMPANY_CODE
                INNER JOIN DIST_AREA_MASTER AM ON AM.AREA_CODE = DM.AREA_CODE AND AM.COMPANY_CODE = DM.COMPANY_CODE
                LEFT JOIN (SELECT A.SP_CODE, A.CUSTOMER_CODE, A.CUSTOMER_TYPE, A.COMPANY_CODE, C.EMPLOYEE_EDESC AS LAST_VISIT_BY, A.IS_VISITED AS LAST_VISIT_STATUS, 
                          (CASE 
                            WHEN A.IS_VISITED IS NULL THEN 'X' 
                              ELSE
                                CASE WHEN TO_CHAR(SYSDATE, 'DD-MON-RRRR') = TO_CHAR(A.UPDATE_DATE, 'DD-MON-RRRR') THEN A.IS_VISITED ELSE 'X' END
                           END
                          ) IS_VISITED, 
                          A.REMARKS,
                          TO_CHAR(MAX(A.UPDATE_DATE), 'DD-MON-RRRR HH24:MI A.M.') LAST_VISIT_DATE
                          FROM DIST_LOCATION_TRACK A
                          INNER JOIN SA_CUSTOMER_SETUP B ON B.CUSTOMER_CODE = A.CUSTOMER_CODE AND B.COMPANY_CODE = A.COMPANY_CODE
                          INNER JOIN HR_EMPLOYEE_SETUP C ON C.EMPLOYEE_CODE = A.SP_CODE AND C.COMPANY_CODE = A.COMPANY_CODE
                          WHERE 1 = 1
                          -- AND SP_CODE = '1000097' -- Commented out because customer can be visited by another SP_CODE
                          AND A.CUSTOMER_TYPE = 'D'
                          AND A.UPDATE_DATE = (SELECT MAX(UPDATE_DATE) FROM DIST_LOCATION_TRACK WHERE CUSTOMER_CODE = A.CUSTOMER_CODE AND CUSTOMER_TYPE = A.CUSTOMER_TYPE)
                          GROUP BY A.SP_CODE, A.CUSTOMER_CODE, A.CUSTOMER_TYPE, A.COMPANY_CODE, C.EMPLOYEE_EDESC, A.IS_VISITED, 
                              (CASE 
                                WHEN A.IS_VISITED IS NULL THEN 'X' 
                                  ELSE
                                    CASE WHEN TO_CHAR(SYSDATE, 'DD-MON-RRRR') = TO_CHAR(A.UPDATE_DATE, 'DD-MON-RRRR') THEN A.IS_VISITED ELSE 'X' END
                               END
                              ), 
                              A.REMARKS
                          ) LT
                          ON LT.CUSTOMER_CODE = DM.DISTRIBUTOR_CODE AND LT.COMPANY_CODE = RM.COMPANY_CODE
                WHERE 1 = 1
                --AND TO_CHAR(RD.ASSIGN_DATE, 'DD-MON-RRRR') = UPPER(''23-MAY-2017'')
                AND RD.EMP_CODE = '{model.spcode}'
                AND RD.COMPANY_CODE = '{model.COMPANY_CODE}'
                GROUP BY 
                  DM.DISTRIBUTOR_CODE,
                  TRIM(CS.CUSTOMER_EDESC),
                  TRIM(CS.ACC_CODE),
                  DM.CONTACT_NO, NVL(DM.REG_OFFICE_ADDRESS, CS.REGD_OFFICE_EADDRESS),
                  RM.ROUTE_CODE, RM.ROUTE_NAME,
                  AM.AREA_CODE, AM.AREA_NAME,
                  RE.ORDER_NO,
                  RM.COMPANY_CODE,
                  'dealer',
                  CS.PARTY_TYPE_CODE,
                  '',
                  '',
                  NVL(DM.LATITUDE,0), NVL(DM.LONGITUDE,0),
                  LT.LAST_VISIT_DATE, LT.LAST_VISIT_BY, NVL(LT.LAST_VISIT_STATUS, 'X'), NVL(LT.IS_VISITED, 'X'), LT.REMARKS
                ORDER BY UPPER(TRIM(CS.CUSTOMER_EDESC)) ASC";

            var data = dbContext.SqlQuery<SPEntityModel>(Query).ToList();
            return data;
        }

        public Dictionary<string, List<PurchaseOrderResponseModel>> FetchPOStatus(PurchaseOrderRequestModel model, NeoErpCoreEntity dbContext)
        {
            var date = dbContext.SqlQuery<DateTime>($"SELECT FY_START_DATE FROM PREFERENCE_SETUP WHERE COMPANY_CODE = '{model.COMPANY_CODE}'").FirstOrDefault();
            var fromDate = date.ToString("dd-MMM-yyyy");
            var endDate = DateTime.Now.ToString("dd-MMM-yyyy");

            string Query = string.Empty;
            if (model.type.ToUpper() == "P" || model.type.ToUpper() == "DEALER")
            {
                Query = $@"SELECT A.DEALER_CODE AS CODE, TO_CHAR(C.ORDER_NO) ORDER_NO, TO_CHAR(C.ORDER_DATE, 'DD-MON-YYYY') AS ORDER_DATE, 
                    C.ITEM_CODE, B.ITEM_EDESC, C.MU_CODE, TO_CHAR(C.QUANTITY) QUANTITY, TO_CHAR(C.UNIT_PRICE) UNIT_PRICE, TO_CHAR(C.TOTAL_PRICE) TOTAL_PRICE, C.REMARKS, C.APPROVED_FLAG, 
                    C.DISPATCH_FLAG, C.ACKNOWLEDGE_FLAG, C.REJECT_FLAG, TO_CHAR(NVL(IR.SALES_RATE, 0)) SALES_RATE, TO_CHAR(IR.APPLY_DATE) APPLY_DATE
                    FROM DIST_DEALER_MASTER A, IP_ITEM_MASTER_SETUP B, DIST_IP_SSD_PURCHASE_ORDER C,
                    (SELECT A.ITEM_CODE, A.APPLY_DATE, B.SALES_RATE, B.COMPANY_CODE
                              FROM (SELECT ITEM_CODE, COMPANY_CODE, MAX(APP_DATE) APPLY_DATE 
                                FROM IP_ITEM_RATE_APPLICAT_SETUP
                                WHERE COMPANY_CODE = '{model.COMPANY_CODE}' 
                                AND BRANCH_CODE = '{model.BRANCH_CODE}'
                                GROUP BY ITEM_CODE, COMPANY_CODE) A
                              INNER JOIN IP_ITEM_RATE_APPLICAT_SETUP B
                                ON B.ITEM_CODE = A.ITEM_CODE
                                AND B.APP_DATE = A.APPLY_DATE
                                AND B.COMPANY_CODE = '{model.COMPANY_CODE}'
                                AND B.BRANCH_CODE = '{model.BRANCH_CODE}') IR
                    WHERE A.DEALER_CODE = '{model.code}'
                    AND A.DEALER_CODE = C.PARTY_TYPE_CODE
                    AND B.ITEM_CODE = C.ITEM_CODE
                    AND B.GROUP_SKU_FLAG = '{GROUP_SKU_FLAG}'
                    AND B.CATEGORY_CODE = '$CATEGORY_CODE'
                    AND TRUNC(C.ORDER_DATE) BETWEEN TO_DATE('{fromDate}','DD-MON-RRRR') AND TO_DATE('{endDate}','DD-MON-RRRR')
                    AND A.COMPANY_CODE = '{model.COMPANY_CODE}'
                    AND A.COMPANY_CODE = B.COMPANY_CODE
                    AND B.COMPANY_CODE = C.COMPANY_CODE
                    AND IR.ITEM_CODE(+) = C.ITEM_CODE AND IR.COMPANY_CODE(+) = C.COMPANY_CODE
                    ORDER BY C.ORDER_NO DESC";
            }
            else if (model.type.ToUpper() == "D" || model.type.ToUpper() == "DISTRIBUTOR")
            {
                Query = $@"SELECT A.DISTRIBUTOR_CODE AS CODE, TO_CHAR(C.ORDER_NO) ORDER_NO, TO_CHAR(C.ORDER_DATE, 'DD-MON-YYYY') AS ORDER_DATE, 
                    C.ITEM_CODE, B.ITEM_EDESC, C.MU_CODE, TO_CHAR(C.QUANTITY) QUANTITY, TO_CHAR(C.UNIT_PRICE) UNIT_PRICE, TO_CHAR(C.TOTAL_PRICE) TOTAL_PRICE, C.REMARKS, C.APPROVED_FLAG, 
                    C.DISPATCH_FLAG, C.ACKNOWLEDGE_FLAG, C.REJECT_FLAG, TO_CHAR(NVL(IR.SALES_RATE, 0)) SALES_RATE, TO_CHAR(IR.APPLY_DATE) APPLY_DATE
                    FROM DIST_DISTRIBUTOR_MASTER A, IP_ITEM_MASTER_SETUP B, DIST_IP_SSD_PURCHASE_ORDER C,
                    (SELECT A.ITEM_CODE, A.APPLY_DATE, B.SALES_RATE, B.COMPANY_CODE
                              FROM (SELECT ITEM_CODE, COMPANY_CODE, MAX(APP_DATE) APPLY_DATE 
                                FROM IP_ITEM_RATE_APPLICAT_SETUP
                                WHERE COMPANY_CODE = '{model.COMPANY_CODE}' 
                                AND BRANCH_CODE = '{model.BRANCH_CODE}'
                                GROUP BY ITEM_CODE, COMPANY_CODE) A
                              INNER JOIN IP_ITEM_RATE_APPLICAT_SETUP B
                                ON B.ITEM_CODE = A.ITEM_CODE
                                AND B.APP_DATE = A.APPLY_DATE
                                AND B.COMPANY_CODE = '{model.COMPANY_CODE}'
                                AND B.BRANCH_CODE = '{model.BRANCH_CODE}') IR
                    WHERE A.DISTRIBUTOR_CODE = '{model.code}'
                    AND A.DISTRIBUTOR_CODE = C.CUSTOMER_CODE
                    AND B.ITEM_CODE = C.ITEM_CODE
                    AND B.GROUP_SKU_FLAG = '{GROUP_SKU_FLAG}'
                    AND B.CATEGORY_CODE = '{CATEGORY_CODE}'
                    AND TRUNC(C.ORDER_DATE) BETWEEN TO_DATE('{fromDate}','DD-MON-RRRR') AND TO_DATE('{endDate}','DD-MON-RRRR')
                    AND A.COMPANY_CODE = '{model.COMPANY_CODE}'
                    AND A.COMPANY_CODE = B.COMPANY_CODE
                    AND B.COMPANY_CODE = C.COMPANY_CODE
                    AND IR.ITEM_CODE(+) = C.ITEM_CODE AND IR.COMPANY_CODE(+) = C.COMPANY_CODE
                    ORDER BY C.ORDER_NO DESC";
            }
            else if (model.type.ToUpper() == "R" || model.type.ToUpper() == "RESELLER")
            {
                Query = $@"SELECT A.RESELLER_CODE AS CODE, TO_CHAR(C.ORDER_NO) ORDER_NO, TO_CHAR(C.ORDER_DATE, 'DD-MON-YYYY') AS ORDER_DATE, 
                    C.ITEM_CODE, B.ITEM_EDESC, C.MU_CODE, TO_CHAR(C.QUANTITY) QUANTITY, TO_CHAR(C.UNIT_PRICE) UNIT_PRICE, TO_CHAR(C.TOTAL_PRICE) TOTAL_PRICE, C.REMARKS, C.APPROVED_FLAG, 
                    C.DISPATCH_FLAG, C.ACKNOWLEDGE_FLAG, C.REJECT_FLAG, TO_CHAR(NVL(IR.SALES_RATE, 0)) SALES_RATE, TO_CHAR(IR.APPLY_DATE) APPLY_DATE
                    FROM DIST_RESELLER_MASTER A, IP_ITEM_MASTER_SETUP B, DIST_IP_SSR_PURCHASE_ORDER C,
                    (SELECT A.ITEM_CODE, A.APPLY_DATE, B.SALES_RATE, B.COMPANY_CODE
                              FROM (SELECT ITEM_CODE, COMPANY_CODE, MAX(APP_DATE) APPLY_DATE 
                                FROM IP_ITEM_RATE_APPLICAT_SETUP
                                WHERE COMPANY_CODE = '{model.COMPANY_CODE}' 
                                AND BRANCH_CODE = '{model.BRANCH_CODE}'
                                GROUP BY ITEM_CODE, COMPANY_CODE) A
                              INNER JOIN IP_ITEM_RATE_APPLICAT_SETUP B
                                ON B.ITEM_CODE = A.ITEM_CODE
                                AND B.APP_DATE = A.APPLY_DATE
                                AND B.COMPANY_CODE = '{model.COMPANY_CODE}'
                                AND B.BRANCH_CODE = '{model.BRANCH_CODE}') IR
                    WHERE A.RESELLER_CODE = '{model.code}'
                    AND A.RESELLER_CODE = C.RESELLER_CODE
                    AND B.ITEM_CODE = C.ITEM_CODE
                    AND B.GROUP_SKU_FLAG = '{GROUP_SKU_FLAG}'
                    AND B.CATEGORY_CODE = '{CATEGORY_CODE}'
                    AND TRUNC(C.ORDER_DATE) BETWEEN TO_DATE('{fromDate}','DD-MON-RRRR') AND TO_DATE('{endDate}','DD-MON-RRRR')
                    AND A.COMPANY_CODE = '{model.COMPANY_CODE}'
                    AND A.COMPANY_CODE = B.COMPANY_CODE
                    AND B.COMPANY_CODE = C.COMPANY_CODE
                    AND IR.ITEM_CODE(+) = C.ITEM_CODE AND IR.COMPANY_CODE(+) = C.COMPANY_CODE
                    ORDER BY C.ORDER_NO DESC";
            }
            else
                throw new Exception("Invalid type");
            var data = dbContext.SqlQuery<PurchaseOrderResponseModel>(Query).ToList();
            var groups = data.GroupBy(x => x.ORDER_NO);
            var result = new Dictionary<string, List<PurchaseOrderResponseModel>>();
            foreach (var group in groups)
            {
                result.Add(group.Key, group.ToList());
            }
            return result;
        }

        public List<ImageCategoryModel> FetchImageCategory(CommonRequestModel model, NeoErpCoreEntity dbContext)
        {
            string Query = $@"SELECT CATEGORYID,CATEGORY_CODE,CATEGORY_EDESC,MAX_ITEMS,COMPANY_CODE FROM DIST_IMAGE_CATEGORY WHERE COMPANY_CODE='{model.COMPANY_CODE}'";
            var data = dbContext.SqlQuery<ImageCategoryModel>(Query).ToList();
            return data;
        }

        public List<ResellerEntityModel> FetchResellerEntity(EntityRequestModel model, NeoErpCoreEntity dbContext)
        {
            var data = dbContext.SqlQuery<ResellerEntityModel>($"SELECT RESELLER_CODE,ENTITY_CODE,ENTITY_TYPE,COMPANY_CODE FROM DIST_RESELLER_ENTITY WHERE DELETED_FLAG='N' AND COMPANY_CODE='{model.COMPANY_CODE}' AND RESELLER_CODE='{model.entity_code}'").ToList();
            return data;
        }

        public List<DistributorItemModel> FetchDistributorItems(EntityRequestModel model, NeoErpCoreEntity dbContext)
        {
            var data = dbContext.SqlQuery<DistributorItemModel>($"SELECT DISTRIBUTOR_CODE,ITEM_CODE,COMPANY_CODE FROM DIST_DISTRIBUTOR_ITEM WHERE DELETED_FLAG='N' AND COMPANY_CODE='{model.COMPANY_CODE}' AND DISTRIBUTOR_CODE='{model.entity_code}'").ToList();
            return data;
        }

        public List<ResellerGroupModel> GetResellerGroups(CommonRequestModel model, NeoErpCoreEntity dbContext)
        {
            string Query = $"SELECT GROUPID,GROUP_EDESC,GROUP_CODE FROM DIST_GROUP_MASTER WHERE DELETED_FLAG='N' AND COMPANY_CODE='{model.COMPANY_CODE}' ORDER BY TRIM(GROUP_EDESC) ASC";
            var list = dbContext.SqlQuery<ResellerGroupModel>(Query).ToList();
            return list;
        }

        public List<ContractModel> GetContracts(CommonRequestModel model, NeoErpCoreEntity dbContext)
        {
            var query = $@"SELECT CON.CONTRACT_CODE,CON.SUPPLIER_CODE,SS.SUPPLIER_EDESC,CON.CUSTOMER_CODE,CS.CUSTOMER_EDESC,CON.CONTRACT_EDESC,
                                    CON.BRAND_CODE,CON.BRANDING_TYPE,CON.SPROVIDER_CODE,CON.START_DATE,CON.END_DATE,CON.AREA_CODE,CON.CONTRACT_TYPE,
                                    CON.AMOUNT_TYPE,CON.AMOUNT,CON.NEXT_PAYMENT_DATE,CON.PAYMENT_DATE,CON.ADVANCE_AMOUNT,CON.CONTRACTOR_NAME,
                                    CON.CONTRACTOR_ADDRESS,CON.CONTRACTOR_EMAIL,CON.CONTRACTOR_PHONE,CON.CONTRACTOR_MOBILE,CON.CONTRACTOR_DESIGNATION,
                                    CON.CONTRACTOR_PAN_NO,CON.CONTRACTOR_VAT_NO,CON.OWNER_NAME,CON.OWNER_ADDRESS,CON.OWNER_PHONE,CON.OWNER_MOBILE,
                                    CON.OWNER_COMPANY_NAME,CON.OWNER_PAN_NO,CON.OWNER_VAT_NO,CON.JOB_ORDER_NO,CON.DESCRIPTION,CON.REMARKS,CON.COMPANY_CODE,CON.BRANCH_CODE
                        FROM BRD_CONTRACT CON
                        JOIN SA_CUSTOMER_SETUP CS ON CON.CUSTOMER_CODE=CS.CUSTOMER_CODE AND CON.COMPANY_CODE=CS.COMPANY_CODE
                        JOIN IP_SUPPLIER_SETUP SS ON CON.SUPPLIER_CODE=SS.SUPPLIER_CODE AND CON.COMPANY_CODE=SS.COMPANY_CODE
                        WHERE SYSDATE BETWEEN CON.START_DATE AND CON.END_DATE
                                AND CON.AMOUNT_TYPE='SCHEME_ITEM'
                                AND CON.APPROVED_FLAG='Y'
                                AND CON.DELETED_FLAG='N'
                                AND CON.COMPANY_CODE='{model.COMPANY_CODE}'
                                AND CON.BRANCH_CODE='{model.BRANCH_CODE}'";
            var data = dbContext.SqlQuery<ContractModel>(query).ToList();
            return data;
        }


        public List<AchievementReportResponseModel> GetAchievementData(AchievementReportRequestModel model, NeoErpCoreEntity dbContext)
        {
            string subquery = String.Empty;
            string MTDfilter = String.Empty;
            string filter = String.Empty;
            string SUBfilter = String.Empty;

            if (model.REPORT_TYPE.ToUpper() == "MTD")
            {

                int month = DateTime.Now.Month;
                int year = DateTime.Now.Year;
                int day = DateTime.Now.Day;
                DateConverter converter = DateConverter.ConvertToNepali(year, month, day); //converting english date to nepali date
                string monthName = converter.MonthName;
                MTDfilter = $@"HAVING FN_BS_MONTH (SUBSTR (NEPALI_MONTH, 5, 2))='{monthName}'";


            }
            else if (model.REPORT_TYPE.ToUpper() == "YTD")
            {
                filter = $@"AND DT.PLAN_DATE < trunc(sysdate)";


                if (model.TYPE.ToUpper() == "D")
                {
                    SUBfilter = $@"AND A.SALES_DATE < trunc(sysdate)";

                }
                else if (model.TYPE.ToUpper() == "R")
                {
                    SUBfilter = $@"AND A.ORDER_DATE < trunc(sysdate)";

                }
            }

            if (model.TYPE.ToUpper() == "D")
            {
                subquery = $@"SELECT A.CUSTOMER_CODE,A.COMPANY_CODE,B.BRAND_NAME,C.ITEM_EDESC,TO_NUMBER(REPLACE (SUBSTR (BS_DATE (A.SALES_DATE), 0, 7), '-')) NEPALI_MONTH,
                              0 TARGET_QUANTITY,0 TARGET_VALUE,SUM(A.QUANTITY) AS QUANTITY_ACHIVE, SUM (A.CALC_TOTAL_PRICE) ACHIVE_VALUE
                          FROM SA_SALES_INVOICE A, IP_ITEM_SPEC_SETUP B, IP_ITEM_MASTER_SETUP C
                          WHERE A.ITEM_CODE = B.ITEM_CODE(+)
                              AND A.COMPANY_CODE = B.COMPANY_CODE
                              AND B.ITEM_CODE = C.ITEM_CODE
                               AND B.COMPANY_CODE = C.COMPANY_CODE
                              AND A.DELETED_FLAG = 'N'
                              AND A.CUSTOMER_CODE = '{model.SP_CODE}'
                              AND A.COMPANY_CODE = '{model.COMPANY_CODE}'
                              AND A.BRANCH_CODE='{model.BRANCH_CODE}'
                               {SUBfilter}
                      GROUP BY A.CUSTOMER_CODE,A.COMPANY_CODE,B.BRAND_NAME,TO_NUMBER(REPLACE(SUBSTR(BS_DATE(A.SALES_DATE), 0, 7), '-')),C.ITEM_EDESC";
            }
            else if (model.TYPE.ToUpper() == "R")
            {
                subquery = $@"SELECT A.CUSTOMER_CODE,A.COMPANY_CODE,B.BRAND_NAME,C.ITEM_EDESC,TO_NUMBER(REPLACE (SUBSTR (BS_DATE (TRUNC(A.ORDER_DATE)), 0, 7), '-')) NEPALI_MONTH,
                            0 TARGET_QUANTITY,0 TARGET_VALUE,SUM (A.QUANTITY) AS QUANTITY_ACHIVE,SUM (A.TOTAL_PRICE) ACHIVE_VALUE
                        FROM DIST_IP_SSR_PURCHASE_ORDER A, IP_ITEM_SPEC_SETUP B, IP_ITEM_MASTER_SETUP C
                        WHERE A.ITEM_CODE = B.ITEM_CODE(+)
                            AND A.COMPANY_CODE = B.COMPANY_CODE
                           AND B.ITEM_CODE = C.ITEM_CODE
                             AND B.COMPANY_CODE=C.COMPANY_CODE
                            AND A.DELETED_FLAG = 'N'
                            AND A.CUSTOMER_CODE =  '{model.SP_CODE}'
                            AND A.COMPANY_CODE = '{model.COMPANY_CODE}'
                            AND A.BRANCH_CODE='{model.BRANCH_CODE}'
                            {SUBfilter}
                    GROUP BY A.CUSTOMER_CODE,A.COMPANY_CODE,B.BRAND_NAME,TO_NUMBER(REPLACE (SUBSTR (BS_DATE (TRUNC(A.ORDER_DATE)), 0, 7), '-')),C.ITEM_EDESC";
            }


            string query = $@"  SELECT CUSTOMER_CODE,BRAND_NAME,ITEM_EDESC,NEPALI_MONTH,
                    FN_BS_MONTH (SUBSTR (NEPALI_MONTH, 5, 2)) AS NEPALI_MONTHINT,
                    ROUND(SUM(TARGET_QUANTITY),0) TARGET_QUANTITY,
                    SUM(TARGET_VALUE) TARGET_VALUE,
                    ROUND(SUM(QUANTITY_ACHIVE),0) QUANTITY_ACHIVE,
                    SUM(ACHIVE_VALUE) ACHIVE_VALUE
                FROM (SELECT DT.CUSTOMER_CODE,DT.COMPANY_CODE,B.BRAND_NAME,C.ITEM_EDESC,
                              TO_NUMBER(REPLACE (SUBSTR (BS_DATE (DT.PLAN_DATE), 0, 7), '-')) NEPALI_MONTH,
                              SUM (DT.PER_DAY_QUANTITY) AS TARGET_QUANTITY,
                              SUM (DT.PER_DAY_AMOUNT) TARGET_VALUE,0 QUANTITY_ACHIVE,0 ACHIVE_VALUE
                          FROM PL_SALES_PLAN_DTL DT, IP_ITEM_SPEC_SETUP B, IP_ITEM_MASTER_SETUP C
                          WHERE DT.ITEM_CODE = B.ITEM_CODE(+)
                              AND DT.COMPANY_CODE = B.COMPANY_CODE
                              AND DT.COMPANY_CODE = C.COMPANY_CODE
                              AND B.ITEM_CODE = C.ITEM_CODE
                              AND DT.DELETED_FLAG = 'N'
                              AND DT.CUSTOMER_CODE = '{model.SP_CODE}'
                              AND DT.COMPANY_CODE = '{model.COMPANY_CODE}'
                              AND DT.BRANCH_CODE='{model.BRANCH_CODE}'
                              AND C.GROUP_SKU_FLAG = 'I' 
                              {filter}
                      GROUP BY DT.CUSTOMER_CODE,DT.COMPANY_CODE,TO_NUMBER(REPLACE (SUBSTR (BS_DATE (PLAN_DATE), 0, 7), '-')),
                              DT.CUSTOMER_CODE,B.BRAND_NAME,C.ITEM_EDESC
                      UNION ALL
                        {subquery})
            GROUP BY NEPALI_MONTH,CUSTOMER_CODE,BRAND_NAME,ITEM_EDESC {MTDfilter}
            ORDER BY NEPALI_MONTH";
            var list = dbContext.SqlQuery<AchievementReportResponseModel>(query).ToList();
            return list;
        }
        public List<AchievementReportResponseModel> fetchAchievementReportMonthWise(AchievementReportRequestModel model, NeoErpCoreEntity dbContext)
        {

            string query = $@"  SELECT CUSTOMER_CODE,BRAND_NAME,ITEM_EDESC,NEPALI_MONTH,
                    FN_BS_MONTH (SUBSTR (NEPALI_MONTH, 5, 2)) AS NEPALI_MONTHINT,
                    ROUND(SUM(TARGET_QUANTITY),0) TARGET_QUANTITY,
                    SUM(TARGET_VALUE) TARGET_VALUE,
                    ROUND(SUM(QUANTITY_ACHIVE),0) QUANTITY_ACHIVE,
                    SUM(ACHIVE_VALUE) ACHIVE_VALUE
                FROM (SELECT DT.CUSTOMER_CODE,DT.COMPANY_CODE,B.BRAND_NAME,C.ITEM_EDESC,
                              TO_NUMBER(REPLACE (SUBSTR (BS_DATE (DT.PLAN_DATE), 0, 7), '-')) NEPALI_MONTH,
                              SUM (DT.PER_DAY_QUANTITY) AS TARGET_QUANTITY,
                              SUM (DT.PER_DAY_AMOUNT) TARGET_VALUE,0 QUANTITY_ACHIVE,0 ACHIVE_VALUE
                          FROM PL_SALES_PLAN_DTL DT, IP_ITEM_SPEC_SETUP B, IP_ITEM_MASTER_SETUP C
                          WHERE DT.ITEM_CODE = B.ITEM_CODE(+)
                              AND DT.COMPANY_CODE = B.COMPANY_CODE
                              AND DT.COMPANY_CODE = C.COMPANY_CODE
                              AND B.ITEM_CODE = C.ITEM_CODE
                              AND DT.DELETED_FLAG = 'N'
                              AND DT.CUSTOMER_CODE = '{model.SP_CODE}'
                              AND DT.COMPANY_CODE = '{model.COMPANY_CODE}'
                              AND DT.BRANCH_CODE='{model.BRANCH_CODE}'                              
                              AND C.GROUP_SKU_FLAG = 'I'
                      GROUP BY DT.CUSTOMER_CODE,DT.COMPANY_CODE,TO_NUMBER(REPLACE (SUBSTR (BS_DATE (PLAN_DATE), 0, 7), '-')),
                              DT.CUSTOMER_CODE,B.BRAND_NAME,C.ITEM_EDESC
                      UNION ALL
                        SELECT A.CUSTOMER_CODE,A.COMPANY_CODE,B.BRAND_NAME,C.ITEM_EDESC,TO_NUMBER(REPLACE (SUBSTR (BS_DATE (A.SALES_DATE), 0, 7), '-')) NEPALI_MONTH,
                              0 TARGET_QUANTITY,0 TARGET_VALUE,SUM(A.QUANTITY) AS QUANTITY_ACHIVE, SUM (A.CALC_TOTAL_PRICE) ACHIVE_VALUE
                          FROM SA_SALES_INVOICE A, IP_ITEM_SPEC_SETUP B, IP_ITEM_MASTER_SETUP C
                          WHERE A.ITEM_CODE = B.ITEM_CODE(+)
                              AND A.COMPANY_CODE = B.COMPANY_CODE
                              AND B.ITEM_CODE = C.ITEM_CODE
                               AND B.COMPANY_CODE = C.COMPANY_CODE
                              AND A.DELETED_FLAG = 'N'
                              AND A.CUSTOMER_CODE = '{model.SP_CODE}'
                              AND A.COMPANY_CODE = '{model.COMPANY_CODE}'
                              AND A.BRANCH_CODE='{model.BRANCH_CODE}'                              
                      GROUP BY A.CUSTOMER_CODE,A.COMPANY_CODE,B.BRAND_NAME,TO_NUMBER(REPLACE(SUBSTR(BS_DATE(A.SALES_DATE), 0, 7), '-')),C.ITEM_EDESC)
            GROUP BY NEPALI_MONTH,CUSTOMER_CODE,BRAND_NAME,ITEM_EDESC
            ORDER BY NEPALI_MONTH";
            var list = dbContext.SqlQuery<AchievementReportResponseModel>(query).ToList();
            return list;
        }
        public Dictionary<string, object> synCalenderData(ProfileDetailsModel model, NeoErpCoreEntity dbContext)
        {
            var result = new Dictionary<string, object>();
            try
            {
                // Fetch the calendar data and attendance status
                var calendarData = FetchCalendarData(model, dbContext);
                var attendanceStatus = FetchAttendanceStatus(model, dbContext);

                // Add the data to the dictionary
                result.Add("Data", calendarData ?? new List<CalenderDataModel>()); // Ensures non-null response
                result.Add("Status", attendanceStatus ?? new List<string>());      // Ensures non-null response
            }
            catch (Exception ex)
            {
                result.Add("Data", new List<CalenderDataModel>());
                result.Add("Status", new List<string>());
            }

            return result;
        }
        public List<AttendanceCountModel> AttendanceCountData(ProfileDetailsModel model, NeoErpCoreEntity dbContext)
        {
            var query = $@"SELECT 
                    COUNT(CASE WHEN D.overall_status = 'PR' OR D.overall_status = 'WD' OR  D.overall_status = 'WH' OR D.overall_status = 'LA' OR D.overall_status = 'TN' OR  D.overall_status = 'BA' OR D.overall_status = 'TR' OR D.overall_status = 'TV' THEN 1 END) AS PRESENT,
                    COUNT(CASE WHEN D.overall_status = 'AB' THEN 1 END) AS ABSENT,
                    COUNT(CASE WHEN D.late_status = 'L' OR D.late_status = 'Y'  OR D.late_status = 'B' THEN 1 END) AS LATE_IN,
                    COUNT(CASE WHEN D.late_status = 'E' OR D.late_status = 'B' THEN 1 END) AS EARLY_OUT,
                    COUNT(CASE WHEN D.overall_status = 'LV' THEN 1 END) AS LEAVE,
                    COUNT(CASE WHEN D.overall_status = 'TV' THEN 1 END) AS TRAVEL,
                    COUNT(CASE WHEN D.overall_status = 'TN' THEN 1 END) AS TRAINING,
                    COUNT(CASE WHEN D.overall_status = 'WD' THEN 1 END) AS WORKON_DAYOFF,
                    COUNT(CASE WHEN D.overall_status = 'WH' THEN 1 END) AS WORKON_HOLIDAY,
                    COUNT(CASE WHEN D.overall_status = 'LA' THEN 1 END) AS LATE_PENALTY,
                    COUNT(CASE WHEN D.late_status='X' OR D.late_status='Y' THEN 1 END) AS MISSED_PUNCH
                FROM HRIS_ATTENDANCE_DETAIL D 
                LEFT JOIN 
                        HRIS_EMPLOYEES E ON (D.EMPLOYEE_ID=E.EMPLOYEE_ID)
                LEFT JOIN 
                        HRIS_COMPANY C ON E.COMPANY_ID = C.COMPANY_ID
                WHERE D.attendance_dt BETWEEN TO_DATE('{model.start_date}', 'DD-MON-YYYY') AND TO_DATE('{model.end_date}', 'DD-MON-YYYY')
                    AND E.STATUS='E' AND E.RESIGNED_FLAG='N' AND D.EMPLOYEE_ID={model.sp_code}
                    AND C.company_code in '{model.company_code}'";
            var list = dbContext.SqlQuery<AttendanceCountModel>(query).ToList();
            return list;
        }
        public object fetchSalesVsCollectionData(ProfileDetailsModel model, NeoErpCoreEntity dbContext)
        {
            var dateData = dbContext.SqlQuery<DateModel>($"select startdate as START_DATE, enddate as END_DATE from v_date_range where rangename='This Year'").ToList();
            if (string.IsNullOrEmpty(model.start_date))
            {
                model.start_date = dateData[0].START_DATE.ToString("dd-MMM-yyyy");
            }
            if (string.IsNullOrEmpty(model.end_date))
            {
                model.end_date = dateData[0].END_DATE.ToString("dd-MMM-yyyy");
            }
            string branchCode = model.branch_code;
            string formatedBranchCodes = branchCode.Replace("[", "'").Replace("]", "'").Replace(", ", "','");
            string company_code = model.company_code;
            string formatedCompanyCodes = company_code.Replace("[", "'").Replace("]", "'").Replace(", ", "','");
            try
            {
                string query = $@"SELECT DISTINCT E.CUSTOMER_CODE, E.CUSTOMER_EDESC,E.master_customer_code as MASTER_CODE,E.pre_customer_code AS PRE_CODE,E.group_sku_flag, round(NVL(E.NET_SALES_VALUE, 0),2) AS NET_SALES_VALUE, round(NVL(E.COLLECTION, 0),2) AS COLLECTION, round(NVL(E.OPENING, 0),2) AS OPENING,E.COMPANY_CODE,(LENGTH (e.MASTER_CUSTOMER_CODE) - LENGTH (REPLACE (e.MASTER_CUSTOMER_CODE, '.', ''))) AS ROW_LEVEL
                FROM (SELECT * FROM ( SELECT  A.CUSTOMER_CODE,A.CUSTOMER_EDESC, A.master_customer_code,A.pre_customer_code,A.group_sku_flag,A.company_code FROM SA_CUSTOMER_SETUP A WHERE 
                 A.COMPANY_CODE IN ({formatedCompanyCodes}, '0') AND A.deleted_flag = 'N'  AND A.group_sku_flag = 'I'  GROUP BY  A.CUSTOMER_CODE, A.CUSTOMER_EDESC, A.master_customer_code, A.pre_customer_code,A.group_sku_flag,
                 A.company_code ) A LEFT OUTER JOIN ( SELECT  COD,NET_SALES_VALUE FROM (SELECT cus_name,Cod, SALES_ROLL_QTY,SALES_QTY,SALES_VALUE,SALES_RET_ROLL_QTY,SALES_RET_QTY,
                 SALES_RET_VALUE,SALES_ROLL_QTY - SALES_RET_ROLL_QTY AS NET_ROLL_QTY, SALES_QTY - SALES_RET_QTY AS NET_SALES_QTY, SALES_VALUE - SALES_RET_VALUE AS NET_SALES_VALUE 
                FROM (SELECT  D.CUS_name, D.cod,SUM(A.SALES_ROLL_QTY) AS SALES_ROLL_QTY, SUM(A.SALES_QTY) AS SALES_QTY, SUM(A.SALES_VALUE) AS SALES_VALUE, SUM(SALES_RET_ROLL_QTY) AS SALES_RET_ROLL_QTY, 
                SUM(A.SALES_RET_QTY) AS SALES_RET_QTY, SUM(A.SALES_RET_VALUE) AS SALES_RET_VALUE FROM ( SELECT  A.COMPANY_CODE,A.BRANCH_CODE,A.customer_code, SUM(NVL(A.ROLL_QTY, 0)) AS SALES_ROLL_QTY, 
                SUM(NVL(A.QUANTITY, 0)) AS SALES_QTY,SUM(NVL(A.QUANTITY * A.NET_sales_RATE, 0)) AS SALES_VALUE,0 AS SALES_RET_ROLL_QTY,0 AS SALES_RET_QTY,0 AS SALES_RET_VALUE  FROM SA_SALES_INVOICE A 
                WHERE  A.DELETED_FLAG = 'N' AND A.COMPANY_CODE IN ({formatedCompanyCodes}, '0') AND A.BRANCH_CODE IN (SELECT sbc.branch_code  FROM SC_BRANCH_CONTROL  sbc,sc_application_users sau WHERE
                sbc.user_no = sau.user_no and sbc.company_code=sau.company_code and sau.employee_code='{model.sp_code}' AND sbc.company_code IN ({formatedCompanyCodes}, '0'))
                AND TRUNC(A.SALES_DATE) BETWEEN '{model.start_date}' AND '{model.end_date}'  GROUP BY   A.COMPANY_CODE, A.BRANCH_CODE, A.customer_CODE
                UNION ALL
                SELECT  A.COMPANY_CODE, A.BRANCH_CODE, A.customer_code, 0 AS SALES_ROLL_QTY, 0 AS SALES_QTY,0 AS SALES_VALUE, SUM(NVL(A.ROLL_QTY, 0)) AS SALES_RET_ROLL_QTY,SUM(NVL(A.QUANTITY, 0)) AS SALES_RET_QTY,
                SUM(NVL(A.QUANTITY * A.NET_sales_RATE, 0)) AS SALES_RET_VALUE FROM SA_SALES_RETURN A WHERE  A.DELETED_FLAG = 'N' AND A.COMPANY_CODE IN ({formatedCompanyCodes}, '0') AND A.BRANCH_CODE IN (
                SELECT sbc.branch_code  FROM SC_BRANCH_CONTROL  sbc,sc_application_users sau WHERE sbc.user_no = sau.user_no and sbc.company_code=sau.company_code and sau.employee_code='{model.sp_code}' AND sbc.company_code IN ({formatedCompanyCodes}, '0') ) AND TRUNC(A.RETURN_DATE) BETWEEN '{model.start_date}' AND '{model.end_date}'
                GROUP BY  A.COMPANY_CODE,  A.BRANCH_CODE, A.customer_CODE  ) A  LEFT OUTER JOIN (SELECT customer_code AS cod,customer_edesc AS cus_name  FROM SA_CUSTOMER_SETUP  WHERE COMPANY_CODE IN ({formatedCompanyCodes}, '0') 
                GROUP BY customer_code, customer_edesc ) D ON D.cod = A.CUSTOMER_CODE GROUP BY   D.cod,D.cus_name)) ORDER BY Cus_name) B ON A.customer_code = B.cod
                LEFT OUTER JOIN ( SELECT  sub_code, NVL(SUM(NVL(CR_AMOUNT, 0) * NVL(EXCHANGE_RATE, 1)), 0) AS collection FROM V$VIRTUAL_SUB_LEDGER b WHERE (COMPANY_CODE, Voucher_NO) IN (
                SELECT  COMPANY_CODE, A.voucher_no FROM V$VIRTUAL_GENERAL_LEDGER A WHERE A.ACC_CODE IN ( SELECT  ACC_CODE FROM FA_CHART_OF_ACCOUNTS_SETUP WHERE ACC_NATURE IN ('AB', 'AC', 'LC')
                AND COMPANY_CODE = A.COMPANY_CODE ) AND A.TRANSACTION_TYPE = 'DR' AND COMPANY_CODE IN ({formatedCompanyCodes}, '0') AND BRANCH_CODE IN (SELECT sbc.branch_code  FROM SC_BRANCH_CONTROL  sbc,sc_application_users sau WHERE
                sbc.user_no = sau.user_no and sbc.company_code=sau.company_code and sau.employee_code='{model.sp_code}' AND sbc.company_code IN ({formatedCompanyCodes}, '0') ) AND TRUNC(voucher_DATE) BETWEEN '{model.start_date}' AND '{model.end_date}' AND A.DELETED_FLAG = 'N' AND A.Voucher_NO != '0') AND SUBSTR(sub_code, 1, 1) = 'C'
                AND TRANSACTION_TYPE = 'CR' GROUP BY sub_code) C ON 'C' || A.customer_code = C.SUB_CODE LEFT OUTER JOIN (SELECT  sub_code AS D_CODE,NVL(SUM(dr_amount) - SUM(cr_amount), 0) AS OPENING
                FROM V$VIRTUAL_SUB_LEDGEr WHERE company_code IN ({formatedCompanyCodes}, '0') AND deleted_flag = 'N' AND (form_code = '0' OR voucher_date < '{model.start_date}')  AND SUBSTR(sub_code, 1, 1) = 'C' GROUP BY sub_code
                ) D ON 'C' || A.customer_code = D.D_CODE) E LEFT JOIN DIST_USER_AREAS dua ON dua.company_code = E.company_code and dua.customer_code=E.customer_code where dua.sp_code = '{model.sp_code}' and 
                dua.company_code in ({formatedCompanyCodes}) ORDER BY 2, 1";
                var list = dbContext.SqlQuery<SalesVsCollectionModel>(query).ToList();
                var cus = dbContext.SqlQuery<SalesVsCollectionModel>($@"SELECT 
                                        customer_code,
                                        CUSTOMER_EDESC,
                                        GROUP_SKU_FLAG,
                                        MASTER_CUSTOMER_CODE as MASTER_CODE,
                                        PRE_CUSTOMER_CODE AS PRE_CODE,
                                        (LENGTH (MASTER_CUSTOMER_CODE) - LENGTH (REPLACE (MASTER_CUSTOMER_CODE, '.', ''))) AS ROW_LEVEL
                                    FROM SA_CUSTOMER_SETUP 
                                    WHERE 1=1 and company_code in ({formatedCompanyCodes})  AND deleted_flag = 'N' AND GROUP_SKU_FLAG = 'G'
                                    GROUP BY CUSTOMER_CODE, CUSTOMER_EDESC, GROUP_SKU_FLAG, MASTER_CUSTOMER_CODE, PRE_CUSTOMER_CODE 
                                    ORDER BY MASTER_CUSTOMER_CODE").ToList();
                var finalList = new List<SalesVsCollectionModel>();

                foreach (var parent in cus.Where(x => x.GROUP_SKU_FLAG == "G"))
                {
                    string masterCode = parent.MASTER_CODE ?? "";

                    decimal totOpening = 0;
                    decimal totCollection = 0;
                    decimal totNetSales = 0;

                    bool found = false;
                    foreach (var item in list)
                    {
                        string itemMaster = item.MASTER_CODE ?? "";

                        if (itemMaster.StartsWith(masterCode) && itemMaster != masterCode)
                        {
                            found = true;

                            totOpening += item.OPENING;
                            totCollection += item.COLLECTION;
                            totNetSales += item.NET_SALES_VALUE;
                        }
                    }
                    if (found)
                    {
                        var groupTotal = new SalesVsCollectionModel
                        {
                            CUSTOMER_CODE = parent.CUSTOMER_CODE,
                            CUSTOMER_EDESC = parent.CUSTOMER_EDESC,
                            MASTER_CODE = parent.MASTER_CODE,
                            PRE_CODE = parent.PRE_CODE,
                            GROUP_SKU_FLAG = "G",
                            ROW_LEVEL = parent.ROW_LEVEL,

                            OPENING = totOpening,
                            COLLECTION = totCollection,
                            NET_SALES_VALUE = totNetSales,

                            COMPANY_CODE = parent.COMPANY_CODE
                        };

                        finalList.Add(groupTotal);
                    }
                    finalList.Add(parent);

                    foreach (var item in list.Where(x =>
                        x.MASTER_CODE.StartsWith(masterCode) &&
                        x.PRE_CODE == masterCode))
                    {
                        finalList.Add(item);
                    }
                }
                list = finalList.Where(row =>
                {
                    decimal opening = Convert.ToDecimal(row.OPENING);
                    decimal sales = Convert.ToDecimal(row.NET_SALES_VALUE);
                    decimal collection = Convert.ToDecimal(row.COLLECTION);

                    return opening != 0 || sales != 0 || collection != 0;
                }).ToList();
                var itemSum = new SalesVsCollectionModel
                {
                    CUSTOMER_CODE = "",
                    CUSTOMER_EDESC = "Total",
                    GROUP_SKU_FLAG = "",
                    OPENING = list.Sum(x => x.OPENING),
                    COLLECTION = list.Sum(x => x.COLLECTION),
                    NET_SALES_VALUE = list.Sum(x => x.NET_SALES_VALUE),
                    COMPANY_CODE = ""
                };

                var data = new { data = list, total = itemSum };
                return data;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
        public object fetchMonthlySalesCollectionData(UserDetailsModel model, NeoErpCoreEntity dbContext)
        {
            var dateData = dbContext.SqlQuery<DateModel>($"select startdate as START_DATE, enddate as END_DATE from v_date_range where rangename='This Year'").ToList();

            model.START_DT = dateData[0].START_DATE.ToString("dd-MMM-yyyy");
            model.END_DT = dateData[0].END_DATE.ToString("dd-MMM-yyyy");
            string branchCode = model.BRANCH_CODE;
            string company_code = model.COMPANY_CODE;

            string formatedBranchCodes;
            if (branchCode.Contains("[") && branchCode.Contains("]"))
            {
                formatedBranchCodes = branchCode.Replace("[", "'").Replace("]", "'").Replace(", ", "','");
            }
            else
            {
                formatedBranchCodes = $"'{branchCode}'";
            }

            string formatedCompanyCodes;
            if (company_code.Contains("[") && company_code.Contains("]"))
            {
                formatedCompanyCodes = company_code.Replace("[", "'").Replace("]", "'").Replace(", ", "','");
            }
            else
            {
                formatedCompanyCodes = $"'{company_code}'";
            }

            try
            {
                string query = $@"SELECT rangename as MONTH_NAME, nvl(CAST(opening AS DECIMAL(18, 2)),0) as OPENING, nvl(CAST(sales AS DECIMAL(18, 2)),0) as SALES_VALUE, nvl(CAST(collection AS DECIMAL(18, 2)), 0) as COLLECTION
            FROM (
                SELECT * FROM (
                    SELECT rangename FROM v_date_range WHERE sortorder = 1 AND startdate <= trunc(sysdate) ORDER BY startdate
                ) a LEFT OUTER JOIN (
                    SELECT rangename rs, val sales FROM (
                        SELECT rangename, SUM(sales_qty - sales_ret_qty), SUM(sales_value - sales_ret_value) val 
                        FROM (
                            SELECT * FROM (SELECT a.company_code, a.branch_code, a.sales_date, 
                                SUM(nvl(a.roll_qty, 0)) sales_roll_qty, SUM(nvl(a.quantity, 0)) sales_qty,
                                SUM(nvl(a.quantity * a.net_sales_rate, 0)) sales_value, 0 sales_ret_roll_qty,
                                0 sales_ret_qty, 0 sales_ret_value FROM sa_sales_invoice a WHERE a.deleted_flag = 'N' 
                                AND a.customer_code='{model.CUSTOMER_CODE}' AND a.branch_code in ({formatedBranchCodes}) AND a.company_code IN ({formatedCompanyCodes}) 
                                AND trunc(a.sales_date) BETWEEN '{model.START_DT}' AND '{model.END_DT}' GROUP BY a.company_code, a.branch_code, a.sales_date 
                                UNION ALL
                                SELECT a.company_code, a.branch_code, a.return_date, 0 sales_roll_qty, 0 sales_qty, 
                                0 sales_value, SUM(nvl(a.roll_qty, 0)) sales_ret_roll_qty, SUM(nvl(a.quantity, 0)) sales_ret_qty,
                                SUM(nvl(a.quantity * a.net_sales_rate, 0)) sales_ret_value FROM sa_sales_return a WHERE 
                                a.deleted_flag = 'N' and a.customer_code='{model.CUSTOMER_CODE}' AND a.company_code IN ({formatedCompanyCodes}) 
                                AND a.branch_code in ({formatedBranchCodes}) AND trunc(a.return_date) BETWEEN '{model.START_DT}' AND '{model.END_DT}' 
                                GROUP BY a.company_code, a.branch_code, a.return_date ORDER BY sales_date 
                        ) a LEFT OUTER JOIN (SELECT * FROM v_date_range WHERE sortorder = '1') b ON a.sales_date BETWEEN b.startdate AND b.enddate
                    ) WHERE rangename IS NOT NULL GROUP BY startdate, rangename)
                ) b ON a.rangename = b.rs LEFT OUTER JOIN (
                    SELECT rangename ms, SUM(amt) collection FROM (
                        SELECT * FROM (
                            SELECT voucher_date, nvl(SUM(nvl(cr_amount, 0) * nvl(exchange_rate, 1)), 0) amt 
                            FROM v$virtual_sub_ledger b WHERE (company_code, voucher_no) IN (
                                SELECT company_code, a.voucher_no FROM v$virtual_general_ledger a WHERE 
                                a.acc_code IN (SELECT acc_code FROM fa_chart_of_accounts_setup WHERE acc_nature IN ('AB', 'AC', 'LC') 
                                AND company_code = a.company_code) AND a.transaction_type = 'DR' AND a.company_code IN ({formatedCompanyCodes}) 
                                AND a.branch_code in ({formatedBranchCodes}) AND a.deleted_flag = 'N' AND a.voucher_no != '0'
                            ) and sub_code='C{model.CUSTOMER_CODE}' AND substr(sub_code, 1, 1) = 'C' AND transaction_type = 'CR' 
                            GROUP BY voucher_date 
                        ) a LEFT OUTER JOIN (SELECT * FROM v_date_range WHERE sortorder = '1') b ON a.voucher_date BETWEEN b.startdate AND b.enddate
                    ) WHERE rangename IS NOT NULL GROUP BY startdate, rangename
                ) c ON a.rangename = c.ms LEFT OUTER JOIN (
                    SELECT CASE WHEN mahina = 'Shrawan' THEN 'Bhadra' WHEN mahina = 'Bhadra' THEN 'Ashoj' 
                        WHEN mahina = 'Ashoj' THEN 'Kartik' WHEN mahina = 'Kartik' THEN 'Mangsir' 
                        WHEN mahina = 'Mangsir' THEN 'Poush' WHEN mahina = 'Poush' THEN 'Magh' 
                        WHEN mahina = 'Magh' THEN 'Falgun' WHEN mahina = 'Falgun' THEN 'Chaitra' 
                        WHEN mahina = 'Chaitra' THEN 'Baishakh' WHEN mahina = 'Baishakh' THEN 'Jestha' 
                        WHEN mahina = 'Jestha' THEN 'Ashadh' ELSE 'undefined' END mahina, 
                        SUM(balance) as opening, startdate FROM (
                        SELECT acc_code, company_code, b.startdate, b.rangename mahina, CASE WHEN (
                            SELECT transaction_type FROM fa_chart_of_accounts_setup WHERE acc_code = a.acc_code 
                            AND company_code = a.company_code) = 'DR' THEN SUM(nvl(dr_amount, 0) - nvl(cr_amount, 0)) 
                            ELSE SUM(nvl(cr_amount, 0) - nvl(dr_amount, 0)) END balance 
                        FROM v$virtual_general_ledger a, v_date_range b WHERE voucher_date <= enddate and voucher_no in (
                            select voucher_no from V$VIRTUAL_SUB_LEDGER where company_code in ({formatedCompanyCodes}) and sub_code='C{model.CUSTOMER_CODE}' 
                            and form_code!=0 ) AND sortorder = 1 AND enddate <= (SELECT enddate FROM v_date_range WHERE enddate > (
                                SELECT MAX(voucher_date) FROM v$virtual_general_ledger WHERE company_code IN ({formatedCompanyCodes}) 
                                AND trunc(voucher_date) <= '{model.END_DT}' ) AND ROWNUM = 1) AND company_code IN ({formatedCompanyCodes}) 
                                AND deleted_flag = 'N' AND acc_code IN (SELECT acc_code FROM fa_chart_of_accounts_setup WHERE 1 = 1 
                                AND company_code IN ({formatedCompanyCodes}) AND deleted_flag = 'N' AND acc_nature IN ('AE') ) 
                        GROUP BY acc_code, b.startdate, b.rangename, company_code
                    ) GROUP BY mahina, startdate
                ) d ON a.rangename = d.mahina)";
                var list = dbContext.SqlQuery<MonthlySalesVsCollectionModel>(query).ToList();
                Debug.WriteLine("reaches here?");

                var itemSum = new MonthlySalesVsCollectionModel
                {
                    MONTH_NAME = "Total",
                    OPENING = list.Sum(x => x.OPENING),
                    COLLECTION = list.Sum(x => x.COLLECTION),
                    SALES_VALUE = list.Sum(x => x.SALES_VALUE)
                };
                var data = new { data = list, total = itemSum };
                return data;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public List<ClosingStockDtlModel> fetchLatestClosingStock(ClosingStockModel model, NeoErpCoreEntity dbContext)
        {
            var list = new List<ClosingStockDtlModel>();
            if (!string.IsNullOrEmpty(model.reseller_code))
            {
                string query1 = $@"select * from (SELECT drs.item_code,drs.current_stock lvs,drs.created_date as LVS_DATE,drs.company_code,drs.branch_code,drs.reseller_code as code,
                ROW_NUMBER() OVER( PARTITION BY drs.item_code ORDER BY drs.created_date DESC ) AS rn
                FROM dist_reseller_stock     drs,dist_reseller_entity    dse,dist_distributor_item   ddi WHERE
                drs.company_code = '{model.company_code}' AND drs.company_code = dse.company_code AND drs.reseller_code = dse.reseller_code AND dse.deleted_flag = 'N' and ddi.deleted_flag='N'
                AND drs.branch_code = dse.branch_code AND dse.entity_code = ddi.distributor_code AND dse.company_code = ddi.company_code AND drs.item_code = ddi.item_code AND drs.reseller_code IN ('{model.reseller_code}') AND drs.branch_code = '{model.branch_code}') sub where rn=1";
                list = dbContext.SqlQuery<ClosingStockDtlModel>(query1).ToList();

            }
            else if (!string.IsNullOrEmpty(model.distributor_code))
            {
                string query2 = $@"SELECT * FROM (SELECT dds.item_code,dds.current_stock as lvs,dds.created_date as LVS_DATE,dds.company_code,dds.branch_code,dds.distributor_code as code,ROW_NUMBER() OVER (PARTITION BY dds.item_code ORDER BY dds.created_date DESC) AS rn
                FROM dist_distributor_stock dds,dist_distributor_item uim WHERE dds.company_code=uim.company_code and dds.distributor_code=uim.distributor_code and dds.item_code=uim.item_code and 
                dds.company_code = '{model.company_code}' and uim.deleted_flag='N'  AND dds.distributor_code IN ('{model.distributor_code}') AND dds.branch_code = '{model.branch_code}') sub WHERE rn = 1";
                list = dbContext.SqlQuery<ClosingStockDtlModel>(query2).ToList();
            }
            //var list = dbContext.SqlQuery<ClosingStockDtlModel>(query).ToList();
            return list;
        }
        public Dictionary<string, object> SynProfileData(ProfileDetailsModel model, NeoErpCoreEntity dbContext)
        {
            var result = new Dictionary<string, object>();
            dbContext.ExecuteSqlCommand(@"
                BEGIN
                   SP_INSERT_DAILY_TARGET;
                END;
            ");
            #region Dailywise 
            if (model.data == "TODAY")
            {
                try
                {
                    model.start_date = DateTime.Today.ToString("dd-MMM-yyyy");
                    model.end_date = DateTime.Today.ToString("dd-MMM-yyyy");
                    // Adding TODAY data
                    var todayData = new List<object>
        {
                     new { name = "Visit_Target", data = FetchPlanVisitedDataSafe(model, dbContext) },
                    //new { name = "Collection_Target", data = FetchCollectionDataSafe(model, dbContext) },
                    };

                    result["TODAY"] = todayData;  // Directly assigning the list
                }
                catch (Exception ex)
                {
                    result["TODAY"] = new object[] { new { result = new object[] { }, response = false, error = ex.Message } };
                }
                #endregion Dailywise

            }

            #region Monthly 
            else if (model.data == "MTD")
            {
                try
                {
                    var dateData = dbContext.SqlQuery<DateModel>($"select startdate as START_DATE, enddate as END_DATE from v_date_range where rangename='This Month'").ToList();

                    model.start_date = dateData[0].START_DATE.ToString("dd-MMM-yyyy");
                    model.end_date = dateData[0].END_DATE.ToString("dd-MMM-yyyy");

                    var monthlyData = new List<object>
                {
                    new { name = "Visit_Target", data = FetchPlanVisitedDataSafe(model, dbContext) },
                    //new { name = "Collection_Target", data = FetchCollectionDataSafe(model, dbContext) },

                };

                    result["MTD"] = monthlyData;  // Directly assigning the list
                }
                catch (Exception ex)
                {
                    result["MTD"] = new object[] { new { result = new object[] { }, response = false, error = ex.Message } };
                }
            }
            #endregion Monthly
            #region Yearly 
            else if (model.data == "YTD")
            {
                try
                {
                    var dateData = dbContext.SqlQuery<DateModel>($"select startdate as START_DATE, enddate as END_DATE from v_date_range where rangename='This Year'").ToList();

                    model.start_date = dateData[0].START_DATE.ToString("dd-MMM-yyyy");
                    model.end_date = dateData[0].END_DATE.ToString("dd-MMM-yyyy");

                    var yearlyData = new List<object>
                {
                    new { name = "Visit_Target", data = FetchPlanVisitedDataSafe(model, dbContext) },
                    //new { name = "Collection_Target", data = FetchCollectionDataSafe(model, dbContext) },
                };

                    result["YTD"] = yearlyData;  // Directly assigning the list
                }
                catch (Exception ex)
                {
                    result["YTD"] = new object[] { new { result = new object[] { }, response = false, error = ex.Message } };
                }
            }
            #endregion Yearly
            return result;
        }
        public Dictionary<string, object> SynProductQuantityData(ProfileDetailsModel model, NeoErpCoreEntity dbContext)
        {
            var result = new Dictionary<string, object>();
            dbContext.ExecuteSqlCommand(@"
                BEGIN
                   SP_INSERT_DAILY_TARGET;
                END;
            ");
            #region Dailywise 
            if (model.data == "TODAY")
            {
                try
                {
                    model.start_date = DateTime.Today.ToString("dd-MMM-yyyy");
                    model.end_date = DateTime.Today.ToString("dd-MMM-yyyy");

                    result["TODAY"] = FetchProductWiseSafe(model, dbContext);  // Directly assigning the list
                }
                catch (Exception ex)
                {
                    result["TODAY"] = new object[] { new { result = new object[] { }, response = false, error = ex.Message } };
                }
            }
            #endregion Dailywise

            #region Monthly 
            else if (model.data == "MTD")
            {
                try
                {
                    var dateData = dbContext.SqlQuery<DateModel>($"select startdate as START_DATE, enddate as END_DATE from v_date_range where rangename='This Month'").ToList();

                    model.start_date = dateData[0].START_DATE.ToString("dd-MMM-yyyy");
                    model.end_date = dateData[0].END_DATE.ToString("dd-MMM-yyyy");

                    result["MTD"] = FetchProductWiseSafe(model, dbContext);  // Directly assigning the list
                }
                catch (Exception ex)
                {
                    result["MTD"] = new object[] { new { result = new object[] { }, response = false, error = ex.Message } };
                }
            }
            #endregion Monthly
            #region Yearly 
            else if (model.data == "YTD")
            {
                try
                {
                    var dateData = dbContext.SqlQuery<DateModel>($"select startdate as START_DATE, enddate as END_DATE from v_date_range where rangename='This Year'").ToList();

                    model.start_date = dateData[0].START_DATE.ToString("dd-MMM-yyyy");
                    model.end_date = dateData[0].END_DATE.ToString("dd-MMM-yyyy");

                    result["YTD"] = FetchProductWiseSafe(model, dbContext);  // Directly assigning the list
                }
                catch (Exception ex)
                {
                    result["YTD"] = new object[] { new { result = new object[] { }, response = false, error = ex.Message } };
                }
            }
            #endregion Yearly

            return result;
        }
        public Dictionary<string, object> SynAreaCustomerData(ProfileDetailsModel model, NeoErpCoreEntity dbContext)
        {
            var result = new Dictionary<string, object>();
            dbContext.ExecuteSqlCommand(@"
                BEGIN
                   SP_INSERT_DAILY_TARGET;
                END;
            ");
            #region Dailywise 
            if (model.data == "TODAY")
            {
                try
                {
                    model.start_date = DateTime.Today.ToString("dd-MMM-yyyy");
                    model.end_date = DateTime.Today.ToString("dd-MMM-yyyy");
                    result["TODAY"] = FetchAreaWiseSafe(model, dbContext); ;  // Directly assigning the list
                }
                catch (Exception ex)
                {
                    result["TODAY"] = new object[] { new { result = new object[] { }, response = false, error = ex.Message } };
                }
            }
            #endregion Dailywise
            #region Monthly 
            else if (model.data == "MTD")
            {
                try
                {
                    var dateData = dbContext.SqlQuery<DateModel>($"select startdate as START_DATE, enddate as END_DATE from v_date_range where rangename='This Month'").ToList();

                    model.start_date = dateData[0].START_DATE.ToString("dd-MMM-yyyy");
                    model.end_date = dateData[0].END_DATE.ToString("dd-MMM-yyyy");
                    result["MTD"] = FetchAreaWiseSafe(model, dbContext); ;  // Directly assigning the list
                }
                catch (Exception ex)
                {
                    result["MTD"] = new object[] { new { result = new object[] { }, response = false, error = ex.Message } };
                }
            }
            #endregion Monthly
            #region Yearly 
            else if (model.data == "YTD")
            {
                try
                {
                    var dateData = dbContext.SqlQuery<DateModel>($"select startdate as START_DATE, enddate as END_DATE from v_date_range where rangename='This Year'").ToList();

                    model.start_date = dateData[0].START_DATE.ToString("dd-MMM-yyyy");
                    model.end_date = dateData[0].END_DATE.ToString("dd-MMM-yyyy");
                    result["YTD"] = FetchAreaWiseSafe(model, dbContext);  // Directly assigning the list
                }
                catch (Exception ex)
                {
                    result["YTD"] = new object[] { new { result = new object[] { }, response = false, error = ex.Message } };
                }
            }
            #endregion Yearly

            return result;
        }
        private IEnumerable<object> FetchPlanVisitedDataSafe(ProfileDetailsModel model, NeoErpCoreEntity dbContext)
        {
            try { return FetchPlanVisitedData(model, dbContext); }
            catch { return new object[] { }; }
        }

        private IEnumerable<object> FetchCollectionDataSafe(ProfileDetailsModel model, NeoErpCoreEntity dbContext)
        {
            try { return FetchCollectionData(model, dbContext); }
            catch { return new object[] { }; }
        }
        private object FetchProductWiseSafe(ProfileDetailsModel model, NeoErpCoreEntity dbContext)
        {
            try { return FetchProductWise(model, dbContext); }
            catch { return new object[] { }; }
        }

        private object FetchAreaWiseSafe(ProfileDetailsModel model, NeoErpCoreEntity dbContext)
        {
            try { return FetchAreaWise(model, dbContext); }
            catch { return new object[] { }; }
        }
        public object FetchAreaWise(ProfileDetailsModel model, NeoErpCoreEntity dbContext)
        {
            try
            {
                string query = $@"
                        WITH target_data AS (
                            SELECT 
                                '{model.company_code}' company_code,
                                (select userid from dist_login_user where sp_code = a.employee_code and company_code = '{model.company_code}') as assign_employee,
                                a.DISTIBUTOR_CODE AS customer_code,
                                round(SUM(NVL(a.amount, 0)), 2) AS target_amount,
                                round(SUM(NVL(a.qty, 0)), 2) AS target_quantity
                            FROM daily_target_setup a
                            WHERE a.employee_code = '{model.sp_code}'
                              AND TRUNC(a.ASSIGNDATE) BETWEEN TO_DATE('{model.start_date}','DD-MON-YY') 
                                                         AND TO_DATE('{model.end_date}','DD-MON-YY')
                              --AND a.company_code = '{model.company_code}'
                           GROUP BY A.EMPLOYEE_CODE, A.DISTIBUTOR_CODE
                        ),
                        ssd_achieved AS (
                            SELECT 
                                a.company_code,
                                u.userid AS assign_employee,
                                a.customer_code,
                                round(SUM(a.quantity), 2) AS quantity_achieved,
                                round(SUM(a.total_price), 2) AS amount_achieved
                            FROM dist_ip_ssd_purchase_order a
                            JOIN dist_login_user u ON a.created_by = u.userid
                            WHERE a.deleted_flag = 'N'
                              AND u.active = 'Y'
                              AND u.userid = '{model.userId}'
                              AND TRUNC(a.order_date) BETWEEN TO_DATE('{model.start_date}','DD-MON-YY') 
                                                         AND TO_DATE('{model.end_date}','DD-MON-YY')
                              AND a.company_code = '{model.company_code}'
                            GROUP BY a.company_code, u.userid, a.customer_code
                        ),
                        ssr_achieved AS (
                            SELECT 
                                a.company_code,
                                u.userid AS assign_employee,
                                a.customer_code,
                                round(SUM(a.quantity), 2) AS quantity_achieved,
                                round(SUM(a.total_price), 2) AS amount_achieved
                            FROM dist_ip_ssr_purchase_order a
                            JOIN dist_login_user u ON a.created_by = u.userid
                            WHERE a.deleted_flag = 'N'
                              AND u.active = 'Y'
                              AND u.userid = '{model.userId}'
                              AND TRUNC(a.order_date) BETWEEN TO_DATE('{model.start_date}','DD-MON-YY') 
                                                         AND TO_DATE('{model.end_date}','DD-MON-YY')
                              AND a.company_code = '{model.company_code}'
                            GROUP BY a.company_code, u.userid, a.customer_code
                        ),
                        combined_achieved AS (
                            SELECT
                                COALESCE(s.company_code, r.company_code) AS company_code,
                                COALESCE(s.assign_employee, r.assign_employee) AS assign_employee,
                                COALESCE(s.customer_code, r.customer_code) AS customer_code,
                                NVL(s.quantity_achieved,0) + NVL(r.quantity_achieved,0) AS quantity_achieved,
                                NVL(s.amount_achieved,0) + NVL(r.amount_achieved,0) AS amount_achieved
                            FROM ssd_achieved s
                            FULL OUTER JOIN ssr_achieved r
                               ON s.company_code = r.company_code
                              AND s.assign_employee = r.assign_employee
                              AND s.customer_code = r.customer_code
                        )
                        SELECT 
                            t.target_amount,
                            t.target_quantity,
                            NVL(c.quantity_achieved, 0) AS quantity_achieved,
                            NVL(c.amount_achieved, 0) AS amount_achieved,
                            b.area_code,
                            d.area_name,
                            t.customer_code,
                            e.customer_edesc
                        FROM target_data t
                        LEFT JOIN combined_achieved c
                               ON c.company_code = t.company_code
                              AND c.assign_employee = t.assign_employee
                              AND c.customer_code = t.customer_code
                        LEFT JOIN dist_distributor_master b
                               ON t.company_code = b.company_code
                              AND t.customer_code = b.distributor_code
                        LEFT JOIN dist_area_master d
                               ON b.area_code = d.area_code
                              AND b.company_code = d.company_code
                        LEFT JOIN sa_customer_setup e
                               ON t.company_code = e.company_code
                              AND t.customer_code = e.customer_code
                        ORDER BY b.area_code, t.customer_code";


                //    string query = $@"SELECT NVL(a.target_amount, 0) AS TARGET_AMOUNT,NVL(a.target_quantity, 0) AS TARGET_QUANTITY,0 AS QUANTITY_ACHIEVED,0 AS AMOUNT_ACHIEVED,CASE WHEN b.area_code IS NULL THEN 'not_defined' ELSE b.area_code END AS area_code, CASE  WHEN b.area_code IS NULL THEN 'not_defined' ELSE c.area_name END AS name,
                //CASE WHEN a.master_code IS NULL THEN 'not_defined' ELSE a.master_code END AS customer_code, CASE WHEN a.master_code IS NULL THEN 'not_defined' ELSE d.customer_edesc END AS customer_edesc
                //FROM ip_target_setup a,dist_distributor_master b,dist_area_master c,sa_customer_setup  d 
                //WHERE a.company_code = b.company_code AND a.master_code = b.distributor_code AND b.area_code = c.area_code AND b.company_code = c.company_code AND a.master_code = d.customer_code AND c.company_code = d.company_code
                //AND a.target_type = 'SAL' and a.deleted_flag='N' and b.deleted_flag='N'  and c.deleted_flag='N'
                //AND a.sub_target_type = 'CUS' AND a.assign_employee = '{model.sp_code}' AND a.deleted_flag = 'N' AND trunc(a.from_date) BETWEEN TO_DATE('{model.start_date}', 'DD-MON-YY') AND TO_DATE('{model.end_date}', 'DD-MON-YY') AND a.company_code in ('{model.company_code}')
                //UNION ALL
                //SELECT 0 as TARGET_QUANTITY,0 as TARGET_AMOUNT,NVL(a.quantity, 0) AS QUANTITY_ACHIEVED,NVL(a.total_price, 0) AS AMOUNT_ACHIEVED,
                //CASE  WHEN b.area_code IS NULL THEN 'not_defined' ELSE b.area_code END AS area_code, CASE WHEN b.area_code IS NULL THEN 'not_defined' ELSE d.area_name END AS name,CASE WHEN a.customer_code IS NULL THEN 'not_defined' ELSE a.customer_code END AS customer_code,CASE WHEN a.customer_code IS NULL THEN 'not_defined' ELSE f.customer_edesc END AS customer_edesc
                //FROM dist_ip_ssd_purchase_order a,dist_distributor_master b,dist_login_user c,dist_area_master d,(SELECT DISTINCT area_code, sp_code, company_code FROM dist_user_areas where deleted_flag='N') e,sa_customer_setup f
                //WHERE a.company_code = b.company_code AND a.customer_code = b.distributor_code AND a.company_code = c.company_code AND a.created_by = c.userid and e.sp_code=c.sp_code and a.customer_code=f.customer_code AND a.company_code = f.company_code
                //AND c.sp_code = e.sp_code  AND c.company_code = e.company_code AND b.area_code = e.area_code AND b.area_code = d.area_code AND b.company_code = d.company_code
                //AND trunc(a.order_date) BETWEEN TO_DATE('{model.start_date}', 'DD-MON-YY')  AND TO_DATE('{model.end_date}', 'DD-MON-YY') 
                //AND a.company_code in ('{model.company_code}') AND a.deleted_flag = 'N' AND f.deleted_flag = 'N' AND b.deleted_flag = 'N'  AND c.active = 'Y' AND d.deleted_flag = 'N'  AND c.sp_code = '{model.sp_code}'
                //UNION ALL
                //SELECT 0 as TARGET_QUANTITY,0 as TARGET_AMOUNT,nvl(a.quantity, 0) QUANTITY_ACHIEVED, nvl(total_price, 0) AMOUNT_ACHIEVED,
                //CASE WHEN b.area_code IS NULL THEN 'not_defined' ELSE b.area_code  END area_code, CASE WHEN b.area_code IS NULL THEN 'not_defined' ELSE d.area_name END AS name,CASE WHEN a.customer_code IS NULL THEN 'not_defined' ELSE a.customer_code END AS customer_code,CASE WHEN a.customer_code IS NULL THEN 'not_defined' ELSE f.customer_edesc END AS customer_edesc
                //FROM dist_ip_ssr_purchase_order a, dist_distributor_master b,dist_login_user c,dist_area_master d,(SELECT DISTINCT area_code, sp_code, company_code FROM dist_user_areas where deleted_flag='N') e,sa_customer_setup f
                //WHERE a.company_code = b.company_code AND a.customer_code = b.distributor_code  AND a.company_code = c.company_code AND a.created_by = c.userid and a.customer_code=f.customer_code AND a.company_code = f.company_code
                //AND c.sp_code = e.sp_code AND c.company_code = e.company_code AND b.area_code = e.area_code AND b.area_code = d.area_code and e.sp_code=c.sp_code AND f.deleted_flag = 'N'
                //AND b.company_code = d.company_code AND a.company_code in ('{model.company_code}') AND a.deleted_flag = 'N' AND b.deleted_flag = 'N'  AND c.active = 'Y' AND d.deleted_flag = 'N'
                //AND c.sp_code = '{model.sp_code}' AND trunc(a.order_date) BETWEEN TO_DATE('{model.start_date}', 'DD-MON-YY')  AND TO_DATE('{model.end_date}', 'DD-MON-YY')";
                var result = dbContext.SqlQuery<AreaCusWiseModel>(query).ToList();
                if (result.Count <= 0)
                {
                    var dat = new List<object>
                {
                    //new { name = "Area_Target", data = new List<AreaWiseModel>(),total = new AreaWiseModel { NAME = "Total", TARGET_QUANTITY = 0, QUANTITY_ACHIEVED = 0, TARGET_AMOUNT = 0, AMOUNT_ACHIEVED = 0 } },
                    new { name = "Customer_Target", data = new List<CustomerWiseModel>(),total = new CustomerWiseModel { NAME = "Total", TARGET_QUANTITY = 0, QUANTITY_ACHIEVED = 0, TARGET_AMOUNT = 0, AMOUNT_ACHIEVED = 0 } }
                };
                    return dat;
                }
                var areaData = result
              .GroupBy(x => new { x.AREA_CODE, x.NAME })
              .Select(g => new AreaWiseModel
              {
                  NAME = g.Key.NAME,
                  TARGET_QUANTITY = g.Sum(x => x.TARGET_QUANTITY),
                  TARGET_AMOUNT = g.Sum(x => x.TARGET_AMOUNT),
                  QUANTITY_ACHIEVED = g.Sum(x => x.QUANTITY_ACHIEVED),
                  AMOUNT_ACHIEVED = g.Sum(x => x.AMOUNT_ACHIEVED)
              }).OrderBy(g => (g.QUANTITY_ACHIEVED + g.AMOUNT_ACHIEVED) > 0 ? 1 : 2).ThenBy(g => g.NAME).ToList();
                var areaSum = new AreaWiseModel
                {
                    NAME = "Total",
                    TARGET_QUANTITY = areaData.Sum(x => x.TARGET_QUANTITY),
                    TARGET_AMOUNT = areaData.Sum(x => x.TARGET_AMOUNT),
                    QUANTITY_ACHIEVED = areaData.Sum(x => x.QUANTITY_ACHIEVED),
                    AMOUNT_ACHIEVED = areaData.Sum(x => x.AMOUNT_ACHIEVED)
                };
                var customerData = result
              .GroupBy(x => new { x.CUSTOMER_CODE, x.CUSTOMER_EDESC })
              .Select(g => new CustomerWiseModel
              {
                  NAME = g.Key.CUSTOMER_EDESC,
                  TARGET_QUANTITY = g.Sum(x => x.TARGET_QUANTITY),
                  TARGET_AMOUNT = g.Sum(x => x.TARGET_AMOUNT),
                  QUANTITY_ACHIEVED = g.Sum(x => x.QUANTITY_ACHIEVED),
                  AMOUNT_ACHIEVED = g.Sum(x => x.AMOUNT_ACHIEVED)
              }).OrderBy(g => (g.QUANTITY_ACHIEVED + g.AMOUNT_ACHIEVED) > 0 ? 1 : 2).ThenBy(g => g.NAME).ToList();
                var customerSum = new CustomerWiseModel
                {
                    NAME = "Total",
                    TARGET_QUANTITY = customerData.Sum(x => x.TARGET_QUANTITY),
                    TARGET_AMOUNT = customerData.Sum(x => x.TARGET_AMOUNT),
                    QUANTITY_ACHIEVED = customerData.Sum(x => x.QUANTITY_ACHIEVED),
                    AMOUNT_ACHIEVED = customerData.Sum(x => x.AMOUNT_ACHIEVED)
                };
                var data = new List<object>
                {
                    //new { name = "Area_Target", data =areaData,total= areaSum},
                    new { name = "Customer_Target", data = customerData,total=customerSum }
                };
                return (data);
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
        public List<CollectionModel> FetchCollectionData(ProfileDetailsModel model, NeoErpCoreEntity dbContext)
        {
            string query = $@"SELECT 'Collection_Target' AS name,nvl(target_amount,0) AS target_amount,0 AS amount_achieved
            FROM ip_target_setup WHERE assign_employee = '{model.sp_code}' AND target_type = 'COL' AND company_code in ('{model.company_code}') 
            AND trunc(from_date) BETWEEN TO_DATE('{model.start_date}', 'DD-MON-YYYY') AND TO_DATE('{model.end_date}', 'DD-MON-YYYY')
            union all 
            SELECT 'Collection_Target' AS name, 0 AS target_amount,nvl(amount,0) AS amount_achieved
            FROM dist_collection WHERE sp_code = '{model.sp_code}' AND company_code in ('{model.company_code}') 
            AND trunc(created_date) BETWEEN TO_DATE('{model.start_date}', 'DD-MON-YYYY') AND TO_DATE('{model.end_date}', 'DD-MON-YYYY')";
            var result = dbContext.SqlQuery<CollectionModel>(query).ToList();
            var groupedData = result
            .GroupBy(x => new { x.NAME })
            .Select(g => new CollectionModel
            {
                NAME = g.Key.NAME,
                TARGET_AMOUNT = g.Sum(x => x.TARGET_AMOUNT),
                AMOUNT_ACHIEVED = g.Sum(x => x.AMOUNT_ACHIEVED),
            }).ToList();
            if (result.Count <= 0)
            {
                groupedData.Add(new CollectionModel
                {
                    NAME = "Collection_Target",
                    TARGET_AMOUNT = 0,
                    AMOUNT_ACHIEVED = 0
                });
            }
            return groupedData;
        }
        public List<VisitPlanWiseModel> FetchPlanVisitedData(ProfileDetailsModel model, NeoErpCoreEntity dbContext)
        {

            string query = $@"SELECT  t.name, T.TOTAL_TARGETS as TARGET_QUANTITY, NVL(V.TOTAL_ACHIEVED, 0) AS QUANTITY_ACHIEVED
                FROM (SELECT SP_CODE, FULL_NAME,'Plan_wise' AS name, COUNT(*) AS TOTAL_TARGETS FROM DIST_TARGET_ENTITY WHERE COMPANY_CODE = '{model.company_code}'
                AND SP_CODE = {model.sp_code} AND TRUNC(ASSIGN_DATE) BETWEEN TO_DATE('{model.start_date}', 'DD-MON-YY') AND TO_DATE('{model.end_date}', 'DD-MON-YY')  GROUP BY SP_CODE, FULL_NAME ) T
                LEFT JOIN ( SELECT SP_CODE,FULL_NAME,'plan_wise' AS name, COUNT(*) AS TOTAL_ACHIEVED FROM  DIST_VISITED_ENTITY WHERE IS_VISITED = 'Y' AND CUSTOMER_CODE  IN (SELECT 
                        ENTITY_CODE FROM  DIST_TARGET_ENTITY WHERE COMPANY_CODE ='{model.company_code}' AND SP_CODE = {model.sp_code}
                        AND TRUNC(ASSIGN_DATE) BETWEEN TO_DATE('{model.start_date}', 'DD-MON-YY') AND TO_DATE('{model.end_date}', 'DD-MON-YY')
            ) AND TRUNC(UPDATE_DATE) BETWEEN TO_DATE('{model.start_date}', 'DD-MON-YY') AND TO_DATE('{model.end_date}', 'DD-MON-YY')
                GROUP BY SP_CODE, FULL_NAME) V ON T.SP_CODE = V.SP_CODE AND t.full_name = v.full_name
                union 
                SELECT  t.name, T.TOTAL_TARGETS as TARGET_QUANTITY, NVL(V.TOTAL_ACHIEVED, 0) AS QUANTITY_ACHIEVED
                FROM (SELECT SP_CODE, FULL_NAME,'Nonplan_wise' AS name, 0 AS TOTAL_TARGETS FROM DIST_TARGET_ENTITY WHERE COMPANY_CODE = '{model.company_code}'
                AND SP_CODE = {model.sp_code} AND TRUNC(ASSIGN_DATE) BETWEEN TO_DATE('{model.start_date}', 'DD-MON-YY') AND TO_DATE('{model.end_date}', 'DD-MON-YY')  GROUP BY SP_CODE, FULL_NAME ) T
                LEFT JOIN ( SELECT SP_CODE,FULL_NAME,'nonplan_wise' AS name, COUNT(*) AS TOTAL_ACHIEVED FROM  DIST_VISITED_ENTITY WHERE IS_VISITED = 'Y' AND CUSTOMER_CODE NOT IN (SELECT 
                        ENTITY_CODE FROM  DIST_TARGET_ENTITY WHERE COMPANY_CODE ='{model.company_code}' AND SP_CODE = {model.sp_code}
                        AND TRUNC(ASSIGN_DATE) BETWEEN TO_DATE('{model.start_date}', 'DD-MON-YY') AND TO_DATE('{model.end_date}', 'DD-MON-YY')
                ) AND TRUNC(UPDATE_DATE) BETWEEN TO_DATE('{model.start_date}', 'DD-MON-YY') AND TO_DATE('{model.end_date}', 'DD-MON-YY')
                GROUP BY SP_CODE, FULL_NAME) V ON T.SP_CODE = V.SP_CODE AND t.full_name = v.full_name order by name desc";
            var result = dbContext.SqlQuery<VisitPlanWiseModel>(query).ToList();
            if (result.Count <= 0)
            {
                result.Add(new VisitPlanWiseModel
                {
                    NAME = "Plan_wise",
                    TARGET_QUANTITY = 0,
                    QUANTITY_ACHIEVED = 0
                });

                result.Add(new VisitPlanWiseModel
                {
                    NAME = "Nonplan_wise",
                    TARGET_QUANTITY = 0,
                    QUANTITY_ACHIEVED = 0
                });
            }
            return result;
        }
        public object FetchProductWise(ProfileDetailsModel model, NeoErpCoreEntity dbContext)
        {
            try
            {
                string query = $@"
        WITH target_data AS (
            SELECT 
                '{model.company_code}' company_code,
                --a.employee_code AS userId,
                (select userid from dist_login_user where sp_code = a.employee_code and company_code = '{model.company_code}') as userid,
                SUM(NVL(a.qty,0)) AS target_quantity,
                SUM(NVL(a.amount,0)) AS target_amount
            FROM daily_target_setup a
            WHERE a.employee_code = '{model.sp_code}'
              AND TRUNC(a.assigndate) between TO_DATE('{model.start_date}','DD-MON-YY')
                                         AND TO_DATE('{model.end_date}','DD-MON-YY')
            GROUP BY a.employee_code
        ),
        achieved_data AS (
            SELECT
                company_code,
                created_by AS userId,
                SUM(nvl(quantity,0)) AS quantity_achieved,
                SUM(nvl(total_price,0)) AS amount_achieved
            FROM (
                SELECT 
                    a.company_code,
                    a.created_by,
                    (NVL(a.quantity,0) + NVL(a.APPROVE_QTY,0)) AS quantity,
                    NVL(a.total_price,0) AS total_price
                FROM dist_ip_ssd_purchase_order a
                WHERE a.deleted_flag = 'N'
                  AND a.created_by = '{model.userId}'
                  AND TRUNC(a.order_date) BETWEEN TO_DATE('{model.start_date}','DD-MON-YY')
                                             AND TO_DATE('{model.end_date}','DD-MON-YY')
                  AND a.company_code = '{model.company_code}'
                UNION ALL
                SELECT 
                    a.company_code,
                    a.created_by,
                    (NVL(a.quantity,0) + NVL(a.APPROVE_QTY,0)) AS quantity,
                    NVL(a.total_price,0)
                FROM dist_ip_ssr_purchase_order a
                WHERE a.deleted_flag = 'N'
                  AND a.created_by = '{model.userId}'
                  AND TRUNC(a.order_date) BETWEEN TO_DATE('{model.start_date}','DD-MON-YY')
                                             AND TO_DATE('{model.end_date}','DD-MON-YY')
                  AND a.company_code = '{model.company_code}'
            )
            GROUP BY company_code, created_by
        )
        SELECT
            round(NVL(t.target_quantity,0), 2) AS TARGET_QUANTITY,
            round(NVL(t.target_amount,0), 2)   AS TARGET_AMOUNT,
            round(NVL(a.quantity_achieved,0), 2) AS QUANTITY_ACHIEVED,
            round(NVL(a.amount_achieved,0), 2)   AS AMOUNT_ACHIEVED,
            'Quantity Wise' AS Quantity_name,
            'Amount Wise'   AS Amount_name
        FROM achieved_data a
        FULL OUTER JOIN target_data t 
               ON a.company_code = t.company_code
              AND a.userId = t.userId";

                var result = dbContext.SqlQuery<ProductQuantityWiseModel>(query).ToList();

                if (result.Count <= 0)
                {
                    return new List<object>
            {
                new
                {
                    name = "Sales_Target",
                    data = new List<object>
                    {
                        new QuantityModel
                                {
                                    NAME = "Quantity Wise",
                                    TARGET_QUANTITY = 0,
                                    QUANTITY_ACHIEVED = 0
                                },
                            new AmountModel
                                {
                                    NAME = "Amount Wise",
                                    TARGET_AMOUNT = 0,
                                    AMOUNT_ACHIEVED = 0
                                }
                    }
                }
            };
                }

                var quantityData = result
                    .GroupBy(x => x.Quantity_name)
                    .Select(g => new QuantityModel
                    {
                        NAME = g.Key,
                        TARGET_QUANTITY = g.Sum(x => x.TARGET_QUANTITY),
                        QUANTITY_ACHIEVED = g.Sum(x => x.QUANTITY_ACHIEVED)
                    }).ToList();

                var amountData = result
                    .GroupBy(x => x.Amount_name)
                    .Select(g => new AmountModel
                    {
                        NAME = g.Key,
                        TARGET_AMOUNT = g.Sum(x => x.TARGET_AMOUNT),
                        AMOUNT_ACHIEVED = g.Sum(x => x.AMOUNT_ACHIEVED)
                    }).ToList();

                return new List<object>
        {
            new
            {
                name = "Sales_Target",
                //data = quantityData.Union(amountData).ToList()
                data = new List<object>
                {
                        quantityData.FirstOrDefault(),
                        amountData.FirstOrDefault()
                }
            }
        };
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public Dictionary<string, object> fetchProfileDetails(ProfileDetails model, NeoErpCoreEntity dbContext)
        {
            var data = new Dictionary<string, object>();
            try
            {
                var personalData = FetchPersonalDetails(model, dbContext);
                data.Add("personalData", personalData);
            }
            catch (Exception ex)
            {
                data.Add("personalData", new object[] { });
            }
            try
            {
                var planVisit = FetchTargetVisits(model, dbContext);
                var unplanVisit = FetchNonPlanVisits(model, dbContext);

                var targetVisitData = new Dictionary<string, object>
                {
                    { "planVisit", planVisit },
                    { "unplanVisit", unplanVisit }
                };

                data.Add("targetVisit", targetVisitData);
            }
            catch (Exception ex)
            {
                var targetVisitData = new Dictionary<string, object>
                {
                    { "planVisit", new object[] { } },
                    { "unplanVisit", new object[] { } }
                };

                data.Add("targetVisit", targetVisitData);
            }
            return data;
        }
        public List<PERSONAL_DETAILS> FetchPersonalDetails(ProfileDetails model, NeoErpCoreEntity dbContext)
        {
            string query = $@"select sp_code,userid,employee_edesc,contact_no,company_code,branch_code,to_char(atn_time, 'DD-MON-YYYY HH:MI:SS AM') LoginTime from (SELECT
                            a.sp_code, a.userid,a.full_name AS employee_edesc,a.contact_no,a.company_code,a.branch_code,MIN(b.submit_date) as  atn_time
                            FROM dist_login_user a left JOIN dist_lm_location_tracking b ON a.sp_code = b.sp_code and a.company_code=b.company_code and a.branch_code=b.branch_code
                            WHERE  a.sp_code = '{model.SP_CODE}' AND a.company_code = '{model.COMPANY_CODE}' AND trunc(b.submit_date) = trunc(sysdate)
                            GROUP BY  a.sp_code, a.userid,a.full_name, a.contact_no, a.company_code, a.branch_code)";
            List<PERSONAL_DETAILS> personalInfo = dbContext.SqlQuery<PERSONAL_DETAILS>(query).ToList();
            return personalInfo;
        }
        public List<PLAN_VISIT_TARGET> FetchTargetVisits(ProfileDetails model, NeoErpCoreEntity dbContext)
        {
            string query = $@"SELECT group_edesc, sp_code, employee_edesc, SUM(target) PLAN_TARGET, SUM(visited) PLAN_ACHIEVED, SUM(total_visited) visited,0 as NONPLAN_TARGET,SUM(extra) NONPLAN_ACHIEVED 
                            FROM (SELECT group_edesc, sp_code, full_name employee_edesc, trunc(assign_date) assign_date, SUM(target) target, SUM(visited) visited, nvl((SELECT COUNT(DISTINCT customer_code)
                            FROM dist_visited_entity WHERE userid = aa.userid AND company_code = aa.company_code AND trunc(update_date) = trunc(aa.assign_date)), 0) total_visited, 
                            SUM(nvl((SELECT COUNT(DISTINCT customer_code) FROM dist_visited_entity WHERE userid = aa.userid AND company_code = aa.company_code AND trunc(update_date) = trunc(aa.assign_date)), 0) - visited) extra 
                            FROM (SELECT b.group_edesc, a.userid, a.full_name, a.sp_code, b.assign_date, b.company_code, CASE WHEN wm_concat(b.entity_code) IS NULL THEN 0 ELSE nvl(COUNT(DISTINCT b.entity_code), 0) END target,
                            nvl((SELECT COUNT(DISTINCT customer_code) FROM dist_visited_entity WHERE userid = a.userid AND company_code = a.company_code AND trunc(update_date) = trunc(b.assign_date)
                            AND customer_code IN (SELECT entity_code FROM dist_target_entity WHERE userid = a.userid AND company_code = a.company_code AND route_code = b.route_code AND
                            trunc(assign_date) = trunc(b.assign_date))), 0) visited FROM dist_login_user a, dist_target_entity b WHERE a.userid = b.userid AND a.company_code = b.company_code AND
                            a.active = 'Y' AND a.company_code IN ('{model.COMPANY_CODE}') AND b.assign_date BETWEEN TO_DATE('{model.START_DATE}','DD-MON-RRRR') AND TO_DATE('{model.END_DATE}','DD-MON-RRRR') GROUP BY a.userid, 
                            a.full_name, a.sp_code, b.assign_date, a.company_code, b.route_code, b.route_name, b.group_edesc, b.company_code ORDER BY b.assign_date) aa WHERE 1 = 1
                            AND sp_code IN ('{model.SP_CODE}') GROUP BY userid, company_code, trunc(assign_date), sp_code, group_edesc, sp_code, full_name) GROUP BY sp_code, group_edesc, sp_code,
                            employee_edesc ORDER BY sp_code";
            List<PLAN_VISIT_TARGET> planVisitData = dbContext.SqlQuery<PLAN_VISIT_TARGET>(query).ToList();
            return planVisitData;
        }
        public List<NONPLAN_VISIT_TARGET> FetchNonPlanVisits(ProfileDetails model, NeoErpCoreEntity dbContext)
        {
            string query = $@"SELECT group_edesc, sp_code, employee_edesc,SUM(target) PLAN_TARGET, SUM(visited) PLAN_ACHIEVED, SUM(total_visited) visited,0 as NONPLAN_TARGET,SUM(extra) NONPLAN_ACHIEVED 
                            FROM (SELECT group_edesc, sp_code, full_name employee_edesc, trunc(assign_date) assign_date, SUM(target) target, SUM(visited) visited, nvl((SELECT COUNT(DISTINCT customer_code)
                            FROM dist_visited_entity WHERE userid = aa.userid AND company_code = aa.company_code AND trunc(update_date) = trunc(aa.assign_date)), 0) total_visited, 
                            SUM(nvl((SELECT COUNT(DISTINCT customer_code) FROM dist_visited_entity WHERE userid = aa.userid AND company_code = aa.company_code AND trunc(update_date) = trunc(aa.assign_date)), 0) - visited) extra 
                            FROM (SELECT b.group_edesc, a.userid, a.full_name, a.sp_code, b.assign_date, b.company_code, CASE WHEN wm_concat(b.entity_code) IS NULL THEN 0 ELSE nvl(COUNT(DISTINCT b.entity_code), 0) END target,
                            nvl((SELECT COUNT(DISTINCT customer_code) FROM dist_visited_entity WHERE userid = a.userid AND company_code = a.company_code AND trunc(update_date) = trunc(b.assign_date)
                            AND customer_code IN (SELECT entity_code FROM dist_target_entity WHERE userid = a.userid AND company_code = a.company_code AND route_code = b.route_code AND
                            trunc(assign_date) = trunc(b.assign_date))), 0) visited FROM dist_login_user a, dist_target_entity b WHERE a.userid = b.userid AND a.company_code = b.company_code AND
                            a.active = 'Y' AND a.company_code IN ('{model.COMPANY_CODE}') AND b.assign_date BETWEEN TO_DATE('{model.START_DATE}','DD-MON-RRRR') AND TO_DATE('{model.END_DATE}','DD-MON-RRRR') GROUP BY a.userid, 
                            a.full_name, a.sp_code, b.assign_date, a.company_code, b.route_code, b.route_name, b.group_edesc, b.company_code ORDER BY b.assign_date) aa WHERE 1 = 1
                            AND sp_code IN ('{model.SP_CODE}') GROUP BY userid, company_code, trunc(assign_date), sp_code, group_edesc, sp_code, full_name) GROUP BY sp_code, group_edesc, sp_code,
                            employee_edesc ORDER BY sp_code";
            List<NONPLAN_VISIT_TARGET> nonplanVisit = dbContext.SqlQuery<NONPLAN_VISIT_TARGET>(query).ToList();
            return nonplanVisit;
        }
        public List<SchemeReportResponseModel> fetchSchemeReportData(SchemeReportRequestModel model, NeoErpCoreEntity dbContext)
        {
            List<SchemeReportResponseModel> NoItemShcemeList = new List<SchemeReportResponseModel>();
            string date = model.DATE.ToString("MM/dd/yyyy");
            string query = $@"select ds.SCHEME_ID as SchemeID ,ds.SCHEME_NAME as SchemeName , ds.Start_Date as StartDate, ds.End_Date as EndDate, ds.AREA_CODE as AreaCode, da.AREA_NAME as AreaName,  ds.OFFER_TYPE as OfferType, dm.ENTITY_CODE as SP_CODE from DIST_SCHEME ds,DIST_AREA_MASTER da,  DIST_SCHEME_ENTITY_MAPPING dm  where ds.AREA_CODE=da.AREA_CODE and  ds.Scheme_ID=dm.Scheme_ID and  TO_DATE( '{date}', 'MM/DD/RRRR' ) BETWEEN ds.Start_Date AND ds.End_Date and dm.ENTITY_CODE='{model.SP_CODE}' and   ds.COMPANY_CODE={model.COMPANY_CODE} and ds.BRANCH_CODE={model.BRANCH_CODE} and ds.DELETED_FLAG='N'";
            var schemes = dbContext.SqlQuery<SchemeReportResponseModel>(query).ToList();
            foreach (var scheme in schemes)
            {
                var Itemquery = $@"select  distinct sc.Item_code, it.Item_Edesc from DIST_SCHEME_ITEMS sc, IP_ITEM_MASTER_SETUP it where sc.ITEM_CODE=it.ITEM_CODE and sc.SCHEME_ID={scheme.SchemeID} and sc.ITEM_CODE='{model.ITEM_CODE}'";
                scheme.Items = dbContext.SqlQuery<ItemDetails>(Itemquery).ToList();
                if (scheme.Items.Count != 0)
                {
                    if (scheme.OfferType == "GIFT")
                    {
                        var Ruleresult = new List<SchemeDetailModel>();
                        var Itemresult = new List<ItemDetails>();
                        var Mappingquery = $@"select distinct sr.Rule_ID, sr.Max_Value, sr.Min_Value, sr.Gift_QTY from DIST_SCHEME_GIFT_ITEMS scg, DIST_SCHEME_RULE_MAPPING sr where scg.RULE_ID=sr.RULE_ID and sr.SCHEME_ID={scheme.SchemeID}";
                        Ruleresult = dbContext.SqlQuery<SchemeDetailModel>(Mappingquery).ToList();
                        foreach (var rule in Ruleresult)
                        {
                            var query2 = $@"select distinct it.Item_code, it.Item_Edesc from DIST_SCHEME_GIFT_ITEMS scg, IP_ITEM_MASTER_SETUP it where scg.GIFT_ITEM_CODE=it.ITEM_CODE and scg.RULE_ID={rule.Rule_ID} and it.COMPANY_CODE={model.COMPANY_CODE} and it.BRANCH_CODE={model.BRANCH_CODE}";
                            Itemresult = dbContext.SqlQuery<ItemDetails>(query2).ToList();
                            foreach (var item in Itemresult)
                            {
                                rule.Gift_Items.Add(item);
                            }


                        }
                        scheme.SchemeDetails = Ruleresult;

                    }
                    else
                    {
                        var result = new List<SchemeDetailModel>();
                        var Discountquery = $@"select distinct sr.Rule_ID, sr.Max_Value, sr.Min_Value, sr.Discount, sr.Discount_Type as DiscountType from DIST_SCHEME_RULE_MAPPING sr where sr.SCHEME_ID={scheme.SchemeID}";
                        result = dbContext.SqlQuery<SchemeDetailModel>(Discountquery).ToList();
                        scheme.SchemeDetails = result;
                    }
                }
                else
                {
                    NoItemShcemeList.Add(scheme);
                }
            }
            foreach (var scheme in NoItemShcemeList)
            {
                schemes.Remove(scheme);
            }
            return schemes;

        }
        //public List<>
        public List<DistributionSalesReturnModel> GetAllDistSalesReturn(CommonRequestModel requestParam, NeoErpCoreEntity dbContext)
        {
            var distSR = new List<DistributionSalesReturnModel>() { new DistributionSalesReturnModel { Id = "1", Response = "Wawooo You Hit the return API" } };
            return distSR;
        }
        #endregion Fetching Data

        #region Inserting Data
        public Dictionary<string, string> UpdateMyLocation(UpdateRequestModel model, NeoErpCoreEntity dbContext)
        {
            var result = new Dictionary<string, string>();
            var query = string.Empty;
            if (string.IsNullOrWhiteSpace(model.sp_code))
            {
                throw new Exception("Salesperson code missing.");
            }
            if (string.IsNullOrWhiteSpace(model.latitude))
            {
                throw new Exception("Latitude missing.");
            }
            if (string.IsNullOrWhiteSpace(model.longitude))
            {
                throw new Exception("Longitude missing.");
            }
            if (string.IsNullOrWhiteSpace(model.customer_code))
            {
                throw new Exception("Distributor/Reseller code missing.");
            }
            model.is_visited = (string.IsNullOrWhiteSpace(model.is_visited)) ? "Y" : model.is_visited;
            model.destination = model.destination == null ? "X" : model.destination;

            if (model.customer_type.Equals("P", StringComparison.OrdinalIgnoreCase) || model.customer_type.Equals("DEALER", StringComparison.OrdinalIgnoreCase))
                model.customer_type = "P";
            else if (model.customer_type.Equals("D", StringComparison.OrdinalIgnoreCase) || model.customer_type.Equals("DISTRIBUTOR", StringComparison.OrdinalIgnoreCase))
                model.customer_type = "D";
            else if (model.customer_type.Equals("R", StringComparison.OrdinalIgnoreCase) || model.customer_type.Equals("RESELLER", StringComparison.OrdinalIgnoreCase))
                model.customer_type = "R";
            else if (model.customer_type.Equals("F", StringComparison.OrdinalIgnoreCase) || model.customer_type.Equals("FARMER", StringComparison.OrdinalIgnoreCase))
                model.customer_type = "F";
            else
                throw new Exception("Invalid customer type");

            query = $@"INSERT INTO DIST_LOCATION_TRACK (SP_CODE, UPDATE_DATE, LATITUDE, LONGITUDE, DESTINATION, CUSTOMER_CODE, CUSTOMER_TYPE, REMARKS, IS_VISITED,COMPANY_CODE,BRANCH_CODE)
            VALUES ('{model.sp_code}', TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}', 'MM/dd/yyyy hh24:mi:ss'), '{model.latitude}', '{model.longitude}', '{model.destination}', '{model.customer_code}',
            '{model.customer_type}', '{model.remarks}', '{model.is_visited}', '{model.COMPANY_CODE}', '{model.BRANCH_CODE}')";
            var row = dbContext.ExecuteSqlCommand(query);
            if (row > 0)
            {
                result.Add("date", DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"));
                return result;
            }
            else
                throw new Exception("Unable to update location.");
        }



        public Dictionary<string, string> UpdateCurrentLocation(UpdateRequestModel model, NeoErpCoreEntity dbContext)
        {
            //validate sp_code removed since multiple company items can be inserted in eod

            //var query = $"SELECT * FROM HR_EMPLOYEE_SETUP WHERE EMPLOYEE_CODE='{model.sp_code}' AND COMPANY_CODE='{model.COMPANY_CODE}'";
            //var user = dbContext.SqlQuery<object>(query).FirstOrDefault();
            //if (user == null)
            //    throw new Exception("Invalid User!!!");

            //string lat1 = "27.710234";
            //string lon1 = "85.328426";


            //Debug.WriteLine(lat1);
            //bool isGreater = this.IsDistanceGreater(lat1, lon1, lat1, lon1, 20);
            //Debug.WriteLine(isGreater);





            string insertQuery = "";
            int row;
            //Debug.WriteLine("yoiiii");
            //IsDistanceGreater("27.710234", "85.328426", "27.710234", "85.328426", 20);

            //Debug.WriteLine(IsDistanceGreater("27.710234", "85.328426", "27.710234", "85.328426", 20));
            if (model.Track_Type == "ATN" || model.Track_Type == "EOD") //checking if EOD and ATN are already inserted or not
            {
                var prev = dbContext.SqlQuery<int>($"SELECT COUNT(*) FROM DIST_LM_LOCATION_TRACKING WHERE TRACK_TYPE ='{model.Track_Type}' AND SP_CODE = '{model.sp_code}' AND TRUNC(SUBMIT_DATE) =TRUNC(SYSDATE) AND COMPANY_CODE = '{model.COMPANY_CODE}'").FirstOrDefault();
                if (prev > 0) //if previously inserted update the EOD insert a TRK row
                {
                    insertQuery = $@"INSERT INTO DIST_LM_LOCATION_TRACKING (SP_CODE, SUBMIT_DATE, LATITUDE, LONGITUDE,COMPANY_CODE,BRANCH_CODE,TRACK_TYPE) VALUES 
                            ('{model.sp_code}',TO_DATE('{model.Saved_Date}','MM/dd/yyyy hh24:mi:ss'),'{model.latitude}','{model.longitude}','{model.COMPANY_CODE}','{model.BRANCH_CODE}','TRK')";
                    if (model.Track_Type == "EOD")
                    {
                        string updateQuery = $@"UPDATE DIST_LM_LOCATION_TRACKING SET SUBMIT_DATE = TO_DATE('{model.Saved_Date}','MM/dd/yyyy hh24:mi:ss'),LATITUDE='{model.latitude}' ,LONGITUDE='{model.longitude}'
                        WHERE SP_CODE = '{model.sp_code}' AND COMPANY_CODE = '{model.COMPANY_CODE}' AND TRUNC(SUBMIT_DATE) = TRUNC(SYSDATE) AND TRACK_TYPE = '{model.Track_Type}'";
                        row = dbContext.ExecuteSqlCommand(updateQuery);
                    }
                } //if not previous entry, insert a new row for EOD/ATN
                else
                    insertQuery = $@"INSERT INTO DIST_LM_LOCATION_TRACKING (SP_CODE, SUBMIT_DATE, LATITUDE, LONGITUDE,COMPANY_CODE,BRANCH_CODE,TRACK_TYPE) VALUES 
                            ('{model.sp_code}',TO_DATE('{model.Saved_Date}','MM/dd/yyyy hh24:mi:ss'),'{model.latitude}','{model.longitude}','{model.COMPANY_CODE}','{model.BRANCH_CODE}','{model.Track_Type}')";
            }
            else //insert a new TRK record
                insertQuery = $@"INSERT INTO DIST_LM_LOCATION_TRACKING (SP_CODE, SUBMIT_DATE, LATITUDE, LONGITUDE,COMPANY_CODE,BRANCH_CODE,TRACK_TYPE) VALUES 
                            ('{model.sp_code}',TO_DATE('{model.Saved_Date}','MM/dd/yyyy hh24:mi:ss'),'{model.latitude}','{model.longitude}','{model.COMPANY_CODE}','{model.BRANCH_CODE}','{model.Track_Type}')";
            row = dbContext.ExecuteSqlCommand(insertQuery);
            if (model.Track_Type == "EOD") //inserting/updating a EOD details record. Maximum of one record per day for each salesperson
            {
                string queryEod = string.Empty; ;

                var data = dbContext.SqlQuery<UpdateRequestModel>($"SELECT * FROM DIST_EOD_UPDATE WHERE TO_DATE(CREATED_DATE)=TO_DATE(TO_DATE('{model.Time_Eod}','MM/dd/yyyy hh24:mi:ss')) AND SP_CODE='{model.sp_code}' AND COMPANY_CODE='{model.COMPANY_CODE}'").ToList();
                if (data.Count > 0)
                {
                    model.PO_DCOUNT = model.PO_DCOUNT + data[0].PO_DCOUNT;
                    model.RES_DETAIL = model.RES_DETAIL + data[0].RES_DETAIL;
                    model.PO_RCOUNT = model.PO_RCOUNT + data[0].PO_RCOUNT;
                    model.RES_MASTER = model.RES_MASTER + data[0].RES_MASTER;
                    model.RES_PHOTO = model.RES_PHOTO + data[0].RES_PHOTO;
                    model.RES_CONTACT_PHOTO = model.RES_CONTACT_PHOTO + data[0].RES_CONTACT_PHOTO;
                    model.RES_ENTITY = model.RES_ENTITY + data[0].RES_ENTITY;

                    queryEod = $@"UPDATE DIST_EOD_UPDATE SET PO_DCOUNT='{model.PO_DCOUNT}', PO_RCOUNT='{model.PO_RCOUNT}', RES_MASTER='{model.RES_MASTER}', RES_DETAIL='{model.RES_DETAIL}',RES_ENTITY='{model.RES_ENTITY}',
                                RES_STORE_PHOTO='{model.RES_PHOTO}',RES_CONTACT_PHOTO='{model.RES_CONTACT_PHOTO}',LATITUDE='{model.latitude}',LONGITUDE='{model.longitude}',REMARKS='{model.remarks}'
                                WHERE TO_DATE(CREATED_DATE)=TO_DATE(TO_DATE('{model.Time_Eod}','MM/dd/yyyy hh24:mi:ss')) AND SP_CODE='{model.sp_code}' AND COMPANY_CODE='{model.COMPANY_CODE}'";
                }
                else
                {
                    queryEod = $@"INSERT INTO DIST_EOD_UPDATE (SP_CODE, PO_DCOUNT, PO_RCOUNT, RES_MASTER, RES_DETAIL,RES_ENTITY,RES_STORE_PHOTO,RES_CONTACT_PHOTO,LATITUDE,LONGITUDE,CREATED_DATE,CREATED_BY,COMPANY_CODE, BRANCH_CODE,REMARKS) VALUES 
                            ('{model.sp_code}','{model.PO_DCOUNT}','{model.PO_RCOUNT}','{model.RES_MASTER}','{model.RES_DETAIL}','{model.RES_ENTITY}','{model.RES_PHOTO}','{model.RES_CONTACT_PHOTO}','{model.latitude}','{model.longitude}',TO_DATE('{model.Time_Eod}','MM/dd/yyyy hh24:mi:ss'),'{model.user_id}','{model.COMPANY_CODE}','{model.BRANCH_CODE}','{model.remarks}')";
                }
                var latLong = string.Empty;
                if (!string.IsNullOrWhiteSpace(model.latitude) && !string.IsNullOrWhiteSpace(model.longitude))
                    latLong = $"{model.latitude}, {model.longitude}";
                string AttendanceQuery = $@"INSERT INTO HRIS_ATTENDANCE (EMPLOYEE_ID,ATTENDANCE_DT,ATTENDANCE_FROM,ATTENDANCE_TIME, LOCATION) VALUES ('{model.sp_code}',TRUNC(TO_DATE('{model.Time_Eod}','MM/dd/yyyy hh24:mi:ss')),'MOBILE',TO_TIMESTAMP('{model.Time_Eod}','MM/dd/yyyy hh24:mi:ss'), '{latLong}')";
                row = dbContext.ExecuteSqlCommand(AttendanceQuery);
                var time = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss.fffffff tt", CultureInfo.InvariantCulture);
                try
                {
                    var thumbId = dbContext.SqlQuery<string>($"SELECT to_char(ID_THUMB_ID) FROM HRIS_EMPLOYEES WHERE EMPLOYEE_ID = '{model.sp_code}'").FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(thumbId))
                    {
                        try
                        {
                            var hris_procedure = $"BEGIN HRIS_ATTENDANCE_INSERT ({thumbId}, TRUNC(SYSDATE), NULL, 'MOBILE', TO_TIMESTAMP('{time}'), {latLong}); END;";
                            dbContext.ExecuteSqlCommand(hris_procedure);
                        }
                        catch (Exception ex)
                        {

                        }

                    }

                    var rowCal = dbContext.ExecuteSqlCommand(queryEod);
                }
                catch (Exception ex)
                {

                }
            }
            var result = new Dictionary<string, string>();
            result.Add(model.sp_code, model.Sync_Id == null ? "" : model.Sync_Id);
            return result;
        }

        //public Dictionary<string, string> UpdateCurrentLocation(UpdateRequestModel model, NeoErpCoreEntity dbContext)
        //{
        //    //validate sp_code removed since multiple company items can be inserted in eod

        //    //var query = $"SELECT * FROM HR_EMPLOYEE_SETUP WHERE EMPLOYEE_CODE='{model.sp_code}' AND COMPANY_CODE='{model.COMPANY_CODE}'";
        //    //var user = dbContext.SqlQuery<object>(query).FirstOrDefault();
        //    //if (user == null)
        //    //    throw new Exception("Invalid User!!!");
        //    string insertQuery = "";
        //    int row;
        //    if (model.Track_Type == "ATN" || model.Track_Type == "EOD") //checking if EOD and ATN are already inserted or not
        //    {
        //        var prev = dbContext.SqlQuery<int>($"SELECT COUNT(*) FROM DIST_LM_LOCATION_TRACKING WHERE TRACK_TYPE ='{model.Track_Type}' AND SP_CODE = '{model.sp_code}' AND TRUNC(SUBMIT_DATE) =TRUNC(SYSDATE) AND COMPANY_CODE = '{model.COMPANY_CODE}'").FirstOrDefault();
        //        if (prev > 0) //if previously inserted update the EOD insert a TRK row
        //        {
        //            insertQuery = $@"INSERT INTO DIST_LM_LOCATION_TRACKING (SP_CODE, SUBMIT_DATE, LATITUDE, LONGITUDE,COMPANY_CODE,BRANCH_CODE,TRACK_TYPE) VALUES 
        //                    ('{model.sp_code}',TO_DATE('{model.Saved_Date}','MM/dd/yyyy hh24:mi:ss'),'{model.latitude}','{model.longitude}','{model.COMPANY_CODE}','{model.BRANCH_CODE}','TRK')";
        //            if (model.Track_Type == "EOD")
        //            {
        //                string updateQuery = $@"UPDATE DIST_LM_LOCATION_TRACKING SET SUBMIT_DATE = TO_DATE('{model.Saved_Date}','MM/dd/yyyy hh24:mi:ss'),LATITUDE='{model.latitude}' ,LONGITUDE='{model.longitude}'
        //                WHERE SP_CODE = '{model.sp_code}' AND COMPANY_CODE = '{model.COMPANY_CODE}' AND TRUNC(SUBMIT_DATE) = TRUNC(SYSDATE) AND TRACK_TYPE = '{model.Track_Type}'";
        //                row = dbContext.ExecuteSqlCommand(updateQuery);
        //            }
        //        } //if not previous entry, insert a new row for EOD/ATN
        //        else
        //            insertQuery = $@"INSERT INTO DIST_LM_LOCATION_TRACKING (SP_CODE, SUBMIT_DATE, LATITUDE, LONGITUDE,COMPANY_CODE,BRANCH_CODE,TRACK_TYPE) VALUES 
        //                    ('{model.sp_code}',TO_DATE('{model.Saved_Date}','MM/dd/yyyy hh24:mi:ss'),'{model.latitude}','{model.longitude}','{model.COMPANY_CODE}','{model.BRANCH_CODE}','{model.Track_Type}')";
        //    }
        //    else //insert a new TRK record
        //        insertQuery = $@"INSERT INTO DIST_LM_LOCATION_TRACKING (SP_CODE, SUBMIT_DATE, LATITUDE, LONGITUDE,COMPANY_CODE,BRANCH_CODE,TRACK_TYPE) VALUES 
        //                    ('{model.sp_code}',TO_DATE('{model.Saved_Date}','MM/dd/yyyy hh24:mi:ss'),'{model.latitude}','{model.longitude}','{model.COMPANY_CODE}','{model.BRANCH_CODE}','{model.Track_Type}')";
        //    row = dbContext.ExecuteSqlCommand(insertQuery);
        //    if (model.Track_Type == "EOD") //inserting/updating a EOD details record. Maximum of one record per day for each salesperson
        //    {
        //        string queryEod = string.Empty;
        //        var data = dbContext.SqlQuery<object>($"SELECT * FROM DIST_EOD_UPDATE WHERE TO_DATE(CREATED_DATE)=TO_DATE(TO_DATE('{model.Time_Eod}','MM/dd/yyyy hh24:mi:ss')) AND SP_CODE='{model.sp_code}' AND COMPANY_CODE='{model.COMPANY_CODE}'").ToList();
        //        if (data.Count > 0)
        //            queryEod = $@"UPDATE DIST_EOD_UPDATE SET PO_DCOUNT='{model.PO_DCOUNT}', PO_RCOUNT='{model.PO_RCOUNT}', RES_MASTER='{model.RES_MASTER}', RES_DETAIL='{model.RES_DETAIL}',RES_ENTITY='{model.RES_ENTITY}',
        //                        RES_STORE_PHOTO='{model.RES_PHOTO}',RES_CONTACT_PHOTO='{model.RES_CONTACT_PHOTO}',LATITUDE='{model.latitude}',LONGITUDE='{model.longitude}',REMARKS='{model.remarks}'
        //                        WHERE TO_DATE(CREATED_DATE)=TO_DATE(TO_DATE('{model.Time_Eod}','MM/dd/yyyy hh24:mi:ss')) AND SP_CODE='{model.sp_code}' AND COMPANY_CODE='{model.COMPANY_CODE}'";
        //        else
        //            queryEod = $@"INSERT INTO DIST_EOD_UPDATE (SP_CODE, PO_DCOUNT, PO_RCOUNT, RES_MASTER, RES_DETAIL,RES_ENTITY,RES_STORE_PHOTO,RES_CONTACT_PHOTO,LATITUDE,LONGITUDE,CREATED_DATE,CREATED_BY,COMPANY_CODE, BRANCH_CODE,REMARKS) VALUES 
        //                    ('{model.sp_code}','{model.PO_DCOUNT}','{model.PO_RCOUNT}','{model.RES_MASTER}','{model.RES_DETAIL}','{model.RES_ENTITY}','{model.RES_PHOTO}','{model.RES_CONTACT_PHOTO}','{model.latitude}','{model.longitude}',TO_DATE('{model.Time_Eod}','MM/dd/yyyy hh24:mi:ss'),'{model.user_id}','{model.COMPANY_CODE}','{model.BRANCH_CODE}','{model.remarks}')";

        //        string AttendanceQuery = $@"INSERT INTO HRIS_ATTENDANCE (EMPLOYEE_ID,ATTENDANCE_DT,ATTENDANCE_FROM,ATTENDANCE_TIME) VALUES ('{model.sp_code}',TRUNC(TO_DATE('{model.Time_Eod}','MM/dd/yyyy hh24:mi:ss')),'MOBILE',TO_TIMESTAMP('{model.Time_Eod}','MM/dd/yyyy hh24:mi:ss'))";
        //        row = dbContext.ExecuteSqlCommand(AttendanceQuery);
        //        var time = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss.fffffff tt", CultureInfo.InvariantCulture);
        //        try
        //        {
        //            var thumbId = dbContext.SqlQuery<string>($"SELECT to_char(ID_THUMB_ID) FROM HRIS_EMPLOYEES WHERE EMPLOYEE_ID = '{model.sp_code}'").FirstOrDefault();
        //            if (!string.IsNullOrWhiteSpace(thumbId))
        //            {
        //                try
        //                {
        //                    var hris_procedure = $"BEGIN HRIS_ATTENDANCE_INSERT ({thumbId}, TRUNC(SYSDATE), NULL, 'MOBILE', TO_TIMESTAMP('{time}')); END;";
        //                    dbContext.ExecuteSqlCommand(hris_procedure);
        //                }
        //                catch (Exception ex)
        //                {

        //                }

        //            }

        //            var rowCal = dbContext.ExecuteSqlCommand(queryEod);
        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //    }
        //    var result = new Dictionary<string, string>();
        //    result.Add(model.sp_code, model.Sync_Id == null ? "" : model.Sync_Id);
        //    return result;
        //}

        public bool SaveExtraActivity(UpdateRequestModel model, NeoErpCoreEntity dbContext)
        {
            var query = string.Empty;
            if (string.IsNullOrWhiteSpace(model.sp_code))
            {
                throw new Exception("Salesperson code missing.");
            }
            if (string.IsNullOrWhiteSpace(model.latitude))
            {
                throw new Exception("Latitude missing.");
            }
            if (string.IsNullOrWhiteSpace(model.longitude))
            {
                throw new Exception("Longitude missing.");
            }
            if (string.IsNullOrWhiteSpace(model.remarks))
            {
                throw new Exception("Remark is null.");
            }
            query = $@"INSERT INTO DIST_EXTRA_ACTIVITY (SP_CODE,LATITUDE,LONGITUDE,REMARKS,COMPANY_CODE,BRANCH_CODE) VALUES(
                    '{model.sp_code}','{model.latitude}','{model.longitude}','{model.remarks}','{model.COMPANY_CODE}','{model.BRANCH_CODE}')";
            var row = dbContext.ExecuteSqlCommand(query);
            if (row > 0)
                return true;
            else
                throw new Exception("Error Processing Request");
        }

        public bool UpdateCustomerLocation(UpdateCustomerRequestModel model, NeoErpCoreEntity dbContext)
        {
            var query = string.Empty;
            int a = 0;
            a++;
            if (string.IsNullOrWhiteSpace(model.user_id))
            {
                throw new Exception("User ID missing.");
            }
            if (string.IsNullOrWhiteSpace(model.code))
            {
                throw new Exception("Customer code missing.");
            }
            if (string.IsNullOrWhiteSpace(model.latitude))
            {
                throw new Exception("Latitude missing.");
            }
            if (string.IsNullOrWhiteSpace(model.longitude))
            {
                throw new Exception("Longitude missing.");
            }
            if (string.IsNullOrWhiteSpace(model.type))
            {
                throw new Exception("Customer type missing.");
            }
            if (model.type.Equals("P", StringComparison.OrdinalIgnoreCase) || model.type.Equals("DEALER", StringComparison.OrdinalIgnoreCase))
                query = $@"UPDATE DIST_DEALER_MASTER SET LUPDATE_BY = '{model.user_id}', LUPDATE_DATE = TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}','MM/dd/yyyy hh24:mi:ss'),
                LATITUDE = '{model.latitude}', LONGITUDE = '{model.longitude}' WHERE DEALER_CODE = '{model.code}' AND COMPANY_CODE='{model.COMPANY_CODE}'";
            else if (model.type.Equals("D", StringComparison.OrdinalIgnoreCase) || model.type.Equals("DISTRIBUTOR", StringComparison.OrdinalIgnoreCase))
                query = $@"UPDATE DIST_DISTRIBUTOR_MASTER SET LUPDATE_BY = '{model.user_id}', LUPDATE_DATE = TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}','MM/dd/yyyy hh24:mi:ss'),
                LATITUDE = '{model.latitude}', LONGITUDE = '{model.longitude}' WHERE DISTRIBUTOR_CODE = '{model.code}' AND COMPANY_CODE='{model.COMPANY_CODE}'";
            else if (model.type.Equals("R", StringComparison.OrdinalIgnoreCase) || model.type.Equals("RESELLER", StringComparison.OrdinalIgnoreCase) || model.type.Equals("outlet", StringComparison.OrdinalIgnoreCase))
                query = $@"UPDATE DIST_RESELLER_MASTER SET LUPDATE_BY = '{model.user_id}', LUPDATE_DATE = TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}','MM/dd/yyyy hh24:mi:ss'),
                LATITUDE = '{model.latitude}', LONGITUDE = '{model.longitude}' WHERE RESELLER_CODE = '{model.code}' AND COMPANY_CODE='{model.COMPANY_CODE}'";
            else if (model.type.Equals("B", StringComparison.OrdinalIgnoreCase) || model.type.Equals("BRANDING", StringComparison.OrdinalIgnoreCase))
                query = $@"UPDATE BRD_OTHER_ENTITY SET LATITUDE = '{model.latitude}', LONGITUDE = '{model.longitude}', SYNC_ID='{model.Sync_Id}' WHERE CODE = '{model.code}' AND COMPANY_CODE='{model.COMPANY_CODE}'";
            else if (model.type.Equals("F", StringComparison.OrdinalIgnoreCase) || model.type.Equals("FARMER", StringComparison.OrdinalIgnoreCase))
                query = $@"UPDATE DIST_FARMER_MASTER SET FARM_LATITUDE = '{model.latitude}', FARM_LONGITUDE = '{model.longitude}' WHERE FARMER_ID = '{model.code}'";
            else
                throw new Exception("Invalid customer type");

            var row = dbContext.ExecuteSqlCommand(query);
            if (row > 0)
            {
                return true;
            }
            else
            {
                throw new Exception("Unable to update customer location.");
            }
        }
        private readonly Dictionary<string, bool> _hasItemsCache = new Dictionary<string, bool>();
        public dynamic GetCustomerProductWiseSales(dynamic data, NeoErpCoreEntity dbContext)
        {
            var result = new List<Dictionary<string, object>>();
            var company_code = data["COMPANY_CODE"] ?? "01";
            var branch_code = data["BRANCH_CODE"] ?? "01.01";
            var companyFilter = string.Empty;
            var branchFilter = string.Empty;
            if (company_code.Contains("[") && company_code.Contains("]"))
            {
                companyFilter = company_code.Replace("[", "'").Replace("]", "'").Replace(", ", "','");
            }
            else
            {
                companyFilter = $"'{company_code}'";
            }
            if (branch_code.Contains("[") && branch_code.Contains("]"))
            {
                branchFilter = branch_code.Replace("[", "'").Replace("]", "'").Replace(", ", "','");
            }
            else
            {
                branchFilter = $"'{branch_code}'";
            }
            var dateModel = dbContext.SqlQuery<DateModel>($"select startdate as START_DATE, enddate as END_DATE from v_date_range where rangename='This Year'").ToList();
            var fromDate = dateModel[0].START_DATE;
            var toDate = dateModel[0].END_DATE;
            var query = $@"
  SELECT B.CUSTOMER_CODE,
         B.CUSTOMER_EDESC,
         B.MASTER_CUSTOMER_CODE                                    MASTER_CODE,
         B.PRE_CUSTOMER_CODE                                       PRE_CODE,
         trim(B.GROUP_SKU_FLAG)                                          GROUP_FLAG,
         (  LENGTH (B.MASTER_CUSTOMER_CODE)
          - LENGTH (REPLACE (B.MASTER_CUSTOMER_CODE, '.', '')))    ROWLEV,
         A.*
    FROM (SELECT *
            FROM (  SELECT A.COMPANY_CODE,
                           A.CUSTOMER_CODE                         CUS_CODE,
                           ITEM_CODE,
                           ITEM_EDESC,
                           INDEX_MU_CODE,
                           MTH,
                           SALES_QTY - SALES_RET_QTY               NET_SALES_QTY,
                             round(SALES_VALUE
                           - SALES_RET_VALUE
                           + DEBIT_VALUE
                           - CREDIT_VALUE,2)                       NET_SALES_VALUE,
                           FREE_QTY,
                           SALES_QTY - SALES_RET_QTY + FREE_QTY    NET_QTY
                      FROM (  SELECT MTH,
                                     A.COMPANY_CODE,
                                     A.CUSTOMER_CODE,
                                     A.ITEM_CODE,
                                     D.ITEM_EDESC,
                                     D.INDEX_MU_CODE            INDEX_MU_CODE,
                                     CASE
                                         WHEN E.BRAND_NAME IS NULL
                                         THEN
                                             D.ITEM_EDESC
                                         ELSE
                                             E.BRAND_NAME
                                     END                        BRAND_NAME,
                                     SUM (A.SALES_QTY)          SALES_QTY,
                                     SUM (A.SALES_VALUE)        SALES_VALUE,
                                     SUM (A.SALES_RET_QTY)      SALES_RET_QTY,
                                     SUM (A.SALES_RET_VALUE)    SALES_RET_VALUE,
                                     SUM (DEBIT_VALUE)          DEBIT_VALUE,
                                     SUM (CREDIT_VALUE)         CREDIT_VALUE,
                                     SUM (NVL (FREE_QTY, 0))    FREE_QTY
                                FROM (  SELECT A.COMPANY_CODE,
                                               A.CUSTOMER_CODE,
                                               A.ITEM_CODE,
                                               SUBSTR (BS_DATE (SALES_DATE), 6, 2)
                                                   MTH,
                                               SUM (NVL (A.QUANTITY, 0))
                                                   SALES_QTY,
                                               SUM (
                                                   NVL (
                                                       A.QUANTITY * A.NET_GROSS_RATE,
                                                       0))
                                                   SALES_VALUE,
                                               0
                                                   SALES_RET_QTY,
                                               0
                                                   SALES_RET_VALUE,
                                               0
                                                   DEBIT_VALUE,
                                               0
                                                   CREDIT_VALUE,
                                               SUM (NVL (FREE_QTY, 0))
                                                   FREE_QTY
                                          FROM SA_SALES_INVOICE A
                                         WHERE     A.DELETED_FLAG = 'N'
                                               AND A.COMPANY_CODE IN ({companyFilter})
                                               AND A.BRANCH_CODE IN ({branchFilter})
                                               AND A.SALES_DATE BETWEEN TO_DATE('{fromDate}', 'MM/DD/YYYY HH12:MI:SS AM')
                                                                    AND TO_DATE('{toDate}', 'MM/DD/YYYY HH12:MI:SS AM')
                                      GROUP BY A.CUSTOMER_CODE,
                                               A.ITEM_CODE,
                                               A.COMPANY_CODE,
                                               SALES_DATE
                                      UNION ALL
                                        SELECT A.COMPANY_CODE,
                                               A.CUSTOMER_CODE,
                                               A.ITEM_CODE,
                                               SUBSTR (BS_DATE (RETURN_DATE), 6, 2)
                                                   MTH,
                                               0
                                                   SALES_QTY,
                                               0
                                                   SALES_VALUE,
                                               SUM (NVL (A.QUANTITY, 0))
                                                   SALES_RET_QTY,
                                               SUM (
                                                   NVL (
                                                       A.QUANTITY * A.NET_GROSS_RATE,
                                                       0))
                                                   SALES_RET_VALUE,
                                               0
                                                   DEBIT_VALUE,
                                               0
                                                   CREDIT_VALUE,
                                               SUM (NVL (FREE_QTY, 0)) * -1
                                                   FREE_QTY
                                          FROM SA_SALES_RETURN A
                                         WHERE     A.DELETED_FLAG = 'N'
                                               AND A.COMPANY_CODE IN ({companyFilter})
                                               AND A.BRANCH_CODE IN ({branchFilter})
                                               AND A.RETURN_DATE BETWEEN TO_DATE('{fromDate}', 'MM/DD/YYYY HH12:MI:SS AM')
                                                                    AND TO_DATE('{toDate}', 'MM/DD/YYYY HH12:MI:SS AM')
                                      GROUP BY A.CUSTOMER_CODE,
                                               A.ITEM_CODE,
                                               A.COMPANY_CODE,
                                               RETURN_DATE
                                      UNION ALL
                                        SELECT A.COMPANY_CODE,
                                               A.CUSTOMER_CODE,
                                               A.ITEM_CODE,
                                               SUBSTR (BS_DATE (VOUCHER_DATE), 6, 2)
                                                   MTH,
                                               0
                                                   SALES_QTY,
                                               0
                                                   SALES_VALUE,
                                               0
                                                   SALES_RET_QTY,
                                               0
                                                   SALES_RET_VALUE,
                                               SUM (
                                                   NVL (
                                                       A.QUANTITY * A.NET_GROSS_RATE,
                                                       0))
                                                   DEBIT_VALUE,
                                               0
                                                   CREDIT_VALUE,
                                               0
                                                   FREE_QTY
                                          FROM SA_DEBIT_NOTE A
                                         WHERE     A.DELETED_FLAG = 'N'
                                               AND A.COMPANY_CODE IN ({companyFilter})
                                               AND A.BRANCH_CODE IN ({branchFilter})
                                               AND A.VOUCHER_DATE BETWEEN TO_DATE('{fromDate}', 'MM/DD/YYYY HH12:MI:SS AM')
                                                                    AND TO_DATE('{toDate}', 'MM/DD/YYYY HH12:MI:SS AM')
                                      GROUP BY A.CUSTOMER_CODE,
                                               A.ITEM_CODE,
                                               A.COMPANY_CODE,
                                               VOUCHER_DATE
                                      UNION ALL
                                        SELECT A.COMPANY_CODE,
                                               A.CUSTOMER_CODE,
                                               A.ITEM_CODE,
                                               SUBSTR (BS_DATE (VOUCHER_DATE), 6, 2)
                                                   MTH,
                                               0
                                                   SALES_QTY,
                                               0
                                                   SALES_VALUE,
                                               0
                                                   SALES_RET_QTY,
                                               0
                                                   SALES_RET_VALUE,
                                               0
                                                   DEBIT_VALUE,
                                               SUM (
                                                   NVL (
                                                       A.QUANTITY * A.NET_GROSS_RATE,
                                                       0))
                                                   CREDIT_VALUE,
                                               0
                                                   FREE_QTY
                                          FROM SA_CREDIT_NOTE A
                                         WHERE     A.DELETED_FLAG = 'N'
                                               AND A.COMPANY_CODE IN ({companyFilter})
                                               AND A.BRANCH_CODE IN ({branchFilter})
                                               AND A.VOUCHER_DATE BETWEEN TO_DATE('{fromDate}', 'MM/DD/YYYY HH12:MI:SS AM')
                                                                    AND TO_DATE('{toDate}', 'MM/DD/YYYY HH12:MI:SS AM')
                                      GROUP BY A.CUSTOMER_CODE,
                                               A.ITEM_CODE,
                                               A.COMPANY_CODE,
                                               VOUCHER_DATE
                                      ORDER BY CUSTOMER_CODE) A,
                                     SA_CUSTOMER_SETUP B,
                                     IP_ITEM_MASTER_SETUP D,
                                     IP_ITEM_SPEC_SETUP E
                               WHERE     A.CUSTOMER_CODE = B.CUSTOMER_CODE
                                     AND A.ITEM_CODE = D.ITEM_CODE
                                     AND A.COMPANY_CODE = D.COMPANY_CODE
                                     AND A.COMPANY_CODE = B.COMPANY_CODE
                                     AND D.GROUP_SKU_FLAG = 'I'
                                     AND D.ITEM_CODE = E.ITEM_CODE(+)
                                     AND D.COMPANY_CODE = E.COMPANY_CODE(+)
                                     AND A.COMPANY_CODE IN ({companyFilter})
                            GROUP BY A.CUSTOMER_CODE,
                                     A.ITEM_CODE,
                                     D.ITEM_EDESC,
                                     D.INDEX_MU_CODE,
                                     E.BRAND_NAME,
                                     A.COMPANY_CODE,
                                     MTH) A
                  ORDER BY CUSTOMER_CODE, ITEM_EDESC)
                 PIVOT (SUM (NET_SALES_QTY)
                       QTY, SUM (NET_SALES_VALUE)
                       VALUE, SUM (FREE_QTY)
                       FREE, SUM (NET_QTY)
                       NET_QTY
                       FOR MTH
                       IN ('04' AS SHRAWAN, '05' AS BHADRA, '06' AS ASHOJ, '07' AS KARTIK,
                                 '08' AS MANGSIR, '09' AS POUSH, '10' AS MAGH, '11' AS FALGUN,
                                 '12' AS CHAITRA, '01' AS BAISHAKH, '02' AS JESTHA, '03' AS ASHADH)
            )) A,
         SA_CUSTOMER_SETUP B,
        DIST_USER_AREAS C
   WHERE     B.CUSTOMER_CODE = A.CUS_CODE(+)
         AND B.COMPANY_CODE = A.COMPANY_CODE(+)
        AND B.CUSTOMER_CODE = C.CUSTOMER_CODE (+)
        and B.COMPANY_CODE = C.COMPANY_CODE (+)
         AND B.COMPANY_CODE IN ({companyFilter})
         and (
            item_code is not null 
            OR group_sku_flag = 'G'
         )
        AND (C.SP_CODE = '{data["SP_CODE"]}' 
            OR GROUP_SKU_FLAG = 'G'
         )
ORDER BY B.MASTER_CUSTOMER_CODE, B.CUSTOMER_EDESC, A.ITEM_EDESC
            ";
            string sConnStr1 = ConfigurationManager.ConnectionStrings["NeoErpCoreEntity"].ToString();
            string[] tokens = sConnStr1.Split('"');
            decimal net_shrawan_qty = 0, net_shrawan_val = 0;
            decimal net_bhadra_qty = 0, net_bhadra_val = 0;
            decimal net_ashoj_qty = 0, net_ashoj_val = 0;
            decimal net_kartik_qty = 0, net_kartik_val = 0;
            decimal net_mangsir_qty = 0, net_mangsir_val = 0;
            decimal net_poush_qty = 0, net_poush_val = 0;
            decimal net_magh_qty = 0, net_magh_val = 0;
            decimal net_falgun_qty = 0, net_falgun_val = 0;
            decimal net_chaitra_qty = 0, net_chaitra_val = 0;
            decimal net_baishakh_qty = 0, net_baishakh_val = 0;
            decimal net_jestha_qty = 0, net_jestha_val = 0;
            decimal net_ashadh_qty = 0, net_ashadh_val = 0;
            using (OracleConnection objConn = new OracleConnection(tokens[1]))
            {
                objConn.Open();

                using (OracleCommand cmd = new OracleCommand(query, objConn))
                {

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new Dictionary<string, object>
                            {
                                ["CUSTOMER_CODE"] = reader["CUSTOMER_CODE"].ToString(),
                                ["PRE_CODE"] = reader["PRE_CODE"].ToString(),
                                ["MASTER_CODE"] = reader["MASTER_CODE"].ToString(),
                                ["CUSTOMER_EDESC"] = reader["CUSTOMER_EDESC"].ToString(),
                                ["LEVEL"] = Convert.ToInt32(reader["ROWLEV"] ?? 0),
                                ["ITEM_CODE"] = reader["ITEM_CODE"] != DBNull.Value ? reader["ITEM_CODE"].ToString() : string.Empty,
                                ["ITEM_EDESC"] = reader["ITEM_EDESC"] != DBNull.Value ? reader["ITEM_EDESC"].ToString() : string.Empty,
                                ["IS_GROUP"] = reader["GROUP_FLAG"] != DBNull.Value && string.Equals(reader["GROUP_FLAG"].ToString(), "G", StringComparison.OrdinalIgnoreCase) ? "Y" : "N",
                                ["SHRAWAN_QTY"] = reader["SHRAWAN_QTY"] != DBNull.Value ? Convert.ToDecimal(reader["SHRAWAN_QTY"]) : 0,
                                ["SHRAWAN_VALUE"] = reader["SHRAWAN_VALUE"] != DBNull.Value ? Convert.ToDecimal(reader["SHRAWAN_VALUE"]) : 0,
                                ["BHADRA_QTY"] = reader["BHADRA_QTY"] != DBNull.Value ? Convert.ToDecimal(reader["BHADRA_QTY"]) : 0,
                                ["BHADRA_VALUE"] = reader["BHADRA_VALUE"] != DBNull.Value ? Convert.ToDecimal(reader["BHADRA_VALUE"]) : 0,
                                ["ASHOJ_QTY"] = reader["ASHOJ_QTY"] != DBNull.Value ? Convert.ToDecimal(reader["ASHOJ_QTY"]) : 0,
                                ["ASHOJ_VALUE"] = reader["ASHOJ_VALUE"] != DBNull.Value ? Convert.ToDecimal(reader["ASHOJ_VALUE"]) : 0,
                                ["KARTIK_QTY"] = reader["KARTIK_QTY"] != DBNull.Value ? Convert.ToDecimal(reader["KARTIK_QTY"]) : 0,
                                ["KARTIK_VALUE"] = reader["KARTIK_VALUE"] != DBNull.Value ? Convert.ToDecimal(reader["KARTIK_VALUE"]) : 0,
                                ["MANGSIR_QTY"] = reader["MANGSIR_QTY"] != DBNull.Value ? Convert.ToDecimal(reader["MANGSIR_QTY"]) : 0,
                                ["MANGSIR_VALUE"] = reader["MANGSIR_VALUE"] != DBNull.Value ? Convert.ToDecimal(reader["MANGSIR_VALUE"]) : 0,
                                ["POUSH_QTY"] = reader["POUSH_QTY"] != DBNull.Value ? Convert.ToDecimal(reader["POUSH_QTY"]) : 0,
                                ["POUSH_VALUE"] = reader["POUSH_VALUE"] != DBNull.Value ? Convert.ToDecimal(reader["POUSH_VALUE"]) : 0,
                                ["MAGH_QTY"] = reader["MAGH_QTY"] != DBNull.Value ? Convert.ToDecimal(reader["MAGH_QTY"]) : 0,
                                ["MAGH_VALUE"] = reader["MAGH_VALUE"] != DBNull.Value ? Convert.ToDecimal(reader["MAGH_VALUE"]) : 0,
                                ["FALGUN_QTY"] = reader["FALGUN_QTY"] != DBNull.Value ? Convert.ToDecimal(reader["FALGUN_QTY"]) : 0,
                                ["FALGUN_VALUE"] = reader["FALGUN_VALUE"] != DBNull.Value ? Convert.ToDecimal(reader["FALGUN_VALUE"]) : 0,
                                ["CHAITRA_QTY"] = reader["CHAITRA_QTY"] != DBNull.Value ? Convert.ToDecimal(reader["CHAITRA_QTY"]) : 0,
                                ["CHAITRA_VALUE"] = reader["CHAITRA_VALUE"] != DBNull.Value ? Convert.ToDecimal(reader["CHAITRA_VALUE"]) : 0,
                                ["BAISHAKH_QTY"] = reader["BAISHAKH_QTY"] != DBNull.Value ? Convert.ToDecimal(reader["BAISHAKH_QTY"]) : 0,
                                ["BAISHAKH_VALUE"] = reader["BAISHAKH_VALUE"] != DBNull.Value ? Convert.ToDecimal(reader["BAISHAKH_VALUE"]) : 0,
                                ["JESTHA_QTY"] = reader["JESTHA_QTY"] != DBNull.Value ? Convert.ToDecimal(reader["JESTHA_QTY"]) : 0,
                                ["JESTHA_VALUE"] = reader["JESTHA_VALUE"] != DBNull.Value ? Convert.ToDecimal(reader["JESTHA_VALUE"]) : 0,
                                ["ASHADH_QTY"] = reader["ASHADH_QTY"] != DBNull.Value ? Convert.ToDecimal(reader["ASHADH_QTY"]) : 0,
                                ["ASHADH_VALUE"] = reader["ASHADH_VALUE"] != DBNull.Value ? Convert.ToDecimal(reader["ASHADH_VALUE"]) : 0
                            };
                            result.Add(row);
                            net_shrawan_qty += reader["SHRAWAN_QTY"] != DBNull.Value ? Convert.ToDecimal(reader["SHRAWAN_QTY"]) : 0;
                            net_shrawan_val += reader["SHRAWAN_VALUE"] != DBNull.Value ? Convert.ToDecimal(reader["SHRAWAN_VALUE"]) : 0;

                            net_bhadra_qty += reader["BHADRA_QTY"] != DBNull.Value ? Convert.ToDecimal(reader["BHADRA_QTY"]) : 0;
                            net_bhadra_val += reader["BHADRA_VALUE"] != DBNull.Value ? Convert.ToDecimal(reader["BHADRA_VALUE"]) : 0;

                            net_ashoj_qty += reader["ASHOJ_QTY"] != DBNull.Value ? Convert.ToDecimal(reader["ASHOJ_QTY"]) : 0;
                            net_ashoj_val += reader["ASHOJ_VALUE"] != DBNull.Value ? Convert.ToDecimal(reader["ASHOJ_VALUE"]) : 0;

                            net_kartik_qty += reader["KARTIK_QTY"] != DBNull.Value ? Convert.ToDecimal(reader["KARTIK_QTY"]) : 0;
                            net_kartik_val += reader["KARTIK_VALUE"] != DBNull.Value ? Convert.ToDecimal(reader["KARTIK_VALUE"]) : 0;

                            net_mangsir_qty += reader["MANGSIR_QTY"] != DBNull.Value ? Convert.ToDecimal(reader["MANGSIR_QTY"]) : 0;
                            net_mangsir_val += reader["MANGSIR_VALUE"] != DBNull.Value ? Convert.ToDecimal(reader["MANGSIR_VALUE"]) : 0;

                            net_poush_qty += reader["POUSH_QTY"] != DBNull.Value ? Convert.ToDecimal(reader["POUSH_QTY"]) : 0;
                            net_poush_val += reader["POUSH_VALUE"] != DBNull.Value ? Convert.ToDecimal(reader["POUSH_VALUE"]) : 0;

                            net_magh_qty += reader["MAGH_QTY"] != DBNull.Value ? Convert.ToDecimal(reader["MAGH_QTY"]) : 0;
                            net_magh_val += reader["MAGH_VALUE"] != DBNull.Value ? Convert.ToDecimal(reader["MAGH_VALUE"]) : 0;

                            net_falgun_qty += reader["FALGUN_QTY"] != DBNull.Value ? Convert.ToDecimal(reader["FALGUN_QTY"]) : 0;
                            net_falgun_val += reader["FALGUN_VALUE"] != DBNull.Value ? Convert.ToDecimal(reader["FALGUN_VALUE"]) : 0;

                            net_chaitra_qty += reader["CHAITRA_QTY"] != DBNull.Value ? Convert.ToDecimal(reader["CHAITRA_QTY"]) : 0;
                            net_chaitra_val += reader["CHAITRA_VALUE"] != DBNull.Value ? Convert.ToDecimal(reader["CHAITRA_VALUE"]) : 0;

                            net_baishakh_qty += reader["BAISHAKH_QTY"] != DBNull.Value ? Convert.ToDecimal(reader["BAISHAKH_QTY"]) : 0;
                            net_baishakh_val += reader["BAISHAKH_VALUE"] != DBNull.Value ? Convert.ToDecimal(reader["BAISHAKH_VALUE"]) : 0;

                            net_jestha_qty += reader["JESTHA_QTY"] != DBNull.Value ? Convert.ToDecimal(reader["JESTHA_QTY"]) : 0;
                            net_jestha_val += reader["JESTHA_VALUE"] != DBNull.Value ? Convert.ToDecimal(reader["JESTHA_VALUE"]) : 0;

                            net_ashadh_qty += reader["ASHADH_QTY"] != DBNull.Value ? Convert.ToDecimal(reader["ASHADH_QTY"]) : 0;
                            net_ashadh_val += reader["ASHADH_VALUE"] != DBNull.Value ? Convert.ToDecimal(reader["ASHADH_VALUE"]) : 0;
                        }
                    }
                }
            }
            var finalList = new List<Dictionary<string, object>>();
            
            foreach (var grp in result.Where(x => x["IS_GROUP"].ToString() == "Y"))
            {
                string masterCode = grp["MASTER_CODE"].ToString();
                int level = Convert.ToInt32(grp["LEVEL"]);

                decimal tot_shrawan_qty = 0, tot_shrawan_val = 0;
                decimal tot_bhadra_qty = 0, tot_bhadra_val = 0;
                decimal tot_ashoj_qty = 0, tot_ashoj_val = 0;
                decimal tot_kartik_qty = 0, tot_kartik_val = 0;
                decimal tot_mangsir_qty = 0, tot_mangsir_val = 0;
                decimal tot_poush_qty = 0, tot_poush_val = 0;
                decimal tot_magh_qty = 0, tot_magh_val = 0;
                decimal tot_falgun_qty = 0, tot_falgun_val = 0;
                decimal tot_chaitra_qty = 0, tot_chaitra_val = 0;
                decimal tot_baishakh_qty = 0, tot_baishakh_val = 0;
                decimal tot_jestha_qty = 0, tot_jestha_val = 0;
                decimal tot_ashadh_qty = 0, tot_ashadh_val = 0;

                bool found = false;

                foreach (var item in result)
                {
                    if (item["IS_GROUP"].ToString() == "N")
                    {
                        string itemMaster = item["MASTER_CODE"].ToString();

                        if (itemMaster.StartsWith(masterCode) && itemMaster != masterCode)
                        {
                            found = true;

                            tot_shrawan_qty += Convert.ToDecimal(item["SHRAWAN_QTY"]);
                            tot_shrawan_val += Convert.ToDecimal(item["SHRAWAN_VALUE"]);

                            tot_bhadra_qty += Convert.ToDecimal(item["BHADRA_QTY"]);
                            tot_bhadra_val += Convert.ToDecimal(item["BHADRA_VALUE"]);

                            tot_ashoj_qty += Convert.ToDecimal(item["ASHOJ_QTY"]);
                            tot_ashoj_val += Convert.ToDecimal(item["ASHOJ_VALUE"]);

                            tot_kartik_qty += Convert.ToDecimal(item["KARTIK_QTY"]);
                            tot_kartik_val += Convert.ToDecimal(item["KARTIK_VALUE"]);

                            tot_mangsir_qty += Convert.ToDecimal(item["MANGSIR_QTY"]);
                            tot_mangsir_val += Convert.ToDecimal(item["MANGSIR_VALUE"]);

                            tot_poush_qty += Convert.ToDecimal(item["POUSH_QTY"]);
                            tot_poush_val += Convert.ToDecimal(item["POUSH_VALUE"]);

                            tot_magh_qty += Convert.ToDecimal(item["MAGH_QTY"]);
                            tot_magh_val += Convert.ToDecimal(item["MAGH_VALUE"]);

                            tot_falgun_qty += Convert.ToDecimal(item["FALGUN_QTY"]);
                            tot_falgun_val += Convert.ToDecimal(item["FALGUN_VALUE"]);

                            tot_chaitra_qty += Convert.ToDecimal(item["CHAITRA_QTY"]);
                            tot_chaitra_val += Convert.ToDecimal(item["CHAITRA_VALUE"]);

                            tot_baishakh_qty += Convert.ToDecimal(item["BAISHAKH_QTY"]);
                            tot_baishakh_val += Convert.ToDecimal(item["BAISHAKH_VALUE"]);

                            tot_jestha_qty += Convert.ToDecimal(item["JESTHA_QTY"]);
                            tot_jestha_val += Convert.ToDecimal(item["JESTHA_VALUE"]);

                            tot_ashadh_qty += Convert.ToDecimal(item["ASHADH_QTY"]);
                            tot_ashadh_val += Convert.ToDecimal(item["ASHADH_VALUE"]);
                            

                        }
                    }
                }

                if (found)
                {
                    var groupTotalRow = new Dictionary<string, object>(grp)
                    {
                        ["IS_GROUP"] = "Y",
                        ["IS_TOTAL"] = "Y",
                        ["SHRAWAN_QTY"] = tot_shrawan_qty,
                        ["SHRAWAN_VALUE"] = tot_shrawan_val,

                        ["BHADRA_QTY"] = tot_bhadra_qty,
                        ["BHADRA_VALUE"] = tot_bhadra_val,

                        ["ASHOJ_QTY"] = tot_ashoj_qty,
                        ["ASHOJ_VALUE"] = tot_ashoj_val,

                        ["KARTIK_QTY"] = tot_kartik_qty,
                        ["KARTIK_VALUE"] = tot_kartik_val,

                        ["MANGSIR_QTY"] = tot_mangsir_qty,
                        ["MANGSIR_VALUE"] = tot_mangsir_val,

                        ["POUSH_QTY"] = tot_poush_qty,
                        ["POUSH_VALUE"] = tot_poush_val,

                        ["MAGH_QTY"] = tot_magh_qty,
                        ["MAGH_VALUE"] = tot_magh_val,

                        ["FALGUN_QTY"] = tot_falgun_qty,
                        ["FALGUN_VALUE"] = tot_falgun_val,

                        ["CHAITRA_QTY"] = tot_chaitra_qty,
                        ["CHAITRA_VALUE"] = tot_chaitra_val,

                        ["BAISHAKH_QTY"] = tot_baishakh_qty,
                        ["BAISHAKH_VALUE"] = tot_baishakh_val,

                        ["JESTHA_QTY"] = tot_jestha_qty,
                        ["JESTHA_VALUE"] = tot_jestha_val,

                        ["ASHADH_QTY"] = tot_ashadh_qty,
                        ["ASHADH_VALUE"] = tot_ashadh_val
                    };

                    finalList.Add(groupTotalRow);
                }

                finalList.Add(grp);

                foreach (var item in result.Where(x =>
                    x["MASTER_CODE"].ToString().StartsWith(masterCode)
                    && x["MASTER_CODE"].ToString() != masterCode))
                {
                    finalList.Add(item);
                }
            }
            var months = new[]
                {
                    "SHRAWAN", "BHADRA", "ASHOJ", "KARTIK", "MANGSIR", "POUSH",
                    "MAGH", "FALGUN", "CHAITRA", "BAISHAKH", "JESTHA", "ASHADH"
                };
            result = finalList.Where(row =>
            {
                if (row["IS_GROUP"].ToString() == "N")
                    return true;

                bool hasValue = months.Any(m =>
                {
                    decimal qty = Convert.ToDecimal(row[$"{m}_QTY"]);
                    decimal val = Convert.ToDecimal(row[$"{m}_VALUE"]);
                    return qty != 0 || val != 0;
                });

                return hasValue;
            }).ToList();
            var finalHierarchy = new List<Dictionary<string, object>>();
            foreach (var g in result.Where(x => x["IS_GROUP"].ToString() == "Y"))
            {
                finalHierarchy.Add(new Dictionary<string, object>(g));
            }
            var individualItems = result.Where(x => x["IS_GROUP"].ToString() == "N").Distinct()
                .GroupBy(x => x["CUSTOMER_CODE"].ToString());

            foreach (var custGroup in individualItems)
            {
                var firstItem = custGroup.First();
                string preCode = firstItem["PRE_CODE"].ToString();
                int level = Convert.ToInt32(firstItem["LEVEL"]);
                var custMaster = $"{preCode}.{custGroup.Key}";
                var custRow = new Dictionary<string, object>
                {
                    ["CUSTOMER_CODE"] = custGroup.Key,
                    ["PRE_CODE"] = preCode,
                    ["MASTER_CODE"] = custMaster,
                    ["IS_GROUP"] = "Y",
                    ["LEVEL"] = level,
                    ["ITEM_CODE"] = "",
                    ["ITEM_EDESC"] = "",
                    ["CUSTOMER_EDESC"] = firstItem["CUSTOMER_EDESC"]
                };
                foreach (var m in months)
                {
                    custRow[$"{m}_QTY"] = custGroup.Sum(x => Convert.ToDecimal(x[$"{m}_QTY"]));
                    custRow[$"{m}_VALUE"] = custGroup.Sum(x => Convert.ToDecimal(x[$"{m}_VALUE"]));
                }

                finalHierarchy.Add(custRow);
                foreach (var item in custGroup.OrderBy(x => x["ITEM_CODE"]))
                {
                    var itemRow = new Dictionary<string, object>(item)
                    {
                        ["PRE_CODE"] = custMaster,
                        ["MASTER_CODE"] = $"{custMaster}.{item["ITEM_CODE"]}",
                        ["LEVEL"] = level + 1,
                    };
                    finalHierarchy.Add(itemRow);
                }
            }
            result = finalHierarchy.Where(row =>
            {
                if (row["IS_GROUP"].ToString() == "N")
                    return true;

                bool hasValue = months.Any(m =>
                {
                    decimal qty = Convert.ToDecimal(row[$"{m}_QTY"]);
                    decimal val = Convert.ToDecimal(row[$"{m}_VALUE"]);
                    return qty != 0 || val != 0;
                });

                return hasValue;
            }).ToList();
            //return result;
            return new Dictionary<string, object>
            {
                ["details"] = result,
                ["total"] = new Dictionary<string, object>
                {
                    ["SHRAWAN_QTY"] = net_shrawan_qty,
                    ["SHRAWAN_VALUE"] = net_shrawan_val,
                    ["BHADRA_QTY"] = net_bhadra_qty,
                    ["BHADRA_VALUE"] = net_bhadra_val,
                    ["ASHOJ_QTY"] = net_ashoj_qty,
                    ["ASHOJ_VALUE"] = net_ashoj_val,
                    ["KARTIK_QTY"] = net_kartik_qty,
                    ["KARTIK_VALUE"] = net_kartik_val,
                    ["MANGSIR_QTY"] = net_mangsir_qty,
                    ["MANGSIR_VALUE"] = net_mangsir_val,
                    ["POUSH_QTY"] = net_poush_qty,
                    ["POUSH_VALUE"] = net_poush_val,
                    ["MAGH_QTY"] = net_magh_qty,
                    ["MAGH_VALUE"] = net_magh_val,
                    ["FALGUN_QTY"] = net_falgun_qty,
                    ["FALGUN_VALUE"] = net_falgun_val,
                    ["CHAITRA_QTY"] = net_chaitra_qty,
                    ["CHAITRA_VALUE"] = net_chaitra_val,
                    ["BAISHAKH_QTY"] = net_baishakh_qty,
                    ["BAISHAKH_VALUE"] = net_baishakh_val,
                    ["JESTHA_QTY"] = net_jestha_qty,
                    ["JESTHA_VALUE"] = net_jestha_val,
                    ["ASHADH_QTY"] = net_ashadh_qty,
                    ["ASHADH_VALUE"] = net_ashadh_val
                }
            };
        }
        public dynamic GetFreeQtyData(dynamic data, NeoErpCoreEntity dbContext)
        {
            var result = new List<Dictionary<string, object>>();
            var company_code = data["COMPANY_CODE"] ?? "01";
            var branch_code = data["BRANCH_CODE"] ?? "01.01";
            var form_code = data["FORM_CODE"] ?? "";
            var cus_code = data["CUSTOMER_CODE"] ?? "";

            if (string.IsNullOrWhiteSpace(form_code) || string.IsNullOrWhiteSpace(cus_code))
                throw new Exception("Necessary Data is not being sent! Please try again or contact the respective representative.");
            
            var companyFilter = string.Empty;
            var branchFilter = string.Empty;
            if (company_code.Contains("[") && company_code.Contains("]"))
            {
                companyFilter = company_code.Replace("[", "'").Replace("]", "'").Replace(", ", "','");
            }
            else
            {
                companyFilter = $"'{company_code}'";
            }
            if (branch_code.Contains("[") && branch_code.Contains("]"))
            {
                branchFilter = branch_code.Replace("[", "'").Replace("]", "'").Replace(", ", "','");
            }
            else
            {
                branchFilter = $"'{branch_code}'";
            }
            var dateModel = dbContext.SqlQuery<DateModel>($"select startdate as START_DATE, enddate as END_DATE from v_date_range where rangename='This Year'").ToList();
            var fromDate = dateModel[0].START_DATE;
            var toDate = dateModel[0].END_DATE;
            var query = $@"
                    WITH free
                         AS (SELECT *
                               FROM free_quantity
                              WHERE COMPANY_CODE IN ({companyFilter}) AND DELETED_FLAG = 'N' 
                                        AND FORM_CODE = '{form_code}' AND CUSTOMER_CODE = '{cus_code}'),
                         conversion
                         AS (SELECT i.item_code,
                                    mu_code AS free_unit,
                                    mu_code AS main_unit,
                                    round(( (f.qty * i.conversion_factor) / i.fraction), 4) AS qty,
                                    round(( (f.FREE_QTY * i.conversion_factor) / i.fraction), 4)
                                       AS FREE_QTY
                               FROM    ip_item_unit_setup i
                                    JOIN
                                       free f
                                    ON     f.item_code = i.item_code
                                       AND f.company_code = i.company_code)
                    SELECT a.ITEM_CODE,
                           a.QTY,
                           a.FREE_QTY,
                           a.MAIN_UNIT,
                           a.FREE_UNIT
                      FROM free a
                    UNION ALL
                    SELECT b.ITEM_CODE,
                           b.QTY,
                           b.FREE_QTY,
                           b.MAIN_UNIT,
                           b.FREE_UNIT
                      FROM conversion b
            ";

            string sConnStr1 = ConfigurationManager.ConnectionStrings["NeoErpCoreEntity"].ToString();
            string[] tokens = sConnStr1.Split('"');
            using (OracleConnection objConn = new OracleConnection(tokens[1]))
            {
                objConn.Open();

                using (OracleCommand cmd = new OracleCommand(query, objConn))
                {

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new Dictionary<string, object>
                            {
                                ["ITEM_CODE"] = reader["ITEM_CODE"].ToString(),
                                ["QTY"] = reader["QTY"] != DBNull.Value ? Convert.ToDecimal(reader["QTY"].ToString()) : 0,
                                ["MAIN_UNIT"] = reader["MAIN_UNIT"].ToString(),
                                ["FREE_UNIT"] = reader["FREE_UNIT"].ToString(),
                                ["FREE_QTY"] = reader["FREE_QTY"] != DBNull.Value ? Convert.ToDecimal(reader["FREE_QTY"].ToString()) : 0,
                            };
                            result.Add(row);
                        }
                    }
                }
            }
            var groupedResult = result.GroupBy(x => x["ITEM_CODE"])
                    .ToDictionary(
                        g => g.Key.ToString(),
                        g => g.Select(x => new Dictionary<string, object>
                        {
                            { "QTY", x["QTY"] },
                            { "MAIN_UNIT", x["MAIN_UNIT"] },
                            { "FREE_UNIT", x["FREE_UNIT"] },
                            { "FREE_QTY", x["FREE_QTY"] }
                        }).ToList()
                    );

            return groupedResult;
        }
        public dynamic GetFarmerProblems(dynamic data, NeoErpCoreEntity dbContext)
        {
            var result = new List<Dictionary<string, object>>();
            var company_code = data["COMPANY_CODE"] ?? "01";
            var branch_code = data["BRANCH_CODE"] ?? "01.01";

            var companyFilter = string.Empty;
            var branchFilter = string.Empty;
            if (company_code.Contains("[") && company_code.Contains("]"))
            {
                companyFilter = company_code.Replace("[", "'").Replace("]", "'").Replace(", ", "','");
            }
            else
            {
                companyFilter = $"'{company_code}'";
            }

            var query = $@" SELECT * FROM DIST_FARMER_PROBLEM WHERE COMPANY_CODE IN ({companyFilter}) AND DELETED_FLAG = 'N'";

            string sConnStr1 = ConfigurationManager.ConnectionStrings["NeoErpCoreEntity"].ToString();
            string[] tokens = sConnStr1.Split('"');
            using (OracleConnection objConn = new OracleConnection(tokens[1]))
            {
                objConn.Open();

                using (OracleCommand cmd = new OracleCommand(query, objConn))
                {

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new Dictionary<string, object>
                            {
                                ["NAME"] = reader["PROBLEM_NAME"].ToString(),
                                ["ID"] = reader["ID"] != DBNull.Value ? reader["ID"].ToString() : "0"
                            };
                            result.Add(row);
                        }
                    }
                }
            }
            return result;
        }
        public string GeneratePurchaseOrderPdf(PurchaseOrderModel model, 
            int orderId, string entity, decimal netTotal, decimal qtyTotal, string customerName, string userName, 
            string shippingAddress, string shippingContact, string freeQty, 
            string secondaryUnit, decimal? secondaryQty, string thirdUnit, decimal? thirdQty, string altQty = "N")
        {
            string toGivePath = @"Areas\NeoErp.Distribution\Images\Uploads\PurchaseOrders";
            string folderPath = Path.Combine(UploadPath, "Uploads", "PurchaseOrders");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, $"PurchaseOrder_{orderId}_{entity}.pdf");
            string filePath1 = Path.Combine(toGivePath, $"PurchaseOrder_{orderId}_{entity}.pdf");

            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (Document doc = new Document(PageSize.A4, 50, 50, 25, 25))
            {
                PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                doc.Open();

                // Title
                Paragraph title = new Paragraph("Order Detail", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14))
                {
                    Alignment = Element.ALIGN_CENTER
                };
                doc.Add(title);
                doc.Add(new Paragraph(" "));

                // Customer Info
                doc.Add(new Paragraph($"Order No: {orderId}", FontFactory.GetFont(FontFactory.HELVETICA, 11)));
                doc.Add(new Paragraph($"Customer Name: {customerName}", FontFactory.GetFont(FontFactory.HELVETICA, 11)));
                //doc.Add(new Paragraph($"SalesPerson: {userName}", FontFactory.GetFont(FontFactory.HELVETICA, 11)));
                if (model.type.Equals("P", StringComparison.OrdinalIgnoreCase) || model.type.Equals("DEALER", StringComparison.OrdinalIgnoreCase)
                || model.type.Equals("D", StringComparison.OrdinalIgnoreCase) || model.type.Equals("DISTRIBUTOR", StringComparison.OrdinalIgnoreCase))
                    doc.Add(new Paragraph($"Shipping Address: {shippingAddress}", FontFactory.GetFont(FontFactory.HELVETICA, 11)));
                doc.Add(new Paragraph($"Shipping Contact: {shippingContact}", FontFactory.GetFont(FontFactory.HELVETICA, 11)));
                doc.Add(new Paragraph($"Date: {DateTime.Now:yyyy-MM-dd}", FontFactory.GetFont(FontFactory.HELVETICA, 11)));
                doc.Add(new Paragraph(" "));

                // Table for items
                List<string> headers = new List<string> { "Item Name", "Qty", "Unit", "Rate" };
                List<float> widths = new List<float>
                {
                    4f, 1.2f, 1.2f, 1.5f
                };

                if (freeQty == "Y")
                {
                    headers.Insert(3, "Free Qty");
                    widths.Insert(3, 1.2f);
                }
                if(altQty == "Y")
                {
                    if (!string.IsNullOrWhiteSpace(secondaryUnit))
                    {
                        headers.Add("2nd Qty");
                        headers.Add("2nd Unit");

                        widths.Add(1.2f);
                        widths.Add(1.2f);
                    }

                    if (!string.IsNullOrWhiteSpace(thirdUnit))
                    {
                        headers.Add("3rd Qty");
                        headers.Add("3rd Unit");

                        widths.Add(1.2f);
                        widths.Add(1.2f);
                    }
                }
                headers.Add("Total");
                widths.Add(1.8f);
                PdfPTable table = new PdfPTable(headers.Count);
                table.WidthPercentage = 100;
                table.SetWidths(widths.ToArray());
                foreach (var h in headers)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(h, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, BaseColor.WHITE)))
                    {
                        BackgroundColor = new BaseColor(40, 167, 69), // green
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 5
                    };
                    table.AddCell(cell);
                }

                foreach (var item in model.products)
                {
                    //table.AddCell(new PdfPCell(new Phrase(item.item_code)) { HorizontalAlignment = Element.ALIGN_LEFT });
                    table.AddCell(new PdfPCell(new Phrase(item.item_edesc ?? "")) { HorizontalAlignment = Element.ALIGN_LEFT });
                    table.AddCell(new PdfPCell(new Phrase(item.quantity.ToString("N2"))) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    table.AddCell(new PdfPCell(new Phrase(item.mu_code)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    if (freeQty == "Y")
                    {
                        table.AddCell(new PdfPCell(new Phrase((item.FREE_QTY).ToString())) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    }
                    table.AddCell(new PdfPCell(new Phrase(item.calc_rate.ToString("N2"))) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    if (altQty == "Y")
                    {
                        if (!string.IsNullOrWhiteSpace(secondaryUnit))
                        {
                            table.AddCell(new PdfPCell(new Phrase(item.SECOND_QUANTITY?.ToString("N2"))) { HorizontalAlignment = Element.ALIGN_RIGHT });
                            table.AddCell(new PdfPCell(new Phrase(item.SECONDARY_UNIT)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        }

                        if (!string.IsNullOrWhiteSpace(thirdUnit))
                        {
                            table.AddCell(new PdfPCell(new Phrase(item.THIRD_QTY?.ToString("N2"))) { HorizontalAlignment = Element.ALIGN_RIGHT });
                            table.AddCell(new PdfPCell(new Phrase(item.THIRD_UNIT)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        }
                    }
                    table.AddCell(new PdfPCell(new Phrase((item.quantity * item.calc_rate).ToString("N2"))) { HorizontalAlignment = Element.ALIGN_RIGHT });
                }

                doc.Add(table);

                // Footer / Net total
                doc.Add(new Paragraph(" "));
                Paragraph qtyPara = new Paragraph($"Total Quantity: {qtyTotal:N2}", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11))
                {
                    Alignment = Element.ALIGN_RIGHT
                };
                Paragraph netPara = new Paragraph($"Net Total: {netTotal:N2}", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11))
                {
                    Alignment = Element.ALIGN_RIGHT
                };
                doc.Add(qtyPara);
                doc.Add(netPara);

                doc.Close();
                writer.Close();
            }
            return filePath1.Replace("\\", "/");
        }
        public class ConversionData
        {
            public string MU_CODE { get; set; }
            public int SERIAL_NO { get; set; }
            public decimal CONVERSION_FACTOR { get; set; }
            public decimal FRACTION { get; set; }
        }
        public string NewPurchaseOrder(PurchaseOrderModel model, HttpFileCollection Files, NeoErpCoreEntity dbContext)
        {
            int id = 0;
            var pdfLink = string.Empty;
            var billedTo = string.Empty;
            var shippingAddress = string.Empty;
            string secondaryUnit = null;
            decimal? secondaryQty = null;
            string thirdUnit = null;
            decimal? thirdQty = null;
            string sequenceQuery = "";
            if (model.type.Equals("P", StringComparison.OrdinalIgnoreCase) || model.type.Equals("DEALER", StringComparison.OrdinalIgnoreCase)
                || model.type.Equals("D", StringComparison.OrdinalIgnoreCase) || model.type.Equals("DISTRIBUTOR", StringComparison.OrdinalIgnoreCase))
            //id = this.GetMaxId("DIST_IP_SSD_PURCHASE_ORDER", "ORDER_NO", dbContext);
            {
                while (true)
                {
                    sequenceQuery = "SELECT SEQ_DIST_IP_SSD_PURCHASE_ORDER.NEXTVAL FROM DUAL";
                    id = dbContext.SqlQuery<int>(sequenceQuery).FirstOrDefault();

                    string queryCheck = $@"SELECT COUNT(*) FROM dist_ip_ssd_purchase_order WHERE order_no = {id}";
                    int countCheck = dbContext.Database.SqlQuery<int>(queryCheck).FirstOrDefault();

                    if (countCheck == 0)
                        break;
                }
            }
            else if (model.type.Equals("R", StringComparison.OrdinalIgnoreCase) || model.type.Equals("RESELLER", StringComparison.OrdinalIgnoreCase) || model.type.Equals("outlet", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(model.reseller_code))
                    throw new Exception("Reseller code is empty");
                //id = this.GetMaxId("DIST_IP_SSR_PURCHASE_ORDER", "ORDER_NO", dbContext);
                while (true)
                {
                    sequenceQuery = "SELECT SEQ_DIST_IP_SSR_PURCHASE_ORDER.NEXTVAL FROM DUAL";
                    id = dbContext.SqlQuery<int>(sequenceQuery).FirstOrDefault();

                    string queryCheck = $@"SELECT COUNT(*) FROM dist_ip_ssr_purchase_order WHERE order_no = {id}";
                    int countCheck = dbContext.Database.SqlQuery<int>(queryCheck).FirstOrDefault();

                    if (countCheck == 0)
                        break;
                }
            }
            else
                throw new Exception("Invalid customer type");
            if (id <= 0)
                throw new Exception("Unable to get next ID for the purchase order.");


            string spcodeQ = $@"select sp_code from dist_login_user where userid = '{model.user_id}'";
            var spcode = dbContext.SqlQuery<string>(spcodeQ).FirstOrDefault();

            var today = $"TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}','MM/dd/yyyy hh24:mi:ss')";
            var serialnumber = 1;
            try
            {

                int distInsert = 0;
                decimal netTotal = 0;
                decimal qtyTotal = 0;

                var voucherCode = "select  FN_NEW_VOUCHER_NO('" + model.COMPANY_CODE + "','" + model.form_code + "',TRUNC(sysdate),'SA_SALES_ORDER') from dual";
                var data = _objectEntity.SqlQuery<string>(voucherCode).FirstOrDefault();

                var sessionQuery = "SELECT MYSEQUENCE.NEXTVAL FROM DUAL";
                int sessionId = dbContext.SqlQuery<int>(sessionQuery).FirstOrDefault();


                string vouhcerBranchQuery = $@"select branch_code from form_branch_map where form_code = '{model.form_code}' and company_code = '{model.COMPANY_CODE}'";
                var voucherBranch = dbContext.SqlQuery<string>(vouhcerBranchQuery).FirstOrDefault();


                string actualBranch = voucherBranch;
                if (voucherBranch == null)
                {
                    actualBranch = model.BRANCH_CODE;
                }
                string pref = "";
                string cust = "";

                string prefQuery = $@"select SET_DISCOUNTED_RATE from dist_preference_setup where company_code = '{model.COMPANY_CODE}' and rownum = 1";
                string customQuery = $@"select PO_CUSTOM_RATE from dist_preference_setup where company_code = '{model.COMPANY_CODE}' and rownum = 1";
                pref = dbContext.SqlQuery<string>(prefQuery).FirstOrDefault();
                cust = dbContext.SqlQuery<string>(customQuery).FirstOrDefault();

                foreach (var item in model.products)
                {
                    List<ConversionData> convData = dbContext.SqlQuery<ConversionData>($@"
                            SELECT MU_CODE, CONVERSION_FACTOR, nvl(FRACTION,1) FRACTION, SERIAL_NO
                            FROM ip_item_unit_setup
                            WHERE item_code = '{item.item_code}'
                              AND company_code = '{model.COMPANY_CODE}'
                              AND CONVERSION_FACTOR > 0
                              AND DELETED_FLAG = 'N'
                            ORDER BY SERIAL_NO
                        ").ToList();

                    if (!string.IsNullOrWhiteSpace(item.SECONDARY_UNIT))
                    {
                        if (convData == null || convData.Count == 0)
                            throw new Exception($"No conversion data found for item {item.item_code}");
                        var unitInfo = convData.FirstOrDefault(x => x.MU_CODE == item.SECONDARY_UNIT);
                        if (unitInfo == null)
                            throw new Exception($"Conversion info not found for secondary unit '{item.SECONDARY_UNIT}'");

                        item.quantity = (item.quantity / unitInfo.CONVERSION_FACTOR) * unitInfo.FRACTION;
                        item.calc_rate = (item.calc_rate * unitInfo.CONVERSION_FACTOR) / unitInfo.FRACTION;
                        item.rate = (item.rate * unitInfo.CONVERSION_FACTOR) / unitInfo.FRACTION;
                        item.FREE_QTY = Math.Round(Convert.ToDecimal((Convert.ToDecimal(item.FREE_QTY) / unitInfo.CONVERSION_FACTOR) * unitInfo.FRACTION), 4, MidpointRounding.ToEven).ToString(CultureInfo.InvariantCulture);
                    }

                    item.party_type_code = item.party_type_code ?? "";
                    string InsertQuery = string.Empty;
                    string priceQuery = $"SELECT NVL(SALES_PRICE,0) SALES_PRICE FROM IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '{item.item_code}' AND COMPANY_CODE='{model.COMPANY_CODE}'";
                    decimal SP = dbContext.SqlQuery<decimal>(priceQuery).FirstOrDefault();
                    item.rate = item.rate == 0 ? SP : item.rate;
                    shippingAddress = item.CITY_CODE ?? "";
                    decimal total = 0;
                    if (cust == "Y") item.rate = item.calc_rate;
                    if (pref == "Y")
                    {
                        item.rate = item.calc_rate;
                        total = item.rate * item.quantity;
                        //var discount = item.discount + item.discountRate * item.quantity + (item.discountPercentage * total / 100);
                        //total = Math.Round(total - discount, 2);

                        netTotal += Convert.ToDecimal(total);
                    }
                    else
                    {
                        total = item.calc_rate * item.quantity;
                        //var discount = item.discount + item.discountRate * item.quantity + (item.discountPercentage * total / 100);
                        //total = Math.Round(total - discount, 2);

                        netTotal += Convert.ToDecimal(total);
                    }

                    qtyTotal += Convert.ToDecimal(item.quantity);


                    string FileName = null;
                    string ChequePath = string.Empty;
                    if (Files != null && Files.Count > 0 && Files["orderSignature"] != null)
                    {
                        HttpPostedFile file = Files["orderSignature"];
                        var ImageId = this.GetMaxId("DIST_VISIT_IMAGE", "IMAGE_CODE", dbContext);
                        ChequePath = UploadPath + "\\EntityImages";

                        if (!Directory.Exists(ChequePath))
                            Directory.CreateDirectory(ChequePath);
                        FileName = string.Format("Signature_{0}{1}", id, Path.GetExtension(file.FileName));
                        string filePath = Path.Combine(ChequePath, FileName);
                        int count = 1;
                        while (File.Exists(filePath))
                        {
                            FileName = string.Format("Signature_{0}_{1}{2}", id, count++, Path.GetExtension(file.FileName));
                            filePath = Path.Combine(ChequePath, FileName);
                        }
                        file.SaveAs(filePath);
                    }

                    if (model.type.Equals("P", StringComparison.OrdinalIgnoreCase) || model.type.Equals("DEALER", StringComparison.OrdinalIgnoreCase)
                        || model.type.Equals("D", StringComparison.OrdinalIgnoreCase) || model.type.Equals("DISTRIBUTOR", StringComparison.OrdinalIgnoreCase))
                    {
                        var col = ",FREE_QTY";
                        var val = $@", '{item.FREE_QTY}'";
                        var autocol = "";
                        var autoval = "";
                        if (!string.IsNullOrWhiteSpace(item.FREE_QTY))
                        {
                            autocol = ", FREE_QTY";
                            autoval = $@", '{item.FREE_QTY}'";
                        }

                        int checkFreeQty = dbContext.SqlQuery<int>("SELECT COUNT(*) FROM user_tab_columns WHERE table_name = 'DIST_IP_SSD_PURCHASE_ORDER' AND column_name = 'FREE_QTY'").FirstOrDefault();
                        int checkSecondQty = dbContext.SqlQuery<int>("SELECT COUNT(*) FROM user_tab_columns WHERE table_name = 'DIST_IP_SSD_PURCHASE_ORDER' AND column_name = 'SECOND_QUANTITY'").FirstOrDefault();
                        int checkSecondUnit = dbContext.SqlQuery<int>("SELECT COUNT(*) FROM user_tab_columns WHERE table_name = 'DIST_IP_SSD_PURCHASE_ORDER' AND column_name = 'SECONDARY_UNIT'").FirstOrDefault();

                        if (checkFreeQty == 0 || checkSecondQty == 0 || checkSecondUnit == 0)
                            throw new Exception("Table does not contain the required columns!");

                        decimal primaryQty = item.quantity;

                        //if (!string.IsNullOrWhiteSpace(item.SECONDARY_UNIT))
                        //{
                        //    var unitInfo = convData.FirstOrDefault(x => x.MU_CODE == item.SECONDARY_UNIT);
                        //    if (unitInfo == null)
                        //        throw new Exception($"Conversion info not found for secondary unit '{item.SECONDARY_UNIT}'");

                        //    // Convert to base unit (primary)
                        //    // (Qty in secondary) * CONVERSION_FACTOR / FRACTION
                        //    item.quantity = (item.quantity / unitInfo.CONVERSION_FACTOR) * unitInfo.FRACTION;

                        //    //col = ",FREE_QTY,SECOND_QUANTITY,SECONDARY_UNIT";
                        //    //val = $@", '{item.FREE_QTY}', '{item.quantity}', '{item.SECONDARY_UNIT}'";
                        //}
                        secondaryUnit = null;
                        thirdUnit = null;
                        if (convData.Count >= 1)
                        {
                            foreach (var conv in convData)
                            {
                                decimal converted = (item.quantity * conv.CONVERSION_FACTOR) / conv.FRACTION;

                                if (secondaryUnit == null)
                                {
                                    secondaryUnit = conv.MU_CODE;
                                    secondaryQty = converted;
                                    item.SECONDARY_UNIT = conv.MU_CODE;
                                    item.SECOND_QUANTITY = converted;
                                }
                                else if (thirdUnit == null)
                                {
                                    thirdUnit = conv.MU_CODE;
                                    thirdQty = converted;
                                    item.THIRD_UNIT = conv.MU_CODE;
                                    item.THIRD_QTY = converted;
                                }
                            }

                            if (secondaryUnit != null)
                            {
                                col += ",SECONDARY_UNIT,SECOND_QUANTITY";
                                val += $@", '{secondaryUnit}', '{secondaryQty}'";
                                autocol += ",SECOND_QUANTITY";
                                autoval += $@", '{secondaryQty}'";
                            }
                            if (thirdUnit != null)
                            {
                                col += ",THIRD_UNIT,THIRD_QUANTITY";
                                val += $@", '{thirdUnit}', '{thirdQty}'";
                                autocol += ",THIRD_QUANTITY";
                                autoval += $@", '{thirdQty}'";
                            }
                        }

                        InsertQuery = $@"INSERT INTO DIST_IP_SSD_PURCHASE_ORDER (ORDER_NO,ORDER_DATE,CUSTOMER_CODE,ITEM_CODE,MU_CODE,QUANTITY,BILLING_NAME,REMARKS,UNIT_PRICE,TOTAL_PRICE,CREATED_BY,CREATED_DATE,APPROVED_FLAG,DISPATCH_FLAG,ACKNOWLEDGE_FLAG,REJECT_FLAG,DELETED_FLAG,PARTY_TYPE_CODE,CITY_CODE,SALES_TYPE_CODE,SHIPPING_CONTACT,COMPANY_CODE,BRANCH_CODE,SYNC_ID,TEMP_ORDER_NO,DISCOUNT,DISCOUNT_RATE,DISCOUNT_PERCENTAGE,PRIORITY_STATUS_CODE,SIGNATURE{col})
                            VALUES('{id}',{today},'{model.distributor_code}','{item.item_code}','{item.mu_code}','{item.quantity}','{item.billing_name}','{item.remarks}','{item.rate}','{total}','{model.user_id}',SYSTIMESTAMP,'N','N','N','{item.reject_flag}','N','{item.party_type_code}','{item.CITY_CODE}','{item.SALES_TYPE_CODE}','{item.SHIPPING_CONTACT}','{model.COMPANY_CODE}','{model.BRANCH_CODE}','{item.Sync_Id}','{model.Order_No}','{item.discount}','{item.discountRate}','{item.discountPercentage}','{item.PRIORITY_STATUS_CODE}','{FileName}'{val})";
                        int distInsertResult = dbContext.ExecuteSqlCommand(InsertQuery);

                        if (distInsertResult == 0)
                        {
                            throw new Exception("Failed to insert!");
                        }
                        var preferences = FetchPreferences(model.COMPANY_CODE, dbContext);
                        if (preferences.SO_SALES_ORDER.Trim().ToUpper() == "Y")
                        {


                            if (data == null)
                                throw new Exception("Order number could not be generated. Please try again.");

                            decimal chargeAmount = 0;

                            //string discountQ = $@"select discount, discount_rate, discount_percentage from dist_ip_ssd_purchase_order where item_code = '{item.item_code}' AND ORDER_NO = '{id}'";
                            //var discounts = this._objectEntity.SqlQuery<disccountModel>(discountQ).FirstOrDefault();

                            decimal ItemTotal = Convert.ToDecimal(item.rate) * Convert.ToDecimal(item.quantity);

                            if (item.discount > 0)
                            {
                                chargeAmount = item.discount;
                            }
                            if (item.discountPercentage > 0)
                            {
                                decimal percentageDiscount = (ItemTotal * (item.discountPercentage / 100));
                                chargeAmount = percentageDiscount;
                            }

                            ////decimal cal_amt = Convert.ToInt32(total) - chargeAmount;

                            ////decimal cal_unit_price = cal_amt / Convert.ToInt32(item.quantity);

                            //decimal noDiscountTotal = Convert.ToDecimal(item.quantity) * Convert.ToDecimal(item.rate);

                            //decimal cal_unit_price = Convert.ToDecimal(total) / Convert.ToDecimal(item.quantity);

                            var query = string.Format($@"Insert into SA_SALES_ORDER
                                                                   (ORDER_NO, ORDER_DATE, CUSTOMER_CODE, SERIAL_NO, ITEM_CODE, MU_CODE, QUANTITY, UNIT_PRICE, TOTAL_PRICE, CALC_QUANTITY, CALC_UNIT_PRICE, CALC_TOTAL_PRICE, FORM_CODE, COMPANY_CODE, BRANCH_CODE, CREATED_BY, CREATED_DATE, DELETED_FLAG, DELIVERY_DATE, CURRENCY_CODE, EXCHANGE_RATE, TRACKING_NO, STOCK_BLOCK_FLAG,MODIFY_BY,MODIFY_DATE,PARTY_TYPE_CODE,REMARKS,SESSION_ROWID,PRIORITY_CODE,SHIPPING_ADDRESS, SHIPPING_CONTACT_NO,EMPLOYEE_CODE, SALES_TYPE_CODE {autocol})
                                                                 Values
                                                                   ('" + data + @"', trunc(sysdate), '" + model.distributor_code + @"'," + serialnumber + @",
                                                                    '" + item.item_code + @"', '" + item.mu_code + @"'," + item.quantity + @" , " + item.rate + @", " + ItemTotal + @",
                                                                   " + item.quantity + @" , " + item.calc_rate + @", " + total + @",
                                                                    '" + model.form_code + @"', '" + model.COMPANY_CODE + @"', '" + actualBranch + @"', UPPER('" + model.login_code + @"'), sysdate,
                                                                    'N', TO_DATE(sysdate), 'NRS', 1,
                                                                    '0', 'N',UPPER('" + model.login_code + @"'), trunc(sysdate),'" + item.party_type_code + @"', '" + item.remarks + @"','" + sessionId + @"','" + item.PRIORITY_STATUS_CODE + @"','" + item.CITY_CODE + @"','" + item.SHIPPING_CONTACT + @"','" + spcode + @"','" + item.SALES_TYPE_CODE + $@"' {autoval} )");
                            distInsert = dbContext.ExecuteSqlCommand(query);

                            var chargeTransaction = this._objectEntity.SqlQuery<string>("select charge_transaction from dist_preference_setup").FirstOrDefault();

                            if (chargeTransaction == "Y")
                            {



                                //chargeAmount = Math.Max(chargeAmount, 0);


                                if (chargeAmount > 0)
                                {


                                    string transquery = string.Format(@"select to_number((max(to_number(TRANSACTION_NO)) + 1)) ORDER_NO from CHARGE_TRANSACTION");
                                    int newtransno = this._objectEntity.SqlQuery<int>(transquery).FirstOrDefault();

                                    //var sessionQ = "SELECT MYSEQUENCE.NEXTVAL FROM DUAL";
                                    //int sessionid = this._objectEntity.SqlQuery<int>(sessionQ).FirstOrDefault();

                                    string chargeCode = $@"select charge_code, acc_code, apportion_on, non_gl_flag, impact_on from charge_setup where form_code='{model.form_code}'";
                                    var chargeAcc = this._objectEntity.SqlQuery<chargeAccModel>(chargeCode).FirstOrDefault();

                                    if (chargeAcc == null)
                                    {
                                        throw new Exception("Charge code for the specific form code not found.");
                                    }

                                    string insertChargeQuery = $@"INSERT INTO CHARGE_TRANSACTION
                                            (TRANSACTION_NO,TABLE_NAME,REFERENCE_NO, ITEM_CODE, APPLY_ON, BUDGET_CODE, GL_FLAG, 
                                            ACC_CODE,CHARGE_CODE,CHARGE_TYPE_FLAG,CHARGE_AMOUNT,
                                        FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,CURRENCY_CODE,EXCHANGE_RATE,VOUCHER_NO,CALCULATE_BY,
                                                SERIAL_NO,SESSION_ROWID, APPORTION_flag, NON_GL_FLAG, IMPACT_ON)
                                            VALUES('{newtransno}','SA_SALES_ORDER','{data}','{item.item_code}','I','','N',
                                            '{chargeAcc.ACC_CODE}','{chargeAcc.CHARGE_CODE}','D', {chargeAmount},'{model.form_code}',
                                        '{model.COMPANY_CODE}','{actualBranch}','{model.login_code}',
                                        SYSDATE,'N','NRS',1,'','A',{serialnumber},'{sessionId}', '{chargeAcc.APPORTION_ON}', '{chargeAcc.NON_GL_FLAG}', '{chargeAcc.IMPACT_ON}')";

                                    int chargeRow = _objectEntity.ExecuteSqlCommand(insertChargeQuery);

                                    if (chargeRow == 0)
                                    {
                                        throw new Exception("Couldn't insert in charge transaction");
                                    }

                                }
                            }
                            var partialUpdate = $@"UPDATE DIST_IP_SSD_PURCHASE_ORDER SET REJECT_FLAG='N',BRANCH_CODE = '{actualBranch}',APPROVE_QTY = {item.quantity}, APPROVE_AMT = {total}, QUANTITY=0,SALES_ORDER_NO='{data}' WHERE ORDER_NO = {id} and ITEM_CODE = {item.item_code}";
                            var Pupdate = dbContext.ExecuteSqlCommand(partialUpdate);

                            serialnumber++;
                        }

                    }

                    else
                    {
                        var col = ",FREE_QTY";
                        var val = $@", '{item.FREE_QTY}'";

                        int checkFreeQty = dbContext.SqlQuery<int>("SELECT COUNT(*) FROM user_tab_columns WHERE table_name = 'DIST_IP_SSD_PURCHASE_ORDER' AND column_name = 'FREE_QTY'").FirstOrDefault();
                        int checkSecondQty = dbContext.SqlQuery<int>("SELECT COUNT(*) FROM user_tab_columns WHERE table_name = 'DIST_IP_SSD_PURCHASE_ORDER' AND column_name = 'SECOND_QUANTITY'").FirstOrDefault();
                        int checkSecondUnit = dbContext.SqlQuery<int>("SELECT COUNT(*) FROM user_tab_columns WHERE table_name = 'DIST_IP_SSD_PURCHASE_ORDER' AND column_name = 'SECONDARY_UNIT'").FirstOrDefault();

                        if (checkFreeQty == 0 || checkSecondQty == 0 || checkSecondUnit == 0)
                            throw new Exception("Table does not contain the required columns!");

                        decimal primaryQty = item.quantity;

                        secondaryUnit = null;
                        thirdUnit = null;
                        if (convData.Count >= 1)
                        {
                            foreach (var conv in convData)
                            {
                                decimal converted = (item.quantity * conv.CONVERSION_FACTOR) / conv.FRACTION;

                                if (secondaryUnit == null)
                                {
                                    secondaryUnit = conv.MU_CODE;
                                    secondaryQty = converted;
                                    item.SECONDARY_UNIT = conv.MU_CODE;
                                    item.SECOND_QUANTITY = converted;
                                }
                                else if (thirdUnit == null)
                                {
                                    thirdUnit = conv.MU_CODE;
                                    thirdQty = converted;
                                    item.THIRD_UNIT = conv.MU_CODE;
                                    item.THIRD_QTY = converted;
                                }
                            }

                            if (secondaryUnit != null)
                            {
                                col += ",SECONDARY_UNIT,SECOND_QUANTITY";
                                val += $@", '{secondaryUnit}', '{secondaryQty}'";
                            }
                            if (thirdUnit != null)
                            {
                                col += ",THIRD_UNIT,THIRD_QUANTITY";
                                val += $@", '{thirdUnit}', '{thirdQty}'";
                            }
                        }

                        InsertQuery = $@"INSERT INTO DIST_IP_SSR_PURCHASE_ORDER (ORDER_NO,ORDER_DATE,RESELLER_CODE,CUSTOMER_CODE,ITEM_CODE,MU_CODE,QUANTITY,BILLING_NAME,REMARKS,UNIT_PRICE,TOTAL_PRICE,CREATED_BY,CREATED_DATE,APPROVED_FLAG,DISPATCH_FLAG,ACKNOWLEDGE_FLAG,REJECT_FLAG,DELETED_FLAG,PARTY_TYPE_CODE,CITY_CODE,SALES_TYPE_CODE,SHIPPING_CONTACT,COMPANY_CODE,BRANCH_CODE,SYNC_ID,TEMP_ORDER_NO,DISPATCH_FROM,WHOLESELLER_CODE,PRIORITY_STATUS_CODE,DISCOUNT,DISCOUNT_RATE,DISCOUNT_PERCENTAGE,SIGNATURE{col})
                            VALUES('{id}',{today},'{model.reseller_code}','{model.distributor_code}','{item.item_code}','{item.mu_code}','{item.quantity}','{item.billing_name}','{item.remarks}','{item.rate}','{total}','{model.user_id}',TO_DATE('{model.Saved_Date}','MM/dd/yyyy hh24:mi:ss'),'N','N','N','N','N','{item.party_type_code}','{item.CITY_CODE}','{item.SALES_TYPE_CODE}','{item.SHIPPING_CONTACT}','{model.COMPANY_CODE}','{model.BRANCH_CODE}','{item.Sync_Id}','{model.Order_No}','{model.Dispatch_From}','{model.WholeSeller_Code}','{item.PRIORITY_STATUS_CODE}','{item.discount}','{item.discountRate}','{item.discountPercentage}','{FileName}'{val})";
                        int rowNum = dbContext.ExecuteSqlCommand(InsertQuery);
                    }
                }

                if (distInsert > 0)
                {
                    var masterQuery = @"Insert into MASTER_TRANSACTION
                                       (VOUCHER_NO, VOUCHER_AMOUNT, FORM_CODE, CHECKED_BY, AUTHORISED_BY, POSTED_BY, COMPANY_CODE, BRANCH_CODE, CREATED_BY, CREATED_DATE, DELETED_FLAG, VOUCHER_DATE, CURRENCY_CODE, EXCHANGE_RATE, PRINT_COUNT,PRINT_FLAG,SESSION_ROWID)
                                     Values
                                       ('" + data + @"', " + netTotal + @", '" + model.form_code + @"', '', '',
                                        '', '" + model.COMPANY_CODE + "', '" + actualBranch + "', UPPER('" + model.login_code + @"'),
                                        sysdate, 'N', trunc(sysdate),'NRS', 1, 0,'N', '" + sessionId + @"')";
                    var masterRowaffected = dbContext.ExecuteSqlCommand(masterQuery);
                }
                //For PDF Generation and Sharing
                var entity = "D";
                if (model.type.Equals("P", StringComparison.OrdinalIgnoreCase) || model.type.Equals("DEALER", StringComparison.OrdinalIgnoreCase)
                || model.type.Equals("D", StringComparison.OrdinalIgnoreCase) || model.type.Equals("DISTRIBUTOR", StringComparison.OrdinalIgnoreCase))
                //id = this.GetMaxId("DIST_IP_SSD_PURCHASE_ORDER", "ORDER_NO", dbContext);
                {
                    entity = "D";
                }
                else if (model.type.Equals("R", StringComparison.OrdinalIgnoreCase) || model.type.Equals("RESELLER", StringComparison.OrdinalIgnoreCase) || model.type.Equals("outlet", StringComparison.OrdinalIgnoreCase))
                {
                    entity = "R";
                }
                // Get distinct city_code from items
                var cityCode = model.products.Select(x => x.CITY_CODE).FirstOrDefault();
                if (!string.IsNullOrEmpty(cityCode))
                {
                    string cityQuery = $@"SELECT city_edesc 
                                  FROM city_code 
                                  WHERE city_code = '{cityCode}' 
                                  AND ROWNUM = 1";
                    shippingAddress = dbContext.SqlQuery<string>(cityQuery).FirstOrDefault();
                }
                else
                {
                    shippingAddress = model.products.Select(x => x.RESELLER_SHIPPING_ADDRESS).FirstOrDefault();
                }

                // Get distinct shipping contact
                var shippingContact = model.products.Select(x => x.SHIPPING_CONTACT).FirstOrDefault();

                // Get customer name
                string customerName = string.Empty;
                string custCode = model.distributor_code ?? model.reseller_code;
                if (entity == "D")
                {
                    string custQuery = $@"SELECT customer_edesc 
                                  FROM sa_customer_setup 
                                  WHERE company_code = '{model.COMPANY_CODE}' 
                                  AND customer_code = '{custCode}' 
                                  AND deleted_flag = 'N'";
                    customerName = dbContext.SqlQuery<string>(custQuery).FirstOrDefault();
                }
                else
                {
                    string custQuery = $@"SELECT RESELLER_NAME from DIST_RESELLER_MASTER WHERE company_code = '{model.COMPANY_CODE}' AND RESELLER_CODE = '{model.reseller_code}' AND DELETED_FLAG = 'N'";
                    customerName = dbContext.SqlQuery<string>(custQuery).FirstOrDefault();
                }

                // Get user full name
                string userName = string.Empty;
                if (!string.IsNullOrEmpty(model.user_id))
                {
                    string userQuery = $@"SELECT full_name 
                                  FROM dist_login_user 
                                  WHERE userid = '{model.user_id}'";
                    userName = dbContext.SqlQuery<string>(userQuery).FirstOrDefault();
                }

                // Get item descriptions (for each item)
                var itemCodes = model.products.Select(x => x.item_code).Distinct().ToList();
                var itemDescDict = new Dictionary<string, string>();

                if (itemCodes.Any())
                {
                    string codesIn = string.Join(",", itemCodes.Select(c => $"'{c}'"));
                    string itemQuery = $@"
        SELECT item_code, item_edesc
        FROM ip_item_master_setup
        WHERE item_code IN ({codesIn})
          AND company_code = '{model.COMPANY_CODE}'
          AND deleted_flag = 'N'";

                    var items = dbContext.Database.SqlQuery<ProductModel>(itemQuery).ToList();

                    foreach (var item in items)
                    {
                        itemDescDict[item.item_code] = item.item_edesc;
                    }
                }

                foreach (var item in model.products)
                {
                    if (itemDescDict.ContainsKey(item.item_code))
                        item.item_edesc = itemDescDict[item.item_code];
                }
                string shareQ = $@"select PO_SHARE_FLAG from dist_preference_setup where company_code = '{model.COMPANY_CODE}' and rownum = 1";
                var share = dbContext.SqlQuery<string>(shareQ).FirstOrDefault();
                var freeQty = "";
                int checkFreePrefQty = dbContext.SqlQuery<int>("SELECT COUNT(*) FROM user_tab_columns WHERE table_name = 'DIST_PREFERENCE_SETUP' AND column_name = 'HAS_FREE_QUANTITY'").FirstOrDefault();
                if (checkFreePrefQty == 0)
                    freeQty = "N";
                else
                {
                    string freeQtyQ = $@"select TRIM(HAS_FREE_QUANTITY) from dist_preference_setup where company_code = '{model.COMPANY_CODE}' and rownum = 1";
                    freeQty = dbContext.SqlQuery<string>(freeQtyQ).FirstOrDefault();
                }

                if (share == "Y")
                {
                    string altQty = dbContext.SqlQuery<string>($"select TRIM(ALT_QTY_PDF) from DIST_PREFERENCE_SETUP where company_code = '{model.COMPANY_CODE}' and rownum = 1").FirstOrDefault().ToString();
                    pdfLink = GeneratePurchaseOrderPdf(model, id, entity, netTotal, qtyTotal, customerName, userName, shippingAddress, shippingContact, freeQty, secondaryUnit, secondaryQty, thirdUnit, thirdQty, altQty);
                    return pdfLink.ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return id.ToString();
        }

        public bool NewCollection(CollectionRequestModel model, HttpFileCollection Files, NeoErpCoreEntity dbContext)
        {
            if (string.IsNullOrWhiteSpace(model.sp_code))
                throw new Exception("Sp code is empty");
            if (string.IsNullOrWhiteSpace(model.entity_type))
                throw new Exception("Entity type is empty");
            if (string.IsNullOrWhiteSpace(model.created_by))
                throw new Exception("Created by is empty");
            decimal Amount;
            string[] types = { "P", "D", "R" };
            if (string.IsNullOrWhiteSpace(model.amount) || !decimal.TryParse(model.amount, out Amount))
                throw new Exception("Amount should be in Number");
            if (!types.Contains(model.entity_type.ToUpper()))
                throw new Exception(@"ENITY_TYPE must be 'P' or 'D' or 'R' ");

            var id = this.GetMaxId("DIST_COLLECTION", "ID", dbContext);
            string insertQuery = $@"INSERT INTO DIST_COLLECTION (SP_CODE,ENTITY_CODE,ENTITY_TYPE,BILL_NO, CHEQUE_NO, BANK_NAME, AMOUNT,PAYMENT_MODE,CHEQUE_CLEARANCE_DATE,CHEQUE_DEPOSIT_BANK,LATITUDE,LONGITUDE,REMARKS,CREATED_BY,DELETED_FLAG,COMPANY_CODE,BRANCH_CODE,OTP_CODE,ID)
            VALUES ('{model.sp_code}','{model.entity_code}','{model.entity_type}','{model.bill_no}','{model.cheque_no}','{model.bank_name}','{model.amount}','{model.payment_mode}',TO_DATE('{model.cheque_clearance_date}','dd-mm-yyyy'),
            '{model.cheque_deposit_bank}', '{model.latitude}','{model.longitude}','{model.remarks}','{model.created_by}','N','{model.COMPANY_CODE}','{model.BRANCH_CODE}','{model.otp_code}',{id})";
            var row = dbContext.ExecuteSqlCommand(insertQuery);
            string checkPdcFlag = $@"SELECT AUTO_PDC FROM DIST_PREFERENCE_SETUP WHERE COMPANY_CODE = '{model.COMPANY_CODE}'";
            var pdcFlag = dbContext.SqlQuery<string>(checkPdcFlag).FirstOrDefault();
            if (pdcFlag == "Y" && model.payment_mode == "CHEQUE" && model.entity_type == "D")
            {
                string max_receipt = dbContext.SqlQuery<string>("SELECT LPAD(nvl(MAX(RECEIPT_NO),0) + 1, 5, '0') as receipt_no FROM FA_PDC_RECEIPTS").FirstOrDefault();

                string pdcRowQuery = $@"INSERT INTO FA_PDC_RECEIPTS
                    (
                        RECEIPT_NO, RECEIPT_DATE, CHEQUE_DATE, CUSTOMER_CODE, PDC_AMOUNT, BANK_NAME, REMARKS, BRANCH_CODE, COMPANY_CODE, 
                        CREATED_BY,CREATED_DATE, MANUAL_NO, CHEQUE_NO, PDC_DETAILS
                    ) VALUES
                    (
                        '{max_receipt}', TRUNC(SYSDATE), TO_DATE('{model.cheque_clearance_date}','dd/mm/yyyy'), '{model.entity_code}', '{model.amount}', '{model.bank_name}', '{model.remarks}', '{model.BRANCH_CODE}', '{model.COMPANY_CODE}', 
                        '{model.created_by}', SYSDATE, '{model.cheque_no}', '{model.cheque_no}', ' '
                    )";
                var pdcRow = dbContext.ExecuteSqlCommand(pdcRowQuery);
                int checkFlag = dbContext.SqlQuery<int>("SELECT COUNT(*) FROM user_tab_columns WHERE table_name = 'DIST_COLLECTION' AND column_name = 'PDC_FLAG'").FirstOrDefault();
                if (checkFlag == 0)
                    dbContext.ExecuteSqlCommand("ALTER TABLE DIST_COLLECTION ADD PDC_FLAG VARCHAR2(1) DEFAULT 'N'");
                dbContext.ExecuteSqlCommand($@"update dist_collection set PDC_FLAG = 'Y' where id = '{id}'");
            }
            if (row <= 0)
                throw new Exception("Unable to save collection");

            foreach (string tagName in Files)
            {
                HttpPostedFile file = Files[tagName];
                string ChequePath = string.Empty;

                var ImageId = this.GetMaxId("DIST_VISIT_IMAGE", "IMAGE_CODE", dbContext);
                ChequePath = UploadPath + "\\EntityImages";

                if (!Directory.Exists(ChequePath))
                    Directory.CreateDirectory(ChequePath);
                string FileName = string.Format("{0}{1}", model.entity_code, Path.GetExtension(file.FileName));
                string filePath = Path.Combine(ChequePath, FileName);
                int count = 1;
                while (File.Exists(filePath))
                {
                    FileName = string.Format("{0}_{1}{2}", model.entity_code, count++, Path.GetExtension(file.FileName));
                    filePath = Path.Combine(ChequePath, FileName);
                }
                string mediaType;
                if (tagName.IndexOf("cheque", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    mediaType = "cheque";
                    //string chequeQuery = $"SELECT CATEGORYID FROM DIST_IMAGE_CATEGORY WHERE CATEGORY_CODE = 'cheque'";
                    //categoryId = dbContext.SqlQuery<int>(chequeQuery).FirstOrDefault();
                    file.SaveAs(filePath);
                }
                else if (tagName.IndexOf("signature", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    mediaType = "signature";
                    //string chequeQuery = $"SELECT CATEGORYID FROM DIST_IMAGE_CATEGORY WHERE CATEGORY_CODE = 'signature'";
                    //categoryId = dbContext.SqlQuery<int>(chequeQuery).FirstOrDefault();
                    file.SaveAs(filePath);
                }
                else if (tagName.IndexOf("cash", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    mediaType = "cash";
                    //string chequeQuery = $"SELECT CATEGORYID FROM DIST_IMAGE_CATEGORY WHERE CATEGORY_CODE = 'signature'";
                    //categoryId = dbContext.SqlQuery<int>(chequeQuery).FirstOrDefault();
                    file.SaveAs(filePath);
                }
                else if (tagName.IndexOf("ips", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    mediaType = "ips";
                    //string chequeQuery = $"SELECT CATEGORYID FROM DIST_IMAGE_CATEGORY WHERE CATEGORY_CODE = 'signature'";
                    //categoryId = dbContext.SqlQuery<int>(chequeQuery).FirstOrDefault();
                    file.SaveAs(filePath);
                }
                else
                    continue;

                var InsertQuery = $@"INSERT INTO DIST_VISIT_IMAGE (IMAGE_CODE,IMAGE_NAME,IMAGE_TITLE,IMAGE_DESC,SP_CODE,ENTITY_CODE,TYPE,UPLOAD_DATE,LONGITUDE,LATITUDE,COMPANY_CODE,BRANCH_CODE,SYNC_ID,ID)
                                    VALUES ({ImageId}, '{FileName}', '{DBNull.Value}', '{mediaType}', '{model.sp_code}', '{model.entity_code}', '{model.entity_type}',TO_DATE('{model.Saved_Date}','MM/dd/yyyy  HH24:MI:SS'),'{model.longitude}', '{model.latitude}','{model.COMPANY_CODE}', '{model.BRANCH_CODE}','{model.Sync_Id}',{id})";
                var rows = dbContext.ExecuteSqlCommand(InsertQuery);
            }

            /*sashi*/

            return true;
        }
        public bool OdoMeterClaim(Dictionary<string, string> model, HttpFileCollection Files, NeoErpCoreEntity dbContext)
        {
            if (string.IsNullOrWhiteSpace(model["SP_CODE"]))
                throw new Exception("Sp code is empty");
            if (string.IsNullOrWhiteSpace(model["CLAIM_CODE"]))
                throw new Exception("Claim code is empty");

            try
            {
                var sp_code = model["SP_CODE"];
                var claim_code = model["CLAIM_CODE"];
                var mode_code = model["MODE_CODE"] ?? "";
                var travelPurpose = model["TRAVEL_PURPOSE"] ?? "";
                var MILEAGE = model["MILEAGE"] ?? "";
                var fuel = model["FUEL_PRICE"] ?? "";
                var exp = model["TOTAL_EXPENSE"] ?? "";

                var sequenceQuery = "SELECT CLAIM_REPORT_SEQ.NEXTVAL FROM DUAL";
                var claim_id = dbContext.SqlQuery<int>(sequenceQuery).FirstOrDefault();

                var checkInRemarks = model["IN_REMARKS"];
                var checkOutRemarks = model["OUT_REMARKS"];
                var checkInKm = model["START_KM_READING"];
                var checkOutKm = model["END_KM_READING"];

                var checkInValue = string.IsNullOrWhiteSpace(model["CHECK_IN_TIME"]) ? "NULL" : $"TO_DATE('{model["CHECK_IN_TIME"]}', 'YYYY-MM-DD HH:MI AM')";
                var checkOutValue = string.IsNullOrWhiteSpace(model["CHECK_OUT_TIME"]) ? "NULL" : $"TO_DATE('{model["CHECK_OUT_TIME"]}', 'YYYY-MM-DD HH:MI AM')";

                var checkInImg = string.Empty;
                var checkOutImg = string.Empty;

                foreach (string tagName in Files)
                {
                    //var sequenceImgQuery = "SELECT CLAIM_IMAGES_SEQ.NEXTVAL FROM DUAL";
                    //var image_id = dbContext.SqlQuery<int>(sequenceImgQuery).FirstOrDefault();
                    string checkPath = string.Empty;
                    string claimPath = model["CLAIM_CODE"];
                    if (model["CLAIM_CODE"] == "VA")
                    {
                        if (tagName == "CHECK_IN_IMAGE")
                        {
                            checkPath = "\\CheckIn";
                        }
                        else if (tagName == "CHECK_IN_IMAGE")
                        {
                            checkPath = "\\CheckOut";
                        }
                        else
                        {
                            return false;
                        }
                    }

                    HttpPostedFile file = Files[tagName];
                    string imgPath = string.Empty;
                    string imgSavePath = string.Empty;

                    imgPath = UploadPath + "\\OdoMeterImages\\" + claimPath + checkPath;
                    imgSavePath = "Areas\\NeoErp.Distribution\\Images\\OdoMeterImages\\" + claimPath + checkPath;

                    if (!Directory.Exists(imgPath))
                        Directory.CreateDirectory(imgPath);
                    string FileName = string.Format("{0}{1}", model["SP_CODE"], Path.GetExtension(file.FileName));
                    string filePath = Path.Combine(imgPath, FileName);
                    string fileSavePath = Path.Combine(imgSavePath, FileName);
                    int count = 1;
                    while (File.Exists(filePath))
                    {
                        FileName = string.Format("{0}_{1}{2}", model["SP_CODE"], count++, Path.GetExtension(file.FileName));
                        filePath = Path.Combine(imgPath, FileName);
                        fileSavePath = Path.Combine(imgSavePath, FileName);
                    }
                    file.SaveAs(filePath);

                    fileSavePath = fileSavePath.Replace("\\", "/");

                    if (model["CLAIM_CODE"] == "VA")
                    {
                        if (tagName == "CHECK_IN_IMAGE")
                        {
                            checkInImg = fileSavePath;
                        }
                        else if (tagName == "CHECK_IN_IMAGE")
                        {
                            checkOutImg = fileSavePath;
                        }
                    }

                }
                string claimInsertQuery = $@"
                    INSERT INTO DIST_CLAIM_REPORT 
                    (
                        CLAIM_ID,
                        SP_CODE,
                        CLAIM_CODE,
                        TRAVEL_MODE,
                        TRAVEL_PURPOSE,
                        VEHICLE_MILEAGE,
                        FUEL_PRICE,
                        START_KM_READING,
                        END_KM_READING,
                        CHECK_IN,
                        CHECK_OUT,
                        TOTAL_EXPENSE,
                        IN_REMARKS,
                        OUT_REMARKS,
                        CREATED_BY,
                        COMPANY_CODE, BRANCH_CODE,
                        CHECK_IN_IMAGE,
                        CHECK_OUT_IMAGE
                    ) VALUES 
                    (
                        '{claim_id}',               -- CLAIM_ID
                        '{sp_code}',                -- SP_CODE
                        '{claim_code}',             -- CLAIM_CODE
                        '{mode_code}',              -- TRAVEL_MODE
                        '{travelPurpose}',          -- TRAVEL_PURPOSE
                        '{MILEAGE}',                -- VEHICLE_MILEAGE
                        '{fuel}',                   -- FUEL_PRICE
                        '{checkInKm}',              -- START_KM_READING
                        '{checkOutKm}',             -- END_KM_READING
                        {checkInValue},             -- CHECK_IN
                        {checkOutValue},            -- CHECK_OUT
                        '{exp}',                    -- TOTAL_EXPENSE
                        '{checkInRemarks}',         -- IN_REMARKS
                        '{checkOutRemarks}',        -- OUT_REMARKS
                        (select USER_NAME from dist_login_user WHERE SP_CODE = '{sp_code}'),       -- CREATED_BY
                        '{model["COMPANY_CODE"]}', '{model["BRANCH_CODE"]}',
                        '{checkInImg}', '{checkOutImg}' -- Images
                    )
                    ";
                string claimLogQuery = $@"
                    INSERT INTO DIST_CLAIM_REPORT_UPDATE_LOG
                    (
                        CLAIM_ID,
                        SP_CODE,
                        CLAIM_CODE,
                        TRAVEL_MODE,
                        TRAVEL_PURPOSE,
                        VEHICLE_MILEAGE,
                        FUEL_PRICE,
                        START_KM_READING,
                        END_KM_READING,
                        CHECK_IN,
                        CHECK_OUT,
                        TOTAL_EXPENSE,
                        IN_REMARKS,
                        OUT_REMARKS,
                        CREATED_BY,
                        CHECK_IN_IMAGE,
                        CHECK_OUT_IMAGE
                    ) VALUES 
                    (
                        '{claim_id}',               -- CLAIM_ID
                        '{sp_code}',                -- SP_CODE
                        '{claim_code}',             -- CLAIM_CODE
                        '{mode_code}',              -- TRAVEL_MODE
                        '{travelPurpose}',          -- TRAVEL_PURPOSE
                        '{MILEAGE}',                -- VEHICLE_MILEAGE
                        '{fuel}',                   -- FUEL_PRICE
                        '{checkInKm}',              -- START_KM_READING
                        '{checkOutKm}',             -- END_KM_READING
                        {checkInValue},             -- CHECK_IN
                        {checkOutValue},            -- CHECK_OUT
                        '{exp}',                    -- TOTAL_EXPENSE
                        '{checkInRemarks}',         -- IN_REMARKS
                        '{checkOutRemarks}',        -- OUT_REMARKS
                        (select USER_NAME from dist_login_user WHERE SP_CODE = '{sp_code}'),       -- CREATED_BY
                        '{checkInImg}', '{checkOutImg}' -- Images
                    )
                    ";

                var row = dbContext.ExecuteSqlCommand(claimInsertQuery);
                var row2 = dbContext.ExecuteSqlCommand(claimLogQuery);
                if (row == 0)
                {
                    throw new Exception("Something went wrong! Could not insert.");
                }
                if (row2 == 0)
                {
                    throw new Exception("Could not maintain update log.");
                }

                return true;

            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong! " + ex.Message);
            }

        }
        public bool UpdateOdoMeterClaim(Dictionary<string, string> model, HttpFileCollection Files, NeoErpCoreEntity dbContext)
        {
            if (string.IsNullOrWhiteSpace(model["SP_CODE"]))
                throw new Exception("Sp code is empty");
            if (string.IsNullOrWhiteSpace(model["CLAIM_CODE"]))
                throw new Exception("Claim code is empty");
            if (string.IsNullOrWhiteSpace(model["CLAIM_ID"]))
                throw new Exception("Claim ID is empty");

            try
            {
                var sp_code = model["SP_CODE"];
                var claim_code = model["CLAIM_CODE"];
                var mode_code = model["MODE_CODE"] ?? "";
                var travelPurpose = model["TRAVEL_PURPOSE"] ?? "";
                var MILEAGE = model["MILEAGE"] ?? "";
                var fuel = model["FUEL_PRICE"] ?? "";
                var exp = model["TOTAL_EXPENSE"] ?? "";

                var claim_id = model["CLAIM_ID"];

                var checkInRemarks = model["IN_REMARKS"];
                var checkOutRemarks = model["OUT_REMARKS"];
                var checkInKm = model["START_KM_READING"];
                var checkOutKm = model["END_KM_READING"];

                var checkInValue = string.IsNullOrWhiteSpace(model["CHECK_IN_TIME"]) ? "NULL" : $"TO_DATE('{model["CHECK_IN_TIME"]}', 'YYYY-MM-DD HH:MI AM')";
                var checkOutValue = string.IsNullOrWhiteSpace(model["CHECK_OUT_TIME"]) ? "NULL" : $"TO_DATE('{model["CHECK_OUT_TIME"]}', 'YYYY-MM-DD HH:MI AM')";

                var checkInImg = string.Empty;
                var checkOutImg = string.Empty;
                var billImg = string.Empty;

                foreach (string tagName in Files)
                {
                    HttpPostedFile file = Files[tagName];
                    if (file == null) continue;

                    //var sequenceImgQuery = "SELECT CLAIM_IMAGES_SEQ.NEXTVAL FROM DUAL";
                    //var image_id = dbContext.SqlQuery<int>(sequenceImgQuery).FirstOrDefault();

                    string claimPath = model["CLAIM_CODE"];
                    string checkPath = "";

                    if (model["CLAIM_CODE"] == "VA")
                    {
                        if (tagName == "CHECK_IN_IMAGE") checkPath = "\\CheckIn";
                        else if (tagName == "CHECK_OUT_IMAGE") checkPath = "\\CheckOut";
                        else checkPath = "\\Bill";
                    }

                    string imgPath = Path.Combine(UploadPath, "OdoMeterImages", claimPath + checkPath);
                    string imgSavePath = Path.Combine("Areas\\NeoErp.Distribution\\Images\\OdoMeterImages", claimPath + checkPath);

                    if (!Directory.Exists(imgPath))
                        Directory.CreateDirectory(imgPath);

                    string fileName = $"{model["SP_CODE"]}{Path.GetExtension(file.FileName)}";
                    string filePath = Path.Combine(imgPath, fileName);
                    string fileSaveFilePath = Path.Combine(imgSavePath, fileName);

                    int count = 1;
                    while (File.Exists(filePath))
                    {
                        fileName = $"{model["SP_CODE"]}_{count++}{Path.GetExtension(file.FileName)}";
                        filePath = Path.Combine(imgPath, fileName);
                        fileSaveFilePath = Path.Combine(imgSavePath, fileName);
                    }
                    file.SaveAs(filePath);

                    fileSaveFilePath = fileSaveFilePath.Replace("\\", "/");

                    if (tagName == "CHECK_IN_IMAGE") checkInImg = fileSaveFilePath;
                    else if (tagName == "CHECK_OUT_IMAGE") checkOutImg = fileSaveFilePath;
                    else billImg = fileSaveFilePath;
                }

                var updateFields = $@"
                    TRAVEL_MODE = '{mode_code}',
                    TRAVEL_PURPOSE = '{travelPurpose}',
                    VEHICLE_MILEAGE = '{MILEAGE}',
                    FUEL_PRICE = '{fuel}',
                    START_KM_READING = '{checkInKm}',
                    END_KM_READING = '{checkOutKm}',
                    CHECK_IN = {checkInValue},
                    CHECK_OUT = {checkOutValue},
                    TOTAL_EXPENSE = '{exp}',
                    IN_REMARKS = '{checkInRemarks}',
                    OUT_REMARKS = '{checkOutRemarks}',
                    BILL_IMG = '{billImg}'";

                if (!string.IsNullOrEmpty(checkInImg))
                    updateFields += $", CHECK_IN_IMAGE = '{checkInImg}'";
                if (!string.IsNullOrEmpty(checkOutImg))
                    updateFields += $", CHECK_OUT_IMAGE = '{checkOutImg}'";

                string claimUpdateQuery = $@"
                        UPDATE DIST_CLAIM_REPORT
                        SET {updateFields}
                        WHERE SP_CODE = '{sp_code}' 
                          AND CLAIM_ID = '{claim_id}'";

                string claimLogQuery = $@"
                    INSERT INTO DIST_CLAIM_REPORT_UPDATE_LOG
                    (
                        CLAIM_ID,
                        SP_CODE,
                        CLAIM_CODE,
                        TRAVEL_MODE,
                        TRAVEL_PURPOSE,
                        VEHICLE_MILEAGE,
                        FUEL_PRICE,
                        START_KM_READING,
                        END_KM_READING,
                        CHECK_IN,
                        CHECK_OUT,
                        TOTAL_EXPENSE,
                        IN_REMARKS,
                        OUT_REMARKS,
                        CREATED_BY,
                        CHECK_IN_IMAGE,
                        CHECK_OUT_IMAGE,
                        BILL_IMG
                    ) VALUES 
                    (
                        '{claim_id}',               -- CLAIM_ID
                        '{sp_code}',                -- SP_CODE
                        '{claim_code}',             -- CLAIM_CODE
                        '{mode_code}',              -- TRAVEL_MODE
                        '{travelPurpose}',          -- TRAVEL_PURPOSE
                        '{MILEAGE}',                -- VEHICLE_MILEAGE
                        '{fuel}',                   -- FUEL_PRICE
                        '{checkInKm}',              -- START_KM_READING
                        '{checkOutKm}',             -- END_KM_READING
                        {checkInValue},             -- CHECK_IN
                        {checkOutValue},            -- CHECK_OUT
                        '{exp}',                    -- TOTAL_EXPENSE
                        '{checkInRemarks}',         -- IN_REMARKS
                        '{checkOutRemarks}',        -- OUT_REMARKS
                        (select USER_NAME from dist_login_user WHERE SP_CODE = '{sp_code}'),       -- CREATED_BY
                        '{checkInImg}', '{checkOutImg}', '{billImg}' -- Images
                    )
                    ";

                var row = dbContext.ExecuteSqlCommand(claimUpdateQuery);
                var row2 = dbContext.ExecuteSqlCommand(claimLogQuery);

                if (row == 0)
                {
                    throw new Exception("Something went wrong! Could not insert.");
                }
                if (row2 == 0)
                {
                    throw new Exception("Could not maintain update log.");
                }
                return true;

            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong! " + ex.Message);
            }

        }
        public bool OdoMeterVehicleSetup(Dictionary<string, string> model, HttpFileCollection Files, NeoErpCoreEntity dbContext)
        {
            if (string.IsNullOrWhiteSpace(model["SP_CODE"]) || string.IsNullOrWhiteSpace(model["MODE_CODE"]) || string.IsNullOrWhiteSpace(model["VEHICLE_NO"]) ||
                string.IsNullOrWhiteSpace(model["MODEL"]) || string.IsNullOrWhiteSpace(model["MILEAGE"]) || string.IsNullOrWhiteSpace(model["YEAR_MADE"]) ||
                string.IsNullOrWhiteSpace(model["FUEL_PRICE"]))
                throw new Exception("The fields cannot be empty.");
            try
            {
                var sp_code = model["SP_CODE"];
                var vehicle_code = model["MODE_CODE"];
                var vehicle_no = model["VEHICLE_NO"] ?? "";
                var vehicle_model = model["MODEL"] ?? "";
                var MILEAGE = model["MILEAGE"] ?? "";
                var fuel = model["FUEL_PRICE"] ?? "";
                var year_made = model["YEAR_MADE"] ?? "";
                string query = "";

                string imgPath = string.Empty;
                string imgSavePath = string.Empty;
                string fileSavePath = string.Empty;

                // handle files if uploaded
                if (Files != null && Files.Count > 0)
                {
                    foreach (string tagName in Files)
                    {
                        HttpPostedFile file = Files[tagName];

                        imgPath = UploadPath + "\\OdoMeterImages\\VehicleImage";
                        imgSavePath = "Areas\\NeoErp.Distribution\\Images\\OdoMeterImages\\VehicleImage";

                        if (!Directory.Exists(imgPath))
                            Directory.CreateDirectory(imgPath);

                        string FileName = string.Format("{0}{1}", model["SP_CODE"], Path.GetExtension(file.FileName));
                        string filePath = Path.Combine(imgPath, FileName);
                        fileSavePath = Path.Combine(imgSavePath, FileName);

                        int count = 1;
                        while (File.Exists(filePath))
                        {
                            FileName = string.Format("{0}_{1}{2}", model["SP_CODE"], count++, Path.GetExtension(file.FileName));
                            filePath = Path.Combine(imgPath, FileName);
                            fileSavePath = Path.Combine(imgSavePath, FileName);
                        }
                        file.SaveAs(filePath);
                    }
                }

                query = $@"
                         INSERT INTO DIST_VEHICLE_SETUP 
                         (SP_CODE, VEHICLE_CODE, VEHICLE_NO, VEHICLE_MODEL, MILAGE, FUEL_PRICE, YEAR_MADE, IMAGE, CREATED_BY, COMPANY_CODE, BRANCH_CODE)
                         VALUES ('{sp_code}', '{vehicle_code}', '{vehicle_no}', '{vehicle_model}', '{MILEAGE}', '{fuel}', '{year_made}', '{fileSavePath}', (select lower(user_name) from dist_login_user where sp_code = '{sp_code}'), '{model["COMPANY_CODE"]}', '{model["BRANCH_CODE"]}')
                 ";

                var imgRow = dbContext.ExecuteSqlCommand(query);
                if (imgRow == 0)
                {
                    throw new Exception("Something went wrong! Could not insert.");
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong! " + ex.Message);
            }

        }
        public bool OdoMeterUpdateVehicleSetup(Dictionary<string, string> model, HttpFileCollection Files, NeoErpCoreEntity dbContext)
        {
            if (string.IsNullOrWhiteSpace(model["SP_CODE"]) || string.IsNullOrWhiteSpace(model["MODE_CODE"]) || string.IsNullOrWhiteSpace(model["VEHICLE_NO"]) ||
                string.IsNullOrWhiteSpace(model["MODEL"]) || string.IsNullOrWhiteSpace(model["MILEAGE"]) || string.IsNullOrWhiteSpace(model["YEAR_MADE"]) ||
                string.IsNullOrWhiteSpace(model["FUEL_PRICE"]))
                throw new Exception("The fields cannot be empty.");
            try
            {
                var id = model["ID"];
                var sp_code = model["SP_CODE"];
                var vehicle_code = model["MODE_CODE"];
                var vehicle_no = model["VEHICLE_NO"] ?? "";
                var vehicle_model = model["MODEL"] ?? "";
                var milage = model["MILEAGE"] ?? "";
                var fuel = model["FUEL_PRICE"] ?? "";
                var year_made = model["YEAR_MADE"] ?? "";
                string query = "";

                string imgPath = string.Empty;
                string imgSavePath = string.Empty;
                string fileSavePath = string.Empty;

                // handle files if uploaded
                if (Files != null && Files.Count > 0)
                {
                    foreach (string tagName in Files)
                    {
                        HttpPostedFile file = Files[tagName];

                        imgPath = UploadPath + "\\OdoMeterImages\\VehicleImage";
                        imgSavePath = "Areas\\NeoErp.Distribution\\Images\\OdoMeterImages\\VehicleImage";

                        if (!Directory.Exists(imgPath))
                            Directory.CreateDirectory(imgPath);

                        string FileName = string.Format("{0}{1}", model["SP_CODE"], Path.GetExtension(file.FileName));
                        string filePath = Path.Combine(imgPath, FileName);
                        fileSavePath = Path.Combine(imgSavePath, FileName);

                        int count = 1;
                        while (File.Exists(filePath))
                        {
                            FileName = string.Format("{0}_{1}{2}", model["SP_CODE"], count++, Path.GetExtension(file.FileName));
                            filePath = Path.Combine(imgPath, FileName);
                            fileSavePath = Path.Combine(imgSavePath, FileName);
                        }
                        file.SaveAs(filePath);
                    }
                }
                query = $@"
                        UPDATE DIST_VEHICLE_SETUP SET 
                            VEHICLE_CODE = '{vehicle_code}', 
                            VEHICLE_NO = '{vehicle_no}', 
                            VEHICLE_MODEL = '{vehicle_model}', 
                            MILAGE = '{milage}', 
                            FUEL_PRICE = '{fuel}', 
                            YEAR_MADE = '{year_made}',
                            RECOMMEND_FLAG = 'N',
                            RECOMMEND_DATE  = NULL,
                            RECOMMEND_BY  = NULL,
                            APPROVE_FLAG  = 'N',
                            APPROVE_DATE  = NULL,
                            APPROVE_BY = NULL
                            {(fileSavePath != null ? $", IMAGE = '{fileSavePath}'" : "")}
                        WHERE SP_CODE = '{sp_code}' AND ID = '{id}'
                    ";

                var imgRow = dbContext.ExecuteSqlCommand(query);
                if (imgRow == 0)
                {
                    throw new Exception("Something went wrong! Could not insert.");
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong! " + ex.Message);
            }

        }


        public bool NewMarketingInformation(InformationSaveModel model, NeoErpCoreEntity dbContext)
        {
            var today = $"TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy')";
            var MktCode = this.GetMaxId("DIST_MKT_INFO", "MKT_CODE", dbContext);
            if (model.entity_type.Equals("P", StringComparison.OrdinalIgnoreCase) || model.entity_type.Equals("DEALER", StringComparison.OrdinalIgnoreCase))
                model.entity_type = "P";
            else if (model.entity_type.Equals("D", StringComparison.OrdinalIgnoreCase) || model.entity_type.Equals("DISTRIBUTOR", StringComparison.OrdinalIgnoreCase))
                model.entity_type = "D";
            else if (model.entity_type.Equals("R", StringComparison.OrdinalIgnoreCase) || model.entity_type.Equals("RESELLER", StringComparison.OrdinalIgnoreCase))
                model.entity_type = "R";
            else
                throw new Exception("Invalid customer type");
            string InsertQuery = $@"INSERT INTO DIST_MKT_INFO (MKT_CODE, INFO_TEXT, ENTITY_TYPE, ENTITY_CODE, USER_ID, CREATE_DATE,COMPANY_CODE,BRANCH_CODE)
                VALUES ({MktCode}, '{model.information}', '{model.entity_type}', '{model.entity_code}', '{model.user_id}', {today},'{model.COMPANY_CODE}','{model.BRANCH_CODE}')";
            var row = dbContext.ExecuteSqlCommand(InsertQuery);
            if (row <= 0)
                throw new Exception("Unable to add marketing information.");

            return true;
        }

        public bool NewCompetitorInformation(InformationSaveModel model, NeoErpCoreEntity dbContext)
        {
            var today = $"TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy')";
            var ComptCode = this.GetMaxId("DIST_COMPT_INFO", "COMPT_CODE", dbContext);
            if (model.entity_type.Equals("P", StringComparison.OrdinalIgnoreCase) || model.entity_type.Equals("DEALER", StringComparison.OrdinalIgnoreCase))
                model.entity_type = "P";
            else if (model.entity_type.Equals("D", StringComparison.OrdinalIgnoreCase) || model.entity_type.Equals("DISTRIBUTOR", StringComparison.OrdinalIgnoreCase))
                model.entity_type = "D";
            else if (model.entity_type.Equals("R", StringComparison.OrdinalIgnoreCase) || model.entity_type.Equals("RESELLER", StringComparison.OrdinalIgnoreCase))
                model.entity_type = "R";
            else
                throw new Exception("Invalid customer type");
            string InsertQuery = $@"INSERT INTO DIST_COMPT_INFO (COMPT_CODE, INFO_TEXT, ENTITY_TYPE, ENTITY_CODE, USER_ID, CREATE_DATE,COMPANY_CODE,BRANCH_CODE)
                VALUES ({ComptCode}, '{model.information}', '{model.entity_type}', '{model.entity_code}', '{model.user_id}', {today},'{model.COMPANY_CODE}','{model.BRANCH_CODE}')";
            var row = dbContext.ExecuteSqlCommand(InsertQuery);
            if (row <= 0)
                throw new Exception("Unable to add competitor information.");

            return true;
        }

        public bool SaveQuestionaire(QuestionaireSaveModel model, NeoErpCoreEntity dbContext)
        {
            var today = $"TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy')";
            var ComptCode = this.GetMaxId("DIST_COMPT_INFO", "COMPT_CODE", dbContext);
            if (model.entity_type.Equals("P", StringComparison.OrdinalIgnoreCase) || model.entity_type.Equals("DEALER", StringComparison.OrdinalIgnoreCase))
                model.entity_type = "P";
            else if (model.entity_type.Equals("D", StringComparison.OrdinalIgnoreCase) || model.entity_type.Equals("DISTRIBUTOR", StringComparison.OrdinalIgnoreCase))
                model.entity_type = "D";
            else if (model.entity_type.Equals("R", StringComparison.OrdinalIgnoreCase) || model.entity_type.Equals("RESELLER", StringComparison.OrdinalIgnoreCase) || model.entity_type.Equals("outlet", StringComparison.OrdinalIgnoreCase))
                model.entity_type = "R";
            else
                throw new Exception("Invalid customer type");
            var AnsQuery = $"SELECT QA_CODE,ANSWER FROM DIST_QA_ANSWER WHERE CREATED_DATE ='{DateTime.Now.ToString("dd-MMM-yyyy")}' AND ENTITY_CODE='{model.entity_code}' AND ENTITY_TYPE='{model.entity_type}' AND DELETED_FLAG='N' AND COMPANY_CODE='{model.COMPANY_CODE}'";
            var answers = dbContext.SqlQuery<object>(AnsQuery).ToList();
            if (answers.Count > 0)
                throw new Exception("You Have Already Answers These Questionaire For The day!!");
            foreach (var general in model.general)
            {
                var InsertQuery = $@"INSERT INTO DIST_QA_ANSWER (SP_CODE, QA_CODE,ANSWER,ENTITY_TYPE,ENTITY_CODE,DELETED_FLAG,CREATED_DATE,CREATED_BY,COMPANY_CODE,BRANCH_CODE)
                    VALUES('{model.sp_code}','{general.qa_code}','{general.answer}','{model.entity_type}','{model.entity_code}','N',{today},'{model.entity_code}','{model.COMPANY_CODE}','{model.BRANCH_CODE}')";
                var row = dbContext.ExecuteSqlCommand(InsertQuery);
                if (row <= 0)
                    throw new Exception("Error Processing Request.");
            }
            foreach (var tab in model.tabular)
            {
                var AnsId = this.GetMaxId("DIST_QA_TAB_CELL_ANSWER", "ANSWER_ID", dbContext);
                if (tab.answer.Length > 30)
                {
                    Byte[] bytes = Convert.FromBase64String(tab.answer);
                    var name = $"qaFile_{AnsId}.jpg";
                    File.WriteAllBytes(UploadPath + @"\QAFiles\" + name, bytes);
                    tab.answer = name;
                }
                var InsertQuery = $@"INSERT INTO DIST_QA_TAB_CELL_ANSWER (ANSWER_ID,CELL_ID,ANSWER,ENTITY_CODE,ENTITY_TYPE,SP_CODE,CREATED_DATE)
                    VALUES('{AnsId}','{tab.cell_id}','{tab.answer}','{model.entity_code}','{model.entity_type}','{model.sp_code}',{today})";
                var row = dbContext.ExecuteSqlCommand(InsertQuery);
                if (row <= 0)
                    throw new Exception("Error Processing Request.");
            }
            return true;
        }

        public UpdateEntityResponsetModel UpdateDealerStock(UpdateEntityRequestModel model, NeoErpCoreEntity dbContext)
        {
            var result = new UpdateEntityResponsetModel();
            var CheckQuery = $"SELECT * FROM DIST_DEALER_STOCK WHERE COMPANY_CODE='{model.COMPANY_CODE}' AND DEALER_CODE='{model.customer_code}' AND trunc(CREATED_DATE)='{DateTime.Now.ToString("dd-MMM-yyyy")}'";
            var data = dbContext.SqlQuery<object>(CheckQuery).ToList();
            if (data.Count > 0)
            {
                result.msg = "You Have Already updated Stock For " + DateTime.Now.ToString("dd-MMM-yyyy");
            }
            else
            {
                var today = $"TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy')";
                foreach (var stock in model.stock)
                {
                    var InsertQuery = $@"INSERT INTO DIST_DEALER_STOCK(DEALER_CODE,ITEM_CODE,MU_CODE,CURRENT_STOCK,PURCHASE_QTY,SP_CODE,CREATED_DATE,DELETED_FLAG,COMPANY_CODE,BRANCH_CODE)
                        VALUES('{model.customer_code}','{stock.item_code}','{stock.mu_code}','{stock.cs}','{stock.p_qty}','{model.sp_code}',{today},'N','{model.COMPANY_CODE}','{model.BRANCH_CODE}')";
                    var row = dbContext.ExecuteSqlCommand(InsertQuery);
                    if (row <= 0)
                        throw new Exception("Error updating the stock");
                }
                result.msg = "Stock has been successfully updated For " + DateTime.Now.ToString("dd-MMM-yyyy");
            }
            var fetchModel = new EntityRequestModel
            {
                entity_code = model.customer_code,
                BRANCH_CODE = model.BRANCH_CODE,
                COMPANY_CODE = model.COMPANY_CODE,
                entity_type = "P"
            };
            var entityList = this.FetchEntityById(fetchModel, dbContext);
            result.entity = entityList.FirstOrDefault();
            return result;
        }

        public UpdateEntityResponsetModel UpdateDistributorStock(UpdateEntityRequestModel model, NeoErpCoreEntity dbContext)
        {
            var result = new UpdateEntityResponsetModel();
            var CheckQuery = $"SELECT * FROM DIST_DISTRIBUTOR_STOCK WHERE COMPANY_CODE='{model.COMPANY_CODE}' AND DISTRIBUTOR_CODE='{model.customer_code}' AND trunc(CREATED_DATE)='{DateTime.Now.ToString("dd-MMM-yyyy")}'";
            var data = dbContext.SqlQuery<object>(CheckQuery).ToList();
            if (data.Count > 0)
            {
                result.msg = "You Have Already updated Stock For " + DateTime.Now.ToString("dd-MMM-yyyy");
            }
            else
            {
                var today = $"TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy')";
                var StockId = dbContext.SqlQuery<int>("SELECT (NVL(MAX(STOCK_ID),0)+1) MAXID FROM DIST_DISTRIBUTOR_STOCK").FirstOrDefault();
                foreach (var stock in model.stock)
                {
                    var InsertQuery = $@"INSERT INTO DIST_DISTRIBUTOR_STOCK(STOCK_ID,DISTRIBUTOR_CODE,ITEM_CODE,MU_CODE,CURRENT_STOCK,PURCHASE_QTY,SP_CODE,CREATED_DATE,DELETED_FLAG,COMPANY_CODE,BRANCH_CODE)
                        VALUES('{StockId}','{model.customer_code}','{stock.item_code}','{stock.mu_code}','{stock.cs}','{stock.p_qty}','{model.sp_code}',{today},'N','{model.COMPANY_CODE}','{model.BRANCH_CODE}')";
                    var row = dbContext.ExecuteSqlCommand(InsertQuery);
                    if (row <= 0)
                        throw new Exception("Error updating the stock");
                }
                result.msg = "Stock has been successfully updated For " + DateTime.Now.ToString("dd-MMM-yyyy");
            }
            var fetchModel = new EntityRequestModel
            {
                entity_code = model.customer_code,
                BRANCH_CODE = model.BRANCH_CODE,
                COMPANY_CODE = model.COMPANY_CODE,
                entity_type = "D"
            };
            var entityList = this.FetchEntityById(fetchModel, dbContext);
            result.entity = entityList.FirstOrDefault();
            return result;
        }

        public UpdateEntityResponsetModel UpdateResellerStock(UpdateEntityRequestModel model, NeoErpCoreEntity dbContext)
        {

            var result = new UpdateEntityResponsetModel();
            var CheckQuery = $"SELECT * FROM DIST_RESELLER_STOCK WHERE COMPANY_CODE='{model.COMPANY_CODE}' AND RESELLER_CODE='{model.customer_code}' AND trunc(CREATED_DATE)='{DateTime.Now.ToString("dd-MMM-yyyy")}'";
            var data = dbContext.SqlQuery<object>(CheckQuery).ToList();
            if (data.Count > 0)
            {
                result.msg = "You Have Already updated Stock For " + DateTime.Now.ToString("dd-MMM-yyyy");
            }
            else
            {
                var today = $"TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy')";
                foreach (var stock in model.stock)
                {
                    var InsertQuery = $@"INSERT INTO DIST_RESELLER_STOCK(RESELLER_CODE,ITEM_CODE,MU_CODE,CURRENT_STOCK,PURCHASE_QTY,SP_CODE,CREATED_DATE,DELETED_FLAG,COMPANY_CODE,BRANCH_CODE)
                        VALUES('{model.customer_code}','{stock.item_code}','{stock.mu_code}','{stock.cs}','{stock.p_qty}','{model.sp_code}',{today},'N','{model.COMPANY_CODE}','{model.BRANCH_CODE}')";
                    var row = dbContext.ExecuteSqlCommand(InsertQuery);
                    if (row <= 0)
                        throw new Exception("Error updating the stock");
                }
                result.msg = "Stock has been successfully updated For " + DateTime.Now.ToString("dd-MMM-yyyy");
            }
            var fetchModel = new EntityRequestModel
            {
                entity_code = model.customer_code,
                BRANCH_CODE = model.BRANCH_CODE,
                COMPANY_CODE = model.COMPANY_CODE,
                entity_type = "R"
            };
            var entityList = this.FetchEntityById(fetchModel, dbContext);
            result.entity = entityList.FirstOrDefault();
            return result;
        }

        public EntityResponseModel CreateReseller(CreateResellerModel model, HttpFileCollection Files, Dictionary<string, string> descriptions, NeoErpCoreEntity dbContext)
        {
            //primary contact
            var primary = new ContactModel();
            foreach (var c in model.contact)
                if (c.primary.Equals("Y", StringComparison.OrdinalIgnoreCase))
                    primary = c;
            if (primary != null)
                model.contact.Remove(primary);

            if (string.IsNullOrWhiteSpace(model.address))
                throw new Exception("Address is empty.");
            if (string.IsNullOrWhiteSpace(model.latitude))
                throw new Exception("Latitude is empty.");
            if (string.IsNullOrWhiteSpace(model.longitude))
                throw new Exception("Longitude is empty.");
            if (string.IsNullOrWhiteSpace(model.area_code))
                throw new Exception("Area code not selected.");
            string testQuery = $"SELECT * FROM DIST_RESELLER_MASTER WHERE RESELLER_NAME = '{model.reseller_name}' AND PAN_NO = '{model.pan}' AND COMPANY_CODE='{model.COMPANY_CODE}'";
            var testObj = dbContext.SqlQuery<object>(testQuery).ToList();
            if (testObj.Count > 0)
                throw new Exception("Reseller with the provided name and PAN no. already exists.");
            //Generate reseller code
            string RCodeQuery = $"SELECT 'R-{model.user_id.Trim()}-'||TO_CHAR(SYSDATE,'YYMMDD-HH24MMSS') FROM DUAL";
            string ResellerCode = dbContext.SqlQuery<string>(RCodeQuery).FirstOrDefault();

            //insert reseller
            string ResellerInsert = $@"INSERT INTO DIST_RESELLER_MASTER
                        (RESELLER_CODE,RESELLER_NAME,REG_OFFICE_ADDRESS,EMAIL,PAN_NO,LATITUDE,LONGITUDE,WHOLESELLER,AREA_CODE,CONTACT_SUFFIX,CONTACT_NAME,CONTACT_NO,OUTLET_TYPE_ID,OUTLET_SUBTYPE_ID,GROUPID,CREATED_BY,CREATED_DATE,COMPANY_CODE,BRANCH_CODE,RESELLER_CONTACT,SOURCE,ACTIVE,TEMP_ROUTE_CODE) VALUES 
                        ('{ResellerCode}','{model.reseller_name}','{model.address}','{model.email}','{model.pan}','{model.latitude}','{model.longitude}','{model.wholeseller}','{model.area_code}','{primary.contact_suffix}','{primary.name}',
                        '{primary.number}','{model.type_id}','{model.subtype_id}','{model.Group_id}','{model.user_id}',TO_DATE(SYSDATE),'{model.COMPANY_CODE}','{model.BRANCH_CODE}','{model.Reseller_contact}','MOB','N','{model.ROUTE_CODE}')";
            var row = dbContext.ExecuteSqlCommand(ResellerInsert);

            //insert contact details
            foreach (var con in model.contact)
            {
                string ContactQuery = $@"INSERT INTO DIST_RESELLER_DETAIL(RESELLER_CODE,COMPANY_CODE,CONTACT_SUFFIX,CONTACT_NAME,CONTACT_NO,DESIGNATION,CREATED_BY,CREATED_DATE) VALUES
                            ('{ResellerCode}','{model.COMPANY_CODE}','{con.contact_suffix}','{con.name}','{con.number}','{con.designation}','{model.user_id}',TO_DATE(SYSDATE))";
                row = dbContext.ExecuteSqlCommand(ContactQuery);
            }

            List<string> dist = new List<string>();
            List<string> Who = new List<string>();
            if (!string.IsNullOrWhiteSpace(model.distributor_code))
                dist = model.distributor_code.Replace(" ", string.Empty).Split(',').ToList();
            if (!string.IsNullOrWhiteSpace(model.wholeseller_code))
                Who = model.wholeseller_code.Replace(" ", string.Empty).Split(',').ToList();

            foreach (var distributor in dist)
            {
                string disInsertQuery = $@"INSERT INTO DIST_RESELLER_ENTITY (RESELLER_CODE,ENTITY_CODE,ENTITY_TYPE,CREATED_BY,CREATED_DATE,DELETED_FLAG,COMPANY_CODE,BRANCH_CODE)
                                                VALUES('{ResellerCode}','{distributor}','D','{model.user_id}',TO_DATE(SYSDATE),'N','{model.COMPANY_CODE}','{model.BRANCH_CODE}')";
                row = dbContext.ExecuteSqlCommand(disInsertQuery);
            }
            foreach (var wholeseller in Who)
            {
                string whoInsertQuery = $@"INSERT INTO DIST_RESELLER_ENTITY (RESELLER_CODE,ENTITY_CODE,ENTITY_TYPE,CREATED_BY,CREATED_DATE,DELETED_FLAG,COMPANY_CODE,BRANCH_CODE)
                                                VALUES('{ResellerCode}','{wholeseller}','W','{model.user_id}',TO_DATE(SYSDATE),'N','{model.COMPANY_CODE}','{model.BRANCH_CODE}')";
                row = dbContext.ExecuteSqlCommand(whoInsertQuery);
            }

            //upload files
            foreach (string tagName in Files)
            {
                HttpPostedFile file = Files[tagName];
                string ResellerPath = string.Empty;

                ResellerPath = UploadPath + "\\ResellerImages";

                if (!Directory.Exists(ResellerPath))
                    Directory.CreateDirectory(ResellerPath);
                string FileName = string.Format("{0}{1}", ResellerCode, Path.GetExtension(file.FileName));
                string filePath = Path.Combine(ResellerPath, FileName);
                int count = 1;
                while (File.Exists(filePath))
                {
                    FileName = string.Format("{0}_{1}{2}", ResellerCode, count++, Path.GetExtension(file.FileName));
                    filePath = Path.Combine(ResellerPath, FileName);
                }
                string mediaType;
                if (tagName.IndexOf("store", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    mediaType = "STORE";
                    file.SaveAs(filePath);
                }
                else if (tagName.IndexOf("pcontact", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    mediaType = "PCONTACT";
                    file.SaveAs(filePath);
                }
                else
                    continue;
                string ImageQuery = $@"INSERT INTO DIST_PHOTO_INFO (FILENAME,DESCRIPTION,ENTITY_TYPE,ENTITY_CODE,MEDIA_TYPE,CREATED_BY,CREATE_DATE,COMPANY_CODE,BRANCH_CODE) VALUES
                            ('{FileName}','{descriptions[tagName]}','R','{ResellerCode}','{mediaType}','{model.user_id}',SYSDATE,'{model.COMPANY_CODE}','{model.BRANCH_CODE}')";
                row = dbContext.ExecuteSqlCommand(ImageQuery);
            }

            var fetchModel = new EntityRequestModel
            {
                entity_code = ResellerCode,
                BRANCH_CODE = model.BRANCH_CODE,
                COMPANY_CODE = model.COMPANY_CODE,
                entity_type = "R"
            };
            var entityList = this.FetchEntityById(fetchModel, dbContext);
            var result = entityList.FirstOrDefault();
            return result;
        }
        public EntityResponseModel CreateDistributor(CreateDistributorModel model, HttpFileCollection Files, Dictionary<string, string> descriptions, NeoErpCoreEntity dbContext)
        {
            //primary contact
            var primary = new ContactModel();
            foreach (var c in model.contact)
                if (c.primary.Equals("Y", StringComparison.OrdinalIgnoreCase))
                    primary = c;
            if (primary != null)
                model.contact.Remove(primary);

            if (string.IsNullOrWhiteSpace(model.address))
                throw new Exception("Address is empty.");
            if (string.IsNullOrWhiteSpace(model.latitude))
                throw new Exception("Latitude is empty.");
            if (string.IsNullOrWhiteSpace(model.longitude))
                throw new Exception("Longitude is empty.");
            if (string.IsNullOrWhiteSpace(model.area_code))
                throw new Exception("Area code not selected.");
            string testQuery = $"SELECT * FROM sa_customer_setup WHERE CUSTOMER_EDESC = '{model.distributor_name}' AND PAN_NO = '{model.pan}' AND COMPANY_CODE='{model.COMPANY_CODE}'";
            var testObj = dbContext.SqlQuery<object>(testQuery).ToList();
            if (testObj.Count > 0)
                throw new Exception("Distributor with the provided name and PAN no. already exists.");
            //Generate distributor code
            //sashi
            var newmaxitemcode = string.Empty;
            var newmaxitemcodequery = $@"SELECT MAX(TO_NUMBER(CUSTOMER_CODE))+1 as MASTER_CUSTOMER_CODE FROM SA_CUSTOMER_SETUP";
            newmaxitemcode = dbContext.SqlQuery<int>(newmaxitemcodequery).FirstOrDefault().ToString();

            using (var transaction = _objectEntity.Database.BeginTransaction())
            {
                try
                {

                    if (newmaxitemcodequery != null)
                    {

                        string CustomerQuery = $@"INSERT INTO sa_customer_setup(CUSTOMER_CODE,CUSTOMER_EDESC,CUSTOMER_NDESC,REGD_OFFICE_EADDRESS,REGD_OFFICE_NADDRESS,TEL_MOBILE_NO1,TEL_MOBILE_NO2,FAX_NO,EMAIL,PARTY_TYPE_CODE,CUSTOMER_FLAG,LINK_SUB_CODE,CREDIT_RATE,CREDIT_LIMIT
                                            ,CUSHION_PERCENT,DUE_BILL_COUNT,ACTIVE_FLAG,REMARKS,GROUP_SKU_FLAG,MASTER_CUSTOMER_CODE,PRE_CUSTOMER_CODE,DISCOUNT_FLAT_RATE,EXCLUSIVE_FLAG,DISCOUNT_DAYS,DISCOUNT_PERCENT,COMPANY_CODE,CREATED_BY
                                            ,CREATED_DATE,DELETED_FLAG,OPENING_DATE,MATURITY_DATE,CUSTOMER_GROUP_ID,COUNTRY_CODE,ZONE_CODE,DISTRICT_CODE,CITY_CODE,DEALING_PERSON,EXCISE_NO,TIN,EXPORT_FLAG,GST_NO,IEC_NO,FSSAI_NO,AD_CODE) VALUES
                                            ('{newmaxitemcode}','{model.distributor_name}','{model.distributor_name}','{model.address.Replace("'", "''")}','{model.address.Replace("'", "''")}','{primary.contact_suffix}','{primary.contact_suffix}','{null}','{model.email}','{null}','D','{'C' + newmaxitemcode}','{null}','{null}'
                                            ,'{null}','{null}','Y','{null}','G','{model.BRANCH_CODE}','{model.COMPANY_CODE}','{null}','{null}','{null}','{null}','{model.COMPANY_CODE}','ADMIN'
                                            ,TO_DATE('{model.Saved_Date}','MM/dd/yyyy hh24:mi:ss'),'N',TO_DATE('{model.Saved_Date}','MM/dd/yyyy hh24:mi:ss')
                                            ,TO_DATE('{model.Saved_Date}','MM/dd/yyyy hh24:mi:ss'),'{null}','{null}','{null}','{null}','{null}','{null}','{null}','{null}','N','{null}','{null}','{null}','{null}')";
                        var row = dbContext.ExecuteSqlCommand(CustomerQuery);

                        string DistributorQuery = $@"INSERT INTO DIST_DISTRIBUTOR_MASTER(DISTRIBUTOR_CODE,LATITUDE,LONGITUDE,CREATED_BY,CREATED_DATE,ACTIVE,COMPANY_CODE,BRANCH_CODE,AREA_CODE,GROUPID,WEIGHT,DELETED_FLAG,DISTRIBUTOR_TYPE_ID,DISTRIBUTOR_SUBTYPE_ID) VALUES
                            ('{newmaxitemcode}','{model.latitude}','{model.longitude}','{model.user_id}',TO_DATE('{model.Saved_Date}', 'MM/dd/yyyy hh24:mi:ss'),'Y','{model.COMPANY_CODE}','{model.BRANCH_CODE}','{model.area_code}','{model.Group_id}','{0}','{'N'}','{model.DISTRIBUTOR_TYPE_ID}','{model.DISTRIBUTOR_SUBTYPE_ID}')";
                        row = dbContext.ExecuteSqlCommand(DistributorQuery);

                        //insert contact details
                        foreach (var con in model.contact)
                        {
                            string ContactQuery = $@"INSERT INTO DIST_DISTRIBUTOR_DETAIL(DISTRIBUTOR_CODE,COMPANY_CODE,CONTACT_SUFFIX,CONTACT_NAME,CONTACT_NO,DESIGNATION,CREATED_BY,CREATED_DATE) VALUES
                            ('{newmaxitemcode}','{model.COMPANY_CODE}','{con.contact_suffix}','{con.name}','{con.number}','{con.designation}','{model.user_id}',TO_DATE(SYSDATE))";
                            row = dbContext.ExecuteSqlCommand(ContactQuery);
                        }

                        //upload files
                        foreach (string tagName in Files)
                        {
                            HttpPostedFile file = Files[tagName];
                            string DistributorPath = string.Empty;

                            DistributorPath = UploadPath + "\\DistributorImages";

                            if (!Directory.Exists(DistributorPath))
                                Directory.CreateDirectory(DistributorPath);
                            string FileName = string.Format("{0}{1}", newmaxitemcode, Path.GetExtension(file.FileName));
                            string filePath = Path.Combine(DistributorPath, FileName);
                            int count = 1;
                            while (File.Exists(filePath))
                            {
                                FileName = string.Format("{0}_{1}{2}", newmaxitemcode, count++, Path.GetExtension(file.FileName));
                                filePath = Path.Combine(DistributorPath, FileName);
                            }
                            string mediaType;
                            if (tagName.IndexOf("store", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                mediaType = "STORE";
                                file.SaveAs(filePath);
                            }
                            else if (tagName.IndexOf("pcontact", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                mediaType = "PCONTACT";
                                file.SaveAs(filePath);
                            }
                            else
                                continue;
                            string ImageQuery = $@"INSERT INTO DIST_PHOTO_INFO (FILENAME,DESCRIPTION,ENTITY_TYPE,ENTITY_CODE,MEDIA_TYPE,CREATED_BY,CREATE_DATE,COMPANY_CODE,BRANCH_CODE) VALUES
                            ('{FileName}','{descriptions[tagName]}','D','{newmaxitemcode}','{mediaType}','{model.user_id}',SYSDATE,'{model.COMPANY_CODE}','{model.BRANCH_CODE}')";
                            row = dbContext.ExecuteSqlCommand(ImageQuery);
                        }
                        _objectEntity.SaveChanges();
                        transaction.Commit();
                    }
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                }
            }
            //sashi
            var fetchModel = new EntityRequestModel
            {
                entity_code = newmaxitemcode,
                BRANCH_CODE = model.BRANCH_CODE,
                COMPANY_CODE = model.COMPANY_CODE,
                entity_type = "D"
            };
            var entityList = this.FetchEntityById(fetchModel, dbContext);
            var result = entityList.FirstOrDefault();
            return result;
        }

        public string UpdateReseller(CreateResellerModel model, NeoErpCoreEntity dbContext)
        {

            if (string.IsNullOrWhiteSpace(model.reseller_code))
                throw new Exception("Reseller code is empty.");
            //if (string.IsNullOrWhiteSpace(model.address))
            //    throw new Exception("Address is empty.");o
            //if (model.contact.Count == 0)
            //    model.contact.Add(new ContactModel());

            //update reseller
            //string ResellerInsert = $@"UPDATE DIST_RESELLER_MASTER
            //            SET RESELLER_NAME='{model.reseller_name}',REG_OFFICE_ADDRESS='{model.address}',EMAIL='{model.email}',CONTACT_SUFFIX='{model.contact[0].contact_suffix}',
            //            CONTACT_NAME='{model.contact[0].name}',CONTACT_NO='{model.contact[0].number}',OUTLET_TYPE_ID='{model.type_id}',OUTLET_SUBTYPE_ID='{model.subtype_id}',
            //            RESELLER_CONTACT='{model.Reseller_contact}',LUPDATE_BY='{model.user_id}',LUPDATE_DATE=SYSDATE
            //            WHERE RESELLER_CODE='{model.reseller_code}' AND COMPANY_CODE='{model.COMPANY_CODE}'";

            string ResellerInsert = $@"UPDATE DIST_RESELLER_MASTER
                        SET REG_OFFICE_ADDRESS='{model.address}',EMAIL='{model.email}',CONTACT_NO='{model.Reseller_contact}',
                        LUPDATE_BY='{model.user_id}',LUPDATE_DATE=SYSDATE
                        WHERE RESELLER_CODE='{model.reseller_code}' AND COMPANY_CODE='{model.COMPANY_CODE}'";

            var row = dbContext.ExecuteSqlCommand(ResellerInsert);
            return "Reseller Updated Successfully";
        }

        public Dictionary<string, string> UploadEntityMedia(EntityRequestModel model, HttpFileCollection files, Dictionary<string, ImageSaveModel> descriptions, NeoErpCoreEntity dbContext)
        {
            model.Sync_Id = model.Sync_Id == null ? "" : model.Sync_Id;
            var result = new Dictionary<string, string>();
            if (model.entity_type.Equals("P", StringComparison.OrdinalIgnoreCase) || model.entity_type.Equals("DEALER", StringComparison.OrdinalIgnoreCase))
                model.entity_type = "P";
            else if (model.entity_type.Equals("D", StringComparison.OrdinalIgnoreCase) || model.entity_type.Equals("DISTRIBUTOR", StringComparison.OrdinalIgnoreCase) || model.entity_type.Equals("PartyTypes.distributor", StringComparison.OrdinalIgnoreCase))
                model.entity_type = "D";
            else if (model.entity_type.Equals("R", StringComparison.OrdinalIgnoreCase) || model.entity_type.Equals("RESELLER", StringComparison.OrdinalIgnoreCase) || model.entity_type.Equals("outlet", StringComparison.OrdinalIgnoreCase) || model.entity_type.Equals("PartyTypes.outlet", StringComparison.OrdinalIgnoreCase))
                model.entity_type = "R";
            else if (model.entity_type.Equals("B", StringComparison.OrdinalIgnoreCase) || model.entity_type.Equals("BRANDING", StringComparison.OrdinalIgnoreCase))
                model.entity_type = "B";
            else if (model.entity_type.Equals("F", StringComparison.OrdinalIgnoreCase) || model.entity_type.Equals("FARMER", StringComparison.OrdinalIgnoreCase))
                model.entity_type = "F";
            else
                throw new Exception("Invalid customer type");

            int row = 0;
            var folderpath = UploadPath + "\\EntityImages";
            foreach (string tagName in files)
            {
                HttpPostedFile file = files[tagName];
                var ImageId = this.GetMaxId("DIST_VISIT_IMAGE", "IMAGE_CODE", dbContext);
                if (!Directory.Exists(folderpath))
                    Directory.CreateDirectory(folderpath);
                string FileName = string.Format("{0}{1}{2}", "EntityImage", ImageId, Path.GetExtension(file.FileName));
                string filePath = Path.Combine(folderpath, FileName);
                int count = 1;
                while (File.Exists(filePath))
                {
                    FileName = string.Format("{0}{1}_{2}{3}", "EntityImage", ImageId, count++, Path.GetExtension(file.FileName));
                    filePath = Path.Combine(folderpath, FileName);
                }
                file.SaveAs(filePath);

                //model.ACC_CODE is actually sp_code
                var InsertQuery = $@"INSERT INTO DIST_VISIT_IMAGE (IMAGE_CODE,IMAGE_NAME,IMAGE_TITLE,IMAGE_DESC,SP_CODE,ENTITY_CODE,TYPE,UPLOAD_DATE,LONGITUDE,LATITUDE,CATEGORYID,COMPANY_CODE,BRANCH_CODE,SYNC_ID)
                                    VALUES ({ImageId}, '{FileName}', '{DBNull.Value}', '{descriptions[tagName].Description}', '{model.ACC_CODE}', '{model.entity_code}', '{model.entity_type}', SYSDATE,'{model.longitude}', '{model.latitude}','{descriptions[tagName].CategoryId}','{model.COMPANY_CODE}', '{model.BRANCH_CODE}','{model.Sync_Id}')";
                row += dbContext.ExecuteSqlCommand(InsertQuery);
            }
            result.Add("msg", "Image Successfully Uploaded");
            return result;
        }

        public string AddFarmer(FarmerModel model, HttpFileCollection Files, NeoErpCoreEntity dbContext)
        {
            var query = "";
            string FileName = "";
            try
            {
                var UserFolderpath = UploadPath + "\\FarmerImages";

                if (!Directory.Exists(UserFolderpath))
                    Directory.CreateDirectory(UserFolderpath);

                if (Files.AllKeys.Contains("PROFILE_IMAGE"))
                {
                    HttpPostedFile file = Files["PROFILE_IMAGE"];

                    FileName = string.Format("FARMER_{0}_{1}{2}", model.FARMER_EDESC.Split(' ')[0], DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss-fff"), Path.GetExtension(file.FileName));
                    var filePath = Path.Combine(UserFolderpath, FileName);
                    int count = 1;
                    while (File.Exists(filePath))
                    {
                        FileName = string.Format("FARMER_{0}_{1}_{2}{3}", model.FARMER_EDESC.Split(' ')[0], DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss-fff"), count++, Path.GetExtension(file.FileName));
                        filePath = Path.Combine(UserFolderpath, FileName);
                    }

                    file.SaveAs(filePath);
                }



                var farmerCode = dbContext.Database.SqlQuery<string>("select to_char(farmer_id_seq.nextval) from dual").FirstOrDefault();

                query = $@"INSERT INTO dist_farmer_master 
                (FARMER_ID, FARMER_EDESC, FARM_EDESC, ADDRESS, CONTACT_NO, AREA_CODE, 
                 FARM_LONGITUDE, FARM_LATITUDE, FARM_AREA, FARMING_CROPS, EXPERIENCE, REMARKS, PROFILE_IMG) 
                VALUES 
                ('{farmerCode}', '{model.FARMER_EDESC}', '{model.FARM_EDESC}', 
                 '{model.ADDRESS}', '{model.CONTACT_NO}', '{model.AREA_CODE}', 
                 '{model.FARM_LONGITUDE}','{model.FARM_LATITUDE}', '{model.FARM_AREA}', '{model.FARM_CROPS}', 
                 '{model.EXPERIENCE}', '{model.REMARKS}', '{FileName}')";

                var rowAffected = dbContext.ExecuteSqlCommand(query);

                var dealerRowNum = 0;
                var subDealerRowNum = 0;

                if (model.DEALERS.Count() == 0)
                {
                    dealerRowNum = 1;
                }

                foreach (var dealer in model.DEALERS)
                {
                    var dealerQuery = $@"INSERT INTO dist_farmer_dealers 
        (FARMER_ID, DEALER_EDESC, DEALER_CODE) 
        VALUES 
        ('{farmerCode}', '{dealer.DEALER_NAME}', '{dealer.DEALER_CODE}')";

                    dealerRowNum = dbContext.ExecuteSqlCommand(dealerQuery);
                }

                if (model.SUB_DEALERS.Count() == 0)
                {
                    subDealerRowNum = 1;
                }
                foreach (var subDealer in model.SUB_DEALERS)
                {
                    var subDealerQuery = $@"INSERT INTO dist_farmer_sub_dealers 
        (FARMER_ID, SUB_DEALER_EDESC, SUB_DEALER_CODE) 
        VALUES 
        ('{farmerCode}', '{subDealer.DEALER_NAME}', '{subDealer.DEALER_CODE}')";

                    subDealerRowNum = dbContext.ExecuteSqlCommand(subDealerQuery);
                }



                if (rowAffected > 0 && dealerRowNum > 0 && subDealerRowNum > 0)
                {
                    return "Farmer added successfully!";
                }
                else
                {
                    throw new Exception("An error occurred while processing farmer data.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string DeliveryApprove(DeliveryApproveModel model, HttpFileCollection Files, NeoErpCoreEntity dbContext)
        {

            try
            {
                string filePath = "";
                string UserFolderpath = string.Empty;
                string sigPath;
                List<string> ImagesNames = new List<string>();

                UserFolderpath = UploadPath + "\\DriverImages";

                if (!Directory.Exists(UserFolderpath))
                    Directory.CreateDirectory(UserFolderpath);

                HttpPostedFile sig = Files["SIGNATURE"];


                string sigName = string.Format("sig_{0}_{1}{2}", model.DRIVER_MOBILE_NO, DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss-fff"), Path.GetExtension(sig.FileName));
                sigPath = Path.Combine(UserFolderpath, sigName);
                int sigcount = 1;
                while (File.Exists(filePath))
                {
                    sigName = string.Format("sig_{0}_{1}_{2}{3}", model.DRIVER_MOBILE_NO, DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss-fff"), sigcount++, Path.GetExtension(sig.FileName));
                    sigPath = Path.Combine(UserFolderpath, sigName);
                }

                sig.SaveAs(sigPath);

                foreach (string tagName in Files)
                {
                    if (tagName != "SIGNATURE")
                    {
                        HttpPostedFile file = Files[tagName];


                        string FileName = string.Format("DRI_{0}_{1}{2}", model.DRIVER_MOBILE_NO, DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss-fff"), Path.GetExtension(file.FileName));
                        filePath = Path.Combine(UserFolderpath, FileName);
                        int count = 1;
                        while (File.Exists(filePath))
                        {
                            FileName = string.Format("DRI_{0}_{1}_{2}{3}", model.DRIVER_MOBILE_NO, DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss-fff"), count++, Path.GetExtension(file.FileName));
                            filePath = Path.Combine(UserFolderpath, FileName);
                        }

                        file.SaveAs(filePath);
                        ImagesNames.Add(FileName);
                    }

                }
                if(model.TYPE == "TRANSPORTER")
                {
                    List<DeliveryApproveModel> chalanList = dbContext.SqlQuery<DeliveryApproveModel>($@"
                        select distinct voucher_no CHALAN_NO, reference_no from CHALAN_DELIVERY_STATUS_UPDATE 
                        where transporter_code = '{model.TRANSPORTER_CODE}' and bilty_number = '{model.BILTY_NUMBER}' and company_code = '{model.COMPANY_CODE}'
                        ").ToList();
                    foreach(var chalan in chalanList)
                    {
                        var insertQ = $@"
        UPDATE chalan_delivery_status_update
        SET STATUS = 'AP',
            CREATED_DATE = SYSDATE,
            REMARKS = '{model.REMARKS}',
            LATITUDE = '{model.LATITUDE}',
            LONGITUDE = '{model.LONGTITUDE}',
            SIGNATURE = '{sigName}',
            IMG0 = {(ImagesNames.Count > 0 ? $"'{ImagesNames[0]}'" : "NULL")},
            IMG1 = {(ImagesNames.Count > 1 ? $"'{ImagesNames[1]}'" : "NULL")},
            IMG2 = {(ImagesNames.Count > 2 ? $"'{ImagesNames[2]}'" : "NULL")},
            IMG3 = {(ImagesNames.Count > 3 ? $"'{ImagesNames[3]}'" : "NULL")},
            RECEIVER_NAME = '{model.RECEIVER_NAME}',
            RECEIVER_NUMBER = '{model.RECEIVER_NUMBER}'
        WHERE VOUCHER_NO = '{chalan.CHALAN_NO}'
        AND REFERENCE_NO = '{chalan.REFERENCE_NO}'
        AND COMPANY_CODE = '{model.COMPANY_CODE}'";
                        int row = dbContext.Database.ExecuteSqlCommand(insertQ);
                        if (row == 0) throw new Exception("Something went wrong");
                    }
                    //var updateQ = $@"
                    //        UPDATE ip_vehicle_track SET status = 'AP' , SIGNATURE = '{sigName}',
                    //           img0= {(ImagesNames.Count > 0 ? $"'{ImagesNames[0]}'" : "NULL")},
                    //            img1={(ImagesNames.Count > 1 ? $"'{ImagesNames[1]}'" : "NULL")},
                    //            img2={(ImagesNames.Count > 2 ? $"'{ImagesNames[2]}'" : "NULL")},
                    //            img3={(ImagesNames.Count > 3 ? $"'{ImagesNames[3]}'" : "NULL")},
                    //            RECEIVER_NAME = '{model.RECEIVER_NAME}',
                    //            RECEIVER_NUMBER = '{model.RECEIVER_NUMBER}',
                    //            DELIVERED_DATE = sysdate
                    //        where transporter_code = '{model.TRANSPORTER_CODE}' 
                    //        and bilty_number = '{model.BILTY_NUMBER}' 
                    //        and company_code = '{model.COMPANY_CODE}'";
                    //dbContext.ExecuteSqlCommand(updateQ);
                    return "Delivery approved successfully!";
                }

                var insertQuery = $@"
INSERT INTO chalan_delivery_status_update (
    VOUCHER_NO,
REFERENCE_NO,
    STATUS,
    COMPANY_CODE,
    CREATED_DATE,
REMARKS,
    DELETED_FLAG,
    LATITUDE,
    LONGITUDE,
    SIGNATURE,
    IMG0,
    IMG1,
    IMG2,
    IMG3,
    RECEIVER_NAME,
    RECEIVER_NUMBER
) VALUES (
    '{model.CHALAN_NO}',
'{model.REFERENCE_NO}',
    'AP',
    '{model.COMPANY_CODE}',
    SYSDATE,
'{model.REMARKS}',
    'N',
    '{model.LATITUDE}',
    '{model.LONGTITUDE}',
    '{sigName}',
    {(ImagesNames.Count > 0 ? $"'{ImagesNames[0]}'" : "NULL")},
    {(ImagesNames.Count > 1 ? $"'{ImagesNames[1]}'" : "NULL")},
    {(ImagesNames.Count > 2 ? $"'{ImagesNames[2]}'" : "NULL")},
    {(ImagesNames.Count > 3 ? $"'{ImagesNames[3]}'" : "NULL")},
    '{model.RECEIVER_NAME}',
    '{model.RECEIVER_NUMBER}'
)";

                int rownum = dbContext.Database.ExecuteSqlCommand(insertQuery);

                var updateQuery = $@"UPDATE ip_vehicle_track SET status = 'AP' WHERE driver_mobile_no = '{model.DRIVER_MOBILE_NO}' and transaction_no = '{model.TRANSACTION_NO}' and reference_no = '{model.REFERENCE_NO}'";

                string checkQuery = $@"SELECT 
    CASE 
        WHEN (SELECT COUNT(DISTINCT voucher_no) FROM reference_Detail WHERE reference_no =  '{model.REFERENCE_NO}') = 
             (SELECT COUNT(voucher_no) FROM chalan_delivery_status_update WHERE reference_no =  '{model.REFERENCE_NO}')
        THEN 'TRUE'
        ELSE 'FALSE'
    END AS result
FROM dual";

                string allApproved = dbContext.SqlQuery<string>(checkQuery).FirstOrDefault();

                if (allApproved == "TRUE")
                {
                    dbContext.Database.ExecuteSqlCommand(updateQuery);
                }

                if (rownum >= 1)
                {
                    return "Delivery approved successfully!";
                }
                else
                {
                    throw new Exception("Something went wrong!");
                }
            }
            catch
            {
                throw new Exception("Something went wrong!");
            }
        }
        public Dictionary<string, string> UploadAttendencePic(EntityRequestModel model, UpdateRequestModel locationModel, HttpFileCollection Files, Dictionary<string, string> descriptions, NeoErpCoreEntity dbContext)
        {
            var result = new Dictionary<string, string>();
            var today = DateTime.Now.ToString("MM/dd/yyyyHH:mm:ss");
            var todayAttendence = dbContext.SqlQuery<object>($"SELECT * FROM HRIS_ATTENDANCE WHERE EMPLOYEE_ID='{model.entity_code}' AND TO_DATE(ATTENDANCE_DT)=TO_DATE(SYSDATE)").ToList();
            //if (todayAttendence.Count <= 0)
            // {
            foreach (string tagName in Files)
            {
                HttpPostedFile file = Files[tagName];
                string UserFolderpath = string.Empty;

                UserFolderpath = UploadPath + "\\AttnImages";  //model.entity_code is the sp_code(sales person code)

                if (!Directory.Exists(UserFolderpath))
                    Directory.CreateDirectory(UserFolderpath);
                string FileName = string.Format("ATTN_{0}_{1}{2}", model.entity_code, DateTime.Now.ToString("dd-MM-yyyy"), Path.GetExtension(file.FileName));
                string filePath = Path.Combine(UserFolderpath, FileName);
                int count = 1;
                while (File.Exists(filePath))
                {
                    FileName = string.Format("ATTN_{0}_{1}_{2}{3}", model.entity_code, DateTime.Now.ToString("dd-MM-yyyy"), count++, Path.GetExtension(file.FileName));
                    filePath = Path.Combine(UserFolderpath, FileName);
                }

                file.SaveAs(filePath);

                string ImageQuery = $@"INSERT INTO DIST_PHOTO_INFO (FILENAME,DESCRIPTION,ENTITY_TYPE,ENTITY_CODE,MEDIA_TYPE,CATEGORYID,CREATED_BY,CREATE_DATE,COMPANY_CODE,BRANCH_CODE) VALUES
                            ('{FileName}','{descriptions[tagName]}','S','{model.entity_code}','ATTN',1,'{model.user_id}',TO_DATE('{model.Saved_Date}','MM/dd/yyyy hh24:mi:ss'),'{model.COMPANY_CODE}','{model.BRANCH_CODE}')";
                var row = dbContext.ExecuteSqlCommand(ImageQuery);
            }
            string Atten = dbContext.SqlQuery<string>($"SELECT ATN_DEFAULT FROM DIST_PREFERENCE_SETUP WHERE COMPANY_CODE='{model.COMPANY_CODE}'").FirstOrDefault();
            Atten = string.IsNullOrWhiteSpace(Atten) ? "N" : Atten.Trim();
            string resultValue = "Image successfully uploaded";
            if (Atten == "Y")
            {
                string type = dbContext.SqlQuery<string>($"SELECT USER_TYPE FROM DIST_LOGIN_USER WHERE SP_CODE='{model.entity_code}' AND COMPANY_CODE='{model.COMPANY_CODE}'").FirstOrDefault();
                type = string.IsNullOrWhiteSpace(type) ? "N" : type.Trim();

                if (type.ToUpper() == "E" || type.ToUpper() == "S")
                {
                    int checkColumn = dbContext.SqlQuery<int>("SELECT COUNT(*) FROM user_tab_columns WHERE table_name = 'HRIS_ATTENDANCE' AND column_name = 'LOCATION'").FirstOrDefault();
                    if (checkColumn == 0)
                        dbContext.ExecuteSqlCommand("alter table HRIS_ATTENDANCE add LOCATION varchar2(100)");
                    var latLong = string.Empty;
                    if (!string.IsNullOrWhiteSpace(locationModel.latitude) && !string.IsNullOrWhiteSpace(locationModel.longitude))
                        latLong = $"{locationModel.latitude}, {locationModel.longitude}";
                    string AttendanceQuery = $@"INSERT INTO HRIS_ATTENDANCE (EMPLOYEE_ID,ATTENDANCE_DT,ATTENDANCE_FROM,ATTENDANCE_TIME,LOCATION) VALUES ('{model.entity_code}',TRUNC(TO_DATE('{model.Saved_Date}','MM/dd/yyyy hh24:mi:ss')),'MOBILE',TO_TIMESTAMP('{model.Saved_Date}','MM/dd/yyyy hh24:mi:ss'),'{latLong}')";
                    var row = dbContext.ExecuteSqlCommand(AttendanceQuery);
                    var time = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss.fffffff tt", CultureInfo.InvariantCulture);
                    try
                    {
                        var thumbId = dbContext.SqlQuery<string>($"SELECT ID_THUMB_ID FROM HRIS_EMPLOYEES WHERE EMPLOYEE_ID = '{model.entity_code}'").FirstOrDefault();
                        if (!string.IsNullOrWhiteSpace(thumbId))
                        {
                            var hris_procedure = $"BEGIN HRIS_ATTENDANCE_INSERT ({thumbId}, TRUNC(SYSDATE), NULL, 'MOBILE', TO_TIMESTAMP('{time}'), {latLong}); END;";
                            dbContext.ExecuteSqlCommand(hris_procedure);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    if (row <= 0)
                        resultValue += " But could not make attendence made";
                    else
                        resultValue += " And attendence made";
                }
            }
            result.Add("msg", resultValue);
            // }
            // else
            // {
            //  result.Add("msg", "Attendence Successful");
            // }
            return result;
        }

        public Dictionary<string, string> UploadTrackingData(DistanceTrackingModel model, HttpFileCollection Files, NeoErpCoreEntity dbContext)
        {
            var result = new Dictionary<string, string>();
            try
            {
                var today = DateTime.Now.ToString("MM/dd/yyyyHH:mm:ss");
                var fileId = 0;
                HttpPostedFile file = Files[0];
                string UserFolderpath = string.Empty;

                UserFolderpath = UploadPath + "\\TrackingImage";

                if (!Directory.Exists(UserFolderpath))
                    Directory.CreateDirectory(UserFolderpath);
                string FileName = string.Format("TRK_{0}_{1}{2}", model.sp_code, DateTime.Now.ToString("dd-MM-yyyy"), Path.GetExtension(file.FileName));
                string filePath = Path.Combine(UserFolderpath, FileName);
                int count = 1;
                while (File.Exists(filePath))
                {
                    FileName = string.Format("TRK_{0}_{1}_{2}{3}", model.sp_code, DateTime.Now.ToString("dd-MM-yyyy"), count++, Path.GetExtension(file.FileName));
                    filePath = Path.Combine(UserFolderpath, FileName);
                }
                file.SaveAs(filePath);
                fileId = dbContext.SqlQuery<int>("SELECT NVL(MAX(ID),0)+1 ID FROM DIST_PHOTO_INFO").FirstOrDefault();

                string ImageQuery = $@"INSERT INTO DIST_PHOTO_INFO (FILENAME,DESCRIPTION,ENTITY_TYPE,ENTITY_CODE,MEDIA_TYPE,CATEGORYID,CREATED_BY,CREATE_DATE,COMPANY_CODE,BRANCH_CODE,ID) VALUES
                            ('{FileName}','','','{model.sp_code}','TRK','','{model.sp_code}',TO_DATE('{today}','MM/dd/yyyy hh24:mi:ss'),'{model.COMPANY_CODE}','{model.BRANCH_CODE}',{fileId})";
                var data = dbContext.ExecuteSqlCommand(ImageQuery);
                string resultValue = "Image successfully uploaded";
                if (data > 0)
                {
                    var trackingId = dbContext.SqlQuery<int>("SELECT NVL(MAX(ID),0)+1 ID FROM DIST_DISTANCE_TRACKING").FirstOrDefault();
                    string Query = $@"INSERT INTO Dist_distance_tracking (Id, sp_code, Distance, File_id, Created_by, Created_dt) 
                        VALUES ({trackingId}, '{model.sp_code}', '{model.Km_Run}', {fileId}, '{model.sp_code}',TRUNC(TO_DATE('{today}','MM/dd/yyyy hh24:mi:ss')))";
                    var row = dbContext.ExecuteSqlCommand(Query);
                    if (row > 0)
                        resultValue += "Distance Tracking saved successfully.";
                    else
                        resultValue += "Failed to save distance tracking.";
                }
                result.Add("msg", resultValue);
                return result;
            }
            catch (Exception ex)
            {
                result.Add("msg", $"An error occurred: {ex.Message}");
                return result;
            }
        }
        //public Dictionary<string, string> UploadDistSalesReturnPic(NameValueCollection form, HttpFileCollection Files, NeoErpCoreEntity dbContext)
        //{
        //    var result = new Dictionary<string, string>();
        //    var today = DateTime.Now.ToString("MM/dd/yyyyHH:mm:ss");
        //    // var time = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
        //    var coll = new Dictionary<string, string>();
        //    string item_code = string.Empty;
        //    int itemCount = 0;
        //    foreach (string tagName in Files)
        //    {
        //        HttpPostedFile file = Files[tagName];
        //        string UserFolderpath = string.Empty;

        //        coll.Add(tagName, form[$"description"]);
        //        UserFolderpath = UploadPath + "\\DistSalesReturnImages";  //model.entity_code is the sp_code(sales person code)

        //        if (!Directory.Exists(UserFolderpath))
        //            Directory.CreateDirectory(UserFolderpath);
        //        string FileName = string.Format("DSR_{0}_{1}{2}", form["itemcode[" + itemCount + "]"], form["order_no[" + itemCount + "]"], Path.GetExtension(file.FileName));
        //        string filePath = Path.Combine(UserFolderpath, FileName);
        //        int count = 1;
        //        while (File.Exists(filePath))
        //        {
        //            FileName = string.Format("DSR_{0}_{1}_{2}{3}", form["itemcode[" + itemCount + "]"], form["order_no[" + itemCount + "]"], itemCount, Path.GetExtension(file.FileName));
        //            filePath = Path.Combine(UserFolderpath, FileName);
        //            break;
        //        }

        //        file.SaveAs(filePath);

        //        string ImageQuery = $@"INSERT INTO DIST_PHOTO_INFO (FILENAME,DESCRIPTION,ENTITY_TYPE,ENTITY_CODE,MEDIA_TYPE,CATEGORYID,CREATED_BY,CREATE_DATE,COMPANY_CODE,BRANCH_CODE) VALUES
        //                    ('{FileName}','{coll[tagName]}','R','{form["SP_CODE"]}','GENERAL',1,'{form["SP_CODE"]}',TO_DATE('{today}','MM/dd/yyyy hh24:mi:ss'),'{form["COMPANY_CODE"]}','{form["BRANCH_CODE"]}')";
        //        var row = dbContext.ExecuteSqlCommand(ImageQuery);
        //        itemCount++;
        //    }

        //    string resultValue = "Image successfully uploaded";

        //    result.Add("msg", resultValue);
        //    // }
        //    // else
        //    // {
        //    //  result.Add("msg", "Attendence Successful");
        //    // }
        //    return result;
        //}

        public Dictionary<string, string> UploadDistSalesReturnPic(DistributionSalesReturnModel returnModel, HttpFileCollection Files, Dictionary<string, string> descriptions, NeoErpCoreEntity dbContext)
        {
            var result = new Dictionary<string, string>();
            var todayDt = DateTime.Now.ToString("MM/dd/yyyyHH:mm:ss");
            // var time = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            var coll = new Dictionary<string, string>();
            string item_code = string.Empty;
            int itemCount = 0;
            try
            {
                var saveResult = new Dictionary<string, string>();
                if (returnModel.locationinfo != null)
                {
                    returnModel.locationinfo.remarks = "Sales Return Begin(auto)";
                    var locationRes = this.UpdateMyLocation(returnModel.locationinfo, dbContext);
                }

                int id = 0;
                long idL = 1L;
                if (returnModel.ENTITY_TYPE.Equals("P", StringComparison.OrdinalIgnoreCase) || returnModel.ENTITY_TYPE.Equals("DEALER", StringComparison.OrdinalIgnoreCase)
                    || returnModel.ENTITY_TYPE.Equals("D", StringComparison.OrdinalIgnoreCase) || returnModel.ENTITY_TYPE.Equals("DISTRIBUTOR", StringComparison.OrdinalIgnoreCase))
                    //idL =this.GetMaxIdSalesReturn("DIST_SALES_RETURN", "RETURN_NO", dbContext);
                    idL = 2;
                else if (returnModel.ENTITY_TYPE.Equals("R", StringComparison.OrdinalIgnoreCase) || returnModel.ENTITY_TYPE.Equals("RESELLER", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrWhiteSpace(returnModel.RESELLER_CODE))
                        throw new Exception("Reseller code is empty");
                    //idL=this.GetMaxIdSalesReturn("DIST_SALES_RETURN", "RETURN_NO", dbContext);
                }
                else
                    throw new Exception("Invalid customer type");
                if (idL <= 0)
                    throw new Exception("Unable to get next ID for the sales return.");

                var today = $"TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}','MM/dd/yyyy hh24:mi:ss')";
                returnModel.Saved_Date = string.IsNullOrWhiteSpace(returnModel.Saved_Date
                    ) ? today : $"TO_DATE('{returnModel.Saved_Date}','MM/dd/yyyy hh24:mi:ss')";
                //var OrderDate = string.IsNullOrWhiteSpace(returnModel.ORDER_DATE.ToString()) ? today : $"TO_DATE('{returnModel.ORDER_DATE}','MM/dd/yyyy hh24:mi:ss')";

                foreach (var item in returnModel.products)
                {
                    item.PARTY_TYPE_CODE = item.PARTY_TYPE_CODE ?? "";
                    string InsertQuery = string.Empty;
                    string priceQuery = $"SELECT NVL(SALES_PRICE,0) SALES_PRICE FROM IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '{item.ITEM_CODE}' AND COMPANY_CODE='{returnModel.COMPANY_CODE}'";
                    decimal SP = dbContext.SqlQuery<decimal>(priceQuery).FirstOrDefault();

                    var saveQuery = $@"INSERT INTO DIST_SALES_RETURN(RETURN_NO,RETURN_DATE,CUSTOMER_CODE,SERIAL_NO,ITEM_CODE,MU_CODE,QUANTITY,FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,
                                                    MFD_DATE,EXPIRY_DATE,RETRUN_CONDITIONS,COMPLAIN_TYPE,COMPLAIN_SERIOUSNESS,DISTRIBUTOR_REMARKS,ASM_REMARKS,BATCH_NO,CUSTOMER_TYPE,DELETED_FLAG,REMARKS) 
                                       VALUES('{returnModel.ORDER_NO}',TRUNC(TO_DATE('{returnModel.ORDER_DATE.ToShortDateString()}','MM/DD/YYYY')),'{returnModel.CUSTOMER_CODE}',
                                                   '{item.BATCH_NO}','{item.ITEM_CODE}','{item.MU_CODE}','{item.QUANTITY}','0','{returnModel.COMPANY_CODE}','{returnModel.BRANCH_CODE}',
                                                   '{returnModel.user_id}',SYSDATE,'{item.MBF_DATA}','{item.EXP_DATE}',
                                                    '{returnModel.CONDITION}','{returnModel.COMPLAIN_TYPE}','{returnModel.SERIOUSNESS}','{returnModel.REMARKS_DIST}','{returnModel.REMARKS_ASM}','{item.BATCH_NO}','{returnModel.ENTITY_TYPE}','N', '{item.REMARKS}')";
                    var rowAffacted = dbContext.ExecuteSqlCommand(saveQuery);
                    saveResult.Add(item.SYNC_ID, returnModel.ORDER_NO);

                }
                string lastTag = "";
                foreach (string tagName in Files)
                {
                    int count = 0;
                    string imageTag = "";
                    if (tagName == lastTag)
                    {
                        count++;
                    }
                    else
                    {
                        count = 0;
                        lastTag = tagName;
                    }

                    string imageKey = tagName + "[" + count + "]";
                    imageTag = imageKey;


                    HttpPostedFile file = Files[tagName];
                    string UserFolderpath = string.Empty;

                    //coll.Add(tagName, form[$"description"]);
                    UserFolderpath = UploadPath + "\\DistSalesReturnImages";  //model.entity_code is the sp_code(sales person code)

                    if (!Directory.Exists(UserFolderpath))
                        Directory.CreateDirectory(UserFolderpath);
                    string FileName = string.Format("DSR_{0}_{1}{2}", returnModel.ORDER_NO, itemCount, Path.GetExtension(file.FileName));
                    string filePath = Path.Combine(UserFolderpath, FileName);
                    //int count = 1;
                    while (File.Exists(filePath))
                    {
                        FileName = string.Format("DSR_{0}_{1}_{2}{3}", returnModel.ORDER_NO, itemCount, itemCount, Path.GetExtension(file.FileName));
                        filePath = Path.Combine(UserFolderpath, FileName);
                        break;
                    }
                    file.SaveAs(filePath);
                    var desc = string.Empty;
                    if (descriptions != null && descriptions.Count > 0 && descriptions.ContainsKey(imageTag) && descriptions[imageTag] != null)
                        desc = descriptions?[imageTag];
                    string ImageQuery = $@"INSERT INTO DIST_PHOTO_INFO (FILENAME,DESCRIPTION,ENTITY_TYPE,ENTITY_CODE,MEDIA_TYPE,CATEGORYID,CREATED_BY,CREATE_DATE,COMPANY_CODE,BRANCH_CODE) VALUES
                            ('{FileName}','{desc}','{returnModel.ENTITY_TYPE}','{returnModel.CUSTOMER_CODE}','GENERAL',1,'{returnModel.user_id}',TO_DATE('{todayDt}','MM/dd/yyyy hh24:mi:ss'),'{returnModel.COMPANY_CODE}','{returnModel.BRANCH_CODE}')";
                    var row = dbContext.ExecuteSqlCommand(ImageQuery);
                    itemCount++;
                }
                //return saveResult;
                string resultValue = "Data saved successfully";

                result.Add("msg", resultValue);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string SaveScheme(SchemeModel model, NeoErpCoreEntity dbContext)
        {
            try
            {
                var SchemeId = dbContext.SqlQuery<int>("SELECT NVL(MAX(SCHEME_CODE),0)+1 S_ID FROM BRD_SCHEME").FirstOrDefault();
                string InsertQuery = $@"INSERT INTO BRD_SCHEME (SCHEME_CODE, CONTRACT_CODE, RESELLER_CODE, EMPLOYEE_CODE, ITEM_CODE, QUANTITY, MU_CODE, END_USER,
                        DIVISION_CODE, BRAND_CODE, HANDOVER_DATE, CREATED_BY, CREATED_DATE, DELETED_FLAG) VALUES(
                        '{SchemeId}','{model.CONTRACT_CODE}','{model.RESELLER_CODE}','{model.user_id}','{model.ITEM_CODE}','{model.QUANTITY}','{model.MU_CODE}','{model.END_USER}',
                        '{model.DIVISION_CODE}','{model.BRAND_CODE}',TO_DATE('{model.HANDOVER_DATE.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR'),'{model.user_id}',SYSDATE,'N')";
                dbContext.ExecuteSqlCommand(InsertQuery);
            }
            catch
            {
                throw new Exception("Something went Wrong");
            }
            return "Success";
        }
        #endregion Inserting Data

        #region Sending Mail
        //public bool SendEODMail(List<UpdateEodUpdate> model, NeoErpCoreEntity dbContext)
        //{
        //    var SpCode = model[0].sp_code;
        //    var Company = model[0].COMPANY_CODE;
        public bool SendEODMail(List<UpdateEodUpdate> model, string sp_code, string company_code, NeoErpCoreEntity dbContext)
        {
            //var SpCode = model[0].sp_code;
            //var Company = model[0].COMPANY_CODE;
            var SpCode = sp_code;

            var Company = company_code;
            //Old Running -aaku
            //var emailQuery = $@"(SELECT EMAIL FROM DIST_LOGIN_USER WHERE SP_CODE='{SpCode}')
            //                UNION ALL
            //                (SELECT EMAIL FROM DIST_LOGIN_USER WHERE USERID IN (SELECT PARENT_USERID FROM DIST_LOGIN_USER WHERE SP_CODE='{SpCode}'))";


            //var emailQuery = $@"SELECT EMAIL  FROM DIST_LOGIN_USER START WITH SP_CODE = '{SpCode}' CONNECT BY PRIOR  PARENT_USERID = USERID 
            //                             UNION  
            //                            SELECT EMAIL FROM DIST_LOGIN_USER WHERE SUPER_USER='Y'";

            var emailQuery = $@"SELECT EMAIL  FROM DIST_LOGIN_USER where SP_CODE = '{SpCode}' ";

            var parentEmail = dbContext.SqlQuery<string>(emailQuery).ToList();
            //List<string> parentEmail = new List<string>();
            //parentEmail.Add("animesh.gautam@neosoftware.com.np");
            ////parentEmail.Add("ashok.chhetri@neosoftware.com.np");

            //parentEmail.Add("bikalp.karn@neosoftware.com.np");

            if (parentEmail.Count > 0)
            {
                var SpName = dbContext.SqlQuery<string>($"SELECT FULL_NAME FROM DIST_LOGIN_USER WHERE SP_CODE='{SpCode}'").FirstOrDefault();
                //sending mail start
                var mailModel = new Core.Models.CustomModels.MailListModel()
                {
                    EMAIL_TO = string.Join(",", parentEmail),
                    SUBJECT = $"End Of Day Update Of {SpName}({SpCode})",
                    MESSAGE = "Attachments"
                };
                var companyName = dbContext.SqlQuery<string>($@"SELECT COMPANY_EDESC FROM COMPANY_SETUP WHERE COMPANY_CODE='{Company}'").FirstOrDefault();
                var ResPOQuery = String.Empty;
                var SalesPersonQuery = String.Empty;
                if (companyName.Equals("Bhudeo Khadya Udyog P. Ltd."))
                {
                    ResPOQuery = $@"   SELECT WM_CONCAT(DISTINCT ROUTE_NAME) ROUTE_NAME, GROUP_EDESC,SP_CODE, EMPLOYEE_EDESC, ASSIGN_DATE,ATN_TIME,EOD_TIME,WORKING_HOURS,
sum(TARGET) TARGET,sum(VISITED) TARGET_VISITED,sum(TOTAL_VISITED) VISITED,sum(EXTRA) EXTRA,sum(NOT_VISITED) NOT_VISITED,
sum(TOTAL_PJP) TOTAL_PJP,sum(PJP) PJP_PRODUCTIVE,sum(NON_PJP) PJP_NON_PRODUCTIVE,
sum(NON_N_PJP) NPJP_PRODUCTIVE,sum(TOTAL_QUANTITY) PJP_TOTAL_QUANTITY,sum(TOTAL_PRICE) PJP_TOTAL_AMOUNT,
ROUND( (sum(TOTAL_VISITED)/DECODE(sum(TARGET),0,1,sum(TARGET))  * 100),2)  PERCENT_EFFECTIVE_CALLS,
ROUND( (sum(TOTAL_PJP)/DECODE(sum(TOTAL_VISITED),0,1,sum(TOTAL_VISITED)) * 100),2)  PERCENT_PRODUCTIVE_CALLS,
EOD_REMARKS
FROM(
SELECT WM_CONCAT(DISTINCT ROUTE_NAME) ROUTE_NAME,GROUP_EDESC,SP_CODE, FULL_NAME EMPLOYEE_EDESC,TRUNC(ASSIGN_DATE) ASSIGN_DATE,
TO_CHAR(ATN_TIME,'HH:MI:SS A.M.') ATN_TIME,
CASE WHEN ATN_TIME = EOD_TIME THEN NULL
ELSE TO_CHAR(EOD_TIME,'HH:MI:SS A.M.')
END EOD_TIME,
--TO_CHAR(EOD_TIME,'HH:MI:SS A.M.') EOD_TIME,
NVL(ROUND(24 * (EOD_TIME - ATN_TIME),2),0) WORKING_HOURS,
SUM(TARGET) TARGET,
SUM(VISITED) VISITED
,NVL((SELECT COUNT(DISTINCT CUSTOMER_CODE) FROM DIST_VISITED_ENTITY WHERE USERID = AA.USERID AND COMPANY_CODE = AA.COMPANY_CODE AND TRUNC(UPDATE_DATE)= TRUNC(AA.ASSIGN_DATE)),0) TOTAL_VISITED
, SUM(NVL((SELECT COUNT(DISTINCT CUSTOMER_CODE) FROM DIST_VISITED_ENTITY WHERE USERID = AA.USERID AND COMPANY_CODE = AA.COMPANY_CODE AND TRUNC(UPDATE_DATE)= TRUNC(AA.ASSIGN_DATE)),0)  - VISITED) EXTRA, SUM(TARGET- VISITED) NOT_VISITED
,SUM(PJP) PJP
, SUM(VISITED - PJP)  NON_PJP
, SUM(NVL((SELECT COUNT(DISTINCT RESELLER_CODE) FROM DIST_VISITED_PO WHERE USERID = AA.USERID AND COMPANY_CODE = AA.COMPANY_CODE AND TRUNC(ORDER_DATE) = TRUNC(AA.ASSIGN_DATE)),0)- PJP) NON_N_PJP
,NVL((SELECT COUNT(DISTINCT RESELLER_CODE) FROM DIST_VISITED_PO WHERE USERID = AA.USERID AND COMPANY_CODE = AA.COMPANY_CODE AND TRUNC(ORDER_DATE) = TRUNC(AA.ASSIGN_DATE) ),0) TOTAL_PJP
,NVL((SELECT SUM(QUANTITY)  FROM DIST_VISITED_PO WHERE USERID = AA.USERID AND COMPANY_CODE = AA.COMPANY_CODE AND TRUNC(ORDER_DATE) = TRUNC(AA.ASSIGN_DATE)),0) TOTAL_QUANTITY
,NVL((SELECT SUM(TOTAL_PRICE)  FROM DIST_VISITED_PO WHERE USERID = AA.USERID AND COMPANY_CODE = AA.COMPANY_CODE AND TRUNC(ORDER_DATE) =TRUNC(AA.ASSIGN_DATE)),0) TOTAL_PRICE                   
,EOD_REMARKS
FROM(
SELECT  B.ROUTE_NAME ROUTE_NAME,  B.GROUP_EDESC, A.USERID, A.FULL_NAME, A.SP_CODE, B.ASSIGN_DATE, B.COMPANY_CODE
,(SELECT MIN(SUBMIT_DATE) FROM DIST_LM_LOCATION_TRACKING WHERE TRACK_TYPE='ATN' AND SP_CODE = A.SP_CODE AND TRUNC(SUBMIT_DATE) =TRUNC(B.ASSIGN_DATE)) ATN_TIME
,(SELECT TO_DATE(TO_CHAR(MAX(ATTENDANCE_TIME),'DD/MM/YYYY HH:MI:SS AM'),'DD/MM/YYYY HH:MI:SS AM') FROM HRIS_ATTENDANCE WHERE EMPLOYEE_ID = A.SP_CODE AND TRUNC(ATTENDANCE_DT) = TRUNC(B.ASSIGN_DATE)) EOD_TIME
,(SELECT REMARKS FROM DIST_EOD_UPDATE WHERE TRUNC(CREATED_DATE) = TO_DATE(B.ASSIGN_DATE,'RRRR-MON-DD')  AND SP_CODE =  A.SP_CODE AND ROWNUM = 1) EOD_REMARKS
,CASE WHEN WM_CONCAT(B.ENTITY_CODE) IS NULL THEN 0
ELSE NVL(COUNT(*),0)
END TARGET
,NVL((SELECT COUNT(DISTINCT CUSTOMER_CODE) FROM DIST_VISITED_ENTITY WHERE USERID = A.USERID AND COMPANY_CODE = A.COMPANY_CODE AND TRUNC(UPDATE_DATE) = trunc(B.ASSIGN_DATE) AND CUSTOMER_CODE IN (SELECT ENTITY_CODE FROM DIST_TARGET_ENTITY WHERE  USERID = A.USERID AND COMPANY_CODE = A.COMPANY_CODE  AND ROUTE_CODE = B.ROUTE_CODE AND TRUNC(ASSIGN_DATE) = trunc(B.ASSIGN_DATE) )),0) VISITED
,NVL((SELECT COUNT(DISTINCT RESELLER_CODE) FROM DIST_VISITED_PO WHERE USERID = A.USERID AND COMPANY_CODE = A.COMPANY_CODE AND TRUNC(ORDER_DATE) =trunc(B.ASSIGN_DATE) AND RESELLER_CODE IN (SELECT ENTITY_CODE FROM DIST_TARGET_ENTITY WHERE  USERID = A.USERID AND COMPANY_CODE = A.COMPANY_CODE AND ROUTE_CODE = B.ROUTE_CODE AND TRUNC(ASSIGN_DATE) = trunc(B.ASSIGN_DATE))),0) PJP
FROM DIST_LOGIN_USER A, DIST_TARGET_ENTITY B  
WHERE A.USERID = B.USERID
AND A.COMPANY_CODE = B.COMPANY_CODE
AND A.ACTIVE = 'Y'
AND A.COMPANY_CODE IN ('{Company}')
AND B.ASSIGN_DATE  BETWEEN TO_DATE('2021-Dec-15','RRRR-MON-DD') AND TO_DATE('2021-Dec-15','RRRR-MON-DD') /*sysdate*/
GROUP BY A.USERID, A.FULL_NAME, A.SP_CODE, B.ASSIGN_DATE, A.COMPANY_CODE,B.ROUTE_CODE, B.ROUTE_NAME,B.GROUP_EDESC, B.COMPANY_CODE
ORDER BY B.ASSIGN_DATE) AA
WHERE 1=1  
 AND SP_CODE IN  ('{SpCode}')
 GROUP BY  USERID, COMPANY_CODE,TRUNC(ASSIGN_DATE),  ATN_TIME,EOD_TIME, SP_CODE,GROUP_EDESC,SP_CODE, FULL_NAME,EOD_REMARKS)  group by   ASSIGN_DATE,ATN_TIME,EOD_TIME, SP_CODE,GROUP_EDESC,SP_CODE,EOD_REMARKS,EMPLOYEE_EDESC,WORKING_HOURS  order by sp_code
 ";


                    SalesPersonQuery = $@"SELECT * FROM (
                                SELECT DPO1.ORDER_NO, DPO1.ORDER_DATE,BS_DATE(TO_CHAR(DPO1.ORDER_DATE)) MITI, DPO1.CUSTOMER_CODE, DPO1.BILLING_NAME CUSTOMER_EDESC, '' RESELLER_NAME, 'D' ORDER_ENTITY, TRIM(IMS.ITEM_EDESC) ITEM_EDESC, 
                                        DPO1.MU_CODE, DPO1.QUANTITY, DPO1.UNIT_PRICE, DPO1.TOTAL_PRICE NET_TOTAL, IUS.MU_CODE CONVERSION_MU_CODE, IUS.CONVERSION_FACTOR,
                                         DPO1.PARTY_TYPE_CODE,
                                        (CASE WHEN DPO1.PARTY_TYPE_CODE IS NULL
                                          THEN FN_FETCH_DESC (DPO1.COMPANY_CODE,'IP_PARTY_TYPE_CODE',CS.PARTY_TYPE_CODE)
                                          ELSE FN_FETCH_DESC (DPO1.COMPANY_CODE,'IP_PARTY_TYPE_CODE',DPO1.PARTY_TYPE_CODE)
                                        END
                                        ) PARTY_TYPE_EDESC,
                                        DPO1.CREATED_BY, DPO1.CREATED_DATE, DPO1.DELETED_FLAG,
                                        --DPO1.COMPANY_CODE, DPO1.BRANCH_CODE,
                                        DPO1.REMARKS,
                                        DPO1.APPROVED_FLAG, DPO1.DISPATCH_FLAG, DPO1.ACKNOWLEDGE_FLAG, DPO1.REJECT_FLAG,
                                        ES.EMPLOYEE_EDESC,
                                        PS.PO_PARTY_TYPE,
                                        PS.PO_CONVERSION_UNIT,
                                        PS.PO_CONVERSION_FACTOR,
                                        PS.SO_CREDIT_LIMIT_CHK SO_CREDIT_LIMIT_FLAG,
                                        NVL(DPO2.TOTAL_QUANTITY,0) TOTAL_QUANTITY,
                                        DPO2.TOTAL_AMOUNT Grand_Total_Amount,
                                        NVL(DPO2.TOTAL_APPROVE_QTY,0) GRAND_APPROVE_QUENTITY,
                                        NVL(DPO2.TOTAL_APPROVE_AMT,0) TOTAL_APPROVE_AMT,'Distributor' EntityName
                                FROM DIST_IP_SSD_PURCHASE_ORDER DPO1
                                INNER JOIN IP_ITEM_MASTER_SETUP IMS ON IMS.ITEM_CODE = DPO1.ITEM_CODE AND IMS.COMPANY_CODE = DPO1.COMPANY_CODE AND IMS.CATEGORY_CODE in (select CATEGORY_CODE  from IP_CATEGORY_CODE WHERE CATEGORY_TYPE IN ('FG','TF') and company_code='01') AND IMS.GROUP_SKU_FLAG = 'I'
                                LEFT JOIN SA_CUSTOMER_SETUP CS ON CS.CUSTOMER_CODE = DPO1.CUSTOMER_CODE AND CS.COMPANY_CODE = DPO1.COMPANY_CODE
                                INNER JOIN DIST_LOGIN_USER LU ON LU.USERID = DPO1.CREATED_BY AND LU.ACTIVE = 'Y'
                                INNER JOIN HR_EMPLOYEE_SETUP ES ON ES.EMPLOYEE_CODE = LU.SP_CODE AND ES.COMPANY_CODE = LU.COMPANY_CODE
                                LEFT JOIN IP_ITEM_UNIT_SETUP IUS ON IUS.ITEM_CODE = DPO1.ITEM_CODE AND IUS.COMPANY_CODE = DPO1.COMPANY_CODE
                                INNER JOIN DIST_PREFERENCE_SETUP PS ON PS.COMPANY_CODE = DPO1.COMPANY_CODE
                                INNER JOIN (SELECT POT.ORDER_NO, SUM(POT.NET_QUANTITY) TOTAL_QUANTITY, SUM(POT.NET_PRICE) TOTAL_AMOUNT, SUM(POT.APPROVE_QTY) TOTAL_APPROVE_QTY, SUM(POT.APPROVE_AMT) TOTAL_APPROVE_AMT
                                FROM (SELECT A.ORDER_NO, A.ITEM_CODE, A.MU_CODE, A.QUANTITY, A.TOTAL_PRICE NET_PRICE, A.APPROVE_QTY, A.APPROVE_AMT, C.MU_CODE AS CONVERSION_UNIT, C.CONVERSION_FACTOR,
                                      (CASE
                                        WHEN (C.MU_CODE IS NULL AND C.CONVERSION_FACTOR IS NULL)
                                        THEN A.QUANTITY
                                        ELSE (CASE WHEN A.MU_CODE = C.MU_CODE THEN A.QUANTITY ELSE (A.QUANTITY * C.CONVERSION_FACTOR) END)
                                      END) NET_QUANTITY
                                      FROM DIST_IP_SSD_PURCHASE_ORDER A
                                      LEFT JOIN IP_ITEM_UNIT_SETUP C ON C.ITEM_CODE = A.ITEM_CODE AND C.COMPANY_CODE = A.COMPANY_CODE
                                ) POT
                                GROUP BY POT.ORDER_NO) DPO2 ON DPO2.ORDER_NO = DPO1.ORDER_NO
                                WHERE 1 = 1
                                      AND TRUNC(DPO1.ORDER_DATE) BETWEEN TO_DATE('2021-Jul-16','YYYY-MON-DD') AND TO_DATE('2022-Jul-16','YYYY-MON-DD')
                                      AND DPO1.DELETED_FLAG = 'N'
                                       AND DPO1.REJECT_FLAG = 'N'
                                AND DPO1.APPROVED_FLAG = 'N'
                                AND  LU.SP_CODE ='1000839' 
                                    AND DPO1.COMPANY_CODE IN ('01')
                                GROUP BY DPO1.ORDER_NO, DPO1.ORDER_DATE,BS_DATE(TO_CHAR(DPO1.ORDER_DATE)), DPO1.CUSTOMER_CODE, DPO1.BILLING_NAME, '', TRIM(IMS.ITEM_EDESC), 
                                       DPO1.MU_CODE, DPO1.QUANTITY, DPO1.UNIT_PRICE, DPO1.TOTAL_PRICE, IUS.MU_CODE, IUS.CONVERSION_FACTOR, 
                                       'D', DPO1.PARTY_TYPE_CODE,
                                       (CASE WHEN DPO1.PARTY_TYPE_CODE IS NULL
                                          THEN FN_FETCH_DESC (DPO1.COMPANY_CODE,'IP_PARTY_TYPE_CODE',CS.PARTY_TYPE_CODE)
                                          ELSE FN_FETCH_DESC (DPO1.COMPANY_CODE,'IP_PARTY_TYPE_CODE',DPO1.PARTY_TYPE_CODE)
                                        END
                                       ),
                                       DPO1.CREATED_BY, DPO1.CREATED_DATE, DPO1.DELETED_FLAG,
                                       --DPO1.COMPANY_CODE, DPO1.BRANCH_CODE,
                                       DPO1.REMARKS,
                                       DPO1.APPROVED_FLAG, DPO1.DISPATCH_FLAG, DPO1.ACKNOWLEDGE_FLAG, DPO1.REJECT_FLAG,
                                       ES.EMPLOYEE_EDESC,
                                       PS.PO_PARTY_TYPE,
                                       PS.PO_CONVERSION_UNIT,
                                       PS.PO_CONVERSION_FACTOR,
                                       PS.SO_CREDIT_LIMIT_CHK,
                                       DPO2.TOTAL_QUANTITY,
                                       DPO2.TOTAL_AMOUNT,
                                       DPO2.TOTAL_APPROVE_QTY,
                                       DPO2.TOTAL_APPROVE_AMT,
                                       LU.SP_CODE
                            union all SELECT DPO1.ORDER_NO,DPO1.ORDER_DATE,BS_DATE(TO_CHAR(DPO1.ORDER_DATE)), DPO1.CUSTOMER_CODE,DPO1.BILLING_NAME CUSTOMER_EDESC, RM.RESELLER_NAME, 'R' ORDER_ENTITY,TRIM(IMS.ITEM_EDESC) ITEM_EDESC,
                                   DPO1.MU_CODE, DPO1.QUANTITY, DPO1.UNIT_PRICE, DPO1.TOTAL_PRICE NET_TOTAL, IUS.MU_CODE CONVERSION_MU_CODE, IUS.CONVERSION_FACTOR,
                                         DPO1.PARTY_TYPE_CODE,
                                        (CASE WHEN DPO1.PARTY_TYPE_CODE IS NULL
                                          THEN FN_FETCH_DESC (DPO1.COMPANY_CODE,'IP_PARTY_TYPE_CODE',CS.PARTY_TYPE_CODE)
                                          ELSE FN_FETCH_DESC (DPO1.COMPANY_CODE,'IP_PARTY_TYPE_CODE',DPO1.PARTY_TYPE_CODE)
                                        END
                                        ) PARTY_TYPE_EDESC,
                                        DPO1.CREATED_BY, DPO1.CREATED_DATE, DPO1.DELETED_FLAG,
                                        --DPO1.COMPANY_CODE, DPO1.BRANCH_CODE,
                                        DPO1.REMARKS,
                                        DPO1.APPROVED_FLAG, DPO1.DISPATCH_FLAG, DPO1.ACKNOWLEDGE_FLAG, DPO1.REJECT_FLAG,
                                        ES.EMPLOYEE_EDESC,
                                        PS.PO_PARTY_TYPE,
                                        PS.PO_CONVERSION_UNIT,
                                        PS.PO_CONVERSION_FACTOR,
                                        PS.SO_CREDIT_LIMIT_CHK SO_CREDIT_LIMIT_FLAG,
                                        NVL(DPO2.TOTAL_QUANTITY,0) TOTAL_QUANTITY,
                                        DPO2.TOTAL_AMOUNT Grand_Total_Amount,
                                        NVL(DPO2.TOTAL_APPROVE_QTY,0) GRAND_APPROVE_QUENTITY,
                                        NVL(DPO2.TOTAL_APPROVE_AMT,0) TOTAL_APPROVE_AMT,'Reseller' EntityName
                            FROM DIST_IP_SSR_PURCHASE_ORDER DPO1
                            INNER JOIN DIST_RESELLER_MASTER RM ON RM.RESELLER_CODE = DPO1.RESELLER_CODE AND RM.IS_CLOSED = 'N'
                            INNER JOIN IP_ITEM_MASTER_SETUP IMS ON IMS.ITEM_CODE = DPO1.ITEM_CODE AND IMS.COMPANY_CODE = DPO1.COMPANY_CODE AND IMS.CATEGORY_CODE in(select CATEGORY_CODE  from IP_CATEGORY_CODE WHERE CATEGORY_TYPE IN ('FG','TF') and company_code='01') AND IMS.GROUP_SKU_FLAG = 'I'
                            LEFT JOIN SA_CUSTOMER_SETUP CS ON CS.CUSTOMER_CODE = DPO1.CUSTOMER_CODE AND CS.COMPANY_CODE = DPO1.COMPANY_CODE
                            INNER JOIN DIST_LOGIN_USER LU ON LU.USERID = DPO1.CREATED_BY AND LU.ACTIVE = 'Y'
                            INNER JOIN HR_EMPLOYEE_SETUP ES ON ES.EMPLOYEE_CODE = LU.SP_CODE AND ES.COMPANY_CODE = LU.COMPANY_CODE
                            LEFT JOIN IP_ITEM_UNIT_SETUP IUS ON IUS.ITEM_CODE = DPO1.ITEM_CODE AND IUS.COMPANY_CODE = DPO1.COMPANY_CODE
                            INNER JOIN DIST_PREFERENCE_SETUP PS ON PS.COMPANY_CODE = DPO1.COMPANY_CODE
                            --LEFT JOIN (SELECT V.SUB_CODE, NVL((SUM (V.DR_AMOUNT) - SUM (V.CR_AMOUNT)),0) BALANCE
                            --  FROM V$VIRTUAL_SUB_LEDGER V
                            --  WHERE 1 = 1
                            --  AND V.COMPANY_CODE IN ('01')
                            --  AND V.SUB_LEDGER_FLAG = 'C'
                            -- GROUP BY V.SUB_CODE) VSL ON TRIM(VSL.SUB_CODE) = TRIM(CS.LINK_SUB_CODE)
                            INNER JOIN (SELECT POT.ORDER_NO, SUM(POT.NET_QUANTITY) TOTAL_QUANTITY, SUM(POT.NET_PRICE) TOTAL_AMOUNT, SUM(POT.APPROVE_QTY) TOTAL_APPROVE_QTY, SUM(POT.APPROVE_AMT) TOTAL_APPROVE_AMT
                                        FROM (SELECT A.ORDER_NO, A.ITEM_CODE, A.MU_CODE, A.QUANTITY, A.TOTAL_PRICE NET_PRICE, A.APPROVE_QTY, A.APPROVE_AMT, C.MU_CODE AS CONVERSION_UNIT, C.CONVERSION_FACTOR,
                                        (CASE
                                          WHEN (C.MU_CODE IS NULL AND C.CONVERSION_FACTOR IS NULL)
                                          THEN A.QUANTITY
                                          ELSE (CASE WHEN A.MU_CODE = C.MU_CODE THEN A.QUANTITY ELSE (A.QUANTITY * C.CONVERSION_FACTOR) END)
                                        END) NET_QUANTITY
                                        FROM DIST_IP_SSR_PURCHASE_ORDER A
                                        LEFT JOIN IP_ITEM_UNIT_SETUP C ON C.ITEM_CODE = A.ITEM_CODE AND C.COMPANY_CODE = A.COMPANY_CODE
                                        WHERE 1=1 
                                        --ORDER BY A.ORDER_NO DESC, A.ITEM_CODE
                            ) POT
                           GROUP BY POT.ORDER_NO) DPO2 ON DPO2.ORDER_NO = DPO1.ORDER_NO
                            WHERE 1 = 1
                              AND TRUNC(DPO1.ORDER_DATE) >= TO_DATE('2021-Jul-16','YYYY-MM-DD') AND TRUNC(DPO1.ORDER_DATE) <= TO_DATE('2022-Jul-16','YYYY-MM-DD')
                              AND DPO1.DELETED_FLAG = 'N'  
                              AND DPO1.COMPANY_CODE IN ('01')  
                              AND DPO1.CUSTOMER_CODE = '3081'                              
                            ) ORDER BY EMPLOYEE_EDESC, ITEM_EDESC, ORDER_NO
                            ";
                }
                else
                {
                    ResPOQuery = $@"SELECT  PO.EMPLOYEE_EDESC,  PO.BRAND_NAME,
                                    SUM(PO.TOTAL_QUANTITY) TOTAL_QUANTITY, SUM(PO.TOTAL_AMOUNT) TOTAL_AMOUNT,PO.MU_CODE FROM (SELECT DPO.CREATED_BY, DLU.SP_CODE, TRIM(HES.EMPLOYEE_EDESC) EMPLOYEE_EDESC, DPO.COMPANY_CODE, TRIM(ISS.BRAND_NAME) BRAND_NAME,
                                    SUM(DPO.QUANTITY) TOTAL_QUANTITY, SUM(DPO.TOTAL_PRICE) TOTAL_AMOUNT,DPO.MU_CODE
                                   FROM DIST_IP_SSD_PURCHASE_ORDER DPO
                                   INNER JOIN DIST_LOGIN_USER DLU ON DLU.USERID = DPO.CREATED_BY AND DLU.COMPANY_CODE = DPO.COMPANY_CODE AND DLU.SP_CODE = '{SpCode}'
                                   INNER JOIN HR_EMPLOYEE_SETUP HES ON HES.EMPLOYEE_CODE = DLU.SP_CODE AND HES.COMPANY_CODE = DPO.COMPANY_CODE
                                   INNER JOIN DIST_DISTRIBUTOR_MASTER D ON D.DISTRIBUTOR_CODE = DPO.CUSTOMER_CODE AND D.COMPANY_CODE = DPO.COMPANY_CODE
                                   INNER JOIN SA_CUSTOMER_SETUP SCS ON SCS.CUSTOMER_CODE = D.DISTRIBUTOR_CODE AND SCS.COMPANY_CODE = D.COMPANY_CODE
                                   LEFT JOIN IP_ITEM_SPEC_SETUP ISS ON ISS.ITEM_CODE = DPO.ITEM_CODE AND ISS.COMPANY_CODE = DPO.COMPANY_CODE
                                   WHERE TRUNC(DPO.ORDER_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR') AND DPO.COMPANY_CODE = '{Company}' AND
                                    TRIM(ISS.BRAND_NAME) IS NOT NULL
                                   GROUP BY DPO.CREATED_BY, DLU.SP_CODE, TRIM(HES.EMPLOYEE_EDESC), DPO.CUSTOMER_CODE, TRIM(SCS.CUSTOMER_EDESC), DPO.COMPANY_CODE, TRIM(ISS.BRAND_NAME),DPO.MU_CODE
                                       UNION ALL
                                       SELECT RPO.CREATED_BY, DLU.SP_CODE, TRIM(HES.EMPLOYEE_EDESC) EMPLOYEE_EDESC, RPO.COMPANY_CODE, TRIM(ISS.BRAND_NAME) BRAND_NAME,
                                SUM(RPO.QUANTITY) TOTAL_QUANTITY, SUM(RPO.TOTAL_PRICE) TOTAL_AMOUNT,RPO.MU_CODE
                                   FROM DIST_IP_SSR_PURCHASE_ORDER RPO
                                   INNER JOIN DIST_LOGIN_USER DLU ON DLU.USERID = RPO.CREATED_BY AND DLU.COMPANY_CODE = RPO.COMPANY_CODE AND DLU.SP_CODE = '{SpCode}'
                                   INNER JOIN HR_EMPLOYEE_SETUP HES ON HES.EMPLOYEE_CODE = DLU.SP_CODE AND HES.COMPANY_CODE = RPO.COMPANY_CODE
                                   INNER JOIN DIST_RESELLER_MASTER DRM ON DRM.RESELLER_CODE = RPO.RESELLER_CODE AND DRM.COMPANY_CODE = RPO.COMPANY_CODE
                                   LEFT JOIN IP_ITEM_SPEC_SETUP ISS ON ISS.ITEM_CODE = RPO.ITEM_CODE AND ISS.COMPANY_CODE = RPO.COMPANY_CODE
                                   WHERE TRUNC(RPO.ORDER_DATE) = TRUNC(TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR')) AND RPO.COMPANY_CODE = '{Company}' 
                                    AND TRIM(ISS.BRAND_NAME) IS NOT NULL
                                                                       GROUP BY RPO.CREATED_BY, DLU.SP_CODE, TRIM(HES.EMPLOYEE_EDESC), RPO.COMPANY_CODE, TRIM(ISS.BRAND_NAME),RPO.MU_CODE
                                    ) PO
                                    GROUP BY PO.SP_CODE, PO.EMPLOYEE_EDESC, PO.COMPANY_CODE, PO.BRAND_NAME,PO.MU_CODE";
                    //var DisPOQuery = $@"SELECT DISTINCT DS.ORDER_NO,IM.ITEM_EDESC,CS.CUSTOMER_EDESC, DS.ORDER_DATE, DS.CUSTOMER_CODE,DS.BILLING_NAME,DS.ITEM_CODE,DS.MU_CODE,DS.QUANTITY, DS.UNIT_PRICE,DS.TOTAL_PRICE
                    //                    FROM DIST_IP_SSD_PURCHASE_ORDER DS, DIST_LOGIN_USER LU,SA_CUSTOMER_SETUP CS,IP_ITEM_MASTER_SETUP IM,DIST_PHOTO_INFO DF,DIST_LM_LOCATION_TRACKING LT
                    //                     WHERE DS.CUSTOMER_CODE = CS.CUSTOMER_CODE AND CS.COMPANY_CODE =DS.COMPANY_CODE AND DS.ITEM_CODE = IM.ITEM_CODE AND DS.COMPANY_CODE= IM.COMPANY_CODE
                    //                     AND LU.USERID=DS.CREATED_BY AND LU.COMPANY_CODE=DS.COMPANY_CODE
                    //                     AND LT.SP_CODE=LU.SP_CODE AND LT.TRACK_TYPE='EOD'
                    //                     AND LU.SP_CODE = '{SpCode}'
                    //                  AND TRUNC(DS.ORDER_DATE) >=TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy') AND TRUNC(DS.ORDER_DATE) <=TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy')";


                }
                var ResellerData = dbContext.SqlQuery(ResPOQuery);
                //   var DistrubutorData = dbContext.SqlQuery(DisPOQuery);

                string EodData = string.Empty;


                if (companyName.Equals("JGI Distribution Pvt. Ltd."))
                {

                    #region EODQuery
                    EodData = $@"SELECT PFMTBL.GROUP_EDESC, PFMTBL.SP_CODE, PFMTBL.EMPLOYEE_EDESC,
  TODTBL.TOD_ROUTE_CODE, TODTBL.TOD_ROUTE_NAME, TOMTBL.TOM_ROUTE_CODE, TOMTBL.TOM_ROUTE_NAME,
  PFMTBL.ATN_IMAGE, PFMTBL.ATN_DATE, PFMTBL.ATN_LATITUDE, PFMTBL.ATN_LONGITUDE, PFMTBL.EOD_DATE, PFMTBL.EOD_LATITUDE, PFMTBL.EOD_LONGITUDE,
  SUM(PFMTBL.TARGET) TARGET, SUM(PFMTBL.VISITED) VISITED, SUM(PFMTBL.NOT_VISITED) NOT_VISITED,
  SUM(PFMTBL.PJP_PRODUCTIVE) PJP_PRODUCTIVE,
  SUM(PFMTBL.VISITED) - SUM(PFMTBL.PJP_PRODUCTIVE) PJP_NON_PRODUCTIVE,
  SUM(PFMTBL.NPJP_PRODUCTIVE) NPJP_PRODUCTIVE,  
ROUND(DECODE(SUM(PFMTBL.PJP_PRODUCTIVE), NULL, 0,
       0, 0,
      (SUM(PFMTBL.PJP_PRODUCTIVE) /DECODE(SUM(PFMTBL.VISITED),0,1, SUM(PFMTBL.VISITED)) * 100),2)) PERCENT_EFFECTIVE_CALLS,
  SUM(PFMTBL.OUTLET_ADDED) OUTLET_ADDED,
  SUM(PFMTBL.TOTAL_QUANTITY) TOTAL_QUANTITY, SUM(PFMTBL.TOTAL_AMOUNT) TOTAL_AMOUNT
FROM (SELECT PTBL.*
      FROM (SELECT PETBL.GROUP_EDESC, PETBL.SP_CODE, PETBL.EMPLOYEE_EDESC, PETBL.COMPANY_CODE, PETBL.ATN_IMAGE, PETBL.ATN_DATE, PETBL.ATN_LATITUDE, PETBL.ATN_LONGITUDE, PETBL.EOD_DATE, PETBL.EOD_LATITUDE, PETBL.EOD_LONGITUDE, NVL(PETBL.TARGET, 0) TARGET, 
            NVL(PVTBL.VISITED, 0) VISITED, NVL(PNVTBL.NOT_VISITED,0) NOT_VISITED, 
            NVL(PPJPTBL.PJP_PRODUCTIVE,0) PJP_PRODUCTIVE,
            (NVL(PVTBL.VISITED, 0) - NVL(PPJPTBL.PJP_PRODUCTIVE,0)) PJP_NON_PRODUCTIVE,
            NVL(PNPJPTBL.NPJP_PRODUCTIVE,0) NPJP_PRODUCTIVE,
            NVL(DECODE(PPJPTBL.PJP_PRODUCTIVE, NULL, 0,
                                             0, 0,
                                             ROUND((PPJPTBL.PJP_PRODUCTIVE / PVTBL.VISITED) * 100, 2)),0) NET_PERCENT_EFFECTIVE_CALLS,
            0 OUTLET_ADDED,
            NVL(PPJPTBL.TOTAL_QUANTITY,0) TOTAL_QUANTITY, NVL(PPJPTBL.TOTAL_PRICE,0) TOTAL_AMOUNT
            FROM (SELECT ENT.GROUP_EDESC, ENT.SP_CODE, ENT.EMPLOYEE_EDESC, ENT.COMPANY_CODE, ENT.ATN_IMAGE, ENT.ATN_DATE, ENT.ATN_LATITUDE, ENT.ATN_LONGITUDE, ENT.EOD_DATE, ENT.EOD_LATITUDE, ENT.EOD_LONGITUDE, COUNT(ENT.ENTITY_CODE) TARGET
                  FROM (
                      SELECT DLU.SP_CODE, TRIM(HES.EMPLOYEE_EDESC) EMPLOYEE_EDESC, TRIM(DGM.GROUP_EDESC) GROUP_EDESC, PTC.PARTY_TYPE_CODE ENTITY_CODE, TRIM(PTC.PARTY_TYPE_EDESC) ENTITY_NAME, DLU.COMPANY_CODE, API.ATN_IMAGE, ATN.ATN_DATE, ATN.ATN_LATITUDE, ATN.ATN_LONGITUDE, EOD.EOD_DATE, EOD.EOD_LATITUDE, EOD.EOD_LONGITUDE
                      FROM DIST_LOGIN_USER DLU
                      INNER JOIN HR_EMPLOYEE_SETUP HES ON HES.EMPLOYEE_CODE = DLU.SP_CODE AND HES.COMPANY_CODE = DLU.COMPANY_CODE
                      LEFT JOIN DIST_GROUP_MASTER DGM ON DGM.GROUPID = DLU.GROUPID AND DGM.COMPANY_CODE = DGM.COMPANY_CODE
                      LEFT JOIN (SELECT A.ENTITY_CODE SP_CODE, A.COMPANY_CODE, A.CREATE_DATE, A.FILENAME ATN_IMAGE
                                  FROM DIST_PHOTO_INFO A
                                  WHERE A.CREATE_DATE = (SELECT MAX(CREATE_DATE) FROM DIST_PHOTO_INFO WHERE ENTITY_CODE = A.ENTITY_CODE AND ENTITY_TYPE = 'S' AND (MEDIA_TYPE = 'ATN' OR CATEGORYID = 1) AND TRUNC(CREATE_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR'))
                                  GROUP BY A.ENTITY_CODE, A.COMPANY_CODE, A.CREATE_DATE, A.FILENAME
                                  ORDER BY A.ENTITY_CODE DESC
                      ) API ON API.SP_CODE = DLU.SP_CODE AND API.COMPANY_CODE = DLU.COMPANY_CODE
                      LEFT JOIN (SELECT A.EMPLOYEE_ID SP_CODE, A.ATTENDANCE_TIME ATN_DATE,''ATN_LATITUDE, '' ATN_LONGITUDE
                                  FROM HRIS_ATTENDANCE A
                                  WHERE A.ATTENDANCE_TIME = (SELECT MIN(ATTENDANCE_TIME) FROM HRIS_ATTENDANCE WHERE EMPLOYEE_ID = A.EMPLOYEE_ID AND TRUNC(ATTENDANCE_DT) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR'))
                                  GROUP BY A.EMPLOYEE_ID, A.ATTENDANCE_TIME
                                  ORDER BY A.EMPLOYEE_ID DESC
                      ) ATN ON ATN.SP_CODE = DLU.SP_CODE
                      LEFT JOIN (SELECT A.EMPLOYEE_ID SP_CODE, A.ATTENDANCE_TIME EOD_DATE, '' EOD_LATITUDE,'' EOD_LONGITUDE
                                  FROM HRIS_ATTENDANCE A
                                  WHERE A.ATTENDANCE_TIME = (SELECT MAX(ATTENDANCE_TIME) FROM HRIS_ATTENDANCE WHERE EMPLOYEE_ID = A.EMPLOYEE_ID AND TRUNC(ATTENDANCE_DT) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR'))
                                  GROUP BY A.EMPLOYEE_ID, A.ATTENDANCE_TIME
                                  ORDER BY A.EMPLOYEE_ID DESC
                      ) EOD ON EOD.SP_CODE = DLU.SP_CODE
                      LEFT JOIN DIST_ROUTE_DETAIL DRD ON DRD.EMP_CODE = DLU.SP_CODE AND DRD.COMPANY_CODE = DLU.COMPANY_CODE AND DRD.DELETED_FLAG = 'N' AND TRUNC(DRD.ASSIGN_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR')
                      LEFT JOIN DIST_ROUTE_ENTITY DRE ON DRE.ROUTE_CODE = DRD.ROUTE_CODE AND DRE.COMPANY_CODE = DRD.COMPANY_CODE AND DRE.ENTITY_TYPE = 'P' AND DRE.DELETED_FLAG = 'N'
                      LEFT JOIN IP_PARTY_TYPE_CODE PTC ON PTC.PARTY_TYPE_CODE = DRE.ENTITY_CODE AND PTC.COMPANY_CODE = DRE.COMPANY_CODE
                      GROUP BY DLU.SP_CODE, TRIM(HES.EMPLOYEE_EDESC), TRIM(DGM.GROUP_EDESC), PTC.PARTY_TYPE_CODE, TRIM(PTC.PARTY_TYPE_EDESC), DLU.COMPANY_CODE, API.ATN_IMAGE, ATN.ATN_DATE, ATN.ATN_LATITUDE, ATN.ATN_LONGITUDE, EOD.EOD_DATE, EOD.EOD_LATITUDE, EOD.EOD_LONGITUDE
                  ) ENT
                  GROUP BY ENT.GROUP_EDESC, ENT.SP_CODE, ENT.EMPLOYEE_EDESC, ENT.COMPANY_CODE, ENT.ATN_IMAGE, ENT.ATN_DATE, ENT.ATN_LATITUDE, ENT.ATN_LONGITUDE, ENT.EOD_DATE, ENT.EOD_LATITUDE, ENT.EOD_LONGITUDE
                  ) PETBL -- Party Type Entity Table
            LEFT JOIN (SELECT ENT.SP_CODE, ENT.EMPLOYEE_EDESC, ENT.COMPANY_CODE, COUNT(ENT.ENTITY_CODE) VISITED
                        FROM (
                            SELECT DLU.SP_CODE, TRIM(HES.EMPLOYEE_EDESC) EMPLOYEE_EDESC, PTC.PARTY_TYPE_CODE ENTITY_CODE, TRIM(PTC.PARTY_TYPE_EDESC) ENTITY_NAME, DLU.COMPANY_CODE
                            FROM DIST_LOGIN_USER DLU
                            INNER JOIN HR_EMPLOYEE_SETUP HES ON HES.EMPLOYEE_CODE = DLU.SP_CODE AND HES.COMPANY_CODE = DLU.COMPANY_CODE
                            LEFT JOIN DIST_ROUTE_DETAIL DRD ON DRD.EMP_CODE = DLU.SP_CODE AND DRD.COMPANY_CODE = DLU.COMPANY_CODE AND DRD.DELETED_FLAG = 'N' AND TRUNC(DRD.ASSIGN_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR')
                            LEFT JOIN DIST_ROUTE_ENTITY DRE ON DRE.ROUTE_CODE = DRD.ROUTE_CODE AND DRE.COMPANY_CODE = DRD.COMPANY_CODE AND DRE.ENTITY_TYPE = 'P' AND DRE.DELETED_FLAG = 'N'
                            LEFT JOIN IP_PARTY_TYPE_CODE PTC ON PTC.PARTY_TYPE_CODE = DRE.ENTITY_CODE AND PTC.COMPANY_CODE = DRE.COMPANY_CODE
                            GROUP BY DLU.SP_CODE, TRIM(HES.EMPLOYEE_EDESC), PTC.PARTY_TYPE_CODE, TRIM(PTC.PARTY_TYPE_EDESC), DLU.COMPANY_CODE
                        ) ENT
                        INNER JOIN (SELECT SP_CODE, CUSTOMER_CODE, COMPANY_CODE FROM DIST_LOCATION_TRACK WHERE TRUNC(UPDATE_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR') GROUP BY SP_CODE, CUSTOMER_CODE, COMPANY_CODE) DLT ON DLT.SP_CODE = ENT.SP_CODE AND DLT.CUSTOMER_CODE = ENT.ENTITY_CODE AND DLT.COMPANY_CODE = ENT.COMPANY_CODE
                        WHERE DLT.SP_CODE IS NOT NULL
                        GROUP BY ENT.SP_CODE, ENT.EMPLOYEE_EDESC, ENT.COMPANY_CODE
                  ) PVTBL -- Party Type Visit Table
                  ON PVTBL.SP_CODE = PETBL.SP_CODE AND PVTBL.COMPANY_CODE = PETBL.COMPANY_CODE
            LEFT JOIN (SELECT ENT.SP_CODE, ENT.EMPLOYEE_EDESC, ENT.COMPANY_CODE, COUNT(ENT.ENTITY_CODE) NOT_VISITED
                      FROM (
                          SELECT DLU.SP_CODE, TRIM(HES.EMPLOYEE_EDESC) EMPLOYEE_EDESC, SCS.PARTY_TYPE_CODE ENTITY_CODE, TRIM(SCS.PARTY_TYPE_EDESC) ENTITY_NAME, DLU.COMPANY_CODE
                          FROM DIST_LOGIN_USER DLU
                          INNER JOIN HR_EMPLOYEE_SETUP HES ON HES.EMPLOYEE_CODE = DLU.SP_CODE AND HES.COMPANY_CODE = DLU.COMPANY_CODE
                          LEFT JOIN DIST_ROUTE_DETAIL DRD ON DRD.EMP_CODE = DLU.SP_CODE AND DRD.COMPANY_CODE = DLU.COMPANY_CODE AND DRD.DELETED_FLAG = 'N' AND TRUNC(DRD.ASSIGN_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR')
                          LEFT JOIN DIST_ROUTE_ENTITY DRE ON DRE.ROUTE_CODE = DRD.ROUTE_CODE AND DRE.COMPANY_CODE = DRD.COMPANY_CODE AND DRE.ENTITY_TYPE = 'P' AND DRE.DELETED_FLAG = 'N'
                          LEFT JOIN IP_PARTY_TYPE_CODE SCS ON SCS.PARTY_TYPE_CODE = DRE.ENTITY_CODE AND SCS.COMPANY_CODE = DRE.COMPANY_CODE 
                          GROUP BY DLU.SP_CODE, TRIM(HES.EMPLOYEE_EDESC), SCS.PARTY_TYPE_CODE, TRIM(SCS.PARTY_TYPE_EDESC), DLU.COMPANY_CODE
                      ) ENT
                      LEFT JOIN (SELECT SP_CODE, CUSTOMER_CODE, COMPANY_CODE FROM DIST_LOCATION_TRACK WHERE TRUNC(UPDATE_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR') GROUP BY SP_CODE, CUSTOMER_CODE, COMPANY_CODE) DLT ON DLT.SP_CODE = ENT.SP_CODE AND DLT.CUSTOMER_CODE = ENT.ENTITY_CODE AND DLT.COMPANY_CODE = ENT.COMPANY_CODE
                      WHERE DLT.SP_CODE IS NULL
                      GROUP BY ENT.SP_CODE, ENT.EMPLOYEE_EDESC, ENT.COMPANY_CODE
                  ) PNVTBL -- Party Type Not Visited Table
                  ON PNVTBL.SP_CODE = PETBL.SP_CODE AND PNVTBL.COMPANY_CODE = PETBL.COMPANY_CODE
            LEFT JOIN (SELECT PJPENT.SP_CODE, PJPENT.EMPLOYEE_EDESC, PJPENT.COMPANY_CODE, COUNT(PPO.PARTY_TYPE_CODE) PJP_PRODUCTIVE, SUM(PPO.TOTAL_QUANTITY) TOTAL_QUANTITY, SUM(PPO.TOTAL_PRICE) TOTAL_PRICE
                      FROM (SELECT DLU.USERID, DLU.SP_CODE, TRIM(HES.EMPLOYEE_EDESC) EMPLOYEE_EDESC, PTC.PARTY_TYPE_CODE ENTITY_CODE, TRIM(PTC.PARTY_TYPE_EDESC) ENTITY_NAME, DLU.COMPANY_CODE
                            FROM DIST_LOGIN_USER DLU
                            INNER JOIN HR_EMPLOYEE_SETUP HES ON HES.EMPLOYEE_CODE = DLU.SP_CODE AND HES.COMPANY_CODE = DLU.COMPANY_CODE
                            LEFT JOIN DIST_ROUTE_DETAIL DRD ON DRD.EMP_CODE = DLU.SP_CODE AND DRD.COMPANY_CODE = DLU.COMPANY_CODE AND DRD.DELETED_FLAG = 'N' AND TRUNC(DRD.ASSIGN_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR')
                            LEFT JOIN DIST_ROUTE_ENTITY DRE ON DRE.ROUTE_CODE = DRD.ROUTE_CODE AND DRE.COMPANY_CODE = DRD.COMPANY_CODE AND DRE.ENTITY_TYPE = 'P' AND DRE.DELETED_FLAG = 'N'
                            LEFT JOIN IP_PARTY_TYPE_CODE PTC ON PTC.PARTY_TYPE_CODE = DRE.ENTITY_CODE AND PTC.COMPANY_CODE = DRE.COMPANY_CODE
                            GROUP BY DLU.USERID, DLU.SP_CODE, TRIM(HES.EMPLOYEE_EDESC), PTC.PARTY_TYPE_CODE, TRIM(PTC.PARTY_TYPE_EDESC), DLU.COMPANY_CODE
                          ) PJPENT
                      LEFT JOIN (SELECT CREATED_BY, PARTY_TYPE_CODE, COMPANY_CODE, COUNT(PARTY_TYPE_CODE) TOTAL_ORDER, SUM(QUANTITY) TOTAL_QUANTITY, SUM(TOTAL_PRICE) TOTAL_PRICE
                                  FROM DIST_IP_SSD_PURCHASE_ORDER
                                  WHERE TRUNC(ORDER_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR')
                                  GROUP BY CREATED_BY, PARTY_TYPE_CODE, COMPANY_CODE
                                ) PPO ON PPO.CREATED_BY = PJPENT.USERID AND PPO.PARTY_TYPE_CODE = PJPENT.ENTITY_CODE AND PPO.COMPANY_CODE = PJPENT.COMPANY_CODE
                      GROUP BY PJPENT.SP_CODE, PJPENT.EMPLOYEE_EDESC, PJPENT.COMPANY_CODE
                   ) PPJPTBL -- Party Type PJP Table
                   ON PPJPTBL.SP_CODE = PETBL.SP_CODE AND PPJPTBL.COMPANY_CODE = PETBL.COMPANY_CODE
            LEFT JOIN (SELECT PPO.SP_CODE, PPO.EMPLOYEE_EDESC, PPO.COMPANY_CODE, COUNT(PPO.PARTY_TYPE_CODE) NPJP_PRODUCTIVE, SUM(PPO.TOTAL_QUANTITY) TOTAL_QUANTITY, SUM(PPO.TOTAL_PRICE) TOTAL_PRICE
                      FROM (SELECT A.CREATED_BY, B.SP_CODE, A.PARTY_TYPE_CODE, TRIM(C.EMPLOYEE_EDESC) EMPLOYEE_EDESC, A.COMPANY_CODE, COUNT(A.PARTY_TYPE_CODE) TOTAL_ORDER, SUM(A.QUANTITY) TOTAL_QUANTITY, SUM(A.TOTAL_PRICE) TOTAL_PRICE
                            FROM DIST_IP_SSD_PURCHASE_ORDER A
                            INNER JOIN DIST_LOGIN_USER B ON B.USERID = A.CREATED_BY AND B.COMPANY_CODE = A.COMPANY_CODE
                            INNER JOIN HR_EMPLOYEE_SETUP C ON C.EMPLOYEE_CODE = B.SP_CODE AND C.COMPANY_CODE = A.COMPANY_CODE
                            WHERE TRUNC(A.ORDER_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR')
                            GROUP BY A.CREATED_BY, B.SP_CODE, A.PARTY_TYPE_CODE, TRIM(C.EMPLOYEE_EDESC), A.COMPANY_CODE
                      ) PPO
                      LEFT JOIN (SELECT DLU.USERID, DLU.SP_CODE, TRIM(HES.EMPLOYEE_EDESC) EMPLOYEE_EDESC, PTC.PARTY_TYPE_CODE ENTITY_CODE, TRIM(PTC.PARTY_TYPE_EDESC) ENTITY_NAME, DLU.COMPANY_CODE
                                  FROM DIST_LOGIN_USER DLU
                                  INNER JOIN HR_EMPLOYEE_SETUP HES ON HES.EMPLOYEE_CODE = DLU.SP_CODE AND HES.COMPANY_CODE = DLU.COMPANY_CODE
                                  LEFT JOIN DIST_ROUTE_DETAIL DRD ON DRD.EMP_CODE = DLU.SP_CODE AND DRD.COMPANY_CODE = DLU.COMPANY_CODE AND DRD.DELETED_FLAG = 'N' AND TRUNC(DRD.ASSIGN_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR')
                                  LEFT JOIN DIST_ROUTE_ENTITY DRE ON DRE.ROUTE_CODE = DRD.ROUTE_CODE AND DRE.COMPANY_CODE = DRD.COMPANY_CODE AND DRE.ENTITY_TYPE = 'P' AND DRE.DELETED_FLAG = 'N'
                                  LEFT JOIN IP_PARTY_TYPE_CODE PTC ON PTC.PARTY_TYPE_CODE = DRE.ENTITY_CODE AND PTC.COMPANY_CODE = DRE.COMPANY_CODE
                                  GROUP BY DLU.USERID, DLU.SP_CODE, TRIM(HES.EMPLOYEE_EDESC), PTC.PARTY_TYPE_CODE, TRIM(PTC.PARTY_TYPE_EDESC), DLU.COMPANY_CODE
                      ) NPJPENT ON NPJPENT.USERID = PPO.CREATED_BY AND NPJPENT.SP_CODE = PPO.SP_CODE AND NPJPENT.ENTITY_CODE = PPO.PARTY_TYPE_CODE AND NPJPENT.COMPANY_CODE = PPO.COMPANY_CODE
                      WHERE 1 = 1
                        AND NPJPENT.SP_CODE IS NULL
                      GROUP BY PPO.SP_CODE, PPO.EMPLOYEE_EDESC, PPO.COMPANY_CODE
                   ) PNPJPTBL -- Party Type PJP Table
                   ON PNPJPTBL.SP_CODE = PETBL.SP_CODE AND PNPJPTBL.COMPANY_CODE = PETBL.COMPANY_CODE
      ) PTBL
      UNION ALL
      SELECT DTBL.*
      FROM (SELECT DETBL.GROUP_EDESC, DETBL.SP_CODE, DETBL.EMPLOYEE_EDESC, DETBL.COMPANY_CODE, DETBL.ATN_IMAGE, DETBL.ATN_DATE, DETBL.ATN_LATITUDE, DETBL.ATN_LONGITUDE, DETBL.EOD_DATE, DETBL.EOD_LATITUDE, DETBL.EOD_LONGITUDE, NVL(DETBL.TARGET, 0) TARGET, 
            NVL(DVTBL.VISITED, 0) VISITED, NVL(DNVTBL.NOT_VISITED,0) NOT_VISITED, 
            NVL(DPJPTBL.PJP_PRODUCTIVE,0) PJP_PRODUCTIVE,
            (NVL(DVTBL.VISITED, 0) - NVL(DPJPTBL.PJP_PRODUCTIVE,0)) PJP_NON_PRODUCTIVE,
            NVL(DNPJPTBL.NPJP_PRODUCTIVE,0) NPJP_PRODUCTIVE,
            NVL(DECODE(DPJPTBL.PJP_PRODUCTIVE, NULL, 0,
                                             0, 0,
                                             ROUND((DPJPTBL.PJP_PRODUCTIVE / DVTBL.VISITED) * 100,2)),0) NET_PERCENT_EFFECTIVE_CALLS,
            0 OUTLET_ADDED,
            NVL(DPJPTBL.TOTAL_QUANTITY,0) TOTAL_QUANTITY, NVL(DPJPTBL.TOTAL_PRICE,0) TOTAL_AMOUNT
            FROM (SELECT ENT.GROUP_EDESC, ENT.SP_CODE, ENT.EMPLOYEE_EDESC, ENT.COMPANY_CODE, ENT.ATN_IMAGE, ENT.ATN_DATE, ENT.ATN_LATITUDE, ENT.ATN_LONGITUDE, ENT.EOD_DATE, ENT.EOD_LATITUDE, ENT.EOD_LONGITUDE, COUNT(ENT.ENTITY_CODE) TARGET
                  FROM (
                      SELECT DLU.SP_CODE, TRIM(HES.EMPLOYEE_EDESC) EMPLOYEE_EDESC, TRIM(DGM.GROUP_EDESC) GROUP_EDESC, SCS.CUSTOMER_CODE ENTITY_CODE, TRIM(SCS.CUSTOMER_EDESC) ENTITY_NAME, DLU.COMPANY_CODE, API.ATN_IMAGE, ATN.ATN_DATE, ATN.ATN_LATITUDE, ATN.ATN_LONGITUDE, EOD.EOD_DATE, EOD.EOD_LATITUDE, EOD.EOD_LONGITUDE
                      FROM DIST_LOGIN_USER DLU
                      INNER JOIN HR_EMPLOYEE_SETUP HES ON HES.EMPLOYEE_CODE = DLU.SP_CODE AND HES.COMPANY_CODE = DLU.COMPANY_CODE
                      LEFT JOIN DIST_GROUP_MASTER DGM ON DGM.GROUPID = DLU.GROUPID AND DGM.COMPANY_CODE = DGM.COMPANY_CODE
                      LEFT JOIN (SELECT A.ENTITY_CODE SP_CODE, A.COMPANY_CODE, A.CREATE_DATE, A.FILENAME ATN_IMAGE
                                  FROM DIST_PHOTO_INFO A
                                  WHERE A.CREATE_DATE = (SELECT MAX(CREATE_DATE) FROM DIST_PHOTO_INFO WHERE ENTITY_CODE = A.ENTITY_CODE AND ENTITY_TYPE = 'S' AND (MEDIA_TYPE = 'ATN' OR CATEGORYID = 1) AND TRUNC(CREATE_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR'))
                                  GROUP BY A.ENTITY_CODE, A.COMPANY_CODE, A.CREATE_DATE, A.FILENAME
                                  ORDER BY A.ENTITY_CODE DESC
                      ) API ON API.SP_CODE = DLU.SP_CODE AND API.COMPANY_CODE = DLU.COMPANY_CODE
                      LEFT JOIN (SELECT A.EMPLOYEE_ID SP_CODE, A.ATTENDANCE_TIME ATN_DATE,''ATN_LATITUDE, '' ATN_LONGITUDE
                                  FROM HRIS_ATTENDANCE A
                                  WHERE A.ATTENDANCE_TIME = (SELECT MIN(ATTENDANCE_TIME) FROM HRIS_ATTENDANCE WHERE EMPLOYEE_ID = A.EMPLOYEE_ID AND TRUNC(ATTENDANCE_DT) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR'))
                                  GROUP BY A.EMPLOYEE_ID, A.ATTENDANCE_TIME
                                  ORDER BY A.EMPLOYEE_ID DESC
                      ) ATN ON ATN.SP_CODE = DLU.SP_CODE
                      LEFT JOIN (SELECT A.EMPLOYEE_ID SP_CODE, A.ATTENDANCE_TIME EOD_DATE, '' EOD_LATITUDE,'' EOD_LONGITUDE
                                  FROM HRIS_ATTENDANCE A
                                  WHERE A.ATTENDANCE_TIME = (SELECT MAX(ATTENDANCE_TIME) FROM HRIS_ATTENDANCE WHERE EMPLOYEE_ID = A.EMPLOYEE_ID AND TRUNC(ATTENDANCE_DT) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR'))
                                  GROUP BY A.EMPLOYEE_ID, A.ATTENDANCE_TIME
                                  ORDER BY A.EMPLOYEE_ID DESC
                      ) EOD ON EOD.SP_CODE = DLU.SP_CODE
                      LEFT JOIN DIST_ROUTE_DETAIL DRD ON DRD.EMP_CODE = DLU.SP_CODE AND DRD.COMPANY_CODE = DLU.COMPANY_CODE AND DRD.DELETED_FLAG = 'N' AND TRUNC(DRD.ASSIGN_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR')
                      LEFT JOIN DIST_ROUTE_ENTITY DRE ON DRE.ROUTE_CODE = DRD.ROUTE_CODE AND DRE.COMPANY_CODE = DRD.COMPANY_CODE AND DRE.ENTITY_TYPE = 'D' AND DRE.DELETED_FLAG = 'N'
                      LEFT JOIN SA_CUSTOMER_SETUP SCS ON SCS.CUSTOMER_CODE = DRE.ENTITY_CODE AND SCS.COMPANY_CODE = DRE.COMPANY_CODE
                      GROUP BY DLU.SP_CODE, TRIM(HES.EMPLOYEE_EDESC), TRIM(DGM.GROUP_EDESC), SCS.CUSTOMER_CODE, TRIM(SCS.CUSTOMER_EDESC), DLU.COMPANY_CODE, API.ATN_IMAGE, ATN.ATN_DATE, ATN.ATN_LATITUDE, ATN.ATN_LONGITUDE, EOD.EOD_DATE, EOD.EOD_LATITUDE, EOD.EOD_LONGITUDE
                  ) ENT
                  GROUP BY ENT.GROUP_EDESC, ENT.SP_CODE, ENT.EMPLOYEE_EDESC, ENT.COMPANY_CODE, ENT.ATN_IMAGE, ENT.ATN_DATE, ENT.ATN_LATITUDE, ENT.ATN_LONGITUDE, ENT.EOD_DATE, ENT.EOD_LATITUDE, ENT.EOD_LONGITUDE
                  ) DETBL -- Customer/Distributor Entity Table
            LEFT JOIN (SELECT ENT.SP_CODE, ENT.EMPLOYEE_EDESC, ENT.COMPANY_CODE, COUNT(ENT.ENTITY_CODE) VISITED
                        FROM (
                            SELECT DLU.SP_CODE, TRIM(HES.EMPLOYEE_EDESC) EMPLOYEE_EDESC, SCS.CUSTOMER_CODE ENTITY_CODE, TRIM(SCS.CUSTOMER_EDESC) ENTITY_NAME, DLU.COMPANY_CODE
                            FROM DIST_LOGIN_USER DLU
                            INNER JOIN HR_EMPLOYEE_SETUP HES ON HES.EMPLOYEE_CODE = DLU.SP_CODE AND HES.COMPANY_CODE = DLU.COMPANY_CODE
                            LEFT JOIN DIST_ROUTE_DETAIL DRD ON DRD.EMP_CODE = DLU.SP_CODE AND DRD.COMPANY_CODE = DLU.COMPANY_CODE AND DRD.DELETED_FLAG = 'N' AND TRUNC(DRD.ASSIGN_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR')
                            LEFT JOIN DIST_ROUTE_ENTITY DRE ON DRE.ROUTE_CODE = DRD.ROUTE_CODE AND DRE.COMPANY_CODE = DRD.COMPANY_CODE AND DRE.ENTITY_TYPE = 'D' AND DRE.DELETED_FLAG = 'N'
                            LEFT JOIN SA_CUSTOMER_SETUP SCS ON SCS.CUSTOMER_CODE = DRE.ENTITY_CODE AND SCS.COMPANY_CODE = DRE.COMPANY_CODE
                            GROUP BY DLU.SP_CODE, TRIM(HES.EMPLOYEE_EDESC), SCS.CUSTOMER_CODE, TRIM(SCS.CUSTOMER_EDESC), DLU.COMPANY_CODE
                        ) ENT
                        INNER JOIN (SELECT SP_CODE, CUSTOMER_CODE, COMPANY_CODE FROM DIST_LOCATION_TRACK WHERE TRUNC(UPDATE_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR') GROUP BY SP_CODE, CUSTOMER_CODE, COMPANY_CODE) DLT ON DLT.SP_CODE = ENT.SP_CODE AND DLT.CUSTOMER_CODE = ENT.ENTITY_CODE AND DLT.COMPANY_CODE = ENT.COMPANY_CODE
                        WHERE DLT.SP_CODE IS NOT NULL
                        GROUP BY ENT.SP_CODE, ENT.EMPLOYEE_EDESC, ENT.COMPANY_CODE
                  ) DVTBL -- Customer/Distributor Visit Table
                  ON DVTBL.SP_CODE = DETBL.SP_CODE AND DVTBL.COMPANY_CODE = DETBL.COMPANY_CODE
            LEFT JOIN (SELECT ENT.SP_CODE, ENT.EMPLOYEE_EDESC, ENT.COMPANY_CODE, COUNT(ENT.ENTITY_CODE) NOT_VISITED
                      FROM (
                          SELECT DLU.SP_CODE, TRIM(HES.EMPLOYEE_EDESC) EMPLOYEE_EDESC, SCS.CUSTOMER_CODE ENTITY_CODE, TRIM(SCS.CUSTOMER_EDESC) ENTITY_NAME, DLU.COMPANY_CODE
                          FROM DIST_LOGIN_USER DLU
                          INNER JOIN HR_EMPLOYEE_SETUP HES ON HES.EMPLOYEE_CODE = DLU.SP_CODE AND HES.COMPANY_CODE = DLU.COMPANY_CODE
                          LEFT JOIN DIST_ROUTE_DETAIL DRD ON DRD.EMP_CODE = DLU.SP_CODE AND DRD.COMPANY_CODE = DLU.COMPANY_CODE AND DRD.DELETED_FLAG = 'N' AND TRUNC(DRD.ASSIGN_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR')
                          LEFT JOIN DIST_ROUTE_ENTITY DRE ON DRE.ROUTE_CODE = DRD.ROUTE_CODE AND DRE.COMPANY_CODE = DRD.COMPANY_CODE AND DRE.ENTITY_TYPE = 'D' AND DRE.DELETED_FLAG = 'N'
                          LEFT JOIN SA_CUSTOMER_SETUP SCS ON SCS.CUSTOMER_CODE = DRE.ENTITY_CODE AND SCS.COMPANY_CODE = DRE.COMPANY_CODE 
                          GROUP BY DLU.SP_CODE, TRIM(HES.EMPLOYEE_EDESC), SCS.CUSTOMER_CODE, TRIM(SCS.CUSTOMER_EDESC), DLU.COMPANY_CODE
                      ) ENT
                      LEFT JOIN (SELECT SP_CODE, CUSTOMER_CODE, COMPANY_CODE FROM DIST_LOCATION_TRACK WHERE TRUNC(UPDATE_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR') GROUP BY SP_CODE, CUSTOMER_CODE, COMPANY_CODE) DLT ON DLT.SP_CODE = ENT.SP_CODE AND DLT.CUSTOMER_CODE = ENT.ENTITY_CODE AND DLT.COMPANY_CODE = ENT.COMPANY_CODE
                      WHERE DLT.SP_CODE IS NULL
                      GROUP BY ENT.SP_CODE, ENT.EMPLOYEE_EDESC, ENT.COMPANY_CODE
                ) DNVTBL -- Customer/Distributor Not Visited Table
                ON DNVTBL.SP_CODE = DETBL.SP_CODE AND DNVTBL.COMPANY_CODE = DETBL.COMPANY_CODE
            LEFT JOIN (SELECT PJPENT.SP_CODE, PJPENT.EMPLOYEE_EDESC, PJPENT.COMPANY_CODE, COUNT(DPO.CUSTOMER_CODE) PJP_PRODUCTIVE, SUM(DPO.TOTAL_QUANTITY) TOTAL_QUANTITY, SUM(DPO.TOTAL_PRICE) TOTAL_PRICE
                      FROM (SELECT DLU.USERID, DLU.SP_CODE, TRIM(HES.EMPLOYEE_EDESC) EMPLOYEE_EDESC, SCS.CUSTOMER_CODE ENTITY_CODE, TRIM(SCS.CUSTOMER_EDESC) ENTITY_NAME, DLU.COMPANY_CODE
                            FROM DIST_LOGIN_USER DLU
                            INNER JOIN HR_EMPLOYEE_SETUP HES ON HES.EMPLOYEE_CODE = DLU.SP_CODE AND HES.COMPANY_CODE = DLU.COMPANY_CODE
                            LEFT JOIN DIST_ROUTE_DETAIL DRD ON DRD.EMP_CODE = DLU.SP_CODE AND DRD.COMPANY_CODE = DLU.COMPANY_CODE AND DRD.DELETED_FLAG = 'N' AND TRUNC(DRD.ASSIGN_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR')
                            LEFT JOIN DIST_ROUTE_ENTITY DRE ON DRE.ROUTE_CODE = DRD.ROUTE_CODE AND DRE.COMPANY_CODE = DRD.COMPANY_CODE AND DRE.ENTITY_TYPE = 'D' AND DRE.DELETED_FLAG = 'N'
                            LEFT JOIN SA_CUSTOMER_SETUP SCS ON SCS.CUSTOMER_CODE = DRE.ENTITY_CODE AND SCS.COMPANY_CODE = DRE.COMPANY_CODE
                            GROUP BY DLU.USERID, DLU.SP_CODE, TRIM(HES.EMPLOYEE_EDESC), SCS.CUSTOMER_CODE, TRIM(SCS.CUSTOMER_EDESC), DLU.COMPANY_CODE
                      ) PJPENT
                      LEFT JOIN (SELECT CREATED_BY, CUSTOMER_CODE, COMPANY_CODE, COUNT(CUSTOMER_CODE) TOTAL_ORDER, SUM(QUANTITY) TOTAL_QUANTITY, SUM(TOTAL_PRICE) TOTAL_PRICE
                                  FROM DIST_IP_SSD_PURCHASE_ORDER
                                  WHERE TRUNC(ORDER_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR')
                                  GROUP BY CREATED_BY, CUSTOMER_CODE, COMPANY_CODE
                      ) DPO ON DPO.CREATED_BY = PJPENT.USERID AND DPO.CUSTOMER_CODE = PJPENT.ENTITY_CODE AND DPO.COMPANY_CODE = PJPENT.COMPANY_CODE
                      GROUP BY PJPENT.SP_CODE, PJPENT.EMPLOYEE_EDESC, PJPENT.COMPANY_CODE
                   ) DPJPTBL -- Customer/Distributor PJP Table
                   ON DPJPTBL.SP_CODE = DETBL.SP_CODE AND DPJPTBL.COMPANY_CODE = DETBL.COMPANY_CODE
            LEFT JOIN (SELECT DPO.SP_CODE, DPO.EMPLOYEE_EDESC, DPO.COMPANY_CODE, COUNT(DPO.CUSTOMER_CODE) NPJP_PRODUCTIVE, SUM(DPO.TOTAL_QUANTITY) TOTAL_QUANTITY, SUM(DPO.TOTAL_PRICE) TOTAL_PRICE
                      FROM (SELECT A.CREATED_BY, B.SP_CODE, A.CUSTOMER_CODE, TRIM(C.EMPLOYEE_EDESC) EMPLOYEE_EDESC, A.COMPANY_CODE, COUNT(A.CUSTOMER_CODE) TOTAL_ORDER, SUM(A.QUANTITY) TOTAL_QUANTITY, SUM(A.TOTAL_PRICE) TOTAL_PRICE
                            FROM DIST_IP_SSD_PURCHASE_ORDER A
                            INNER JOIN DIST_LOGIN_USER B ON B.USERID = A.CREATED_BY AND B.COMPANY_CODE = A.COMPANY_CODE
                            INNER JOIN HR_EMPLOYEE_SETUP C ON C.EMPLOYEE_CODE = B.SP_CODE AND C.COMPANY_CODE = A.COMPANY_CODE
                            WHERE TRUNC(A.ORDER_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR')
                            GROUP BY A.CREATED_BY, B.SP_CODE, A.CUSTOMER_CODE, TRIM(C.EMPLOYEE_EDESC), A.COMPANY_CODE
                      ) DPO
                      LEFT JOIN (SELECT DLU.USERID, DLU.SP_CODE, TRIM(HES.EMPLOYEE_EDESC) EMPLOYEE_EDESC, SCS.CUSTOMER_CODE ENTITY_CODE, TRIM(SCS.CUSTOMER_EDESC) ENTITY_NAME, DLU.COMPANY_CODE
                                  FROM DIST_LOGIN_USER DLU
                                  INNER JOIN HR_EMPLOYEE_SETUP HES ON HES.EMPLOYEE_CODE = DLU.SP_CODE AND HES.COMPANY_CODE = DLU.COMPANY_CODE
                                  LEFT JOIN DIST_ROUTE_DETAIL DRD ON DRD.EMP_CODE = DLU.SP_CODE AND DRD.COMPANY_CODE = DLU.COMPANY_CODE AND DRD.DELETED_FLAG = 'N' AND TRUNC(DRD.ASSIGN_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR')
                                  LEFT JOIN DIST_ROUTE_ENTITY DRE ON DRE.ROUTE_CODE = DRD.ROUTE_CODE AND DRE.COMPANY_CODE = DRD.COMPANY_CODE AND DRE.ENTITY_TYPE = 'D' AND DRE.DELETED_FLAG = 'N'
                                  LEFT JOIN SA_CUSTOMER_SETUP SCS ON SCS.CUSTOMER_CODE = DRE.ENTITY_CODE AND SCS.COMPANY_CODE = DRE.COMPANY_CODE
                                  GROUP BY DLU.USERID, DLU.SP_CODE, TRIM(HES.EMPLOYEE_EDESC), SCS.CUSTOMER_CODE, TRIM(SCS.CUSTOMER_EDESC), DLU.COMPANY_CODE
                      ) NPJPENT ON NPJPENT.USERID = DPO.CREATED_BY AND NPJPENT.SP_CODE = DPO.SP_CODE AND NPJPENT.ENTITY_CODE = DPO.CUSTOMER_CODE AND NPJPENT.COMPANY_CODE = DPO.COMPANY_CODE
                      WHERE 1 = 1
                        AND NPJPENT.SP_CODE IS NULL
                      GROUP BY DPO.SP_CODE, DPO.EMPLOYEE_EDESC, DPO.COMPANY_CODE
                   ) DNPJPTBL -- Customer/Distributor NPJP Table
                   ON DNPJPTBL.SP_CODE = DETBL.SP_CODE AND DNPJPTBL.COMPANY_CODE = DETBL.COMPANY_CODE
      ) DTBL
      UNION ALL
      SELECT RTBL.*
      FROM (SELECT RETBL.GROUP_EDESC, RETBL.SP_CODE, RETBL.EMPLOYEE_EDESC, RETBL.COMPANY_CODE, RETBL.ATN_IMAGE, RETBL.ATN_DATE, RETBL.ATN_LATITUDE, RETBL.ATN_LONGITUDE, RETBL.EOD_DATE, RETBL.EOD_LATITUDE, RETBL.EOD_LONGITUDE, NVL(RETBL.TARGET, 0) TARGET, 
            NVL(RVTBL.VISITED, 0) VISITED, NVL(RNVTBL.NOT_VISITED,0) NOT_VISITED, 
            NVL(RPJPTBL.PJP_PRODUCTIVE,0) PJP_PRODUCTIVE,
            (NVL(RVTBL.VISITED, 0) - NVL(RPJPTBL.PJP_PRODUCTIVE,0)) PJP_NON_PRODUCTIVE,
            NVL(RNPJPTBL.NPJP_PRODUCTIVE,0) NPJP_PRODUCTIVE,
            NVL(DECODE(RPJPTBL.PJP_PRODUCTIVE, NULL, 0,
                                             0, 0,
                                             ROUND((RPJPTBL.PJP_PRODUCTIVE / RVTBL.VISITED) * 100,2)),0) NET_PERCENT_EFFECTIVE_CALLS,
            NVL(ROUT.OUTLET_ADDED, 0) OUTLET_ADDED,
            NVL(RPJPTBL.TOTAL_QUANTITY,0) TOTAL_QUANTITY, NVL(RPJPTBL.TOTAL_PRICE,0) TOTAL_AMOUNT
            FROM (SELECT ENT.GROUP_EDESC, ENT.SP_CODE, ENT.EMPLOYEE_EDESC, ENT.COMPANY_CODE, ENT.ATN_IMAGE, ENT.ATN_DATE, ENT.ATN_LATITUDE, ENT.ATN_LONGITUDE, ENT.EOD_DATE, ENT.EOD_LATITUDE, ENT.EOD_LONGITUDE, COUNT(ENT.ENTITY_CODE) TARGET
                  FROM (
                      SELECT DLU.SP_CODE, TRIM(HES.EMPLOYEE_EDESC) EMPLOYEE_EDESC, TRIM(DGM.GROUP_EDESC) GROUP_EDESC, DRM.RESELLER_CODE ENTITY_CODE, TRIM(DRM.RESELLER_NAME) ENTITY_NAME, DLU.COMPANY_CODE, API.ATN_IMAGE, ATN.ATN_DATE, ATN.ATN_LATITUDE, ATN.ATN_LONGITUDE, EOD.EOD_DATE, EOD.EOD_LATITUDE, EOD.EOD_LONGITUDE
                      FROM DIST_LOGIN_USER DLU
                      INNER JOIN HR_EMPLOYEE_SETUP HES ON HES.EMPLOYEE_CODE = DLU.SP_CODE AND HES.COMPANY_CODE = DLU.COMPANY_CODE
                      LEFT JOIN DIST_GROUP_MASTER DGM ON DGM.GROUPID = DLU.GROUPID AND DGM.COMPANY_CODE = DGM.COMPANY_CODE
                      LEFT JOIN (SELECT A.ENTITY_CODE SP_CODE, A.COMPANY_CODE, A.CREATE_DATE, A.FILENAME ATN_IMAGE
                                  FROM DIST_PHOTO_INFO A
                                  WHERE A.CREATE_DATE = (SELECT MAX(CREATE_DATE) FROM DIST_PHOTO_INFO WHERE ENTITY_CODE = A.ENTITY_CODE AND ENTITY_TYPE = 'S' AND (MEDIA_TYPE = 'ATN' OR CATEGORYID = 1) AND TRUNC(CREATE_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR'))
                                  GROUP BY A.ENTITY_CODE, A.COMPANY_CODE, A.CREATE_DATE, A.FILENAME
                                  ORDER BY A.ENTITY_CODE DESC
                      ) API ON API.SP_CODE = DLU.SP_CODE AND API.COMPANY_CODE = DLU.COMPANY_CODE
                      LEFT JOIN (SELECT A.EMPLOYEE_ID SP_CODE, A.ATTENDANCE_TIME ATN_DATE,''ATN_LATITUDE, '' ATN_LONGITUDE
                                  FROM HRIS_ATTENDANCE A
                                  WHERE A.ATTENDANCE_TIME = (SELECT MIN(ATTENDANCE_TIME) FROM HRIS_ATTENDANCE WHERE EMPLOYEE_ID = A.EMPLOYEE_ID AND TRUNC(ATTENDANCE_DT) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR'))
                                  GROUP BY A.EMPLOYEE_ID, A.ATTENDANCE_TIME
                                  ORDER BY A.EMPLOYEE_ID DESC
                      ) ATN ON ATN.SP_CODE = DLU.SP_CODE
                      LEFT JOIN (SELECT A.EMPLOYEE_ID SP_CODE, A.ATTENDANCE_TIME EOD_DATE, '' EOD_LATITUDE,'' EOD_LONGITUDE
                                  FROM HRIS_ATTENDANCE A
                                  WHERE A.ATTENDANCE_TIME = (SELECT MAX(ATTENDANCE_TIME) FROM HRIS_ATTENDANCE WHERE EMPLOYEE_ID = A.EMPLOYEE_ID AND TRUNC(ATTENDANCE_DT) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR'))
                                  GROUP BY A.EMPLOYEE_ID, A.ATTENDANCE_TIME
                                  ORDER BY A.EMPLOYEE_ID DESC
                      ) EOD ON EOD.SP_CODE = DLU.SP_CODE
                      LEFT JOIN DIST_ROUTE_DETAIL DRD ON DRD.EMP_CODE = DLU.SP_CODE AND DRD.COMPANY_CODE = DLU.COMPANY_CODE AND DRD.DELETED_FLAG = 'N' AND TRUNC(DRD.ASSIGN_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR')
                      LEFT JOIN DIST_ROUTE_ENTITY DRE ON DRE.ROUTE_CODE = DRD.ROUTE_CODE AND DRE.COMPANY_CODE = DRD.COMPANY_CODE AND DRE.ENTITY_TYPE = 'R' AND DRE.DELETED_FLAG = 'N'
                      LEFT JOIN DIST_RESELLER_MASTER DRM ON DRM.RESELLER_CODE = DRE.ENTITY_CODE AND DRM.COMPANY_CODE = DRE.COMPANY_CODE AND DRM.ACTIVE = 'Y'
                      GROUP BY DLU.SP_CODE, TRIM(HES.EMPLOYEE_EDESC), TRIM(DGM.GROUP_EDESC), DRM.RESELLER_CODE, TRIM(DRM.RESELLER_NAME), DLU.COMPANY_CODE, API.ATN_IMAGE, ATN.ATN_DATE, ATN.ATN_LATITUDE, ATN.ATN_LONGITUDE, EOD.EOD_DATE, EOD.EOD_LATITUDE, EOD.EOD_LONGITUDE
                  ) ENT
                  GROUP BY ENT.GROUP_EDESC, ENT.SP_CODE, ENT.EMPLOYEE_EDESC, ENT.COMPANY_CODE, ENT.ATN_IMAGE, ENT.ATN_DATE, ENT.ATN_LATITUDE, ENT.ATN_LONGITUDE, ENT.EOD_DATE, ENT.EOD_LATITUDE, ENT.EOD_LONGITUDE
                  ) RETBL -- Retailer Entity Table
            LEFT JOIN (SELECT ENT.SP_CODE, ENT.EMPLOYEE_EDESC, ENT.COMPANY_CODE, COUNT(ENT.ENTITY_CODE) VISITED
                        FROM (
                            SELECT DLU.SP_CODE, TRIM(HES.EMPLOYEE_EDESC) EMPLOYEE_EDESC, DRM.RESELLER_CODE ENTITY_CODE, TRIM(DRM.RESELLER_NAME) ENTITY_NAME, DLU.COMPANY_CODE
                            FROM DIST_LOGIN_USER DLU
                            INNER JOIN HR_EMPLOYEE_SETUP HES ON HES.EMPLOYEE_CODE = DLU.SP_CODE AND HES.COMPANY_CODE = DLU.COMPANY_CODE
                            LEFT JOIN DIST_ROUTE_DETAIL DRD ON DRD.EMP_CODE = DLU.SP_CODE AND DRD.COMPANY_CODE = DLU.COMPANY_CODE AND DRD.DELETED_FLAG = 'N' AND TRUNC(DRD.ASSIGN_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR')
                            LEFT JOIN DIST_ROUTE_ENTITY DRE ON DRE.ROUTE_CODE = DRD.ROUTE_CODE AND DRE.COMPANY_CODE = DRD.COMPANY_CODE AND DRE.ENTITY_TYPE = 'R' AND DRE.DELETED_FLAG = 'N'
                            LEFT JOIN DIST_RESELLER_MASTER DRM ON DRM.RESELLER_CODE = DRE.ENTITY_CODE AND DRM.COMPANY_CODE = DRE.COMPANY_CODE AND DRM.ACTIVE = 'Y'
                            GROUP BY DLU.SP_CODE, TRIM(HES.EMPLOYEE_EDESC), DRM.RESELLER_CODE, TRIM(DRM.RESELLER_NAME), DLU.COMPANY_CODE
                        ) ENT
                        INNER JOIN (SELECT SP_CODE, CUSTOMER_CODE, COMPANY_CODE FROM DIST_LOCATION_TRACK WHERE TRUNC(UPDATE_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR') GROUP BY SP_CODE, CUSTOMER_CODE, COMPANY_CODE) DLT ON DLT.SP_CODE = ENT.SP_CODE AND DLT.CUSTOMER_CODE = ENT.ENTITY_CODE AND DLT.COMPANY_CODE = ENT.COMPANY_CODE
                        WHERE DLT.SP_CODE IS NOT NULL
                        GROUP BY ENT.SP_CODE, ENT.EMPLOYEE_EDESC, ENT.COMPANY_CODE
                  ) RVTBL -- Retailer Visit Table
                  ON RVTBL.SP_CODE = RETBL.SP_CODE AND RVTBL.COMPANY_CODE = RETBL.COMPANY_CODE
            LEFT JOIN (SELECT ENT.SP_CODE, ENT.EMPLOYEE_EDESC, ENT.COMPANY_CODE, COUNT(ENT.ENTITY_CODE) NOT_VISITED
                      FROM (
                          SELECT DLU.SP_CODE, TRIM(HES.EMPLOYEE_EDESC) EMPLOYEE_EDESC, DRM.RESELLER_CODE ENTITY_CODE, TRIM(DRM.RESELLER_NAME) ENTITY_NAME, DLU.COMPANY_CODE
                          FROM DIST_LOGIN_USER DLU
                          INNER JOIN HR_EMPLOYEE_SETUP HES ON HES.EMPLOYEE_CODE = DLU.SP_CODE AND HES.COMPANY_CODE = DLU.COMPANY_CODE
                          LEFT JOIN DIST_ROUTE_DETAIL DRD ON DRD.EMP_CODE = DLU.SP_CODE AND DRD.COMPANY_CODE = DLU.COMPANY_CODE AND DRD.DELETED_FLAG = 'N' AND TRUNC(DRD.ASSIGN_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR')
                          LEFT JOIN DIST_ROUTE_ENTITY DRE ON DRE.ROUTE_CODE = DRD.ROUTE_CODE AND DRE.COMPANY_CODE = DRD.COMPANY_CODE AND DRE.ENTITY_TYPE = 'R' AND DRE.DELETED_FLAG = 'N'
                          LEFT JOIN DIST_RESELLER_MASTER DRM ON DRM.RESELLER_CODE = DRE.ENTITY_CODE AND DRM.COMPANY_CODE = DRE.COMPANY_CODE AND DRM.ACTIVE = 'Y'
                          GROUP BY DLU.SP_CODE, TRIM(HES.EMPLOYEE_EDESC), DRM.RESELLER_CODE, TRIM(DRM.RESELLER_NAME), DLU.COMPANY_CODE
                      ) ENT
                      LEFT JOIN (SELECT SP_CODE, CUSTOMER_CODE, COMPANY_CODE FROM DIST_LOCATION_TRACK WHERE TRUNC(UPDATE_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR') GROUP BY SP_CODE, CUSTOMER_CODE, COMPANY_CODE) DLT ON DLT.SP_CODE = ENT.SP_CODE AND DLT.CUSTOMER_CODE = ENT.ENTITY_CODE AND DLT.COMPANY_CODE = ENT.COMPANY_CODE
                      WHERE DLT.SP_CODE IS NULL
                      GROUP BY ENT.SP_CODE, ENT.EMPLOYEE_EDESC, ENT.COMPANY_CODE
                ) RNVTBL -- Retailer Not Visited Table
                ON RNVTBL.SP_CODE = RETBL.SP_CODE AND RNVTBL.COMPANY_CODE = RETBL.COMPANY_CODE
            LEFT JOIN (SELECT PJPENT.SP_CODE, PJPENT.EMPLOYEE_EDESC, PJPENT.COMPANY_CODE, COUNT(RPO.RESELLER_CODE) PJP_PRODUCTIVE, SUM(RPO.TOTAL_QUANTITY) TOTAL_QUANTITY, SUM(RPO.TOTAL_PRICE) TOTAL_PRICE
                      FROM (SELECT DLU.USERID, DLU.SP_CODE, TRIM(HES.EMPLOYEE_EDESC) EMPLOYEE_EDESC, DRM.RESELLER_CODE ENTITY_CODE, TRIM(DRM.RESELLER_NAME) ENTITY_NAME, DLU.COMPANY_CODE
                            FROM DIST_LOGIN_USER DLU
                            INNER JOIN HR_EMPLOYEE_SETUP HES ON HES.EMPLOYEE_CODE = DLU.SP_CODE AND HES.COMPANY_CODE = DLU.COMPANY_CODE
                            LEFT JOIN DIST_ROUTE_DETAIL DRD ON DRD.EMP_CODE = DLU.SP_CODE AND DRD.COMPANY_CODE = DLU.COMPANY_CODE AND DRD.DELETED_FLAG = 'N' AND TRUNC(DRD.ASSIGN_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR')
                            LEFT JOIN DIST_ROUTE_ENTITY DRE ON DRE.ROUTE_CODE = DRD.ROUTE_CODE AND DRE.COMPANY_CODE = DRD.COMPANY_CODE AND DRE.ENTITY_TYPE = 'R' AND DRE.DELETED_FLAG = 'N'
                            LEFT JOIN DIST_RESELLER_MASTER DRM ON DRM.RESELLER_CODE = DRE.ENTITY_CODE AND DRM.COMPANY_CODE = DRE.COMPANY_CODE AND DRM.ACTIVE = 'Y'
                            GROUP BY DLU.USERID, DLU.SP_CODE, TRIM(HES.EMPLOYEE_EDESC), DRM.RESELLER_CODE, TRIM(DRM.RESELLER_NAME), DLU.COMPANY_CODE
                      ) PJPENT
                      LEFT JOIN (SELECT CREATED_BY, RESELLER_CODE, COMPANY_CODE, COUNT(RESELLER_CODE) TOTAL_ORDER, SUM(QUANTITY) TOTAL_QUANTITY, SUM(TOTAL_PRICE) TOTAL_PRICE
                                  FROM DIST_IP_SSR_PURCHASE_ORDER
                                  WHERE TRUNC(ORDER_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR')
                                  GROUP BY CREATED_BY, RESELLER_CODE, COMPANY_CODE
                      ) RPO ON RPO.CREATED_BY = PJPENT.USERID AND RPO.RESELLER_CODE = PJPENT.ENTITY_CODE AND RPO.COMPANY_CODE = PJPENT.COMPANY_CODE
                      GROUP BY PJPENT.SP_CODE, PJPENT.EMPLOYEE_EDESC, PJPENT.COMPANY_CODE
                   ) RPJPTBL -- Retailer PJP Table
                   ON RPJPTBL.SP_CODE = RETBL.SP_CODE AND RPJPTBL.COMPANY_CODE = RETBL.COMPANY_CODE
            LEFT JOIN (SELECT RPO.SP_CODE, RPO.EMPLOYEE_EDESC, RPO.COMPANY_CODE, COUNT(RPO.RESELLER_CODE) NPJP_PRODUCTIVE, SUM(RPO.TOTAL_QUANTITY) TOTAL_QUANTITY, SUM(RPO.TOTAL_PRICE) TOTAL_PRICE
                      FROM (SELECT A.CREATED_BY, B.SP_CODE, A.RESELLER_CODE, TRIM(C.EMPLOYEE_EDESC) EMPLOYEE_EDESC, A.COMPANY_CODE, COUNT(A.RESELLER_CODE) TOTAL_ORDER, SUM(A.QUANTITY) TOTAL_QUANTITY, SUM(A.TOTAL_PRICE) TOTAL_PRICE
                            FROM DIST_IP_SSR_PURCHASE_ORDER A
                            INNER JOIN DIST_LOGIN_USER B ON B.USERID = A.CREATED_BY AND B.COMPANY_CODE = A.COMPANY_CODE
                            INNER JOIN HR_EMPLOYEE_SETUP C ON C.EMPLOYEE_CODE = B.SP_CODE AND C.COMPANY_CODE = A.COMPANY_CODE
                            WHERE TRUNC(A.ORDER_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR')
                            GROUP BY A.CREATED_BY, B.SP_CODE, A.RESELLER_CODE, TRIM(C.EMPLOYEE_EDESC), A.COMPANY_CODE
                      ) RPO
                      LEFT JOIN (SELECT DLU.USERID, DLU.SP_CODE, TRIM(HES.EMPLOYEE_EDESC) EMPLOYEE_EDESC, DRM.RESELLER_CODE ENTITY_CODE, TRIM(DRM.RESELLER_NAME) ENTITY_NAME, DLU.COMPANY_CODE
                                  FROM DIST_LOGIN_USER DLU
                                  INNER JOIN HR_EMPLOYEE_SETUP HES ON HES.EMPLOYEE_CODE = DLU.SP_CODE AND HES.COMPANY_CODE = DLU.COMPANY_CODE
                                  LEFT JOIN DIST_ROUTE_DETAIL DRD ON DRD.EMP_CODE = DLU.SP_CODE AND DRD.COMPANY_CODE = DLU.COMPANY_CODE AND DRD.DELETED_FLAG = 'N' AND TRUNC(DRD.ASSIGN_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR')
                                  LEFT JOIN DIST_ROUTE_ENTITY DRE ON DRE.ROUTE_CODE = DRD.ROUTE_CODE AND DRE.COMPANY_CODE = DRD.COMPANY_CODE AND DRE.ENTITY_TYPE = 'R' AND DRE.DELETED_FLAG = 'N'
                                  LEFT JOIN DIST_RESELLER_MASTER DRM ON DRM.RESELLER_CODE = DRE.ENTITY_CODE AND DRM.COMPANY_CODE = DRE.COMPANY_CODE AND DRM.ACTIVE = 'Y'
                                  GROUP BY DLU.USERID, DLU.SP_CODE, TRIM(HES.EMPLOYEE_EDESC), DRM.RESELLER_CODE, TRIM(DRM.RESELLER_NAME), DLU.COMPANY_CODE
                      ) NPJPENT ON NPJPENT.USERID = RPO.CREATED_BY AND NPJPENT.SP_CODE = RPO.SP_CODE AND NPJPENT.ENTITY_CODE = RPO.RESELLER_CODE AND NPJPENT.COMPANY_CODE = RPO.COMPANY_CODE
                      WHERE 1 = 1
                        AND NPJPENT.SP_CODE IS NULL
                      GROUP BY RPO.SP_CODE, RPO.EMPLOYEE_EDESC, RPO.COMPANY_CODE
                   ) RNPJPTBL -- Retailer NPJP Table
                   ON RNPJPTBL.SP_CODE = RETBL.SP_CODE AND RNPJPTBL.COMPANY_CODE = RETBL.COMPANY_CODE
            LEFT JOIN (SELECT DLU.USERID, DLU.SP_CODE, DLU.COMPANY_CODE, COUNT(DRM.RESELLER_CODE) OUTLET_ADDED
                        FROM DIST_RESELLER_MASTER DRM 
                        INNER JOIN DIST_LOGIN_USER DLU ON DLU.USERID = DRM.CREATED_BY AND DLU.COMPANY_CODE = DRM.COMPANY_CODE
                        WHERE 1 = 1
                          AND DRM.CREATED_DATE = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR')
                        GROUP BY DLU.USERID, DLU.SP_CODE, DLU.COMPANY_CODE
            ) ROUT -- New Retailer/Outlets
            ON ROUT.SP_CODE = RETBL.SP_CODE AND ROUT.COMPANY_CODE = RETBL.COMPANY_CODE
      ) RTBL
) PFMTBL
LEFT JOIN (SELECT TOD.SP_CODE, LISTAGG(TOD.ROUTE_CODE, ', ') WITHIN GROUP (ORDER BY TOD.ROUTE_CODE) TOD_ROUTE_CODE, LISTAGG(TOD.ROUTE_NAME, ', ') WITHIN GROUP (ORDER BY TOD.ROUTE_NAME) TOD_ROUTE_NAME
            FROM (SELECT DRD.EMP_CODE SP_CODE, DRM.ROUTE_CODE, TRIM(DRM.ROUTE_NAME) ROUTE_NAME, DRM.COMPANY_CODE
                  FROM DIST_ROUTE_DETAIL DRD
                  INNER JOIN DIST_ROUTE_MASTER DRM ON DRM.ROUTE_CODE = DRD.ROUTE_CODE AND DRM.COMPANY_CODE = DRD.COMPANY_CODE
                  WHERE 1 = 1
                    AND TRUNC(DRD.ASSIGN_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR') 
                    AND DRD.COMPANY_CODE = '{Company}'
                  GROUP BY DRD.EMP_CODE, DRM.ROUTE_CODE, DRM.ROUTE_NAME, DRM.COMPANY_CODE
                  ORDER BY UPPER(TRIM(DRM.ROUTE_NAME))
            ) TOD
            GROUP BY TOD.SP_CODE
) TODTBL ON TODTBL.SP_CODE = PFMTBL.SP_CODE
LEFT JOIN (SELECT TOM.SP_CODE, LISTAGG(TOM.ROUTE_CODE, ', ') WITHIN GROUP (ORDER BY TOM.ROUTE_CODE) TOM_ROUTE_CODE, LISTAGG(TOM.ROUTE_NAME, ', ') WITHIN GROUP (ORDER BY TOM.ROUTE_NAME) TOM_ROUTE_NAME
            FROM (SELECT DRD.EMP_CODE SP_CODE, DRM.ROUTE_CODE, TRIM(DRM.ROUTE_NAME) ROUTE_NAME, DRM.COMPANY_CODE
                  FROM DIST_ROUTE_DETAIL DRD
                  INNER JOIN DIST_ROUTE_MASTER DRM ON DRM.ROUTE_CODE = DRD.ROUTE_CODE AND DRM.COMPANY_CODE = DRD.COMPANY_CODE
                  WHERE 1 = 1
                    AND TRUNC(DRD.ASSIGN_DATE) = (TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR') + 1)
                    AND DRD.COMPANY_CODE = '{Company}'
                  GROUP BY DRD.EMP_CODE, DRM.ROUTE_CODE, DRM.ROUTE_NAME, DRM.COMPANY_CODE
                  ORDER BY UPPER(TRIM(DRM.ROUTE_NAME))
            ) TOM
            GROUP BY TOM.SP_CODE
) TOMTBL ON TOMTBL.SP_CODE = PFMTBL.SP_CODE
WHERE 1 = 1
  AND PFMTBL.COMPANY_CODE = '{Company}'  
GROUP BY PFMTBL.GROUP_EDESC, PFMTBL.SP_CODE, PFMTBL.EMPLOYEE_EDESC, 
  TODTBL.TOD_ROUTE_CODE, TODTBL.TOD_ROUTE_NAME, TOMTBL.TOM_ROUTE_CODE, TOMTBL.TOM_ROUTE_NAME,
  PFMTBL.ATN_IMAGE, PFMTBL.ATN_DATE, PFMTBL.ATN_LATITUDE, PFMTBL.ATN_LONGITUDE, PFMTBL.EOD_DATE, PFMTBL.EOD_LATITUDE, PFMTBL.EOD_LONGITUDE
ORDER BY UPPER(PFMTBL.GROUP_EDESC), UPPER(PFMTBL.EMPLOYEE_EDESC)";

                    //                var totalNPJPQuery = $@"SELECT
                    //NVL((SELECT COUNT(DISTINCT CUSTOMER_CODE) FROM DIST_VISITED_ENTITY WHERE SP_CODE = '{SpCode}' AND COMPANY_CODE = '{Company}' AND TRUNC(UPDATE_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR') ),0)
                    //-
                    //NVL((SELECT COUNT(DISTINCT RESELLER_CODE) FROM DIST_VISITED_PO WHERE SP_CODE = '{SpCode}' AND COMPANY_CODE = '{Company}' AND TRUNC(ORDER_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR') 
                    //        AND RESELLER_CODE IN (SELECT ENTITY_CODE FROM DIST_TARGET_ENTITY WHERE  SP_CODE = '{SpCode}' AND COMPANY_CODE = '{Company}' AND TRUNC(ASSIGN_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR') )),0)
                    //TOTAL_NPJP
                    //FROM DUAL";

                    #endregion EODQuery

                }
                else
                {
                    var totalVis = $@"-- VIIST, PLAN AND NEW OUTLET COUNT
SELECT DRD.EMP_CODE
	,DRD.ASSIGN_DATE
	,DRD.COMPANY_CODE
	,(
		SELECT COUNT(ROUTE_CODE)
		FROM DIST_ROUTE_ENTITY
		WHERE ROUTE_CODE = DRD.ROUTE_CODE
		)  TARGET
	,(
		SELECT COUNT(*)
		FROM DIST_LOCATION_TRACK
		WHERE SP_CODE = DRD.EMP_CODE
			AND IS_VISITED = 'Y'
			AND TRUNC(UPDATE_DATE) = DRD.ASSIGN_DATE
		) VISITED
	,(
		SELECT COUNT(*)
		FROM DIST_RESELLER_MASTER DRM
			,DIST_LOGIN_USER DLU
		WHERE DRM.CREATED_BY = DLU.USERID
			AND DLU.SP_CODE = DRD.EMP_CODE
			AND TRUNC(DRM.CREATED_DATE) = DRD.ASSIGN_DATE
		) OUTLET_ADDED
FROM DIST_ROUTE_DETAIL DRD
WHERE COMPANY_CODE ='{Company}' ";

                    var eodInOut = $@"--ATTENDANCE IN AND OUT TIME IN REFERENCE WITH ALL TRACK_TYPE, EMP CODE IS TAKEN REFERENCE FROM ABOVE QUERY RESULT
SELECT MIN(LLT.SUBMIT_DATE) ATN_DATE
	,MAX(LLT.SUBMIT_DATE) EOD_DATE
FROM DIST_LM_LOCATION_TRACKING LLT
WHERE TRUNC(LLT.SUBMIT_DATE) = TRUNC(TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR'))
	AND LLT.SP_CODE ='{SpCode}'";



                    var toDRout = $@"SELECT DRM.ROUTE_CODE, DRM.ROUTE_NAME TOD_ROUTE_NAME
  FROM DIST_ROUTE_DETAIL DRD, DIST_ROUTE_MASTER DRM
  WHERE DRD.ROUTE_CODE = DRM.ROUTE_CODE
  AND DRD.COMPANY_CODE = DRM.COMPANY_CODE
  AND TRUNC(DRD.ASSIGN_DATE) =TRUNC(TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR'))
  AND DRD.EMP_CODE = '{SpCode}'";

                    var tomRout = $@"  --TOMORROW ROUTE, DATE AND EMP CODE IS TAKEN FROM ABOVE
SELECT DRM.ROUTE_CODE
	,DRM.ROUTE_NAME TOM_ROUTE_NAME
FROM DIST_ROUTE_DETAIL DRD
	,DIST_ROUTE_MASTER DRM
WHERE DRD.ROUTE_CODE = DRM.ROUTE_CODE
	AND DRD.COMPANY_CODE = DRM.COMPANY_CODE
	AND TRUNC(DRD.ASSIGN_DATE) =TRUNC(TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR'))
	AND DRD.EMP_CODE ='{SpCode}'";


                    var eodTime = $@" -- IF DATA, EOD IS DONE ELSE NOT DONE, REPLACE OUT TIME BY EOD TIME IF DATA EXISTS, EMP CODE IS TAKEN REFERENCE FROM ABOVE QUERY RESULT
SELECT MAX(SUBMIT_DATE) EOD_TIME
FROM DIST_LM_LOCATION_TRACKING
WHERE TRUNC(SUBMIT_DATE) = TRUNC(TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR'))
	AND SP_CODE ='{SpCode}'
	AND TRACK_TYPE = 'EOD'";


                    var order = $@"-- EMP CODE AND DATE IS TAKEN REFERENCE FROM ABOVE QUERY RESULT 
SELECT BRAND_NAME
	,MU_CODE
	,SUM(QTY) QTY
	,SUM(TOTAL_AMT) AMT
FROM (
	SELECT ODATA.*
	FROM (
		SELECT NVL(ISS.BRAND_NAME, 'NA') BRAND_NAME
			,DPO.CREATED_BY
			,TRUNC(ORDER_DATE) ORDER_DATE
			,DPO.MU_CODE
			,SUM(QUANTITY) QTY
			,SUM(TOTAL_PRICE) TOTAL_AMT
		FROM DIST_IP_SSD_PURCHASE_ORDER DPO
			,IP_ITEM_SPEC_SETUP ISS
		WHERE 1 = 1
			AND DPO.ITEM_CODE = ISS.ITEM_CODE
			AND DPO.COMPANY_CODE = ISS.COMPANY_CODE
		GROUP BY DPO.MU_CODE
			,NVL(ISS.BRAND_NAME, 'NA')
			,DPO.CREATED_BY
			,TRUNC(ORDER_DATE)
		
		UNION ALL
		
		SELECT NVL(ISS.BRAND_NAME, 'NA') BRAND_NAME
			,DPO.CREATED_BY
			,TRUNC(ORDER_DATE) ORDER_DATE
			,DPO.MU_CODE
			,SUM(QUANTITY) QTY
			,SUM(TOTAL_PRICE) TOTAL_AMT
		FROM DIST_IP_SSR_PURCHASE_ORDER DPO
			,IP_ITEM_SPEC_SETUP ISS
		WHERE 1 = 1
			AND DPO.ITEM_CODE = ISS.ITEM_CODE
			AND DPO.COMPANY_CODE = ISS.COMPANY_CODE
		GROUP BY DPO.MU_CODE
			,NVL(ISS.BRAND_NAME, 'NA')
			,DPO.CREATED_BY
			,TRUNC(ORDER_DATE)
		) ODATA
		,DIST_LOGIN_USER DLU
	WHERE ODATA.CREATED_BY = DLU.USERID(+)
		AND DLU.SP_CODE = '{SpCode}'
		AND TRUNC(ODATA.ORDER_DATE) =TRUNC(TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR'))
	)
GROUP BY BRAND_NAME
	,MU_CODE";

                    var collec = $@"  --Collection report
-- EMP CODE AND DATE IS TAKEN REFERENCE FROM ABOVE QUERY RESULT 
SELECT PAYMENT_MODE
	,(
		CASE 
			WHEN ENTITY_TYPE = 'D'
				THEN 'DISTRIBUTOR'
			WHEN ENTITY_TYPE = 'R'
				THEN 'RESELLER'
			END
		) COLLECTED_FROM
	,SUM(AMOUNT)
FROM DIST_COLLECTION
WHERE SP_CODE = '{SpCode}'
	AND TRUNC(CREATED_DATE) = TRUNC(TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR'))
GROUP BY PAYMENT_MODE
	,(
		CASE 
			WHEN ENTITY_TYPE = 'D'
				THEN 'DISTRIBUTOR'
			WHEN ENTITY_TYPE = 'R'
				THEN 'RESELLER'
			END
		)";


                    var sReturn = $@"--SALES RETURN 
--EMP CODE AND DATE IS TAKEN REFERENCE FROM ABOVE QUERY RESULT 
SELECT NVL(ISS.BRAND_NAME, 'NA') BRAND_NAME
	,DPO.MU_CODE
	,SUM(QUANTITY) QTY
    ,SUM(TOTAL_PRICE) TOTAL_AMT
FROM DIST_SALES_RETURN DPO
	,IP_ITEM_SPEC_SETUP ISS
	,DIST_LOGIN_USER DLU
WHERE 1 = 1
	AND DPO.ITEM_CODE = ISS.ITEM_CODE
	AND DPO.COMPANY_CODE = ISS.COMPANY_CODE
	AND DPO.CREATED_BY = DLU.USERID(+)
	AND DPO.COMPANY_CODE = DLU.COMPANY_CODE(+)
	AND DLU.SP_CODE ='{SpCode}'
	AND TRUNC(DPO.CREATED_DATE) = NVL(sysdate, TRUNC(SYSDATE))
GROUP BY DPO.MU_CODE
	,NVL(ISS.BRAND_NAME, 'NA')
	,DPO.MU_CODE";

                    //                    EodData = $@"SELECT 
                    //    DRD.EMP_CODE,
                    //    DRD.ASSIGN_DATE,
                    //    DRD.COMPANY_CODE,

                    //    -- Count of routes for the employee's route code
                    //    (SELECT COUNT(ROUTE_CODE)
                    //     FROM DIST_ROUTE_ENTITY
                    //     WHERE ROUTE_CODE = DRD.ROUTE_CODE) AS TARGET,

                    //    -- Count of visited locations for the employee on the assigned date
                    //    (SELECT COUNT(*)
                    //     FROM DIST_LOCATION_TRACK
                    //     WHERE SP_CODE = DRD.EMP_CODE
                    //       AND IS_VISITED = 'Y'
                    //       AND TRUNC(UPDATE_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR')) AS VISITED,

                    //    -- Count of not visited locations for the employee on the assigned date
                    //    --    (SELECT COUNT(*)
                    //    --     FROM DIST_LOCATION_TRACK
                    //    --     WHERE SP_CODE = DRD.EMP_CODE
                    //    --       AND IS_VISITED = 'N'
                    //    --       AND TRUNC(UPDATE_DATE) = DRD.ASSIGN_DATE) AS NOT_VISITED,

                    //    -- Count of outlets added by the employee on the assigned date
                    //    (SELECT COUNT(*)
                    //     FROM DIST_RESELLER_MASTER DRM
                    //     JOIN DIST_LOGIN_USER DLU ON DRM.CREATED_BY = DLU.USERID
                    //     WHERE DLU.SP_CODE = DRD.EMP_CODE
                    //       AND TRUNC(DRM.CREATED_DATE) = TRUNC(DRD.ASSIGN_DATE)) AS OUTLET_ADDED,

                    //    -- Minimum submit date from DIST_LM_LOCATION_TRACKING for the employee
                    //    (SELECT MIN(LLT.SUBMIT_DATE)
                    //     FROM DIST_LM_LOCATION_TRACKING LLT
                    //     WHERE LLT.SP_CODE = DRD.EMP_CODE
                    //       AND TRUNC(LLT.SUBMIT_DATE) = TRUNC(DRD.ASSIGN_DATE)) AS ATN_DATE,

                    //    -- Maximum submit date from DIST_LM_LOCATION_TRACKING for the employee
                    //    (SELECT MAX(LLT.SUBMIT_DATE)
                    //     FROM DIST_LM_LOCATION_TRACKING LLT
                    //     WHERE LLT.SP_CODE = DRD.EMP_CODE
                    //       AND TRUNC(LLT.SUBMIT_DATE) = TRUNC(DRD.ASSIGN_DATE)) AS EOD_DATE,

                    //    (SELECT COUNT(DISTINCT ORDER_NO)  
                    //     FROM dist_visited_po
                    //     WHERE SP_CODE = DRD.EMP_CODE
                    //       AND TRUNC(ORDER_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR')) AS PJP_PRODUCTIVE,

                    //    (SELECT 
                    //        COUNT(DISTINCT RESELLER_CODE)
                    //    FROM 
                    //        DIST_VISITED_PO
                    //    WHERE 
                    //        SP_CODE = DRD.EMP_CODE
                    //        AND TRUNC(ORDER_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-YYYY')
                    //        AND RESELLER_CODE NOT IN (
                    //            SELECT ENTITY_CODE 
                    //            FROM DIST_TARGET_ENTITY 
                    //            WHERE SP_CODE = DRD.EMP_CODE and assign_date = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-YYYY')
                    //        )) AS NPJP_PRODUCTIVE,

                    //(SELECT 
                    //    COUNT(*)
                    //    FROM DIST_VISITED_ENTITY
                    //     WHERE SP_CODE = DRD.EMP_CODE
                    //       AND IS_VISITED = 'Y'
                    //       AND TRUNC(UPDATE_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR')
                    //    AND ENTITY_NAME NOT IN (
                    //        SELECT ENTITY_NAME 
                    //        FROM DIST_TARGET_ENTITY 
                    //        WHERE SP_CODE = DRD.EMP_CODE and assign_date = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-YYYY')
                    //    )) AS NPJP_COUNT,


                    //    -- Route name for the assigned route
                    //    (SELECT DRM.ROUTE_NAME
                    //     FROM DIST_ROUTE_MASTER DRM
                    //     WHERE DRM.ROUTE_CODE = DRD.ROUTE_CODE
                    //       AND DRM.COMPANY_CODE = DRD.COMPANY_CODE) AS TOD_ROUTE_NAME,

                    //    -- Route name for the assigned route (again, if needed)
                    //    (SELECT DRM.ROUTE_NAME
                    // FROM DIST_ROUTE_MASTER DRM
                    // WHERE DRM.ROUTE_CODE = (
                    //         SELECT ROUTE_CODE
                    //         FROM DIST_ROUTE_DETAIL
                    //         WHERE EMP_CODE = DRD.EMP_CODE
                    //           AND TRUNC(ASSIGN_DATE) = TRUNC(DRD.ASSIGN_DATE) + 1
                    //           AND COMPANY_CODE = DRD.COMPANY_CODE
                    //     )
                    //) AS TOM_ROUTE_NAME

                    //FROM 
                    //    DIST_ROUTE_DETAIL DRD
                    //WHERE 
                    //    DRD.COMPANY_CODE = '{Company}'
                    //    AND DRD.EMP_CODE = '{SpCode}'  -- Replace '1020' with the specific employee code you want to filter by
                    //    AND TRUNC(DRD.ASSIGN_DATE) = TRUNC(TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR'))";

                    EodData = $@"
select a.*,
nvl(a.TARGET,0)-(nvl(a.VISITED,0)-nvl(a.NPJP_COUNT,0)) NOT_VISITED,
NVL(a.VISITED, 0) - NVL(a.NPJP_COUNT, 0) AS PJP_COUNT
 from 
(
select hr.EMPLOYEE_CODE,hr.EMPLOYEE_CODE sp_code,
hr.EMPLOYEE_CODE EMP_CODE,   DRD.COMPANY_CODE,
(SELECT COUNT (ROUTE_CODE)
          FROM DIST_ROUTE_ENTITY
         WHERE ROUTE_CODE = DRD.ROUTE_CODE)
          AS TARGET,
 (
       SELECT COUNT (*)
          FROM DIST_LOCATION_TRACK
         WHERE     SP_CODE = hr.EMPLOYEE_CODE
               AND IS_VISITED = 'Y'
               AND TRUNC (UPDATE_DATE) =
                     trunc(sysdate)
                      )
          AS VISITED,
(SELECT COUNT (*)
          FROM    DIST_RESELLER_MASTER DRM
               JOIN
                  DIST_LOGIN_USER DLU
               ON DRM.CREATED_BY = DLU.USERID
         WHERE     DLU.SP_CODE = hr.EMPLOYEE_CODE
               AND TRUNC (DRM.CREATED_DATE) = TRUNC (sysdate))
          AS OUTLET_ADDED,
(SELECT MIN (LLT.SUBMIT_DATE)
          FROM DIST_LM_LOCATION_TRACKING LLT
         WHERE     LLT.SP_CODE = hr.EMPLOYEE_CODE
               AND TRUNC (LLT.SUBMIT_DATE) = TRUNC (sysdate))
          AS ATN_DATE,
 (SELECT MAX (LLT.SUBMIT_DATE)
          FROM DIST_LM_LOCATION_TRACKING LLT
         WHERE     LLT.SP_CODE = hr.EMPLOYEE_CODE
               AND TRUNC (LLT.SUBMIT_DATE) = TRUNC (sysdate))
          AS EOD_DATE,
 (SELECT COUNT (DISTINCT ORDER_NO)
          FROM dist_visited_po
         WHERE     SP_CODE = hr.EMPLOYEE_CODE
               AND TRUNC (ORDER_DATE) =
                      trunc(sysdate) )
          AS PJP_PRODUCTIVE,
(SELECT COUNT (DISTINCT RESELLER_CODE)
          FROM DIST_VISITED_PO
         WHERE     SP_CODE = hr.EMPLOYEE_CODE
               AND TRUNC (ORDER_DATE) =
                      trunc(sysdate)
               AND RESELLER_CODE NOT IN
                      (SELECT ENTITY_CODE
                         FROM DIST_TARGET_ENTITY
                        WHERE     SP_CODE = hr.EMPLOYEE_CODE
                              AND assign_date =
                                     trunc(sysdate)))
          AS NPJP_PRODUCTIVE,
(SELECT COUNT (*)
          FROM DIST_VISITED_ENTITY
         WHERE     SP_CODE = hr.EMPLOYEE_CODE
               AND IS_VISITED = 'Y'
               AND TRUNC (UPDATE_DATE) =
                      trunc(sysdate)
               AND ENTITY_NAME NOT IN
                      (SELECT ENTITY_NAME
                         FROM DIST_TARGET_ENTITY
                        WHERE     SP_CODE = hr.EMPLOYEE_CODE
                              AND assign_date =trunc(sysdate) ))
          AS NPJP_COUNT,
(SELECT DRM.ROUTE_NAME
          FROM DIST_ROUTE_MASTER DRM
         WHERE     DRM.ROUTE_CODE = DRD.ROUTE_CODE
               AND DRM.COMPANY_CODE = DRD.COMPANY_CODE)
          AS TOD_ROUTE_NAME,
(SELECT DRM.ROUTE_NAME
          FROM DIST_ROUTE_MASTER DRM
         WHERE DRM.ROUTE_CODE =
                  (SELECT ROUTE_CODE
                     FROM DIST_ROUTE_DETAIL
                    WHERE     EMP_CODE = hr.employee_code
                          AND TRUNC (ASSIGN_DATE) =TRUNC (sysdate+1)
                          AND COMPANY_CODE = hr.COMPANY_CODE
                          )
                          )
          AS TOM_ROUTE_NAME 
from 
hr_employee_setup hr,
(select * from DIST_ROUTE_DETAIL  where ASSIGN_DATE=trunc(sysdate)) DRD
where 
hr.COMPANY_CODE = '{Company}'
      AND hr.EMPLOYEE_CODE = '{SpCode}'
       and  hr.company_code=DRD.company_code(+) 
       and  hr.EMPLOYEE_CODE=DRD.emp_code(+)            
       )a
";

                    var data1 = dbContext.SqlQuery<EODUpdate>(EodData).FirstOrDefault();
                    var orderData = dbContext.SqlQuery(order);
                    var returnData = dbContext.SqlQuery(sReturn);

                    //var message1 = $@"<b>PJP Call</b><br>Today's:{data1.TOD_ROUTE_NAME}<br>Target Calls:{data1.TARGET}<br> Total Calls (TC): {data1.VISITED}
                    //             <br>Productive Calls (PC): {data1.PJP_PRODUCTIVE}<br>Total NPJP (NPJP): N/A<br>NPJP Productive Calls (NPC): {data1.NPJP_PRODUCTIVE}
                    //             <br>Added Outlet (AO): {data1.OUTLET_ADDED}
                    //             <br>Total Not Visited:{data1.NOT_VISITED}<br><br><b>Attendance Time: {data1.ATN_DATE}</b><br>EOD Time:{data1.EOD_DATE}
                    //             <br>Tomorrow Route: {data1.TOM_ROUTE_NAME} <br> Remarks: {model[0].Remarks}";


                    var message1 = $@"<b>PJP Call</b><br>Today's:{data1.TOD_ROUTE_NAME}<br>Target Calls:{data1.TARGET}<br> Total Calls (TC): {data1.VISITED} <br>Total PJP Calls (PJP): {data1.PJP_COUNT}
                                 <br>PJP Productive Calls (PC): {data1.PJP_PRODUCTIVE}<br>Total NPJP Calls(NPJP): {data1.NPJP_COUNT} <br>NPJP Productive Calls (NPC): {data1.NPJP_PRODUCTIVE}
                                 <br>Added Outlet (AO): {data1.OUTLET_ADDED}
                                 <br>Total Not Visited:{data1.NOT_VISITED}<br><br><b>Attendance Time: {data1.ATN_DATE}</b><br>EOD Time:{data1.EOD_DATE}
                                 <br>Tomorrow Route: {data1.TOM_ROUTE_NAME} <br> Remarks: {model.FirstOrDefault()?.Remarks ?? ""}";

                    //var message1 = $@"<b>PJP Call</b><br>Today's:{data1.TOD_ROUTE_NAME}<br>Target Calls:{data1.TARGET}<br> Total Calls (TC): {data1.VISITED}
                    //             <br>Productive Calls (PC): {data1.PJP_PRODUCTIVE}<br>Total NPJP (NPJP): N/A<br>NPJP Productive Calls (NPC): {data1.NPJP_PRODUCTIVE}
                    //             <br>Added Outlet (AO): {data1.OUTLET_ADDED}
                    //             <br>Total Not Visited:{data1.NOT_VISITED}<br><br><b>Attendance Time: {data1.ATN_DATE}</b><br>EOD Time:{data1.EOD_DATE}
                    //             <br>Tomorrow Route: {data1.TOM_ROUTE_NAME} <br> Remarks: {model[0].Remarks}";

                    //var message1 = $@"<b>PJP Call</b><br>{data1.TOD_ROUTE_NAME}<br>{data1.TARGET}<br>{data1.VISITED}
                    //             <br>{data1.PJP_PRODUCTIVE}<br>{data1.NPJP_PRODUCTIVE}
                    //             <br>{data1.OUTLET_ADDED}
                    //             <br>{data1.NOT_VISITED}<br><br><b>{data1.ATN_DATE}</b><br>{data1.EOD_DATE}
                    //             <br>{data1.TOM_ROUTE_NAME}";



                    //var message1 = $@"<b>PJP Call</b><br>{data1.TOD_ROUTE_NAME}<br>{data1.TARGET}<br> {data1.VISITED}
                    //             <br>{data1.PJP_PRODUCTIVE}<br> {data1.NPJP_PRODUCTIVE}
                    //             <br> {data1.OUTLET_ADDED}
                    //             <br>{data1.NOT_VISITED}<br><br><b>{data1.ATN_DATE}</b><br>{data1.EOD_DATE}
                    //             <br> {data1.TOM_ROUTE_NAME} <br>  {model.FirstOrDefault()?.Remarks ?? ""}";

                    //var message1 = "<b>PJP Call</b><br> <b>Todays</b><br> " + (data1.TOD_ROUTE_NAME ?? "N/A") + "<br>";

                    //var message1 = $@"<b>PJP Call</b><br> <b>Todays</b><br>{data1.TOD_ROUTE_NAME}";
                    message1 += @"<br><br><b>Today's order details</b><br><table border='2'><tr><th>Brand</th><th>Quantity</td><th>Amount</th><th>Unit</th></tr>";
                    //foreach (DataRow row in ResellerData.Rows)
                    //{
                    //    message1 += $@"<tr><td>{row["BRAND_NAME"]}</td><td>{row["TOTAL_QUANTITY"]}</td><td>{row["TOTAL_AMOUNT"]}</td><td>{row["MU_CODE"]}</td></tr>";
                    //}
                    if (orderData != null)
                    {
                        foreach (DataRow row in orderData.Rows)
                        {
                            message1 += $@"<tr><td>{row["BRAND_NAME"]}</td><td>{row["QTY"]}</td><td>{row["AMT"]}</td><td>{row["MU_CODE"]}</td></tr>";
                        }
                    }

                    if (returnData != null)
                    {
                        foreach (DataRow row in returnData.Rows)
                        {
                            message1 += $@"<tr><td>{row["BRAND_NAME"]}</td><td>{row["QTY"]}</td><td>{row["TOTAL_AMT"]}</td><td>{row["MU_CODE"]}</td></tr>";
                        }
                    }
                    message1 += "</table>";
                    mailModel.MESSAGE = message1;
                    System.Net.Mail.Attachment ResellerAttach1;
                    if (companyName.Equals("Bhudeo Khadya Udyog P. Ltd."))
                    {
                        var SalesPersonData = dbContext.SqlQuery(SalesPersonQuery);
                        ResellerAttach1 = new System.Net.Mail.Attachment(CommonHelper.ConvertTableIntoExcel(ResellerData, SalesPersonData), string.Format("{0}.{1}", "Purchase Orders", "xls"));

                    }
                    else
                    {
                        ResellerAttach1 = new System.Net.Mail.Attachment(ResellerData.DataToExcel(), string.Format("{0}.{1}", "Purchase Orders", "xls"));

                    }
                    // var DistrubutorAttach = new System.Net.Mail.Attachment(DistrubutorData.DataToExcel(), string.Format("{0}.{1}", "Distributor Purchase Orders", "xls"));
                    System.Net.Mail.Attachment[] file1 = new System.Net.Mail.Attachment[] { ResellerAttach1 };


                    //mailModel.ATTACHMENT_FILE = file;
                    var emailSuccess1 = MailHelper.SendMailDirectAttach(string.Empty, mailModel.SUBJECT, mailModel.MESSAGE, mailModel.EMAIL_TO, mailModel.EMAIL_CC, mailModel.EMAIL_BCC, "", file1);
                    return true;
                }
                var totalNPJPQuery = $@"SELECT COUNT(DISTINCT CUSTOMER_CODE) TOTAL_NPJP  FROM DIST_VISITED_ENTITY WHERE SP_CODE='{SpCode}' AND COMPANY_CODE='{Company}' AND TRUNC(UPDATE_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR')
                                         AND CUSTOMER_CODE NOT IN (SELECT coalesce(ENTITY_CODE,'0') FROM DIST_TARGET_ENTITY WHERE  SP_CODE = '{SpCode}' AND COMPANY_CODE = '{Company}' AND TRUNC(ASSIGN_DATE) = TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy")}', 'DD-MON-RRRR') )";

                var data = dbContext.SqlQuery<EODUpdate>(EodData).Where(x => x.SP_CODE.Trim() == SpCode.Trim()).FirstOrDefault();
                int? total_npjp = dbContext.SqlQuery<int>(totalNPJPQuery).FirstOrDefault();
                string message = string.Empty;
                if (data != null)
                    message = $@"<b>PJP Call</b><br>Today's:{data.TOD_ROUTE_NAME}<br>Target Calls:{data.TARGET}<br> Total Calls (TC): {data.VISITED}
                                 <br>Productive Calls (PC): {data.PJP_PRODUCTIVE}<br>Total NPJP (NPJP): {total_npjp}<br>NPJP Productive Calls (NPC): {data.NPJP_PRODUCTIVE}
                                 <br>Added Outlet (AO): {data.OUTLET_ADDED}
                                 <br>Total Not Visited:{data.NOT_VISITED}<br><br><b>Attendance Time: {data.ATN_DATE}</b><br>EOD Time:{data.EOD_DATE}
                                 <br>Tomorrow Route: {data.TOM_ROUTE_NAME} <br> Remarks: {model[0].Remarks}";

                else
                    message = $@"<b>PJP Call</b><br>Today's: ---<br>Target Calls: 0<br>Total Calls (TC): 0
                                 <br>Productive Calls (PC): 0<br>Total NPJP (NPJP): 0<br>NPJP Productive Calls (NPC):0<br>Addition Outlet (AO): 0
                                 <br>Total Not Visisted: 0<br><br><b>Attendance Time: ---</b><br>EOD Time: ---
                                 <br>Tomorrow Route: ---<br> Remarks: {model[0].Remarks}";
                message += @"<br><br><b>Today's order details</b><br><table border='2'><tr><th>Brand</th><th>Quantity</td><th>Amount</th><th>Unit</th></tr>";
                foreach (DataRow row in ResellerData.Rows)
                {
                    message += $@"<tr><td>{row["BRAND_NAME"]}</td><td>{row["TOTAL_QUANTITY"]}</td><td>{row["TOTAL_AMOUNT"]}</td><td>{row["MU_CODE"]}</td></tr>";
                }
                message += "</table>";
                mailModel.MESSAGE = message;

                //mailModel.MESSAGE="K xa halkhabar dai meeting kasto hudai xa lol hahhahaha";

                var ResellerAttach = new System.Net.Mail.Attachment(ResellerData.DataToExcel(), string.Format("{0}.{1}", "Purchase Orders", "xls"));
                // var DistrubutorAttach = new System.Net.Mail.Attachment(DistrubutorData.DataToExcel(), string.Format("{0}.{1}", "Distributor Purchase Orders", "xls"));
                System.Net.Mail.Attachment[] file = new System.Net.Mail.Attachment[] { ResellerAttach };


                //mailModel.ATTACHMENT_FILE = file;
                var emailSuccess = MailHelper.SendMailDirectAttach(string.Empty, mailModel.SUBJECT, mailModel.MESSAGE, mailModel.EMAIL_TO, mailModel.EMAIL_CC, mailModel.EMAIL_BCC, "", file);
            }
            return true;
        }
        #endregion Sending Mail
    }
}