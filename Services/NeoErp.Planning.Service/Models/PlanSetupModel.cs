using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Planning.Service.Models
{
    public class ItemModel
    {
        public string ITEM_EDESC { get; set; }
        public string ITEM_CODE { get; set; }
        public string PARENTID { get; set; }
        public string PRE_ITEM_CODE { get; set; }
        public string MASTER_ITEM_CODE { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public bool hasChildren { get; set; } = true;
        public string[] FREQUENCY { get; set; }
        public bool hasItems { get; set; }
        public string IS_CHILD_SELECTED { get; set; }
        public int Level { get; set; }
        public IEnumerable<ItemModel> Items { get; set; }
    }

    public class EmployeeHandoverModel
    {
        public string TO_EMPLOYEE_CODE { get; set; }
        public string FROM_EMPLOYEE_CODE { get; set; }
        public string TO_EMPLOYEE_EDESC { get; set; }
        public string FROM_EMPLOYEE_EDESC { get; set; }
        public string FROM_DATE { get; set; }
        public string PLAN_CODE { get; set; }
    }
    public class PlanListModelForMigrate
    {
        public int? PLAN_CODE { get; set; }
    }

    public class PlanListModel
    {
        public int? PLAN_CODE { get; set; }
        public string PLAN_EDESC { get; set; }
        public DateTime? START_DATE { get; set; }
        public DateTime? END_DATE { get; set; }
    }
    public class SalesItemForReferenceModel
    {
        public string ItemList { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string customerCode { get; set; }
        public string divisionCode { get; set; }
        public string branchCode { get; set; }
        public string dateFormat { get; set; }
        public string frequency { get; set; }
        public string FiscalYear { get; set; }
        public string salesFlag { get; set; }
        public string planFor { get; set; }
    }
    public class PlanSetupTitleModel
    {
        public string ITEM_EDESC { get; set; }
        public string ITEM_CODE { get; set; }
        public string START_DATE { get; set; }
        public string END_DATE { get; set; }
        public string PLAN_TYPE { get; set; }
        public string TIME_FRAME_CODE { get; set; }
        public string TIME_FRAME_EDESC { get; set; }
    }
    public class planDetailModel
    {
        public int PL_PLAN_NEXT_CODE { get; set; }
    }
    public class FrequencyColumnModel
    {
        public string PLAN_CODE { get; set; }
        public string PLAN_EDESC { get; set; }
        public string START_DATE { get; set; }
        public string END_DATE { get; set; }
        public string DAYS { get; set; }
        public string DAY { get; set; }
        public string MONTH { get; set; }
        public string YEAR { get; set; }
        public string YEARCOUNT { get; set; }
        public string MONTHCOUNT { get; set; }
        public string FREQUENCY { get; set; }
        public string TIME_FRAME_DAYS { get; set; }
        public string MONTHINT { get; set; }
        public string WEEKS { get; set; }
        public string NWEEKS { get; set; }

        public string YEARWEEK { get; set; }
    }

    public class YearMonthWeekModel
    {
        public string YEARWEEK { get; set; }

        public string YEARWEEK1 { get; set; }
        public string NWEEK { get; set; }
        public string EWEEK { get; set; }
        public string BS_MONTH { get; set; }
    }

    public class freqNameVlaue
    {
        public string fname { get; set; }
        public string fvalue { get; set; }
        public string fvalue_amt { get; set; }
        public List<string> skipMonth { get; set; }
    }
    public class savePlan
    {
        public savePlan()
        {
            frequency = new List<freqNameVlaue>();
        }
        public string name { get; set; }
        public string value { get; set; }
        public string value_amt { get; set; }
        public string itemCode { get; set; }
        public List<string> skipMonth { get; set; }
        public List<freqNameVlaue> frequency { get; set; }

    }
    public class ProductSetupModel
    {
        public int LEVEL { get; set; }
        public string ItemName { get; set; }
        public string GroupFlag { get; set; }
        public string ItemCode { get; set; }
        public decimal? Rate { get; set; }
        public string MasterItemCode { get; set; }
        public string PreItemCode { get; set; }
        public int? Childrens { get; set; }
    }
    //public class ConsolidateSetupModel
    //{
    //    public int LEVEL { get; set; }
    //    public string BRANCH_EDESC { get; set; }
    //    public string GroupFlag { get; set; }
    //    public string BRANCH_CODE { get; set; }
    //    public string MasterBranchCode { get; set; }
    //    public string PRE_BRANCH_CODE { get; set; }
    //    public int? Childrens { get; set; }
    //}
    //public class BranchSetupModel
    //{
    //    public int LEVEL { get; set; }
    //    public string branchName { get; set; }
    //    public string GroupFlag { get; set; }
    //    public string BranchCode { get; set; }
    //    public string PreBranchCode { get; set; }
    //    public int? Childrens { get; set; }
    //}

    public class ProductModel
    {
        public int LEVEL { get; set; }
        public string ITEM_EDESC { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string ITEM_CODE { get; set; }
        public string MASTER_ITEM_CODE { get; set; }
        public string PRE_ITEM_CODE { get; set; }
        public int? Childrens { get; set; }
    }

    public class SalesPlan
    {
        public string END_DATE { get; set; }
        public string IS_ITEMS_VISIBLE_ONLY { get; set; }
        public string ITEM_CODE { get; set; }
        public string IsCustomerProduct { get; set; }
        public string PLAN_CODE { get; set; }
        public string PLAN_EDESC { get; set; }
        public string PLAN_NDESC { get; set; }
        public string PLAN_FOR { get; set; }
        public string PLAN_TYPE { get; set; }
        public string SALES_TYPE { get; set; }
        public string REMARKS { get; set; }
        public string START_DATE { get; set; }
        public string TIME_FRAME_CODE { get; set; }
        public string TIME_FRAME_EDESC { get; set; }
        public string customerCode { get; set; }
        public string branchCode { get; set; }
        public string accountCode { get; set; }
        public string partytypeCode { get; set; }
        public string agentCode { get; set; }
        public string divisionCode { get; set; }
        public string employeeCode { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string dateFormat { get; set; }
        public string salesRateType { get; set; }
        public string SALES_QUANTITY { get; set; }
        public string SALES_AMOUNT { get; set; }
        public string CALENDAR_TYPE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string DIVISION_EDESC { get; set; }
        public string BRANCH_EDESC { get; set; }
        public string EMPLOYEE_CODE { get; set; }
        public string EMPLOYEE_EDESC { get; set; }
        public string PARTY_TYPE_EDESC { get; set; }
        public string APPROVED_FLAG { get; set; }
        public string CREATED_BY { get; set; }
        public string APPROVED_BY { get; set; }
        public string APPROVED_BY_CODE { get; set; }
        public string APPROVED_BY_EMP_CODE { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string SUPER_FLAG { get; set; }
        public List<SalesPlanItems> selectedItemsList { get; set; }
        public List<SalesPlanDetail> salesPlanDetail { get; set; }
    }
    public class SalesPlanItems
    {
        public string GROUP_SKU_FLAG { get; set; }
        public string IS_CHILD_SELECTED { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string MASTER_ITEM_CODE { get; set; }
        public string PRE_ITEM_CODE { get; set; }
    }

    public class SalesPlanDetail
    {
        public string PLAN_CODE { get; set; }
        public string PLAN_DATE { get; set; }
        public string PER_DAY_QUANTITY { get; set; }
        public string PER_DAY_AMOUNT { get; set; }
        public string ITEM_CODE { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string EMPLOYEE_CODE { get; set; }
        public string DIVISION_CODE { get; set; }
        public string REMARKS { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_DATE { get; set; }
        public string LAST_MODIFIED_BY { get; set; }
        public string LAST_MODIFIED_DATE { get; set; }
        public string APPROVED_FLAG { get; set; }
        public string APPROVED_BY { get; set; }
        public string APPROVED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string FREQUENCY_JSON { get; set; }
        public string PARTY_TYPE_CODE { get; set; }
        public string AGENT_CODE { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
    }
    public class periodClass
    {
        public string YEAR { get; set; }
        public string MONTH { get; set; }
        public string WEEK { get; set; }
        public string DAY { get; set; }
        public string HALFYEAR { get; set; }
        public string BIMONTH { get; set; }
        public string TRIMONTH { get; set; }
        public string YEARCOUNT { get; set; }
        public string MONTHCOUNT { get; set; }
    }

    public class PlanSalesRefrenceModel
    {
        public string COLNAME { get; set; }
        public string TOTAL_QTY { get; set; }
        public string QTY { get; set; }
        public string AMOUNT { get; set; }
        public string ITEM_CODE { get; set; }
        public string NEPALI_MONTH { get; set; }
        public string NEPALI_YEAR { get; set; }

    }


    public class PreferenceSetupModel
    {
        public string PL_NAME { get; set; }
        public string SHOW_CUSTOMER { get; set; }
        public string SHOW_BRANCH { get; set; }
        public string SHOW_EMPLOYEE { get; set; }
        public string ITEM_GROUP_ENTRY { get; set; }
        public string SHOW_ITEM { get; set; }
        public string SHOW_DIVISION { get; set; }
        public string SHOW_PARTY_TYPE { get; set; }
        public string SHOW_AGENT { get; set; }
        public string PARTIAL_EDIT { get; set; }
        public string EDIT_VALUE { get; set; }

    }

    public class MyColumnSettings
    {
        public MyColumnSettings()
        {
            getPeriod = new List<periodClass>();
        }
        /// <summary>Title used in header.</summary>
        public string Title { get; set; }

        /// <summary>Property name in row viewmodel that the column is bound to.</summary>
        public string PropertyName { get; set; }

        public string FrequencyName { get; set; }

        /// <summary>True if field can be edited</summary>
        public bool Editable { get; set; }

        /// <summary>System type of the property being edited. Required for grid edtiable setting.</summary>
        public Type ColType { get; set; }

        /// <summary>Width to set the column</summary>
        public int Width { get; set; }
        public string MONTHINT { get; set; }

        public List<periodClass> getPeriod { get; set; }

    }

    public class MasterSalesPlan
    {
        public int? MASTER_PLAN_CODE { get; set; }
        public string MASTER_PLAN_EDESC { get; set; }
        public string MASTER_PLAN_NDESC { get; set; }
        public string START_DATE { get; set; }
        public string END_DATE { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string EMPLOYEE_CODE { get; set; }
        public string DIVISION_CODE { get; set; }
        public string REMARKS { get; set; }
        public string COMPANY_CODE { get; set; }
        public List<SalesPlanMap> salesPlanMap { get; set; }
    }

    public class SalesPlanMap
    {
        public string MASTER_PLAN_CODE { get; set; }
        public string PLAN_CODE { get; set; }
        public string PLAN_EDESC { get; set; }
        public string PARENT_PLAN_CODE { get; set; }
        public string PLAN_FLAG { get; set; }
    }

    public class sa_sales_invoice_viewmodel
    {
        public string ITEM_CODE { get; set; }
        public double? CALC_TOTAL_PRICE { get; set; }
        public double? CALC_QUANTITY { get; set; }
    }

    public class SalesPlans_CustomersEmployees
    {
        public string PLAN_CODE { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string EMPLOYEE_CODE { get; set; }
        public string EMPLOYEE_EDESC { get; set; }
        public string DIVISION_CODE { get; set; }
        public string DIVISION_EDESC { get; set; }
        public string BRANCH_CODE { get; set; }
        public string BRANCH_EDESC { get; set; }
    }

    public class Employee
    {
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }

    }
    public class EmployeeModel
    {
        public string PARENT_EMPLOYEE_CODE { get; set; }
        public string EMPLOYEE_CODE { get; set; }
        public string EMPLOYEE_EDESC { get; set; }

    }

}
