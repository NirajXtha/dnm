using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Planning.Service.Models
{

    public class ProductionPlan
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
        public List<ProductionPlanItems> selectedItemsList { get; set; }
        public List<ProductionPlanDetail> salesPlanDetail { get; set; }
    }
    public class PL_PRO_PLAN
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
        public List<ProductionPlanItems> selectedItemsList { get; set; }
        public List<ProductionPlanDetail> salesPlanDetail { get; set; }

    }
    public class ProductionPlanDetail
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
    public class ProductionPlanItems
    {
        public string GROUP_SKU_FLAG { get; set; }
        public string IS_CHILD_SELECTED { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string MASTER_ITEM_CODE { get; set; }
        public string PRE_ITEM_CODE { get; set; }
    }

    public class ProductionPlanSetupDetailViewModal
    {
        public virtual PL_PRO_PLAN ProductionPlan { get; set; }
        public virtual List<ProductionPlanDetail> ProductionPlanDtlList { get; set; }
        public virtual List<ProductionPlanItems> ProductionPlanItems { get; set; }
    }

    public class ProductionTree
    {
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public bool hasProducts { get; set; }
        public int Level { get; set; }
        public string MasterItemCode { get; set; }
        public string PreItemCode { get; set; }
        public IEnumerable<ProductionTree> Items { get; set; }
    }
    public class ProductionSetupModel
    {
        public int LEVEL { get; set; }
        public string ItemName { get; set; }
        public string GroupFlag { get; set; }
        public string ItemCode { get; set; }
        public string MasterItemCode { get; set; }
        public string PreItemCode { get; set; }
        public int? Childrens { get; set; }
    }
    public class PlanProductionRefrenceModel
    {
        public string COLNAME { get; set; }
        public string QTY { get; set; }
        public string AMOUNT { get; set; }
        public string CR_AMOUNT { get; set; }
        public string DR_AMOUNT { get; set; }

    }
    public class PorductionDetailsModel
    {
        public string order_no { get; set; }
        public string item_code { get; set; }
        public string customer_code { get; set; }
        public string customer_edesc { get; set; }

        public string item_edesc { get; set; }
        public decimal? qty { get; set; }

    }

}
