using NeoErp.Core.Domain;
using NeoErp.Core.Helpers;
using NeoErp.Core.Models;
using NeoErp.LOC.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.LOC.Services.Services
{
    public interface ILcReportService
    {
        List<DueInvoiceReportModels> GetAllDueInvoiceReports(ReportFiltersModel reportFilters);
        List<PendingLcReportModels> GetAllPendingLcReports(ReportFiltersModel reportfilters);
        
        List<PendingLcReportModels> GetAllOpenLCReports(ReportFiltersModel reportfilters);
        List<LcStatusReportModels> GetAllLcStatusReports(ReportFiltersModel reportfilters);
        List<LcStatusReportModels> GetAllLcChildStatus(string lctrackno);
        List<VehicleMovementReportModels> GetAllVehicleMovementReports(ReportFiltersModel reportFilters);
        List<VehicleMovementReportModels> GetAllURVehicleMovementReports(ReportFiltersModel reportFilters);
        List<VehicleMovementReportModels> GetAllVehicleChildStatus(string lcnumber, string cinumber);
        List<ItemReportModels> GetAllLcChildPendingLc(string lcnumber, string brandname);
        List<ItemReportModels> GetAllLcChildOpenLc(string lcnumber, string brandname);
        List<LcProductWiseReportModels> GetAllLcProductWiseReport(ReportFiltersModel reportfilters);
        List<PendingCommercialInvoiceReportModels> GetAllPendingCommercialInvoiceReport(ReportFiltersModel reportfilters);

        List<POPendingReportModel> POPendingReport(ReportFiltersModel reportfilters);
        List<OptimizedPendingLcModel> GetAllOptimizedPendingLcReports(ReportFiltersModel reportfilters);
        List<PendingCIReportViewModel> GetPendingCIReports(ReportFiltersModel reportfilters);
        List<ExchangeGainLossReportViewModel> GetExchangeGainLossReports(ReportFiltersModel reportfilters);

        List<ExgGainLossReportVModel> GetExgGLReports(ReportFiltersModel reportfilters);

        List<MITReportViewModel> GetmitReports(ReportFiltersModel reportfilters);
    }
}
