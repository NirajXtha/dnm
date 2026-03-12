using NeoErp.Core.Domain;
using NeoErp.Core.Models;
using NeoErp.Distribution.Service.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Web;
using System.Data;

namespace NeoErp.Distribution.Service.Service
{
    public class QuickSetupService : IQuickSetupService
    {
        private NeoErpCoreEntity _objectEntity;
        public QuickSetupService(NeoErpCoreEntity objectEntity)
        {
            _objectEntity = objectEntity;
        }
        public string CreateDistResellerMap(User userInfo)
        {
            var result = "";
            using (var trans = _objectEntity.Database.BeginTransaction())
            {
                try
                {
                    var allAreas = _objectEntity.SqlQuery<string>($"SELECT AREA_CODE FROM DIST_AREA_MASTER where company_code = '{userInfo.company_code}'").ToList();
                    foreach (var area in allAreas)
                    {
                        //adding the new distributors to mapping table
                        string AdditionQuery = $@"INSERT INTO DIST_RESELLER_ENTITY(RESELLER_CODE,ENTITY_CODE,ENTITY_TYPE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE) (
                    SELECT DRM.RESELLER_CODE,DDM.DISTRIBUTOR_CODE ENTITY_CODE,'D' ENTITY_TYPE, '{userInfo.company_code}' COMPANY_CODE, '{userInfo.branch_code}' BRANCH_CODE,
                                '{userInfo.User_id}' CREATED_BY, SYSDATE CREATED_DATE
                                FROM DIST_RESELLER_MASTER DRM
                                INNER JOIN DIST_DISTRIBUTOR_MASTER DDM ON DRM.AREA_CODE = DDM.AREA_CODE AND DRM.COMPANY_CODE = DDM.COMPANY_CODE
                                WHERE DDM.AREA_CODE = '{area}' AND DDM.DELETED_FLAG = 'N' AND DDM.COMPANY_CODE = '{userInfo.company_code}'
                    MINUS
                    SELECT RESELLER_CODE,ENTITY_CODE,'D' ENTITY_TYPE, '{userInfo.company_code}' COMPANY_CODE, '{userInfo.branch_code}' BRANCH_CODE,
                                '{userInfo.User_id}' CREATED_BY, SYSDATE CREATED_DATE
                                FROM DIST_RESELLER_ENTITY
                                WHERE ENTITY_TYPE = 'D' AND COMPANY_CODE = '{userInfo.company_code}'
                                AND ENTITY_CODE IN (SELECT DISTRIBUTOR_CODE FROM DIST_DISTRIBUTOR_MASTER WHERE AREA_CODE = '{area}'))";
                        var rows = _objectEntity.ExecuteSqlCommand(AdditionQuery);

                        //deleting area changed distributors
                        var query = $@"SELECT RESELLER_CODE CODE,ENTITY_CODE NAME
                                                FROM DIST_RESELLER_ENTITY
                                                WHERE ENTITY_TYPE = 'D' AND COMPANY_CODE = '{userInfo.company_code}'
                                                AND ENTITY_CODE IN (SELECT DISTRIBUTOR_CODE FROM DIST_DISTRIBUTOR_MASTER WHERE AREA_CODE = '{area}')
                                                MINUS
                    SELECT DRM.RESELLER_CODE CODE,DDM.DISTRIBUTOR_CODE NAME
                                                FROM DIST_RESELLER_MASTER DRM
                                                INNER JOIN DIST_DISTRIBUTOR_MASTER DDM ON DRM.AREA_CODE = DDM.AREA_CODE AND DRM.COMPANY_CODE = DDM.COMPANY_CODE
                                                WHERE DDM.AREA_CODE = '{area}' AND DDM.COMPANY_CODE = '{userInfo.company_code}'";
                        var deleted = _objectEntity.SqlQuery<CustomerModel>(query).GroupBy(x => x.Name);
                        foreach (var group in deleted)
                        {
                            var resellers = string.Join("','", group.Select(x => x.Code));
                            //(column,0) in ((1,0),(2,0),(3,0),(4,0), ... ,(1500,0))
                            var col = string.Join("',0),('", group.Select(x => x.Code));
                            var resellerwhere = $@"(RESELLER_CODE,0) IN (('{col}',0))";
                            _objectEntity.ExecuteSqlCommand($"DELETE FROM DIST_RESELLER_ENTITY WHERE ENTITY_CODE = '{group.Key}' AND COMPANY_CODE = '{userInfo.company_code}' AND {resellerwhere}-- AND RESELLER_CODE IN ('{resellers}')");
                        }
                    }
                    //removing the deleted distributors from mapping table
                    string RemoveQuery = $@"DELETE  FROM DIST_RESELLER_ENTITY
                    WHERE ENTITY_TYPE = 'D'
                    AND COMPANY_CODE = '{userInfo.company_code}'
                    AND ENTITY_CODE IN (
                    SELECT DISTINCT ENTITY_CODE FROM DIST_RESELLER_ENTITY WHERE ENTITY_TYPE = 'D'
                    MINUS
                    SELECT DISTRIBUTOR_CODE ENTITY_CODE FROM DIST_DISTRIBUTOR_MASTER WHERE DELETED_FLAG = 'N')";
                    var removedRows = _objectEntity.ExecuteSqlCommand(RemoveQuery);
                    trans.Commit();
                    result = "200";
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    result = ex.Message;
                }
            }
            return result;
        }

