using NeoErp.Core.Caching;
using NeoErp.Core.Helpers;
using NeoErp.Core.Services;
using NeoErp.Sales.Modules.Services.Models.AgeingReport;
using NeoErp.Sales.Modules.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using System.Net.Http;
using System.Web.Http;

namespace NeoErp.sales.Module.Controllers.Api
{
    public class AgeingReportController : ApiController
    {
        private IAgeingReportService _ageingReportService;
        private IControlService _controlService;

        public AgeingReportController(IAgeingReportService ageingReportService,IControlService controlService)
        {
            this._ageingReportService = ageingReportService;
            this._controlService = controlService;
        }

        [HttpPost]
        public List<AgeingDataViewModel> GetAgeingReport(AgeingFilterModel model)
        {
          
            return this._ageingReportService.GetAgeingReport(model).ToList();
        }
        [HttpPost]
        public List<AgeingDataViewModel> GetMonthlyAgeingReport(AgeingFilterModel model)

        {
            var FincalYear = System.Configuration.ConfigurationManager.AppSettings["FiscalYear"].ToString();
            var result = this._controlService.GetNepaliDateFilters(FincalYear).OrderByDescending(q => q.SortOrder).ToList();
            var listAgeing = new List<AgeingDataViewModel>();
            var asondate = Convert.ToDateTime(model.AsOnDate);
            var matchingMonth = result.Where(x => x.StartDate <= asondate && x.EndDate >= asondate).FirstOrDefault();
            var isloopBreak = false;
            foreach (var nepalimonth in result)
            {
                if (isloopBreak)
                    break;
                if (matchingMonth.RangeName == nepalimonth.RangeName)
                    isloopBreak = true;
                var ageingreport = new AgeingDataViewModel();
                ageingreport.Description = nepalimonth.RangeName;
                var modelconfig = new AgeingFilterModel();
                modelconfig = model;
                modelconfig.AsOnDate = nepalimonth.EndDate.ToString("yyyy-MM-dd");
                ageingreport.RangeColumnData= this._ageingReportService.GetAgeingChartReport(modelconfig).ToList();
                ageingreport.RangeColumnData.ForEach(x => x.Descriptions = nepalimonth.RangeName);
                ageingreport.Total= ageingreport.RangeColumnData.Select(q => q.NetAmount).Sum();
                listAgeing.Add(ageingreport);
               
            }

            return listAgeing;
        }


        [HttpPost]
        public List<AgeingDataViewModel> GetMonthlyDivisionAgeingReport(AgeingFilterModel model)

        {
            var FincalYear = System.Configuration.ConfigurationManager.AppSettings["FiscalYear"].ToString();
            var result = this._controlService.GetNepaliDateFilters(FincalYear).OrderByDescending(q => q.SortOrder).ToList();
            var listAgeing = new List<AgeingDataViewModel>();
            var asondate = Convert.ToDateTime(model.AsOnDate);
            var matchingMonth = result.Where(x => x.StartDate <= asondate && x.EndDate >= asondate).FirstOrDefault();
            var isloopBreak = false;
            foreach (var nepalimonth in result)
            {
                if (isloopBreak)
                    break;
                if (matchingMonth.RangeName == nepalimonth.RangeName)
                    isloopBreak = true;
                var ageingreport = new AgeingDataViewModel();
                ageingreport.Description = nepalimonth.RangeName;
                var modelconfig = new AgeingFilterModel();
                modelconfig = model;
                modelconfig.AsOnDate = nepalimonth.EndDate.ToString("yyyy-MM-dd");
                ageingreport.RangeColumnData = this._ageingReportService.GetAgeingChartReport(modelconfig).ToList();
                ageingreport.RangeColumnData.ForEach(x => x.Descriptions = nepalimonth.RangeName);
                ageingreport.Total = ageingreport.RangeColumnData.Select(q => q.NetAmount).Sum();
                listAgeing.Add(ageingreport);

            }

            return listAgeing;
        }

        [HttpPost]
        public List<AgeingDataViewModel.AgeingColumnRangeData> GetAgeingChartReport(AgeingFilterModel model)
        {
            model.ReportFilters.AreaTypeFilter.AddRange(model.Area);
            model.ReportFilters.CustomerFilter.AddRange(model.Codes);
            return this._ageingReportService.GetAgeingChartReport(model).ToList();
        }

        [HttpPost]
        public List<AgeingDataViewModel.AgeingColumnRangeData> GetAgeingChartReportBranchwise(AgeingFilterModel model)
        {
            model.ReportFilters.AreaTypeFilter.AddRange(model.Area);
            model.ReportFilters.CustomerFilter.AddRange(model.Codes);
            return this._ageingReportService.GetAgeingChartReportBranchWise(model).ToList();
        }
    }
}
