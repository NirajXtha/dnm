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
    public interface IPlan
    {
        string AddUpdateFrequencies(FrequencyModels model);
        void deleteFrequency(int freqcode);
        List<FrequencyModels> getAllFrequency();
        string generatePlan(PlanModels plandetail);
        List<PlanModels> getAllPlans();
        List<FrequencyModels> getFrequencyByFilter(string filter);
        bool checkifexists(FrequencyModels model);
        List<SalesPlan> getAllSalesPlans(ReportFiltersModel filters);
        bool deleteSalesPlan(int planCode);
        bool approvedSalesPlan(int planCode, bool isChecked);
        bool cloneSalesPlan(int planCode);
        List<ProductSetupModel> GetProductsListWithChild(User userinfo);
        List<ProductSetupModel> GetProductsListWithChild(string level, string masterProductCode, User userinfo);
        string getUsersEmployeeCode();
        List<ProductSetupModel> GetProductListWithChildByPreCode(string preItemCode, User userinfo);
        List<ProductSetupModel> getAllProductsWithChildItem(string pageNameId);
        bool cloneSalesPlan(string planCode, string planName, string customers, string employees, string branchs, string divisions,string partyType, string remarks);
        List<PlanModels> GetPlanDetailByPlanCode(string planCode, User userinfo);
        bool UpdateSalesPlan(SalesPlan sp);
        string GetDivisionFlag();
        List<PreferenceSetupModel> GetPreferenceSetupFlag();
        List<PreferenceSetupModel> GetProcuremntSetupFlag();
        List<PreferenceSetupModel> GeLedgerPlanSetupFlag();
        List<PreferenceSetupModel> GetBudgetPlanSetupFlag();
        List<PreferenceSetupModel> GetProductionPlanSetupFlag();
        List<PreferenceSetupModel> GetBrandingSetupFlag();
        List<PreferenceSetupModel> GetCollectionSetupFlag();

        List<SalesPlan> getAllSalesPlans(string customercode, string employeecode, string divisioncode, string branchcode, string startdate, string enddate);
        string SaveMasterPlan(string masterPlanCode, string masterPlanName, string startDate, string endDate, string customer, string employee, string division, string branch,string selectedPlans);
        List<SalesPlans_CustomersEmployees> GetDateWiseCustomerEmployeeDivisions(string startdate, string enddate);
        List<SalesPlans_CustomersEmployees> GetDateWiseCustomerEmployeeDivisions(string startdate, string enddate,string getFor);
        List<MasterSalesPlan> getMasterSalesPlanList();
        bool DeleteMasterSalesPlan(string code);
        MasterSalesPlan getMasterSalesPlanDetail(string masterplancode);
        List<SalesPlanMap> getMasterSalesPlanDetailList(string masterplancode);
        string GetEmployeeByID(string userId);
        List<HREmployeeTreeModel> GetParentEmployeeTreeList();
        string SaveParentEmployeeTreeList(string parentEmployeeCode, string selectedEmployees);
        string DeleteEmployeeFromTree(string parent_employee_code, string employee_code);

        List<Employee> GetEmployeesList();
        List<PreferenceSetupModel> GetSalesPlanSetupFlag();
        string GetDashboardWidgets(string name, string type);
        List<AggregatePlanModel> GetAggregatePlanType(string filter);
        List<AggregatePlanModel> GetAllPlansWithType();
        List<EmployeeHandoverModel> getEmployeeHandoverList();
        List<PlanListModel> getEmployeeHandoverListWithPlan(string fromEmpCode,string toEmpCode);
        List<ProductCategory> GetAllCategory(string filter = null);
        string SaveConfigSetup(ConfigSetupModel model);
        List<User> getAllUsers(string filter = null);
    }
}