        public int createEntities()
        {
            FileInfo file = new FileInfo(System.AppDomain.CurrentDomain.BaseDirectory + "Areas\\NeoErp.Distribution\\App_Data\\DIST_ENTITES.sql");
            var stream = file.OpenText();
            string script = stream.ReadToEnd();
            stream.Close();
            stream.Dispose();

            var individualStmt = script.Split('#').ToList();
            var count = 0;
            foreach (var stmt in individualStmt)
            {
                try
                {
                    var success = _objectEntity.ExecuteSqlCommand(stmt);
                    count += success;
                }
                catch (Exception ex) { }
            }
            return count;
        }

        public int CreateDefaultValues()
        {

            FileInfo fileAddress = new FileInfo(System.AppDomain.CurrentDomain.BaseDirectory + "Areas\\NeoErp.Distribution\\App_Data\\ADDRESS_MASTER.sql");
            var stream = fileAddress.OpenText();
            string script_AddressMaster = stream.ReadToEnd();
            //districts
            fileAddress = new FileInfo(System.AppDomain.CurrentDomain.BaseDirectory + "Areas\\NeoErp.Distribution\\App_Data\\DISTRICT_CODE.sql");
            stream = fileAddress.OpenText();
            string script_districtCodes = stream.ReadToEnd();
            //Roles
            fileAddress = new FileInfo(System.AppDomain.CurrentDomain.BaseDirectory + "Areas\\NeoErp.Distribution\\App_Data\\DIST_ROLES.sql");
            stream = fileAddress.OpenText();
            string script_distRoles = stream.ReadToEnd();

            stream.Close();
            stream.Dispose();
            var individualStmt = script_AddressMaster.Split(';').ToList();
            var count = 0;
            using (var trans = _objectEntity.Database.BeginTransaction())
            {
                try
                {

                    //All districts,vdc's and regions in nepal
                    var deleted = _objectEntity.ExecuteSqlCommand("DELETE FROM DIST_ADDRESS_MASTER");
                    foreach (var stmt in individualStmt)
                    {
                        var success = string.IsNullOrWhiteSpace(stmt) ? 0 : _objectEntity.ExecuteSqlCommand(stmt);
                        count += success;
                    }

                    //districts Creation
                    deleted = _objectEntity.ExecuteSqlCommand("DELETE FROM DISTRICT_CODE");
                    count += _objectEntity.ExecuteSqlCommand(script_districtCodes);

                    //Roles Creation
                    deleted = _objectEntity.ExecuteSqlCommand("DELETE FROM DIST_ROLE_MASTER_SETUP");
                    count += _objectEntity.ExecuteSqlCommand(script_distRoles);

                    //default image category for attendance image
                    deleted = _objectEntity.ExecuteSqlCommand("DELETE FROM DIST_IMAGE_CATEGORY");
                    var atnImgCategory = $@"INSERT INTO DIST_IMAGE_CATEGORY (CATEGORYID,CATEGORY_CODE,CATEGORY_EDESC,MAX_ITEMS,CREATED_BY,CREATED_DATE,COMPANY_CODE)
                    (SELECT 1 CATEGORYID,'ATN' CATEGORY_CODE,'Attendance' CATEGORY_EDESC,1 MAX_ITEMS,1 CREATED_BY, SYSDATE CREATED_DATE,COMPANY_CODE FROM COMPANY_SETUP)";
                    count += _objectEntity.ExecuteSqlCommand(atnImgCategory);

                    //preference setup
                    var preferenceQuery = $@"INSERT INTO DIST_PREFERENCE_SETUP
                        (MO_GPS, MO_AGPS, PO_COMPANY_LIST, PO_PARTY_TYPE, PO_BILLING_NAME,
                        PO_SYN_RATE, PO_CUSTOM_RATE, PO_REMARKS, PO_CONVERSION_UNIT, PO_CONVERSION_FACTOR,
                        PO_SALES_TYPE, PO_SHIPPING_ADDRESS, PO_SHIPPING_CONTACT, PO_DISPLAY_DIST_ITEM, SO_CREDIT_LIMIT_CHK,
                        SO_CREDIT_DAYS_CHK, SO_CONSOLIDATE_DEFAULT, CS_CONVERSION_UNIT, QA_MKT_INFO, QA_COMPT_INFO,
                        SQL_NN_CONVERSION_UNIT_FACTOR, ATN_DEFAULT, ATN_IMAGE, SET_RES_MAP_WHOLESALER, COMPANY_CODE,
                        BRANCH_CODE, SQL_MULTIPLE_COMPANY, SQL_COMPANY_ENTITY, LO_BG_TRACK,
                        PO_RATE_TABLE, PO_RATE_COLUMN, PO_DIST_RATE_COLUMN, LO_BG_TIME, SQL_PEV_DAYS, SQL_FOL_DAYS,
                        MO_DISABLE_PLAYSTORE, MO_SAVE_DATA, IS_PHARMA)
                    (SELECT 'Y' MO_GPS, 'Y' MO_AGPS, 'N' PO_COMPANY_LIST, 'N' PO_PARTY_TYPE, 'N' PO_BILLING_NAME, 
                        'N' PO_SYN_RATE, 'N' PO_CUSTOM_RATE, 'N' PO_REMARKS, 'N' PO_CONVERSION_UNIT, 'N' PO_CONVERSION_FACTOR, 
                        'N' PO_SALES_TYPE, 'N' PO_SHIPPING_ADDRESS, 'N' PO_SHIPPING_CONTACT, 'Y' PO_DISPLAY_DIST_ITEM, 'Y' SO_CREDIT_LIMIT_CHK, 
                        'Y' SO_CREDIT_DAYS_CHK, 'N' SO_CONSOLIDATE_DEFAULT, 'N' CS_CONVERSION_UNIT, 'Y' QA_MKT_INFO, 'Y' QA_COMPT_INFO, 
                        'N' SQL_NN_CONVERSION_UNIT_FACTOR, 'Y' ATN_DEFAULT, 'Y' ATN_IMAGE, 'Y' SET_RES_MAP_WHOLESALER, COMPANY_CODE, 
                        COMPANY_CODE ||'.01' BRANCH_CODE, 'N' SQL_MULTIPLE_COMPANY, 'N' SQL_COMPANY_ENTITY, 'N' LO_BG_TRACK, 
                        'IP_ITEM_RATE_SCHEDULE_SETUP' PO_RATE_TABLE,'STANDARD_RATE' PO_RATE_COLUMN, 'STANDARD_RATE' PO_DIST_RATE_COLUMN,
                        15 LO_BG_TIME, 15 SQL_PEV_DAYS, 15 SQL_FOL_DAYS,
                        'N' MO_DISABLE_PLAYSTORE, 'N' MO_SAVE_DATA, 'N' IS_PHARMA
                    FROM COMPANY_SETUP)";
                    count += _objectEntity.ExecuteSqlCommand(preferenceQuery);

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new Exception(ex.Message);
                }
            }
            return count;
        }



