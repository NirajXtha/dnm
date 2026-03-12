using NeoErp.Core.Domain;
using NeoErp.Core.Models;
using NeoErp.Planning.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Planning.Service.Interface
{
     public interface ICollectionPlanRepo
     {
        string SaveCollectionPlan(List<savePlan> parentArr, CollectionPlan sp);
        List<CollectionPlan> getAllCollectionPlans(ReportFiltersModel filters);
        List<CollectionPlan> getAllCollectionPlans(string customercode, string employeecode, string divisioncode, string branchcode, string startdate, string enddate);
        bool deleteCollectionPlan(int planCode);
        PL_BRD_CLTN_PLAN GetPlanDetailValueByPlanCode(int plancode);
        List<PlanSalesRefrenceModel> getSalesItemDataForRefrence(string itemList, string startDate, string endDate, string customerCode, string divisionCode, string branchCode, string employeeCode, string dateFormat, string frequency, string MATERIAL_PLAN_CODE, string REFERENCE_FLAG);
        List<PlanSalesRefrenceModel> getCollectionItemDataForStock(string itemList, string startDate, string endDate, string customerCode, string divisionCode, string branchCode, string dateFormat, string frequency);
        bool cloneCollectionPlan(string planCode, string planName, string customers, string employees, string branchs, string divisions, string remarks);
        string CreateTemCollectionPlanReportTable(ReportFiltersModel model);
        List<MaterialPlanModel> GetMaterialPlanList();
        List<ItemModel> getItemByPlanCode(string planCode);
       List<CollectionFromMaterialModel> GetAllRawMaterialByMaterialCode(string pCode);
        string GetEmployeeCodeByPlanCode(string plan_code);
        string GetCustomerCodeByPlanCode(string plan_code);
        List<ProductSetupModel> ProductListAllNodes(User userinfo);
        List<ProductSetupModel> GetProductsListByProductCode(string level, string masterProductCode, User userinfo);
        #region collection Report
        List<PlanReportModel> getMonthlyCollectionPlanChart(ReportFiltersModel filters);
        List<PlanReportModel> getEmployeeWiseCollectionPlanChart(ReportFiltersModel model);
        List<PlanReportModel> getEmployeeCustomerWiseCollectionPlanChart(ReportFiltersModel model,string EmpCode);
        List<PlanRegisterProductModel> PlanRegisterProducts(User userinfo);
        List<CollectionPlanTreeReportModel> GetTreewiseCustomerCollectionPlanReport(ReportFiltersModel reportFilters, User userInfo);
        #endregion
    }
}
