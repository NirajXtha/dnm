using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model
{
   public class PurchaseOrderModel
    {
        public PurchaseOrderModel()
        {
            ITEMS = new List<PurchaseOrderModel>();
        }
        public decimal ORDER_NO { get; set; }
        public string ORDER_ENTITY { get; set; }
        public string RESELLER_NAME { get; set; }
        public DateTime? ORDER_DATE { get; set; }
        public string MITI { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string PARTY_TYPE_CODE { get; set; }
        public string PARTY_TYPE_EDESC { get; set; }
        public string PO_PARTY_TYPE { get; set; }
        public string SALES_TYPE_CODE { get; set; }
        public string PRIORITY_CODE { get; set; }
        public string EMPLOYEE_CODE { get; set; }
        public string ITEM_CODE { get; set; }
        public string MU_CODE { get; set; }
        public decimal? QUANTITY { get; set; }
        public decimal? UNIT_PRICE { get; set; } = 0;
        public decimal? TOTAL_PRICE { get; set; }
        public string REMARKS { get; set; }
        public string CREATED_BY { get; set; }
        public string TYPE { get; set; }
        public string CITY_CODE { get; set; }
        public string SHIPPING_CONTACT { get; set; }
        public string SIGNATURE { get; set; }

        public string UNIT_EDITED_FLAG { get; set; } = "N";
        public DateTime? CREATED_DATE { get; set; }
        public DateTime? MODIFY_DATE { get; set; }
        public string BILLING_NAME { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string ITEM_EDESC { get; set; }
        public string EMPLOYEE_EDESC { get; set; }
        public string DELETED_FLAG { get; set; }
        public string APPROVED_FLAG { get; set; }
        public bool? APPROVED_FLAGBOOL { get; set; }
        public string DISPATCH_FLAG { get; set; }
        public  bool? DISPATCH_FLAGBOOL { get; set; }
        public string ACKNOWLEDGE_FLAG { get; set; }
        public bool? ACKNOWLEDGE_FLAGBOOL { get; set; }
        public string REJECT_FLAG { get; set; }
        public bool? REJECT_FLAGBOOL { get; set; }
        public decimal? APPROVEQTY { get; set; }
        public bool CREDITLIMIT { get; set; }
        public string CREDITLIMITREMARKS { get; set; }
        public string SO_CREDIT_LIMIT_FLAG { get; set; }
        public string SO_CREDIT_DAYS_FLAG { get; set; }

        public decimal? GrantTotalAmount { get; set; } = 0;
        public bool? ISEDITED { get; set; } = false;

        public decimal? ApprovedAmount { get; set; } = 0;
        public decimal? TOTAL_APPROVE_AMT { get; set; } = 0;

        public decimal? credit_limit { get; set; }

        public string CreditLimitRemarks { get; set; }

        public decimal? balance { get; set; }
        public string PO_REMARKS { get; set; }
        public string PO_CUSTOM_RATE { get; set; }
        public string PO_CONVERSION_FACTOR { get; set; }
        public string PO_CONVERSION_UNIT { get; set; }
        public string CONVERSION_MU_CODE { get; set; }
        public string PO_BILLING_NAME { get; set; }
        public decimal? CONVERSION_FACTOR { get; set; }
        public decimal? CONVERSION_QUANTITY { get; set; } = 0;
        public string FORM_CODE { get; set; }
        public decimal? GRAND_APPROVE_QUENTITY { get; set; }
        public string COMPANY_CODE { get; set; }
        public string COMPANY_EDESC { get; set; }
        public string BRANCH_CODE { get; set; }
        public string IsDeleted { get; set; } = "update";

        public string GROUP_EDESC { get; set; }

        public decimal NEW_RATE { get; set; }
        public decimal LAST_RATE { get; set; } = 0;
        public decimal NEW_TOTAL_AMOUNT { get; set; }
        public decimal? DISCOUNT { get; set; }
        public decimal? DISCOUNT_RATE { get; set; }
        public decimal? DISCOUNT_PERCENTAGE { get; set; }
        public string HAS_FREE_QUANTITY { get; set; }
        public decimal? FREE_QTY { get; set; }

        public List<PurchaseOrderModel> ITEMS { get; set; }

    }
    public class test
    {
        public List<PurchaseOrderModel> model { get; set; }
        
    }

    public class DistributorSearch
    {
        public string companyCode { get; set; }
        public string BranchCode { get; set; }
        public string AsOnDate { get; set; }

    }
    public class SalesRegisterProductModel
    {
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string UNIT { get; set; }
        public string MU_EDESC { get; set; }
        public string CONVERSION_FACTOR { get; set; }
        public string CONVERSION_UNIT { get; set; }
        public string SALES_RATE { get; set; }
        public string PO_DISPLAY_DIST_ITEM { get; set; }
        public string SQL_NN_CONVERSION_UNIT_FACTOR { get; set; }
        public string BRAND_NAME { get; set; }
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
    public class CreditDaysBalanceModel
    {
        public List<CreditDaysBalance> CreditDaysBalance { get; set; }
        public decimal TOTALPAIDAMT { get; set; }
        public int CREDIT_DAYS { get; set; }
        public int DUE_DAYS { get; set; }
        public string PENDING_STATUS { get; set; }
        public string CREDIT_LIMIT { get; set; }
        public string FREE_LIMIT { get; set; }
        public double SALES_CREDIT_AMOUNT { get; set; }
       public CreditDaysBalanceModel()
        {
            CreditDaysBalance = new List<Model.CreditDaysBalance>();
        }
    }

    public class PendingBillData
    {
        public string VOUCHER_NO { get; set; }
        public string VOUCHER_DATE { get; set; }
        public string CREDIT_LIMIT { get; set; }
        public string CREDIT_DAYS { get; set; }
        public decimal? DUE_DAYS { get; set; }
        public decimal? SALES_AMT { get; set; }
        public decimal? REC_AMT { get; set; }
        public decimal? BALANCE { get; set; }
    }

    public class PendingStatusModal
    {
        public string SUB_CODE { get; set; }
        public string SUB_EDESC { get; set; }
        public string VOUCHER_NO { get; set; }
        public decimal OPL_BAL { get; set; } 
        public decimal CL_BAL { get; set; }
    }

    public class CreditDaysBalance
    {
        public string VOUCHER_NO { get; set; }
        public DateTime? VOUCHER_DATE { get; set; }
        public int DUEDAYS { get; set; }
        public decimal SALES_AMT { get; set; }
        public decimal SALES_CREDIT_AMOUNT { get; set; }
        public decimal BAL_AMT { get; set; }
        public decimal CREDIT_LIMIT { get; set; }
        public decimal CREDIT_DAYS { get; set; }
    }

    public class DistPurchaseOrder
    {
        
        public string ITEM_CODE { get; set; }
        public decimal ORDER_NO { get; set; }
        public string Remarks { get; set; }
        public int MAXID { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public List<selectedPOArrayy> selectedPOArray { get; set; }
    }

    public class selectedPOArrayy
    {
        public decimal? cQuantity { get; set; }
        public string cUnit { get; set; }
        public decimal? reqQuantity { get; set; }
        public string reqUnit { get; set; }
        public decimal? totalPrice { get; set; }
        public decimal unitPrice { get; set; }
        public List<string> selectedItems { get; set; }
        public selectedPOArrayy() {

            selectedItems = new List<string>();
        }
    }


    public class ChargeTransactionModel
    {
        public string TRANSACTION_NO { get; set; }
        public string TABLE_NAME { get; set; }
        public string REFERENCE_NO { get; set; }
        public string ITEM_CODE { get; set; }
        public int? SERIAL_NO { get; set; }
        public char? APPLY_ON { get; set; }
        public char? CALCULATE_BY { get; set; }
        public string CHARGE_CODE { get; set; }
        public char CHARGE_TYPE_FLAG { get; set; }
        public decimal? CHARGE_AMOUNT { get; set; }
        public string BUDGET_CODE { get; set; }
        public char? GL_FLAG { get; set; }
        public string ACC_CODE { get; set; }
        public string SUB_CODE { get; set; }
        public decimal? VALUE_PERCENT_AMOUNT { get; set; }
        public char? APPORTION_FLAG { get; set; }
        public string FORM_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public char? DELETED_FLAG { get; set; }
        public char? IMPACT_ON { get; set; }
        public string CURRENCY_CODE { get; set; }
        public decimal? EXCHANGE_RATE { get; set; }
        public string SYN_ROWID { get; set; }
        public string SESSION_ROWID { get; set; }
        public char? CHARGE_FLAG { get; set; }
        public DateTime? MODIFY_DATE { get; set; }
        public char? NON_GL_FLAG { get; set; }
        public string MODIFY_BY { get; set; }
        public string VOUCHER_NO { get; set; }
        public decimal? MANUAL_CALC_VALUE { get; set; }
        public int? REF_SERIAL_NO { get; set; }
        public char? OPENING_DATA_FLAG { get; set; }
        public string DIVISION_CODE { get; set; }
    }

}