        public List<TroubleshootModel> GetTroubleshoot(User UserInfo)
        {

            string query = $@"SELECT DIST.FULL_NAME,DIST.ATTENDANCE,DIST.SP_CODE,DIST.EXPIRY_DATE,DIST.COMPANY_EDESC AS DIST_COMPANY,
                               DIST.GROUP_EDESC,EMP.COMPANY_EDESC AS SYN_COMPANY
                             FROM 
                               (SELECT DLU.FULL_NAME,DLU.SP_CODE,DLU.EXPIRY_DATE,DLU.ATTENDANCE,DLU.COMPANY_CODE,
                               CS.COMPANY_EDESC,DGM.GROUP_EDESC
                               FROM DIST_LOGIN_USER DLU
                               INNER JOIN COMPANY_SETUP CS ON DLU.COMPANY_CODE = CS.COMPANY_CODE
                               INNER JOIN DIST_GROUP_MASTER DGM ON DLU.GROUPID = DGM.GROUPID
                               WHERE DLU.USER_TYPE = 'S' AND DLU.ACTIVE = 'Y') DIST
                               INNER JOIN
                               (SELECT HES.EMPLOYEE_CODE,HES.COMPANY_CODE,CS.COMPANY_EDESC 
                               FROM HR_EMPLOYEE_SETUP HES
                               INNER JOIN COMPANY_SETUP CS ON HES.COMPANY_CODE = CS.COMPANY_CODE
                              WHERE HES.GROUP_SKU_FLAG = 'I' AND HES.DELETED_FLAG = 'N') EMP ON DIST.SP_CODE = EMP.EMPLOYEE_CODE";
            var data = _objectEntity.SqlQuery<TroubleshootModel>(query).ToList();
            return data;
        }

