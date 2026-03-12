using NeoErp.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoErp.Planning.Service.Models;

namespace NeoErp.Planning.Service.Interface
{
    public interface IProcurementPlanRepo
    {

        string SaveProcurementPlan(List<savePlan> parentArr, ProcurementPlan sp);
        List<ProcurementPlan> getAllProcurementPlans(ReportFiltersModel filters);
        List<ProcurementPlan> getAllProcurementPlans(string customercode, string employeecode, string divisioncode, string branchcode, string startdate, string enddate);
        bool deleteProcurementPlan(int planCode);
        PL_BRD_PRCMT_PLAN GetPlanDetailValueByPlanCode(int plancode);
        List<PlanSalesRefrenceModel> getSalesItemDataForRefrence(string itemList, string startDate, string endDate, string customerCode, string divisionCode, string branchCode, string employeeCode, string dateFormat, string frequency, string MATERIAL_PLAN_CODE, string REFERENCE_FLAG);
        List<PlanSalesRefrenceModel> getPorcumentItemDataForStock(string itemList, string startDate, string endDate, string customerCode, string divisionCode, string branchCode, string dateFormat, string frequency);
        bool cloneProcurementPlan(string planCode, string planName, string customers, string employees, string branchs, string divisions, string remarks);
        string CreateTemProcurementPlanReportTable(ReportFiltersModel model);
        List<MaterialPlanModel> GetMaterialPlanList();
        List<ItemModel> getItemByPlanCode(string planCode);
        List<ProcureFromMaterialModel> GetAllRawMaterialByMaterialCode(string pCode);
    }
}
