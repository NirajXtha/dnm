using NeoErp.Core.Models;
using NeoErp.Sales.Modules.Services.Models.SalesDashBoard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Services
{
    public interface IPlanChartReport 
    {
        List<PlanReportModel> getMonthlySalesPlanChart(ReportFiltersModel filters, string DateFormat);
        List<PlanReportModel> getMonthlyProductSalesPlanChart(ReportFiltersModel filters, string DateFormat,int month);
        List<PlanReportModel> getDivisionWiseSalesPlanChart(ReportFiltersModel model, string DateFormat);
        List<PlanReportModel> getDivisionEmployeeWiseSalesPlanChart(ReportFiltersModel model, string DateFormat,string divisionCode);
        List<PlanReportModel> getEmployeeWiseSalesPlanChart(ReportFiltersModel model, string DateFormat);
        List<PlanReportModel> getEmployeeCustomerWiseSalesPlanChart(ReportFiltersModel model, string DateFormat,string EmpCode);
        List<PlanReportModel> getMonthlyProcurementPlanChart(ReportFiltersModel model, string DateFormat);
        List<PlanReportModel> getMonthlyProductionPlanChart(ReportFiltersModel model, string DateFormat);
        List<PlanReportModel> getMonthlyLedgerPlanChart(ReportFiltersModel model, string DateFormat);
        List<PlanReportModel> getMonthlyBudgetPlanChart(ReportFiltersModel model, string DateFormat);
        List<PlanReportModel> getMonthlySalesPlanVsSalesChart(ReportFiltersModel model, string DateFormat);
        List<PlanReportModel> getMonthlySalesPlanVsSalesProductChart(ReportFiltersModel model, string DateFormat,int month);
        List<PlanReportModel> getEmployeeWiseSalesPlanVsPreviousSalesChart(ReportFiltersModel model, string DateFormat);
        List<PlanReportModel> getEmployeeCustomerWiseSalesPlanVsPreviousSalesChart(ReportFiltersModel model, string DateFormat, string EmpCode);//VsPreviousYearSalesChart
        List<PlanReportModel> getDivisionWiseSalesPlanVsPreviousYearSalesChartChart(ReportFiltersModel model, string DateFormat);
        List<PlanReportModel> getDivisionEmployeeWiseSalesPlanVsPreviousYearSalesChartChart(ReportFiltersModel model, string DateFormat, string divisionCode);
        List<PlanReportModel> getItemWiseQuantitySalesPlanChart(ReportFiltersModel model, string DateFormat);
        List<PlanReportModel> getMonthlySalesVsTargetChart(ReportFiltersModel model, string DateFormat);
        List<PlanReportModel> getMonthlySalesVsTargetByProductChart(ReportFiltersModel model, string DateFormat, int month);
    }
}
