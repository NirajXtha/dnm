using NeoErp.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoErp.Planning.Service.Models;
using NeoErp.Core.Helpers;

namespace NeoErp.Planning.Service.Interface
{
    public interface IMaterialPlanRepo
    {

        string SaveMaterialPlan(List<MaterialPlanModel> parentArr, MaterialPlan sp);
        List<MaterialPlan> getAllMaterialPlans(ReportFiltersModel filters);
        List<MaterialPlan> getGroupMaterialPlans(ReportFiltersModel filters);
        List<MaterialPlan> getAllMaterialPlans(string customercode, string employeecode, string divisioncode, string branchcode, string startdate, string enddate);
        bool deleteMaterialPlan(int planCode);
        PL_MATERIAL_PLAN GetPlanDetailValueByPlanCode(int plancode);
        List<PlanSalesRefrenceModel> getSalesItemDataForRefrence(string itemList, string startDate, string endDate, string customerCode, string divisionCode, string branchCode, string dateFormat, string frequency);
        List<PlanSalesRefrenceModel> getPorcumentItemDataForStock(string itemList, string startDate, string endDate, string customerCode, string divisionCode, string branchCode, string dateFormat, string frequency);
        List<PlanSalesRefrenceModel> getProductionItemDataForRefrence(string itemList, string startDate, string endDate, string branchCode, string dateFormat, string frequency);
        bool cloneMaterialPlan(string planCode, string planName, string customers, string employees, string branchs, string divisions, string remarks);
        List<PlanProductTree> getAllFGProducts();
        List<MaterialPlanModel> GetAllRawMaterialByFinishGood(GetMaterialPlanModel model);
        List<MaterialPlanModel> getAllChildItemsForMaterialPlan(GetMaterialPlanModel model);
        string SaveMaterialPlans(SaveMaterialPlanModel model);
        string SaveMaterialPlanReference(SaveMaterialPlanReferenceModel model);
        List<MaterialPlanModel> getPlanDetailById(string planCode);
        List<ProcurementPlan> BindSalesPlanByPlanCode();
        List<ProcurementPlan> BindProductionPlanByPlanCode();
        List<SalesOrderModel> BindSalesOrderCustomer();
        List<ProcureFromMaterialModel> GetAllSalesPlanItemByPlanCode(string planCode);
        List<ProcureFromMaterialModel> GetAllProductionPlanItemByPlanCode(string planCode);
        List<ProcureFromMaterialModel> GetAllRawMaterialByProductionPlanCode(string pCode);

        List<ProcureFromMaterialModel> GetAllRawMaterialBySalesPlanCode(string pCode);
        List<ProcureFromMaterialModel> GetAllRawMaterialBySalesOrderCustomerCode(string pCode, string startDate, string endDate);

        string SaveMaterialPlanReference(List<savePlan> obj, SalesPlan sp);

        string NewVoucherNo(string companycode, string formcode, string transactiondate, string tablename);

        bool InsertMaterialPlanPI(IEnumerable<checklist> modellist,string from_department, string to_department,string voucherno);
      
    }
}
