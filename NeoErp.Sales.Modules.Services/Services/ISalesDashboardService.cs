using NeoErp.Sales.Modules.Services.Models.SalesDashBoard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoErp.Core.Models;
using NeoErp.Core.Helpers;
using NeoErp.Core.Domain;
using NeoErp.Sales.Modules.Services.Models.Stock;
using NeoErp.Sales.Modules.Services.Models.Division;
using NeoErp.Sales.Modules.Services.Models;

namespace NeoErp.Sales.Modules.Services.Services
{
    public interface ISalesDashboardService
    {
        List<SalesCollectionGraph> GetSalesCollectionMonthlyReport(ReportFiltersModel reportFilters, User userInfo);
        List<SalesCollectionGraphDayWise> GetSalesCollectionDailyReport(ReportFiltersModel reportFilters, User userInfo, string month, string DateFormat);
        List<TargetCollectionGraphDayWise> GetTargetCollectionDailyReport(ReportFiltersModel reportFilters, User userInfo, string DateFormat, string monthName);
        List<SalesTargetGraphDayWise> GetSalesTargetDailyReport(ReportFiltersModel reportFilters, User userInfo, string DateFormat, string monthName,string monthInt);
        List<SalesTargetGraphDayWise> GetSalesTargetItemWiseReport(ReportFiltersModel reportFilters, User userInfo, string DateFormat, string monthName,string monthInt); 
        List<BranchWiseSalesCollection> GetSalesCollectionBranchWiseReport(ReportFiltersModel reportFilters, User userInfo);
        List<BranchWiseSalesCollection> GetSalesCollectionDivisionWiseReport(ReportFiltersModel reportFilters, User userInfo);
        List<BranchWiseSalesCollection> GetSalesCollectionBranchWiseReport(ReportFiltersModel reportFilters, User userInfo, string branchCode);
        List<BranchWiseTargetCollection> GetTargetCollectionBranchWiseReport(ReportFiltersModel reportFilters, User userInfo);
        List<MonthlySalesGraph> GetSalesMonthSummanry(ReportFiltersModel reportFilters, User userInfo);
        List<MonthlySalesGraph> GetSalesMonthSummanryMobile(ReportFiltersModel reportFilters, User userInfo, string dateFormat, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode,string divisionCode, string formCode);
        List<MonthlySalesGraph> GetSalesMonthSummanry(ReportFiltersModel reportFilters, string dateFormat, User userInfo, bool salesReturn);
        List<MonthlySalesFiscalYearGraph> GetSalesMonthFiscal(ReportFiltersModel reportFilters, User userInfo, string dateFormat);
        List<MonthlySalesFiscalYearGraph> GetCollectionMonthFiscal(ReportFiltersModel reportFilters, User userInfo, string dateFormat);
        List<SalesCollectionDivisionDayWise> GetSalesCollectionDivisionWise(ReportFiltersModel reportFilters, User userInfo, string dateFormat,string divisionCode);
        List<SalesCollectionDivisionDayWise> GetSalesCollectionDivisionWiseDaily(ReportFiltersModel reportFilters, User userInfo, string month, string dateFormat);
        List<SalesProductRateFiscalYearGraph> GetSalesProductRateFiscalYear(ReportFiltersModel reportFilters, string dateFormat);
        List<SalesProductRateFiscalYearGraph> GetAvgPurchaseRateFiscalYear(ReportFiltersModel reportFilters, string duration, User userInfo);
        List<SaudaModel> GetSaudaQuantityReport(ReportFiltersModel reportFilters, string duration, User userInfo);
        List<SaudaModel> GetSaudaAveragePurchaseRateReport(ReportFiltersModel reportFilters, string duration, User userInfo);
        List<BranchWiseSalesFiscalYearGraph> GetSalesBranchWiseFiscalYear(ReportFiltersModel reportFilters, User userInfo);
        List<MonthlySalesFiscalYearGraph> GetSalesDailyFiscal(ReportFiltersModel reportFilters, User userInfo, string month, string dbName, string DateFormat);        
        List<DaysSalesGraph> GetDailySalesSummary(ReportFiltersModel reportFilters, User userInfo, string DateFormat);
        List<DaysSalesGraph> GetDailySalesSummary(ReportFiltersModel reportFilters, User userInfo, string month, string DateFormat, bool salesReturn);
        List<MonthlyBranchSalesGraph> GetMonthlyBranchSalesSummary(ReportFiltersModel reportFilters, User userInfo);
        List<BalanceChartModel> GetCashBalanceDetailSummary(ReportFiltersModel reportFilters, User userInfo);
        List<BalanceChartModel> GetLoanBalanceDetailSummary(ReportFiltersModel reportFilters, string duration, User userInfo);
        List<BalanceChartModel> GetExpenseBalanceSummary(ReportFiltersModel reportFilters, User userInfo);
        List<BalanceChartModel> GetExpenseBalanceDetailSummary(ReportFiltersModel reportFilters,string accCode, User userInfo);
        List<MonthlyBranchSalesGraph> GetMonthlyBranchSalesSummary(ReportFiltersModel reportFilters, string dateFormat, User userInfo);
        List<MonthlyBranchSalesGraph> GetMonthlyBranchSalesSummary(ReportFiltersModel reportFilters, User userInfo, string dateFormat, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode, bool salesReturn, string AmountType);
        List<BranchDaysSalesGraph> GetDailyBranchSalesSummary(ReportFiltersModel reportFilters, User userInfo, bool salesReturn);
       
