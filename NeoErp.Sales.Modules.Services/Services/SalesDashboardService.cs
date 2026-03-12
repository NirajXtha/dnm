using NeoErp.Core;
using NeoErp.Core.Domain;
using NeoErp.Core.Helpers;
using NeoErp.Core.Models;
using NeoErp.Core.Plugins;
using NeoErp.Core.Services;
using NeoErp.Sales.Modules.Services.Models;
using NeoErp.Sales.Modules.Services.Models.Division;
using NeoErp.Sales.Modules.Services.Models.SalesDashBoard;
using NeoErp.Sales.Modules.Services.Models.Stock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NeoErp.Sales.Modules.Services.Services
{
    public class SalesDashboardService : ISalesDashboardService
    {
        private NeoErpCoreEntity _objectEntity;
        private IControlService _controlService;
        private IWorkContext _workContext;
        private readonly IPluginFinder _pluginFinder;
        public SalesDashboardService(NeoErpCoreEntity objectEntity, IWorkContext workContext, IControlService controlService, IPluginFinder pluginFinder)
        {
            this._objectEntity = objectEntity;
            this._controlService = controlService;
            this._workContext = workContext;
            this._pluginFinder = pluginFinder;
        }
        public List<MonthlySalesGraph> GetSalesMonthSummanry(ReportFiltersModel reportFilters, User userInfo)
        {
            if (userInfo == null)
            {
                userInfo = new Core.Domain.User();
                userInfo.company_code = "01";
                userInfo.branch_code = "01.01";

            }
            else if (string.IsNullOrEmpty(userInfo.company_code) || string.IsNullOrEmpty(userInfo.branch_code))
            {
                userInfo.company_code = "01";
                userInfo.branch_code = "01.01";
            }
            string Query = string.Format(@"select to_char(sales_date,'fmMonth') as   Month , 
                             SUM (nvl(calc_total_price,0))/{0} AS Amount,                             
                             SUM (nvl(calc_Quantity,0))/{0} as Quantity
                            from sa_sales_invoice
                             where company_code = '{1}'
                            group by to_char(sales_date, 'fmMonth')
                            order by 1", ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), userInfo.company_code, userInfo.branch_code);
            var salesReportList = _objectEntity.SqlQuery<MonthlySalesGraph>(Query).ToList();
            return salesReportList;
        }
        public List<DaysSalesGraph> GetDailySalesSummary(ReportFiltersModel reportFilters, User userInfo, string DateFormat)
        {
            if (userInfo == null)
            {
                userInfo = new Core.Domain.User();
                userInfo.company_code = "01";
                userInfo.branch_code = "01.01";

            }
            else if (string.IsNullOrEmpty(userInfo.company_code) || string.IsNullOrEmpty(userInfo.branch_code))
            {
                userInfo.company_code = "01";
                userInfo.branch_code = "01.01";
            }
            //string Query =string.Format(@"SELECT 
            //        TO_CHAR(sales_date, 'DD') day, 
            //        TO_CHAR(sales_date, 'MM') MonthNo ,
            //        TO_CHAR(sales_date, 'Month') MonthEnglish, 
            //        fn_bs_month(substr(bs_date(sales_date),6,2)) AS Nepalimonth,
            //        SUM (nvl(calc_total_price,0))/{0} as Amount,
            //        SUM (nvl(calc_Quantity,0))/{0} as Quantity
            //    FROM sa_sales_invoice
            //     where company_code = '{1}' and branch_code = '{2}'
            //    GROUP by TO_CHAR(sales_date, 'DD') , 
            //    TO_CHAR(sales_date, 'MM') ,
            //    fn_bs_month(substr(bs_date(sales_date),6,2)),
            //    TO_CHAR(sales_date, 'Month') order by MonthNo,day", ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), userInfo.company_code, userInfo.branch_code);
            string Query = string.Empty;
            if (string.IsNullOrEmpty(DateFormat) || string.Equals(DateFormat, "AD"))
            {
                Query = string.Format(@"SELECT 
                TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'DD') day, 
                TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MM') MonthNo ,
                TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'Month') MonthEnglish, 
                fn_bs_month(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2)) AS Nepalimonth,
                (select nvl(SUM(nvl(calc_total_price,0))/{0},0) as Amount FROM sa_sales_invoice
                 where company_code = '{1}' and TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'DD')=TO_CHAR(sales_date, 'DD')) as Amount,
                (select nvl(SUM(nvl(calc_Quantity,0))/{0},0) as Quantity FROM sa_sales_invoice
                 where company_code = '{1}' and TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'DD')=TO_CHAR(sales_date, 'DD')) as Quantity
            FROM all_objects,
               (SELECT start_date, end_date
                  FROM HR_FISCAL_YEAR_CODE
                 WHERE sysdate BETWEEN start_date AND end_date) fiscal_year
            WHERE ROWNUM <= fiscal_year.end_date - fiscal_year.start_date + 1
            group by 
            TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'DD'), 
                TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MM') ,
                TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'Month'), 
                fn_bs_month(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2))",
                ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), userInfo.company_code);
            }
            else if (string.Equals(DateFormat, "BS"))
            {
                Query = string.Format(@"SELECT 
                to_char(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),9,2)) day,
                --TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'DD') day, 
                TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MM') MonthNo ,
                TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'Month') MonthEnglish, 
                fn_bs_month(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2)) AS Nepalimonth,
                (select nvl(SUM(nvl(calc_total_price,0))/{0},0) as Amount FROM sa_sales_invoice
                 where company_code = '{1}' and TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'DD')=TO_CHAR(sales_date, 'DD')) as Amount,
                (select nvl(SUM(nvl(calc_Quantity,0))/{0},0) as Quantity FROM sa_sales_invoice
                 where company_code = '{1}' and TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'DD')=TO_CHAR(sales_date, 'DD')) as Quantity
            FROM all_objects,
               (SELECT start_date, end_date
                  FROM HR_FISCAL_YEAR_CODE
                 WHERE sysdate BETWEEN start_date AND end_date) fiscal_year
            WHERE ROWNUM <= fiscal_year.end_date - fiscal_year.start_date + 1
            group by 
            TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'DD'), 
                TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MM') ,
                TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'Month'), 
                fn_bs_month(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2)),
                to_char(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),9,2))",
                ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), userInfo.company_code);
            }
            var salesReportList = _objectEntity.SqlQuery<DaysSalesGraph>(Query).ToList();
            return salesReportList;
        }
        public List<DaysSalesGraph> GetDailySalesSummary(ReportFiltersModel reportFilters, User userInfo, string month, string DateFormat, bool salesReturn)
        {
            //var companyCode = string.Join(",", reportFilters.CompanyFilter);
            //companyCode = companyCode == "" ? userInfo.company_code : companyCode;
            //var companyCode = string.Join(",'", reportFilters.CompanyFilter);
            var companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }
            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            string Query = string.Empty;
            if (string.Equals(DateFormat.ToLower(), "ad"))
            {
                Query = @"select to_char(SI.sales_date, 'DD') as day,
                                TO_CHAR(SI.sales_date, 'Month') MonthEnglish,
                                nvl(sum(nvl(SI.TOTAL_SALES,0))/{0},0)as TotalAmount,  
                                nvl(sum(nvl(SI.GROSS_SALES,0))/{0},0)as GrossAmount,                           
                                sum(nvl(SI.Quantity,0))/{0} as Quantity,    
                                nvl(sum(nvl(SI.NET_SALES,0))/{0},0)as NETAmount                                                          
                                                from V$SALES_INVOICE_REPORT3 SI
                                                where SI.deleted_flag='N'                                                
                                                AND TO_CHAR(SI.sales_date, 'YYYYMM') = '{2}'
                                                AND SI.COMPANY_CODE IN ({1}) ";
            }
            else
            {
                Query = @"select to_char(substr(bs_date(SI.sales_date),9,2)) day,        
                                trim(substr(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SI.sales_date),6,2)),5,20))  Nepalimonth, 
                                  nvl(sum(nvl(SI.TOTAL_SALES,0))/{0},0)as TotalAmount,  
                                  nvl(sum(nvl(SI.GROSS_SALES,0))/{0},0)as GrossAmount,                           
                                  sum(nvl(SI.Quantity,0))/{0} as Quantity,    
                                  nvl(sum(nvl(SI.NET_SALES,0))/{0},0)as NETAmount     
                                 from V$SALES_INVOICE_REPORT3 SI
                                 where SI.deleted_flag='N'                                
                                 AND SI.COMPANY_CODE IN ({1})
                                 AND SUBSTR(BS_DATE(SI.sales_date),6,2) = '{2}' ";
            }


            //****************************
            //CONDITIONS FITLER START HERE
            //****************************


            //for sales return
            if (salesReturn)
                Query += " and SI.TABLE_NAME IN('SALES','SALES_RETURN')";
            else
                Query += " and SI.TABLE_NAME IN('SALES')";


            //for customer Filter
            var customerFilter = string.Empty;
            if (reportFilters.CustomerFilter.Count() > 0)
            {
                customerFilter = @"select  DISTINCT(customer_code) from sa_customer_setup where (";
                //IF CUSTOMER_SKU_FLAG = G
                foreach (var item in reportFilters.CustomerFilter)
                {
                    customerFilter += "master_customer_code like  (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                customerFilter = customerFilter.Substring(0, customerFilter.Length - 3);
                //IF CUSTOMER_SKU_FLAG = I                
                customerFilter += " or (customer_code in (" + string.Join(",", reportFilters.CustomerFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                Query += " and SI.customer_code IN(" + customerFilter + ")";
            }




            ////for Product Filter
            var productFilter = string.Empty;
            if (reportFilters.ProductFilter.Count() > 0)
            {
                productFilter = @"select  DISTINCT item_code from IP_ITEM_MASTER_SETUP where (";
                //IF PRODUCT_SKU_FLAG = G
                foreach (var item in reportFilters.ProductFilter)
                {
                    productFilter += "MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                productFilter = productFilter.Substring(0, productFilter.Length - 3);
                //IF PRODUCT_SKU_FLAG = I                
                productFilter += " or (ITEM_CODE in (" + string.Join(",", reportFilters.ProductFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                Query += " and SI.ITEM_CODE IN(" + productFilter + ")";
            }


            if (reportFilters.BranchFilter.Count > 0)
            {
                Query += string.Format(@" AND SI.BRANCH_CODE IN ('{0}')", string.Join("','", reportFilters.BranchFilter).ToString());
            }
            if (reportFilters.EmployeeFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SI.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", reportFilters.EmployeeFilter).ToString());
            }
            if (reportFilters.AgentFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SI.AGENT_CODE IN  ('{0}')", string.Join("','", reportFilters.AgentFilter).ToString());
            }
            if (reportFilters.DivisionFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SI.DIVISION_CODE IN  ('{0}')", string.Join("','", reportFilters.DivisionFilter).ToString());
            }
            string locationFilter = string.Empty;
            if (reportFilters.LocationFilter.Count > 0)
            {

                var locations = reportFilters.LocationFilter;
                for (int i = 0; i < locations.Count; i++)
                {

                    if (i == 0)
                        locationFilter += string.Format("SELECT LOCATION_CODE FROM IP_LOCATION_SETUP WHERE LOCATION_CODE LIKE '{0}%' ", locations[i]);
                    else
                    {
                        locationFilter += string.Format(" OR LOCATION_CODE like '{0}%' ", locations[i]);
                    }
                }
                locationFilter = string.Format(@" AND SI.FROM_LOCATION_CODE IN ({0} OR LOCATION_CODE IN ('{1}'))", locationFilter, string.Join("','", locations));
                //query = query.AppendFormat(@" AND A.FROM_LOCATION_CODE IN ('{0}')", string.Join("','", filters.LocationFilter).ToString());
                Query = Query + locationFilter;
            }
            //****************************
            //CONDITIONS FITLER END HERE
            //****************************

            if (string.Equals(DateFormat.ToLower(), "ad"))
            {
                Query += @" 
                           group by  to_char(SI.sales_date, 'Month'),to_char(SI.sales_date, 'DD')
                           order by to_char(SI.sales_date, 'DD')";
            }
            else
            {
                Query += @" 
                             group by  FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SI.Sales_date),6,2)),substr(bs_date(SI.sales_date),9,2)
                            ORDER BY substr(bs_date(SI.sales_date),9,2)  ";
            }

            Query = string.Format(Query, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode, month);
            var datas = _objectEntity.SqlQuery<DaysSalesGraph>(Query).ToList();
            return datas;



            //if (userInfo == null)
            //{
            //    userInfo = new Core.Domain.User();
            //    userInfo.company_code = "01";
            //    userInfo.branch_code = "01.01";

            //}
            //else if (string.IsNullOrEmpty(userInfo.company_code) || string.IsNullOrEmpty(userInfo.branch_code))
            //{
            //    userInfo.company_code = "01";
            //    userInfo.branch_code = "01.01";
            //}
            //string ad_bs_select = "TO_CHAR(SI.sales_date, 'DD') day, ";
            //string ad_bs_select_new = "TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'DD') day, ";
            //string ad_bs_group = "TO_CHAR(sales_date, 'DD') , ";
            //string ad_bs_group_new= "TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'DD'),";
            //if (string.Equals(DateFormat, "BS"))
            //{
            //    ad_bs_select = "to_char(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),9,2)) day,";
            //    ad_bs_select_new = "to_char(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),9,2)) day,";
            //    ad_bs_group = "TO_CHAR(sales_date, 'DD') ,to_char(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),9,2)), ";
            //    ad_bs_group_new = "TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'DD'),to_char(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),9,2)),";
            //}

            //string Query = @"SELECT 
            //         "+
            //        ad_bs_select+
            //        @"TO_CHAR(SI.sales_date, 'MM') MonthNo ,
            //        TO_CHAR(SI.sales_date, 'Month') MonthEnglish, 
            //        fn_bs_month(substr(bs_date(SI.sales_date),6,2)) AS Nepalimonth,
            //        SUM (nvl(SI.calc_total_price,0))/{0} as Amount,
            //        SUM (nvl(SI.calc_Quantity,0))/{0} as Quantity
            //    FROM sa_sales_invoice SI,SA_CUSTOMER_SETUP CS,IP_ITEM_MASTER_SETUP IM
            //    WHERE  SI.CUSTOMER_CODE = CS.CUSTOMER_CODE AND  IM.ITEM_CODE = SI.ITEM_CODE
            //    AND SI.COMPANY_CODE = '{1}'";
            //string QueryNew = @"SELECT 
            //    "+
            //    ad_bs_select_new+
            //    @"TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MM') MonthNo ,
            //    TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'Month') MonthEnglish, 
            //    fn_bs_month(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2)) AS Nepalimonth,";
            //string subQueryAmount = @"(select nvl(SUM (nvl(SI.calc_total_price,0))/{0},0) FROM sa_sales_invoice SI,SA_CUSTOMER_SETUP CS,IP_ITEM_MASTER_SETUP IM
            //    WHERE SI.CUSTOMER_CODE = CS.CUSTOMER_CODE AND IM.ITEM_CODE = SI.ITEM_CODE
            //    AND SI.COMPANY_CODE = '{1}' and TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'DD')= TO_CHAR(SI.sales_date, 'DD') ";
            //string subQueryQuantity = @"(select nvl(SUM (nvl(SI.calc_Quantity,0))/{0},0) FROM sa_sales_invoice SI,SA_CUSTOMER_SETUP CS,IP_ITEM_MASTER_SETUP IM
            //    WHERE  SI.CUSTOMER_CODE = CS.CUSTOMER_CODE AND  IM.ITEM_CODE = SI.ITEM_CODE
            //    AND SI.COMPANY_CODE = '{1}' and TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'DD')=TO_CHAR(SI.sales_date, 'DD') ";

            //string QueryNewEnd = @"FROM all_objects,
            //       (SELECT start_date, end_date
            //          FROM HR_FISCAL_YEAR_CODE
            //         WHERE sysdate BETWEEN start_date AND end_date) fiscal_year
            //    WHERE ROWNUM <= fiscal_year.end_date - fiscal_year.start_date + 1
            //    group by 
            //        "+
            //        ad_bs_group_new+
            //        @"TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MM'),
            //        TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'Month'), 
            //        fn_bs_month(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2))";
            //string condition = string.Empty;

            ////for customer Filter
            //if (!string.IsNullOrEmpty(customerCode))
            //{
            //    condition += " and (";
            //    Query += " and (";
            //    foreach (var item in customerCode.Split(','))
            //    {
            //        Query += "cs.master_customer_code like  (Select MASTER_CUSTOMER_CODE || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND COMPANY_CODE='" + userInfo.company_code + "' ) OR ";
            //        condition += "cs.master_customer_code like  (Select MASTER_CUSTOMER_CODE || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND COMPANY_CODE='" + userInfo.company_code + "' ) OR ";
            //    }
            //    Query = Query.Substring(0, Query.Length - 3) + ") ";
            //    condition = condition.Substring(0, condition.Length - 3) + ") ";
            //}

            ////for item Filter
            //if (!string.IsNullOrEmpty(itemCode))
            //{
            //    Query += " and (";
            //    condition += " and (";
            //    foreach (var item in itemCode.Split(','))
            //    {
            //        Query += "IM.MASTER_ITEM_CODE LIKE (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND COMPANY_CODE='" + userInfo.company_code + "') OR ";
            //        condition += "IM.MASTER_ITEM_CODE LIKE (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND COMPANY_CODE='" + userInfo.company_code + "') OR ";
            //    }
            //    Query = Query.Substring(0, Query.Length - 3) + ") ";
            //    condition = condition.Substring(0, condition.Length - 3) + ") ";
            //}


            ////for category Filter
            //if (!string.IsNullOrEmpty(categoryCode))
            //{
            //    Query += " and (";
            //    condition += " and (";
            //    foreach (var item in categoryCode.Split(','))
            //    {
            //        Query += "IM.CATEGORY_CODE = '" + item + "' OR ";
            //        condition += "IM.CATEGORY_CODE = '" + item + "' OR ";
            //    }
            //    Query = Query.Substring(0, Query.Length - 3) + ") ";
            //    condition = condition.Substring(0, condition.Length - 3) + ") ";
            //}

            ////FOR COMPANY FILTER
            //if (!string.IsNullOrEmpty(companyCode))
            //{
            //    Query += " AND SI.COMPANY_CODE IN (" + companyCode + ")";
            //    condition += " AND SI.COMPANY_CODE IN (" + companyCode + ")";
            //}


            ////FOR BRANCH FILTER
            //if (reportFilters.BranchFilter.Count() > 0)
            //{
            //    Query += " AND SI.BRANCH_CODE IN (" + string.Join(",", reportFilters.BranchFilter) + ")";
            //    condition += " AND SI.BRANCH_CODE IN (" + string.Join(",", reportFilters.BranchFilter) + ")";
            //    try { userInfo.company_code = reportFilters.BranchFilter[0].Split('.')[0]; } catch { }

            //}            

            ////FOR partyType FILTER
            //if (!string.IsNullOrEmpty(partyTypeCode))
            //{
            //    Query += " AND SI.PARTY_TYPE_CODE IN (" + partyTypeCode + ")";
            //    condition += " AND SI.PARTY_TYPE_CODE IN (" + partyTypeCode + ")";
            //}

            ////FOR FORMCODE FILER
            //if (!string.IsNullOrEmpty(formCode))
            //{
            //    Query += " AND SI.FORM_CODE IN (" + formCode + ")";
            //    condition += " AND SI.FORM_CODE IN (" + formCode + ")";
            //}

            //Query += @" GROUP by "+
            //    ad_bs_group+
            //    @"TO_CHAR(sales_date, 'MM') ,
            //    fn_bs_month(substr(bs_date(sales_date),6,2)),
            //    TO_CHAR(sales_date, 'Month') order by MonthNo,day";

            //Query = QueryNew + subQueryAmount + ") as Amount, " + subQueryQuantity + ") as Quantity " + QueryNewEnd;
            //Query = string.Format(Query, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), userInfo.company_code, branchCode);
            //var salesReportList = _objectEntity.SqlQuery<DaysSalesGraph>(Query).ToList();
            //return salesReportList;
        }

        public List<BranchDaysSalesGraph> GetDailyBranchSalesSummary(ReportFiltersModel reportFilters, User userInfo, bool salesReturn)
        {
            if (userInfo == null)
            {
                userInfo = new Core.Domain.User();
                userInfo.company_code = "01";
                userInfo.branch_code = "01.01";

            }
            else if (string.IsNullOrEmpty(userInfo.company_code) || string.IsNullOrEmpty(userInfo.branch_code))
            {
                userInfo.company_code = "01";
                userInfo.branch_code = "01.01";
            }
            //string Query = string.Format(@"SELECT si.branch_code,
            //                BS.BRANCH_EDESC as BranchName,
            //                 TO_CHAR(sales_date, 'DD') day, TO_CHAR(sales_date, 'MM') month ,
            //                 fn_bs_month(substr(bs_date(sales_date),6,2)) AS Nepalimonth,
            //                 TO_CHAR(sales_date, 'Month') Monthenglish,
            //                 SUM (nvl(si.calc_total_price,0))/{0} AS Amount,
            //                 SUM (nvl(si.calc_Quantity,0))/{0} AS Quantity
            //            FROM sa_sales_invoice si, fa_branch_setup bs
            //           WHERE si.deleted_flag = 'N' AND si.company_code = '{1}'
            //           and SI.BRANCH_CODE = BS.BRANCH_CODE
            //        GROUP BY si.branch_code,BS.BRANCH_EDESC, 
            //        TO_CHAR(sales_date, 'DD') , 
            //        TO_CHAR(sales_date, 'MM') ,
            //        fn_bs_month(substr(bs_date(sales_date),6,2)),
            //        TO_CHAR(sales_date, 'Month')", ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), userInfo.company_code, userInfo.branch_code);

            string Query = string.Format(@"select
                    bs.branch_code,
                    bs.BRANCH_EDESC as BranchName,
                    TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'DD') day, 
                    TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MM') month ,
                    fn_bs_month(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2)) AS Nepalimonth,
                    TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'Month') Monthenglish,
                    (SELECT nvl(SUM (nvl(si.calc_total_price,0))/{0},0) AS Amount
                    FROM sa_sales_invoice si
                        WHERE si.deleted_flag = 'N'  AND si.company_code = '{1}'
                        and SI.BRANCH_CODE = bs.BRANCH_CODE
                        and TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'Month')=TO_CHAR(sales_date, 'Month')) as Amount,
                    (select nvl(SUM (nvl(si.calc_Quantity,0))/{0},0) AS Quantity
                        FROM sa_sales_invoice si
                        WHERE si.deleted_flag = 'N'  AND si.company_code = '{1}'
                        and SI.BRANCH_CODE = bs.BRANCH_CODE
                        and TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'Month')=TO_CHAR(sales_date, 'Month')) as Quantity
                FROM all_objects, fa_branch_setup bs,
                   (SELECT start_date, end_date
                      FROM HR_FISCAL_YEAR_CODE
                     WHERE sysdate BETWEEN start_date AND end_date) fiscal_year
                WHERE ROWNUM <= fiscal_year.end_date - fiscal_year.start_date + 1
                group by 
                    bs.branch_code,
                    bs.BRANCH_EDESC,
                    TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'DD'), 
                    TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MM'),
                    fn_bs_month(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2)),
                    TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'Month')",
                    ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), userInfo.company_code);
            var salesReportList = _objectEntity.SqlQuery<BranchDaysSalesGraph>(Query).ToList();
            return salesReportList;
        }
        public List<BranchDaysSalesGraph> GetDailyBranchSalesSummary(ReportFiltersModel reportFilters, User userInfo, string monthName, string branchName,
            string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode,
            string partyTypeCode, string formCode, string DateFormat, bool salesReturn, string AmountType)
        {
            //companyCode = string.Join(",", reportFilters.CompanyFilter);
            //var companyCode = string.Join(",'", reportFilters.CompanyFilter);
            companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            //   companyCode = companyCode == "" ? userInfo.company_code : companyCode;
            string saleReturnQuery = salesReturn ? "'SALES','SALES_RETURN'" : "'SALES'";
            var Query = string.Empty;
            var QueryBS = string.Empty;
            var QueryNew = string.Empty;
            var QueryNewBS = string.Empty;

            Query = string.Format(@"select trim(substr(FN_CHARTAD_MONTH(SUBSTR(sales_date,4,3)),2,12)) MONTH,                            
                           TO_CHAR(sales_date,'MM') MonthNo,    
                           TO_CHAR(sales_date,'DD') DAY,        
                          SUM (nvl(sl.calc_total_price,0))/{0} AS GrossAmount,        
                          SUM (nvl(sl.calc_quantity,0))/1 AS Quantity,                         
                          bs.branch_edesc as BranchName 
                          from sa_sales_invoice  sl ,fa_branch_setup bs,SA_CUSTOMER_SETUP CS,IP_ITEM_MASTER_SETUP IM
                                            where sl.branch_code=bs.branch_code AND
                                            sl.CUSTOMER_CODE = CS.CUSTOMER_CODE AND
                                            IM.ITEM_CODE = sl.ITEM_CODE
                                            and sl.deleted_flag='N'
                                            and sl.company_code IN({1})                                            
                                            and TO_CHAR(sales_date,'MM') = '{2}'
                                            and sl.branch_code = '{3}'"
                              , ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode, monthName, branchName);

            QueryBS = string.Format(@"select trim(substr(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SALES_DATE),6,2)),6,12)) MONTH,                                                
                                         SUBSTR(BS_DATE(SALES_DATE),6,2) MonthNo,
                                         SUBSTR(BS_DATE(SALES_DATE),9,2) Day,
                                         SUM (nvl(sl.calc_total_price,0))/{0} AS GrossAmount,        
                                          SUM (nvl(sl.calc_quantity,0))/1 AS Quantity,                                        
                                          bs.branch_edesc as BranchName,
                                          trim(substr((BS_DATE(SALES_DATE)),0,7)) BsYear_Month 
                                          from sa_sales_invoice  sl ,fa_branch_setup bs,SA_CUSTOMER_SETUP CS,IP_ITEM_MASTER_SETUP IM
                                            where sl.branch_code=bs.branch_code
                                            AND sl.CUSTOMER_CODE = CS.CUSTOMER_CODE
                                            AND IM.ITEM_CODE = sl.ITEM_CODE
                                            and sl.deleted_flag='N'
                                            and sl.company_code IN({1})
                                            and SUBSTR(BS_DATE(SALES_DATE),6,2) = '{2}'
                                            and sl.branch_code = '{3}'",
                                             ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode, monthName, branchName);

            QueryNew = string.Format(@"select trim(substr(FN_CHARTAD_MONTH(SUBSTR(sales_date,4,3)),2,12)) MONTH,
                 TO_CHAR(sales_date,'MM') MonthNo,
                 TO_CHAR(sales_date,'DD') DAY,
                 SUM (nvl(sl.total_sales,0))/1 AS TotalAmount,
                 SUM (nvl(sl.gross_sales,0))/1 AS GrossAmount,
                 SUM (nvl(sl.Net_sales,0))/1 AS NetAmount,
                 SUM (nvl(sl.quantity,0))/1 AS Quantity,                                       
                 bs.branch_edesc as BranchName
                 from V$SALES_INVOICE_REPORT3 sl,fa_branch_setup bs --, SA_CUSTOMER_SETUP CS
                        where sl.branch_code=bs.branch_code
                        and sl.deleted_flag='N'
                        and sl.company_code IN({0})
                        and TO_CHAR(sales_date,'MM') = '{1}'
                        and sl.branch_code = '{2}'
                        and sl.TABLE_NAME IN ({3})", companyCode, monthName, branchName, saleReturnQuery);

            QueryNewBS = string.Format(@"select trim(substr(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SALES_DATE),6,2)),6,12)) MONTH,                                                
                 SUBSTR(BS_DATE(SALES_DATE),6,2) MonthNo,
                 SUBSTR(BS_DATE(SALES_DATE),9,2) Day,
                 SUM (nvl(sl.total_sales,0))/1 AS TotalAmount,
                 SUM (nvl(sl.gross_sales,0))/1 AS GrossAmount,
                 SUM (nvl(sl.Net_sales,0))/1 AS NetAmount,
                 SUM (nvl(sl.quantity,0))/1 AS Quantity,                                       
                 bs.branch_edesc as BranchName,
                 trim(substr((BS_DATE(SALES_DATE)),0,7)) BsYear_Month 
                 from V$SALES_INVOICE_REPORT3  sl ,fa_branch_setup bs --, SA_CUSTOMER_SETUP CS
                    where sl.branch_code=bs.branch_code
                    and sl.deleted_flag='N'
                    and sl.company_code IN({0})
                    and SUBSTR(BS_DATE(SALES_DATE),6,2) = '{1}'
                    and sl.branch_code = '{2}'
                    and sl.TABLE_NAME IN ({3})", companyCode, monthName, branchName, saleReturnQuery);


            //****************************
            //CONDITIONS FITLER START HERE
            //****************************
            string Condition = string.Empty;
            string ConditionNew = string.Empty;
            //for customer Filter
            if (reportFilters.CustomerFilter.Count() > 0)
            {
                Condition += " and (";
                //IF CUSTOMER_SKU_FLAG = G
                foreach (var item in reportFilters.CustomerFilter)
                {
                    Condition += "cs.master_customer_code like  (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND COMPANY_CODE IN(" + companyCode + ") ) OR ";
                }
                //IF CUSTOMER_SKU_FLAG = I                
                Condition += "(cs.CUSTOMER_CODE IN (" + string.Join(",", reportFilters.CustomerFilter) + ") AND cs.GROUP_SKU_FLAG = 'I' AND cs.COMPANY_CODE IN(" + companyCode + ") )) ";

                Condition = Condition.Substring(0, Condition.Length - 1);
                ConditionNew = string.Equals(DateFormat, "BS") ? Condition : "";
            }

            ////for Product Filter
            if (reportFilters.ProductFilter.Count() > 0)
            {
                Condition += " and (";
                //IF PRODUCT_SKU_FLAG = G
                foreach (var item in reportFilters.ProductFilter)
                {
                    Condition += "IM.MASTER_ITEM_CODE like  (Select DISTINCT(MASTER_ITEM_CODE) || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND COMPANY_CODE IN(" + companyCode + ") ) OR ";
                }
                //IF PRODUCT_SKU_FLAG = I                
                Condition += "(IM.ITEM_CODE IN (" + string.Join(",", reportFilters.ProductFilter) + ") AND IM.GROUP_SKU_FLAG = 'I')) ";

                Condition = Condition.Substring(0, Condition.Length - 1);
            }


            //for category Filter
            if (reportFilters.CategoryFilter.Count() > 0)
            {
                Condition += " and (";
                foreach (var item in reportFilters.CategoryFilter)
                {
                    Condition += "IM.CATEGORY_CODE = '" + item + "' OR ";
                }
                Condition = Condition.Substring(0, Condition.Length - 3) + ") ";
            }



            //if (reportFilters.BranchFilter.Count > 0)
            //{
            //    Condition += string.Format(@" AND sl.BRANCH_CODE IN ('{0}')", string.Join("','", reportFilters.BranchFilter).ToString());
            //    ConditionNew += string.Format(@" AND sl.BRANCH_CODE IN ('{0}')", string.Join("','", reportFilters.BranchFilter).ToString());
            //}
            if (reportFilters.EmployeeFilter.Count > 0)
            {
                Condition = Condition + string.Format(@" AND sl.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", reportFilters.EmployeeFilter).ToString());
            }
            if (reportFilters.AgentFilter.Count > 0)
            {
                Condition = Condition + string.Format(@" AND sl.AGENT_CODE IN  ('{0}')", string.Join("','", reportFilters.AgentFilter).ToString());
            }
            if (reportFilters.DivisionFilter.Count > 0)
            {
                Condition = Condition + string.Format(@" AND sl.DIVISION_CODE IN  ('{0}')", string.Join("','", reportFilters.DivisionFilter).ToString());
                ConditionNew += string.Format(@" AND sl.DIVISION_CODE IN  ('{0}')", string.Join("','", reportFilters.DivisionFilter).ToString());
            }
            string locationFilter = string.Empty;
            if (reportFilters.LocationFilter.Count > 0)
            {

                var locations = reportFilters.LocationFilter;
                for (int i = 0; i < locations.Count; i++)
                {

                    if (i == 0)
                        locationFilter += string.Format("SELECT LOCATION_CODE FROM IP_LOCATION_SETUP WHERE LOCATION_CODE LIKE '{0}%' ", locations[i]);
                    else
                    {
                        locationFilter += string.Format(" OR LOCATION_CODE like '{0}%' ", locations[i]);
                    }
                }
                locationFilter = string.Format(@" AND sl.FROM_LOCATION_CODE IN ({0} OR LOCATION_CODE IN ('{1}'))", locationFilter, string.Join("','", locations));
                //query = query.AppendFormat(@" AND A.FROM_LOCATION_CODE IN ('{0}')", string.Join("','", filters.LocationFilter).ToString());
                Condition = Condition + locationFilter;
            }

            //****************************
            //CONDITIONS FITLER END HERE
            //****************************

            Query += Condition + @"
                         group by sl.branch_code,bs.branch_edesc,FN_CHARTAD_MONTH(SUBSTR(sales_date,4,3)),TO_CHAR(sales_date,'MON-YYYY'),TO_CHAR(sales_date,'MM'),TO_CHAR(sales_date,'DD')
                         order by TO_CHAR(sales_date,'DD')";
            QueryNew += ConditionNew + @"
                         group by sl.branch_code,bs.branch_edesc,FN_CHARTAD_MONTH(SUBSTR(sales_date,4,3)),TO_CHAR(sales_date,'MON-YYYY'),TO_CHAR(sales_date,'MM'),TO_CHAR(sales_date,'DD')
                         order by TO_CHAR(sales_date,'DD')";
            QueryBS += Condition + @"
                         group by  sl.branch_code,bs.branch_edesc,FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SALES_DATE),6,2)),SUBSTR(BS_DATE(SALES_DATE),6,2),SUBSTR(BS_DATE(SALES_DATE),9,2),trim(substr((BS_DATE(SALES_DATE)),0,7))
                         ORDER BY SUBSTR(BS_DATE(SALES_DATE),9,2)";
            QueryNewBS += ConditionNew + @"
                         group by  sl.branch_code,bs.branch_edesc,FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SALES_DATE),6,2)),SUBSTR(BS_DATE(SALES_DATE),6,2),SUBSTR(BS_DATE(SALES_DATE),9,2),trim(substr((BS_DATE(SALES_DATE)),0,7))
                         ORDER BY SUBSTR(BS_DATE(SALES_DATE),9,2)";

            string QueryFinal = string.Empty;
            if (AmountType == "NetAmount" || AmountType == "TotalAmount")
            {
                QueryFinal = string.Equals(DateFormat, "BS") ? QueryNewBS : QueryNew;
            }
            else
            {
                QueryFinal = string.Equals(DateFormat, "BS") ? QueryBS : Query;
            }

            var salesReportList = _objectEntity.SqlQuery<BranchDaysSalesGraph>(QueryFinal).ToList();
            var sales = salesReportList.FirstOrDefault();
            if (sales != null)
            {
                if (string.Equals(DateFormat, "AD"))
                {
                    int year = DateTime.Today.Year;
                    int month = DateTime.Today.Month;
                    int.TryParse(monthName.Substring(0, 4), out year);
                    int.TryParse(monthName.Substring(4, 2), out month);
                    int daysInMonth = DateTime.DaysInMonth(year, month);
                    for (int i = 1; i <= daysInMonth; i++)
                    {
                        var checkday = i.ToString().PadLeft(2, '0');
                        if (salesReportList.Any(x => x.day == checkday))
                            continue;
                        var branchtest = new BranchDaysSalesGraph()
                        {
                            day = checkday,
                            Month = sales.Month,
                            MonthNo = sales.MonthNo,
                            Quantity = 0,
                            TotalAmount = 0,
                            GrossAmount = 0,
                            NetAmount = 0,
                            BranchName = sales.BranchName,
                        };
                        salesReportList.Add(branchtest);
                    }
                }
                else if (string.Equals(DateFormat, "BS"))
                {
                    int daysInMonth = 0;
                    var monthday = this._objectEntity.SqlQuery<int>("select Days_no from calendar_setup Where bs_month='" + sales.BsYear_Month + "'").FirstOrDefault();
                    int.TryParse(monthday.ToString(), out daysInMonth);
                    if (daysInMonth > 0)
                    {
                        for (int i = 1; i <= daysInMonth; i++)
                        {
                            var checkday = i.ToString().PadLeft(2, '0');
                            if (salesReportList.Any(x => x.day == checkday))
                                continue;
                            var branchtest = new BranchDaysSalesGraph()
                            {
                                day = checkday,
                                Month = sales.Month,
                                MonthNo = sales.MonthNo,
                                Quantity = 0,
                                TotalAmount = 0,
                                GrossAmount = 0,
                                NetAmount = 0,
                                BranchName = sales.BranchName,
                            };
                            salesReportList.Add(branchtest);
                        }
                    }
                }
            }
            return salesReportList;



            //string ad_bs_select = "TO_CHAR(sales_date, 'DD') day, ";
            //string ad_bs_select_new = "TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'dd') day,to_char(fiscal_year.start_date + ROWNUM - 1, 'fmMonth') as month, ";
            //string ad_bs_group = "TO_CHAR(sales_date, 'DD') , ";
            //string ad_bs_group_new = "TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'dd'),";
            //string ad_bs_month_filterCondition = " to_char(fiscal_year.start_date + ROWNUM - 1, 'fmMonth') ";
            //if (string.Equals(DateFormat, "BS"))
            //{
            //    ad_bs_select = "to_char(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),9,2)) day,";
            //    ad_bs_select_new = "to_char(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),9,2)) day,fn_bs_month(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2)) as month, ";
            //    ad_bs_group = "TO_CHAR(sales_date, 'DD') ,to_char(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),9,2)), ";
            //    ad_bs_group_new = "TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'dd'),to_char(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),9,2)),fn_bs_month(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2)),";
            //    ad_bs_month_filterCondition = " fn_bs_month(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2)) ";
            //}

            //string Query = @"SELECT 
            //        si.branch_code,
            //                BS.BRANCH_EDESC as BranchName,
            //                "+ 
            //                 ad_bs_select+
            //                 @"TO_CHAR(sales_date, 'MM') month ,
            //                 fn_bs_month(substr(bs_date(sales_date), 6, 2)) AS Nepalimonth,
            //                   TO_CHAR(sales_date, 'Month') Monthenglish,
            //                 SUM (nvl(si.calc_total_price,0))/{0} AS Amount,
            //                 SUM (nvl(si.calc_Quantity,0))/{0} AS Quantity
            //            FROM sa_sales_invoice si, fa_branch_setup bs,SA_CUSTOMER_SETUP CS,IP_ITEM_MASTER_SETUP IM               
            //    WHERE si.deleted_flag = 'N' AND si.company_code = '{1}'
            //           and SI.BRANCH_CODE = BS.BRANCH_CODE AND SI.CUSTOMER_CODE = CS.CUSTOMER_CODE AND  IM.ITEM_CODE = SI.ITEM_CODE";

            //string QueryNew = @"select * from ( 
            //    SELECT bs.BRANCH_CODE,
            //    bs.branch_edesc as BranchName, 
            //    " +
            //    ad_bs_select_new;
            ////string subQueryAmount = @"(select nvl(SUM (nvl(si.calc_total_price,0))/1,0)
            ////    FROM sa_sales_invoice si, fa_branch_setup bs,SA_CUSTOMER_SETUP CS,IP_ITEM_MASTER_SETUP IM               
            ////    WHERE si.deleted_flag = 'N' AND si.company_code = '01'
            ////    and TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'Month')=TO_CHAR(si.sales_date, 'Month')
            ////    and SI.BRANCH_CODE = BS.BRANCH_CODE AND SI.CUSTOMER_CODE = CS.CUSTOMER_CODE AND  IM.ITEM_CODE = SI.ITEM_CODE ";
            //string subQueryAmount = @"(select
            //     nvl(SUM (nvl(si.calc_total_price,0))/{0},0) as amount
            //    from sa_sales_invoice si,fa_branch_setup bi
            //    WHERE  si.company_code = '{01}' and si.branch_code=bi.branch_code  
            //    and si.branch_code=(select branch_code from fa_branch_setup where branch_edesc='" + branchName + @"'  and company_code='{1}') 
            //    and to_char(fiscal_year.start_date + ROWNUM - 1, 'fmMonth')=to_char(si.sales_date, 'fmMonth') and  
            //    to_char(fiscal_year.start_date + ROWNUM - 1, 'dd')= to_char(si.sales_date, 'dd') 
            //    ";
            ////string subQueryQuantity = @"(select nvl(SUM (nvl(si.calc_Quantity,0))/1,0)
            ////    FROM sa_sales_invoice si, fa_branch_setup bs,SA_CUSTOMER_SETUP CS,IP_ITEM_MASTER_SETUP IM               
            ////    WHERE si.deleted_flag = 'N' AND si.company_code = '01'
            ////    and TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'Month')=TO_CHAR(si.sales_date, 'Month')
            ////    and SI.BRANCH_CODE = BS.BRANCH_CODE AND SI.CUSTOMER_CODE = CS.CUSTOMER_CODE AND  IM.ITEM_CODE = SI.ITEM_CODE ";
            //string subQueryQuantity = @"(select
            //    nvl( SUM (nvl(si.calc_Quantity,0))/{0},0) as Quanity
            //    from sa_sales_invoice si,fa_branch_setup bs
            //    WHERE  si.company_code = '{01}' and si.branch_code=
            //    (select branch_code from fa_branch_setup where branch_edesc='" + branchName + @"'   and company_code='{1}')  
            //    and si.branch_code=bs.branch_code and to_char(fiscal_year.start_date + ROWNUM - 1, 'fmMonth')=
            //    to_char(si.sales_date, 'fmMonth') and  to_char(fiscal_year.start_date + ROWNUM - 1, 'dd')= 
            //    to_char(si.sales_date, 'dd') ";

            //string QueryNewEnd = string.Empty;
            //string condition = string.Empty;
            ////for customer Filter
            //if (!string.IsNullOrEmpty(customerCode))
            //{
            //    Query += " and (";
            //    condition += " and (";
            //    foreach (var item in customerCode.Split(','))
            //    {
            //        Query += "cs.master_customer_code like  (Select MASTER_CUSTOMER_CODE || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND COMPANY_CODE='" + userInfo.company_code + "' ) OR ";
            //        condition += "cs.master_customer_code like  (Select MASTER_CUSTOMER_CODE || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND COMPANY_CODE='" + userInfo.company_code + "' ) OR ";
            //    }
            //    Query = Query.Substring(0, Query.Length - 3) + ") ";
            //    condition = condition.Substring(0, condition.Length - 3) + ") ";
            //}

            ////for item Filter
            //if (!string.IsNullOrEmpty(itemCode))
            //{
            //    Query += " and (";
            //    condition += " and (";
            //    foreach (var item in itemCode.Split(','))
            //    {
            //        Query += "IM.MASTER_ITEM_CODE LIKE (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND COMPANY_CODE='" + userInfo.company_code + "') OR ";
            //        condition += "IM.MASTER_ITEM_CODE LIKE (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND COMPANY_CODE='" + userInfo.company_code + "') OR ";
            //    }
            //    Query = Query.Substring(0, Query.Length - 3) + ") ";
            //    condition = condition.Substring(0, condition.Length - 3) + ") ";
            //}


            ////for category Filter
            //if (!string.IsNullOrEmpty(categoryCode))
            //{
            //    Query += " and (";
            //    condition += " and (";
            //    foreach (var item in categoryCode.Split(','))
            //    {
            //        Query += "IM.CATEGORY_CODE = '" + item + "' OR ";
            //        condition += "IM.CATEGORY_CODE = '" + item + "' OR ";
            //    }
            //    Query = Query.Substring(0, Query.Length - 3) + ") ";
            //    condition = condition.Substring(0, condition.Length - 3) + ") ";
            //}

            ////FOR COMPANY FILTER
            //if (!string.IsNullOrEmpty(companyCode))
            //{
            //    Query += " AND SI.COMPANY_CODE IN (" + companyCode + ")";
            //    condition += " AND SI.COMPANY_CODE IN (" + companyCode + ")";
            //}


            ////FOR BRANCH FILTER
            //if (reportFilters.BranchFilter.Count() > 0)
            //{
            //    Query += " AND SI.BRANCH_CODE IN (" + string.Join(",", reportFilters.BranchFilter) + ")";
            //    condition += " AND SI.BRANCH_CODE IN (" + string.Join(",", reportFilters.BranchFilter) + ")";
            //    try { userInfo.company_code = reportFilters.BranchFilter[0].Split('.')[0]; } catch { }

            //}           


            ////FOR partyType FILTER
            //if (!string.IsNullOrEmpty(partyTypeCode))
            //{
            //    Query += " AND SI.PARTY_TYPE_CODE IN (" + partyTypeCode + ")";
            //    condition += " AND SI.PARTY_TYPE_CODE IN (" + partyTypeCode + ")";
            //}

            ////FOR FORMCODE FILER
            //if (!string.IsNullOrEmpty(formCode))
            //{
            //    Query += " AND SI.FORM_CODE IN (" + formCode + ")";
            //    condition += " AND SI.FORM_CODE IN (" + formCode + ")";
            //}

            //if (!string.IsNullOrEmpty(monthName))
            //{
            //    //condition += " and TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'fmMonth')=TO_CHAR('" + monthName + "') ";
            //    condition+=" and "+ad_bs_month_filterCondition +"= TO_CHAR('" + monthName + "') ";
            //}


            //Query += @"GROUP BY si.branch_code,BS.BRANCH_EDESC, 
            //        "+
            //        ad_bs_group+
            //        @"TO_CHAR(sales_date, 'MM') ,
            //        fn_bs_month(substr(bs_date(sales_date),6,2)),
            //        TO_CHAR(sales_date, 'Month')";

            //if (string.IsNullOrEmpty(branchName))
            //{
            //    QueryNewEnd = @"FROM all_objects, fa_branch_setup bs,
            //       (SELECT start_date, end_date
            //          FROM HR_FISCAL_YEAR_CODE
            //         WHERE sysdate BETWEEN start_date AND end_date) fiscal_year
            //    WHERE ROWNUM <= fiscal_year.end_date - fiscal_year.start_date + 1
            //    group by 
            //        bs.branch_code,
            //        BS.BRANCH_EDESC,
            //        "+
            //        ad_bs_group_new+
            //        @"TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MM'),
            //        fn_bs_month(substr(bs_date(fiscal_year.start_date + ROWNUM - 1), 6, 2)),
            //        TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'fmMonth')";
            //}
            //else
            //{
            //    QueryNewEnd = @"FROM all_objects, fa_branch_setup bs,
            //               (SELECT start_date, end_date
            //                  FROM HR_FISCAL_YEAR_CODE
            //                 WHERE sysdate BETWEEN start_date AND end_date) fiscal_year 
            //         WHERE ROWNUM <= fiscal_year.end_date - fiscal_year.start_date + 1
            //         and  bs.branch_code=(select branch_code from fa_branch_setup where branch_edesc='" + branchName + @"'  and company_code='{1}')
            //         and bs.company_code='{1}'
            //         group by  
            //            " +
            //            ad_bs_group_new+
            //            @"to_char(fiscal_year.start_date + ROWNUM - 1, 'fmMonth'),
            //            bs.BRANCH_CODE,bs.branch_edesc
            //            order by to_char(fiscal_year.start_date + ROWNUM - 1, 'fmMonth')) fa where fa.month='" + monthName + @"' 
            //         ";
            //}


            //Query = QueryNew +
            //    subQueryAmount + condition + ") as Amount," +
            //    subQueryQuantity + condition + ") as Quantity " +
            //    QueryNewEnd;
            //Query = string.Format(Query, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), userInfo.company_code, userInfo.branch_code);

            //var salesReportList = _objectEntity.SqlQuery<BranchDaysSalesGraph>(Query).ToList();
            //return salesReportList;
        }
        public List<ChartSalesModel> GetItemWiseBranchSalesSummary(ReportFiltersModel reportFilters, User userInfo, string monthCode, string branchCode, string companyCode)
        {
            //companyCode = string.Join(",", reportFilters.CompanyFilter);
            //companyCode = companyCode == "" ? userInfo.company_code : companyCode;
            //string saleReturnQuery = salesReturn ? "'SALES','SALES_RETURN'" : "'SALES'";
            //var companyCode = string.Join(",'", reportFilters.CompanyFilter);
            companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            var Query = string.Empty;
            Query = @"SELECT  IM.ITEM_CODE Code, IM.ITEM_EDESC DESCRIPTION,
         SUM(nvl(SI.CALC_TOTAL_PRICE,0))/{0} GrossAmount ,
         SUM(nvl(SI.CALC_QUANTITY,0))/1 QUANTITY
 FROM SA_SALES_INVOICE SI
 INNER JOIN  IP_ITEM_MASTER_SETUP IM
        ON SI.ITEM_CODE = IM.ITEM_CODE
 WHERE SI.DELETED_FLAG = 'N' AND IM.DELETED_FLAG= 'N'
            AND IM.ITEM_CODE = SI.ITEM_CODE  
            AND IM.COMPANY_CODE = SI.COMPANY_CODE
            AND SI.company_code IN({1})
            AND SI.BRANCH_CODE IN ({2})
            AND SUBSTR(BS_DATE(SI.SALES_DATE),6,2)='{3}'";
            Query = string.Format(Query, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode, branchCode, monthCode);

            //for customer Filter
            if (reportFilters.CustomerFilter.Count() > 0)
            {
                var customerFilter = string.Empty;
                customerFilter = @"SELECT  DISTINCT(customer_code) FROM sa_customer_setup where (";
                //IF CUSTOMER_SKU_FLAG = G
                foreach (var item in reportFilters.CustomerFilter)
                {
                    customerFilter += "master_customer_code LIKE  (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                customerFilter = customerFilter.Substring(0, customerFilter.Length - 3);
                //IF CUSTOMER_SKU_FLAG = I                
                customerFilter += " or (customer_code in (" + string.Join(",", reportFilters.CustomerFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";

                Query += " and SI.customer_code IN(" + customerFilter + ")";
            }
            //for Product Filter
            if (reportFilters.ProductFilter.Count() > 0)
            {
                var productFilter = string.Empty;
                productFilter = @"SELECT  DISTINCT item_code from IP_ITEM_MASTER_SETUP WHERE (";
                //IF PRODUCT_SKU_FLAG = G
                foreach (var item in reportFilters.ProductFilter)
                {
                    productFilter += "MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                productFilter = productFilter.Substring(0, productFilter.Length - 3);
                //IF PRODUCT_SKU_FLAG = I                
                productFilter += " OR (ITEM_CODE in (" + string.Join(",", reportFilters.ProductFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";

                Query += " AND SI.ITEM_CODE IN(" + productFilter + ")";
            }
            //FOR BRANCH FILTER
            if (reportFilters.BranchFilter.Count() > 0)
            {
                Query += " AND SI.BRANCH_CODE IN (" + string.Join(",", reportFilters.BranchFilter) + ")";
            }

            if (reportFilters.EmployeeFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SI.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", reportFilters.EmployeeFilter).ToString());
            }
            if (reportFilters.AgentFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SI.AGENT_CODE IN  ('{0}')", string.Join("','", reportFilters.AgentFilter).ToString());
            }
            string locationFilter = string.Empty;
            if (reportFilters.LocationFilter.Count > 0)
            {
                var locations = reportFilters.LocationFilter;
                for (int i = 0; i < locations.Count; i++)
                {
                    if (i == 0)
                        locationFilter += string.Format("SELECT LOCATION_CODE FROM IP_LOCATION_SETUP WHERE LOCATION_CODE LIKE '{0}%' ", locations[i]);
                    else
                    {
                        locationFilter += string.Format(" OR LOCATION_CODE like '{0}%' ", locations[i]);
                    }
                }
                locationFilter = string.Format(@" AND SI.FROM_LOCATION_CODE IN ({0} OR LOCATION_CODE IN ('{1}'))", locationFilter, string.Join("','", locations));
                //query = query.AppendFormat(@" AND A.FROM_LOCATION_CODE IN ('{0}')", string.Join("','", filters.LocationFilter).ToString());
                Query = Query + locationFilter;
            }

            Query += " GROUP BY IM.ITEM_CODE,IM.ITEM_EDESC";

            var productSales = _objectEntity.SqlQuery<ChartSalesModel>(Query);
            return productSales.ToList();
        }

        public List<ChartSalesModel> GetItemWiseBranchPurchaseGroupSummary(ReportFiltersModel reportFilters, User userInfo, string monthCode, string branchCode, string companyCode)
        {
            //companyCode = string.Join(",", reportFilters.CompanyFilter);
            //companyCode = companyCode == "" ? userInfo.company_code : companyCode;
            //string saleReturnQuery = salesReturn ? "'SALES','SALES_RETURN'" : "'SALES'";
            //var companyCode = string.Join(",'", reportFilters.CompanyFilter);
            companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            var Query = string.Empty;
            Query = @"SELECT  IM.ITEM_CODE Code, IM.ITEM_EDESC DESCRIPTION,
         SUM(nvl(SI.CALC_TOTAL_PRICE,0))/{0} GrossAmount ,
         SUM(nvl(SI.CALC_QUANTITY,0))/1 QUANTITY
 FROM IP_PURCHASE_INVOICE SI
 INNER JOIN  IP_ITEM_MASTER_SETUP IM
        ON SI.ITEM_CODE = IM.ITEM_CODE
    inner join BI_ITEM_GROUP_MAP IMP
        ON   SI.ITEM_CODE=IMP.ITEM_CODE
 WHERE SI.DELETED_FLAG = 'N' AND IM.DELETED_FLAG= 'N'
            AND IM.ITEM_CODE = SI.ITEM_CODE  
            AND IM.COMPANY_CODE = SI.COMPANY_CODE
            AND SI.company_code IN({1})
            AND  IMP.ITEM_GROUP_CODE IN ({2})
            AND SUBSTR(BS_DATE(SI.INVOICE_DATE),6,2)='{3}'";
            Query = string.Format(Query, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode, branchCode, monthCode);

            //for customer Filter
            if (reportFilters.CustomerFilter.Count() > 0)
            {
                var customerFilter = string.Empty;
                customerFilter = @"SELECT  DISTINCT(customer_code) FROM sa_customer_setup where (";
                //IF CUSTOMER_SKU_FLAG = G
                foreach (var item in reportFilters.CustomerFilter)
                {
                    customerFilter += "master_customer_code LIKE  (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                customerFilter = customerFilter.Substring(0, customerFilter.Length - 3);
                //IF CUSTOMER_SKU_FLAG = I                
                customerFilter += " or (customer_code in (" + string.Join(",", reportFilters.CustomerFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";

                Query += " and SI.customer_code IN(" + customerFilter + ")";
            }
            //for Product Filter
            if (reportFilters.ProductFilter.Count() > 0)
            {
                var productFilter = string.Empty;
                productFilter = @"SELECT  DISTINCT item_code from IP_ITEM_MASTER_SETUP WHERE (";
                //IF PRODUCT_SKU_FLAG = G
                foreach (var item in reportFilters.ProductFilter)
                {
                    productFilter += "MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                productFilter = productFilter.Substring(0, productFilter.Length - 3);
                //IF PRODUCT_SKU_FLAG = I                
                productFilter += " OR (ITEM_CODE in (" + string.Join(",", reportFilters.ProductFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";

                Query += " AND SI.ITEM_CODE IN(" + productFilter + ")";
            }
            //FOR BRANCH FILTER
            if (reportFilters.BranchFilter.Count() > 0)
            {
                Query += " AND SI.BRANCH_CODE IN (" + string.Join(",", reportFilters.BranchFilter) + ")";
            }

            if (reportFilters.EmployeeFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SI.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", reportFilters.EmployeeFilter).ToString());
            }
            if (reportFilters.AgentFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SI.AGENT_CODE IN  ('{0}')", string.Join("','", reportFilters.AgentFilter).ToString());
            }
            string locationFilter = string.Empty;
            if (reportFilters.LocationFilter.Count > 0)
            {
                var locations = reportFilters.LocationFilter;
                for (int i = 0; i < locations.Count; i++)
                {
                    if (i == 0)
                        locationFilter += string.Format("SELECT LOCATION_CODE FROM IP_LOCATION_SETUP WHERE LOCATION_CODE LIKE '{0}%' ", locations[i]);
                    else
                    {
                        locationFilter += string.Format(" OR LOCATION_CODE like '{0}%' ", locations[i]);
                    }
                }
                locationFilter = string.Format(@" AND SI.FROM_LOCATION_CODE IN ({0} OR LOCATION_CODE IN ('{1}'))", locationFilter, string.Join("','", locations));
                //query = query.AppendFormat(@" AND A.FROM_LOCATION_CODE IN ('{0}')", string.Join("','", filters.LocationFilter).ToString());
                Query = Query + locationFilter;
            }

            Query += " GROUP BY IM.ITEM_CODE,IM.ITEM_EDESC";

            var productSales = _objectEntity.SqlQuery<ChartSalesModel>(Query);
            return productSales.ToList();
        }

        public List<ChartSalesModel> GetDivisionItemWiseReport(ReportFiltersModel reportFilters, User userInfo, string monthCode, string divisionCode, string companyCode, string dateFormat)
        {
            //companyCode = string.Join(",", reportFilters.CompanyFilter);
            //companyCode = companyCode == "" ? userInfo.company_code : companyCode;
            //string saleReturnQuery = salesReturn ? "'SALES','SALES_RETURN'" : "'SALES'";
            //var companyCode = string.Join(",'", reportFilters.CompanyFilter);
            companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            var Query = string.Empty;
            if (dateFormat == "AD")
            {
                Query = @"SELECT  IM.ITEM_CODE Code, IM.ITEM_EDESC DESCRIPTION,
                         SUM(nvl(SI.CALC_TOTAL_PRICE,0))/{0} GrossAmount ,
                         SUM(nvl(SI.CALC_QUANTITY,0))/1 QUANTITY
                         FROM SA_SALES_INVOICE SI
                         INNER JOIN  IP_ITEM_MASTER_SETUP IM
                                ON SI.ITEM_CODE = IM.ITEM_CODE
                         WHERE SI.DELETED_FLAG = 'N' AND IM.DELETED_FLAG= 'N'
                            AND IM.ITEM_CODE = SI.ITEM_CODE  
                            AND IM.COMPANY_CODE = SI.COMPANY_CODE
                            AND SI.company_code IN({1})
                            AND SI.DIVISION_CODE IN ({2})
                            AND TO_CHAR (si.sales_date, 'MM') = '{3}'";
            }
            else
            {
                Query = @"SELECT  IM.ITEM_CODE Code, IM.ITEM_EDESC DESCRIPTION,
                         SUM(nvl(SI.CALC_TOTAL_PRICE,0))/{0} GrossAmount ,
                         SUM(nvl(SI.CALC_QUANTITY,0))/1 QUANTITY
                         FROM SA_SALES_INVOICE SI
                         INNER JOIN  IP_ITEM_MASTER_SETUP IM
                                ON SI.ITEM_CODE = IM.ITEM_CODE
                         WHERE SI.DELETED_FLAG = 'N' AND IM.DELETED_FLAG= 'N'
                            AND IM.ITEM_CODE = SI.ITEM_CODE  
                            AND IM.COMPANY_CODE = SI.COMPANY_CODE
                            AND SI.company_code IN({1})
                            AND SI.DIVISION_CODE IN ({2})
                            AND SUBSTR(BS_DATE(SI.SALES_DATE),6,2) = '{3}'";
            }
            Query = string.Format(Query, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode, divisionCode, monthCode);

            //for customer Filter
            if (reportFilters.CustomerFilter.Count() > 0)
            {
                var customerFilter = string.Empty;
                customerFilter = @"SELECT  DISTINCT(customer_code) FROM sa_customer_setup where (";
                //IF CUSTOMER_SKU_FLAG = G
                foreach (var item in reportFilters.CustomerFilter)
                {
                    customerFilter += "master_customer_code LIKE  (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                customerFilter = customerFilter.Substring(0, customerFilter.Length - 3);
                //IF CUSTOMER_SKU_FLAG = I                
                customerFilter += " or (customer_code in (" + string.Join(",", reportFilters.CustomerFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";

                Query += " and SI.customer_code IN(" + customerFilter + ")";
            }
            //for Product Filter
            if (reportFilters.ProductFilter.Count() > 0)
            {
                var productFilter = string.Empty;
                productFilter = @"SELECT  DISTINCT item_code from IP_ITEM_MASTER_SETUP WHERE (";
                //IF PRODUCT_SKU_FLAG = G
                foreach (var item in reportFilters.ProductFilter)
                {
                    productFilter += "MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                productFilter = productFilter.Substring(0, productFilter.Length - 3);
                //IF PRODUCT_SKU_FLAG = I                
                productFilter += " OR (ITEM_CODE in (" + string.Join(",", reportFilters.ProductFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";

                Query += " AND SI.ITEM_CODE IN(" + productFilter + ")";
            }
            //FOR BRANCH FILTER
            if (reportFilters.BranchFilter.Count() > 0)
            {
                Query += " AND SI.BRANCH_CODE IN (" + string.Join(",", reportFilters.BranchFilter) + ")";
            }

            if (reportFilters.EmployeeFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SI.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", reportFilters.EmployeeFilter).ToString());
            }
            if (reportFilters.AgentFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SI.AGENT_CODE IN  ('{0}')", string.Join("','", reportFilters.AgentFilter).ToString());
            }
            string locationFilter = string.Empty;
            if (reportFilters.LocationFilter.Count > 0)
            {
                var locations = reportFilters.LocationFilter;
                for (int i = 0; i < locations.Count; i++)
                {
                    if (i == 0)
                        locationFilter += string.Format("SELECT LOCATION_CODE FROM IP_LOCATION_SETUP WHERE LOCATION_CODE LIKE '{0}%' ", locations[i]);
                    else
                    {
                        locationFilter += string.Format(" OR LOCATION_CODE like '{0}%' ", locations[i]);
                    }
                }
                locationFilter = string.Format(@" AND SI.FROM_LOCATION_CODE IN ({0} OR LOCATION_CODE IN ('{1}'))", locationFilter, string.Join("','", locations));
                //query = query.AppendFormat(@" AND A.FROM_LOCATION_CODE IN ('{0}')", string.Join("','", filters.LocationFilter).ToString());
                Query = Query + locationFilter;
            }

            Query += " GROUP BY IM.ITEM_CODE,IM.ITEM_EDESC";

            var productSales = _objectEntity.SqlQuery<ChartSalesModel>(Query);
            return productSales.ToList();
        }

        public List<MonthlyBranchSalesGraph> GetMonthlyBranchSalesSummary(ReportFiltersModel reportFilters, User userInfo)
        {
            if (userInfo == null)
            {
                userInfo = new Core.Domain.User();
                userInfo.company_code = "01";
                userInfo.branch_code = "01.01";

            }
            else if (string.IsNullOrEmpty(userInfo.company_code) || string.IsNullOrEmpty(userInfo.branch_code))
            {
                userInfo.company_code = "01";
                userInfo.branch_code = "01.01";
            }
            //var Query = string.Format(@"SELECT si.branch_code,
            //                    BS.BRANCH_EDESC as BranchName,
            //                     TO_CHAR (si.sales_date, 'fmMonth') AS Month,
            //                     SUM (nvl(si.calc_total_price,0))/{0} AS Amount,
            //                 SUM (nvl(si.calc_Quantity,0))/{0} AS Quantity
            //                FROM sa_sales_invoice si, fa_branch_setup bs
            //               WHERE si.deleted_flag = 'N'  AND si.company_code = '{1}' and si.branch_code = '{2}'
            //               and SI.BRANCH_CODE = BS.BRANCH_CODE
            //            GROUP BY si.branch_code,BS.BRANCH_EDESC, TO_CHAR (si.sales_date, 'fmMonth')", ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), userInfo.company_code, userInfo.branch_code);

            var Query = string.Format(@"select
                    TO_CHAR (fiscal_year.start_date + ROWNUM - 1, 'fmMonth') AS Month,
                    bs.branch_code,
                    bs.BRANCH_EDESC as BranchName,
                    (SELECT nvl(SUM (nvl(si.calc_total_price,0))/{0},0) AS Amount
                    FROM sa_sales_invoice si
                        WHERE si.deleted_flag = 'N'  AND si.company_code = '{1}'
                        and SI.BRANCH_CODE = BS.BRANCH_CODE
                        and TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'fmMonth')=TO_CHAR(sales_date, 'fmMonth')) as Amount,
                    (select nvl(SUM (nvl(si.calc_Quantity,0))/{0},0) AS Quantity
                        FROM sa_sales_invoice si
                        WHERE si.deleted_flag = 'N'  AND si.company_code = '{1}'
                        and SI.BRANCH_CODE = BS.BRANCH_CODE
                        and TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'fmMonth')=TO_CHAR(sales_date, 'fmMonth')) as Quantity
                FROM all_objects, fa_branch_setup bs,
                   (SELECT start_date, end_date
                      FROM HR_FISCAL_YEAR_CODE
                     WHERE sysdate BETWEEN start_date AND end_date) fiscal_year
                WHERE ROWNUM <= fiscal_year.end_date - fiscal_year.start_date + 1
                group by 
                TO_CHAR (fiscal_year.start_date + ROWNUM - 1, 'fmMonth'),
                bs.branch_code,bs.BRANCH_EDESC",
            ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), userInfo.company_code);
            var salesReportList = _objectEntity.SqlQuery<MonthlyBranchSalesGraph>(Query).ToList();
            return salesReportList;
        }

        public List<SalesCollectionGraph> GetSalesCollectionMonthlyReport(ReportFiltersModel reportFilters, User userInfo)
        {
            if (userInfo == null)
            {
                userInfo = new Core.Domain.User();
                userInfo.company_code = "01";
                userInfo.branch_code = "01.01";

            }
            else if (string.IsNullOrEmpty(userInfo.company_code) || string.IsNullOrEmpty(userInfo.branch_code))
            {
                userInfo.company_code = "01";
                userInfo.branch_code = "01.01";
            }
            //string Query = string.Format(@"select
            //                    to_char(voucher_date,'fmMonth') as   Month , 
            //                     SUM (nvl(dr_amount,0))/{0} as Sales,
            //                     SUM (nvl(cr_amount,0))/{0} as Collection
            //                from v$virtual_sub_ledger
            //              WHERE  company_code = '{1}' and branch_code = '{2}'
            //                group by to_char(voucher_date, 'fmMonth')
            //                order by 1 ", ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), userInfo.company_code, userInfo.branch_code);

            string Query = string.Format(@"SELECT  to_char(fiscal_year.start_date + ROWNUM - 1, 'fmMonth') as month,
to_char(fiscal_year.start_date + ROWNUM - 1, 'MM'),
(select
                 SUM (nvl(dr_amount,0))/{0} as Sales
                from v$virtual_sub_ledger
                WHERE  company_code = '{1}' and to_char(fiscal_year.start_date + ROWNUM - 1, 'fmMonth')= to_char(voucher_date, 'fmMonth'))  as sales,(select
                 SUM (nvl(cr_amount,0))/{0} as Sales
                from v$virtual_sub_ledger
                WHERE  company_code = '{1}' and to_char(fiscal_year.start_date + ROWNUM - 1, 'fmMonth')= to_char(voucher_date, 'fmMonth'))  as collection
                          FROM all_objects,
                               (SELECT start_date, end_date
                                  FROM HR_FISCAL_YEAR_CODE
                                 WHERE sysdate BETWEEN start_date AND end_date) fiscal_year
                         WHERE ROWNUM <= fiscal_year.end_date - fiscal_year.start_date + 1
                         group by to_char(fiscal_year.start_date + ROWNUM - 1, 'fmMonth'),
to_char(fiscal_year.start_date + ROWNUM - 1, 'MM')
order by to_char(fiscal_year.start_date + ROWNUM - 1, 'MM')"
                , ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), userInfo.company_code);
            var salesReportList = _objectEntity.SqlQuery<SalesCollectionGraph>(Query).Where(a => a.Month != null).ToList();
            return salesReportList;
        }



        public List<SalesTargetGraphDayWise> GetSalesTargetDailyReport(ReportFiltersModel reportFilters, User userInfo, string DateFormat, string monthName, string monthInt)
        {
            var companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            //FOR BRANCH FILTER
            string branchCondition = string.Empty;
            if (reportFilters.BranchFilter.Count() > 0)
            {
                branchCondition = " AND BRANCH_CODE IN (" + string.Join(",", reportFilters.BranchFilter) + ")";
            }

            string Query = string.Empty;
            if (string.IsNullOrEmpty(DateFormat) || string.Equals(DateFormat, "AD"))
            {
                Query = string.Format(@"SELECT day,MonthNo,Month,Sales,Target from (SELECT 
                    to_char(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),9,2)) day,                  
                    to_char(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2)) MonthNo ,                    
                    fn_bs_month(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2)) AS Month,
                    (select
                     nvl(SUM(nvl(dr_amount,0))/{0},0)
                    from v$virtual_sub_ledger
                    WHERE  company_code IN ({1}) {2}  
                      and to_char(fiscal_year.start_date + ROWNUM - 1, 'YYY-MM-DD')= to_char(voucher_date, 'YYY-MM-DD')
                       AND trim(TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'Month')) = '{3}')  as sales,
                   (SELECT NVL (SUM (NVL (si.PER_DAY_AMOUNT, 0)) /{0}, 0)
                      FROM PL_SALES_PLAN_DTL si
                     WHERE     company_code IN ({1}) {2} 
                           AND TO_CHAR (fiscal_year.start_date + ROWNUM - 1,'YYY-MM-DD') =TO_CHAR (si.plan_date, 'YYY-MM-DD')
                          AND trim(TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'Month')) = '{3}')  as Target,
                FROM all_objects,
                   (SELECT start_date, end_date
                      FROM HR_FISCAL_YEAR_CODE
                     WHERE sysdate BETWEEN start_date AND end_date) fiscal_year
                WHERE ROWNUM <= fiscal_year.end_date - fiscal_year.start_date + 1
                group by
                TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MM'),
                TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'Month') ,
                bs_date(fiscal_year.start_date + ROWNUM - 1),
                TO_CHAR (fiscal_year.start_date + ROWNUM - 1,'YYY-MM-DD'),
                fn_bs_month(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2)),
                to_char(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),9,2)))
               where trim(TO_CHAR(Month)) = '{3}'                     
               order by day",
                  ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode, branchCondition, monthName);
                //Query = string.Format(@"select  day, MonthNo, Month,Sales,Target from (
                //            SELECT 
                //                TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'DD') day, 
                //                TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MM') MonthNo ,
                //                TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'Month') Month, 
                //                (select
                //                 nvl(SUM(nvl(dr_amount,0))/{0},0) as Sales
                //                from v$virtual_sub_ledger
                //                WHERE  company_code IN ({1}) {2} 
                //                and to_char(fiscal_year.start_date + ROWNUM - 1, 'YYY-MM-DD')= to_char(voucher_date, 'YYY-MM-DD')
                //                AND trim(TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'Month')) = '{3}')  as sales,
                //               (SELECT NVL (SUM (NVL (si.PER_DAY_AMOUNT, 0)) / {0}, 0)
                //                  FROM PL_SALES_PLAN_DTL si
                //                 WHERE company_code IN ({1}) {2} 
                //                       AND TO_CHAR (fiscal_year.start_date + ROWNUM - 1,'YYY-MM-DD') =TO_CHAR (si.plan_date, 'YYY-MM-DD')
                //                       AND trim(TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'Month')) = '{3}')  as Target
                //            FROM all_objects,
                //               (SELECT start_date, end_date
                //                  FROM HR_FISCAL_YEAR_CODE
                //                 WHERE sysdate BETWEEN start_date AND end_date) fiscal_year
                //            WHERE ROWNUM <= fiscal_year.end_date - fiscal_year.start_date + 1                         
                //            group by  to_char(fiscal_year.start_date + ROWNUM - 1, 'YYY-MM-DD'),
                //            TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MM'),
                //            TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'Month') ,
                //            TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'DD'))
                //            where trim(TO_CHAR(Month)) = '{3}'                     
                //            order by day",
                //    ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode, branchCondition, monthName);
            }
            else if (string.Equals(DateFormat, "BS"))
            {
                Query = string.Format(@"SELECT day,MonthNo,Month,Sales,Target from (SELECT 
                    to_char(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),9,2)) day,                  
                    to_char(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2)) MonthNo ,                    
                    fn_bs_month(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2)) AS Month,
                    (select
                     nvl(SUM(nvl(dr_amount,0))/{0},0)
                    from v$virtual_sub_ledger
                    WHERE  company_code IN ({1}) {2}  
                      and to_char(fiscal_year.start_date + ROWNUM - 1, 'YYY-MM-DD')= to_char(voucher_date, 'YYY-MM-DD')
                       AND trim(fn_bs_month(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2))) = '{3}')  as sales,
                   (SELECT NVL (SUM (NVL (si.PER_DAY_AMOUNT, 0)) /{0}, 0)
                      FROM PL_SALES_PLAN_DTL si
                     WHERE     company_code IN ({1}) {2} 
                           AND TO_CHAR (fiscal_year.start_date + ROWNUM - 1,'YYY-MM-DD') =TO_CHAR (si.plan_date, 'YYY-MM-DD')
                           AND trim(fn_bs_month(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2))) = '{3}')  as Target
                FROM all_objects,
                   (SELECT start_date, end_date
                      FROM HR_FISCAL_YEAR_CODE
                     WHERE sysdate BETWEEN start_date AND end_date) fiscal_year
                WHERE ROWNUM <= fiscal_year.end_date - fiscal_year.start_date + 1
                group by
                TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MM'),
                TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'Month') ,
                bs_date(fiscal_year.start_date + ROWNUM - 1),
                TO_CHAR (fiscal_year.start_date + ROWNUM - 1,'YYY-MM-DD'),
                fn_bs_month(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2)),
                to_char(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),9,2)))
               where trim(TO_CHAR(Month)) = '{3}'                     
               order by day",
                    ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode, branchCondition, monthName);
            }

            var salesReportList = _objectEntity.SqlQuery<SalesTargetGraphDayWise>(Query).Where(a => a.Month != null).ToList();
            return salesReportList;
        }

        public List<SalesTargetGraphDayWise> GetSalesTargetItemWiseReport(ReportFiltersModel reportFilters, User userInfo, string DateFormat, string monthName, string monthInt)
        {
            var companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            //FOR BRANCH FILTER
            string branchCondition = string.Empty;
            if (reportFilters.BranchFilter.Count() > 0)
            {
                branchCondition = " AND BRANCH_CODE IN (" + string.Join(",", reportFilters.BranchFilter) + ")";
            }

            string query = string.Empty;
            if (string.IsNullOrEmpty(DateFormat) || string.Equals(DateFormat, "AD"))
            {
                query = string.Format(@"SELECT IIMS.ITEM_CODE Code, IIMS.ITEM_EDESC DESCRIPTION,
                            SUM (nvl(SSI.CALC_TOTAL_PRICE,0))/{0} GrossAmount , 
                           SUM (nvl(SSI.CALC_QUANTITY,0))/{0} QUANTITY FROM
                           IP_ITEM_MASTER_SETUP IIMS 
                          INNER JOIN SA_SALES_INVOICE SSI on SSI.ITEM_CODE = IIMS.ITEM_CODE 
                          WHERE IIMS.DELETED_FLAG = 'N' AND TO_CHAR(SSI.sales_date, 'YYYYMM') = '{1}'
                          and ssi.company_code IN({2})
                          GROUP BY IIMS.ITEM_CODE, IIMS.ITEM_EDESC", ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), monthName, companyCode);
            }
            else
            {
                query = string.Format(@"SELECT IIMS.ITEM_CODE Code, IIMS.ITEM_EDESC Month,
                            SUM (nvl(SSI.CALC_TOTAL_PRICE,0))/{0} Sales , 
                           SUM (nvl(SSI.CALC_QUANTITY,0))/{0} QUANTITY,(SELECT NVL (SUM (NVL (si.PER_DAY_AMOUNT, 0)) /{0}, 0)
                      FROM PL_SALES_PLAN_DTL si, (SELECT start_date, end_date
                      FROM HR_FISCAL_YEAR_CODE
                        WHERE sysdate BETWEEN start_date AND end_date) fiscal_year
                        WHERE si.company_code IN ({2})  
                        AND ITEM_CODE = IIMS.ITEM_CODE
                        AND TO_CHAR (fiscal_year.start_date + ROWNUM - 1,'YYY-MM-DD') =TO_CHAR (si.plan_date, 'YYY-MM-DD')
                           AND trim(fn_bs_month(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2))) = ('{1}')
                           group by si.item_code) TARGET FROM
                           IP_ITEM_MASTER_SETUP IIMS 
                          INNER JOIN SA_SALES_INVOICE SSI on SSI.ITEM_CODE = IIMS.ITEM_CODE 
                          WHERE IIMS.DELETED_FLAG = 'N' AND SUBSTR(BS_DATE(SSI.sales_date),6,2) = ('{3}')
                          and ssi.company_code IN ({2})
                          GROUP BY IIMS.ITEM_CODE, IIMS.ITEM_EDESC", ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), monthName, companyCode, monthInt);
            }

            var salesReportList = _objectEntity.SqlQuery<SalesTargetGraphDayWise>(query).Where(a => a.Month != null).ToList();
            return salesReportList;
        }


        public List<TargetCollectionGraphDayWise> GetTargetCollectionDailyReport(ReportFiltersModel reportFilters, User userInfo, string DateFormat, string monthName)
        {
            var companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);

            //FOR BRANCH FILTER
            string branchCondition = string.Empty;
            if (reportFilters.BranchFilter.Count() > 0)
            {
                branchCondition = " AND BRANCH_CODE IN (" + string.Join(",", reportFilters.BranchFilter) + ")";
            }
            //string Query = string.Format(@"SELECT 
            //                    TO_CHAR(voucher_date, 'DD') day, 
            //                    TO_CHAR(voucher_date, 'MM') MonthNo ,
            //                    TO_CHAR(voucher_date, 'Month') MonthEnglish, 
            //                    fn_bs_month(substr(bs_date(voucher_date),6,2)) AS Nepalimonth,
            //                    SUM (nvl(dr_amount,0))/{0} as Sales,
            //                     SUM (nvl(cr_amount,0))/{0} as Collection
            //                FROM v$virtual_sub_ledger
            //                WHERE  company_code = '{1}' and branch_code = '{2}'
            //                GROUP by TO_CHAR(voucher_date, 'DD') 
            //                ,TO_CHAR(voucher_date, 'MM')
            //                ,TO_CHAR(voucher_date, 'Month') 
            //                ,fn_bs_month(substr(bs_date(voucher_date),6,2))
            //                order by MonthNo,day", ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), userInfo.company_code, userInfo.branch_code);
            string Query = string.Empty;
            if (string.IsNullOrEmpty(DateFormat) || string.Equals(DateFormat, "AD"))
            {
                Query = string.Format(@"Select day,MonthNo,Month,Target,Collection from (
                            SELECT 
                                TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'DD') day, 
                                TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MM') MonthNo ,
                                TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'Month') Month,                    
                                (SELECT NVL (SUM (NVL (per_day_amount, 0)) /{0}, 0)
                                      FROM pl_sales_plan_dtl
                                     WHERE company_code = '{1}' {2} 
                                           AND TO_CHAR (fiscal_year.start_date + ROWNUM - 1,'YYY-MM-DD') = TO_CHAR (plan_date, 'YYY-MM-DD')
                                           AND trim(TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'Month')) = '{3}') AS Target,
                                (select
                                 nvl(SUM(nvl(cr_amount,0))/{0},0)
                                from v$virtual_sub_ledger
                                WHERE  company_code = '{1}' {2} and to_char(fiscal_year.start_date + ROWNUM - 1, 'YYY-MM-DD')= to_char(voucher_date, 'YYY-MM-DD')
                                AND trim(TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'Month')) = '{3}')  as Collection
                            FROM all_objects,
                               (SELECT start_date, end_date
                                  FROM HR_FISCAL_YEAR_CODE
                                 WHERE sysdate BETWEEN start_date AND end_date) fiscal_year
                            WHERE ROWNUM <= fiscal_year.end_date - fiscal_year.start_date + 1
                            group by  to_char(fiscal_year.start_date + ROWNUM - 1, 'DD'),
                            TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MM'),
                            to_char(fiscal_year.start_date + ROWNUM - 1, 'YYY-MM-DD'),
                            TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'Month'))
                            where trim(TO_CHAR(Month)) = '{3}'                     
                            order by day", ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), userInfo.company_code, branchCondition, monthName);
            }
            else if (string.Equals(DateFormat, "BS"))
            {
                Query = string.Format(@"Select day,MonthNo,Month,Target,Collection from (
                        SELECT 
                              to_char(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),9,2)) day,                  
                              to_char(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2)) MonthNo ,                    
                               fn_bs_month(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2)) AS Month,
                              (SELECT NVL (SUM (NVL (per_day_amount, 0)) /{0}, 0)
                                                  FROM pl_sales_plan_dtl
                                                 WHERE company_code = '{1}' {2}  
                                                       AND TO_CHAR (fiscal_year.start_date + ROWNUM - 1,'YYY-MM-DD') = TO_CHAR (plan_date, 'YYY-MM-DD')
                                                       and trim(fn_bs_month(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2))) = '{3}') AS Target,
                                            (select
                                             nvl(SUM(nvl(cr_amount,0))/{0},0)
                                            from v$virtual_sub_ledger
                                            WHERE  company_code = '{1}' {2} 
                                               and to_char(fiscal_year.start_date + ROWNUM - 1, 'YYY-MM-DD')= to_char(voucher_date, 'YYY-MM-DD')
                                               and trim(fn_bs_month(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2))) = '{3}')  as Collection
                            FROM all_objects,
                               (SELECT start_date, end_date
                                  FROM HR_FISCAL_YEAR_CODE
                                 WHERE sysdate BETWEEN start_date AND end_date) fiscal_year
                            WHERE ROWNUM <= fiscal_year.end_date - fiscal_year.start_date + 1
                            group by    
                            TO_CHAR (fiscal_year.start_date + ROWNUM - 1,'YYY-MM-DD'),      
                              fn_bs_month(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2)),
                              to_char(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2)) ,
                              to_char(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),9,2)))
                              where trim(Month) = '{3}'                     
                              order by day",
                               ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), userInfo.company_code, branchCondition, monthName);
            }

            var salesReportList = _objectEntity.SqlQuery<TargetCollectionGraphDayWise>(Query).Where(a => a.Month != null).ToList();
            return salesReportList;
        }


        public List<SalesCollectionGraphDayWise> GetSalesCollectionDailyReport(ReportFiltersModel reportFilters, User userInfo, string month, string DateFormat)
        {


            //var companyCode = string.Join(",", reportFilters.CompanyFilter);
            //companyCode = companyCode == "" ? userInfo.company_code : companyCode;
            //var companyCode = string.Join(",'", reportFilters.CompanyFilter);
            var companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);

            //new query by bikalpa sir
            string Query = string.Empty;
            if (string.Equals(DateFormat, "AD"))
            {
                Query = string.Format(@"SELECT TO_CHAR (l.voucher_date, 'DD') AS day,
                                         TO_CHAR (l.voucher_date, 'MM') AS MonthNo,
                                         TO_CHAR (l.voucher_date, 'Month') AS MonthEnglish,
                                         SUM (NVL (l.dr_amount, 0)) /{0} AS Sales,
                                         SUM (NVL (l.cr_amount, 0)) /{0} AS Collection
                                       FROM v$virtual_sub_ledger l
                                       WHERE l.SUB_LEDGER_FLAG = 'C'
                                         AND l.DELETED_FLAG = 'N'
                                         AND l.FORM_CODE <> 0
                                         AND l.company_code IN ({1})
                                        AND TO_CHAR (l.voucher_date, 'YYYYMM') = '{2}' "
                                    , ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode, month);
            }
            else
            {
                Query = string.Format(@"SELECT substr(bs_date(l.voucher_date),9,2) AS day,
                                         substr(bs_date(l.voucher_date),6,2) AS MonthNo,
                                         trim(substr(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(l.voucher_date),6,2)),5,20)) AS Nepalimonth,
                                         SUM (NVL (l.dr_amount, 0)) /{0} AS Sales,
                                         SUM (NVL (l.cr_amount, 0)) /{0} AS Collection
                                       FROM v$virtual_sub_ledger l
                                       WHERE l.SUB_LEDGER_FLAG = 'C'
                                         AND l.DELETED_FLAG = 'N'
                                         AND l.FORM_CODE <> 0
                                         AND l.company_code IN ({1})
                                        AND substr(bs_date(l.voucher_date),6,2) = '{2}' "
                     , ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode, month);
            }

            //for branch filter
            if (reportFilters.BranchFilter.Count > 0)
            {
                Query += string.Format(@" AND l.BRANCH_CODE IN ('{0}')", string.Join("','", reportFilters.BranchFilter));
            }
            //for division filter
            if (reportFilters.DivisionFilter.Count > 0)
            {
                Query += string.Format(@" AND l.DIVISION_CODE IN  ('{0}')", string.Join("','", reportFilters.DivisionFilter).ToString());
            }

            if (string.Equals(DateFormat, "AD"))
            {
                Query += @"
                           GROUP BY TO_CHAR (l.voucher_date, 'Month'),        
                                         TO_CHAR (l.voucher_date, 'DD'),
                                         TO_CHAR (l.voucher_date, 'MM')
                                ORDER BY TO_CHAR (l.voucher_date, 'DD')";
            }
            else
            {
                Query += @"
                         GROUP BY substr(bs_date(l.voucher_date),9,2) ,          
                                         substr(bs_date(l.voucher_date),6,2),
                                         substr(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(l.voucher_date),6,2)),5,20),
                                         substr(bs_date(l.voucher_date),6,2)
                                ORDER BY substr(bs_date(l.voucher_date),9,2)";
            }

            var salesReportList = _objectEntity.SqlQuery<SalesCollectionGraphDayWise>(Query).ToList();
            return salesReportList;







            //query by pramod sir
            //if (userInfo == null)
            //{
            //    userInfo = new Core.Domain.User();
            //    userInfo.company_code = "01";
            //    userInfo.branch_code = "01.01";

            //}
            //else if (string.IsNullOrEmpty(userInfo.company_code) || string.IsNullOrEmpty(userInfo.branch_code))
            //{
            //    userInfo.company_code = "01";
            //    userInfo.branch_code = "01.01";
            //}

            ////FOR BRANCH FILTER
            //string branchCondition = string.Empty;
            //if (reportFilters.BranchFilter.Count() > 0)
            //{
            //    branchCondition = " AND BRANCH_CODE IN (" + string.Join(",", reportFilters.BranchFilter) + ")";
            //    try { userInfo.company_code = reportFilters.BranchFilter[0].Split('.')[0]; } catch { }
            //}
            ////string Query = string.Format(@"SELECT 
            ////                    TO_CHAR(voucher_date, 'DD') day, 
            ////                    TO_CHAR(voucher_date, 'MM') MonthNo ,
            ////                    TO_CHAR(voucher_date, 'Month') MonthEnglish, 
            ////                    fn_bs_month(substr(bs_date(voucher_date),6,2)) AS Nepalimonth,
            ////                    SUM (nvl(dr_amount,0))/{0} as Sales,
            ////                     SUM (nvl(cr_amount,0))/{0} as Collection
            ////                FROM v$virtual_sub_ledger
            ////                WHERE  company_code = '{1}' and branch_code = '{2}'
            ////                GROUP by TO_CHAR(voucher_date, 'DD') 
            ////                ,TO_CHAR(voucher_date, 'MM')
            ////                ,TO_CHAR(voucher_date, 'Month') 
            ////                ,fn_bs_month(substr(bs_date(voucher_date),6,2))
            ////                order by MonthNo,day", ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), userInfo.company_code, userInfo.branch_code);
            //string Query = string.Empty;
            //if (string.IsNullOrEmpty(DateFormat) || string.Equals(DateFormat, "AD"))
            //{
            //    Query = string.Format(@"SELECT 
            //        TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'DD') day, 
            //        TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MM') MonthNo ,
            //        TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'Month') MonthEnglish, 
            //        fn_bs_month(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2)) AS Nepalimonth,
            //        (select
            //         nvl(SUM(nvl(dr_amount,0))/{0},0) as Sales
            //        from v$virtual_sub_ledger
            //        WHERE  company_code = '{1}' {2} and to_char(fiscal_year.start_date + ROWNUM - 1, 'DD')= to_char(voucher_date, 'DD'))  as sales,
            //        (select
            //         nvl(SUM(nvl(cr_amount,0))/{0},0) as Sales
            //        from v$virtual_sub_ledger
            //        WHERE  company_code = '{1}' {2} and to_char(fiscal_year.start_date + ROWNUM - 1, 'DD')= to_char(voucher_date, 'DD'))  as collection
            //    FROM all_objects,
            //       (SELECT start_date, end_date
            //          FROM HR_FISCAL_YEAR_CODE
            //         WHERE sysdate BETWEEN start_date AND end_date) fiscal_year
            //    WHERE ROWNUM <= fiscal_year.end_date - fiscal_year.start_date + 1
            //    group by  to_char(fiscal_year.start_date + ROWNUM - 1, 'DD'),
            //    TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MM'),
            //    TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'Month') ,
            //    fn_bs_month(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2))",
            //        ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), userInfo.company_code, branchCondition);
            //}
            //else if(string.Equals(DateFormat,"BS"))
            //{
            //    Query = string.Format(@"SELECT 
            //        to_char(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),9,2)) day,
            //        --TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'DD') day, 
            //        TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MM') MonthNo ,
            //        TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'Month') MonthEnglish, 
            //        fn_bs_month(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2)) AS Nepalimonth,
            //        (select
            //         nvl(SUM(nvl(dr_amount,0))/{0},0) as Sales
            //        from v$virtual_sub_ledger
            //        WHERE  company_code = '{1}' {2} and to_char(fiscal_year.start_date + ROWNUM - 1, 'DD')= to_char(voucher_date, 'DD'))  as sales,
            //        (select
            //         nvl(SUM(nvl(cr_amount,0))/{0},0) as Sales
            //        from v$virtual_sub_ledger
            //        WHERE  company_code = '{1}' {2} and to_char(fiscal_year.start_date + ROWNUM - 1, 'DD')= to_char(voucher_date, 'DD'))  as collection
            //    FROM all_objects,
            //       (SELECT start_date, end_date
            //          FROM HR_FISCAL_YEAR_CODE
            //         WHERE sysdate BETWEEN start_date AND end_date) fiscal_year
            //    WHERE ROWNUM <= fiscal_year.end_date - fiscal_year.start_date + 1
            //    group by  to_char(fiscal_year.start_date + ROWNUM - 1, 'DD'),
            //    TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MM'),
            //    TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'Month') ,
            //    fn_bs_month(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2)),
            //    to_char(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),9,2))",
            //        ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), userInfo.company_code, branchCondition);
            //}

            //var salesReportList = _objectEntity.SqlQuery<SalesCollectionGraphDayWise>(Query).Where(a => a.MonthEnglish != null || a.Nepalimonth != null).ToList();
            //return salesReportList;
        }

        public List<BranchWiseSalesCollection> GetSalesCollectionBranchWiseReport(ReportFiltersModel reportFilters, User userInfo)
        {
            if (userInfo == null)
            {
                userInfo = new Core.Domain.User();
                userInfo.company_code = "01";
                userInfo.branch_code = "01.01";

            }
            else if (string.IsNullOrEmpty(userInfo.company_code) || string.IsNullOrEmpty(userInfo.branch_code))
            {
                userInfo.company_code = "01";
                userInfo.branch_code = "01.01";
            }
            string Query = string.Format(@"select 
                                SUM (nvl(dr_amount,0))/{0} as Sales,
                                 SUM (nvl(cr_amount,0))/{0} as Collection,
                                Branch_Code as Branch,
                                (select Branch_EDESC from fa_branch_setup Where Branch_code in l.Branch_Code) as BranchName 
                            from v$virtual_sub_ledger l
                            WHERE  company_code = '{1}'
                            group by Branch_Code", ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), userInfo.company_code, userInfo.branch_code);
            var salesReportList = _objectEntity.SqlQuery<BranchWiseSalesCollection>(Query).ToList();
            return salesReportList;
        }

        public List<BranchWiseSalesCollection> GetSalesCollectionDivisionWiseReport(ReportFiltersModel reportFilters, User userInfo)
        {
            //var companyCode = string.Join(",", reportFilters.CompanyFilter);
            //companyCode = companyCode == "" ? userInfo.company_code : companyCode;
            //var companyCode = string.Join(",'", reportFilters.CompanyFilter);
            var companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }
            var fiscalYearFilter = string.Empty;
            if (reportFilters.FiscalYearFilter.Count > 0)
            {
                fiscalYearFilter = $"{reportFilters.FiscalYearFilter.FirstOrDefault().DBName}.";
            }
            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            var Query = $@"select c.Collection,nvl((select sum(total_sales)  from {fiscalYearFilter}V$SALES_INVOICE_REPORT3 where division_code=c.branch and deleted_flag='N' and company_code=c.company_code),0) Sales,c.Branch,c.BranchName from (select 
                               SUM (nvl(cr_amount,0)) as Collection,
                               l.division_code as Branch,
                               BS.division_edesc as BranchName,
                               l.company_code
                               from {fiscalYearFilter}v$virtual_sub_ledger l, {fiscalYearFilter}FA_DIVISION_SETUP BS
                               WHERE l.division_code = BS.division_code 
                                 and l.SUB_LEDGER_FLAG='C'
                                 and l.deleted_flag = 'N'
                                 and l.form_code<>'0'
                                 and l.division_code is not null
                                 AND l.company_code IN({companyCode})
                                 and bs.company_code=l.company_code    
                             group by l.division_code,BS.division_edesc,l.company_code) C";

            var Query1 = $@"select division_code as Branch,division_edesc as BranchName ,(SELECT NVL(SUM(NVL(CR_AMOUNT,0)),0) COLLECTION FROM {fiscalYearFilter}V$VIRTUAL_SUB_LEDGER
WHERE COMPANY_CODE IN({companyCode})  and   sub_ledger_flag='C' AND FORM_CODE IN (SELECT FORM_CODE FROM {fiscalYearFilter}FORM_SETUP WHERE FORM_TYPE IN('BK','CH')) and deleted_flag='N' and division_code=b.division_code and division_code is not null ) Collection,
nvl((select sum(NET_SALES)  from {fiscalYearFilter}V$SALES_INVOICE_REPORT3 where division_code=b.division_code and deleted_flag='N' and COMPANY_CODE IN({companyCode})  and company_code=b.company_code  ),0) Sales  from {fiscalYearFilter}FA_DIVISION_SETUP  b where deleted_flag='N' and company_code IN ({companyCode}) and group_SKU_flag='I'";

            //string Query = string.Format(@"select 
            //                                    SUM (nvl(l.dr_amount,0))/{0} as Sales,
            //                                     SUM (nvl(l.cr_amount,0))/{0} as Collection,
            //                                     l.Division_code as Branch,
            //                                     (select Division_edesc from FA_DIVISION_SETUP Where division_code in l.division_code) as BranchName                                        
            //                                from v$virtual_sub_ledger l
            //                                WHERE  l.company_code = '{1}'
            //                                AND DELETED_FLAG = 'N' AND l.DIVISION_CODE IS NOT NULL
            //                                group by l.Division_code", ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), userInfo.company_code, userInfo.branch_code);
            var salesReportList = _objectEntity.SqlQuery<BranchWiseSalesCollection>(Query1).ToList();
            return salesReportList;
        }

        public List<BranchWiseSalesTarget> GetSalesTargetSegmentReport(ReportFiltersModel reportFilters, User userInfo)
        {

            var companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            string Query = $@"select substr(A.PRE_ITEM_CODE,1,5) , B.ITEM_EDESC as BranchName,B.ITEM_CODE, sum(A.quantity)  Sales,
nvl((select round(SUM(PER_DAY_QUANTITY),0) from PL_SALES_PLAN_dtl where ITEM_CODE =B.ITEM_CODE and company_code = b.company_code ),0) Target from V_NET_SALES A, IP_ITEM_MASTER_SETUP B
where A.PRE_ITEM_CODE like '01.%'
AND substr(A.PRE_ITEM_CODE,1,5)  = B.MASTER_ITEM_CODE
AND A.COMPANY_CODE  = B.COMPANY_CODE
AND A.COMPANY_CODE in ({companyCode})
group by substr(A.PRE_ITEM_CODE,1,5),B.ITEM_EDESC,B.ITEM_CODE, B.COMPANY_CODE";


            //string Query = $@"select (SELECT NVL (SUM (NVL (calc_total_price, 0)) /{ ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter)}, 0)
            //                                  FROM sa_sales_invoice
            //                                 WHERE company_code IN({companyCode}) and branch_code in ts.branch_code) AS sales,
            //                      (SELECT NVL (SUM (NVL (per_day_quantity, 0)) /{ ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter)}, 0)
            //                                  FROM pl_sales_plan_dtl
            //                                 WHERE company_code IN({companyCode}) and branch_code in ts.branch_code) AS Target,
            //                    Branch_Code as Branch,
            //                    (select Branch_EDESC from fa_branch_setup Where company_code IN('01') and branch_code in ts.branch_code) as BranchName 
            //                from pl_sales_plan_dtl ts
            //                WHERE  ts.company_code IN({companyCode})                             
            //                group by Branch_Code";
            //FOR BRANCH FILTER
            var condition = string.Empty;
            //if (reportFilters.BranchFilter.Count() > 0)
            //{
            //    Query += " AND BRANCH_CODE IN (" + string.Join(",", reportFilters.BranchFilter.Where(x => x != null)) + ")";
            //}
            var salesReportList = _objectEntity.SqlQuery<BranchWiseSalesTarget>(Query).ToList();
            return salesReportList;
        }

        public List<BranchWiseSalesTarget> AgingFinishedGood(ReportFiltersModel reportFilters, User userInfo)
        {

            var companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            string Query = $@"select substr(A.PRE_ITEM_CODE,1,5) , B.ITEM_EDESC as BranchName,B.ITEM_CODE, sum(A.quantity)  Sales,
nvl((select round(SUM(PER_DAY_QUANTITY),0) from PL_SALES_PLAN_dtl where ITEM_CODE =B.ITEM_CODE and company_code = b.company_code ),0) Target from V_NET_SALES A, IP_ITEM_MASTER_SETUP B
where A.PRE_ITEM_CODE like '01.%'
AND substr(A.PRE_ITEM_CODE,1,5)  = B.MASTER_ITEM_CODE
AND A.COMPANY_CODE  = B.COMPANY_CODE
AND A.COMPANY_CODE in ({companyCode})
group by substr(A.PRE_ITEM_CODE,1,5),B.ITEM_EDESC,B.ITEM_CODE, B.COMPANY_CODE";


            //string Query = $@"select (SELECT NVL (SUM (NVL (calc_total_price, 0)) /{ ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter)}, 0)
            //                                  FROM sa_sales_invoice
            //                                 WHERE company_code IN({companyCode}) and branch_code in ts.branch_code) AS sales,
            //                      (SELECT NVL (SUM (NVL (per_day_quantity, 0)) /{ ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter)}, 0)
            //                                  FROM pl_sales_plan_dtl
            //                                 WHERE company_code IN({companyCode}) and branch_code in ts.branch_code) AS Target,
            //                    Branch_Code as Branch,
            //                    (select Branch_EDESC from fa_branch_setup Where company_code IN('01') and branch_code in ts.branch_code) as BranchName 
            //                from pl_sales_plan_dtl ts
            //                WHERE  ts.company_code IN({companyCode})                             
            //                group by Branch_Code";
            //FOR BRANCH FILTER
            var condition = string.Empty;
            //if (reportFilters.BranchFilter.Count() > 0)
            //{
            //    Query += " AND BRANCH_CODE IN (" + string.Join(",", reportFilters.BranchFilter.Where(x => x != null)) + ")";
            //}
            var salesReportList = _objectEntity.SqlQuery<BranchWiseSalesTarget>(Query).ToList();
            return salesReportList;
        }


        public List<BranchWiseSalesTarget> GetSalesTargetBranchWiseReport(ReportFiltersModel reportFilters, User userInfo)
        {

            var companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            string Query = $@"select (SELECT  TRUNC(NVL (SUM (NVL (NET_SALES_RATE*CALC_QUANTITY, 0)) /{ ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter)}, 0),0)
                                              FROM sa_sales_invoice sa,IP_ITEM_MASTER_SETUP IP
                                             WHERE  ip.company_code=sa.Company_code and sa.Item_Code=ip.Item_code and IP.DELETED_FLAG='N' and IP.GROUP_SKU_FLAG='I' and sa.company_code IN({companyCode})  AND sa.DELETED_FLAG='N'  and sa.branch_code in ts.branch_code) AS sales,
                                  (SELECT TRUNC(NVL (SUM (NVL (per_day_quantity, 0)) /{ ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter)}, 0),0)
                                              FROM pl_sales_plan_dtl DT,IP_ITEM_MASTER_SETUP IP 
                                             WHERE IP.COMPANY_CODE=DT.COMPANY_CODE AND DT.ITEM_CODE=IP.ITEM_CODE AND IP.GROUP_SKU_FLAG='I' AND IP.DELETED_FLAG='N' AND DT.company_code IN({companyCode}) and DT.branch_code in ts.branch_code) AS Target,
                                Branch_Code as Branch,
                                (select Branch_EDESC from fa_branch_setup Where company_code IN({companyCode}) and branch_code in ts.branch_code) as BranchName 
                            from fa_branch_setup ts
                            WHERE  ts.company_code IN({companyCode}) AND ts.DELETED_FLAG = 'N' AND TS.GROUP_SKU_FLAG = 'I'      
                            group by Branch_Code";


            //string Query = $@"select (SELECT NVL (SUM (NVL (calc_total_price, 0)) /{ ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter)}, 0)
            //                                  FROM sa_sales_invoice
            //                                 WHERE company_code IN({companyCode}) and branch_code in ts.branch_code) AS sales,
            //                      (SELECT NVL (SUM (NVL (per_day_quantity, 0)) /{ ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter)}, 0)
            //                                  FROM pl_sales_plan_dtl
            //                                 WHERE company_code IN({companyCode}) and branch_code in ts.branch_code) AS Target,
            //                    Branch_Code as Branch,
            //                    (select Branch_EDESC from fa_branch_setup Where company_code IN('01') and branch_code in ts.branch_code) as BranchName 
            //                from pl_sales_plan_dtl ts
            //                WHERE  ts.company_code IN({companyCode})                             
            //                group by Branch_Code";
            //FOR BRANCH FILTER
            var condition = string.Empty;
            if (reportFilters.BranchFilter.Count() > 0)
            {
                Query += " AND BRANCH_CODE IN (" + string.Join(",", reportFilters.BranchFilter.Where(x => x != null)) + ")";
            }
            var salesReportList = _objectEntity.SqlQuery<BranchWiseSalesTarget>(Query).ToList();
            return salesReportList;
        }

        public List<BranchWiseSalesTarget> GetSalesTargetBranchWiseMonthlyReport(ReportFiltersModel reportFilters, User userInfo, string branchcode)
        {

            var companyCode = string.Empty;
            var Query = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);

            Query = $@"SELECT SUBSTR (bs_date (SI.sales_date), 1, 4) as Year, DS.BRANCH_EDESC as branch_name, DS.BRANCH_CODE as branch_code,
                            fn_bs_month(substr(bs_date(si.sales_date),6,2)) AS Month,
                            SUBSTR(BS_DATE(SI.SALES_DATE),6,2) as MonthInt,
                           0 TargetQty,
                             0 TargetAmount,
                            SUM (nvl(si.calc_Quantity,0))/{ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter)} AS Quantity,
                            SUM (nvl(si.calc_total_price,0))/{ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter)} AS GrossAmount,
                            'SALES' DATATYPE
                    FROM sa_sales_invoice si, FA_BRANCH_SETUP DS
                    WHERE  1=1
                    AND SI.BRANCH_CODE = '{branchcode}'
                    AND si.deleted_flag = 'N' AND si.company_code in ({companyCode})
                    and SI.BRANCH_CODE = DS.BRANCH_CODE
                   AND SI.COMPANY_CODE IN ({companyCode})" +
                $@"GROUP BY DS.BRANCH_CODE, DS.BRANCH_EDESC, SUBSTR(BS_DATE(SI.SALES_DATE),6,2)
                    ,fn_bs_month(substr(bs_date(si.sales_date),6,2))
                    ,to_number(substr(bs_date(sales_date),6,2)),SUBSTR (bs_date (SI.sales_date), 1, 4)
                    UNION  ALL
                    SELECT SUBSTR (bs_date (SI.PLAN_DATE), 1, 4) as Year,DS.BRANCH_EDESC as branch_name, DS.BRANCH_CODE as branch_code,
                            fn_bs_month(substr(bs_date(si.PLAN_DATE),6,2)) AS Month,
                            SUBSTR(BS_DATE(SI.PLAN_DATE),6,2) as MonthInt,
                            SUM (nvl(si.PER_DAY_QUANTITY,0))/{ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter)} AS TargetQty,
                            SUM (nvl(si.PER_DAY_AMOUNT,0))/{ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter)} AS TargetAmount,
                            0 Quantity,
                            0 GrossAmount,
                               'TARGET' DATATYPE
                    FROM PL_SALES_PLAN_DTL si, FA_BRANCH_SETUP DS,IP_ITEM_MASTER_SETUP I
                    WHERE 1=1
                    AND SI.BRANCH_CODE = '{branchcode}'
                    AND si.deleted_flag = 'N' AND si.company_code in ({companyCode})
                    AND I.ITEM_CODE=SI.ITEM_CODE
                    AND I.GROUP_SKU_FLAG='I'
                    and SI.BRANCH_CODE = DS.BRANCH_CODE
                    AND SI.COMPANY_CODE IN ({companyCode}) ";

            //FOR BRANCH FILTER
            var condition = string.Empty;
            if (reportFilters.BranchFilter.Count() > 0)
            {
                Query += " AND BRANCH_CODE IN (" + string.Join(",", reportFilters.BranchFilter.Where(x => x != null)) + ")";

            }
            Query += @" 
                       GROUP BY DS.BRANCH_CODE, DS.BRANCH_EDESC, SUBSTR(BS_DATE(SI.PLAN_DATE),6,2)
                    ,fn_bs_month(substr(bs_date(si.PLAN_DATE),6,2))
                    ,to_number(substr(bs_date(PLAN_DATE),6,2)),SUBSTR (bs_date (SI.PLAN_DATE), 1, 4) ";
            var salesReportList = _objectEntity.SqlQuery<SalesTargetViewModel>(Query).ToList();

            var branchwisesales = new List<BranchWiseSalesTarget>();
            foreach (var data in salesReportList.Where(x => x.DataType == "SALES" || x.DataType == "TARGET").OrderBy(x => x.MonthInt).GroupBy(x => x.MonthInt).Select(x => x.FirstOrDefault()).ToList())
            {
                var branchWise = new BranchWiseSalesTarget();
                branchWise.BranchName = data.Month;
                //branchWise.MonthInt = data.MonthInt;
                //branchWise.MonthYear = data.Month;
                branchWise.Sales = Convert.ToDecimal(salesReportList.Where(x => x.DataType == "SALES" && x.MonthInt == data.MonthInt).Sum(X => X.GrossAmount));
                branchWise.Target = Convert.ToDecimal(salesReportList.Where(x => x.DataType != "SALES" && x.MonthInt == data.MonthInt).Sum(X => X.TargetAmount));
                branchwisesales.Add(branchWise);
            }
            return branchwisesales;
        }

        public List<BranchWiseSalesTarget> GetSalesTargetBranchWiseItemsReport(ReportFiltersModel reportFilters, User userInfo, string branchcode)
        {

            var companyCode = string.Empty;
            var Query = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);

            Query = $@"SELECT IIMS.ITEM_CODE Code, IIMS.ITEM_EDESC BranchName,
                            SUM (nvl(SSI.CALC_TOTAL_PRICE,0))/{ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter)} Sales , 
                           SUM (nvl(SSI.CALC_QUANTITY,0))/1 QUANTITY,(SELECT NVL (SUM (NVL (si.PER_DAY_AMOUNT, 0)) /{ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter)}, 0)
                      FROM PL_SALES_PLAN_DTL si, (SELECT start_date, end_date
                      FROM HR_FISCAL_YEAR_CODE
                        WHERE sysdate BETWEEN start_date AND end_date) fiscal_year
                        WHERE si.company_code IN ({companyCode})  
                        AND ITEM_CODE = IIMS.ITEM_CODE
                        AND TO_CHAR (fiscal_year.start_date + ROWNUM - 1,'YYY-MM-DD') =TO_CHAR (si.plan_date, 'YYY-MM-DD')
                            AND SI.BRANCH_CODE = '{branchcode}'
                           group by si.item_code) TARGET FROM
                           IP_ITEM_MASTER_SETUP IIMS 
                          INNER JOIN SA_SALES_INVOICE SSI on SSI.ITEM_CODE = IIMS.ITEM_CODE 
                          WHERE IIMS.DELETED_FLAG = 'N' 
                          and ssi.company_code IN ({companyCode}) AND IIMS.BRANCH_CODE = '{branchcode}'";

            //FOR BRANCH FILTER

            Query += @" GROUP BY IIMS.ITEM_CODE, IIMS.ITEM_EDESC";
            var salesReportList = _objectEntity.SqlQuery<BranchWiseSalesTarget>(Query).ToList();
            return salesReportList;
        }

        public List<BranchWiseTargetCollection> GetTargetCollectionBranchWiseReport(ReportFiltersModel reportFilters, User userInfo)
        {
            if (userInfo == null)
            {
                userInfo = new Core.Domain.User();
                //userInfo.company_code = "01";
                userInfo.branch_code = "01.01";

            }
            else if (string.IsNullOrEmpty(userInfo.company_code) || string.IsNullOrEmpty(userInfo.branch_code))
            {
                //userInfo.company_code = "01";
                userInfo.branch_code = "01.01";
            }
            var companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            string Query = @"select 
                              (SELECT NVL (SUM (NVL (collection_target, 0)) /{0}, 0)
                                          FROM fa_target_setup
                                         WHERE company_code IN('{1}') and branch_code in ts.branch_code) AS Target,
                                         (SELECT NVL (SUM (NVL (cr_amount, 0)) /{0}, 1)
                                          FROM v$virtual_sub_ledger
                                         WHERE company_code IN('{1}') and branch_code in ts.branch_code) AS Collection,
                            Branch_Code as Branch,
                            (select Branch_EDESC from fa_branch_setup Where company_code ='01' and branch_code in ts.branch_code) as BranchName 
                        from fa_target_setup ts
                        WHERE  ts.company_code IN('{1}') {2}                           
                        group by Branch_Code";



            //FOR BRANCH FILTER
            var condition = string.Empty;
            if (reportFilters.BranchFilter.Count() > 0)
            {
                condition = " AND BRANCH_CODE IN (" + string.Join(",", reportFilters.BranchFilter.Where(x => x != null)) + ")";
            }

            Query = string.Format(Query, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), userInfo.company_code, condition);
            var salesReportList = _objectEntity.SqlQuery<BranchWiseTargetCollection>(Query).ToList();
            return salesReportList;
        }
        public List<BranchWiseSalesCollection> GetSalesCollectionBranchWiseReport(ReportFiltersModel reportFilters, User userInfo, string branchCode)
        {
            //COMPANY FILTER
            //var companyCode = string.Join(",", reportFilters.CompanyFilter);
            //companyCode = companyCode == "" ? userInfo.company_code : companyCode;
            //var companyCode = string.Join(",'", reportFilters.CompanyFilter);
            var companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            string customerFilter = string.Empty;
            var CustomerCodeFilter = string.Empty;
            if (reportFilters.CustomerFilter.Count > 0)
            {
                customerFilter += @"AND SUB_CODE IN (SELECT DISTINCT TRIM(LINK_SUB_CODE) FROM SA_CUSTOMER_SETUP WHERE 
                                    CUSTOMER_CODE IN (SELECT CUSTOMER_CODE FROM SA_CUSTOMER_SETUP WHERE";
                CustomerCodeFilter += @"AND CUSTOMER_CODE IN (SELECT DISTINCT TRIM(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE 
                                    CUSTOMER_CODE IN (SELECT CUSTOMER_CODE FROM SA_CUSTOMER_SETUP WHERE";
                foreach (var item in reportFilters.CustomerFilter)
                {
                    customerFilter += " MASTER_CUSTOMER_CODE LIKE (SELECT DISTINCT(MASTER_CUSTOMER_CODE) || '%'  FROM SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                    CustomerCodeFilter += " MASTER_CUSTOMER_CODE LIKE (SELECT DISTINCT(MASTER_CUSTOMER_CODE) || '%'  FROM SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND COMPANY_CODE IN(" + companyCode + ")) OR "; //OR
                }
                customerFilter = customerFilter.Substring(0, customerFilter.Length - 3);
                CustomerCodeFilter = CustomerCodeFilter.Substring(0, CustomerCodeFilter.Length - 3);
                customerFilter += " OR CUSTOMER_CODE IN (" + string.Join(",", reportFilters.CustomerFilter) + ") AND GROUP_SKU_FLAG = 'I' AND COMPANY_CODE IN(" + companyCode + ")))";
                CustomerCodeFilter += " OR CUSTOMER_CODE IN (" + string.Join(",", reportFilters.CustomerFilter) + ") AND GROUP_SKU_FLAG = 'I' AND COMPANY_CODE IN(" + companyCode + ")))";
            }

            var innerBranchQuery = string.Empty;
            var substringQuery = string.Empty;
            var branchFilter = string.Empty;
            //FOR BRANCH FILTER
            if (reportFilters.BranchFilter.Count() > 0)
            {
                var branch_codeString = string.Empty;
                foreach (var branch in reportFilters.BranchFilter)
                {
                    branch_codeString += $@"'{branch}',";
                }
                innerBranchQuery += " AND l.Branch_Code IN (" + string.Join(",", reportFilters.BranchFilter) + ")";
                substringQuery += $" AND Branch_Code IN ({branch_codeString})";
                branchFilter = $" AND Branch_Code IN ({branch_codeString})"; 
            }
            if (reportFilters.DivisionFilter.Count() > 0)
            {
                var DivisionCodeString = string.Empty;
                foreach (var division in reportFilters.DivisionFilter)
                {
                    DivisionCodeString += $@"'{division}',";
                }
                DivisionCodeString = DivisionCodeString.TrimEnd(',');
                innerBranchQuery += " AND l.DIVISION_CODE IN (" + string.Join(",", reportFilters.DivisionFilter) + ")";
                substringQuery += $" AND DIVISION_CODE IN ({DivisionCodeString})";
            }
            var fiscalYearFilter = string.Empty;
            if(reportFilters.FiscalYearFilter.Count>0)
            {
                fiscalYearFilter = $"{reportFilters.FiscalYearFilter.FirstOrDefault().DBName}.";
            }
            

            var Query1 = $@"select branch_code as Branch,branch_edesc as BranchName ,(SELECT NVL(SUM(NVL(CR_AMOUNT,0)),0) COLLECTION FROM {fiscalYearFilter}V$VIRTUAL_SUB_LEDGER
WHERE COMPANY_CODE IN({companyCode}) {customerFilter} and   sub_ledger_flag='C' 
--AND FORM_CODE IN (SELECT FORM_CODE FROM FORM_SETUP WHERE FORM_TYPE IN('BK','CH')) 
and  form_code<>'0' and deleted_flag='N' and branch_code=b.branch_code {substringQuery}) Collection,
nvl((select sum(TOTAL_SALES)  from {fiscalYearFilter}V$SALES_INVOICE_REPORT3 where branch_code=b.branch_code and deleted_flag='N' and COMPANY_CODE IN({companyCode}) {CustomerCodeFilter} {substringQuery} and company_code=b.company_code  ),0) Sales  from {fiscalYearFilter}fa_branch_setup  b where deleted_flag='N' and company_code IN ({companyCode}) {branchFilter} and group_SKU_flag='I'";
            // old Query 
            //string Query = @"select 
            //                   SUM (nvl(dr_amount,0))/{0} as Sales,
            //                   SUM (nvl(cr_amount,0))/{0} as Collection,
            //                   l.Branch_Code as Branch,
            //                   BS.BRANCH_EDESC as BranchName
            //                   from v$virtual_sub_ledger l, fa_branch_setup BS
            //                   WHERE l.branch_code = BS.BRANCH_CODE 
            //                     and l.SUB_LEDGER_FLAG='C'
            //                     and l.deleted_flag = 'N'
            //                     and l.form_code<>'0'
            //                     AND l.company_code IN({1})
            //                     {2}";



            //FOR BRANCH FILTER
            //if (reportFilters.BranchFilter.Count() > 0)
            //{
            //    Query += " AND l.Branch_Code IN (" + string.Join(",", reportFilters.BranchFilter) + ")";
            //}
            //if (reportFilters.DivisionFilter.Count() > 0)
            //{
            //    Query += " AND l.DIVISION_CODE IN (" + string.Join(",", reportFilters.DivisionFilter) + ")";
            //}


            //Query += @"
            //           group by l.Branch_Code,BS.branch_edesc";
            // Query = string.Format(Query, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode, customerFilter);

            var salesReportList = _objectEntity.SqlQuery<BranchWiseSalesCollection>(Query1).ToList();
            return salesReportList;
        }


        public List<BranchWiseSalesCollection> GetSalesCollectionAreaWiseReport(ReportFiltersModel reportFilters, User userInfo, string branchCode)
        {
            //COMPANY FILTER
            var companyCode = string.Join(",", reportFilters.CompanyFilter);
            companyCode = companyCode == "" ? userInfo.company_code : companyCode;
            string customerFilter = string.Empty;
            var CustomerCodeFilter = string.Empty;
            if (reportFilters.CustomerFilter.Count > 0)
            {
                customerFilter += @"AND l.SUB_CODE IN (SELECT DISTINCT TRIM(LINK_SUB_CODE) FROM SA_CUSTOMER_SETUP WHERE 
                                    CUSTOMER_CODE IN (SELECT CUSTOMER_CODE FROM SA_CUSTOMER_SETUP WHERE";
                CustomerCodeFilter += @"AND CUSTOMER_CODE IN (SELECT DISTINCT TRIM(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE 
                                    CUSTOMER_CODE IN (SELECT CUSTOMER_CODE FROM SA_CUSTOMER_SETUP WHERE";
                foreach (var item in reportFilters.CustomerFilter)
                {
                    customerFilter += " MASTER_CUSTOMER_CODE LIKE (SELECT DISTINCT(MASTER_CUSTOMER_CODE) || '%'  FROM SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                    CustomerCodeFilter += " MASTER_CUSTOMER_CODE LIKE (SELECT DISTINCT(MASTER_CUSTOMER_CODE) || '%'  FROM SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND COMPANY_CODE IN(" + companyCode + ")) OR "; //OR
                }
                customerFilter = customerFilter.Substring(0, customerFilter.Length - 3);
                CustomerCodeFilter = CustomerCodeFilter.Substring(0, CustomerCodeFilter.Length - 3);
                customerFilter += " OR CUSTOMER_CODE IN (" + string.Join(",", reportFilters.CustomerFilter) + ") AND GROUP_SKU_FLAG = 'I' AND COMPANY_CODE IN(" + companyCode + ")))";
                CustomerCodeFilter += " OR CUSTOMER_CODE IN (" + string.Join(",", reportFilters.CustomerFilter) + ") AND GROUP_SKU_FLAG = 'I' AND COMPANY_CODE IN(" + companyCode + ")))";
            }

            var innerBranchQuery = string.Empty;
            var substringQuery = string.Empty;
            //FOR BRANCH FILTER
            if (reportFilters.BranchFilter.Count() > 0)
            {
                innerBranchQuery += " AND l.Branch_Code IN (" + string.Join(",", reportFilters.BranchFilter) + ")";
                substringQuery += " AND Branch_Code IN (" + string.Join(",", reportFilters.BranchFilter) + ")";
            }

            if (reportFilters.DivisionFilter.Count() > 0)
            {
                innerBranchQuery += " AND l.DIVISION_CODE IN (" + string.Join(",", reportFilters.DivisionFilter) + ")";
                substringQuery += " AND DIVISION_CODE IN (" + string.Join(",", reportFilters.DivisionFilter) + ")";
            }
            var Query = $@"SELECT C.COLLECTION Collection , to_char(C.AREA_CODE) Branch,C.AREA_EDESC BranchName,(SELECT ROUND(SUM(NET_SALES_RATE*nvl(CALC_QUANTITY,0)),2) FROM SA_SALES_INVOICE WHERE DELETED_FLAG='N' AND AREA_CODE=C.AREA_CODE  AND COMPANY_CODE IN ('01')) Sales
 FROM (SELECT SUM(L.CR_AMOUNT) COLLECTION,  B.AREA_CODE,AR.AREA_EDESC FROM FA_SUB_LEDGER_DEALER_MAP A, IP_PARTY_TYPE_CODE B, V$VIRTUAL_SUB_DEALER_LEDGER  L  ,AREA_SETUP AR
WHERE A.PARTY_TYPE_CODE =  B.PARTY_TYPE_CODE
AND A.COMPANY_CODE =  B.COMPANY_CODE
AND A.COMPANY_CODE =  L.COMPANY_CODE
AND A.SUB_CODE =  L.SUB_CODE
and b.party_type_code=l.party_type_code
AND L.SUB_LEDGER_FLAG='C'
AND L.DELETED_FLAG = 'N'
AND L.FORM_CODE<>'0'
AND L.COMPANY_CODE IN('01')
AND AR.AREA_CODE=B.AREA_CODE
AND A.COMPANY_CODE=AR.COMPANY_CODE
GROUP BY   B.AREA_CODE ,AR.AREA_EDESC
ORDER BY 1)  C  ";
            // old Query 
            //string Query = @"select 
            //                   SUM (nvl(dr_amount,0))/{0} as Sales,
            //                   SUM (nvl(cr_amount,0))/{0} as Collection,
            //                   l.Branch_Code as Branch,
            //                   BS.BRANCH_EDESC as BranchName
            //                   from v$virtual_sub_ledger l, fa_branch_setup BS
            //                   WHERE l.branch_code = BS.BRANCH_CODE 
            //                     and l.SUB_LEDGER_FLAG='C'
            //                     and l.deleted_flag = 'N'
            //                     and l.form_code<>'0'
            //                     AND l.company_code IN({1})
            //                     {2}";



            //FOR BRANCH FILTER
            //if (reportFilters.BranchFilter.Count() > 0)
            //{
            //    Query += " AND l.Branch_Code IN (" + string.Join(",", reportFilters.BranchFilter) + ")";
            //}
            //if (reportFilters.DivisionFilter.Count() > 0)
            //{
            //    Query += " AND l.DIVISION_CODE IN (" + string.Join(",", reportFilters.DivisionFilter) + ")";
            //}


            //Query += @"
            //           group by l.Branch_Code,BS.branch_edesc";
            // Query = string.Format(Query, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode, customerFilter);

            var salesReportList = _objectEntity.SqlQuery<BranchWiseSalesCollection>(Query).ToList();
            return salesReportList;
        }


        public bool SaveDashboardWidgets(string userId, string order, string type)
        {
            try
            {


                //First check exist or not
                userId = userId.ToUpper();
                var test = _objectEntity.SqlQuery<string>("SELECT user_id from dashboard_widgets where user_id='" + userId + "' and module_name='" + type + "'");
                if (test.Count() > 0)
                {
                    //update
                    _objectEntity.ExecuteSqlCommand("update dashboard_widgets set order_no = '" + order + "' where user_id= '" + userId + "' and module_name='" + type + "'");
                }
                else
                {
                    //insert
                    _objectEntity.ExecuteSqlCommand("insert into dashboard_widgets(id,module_name,user_id,order_no) values(1,'" + type + "','" + userId + "','" + order + "')");
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public string GetDashboardWidgets(string name, string type)
        {
            try
            {
                name = name.ToUpper();
                string Query = @"SELECT order_no from dashboard_widgets where user_id='" + name + "' and module_name='" + type + "'";
                var widgets = _objectEntity.SqlQuery<string>(Query).FirstOrDefault();
                return widgets;
            }
            catch
            {
                return "";
            }
        }
        public List<string> GetDashboardWidgetsForPersonalDashboard(string name)
        {
            try
            {
                //test 1
                //string Query = $@"SELECT DISTINCT TRIM(REGEXP_SUBSTR(ORDER_NO, '[^,]+', 1, LEVEL)) ORDER_NO
                //                      FROM DASHBOARD_WIDGETS  WHERE USER_ID=UPPER('{name}')
                //                    CONNECT BY LEVEL <=
                //                      LENGTH(ORDER_NO)
                //                        - LENGTH(REPLACE(ORDER_NO, ',', ''))
                //                        + 1";
                //var widgets = _objectEntity.SqlQuery<string>(Query).ToList();
                //return widgets;

                //test 2


                string Query = $@"SELECT ORDER_NO FROM DASHBOARD_WIDGETS  WHERE USER_ID=UPPER('{name}')";
                var widgets = _objectEntity.SqlQuery<string>(Query).ToList();
                List<string> list = new List<string>();
                foreach (var widget in widgets)
                {
                    list = list.Concat(widget.Split(',')).Distinct().ToList();
                }
                return list;



            }
            catch
            {
                return new List<string>();
            }
        }

        public bool ResetDashboardWidgets(string name, string type)
        {
            try
            {
                name = name.ToUpper();
                string Query = @"DELETE dashboard_widgets where user_id='" + name + "' and module_name in (" + type + ")";
                _objectEntity.ExecuteSqlCommand(Query);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IList<UserwiseChartList> GetUserwiseChartList(string Module_Code)
        {
            //string Query = $@"SELECT MM.FULL_PATH, MM.MODULE_CODE, MM.MENU_NO,MM.MENU_EDESC,MM.MENU_OBJECT_NAME,MM.ORDERby FROM WEB_MENU_MANAGEMENT MM
            //                   WHERE MODULE_CODE = '01'";

            //string Query = $@"SELECT MM.MODULE_ABBR,MM.COLOR,MM.DESCRIPTION, MM.ICON_PATH,MM.ORDERBY,MM.FULL_PATH,MM.VIRTUAL_PATH, MM.MODULE_CODE,MM.MENU_NO,MM.MENU_EDESC,MM.MENU_OBJECT_NAME,DW.USER_ID, ORDER_NO FROM WEB_MENU_MANAGEMENT MM,DASHBOARD_WIDGETS DW
            //                   WHERE MM.MENU_OBJECT_NAME = DW.MODULE_NAME AND MODULE_CODE = '{Module_Code}' AND MODULE_ABBR = 'DB'";

            string Query = $@"SELECT MM.MODULE_ABBR,MM.COLOR,MM.DESCRIPTION, MM.ICON_PATH,MM.ORDERBY,MM.FULL_PATH,MM.VIRTUAL_PATH, MM.MODULE_CODE,MM.MENU_NO,MM.MENU_EDESC,MM.MENU_OBJECT_NAME,DW.USER_ID, ORDER_NO,DW.QuickCap FROM WEB_MENU_MANAGEMENT MM,DASHBOARD_WIDGETS DW
                               WHERE MM.MENU_OBJECT_NAME = DW.MODULE_NAME  AND MODULE_ABBR = 'DB'
UNION
SELECT MM.MODULE_ABBR,MM.COLOR,MM.DESCRIPTION, MM.ICON_PATH,MM.ORDERBY,MM.FULL_PATH,MM.VIRTUAL_PATH, MM.MODULE_CODE,MM.MENU_NO,MM.MENU_EDESC,MM.MENU_OBJECT_NAME,'  ' USER_ID, ' ' ORDER_NO,'' QuickCap FROM WEB_MENU_MANAGEMENT MM
                               WHERE  MODULE_ABBR = 'SM' ORDER BY ORDERBY ASC";
            //AND MODULE_CODE = '{Module_Code}' MODULE_CODE = '{Module_Code}' AND 
            var list = _objectEntity.SqlQuery<UserwiseChartList>(Query).ToList();
            var installPlugins = _pluginFinder.GetPluginDescriptors(LoadPluginsMode.InstalledOnly, 0).ToList();

            foreach (var item in list)
            {
                foreach (var installedPluginsItem in installPlugins)
                {
                    if (item.MODULE_CODE == installedPluginsItem.ModuleCode)
                    {
                        item.MODULE_EDESC = installedPluginsItem.FriendlyName;
                    }
                }
            }
            return list;
        }

        public IList<UserwiseChartList> GetUserWiseMenuPermission(string Module_Code, User userInfo)
        {
            string query = $@"SELECT MM.MODULE_ABBR,MM.COLOR,MM.DESCRIPTION, MM.ICON_PATH,MM.ORDERBY,MM.FULL_PATH,MM.VIRTUAL_PATH, MM.MODULE_CODE,MM.MENU_NO,MM.MENU_EDESC,MM.MENU_OBJECT_NAME,DW.USER_ID, ORDER_NO FROM WEB_MENU_MANAGEMENT MM,DASHBOARD_WIDGETS DW, WEB_MENU_CONTROL WC
                               WHERE WC.MENU_NO=MM.MENU_NO AND  MM.MENU_OBJECT_NAME = DW.MODULE_NAME AND DW.USER_ID =UPPER('{userInfo.login_code}') AND MODULE_CODE = '{Module_Code}' AND MODULE_ABBR = 'DB'
UNION
SELECT MM.MODULE_ABBR,MM.COLOR,MM.DESCRIPTION, MM.ICON_PATH,MM.ORDERBY,MM.FULL_PATH,MM.VIRTUAL_PATH, MM.MODULE_CODE,MM.MENU_NO,MM.MENU_EDESC,MM.MENU_OBJECT_NAME,'  ' USER_ID, ' ' ORDER_NO FROM WEB_MENU_MANAGEMENT MM, WEB_MENU_CONTROL WC
                               WHERE WC.MENU_NO=MM.MENU_NO AND  MODULE_CODE = '{Module_Code}' AND MODULE_ABBR = 'SM' ORDER BY ORDERBY ASC";
            return _objectEntity.SqlQuery<UserwiseChartList>(query).ToList();
        }



        public bool DeleteUserwiseChartList(UserwiseChartList item)
        {
            //delete from dashboard_widgets
            ResetDashboardWidgets(item.USER_ID, "'" + item.MENU_OBJECT_NAME + "'");
            //check if other user has assing to that menu
            int userCount = _objectEntity.SqlQuery<int>("SELECT count(*) from dashboard_widgets where module_name = '" + item.MENU_OBJECT_NAME + "'").FirstOrDefault();
            if (userCount <= 0)
            {
                string Query = $@"DELETE WEB_MENU_MANAGEMENT where menu_no = {item.MENU_NO}";
                _objectEntity.ExecuteSqlCommand(Query);
            }
            return true;
        }




        public List<MonthlyDivisionSalesGraph> GetMonthlyDivisionSalesSummary(ReportFiltersModel reportFilters, User userInfo)
        {
            var query = string.Format(@"select pa.DIVISION_EDESC,pa.MONTH_NAME, nvl(SUM (nvl(si.calc_Quantity,0))/{0},0) AS QUANTITY, nvl(SUM (nvl(si.calc_total_price,0))/{0} ,0) AS AMOUNT from (SELECT 
    DS.DIVISION_EDESC , DS.DIVISION_CODE,
    TO_CHAR (fiscal_year.start_date + ROWNUM - 1, 'fmMonth') AS MONTH_NAME
FROM all_objects, fa_division_setup DS,
               (SELECT start_date, end_date
                  FROM HR_FISCAL_YEAR_CODE
                 WHERE sysdate BETWEEN start_date AND end_date) fiscal_year 
         WHERE ROWNUM <= fiscal_year.end_date - fiscal_year.start_date + 1 GROUP BY DS.DIVISION_EDESC , DS.DIVISION_CODE, TO_CHAR (fiscal_year.start_date + ROWNUM - 1, 'fmMonth'))  
         pa LEFT JOIN sa_sales_invoice si on pa.DIVISION_CODE = si.DIVISION_CODE AND pa.MONTH_NAME = TO_CHAR(si.sales_date,'fmMonth')
         GROUP BY DIVISION_EDESC, MONTH_NAME",
                ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), userInfo.company_code);
            var salesReportList = _objectEntity.SqlQuery<MonthlyDivisionSalesGraph>(query).ToList();
            return salesReportList;
        }

        public List<DivisionDaysSalesGraph> GetDailyDivisionSalesSummary(ReportFiltersModel reportFilters, User userInfo)
        {
            //string Query = string.Format(@"SELECT --si.branch_code,
            //                        BS.DIVISION_EDESC as DivisionName,
            //                         TO_CHAR(sales_date, 'DD') day, TO_CHAR(sales_date, 'MM') month ,TO_CHAR(sales_date, 'Month') Monthenglish,
            //                         SUM (nvl(si.calc_total_price,0))/{0} AS Amount,
            //                 SUM (nvl(si.calc_Quantity,0))/{0} AS Quantity
            //                    FROM sa_sales_invoice si, fa_division_setup bs
            //                   WHERE si.deleted_flag = 'N'
            //                    and si.company_code = '{1}'
            //                   and SI.DIVISION_CODE = BS.DIVISION_CODE
            //                GROUP BY --si.branch_code, 
            //                BS.DIVISION_EDESC, TO_CHAR(sales_date, 'DD') , TO_CHAR(sales_date, 'MM') 
            //                ,TO_CHAR(sales_date, 'Month')", 
            //                ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), userInfo.company_code, userInfo.branch_code);
            string Query = string.Format(@"select DivisionName,day,month,Monthenglish,Amount,Quantity  from (
                                SELECT 
                                    BS.DIVISION_EDESC as DivisionName,
                                    TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'DD') day, 
                                    TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MM') month ,
                                    TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'Month') Monthenglish,
                                    (select nvl(SUM (nvl(si.calc_total_price,0))/1,0) Amount
                                        FROM sa_sales_invoice si, fa_division_setup bs
                                        WHERE si.deleted_flag = 'N'
                                        and si.company_code = '01'
                                        and SI.DIVISION_CODE = BS.DIVISION_CODE
                                        and TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'DD') =TO_CHAR(sales_date, 'DD') 
                                    ) as Amount,
                                    (select nvl(SUM (nvl(si.calc_Quantity,0))/1,0) Quantity
                                        FROM sa_sales_invoice si, fa_division_setup bs
                                        WHERE si.deleted_flag = 'N'
                                        and si.company_code = '01'
                                        and SI.DIVISION_CODE = BS.DIVISION_CODE
                                        and TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'DD') =TO_CHAR(sales_date, 'DD') 
                                    ) as Quantity
                                FROM all_objects, fa_division_setup BS,
                                   (SELECT start_date, end_date
                                      FROM HR_FISCAL_YEAR_CODE
                                     WHERE sysdate BETWEEN start_date AND end_date) fiscal_year
                                WHERE ROWNUM <= fiscal_year.end_date - fiscal_year.start_date + 1
                                GROUP BY  
                                BS.DIVISION_EDESC
                                ,TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'DD') 
                                ,TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MM') 
                                ,TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'Month')
                                ) ",
                                ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), userInfo.company_code, userInfo.branch_code);
            var salesReportList = _objectEntity.SqlQuery<DivisionDaysSalesGraph>(Query).ToList();
            return salesReportList;
        }


        public List<DivisionDaysSalesGraph> GetDailyDivisionSalesSummary(ReportFiltersModel reportFilters, User userInfo,
            string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string divisionName,
            string partyTypeCode, string formCode, string DateFormat, string AmountType)
        {
            string Query = @"SELECT BS.DIVISION_EDESC as DivisionName,
                                     TO_CHAR(sales_date, 'DD') day, TO_CHAR(sales_date, 'MM') month ,TO_CHAR(sales_date, 'Month') Monthenglish,
                                    SUM (nvl(si.calc_total_price,0))/{0} AS GrossAmount,
                             SUM (nvl(si.calc_Quantity,0))/{0} AS Quantity
                                FROM sa_sales_invoice si, fa_division_setup bs,SA_CUSTOMER_SETUP CS, IP_ITEM_MASTER_SETUP IM
                               WHERE si.deleted_flag = 'N' AND si.company_code = '" + userInfo.company_code + @"' AND SI.CUSTOMER_CODE = CS.CUSTOMER_CODE AND  IM.ITEM_CODE = SI.ITEM_CODE 
                               and SI.DIVISION_CODE = BS.DIVISION_CODE
                                and BS.DIVISION_EDESC='{2}'";
            string QueryBS = @"SELECT BS.DIVISION_EDESC as DivisionName,
                             to_char(substr(bs_date(si.sales_date),9,2)) day,
                             to_char(substr(bs_date(si.sales_date),6,2)) month,
                             fn_bs_month(substr(bs_date(si.sales_date),6,2)) Monthenglish,     
                            SUM (nvl(si.calc_total_price,0))/{0} AS GrossAmount,
                        SUM (nvl(si.calc_Quantity,0))/1 AS Quantity
                        FROM sa_sales_invoice si, fa_division_setup bs,SA_CUSTOMER_SETUP CS, IP_ITEM_MASTER_SETUP IM
                        WHERE si.deleted_flag = 'N' AND si.company_code = '{1}' AND SI.CUSTOMER_CODE = CS.CUSTOMER_CODE AND  IM.ITEM_CODE = SI.ITEM_CODE 
                        and SI.DIVISION_CODE = BS.DIVISION_CODE 
                        and BS.DIVISION_EDESC='{2}'";
            string QueryNew = @"
                select DivisionName,day,month,Monthenglish,GrossAmount,Quantity  from (
                SELECT 
                    BS.DIVISION_EDESC as DivisionName,
                    TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'DD') day, 
                    TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MM') month ,
                    TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'Month') Monthenglish,";
            string QueryAmount = @"(select nvl(SUM (nvl(si.calc_total_price,0))/{0},0) GrossAmount
                        FROM sa_sales_invoice si, fa_division_setup bs,SA_CUSTOMER_SETUP CS, IP_ITEM_MASTER_SETUP IM
                        WHERE si.deleted_flag = 'N'
                        and si.company_code = '{1}'
                        and SI.DIVISION_CODE = BS.DIVISION_CODE
                        AND SI.CUSTOMER_CODE = CS.CUSTOMER_CODE 
                        AND  IM.ITEM_CODE = SI.ITEM_CODE 
                        and TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'DD') =TO_CHAR(sales_date, 'DD') 
                    ";
            string QueryQuantity = @"
                    (select nvl(SUM (nvl(si.calc_Quantity,0))/{0},0) Quantity
                        FROM sa_sales_invoice si, fa_division_setup bs,SA_CUSTOMER_SETUP CS, IP_ITEM_MASTER_SETUP IM
                        WHERE si.deleted_flag = 'N'
                        and si.company_code = '{1}'
                        AND SI.CUSTOMER_CODE = CS.CUSTOMER_CODE 
                        AND  IM.ITEM_CODE = SI.ITEM_CODE 
                        and SI.DIVISION_CODE = BS.DIVISION_CODE
                        and TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'DD') =TO_CHAR(sales_date, 'DD') 
                    ";
            string QueryNewEnd = @"
                FROM all_objects, fa_division_setup BS,
                   (SELECT start_date, end_date
                      FROM HR_FISCAL_YEAR_CODE
                     WHERE sysdate BETWEEN start_date AND end_date) fiscal_year
                WHERE ROWNUM <= fiscal_year.end_date - fiscal_year.start_date + 1
                and BS.DIVISION_EDESC='{2}'
                GROUP BY  
                BS.DIVISION_EDESC
                ,TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'DD') 
                ,TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MM') 
                ,TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'Month')
                )";
            string condition = string.Empty;
            var customerFilter = string.Empty;
            if (!string.IsNullOrEmpty(customerCode))
            {
                Query += " and (";
                condition += " and (";
                foreach (var item in customerCode.Split(','))
                {
                    Query += "cs.master_customer_code like  (Select MASTER_CUSTOMER_CODE || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND COMPANY_CODE='" + userInfo.company_code + "' ) OR ";
                    condition += "cs.master_customer_code like  (Select MASTER_CUSTOMER_CODE || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND COMPANY_CODE='" + userInfo.company_code + "' ) OR ";
                }
                Query = Query.Substring(0, Query.Length - 3) + ") ";
                condition = condition.Substring(0, condition.Length - 3) + ") ";
            }
            if (reportFilters.CustomerFilter.Count() > 0)
            {
                customerFilter = @"select DISTINCT(customer_code) from sa_customer_setup where (";
                //IF CUSTOMER_SKU_FLAG = G
                foreach (var item in reportFilters.CustomerFilter)
                {
                    customerFilter += "master_customer_code like  (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                customerFilter = customerFilter.Substring(0, customerFilter.Length - 3);
                //IF CUSTOMER_SKU_FLAG = I                
                customerFilter += " or (customer_code in (" + string.Join(",", reportFilters.CustomerFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                condition += " and SI.customer_code IN(" + customerFilter + ")";
            }
            //for item Filter
            if (!string.IsNullOrEmpty(itemCode))
            {
                condition += " and (";
                foreach (var item in itemCode.Split(','))
                {
                    condition += "IM.MASTER_ITEM_CODE LIKE (Select MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' and company_code = '" + userInfo.company_code + "') OR ";
                }
                condition = condition.Substring(0, condition.Length - 3) + ") ";
            }
            var productFilter = string.Empty;

            if (reportFilters.ProductFilter.Count() > 0)
            {
                // condition += " and (";
                productFilter = @"select  DISTINCT item_code from IP_ITEM_MASTER_SETUP where (";
                //IF PRODUCT_SKU_FLAG = G
                foreach (var item in reportFilters.ProductFilter)
                {
                    //condition += "IM.MASTER_ITEM_CODE LIKE (Select MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' and company_code = '" + userInfo.company_code + "') OR ";
                    productFilter += "MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                productFilter = productFilter.Substring(0, productFilter.Length - 3);
                //condition += ")";

                //IF PRODUCT_SKU_FLAG = I                
                productFilter += " or (ITEM_CODE in (" + string.Join(",", reportFilters.ProductFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                condition += " and SI.ITEM_CODE IN(" + productFilter + ")";
            }
            //for customer Filter


            //for item Filter
            if (!string.IsNullOrEmpty(itemCode))
            {
                Query += " and (";
                condition += " and (";
                foreach (var item in itemCode.Split(','))
                {
                    Query += "IM.MASTER_ITEM_CODE LIKE (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND COMPANY_CODE='" + userInfo.company_code + "') OR ";
                    condition += "IM.MASTER_ITEM_CODE LIKE (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND COMPANY_CODE='" + userInfo.company_code + "') OR ";
                }
                Query = Query.Substring(0, Query.Length - 3) + ") ";
                condition = condition.Substring(0, condition.Length - 3) + ") ";
            }


            //for category Filter
            if (!string.IsNullOrEmpty(categoryCode))
            {
                Query += " and (";
                condition += " and (";
                foreach (var item in categoryCode.Split(','))
                {
                    Query += "IM.CATEGORY_CODE = '" + item + "' OR ";
                    condition += "IM.CATEGORY_CODE = '" + item + "' OR ";
                }
                Query = Query.Substring(0, Query.Length - 3) + ") ";
                condition = condition.Substring(0, condition.Length - 3) + ") ";
            }

            //FOR COMPANY FILTER
            if (!string.IsNullOrEmpty(companyCode))
            {
                Query += " AND SI.COMPANY_CODE IN (" + companyCode + ")";
                condition += " AND SI.COMPANY_CODE IN (" + companyCode + ")";
            }


            //FOR BRANCH FILTER
            if (!string.IsNullOrEmpty(branchCode))
            {
                Query += " AND SI.BRANCH_CODE IN (" + branchCode + ")";
                condition += " AND SI.BRANCH_CODE IN (" + branchCode + ")";
            }


            //FOR partyType FILTER
            if (!string.IsNullOrEmpty(partyTypeCode))
            {
                Query += " AND SI.PARTY_TYPE_CODE IN (" + partyTypeCode + ")";
                condition += " AND SI.PARTY_TYPE_CODE IN (" + partyTypeCode + ")";
            }

            //FOR FORMCODE FILER
            if (!string.IsNullOrEmpty(formCode))
            {
                Query += " AND SI.FORM_CODE IN (" + formCode + ")";
                condition += " AND SI.FORM_CODE IN (" + formCode + ")";
            }

            //FOR employee Filter
            if (reportFilters.EmployeeFilter.Count > 0)
            {
                Query += string.Format(@" AND SI.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", reportFilters.EmployeeFilter).ToString());
                condition += string.Format(@" AND SI.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", reportFilters.EmployeeFilter).ToString());
            }
            //FOR Agent Filter
            if (reportFilters.AgentFilter.Count > 0)
            {
                Query += string.Format(@" AND SI.AGENT_CODE IN  ('{0}')", string.Join("','", reportFilters.AgentFilter).ToString());
                condition += string.Format(@" AND SI.AGENT_CODE IN  ('{0}')", string.Join("','", reportFilters.AgentFilter).ToString());
            }
            //FOR division Filter
            if (reportFilters.DivisionFilter.Count > 0)
            {
                Query += string.Format(@" AND SI.DIVISION_CODE IN  ('{0}')", string.Join("','", reportFilters.DivisionFilter).ToString());
                condition += string.Format(@" AND SI.DIVISION_CODE IN  ('{0}')", string.Join("','", reportFilters.DivisionFilter).ToString());
            }
            //FOR Location Filter
            if (reportFilters.LocationFilter.Count > 0)
            {
                string locationFilter = string.Empty;
                var locations = reportFilters.LocationFilter;
                for (int i = 0; i < locations.Count; i++)
                {

                    if (i == 0)
                        locationFilter += string.Format("SELECT LOCATION_CODE FROM IP_LOCATION_SETUP WHERE LOCATION_CODE LIKE '{0}%' ", locations[i]);
                    else
                    {
                        locationFilter += string.Format(" OR LOCATION_CODE like '{0}%' ", locations[i]);
                    }
                }
                locationFilter = string.Format(@" AND SI.FROM_LOCATION_CODE IN ({0} OR LOCATION_CODE IN ('{1}'))", locationFilter, string.Join("','", locations));
                Query += locationFilter;
                condition += locationFilter;
            }

            Query += " GROUP BY BS.DIVISION_EDESC, TO_CHAR(sales_date, 'DD') , TO_CHAR(sales_date, 'MM') ,TO_CHAR(sales_date, 'Month'),TO_NUMBER(TO_CHAR(sales_date, 'DD')) ORDER BY TO_NUMBER(TO_CHAR(sales_date, 'DD')) ASC";
            QueryBS += condition + @"GROUP BY 
                        BS.DIVISION_EDESC
                        ,to_number(substr(bs_date(si.sales_date), 9, 2))
                        ,to_char(substr(bs_date(si.sales_date), 9, 2))
                        ,to_char(substr(bs_date(si.sales_date), 6, 2))
                        ,fn_bs_month(substr(bs_date(si.sales_date), 6, 2))
                        ORDER BY to_number(substr(bs_date(si.sales_date), 9, 2)) asc";
            Query = QueryNew +
                QueryAmount + condition + ") as GrossAmount," +
                QueryQuantity + condition + ") as Quantity" +
                QueryNewEnd;
            Query = string.Format(Query, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), userInfo.company_code, divisionName);
            QueryBS = string.Format(QueryBS, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), userInfo.company_code, divisionName);

            if (string.Equals(DateFormat, "BS"))
            {
                var salesReportList = _objectEntity.SqlQuery<DivisionDaysSalesGraph>(QueryBS).ToList();
                return salesReportList;
            }
            else
            {
                var salesReportList = _objectEntity.SqlQuery<DivisionDaysSalesGraph>(Query).ToList();
                return salesReportList;
            }
        }

        public List<SalesCollectionGraph> GetSalesCollectionMonthlyReport(ReportFiltersModel reportFilters, User userInfo, string dateformat)
        {
            if (userInfo == null)
            {
                userInfo = new Core.Domain.User();
                userInfo.company_code = "01";
                userInfo.branch_code = "01.01";

            }
            else if (string.IsNullOrEmpty(userInfo.company_code) || string.IsNullOrEmpty(userInfo.branch_code))
            {
                userInfo.company_code = "01";
                userInfo.branch_code = "01.01";
            }
            string Query = string.Empty;
            var salesReportList = new List<SalesCollectionGraph>();
            if (string.Equals(dateformat, "BS"))
            {
                Query = string.Format(@"select
                        fn_bs_month(substr(bs_date(voucher_date),6,2)) AS Nepalimonth,
                        substr(bs_date(voucher_date),6,2) AS NepaliMonthInt, 
                        sum(nvl(dr_amount,0))/'{0}' as Sales,
                        sum(nvl(cr_amount,0))/'{0}' as Collection
                    from v$virtual_sub_ledger
                    WHERE  company_code = '{1}'
                    group by fn_bs_month(substr(bs_date(voucher_date),6,2))
                    ,substr(bs_date(voucher_date),6,2)
                    order by NepaliMonthInt", ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), userInfo.company_code, userInfo.branch_code);
                salesReportList = _objectEntity.SqlQuery<SalesCollectionGraph>(Query).Where(a => a.NepaliMonth != null).OrderBy(a => a.NepaliMonthInt).ToList();
            }
            else
            {
                //Query = string.Format(@"select
                //                to_char(voucher_date,'fmMonth') as   Month ,
                //                to_char(EXTRACT(month FROM voucher_date)) AS MonthInt, 
                //                sum(nvl(dr_amount,0))/{0} as Sales,
                //                sum(nvl(cr_amount,0))/{0} as Collection
                //            from v$virtual_sub_ledger
                //            WHERE  company_code = '{1}' and branch_code = '{2}'
                //            group by to_char(voucher_date, 'fmMonth')
                //            ,to_char(EXTRACT(month FROM voucher_date))
                //            order by 1 ", ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), userInfo.company_code, userInfo.branch_code);

                Query = string.Format(@"SELECT  to_char(fiscal_year.start_date + ROWNUM - 1, 'fmMonth') as Month,
                    to_char(fiscal_year.start_date + ROWNUM - 1, 'MM') AS MonthInt, 
                    (select
                     nvl(SUM (nvl(dr_amount,0))/{0},0) as Sales
                    from v$virtual_sub_ledger
                    WHERE  company_code = '{1}' and branch_code = '{2}' and to_char(fiscal_year.start_date + ROWNUM - 1, 'fmMonth')= to_char(voucher_date, 'fmMonth'))  as Sales,
                    (select
                    nvl(SUM (nvl(cr_amount,0))/{0},0) as Collection
                    from v$virtual_sub_ledger
                    WHERE  company_code = '{1}' and branch_code = '{2}' and to_char(fiscal_year.start_date + ROWNUM - 1, 'fmMonth')= to_char(voucher_date, 'fmMonth'))  as Collection
                              FROM all_objects,
                                   (SELECT start_date, end_date
                                      FROM HR_FISCAL_YEAR_CODE
                                     WHERE sysdate BETWEEN start_date AND end_date) fiscal_year
                             WHERE ROWNUM <= fiscal_year.end_date - fiscal_year.start_date + 1
                             group by  to_char(fiscal_year.start_date + ROWNUM - 1, 'fmMonth'),
                             to_char(fiscal_year.start_date + ROWNUM - 1, 'MM')
order by to_char(fiscal_year.start_date + ROWNUM - 1, 'MM')"
                , ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), userInfo.company_code, userInfo.branch_code);
                salesReportList = _objectEntity.SqlQuery<SalesCollectionGraph>(Query)
                    .Where(a => a.Month != null).OrderBy(a => a.MonthInt).ToList();
            }
            return salesReportList;
        }

        public List<TargetCollectionGraph> GetTargetCollectionMonthlyReport(ReportFiltersModel reportFilters, User userInfo, string dateformat)
        {
            string Query = string.Empty;
            var salesReportList = new List<TargetCollectionGraph>();

            var companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            string branchCodeCondition = string.Empty;
            if (reportFilters.BranchFilter.Count() > 0)
            {
                branchCodeCondition = "and branch_code in (" + string.Join(",", reportFilters.BranchFilter.Where(x => x != null)) + ")";// userInfo.branch_code;
            }

            if (string.Equals(dateformat, "BS"))
            {
                Query = string.Format(@"
                        select years,monthint,month,sum(Target) as Target,sum(collection) as Collection,(shortmonth||'-'||years) MonthYear from(
                        SELECT  
                        SUBSTR (bs_date (TO_DATE (fiscal_year.start_date + ROWNUM - 1)),1,4) AS Years,
                       SUBSTR (bs_date (TO_DATE (fiscal_year.start_date + ROWNUM - 1)),6,2)AS monthInt,
                   fn_bs_month (SUBSTR (bs_date (TO_DATE (fiscal_year.start_date + ROWNUM - 1)),6,2))AS shortmonth,
                   fn_bs_month (SUBSTR (bs_date (TO_DATE (fiscal_year.start_date + ROWNUM - 1)),6,2))AS month,
                        (SELECT NVL (SUM (NVL (per_day_amount, 0)) / {0}, 0)
                      FROM pl_sales_plan_dtl
                     WHERE     company_code ='{1}' {2}
                           AND bs_date(to_date(fiscal_year.start_date + ROWNUM - 1)) = bs_date(TO_date (start_date)))  as Target,
                        (select
                            nvl(SUM(nvl(cr_amount,0))/{0},0)
                            from v$virtual_sub_ledger
                            WHERE  company_code = '{1}' {2}
                              AND TO_CHAR (fiscal_year.start_date + ROWNUM - 1,'yyy-MM') = TO_CHAR (voucher_date, 'yyy-MM'))  as collection
                                      FROM all_objects,
                                           (SELECT start_date, end_date
                                              FROM HR_FISCAL_YEAR_CODE
                                             WHERE sysdate BETWEEN start_date AND end_date) fiscal_year
                                     WHERE ROWNUM <= fiscal_year.end_date - fiscal_year.start_date + 1
                                     group by                                     
                       SUBSTR (bs_date (TO_DATE (fiscal_year.start_date + ROWNUM - 1)),1,4),
                        bs_date(to_date(fiscal_year.start_date + ROWNUM - 1)),
                       bs_date(TO_date (start_date)),
                       TO_CHAR (fiscal_year.start_date + ROWNUM - 1,'yyy-MM') 
                        ) t1 
                        group by years,monthint,month,(shortmonth||'-'||years)
                        order by to_number(years||monthint)", ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), userInfo.company_code, branchCodeCondition);
                salesReportList = _objectEntity.SqlQuery<TargetCollectionGraph>(Query).Where(a => a.Month != null).ToList();
            }
            else
            {
                //Query = string.Format(@"select
                //                to_char(voucher_date,'fmMonth') as   Month ,
                //                to_char(EXTRACT(month FROM voucher_date)) AS MonthInt, 
                //                sum(nvl(dr_amount,0))/{0} as Sales,
                //                sum(nvl(cr_amount,0))/{0} as Collection
                //            from v$virtual_sub_ledger
                //            group by to_char(voucher_date, 'fmMonth')
                //            ,to_char(EXTRACT(month FROM voucher_date))
                //            order by 1 ", ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter));

                //Query = string.Format(@"
                //SELECT  to_char(fiscal_year.start_date + ROWNUM - 1, 'fmMonth') as month,
                //            to_char(fiscal_year.start_date + ROWNUM - 1, 'MM'),
                //            (select
                //                nvl(SUM(nvl(dr_amount,0))/{0},0) as Sales
                //                from v$virtual_sub_ledger
                //                WHERE  company_code = '{1}' {2} and to_char(fiscal_year.start_date + ROWNUM - 1, 'fmMonth')= to_char(voucher_date, 'fmMonth'))  as sales,(select
                //                nvl(SUM(nvl(cr_amount,0))/{0},0) as Sales
                //                from v$virtual_sub_ledger
                //                WHERE  company_code = '{1}' {2} and to_char(fiscal_year.start_date + ROWNUM - 1, 'fmMonth')= to_char(voucher_date, 'fmMonth'))  as collection
                //                          FROM all_objects,
                //                               (SELECT start_date, end_date
                //                                  FROM HR_FISCAL_YEAR_CODE
                //                                 WHERE sysdate BETWEEN start_date AND end_date) fiscal_year
                //                         WHERE ROWNUM <= fiscal_year.end_date - fiscal_year.start_date + 1
                //                         group by to_char(fiscal_year.start_date + ROWNUM - 1, 'fmMonth'),
                //            to_char(fiscal_year.start_date + ROWNUM - 1, 'MM')
                //            order by to_char(fiscal_year.start_date + ROWNUM - 1, 'MM')",
                //                         ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), userInfo.company_code, branchCodeCondition);
                Query = string.Format(@"
                        SELECT years,monthint,month,Target,collection,(shortmonth || '-' || years) MonthYear
                            FROM (  SELECT TO_CHAR (fiscal_year.start_date + ROWNUM - 1, 'YYY') AS Years,
                                           TO_CHAR (fiscal_year.start_date + ROWNUM - 1, 'MM') AS monthInt,
                                           TO_CHAR (fiscal_year.start_date + ROWNUM - 1, 'MON') AS shortmonth,
                                           TO_CHAR (fiscal_year.start_date + ROWNUM - 1, 'fmMonth') AS month,
                                           (SELECT NVL (SUM (NVL (per_day_amount, 0)) / {0}, 0) AS target
                                              FROM pl_sales_plan_dtl
                                             WHERE company_code = '{1}' {2}
                                                   AND TO_CHAR (fiscal_year.start_date + ROWNUM - 1,'YYY-MM') = TO_CHAR (start_date, 'YYY-MM')) AS Target,
                                           (SELECT NVL (SUM (NVL (cr_amount, 0)) / {0}, 0) AS Target
                                              FROM v$virtual_sub_ledger
                                             WHERE     company_code = '{1}' {2}
                                                   AND TO_CHAR (fiscal_year.start_date + ROWNUM - 1,'MON') = TO_CHAR (voucher_date, 'MON') 
                                                   AND TO_CHAR (fiscal_year.start_date + ROWNUM - 1,'YYY') = TO_CHAR (voucher_date, 'YYY')) AS collection
                                      FROM all_objects,
                                           (SELECT start_date, end_date
                                              FROM HR_FISCAL_YEAR_CODE
                                             WHERE SYSDATE BETWEEN start_date AND end_date) fiscal_year
                                     WHERE ROWNUM <= fiscal_year.end_date - fiscal_year.start_date + 1
                                  GROUP BY TO_CHAR (fiscal_year.start_date + ROWNUM - 1, 'MON'),
                                           TO_CHAR (fiscal_year.start_date + ROWNUM - 1, 'MM'),
                                           TO_CHAR (fiscal_year.start_date + ROWNUM - 1, 'YYY'),
                                           TO_CHAR (fiscal_year.start_date + ROWNUM - 1, 'fmMonth'),
                                           TO_CHAR (fiscal_year.start_date + ROWNUM - 1, 'YYY-MM'),
                                           TO_CHAR (start_date, 'YYY-MM')) t1
                        ORDER BY TO_NUMBER (years || monthint)",
                        ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), userInfo.company_code, branchCodeCondition);
                salesReportList = _objectEntity.SqlQuery<TargetCollectionGraph>(Query).Where(a => a.Month != null).ToList();
            }

            return salesReportList;
        }

        public List<SalesCollectionGraph> GetSalesCollectionMonthlyReport(ReportFiltersModel reportFilters, User userInfo, string dateformat, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode)
        {
            //companyCode = string.Join(",", reportFilters.CompanyFilter);
            //companyCode = companyCode == "" ? userInfo.company_code : companyCode;
            //var companyCode = string.Join(",'", reportFilters.CompanyFilter);
            companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            var subQuery = string.Empty;
            var innersubquery = string.Empty;
            if (reportFilters.BranchFilter.Count > 0)
            {
                subQuery += string.Format(@" AND l.BRANCH_CODE IN ('{0}')", string.Join("','", reportFilters.BranchFilter));
                innersubquery = string.Format(@" AND BRANCH_CODE IN ('{0}')", string.Join("','", reportFilters.BranchFilter));
            }
            if (reportFilters.DivisionFilter.Count > 0)
            {
                subQuery = subQuery + string.Format(@" AND l.DIVISION_CODE IN  ('{0}')", string.Join("','", reportFilters.DivisionFilter).ToString());
                innersubquery = string.Format(@" AND DIVISION_CODE IN ('{0}')", string.Join("','", reportFilters.DivisionFilter));
            }

            //new query by bikalpa sir
            string Query = string.Empty;
            if (string.Equals(dateformat, "AD"))
            {
                Query = $@" select  monthInt,Month,MonthYear,(select sum(total_sales)/{ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter)}  from V$SALES_INVOICE_REPORT3 where deleted_flag='N' and company_code=c.company_code and  TO_CHAR(sales_date, 'MON-YYY')=c.MonthYear {innersubquery} ) Sales from (SELECT TO_CHAR (l.voucher_date, 'YYYYMM') AS monthInt,
                                         TO_CHAR (l.voucher_date, 'fmMonth') AS Month,             
                                         TO_CHAR (l.voucher_date, 'MON-YYY') AS MonthYear, 
                                         SUM (NVL (l.cr_amount, 0)) /{ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter)} AS Collection,
                                         l.company_code
                                      FROM v$virtual_sub_ledger l
                                      WHERE l.SUB_LEDGER_FLAG = 'C'
                                     AND l.DELETED_FLAG = 'N'
                                     AND l.FORM_CODE <> 0
                                     AND l.company_code IN ({companyCode})
 {subQuery}
                           GROUP BY TO_CHAR (l.voucher_date, 'fmMonth'),
                                     TO_CHAR (l.voucher_date, 'fmMonth'),
                                     TO_CHAR (l.voucher_date, 'YYYYMM'),
                                     TO_CHAR (l.voucher_date, 'MON-YYY'),
                                       l.company_code
                            ORDER BY TO_CHAR (l.voucher_date, 'YYYYMM')) c";

                //Query = string.Format(@"SELECT TO_CHAR (l.voucher_date, 'YYYYMM') AS monthInt,
                //                         TO_CHAR (l.voucher_date, 'fmMonth') AS Month,             
                //                         TO_CHAR (l.voucher_date, 'MON-YYY') AS MonthYear,         
                //                         SUM (NVL (l.dr_amount, 0)) /{0} AS Sales,
                //                         SUM (NVL (l.cr_amount, 0)) /{0} AS Collection
                //                      FROM v$virtual_sub_ledger l
                //                      WHERE l.SUB_LEDGER_FLAG = 'C'
                //                     AND l.DELETED_FLAG = 'N'
                //                     AND l.FORM_CODE <> 0
                //                     AND l.company_code IN ({1})"
                //                    , ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode);
            }
            else
            {
                Query = $@"select Nepalimonth,NepaliMonthInt,NepaliMonthInt monthInt,Collection,(select sum(total_sales)/{ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter)}  from V$SALES_INVOICE_REPORT3 where deleted_flag='N' and company_code=c.company_code  and substr(bs_date(sales_date),6,2)=c.NepaliMonthInt {innersubquery}) as Sales from ( select
                        trim(substr(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(l.voucher_date),6,2)),5,20)) AS Nepalimonth,
                        substr(bs_date(l.voucher_date),6,2) AS NepaliMonthInt, 
    l.company_code,
                        sum(nvl(l.cr_amount,0))/{ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter)} as Collection
                    from v$virtual_sub_ledger l
                     where L.SUB_LEDGER_FLAG = 'C'
                     AND l.DELETED_FLAG = 'N'
                     AND l.FORM_CODE <> '0'
                     AND l.company_code IN({companyCode})
                     {subQuery}
                         group by fn_bs_month(substr(bs_date(voucher_date),6,2)),substr(bs_date(voucher_date),1,4) || substr(bs_date(voucher_date),6,2),
                     substr(bs_date(voucher_date),6,2),l.company_code
                     order by  substr(bs_date(voucher_date),1,4) || substr(bs_date(voucher_date),6,2))  c";

                //Query = string.Format(@"select
                //        trim(substr(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(l.voucher_date),6,2)),5,20)) AS Nepalimonth,
                //        substr(bs_date(l.voucher_date),6,2) AS NepaliMonthInt, 
                //        substr(bs_date(l.voucher_date),6,2) AS monthInt,
                //        sum(nvl(l.dr_amount,0))/{0} as Sales,
                //        sum(nvl(l.cr_amount,0))/{0} as Collection
                //    from v$virtual_sub_ledger l
                //     where L.SUB_LEDGER_FLAG = 'C'
                //     AND l.DELETED_FLAG = 'N'
                //     AND l.FORM_CODE <> '0'
                //     AND l.company_code IN({1})"
                //     , ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode);
            }

            //for branch filter
            //if (reportFilters.BranchFilter.Count > 0)
            //{
            //    Query += string.Format(@" AND l.BRANCH_CODE IN ('{0}')", string.Join("','", reportFilters.BranchFilter));
            //}
            //if (reportFilters.DivisionFilter.Count > 0)
            //{
            //    Query = Query + string.Format(@" AND l.DIVISION_CODE IN  ('{0}')", string.Join("','", reportFilters.DivisionFilter).ToString());
            //}

            //if (string.Equals(dateformat, "AD"))
            //{
            //    Query += @"
            //               GROUP BY TO_CHAR (l.voucher_date, 'fmMonth'),
            //                         TO_CHAR (l.voucher_date, 'fmMonth'),
            //                         TO_CHAR (l.voucher_date, 'YYYYMM'),
            //                         TO_CHAR (l.voucher_date, 'MON-YYY')
            //                ORDER BY TO_CHAR (l.voucher_date, 'YYYYMM')";
            //}
            //else
            //{
            //    //Query += @"
            //    //         group by fn_bs_month(substr(bs_date(voucher_date),6,2)),substr(bs_date(voucher_date),1,4) || substr(bs_date(voucher_date),6,2),
            //    //     substr(bs_date(voucher_date),6,2)
            //    //     order by  substr(bs_date(voucher_date),1,4) || substr(bs_date(voucher_date),6,2)";
            //}

            var salesReportList = _objectEntity.SqlQuery<SalesCollectionGraph>(Query).ToList();
            return salesReportList;




        }


        public List<SalesTargetGraph> GetSalesTargetMonthlyReport(ReportFiltersModel reportFilters, User userInfo, string dateformat)
        {

            //var companyCode = string.Join(",", reportFilters.CompanyFilter);
            //companyCode = companyCode == "" ? userInfo.company_code : companyCode;
            var companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            string Query = string.Empty;
            //****************************
            //CONDITIONS FITLER START HERE
            //****************************

            //for customer Filter
            var customerFilter = string.Empty;
            if (reportFilters.CustomerFilter.Count() > 0)
            {
                customerFilter = @"select DISTINCT(customer_code) from sa_customer_setup where (";
                //IF CUSTOMER_SKU_FLAG = G
                foreach (var item in reportFilters.CustomerFilter)
                {
                    customerFilter += "master_customer_code like  (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                customerFilter = customerFilter.Substring(0, customerFilter.Length - 3);
                //IF CUSTOMER_SKU_FLAG = I                
                customerFilter += " or (customer_code in (" + string.Join(",", reportFilters.CustomerFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                customerFilter = " and SI.customer_code IN(" + customerFilter + ")";
            }




            ////for Product Filter
            var productFilter = string.Empty;
            if (reportFilters.ProductFilter.Count() > 0)
            {
                productFilter = @"select  DISTINCT item_code from IP_ITEM_MASTER_SETUP where (";
                //IF PRODUCT_SKU_FLAG = G
                foreach (var item in reportFilters.ProductFilter)
                {
                    productFilter += "MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                productFilter = productFilter.Substring(0, productFilter.Length - 3);
                //IF PRODUCT_SKU_FLAG = I                
                productFilter += " or (ITEM_CODE in (" + string.Join(",", reportFilters.ProductFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                productFilter = " and SI.ITEM_CODE IN(" + productFilter + ")";
            }

            var branchFilter = string.Empty;
            var planBranchFilter = string.Empty;
            if (reportFilters.BranchFilter.Count > 0)
            {
                var branchFilterStringBuilder = string.Empty;
                foreach (var company in reportFilters.BranchFilter)
                {
                    branchFilterStringBuilder += $@"'{company}',";
                }

                branchFilterStringBuilder = branchFilterStringBuilder.TrimEnd(',');
                     branchFilter = string.Format(@" AND SI.BRANCH_CODE IN ({0})", branchFilterStringBuilder);
                planBranchFilter = $@" and BRANCH_CODE IN ({branchFilterStringBuilder})";
            }

            //****************************
            //CONDITIONS FITLER END HERE
            //****************************
            if (dateformat == "AD")
            {
                //Query = string.Format(@"select to_char(SI.sales_date, 'YYY') as Year,
                //     to_char(SI.sales_date, 'YYYYMM') as monthint, 
                //     to_char(SI.sales_date, 'Month') as month, 
                //     to_char(SI.sales_date, 'Mon-YYY') as MonthYear,
                //     nvl(sum(nvl(SI.calc_total_price,0))/{0},0) as Sales,
                //     NVL (SUM (NVL (sales_target, 0)) / {0}, 0) AS target             
                //   from sa_sales_invoice SI,fa_target_setup ts
                //   where
                //       to_char(SI.sales_date, 'YYYYMM') BETWEEN to_char(ts.start_date, 'YYYYMM') AND to_char(ts.end_date, 'YYYYMM')
                //        and SI.deleted_flag='N'
                //        and ts.deleted_flag = 'N'
                //        and si.company_code = ts.company_code
                //        AND SI.COMPANY_CODE IN ({1}) "
                //       , ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode);

                Query = $@"SELECT to_char(SI.sales_date, 'YYY') as Year, DS.BRANCH_EDESC as branch_name, DS.BRANCH_CODE as branch_code,
                            to_char(SI.sales_date, 'Month') as month,
                            to_char(SI.sales_date, 'YYYYMM') as monthint,
                           0 TargetQty,
                             0 TargetAmount,
                            SUM (nvl(si.calc_Quantity,0))/{ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter)} AS Quantity,
                            SUM (nvl(si.calc_total_price,0))/{ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter)} AS GrossAmount,
                            'SALES' DATATYPE
                    FROM sa_sales_invoice si, FA_BRANCH_SETUP DS
                    WHERE  1=1
                    AND si.deleted_flag = 'N' AND si.company_code in ({companyCode})
                    and SI.BRANCH_CODE = DS.BRANCH_CODE
                   AND SI.COMPANY_CODE IN ({companyCode})" + customerFilter + productFilter + branchFilter +
                    $@" GROUP BY DS.BRANCH_CODE, DS.BRANCH_EDESC, to_char(SI.sales_date, 'YYY')
                    ,to_char(SI.sales_date, 'Month')
                    ,to_char(SI.sales_date, 'Month'),to_char(SI.sales_date, 'YYYYMM')
                    UNION  ALL
                     SELECT  to_char(SI.PLAN_DATE,'YYY') as Year,DS.BRANCH_EDESC as branch_name, DS.BRANCH_CODE as branch_code,
                            to_char(si.PLAN_DATE, 'Month') as month,
                            to_char(SI.PLAN_DATE, 'YYYYMM') as monthint,
                            SUM (nvl(si.PER_DAY_QUANTITY,0))/{ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter)} AS TargetQty,
                            SUM (nvl(si.PER_DAY_AMOUNT,0))/{ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter)} AS TargetAmount,
                            0 Quantity,
                            0 GrossAmount,
                               'TARGET' DATATYPE
                    FROM PL_SALES_PLAN_DTL si, FA_BRANCH_SETUP DS,IP_ITEM_MASTER_SETUP I
                    WHERE 1=1
                    AND si.deleted_flag = 'N' AND si.company_code in ({companyCode})
                    AND I.ITEM_CODE=SI.ITEM_CODE
                    AND I.GROUP_SKU_FLAG='I'
                    and SI.BRANCH_CODE = DS.BRANCH_CODE
                    AND SI.COMPANY_CODE IN ({companyCode})";

            }
            else
            {
                //Query = $@"SELECT SUBSTR (bs_date (SI.sales_date), 1, 4) as Year, DS.BRANCH_EDESC as branch_name, DS.BRANCH_CODE as branch_code,
                //            fn_bs_month(substr(bs_date(si.sales_date),6,2)) AS Month,
                //            SUBSTR(BS_DATE(SI.SALES_DATE),6,2) as MonthInt,
                //           0 TargetQty,
                //             0 TargetAmount,
                //            SUM (nvl(si.calc_Quantity,0))/{ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter)} AS Quantity,
                //            SUM (nvl(si.calc_total_price,0))/{ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter)} AS GrossAmount,
                //            'SALES' DATATYPE
                //    FROM sa_sales_invoice si, FA_BRANCH_SETUP DS
                //    WHERE  1=1
                //    AND si.deleted_flag = 'N' AND si.company_code in ({companyCode})
                //    and SI.BRANCH_CODE = DS.BRANCH_CODE
                //   AND SI.COMPANY_CODE IN ({companyCode})" + customerFilter + productFilter + branchFilter +
                //    $@"GROUP BY DS.BRANCH_CODE, DS.BRANCH_EDESC, SUBSTR(BS_DATE(SI.SALES_DATE),6,2)
                //    ,fn_bs_month(substr(bs_date(si.sales_date),6,2))
                //    ,to_number(substr(bs_date(sales_date),6,2)),SUBSTR (bs_date (SI.sales_date), 1, 4)
                //    UNION  ALL
                //    SELECT SUBSTR (bs_date (SI.PLAN_DATE), 1, 4) as Year,DS.BRANCH_EDESC as branch_name, DS.BRANCH_CODE as branch_code,
                //            fn_bs_month(substr(bs_date(si.PLAN_DATE),6,2)) AS Month,
                //            SUBSTR(BS_DATE(SI.PLAN_DATE),6,2) as MonthInt,
                //            SUM (nvl(si.PER_DAY_QUANTITY,0))/{ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter)} AS TargetQty,
                //            SUM (nvl(si.PER_DAY_AMOUNT,0))/{ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter)} AS TargetAmount,
                //            0 Quantity,
                //            0 GrossAmount,
                //               'TARGET' DATATYPE
                //    FROM PL_SALES_PLAN_DTL si, FA_BRANCH_SETUP DS,IP_ITEM_MASTER_SETUP I
                //    WHERE 1=1
                //    AND si.deleted_flag = 'N' AND si.company_code in ({companyCode})
                //    AND I.ITEM_CODE=SI.ITEM_CODE
                //    AND I.GROUP_SKU_FLAG='I'
                //    and SI.BRANCH_CODE = DS.BRANCH_CODE
                //    AND SI.COMPANY_CODE IN ({companyCode}) " + customerFilter + productFilter + branchFilter;

                 Query = $@"SELECT SUBSTR (bs_date (SI.sales_date), 1, 4) as Year, DS.BRANCH_EDESC as branch_name, DS.BRANCH_CODE as branch_code,
                            fn_bs_month(substr(bs_date(si.sales_date),6,2)) AS Month,
                            SUBSTR(BS_DATE(SI.SALES_DATE),6,2) as MonthInt,
                           0 TargetQty,
                             0 TargetAmount,
                            SUM (nvl(si.calc_Quantity,0)) AS Quantity,
                            round(SUM (nvl(si.NET_SALES_RATE,0)*nvl(si.calc_Quantity,0)),2) AS GrossAmount,
                            'SALES' DATATYPE
                    FROM sa_sales_invoice si, FA_BRANCH_SETUP DS
                    WHERE  1=1
                    AND si.deleted_flag = 'N' AND si.company_code in ({companyCode}) {branchFilter} {customerFilter} {productFilter}
                    and SI.BRANCH_CODE = DS.BRANCH_CODE
                   AND SI.COMPANY_CODE IN ({companyCode})  GROUP BY DS.BRANCH_CODE, DS.BRANCH_EDESC, SUBSTR(BS_DATE(SI.SALES_DATE),6,2)
                    ,fn_bs_month(substr(bs_date(si.sales_date),6,2))
                    ,to_number(substr(bs_date(sales_date),6,2)),SUBSTR (bs_date (SI.sales_date), 1, 4)
                    UNION  ALL
                      SELECT YEAR,branch_edesc,branch_code,month,MONTHINT,
                                  SUM(PER_DAY_QUANTITY) TargetQty,
                                  SUM(PER_DAY_AMOUNT) TargetAmount,
                                  0 Quantity,
                            0 GrossAmount,
                               'TARGET' DATATYPE
                                  FROM TEMP_PL_SALES_PLAN_REPORT SI
                                WHERE SI.COMPANY_CODE in ({companyCode}) {branchFilter} {customerFilter} {productFilter}  GROUP BY YEAR,branch_edesc,branch_code,month,MONTHINT";


                //Query = string.Format(@"select SUBSTR (bs_date (SI.sales_date), 1, 4)  as Year,
                //                            SUBSTR(bs_date (SI.sales_date), 6, 2) as monthint, 
                //                            TRIM (SUBSTR (FN_CHARTBS_MONTH (SUBSTR (BS_DATE (SI.sales_date), 6, 2)),5,20)) month, 
                //                            TRIM (SUBSTR (FN_CHARTBS_MONTH (SUBSTR (BS_DATE (SI.sales_date), 6, 2)),5,20))  as MonthYear,       
                //                             nvl(sum(nvl(SI.calc_total_price,0))/{0},0) as Sales,
                //                             NVL (SUM (NVL (sales_target, 0)) /{0}, 0) AS target             
                //                        from fa_target_setup ts,sa_sales_invoice SI
                //                        where
                //                        bs_date (SI.sales_date) BETWEEN  bs_date (ts.start_date) AND  bs_date (ts.end_date)   
                //                        and SI.deleted_flag='N'
                //                       and ts.deleted_flag = 'N'
                //                       and si.company_code = ts.company_code 
                //                        AND SI.COMPANY_CODE IN ({1}) "
                //                      , ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode);


            }








            if (dateformat == "AD")
            {
                //Query += @" 
                //       group by  to_char(SI.sales_date, 'Month'),to_char(SI.sales_date, 'YYYYMM'),to_char(SI.sales_date, 'YYY'),to_char(SI.sales_date, 'Mon-YYY')
                //      order by to_char(SI.sales_date, 'YYYYMM')";
                Query += @" 
                      GROUP BY DS.BRANCH_CODE, DS.BRANCH_EDESC,  to_char(si.PLAN_DATE, 'Month'),
                     to_char(SI.PLAN_DATE,'YYY'),to_char(SI.PLAN_DATE, 'YYYYMM'),
                    to_char(SI.PLAN_DATE, 'YYYYMM') ";
            }
            else
            {
                //Query += @" 
                //       group by SUBSTR (bs_date (SI.sales_date), 1, 4),SUBSTR(bs_date (SI.sales_date), 6, 2)
                //       order by SUBSTR(bs_date (SI.sales_date), 6, 2)";
                //Query += @" 
                //       GROUP BY DS.BRANCH_CODE, DS.BRANCH_EDESC, SUBSTR(BS_DATE(SI.PLAN_DATE),6,2)
                //    ,fn_bs_month(substr(bs_date(si.PLAN_DATE),6,2))
                //    ,to_number(substr(bs_date(PLAN_DATE),6,2)),SUBSTR (bs_date (SI.PLAN_DATE), 1, 4) ";
            }




            //Query = string.Format(Query, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode);
            //var dataCOLLECTION = _objectEntity.SqlQuery<SalesTargetGraph>(Query).ToList();
            //return dataCOLLECTION;
            var datas = this._objectEntity.SqlQuery<SalesTargetViewModel>(Query).ToList();

            var branchwisesales = new List<SalesTargetGraph>();
            foreach (var data in datas.Where(x => x.DataType == "SALES" || x.DataType == "TARGET").OrderBy(x => x.MonthInt).GroupBy(x => x.MonthInt).Select(x => x.FirstOrDefault()).ToList())
            {
                var branchWise = new SalesTargetGraph();
                branchWise.Month = data.Month;
                branchWise.MonthInt = data.MonthInt;
                branchWise.MonthYear = data.Month;
                branchWise.Sales = Convert.ToDecimal(datas.Where(x => x.DataType == "SALES" && x.MonthInt == data.MonthInt).Sum(X => X.GrossAmount));
                branchWise.Target = Convert.ToDecimal(datas.Where(x => x.DataType != "SALES" && x.MonthInt == data.MonthInt).Sum(X => X.TargetAmount));
                branchwisesales.Add(branchWise);
            }

            return branchwisesales;


        }

        //public List<MonthlySalesGraph> GetSalesMonthSummanry(ReportFiltersModel reportFilters, User userInfo)
        //{
        //    string Query = String.Format(@"select to_char(sales_date,'fmMonth') as   Month , 
        //                                sum(nvl(calc_total_price,0))/{0} as Amount,
        //                                sum(nvl(calc_Quantity,0))/{0} as Quantity
        //                            from sa_sales_invoice
        //                            WHERE SSI.COMPANY_CODE = '{1}' AND SSI.BRANCH_CODE = '{2}' 
        //                            group by to_char(sales_date, 'fmMonth')
        //                            order by 1",
        //                            ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), userInfo.company_code, userInfo.branch_code);
        //    var salesReportList = _objectEntity.SqlQuery<MonthlySalesGraph>(Query).ToList();
        //    return salesReportList;
        //}

        //public List<MonthlyBranchSalesGraph> GetMonthlyBranchSalesSummary(ReportFiltersModel reportFilters, User userInfo)
        //{
        //    var Query = string.Format(@"SELECT si.branch_code,
        //                        BS.BRANCH_EDESC as BranchName,
        //                         TO_CHAR (si.sales_date, 'fmMonth') AS Month,
        //                         SUM (nvl(si.calc_total_price,0))/{0} AS Amount,
        //                         SUM (nvl(si.calc_Quantity,0))/{0} AS Quantity
        //                    FROM sa_sales_invoice si, fa_branch_setup bs
        //                   WHERE si.deleted_flag = 'N' AND si.company_code = '01'
        //                   and SI.BRANCH_CODE = BS.BRANCH_CODE
        //                GROUP BY si.branch_code,BS.BRANCH_EDESC, TO_CHAR (si.sales_date, 'fmMonth')
        //                ", ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter));

        //    var salesReportList = _objectEntity.SqlQuery<MonthlyBranchSalesGraph>(Query).ToList();
        //    return salesReportList;
        //}

        //public List<MonthlyBranchSalesGraph> GetMonthlyBranchSalesSummary(ReportFiltersModel reportFilters, User userInfo)
        //{
        //    var Query = string.Format(@"SELECT si.branch_code,
        //                        BS.BRANCH_EDESC as BranchName,
        //                         TO_CHAR (si.sales_date, 'fmMonth') AS Month,
        //                         SUM (nvl(si.calc_total_price,0))/{0} AS Amount,
        //                         SUM (nvl(si.calc_Quantity,0))/{0} AS Quantity
        //                    FROM sa_sales_invoice si, fa_branch_setup bs
        //                   WHERE si.deleted_flag = 'N'
        //                    AND SI.COMPANY_CODE = '{1}'
        //                   and SI.BRANCH_CODE = BS.BRANCH_CODE
        //                GROUP BY si.branch_code,BS.BRANCH_EDESC, TO_CHAR (si.sales_date, 'fmMonth')
        //                ", ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter),userInfo.company_code);

        //    var salesReportList = _objectEntity.SqlQuery<MonthlyBranchSalesGraph>(Query).ToList();
        //    return salesReportList;
        //}

        //public List<MonthlyDivisionSalesGraph> GetMonthlyDivisionSalesSummary(ReportFiltersModel reportFilters, User userInfo)
        //{          
        //    var Query = string.Format(@"SELECT 
        //                                    --si.branch_code,
        //                                    DS.DIVISION_EDESC as DivisionName,
        //                                     TO_CHAR (si.sales_date, 'fmMonth') AS Month,
        //                                     SUM (nvl(si.calc_total_price,0))/{0} AS Amount,
        //                                     SUM (nvl(si.calc_Quantity,0))/{0} AS Quantity
        //                                FROM sa_sales_invoice si, fa_division_setup ds
        //                               WHERE si.deleted_flag = 'N' AND si.company_code = '01'
        //                               and SI.DIVISION_CODE = DS.DIVISION_CODE 
        //                               and si.company_code ='{1}' and si.branch_code = '{2}' 
        //                            GROUP BY 
        //                            --si.branch_code,
        //                            DS.DIVISION_EDESC, TO_CHAR (si.sales_date, 'fmMonth')
        //                ", ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), userInfo.company_code, userInfo.branch_code);

        //    var salesReportList = _objectEntity.SqlQuery<MonthlyDivisionSalesGraph>(Query).ToList();
        //    return salesReportList;
        //}
        public List<MonthlyDivisionSalesGraph> GetMonthlyDivisionSalesSummary(ReportFiltersModel reportFilters, User userInfo, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode, string DateFormat, string AmountType)
        {
            if (userInfo == null)
            {
                userInfo = new Core.Domain.User();
                //userInfo.company_code = "01";
                userInfo.branch_code = "01.01";

            }
            else if (string.IsNullOrEmpty(userInfo.company_code) || string.IsNullOrEmpty(userInfo.branch_code))
            {
                //userInfo.company_code = "01";
                userInfo.branch_code = "01.01";
            }
            //companyCode = string.Join(",", reportFilters.CompanyFilter);
            //companyCode = companyCode == "" ? userInfo.company_code : companyCode;
            //var companyCode = string.Join(",'", reportFilters.CompanyFilter);
            companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }
            var fiscalYearFilter = string.Empty;
            if (reportFilters.FiscalYearFilter.Count > 0)
            {
                fiscalYearFilter = $"{reportFilters.FiscalYearFilter.FirstOrDefault().DBName}.";
            }
            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            //new query from View
            string QueryNew = $@"SELECT DS.DIVISION_EDESC as DivisionName, DS.DIVISION_CODE as DivisionCode,
                            TO_CHAR (si.sales_date, 'fmMonth') AS Month,
                            TO_CHAR (si.sales_date, 'MM') AS MonthInt,
                            SUM (nvl(si.TOTAL_SALES,0)) AS TotalAmount,
                            SUM (nvl(si.QUANTITY,0)) AS Quantity,
                            SUM (nvl(si.GROSS_SALES,0)) AS GrossAmount,
                            SUM (nvl(si.NET_SALES,0)) AS NetAmount
                    FROM {fiscalYearFilter}V$SALES_INVOICE_REPORT3 si, {fiscalYearFilter}fa_division_setup ds,{fiscalYearFilter}SA_CUSTOMER_SETUP CS
                    WHERE  SI.CUSTOMER_CODE = CS.CUSTOMER_CODE
                    AND si.deleted_flag = 'N' AND si.company_code = '" + userInfo.company_code + @"'
                    and SI.DIVISION_CODE = DS.DIVISION_CODE";
            string QueryNewBS = $@"SELECT DS.DIVISION_EDESC as DivisionName, DS.DIVISION_CODE as DivisionCode,
                            {fiscalYearFilter}fn_bs_month(substr({fiscalYearFilter}bs_date(si.sales_date),6,2)) AS Month,
                            SUBSTR({fiscalYearFilter}BS_DATE(SI.SALES_DATE),6,2) as MonthInt,
                            SUM (nvl(si.TOTAL_SALES,0)) AS TotalAmount,
                            SUM (nvl(si.QUANTITY,0)) AS Quantity,
                            SUM (nvl(si.GROSS_SALES,0)) AS GrossAmount,
                            SUM (nvl(si.NET_SALES,0)) AS NetAmount
                    FROM {fiscalYearFilter}V$SALES_INVOICE_REPORT3 si, {fiscalYearFilter}fa_division_setup ds,{fiscalYearFilter}SA_CUSTOMER_SETUP CS
                    WHERE  SI.CUSTOMER_CODE = CS.CUSTOMER_CODE
                    AND si.deleted_flag = 'N' AND si.company_code ='" + userInfo.company_code + @"'
                    and SI.DIVISION_CODE = DS.DIVISION_CODE";
            //Old Query from table
            string Query = $@"SELECT DS.DIVISION_EDESC as DivisionName, DS.DIVISION_CODE as DivisionCode,
                            TO_CHAR (si.sales_date, 'fmMonth') AS Month,
                            TO_CHAR (si.sales_date, 'MM') AS MonthInt,
                            SUM (nvl(si.calc_Quantity,0)) AS Quantity,
                            SUM (nvl(si.calc_total_price,0)) AS GrossAmount
                    FROM {fiscalYearFilter}sa_sales_invoice si, {fiscalYearFilter}fa_division_setup DS,{fiscalYearFilter}SA_CUSTOMER_SETUP CS, {fiscalYearFilter}IP_ITEM_MASTER_SETUP IM
                    WHERE  SI.CUSTOMER_CODE = CS.CUSTOMER_CODE
   and si.company_code=cs.company_code
                    and si.company_code=ds.company_code
                    and si.company_code=IM.COMPANY_CODE
                    AND si.deleted_flag = 'N' AND si.company_code = '" + userInfo.company_code + @"'
                    AND SI.DIVISION_CODE = DS.DIVISION_CODE
                    AND IM.ITEM_CODE = SI.ITEM_CODE";
            string QueryBS = $@"SELECT DS.DIVISION_EDESC as DivisionName, DS.DIVISION_CODE as DivisionCode,
                            {fiscalYearFilter}fn_bs_month(substr(bs_date(si.sales_date),6,2)) AS Month,
                            SUBSTR(BS_DATE(SI.SALES_DATE),6,2) as MonthInt,
                            SUM (nvl(si.calc_Quantity,0)) AS Quantity,
                            SUM (nvl(si.calc_total_price,0)) AS GrossAmount
                    FROM {fiscalYearFilter}sa_sales_invoice si, {fiscalYearFilter}fa_division_setup DS,{fiscalYearFilter}SA_CUSTOMER_SETUP CS, {fiscalYearFilter}IP_ITEM_MASTER_SETUP IM
                    WHERE  SI.CUSTOMER_CODE = CS.CUSTOMER_CODE
   and si.company_code=cs.company_code
                    and si.company_code=ds.company_code
                    and si.company_code=IM.COMPANY_CODE
                    AND si.deleted_flag = 'N' AND si.company_code ='" + userInfo.company_code + @"'
                    and SI.DIVISION_CODE = DS.DIVISION_CODE
                    AND IM.ITEM_CODE = SI.ITEM_CODE";


            string condition = string.Empty;
            string conditionNew = string.Empty;
            //for customer Filter
            if (!string.IsNullOrEmpty(customerCode))
            {
                condition += " and (";
                foreach (var item in customerCode.Split(','))
                {
                    condition += $"cs.master_customer_code like  (Select MASTER_CUSTOMER_CODE || '%'  from {fiscalYearFilter}SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' and company_code = '" + userInfo.company_code + "') OR ";
                }
                condition = condition.Substring(0, condition.Length - 3) + ") ";
                conditionNew = condition;
            }
            var customerFilter = string.Empty;
            if (reportFilters.CustomerFilter.Count() > 0)
            {
                customerFilter = $@"select DISTINCT(customer_code) from {fiscalYearFilter}sa_customer_setup where (";
                //IF CUSTOMER_SKU_FLAG = G
                foreach (var item in reportFilters.CustomerFilter)
                {
                    customerFilter += $"master_customer_code like  (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from {fiscalYearFilter}SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                customerFilter = customerFilter.Substring(0, customerFilter.Length - 3);
                //IF CUSTOMER_SKU_FLAG = I                
                customerFilter += " or (customer_code in (" + string.Join(",", reportFilters.CustomerFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                condition += " and SI.customer_code IN(" + customerFilter + ")";
            }
            //for item Filter
            if (!string.IsNullOrEmpty(itemCode))
            {
                condition += " and (";
                foreach (var item in itemCode.Split(','))
                {
                    condition += $"IM.MASTER_ITEM_CODE LIKE (Select MASTER_ITEM_CODE || '%'  from {fiscalYearFilter}IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' and company_code = '" + userInfo.company_code + "') OR ";
                }
                condition = condition.Substring(0, condition.Length - 3) + ") ";
            }
            var productFilter = string.Empty;

            if (reportFilters.ProductFilter.Count() > 0)
            {
                // condition += " and (";
                productFilter = $@"select  DISTINCT item_code from {fiscalYearFilter}IP_ITEM_MASTER_SETUP where (";
                //IF PRODUCT_SKU_FLAG = G
                foreach (var item in reportFilters.ProductFilter)
                {
                    //condition += "IM.MASTER_ITEM_CODE LIKE (Select MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' and company_code = '" + userInfo.company_code + "') OR ";
                    productFilter +=$"MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from {fiscalYearFilter}IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                productFilter = productFilter.Substring(0, productFilter.Length - 3);
                //condition += ")";

                //IF PRODUCT_SKU_FLAG = I                
                productFilter += " or (ITEM_CODE in (" + string.Join(",", reportFilters.ProductFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                condition += " and SI.ITEM_CODE IN(" + productFilter + ")";
            }

            //for category Filter
            if (!string.IsNullOrEmpty(categoryCode))
            {
                condition += " and (";
                foreach (var item in categoryCode.Split(','))
                {
                    condition += "IM.CATEGORY_CODE = '" + item + "' OR ";
                }
                condition = condition.Substring(0, condition.Length - 3) + ") ";
            }

            if (reportFilters.CategoryFilter.Count > 0)
            {
                condition += " and (";
                foreach (var item in reportFilters.CategoryFilter)
                {
                    condition += "IM.CATEGORY_CODE = '" + item + "' OR ";
                }
                condition = condition.Substring(0, condition.Length - 3) + ") ";
            }

            //FOR COMPANY FILTER
            if (!string.IsNullOrEmpty(companyCode))
            {
                condition += " AND SI.COMPANY_CODE IN (" + companyCode + ")";
            }

            //FOR BRANCH FILTER
            if (!string.IsNullOrEmpty(branchCode))
            {
                condition += " AND SI.BRANCH_CODE IN (" + branchCode + ")";
            }

            if (reportFilters.BranchFilter.Count > 0)
            {
                condition += $@" AND SI.BRANCH_CODE IN ({string.Join(",", reportFilters.BranchFilter)})";
            }

            //FOR partyType FILTER
            if (!string.IsNullOrEmpty(partyTypeCode))
            {
                condition += " AND SI.PARTY_TYPE_CODE IN (" + partyTypeCode + ")";
            }

            //FOR FORMCODE FILER
            if (!string.IsNullOrEmpty(formCode))
            {
                condition += " AND SI.FORM_CODE IN (" + formCode + ")";
            }
            //FOR employee Filter
            if (reportFilters.EmployeeFilter.Count > 0)
            {
                condition += string.Format(@" AND SI.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", reportFilters.EmployeeFilter).ToString());
            }
            //FOR Agent Filter
            if (reportFilters.AgentFilter.Count > 0)
            {
                condition += string.Format(@" AND SI.AGENT_CODE IN  ('{0}')", string.Join("','", reportFilters.AgentFilter).ToString());
            }
            //FOR divison Filter
            if (reportFilters.DivisionFilter.Count > 0)
            {
                condition += string.Format(@" AND SI.DIVISION_CODE IN  ('{0}')", string.Join("','", reportFilters.DivisionFilter).ToString());
                conditionNew += string.Format(@" AND SI.DIVISION_CODE IN  ('{0}')", string.Join("','", reportFilters.DivisionFilter).ToString());
            }
            //FOR Location Filter
            if (reportFilters.LocationFilter.Count > 0)
            {
                string locationFilter = string.Empty;
                var locations = reportFilters.LocationFilter;
                for (int i = 0; i < locations.Count; i++)
                {

                    if (i == 0)
                        locationFilter += string.Format("SELECT LOCATION_CODE FROM IP_LOCATION_SETUP WHERE LOCATION_CODE LIKE '{0}%' ", locations[i]);
                    else
                    {
                        locationFilter += string.Format(" OR LOCATION_CODE like '{0}%' ", locations[i]);
                    }
                }
                locationFilter = string.Format(@" AND SI.FROM_LOCATION_CODE IN ({0} OR LOCATION_CODE IN ('{1}'))", locationFilter, string.Join("','", locations));
                condition += locationFilter;
            }
            //Filters for new query
            QueryNew += conditionNew +
                @" GROUP BY DS.DIVISION_EDESC, DS.DIVISION_CODE, TO_CHAR (si.sales_date, 'fmMonth'),TO_CHAR (si.sales_date, 'MM'),to_number(to_char(si.sales_date, 'MM'))
                        order by to_number(to_char(si.sales_date, 'MM')) asc";
            QueryNewBS += conditionNew +
                @" GROUP BY DS.DIVISION_EDESC,DS.DIVISION_CODE, SUBSTR(BS_DATE(SI.SALES_DATE),6,2)
                    ,fn_bs_month(substr(bs_date(si.sales_date),6,2))
                    ,to_number(substr(bs_date(sales_date),6,2))
                    Order by to_number(substr(bs_date(sales_date),6,2)) asc";

            //FIlters for old query
            Query += condition +
                @" GROUP BY DS.DIVISION_EDESC, DS.DIVISION_CODE, TO_CHAR (si.sales_date, 'fmMonth'),TO_CHAR (si.sales_date, 'MM'),to_number(to_char(si.sales_date, 'MM'))
                        order by to_number(to_char(si.sales_date, 'MM')) asc";
            QueryBS += condition +
                @" GROUP BY DS.DIVISION_EDESC, DS.DIVISION_CODE, SUBSTR(BS_DATE(SI.SALES_DATE),6,2)
                    ,fn_bs_month(substr(bs_date(si.sales_date),6,2))
                    ,to_number(substr(bs_date(sales_date),6,2))
                    Order by to_number(substr(bs_date(sales_date),6,2)) asc";

            Query = string.Format(Query, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter));
            QueryBS = string.Format(QueryBS, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter));
            QueryNew = string.Format(QueryNew, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter));
            QueryNewBS = string.Format(QueryNewBS, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter));

            string QueryFinal = string.Empty;
            if (AmountType == "NetAmount" || AmountType == "TotalAmount")
            {
                QueryFinal = string.Equals(DateFormat, "BS") ? QueryNewBS : QueryNew;
            }
            else
            {
                QueryFinal = string.Equals(DateFormat, "BS") ? QueryBS : Query;
            }

            var salesReportList = _objectEntity.SqlQuery<MonthlyDivisionSalesGraph>(QueryFinal).ToList();
            return salesReportList;
        }

        public List<Employee> GetEmployeesList(User userInfo)
        {
            var Query = string.Format(@"SELECT si.employee_code as EmployeeCode,
                                            es.employee_edesc as EmployeeName                                            
                                        FROM sa_sales_invoice si, hr_employee_setup es
                                       WHERE si.deleted_flag = 'N' AND si.company_code = '{0}'
                                       and SI.Employee_code = es.Employee_code
                                    GROUP BY si.employee_code,es.employee_edesc", userInfo.company_code);

            var employeeList = _objectEntity.SqlQuery<Employee>(Query).ToList();
            return employeeList;
        }
        public List<Agent> GetAgentList(User userInfo)
        {
            var Query = string.Format(@"SELECT AGENT_CODE as AgentCode,
            AGENT_EDESC as AgentName
FROM AGENT_SETUP
WHERE DELETED_FLAG ='N'");

            var AgentList = _objectEntity.SqlQuery<Agent>(Query).ToList();
            return AgentList;
        }
        public List<Division> GetDivisionList(User userInfo)
        {
            var Query = string.Format(@"SELECT DIVISION_CODE as DivisionCode,
            DIVISION_EDESC as DivisionName
FROM FA_DIVISION_SETUP
WHERE DELETED_FLAG='N' AND GROUP_SKU_FLAG='I'");

            var DivisionList = _objectEntity.SqlQuery<Division>(Query).ToList();
            return DivisionList;
        }


        public List<EmployeeWiseReport> GetEmployeesSalesReport(ReportFiltersModel reportFilters, User userInfo)
        {
            if (userInfo == null)
            {
                userInfo = new Core.Domain.User();
                //userInfo.company_code = "01";
                userInfo.branch_code = "01.01";

            }
            else if (string.IsNullOrEmpty(userInfo.company_code) || string.IsNullOrEmpty(userInfo.branch_code))
            {
                //userInfo.company_code = "01";
                userInfo.branch_code = "01.01";
            }
            //var companyCode = string.Join(",'", reportFilters.CompanyFilter);
            var companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            var fiscalYearFilter = string.Empty;
            if (reportFilters.FiscalYearFilter.Count > 0)
            {
                fiscalYearFilter = $"{reportFilters.FiscalYearFilter.FirstOrDefault().DBName}.";
            }
            var Query = string.Format($@"SELECT si.employee_code as EmployeeCode,
                                            es.employee_edesc as EmployeeName,
                                            -- TO_CHAR (si.sales_date, 'fmMonth') AS Month,
                                             SUM (nvl(si.calc_total_price,0)) AS Amount,
                                             SUM (nvl(si.calc_Quantity,0)) AS Quantity
                                        FROM {fiscalYearFilter}sa_sales_invoice si, {fiscalYearFilter}hr_employee_setup es
                                       WHERE si.deleted_flag = 'N' 
                                       AND SI.COMPANY_CODE IN('{userInfo.company_code}')
                                       and SI.Employee_code = es.Employee_code
                                      and si.company_code=es.company_code
                                    GROUP BY si.employee_code,es.employee_edesc--, TO_CHAR (si.sales_date, 'fmMonth')
                        ");

            var salesReportList = _objectEntity.SqlQuery<EmployeeWiseReport>(Query).ToList();
            return salesReportList;
        }


        public List<EmployeeWiseReport> GetEmployeesSalesReport(ReportFiltersModel reportFilters, User userInfo, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode, string employeeCode)
        {
            if (userInfo == null)
            {
                userInfo = new Core.Domain.User();
                userInfo.company_code = "01";
                userInfo.branch_code = "01.01";

            }
            else if (string.IsNullOrEmpty(userInfo.company_code) || string.IsNullOrEmpty(userInfo.branch_code))
            {
                userInfo.company_code = "01";
                userInfo.branch_code = "01.01";
            }
            var fiscalYearFilter = string.Empty;
            if (reportFilters.FiscalYearFilter.Count > 0)
            {
                fiscalYearFilter = $"{reportFilters.FiscalYearFilter.FirstOrDefault().DBName}.";
            }
             companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            string Query = $@"SELECT si.employee_code as EmployeeCode,
                        es.employee_edesc as EmployeeName,                           
                            SUM (nvl(si.calc_total_price,0)) AS Amount,
                            SUM (nvl(si.calc_Quantity,0)) AS Quantity
                    FROM {fiscalYearFilter}sa_sales_invoice si, {fiscalYearFilter}hr_employee_setup es,{fiscalYearFilter}SA_CUSTOMER_SETUP CS,{fiscalYearFilter}IP_ITEM_MASTER_SETUP IM                                  
                    WHERE  SI.CUSTOMER_CODE = CS.CUSTOMER_CODE AND  IM.ITEM_CODE = SI.ITEM_CODE 
                    and SI.Employee_code = es.Employee_code
                    AND si.deleted_flag = 'N' AND si.company_code in({companyCode})
                    and SI.Employee_code = es.Employee_code";


            //for customer Filter
            if (!string.IsNullOrEmpty(customerCode))
            {
                Query += " and (";
                foreach (var item in customerCode.Split(','))
                {
                    Query += $"cs.master_customer_code like  (Select MASTER_CUSTOMER_CODE || '%'  from {fiscalYearFilter}SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{item}' AND COMPANY_CODE in({companyCode}) ) OR ";
                }
                Query = Query.Substring(0, Query.Length - 3) + ") ";
            }

            //for item Filter
            if (!string.IsNullOrEmpty(itemCode))
            {
                Query += " and (";
                foreach (var item in itemCode.Split(','))
                {
                    Query += $"IM.MASTER_ITEM_CODE LIKE (Select DISTINCT MASTER_ITEM_CODE || '%'  from {fiscalYearFilter}IP_ITEM_MASTER_SETUP WHERE ITEM_CODE in('{item}') AND COMPANY_CODE in ({companyCode})) OR ";
                }
                Query = Query.Substring(0, Query.Length - 3) + ") ";
            }


            //for category Filter
            if (!string.IsNullOrEmpty(categoryCode))
            {
                Query += " and (";
                foreach (var item in categoryCode.Split(','))
                {
                    Query += "IM.CATEGORY_CODE = '" + item + "' OR ";
                }
                Query = Query.Substring(0, Query.Length - 3) + ") ";
            }

            //FOR COMPANY FILTER
            if (!string.IsNullOrEmpty(companyCode))
            {
                Query += $" AND SI.COMPANY_CODE IN ({companyCode})";
            }


            //FOR BRANCH FILTER
            if (!string.IsNullOrEmpty(branchCode))
            {
                Query += " AND SI.BRANCH_CODE IN (" + branchCode + ")";
            }

            //For AREA FILTER
            if (reportFilters.AreaTypeFilter.Count > 0)
            {
                Query += string.Format(@" AND SI.AREA_CODE IN ('{0}')", string.Join("','", reportFilters.AreaTypeFilter).ToString());
            }
            //FOR employee Filter
            if (reportFilters.EmployeeFilter.Count > 0)
            {
                Query += string.Format(@" AND SI.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", reportFilters.EmployeeFilter).ToString());
            }
            //FOR Agent Filter
            if (reportFilters.AgentFilter.Count > 0)
            {
                Query += string.Format(@" AND SI.AGENT_CODE IN  ('{0}')", string.Join("','", reportFilters.AgentFilter).ToString());
            }
            //FOR Location Filter
            if (reportFilters.LocationFilter.Count > 0)
            {
                string locationFilter = string.Empty;
                var locations = reportFilters.LocationFilter;
                for (int i = 0; i < locations.Count; i++)
                {

                    if (i == 0)
                        locationFilter += string.Format($"SELECT LOCATION_CODE FROM {fiscalYearFilter}IP_LOCATION_SETUP WHERE LOCATION_CODE LIKE '{locations[i]}%' ");
                    else
                    {
                        locationFilter += string.Format($" OR LOCATION_CODE like '{locations[i]}%' ");
                    }
                }
                locationFilter = string.Format(@" AND SI.FROM_LOCATION_CODE IN ({0} OR LOCATION_CODE IN ('{1}'))", locationFilter, string.Join("','", locations));
                Query += locationFilter;
            }

            Query += " GROUP BY si.EMPLOYEE_CODE,es.EMPLOYEE_EDESC";

            Query = string.Format(Query);
            var salesReportList = _objectEntity.SqlQuery<EmployeeWiseReport>(Query).ToList();
            return salesReportList;
        }

        public List<WeekWiseCollectionReport> GetWeekWiseCollectionReport(ReportFiltersModel reportFilters, User userInfo)
        {
            try
            {
                if (userInfo == null)
                {
                    userInfo = new Core.Domain.User();
                    userInfo.company_code = "01";
                    userInfo.branch_code = "01.01";

                }
                else if (string.IsNullOrEmpty(userInfo.company_code) || string.IsNullOrEmpty(userInfo.branch_code))
                {
                    userInfo.company_code = "01";
                    userInfo.branch_code = "01.01";
                }
                //string startDate = DateTime.Today.AddMonths(-1).ToString("MM/dd/yyyy");
                //string endDate = DateTime.Today.Date.ToString("MM/dd/yyyy");
                string startDate = "05/01/2015";
                string endDate = "07/30/2015";
                var Query = string.Format(@"SELECT 
                                             TO_CHAR (SI.SALES_DATE - 7 / 24, 'IW') AS week,
                                             SUM (nvl(SI.QUANTITY,0))/{0} AS Quantity,
                                             SUM (nvl(SI.CALC_TOTAL_PRICE,0))/{0} AS Amount
                                        FROM SA_SALES_INVOICE SI, SA_CUSTOMER_SETUP CS
                                       WHERE 
                                       --SI.SALES_DATE >= to_date('{1}','MM/dd/yyyy') AND SI.SALES_DATE < to_date('{2}','MM/dd/yyyy')
                                      SI.COMPANY_CODE = '{3}' AND
                                       SI.CUSTOMER_CODE = CS.CUSTOMER_CODE   
                                    GROUP BY TO_CHAR (SI.SALES_DATE - 7 / 24, 'IW')
                                    ORDER BY TO_CHAR (SI.SALES_DATE - 7 / 24, 'IW')
                        ", ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), startDate, endDate, userInfo.company_code, userInfo.branch_code);

                var report = _objectEntity.SqlQuery<WeekWiseCollectionReport>(Query).ToList();
                return report;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public List<WeekWiseCollectionReport> GetWeekWiseCollectionReport(ReportFiltersModel reportFilters, User userInfo, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode)
        {
            //companyCode = string.Join(",", reportFilters.CompanyFilter);
            //companyCode = companyCode == "" ? userInfo.company_code : companyCode;
            companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            string Query = string.Empty;

            Query = @"select TO_CHAR (SI.SALES_DATE - 7 / 24, 'IW') AS week,
                             nvl(sum(nvl(SI.calc_total_price,0))/{0},0) as amount,
                             sum(nvl(SI.calc_Quantity,0))/'{0}' as quantity                             
                         from sa_sales_invoice SI
                            where SI.deleted_flag='N'
                            AND SI.COMPANY_CODE IN (" + companyCode + ")";



            //****************************
            //CONDITIONS FITLER START HERE
            //****************************

            //for customer Filter
            var customerFilter = string.Empty;
            if (reportFilters.CustomerFilter.Count() > 0)
            {
                customerFilter = @"select DISTINCT(customer_code) from sa_customer_setup where (";
                //IF CUSTOMER_SKU_FLAG = G
                foreach (var item in reportFilters.CustomerFilter)
                {
                    customerFilter += "master_customer_code like  (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                customerFilter = customerFilter.Substring(0, customerFilter.Length - 3);
                //IF CUSTOMER_SKU_FLAG = I                
                customerFilter += " or (customer_code in (" + string.Join(",", reportFilters.CustomerFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                Query += " and SI.customer_code IN(" + customerFilter + ")";
            }




            ////for Product Filter
            var productFilter = string.Empty;
            if (reportFilters.ProductFilter.Count() > 0)
            {
                productFilter = @"select  DISTINCT item_code from IP_ITEM_MASTER_SETUP where (";
                //IF PRODUCT_SKU_FLAG = G
                foreach (var item in reportFilters.ProductFilter)
                {
                    productFilter += "MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                productFilter = productFilter.Substring(0, productFilter.Length - 3);
                //IF PRODUCT_SKU_FLAG = I                
                productFilter += " or (ITEM_CODE in (" + string.Join(",", reportFilters.ProductFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                Query += " and SI.ITEM_CODE IN(" + productFilter + ")";
            }


            if (reportFilters.BranchFilter.Count > 0)
            {
                Query += string.Format(@" AND SI.BRANCH_CODE IN ('{0}')", string.Join("','", reportFilters.BranchFilter).ToString());
            }
            if (reportFilters.AreaTypeFilter.Count > 0)
            {
                Query += string.Format(@" AND SI.AREA_CODE IN ('{0}')", string.Join("','", reportFilters.AreaTypeFilter).ToString());
            }

            //****************************
            //CONDITIONS FITLER END HERE
            //****************************


            Query += @" 
                            GROUP BY TO_CHAR (SI.SALES_DATE - 7 / 24, 'IW')
                            ORDER BY TO_CHAR (SI.SALES_DATE - 7 / 24, 'IW')";


            Query = string.Format(Query, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode);
            var datas = _objectEntity.SqlQuery<WeekWiseCollectionReport>(Query).ToList();
            return datas;


            //OLD QUERY BY PRAMOD SIR
            //if (userInfo == null)
            //    {
            //        userInfo = new Core.Domain.User();
            //        userInfo.company_code = "01";
            //        userInfo.branch_code = "01.01";

            //    }
            //    else if (string.IsNullOrEmpty(userInfo.company_code) || string.IsNullOrEmpty(userInfo.branch_code))
            //    {
            //        userInfo.company_code = "01";
            //        userInfo.branch_code = "01.01";
            //    }
            //    var Query = @"SELECT  TO_CHAR (SI.SALES_DATE - 7 / 24, 'IW') AS week,
            //                        SUM (nvl(SI.QUANTITY,0))/" + ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter) + @" AS Quantity,
            //                        SUM (nvl(SI.CALC_TOTAL_PRICE,0))/" + ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter) + @" AS Amount
            //                    FROM SA_SALES_INVOICE SI, SA_CUSTOMER_SETUP CS, IP_ITEM_MASTER_SETUP IM
            //                    WHERE    
            //                    SI.CUSTOMER_CODE = CS.CUSTOMER_CODE AND  IM.ITEM_CODE = SI.ITEM_CODE AND
            //                    SI.COMPANY_CODE = " + userInfo.company_code + @"";


            //    //for customer Filter
            //    if (!string.IsNullOrEmpty(customerCode))
            //    {
            //        Query += " and (";
            //        foreach (var item in customerCode.Split(','))
            //        {
            //            Query += "cs.master_customer_code like  (Select MASTER_CUSTOMER_CODE || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND COMPANY_CODE='" + userInfo.company_code + "' ) OR ";
            //        }
            //        Query = Query.Substring(0, Query.Length - 3) + ") ";
            //    }

            //    //for item Filter
            //    if (!string.IsNullOrEmpty(itemCode))
            //    {
            //        Query += " and (";
            //        foreach (var item in itemCode.Split(','))
            //        {
            //            Query += "IM.MASTER_ITEM_CODE LIKE (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND COMPANY_CODE='" + userInfo.company_code + "') OR ";
            //        }
            //        Query = Query.Substring(0, Query.Length - 3) + ") ";
            //    }


            //    //for category Filter
            //    if (!string.IsNullOrEmpty(categoryCode))
            //    {
            //        Query += " and (";
            //        foreach (var item in categoryCode.Split(','))
            //        {
            //            Query += "IM.CATEGORY_CODE = '" + item + "' OR ";
            //        }
            //        Query = Query.Substring(0, Query.Length - 3) + ") ";
            //    }

            //    //FOR COMPANY FILTER
            //    if (!string.IsNullOrEmpty(companyCode))
            //    {
            //        Query += " AND SI.COMPANY_CODE IN (" + companyCode + ")";
            //    }


            //    //FOR BRANCH FILTER
            //    if (reportFilters.BranchFilter.Count() > 0)
            //    {
            //        Query += " AND SI.BRANCH_CODE IN (" + string.Join(",", reportFilters.BranchFilter) + ")";
            //        try { userInfo.company_code = reportFilters.BranchFilter[0].Split('.')[0]; } catch { }

            //    }                


            //    //FOR partyType FILTER
            //    if (!string.IsNullOrEmpty(partyTypeCode))
            //    {
            //        Query += " AND SI.PARTY_TYPE_CODE IN (" + partyTypeCode + ")";
            //    }

            //    //FOR FORMCODE FILER
            //    if (!string.IsNullOrEmpty(formCode))
            //    {
            //        Query += " AND SI.FORM_CODE IN (" + formCode + ")";
            //    }



            //    Query += @" GROUP BY TO_CHAR (SI.SALES_DATE - 7 / 24, 'IW')
            //                ORDER BY TO_CHAR (SI.SALES_DATE - 7 / 24, 'IW')";

            //    var report = _objectEntity.SqlQuery<WeekWiseCollectionReport>(Query).ToList();
            //    return report;

        }
        public List<WeekWiseCollectionReport> GetWeekWiseCustomerCollectionReport(ReportFiltersModel reportFilters, string week, User userInfo)
        {
            try
            {
                if (userInfo == null)
                {
                    userInfo = new Core.Domain.User();
                    userInfo.company_code = "01";
                    userInfo.branch_code = "01.01";

                }
                else if (string.IsNullOrEmpty(userInfo.company_code) || string.IsNullOrEmpty(userInfo.branch_code))
                {
                    userInfo.company_code = "01";
                    userInfo.branch_code = "01.01";
                }
                if (reportFilters.BranchFilter.Count() > 0)
                {
                    try { userInfo.company_code = reportFilters.BranchFilter[0].Split('.')[0]; } catch { }

                }
                var Query = string.Format(@"select distinct p.customercode,pi.customer_edesc as CustomerName,p.quantity,p.amount
                                             from(SELECT  substr(cs.master_customer_code, 0, 2) as CustomerCode,
                                                TO_CHAR(SI.SALES_DATE - 7 / 24, 'IW') as Week,
                                               SUM (nvl(SI.QUANTITY,0))/{3} AS Quantity,
                                             SUM (nvl(SI.CALC_TOTAL_PRICE,0))/{3} AS Amount
                                            FROM SA_SALES_INVOICE SI, SA_CUSTOMER_SETUP CS
                                            WHERE
                                                TO_CHAR(SI.SALES_DATE - 7 / 24, 'IW') = '{0}' AND                                               
                                                SI.CUSTOMER_CODE = CS.CUSTOMER_CODE
                                                AND SI.COMPANY_CODE = '{1}'
                                                GROUP BY
                                                substr(cs.master_customer_code, 0, 2),
                                                TO_CHAR(SI.SALES_DATE - 7 / 24, 'IW')
                                            ORDER BY
                                                substr(cs.master_customer_code, 0, 2)) p,
                                                sa_customer_setup pi where p.customercode = pi.master_customer_code"
                                        , week, userInfo.company_code, userInfo.branch_code, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter));

                var report = _objectEntity.SqlQuery<WeekWiseCollectionReport>(Query).ToList();
                return report;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ExpensesTrendReport> GetExpensesAccount(User userinfo)
        {
            try
            {
                if (userinfo == null)
                {
                    userinfo = new Core.Domain.User();
                    userinfo.company_code = "01";
                    userinfo.branch_code = "01.01";

                }
                else if (string.IsNullOrEmpty(userinfo.company_code) || string.IsNullOrEmpty(userinfo.branch_code))
                {
                    userinfo.company_code = "01";
                    userinfo.branch_code = "01.01";
                }
                var Query = string.Format(@"SELECT SUBSTR (CAS.MASTER_ACC_CODE, 0, 2) Master_Acc_Code, 
                                            (select acc_edesc from fa_chart_of_accounts_setup 
                                                where company_code='{0}' and master_acc_code= SUBSTR (CAS.MASTER_ACC_CODE, 0, 2)) acc_group                                                    
                                                FROM fa_general_ledger gl, fa_chart_of_accounts_setup cas
                                               WHERE gl.transaction_type = 'DR' AND gl.acc_code = cas.acc_code
                                            and gl.company_code='{0}'
                                            GROUP BY SUBSTR (CAS.MASTER_ACC_CODE, 0, 2)
                                            order by SUBSTR (CAS.MASTER_ACC_CODE, 0, 2)",
                                         userinfo.company_code, userinfo.branch_code
                                         );

                var report = _objectEntity.SqlQuery<ExpensesTrendReport>(Query).ToList();
                return report;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<ExpensesTrendReport> GetExpensesTrendMonthlyReport(ReportFiltersModel reportFilters, User userInfo)
        {
            try
            {
                if (userInfo == null)
                {
                    userInfo = new Core.Domain.User();
                    userInfo.company_code = "01";
                    userInfo.branch_code = "01.01";

                }
                else if (string.IsNullOrEmpty(userInfo.company_code) || string.IsNullOrEmpty(userInfo.branch_code))
                {
                    userInfo.company_code = "01";
                    userInfo.branch_code = "01.01";
                }
                var Query = string.Format(@"SELECT SUBSTR (CAS.MASTER_ACC_CODE, 0, 2) Master_Acc_Code, 
                                            (select acc_edesc from fa_chart_of_accounts_setup 
                                                where company_code='{1}' and branch_code like '{1}%' and master_acc_code= SUBSTR (CAS.MASTER_ACC_CODE, 0, 2)) acc_group , 
                                                     SUM (nvl(GL.DR_AMOUNT,0))/{0} Amount
                                                FROM fa_general_ledger gl, fa_chart_of_accounts_setup cas
                                               WHERE gl.transaction_type = 'DR' AND gl.acc_code = cas.acc_code
                                            and gl.company_code='{1}'
                                            GROUP BY SUBSTR (CAS.MASTER_ACC_CODE, 0, 2)
                                            order by SUBSTR (CAS.MASTER_ACC_CODE, 0, 2)",
                                         ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter),
                                         userInfo.company_code, userInfo.branch_code
                                         );

                var report = _objectEntity.SqlQuery<ExpensesTrendReport>(Query).ToList();
                return report;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public List<ExpensesTrendReport> GetExpensesTrendMonthlyReport(ReportFiltersModel reportFilters, User userInfo, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode, string accountCode)
        {
            try
            {
                //companyCode = string.Join(",", reportFilters.CompanyFilter);
                //companyCode = companyCode == "" ? userInfo.company_code : companyCode;
                //var companyCode = string.Join(",'", reportFilters.CompanyFilter);
                companyCode = string.Empty;
                foreach (var company in reportFilters.CompanyFilter)
                {
                    companyCode += $@"'{company}',";
                }

                companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);

                var fiscalYearFilter = string.Empty;
                if (reportFilters.FiscalYearFilter.Count > 0)
                {
                    fiscalYearFilter = $"{reportFilters.FiscalYearFilter.FirstOrDefault().DBName}.";
                }
                var Query = $@"SELECT SUBSTR (cas.MASTER_ACC_CODE, 0, 2) Master_Acc_Code, 
                                            (select acc_edesc from {fiscalYearFilter}fa_chart_of_accounts_setup 
                                                where company_code  ='{userInfo.company_code}' and master_acc_code= SUBSTR (CAS.MASTER_ACC_CODE, 0, 2)) acc_group , 
                                                     SUM (nvl(gl.DR_AMOUNT,0)) Amount
                                                FROM {fiscalYearFilter}fa_general_ledger gl, {fiscalYearFilter}fa_chart_of_accounts_setup cas
                                               WHERE gl.transaction_type = 'DR' 
                                                 AND gl.acc_code = cas.acc_code                                             
                                                 AND gl.COMPANY_CODE = cas.COMPANY_CODE
                                                 AND gl.DELETED_FLAG = 'N' AND cas.DELETED_FLAG = 'N'
                                                 AND gl.company_code IN ({companyCode})";





                //FOR BRANCH FILTER
                if (reportFilters.BranchFilter.Count() > 0)
                {
                    Query += " AND gl.BRANCH_CODE IN (" + string.Join(",", reportFilters.BranchFilter) + ")";

                }




                //FOR account FILER
                if (!string.IsNullOrEmpty(accountCode))
                {
                    Query += " AND SUBSTR (CAS.MASTER_ACC_CODE, 0, 2) IN (" + accountCode + ")";
                }



                Query += @"  GROUP BY SUBSTR (CAS.MASTER_ACC_CODE, 0, 2)
                            order by SUBSTR (CAS.MASTER_ACC_CODE, 0, 2)";


                Query = string.Format(Query);

                var report = _objectEntity.SqlQuery<ExpensesTrendReport>(Query).ToList();
                return report;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public List<ExpensesTrendReport> GetExpensesTrendMonthlyReport(ReportFiltersModel reportFilters, string accountCode, User userInfo)
        {
            try
            {
                if (userInfo == null)
                {
                    userInfo = new Core.Domain.User();
                    userInfo.company_code = "01";
                    userInfo.branch_code = "01.01";

                }
                else if (string.IsNullOrEmpty(userInfo.company_code) || string.IsNullOrEmpty(userInfo.branch_code))
                {
                    userInfo.company_code = "01";
                    userInfo.branch_code = "01.01";
                }
                var Query = string.Format(@"SELECT SUBSTR (CAS.MASTER_ACC_CODE, 0, 2) Master_Acc_Code, 
                                            (select acc_edesc from fa_chart_of_accounts_setup where  and company_code='{2}' and gl.branch_code like '{2}%' master_acc_code=  SUBSTR (CAS.MASTER_ACC_CODE, 0, 2)) acc_group , 
                                                     SUM (nvl(GL.DR_AMOUNT,0))/{0} Amount
                                                FROM fa_general_ledger gl, fa_chart_of_accounts_setup cas
                                               WHERE gl.transaction_type = 'DR' AND gl.acc_code = cas.acc_code
                                               AND SUBSTR (CAS.MASTER_ACC_CODE, 0, 2) IN ({1})
                                              and gl.company_code='{2}'
                                            GROUP BY SUBSTR (CAS.MASTER_ACC_CODE, 0, 2)
                                            order by SUBSTR (CAS.MASTER_ACC_CODE, 0, 2)",
                                                    ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), accountCode, userInfo.company_code, userInfo.branch_code);

                var report = _objectEntity.SqlQuery<ExpensesTrendReport>(Query).ToList();
                return report;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ExpensesTrendReport> GetExpensesTrendMonthlyReport(User userInfo)
        {
            try
            {
                if (userInfo == null)
                {
                    userInfo = new Core.Domain.User();
                    userInfo.company_code = "01";
                    userInfo.branch_code = "01.01";

                }
                else if (string.IsNullOrEmpty(userInfo.company_code) || string.IsNullOrEmpty(userInfo.branch_code))
                {
                    userInfo.company_code = "01";
                    userInfo.branch_code = "01.01";
                }
                var Query = string.Format(@"SELECT SUBSTR (CAS.MASTER_ACC_CODE, 0, 2) Master_Acc_Code, 
                                            (select acc_edesc from fa_chart_of_accounts_setup where master_acc_code=  SUBSTR (CAS.MASTER_ACC_CODE, 0, 2)
                                               and company_code = '{0}' and branch_code like '{0}%') acc_group 
                                                FROM  fa_general_ledger gl, fa_chart_of_accounts_setup cas
                                               WHERE gl.transaction_type = 'DR' AND gl.acc_code = cas.acc_code  
                                               and gl.company_code='{0}'                                
                                            GROUP BY SUBSTR (CAS.MASTER_ACC_CODE, 0, 2)
                                            order by SUBSTR (CAS.MASTER_ACC_CODE, 0, 2)", userInfo.company_code, userInfo.branch_code);

                var report = _objectEntity.SqlQuery<ExpensesTrendReport>(Query).ToList();
                return report;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //
        public List<ExpensesTrendReport> GetExpensesTrendAccountWiseReport(ReportFiltersModel reportFilters, string master_acc_code, User userInfo, string DateFormat)
        {

            var companyCode = string.Join(",", reportFilters.CompanyFilter);
            companyCode = companyCode == "" ? userInfo.company_code : companyCode;
            var fiscalYearFilter = string.Empty;
            if (reportFilters.FiscalYearFilter.Count > 0)
            {
                fiscalYearFilter = $"{reportFilters.FiscalYearFilter.FirstOrDefault().DBName}.";
            }
            string Query = string.Empty;
            if (string.Equals(DateFormat, "AD"))
            {
                Query = string.Format($@"SELECT 
                                     TO_CHAR(gl.voucher_date, 'MON-YYY') MonthYear,
                                     TO_CHAR(gl.voucher_date, 'MON') Month,
                                     TO_CHAR(gl.voucher_date, 'YYYYMM') MonthInt,
                                                    SUBSTR (cas.MASTER_ACC_CODE, 0, 2) Master_Acc_Code, 
                                                                        (select distinct acc_edesc from {fiscalYearFilter}fa_chart_of_accounts_setup 
                                                                            where company_code in({userInfo.company_code}) and master_acc_code= SUBSTR (CAS.MASTER_ACC_CODE, 0, 2)) acc_group , 
                                                                                 SUM (nvl(gl.DR_AMOUNT,0)) Amount
                                                                            FROM fa_general_ledger gl, fa_chart_of_accounts_setup cas
                                                                           WHERE gl.transaction_type = 'DR' 
                                                                             AND gl.acc_code = cas.acc_code                                             
                                                                             AND gl.COMPANY_CODE = cas.COMPANY_CODE
                                                                             AND gl.DELETED_FLAG = 'N' AND cas.DELETED_FLAG = 'N'
                                                                             AND gl.company_code in({companyCode})  
                                                                             and SUBSTR(cas.MASTER_ACC_CODE, 0, 2)='{master_acc_code}' "
                                                   );
            }
            else
            {
                Query = string.Format($@"SELECT 
                                        FN_BS_MONTH(SUBSTR(BS_DATE(gl.voucher_date),6,2)) MonthYear,
                                         FN_BS_MONTH(SUBSTR(BS_DATE(gl.voucher_date),6,2)) Month,
                                         SUBSTR(BS_DATE(gl.voucher_date),6,2) MonthInt,
                                                        SUBSTR (cas.MASTER_ACC_CODE, 0, 2) Master_Acc_Code, 
                                                                            (select distinct acc_edesc from fa_chart_of_accounts_setup 
                                                                                where company_code in({userInfo.company_code}) and master_acc_code= SUBSTR (CAS.MASTER_ACC_CODE, 0, 2)) acc_group , 
                                                                                     SUM (nvl(gl.DR_AMOUNT,0)) Amount
                                                                                FROM {fiscalYearFilter}fa_general_ledger gl, {fiscalYearFilter}fa_chart_of_accounts_setup cas
                                                                               WHERE gl.transaction_type = 'DR' 
                                                                                 AND gl.acc_code = cas.acc_code                                             
                                                                                 AND gl.COMPANY_CODE = cas.COMPANY_CODE
                                                                                 AND gl.DELETED_FLAG = 'N' AND cas.DELETED_FLAG = 'N'
                                                                                 AND gl.company_code in({companyCode}) 
                                                                                 and SUBSTR(cas.MASTER_ACC_CODE, 0, 2)='{master_acc_code}' ");
            }

            //FOR BRANCH FILTER
            if (reportFilters.BranchFilter.Count() > 0)
            {
                Query += " AND gl.BRANCH_CODE IN (" + string.Join(",", reportFilters.BranchFilter) + ")";

            }



            if (string.Equals(DateFormat, "AD"))
            {
                Query += @"
                        GROUP BY SUBSTR (CAS.MASTER_ACC_CODE, 0, 2),TO_CHAR(gl.voucher_date, 'MON-YYY'),TO_CHAR(gl.voucher_date, 'MON'),TO_CHAR(gl.voucher_date, 'YYYYMM')
                        order by TO_CHAR(gl.voucher_date, 'YYYYMM')";
            }
            else
            {
                Query += @"
                          GROUP BY SUBSTR (CAS.MASTER_ACC_CODE, 0, 2),SUBSTR(BS_DATE(gl.voucher_date),6,2) 
                            order by SUBSTR(BS_DATE(gl.voucher_date),6,2)";
            }
            var report = _objectEntity.SqlQuery<ExpensesTrendReport>(Query).ToList();
            return report;
            //            try
            //            {
            //                if (userInfo == null)
            //                {
            //                    userInfo = new Core.Domain.User();
            //                    userInfo.company_code = "01";
            //                    userInfo.branch_code = "01.01";

            //                }
            //                else if (string.IsNullOrEmpty(userInfo.company_code) || string.IsNullOrEmpty(userInfo.branch_code))
            //                {
            //                    userInfo.company_code = "01";
            //                    userInfo.branch_code = "01.01";
            //                }

            //                //FOR BRANCH FILTER
            //                string branchCondition = string.Empty;
            //                if (reportFilters.BranchFilter.Count() > 0)
            //                {
            //                    branchCondition = " AND gl.BRANCH_CODE IN (" + string.Join(",", reportFilters.BranchFilter) + ")";
            //                    try { userInfo.company_code = reportFilters.BranchFilter[0].Split('.')[0]; } catch { }
            //                }
            //                string Query = string.Empty;
            //                if (string.Equals(DateFormat, "AD") || string.IsNullOrEmpty(DateFormat))
            //                {
            //                    //var Query = string.Format(@"SELECT SUBSTR (CAS.MASTER_ACC_CODE, 0, 2) Master_Acc_Code, 
            //                    //                            (select acc_edesc from fa_chart_of_accounts_setup where  master_acc_code=  SUBSTR (CAS.MASTER_ACC_CODE, 0, 2) 
            //                    //                             and company_code = '{1}' and branch_code like '{2}%') acc_group,
            //                    //                                     to_char( GL.VOUCHER_DATE, 'MON' ) month,
            //                    //                                    SUM (nvl(GL.DR_AMOUNT,0))/{3} Amount
            //                    //                                FROM fa_general_ledger gl, fa_chart_of_accounts_setup cas
            //                    //                               WHERE gl.transaction_type = 'DR' AND gl.acc_code = cas.acc_code
            //                    //                              --and SUBSTR (CAS.MASTER_ACC_CODE, 0, 2) = (select master_acc_code from fa_chart_of_accounts_setup where acc_edesc= '{0}')
            //                    //                                and SUBSTR (CAS.MASTER_ACC_CODE, 0, 2) = '{0}'
            //                    //                            and gl.company_code = '{1}'
            //                    //                            GROUP BY SUBSTR (CAS.MASTER_ACC_CODE, 0, 2),
            //                    //                                    to_char( GL.VOUCHER_DATE, 'MON' )
            //                    //                            order by to_char( GL.VOUCHER_DATE, 'MON' )", master_acc_code, userInfo.company_code, userInfo.branch_code, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter));
            //                    Query = string.Format(@"
            //select (Month||'-'||y) MonthYear,Month,Master_Acc_Code,Acc_Group,Amount from(
            //SELECT 
            //    TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'YYY') Y,
            //    TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MM') M,
            //                           TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MON') MONTH,    
            //                           SUBSTR(CAS.MASTER_ACC_CODE, 0, 2) Master_Acc_Code,
            //                           (select f.acc_edesc from fa_chart_of_accounts_setup f where f.master_acc_code= SUBSTR (CAS.MASTER_ACC_CODE, 0, 2) 
            //                               and company_code = '01' and branch_code like '01%') acc_group,
            //                           (select nvl(SUM (nvl(GL.DR_AMOUNT,0))/{2},0) 
            //                               FROM fa_general_ledger gl,fa_chart_of_accounts_setup fcas
            //                               WHERE gl.transaction_type = 'DR' 
            //                               AND gl.acc_code = fcas.acc_code
            //                               and gl.company_code = '{1}'
            //{3}
            //                               and SUBSTR(fcas.MASTER_ACC_CODE, 0, 2)='{0}'
            //                               and TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MON')=to_char( GL.VOUCHER_DATE, 'MON' )
            //                               and TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'YYY')=to_char( GL.VOUCHER_DATE, 'YYY' )
            //                               ) as Amount                
            //                       FROM all_objects,fa_chart_of_accounts_setup CAS,
            //                          (SELECT start_date, end_date
            //                             FROM HR_FISCAL_YEAR_CODE
            //                            WHERE SYSDATE BETWEEN start_date AND end_date) fiscal_year
            //                       WHERE ROWNUM <= fiscal_year.end_date - fiscal_year.start_date + 1
            //                       and  SUBSTR(CAS.MASTER_ACC_CODE, 0, 2)='{0}'
            //                       group by TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MON'),
            //TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'YYY'),
            //TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MM'),
            //                       SUBSTR(CAS.MASTER_ACC_CODE, 0, 2) ) t1 
            //order by to_number(Y||M)",
            //                           master_acc_code,
            //                           userInfo.company_code,
            //                           ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), branchCondition);
            //                }
            //                else if (string.Equals(DateFormat, "BS"))
            //                {
            //                    Query = string.Format(@"
            //                                SELECT 
            //                                substr(bs_date(fiscal_year.start_date + ROWNUM - 1),0,4) y,
            //                                substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2) m,
            //                                fn_bs_month(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2)) AS MONTH,
            //                                fn_bs_month(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2)) AS MonthYear,
            //                               SUBSTR(CAS.MASTER_ACC_CODE, 0, 2) Master_Acc_Code,
            //                               (select f.acc_edesc from fa_chart_of_accounts_setup f where f.master_acc_code= SUBSTR (CAS.MASTER_ACC_CODE, 0, 2) 
            //                                   and company_code = '{1}' and branch_code like '01%') acc_group,
            //                               (select nvl(SUM (nvl(GL.DR_AMOUNT,0))/{0},0) 
            //                                   FROM fa_general_ledger gl,fa_chart_of_accounts_setup fcas
            //                                   WHERE gl.transaction_type = 'DR' 
            //                                   AND gl.acc_code = fcas.acc_code
            //                                   and gl.company_code = '{1}'
            //{3}
            //                                   and SUBSTR(fcas.MASTER_ACC_CODE, 0, 2)='{2}'
            //                                   and fn_bs_month(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2))=fn_bs_month(substr(bs_date(GL.VOUCHER_DATE),6,2))
            //                                   ) as Amount                
            //                            FROM all_objects,fa_chart_of_accounts_setup CAS,
            //                              (SELECT start_date, end_date
            //                                 FROM HR_FISCAL_YEAR_CODE
            //                                WHERE SYSDATE BETWEEN start_date AND end_date) fiscal_year
            //                            WHERE ROWNUM <= fiscal_year.end_date - fiscal_year.start_date + 1
            //                            and  SUBSTR(CAS.MASTER_ACC_CODE, 0, 2)='{2}'
            //                            group by 
            //                                substr(bs_date(fiscal_year.start_date + ROWNUM - 1),0,4),
            //                                substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2),
            //                                fn_bs_month(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2)),
            //                                SUBSTR(CAS.MASTER_ACC_CODE, 0, 2) 
            //                            order by to_number(y||m)"
            //                                , ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter),
            //                                userInfo.company_code, master_acc_code, branchCondition);
            //                }
            //                var report = _objectEntity.SqlQuery<ExpensesTrendReport>(Query).ToList();
            //                return report;
            //            }
            //            catch (Exception)
            //            {
            //                throw;
            //            }
        }

        public List<ExpensesTrendReport> GetExpensesTrendAccountWiseDailyReport(ReportFiltersModel reportFilters, User userInfo, string master_acc_code, string month, string DateFormat)
        {

            var companyCode = string.Join(",", reportFilters.CompanyFilter);
            companyCode = companyCode == "" ? userInfo.company_code : companyCode;

            string Query = string.Empty;
            if (string.Equals(DateFormat, "AD"))
            {
                Query = string.Format(@"SELECT 
                                    TO_CHAR(gl.voucher_date, 'MON') Month,
                                    TO_CHAR(gl.voucher_date, 'DD') day,
                                    SUBSTR (cas.MASTER_ACC_CODE, 0, 2) Master_Acc_Code, 
                                                        (select distinct acc_edesc from fa_chart_of_accounts_setup 
                                                            where company_code in({1}) and master_acc_code= SUBSTR (CAS.MASTER_ACC_CODE, 0, 2)) acc_group , 
                                                                    SUM (nvl(gl.DR_AMOUNT,0))/{0} Amount
                                                            FROM fa_general_ledger gl, fa_chart_of_accounts_setup cas
                                                            WHERE gl.transaction_type = 'DR' 
                                                                AND gl.acc_code = cas.acc_code                                             
                                                                AND gl.COMPANY_CODE = cas.COMPANY_CODE
                                                                AND gl.DELETED_FLAG = 'N' AND cas.DELETED_FLAG = 'N'
                                                                AND gl.company_code in({1})  
                                                                and SUBSTR(cas.MASTER_ACC_CODE, 0, 2)='{2}' 
                                                                and TO_CHAR(gl.voucher_date, 'YYYYMM')  = '{3}'"
                                    , ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode, master_acc_code, month);
            }
            else
            {
                Query = string.Format(@"SELECT 
                                         FN_BS_MONTH(SUBSTR(BS_DATE(gl.voucher_date),6,2)) Month,
                                         SUBSTR(BS_DATE(gl.voucher_date),9,2) day,
                                            SUBSTR (cas.MASTER_ACC_CODE, 0, 2) Master_Acc_Code, 
                                                                (select distinct acc_edesc from fa_chart_of_accounts_setup 
                                                                    where company_code in({1}) and master_acc_code= SUBSTR (CAS.MASTER_ACC_CODE, 0, 2)) acc_group , 
                                                                            SUM (nvl(gl.DR_AMOUNT,0))/{0} Amount
                                                                    FROM fa_general_ledger gl, fa_chart_of_accounts_setup cas
                                                                    WHERE gl.transaction_type = 'DR' 
                                                                        AND gl.acc_code = cas.acc_code                                             
                                                                        AND gl.COMPANY_CODE = cas.COMPANY_CODE
                                                                        AND gl.DELETED_FLAG = 'N' AND cas.DELETED_FLAG = 'N'
                                                                        AND gl.company_code in({1}) 
                                                                        and SUBSTR(cas.MASTER_ACC_CODE, 0, 2)='{2}'
                                                                        and  SUBSTR(BS_DATE(gl.voucher_date),6,2) = '{3}'"
                            , ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode, master_acc_code, month);
            }

            //FOR BRANCH FILTER
            if (reportFilters.BranchFilter.Count() > 0)
            {
                Query += " AND gl.BRANCH_CODE IN (" + string.Join(",", reportFilters.BranchFilter) + ")";

            }



            if (string.Equals(DateFormat, "AD"))
            {
                Query += @"
                        GROUP BY SUBSTR (CAS.MASTER_ACC_CODE, 0, 2),TO_CHAR(gl.voucher_date, 'MON'),TO_CHAR(gl.voucher_date, 'DD'),TO_CHAR(gl.voucher_date, 'YYYYMM')
                        order by TO_CHAR(gl.voucher_date, 'DD')";
            }
            else
            {
                Query += @"
                          GROUP BY SUBSTR (CAS.MASTER_ACC_CODE, 0, 2),SUBSTR(BS_DATE(gl.voucher_date),6,2),SUBSTR(BS_DATE(gl.voucher_date),9,2) 
                            order by SUBSTR(BS_DATE(gl.voucher_date),6,2)";
            }
            var ExpensesTrendReport = _objectEntity.SqlQuery<ExpensesTrendReport>(Query).ToList();
            return ExpensesTrendReport;



            //string Query = string.Format(@"SELECT SUBSTR (CAS.MASTER_ACC_CODE, 0, 2) Master_Acc_Code, 
            //                                (select acc_edesc from fa_chart_of_accounts_setup where master_acc_code=  SUBSTR (CAS.MASTER_ACC_CODE, 0, 2)) acc_group,
            //                                to_char( GL.VOUCHER_DATE, 'MON' ) month,
            //                                to_char(GL.Voucher_Date,'DD') Day,
            //                                 SUM (nvl(GL.DR_AMOUNT,0))/{2} Amount
            //                                FROM fa_general_ledger gl, fa_chart_of_accounts_setup cas
            //                               WHERE gl.transaction_type = 'DR' AND gl.acc_code = cas.acc_code
            //                                and SUBSTR (CAS.MASTER_ACC_CODE, 0, 2) = '{0}'
            //                                and to_char( GL.VOUCHER_DATE, 'MON' )='{1}'
            //                            GROUP BY SUBSTR (CAS.MASTER_ACC_CODE, 0, 2),
            //                                    to_char( GL.VOUCHER_DATE, 'MON' ),
            //                                    TO_CHAR(GL.VOUCHER_DATE, 'DD')
            //                            order by to_char( GL.VOUCHER_DATE, 'MON' )", master_acc_code, month, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter));
            //string Query = string.Empty;
            //string branchCondition = string.Empty;
            //if (reportFilters.BranchFilter.Count() > 0)
            //{
            //    branchCondition = " AND gl.BRANCH_CODE IN (" + string.Join(",", reportFilters.BranchFilter) + ")";
            //    try { userInfo.company_code = reportFilters.BranchFilter[0].Split('.')[0]; } catch { }
            //}
            //string Query = string.Empty;
            ////FOR BRANCH FILTER
            //string branchCondition = string.Empty;
            //if (reportFilters.BranchFilter.Count() > 0)
            //{
            //    branchCondition = " AND gl.BRANCH_CODE IN (" + string.Join(",", reportFilters.BranchFilter) + ")";               
            //}

            //if (string.Equals(DateFormat, "AD"))
            //{
            //    Query = string.Format(@"select * from (SELECT 
            //    TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MON') MONTH,  
            //    to_char(fiscal_year.start_date + ROWNUM - 1,'DD') Day,  
            //    SUBSTR(CAS.MASTER_ACC_CODE, 0, 2) Master_Acc_Code,
            //    (select f.acc_edesc from fa_chart_of_accounts_setup f where f.master_acc_code= SUBSTR (CAS.MASTER_ACC_CODE, 0, 2) 
            //        and company_code = '01' and branch_code like '01%') acc_group,
            //    (select nvl(SUM (nvl(GL.DR_AMOUNT,0))/{2},0) 
            //        FROM fa_general_ledger gl,fa_chart_of_accounts_setup fcas
            //        WHERE gl.transaction_type = 'DR' 
            //        AND gl.acc_code = fcas.acc_code
            //        and gl.company_code = '01'
            //        {3}
            //        and SUBSTR(fcas.MASTER_ACC_CODE, 0, 2)='{0}'
            //        --and to_char( gl.VOUCHER_DATE, 'MON' )='AUG'
            //        and TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'DD')=to_char( GL.VOUCHER_DATE, 'DD' )
            //        ) as Amount                
            //FROM all_objects,fa_chart_of_accounts_setup CAS,
            //   (SELECT start_date, end_date
            //      FROM HR_FISCAL_YEAR_CODE
            //     WHERE SYSDATE BETWEEN start_date AND end_date) fiscal_year
            //WHERE ROWNUM <= fiscal_year.end_date - fiscal_year.start_date + 1
            //and  SUBSTR(CAS.MASTER_ACC_CODE, 0, 2)='{0}'
            //--and TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MON')='AUG'
            //group by 
            //TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MON'),
            //to_char(fiscal_year.start_date + ROWNUM - 1,'DD'),
            //SUBSTR(CAS.MASTER_ACC_CODE, 0, 2) 
            //) t1 where t1.Month='{1}'"
            //, master_acc_code, month, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), branchCondition);
            //}
            //else if (string.Equals(DateFormat, "BS"))
            //{
            //    Query = string.Format(@"select * from (SELECT 
            //        fn_bs_month(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2)) AS MONTH,
            //        --to_char(fiscal_year.start_date + ROWNUM - 1,'DD') Day,
            //        substr(bs_date(fiscal_year.start_date + ROWNUM - 1),9,2) Day,  
            //        SUBSTR(CAS.MASTER_ACC_CODE, 0, 2) Master_Acc_Code,
            //        (select f.acc_edesc from fa_chart_of_accounts_setup f where f.master_acc_code= SUBSTR (CAS.MASTER_ACC_CODE, 0, 2) 
            //            and company_code = '{3}' and branch_code like '01%') acc_group,
            //        (select nvl(SUM (nvl(GL.DR_AMOUNT,0))/{2},0) 
            //            FROM fa_general_ledger gl,fa_chart_of_accounts_setup fcas
            //            WHERE gl.transaction_type = 'DR' 
            //            AND gl.acc_code = fcas.acc_code
            //            and gl.company_code = '{3}'
            //            {4}
            //            and SUBSTR(fcas.MASTER_ACC_CODE, 0, 2)='{0}'
            //            and TO_CHAR(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),9,2))=to_char(substr(bs_date( gl.VOUCHER_DATE + ROWNUM - 1),9,2))
            //            ) as Amount                
            //    FROM all_objects,fa_chart_of_accounts_setup CAS,
            //       (SELECT start_date, end_date
            //          FROM HR_FISCAL_YEAR_CODE
            //         WHERE SYSDATE BETWEEN start_date AND end_date) fiscal_year
            //    WHERE ROWNUM <= fiscal_year.end_date - fiscal_year.start_date + 1
            //    and  SUBSTR(CAS.MASTER_ACC_CODE, 0, 2)='{0}'
            //    group by 
            //    substr(bs_date(fiscal_year.start_date + ROWNUM - 1),0,4),
            //    substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2),
            //    fn_bs_month(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2)),
            //    substr(bs_date(fiscal_year.start_date + ROWNUM - 1),9,2),
            //    TO_CHAR(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),9,2)),
            //    SUBSTR(CAS.MASTER_ACC_CODE, 0, 2) 
            //    Order by to_number(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),0,4) || substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2))
            //    ) t1 where t1.Month='{1}'  "
            //    , master_acc_code, month, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), userInfo.company_code, branchCondition);
            //}

            //var ExpensesTrendReport = _objectEntity.SqlQuery<ExpensesTrendReport>(Query).ToList();
            //return ExpensesTrendReport;
        }


        public List<MonthlySalesFiscalYearGraph> GetSalesMonthFiscal(ReportFiltersModel reportFilters, User userInfo, string dateFormat)
        {

            var fiscalYear = System.Configuration.ConfigurationManager.AppSettings["FiscalYear"].Substring(2);
            var companyCode = string.Join(",", reportFilters.CompanyFilter);
            companyCode = companyCode == "" ? userInfo.company_code : companyCode;

            string Query = string.Empty;
            if (string.Equals(dateFormat.ToLower(), "ad"))
            {
                Query = @"select to_char(SI.sales_date, 'Month') as month,to_char(SI.sales_date, 'YYYYMM') as MonthInt, nvl(sum(nvl(SI.calc_total_price,0))/{0},0)as amount,sum(nvl(SI.calc_Quantity,0))/'{0}' as quantity,to_char(SI.sales_date, 'Mon-YYY') as DisplayMonth 
                          ,CS.ABBR_CODE ||' " + fiscalYear + @"' AS FiscalYear ,'dbName' as DBName
                           from sa_sales_invoice SI,COMPANY_SETUP CS
                            where SI.COMPANY_CODE = CS.COMPANY_CODE
                            AND SI.deleted_flag='N' AND CS.DELETED_FLAG  = 'N'    
                            AND SI.COMPANY_CODE IN (" + companyCode + ")";
            }
            else
            {
                Query = @"select trim(substr(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SI.sales_date),6,2)),5,20))  DisplayMonth, nvl(sum(nvl(SI.calc_total_price,0))/{0},0)as amount,sum(nvl(SI.calc_Quantity,0))/'{0}' as quantity,substr(bs_date(SI.sales_date),1,7) as MonthInt 
                         ,CS.ABBR_CODE ||' " + fiscalYear + @"' AS FiscalYear,'dbName' as DBName from sa_sales_invoice SI,COMPANY_SETUP CS    
                         where SI.COMPANY_CODE = CS.COMPANY_CODE
                         AND  SI.deleted_flag='N' AND CS.DELETED_FLAG  = 'N'    
                         AND SI.COMPANY_CODE IN (" + companyCode + ")";
            }


            //****************************
            //CONDITIONS FITLER START HERE
            //****************************
            //for customer Filter
            var customerFilter = string.Empty;
            if (reportFilters.CustomerFilter.Count() > 0)
            {
                customerFilter = @"select  DISTINCT(customer_code) from sa_customer_setup where (";
                //IF CUSTOMER_SKU_FLAG = G
                foreach (var item in reportFilters.CustomerFilter)
                {
                    customerFilter += "master_customer_code like  (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                customerFilter = customerFilter.Substring(0, customerFilter.Length - 3);
                //IF CUSTOMER_SKU_FLAG = I                
                customerFilter += " or (customer_code in (" + string.Join(",", reportFilters.CustomerFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                Query += " and SI.customer_code IN(" + customerFilter + ")";
            }

            //for Product Filter
            var productFilter = string.Empty;
            if (reportFilters.ProductFilter.Count() > 0)
            {
                productFilter = @"select  DISTINCT item_code from IP_ITEM_MASTER_SETUP where (";
                //IF PRODUCT_SKU_FLAG = G
                foreach (var item in reportFilters.ProductFilter)
                {
                    productFilter += "MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                productFilter = productFilter.Substring(0, productFilter.Length - 3);
                //IF PRODUCT_SKU_FLAG = I                
                productFilter += " or (ITEM_CODE in (" + string.Join(",", reportFilters.ProductFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                Query += " and SI.ITEM_CODE IN(" + productFilter + ")";
            }


            if (reportFilters.BranchFilter.Count > 0)
            {
                Query += string.Format(@" AND SI.BRANCH_CODE IN ('{0}')", string.Join("','", reportFilters.BranchFilter).ToString());
            }
            if (reportFilters.EmployeeFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND  SI.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", reportFilters.EmployeeFilter).ToString());
            }
            if (reportFilters.AgentFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND  SI.AGENT_CODE IN  ('{0}')", string.Join("','", reportFilters.AgentFilter).ToString());
            }
            if (reportFilters.DivisionFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND  SI.DIVISION_CODE IN  ('{0}')", string.Join("','", reportFilters.DivisionFilter).ToString());
            }
            string locationFilter = string.Empty;
            if (reportFilters.LocationFilter.Count > 0)
            {

                var locations = reportFilters.LocationFilter;
                for (int i = 0; i < locations.Count; i++)
                {

                    if (i == 0)
                        locationFilter += string.Format("SELECT LOCATION_CODE FROM IP_LOCATION_SETUP WHERE LOCATION_CODE LIKE '{0}%' ", locations[i]);
                    else
                    {
                        locationFilter += string.Format(" OR LOCATION_CODE like '{0}%' ", locations[i]);
                    }
                }
                locationFilter = string.Format(@" AND SI.FROM_LOCATION_CODE IN ({0} OR LOCATION_CODE IN ('{1}'))", locationFilter, string.Join("','", locations));
                //query = query.AppendFormat(@" AND A.FROM_LOCATION_CODE IN ('{0}')", string.Join("','", filters.LocationFilter).ToString());
                Query = Query + locationFilter;
            }
            //****************************
            //CONDITIONS FITLER END HERE
            //****************************

            if (string.Equals(dateFormat.ToLower(), "ad"))
            {
                Query += @" 
                            group by  to_char(SI.sales_date, 'Month'),to_char(SI.sales_date, 'YYYYMM'),to_char(SI.sales_date, 'Mon-YYY'),CS.ABBR_CODE
                           -- order by to_char(SI.sales_date, 'YYYYMM')";
            }
            else
            {
                Query += @" 
                            group by  FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SI.Sales_date),6,2)),substr(bs_date(SI.sales_date),1,7),CS.ABBR_CODE
                           -- ORDER BY substr(bs_date(SI.sales_date),1,7) ";
            }
            if (reportFilters.FiscalYearFilter.Count() > 0)
            {
                var oldQuery = Query;
                List<string> temp = new List<string>();
                foreach (var item in reportFilters.FiscalYearFilter)
                {
                    Query = oldQuery.Replace("sa_sales_invoice", item.DBName + ".sa_sales_invoice").Replace("COMPANY_SETUP", item.DBName + ".COMPANY_SETUP");
                    Query = Query.Replace(fiscalYear, item.FiscalYear);
                    Query = Query.Replace("dbName", item.DBName);
                    temp.Add(Query);
                }
                Query = string.Join(@"
                                UNION
                                ", temp);
            }


            Query = string.Format(Query, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode);
            var datas = _objectEntity.SqlQuery<MonthlySalesFiscalYearGraph>(Query).ToList();
            //var groupMetric = datas.GroupBy(info => info.FiscalYear)
            //            .Select(group => new
            //            {
            //                Metric = group.Key,
            //                Count = group.Count()
            //            });

            //var MaxFroupMetric = groupMetric.Where(x=>x.Count== groupMetric.Max(y => y.Count)).FirstOrDefault();

            //var MainList = new List<MonthlySalesFiscalYearGraph>();
            //var tempList = new List<MonthlySalesFiscalYearGraph>();
            //MainList.AddRange(datas.Where(x => x.FiscalYear == MaxFroupMetric.Metric).ToList());
            //var OtherMaxdata = datas.Where(x => x.FiscalYear != MaxFroupMetric.Metric);
            //   foreach(var newarray in MainList)
            //    {
            //    foreach (var a in groupMetric)
            //    {
            //        if (a.Metric == newarray.FiscalYear)
            //            continue;
            //        var matchingMetric = OtherMaxdata.Where(x => x.FiscalYear == a.Metric && x.MonthInt == newarray.MonthInt).FirstOrDefault();
            //        if (matchingMetric == null)
            //        {
            //            var model = new MonthlySalesFiscalYearGraph();
            //            model = newarray;
            //            model.Amount = 0;
            //            model.Quantity = 0;
            //            model.DBName = OtherMaxdata.Where(x => x.FiscalYear == a.Metric).FirstOrDefault()==null?a.Metric:OtherMaxdata.Where(x => x.FiscalYear == a.Metric).FirstOrDefault().DBName;
            //            model.FiscalYear = OtherMaxdata.Where(x => x.FiscalYear == a.Metric).FirstOrDefault() == null ? a.Metric : OtherMaxdata.Where(x => x.FiscalYear == a.Metric).FirstOrDefault().FiscalYear;
            //            //  model.
            //            tempList.Add(model);
            //        }
            //        else
            //        {
            //            tempList.Add(matchingMetric);
            //        }
            //    }     
            //    }

            //MainList.AddRange(tempList);

            return datas;
        }

        public List<MonthlySalesFiscalYearGraph> AgingCategoryWise(ReportFiltersModel reportFilters, User userInfo, string dateFormat)
        {

            var fiscalYear = System.Configuration.ConfigurationManager.AppSettings["FiscalYear"].Substring(2);
            var companyCode = string.Join(",", reportFilters.CompanyFilter);
            companyCode = companyCode == "" ? userInfo.company_code : companyCode;
            var branchname= string.Join(",", reportFilters.DivisionFilter);
            if (dateFormat.ToLower() != "amount".Trim())
            {
                string Query = $@"select * from (SELECT round(SUM(""0-30""),2) ""0-30"",round(SUM(""31-60""),2)  ""31-60"",  round(SUM(""61-90""),2) ""61-90"", round(SUM(""90++""),2) ""90+"" FROM ( 
WITH STOCK_AGEING AS
(
SELECT ITEM_CODE, ITEM_EDESC, CATEGORY_CODE, STOCK_AGEING FROM (
SELECT ITEM_CODE, ITEM_EDESC , CATEGORY_CODE, FN_STOCK_AGEING(ITEM_CODE, COMPANY_CODE) STOCK_AGEING  FROM IP_ITEM_MASTER_SETUP 
WHERE COMPANY_CODE IN ('{companyCode}')
AND PRE_ITEM_CODE LIKE('{branchname}%')
--AND CATEGORY_CODE = 'RM'
--AND ITEM_CODE = 2959
ORDER BY ITEM_EDESC
)
)
SELECT ITEM_CODE, ITEM_EDESC, CATEGORY_CODE,
REGEXP_SUBSTR(STOCK_AGEING, '[^,]+', 1, 1) AS ""0-30"", 
REGEXP_SUBSTR(STOCK_AGEING, '[^,]+', 1, 2) AS ""31-60"", 
REGEXP_SUBSTR(STOCK_AGEING, '[^,]+', 1, 3) AS ""61-90"", 
REGEXP_SUBSTR(STOCK_AGEING, '[^,]+', 1, 4) AS ""90++"" FROM STOCK_AGEING
)
) UNPIVOT(Quantity FOR DisplayMonth IN(""0-30"" AS '0-30', ""31-60"" AS '31-60', ""61-90"" as '61-90', ""90+"" as '90+'))";
                var datas = _objectEntity.SqlQuery<MonthlySalesFiscalYearGraph>(Query).ToList();
                return datas;
            }
            else
            {
                string Query = $@"select * from (SELECT  SUM(""0-30"") as ""0-30"",SUM(""31-60"") as ""31-60"",  SUM(""61-90"") as ""61-90"", SUM(""90++"") as ""90++"" FROM ( 
WITH STOCK_AGEING AS
(
SELECT ITEM_CODE, ITEM_EDESC, CATEGORY_CODE, RATE, STOCK_AGEING FROM(
SELECT ITEM_CODE, ITEM_EDESC, CATEGORY_CODE,
CASE NVL(PURCHASE_PRICE, 0)
WHEN 0 THEN
    (SELECT NVL(SUM(B.PHYSICAL_STOCK_VALUE), 0) / DECODE(NVL(SUM(B.PHYSICAL_QUANTITY), 1), 0, 1, NVL(SUM(B.PHYSICAL_QUANTITY), 1))FROM IP_ITEM_OPENING_DETAIL_SETUP B
    WHERE B.ITEM_CODE = A.ITEM_CODE
    AND B.COMPANY_CODE = A.COMPANY_CODE)
ELSE
    NVL(PURCHASE_PRICE, 0)
END RATE,
 FN_STOCK_AGEING(ITEM_CODE, COMPANY_CODE) STOCK_AGEING  FROM IP_ITEM_MASTER_SETUP A
WHERE COMPANY_CODE IN ('{companyCode}')
AND PRE_ITEM_CODE LIKE('{branchname}%')
AND A.DELETED_FLAG = 'N'
--AND CATEGORY_CODE = 'RM'
--AND ITEM_CODE = 2959
ORDER BY ITEM_EDESC
)
)
SELECT ITEM_CODE, ITEM_EDESC, CATEGORY_CODE, RATE,
REGEXP_SUBSTR(STOCK_AGEING, '[^,]+', 1, 1)* RATE     AS ""0-30"", 
REGEXP_SUBSTR(STOCK_AGEING, '[^,]+', 1, 2)* RATE    AS ""31-60"", 
REGEXP_SUBSTR(STOCK_AGEING, '[^,]+', 1, 3)* RATE    AS ""61-90"", 
REGEXP_SUBSTR(STOCK_AGEING, '[^,]+', 1, 4)* RATE    AS ""90++"" FROM STOCK_AGEING
--WHERE REGEXP_SUBSTR(STOCK_AGEING, '[^,]+', 1, 1) >= 0
)) UNPIVOT(Quantity FOR DisplayMonth IN(""0-30"" AS '0-30', ""31-60"" AS '31-60', ""61-90"" as '61-90', ""90++"" as '90+'))";
                var datas = _objectEntity.SqlQuery<MonthlySalesFiscalYearGraph>(Query).ToList();
                return datas;
            }
            //var groupMetric = datas.GroupBy(info => info.FiscalYear)
            //            .Select(group => new
            //            {
            //                Metric = group.Key,
            //                Count = group.Count()
            //            });

            //var MaxFroupMetric = groupMetric.Where(x=>x.Count== groupMetric.Max(y => y.Count)).FirstOrDefault();

            //var MainList = new List<MonthlySalesFiscalYearGraph>();
            //var tempList = new List<MonthlySalesFiscalYearGraph>();
            //MainList.AddRange(datas.Where(x => x.FiscalYear == MaxFroupMetric.Metric).ToList());
            //var OtherMaxdata = datas.Where(x => x.FiscalYear != MaxFroupMetric.Metric);
            //   foreach(var newarray in MainList)
            //    {
            //    foreach (var a in groupMetric)
            //    {
            //        if (a.Metric == newarray.FiscalYear)
            //            continue;
            //        var matchingMetric = OtherMaxdata.Where(x => x.FiscalYear == a.Metric && x.MonthInt == newarray.MonthInt).FirstOrDefault();
            //        if (matchingMetric == null)
            //        {
            //            var model = new MonthlySalesFiscalYearGraph();
            //            model = newarray;
            //            model.Amount = 0;
            //            model.Quantity = 0;
            //            model.DBName = OtherMaxdata.Where(x => x.FiscalYear == a.Metric).FirstOrDefault()==null?a.Metric:OtherMaxdata.Where(x => x.FiscalYear == a.Metric).FirstOrDefault().DBName;
            //            model.FiscalYear = OtherMaxdata.Where(x => x.FiscalYear == a.Metric).FirstOrDefault() == null ? a.Metric : OtherMaxdata.Where(x => x.FiscalYear == a.Metric).FirstOrDefault().FiscalYear;
            //            //  model.
            //            tempList.Add(model);
            //        }
            //        else
            //        {
            //            tempList.Add(matchingMetric);
            //        }
            //    }     
            //    }

            //MainList.AddRange(tempList);
            var datas2 = new List<MonthlySalesFiscalYearGraph>();
            return datas2;
        }

        public List<MonthlySalesFiscalYearGraph> customerWiseaging(ReportFiltersModel reportFilters, User userInfo, string dateFormat)
        {

            //var fiscalYear = System.Configuration.ConfigurationManager.AppSettings["FiscalYear"].Substring(2);
            var companyCode = string.Join(",", reportFilters.CompanyFilter);
            companyCode = companyCode == "" ? userInfo.company_code : companyCode;
            var branchname = string.Join(",", reportFilters.DivisionFilter);
            string Query = $@"select * from (SELECT  SUM(""0-30"") as ""0-30"",SUM(""31-60"") as ""31-60"",  SUM(""61-90"") as ""61-90"", SUM(""90++"") as ""90++"" FROM ( 
WITH CUSTOMER_AGEING AS
(
SELECT CUSTOMER_CODE, CUSTOMER_EDESC, CUSTOMER_AGEING FROM(
SELECT CUSTOMER_CODE, CUSTOMER_EDESC,
 FN_CUSTOMER_AGEING('C' || CUSTOMER_CODE, COMPANY_CODE) CUSTOMER_AGEING  FROM SA_CUSTOMER_SETUP A
WHERE COMPANY_CODE IN('{companyCode}')
--AND PRE_CUSTOMER_CODE LIKE('07%')
AND A.DELETED_FLAG = 'N'
ORDER BY CUSTOMER_EDESC
)
)
SELECT CUSTOMER_CODE, CUSTOMER_EDESC,
REGEXP_SUBSTR(CUSTOMER_AGEING, '[^,]+', 1, 1) AS ""0-30"", 
REGEXP_SUBSTR(CUSTOMER_AGEING, '[^,]+', 1, 2) AS ""31-60"", 
REGEXP_SUBSTR(CUSTOMER_AGEING, '[^,]+', 1, 3) AS ""61-90"", 
REGEXP_SUBSTR(CUSTOMER_AGEING, '[^,]+', 1, 4) AS ""90++"" FROM CUSTOMER_AGEING
)) UNPIVOT(Quantity FOR DisplayMonth IN(""0-30"" AS '0-30', ""31-60"" AS '31-60', ""61-90"" as '61-90', ""90++"" as '90+'))";
            var datas = _objectEntity.SqlQuery<MonthlySalesFiscalYearGraph>(Query).ToList();
            return datas;
           // var datas2 = new List<MonthlySalesFiscalYearGraph>();
           // return datas2;
        }
        public List<MonthlySalesFiscalYearGraph> GetCollectionMonthFiscal(ReportFiltersModel reportFilters, User userInfo, string dateFormat)
        {

            var fiscalYear = System.Configuration.ConfigurationManager.AppSettings["FiscalYear"].Substring(2);
            var companyCode = string.Join(",", reportFilters.CompanyFilter);
            companyCode = companyCode == "" ? userInfo.company_code : companyCode;

            string Query = string.Empty;
            if (string.Equals(dateFormat.ToLower(), "ad"))
            {
                Query = @"select to_char(I.VOUCHER_DATE, 'Month') as month,to_char(I.VOUCHER_DATE, 'YYYYMM') as MonthInt, nvl(sum(nvl(I.CR_AMOUNT,0))/{0},0)as amount,to_char(I.VOUCHER_DATE, 'Mon-YYY') as DisplayMonth 
                          ,CS.ABBR_CODE ||' " + fiscalYear + @"' AS FiscalYear ,'dbName' as DBName
                           from v$virtual_sub_ledger I,COMPANY_SETUP CS
                            where I.COMPANY_CODE = CS.COMPANY_CODE
                            AND I.DELETED_FLAG='N' AND CS.DELETED_FLAG  = 'N'    
                            AND I.COMPANY_CODE IN (" + companyCode + ")";
            }
            else
            {
                Query = @" select trim(substr(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(I.VOUCHER_DATE),6,2)),5,20))  DisplayMonth, 
                          nvl(sum(nvl(I.CR_AMOUNT,0))/{0},0)as amount,substr(bs_date(I.VOUCHER_DATE),1,7) as MonthInt 
                         ,CS.ABBR_CODE ||' " + fiscalYear + @"' AS FiscalYear,'dbName' as DBName from  v$virtual_sub_ledger I,COMPANY_SETUP CS    
                         where I.COMPANY_CODE = CS.COMPANY_CODE
                         AND  I.deleted_flag='N' AND CS.DELETED_FLAG  = 'N'    
                         AND I.COMPANY_CODE IN (" + companyCode + @")";
            }


            //****************************
            //CONDITIONS FITLER START HERE
            //****************************
            //for customer Filter
            var customerFilter = string.Empty;
            if (reportFilters.CustomerFilter.Count() > 0)
            {
                customerFilter = @"select DISTINCT('C' || customer_code) from sa_customer_setup where (";
                //IF CUSTOMER_SKU_FLAG = G
                foreach (var item in reportFilters.CustomerFilter)
                {
                    customerFilter += "master_customer_code like  (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                customerFilter = customerFilter.Substring(0, customerFilter.Length - 3);
                //IF CUSTOMER_SKU_FLAG = I                
                customerFilter += " or (customer_code in (" + string.Join(",", reportFilters.CustomerFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                Query += " and I.SUB_CODE IN(" + customerFilter + ")";
            }

            //for Product Filter
            var productFilter = string.Empty;
            if (reportFilters.ProductFilter.Count() > 0)
            {
                productFilter = @"select  DISTINCT item_code from IP_ITEM_MASTER_SETUP where (";
                //IF PRODUCT_SKU_FLAG = G
                foreach (var item in reportFilters.ProductFilter)
                {
                    productFilter += "MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                productFilter = productFilter.Substring(0, productFilter.Length - 3);
                //IF PRODUCT_SKU_FLAG = I                
                productFilter += " or (ITEM_CODE in (" + string.Join(",", reportFilters.ProductFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                Query += " and SI.ITEM_CODE IN(" + productFilter + ")";
            }


            if (reportFilters.BranchFilter.Count > 0)
            {
                Query += string.Format(@" AND SI.BRANCH_CODE IN ('{0}')", string.Join("','", reportFilters.BranchFilter).ToString());
            }
            if (reportFilters.EmployeeFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND  SI.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", reportFilters.EmployeeFilter).ToString());
            }
            if (reportFilters.AgentFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND  SI.AGENT_CODE IN  ('{0}')", string.Join("','", reportFilters.AgentFilter).ToString());
            }
            if (reportFilters.DivisionFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND  I.DIVISION_CODE IN  ('{0}')", string.Join("','", reportFilters.DivisionFilter).ToString());
            }
            string locationFilter = string.Empty;
            if (reportFilters.LocationFilter.Count > 0)
            {

                var locations = reportFilters.LocationFilter;
                for (int i = 0; i < locations.Count; i++)
                {

                    if (i == 0)
                        locationFilter += string.Format("SELECT LOCATION_CODE FROM IP_LOCATION_SETUP WHERE LOCATION_CODE LIKE '{0}%' ", locations[i]);
                    else
                    {
                        locationFilter += string.Format(" OR LOCATION_CODE like '{0}%' ", locations[i]);
                    }
                }
                locationFilter = string.Format(@" AND SI.FROM_LOCATION_CODE IN ({0} OR LOCATION_CODE IN ('{1}'))", locationFilter, string.Join("','", locations));
                //query = query.AppendFormat(@" AND A.FROM_LOCATION_CODE IN ('{0}')", string.Join("','", filters.LocationFilter).ToString());
                Query = Query + locationFilter;
            }
            //****************************
            //CONDITIONS FITLER END HERE
            //****************************

            if (string.Equals(dateFormat.ToLower(), "ad"))
            {
                Query += @" 
                            group by to_char(I.VOUCHER_DATE, 'Month'),to_char(I.VOUCHER_DATE, 'YYYYMM'),to_char(I.VOUCHER_DATE, 'Mon-YYY'),CS.ABBR_CODE
                           -- order by to_char(SI.sales_date, 'YYYYMM')";
            }
            else
            {
                Query += @" 
                            group by FN_CHARTBS_MONTH(SUBSTR(BS_DATE(I.VOUCHER_DATE),6,2)),substr(bs_date(I.VOUCHER_DATE),1,7),CS.ABBR_CODE
                           -- ORDER BY substr(bs_date(SI.sales_date),1,7) ";
            }
            if (reportFilters.FiscalYearFilter.Count() > 0)
            {
                var oldQuery = Query;
                List<string> temp = new List<string>();
                foreach (var item in reportFilters.FiscalYearFilter)
                {
                    Query = oldQuery.Replace("sa_sales_invoice", item.DBName + ".sa_sales_invoice").Replace("COMPANY_SETUP", item.DBName + ".COMPANY_SETUP");
                    Query = Query.Replace(fiscalYear, item.FiscalYear);
                    Query = Query.Replace("dbName", item.DBName);
                    temp.Add(Query);
                }
                Query = string.Join(@"
                                UNION
                                ", temp);
            }


            Query = string.Format(Query, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode);
            var datas = _objectEntity.SqlQuery<MonthlySalesFiscalYearGraph>(Query).ToList();
            //var groupMetric = datas.GroupBy(info => info.FiscalYear)
            //            .Select(group => new
            //            {
            //                Metric = group.Key,
            //                Count = group.Count()
            //            });

            //var MaxFroupMetric = groupMetric.Where(x=>x.Count== groupMetric.Max(y => y.Count)).FirstOrDefault();

            //var MainList = new List<MonthlySalesFiscalYearGraph>();
            //var tempList = new List<MonthlySalesFiscalYearGraph>();
            //MainList.AddRange(datas.Where(x => x.FiscalYear == MaxFroupMetric.Metric).ToList());
            //var OtherMaxdata = datas.Where(x => x.FiscalYear != MaxFroupMetric.Metric);
            //   foreach(var newarray in MainList)
            //    {
            //    foreach (var a in groupMetric)
            //    {
            //        if (a.Metric == newarray.FiscalYear)
            //            continue;
            //        var matchingMetric = OtherMaxdata.Where(x => x.FiscalYear == a.Metric && x.MonthInt == newarray.MonthInt).FirstOrDefault();
            //        if (matchingMetric == null)
            //        {
            //            var model = new MonthlySalesFiscalYearGraph();
            //            model = newarray;
            //            model.Amount = 0;
            //            model.Quantity = 0;
            //            model.DBName = OtherMaxdata.Where(x => x.FiscalYear == a.Metric).FirstOrDefault()==null?a.Metric:OtherMaxdata.Where(x => x.FiscalYear == a.Metric).FirstOrDefault().DBName;
            //            model.FiscalYear = OtherMaxdata.Where(x => x.FiscalYear == a.Metric).FirstOrDefault() == null ? a.Metric : OtherMaxdata.Where(x => x.FiscalYear == a.Metric).FirstOrDefault().FiscalYear;
            //            //  model.
            //            tempList.Add(model);
            //        }
            //        else
            //        {
            //            tempList.Add(matchingMetric);
            //        }
            //    }     
            //    }

            //MainList.AddRange(tempList);

            return datas;
        }

        public List<SalesCollectionDivisionDayWise> GetSalesCollectionDivisionWise(ReportFiltersModel reportFilters, User userInfo, string dateFormat, string divisionCode)
        {

            var companyCode = string.Join(",", reportFilters.CompanyFilter);
            companyCode = companyCode == "" ? userInfo.company_code : companyCode;

            var Query = string.Empty;
            if (dateFormat == "AD")
            {

            }
            else
            {

                Query = $@" SELECT c.COLLECTION,C.MONTH,C.MonthNo,nvl((select sum(total_sales)/1  from V$SALES_INVOICE_REPORT3 where division_code=c.division_code and  deleted_flag='N' and company_code=c.company_code and  substr(bs_date(sales_date),6,2)= MonthNo ),0) Sales FROM ( select 
                                             SUM (nvl(l.cr_amount,0))/1 as Collection,                                             
                                             fn_bs_month(substr(bs_date(l.VOUCHER_DATE),6,2)) AS MONTH    ,
                                             substr(bs_date(l.VOUCHER_DATE),6,2) MonthNo  ,
                                             l.company_code   ,
                                             l.division_code                         
                                        from v$virtual_sub_ledger l
                                        WHERE  l.company_code in('01')
                                        AND DELETED_FLAG = 'N' AND l.DIVISION_CODE IS NOT NULL
                                        and l.division_code='{divisionCode}'
                                        group by substr(bs_date(l.VOUCHER_DATE),6,2),l.company_code ,  l.division_code
                                        ORDER BY substr(bs_date(l.VOUCHER_DATE),6,2)
                                        ) c";

                //Query = string.Format(@"select 
                //                            SUM (nvl(l.dr_amount,0))/{0} as Sales,
                //                             SUM (nvl(l.cr_amount,0))/{0} as Collection,                                             
                //                             fn_bs_month(substr(bs_date(l.VOUCHER_DATE),6,2)) AS MONTH    ,
                //                             substr(bs_date(l.VOUCHER_DATE),6,2) MonthNo                               
                //                        from v$virtual_sub_ledger l
                //                        WHERE  l.company_code in('{1}')
                //                        AND DELETED_FLAG = 'N' AND l.DIVISION_CODE IS NOT NULL
                //                        group by substr(bs_date(l.VOUCHER_DATE),6,2)
                //                        ORDER BY substr(bs_date(l.VOUCHER_DATE),6,2)", ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode);
            }

            var salesReportList = _objectEntity.SqlQuery<SalesCollectionDivisionDayWise>(Query).ToList();
            return salesReportList;
        }

        public List<SalesCollectionDivisionDayWise> GetSalesCollectionDivisionWiseDaily(ReportFiltersModel reportFilters, User userInfo, string month, string dateFormat)
        {

            var companyCode = string.Join(",", reportFilters.CompanyFilter);
            companyCode = companyCode == "" ? userInfo.company_code : companyCode;

            var Query = string.Format(@"select 
                                            SUM (nvl(l.dr_amount,0))/{0} as Sales,
                                             SUM (nvl(l.cr_amount,0))/{0} as Collection,
                                            --.Division_code Division,
                                            substr(bs_date(l.VOUCHER_DATE),9,2) Day,
                                             fn_bs_month(substr(bs_date(l.VOUCHER_DATE),6,2)) AS MONTH    ,
                                             substr(bs_date(l.VOUCHER_DATE),6,2) MonthNo                               
                                        from v$virtual_sub_ledger l
                                        WHERE  l.company_code = '{1}'
                                        and substr(bs_date(l.VOUCHER_DATE),6,2) ='{2}'
                                        AND DELETED_FLAG = 'N' AND l.DIVISION_CODE IS NOT NULL
                                        group by  substr(bs_date(l.VOUCHER_DATE),9,2) , substr(bs_date(l.VOUCHER_DATE),6,2)
                                        ORDER BY substr(bs_date(l.VOUCHER_DATE),9,2) ", ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode, month);

            var salesReportList = _objectEntity.SqlQuery<SalesCollectionDivisionDayWise>(Query).ToList();
            return salesReportList;
        }


        public List<SalesProductRateFiscalYearGraph> GetSalesProductRateFiscalYear(ReportFiltersModel reportFilters, string dateFormat)
        {

            var fiscalYear = System.Configuration.ConfigurationManager.AppSettings["FiscalYear"].Substring(2);
            var companyCode = string.Join(",", reportFilters.CompanyFilter);

            string branchFilter = string.Empty;
            if (reportFilters.BranchFilter.Count > 0)
            {
                branchFilter = string.Format(@" AND BRANCH_CODE IN ('{0}')", string.Join("','", reportFilters.BranchFilter).ToString());
            }

            string Query = string.Empty;
            if (string.Equals(dateFormat.ToLower(), "ad"))
            {
                Query = @"select DISTINCT  ITEM_CODE,
                                       FN_FETCH_DESC(COMPANY_CODE,'IP_ITEM_MASTER_SETUP',ITEM_CODE) AS ITEM_DESC,
                                       to_char(APP_DATE, 'YYYY-MON-DD') as APP_DATE ,
                                       to_char(APP_DATE, 'YYYYMMDD') as MonthInt, 
                                       SALES_RATE 
                              from IP_ITEM_RATE_APPLICAT_SETUP
                            WHERE DELETED_FLAG = 'N'
                            AND SALES_RATE <> 0
                            AND COMPANY_CODE IN (" + companyCode + ")" + branchFilter;
            }
            else
            {
                Query = @"";
            }


            //****************************
            //CONDITIONS FITLER START HERE
            //****************************

            //for customer Filter
            var customerFilter = string.Empty;
            if (reportFilters.CustomerFilter.Count() > 0)
            {
                customerFilter = @"select  DISTINCT(customer_code) from sa_customer_setup where (";
                //IF CUSTOMER_SKU_FLAG = G
                foreach (var item in reportFilters.CustomerFilter)
                {
                    customerFilter += "master_customer_code like  (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                customerFilter = customerFilter.Substring(0, customerFilter.Length - 3);
                //IF CUSTOMER_SKU_FLAG = I                
                customerFilter += " or (customer_code in (" + string.Join(",", reportFilters.CustomerFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                Query += " and customer_code IN(" + customerFilter + ")";
            }




            ////for Product Filter
            var productFilter = string.Empty;
            if (reportFilters.ProductFilter.Count() > 0)
            {
                productFilter = @"select  DISTINCT item_code from IP_ITEM_MASTER_SETUP where (";
                //IF PRODUCT_SKU_FLAG = G
                foreach (var item in reportFilters.ProductFilter)
                {
                    productFilter += "MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                productFilter = productFilter.Substring(0, productFilter.Length - 3);
                //IF PRODUCT_SKU_FLAG = I                
                productFilter += " or (ITEM_CODE in (" + string.Join(",", reportFilters.ProductFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                Query += " and ITEM_CODE IN(" + productFilter + ")";
            }


            if (reportFilters.BranchFilter.Count > 0)
            {
                Query += string.Format(@" AND BRANCH_CODE IN ('{0}')", string.Join("','", reportFilters.BranchFilter).ToString());
            }

            //****************************
            //CONDITIONS FITLER END HERE
            //****************************



            Query += @" 
                       ORDER BY APP_DATE";

            if (reportFilters.FiscalYearFilter.Count() > 0)
            {
                var oldQuery = Query;
                List<string> temp = new List<string>();
                foreach (var item in reportFilters.FiscalYearFilter)
                {
                    Query = oldQuery.Replace("sa_sales_invoice", item.DBName + ".sa_sales_invoice").Replace("COMPANY_SETUP", item.DBName + ".COMPANY_SETUP");
                    Query = Query.Replace(fiscalYear, item.FiscalYear);
                    Query = Query.Replace("dbName", item.DBName);
                    temp.Add(Query);
                }
                Query = string.Join(@"
                                UNION
                                ", temp);
            }


            Query = string.Format(Query, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode);
            var datas = _objectEntity.SqlQuery<SalesProductRateFiscalYearGraph>(Query).ToList();
            return datas;
        }


        public List<SalesProductRateFiscalYearGraph> GetAvgPurchaseRateFiscalYear(ReportFiltersModel reportFilters, string duration, User userInfo)
        {
            var FincalYear = System.Configuration.ConfigurationManager.AppSettings["FiscalYear"].ToString();
            var fiscalYear = FincalYear.Substring(2);
            var dateRange = this._controlService.GetDateFilters(FincalYear).OrderByDescending(q => q.SortOrder).ToList();
            var dates = dateRange.Where(x => x.RangeName == duration).FirstOrDefault();
            var companyCode = string.Join(",", reportFilters.CompanyFilter);
            companyCode = companyCode == "" ? userInfo.company_code : companyCode;

            string Query = string.Empty;
            Query = $@"SELECT ITEM_CODE,FN_FETCH_DESC(COMPANY_CODE,'IP_ITEM_MASTER_SETUP',ITEM_CODE) AS ITEM_DESC,
                              ROUND(AVG(UNIT_PRICE),2) SALES_RATE,
                              '{ fiscalYear }' AS FiscalYear ,'dbName' as DBName
                        FROM IP_PURCHASE_INVOICE
                        WHERE DELETED_FLAG = 'N'                        
                            AND COMPANY_CODE IN ({companyCode})
                            AND INVOICE_DATE >= TO_DATE('{dates.StartDateString}','YYYY-MM-DD') AND INVOICE_DATE <= TO_DATE('{dates.EndDateString}','YYYY-MM-DD') ";
            //****************************
            //CONDITIONS FITLER START HERE
            //****************************

            ////for Product Filter
            var productFilter = string.Empty;
            if (reportFilters.ProductFilter.Count() > 0)
            {
                productFilter = @"select  DISTINCT item_code from IP_ITEM_MASTER_SETUP where (";
                //IF PRODUCT_SKU_FLAG = G
                foreach (var item in reportFilters.ProductFilter)
                {
                    productFilter += "MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                productFilter = productFilter.Substring(0, productFilter.Length - 3);
                //IF PRODUCT_SKU_FLAG = I                
                productFilter += " or (ITEM_CODE in (" + string.Join(",", reportFilters.ProductFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                Query += " and ITEM_CODE IN(" + productFilter + ")";
            }


            if (reportFilters.BranchFilter.Count > 0)
            {
                Query += string.Format(@" AND BRANCH_CODE IN ('{0}')", string.Join("','", reportFilters.BranchFilter).ToString());
            }

            //****************************
            //CONDITIONS FITLER END HERE
            //****************************



            Query += @" 
                       GROUP BY ITEM_CODE,COMPANY_CODE";

            if (reportFilters.FiscalYearFilter.Count() > 0)
            {
                var oldQuery = Query;
                List<string> temp = new List<string>();

                foreach (var item in reportFilters.FiscalYearFilter)
                {
                    //for fiscal year dates
                    var fiscalYear1 = "20" + item.FiscalYear;
                    int substractYear = Convert.ToInt32(item.FiscalYear.Substring(0, 2)) - Convert.ToInt32(fiscalYear.Substring(0, 2));
                    var dateRange1 = this._controlService.GetDateFilters(fiscalYear1, "", false, substractYear).OrderByDescending(q => q.SortOrder).ToList();
                    var dates1 = dateRange1.Where(x => x.RangeName == duration).FirstOrDefault();

                    Query = oldQuery.Replace("IP_PURCHASE_INVOICE", item.DBName + ".IP_PURCHASE_INVOICE")
                                    .Replace(dates.StartDateString, dates1.StartDateString)
                                    .Replace(dates.EndDateString, dates1.EndDateString);
                    Query = Query.Replace(fiscalYear, item.FiscalYear);
                    Query = Query.Replace("dbName", item.DBName);
                    temp.Add(Query);
                }
                Query = string.Join(@"
                                UNION
                                ", temp);
            }


            Query = string.Format(Query, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode);
            var datas = _objectEntity.SqlQuery<SalesProductRateFiscalYearGraph>(Query).ToList();
            return datas;
        }


        public List<SaudaModel> GetSaudaQuantityReport(ReportFiltersModel reportFilters, string duration, User userInfo)
        {
            var FincalYear = System.Configuration.ConfigurationManager.AppSettings["FiscalYear"].ToString();
            var dateRange = this._controlService.GetDateFilters(FincalYear).OrderByDescending(q => q.SortOrder).ToList();
            var dates = dateRange.Where(x => x.RangeName == duration).FirstOrDefault();
            //var companyCode = string.Join(",", reportFilters.CompanyFilter);
            //companyCode = companyCode == "" ? userInfo.company_code : companyCode;
            var companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            string Query = string.Empty;

            //****************************
            //CONDITIONS FITLER START HERE
            //****************************

            ////for Product Filter
            var productFilter = string.Empty;
            if (reportFilters.ProductFilter.Count() > 0)
            {
                productFilter = @"select  DISTINCT item_code from IP_ITEM_MASTER_SETUP where (";
                //IF PRODUCT_SKU_FLAG = G
                foreach (var item in reportFilters.ProductFilter)
                {
                    productFilter += "MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                productFilter = productFilter.Substring(0, productFilter.Length - 3);
                //IF PRODUCT_SKU_FLAG = I                
                productFilter += " or (ITEM_CODE in (" + string.Join(",", reportFilters.ProductFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                productFilter = " and IM.ITEM_CODE IN(" + productFilter + ")";
            }

            var branchFilter1 = string.Empty;
            var branchFilter2 = string.Empty;
            if (reportFilters.BranchFilter.Count > 0)
            {
                branchFilter1 = string.Format(@" AND PI.BRANCH_CODE IN ('{0}')", string.Join("','", reportFilters.BranchFilter).ToString());
                branchFilter2 = string.Format(@" AND PO.BRANCH_CODE IN ('{0}')", string.Join("','", reportFilters.BranchFilter).ToString());
            }

            //****************************
            //CONDITIONS FITLER END HERE
            //****************************


            Query = $@"SELECT D.ITEM_CODE,D.ITEM_DESC, NVL(D.DIRECT_QUANTITY,0) DIRECT_QUANTITY,NVL(S.SAUDA_QUANTITY,0) SAUDA_QUANTITY  FROM
                                (SELECT PI.ITEM_CODE,IM.ITEM_EDESC ITEM_DESC, NVL(SUM(PI.CALC_QUANTITY),0) DIRECT_QUANTITY
                                FROM IP_PURCHASE_INVOICE PI,IP_ITEM_MASTER_SETUP IM
                                WHERE 
                                PI.ITEM_CODE = IM.ITEM_CODE
                                AND PI.COMPANY_CODE = IM.COMPANY_CODE
                                AND PI.COMPANY_CODE IN({companyCode})  {branchFilter1}
                                {productFilter}
                                AND INVOICE_DATE >= TO_DATE('{dates.StartDateString}','YYYY-MM-DD') AND INVOICE_DATE <= TO_DATE('{dates.EndDateString}','YYYY-MM-DD')
                                AND PI.DELETED_FLAG = 'N'
                                AND IM.CATEGORY_CODE = 'RM'
                                GROUP BY PI.ITEM_CODE,IM.ITEM_EDESC) D,
                                (SELECT PO.ITEM_CODE,IM.ITEM_EDESC AS ITEM_DESC, NVL(SUM(PO.CALC_QUANTITY),0) SAUDA_QUANTITY
                                FROM IP_PURCHASE_ORDER PO,IP_ITEM_MASTER_SETUP IM
                                WHERE PO.ITEM_CODE = IM.ITEM_CODE
                                AND PO.COMPANY_CODE = IM.COMPANY_CODE
                                AND PO.COMPANY_CODE IN({companyCode}) {branchFilter2}
                                {productFilter}
                                AND ORDER_DATE >= TO_DATE('{dates.StartDateString}','YYYY-MM-DD') AND ORDER_DATE <= TO_DATE('{dates.EndDateString}','YYYY-MM-DD')
                                AND PO.DELETED_FLAG = 'N'
                                AND IM.CATEGORY_CODE = 'RM'
                                GROUP BY PO.ITEM_CODE,IM.ITEM_EDESC) S
                                WHERE D.ITEM_CODE(+) = S.ITEM_CODE
                                ORDER BY ITEM_DESC";




            Query = string.Format(Query, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode);
            var datas = _objectEntity.SqlQuery<SaudaModel>(Query).ToList();
            return datas;
        }

        public List<SaudaModel> GetSaudaAveragePurchaseRateReport(ReportFiltersModel reportFilters, string duration, User userInfo)
        {
            var FincalYear = System.Configuration.ConfigurationManager.AppSettings["FiscalYear"].ToString();
            var dateRange = this._controlService.GetDateFilters(FincalYear).OrderByDescending(q => q.SortOrder).ToList();
            var dates = dateRange.Where(x => x.RangeName == duration).FirstOrDefault();
            //var companyCode = string.Join(",", reportFilters.CompanyFilter);
            //companyCode = companyCode == "" ? userInfo.company_code : companyCode;
            var companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            string Query = string.Empty;

            //****************************
            //CONDITIONS FITLER START HERE
            //****************************

            ////for Product Filter
            var productFilter = string.Empty;
            if (reportFilters.ProductFilter.Count() > 0)
            {
                productFilter = @"select  DISTINCT item_code from IP_ITEM_MASTER_SETUP where (";
                //IF PRODUCT_SKU_FLAG = G
                foreach (var item in reportFilters.ProductFilter)
                {
                    productFilter += "MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                productFilter = productFilter.Substring(0, productFilter.Length - 3);
                //IF PRODUCT_SKU_FLAG = I                
                productFilter += " or (ITEM_CODE in (" + string.Join(",", reportFilters.ProductFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                productFilter = " and IM.ITEM_CODE IN(" + productFilter + ")";
            }

            var branchFilter1 = string.Empty;
            var branchFilter2 = string.Empty;
            if (reportFilters.BranchFilter.Count > 0)
            {
                branchFilter1 = string.Format(@" AND PI.BRANCH_CODE IN ('{0}')", string.Join("','", reportFilters.BranchFilter).ToString());
                branchFilter2 = string.Format(@" AND PO.BRANCH_CODE IN ('{0}')", string.Join("','", reportFilters.BranchFilter).ToString());
            }

            //****************************
            //CONDITIONS FITLER END HERE
            //****************************


            Query = $@"SELECT D.ITEM_CODE,D.ITEM_DESC, D.DIRECT_PURCHASE_RATE,S.SAUDA_PURCHASE_RATE FROM
                                (SELECT PI.ITEM_CODE,IM.ITEM_EDESC ITEM_DESC, ROUND(NVL(AVG(PI.UNIT_PRICE),0),2) DIRECT_PURCHASE_RATE
                                FROM IP_PURCHASE_INVOICE PI,IP_ITEM_MASTER_SETUP IM
                                WHERE 
                                PI.ITEM_CODE = IM.ITEM_CODE
                                AND PI.COMPANY_CODE = IM.COMPANY_CODE
                                AND PI.COMPANY_CODE IN({companyCode})  {branchFilter1}
                                {productFilter}
                                AND INVOICE_DATE >= TO_DATE('{dates.StartDateString}','YYYY-MM-DD') AND INVOICE_DATE <= TO_DATE('{dates.EndDateString}','YYYY-MM-DD')
                                AND PI.DELETED_FLAG = 'N'
                                AND IM.CATEGORY_CODE = 'RM'
                                GROUP BY PI.ITEM_CODE,IM.ITEM_EDESC) D,
                                (SELECT PO.ITEM_CODE,IM.ITEM_EDESC AS ITEM_DESC, ROUND(NVL(AVG(PO.UNIT_PRICE),0),2) SAUDA_PURCHASE_RATE
                                FROM IP_PURCHASE_ORDER PO,IP_ITEM_MASTER_SETUP IM
                                WHERE PO.ITEM_CODE = IM.ITEM_CODE
                                AND PO.COMPANY_CODE = IM.COMPANY_CODE
                                AND PO.COMPANY_CODE IN({companyCode}) {branchFilter2}
                                {productFilter}
                                AND ORDER_DATE >= TO_DATE('{dates.StartDateString}','YYYY-MM-DD') AND ORDER_DATE <= TO_DATE('{dates.EndDateString}','YYYY-MM-DD')
                                AND PO.DELETED_FLAG = 'N'
                                AND IM.CATEGORY_CODE = 'RM'
                                GROUP BY PO.ITEM_CODE,IM.ITEM_EDESC) S
                                WHERE D.ITEM_CODE = S.ITEM_CODE
                                ORDER BY ITEM_DESC";




            Query = string.Format(Query, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode);
            var datas = _objectEntity.SqlQuery<SaudaModel>(Query).ToList();
            return datas;
        }

        public List<BranchWiseSalesFiscalYearGraph> GetSalesBranchWiseFiscalYear(ReportFiltersModel reportFilters, User userInfo)
        {


            var fiscalYear = System.Configuration.ConfigurationManager.AppSettings["FiscalYear"].Substring(2);
            var companyCode = string.Join(",", reportFilters.CompanyFilter);
            companyCode = companyCode == "" ? userInfo.company_code : companyCode;

            string Query = $@"select  
                                nvl(sum(nvl(SI.calc_total_price,0))/{ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter)},0)as amount,
                               sum(nvl(SI.calc_Quantity,0))/'1' as quantity,
                               SI.BRANCH_CODE ,BS.BRANCH_EDESC AS BranchName,
                              CS.ABBR_CODE ||' { fiscalYear }' AS FiscalYear ,'dbName' as DBName
                               from sa_sales_invoice SI,COMPANY_SETUP CS,fa_branch_setup BS
                                where SI.COMPANY_CODE = CS.COMPANY_CODE
                                AND SI.COMPANY_CODE = BS.COMPANY_CODE
                                AND SI.BRANCH_CODE = BS.BRANCH_CODE
                                AND SI.deleted_flag='N' AND CS.DELETED_FLAG  = 'N'      
                                AND SI.COMPANY_CODE IN ({ companyCode })";




            //****************************
            //CONDITIONS FITLER START HERE
            //****************************

            //for customer Filter
            var customerFilter = string.Empty;
            if (reportFilters.CustomerFilter.Count() > 0)
            {
                customerFilter = @"select  DISTINCT(customer_code) from sa_customer_setup where (";
                //IF CUSTOMER_SKU_FLAG = G
                foreach (var item in reportFilters.CustomerFilter)
                {
                    customerFilter += $"master_customer_code like  (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{ item }' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN({ companyCode })) OR ";
                }
                customerFilter = customerFilter.Substring(0, customerFilter.Length - 3);
                //IF CUSTOMER_SKU_FLAG = I                
                customerFilter += $" or (customer_code in ({ string.Join(",", reportFilters.CustomerFilter) }) and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN({ companyCode }))) ";


                Query += $" and SI.customer_code IN({ customerFilter })";
            }




            ////for Product Filter
            var productFilter = string.Empty;
            if (reportFilters.ProductFilter.Count() > 0)
            {
                productFilter = @"select  DISTINCT item_code from IP_ITEM_MASTER_SETUP where (";
                //IF PRODUCT_SKU_FLAG = G
                foreach (var item in reportFilters.ProductFilter)
                {
                    productFilter += "MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                productFilter = productFilter.Substring(0, productFilter.Length - 3);
                //IF PRODUCT_SKU_FLAG = I                
                productFilter += " or (ITEM_CODE in (" + string.Join(",", reportFilters.ProductFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                Query += " and SI.ITEM_CODE IN(" + productFilter + ")";
            }


            if (reportFilters.BranchFilter.Count > 0)
            {
                Query += string.Format(@" AND SI.BRANCH_CODE IN ('{0}')", string.Join("','", reportFilters.BranchFilter).ToString());
            }
            if (reportFilters.EmployeeFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SI.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", reportFilters.EmployeeFilter).ToString());
            }
            if (reportFilters.AgentFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND  SI.AGENT_CODE IN  ('{0}')", string.Join("','", reportFilters.AgentFilter).ToString());
            }
            if (reportFilters.DivisionFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND  SI.DIVISION_CODE IN  ('{0}')", string.Join("','", reportFilters.DivisionFilter).ToString());
            }
            string locationFilter = string.Empty;
            if (reportFilters.LocationFilter.Count > 0)
            {

                var locations = reportFilters.LocationFilter;
                for (int i = 0; i < locations.Count; i++)
                {

                    if (i == 0)
                        locationFilter += string.Format("SELECT LOCATION_CODE FROM IP_LOCATION_SETUP WHERE LOCATION_CODE LIKE '{0}%' ", locations[i]);
                    else
                    {
                        locationFilter += string.Format(" OR LOCATION_CODE like '{0}%' ", locations[i]);
                    }
                }
                locationFilter = string.Format(@" AND SI.FROM_LOCATION_CODE IN ({0} OR LOCATION_CODE IN ('{1}'))", locationFilter, string.Join("','", locations));
                //query = query.AppendFormat(@" AND A.FROM_LOCATION_CODE IN ('{0}')", string.Join("','", filters.LocationFilter).ToString());
                Query = Query + locationFilter;
            }
            //****************************
            //CONDITIONS FITLER END HERE
            //****************************


            Query += @" 
                            group by  SI.BRANCH_CODE ,BS.BRANCH_EDESC,CS.ABBR_CODE";



            if (reportFilters.FiscalYearFilter.Count() > 0)
            {
                var oldQuery = Query;
                List<string> temp = new List<string>();
                foreach (var item in reportFilters.FiscalYearFilter)
                {
                    Query = oldQuery.Replace("sa_sales_invoice", item.DBName + ".sa_sales_invoice")
                                    .Replace("COMPANY_SETUP", item.DBName + ".COMPANY_SETUP")
                                    .Replace("fa_branch_setup", item.DBName + ".fa_branch_setup");
                    Query = Query.Replace(fiscalYear, item.FiscalYear);
                    Query = Query.Replace("dbName", item.DBName);
                    temp.Add(Query);
                }
                Query = string.Join(@"
                                UNION
                                ", temp);
            }


            Query = string.Format(Query, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode);
            var datas = _objectEntity.SqlQuery<BranchWiseSalesFiscalYearGraph>(Query).ToList();
            return datas;
        }

        public List<MonthlySalesFiscalYearGraph> GetSalesDailyFiscal(ReportFiltersModel reportFilters, User userInfo, string month, string dbName, string DateFormat)
        {
            var fiscalYear = System.Configuration.ConfigurationManager.AppSettings["FiscalYear"].Substring(2);
            var companyCode = string.Join(",", reportFilters.CompanyFilter);
            companyCode = companyCode == "" ? userInfo.company_code : companyCode;

            //to display from all fiscal year, remove year in month
            var monthFilter = string.Empty;
            if (reportFilters.FiscalYearFilter.Count() > 0)
            {
                if (DateFormat == "BS")
                    monthFilter = $" AND substr(bs_date(SI.sales_date), 6, 2) = '{ month.Substring(5) }' ";
                else
                    monthFilter = $" AND to_char(SI.sales_date, 'MM')  = '{ month.Substring(4) }' ";
            }
            else
            {
                if (DateFormat == "BS")
                    monthFilter = $" AND substr(bs_date(SI.sales_date),1,7) = '{ month }' ";
                else
                    monthFilter = $" AND to_char(SI.sales_date, 'YYYYMM')  = '{ month }' ";
            }

            string Query = string.Empty;
            if (string.Equals(DateFormat.ToLower(), "ad"))
            {
                Query = $@"select to_char(SI.sales_date, 'Month') as month,to_char(SI.sales_date, 'DD') as MonthInt, nvl(sum(nvl(SI.calc_total_price,0))/{ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter)},0)as amount,sum(nvl(SI.calc_Quantity,0))/'{ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter)}' as quantity,to_char(SI.sales_date, 'Mon-YYY') as DisplayMonth 
                          ,to_char(SI.sales_date, 'DD') as Day,CS.ABBR_CODE ||' {fiscalYear}' AS FiscalYear ,'dbName' as DBName
                           from sa_sales_invoice SI,COMPANY_SETUP CS
                            where SI.COMPANY_CODE = CS.COMPANY_CODE
                            AND SI.deleted_flag='N' AND CS.DELETED_FLAG  = 'N'   
                            { monthFilter }
                            AND SI.COMPANY_CODE IN ({ companyCode })";
            }
            else
            {
                Query = $@"select trim(substr(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SI.sales_date),6,2)),5,20))  DisplayMonth, nvl(sum(nvl(SI.calc_total_price,0))/{ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter)},0)as amount,sum(nvl(SI.calc_Quantity,0))/'{ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter)}' as quantity,substr(bs_date(SI.sales_date),9,2) as MonthInt 
                         ,substr(bs_date(SI.sales_date),9,2) as Day ,CS.ABBR_CODE ||' {fiscalYear}' AS FiscalYear,'dbName' as DBName from sa_sales_invoice SI,COMPANY_SETUP CS    
                         where SI.COMPANY_CODE = CS.COMPANY_CODE
                         AND  SI.deleted_flag='N' AND CS.DELETED_FLAG  = 'N'  
                          { monthFilter }
                         AND SI.COMPANY_CODE IN ({ companyCode })";
            }


            //****************************
            //CONDITIONS FITLER START HERE
            //****************************

            //for customer Filter
            var customerFilter = string.Empty;
            if (reportFilters.CustomerFilter.Count() > 0)
            {
                customerFilter = @"select  DISTINCT(customer_code) from sa_customer_setup where (";
                //IF CUSTOMER_SKU_FLAG = G
                foreach (var item in reportFilters.CustomerFilter)
                {
                    customerFilter += "master_customer_code like  (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                customerFilter = customerFilter.Substring(0, customerFilter.Length - 3);
                //IF CUSTOMER_SKU_FLAG = I                
                customerFilter += " or (customer_code in (" + string.Join(",", reportFilters.CustomerFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                Query += " and SI.customer_code IN(" + customerFilter + ")";
            }




            ////for Product Filter
            var productFilter = string.Empty;
            if (reportFilters.ProductFilter.Count() > 0)
            {
                productFilter = @"select  DISTINCT item_code from IP_ITEM_MASTER_SETUP where (";
                //IF PRODUCT_SKU_FLAG = G
                foreach (var item in reportFilters.ProductFilter)
                {
                    productFilter += "MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                productFilter = productFilter.Substring(0, productFilter.Length - 3);
                //IF PRODUCT_SKU_FLAG = I                
                productFilter += " or (ITEM_CODE in (" + string.Join(",", reportFilters.ProductFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                Query += " and SI.ITEM_CODE IN(" + productFilter + ")";
            }


            if (reportFilters.BranchFilter.Count > 0)
            {
                Query += string.Format(@" AND SI.BRANCH_CODE IN ('{0}')", string.Join("','", reportFilters.BranchFilter).ToString());
            }
            if (reportFilters.DivisionFilter.Count > 0)
            {
                Query += string.Format(@" AND SI.DIVISION_CODE IN ('{0}')", string.Join("','", reportFilters.DivisionFilter).ToString());
            }

            //****************************
            //CONDITIONS FITLER END HERE
            //****************************

            if (string.Equals(DateFormat.ToLower(), "ad"))
            {
                Query += @" 
                            group by  to_char(SI.sales_date, 'Month'),to_char(SI.sales_date, 'YYYYMM'),to_char(SI.sales_date, 'DD'),to_char(SI.sales_date, 'Mon-YYY'),CS.ABBR_CODE
                           -- order by to_char(SI.sales_date, 'YYYYMM')";
            }
            else
            {
                Query += @" 
                            group by  FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SI.Sales_date),6,2)),substr(bs_date(SI.sales_date),9,2),CS.ABBR_CODE
                           -- ORDER BY substr(bs_date(SI.sales_date),1,7) ";
            }

            //TEMP

            // reportFilters.FiscalYearFilter.Add(new FiscalYearFilter() { DBName = "LIFECOM73", FiscalYear = "73/74" });
            // reportFilters.FiscalYearFilter.Add(new FiscalYearFilter() { DBName = "GLOBALDEC_7374", FiscalYear = "72/73" });


            if (reportFilters.FiscalYearFilter.Count() > 0)
            {
                var oldQuery = Query;
                List<string> temp = new List<string>();
                foreach (var item in reportFilters.FiscalYearFilter)
                {
                    Query = oldQuery.Replace("sa_sales_invoice", item.DBName + ".sa_sales_invoice").Replace("COMPANY_SETUP", item.DBName + ".COMPANY_SETUP");
                    Query = Query.Replace(fiscalYear, item.FiscalYear);
                    Query = Query.Replace("dbName", item.DBName);
                    temp.Add(Query);
                }
                Query = string.Join(@"
                                UNION
                                ", temp);
            }


            Query = string.Format(Query, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode);
            var datas = _objectEntity.SqlQuery<MonthlySalesFiscalYearGraph>(Query).ToList();
            return datas;
        }

        public List<MonthlySalesGraph> GetSalesMonthSummanryMobile(ReportFiltersModel reportFilters, User userInfo, string dateFormat, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode,string divisionCode, string formCode)
        {


            companyCode = string.Join(",", reportFilters.CompanyFilter);
            companyCode = companyCode == "" ? userInfo.company_code : companyCode;

            //****************************
            //CONDITIONS FITLER START HERE
            //****************************

            var filterQuery = string.Empty;

            //for customer Filter
            var customerFilter = string.Empty;
            if (reportFilters.CustomerFilter.Count() > 0)
            {
                customerFilter = @"select  DISTINCT(customer_code) from sa_customer_setup where (";
                //IF CUSTOMER_SKU_FLAG = G
                foreach (var item in reportFilters.CustomerFilter)
                {
                    customerFilter += "master_customer_code like  (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                customerFilter = customerFilter.Substring(0, customerFilter.Length - 3);
                //IF CUSTOMER_SKU_FLAG = I                
                customerFilter += " or (customer_code in (" + string.Join(",", reportFilters.CustomerFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                filterQuery += " and SI.customer_code IN(" + customerFilter + ")";


            }




            ////for Product Filter
            var productFilter = string.Empty;
            if (reportFilters.ProductFilter.Count() > 0)
            {
                productFilter = @"select  DISTINCT item_code from IP_ITEM_MASTER_SETUP where (";
                //IF PRODUCT_SKU_FLAG = G
                foreach (var item in reportFilters.ProductFilter)
                {
                    productFilter += "MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                productFilter = productFilter.Substring(0, productFilter.Length - 3);
                //IF PRODUCT_SKU_FLAG = I                
                productFilter += " or (ITEM_CODE in (" + string.Join(",", reportFilters.ProductFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                filterQuery += " and SI.ITEM_CODE IN(" + productFilter + ")";
            }

            if (branchCode != null || branchCode != "")
            {
                filterQuery += string.Format(@" AND SI.BRANCH_CODE IN ({0})", branchCode).ToString();
            }

            if (reportFilters.BranchFilter.Count > 0)
            {
                filterQuery += string.Format(@" AND SI.BRANCH_CODE IN ('{0}')", string.Join("','", reportFilters.BranchFilter).ToString());
            }
            if (reportFilters.EmployeeFilter.Count > 0)
            {
                filterQuery = filterQuery + string.Format(@" AND SI.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", reportFilters.EmployeeFilter).ToString());
            }
            if (reportFilters.AgentFilter.Count > 0)
            {
                filterQuery = filterQuery + string.Format(@" AND SI.AGENT_CODE IN  ('{0}')", string.Join("','", reportFilters.AgentFilter).ToString());
            }
            if (!string.IsNullOrEmpty(divisionCode))
            {
                filterQuery = filterQuery + string.Format(@" AND SI.DIVISION_CODE IN  ('{0}')", string.Join("','", divisionCode));
            }




            //****************************\\
            // CONDITIONS FITLER END HERE \\
            //****************************\\




            string Query = string.Empty;
            if (string.Equals(dateFormat.ToLower(), "ad"))
            {
                Query = @"select  to_char(SI.sales_date, 'Month') as month,
                                                     to_char(SI.sales_date, 'YYYYMM') as NepaliMonthInt,
                                                     to_char(SI.sales_date, 'Mon-YYY') as MonthYear, 
                                                     nvl(sum(nvl(SI.NET_SALES,0))/{0},0)as NETAmount,
                                                     nvl(sum(nvl(SI.total_sales,0))/{0},0)as Amount,                                  
                                                     sum(nvl(SI.quantity,0))/{0} as Quantity 
                                                 from V$SALES_INVOICE_REPORT3 SI
                                                 where SI.deleted_flag='N'
                                                 and SI.TABLE_NAME = 'SALES'
                                                 AND SI.COMPANY_CODE IN ({1}) {2}
                                                    group by  to_char(SI.sales_date, 'Month'),to_char(SI.sales_date, 'YYYYMM'),to_char(SI.sales_date, 'Mon-YYY')
                                                    order by to_char(SI.sales_date, 'YYYYMM') ";

                //Query = @"SELECT T1.month,T1.NepaliMonthInt, T1.MonthYear, T1.Amount,T2.NetAmount,T1.quantity FROM 
                //           (select to_char(SI.sales_date, 'Month') as month,
                //                                     to_char(SI.sales_date, 'YYYYMM') as NepaliMonthInt,
                //                                     to_char(SI.sales_date, 'Mon-YYY') as MonthYear, 
                //                                     nvl(sum(nvl(SI.calc_total_price,0))/{0},0)as Amount,                                  
                //                                     sum(nvl(SI.calc_Quantity,0))/{0} as Quantity         
                //                                     from sa_sales_invoice SI
                //                                     where   SI.deleted_flag='N'
                //                                        AND SI.COMPANY_CODE IN ({1}) {2}
                //                                        group by  to_char(SI.sales_date, 'Month'),to_char(SI.sales_date, 'YYYYMM'),to_char(SI.sales_date, 'Mon-YYY')
                //                                    order by to_char(SI.sales_date, 'YYYYMM')) T1
                //           LEFT JOIN 
                //           (select  to_char(SI.sales_date, 'Month') as month,
                //                                     to_char(SI.sales_date, 'YYYYMM') as NepaliMonthInt,
                //                                     to_char(SI.sales_date, 'Mon-YYY') as MonthYear, 
                //                                     nvl(sum(nvl(SI.NET_SALES,0))/{0},0)as NETAmount                                                          
                //                                 from V$SALES_INVOICE_REPORT3 SI
                //                                 where SI.deleted_flag='N'
                //                                 and SI.TABLE_NAME = 'SALES'
                //                                 AND SI.COMPANY_CODE IN ({1}) {2}
                //                                    group by  to_char(SI.sales_date, 'Month'),to_char(SI.sales_date, 'YYYYMM'),to_char(SI.sales_date, 'Mon-YYY')
                //                                    order by to_char(SI.sales_date, 'YYYYMM')) T2 
                //          ON T1.NepaliMonthInt = T2.NepaliMonthInt ";
            }
            else
            {
                Query = @"select NVL(SUM (NVL (SI.total_sales, 0)) / 1, 0) AS amount,
                        SUM (NVL(SI.quantity, 0)) / 1 AS quantity,
                        SUBSTR (bs_date (SI.sales_date), 6, 2) AS NepaliMonthInt,
                        TRIM (SUBSTR (FN_CHARTBS_MONTH (SUBSTR (BS_DATE (SI.sales_date), 6, 2)),5,20))Nepalimonth,sum(nvl(SI.NET_SALES,0))/{0} as NetAmount
                        from V$SALES_INVOICE_REPORT3 SI
                         where SI.deleted_flag='N'
                         and SI.TABLE_NAME = 'SALES'
                        AND SI.COMPANY_CODE IN ({1}) {2}
                            group by  FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SI.Sales_date),6,2)),substr(bs_date(SI.sales_date),6,2)
                            ORDER BY substr(bs_date(SI.sales_date),6,2)";

                //Query = @"SELECT T1.Nepalimonth,T1.NepaliMonthInt, T1.amount,T2.NetAmount,T1.quantity FROM 
                //              (select trim(substr(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SI.sales_date),6,2)),5,20)) Nepalimonth, 
                //nvl(sum(nvl(SI.calc_total_price,0))/{0},0)as amount,sum(nvl(SI.calc_Quantity,0))/{0} as quantity,substr(bs_date(SI.sales_date),6,2) as NepaliMonthInt 
                //         from sa_sales_invoice SI                         
                //         where SI.deleted_flag='N'
                //         AND SI.COMPANY_CODE IN ({1}) {2}
                //            group by  FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SI.Sales_date),6,2)),substr(bs_date(SI.sales_date),6,2) 
                //            ORDER BY substr(bs_date(SI.sales_date),6,2) ) T1
                //            LEFT JOIN                                                        
                //            (select trim(substr(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SI.sales_date),6,2)),5,20))  Nepalimonth, sum(nvl(SI.NET_SALES,0))/{0} as NetAmount
                //         from V$SALES_INVOICE_REPORT3 SI
                //         where SI.deleted_flag='N'
                //         and SI.TABLE_NAME = 'SALES'
                //         AND SI.COMPANY_CODE IN ({1}) {2}
                //            group by  FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SI.Sales_date),6,2)),substr(bs_date(SI.sales_date),6,2)
                //            ORDER BY substr(bs_date(SI.sales_date),6,2)  ) T2
                //            ON T1.Nepalimonth = T2.Nepalimonth";
            }


            Query = string.Format(Query, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode, filterQuery);
            var datas = _objectEntity.SqlQuery<MonthlySalesGraph>(Query).ToList();
            return datas;

        }

        public List<MonthlySalesGraph> GetSalesMonthSummanry(ReportFiltersModel reportFilters, string dateFormat, User userInfo, bool salesReturn)
        {


            //var companyCode = string.Join(",'", reportFilters.CompanyFilter);
            var companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);

            //****************************
            //CONDITIONS FITLER START HERE
            //****************************

            var filterQuery = string.Empty;

            //for sales return
            if (salesReturn)
                filterQuery += " and SI.TABLE_NAME IN('SALES','SALES_RETURN')";
            else
                filterQuery += " and SI.TABLE_NAME IN('SALES')";


            //for customer Filter
            var customerFilter = string.Empty;
            if (reportFilters.CustomerFilter.Count() > 0)
            {
                customerFilter = @"select  DISTINCT(customer_code) from sa_customer_setup where (";
                //IF CUSTOMER_SKU_FLAG = G
                foreach (var item in reportFilters.CustomerFilter)
                {
                    customerFilter += "master_customer_code like  (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                customerFilter = customerFilter.Substring(0, customerFilter.Length - 3);
                //IF CUSTOMER_SKU_FLAG = I                
                customerFilter += " or (customer_code in ('" + string.Join("','", reportFilters.CustomerFilter) + "') and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                filterQuery += " and SI.customer_code IN(" + customerFilter + ")";
            }




            ////for Product Filter
            var productFilter = string.Empty;
            if (reportFilters.ProductFilter.Count() > 0)
            {
                productFilter = @"select  DISTINCT item_code from IP_ITEM_MASTER_SETUP where (";
                //IF PRODUCT_SKU_FLAG = G
                foreach (var item in reportFilters.ProductFilter)
                {
                    productFilter += "MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                productFilter = productFilter.Substring(0, productFilter.Length - 3);
                //IF PRODUCT_SKU_FLAG = I                
                productFilter += " or (ITEM_CODE in (" + string.Join(",", reportFilters.ProductFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                filterQuery += " and SI.ITEM_CODE IN(" + productFilter + ")";
            }


            if (reportFilters.BranchFilter.Count > 0)
            {
                filterQuery += string.Format(@" AND SI.BRANCH_CODE IN ('{0}')", string.Join("','", reportFilters.BranchFilter).ToString());
            }

            if (reportFilters.EmployeeFilter.Count > 0)
            {
                filterQuery = filterQuery + string.Format(@" AND SI.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", reportFilters.EmployeeFilter).ToString());
            }
            if (reportFilters.AgentFilter.Count > 0)
            {
                filterQuery = filterQuery + string.Format(@" AND SI.AGENT_CODE IN  ('{0}')", string.Join("','", reportFilters.AgentFilter).ToString());
            }




            //****************************\\
            // CONDITIONS FITLER END HERE \\
            //****************************\\




            string Query = string.Empty;
            if (string.Equals(dateFormat.ToLower(), "ad"))
            {
                Query = @"select  to_char(SI.sales_date, 'Month') as Month,
                                    to_char(SI.sales_date, 'YYYYMM') as NepaliMonthInt,
                                    to_char(SI.sales_date, 'Mon-YYY') as MonthYear, 
                                    nvl(sum(nvl(SI.TOTAL_SALES,0))/{0},0)as TotalAmount,  
                                    nvl(sum(nvl(SI.GROSS_SALES,0))/{0},0)as GrossAmount,                           
                                    sum(nvl(SI.Quantity,0))/{0} as Quantity,    
                                    nvl(sum(nvl(SI.NET_SALES,0))/{0},0)as NETAmount                                                          
                                from V$SALES_INVOICE_REPORT3 SI
                                where SI.deleted_flag='N'                                
                                AND SI.COMPANY_CODE IN ({1}) {2} 
                                group by  to_char(SI.sales_date, 'Month'),to_char(SI.sales_date, 'YYYYMM'),to_char(SI.sales_date, 'Mon-YYY')
                                order by to_char(SI.sales_date, 'YYYYMM')";
            }
            else
            {
                Query = @"select trim(substr(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SI.sales_date),6,2)),5,20))  MonthYear, 
                         substr(bs_date(SI.sales_date),6,2) as NepaliMonthInt,
                          nvl(sum(nvl(SI.TOTAL_SALES,0))/{0},0)as TotalAmount,  
                          nvl(sum(nvl(SI.GROSS_SALES,0))/{0},0)as GrossAmount,                           
                          sum(nvl(SI.Quantity,0))/{0} as Quantity,    
                          nvl(sum(nvl(SI.NET_SALES,0))/{0},0)as NETAmount     
                         from V$SALES_INVOICE_REPORT3 SI
                         where SI.deleted_flag='N'                         
                         AND SI.COMPANY_CODE IN ({1}) {2}
                            group by  FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SI.Sales_date),6,2)),substr(bs_date(SI.sales_date),6,2)
                            ORDER BY substr(bs_date(SI.sales_date),6,2) ";
            }


            Query = string.Format(Query, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode, filterQuery);
            var datas = _objectEntity.SqlQuery<MonthlySalesGraph>(Query).ToList();
            return datas;

        }

        public List<MonthlyBranchSalesGraph> GetMonthlyBranchSalesSummary(ReportFiltersModel reportFilters, string dateFormat, User userInfo)
        {
            if (userInfo == null)
            {
                userInfo = new Core.Domain.User();
                userInfo.company_code = "01";
                userInfo.branch_code = "01.01";

            }
            else if (string.IsNullOrEmpty(userInfo.company_code) || string.IsNullOrEmpty(userInfo.branch_code))
            {
                userInfo.company_code = "01";
                userInfo.branch_code = "01.01";
            }
            string Query = string.Empty;
            if (string.Equals(dateFormat.ToLower(), "ad"))
            {
                //Query = string.Format(@"SELECT si.branch_code,
                //                BS.BRANCH_EDESC as BranchName,
                //                 TO_CHAR (si.sales_date, 'fmMonth') AS Month,
                //                 SUM (nvl(si.calc_total_price,0))/{0} AS Amount,
                //                 SUM (nvl(si.calc_Quantity,0))/{0} AS Quantity
                //            FROM sa_sales_invoice si, fa_branch_setup bs
                //           WHERE si.deleted_flag = 'N'
                //           and SI.BRANCH_CODE = BS.BRANCH_CODE
                //            AND SI.COMPANY_CODE = '{1}'
                //        GROUP BY si.branch_code,BS.BRANCH_EDESC, TO_CHAR (si.sales_date, 'fmMonth')
                //        ", ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), userInfo.company_code);
                Query = string.Format(@"select
                    TO_CHAR (fiscal_year.start_date + ROWNUM - 1, 'fmMonth') AS Month,
                    bs.branch_code,
                    bs.BRANCH_EDESC as BranchName,
                    (SELECT nvl(SUM (nvl(si.calc_total_price,0))/{0},0) AS Amount
                    FROM sa_sales_invoice si
                        WHERE si.deleted_flag = 'N'  AND si.company_code = '{1}'
                        and SI.BRANCH_CODE = BS.BRANCH_CODE
                        and TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'fmMonth')=TO_CHAR(sales_date, 'fmMonth')) as Amount,
                    (select nvl(SUM (nvl(si.calc_Quantity,0))/{0},0) AS Quantity
                        FROM sa_sales_invoice si
                        WHERE si.deleted_flag = 'N'  AND si.company_code = '{1}'
                        and SI.BRANCH_CODE = BS.BRANCH_CODE
                        and TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'fmMonth')=TO_CHAR(sales_date, 'fmMonth')) as Quantity
                FROM all_objects, fa_branch_setup bs,
                   (SELECT start_date, end_date
                      FROM HR_FISCAL_YEAR_CODE
                     WHERE sysdate BETWEEN start_date AND end_date) fiscal_year
                WHERE ROWNUM <= fiscal_year.end_date - fiscal_year.start_date + 1 and bs.BRANCH_EDESC = 'Thimi'
                group by 
                TO_CHAR (fiscal_year.start_date + ROWNUM - 1, 'fmMonth'),
                bs.branch_code,bs.BRANCH_EDESC",
            ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), userInfo.company_code, userInfo.branch_code);

            }
            else if (string.Equals(dateFormat.ToLower(), "bs"))
            {
                Query = string.Format(@"SELECT si.branch_code,
                                BS.BRANCH_EDESC as BranchName,
                                 --TO_CHAR (si.sales_date, 'fmMonth') AS Month,
                                 fn_bs_month(substr(bs_date(si.sales_date),6,2)) AS Nepalimonth,
                                 substr(bs_date(si.sales_date),6,2) AS NepaliMonthInt,  
                                 SUM (nvl(si.calc_total_price,0))/{0} AS Amount,
                                 SUM (nvl(si.calc_Quantity,0))/{0} AS Quantity
                            FROM sa_sales_invoice si, fa_branch_setup bs
                           WHERE si.deleted_flag = 'N'
                            --AND SI.COMPANY_CODE = '{1}'
                           and SI.BRANCH_CODE = BS.BRANCH_CODE
                        GROUP BY si.branch_code,BS.BRANCH_EDESC, --TO_CHAR (si.sales_date, 'fmMonth'),
                        fn_bs_month(substr(bs_date(sales_date),6,2)),
                        substr(bs_date(sales_date),6,2) 
                        ", ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), userInfo.company_code);
            }

            var salesReportList = _objectEntity.SqlQuery<MonthlyBranchSalesGraph>(Query).ToList();
            return salesReportList;
        }


        public List<MonthlyBranchSalesGraph> GetMonthlyBranchSalesSummary(ReportFiltersModel reportFilters, User userInfo, string dateFormat, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode, bool salesReturn, string AmountType)
        {
            // old query implemented as new query took more exicution time


            companyCode = string.Join(",", reportFilters.CompanyFilter);
            companyCode = companyCode == "" ? userInfo.company_code : companyCode;
            string saleReturnQuery = salesReturn ? "'SALES','SALES_RETURN'" : "'SALES'";
            //**********************************************************************************
            //THIS QUERY IS GIVEN BY BIKALPA SIR
            string Query = string.Empty;
            string QueryBS = string.Empty;
            string QueryNew = string.Empty;
            string QueryNewBS = string.Empty;

            //Old qewry from Table
            Query = string.Format(@"select trim(substr(FN_CHARTAD_MONTH(SUBSTR(sales_date,4,3)),2,12)) MONTH,  
                          TO_CHAR(sales_date,'MON-YYYY') MONTHYear,
                           TO_CHAR(sales_date,'YYYYMM') NepaliMonthInt,                    
                          SUM (nvl(sl.calc_total_price,0))/{0} AS GrossAmount,        
                          SUM (nvl(sl.calc_quantity,0))/1 AS Quantity,
                          sl.branch_code,
                          bs.branch_edesc as BranchName 
                          from sa_sales_invoice  sl ,fa_branch_setup bs
                                            where sl.branch_code=bs.branch_code                                           
                                            and sl.deleted_flag='N'
                                            and sl.company_code IN({1})",
                                        ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode);
            QueryBS = string.Format(@"select trim(substr(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SALES_DATE),6,2)),6,12)) MONTH,      
                                        trim(substr(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SALES_DATE),6,2)),6,12)) MONTHYEAR,  
                                         SUBSTR(BS_DATE(SALES_DATE),6,2) NepaliMonthInt,
                                         SUM (nvl(sl.calc_total_price,0))/{0} AS GrossAmount,        
                                          SUM (nvl(sl.calc_quantity,0))/1 AS Quantity,
                                          sl.branch_code,
                                          bs.branch_edesc as BranchName 
                                          from sa_sales_invoice  sl ,fa_branch_setup bs
                                            where sl.branch_code=bs.branch_code     
                                             and sl.company_code=bs.company_code        
                                            and sl.deleted_flag='N'
                                            and sl.company_code IN({1})",
                                        ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode);

            //New qewry from View
            QueryNew = string.Format(@"select trim(substr(FN_CHARTAD_MONTH(SUBSTR(sales_date,4,3)),2,12)) MONTH,  
                  TO_CHAR(sales_date,'MON-YYYY') MONTHYear,
                   TO_CHAR(sales_date,'YYYYMM') NepaliMonthInt,                    
                  SUM (nvl(sl.total_sales,0))/1 AS TotalAmount,
                                  SUM (nvl(sl.gross_sales,0))/1 AS GrossAmount,
                                  SUM (nvl(sl.Net_sales,0))/1 AS NetAmount,
                                  SUM (nvl(sl.quantity,0))/1 AS Quantity,
                                  sl.branch_code,
                                  bs.branch_edesc as BranchName 
                                  from V$SALES_INVOICE_REPORT3 sl ,fa_branch_setup bs
                                    where sl.branch_code=bs.branch_code
                                     and sl.company_code=bs.company_code        
                                    and sl.TABLE_NAME IN ({2})
                                    and sl.deleted_flag='N'
                                    and sl.company_code IN({1})",
                                        ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode, saleReturnQuery);
            QueryNewBS = string.Format(@"select trim(substr(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SALES_DATE),6,2)),6,12)) MONTH,      
                                trim(substr(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SALES_DATE),6,2)),6,12)) MONTHYEAR,  
                                 SUBSTR(BS_DATE(SALES_DATE),6,2) NepaliMonthInt,
                                 SUM (nvl(sl.total_sales,0))/1 AS TotalAmount,
                                  SUM (nvl(sl.gross_sales,0))/1 AS GrossAmount,
                                  SUM (nvl(sl.Net_sales,0))/1 AS NetAmount,
                                  SUM (nvl(sl.quantity,0))/1 AS Quantity,
                                  sl.branch_code,
                                  bs.branch_edesc as BranchName 
                                  from V$SALES_INVOICE_REPORT3 sl ,fa_branch_setup bs
                                    where sl.branch_code=bs.branch_code
                                      and sl.company_code=bs.company_code        
                                    and sl.TABLE_NAME IN ({2})
                                    and sl.deleted_flag='N'
                                    and sl.company_code IN({1})",
                                        ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode, saleReturnQuery);

            //****************************
            //CONDITIONS FITLER START HERE
            //****************************
            string Condition = string.Empty;
            string ConditionNew = string.Empty;
            //for customer Filter
            var customerFilter = string.Empty;
            if (reportFilters.CustomerFilter.Count() > 0)
            {
                customerFilter = @"select  DISTINCT(customer_code) from sa_customer_setup where (";
                //IF CUSTOMER_SKU_FLAG = G
                foreach (var item in reportFilters.CustomerFilter)
                {
                    customerFilter += "master_customer_code like  (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                customerFilter = customerFilter.Substring(0, customerFilter.Length - 3);
                //IF CUSTOMER_SKU_FLAG = I                
                customerFilter += " or (customer_code in (" + string.Join(",", reportFilters.CustomerFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                Condition += " and sl.customer_code IN(" + customerFilter + ")";
                ConditionNew += " and sl.customer_code IN(" + customerFilter + ")";
            }

            //for Product Filter
            var productFilter = string.Empty;
            if (reportFilters.ProductFilter.Count() > 0)
            {
                productFilter = @"select  DISTINCT item_code from IP_ITEM_MASTER_SETUP where (";
                //IF PRODUCT_SKU_FLAG = G
                foreach (var item in reportFilters.ProductFilter)
                {
                    productFilter += "MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                productFilter = productFilter.Substring(0, productFilter.Length - 3);
                //IF PRODUCT_SKU_FLAG = I                
                productFilter += " or (ITEM_CODE in (" + string.Join(",", reportFilters.ProductFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                Condition += " and sl.ITEM_CODE IN(" + productFilter + ")";
            }


            if (reportFilters.BranchFilter.Count > 0)
            {
                Condition += string.Format(@" AND sl.BRANCH_CODE IN ('{0}')", string.Join("','", reportFilters.BranchFilter).ToString());
                ConditionNew += string.Format(@" AND sl.BRANCH_CODE IN ('{0}')", string.Join("','", reportFilters.BranchFilter).ToString());
            }
            if (reportFilters.AreaTypeFilter.Count > 0)
            {
                Condition += string.Format(@" AND sl.AREA_CODE IN ('{0}')", string.Join("','", reportFilters.AreaTypeFilter).ToString());
                ConditionNew += string.Format(@" AND sl.AREA_CODE IN ('{0}')", string.Join("','", reportFilters.AreaTypeFilter).ToString());
            }
            if (reportFilters.EmployeeFilter.Count > 0)
            {
                Condition = Condition + string.Format(@" AND sl.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", reportFilters.EmployeeFilter).ToString());
            }
            if (reportFilters.AgentFilter.Count > 0)
            {
                Condition = Condition + string.Format(@" AND sl.AGENT_CODE IN  ('{0}')", string.Join("','", reportFilters.AgentFilter).ToString());
            }
            if (reportFilters.DivisionFilter.Count > 0)
            {
                Condition = Condition + string.Format(@" AND sl.DIVISION_CODE IN  ('{0}')", string.Join("','", reportFilters.DivisionFilter).ToString());
                ConditionNew = ConditionNew + string.Format(@" AND sl.DIVISION_CODE IN  ('{0}')", string.Join("','", reportFilters.DivisionFilter).ToString());

            }
            string locationFilter = string.Empty;
            if (reportFilters.LocationFilter.Count > 0)
            {

                var locations = reportFilters.LocationFilter;
                for (int i = 0; i < locations.Count; i++)
                {

                    if (i == 0)
                        locationFilter += string.Format("SELECT LOCATION_CODE FROM IP_LOCATION_SETUP WHERE LOCATION_CODE LIKE '{0}%' ", locations[i]);
                    else
                    {
                        locationFilter += string.Format(" OR LOCATION_CODE like '{0}%' ", locations[i]);
                    }
                }
                locationFilter = string.Format(@" AND sl.FROM_LOCATION_CODE IN ({0} OR LOCATION_CODE IN ('{1}'))", locationFilter, string.Join("','", locations));
                //query = query.AppendFormat(@" AND A.FROM_LOCATION_CODE IN ('{0}')", string.Join("','", filters.LocationFilter).ToString());
                Condition = Condition + locationFilter;
            }
            //****************************
            //CONDITIONS FITLER END HERE
            //****************************
            Query += Condition + @"
                          group by sl.branch_code,bs.branch_edesc,FN_CHARTAD_MONTH(SUBSTR(sales_date,4,3)),TO_CHAR(sales_date,'MON-YYYY'),TO_CHAR(sales_date,'YYYYMM')
                          order by TO_CHAR(sales_date,'YYYYMM')";
            QueryBS += Condition + @"
                            group by  sl.branch_code,bs.branch_edesc,FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SALES_DATE),6,2)),SUBSTR(BS_DATE(SALES_DATE),6,2)
                            ORDER BY SUBSTR(BS_DATE(SALES_DATE),6,2)";

            QueryNew += ConditionNew + @"
                          group by sl.branch_code,bs.branch_edesc,FN_CHARTAD_MONTH(SUBSTR(sales_date,4,3)),TO_CHAR(sales_date,'MON-YYYY'),TO_CHAR(sales_date,'YYYYMM')
                          order by TO_CHAR(sales_date,'YYYYMM')";
            QueryNewBS += ConditionNew + @"
                            group by  sl.branch_code,bs.branch_edesc,FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SALES_DATE),6,2)),SUBSTR(BS_DATE(SALES_DATE),6,2)
                            ORDER BY SUBSTR(BS_DATE(SALES_DATE),6,2)";

            string QueryFinal = string.Empty;
            if (AmountType == "NetAmount" || AmountType == "TotalAmount")
            {
                if (string.Equals(dateFormat, "BS"))
                {
                    var salesReportList = _objectEntity.SqlQuery<MonthlyBranchSalesGraph>(QueryNewBS).ToList();
                    return salesReportList;
                }
                else
                {
                    var salesReportList = _objectEntity.SqlQuery<MonthlyBranchSalesGraph>(QueryNew).ToList();
                    return salesReportList;
                }
            }
            else
            {
                if (string.Equals(dateFormat, "BS"))
                {
                    var salesReportList = _objectEntity.SqlQuery<MonthlyBranchSalesGraph>(QueryBS).ToList();
                    return salesReportList;
                }
                else
                {
                    var salesReportList = _objectEntity.SqlQuery<MonthlyBranchSalesGraph>(Query).ToList();
                    return salesReportList;
                }
            }

            //***********************************************************************************
            //THIS QUERY IS GIVEN BY PROMOD SIR
            //string Query = string.Empty;
            //string QueryNew = string.Empty;
            //string subQueryAmount = string.Empty;
            //string subQueryQuantity = string.Empty;
            //string condition = string.Empty;
            //string QueryNewEnd = string.Empty;
            //if (string.Equals(dateFormat.ToLower(), "ad"))
            //{
            //    Query = @"SELECT si.branch_code,
            //                BS.BRANCH_EDESC as BranchName,
            //                 TO_CHAR (si.sales_date, 'fmMonth') AS Month,
            //                 SUM (nvl(si.calc_total_price,0))/'{0}' AS Amount,
            //                 SUM (nvl(si.calc_Quantity,0))/'{0}' AS Quantity
            //            FROM sa_sales_invoice si, fa_branch_setup bs,SA_CUSTOMER_SETUP CS,IP_ITEM_MASTER_SETUP IM                                  
            //           WHERE  SI.CUSTOMER_CODE = CS.CUSTOMER_CODE AND  IM.ITEM_CODE = SI.ITEM_CODE AND
            //            si.deleted_flag = 'N' ({1})
            //           and SI.BRANCH_CODE = BS.BRANCH_CODE";

            //    QueryNew = @"select (shortmonth||'-'||years) MonthYear,(years || monthint) NepaliMonthInt,Month,branch_code,branchname,amount,quantity from
            //                (
            //                select
            //                    TO_CHAR (fiscal_year.start_date + ROWNUM - 1, 'fmMonth') AS Month,
            //                    TO_CHAR (fiscal_year.start_date + ROWNUM - 1, 'yyy') years,
            //                    TO_CHAR (fiscal_year.start_date + ROWNUM - 1, 'MM') monthint,
            //                    TO_CHAR (fiscal_year.start_date + ROWNUM - 1, 'Mon') shortmonth,
            //                    bs.branch_code,
            //                    bs.BRANCH_EDESC as BranchName,";

            //    QueryNewEnd = @" FROM all_objects, fa_branch_setup bs,
            //                   (SELECT start_date, end_date
            //                      FROM HR_FISCAL_YEAR_CODE
            //                     WHERE sysdate BETWEEN start_date AND end_date) fiscal_year
            //                WHERE ROWNUM <= fiscal_year.end_date - fiscal_year.start_date + 1 
            //                {1}
            //                group by 
            //                TO_CHAR (fiscal_year.start_date + ROWNUM - 1, 'fmMonth'),
            //                bs.branch_code,bs.BRANCH_EDESC,bs.COMPANY_CODE
            //                ,TO_CHAR (fiscal_year.start_date + ROWNUM - 1, 'yyy')
            //                ,TO_CHAR (fiscal_year.start_date + ROWNUM - 1, 'MM')
            //                ,TO_CHAR (fiscal_year.start_date + ROWNUM - 1, 'Mon')
            //                ) t1 order by to_number(years||monthint)";
            //    subQueryAmount = @" (SELECT nvl(SUM (nvl(si.calc_total_price,0))/{0},0) AS Amount
            //                    FROM sa_sales_invoice si
            //                    --,SA_CUSTOMER_SETUP CS
            //                    {3}
            //                    ,IP_ITEM_MASTER_SETUP IM 
            //                        WHERE si.deleted_flag = 'N' {1}
            //                        and SI.BRANCH_CODE = BS.BRANCH_CODE
            //                        and TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'yyy')=TO_CHAR(sales_date, 'yyy')
            //                        and TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'fmMonth')=TO_CHAR(sales_date, 'fmMonth')
            //                        --and CS.BRANCH_CODE=si.branch_code
            //                        {4}
            //                        and IM.BRANCH_CODE=SI.BRANCH_CODE
            //                     ";
            //    subQueryQuantity = @"(select nvl(SUM (nvl(si.calc_Quantity,0))/{0},0) AS Quantity
            //                        FROM sa_sales_invoice si
            //                        --,SA_CUSTOMER_SETUP CS
            //                        {3}
            //                        ,IP_ITEM_MASTER_SETUP IM 
            //                        WHERE si.deleted_flag = 'N' {1}
            //                        and SI.BRANCH_CODE = BS.BRANCH_CODE
            //                        --and CS.BRANCH_CODE=si.branch_code
            //                        {4}
            //                        and IM.BRANCH_CODE=SI.BRANCH_CODE
            //                        and TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'yyy')=TO_CHAR(sales_date, 'yyy')
            //                        and TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'fmMonth')=TO_CHAR(sales_date, 'fmMonth')
            //                 ";

            //}
            //else if (string.Equals(dateFormat.ToLower(), "bs"))
            //{
            //    Query = @"SELECT si.branch_code,
            //                    BS.BRANCH_EDESC as BranchName,
            //                     --TO_CHAR (si.sales_date, 'fmMonth') AS Month,
            //                     fn_bs_month(substr(bs_date(si.sales_date),6,2)) AS Nepalimonth,
            //                     substr(bs_date(si.sales_date),2,3) || substr(bs_date(si.sales_date),6,2) AS NepaliMonthInt,  
            //                     SUM (nvl(si.calc_total_price,0))/'{0}' AS Amount,
            //                     SUM (nvl(si.calc_Quantity,0))/'{0}' AS Quantity
            //                     FROM sa_sales_invoice si, fa_branch_setup bs,SA_CUSTOMER_SETUP CS,IP_ITEM_MASTER_SETUP IM                                  
            //                     WHERE  SI.CUSTOMER_CODE = CS.CUSTOMER_CODE AND  IM.ITEM_CODE = SI.ITEM_CODE AND
            //               si.deleted_flag = 'N' {1} 
            //               and SI.BRANCH_CODE = BS.BRANCH_CODE";
            //}

            //string customer_code_join = string.Empty;
            //string customer_code_condition = string.Empty;
            ////for customer Filter
            //if (!string.IsNullOrEmpty(customerCode))
            //{
            //    Query += " and (";
            //    condition += " and (";
            //    foreach (var item in customerCode.Split(','))
            //    {
            //        Query += "cs.master_customer_code like  (Select MASTER_CUSTOMER_CODE || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND COMPANY_CODE='" + userInfo.company_code + "' ) OR ";
            //        condition += "cs.master_customer_code like  (Select MASTER_CUSTOMER_CODE || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND COMPANY_CODE='" + userInfo.company_code + "' ) OR ";
            //    }
            //    Query = Query.Substring(0, Query.Length - 3) + ") ";
            //    condition = condition.Substring(0, condition.Length - 3) + ") ";
            //    customer_code_join = ",SA_CUSTOMER_SETUP CS";
            //    customer_code_condition = "and CS.BRANCH_CODE=si.branch_code";
            //}

            ////for item Filter
            //if (!string.IsNullOrEmpty(itemCode))
            //{
            //    Query += " and (";
            //    condition += " and (";
            //    foreach (var item in itemCode.Split(','))
            //    {
            //        Query += "IM.MASTER_ITEM_CODE LIKE (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND COMPANY_CODE='" + userInfo.company_code + "') OR ";
            //        condition += "IM.MASTER_ITEM_CODE LIKE (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND COMPANY_CODE='" + userInfo.company_code + "') OR ";
            //    }
            //    Query = Query.Substring(0, Query.Length - 3) + ") ";
            //    condition = condition.Substring(0, condition.Length - 3) + ") ";
            //}


            ////for category Filter
            //if (!string.IsNullOrEmpty(categoryCode))
            //{
            //    Query += " and (";
            //    condition += " and (";
            //    foreach (var item in categoryCode.Split(','))
            //    {
            //        Query += "IM.CATEGORY_CODE = '" + item + "' OR ";
            //        condition += "IM.CATEGORY_CODE = '" + item + "' OR ";
            //    }
            //    Query = Query.Substring(0, Query.Length - 3) + ") ";
            //    condition = condition.Substring(0, condition.Length - 3) + ") ";
            //}

            //string companycondition = string.Empty;
            ////FOR COMPANY FILTER
            //if (!string.IsNullOrEmpty(companyCode))
            //{
            //    //Query += " AND SI.COMPANY_CODE IN (" + companyCode + ")";
            //    //condition += " AND SI.COMPANY_CODE IN (" + companyCode + ")";
            //    companycondition= " AND bs.COMPANY_CODE IN (" + companyCode + ")";
            //}
            //else
            //{
            //    companycondition = " AND bs.COMPANY_CODE IN (" + userInfo.company_code + ")";
            //}

            //string branchFilter = string.Empty;
            ////FOR BRANCH FILTER
            //if (reportFilters.BranchFilter.Count() > 0)
            //{
            //    Query += " AND SI.BRANCH_CODE IN (" + string.Join(",", reportFilters.BranchFilter) + ")";
            //    condition += " AND SI.BRANCH_CODE IN (" + string.Join(",", reportFilters.BranchFilter) + ")";
            //    branchFilter = " AND bs.BRANCH_CODE IN (" + string.Join(",", reportFilters.BranchFilter) + ")";
            //    try { userInfo.company_code = reportFilters.BranchFilter[0].Split('.')[0]; } catch { }

            //}            

            ////FOR partyType FILTER
            //if (!string.IsNullOrEmpty(partyTypeCode))
            //{
            //    Query += " AND SI.PARTY_TYPE_CODE IN (" + partyTypeCode + ")";
            //    condition += " AND SI.PARTY_TYPE_CODE IN (" + partyTypeCode + ")";
            //}

            ////FOR FORMCODE FILER
            //if (!string.IsNullOrEmpty(formCode))
            //{
            //    Query += " AND SI.FORM_CODE IN (" + formCode + ")";
            //    condition += " AND SI.FORM_CODE IN (" + formCode + ")";
            //}




            //if (string.Equals(dateFormat.ToLower(), "ad"))
            //{
            //    Query += @" GROUP BY si.branch_code,BS.BRANCH_EDESC, TO_CHAR (si.sales_date, 'fmMonth')";
            //    // old query implemented as new query took more execution time
            //    Query = QueryNew +
            //        subQueryAmount + condition + ") as Amount," +
            //        subQueryQuantity + condition + ") as Quantity" +
            //        QueryNewEnd;
            //}
            //else
            //{
            //    Query += @" GROUP BY si.branch_code,BS.BRANCH_EDESC,
            //            fn_bs_month(substr(bs_date(sales_date),6,2)),
            //             substr(bs_date(si.sales_date),2,3) || substr(bs_date(si.sales_date),6,2)  ";
            //}

            ////Query = string.Format(Query, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), userInfo.company_code, branchFilter, customer_code_join, customer_code_condition);
            //Query = string.Format(Query, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companycondition, branchFilter, customer_code_join, customer_code_condition);
            //var salesReportList = _objectEntity.SqlQuery<MonthlyBranchSalesGraph>(Query).ToList();
            //return salesReportList;
        }

        public List<MonthlyBranchSalesGraph> GetMonthlyBranchPurchaseGroupsummary(ReportFiltersModel reportFilters, User userInfo, string dateFormat, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode, bool salesReturn, string AmountType)
        {
            // old query implemented as new query took more exicution time


            companyCode = string.Join(",", reportFilters.CompanyFilter);
            companyCode = companyCode == "" ? userInfo.company_code : companyCode;
            string Query = string.Empty;
             Query= $@"select trim(substr(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(INVOICE_DATE),6,2)),6,12)) MONTH,      
                                        trim(substr(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(INVOICE_DATE), 6, 2)), 6, 12)) MONTHYEAR,  
                                         SUBSTR(BS_DATE(INVOICE_DATE), 6, 2) NepaliMonthInt,
                                         SUM(nvl(sl.calc_total_price, 0)) / {ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter)} AS GrossAmount,
                                            SUM (nvl(sl.calc_quantity, 0))/  {ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter)} AS Quantity,
                                                   to_char(bs.ITEM_GROUP_CODE) as Branch_Code,
                                          bs.ITEM_GROUP_NAME as BranchName 
                                          from IP_PURCHASE_INVOICE  sl ,BI_ITEM_GROUP_SETUP bs,BI_ITEM_GROUP_MAP IM
                                            where  sl.ITEM_CODE=IM.ITEM_CODE        
                                              and bs.item_group_code=im.item_group_code
                                            and sl.deleted_flag = 'N'
                                            and sl.company_code IN({companyCode})
                            group by  bs.ITEM_GROUP_CODE, bs.ITEM_GROUP_NAME,FN_CHARTBS_MONTH(SUBSTR(BS_DATE(INVOICE_DATE), 6, 2)),SUBSTR(BS_DATE(INVOICE_DATE), 6, 2)
                            ORDER BY SUBSTR(BS_DATE(INVOICE_DATE), 6, 2)";
            var salesReportList = _objectEntity.SqlQuery<MonthlyBranchSalesGraph>(Query).ToList();
            return salesReportList;

        }

        public List<MonthlyBranchSalesGraph> GetMonthlyAreaalesSummary(ReportFiltersModel reportFilters, User userInfo, string dateFormat, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode, bool salesReturn, string AmountType)
        {
            // old query implemented as new query took more exicution time


            companyCode = string.Join(",", reportFilters.CompanyFilter);
            companyCode = companyCode == "" ? userInfo.company_code : companyCode;
            string saleReturnQuery = salesReturn ? "'SALES','SALES_RETURN'" : "'SALES'";
            //**********************************************************************************
            //THIS QUERY IS GIVEN BY BIKALPA SIR
            string Query = string.Empty;
            string QueryBS = string.Empty;
            string QueryNew = string.Empty;
            string QueryNewBS = string.Empty;

            //Old qewry from Table
            Query = string.Format(@"select trim(substr(FN_CHARTAD_MONTH(SUBSTR(sales_date,4,3)),2,12)) MONTH,  
                          TO_CHAR(sales_date,'MON-YYYY') MONTHYear,
                           TO_CHAR(sales_date,'YYYYMM') NepaliMonthInt,                    
                          SUM (nvl(sl.calc_total_price,0))/{0} AS GrossAmount,        
                          SUM (nvl(sl.calc_quantity,0))/1 AS Quantity,
                          sl.branch_code,
                          bs.branch_edesc as BranchName 
                          from sa_sales_invoice  sl ,fa_branch_setup bs
                                            where sl.branch_code=bs.branch_code                                           
                                            and sl.deleted_flag='N'
                                            and sl.company_code IN({1})",
                                        ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode);
            QueryBS = string.Format(@"select to_char(trim(substr(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SALES_DATE),6,2)),6,12))) MONTH,      
                                        to_char(trim(substr(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SALES_DATE),6,2)),6,12))) MONTHYEAR,  
                                         to_char(SUBSTR(BS_DATE(SALES_DATE),6,2)) NepaliMonthInt,
                                         SUM (nvl(sl.calc_total_price,0)*sl.calc_quantity)/{0} AS GrossAmount,        
                                          SUM (nvl(sl.calc_quantity,0))/1 AS Quantity,
                                          to_char(sl.AREA_code) as branch_code,
                                          bs.AREA_EDESC as BranchName 
                                          from sa_sales_invoice  sl ,area_setup bs
                                            where  sl.company_code=bs.company_code        
                                            and sl.area_code=BS.AREA_code      
                                            and sl.deleted_flag='N'
                                            and sl.company_code IN({1})",
                                        ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode);

            //New qewry from View
            QueryNew = string.Format(@"select trim(substr(FN_CHARTAD_MONTH(SUBSTR(sales_date,4,3)),2,12)) MONTH,  
                  TO_CHAR(sales_date,'MON-YYYY') MONTHYear,
                   TO_CHAR(sales_date,'YYYYMM') NepaliMonthInt,                    
                  SUM (nvl(sl.total_sales,0))/1 AS TotalAmount,
                                  SUM (nvl(sl.gross_sales,0))/1 AS GrossAmount,
                                  SUM (nvl(sl.Net_sales,0))/1 AS NetAmount,
                                  SUM (nvl(sl.quantity,0))/1 AS Quantity,
                                  sl.branch_code,
                                  bs.branch_edesc as BranchName 
                                  from V$SALES_INVOICE_REPORT3 sl ,fa_branch_setup bs
                                    where sl.branch_code=bs.branch_code
                                     and sl.company_code=bs.company_code        
                                    and sl.TABLE_NAME IN ({2})
                                    and sl.deleted_flag='N'
                                    and sl.company_code IN({1})",
                                        ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode, saleReturnQuery);
            QueryNewBS = string.Format(@"select trim(substr(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SALES_DATE),6,2)),6,12)) MONTH,      
                                trim(substr(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SALES_DATE),6,2)),6,12)) MONTHYEAR,  
                                 SUBSTR(BS_DATE(SALES_DATE),6,2) NepaliMonthInt,
                                 SUM (nvl(sl.total_sales,0))/1 AS TotalAmount,
                                  SUM (nvl(sl.gross_sales,0))/1 AS GrossAmount,
                                  SUM (nvl(sl.Net_sales,0))/1 AS NetAmount,
                                  SUM (nvl(sl.quantity,0))/1 AS Quantity,
                                  sl.branch_code,
                                  bs.branch_edesc as BranchName 
                                  from V$SALES_INVOICE_REPORT3 sl ,fa_branch_setup bs
                                    where sl.branch_code=bs.branch_code
                                      and sl.company_code=bs.company_code        
                                    and sl.TABLE_NAME IN ({2})
                                    and sl.deleted_flag='N'
                                    and sl.company_code IN({1})",
                                        ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode, saleReturnQuery);

            //****************************
            //CONDITIONS FITLER START HERE
            //****************************
            string Condition = string.Empty;
            string ConditionNew = string.Empty;
            //for customer Filter
            var customerFilter = string.Empty;
            if (reportFilters.CustomerFilter.Count() > 0)
            {
                customerFilter = @"select  DISTINCT(customer_code) from sa_customer_setup where (";
                //IF CUSTOMER_SKU_FLAG = G
                foreach (var item in reportFilters.CustomerFilter)
                {
                    customerFilter += "master_customer_code like  (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                customerFilter = customerFilter.Substring(0, customerFilter.Length - 3);
                //IF CUSTOMER_SKU_FLAG = I                
                customerFilter += " or (customer_code in (" + string.Join(",", reportFilters.CustomerFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                Condition += " and sl.customer_code IN(" + customerFilter + ")";
                ConditionNew += " and sl.customer_code IN(" + customerFilter + ")";
            }

            //for Product Filter
            var productFilter = string.Empty;
            if (reportFilters.ProductFilter.Count() > 0)
            {
                productFilter = @"select  DISTINCT item_code from IP_ITEM_MASTER_SETUP where (";
                //IF PRODUCT_SKU_FLAG = G
                foreach (var item in reportFilters.ProductFilter)
                {
                    productFilter += "MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                productFilter = productFilter.Substring(0, productFilter.Length - 3);
                //IF PRODUCT_SKU_FLAG = I                
                productFilter += " or (ITEM_CODE in (" + string.Join(",", reportFilters.ProductFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                Condition += " and sl.ITEM_CODE IN(" + productFilter + ")";
            }


            if (reportFilters.BranchFilter.Count > 0)
            {
                Condition += string.Format(@" AND sl.BRANCH_CODE IN ('{0}')", string.Join("','", reportFilters.BranchFilter).ToString());
                ConditionNew += string.Format(@" AND sl.BRANCH_CODE IN ('{0}')", string.Join("','", reportFilters.BranchFilter).ToString());
            }
            if (reportFilters.AreaTypeFilter.Count > 0)
            {
                Condition += string.Format(@" AND sl.AREA_CODE IN ('{0}')", string.Join("','", reportFilters.AreaTypeFilter).ToString());
                ConditionNew += string.Format(@" AND sl.AREA_CODE IN ('{0}')", string.Join("','", reportFilters.AreaTypeFilter).ToString());
            }
            if (reportFilters.EmployeeFilter.Count > 0)
            {
                Condition = Condition + string.Format(@" AND sl.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", reportFilters.EmployeeFilter).ToString());
            }
            if (reportFilters.AgentFilter.Count > 0)
            {
                Condition = Condition + string.Format(@" AND sl.AGENT_CODE IN  ('{0}')", string.Join("','", reportFilters.AgentFilter).ToString());
            }
            if (reportFilters.DivisionFilter.Count > 0)
            {
                Condition = Condition + string.Format(@" AND sl.DIVISION_CODE IN  ('{0}')", string.Join("','", reportFilters.DivisionFilter).ToString());
                ConditionNew = ConditionNew + string.Format(@" AND sl.DIVISION_CODE IN  ('{0}')", string.Join("','", reportFilters.DivisionFilter).ToString());

            }
            string locationFilter = string.Empty;
            if (reportFilters.LocationFilter.Count > 0)
            {

                var locations = reportFilters.LocationFilter;
                for (int i = 0; i < locations.Count; i++)
                {

                    if (i == 0)
                        locationFilter += string.Format("SELECT LOCATION_CODE FROM IP_LOCATION_SETUP WHERE LOCATION_CODE LIKE '{0}%' ", locations[i]);
                    else
                    {
                        locationFilter += string.Format(" OR LOCATION_CODE like '{0}%' ", locations[i]);
                    }
                }
                locationFilter = string.Format(@" AND sl.FROM_LOCATION_CODE IN ({0} OR LOCATION_CODE IN ('{1}'))", locationFilter, string.Join("','", locations));
                //query = query.AppendFormat(@" AND A.FROM_LOCATION_CODE IN ('{0}')", string.Join("','", filters.LocationFilter).ToString());
                Condition = Condition + locationFilter;
            }
            //****************************
            //CONDITIONS FITLER END HERE
            //****************************
            Query += Condition + @"
                          group by sl.branch_code,bs.branch_edesc,FN_CHARTAD_MONTH(SUBSTR(sales_date,4,3)),TO_CHAR(sales_date,'MON-YYYY'),TO_CHAR(sales_date,'YYYYMM')
                          order by TO_CHAR(sales_date,'YYYYMM')";
            QueryBS += Condition + @"
                            group by  sl.AREA_code,bs.AREA_EDESC,FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SALES_DATE),6,2)),SUBSTR(BS_DATE(SALES_DATE),6,2)
                            ORDER BY SUBSTR(BS_DATE(SALES_DATE),6,2)";

            QueryNew += ConditionNew + @"
                          group by sl.branch_code,bs.branch_edesc,FN_CHARTAD_MONTH(SUBSTR(sales_date,4,3)),TO_CHAR(sales_date,'MON-YYYY'),TO_CHAR(sales_date,'YYYYMM')
                          order by TO_CHAR(sales_date,'YYYYMM')";
            QueryNewBS += ConditionNew + @"
                            group by  sl.branch_code,bs.branch_edesc,FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SALES_DATE),6,2)),SUBSTR(BS_DATE(SALES_DATE),6,2)
                            ORDER BY SUBSTR(BS_DATE(SALES_DATE),6,2)";

            string QueryFinal = string.Empty;
            if (AmountType == "NetAmount" || AmountType == "TotalAmount")
            {
                if (string.Equals(dateFormat, "BS"))
                {
                    var salesReportList = _objectEntity.SqlQuery<MonthlyBranchSalesGraph>(QueryNewBS).ToList();
                    return salesReportList;
                }
                else
                {
                    var salesReportList = _objectEntity.SqlQuery<MonthlyBranchSalesGraph>(QueryNew).ToList();
                    return salesReportList;
                }
            }
            else
            {
                if (string.Equals(dateFormat, "BS"))
                {
                    var salesReportList = _objectEntity.SqlQuery<MonthlyBranchSalesGraph>(QueryBS).ToList();
                    return salesReportList;
                }
                else
                {
                    var salesReportList = _objectEntity.SqlQuery<MonthlyBranchSalesGraph>(Query).ToList();
                    return salesReportList;
                }
            }

            
        }

        public List<MonthlyBranchSalesGraph> GetMonthlyDealersalesSummary(ReportFiltersModel reportFilters, User userInfo, string dateFormat, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode, bool salesReturn, string AmountType)
        {
            // old query implemented as new query took more exicution time


            companyCode = string.Join(",", reportFilters.CompanyFilter);
            companyCode = companyCode == "" ? userInfo.company_code : companyCode;
            string saleReturnQuery = salesReturn ? "'SALES','SALES_RETURN'" : "'SALES'";
            //**********************************************************************************
            //THIS QUERY IS GIVEN BY BIKALPA SIR
            string Query = string.Empty;
            string QueryBS = string.Empty;
            string QueryNew = string.Empty;
            string QueryNewBS = string.Empty;

            //Old qewry from Table
            Query = string.Format(@"select trim(substr(FN_CHARTAD_MONTH(SUBSTR(sales_date,4,3)),2,12)) MONTH,  
                          TO_CHAR(sales_date,'MON-YYYY') MONTHYear,
                           TO_CHAR(sales_date,'YYYYMM') NepaliMonthInt,                    
                          SUM (nvl(sl.calc_total_price,0))/{0} AS GrossAmount,        
                          SUM (nvl(sl.calc_quantity,0))/1 AS Quantity,
                          sl.branch_code,
                          bs.branch_edesc as BranchName 
                          from sa_sales_invoice  sl ,fa_branch_setup bs
                                            where sl.branch_code=bs.branch_code                                           
                                            and sl.deleted_flag='N'
                                            and sl.company_code IN({1})",
                                        ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode);
            QueryBS = string.Format(@"select to_char(trim(substr(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SALES_DATE),6,2)),6,12))) MONTH,      
                                        to_char(trim(substr(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SALES_DATE),6,2)),6,12))) MONTHYEAR,  
                                         to_char(SUBSTR(BS_DATE(SALES_DATE),6,2)) NepaliMonthInt,
                                         SUM (nvl(sl.calc_total_price,0)*sl.calc_quantity)/{0} AS GrossAmount,        
                                          SUM (nvl(sl.calc_quantity,0))/1 AS Quantity,
                                          to_char(sl.AREA_code) as branch_code,
                                          bs.AREA_EDESC as BranchName 
                                          from sa_sales_invoice  sl ,area_setup bs
                                            where  sl.company_code=bs.company_code        
                                            and sl.area_code=BS.AREA_code      
                                            and sl.deleted_flag='N'
                                            and sl.company_code IN({1})",
                                        ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode);

            //New qewry from View
            QueryNew = string.Format(@"select trim(substr(FN_CHARTAD_MONTH(SUBSTR(sales_date,4,3)),2,12)) MONTH,  
                  TO_CHAR(sales_date,'MON-YYYY') MONTHYear,
                   TO_CHAR(sales_date,'YYYYMM') NepaliMonthInt,                    
                  SUM (nvl(sl.total_sales,0))/1 AS TotalAmount,
                                  SUM (nvl(sl.gross_sales,0))/1 AS GrossAmount,
                                  SUM (nvl(sl.Net_sales,0))/1 AS NetAmount,
                                  SUM (nvl(sl.quantity,0))/1 AS Quantity,
                                  sl.branch_code,
                                  bs.branch_edesc as BranchName 
                                  from V$SALES_INVOICE_REPORT3 sl ,fa_branch_setup bs
                                    where sl.branch_code=bs.branch_code
                                     and sl.company_code=bs.company_code        
                                    and sl.TABLE_NAME IN ({2})
                                    and sl.deleted_flag='N'
                                    and sl.company_code IN({1})",
                                        ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode, saleReturnQuery);
            QueryNewBS = string.Format(@"select trim(substr(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SALES_DATE),6,2)),6,12)) MONTH,      
                                trim(substr(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SALES_DATE),6,2)),6,12)) MONTHYEAR,  
                                 SUBSTR(BS_DATE(SALES_DATE),6,2) NepaliMonthInt,
                                 SUM (nvl(sl.total_sales,0))/1 AS TotalAmount,
                                  SUM (nvl(sl.gross_sales,0))/1 AS GrossAmount,
                                  SUM (nvl(sl.Net_sales,0))/1 AS NetAmount,
                                  SUM (nvl(sl.quantity,0))/1 AS Quantity,
                                  sl.branch_code,
                                  bs.branch_edesc as BranchName 
                                  from V$SALES_INVOICE_REPORT3 sl ,fa_branch_setup bs
                                    where sl.branch_code=bs.branch_code
                                      and sl.company_code=bs.company_code        
                                    and sl.TABLE_NAME IN ({2})
                                    and sl.deleted_flag='N'
                                    and sl.company_code IN({1})",
                                        ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode, saleReturnQuery);

            //****************************
            //CONDITIONS FITLER START HERE
            //****************************
            string Condition = string.Empty;
            string ConditionNew = string.Empty;
            //for customer Filter
            var customerFilter = string.Empty;
            if (reportFilters.CustomerFilter.Count() > 0)
            {
                customerFilter = @"select  DISTINCT(customer_code) from sa_customer_setup where (";
                //IF CUSTOMER_SKU_FLAG = G
                foreach (var item in reportFilters.CustomerFilter)
                {
                    customerFilter += "master_customer_code like  (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                customerFilter = customerFilter.Substring(0, customerFilter.Length - 3);
                //IF CUSTOMER_SKU_FLAG = I                
                customerFilter += " or (customer_code in (" + string.Join(",", reportFilters.CustomerFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                Condition += " and sl.customer_code IN(" + customerFilter + ")";
                ConditionNew += " and sl.customer_code IN(" + customerFilter + ")";
            }

            //for Product Filter
            var productFilter = string.Empty;
            if (reportFilters.ProductFilter.Count() > 0)
            {
                productFilter = @"select  DISTINCT item_code from IP_ITEM_MASTER_SETUP where (";
                //IF PRODUCT_SKU_FLAG = G
                foreach (var item in reportFilters.ProductFilter)
                {
                    productFilter += "MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                productFilter = productFilter.Substring(0, productFilter.Length - 3);
                //IF PRODUCT_SKU_FLAG = I                
                productFilter += " or (ITEM_CODE in (" + string.Join(",", reportFilters.ProductFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                Condition += " and sl.ITEM_CODE IN(" + productFilter + ")";
            }


            if (reportFilters.BranchFilter.Count > 0)
            {
                Condition += string.Format(@" AND sl.BRANCH_CODE IN ('{0}')", string.Join("','", reportFilters.BranchFilter).ToString());
                ConditionNew += string.Format(@" AND sl.BRANCH_CODE IN ('{0}')", string.Join("','", reportFilters.BranchFilter).ToString());
            }
            if (reportFilters.AreaTypeFilter.Count > 0)
            {
                Condition += string.Format(@" AND sl.AREA_CODE IN ('{0}')", string.Join("','", reportFilters.AreaTypeFilter).ToString());
                ConditionNew += string.Format(@" AND sl.AREA_CODE IN ('{0}')", string.Join("','", reportFilters.AreaTypeFilter).ToString());
            }
            if (reportFilters.EmployeeFilter.Count > 0)
            {
                Condition = Condition + string.Format(@" AND sl.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", reportFilters.EmployeeFilter).ToString());
            }
            if (reportFilters.AgentFilter.Count > 0)
            {
                Condition = Condition + string.Format(@" AND sl.AGENT_CODE IN  ('{0}')", string.Join("','", reportFilters.AgentFilter).ToString());
            }
            if (reportFilters.DivisionFilter.Count > 0)
            {
                Condition = Condition + string.Format(@" AND sl.DIVISION_CODE IN  ('{0}')", string.Join("','", reportFilters.DivisionFilter).ToString());
                ConditionNew = ConditionNew + string.Format(@" AND sl.DIVISION_CODE IN  ('{0}')", string.Join("','", reportFilters.DivisionFilter).ToString());

            }
            string locationFilter = string.Empty;
            if (reportFilters.LocationFilter.Count > 0)
            {

                var locations = reportFilters.LocationFilter;
                for (int i = 0; i < locations.Count; i++)
                {

                    if (i == 0)
                        locationFilter += string.Format("SELECT LOCATION_CODE FROM IP_LOCATION_SETUP WHERE LOCATION_CODE LIKE '{0}%' ", locations[i]);
                    else
                    {
                        locationFilter += string.Format(" OR LOCATION_CODE like '{0}%' ", locations[i]);
                    }
                }
                locationFilter = string.Format(@" AND sl.FROM_LOCATION_CODE IN ({0} OR LOCATION_CODE IN ('{1}'))", locationFilter, string.Join("','", locations));
                //query = query.AppendFormat(@" AND A.FROM_LOCATION_CODE IN ('{0}')", string.Join("','", filters.LocationFilter).ToString());
                Condition = Condition + locationFilter;
            }
            //****************************
            //CONDITIONS FITLER END HERE
            //****************************
            Query += Condition + @"
                          group by sl.branch_code,bs.branch_edesc,FN_CHARTAD_MONTH(SUBSTR(sales_date,4,3)),TO_CHAR(sales_date,'MON-YYYY'),TO_CHAR(sales_date,'YYYYMM')
                          order by TO_CHAR(sales_date,'YYYYMM')";
            QueryBS += Condition + @"
                            group by  sl.AREA_code,bs.AREA_EDESC,FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SALES_DATE),6,2)),SUBSTR(BS_DATE(SALES_DATE),6,2)
                            ORDER BY SUBSTR(BS_DATE(SALES_DATE),6,2)";

            QueryNew += ConditionNew + @"
                          group by sl.branch_code,bs.branch_edesc,FN_CHARTAD_MONTH(SUBSTR(sales_date,4,3)),TO_CHAR(sales_date,'MON-YYYY'),TO_CHAR(sales_date,'YYYYMM')
                          order by TO_CHAR(sales_date,'YYYYMM')";
            QueryNewBS += ConditionNew + @"
                            group by  sl.branch_code,bs.branch_edesc,FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SALES_DATE),6,2)),SUBSTR(BS_DATE(SALES_DATE),6,2)
                            ORDER BY SUBSTR(BS_DATE(SALES_DATE),6,2)";

            string QueryFinal = string.Empty;
            if (AmountType == "NetAmount" || AmountType == "TotalAmount")
            {
                if (string.Equals(dateFormat, "BS"))
                {
                    var salesReportList = _objectEntity.SqlQuery<MonthlyBranchSalesGraph>(QueryNewBS).ToList();
                    return salesReportList;
                }
                else
                {
                    var salesReportList = _objectEntity.SqlQuery<MonthlyBranchSalesGraph>(QueryNew).ToList();
                    return salesReportList;
                }
            }
            else
            {
                if (string.Equals(dateFormat, "BS"))
                {
                    var salesReportList = _objectEntity.SqlQuery<MonthlyBranchSalesGraph>(QueryBS).ToList();
                    return salesReportList;
                }
                else
                {
                    var salesReportList = _objectEntity.SqlQuery<MonthlyBranchSalesGraph>(Query).ToList();
                    return salesReportList;
                }
            }


        }

        public List<MonthlyBranchSalesBill> GetMonthlyBranchBillCount(ReportFiltersModel reportFilters, User userInfo, string DateFormat = "BS")
        {
            // old query implemented as new query took more exicution time


            var companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            var query = string.Empty;
            if (DateFormat == "BS")
            {
                query = $@"select trim(substr(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SALES_DATE),6,2)),6,12)) MONTH,      
                                        trim(substr(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SALES_DATE),6,2)),6,12)) MONTHYEAR,  
                                         SUBSTR(BS_DATE(SALES_DATE),6,2) NepaliMonthInt,
                                         count (nvl(sl.sales_no,0)) AS BillCount,    
                                          sl.branch_code,
                                          bs.branch_edesc as BranchName 
                                          from sa_sales_invoice  sl ,fa_branch_setup bs
                                            where sl.branch_code=bs.branch_code         
                                            and sl.company_code=bs.company_code                                                                              
                                            and sl.deleted_flag='N'
                                            and sl.company_code IN({companyCode})
                            group by  sl.branch_code,bs.branch_edesc,
                            FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SALES_DATE),6,2)),SUBSTR(BS_DATE(SALES_DATE),6,2)
                            ORDER BY SUBSTR(BS_DATE(SALES_DATE),6,2)";
            }
            else
            {
                query = $@"select  to_char(sl.SALES_DATE,'MONTH') MONTH,      
                                         to_char(sl.SALES_DATE,'MMYYYY') MONTHYEAR,  
                                           to_char(sl.SALES_DATE,'MM') NepaliMonthInt,
                                         count (nvl(sl.sales_no,0)) AS BillCount,    
                                          sl.branch_code,
                                          bs.branch_edesc as BranchName 
                                          from sa_sales_invoice  sl ,fa_branch_setup bs
                                            where sl.branch_code=bs.branch_code         
                                            and sl.company_code=bs.company_code                                                                              
                                            and sl.deleted_flag='N'
                                            and sl.company_code IN({companyCode})
                            group by  sl.branch_code,bs.branch_edesc,
                             to_char(sl.SALES_DATE,'MONTH'),  to_char(sl.SALES_DATE,'MMYYYY'),to_char(sl.SALES_DATE,'MM')
                            ORDER BY to_char(sl.SALES_DATE,'MONTH')";
            }
            var data = _objectEntity.SqlQuery<MonthlyBranchSalesBill>(query).ToList();
            return data;




        }


        public List<GrossProfitReport> GetGPReportMonthWise(ReportFiltersModel reportFilters, User userInfo
           , string customerCode, string itemCode,
                   string categoryCode, string companyCode, string branchCode, string partyTypeCode,
                   string formCode, string DateFormat)
        {
            var sales = new List<GrossProfitReport>();
            if (userInfo == null)
            {
                userInfo = new Core.Domain.User();
                //userInfo.company_code = "01";
                userInfo.branch_code = "01.01";

            }
            else if (string.IsNullOrEmpty(userInfo.company_code) || string.IsNullOrEmpty(userInfo.branch_code))
            {
                //userInfo.company_code = "01";
                userInfo.branch_code = "01.01";
            }
            companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            try
            {
                string Query = string.Empty;
                if (string.Equals(DateFormat, "AD"))
                {
                    Query = @"SELECT MONTHS as MonthsName,
monthsyear as MonthsDisplayName,
       YEARMONTH as yearmonth,
       TOATL_SALES_AMT as TotalSalesAmount,
       LANDED_COST as TotalLandedCost,
       ROUND((((TOATL_SALES_AMT - LANDED_COST) / LANDED_COST) * 100), 2)
         TotalProfitPer
  FROM(SELECT SUM(NVL(a.QUANTITY, 0)) QTY,
                 TO_CHAR(A.SALES_DATE, 'MON')MONTHS,
                   TO_CHAR(A.SALES_DATE, 'MON') || '-' || TO_CHAR(A.SALES_DATE, 'yyy') monthsyear,
                 TO_CHAR(A.SALES_DATE, 'YYYYMM') YEARMONTH,
                 SUM(
                    NVL(
                       FN_CONVERT_CURRENCY(
                          NVL(a.TOTAL_PRICE, 0) * NVL(a.EXCHANGE_RATE, 1),
                          'NRS',
                          a.SALES_DATE),
                       0))
                    TOATL_SALES_AMT,
                 SUM(NVL(a.QUANTITY, 0) * NVL(d.LANDED_COST, 0)) LANDED_COST
            FROM SA_SALES_INVOICE a,
                 IP_ITEM_MASTER_SETUP b,
                 SA_CUSTOMER_SETUP c,
                 MP_ITEM_STD_RATE d
           WHERE     a.ITEM_CODE = b.ITEM_CODE
                 AND a.CUSTOMER_CODE = c.CUSTOMER_CODE
                 AND a.COMPANY_CODE = b.COMPANY_CODE
                 AND a.COMPANY_CODE = c.COMPANY_CODE
                 AND a.ITEM_CODE = d.ITEM_CODE(+)
                 AND a.COMPANY_CODE = d.COMPANY_CODE(+)
                 AND a.DELETED_FLAG = 'N'
                 AND a.COMPANY_CODE = '" + userInfo.company_code + @"'
        GROUP BY TO_CHAR(A.SALES_DATE, 'MON'),
                 TO_CHAR(A.SALES_DATE, 'YYYYMM'),
                  TO_CHAR(A.SALES_DATE, 'MON') || '-' || TO_CHAR(A.SALES_DATE, 'yyy')
        ORDER BY TO_CHAR(A.SALES_DATE, 'YYYYMM'))";
                }
                else if (string.Equals(DateFormat, "BS"))
                {
                    Query = @"SELECT Nepalimonth as MonthsDisplayName,to_number(NepaliMonthInt) as Monthsint,
                       TOATL_SALES_AMT as TotalSalesAmount,
                       LANDED_COST as TotalLandedCost,
                       ROUND((((TOATL_SALES_AMT - LANDED_COST) / LANDED_COST) * 100), 2)
                         TotalProfitPer
                  FROM(SELECT SUM(NVL(a.QUANTITY, 0)) QTY,
                                  bs.Nepalimonth,
                                  bs.NepaliMonthInt,
                                 SUM(
                                    NVL(
                                       FN_CONVERT_CURRENCY(
                                          NVL(a.TOTAL_PRICE, 0) * NVL(a.EXCHANGE_RATE, 1),
                                          'NRS',
                                          a.SALES_DATE),
                                       0))
                                    TOATL_SALES_AMT,
                                 SUM(NVL(a.QUANTITY, 0) * NVL(d.LANDED_COST, 0)) LANDED_COST
                            FROM SA_SALES_INVOICE a,
                                 IP_ITEM_MASTER_SETUP b,
                                 SA_CUSTOMER_SETUP c,
                                 MP_ITEM_STD_RATE d,(SELECT  BS_DATE(a) as Miti,a as AdDate,
                                                        SUBSTR(BS_DATE(a),6,2) as NepaliMonthInt,
                                                        TO_CHAR(a ,'MM') as MonthInt,
                                                        fn_bs_month(substr(bs_date(a),6,2)) as Nepalimonth,
                                                        TO_CHAR(a ,'Month') as month,  
                                                        substr(bs_date(a),0,4) as Nepaliyear,
                                                        TO_CHAR(a ,'YYYY') as Englishyear,  
                                                        trunc((a-  TO_DATE ('" + userInfo.startFiscalYear.ToString("dd-MMM-yyy") + @"', 'dd-mon-yyyy'))/7)+1  weeks 
                                                        FROM(
                                                                SELECT ROWNUM - 1 + TO_DATE ('" + userInfo.startFiscalYear.ToString("dd-MMM-yyy") + @"', 'dd-mon-yyyy') a 
                                                                FROM all_objects 
                                                                WHERE ROWNUM < TO_DATE ('" + userInfo.endfiscalyear.ToString("dd-MMM-yyy") + @"', 'dd-mon-yyyy') - TO_DATE ('" + userInfo.startFiscalYear.ToString("dd-MMM-yyy") + @"', 'dd-mon-yyyy')
                                                            + 2))  bs
                           WHERE     a.ITEM_CODE = b.ITEM_CODE
                                 AND a.CUSTOMER_CODE = c.CUSTOMER_CODE
                                 AND a.COMPANY_CODE = b.COMPANY_CODE
                                 AND a.COMPANY_CODE = c.COMPANY_CODE
                                 AND a.ITEM_CODE = d.ITEM_CODE(+)
                                 AND a.COMPANY_CODE = d.COMPANY_CODE(+)
                                 AND a.DELETED_FLAG = 'N'
                                 AND a.COMPANY_CODE = '01'
                                 and bs.AdDate=a.sales_date
                        GROUP BY  bs.NepaliMonthInt,bs.Nepalimonth
                        ORDER BY to_number(bs.NepaliMonthInt))";
                }
                sales = _objectEntity.SqlQuery<GrossProfitReport>(Query).ToList();

            }
            catch (Exception ex)
            {

            }

            return sales;
        }

        //public List<GrossProfitReport> GetGPReportBSMonthWise(ReportFiltersModel reportFilters, User userInfo)
        //{
        //    var sales = new List<GrossProfitReport>();
        //    if (userInfo == null)
        //    {
        //        userInfo = new Core.Domain.User();
        //        userInfo.company_code = "01";
        //        userInfo.branch_code = "01.01";

        //    }
        //    else if (string.IsNullOrEmpty(userInfo.company_code) || string.IsNullOrEmpty(userInfo.branch_code))
        //    {
        //        userInfo.company_code = "01";
        //        userInfo.branch_code = "01.01";
        //    }
        //    try
        //    {
        //        var Query = @"SELECT Nepalimonth as MonthsDisplayName,NepaliMonthInt as Monthsint,
        //               TOATL_SALES_AMT as TotalSalesAmount,
        //               LANDED_COST as TotalLandedCost,
        //               ROUND((((TOATL_SALES_AMT - LANDED_COST) / LANDED_COST) * 100), 2)
        //                 TotalProfitPer
        //          FROM(SELECT SUM(NVL(a.QUANTITY, 0)) QTY,
        //                          bs.Nepalimonth,
        //                          bs.NepaliMonthInt,
        //                         SUM(
        //                            NVL(
        //                               FN_CONVERT_CURRENCY(
        //                                  NVL(a.TOTAL_PRICE, 0) * NVL(a.EXCHANGE_RATE, 1),
        //                                  'NRS',
        //                                  a.SALES_DATE),
        //                               0))
        //                            TOATL_SALES_AMT,
        //                         SUM(NVL(a.QUANTITY, 0) * NVL(d.LANDED_COST, 0)) LANDED_COST
        //                    FROM SA_SALES_INVOICE a,
        //                         IP_ITEM_MASTER_SETUP b,
        //                         SA_CUSTOMER_SETUP c,
        //                         MP_ITEM_STD_RATE d,(SELECT  BS_DATE(a) as Miti,a as AdDate,
        //                                                SUBSTR(BS_DATE(a),6,2) as NepaliMonthInt,
        //                                                TO_CHAR(a ,'MM') as MonthInt,
        //                                                fn_bs_month(substr(bs_date(a),6,2)) as Nepalimonth,
        //                                                TO_CHAR(a ,'Month') as month,  
        //                                                substr(bs_date(a),0,4) as Nepaliyear,
        //                                                TO_CHAR(a ,'YYYY') as Englishyear,  
        //                                                trunc((a-  TO_DATE ('" + userInfo.startFiscalYear.ToString("dd-MMM-yyy") + @"', 'dd-mon-yyyy'))/7)+1  weeks 
        //                                                FROM(
        //                                                        SELECT ROWNUM - 1 + TO_DATE ('" + userInfo.startFiscalYear.ToString("dd-MMM-yyy") + @"', 'dd-mon-yyyy') a 
        //                                                        FROM all_objects 
        //                                                        WHERE ROWNUM < TO_DATE ('" + userInfo.endfiscalyear.ToString("dd-MMM-yyy") + @"', 'dd-mon-yyyy') - TO_DATE ('" + userInfo.startFiscalYear.ToString("dd-MMM-yyy") + @"', 'dd-mon-yyyy')
        //                                                    + 2))  bs
        //                   WHERE     a.ITEM_CODE = b.ITEM_CODE
        //                         AND a.CUSTOMER_CODE = c.CUSTOMER_CODE
        //                         AND a.COMPANY_CODE = b.COMPANY_CODE
        //                         AND a.COMPANY_CODE = c.COMPANY_CODE
        //                         AND a.ITEM_CODE = d.ITEM_CODE(+)
        //                         AND a.COMPANY_CODE = d.COMPANY_CODE(+)
        //                         AND a.DELETED_FLAG = 'N'
        //                         AND a.COMPANY_CODE = '01'
        //                         and bs.AdDate=a.sales_date
        //                GROUP BY  bs.NepaliMonthInt,bs.Nepalimonth
        //                ORDER BY to_number(bs.NepaliMonthInt))";

        //        sales = _objectEntity.SqlQuery<GrossProfitReport>(Query).ToList();

        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //    return sales;
        //}

        public List<GrossProfitReport> GetGPReportMonthCategoryWise(ReportFiltersModel reportFilters, User userInfo, string format)
        {
            var sales = new List<GrossProfitReport>();
            if (userInfo == null)
            {
                userInfo = new Core.Domain.User();
                //userInfo.company_code = "01";
                userInfo.branch_code = "01.01";

            }
            else if (string.IsNullOrEmpty(userInfo.company_code) || string.IsNullOrEmpty(userInfo.branch_code))
            {
                //userInfo.company_code = "01";
                userInfo.branch_code = "01.01";
            }
            var companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                if (company != null)
                    companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);


            try
            {
                var Query = string.Empty;
                if (format == "AD")
                {
                    Query = @"SELECT MONTHS as MonthsName,MONTHINT Monthsint,
monthsyear  as MonthsDisplayName,
       YEARMONTH  as yearmonth,
       TOATL_SALES_AMT as TotalSalesAmount,
       LANDED_COST as TotalLandedCost,
       category_code as CategoryCode,
       category_edesc as CategoryName,
      ( case when landed_cost = 0 then 0
       else
       ROUND ( ( ( (TOATL_SALES_AMT - LANDED_COST) / LANDED_COST) * 100), 2) end)
          TotalProfitPer
  FROM (  SELECT SUM (NVL (a.QUANTITY, 0)) QTY,
    TO_CHAR(A.SALES_DATE, 'MON')MONTHS,
   to_number(TO_CHAR(A.SALES_DATE, 'MM'))MONTHINT,
                   TO_CHAR(A.SALES_DATE, 'MON') || '-' || TO_CHAR(A.SALES_DATE, 'yyy') monthsyear,
                 TO_CHAR (A.SALES_DATE, 'YYYYMM') YEARMONTH,
                 b.category_code,
                 ca.category_edesc,
                 SUM (
                    NVL (
                       FN_CONVERT_CURRENCY (
                          NVL (a.TOTAL_PRICE, 0) * NVL (a.EXCHANGE_RATE, 1),
                          'NRS',
                          a.SALES_DATE),
                       0))
                    TOATL_SALES_AMT,
                 SUM (NVL (a.QUANTITY, 0) * NVL (d.LANDED_COST, 0)) LANDED_COST
            FROM SA_SALES_INVOICE a,
                 IP_ITEM_MASTER_SETUP b,
                 SA_CUSTOMER_SETUP c,
                 MP_ITEM_STD_RATE d,
                 ip_category_code ca
           WHERE  a.ITEM_CODE = b.ITEM_CODE
                 AND a.CUSTOMER_CODE = c.CUSTOMER_CODE
                 AND a.COMPANY_CODE = b.COMPANY_CODE
                 AND a.COMPANY_CODE = c.COMPANY_CODE
                 AND a.ITEM_CODE = d.ITEM_CODE(+)
                 AND a.COMPANY_CODE = d.COMPANY_CODE(+)
                 AND a.DELETED_FLAG = 'N'
                 AND a.COMPANY_CODE IN ({0})
                 AND a.BRANCH_CODE = '{1}'
                 and ca.category_code=b.category_code
        GROUP BY  b.category_code, ca.category_edesc,TO_CHAR (A.SALES_DATE, 'MON'),
                TO_CHAR(A.SALES_DATE, 'MM'),
                 TO_CHAR (A.SALES_DATE, 'YYYYMM'),  TO_CHAR(A.SALES_DATE, 'MON'),
                   TO_CHAR(A.SALES_DATE, 'MON') || '-' || TO_CHAR(A.SALES_DATE, 'yyy')
        ORDER BY TO_CHAR (A.SALES_DATE, 'YYYYMM'))";


                }
                else
                {
                    Query = @"  SELECT Nepalimonth as MonthsDisplayName,to_number(NepaliMonthInt) as Monthsint,
       TOATL_SALES_AMT as TotalSalesAmount,
       LANDED_COST as TotalLandedCost,
       category_code as CategoryCode,
       category_edesc as CategoryName,
      ( case when landed_cost = 0 then 0
       else
       ROUND ( ( ( (TOATL_SALES_AMT - LANDED_COST) / LANDED_COST) * 100), 2) end)
          TotalProfitPer
  FROM(SELECT SUM(NVL(a.QUANTITY, 0)) QTY,
                  bs.Nepalimonth,
                  bs.NepaliMonthInt,
                      b.category_code,
                 ca.category_edesc,
                 SUM(
                    NVL(
                       FN_CONVERT_CURRENCY(
                          NVL(a.TOTAL_PRICE, 0) * NVL(a.EXCHANGE_RATE, 1),
                          'NRS',
                          a.SALES_DATE),
                       0))
                    TOATL_SALES_AMT,
                 SUM(NVL(a.QUANTITY, 0) * NVL(d.LANDED_COST, 0)) LANDED_COST
            FROM SA_SALES_INVOICE a,
                 IP_ITEM_MASTER_SETUP b,
                 SA_CUSTOMER_SETUP c,
                 MP_ITEM_STD_RATE d,
                   ip_category_code ca, (SELECT  BS_DATE(a) as Miti, a as AdDate,
                                        SUBSTR(BS_DATE(a), 6, 2) as NepaliMonthInt,
                                        TO_CHAR(a, 'MM') as MonthInt,
                                        fn_bs_month(substr(bs_date(a), 6, 2)) as Nepalimonth,
                                        TO_CHAR(a, 'Month') as month,
                                        substr(bs_date(a), 0, 4) as Nepaliyear,
                                        TO_CHAR(a, 'YYYY') as Englishyear,
                                        trunc((a - TO_DATE('16-Jul-2016', 'dd-mon-yyyy')) / 7) + 1  weeks
                                        FROM(
                                                SELECT ROWNUM - 1 + TO_DATE('16-Jul-2016', 'dd-mon-yyyy') a
                                                FROM all_objects
                                                WHERE ROWNUM < TO_DATE('15-Jul-2017', 'dd-mon-yyyy') - TO_DATE('16-Jul-2016', 'dd-mon-yyyy')
                                            + 2))  bs
           WHERE     a.ITEM_CODE = b.ITEM_CODE
                 AND a.CUSTOMER_CODE = c.CUSTOMER_CODE
                 AND a.COMPANY_CODE = b.COMPANY_CODE
                 AND a.COMPANY_CODE = c.COMPANY_CODE
                 AND a.ITEM_CODE = d.ITEM_CODE(+)
                 AND a.COMPANY_CODE = d.COMPANY_CODE(+)
                 AND a.DELETED_FLAG = 'N'
                 AND a.COMPANY_CODE IN ({0})
                   and a.BRANCH_CODE = '{1}'
                  and ca.category_code=b.category_code
                 and bs.AdDate = a.sales_date
        GROUP BY b.category_code, ca.category_edesc, bs.NepaliMonthInt,bs.Nepalimonth
        ORDER BY to_number(bs.NepaliMonthInt))";
                }
                Query = string.Format(Query, companyCode, userInfo.branch_code);
                sales = _objectEntity.SqlQuery<GrossProfitReport>(Query).ToList();

            }
            catch (Exception ex)
            {

            }

            return sales;
        }

        public List<GrossProfitReport> GetGPReportDayWise(ReportFiltersModel reportFilters, User currentUserinformation, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode, string month, string dateFormat)
        {
            string branchCondition = string.Empty;
            if (!string.IsNullOrEmpty(branchCode))
            {
                branchCondition = " AND a.BRANCH_CODE IN (" + branchCode + ") ";
            }
            string Query = string.Empty;
            if (string.Equals(dateFormat, "AD"))
            {
                Query = string.Format(@"select * from (
                    SELECT MONTHS as MonthsName,
                           day,
                           YEARMONTH as yearmonth,
                           TOATL_SALES_AMT as TotalSalesAmount,
                           LANDED_COST as TotalLandedCost,
                           ROUND((((TOATL_SALES_AMT - LANDED_COST) / LANDED_COST) * 100), 2)
                             TotalProfitPer
                      FROM(SELECT SUM(NVL(a.QUANTITY/{0}, 0)) QTY,
                                     TO_CHAR(A.SALES_DATE, 'MON')MONTHS,
                                     TO_CHAR(A.SALES_DATE, 'DD')day,
                                     TO_CHAR(A.SALES_DATE, 'YYYYMM') YEARMONTH,
                                     SUM(
                                        NVL(
                                           FN_CONVERT_CURRENCY(
                                              NVL(a.TOTAL_PRICE, 0) * NVL(a.EXCHANGE_RATE, 1),
                                              'NRS',
                                              a.SALES_DATE),
                                           0))
                                        TOATL_SALES_AMT,
                                     SUM((NVL(a.QUANTITY, 0) * NVL(d.LANDED_COST, 0))/{0}) LANDED_COST
                                FROM SA_SALES_INVOICE a,
                                     IP_ITEM_MASTER_SETUP b,
                                     SA_CUSTOMER_SETUP c,
                                     MP_ITEM_STD_RATE d
                               WHERE     a.ITEM_CODE = b.ITEM_CODE
                                     AND a.CUSTOMER_CODE = c.CUSTOMER_CODE
                                     AND a.COMPANY_CODE = b.COMPANY_CODE
                                     AND a.COMPANY_CODE = c.COMPANY_CODE
                                     AND a.ITEM_CODE = d.ITEM_CODE(+)
                                     AND a.COMPANY_CODE = d.COMPANY_CODE(+)
                                     AND a.DELETED_FLAG = 'N'
                                     AND a.COMPANY_CODE = '{1}'
                                    {2}
                            GROUP BY TO_CHAR(A.SALES_DATE, 'MON'),
                                     TO_CHAR(A.SALES_DATE, 'YYYYMM'),
                                      TO_CHAR(A.SALES_DATE, 'DD')
                            ORDER BY TO_CHAR(A.SALES_DATE, 'YYYYMM')) ) t1 where Monthsname='{3}'
            ",
               ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter),
               currentUserinformation.company_code, branchCondition, month);
            }
            else if (string.Equals(dateFormat, "BS"))
            {
                Query = string.Format(@"
                    select * from (
                    SELECT MONTHS as MonthsName,
                           day,
                           YEARMONTH as yearmonth,
                           TOATL_SALES_AMT as TotalSalesAmount,
                           LANDED_COST as TotalLandedCost,
                           ROUND((((TOATL_SALES_AMT - LANDED_COST) / LANDED_COST) * 100), 2)
                             TotalProfitPer
                      FROM(SELECT SUM(NVL(a.QUANTITY/{0}, 0)) QTY,
                                     fn_bs_month(substr(bs_date(a.sales_date),6,2)) MONTHS,
                                     TO_CHAR(A.SALES_DATE, 'DD') day,
                                     TO_CHAR(A.SALES_DATE, 'YYYYMM') YEARMONTH,
                                     SUM(
                                        NVL(
                                           FN_CONVERT_CURRENCY(
                                              NVL(a.TOTAL_PRICE, 0) * NVL(a.EXCHANGE_RATE, 1),
                                              'NRS',
                                              a.SALES_DATE),
                                           0))
                                        TOATL_SALES_AMT,
                                     SUM((NVL(a.QUANTITY, 0) * NVL(d.LANDED_COST, 0))/{0}) LANDED_COST
                                FROM SA_SALES_INVOICE a,
                                     IP_ITEM_MASTER_SETUP b,
                                     SA_CUSTOMER_SETUP c,
                                     MP_ITEM_STD_RATE d
                               WHERE     a.ITEM_CODE = b.ITEM_CODE
                                     AND a.CUSTOMER_CODE = c.CUSTOMER_CODE
                                     AND a.COMPANY_CODE = b.COMPANY_CODE
                                     AND a.COMPANY_CODE = c.COMPANY_CODE
                                     AND a.ITEM_CODE = d.ITEM_CODE(+)
                                     AND a.COMPANY_CODE = d.COMPANY_CODE(+)
                                     AND a.DELETED_FLAG = 'N'
                                     AND a.COMPANY_CODE = '{1}' 
                                    {2}                                   
                            GROUP BY fn_bs_month(substr(bs_date(a.sales_date),6,2)),
                                     TO_CHAR(A.SALES_DATE, 'YYYYMM'),
                                      TO_CHAR(A.SALES_DATE, 'DD')
                            ORDER BY TO_CHAR(A.SALES_DATE, 'YYYYMM'))) t1 where Monthsname='{3}'
            
",
ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter),
               currentUserinformation.company_code, branchCondition, month);
            }
            List<GrossProfitReport> result = new List<GrossProfitReport>();
            result = this._objectEntity.SqlQuery<GrossProfitReport>(Query).ToList();
            return result;
        }
        public List<PendingVoucherModel> GetPendingVoucher()
        {
            var companyCode = string.Empty;
            companyCode = companyCode == "" ? $@"'{_workContext.CurrentUserinformation.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            string Query = $@"SELECT B.BRANCH_EDESC as BranchName,
       B.BRANCH_CODE ,
       PENDING.HEADINGS as VoucherType,
       PENDING.CNT as PendingVoucherCount
  FROM (  SELECT 'SALES INVOICE' HEADINGS, COUNT (*) CNT, BRANCH_CODE
            FROM MASTER_TRANSACTION
           WHERE POSTED_BY IS NULL
                 AND FORM_CODE IN
                        (SELECT FORM_CODE
                           FROM FORM_DETAIL_SETUP
                          WHERE TABLE_NAME IN ('SA_SALES_INVOICE')
                                AND COMPANY_CODE IN({companyCode}))
        GROUP BY BRANCH_CODE
        UNION ALL
          SELECT 'FINANCE' HEADINGS, COUNT (*) CNT, BRANCH_CODE
            FROM MASTER_TRANSACTION
           WHERE POSTED_BY IS NULL
                 AND FORM_CODE IN
                        (SELECT FORM_CODE
                           FROM FORM_DETAIL_SETUP
                          WHERE TABLE_NAME IN ('FA_DOUBLE_VOUCHER')
                                AND COMPANY_CODE IN({companyCode}))
        GROUP BY BRANCH_CODE
        UNION ALL
          SELECT 'SALES CHALAN' HEADINGS, COUNT (*) CNT, BRANCH_CODE
            FROM MASTER_TRANSACTION
           WHERE POSTED_BY IS NULL
                 AND FORM_CODE IN
                        (SELECT FORM_CODE
                           FROM FORM_DETAIL_SETUP
                          WHERE TABLE_NAME IN ('SA_SALES_CHALAN')
                                AND COMPANY_CODE IN({companyCode}))
        GROUP BY BRANCH_CODE
        UNION ALL
          SELECT 'SALES ORDER' HEADINGS, COUNT (*) CNT, BRANCH_CODE
            FROM MASTER_TRANSACTION
           WHERE POSTED_BY IS NULL
                 AND FORM_CODE IN
                        (SELECT FORM_CODE
                           FROM FORM_DETAIL_SETUP
                          WHERE TABLE_NAME IN ('SA_SALES_ORDER')
                                AND COMPANY_CODE IN({companyCode}))
        GROUP BY BRANCH_CODE
        UNION ALL
          SELECT 'SALES RETURN' HEADINGS, COUNT (*) CNT, BRANCH_CODE
            FROM MASTER_TRANSACTION
           WHERE POSTED_BY IS NULL
                 AND FORM_CODE IN
                        (SELECT FORM_CODE
                           FROM FORM_DETAIL_SETUP
                          WHERE TABLE_NAME IN ('SA_SALES_RETURN')
                                AND  COMPANY_CODE IN({companyCode}))
        GROUP BY BRANCH_CODE
        UNION ALL
          SELECT 'PURCHASE INVOICE' HEADINGS, COUNT (*) CNT, BRANCH_CODE
            FROM MASTER_TRANSACTION
           WHERE POSTED_BY IS NULL
                 AND FORM_CODE IN
                        (SELECT FORM_CODE
                           FROM FORM_DETAIL_SETUP
                          WHERE TABLE_NAME IN ('IP_PURCHASE_INVOICE')
                                AND COMPANY_CODE IN({companyCode}))
        GROUP BY BRANCH_CODE
        UNION ALL
          SELECT 'PURCHASE RECIEPT' HEADINGS, COUNT (*) CNT, BRANCH_CODE
            FROM MASTER_TRANSACTION
           WHERE POSTED_BY IS NULL
                 AND FORM_CODE IN
                        (SELECT FORM_CODE
                           FROM FORM_DETAIL_SETUP
                          WHERE TABLE_NAME IN ('IP_PURCHASE_MRR')
                                AND COMPANY_CODE IN({companyCode}))
        GROUP BY BRANCH_CODE
        UNION ALL
          SELECT 'PURCHASE ORDER' HEADINGS, COUNT (*) CNT, BRANCH_CODE
            FROM MASTER_TRANSACTION
           WHERE POSTED_BY IS NULL
                 AND FORM_CODE IN
                        (SELECT FORM_CODE
                           FROM FORM_DETAIL_SETUP
                          WHERE TABLE_NAME IN ('IP_PURCHASE_ORDER')
                                AND COMPANY_CODE IN({companyCode}))
        GROUP BY BRANCH_CODE
        UNION ALL
          SELECT 'PURCHASE RETURN' HEADINGS, COUNT (*) CNT, BRANCH_CODE
            FROM MASTER_TRANSACTION
           WHERE POSTED_BY IS NULL
                 AND FORM_CODE IN
                        (SELECT FORM_CODE
                           FROM FORM_DETAIL_SETUP
                          WHERE TABLE_NAME IN ('IP_PURCHASE_RETURN')
                                AND COMPANY_CODE IN({companyCode}))
        GROUP BY BRANCH_CODE
        UNION ALL
          SELECT 'CONSUMPTION ISSUE' HEADINGS, COUNT (*) CNT, BRANCH_CODE
            FROM MASTER_TRANSACTION
           WHERE POSTED_BY IS NULL
                 AND FORM_CODE IN
                        (SELECT FORM_CODE
                           FROM FORM_DETAIL_SETUP
                          WHERE TABLE_NAME IN ('IP_GOODS_ISSUE')
                                AND COMPANY_CODE IN({companyCode}))
        GROUP BY BRANCH_CODE
        UNION ALL
          SELECT 'PRODUCTION ISSUE' HEADINGS, COUNT (*) CNT, BRANCH_CODE
            FROM MASTER_TRANSACTION
           WHERE POSTED_BY IS NULL
                 AND FORM_CODE IN
                        (SELECT FORM_CODE
                           FROM FORM_DETAIL_SETUP
                          WHERE TABLE_NAME IN ('IP_PRODUCTION_ISSUE')
                                AND COMPANY_CODE IN({companyCode}))
        GROUP BY BRANCH_CODE
        UNION ALL
          SELECT 'PRODUCTION RECIEPT' HEADINGS, COUNT (*) CNT, BRANCH_CODE
            FROM MASTER_TRANSACTION
           WHERE POSTED_BY IS NULL
                 AND FORM_CODE IN
                        (SELECT FORM_CODE
                           FROM FORM_DETAIL_SETUP
                          WHERE TABLE_NAME IN ('IP_PRODUCTION_MRR')
                                AND COMPANY_CODE IN({companyCode}))
        GROUP BY BRANCH_CODE) PENDING,
       FA_BRANCH_SETUP B
 WHERE PENDING.BRANCH_CODE = B.BRANCH_CODE";
            var vouchers = _objectEntity.SqlQuery<PendingVoucherModel>(Query).ToList();
            return vouchers;
        }
        public List<GrossProfitReportDayWise> GetGPDayWiseItemReport(ReportFiltersModel reportFilters, User userInfo, string categoryCode, string month)
        {
            var sales = new List<GrossProfitReportDayWise>();
            if (userInfo == null)
            {
                userInfo = new Core.Domain.User();
                userInfo.company_code = "01";
                userInfo.branch_code = "01.01";

            }
            else if (string.IsNullOrEmpty(userInfo.company_code) || string.IsNullOrEmpty(userInfo.branch_code))
            {
                userInfo.company_code = "01";
                userInfo.branch_code = "01.01";
            }
            try
            {
                var conditioin = string.Empty;
                if (reportFilters.ProductFilter.Count() > 0)
                {
                    conditioin = " and a.item_code in (" + string.Join(",", reportFilters.ProductFilter) + @")";
                }
                var Query = @"SELECT Day, MONTHS as MonthsName,
monthsyear  as MonthsDisplayName,
       YEARMONTH  as yearmonth,
       TOATL_SALES_AMT as TotalSalesAmount,
       LANDED_COST as TotalLandedCost,       
       item_code as ItemCode, item_edesc as ItemName,
       
      ( case when landed_cost = 0 then 0
       else
       ROUND ( ( ( (TOATL_SALES_AMT - LANDED_COST) / LANDED_COST) * 100), 2) end)
          TotalProfitPer
  FROM (  SELECT SUM (NVL (a.QUANTITY, 0)) QTY,
    TO_CHAR(A.SALES_DATE, 'MON')MONTHS,
                   TO_CHAR(A.SALES_DATE, 'MON') || '-' || TO_CHAR(A.SALES_DATE, 'yyy') monthsyear,
                 TO_CHAR (A.SALES_DATE, 'YYYYMM') YEARMONTH,
                TO_CHAR (A.SALES_DATE, 'DD') as Day,
               b.item_code, b.item_edesc,
                 SUM (
                    NVL (
                       FN_CONVERT_CURRENCY (
                          NVL (a.TOTAL_PRICE, 0) * NVL (a.EXCHANGE_RATE, 1),
                          'NRS',
                          a.SALES_DATE),
                       0))
                    TOATL_SALES_AMT,
                 SUM (NVL (a.QUANTITY, 0) * NVL (d.LANDED_COST, 0)) LANDED_COST
            FROM SA_SALES_INVOICE a,
                 IP_ITEM_MASTER_SETUP b,
                 SA_CUSTOMER_SETUP c,
                 MP_ITEM_STD_RATE d,
                 ip_category_code ca
           WHERE  a.ITEM_CODE = b.ITEM_CODE
                 AND a.CUSTOMER_CODE = c.CUSTOMER_CODE
                 AND a.COMPANY_CODE = b.COMPANY_CODE
                 AND a.COMPANY_CODE = c.COMPANY_CODE
                 AND a.ITEM_CODE = d.ITEM_CODE(+)
                 AND a.COMPANY_CODE = d.COMPANY_CODE(+)
                 AND a.DELETED_FLAG = 'N'
                 AND a.COMPANY_CODE = '{0}'
                 AND a.BRANCH_CODE = '{1}'
                 and ca.category_code=b.category_code
                 and b.category_code = '{2}' and TO_CHAR(A.SALES_DATE, 'MON') = '{3}'
                 {4}        
        GROUP BY  b.category_code, ca.category_edesc,TO_CHAR (A.SALES_DATE, 'MON'),
                 TO_CHAR (A.SALES_DATE, 'YYYYMM'),  TO_CHAR(A.SALES_DATE, 'MON'),
                   TO_CHAR(A.SALES_DATE, 'MON') || '-' || TO_CHAR(A.SALES_DATE, 'yyy'),
                 TO_CHAR (A.SALES_DATE, 'DD'),b.item_code, b.item_edesc
        ORDER BY TO_CHAR (A.SALES_DATE, 'YYYYMM'))";

                Query = string.Format(Query, userInfo.company_code, userInfo.branch_code, categoryCode, month, conditioin);
                sales = _objectEntity.SqlQuery<GrossProfitReportDayWise>(Query).ToList();

            }
            catch (Exception ex)
            {

            }

            return sales;
        }

        public List<CompanySalesModel> GetCompanySalesMonthlyReport(filterOption model, string dateformat, bool salesReturn, string AmountType)
        {
            try
            {
                if (model.ReportFilters.CompanyFilter.Count() < 1)
                {
                    string CompanyQuery = "SELECT COMPANY_CODE FROM COMPANY_SETUP";
                    List<string> CompanyCode = this._objectEntity.SqlQuery<string>(CompanyQuery).ToList();

                    model.ReportFilters.CompanyFilter.AddRange(CompanyCode);
                }
                //var companyCode = string.Empty;
                //foreach (var company in model.ReportFilters.CompanyFilter)
                //{
                //    companyCode += $@"'{company}',";
                //}

                //companyCode = companyCode == "" ? $@"'{_workContext.CurrentUserinformation.company_code}'" : companyCode.Remove(companyCode.Length - 1);
                //Old Query From sa_sales_invoice Table
                string Query = @"SELECT 
                            --SUM(SI.CALC_TOTAL_PRICE) GrossAmount,
                            SUM (nvl(SI.CALC_TOTAL_PRICE,0)) -sum(nvl(C.CHARGE_AMOUNT,0)) GrossAmount,
                            sum(si.calc_quantity) quantity,
                            TO_CHAR (si.sales_date, 'YYYYMM') yearmonth,
                            TO_CHAR (si.sales_date, 'Month') month,
                            SI.COMPANY_CODE companycode,
                            CS.COMPANY_EDESC companyname
                        --FROM SA_SALES_INVOICE SI ,company_setup CS 
                        FROM SA_SALES_INVOICE SI inner join  company_setup CS on  SI.COMPANY_CODE = CS.COMPANY_CODE and cs.deleted_flag='N'
                         -- added by chandra for get total amount after discount
                             LEFT JOIN charge_transaction c
                            ON     SI.SALES_NO = C.REFERENCE_NO
                               AND SI.FORM_CODE = C.FORM_CODE
                               AND SI.COMPANY_CODE = C.COMPANY_CODE
                               AND CHARGE_CODE IN (SELECT DISTINCT CHARGE_CODE
                                                     FROM IP_CHARGE_CODE
                                                    WHERE SPECIFIC_CHARGE_FLAG = 'D')
                               AND APPLY_ON = 'D'
                               AND c.DELETED_FLAG = 'N'
                            --end added by chandra
                        WHERE SI.DELETED_FLAG='N' --AND SI.COMPANY_CODE=CS.COMPANY_CODE
                        {0}
                        GROUP BY SI.COMPANY_CODE ,
                        CS.COMPANY_EDESC,
                        TO_CHAR (si.sales_date, 'YYYYMM'),
                        TO_CHAR (si.sales_date, 'Month')";

                string QueryBS = @"SELECT 
                        --SUM(SI.CALC_TOTAL_PRICE) GrossAmount,
                          SUM (nvl(SI.CALC_TOTAL_PRICE,0)) -sum(nvl(C.CHARGE_AMOUNT,0))  GrossAmount,
                        sum(si.calc_quantity) quantity,
                        substr(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SI.SALES_DATE),6,2)),6,20) month,
                        SI.COMPANY_CODE companycode,
                        CS.COMPANY_EDESC companyname
                    --FROM SA_SALES_INVOICE SI ,company_setup CS 
                     FROM SA_SALES_INVOICE SI inner join  company_setup CS on  SI.COMPANY_CODE = CS.COMPANY_CODE and cs.deleted_flag='N'
                         -- added by chandra for get total amount after discount
                             LEFT JOIN charge_transaction c
                            ON     SI.SALES_NO = C.REFERENCE_NO
                               AND SI.FORM_CODE = C.FORM_CODE
                               AND SI.COMPANY_CODE = C.COMPANY_CODE
                               AND CHARGE_CODE IN (SELECT DISTINCT CHARGE_CODE
                                                     FROM IP_CHARGE_CODE
                                                    WHERE SPECIFIC_CHARGE_FLAG = 'D')
                               AND APPLY_ON = 'D'
                               AND c.DELETED_FLAG = 'N'
                            --end added by chandra
                    WHERE SI.DELETED_FLAG='N' --AND SI.COMPANY_CODE=CS.COMPANY_CODE
                    {0}
                    GROUP BY SI.COMPANY_CODE ,
                    CS.COMPANY_EDESC,
                    FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SI.SALES_DATE),6,2)),substr(bs_date(si.sales_date),6,2)
                    Order by substr(bs_date(si.sales_date),6,2)";


                //New Query From V$SALES_INVOICE_REPORT3 View
                string QueryNew = @"SELECT 
                            nvl(sum(nvl(SI.TOTAL_SALES,0))/1,0)as TotalAmount,  
                        nvl(sum(nvl(SI.GROSS_SALES,0))/1,0)as GrossAmount,                           
                        sum(nvl(SI.Quantity,0))/1 as Quantity,    
                        nvl(sum(nvl(SI.NET_SALES,0))/1,0)as NETAmount,
                            TO_CHAR (si.sales_date, 'YYYYMM') yearmonth,
                        TO_CHAR (si.sales_date, 'Month') month,
                        SI.COMPANY_CODE companycode,
                        CS.COMPANY_EDESC companyname
                    FROM V$SALES_INVOICE_REPORT3 SI ,company_setup CS 
                        WHERE SI.DELETED_FLAG='N' AND SI.COMPANY_CODE=CS.COMPANY_CODE
                        {0}
                        GROUP BY SI.COMPANY_CODE,
                        CS.COMPANY_EDESC,
                        TO_CHAR (si.sales_date, 'YYYYMM'),
                        TO_CHAR (si.sales_date, 'Month')";

                string QueryNewBS = @"SELECT 
                            nvl(sum(nvl(SI.TOTAL_SALES,0))/1,0)as TotalAmount,  
                        nvl(sum(nvl(SI.GROSS_SALES,0))/1,0)as GrossAmount,                           
                        sum(nvl(SI.Quantity,0))/1 as Quantity,    
                        nvl(sum(nvl(SI.NET_SALES,0))/1,0)as NETAmount,
                        substr(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SI.SALES_DATE),6,2)),6,20) month,
                        SI.COMPANY_CODE companycode,
                        CS.COMPANY_EDESC companyname
                    FROM V$SALES_INVOICE_REPORT3 SI ,company_setup CS 
                        WHERE SI.DELETED_FLAG='N' AND SI.COMPANY_CODE=CS.COMPANY_CODE
                        {0}
                        GROUP BY SI.COMPANY_CODE ,
                        CS.COMPANY_EDESC,
                        FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SI.SALES_DATE),6,2)),substr(bs_date(si.sales_date),6,2)
                    Order by substr(bs_date(si.sales_date),6,2)";

                string Condition = string.Empty;
                string ConditionNew = string.Empty;

                if (model.ReportFilters.ProductFilter.Count() > 0)
                {
                    string productFilter = string.Empty;
                    productFilter = @"select  DISTINCT item_code from IP_ITEM_MASTER_SETUP where (";
                    //IF PRODUCT_SKU_FLAG = G
                    foreach (var company in model.ReportFilters.CompanyFilter)
                    {
                        foreach (var item in model.ReportFilters.ProductFilter)
                        {
                            productFilter += "MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE ='" + company + "') OR ";
                        }
                    }
                    productFilter = productFilter.Substring(0, productFilter.Length - 3);
                    //IF PRODUCT_SKU_FLAG = I                
                    productFilter += " OR (ITEM_CODE in (" + string.Join(",", model.ReportFilters.ProductFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN('" + string.Join("','", model.ReportFilters.CompanyFilter) + "'))) ";
                    Condition += " AND si.ITEM_CODE IN(" + productFilter + ")";
                }

                if (model.ReportFilters.CustomerFilter.Count() > 0)
                {
                    string customerFilter = string.Empty;
                    customerFilter = @"SELECT  DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE (";
                    //IF CUSTOMER_SKU_FLAG = G
                    foreach (var item in model.ReportFilters.CustomerFilter)
                    {
                        customerFilter += "MASTER_CUSTOMER_CODE LIKE (SELECT DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN('" + string.Join("','", model.ReportFilters.CompanyFilter) + "')) OR ";
                    }
                    customerFilter = customerFilter.Substring(0, customerFilter.Length - 3);
                    //IF CUSTOMER_SKU_FLAG = I                
                    customerFilter += " OR (CUSTOMER_CODE IN (" + string.Join(",", model.ReportFilters.CustomerFilter) + ") AND GROUP_SKU_FLAG = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN('" + string.Join("','", model.ReportFilters.CompanyFilter) + "'))) ";
                    Condition += " AND si.CUSTOMER_CODE IN(" + customerFilter + ")";
                }
                //Area Filter
                if (model.ReportFilters.AreaTypeFilter.Count > 0)
                {
                    Condition += string.Format(@" AND si.AREA_CODE IN ('{0}') ", string.Join("','", model.ReportFilters.AreaTypeFilter).ToString());
                }
                if (model.ReportFilters.EmployeeFilter.Count > 0)
                {
                    Condition += string.Format(@" AND si.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", model.ReportFilters.EmployeeFilter).ToString());
                }
                if (model.ReportFilters.AgentFilter.Count > 0)
                {
                    Condition += string.Format(@" AND si.AGENT_CODE IN  ('{0}')", string.Join("','", model.ReportFilters.AgentFilter).ToString());
                }
                if (model.ReportFilters.DivisionFilter.Count > 0)
                {
                    Condition += string.Format(@" AND si.DIVISION_CODE IN  ('{0}')", string.Join("','", model.ReportFilters.DivisionFilter).ToString());
                    ConditionNew += string.Format(@" AND si.DIVISION_CODE IN  ('{0}')", string.Join("','", model.ReportFilters.DivisionFilter).ToString());
                }
                if (model.ReportFilters.LocationFilter.Count > 0)
                {
                    string locationFilter = string.Empty;
                    var locations = model.ReportFilters.LocationFilter;
                    for (int i = 0; i < locations.Count; i++)
                    {

                        if (i == 0)
                            locationFilter += string.Format("SELECT LOCATION_CODE FROM IP_LOCATION_SETUP WHERE LOCATION_CODE LIKE '{0}%' ", locations[i]);
                        else
                        {
                            locationFilter += string.Format(" OR LOCATION_CODE like '{0}%' ", locations[i]);
                        }
                    }
                    Condition += string.Format(@" AND si.FROM_LOCATION_CODE IN ({0} OR LOCATION_CODE IN ('{1}'))", locationFilter, string.Join("','", locations));
                    //query = query.AppendFormat(@" AND A.FROM_LOCATION_CODE IN ('{0}')", string.Join("','", filters.LocationFilter).ToString());
                }
                List<CompanySalesModel> resultList = new List<CompanySalesModel>();
                Query = string.Format(Query, Condition);
                QueryBS = string.Format(QueryBS, Condition);
                QueryNew = string.Format(QueryNew, ConditionNew);
                QueryNewBS = string.Format(QueryNewBS, ConditionNew);

                string QueryFinal = string.Empty;
                if (AmountType == "NetAmount" || AmountType == "TotalAmount")
                {
                    QueryFinal = string.Equals(dateformat, "BS") ? QueryNewBS : QueryNew;
                }
                else
                {
                    QueryFinal = string.Equals(dateformat, "BS") ? QueryBS : Query;
                }

                resultList = this._objectEntity.SqlQuery<CompanySalesModel>(QueryFinal).ToList();
                return resultList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CompanyProductionModel> GetCompanyProductionMonthlyReport(filterOption model, string dateformat)
        {
            try
            {
                if (model.ReportFilters.CompanyFilter.Count() < 1)
                {
                    string CompanyQuery = "SELECT COMPANY_CODE FROM COMPANY_SETUP";
                    List<string> CompanyCode = this._objectEntity.SqlQuery<string>(CompanyQuery).ToList();


                    model.ReportFilters.CompanyFilter.AddRange(CompanyCode);
                }

                string itemfilter = string.Empty;
                if (model.ReportFilters.ProductFilter.Count() > 0)
                {
                    itemfilter = @"select  DISTINCT item_code from IP_ITEM_MASTER_SETUP where (";
                    //IF PRODUCT_SKU_FLAG = G
                    foreach (var company in model.ReportFilters.CompanyFilter)
                    {
                        foreach (var item in model.ReportFilters.ProductFilter)
                        {
                            itemfilter += "MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE ='" + company + "') OR ";
                        }
                    }
                    itemfilter = itemfilter.Substring(0, itemfilter.Length - 3);
                    //IF PRODUCT_SKU_FLAG = I                
                    itemfilter += " OR (ITEM_CODE in (" + string.Join(",", model.ReportFilters.ProductFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN('" + string.Join("','", model.ReportFilters.CompanyFilter) + "'))) ";
                    itemfilter = " AND ITEM_CODE IN(" + itemfilter + ")";
                }
                string customerfilter = string.Empty;
                if (model.ReportFilters.CustomerFilter.Count() > 0)
                {
                    string condition = " 0 = 0 and (";
                    foreach (var item in model.ReportFilters.CustomerFilter)
                    {
                        condition += "cus.master_customer_code like  (Select MASTER_CUSTOMER_CODE || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "') OR ";

                    }
                    condition = condition.Substring(0, condition.Length - 3) + ") ";
                    customerfilter += " and si.customer_code in (select cus.customer_code from sa_customer_setup cus where " + condition + ")";
                }

                string query = @"SELECT 
                                    FN_FETCH_DESC(COMPANY_CODE, 'COMPANY_SETUP', COMPANY_CODE) COMPANYNAME,
                                    COMPANY_CODE companycode,
                                    substr(FN_CHARTAD_MONTH(SUBSTR(MRR_DATE,4,3)),3,20) month, 
                                    SUM (NVL(QUANTITY,0))/1 as quantity
                                FROM IP_PRODUCTION_MRR 
                                WHERE COMPANY_CODE IN(select company_code from company_setup where deleted_flag='N')
                                {0}
                                GROUP BY FN_CHARTAD_MONTH(SUBSTR(MRR_DATE,4,3)) ,FN_FETCH_DESC(COMPANY_CODE, 'COMPANY_SETUP', COMPANY_CODE) ,COMPANY_CODE
                                ORDER BY FN_FETCH_DESC(COMPANY_CODE, 'COMPANY_SETUP', COMPANY_CODE) ,FN_CHARTAD_MONTH(SUBSTR(MRR_DATE,4,3))";
                if (dateformat == "BS")
                {
                    query = @"
                    SELECT
                    SUM (NVL(QUANTITY,0))/1 as quantity,
                    COMPANY_CODE companycode,
                    FN_FETCH_DESC(COMPANY_CODE, 'COMPANY_SETUP', COMPANY_CODE) COMPANYNAME,
                    substr(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(MRR_DATE),6,2)),6,20) month
                    FROM IP_PRODUCTION_MRR 
                    WHERE COMPANY_CODE IN(select company_code from company_setup where deleted_flag='N')
                    {0}
                    GROUP BY FN_CHARTBS_MONTH(SUBSTR(BS_DATE(MRR_DATE),6,2)) ,FN_FETCH_DESC(COMPANY_CODE, 'COMPANY_SETUP', COMPANY_CODE),
                    COMPANY_CODE 
                    ORDER BY FN_FETCH_DESC(COMPANY_CODE, 'COMPANY_SETUP', COMPANY_CODE) ,FN_CHARTBS_MONTH(SUBSTR(BS_DATE(MRR_DATE),6,2))";
                }

                query = string.Format(query, itemfilter, customerfilter);
                List<CompanyProductionModel> resultList = this._objectEntity.SqlQuery<CompanyProductionModel>(query).ToList();
                return resultList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Method for Stock Company Monthly Report
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dateformat"></param>
        /// <returns></returns>
        public List<StockChartModel> GetCompanyStockMonthlyReport(filterOption model, string dateformat)
        {
            try
            {
                string Query = string.Format(@"SELECT COMPANY_CODE as Companycode, FN_FETCH_DESC(COMPANY_CODE, 'COMPANY_SETUP', COMPANY_CODE) COMPANYNAME ,
                            SUM (NVL(IN_QUANTITY,0)-NVL(OUT_QUANTITY,0)) as quantity
                            FROM V$VIRTUAL_STOCK_WIP_LEDGER1
                            WHERE COMPANY_CODE IN(select company_code from company_setup where deleted_flag='N')
                            and deleted_flag='N'
                            and VOUCHER_DATE <= to_date('{0}','YYYY-MM-DD')"
                           , model.ReportFilters.FromDate);

                ////for Product Filter
                var productFilter = string.Empty;
                if (model.ReportFilters.ProductFilter.Count() > 0)
                {
                    productFilter = @"select  DISTINCT item_code from IP_ITEM_MASTER_SETUP where (";
                    //IF PRODUCT_SKU_FLAG = G
                    foreach (var item in model.ReportFilters.ProductFilter)
                    {
                        productFilter += "MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N') OR ";
                    }
                    productFilter = productFilter.Substring(0, productFilter.Length - 3);
                    //IF PRODUCT_SKU_FLAG = I                
                    productFilter += " or (ITEM_CODE in (" + string.Join(",", model.ReportFilters.ProductFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N')) ";

                    Query += " and ITEM_CODE IN(" + productFilter + ")";
                }

                /////for category Filter
                if (model.ReportFilters.CategoryFilter.Count() > 0)
                {
                    Query += " and CATEGORY_CODE IN('" + string.Join("','", model.ReportFilters.CategoryFilter) + "')";
                }


                Query += @" 
                          GROUP BY COMPANY_CODE,FN_FETCH_DESC(COMPANY_CODE, 'COMPANY_SETUP', COMPANY_CODE) ";

                List<StockChartModel> resultList = this._objectEntity.SqlQuery<StockChartModel>(Query).ToList();
                return resultList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public List<BalanceChartModel> GetCompanyBankBalanceReport(filterOption model, string dateformat)
        {
            try
            {
                string Query = string.Format(@"SELECT COMPANY_CODE as Companycode,
                                               FN_FETCH_DESC(COMPANY_CODE,'COMPANY_SETUP',COMPANY_CODE) AS Companyname, 
                                               NVL(SUM(NVL( CR_AMOUNT,0))-SUM(NVL(DR_AMOUNT,0)),0)/{1} Amount 
                                               FROM V$VIRTUAL_GENERAL_LEDGER 
                                               WHERE DELETED_FLAG='N'    
                                               AND ACC_NATURE in('AC')
                                               and VOUCHER_DATE <= to_date('{0}','YYYY-MM-DD')
                                              GROUP BY FN_FETCH_DESC(COMPANY_CODE,'COMPANY_SETUP',COMPANY_CODE),COMPANY_CODE
                                              order by FN_FETCH_DESC(COMPANY_CODE,'COMPANY_SETUP',COMPANY_CODE) "
                           , model.ReportFilters.FromDate, ReportFilterHelper.FigureFilterValue(model.ReportFilters.AmountFigureFilter));

                List<BalanceChartModel> resultList = this._objectEntity.SqlQuery<BalanceChartModel>(Query).ToList();
                return resultList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public List<BalanceChartModel> GetCompanyCashBalanceReport(filterOption model, string dateformat)
        {
            try
            {
                string Query = string.Format(@"SELECT COMPANY_CODE as Companycode,
                                               FN_FETCH_DESC(COMPANY_CODE,'COMPANY_SETUP',COMPANY_CODE) AS Companyname, 
                                               NVL(SUM(NVL( DR_AMOUNT,0))-SUM(NVL(CR_AMOUNT,0)),0)/{1} Amount 
                                               FROM V$VIRTUAL_GENERAL_LEDGER 
                                               WHERE DELETED_FLAG='N'    
                                               AND ACC_NATURE in('AB')
                                               and VOUCHER_DATE <= to_date('{0}','YYYY-MM-DD')
                                              GROUP BY FN_FETCH_DESC(COMPANY_CODE,'COMPANY_SETUP',COMPANY_CODE),COMPANY_CODE
                                              order by FN_FETCH_DESC(COMPANY_CODE,'COMPANY_SETUP',COMPANY_CODE) "
                           , model.ReportFilters.FromDate, ReportFilterHelper.FigureFilterValue(model.ReportFilters.AmountFigureFilter));

                List<BalanceChartModel> resultList = this._objectEntity.SqlQuery<BalanceChartModel>(Query).ToList();
                return resultList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<CompanyExpenseModel> GetCompanyExpenseMonthlyReport(filterOption model, string dateformat)
        {
            try
            {

                string Query = string.Empty;
                if (dateformat == "BS")
                {
                    Query = @"SELECT COMPANY_CODE as CompanyCode,
                           FN_FETCH_DESC(COMPANY_CODE,'COMPANY_SETUP',COMPANY_CODE) AS CompanyName, 
                          TRIM(SUBSTR(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(VOUCHER_DATE),6,2)),6,20)) Month,
                          TRIM(SUBSTR(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(VOUCHER_DATE),6,2)),6,20)) YearMonth,
                         NVL(SUM(NVL( DR_AMOUNT,0))-SUM(NVL(CR_AMOUNT,0)),0) Amount 
                      FROM V$VIRTUAL_GENERAL_LEDGER 
                      WHERE DELETED_FLAG='N'
                         AND ACC_NATURE in('EC','EB')
                  GROUP BY FN_CHARTBS_MONTH(SUBSTR(BS_DATE(VOUCHER_DATE),6,2)),FN_FETCH_DESC(COMPANY_CODE,'COMPANY_SETUP',COMPANY_CODE),COMPANY_CODE
                  order by FN_CHARTBS_MONTH(SUBSTR(BS_DATE(VOUCHER_DATE),6,2)),FN_FETCH_DESC(COMPANY_CODE,'COMPANY_SETUP',COMPANY_CODE)";
                }
                else
                {
                    Query = @"SELECT COMPANY_CODE as CompanyCode,
                               FN_FETCH_DESC(COMPANY_CODE,'COMPANY_SETUP',COMPANY_CODE) AS CompanyName, 
                                TO_CHAR(VOUCHER_DATE, 'MON') Month,
                                TO_CHAR(VOUCHER_DATE, 'MON-YYY') YearMonth,             
                             NVL(SUM(NVL( DR_AMOUNT,0))-SUM(NVL(CR_AMOUNT,0)),0) Amount 
                          FROM V$VIRTUAL_GENERAL_LEDGER 
                          WHERE DELETED_FLAG='N'
                             AND ACC_NATURE in('EC','EB')
                      GROUP BY TO_CHAR(VOUCHER_DATE, 'MON'),TO_CHAR(VOUCHER_DATE, 'MON-YYY'),TO_CHAR(VOUCHER_DATE, 'YYYYMM') ,FN_FETCH_DESC(COMPANY_CODE,'COMPANY_SETUP',COMPANY_CODE),COMPANY_CODE
                      order by TO_CHAR(VOUCHER_DATE, 'YYYYMM'),FN_FETCH_DESC(COMPANY_CODE,'COMPANY_SETUP',COMPANY_CODE)";
                }
                List<CompanyExpenseModel> resultList = this._objectEntity.SqlQuery<CompanyExpenseModel>(Query).ToList();
                return resultList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CompanySalesModel> GetCompanyPurchaseMonthlyReport(filterOption model, string dateformat, User userInfo)
        {
            var totalPurchase = new List<CompanySalesModel>();
            try
            {
                var companyCode = string.Join(",", model.ReportFilters.CompanyFilter);
                companyCode = companyCode == "" ? userInfo.company_code : companyCode;
                //var companyCode = string.Empty;
                //foreach (var company in model.ReportFilters.CompanyFilter)
                //{
                //    companyCode += $@"'{company}',";
                //}

                //companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
                model.ReportFilters.CompanyFilter = companyCode.Split(',').ToList();

                if (model.ReportFilters.CompanyFilter.Count() < 1)
                {
                    string CompanyQuery = "SELECT COMPANY_CODE FROM COMPANY_SETUP";
                    List<string> CompanyCode = this._objectEntity.SqlQuery<string>(CompanyQuery).ToList();
                    model.ReportFilters.CompanyFilter.AddRange(CompanyCode);
                }

                string productFilter = string.Empty;
                if (model.ReportFilters.ProductFilter.Count() > 0)
                {
                    productFilter = @"select  DISTINCT item_code from IP_ITEM_MASTER_SETUP WHERE ";
                    //IF PRODUCT_SKU_FLAG = G
                    foreach (var company in model.ReportFilters.CompanyFilter)
                    {
                        foreach (var item in model.ReportFilters.ProductFilter)
                        {
                            productFilter += "MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE ='" + company + "') OR ";
                        }
                    }
                    productFilter = productFilter.Substring(0, productFilter.Length - 3);
                    //IF PRODUCT_SKU_FLAG = I                
                    productFilter += " OR (ITEM_CODE in (" + string.Join(",", model.ReportFilters.ProductFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN('" + string.Join("','", model.ReportFilters.CompanyFilter) + "')) ";
                    productFilter = " AND ITEM_CODE IN(" + productFilter + ")";
                }

                string partyFilter = string.Empty;
                if (model.ReportFilters.PartyTypeFilter.Count() > 0)
                {
                    partyFilter = @"SELECT DISTINCT SUPPLIER_CODE FROM IP_SUPPLIER_SETUP WHERE PARTY_TYPE_CODE IN ('" + string.Join("','", model.ReportFilters.PartyTypeFilter) + "')";
                    partyFilter = " AND SUPPLIER_CODE IN(" + partyFilter + ")";
                }
                string Query = string.Empty;
                if (dateformat.ToLower().Trim().Equals("AD".ToLower().Trim()))
                {
                    Query = @"SELECT FN_FETCH_DESC(COMPANY_CODE, 'COMPANY_SETUP', COMPANY_CODE) Companyname,company_code as Companycode ,to_char(INVOICE_DATE,'Month') Month,
    SUM(NVL(QUANTITY, 0)) / 1 as Quantity,
    SUM(NVL(CALC_TOTAL_PRICE, 0)) / 1 as Amount,
    to_char(INVOICE_DATE, 'MM')
    FROM IP_PURCHASE_INVOICE
    where deleted_flag = 'N' AND  COMPANY_CODE IN(select company_code from COMPANY_SETUP where deleted_flag='N')
        {0}
        {1}
    group by to_char(INVOICE_DATE, 'Month'),to_char(INVOICE_DATE, 'MM'),company_code,FN_FETCH_DESC(COMPANY_CODE, 'COMPANY_SETUP', COMPANY_CODE)
    order by to_char(INVOICE_DATE, 'MM'), company_code";
                }

                else
                {
                    Query = @"SELECT FN_FETCH_DESC(COMPANY_CODE, 'COMPANY_SETUP', COMPANY_CODE) Companyname,company_code as Companycode ,
    trim(substr(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(INVOICE_DATE),6,2)),5,10))  Month, 
    SUM(NVL(QUANTITY, 0)) / 1 as Quantity,
    SUM(NVL(CALC_TOTAL_PRICE, 0)) / 1 as Amount
    FROM IP_PURCHASE_INVOICE
    WHERE deleted_flag = 'N' and  COMPANY_CODE IN(select company_code from COMPANY_SETUP where deleted_flag='N')
        {0}
        {1}
    GROUP BY FN_CHARTBS_MONTH(SUBSTR(BS_DATE(INVOICE_DATE), 6, 2)) ,company_code,FN_FETCH_DESC(COMPANY_CODE, 'COMPANY_SETUP', COMPANY_CODE)
    ORDER BY FN_FETCH_DESC(COMPANY_CODE, 'COMPANY_SETUP', COMPANY_CODE) ,FN_CHARTBS_MONTH(SUBSTR(BS_DATE(INVOICE_DATE), 6, 2))";
                }

                Query = string.Format(Query, productFilter, partyFilter);
                totalPurchase = _objectEntity.SqlQuery<CompanySalesModel>(Query).ToList();
                return totalPurchase;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<MonthlyBranchSalesGraph> GetMonthlyPurchaseSalesSummary(ReportFiltersModel reportFilters, User userInfo, string dateFormat)
        {
            //reportFilters.CompanyFilter.Add("02");
            var totalPurchases = new List<MonthlyBranchSalesGraph>();
            try
            {
                if (userInfo == null)
                {
                    userInfo = new Core.Domain.User();
                    //userInfo.company_code = "01";
                    userInfo.branch_code = "01.01";

                }
                else if (string.IsNullOrEmpty(userInfo.company_code) || string.IsNullOrEmpty(userInfo.branch_code))
                {
                    //userInfo.company_code = "01";
                    userInfo.branch_code = "01.01";
                }
                var companyCode = string.Empty;
                foreach (var company in reportFilters.CompanyFilter)
                {
                    if (company != null)
                    {
                        companyCode += $@"'{company}',";
                    }
                }
                companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
                if (dateFormat.Trim().ToLower().Equals("Ad".ToLower()))
                {
                    var query = @"SELECT FN_FETCH_DESC(COMPANY_CODE, 'FA_BRANCH_SETUP', BRANCH_CODE) BranchName,
BRANCH_CODE as branch_code,
to_char(INVOICE_DATE,'Month') Month, 
SUM(NVL(QUANTITY, 0)) / 1 as Quantity,
SUM(NVL(CALC_TOTAL_PRICE, 0)) / 1 as Amount,
to_char(INVOICE_DATE, 'MM')
FROM IP_PURCHASE_INVOICE
WHERE COMPANY_CODE IN(" + companyCode + @")
and deleted_flag='N'
GROUP BY to_char(INVOICE_DATE,'Month'),to_char(INVOICE_DATE, 'MM'),FN_FETCH_DESC(COMPANY_CODE, 'FA_BRANCH_SETUP', BRANCH_CODE),BRANCH_CODE
  ORDER BY FN_FETCH_DESC(COMPANY_CODE, 'FA_BRANCH_SETUP', BRANCH_CODE) ,to_char(INVOICE_DATE,'Month')";
                    totalPurchases = _objectEntity.SqlQuery<MonthlyBranchSalesGraph>(query).ToList();

                    return totalPurchases;
                }
                else
                {
                    var query = @"SELECT FN_FETCH_DESC(COMPANY_CODE, 'FA_BRANCH_SETUP', BRANCH_CODE) BranchName,
BRANCH_CODE as branch_code,
trim(substr(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(INVOICE_DATE), 6, 2)), 5, 20)) Month, 
SUBSTR(BS_DATE(INVOICE_DATE), 6, 2) NepaliMonthInt ,
SUM(NVL(QUANTITY, 0)) / 1 as Quantity,
SUM(NVL(CALC_TOTAL_PRICE, 0)) / 1 as Amount
FROM IP_PURCHASE_INVOICE
WHERE COMPANY_CODE IN(" + companyCode + @")
GROUP BY SUBSTR(BS_DATE(INVOICE_DATE), 6, 2),FN_FETCH_DESC(COMPANY_CODE, 'FA_BRANCH_SETUP', BRANCH_CODE),BRANCH_CODE
  ORDER BY SUBSTR(BS_DATE(INVOICE_DATE), 6, 2),FN_FETCH_DESC(COMPANY_CODE, 'FA_BRANCH_SETUP', BRANCH_CODE) ";
                    totalPurchases = _objectEntity.SqlQuery<MonthlyBranchSalesGraph>(query).ToList();
                    return totalPurchases;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public List<MonthlyBranchProductionGraph> GetMonthlyBranchProductionSummary(ReportFiltersModel reportFilters, User userInfo, string dateFormat, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode)
        {

            // old query implemented as new query took more exicution time


            companyCode = string.Join(",", reportFilters.CompanyFilter);
            companyCode = companyCode == "" ? userInfo.company_code : companyCode;

            //**********************************************************************************
            //THIS QUERY IS GIVEN BY BIKALPA SIR
            string Query = string.Empty;
            if (string.Equals(dateFormat, "AD"))
            {
                Query = string.Format(@"SELECT 
                                FN_FETCH_DESC(COMPANY_CODE, 'FA_BRANCH_SETUP', BRANCH_CODE) BranchName,
                                Branch_code,
                                trim(substr(FN_CHARTAD_MONTH(SUBSTR(MRR_DATE,4,3)),2,12)) MONTH, 
                                TO_CHAR(MRR_DATE,'MON-YYYY') MONTHYEAR,  
                                TO_CHAR(MRR_DATE,'YYYYMM') NepaliMonthInt,
                                SUM (NVL(QUANTITY,0))/{0} as Quantity
                            FROM IP_PRODUCTION_MRR 
                            WHERE COMPANY_CODE = '{1}'
                            ",
                                            ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode);
            }
            else
            {
                Query = string.Format(@"SELECT 
                                FN_FETCH_DESC(COMPANY_CODE, 'FA_BRANCH_SETUP', BRANCH_CODE) BranchName,
                                Branch_code,
                                trim(substr(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(MRR_DATE),6,2)),6,12)) MONTH, 
                                trim(substr(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(MRR_DATE),6,2)),6,12)) MONTHYEAR,  
                                SUBSTR(BS_DATE(MRR_DATE),6,2) NepaliMonthInt,
                                SUM (NVL(QUANTITY,0))/{0} as Quantity
                            FROM IP_PRODUCTION_MRR 
                            WHERE COMPANY_CODE = '{1}'
                            ",
                                            ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode);
            }

            //****************************
            //CONDITIONS FITLER START HERE
            //****************************
            // no customer_code in IP_PRODUCTION_MRR
            //for customer Filter
            //var customerFilter = string.Empty;
            //if (reportFilters.CustomerFilter.Count() > 0)
            //{
            //    customerFilter = @"select  DISTINCT(customer_code) from sa_customer_setup where (";
            //    //IF CUSTOMER_SKU_FLAG = G
            //    foreach (var item in reportFilters.CustomerFilter)
            //    {
            //        customerFilter += "master_customer_code like  (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
            //    }
            //    customerFilter = customerFilter.Substring(0, customerFilter.Length - 3);
            //    //IF CUSTOMER_SKU_FLAG = I                
            //    customerFilter += " or (customer_code in (" + string.Join(",", reportFilters.CustomerFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


            //    Query += " and sl.customer_code IN(" + customerFilter + ")";
            //}




            ////for Product Filter
            //var productFilter = string.Empty;
            //if (reportFilters.ProductFilter.Count() > 0)
            //{
            //    productFilter = @"select  DISTINCT item_code from IP_ITEM_MASTER_SETUP where (";
            //    //IF PRODUCT_SKU_FLAG = G
            //    foreach (var item in reportFilters.ProductFilter)
            //    {
            //        productFilter += "MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
            //    }
            //    productFilter = productFilter.Substring(0, productFilter.Length - 3);
            //    //IF PRODUCT_SKU_FLAG = I                
            //    productFilter += " or (ITEM_CODE in (" + string.Join(",", reportFilters.ProductFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


            //    Query += " and ITEM_CODE IN(" + productFilter + ")";
            //}


            //if (reportFilters.BranchFilter.Count > 0)
            //{
            //    Query += string.Format(@" AND BRANCH_CODE IN ('{0}')", string.Join("','", reportFilters.BranchFilter).ToString());
            //}

            //****************************
            //CONDITIONS FITLER END HERE
            //****************************

            if (string.Equals(dateFormat, "AD"))
            {
                Query += @"
                          GROUP BY 
                                FN_FETCH_DESC(COMPANY_CODE, 'FA_BRANCH_SETUP', BRANCH_CODE),
                                Branch_code,
                                trim(substr(FN_CHARTAD_MONTH(SUBSTR(MRR_DATE,4,3)),2,12)),
                                TO_CHAR(MRR_DATE,'MON-YYYY'),
                                TO_CHAR(MRR_DATE,'YYYYMM')
                            ORDER BY 
                                FN_FETCH_DESC(COMPANY_CODE, 'FA_BRANCH_SETUP', BRANCH_CODE),
                                TO_CHAR(MRR_DATE,'MON-YYYY')";
            }
            else
            {
                Query += @"
                            GROUP BY FN_CHARTBS_MONTH(SUBSTR(BS_DATE(MRR_DATE),6,2)) ,
                            FN_FETCH_DESC(COMPANY_CODE, 'FA_BRANCH_SETUP', BRANCH_CODE),
                            Branch_code,
                            trim(substr(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(MRR_DATE),6,2)),6,12)),
                            trim(substr(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(MRR_DATE),6,2)),6,12)) ,
                            SUBSTR(BS_DATE(MRR_DATE),6,2)
                            ORDER BY FN_FETCH_DESC(COMPANY_CODE, 'FA_BRANCH_SETUP', BRANCH_CODE) ,FN_CHARTBS_MONTH(SUBSTR(BS_DATE(MRR_DATE),6,2))";
            }

            var salesReportList = _objectEntity.SqlQuery<MonthlyBranchProductionGraph>(Query).ToList();
            return salesReportList;

        }


        public List<BranchDaysSalesGraph> GetDailyBranchPurchaseSummary(ReportFiltersModel reportFilters, User userInfo, string monthName, string branchName,
          string DateFormat)
        {
            var companyCode = string.Join(",", reportFilters.CompanyFilter);
            companyCode = companyCode == "" ? userInfo.company_code : companyCode;

            var Query = string.Empty;
            if (string.Equals(DateFormat, "AD"))
            {
                Query = @"SELECT FN_FETCH_DESC(COMPANY_CODE, 'FA_BRANCH_SETUP', BRANCH_CODE) AS BranchName,
BRANCH_CODE as BRANCH_CODE,
to_char(invoice_date,'dd') DAY,
SUM(NVL(QUANTITY, 0)) / 1 as Quantity,
SUM(NVL(CALC_TOTAL_PRICE, 0)) / 1 as Amount
FROM IP_PURCHASE_INVOICE
WHERE
deleted_flag='N' and  
BRANCH_CODE='" + branchName + @"' and 
LOWER(TRIM(to_char(invoice_date,'Month')))=LOWER(trim('" + monthName + @"'))
GROUP BY to_char(invoice_date,'dd'),BRANCH_CODE ,FN_FETCH_DESC(COMPANY_CODE, 'FA_BRANCH_SETUP', BRANCH_CODE)
ORDER BY FN_FETCH_DESC(COMPANY_CODE, 'FA_BRANCH_SETUP', BRANCH_CODE),to_char(invoice_date,'dd')";
                var daysTotalsales = _objectEntity.SqlQuery<BranchDaysSalesGraph>(Query).ToList();
                return daysTotalsales;

            }
            else
            {
                Query = @"SELECT FN_FETCH_DESC(COMPANY_CODE, 'FA_BRANCH_SETUP', BRANCH_CODE) AS BranchName,
SUBSTR(BS_DATE(INVOICE_DATE), 9, 2) DAY,
SUM(NVL(QUANTITY, 0)) / 1 as Quantity,
SUM(NVL(CALC_TOTAL_PRICE, 0)) / 1 as Amount
FROM IP_PURCHASE_INVOICE
WHERE deleted_flag='N' and  
BRANCH_CODE='" + branchName + @"'
and 
LOWER(TRIM(substr(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(INVOICE_DATE),6,2)),6,19)))=LOWER(trim('" + monthName + @"'))
GROUP BY SUBSTR(BS_DATE(INVOICE_DATE), 9, 2) ,BRANCH_CODE,FN_FETCH_DESC(COMPANY_CODE, 'FA_BRANCH_SETUP', BRANCH_CODE)
ORDER BY FN_FETCH_DESC(COMPANY_CODE, 'FA_BRANCH_SETUP', BRANCH_CODE) ,SUBSTR(BS_DATE(INVOICE_DATE), 9, 2)";
                var daysTotalsales = _objectEntity.SqlQuery<BranchDaysSalesGraph>(Query).ToList();
                return daysTotalsales;


            }



            //****************************
            //CONDITIONS FITLER START HERE
            //****************************

            //for customer Filter
            if (reportFilters.CustomerFilter.Count() > 0)
            {
                Query += " and (";
                //IF CUSTOMER_SKU_FLAG = G
                foreach (var item in reportFilters.CustomerFilter)
                {
                    Query += "cs.master_customer_code like  (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND COMPANY_CODE IN(" + companyCode + ") ) OR ";
                }
                //IF CUSTOMER_SKU_FLAG = I                
                Query += "(cs.CUSTOMER_CODE IN (" + string.Join(",", reportFilters.CustomerFilter) + ") AND cs.GROUP_SKU_FLAG = 'I' AND cs.COMPANY_CODE IN(" + companyCode + ") )) ";

                Query = Query.Substring(0, Query.Length - 1);
            }

            ////for Product Filter
            if (reportFilters.ProductFilter.Count() > 0)
            {
                Query += " and (";
                //IF PRODUCT_SKU_FLAG = G
                foreach (var item in reportFilters.ProductFilter)
                {
                    Query += "IM.MASTER_ITEM_CODE like  (Select DISTINCT(MASTER_ITEM_CODE) || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND COMPANY_CODE IN(" + companyCode + ") ) OR ";
                }
                //IF PRODUCT_SKU_FLAG = I                
                Query += "(IM.ITEM_CODE IN (" + string.Join(",", reportFilters.ProductFilter) + ") AND IM.GROUP_SKU_FLAG = 'I')) ";

                Query = Query.Substring(0, Query.Length - 1);
            }


            //for category Filter
            if (reportFilters.CategoryFilter.Count() > 0)
            {
                Query += " and (";
                foreach (var item in reportFilters.CategoryFilter)
                {
                    Query += "IM.CATEGORY_CODE = '" + item + "' OR ";
                }
                Query = Query.Substring(0, Query.Length - 3) + ") ";
            }



            if (reportFilters.BranchFilter.Count > 0)
            {
                Query += string.Format(@" AND sl.BRANCH_CODE IN ('{0}')", string.Join("','", reportFilters.BranchFilter).ToString());
            }

            //****************************
            //CONDITIONS FITLER END HERE
            //****************************

            if (string.Equals(DateFormat, "AD"))
            {
                Query += @"
                         group by sl.branch_code,bs.branch_edesc,FN_CHARTAD_MONTH(SUBSTR(sales_date,4,3)),TO_CHAR(sales_date,'MON-YYYY'),TO_CHAR(sales_date,'MM'),TO_CHAR(sales_date,'DD')
                         order by TO_CHAR(sales_date,'DD')";
            }
            else
            {
                Query += @"
                         group by  sl.branch_code,bs.branch_edesc,FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SALES_DATE),6,2)),SUBSTR(BS_DATE(SALES_DATE),6,2),SUBSTR(BS_DATE(SALES_DATE),9,2)
                         ORDER BY SUBSTR(BS_DATE(SALES_DATE),9,2)";
            }

            var salesReportList = _objectEntity.SqlQuery<BranchDaysSalesGraph>(Query).ToList();
            return salesReportList;



            //string ad_bs_select = "TO_CHAR(sales_date, 'DD') day, ";
            //string ad_bs_select_new = "TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'dd') day,to_char(fiscal_year.start_date + ROWNUM - 1, 'fmMonth') as month, ";
            //string ad_bs_group = "TO_CHAR(sales_date, 'DD') , ";
            //string ad_bs_group_new = "TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'dd'),";
            //string ad_bs_month_filterCondition = " to_char(fiscal_year.start_date + ROWNUM - 1, 'fmMonth') ";
            //if (string.Equals(DateFormat, "BS"))
            //{
            //    ad_bs_select = "to_char(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),9,2)) day,";
            //    ad_bs_select_new = "to_char(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),9,2)) day,fn_bs_month(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2)) as month, ";
            //    ad_bs_group = "TO_CHAR(sales_date, 'DD') ,to_char(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),9,2)), ";
            //    ad_bs_group_new = "TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'dd'),to_char(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),9,2)),fn_bs_month(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2)),";
            //    ad_bs_month_filterCondition = " fn_bs_month(substr(bs_date(fiscal_year.start_date + ROWNUM - 1),6,2)) ";
            //}

            //string Query = @"SELECT 
            //        si.branch_code,
            //                BS.BRANCH_EDESC as BranchName,
            //                "+ 
            //                 ad_bs_select+
            //                 @"TO_CHAR(sales_date, 'MM') month ,
            //                 fn_bs_month(substr(bs_date(sales_date), 6, 2)) AS Nepalimonth,
            //                   TO_CHAR(sales_date, 'Month') Monthenglish,
            //                 SUM (nvl(si.calc_total_price,0))/{0} AS Amount,
            //                 SUM (nvl(si.calc_Quantity,0))/{0} AS Quantity
            //            FROM sa_sales_invoice si, fa_branch_setup bs,SA_CUSTOMER_SETUP CS,IP_ITEM_MASTER_SETUP IM               
            //    WHERE si.deleted_flag = 'N' AND si.company_code = '{1}'
            //           and SI.BRANCH_CODE = BS.BRANCH_CODE AND SI.CUSTOMER_CODE = CS.CUSTOMER_CODE AND  IM.ITEM_CODE = SI.ITEM_CODE";

            //string QueryNew = @"select * from ( 
            //    SELECT bs.BRANCH_CODE,
            //    bs.branch_edesc as BranchName, 
            //    " +
            //    ad_bs_select_new;
            ////string subQueryAmount = @"(select nvl(SUM (nvl(si.calc_total_price,0))/1,0)
            ////    FROM sa_sales_invoice si, fa_branch_setup bs,SA_CUSTOMER_SETUP CS,IP_ITEM_MASTER_SETUP IM               
            ////    WHERE si.deleted_flag = 'N' AND si.company_code = '01'
            ////    and TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'Month')=TO_CHAR(si.sales_date, 'Month')
            ////    and SI.BRANCH_CODE = BS.BRANCH_CODE AND SI.CUSTOMER_CODE = CS.CUSTOMER_CODE AND  IM.ITEM_CODE = SI.ITEM_CODE ";
            //string subQueryAmount = @"(select
            //     nvl(SUM (nvl(si.calc_total_price,0))/{0},0) as amount
            //    from sa_sales_invoice si,fa_branch_setup bi
            //    WHERE  si.company_code = '{01}' and si.branch_code=bi.branch_code  
            //    and si.branch_code=(select branch_code from fa_branch_setup where branch_edesc='" + branchName + @"'  and company_code='{1}') 
            //    and to_char(fiscal_year.start_date + ROWNUM - 1, 'fmMonth')=to_char(si.sales_date, 'fmMonth') and  
            //    to_char(fiscal_year.start_date + ROWNUM - 1, 'dd')= to_char(si.sales_date, 'dd') 
            //    ";
            ////string subQueryQuantity = @"(select nvl(SUM (nvl(si.calc_Quantity,0))/1,0)
            ////    FROM sa_sales_invoice si, fa_branch_setup bs,SA_CUSTOMER_SETUP CS,IP_ITEM_MASTER_SETUP IM               
            ////    WHERE si.deleted_flag = 'N' AND si.company_code = '01'
            ////    and TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'Month')=TO_CHAR(si.sales_date, 'Month')
            ////    and SI.BRANCH_CODE = BS.BRANCH_CODE AND SI.CUSTOMER_CODE = CS.CUSTOMER_CODE AND  IM.ITEM_CODE = SI.ITEM_CODE ";
            //string subQueryQuantity = @"(select
            //    nvl( SUM (nvl(si.calc_Quantity,0))/{0},0) as Quanity
            //    from sa_sales_invoice si,fa_branch_setup bs
            //    WHERE  si.company_code = '{01}' and si.branch_code=
            //    (select branch_code from fa_branch_setup where branch_edesc='" + branchName + @"'   and company_code='{1}')  
            //    and si.branch_code=bs.branch_code and to_char(fiscal_year.start_date + ROWNUM - 1, 'fmMonth')=
            //    to_char(si.sales_date, 'fmMonth') and  to_char(fiscal_year.start_date + ROWNUM - 1, 'dd')= 
            //    to_char(si.sales_date, 'dd') ";

            //string QueryNewEnd = string.Empty;
            //string condition = string.Empty;
            ////for customer Filter
            //if (!string.IsNullOrEmpty(customerCode))
            //{
            //    Query += " and (";
            //    condition += " and (";
            //    foreach (var item in customerCode.Split(','))
            //    {
            //        Query += "cs.master_customer_code like  (Select MASTER_CUSTOMER_CODE || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND COMPANY_CODE='" + userInfo.company_code + "' ) OR ";
            //        condition += "cs.master_customer_code like  (Select MASTER_CUSTOMER_CODE || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND COMPANY_CODE='" + userInfo.company_code + "' ) OR ";
            //    }
            //    Query = Query.Substring(0, Query.Length - 3) + ") ";
            //    condition = condition.Substring(0, condition.Length - 3) + ") ";
            //}

            ////for item Filter
            //if (!string.IsNullOrEmpty(itemCode))
            //{
            //    Query += " and (";
            //    condition += " and (";
            //    foreach (var item in itemCode.Split(','))
            //    {
            //        Query += "IM.MASTER_ITEM_CODE LIKE (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND COMPANY_CODE='" + userInfo.company_code + "') OR ";
            //        condition += "IM.MASTER_ITEM_CODE LIKE (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND COMPANY_CODE='" + userInfo.company_code + "') OR ";
            //    }
            //    Query = Query.Substring(0, Query.Length - 3) + ") ";
            //    condition = condition.Substring(0, condition.Length - 3) + ") ";
            //}


            ////for category Filter
            //if (!string.IsNullOrEmpty(categoryCode))
            //{
            //    Query += " and (";
            //    condition += " and (";
            //    foreach (var item in categoryCode.Split(','))
            //    {
            //        Query += "IM.CATEGORY_CODE = '" + item + "' OR ";
            //        condition += "IM.CATEGORY_CODE = '" + item + "' OR ";
            //    }
            //    Query = Query.Substring(0, Query.Length - 3) + ") ";
            //    condition = condition.Substring(0, condition.Length - 3) + ") ";
            //}

            ////FOR COMPANY FILTER
            //if (!string.IsNullOrEmpty(companyCode))
            //{
            //    Query += " AND SI.COMPANY_CODE IN (" + companyCode + ")";
            //    condition += " AND SI.COMPANY_CODE IN (" + companyCode + ")";
            //}


            ////FOR BRANCH FILTER
            //if (reportFilters.BranchFilter.Count() > 0)
            //{
            //    Query += " AND SI.BRANCH_CODE IN (" + string.Join(",", reportFilters.BranchFilter) + ")";
            //    condition += " AND SI.BRANCH_CODE IN (" + string.Join(",", reportFilters.BranchFilter) + ")";
            //    try { userInfo.company_code = reportFilters.BranchFilter[0].Split('.')[0]; } catch { }

            //}           


            ////FOR partyType FILTER
            //if (!string.IsNullOrEmpty(partyTypeCode))
            //{
            //    Query += " AND SI.PARTY_TYPE_CODE IN (" + partyTypeCode + ")";
            //    condition += " AND SI.PARTY_TYPE_CODE IN (" + partyTypeCode + ")";
            //}

            ////FOR FORMCODE FILER
            //if (!string.IsNullOrEmpty(formCode))
            //{
            //    Query += " AND SI.FORM_CODE IN (" + formCode + ")";
            //    condition += " AND SI.FORM_CODE IN (" + formCode + ")";
            //}

            //if (!string.IsNullOrEmpty(monthName))
            //{
            //    //condition += " and TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'fmMonth')=TO_CHAR('" + monthName + "') ";
            //    condition+=" and "+ad_bs_month_filterCondition +"= TO_CHAR('" + monthName + "') ";
            //}


            //Query += @"GROUP BY si.branch_code,BS.BRANCH_EDESC, 
            //        "+
            //        ad_bs_group+
            //        @"TO_CHAR(sales_date, 'MM') ,
            //        fn_bs_month(substr(bs_date(sales_date),6,2)),
            //        TO_CHAR(sales_date, 'Month')";

            //if (string.IsNullOrEmpty(branchName))
            //{
            //    QueryNewEnd = @"FROM all_objects, fa_branch_setup bs,
            //       (SELECT start_date, end_date
            //          FROM HR_FISCAL_YEAR_CODE
            //         WHERE sysdate BETWEEN start_date AND end_date) fiscal_year
            //    WHERE ROWNUM <= fiscal_year.end_date - fiscal_year.start_date + 1
            //    group by 
            //        bs.branch_code,
            //        BS.BRANCH_EDESC,
            //        "+
            //        ad_bs_group_new+
            //        @"TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MM'),
            //        fn_bs_month(substr(bs_date(fiscal_year.start_date + ROWNUM - 1), 6, 2)),
            //        TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'fmMonth')";
            //}
            //else
            //{
            //    QueryNewEnd = @"FROM all_objects, fa_branch_setup bs,
            //               (SELECT start_date, end_date
            //                  FROM HR_FISCAL_YEAR_CODE
            //                 WHERE sysdate BETWEEN start_date AND end_date) fiscal_year 
            //         WHERE ROWNUM <= fiscal_year.end_date - fiscal_year.start_date + 1
            //         and  bs.branch_code=(select branch_code from fa_branch_setup where branch_edesc='" + branchName + @"'  and company_code='{1}')
            //         and bs.company_code='{1}'
            //         group by  
            //            " +
            //            ad_bs_group_new+
            //            @"to_char(fiscal_year.start_date + ROWNUM - 1, 'fmMonth'),
            //            bs.BRANCH_CODE,bs.branch_edesc
            //            order by to_char(fiscal_year.start_date + ROWNUM - 1, 'fmMonth')) fa where fa.month='" + monthName + @"' 
            //         ";
            //}


            //Query = QueryNew +
            //    subQueryAmount + condition + ") as Amount," +
            //    subQueryQuantity + condition + ") as Quantity " +
            //    QueryNewEnd;
            //Query = string.Format(Query, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), userInfo.company_code, userInfo.branch_code);

            //var salesReportList = _objectEntity.SqlQuery<BranchDaysSalesGraph>(Query).ToList();
            //return salesReportList;
        }

        public List<StockChartModel> GetMonthlyBranchStockSummary(ReportFiltersModel reportFilters, User userInfo, string dateFormat)
        {

            try
            {
                string Query = string.Format(@"SELECT BRANCH_CODE, FN_FETCH_DESC(COMPANY_CODE, 'FA_BRANCH_SETUP', BRANCH_CODE) BranchName ,
                            FN_FETCH_DESC(COMPANY_CODE, 'COMPANY_SETUP', COMPANY_CODE) AS Companyname,
                            SUM (NVL(IN_QUANTITY,0)-NVL(OUT_QUANTITY,0)) as quantity
                            FROM V$VIRTUAL_STOCK_WIP_LEDGER1
                            WHERE COMPANY_CODE IN(select company_code from company_setup where deleted_flag='N')
                            and deleted_flag='N'
                            and COMPANY_CODE ='{0}'
                            and VOUCHER_DATE <= to_date('{1}','YYYY-MM-DD') "
                           , reportFilters.CompanyFilter.FirstOrDefault(), reportFilters.FromDate);

                ////for Product Filter
                var productFilter = string.Empty;
                if (reportFilters.ProductFilter.Count() > 0)
                {
                    productFilter = @"select  DISTINCT item_code from IP_ITEM_MASTER_SETUP where (";
                    //IF PRODUCT_SKU_FLAG = G
                    foreach (var item in reportFilters.ProductFilter)
                    {
                        productFilter += "MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N') OR ";
                    }
                    productFilter = productFilter.Substring(0, productFilter.Length - 3);
                    //IF PRODUCT_SKU_FLAG = I                
                    productFilter += " or (ITEM_CODE in (" + string.Join(",", reportFilters.ProductFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N')) ";

                    Query += " and ITEM_CODE IN(" + productFilter + ")";
                }

                /////for category Filter
                if (reportFilters.CategoryFilter.Count() > 0)
                {
                    Query += " and CATEGORY_CODE IN('" + string.Join("','", reportFilters.CategoryFilter) + "')";
                }


                Query += @" 
                          GROUP BY BRANCH_CODE,FN_FETCH_DESC(COMPANY_CODE, 'FA_BRANCH_SETUP', BRANCH_CODE),FN_FETCH_DESC(COMPANY_CODE, 'COMPANY_SETUP', COMPANY_CODE)  ";

                List<StockChartModel> resultList = this._objectEntity.SqlQuery<StockChartModel>(Query).ToList();
                return resultList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public List<BalanceChartModel> GetBranchBankBalanceSummary(ReportFiltersModel reportFilters, User userInfo, string dateFormat)
        {

            try
            {
               //// string Query = string.Format(@"SELECT BRANCH_CODE,                          
               //                                FN_FETCH_DESC(COMPANY_CODE,'FA_BRANCH_SETUP',BRANCH_CODE) AS BranchName,                                                   
               //                                ACC_EDESC as BankName,  
               //                                NVL(SUM(NVL( DR_AMOUNT,0))-SUM(NVL(CR_AMOUNT,0)),0)/{2} Amount 
               //                                FROM V$VIRTUAL_GENERAL_LEDGER 
               //                                    WHERE DELETED_FLAG='N'    
               //                                         AND ACC_NATURE in('AC')
               //                                         and COMPANY_CODE ='{0}'
               //                                         and VOUCHER_DATE <= to_date('{1}','YYYY-MM-DD') 
               //                                GROUP BY BRANCH_CODE,FN_FETCH_DESC(COMPANY_CODE,'FA_BRANCH_SETUP',BRANCH_CODE),ACC_EDESC
               //                               ORDER BY BRANCH_CODE"
               //                                        , reportFilters.CompanyFilter.FirstOrDefault(), reportFilters.FromDate, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter));

                var Query = $@"SELECT BRANCH_CODE, BranchName,BankName, SUM(Amount) Amount FROM (                            
    SELECT A.BRANCH_CODE,  C.BRANCH_EDESC BranchName,                                                   
   A.ACC_CODE, DECODE(B.ACC_SNAME, '', B.ACC_EDESC, B.ACC_SNAME) BankName ,
    NVL(SUM(NVL( A.DR_AMOUNT,0))-SUM(NVL(A.CR_AMOUNT,0)),0)/{ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter)} Amount 
    FROM V$VIRTUAL_GENERAL_LEDGER1  A, FA_CHART_OF_ACCOUNTS_SETUP B, FA_BRANCH_SETUP C 
    WHERE   B.ACC_NATURE in('AC')
    and A.COMPANY_CODE ='{reportFilters.CompanyFilter.FirstOrDefault()}'
    and A.COMPANY_CODE = B.COMPANY_CODE
    and A.ACC_CODE = B.ACC_CODE
    and A.COMPANY_CODE = C.COMPANY_CODE
    and A.BRANCH_CODE = C.BRANCH_CODE
    and A.VOUCHER_DATE <= to_date('{reportFilters.FromDate}','YYYY-MM-DD') 
    GROUP BY A.BRANCH_CODE, B.ACC_EDESC, C.BRANCH_EDESC, A.ACC_CODE, B.ACC_SNAME
    ORDER BY BRANCH_CODE)
    GROUP BY BRANCH_CODE, BranchName,BankName";


                List<BalanceChartModel> resultList = this._objectEntity.SqlQuery<BalanceChartModel>(Query).ToList();
                return resultList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<MonthlyLoan> GetMonthlyShortteamLoan(ReportFiltersModel reportFilters, User userInfo, string dateFormat)
        {

            try
            {
                var companyCode = string.Empty;
                foreach (var company in reportFilters.CompanyFilter)
                {
                    companyCode += $@"'{company}',";
                }

                companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
                var Query = $@"SELECT 'current' as DBName,A.MITI as MONTH, A.BALANCE_AMOUNT , A.COMPANY_CODE FROM V_ST_LOAN_MONTHLY_BALANCE A, V_ST_LOAN_MONTHLY_BAL_ROW B
WHERE A.COMPANY_CODE = B.COMPANY_CODE
AND A.SERIAL = B.SERIAL and a.company_code in ({companyCode})";
                if(reportFilters.FiscalYearFilter.Count>0)
                {
                    Query = "";
                    foreach (var DBNAME in reportFilters.FiscalYearFilter)
                    {
                        
                       
                         var Query1 = $@" SELECT '{DBNAME.FiscalYear}' AS DBName,  A.MITI as MONTH, A.BALANCE_AMOUNT , A.COMPANY_CODE FROM {DBNAME.DBName}.V_ST_LOAN_MONTHLY_BALANCE A, {DBNAME.DBName}.V_ST_LOAN_MONTHLY_BAL_ROW B
WHERE A.COMPANY_CODE = B.COMPANY_CODE
AND A.SERIAL = B.SERIAL and a.company_code in ({companyCode}) union all";
                        Query = Query + Query1;

                    }
                    if(Query.EndsWith("union all"))
                    {
                        Query = Query.Substring(0, Query.Length - 10);
                    }
                }


                List<MonthlyLoan> resultList = this._objectEntity.SqlQuery<MonthlyLoan>(Query).ToList();
                return resultList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public List<MonthlyLoan> GetMonthlyLongteamLoan(ReportFiltersModel reportFilters, User userInfo, string dateFormat)
        {

            try
            {
                var companyCode = string.Empty;
                foreach (var company in reportFilters.CompanyFilter)
                {
                    companyCode += $@"'{company}',";
                }

                companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
                var Query = $@"SELECT A.MITI as MONTH, A.BALANCE_AMOUNT , A.COMPANY_CODE FROM  V_LT_LOAN_MONTHLY_BALANCE A, V_LT_LOAN_MONTHLY_BAL_ROW B
WHERE A.COMPANY_CODE = B.COMPANY_CODE
AND A.SERIAL = B.SERIAL and a.company_code in ({companyCode})";
                if (reportFilters.FiscalYearFilter.Count > 0)
                {
                    Query = "";
                    foreach (var DBNAME in reportFilters.FiscalYearFilter)
                    {


                        var Query1 = $@" SELECT '{DBNAME.FiscalYear}' AS DBName,  A.MITI as MONTH, A.BALANCE_AMOUNT , A.COMPANY_CODE FROM {DBNAME.DBName}.V_LT_LOAN_MONTHLY_BALANCE A, {DBNAME.DBName}.V_LT_LOAN_MONTHLY_BAL_ROW B
WHERE A.COMPANY_CODE = B.COMPANY_CODE
AND A.SERIAL = B.SERIAL and a.company_code in ({companyCode}) union all";
                        Query = Query + Query1;

                    }
                    if (Query.EndsWith("union all"))
                    {
                        Query = Query.Substring(0, Query.Length - 10);
                    }
                }

                List<MonthlyLoan> resultList = this._objectEntity.SqlQuery<MonthlyLoan>(Query).ToList();
                return resultList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<BalanceChartModel> GetBranchBankBalanceGroupBySummary(ReportFiltersModel reportFilters, User userInfo, string dateFormat)
        {

            try
            {
                string Query = string.Format(@"SELECT L.BRANCH_CODE,                          
                                               FN_FETCH_DESC(L.COMPANY_CODE,'FA_BRANCH_SETUP',L.BRANCH_CODE) AS BranchName,                                                   
                                               M.ACC_EDESC as BankName,  
                                               NVL(SUM(NVL( L.DR_AMOUNT,0))-SUM(NVL(L.CR_AMOUNT,0)),0)/{2} Amount 
                                               FROM V$VIRTUAL_GENERAL_LEDGER L ,BI_ACCOUNT_MAP M
                                                   WHERE L.DELETED_FLAG='N'    
                                                        AND L.ACC_NATURE in('AC')
                                                        and L.COMPANY_CODE ='{0}'
                                                          AND M.ACC_CODE=L.ACC_CODE
                                                        and L.VOUCHER_DATE <= to_date('{1}','YYYY-MM-DD') 
                                               GROUP BY L.BRANCH_CODE,FN_FETCH_DESC(COMPANY_CODE,'FA_BRANCH_SETUP',BRANCH_CODE),M.ACC_EDESC
                                              ORDER BY BRANCH_CODE"
                                                       , reportFilters.CompanyFilter.FirstOrDefault(), reportFilters.FromDate, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter));


                List<BalanceChartModel> resultList = this._objectEntity.SqlQuery<BalanceChartModel>(Query).ToList();
                return resultList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public List<BalanceChartModel> GetBranchCashBalanceSummary(ReportFiltersModel reportFilters, User userInfo, string dateFormat)
        {

            try
            {
                string Query = string.Format(@"SELECT BRANCH_CODE,                          
                                               FN_FETCH_DESC(COMPANY_CODE,'FA_BRANCH_SETUP',BRANCH_CODE) AS BranchName,                                                   
                                               ACC_EDESC as BankName,  
                                               NVL(SUM(NVL( DR_AMOUNT,0))-SUM(NVL(CR_AMOUNT,0)),0)/{2} Amount 
                                               FROM V$VIRTUAL_GENERAL_LEDGER 
                                                   WHERE DELETED_FLAG='N'    
                                                        AND ACC_NATURE in('AB')
                                                        AND COMPANY_CODE ='{0}'
                                                        AND VOUCHER_DATE <= to_date('{1}','YYYY-MM-DD')
                                                        HAVING NVL(SUM(NVL( DR_AMOUNT,0))-SUM(NVL(CR_AMOUNT,0)),0)/1 <> 0
                                               GROUP BY BRANCH_CODE,FN_FETCH_DESC(COMPANY_CODE,'FA_BRANCH_SETUP',BRANCH_CODE),ACC_EDESC
                                              ORDER BY BRANCH_CODE"
                                                       , reportFilters.CompanyFilter.FirstOrDefault(), reportFilters.FromDate, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter));


                List<BalanceChartModel> resultList = this._objectEntity.SqlQuery<BalanceChartModel>(Query).ToList();
                return resultList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<BalanceChartModel> GetLoanBalanceDetailSummary(ReportFiltersModel reportFilters, string duration, User userInfo)
        {

            try
            {
                var FincalYear = System.Configuration.ConfigurationManager.AppSettings["FiscalYear"].ToString();
                var dateRange = this._controlService.GetDateFilters(FincalYear).OrderByDescending(q => q.SortOrder).ToList();
                var dates = dateRange.Where(x => x.RangeName == duration).FirstOrDefault();

                //var companyCode = string.Join(",", reportFilters.CompanyFilter);
                //companyCode = companyCode == "" ? userInfo.company_code : companyCode;
                var companyCode = string.Empty;
                foreach (var company in reportFilters.CompanyFilter)
                {
                    companyCode += $@"'{company}',";
                }

                companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);

                string branchFilter = string.Empty;
                if (reportFilters.BranchFilter.Count > 0)
                {
                    branchFilter = string.Format(@" AND GL.BRANCH_CODE IN ('{0}')", string.Join("','", reportFilters.BranchFilter).ToString());
                }

                string Query = string.Format(@"SELECT GL.ACC_EDESC as BankName,  
                                                NVL(SUM(NVL(CR_AMOUNT,0))-SUM(NVL(DR_AMOUNT,0)),0) Amount, CASE WHEN  AC.LIMIT =0 then 1 else AC.LIMIT end as LIMIT 
                                               FROM V$VIRTUAL_GENERAL_LEDGER GL,FA_CHART_OF_ACCOUNTS_SETUP AC
                                                   WHERE GL.DELETED_FLAG='N'    
                                                        AND GL.ACC_NATURE in('LC')
                                                        AND GL.COMPANY_CODE IN({0})  {1}
                                                        AND GL.COMPANY_CODE = AC.COMPANY_CODE
                                                        AND GL.ACC_CODE = AC.ACC_CODE  
                                                        AND GL.VOUCHER_DATE >= TO_DATE('{2}','YYYY-MM-DD') AND GL.VOUCHER_DATE <= TO_DATE('{3}','YYYY-MM-DD')
                                                        HAVING NVL(SUM(NVL( DR_AMOUNT,0))-SUM(NVL(CR_AMOUNT,0)),0) < 0
                                               GROUP BY GL.ACC_EDESC,AC.LIMIT
                                              ORDER BY GL.ACC_EDESC", companyCode, branchFilter, dates.StartDateString, dates.EndDateString);


                List<BalanceChartModel> resultList = this._objectEntity.SqlQuery<BalanceChartModel>(Query).ToList();
                return resultList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<BalanceChartModel> GetCashBalanceDetailSummary(ReportFiltersModel reportFilters, User userInfo)
        {

            try
            {
                //var companyCode = string.Join("','", reportFilters.CompanyFilter);
                //companyCode = companyCode == "" ? userInfo.company_code : companyCode;
                var companyCode = string.Empty;
                foreach (var company in reportFilters.CompanyFilter)
                {
                    companyCode += $@"'{company}',";
                }

                companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
                string branchFilter = string.Empty;
                if (reportFilters.BranchFilter.Count > 0)
                {
                    branchFilter = string.Format(@" AND BRANCH_CODE IN ('{0}')", string.Join("','", reportFilters.BranchFilter).ToString());
                }
                string Query = string.Format(@"SELECT ACC_EDESC as BankName,  
                                               NVL(SUM(NVL( DR_AMOUNT,0))-SUM(NVL(CR_AMOUNT,0)),0) Amount 
                                               FROM V$VIRTUAL_GENERAL_LEDGER 
                                                   WHERE DELETED_FLAG='N'    
                                                        AND ACC_NATURE in('AB')
                                                        AND COMPANY_CODE IN({0}) {1}                                                       
                                                        HAVING NVL(SUM(NVL( DR_AMOUNT,0))-SUM(NVL(CR_AMOUNT,0)),0) <> 0
                                               GROUP BY ACC_EDESC
                                              ORDER BY ACC_EDESC", companyCode, branchFilter);


                List<BalanceChartModel> resultList = this._objectEntity.SqlQuery<BalanceChartModel>(Query).ToList();
                return resultList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<BalanceChartModel> GetBankBalanceDetailSummary(ReportFiltersModel reportFilters, User userInfo)
        {

            try
            {
                //var companyCode = string.Join("','", reportFilters.CompanyFilter);
                //companyCode = companyCode == "" ? userInfo.company_code : companyCode;
                var companyCode = string.Empty;
                foreach (var company in reportFilters.CompanyFilter)
                {
                    companyCode += $@"'{company}',";
                }

                companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
                string branchFilter = string.Empty;
                if (reportFilters.BranchFilter.Count > 0)
                {
                    branchFilter = string.Format(@" AND BRANCH_CODE IN ('{0}')", string.Join("','", reportFilters.BranchFilter).ToString());
                }
                string Query = string.Format(@"SELECT ACC_EDESC as BankName,  
                                               NVL(SUM(NVL( DR_AMOUNT,0))-SUM(NVL(CR_AMOUNT,0)),0) Amount 
                                               FROM V$VIRTUAL_GENERAL_LEDGER 
                                                   WHERE DELETED_FLAG='N'    
                                                        AND ACC_NATURE in('AC')
                                                        AND COMPANY_CODE IN({0}) {1}                                                       
                                                        HAVING NVL(SUM(NVL( DR_AMOUNT,0))-SUM(NVL(CR_AMOUNT,0)),0) <> 0
                                               GROUP BY ACC_EDESC
                                              ORDER BY ACC_EDESC", companyCode, branchFilter);


                List<BalanceChartModel> resultList = this._objectEntity.SqlQuery<BalanceChartModel>(Query).ToList();
                return resultList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        

        public List<BalanceChartModel> GetExpenseBalanceSummary(ReportFiltersModel reportFilters, User userInfo)
        {

            try
            {
                //var companyCode = string.Join("','", reportFilters.CompanyFilter);
                //companyCode = companyCode == "" ? userInfo.company_code : companyCode;
                var companyCode = string.Empty;
                foreach (var company in reportFilters.CompanyFilter)
                {
                    companyCode += $@"'{company}',";
                }

                companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
                string branchFilter = string.Empty;
                if (reportFilters.BranchFilter.Count > 0)
                {
                    branchFilter = string.Format(@" AND BRANCH_CODE IN ('{0}')", string.Join("','", reportFilters.BranchFilter).ToString());
                }

                string Query = string.Format(@"SELECT ACC_NATURE as BranchName,  
                                               NVL(SUM(NVL( DR_AMOUNT,0))-SUM(NVL(CR_AMOUNT,0)),0) Amount
                                               FROM V$VIRTUAL_GENERAL_LEDGER 
                                                   WHERE DELETED_FLAG='N'    
                                                        AND ACC_NATURE in('EA','EB','EC')
                                                        AND COMPANY_CODE IN({0}) {1}                                                                                                               
                                               GROUP BY ACC_NATURE
                                              ORDER BY ACC_NATURE", companyCode, branchFilter);


                List<BalanceChartModel> resultList = this._objectEntity.SqlQuery<BalanceChartModel>(Query).ToList();
                return resultList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<BalanceChartModel> GetExpenseBalanceDetailSummary(ReportFiltersModel reportFilters, string accCode, User userInfo)
        {

            try
            {
                var companyCode = string.Join("','", reportFilters.CompanyFilter);
                companyCode = companyCode == "" ? userInfo.company_code : companyCode;
                string Query = string.Format(@"SELECT ACC_EDESC as BankName,  
                                               NVL(SUM(NVL( DR_AMOUNT,0))-SUM(NVL(CR_AMOUNT,0)),0) Amount 
                                               FROM V$VIRTUAL_GENERAL_LEDGER 
                                                   WHERE DELETED_FLAG='N'    
                                                        AND ACC_NATURE in('{1}')
                                                        AND COMPANY_CODE IN('{0}')                                                                                                                
                                               GROUP BY ACC_EDESC
                                              ORDER BY ACC_EDESC", companyCode, accCode);


                List<BalanceChartModel> resultList = this._objectEntity.SqlQuery<BalanceChartModel>(Query).ToList();
                return resultList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<BalanceChartMonthlyModel> GetBankBalanceMonthlySummary(ReportFiltersModel reportFilters, User userInfo, string bankName)
        {

            try
            {
                string Query = string.Format(@"SELECT  TO_CHAR(A.VOUCHER_DATE, 'MON-YYY')  MonthYear, 
                                                     TO_CHAR(A.VOUCHER_DATE, 'YYYYMM')  MonthINT, 
                                                     B.ACC_EDESC as BankName,  
                                                     FN_FETCH_DESC(A.COMPANY_CODE,'FA_BRANCH_SETUP',A.BRANCH_CODE) AS BranchName,
                                                     SUM(NVL( A.DR_AMOUNT,0))/{3} Deposit,
                                                     SUM(NVL(A.CR_AMOUNT,0))/{3} WithDrawn                                             
                                               FROM V$VIRTUAL_GENERAL_LEDGER1 A, FA_CHART_OF_ACCOUNTS_SETUP B
                                               WHERE B.DELETED_FLAG='N'    
                                                    AND B.ACC_NATURE in('AC')
                                                    AND A.COMPANY_CODE=B.COMPANY_CODE
                                                    AND A.BRANCH_CODE=B.BRANCH_CODE
                                                    AND A.ACC_CODE=B.ACC_CODE
                                                    and A.BRANCH_CODE ='{0}' and
                                                    (TRIM(B.ACC_EDESC) = '{2}'
                                                    or   TRIM(B.ACC_SNAME) = '{2}')
                                                    and A.VOUCHER_DATE <= to_date('{1}','YYYY-MM-DD') 
                                               GROUP BY B.ACC_EDESC,TO_CHAR(A.VOUCHER_DATE, 'MON-YYY'),TO_CHAR(A.VOUCHER_DATE, 'YYYYMM') ,FN_FETCH_DESC(A.COMPANY_CODE,'FA_BRANCH_SETUP',A.BRANCH_CODE)
                                               ORDER BY TO_CHAR(A.VOUCHER_DATE, 'YYYYMM')"
                                                       , reportFilters.BranchFilter.FirstOrDefault(), reportFilters.FromDate, bankName, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter));

                List<BalanceChartMonthlyModel> resultList = this._objectEntity.SqlQuery<BalanceChartMonthlyModel>(Query).ToList();
                return resultList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public List<BalanceChartMonthlyModel> GetCashBalanceMonthlySummary(ReportFiltersModel reportFilters, User userInfo, string bankName)
        {

            try
            {
                string Query = string.Format(@"SELECT  TO_CHAR(VOUCHER_DATE, 'MON-YYY')  MonthYear, 
                                                     TO_CHAR(VOUCHER_DATE, 'YYYYMM')  MonthINT, 
                                                     ACC_EDESC as BankName,  
                                                     FN_FETCH_DESC(COMPANY_CODE,'FA_BRANCH_SETUP',BRANCH_CODE) AS BranchName,
                                                     SUM(NVL( DR_AMOUNT,0))/{3} Deposit,
                                                     SUM(NVL(CR_AMOUNT,0))/{3} WithDrawn                                             
                                               FROM V$VIRTUAL_GENERAL_LEDGER 
                                               WHERE DELETED_FLAG='N'    
                                                    AND ACC_NATURE in('AB')
                                                    and BRANCH_CODE ='{0}'
                                                    AND TRIM(ACC_EDESC) = '{2}'
                                                    and VOUCHER_DATE <= to_date('{1}','YYYY-MM-DD') 
                                               GROUP BY ACC_EDESC,TO_CHAR(VOUCHER_DATE, 'MON-YYY'),TO_CHAR(VOUCHER_DATE, 'YYYYMM') ,FN_FETCH_DESC(COMPANY_CODE,'FA_BRANCH_SETUP',BRANCH_CODE)
                                               ORDER BY TO_CHAR(VOUCHER_DATE, 'YYYYMM')"
                                                       , reportFilters.BranchFilter.FirstOrDefault(), reportFilters.FromDate, bankName, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter));

                List<BalanceChartMonthlyModel> resultList = this._objectEntity.SqlQuery<BalanceChartMonthlyModel>(Query).ToList();
                return resultList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<StockChartModel> GetStockCategorySummary(ReportFiltersModel reportFilters, User userInfo, string dateFormat)
        {

            try
            {
                string Query = string.Format(@"SELECT 
                            FN_FETCH_DESC(COMPANY_CODE, 'COMPANY_SETUP', COMPANY_CODE) AS Companyname,
                            FN_FETCH_DESC(COMPANY_CODE, 'FA_BRANCH_SETUP', BRANCH_CODE) BranchName ,
                            BRANCH_CODE AS Branch_Code, Category_Code , FN_FETCH_DESC(COMPANY_CODE, 'IP_CATEGORY_CODE', Category_Code) CategoryName ,                                 
                            SUM (NVL(IN_QUANTITY,0)-NVL(OUT_QUANTITY,0)) as quantity
                            FROM V$VIRTUAL_STOCK_WIP_LEDGER1
                            WHERE COMPANY_CODE IN(select company_code from company_setup where deleted_flag='N')
                            and deleted_flag='N'                            
                            and BRANCH_CODE = '{0}'
                            and VOUCHER_DATE <= to_date('{1}','YYYY-MM-DD')  "
                           , reportFilters.BranchFilter.FirstOrDefault(), reportFilters.FromDate);

                ////for Product Filter
                var productFilter = string.Empty;
                if (reportFilters.ProductFilter.Count() > 0)
                {
                    productFilter = @"select  DISTINCT item_code from IP_ITEM_MASTER_SETUP where (";
                    //IF PRODUCT_SKU_FLAG = G
                    foreach (var item in reportFilters.ProductFilter)
                    {
                        productFilter += "MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N') OR ";
                    }
                    productFilter = productFilter.Substring(0, productFilter.Length - 3);
                    //IF PRODUCT_SKU_FLAG = I                
                    productFilter += " or (ITEM_CODE in (" + string.Join(",", reportFilters.ProductFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N')) ";

                    Query += " and ITEM_CODE IN(" + productFilter + ")";
                }

                /////for category Filter
                if (reportFilters.CategoryFilter.Count() > 0)
                {
                    Query += " and CATEGORY_CODE IN('" + string.Join("','", reportFilters.CategoryFilter) + "')";
                }


                Query += @" 
                          GROUP BY BRANCH_CODE,Category_Code,FN_FETCH_DESC(COMPANY_CODE, 'IP_CATEGORY_CODE', Category_Code),
                         FN_FETCH_DESC(COMPANY_CODE, 'COMPANY_SETUP', COMPANY_CODE),FN_FETCH_DESC(COMPANY_CODE, 'FA_BRANCH_SETUP', BRANCH_CODE)";

                List<StockChartModel> resultList = this._objectEntity.SqlQuery<StockChartModel>(Query).ToList();
                return resultList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public List<StockChartModel> GetStockItemSummary(ReportFiltersModel reportFilters, User userInfo, string dateFormat)
        {

            try
            {
                string Query = string.Format(@"SELECT 
                             FN_FETCH_DESC(COMPANY_CODE, 'COMPANY_SETUP', COMPANY_CODE) AS Companyname,
                             FN_FETCH_DESC(COMPANY_CODE, 'FA_BRANCH_SETUP', BRANCH_CODE) BranchName ,
                             FN_FETCH_DESC(COMPANY_CODE, 'IP_CATEGORY_CODE', CATEGORY_CODE) CategoryName ,
                            Item_Code , FN_FETCH_DESC(COMPANY_CODE, 'IP_ITEM_MASTER_SETUP', Item_Code) ItemName ,
                            SUM (NVL(IN_QUANTITY,0)-NVL(OUT_QUANTITY,0)) as quantity
                            FROM V$VIRTUAL_STOCK_WIP_LEDGER1
                            WHERE COMPANY_CODE IN(select company_code from company_setup where deleted_flag='N')
                            and deleted_flag='N'                            
                            and BRANCH_CODE = '{1}'
                            AND Category_Code = '{0}'
                            and VOUCHER_DATE <= to_date('{2}','YYYY-MM-DD') "
                           , reportFilters.CategoryFilter.FirstOrDefault(), reportFilters.BranchFilter.FirstOrDefault(), reportFilters.FromDate);

                ////for Product Filter
                var productFilter = string.Empty;
                if (reportFilters.ProductFilter.Count() > 0)
                {
                    productFilter = @"select  DISTINCT item_code from IP_ITEM_MASTER_SETUP where (";
                    //IF PRODUCT_SKU_FLAG = G
                    foreach (var item in reportFilters.ProductFilter)
                    {
                        productFilter += "MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N') OR ";
                    }
                    productFilter = productFilter.Substring(0, productFilter.Length - 3);
                    //IF PRODUCT_SKU_FLAG = I                
                    productFilter += " or (ITEM_CODE in (" + string.Join(",", reportFilters.ProductFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N')) ";

                    Query += " and ITEM_CODE IN(" + productFilter + ")";
                }

                /////for category Filter
                if (reportFilters.CategoryFilter.Count() > 0)
                {
                    Query += " and CATEGORY_CODE IN('" + string.Join("','", reportFilters.CategoryFilter) + "')";
                }


                Query += @" 
                          GROUP BY Item_Code,FN_FETCH_DESC(COMPANY_CODE, 'IP_ITEM_MASTER_SETUP', Item_Code),
                            FN_FETCH_DESC(COMPANY_CODE, 'COMPANY_SETUP', COMPANY_CODE),FN_FETCH_DESC(COMPANY_CODE, 'FA_BRANCH_SETUP', BRANCH_CODE), FN_FETCH_DESC(COMPANY_CODE, 'IP_CATEGORY_CODE', CATEGORY_CODE)";

                List<StockChartModel> resultList = this._objectEntity.SqlQuery<StockChartModel>(Query).ToList();
                return resultList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<MonthlyBranchExpenseGraph> GetMonthlyBranchExpenseSummary(ReportFiltersModel reportFilters, User userInfo, string dateFormat, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode)
        {

            // old query implemented as new query took more exicution time


            companyCode = companyCode != null ? companyCode : string.Join(",", reportFilters.CompanyFilter);
            companyCode = companyCode == "" ? userInfo.company_code : companyCode;

            //**********************************************************************************            
            string Query = string.Empty;
            if (string.Equals(dateFormat, "AD"))
            {
                Query = string.Format(@"SELECT BRANCH_CODE,  
                                             FN_FETCH_DESC(COMPANY_CODE, 'FA_BRANCH_SETUP', BRANCH_CODE) BranchName,
                                             TO_CHAR(VOUCHER_DATE, 'MON') MONTH,                       
                                             TO_CHAR(VOUCHER_DATE, 'YYYYMM') NepaliMonthInt,
                                            TO_CHAR(VOUCHER_DATE, 'MON-YYY')  MonthYear, 
                                 (sum(nvl(dr_amount,0))-sum(nvl(cr_amount,0)))/{0} Amount
                                FROM V$VIRTUAL_GENERAL_LEDGER 
                                WHERE COMPANY_CODE = '{1}'
                                and deleted_flag='N'
                                   AND ACC_NATURE in('EC','EB')
                            ", ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode);
            }
            else
            {
                Query = string.Format(@"SELECT BRANCH_CODE,  
                                         FN_FETCH_DESC(COMPANY_CODE, 'FA_BRANCH_SETUP', BRANCH_CODE) BranchName,
                                        SUBSTR(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(VOUCHER_DATE),6,2)),6,20) MONTH, 
                                        SUBSTR(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(VOUCHER_DATE),6,2)),6,20) Nepalimonth, 
                                        SUBSTR(BS_DATE(VOUCHER_DATE),6,2) NepaliMonthInt,
                                        SUBSTR(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(VOUCHER_DATE),6,2)),6,20) MonthYear, 
                             (sum(nvl(dr_amount,0))-sum(nvl(cr_amount,0)))/{0} Amount
                            FROM V$VIRTUAL_GENERAL_LEDGER 
                            WHERE COMPANY_CODE = '{1}'
                            and deleted_flag='N'
                               AND ACC_NATURE in('EC','EB')
                            ",
                                            ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode);
            }

            //****************************
            //CONDITIONS FITLER START HERE
            //****************************
            // no customer_code in IP_PRODUCTION_MRR
            //for customer Filter
            //var customerFilter = string.Empty;
            //if (reportFilters.CustomerFilter.Count() > 0)
            //{
            //    customerFilter = @"select  DISTINCT(customer_code) from sa_customer_setup where (";
            //    //IF CUSTOMER_SKU_FLAG = G
            //    foreach (var item in reportFilters.CustomerFilter)
            //    {
            //        customerFilter += "master_customer_code like  (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
            //    }
            //    customerFilter = customerFilter.Substring(0, customerFilter.Length - 3);
            //    //IF CUSTOMER_SKU_FLAG = I                
            //    customerFilter += " or (customer_code in (" + string.Join(",", reportFilters.CustomerFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


            //    Query += " and sl.customer_code IN(" + customerFilter + ")";
            //}




            ////for Product Filter
            //var productFilter = string.Empty;
            //if (reportFilters.ProductFilter.Count() > 0)
            //{
            //    productFilter = @"select  DISTINCT item_code from IP_ITEM_MASTER_SETUP where (";
            //    //IF PRODUCT_SKU_FLAG = G
            //    foreach (var item in reportFilters.ProductFilter)
            //    {
            //        productFilter += "MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
            //    }
            //    productFilter = productFilter.Substring(0, productFilter.Length - 3);
            //    //IF PRODUCT_SKU_FLAG = I                
            //    productFilter += " or (ITEM_CODE in (" + string.Join(",", reportFilters.ProductFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


            //    Query += " and ITEM_CODE IN(" + productFilter + ")";
            //}


            //if (reportFilters.BranchFilter.Count > 0)
            //{
            //    Query += string.Format(@" AND BRANCH_CODE IN ('{0}')", string.Join("','", reportFilters.BranchFilter).ToString());
            //}

            //****************************
            //CONDITIONS FITLER END HERE
            //****************************

            if (string.Equals(dateFormat, "AD"))
            {
                Query += @"
                          GROUP BY BRANCH_CODE, FN_FETCH_DESC(COMPANY_CODE, 'FA_BRANCH_SETUP', BRANCH_CODE),TO_CHAR(VOUCHER_DATE, 'MON'),TO_CHAR(VOUCHER_DATE, 'YYYYMM'),TO_CHAR(VOUCHER_DATE, 'MON-YYY')
                          ORDER BY BRANCH_CODE,TO_CHAR(VOUCHER_DATE, 'YYYYMM')";
            }
            else
            {
                Query += @"
                           GROUP BY FN_CHARTBS_MONTH(SUBSTR(BS_DATE(VOUCHER_DATE),6,2)),SUBSTR(BS_DATE(VOUCHER_DATE),6,2),FN_FETCH_DESC(COMPANY_CODE, 'FA_BRANCH_SETUP', BRANCH_CODE),BRANCH_CODE
                           ORDER BY BRANCH_CODE,FN_CHARTBS_MONTH(SUBSTR(BS_DATE(VOUCHER_DATE),6,2))";
            }

            var salesReportList = _objectEntity.SqlQuery<MonthlyBranchExpenseGraph>(Query).ToList();
            return salesReportList;

        }


        public List<BranchDaysExpenseGraph> GetDailyBranchExpenseSummary(ReportFiltersModel reportFilters, User userInfo, string monthName, string branchName, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode, string DateFormat)
        {
            companyCode = companyCode != null ? companyCode : string.Join(",", reportFilters.CompanyFilter);
            companyCode = companyCode == "" ? userInfo.company_code : companyCode;



            var Query = string.Empty;
            if (string.Equals(DateFormat, "AD"))
            {
                //Query = string.Format(@"SELECT acc_edesc as Acc_Edesc,  
                //             (sum(nvl(dr_amount,0))-sum(nvl(cr_amount,0)))/{0} Amount
                //            FROM V$VIRTUAL_GENERAL_LEDGER 
                //            WHERE COMPANY_CODE = '{1}'
                //            and branch_code = '{3}'
                //             and  TO_CHAR(VOUCHER_DATE, 'YYYYMM') = '{2}'
                //            and deleted_flag='N'
                //               AND ACC_NATURE in('EC','EB','EA')"
                //              , ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode, monthName, branchName);
                // this Query for month day and expensise detail
                Query = string.Format(@"SELECT   acc_edesc as Acc_Edesc,
                                         FN_FETCH_DESC(COMPANY_CODE, 'FA_BRANCH_SETUP', BRANCH_CODE) BranchName,
                                         TO_CHAR(VOUCHER_DATE, 'MON') MONTH,                       
                                         TO_CHAR(VOUCHER_DATE, 'YYYYMM') MonthNo,
                                        TO_CHAR(VOUCHER_DATE, 'DD')  Day, 
                             (sum(nvl(dr_amount,0))-sum(nvl(cr_amount,0)))/{0} Amount
                            FROM V$VIRTUAL_GENERAL_LEDGER 
                            WHERE COMPANY_CODE = '{1}'
                            and branch_code = '{3}'
                             and  TO_CHAR(VOUCHER_DATE, 'YYYYMM') = '{2}'
                            and deleted_flag='N'
                               AND ACC_NATURE in('EC','EB')"
                              , ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode, monthName, branchName);
            }
            else
            {
                //Query = string.Format(@"SELECT  acc_edesc as Acc_Edesc, 
                //             (sum(nvl(dr_amount,0))-sum(nvl(cr_amount,0)))/{0} Amount
                //            FROM V$VIRTUAL_GENERAL_LEDGER 
                //            WHERE COMPANY_CODE = '{1}'
                //            and branch_code = '{3}'
                //           AND SUBSTR(BS_DATE(VOUCHER_DATE),6,2)= '{2}'
                //            and deleted_flag='N'
                //               AND ACC_NATURE in('EC','EB','EA')"
                //             , ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode, monthName, branchName);
                // this Query for month day and expensise detail
                Query = string.Format(@"SELECT  acc_edesc as Acc_Edesc, 
                                         FN_FETCH_DESC(COMPANY_CODE, 'FA_BRANCH_SETUP', BRANCH_CODE) BranchName,
                                          TRIM(SUBSTR(FN_CHARTBS_MONTH(SUBSTR(BS_DATE(VOUCHER_DATE),6,2)),6,20)) MONTH,
                                           trim(substr((BS_DATE(VOUCHER_DATE)),0,7)) BsYear_Month,                    
                                         SUBSTR(BS_DATE(VOUCHER_DATE),6,2) MonthNo,
                                        SUBSTR(BS_DATE(VOUCHER_DATE),9,2) Day, 
                             (sum(nvl(dr_amount,0))-sum(nvl(cr_amount,0)))/{0} Amount
                            FROM V$VIRTUAL_GENERAL_LEDGER 
                            WHERE COMPANY_CODE = '{1}'
                            and branch_code = '{3}'
                           AND SUBSTR(BS_DATE(VOUCHER_DATE),6,2)= '{2}'
                            and deleted_flag='N'
                               AND ACC_NATURE in('EC','EB')"
                              , ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode, monthName, branchName);
            }



            //****************************
            //CONDITIONS FITLER START HERE
            //****************************

            //for customer Filter
            //if (reportFilters.CustomerFilter.Count() > 0)
            //{
            //    Query += " and (";
            //    //IF CUSTOMER_SKU_FLAG = G
            //    foreach (var item in reportFilters.CustomerFilter)
            //    {
            //        Query += "cs.master_customer_code like  (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND COMPANY_CODE IN(" + companyCode + ") ) OR ";
            //    }
            //    //IF CUSTOMER_SKU_FLAG = I                
            //    Query += "(CUSTOMER_CODE IN (" + string.Join(",", reportFilters.CustomerFilter) + ") AND cs.GROUP_SKU_FLAG = 'I' AND cs.COMPANY_CODE IN(" + companyCode + ") )) ";

            //    Query = Query.Substring(0, Query.Length - 1);
            //}

            //////for Product Filter
            //if (reportFilters.ProductFilter.Count() > 0)
            //{
            //    Query += " and (";
            //    //IF PRODUCT_SKU_FLAG = G
            //    foreach (var item in reportFilters.ProductFilter)
            //    {
            //        Query += "IM.MASTER_ITEM_CODE like  (Select DISTINCT(MASTER_ITEM_CODE) || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND COMPANY_CODE IN(" + companyCode + ") ) OR ";
            //    }
            //    //IF PRODUCT_SKU_FLAG = I                
            //    Query += "(ITEM_CODE IN (" + string.Join(",", reportFilters.ProductFilter) + ") AND IM.GROUP_SKU_FLAG = 'I')) ";

            //    Query = Query.Substring(0, Query.Length - 1);
            //}


            //for category Filter
            //if (reportFilters.CategoryFilter.Count() > 0)
            //{
            //    Query += " and (";
            //    foreach (var item in reportFilters.CategoryFilter)
            //    {
            //        Query += "IM.CATEGORY_CODE = '" + item + "' OR ";
            //    }
            //    Query = Query.Substring(0, Query.Length - 3) + ") ";
            //}



            //if (reportFilters.BranchFilter.Count > 0)
            //{
            //    Query += string.Format(@" AND sl.BRANCH_CODE IN ('{0}')", string.Join("','", reportFilters.BranchFilter).ToString());
            //}

            //****************************
            //CONDITIONS FITLER END HERE
            //****************************

            if (string.Equals(DateFormat, "AD"))
            {
                //Query += @"
                //           GROUP BY acc_edesc";
                // this Query for month day and expensise detail
                Query += @"
                           GROUP BY  FN_FETCH_DESC(COMPANY_CODE, 'FA_BRANCH_SETUP', BRANCH_CODE),TO_CHAR(VOUCHER_DATE, 'MON'),TO_CHAR(VOUCHER_DATE, 'YYYYMM'),TO_CHAR(VOUCHER_DATE, 'DD'),acc_edesc
                           ORDER BY TO_CHAR(VOUCHER_DATE, 'DD')";
            }
            else
            {
                //Query += @"
                //           GROUP BY acc_edesc";
                // this Query for month day and expensise detail
                Query += @"
                           GROUP BY  FN_FETCH_DESC(COMPANY_CODE, 'FA_BRANCH_SETUP', BRANCH_CODE),SUBSTR(BS_DATE(VOUCHER_DATE),6,2), SUBSTR(BS_DATE(VOUCHER_DATE),9,2),substr((BS_DATE(VOUCHER_DATE)),0,7),acc_edesc
                           ORDER BY SUBSTR(BS_DATE(VOUCHER_DATE),9,2)";
            }

            var salesReportList = _objectEntity.SqlQuery<BranchDaysExpenseGraph>(Query).ToList();
            #region all day wise
            //var sales = salesReportList.FirstOrDefault();

            //if (sales != null)
            //{
            //    if (string.Equals(DateFormat, "AD"))
            //    {
            //        int year = 2016;
            //        int month = 06;
            //        int.TryParse(monthName.Substring(0, 4), out year);
            //        int.TryParse(monthName.Substring(4, 2), out month);
            //        int daysInMonth = DateTime.DaysInMonth(year, month);
            //        for (int i = 1; i <= daysInMonth; i++)
            //        {
            //            var checkday = i.ToString().PadLeft(2, '0');
            //            if (salesReportList.Any(x => x.day == checkday))
            //                continue;
            //            var branchtest = new BranchDaysExpenseGraph()
            //            {
            //                day = checkday,
            //                Month = sales.Month,
            //                MonthNo = sales.MonthNo,                           
            //                Amount = 0,
            //                BranchName = sales.BranchName,
            //            };
            //            salesReportList.Add(branchtest);
            //        }
            //    }
            //    else if (string.Equals(DateFormat, "BS"))
            //    {
            //        int daysInMonth = 0;
            //        var monthday = this._objectEntity.SqlQuery<int>("select Days_no from calendar_setup Where bs_month='" + sales.BsYear_Month + "'").FirstOrDefault();
            //        int.TryParse(monthday.ToString(), out daysInMonth);
            //        if (daysInMonth > 0)
            //        {
            //            for (int i = 1; i <= daysInMonth; i++)
            //            {
            //                var checkday = i.ToString().PadLeft(2, '0');
            //                if (salesReportList.Any(x => x.day == checkday))
            //                    continue;
            //                var branchtest = new BranchDaysExpenseGraph()
            //                {
            //                    day = checkday,
            //                    Month = sales.Month,
            //                    MonthNo = sales.MonthNo,                               
            //                    Amount = 0,
            //                    BranchName = sales.BranchName,
            //                };
            //                salesReportList.Add(branchtest);
            //            }
            //        }
            //    }
            //}
            #endregion
            return salesReportList;
        }




        public List<BranchDaysProductionGraph> GetDailyBranchProductionSummary(ReportFiltersModel reportFilters, User userInfo, string monthName, string branchName,
            string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode,
            string partyTypeCode, string formCode, string DateFormat)
        {
            companyCode = string.Join(",", reportFilters.CompanyFilter);
            companyCode = companyCode == "" ? userInfo.company_code : companyCode;

            var Query = string.Empty;
            if (string.Equals(DateFormat, "AD"))
            {
                Query = string.Format(@"SELECT 
                                branch_code,
                                FN_FETCH_DESC(COMPANY_CODE, 'FA_BRANCH_SETUP', BRANCH_CODE) BranchName,
                                SUBSTR(to_char(MRR_DATE,'yyyy-MM-dd'),9,2) DAY,
                                substr(to_char(MRR_DATE,'yyyy-MM-dd'),6,2) month,
                                SUM (NVL(QUANTITY,0))/{0} as Quantity
                            FROM IP_PRODUCTION_MRR 
                            WHERE COMPANY_CODE = '{1}'
                            and substr(to_char(MRR_DATE,'yyyy-MM-dd'),0,4)||substr(to_char(MRR_DATE,'yyyy-MM-dd'),6,2)='{2}'
                            and branch_code = '{3}'
                            "
                              , ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode, monthName, branchName);
            }
            else
            {
                Query = string.Format(@"SELECT 
                                    branch_code,
                                    FN_FETCH_DESC(COMPANY_CODE, 'FA_BRANCH_SETUP', BRANCH_CODE) BranchName,
                                    SUBSTR(BS_DATE(MRR_DATE),9,2) DAY,
                                    SUBSTR(BS_DATE(MRR_DATE),6,2) month,
                                    SUM (NVL(QUANTITY,0))/{0} as Quantity,
                                    trim(substr((BS_DATE(MRR_DATE)),0,7)) BsYear_Month
                                FROM IP_PRODUCTION_MRR 
                                WHERE COMPANY_CODE = '{1}'
                                and SUBSTR(BS_DATE(MRR_DATE),6,2)='{2}' and branch_code = '{3}'
                               ",
                                             ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode, monthName, branchName);
            }



            //****************************
            //CONDITIONS FITLER START HERE
            //****************************

            //for customer Filter
            //if (reportFilters.CustomerFilter.Count() > 0)
            //{
            //    Query += " and (";
            //    //IF CUSTOMER_SKU_FLAG = G
            //    foreach (var item in reportFilters.CustomerFilter)
            //    {
            //        Query += "cs.master_customer_code like  (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND COMPANY_CODE IN(" + companyCode + ") ) OR ";
            //    }
            //    //IF CUSTOMER_SKU_FLAG = I                
            //    Query += "(CUSTOMER_CODE IN (" + string.Join(",", reportFilters.CustomerFilter) + ") AND cs.GROUP_SKU_FLAG = 'I' AND cs.COMPANY_CODE IN(" + companyCode + ") )) ";

            //    Query = Query.Substring(0, Query.Length - 1);
            //}

            //////for Product Filter
            //if (reportFilters.ProductFilter.Count() > 0)
            //{
            //    Query += " and (";
            //    //IF PRODUCT_SKU_FLAG = G
            //    foreach (var item in reportFilters.ProductFilter)
            //    {
            //        Query += "IM.MASTER_ITEM_CODE like  (Select DISTINCT(MASTER_ITEM_CODE) || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND COMPANY_CODE IN(" + companyCode + ") ) OR ";
            //    }
            //    //IF PRODUCT_SKU_FLAG = I                
            //    Query += "(ITEM_CODE IN (" + string.Join(",", reportFilters.ProductFilter) + ") AND IM.GROUP_SKU_FLAG = 'I')) ";

            //    Query = Query.Substring(0, Query.Length - 1);
            //}


            //for category Filter
            //if (reportFilters.CategoryFilter.Count() > 0)
            //{
            //    Query += " and (";
            //    foreach (var item in reportFilters.CategoryFilter)
            //    {
            //        Query += "IM.CATEGORY_CODE = '" + item + "' OR ";
            //    }
            //    Query = Query.Substring(0, Query.Length - 3) + ") ";
            //}



            //if (reportFilters.BranchFilter.Count > 0)
            //{
            //    Query += string.Format(@" AND sl.BRANCH_CODE IN ('{0}')", string.Join("','", reportFilters.BranchFilter).ToString());
            //}

            //****************************
            //CONDITIONS FITLER END HERE
            //****************************

            if (string.Equals(DateFormat, "AD"))
            {
                Query += @"GROUP BY 
                                branch_code,
                                FN_FETCH_DESC(COMPANY_CODE, 'FA_BRANCH_SETUP', BRANCH_CODE),
                                SUBSTR(to_char(MRR_DATE,'yyyy-MM-dd'),9,2) ,
                                substr(to_char(MRR_DATE,'yyyy-MM-dd'),6,2)
                            ORDER BY FN_FETCH_DESC(COMPANY_CODE, 'FA_BRANCH_SETUP', BRANCH_CODE) ,substr(to_char(MRR_DATE,'yyyy-MM-dd'),6,2)";
            }
            else
            {
                Query += @"
                          GROUP BY 
                                branch_code,
                                    SUBSTR(BS_DATE(MRR_DATE),9,2) ,
                                    SUBSTR(BS_DATE(MRR_DATE),6,2),
                                    FN_FETCH_DESC(COMPANY_CODE, 'FA_BRANCH_SETUP', BRANCH_CODE),
                                    trim(substr((BS_DATE(MRR_DATE)),0,7))
                                ORDER BY 
                                FN_FETCH_DESC(COMPANY_CODE, 'FA_BRANCH_SETUP', BRANCH_CODE) ,
                                SUBSTR(BS_DATE(MRR_DATE),9,2)";
            }

            var salesReportList = _objectEntity.SqlQuery<BranchDaysProductionGraph>(Query).ToList();
            var sales = salesReportList.FirstOrDefault();

            if (sales != null)
            {
                if (string.Equals(DateFormat, "AD"))
                {
                    int year = 2016;
                    int month = 06;
                    int.TryParse(monthName.Substring(0, 4), out year);
                    int.TryParse(monthName.Substring(4, 2), out month);
                    int daysInMonth = DateTime.DaysInMonth(year, month);
                    for (int i = 1; i <= daysInMonth; i++)
                    {
                        var checkday = i.ToString().PadLeft(2, '0');
                        if (salesReportList.Any(x => x.day == checkday))
                            continue;
                        var branchtest = new BranchDaysProductionGraph()
                        {
                            day = checkday,
                            Month = sales.Month,
                            MonthNo = sales.MonthNo,
                            Quantity = 0,
                            Amount = 0,
                            BranchName = sales.BranchName,
                        };
                        salesReportList.Add(branchtest);
                    }
                }
                else if (string.Equals(DateFormat, "BS"))
                {
                    int daysInMonth = 0;
                    var monthday = this._objectEntity.SqlQuery<int>("select Days_no from calendar_setup Where bs_month='" + sales.BsYear_Month + "'").FirstOrDefault();
                    int.TryParse(monthday.ToString(), out daysInMonth);
                    if (daysInMonth > 0)
                    {
                        for (int i = 1; i <= daysInMonth; i++)
                        {
                            var checkday = i.ToString().PadLeft(2, '0');
                            if (salesReportList.Any(x => x.day == checkday))
                                continue;
                            var branchtest = new BranchDaysProductionGraph()
                            {
                                day = checkday,
                                Month = sales.Month,
                                MonthNo = sales.MonthNo,
                                Quantity = 0,
                                Amount = 0,
                                BranchName = sales.BranchName,
                            };
                            salesReportList.Add(branchtest);
                        }
                    }
                }
            }
            return salesReportList;
        }

        public List<StockValutionViewModel> GetStockValutionSummary(filterOption reportFilters, User userInfo)
        {
            var companyCode = "01";
            companyCode = string.Join(",", reportFilters.ReportFilters.CompanyFilter);
            companyCode = companyCode == "" ? userInfo.company_code : companyCode;
            var query = @"SELECT * FROM (
                             SELECT SUM (NVL(IN_QUANTITY, 0) * NVL(IN_UNIT_PRICE, 0))
                             - SUM(NVL(OUT_QUANTITY, 0) * NVL(OUT_UNIT_PRICE, 0)) Amount,
                              CC.CATEGORY_EDESC AS CatagoryName,
                              SL.CATEGORY_CODE AS CatagoryCode,
                              SL.METHOD AS Method
                              FROM V$VIRTUAL_VALUE_STOCK_LEDGER SL, IP_CATEGORY_CODE CC
                               WHERE SL.CATEGORY_CODE = CC.CATEGORY_CODE
                                 and SL.COMPANY_CODE = CC.COMPANY_CODE
                              AND SL.DELETED_FLAG = 'N'
                       AND SL.COMPANY_CODE IN(" + companyCode + @")";
            if (reportFilters.ReportFilters.BranchFilter.Count > 0)
            {
                query += string.Format(@" AND SL.BRANCH_CODE IN ('{0}')", string.Join("','", reportFilters.ReportFilters.BranchFilter).ToString());
            }
            query += " GROUP BY  CC.CATEGORY_EDESC, SL.CATEGORY_CODE, SL.METHOD) WHERE Amount <> 0";
            var data = _objectEntity.SqlQuery<StockValutionViewModel>(query).ToList();
            return data;
        }

        public List<StockValutionViewModel> GetProductStockValutions(filterOption reportFilters, User UserInfo, string CatagoryCode)
        {
            var companyCode = "01";
            companyCode = string.Join(",", reportFilters.ReportFilters.CompanyFilter);
            companyCode = companyCode == "" ? UserInfo.company_code : companyCode;
            var query = @"select * from (SELECT SUM (NVL(IN_QUANTITY,0) * NVL(IN_UNIT_PRICE,0))
                      - SUM (NVL(OUT_QUANTITY,0) *NVL( OUT_UNIT_PRICE,0)) Amount,
                     IMS.ITEM_EDESC as CatagoryName,
                    IMS.ITEM_CODE AS CatagoryCode,
                      SL.METHOD AS Method
                    FROM V$VIRTUAL_VALUE_STOCK_LEDGER SL, IP_ITEM_MASTER_SETUP IMS
                   WHERE  SL.CATEGORY_CODE = '" + CatagoryCode + @"'
              AND SL.ITEM_CODE = IMS.ITEM_CODE
             AND SL.COMPANY_CODE=IMS.COMPANY_CODE AND SL.COMPANY_CODE IN (" + companyCode + ")";
            if (reportFilters.ReportFilters.BranchFilter.Count > 0)
            {
                query += string.Format(@" AND SL.BRANCH_CODE IN ('{0}')", string.Join("','", reportFilters.ReportFilters.BranchFilter).ToString());
            }
            query += "  GROUP BY   IMS.ITEM_EDESC,IMS.ITEM_CODE, SL.METHOD) where Amount<>0 ";
            var data = _objectEntity.SqlQuery<StockValutionViewModel>(query).ToList();
            return data;

        }

        #region DivisonWise OutStanding 

        public List<DivisionOutStanding> GetDivisionWiseOutStanding(filterOption reportFilers, User userInfo)
        {
            string companyCode = string.Join(",", reportFilers.ReportFilters.CompanyFilter);

            var Query = @"SELECT VS.DIVISION_CODE as DivisonId,SUM(nvl(VS.DR_AMOUNT,0))-SUM(nvl(VS.CR_AMOUNT,0)) AS Amount,D.DIVISION_EDESC as DivisionName FROM V$VIRTUAL_SUB_LEDGER VS,FA_DIVISION_SETUP  D WHERE VS.COMPANY_CODE='01' 
    AND VS.VOUCHER_DATE <= sysdate AND VS.SUB_LEDGER_FLAG = 'C'
    AND VS.DELETED_FLAG = 'N'
    AND VS.DIVISION_CODE = D.DIVISION_CODE(+)
    AND VS.COMPANY_CODE = D.COMPANY_CODE(+)
    {0}
    {1}
    GROUP BY VS.DIVISION_CODE,D.DIVISION_EDESC";
            string customerFilter = string.Empty;
            if (reportFilers.ReportFilters.CustomerFilter.Count > 0)
            {
                customerFilter += @"AND VS.SUB_CODE IN (SELECT DISTINCT TRIM(LINK_SUB_CODE) FROM SA_CUSTOMER_SETUP WHERE 
                                    CUSTOMER_CODE IN (SELECT CUSTOMER_CODE FROM SA_CUSTOMER_SETUP WHERE";
                foreach (var item in reportFilers.ReportFilters.CustomerFilter)
                {
                    customerFilter += " MASTER_CUSTOMER_CODE LIKE (SELECT DISTINCT(MASTER_CUSTOMER_CODE) || '%'  FROM SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                customerFilter = customerFilter.Substring(0, customerFilter.Length - 3);
                customerFilter += " OR CUSTOMER_CODE IN (" + string.Join(",", reportFilers.ReportFilters.CustomerFilter) + ") AND GROUP_SKU_FLAG = 'I' AND COMPANY_CODE IN(" + companyCode + ")))";
            }
            string itemFilter = string.Empty;
            Query = string.Format(Query, customerFilter, itemFilter);
            var data = _objectEntity.SqlQuery<DivisionOutStanding>(Query).ToList();
            return data;
        }

        public List<LcChartModel> GetLcOutStanding(filterOption reportFilter, User userInfo)
        {
            var query = @"SELECT SUM (BALANCE)  Balance, SUM (TARGET) Target
  FROM (SELECT BALANCE Balance,
               ACC_CODE,
               (SELECT LIMIT
                  FROM FA_CHART_OF_ACCOUNTS_SETUP
                 WHERE     ACC_NATURE = 'LC'
                       AND DELETED_FLAG = 'N'
                       AND COMPANY_CODE = '01'
                       AND ACC_CODE = BAL.ACC_CODE)
                  Target
          FROM (  SELECT (  SUM (NVL (VGL.CR_AMOUNT, 0))
                          - SUM (NVL (VGL.DR_AMOUNT, 0)))
                            BALANCE,
                         VGL.ACC_CODE ACC_CODE
                    FROM V$VIRTUAL_GENERAL_LEDGER VGL,
                         FA_CHART_OF_ACCOUNTS_SETUP CAS
                   WHERE     CAS.ACC_NATURE = 'LC'
                         AND VGL.ACC_CODE = CAS.ACC_CODE
                         AND CAS.COMPANY_CODE = VGL.COMPANY_CODE
                GROUP BY VGL.ACC_CODE) BAL)";

            var data = _objectEntity.SqlQuery<LcChartModel>(query).ToList();
            return data;
        }

        public List<LcChartModel> GetLcOutStandingDetails(filterOption reportFilter, User userInfo)
        {
            var query = @"SELECT BALANCE,ACC_CODE,ACC_EDESC,
(SELECT LIMIT
FROM FA_CHART_OF_ACCOUNTS_SETUP
WHERE ACC_NATURE = 'LC'
AND DELETED_FLAG = 'N'
AND ACC_CODE = BAL.ACC_CODE)
Target
FROM ( SELECT (SUM (NVL(VGL.CR_AMOUNT,0)) - SUM(NVL(VGL.DR_AMOUNT,0))) BALANCE,
VGL.ACC_CODE ACC_CODE,CAS.ACC_EDESC
FROM V$VIRTUAL_GENERAL_LEDGER VGL, FA_CHART_OF_ACCOUNTS_SETUP CAS
WHERE CAS.ACC_NATURE = 'LC' AND VGL.ACC_CODE = CAS.ACC_CODE
GROUP BY VGL.ACC_CODE,CAS.ACC_EDESC) BAL";

            var data = _objectEntity.SqlQuery<LcChartModel>(query).ToList();
            data.ForEach(x => x.UsedBalance = x.Target - x.Balance);
            return data;
        }
        #endregion

        public List<CurrencyModel> GetCurrencyType()
        {
            var query = $@"SELECT DISTINCT CURRENCY_CODE, CURRENCY_EDESC FROM CURRENCY_SETUP WHERE DELETED_FLAG = 'N' AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
            var data = _objectEntity.SqlQuery<CurrencyModel>(query).ToList();
            return data;
        }

        public List<BrandFilterModel> GetBrandType()
        {
            var query = $@"SELECT UNIQUE BRAND_NAME FROM IP_ITEM_SPEC_SETUP WHERE BRAND_NAME IS NOT NULL AND DELETED_FLAG = 'N'";
            var data = _objectEntity.SqlQuery<BrandFilterModel>(query).ToList();
            return data;
        }

        public List<MonthlyDivisionSalesGraph> GetMonthlyAreaSalesSummary(ReportFiltersModel reportFilters, User userInfo, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode, string DateFormat, string AmountType)
        {
            if (userInfo == null)
            {
                userInfo = new Core.Domain.User();
                userInfo.company_code = "01";
                userInfo.branch_code = "01.01";

            }
            else if (string.IsNullOrEmpty(userInfo.company_code) || string.IsNullOrEmpty(userInfo.branch_code))
            {
                userInfo.company_code = "01";
                userInfo.branch_code = "01.01";
            }
            companyCode = string.Join(",", reportFilters.CompanyFilter);
            companyCode = companyCode == "" ? userInfo.company_code : companyCode;
            //new query from View
            string QueryNew = @"SELECT DS.DIVISION_EDESC as DivisionName, DS.DIVISION_CODE as DivisionCode,
                            TO_CHAR (si.sales_date, 'fmMonth') AS Month,
                            TO_CHAR (si.sales_date, 'MM') AS MonthInt,
                            SUM (nvl(si.TOTAL_SALES,0))/{0} AS TotalAmount,
                            SUM (nvl(si.QUANTITY,0))/{0} AS Quantity,
                            SUM (nvl(si.GROSS_SALES,0))/{0} AS GrossAmount,
                            SUM (nvl(si.NET_SALES,0))/{0} AS NetAmount
                    FROM V$SALES_INVOICE_REPORT3 si, fa_division_setup ds,SA_CUSTOMER_SETUP CS
                    WHERE  SI.CUSTOMER_CODE = CS.CUSTOMER_CODE
                    AND si.deleted_flag = 'N' AND si.company_code = '" + userInfo.company_code + @"'
                    and SI.DIVISION_CODE = DS.DIVISION_CODE";
            string QueryNewBS = @"SELECT DS.DIVISION_EDESC as DivisionName, DS.DIVISION_CODE as DivisionCode,
                            fn_bs_month(substr(bs_date(si.sales_date),6,2)) AS Month,
                            SUBSTR(BS_DATE(SI.SALES_DATE),6,2) as MonthInt,
                            SUM (nvl(si.TOTAL_SALES,0))/{0} AS TotalAmount,
                            SUM (nvl(si.QUANTITY,0))/{0} AS Quantity,
                            SUM (nvl(si.GROSS_SALES,0))/{0} AS GrossAmount,
                            SUM (nvl(si.NET_SALES,0))/{0} AS NetAmount
                    FROM V$SALES_INVOICE_REPORT3 si, fa_division_setup ds,SA_CUSTOMER_SETUP CS
                    WHERE  SI.CUSTOMER_CODE = CS.CUSTOMER_CODE
                    AND si.deleted_flag = 'N' AND si.company_code ='" + userInfo.company_code + @"'
                    and SI.DIVISION_CODE = DS.DIVISION_CODE";
            //Old Query from table
            string Query = @"SELECT DS.DIVISION_EDESC as DivisionName, DS.DIVISION_CODE as DivisionCode,
                            TO_CHAR (si.sales_date, 'fmMonth') AS Month,
                            TO_CHAR (si.sales_date, 'MM') AS MonthInt,
                            SUM (nvl(si.calc_Quantity,0))/{0} AS Quantity,
                            SUM (nvl(si.calc_total_price,0))/{0} AS GrossAmount
                    FROM sa_sales_invoice si, fa_division_setup DS,SA_CUSTOMER_SETUP CS, IP_ITEM_MASTER_SETUP IM
                    WHERE  SI.CUSTOMER_CODE = CS.CUSTOMER_CODE
                    AND si.deleted_flag = 'N' AND si.company_code = '" + userInfo.company_code + @"'
                    AND SI.DIVISION_CODE = DS.DIVISION_CODE
                    AND IM.ITEM_CODE = SI.ITEM_CODE";
            string QueryBS = @"SELECT DS.DIVISION_EDESC as DivisionName, DS.DIVISION_CODE as DivisionCode,
                            fn_bs_month(substr(bs_date(si.sales_date),6,2)) AS Month,
                            SUBSTR(BS_DATE(SI.SALES_DATE),6,2) as MonthInt,
                            SUM (nvl(si.calc_Quantity,0))/{0} AS Quantity,
                            SUM (nvl(si.calc_total_price,0))/{0} AS GrossAmount
                    FROM sa_sales_invoice si, fa_division_setup DS,SA_CUSTOMER_SETUP CS, IP_ITEM_MASTER_SETUP IM
                    WHERE  SI.CUSTOMER_CODE = CS.CUSTOMER_CODE
                    AND si.deleted_flag = 'N' AND si.company_code ='" + userInfo.company_code + @"'
                    and SI.DIVISION_CODE = DS.DIVISION_CODE
                    AND IM.ITEM_CODE = SI.ITEM_CODE";


            string condition = string.Empty;
            string conditionNew = string.Empty;
            //for customer Filter
            if (!string.IsNullOrEmpty(customerCode))
            {
                condition += " and (";
                foreach (var item in customerCode.Split(','))
                {
                    condition += "cs.master_customer_code like  (Select MASTER_CUSTOMER_CODE || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' and company_code = '" + userInfo.company_code + "') OR ";
                }
                condition = condition.Substring(0, condition.Length - 3) + ") ";
                conditionNew = condition;
            }
            var customerFilter = string.Empty;
            if (reportFilters.CustomerFilter.Count() > 0)
            {
                customerFilter = @"select DISTINCT(customer_code) from sa_customer_setup where (";
                //IF CUSTOMER_SKU_FLAG = G
                foreach (var item in reportFilters.CustomerFilter)
                {
                    customerFilter += "master_customer_code like  (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                customerFilter = customerFilter.Substring(0, customerFilter.Length - 3);
                //IF CUSTOMER_SKU_FLAG = I                
                customerFilter += " or (customer_code in (" + string.Join(",", reportFilters.CustomerFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                condition += " and SI.customer_code IN(" + customerFilter + ")";
            }
            //for item Filter
            if (!string.IsNullOrEmpty(itemCode))
            {
                condition += " and (";
                foreach (var item in itemCode.Split(','))
                {
                    condition += "IM.MASTER_ITEM_CODE LIKE (Select MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' and company_code = '" + userInfo.company_code + "') OR ";
                }
                condition = condition.Substring(0, condition.Length - 3) + ") ";
            }
            var productFilter = string.Empty;

            if (reportFilters.ProductFilter.Count() > 0)
            {
                // condition += " and (";
                productFilter = @"select  DISTINCT item_code from IP_ITEM_MASTER_SETUP where (";
                //IF PRODUCT_SKU_FLAG = G
                foreach (var item in reportFilters.ProductFilter)
                {
                    //condition += "IM.MASTER_ITEM_CODE LIKE (Select MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' and company_code = '" + userInfo.company_code + "') OR ";
                    productFilter += "MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                productFilter = productFilter.Substring(0, productFilter.Length - 3);
                //condition += ")";

                //IF PRODUCT_SKU_FLAG = I                
                productFilter += " or (ITEM_CODE in (" + string.Join(",", reportFilters.ProductFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                condition += " and SI.ITEM_CODE IN(" + productFilter + ")";
            }

            //for category Filter
            if (!string.IsNullOrEmpty(categoryCode))
            {
                condition += " and (";
                foreach (var item in categoryCode.Split(','))
                {
                    condition += "IM.CATEGORY_CODE = '" + item + "' OR ";
                }
                condition = condition.Substring(0, condition.Length - 3) + ") ";
            }

            if (reportFilters.CategoryFilter.Count > 0)
            {
                condition += " and (";
                foreach (var item in reportFilters.CategoryFilter)
                {
                    condition += "IM.CATEGORY_CODE = '" + item + "' OR ";
                }
                condition = condition.Substring(0, condition.Length - 3) + ") ";
            }

            //FOR COMPANY FILTER
            if (!string.IsNullOrEmpty(companyCode))
            {
                condition += " AND SI.COMPANY_CODE IN (" + companyCode + ")";
            }

            //FOR BRANCH FILTER
            if (!string.IsNullOrEmpty(branchCode))
            {
                condition += " AND SI.BRANCH_CODE IN (" + branchCode + ")";
            }

            if (reportFilters.BranchFilter.Count > 0)
            {
                condition += $@" AND SI.BRANCH_CODE IN ({string.Join(",", reportFilters.BranchFilter)})";
            }

            //FOR partyType FILTER
            if (!string.IsNullOrEmpty(partyTypeCode))
            {
                condition += " AND SI.PARTY_TYPE_CODE IN (" + partyTypeCode + ")";
            }

            //FOR FORMCODE FILER
            if (!string.IsNullOrEmpty(formCode))
            {
                condition += " AND SI.FORM_CODE IN (" + formCode + ")";
            }
            //FOR employee Filter
            if (reportFilters.EmployeeFilter.Count > 0)
            {
                condition += string.Format(@" AND SI.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", reportFilters.EmployeeFilter).ToString());
            }
            //FOR Agent Filter
            if (reportFilters.AgentFilter.Count > 0)
            {
                condition += string.Format(@" AND SI.AGENT_CODE IN  ('{0}')", string.Join("','", reportFilters.AgentFilter).ToString());
            }
            //FOR divison Filter
            if (reportFilters.DivisionFilter.Count > 0)
            {
                condition += string.Format(@" AND SI.DIVISION_CODE IN  ('{0}')", string.Join("','", reportFilters.DivisionFilter).ToString());
                conditionNew += string.Format(@" AND SI.DIVISION_CODE IN  ('{0}')", string.Join("','", reportFilters.DivisionFilter).ToString());
            }
            //FOR Location Filter
            if (reportFilters.LocationFilter.Count > 0)
            {
                string locationFilter = string.Empty;
                var locations = reportFilters.LocationFilter;
                for (int i = 0; i < locations.Count; i++)
                {

                    if (i == 0)
                        locationFilter += string.Format("SELECT LOCATION_CODE FROM IP_LOCATION_SETUP WHERE LOCATION_CODE LIKE '{0}%' ", locations[i]);
                    else
                    {
                        locationFilter += string.Format(" OR LOCATION_CODE like '{0}%' ", locations[i]);
                    }
                }
                locationFilter = string.Format(@" AND SI.FROM_LOCATION_CODE IN ({0} OR LOCATION_CODE IN ('{1}'))", locationFilter, string.Join("','", locations));
                condition += locationFilter;
            }
            //Filters for new query
            QueryNew += conditionNew +
                @" GROUP BY DS.DIVISION_EDESC, DS.DIVISION_CODE, TO_CHAR (si.sales_date, 'fmMonth'),TO_CHAR (si.sales_date, 'MM'),to_number(to_char(si.sales_date, 'MM'))
                        order by to_number(to_char(si.sales_date, 'MM')) asc";
            QueryNewBS += conditionNew +
                @" GROUP BY DS.DIVISION_EDESC,DS.DIVISION_CODE, SUBSTR(BS_DATE(SI.SALES_DATE),6,2)
                    ,fn_bs_month(substr(bs_date(si.sales_date),6,2))
                    ,to_number(substr(bs_date(sales_date),6,2))
                    Order by to_number(substr(bs_date(sales_date),6,2)) asc";

            //FIlters for old query
            Query += condition +
                @" GROUP BY DS.DIVISION_EDESC, DS.DIVISION_CODE, TO_CHAR (si.sales_date, 'fmMonth'),TO_CHAR (si.sales_date, 'MM'),to_number(to_char(si.sales_date, 'MM'))
                        order by to_number(to_char(si.sales_date, 'MM')) asc";
            QueryBS += condition +
                @" GROUP BY DS.DIVISION_EDESC, DS.DIVISION_CODE, SUBSTR(BS_DATE(SI.SALES_DATE),6,2)
                    ,fn_bs_month(substr(bs_date(si.sales_date),6,2))
                    ,to_number(substr(bs_date(sales_date),6,2))
                    Order by to_number(substr(bs_date(sales_date),6,2)) asc";

            Query = string.Format(Query, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter));
            QueryBS = string.Format(QueryBS, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter));
            QueryNew = string.Format(QueryNew, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter));
            QueryNewBS = string.Format(QueryNewBS, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter));

            string QueryFinal = string.Empty;
            if (AmountType == "NetAmount" || AmountType == "TotalAmount")
            {
                QueryFinal = string.Equals(DateFormat, "BS") ? QueryNewBS : QueryNew;
            }
            else
            {
                QueryFinal = string.Equals(DateFormat, "BS") ? QueryBS : Query;
            }

            var salesReportList = _objectEntity.SqlQuery<MonthlyDivisionSalesGraph>(QueryFinal).ToList();
            return salesReportList;
        }

        public List<NCRChartModel> GetTreewiseSalesReport(ReportFiltersModel reportFilters, User userInfo)
        {
            var companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);

            var customerQuery = string.Empty;
            var dataQuery = string.Empty;
            customerQuery = $@"SELECT CUSTOMER_CODE,CUSTOMER_EDESC,PRE_CUSTOMER_CODE,MASTER_CUSTOMER_CODE,GROUP_SKU_FLAG FROM SA_CUSTOMER_SETUP WHERE DELETED_FLAG = 'N' AND COMPANY_CODE = {companyCode} ORDER BY  PRE_CUSTOMER_CODE ASC";
            var customersList = _objectEntity.SqlQuery<NCRChartModel>(customerQuery).ToList();

            List<NCRChartModel> newList = new List<NCRChartModel>();

            dataQuery = $@" select V.CUSTOMER_CODE,SC.CUSTOMER_EDESC,V.TOTAL_SALES,V.QUANTITY,V.PRE_CUSTOMER_CODE,V.MASTER_CUSTOMER_CODE from V$SALES_INVOICE_REPORT3 V JOIN SA_CUSTOMER_SETUP SC ON V.CUSTOMER_CODE = SC.CUSTOMER_CODE WHERE V.COMPANY_CODE = {companyCode} AND V.DELETED_FLAG = 'N'  AND V.SALES_DATE>= TO_DATE('{reportFilters.FromDate}', 'YYYY-MON-DD')
                    AND V.SALES_DATE <= TO_DATE('{ reportFilters.ToDate}',' YYYY-MON-DD')";
            var dataList = _objectEntity.SqlQuery<NCRChartModel>(dataQuery).ToList();

            foreach (var customer in customersList)
            {
                if (customer.GROUP_SKU_FLAG == "I")
                {
                    var forlastItems = dataList.Where(x => x.CUSTOMER_CODE == customer.CUSTOMER_CODE).ToList();
                    customer.QUANTITY = forlastItems.Sum(x => x.QUANTITY);
                    customer.TOTAL_SALES = forlastItems.Sum(x => x.TOTAL_SALES);
                }
                else
                {
                    var filteredData = dataList.Where(x => x.PRE_CUSTOMER_CODE.StartsWith(customer.MASTER_CUSTOMER_CODE)).ToList();
                    customer.QUANTITY = filteredData.Sum(x => x.QUANTITY);
                    customer.TOTAL_SALES = filteredData.Sum(x => x.TOTAL_SALES);
                }
                customer.PRE_CUSTOMER_CODE = Convert.ToInt64(customer.PRE_CUSTOMER_CODE.Replace(".", "")).ToString();
                customer.MASTER_CUSTOMER_CODE = Convert.ToInt64(customer.MASTER_CUSTOMER_CODE.Replace(".", "")).ToString();

            }
            customersList.RemoveAll(x => x.TOTAL_SALES == 0);
            var totalHeader = customersList.Where(x => x.PRE_CUSTOMER_CODE == "0").ToList();
            var totalSales = totalHeader.Sum(x => x.TOTAL_SALES);
            var totalQuantity = totalHeader.Sum(x => x.QUANTITY);
            NCRChartModel totalAmount = new NCRChartModel();
            totalAmount.CUSTOMER_EDESC = "Total";
            totalAmount.TOTAL_SALES = totalSales;
            totalAmount.QUANTITY = totalQuantity;
            totalAmount.PRE_CUSTOMER_CODE = "0";
            customersList.Add(totalAmount);
            return customersList;
        }
        public List<NCRChartModel> GetTreewiseCustomerSalesPlanReport(ReportFiltersModel reportFilters, User userInfo)
        {
            var userId = userInfo.User_id;
            var superFlagQuery = $@"SELECT SUPER_USER_FLAG FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE = '{userInfo.company_code}'";
            var superFlag = _objectEntity.SqlQuery<string>(superFlagQuery).FirstOrDefault();
            var loginEmpCodeQuery = $@"SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE DELETED_FLAG='N' AND USER_NO='{userId}' AND COMPANY_CODE='{userInfo.company_code}'";
            var loginEmpCode = _objectEntity.SqlQuery<string>(loginEmpCodeQuery).FirstOrDefault();


            var companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);

            var customerQuery = string.Empty;
            var dataQuery = string.Empty;
            customerQuery = $@"SELECT CUSTOMER_CODE,CUSTOMER_EDESC,PRE_CUSTOMER_CODE,MASTER_CUSTOMER_CODE,GROUP_SKU_FLAG FROM SA_CUSTOMER_SETUP WHERE DELETED_FLAG = 'N' AND COMPANY_CODE = {companyCode} ORDER BY  PRE_CUSTOMER_CODE ASC";
            var customersList = _objectEntity.SqlQuery<NCRChartModel>(customerQuery).ToList();

            List<NCRChartModel> newList = new List<NCRChartModel>();

            dataQuery = $@" select V.CUSTOMER_CODE,SC.CUSTOMER_EDESC,V.PER_DAY_AMOUNT TOTAL_SALES,V.PER_DAY_QUANTITY QUANTITY,sc.PRE_CUSTOMER_CODE ,SC.MASTER_CUSTOMER_CODE  from TEMP_PL_SALES_PLAN_REPORT V JOIN SA_CUSTOMER_SETUP SC ON V.CUSTOMER_CODE = SC.CUSTOMER_CODE and v.company_code=sc.company_code WHERE V.COMPANY_CODE = {companyCode} AND sc.DELETED_FLAG = 'N' AND (V.PER_DAY_AMOUNT>0 OR V.PER_DAY_QUANTITY>0) AND V.PLAN_DATE>= TO_DATE('{reportFilters.FromDate}', 'YYYY-MON-DD')
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
            var dataList = _objectEntity.SqlQuery<NCRChartModel>(dataQuery).ToList();
            if(dataList.Count>0)
            {
                foreach (var customer in customersList)
                {
                    if (customer.GROUP_SKU_FLAG == "I")
                    {
                        var forlastItems = dataList.Where(x => x.CUSTOMER_CODE == customer.CUSTOMER_CODE).ToList();
                        customer.QUANTITY = forlastItems.Sum(x => x.QUANTITY);
                        customer.TOTAL_SALES = forlastItems.Sum(x => x.TOTAL_SALES);
                    }
                    else
                    {
                        var filteredData = dataList.Where(x => x.PRE_CUSTOMER_CODE.StartsWith(customer.MASTER_CUSTOMER_CODE)).ToList();
                        customer.QUANTITY = filteredData.Sum(x => x.QUANTITY);
                        customer.TOTAL_SALES = filteredData.Sum(x => x.TOTAL_SALES);
                    }
                    customer.PRE_CUSTOMER_CODE = Convert.ToInt64(customer.PRE_CUSTOMER_CODE.Replace(".", "")).ToString();
                    customer.MASTER_CUSTOMER_CODE = Convert.ToInt64(customer.MASTER_CUSTOMER_CODE.Replace(".", "")).ToString();

                }
            }
            
            customersList.RemoveAll(x => x.TOTAL_SALES == 0);
            var totalHeader = customersList.Where(x => x.PRE_CUSTOMER_CODE == "0").ToList();
            var totalSales = totalHeader.Sum(x => x.TOTAL_SALES);
            var totalQuantity = totalHeader.Sum(x => x.QUANTITY);
            NCRChartModel totalAmount = new NCRChartModel();
            totalAmount.CUSTOMER_EDESC = "Total";
            totalAmount.TOTAL_SALES = totalSales;
            totalAmount.QUANTITY = totalQuantity;
            totalAmount.PRE_CUSTOMER_CODE = "0";
            customersList.Add(totalAmount);
            return customersList;
        }
        public List<NCRChartModel> GetTreewiseProductSalesPlanReport(ReportFiltersModel reportFilters, User userInfo)
        {
            var userId = userInfo.User_id;
            var superFlagQuery = $@"SELECT SUPER_USER_FLAG FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE = '{userInfo.company_code}'";
            var superFlag = _objectEntity.SqlQuery<string>(superFlagQuery).FirstOrDefault();
            var loginEmpCodeQuery = $@"SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE DELETED_FLAG='N' AND USER_NO='{userId}' AND COMPANY_CODE='{userInfo.company_code}'";
            var loginEmpCode = _objectEntity.SqlQuery<string>(loginEmpCodeQuery).FirstOrDefault();

            var companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);

            var customerQuery = string.Empty;
            var dataQuery = string.Empty;
            customerQuery = $@"SELECT ITEM_CODE  CUSTOMER_CODE,ITEM_EDESC CUSTOMER_EDESC,PRE_ITEM_CODE PRE_CUSTOMER_CODE,MASTER_ITEM_CODE MASTER_CUSTOMER_CODE,GROUP_SKU_FLAG FROM IP_ITEM_MASTER_SETUP WHERE DELETED_FLAG = 'N' AND COMPANY_CODE = {companyCode} ORDER BY  PRE_ITEM_CODE ASC";
            var customersList = _objectEntity.SqlQuery<NCRChartModel>(customerQuery).ToList();

            List<NCRChartModel> newList = new List<NCRChartModel>();

            dataQuery = $@" select V.ITEM_CODE CUSTOMER_CODE,SC.ITEM_EDESC CUSTOMER_EDESC,V.PER_DAY_AMOUNT TOTAL_SALES,V.PER_DAY_QUANTITY QUANTITY,sc.PRE_ITEM_CODE PRE_CUSTOMER_CODE ,SC.MASTER_ITEM_CODE MASTER_CUSTOMER_CODE  from TEMP_PL_SALES_PLAN_REPORT V JOIN IP_ITEM_MASTER_SETUP SC ON V.ITEM_CODE = SC.ITEM_CODE and v.company_code=sc.company_code WHERE V.COMPANY_CODE = {companyCode} AND sc.DELETED_FLAG = 'N' AND (V.PER_DAY_AMOUNT>0 OR V.PER_DAY_QUANTITY>0) AND V.PLAN_DATE>= TO_DATE('{reportFilters.FromDate}', 'YYYY-MON-DD')
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
            var dataList = _objectEntity.SqlQuery<NCRChartModel>(dataQuery).ToList();
            if(dataList.Count>0)
            {
                foreach (var customer in customersList)
                {
                    if (customer.GROUP_SKU_FLAG == "I")
                    {
                        var forlastItems = dataList.Where(x => x.CUSTOMER_CODE == customer.CUSTOMER_CODE).ToList();
                        customer.QUANTITY = forlastItems.Sum(x => x.QUANTITY);
                        customer.TOTAL_SALES = forlastItems.Sum(x => x.TOTAL_SALES);
                    }
                    else
                    {
                        var filteredData = dataList.Where(x => x.PRE_CUSTOMER_CODE.StartsWith(customer.MASTER_CUSTOMER_CODE)).ToList();
                        customer.QUANTITY = filteredData.Sum(x => x.QUANTITY);
                        customer.TOTAL_SALES = filteredData.Sum(x => x.TOTAL_SALES);
                    }
                    customer.PRE_CUSTOMER_CODE = Convert.ToInt64(customer.PRE_CUSTOMER_CODE.Replace(".", "")).ToString();
                    customer.MASTER_CUSTOMER_CODE = Convert.ToInt64(customer.MASTER_CUSTOMER_CODE.Replace(".", "")).ToString();

                }
            }
            
            customersList.RemoveAll(x => x.TOTAL_SALES == 0);
            var totalHeader = customersList.Where(x => x.PRE_CUSTOMER_CODE == "0").ToList();
            var totalSales = totalHeader.Sum(x => x.TOTAL_SALES);
            var totalQuantity = totalHeader.Sum(x => x.QUANTITY);
            NCRChartModel totalAmount = new NCRChartModel();
            totalAmount.CUSTOMER_EDESC = "Total";
            totalAmount.TOTAL_SALES = totalSales;
            totalAmount.QUANTITY = totalQuantity;
            totalAmount.PRE_CUSTOMER_CODE = "0";
            customersList.Add(totalAmount);
            return customersList;
        }
        public List<NCRChartModel> GetYearTreewiseCollectonReport(ReportFiltersModel reportFilters, User userInfo)
        {
            var companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);

            var customerQuery = string.Empty;
            var dataQuery = string.Empty;
            customerQuery = $@"SELECT CUSTOMER_CODE,CUSTOMER_EDESC,PRE_CUSTOMER_CODE,MASTER_CUSTOMER_CODE,GROUP_SKU_FLAG FROM SA_CUSTOMER_SETUP WHERE DELETED_FLAG = 'N' AND COMPANY_CODE = {companyCode} ORDER BY  PRE_CUSTOMER_CODE ASC";
            var customersList = _objectEntity.SqlQuery<NCRChartModel>(customerQuery).ToList();

            List<NCRChartModel> newList = new List<NCRChartModel>();

            dataQuery = $@"   select SC.CUSTOMER_CODE,SC.CUSTOMER_EDESC,V.CR_AMOUNT TOTAL_SALES,SC.PRE_CUSTOMER_CODE,SC.MASTER_CUSTOMER_CODE
                     from V$VIRTUAL_SUB_LEDGER V JOIN SA_CUSTOMER_SETUP SC 
                     ON trim(V.SUB_CODE) = trim(SC.LINK_SUB_CODE) and sc.company_code=v.company_code WHERE V.COMPANY_CODE ={companyCode} AND V.DELETED_FLAG = 'N'  
                     and sub_ledger_flag='C' 
                        and V.form_code<>'0'
                     --AND FORM_CODE IN (SELECT FORM_CODE FROM FORM_SETUP WHERE FORM_TYPE IN('BK','CH')) 
                     --and VOUCHER_DATE >= (TO_DATE('{ reportFilters.FromDate}',' YYYY-MON-DD'))
                  --AND VOUCHER_DATE <=TO_DATE('{ reportFilters.ToDate}',' YYYY-MON-DD')";
            var dataList = _objectEntity.SqlQuery<NCRChartModel>(dataQuery).ToList();

            foreach (var customer in customersList)
            {
                if (customer.GROUP_SKU_FLAG == "I")
                {
                    var forlastItems = dataList.Where(x => x.CUSTOMER_CODE == customer.CUSTOMER_CODE).ToList();
                    //customer.QUANTITY = forlastItems.Sum(x => x.QUANTITY);
                    customer.TOTAL_SALES = forlastItems.Sum(x => x.TOTAL_SALES);
                }
                else
                {
                    var filteredData = dataList.Where(x => x.PRE_CUSTOMER_CODE.StartsWith(customer.MASTER_CUSTOMER_CODE)).ToList();
                   // customer.QUANTITY = filteredData.Sum(x => x.QUANTITY);
                    customer.TOTAL_SALES = filteredData.Sum(x => x.TOTAL_SALES);
                }
                customer.PRE_CUSTOMER_CODE = Convert.ToInt64(customer.PRE_CUSTOMER_CODE.Replace(".", "")).ToString();
                customer.MASTER_CUSTOMER_CODE = Convert.ToInt64(customer.MASTER_CUSTOMER_CODE.Replace(".", "")).ToString();

            }
            customersList.RemoveAll(x => x.TOTAL_SALES == 0);
            var totalHeader = customersList.Where(x => x.PRE_CUSTOMER_CODE == "0").ToList();
            var totalSales = totalHeader.Sum(x => x.TOTAL_SALES);
           // var totalQuantity = totalHeader.Sum(x => x.QUANTITY);
            NCRChartModel totalAmount = new NCRChartModel();
            totalAmount.CUSTOMER_EDESC = "Total";
            totalAmount.TOTAL_SALES = totalSales;
           // totalAmount.QUANTITY = totalQuantity;
            totalAmount.PRE_CUSTOMER_CODE = "0";
            customersList.Add(totalAmount);
            return customersList;
        }

        public List<NCRChartModel> GetThisMonthTreewiseCollectonReport(ReportFiltersModel reportFilters, User userInfo)
        {
            var companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);

            var customerQuery = string.Empty;
            var dataQuery = string.Empty;
            customerQuery = $@"SELECT CUSTOMER_CODE,CUSTOMER_EDESC,PRE_CUSTOMER_CODE,MASTER_CUSTOMER_CODE,GROUP_SKU_FLAG FROM SA_CUSTOMER_SETUP WHERE DELETED_FLAG = 'N' AND COMPANY_CODE = {companyCode} ORDER BY  PRE_CUSTOMER_CODE ASC";
            var customersList = _objectEntity.SqlQuery<NCRChartModel>(customerQuery).ToList();

            List<NCRChartModel> newList = new List<NCRChartModel>();

            dataQuery = $@"   select SC.CUSTOMER_CODE,SC.CUSTOMER_EDESC,V.CR_AMOUNT TOTAL_SALES,SC.PRE_CUSTOMER_CODE,SC.MASTER_CUSTOMER_CODE
                     from V$VIRTUAL_SUB_LEDGER V JOIN SA_CUSTOMER_SETUP SC 
                     ON trim(V.SUB_CODE) = trim(SC.LINK_SUB_CODE) WHERE V.COMPANY_CODE ={companyCode} AND V.DELETED_FLAG = 'N'  
                     and sub_ledger_flag='C' AND FORM_CODE IN (SELECT FORM_CODE FROM FORM_SETUP WHERE FORM_TYPE IN('BK','CH')) 
                    AND VOUCHER_DATE >= (SELECT FN_FIND_BS1STDATE(SYSDATE) FROM DUAL) 
AND VOUCHER_DATE <= TO_DATE(SYSDATE)";
            var dataList = _objectEntity.SqlQuery<NCRChartModel>(dataQuery).ToList();

            foreach (var customer in customersList)
            {
                if (customer.GROUP_SKU_FLAG == "I")
                {
                    var forlastItems = dataList.Where(x => x.CUSTOMER_CODE == customer.CUSTOMER_CODE).ToList();
                    //customer.QUANTITY = forlastItems.Sum(x => x.QUANTITY);
                    customer.TOTAL_SALES = forlastItems.Sum(x => x.TOTAL_SALES);
                }
                else
                {
                    var filteredData = dataList.Where(x => x.PRE_CUSTOMER_CODE.StartsWith(customer.MASTER_CUSTOMER_CODE)).ToList();
                    // customer.QUANTITY = filteredData.Sum(x => x.QUANTITY);
                    customer.TOTAL_SALES = filteredData.Sum(x => x.TOTAL_SALES);
                }
                customer.PRE_CUSTOMER_CODE = Convert.ToInt64(customer.PRE_CUSTOMER_CODE.Replace(".", "")).ToString();
                customer.MASTER_CUSTOMER_CODE = Convert.ToInt64(customer.MASTER_CUSTOMER_CODE.Replace(".", "")).ToString();

            }
            customersList.RemoveAll(x => x.TOTAL_SALES == 0);
            var totalHeader = customersList.Where(x => x.PRE_CUSTOMER_CODE == "0").ToList();
            var totalSales = totalHeader.Sum(x => x.TOTAL_SALES);
            // var totalQuantity = totalHeader.Sum(x => x.QUANTITY);
            NCRChartModel totalAmount = new NCRChartModel();
            totalAmount.CUSTOMER_EDESC = "Total";
            totalAmount.TOTAL_SALES = totalSales;
            // totalAmount.QUANTITY = totalQuantity;
            totalAmount.PRE_CUSTOMER_CODE = "0";
            customersList.Add(totalAmount);
            return customersList;
        }

        public List<NCRChartModel> GetTreewiseSalesReportDealer(ReportFiltersModel reportFilters, User userInfo)
        {
            var companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);

            var customerQuery = string.Empty;
            var dataQuery = string.Empty;
            customerQuery = $@" SELECT PARTY_TYPE_CODE CUSTOMER_CODE,PARTY_TYPE_EDESC CUSTOMER_EDESC,PRE_PARTY_CODE PRE_CUSTOMER_CODE,MASTER_PARTY_CODE MASTER_CUSTOMER_CODE,GROUP_SKU_FLAG FROM IP_PARTY_TYPE_CODE WHERE PARTY_TYPE_FLAG='D' AND DELETED_FLAG = 'N' AND COMPANY_CODE ={companyCode} ORDER BY  PRE_PARTY_CODE ASC   ";
            var customersList = _objectEntity.SqlQuery<NCRChartModel>(customerQuery).ToList();

            List<NCRChartModel> newList = new List<NCRChartModel>();

            dataQuery = $@"  select  V.PARTY_TYPE_CODE  CUSTOMER_CODE,SC.PARTY_TYPE_EDESC   CUSTOMER_EDESC, V.NET_SALES TOTAL_SALES,V.QUANTITY,SC.PRE_PARTY_CODE  PRE_CUSTOMER_CODE, SC.MASTER_PARTY_CODE  MASTER_CUSTOMER_CODE from V$SALES_INVOICE_REPORT3 V JOIN IP_PARTY_TYPE_CODE SC ON V.PARTY_TYPE_CODE = SC.PARTY_TYPE_CODE  WHERE V.COMPANY_CODE = {companyCode} AND V.DELETED_FLAG = 'N'  AND V.SALES_DATE>= TO_DATE('{reportFilters.FromDate}', 'YYYY-MON-DD')
                    AND V.SALES_DATE <= TO_DATE('{ reportFilters.ToDate}',' YYYY-MON-DD')";
            var dataList = _objectEntity.SqlQuery<NCRChartModel>(dataQuery).ToList();

            foreach (var customer in customersList)
            {
                if (customer.GROUP_SKU_FLAG == "I")
                {
                    var forlastItems = dataList.Where(x => x.CUSTOMER_CODE == customer.CUSTOMER_CODE).ToList();
                    customer.QUANTITY = forlastItems.Sum(x => x.QUANTITY);
                    customer.TOTAL_SALES = forlastItems.Sum(x => x.TOTAL_SALES);
                }
                else
                {
                    var filteredData = dataList.Where(x => x.PRE_CUSTOMER_CODE.StartsWith(customer.MASTER_CUSTOMER_CODE)).ToList();
                    customer.QUANTITY = filteredData.Sum(x => x.QUANTITY);
                    customer.TOTAL_SALES = filteredData.Sum(x => x.TOTAL_SALES);
                }
                customer.PRE_CUSTOMER_CODE = Convert.ToInt64(customer.PRE_CUSTOMER_CODE.Replace(".", "")).ToString();
                customer.MASTER_CUSTOMER_CODE = Convert.ToInt64(customer.MASTER_CUSTOMER_CODE.Replace(".", "")).ToString();

            }
            customersList.RemoveAll(x => x.TOTAL_SALES == 0);
            var totalHeader = customersList.Where(x => x.PRE_CUSTOMER_CODE == "0").ToList();
            var totalSales = totalHeader.Sum(x => x.TOTAL_SALES);
            var totalQuantity = totalHeader.Sum(x => x.QUANTITY);
            NCRChartModel totalAmount = new NCRChartModel();
            totalAmount.CUSTOMER_EDESC = "Total";
            totalAmount.TOTAL_SALES = totalSales;
            totalAmount.QUANTITY = totalQuantity;
            totalAmount.PRE_CUSTOMER_CODE = "0";
            customersList.Add(totalAmount);
            return customersList;
        }

        public List<Employee> GetEmployeesListForScheduler(User userInfo)
        {
            var Query = string.Format(@"SELECT es.employee_code as EmployeeCode,
                                            es.employee_edesc as EmployeeName,es.personal_email as PersonalEmail                                            
                                        FROM hr_employee_setup es
                                       WHERE es.deleted_flag = 'N' AND es.company_code = '{0}'", userInfo.company_code);

            var employeeList = _objectEntity.SqlQuery<Employee>(Query).ToList();
            return employeeList;
        }

        public List<DistArea> GetDistAreaList(User userInfo)
        {

            var Query = $@"SELECT AREA_CODE,
            AREA_NAME
                FROM DIST_AREA_MASTER
                WHERE DELETED_FLAG='N' AND COMPANY_CODE = '{userInfo.company_code}'";

            var distarealist = _objectEntity.SqlQuery<DistArea>(Query).ToList();
            return distarealist;

        }
        public List<ItemCustomerTreeModel> GetTreewiseKiranshoesSalesReport(ReportFiltersModel reportFilters, User userInfo)
        {
            var companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);

            var customerQuery = string.Empty;
            var dataQuery = string.Empty;
            customerQuery = $@"SELECT * FROM V_PRODUCT_SALES_TREE";
            var customersList = _objectEntity.SqlQuery<ItemCustomerTreeModel>(customerQuery).ToList();


            //ItemCustomerTreeModel totalAmount = new ItemCustomerTreeModel();
            //totalAmount.SERIAL_NO = 900;
            //totalAmount.MTD_QUANTITY = totalSales;
            //totalAmount.QUANTITY = totalQuantity;
            //totalAmount.PRE_CUSTOMER_CODE = "0";
            //customersList.Add(totalAmount);
            return customersList;
        }
        public List<UserwiseChartList> GetUserwiseChartById(UserwiseChartList model)
        {
            var userid = _workContext.CurrentUserinformation.login_code;
            var query = $@"select * from( 
                    SELECT MM.MODULE_ABBR,MM.COLOR,MM.DESCRIPTION, MM.ICON_PATH,MM.ORDERBY,MM.FULL_PATH,MM.VIRTUAL_PATH, MM.MODULE_CODE,MM.MENU_NO,MM.MENU_EDESC,MM.MENU_OBJECT_NAME,DW.USER_ID, ORDER_NO,DW.QuickCap FROM WEB_MENU_MANAGEMENT MM,DASHBOARD_WIDGETS DW
                                                   WHERE MM.MENU_OBJECT_NAME = DW.MODULE_NAME  AND MODULE_ABBR = 'DB'
                      UNION
                    SELECT MM.MODULE_ABBR,MM.COLOR,MM.DESCRIPTION, MM.ICON_PATH,MM.ORDERBY,MM.FULL_PATH,MM.VIRTUAL_PATH, MM.MODULE_CODE,MM.MENU_NO,MM.MENU_EDESC,MM.MENU_OBJECT_NAME,'  ' USER_ID, ' ' ORDER_NO,'' QuickCap FROM WEB_MENU_MANAGEMENT MM
                                                   WHERE  MODULE_ABBR = 'SM' ORDER BY ORDERBY ASC) where upper(user_id)=upper('{userid}') AND menu_object_name = '{model.MENU_OBJECT_NAME}'";
            var result = _objectEntity.SqlQuery<UserwiseChartList>(query).ToList();
            return result;
        }
        public List<DashboardSalesReport> SalesDashboardOverAll()
        {
            var query = @"SELECT  ITEM_GROUP_NAME ,YTDQUANTITY,MTDQUANTITY,TODAYQUANTITY,YTDAMOUNT,MTDAMOUNT,TODAYAMOUNT,YTDTARGETQUANTITY,MTDTARGETQUANTITY,TODAYTARGETQUANTITY,ROUND( (YTDQUANTITY-MTDTARGETQUANTITY)/YTDQUANTITY,2) VARIENCE FROM 

            (SELECT ITEM_GROUP_NAME,SUM(TE.YTDQUANTITY) YTDQUANTITY  ,SUM(MTDQUANTITY) MTDQUANTITY ,SUM(TODAYQUANTITY) TODAYQUANTITY,SUM(YTDAMOUNT) YTDAMOUNT,SUM(MTDAMOUNT) MTDAMOUNT ,SUM(TODAYAMOUNT) TODAYAMOUNT,SUM(YTDTARGETQUANTITY) YTDTARGETQUANTITY,SUM(TODAYTARGETQUANTITY) TODAYTARGETQUANTITY,SUM(MTDTARGETQUANTITY)MTDTARGETQUANTITY
             FROM 
 
             (select G.ITEM_GROUP_NAME,
             -- quantity
            (SELECT SUM(CALC_QUANTITY) FROM SA_SALES_INVOICE WHERE DELETED_FLAG='N' AND ITEM_CODE=GM.ITEM_CODE AND TRIM(MU_CODE)='PAIR' ) AS YTDQUANTITY,

            (SELECT SUM(CALC_QUANTITY) FROM SA_SALES_INVOICE WHERE DELETED_FLAG='N' AND ITEM_CODE=GM.ITEM_CODE AND TRIM(MU_CODE)='PAIR' AND SALES_DATE >= (SELECT FN_FIND_BS1STDATE(SYSDATE) FROM DUAL) 
            AND SALES_DATE <=TO_DATE(sysdate) )AS MTDQUANTITY,

            (SELECT SUM(CALC_QUANTITY) FROM SA_SALES_INVOICE WHERE DELETED_FLAG='N' AND ITEM_CODE=GM.ITEM_CODE AND SALES_DATE=TO_DATE(SYSDATE)  AND TRIM(MU_CODE)='PAIR' ) AS TODAYQUANTITY,
            --END QUANTITY
            --Amount

            (SELECT SUM(CALC_TOTAL_PRICE) FROM SA_SALES_INVOICE WHERE DELETED_FLAG='N' AND ITEM_CODE=GM.ITEM_CODE AND TRIM(MU_CODE)='PAIR' ) AS YTDAMOUNT,

            (SELECT SUM(CALC_TOTAL_PRICE) FROM SA_SALES_INVOICE WHERE DELETED_FLAG='N' AND ITEM_CODE=GM.ITEM_CODE AND TRIM(MU_CODE)='PAIR' AND SALES_DATE >= (SELECT FN_FIND_BS1STDATE(SYSDATE) FROM DUAL) 
            AND SALES_DATE <=TO_DATE(sysdate) )AS MTDAMOUNT,

            (SELECT SUM(CALC_TOTAL_PRICE) FROM SA_SALES_INVOICE WHERE DELETED_FLAG='N' AND ITEM_CODE=GM.ITEM_CODE AND SALES_DATE=TO_DATE(SYSDATE)  AND TRIM(MU_CODE)='PAIR') AS TODAYAMOUNT,
             --end AMOUNT
             --target quantity
            (SELECT SUM(PER_DAY_QUANTITY)  FROM PL_SALES_PLAN_DTL PL, IP_ITEM_MASTER_SETUP IT WHERE IT.ITEM_CODE=PL.ITEM_CODE AND PL.COMPANY_CODE=IT.COMPANY_CODE
            AND PL.DELETED_FLAG='N' AND IT.DELETED_FLAG='N'  AND IT.GROUP_SKU_FLAG='G' AND PL.ITEM_CODE=GM.ITEM_CODE) AS YTDTARGETQUANTITY ,

            (SELECT SUM(PER_DAY_QUANTITY)  FROM PL_SALES_PLAN_DTL PL, IP_ITEM_MASTER_SETUP IT WHERE IT.ITEM_CODE=PL.ITEM_CODE AND PL.COMPANY_CODE=IT.COMPANY_CODE
            AND PL.DELETED_FLAG='N' AND IT.DELETED_FLAG='N'  AND IT.GROUP_SKU_FLAG='G' AND PL.ITEM_CODE=GM.ITEM_CODE AND PL.PLAN_DATE=TO_DATE(SYSDATE)) AS  TODAYTARGETQUANTITY ,
            (SELECT SUM(PER_DAY_QUANTITY)  FROM PL_SALES_PLAN_DTL PL, IP_ITEM_MASTER_SETUP IT WHERE IT.ITEM_CODE=PL.ITEM_CODE AND PL.COMPANY_CODE=IT.COMPANY_CODE
            AND PL.DELETED_FLAG='N' AND IT.DELETED_FLAG='N'  AND IT.GROUP_SKU_FLAG='G' AND PL.ITEM_CODE=GM.ITEM_CODE AND PLAN_DATE >= (SELECT FN_FIND_BS1STDATE(SYSDATE) FROM DUAL) 
            AND PLAN_DATE <=TO_DATE(sysdate) ) AS  MTDTARGETQUANTITY
            --END TARGET
             from BI_ITEM_GROUP_SETUP G,BI_ITEM_GROUP_MAP GM WHERE G.ITEM_GROUP_CODE=GM.ITEM_GROUP_CODE AND G.DELETED_FLAG='N' 
            AND UPPER(GROUP_FLAG)='SALES'  GROUP BY ITEM_GROUP_NAME,GM.ITEM_CODE) TE  GROUP BY ITEM_GROUP_NAME)";
            var result = _objectEntity.SqlQuery<DashboardSalesReport>(query).ToList();
            return result==null? new List<DashboardSalesReport>():result;
        }
    }
}
