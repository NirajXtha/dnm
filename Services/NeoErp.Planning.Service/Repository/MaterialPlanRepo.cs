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
using NeoErp.Core.Helpers;
using System.Web;
using System.Xml.Linq;

namespace NeoErp.Planning.Service.Repository
{
    public class MaterialPlanRepo : IMaterialPlanRepo
    {
        private NeoErpCoreEntity _dbContext;
        private IWorkContext _workcontext;
        public MaterialPlanRepo(NeoErpCoreEntity dbContext, IWorkContext workContext)
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


        #region Material Plan
        public List<MaterialPlanModel> getPlanDetailById(string planCode)
        {
            var company_code = _workcontext.CurrentUserinformation.company_code;
            var result = new List<MaterialPlanModel>();
            try
            {
                var qry = $@"SELECT DISTINCT MP.PLAN_CODE,MP.PLAN_EDESC, MPD.FINISHED_ITEM_CODE,MPD.MATERIAL_QUANTITY,MP.PLAN_DATE,
                                MP.REFERENCE_FLAG,MP.REFERENCE_CODE FROM PL_MATERIAL_PLAN MP
                                    INNER JOIN PL_MATERIAL_PLAN_DTL MPD ON MP.PLAN_CODE=MPD.PLAN_CODE
                                    WHERE MP.DELETED_FLAG='N'
                                    AND MP.COMPANY_CODE ='{company_code}'
                                    AND MP.PLAN_CODE='{planCode}'";
                result = _dbContext.SqlQuery<MaterialPlanModel>(qry).ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return result;
        }
        public string SaveMaterialPlans(SaveMaterialPlanModel model)
        {
            int planCode = 0;
            var userID = _workcontext.CurrentUserinformation.User_id;
            var branchCode = _workcontext.CurrentUserinformation.branch_code;
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            try
            {
                var result = "";
                if (!string.IsNullOrEmpty(model.PLAN_CODE))
                {
                    string updateQuery = $@"UPDATE PL_MATERIAL_PLAN SET PLAN_EDESC = '{model.PLAN_EDESC}',PLAN_NDESC = '{model.PLAN_EDESC}', PLAN_DATE=TO_DATE('{model.PLAN_DATE}','MM/DD/YYYY')
                        WHERE PLAN_CODE='{model.PLAN_CODE}'";
                    var update_result = this._dbContext.ExecuteSqlCommand(updateQuery);

                    var delQry = $@"DELETE FROM PL_MATERIAL_PLAN_DTL WHERE PLAN_CODE='{model.PLAN_CODE}' AND COMPANY_CODE='{companyCode}'";
                    _dbContext.ExecuteSqlCommand(delQry);
                    planCode = Convert.ToInt32(model.PLAN_CODE);
                }
                else
                {
                    string insert_plan_query = $@"INSERT INTO PL_MATERIAL_PLAN(PLAN_CODE,PLAN_EDESC,PLAN_NDESC,MATERIAL_QUANTITY,PLAN_DATE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,
                    MODIFIED_BY,MODIFIED_DATE,DELETED_FLAG)
                    VALUES((SELECT COALESCE(MAX(PLAN_CODE)+1, MAX(PLAN_CODE) + 1, 1) FROM PL_MATERIAL_PLAN),'{model.PLAN_EDESC}','{model.PLAN_EDESC}','0',TO_DATE('{model.PLAN_DATE}','MM/DD/YYYY'),'{companyCode}','{branchCode}','{userID}',
                    TO_DATE('{DateTime.Today.ToString("MM/dd/yyyy")}','MM/DD/YYYY'),'{userID}',TO_DATE('{DateTime.Today.ToString("MM/dd/yyyy")}','MM/DD/YYYY'),'N')";

                    var insertResult = this._dbContext.ExecuteSqlCommand(insert_plan_query);

                    string fetchPlanCode = $@"SELECT MAX(PLAN_CODE) FROM PL_MATERIAL_PLAN WHERE PLAN_EDESC='{model.PLAN_EDESC}' AND COMPANY_CODE='{companyCode}' AND DELETED_FLAG='N'";
                    planCode = this._dbContext.SqlQuery<int>(fetchPlanCode).First();
                }

                var sbInsertQuery = string.Empty;
                string[] insert_query_array = new string[model.finishedItemList.Count() * model.rawItemList.Count()];
                int array_count = 0;
                var monthDiff = Convert.ToInt32(model.MONTH_DIFF);
                if (monthDiff <= 0)
                    monthDiff = 1;
                for (var i = 0; i < monthDiff; i++)
                {
                    foreach (var childItem in model.rawItemList)
                    {
                        var dt = DateTime.ParseExact(model.START_DATE, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).AddMonths(i);
                        var planDate = dt.ToString("MM-dd-yyyy");

                        decimal calcQty = Convert.ToDecimal(childItem.CALC_QTY), remainingQty = Convert.ToDecimal(childItem.REMAINING_QTY);

                        var materialQty = model.finishedItemList.Where(x => x.FG_ITEM_CODE == childItem.FINISHED_ITEM_CODE).Select(x => x.QTY).FirstOrDefault();
                        remainingQty = calcQty - (Convert.ToDecimal(childItem.STOCK) + Convert.ToDecimal(childItem.PO_PENDING));
                        if (remainingQty < 0)
                            remainingQty = 0;
                        string insertinto_plandtl = $@"SELECT {planCode},TO_DATE('{planDate}','MM/DD/YYYY'),'{childItem.ITEM_CODE}','{ childItem.FINISHED_ITEM_CODE}','{materialQty}','0','{calcQty}','{childItem.STOCK}',
                                '{childItem.PO_PENDING}','{remainingQty}','{companyCode}','{branchCode}','{userID}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/DD/YYYY'),'','','N' FROM DUAL UNION ALL ";

                        sbInsertQuery += insertinto_plandtl;
                    }
                }

                string query = @"INSERT INTO PL_MATERIAL_PLAN_DTL(PLAN_CODE,PLAN_DATE,ITEM_CODE,FINISHED_ITEM_CODE,MATERIAL_QUANTITY,REQUIRED_QUANTITY,CALC_QUANTITY,
                    STOCK,PO_PENDING,REMAINING_QUANTITY,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,DELETED_FLAG) "
        + sbInsertQuery;
                query = query.Substring(0, query.Length - 11);
                insert_query_array[array_count] = query;
                try
                {
                    var insertItems = insert_query_array.Where(a => a != null);
                    foreach (var item in insertItems)
                    {
                        result = _dbContext.ExecuteSqlCommand(item.ToString()).ToString();
                    }
                }
                catch (Exception ex)
                {
                    if (!ex.Message.Contains("unique constraint"))
                        throw;
                }
                var delFromMaterialPlanQry = $@"DELETE FROM PL_MATERIAL_PLAN WHERE PLAN_CODE NOT IN (SELECT PLAN_CODE FROM PL_MATERIAL_PLAN_DTL)";
                _dbContext.ExecuteSqlCommand(delFromMaterialPlanQry);
                return result.ToString();
            }
            catch (Exception ex)
            {
                _dbContext.ExecuteSqlCommand($@"DELETE FROM PL_MATERIAL_PLAN WHERE PLAN_CODE='{planCode}'");
                throw ex;
            }
        }
        public string SaveMaterialPlanReference(SaveMaterialPlanReferenceModel model)
        {
            int planCode = 0;
            var userID = _workcontext.CurrentUserinformation.User_id;
            var branchCode = _workcontext.CurrentUserinformation.branch_code;
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            try
            {
                var result = "";
                var planList = new List<ProcureFromMaterialModel>();
                if (model.REFERENCE_FLAG == "SALES")
                    planList = GetAllRawMaterialBySalesPlanCode(model.REFERENCE_CODE);
                else if (model.REFERENCE_FLAG == "PROD")
                    planList = GetAllRawMaterialByProductionPlanCode(model.REFERENCE_CODE);
                else if (model.REFERENCE_FLAG == "ORDER")
                    planList = GetAllRawMaterialBySalesOrderCustomerCode(model.REFERENCE_CODE, model.START_DATE, model.END_DATE);

                if (!string.IsNullOrEmpty(model.PLAN_CODE))
                {
                    string updateQuery = $@"UPDATE PL_MATERIAL_PLAN SET PLAN_EDESC = '{model.PLAN_EDESC}',PLAN_NDESC = '{model.PLAN_EDESC}', PLAN_DATE=TO_DATE('{model.PLAN_DATE}','MM/DD/YYYY'), REFERENCE_CODE='{model.REFERENCE_CODE}',REFERENCE_FLAG='{model.REFERENCE_FLAG}'
                        WHERE PLAN_CODE='{model.PLAN_CODE}'";
                    var update_result = this._dbContext.ExecuteSqlCommand(updateQuery);

                    var delQry = $@"DELETE FROM PL_MATERIAL_PLAN_DTL WHERE PLAN_CODE='{model.PLAN_CODE}' AND COMPANY_CODE='{companyCode}'";
                    _dbContext.ExecuteSqlCommand(delQry);
                    planCode = Convert.ToInt32(model.PLAN_CODE);
                }
                else
                {
                    string insert_plan_query = $@"INSERT INTO PL_MATERIAL_PLAN(PLAN_CODE,PLAN_EDESC,PLAN_NDESC,MATERIAL_QUANTITY,PLAN_DATE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,
                    MODIFIED_BY,MODIFIED_DATE,DELETED_FLAG,REFERENCE_FLAG,REFERENCE_CODE)
                    VALUES((SELECT COALESCE(MAX(PLAN_CODE)+1, MAX(PLAN_CODE) + 1, 1) FROM PL_MATERIAL_PLAN),'{model.PLAN_EDESC}','{model.PLAN_EDESC}','0',TO_DATE('{model.PLAN_DATE}','MM/DD/YYYY'),'{companyCode}','{branchCode}','{userID}',
                    TO_DATE('{DateTime.Today.ToString("MM/dd/yyyy")}','MM/DD/YYYY'),'{userID}',TO_DATE('{DateTime.Today.ToString("MM/dd/yyyy")}','MM/DD/YYYY'),'N','{model.REFERENCE_FLAG}','{model.REFERENCE_CODE}')";

                    var insertResult = this._dbContext.ExecuteSqlCommand(insert_plan_query);

                    string fetchPlanCode = $@"SELECT MAX(PLAN_CODE) FROM PL_MATERIAL_PLAN WHERE PLAN_EDESC='{model.PLAN_EDESC}' AND COMPANY_CODE='{companyCode}' AND DELETED_FLAG='N'";
                    planCode = this._dbContext.SqlQuery<int>(fetchPlanCode).First();
                }

                var sbInsertQuery = string.Empty;
                string[] insert_query_array = new string[model.rawItemList.Count()];
                int array_count = 0;
                var list = model.rawItemList;
                var arrList = new List<MaterialPlanModel>();


                foreach (var childItem in planList)
                {
                    var dt = DateTime.Now;
                    var planDate = childItem.MONTHINT + "/" + DateTime.Now.Day + "/" + DateTime.Now.Year;
                    decimal calcQty = Convert.ToDecimal(childItem.REQUIRED_QUANTITY), remainingQty = 0;//Convert.ToDecimal(childItem.REMAINING_QTY);

                    if (remainingQty < 0)
                        remainingQty = 0;
                    string insertinto_plandtl = $@"SELECT '{planCode}',TO_DATE('{planDate}','MM/DD/YYYY'),'{childItem.ITEM_CODE}','{ childItem.FINISHED_ITEM_CODE}','0','{childItem.REQUIRED_QTY}','{calcQty}','{childItem.STOCK}',
                                '{childItem.PO_PENDING}','{remainingQty}','{companyCode}','{branchCode}','{userID}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/DD/YYYY'),'','','N' FROM DUAL UNION ALL ";

                    sbInsertQuery += insertinto_plandtl;
                }

                string query = @"INSERT INTO PL_MATERIAL_PLAN_DTL(PLAN_CODE,PLAN_DATE,ITEM_CODE,FINISHED_ITEM_CODE,MATERIAL_QUANTITY,REQUIRED_QUANTITY,CALC_QUANTITY,
                    STOCK,PO_PENDING,REMAINING_QUANTITY,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,DELETED_FLAG) "
        + sbInsertQuery;
                query = query.Substring(0, query.Length - 11);
                insert_query_array[array_count] = query;
                try
                {
                    var insertItems = insert_query_array.Where(a => a != null);
                    foreach (var item in insertItems)
                    {
                        result = _dbContext.ExecuteSqlCommand(item.ToString()).ToString();
                    }
                }
                catch (Exception ex)
                {
                    if (!ex.Message.Contains("unique constraint"))
                        throw;
                }
                var delFromMaterialPlanQry = $@"DELETE FROM PL_MATERIAL_PLAN WHERE PLAN_CODE NOT IN (SELECT PLAN_CODE FROM PL_MATERIAL_PLAN_DTL)";
                _dbContext.ExecuteSqlCommand(delFromMaterialPlanQry);
                return result.ToString();
            }
            catch (Exception ex)
            {
                _dbContext.ExecuteSqlCommand($@"DELETE FROM PL_MATERIAL_PLAN WHERE PLAN_CODE='{planCode}'");
                throw ex;
            }
        }


        public string SaveMaterialPlanReference(List<savePlan> sv, SalesPlan sp)
        {
            try
            {
                var userID = _workcontext.CurrentUserinformation.User_id;
                var branchCode = _workcontext.CurrentUserinformation.branch_code;
                var spbranchCode = sp.branchCode == "" ? _workcontext.CurrentUserinformation.branch_code : sp.branchCode;
                var companyCode = _workcontext.CurrentUserinformation.company_code;
                var result = "";
                int planCode = 0;
                string time_frame_code = sp.TIME_FRAME_CODE;

                var company_code = _workcontext.CurrentUserinformation.company_code;

                string checkPlanQuery = $@"SELECT COUNT(*) FROM PL_SALES_PLAN WHERE PLAN_EDESC='{sp.PLAN_EDESC}' 
                AND TIME_FRAME_CODE='{sp.TIME_FRAME_CODE}' AND START_DATE=TO_DATE('{sp.START_DATE}','YYYY-Mon-DD') AND END_DATE=TO_DATE('{sp.END_DATE}','YYYY-Mon-DD')";
                var checkPlanQueryResult = this._dbContext.SqlQuery<int>(checkPlanQuery).First();
                if (checkPlanQueryResult == 0 && string.IsNullOrEmpty(sp.PLAN_CODE))
                {
                    if (sp.PLAN_FOR.ToLower() == "quantity")
                    {
                        sp.SALES_AMOUNT = string.Empty;
                    }
                    else if (sp.PLAN_FOR.ToLower() == "amount")
                    {
                        sp.SALES_QUANTITY = string.Empty;
                    }
                    string sales_plan_query = $@"INSERT INTO PL_SALES_PLAN(PLAN_CODE,PLAN_EDESC,PLAN_NDESC,SALES_QUANTITY,SALES_AMOUNT,TIME_FRAME_CODE,
                    CALENDAR_TYPE,START_DATE,END_DATE,REMARKS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,
                    LAST_MODIFIED_BY,LAST_MODIFIED_DATE,
                    DELETED_FLAG,SALES_PRICE_TYPE)
                    VALUES((SELECT COALESCE(MAX(PLAN_CODE)+1, MAX(PLAN_CODE) + 1, 1) FROM PL_SALES_PLAN),'{sp.PLAN_EDESC}','','{sp.SALES_QUANTITY}','{sp.SALES_AMOUNT}','{sp.TIME_FRAME_CODE}','{sp.CALENDAR_TYPE}',
                    TO_DATE('{sp.START_DATE}','YYYY-Mon-DD'),TO_DATE('{sp.END_DATE}','YYYY-Mon-DD'),'{sp.REMARKS}','{companyCode}','{branchCode}','{userID}',
                    TO_DATE('{DateTime.Today.ToString("MM/dd/yyyy")}','MM/DD/YYYY'),'{userID}',TO_DATE('{DateTime.Today.ToString("MM/dd/yyyy")}','MM/DD/YYYY'),'N','{sp.salesRateType}')";

                    var insertResult = this._dbContext.ExecuteSqlCommand(sales_plan_query);

                    string fetchSalesPlan = $@"SELECT PLAN_CODE FROM PL_SALES_PLAN WHERE PLAN_EDESC='{sp.PLAN_EDESC}' 
                AND TIME_FRAME_CODE='{sp.TIME_FRAME_CODE}' AND START_DATE=TO_DATE('{sp.START_DATE}','YYYY-Mon-DD') AND END_DATE=TO_DATE('{sp.END_DATE}','YYYY-Mon-DD')";
                    planCode = this._dbContext.SqlQuery<int>(fetchSalesPlan).First();

                }
                else
                {
                    if (!string.IsNullOrEmpty(sp.PLAN_CODE) && sp.PLAN_CODE != "0")
                    {
                        planCode = Convert.ToInt32(sp.PLAN_CODE);
                        string updateQuery = $@"UPDATE PL_SALES_PLAN SET PLAN_EDESC = '{sp.PLAN_EDESC}' , REMARKS = '{sp.REMARKS}' , SALES_PRICE_TYPE ='{sp.salesRateType}' 
                        ,TIME_FRAME_CODE='{sp.TIME_FRAME_CODE}' , CALENDAR_TYPE='{sp.CALENDAR_TYPE}', START_DATE=TO_DATE('{sp.START_DATE}','YYYY-MON-DD') , END_DATE=TO_DATE('{sp.END_DATE}','YYYY-MON-DD')
                        WHERE PLAN_CODE='{sp.PLAN_CODE}'";
                        var update_result = this._dbContext.ExecuteSqlCommand(updateQuery);
                    }
                    else
                    {
                        string plancode_query = $@"SELECT PLAN_CODE FROM PL_SALES_PLAN WHERE PLAN_EDESC='{sp.PLAN_EDESC}' 
                AND TIME_FRAME_CODE='{sp.TIME_FRAME_CODE}' AND START_DATE=TO_DATE('{sp.START_DATE}','YYYY-Mon-DD') AND END_DATE=TO_DATE('{sp.END_DATE}','YYYY-Mon-DD')";
                        planCode = this._dbContext.SqlQuery<int>(plancode_query).First();
                    }

                    string delete_detail_already_set = $@"DELETE FROM PL_SALES_PLAN_DTL WHERE PLAN_CODE='{planCode}'";
                    this._dbContext.ExecuteSqlCommand(delete_detail_already_set);
                }

                string sbInsertQuery = string.Empty;
                List<sa_sales_invoice_viewmodel> salesInvoice = new List<sa_sales_invoice_viewmodel>();
                string averageQuery = $@"SELECT CALC_TOTAL_PRICE,CALC_QUANTITY, ITEM_CODE FROM SA_SALES_INVOICE WHERE DELETED_FLAG = 'N' AND COMPANY_CODE = '{company_code}' AND ITEM_CODE IN ({string.Join(",", sv.Select(a => a.itemCode))})";
                salesInvoice = this._dbContext.SqlQuery<sa_sales_invoice_viewmodel>(averageQuery).ToList();

                string itemQuery = $@"SELECT PURCHASE_PRICE AS CALC_TOTAL_PRICE,ITEM_CODE FROM IP_ITEM_MASTER_SETUP WHERE DELETED_FLAG = 'N' AND COMPANY_CODE = '{company_code}'  AND ITEM_CODE IN ({string.Join(",", sv.Select(a => a.itemCode))})";
                var itemsFromItemMaster = this._dbContext.SqlQuery<sa_sales_invoice_viewmodel>(itemQuery).ToList();
                //
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

                    if (string.IsNullOrEmpty(salesPriceResult) || salesPriceResult == "0")
                    {
                        salesPriceResult = "1";
                    }
                    // end
                    var totalMonthQty = 0M;
                    //foreach (var freq in item.frequency)
                    //{
                    //    if (freq.fvalue != "")
                    //    {
                    //        totalMonthQty = totalMonthQty + Convert.ToDecimal(freq.fvalue);
                    //    }
                    //}
                    //foreach (var freq in item.frequency)
                    //{
                    //    try
                    //    {
                    //        string[] freq_date = null;
                    //        int interval = 0;

                    //        string freValue = freq.fname.Split('_')[0]; // month or week
                    //        string freYear = freq.fname.Split('_')[1]; // year

                    //        string freYearVal = freYear + '-' + freValue;

                    //        List<FrequencyColumnModel> allYearByNYM = new List<FrequencyColumnModel>();
                    //        allYearByNYM = getAllDaysByNepaliYearMonth(sp.dateFormat, sp.TIME_FRAME_CODE, sp.START_DATE, sp.END_DATE);

                    //        if (sp.TIME_FRAME_EDESC.ToLower() == "week" && (sp.dateFormat.ToLower() == "ad" || sp.dateFormat.ToLower() == "eng"))
                    //        {
                    //            freYear = freq.fname.Split('_')[0];
                    //            freValue = freq.fname.Split('_')[1];
                    //        }
                    //        if (sp.dateFormat.ToLower() == "loc" || sp.dateFormat.ToLower() == "bs")
                    //        {
                    //            if (sp.TIME_FRAME_EDESC.ToLower() == "week")
                    //            {
                    //                string yearmonth = allYearWeekObj.Where(a => a.NWEEK == freYear).First().YEARWEEK;
                    //                freValue = yearmonth.Split('-')[1];
                    //                freYear = yearmonth.Split('-')[0];
                    //            }
                    //            else
                    //            {
                    //                string bsyearmonth = allYearWeekObj.Where(a => a.BS_MONTH == freYear + "-" + freValue).First().YEARWEEK;
                    //                freValue = bsyearmonth.Split('-')[1];
                    //                freYear = bsyearmonth.Split('-')[0];
                    //            }
                    //        }

                    //        if (sp.TIME_FRAME_EDESC.Trim().ToLower() == "week")
                    //        {
                    //            freq_date = allDaysObj.Where(a => Convert.ToInt32(a.WEEKS) == Convert.ToInt32(freValue) && a.YEAR == freYear).Select(a => a.DAYS).ToArray();
                    //        }
                    //        else if (sp.TIME_FRAME_EDESC.Trim().ToLower() == "month")
                    //        {
                    //            freq_date = allDaysObj.Where(a => Convert.ToInt32(a.MONTHINT) == Convert.ToInt32(freValue) && a.YEAR == freYear).Select(a => a.DAYS).ToArray();

                    //            if (sp.dateFormat.ToLower() == "loc" || sp.dateFormat.ToLower() == "bs")
                    //            {
                    //                freq_date = allYearByNYM.Where(a => a.YEARWEEK == freYearVal).Select(a => a.DAYS).ToArray();
                    //            }
                    //        }
                    //        interval = freq_date.Length;
                    //        // each day insertion loop.
                    //        foreach (var date in freq_date)
                    //        {
                    //            var eachday_value = Math.Round((Convert.ToDecimal(freq.fvalue) / interval), 5);

                    //            var eachday_quantity_value = 0M;
                    //            var eachday_amount_value = 0M;


                    //            if (sp.PLAN_FOR.ToLower() == "quantity")
                    //            {
                    //                eachday_quantity_value = eachday_value;
                    //                var eachday_amount = 0M;
                    //                var rate = Math.Round(((Convert.ToDecimal(freq.fvalue_amt) / totalMonthQty)), 5);
                    //                if (freq.fvalue_amt != null)
                    //                    eachday_amount = Math.Round(((rate * (Convert.ToDecimal(freq.fvalue) / interval))), 5);
                    //                else
                    //                    eachday_amount = eachday_value;
                    //                eachday_amount_value = eachday_amount;//Math.Round(eachday_value * Convert.ToDecimal(salesPriceResult), 5);
                    //            }
                    //            else if (sp.PLAN_FOR.ToLower() == "amount")
                    //            {
                    //                eachday_quantity_value = Math.Round(eachday_value / Convert.ToDecimal(salesPriceResult), 5);
                    //                eachday_amount_value = eachday_value;
                    //            }

                    //            string frequency_json = @"fname__" + freq.fname + "__fvalue__" + freq.fvalue;

                    //            string insertinto_plandtl = $@"SELECT '{planCode}',TO_DATE('{date}','DD-MON-YYYY'),'{eachday_quantity_value}','{eachday_amount_value}','{item.itemCode}',
                    //            '{sp.customerCode}','{sp.employeeCode}','{sp.divisionCode}','{sp.partytypeCode}','',
                    //            '{companyCode}','{spbranchCode}','{userID}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/DD/YYYY'),'','','N','{frequency_json}','{salesPriceResult}' FROM DUAL UNION ALL ";

                    //            sbInsertQuery += insertinto_plandtl;
                    //        }
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        if (!ex.Message.Contains("unique constraint"))
                    //            throw;
                    //        else
                    //        {
                    //            var sqlquery = $@"UPDATE PL_PLAN_DTL SET TARGET_VALUE='{freq.fvalue}',LAST_MODIFIED_BY='{userID}', LAST_MODIFIED_DATE=TO_DATE('{DateTime.Now:MM/dd/yyyy/ HH:mm:ss}', 'mm/dd/yyyy hh24:mi:ss') where PLAN_CODE='{planCode}' AND ITEM_CODE='{item.itemCode}' AND TIME_FRAME_CODE='{time_frame_code}' AND TIME_FRAME_VALUE='{freq.fname}'";
                    //        }
                    //    }
                    //}
                    if (itteration <= (sv.Count() - remainderCount))
                    {
                        while (itemCount == itemRange)
                        {
                            string query = @"insert into PL_SALES_PLAN_DTL(PLAN_CODE,PLAN_DATE,PER_DAY_QUANTITY,PER_DAY_AMOUNT,ITEM_CODE,
                    CUSTOMER_CODE,EMPLOYEE_CODE,DIVISION_CODE,PARTY_TYPE_CODE,REMARKS,
                    COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,DELETED_FLAG,FREQUENCY_JSON,SALES_PRICE_APPLIED) "
+ sbInsertQuery;
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
                            string query = @"insert into PL_SALES_PLAN_DTL(PLAN_CODE,PLAN_DATE,PER_DAY_QUANTITY,PER_DAY_AMOUNT,ITEM_CODE,
                    CUSTOMER_CODE,EMPLOYEE_CODE,DIVISION_CODE,PARTY_TYPE_CODE,REMARKS,
                    COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,DELETED_FLAG,FREQUENCY_JSON,SALES_PRICE_APPLIED) "
+ sbInsertQuery;
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
                        result = _dbContext.ExecuteSqlCommand(item.ToString()).ToString();
                    }
                }
                catch (Exception ex)
                {
                    if (!ex.Message.Contains("unique constraint"))
                        throw;
                }
                return result.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string SaveMaterialPlan(List<MaterialPlanModel> sv, MaterialPlan sp)
        {
            int planCode = 0;
            try
            {
                var userID = _workcontext.CurrentUserinformation.User_id;
                var branchCode = _workcontext.CurrentUserinformation.branch_code;
                var companyCode = _workcontext.CurrentUserinformation.company_code;
                var result = "";

                string insert_plan_query = $@"INSERT INTO PL_MATERIAL_PLAN(PLAN_CODE,PLAN_EDESC,PLAN_NDESC,MATERIAL_QUANTITY,PLAN_DATE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,
                    MODIFIED_BY,MODIFIED_DATE,DELETED_FLAG)
                    VALUES((SELECT COALESCE(MAX(PLAN_CODE)+1, MAX(PLAN_CODE) + 1, 1) FROM PL_MATERIAL_PLAN),'{sp.PLAN_EDESC}','{sp.PLAN_EDESC}','{sp.PLAN_QTY}',SYSDATE,'{companyCode}','{branchCode}','{userID}',
                    TO_DATE('{DateTime.Today.ToString("MM/dd/yyyy")}','MM/DD/YYYY'),'{userID}',TO_DATE('{DateTime.Today.ToString("MM/dd/yyyy")}','MM/DD/YYYY'),'N')";

                var insertResult = this._dbContext.ExecuteSqlCommand(insert_plan_query);

                string fetchPlanCode = $@"SELECT MAX(PLAN_CODE) FROM PL_MATERIAL_PLAN WHERE PLAN_EDESC='{sp.PLAN_EDESC}'";
                planCode = this._dbContext.SqlQuery<int>(fetchPlanCode).First();


                var sbInsertQuery = string.Empty;
                string[] insert_query_array = new string[sv.Count()];
                int array_count = 0;

                var itemList = string.Join("','", sv.Select(x => x.ITEM_CODE).ToArray());
                //var deleteExistQry =$@"DELETE FROM PL_MATERIAL_PLAN_DTL WHERE ITEM_CODE IN ('{itemList}')";
                //var deletedResult = _dbContext.ExecuteSqlCommand(deleteExistQry);
                foreach (var item in sv)
                {
                    string insertinto_plandtl = $@"SELECT '{planCode}',SYSDATE,'{item.ITEM_CODE}','{item.REQUIRED_QTY}','{item.CALC_QTY}','{item.STOCK}',
                                '{item.PO_PENDING}','{item.REMAINING_QTY}','{companyCode}','{branchCode}','{userID}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/DD/YYYY'),'','','N' FROM DUAL UNION ALL ";

                    sbInsertQuery += insertinto_plandtl;

                }

                string query = @"INSERT INTO PL_MATERIAL_PLAN_DTL(PLAN_CODE,PLAN_DATE,ITEM_CODE,REQUIRED_QUANTITY,CALC_QUANTITY,
                    STOCK,PO_PENDING,REMAINING_QUANTITY,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,DELETED_FLAG) "
