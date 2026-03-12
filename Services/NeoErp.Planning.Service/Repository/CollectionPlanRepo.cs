using System;
using NeoErp.Core;
using NeoErp.Core.Models;
using NeoErp.Data;
using NeoErp.Planning.Service.Interface;
using NeoErp.Planning.Service.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using NeoErp.Core.Domain;

namespace NeoErp.Planning.Service.Repository
{
   public class CollectionPlanRepo: ICollectionPlanRepo
    {
        private IDbContext _dbContext;
        private IWorkContext _workcontext;
        public CollectionPlanRepo(IDbContext dbContext, IWorkContext workContext)
        {
            this._workcontext = workContext;
            this._dbContext = dbContext;
        }
        #region DAYS AND MONTH
        private List<FrequencyColumnModel> getAllDaysBetweenDateRange(string dateFormat, string time_frame_edesc, string sTART_DATE, string eND_DATE)
        {
            List<FrequencyColumnModel> allDaysObj = new List<FrequencyColumnModel>();
            if (time_frame_edesc.ToLower() == "week")
            {
                var weekQuery = $@"SELECT DISTINCT TO_CHAR(DAYSITEM+(ROWNUM-1+7)/7,'IW')  WEEKS 
                                            ,TO_CHAR(DAYSITEM,'MM') MONTHINT
                                            ,TO_CHAR(DAYSITEM,'MON') MONTH
                                            ,TO_CHAR(DAYSITEM,'YYYY') YEAR 
                                            ,TO_CHAR(DAYSITEM,'YYYYMM') YEARMONTH 
                                            ,TO_CHAR(DAYSITEM,'DD-MON-YYYY') DAYS
                                              FROM (SELECT FISCAL_YEAR.START_DATE + ROWNUM - 1 DAYSITEM FROM ALL_OBJECTS 
                                              ,(SELECT TO_DATE('{sTART_DATE}','YYYY-Mon-DD') START_DATE, TO_DATE('{eND_DATE}','YYYY-Mon-DD') END_DATE FROM DUAL) FISCAL_YEAR
                                              WHERE ROWNUM <= FISCAL_YEAR.END_DATE - FISCAL_YEAR.START_DATE + 1) ALLDAYOFYEAR
                                            ORDER BY 
                                            TO_CHAR(DAYSITEM,'YYYYMM'),DAYS";
                allDaysObj = this._dbContext.SqlQuery<FrequencyColumnModel>(weekQuery).ToList();
            }
            else if (time_frame_edesc.ToLower() == "month")
            {
                var monthQuery = $@"SELECT 
                                TO_CHAR(DAYSITEM,'MM') MONTHINT
                                ,TO_CHAR(DAYSITEM,'MON') MONTH
                                ,TO_CHAR(DAYSITEM,'YYYY') YEAR 
                                ,TO_CHAR(DAYSITEM,'YYYYMM') YEARMONTH 
                                ,TO_CHAR(DAYSITEM,'DD-MON-YYYY') DAYS
                                ,TO_CHAR(DAYSITEM,'DD') DAY
                                  FROM (SELECT FISCAL_YEAR.START_DATE + ROWNUM - 1 DAYSITEM FROM ALL_OBJECTS 
                                  ,(SELECT TO_DATE('{sTART_DATE}','YYYY-Mon-DD') START_DATE, TO_DATE('{eND_DATE}','YYYY-Mon-DD') END_DATE FROM DUAL) FISCAL_YEAR
                                  WHERE ROWNUM <= FISCAL_YEAR.END_DATE - FISCAL_YEAR.START_DATE + 1) ALLDAYOFYEAR
                                ORDER BY 
                                TO_CHAR(DAYSITEM,'YYYYMM'),DAYS";
                allDaysObj = this._dbContext.SqlQuery<FrequencyColumnModel>(monthQuery).ToList();
            }
            return allDaysObj;
        }
        private List<YearMonthWeekModel> getAllYearMonthWeek(string dateFormat, string tIME_FRAME_EDESC, string sTART_DATE, string eND_DATE)
        {
            List<YearMonthWeekModel> allYearMonthWeekObj = new List<YearMonthWeekModel>();
            if (dateFormat == "LOC" || dateFormat == "BS")
            {
                if (tIME_FRAME_EDESC.ToLower() == "week")
                {
                    string engDateQuery = $@"select DISTINCT yearWeek,to_char(nweek) nweek,to_char(eweek) eweek from (SELECT  FLOOR (nepali_week / 7) + 1 nweek, days, TO_CHAR (days+1, 'IW') eweek, bs_date(days) nepali_date,  fn_bs_month(substr(bs_date(days),6,2)) as NMONTH--, to_char(days,'day') dd
                                                 ,(TO_CHAR(days,'YYYY')||'-'|| TO_CHAR (days+1, 'IW')) yearWeek                                                      
                                                    FROM (SELECT (  fiscal_year.start_date
                                                                    + ROWNUM
                                                                    - 1
                                                                    - NEXT_DAY (start_date, 'SUN'))
                                                                      nepali_week,
                                                                   TO_DATE (fiscal_year.start_date + ROWNUM - 1) days
                                                              FROM hr_fiscal_year_code fiscal_year, all_objects
                                                             WHERE sysdate BETWEEN start_date AND end_date
                                                                   AND ROWNUM <=
                                                                          fiscal_year.end_date - fiscal_year.start_date + 1)
                                                     WHERE nepali_week >= 0)
                                                     where days between TO_DATE('{sTART_DATE}','YYYY-Mon-DD') and TO_DATE('{eND_DATE}','YYYY-Mon-DD')";
                    allYearMonthWeekObj = this._dbContext.SqlQuery<YearMonthWeekModel>(engDateQuery).ToList();
                }
                else
                {
                    string engDateQuery = $@"SELECT  TO_CHAR(AD_DATE,'YYYY-MM-DD') yearWeek,  TO_CHAR(AD_DATE+DAYS_NO-1,'YYYY-MM-DD')yearWeek1,TO_CHAR(BS_MONTH) BS_MONTH FROM CALENDAR_SETUP ";
                    allYearMonthWeekObj = this._dbContext.SqlQuery<YearMonthWeekModel>(engDateQuery).ToList();
                }

            }
            return allYearMonthWeekObj;
        }
        private List<FrequencyColumnModel> getAllDaysByNepaliYearMonth(string dateFormat, string tIME_FRAME_EDESC, string sTART_DATE, string eND_DATE)
        {
            List<FrequencyColumnModel> allYearMonthWeekObj = new List<FrequencyColumnModel>();
            if (dateFormat == "LOC" || dateFormat == "BS")
            {
                if (tIME_FRAME_EDESC.ToLower() == "week")
                {
                    string engDateQuery = $@"select DISTINCT yearWeek,to_char(nweek) nweek,to_char(eweek) eweek from (SELECT  FLOOR (nepali_week / 7) + 1 nweek, days, TO_CHAR (days+1, 'IW') eweek, bs_date(days) nepali_date,  fn_bs_month(substr(bs_date(days),6,2)) as NMONTH--, to_char(days,'day') dd
                                                 ,(TO_CHAR(days,'YYYY')||'-'|| TO_CHAR (days+1, 'IW')) yearWeek                                                      
                                                    FROM (SELECT (  fiscal_year.start_date
                                                                    + ROWNUM
                                                                    - 1
                                                                    - NEXT_DAY (start_date, 'SUN'))
                                                                      nepali_week,
                                                                   TO_DATE (fiscal_year.start_date + ROWNUM - 1) days
                                                              FROM hr_fiscal_year_code fiscal_year, all_objects
                                                             WHERE sysdate BETWEEN start_date AND end_date
                                                                   AND ROWNUM <=
                                                                          fiscal_year.end_date - fiscal_year.start_date + 1)
                                                     WHERE nepali_week >= 0)
                                                     where days between TO_DATE('{sTART_DATE}','YYYY-Mon-DD') and TO_DATE('{eND_DATE}','YYYY-Mon-DD')";
                    allYearMonthWeekObj = this._dbContext.SqlQuery<FrequencyColumnModel>(engDateQuery).ToList();
                }
                else
                {
                    var query = $@"SELECT 
                                TO_CHAR(DAYSITEM,'MM') MONTHINT
                                ,TO_CHAR(DAYSITEM,'MON') MONTH
                                ,TO_CHAR(DAYSITEM,'YYYY') YEAR 
                                ,TO_CHAR(DAYSITEM,'YYYYMM') YEARMONTH 
                                ,TO_CHAR(DAYSITEM,'DD-MON-YYYY') DAYS
                                ,TO_CHAR(DAYSITEM,'DD') DAY
                                ,substr(bs_date(daysitem),0,7) YEARWEEK
                                  FROM (SELECT FISCAL_YEAR.START_DATE + ROWNUM - 1 DAYSITEM FROM ALL_OBJECTS 
                                  ,(SELECT TO_DATE('{sTART_DATE}','YYYY-Mon-DD') START_DATE, TO_DATE('{eND_DATE}','YYYY-Mon-DD') END_DATE FROM DUAL) FISCAL_YEAR
                                  WHERE  ROWNUM <= FISCAL_YEAR.END_DATE - FISCAL_YEAR.START_DATE + 1) ALLDAYOFYEAR  
                                ORDER BY 
                                TO_CHAR(DAYSITEM,'YYYYMM'),DAYS";
                    allYearMonthWeekObj = this._dbContext.SqlQuery<FrequencyColumnModel>(query).ToList();
                }

            }
            return allYearMonthWeekObj;

        }
        #endregion
        #region Collection Plan
        public string SaveCollectionPlan(List<savePlan> sv, CollectionPlan sp)
        {
            try
            {
                var EST_AMOUNT = sp.SALES_AMOUNT;
                var userID = _workcontext.CurrentUserinformation.User_id;
                var branchCode = _workcontext.CurrentUserinformation.branch_code;
                var spbranchCode = sp.branchCode == "" ? _workcontext.CurrentUserinformation.branch_code : sp.branchCode;
                var companyCode = _workcontext.CurrentUserinformation.company_code;
                var result = "";
                int planCode = 0;
                string time_frame_code = sp.TIME_FRAME_CODE;

                var company_code = _workcontext.CurrentUserinformation.company_code;

                // first insertion to PL_BRD_COLLECTION_PLAN table
                string checkPlanQuery = $@"SELECT COUNT(*) FROM PL_BRD_COLLECTION_PLAN WHERE PLAN_EDESC='{sp.PLAN_EDESC}' 
                AND TIME_FRAME_CODE='{sp.TIME_FRAME_CODE}' AND START_DATE=TO_DATE('{sp.START_DATE}','YYYY-Mon-DD') AND END_DATE=TO_DATE('{sp.END_DATE}','YYYY-Mon-DD') AND DELETED_FLAG='N'";
                var checkPlanQueryResult = this._dbContext.SqlQuery<int>(checkPlanQuery).First();

                var ref_flag = sp.REFERENCE_FLAG;
                if (ref_flag != "")
                {
                    var rf = sp.REFERENCE_FLAG.Split('_');
                    if (rf.Length > 1)
                        ref_flag = rf[1];
                    else
                        ref_flag = "";
                }

                if (checkPlanQueryResult == 0 && string.IsNullOrEmpty(sp.PLAN_CODE))
                {
                    string sales_plan_query = $@"INSERT INTO PL_BRD_COLLECTION_PLAN(PLAN_CODE,PLAN_EDESC,PLAN_NDESC,EST_AMOUNT,TIME_FRAME_CODE,
                    CALENDAR_TYPE,START_DATE,END_DATE,REMARKS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,
                    LAST_MODIFIED_BY,LAST_MODIFIED_DATE,
                    DELETED_FLAG,REFERENCE_FLAG,REFERENCE_PLAN_CODE)
                    VALUES((SELECT COALESCE(MAX(PLAN_CODE)+1, MAX(PLAN_CODE) + 1, 1) FROM PL_BRD_COLLECTION_PLAN),'{sp.PLAN_EDESC}','','{EST_AMOUNT}','{sp.TIME_FRAME_CODE}','{sp.CALENDAR_TYPE}',
                    TO_DATE('{sp.START_DATE}','YYYY-Mon-DD'),TO_DATE('{sp.END_DATE}','YYYY-Mon-DD'),'{sp.REMARKS}','{companyCode}','{branchCode}','{userID}',
                    TO_DATE('{DateTime.Today.ToString("MM/dd/yyyy")}','MM/DD/YYYY'),'{userID}',TO_DATE('{DateTime.Today.ToString("MM/dd/yyyy")}','MM/DD/YYYY'),'N','{ref_flag}','{sp.REFERENCE_PLAN_CODE}')";

                    var insertResult = this._dbContext.ExecuteSqlCommand(sales_plan_query);

                    string fetchProcurementPlan = $@"SELECT PLAN_CODE FROM PL_BRD_COLLECTION_PLAN WHERE PLAN_EDESC='{sp.PLAN_EDESC}' 
                AND TIME_FRAME_CODE='{sp.TIME_FRAME_CODE}' AND START_DATE=TO_DATE('{sp.START_DATE}','YYYY-Mon-DD') AND END_DATE=TO_DATE('{sp.END_DATE}','YYYY-Mon-DD')";
                    planCode = this._dbContext.SqlQuery<int>(fetchProcurementPlan).First();

                }
                else
                {
                    if (!string.IsNullOrEmpty(sp.PLAN_CODE) && sp.PLAN_CODE != "0")
                    {
                        planCode = Convert.ToInt32(sp.PLAN_CODE);
                        string updateQuery = $@"UPDATE PL_BRD_COLLECTION_PLAN SET PLAN_EDESC = '{sp.PLAN_EDESC}' , REMARKS = '{sp.REMARKS}' 
                        ,TIME_FRAME_CODE='{sp.TIME_FRAME_CODE}' , CALENDAR_TYPE='{sp.CALENDAR_TYPE}', START_DATE=TO_DATE('{sp.START_DATE}','YYYY-MON-DD') , END_DATE=TO_DATE('{sp.END_DATE}','YYYY-MON-DD')
                        WHERE PLAN_CODE='{sp.PLAN_CODE}'";
                        var update_result = this._dbContext.ExecuteSqlCommand(updateQuery);
                    }
                    else
                    {
                        //string plancode_query = $@"SELECT PLAN_CODE FROM PL_BRD_COLLECTION_PLAN WHERE PLAN_EDESC='{sp.PLAN_EDESC}' 
                        //AND TIME_FRAME_CODE='{sp.TIME_FRAME_CODE}' AND START_DATE=TO_DATE('{sp.START_DATE}','YYYY-Mon-DD') AND END_DATE=TO_DATE('{sp.END_DATE}','YYYY-Mon-DD')";
                        //planCode = this._dbContext.SqlQuery<int>(plancode_query).First();
                        throw new Exception("Plan Name Already Exist!!");
                    }

                    string delete_detail_already_set = $@"DELETE FROM PL_BRD_COLLECTION_PLAN_DTL WHERE PLAN_CODE='{planCode}'";
                    this._dbContext.ExecuteSqlCommand(delete_detail_already_set);
                }

                string sbInsertQuery = string.Empty;

                List<FrequencyColumnModel> allDaysObj = new List<FrequencyColumnModel>();
                List<YearMonthWeekModel> allYearWeekObj = new List<YearMonthWeekModel>();

                //eachday from start to end date
                allDaysObj = getAllDaysBetweenDateRange(sp.dateFormat, sp.TIME_FRAME_EDESC, sp.START_DATE, sp.END_DATE);
                //all year-month or year-week list from start to end date
                allYearWeekObj = getAllYearMonthWeek(sp.dateFormat, sp.TIME_FRAME_EDESC, sp.START_DATE, sp.END_DATE);


                //
                string[] insert_query_array = new string[sv.Count()];
                int itemCount = 0,
                    itemRange = 5,
                    array_count = 0,
                    itteration = 0,
                    remainderCount = 0;
                remainderCount = (sv.Count() % itemRange);
                foreach (var item in sv)
                {
                    itemCount++;
                    itteration++;
                    // start sales price per item.
                    string averagePriceQuery = string.Empty;
                    var salesPriceResult = string.Empty;

                    salesPriceResult = "1";

                    foreach (var freq in item.frequency)
                    {
                        try
                        {
                            string[] freq_date = null;
                            int interval = 0;
                            string freValue = freq.fname.Split('_')[0]; // month or week
                            string freYear = freq.fname.Split('_')[1]; // year

                            string freYearVal = freYear + '-' + freValue;

                            List<FrequencyColumnModel> allYearByNYM = new List<FrequencyColumnModel>();
                            allYearByNYM = getAllDaysByNepaliYearMonth(sp.dateFormat, sp.TIME_FRAME_CODE, sp.START_DATE, sp.END_DATE);
                            if (sp.TIME_FRAME_EDESC.ToLower() == "week" && (sp.dateFormat.ToLower() == "ad" || sp.dateFormat.ToLower() == "eng"))
                            {
                                freYear = freq.fname.Split('_')[0];
                                freValue = freq.fname.Split('_')[1];
                                freYearVal = freYear + '-' + freValue;
                            }
                            if (sp.dateFormat.ToLower() == "loc" || sp.dateFormat.ToLower() == "bs")
                            {
                                if (sp.TIME_FRAME_EDESC.ToLower() == "week")
                                {
                                    string yearmonth = allYearWeekObj.Where(a => a.NWEEK == freYear).First().YEARWEEK;
                                    //string yearmonth = allYearWeekObj.Where(a => a.NWEEK == freYear).First().BS_MONTH;
                                    freValue = yearmonth.Split('-')[1];
                                    freYear = yearmonth.Split('-')[0];
                                    freYearVal = freYear + '-' + freValue;
                                }
                                else
                                {
                                    //string bsyearmonth = allYearWeekObj.Where(a => a.BS_MONTH == freYear + "-" + freValue).First().YEARWEEK;
                                    string bsyearmonth = allYearWeekObj.Where(a => a.BS_MONTH == freYear + "-" + freValue).First().BS_MONTH;
                                    freValue = bsyearmonth.Split('-')[1];
                                    freYear = bsyearmonth.Split('-')[0];
                                    freYearVal = freYear + '-' + freValue;
                                }
                            }

                            if (sp.TIME_FRAME_EDESC.Trim().ToLower() == "week")
                            {
                                freq_date = allDaysObj.Where(a => Convert.ToInt32(a.WEEKS) == Convert.ToInt32(freValue) && a.YEAR == freYear).Select(a => a.DAYS).ToArray();
                            }
                            else if (sp.TIME_FRAME_EDESC.Trim().ToLower() == "month")
                            {
                                freq_date = allDaysObj.Where(a => Convert.ToInt32(a.MONTHINT) == Convert.ToInt32(freValue) && a.YEAR == freYear).Select(a => a.DAYS).ToArray();
                                if (sp.dateFormat.ToLower() == "loc" || sp.dateFormat.ToLower() == "bs")
                                {
                                    freq_date = allYearByNYM.Where(a => a.YEARWEEK == freYearVal).Select(a => a.DAYS).ToArray();
                                }
                            }
                            interval = freq_date.Length;
                            foreach (var date in freq_date)
                            {
                                var eachday_value = Math.Round((Convert.ToDecimal(freq.fvalue) / interval), 5);

                                string frequency_json = @"fname__" + freq.fname + "__fvalue__" + freq.fvalue;
                                string insertinto_plandtl = $@"SELECT '{planCode}',TO_DATE('{date}','DD-MON-YYYY'),'{eachday_value}','{item.itemCode}',
                                '{sp.divisionCode}','{frequency_json}','',
                                '{companyCode}','{spbranchCode}','{userID}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/DD/YYYY'),'','','N','{sp.employeeCode}','{item.itemCode}' FROM DUAL UNION ALL ";

                                sbInsertQuery += insertinto_plandtl;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (!ex.Message.Contains("unique constraint"))
                                throw;
                            else
                            {
                                var sqlquery = $@"UPDATE PL_BRD_COLLECTION_PLAN_DTL SET TARGET_VALUE='{freq.fvalue}',LAST_MODIFIED_BY='{userID}', LAST_MODIFIED_DATE=TO_DATE('{DateTime.Now:MM/dd/yyyy/ HH:mm:ss}', 'mm/dd/yyyy hh24:mi:ss') where PLAN_CODE='{planCode}' AND ITEM_CODE='{item.itemCode}' AND TIME_FRAME_CODE='{time_frame_code}' AND TIME_FRAME_VALUE='{freq.fname}'";
                            }
                        }
                    }
                    if (itteration <= (sv.Count() - remainderCount))
                    {
                        while (itemCount == itemRange)
                        {
                            string query = @"insert into PL_BRD_COLLECTION_PLAN_DTL(PLAN_CODE,PLAN_DATE,PER_DAY_AMOUNT,ITEM_CODE,
                                            DIVISION_CODE,FREQUENCY_JSON,REMARKS,
                                            COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,DELETED_FLAG,EMPLOYEE_CODE,CUSTOMER_CODE) " + sbInsertQuery;
                            query = query.Substring(0, query.Length - 11);
                            insert_query_array[array_count] = query;
                            array_count++;
                            itemCount = 0;
                            sbInsertQuery = string.Empty;

                        }
                    }
                    else
                    {
                        while (itemCount == remainderCount)
                        {
                            string query = @"insert into PL_BRD_COLLECTION_PLAN_DTL(PLAN_CODE,PLAN_DATE,PER_DAY_AMOUNT,ITEM_CODE,
                    DIVISION_CODE,FREQUENCY_JSON,REMARKS,
                    COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,DELETED_FLAG,EMPLOYEE_CODE,CUSTOMER_CODE) " + sbInsertQuery;
                            query = query.Substring(0, query.Length - 11);
                            insert_query_array[array_count] = query;
                            array_count++;
                            itemCount = 0;
                            sbInsertQuery = string.Empty;
                        }
                    }
                }
                try
                {
                    var insertItems = insert_query_array.Where(a => a != null);
                    foreach (var item in insertItems)
                    {
                        var a = item.ToString();
                        result = _dbContext.ExecuteSqlCommand(item.ToString()).ToString();

                    }
                }
                catch (Exception ex)
                {
                    if (!ex.Message.Contains("unique constraint"))
                        throw;
                }

                string update_amout_quantity = string.Empty;
                update_amout_quantity = $@"UPDATE PL_BRD_COLLECTION_PLAN PSP SET PSP.EST_AMOUNT = (
                        SELECT ROUND(SUM(SPD.PER_DAY_AMOUNT),2) FROM PL_BRD_COLLECTION_PLAN_DTL SPD, SA_CUSTOMER_SETUP IMS
                        WHERE 
                        IMS.GROUP_SKU_FLAG ='I'
                        AND SPD.COMPANY_CODE = IMS.COMPANY_CODE
                        AND SPD.ITEM_CODE = IMS.CUSTOMER_CODE
                        AND
                        SPD.PLAN_CODE='{planCode}')
                        WHERE PSP.PLAN_CODE='{planCode}'";
                this._dbContext.ExecuteSqlCommand(update_amout_quantity);
                return result.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CollectionPlan> getAllCollectionPlans(ReportFiltersModel filters)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var userId = _workcontext.CurrentUserinformation.User_id;
            string query = $@"SELECT DISTINCT TO_CHAR(PL.PLAN_CODE) PLAN_CODE, PL.PLAN_EDESC, PL.PLAN_NDESC,
                             TO_CHAR(PL.EST_AMOUNT) SALES_AMOUNT, TO_CHAR(PL.TIME_FRAME_CODE) TIME_FRAME_CODE, 
                             PL.CALENDAR_TYPE,TO_CHAR(PL.START_DATE,'DD-MON-YYYY') START_DATE,TO_CHAR(PL.END_DATE,'DD-MON-YYYY') END_DATE, PL.REMARKS,
                             PLD.DIVISION_CODE, TO_CHAR(PLD.BRANCH_CODE), DS.DIVISION_EDESC, FS.BRANCH_EDESC,PL.REFERENCE_FLAG ,ES.EMPLOYEE_CODE,ES.EMPLOYEE_EDESC--,CS.CUSTOMER_CODE,CS.CUSTOMER_EDESC
                             FROM PL_BRD_COLLECTION_PLAN PL, 
                             PL_BRD_COLLECTION_PLAN_DTL PLD, FA_DIVISION_SETUP DS, FA_BRANCH_SETUP FS ,HR_EMPLOYEE_SETUP ES,SA_CUSTOMER_SETUP CS
                             WHERE PL.DELETED_FLAG = 'N' AND PL.PLAN_CODE = PLD.PLAN_CODE AND PL.COMPANY_CODE=PLD.COMPANY_CODE
                             AND ES.EMPLOYEE_CODE=PLD.EMPLOYEE_CODE AND ES.COMPANY_CODE=PLD.COMPANY_CODE --AND CS.CUSTOMER_CODE=PLD.CUSTOMER_CODE and CS.COMPANY_CODE=PLD.COMPANY_CODE
                             AND DS.DIVISION_CODE(+) = PLD.DIVISION_CODE AND FS.BRANCH_CODE (+)= PLD.BRANCH_CODE
                             AND PL.COMPANY_CODE='{companyCode}'
                             AND PL.START_DATE BETWEEN TO_DATE('{filters.FromDate}','YYYY-MON-DD') AND TO_DATE('{filters.ToDate}', 'YYYY-MON-DD')
                             AND PL.END_DATE BETWEEN TO_DATE('{filters.FromDate}','YYYY-MON-DD') AND TO_DATE('{filters.ToDate}', 'YYYY-MON-DD')";

            if (filters.BranchFilter.Count > 0)
            {
                query += $@" AND PLD.BRANCH_CODE IN ('{string.Join("','", filters.BranchFilter)}')";

            }
            if (filters.DivisionFilter.Count > 0)
            {
                query += $@" AND PLD.DIVISION_CODE IN ('{string.Join("','", filters.DivisionFilter)}')";

            }
            //if(filters.CustomerFilter.Count>0)
            //{
            //    query += $@" AND PLD.CUSTOMER_CODE IN ('{string.Join("','", filters.CustomerFilter)}')";
            //}
            if (filters.EmployeeFilter.Count > 0)
            {
                query += $@" AND PLD.EMPLOYEE_CODE IN ('{string.Join("','", filters.EmployeeFilter)}')";
            }
            query += $@" ORDER BY TO_NUMBER (PL.PLAN_CODE)  DESC";

            List<CollectionPlan> spList = new List<CollectionPlan>();

            spList = this._dbContext.SqlQuery<CollectionPlan>(query).ToList();
            return spList;
        }

        public List<CollectionPlan> getAllCollectionPlans(string customercode, string employeecode, string divisioncode, string branchcode, string startdate, string enddate)
        {
            var userno = this._workcontext.CurrentUserinformation.User_id;
            var company_code = this._workcontext.CurrentUserinformation.company_code;
            string query = $@"SELECT DISTINCT TO_CHAR(PL.PLAN_CODE), TO_CHAR(PL.PLAN_CODE)||'_SP' PLAN_CODE, PL.PLAN_EDESC, PL.PLAN_NDESC,
                              CASE WHEN PL.SALES_QUANTITY IS NULL 
                                                    THEN 'Amt'
                                                ELSE
                                                   'Qty'
                                                END  SALES_TYPE,
                            CASE WHEN PL.SALES_QUANTITY IS NULL 
                                        THEN  TO_CHAR(PL.SALES_AMOUNT)
                                    ELSE
                                        TO_CHAR(PL.SALES_QUANTITY)
                                    END SALES_QUANTITY,
                            TO_CHAR(PL.TIME_FRAME_CODE) TIME_FRAME_CODE,
                            PL.CALENDAR_TYPE,TO_CHAR(PL.START_DATE,'DD-MON-YYYY') START_DATE,TO_CHAR(PL.END_DATE,'DD-MON-YYYY') END_DATE,
                            PL.REMARKS, PLD.CUSTOMER_CODE, PLD.DIVISION_CODE, PLD.BRANCH_CODE, PLD.EMPLOYEE_CODE, CS.CUSTOMER_EDESC, DS.DIVISION_EDESC, ES.EMPLOYEE_EDESC,
                            FS.BRANCH_EDESC
                            FROM PL_BRD_COLLECTION_PLAN PL, PL_BRD_COLLECTION_PLAN_DTL PLD, SA_CUSTOMER_SETUP CS , FA_DIVISION_SETUP DS, HR_EMPLOYEE_SETUP ES, FA_BRANCH_SETUP FS
                             WHERE PL.DELETED_FLAG = 'N' 
                             AND PL.PLAN_CODE = PLD.PLAN_CODE
                             AND PL.COMPANY_CODE=PLD.COMPANY_CODE
                             AND CS.CUSTOMER_CODE(+) = PLD.CUSTOMER_CODE
                             AND DS.DIVISION_CODE(+) = PLD.DIVISION_CODE
                             AND ES.EMPLOYEE_CODE (+)= PLD.EMPLOYEE_CODE
                             AND FS.BRANCH_CODE (+)= PLD.BRANCH_CODE
                            AND PLD.EMPLOYEE_CODE in (
                             SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO='{userno}' AND COMPANY_CODE='{company_code}'
                              UNION ALL
                             SELECT HES.EMPLOYEE_CODE FROM HR_EMPLOYEE_SETUP HES, HR_EMPLOYEE_TREE ET
                                WHERE HES.EMPLOYEE_CODE = ET.EMPLOYEE_CODE 
                                 AND PARENT_EMPLOYEE_CODE=(SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO='{userno}' AND COMPANY_CODE='{company_code}')
                                    START WITH PARENT_EMPLOYEE_CODE IS NULL
                                CONNECT BY PRIOR ET.EMPLOYEE_CODE  = PARENT_EMPLOYEE_CODE)
                             ";

            if (!string.IsNullOrEmpty(startdate) && !string.IsNullOrEmpty(enddate))
            {
                //query += $@" AND PL.START_DATE BETWEEN TO_DATE('{startdate}','YYYY-MON-DD') AND TO_DATE('{enddate}', 'YYYY-MON-DD')
                //             AND PL.END_DATE BETWEEN TO_DATE('{startdate}','YYYY-MON-DD') AND TO_DATE('{enddate}', 'YYYY-MON-DD')";
                query += $@" AND PL.START_DATE = TO_DATE('{startdate}','YYYY-MON-DD')
                             AND PL.END_DATE = TO_DATE('{enddate}','YYYY-MON-DD') ";
            }

            if (!string.IsNullOrEmpty(customercode))
            {
                query += $@" AND PLD.CUSTOMER_CODE IN ('{customercode}')";

            }
            if (!string.IsNullOrEmpty(branchcode))
            {
                query += $@" AND PLD.BRANCH_CODE IN ('{branchcode}')";

            }
            if (!string.IsNullOrEmpty(divisioncode))
            {
                query += $@" AND PLD.DIVISION_CODE IN ('{divisioncode}')";

            }
            if (!string.IsNullOrEmpty(employeecode))
            {
                query += $@" AND PLD.EMPLOYEE_CODE IN ('{employeecode}')";

            }

            query += $@" ORDER BY TO_NUMBER (PL.PLAN_CODE)  DESC";

            List<CollectionPlan> spList = new List<CollectionPlan>();
            spList = this._dbContext.SqlQuery<CollectionPlan>(query).ToList();

            string masterplan_query = $@"SELECT DISTINCT TO_CHAR(MSP.MASTER_PLAN_CODE)||'_MP' PLAN_CODE,
                                        MSP.MASTER_PLAN_EDESC PLAN_EDESC ,
                                        TO_CHAR (MSP.START_DATE, 'DD-MON-YYYY') START_DATE,
                                             TO_CHAR (MSP.END_DATE, 'DD-MON-YYYY') END_DATE
                                    FROM PL_MASTER_SALES_PLAN MSP ,PL_BRD_COLLECTION_PLAN_MAP SPM
                                    WHERE MSP.MASTER_PLAN_CODE=SPM.MASTER_PLAN_CODE ";
            if (!string.IsNullOrEmpty(startdate) && !string.IsNullOrEmpty(enddate))
            {
                masterplan_query += $@" AND MSP.START_DATE = TO_DATE('{startdate}','YYYY-MON-DD')
                             AND MSP.END_DATE = TO_DATE('{enddate}','YYYY-MON-DD') ";
            }
            List<CollectionPlan> mpList = new List<CollectionPlan>();
            mpList = this._dbContext.SqlQuery<CollectionPlan>(masterplan_query).ToList();
            if (mpList.Count > 0)
            {
                foreach (var item in mpList)
                {
                    spList.Add(new CollectionPlan
                    {
                        PLAN_CODE = item.PLAN_CODE,
                        PLAN_EDESC = item.PLAN_EDESC,
                        START_DATE = item.START_DATE,
                        END_DATE = item.END_DATE
                    });
                }
            }
            return spList;
        }

        public bool deleteCollectionPlan(int planCode)
        {
            try
            {
                string deleteYes_salesPlan = $@"UPDATE PL_BRD_COLLECTION_PLAN SET DELETED_FLAG='Y' WHERE PLAN_CODE='{planCode}'";
                string deleteYes_salesPlanDtl = $@"UPDATE PL_BRD_COLLECTION_PLAN_DTL SET DELETED_FLAG='Y' WHERE PLAN_CODE='{planCode}'";
                this._dbContext.ExecuteSqlCommand(deleteYes_salesPlanDtl);
                this._dbContext.ExecuteSqlCommand(deleteYes_salesPlan);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public PL_BRD_CLTN_PLAN GetPlanDetailValueByPlanCode(int plancode)
        {
            var company_code = this._workcontext.CurrentUserinformation.company_code;
            PL_BRD_CLTN_PLAN ProcurementPlan = new PL_BRD_CLTN_PLAN();
            string queryPlan = $@"SELECT 
                PLAN_CODE PLAN_CODE, PLAN_EDESC,PLAN_NDESC,EST_AMOUNT SALES_AMOUNT,TIME_FRAME_CODE TIME_FRAME_CODE,
                (SELECT TIME_FRAME_EDESC FROM PL_TIME_FRAME WHERE TIME_FRAME_CODE = PSP.TIME_FRAME_CODE) TIME_FRAME_EDESC,
                CALENDAR_TYPE,TO_CHAR(START_DATE) START_DATE,TO_CHAR(END_DATE) END_DATE,REMARKS,COMPANY_CODE,BRANCH_CODE
                , REFERENCE_FLAG,REFERENCE_PLAN_CODE FROM PL_BRD_COLLECTION_PLAN PSP
                WHERE PLAN_CODE= '{plancode}'";

            ProcurementPlan = this._dbContext.SqlQuery<PL_BRD_CLTN_PLAN>(queryPlan).FirstOrDefault();

            List<CollectionPlanDetail> salesPlanDetailList = new List<CollectionPlanDetail>();
            string ProcurementPlanDetailQuery = $@"SELECT TO_CHAR (SPD.PLAN_CODE) PLAN_CODE,
       SPD.PLAN_DATE PLAN_DATE,
      -- TO_CHAR (PER_DAY_QUANTITY) PER_DAY_QUANTITY,
       TO_CHAR (PER_DAY_AMOUNT) PER_DAY_AMOUNT,
       SPD.ITEM_CODE,
       SPD.CUSTOMER_CODE,
       SPD.EMPLOYEE_CODE,
       SPD.DIVISION_CODE,
       SPD.PARTY_TYPE_CODE,
       SPD.REMARKS,
       SPD.COMPANY_CODE,
       SPD.BRANCH_CODE,
       SPD.CREATED_BY,
       TO_CHAR (SPD.CREATED_DATE) CREATED_DATE,
       FREQUENCY_JSON,
       IMS.GROUP_SKU_FLAG
        FROM PL_BRD_COLLECTION_PLAN_DTL SPD, SA_CUSTOMER_SETUP IMS
         WHERE     IMS.CUSTOMER_CODE = SPD.ITEM_CODE
       AND IMS.COMPANY_CODE = SPD.COMPANY_CODE
       AND SPD.DELETED_FLAG = 'N'
       AND PLAN_CODE = '{plancode}'";
            salesPlanDetailList = this._dbContext.SqlQuery<CollectionPlanDetail>(ProcurementPlanDetailQuery).ToList();

            List<CollectionPlanItems> salesPlanItem = new List<CollectionPlanItems>();
            string ProcurementPlanItemsQuery = $@" SELECT DISTINCT TO_CHAR (PSPD.ITEM_CODE) ITEM_CODE,
                  INITCAP (IMS.CUSTOMER_EDESC) ITEM_EDESC,
                  TO_CHAR (IMS.MASTER_CUSTOMER_CODE) MASTER_ITEM_CODE,
                  TO_CHAR (IMS.PRE_CUSTOMER_CODE) PRE_ITEM_CODE,
                  TO_CHAR (IMS.GROUP_SKU_FLAG) GROUP_SKU_FLAG
                    FROM PL_BRD_COLLECTION_PLAN_DTL PSPD, SA_CUSTOMER_SETUP IMS
                   WHERE     PSPD.PLAN_CODE = '{plancode}'
                         AND PSPD.DELETED_FLAG = 'N'
                         AND IMS.DELETED_FLAG = 'N'
                         AND IMS.CUSTOMER_CODE = PSPD.ITEM_CODE
                         AND IMS.COMPANY_CODE = PSPD.COMPANY_CODE
                         AND PSPD.COMPANY_CODE = '{company_code}'
                ORDER BY PRE_ITEM_CODE, MASTER_ITEM_CODE, ITEM_CODE";
            salesPlanItem = this._dbContext.SqlQuery<CollectionPlanItems>(ProcurementPlanItemsQuery).ToList();

            ProcurementPlan.selectedItemsList = salesPlanItem;
            ProcurementPlan.salesPlanDetail = salesPlanDetailList;

            return ProcurementPlan;
        }

        public string getQueryForSalesReference(string itemList, string optionalCondition, string startDate, string endDate, string materialPlanCode, string referenceFlag)
        {
            var qry = "";
            if (referenceFlag == "FROM_MRP")
            {
                qry = $@" SELECT COLNAME,NEPALI_MONTH,NEPALI_YEAR,ITEM_CODE,ITEM_EDESC,QTY,
                                TO_CHAR((select sum(CAlC_QUANTITY) from PL_MATERIAL_PLAN_DTL where  plan_date between to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD') and
                                        item_code=t.item_code and plan_code in({materialPlanCode}) group by item_code)) TOTAL_QTY FROM
                                    (SELECT 'freqItemNum_'||SI.ITEM_CODE ||'_' ||  UPPER(fn_bs_month(SUBSTR(BS_DATE(SI.PLAN_DATE),6,2))) ||'_'|| SUBSTR(BS_DATE(SI.PLAN_DATE),0,4) COLNAME ,
                                     SUBSTR(BS_DATE(SI.PLAN_DATE),6,2)NEPALI_MONTH,
                                     SUBSTR(BS_DATE(SI.PLAN_DATE),0,4)NEPALI_YEAR,
                                     SI.ITEM_CODE,IMS.ITEM_EDESC,
                                     TO_CHAR(SUM (SI.CAlC_QUANTITY)) TOTAL_QTY,
                                     TO_CHAR(SUM (SI.CALC_QUANTITY)) QTY
                                FROM PL_MATERIAL_PLAN_DTL SI, IP_ITEM_MASTER_SETUP IMS
                                WHERE IMS.ITEM_CODE IN ({itemList}) {optionalCondition} 
                                AND SI.ITEM_CODE = IMS.ITEM_CODE
                                AND SI.COMPANY_CODE = IMS.COMPANY_CODE
                                AND IMS.GROUP_SKU_FLAG = 'I'
                                AND SI.PLAN_CODE IN ({materialPlanCode})
                                AND SI.PLAN_DATE BETWEEN to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD')
                            GROUP BY SI.ITEM_CODE,
                                     IMS.ITEM_EDESC,
                                     SUBSTR(BS_DATE(SI.PLAN_DATE),0,4),
                                     SUBSTR(BS_DATE(SI.PLAN_DATE),6,2)
                                    ORDER BY NEPALI_YEAR,ITEM_CODE,NEPALI_MONTH ASC)t";
            }
            else if (referenceFlag == "FROM_SALES")
            {
                qry = $@"SELECT COLNAME,NEPALI_MONTH,NEPALI_YEAR,ITEM_CODE,ITEM_EDESC,CUSTOMER_CODE,DIVISION_CODE,QTY,
                                TO_CHAR((select sum(PER_DAY_QUANTITY) from PL_SALES_PLAN_DTL where  plan_date between to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD') and
                                        item_code=t.item_code and plan_code IN ({materialPlanCode}) group by item_code)) TOTAL_QTY FROM
                                    (SELECT 'freqItemNum_'||SI.ITEM_CODE ||'_' ||  UPPER(fn_bs_month(SUBSTR(BS_DATE(SI.PLAN_DATE),6,2))) ||'_'|| SUBSTR(BS_DATE(SI.PLAN_DATE),0,4) COLNAME ,
                                     SUBSTR(BS_DATE(SI.PLAN_DATE),6,2)NEPALI_MONTH,
                                     SUBSTR(BS_DATE(SI.PLAN_DATE),0,4)NEPALI_YEAR,
                                     SI.ITEM_CODE,IMS.ITEM_EDESC,SI.CUSTOMER_CODE,
                                     SI.DIVISION_CODE,SI.BRANCH_CODE,
                                     TO_CHAR(SUM (SI.PER_DAY_QUANTITY)) TOTAL_QTY,
                                     TO_CHAR(SUM (SI.PER_DAY_QUANTITY)) QTY
                                FROM PL_SALES_PLAN_DTL SI, IP_ITEM_MASTER_SETUP IMS
                                WHERE IMS.ITEM_CODE IN ({itemList}) {optionalCondition} 
                                AND SI.ITEM_CODE = IMS.ITEM_CODE
                                AND SI.COMPANY_CODE = IMS.COMPANY_CODE
                                AND IMS.GROUP_SKU_FLAG = 'I'
                                AND SI.PLAN_CODE IN ({materialPlanCode})
                                AND SI.PLAN_DATE BETWEEN to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD')
                            GROUP BY SI.ITEM_CODE,
                                     IMS.ITEM_EDESC,
                                     SI.CUSTOMER_CODE,
                                     SI.DIVISION_CODE,
                                     SI.BRANCH_CODE,
                                     SUBSTR(BS_DATE(SI.PLAN_DATE),0,4),
                                     SUBSTR(BS_DATE(SI.PLAN_DATE),6,2)
                                    ORDER BY NEPALI_YEAR,ITEM_CODE,NEPALI_MONTH ASC)t";
            }
            else if (referenceFlag == "FROM_PRODUCTION")
            {
                qry = $@"   SELECT COLNAME,NEPALI_MONTH,NEPALI_YEAR,ITEM_CODE,ITEM_EDESC,DIVISION_CODE,QTY,
                                TO_CHAR((select sum(PER_DAY_QUANTITY) from PL_PRO_PLAN_DTL where  plan_date between to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD') and
                                        item_code=t.item_code and plan_code IN ({materialPlanCode}) group by item_code)) TOTAL_QTY FROM
                                    (SELECT 'freqItemNum_'||SI.ITEM_CODE ||'_' ||  UPPER(fn_bs_month(SUBSTR(BS_DATE(SI.PLAN_DATE),6,2))) ||'_'|| SUBSTR(BS_DATE(SI.PLAN_DATE),0,4) COLNAME ,
                                     SUBSTR(BS_DATE(SI.PLAN_DATE),6,2)NEPALI_MONTH,
                                     SUBSTR(BS_DATE(SI.PLAN_DATE),0,4)NEPALI_YEAR,
                                     SI.ITEM_CODE,IMS.ITEM_EDESC,SI.DIVISION_CODE,SI.BRANCH_CODE,
                                     TO_CHAR(SUM (SI.PER_DAY_QUANTITY)) TOTAL_QTY,
                                     TO_CHAR(SUM (SI.PER_DAY_QUANTITY)) QTY
                                FROM PL_PRO_PLAN_DTL SI, IP_ITEM_MASTER_SETUP IMS
                                WHERE IMS.ITEM_CODE IN ({itemList}) {optionalCondition}  
                                AND SI.ITEM_CODE = IMS.ITEM_CODE
                                AND SI.COMPANY_CODE = IMS.COMPANY_CODE
                                AND IMS.GROUP_SKU_FLAG = 'I'
                                AND SI.PLAN_CODE IN ({materialPlanCode})
                                AND SI.PLAN_DATE BETWEEN to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD')
                            GROUP BY SI.ITEM_CODE,
                                     IMS.ITEM_EDESC,
                                     SI.DIVISION_CODE,
                                     SI.BRANCH_CODE,
                                     SUBSTR(BS_DATE(SI.PLAN_DATE),0,4),
                                     SUBSTR(BS_DATE(SI.PLAN_DATE),6,2)
                                    ORDER BY NEPALI_YEAR,ITEM_CODE,NEPALI_MONTH ASC)t";
            }
            else
            {
                qry = $@"SELECT COLNAME,NEPALI_MONTH,NEPALI_YEAR,ITEM_CODE,ITEM_EDESC,CUSTOMER_CODE,DIVISION_CODE,QTY,
                                TO_CHAR((select sum(PER_DAY_QUANTITY) from PL_SALES_PLAN_DTL where  plan_date between to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD') and
                                        item_code=t.item_code group by item_code)) TOTAL_QTY FROM
                                    (SELECT 'freqItemNum_'||SI.ITEM_CODE ||'_' ||  UPPER(fn_bs_month(SUBSTR(BS_DATE(SI.PLAN_DATE),6,2))) ||'_'|| SUBSTR(BS_DATE(SI.PLAN_DATE),0,4) COLNAME ,
                                     SUBSTR(BS_DATE(SI.PLAN_DATE),6,2)NEPALI_MONTH,
                                     SUBSTR(BS_DATE(SI.PLAN_DATE),0,4)NEPALI_YEAR,
                                     SI.ITEM_CODE,
                                     IMS.ITEM_EDESC,
                                     SI.CUSTOMER_CODE,
                                     SI.DIVISION_CODE,
                                     SI.BRANCH_CODE,
                                    -- TO_CHAR(SUM (SI.CALC_TOTAL_PRICE)) AMOUNT,
                                     TO_CHAR(SUM (SI.PER_DAY_QUANTITY)) TOTAL_QTY,
                                     TO_CHAR(SUM (SI.PER_DAY_QUANTITY)) QTY
                                FROM PL_SALES_PLAN_DTL SI, IP_ITEM_MASTER_SETUP IMS
                                WHERE IMS.ITEM_CODE IN ({itemList}) {optionalCondition} 
                                AND SI.ITEM_CODE = IMS.ITEM_CODE
                                AND SI.COMPANY_CODE = IMS.COMPANY_CODE
                                AND IMS.GROUP_SKU_FLAG = 'I'
                                AND SI.PLAN_DATE BETWEEN to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD')
                            GROUP BY SI.ITEM_CODE,
                                     IMS.ITEM_EDESC,
                                     SI.CUSTOMER_CODE,
                                     SI.DIVISION_CODE,
                                     SI.BRANCH_CODE,
                                     SUBSTR(BS_DATE(SI.PLAN_DATE),0,4),
                                     SUBSTR(BS_DATE(SI.PLAN_DATE),6,2)
                                    ORDER BY NEPALI_YEAR,ITEM_CODE,NEPALI_MONTH ASC)t";
            }
            return qry;
        }

        public List<PlanSalesRefrenceModel> getSalesItemDataForRefrence(string itemList, string startDate, string endDate, string customerCode, string divisionCode, string branchCode, string employeeCode, string dateFormat, string frequency, string materialPlanCode, string REFERENCE_FLAG)
        {
            var userNo = _workcontext.CurrentUserinformation.User_id;
            var result = new List<PlanSalesRefrenceModel>();
            try
            {
                if (!string.IsNullOrEmpty(itemList))
                {
                    var itemListArr = itemList.Split(',');
                    itemList = $@"'{string.Join("','", itemListArr)}'";
                }
                if (!string.IsNullOrEmpty(branchCode))
                {
                    var branchCodeArr = branchCode.Split(',');
                    branchCode = $@"'{string.Join("','", branchCodeArr)}'";
                }
                if (!string.IsNullOrEmpty(employeeCode))
                {
                    var employeeCodeArr = employeeCode.Split(',');
                    employeeCode = $@"'{string.Join("','", employeeCodeArr)}'";
                }
                var refrenceQuery = string.Empty;
                string optionalCondition = string.Empty;
                if (!string.IsNullOrEmpty(customerCode))
                {
                    optionalCondition += $@"  AND SI.CUSTOMER_CODE = '{customerCode}'";
                }
                if (!string.IsNullOrEmpty(divisionCode))
                {
                    optionalCondition += $@"  AND SI.DIVISION_CODE = '{divisionCode}'";
                }
                if (!string.IsNullOrEmpty(branchCode))
                {
                    optionalCondition += $@"  AND SI.BRANCH_CODE in ({branchCode})";
                }
                if (!string.IsNullOrEmpty(employeeCode))
                {
                    optionalCondition += $@"  AND SI.EMPLOYEE_CODE in ({employeeCode})";
                }

                if (dateFormat.ToUpper() == "BS" || dateFormat.ToUpper() == "LOC")
                {
                    if (frequency.ToLower() == "week")
                    {
                        refrenceQuery = $@" SELECT  'freqItemNum_'||SI.ITEM_CODE ||'_WEEK_' || NEPALI_WEEKS.WEEKS ||'_'||  TO_CHAR(SUBSTR(BS_DATE(SI.PLAN_DATE),0,4) )  COLNAME, SI.ITEM_CODE,
                                         IMS.ITEM_EDESC,
                                         SI.CUSTOMER_CODE,
                                         SI.DIVISION_CODE,
                                         SI.BRANCH_CODE,
                                        NEPALI_WEEKS.WEEKS,
                                        TO_CHAR(SUM (SI.PER_DAY_QUANTITY)) TOTAL_QTY,
                                        TO_CHAR(SUM (SI.PER_DAY_QUANTITY)) QTY
                                    FROM PL_SALES_PLAN_DTL SI, IP_ITEM_MASTER_SETUP IMS, (SELECT DAYS, ROWNUM  DAY_NO , TRUNC((ROWNUM+7)/7)  WEEKS
                                  FROM (SELECT FISCAL_YEAR.START_DATE + ROWNUM - 1 DAYS
                                          FROM ALL_OBJECTS,
                                               (SELECT START_DATE, END_DATE
                                                  FROM HR_FISCAL_YEAR_CODE
                                                 WHERE SYSDATE BETWEEN START_DATE AND END_DATE) FISCAL_YEAR
                                         WHERE ROWNUM <= FISCAL_YEAR.END_DATE - FISCAL_YEAR.START_DATE + 1) )NEPALI_WEEKS
                                WHERE IMS.ITEM_CODE IN ({itemList}) {optionalCondition} 
                                AND SI.ITEM_CODE = IMS.ITEM_CODE
                                    AND NEPALI_WEEKS.DAYS = SI.PLAN_DATE
                                    AND SI.COMPANY_CODE = IMS.COMPANY_CODE
                                    AND IMS.GROUP_SKU_FLAG = 'I'
                                    AND SI.PLAN_DATE BETWEEN to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD')
                                GROUP BY SI.ITEM_CODE,
                                         IMS.ITEM_EDESC,
                                         SI.CUSTOMER_CODE,
                                         SI.DIVISION_CODE,
                                         SI.BRANCH_CODE,
                                        NEPALI_WEEKS.WEEKS
                                        ,SUBSTR(BS_DATE(SI.PLAN_DATE),0,4)  
                                 ORDER BY   NEPALI_WEEKS.WEEKS";
                    }
                    else if (frequency.ToLower() == "month")
                    {
                        refrenceQuery = getQueryForSalesReference(itemList, optionalCondition, startDate, endDate, materialPlanCode, REFERENCE_FLAG);
                        //    if (materialPlanCode != "0" && materialPlanCode != null)
                        //    {
                        //        refrenceQuery = $@" SELECT COLNAME,NEPALI_MONTH,NEPALI_YEAR,ITEM_CODE,ITEM_EDESC,QTY,
                        //            TO_CHAR((select sum(CAlC_QUANTITY) from PL_MATERIAL_PLAN_DTL where  plan_date between to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD') and
                        //                    item_code=t.item_code and plan_code='{materialPlanCode}' group by item_code)) TOTAL_QTY FROM
                        //                (SELECT 'freqItemNum_'||SI.ITEM_CODE ||'_' ||  UPPER(fn_bs_month(SUBSTR(BS_DATE(SI.PLAN_DATE),6,2))) ||'_'|| SUBSTR(BS_DATE(SI.PLAN_DATE),0,4) COLNAME ,
                        //                 SUBSTR(BS_DATE(SI.PLAN_DATE),6,2)NEPALI_MONTH,
                        //                 SUBSTR(BS_DATE(SI.PLAN_DATE),0,4)NEPALI_YEAR,
                        //                 SI.ITEM_CODE,IMS.ITEM_EDESC,
                        //                 TO_CHAR(SUM (SI.CAlC_QUANTITY)) TOTAL_QTY,
                        //                 TO_CHAR(SUM (SI.CALC_QUANTITY)) QTY
                        //            FROM PL_MATERIAL_PLAN_DTL SI, IP_ITEM_MASTER_SETUP IMS
                        //            WHERE IMS.ITEM_CODE IN ({itemList}) {optionalCondition} 
                        //            AND SI.ITEM_CODE = IMS.ITEM_CODE
                        //            AND SI.COMPANY_CODE = IMS.COMPANY_CODE
                        //            AND IMS.GROUP_SKU_FLAG = 'I'
                        //            AND SI.PLAN_CODE='{materialPlanCode}'
                        //            AND SI.PLAN_DATE BETWEEN to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD')
                        //        GROUP BY SI.ITEM_CODE,
                        //                 IMS.ITEM_EDESC,
                        //                 SUBSTR(BS_DATE(SI.PLAN_DATE),0,4),
                        //                 SUBSTR(BS_DATE(SI.PLAN_DATE),6,2)
                        //                ORDER BY NEPALI_YEAR,ITEM_CODE,NEPALI_MONTH ASC)t";
                        //    }
                        //    else
                        //    {
                        //        refrenceQuery = $@"SELECT COLNAME,NEPALI_MONTH,NEPALI_YEAR,ITEM_CODE,ITEM_EDESC,CUSTOMER_CODE,DIVISION_CODE,QTY,
                        //            TO_CHAR((select sum(PER_DAY_QUANTITY) from PL_SALES_PLAN_DTL where  plan_date between to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD') and
                        //                    item_code=t.item_code group by item_code)) TOTAL_QTY FROM
                        //                (SELECT 'freqItemNum_'||SI.ITEM_CODE ||'_' ||  UPPER(fn_bs_month(SUBSTR(BS_DATE(SI.PLAN_DATE),6,2))) ||'_'|| SUBSTR(BS_DATE(SI.PLAN_DATE),0,4) COLNAME ,
                        //                 SUBSTR(BS_DATE(SI.PLAN_DATE),6,2)NEPALI_MONTH,
                        //                 SUBSTR(BS_DATE(SI.PLAN_DATE),0,4)NEPALI_YEAR,
                        //                 SI.ITEM_CODE,
                        //                 IMS.ITEM_EDESC,
                        //                 SI.CUSTOMER_CODE,
                        //                 SI.DIVISION_CODE,
                        //                 SI.BRANCH_CODE,
                        //                -- TO_CHAR(SUM (SI.CALC_TOTAL_PRICE)) AMOUNT,
                        //                 TO_CHAR(SUM (SI.PER_DAY_QUANTITY)) TOTAL_QTY,
                        //                 TO_CHAR(SUM (SI.PER_DAY_QUANTITY)) QTY
                        //            FROM PL_SALES_PLAN_DTL SI, IP_ITEM_MASTER_SETUP IMS
                        //            WHERE IMS.ITEM_CODE IN ({itemList}) {optionalCondition} 
                        //            AND SI.ITEM_CODE = IMS.ITEM_CODE
                        //            AND SI.COMPANY_CODE = IMS.COMPANY_CODE
                        //            AND IMS.GROUP_SKU_FLAG = 'I'
                        //            AND SI.PLAN_DATE BETWEEN to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD')
                        //        GROUP BY SI.ITEM_CODE,
                        //                 IMS.ITEM_EDESC,
                        //                 SI.CUSTOMER_CODE,
                        //                 SI.DIVISION_CODE,
                        //                 SI.BRANCH_CODE,
                        //                 SUBSTR(BS_DATE(SI.PLAN_DATE),0,4),
                        //                 SUBSTR(BS_DATE(SI.PLAN_DATE),6,2)
                        //                ORDER BY NEPALI_YEAR,ITEM_CODE,NEPALI_MONTH ASC)t";
                        //    }

                    }
                }
                else if (dateFormat.ToUpper() == "AD" || dateFormat.ToUpper() == "ENG")
                {
                    if (frequency.ToLower() == "week")
                    {
                        refrenceQuery = $@"SELECT 'freqItemNum_'||SI.ITEM_CODE ||'_WEEK_' ||  TO_CHAR(SI.PLAN_DATE,'IW') ||'_'||  TO_CHAR(SI.PLAN_DATE,'YYYY') COLNAME, SI.ITEM_CODE,
                                       IMS.ITEM_EDESC,
                                     SI.CUSTOMER_CODE,
                                     SI.DIVISION_CODE,
                                     SI.BRANCH_CODE,
                                     TO_CHAR(SI.PLAN_DATE,'YYYY') AD_YEAR,
                                     TO_CHAR(SI.PLAN_DATE,'IW') SALES_WEEK,
                                     --TO_CHAR(SUM (SI.CALC_TOTAL_PRICE)) AMOUNT,
                                     TO_CHAR(SUM (SI.PER_DAY_QUANTITY)) TOTAL_QTY,
                                     TO_CHAR(SUM (SI.PER_DAY_QUANTITY)) QTY
                                FROM PL_SALES_PLAN_DTL SI, IP_ITEM_MASTER_SETUP IMS
                                WHERE IMS.ITEM_CODE IN ({itemList}) {optionalCondition} 
                                AND SI.ITEM_CODE = IMS.ITEM_CODE
                                AND IMS.GROUP_SKU_FLAG = 'I'
                                AND SI.PLAN_DATE BETWEEN to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD')
                           GROUP BY SI.ITEM_CODE,
                             IMS.ITEM_EDESC,
                             SI.CUSTOMER_CODE,
                             SI.DIVISION_CODE,
                             SI.BRANCH_CODE,
                             TO_CHAR(SI.PLAN_DATE,'YYYY'),
                             TO_CHAR(SI.PLAN_DATE,'IW')
                     ORDER BY  TO_CHAR(SI.PLAN_DATE,'YYYY'), TO_CHAR(SI.PLAN_DATE,'IW')";
                    }
                    else if (frequency.ToLower() == "month")
                    {
                        refrenceQuery = $@"SELECT TO_CHAR((select sum(PER_DAY_QUANTITY) from PL_SALES_PLAN_DTL where item_code=t.item_code group by item_code)) TOTAL_QTY,COLNAME,ITEM_EDESC,
                                        ITEM_CODE,CUSTOMER_CODE,DIVISION_CODE,BRANCH_CODE,QTY FROM
                                    (SELECT  'freqItemNum_'||SI.ITEM_CODE ||'_' ||  TO_CHAR(SI.PLAN_DATE,'MON') ||'_'||  TO_CHAR(SI.PLAN_DATE,'YYYY') COLNAME ,
                                     IMS.ITEM_EDESC,
                                     SI.CUSTOMER_CODE,
                                     SI.DIVISION_CODE,
                                     SI.BRANCH_CODE,
                                     TO_CHAR(SUM (SI.PER_DAY_QUANTITY)) QTY
                                FROM PL_SALES_PLAN_DTL SI, IP_ITEM_MASTER_SETUP IMS
                                WHERE IMS.ITEM_CODE IN ({itemList}) {optionalCondition} 
                                AND SI.ITEM_CODE = IMS.ITEM_CODE
                                AND SI.COMPANY_CODE = IMS.COMPANY_CODE
                                AND IMS.GROUP_SKU_FLAG = 'I'
                                AND SI.PLAN_DATE BETWEEN to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD')
                            GROUP BY SI.ITEM_CODE,
                                     IMS.ITEM_EDESC,
                                     SI.CUSTOMER_CODE,
                                     SI.DIVISION_CODE,
                                     SI.BRANCH_CODE,
                                     TO_CHAR(SI.PLAN_DATE,'YYYY'),
                                     TO_CHAR(SI.PLAN_DATE,'MON'))t";
                    }
                }

                result = this._dbContext.SqlQuery<PlanSalesRefrenceModel>(refrenceQuery).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public List<PlanSalesRefrenceModel> getCollectionItemDataForStock(string itemList, string startDate, string endDate, string customerCode, string divisionCode, string branchCode, string dateFormat, string frequency)
        {
            var userNo = _workcontext.CurrentUserinformation.User_id;
            var company_code = _workcontext.CurrentUserinformation.company_code;
            var result = new List<PlanSalesRefrenceModel>();
            try
            {
                var refrenceQuery = string.Empty;
                string optionalCondition = string.Empty;

                if (!string.IsNullOrEmpty(itemList))
                {
                    var itemListArr = itemList.Split(',');
                    itemList = $@"'{string.Join("','", itemListArr)}'";
                }
                if (!string.IsNullOrEmpty(branchCode))
                {
                    var branchCodeArr = branchCode.Split(',');
                    branchCode = $@"'{string.Join("','", branchCodeArr)}'";
                }

                if (!string.IsNullOrEmpty(branchCode))
                {
                    optionalCondition += $@"  AND SI.BRANCH_CODE in ({branchCode})";
                }

                refrenceQuery = $@"SELECT SI.ITEM_CODE,
                                    TO_CHAR(SUM(SI.IN_QUANTITY-SI.OUT_QUANTITY)) QTY
                                FROM V$VIRTUAL_STOCK_WIP_LEDGER1 SI, IP_ITEM_MASTER_SETUP IMS
                                WHERE SI.DELETED_FLAG='N' AND SI.COMPANY_CODE='{company_code}'
                                AND  IMS.ITEM_CODE IN ({itemList}) {optionalCondition} 
                                AND SI.ITEM_CODE = IMS.ITEM_CODE
                                AND SI.COMPANY_CODE =IMS.COMPANY_CODE
                                AND SI.VOUCHER_DATE BETWEEN to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD')
                            GROUP BY SI.ITEM_CODE";

                //if (dateFormat.ToUpper() == "BS" || dateFormat.ToUpper() == "LOC")
                //{
                //    if (frequency.ToLower() == "week")
                //    {
                //        refrenceQuery = $@" SELECT  'freqItemNum_'||SI.ITEM_CODE ||'_WEEK_' || NEPALI_WEEKS.WEEKS ||'_'||  TO_CHAR(SUBSTR(BS_DATE(SI.SALES_DATE),0,4) )  COLNAME, SI.ITEM_CODE,
                //                         IMS.ITEM_EDESC,
                //                         SI.CUSTOMER_CODE,
                //                         SI.DIVISION_CODE,
                //                         SI.BRANCH_CODE,
                //                        NEPALI_WEEKS.WEEKS,
                //                         TO_CHAR(SUM (SI.CALC_TOTAL_PRICE)) AMOUNT,
                //                        TO_CHAR(SUM (SI.CALC_QUANTITY)) QTY
                //                    FROM SA_SALES_INVOICE SI, IP_ITEM_MASTER_SETUP IMS, (SELECT DAYS, ROWNUM  DAY_NO , TRUNC((ROWNUM+7)/7)  WEEKS
                //                  FROM (SELECT FISCAL_YEAR.START_DATE + ROWNUM - 1 DAYS
                //                          FROM ALL_OBJECTS,
                //                               (SELECT START_DATE, END_DATE
                //                                  FROM HR_FISCAL_YEAR_CODE
                //                                 WHERE SYSDATE BETWEEN START_DATE AND END_DATE) FISCAL_YEAR
                //                         WHERE ROWNUM <= FISCAL_YEAR.END_DATE - FISCAL_YEAR.START_DATE + 1) )NEPALI_WEEKS
                //                WHERE IMS.ITEM_CODE IN ({itemList}) {optionalCondition} 
                //                AND SI.ITEM_CODE = IMS.ITEM_CODE
                //                    AND NEPALI_WEEKS.DAYS = SI.SALES_DATE
                //                    AND SI.SALES_DATE BETWEEN to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD')
                //                GROUP BY SI.ITEM_CODE,
                //                         IMS.ITEM_EDESC,
                //                         SI.CUSTOMER_CODE,
                //                         SI.DIVISION_CODE,
                //                         SI.BRANCH_CODE,
                //                        NEPALI_WEEKS.WEEKS
                //                        ,SUBSTR(BS_DATE(SI.SALES_DATE),0,4)  
                //                 ORDER BY   NEPALI_WEEKS.WEEKS";
                //    }
                //    else if (frequency.ToLower() == "month")
                //    {
                //        refrenceQuery = $@"SELECT 'freqItemNum_'||SI.ITEM_CODE ||'_' ||  UPPER(fn_bs_month(SUBSTR(BS_DATE(SI.VOUCHER_DATE),6,2))) ||'_'|| SUBSTR(BS_DATE(SI.VOUCHER_DATE),0,4) COLNAME ,
                //                    TO_CHAR(SUM(SI.IN_QUANTITY-SI.OUT_QUANTITY)) QTY
                //                FROM V$VIRTUAL_STOCK_WIP_LEDGER SI, IP_ITEM_MASTER_SETUP IMS
                //                WHERE IMS.ITEM_CODE IN ({itemList}) {optionalCondition} 
                //                AND SI.ITEM_CODE = IMS.ITEM_CODE
                //                AND SI.VOUCHER_DATE BETWEEN to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD')
                //            GROUP BY SI.ITEM_CODE,
                //                     SUBSTR(BS_DATE(SI.VOUCHER_DATE),0,4),
                //                     fn_bs_month(SUBSTR(BS_DATE(SI.VOUCHER_DATE),6,2))";
                //    }
                //}
                //else if (dateFormat.ToUpper() == "AD" || dateFormat.ToUpper() == "ENG")
                //{
                //    if (frequency.ToLower() == "week")
                //    {
                //        refrenceQuery = $@"SELECT 'freqItemNum_'||SI.ITEM_CODE ||'_WEEK_' ||  TO_CHAR(SI.SALES_DATE,'IW') ||'_'||  TO_CHAR(SI.SALES_DATE,'YYYY') COLNAME, SI.ITEM_CODE,
                //                       IMS.ITEM_EDESC,
                //                     SI.CUSTOMER_CODE,
                //                     SI.DIVISION_CODE,
                //                     SI.BRANCH_CODE,
                //                     TO_CHAR(SI.SALES_DATE,'YYYY') AD_YEAR,
                //                     TO_CHAR(SI.SALES_DATE,'IW') SALES_WEEK,
                //                     TO_CHAR(SUM (SI.CALC_TOTAL_PRICE)) AMOUNT,
                //                     TO_CHAR(SUM (SI.CALC_QUANTITY)) QTY
                //                FROM SA_SALES_INVOICE SI, IP_ITEM_MASTER_SETUP IMS
                //                WHERE IMS.ITEM_CODE IN ({itemList}) {optionalCondition} 
                //                AND SI.ITEM_CODE = IMS.ITEM_CODE
                //                AND SI.SALES_DATE BETWEEN to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD')
                //           GROUP BY SI.ITEM_CODE,
                //             IMS.ITEM_EDESC,
                //             SI.CUSTOMER_CODE,
                //             SI.DIVISION_CODE,
                //             SI.BRANCH_CODE,
                //             TO_CHAR(SI.SALES_DATE,'YYYY'),
                //             TO_CHAR(SI.SALES_DATE,'IW')
                //     ORDER BY  TO_CHAR(SI.SALES_DATE,'YYYY'), TO_CHAR(SI.SALES_DATE,'IW')";
                //    }
                //    else if (frequency.ToLower() == "month")
                //    {
                //        refrenceQuery = $@"SELECT  'freqItemNum_'||SI.ITEM_CODE ||'_' ||  TO_CHAR(SI.SALES_DATE,'MON') ||'_'||  TO_CHAR(SI.SALES_DATE,'YYYY') COLNAME ,
                //                     IMS.ITEM_EDESC,
                //                     SI.CUSTOMER_CODE,
                //                     SI.DIVISION_CODE,
                //                     SI.BRANCH_CODE,
                //                    -- SUBSTR(TO_BS(SI.SALES_DATE),7,7) NEPALI_MONTH,
                //                     TO_CHAR(SUM (SI.CALC_TOTAL_PRICE)) AMOUNT,
                //                     TO_CHAR(SUM (SI.CALC_QUANTITY)) QTY
                //                FROM SA_SALES_INVOICE SI, IP_ITEM_MASTER_SETUP IMS
                //                WHERE IMS.ITEM_CODE IN ({itemList}) {optionalCondition} 
                //                AND SI.ITEM_CODE = IMS.ITEM_CODE
                //                AND SI.SALES_DATE BETWEEN to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD')
                //            GROUP BY SI.ITEM_CODE,
                //                     IMS.ITEM_EDESC,
                //                     SI.CUSTOMER_CODE,
                //                     SI.DIVISION_CODE,
                //                     SI.BRANCH_CODE,
                //                     TO_CHAR(SI.SALES_DATE,'YYYY'),
                //                     TO_CHAR(SI.SALES_DATE,'MON')";
                //    }
                //}

                result = this._dbContext.SqlQuery<PlanSalesRefrenceModel>(refrenceQuery).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public bool cloneCollectionPlan(string planCode, string planName, string customers, string employees, string branchs, string divisions, string remarks)
        {
            try
            {
                remarks = remarks.Replace("\"", " ");
                string existingPlanNameQuery = $@"SELECT PLAN_EDESC FROM PL_BRD_COLLECTION_PLAN WHERE PLAN_CODE='{planCode}'";
                string existingPlanName = this._dbContext.SqlQuery<String>(existingPlanNameQuery).FirstOrDefault();
                if (existingPlanName != null)
                {
                    string copyPlanName = existingPlanName + "_copy";
                    if (!string.IsNullOrEmpty(planName))
                        copyPlanName = planName;
                    string maxPlanCodeQuery = "SELECT COALESCE(MAX(PLAN_CODE)+1, MAX(PLAN_CODE) + 1, 1) FROM PL_BRD_COLLECTION_PLAN";
                    int maxPlanCode = this._dbContext.SqlQuery<int>(maxPlanCodeQuery).First();

                    string copySalesPlanQuery = $@"INSERT INTO PL_BRD_COLLECTION_PLAN(PLAN_CODE,PLAN_EDESC,PLAN_NDESC,EST_AMOUNT,TIME_FRAME_CODE,
                    CALENDAR_TYPE,START_DATE,END_DATE,REMARKS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,
                    DELETED_FLAG)
                    SELECT '{maxPlanCode}','{copyPlanName}', PLAN_NDESC,EST_AMOUNT,TIME_FRAME_CODE,
                    CALENDAR_TYPE,START_DATE,END_DATE,'{remarks}',COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,
                    DELETED_FLAG FROM PL_BRD_COLLECTION_PLAN WHERE PLAN_CODE='{planCode}'";
                    var insertCopiedPlan = this._dbContext.ExecuteSqlCommand(copySalesPlanQuery);

                    int copiedPlanCode = maxPlanCode;
                    if (copiedPlanCode > 0)
                    {
                        string copyinto_plandtl = $@"INSERT INTO PL_BRD_COLLECTION_PLAN_DTL
                                (PLAN_CODE,PLAN_DATE,PER_DAY_AMOUNT,ITEM_CODE,
                                DIVISION_CODE,REMARKS,
                                COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,DELETED_FLAG,FREQUENCY_JSON)
                                SELECT '{copiedPlanCode}', PLAN_DATE,PER_DAY_AMOUNT,ITEM_CODE,
                               '{divisions}',REMARKS,
                                COMPANY_CODE,'{branchs}',CREATED_BY,CREATED_DATE,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,DELETED_FLAG,FREQUENCY_JSON FROM PL_BRD_COLLECTION_PLAN_DTL WHERE PLAN_CODE='{planCode}'";

                        var result = _dbContext.ExecuteSqlCommand(copyinto_plandtl).ToString();
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public string CreateTemCollectionPlanReportTable(ReportFiltersModel model)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var response = "";
            var createTableQry = $@"CREATE TABLE TEMP_PL_COLLECTION_PLAN_REPORT AS  SELECT DISTINCT 
            TO_CHAR(fn_bs_month(substr(bs_date(SPD.PLAN_DATE), 6, 2))) MONTH,
            SUBSTR(BS_DATE(SPD.PLAN_DATE), 6, 2) MONTHINT,
            TO_CHAR(substr(bs_date(SPD.PLAN_DATE), 0, 4)) as YEAR,-- SPD.ITEM_CODE, IMS.ITEM_EDESC,
             SPD.CUSTOMER_CODE, SPD.EMPLOYEE_CODE,
            SPD.DIVISION_CODE, CS.CUSTOMER_EDESC, DS.DIVISION_EDESC, FS.BRANCH_CODE, SPD.COMPANY_CODE,
            ES.EMPLOYEE_EDESC, FS.BRANCH_EDESC ,
            SUM(SPD.PER_DAY_AMOUNT) PER_DAY_AMOUNT, 
            --SUM(SPD.PER_DAY_QUANTITY) PER_DAY_QUANTITY,
            SP.PLAN_EDESC,
            SPD.PLAN_DATE
            FROM PL_BRD_COLLECTION_PLAN_DTL SPD,
            PL_BRD_COLLECTION_PLAN SP,
            SA_CUSTOMER_SETUP CS,
            FA_DIVISION_SETUP DS,
            HR_EMPLOYEE_SETUP ES,
            FA_BRANCH_SETUP FS
            WHERE SP.DELETED_FLAG = 'N' 
            AND SP.PLAN_CODE = SPD.PLAN_CODE
            AND CS.CUSTOMER_CODE (+) = SPD.CUSTOMER_CODE
            AND DS.DIVISION_CODE (+)= SPD.DIVISION_CODE
            AND ES.EMPLOYEE_CODE (+)= SPD.EMPLOYEE_CODE
            AND FS.BRANCH_CODE (+)= SPD.BRANCH_CODE
            AND CS.COMPANY_CODE (+)=SPD.COMPANY_CODE
            AND DS.COMPANY_CODE (+)=SPD.COMPANY_CODE
            AND ES.COMPANY_CODE (+)=SPD.COMPANY_CODE
            AND FS.COMPANY_CODE (+)=SPD.COMPANY_CODE  
            AND CS.GROUP_SKU_FLAG='I'
            GROUP BY TO_CHAR(fn_bs_month(substr(bs_date(SPD.PLAN_DATE), 6, 2))),
                    TO_CHAR(substr(bs_date(SPD.PLAN_DATE), 0, 4)) ,
                    SUBSTR(BS_DATE(SPD.PLAN_DATE), 6, 2),
                     SPD.CUSTOMER_CODE, SPD.EMPLOYEE_CODE,--IMS.ITEM_EDESC, SPD.ITEM_CODE,
                    SPD.DIVISION_CODE, CS.CUSTOMER_EDESC, DS.DIVISION_EDESC,
                    ES.EMPLOYEE_EDESC, FS.BRANCH_EDESC ,FS.BRANCH_CODE , SPD.COMPANY_CODE,SP.PLAN_EDESC,SPD.PLAN_DATE ";
            try
            {
                var tableExistsQry = $@"SELECT Count(*) FROM TEMP_PL_COLLECTION_PLAN_REPORT";
                var result = this._dbContext.SqlQuery<int>(tableExistsQry).ToList();
                if (result.Count>0)
                {
                    var dropQry = $@"DROP TABLE TEMP_PL_COLLECTION_PLAN_REPORT";
                    var dropResponse = this._dbContext.ExecuteSqlCommand(dropQry);
                    var createResponse = this._dbContext.ExecuteSqlCommand(createTableQry);
                    response = "TableDropedAndCreated";
                }
            }
            catch (Exception ex)
            {

                if (ex.Message.Contains("table or view does not exist"))
                {
                    var result = this._dbContext.ExecuteSqlCommand(createTableQry);
                    response = "TableCreated";
                }
                else
                {
                    var dropQry = $@"DROP TABLE TEMP_PL_COLLECTION_PLAN_REPORT";
                    var dropResponse = this._dbContext.ExecuteSqlCommand(dropQry);
                    response = ex.Message;
                }
            }
            return response;
        }
        #endregion
        #region Material plan in procurement
        public List<MaterialPlanModel> GetMaterialPlanList()
        {
            var company_code = _workcontext.CurrentUserinformation.company_code;
            try
            {
                var query = $@"SELECT PLAN_CODE,PLAN_EDESC FROM PL_MATERIAL_PLAN WHERE DELETED_FLAG='N' AND COMPANY_CODE='{company_code}'";
                var result = _dbContext.SqlQuery<MaterialPlanModel>(query).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<CollectionFromMaterialModel> GetAllRawMaterialByMaterialCode(string pCode)
        {
            var company_code = _workcontext.CurrentUserinformation.company_code;
            var result = new List<CollectionFromMaterialModel>();
            try
            {
                var query = $@"SELECT NVL(MPD.CALC_QUANTITY,0)REQUIRED_QUANTITY, MPD.FINISHED_ITEM_CODE,IMS.CATEGORY_CODE,IC.CATEGORY_EDESC,
                                    (SELECT ITEM_EDESC FROM IP_ITEM_MASTER_SETUP WHERE DELETED_FLAG='N' AND COMPANY_CODE='{company_code}' AND ITEM_CODE=MPD.FINISHED_ITEM_CODE)FINISHED_ITEM_EDESC
                                    ,MPD.ITEM_CODE,INITCAP(IMS.ITEM_EDESC)ITEM_EDESC,IMS.GROUP_SKU_FLAG,NVL(IMS.MIN_LEVEL,0)MIN_LEVEL,NVL(IMS.MAX_LEVEL,0)MAX_LEVEL
                                    ,MPD.PLAN_DATE,TO_CHAR(fn_bs_month(substr(bs_date(MPD.PLAN_DATE), 6, 2)))MONTH FROM PL_MATERIAL_PLAN_DTL MPD
                                    INNER JOIN IP_ITEM_MASTER_SETUP IMS ON MPD.ITEM_CODE=IMS.ITEM_CODE AND MPD.COMPANY_CODE=IMS.COMPANY_CODE
                                    INNER JOIN IP_CATEGORY_CODE IC ON IMS.CATEGORY_CODE = IC.CATEGORY_CODE AND IMS.COMPANY_CODE=IC.COMPANY_CODE
                                    WHERE MPD.DELETED_FLAG='N' AND MPD.COMPANY_CODE='{company_code}' AND MPD.PLAN_CODE IN ('{pCode}')";

                result = _dbContext.SqlQuery<CollectionFromMaterialModel>(query).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public List<ItemModel> getItemByPlanCode(string planCode)
        {
            throw new NotImplementedException();
        }
        public string GetEmployeeCodeByPlanCode(string plan_code)
        {
            var query = $@"Select Distinct EMPLOYEE_CODE from PL_BRD_COLLECTION_PLAN_DTL Where PLAN_CODE='{plan_code}'";
            string emp_code = _dbContext.SqlQuery<string>(query).FirstOrDefault();
            return emp_code;
        }
        public string GetCustomerCodeByPlanCode(string plan_code)
        {
            var query = $@"Select Distinct CUSTOMER_CODE from PL_BRD_COLLECTION_PLAN_DTL Where PLAN_CODE='{plan_code}'";
            string emp_code = _dbContext.SqlQuery<string>(query).FirstOrDefault();
            return emp_code;
        }
        #endregion
        public List<PlanRegisterProductModel> PlanRegisterProducts(User userinfo)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
           

            string query = $@"SELECT DISTINCT --LEVEL, 
            INITCAP(CUSTOMER_EDESC) AS ItemName,
            CUSTOMER_CODE AS ItemCode,
            MASTER_CUSTOMER_CODE AS MasterItemCode, 
            PRE_CUSTOMER_CODE AS PreItemCode, 
            GROUP_SKU_FLAG AS GroupFlag
             --CATEGORY_CODE
            FROM SA_CUSTOMER_SETUP ims
            WHERE ims.DELETED_FLAG = 'N' 
            AND ims.COMPANY_CODE = '{userinfo.company_code}'           
            --START WITH PRE_ITEM_CODE = '00' AND COMPANY_CODE = '{userinfo.company_code}' AND DELETED_FLAG='N'
            --CONNECT BY PRIOR MASTER_ITEM_CODE = PRE_ITEM_CODE AND DELETED_FLAG='N' AND COMPANY_CODE='{userinfo.company_code}'";
            var productListNodes = _dbContext.SqlQuery<PlanRegisterProductModel>(query).ToList();
            return productListNodes;
        }
        public List<ProductSetupModel> ProductListAllNodes(User userinfo)
        {

            string query = $@"SELECT Distinct LEVEL, 
                            INITCAP(CUSTOMER_EDESC) AS ItemName,
                            CUSTOMER_CODE AS ItemCode,
                            MASTER_CUSTOMER_CODE AS MasterItemCode, 
                            PRE_CUSTOMER_CODE AS PreItemCode, 
                            GROUP_SKU_FLAG AS GroupFlag,
                            --PURCHASE_PRICE As Rate,
                            (SELECT COUNT(*) FROM SA_CUSTOMER_SETUP WHERE  
                             COMPANY_CODE='{userinfo.company_code}'  AND DELETED_FLAG='N' AND PRE_CUSTOMER_CODE = ims.MASTER_CUSTOMER_CODE) as Childrens 
                            FROM SA_CUSTOMER_SETUP ims
                            WHERE ims.DELETED_FLAG = 'N' 
                            AND ims.COMPANY_CODE = '{userinfo.company_code}'
                            START WITH PRE_CUSTOMER_CODE = '00' AND COMPANY_CODE='{userinfo.company_code}' AND DELETED_FLAG='N'
                            CONNECT BY PRIOR MASTER_CUSTOMER_CODE = PRE_CUSTOMER_CODE AND COMPANY_CODE='{userinfo.company_code}' AND DELETED_FLAG='N'
                            ORDER BY ItemName";
            var productListNodes = _dbContext.SqlQuery<ProductSetupModel>(query).ToList();
            return productListNodes;
        }
        public List<ProductSetupModel> GetProductsListByProductCode(string level, string masterProductCode, User userinfo)
        {
            string query = string.Format(@"SELECT LEVEL, 
            INITCAP(CUSTOMER_EDESC) AS ItemName,
            CUSTOMER_CODE AS ItemCode,
            MASTER_CUSTOMER_CODE AS MasterItemCode, 
            PRE_CUSTOMER_CODE AS PreItemCode, 
            GROUP_SKU_FLAG AS GroupFlag,
            (SELECT COUNT(*) FROM SA_CUSTOMER_SETUP WHERE  
             COMPANY_CODE='" + userinfo.company_code + @"' AND DELETED_FLAG='N' AND PRE_CUSTOMER_CODE = ims.MASTER_CUSTOMER_CODE) as Childrens 
            FROM SA_CUSTOMER_SETUP ims
            WHERE ims.DELETED_FLAG = 'N' 
            AND ims.COMPANY_CODE = '" + userinfo.company_code + @"'
            AND LEVEL = {0}
            START WITH PRE_CUSTOMER_CODE = '{1}'
            CONNECT BY PRIOR MASTER_CUSTOMER_CODE = PRE_CUSTOMER_CODE
            ORDER SIBLINGS BY CUSTOMER_EDESC", level.ToString(), masterProductCode.ToString());
            var productListNodes = _dbContext.SqlQuery<ProductSetupModel>(query).ToList();
            return productListNodes;
        }
        #region collection Report
        public List<PlanReportModel> getMonthlyCollectionPlanChart(ReportFiltersModel model)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var branchCode = _workcontext.CurrentUserinformation.branch_code;

            var userId = _workcontext.CurrentUserinformation.User_id;
            var superFlagQuery = $@"SELECT SUPER_USER_FLAG FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE = '{companyCode}'";
            var superFlag = _dbContext.SqlQuery<string>(superFlagQuery).FirstOrDefault();
            var loginEmpCodeQuery = $@"SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE DELETED_FLAG='N' AND USER_NO='{userId}' AND COMPANY_CODE='{companyCode}'";
            var loginEmpCode = _dbContext.SqlQuery<string>(loginEmpCodeQuery).FirstOrDefault();
            var Query = $@"SELECT SUM(PER_DAY_AMOUNT) PER_DAY_AMOUNT,
                                  --SUM(PER_DAY_QUANTITY) PER_DAY_QUANTITY,
                                  MONTH,MONTHINT,YEAR,COMPANY_CODE
                                  FROM TEMP_PL_COLLECTION_PLAN_REPORT SPD
                                WHERE SPD.COMPANY_CODE = '{companyCode}' AND SPD.PER_DAY_AMOUNT > 0  ";
            if (superFlag != "Y" && model.EmployeeFilter.Count == 0)
            {
                Query += $@" AND SPD.EMPLOYEE_CODE IN(
                    SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE='{companyCode}'
                     UNION ALL
                    SELECT EMPLOYEE_CODE FROM(
                    SELECT EMPLOYEE_CODE,COMPANY_CODE FROM HR_EMPLOYEE_TREE ET 
                    CONNECT BY PRIOR ET.EMPLOYEE_CODE=ET.PARENT_EMPLOYEE_CODE
                     START WITH PARENT_EMPLOYEE_CODE='{loginEmpCode}' )X WHERE X.COMPANY_CODE='{companyCode}'
                    )";
            }

            if (model.CustomerFilter.Count > 0)
            {
                var customers = model.CustomerFilter;
                var customerConditionQuery = string.Empty;
                for (int i = 0; i < customers.Count; i++)
                {
                    if (i == 0)
                        customerConditionQuery += string.Format("MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%' from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    else
                    {
                        customerConditionQuery += string.Format(" OR  MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    }
                }

                Query = Query + string.Format(@" AND SPD.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0} OR (CUSTOMER_CODE IN ({1}) AND GROUP_SKU_FLAG='I')) ", customerConditionQuery, string.Join(",", customers));
            }

            if (model.EmployeeFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SPD.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", model.EmployeeFilter).ToString());
            }
            if (model.DivisionFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SPD.DIVISION_CODE IN ('{0}')", string.Join("','", model.DivisionFilter).ToString());
            }

            if (model.BranchFilter.Count > 0)
            {
                Query += string.Format(@" AND SPD.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
            }
            Query += FiscalYearDateFilter()+ $@" GROUP BY MONTH,MONTHINT,YEAR,COMPANY_CODE";
            var result = new List<PlanReportModel>();
            try
            {
                result = _dbContext.SqlQuery<PlanReportModel>(Query).ToList();
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public List<PlanReportModel> getEmployeeWiseCollectionPlanChart(ReportFiltersModel model)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var branchCode = _workcontext.CurrentUserinformation.branch_code;
            var userId = _workcontext.CurrentUserinformation.User_id;

            var superFlagQuery = $@"SELECT SUPER_USER_FLAG FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE = '{companyCode}'";
            var superFlag = _dbContext.SqlQuery<string>(superFlagQuery).FirstOrDefault();
            var loginEmpCodeQuery = $@"SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE DELETED_FLAG='N' AND USER_NO='{userId}' AND COMPANY_CODE='{companyCode}'";
            var loginEmpCode = _dbContext.SqlQuery<string>(loginEmpCodeQuery).FirstOrDefault();


            var Query = $@"SELECT SUM(PER_DAY_AMOUNT) PER_DAY_AMOUNT,
                                  --SUM(PER_DAY_QUANTITY) PER_DAY_QUANTITY,
                                  EMPLOYEE_CODE,EMPLOYEE_EDESC as DESCRIPTION,COMPANY_CODE
                                  FROM TEMP_PL_COLLECTION_PLAN_REPORT SPD
                                WHERE SPD.COMPANY_CODE='{companyCode}' AND SPD.PER_DAY_AMOUNT>0 ";

            // added by chandra for load  only related report of logined person if not admin
            if (superFlag != "Y" && model.EmployeeFilter.Count == 0)
            {
                Query += $@" AND SPD.EMPLOYEE_CODE IN(
                    SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE='{companyCode}'
                     UNION ALL
                    SELECT EMPLOYEE_CODE FROM(
                    SELECT EMPLOYEE_CODE,COMPANY_CODE FROM HR_EMPLOYEE_TREE ET 
                    CONNECT BY PRIOR ET.EMPLOYEE_CODE=ET.PARENT_EMPLOYEE_CODE
                     START WITH PARENT_EMPLOYEE_CODE='{loginEmpCode}' )X WHERE X.COMPANY_CODE='{companyCode}'
                    )";
            }

            if (model.CustomerFilter.Count > 0)
            {
                var customers = model.CustomerFilter;
                var customerConditionQuery = string.Empty;
                for (int i = 0; i < customers.Count; i++)
                {
                    if (i == 0)
                        customerConditionQuery += string.Format("MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%' from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    else
                    {
                        customerConditionQuery += string.Format(" OR  MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    }
                }

                Query = Query + string.Format(@" AND SPD.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0} OR (CUSTOMER_CODE IN ({1}) AND GROUP_SKU_FLAG='I')) ", customerConditionQuery, string.Join(",", customers));
            }

            if (model.EmployeeFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SPD.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", model.EmployeeFilter).ToString());
            }
            if (model.DivisionFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SPD.DIVISION_CODE IN ('{0}')", string.Join("','", model.DivisionFilter).ToString());
            }

            if (model.BranchFilter.Count > 0)
            {
                Query += string.Format(@" AND SPD.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
            }

            Query +=FiscalYearDateFilter()+ $@" GROUP BY EMPLOYEE_CODE,EMPLOYEE_EDESC,COMPANY_CODE";

            var result = new List<PlanReportModel>();
            try
            {
                result = _dbContext.SqlQuery<PlanReportModel>(Query).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public List<PlanReportModel> getEmployeeCustomerWiseCollectionPlanChart(ReportFiltersModel model,string EmpCode)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            //var branchCode = _workcontext.CurrentUserinformation.branch_code;
            //var userId = _workcontext.CurrentUserinformation.User_id;

           // var superFlagQuery = $@"SELECT SUPER_USER_FLAG FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE = '{companyCode}'";
            //var superFlag = _dbContext.SqlQuery<string>(superFlagQuery).FirstOrDefault();
            //var loginEmpCodeQuery = $@"SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE DELETED_FLAG='N' AND USER_NO='{userId}' AND COMPANY_CODE='{companyCode}'";
            //var loginEmpCode = _dbContext.SqlQuery<string>(loginEmpCodeQuery).FirstOrDefault();


            var Query = $@"SELECT SUM(PER_DAY_AMOUNT) PER_DAY_AMOUNT,
                                  --SUM(PER_DAY_QUANTITY) PER_DAY_QUANTITY,
                                  EMPLOYEE_EDESC,CUSTOMER_EDESC as DESCRIPTION,COMPANY_CODE
                                  FROM TEMP_PL_COLLECTION_PLAN_REPORT SPD
                                WHERE SPD.COMPANY_CODE='{companyCode}' AND EMPLOYEE_CODE='{EmpCode}' AND SPD.PER_DAY_AMOUNT>0 ";

            // added by chandra for load  only related report of logined person if not admin
            //if (superFlag != "Y" && model.EmployeeFilter.Count == 0)
            //{
            //    Query += $@" AND SPD.EMPLOYEE_CODE IN(
            //        SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE='{companyCode}'
            //         UNION ALL
            //        SELECT EMPLOYEE_CODE FROM(
            //        SELECT EMPLOYEE_CODE,COMPANY_CODE FROM HR_EMPLOYEE_TREE ET 
            //        CONNECT BY PRIOR ET.EMPLOYEE_CODE=ET.PARENT_EMPLOYEE_CODE
            //         START WITH PARENT_EMPLOYEE_CODE='{loginEmpCode}' )X WHERE X.COMPANY_CODE='{companyCode}'
            //        )";
            //}

            if (model.CustomerFilter.Count > 0)
            {
                var customers = model.CustomerFilter;
                var customerConditionQuery = string.Empty;
                for (int i = 0; i < customers.Count; i++)
                {
                    if (i == 0)
                        customerConditionQuery += string.Format("MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%' from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    else
                    {
                        customerConditionQuery += string.Format(" OR  MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    }
                }

                Query = Query + string.Format(@" AND SPD.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0} OR (CUSTOMER_CODE IN ({1}) AND GROUP_SKU_FLAG='I')) ", customerConditionQuery, string.Join(",", customers));
            }

            if (model.EmployeeFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SPD.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", model.EmployeeFilter).ToString());
            }
            if (model.DivisionFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SPD.DIVISION_CODE IN ('{0}')", string.Join("','", model.DivisionFilter).ToString());
            }

            if (model.BranchFilter.Count > 0)
            {
                Query += string.Format(@" AND SPD.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
            }

            Query += FiscalYearDateFilter() + $@" GROUP BY CUSTOMER_EDESC,EMPLOYEE_EDESC,COMPANY_CODE";

            var result = new List<PlanReportModel>();
            try
            {
                result = _dbContext.SqlQuery<PlanReportModel>(Query).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        private string FiscalYearDateFilter()
        {
            string result = string.Empty;
            string FiscalYearCode = ConfigurationManager.AppSettings["FiscalYear"].ToString();
            string sqlQuery = $@"SELECT START_DATE,END_DATE FROM PL_FISCAL_YEAR_CODE WHERE FISCAL_YEAR_CODE='{FiscalYearCode}' AND ACTIVE='Y' AND COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}'";
            var data = _dbContext.SqlQuery<FiscalYearModel>(sqlQuery).FirstOrDefault();
            if (data != null)
            {

                result = $@" AND SPD.PLAN_DATE >= TO_DATE('{data.START_DATE.ToString("MM/dd/yyyy")}','MM/DD/YYYY') AND SPD.PLAN_DATE <= TO_DATE('{data.END_DATE.ToString("MM/dd/yyyy")}','MM/DD/YYYY') ";
            }
            return result;
        }
        public List<CollectionPlanTreeReportModel> GetTreewiseCustomerCollectionPlanReport(ReportFiltersModel reportFilters, User userInfo)
        {
            var userId = userInfo.User_id;
            var superFlagQuery = $@"SELECT SUPER_USER_FLAG FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE = '{userInfo.company_code}'";
            var superFlag = _dbContext.SqlQuery<string>(superFlagQuery).FirstOrDefault();
            var loginEmpCodeQuery = $@"SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE DELETED_FLAG='N' AND USER_NO='{userId}' AND COMPANY_CODE='{userInfo.company_code}'";
            var loginEmpCode = _dbContext.SqlQuery<string>(loginEmpCodeQuery).FirstOrDefault();


            var companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);

            var customerQuery = string.Empty;
            var dataQuery = string.Empty;
            customerQuery = $@"SELECT CUSTOMER_CODE,CUSTOMER_EDESC,PRE_CUSTOMER_CODE,MASTER_CUSTOMER_CODE,GROUP_SKU_FLAG FROM SA_CUSTOMER_SETUP WHERE DELETED_FLAG = 'N' AND COMPANY_CODE = {companyCode} ORDER BY  PRE_CUSTOMER_CODE ASC";
            var customersList = _dbContext.SqlQuery<CollectionPlanTreeReportModel>(customerQuery).ToList();

            List<CollectionPlanTreeReportModel> newList = new List<CollectionPlanTreeReportModel>();

            dataQuery = $@" select V.CUSTOMER_CODE,SC.CUSTOMER_EDESC,V.PER_DAY_AMOUNT TOTAL_SALES,sc.PRE_CUSTOMER_CODE ,SC.MASTER_CUSTOMER_CODE  from TEMP_PL_COLLECTION_PLAN_REPORT V JOIN SA_CUSTOMER_SETUP SC ON V.CUSTOMER_CODE = SC.CUSTOMER_CODE and v.company_code=sc.company_code WHERE V.COMPANY_CODE = {companyCode} AND sc.DELETED_FLAG = 'N' AND V.PER_DAY_AMOUNT>0  AND V.PLAN_DATE>= TO_DATE('{reportFilters.FromDate}', 'YYYY-MON-DD')
                    AND V.PLAN_DATE <= TO_DATE('{ reportFilters.ToDate}',' YYYY-MON-DD')";
            if (superFlag != "Y")
            {
                dataQuery += $@" AND V.EMPLOYEE_CODE IN(
                    SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE='{userInfo.company_code}'
                     UNION ALL
                    SELECT EMPLOYEE_CODE FROM(
                    SELECT EMPLOYEE_CODE,COMPANY_CODE FROM HR_EMPLOYEE_TREE ET 
                    CONNECT BY PRIOR ET.EMPLOYEE_CODE=ET.PARENT_EMPLOYEE_CODE
                     START WITH PARENT_EMPLOYEE_CODE='{loginEmpCode}' )X WHERE X.COMPANY_CODE='{userInfo.company_code}'
                    )";
            }
            var dataList = _dbContext.SqlQuery<CollectionPlanTreeReportModel>(dataQuery).ToList();
            if (dataList.Count > 0)
            {
                foreach (var customer in customersList)
                {
                    if (customer.GROUP_SKU_FLAG == "I")
                    {
                        var forlastItems = dataList.Where(x => x.CUSTOMER_CODE == customer.CUSTOMER_CODE).ToList();
                        customer.TOTAL_SALES = forlastItems.Sum(x => x.TOTAL_SALES);
                    }
                    else
                    {
                        var filteredData = dataList.Where(x => x.PRE_CUSTOMER_CODE.StartsWith(customer.MASTER_CUSTOMER_CODE)).ToList();
                        customer.TOTAL_SALES = filteredData.Sum(x => x.TOTAL_SALES);
                    }
                    customer.PRE_CUSTOMER_CODE = Convert.ToInt64(customer.PRE_CUSTOMER_CODE.Replace(".", "")).ToString();
                    customer.MASTER_CUSTOMER_CODE = Convert.ToInt64(customer.MASTER_CUSTOMER_CODE.Replace(".", "")).ToString();

                }
            }

            customersList.RemoveAll(x => x.TOTAL_SALES == 0);
            var totalHeader = customersList.Where(x => x.PRE_CUSTOMER_CODE == "0").ToList();
            var totalSales = totalHeader.Sum(x => x.TOTAL_SALES);
            CollectionPlanTreeReportModel totalAmount = new CollectionPlanTreeReportModel();
            totalAmount.CUSTOMER_EDESC = "Total";
            totalAmount.TOTAL_SALES = totalSales;
            totalAmount.PRE_CUSTOMER_CODE = "0";
            customersList.Add(totalAmount);
            return customersList;

        }
        #endregion
    }
    public class FiscalYearModel
    {
        public string FISCAL_YEAR_CODE { get; set; }
        public DateTime START_DATE { get; set; }
        public DateTime END_DATE { get; set; }
    }
}
