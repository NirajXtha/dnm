using NeoErp.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoErp.Planning.Service.Models;
using NeoErp.Core.Domain;

namespace NeoErp.Planning.Service.Interface
{
   public interface IProductionPlanRepo
    {

        string SaveProductionPlan(List<savePlan> parentArr, ProductionPlan sp);
        List<ProductionPlan> getAllProductionPlans(ReportFiltersModel filters);
        List<ProductionPlan> getAllProductionPlans(string customercode, string employeecode, string divisioncode, string branchcode, string startdate, string enddate);
        bool deleteProductionPlan(int planCode);
        PL_PRO_PLAN GetPlanDetailValueByPlanCode(int plancode);

        List<ProductSetupModel> getAllProductsWithChildItem();
        List<ProductSetupModel> ProductListAllNodes(User userinfo);
        List<ProductSetupModel> GetProductsListByProductCode(string level, string masterProductCode, User userinfo);
        List<PlanSalesRefrenceModel> getSalesItemDataForRefrence(string itemList, string startDate, string endDate, string customerCode, string divisionCode, string branchCode, string dateFormat, string frequency);
        List<PlanSalesRefrenceModel> getPorcumentItemDataForStock(string itemList, string startDate, string endDate, string customerCode, string divisionCode, string branchCode, string dateFormat, string frequency);
        bool cloneProductionPlan(string planCode, string planName, string customers, string employees, string branchs, string divisions, string remarks);
        string CreateTemProductionPlanReportTable(ReportFiltersModel model);

        List<PorductionDetailsModel> GetPorductionDetails(string startDate, string endDate, string pCode);


    }
}
