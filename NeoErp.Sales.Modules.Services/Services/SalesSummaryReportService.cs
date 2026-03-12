using NeoErp.Core.Domain;
using NeoErp.Core.Helpers;
using NeoErp.Core.Models;
using NeoErp.Core.Services;
using NeoErp.Sales.Modules.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Services
{
    public class SalesSummaryReportService : ISalesSummaryReportService
    {
        private NeoErpCoreEntity _objectEntity;
        private IControlService _controlService;

        public SalesSummaryReportService(NeoErpCoreEntity _objectEntity, IControlService _controlService)
        {
            this._objectEntity = _objectEntity;
            this._controlService = _controlService;
        }


        public List<SalesSummaryReportModel> GetPartyWiseGPAnalysisSalesSummaryReport(ReportFiltersModel reportFilters, User userInfo)
        {
            var companyCode = string.Join(",", reportFilters.CompanyFilter);
            //companyCode = companyCode == (null||"" )? userInfo.company_code : companyCode;
            if (string.IsNullOrEmpty(companyCode))
            {
                companyCode = userInfo.company_code;
            }
            var list = new List<SalesSummaryReportModel>();
            try
            {
                string query = @"SELECT 
 ITEM_CODE
	,ITEM_EDESC
	,UNIT
	,QUANTITY
	,UNIT_COST
	,RATE
	,SALES_AMOUNT
	,LANDED_COST
	,ROUND((NVL(RATE,0)-NVL(UNIT_COST,0))*NVL(QUANTITY,0),2) GROSS_PROFIT
	, ROUND(((NVL(RATE,0)-NVL(UNIT_COST,0))/NVL(UNIT_COST,1))*100,2) GROSS_PROFIT_PERCENTAGE FROM(SELECT ITEM_CODE, ITEM_EDESC, UNIT, QUANTITY ,FN_UNIT_COST(ITEM_CODE,NULL,COMPANY_CODE,BRANCH_CODE,TO_DATE('{2}','YYYY-MM-DD'),TO_DATE('{3}','YYYY-MM-DD')) UNIT_COST, RATE, SALES_AMOUNT, LANDED_COST, GROSS_PROFIT,
                            CASE NVL(LANDED_COST, 0) WHEN  0 THEN 0
                            ELSE     
                                    ROUND(GROSS_PROFIT/LANDED_COST*100,2)
                            END GROSS_PROFIT_PERCENTAGE                               
                                FROM 
                            (SELECT a.ITEM_CODE, a.MU_CODE as UNIT
                            ,A.COMPANY_CODE
    ,A.BRANCH_CODE  
                            ,SUM(NVL(a.QUANTITY,0)) as QUANTITY,
                            ROUND(NVL(SUM(NVL(a.calc_total_price,0))/SUM(NVL(a.QUANTITY,0)) ,0)/{4},{5})  as UNIT_COST,                                                 
                            ROUND(SUM(NVL(FN_CONVERT_CURRENCY(NVL(a.TOTAL_PRICE,0) * NVL(a.EXCHANGE_RATE,1),'NRS', a.SALES_DATE),0))/DECODE(SUM(NVL(a.QUANTITY,1)),0,1,SUM(NVL(a.QUANTITY,1)))/{4},{5}) as RATE,                                                 
                            ROUND(SUM(NVL(FN_CONVERT_CURRENCY(NVL(a.TOTAL_PRICE,0) * NVL(a.EXCHANGE_RATE,1),'NRS', a.SALES_DATE),0))/{4},{5}) as SALES_AMOUNT,
                            b.ITEM_EDESC,
                            SUM((SELECT SUM(NVL(a.QUANTITY,0) * NVL(D.LANDED_COST,0)) FROM MP_ITEM_STD_RATE D 
                            WHERE d.ITEM_CODE = a.ITEM_CODE 
                            AND d.COMPANY_CODE = A.COMPANY_CODE 
                            AND D.STD_DATE IN(SELECT MAX(STD_DATE) FROM MP_ITEM_STD_RATE WHERE ITEM_CODE = D.ITEM_CODE))) LANDED_COST,
                            ROUND(SUM(NVL(FN_CONVERT_CURRENCY(NVL(a.TOTAL_PRICE,0) * NVL(a.EXCHANGE_RATE,1),'NRS', a.SALES_DATE),0))/{4},{5}) - SUM((SELECT SUM(NVL(a.QUANTITY,0) * NVL(D.LANDED_COST,0)) FROM MP_ITEM_STD_RATE D 
                            WHERE d.ITEM_CODE = a.ITEM_CODE 
                            AND d.COMPANY_CODE = A.COMPANY_CODE 
                            AND D.STD_DATE IN(SELECT MAX(STD_DATE) FROM MP_ITEM_STD_RATE WHERE ITEM_CODE = D.ITEM_CODE))) GROSS_PROFIT
                            FROM SA_SALES_INVOICE a, IP_ITEM_MASTER_SETUP b  
                            WHERE a.ITEM_CODE = b.ITEM_CODE                                                 
                            AND a.COMPANY_CODE = b.COMPANY_CODE 
                            AND a.DELETED_FLAG='N'                                                 
                            AND a.COMPANY_CODE IN({0})                                          
                            AND a.SALES_DATE >= TO_DATE('{2}','YYYY-MM-DD')                                                 
                            AND a.SALES_DATE <= TO_DATE('{3}','YYYY-MM-DD') ";
                //string query = @"SELECT ITEM_CODE, ITEM_EDESC, UNIT, QUANTITY, UNIT_COST, RATE, SALES_AMOUNT, LANDED_COST, GROSS_PROFIT,
                //            CASE NVL(LANDED_COST, 0) WHEN  0 THEN 0
                //            ELSE     
                //                    ROUND(GROSS_PROFIT/LANDED_COST*100,2)
                //            END GROSS_PROFIT_PERCENTAGE                               
                //                FROM 
                //            (SELECT a.ITEM_CODE, a.MU_CODE as UNIT,
                //            SUM(NVL(a.QUANTITY,0)) as QUANTITY,
                //            ROUND(NVL(SUM(NVL(a.calc_total_price,0))/SUM(NVL(a.QUANTITY,0)) ,0)/{4},{5})  as UNIT_COST,                                                 
                //            ROUND(SUM(NVL(FN_CONVERT_CURRENCY(NVL(a.TOTAL_PRICE,0) * NVL(a.EXCHANGE_RATE,1),'NRS', a.SALES_DATE),0))/DECODE(SUM(NVL(a.QUANTITY,1)),0,1,SUM(NVL(a.QUANTITY,1)))/{4},{5}) as RATE,                                                 
                //            ROUND(SUM(NVL(FN_CONVERT_CURRENCY(NVL(a.TOTAL_PRICE,0) * NVL(a.EXCHANGE_RATE,1),'NRS', a.SALES_DATE),0))/{4},{5}) as SALES_AMOUNT,
                //            b.ITEM_EDESC,
                //            SUM((SELECT SUM(NVL(a.QUANTITY,0) * NVL(D.LANDED_COST,0)) FROM MP_ITEM_STD_RATE D 
                //            WHERE d.ITEM_CODE = a.ITEM_CODE 
                //            AND d.COMPANY_CODE = A.COMPANY_CODE 
                //            AND D.STD_DATE IN(SELECT MAX(STD_DATE) FROM MP_ITEM_STD_RATE WHERE ITEM_CODE = D.ITEM_CODE))) LANDED_COST,
                //            ROUND(SUM(NVL(FN_CONVERT_CURRENCY(NVL(a.TOTAL_PRICE,0) * NVL(a.EXCHANGE_RATE,1),'NRS', a.SALES_DATE),0))/{4},{5}) - SUM((SELECT SUM(NVL(a.QUANTITY,0) * NVL(D.LANDED_COST,0)) FROM MP_ITEM_STD_RATE D 
                //            WHERE d.ITEM_CODE = a.ITEM_CODE 
                //            AND d.COMPANY_CODE = A.COMPANY_CODE 
                //            AND D.STD_DATE IN(SELECT MAX(STD_DATE) FROM MP_ITEM_STD_RATE WHERE ITEM_CODE = D.ITEM_CODE))) GROSS_PROFIT
                //            FROM SA_SALES_INVOICE a, IP_ITEM_MASTER_SETUP b  
                //            WHERE a.ITEM_CODE = b.ITEM_CODE                                                 
                //            AND a.COMPANY_CODE = b.COMPANY_CODE 
                //            AND a.DELETED_FLAG='N'                                                 
                //            AND a.COMPANY_CODE IN({0})                                          
                //            AND a.SALES_DATE >= TO_DATE('{2}','YYYY-MM-DD')                                                 
                //            AND a.SALES_DATE <= TO_DATE('{3}','YYYY-MM-DD') ";

                ////for Item Filter
                if (reportFilters.ProductFilter.Count() > 0)
                {
                    query += " and (";
                    //IF PRODUCT_SKU_FLAG = G
                    foreach (var item in reportFilters.ProductFilter)
                    {
                        query += "b.MASTER_ITEM_CODE like  (Select DISTINCT(MASTER_ITEM_CODE) || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND COMPANY_CODE IN(" + companyCode + ") ) OR ";
                    }
                    //IF PRODUCT_SKU_FLAG = I                
                    query += "(a.ITEM_CODE IN (" + string.Join(",", reportFilters.ProductFilter) + ") AND b.GROUP_SKU_FLAG = 'I')) ";

                    query = query.Substring(0, query.Length - 1);
                }
                //for customer Filter
                if (reportFilters.CustomerFilter.Count() > 0)
                {
                    query += " and (";
                    foreach (var item in reportFilters.CustomerFilter)
                    {
                        query += "c.master_customer_code like  (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND COMPANY_CODE IN(" + companyCode + ") ) OR ";
                    }
                    query = query.Substring(0, query.Length - 3) + ") ";
                }

                //for Item category Filter
                if (reportFilters.CategoryFilter.Count() > 0)
                {
                    query += " and (";
                    foreach (var item in reportFilters.CategoryFilter)
                    {
                        query += "b.CATEGORY_CODE = '" + item + "' OR ";
                    }
                    query = query.Substring(0, query.Length - 3) + ") ";
                }

                if (reportFilters.BranchFilter.Count > 0)
                {
                    query += string.Format(@" AND a.BRANCH_CODE IN ('{0}')", string.Join("','", reportFilters.BranchFilter).ToString());
                }

                query += @" 
                GROUP BY a.ITEM_CODE, a.MU_CODE, b.ITEM_EDESC ,A.COMPANY_CODE
    ,A.BRANCH_CODE
                ORDER BY  b.ITEM_EDESC))";
                query = string.Format(query, companyCode, userInfo.branch_code, reportFilters.FromDate, reportFilters.ToDate, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), ReportFilterHelper.RoundUpFilterValue(reportFilters.AmountRoundUpFilter));

                list = this._objectEntity.SqlQuery<SalesSummaryReportModel>(query).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }


        public IEnumerable<SalesAnalysisModel> getSalesAnalysis(ReportFiltersModel model, User userInfo)
        {

            var companyCode = string.Empty;
            foreach (var company in model.CompanyFilter)
            {
                if (company != null)
                    companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);


            string firstQuery = $@"SELECT IGS.ITEM_GROUP_CODE,IGS.ITEM_GROUP_NAME,wm_concat(IGM.ITEM_CODE) ITEM_CODE
                                    FROM IP_ITEM_GROUP_SETUP IGS,
                                              IP_ITEM_GROUP_MAP IGM         
                                    WHERE IGS.ITEM_GROUP_CODE = IGM.GROUP_CODE 
                                    AND IGS.DELETED_FLAG='N'
                                    GROUP BY  IGS.ITEM_GROUP_CODE,IGS.ITEM_GROUP_NAME";


            List<ItemCode> itemGroupData = this._objectEntity.SqlQuery<ItemCode>(firstQuery).ToList();
            string secondQuery = "";
            List<SalesAnalysisModel> data = new List<SalesAnalysisModel>();
            foreach (ItemCode m in itemGroupData)
            {
                ////for Product Filter
                var productFilter = string.Empty;
                if (!string.IsNullOrEmpty(m.ITEM_CODE))
                {
                    productFilter = @"select  DISTINCT item_code from IP_ITEM_MASTER_SETUP where (";
                    //IF PRODUCT_SKU_FLAG = G
                    foreach (var item in m.ITEM_CODE.Split(','))
                    {
                        productFilter += "MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                    }
                    productFilter = productFilter.Substring(0, productFilter.Length - 3);
                    //IF PRODUCT_SKU_FLAG = I                
                    productFilter += " or (ITEM_CODE in (" + m.ITEM_CODE + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";
                }

                secondQuery = $@"SELECT to_char(SI.SALES_DATE) DURATION, TO_DATE(SI.SALES_DATE) SALES_DATE,'{m.ITEM_GROUP_NAME}' ITEM_GROUP_NAME, NVL(SUM(CALC_QUANTITY),0) Quantity,ROUND(NVL(SUM(CALC_TOTAL_PRICE),0),2) AMOUNT,               
                                   ROUND(NVL(SUM(CT.CHARGE_AMOUNT),0),2) DISCOUNT,
                                   ROUND(NVL(SUM((CALC_TOTAL_PRICE-CT.CHARGE_AMOUNT)/NVL(NULLIF(CALC_QUANTITY,0),1)),0),2) REALIZATION
                                    FROM SA_SALES_INVOICE SI,CHARGE_TRANSACTION CT
                                WHERE 1=1 
                                AND SI.ITEM_CODE IN({productFilter})
                                AND SI.SALES_NO = CT.REFERENCE_NO(+)
                                AND SI.COMPANY_CODE = CT.COMPANY_CODE(+)
                                AND CT.CHARGE_CODE(+) ='DC'
                              GROUP BY SI.SALES_DATE";

                var itemData = this._objectEntity.SqlQuery<SalesAnalysisModel>(secondQuery).ToList();
                data.AddRange(itemData);
            }

            var te = data.ToList();
            return te;
        }


        public IEnumerable<HighestSellingModel> GetAllHighestSelling(User userInfo, int selectedItem)
        {
            string selectQuery = $@"select SI.item_code as ITEM_CODE,sum(SI.CALC_TOTAL_PRICE) QTY,I.ITEM_EDESC from SA_SALES_INVOICE SI,IP_ITEM_MASTER_SETUP I where I.ITEM_CODE=SI.ITEM_CODE AND SI.COMPANY_CODE=I.COMPANY_CODE AND SI.deleted_flag='N' and SI.company_code='{userInfo.company_code}' group by SI.item_code,I.ITEM_EDESC
                                       ORDER BY QTY DESC";
            var result = _objectEntity.SqlQuery<HighestSellingModel>(selectQuery).Take(selectedItem).ToList();
            return result;
        }


        public IList<TopEmployeeListModel> getTopSalesEmployee(ReportFiltersModel model, User userInfo, int pageSize)
        {

            var companyCode = string.Empty;
            foreach (var company in model.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);

            var FincalYear = System.Configuration.ConfigurationManager.AppSettings["FiscalYear"].ToString();
            var dateRange = this._controlService.GetDateFilters(FincalYear).OrderByDescending(q => q.SortOrder).ToList();
            var thisMonth = dateRange.Where(x => x.RangeName == "This Month").FirstOrDefault();
            var thisYear = dateRange.Where(x => x.RangeName == "This Year").FirstOrDefault();
            //There  is not   RangeName = thisYear in dateRange  so we are getting error in ytdQuery 
            //this month
            string mtdQuery = $@"SELECT ES.EMPLOYEE_EDESC EMPLOYEE_MTD, NVL(SUM(SI.CALC_TOTAL_PRICE),0) AMOUNT_MTD ,NVL(SUM(SPD.PER_DAY_AMOUNT),0) TARGET_MTD
                                FROM SA_SALES_INVOICE SI,PL_SALES_PLAN_DTL SPD,HR_EMPLOYEE_SETUP ES
                                WHERE SI.EMPLOYEE_CODE = SPD.EMPLOYEE_CODE(+)
                                           AND SI.SALES_DATE = SPD.PLAN_DATE(+)
                                           AND SI.COMPANY_CODE = SPD.COMPANY_CODE(+)
                                           AND SI.EMPLOYEE_CODE = ES.EMPLOYEE_CODE
                                           AND SI.COMPANY_CODE = ES.COMPANY_CODE(+)
                                           AND SI.SALES_DATE >= TO_DATE('{thisMonth.StartDateString}','YYYY-MM-DD') AND SI.SALES_DATE <= TO_DATE('{thisMonth.EndDateString}','YYYY-MM-DD')                                            
                                GROUP BY  ES.EMPLOYEE_EDESC
                                ORDER BY SUM(SI.CALC_TOTAL_PRICE) DESC";
            var mtdData = this._objectEntity.SqlQuery<TopEmployeeListModel>(mtdQuery).Take(pageSize).ToList();
            //this year
            string ytdQuery = $@"SELECT ES.EMPLOYEE_EDESC EMPLOYEE_YTD, NVL(SUM(SI.CALC_TOTAL_PRICE),0) AMOUNT_YTD ,NVL(SUM(SPD.PER_DAY_AMOUNT),0) TARGET_YTD
                                FROM SA_SALES_INVOICE SI,PL_SALES_PLAN_DTL SPD,HR_EMPLOYEE_SETUP ES
                                WHERE SI.EMPLOYEE_CODE = SPD.EMPLOYEE_CODE(+)
                                           AND SI.SALES_DATE = SPD.PLAN_DATE(+)
                                           AND SI.COMPANY_CODE = SPD.COMPANY_CODE(+)
                                           AND SI.EMPLOYEE_CODE = ES.EMPLOYEE_CODE
                                           AND SI.COMPANY_CODE = ES.COMPANY_CODE(+)
                                           AND SI.SALES_DATE >= TO_DATE('{thisYear.StartDateString}','YYYY-MM-DD') AND SI.SALES_DATE <= TO_DATE('{thisYear.EndDateString}','YYYY-MM-DD')   
                                GROUP BY  ES.EMPLOYEE_EDESC
                                ORDER BY SUM(SI.CALC_TOTAL_PRICE) DESC";
            var ytdData = this._objectEntity.SqlQuery<TopEmployeeListModel>(ytdQuery).Take(pageSize).ToList();

            for (int i = 0; i < ytdData.Count(); i++)
            {
                try
                {
                    ytdData[i].EMPLOYEE_MTD = mtdData[i].EMPLOYEE_MTD;
                    ytdData[i].AMOUNT_MTD = mtdData[i].AMOUNT_MTD;
                    ytdData[i].TARGET_MTD = mtdData[i].TARGET_MTD;
                }
                catch (ArgumentOutOfRangeException)
                {
                    ytdData[i].EMPLOYEE_MTD = "-";
                }

            }


            //var TEST = mtdData.Concat(ytdData).ToList();

            return ytdData.Take(pageSize).ToList();
        }


        public IList<SalesAchieveModel> getSalesAchieve(ReportFiltersModel model, string duration, User userInfo)
        {
            //var companyCode = string.Join(",", model.CompanyFilter);
            //companyCode = companyCode == "" ? userInfo.company_code : companyCode;
            var companyCode = string.Empty;
            foreach (var company in model.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);

            var branchFilter = string.Empty;
            if (model.BranchFilter.Count > 0)
            {
                branchFilter += string.Format(@" AND BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
            }

            var FincalYear = System.Configuration.ConfigurationManager.AppSettings["FiscalYear"].ToString();
            var dateRange = this._controlService.GetDateFilters(FincalYear).OrderByDescending(q => q.SortOrder).ToList();
            var dates = dateRange.Where(x => x.RangeName == duration).FirstOrDefault();

            //query
            var mtdQuery = $@"select 
                                        (select sum(CALC_TOTAL_PRICE) from SA_SALES_INVOICE  where company_code in({companyCode}) {branchFilter} and deleted_flag='N' and   SALES_DATE >= TO_DATE('{dates.StartDateString}','YYYY-MM-DD') AND SALES_DATE <= TO_DATE('{dates.EndDateString}','YYYY-MM-DD')) SALES_ACHIEVE,                               
                                       nvl((   select sum(S.PER_DAY_AMOUNT) from PL_SALES_PLAN_DTL S,ip_item_master_setup I where I.ITEM_CODE=S.ITEM_CODE AND I.GROUP_SKU_FLAG='I' AND I.COMPANY_CODE=S.COMPANY_CODE AND S.company_code in({companyCode})  and S.deleted_flag='N' and   S.PLAN_DATE >= TO_DATE('{dates.StartDateString}','YYYY-MM-DD') AND S.PLAN_DATE <= TO_DATE('{dates.EndDateString}','YYYY-MM-DD')),0) TARGET  from dual";
            //string mtdQuery = $@"SELECT NVL(SUM(SI.CALC_TOTAL_PRICE),0) SALES_ACHIEVE,
            //                     NVL(SUM(SPD.PER_DAY_AMOUNT),0) TARGET
            //                    FROM SA_SALES_INVOICE SI,PL_SALES_PLAN_DTL SPD
            //                    WHERE   SI.SALES_DATE = SPD.PLAN_DATE
            //                            AND SI.COMPANY_CODE = SPD.COMPANY_CODE                                           
            //                            AND SI.COMPANY_CODE IN ({companyCode}) {branchFilter}
            //                            AND SI.SALES_DATE >= TO_DATE('{dates.StartDateString}','YYYY-MM-DD') AND SI.SALES_DATE <= TO_DATE('{dates.EndDateString}','YYYY-MM-DD')";
            var data = this._objectEntity.SqlQuery<SalesAchieveModel>(mtdQuery).ToList();
            return data;
        }

        public IList<SalesAchieveModel> getSalesAchieveProjected(ReportFiltersModel model, string duration, User userInfo)
        {
            //var companyCode = string.Join(",", model.CompanyFilter);
            //companyCode = companyCode == "" ? userInfo.company_code : companyCode;
            var companyCode = string.Empty;
            foreach (var company in model.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);

            var branchFilter = string.Empty;
            if (model.BranchFilter.Count > 0)
            {
                branchFilter += string.Format(@" AND BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
            }

            var FincalYear = System.Configuration.ConfigurationManager.AppSettings["FiscalYear"].ToString();
            var dateRange = this._controlService.GetDateFilters(FincalYear).OrderByDescending(q => q.SortOrder).ToList();
            var dates = dateRange.Where(x => x.RangeName == duration).FirstOrDefault();
            var totalDays = (dates.EndDate- dates.StartDate).TotalDays;
            var todayDays= (DateTime.Now-dates.StartDate).TotalDays;

            //query
            var mtdQuery = $@"select 
                                        (select sum(CALC_TOTAL_PRICE) from SA_SALES_INVOICE  where company_code in({companyCode}) {branchFilter} and deleted_flag='N' and   SALES_DATE >= TO_DATE('{dates.StartDateString}','YYYY-MM-DD') AND SALES_DATE <= TO_DATE('{dates.EndDateString}','YYYY-MM-DD')) SALES_ACHIEVE,                               
                                       nvl((
select sum(S.PER_DAY_AMOUNT) from PL_SALES_PLAN_DTL S
                                            where (S.ITEM_CODE IN (SELECT ITEM_CODE FROM ip_item_master_setup 
                                            WHERE GROUP_SKU_FLAG='I'
                                            AND COMPANY_CODE = S.COMPANY_CODE) 
                                            OR ITEM_CODE = 0)
                                             AND S.company_code in({companyCode})  and S.deleted_flag='N' 
                                             and   S.PLAN_DATE >= TO_DATE('{dates.StartDateString}','YYYY-MM-DD') 
                                             AND S.PLAN_DATE <= TO_DATE('{dates.EndDateString}','YYYY-MM-DD')),0) TARGET  from dual";
            //string mtdQuery = $@"SELECT NVL(SUM(SI.CALC_TOTAL_PRICE),0) SALES_ACHIEVE,
            //                     NVL(SUM(SPD.PER_DAY_AMOUNT),0) TARGET
            //                    FROM SA_SALES_INVOICE SI,PL_SALES_PLAN_DTL SPD
            //                    WHERE   SI.SALES_DATE = SPD.PLAN_DATE
            //                            AND SI.COMPANY_CODE = SPD.COMPANY_CODE                                           
            //                            AND SI.COMPANY_CODE IN ({companyCode}) {branchFilter}
            //                            AND SI.SALES_DATE >= TO_DATE('{dates.StartDateString}','YYYY-MM-DD') AND SI.SALES_DATE <= TO_DATE('{dates.EndDateString}','YYYY-MM-DD')";
            var data = this._objectEntity.SqlQuery<SalesAchieveModel>(mtdQuery).ToList();
            foreach(var days in data)
            {
                var perDayAchived = days.SALES_ACHIEVE /  Convert.ToDecimal(todayDays);
                var daysRemaining = totalDays - todayDays;
                var unachived = perDayAchived * Convert.ToDecimal(daysRemaining);
                var ProjectedSales = days.SALES_ACHIEVE + unachived;
                days.SALES_PROJECTED = Convert.ToDecimal(Convert.ToDecimal(ProjectedSales).ToString("0.00"));
                if (days.TARGET>0)
                {
                    days.Projected = ((ProjectedSales / days.TARGET) * 100)??0;
                    
                }
                else
                {
                    days.Projected = ((ProjectedSales / ProjectedSales) * 100) ?? 0;
                }
              

            }

            return data;
        }


        public IList<PendingOrder> getPendingOrderChart(ReportFiltersModel model, bool branchwiseChart, User userInfo)
        {
            //var companyCode = string.Join("','", model.CompanyFilter);
            //companyCode = companyCode == "" ? userInfo.company_code : companyCode;
            var companyCode = string.Empty;
            foreach (var company in model.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);

            var branchFilter = string.Empty;
            if (model.BranchFilter.Count > 0)
            {
                branchFilter += string.Format(@" AND BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
            }
            string Query = string.Empty;
            if (!branchwiseChart)
            {
                Query = $@"SELECT * FROM(
                                SELECT COUNT(ORDER_NO) ORDER_PLACED FROM (SELECT DISTINCT ORDER_NO 
                                FROM  SA_SALES_ORDER
                                WHERE DELETED_FLAG = 'N'
                                AND COMPANY_CODE IN({companyCode}) {branchFilter})),
                                (SELECT COUNT(REFERENCE_NO) ORDER_DISPATCHED FROM (
                                SELECT DISTINCT REFERENCE_NO FROM REFERENCE_DETAIL
                                   WHERE REFERENCE_FORM_CODE IN (SELECT DISTINCT FORM_CODE FROM  FORM_DETAIL_SETUP WHERE TABLE_NAME='SA_SALES_ORDER' AND DELETED_FLAG='N' AND COMPANY_CODE IN({companyCode}) {branchFilter} )
                                              AND DELETED_FLAG = 'N' AND COMPANY_CODE IN({companyCode}) {branchFilter}))";
            }
            else
            {
                Query = $@"SELECT A.BRANCH_CODE,C.ABBR_CODE, ORDER_PLACED,NVL(ORDER_DISPATCHED,0) ORDER_DISPATCHED FROM (SELECT COUNT(ORDER_NO) ORDER_PLACED,BRANCH_CODE FROM (SELECT DISTINCT ORDER_NO,BRANCH_CODE
                                FROM  SA_SALES_ORDER
                                WHERE DELETED_FLAG = 'N'
                                AND COMPANY_CODE IN({companyCode}) {branchFilter})
                                GROUP BY BRANCH_CODE) A,
                                (SELECT COUNT(REFERENCE_NO) ORDER_DISPATCHED,BRANCH_CODE FROM (
                                SELECT DISTINCT REFERENCE_NO,BRANCH_CODE FROM REFERENCE_DETAIL
                                   WHERE REFERENCE_FORM_CODE IN (SELECT DISTINCT FORM_CODE FROM  FORM_DETAIL_SETUP WHERE TABLE_NAME='SA_SALES_ORDER' AND DELETED_FLAG='N' AND COMPANY_CODE IN({companyCode}) )
                                              AND DELETED_FLAG = 'N' AND COMPANY_CODE IN('{companyCode}') {branchFilter})
                                GROUP BY BRANCH_CODE ) B, FA_BRANCH_SETUP C
                                WHERE A.BRANCH_CODE = B.BRANCH_CODE(+)
                                AND A.BRANCH_CODE = C.BRANCH_CODE(+)";

            }

            var data = this._objectEntity.SqlQuery<PendingOrder>(Query).ToList();
            return data;
        }

        public IList<SalesAchieveModel> getSalesAchieveBranch(ReportFiltersModel model, string duration, User userInfo)
        {
            var companyCode = string.Join(",", model.CompanyFilter);
            companyCode = companyCode == "" ? userInfo.company_code : companyCode;

            var branchFilter = string.Empty;
            if (model.BranchFilter.Count > 0)
            {
                branchFilter += string.Format(@" AND SI.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
            }

            var FincalYear = System.Configuration.ConfigurationManager.AppSettings["FiscalYear"].ToString();
            var dateRange = this._controlService.GetDateFilters(FincalYear).OrderByDescending(q => q.SortOrder).ToList();
            var dates = dateRange.Where(x => x.RangeName == duration).FirstOrDefault();
            //need optimize query

            string mtdQuery = $@"SELECT DS.BRANCH_EDESC as branch_name, DS.BRANCH_CODE as branch_code,
                            fn_bs_month(substr(bs_date(si.sales_date),6,2)) AS Month,
                            SUBSTR(BS_DATE(SI.SALES_DATE),6,2) as MonthInt,
                           0 TargetQty,
                             0 TargetAmount,
                            SUM (nvl(si.calc_Quantity,0))/1 AS Quantity,
                            SUM (nvl(si.calc_total_price,0))/1 AS GrossAmount,
                            'SALES' DATATYPE
                    FROM sa_sales_invoice si, FA_BRANCH_SETUP DS
                    WHERE  1=1
                    AND si.deleted_flag = 'N' AND si.company_code IN ({companyCode}) {branchFilter}
                    and SI.BRANCH_CODE = DS.BRANCH_CODE
                   AND SI.COMPANY_CODE IN ({companyCode}) 
                     and SI.SALES_DATE >= TO_DATE('{dates.StartDateString}','YYYY-MM-DD') AND SI.SALES_DATE <= TO_DATE('{dates.EndDateString}','YYYY-MM-DD')
            GROUP BY DS.BRANCH_CODE, DS.BRANCH_EDESC, SUBSTR(BS_DATE(SI.SALES_DATE),6,2)
                    ,fn_bs_month(substr(bs_date(si.sales_date),6,2))
                    ,to_number(substr(bs_date(sales_date),6,2))
                    UNION  ALL
                    SELECT DS.BRANCH_EDESC as branch_name, DS.BRANCH_CODE as branch_code,
                            fn_bs_month(substr(bs_date(si.PLAN_DATE),6,2)) AS Month,
                            SUBSTR(BS_DATE(SI.PLAN_DATE),6,2) as MonthInt,
                            SUM (nvl(si.PER_DAY_QUANTITY,0))/1 AS TargetQty,
                            SUM (nvl(si.PER_DAY_AMOUNT,0))/1 AS TargetAmount,
                            0 Quantity,
                            0 GrossAmount,
                               'TARGET' DATATYPE
                    FROM PL_SALES_PLAN_DTL si, FA_BRANCH_SETUP DS,IP_ITEM_MASTER_SETUP I
                    WHERE 1=1
                    AND si.deleted_flag = 'N' AND si.company_code in({companyCode}) {branchFilter}
                    AND I.ITEM_CODE=SI.ITEM_CODE
                    AND I.GROUP_SKU_FLAG='I'
                    and SI.BRANCH_CODE = DS.BRANCH_CODE
                    AND SI.COMPANY_CODE IN ({companyCode}) 
                    AND SI.PLAN_DATE >= TO_DATE('{dates.StartDateString}','YYYY-MM-DD') AND SI.PLAN_DATE <= TO_DATE('{dates.EndDateString}','YYYY-MM-DD')
                    GROUP BY DS.BRANCH_CODE, DS.BRANCH_EDESC, SUBSTR(BS_DATE(SI.PLAN_DATE),6,2)
                    ,fn_bs_month(substr(bs_date(si.PLAN_DATE),6,2))
                    ,to_number(substr(bs_date(PLAN_DATE),6,2))";
            //this month
            //string mtdQuery = $@"SELECT BS.BRANCH_CODE, BS.BRANCH_EDESC, NVL(SUM(SI.CALC_TOTAL_PRICE),0) SALES_ACHIEVE,
            //                     NVL(SUM(SPD.PER_DAY_AMOUNT),1) TARGET
            //                    FROM SA_SALES_INVOICE SI,PL_SALES_PLAN_DTL SPD,FA_BRANCH_SETUP BS
            //                    WHERE  SI.SALES_DATE = SPD.PLAN_DATE(+)
            //                               AND SI.COMPANY_CODE = SPD.COMPANY_CODE(+)
            //                               AND SI.BRANCH_CODE = BS.BRANCH_CODE
            //                               AND SI.COMPANY_CODE = BS.COMPANY_CODE(+)  
            //                               AND SI.COMPANY_CODE IN ({companyCode}) {branchFilter}
            //                               AND SI.SALES_DATE >= TO_DATE('{dates.StartDateString}','YYYY-MM-DD') AND SI.SALES_DATE <= TO_DATE('{dates.EndDateString}','YYYY-MM-DD')
            //                    GROUP BY  BS.BRANCH_EDESC,BS.BRANCH_CODE
            //                    ORDER BY BS.BRANCH_EDESC";
            var datas = this._objectEntity.SqlQuery<SalesTargetViewModel>(mtdQuery).ToList();

            var branchwisesales = new List<SalesAchieveModel>();
            foreach (var data in datas.GroupBy(x => x.Branch_code).Select(y => y.FirstOrDefault()))
            {
                var branchWise = new SalesAchieveModel();
                branchWise.BRANCH_CODE = data.Branch_code;
                branchWise.BRANCH_EDESC = data.Branch_name;
                branchWise.SALES_ACHIEVE = Convert.ToDecimal(datas.Where(x => x.Branch_code == data.Branch_code && x.DataType == "SALES").Sum(X => X.GrossAmount));
                branchWise.TARGET = Convert.ToDecimal(datas.Where(X => X.Branch_code == data.Branch_code && X.DataType != "SALES").Sum(X => X.TargetAmount));
                branchwisesales.Add(branchWise);

                //var 
            }
            return branchwisesales;
        }

        public IList<SalesAchieveModel> getSalesAchieveDivision(ReportFiltersModel model, string duration, User userInfo)
        {
            var companyCode = string.Join(",", model.CompanyFilter);
            companyCode = companyCode == "" ? userInfo.company_code : companyCode;

            var branchFilter = string.Empty;
            if (model.BranchFilter.Count > 0)
            {
                branchFilter += string.Format(@" AND SI.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
            }
            //if (model.DivisionFilter.Count > 0)
            //{
            //    branchFilter += string.Format(@" AND SI.DIVISION_CODE IN ('{0}')", string.Join("','", model.DivisionFilter).ToString());
            //}
            var FincalYear = System.Configuration.ConfigurationManager.AppSettings["FiscalYear"].ToString();
            var dateRange = this._controlService.GetDateFilters(FincalYear).OrderByDescending(q => q.SortOrder).ToList();
            var dates = dateRange.Where(x => x.RangeName == duration).FirstOrDefault();
            //need optimize query

            var mtdQuery = $@"SELECT DS.DIVISION_EDESC as branch_name, DS.DIVISION_CODE as branch_code,
                            fn_bs_month(substr(bs_date(si.sales_date),6,2)) AS Month,
                            SUBSTR(BS_DATE(SI.SALES_DATE),6,2) as MonthInt,
                           0 TargetQty,
                             0 TargetAmount,
                            SUM (nvl(si.calc_Quantity,0))/1 AS Quantity,
                            SUM (nvl(si.calc_total_price,0))/1 AS GrossAmount,
                            'SALES' DATATYPE
                    FROM sa_sales_invoice si, fa_division_setup DS
                    WHERE  1=1
                    AND si.deleted_flag = 'N' AND  si.company_code IN ({companyCode}) {branchFilter}
                    and SI.DIVISION_CODE = DS.DIVISION_CODE
 and SI.SALES_DATE >= TO_DATE('{dates.StartDateString}','YYYY-MM-DD') AND SI.SALES_DATE <= TO_DATE('{dates.EndDateString}','YYYY-MM-DD')
                   AND SI.COMPANY_CODE IN ({companyCode}) GROUP BY DS.DIVISION_EDESC, DS.DIVISION_CODE, SUBSTR(BS_DATE(SI.SALES_DATE),6,2)
                    ,fn_bs_month(substr(bs_date(si.sales_date),6,2))
                    ,to_number(substr(bs_date(sales_date),6,2))
                    UNION  ALL
                    SELECT DS.DIVISION_EDESC as branch_name, DS.DIVISION_CODE as branch_code,
                            fn_bs_month(substr(bs_date(si.PLAN_DATE),6,2)) AS Month,
                            SUBSTR(BS_DATE(SI.PLAN_DATE),6,2) as MonthInt,
                            SUM (nvl(si.PER_DAY_QUANTITY,0))/1 AS TargetQty,
                            SUM (nvl(si.PER_DAY_AMOUNT,0))/1 AS TargetAmount,
                            0 Quantity,
                            0 GrossAmount,
                               'TARGET' DATATYPE
                    FROM PL_SALES_PLAN_DTL si, fa_division_setup DS,IP_ITEM_MASTER_SETUP I
                    WHERE 1=1
                    AND si.deleted_flag = 'N' AND si.company_code in ({companyCode}) {branchFilter}
                    AND I.ITEM_CODE=SI.ITEM_CODE
                    AND I.GROUP_SKU_FLAG='I'
                    and SI.DIVISION_CODE = DS.DIVISION_CODE
                    AND SI.COMPANY_CODE IN ({companyCode}) {branchFilter}
 AND SI.PLAN_DATE >= TO_DATE('{dates.StartDateString}','YYYY-MM-DD') AND SI.PLAN_DATE <= TO_DATE('{dates.EndDateString}','YYYY-MM-DD')
                    GROUP BY DS.DIVISION_EDESC, DS.DIVISION_CODE, SUBSTR(BS_DATE(SI.PLAN_DATE),6,2)
                    ,fn_bs_month(substr(bs_date(si.PLAN_DATE),6,2))
                    ,to_number(substr(bs_date(PLAN_DATE),6,2))";

            //string mtdQuery = $@"SELECT DS.BRANCH_EDESC as branch_name, DS.BRANCH_CODE as branch_code,
            //                fn_bs_month(substr(bs_date(si.sales_date),6,2)) AS Month,
            //                SUBSTR(BS_DATE(SI.SALES_DATE),6,2) as MonthInt,
            //               0 TargetQty,
            //                 0 TargetAmount,
            //                SUM (nvl(si.calc_Quantity,0))/1 AS Quantity,
            //                SUM (nvl(si.calc_total_price,0))/1 AS GrossAmount,
            //                'SALES' DATATYPE
            //        FROM sa_sales_invoice si, FA_BRANCH_SETUP DS
            //        WHERE  1=1
            //        AND si.deleted_flag = 'N' AND si.company_code IN ({companyCode}) {branchFilter}
            //        and SI.BRANCH_CODE = DS.BRANCH_CODE
            //       AND SI.COMPANY_CODE IN ({companyCode}) 
            //         and SI.SALES_DATE >= TO_DATE('{dates.StartDateString}','YYYY-MM-DD') AND SI.SALES_DATE <= TO_DATE('{dates.EndDateString}','YYYY-MM-DD')
            //GROUP BY DS.BRANCH_CODE, DS.BRANCH_EDESC, SUBSTR(BS_DATE(SI.SALES_DATE),6,2)
            //        ,fn_bs_month(substr(bs_date(si.sales_date),6,2))
            //        ,to_number(substr(bs_date(sales_date),6,2))
            //        UNION  ALL
            //        SELECT DS.BRANCH_EDESC as branch_name, DS.BRANCH_CODE as branch_code,
            //                fn_bs_month(substr(bs_date(si.PLAN_DATE),6,2)) AS Month,
            //                SUBSTR(BS_DATE(SI.PLAN_DATE),6,2) as MonthInt,
            //                SUM (nvl(si.PER_DAY_QUANTITY,0))/1 AS TargetQty,
            //                SUM (nvl(si.PER_DAY_AMOUNT,0))/1 AS TargetAmount,
            //                0 Quantity,
            //                0 GrossAmount,
            //                   'TARGET' DATATYPE
            //        FROM PL_SALES_PLAN_DTL si, FA_BRANCH_SETUP DS,IP_ITEM_MASTER_SETUP I
            //        WHERE 1=1
            //        AND si.deleted_flag = 'N' AND si.company_code in({companyCode}) {branchFilter}
            //        AND I.ITEM_CODE=SI.ITEM_CODE
            //        AND I.GROUP_SKU_FLAG='I'
            //        and SI.BRANCH_CODE = DS.BRANCH_CODE
            //        AND SI.COMPANY_CODE IN ({companyCode}) 
            //        AND SI.PLAN_DATE >= TO_DATE('{dates.StartDateString}','YYYY-MM-DD') AND SI.PLAN_DATE <= TO_DATE('{dates.EndDateString}','YYYY-MM-DD')
            //        GROUP BY DS.BRANCH_CODE, DS.BRANCH_EDESC, SUBSTR(BS_DATE(SI.PLAN_DATE),6,2)
            //        ,fn_bs_month(substr(bs_date(si.PLAN_DATE),6,2))
            //        ,to_number(substr(bs_date(PLAN_DATE),6,2))";
            //this month
            //string mtdQuery = $@"SELECT BS.BRANCH_CODE, BS.BRANCH_EDESC, NVL(SUM(SI.CALC_TOTAL_PRICE),0) SALES_ACHIEVE,
            //                     NVL(SUM(SPD.PER_DAY_AMOUNT),1) TARGET
            //                    FROM SA_SALES_INVOICE SI,PL_SALES_PLAN_DTL SPD,FA_BRANCH_SETUP BS
            //                    WHERE  SI.SALES_DATE = SPD.PLAN_DATE(+)
            //                               AND SI.COMPANY_CODE = SPD.COMPANY_CODE(+)
            //                               AND SI.BRANCH_CODE = BS.BRANCH_CODE
            //                               AND SI.COMPANY_CODE = BS.COMPANY_CODE(+)  
            //                               AND SI.COMPANY_CODE IN ({companyCode}) {branchFilter}
            //                               AND SI.SALES_DATE >= TO_DATE('{dates.StartDateString}','YYYY-MM-DD') AND SI.SALES_DATE <= TO_DATE('{dates.EndDateString}','YYYY-MM-DD')
            //                    GROUP BY  BS.BRANCH_EDESC,BS.BRANCH_CODE
            //                    ORDER BY BS.BRANCH_EDESC";
            var datas = this._objectEntity.SqlQuery<SalesTargetViewModel>(mtdQuery).ToList();

            var branchwisesales = new List<SalesAchieveModel>();
            foreach (var data in datas.GroupBy(x => x.Branch_code).Select(y => y.FirstOrDefault()))
            {
                var branchWise = new SalesAchieveModel();
                branchWise.BRANCH_CODE = data.Branch_code;
                branchWise.BRANCH_EDESC = data.Branch_name;
                branchWise.SALES_ACHIEVE = Convert.ToDecimal(datas.Where(x => x.Branch_code == data.Branch_code && x.DataType == "SALES").Sum(X => X.GrossAmount));
                branchWise.TARGET = Convert.ToDecimal(datas.Where(X => X.Branch_code == data.Branch_code && X.DataType != "SALES").Sum(X => X.TargetAmount));
                branchwisesales.Add(branchWise);

                //var 
            }
            return branchwisesales;
        }

        public IList<SalesAchieveMonthModel> getSalesAchieveDivisionMonth(ReportFiltersModel model, string duration, User userInfo)
        {
            var companyCode = string.Join(",", model.CompanyFilter);
            companyCode = companyCode == "" ? userInfo.company_code : companyCode;

            var branchFilter = string.Empty;
            var DivisionFilter = string.Empty;
            if (model.BranchFilter.Count > 0)
            {
                branchFilter += string.Format(@" AND SI.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
            }
            if (model.DivisionFilter.Count > 0)
            {
                DivisionFilter += string.Format(@" AND SI.DIVISION_CODE IN ('{0}')", string.Join("','", model.DivisionFilter).ToString());
            }
            var FincalYear = System.Configuration.ConfigurationManager.AppSettings["FiscalYear"].ToString();
          //  var dateRange = this._controlService.GetDateFilters(FincalYear).OrderByDescending(q => q.SortOrder).ToList();
           // var dates = dateRange.Where(x => x.RangeName == duration).FirstOrDefault();
            //need optimize query

            var mtdQuery = $@"SELECT YEAR , MONTH, MONTHINT, SUM(TARGETQTY) TARGETQTY, SUM(TARGETAMOUNT) TARGETAMOUNT, SUM(QUANTITY) QUANTITY, SUM(GROSSAMOUNT) GROSSAMOUNT FROM (
SELECT SUBSTR (BS_DATE (SI.SALES_DATE), 1, 4) AS YEAR,  FN_BS_MONTH(SUBSTR(BS_DATE(SI.SALES_DATE),6,2)) AS MONTH, SUBSTR(BS_DATE(SI.SALES_DATE),6,2) AS MONTHINT,
0 TARGETQTY, 0 TARGETAMOUNT, SUM (NVL(SI.CALC_QUANTITY,0))/1 AS QUANTITY, SUM (NVL(SI.CALC_TOTAL_PRICE,0))/1 AS GROSSAMOUNT,  'SALES' DATATYPE
FROM SA_SALES_INVOICE SI
WHERE  1=1
AND SI.DELETED_FLAG = 'N' 
AND SI.COMPANY_CODE IN ({companyCode}) 
{DivisionFilter}
GROUP BY  SUBSTR(BS_DATE(SI.SALES_DATE),6,2)
,FN_BS_MONTH(SUBSTR(BS_DATE(SI.SALES_DATE),6,2))
,TO_NUMBER(SUBSTR(BS_DATE(SALES_DATE),6,2)),
SUBSTR (BS_DATE (SI.SALES_DATE), 1, 4)
UNION  ALL
SELECT SUBSTR (BS_DATE (SI.PLAN_DATE), 1, 4) AS YEAR,
FN_BS_MONTH(SUBSTR(BS_DATE(SI.PLAN_DATE),6,2)) AS MONTH,
SUBSTR(BS_DATE(SI.PLAN_DATE),6,2) AS MONTHINT,
SUM (NVL(SI.PER_DAY_QUANTITY,0))/1 AS TARGETQTY,
SUM (NVL(SI.PER_DAY_AMOUNT,0))/1 AS TARGETAMOUNT,
0 QUANTITY,
0 GROSSAMOUNT,
'TARGET' DATATYPE
FROM PL_SALES_PLAN_DTL SI,IP_ITEM_MASTER_SETUP I
WHERE 1=1
AND SI.DELETED_FLAG = 'N' 
AND SI.COMPANY_CODE=I.COMPANY_CODE
AND I.ITEM_CODE=SI.ITEM_CODE
AND I.GROUP_SKU_FLAG='I'
AND SI.COMPANY_CODE IN ({companyCode}) 
{DivisionFilter}
GROUP BY SUBSTR(BS_DATE(SI.PLAN_DATE),6,2)
,FN_BS_MONTH(SUBSTR(BS_DATE(SI.PLAN_DATE),6,2))
,TO_NUMBER(SUBSTR(BS_DATE(PLAN_DATE),6,2)),SUBSTR (BS_DATE (SI.PLAN_DATE), 1, 4) 
) GROUP BY YEAR , MONTH, MONTHINT
ORDER BY YEAR , MONTHINT";

            //string mtdQuery = $@"SELECT DS.BRANCH_EDESC as branch_name, DS.BRANCH_CODE as branch_code,
            //                fn_bs_month(substr(bs_date(si.sales_date),6,2)) AS Month,
            //                SUBSTR(BS_DATE(SI.SALES_DATE),6,2) as MonthInt,
            //               0 TargetQty,
            //                 0 TargetAmount,
            //                SUM (nvl(si.calc_Quantity,0))/1 AS Quantity,
            //                SUM (nvl(si.calc_total_price,0))/1 AS GrossAmount,
            //                'SALES' DATATYPE
            //        FROM sa_sales_invoice si, FA_BRANCH_SETUP DS
            //        WHERE  1=1
            //        AND si.deleted_flag = 'N' AND si.company_code IN ({companyCode}) {branchFilter}
            //        and SI.BRANCH_CODE = DS.BRANCH_CODE
            //       AND SI.COMPANY_CODE IN ({companyCode}) 
            //         and SI.SALES_DATE >= TO_DATE('{dates.StartDateString}','YYYY-MM-DD') AND SI.SALES_DATE <= TO_DATE('{dates.EndDateString}','YYYY-MM-DD')
            //GROUP BY DS.BRANCH_CODE, DS.BRANCH_EDESC, SUBSTR(BS_DATE(SI.SALES_DATE),6,2)
            //        ,fn_bs_month(substr(bs_date(si.sales_date),6,2))
            //        ,to_number(substr(bs_date(sales_date),6,2))
            //        UNION  ALL
            //        SELECT DS.BRANCH_EDESC as branch_name, DS.BRANCH_CODE as branch_code,
            //                fn_bs_month(substr(bs_date(si.PLAN_DATE),6,2)) AS Month,
            //                SUBSTR(BS_DATE(SI.PLAN_DATE),6,2) as MonthInt,
            //                SUM (nvl(si.PER_DAY_QUANTITY,0))/1 AS TargetQty,
            //                SUM (nvl(si.PER_DAY_AMOUNT,0))/1 AS TargetAmount,
            //                0 Quantity,
            //                0 GrossAmount,
            //                   'TARGET' DATATYPE
            //        FROM PL_SALES_PLAN_DTL si, FA_BRANCH_SETUP DS,IP_ITEM_MASTER_SETUP I
            //        WHERE 1=1
            //        AND si.deleted_flag = 'N' AND si.company_code in({companyCode}) {branchFilter}
            //        AND I.ITEM_CODE=SI.ITEM_CODE
            //        AND I.GROUP_SKU_FLAG='I'
            //        and SI.BRANCH_CODE = DS.BRANCH_CODE
            //        AND SI.COMPANY_CODE IN ({companyCode}) 
            //        AND SI.PLAN_DATE >= TO_DATE('{dates.StartDateString}','YYYY-MM-DD') AND SI.PLAN_DATE <= TO_DATE('{dates.EndDateString}','YYYY-MM-DD')
            //        GROUP BY DS.BRANCH_CODE, DS.BRANCH_EDESC, SUBSTR(BS_DATE(SI.PLAN_DATE),6,2)
            //        ,fn_bs_month(substr(bs_date(si.PLAN_DATE),6,2))
            //        ,to_number(substr(bs_date(PLAN_DATE),6,2))";
            //this month
            //string mtdQuery = $@"SELECT BS.BRANCH_CODE, BS.BRANCH_EDESC, NVL(SUM(SI.CALC_TOTAL_PRICE),0) SALES_ACHIEVE,
            //                     NVL(SUM(SPD.PER_DAY_AMOUNT),1) TARGET
            //                    FROM SA_SALES_INVOICE SI,PL_SALES_PLAN_DTL SPD,FA_BRANCH_SETUP BS
            //                    WHERE  SI.SALES_DATE = SPD.PLAN_DATE(+)
            //                               AND SI.COMPANY_CODE = SPD.COMPANY_CODE(+)
            //                               AND SI.BRANCH_CODE = BS.BRANCH_CODE
            //                               AND SI.COMPANY_CODE = BS.COMPANY_CODE(+)  
            //                               AND SI.COMPANY_CODE IN ({companyCode}) {branchFilter}
            //                               AND SI.SALES_DATE >= TO_DATE('{dates.StartDateString}','YYYY-MM-DD') AND SI.SALES_DATE <= TO_DATE('{dates.EndDateString}','YYYY-MM-DD')
            //                    GROUP BY  BS.BRANCH_EDESC,BS.BRANCH_CODE
            //                    ORDER BY BS.BRANCH_EDESC";
            var datas = this._objectEntity.SqlQuery<SalesAchieveMonthModel>(mtdQuery).ToList();
            return datas;
        }

        public IList<SalesAchieveModel> getSalesAchieveItemWise(ReportFiltersModel model, string duration, string branchCode, User userInfo)
        {
            var companyCode = string.Join(",", model.CompanyFilter);
            companyCode = companyCode == "" ? userInfo.company_code : companyCode;

            var FincalYear = System.Configuration.ConfigurationManager.AppSettings["FiscalYear"].ToString();
            var dateRange = this._controlService.GetDateFilters(FincalYear).OrderByDescending(q => q.SortOrder).ToList();
            var dates = dateRange.Where(x => x.RangeName == duration).FirstOrDefault();

            //this month
            string mtdQuery = $@"SELECT  IM.ITEM_EDESC, NVL(SUM(SI.CALC_TOTAL_PRICE),0) SALES_ACHIEVE,
                                 NVL(SUM(SPD.PER_DAY_AMOUNT),1) TARGET
                                FROM SA_SALES_INVOICE SI,PL_SALES_PLAN_DTL SPD,IP_ITEM_MASTER_SETUP IM
                                WHERE  SI.SALES_DATE = SPD.PLAN_DATE(+)
                                           AND SI.COMPANY_CODE = SPD.COMPANY_CODE(+)
                                             AND SI.ITEM_CODE = SPD.ITEM_CODE(+)
                                           AND SI.ITEM_CODE = IM.ITEM_CODE(+)
                                           AND SI.COMPANY_CODE = IM.COMPANY_CODE(+)  
                                           AND SI.COMPANY_CODE IN ({companyCode})
                                           AND SI.BRANCH_CODE IN ({branchCode})                                          
                                           AND SI.SALES_DATE >= TO_DATE('{dates.StartDateString}','YYYY-MM-DD') AND SI.SALES_DATE <= TO_DATE('{dates.EndDateString}','YYYY-MM-DD')
                                GROUP BY  IM.ITEM_EDESC
                                ORDER BY IM.ITEM_EDESC";
            var data = this._objectEntity.SqlQuery<SalesAchieveModel>(mtdQuery).ToList();
            return data;
        }


        public IList<SalesAchieveModel> getSalesDivisionAchieveItemWise(ReportFiltersModel model, string duration, string branchCode, User userInfo)
        {
            var companyCode = string.Join(",", model.CompanyFilter);
            companyCode = companyCode == "" ? userInfo.company_code : companyCode;

            var FincalYear = System.Configuration.ConfigurationManager.AppSettings["FiscalYear"].ToString();
            var dateRange = this._controlService.GetDateFilters(FincalYear).OrderByDescending(q => q.SortOrder).ToList();
            var dates = dateRange.Where(x => x.RangeName == duration).FirstOrDefault();

            //this month
            string mtdQuery = $@"SELECT  IM.ITEM_EDESC, NVL(SUM(SI.CALC_TOTAL_PRICE),0) SALES_ACHIEVE,
                                 NVL(SUM(SPD.PER_DAY_AMOUNT),1) TARGET
                                FROM SA_SALES_INVOICE SI,PL_SALES_PLAN_DTL SPD,IP_ITEM_MASTER_SETUP IM
                                WHERE  SI.SALES_DATE = SPD.PLAN_DATE(+)
                                           AND SI.COMPANY_CODE = SPD.COMPANY_CODE(+)
                                             AND SI.ITEM_CODE = SPD.ITEM_CODE(+)
                                           AND SI.ITEM_CODE = IM.ITEM_CODE(+)
                                           AND SI.COMPANY_CODE = IM.COMPANY_CODE(+)  
                                           AND SI.COMPANY_CODE IN ({companyCode})
                                           AND SI.DIVISION_CODE IN ({branchCode})                                          
                                           AND SI.SALES_DATE >= TO_DATE('{dates.StartDateString}','YYYY-MM-DD') AND SI.SALES_DATE <= TO_DATE('{dates.EndDateString}','YYYY-MM-DD')
                                GROUP BY  IM.ITEM_EDESC
                                ORDER BY IM.ITEM_EDESC";
            var data = this._objectEntity.SqlQuery<SalesAchieveModel>(mtdQuery).ToList();
            return data;
        }

        public IList<TopDealerListModel> getTopSalesDealer(ReportFiltersModel model, User userInfo, int pageSize)
        {
            //var companyCode = string.Join(",", model.CompanyFilter);
            //companyCode = companyCode == "" ? userInfo.company_code : companyCode;
            var companyCode = string.Empty;
            foreach (var company in model.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);

            var FincalYear = System.Configuration.ConfigurationManager.AppSettings["FiscalYear"].ToString();
            var dateRange = this._controlService.GetDateFilters(FincalYear).OrderByDescending(q => q.SortOrder).ToList();
            var thisMonth = dateRange.Where(x => x.RangeName == "This Month").FirstOrDefault();
            var thisYear = dateRange.Where(x => x.RangeName == "This Year").FirstOrDefault();

            //this month
            string mtdQuery = $@"SELECT CS.PARTY_TYPE_EDESC DEALER_MTD, NVL(SUM(SI.CALC_QUANTITY),0) QUANTITY_MTD ,NVL(SUM(SI.CALC_TOTAL_PRICE),0) SALES_MTD,NVL(SUM(SL.CR_AMOUNT),0) COLLECTION_MTD,COUNT(SI.SALES_NO) BILLCOUNT_MTD
                                    FROM SA_SALES_INVOICE SI,IP_PARTY_TYPE_CODE CS,v$virtual_sub_ledger sl
                                               WHERE SI.PARTY_TYPE_CODE = CS.PARTY_TYPE_CODE
                                    AND SI.COMPANY_CODE = SL.COMPANY_CODE(+)
                                    AND SI.COMPANY_CODE=CS.COMPANY_CODE
                                     AND SI.PARTY_TYPE_CODE = SL.PARTY_TYPE_CODE(+)
                                    AND SI.SALES_DATE = SL.VOUCHER_DATE(+)
                                    AND SI.SALES_DATE >= TO_DATE('{thisMonth.StartDateString}','YYYY-MM-DD') AND SI.SALES_DATE <= TO_DATE('{thisMonth.EndDateString}','YYYY-MM-DD')                                            
                                    GROUP BY CS.PARTY_TYPE_EDESC
                                    ORDER BY SUM(SI.CALC_TOTAL_PRICE) DESC,CS.PARTY_TYPE_EDESC";

            var mtdData = this._objectEntity.SqlQuery<TopDealerListModel>(mtdQuery).Take(pageSize).ToList();
            //total sales/collection
            string totalMtdQuery = $@"SELECT * FROM
                                        (SELECT NVL(SUM(CALC_TOTAL_PRICE), 0) SALES
                                          FROM  SA_SALES_INVOICE WHERE
                                        COMPANY_CODE IN({companyCode})
                                       AND SALES_DATE >= TO_DATE('{thisMonth.StartDateString}', 'YYYY-MM-DD') AND SALES_DATE <= TO_DATE('{thisMonth.EndDateString}', 'YYYY-MM-DD')),
                                        (SELECT NVL(SUM(CR_AMOUNT), 0) COLLECTION FROM  v$virtual_sub_ledger WHERE
                                           COMPANY_CODE IN({companyCode})
                                           AND VOUCHER_DATE >= TO_DATE('{thisMonth.StartDateString}', 'YYYY-MM-DD') AND VOUCHER_DATE <= TO_DATE('{thisMonth.EndDateString}', 'YYYY-MM-DD'))";
            var totalMtd = this._objectEntity.SqlQuery<TotalValue>(totalMtdQuery).FirstOrDefault();


            //this year
            string ytdQuery = $@"SELECT CS.PARTY_TYPE_EDESC DEALER_YTD, NVL(SUM(SI.CALC_QUANTITY),0) QUANTITY_YTD ,NVL(SUM(SI.CALC_TOTAL_PRICE),0) SALES_YTD,NVL(SUM(SL.CR_AMOUNT),0) COLLECTION_YTD,COUNT(SI.SALES_NO) BILLCOUNT_YTD
                                    FROM SA_SALES_INVOICE SI,IP_PARTY_TYPE_CODE CS,v$virtual_sub_ledger sl
                                    WHERE SI.PARTY_TYPE_CODE = CS.PARTY_TYPE_CODE
                                    AND SI.COMPANY_CODE = SL.COMPANY_CODE(+)
                                    AND SI.COMPANY_CODE=CS.COMPANY_CODE
                                     AND SI.PARTY_TYPE_CODE = SL.PARTY_TYPE_CODE(+)
                                    AND SI.SALES_DATE = SL.VOUCHER_DATE(+)
                                    AND SI.SALES_DATE >= TO_DATE('{thisYear.StartDateString}','YYYY-MM-DD') AND SI.SALES_DATE <= TO_DATE('{thisYear.EndDateString}','YYYY-MM-DD')                                            
                                    GROUP BY CS.PARTY_TYPE_EDESC
                                    ORDER BY SUM(SI.CALC_TOTAL_PRICE) DESC,CS.PARTY_TYPE_EDESC";
            var ytdData = this._objectEntity.SqlQuery<TopDealerListModel>(ytdQuery).Take(pageSize).ToList();
            string totalYtdQuery = $@"SELECT * FROM                   
                                        (SELECT NVL(SUM(CALC_TOTAL_PRICE),0) SALES
                                          FROM  SA_SALES_INVOICE WHERE 
                                        COMPANY_CODE IN({companyCode}) 
                                       AND SALES_DATE >= TO_DATE('{thisYear.StartDateString}','YYYY-MM-DD') AND SALES_DATE <= TO_DATE('{thisYear.EndDateString}','YYYY-MM-DD')),        
                                        (SELECT NVL(SUM(CR_AMOUNT),0) COLLECTION FROM  v$virtual_sub_ledger WHERE 
                                           COMPANY_CODE IN({companyCode})
                                           AND VOUCHER_DATE >= TO_DATE('{thisYear.StartDateString}','YYYY-MM-DD') AND VOUCHER_DATE <= TO_DATE('{thisYear.EndDateString}','YYYY-MM-DD'))";


            var totalYtd = this._objectEntity.SqlQuery<TotalValue>(totalYtdQuery).FirstOrDefault();

            for (int i = 0; i < ytdData.Count(); i++)
            {
                try
                {
                    ytdData[i].SALES_YTD = ytdData[i].SALES_YTD / totalYtd.Sales * 10000;
                    ytdData[i].COLLECTION_YTD = ytdData[i].COLLECTION_YTD / totalYtd.Collection * 10000;

                    ytdData[i].DEALER_MTD = mtdData[i].DEALER_MTD;
                    ytdData[i].QUANTITY_MTD = mtdData[i].QUANTITY_MTD;
                    ytdData[i].BILLCOUNT_MTD = mtdData[i].BILLCOUNT_MTD;
                    ytdData[i].CANCLE_ORDER_MTD = mtdData[i].CANCLE_ORDER_MTD;
                    ytdData[i].DUE_BALANCE_MTD = mtdData[i].DUE_BALANCE_MTD;
                    ytdData[i].DUE_DAYS_MTD = mtdData[i].DUE_DAYS_MTD;
                    ytdData[i].CREDIT_LIMIT_MTD = mtdData[i].CREDIT_LIMIT_MTD;
                    ytdData[i].EXCEED_MTD = mtdData[i].EXCEED_MTD;

                    ytdData[i].SALES_MTD = mtdData[i].SALES_MTD / totalMtd.Sales * 10000;
                    ytdData[i].COLLECTION_MTD = mtdData[i].COLLECTION_MTD / totalMtd.Collection * 10000;
                }
                catch (ArgumentOutOfRangeException)
                {
                    ytdData[i].DEALER_MTD = "-";
                }
                catch
                {

                }
            }



            return ytdData.Take(pageSize).ToList();
        }

        public List<RegionWiseSalesModel> GetRegionWiseSales()
        {
            var Query = @"SELECT RC.REGION_EDESC RegionName,NVL(SUM(SI.QUANTITY),0) Qty
                FROM REGION_CODE RC LEFT JOIN sa_customer_setup CS ON RC.REGION_CODE=CS.REGION_CODE,
                SA_SALES_INVOICE SI
                WHERE SI.CUSTOMER_CODE=CS.CUSTOMER_CODE
                GROUP BY RC.REGION_EDESC";
            var data = _objectEntity.SqlQuery<RegionWiseSalesModel>(Query).ToList();
            var total = data.Sum(x => x.Qty);
            for (int i = 0; i < data.Count; i++)
            {
                if (total > 0)
                {
                    var Per = (data[i].Qty / total) * 100;
                    data[i].SalesPercent = Per;
                }
                else
                    data[i].SalesPercent = 0;
            }
            return data;
        }
        public List<ProductWiseGpModel> GetProductWiseGp()
        {
            var Query = @"SELECT 
                             IGS.ITEM_GROUP_NAME AS Product,
                             NVL(SUM(SalesSummary.TotalQty), 0) AS Qty,
                             NVL(SUM(SalesSummary.TotalGross), 0) AS GrossAmount
                         FROM IP_ITEM_GROUP_SETUP IGS
                         LEFT JOIN IP_ITEM_GROUP_MAP IGM 
                             ON IGS.ITEM_GROUP_CODE = IGM.GROUP_CODE -- Fixed column name here
                         LEFT JOIN (
                             /* Pre-aggregating sales to improve performance */
                             SELECT 
                                 ITEM_CODE, 
                                 SUM(CALC_QUANTITY) AS TotalQty, 
                                 SUM(CALC_TOTAL_PRICE) AS TotalGross
                             FROM SA_SALES_INVOICE
                             GROUP BY ITEM_CODE
                         ) SalesSummary 
                             ON IGM.ITEM_CODE = SalesSummary.ITEM_CODE
                         WHERE IGS.DELETED_FLAG = 'N'
                         GROUP BY IGS.ITEM_GROUP_NAME";
            var data = _objectEntity.SqlQuery<ProductWiseGpModel>(Query).ToList();
            var total = data.Sum(x => x.GrossAmount);
            for (int i = 0; i < data.Count; i++)
            {
                if (total > 0)
                {
                    var Per = (data[i].GrossAmount / total) * 100;
                    data[i].GpPercent = Per;
                }
                else
                    data[i].GpPercent = 0;
            }
            return data;
        }


        public List<WeeklyExpenseAnalysis> GetWeeklyExpenseAnalysis(ReportFiltersModel model, User userInfo)
        {
            var companyCode = string.Empty;
            foreach (var company in model.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }
            //AND(GL.VOUCHER_DATE IS NULL OR GL.VOUCHER_DATE >= TO_DATE('" + model.FromDate + @"', 'YYYY-MM-DD'))
            //AND(GL.VOUCHER_DATE IS NULL OR GL.VOUCHER_DATE <= TO_DATE('" + model.ToDate + @"', 'YYYY-MM-DD'))
            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            var Query = $@"SELECT FAS.ACC_EDESC,GL.VOUCHER_DATE,NVL(GL.DR_AMOUNT,0) AMOUNT,FAS.MASTER_ACC_CODE, REPLACE(FAS.PRE_ACC_CODE,'00',NULL) PRE_ACC_CODE  FROM FA_CHART_OF_ACCOUNTS_SETUP FAS, FA_GENERAL_LEDGER GL
                            WHERE  FAS.ACC_CODE = GL.ACC_CODE (+)
                                        AND FAS.COMPANY_CODE = GL.COMPANY_CODE(+)
                                        AND FAS.DELETED_FLAG = 'N'
                                        AND FAS.ACC_NATURE IN('EA','EB','EC','ED','IE')                                          
                                        AND GL.TRANSACTION_TYPE(+) ='DR'  
                                        AND FAS.COMPANY_CODE IN ({companyCode})
                                        ORDER BY PRE_ACC_CODE "; 
            var data = _objectEntity.SqlQuery<WeeklyExpenseAnalysis>(Query).ToList();
            return data;
        }


        public List<WeeklyVendorPaymentAnalysis> GetWeeklyVendorPaymentAnalysis(ReportFiltersModel model,User userInfo)
        {
            var companyCode = string.Empty;
            foreach (var company in model.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }
            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            //AND(GL.VOUCHER_DATE IS NULL OR GL.VOUCHER_DATE >= TO_DATE('" + model.FromDate + @"', 'YYYY-MM-DD'))
            //AND(GL.VOUCHER_DATE IS NULL OR GL.VOUCHER_DATE <= TO_DATE('" + model.ToDate + @"', 'YYYY-MM-DD'))
            var Query = $@"SELECT FAS.ACC_EDESC,GL.VOUCHER_DATE,NVL(GL.DR_AMOUNT,0) AMOUNT,FAS.MASTER_ACC_CODE, REPLACE(FAS.PRE_ACC_CODE,'00',NULL) PRE_ACC_CODE  FROM FA_CHART_OF_ACCOUNTS_SETUP FAS, FA_GENERAL_LEDGER GL
                            WHERE  FAS.ACC_CODE = GL.ACC_CODE (+)
                                        AND FAS.COMPANY_CODE = GL.COMPANY_CODE(+)
                                        AND FAS.DELETED_FLAG = 'N'
                                        AND FAS.ACC_NATURE IN('LB','LD','LS')                                          
                                        AND GL.TRANSACTION_TYPE(+) ='DR'  
                                        AND FAS.COMPANY_CODE = ({companyCode})
                                        ORDER BY PRE_ACC_CODE ";
            var data = _objectEntity.SqlQuery<WeeklyVendorPaymentAnalysis>(Query).ToList();
            return data;
        }

        public List<WeeklyExpenseAnalysis> GetWeeklyExpenseAnalysis(User userInfo)
        {
            throw new NotImplementedException();
        }

        public IList<SalesAchieveModel> getSalesAchieveBranchProjection(ReportFiltersModel model, string duration, User userInfo)
        {
            var companyCode = string.Join(",", model.CompanyFilter);
            companyCode = companyCode == "" ? userInfo.company_code : companyCode;

            var branchFilter = string.Empty;
            if (model.BranchFilter.Count > 0)
            {
                branchFilter += string.Format(@" AND SI.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
            }

            var FincalYear = System.Configuration.ConfigurationManager.AppSettings["FiscalYear"].ToString();
            var dateRange = this._controlService.GetDateFilters(FincalYear).OrderByDescending(q => q.SortOrder).ToList();
            var dates = dateRange.Where(x => x.RangeName == duration).FirstOrDefault();
            var totalDays = (dates.EndDate - dates.StartDate).TotalDays;
            var todayDays = Math.Round((DateTime.Now - dates.StartDate).TotalDays,0);
            if (totalDays < todayDays)
                todayDays = totalDays;
            //need optimize query

            string mtdQuery = $@"SELECT DS.BRANCH_EDESC as branch_name, DS.BRANCH_CODE as branch_code,
                            fn_bs_month(substr(bs_date(si.sales_date),6,2)) AS Month,
                            SUBSTR(BS_DATE(SI.SALES_DATE),6,2) as MonthInt,
                           0 TargetQty,
                             0 TargetAmount,
                            SUM (nvl(si.calc_Quantity,0))/1 AS Quantity,
                            SUM (nvl(si.calc_total_price,0))/1 AS GrossAmount,
                            'SALES' DATATYPE
                    FROM sa_sales_invoice si, FA_BRANCH_SETUP DS
                    WHERE  1=1
                    AND si.deleted_flag = 'N' AND si.company_code IN ({companyCode}) {branchFilter}
                    and SI.BRANCH_CODE = DS.BRANCH_CODE
                   AND SI.COMPANY_CODE IN ({companyCode}) 
                     and SI.SALES_DATE >= TO_DATE('{dates.StartDateString}','YYYY-MM-DD') AND SI.SALES_DATE <= TO_DATE('{dates.EndDateString}','YYYY-MM-DD')
            GROUP BY DS.BRANCH_CODE, DS.BRANCH_EDESC, SUBSTR(BS_DATE(SI.SALES_DATE),6,2)
                    ,fn_bs_month(substr(bs_date(si.sales_date),6,2))
                    ,to_number(substr(bs_date(sales_date),6,2))
                    UNION  ALL
                    SELECT DS.BRANCH_EDESC as branch_name, DS.BRANCH_CODE as branch_code,
                            fn_bs_month(substr(bs_date(si.PLAN_DATE),6,2)) AS Month,
                            SUBSTR(BS_DATE(SI.PLAN_DATE),6,2) as MonthInt,
                            SUM (nvl(si.PER_DAY_QUANTITY,0))/1 AS TargetQty,
                            SUM (nvl(si.PER_DAY_AMOUNT,0))/1 AS TargetAmount,
                            0 Quantity,
                            0 GrossAmount,
                               'TARGET' DATATYPE
                    FROM PL_SALES_PLAN_DTL si, FA_BRANCH_SETUP DS
                    WHERE 1=1
                      and  (SI.ITEM_CODE IN (SELECT ITEM_CODE FROM ip_item_master_setup 
                                            WHERE GROUP_SKU_FLAG='I'
                                            AND COMPANY_CODE = Si.COMPANY_CODE) 
                                            OR ITEM_CODE = 0)
                    AND si.deleted_flag = 'N' AND si.company_code in({companyCode}) {branchFilter}
                    and SI.BRANCH_CODE = DS.BRANCH_CODE
                    and si.company_code=ds.company_code
                    AND SI.COMPANY_CODE IN ({companyCode}) 
                    AND SI.PLAN_DATE >= TO_DATE('{dates.StartDateString}','YYYY-MM-DD') AND SI.PLAN_DATE <= TO_DATE('{dates.EndDateString}','YYYY-MM-DD')
                    GROUP BY DS.BRANCH_CODE, DS.BRANCH_EDESC, SUBSTR(BS_DATE(SI.PLAN_DATE),6,2)
                    ,fn_bs_month(substr(bs_date(si.PLAN_DATE),6,2))
                    ,to_number(substr(bs_date(PLAN_DATE),6,2))";
            //this month
            //string mtdQuery = $@"SELECT BS.BRANCH_CODE, BS.BRANCH_EDESC, NVL(SUM(SI.CALC_TOTAL_PRICE),0) SALES_ACHIEVE,
            //                     NVL(SUM(SPD.PER_DAY_AMOUNT),1) TARGET
            //                    FROM SA_SALES_INVOICE SI,PL_SALES_PLAN_DTL SPD,FA_BRANCH_SETUP BS
            //                    WHERE  SI.SALES_DATE = SPD.PLAN_DATE(+)
            //                               AND SI.COMPANY_CODE = SPD.COMPANY_CODE(+)
            //                               AND SI.BRANCH_CODE = BS.BRANCH_CODE
            //                               AND SI.COMPANY_CODE = BS.COMPANY_CODE(+)  
            //                               AND SI.COMPANY_CODE IN ({companyCode}) {branchFilter}
            //                               AND SI.SALES_DATE >= TO_DATE('{dates.StartDateString}','YYYY-MM-DD') AND SI.SALES_DATE <= TO_DATE('{dates.EndDateString}','YYYY-MM-DD')
            //                    GROUP BY  BS.BRANCH_EDESC,BS.BRANCH_CODE
            //                    ORDER BY BS.BRANCH_EDESC";
            var datas = this._objectEntity.SqlQuery<SalesTargetViewModel>(mtdQuery).ToList();

            var branchwisesales = new List<SalesAchieveModel>();
            foreach (var data in datas.GroupBy(x => x.Branch_code).Select(y => y.FirstOrDefault()))
            {
                var branchWise = new SalesAchieveModel();
                branchWise.BRANCH_CODE = data.Branch_code;
                branchWise.BRANCH_EDESC = data.Branch_name;
                branchWise.SALES_ACHIEVE = Convert.ToDecimal(datas.Where(x => x.Branch_code == data.Branch_code && x.DataType == "SALES").Sum(X => X.GrossAmount));
                branchWise.TARGET = Convert.ToDecimal(datas.Where(X => X.Branch_code == data.Branch_code && X.DataType != "SALES").Sum(X => X.TargetAmount));
                branchwisesales.Add(branchWise);

                //var 
            }
            foreach (var days in branchwisesales)
            {
                var perDayAchived = days.SALES_ACHIEVE / Convert.ToDecimal(todayDays);
                var daysRemaining = totalDays - todayDays;
                var unachived = perDayAchived * Convert.ToDecimal(daysRemaining);
                var ProjectedSales = days.SALES_ACHIEVE + unachived;
                days.SALES_PROJECTED = Convert.ToDecimal(Convert.ToDecimal(ProjectedSales).ToString("0.00"));
                if (days.TARGET > 0)
                {
                    days.Projected = ((ProjectedSales / days.TARGET) * 100) ?? 0;
                }
                else
                {
                    days.Projected = ((ProjectedSales / ProjectedSales) * 100) ?? 0;
                }
            }

            return branchwisesales;
        }

        public IList<SalesAchieveProjectionModel> GetSalesAchieveBranchProjectionWithTarget(ReportFiltersModel model, string duration, User userInfo)
        {
            try
            {
                var companyCode = string.Join(",", model.CompanyFilter);
                companyCode = companyCode == "" ? userInfo.company_code : companyCode;

                var branchFilter = string.Empty;
                if (model.BranchFilter.Count > 0)
                {
                    branchFilter += string.Format(@" AND SI.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
                }

                var FincalYear = System.Configuration.ConfigurationManager.AppSettings["FiscalYear"].ToString();
                var dateRange = this._controlService.GetDateFilters(FincalYear).OrderByDescending(q => q.SortOrder).ToList();
                var dates = dateRange.Where(x => x.RangeName == duration).FirstOrDefault();
                var totalDays = (dates.EndDate - dates.StartDate).TotalDays;
                var todayDays = Math.Round((DateTime.Now - dates.StartDate).TotalDays, 0);
                if (totalDays < todayDays)
                    todayDays = totalDays;

                var query = $@"SELECT DS.BRANCH_EDESC as BRANCH_EDESC, DS.BRANCH_CODE as branch_code,
                            SUM (nvl(si.calc_Quantity,0))/1 AS Quantity,
                            SUM (nvl(si.calc_total_price,0))/1 AS Amount,
                              (SELECT SUM (nvl(PER_DAY_AMOUNT,0))/1
                                    FROM PL_SALES_PLAN_DTL
                                    WHERE (ITEM_CODE IN (SELECT ITEM_CODE FROM ip_item_master_setup  WHERE GROUP_SKU_FLAG='I' AND COMPANY_CODE = PL_SALES_PLAN_DTL.COMPANY_CODE)  OR ITEM_CODE = 0) 
                                    AND deleted_flag = 'N' AND company_code in({companyCode})
                                     and BRANCH_CODE = DS.BRANCH_CODE
                                     AND PLAN_DATE >= TO_DATE('{dates.StartDateString}','YYYY-MM-DD') AND PLAN_DATE <= TO_DATE('{dates.EndDateString}','YYYY-MM-DD')) AS Target,
                                      (SELECT SUM (nvl(PER_DAY_QUANTITY,0))/1
                                    FROM PL_SALES_PLAN_DTL 
                                    WHERE (ITEM_CODE IN (SELECT ITEM_CODE FROM ip_item_master_setup  WHERE GROUP_SKU_FLAG='I' AND COMPANY_CODE = PL_SALES_PLAN_DTL.COMPANY_CODE)  OR ITEM_CODE = 0) 
                                    AND deleted_flag = 'N' 
                                    AND company_code in({companyCode})
                                     and BRANCH_CODE = DS.BRANCH_CODE
                                     AND PLAN_DATE >= TO_DATE('{dates.StartDateString}','YYYY-MM-DD') AND PLAN_DATE <= TO_DATE('{dates.EndDateString}','YYYY-MM-DD'))  AS TARGET_QUANTITY,
                                            'SALES' DATATYPE
                                    FROM sa_sales_invoice si, FA_BRANCH_SETUP DS
                                    WHERE  1=1
                                    AND si.deleted_flag = 'N' 
                                    and SI.BRANCH_CODE = DS.BRANCH_CODE
                                   AND SI.COMPANY_CODE IN ({companyCode}) 
                                     and SI.SALES_DATE >= TO_DATE('{dates.StartDateString}','YYYY-MM-DD') AND SI.SALES_DATE <= TO_DATE('{dates.EndDateString}','YYYY-MM-DD')
                            GROUP BY DS.BRANCH_CODE, DS.BRANCH_EDESC";

                var datas = this._objectEntity.SqlQuery<SalesAchieveProjectionModel>(query).ToList();
                foreach (var data in datas)
                {
                    var perDayAchived = data.AMOUNT / Convert.ToDecimal(todayDays);
                    var daysRemaining = totalDays - todayDays;
                    var unachived = perDayAchived * Convert.ToDecimal(daysRemaining);
                    var ProjectedSales = data.AMOUNT + unachived;
                    data.PROJECTION = Convert.ToDecimal(Convert.ToDecimal(ProjectedSales).ToString("0.00"));

                    var perDayAchivedQty = data.QUANTITY / Convert.ToDecimal(todayDays);
                    var daysRemainingQty = totalDays - todayDays;
                    var unachivedQty = perDayAchived * Convert.ToDecimal(daysRemaining);
                    var ProjectedSalesQty = data.AMOUNT + unachived;
                    data.PROJECTION_QUANTITY = Convert.ToDecimal(Convert.ToDecimal(ProjectedSalesQty).ToString("0.00"));
                }
                return datas;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       public IList<SalesSummaryReportModel> GetSlowMovingItem(User userInfo, int selectedItem)
        {
            try
            {
                string selectQuery = $@"select SI.item_code as ITEM_CODE,sum(SI.calc_quantity) QUANTITY,I.ITEM_EDESC from SA_SALES_INVOICE SI,IP_ITEM_MASTER_SETUP I where I.ITEM_CODE=SI.ITEM_CODE AND SI.COMPANY_CODE=I.COMPANY_CODE AND SI.deleted_flag='N' AND I.category_code='FG' and SI.company_code='{userInfo.company_code}' group by SI.item_code,I.ITEM_EDESC
                                       ORDER BY QUANTITY ASC";
                var result = _objectEntity.SqlQuery<SalesSummaryReportModel>(selectQuery).Take(selectedItem).ToList();


                //string query = @"select * from(select row_number() over (order by QUANTITY ASC)as rn, i.ITEM_EDESC,  i.ITEM_CODE,t.QUANTITY from IP_ITEM_MASTER_SETUP i  join (select  ITEM_CODE,sum(calc_quantity) QUANTITY   from sa_sales_invoice group by ITEM_CODE) t on t.item_code=i.item_code ) where  rn <= 10 order by QUANTITY ASC ";
                //var datas = this._objectEntity.SqlQuery<SalesSummaryReportModel>(query).ToList();
                return result;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public IList<SalesSummaryReportModel> GetTopMovingItem(User userInfo, int selectedItem)
        {
            try
            {
                string selectQuery = $@"select SI.item_code as ITEM_CODE,sum(SI.calc_quantity) QUANTITY,I.ITEM_EDESC from SA_SALES_INVOICE SI,IP_ITEM_MASTER_SETUP I where I.ITEM_CODE=SI.ITEM_CODE AND SI.COMPANY_CODE=I.COMPANY_CODE AND SI.deleted_flag='N' and SI.company_code='{userInfo.company_code}' group by SI.item_code,I.ITEM_EDESC
                                       ORDER BY QUANTITY DESC";
                var result = _objectEntity.SqlQuery<SalesSummaryReportModel>(selectQuery).Take(selectedItem).ToList();


                //string query = @"select * from(select row_number() over (order by QUANTITY ASC)as rn, i.ITEM_EDESC,  i.ITEM_CODE,t.QUANTITY from IP_ITEM_MASTER_SETUP i  join (select  ITEM_CODE,sum(calc_quantity) QUANTITY   from sa_sales_invoice group by ITEM_CODE) t on t.item_code=i.item_code ) where  rn <= 10 order by QUANTITY ASC ";
                //var datas = this._objectEntity.SqlQuery<SalesSummaryReportModel>(query).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<SalesSummaryReportModel> GetNonMovingItem(User userInfo, int selectedItem)
        {
            try
            {
                string query = $@" select i.ITEM_EDESC,ip.QUANTITY,i.ITEM_CODE,ip.COMPANY_CODE ,i.category_code from IP_ITEM_MASTER_SETUP i left join (select ITEM_CODE,sum(calc_quantity) QUANTITY,COMPANY_CODE from SA_SALES_INVOICE group by ITEM_CODE,COMPANY_CODE) ip on i.ITEM_CODE=ip.ITEM_CODE 
                and I.COMPANY_CODE=ip.COMPANY_CODE  where i.category_code='FG' and ip.quantity is null and i.group_sku_flag='I'  and i.company_code='{userInfo.company_code}' order by i.ITEM_EDESC asc";
                //string selectQuery = $@"select SI.item_code as ITEM_CODE,sum(SI.calc_quantity) QUANTITY,I.ITEM_EDESC from SA_SALES_INVOICE SI,IP_ITEM_MASTER_SETUP I where I.ITEM_CODE=SI.ITEM_CODE AND SI.COMPANY_CODE=I.COMPANY_CODE AND SI.deleted_flag='N' and SI.company_code='{userInfo.company_code}' group by SI.item_code,I.ITEM_EDESC
                //                       ORDER BY QUANTITY ASC";
                var result = _objectEntity.SqlQuery<SalesSummaryReportModel>(query).Take(selectedItem).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<ProductLevelModel> GetProductLevel(User userInfo, int selectedItem)
        {
            try
            {
                string Query = @"SELECT SUM(QUANTITY) AS QUANTITY,PRODUCT FROM V_PRODUCT_STOCK GROUP BY PRODUCT";
                var result = _objectEntity.SqlQuery<ProductLevelModel>(Query).ToList();
                return result;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

    }
}
