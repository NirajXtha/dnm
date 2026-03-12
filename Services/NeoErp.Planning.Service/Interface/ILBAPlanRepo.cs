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
    public interface ILBAPlanRepo
    {
        List<LBAPlanViewModal> getAllLBAPlans(ReportFiltersModel reportFilters);
        //List<LBAPlanViewModal_Grid> getAllLBAPlans(ReportFiltersModel reportFilters);
        List<LBASetupModel> LBAListAllNodes(User userinfo);
        List<LBASetupModel> GetLBAListByLBACode(string level, string masterProductCode, User userinfo);
        string SaveLedgerBudgetPlan(List<savePlan> parentArr, SalesPlan sp);
        LBAPlanSetupDetailViewModal GetLedgerBudgetPlanDetailValueByPlanCode(int plancode);
        bool deleteSalesPlan(int planCode);
        List<ProductTree> ChartOfAccountList();
        List<AccountSetup> getAllAccounts();
        List<ProductSetupModel> getAllProductsWithChildItem(string accCode);
        bool cloneSalesPlan(string planCode, string planName, string branchs, string divisions, string remarks);
        List<PlanLBARefrenceModel> getDataForRefrence (string plan_code,string itemList, string subCode, string startDate, string endDate, string divisionCode, string branchCode, string dateFormat, string frequency);
        string getTotalReferenceAmount(string acc_code);
        string CreateTemLBPlanReportTable(ReportFiltersModel model);
    }
}
