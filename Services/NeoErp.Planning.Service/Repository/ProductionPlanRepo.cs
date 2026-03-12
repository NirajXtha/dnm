using NeoErp.Planning.Service.Interface;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoErp.Planning.Service.Models;
using NeoErp.Core;
using NeoErp.Data;
using NeoErp.Core.Models;
using System.Collections.Generic;
using NeoErp.Core.Domain;
using System.Web;
using System.Xml.Linq;

namespace NeoErp.Planning.Service.Repository
{
    public class ProductionPlanRepo : IProductionPlanRepo
    {
        private IDbContext _dbContext;
        private IWorkContext _workcontext;
        public ProductionPlanRepo(IDbContext dbContext, IWorkContext workContext)
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

        #region Production Plan
        public string SaveProductionPlan(List<savePlan> sv, ProductionPlan sp)
        {
            try
            {
                var PRODUCTION_QTY = sp.SALES_AMOUNT;
                var userID = _workcontext.CurrentUserinformation.User_id;
                var branchCode = _workcontext.CurrentUserinformation.branch_code;
                var spbranchCode = sp.branchCode == "" ? _workcontext.CurrentUserinformation.branch_code : sp.branchCode;
                var companyCode = _workcontext.CurrentUserinformation.company_code;
                var result = "";
                int planCode = 0;
                string time_frame_code = sp.TIME_FRAME_CODE;

                var company_code = _workcontext.CurrentUserinformation.company_code;

                // first insertion to PL_PRO_PLAN table
                string checkPlanQuery = $@"SELECT COUNT(*) FROM PL_PRO_PLAN WHERE PLAN_EDESC='{sp.PLAN_EDESC}' 
                AND TIME_FRAME_CODE='{sp.TIME_FRAME_CODE}' AND START_DATE=TO_DATE('{sp.START_DATE}','YYYY-Mon-DD') AND END_DATE=TO_DATE('{sp.END_DATE}','YYYY-Mon-DD')";
                var checkPlanQueryResult = this._dbContext.SqlQuery<int>(checkPlanQuery).First();
                if (checkPlanQueryResult == 0 && string.IsNullOrEmpty(sp.PLAN_CODE))
                {
                    string sales_plan_query = $@"INSERT INTO PL_PRO_PLAN(PLAN_CODE,PLAN_EDESC,PLAN_NDESC,PRODUCTION_QTY,TIME_FRAME_CODE,
                    CALENDAR_TYPE,START_DATE,END_DATE,REMARKS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,
                    LAST_MODIFIED_BY,LAST_MODIFIED_DATE,
                    DELETED_FLAG)
                    VALUES((SELECT COALESCE(MAX(PLAN_CODE)+1, MAX(PLAN_CODE) + 1, 1) FROM PL_PRO_PLAN),'{sp.PLAN_EDESC}','','{PRODUCTION_QTY}','{sp.TIME_FRAME_CODE}','{sp.CALENDAR_TYPE}',
                    TO_DATE('{sp.START_DATE}','YYYY-Mon-DD'),TO_DATE('{sp.END_DATE}','YYYY-Mon-DD'),'{sp.REMARKS}','{companyCode}','{branchCode}','{userID}',
                    TO_DATE('{DateTime.Today.ToString("MM/dd/yyyy")}','MM/DD/YYYY'),'{userID}',TO_DATE('{DateTime.Today.ToString("MM/dd/yyyy")}','MM/DD/YYYY'),'N')";

                    var insertResult = this._dbContext.ExecuteSqlCommand(sales_plan_query);

                    string fetchProductionPlan = $@"SELECT PLAN_CODE FROM PL_PRO_PLAN WHERE PLAN_EDESC='{sp.PLAN_EDESC}' 
                AND TIME_FRAME_CODE='{sp.TIME_FRAME_CODE}' AND START_DATE=TO_DATE('{sp.START_DATE}','YYYY-Mon-DD') AND END_DATE=TO_DATE('{sp.END_DATE}','YYYY-Mon-DD')";
                    planCode = this._dbContext.SqlQuery<int>(fetchProductionPlan).First();

                }
                else
                {
                    if (!string.IsNullOrEmpty(sp.PLAN_CODE) && sp.PLAN_CODE != "0")
                    {
                        planCode = Convert.ToInt32(sp.PLAN_CODE);
                        string updateQuery = $@"UPDATE PL_PRO_PLAN SET PLAN_EDESC = '{sp.PLAN_EDESC}' , REMARKS = '{sp.REMARKS}' 
                        ,TIME_FRAME_CODE='{sp.TIME_FRAME_CODE}' , CALENDAR_TYPE='{sp.CALENDAR_TYPE}', START_DATE=TO_DATE('{sp.START_DATE}','YYYY-MON-DD') , END_DATE=TO_DATE('{sp.END_DATE}','YYYY-MON-DD')
                        WHERE PLAN_CODE='{sp.PLAN_CODE}'";
                        var update_result = this._dbContext.ExecuteSqlCommand(updateQuery);
                    }
                    else
                    {
                        string plancode_query = $@"SELECT PLAN_CODE FROM PL_PRO_PLAN WHERE PLAN_EDESC='{sp.PLAN_EDESC}' 
                AND TIME_FRAME_CODE='{sp.TIME_FRAME_CODE}' AND START_DATE=TO_DATE('{sp.START_DATE}','YYYY-Mon-DD') AND END_DATE=TO_DATE('{sp.END_DATE}','YYYY-Mon-DD')";
                        planCode = this._dbContext.SqlQuery<int>(plancode_query).First();
                    }

                    string delete_detail_already_set = $@"DELETE FROM PL_PRO_PLAN_DTL WHERE PLAN_CODE='{planCode}'";
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
                                '{companyCode}','{spbranchCode}','{userID}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/DD/YYYY'),'','','N' FROM DUAL UNION ALL ";

                                sbInsertQuery += insertinto_plandtl;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (!ex.Message.Contains("unique constraint"))
                                throw;
                            else
                            {
                                var sqlquery = $@"UPDATE PL_PRO_PLAN_DTL SET TARGET_VALUE='{freq.fvalue}',LAST_MODIFIED_BY='{userID}', LAST_MODIFIED_DATE=TO_DATE('{DateTime.Now:MM/dd/yyyy/ HH:mm:ss}', 'mm/dd/yyyy hh24:mi:ss') where PLAN_CODE='{planCode}' AND ITEM_CODE='{item.itemCode}' AND TIME_FRAME_CODE='{time_frame_code}' AND TIME_FRAME_VALUE='{freq.fname}'";
                            }
                        }
                    }
                    if (itteration <= (sv.Count() - remainderCount))
                    {
                        while (itemCount == itemRange)
                        {
                            string query = @"insert into PL_PRO_PLAN_DTL(PLAN_CODE,PLAN_DATE,PER_DAY_QUANTITY,ITEM_CODE,
                                            DIVISION_CODE,FREQUENCY_JSON,REMARKS,
                                            COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,DELETED_FLAG) " + sbInsertQuery;
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
                            string query = @"insert into PL_PRO_PLAN_DTL(PLAN_CODE,PLAN_DATE,PER_DAY_QUANTITY,ITEM_CODE,
                    DIVISION_CODE,FREQUENCY_JSON,REMARKS,
                    COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,DELETED_FLAG) " + sbInsertQuery;
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
                update_amout_quantity = $@"UPDATE PL_PRO_PLAN PSP SET PSP.PRODUCTION_QTY = (
                        SELECT ROUND(SUM(SPD.PER_DAY_QUANTITY),2) FROM PL_PRO_PLAN_DTL SPD, IP_ITEM_MASTER_SETUP IMS
                        WHERE 
                        IMS.GROUP_SKU_FLAG ='I'
                        AND SPD.COMPANY_CODE = IMS.COMPANY_CODE
                        AND SPD.ITEM_CODE = IMS.ITEM_CODE
                        AND SPD.PLAN_CODE='{planCode}')
                        WHERE PSP.PLAN_CODE='{planCode}'";
                this._dbContext.ExecuteSqlCommand(update_amout_quantity);
                return result.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ProductionPlan> getAllProductionPlans(ReportFiltersModel filters)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var userId = _workcontext.CurrentUserinformation.User_id;
            string query = $@"SELECT DISTINCT TO_CHAR(PL.PLAN_CODE) PLAN_CODE, PL.PLAN_EDESC, PL.PLAN_NDESC,
                             TO_CHAR(PL.PRODUCTION_QTY) SALES_AMOUNT, TO_CHAR(PL.TIME_FRAME_CODE) TIME_FRAME_CODE, 
                             PL.CALENDAR_TYPE,TO_CHAR(PL.START_DATE,'DD-MON-YYYY') START_DATE,TO_CHAR(PL.END_DATE,'DD-MON-YYYY') END_DATE, PL.REMARKS,
                             PLD.DIVISION_CODE, TO_CHAR(PLD.BRANCH_CODE), DS.DIVISION_EDESC, FS.BRANCH_EDESC FROM PL_PRO_PLAN PL, 
                             PL_PRO_PLAN_DTL PLD, FA_DIVISION_SETUP DS, FA_BRANCH_SETUP FS 
                             WHERE PL.DELETED_FLAG = 'N' AND PL.PLAN_CODE = PLD.PLAN_CODE 
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
            query += $@" ORDER BY TO_NUMBER (PL.PLAN_CODE)  DESC";

            List<ProductionPlan> spList = new List<ProductionPlan>();

            spList = this._dbContext.SqlQuery<ProductionPlan>(query).ToList();
            return spList;
        }

        public List<ProductionPlan> getAllProductionPlans(string customercode, string employeecode, string divisioncode, string branchcode, string startdate, string enddate)
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
                            FROM PL_PRO_PLAN PL, PL_PRO_PLAN_DTL PLD, SA_CUSTOMER_SETUP CS , FA_DIVISION_SETUP DS, HR_EMPLOYEE_SETUP ES, FA_BRANCH_SETUP FS
                             WHERE PL.DELETED_FLAG = 'N' 
                             AND PL.PLAN_CODE = PLD.PLAN_CODE
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

            List<ProductionPlan> spList = new List<ProductionPlan>();
            spList = this._dbContext.SqlQuery<ProductionPlan>(query).ToList();

            string masterplan_query = $@"SELECT DISTINCT TO_CHAR(MSP.MASTER_PLAN_CODE)||'_MP' PLAN_CODE,
                                        MSP.MASTER_PLAN_EDESC PLAN_EDESC ,
                                        TO_CHAR (MSP.START_DATE, 'DD-MON-YYYY') START_DATE,
                                             TO_CHAR (MSP.END_DATE, 'DD-MON-YYYY') END_DATE
                                    FROM PL_MASTER_SALES_PLAN MSP ,PL_PRO_PLAN_MAP SPM
                                    WHERE MSP.MASTER_PLAN_CODE=SPM.MASTER_PLAN_CODE ";
            if (!string.IsNullOrEmpty(startdate) && !string.IsNullOrEmpty(enddate))
            {
                masterplan_query += $@" AND MSP.START_DATE = TO_DATE('{startdate}','YYYY-MON-DD')
                             AND MSP.END_DATE = TO_DATE('{enddate}','YYYY-MON-DD') ";
            }
            List<ProductionPlan> mpList = new List<ProductionPlan>();
            mpList = this._dbContext.SqlQuery<ProductionPlan>(masterplan_query).ToList();
            if (mpList.Count > 0)
            {
                foreach (var item in mpList)
                {
                    spList.Add(new ProductionPlan
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

        public bool deleteProductionPlan(int planCode)
        {
            try
            {
                string deleteYes_salesPlan = $@"UPDATE PL_PRO_PLAN SET DELETED_FLAG='Y' WHERE PLAN_CODE='{planCode}'";
                string deleteYes_salesPlanDtl = $@"UPDATE PL_PRO_PLAN_DTL SET DELETED_FLAG='Y' WHERE PLAN_CODE='{planCode}'";
                this._dbContext.ExecuteSqlCommand(deleteYes_salesPlanDtl);
                this._dbContext.ExecuteSqlCommand(deleteYes_salesPlan);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public PL_PRO_PLAN GetPlanDetailValueByPlanCode(int plancode)
        {
            var company_code = this._workcontext.CurrentUserinformation.company_code;
            PL_PRO_PLAN ProductionPlan = new PL_PRO_PLAN();
            string queryPlan = $@"SELECT 
                PLAN_CODE PLAN_CODE, PLAN_EDESC,PLAN_NDESC,PRODUCTION_QTY SALES_AMOUNT,TIME_FRAME_CODE TIME_FRAME_CODE,
                (SELECT TIME_FRAME_EDESC FROM PL_TIME_FRAME WHERE TIME_FRAME_CODE = PSP.TIME_FRAME_CODE) TIME_FRAME_EDESC,
                CALENDAR_TYPE,TO_CHAR(START_DATE) START_DATE,TO_CHAR(END_DATE) END_DATE,REMARKS,COMPANY_CODE,BRANCH_CODE
                FROM PL_PRO_PLAN PSP
                WHERE PLAN_CODE= '{plancode}'";

            ProductionPlan = this._dbContext.SqlQuery<PL_PRO_PLAN>(queryPlan).FirstOrDefault();

            List<ProductionPlanDetail> salesPlanDetailList = new List<ProductionPlanDetail>();
            string ProductionPlanDetailQuery = $@"SELECT PLAN_CODE,PLAN_DATE PLAN_DATE,PER_DAY_QUANTITY PER_DAY_QUANTITY,
                ITEM_CODE,DIVISION_CODE,REMARKS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE CREATED_DATE, FREQUENCY_JSON
                FROM PL_PRO_PLAN_DTL WHERE PLAN_CODE = '{plancode}' AND DELETED_FLAG='N'";
            salesPlanDetailList = this._dbContext.SqlQuery<ProductionPlanDetail>(ProductionPlanDetailQuery).ToList();

            List<ProductionPlanItems> salesPlanItem = new List<ProductionPlanItems>();
            string ProductionPlanItemsQuery = $@"SELECT 
            DISTINCT TO_CHAR(PSPD.ITEM_CODE) ITEM_CODE,INITCAP(IMS.ITEM_EDESC) ITEM_EDESC,TO_CHAR(IMS.MASTER_ITEM_CODE) MASTER_ITEM_CODE,TO_CHAR(IMS.PRE_ITEM_CODE) PRE_ITEM_CODE,TO_CHAR(IMS.GROUP_SKU_FLAG) GROUP_SKU_FLAG
            FROM PL_PRO_PLAN_DTL PSPD,IP_ITEM_MASTER_SETUP IMS WHERE PSPD.PLAN_CODE='{plancode}' AND PSPD.DELETED_FLAG='N' AND IMS.DELETED_FLAG='N' AND IMS.ITEM_CODE=PSPD.ITEM_CODE
            ORDER BY PRE_ITEM_CODE,MASTER_ITEM_CODE,ITEM_CODE";
            salesPlanItem = this._dbContext.SqlQuery<ProductionPlanItems>(ProductionPlanItemsQuery).ToList();

            ProductionPlan.selectedItemsList = salesPlanItem;
            ProductionPlan.salesPlanDetail = salesPlanDetailList;

            return ProductionPlan;
        }
        //public List<ProductSetupModel> getAllProductsWithChildItem()
        //{
        //    var company_code = _workcontext.CurrentUserinformation.company_code;
        //    var branch_code = _workcontext.CurrentUserinformation.branch_code;

        //    string query = @"SELECT Distinct LEVEL, 
        //                    INITCAP(ITEM_EDESC) AS ItemName,
        //                    ITEM_CODE AS ItemCode,
        //                    MASTER_ITEM_CODE AS MasterItemCode, 
        //                    PRE_ITEM_CODE AS PreItemCode, 
        //                    GROUP_SKU_FLAG AS GroupFlag,
        //                    (SELECT COUNT(*) FROM IP_ITEM_MASTER_SETUP WHERE  
        //                     COMPANY_CODE='" + company_code + @"'  AND DELETED_FLAG='N' AND PRE_ITEM_CODE = ims.MASTER_ITEM_CODE) as Childrens 
        //                    FROM IP_ITEM_MASTER_SETUP ims
        //                    WHERE ims.DELETED_FLAG = 'N'
        //                    AND ims.COMPANY_CODE = '" + company_code + @"' 
        //                    --AND LEVEL = {0}
        //                    START WITH PRE_ITEM_CODE = (select MASTER_ITEM_CODE from ip_item_master_setup where LOWER(item_edesc) like '%finish good%' and ROWNUM<2)
        //                    CONNECT BY PRIOR MASTER_ITEM_CODE = PRE_ITEM_CODE";
        //    var productListNodes = _dbContext.SqlQuery<ProductSetupModel>(query).ToList();
        //    return productListNodes;
        //}

        public List<ProductSetupModel> ProductListAllNodes(User userinfo)
        {

            string query = @"SELECT LEVEL, 
                 INITCAP(ITEM_EDESC) AS ItemName,
                 ITEM_CODE AS ItemCode,
                 MASTER_ITEM_CODE AS MasterItemCode, 
                 PRE_ITEM_CODE AS PreItemCode, 
                 GROUP_SKU_FLAG AS GroupFlag,
                (SELECT COUNT(*) FROM IP_ITEM_MASTER_SETUP WHERE  
                GROUP_SKU_FLAG='G' AND COMPANY_CODE='" + userinfo.company_code + @"' AND DELETED_FLAG='N' AND PRE_ITEM_CODE = ims.MASTER_ITEM_CODE) as Childrens 
                 FROM IP_ITEM_MASTER_SETUP ims
                 WHERE ims.DELETED_FLAG = 'N' 
                 AND LEVEL=1
                 AND ims.COMPANY_CODE = '" + userinfo.company_code + @"'
                 AND GROUP_SKU_FLAG = 'G'
                 AND LOWER(ims.ITEM_EDESC) LIKE '%finish good%'
                 START WITH PRE_ITEM_CODE = '00'
                 CONNECT BY PRIOR MASTER_ITEM_CODE = PRE_ITEM_CODE";
            var productListNodes = _dbContext.SqlQuery<ProductSetupModel>(query).ToList();
            return productListNodes;
        }
        public List<ProductSetupModel> getAllProductsWithChildItem()
        {
            var company_code = _workcontext.CurrentUserinformation.company_code;
            var branch_code = _workcontext.CurrentUserinformation.branch_code;

            string item_filter_condition = "";
            try
            {
                string xmlpath = HttpContext.Current.Server.MapPath("~/Areas/NeoERP.Planning/ProductCondition.xml");
                var xml = XDocument.Load(xmlpath);
                var condition_query = from c in xml.Root.Descendants("Vendor")
                                      where (string)c.Attribute("ID") == "02"
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

            string query = @"SELECT Distinct LEVEL, 
                            INITCAP(ITEM_EDESC) AS ItemName,
                            ITEM_CODE AS ItemCode,
                            MASTER_ITEM_CODE AS MasterItemCode, 
                            PRE_ITEM_CODE AS PreItemCode, 
                            GROUP_SKU_FLAG AS GroupFlag,
                            (SELECT COUNT(*) FROM IP_ITEM_MASTER_SETUP WHERE  
                             COMPANY_CODE='" + company_code + @"'  AND DELETED_FLAG='N' AND PRE_ITEM_CODE = ims.MASTER_ITEM_CODE) as Childrens 
                            FROM IP_ITEM_MASTER_SETUP ims
                            WHERE ims.DELETED_FLAG = 'N' " + item_filter_condition + @"
                            AND ims.COMPANY_CODE = '" + company_code + @"' 
                            --AND LEVEL = {0}
                            START WITH PRE_ITEM_CODE = '00'
                            CONNECT BY PRIOR MASTER_ITEM_CODE = PRE_ITEM_CODE AND COMPANY_CODE = '" + company_code + "'";
            var productListNodes = _dbContext.SqlQuery<ProductSetupModel>(query).ToList();
            return productListNodes;
        }

        public List<ProductSetupModel> GetProductsListByProductCode(string level, string masterProductCode, User userinfo)
        {
            string query = string.Format(@"SELECT LEVEL, 
            INITCAP(ITEM_EDESC) AS ItemName,
            ITEM_CODE AS ItemCode,
            MASTER_ITEM_CODE AS MasterItemCode, 
            PRE_ITEM_CODE AS PreItemCode, 
            GROUP_SKU_FLAG AS GroupFlag,
            (SELECT COUNT(*) FROM IP_ITEM_MASTER_SETUP WHERE  
             COMPANY_CODE='" + userinfo.company_code + @"' AND DELETED_FLAG='N' AND PRE_ITEM_CODE = ims.MASTER_ITEM_CODE) as Childrens 
            FROM IP_ITEM_MASTER_SETUP ims
            WHERE ims.DELETED_FLAG = 'N' 
            AND ims.COMPANY_CODE = '" + userinfo.company_code + @"'
            AND LEVEL = {0}
            START WITH PRE_ITEM_CODE = '{1}'
            CONNECT BY PRIOR MASTER_ITEM_CODE = PRE_ITEM_CODE AND COMPANY_CODE = '" + userinfo.company_code + "'" +
            "ORDER SIBLINGS BY ITEM_EDESC", level.ToString(), masterProductCode.ToString());
            var productListNodes = _dbContext.SqlQuery<ProductSetupModel>(query).ToList();
            return productListNodes;
        }
        public List<PlanSalesRefrenceModel> getSalesItemDataForRefrence(string itemList, string startDate, string endDate, string customerCode, string divisionCode, string branchCode, string dateFormat, string frequency)
        {
            var userNo = _workcontext.CurrentUserinformation.User_id;
            var result = new List<PlanSalesRefrenceModel>();
            try
            {
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

                if (dateFormat.ToUpper() == "BS" || dateFormat.ToUpper() == "LOC")
                {
                    if (frequency.ToLower() == "week")
                    {
                        refrenceQuery = $@" SELECT  'freqItemNum_'||SI.ITEM_CODE ||'_WEEK_' || NEPALI_WEEKS.WEEKS ||'_'||  TO_CHAR(SUBSTR(BS_DATE(SI.SALES_DATE),0,4) )  COLNAME, SI.ITEM_CODE,
                                         IMS.ITEM_EDESC,
                                         SI.CUSTOMER_CODE,
                                         SI.DIVISION_CODE,
                                         SI.BRANCH_CODE,
                                        NEPALI_WEEKS.WEEKS,
                                         TO_CHAR(SUM (SI.CALC_TOTAL_PRICE)) AMOUNT,
                                        TO_CHAR(SUM (SI.CALC_QUANTITY)) QTY
                                    FROM SA_SALES_INVOICE SI, IP_ITEM_MASTER_SETUP IMS, (SELECT DAYS, ROWNUM  DAY_NO , TRUNC((ROWNUM+7)/7)  WEEKS
                                  FROM (SELECT FISCAL_YEAR.START_DATE + ROWNUM - 1 DAYS
                                          FROM ALL_OBJECTS,
                                               (SELECT START_DATE, END_DATE
                                                  FROM HR_FISCAL_YEAR_CODE
                                                 WHERE SYSDATE BETWEEN START_DATE AND END_DATE) FISCAL_YEAR
                                         WHERE ROWNUM <= FISCAL_YEAR.END_DATE - FISCAL_YEAR.START_DATE + 1) )NEPALI_WEEKS
                                WHERE IMS.ITEM_CODE IN ({itemList}) {optionalCondition} 
                                AND SI.ITEM_CODE = IMS.ITEM_CODE
                                    AND NEPALI_WEEKS.DAYS = SI.SALES_DATE
                                    AND SI.SALES_DATE BETWEEN to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD')
                                GROUP BY SI.ITEM_CODE,
                                         IMS.ITEM_EDESC,
                                         SI.CUSTOMER_CODE,
                                         SI.DIVISION_CODE,
                                         SI.BRANCH_CODE,
                                        NEPALI_WEEKS.WEEKS
                                        ,SUBSTR(BS_DATE(SI.SALES_DATE),0,4)  
                                 ORDER BY   NEPALI_WEEKS.WEEKS";
                    }
                    else if (frequency.ToLower() == "month")
                    {
                        refrenceQuery = $@"SELECT 'freqItemNum_'||SI.ITEM_CODE ||'_' ||  UPPER(fn_bs_month(SUBSTR(BS_DATE(SI.PLAN_DATE),6,2))) ||'_'|| SUBSTR(BS_DATE(SI.PLAN_DATE),0,4) COLNAME ,
                                     SUBSTR(BS_DATE(SI.PLAN_DATE),6,2)NEPALI_MONTH,
                                     SUBSTR(BS_DATE(SI.PLAN_DATE),0,4)NEPALI_YEAR,
                                     SI.ITEM_CODE,
                                     IMS.ITEM_EDESC,
                                     SI.CUSTOMER_CODE,
                                     SI.DIVISION_CODE,
                                     SI.BRANCH_CODE,
                                    -- TO_CHAR(SUM (SI.CALC_TOTAL_PRICE)) AMOUNT,
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
                                     SUBSTR(BS_DATE(SI.PLAN_DATE),0,4),
                                     SUBSTR(BS_DATE(SI.PLAN_DATE),6,2)
                                    ORDER BY NEPALI_YEAR,ITEM_CODE,NEPALI_MONTH ASC";
                    }
                }
                else if (dateFormat.ToUpper() == "AD" || dateFormat.ToUpper() == "ENG")
                {
                    if (frequency.ToLower() == "week")
                    {
                        refrenceQuery = $@"SELECT 'freqItemNum_'||SI.ITEM_CODE ||'_WEEK_' ||  TO_CHAR(SI.SALES_DATE,'IW') ||'_'||  TO_CHAR(SI.SALES_DATE,'YYYY') COLNAME, SI.ITEM_CODE,
                                       IMS.ITEM_EDESC,
                                     SI.CUSTOMER_CODE,
                                     SI.DIVISION_CODE,
                                     SI.BRANCH_CODE,
                                     TO_CHAR(SI.SALES_DATE,'YYYY') AD_YEAR,
                                     TO_CHAR(SI.SALES_DATE,'IW') SALES_WEEK,
                                     TO_CHAR(SUM (SI.CALC_TOTAL_PRICE)) AMOUNT,
                                     TO_CHAR(SUM (SI.CALC_QUANTITY)) QTY
                                FROM SA_SALES_INVOICE SI, IP_ITEM_MASTER_SETUP IMS
                                WHERE IMS.ITEM_CODE IN ({itemList}) {optionalCondition} 
                                AND SI.ITEM_CODE = IMS.ITEM_CODE
                                AND SI.SALES_DATE BETWEEN to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD')
                           GROUP BY SI.ITEM_CODE,
                             IMS.ITEM_EDESC,
                             SI.CUSTOMER_CODE,
                             SI.DIVISION_CODE,
                             SI.BRANCH_CODE,
                             TO_CHAR(SI.SALES_DATE,'YYYY'),
                             TO_CHAR(SI.SALES_DATE,'IW')
                     ORDER BY  TO_CHAR(SI.SALES_DATE,'YYYY'), TO_CHAR(SI.SALES_DATE,'IW')";
                    }
                    else if (frequency.ToLower() == "month")
                    {
                        refrenceQuery = $@"SELECT  'freqItemNum_'||SI.ITEM_CODE ||'_' ||  TO_CHAR(SI.SALES_DATE,'MON') ||'_'||  TO_CHAR(SI.SALES_DATE,'YYYY') COLNAME ,
                                     IMS.ITEM_EDESC,
                                     SI.CUSTOMER_CODE,
                                     SI.DIVISION_CODE,
                                     SI.BRANCH_CODE,
                                    -- SUBSTR(TO_BS(SI.SALES_DATE),7,7) NEPALI_MONTH,
                                     TO_CHAR(SUM (SI.CALC_TOTAL_PRICE)) AMOUNT,
                                     TO_CHAR(SUM (SI.CALC_QUANTITY)) QTY
                                FROM SA_SALES_INVOICE SI, IP_ITEM_MASTER_SETUP IMS
                                WHERE IMS.ITEM_CODE IN ({itemList}) {optionalCondition} 
                                AND SI.ITEM_CODE = IMS.ITEM_CODE
                                AND SI.SALES_DATE BETWEEN to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD')
                            GROUP BY SI.ITEM_CODE,
                                     IMS.ITEM_EDESC,
                                     SI.CUSTOMER_CODE,
                                     SI.DIVISION_CODE,
                                     SI.BRANCH_CODE,
                                     TO_CHAR(SI.SALES_DATE,'YYYY'),
                                     TO_CHAR(SI.SALES_DATE,'MON')";
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

        public List<PlanSalesRefrenceModel> getPorcumentItemDataForStock(string itemList, string startDate, string endDate, string customerCode, string divisionCode, string branchCode, string dateFormat, string frequency)
        {
            var userNo = _workcontext.CurrentUserinformation.User_id;
            var result = new List<PlanSalesRefrenceModel>();
            try
            {
                var refrenceQuery = string.Empty;
                string optionalCondition = string.Empty;

                if (!string.IsNullOrEmpty(branchCode))
                {
                    optionalCondition += $@"  AND SI.BRANCH_CODE in ({branchCode})";
                }

                refrenceQuery = $@"SELECT SI.ITEM_CODE,
                                    TO_CHAR(SUM(SI.IN_QUANTITY-SI.OUT_QUANTITY)) QTY
                                FROM V$VIRTUAL_STOCK_WIP_LEDGER SI, IP_ITEM_MASTER_SETUP IMS
                                WHERE IMS.ITEM_CODE IN ({itemList}) {optionalCondition} 
                                AND SI.ITEM_CODE = IMS.ITEM_CODE
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

        public bool cloneProductionPlan(string planCode, string planName, string customers, string employees, string branchs, string divisions, string remarks)
        {
            try
            {
                remarks = remarks.Replace("\"", " ");
                string existingPlanNameQuery = $@"SELECT PLAN_EDESC FROM PL_PRO_PLAN WHERE PLAN_CODE='{planCode}'";
                string existingPlanName = this._dbContext.SqlQuery<String>(existingPlanNameQuery).FirstOrDefault();
                if (existingPlanName != null)
                {
                    string copyPlanName = existingPlanName + "_copy";
                    if (!string.IsNullOrEmpty(planName))
                        copyPlanName = planName;
                    string maxPlanCodeQuery = "SELECT COALESCE(MAX(PLAN_CODE)+1, MAX(PLAN_CODE) + 1, 1) FROM PL_PRO_PLAN";
                    int maxPlanCode = this._dbContext.SqlQuery<int>(maxPlanCodeQuery).First();

                    string copySalesPlanQuery = $@"INSERT INTO PL_PRO_PLAN(PLAN_CODE,PLAN_EDESC,PLAN_NDESC,PRODUCTION_QTY,TIME_FRAME_CODE,
                    CALENDAR_TYPE,START_DATE,END_DATE,REMARKS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,
                    DELETED_FLAG)
                    SELECT '{maxPlanCode}','{copyPlanName}', PLAN_NDESC,PRODUCTION_QTY,TIME_FRAME_CODE,
                    CALENDAR_TYPE,START_DATE,END_DATE,'{remarks}',COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,
                    DELETED_FLAG FROM PL_PRO_PLAN WHERE PLAN_CODE='{planCode}'";
                    var insertCopiedPlan = this._dbContext.ExecuteSqlCommand(copySalesPlanQuery);

                    int copiedPlanCode = maxPlanCode;
                    if (copiedPlanCode > 0)
                    {
                        string copyinto_plandtl = $@"INSERT INTO PL_PRO_PLAN_DTL
                                (PLAN_CODE,PLAN_DATE,PER_DAY_QUANTITY,ITEM_CODE,
                                DIVISION_CODE,REMARKS,
                                COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,DELETED_FLAG,FREQUENCY_JSON)
                                SELECT '{copiedPlanCode}', PLAN_DATE,PER_DAY_QUANTITY,ITEM_CODE,
                               '{divisions}',REMARKS,
                                COMPANY_CODE,'{branchs}',CREATED_BY,CREATED_DATE,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,DELETED_FLAG,FREQUENCY_JSON FROM PL_PRO_PLAN_DTL WHERE PLAN_CODE='{planCode}'";

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


        public string CreateTemProductionPlanReportTable(ReportFiltersModel model)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var response = "";
            var createTableQry = $@"CREATE TABLE TEMP_PL_PRO_PLAN_REPORT AS SELECT DISTINCT 
                                        TO_CHAR(fn_bs_month(substr(bs_date(PLD.PLAN_DATE), 6, 2))) MONTH,
                                        SUBSTR(BS_DATE(PLD.PLAN_DATE), 6, 2) MONTHINT,
                                        TO_CHAR(substr(bs_date(PLD.PLAN_DATE), 0, 4)) as YEAR,   PLD.ITEM_CODE, IMS.ITEM_EDESC,  
                                   PLD.DIVISION_CODE,  DS.DIVISION_EDESC, FS.BRANCH_CODE, PLD.COMPANY_CODE,
                                   FS.BRANCH_EDESC ,
                                   SUM(PLD.PER_DAY_QUANTITY) PER_DAY_QUANTITY
                              FROM  PL_PRO_PLAN_DTL PLD,
                                   IP_ITEM_MASTER_SETUP IMS,
                                  PL_PRO_PLAN PL,
                                   FA_DIVISION_SETUP DS,
                                   FA_BRANCH_SETUP FS
                                   WHERE  PL.DELETED_FLAG = 'N' 
                                                         AND PL.PLAN_CODE = PLD.PLAN_CODE
                                                         AND DS.DIVISION_CODE (+)= PLD.DIVISION_CODE
                                                         AND FS.BRANCH_CODE (+)= PLD.BRANCH_CODE
                                                         AND IMS.ITEM_CODE = PLD.ITEM_CODE
                                                         AND PLD.COMPANY_CODE = IMS.COMPANY_CODE
                                                         AND DS.COMPANY_CODE (+)=PLD.COMPANY_CODE
                                                         AND FS.COMPANY_CODE (+)=PLD.COMPANY_CODE
                                    AND IMS.GROUP_SKU_FLAG = 'I' 
                                    GROUP BY  TO_CHAR(fn_bs_month(substr(bs_date(PLD.PLAN_DATE), 6, 2))),
                                    SUBSTR(BS_DATE(PLD.PLAN_DATE), 6, 2),
                                    TO_CHAR(substr(bs_date(PLD.PLAN_DATE), 0, 4)) ,
                                    PLD.ITEM_CODE, IMS.ITEM_EDESC,    
                                   PLD.DIVISION_CODE,DS.DIVISION_EDESC,FS.BRANCH_CODE,
                                   FS.BRANCH_EDESC , PLD.COMPANY_CODE";
            try
            {
                var tableExistsQry = $@"SELECT * FROM TEMP_PL_PRO_PLAN_REPORT";
                var result = this._dbContext.SqlQuery<ProductionPlan>(tableExistsQry).ToList();
                if (result != null)
                {
                    var dropQry = $@"DROP TABLE TEMP_PL_PRO_PLAN_REPORT";
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
                    var dropQry = $@"DROP TABLE TEMP_PL_PRO_PLAN_REPORT";
                    var dropResponse = this._dbContext.ExecuteSqlCommand(dropQry);
                    response = ex.Message;
                }
            }
            return response;
        }
        #endregion
        #region production plan new  subin 
        public List<PorductionDetailsModel> GetPorductionDetails(string startDate, string endDate, string pCode)
        {
            var query = $@"SELECT order_no,item_code,customer_code,customer_edesc,item_edesc,SUM(due_qty) qty FROM v$sales_order_analysis 
WHERE company_code = '{_workcontext.CurrentUserinformation.company_code}' AND deleted_flag = 'N' AND 
customer_code =nvl('{pCode}', customer_code) AND 
order_date > nvl(TRUNC(TO_DATE('{startDate}','YYYY-MON-DD')), order_date - 1) AND order_date <= nvl(TRUNC(TO_DATE('{endDate}','YYYY-MON-DD')), order_date) 
GROUP by order_no,item_code,customer_code,customer_edesc,item_edesc HAVING SUM(due_qty) > 0 ORDER by order_no";
           var result = this._dbContext.SqlQuery<PorductionDetailsModel>(query).ToList();
            return result;

        }
        #endregion
    }
}
