using NeoErp.Core;
using NeoErp.Data;
using NeoErp.Planning.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoErp.Core.Models;
using NeoErp.Planning.Service.Models;
using NeoErp.Core.Domain;
using NeoErp.Core.Helpers;

namespace NeoErp.Planning.Service.Repository
{
    public class COAPlanRepo : ICOAPlanRepo
    {
        private IDbContext _dbContext;
        private IWorkContext _workcontext;
        public COAPlanRepo(IDbContext dbContext, IWorkContext workContext)
        {
            this._workcontext = workContext;
            this._dbContext = dbContext;
        }

        public List<COASetupModel> COAListAllNodes(User userinfo)
        {
            //string query = @"SELECT LEVEL, 
            //     INITCAP(ITEM_EDESC) AS ItemName,
            //     ITEM_CODE AS ItemCode,
            //     MASTER_ITEM_CODE AS MasterItemCode, 
            //     PRE_ITEM_CODE AS PreItemCode, 
            //     GROUP_SKU_FLAG AS GroupFlag,
            //    (SELECT COUNT(*) FROM IP_ITEM_MASTER_SETUP WHERE  
            //    GROUP_SKU_FLAG='G' AND COMPANY_CODE='" + userinfo.company_code + @"' AND DELETED_FLAG='N' AND PRE_ITEM_CODE = ims.MASTER_ITEM_CODE) as Childrens 
            //     FROM IP_ITEM_MASTER_SETUP ims
            //     WHERE ims.DELETED_FLAG = 'N' 
            //     AND LEVEL=1
            //     AND ims.COMPANY_CODE = '" + userinfo.company_code + @"'
            //     AND GROUP_SKU_FLAG = 'G'
            //     START WITH PRE_ITEM_CODE = '00'
            //     CONNECT BY PRIOR MASTER_ITEM_CODE = PRE_ITEM_CODE";
            string query = $@"SELECT LEVEL, 
                 INITCAP(ACC_EDESC) AS ItemName,
                 ACC_CODE AS ItemCode,
                 MASTER_ACC_CODE AS MasterItemCode, 
                 PRE_ACC_CODE AS PreItemCode, 
                 ACC_TYPE_FLAG AS GroupFlag,
                (SELECT COUNT(*) FROM fa_chart_of_accounts_setup WHERE  
                COMPANY_CODE='{userinfo.company_code}' AND DELETED_FLAG='N' AND PRE_ACC_CODE = ims.MASTER_ACC_CODE) as Childrens 
                 FROM fa_chart_of_accounts_setup ims
                 WHERE ims.DELETED_FLAG = 'N' 
                 AND LEVEL=1
                 AND ims.COMPANY_CODE = '{userinfo.company_code}'
                 AND ACC_TYPE_FLAG = 'N'
                 START WITH PRE_ACC_CODE = '00'
                 CONNECT BY PRIOR MASTER_ACC_CODE = PRE_ACC_CODE AND COMPANY_CODE='{userinfo.company_code}'";
            var productListNodes = _dbContext.SqlQuery<COASetupModel>(query).ToList();
            return productListNodes;
        }
        public List<COASetupModel> GetCOAListByCOACode(string level, string masterProductCode, User userinfo)
        {
            //string query = string.Format(@"SELECT LEVEL, 
            //INITCAP(ITEM_EDESC) AS ItemName,
            //ITEM_CODE AS ItemCode,
            //MASTER_ITEM_CODE AS MasterItemCode, 
            //PRE_ITEM_CODE AS PreItemCode, 
            //GROUP_SKU_FLAG AS GroupFlag,
            //(SELECT COUNT(*) FROM IP_ITEM_MASTER_SETUP WHERE  
            // COMPANY_CODE='" + userinfo.company_code + @"' AND DELETED_FLAG='N' AND PRE_ITEM_CODE = ims.MASTER_ITEM_CODE) as Childrens 
            //FROM IP_ITEM_MASTER_SETUP ims
            //WHERE ims.DELETED_FLAG = 'N' 
            //AND ims.COMPANY_CODE = '" + userinfo.company_code + @"'
            //AND LEVEL = {0}
            //START WITH PRE_ITEM_CODE = '{1}'
            //CONNECT BY PRIOR MASTER_ITEM_CODE = PRE_ITEM_CODE
            //ORDER SIBLINGS BY ITEM_EDESC", level.ToString(), masterProductCode.ToString());
            string query = $@"SELECT LEVEL, 
                INITCAP(ACC_EDESC) AS ITEMNAME,
                ACC_CODE AS ITEMCODE,
                MASTER_ACC_CODE AS MASTERITEMCODE, 
                PRE_ACC_CODE AS PREITEMCODE, 
                ACC_TYPE_FLAG AS GroupFlag,
                (SELECT COUNT(*) FROM FA_CHART_OF_ACCOUNTS_SETUP WHERE  
                 COMPANY_CODE='{userinfo.company_code}' AND DELETED_FLAG='N' AND PRE_ACC_CODE = IMS.MASTER_ACC_CODE) AS CHILDRENS 
                FROM FA_CHART_OF_ACCOUNTS_SETUP IMS
                WHERE IMS.DELETED_FLAG = 'N' 
                AND IMS.COMPANY_CODE = '{userinfo.company_code}'
                AND LEVEL = {level.ToString()}
                START WITH PRE_ACC_CODE = '{masterProductCode.ToString()}'
                CONNECT BY PRIOR MASTER_ACC_CODE = PRE_ACC_CODE AND COMPANY_CODE='{userinfo.company_code}'
                ORDER SIBLINGS BY ACC_EDESC ";
            var productListNodes = _dbContext.SqlQuery<COASetupModel>(query).ToList();
            return productListNodes;
        }
        public List<COAPlanViewModal> getAllCOAPlans(ReportFiltersModel filters)
        {
            var userId = _workcontext.CurrentUserinformation.User_id;
            var company_code = _workcontext.CurrentUserinformation.company_code;
            string query = $@"SELECT DISTINCT PL.PLAN_CODE PLAN_CODE, PL.PLAN_EDESC, PL.PLAN_NDESC,
                             PL.SALES_AMOUNT SALES_AMOUNT,
                            PL.TIME_FRAME_CODE TIME_FRAME_CODE,
                            PL.CALENDAR_TYPE,PL.START_DATE START_DATE,PL.END_DATE END_DATE,
                            PL.REMARKS, PLD.DIVISION_CODE, PLD.BRANCH_CODE, DS.DIVISION_EDESC,
                            FS.BRANCH_EDESC
                            FROM PL_COA_PLAN PL, PL_COA_PLAN_DTL PLD, FA_DIVISION_SETUP DS, FA_BRANCH_SETUP FS
                             WHERE PL.DELETED_FLAG = 'N' 
                             AND PL.PLAN_CODE = PLD.PLAN_CODE
                             AND PL.COMPANY_CODE=PLD.COMPANY_CODE
                             AND DS.DIVISION_CODE(+) = PLD.DIVISION_CODE                             
                             AND FS.BRANCH_CODE (+)= PLD.BRANCH_CODE
                             AND PL.COMPANY_CODE ='{company_code}'
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

            List<COAPlanViewModal> spList = new List<COAPlanViewModal>();
            spList = this._dbContext.SqlQuery<COAPlanViewModal>(query).ToList();
            return spList;
        }
        //public List<COAPlanViewModal_Grid> getAllCOAPlans(ReportFiltersModel filters)
        //{
        //    var userId = _workcontext.CurrentUserinformation.User_id;
        //    string query = $@"SELECT DISTINCT TO_CHAR(PL.PLAN_CODE) PLAN_CODE, PL.PLAN_EDESC, PL.PLAN_NDESC,
        //                     TO_CHAR(PL.SALES_AMOUNT) SALES_AMOUNT,
        //                    TO_CHAR(PL.TIME_FRAME_CODE) TIME_FRAME_CODE,
        //                    PL.CALENDAR_TYPE,TO_CHAR(PL.START_DATE,'YYYY-MON-DD') START_DATE,TO_CHAR(PL.END_DATE,'YYYY-MM-DD') END_DATE,
        //                    PL.REMARKS, TO_CHAR(PLD.DIVISION_CODE) DIVISION_CODE, TO_CHAR(PLD.BRANCH_CODE) BRANCH_CODE, DS.DIVISION_EDESC,
        //                    FS.BRANCH_EDESC
        //                    FROM PL_COA_PLAN PL, PL_COA_PLAN_DTL PLD, FA_DIVISION_SETUP DS, FA_BRANCH_SETUP FS
        //                     WHERE PL.DELETED_FLAG = 'N' 
        //                     AND PL.PLAN_CODE = PLD.PLAN_CODE
        //                     AND DS.DIVISION_CODE(+) = PLD.DIVISION_CODE                             
        //                     AND FS.BRANCH_CODE (+)= PLD.BRANCH_CODE
        //                     AND PL.START_DATE BETWEEN TO_DATE('{filters.FromDate}','YYYY-MON-DD') AND TO_DATE('{filters.ToDate}', 'YYYY-MON-DD')
        //                     AND PL.END_DATE BETWEEN TO_DATE('{filters.FromDate}','YYYY-MON-DD') AND TO_DATE('{filters.ToDate}', 'YYYY-MON-DD')";

