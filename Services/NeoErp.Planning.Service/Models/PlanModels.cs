using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NeoErp.Planning.Service.Models
{
    public class PlanModels
    {
        public int? PLAN_CODE { get; set; }
        [Required]
        public string PLAN_EDESC { get; set; }
        [Required]
        public DateTime START_DATE { get; set; }
        [Required]
        public DateTime END_DATE { get; set; }
        [Required]
        public string ITEM_CODE { get; set; }
        public string ITEM_PRE_CODE { get; set; }
        public string ITEM_MASTER_CODE { get; set; }
        [Required]
        public string PLAN_TYPE { get; set; }
        [Required]
        public string PLAN_FOR { get; set; }
        [Required]
        public int TIME_FRAME_CODE { get; set; }
        public string REMARKS { get; set; }
        public string TIME_FRAME_EDESC { get; set; }
        public int TIME_FRAME_VALUE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string frequency { get; set; }
        public string days { get; set; }
        public string COLOR { get; set; }
        public string STATUS { get; set; }
        public string IS_ITEMS_VISIBLE_ONLY { get; set; }
        public string TIME_FRAME { get; set; }
        public double? TARGET_VALUE { get; set; }
        public int? PLAN_DTL_CODE { get; set; }
        public string SUBPLAN_EDESC { get; set; }

    }

    public class PlanModelTree
    {
        public int? PLAN_CODE { get; set; }
        [Required]
        public string PLAN_EDESC { get; set; }
        [Required]
        public DateTime START_DATE { get; set; }
        [Required]
        public DateTime END_DATE { get; set; }
        [Required]
        public string ITEM_CODE { get; set; }
        public string PRE_ITEM_CODE { get; set; }
        public string MASTER_ITEM_CODE { get; set; }
        [Required]
        public string PLAN_TYPE { get; set; }
        [Required]
        public string PLAN_FOR { get; set; }
        [Required]
        public int TIME_FRAME_CODE { get; set; }
        public string REMARKS { get; set; }
        public string TIME_FRAME_EDESC { get; set; }
        public int TIME_FRAME_VALUE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string frequency { get; set; }
        public string days { get; set; }
        public string COLOR { get; set; }
        public string STATUS { get; set; }
        public string IS_ITEMS_VISIBLE_ONLY { get; set; }
        public string TIME_FRAME { get; set; }
        public double? TARGET_VALUE { get; set; }
        public int? PLAN_DTL_CODE { get; set; }
        public string SUBPLAN_EDESC { get; set; }
        public bool hasProducts { get; set; }
        public int LEVEL { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string IS_CHILD_SELECTED { get; set; }
        public IEnumerable<PlanProductTree> Items { get; set; }

    }

    public class GeneratePlanModels
    {
        public int PLAN_DTL_CODE { get; set; }
    }
    public class countModel
    {
        public int COUNT { get; set; }
    }

    public class PlanProductTree
    {
        public string ITEM_EDESC { get; set; }
        public string ITEM_CODE { get; set; }
        public bool hasProducts { get; set; }
        public int LEVEL { get; set; }
        public string MASTER_ITEM_CODE { get; set; }
        public string PRE_ITEM_CODE { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string IS_CHILD_SELECTED { get; set; }
        public IEnumerable<PlanProductTree> Items { get; set; }
    }

    public class LcPreferenceSetup
    {
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string ITEM_GROUP_ENTRY { get; set; }
    }
    public class AggregatePlanModel {
        public string PLAN_CODE { get; set; }
        public string PLAN_EDESC { get; set; }
        public string INITIALS { get; set; }
        public string PLAN_TYPE { get; set; }
        public string PLAN_DATE { get; set; }
        public string REF_FLAG { get; set; }
    }
    public class ConfigSetupModel {
        public string USER_NO { get; set; }
        public string PLAN_NAME { get; set; }
        public string PLAN_CODE { get; set; }
        public string CATEGORY_CODE { get; set; }
    }
    public class ProductCategory {
        public string CATEGORY_CODE { get; set; }
        public string CATEGORY_EDESC { get; set; }
    }
    public class ImportItems
    {
        public string ITEM_CODE { get; set; }
        public string PLAN_EDESC { get; set; }
        public string PLAN_NAME { get; set; }
        public decimal? RATE { get; set; }
        public decimal? SALES_QUANTITY { get; set; }
        public decimal? SALES_AMOUNT { get; set; }
        public string AMOUNT_QUANTITY { get; set; }
        public string TIME_FRAME { get; set; }
        public string CALENDAR_TYPE { get; set; }
        public DateTime? START_DATE { get; set; }
        public DateTime? END_DATE { get; set; }
        public string SALES_TYPE { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string DIVISION_CODE { get; set; }
        public string EMPLOYEE_CODE { get; set; }
        public string PARTY_TYPE { get; set; }
        public string REMARKS { get; set; }
        public string BAISAKH { get; set; }
        public string JESTHA { get; set; }
        public string ASHAD { get; set; }
        public string SHRAWAN { get; set; }
        public string BHADRA { get; set; }
        public string ASHOJ { get; set; }
        public string KARTIK { get; set; }
        public string MANGSIR { get; set; }
        public string POUSH { get; set; }
        public string MAGH { get; set; }
        public string FALGUN { get; set; }
        public string CHAITRA { get; set; }
        public string FISCAL_YEAR { get; set; }
    }
    public class FileUpload
    {
        public HttpPostedFileBase file { get; set; }
    }

    public class fiscalYearModel
    {
        public DateTime? START_DATE  { get; set; }
        public DateTime? END_DATE { get; set; }
    }
    public class NepaliMonthName
    {
        public string BAISAKH { get; set; }
        public string JESTHA { get; set; }
        public string ASHAD { get; set; }
        public string SHRAWAN { get; set; }
        public string BHADRA { get; set; }
        public string ASHOJ { get; set; }
        public string KARTIK { get; set; }
        public string MANGSIR { get; set; }
        public string POUSH { get; set; }
        public string MAGH { get; set; }
        public string FALGUN { get; set; }
        public string CHAITRA { get; set; }

    }
}
