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
   public interface IBrandingPlanRepo
    {

        string SaveBrandingPlan(List<savePlan> parentArr, BrandingPlan sp);
       List<BrandingPlan> getAllBrandingPlans(ReportFiltersModel filters);
        List<BrandingPlan> getAllBrandingPlans(string customercode, string employeecode, string divisioncode, string branchcode, string startdate, string enddate);
        bool deleteBrandingPlan(int planCode);
        COAPlanSetupDetailViewModal GetBudgetPlanDetailValueByPlanCode(int plancode);
        List<PlanSalesRefrenceModel> getSalesItemDataForRefrence(string itemList, string startDate, string endDate, string customerCode, string divisionCode, string branchCode, string dateFormat, string frequency);
        List<PlanSalesRefrenceModel> getBrandingItemDataForStock(string itemList, string startDate, string endDate, string customerCode, string divisionCode, string branchCode, string dateFormat, string frequency);
        bool cloneBrandingPlan(string planCode, string planName, string customers, string employees, string branchs, string divisions, string remarks);
        List<ProductSetupModel> getAllBrandingActivityWithChildItem();
        List<ProductTree> ChartOfAccountList();
    }
}