        public List<BrandItemModel> GetBrandItem(User userInfo)
        {
            string item_filter_condition = "";
            try
            {
                var url = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                string xmlpath = HttpContext.Current.Server.MapPath("~/Areas/NeoERP.Distribution/ProductCondition.xml");
                var xml = XDocument.Load(xmlpath);
                var condition_query = from c in xml.Root.Descendants("Vendor")
                                      where (string)c.Attribute("ModuleName") == "Distribution"
                                      select c.Element("ConditionQuery").Value;
                var result = condition_query.FirstOrDefault();
                if (result != null)
                {
                    item_filter_condition = result;
                }
            }
            catch (Exception)
            {
                item_filter_condition = "";
            }

            string query = $@"SELECT IMS.ITEM_CODE, IMS.ITEM_EDESC,ISS.BRAND_NAME
                   FROM IP_ITEM_SPEC_SETUP ISS
                   INNER JOIN IP_ITEM_MASTER_SETUP IMS ON IMS.ITEM_CODE = ISS.ITEM_CODE AND IMS.COMPANY_CODE =ISS.COMPANY_CODE
                   WHERE IMS.DELETED_FLAG = 'N'  AND IMS.GROUP_SKU_FLAG ='I'
                   AND IMS.COMPANY_CODE='{userInfo.company_code}' {item_filter_condition}
                   ORDER BY IMS.ITEM_CODE";

            var data = _objectEntity.SqlQuery<BrandItemModel>(query).ToList();
            return data;




        }

        public string UpdateCreatedByNameReseller(string Company_code)
        {
            string query = $@"update dist_reseller_master set created_by_name=(select full_name from dist_login_user where userid=dist_reseller_master.created_by) where  created_by_name is null";
            var rowaffected = _objectEntity.ExecuteSqlCommand(query);
            if (rowaffected > 0)
            {
                var resellerCreatedBy = $@" update dist_reseller_master set created_by_name=(select login_edesc from sc_application_users where USER_NO=dist_reseller_master.created_by and company_code='{Company_code}') where  created_by_name is null";
                var rowaffectedmain = _objectEntity.ExecuteSqlCommand(resellerCreatedBy);
                if (rowaffectedmain > 0)
                    return "Done";

            }
            return "Not affected";
        }

