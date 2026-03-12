using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.Mobile
{
    public class PurchaseOrderModel:CommonRequestModel
    {
        public string reseller_code { get; set; }
        public string distributor_code { get; set; }
        public string type { get; set; }
        public string login_code { get; set; }
        public string Order_No { get; set; }
        public string form_code { get; set; }
        public string Order_Date { get; set; }
        public string Dispatch_From { get; set; }
        public string WholeSeller_Code { get; set; }
        public UpdateRequestModel LocationInfo { get; set; }
        public List<ProductModel> products { get; set; }
    }
    public class ProductModel
    {
        public string item_code { get; set; }
        public string item_edesc { get; set; }
        public string mu_code { get; set; }
        public decimal quantity { get; set; }
        public string reject_flag { get; set; }
        public decimal rate { get; set; }
        public decimal calc_rate { get; set; }
        public decimal discountRate { get; set; }
        public decimal discountPercentage { get; set; }
        public decimal discount { get; set; }
        public string billing_name { get; set; }
        public string remarks { get; set; }
        public string party_type_code { get; set; }
        public string Sync_Id { get; set; }
        public string PRIORITY_STATUS_CODE { get; set; }
        public string CITY_CODE { get; set; }
        public string SHIPPING_CONTACT { get; set; }
        public string RESELLER_SHIPPING_ADDRESS { get; set; }
        public string SALES_TYPE_CODE { get; set; }
        public string FREE_QTY { get; set; }
        public decimal? SECOND_QUANTITY { get; set; }
        public string CONVERSION_FACTOR { get; set; }
        public string SECONDARY_UNIT { get; set; }
        public string THIRD_UNIT { get; set; }
        public decimal? THIRD_QTY { get; set; }
    }

    public class chargeAccModel
    {
        public string CHARGE_CODE { get; set; }
        public string ACC_CODE { get; set; }
        public string IMPACT_ON { get; set; }
        public string NON_GL_FLAG { get; set; }
        public string APPORTION_ON { get; set; }

    }

    public class disccountModel
    {
        public decimal DISCOUNT { get; set; }
        public decimal DISCOUNT_RATE { get; set; }
        public decimal DISCOUNT_PERCENTAGE { get; set; }
    }

    public class CancelledProductModal
    {
        public string SYNC_ID { get; set; }
        public string ITEM_CODE { get; set; }
        public string MU_CODE { get; set; }
        public decimal RATE { get; set; }
        public decimal QUANTITY { get; set; }
        public string SHIPPING_CONTACT { get; set; }
        public string REMARKS { get; set; }
        public string BILLING_NAME { get; set; }
        public string PARTY_TYPE_CODE { get; set; }
        public string REJECT_FLAG { get; set; }
    }

    public class CancelPurchaseOrderModal : CommonRequestModel
    {
        public string ORDER_NO { get; set; }
        public string SERVER_GENERATED_ORDER_NO { get; set; }
        public string ORDER_DATE { get; set; }
        public string TYPE { get; set; }
        public string RESELLER_CODE { get; set; }
        public string DISTRIBUTOR_CODE { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public DateTime SAVED_DATE { get; set; }
        public string WHOLESELLER_CODE { get; set; }
        public string BILLING_NAME { get; set; }
        public string DISPATCH_FROM { get; set; }
        public string TEMP_SYNC_ID { get; set; }
        public List<CancelledProductModal> products { get; set; }
        public UpdateRequestModel LocationInfo { get; set; }
    }
    public class FormSetupModel
    {
        public string FORM_CODE { get; set; }
        public string FORM_EDESC { get; set; }
    }
    // For PO Status
    public class PurchaseOrderStatus
    {
        public string SP_CODE { get; set; }
        public DateTime? ORDER_DATE { get; set; }
        public int ORDER_NO { get; set; }
        public string SALES_ORDER_NO { get; set; }
        public string CHALAN_NO { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public string ADDRESS { get; set; }
        public string EMPLOYEE { get; set; }
        public string PRODUCT_NAME { get; set; }
        public decimal? QUANTITY { get; set; }
        public decimal? RATE { get; set; }
        public decimal? DISCOUNT_PER { get; set; }
        public decimal? DISCOUNT_AMOUNT { get; set; }
        public decimal? TAXABLE { get; set; }
        public decimal? VAT { get; set; }
        public decimal? NET_AMOUNT { get; set; }
        public string REMARKS { get; set; }
        public string STATUS { get; set; }
        public DateTime? FROM_DATE { get; set; }
        public DateTime? TO_DATE { get; set; }
        public string BRAND { get; set; }
        public string ORDER_RECEIVED_FLAG { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public decimal? ORDER_QTY { get; set; }
        public decimal? REFERENCE_QTY { get; set; }
        public string CHECKED_BY { get; set; }
        public string AUTHORISED_BY { get; set; }
        public string POSTED_BY { get; set; }
        public string RECEIVER_NAME { get; set; }
        public string RECEIVER_NUMBER { get; set; }
        public string IS_BILLED { get; set; }
        public string FORM_CODE { get; set; }
        public string ENTITY_CODE { get; set; }
        public List<PurchaseOrderPendingStatus> PENDING_STATUS { get; set; } = new List<PurchaseOrderPendingStatus>();
    }
    // PO Status Pending_status list
    public class PurchaseOrderPendingStatus
    {
        public string SOUNAPPROVED { get; set; }
        public string SOPARTIALLYAPPROVED {get; set; }
        public string SOAPPROVED {get; set; }
        public string DISPATCHPENDING {get; set; }
        public string DELIVERED { get; set; }
        public string BILLED {get; set; }
        
    }

    public class POStatusModel
    {
        public int ORDER_NO { get; set; }                // po.order_no
        public string SALES_ORDER_NO { get; set; }           // po.sales_order_no
        public string CHALAN_NO { get; set; }               // rd.voucher_no
        public string BILL_NO { get; set; }               // Bill number
        public DateTime ORDER_DATE { get; set; }            // po.created_date
        public string PARTY_NAME { get; set; }              // cs.customer_edesc
        public string SO_STATUS { get; set; }               // computed so_status
        public string STATUS { get; set; }                 // computed delivery/billing status
        public string IS_FULL_REFERENCE { get; set; } 
        public string IS_ORDER_ALLOCATED { get; set; }
        public DateTime? SO_APPROVE_DATE { get; set; }
        public DateTime? BILL_DATE { get; set; }
        public DateTime? DELIVERED_DATE { get; set; }
        public DateTime? ALLOCATION_DATE { get; set; }
    }


    public class PurchaseOrderItem
    {
        public string Sales_Order_No { get; set; }
        public string Code { get; set; }
        public string Item { get; set; }
        public decimal Order_Quantity { get; set; }
        public decimal Approved_Quantity { get; set; }
        public decimal Allocated_Quantity { get; set; }
        public decimal? Dispatch_Quantity { get; set; }
        public decimal? Delivered_Quantity { get; set; }
    }

    public class PoTimelineModel
    {
        public int Order_No { get; set; }
        public string Is_So_Created { get; set; }            // 'Y' or 'N'
        public string So_Status { get; set; }               // 'Y' or 'N'
        public string Is_Order_Allocated { get; set; }       // 'Y' or 'N'
        public string Is_Billed { get; set; }               // 'Y' or 'N'
        public string Delivered { get; set; }              // Status from ip_vehicle_track
        public string Is_Full_Reference { get; set; }        // 'Y' or 'N'
    }
    public class CustomerHierarchicalSalesReport
    {
        public string CUSTOMER_CODE { get; set; }
        public string PRE_CUSTOMER_CODE { get; set; }
        public string MASTER_CUSTOMER_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }

        public decimal SHRAWAN_QTY { get; set; }
        public decimal SHRAWAN_AMT { get; set; }
        public decimal BHADRA_QTY { get; set; }
        public decimal BHADRA_AMT { get; set; }
        public decimal ASHOJ_QTY { get; set; }
        public decimal ASHOJ_AMT { get; set; }
        public decimal KARTIK_QTY { get; set; }
        public decimal KARTIK_AMT { get; set; }
        public decimal MANGSIR_QTY { get; set; }
        public decimal MANGSIR_AMT { get; set; }
        public decimal POUSH_QTY { get; set; }
        public decimal POUSH_AMT { get; set; }
        public decimal MAGH_QTY { get; set; }
        public decimal MAGH_AMT { get; set; }
        public decimal FALGUN_QTY { get; set; }
        public decimal FALGUN_AMT { get; set; }
        public decimal CHAITRA_QTY { get; set; }
        public decimal CHAITRA_AMT { get; set; }
        public decimal BAISHAKH_QTY { get; set; }
        public decimal BAISHAKH_AMT { get; set; }
        public decimal JESTHA_QTY { get; set; }
        public decimal JESTHA_AMT { get; set; }
        public decimal ASHADH_QTY { get; set; }
        public decimal ASHADH_AMT { get; set; }

        public List<CustomerHierarchicalSalesReport> Children { get; set; } = new List<CustomerHierarchicalSalesReport>();
        public bool HasValue { get; set; } // computed recursively
    }

}
