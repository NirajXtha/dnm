using NeoErp.Core.Helpers;
using NeoErp.Sales.Modules.Services.Models.SalesDashBoard;
using NeoErp.Sales.Modules.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoErp.sales.Module.Controllers.Api
{
    public class PlanChartReportApiController : ApiController
    {
        IPlanChartReport _iPlanChart { get; set; }
        public PlanChartReportApiController(IPlanChartReport iPlanChart)
        {
            this._iPlanChart = iPlanChart;
        }
        #region Sales Plan chart report
        [HttpPost]
        public List<PlanReportModel> GetSalesPlanMonthlyVsSalesReport(filterOption Model, string DateFormat)
        {
            return _iPlanChart.getMonthlySalesPlanVsSalesChart(Model.ReportFilters, DateFormat);
        }
        [HttpPost]
        public List<PlanReportModel> GetSalesPlanMonthlyVsSalesProductReport(filterOption Model, string DateFormat,int month)
        {
            return _iPlanChart.getMonthlySalesPlanVsSalesProductChart(Model.ReportFilters, DateFormat,month);
        }
        [HttpPost]
        public List<PlanReportModel> GetEmployeeWiseSalesPlanVsPreviousSalesChart(filterOption Model, string DateFormat)
        {
            return _iPlanChart.getEmployeeWiseSalesPlanVsPreviousSalesChart(Model.ReportFilters, DateFormat);
        }
        [HttpPost]
        public List<PlanReportModel> GetEmployeeCustomerWiseSalesPlanVsPreviousSalesChart(filterOption Model, string DateFormat,string EmpCode)
        {
            return _iPlanChart.getEmployeeCustomerWiseSalesPlanVsPreviousSalesChart(Model.ReportFilters, DateFormat,EmpCode);
        }
        [HttpPost]
        public List<PlanReportModel> GetDivisionWiseSalesPlanVsPreviousYearSalesChart(filterOption Model, string DateFormat)
        {
            return _iPlanChart.getDivisionWiseSalesPlanVsPreviousYearSalesChartChart(Model.ReportFilters, DateFormat);
        }
        [HttpPost]
        public List<PlanReportModel> GetDivisionEmployeeWiseSalesPlanVsPreviousYearSalesChartChart(filterOption Model, string DateFormat, string divisionCode)
        {
            return _iPlanChart.getDivisionEmployeeWiseSalesPlanVsPreviousYearSalesChartChart(Model.ReportFilters, DateFormat, divisionCode);
        }
        [HttpPost]
        public List<PlanReportModel> GetSalesPlanMonthlyReport(filterOption Model, string DateFormat )
        {
            return _iPlanChart.getMonthlySalesPlanChart(Model.ReportFilters, DateFormat);
        }
        [HttpPost]
        public List<PlanReportModel> GetSalesProductPlanMonthlyReport(filterOption Model, string DateFormat,int month)
        {
            return _iPlanChart.getMonthlyProductSalesPlanChart(Model.ReportFilters, DateFormat,month);
        }
        [HttpPost]
        public List<PlanReportModel> getDivisionWiseSalesPlanChart(filterOption Model, string DateFormat)
        {
            return _iPlanChart.getDivisionWiseSalesPlanChart(Model.ReportFilters, DateFormat);
        }
        [HttpPost]
        public List<PlanReportModel> getDivisionEmployeeWiseSalesPlanChart(filterOption Model, string DateFormat,string divisionCode)
        {
            return _iPlanChart.getDivisionEmployeeWiseSalesPlanChart(Model.ReportFilters, DateFormat,divisionCode);
        }
        [HttpPost]
        public List<PlanReportModel> getEmployeeWiseSalesPlanChart(filterOption Model, string DateFormat)
        {
            return _iPlanChart.getEmployeeWiseSalesPlanChart(Model.ReportFilters, DateFormat);
        }
        [HttpPost]
        public List<PlanReportModel> getEmployeeCustomerWiseSalesPlanChart(filterOption Model, string DateFormat,string EmpCode)
        {
            return _iPlanChart.getEmployeeCustomerWiseSalesPlanChart(Model.ReportFilters, DateFormat,EmpCode);
        }
        [HttpPost]
        public List<PlanReportModel>getItemWiseQuantitySalesPlanReport(filterOption Model,string DateFormat)
        {
            return _iPlanChart.getItemWiseQuantitySalesPlanChart(Model.ReportFilters, DateFormat);
        }
        [HttpPost]
        public List<PlanReportModel> GetSalesVsTargetMonthlyReport(filterOption Model, string DateFormat)
        {
            return _iPlanChart.getMonthlySalesVsTargetChart(Model.ReportFilters, DateFormat);
        }
        [HttpPost]
        public List<PlanReportModel> GetSalesVsTargetMonthlyByProductReport(filterOption Model, string DateFormat, int month)
        {
            return _iPlanChart.getMonthlySalesVsTargetByProductChart(Model.ReportFilters, DateFormat, month);
        }
        #endregion
        #region Procurement plan report
        [HttpPost]
        public List<PlanReportModel> getMonthlyProcurementPlanChart(filterOption model, string DateFormat)
        {
            return _iPlanChart.getMonthlyProcurementPlanChart(model.ReportFilters, DateFormat);
        }
        #endregion
        #region Production plan report
        [HttpPost]
        public List<PlanReportModel> getMonthlyProductionPlanChart(filterOption model, string DateFormat)
        {
            return _iPlanChart.getMonthlyProductionPlanChart(model.ReportFilters, DateFormat);
        }
        #endregion
        #region Ledger plan report
        [HttpPost]
        public List<PlanReportModel> getMonthlyLedgerPlanChart(filterOption model, string DateFormat)
        {
            return _iPlanChart.getMonthlyLedgerPlanChart(model.ReportFilters, DateFormat);
        }
        #endregion
        #region Budget plan report
        [HttpPost]
        public List<PlanReportModel> getMonthlyBudgetPlanChart(filterOption model, string DateFormat)
        {
            return _iPlanChart.getMonthlyBudgetPlanChart(model.ReportFilters, DateFormat);
        }
        #endregion

    }
}
