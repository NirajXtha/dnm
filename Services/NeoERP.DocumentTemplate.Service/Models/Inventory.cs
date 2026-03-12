using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Models
{
    public class Inventory
    {
        public string VOUCHER_NO { get; set; }
        public string TableName { get; set; }
        public string SERIAL_NO { get; set; }
        public string TO_LOCATION_CODE { get; set; }
        public string REMARKS { get; set; }
        public string MANUAL_NO { get; set; }
        public string MRR_DATE { get; set; }
        public string MRR_NO { get; set; }
        public decimal? COMPLETED_QUANTITY { get; set; }
        public decimal CALC_TOTAL_PRICE { get; set; } = 0;
        public decimal CALC_UNIT_PRICE { get; set; } = 0;
        public decimal CALC_QUANTITY { get; set; }
        public decimal? TOTAL_PRICE { get; set; } = 0;
        public decimal? UNIT_PRICE { get; set; } = 0;
        public decimal QUANTITY { get; set; }
        public string PRODUCTION_QTY { get; set; }
        public string MU_CODE { get; set; }
        public string ENTRY_NO { get; set; }
        public string ENTRY_DATE { get; set; }
        public string VEHICLE_NO { get; set; }
        public string BILL_DATE { get; set; }
        public string BILL_NO { get; set; }
        public string REF_NO { get; set; }
        public string RECEIVED_DATE { get; set; }
        public string PRODUCT_CATEGORY { get; set; }
        public string SAMPLE_DATE { get; set; }
        public string QC_ACTION { get; set; }
        public string ITEM_CODE { get; set; }
        public string MODIFY_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_DATE { get; set; }
        public string FORM_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string TO_BRANCH_CODE { get; set; }
        public string SYN_ROWID { get; set; }
        public string SESSION_ROWID { get; set; }
        public string DIVISION_CODE { get; set; }
        public string INVOICE_NO { get; set; }
        public string INVOICE_DATE { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_INV_NO { get; set; }
        public string SUPPLIER_INV_DATE { get; set; }
        public string SUPPLIER_BUDGET_FLAG { get; set; }
        public string BUDGET_FLAG { get; set; }
        public string DUE_DATE { get; set; }
        public string CURRENCY_CODE { get; set; }
        public string EXCHANGE_RATE { get; set; }
        public string TERMS_DAY { get; set; }
        public string TRACKING_NO { get; set; }
        public string BATCH_NO { get; set; }
        public string LOT_NO { get; set; }
        public string SUPPLIER_MRR_NO { get; set; }
        public string PP_NO { get; set; }
        public string P_TYPE { get; set; }
        public string PP_DATE { get; set; }
        public string NET_GROSS_RATE { get; set; }
        public string NET_SALES_RATE { get; set; }
        public string NET_TAXABLE_RATE { get; set; }
        public string MASTER_PP_NO { get; set; }
        public string SECOND_QUANTITY { get; set; }
        public string THIRD_QUANTITY { get; set; }
        public string RECONCILE_DATE { get; set; }
        public string RECONCILE_FLAG { get; set; }
        public string RECONCILE_BY { get; set; }
        public string PHOTO_FILE_NAME1 { get; set; }
        public string PHOTO_FILE_NAME2 { get; set; }
        public string SPECIFICATION { get; set; }
        public string SUPPLIER_MRR_DATE { get; set; }
        public string BRAND_NAME { get; set; }
        public string BRAND_ACCEPT_FLAG { get; set; }
        public string BRAND_REMARKS { get; set; }
        public string RACK_QTY { get; set; }
        public string RACK2_QTY { get; set; }
        public string GATE_ENTRY_NO { get; set; }
        public string ISSUE_NO { get; set; }
        public string ISSUE_DATE { get; set; }
        public string ISSUE_TYPE_CODE { get; set; }
        public string FROM_LOCATION_CODE { get; set; }
        public string FROM_BUDGET_FLAG { get; set; }
        public string TO_BUDGET_FLAG { get; set; }
        public string REQ_QUANTITY { get; set; }
        public string PRODUCT_CODE { get; set; }
        public string USE_PLACE { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string EMPLOYEE_CODE { get; set; }
        public string ISSUE_SLIP_NO { get; set; }
        public string REFERENCE_NO { get; set; }
        public string RETURN_NO { get; set; }
        public string RETURN_DATE { get; set; }
        public string BUDGET_CODE { get; set; }
        public string TERMS_DAYS { get; set; }
        public string REQUISITION_NO { get; set; }
        public string REQUISITION_DATE { get; set; }
        public string BUYERS_NAME { get; set; }
        public string BUYERS_ADDRESS { get; set; }
        public string ACTUAL_QUANTITY { get; set; }
        public string ACKNOWLEDGE_BY { get; set; }
        public string ACKNOWLEDGE_DATE { get; set; }
        public string OPENING_DATA_FLAG { get; set; }
        public string TO_FORM_CODE { get; set; }
        public string ORDER_NO { get; set; }
        public string ORDER_DATE { get; set; }
        public string DELIVERY_DATE { get; set; }
        public string DELIVERY_TERMS { get; set; }
        public string CANCEL_QUANTITY { get; set; }
        public string ADJUST_QUANTITY { get; set; }
        public string CANCEL_FLAG { get; set; }
        public string CANCEL_BY { get; set; }
        public string CANCEL_DATE { get; set; }
        public string ACKNOWLEDGE_FLAG { get; set; }
        public string VOUCHER_DATE { get; set; }
        public string EFFECTIVE_DATE { get; set; }
        public string PARTICULARS { get; set; }
        public string AMOUNT { get; set; }
        public string ACC_CODE { get; set; }
        public string TRANSACTION_TYPE { get; set; }
        public string PARTY_CODE { get; set; }
        public string ACKNOWLEDGE_REMARKS { get; set; }
        public string REFERENCE_FORM_CODE { get; set; }
        public string REFERENCE_PARTY_CODE { get; set; }
        public string PARTY_FLAG { get; set; }
        public string DOCUMENT_TYPE_CODE { get; set; }
        public string REQUEST_NO { get; set; }
        public string REQUEST_DATE { get; set; }
        public string DEMAND_SLIP_NO { get; set; }
        public string QUOTE_NO { get; set; }
        public string QUOTE_DATE { get; set; }
        public string ADDRESS { get; set; }
        public string CONTACT_PERSON { get; set; }
        public string PHONE_NO { get; set; }
        public string CREDIT_DAYS { get; set; }
        public string DELIVERY_DAYS { get; set; }
        public string RANK_VALUE { get; set; }
        public string APPROVED_FLAG { get; set; }
        public string APPROVED_BY { get; set; }
        public string APPROVED_DATE { get; set; }
        public string EST_DELIVERY_DATE { get; set; }
        public string PRIORITY_CODE { get; set; }
        public string TNC_CODE { get; set; }
        public string MODIFY_BY { get; set; }
        public List<REF_MODEL_DEFAULT> RefenceModel { get; set; }
        public int TotalChild { get; set; } = 0;
        public int? PLAN_NO { get; set; }
        public int SUB_PROJECT_CODE { get; set; } = 0;
        public decimal? WT_PER_LTR { get; set; }

        public decimal? M_RATE { get; set; }


    }
    public class CustomTransaction
    {
        public string VOUCHER_NO { get; set; }
        public string FIELD_NAME { get; set; }
        public string FIELD_VALUE { get; set; }
        public string FORM_CODE { get; set; }
    }
    public class BudgetTranaction
    {
        public string TRANSACTION_NO { get; set; }
        public string REFERENCE_NO { get; set; }
        public string BUDGET_FLAG { get; set; }
        public string BUDGET_CODE { get; set; }
        public string BUDGET_AMOUNT { get; set; }
        public string CURRENCY_CODE { get; set; }
        public string EXCHANGE_RATE { get; set; }
        public string CATEGORY_BUDGET_CODE { get; set; }
        public string PARTICULARS { get; set; }
        public string ITEM_SERIAL_FLAG { get; set; }
        public string VALIDATION_FLAG { get; set; }
        public string ACC_CODE { get; set; }
    }
    public class CommonFieldsForInventory
    {
        public string TableName { get; set; }
        public string FormCode { get; set; }
        public decimal ExchangeRate { get; set; }
        public string CurrencyFormat { get; set; }
        public string VoucherNumber { get; set; }
        public string NewVoucherNumber { get; set; }
        public string TempCode { get; set; }
        public decimal DrTotal { get; set; }
        public string PrimaryColumn { get; set; }
        public string PrimaryDateColumn { get; set; }
        public string VoucherDate { get; set; }
        public string ManualNumber { get; set; }
        public string Grand_Total { get; set; }
        public bool FormRef { get; set; }
        public string MODULE_CODE { get; set; }
        public string SESSION_ROWID { get; set; }
        public int SUB_PROJECT_CODE { get; set; } = 0;

    }
    public class SalesInvoiceExcel
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string PK_MID { get; set; }
        public string SRC_BID { get; set; }
        public string SRC_HID { get; set; }
        public string ISMAKE { get; set; }
        public string INVOICE_ID { get; set; }
        public string INVOICE_DATE { get; set; }
        public string ORDER_DATE { get; set; }
        public string CUST_CODE { get; set; }
        public string CUST_NAME { get; set; }
        public string CUST_ADDRESS { get; set; }
        public string CUST_PAN_VAT { get; set; }
        public string CUST_PARTY_CONTACT { get; set; }
        public string PRODUCT_CODE { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string PRODUCT_UNIT { get; set; }
        public string QUANTITY { get; set; }
        public string RATE { get; set; }
        public string PRICE { get; set; }
        public string EXCISE { get; set; }
        public string CURRENCY { get; set; }
        public string EXCHANGE_RATE { get; set; }
        public string PAYMENT_MODE { get; set; }
        public string DEF1 { get; set; }
        public string DEF2 { get; set; }
        public string DEF3 { get; set; }
        public string DEF4 { get; set; }
        public string DEF5 { get; set; }
        public string DEF6 { get; set; }
        public string DEF7 { get; set; }
        public string DEF8 { get; set; }
        public string DEF9 { get; set; }
        public string DEF10 { get; set; }
        public string DEF11 { get; set; }
        public string DEF12 { get; set; }
        public string DEF13 { get; set; }
        public string DEF14 { get; set; }
        public string DEF15 { get; set; }
        public string DEF16 { get; set; }
        public string DEF17 { get; set; }
        public string DEF18 { get; set; }
        public string DEF19 { get; set; }
        public string DEF20 { get; set; }
        public string TS { get; set; }
        public string DR { get; set; }

    }

    public class SalesChalanExcelData
    {
        public string ChalanNo { get; set; }
        public DateTime Date { get; set; }
        public DateTime OrderDate { get; set; }
        public string Dealer { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhoneNo { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Unit { get; set; }
        public int Quantity { get; set; }
        public double Rate { get; set; }
        public double Total { get; set; }
        public int Discount { get; set; }
        public double VAT { get; set; }
        public string ManualNo { get; set; }
        public string CustomerCode { get; set; }
        public string PartyTypeCode { get; set; }
        public string Address { get; set; }
        public string CADCode { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }

    }
    public class DistinctManulaNo
    {
        public string VoucherNo { get; set; }
        public string ManualNo { get; set; }
    }

    public class UniqueCustomerFromExcel
    {
        public string CustomerName { get; set; }
        public string CustomerPhoneNo { get; set; }
    }

    public class UniqueItemFromExcel
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
    }

    public class UniqueDealerFromExcel
    {
        public string Dealer { get; set; }
    }

    public class UniqueDataFromExcel
    {
        public List<UniqueCustomerFromExcel> UniqCus { get; set; } = new List<UniqueCustomerFromExcel>();
        public List<UniqueItemFromExcel> UniqItem { get; set; } = new List<UniqueItemFromExcel>();
        public List<UniqueDealerFromExcel> UniqDel { get; set; } = new List<UniqueDealerFromExcel>();
    }

    public class CommonPropForChalan
    {
        public string Form_Code { get; set; }
        public string Table_Name { get; set; }
        public string Location_Code { get; set; }
        public string Company_Code { get; set; }
        public string New_Voucher_No { get; set; }
        public string Customer_Code { get; set; }
        public string Dealer_Code { get; set; }
        public string Item_Code { get; set; }


    }

    public class DbResponse
    {
        public string NormalCode { get; set; }
        public string MasterCode { get; set; }

        public string AccountCode { get; set; }
    }



    public class GateEntryReqModel
    {
        public DateTime? FROM_DATE { get; set; }
        public DateTime? TO_DATE { get; set; }
        public string PRODUCT_FILTER { get; set; }
    }

    public class PDCReqModel
    {
        public DateTime? FROM_DATE { get; set; }
        public DateTime? TO_DATE { get; set; }
        public string PDC_TYPE { get; set; }
        public string CHEQUE_TYPE { get; set; }
    }

    public class PDCModel
    {
        public string RECEIPT_NO { get; set; }
        public DateTime? RECEIPT_DATE { get; set; }
        public DateTime? CHEQUE_DATE { get; set; }
        public DateTime? ENCASH_DATE { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string PARTY_TYPE_CODE { get; set; }
        public decimal? PDC_AMOUNT { get; set; }
        public string PDC_DETAILS { get; set; }
        public string CHEQUE_PATH { get; set; }
        public string BANK_NAME { get; set; }
        public string REMARKS { get; set; }
        public DateTime? VG_DATE { get; set; }
        public string VOUCHER_NO { get; set; }
        public DateTime? BOUNCE_DATE { get; set; }
        public string BOUNCE_VC_NO { get; set; }
        public string MANUAL_NO { get; set; }
        public string CHEQUE_NO { get; set; }
        public string BOUNCE_REASON { get; set; }
        public string DISPLAY_ORDER { get; set; }
        public string VERIFIED_BY { get; set; }
        public DateTime? VERIFIED_DATE { get; set; }
        public string AUTHORIZED_BY { get; set; }
        public DateTime? AUTHORIZED_DATE { get; set; }
        public string CHEQUE_TYPE { get; set; }
        public string INTRANSIT_FLAG { get; set; }
        public string RETURN_FLAG { get; set; }
        public string BOUNCE_FLAG { get; set; }

    }

    public class PDCPostModel
    {
        public string RECEIPT_NO { get; set; }
        public DateTime? RECEIPT_DATE { get; set; }
        public string CHEQUE_TYPE { get; set; }
        public DateTime? CHEQUE_DATE { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string PARTY_TYPE_CODE { get; set; }
        public decimal? PDC_AMOUNT { get; set; }
        public string PDC_DETAILS { get; set; }
        public string BANK_NAME { get; set; }
        public string CHEQUE_NO { get; set; }
        public string REMARKS { get; set; }
        public string CHEQUE_PATH { get; set; }
        public string MR_ISSUED_BY { get; set; }
        public decimal? PRIOR_DAYS { get; set; }
        public string MR_NO { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string ACC_CODE { get; set; }
        public string MANUAL_NO { get; set; }
        public DateTime? INTRANSIT_DATE { get; set; }
        public string INTRANSIT_FLAG { get; set; }
        public string INTRANSIT_BY { get; set; }
        public string BOUNCE_REASON { get; set; }
        public string BOUNCE_FLAG { get; set; }
        public string BOUNCE_BY { get; set; }
        public string RETURN_FLAG { get; set; }
        public string VOUCHER_NO { get; set; }
        public DateTime? ENCASH_DATE { get; set; }
        public DateTime? VG_DATE { get; set; }
        public DateTime? BOUNCE_DATE { get; set; }
        public DateTime? RETURN_DATE { get; set; }

    }

    public class PdcInTransitDTO
    {
        public string RECEIPT_NO { get; set; }
        public string INTRANSIT_FLAG { get; set; }
        public string ACC_CODE { get; set; }
    }

    public class PdcEncashDTO
    {
        public string RECEIPT_NO { get; set; }
        public string REMARKS { get; set; }
        public int DAYS { get; set; }
        public DateTime? ENCASH_DATE { get; set; }
    }
    public class PdcPaymentEncashDTO
    {
        public string PAYMENT_NO { get; set; }
        public DateTime? ENCASH_DATE { get; set; }
    }

    public class PdcBounceDTO
    {
        public string RECEIPT_NO { get; set; }
        public string BOUNCE_REASON { get; set; }
        public string BOUNCE_FLAG { get; set; }
    }

    public class PdcReturnDTO
    {
        public string RECEIPT_NO { get; set; }
        public string RETURN_FLAG { get; set; }
        public DateTime? RETURN_DATE { get; set; }
    }
    public class PdcPaymentReturnDTO
    {
        public string PAYMENT_NO { get; set; }
        public string RETURN_FLAG { get; set; }
        public DateTime? RETURN_DATE { get; set; }
    }

    public class FormSetupDTO
    {
        public string FORM_CODE { get; set; }
        public string FORM_EDESC { get; set; }
    }
    public class PreferenceDataModel
    {
        public string RECEIPT_ACC_CODE { get; set; }
        public string PAYMENT_ACC_CODE { get; set; }
        public string RECEIPT_FORM_CODE { get; set; }
        public string PAYMENT_FORM_CODE { get; set; }
    }
    public class CustomerSetupDTO
    {
        public string LINK_SUB_CODE { get; set; }
        public string ACC_CODE { get; set; }
        public string PARTY_TYPE_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
    }
    public class SupplierSetupDTO
    {
        public string LINK_SUB_CODE { get; set; }
        public string ACC_CODE { get; set; }
        public string PARTY_TYPE_CODE { get; set; }
        public string SUPPLIER_EDESC { get; set; }
    }

    public class SupplierDTO
    {
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_EDESC { get; set; }
    }


    public class PDCPaymentModel
    {
        public string PAYMENT_NO { get; set; }
        public DateTime? PAYMENT_DATE { get; set; }
        public DateTime? CHEQUE_DATE { get; set; }
        public DateTime? ENCASH_DATE { get; set; }

        public string SUPPLIER_CODE { get; set; }
        public decimal? PDC_AMOUNT { get; set; }
        public string PDC_DETAILS { get; set; }

        public string BANK_NAME { get; set; }
        public string REMARKS { get; set; }

        public string BRANCH_CODE { get; set; }
        public string COMPANY_CODE { get; set; }

        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }

        public DateTime? VG_DATE { get; set; }
        public DateTime? MODIFY_DATE { get; set; }

        public string SYN_ROWID { get; set; }
        public string MANUAL_NO { get; set; }

        public string MODIFY_BY { get; set; }

        public string BOUNCE_FLAG { get; set; }
        public DateTime? BOUNCE_DATE { get; set; }
        public string BOUNCE_VC_NO { get; set; }

        public int PRIOR_DAYS { get; set; }

        public string CHEQUE_NO { get; set; }
        public string CHEQUE_PATH { get; set; }
        public string CHEQUE_TYPE { get; set; }

        public string DELETED_FLAG { get; set; }

        public string ACC_CODE { get; set; }
        public string BANK_ACC_CODE { get; set; }

        public string RETURN_FLAG { get; set; }
        public DateTime? RETURN_DATE { get; set; }

        public string ENCASH_FLAG { get; set; }
        public string RETURN_REASON { get; set; }
        public string PARTY_TYPE_CODE { get; set; }
    }

    public class PDCPaymentDataDTO
    {
        public string PAYMENT_NO { get; set; }
        public DateTime? PAYMENT_DATE { get; set; }
        public DateTime? CHEQUE_DATE { get; set; }
        public DateTime? ENCASH_DATE { get; set; }
        public string SUPPLIER_EDESC { get; set; }
        public decimal? PDC_AMOUNT { get; set; }
        public string PDC_DETAILS { get; set; }
        public string BANK_NAME { get; set; }
        public string REMARKS { get; set; }
        public string CHEQUE_PATH { get; set; }
        public string RETURN_FLAG { get; set; }
        public string VOUCHER_NO { get; set; }
        public DateTime? VG_DATE { get; set; }

    }

    public class Employees
    {
        public string EMPLOYEE_CODE { get; set; }
        public string EMPLOYEE_EDESC { get; set; }
        public string EMPLOYEE_NDESC { get; set; }
        public string EMPLOYEE_MANUAL_CODE { get; set; }
        public string MOBILE { get; set; }
        public string PHONE { get; set; }
        public string EMAIL { get; set; }
        public string PERSONAL_EMAIL { get; set; }
        public string CUR_DEPARTMENT_CODE { get; set; }
        public string CUR_DESIGNATION_CODE { get; set; }
        public string EMPLOYEE_STATUS { get; set; }
        public DateTime? JOIN_DATE { get; set; }
        public decimal? CUR_BASIC_SALARY { get; set; }
        public string PAN_NO { get; set; }
        public string CITIZENSHIP_NO { get; set; }
        public string SSF_NO { get; set; }
        public string ACCOUNT_NO { get; set; }
        public DateTime? BIRTH_DATE { get; set; }
        public string SEX { get; set; }
        public string BLOOD_GROUP { get; set; }
        public string MARITAL_STATUS { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string DEPARTMENT_EDESC { get; set; }
        public string DESIGNATION_EDESC { get; set; }

    }

    public class EmployeeDTO
    {
        public string EMPLOYEE_CODE { get; set; }   
        public string EMPLOYEE_EDESC { get; set; }
        public string MOBILE { get; set; }
        public string SEX { get; set; }
        public string PAN_NO { get; set; }
        public string EMAIL { get; set; }
    }
}