        List<BranchDaysSalesGraph> GetDailyBranchSalesSummary(ReportFiltersModel reportFilters, User userInfo, string monthName, string branchName, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode, string DateFormat, bool salesReturn, string AmountType);
        List<ChartSalesModel> GetItemWiseBranchSalesSummary(ReportFiltersModel reportFilters, User userInfo, string monthCode, string branchCode, string companyCode);
        List<ChartSalesModel> GetDivisionItemWiseReport(ReportFiltersModel reportFilters, User userInfo, string monthCode, string divisionCode, string companyCode, string dateFormat);
        List<MonthlyDivisionSalesGraph> GetMonthlyDivisionSalesSummary(ReportFiltersModel reportFilters, User userInfo);
        List<DivisionDaysSalesGraph> GetDailyDivisionSalesSummary(ReportFiltersModel reportFilters, User userInfo);
        List<DivisionDaysSalesGraph> GetDailyDivisionSalesSummary(ReportFiltersModel reportFilters, User userInfo, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string divisionName, string partyTypeCode, string formCode, string DateFormat, string AmountType);


        //for dashboard widgets                    
        string GetDashboardWidgets(string name, string type);
        List<string> GetDashboardWidgetsForPersonalDashboard(string name);
        bool SaveDashboardWidgets(string userId, string order, string type);
        bool ResetDashboardWidgets(string name, string type);
        IList<UserwiseChartList> GetUserwiseChartList(string Module_Code);
        IList<UserwiseChartList> GetUserWiseMenuPermission(string Module_Code, User userInfo);
        bool DeleteUserwiseChartList(UserwiseChartList model);



        List<MonthlyDivisionSalesGraph> GetMonthlyDivisionSalesSummary(ReportFiltersModel reportFilters, User userInfo, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode, string DateFormat, string AmountType);
        List<Employee> GetEmployeesList(User userInfo);
        List<Employee> GetEmployeesListForScheduler(User userInfo);
        List<Agent> GetAgentList(User userInfo);
        List<Division> GetDivisionList(User userInfo);
        List<DistArea> GetDistAreaList(User userInfo);
        List<EmployeeWiseReport> GetEmployeesSalesReport(ReportFiltersModel reportFilters, User userInfo);
        List<EmployeeWiseReport> GetEmployeesSalesReport(ReportFiltersModel reportFilters, User userInfo, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode, string employeeCode);

        List<WeekWiseCollectionReport> GetWeekWiseCollectionReport(ReportFiltersModel reportFilters, User userInfo);
        List<WeekWiseCollectionReport> GetWeekWiseCollectionReport(ReportFiltersModel reportFilters, User userInfo, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode);

