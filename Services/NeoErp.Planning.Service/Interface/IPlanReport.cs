using NeoErp.Core.Models;
using NeoErp.Planning.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Planning.Service.Interface
{
    public interface IPlanReport
    {
        List<PlanReportModel> getAllSalesPlanReport(ReportFiltersModel filters);
        List<PlanReportModel> getMonthlySalesPlanReport(ReportFiltersModel filters, string dateType);
        List<PlanReportModel> getFavSalesPlanReport(ReportFiltersModel filters,int page,int size);
        List<PlanReportModel> getMasterSalesPlanReport(ReportFiltersModel filters);
        List<PlanReportModel> getMonthlySalesPlanChart(ReportFiltersModel filters);
        List<PlanReportModel> getDivisionWiseSalesPlanChart(ReportFiltersModel model);
        List<PlanReportModel> getEmployeeWiseSalesPlanChart(ReportFiltersModel model);
        List<PlanReportModel> getFavBrandingPlanReport(ReportFiltersModel filters);
        List<PlanReportModel> getFavProductionPlanReport(ReportFiltersModel filters);
        List<PlanReportModel> getFavProcurementPlanReport(ReportFiltersModel filters);
        List<PlanReportModel> getFavLedgerPlanReport(ReportFiltersModel filters);
        List<PlanReportModel> getFavBudgetPlanReport(ReportFiltersModel filters);
        string CreateTempSalesChartPlanReportTable(ReportFiltersModel model);
        List<PlanReportModel> getFavMaterialPlanReport(ReportFiltersModel model);
        List<PlanReportModel> getMonthlySalesPlanByItemGroup(ReportFiltersModel filters, string groupItemCode);
        List<PlanReportModel> getMonthlySalesPlanCompareByItemGroup(ReportFiltersModel filters,string groupItemCode);
        List<PlanReportModel> getEmpwiseSalesPlanByItemGroup(ReportFiltersModel filters, string groupItemCode);
        List<PlanReportModel> getEmpwiseSalesPlanCompareByItemGroup(ReportFiltersModel filters, string groupItemCode);
        List<ItemGroupModel> GetItemGroup();

    }
}
