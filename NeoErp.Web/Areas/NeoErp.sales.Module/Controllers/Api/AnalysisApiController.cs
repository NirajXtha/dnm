using NeoErp.Core;
using NeoErp.Core.Helpers;
using NeoErp.Sales.Modules.Services.Models;
using NeoErp.Sales.Modules.Services.Models.AnalysisReport;
using NeoErp.Sales.Modules.Services.Services;
using NeoErp.Sales.Modules.Services.Services.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoErp.sales.Module.Controllers.Api
{
    public class AnalysisApiController : ApiController
    {
        private readonly IStockService _stockService;
        private readonly IWorkContext _workContext;
        private readonly IAnalysisService _analysisService;
        public AnalysisApiController(IStockService stockService, IWorkContext workContext, IAnalysisService analysisService)
        {
            _stockService = stockService;
            _workContext = workContext;
            _analysisService = analysisService;
        }
        [HttpPost]
        public LocationWiseViewStockModel GetLocationWiseStockReport(filterOption model)
        {
            LocationWiseViewStockModel reportData = new LocationWiseViewStockModel();
            reportData.LocationWiseStockModel = this._stockService.GetLocationWiseStockReport(model.ReportFilters, _workContext.CurrentUserinformation).ToList();
            reportData.total = reportData.LocationWiseStockModel.Count();
            reportData.AggregationResult = KendoGridHelper.GetAggregation(reportData.LocationWiseStockModel, model.aggregate);
            return reportData;
        }
        [HttpPost]
        public ProductRealizationAreaWiseViewModel GetProductRealizationAreaWise(filterOption model)
        {
            ProductRealizationAreaWiseViewModel reportData = new ProductRealizationAreaWiseViewModel();
            reportData.ProductRealizationAreaWiseModel = this._analysisService.GetProductRealizationAreaWise(model.ReportFilters, _workContext.CurrentUserinformation).ToList();
            reportData.total = reportData.ProductRealizationAreaWiseModel.Count();
            reportData.AggregationResult = KendoGridHelper.GetAggregation(reportData.ProductRealizationAreaWiseModel, model.aggregate);
            return reportData;
        }
        [HttpPost]
        public ProductRealizationSOSMWiseViewModel GetProductRealizationSalesManagerAndOfficerWise(filterOption model)
        {
            ProductRealizationSOSMWiseViewModel reportData = new ProductRealizationSOSMWiseViewModel();
            reportData.ProductRealizationSOSMWiseModel = this._analysisService.GetProductRealizationSalesManagerAndOfficerWise(model.ReportFilters, _workContext.CurrentUserinformation).ToList();
            reportData.total = reportData.ProductRealizationSOSMWiseModel.Count();
            reportData.AggregationResult = KendoGridHelper.GetAggregation(reportData.ProductRealizationSOSMWiseModel, model.aggregate);
            return reportData;
        }
        [HttpPost]
        public AvgAgingAndPaymentRotationSOSMWiseViewModel GetAverageAgingAndPaymentRotationSalesManagerAndOfficerWise(filterOption model)
        {
            AvgAgingAndPaymentRotationSOSMWiseViewModel reportData = new AvgAgingAndPaymentRotationSOSMWiseViewModel();
            reportData.AvgAgingAndPaymentRotationSOSMWiseModel = this._analysisService.GetAverageAgingAndPaymentRotationSalesManagerAndOfficerWise(model.ReportFilters, _workContext.CurrentUserinformation).ToList();
            reportData.total = reportData.AvgAgingAndPaymentRotationSOSMWiseModel.Count();
            reportData.AggregationResult = KendoGridHelper.GetAggregation(reportData.AvgAgingAndPaymentRotationSOSMWiseModel, model.aggregate);
            return reportData;
        }
        [HttpPost]
        public DealerSalesRangeViewModel GetDealerSalesRange(filterOption model)
        {
            DealerSalesRangeViewModel reportData = new DealerSalesRangeViewModel();
            reportData.DealerSalesRangeModel = this._analysisService.GetDealerSalesRange(model.ReportFilters, _workContext.CurrentUserinformation).ToList();
            reportData.total = reportData.DealerSalesRangeModel.Count();
            reportData.AggregationResult = KendoGridHelper.GetAggregation(reportData.DealerSalesRangeModel, model.aggregate);
            return reportData;
        }
        [HttpPost]
        public QuantityBalancePaymentRotationDayViewModel GetQuantityBalanceAndPaymentRotationDay(filterOption model)
        {
            QuantityBalancePaymentRotationDayViewModel reportData = new QuantityBalancePaymentRotationDayViewModel();
            reportData.QuantityBalancePaymentRotationDayModel = this._analysisService.GetQuantityBalanceAndPaymentRotationDay(model.ReportFilters, _workContext.CurrentUserinformation).ToList();
            reportData.total = reportData.QuantityBalancePaymentRotationDayModel.Count();
            reportData.AggregationResult = KendoGridHelper.GetAggregation(reportData.QuantityBalancePaymentRotationDayModel, model.aggregate);
            return reportData;
        }
        [HttpPost]
        public List<QuantityBalancePaymentRotationDayModel> GetQuantityBalanceAndPaymentRotationDayT(filterOption model)
        {
            //List<QuantityBalancePaymentRotationDayModel> result = new List<QuantityBalancePaymentRotationDayModel>();
            var result = this._analysisService.GetQuantityBalanceAndPaymentRotationDayT(model.ReportFilters, _workContext.CurrentUserinformation);
            return result;
        }
        [HttpPost]
        public AreaWiseSalesAndCollectionViewModel GetAreaWiseSalesAndCollection(filterOption model)
        {
            AreaWiseSalesAndCollectionViewModel reportData = new AreaWiseSalesAndCollectionViewModel();
            reportData.AreaWiseSalesAndCollectionModel = this._analysisService.GetAreaWiseSalesAndCollection(model.ReportFilters, _workContext.CurrentUserinformation).ToList();
            reportData.total = reportData.AreaWiseSalesAndCollectionModel.Count();
            reportData.AggregationResult = KendoGridHelper.GetAggregation(reportData.AreaWiseSalesAndCollectionModel, model.aggregate);
            return reportData;
        }
        #region lc analysis report
        [HttpPost]
        public BrandWiseGrossProfitViewModel GetBrandWiseGrossProfit(filterOption model)
        {
            BrandWiseGrossProfitViewModel reportData = new BrandWiseGrossProfitViewModel();
            reportData.BrandWiseGrossProfitModel = this._analysisService.GetBrandWiseGrossProfit(model.ReportFilters, _workContext.CurrentUserinformation).ToList();
            reportData.total = reportData.BrandWiseGrossProfitModel.Count();
            reportData.AggregationResult = KendoGridHelper.GetAggregation(reportData.BrandWiseGrossProfitModel, model.aggregate);
            return reportData;
        }

        [HttpPost]
        public BrandWiseSalesViewModel GetBrandWiseSales(filterOption model)
        {
            BrandWiseSalesViewModel reportData = new BrandWiseSalesViewModel();
            reportData.BrandWiseSalesModel = this._analysisService.GetBrandWiseSales(model.ReportFilters, _workContext.CurrentUserinformation).ToList();
            reportData.total = reportData.BrandWiseSalesModel.Count();
            reportData.AggregationResult = KendoGridHelper.GetAggregation(reportData.BrandWiseSalesModel, model.aggregate);
            return reportData;
        }
        [HttpPost]
        public PartyWiseSalesViewModel GetPartyWiseSales(filterOption model)
        {
            PartyWiseSalesViewModel reportData = new PartyWiseSalesViewModel();
            reportData.PartyWiseSalesModel = this._analysisService.GetPartyWiseSales(model.ReportFilters, _workContext.CurrentUserinformation).ToList();
            reportData.total = reportData.PartyWiseSalesModel.Count();
            reportData.AggregationResult = KendoGridHelper.GetAggregation(reportData.PartyWiseSalesModel, model.aggregate);
            return reportData;
        }
        [HttpPost]
        public DebtorAgingPerDayViewModel GetDebtorAgingPerDay(filterOption model)
        {
            DebtorAgingPerDayViewModel reportData = new DebtorAgingPerDayViewModel();
            reportData.DebtorAgingPerDayModel = this._analysisService.GetDebtorAgingPerDay(model.ReportFilters, _workContext.CurrentUserinformation).ToList();
            reportData.total = reportData.DebtorAgingPerDayModel.Count();
            reportData.AggregationResult = KendoGridHelper.GetAggregation(reportData.DebtorAgingPerDayModel, model.aggregate);
            return reportData;
        }
        [HttpPost]
        public DebtorAgingViewModel GetDebtorAging(filterOption model)
        {
            DebtorAgingViewModel reportData = new DebtorAgingViewModel();
            reportData.DebtorAgingModel = this._analysisService.GetDebtorAging(model.ReportFilters, _workContext.CurrentUserinformation).ToList();
            reportData.total = reportData.DebtorAgingModel.Count();
            reportData.AggregationResult = KendoGridHelper.GetAggregation(reportData.DebtorAgingModel, model.aggregate);
            return reportData;
        }
        [HttpPost]
        public WeekSalesViewModel GetWeekOfSales(filterOption model)
        {
            WeekSalesViewModel reportData = new WeekSalesViewModel();
            reportData.WeekSalesModel = this._analysisService.GetWeekOfSales(model.ReportFilters, _workContext.CurrentUserinformation).ToList();
            reportData.total = reportData.WeekSalesModel.Count();
            reportData.AggregationResult = KendoGridHelper.GetAggregation(reportData.WeekSalesModel, model.aggregate);
            return reportData;
        }
        #endregion
    }
}