        List<WeekWiseCollectionReport> GetWeekWiseCustomerCollectionReport(ReportFiltersModel reportFilters, string week, User userInfo);
        List<ExpensesTrendReport> GetExpensesAccount(User userinfo);
        List<ExpensesTrendReport> GetExpensesTrendMonthlyReport(ReportFiltersModel reportFilters, User userInfo);
        List<ExpensesTrendReport> GetExpensesTrendMonthlyReport(ReportFiltersModel reportFilters, string accountCode, User userInfo);
        List<ExpensesTrendReport> GetExpensesTrendMonthlyReport(ReportFiltersModel reportFilters, User userinfo, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode, string accountCode);
        List<ExpensesTrendReport> GetExpensesTrendAccountWiseReport(ReportFiltersModel reportFilters, string master_acc_code, User userInfo, string DateFormat);
        List<BranchWiseSalesTarget> GetSalesTargetBranchWiseReport(ReportFiltersModel reportFilters, User userInfo);
        List<BranchWiseSalesTarget> GetSalesTargetBranchWiseMonthlyReport(ReportFiltersModel reportFilters, User userInfo,string branchcode);
        List<BranchWiseSalesTarget> GetSalesTargetBranchWiseItemsReport(ReportFiltersModel reportFilters, User userInfo, string branchcode);
        
        List<ExpensesTrendReport> GetExpensesTrendAccountWiseDailyReport(ReportFiltersModel reportFilters, User userInfo, string master_acc_code, string month, string DateFormat);
        List<SalesCollectionGraph> GetSalesCollectionMonthlyReport(ReportFiltersModel reportFilters, User userInfo, string dateFormat);
        List<TargetCollectionGraph> GetTargetCollectionMonthlyReport(ReportFiltersModel reportFilters, User userInfo, string dateformat);
        List<SalesTargetGraph> GetSalesTargetMonthlyReport(ReportFiltersModel reportFilters, User userInfo, string dateformat);
        List<SalesCollectionGraph> GetSalesCollectionMonthlyReport(ReportFiltersModel reportFilters, User userInfo, string dateFormat, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode);
        List<GrossProfitReport> GetGPReportMonthWise(ReportFiltersModel reportFilters, User userInfo
            , string customerCode, string itemCode
            , string categoryCode, string companyCode
            , string branchCode, string partyTypeCode
            , string formCode, string DateFormat);
        //List<GrossProfitReport> GetGPReportBSMonthWise(ReportFiltersModel reportFilters, User userInfo);
        List<GrossProfitReport> GetGPReportMonthCategoryWise(ReportFiltersModel reportFilters, User userInfo, string format);
        List<GrossProfitReportDayWise> GetGPDayWiseItemReport(ReportFiltersModel reportFilters, User userInfo, string categoryCode, string month);
        List<PendingVoucherModel> GetPendingVoucher();

        List<MonthlyBranchExpenseGraph> GetMonthlyBranchExpenseSummary(ReportFiltersModel reportFilters, User userInfo, string dateFormat, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode);
        List<GrossProfitReport> GetGPReportDayWise(ReportFiltersModel reportFilters, User currentUserinformation, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode, string month, string dateFormat);
        List<CompanySalesModel> GetCompanySalesMonthlyReport(filterOption model, string dateformat, bool salesReturn, string AmountType);
        List<CompanyProductionModel> GetCompanyProductionMonthlyReport(filterOption model, string dateformat);

        List<CompanyExpenseModel> GetCompanyExpenseMonthlyReport(filterOption model, string dateformat);
        List<MonthlyBranchProductionGraph> GetMonthlyBranchProductionSummary(ReportFiltersModel reportFilters, User userInfo, string dateFormat, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode);
       
        List<BranchDaysExpenseGraph> GetDailyBranchExpenseSummary(ReportFiltersModel reportFilters, User userInfo, string monthName, string branchName, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode, string DateFormat);

        List<BranchDaysProductionGraph> GetDailyBranchProductionSummary(ReportFiltersModel reportFilters, User userInfo, string monthName, string branchName, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode, string DateFormat);
        
        List<CompanySalesModel> GetCompanyPurchaseMonthlyReport(filterOption model, string dateformat, User userInfo);
        List<MonthlyBranchSalesGraph> GetMonthlyPurchaseSalesSummary(ReportFiltersModel reportFilters, User userInfo, string dateFormat);
        List<BranchDaysSalesGraph> GetDailyBranchPurchaseSummary(ReportFiltersModel reportFilters, User userInfo, string monthName, string branchName,
        string DateFormat);
        List<StockChartModel> GetCompanyStockMonthlyReport(filterOption model, string dateformat);
        List<StockChartModel> GetMonthlyBranchStockSummary(ReportFiltersModel reportFilters, User userInfo, string dateFormat);
        List<StockChartModel> GetStockCategorySummary(ReportFiltersModel reportFilters, User userInfo, string dateFormat);
        List<StockChartModel> GetStockItemSummary(ReportFiltersModel reportFilters, User userInfo, string dateFormat);

