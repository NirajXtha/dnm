using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Models.ProductionManagement
{
    public class ProductionPlan
    {
        public int PLAN_NO { get; set; }
        public DateTime PLAN_DATE { get; set; }
        public string MITI { get; set; }
        public string PLAN_NAME { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string INDEX_MU_CODE { get; set; }
        public string ORDER_NO { get; set; }
        public string PLAN_BASE_ON { get; set; }
        public int? ORDER_QUANTITY { get; set; }
        public int PLAN_QUANTITY { get; set; }
        public int? PROD_ISS_QTY { get; set; }
        public string RESOURCE_CODE { get; set; }
        public string DELETED_FLAG { get; set; }


        public string BASE_FLAG { get; set; }
        public string RESOURCE_EDESC { get; set; }
    }


    public class PlanItemModel
    {
        public decimal PLAN_NO { get; set; }
        public decimal CODE { get; set; }
        public string TT { get; set; }
    }

    public class PlanUsedInInputProcessModel
    {
        public decimal PLAN_NO { get; set; }
        public string PLAN_NAME { get; set; }
        public decimal TYPE_CODE { get; set; }
    }

    // this class not in here need to changes
    public class ProductionPlanningFilterParamsModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public string FilterListSearchText { get; set; }
    }

    public class PMProductModel
    {
        public string MasterItemCode { get; set; }
        public string ItemEDesc { get; set; }
        public string PreItemCode { get; set; }
    }

    public class PMProductModelTreeModel : PMProductModel
    {
        public List<PMProductModel> Items { get; set; } = new List<PMProductModel>();
    }


    public class ProductItemModel
    {
        public string pre_item_code { get; set; }       // Parent Code
        public string item_code { get; set; }          // Item Code
        public string item_edesc { get; set; }         // Description
        public string product_code { get; set; }       // Product Code (optional / can be null)
    }
    public class ProductItemDetailModel
    {
        public string CODE { get; set; }
        public string NAME { get; set; }
        public string CATEGORY { get; set; }
        public string UNIT { get; set; }
        public string ALT_UNIT { get; set; }
        public string HS_CODE { get; set; }
        public StockLocationInfo stockLocationInfo { get; set; }
    }


    public class StockLocationInfo
    {
        public string LocationEdesc { get; set; }
        public decimal Quantity { get; set; }
    }


    public class GroupInfoModel
    {
        public string PROCESS_CODE { get; set; }
        public string LOCATION_CODE { get; set; }
        public string PROD_GROUP { get; set; }
    }

    public class ItemQtyProcessCodeModel
    {
        public string ITEM_CODE { get; set; }
        public decimal? QUANTITY { get; set; }
        public string PROCESS_CODE { get; set; }
        public int SERIAL_NO { get; set; }
    }



    public class RawMaterialItemsModel
    {
        public string ITEM_CODE { get; set; }          // 1874, 1904 etc.
        public string ITEM_EDESC { get; set; }       // "Aquamax -WIP", etc.
        public string INDEX_MU_CODE { get; set; }     // "KG", "PCS", etc.
        public string PROCESS_EDESC { get; set; }    // "RT - Aquamax 50 KG", etc.
        public string PROCESS_CODE { get; set; }     // e.g., PR0001
        public string PROCESS_TYPE_EDESC { get; set; } // "Batch Process", "Continous Process"
        public int? STEP_NO { get; set; }             // 1, 2, etc.
        public int? SERIAL_NO { get; set; }           // 1, 2, etc.
        public double? REQUIRED_QUANTITY { get; set; }  // 250, 5, 0.190234622701331, etc.
        public string CATEGORY_CODE { get; set; }    // "IP", "PG", "RM"
        public double? STOCK { get; set; }          // 0, 630599, 636.53, etc.
        public double? PLAN_QUANTITY { get; set; }  // can be NULL, so nullable decimal
    }


    public class VarianceInfoModel
    {
        public string PlanCode { get; set; }
        public int SerialNo { get; set; }
        public string FinishedItemCode { get; set; }
        public decimal FinishedQuantity { get; set; }
        public string RawItemCode { get; set; }
        public decimal? RequiredQuantity { get; set; }
        public string CategoryCode { get; set; }
        public string ProcessCode { get; set; }
        public string CompanyCode { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string DeletedFlag { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string ModifyEntry { get; set; }
    }

    public class ItemAndQty
    {
        public string ItemCode { get; set; }
        public decimal Qty { get; set; }
    }

    public class ItemAndQtyForPrepareRawQtyModel
    {
        public List<ItemAndQty> ItemWithQtyList { get; set; }
        public decimal RequestedQty { get; set; }
        public string PlanNo { get; set; }
    }


    public class SelectedFormDetails
    {
        public string RequisitionFormCodeValue { get; set; }
        public string IndendFormCodeValue { get; set; }

        public string RequisitionFromLocationValue { get; set; }
        public string RequisitionToLocationValue { get; set; }

        public string IndentFromLocationValue { get; set; }
        public string IndentToLocationValue { get; set; }
    }


   

public class RequisitionItem
    {
        public string processTypeName { get; set; }
        public string processName { get; set; }
        public string processCode { get; set; }
        public string itemCode { get; set; }
        public string itemName { get; set; }
        public string unit { get; set; }
        public double? stock { get; set; }
        public double? planQuantity { get; set; }
        public double? requiredQuantity { get; set; }
        public double? variance { get; set; }
    }

    public class GenerateRequisitionRequest
    {
        public string planNo { get; set; }
        public string baseFlag { get; set; }
        public string itemCode { get; set; }
        public string orderNo { get; set; }
        public List<RequisitionItem> items { get; set; }
    }





    //string itemCode, decimal requestQty, string planNo

    public class OrderPlanProcess : IValidatableObject
    {
        [Required]
        public string PlanNo { get; set; }           // PLAN_NO

        [Required]
        public DateTime PlanDate { get; set; }

        [Required]
        public string PlanName { get; set; }         // PLAN_NAME

        //  [Required]
        public string ItemCode { get; set; }         // ITEM_CODE

        public string OrderNo { get; set; }

        public int? OrderQuantity { get; set; }       // ORDER_QUANTITY

        [Required(ErrorMessage = "Plan quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Plan quantity must be greater than zero.")]
        public int? PlanQuantity { get; set; }
        // PLAN_QUANTITY

        [Required]
        public string PlanBaseOn { get; set; }   // PLAN_BASE_ON

        [Required]
        public string BaseFlag { get; set; }      // BASE_FLAG
        public string ResourceCode { get; set; } = "";  // RESOURCE_CODE (nullable)

        public List<BatchTransactionResult> BatchQtyList { get; set; }
        public List<OrderDetail> OrderDetailList { get; set; }

        // Custom validation
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            System.Diagnostics.Debug.WriteLine("Validate() called"); // ← Add this line

            if (BaseFlag == "I" && string.IsNullOrWhiteSpace(ItemCode))
            {
                yield return new ValidationResult("Product name is required", new[] { nameof(ItemCode) });
            }

            if (BaseFlag == "O" && string.IsNullOrWhiteSpace(OrderNo))
            {
                yield return new ValidationResult("OrderNo is required", new[] { nameof(OrderNo) });
            }
        }
    }



    public class OrderDetail
    {

        public string ORDER_NO { get; set; }


        public DateTime ORDER_DATE { get; set; }


        public DateTime? DELIVERY_DATE { get; set; }

        public string ITEM_CODE { get; set; }

        public string ITEM_EDESC { get; set; }

        public string FORM_CODE { get; set; }

        public string CUSTOMER_CODE { get; set; }

        public string CUSTOMER_EDESC { get; set; }

        public decimal QUANTITY { get; set; }

        public int? TAKEN { get; set; }

        public decimal? PLAN_QUANTITY { get; set; }
    }

    public class ProductionPreference
    {
        public string REQUISITION_FORM_CODE { get; set; }
        public string INDENT_FORM_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public DateTime? MODIFY_DATE { get; set; }
        public string MODIFY_BY { get; set; }
        public string SYN_ROWID { get; set; }

        //
        public string TABLE_NAME { get; set; }
        public string TABLE_NAME_INDENT { get; set; }
        public string MODULE_CODE_REQUISITION { get; set; }
        public string MODULE_CODE_INDENT { get; set; }

        public string REQ_FROM_LOCATION_CODE { get; set; }
        public string REQ_TO_LOCATION_CODE { get; set; }

        public string IND_FROM_LOCATION_CODE { get; set; }
        public string IND_TO_LOCATION_CODE { get; set; }
    }

    public class ProductionPipeDataModel
    {
        public string LOCATION_CODE { get; set; }
        public string LOCATION_EDESC { get; set; }       // Godown name
        public string ITEM_EDESC { get; set; }           // Product Name
        public string MU_CODE { get; set; }              // Unit
        public decimal? BALANCE_QUANTITY { get; set; }    // Stock Quantity
        public decimal? PLAN_QUANTITY { get; set; }       // Production in Pipeline

        // Optional: Uncomment if LYCM Sale is added to the query later
        // public decimal? LycmSale { get; set; }
    }


    public class FormSetupDto
    {
        public string FORM_EDESC { get; set; }  // form_edesc
        public string FORM_CODE { get; set; }   // form_code
    }

    public class ItemIndexCapacityModel
    {
        public string PROCESS_CODE { get; set; }
        public string INDEX_ITEM_CODE { get; set; }
        public decimal INDEX_CAPACITY { get; set; }
        public bool IS_REQUESTED { get; set; }
        public decimal REQUESTED_INPUT_QTY { get; set; }
        public string ITEM_CODE { get; set; }
        public string PARENT_ITEM_CODE { get; set; }
    }

    public class ResourceDataModel
    {
        public string RESOURCE_CODE { get; set; }
        public string RESOURCE_EDESC { get; set; }
    }


    public class ProductionOrderDataModel
    {
        public string ORDER_NO { get; set; }
        public DateTime ORDER_DATE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
    }


    public class BatchTransactionResult
    {
        public string REFERENCE_NO { get; set; }
        public string BATCH_NO { get; set; }
        public string ITEM_CODE { get; set; }
        public decimal QUANTITY { get; set; }
        public decimal INPUT_QTY { get; set; }

    }

}
