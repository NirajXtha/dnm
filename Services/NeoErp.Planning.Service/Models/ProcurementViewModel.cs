using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Planning.Service.Models
{

    public class ProcurementPlan
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
        public string REFERENCE_FLAG { get; set; }
        public string REFERENCE_PLAN_CODE { get; set; }
        public string customerCode { get; set; }
        public string branchCode { get; set; }
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
        public string EMPLOYEE_EDESC { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public List<ProcurementPlanItems> selectedItemsList { get; set; }
        public List<ProcurementPlanDetail> salesPlanDetail { get; set; }
    }
    public class PL_BRD_PRCMT_PLAN
    {
        public int PLAN_CODE { get; set; }
        public string PLAN_EDESC { get; set; }
        public string PLAN_NDESC { get; set; }
        public decimal? SALES_AMOUNT { get; set; }
        public int TIME_FRAME_CODE { get; set; }
        public string CALENDAR_TYPE { get; set; } //DEFAULT 'LOC',
        public string START_DATE { get; set; }
        public string END_DATE { get; set; }
        public string REMARKS { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string LAST_MODIFIED_BY { get; set; }
        public DateTime LAST_MODIFIED_DATE { get; set; }
        public string APPROVED_FLAG { get; set; }// DEFAULT 'N',
        public string APPROVED_BY { get; set; }
        public DateTime APPROVED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }// DEFAULT 'N',
        public string TIME_FRAME_EDESC { get; set; }
        public string DIVISION_EDESC { get; set; }
        public string BRANCH_EDESC { get; set; }
        public string REFERENCE_FLAG { get; set; }
        public string REFERENCE_PLAN_CODE { get; set; }
        public List<ProcurementPlanItems> selectedItemsList { get; set; }
        public List<ProcurementPlanDetail> salesPlanDetail { get; set; }

    }
    public class ProcurementPlanDetail
    {
        public int PLAN_CODE { get; set; }
        public DateTime PLAN_DATE { get; set; }
        public int? PER_DAY_AMOUNT { get; set; }//DEFAULT NULL,
        public string ITEM_CODE { get; set; }
        public string DIVISION_CODE { get; set; }
        public string FREQUENCY_JSON { get; set; }
        public string REMARKS { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string LAST_MODIFIED_BY { get; set; }
        public DateTime LAST_MODIFIED_DATE { get; set; }
        public string APPROVED_FLAG { get; set; }//DEFAULT 'N',
        public string APPROVED_BY { get; set; }
        public DateTime APPROVED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }// DEFAULT 'N',


    }
    public class ProcurementPlanItems
    {
        public string GROUP_SKU_FLAG { get; set; }
        public string IS_CHILD_SELECTED { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string MASTER_ITEM_CODE { get; set; }
        public string PRE_ITEM_CODE { get; set; }
    }

    public class ProcurementPlanSetupDetailViewModal
    {
        public virtual PL_BRD_PRCMT_PLAN ProcurementPlan { get; set; }
        public virtual List<ProcurementPlanDetail> ProcurementPlanDtlList { get; set; }
        public virtual List<ProcurementPlanItems> ProcurementPlanItems { get; set; }
    }

    public class ProcurementTree
    {
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public bool hasProducts { get; set; }
        public int Level { get; set; }
        public string MasterItemCode { get; set; }
        public string PreItemCode { get; set; }
        public IEnumerable<ProcurementTree> Items { get; set; }
    }
    public class ProcurementSetupModel
    {
        public int LEVEL { get; set; }
        public string ItemName { get; set; }
        public string GroupFlag { get; set; }
        public string ItemCode { get; set; }
        public string MasterItemCode { get; set; }
        public string PreItemCode { get; set; }
        public int? Childrens { get; set; }
    }
    public class PlanProcurementRefrenceModel
    {
        public string COLNAME { get; set; }
        public string QTY { get; set; }
        public string AMOUNT { get; set; }
        public string CR_AMOUNT { get; set; }
        public string DR_AMOUNT { get; set; }

    }
    public class ProcureFromMaterialModel
    {
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string FINISHED_ITEM_CODE { get; set; }
        public string FINISHED_ITEM_EDESC { get; set; }
        public decimal? MAX_LEVEL { get; set; }
        public decimal? MIN_LEVEL { get; set; }
        public decimal? REQUIRED_QUANTITY { get; set; }
        public decimal? REQUIRED_QTY { get; set; }
        public decimal? MONTHLY_REQ_QTY { get; set; }
        public string CATEGORY_CODE { get; set; }
        public string CATEGORY_EDESC { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string INDEX_MU_CODE { get; set; }
        public decimal? PO_PENDING { get; set; }
        public decimal? STOCK { get; set; }
        public string MONTH { get; set; }
        public string MONTHINT { get; set; }
        public string SHRAWAN { get; set; }
        public string BHADRA { get; set; }
        public string ASHOJ { get; set; }
        public string KARTIK { get; set; }
        public string MANGSIR { get; set; }
        public string PUSH { get; set; }
        public string MAGH { get; set; }
        public string FALGUN { get; set; }
        public string CHAITRA { get; set; }
        public string BAISAKH { get; set; }
        public string JESTHA { get; set; }
        public string ASHAD { get; set; }
    }

    public class ProcureFromMaterialParam
    {
        public string PLAN_CODE { get; set; }
    }
}