        //    if (filters.BranchFilter.Count > 0)
        //    {
        //        query += $@" AND PLD.BRANCH_CODE IN ('{string.Join("','", filters.BranchFilter)}')";

        //    }
        //    if (filters.DivisionFilter.Count > 0)
        //    {
        //        query += $@" AND PLD.DIVISION_CODE IN ('{string.Join("','", filters.DivisionFilter)}')";

        //    }
        //    query += $@" ORDER BY TO_NUMBER (PL.PLAN_CODE)  DESC";

        //    List<COAPlanViewModal_Grid> spList = new List<COAPlanViewModal_Grid>();
        //    spList = this._dbContext.SqlQuery<COAPlanViewModal_Grid>(query).ToList();
        //    return spList;
        //}
        public string SaveBudgetPlan(List<savePlan> sv, SalesPlan sp)
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

                // first insertion to pl_sales_plan table
                string checkPlanQuery = $@"SELECT COUNT(*) FROM pl_coa_plan WHERE PLAN_EDESC='{sp.PLAN_EDESC}' 
                AND TIME_FRAME_CODE='{sp.TIME_FRAME_CODE}' AND START_DATE=TO_DATE('{sp.START_DATE}','YYYY-Mon-DD') AND END_DATE=TO_DATE('{sp.END_DATE}','YYYY-Mon-DD')";
                var checkPlanQueryResult = this._dbContext.SqlQuery<int>(checkPlanQuery).First();
                if (checkPlanQueryResult == 0 && string.IsNullOrEmpty(sp.PLAN_CODE))
                {
                    //if (sp.PLAN_FOR.ToLower() == "quantity")
                    //{
                    //    sp.SALES_AMOUNT = string.Empty;
                    //}
                    //else if (sp.PLAN_FOR.ToLower() == "amount")
                    //{
                    //    sp.SALES_QUANTITY = string.Empty;
                    //}
                    string sales_plan_query = $@"INSERT INTO pl_coa_plan(PLAN_CODE,PLAN_EDESC,PLAN_NDESC,SALES_AMOUNT,TIME_FRAME_CODE,
                    CALENDAR_TYPE,START_DATE,END_DATE,REMARKS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,
                    LAST_MODIFIED_BY,LAST_MODIFIED_DATE,
                    DELETED_FLAG)
                    VALUES((SELECT COALESCE(MAX(PLAN_CODE)+1, MAX(PLAN_CODE) + 1, 1) FROM pl_coa_plan),'{sp.PLAN_EDESC}','','{sp.SALES_AMOUNT}','{sp.TIME_FRAME_CODE}','{sp.CALENDAR_TYPE}',
                    TO_DATE('{sp.START_DATE}','YYYY-Mon-DD'),TO_DATE('{sp.END_DATE}','YYYY-Mon-DD'),'{sp.REMARKS}','{companyCode}','{branchCode}','{userID}',
                    TO_DATE('{DateTime.Today.ToString("MM/dd/yyyy")}','MM/DD/YYYY'),'{userID}',TO_DATE('{DateTime.Today.ToString("MM/dd/yyyy")}','MM/DD/YYYY'),'N')";

                    var insertResult = this._dbContext.ExecuteSqlCommand(sales_plan_query);

