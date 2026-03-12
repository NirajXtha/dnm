using NeoErp.Core;
using NeoErp.Core.Helpers;
using NeoErp.Core.Models;
using NeoErp.LOC.Services.Models;
using NeoErp.LOC.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoERP.LOC.Controllers.Api
{
    public class LcReportController : ApiController
    {
        private ILcReportService _lcreportservice;
        private readonly IWorkContext _workContext;
        public LcReportController(ILcReportService lcreportservice, IWorkContext workContext)
        {
            _lcreportservice = lcreportservice;
            _workContext = workContext;
        }


        [HttpPost]
        public List<DueInvoiceReportModels> GetDueInvoiceReport(filterOption model)
        {

            return this._lcreportservice.GetAllDueInvoiceReports(model.ReportFilters).ToList();

        }

        [HttpPost]
        public List<PendingLcReportModels> PendingLcReport(filterOption model)
        {

            return this._lcreportservice.GetAllPendingLcReports(model.ReportFilters).ToList();

        }


        [HttpGet]
        public List<ItemReportModels> LcChildPendingLc(string lctrackno, string brandname)
        {

            return this._lcreportservice.GetAllLcChildPendingLc(lctrackno, brandname).ToList();

        }

        [HttpPost]
        public List<PendingLcReportModels> OpenLcReport(filterOption model)
        {

            return this._lcreportservice.GetAllOpenLCReports(model.ReportFilters).ToList();

        }


        [HttpGet]
        public List<ItemReportModels> LcChildOpenLc(string lctrackno, string brandname)
        {

            return this._lcreportservice.GetAllLcChildOpenLc(lctrackno, brandname).ToList();

        }


        [HttpPost]
        public List<LcStatusReportModels> LcStatusReport(filterOption model)
        {

            return this._lcreportservice.GetAllLcStatusReports(model.ReportFilters).ToList();

        }

        [HttpGet]
        public List<LcStatusReportModels> LcChildStatus(string lctrackno)
        {

            return this._lcreportservice.GetAllLcChildStatus(lctrackno).ToList();

        }

        [HttpPost]
        public List<VehicleMovementReportModels> VMovReport(filterOption model)
        {

            return this._lcreportservice.GetAllVehicleMovementReports(model.ReportFilters).ToList();

        }

        [HttpPost]
        public List<VehicleMovementReportModels> URVMovReport(filterOption model)
        {

            return this._lcreportservice.GetAllURVehicleMovementReports(model.ReportFilters).ToList();

        }

        [HttpGet]
        public List<VehicleMovementReportModels> VMovChildReport(string lctrackno, string cinumber)
        {

            return this._lcreportservice.GetAllVehicleChildStatus(lctrackno, cinumber).ToList();

        }


        [HttpPost]
        public List<LcProductWiseReportModels> LcProductWiseReport(filterOption model)
        {

            return this._lcreportservice.GetAllLcProductWiseReport(model.ReportFilters).ToList();

        }

        [HttpPost]
        public List<PendingCommercialInvoiceReportModels> PendingCommercialInvoiceReport(filterOption model)
        {

            return this._lcreportservice.GetAllPendingCommercialInvoiceReport(model.ReportFilters).ToList();

        }

        public List<POPendingReportModel> PoPendingReport(filterOption model)
        {
            return this._lcreportservice.POPendingReport(model.ReportFilters).ToList();
        }
        [HttpPost]
        public List<OptimizedPendingLcModel> OptimizedPendingLcReport(filterOption model)
        {
            //OptimizedPendingLcViewModel reportData = new OptimizedPendingLcViewModel();
            //reportData.OptimizedPendingLcModel = this._lcreportservice.GetAllOptimizedPendingLcReports(model.ReportFilters, _workContext.CurrentUserinformation).ToList();
            //reportData.total = reportData.OptimizedPendingLcModel.Count();
            //reportData.AggregationResult = KendoGridHelper.GetAggregation(reportData.OptimizedPendingLcModel, model.aggregate);
            //return reportData;
            return this._lcreportservice.GetAllOptimizedPendingLcReports(model.ReportFilters).ToList();
        }
        [HttpPost]
        public List<PendingCIReportViewModel> GetPendingCIReports(filterOption model)
        {
            return this._lcreportservice.GetPendingCIReports(model.ReportFilters).ToList();
        }
        [HttpPost]
        public List<ExchangeGainLossReportViewModel> GetExchangeGainLossReports(filterOption model)
        {
            return this._lcreportservice.GetExchangeGainLossReports(model.ReportFilters).ToList();
        }
        [HttpPost]
        public List<ExgGainLossReportVModel> GetExchangeGLReports(filterOption model)
        {
            return this._lcreportservice.GetExgGLReports(model.ReportFilters).ToList();
        }
        [HttpPost]
        public List<MITReportViewModel> GetMITReports(filterOption model)
        {
            
            return this._lcreportservice.GetmitReports(model.ReportFilters).ToList();
        }
    }
}