        List<BalanceChartModel> GetCompanyBankBalanceReport(filterOption model, string dateformat);
        List<BalanceChartModel> GetBranchBankBalanceSummary(ReportFiltersModel reportFilters, User userInfo, string dateFormat);
        List<BalanceChartMonthlyModel> GetBankBalanceMonthlySummary(ReportFiltersModel reportFilters, User userInfo, string bankName);

        List<BalanceChartModel> GetCompanyCashBalanceReport(filterOption model, string dateformat);
        List<BalanceChartModel> GetBranchCashBalanceSummary(ReportFiltersModel reportFilters, User userInfo, string dateFormat);
        List<BalanceChartMonthlyModel> GetCashBalanceMonthlySummary(ReportFiltersModel reportFilters, User userInfo, string bankName);

        List<StockValutionViewModel> GetStockValutionSummary(filterOption reportFilters, User userInfo);
        List<StockValutionViewModel> GetProductStockValutions(filterOption reportFilters, User UserInfo, string CatagoryCode);
        List<DivisionOutStanding> GetDivisionWiseOutStanding(filterOption reportFilers, User userInfo);
        List<LcChartModel> GetLcOutStanding(filterOption reportFilter, User userInfo);
        List<LcChartModel> GetLcOutStandingDetails(filterOption reportFilter, User userInfo);

        List<CurrencyModel> GetCurrencyType();

        List<BrandFilterModel> GetBrandType();
        List<BranchWiseSalesCollection> GetSalesCollectionAreaWiseReport(ReportFiltersModel reportFilters, User userInfo, string branchCode);
        List<MonthlyBranchSalesBill> GetMonthlyBranchBillCount(ReportFiltersModel reportFilters, User userInfo, string DateFormat = "BS");
        List<NCRChartModel> GetTreewiseSalesReport(ReportFiltersModel reportFilters, User userInfo);
        List<NCRChartModel> GetTreewiseCustomerSalesPlanReport(ReportFiltersModel reportFilters, User userInfo);
        List<NCRChartModel> GetTreewiseProductSalesPlanReport(ReportFiltersModel reportFilters, User userInfo);
        List<NCRChartModel> GetTreewiseSalesReportDealer(ReportFiltersModel reportFilters, User userInfo);
        List<MonthlyBranchSalesGraph> GetMonthlyAreaalesSummary(ReportFiltersModel reportFilters, User userInfo, string dateFormat, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode, bool salesReturn, string AmountType);
        List<BalanceChartModel> GetBranchBankBalanceGroupBySummary(ReportFiltersModel reportFilters, User userInfo, string dateFormat);
        List<MonthlyBranchSalesGraph> GetMonthlyBranchPurchaseGroupsummary(ReportFiltersModel reportFilters, User userInfo, string dateFormat, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode, bool salesReturn, string AmountType);
        List<ChartSalesModel> GetItemWiseBranchPurchaseGroupSummary(ReportFiltersModel reportFilters, User userInfo, string monthCode, string branchCode, string companyCode);
        List<UserwiseChartList> GetUserwiseChartById(UserwiseChartList model);
        List<NCRChartModel> GetYearTreewiseCollectonReport(ReportFiltersModel reportFilters, User userInfo);
        List<NCRChartModel> GetThisMonthTreewiseCollectonReport(ReportFiltersModel reportFilters, User userInfo);
        List<BalanceChartModel> GetBankBalanceDetailSummary(ReportFiltersModel reportFilters, User userInfo);
        List<MonthlyLoan> GetMonthlyShortteamLoan(ReportFiltersModel reportFilters, User userInfo, string dateFormat);
        List<MonthlyLoan> GetMonthlyLongteamLoan(ReportFiltersModel reportFilters, User userInfo, string dateFormat);

        List<DashboardSalesReport> SalesDashboardOverAll();



        List<BranchWiseSalesTarget> GetSalesTargetSegmentReport(ReportFiltersModel reportFilters, User userInfo);
        List<MonthlySalesFiscalYearGraph> AgingCategoryWise(ReportFiltersModel reportFilters, User userInfo, string dateFormat);
        List<MonthlySalesFiscalYearGraph> customerWiseaging(ReportFiltersModel reportFilters, User userInfo, string dateFormat);

        List<ItemCustomerTreeModel> GetTreewiseKiranshoesSalesReport(ReportFiltersModel reportFilters, User userInfo);

    }
}