                    string fetchSalesPlan = $@"SELECT PLAN_CODE FROM pl_coa_plan WHERE PLAN_EDESC='{sp.PLAN_EDESC}' 
                AND TIME_FRAME_CODE='{sp.TIME_FRAME_CODE}' AND START_DATE=TO_DATE('{sp.START_DATE}','YYYY-Mon-DD') AND END_DATE=TO_DATE('{sp.END_DATE}','YYYY-Mon-DD')";
                    planCode = this._dbContext.SqlQuery<int>(fetchSalesPlan).First();

                }
                else
                {
                    if (!string.IsNullOrEmpty(sp.PLAN_CODE) && sp.PLAN_CODE != "0")
                    {
                        planCode = Convert.ToInt32(sp.PLAN_CODE);
                        string updateQuery = $@"UPDATE pl_coa_plan SET PLAN_EDESC = '{sp.PLAN_EDESC}' , REMARKS = '{sp.REMARKS}' 
                        ,TIME_FRAME_CODE='{sp.TIME_FRAME_CODE}' , CALENDAR_TYPE='{sp.CALENDAR_TYPE}', START_DATE=TO_DATE('{sp.START_DATE}','YYYY-MON-DD') , END_DATE=TO_DATE('{sp.END_DATE}','YYYY-MON-DD')
                        WHERE PLAN_CODE='{sp.PLAN_CODE}'";
                        var update_result = this._dbContext.ExecuteSqlCommand(updateQuery);
                    }
                    else
                    {
                        string plancode_query = $@"SELECT PLAN_CODE FROM pl_coa_plan WHERE PLAN_EDESC='{sp.PLAN_EDESC}' 
                AND TIME_FRAME_CODE='{sp.TIME_FRAME_CODE}' AND START_DATE=TO_DATE('{sp.START_DATE}','YYYY-Mon-DD') AND END_DATE=TO_DATE('{sp.END_DATE}','YYYY-Mon-DD')";
                        planCode = this._dbContext.SqlQuery<int>(plancode_query).First();
                    }

                    string delete_detail_already_set = $@"DELETE FROM pl_coa_plan_dtl WHERE PLAN_CODE='{planCode}'";
                    this._dbContext.ExecuteSqlCommand(delete_detail_already_set);
                }

                string sbInsertQuery = string.Empty;
                //List<sa_sales_invoice_viewmodel> salesInvoice = new List<sa_sales_invoice_viewmodel>();
                //string averageQuery = $@"SELECT CALC_TOTAL_PRICE,CALC_QUANTITY, ITEM_CODE FROM SA_SALES_INVOICE WHERE DELETED_FLAG = 'N' AND COMPANY_CODE = '{company_code}' AND ITEM_CODE IN ({string.Join(",", sv.Select(a => a.itemCode))})";
                //salesInvoice = this._dbContext.SqlQuery<sa_sales_invoice_viewmodel>(averageQuery).ToList();

                //string itemQuery = $@"SELECT PURCHASE_PRICE AS CALC_TOTAL_PRICE,ITEM_CODE FROM IP_ITEM_MASTER_SETUP WHERE DELETED_FLAG = 'N' AND COMPANY_CODE = '{company_code}'  AND ITEM_CODE IN ({string.Join(",", sv.Select(a => a.itemCode))})";
                //var itemsFromItemMaster = this._dbContext.SqlQuery<sa_sales_invoice_viewmodel>(itemQuery).ToList();
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
                    //if (sp.salesRateType == "AVERAGE_SALES_PRICE")
                    //{
                    //    salesPriceResult = salesInvoice.Where(a => a.ITEM_CODE == item.itemCode).FirstOrDefault() != null ? (salesInvoice.Where(a => a.ITEM_CODE == item.itemCode).Sum(a => a.CALC_TOTAL_PRICE) / salesInvoice.Where(a => a.ITEM_CODE == item.itemCode).Sum(a => a.CALC_QUANTITY)).ToString() : null;
                    //    if (string.IsNullOrEmpty(salesPriceResult))
                    //    {
                    //        averagePriceQuery = $@"SELECT TO_CHAR(SALES_RATE) SALES_PRICE_RATE FROM IP_ITEM_RATE_APPLICAT_SETUP WHERE DELETED_FLAG = 'N' AND COMPANY_CODE = '{company_code}' AND ITEM_CODE = '{item.itemCode}' AND ROWNUM <=1 ORDER BY APP_DATE DESC ";
                    //        salesPriceResult = this._dbContext.SqlQuery<string>(averagePriceQuery).FirstOrDefault();
                    //        if (salesPriceResult == null)
                    //        {
                    //            averagePriceQuery = $@"SELECT TO_CHAR(STANDARD_RATE) SALES_PRICE_RATE FROM IP_ITEM_RATE_SCHEDULE_SETUP WHERE DELETED_FLAG = 'N' AND COMPANY_CODE = '{company_code}' AND ITEM_CODE = '{item.itemCode}' AND ROWNUM <= 1 ORDER BY EFFECTIVE_DATE DESC";
                    //            salesPriceResult = this._dbContext.SqlQuery<string>(averagePriceQuery).FirstOrDefault();
                    //            if (salesPriceResult == null)
                    //            {
                    //                salesPriceResult = itemsFromItemMaster.Where(a => a.ITEM_CODE == item.itemCode).FirstOrDefault() == null ? "1" : itemsFromItemMaster.Where(a => a.ITEM_CODE == item.itemCode).First().CALC_TOTAL_PRICE.ToString();
                    //            }
                    //        }
                    //    }
                    //}
                    //else if (sp.salesRateType == "STANDARD_SALES_PRICE")
                    //{
                    //    averagePriceQuery = $@"SELECT TO_CHAR(SALES_RATE) SALES_PRICE_RATE FROM IP_ITEM_RATE_APPLICAT_SETUP WHERE DELETED_FLAG = 'N' AND COMPANY_CODE = '{company_code}' AND ITEM_CODE = '{item.itemCode}' AND ROWNUM <=1 ORDER BY APP_DATE DESC ";
                    //    salesPriceResult = this._dbContext.SqlQuery<string>(averagePriceQuery).FirstOrDefault();
                    //    if (salesPriceResult == null)
                    //    {
                    //        averagePriceQuery = $@"SELECT TO_CHAR(STANDARD_RATE) SALES_PRICE_RATE FROM IP_ITEM_RATE_SCHEDULE_SETUP WHERE DELETED_FLAG = 'N' AND COMPANY_CODE = '{company_code}' AND ITEM_CODE = '{item.itemCode}' AND ROWNUM <= 1 ORDER BY EFFECTIVE_DATE DESC";
                    //        salesPriceResult = this._dbContext.SqlQuery<string>(averagePriceQuery).FirstOrDefault();

