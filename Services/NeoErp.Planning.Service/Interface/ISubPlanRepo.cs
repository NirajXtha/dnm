using NeoErp.Core.Domain;
using NeoErp.Planning.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Planning.Service.Interface
{
    public interface ISubPlanRepo
    {
        List<PlanModels> getPlanList(string filter);
        List<PlanModels> getSubPlanTimeFrame(string PLAN_CODE);
        List<SubPlanModels> ViewSubPlanReport(SubPlanModels viewdetails);
        PlanModels getPlan(string plancode);
        string GetFrequencyColumnByPlanCode(string plancode);
        List<SubPlanModels> getSubPlans(string plancode);
        List<SubPlanModels> getTargetValue(SubPlanModels details);
        string SaveSubPlan(string plancode, string SubPlanName);
        string SaveSubGroupWiseSubPlan(string subPlanCode, string itemCode, string preItemCode, string masterItemCode, string customerCode, string preCustomerCode, string masterCustomerCode, string targetValue, DateTime dateRange, string subgrouptype);
        List<SubPlanModels> getPlanDetailCode(string ItemCode);
        List<DateRangeModels> dateRange(string startDate, string endDate);
        List<CustomerModels> getAllCustomer();
        List<AccountSetup> getAllAccounts();
        List<EmployeeModels> getAllEmployees();
        List<DivisionModels> getAllDivisions();
        List<DateRangeModels> getAllDynamicColumnHeader(string plancode);
        List<CustomerModels> getAllCustomer(string filter);
        List<CustomerModels> getAllCustomerForPlan(string filter);
        List<DivisionModels> getAllDivision(string filter);
        List<BranchModels> getAllBranch(string filter);
        List<AgentModels> getAllAgent(string filter);
        List<EmployeeModels> getAllEmployee(string filter);
        List<PARTY_TYPE_MODELS> GetChiledLevelPartyType(string filter);
        List<EmployeeModels> getAllEmployeeForHrEmployeeTree(string filter);
        List<PlanRegisterProductModel> PlanRegisterProducts(User userinfo,string pageNameId);
        List<PlanRegisterProductModel> PlanRegisterProductsIndividual();
        int createEntities(string systemName);
        IEnumerable<DateFilterModel> GetNextFiscalYearDateFilters(string fiscalYear, string nFiscalYear, string textToAppend = "", bool appendText = false, int substractYear = 0);
        IEnumerable<DateFilterModel> GetFiscalYearDateFilters(string fiscalYear, string nFiscalYear, string textToAppend = "", bool appendText = false, int substractYear = 0);
        IEnumerable<DateFilterModel> GetEnglishDateFilters(string fiscalYear, string textToAppend = "", bool appendText = false);
    }
}
