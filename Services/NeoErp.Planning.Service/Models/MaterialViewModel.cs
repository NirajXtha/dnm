using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Planning.Service.Models
{

    public class MaterialPlan
    {
        public string END_DATE { get; set; }
        public string IS_ITEMS_VISIBLE_ONLY { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string FINISHED_ITEM_CODE { get; set; }
        public string FINISHED_ITEM_EDESC { get; set; }
        public decimal? MATERIAL_QUANTITY { get; set; }
        public decimal? CALC_QUANTITY { get; set; }
        public decimal? REMAINING_QUANTITY { get; set; }
        public string PLAN_DATE { get; set; }
        public string IsCustomerProduct { get; set; }
        public string PLAN_CODE { get; set; }
        public string PLAN_EDESC { get; set; }
        public string PLAN_QTY { get; set; }
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
        public string REFERENCE_FLAG { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public List<MaterialPlanItems> selectedItemsList { get; set; }
        public List<MaterialPlanDetail> salesPlanDetail { get; set; }
    }
    public class PL_MATERIAL_PLAN
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
        public List<MaterialPlanItems> selectedItemsList { get; set; }
        public List<MaterialPlanDetail> salesPlanDetail { get; set; }

    }
    public class MaterialPlanDetail
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
    public class MaterialPlanItems
    {
        public string GROUP_SKU_FLAG { get; set; }
        public string IS_CHILD_SELECTED { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string MASTER_ITEM_CODE { get; set; }
        public string PRE_ITEM_CODE { get; set; }
    }

    public class MaterialPlanSetupDetailViewModal
    {
        public virtual PL_BRD_PRCMT_PLAN MaterialPlan { get; set; }
        public virtual List<MaterialPlanDetail> MaterialPlanDtlList { get; set; }
        public virtual List<MaterialPlanItems> MaterialPlanItems { get; set; }
    }

    public class MaterialTree
    {
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public bool hasProducts { get; set; }
        public int Level { get; set; }
        public string MasterItemCode { get; set; }
        public string PreItemCode { get; set; }
        public IEnumerable<MaterialTree> Items { get; set; }
    }
    public class MaterialSetupModel
    {
        public int LEVEL { get; set; }
        public string ItemName { get; set; }
        public string GroupFlag { get; set; }
        public string ItemCode { get; set; }
        public string MasterItemCode { get; set; }
        public string PreItemCode { get; set; }
        public int? Childrens { get; set; }
    }
    public class PlanMaterialRefrenceModel
    {
        public string COLNAME { get; set; }
        public string QTY { get; set; }
        public string AMOUNT { get; set; }
        public string CR_AMOUNT { get; set; }
        public string DR_AMOUNT { get; set; }

    }
    public class SaveMaterialPlanModel
    {
        public string PLAN_DATE { get; set; }
        public string START_DATE { get; set; }
        public string END_DATE { get; set; }
        public string PLAN_CODE { get; set; }
        public string MONTH_DIFF { get; set; }
        public string PLAN_EDESC { get; set; }
        public List<MaterialPlanFIModel> finishedItemList { get; set; }
        public List<MaterialPlanModel> rawItemList { get; set; }
    }
    public class SaveMaterialPlanReferenceModel
    {
        public string PLAN_DATE { get; set; }
        public string START_DATE { get; set; }
        public string END_DATE { get; set; }
        public string PLAN_CODE { get; set; }
        public string MONTH_DIFF { get; set; }
        public string PLAN_EDESC { get; set; }
        public string REFERENCE_FLAG { get; set; }
        public string REFERENCE_CODE { get; set; }
        public List<MaterialReferenceModel> rawItemList { get; set; }
    }
    public class MaterialReferenceModel
    {
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string FINISHED_ITEM_CODE { get; set; }
        public string FINISHED_ITEM_EDESC { get; set; }
        public decimal? MAX_LEVEL { get; set; }
        public decimal? MIN_LEVEL { get; set; }
        public decimal? REQUIRED_QUANTITY { get; set; }
        public string CATEGORY_CODE { get; set; }
        public string CATEGORY_EDESC { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string MONTH { get; set; }
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
    public class GetMaterialPlanModel
    {
        public List<MaterialPlanFIModel> mpList { get; set; }
        public List<MaterialPlanFI> mpfList { get; set; }
    }
    public class MaterialProcessPlanModel
    {
        public string PROCESS_EDESC { get; set; }
        public string PROCESS_CODE { get; set; }
        public string INDEX_ITEM_CODE { get; set; }
        public string INDEX_ITEM_EDESC { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string MU_CODE { get; set; }
        public decimal? QUANTITY { get; set; }
        public decimal? REQUIRED_QTY { get; set; }
        public string COMPANY_CODE { get; set; }
        public decimal? STOCK { get; set; }
        public decimal? MONTHLY_REQ_QTY { get; set; }
        public decimal? PO_PENDING { get; set; }
        public string CATEGORY_EDESC { get; set; }
        public string CATEGORY_CODE { get; set; }
        public string MONTH { get; set; }
        public string MONTHINT { get; set; }
    }
    public class MaterialPlanModel
    {
        public string ITEM_CODE { get; set; }
        public int PLAN_CODE { get; set; }
        public string PLAN_EDESC { get; set; }
        public decimal? MATERIAL_QUANTITY { get; set; }
        public DateTime? PLAN_DATE { get; set; }
        public string FINISHED_ITEM_CODE { get; set; }
        public string FINISHED_ITEM_EDESC { get; set; }
        public string ITEM_EDESC { get; set; }
        public string PURCHASE_ORDER { get; set; }
        public decimal? REMAINING_QTY { get; set; }
        public decimal? REQUIRED_QTY { get; set; }
        public string CATEGORY_EDESC { get; set; }
        public string CATEGORY_CODE { get; set; }
        public decimal? CALC_QTY { get; set; }
        public decimal? ACTUAL_REQUIRED_QUANTITY { get; set; }
        public decimal? REQUIRED_QUANTITY { get; set; }
        public decimal? PO_PENDING { get; set; }
        public decimal? STOCK { get; set; }
        public string INDEX_MU_CODE { get; set; }
        public string REFERENCE_FLAG { get; set; }
        public string REFERENCE_CODE { get; set; }
        public decimal? PLAN_QTY { get; set; }
        public List<string> finishedItemList { get; set; }
        public List<string> rawItemList { get; set; }
    }
    public class MaterialPlanFI
    {
        public List<string> FG_ITEM_CODE { get; set; }
        public int? QTY { get; set; }
    }
    public class MaterialPlanFIModel
    {
        public string FG_ITEM_CODE { get; set; }
        public int? QTY { get; set; }
    }
    public class SalesOrderModel
    {
        public string ORDER_NO { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }

    }
    public class MaterialPlanPI
    {
        public string FROM_DEPARTMENT { get; set; }
        public string TO_DEPARTMENT { get; set; }
        public List<checklist> checkList { get; set; }

    }
    public class checklist
    {
        public string ITEM_CODE { get; set; }
        public string MU_CODE { get; set; }
        public decimal QUANTITY { get; set; }
    }
}