+ sbInsertQuery;
                query = query.Substring(0, query.Length - 11);
                insert_query_array[array_count] = query;
                try
                {
                    var insertItems = insert_query_array.Where(a => a != null);
                    foreach (var item in insertItems)
                    {
                        result = _dbContext.ExecuteSqlCommand(item.ToString()).ToString();

                    }
                }
                catch (Exception ex)
                {
                    if (!ex.Message.Contains("unique constraint"))
                        throw;
                }
                return result.ToString();
            }
            catch (Exception ex)
            {
                _dbContext.ExecuteSqlCommand($@"DELETE FROM PL_MATERIAL_PLAN WHERE PLAN_CODE='{planCode}'");
                throw ex;
            }
        }

        public List<MaterialPlan> getAllMaterialPlans(ReportFiltersModel filters)
        {
            var company_code = _workcontext.CurrentUserinformation.company_code;
            var userId = _workcontext.CurrentUserinformation.User_id;
            string query = $@"  SELECT DISTINCT TO_CHAR(PL.PLAN_CODE) PLAN_CODE, PL.PLAN_EDESC,REFERENCE_FLAG, PL.PLAN_NDESC,TO_CHAR(PL.PLAN_DATE,'DD-Mon-YYYY') PLAN_DATE,
                                    TO_DATE(PL.CREATED_DATE,'DD-Mon-YYYY')CREATED_DATE,PL.CREATED_BY,
                                    TO_CHAR(PL.MODIFIED_DATE,'DD-Mon-YYYY')MODIFY_DATE,PL.MODIFIED_BY
                             FROM PL_MATERIAL_PLAN PL
                             WHERE PL.DELETED_FLAG = 'N' AND PL.COMPANY_CODE='{company_code}'";

            if (filters.BranchFilter.Count > 0)
            {
                query += $@" AND PLD.BRANCH_CODE IN ('{string.Join("','", filters.BranchFilter)}')";

            }
            if (filters.DivisionFilter.Count > 0)
            {
                query += $@" AND PLD.DIVISION_CODE IN ('{string.Join("','", filters.DivisionFilter)}')";

            }
            query += $@" ORDER BY TO_NUMBER (PL.PLAN_CODE)  DESC";

            List<MaterialPlan> spList = new List<MaterialPlan>();

            spList = this._dbContext.SqlQuery<MaterialPlan>(query).ToList();
            return spList;
        }
        public List<MaterialPlan> getGroupMaterialPlans(ReportFiltersModel filters)
        {
            var company_code = _workcontext.CurrentUserinformation.company_code;
            var userId = _workcontext.CurrentUserinformation.User_id;
            string query = $@"SELECT DISTINCT MPD.FINISHED_ITEM_CODE, IMS.ITEM_EDESC FINISHED_ITEM_EDESC,--MPD.ITEM_CODE,CIMS.ITEM_EDESC,
                             TO_CHAR(MPD.PLAN_DATE,'DD-Mon-YYYY')PLAN_DATE
                              , MPD.MATERIAL_QUANTITY--,MPD.CALC_QUANTITY,MPD.REMAINING_QUANTITY 
                            FROM PL_MATERIAL_PLAN_DTL MPD
                             INNER JOIN IP_ITEM_MASTER_SETUP IMS ON MPD.FINISHED_ITEM_CODE = IMS.ITEM_CODE AND MPD.COMPANY_CODE= IMS.COMPANY_CODE
                             --INNER JOIN IP_ITEM_MASTER_SETUP CIMS ON MPD.ITEM_CODE = CIMS.ITEM_CODE AND MPD.COMPANY_CODE= CIMS.COMPANY_CODE
                             WHERE MPD.DELETED_FLAG='N' AND MPD.COMPANY_CODE='{company_code}'";

            if (filters.BranchFilter.Count > 0)
            {
                query += $@" AND MPD.BRANCH_CODE IN ('{string.Join("','", filters.BranchFilter)}')";

            }

            List<MaterialPlan> spList = new List<MaterialPlan>();

            spList = this._dbContext.SqlQuery<MaterialPlan>(query).ToList();
            return spList;
        }

        public List<MaterialPlan> getAllMaterialPlans(string customercode, string employeecode, string divisioncode, string branchcode, string startdate, string enddate)
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
                            FROM PL_BRD_PRCMT_PLAN PL, PL_BRD_PRCMT_PLAN_DTL PLD, SA_CUSTOMER_SETUP CS , FA_DIVISION_SETUP DS, HR_EMPLOYEE_SETUP ES, FA_BRANCH_SETUP FS
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

            List<MaterialPlan> spList = new List<MaterialPlan>();
            spList = this._dbContext.SqlQuery<MaterialPlan>(query).ToList();

            string masterplan_query = $@"SELECT DISTINCT TO_CHAR(MSP.MASTER_PLAN_CODE)||'_MP' PLAN_CODE,
                                        MSP.MASTER_PLAN_EDESC PLAN_EDESC ,
                                        TO_CHAR (MSP.START_DATE, 'DD-MON-YYYY') START_DATE,
                                             TO_CHAR (MSP.END_DATE, 'DD-MON-YYYY') END_DATE
                                    FROM PL_MASTER_SALES_PLAN MSP ,PL_BRD_PRCMT_PLAN_MAP SPM
                                    WHERE MSP.MASTER_PLAN_CODE=SPM.MASTER_PLAN_CODE ";
            if (!string.IsNullOrEmpty(startdate) && !string.IsNullOrEmpty(enddate))
            {
                masterplan_query += $@" AND MSP.START_DATE = TO_DATE('{startdate}','YYYY-MON-DD')
                             AND MSP.END_DATE = TO_DATE('{enddate}','YYYY-MON-DD') ";
            }
            List<MaterialPlan> mpList = new List<MaterialPlan>();
            mpList = this._dbContext.SqlQuery<MaterialPlan>(masterplan_query).ToList();
            if (mpList.Count > 0)
            {
                foreach (var item in mpList)
                {
                    spList.Add(new MaterialPlan
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

        public bool deleteMaterialPlan(int planCode)
        {
            try
            {
                string deleteYes_salesPlan = $@"UPDATE PL_MATERIAL_PLAN SET DELETED_FLAG='Y' WHERE PLAN_CODE='{planCode}'";
                string deleteYes_salesPlanDtl = $@"UPDATE PL_MATERIAL_PLAN_DTL SET DELETED_FLAG='Y' WHERE PLAN_CODE='{planCode}'";
                this._dbContext.ExecuteSqlCommand(deleteYes_salesPlanDtl);
                this._dbContext.ExecuteSqlCommand(deleteYes_salesPlan);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public PL_MATERIAL_PLAN GetPlanDetailValueByPlanCode(int plancode)
        {
            var company_code = this._workcontext.CurrentUserinformation.company_code;
            PL_MATERIAL_PLAN MaterialPlan = new PL_MATERIAL_PLAN();
            string queryPlan = $@"SELECT 
                PLAN_CODE PLAN_CODE, PLAN_EDESC,PLAN_NDESC,EST_AMOUNT SALES_AMOUNT,TIME_FRAME_CODE TIME_FRAME_CODE,
                (SELECT TIME_FRAME_EDESC FROM PL_TIME_FRAME WHERE TIME_FRAME_CODE = PSP.TIME_FRAME_CODE) TIME_FRAME_EDESC,
                CALENDAR_TYPE,TO_CHAR(START_DATE) START_DATE,TO_CHAR(END_DATE) END_DATE,REMARKS,COMPANY_CODE,BRANCH_CODE
                FROM PL_BRD_PRCMT_PLAN PSP
                WHERE PLAN_CODE= '{plancode}'";

            MaterialPlan = this._dbContext.SqlQuery<PL_MATERIAL_PLAN>(queryPlan).FirstOrDefault();

            List<MaterialPlanDetail> salesPlanDetailList = new List<MaterialPlanDetail>();
            string MaterialPlanDetailQuery = $@"SELECT PLAN_CODE,PLAN_DATE PLAN_DATE,PER_DAY_AMOUNT PER_DAY_AMOUNT,
                ITEM_CODE,DIVISION_CODE,REMARKS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE CREATED_DATE, FREQUENCY_JSON
                FROM PL_BRD_PRCMT_PLAN_DTL WHERE PLAN_CODE = '{plancode}' AND DELETED_FLAG='N'";
            salesPlanDetailList = this._dbContext.SqlQuery<MaterialPlanDetail>(MaterialPlanDetailQuery).ToList();

            List<MaterialPlanItems> salesPlanItem = new List<MaterialPlanItems>();
            string MaterialPlanItemsQuery = $@"SELECT 
            DISTINCT TO_CHAR(PSPD.ITEM_CODE) ITEM_CODE,INITCAP(IMS.ITEM_EDESC) ITEM_EDESC,TO_CHAR(IMS.MASTER_ITEM_CODE) MASTER_ITEM_CODE,TO_CHAR(IMS.PRE_ITEM_CODE) PRE_ITEM_CODE,TO_CHAR(IMS.GROUP_SKU_FLAG) GROUP_SKU_FLAG
            FROM PL_BRD_PRCMT_PLAN_DTL PSPD,IP_ITEM_MASTER_SETUP IMS WHERE PSPD.PLAN_CODE='{plancode}' AND PSPD.DELETED_FLAG='N' AND IMS.DELETED_FLAG='N' AND IMS.ITEM_CODE=PSPD.ITEM_CODE
            ORDER BY PRE_ITEM_CODE,MASTER_ITEM_CODE,ITEM_CODE";
            salesPlanItem = this._dbContext.SqlQuery<MaterialPlanItems>(MaterialPlanItemsQuery).ToList();

            MaterialPlan.selectedItemsList = salesPlanItem;
            MaterialPlan.salesPlanDetail = salesPlanDetailList;

            return MaterialPlan;
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

        public List<PlanSalesRefrenceModel> getProductionItemDataForRefrence(string itemList, string startDate, string endDate, string branchCode, string dateFormat, string frequency)
        {
            var userNo = _workcontext.CurrentUserinformation.User_id;
            var result = new List<PlanSalesRefrenceModel>();
            try
            {
                var refrenceQuery = string.Empty;
                string optionalCondition = string.Empty;
                //if (!string.IsNullOrEmpty(branchCode))
                //{
                //    optionalCondition += $@"  AND SI.BRANCH_CODE in ({branchCode})";
                //}

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
                                  and IMS.COMPANY_CODE=SI.COMPANY_CODE
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
                                     SI.DIVISION_CODE,
                                     SI.BRANCH_CODE,
                                     TO_CHAR(SUM (SI.PER_DAY_QUANTITY)) QTY
                                FROM PL_PRO_PLAN_DTL SI, IP_ITEM_MASTER_SETUP IMS
                                WHERE IMS.ITEM_CODE IN ({itemList}) {optionalCondition} 
                                AND SI.ITEM_CODE = IMS.ITEM_CODE
                                AND IMS.GROUP_SKU_FLAG = 'I'
                                AND SI.PLAN_DATE BETWEEN to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD')
                            GROUP BY SI.ITEM_CODE,
                                     IMS.ITEM_EDESC,
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
                                and IMS.COMPANY_CODE=SI.COMPANY_CODE
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
                        refrenceQuery = $@"SELECT  'freqItemNum_'||SI.ITEM_CODE ||'_' ||  TO_CHAR(SI.PLAN_DATE,'MON') ||'_'||  TO_CHAR(SI.PLAN_DATE,'YYYY') COLNAME ,
                                     IMS.ITEM_EDESC,
                                     SI.DIVISION_CODE,
                                     SI.BRANCH_CODE,
                                     TO_CHAR(SUM (SI.PER_DAY_QUANTITY)) QTY
                                FROM PL_PRO_PLAN_DTL SI, IP_ITEM_MASTER_SETUP IMS
                                WHERE IMS.ITEM_CODE IN ({itemList}) {optionalCondition} 
                                AND SI.ITEM_CODE = IMS.ITEM_CODE
                                AND IMS.COMPANY_CODE=SI.COMPANY_CODE
                                AND IMS.GROUP_SKU_FLAG='I'
                                AND SI.PLAN_DATE BETWEEN to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD')
                            GROUP BY SI.ITEM_CODE,
                                     IMS.ITEM_EDESC,
                                     SI.DIVISION_CODE,
                                     SI.BRANCH_CODE,
                                     TO_CHAR(SI.PLAN_DATE,'YYYY'),
                                     TO_CHAR(SI.PLAN_DATE,'MON')";
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
        public bool cloneMaterialPlan(string planCode, string planName, string customers, string employees, string branchs, string divisions, string remarks)
        {
            try
            {
                remarks = remarks.Replace("\"", " ");
                string existingPlanNameQuery = $@"SELECT PLAN_EDESC FROM PL_BRD_PRCMT_PLAN WHERE PLAN_CODE='{planCode}'";
                string existingPlanName = this._dbContext.SqlQuery<String>(existingPlanNameQuery).FirstOrDefault();
                if (existingPlanName != null)
                {
                    string copyPlanName = existingPlanName + "_copy";
                    if (!string.IsNullOrEmpty(planName))
                        copyPlanName = planName;
                    string maxPlanCodeQuery = "SELECT COALESCE(MAX(PLAN_CODE)+1, MAX(PLAN_CODE) + 1, 1) FROM PL_BRD_PRCMT_PLAN";
                    int maxPlanCode = this._dbContext.SqlQuery<int>(maxPlanCodeQuery).First();

                    string copySalesPlanQuery = $@"INSERT INTO PL_BRD_PRCMT_PLAN(PLAN_CODE,PLAN_EDESC,PLAN_NDESC,EST_AMOUNT,TIME_FRAME_CODE,
                    CALENDAR_TYPE,START_DATE,END_DATE,REMARKS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,
                    DELETED_FLAG)
                    SELECT '{maxPlanCode}','{copyPlanName}', PLAN_NDESC,EST_AMOUNT,TIME_FRAME_CODE,
                    CALENDAR_TYPE,START_DATE,END_DATE,'{remarks}',COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,
                    DELETED_FLAG FROM PL_BRD_PRCMT_PLAN WHERE PLAN_CODE='{planCode}'";
                    var insertCopiedPlan = this._dbContext.ExecuteSqlCommand(copySalesPlanQuery);

                    int copiedPlanCode = maxPlanCode;
                    if (copiedPlanCode > 0)
                    {
                        string copyinto_plandtl = $@"INSERT INTO PL_BRD_PRCMT_PLAN_DTL
                                (PLAN_CODE,PLAN_DATE,PER_DAY_AMOUNT,ITEM_CODE,
                                DIVISION_CODE,REMARKS,
                                COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,DELETED_FLAG,FREQUENCY_JSON)
                                SELECT '{copiedPlanCode}', PLAN_DATE,PER_DAY_AMOUNT,ITEM_CODE,
                               '{divisions}',REMARKS,
                                COMPANY_CODE,'{branchs}',CREATED_BY,CREATED_DATE,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,DELETED_FLAG,FREQUENCY_JSON FROM PL_BRD_PRCMT_PLAN_DTL WHERE PLAN_CODE='{planCode}'";

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
        #endregion
        public List<PlanProductTree> getAllFGProducts()
        {
            var company_code = _workcontext.CurrentUserinformation.company_code;
            var branch_code = _workcontext.CurrentUserinformation.branch_code;

            string item_filter_condition = "";
            try
            {
                var url = HttpContext.Current.Request.Url.AbsoluteUri;

                string xmlpath = HttpContext.Current.Server.MapPath("~/Areas/NeoERP.Planning/ProductCondition.xml");
                var xml = XDocument.Load(xmlpath);
                var condition_query = from c in xml.Root.Descendants("Vendor")
                                      where (string)c.Attribute("ID") == "04"
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

            string query = $@"SELECT * FROM IP_ITEM_MASTER_SETUP WHERE  GROUP_SKU_FLAG='I' AND DELETED_FLAG='N' AND COMPANY_CODE='{company_code}' {item_filter_condition}";
            var productListNodes = _dbContext.SqlQuery<PlanProductTree>(query).ToList();
            return productListNodes;
            //var result = new List<PlanProductTree>();
            //try
            //{
            //    var productQry = $@"SELECT * FROM IP_ITEM_MASTER_SETUP WHERE CATEGORY_CODE='FG' AND GROUP_SKU_FLAG='I' AND DELETED_FLAG='N' AND COMPANY_CODE='{company_code}'";
            //    result = this._dbContext.SqlQuery<PlanProductTree>(productQry).ToList();
            //}
            //catch (Exception)
            //{
            //    throw;
            //}
            //return result;
        }

        public List<MaterialPlanModel> getAllChildItemsForMaterialPlan(GetMaterialPlanModel model)
        {
            var company_code = _workcontext.CurrentUserinformation.company_code;
            var result = new List<MaterialPlanModel>();
            var finalResult = new List<MaterialPlanModel>();
            try
            {
                var resultLists = new List<MaterialProcessPlanModel>();
                foreach (var finishitem in model.mpList)
                {
                    var itemsLists = new List<inOutQtyMaterial>();
                    var finishedGoodItemList = "";
                    finishedGoodItemList = finishitem.FG_ITEM_CODE;


                    var items = recursiveMaterialPlanWithQry(itemsLists, finishedGoodItemList, 1, 1);

                    PR_GenerateActualData(finishedGoodItemList);

                    var allItemsList = string.Join("','", items.Select(a => a.ITEM_CODE).ToList());

                    var itemValueQry = $@" select PLAN_CODE,FINISHED_ITEM_CODE,CATEGORY_CODE,COMPANY_CODE,RAW_ITEM_CODE ,SUM(FINISHED_QUANTITY)FINISHED_QUANTITY,SUM(REQUIRED_QUANTITY)REQUIRED_QUANTITY from MP_VARIANCE_INFO 
                                        WHERE PLAN_CODE=1 AND COMPANY_CODE='{company_code}' AND CATEGORY_CODE IN (select category_code from ip_category_code where category_type='RM' and company_code='{company_code}')
                                        GROUP BY PLAN_CODE,FINISHED_ITEM_CODE,CATEGORY_CODE,COMPANY_CODE,RAW_ITEM_CODE";
                    var itemValue = _dbContext.SqlQuery<VarianceInfoModel>(itemValueQry).ToList();



                    var query = $@"SELECT DISTINCT '' PROCESS_EDESC, '' PROCESS_CODE, '' INDEX_ITEM_CODE,RIS.ITEM_CODE,'' INDEX_ITEM_EDESC,IMS.ITEM_EDESC,
                                          RIS.MU_CODE, RIS.COMPANY_CODE,IC.CATEGORY_CODE,IC.CATEGORY_EDESC,
                                          (SELECT SUM(NVL(QUANTITY,0)) FROM IP_PURCHASE_ORDER WHERE ITEM_CODE = RIS.ITEM_CODE AND COMPANY_CODE = RIS.COMPANY_CODE AND DELETED_FLAG='N'  
                                            AND ORDER_NO NOT IN (SELECT REFERENCE_NO FROM REFERENCE_DETAIL WHERE COMPANY_CODE = RIS.COMPANY_CODE AND DELETED_FLAG = 'N' and ITEM_CODE = RIS.ITEM_CODE))PO_PENDING,
                                            (SELECT SUM(NVL(IN_QUANTITY,0))-SUM(NVL(OUT_QUANTITY,0)) FROM V$VIRTUAL_STOCK_WIP_LEDGER1 WHERE ITEM_CODE = RIS.ITEM_CODE 
                                            AND COMPANY_CODE = RIS.COMPANY_CODE AND DELETED_FLAG='N')STOCK
                                        FROM MP_PROCESS_SETUP PS, MP_ROUTINE_INPUT_SETUP RIS,IP_ITEM_MASTER_SETUP IMS,IP_CATEGORY_CODE IC, MP_ROUTINE_OUTPUT_SETUP MOS
                                        WHERE  PS.COMPANY_CODE = '{company_code}'
                                           AND PS.COMPANY_CODE= RIS.COMPANY_CODE(+) 
                                        AND PS.PROCESS_CODE = RIS.PROCESS_CODE(+)
                                        AND PS.PROCESS_CODE = MOS.PROCESS_CODE(+)
                                        AND RIS.ITEM_CODE= IMS.ITEM_CODE 
                                        AND MOS.ITEM_CODE = PS.INDEX_ITEM_CODE
                                        AND RIS.COMPANY_CODE = IMS.COMPANY_CODE
                                        AND MOS.COMPANY_CODE = IMS.COMPANY_CODE
                                        AND IMS.CATEGORY_CODE = IC.CATEGORY_CODE
                                        AND IMS.COMPANY_CODE = IC.COMPANY_CODE
                                        AND IC.CATEGORY_CODE IN (select category_code from ip_category_code where category_type='RM' and company_code='{company_code}')
                                        AND PS.PROCESS_CODE IN (select PROCESS_CODE from MP_ROUTINE_INPUT_SETUP where item_code in('{allItemsList}') and company_code='{company_code}'
                                                UNION ALL
                                                select PROCESS_CODE from MP_ROUTINE_OUTPUT_SETUP where item_code in('{allItemsList}') and company_code='{company_code}')
                                        ORDER BY IMS.ITEM_EDESC";
                    var allData = _dbContext.SqlQuery<MaterialProcessPlanModel>(query).ToList();

                    var materialQty = Convert.ToDecimal(finishitem.QTY);
                    var filterList = allData.Where(x => x.INDEX_ITEM_CODE == finishitem.FG_ITEM_CODE).ToList();
                    filterList = allData;
                    foreach (var filterItem in filterList)
                    {
                        foreach (var citem in itemValue)
                        {
                            if (citem.RAW_ITEM_CODE == filterItem.ITEM_CODE)
                            {

                                var obj = new MaterialProcessPlanModel()
                                {
                                    ITEM_EDESC = filterItem.ITEM_EDESC,
                                    ITEM_CODE = filterItem.ITEM_CODE,
                                    STOCK = filterItem.STOCK,
                                    PO_PENDING = filterItem.PO_PENDING,
                                    INDEX_ITEM_CODE = filterItem.INDEX_ITEM_CODE,
                                    INDEX_ITEM_EDESC = filterItem.INDEX_ITEM_EDESC,
                                    CATEGORY_CODE = filterItem.CATEGORY_CODE,
                                    CATEGORY_EDESC = filterItem.CATEGORY_EDESC,
                                    REQUIRED_QTY = filterItem.QUANTITY,
                                    QUANTITY = (citem.REQUIRED_QUANTITY * materialQty),
                                    MU_CODE = filterItem.MU_CODE
                                };
                                resultLists.Add(obj);
                            }
                        }

                    }
                }
                if (resultLists.Count > 0)
                {
                    foreach (var item in resultLists)
                    {
                        var obj = new MaterialPlanModel()
                        {
                            ITEM_CODE = item.ITEM_CODE,
                            ITEM_EDESC = item.ITEM_EDESC,
                            CATEGORY_CODE = item.CATEGORY_CODE,
                            CATEGORY_EDESC = item.CATEGORY_EDESC,
                            INDEX_MU_CODE = item.MU_CODE,
                            FINISHED_ITEM_CODE = item.INDEX_ITEM_CODE,
                            FINISHED_ITEM_EDESC = item.INDEX_ITEM_EDESC,
                            REQUIRED_QUANTITY = item.REQUIRED_QTY == null ? 0 : item.REQUIRED_QTY,
                            STOCK = item.STOCK == null ? 0 : item.STOCK,
                            PO_PENDING = item.PO_PENDING == null ? 0 : item.PO_PENDING,
                            CALC_QTY = Convert.ToDecimal(item.QUANTITY),
                            REMAINING_QTY = Convert.ToDecimal(item.QUANTITY) - (Convert.ToDecimal(item.STOCK) + Convert.ToDecimal(item.PO_PENDING))
                        };
                        finalResult.Add(obj);
                    }
                }
                result = finalResult;

            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }
        public List<inOutQtyMaterial> recursiveMaterialPlanWithQry(List<inOutQtyMaterial> itemList, string item_code, decimal outQty, decimal inQty)
        {
            var company_code = _workcontext.CurrentUserinformation.company_code;
            try
            {
                var query = $@"SELECT RIS.ITEM_CODE,ROS.ITEM_CODE PATENT_ITEM_CODE,RIS.QUANTITY IN_QTY,ROS.QUANTITY OUT_QTY--,IMS.CATEGORY_CODE 
                                  FROM MP_ROUTINE_INPUT_SETUP RIS
                                 --INNER JOIN IP_ITEM_MASTER_SETUP IMS ON RIS.ITEM_CODE = IMS.ITEM_CODE AND RIS.COMPANY_CODE= IMS.COMPANY_CODE
                                LEFT JOIN MP_ROUTINE_OUTPUT_SETUP ROS ON RIS.PROCESS_CODE = ROS.PROCESS_CODE AND RIS.COMPANY_CODE = ROS.COMPANY_CODE
                                WHERE ROS.ITEM_CODE='{item_code}' AND ROS.COMPANY_CODE='{company_code}' ";

                var allItems = _dbContext.SqlQuery<inOutQtyMaterial>(query).ToList();
                if (allItems.Count() > 0)
                {
                    foreach (var item in allItems)
                    {
                        if (item.ITEM_CODE != item.PATENT_ITEM_CODE)
                        {
                            var parentInQty = GetParentItemQty(item.PATENT_ITEM_CODE);
                            recursiveMaterialPlanWithQry(itemList, item.ITEM_CODE, outQty, item.IN_QTY / Convert.ToDecimal(parentInQty));
                            var obj = new inOutQtyMaterial
                            {
                                ITEM_CODE = item.ITEM_CODE,
                                IN_QTY = item.IN_QTY,
                                OUT_QTY = item.OUT_QTY
                            };
                            itemList.Add(obj);
                        }
                    }
                }
                return itemList;
            }
            catch (Exception)
            {

                throw;
            }

        }
        public List<MaterialPlanModel> GetAllRawMaterialByFinishGood(GetMaterialPlanModel model)
        {
            var company_code = _workcontext.CurrentUserinformation.company_code;
            var result = new List<MaterialPlanModel>();
            var finalResult = new List<MaterialPlanModel>();
            try
            {
                var finishedGoodItemList = "";
                finishedGoodItemList = string.Join("','", model.mpList.Where(x => x.FG_ITEM_CODE != null || x.FG_ITEM_CODE != "").Select(x => x.FG_ITEM_CODE.ToString()).ToArray()).ToString();

                //var stringItemLists = new List<string>();
                //var items = recursiveMaterialPlanWithQry(stringItemLists, finishedGoodItemList);
                //var allItemsList = string.Join("','", items);
                var query = $@"SELECT DISTINCT LPAD (' ', LEVEL * 2, ' ') || PS.PROCESS_EDESC PROCESS_EDESC, PS.PROCESS_CODE, PS.INDEX_ITEM_CODE,RIS.ITEM_CODE,
                                        (SELECT ITEM_EDESC FROM IP_ITEM_MASTER_SETUP WHERE DELETED_FLAG='N' AND COMPANY_CODE='{company_code}' AND ITEM_CODE=MOS.ITEM_CODE)INDEX_ITEM_EDESC,IMS.ITEM_EDESC,
                                          ROUND((RIS.QUANTITY/MOS.QUANTITY),2)QUANTITY, RIS.MU_CODE, RIS.COMPANY_CODE,IC.CATEGORY_CODE,IC.CATEGORY_EDESC,
                                          (SELECT SUM(NVL(QUANTITY,0)) FROM IP_PURCHASE_ORDER WHERE ITEM_CODE = RIS.ITEM_CODE AND COMPANY_CODE = RIS.COMPANY_CODE AND DELETED_FLAG='N'  
                                            AND ORDER_NO NOT IN (SELECT REFERENCE_NO FROM REFERENCE_DETAIL WHERE COMPANY_CODE = RIS.COMPANY_CODE AND DELETED_FLAG = 'N' and ITEM_CODE = RIS.ITEM_CODE))PO_PENDING,
                                            (SELECT SUM(NVL(IN_QUANTITY,0))-SUM(NVL(OUT_QUANTITY,0)) FROM V$VIRTUAL_STOCK_WIP_LEDGER1 WHERE ITEM_CODE = RIS.ITEM_CODE 
                                            AND COMPANY_CODE = RIS.COMPANY_CODE AND DELETED_FLAG='N')STOCK
                                        FROM MP_PROCESS_SETUP PS, MP_ROUTINE_INPUT_SETUP RIS,IP_ITEM_MASTER_SETUP IMS,IP_CATEGORY_CODE IC, MP_ROUTINE_OUTPUT_SETUP MOS
                                        WHERE  PS.COMPANY_CODE = '{company_code}'
                                           AND PS.COMPANY_CODE= RIS.COMPANY_CODE(+) 
                                        AND PS.PROCESS_CODE = RIS.PROCESS_CODE(+)
                                        AND PS.PROCESS_CODE = MOS.PROCESS_CODE(+)
                                        AND RIS.ITEM_CODE= IMS.ITEM_CODE 
                                        AND MOS.ITEM_CODE = PS.INDEX_ITEM_CODE
                                        AND RIS.COMPANY_CODE = IMS.COMPANY_CODE
                                        AND MOS.COMPANY_CODE = IMS.COMPANY_CODE
                                        AND IMS.CATEGORY_CODE = IC.CATEGORY_CODE
                                        AND IMS.COMPANY_CODE = IC.COMPANY_CODE
                                    CONNECT BY PRIOR PS.PROCESS_CODE = PS.PRE_PROCESS_CODE AND PS.COMPANY_CODE ='{company_code}'
                                    START WITH PS.PRE_PROCESS_CODE IN(SELECT PRE_PROCESS_CODE FROM MP_PROCESS_SETUP WHERE INDEX_ITEM_CODE IN ('{finishedGoodItemList}') AND COMPANY_CODE = '{company_code}')
                                    AND PS.COMPANY_CODE='{company_code}' ORDER BY IMS.ITEM_EDESC";


                var allData = _dbContext.SqlQuery<MaterialProcessPlanModel>(query).ToList();

                var resultLists = new List<MaterialProcessPlanModel>();
                foreach (var finishitem in model.mpList)
                {
                    var materialQty = Convert.ToDecimal(finishitem.QTY);
                    var filterList = allData.Where(x => x.INDEX_ITEM_CODE == finishitem.FG_ITEM_CODE).ToList();
                    //var filterList = allData;
                    foreach (var filterItem in filterList)
                    {
                        var obj = new MaterialProcessPlanModel()
                        {
                            ITEM_EDESC = filterItem.ITEM_EDESC,
                            ITEM_CODE = filterItem.ITEM_CODE,
                            STOCK = filterItem.STOCK,
                            PO_PENDING = filterItem.PO_PENDING,
                            INDEX_ITEM_CODE = filterItem.INDEX_ITEM_CODE,
                            INDEX_ITEM_EDESC = filterItem.INDEX_ITEM_EDESC,
                            CATEGORY_CODE = filterItem.CATEGORY_CODE,
                            CATEGORY_EDESC = filterItem.CATEGORY_EDESC,
                            REQUIRED_QTY = filterItem.QUANTITY,
                            QUANTITY = filterItem.QUANTITY * materialQty,
                            MU_CODE = filterItem.MU_CODE
                        };
                        resultLists.Add(obj);
                        recursiveMaterialPlan(allData, resultLists, filterItem.ITEM_CODE, materialQty);
                    }
                }
                if (resultLists.Count > 0)
                {
                    foreach (var item in resultLists)
                    {
                        var obj = new MaterialPlanModel()
                        {
                            ITEM_CODE = item.ITEM_CODE,
                            ITEM_EDESC = item.ITEM_EDESC,
                            CATEGORY_CODE = item.CATEGORY_CODE,
                            CATEGORY_EDESC = item.CATEGORY_EDESC,
                            INDEX_MU_CODE = item.MU_CODE,
                            FINISHED_ITEM_CODE = item.INDEX_ITEM_CODE,
                            FINISHED_ITEM_EDESC = item.INDEX_ITEM_EDESC,
                            REQUIRED_QUANTITY = item.REQUIRED_QTY == null ? 0 : item.REQUIRED_QTY,
                            STOCK = item.STOCK == null ? 0 : item.STOCK,
                            PO_PENDING = item.PO_PENDING == null ? 0 : item.PO_PENDING,
                            CALC_QTY = Convert.ToDecimal(item.QUANTITY),
                            REMAINING_QTY = Convert.ToDecimal(item.QUANTITY) - (Convert.ToDecimal(item.STOCK) + Convert.ToDecimal(item.PO_PENDING))
                        };
                        finalResult.Add(obj);
                    }
                }
                result = finalResult;

            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }
        public List<MaterialProcessPlanModel> recursiveMaterialPlan(List<MaterialProcessPlanModel> processModel, List<MaterialProcessPlanModel> processList, string item_code, decimal qty)
        {
            var filteredList = processModel.Where(a => a.INDEX_ITEM_CODE == item_code).ToList();
            if (filteredList.Count() > 0)
            {
                foreach (var item in filteredList)
                {
                    var obj = new MaterialProcessPlanModel()
                    {
                        ITEM_EDESC = item.ITEM_EDESC,
                        ITEM_CODE = item.ITEM_CODE,
                        INDEX_ITEM_CODE = item.INDEX_ITEM_CODE,
                        CATEGORY_CODE = item.CATEGORY_CODE,
                        CATEGORY_EDESC = item.CATEGORY_EDESC,
                        REQUIRED_QTY = item.QUANTITY,
                        QUANTITY = item.QUANTITY * qty,
                        MU_CODE = item.MU_CODE
                    };
                    processList.Add(obj);
                    recursiveMaterialPlan(processModel, processList, item.ITEM_CODE, qty);
                }
            }
            return processList;
        }

        private List<EmployeeTree> generateEmployeeTree(List<EmployeeModels> model, List<EmployeeTree> employeeNodes, string preEmployeeCode)
        {
            foreach (var employee in model.Where(x => x.PRE_EMPLOYEE_CODE == preEmployeeCode))
            {
                var employeeNodesChild = new List<EmployeeTree>();
                employeeNodes.Add(new EmployeeTree()
                {
                    Level = employee.LEVEL,
                    employeeName = employee.EMPLOYEE_EDESC,
                    employeeId = employee.EMPLOYEE_CODE,
                    masterEmployeeCode = employee.MASTER_EMPLOYEE_CODE,
                    groupSkuFlag = employee.GROUP_SKU_FLAG,
                    preEmployeeCode = employee.PRE_EMPLOYEE_CODE,
                    hasEmployees = employee.GROUP_SKU_FLAG == "G" ? true : false,
                    Items = employee.GROUP_SKU_FLAG == "G" ? generateEmployeeTree(model, employeeNodesChild, employee.MASTER_EMPLOYEE_CODE) : null,
                });

            }
            return employeeNodes;
        }
        public List<ProcurementPlan> BindSalesPlanByPlanCode()
        {
            var company_code = _workcontext.CurrentUserinformation.company_code;
            try
            {
                var query = $@"SELECT TO_CHAR(PLAN_CODE)PLAN_CODE,PLAN_EDESC FROM PL_SALES_PLAN WHERE DELETED_FLAG='N' AND COMPANY_CODE='{company_code}'";
                var result = _dbContext.SqlQuery<ProcurementPlan>(query).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<SalesOrderModel> BindSalesOrderCustomer()
        {
            var info = this._workcontext.CurrentUserinformation;
            try
            {
                var query = $@"SELECT DISTINCT CS.CUSTOMER_CODE, CS.CUSTOMER_EDESC FROM SA_SALES_ORDER SO
                                INNER JOIN SA_CUSTOMER_SETUP CS ON CS.CUSTOMER_CODE = SO.CUSTOMER_CODE AND CS.COMPANY_CODE = SO.COMPANY_CODE
                                WHERE  CS.DELETED_FLAG='N' AND CS.COMPANY_CODE ='{info.company_code}'";
                var result = _dbContext.SqlQuery<SalesOrderModel>(query).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<ProcurementPlan> BindProductionPlanByPlanCode()
        {
            var company_code = _workcontext.CurrentUserinformation.company_code;
            try
            {
                var query = $@"SELECT TO_CHAR(PLAN_CODE)PLAN_CODE,PLAN_EDESC FROM PL_PRO_PLAN WHERE DELETED_FLAG='N' AND COMPANY_CODE='{company_code}'";
                var result = _dbContext.SqlQuery<ProcurementPlan>(query).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<ProcureFromMaterialModel> GetAllSalesPlanItemByPlanCode(string pCode)
        {
            var company_code = _workcontext.CurrentUserinformation.company_code;
            var result = new List<ProcureFromMaterialModel>();
            try
            {
                var query = $@" SELECT DISTINCT MPD.ITEM_CODE,
                            INITCAP(IMS.ITEM_EDESC)ITEM_EDESC,IMS.GROUP_SKU_FLAG FROM PL_SALES_PLAN_DTL MPD
                                    INNER JOIN IP_ITEM_MASTER_SETUP IMS ON MPD.ITEM_CODE=IMS.ITEM_CODE
                                    WHERE MPD.DELETED_FLAG='N' AND IMS.GROUP_SKU_FLAG='I' AND MPD.COMPANY_CODE='{company_code}' AND MPD.PLAN_CODE IN ('{pCode}')";

                result = _dbContext.SqlQuery<ProcureFromMaterialModel>(query).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public List<ProcureFromMaterialModel> GetAllProductionPlanItemByPlanCode(string pCode)
        {
            var company_code = _workcontext.CurrentUserinformation.company_code;
            var result = new List<ProcureFromMaterialModel>();
            try
            {
                var query = $@"SELECT DISTINCT MPD.ITEM_CODE,
                            INITCAP(IMS.ITEM_EDESC)ITEM_EDESC,IMS.GROUP_SKU_FLAG FROM PL_PRO_PLAN_DTL MPD
                                    INNER JOIN IP_ITEM_MASTER_SETUP IMS ON MPD.ITEM_CODE=IMS.ITEM_CODE
                                    WHERE MPD.DELETED_FLAG='N' AND IMS.GROUP_SKU_FLAG='I' AND MPD.COMPANY_CODE='{company_code}' AND MPD.PLAN_CODE IN ('{pCode}')";

                result = _dbContext.SqlQuery<ProcureFromMaterialModel>(query).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public List<ProcureFromMaterialModel> GetAllRawMaterialByProductionPlanCode(string pCode)
        {
            var company_code = _workcontext.CurrentUserinformation.company_code;
            var result = new List<ProcureFromMaterialModel>();
            var finalResult = new List<ProcureFromMaterialModel>();
            try
            {
                var query = $@"SELECT DISTINCT LPAD (' ', LEVEL * 2, ' ') || PS.PROCESS_EDESC PROCESS_EDESC, PS.PROCESS_CODE, PS.INDEX_ITEM_CODE,RIS.ITEM_CODE,
                                        (SELECT ITEM_EDESC FROM IP_ITEM_MASTER_SETUP WHERE DELETED_FLAG='N' AND COMPANY_CODE='{company_code}' AND ITEM_CODE=MOS.ITEM_CODE)INDEX_ITEM_EDESC,IMS.ITEM_EDESC,
                                          (RIS.QUANTITY/MOS.QUANTITY)QUANTITY, RIS.MU_CODE, RIS.COMPANY_CODE,IC.CATEGORY_CODE,IC.CATEGORY_EDESC,
                                          (SELECT SUM(NVL(QUANTITY,0)) FROM IP_PURCHASE_ORDER WHERE ITEM_CODE = RIS.ITEM_CODE AND COMPANY_CODE = RIS.COMPANY_CODE AND DELETED_FLAG='N'  
                                            AND ORDER_NO NOT IN (SELECT REFERENCE_NO FROM REFERENCE_DETAIL WHERE COMPANY_CODE = RIS.COMPANY_CODE AND DELETED_FLAG = 'N' and ITEM_CODE = RIS.ITEM_CODE))PO_PENDING,
                                            (SELECT SUM(NVL(IN_QUANTITY,0))-SUM(NVL(OUT_QUANTITY,0)) FROM V$VIRTUAL_STOCK_WIP_LEDGER1 WHERE ITEM_CODE = RIS.ITEM_CODE 
                                            AND COMPANY_CODE = RIS.COMPANY_CODE AND DELETED_FLAG='N')STOCK
                                        FROM MP_PROCESS_SETUP PS, MP_ROUTINE_INPUT_SETUP RIS,IP_ITEM_MASTER_SETUP IMS,IP_CATEGORY_CODE IC, MP_ROUTINE_OUTPUT_SETUP MOS
                                        WHERE  PS.COMPANY_CODE = '{company_code}'
                                           AND PS.COMPANY_CODE= RIS.COMPANY_CODE(+) 
                                        AND PS.PROCESS_CODE = RIS.PROCESS_CODE(+)
                                        AND PS.PROCESS_CODE = MOS.PROCESS_CODE(+)
                                        AND RIS.ITEM_CODE= IMS.ITEM_CODE 
                                        AND MOS.ITEM_CODE = PS.INDEX_ITEM_CODE
                                        AND RIS.COMPANY_CODE = IMS.COMPANY_CODE
                                        AND MOS.COMPANY_CODE = IMS.COMPANY_CODE
                                        AND IMS.CATEGORY_CODE = IC.CATEGORY_CODE
                                        AND IMS.COMPANY_CODE = IC.COMPANY_CODE
                                    CONNECT BY PRIOR PS.PROCESS_CODE = PS.PRE_PROCESS_CODE AND PS.COMPANY_CODE ='{company_code}'
                                    START WITH PS.PRE_PROCESS_CODE IN(SELECT PRE_PROCESS_CODE FROM MP_PROCESS_SETUP WHERE
                                     INDEX_ITEM_CODE IN (select DISTINCT ppd.ITEM_CODE from pl_pro_plan_dtl ppd
                                    inner join ip_item_master_setup ims on ppd.item_code=ims.item_code and ppd.company_code= ims.company_code
                                    where ppd.company_code='{company_code}' and ims.group_sku_flag='I' and ppd.deleted_flag='N' and ppd.plan_code IN ({pCode})) AND COMPANY_CODE = '{company_code}')
                                    AND PS.COMPANY_CODE='{company_code}' ORDER BY IMS.ITEM_EDESC";

                var allData = _dbContext.SqlQuery<MaterialProcessPlanModel>(query).ToList();
                var monthWiseResult = new List<ProcureFromMaterialModel>();
                var monthQry = $@" SELECT  PPD.ITEM_CODE,IMS.ITEM_EDESC,TO_CHAR(fn_bs_month(SUBSTR(BS_DATE(PPD.PLAN_DATE),6,2)))MONTH,
                                     TO_CHAR(SUBSTR(BS_DATE(PPD.PLAN_DATE),6,2))MONTHINT,
                                     SUM(PPD.PER_DAY_QUANTITY)REQUIRED_QUANTITY,IMS.GROUP_SKU_FLAG FROM PL_PRO_PLAN_DTL PPD
                                     INNER JOIN IP_ITEM_MASTER_SETUP IMS ON PPD.ITEM_CODE=IMS.ITEM_CODE AND PPD.COMPANY_CODE=IMS.COMPANY_CODE
                                      WHERE PPD.DELETED_FLAG='N' AND PPD.COMPANY_CODE='{company_code}' AND PPD.PLAN_CODE IN({pCode}) AND IMS.GROUP_SKU_FLAG='I'
                                     GROUP BY PPD.ITEM_CODE,TO_CHAR(fn_bs_month(SUBSTR(BS_DATE(PPD.PLAN_DATE),6,2))),
                                        TO_CHAR(SUBSTR(BS_DATE(PPD.PLAN_DATE),6,2)),IMS.GROUP_SKU_FLAG,IMS.ITEM_EDESC";
                var monthResult = _dbContext.SqlQuery<ProcureFromMaterialModel>(monthQry).ToList();

                var resultLists = new List<MaterialProcessPlanModel>();

                foreach (var finishitem in monthResult)
                {
                    var materialQty = Convert.ToDecimal(finishitem.REQUIRED_QUANTITY);
                    var filterList = allData.Where(x => x.INDEX_ITEM_CODE == finishitem.ITEM_CODE).ToList();
                    foreach (var filterItem in filterList)
                    {
                        var obj = new MaterialProcessPlanModel()
                        {
                            ITEM_EDESC = filterItem.ITEM_EDESC,
                            ITEM_CODE = filterItem.ITEM_CODE,
                            INDEX_ITEM_CODE = finishitem.ITEM_CODE,
                            INDEX_ITEM_EDESC = finishitem.ITEM_EDESC,
                            CATEGORY_CODE = filterItem.CATEGORY_CODE,
                            CATEGORY_EDESC = filterItem.CATEGORY_EDESC,
                            REQUIRED_QTY = filterItem.QUANTITY,
                            QUANTITY = filterItem.QUANTITY * materialQty,
                            MU_CODE = filterItem.MU_CODE,
                            MONTH = finishitem.MONTH,
                            MONTHINT = finishitem.MONTHINT,
                            PO_PENDING = filterItem.PO_PENDING,
                            STOCK = filterItem.STOCK,
                            MONTHLY_REQ_QTY = finishitem.REQUIRED_QUANTITY
                        };
                        resultLists.Add(obj);
                        recursiveMaterialPlan(allData, resultLists, filterItem.ITEM_CODE, materialQty);
                    }
                }
                if (resultLists.Count > 0)
                {
                    foreach (var item in resultLists)
                    {
                        var obj = new ProcureFromMaterialModel()
                        {
                            ITEM_CODE = item.ITEM_CODE,
                            ITEM_EDESC = item.ITEM_EDESC,
                            CATEGORY_CODE = item.CATEGORY_CODE,
                            CATEGORY_EDESC = item.CATEGORY_EDESC,
                            FINISHED_ITEM_CODE = item.INDEX_ITEM_CODE,
                            FINISHED_ITEM_EDESC = item.INDEX_ITEM_EDESC,
                            REQUIRED_QTY = item.REQUIRED_QTY,
                            REQUIRED_QUANTITY = item.QUANTITY == null ? 0 : item.QUANTITY,
                            MONTH = item.MONTH,
                            MONTHINT = item.MONTHINT,
                            INDEX_MU_CODE = item.MU_CODE,
                            PO_PENDING = item.PO_PENDING,
                            STOCK = item.STOCK,
                            MONTHLY_REQ_QTY = item.MONTHLY_REQ_QTY
                        };
                        finalResult.Add(obj);
                    }
                }
                result = finalResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public List<ProcureFromMaterialModel> GetAllRawMaterialBySalesPlanCode(string pCode)
        {
            var company_code = _workcontext.CurrentUserinformation.company_code;
            var result = new List<ProcureFromMaterialModel>();
            var finalResult = new List<ProcureFromMaterialModel>();
            try
            {
                var query = $@"SELECT DISTINCT LPAD (' ', LEVEL * 2, ' ') || PS.PROCESS_EDESC PROCESS_EDESC, PS.PROCESS_CODE, PS.INDEX_ITEM_CODE,RIS.ITEM_CODE,
                                        (SELECT ITEM_EDESC FROM IP_ITEM_MASTER_SETUP WHERE DELETED_FLAG='N' AND COMPANY_CODE='{company_code}' AND ITEM_CODE=MOS.ITEM_CODE)INDEX_ITEM_EDESC,IMS.ITEM_EDESC,
                                           ROUND((RIS.QUANTITY/MOS.QUANTITY),2)QUANTITY, RIS.MU_CODE, RIS.COMPANY_CODE,IC.CATEGORY_CODE,IC.CATEGORY_EDESC,
                                          (SELECT SUM(NVL(QUANTITY,0)) FROM IP_PURCHASE_ORDER WHERE ITEM_CODE = RIS.ITEM_CODE AND COMPANY_CODE = RIS.COMPANY_CODE AND DELETED_FLAG='N'  
                                            AND ORDER_NO NOT IN (SELECT REFERENCE_NO FROM REFERENCE_DETAIL WHERE COMPANY_CODE = RIS.COMPANY_CODE AND DELETED_FLAG = 'N' and ITEM_CODE = RIS.ITEM_CODE))PO_PENDING,
                                            (SELECT SUM(NVL(IN_QUANTITY,0))-SUM(NVL(OUT_QUANTITY,0)) FROM V$VIRTUAL_STOCK_WIP_LEDGER1 WHERE ITEM_CODE = RIS.ITEM_CODE 
                                            AND COMPANY_CODE = RIS.COMPANY_CODE AND DELETED_FLAG='N')STOCK
                                        FROM MP_PROCESS_SETUP PS, MP_ROUTINE_INPUT_SETUP RIS,IP_ITEM_MASTER_SETUP IMS,IP_CATEGORY_CODE IC, MP_ROUTINE_OUTPUT_SETUP MOS
                                        WHERE  PS.COMPANY_CODE = '{company_code}'
                                           AND PS.COMPANY_CODE= RIS.COMPANY_CODE(+) 
                                        AND PS.PROCESS_CODE = RIS.PROCESS_CODE(+)
                                        AND PS.PROCESS_CODE = MOS.PROCESS_CODE(+)
                                        AND RIS.ITEM_CODE= IMS.ITEM_CODE 
                                        AND MOS.ITEM_CODE = PS.INDEX_ITEM_CODE
                                        AND RIS.COMPANY_CODE = IMS.COMPANY_CODE
                                        AND MOS.COMPANY_CODE = IMS.COMPANY_CODE
                                        AND IMS.CATEGORY_CODE = IC.CATEGORY_CODE
                                        AND IMS.COMPANY_CODE = IC.COMPANY_CODE
                                    CONNECT BY PRIOR PS.PROCESS_CODE = PS.PRE_PROCESS_CODE AND PS.COMPANY_CODE ='{company_code}'
                                    START WITH PS.PRE_PROCESS_CODE IN(SELECT PRE_PROCESS_CODE FROM MP_PROCESS_SETUP WHERE
                                     INDEX_ITEM_CODE IN (select DISTINCT ppd.ITEM_CODE from pl_sales_plan_dtl ppd
                                    inner join ip_item_master_setup ims on ppd.item_code=ims.item_code and ppd.company_code= ims.company_code
                                    where ppd.company_code='{company_code}' and ims.group_sku_flag='I' and ppd.deleted_flag='N' and ppd.plan_code IN({pCode})) AND COMPANY_CODE = '{company_code}')
                                    AND PS.COMPANY_CODE='{company_code}' ORDER BY IMS.ITEM_EDESC";

                //var query = $@"SELECT DISTINCT '' PROCESS_EDESC, '' PROCESS_CODE, '' INDEX_ITEM_CODE,RIS.ITEM_CODE,'' INDEX_ITEM_EDESC,IMS.ITEM_EDESC,
                //                          RIS.MU_CODE, RIS.COMPANY_CODE,IC.CATEGORY_CODE,IC.CATEGORY_EDESC,
                //                          (SELECT SUM(NVL(QUANTITY,0)) FROM IP_PURCHASE_ORDER WHERE ITEM_CODE = RIS.ITEM_CODE AND COMPANY_CODE = RIS.COMPANY_CODE AND DELETED_FLAG='N'  
                //                            AND ORDER_NO NOT IN (SELECT REFERENCE_NO FROM REFERENCE_DETAIL WHERE COMPANY_CODE = RIS.COMPANY_CODE AND DELETED_FLAG = 'N' and ITEM_CODE = RIS.ITEM_CODE))PO_PENDING,
                //                            (SELECT SUM(NVL(IN_QUANTITY,0))-SUM(NVL(OUT_QUANTITY,0)) FROM V$VIRTUAL_STOCK_WIP_LEDGER1 WHERE ITEM_CODE = RIS.ITEM_CODE 
                //                            AND COMPANY_CODE = RIS.COMPANY_CODE AND DELETED_FLAG='N')STOCK
                //                        FROM MP_PROCESS_SETUP PS, MP_ROUTINE_INPUT_SETUP RIS,IP_ITEM_MASTER_SETUP IMS,IP_CATEGORY_CODE IC, MP_ROUTINE_OUTPUT_SETUP MOS
                //                        WHERE  PS.COMPANY_CODE = '{company_code}'
                //                           AND PS.COMPANY_CODE= RIS.COMPANY_CODE(+) 
                //                        AND PS.PROCESS_CODE = RIS.PROCESS_CODE(+)
                //                        AND PS.PROCESS_CODE = MOS.PROCESS_CODE(+)
                //                        AND RIS.ITEM_CODE= IMS.ITEM_CODE 
                //                        AND MOS.ITEM_CODE = PS.INDEX_ITEM_CODE
                //                        AND RIS.COMPANY_CODE = IMS.COMPANY_CODE
                //                        AND MOS.COMPANY_CODE = IMS.COMPANY_CODE
                //                        AND IMS.CATEGORY_CODE = IC.CATEGORY_CODE
                //                        AND IMS.COMPANY_CODE = IC.COMPANY_CODE
                //                        AND PS.COMPANY_CODE='{company_code}' 
                //                        AND IC.CATEGORY_CODE IN ('RM','PM')
                //                        AND PS.PROCESS_CODE IN (select PROCESS_CODE from MP_ROUTINE_INPUT_SETUP where item_code in(select DISTINCT ppd.ITEM_CODE from pl_sales_plan_dtl ppd
                //                    inner join ip_item_master_setup ims on ppd.item_code=ims.item_code and ppd.company_code= ims.company_code
                //                    where ppd.company_code='{company_code}' and ims.group_sku_flag='I' and ppd.deleted_flag='N' and ppd.plan_code IN({pCode})) and company_code='{company_code}'
                //                                UNION ALL
                //                                select PROCESS_CODE from MP_ROUTINE_OUTPUT_SETUP where item_code in(select DISTINCT ppd.ITEM_CODE from pl_sales_plan_dtl ppd
                //                    inner join ip_item_master_setup ims on ppd.item_code=ims.item_code and ppd.company_code= ims.company_code
                //                    where ppd.company_code='{company_code}' and ims.group_sku_flag='I' and ppd.deleted_flag='N' and ppd.plan_code IN({pCode})) and company_code='{company_code}')
                //                        ORDER BY IMS.ITEM_EDESC";

                var allData = _dbContext.SqlQuery<MaterialProcessPlanModel>(query).ToList();
                var monthWiseResult = new List<ProcureFromMaterialModel>();
                var monthQry = $@" SELECT  PPD.ITEM_CODE,IMS.ITEM_EDESC,TO_CHAR(fn_bs_month(SUBSTR(BS_DATE(PPD.PLAN_DATE),6,2)))MONTH,
                                     TO_CHAR(SUBSTR(BS_DATE(PPD.PLAN_DATE),6,2))MONTHINT,
                                     SUM(PPD.PER_DAY_QUANTITY)REQUIRED_QUANTITY,IMS.GROUP_SKU_FLAG FROM PL_SALES_PLAN_DTL PPD
                                     INNER JOIN IP_ITEM_MASTER_SETUP IMS ON PPD.ITEM_CODE=IMS.ITEM_CODE AND PPD.COMPANY_CODE=IMS.COMPANY_CODE
                                      WHERE PPD.DELETED_FLAG='N' AND PPD.COMPANY_CODE='{company_code}' AND PPD.PLAN_CODE IN ({pCode}) AND IMS.GROUP_SKU_FLAG='I'
                                     GROUP BY PPD.ITEM_CODE,TO_CHAR(fn_bs_month(SUBSTR(BS_DATE(PPD.PLAN_DATE),6,2))),
                                        TO_CHAR(SUBSTR(BS_DATE(PPD.PLAN_DATE),6,2)),IMS.GROUP_SKU_FLAG,IMS.ITEM_EDESC";
                var monthResult = _dbContext.SqlQuery<ProcureFromMaterialModel>(monthQry).ToList();

                var resultLists = new List<MaterialProcessPlanModel>();

                //PR_GenerateActualData();

                var itemValueQry = $@"SELECT * FROM MP_VARIANCE_INFO WHERE PLAN_CODE=1";
                var itemValue = _dbContext.SqlQuery<VarianceInfoModel>(itemValueQry).ToList();

                foreach (var finishitem in monthResult)
                {
                    var materialQty = Convert.ToDecimal(finishitem.REQUIRED_QUANTITY);
                    var filterList = allData.Where(x => x.INDEX_ITEM_CODE == finishitem.ITEM_CODE).ToList();
                    foreach (var filterItem in filterList)
                    {
                        var obj = new MaterialProcessPlanModel()
                        {
                            ITEM_EDESC = filterItem.ITEM_EDESC,
                            ITEM_CODE = filterItem.ITEM_CODE,
                            INDEX_ITEM_CODE = finishitem.ITEM_CODE,
                            INDEX_ITEM_EDESC = finishitem.ITEM_EDESC,
                            CATEGORY_CODE = filterItem.CATEGORY_CODE,
                            CATEGORY_EDESC = filterItem.CATEGORY_EDESC,
                            REQUIRED_QTY = filterItem.QUANTITY,
                            QUANTITY = filterItem.QUANTITY * materialQty,
                            MU_CODE = filterItem.MU_CODE,
                            MONTH = finishitem.MONTH,
                            MONTHINT = finishitem.MONTHINT,
                            PO_PENDING = filterItem.PO_PENDING,
                            STOCK = filterItem.STOCK,
                            MONTHLY_REQ_QTY = finishitem.REQUIRED_QUANTITY
                        };
                        resultLists.Add(obj);
                        recursiveMaterialPlan(allData, resultLists, filterItem.ITEM_CODE, materialQty);
                    }
                }
                if (resultLists.Count > 0)
                {
                    foreach (var item in resultLists)
                    {
                        var obj = new ProcureFromMaterialModel()
                        {
                            ITEM_CODE = item.ITEM_CODE,
                            ITEM_EDESC = item.ITEM_EDESC,
                            CATEGORY_CODE = item.CATEGORY_CODE,
                            CATEGORY_EDESC = item.CATEGORY_EDESC,
                            FINISHED_ITEM_CODE = item.INDEX_ITEM_CODE,
                            FINISHED_ITEM_EDESC = item.INDEX_ITEM_EDESC,
                            REQUIRED_QTY = item.REQUIRED_QTY,
                            REQUIRED_QUANTITY = item.QUANTITY == null ? 0 : item.QUANTITY,
                            MONTH = item.MONTH,
                            MONTHINT = item.MONTHINT,
                            PO_PENDING = item.PO_PENDING,
                            STOCK = item.STOCK,
                            INDEX_MU_CODE = item.MU_CODE,
                            MONTHLY_REQ_QTY = item.MONTHLY_REQ_QTY
                        };
                        finalResult.Add(obj);
                    }
                }
                result = finalResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public List<ProcureFromMaterialModel> GetAllRawMaterialBySalesOrderCustomerCode(string pCode, string startDate, string endDate)
        {
            var company_code = _workcontext.CurrentUserinformation.company_code;
            var result = new List<ProcureFromMaterialModel>();
            var finalResult = new List<ProcureFromMaterialModel>();
            var condition = string.Empty;
            if (!string.IsNullOrEmpty(pCode))
            {
                condition = $@" AND PPD.CUSTOMER_CODE IN ({pCode})";
            }
            try
            {
                var query = $@"SELECT DISTINCT LPAD (' ', LEVEL * 2, ' ') || PS.PROCESS_EDESC PROCESS_EDESC, PS.PROCESS_CODE, PS.INDEX_ITEM_CODE,RIS.ITEM_CODE,
                                        (SELECT ITEM_EDESC FROM IP_ITEM_MASTER_SETUP WHERE DELETED_FLAG='N' AND COMPANY_CODE='{company_code}' AND ITEM_CODE=MOS.ITEM_CODE)INDEX_ITEM_EDESC,IMS.ITEM_EDESC,
                                          ROUND((RIS.QUANTITY/MOS.QUANTITY),2)QUANTITY, RIS.MU_CODE, RIS.COMPANY_CODE,IC.CATEGORY_CODE,IC.CATEGORY_EDESC,
                                          (SELECT SUM(NVL(QUANTITY,0)) FROM IP_PURCHASE_ORDER WHERE ITEM_CODE = RIS.ITEM_CODE AND COMPANY_CODE = RIS.COMPANY_CODE AND DELETED_FLAG='N'  
                                            AND ORDER_NO NOT IN (SELECT REFERENCE_NO FROM REFERENCE_DETAIL WHERE COMPANY_CODE = RIS.COMPANY_CODE AND DELETED_FLAG = 'N' and ITEM_CODE = RIS.ITEM_CODE))PO_PENDING,
                                            (SELECT SUM(NVL(IN_QUANTITY,0))-SUM(NVL(OUT_QUANTITY,0)) FROM V$VIRTUAL_STOCK_WIP_LEDGER1 WHERE ITEM_CODE = RIS.ITEM_CODE 
                                            AND COMPANY_CODE = RIS.COMPANY_CODE AND DELETED_FLAG='N')STOCK
                                        FROM MP_PROCESS_SETUP PS, MP_ROUTINE_INPUT_SETUP RIS,IP_ITEM_MASTER_SETUP IMS,IP_CATEGORY_CODE IC, MP_ROUTINE_OUTPUT_SETUP MOS
                                        WHERE  PS.COMPANY_CODE = '{company_code}'
                                           AND PS.COMPANY_CODE= RIS.COMPANY_CODE(+) 
                                        AND PS.PROCESS_CODE = RIS.PROCESS_CODE(+)
                                        AND PS.PROCESS_CODE = MOS.PROCESS_CODE(+)
                                        AND RIS.ITEM_CODE= IMS.ITEM_CODE 
                                        AND MOS.ITEM_CODE = PS.INDEX_ITEM_CODE
                                        AND RIS.COMPANY_CODE = IMS.COMPANY_CODE
                                        AND MOS.COMPANY_CODE = IMS.COMPANY_CODE
                                        AND IMS.CATEGORY_CODE = IC.CATEGORY_CODE
                                        AND IMS.COMPANY_CODE = IC.COMPANY_CODE
                                    CONNECT BY PRIOR PS.PROCESS_CODE = PS.PRE_PROCESS_CODE AND PS.COMPANY_CODE ='{company_code}'
                                    START WITH PS.PRE_PROCESS_CODE IN(SELECT PRE_PROCESS_CODE FROM MP_PROCESS_SETUP WHERE
                                     INDEX_ITEM_CODE IN (SELECT DISTINCT PPD.ITEM_CODE FROM SA_SALES_ORDER PPD
                                    INNER JOIN MASTER_TRANSACTION MT ON PPD.ORDER_NO = MT.VOUCHER_NO AND PPD.COMPANY_CODE = MT.COMPANY_CODE 
                                    INNER JOIN IP_ITEM_MASTER_SETUP IMS ON PPD.ITEM_CODE=IMS.ITEM_CODE AND PPD.COMPANY_CODE= IMS.COMPANY_CODE
                                    WHERE PPD.COMPANY_CODE='{company_code}' AND IMS.GROUP_SKU_FLAG='I' AND PPD.DELETED_FLAG='N' AND MT.POSTED_DATE IS NOT NULL {condition} AND PPD.ORDER_DATE>= TO_DATE('{startDate}','MM/DD/YYYY') 
                                    AND PPD.ORDER_DATE<= TO_DATE('{endDate}','MM/DD/YYYY')) AND COMPANY_CODE = '{company_code}')
                                    AND PS.COMPANY_CODE='{company_code}' ORDER BY IMS.ITEM_EDESC";

                var allData = _dbContext.SqlQuery<MaterialProcessPlanModel>(query).ToList();
                var monthWiseResult = new List<ProcureFromMaterialModel>();
                var monthQry = $@"  SELECT  PPD.ITEM_CODE,IMS.ITEM_EDESC,TO_CHAR(fn_bs_month(SUBSTR(BS_DATE(PPD.ORDER_DATE),6,2)))MONTH,
                                     TO_CHAR(SUBSTR(BS_DATE(PPD.ORDER_DATE),6,2))MONTHINT,
                                     SUM(PPD.QUANTITY)REQUIRED_QUANTITY,IMS.GROUP_SKU_FLAG FROM SA_SALES_ORDER PPD
                                     INNER JOIN MASTER_TRANSACTION MT ON PPD.ORDER_NO = MT.VOUCHER_NO AND PPD.COMPANY_CODE = MT.COMPANY_CODE 
                                     INNER JOIN IP_ITEM_MASTER_SETUP IMS ON PPD.ITEM_CODE=IMS.ITEM_CODE AND PPD.COMPANY_CODE=IMS.COMPANY_CODE
                                      WHERE PPD.DELETED_FLAG='N' AND MT.POSTED_DATE IS NOT NULL AND PPD.COMPANY_CODE='{company_code}' {condition}
                                    AND PPD.ORDER_DATE>= TO_DATE('{startDate}','MM/DD/YYYY') AND PPD.ORDER_DATE<= TO_DATE('{endDate}','MM/DD/YYYY')
                                    AND IMS.GROUP_SKU_FLAG='I'
                                     GROUP BY PPD.ITEM_CODE,TO_CHAR(fn_bs_month(SUBSTR(BS_DATE(PPD.ORDER_DATE),6,2))),
                                        TO_CHAR(SUBSTR(BS_DATE(PPD.ORDER_DATE),6,2)),IMS.GROUP_SKU_FLAG,IMS.ITEM_EDESC";
                var monthResult = _dbContext.SqlQuery<ProcureFromMaterialModel>(monthQry).ToList();

                var resultLists = new List<MaterialProcessPlanModel>();

                foreach (var finishitem in monthResult)
                {
                    var materialQty = Convert.ToDecimal(finishitem.REQUIRED_QUANTITY);
                    var filterList = allData.Where(x => x.INDEX_ITEM_CODE == finishitem.ITEM_CODE).ToList();
                    foreach (var filterItem in filterList)
                    {
                        var obj = new MaterialProcessPlanModel()
                        {
                            ITEM_EDESC = filterItem.ITEM_EDESC,
                            ITEM_CODE = filterItem.ITEM_CODE,
                            INDEX_ITEM_CODE = finishitem.ITEM_CODE,
                            INDEX_ITEM_EDESC = finishitem.ITEM_EDESC,
                            CATEGORY_CODE = filterItem.CATEGORY_CODE,
                            CATEGORY_EDESC = filterItem.CATEGORY_EDESC,
                            REQUIRED_QTY = filterItem.QUANTITY,
                            QUANTITY = filterItem.QUANTITY * materialQty,
                            MU_CODE = filterItem.MU_CODE,
                            MONTH = finishitem.MONTH,
                            MONTHINT = finishitem.MONTHINT,
                            PO_PENDING = filterItem.PO_PENDING,
                            STOCK = filterItem.STOCK,
                            MONTHLY_REQ_QTY = finishitem.REQUIRED_QUANTITY
                        };
                        resultLists.Add(obj);
                        recursiveMaterialPlan(allData, resultLists, filterItem.ITEM_CODE, materialQty);
                    }
                }
                if (resultLists.Count > 0)
                {
                    foreach (var item in resultLists)
                    {
                        var obj = new ProcureFromMaterialModel()
                        {
                            ITEM_CODE = item.ITEM_CODE,
                            ITEM_EDESC = item.ITEM_EDESC,
                            CATEGORY_CODE = item.CATEGORY_CODE,
                            CATEGORY_EDESC = item.CATEGORY_EDESC,
                            FINISHED_ITEM_CODE = item.INDEX_ITEM_CODE,
                            FINISHED_ITEM_EDESC = item.INDEX_ITEM_EDESC,
                            REQUIRED_QTY = item.REQUIRED_QTY,
                            REQUIRED_QUANTITY = item.QUANTITY == null ? 0 : item.QUANTITY,
                            MONTH = item.MONTH,
                            MONTHINT = item.MONTHINT,
                            PO_PENDING = item.PO_PENDING,
                            STOCK = item.STOCK,
                            INDEX_MU_CODE = item.MU_CODE,
                            MONTHLY_REQ_QTY = item.MONTHLY_REQ_QTY
                        };
                        finalResult.Add(obj);
                    }
                }
                result = finalResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }



        #region Material Plan
        public Double FN_RoutineOutQuantity(string ItemCode)
        {
            var CompanyCode = _workcontext.CurrentUserinformation.company_code;
            var outQty = 0.0;
            var query = $@"SELECT QUANTITY FROM MP_ROUTINE_OUTPUT_SETUP WHERE PROCESS_CODE = (SELECT PROCESS_CODE FROM MP_PROCESS_SETUP WHERE ROWNUM = 1 AND INDEX_ITEM_CODE ='{ItemCode}' 
                        AND COMPANY_CODE = '{CompanyCode}') AND COMPANY_CODE = '{CompanyCode}' AND ITEM_CODE ='{ItemCode}'";
            outQty = _dbContext.SqlQuery<double>(query).FirstOrDefault();
            return outQty;
        }

        public Double GetParentItemQty(string itemCode)
        {
            var CompanyCode = _workcontext.CurrentUserinformation.company_code;
            var outQty = 0.0;
            var query = $@"SELECT RIS.QUANTITY OUT_QTY FROM MP_ROUTINE_INPUT_SETUP RIS
                                LEFT JOIN MP_ROUTINE_OUTPUT_SETUP ROS ON RIS.PROCESS_CODE = ROS.PROCESS_CODE AND RIS.COMPANY_CODE = ROS.COMPANY_CODE
                                WHERE ROS.ITEM_CODE='{itemCode}' AND ROS.COMPANY_CODE='{CompanyCode}' ";
            outQty = _dbContext.SqlQuery<double>(query).FirstOrDefault();
            return outQty;
        }

        public int insertPlanInVariance(string plan_code, int iSerialNo, string vFinishedItemCode, double? iFinishedQuantity, string vItemCode, string strCategoryCode, string strProcessCode, double? iRequiredQuantity)
        {
            var CompanyCode = _workcontext.CurrentUserinformation.company_code;
            var query = $@"INSERT INTO MP_VARIANCE_INFO(plan_code, serial_no, finished_item_code, finished_quantity, raw_item_code,category_code,process_code, required_quantity, COMPANY_CODE, created_by, created_date, DELETED_FLAG)
                            VALUES('{plan_code}', {iSerialNo} , '{vFinishedItemCode}', {iFinishedQuantity} , '{vItemCode}','{strCategoryCode}','{strProcessCode}',{iRequiredQuantity}, '{CompanyCode}', 'ADMIN', SYSDATE,'N')";
            var result = _dbContext.ExecuteSqlCommand(query);
            return result;
        }

        public void PR_GenerateActualData(string itemCode)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            
            var rs1Routine = new List<RoutineMaterial>();
            var rs2Routine = new List<RoutineMaterial>();
            var rs3Routine = new List<RoutineMaterial>();
            var rs4Routine = new List<RoutineMaterial>();
            var rs5Routine = new List<RoutineMaterial>();
            var rs6Routine = new List<RoutineMaterial>();
            var rs7Routine = new List<RoutineMaterial>();
            var rs8Routine = new List<RoutineMaterial>();
            var rs9Routine = new List<RoutineMaterial>();

            var iSerialNo = 0;

            Double iOutQty, iOutQty1, iOutQty2, iOutQty3, iOutQty4, iOutQty5, iOutQty6, iOutQty7, iOutQty8, iOutQty9;

            var fgItem = itemCode;

            iOutQty = FN_RoutineOutQuantity(fgItem);

            var plancodeQry = $@"SELECT NVL(MAX(PLAN_CODE),1)PLAN_CODE FROM MP_VARIANCE_INFO";
            var planCode = _dbContext.SqlQuery<string>(plancodeQry).FirstOrDefault();

            var rs1RoutineQuery = $@" SELECT RIS.ITEM_CODE, RIS.QUANTITY, RIS.PROCESS_CODE,IMS.CATEGORY_CODE FROM MP_ROUTINE_INPUT_SETUP RIS
                    INNER JOIN IP_ITEM_MASTER_SETUP IMS ON  RIS.ITEM_CODE = IMS.ITEM_CODE AND RIS.COMPANY_CODE = IMS.COMPANY_CODE
                    WHERE RIS.PROCESS_CODE = (SELECT PROCESS_CODE FROM MP_PROCESS_SETUP WHERE ROWNUM = 1 AND INDEX_ITEM_CODE ='{fgItem}'AND COMPANY_CODE = '{companyCode}') AND RIS.COMPANY_CODE = '{companyCode}'";
            rs1Routine = _dbContext.SqlQuery<RoutineMaterial>(rs1RoutineQuery).ToList();


            var vQry = $@"DELETE FROM MP_VARIANCE_INFO WHERE COMPANY_CODE ='{companyCode}' AND PLAN_CODE = 1";
            _dbContext.ExecuteSqlCommand(vQry);



            if (rs1Routine.Count > 0)
            {
                foreach (var rsR1 in rs1Routine)
                {
                    var iRequiredQuantity = rsR1.QUANTITY / iOutQty;
                    iSerialNo = ++iSerialNo;
                    iOutQty1 = FN_RoutineOutQuantity(rsR1.ITEM_CODE);

                    var rs2RoutineQuery = $@"SELECT RIS.ITEM_CODE, RIS.QUANTITY, RIS.PROCESS_CODE,IMS.CATEGORY_CODE FROM MP_ROUTINE_INPUT_SETUP RIS
                                                INNER JOIN IP_ITEM_MASTER_SETUP IMS ON  RIS.ITEM_CODE = IMS.ITEM_CODE AND RIS.COMPANY_CODE = IMS.COMPANY_CODE
                                                WHERE RIS.PROCESS_CODE = (SELECT PROCESS_CODE FROM MP_PROCESS_SETUP WHERE ROWNUM = 1 AND INDEX_ITEM_CODE ='{rsR1.ITEM_CODE}'AND COMPANY_CODE = '{companyCode}') AND RIS.COMPANY_CODE = '{companyCode}'";
                    rs2Routine = _dbContext.SqlQuery<RoutineMaterial>(rs2RoutineQuery).ToList();

                    insertPlanInVariance(planCode, iSerialNo, fgItem, rsR1.QUANTITY, rsR1.ITEM_CODE, rsR1.CATEGORY_CODE, rsR1.PROCESS_CODE, iRequiredQuantity);

                    foreach (var rsR2 in rs2Routine)
                    {
                        iRequiredQuantity = rsR2.QUANTITY * rsR1.QUANTITY;
                        iRequiredQuantity = iRequiredQuantity / (iOutQty * iOutQty1);
                        iSerialNo = ++iSerialNo;

                        iOutQty2 = FN_RoutineOutQuantity(rsR2.ITEM_CODE);

                        var rs3RoutineQuery = $@"SELECT RIS.ITEM_CODE, RIS.QUANTITY, RIS.PROCESS_CODE,IMS.CATEGORY_CODE FROM MP_ROUTINE_INPUT_SETUP RIS
                                                INNER JOIN IP_ITEM_MASTER_SETUP IMS ON  RIS.ITEM_CODE = IMS.ITEM_CODE AND RIS.COMPANY_CODE = IMS.COMPANY_CODE
                                                WHERE RIS.PROCESS_CODE = (SELECT PROCESS_CODE FROM MP_PROCESS_SETUP WHERE ROWNUM = 1 AND INDEX_ITEM_CODE ='{rsR2.ITEM_CODE}'AND COMPANY_CODE = '{companyCode}') AND RIS.COMPANY_CODE = '{companyCode}'";
                        rs3Routine = _dbContext.SqlQuery<RoutineMaterial>(rs3RoutineQuery).ToList();

                        insertPlanInVariance(planCode, iSerialNo, fgItem, rsR2.QUANTITY, rsR2.ITEM_CODE, rsR2.CATEGORY_CODE, rsR2.PROCESS_CODE, iRequiredQuantity);

                        foreach (var rsR3 in rs3Routine)
                        {
                            iRequiredQuantity = rsR3.QUANTITY * rsR2.QUANTITY * rsR1.QUANTITY;
                            iRequiredQuantity = iRequiredQuantity / (iOutQty * iOutQty1 * iOutQty2);
                            iSerialNo = ++iSerialNo;

                            iOutQty3 = FN_RoutineOutQuantity(rsR3.ITEM_CODE);

                            var rs4RoutineQuery = $@"SELECT RIS.ITEM_CODE, RIS.QUANTITY, RIS.PROCESS_CODE,IMS.CATEGORY_CODE FROM MP_ROUTINE_INPUT_SETUP RIS
                                                INNER JOIN IP_ITEM_MASTER_SETUP IMS ON  RIS.ITEM_CODE = IMS.ITEM_CODE AND RIS.COMPANY_CODE = IMS.COMPANY_CODE
                                                WHERE RIS.PROCESS_CODE = (SELECT PROCESS_CODE FROM MP_PROCESS_SETUP WHERE ROWNUM = 1 AND INDEX_ITEM_CODE ='{rsR3.ITEM_CODE}'AND COMPANY_CODE = '{companyCode}') AND RIS.COMPANY_CODE = '{companyCode}'";

                            rs4Routine = _dbContext.SqlQuery<RoutineMaterial>(rs4RoutineQuery).ToList();

                            insertPlanInVariance(planCode, iSerialNo, fgItem, rsR3.QUANTITY, rsR3.ITEM_CODE, rsR3.CATEGORY_CODE, rsR3.PROCESS_CODE, iRequiredQuantity);

                            foreach (var rsR4 in rs4Routine)
                            {
                                iRequiredQuantity = rsR4.QUANTITY * rsR3.QUANTITY * rsR2.QUANTITY * rsR1.QUANTITY;
                                iRequiredQuantity = iRequiredQuantity / (iOutQty * iOutQty1 * iOutQty2 * iOutQty3);
                                iSerialNo = ++iSerialNo;

                                iOutQty4 = FN_RoutineOutQuantity(rsR4.ITEM_CODE);

                                var rs5RoutineQuery = $@"SELECT RIS.ITEM_CODE, RIS.QUANTITY, RIS.PROCESS_CODE,IMS.CATEGORY_CODE FROM MP_ROUTINE_INPUT_SETUP RIS
                                                INNER JOIN IP_ITEM_MASTER_SETUP IMS ON  RIS.ITEM_CODE = IMS.ITEM_CODE AND RIS.COMPANY_CODE = IMS.COMPANY_CODE
                                                WHERE RIS.PROCESS_CODE = (SELECT PROCESS_CODE FROM MP_PROCESS_SETUP WHERE ROWNUM = 1 AND INDEX_ITEM_CODE ='{rsR4.ITEM_CODE}'AND COMPANY_CODE = '{companyCode}') AND RIS.COMPANY_CODE = '{companyCode}'";

                                rs5Routine = _dbContext.SqlQuery<RoutineMaterial>(rs5RoutineQuery).ToList();

                                insertPlanInVariance(planCode, iSerialNo, fgItem, rsR4.QUANTITY, rsR4.ITEM_CODE, rsR4.CATEGORY_CODE, rsR4.PROCESS_CODE, iRequiredQuantity);

                                foreach (var rsR5 in rs5Routine)
                                {
                                    iRequiredQuantity = rsR5.QUANTITY * rsR4.QUANTITY * rsR3.QUANTITY * rsR2.QUANTITY * rsR1.QUANTITY;
                                    iRequiredQuantity = iRequiredQuantity / (iOutQty * iOutQty1 * iOutQty2 * iOutQty3 * iOutQty4);
                                    iSerialNo = ++iSerialNo;

                                    iOutQty5 = FN_RoutineOutQuantity(rsR5.ITEM_CODE);

                                    var rs6RoutineQuery = $@"SELECT RIS.ITEM_CODE, RIS.QUANTITY, RIS.PROCESS_CODE,IMS.CATEGORY_CODE FROM MP_ROUTINE_INPUT_SETUP RIS
                                                INNER JOIN IP_ITEM_MASTER_SETUP IMS ON  RIS.ITEM_CODE = IMS.ITEM_CODE AND RIS.COMPANY_CODE = IMS.COMPANY_CODE
                                                WHERE RIS.PROCESS_CODE = (SELECT PROCESS_CODE FROM MP_PROCESS_SETUP WHERE ROWNUM = 1 AND INDEX_ITEM_CODE ='{rsR5.ITEM_CODE}'AND COMPANY_CODE = '{companyCode}') AND RIS.COMPANY_CODE = '{companyCode}'";

                                    rs6Routine = _dbContext.SqlQuery<RoutineMaterial>(rs6RoutineQuery).ToList();

                                    insertPlanInVariance(planCode, iSerialNo, fgItem, rsR5.QUANTITY, rsR5.ITEM_CODE, rsR5.CATEGORY_CODE, rsR5.PROCESS_CODE, iRequiredQuantity);

                                    foreach (var rsR6 in rs6Routine)
                                    {
                                        iRequiredQuantity = rsR5.QUANTITY * rsR4.QUANTITY * rsR3.QUANTITY * rsR6.QUANTITY * rsR2.QUANTITY * rsR1.QUANTITY;
                                        iRequiredQuantity = iRequiredQuantity / (iOutQty * iOutQty1 * iOutQty2 * iOutQty3 * iOutQty4 * iOutQty5);
                                        iSerialNo = ++iSerialNo;

                                        iOutQty6 = FN_RoutineOutQuantity(rsR6.ITEM_CODE);


                                        var rs7RoutineQuery = $@"SELECT RIS.ITEM_CODE, RIS.QUANTITY, RIS.PROCESS_CODE,IMS.CATEGORY_CODE FROM MP_ROUTINE_INPUT_SETUP RIS
                                                INNER JOIN IP_ITEM_MASTER_SETUP IMS ON  RIS.ITEM_CODE = IMS.ITEM_CODE AND RIS.COMPANY_CODE = IMS.COMPANY_CODE
                                                WHERE RIS.PROCESS_CODE = (SELECT PROCESS_CODE FROM MP_PROCESS_SETUP WHERE ROWNUM = 1 AND INDEX_ITEM_CODE ='{rsR6.ITEM_CODE}'AND COMPANY_CODE = '{companyCode}') AND RIS.COMPANY_CODE = '{companyCode}'";

                                        rs7Routine = _dbContext.SqlQuery<RoutineMaterial>(rs7RoutineQuery).ToList();

                                        insertPlanInVariance(planCode, iSerialNo, fgItem, rsR6.QUANTITY, rsR6.ITEM_CODE, rsR6.CATEGORY_CODE, rsR6.PROCESS_CODE, iRequiredQuantity);

                                        foreach (var rsR7 in rs7Routine)
                                        {
                                            iRequiredQuantity = rsR7.QUANTITY * rsR5.QUANTITY * rsR4.QUANTITY * rsR3.QUANTITY * rsR6.QUANTITY * rsR2.QUANTITY * rsR1.QUANTITY;
                                            iRequiredQuantity = iRequiredQuantity / (iOutQty * iOutQty1 * iOutQty2 * iOutQty3 * iOutQty4 * iOutQty5 * iOutQty6);
                                            iSerialNo = ++iSerialNo;

                                            iOutQty7 = FN_RoutineOutQuantity(rsR7.ITEM_CODE);


                                            var rs8RoutineQuery = $@"SELECT RIS.ITEM_CODE, RIS.QUANTITY, RIS.PROCESS_CODE,IMS.CATEGORY_CODE FROM MP_ROUTINE_INPUT_SETUP RIS
                                                INNER JOIN IP_ITEM_MASTER_SETUP IMS ON  RIS.ITEM_CODE = IMS.ITEM_CODE AND RIS.COMPANY_CODE = IMS.COMPANY_CODE
                                                WHERE RIS.PROCESS_CODE = (SELECT PROCESS_CODE FROM MP_PROCESS_SETUP WHERE ROWNUM = 1 AND INDEX_ITEM_CODE ='{rsR7.ITEM_CODE}'AND COMPANY_CODE = '{companyCode}') AND RIS.COMPANY_CODE = '{companyCode}'";

                                            rs8Routine = _dbContext.SqlQuery<RoutineMaterial>(rs8RoutineQuery).ToList();

                                            insertPlanInVariance(planCode, iSerialNo, fgItem, rsR7.QUANTITY, rsR7.ITEM_CODE, rsR7.CATEGORY_CODE, rsR7.PROCESS_CODE, iRequiredQuantity);

                                            foreach (var rsR8 in rs8Routine)
                                            {
                                                iRequiredQuantity = rsR8.QUANTITY * rsR7.QUANTITY * rsR5.QUANTITY * rsR4.QUANTITY * rsR3.QUANTITY * rsR6.QUANTITY * rsR2.QUANTITY * rsR1.QUANTITY;
                                                iRequiredQuantity = iRequiredQuantity / (iOutQty * iOutQty1 * iOutQty2 * iOutQty3 * iOutQty4 * iOutQty5 * iOutQty6 * iOutQty7);
                                                iSerialNo = ++iSerialNo;

                                                iOutQty8 = FN_RoutineOutQuantity(rsR8.ITEM_CODE);


                                                var rs9RoutineQuery = $@"SELECT RIS.ITEM_CODE, RIS.QUANTITY, RIS.PROCESS_CODE,IMS.CATEGORY_CODE FROM MP_ROUTINE_INPUT_SETUP RIS
                                                                            INNER JOIN IP_ITEM_MASTER_SETUP IMS ON  RIS.ITEM_CODE = IMS.ITEM_CODE AND RIS.COMPANY_CODE = IMS.COMPANY_CODE
                                                                            WHERE RIS.PROCESS_CODE = (SELECT PROCESS_CODE FROM MP_PROCESS_SETUP WHERE ROWNUM = 1 AND INDEX_ITEM_CODE ='{rsR8.ITEM_CODE}'AND COMPANY_CODE = '{companyCode}') AND RIS.COMPANY_CODE = '{companyCode}'";

                                                rs9Routine = _dbContext.SqlQuery<RoutineMaterial>(rs9RoutineQuery).ToList();

                                                insertPlanInVariance(planCode, iSerialNo, fgItem, rsR8.QUANTITY, rsR8.ITEM_CODE, rsR8.CATEGORY_CODE, rsR8.PROCESS_CODE, iRequiredQuantity);

                                                foreach (var rsR9 in rs9Routine)
                                                {
                                                    iRequiredQuantity = rsR9.QUANTITY * rsR8.QUANTITY * rsR7.QUANTITY * rsR5.QUANTITY * rsR4.QUANTITY * rsR3.QUANTITY * rsR6.QUANTITY * rsR2.QUANTITY * rsR1.QUANTITY;
                                                    iRequiredQuantity = iRequiredQuantity / (iOutQty * iOutQty1 * iOutQty2 * iOutQty3 * iOutQty4 * iOutQty5 * iOutQty6 * iOutQty7 * iOutQty8);
                                                    iSerialNo = ++iSerialNo;

                                                    iOutQty9 = FN_RoutineOutQuantity(rsR9.ITEM_CODE);

                                                    insertPlanInVariance(planCode, iSerialNo, fgItem, rsR9.QUANTITY, rsR9.ITEM_CODE, rsR9.CATEGORY_CODE, rsR9.PROCESS_CODE, iRequiredQuantity);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
            }
        }
        #endregion Material Plan
        #region Material Plan PI
        public bool InsertMaterialPlanPI(IEnumerable<checklist> modellist, string from_department, string to_department, string voucherno)
        {
            try
            {
               
                foreach (var chk in modellist)
                {
                    int serialno = 1;
                    var insertQuery = $@"INSERT INTO IP_PURCHASE_REQUEST (REQUEST_NO,REQUEST_DATE,MANUAL_NO,FROM_LOCATION_CODE,TO_LOCATION_CODE,SERIAL_NO,ITEM_CODE,           
                            MU_CODE,QUANTITY,FORM_CODE,
                            COMPANY_CODE,BRANCH_CODE, CREATED_BY, CREATED_DATE, 
                                    DELETED_FLAG, CURRENCY_CODE, EXCHANGE_RATE)
                            VALUES ('{voucherno}',SYSDATE,'PlanningPI','{from_department}','{to_department}',{serialno},'{chk.ITEM_CODE}','{chk.MU_CODE}',{chk.QUANTITY},'310','{_workcontext.CurrentUserinformation.company_code}','{_workcontext.CurrentUserinformation.branch_code}','{_workcontext.CurrentUserinformation.login_code}',SYSDATE,'N','NRP',1)";
                    _dbContext.ExecuteSqlCommand(insertQuery);
                    serialno++;
                }
                string insertmasterQuery = $@"INSERT INTO MASTER_TRANSACTION(VOUCHER_NO,FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,DELETED_FLAG,CURRENCY_CODE,CREATED_DATE,VOUCHER_DATE,EXCHANGE_RATE) VALUES('{voucherno}','310','{_workcontext.CurrentUserinformation.company_code}','{_workcontext.CurrentUserinformation.branch_code}','{_workcontext.CurrentUserinformation.login_code}','N','NRS',SYSDATE,SYSDATE,1)";
                _dbContext.ExecuteSqlCommand(insertmasterQuery);
                return true;
            }
            catch (Exception ex)
            {

                return false; 
            }

           
        }
        public string NewVoucherNo(string companycode, string formcode, string transactiondate, string tablename)
        {
            try
            {
                if (companycode != "" && formcode != "" && transactiondate != "" && tablename != "")
                {
                    string query = string.Format(@"select FN_NEW_VOUCHER_NO('{0}','{1}','{2}','{3}') FROM DUAL", companycode, formcode, transactiondate, tablename);
                    string voucherNo = this._dbContext.SqlQuery<string>(query).First();
                    return voucherNo;
                }
                else
                { return ""; }

            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion Material Plan PI

    }
    public class countQty
    {
        public int? count { get; set; }
        public decimal? quantity { get; set; }
    }
    public class inOutQtyMaterial
    {
        public string ITEM_CODE { get; set; }
        public string PATENT_ITEM_CODE { get; set; }
        public decimal IN_QTY { get; set; }
        public decimal OUT_QTY { get; set; }
        public string CATEGORY_CODE { get; set; }
    }
    public class RoutineMaterial
    {
        public string ITEM_CODE { get; set; }
        public double? QUANTITY { get; set; }
        public string PROCESS_CODE { get; set; }
        public string CATEGORY_CODE { get; set; }
    }
    public class RoutineMaterialPlan
    {
        public string PLAN_CODE { get; set; }
        public double? QUANTITY { get; set; }
        public string PROCESS_CODE { get; set; }
    }
    public class VarianceInfoModel
    {
        public string FINISHED_ITEM_CODE { get; set; }
        public decimal? FINISHED_QUANTITY { get; set; }
        public string RAW_ITEM_CODE { get; set; }
        public decimal? REQUIRED_QUANTITY { get; set; }
        public string CATEGORY_CODE { get; set; }
        public string PROCESS_CODE { get; set; }
    }
}
