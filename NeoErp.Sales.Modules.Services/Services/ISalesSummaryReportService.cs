using NeoErp.Core.Domain;
using NeoErp.Core.Models;
using NeoErp.Sales.Modules.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Services
{
    public interface ISalesSummaryReportService
    {
        List<SalesSummaryReportModel> GetPartyWiseGPAnalysisSalesSummaryReport(ReportFiltersModel reportFilters, User userInfo);
        IEnumerable<SalesAnalysisModel> getSalesAnalysis(ReportFiltersModel model,User userInfo);
        IEnumerable<HighestSellingModel> GetAllHighestSelling(User userInfo, int selectedItem);
        IList<TopEmployeeListModel> getTopSalesEmployee(ReportFiltersModel model, User userInfo,int pageSize);
        IList<TopDealerListModel> getTopSalesDealer(ReportFiltersModel model, User userInfo, int pageSize);
        List<RegionWiseSalesModel> GetRegionWiseSales();
        List<ProductWiseGpModel> GetProductWiseGp();
        List<WeeklyExpenseAnalysis> GetWeeklyExpenseAnalysis(ReportFiltersModel model,User userInfo);
        List<WeeklyVendorPaymentAnalysis> GetWeeklyVendorPaymentAnalysis(ReportFiltersModel model,User userInfo);
        IList<SalesAchieveModel> getSalesAchieve(ReportFiltersModel model, string duration, User userInfo);
        IList<PendingOrder> getPendingOrderChart(ReportFiltersModel model,bool branchwiseChart, User userInfo);
        IList<SalesAchieveModel> getSalesAchieveBranch(ReportFiltersModel model, string duration, User userInfo);
        IList<SalesAchieveModel> getSalesAchieveDivision(ReportFiltersModel model, string duration, User userInfo);
        IList<SalesAchieveModel> getSalesAchieveItemWise(ReportFiltersModel model, string duration, string branchCode, User userInfo);
        IList<SalesAchieveModel> getSalesDivisionAchieveItemWise(ReportFiltersModel model, string duration, string branchCode, User userInfo);
        IList<SalesAchieveMonthModel> getSalesAchieveDivisionMonth(ReportFiltersModel model, string duration, User userInfo);
        IList<SalesAchieveModel> getSalesAchieveProjected(ReportFiltersModel model, string duration, User userInfo);
        IList<SalesAchieveModel> getSalesAchieveBranchProjection(ReportFiltersModel model, string duration, User userInfo);
        IList<SalesAchieveProjectionModel> GetSalesAchieveBranchProjectionWithTarget(ReportFiltersModel reportFilters, string duration, User currentUserinformation);
        IList<SalesSummaryReportModel> GetSlowMovingItem(User userInfo, int selectedItem);
        IList<SalesSummaryReportModel> GetTopMovingItem(User userInfo, int selectedItem);
        IList<SalesSummaryReportModel> GetNonMovingItem(User userInfo, int selectedItem);
        IList<ProductLevelModel> GetProductLevel(User userInfo, int selectedItem);

    }
}