        public string RemoveDuplicateRoutes(string Company_code)
        {
            try
            {

                string query = $@"create table dist_route_detail_unique AS SELECT DISTINCT route_code, emp_code, plan_code, assign_date, created_date, created_by, approve_date, approve_by, modify_date, modify_by, deleted_flag, company_code, branch_code FROM dist_route_detail";
                var uniqueRoutes = _objectEntity.ExecuteSqlCommand(query);

                if (uniqueRoutes < 0)
                {
                    DataTable dt = _objectEntity.SqlQuery("select table_name from user_tables where lower(table_name) = 'dist_route_detail_bk'"); //To check if the table exists
                    if (dt.Rows.Count > 0)

                    {
                        var truncateBackupTable = _objectEntity.ExecuteSqlCommand($@"truncate table dist_route_detail_bk");

                    }
                    else
                    {
                        var makeDataBackupTable = _objectEntity.ExecuteSqlCommand($@"create table dist_route_detail_bk as select * from dist_route_detail where 1=2");

                    }

                    var makeDataBackup = _objectEntity.ExecuteSqlCommand($@"insert into dist_route_detail_bk select * from dist_route_detail");

                    var truncateTable = _objectEntity.ExecuteSqlCommand($@"truncate table dist_route_detail");

                    var insertDistinctRoute = $@"INSERT INTO dist_route_detail (route_code, emp_code, plan_code, assign_date, created_date, created_by, approve_date, approve_by, modify_date, modify_by, deleted_flag, company_code, branch_code) select route_code, emp_code, plan_code, assign_date, created_date, created_by, approve_date, approve_by, modify_date, modify_by, deleted_flag, company_code, branch_code from dist_route_detail_unique";

                    var rowaffectedmain = _objectEntity.ExecuteSqlCommand(insertDistinctRoute);
                    if (rowaffectedmain > 0)
                    {

                        var truncateTempTable = _objectEntity.ExecuteSqlCommand($@"drop table dist_route_detail_unique");

                        return "" + rowaffectedmain + " Unique Routes Inserted";
                    }
                }

                return "Not affected";
            }


            catch (Exception ex)
            {
                var dropUniqueTable = _objectEntity.ExecuteSqlCommand($@"drop table dist_route_detail_unique");
                return "Unique Table Dropped. Please Try again !";

            }


        }

           


            //    var droptable = _objectEntity.ExecuteSqlCommand($@"drop table dist_route_detail_unique");
            //    //string query = $@"create table dist_route_detail_unique as SELECT DISTINCT route_code, emp_code, plan_code, assign_date, created_date, created_by, approve_date, approve_by, modify_date, modify_by, deleted_flag, company_code, branch_code FROM dist_route_detail";
            //    string query = $@"create table dist_route_detail_unique as SELECT  * FROM dist_route_detail ORDER BY route_code, emp_code, plan_code, assign_date, created_date, created_by, approve_date, approve_by, modify_date, modify_by, deleted_flag, company_code, branch_code";
            //    var uniqueRoutes = _objectEntity.ExecuteSqlCommand(query);
            //    if (uniqueRoutes < 0)
            //    {

            //        var truncateTable = _objectEntity.ExecuteSqlCommand($@"truncate table dist_route_detail");

            //        var insertDistinctRoute = $@"insert into dist_route_detail select * from dist_route_detail_unique";

            //        var rowaffectedmain = _objectEntity.ExecuteSqlCommand(insertDistinctRoute);
            //        if (rowaffectedmain > 0)
            //        {

            //            var truncateTempTable = _objectEntity.ExecuteSqlCommand($@"drop table dist_route_detail_unique");

            //            return "Unique Routes Inserted";
            //        }
            //    }
            //    return "Not affected";
            //}
        }

    }

