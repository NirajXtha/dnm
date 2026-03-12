using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Planning.Service.Models
{
    public class LBAPlanViewModal
    {
        public int PLAN_CODE { get; set; }
        public string PLAN_EDESC { get; set; }
        public string PLAN_NDESC { get; set; }
        public decimal? SALES_AMOUNT { get; set; }
        public int TIME_FRAME_CODE { get; set; }
        public string CALENDAR_TYPE { get; set; } //DEFAULT 'LOC',
        public DateTime START_DATE { get; set; }
        public DateTime END_DATE { get; set; }
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
    }
    public class LBAPlanViewModal_Grid
    {
      public   string PLAN_CODE { get; set; }
        public string PLAN_EDESC { get; set; }
        public string PLAN_NDESC { get; set; }
        string SALES_AMOUNT { get; set; }
        string TIME_FRAME_CODE { get; set; }
        string CALENDAR_TYPE { get; set; } //DEFAULT 'LOC',
        string START_DATE { get; set; }
        string END_DATE { get; set; }
        string REMARKS { get; set; }
        string COMPANY_CODE { get; set; }
        string BRANCH_CODE { get; set; }
        string CREATED_BY { get; set; }
        DateTime CREATED_DATE { get; set; }
        string LAST_MODIFIED_BY { get; set; }
        DateTime LAST_MODIFIED_DATE { get; set; }
        string APPROVED_FLAG { get; set; }// DEFAULT 'N',
        string APPROVED_BY { get; set; }
        DateTime APPROVED_DATE { get; set; }
        string DELETED_FLAG { get; set; }// DEFAULT 'N',
    }

    public class LBAPlanDtlViewModal
    {
        public int PLAN_CODE { get; set; }
        public DateTime PLAN_DATE { get; set; }
        public int? PER_DAY_AMOUNT { get; set; }//DEFAULT NULL,
        public string ITEM_CODE { get; set; }
        public string ACC_CODE { get; set; }
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

    public class LBA_FA_CHART_OF_ACCOUNTS_SETUP
    {
        public string ACC_CODE { get; set; }
        public string ACC_EDESC { get; set; }
        public string ACC_NDESC { get; set; }
        public string TRANSACTION_TYPE { get; set; }
        public string TPB_FLAG { get; set; }
        public string ACC_TYPE_FLAG { get; set; }
        public string MASTER_ACC_CODE { get; set; }
        public string PRE_ACC_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string SYN_ROWID { get; set; }
        public string FREEZE_FLAG { get; set; }
        public int CURRENT_BALANCE { get; set; }
        public int LIMIT { get; set; }
        public DateTime MODIFY_DATE { get; set; }
        public string ACC_NATURE { get; set; }
        public string BRANCH_CODE { get; set; }
        public int SHARE_VALUE { get; set; }
        public string MODIFY_BY { get; set; }
        public string IND_VAT_FLAG { get; set; }
        public string BANK_ACCOUNT_NO { get; set; }
        public string PRINTING_FLAG { get; set; }
        public string ACC_SNAME { get; set; }
        public string IND_TDS_FLAG { get; set; }
        public string GROUP_START_CODE { get; set; }
        public string GROUP_END_CODE { get; set; }
        public string ACC_ID { get; set; }
        public string TEL_NO { get; set; }
        public string MOBILE_NO { get; set; }
        public string EMAIL_ID { get; set; }
        public string LINK_ID { get; set; }
        public string CONTACT_PERSON { get; set; }
        public string PREFIX_TEXT { get; set; }
    }

    public class LBATree
    {
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public bool hasProducts { get; set; }
        public int Level { get; set; }
        public string MasterItemCode { get; set; }
        public string PreItemCode { get; set; }
        public IEnumerable<LBATree> Items { get; set; }
    }
    public class LBAPlanItems
    {
        public string GROUP_SKU_FLAG { get; set; }
        public string IS_CHILD_SELECTED { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string MASTER_ITEM_CODE { get; set; }
        public string PRE_ITEM_CODE { get; set; }
    }
    public class LBASetupModel
    {
        public int LEVEL { get; set; }
        public string ItemName { get; set; }
        public string GroupFlag { get; set; }
        public string ItemCode { get; set; }
        public string MasterItemCode { get; set; }
        public string PreItemCode { get; set; }
        public int? Childrens { get; set; }
    }

    public class LBAPlanSetupDetailViewModal
    {
        public virtual LBAPlanViewModal LBAPlan { get; set; }
        public virtual List<LBAPlanDtlViewModal> LBAPlanDtlList { get; set; }

        public virtual List<LBAPlanItems> LBAPlanItems { get; set; }
    }

    public class PlanLBARefrenceModel
    {
        public string COLNAME { get; set; }
        public string QTY { get; set; }
        public string AMOUNT { get; set; }
        public string CR_AMOUNT { get; set; }
        public string DR_AMOUNT { get; set; }
        public string YM { get; set; }
        public string ITEM_CODE { get; set; }

    }

  

}
