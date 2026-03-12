using NeoErp.Core;
using NeoErp.Core.Caching;
using NeoErp.Core.Domain;
using NeoErp.Core.Helpers;
using NeoErp.Core.Models;
using NeoErp.Sales.Modules.Services.Models;
using NeoErp.Sales.Modules.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoErp.sales.Module.Controllers.Api
{
    public class SalesSummaryReportApiController : ApiController
    {
        private ISalesSummaryReportService _salesSummaryReportService { get; set; }
        private IWorkContext _workContext;
        private ICacheManager _cacheManager;
        public SalesSummaryReportApiController(ISalesSummaryReportService _salesSummaryReportService, IWorkContext _workContext, ICacheManager _iCacheManager)
        {
            this._salesSummaryReportService = _salesSummaryReportService;
            this._workContext = _workContext;
            this._cacheManager = _iCacheManager;
        }

        // Get Party wise GP Analysis Salse Summary
        [HttpPost]
        public PartyWiseGPAnalysisViewModel GetPartyWiseGPAnalysisSales(filterOption model)
        {
            var partyWiseList = new PartyWiseGPAnalysisViewModel();
            try
            {
                partyWiseList.PartyWiseGPAnalysisModel = _salesSummaryReportService.GetPartyWiseGPAnalysisSalesSummaryReport(model.ReportFilters, _workContext.CurrentUserinformation);
                partyWiseList.total = partyWiseList.PartyWiseGPAnalysisModel.Count();
                partyWiseList.AggregationResult = KendoGridHelper.GetAggregation(partyWiseList.PartyWiseGPAnalysisModel);
            }
            catch (Exception)
            {
                throw;
            }
            return partyWiseList;
        }



        [HttpPost]
        public IEnumerable<SalesAnalysisModel> GetSalesAnalysis(filterOption model)
        {
            return _salesSummaryReportService.getSalesAnalysis(model.ReportFilters, _workContext.CurrentUserinformation).ToList();
        }

        public IEnumerable<HighestSellingModel> GetAllHighestSelling(int selectedItem)
        {
          var data =  _salesSummaryReportService.GetAllHighestSelling(_workContext.CurrentUserinformation, selectedItem);
            return data;
        }

        [HttpPost]
        public IList<TopEmployeeListModel> GetTopSalesEmployee(filterOption model, int pageSize)
        {

            return _salesSummaryReportService.getTopSalesEmployee(model.ReportFilters, _workContext.CurrentUserinformation, pageSize).ToList();

        }

        [HttpPost]
        public IList<SalesAchieveModel> GetSalesAchieve(filterOption model, string duration)
        {
            return _salesSummaryReportService.getSalesAchieve(model.ReportFilters,duration, _workContext.CurrentUserinformation).ToList();
        }
        [HttpPost]
        public IList<SalesAchieveModel> GetSalesAchieveProjected(filterOption model, string duration)
        {
           var data= _salesSummaryReportService.getSalesAchieveProjected(model.ReportFilters, duration, _workContext.CurrentUserinformation).ToList();
            return data;
        }

        [HttpPost]
        public IList<PendingOrder> GetPendingOrderChart(filterOption model,bool branchwiseChart = false)
        {
            return _salesSummaryReportService.getPendingOrderChart(model.ReportFilters, branchwiseChart, _workContext.CurrentUserinformation).ToList();
        }


        [HttpPost]
        public IList<SalesAchieveModel> GetSalesAchieveBranch(filterOption model, string duration)
        {
            return _salesSummaryReportService.getSalesAchieveBranch(model.ReportFilters, duration, _workContext.CurrentUserinformation).ToList();
        }
        [HttpPost]
        public IList<SalesAchieveModel> GetSalesAchieveBranchProjection(filterOption model, string duration)
        {
            var cacheKey = $@"_salesAchieveVsProjection_{duration.Replace(' ', '_')}";
            var result = new List<SalesAchieveModel>();
            if (_cacheManager.IsSet(cacheKey))
            {
                result = _cacheManager.Get<List<SalesAchieveModel>>(cacheKey).ToList();
            }
            else {
                result= _salesSummaryReportService.getSalesAchieveBranchProjection(model.ReportFilters, duration, _workContext.CurrentUserinformation).ToList();
                _cacheManager.Set(cacheKey, result, 30);
            }
            return result;
        }
        [HttpPost]
        public IList<SalesAchieveModel> GetSalesAchieveDivision(filterOption model, string duration)
        {
            return _salesSummaryReportService.getSalesAchieveDivision(model.ReportFilters, duration, _workContext.CurrentUserinformation).ToList();
        }
        [HttpPost]
        public IList<SalesAchieveMonthModel> GetSalesAchieveDivisionMonth(filterOption model, string duration)
        {
            var data= _salesSummaryReportService.getSalesAchieveDivisionMonth(model.ReportFilters, duration, _workContext.CurrentUserinformation).ToList();
            var totalAmount = Math.Round(data.Sum(x => x.GROSSAMOUNT)??0,2);
            var totalTarget = Math.Round(data.Sum(x => x.TARGETAMOUNT)??0,2);
            var achivedPer =totalAmount==0?0: (totalAmount / (totalTarget == 0 ? totalAmount : totalTarget)) * 100;
            var SalesAchived = new List<SalesAchieveMonthModel>();
            foreach(var d in data)
            {
                var salesAchvedModel = new SalesAchieveMonthModel();
                salesAchvedModel.MONTH = d.MONTH;
               var achivedvaluePer =  d.GROSSAMOUNT==0?0:((d.TARGETAMOUNT-d.GROSSAMOUNT) / (d.TARGETAMOUNT==0? d.GROSSAMOUNT : d.TARGETAMOUNT))*100;
                salesAchvedModel.SalesAchived =Math.Round(Math.Abs(achivedvaluePer ?? 0),2);
                salesAchvedModel.order = 1;
                salesAchvedModel.reportType = "Achived";
                salesAchvedModel.MONTHINT = d.MONTHINT;
                salesAchvedModel.YEAR = d.YEAR;
                salesAchvedModel.TARGETAMOUNT = totalTarget;
                salesAchvedModel.achivedPercentage = achivedPer;
                SalesAchived.Add(salesAchvedModel);
                var salestargetModel = new SalesAchieveMonthModel();
                salestargetModel.MONTH = d.MONTH;
                salestargetModel.SalesAchived = achivedvaluePer < 0 ? 0 :Math.Abs(achivedvaluePer??0 - 100);
                salestargetModel.order = 2;
                salestargetModel.reportType = "Target";
                salestargetModel.MONTHINT = d.MONTHINT;
                salestargetModel.YEAR = d.YEAR;
                salestargetModel.TARGETAMOUNT = totalTarget;
                salestargetModel.achivedPercentage = achivedPer;
                SalesAchived.Add(salestargetModel);
               
            }
            return SalesAchived;
        }
        [HttpPost]
        public IList<SalesAchieveModel> GetSalesAchieveItemwise(filterOption model, string duration,string branch)
        {
            return _salesSummaryReportService.getSalesAchieveItemWise(model.ReportFilters, duration,branch, _workContext.CurrentUserinformation).ToList();
        }
        [HttpPost]
        public IList<SalesAchieveModel> GetSalesDivisionAchieveItemwise(filterOption model, string duration, string branch)
        {
            return _salesSummaryReportService.getSalesDivisionAchieveItemWise(model.ReportFilters, duration, branch, _workContext.CurrentUserinformation).ToList();
        }
        
        [HttpPost]
        public IList<TopDealerListModel> GetTopSalesDealer(filterOption model, int pageSize)
        {

            return _salesSummaryReportService.getTopSalesDealer(model.ReportFilters, _workContext.CurrentUserinformation, pageSize).ToList();

        }


        [HttpGet]
        public List<RegionWiseSalesModel> GetRegionWiseSales()
        {
            var result = _salesSummaryReportService.GetRegionWiseSales();
            return result;
        }

        [HttpGet]
        public List<ProductWiseGpModel> GetProductWiseGpAnalysis()
        {
            var result = _salesSummaryReportService.GetProductWiseGp();
            return result;
        }
        public RegionProductGpModel GetRegionProductSales()
        {
            var result = new RegionProductGpModel();
            result.Region = _salesSummaryReportService.GetRegionWiseSales();
            result.ProductGp = _salesSummaryReportService.GetProductWiseGp();
            return result;
        }


        [HttpPost]
        public List<WeeklyExpenseAnalysis> GetWeeklyExpenseAnalysis(ReportFiltersModel model)
        {
            var result = _salesSummaryReportService.GetWeeklyExpenseAnalysis(model,_workContext.CurrentUserinformation);
            return result;
        }


        [HttpPost]
        public List<WeeklyVendorPaymentAnalysis> GetWeeklyVendorPaymentAnalysis(ReportFiltersModel model)
        {
            var result = _salesSummaryReportService.GetWeeklyVendorPaymentAnalysis(model,_workContext.CurrentUserinformation);
            return result;
        }
        public List<SalesSummaryReportModel> GetSlowMovingItem(int selectedItem)
        {
            var data = _salesSummaryReportService.GetSlowMovingItem(_workContext.CurrentUserinformation, selectedItem);
            return data.ToList();
            //return this._salesRegister.GetProductWisePriceList(model.ReportFilters, _workContext.CurrentUserinformation).ToList();
        }
        public List<SalesSummaryReportModel> GetTopMovingItem(int selectedItem)
        {
            var data = _salesSummaryReportService.GetTopMovingItem(_workContext.CurrentUserinformation, selectedItem);
            return data.ToList();
            //return this._salesRegister.GetProductWisePriceList(model.ReportFilters, _workContext.CurrentUserinformation).ToList();
        }
        public List<SalesSummaryReportModel> GetNonMovingItem(int selectedItem)
        {
            var data = _salesSummaryReportService.GetNonMovingItem(_workContext.CurrentUserinformation, selectedItem);
            return data.ToList();
            //return this._salesRegister.GetProductWisePriceList(model.ReportFilters, _workContext.CurrentUserinformation).ToList();
        }
        public List<ProductLevelModel>GetProductLevel(int selectedItem)
        {
            return _salesSummaryReportService.GetProductLevel(_workContext.CurrentUserinformation, selectedItem).ToList();
        }
    }
}
