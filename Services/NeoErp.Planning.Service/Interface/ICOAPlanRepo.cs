using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoErp.Core.Models;
using NeoErp.Planning.Service.Models;
using NeoErp.Core.Domain;
using NeoErp.Core.Helpers;

namespace NeoErp.Planning.Service.Interface
{
    public interface ICOAPlanRepo
    {
        List<COAPlanViewModal> getAllCOAPlans(ReportFiltersModel reportFilters);
        //List<COAPlanViewModal_Grid> getAllCOAPlans(ReportFiltersModel reportFilters);
        List<COASetupModel> COAListAllNodes(User userinfo);
        List<COASetupModel> GetCOAListByCOACode(string level, string masterProductCode, User userinfo);
        string SaveBudgetPlan(List<savePlan> parentArr, SalesPlan sp);
        COAPlanSetupDetailViewModal GetBudgetPlanDetailValueByPlanCode(int plancode);
        bool deleteSalesPlan(int planCode);
        List<ProductTree> ChartOfAccountList();
        bool cloneSalesPlan(string planCode, string planName, string branchs, string divisions, string remarks);
        List<PlanCOARefrenceModel> getDataForRefrence (string itemList, string startDate, string endDate, string divisionCode, string branchCode, string dateFormat, string frequency);
        string CreateTemBudgetPlanReportTable(ReportFiltersModel model);
    }
}
