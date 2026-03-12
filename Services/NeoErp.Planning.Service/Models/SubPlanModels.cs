using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Planning.Service.Models
{
    public class SubPlanModels
    {
        public int PLAN_CODE { get; set; }
        public int SUBPLAN_CODE { get; set; }
        public int PLAN_DTL_CODE { get; set; }
        public string SUBPLAN_EDESC { get; set; }
        public int TARGET_VALUE { get; set; }
        public string REMARKS { get; set; }
        public int TIME_FRAME_CODE { get; set; }
        public string TIME_FRAME_EDESC { get; set; }
        public string ITEM_MASTER_CODE { get; set; }
        public string ITEM_PRE_CODE { get; set; }
        public string ITEM_CODE { get; set; }
        public int TIME_FRAME_VALUE { get; set; }
        public string ITEM_EDESC { get; set; }
        public int MasterCode { get; set; }
        public int PreCode { get; set; }
        public int LEVEL { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
    }

    public class PlanRegisterProductModel
    {
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public string CATEGORY_CODE { get; set; }
        public string PO_DISPLAY_DIST_ITEM { get; set; }
    }
    public class DivisionTree
    {
        public string divisionName { get; set; }
        public string divisionId { get; set; }
        public bool hasDivisions { get; set; }
        public int Level { get; set; }
        public string preDivisionCode { get; set; }
        public string masterDivisionCode { get; set; }
        public string groupSkuFlag { get; set; }
        public IEnumerable<DivisionTree> Items { get; set; }
    }

    public class DivisionModels
    {
        public int LEVEL { get; set; }
        public string DIVISION_CODE { get; set; }
        public string DIVISION_EDESC { get; set; }
        public string ABBR_CODE { get; set; }
        public string ADDRESS { get; set; }
        public string EMAIL { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string PRE_DIVISION_CODE { get; set; }
        public string MASTER_DIVISION_CODE { get; set; }
        public int? Childrens { get; set; }
    }

    public class CustomerModels
    {
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string MASTER_CUSTOMER_CODE { get; set; }
        public string PRE_CUSTOMER_CODE { get; set; }
        public int? Childrens { get; set; }
        public int LEVEL { get; set; }
    }

    public class BranchModels
    {
        public string BRANCH_CODE { get; set; }
        public string BRANCH_EDESC { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string PRE_BRANCH_CODE { get; set; }
        public int? Childrens { get; set; }
        public int LEVEL { get; set; }
    }
    public class AgentModels
    {
        public string AGENT_CODE { get; set; }
        public string AGENT_EDESC { get; set; }
        public string AGENT_ID { get; set; }
        public string CREDIT_DAYS { get; set; }
    }

    public class EmployeeTree
    {
        public string employeeName { get; set; }
        public string employeeId { get; set; }
        public bool hasEmployees { get; set; }
        public int Level { get; set; }
        public string preEmployeeCode { get; set; }
        public string masterEmployeeCode { get; set; }
        public string groupSkuFlag { get; set; }
        public IEnumerable<EmployeeTree> Items { get; set; }
    }

    public class AccountSetup
    {
        public string ACC_CODE { get; set; }
        public string  ACC_EDESC { get; set; }
    }

    public class CustomersTree
    {
        public string customerName { get; set; }
        public string customerId { get; set; }
        public bool hasCustomers { get; set; }
        public int Level { get; set; }
        public string preCustomerCode { get; set; }
        public string masterCustomerCode { get; set; }
        public string groupSkuFlag { get; set; }
        public IEnumerable<CustomersTree> Items { get; set; }
    }

    public class PARTY_TYPE_MODELS
    {
        public string PARTY_TYPE_CODE { get; set; }
        public string PARTY_TYPE_EDESC { get; set; }
    }
    public class EmployeeModels
    {
        public string EMPLOYEE_CODE { get; set; }
        public string EMPLOYEE_EDESC { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string MASTER_EMPLOYEE_CODE { get; set; }
        public string PRE_EMPLOYEE_CODE { get; set; }
        public int? Childrens { get; set; }
        public int LEVEL { get; set; }

    }

    public class DateRangeModels
    {
        public DateTime DATE_RANGES { get; set; }
        public string DATES { get; set; }
        public string YEAR { get; set; }
        public string MONTH { get; set; }
        public string MONTH_NAME { get; set; }
        public string WEEK { get; set; }
        public string DAY { get; set; }
    }
    public class DateFilterModel
    {

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string RangeName { get; set; }

        public int SortOrder { get; set; }

        public string StartDateString { get; set; }

        public string EndDateString { get; set; }
    }
    public class plFiscalYearModel
    {
        public string FISCAL_YEAR_CODE { get; set; }
        public DateTime START_DATE { get; set; }
        public DateTime END_DATE { get; set; }
    }

}
