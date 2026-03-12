using NeoErp.Core.Helpers;
using NeoErp.Planning.Service.Interface;
using NeoErp.Planning.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoERP.Planning.Controllers.Api
{
    public class PlanReportApiController : ApiController
    {
        private IPlanReport _iplanReport { get; set; }

        public PlanReportApiController(IPlanReport iPlanReport)
        {
            this._iplanReport = iPlanReport;
        }

        [HttpPost]
        public List<PlanReportModel> GetSalesPlanReport(filterOption model)
        {
            return _iplanReport.getAllSalesPlanReport(model.ReportFilters);
        }
        [HttpPost]
        public List<PlanReportModel> GetMonthlySalesPlan(filterOption model, string datetype)
        {
            var result = _iplanReport.getMonthlySalesPlanReport(model.ReportFilters, datetype);
            return result;
        }
        [HttpPost]
        public List<PlanReportModel> GetFavSalesPlanReport(filterOption model)
        {
            return _iplanReport.getFavSalesPlanReport(model.ReportFilters,model.page,model.pageSize);
        }
        [HttpPost]
        public List<PlanReportModel> GetMasterSalesPlanReport(filterOption model)
        {
            return _iplanReport.getMasterSalesPlanReport(model.ReportFilters);
        }
        [HttpPost]
        public List<PlanReportModel> GetMonthlySalesPlanChartReport(filterOption model)
        {
            return _iplanReport.getMonthlySalesPlanChart(model.ReportFilters);
        }
        [HttpPost]
        public List<PlanReportModel> getDivisionWiseSalesPlanChart(filterOption model)
        {
            return _iplanReport.getDivisionWiseSalesPlanChart(model.ReportFilters);
        }
        [HttpPost]
        public List<PlanReportModel> getEmployeeWiseSalesPlanChart(filterOption model)
        {
            return _iplanReport.getEmployeeWiseSalesPlanChart(model.ReportFilters);
        }
        [HttpPost]
        public List<PlanReportModel> GetFavBrandingPlanReport(filterOption model)
        {
            return _iplanReport.getFavBrandingPlanReport(model.ReportFilters);
        }
        [HttpPost]
        public List<PlanReportModel> GetFavProcurementPlanReport(filterOption model)
        {
            return _iplanReport.getFavProcurementPlanReport(model.ReportFilters);
        }
        [HttpPost]
        public List<PlanReportModel> GetFavLedgerPlanReport(filterOption model)
        {
            return _iplanReport.getFavLedgerPlanReport(model.ReportFilters);
        }
        [HttpPost]
        public List<PlanReportModel> GetFavBudgetPlanReport(filterOption model)
        {
            return _iplanReport.getFavBudgetPlanReport(model.ReportFilters);
        }
        [HttpPost]
        public List<PlanReportModel> GetFavProductionPlanReport(filterOption model)
        {
            return _iplanReport.getFavProductionPlanReport(model.ReportFilters);
        }

        [HttpPost]
        public List<PlanReportModel> GetFavMaterialPlanReport(filterOption model)
        {
            return _iplanReport.getFavMaterialPlanReport(model.ReportFilters);
        }
        [HttpPost]
        public HttpResponseMessage CreateTempSalesChartPlanReportTable(filterOption model)
        {
            if (model == null)
                model = new filterOption();
            //var result = "";
           var result = this._iplanReport.CreateTempSalesChartPlanReportTable(model.ReportFilters);

            if (result == "TableDropedAndCreated")
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "DELETECREATED", STATUS_CODE = (int)HttpStatusCode.OK });
            }
            else if (result == "TableCreated")
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "CREATED", STATUS_CODE = (int)HttpStatusCode.OK });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Fail", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        [HttpPost]
        public List<PlanReportModel> GetSalesPlanMonthlyReportByItemGroup(filterOption Model, string groupItemCode)
        {
            return _iplanReport.getMonthlySalesPlanByItemGroup(Model.ReportFilters, groupItemCode);
        }
        [HttpPost]
        public List<PlanReportModel> GetSalesPlanMonthlyReportCompareByItemGroup(filterOption Model,string groupItemCode)
        {
            return _iplanReport.getMonthlySalesPlanCompareByItemGroup(Model.ReportFilters, groupItemCode);
        }
        [HttpPost]
        public List<PlanReportModel> GetEmpwiseSalesPlanReportByItemGroup(filterOption Model, string groupItemCode)
        {
            return _iplanReport.getEmpwiseSalesPlanByItemGroup(Model.ReportFilters, groupItemCode);
        }
        [HttpPost]
        public List<PlanReportModel> GetEmpwiseSalesPlanReportCopmareByItemGroup(filterOption Model, string groupItemCode)
        {
            return _iplanReport.getEmpwiseSalesPlanCompareByItemGroup(Model.ReportFilters, groupItemCode);
        }
        public List<ItemGroupModel>GetItemGroup()
        {
            return _iplanReport.GetItemGroup();
        }
    }
}
