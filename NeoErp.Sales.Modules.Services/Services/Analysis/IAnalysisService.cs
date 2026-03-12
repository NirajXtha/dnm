using NeoErp.Core.Domain;
using NeoErp.Core.Models;
using NeoErp.Sales.Modules.Services.Models;
using NeoErp.Sales.Modules.Services.Models.AnalysisReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Services.Analysis
{
    public interface IAnalysisService
    {
        //IList<LocationWiseStockModel> GetLocationWiseStockReport(ReportFiltersModel model, User userInfo);
        IList<ProductRealizationAreaWiseModel> GetProductRealizationAreaWise(ReportFiltersModel model, User userInfo);
        IList<ProductRealizationSOSMWiseModel> GetProductRealizationSalesManagerAndOfficerWise(ReportFiltersModel model, User userInfo);
        IList<AvgAgingAndPaymentRotationSOSMWiseModel> GetAverageAgingAndPaymentRotationSalesManagerAndOfficerWise(ReportFiltersModel model, User userInfo);
        IList<DealerSalesRangeModel> GetDealerSalesRange(ReportFiltersModel model, User userInfo);
        IList<QuantityBalancePaymentRotationDayModel> GetQuantityBalanceAndPaymentRotationDay(ReportFiltersModel model, User userInfo);
        List<QuantityBalancePaymentRotationDayModel> GetQuantityBalanceAndPaymentRotationDayT(ReportFiltersModel model, User userInfo);
        IList<AreaWiseSalesAndCollectionModel> GetAreaWiseSalesAndCollection(ReportFiltersModel model, User userInfo);
        IList<BrandWiseGrossProfitModel> GetBrandWiseGrossProfit(ReportFiltersModel model, User userInfo);
        IList<BrandWiseSalesModel> GetBrandWiseSales(ReportFiltersModel model, User userInfo);
        IList<PartyWiseSalesModel> GetPartyWiseSales(ReportFiltersModel model, User userInfo);
        IList<DebtorAgingPerDayModel> GetDebtorAgingPerDay(ReportFiltersModel model, User userInfo);
        IList<DebtorAgingModel> GetDebtorAging(ReportFiltersModel model, User userInfo);
        IList<WeekSalesModel> GetWeekOfSales(ReportFiltersModel model, User userInfo);
    }
}