                    //        if (salesPriceResult == null)
                    //        {
                    //            salesPriceResult = itemsFromItemMaster.Where(a => a.ITEM_CODE == item.itemCode).FirstOrDefault() == null ? "1" : itemsFromItemMaster.Where(a => a.ITEM_CODE == item.itemCode).First().CALC_TOTAL_PRICE.ToString();
                    //        }
                    //    }
                    //}
                    //else if (sp.salesRateType == "LANDED_COST")
                    //{
                    //    salesPriceResult = itemsFromItemMaster.Where(a => a.ITEM_CODE == item.itemCode).FirstOrDefault() == null ? "1" : itemsFromItemMaster.Where(a => a.ITEM_CODE == item.itemCode).First().CALC_TOTAL_PRICE.ToString();
                    //}


                    //if (string.IsNullOrEmpty(salesPriceResult) || salesPriceResult == "0")
                    //{
                    salesPriceResult = "1";
                    //}
                    // end
                    foreach (var freq in item.frequency)
                    {
                        try
                        {
                            string[] freq_date = null;
                            int interval = 0;
                            string freValue = freq.fname.Split('_')[0]; // month or week
                            string freYear = freq.fname.Split('_')[1]; // year

                            string freYearVal = freYear+'-'+freValue;

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

                                //var eachday_quantity_value = 0M;
                                //var eachday_amount_value = 0M;
                                //if (sp.PLAN_FOR.ToLower() == "quantity")
                                //{
                                //    eachday_quantity_value = eachday_value;
                                //    eachday_amount_value = Math.Round(eachday_value * Convert.ToDecimal(salesPriceResult), 3);
                                //}
                                //else if (sp.PLAN_FOR.ToLower() == "amount")
                                //{
                                //    eachday_quantity_value = Math.Round(eachday_value / Convert.ToDecimal(salesPriceResult), 3);
                                //    eachday_amount_value = eachday_value;
                                //}
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
                                var sqlquery = $@"UPDATE pl_coa_plan_dtl SET TARGET_VALUE='{freq.fvalue}',LAST_MODIFIED_BY='{userID}', LAST_MODIFIED_DATE=TO_DATE('{DateTime.Now:MM/dd/yyyy/ HH:mm:ss}', 'mm/dd/yyyy hh24:mi:ss') where PLAN_CODE='{planCode}' AND ITEM_CODE='{item.itemCode}' AND TIME_FRAME_CODE='{time_frame_code}' AND TIME_FRAME_VALUE='{freq.fname}'";
                            }
                        }
                    }
                    if (itteration <= (sv.Count() - remainderCount))
                    {
                        while (itemCount == itemRange)
                        {
                            string query = @"insert into pl_coa_plan_dtl(PLAN_CODE,PLAN_DATE,PER_DAY_AMOUNT,ACC_CODE,
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
                            string query = @"insert into pl_coa_plan_dtl(PLAN_CODE,PLAN_DATE,PER_DAY_AMOUNT,ACC_CODE,
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
                        result = _dbContext.ExecuteSqlCommand(item.ToString()).ToString();

                    }
                }
                catch (Exception ex)
                {
                    if (!ex.Message.Contains("unique constraint"))
                        throw;
                }

                string update_amout_quantity = string.Empty;
                update_amout_quantity = $@"UPDATE pl_coa_plan PSP SET PSP.SALES_AMOUNT = (
                        SELECT ROUND(SUM(SPD.PER_DAY_AMOUNT),2) FROM pl_coa_plan_dtl SPD, fa_chart_of_accounts_setup IMS
                        WHERE 
                        IMS.ACC_TYPE_FLAG ='T'
                        AND SPD.COMPANY_CODE = IMS.COMPANY_CODE
                        AND SPD.ACC_CODE = IMS.ACC_CODE
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

        public COAPlanSetupDetailViewModal GetBudgetPlanDetailValueByPlanCode(int plancode)
        {
            COAPlanSetupDetailViewModal entity = new COAPlanSetupDetailViewModal();
            COAPlanViewModal coaPlan = new COAPlanViewModal();

            string queryPlan = $@"SELECT 
                PLAN_CODE PLAN_CODE, PLAN_EDESC,PLAN_NDESC,SALES_AMOUNT SALES_AMOUNT,TIME_FRAME_CODE TIME_FRAME_CODE,
                (SELECT TIME_FRAME_EDESC FROM PL_TIME_FRAME WHERE TIME_FRAME_CODE = PSP.TIME_FRAME_CODE) TIME_FRAME_EDESC,
                CALENDAR_TYPE,START_DATE START_DATE,END_DATE END_DATE,REMARKS,COMPANY_CODE,BRANCH_CODE
                FROM PL_COA_PLAN PSP
                WHERE PLAN_CODE= '{plancode}'";
            coaPlan = this._dbContext.SqlQuery<COAPlanViewModal>(queryPlan).FirstOrDefault();

            List<COAPlanDtlViewModal> coaPlanDtlList = new List<COAPlanDtlViewModal>();
            string queryCoaPlanDtlList = $@"SELECT PLAN_CODE PLAN_CODE,PLAN_DATE PLAN_DATE,PER_DAY_AMOUNT PER_DAY_AMOUNT,
                ACC_CODE,DIVISION_CODE,REMARKS,COMPANY_CODE,BRANCH_CODE,ACC_CODE,CREATED_BY,CREATED_DATE CREATED_DATE, FREQUENCY_JSON
                FROM PL_COA_PLAN_DTL WHERE PLAN_CODE = '{plancode}' AND DELETED_FLAG='N'";
            coaPlanDtlList = this._dbContext.SqlQuery<COAPlanDtlViewModal>(queryCoaPlanDtlList).ToList();

            List<COAPlanItems> coaPlanItem = new List<COAPlanItems>();
            string queryCoaPlanItem = $@"SELECT 
                DISTINCT TO_CHAR(PSPD.ACC_CODE) ITEM_CODE,INITCAP(IMS.ACC_EDESC) ITEM_EDESC,TO_CHAR(IMS.MASTER_ACC_CODE) MASTER_ITEM_CODE,TO_CHAR(IMS.PRE_ACC_CODE) PRE_ITEM_CODE,
                TO_CHAR(IMS.ACC_TYPE_FLAG) GROUP_SKU_FLAG
                FROM PL_COA_PLAN_DTL PSPD,FA_CHART_OF_ACCOUNTS_SETUP IMS WHERE PSPD.PLAN_CODE='{plancode}' AND PSPD.DELETED_FLAG='N' AND IMS.DELETED_FLAG='N' AND IMS.ACC_CODE=PSPD.ACC_CODE
                ORDER BY PRE_ITEM_CODE,MASTER_ITEM_CODE,ITEM_CODE";

            coaPlanItem = this._dbContext.SqlQuery<COAPlanItems>(queryCoaPlanItem).ToList();

            entity.CoaPlan = coaPlan;
            entity.CoaPlanDtlList = coaPlanDtlList;
            entity.CoaPlanItems = coaPlanItem;

            return entity;
        }

        public bool deleteSalesPlan(int planCode)
        {
            try
            {
                string deleteYes_salesPlan = $@"UPDATE PL_COA_PLAN SET DELETED_FLAG='Y' WHERE PLAN_CODE='{planCode}'";
                string deleteYes_salesPlanDtl = $@"UPDATE PL_COA_PLAN_DTL SET DELETED_FLAG='Y' WHERE PLAN_CODE='{planCode}'";
                this._dbContext.ExecuteSqlCommand(deleteYes_salesPlanDtl);
                this._dbContext.ExecuteSqlCommand(deleteYes_salesPlan);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<ProductTree> ChartOfAccountList()
        {
            List<ProductTree> list = new List<ProductTree>();
            string query = $@"SELECT ACC_CODE ITEMCODE, ACC_EDESC ITEMNAME  FROM FA_CHART_OF_ACCOUNTS_SETUP WHERE DELETED_FLAG='N'";
            list = this._dbContext.SqlQuery<ProductTree>(query).ToList();
            return list;
        }

        public bool cloneSalesPlan(string planCode, string planName, string branchs, string divisions, string remarks)
        {
            try
            {
                if (!string.IsNullOrEmpty(remarks))
                {
                    remarks = remarks.Replace("\"", " ");
                }
                string existingPlanNameQuery = $@"SELECT PLAN_EDESC FROM PL_COA_PLAN WHERE PLAN_CODE='{planCode}'";
                string existingPlanName = this._dbContext.SqlQuery<String>(existingPlanNameQuery).FirstOrDefault();
                if (existingPlanName != null)
                {
                    string copyPlanName = existingPlanName + "_copy";
                    if (!string.IsNullOrEmpty(planName))
                        copyPlanName = planName;
                    string maxPlanCodeQuery = "SELECT COALESCE(MAX(PLAN_CODE)+1, MAX(PLAN_CODE) + 1, 1) FROM PL_COA_PLAN";
                    int maxPlanCode = this._dbContext.SqlQuery<int>(maxPlanCodeQuery).First();

                    string copySalesPlanQuery = $@"INSERT INTO PL_COA_PLAN(PLAN_CODE,PLAN_EDESC,PLAN_NDESC,SALES_AMOUNT,TIME_FRAME_CODE,
                    CALENDAR_TYPE,START_DATE,END_DATE,REMARKS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,APPROVED_FLAG,
                    DELETED_FLAG)
                    SELECT '{maxPlanCode}','{copyPlanName}', '{copyPlanName}',SALES_AMOUNT,TIME_FRAME_CODE,
                    CALENDAR_TYPE,START_DATE,END_DATE,'{remarks}',COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,APPROVED_FLAG,
                    DELETED_FLAG FROM PL_COA_PLAN WHERE PLAN_CODE='{planCode}'";
                    var insertCopiedPlan = this._dbContext.ExecuteSqlCommand(copySalesPlanQuery);

                    int copiedPlanCode = maxPlanCode;
                    if (copiedPlanCode > 0)
                    {
                        string copyinto_plandtl = $@"INSERT INTO PL_COA_PLAN_DTL
                                (PLAN_CODE,PLAN_DATE,PER_DAY_AMOUNT,ACC_CODE,
                                DIVISION_CODE,REMARKS,
                                COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,APPROVED_FLAG,DELETED_FLAG,FREQUENCY_JSON)
                                SELECT '{copiedPlanCode}', PLAN_DATE,PER_DAY_AMOUNT,ACC_CODE,
                                '{divisions}',REMARKS,
                                COMPANY_CODE,'{branchs}',CREATED_BY,CREATED_DATE,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,APPROVED_FLAG,DELETED_FLAG,FREQUENCY_JSON FROM PL_COA_PLAN_DTL WHERE PLAN_CODE='{planCode}'";

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

        public List<PlanCOARefrenceModel> getDataForRefrence(string itemList, string startDate, string endDate, string divisionCode, string branchCode, string dateFormat, string frequency)
        {
            var userNo = _workcontext.CurrentUserinformation.User_id ;
            var result = new List<PlanCOARefrenceModel>();
            try
            {
                var itemListArr = itemList.Split(',');
                itemList = string.Join("','", itemListArr);
                var refrenceQuery = string.Empty;
                string optionalCondition = string.Empty;
                //if (!string.IsNullOrEmpty(customerCode))
                //{
                //    optionalCondition += $@"  AND SI.CUSTOMER_CODE = '{customerCode}'";
                //}
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
                        //refrenceQuery = $@" SELECT  'freqItemNum_'||SI.ITEM_CODE ||'_WEEK_' || NEPALI_WEEKS.WEEKS ||'_'||  TO_CHAR(SUBSTR(BS_DATE(SI.SALES_DATE),0,4) )  COLNAME, SI.ITEM_CODE,
                        //                 IMS.ITEM_EDESC,
                        //                 SI.CUSTOMER_CODE,
                        //                 SI.DIVISION_CODE,
                        //                 SI.BRANCH_CODE,
                        //                NEPALI_WEEKS.WEEKS,
                        //                 TO_CHAR(SUM (SI.CALC_TOTAL_PRICE)) AMOUNT,
                        //                TO_CHAR(SUM (SI.CALC_QUANTITY)) QTY
                        //            FROM SA_SALES_INVOICE SI, IP_ITEM_MASTER_SETUP IMS, (SELECT DAYS, ROWNUM  DAY_NO , TRUNC((ROWNUM+7)/7)  WEEKS
                        //          FROM (SELECT FISCAL_YEAR.START_DATE + ROWNUM - 1 DAYS
                        //                  FROM ALL_OBJECTS,
                        //                       (SELECT START_DATE, END_DATE
                        //                          FROM HR_FISCAL_YEAR_CODE
                        //                         WHERE SYSDATE BETWEEN START_DATE AND END_DATE) FISCAL_YEAR
                        //                 WHERE ROWNUM <= FISCAL_YEAR.END_DATE - FISCAL_YEAR.START_DATE + 1) )NEPALI_WEEKS
                        //        WHERE IMS.ITEM_CODE IN ({itemList}) {optionalCondition} 
                        //        AND SI.ITEM_CODE = IMS.ITEM_CODE
                        //            AND NEPALI_WEEKS.DAYS = SI.SALES_DATE
                        //            AND SI.SALES_DATE BETWEEN to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD')
                        //        GROUP BY SI.ITEM_CODE,
                        //                 IMS.ITEM_EDESC,
                        //                 SI.CUSTOMER_CODE,
                        //                 SI.DIVISION_CODE,
                        //                 SI.BRANCH_CODE,
                        //                NEPALI_WEEKS.WEEKS
                        //                ,SUBSTR(BS_DATE(SI.SALES_DATE),0,4)  
                        //         ORDER BY   NEPALI_WEEKS.WEEKS";
                        refrenceQuery = $@" SELECT  'freqItemNum_'||SI.ACC_CODE ||'_WEEK_' || NEPALI_WEEKS.WEEKS ||'_'||  TO_CHAR(SUBSTR(BS_DATE(SI.VOUCHER_DATE),0,4) )  COLNAME, SI.ACC_CODE,
                                         IMS.ACC_EDESC,
                                         SI.DIVISION_CODE,
                                         SI.BRANCH_CODE,
                                        NEPALI_WEEKS.WEEKS,
                                        CASE IMS.TRANSACTION_TYPE
                                        WHEN 'DR'
                                            THEN TO_CHAR(SUM (SI.DR_AMOUNT),'FM999999999999.00')
                                        ELSE
                                            TO_CHAR(SUM (SI.CR_AMOUNT),'FM999999999999.00')
                                        END
                                        AS AMOUNT
                                    FROM V$VIRTUAL_GENERAL_LEDGER SI, fa_chart_of_accounts_setup IMS, (SELECT DAYS, ROWNUM  DAY_NO , TRUNC((ROWNUM+7)/7)  WEEKS
                                  FROM (SELECT FISCAL_YEAR.START_DATE + ROWNUM - 1 DAYS
                                          FROM ALL_OBJECTS,
                                               (SELECT START_DATE, END_DATE
                                                  FROM HR_FISCAL_YEAR_CODE
                                                 WHERE SYSDATE BETWEEN START_DATE AND END_DATE) FISCAL_YEAR
                                         WHERE ROWNUM <= FISCAL_YEAR.END_DATE - FISCAL_YEAR.START_DATE + 1) )NEPALI_WEEKS
                                WHERE IMS.ACC_CODE IN ('{itemList}')   {optionalCondition}
                                AND SI.ACC_CODE = IMS.ACC_CODE
                                    AND NEPALI_WEEKS.DAYS = SI.VOUCHER_DATE
                                    AND SI.VOUCHER_DATE BETWEEN to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD')
                                GROUP BY SI.ACC_CODE,
                                         IMS.ACC_EDESC,
                                         SI.DIVISION_CODE,
                                         SI.BRANCH_CODE,
                                        IMS.TRANSACTION_TYPE,
                                        NEPALI_WEEKS.WEEKS
                                        ,SUBSTR(BS_DATE(SI.VOUCHER_DATE),0,4)  
                                 ORDER BY   NEPALI_WEEKS.WEEKS";
                    }
                    else if (frequency.ToLower() == "month")
                    {
                        //refrenceQuery = $@"SELECT 'freqItemNum_'||SI.ITEM_CODE ||'_' ||  UPPER(fn_bs_month(SUBSTR(BS_DATE(SI.SALES_DATE),6,2))) ||'_'|| SUBSTR(BS_DATE(SI.SALES_DATE),0,4) COLNAME ,
                        //             IMS.ITEM_EDESC,
                        //             SI.CUSTOMER_CODE,
                        //             SI.DIVISION_CODE,
                        //             SI.BRANCH_CODE,
                        //            -- SUBSTR(TO_BS(SI.SALES_DATE),7,7) NEPALI_MONTH,
                        //             TO_CHAR(SUM (SI.CALC_TOTAL_PRICE)) AMOUNT,
                        //             TO_CHAR(SUM (SI.CALC_QUANTITY)) QTY
                        //        FROM SA_SALES_INVOICE SI, IP_ITEM_MASTER_SETUP IMS
                        //        WHERE IMS.ITEM_CODE IN ({itemList}) {optionalCondition} 
                        //        AND SI.ITEM_CODE = IMS.ITEM_CODE
                        //        AND SI.SALES_DATE BETWEEN to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD')
                        //    GROUP BY SI.ITEM_CODE,
                        //             IMS.ITEM_EDESC,
                        //             SI.CUSTOMER_CODE,
                        //             SI.DIVISION_CODE,
                        //             SI.BRANCH_CODE,
                        //             SUBSTR(BS_DATE(SI.SALES_DATE),0,4),
                        //             fn_bs_month(SUBSTR(BS_DATE(SI.SALES_DATE),6,2))";
                        refrenceQuery = $@"SELECT 'freqItemNum_'||SI.ACC_CODE ||'_' ||  UPPER(fn_bs_month(SUBSTR(BS_DATE(SI.VOUCHER_DATE),6,2))) ||'_'|| SUBSTR(BS_DATE(SI.VOUCHER_DATE),0,4) COLNAME ,
                                     IMS.ACC_EDESC ITEM_EDESC,
                                     SI.DIVISION_CODE,
                                     SI.BRANCH_CODE,
                                      CASE IMS.TRANSACTION_TYPE
                                        WHEN 'DR'
                                            THEN TO_CHAR(SUM (SI.DR_AMOUNT),'FM999999999999.00')
                                        ELSE
                                            TO_CHAR(SUM (SI.CR_AMOUNT),'FM999999999999.00')
                                        END
                                        AS AMOUNT
                                FROM V$VIRTUAL_GENERAL_LEDGER SI, fa_chart_of_accounts_setup IMS
                                WHERE IMS.ACC_CODE IN ('{itemList}') {optionalCondition} 
                                AND SI.ACC_CODE = IMS.ACC_CODE
                                AND SI.VOUCHER_DATE BETWEEN to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD')
                            GROUP BY SI.ACC_CODE,
                                     IMS.ACC_EDESC,
                                     SI.DIVISION_CODE,
                                     SI.BRANCH_CODE,
                                    IMS.TRANSACTION_TYPE,
                                     SUBSTR(BS_DATE(SI.VOUCHER_DATE),0,4),
                                     fn_bs_month(SUBSTR(BS_DATE(SI.VOUCHER_DATE),6,2))";
                    }
                }
                else if (dateFormat.ToUpper() == "AD" || dateFormat.ToUpper() == "ENG")
                {
                    if (frequency.ToLower() == "week")
                    {
                        //   refrenceQuery = $@"SELECT 'freqItemNum_'||SI.ITEM_CODE ||'_WEEK_' ||  TO_CHAR(SI.SALES_DATE,'IW') ||'_'||  TO_CHAR(SI.SALES_DATE,'YYYY') COLNAME, SI.ITEM_CODE,
                        //                  IMS.ITEM_EDESC,
                        //                SI.CUSTOMER_CODE,
                        //                SI.DIVISION_CODE,
                        //                SI.BRANCH_CODE,
                        //                TO_CHAR(SI.SALES_DATE,'YYYY') AD_YEAR,
                        //                TO_CHAR(SI.SALES_DATE,'IW') SALES_WEEK,
                        //                TO_CHAR(SUM (SI.CALC_TOTAL_PRICE)) AMOUNT,
                        //                TO_CHAR(SUM (SI.CALC_QUANTITY)) QTY
                        //           FROM SA_SALES_INVOICE SI, IP_ITEM_MASTER_SETUP IMS
                        //           WHERE IMS.ITEM_CODE IN ({itemList}) {optionalCondition} 
                        //           AND SI.ITEM_CODE = IMS.ITEM_CODE
                        //           AND SI.SALES_DATE BETWEEN to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD')
                        //      GROUP BY SI.ITEM_CODE,
                        //        IMS.ITEM_EDESC,
                        //        SI.CUSTOMER_CODE,
                        //        SI.DIVISION_CODE,
                        //        SI.BRANCH_CODE,
                        //        TO_CHAR(SI.SALES_DATE,'YYYY'),
                        //        TO_CHAR(SI.SALES_DATE,'IW')
                        //ORDER BY  TO_CHAR(SI.SALES_DATE,'YYYY'), TO_CHAR(SI.SALES_DATE,'IW')";
                        refrenceQuery = $@"SELECT 'freqItemNum_'||SI.ACC_CODE ||'_WEEK_' ||  TO_CHAR(SI.VOUCHER_DATE,'IW') ||'_'||  TO_CHAR(SI.VOUCHER_DATE,'YYYY') COLNAME, SI.ACC_CODE,
                                       IMS.ACC_EDESC,
                                     SI.DIVISION_CODE,
                                     SI.BRANCH_CODE,
                                     TO_CHAR(SI.VOUCHER_DATE,'YYYY') AD_YEAR,
                                     TO_CHAR(SI.VOUCHER_DATE,'IW') SALES_WEEK,
                                     CASE IMS.TRANSACTION_TYPE
                                        WHEN 'DR'
                                            THEN TO_CHAR(SUM (SI.DR_AMOUNT),'FM999999999999.00')
                                        ELSE
                                            TO_CHAR(SUM (SI.CR_AMOUNT),'FM999999999999.00')
                                        END
                                        AS AMOUNT
                                FROM V$VIRTUAL_GENERAL_LEDGER SI, fa_chart_of_accounts_setup IMS
                                WHERE IMS.ACC_CODE IN ('{itemList}') {optionalCondition} 
                                AND SI.ACC_CODE = IMS.ACC_CODE
                                AND SI.VOUCHER_DATE BETWEEN to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD')
                           GROUP BY SI.ACC_CODE,
                             IMS.ACC_EDESC,
                             SI.DIVISION_CODE,
                             SI.BRANCH_CODE,
                             IMS.TRANSACTION_TYPE,
                             TO_CHAR(SI.VOUCHER_DATE,'YYYY'),
                             TO_CHAR(SI.VOUCHER_DATE,'IW')
                     ORDER BY  TO_CHAR(SI.VOUCHER_DATE,'YYYY'), TO_CHAR(SI.VOUCHER_DATE,'IW')";
                    }
                    else if (frequency.ToLower() == "month")
                    {
                        //refrenceQuery = $@"SELECT  'freqItemNum_'||SI.ITEM_CODE ||'_' ||  TO_CHAR(SI.SALES_DATE,'MON') ||'_'||  TO_CHAR(SI.SALES_DATE,'YYYY') COLNAME ,
                        //             IMS.ITEM_EDESC,
                        //             SI.CUSTOMER_CODE,
                        //             SI.DIVISION_CODE,
                        //             SI.BRANCH_CODE,
                        //            -- SUBSTR(TO_BS(SI.SALES_DATE),7,7) NEPALI_MONTH,
                        //             TO_CHAR(SUM (SI.CALC_TOTAL_PRICE)) AMOUNT,
                        //             TO_CHAR(SUM (SI.CALC_QUANTITY)) QTY
                        //        FROM SA_SALES_INVOICE SI, IP_ITEM_MASTER_SETUP IMS
                        //        WHERE IMS.ITEM_CODE IN ({itemList}) {optionalCondition} 
                        //        AND SI.ITEM_CODE = IMS.ITEM_CODE
                        //        AND SI.SALES_DATE BETWEEN to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD')
                        //    GROUP BY SI.ITEM_CODE,
                        //             IMS.ITEM_EDESC,
                        //             SI.CUSTOMER_CODE,
                        //             SI.DIVISION_CODE,
                        //             SI.BRANCH_CODE,
                        //             TO_CHAR(SI.SALES_DATE,'YYYY'),
                        //             TO_CHAR(SI.SALES_DATE,'MON')";
                        refrenceQuery = $@"SELECT  'freqItemNum_'||SI.ACC_CODE ||'_' ||  TO_CHAR(SI.VOUCHER_DATE,'MON') ||'_'||  TO_CHAR(SI.VOUCHER_DATE,'YYYY') COLNAME ,
                                     IMS.ACC_EDESC,
                                     SI.DIVISION_CODE,
                                     SI.BRANCH_CODE,
                                     CASE IMS.TRANSACTION_TYPE
                                        WHEN 'DR'
                                            THEN TO_CHAR(SUM (SI.DR_AMOUNT),'FM999999999999.00')
                                        ELSE
                                            TO_CHAR(SUM (SI.CR_AMOUNT),'FM999999999999.00')
                                        END
                                        AS AMOUNT
                                FROM V$VIRTUAL_GENERAL_LEDGER SI, fa_chart_of_accounts_setup IMS
                                WHERE IMS.ACC_CODE IN ('{itemList}') {optionalCondition}
                                AND SI.ACC_CODE = IMS.ACC_CODE
                                AND SI.VOUCHER_DATE BETWEEN to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD')
                            GROUP BY SI.ACC_CODE,
                                     IMS.ACC_EDESC,
                                     SI.DIVISION_CODE,
                                     SI.BRANCH_CODE,
                                    IMS.TRANSACTION_TYPE,
                                     TO_CHAR(SI.VOUCHER_DATE,'YYYY'),
                                     TO_CHAR(SI.VOUCHER_DATE,'MON')";
                    }
                }

                result = this._dbContext.SqlQuery<PlanCOARefrenceModel>(refrenceQuery).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public string CreateTemBudgetPlanReportTable(ReportFiltersModel model)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var response = "";
            // var createTableQry = $@"CREATE TABLE TEMP_PL_B_PLAN_REPORT AS SELECT DS.DIVISION_CODE,
            // DS.DIVISION_EDESC, FS.BRANCH_CODE, FS.BRANCH_EDESC, CAS.ACC_CODE ITEM_CODE, CAS.ACC_EDESC ITEM_EDESC, PLD.COMPANY_CODE,
            // TO_CHAR (fn_bs_month (SUBSTR (bs_date (PLD.PLAN_DATE), 6, 2))) MONTH,
            // TO_CHAR (SUBSTR (bs_date (PLD.PLAN_DATE), 0, 4)) AS YEAR,
            // SUM (PLD.PER_DAY_AMOUNT) PER_DAY_AMOUNT
            // FROM PL_COA_SUB_PLAN_DTL PLD, FA_CHART_OF_ACCOUNTS_SETUP CAS, FA_DIVISION_SETUP DS, FA_BRANCH_SETUP FS
            // WHERE PLD.DELETED_FLAG = 'N'
            // AND PLD.ACC_CODE = CAS.ACC_CODE  AND DS.DIVISION_CODE(+) = PLD.DIVISION_CODE    
            // AND PLD.COMPANY_CODE = CAS.COMPANY_CODE AND CAS.ACC_TYPE_FLAG = 'T' AND PLD.COMPANY_CODE = '01'
            //GROUP BY DS.DIVISION_CODE, DS.DIVISION_EDESC, FS.BRANCH_CODE, FS.BRANCH_EDESC, CAS.ACC_CODE, CAS.ACC_EDESC,
            // PLD.COMPANY_CODE, TO_CHAR (fn_bs_month (SUBSTR (bs_date (PLD.PLAN_DATE), 6, 2))), TO_CHAR (SUBSTR (bs_date (PLD.PLAN_DATE), 0, 4))";
            var createTableQry = $@"CREATE TABLE TEMP_PL_B_PLAN_REPORT AS
                                    SELECT CAS.ACC_CODE ITEM_CODE, CAS.ACC_EDESC ITEM_EDESC, PLD.COMPANY_CODE,
                                        DS.DIVISION_CODE, DS.DIVISION_EDESC, BCS.BUDGET_CODE,BCS.BUDGET_EDESC,
                                        TO_CHAR (fn_bs_month (SUBSTR (bs_date (PLD.PLAN_DATE), 6, 2))) MONTH,
                                        SUBSTR(BS_DATE(PLD.PLAN_DATE), 6, 2) MONTHINT,
                                        TO_CHAR (SUBSTR (bs_date (PLD.PLAN_DATE), 0, 4)) AS YEAR,
                                        SUM (PLD.PER_DAY_AMOUNT) PER_DAY_AMOUNT
                                       FROM PL_COA_SUB_PLAN_DTL PLD
                                            INNER JOIN FA_CHART_OF_ACCOUNTS_SETUP CAS ON CAS.ACC_CODE = PLD.ACC_CODE AND CAS.COMPANY_CODE = PLD.COMPANY_CODE
                                            INNER JOIN BC_BUDGET_CENTER_SETUP BCS ON BCS.BUDGET_CODE = PLD.BUDGET_CODE AND BCS.COMPANY_CODE = PLD.COMPANY_CODE
                                            LEFT JOIN  FA_DIVISION_SETUP DS ON PLD.DIVISION_CODE =DS.DIVISION_CODE AND PLD.COMPANY_CODE=DS.COMPANY_CODE
                                        WHERE   PLD.DELETED_FLAG = 'N' AND CAS.ACC_TYPE_FLAG = 'T'  AND BCS.GROUP_SKU_FLAG='I' 
                                       GROUP BY DS.DIVISION_CODE, DS.DIVISION_EDESC, 
                                                     CAS.ACC_CODE, CAS.ACC_EDESC,PLD.COMPANY_CODE,BCS.BUDGET_CODE,BCS.BUDGET_EDESC,
                                                     TO_CHAR (fn_bs_month (SUBSTR (bs_date (PLD.PLAN_DATE), 6, 2))),
                                                     SUBSTR(BS_DATE(PLD.PLAN_DATE), 6, 2),
                                                      TO_CHAR (SUBSTR (bs_date (PLD.PLAN_DATE), 0, 4))";
            try
            {
                var tableExistsQry = $@"SELECT * FROM TEMP_PL_B_PLAN_REPORT";
                var result = this._dbContext.SqlQuery<PlanReportModel>(tableExistsQry).ToList();
                if (result != null)
                {
                    var dropQry = $@"DROP TABLE TEMP_PL_B_PLAN_REPORT";
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
                    var dropQry = $@"DROP TABLE TEMP_PL_B_PLAN_REPORT";
                    var dropResponse = this._dbContext.ExecuteSqlCommand(dropQry);
                    response = ex.Message;
                }
            }
            return response;
        }

    }
}
