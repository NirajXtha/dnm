using System;
using System.Collections.Generic;
using System.Linq;
using NeoErp.Core.Models;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QuotationManagement.Service.Models
{
    public class FormDetailSetup
    {
        public int SERIAL_NO { get; set; }
        public string TABLE_NAME { get; set; }
        public string COLUMN_NAME { get; set; }
        public int COLUMN_WIDTH { get; set; }
        public string COLUMN_HEADER { get; set; }
        public int TOP_POSITION { get; set; }

        public int LEFT_POSITION { get; set; }

        public string DISPLAY_FLAG { get; set; }

        public string DEFA_VALUE { get; set; }

        public string IS_DESC_FLAG { get; set; }

        public string MASTER_CHILD_FLAG { get; set; }

        public string FORM_CODE { get; set; }

        public string FORM_EDESC { get; set; }

        public string FORM_TYPE { get; set; }

        public string COMPANY_CODE { get; set; }

        public string COMPANY_EDESC { get; set; }

        public string TELEPHONE { get; set; }

        public string EMAIL { get; set; }

        public string TPIN_VAT_NO { get; set; }

        public string ADDRESS { get; set; }

        public string CREATED_BY { get; set; }

        public DateTime? CREATED_DATE { get; set; }

        public string DELETED_FLAG { get; set; }

        public string PARTY_TYPE_CODE { get; set; }

        public string FILTER_VALUE { get; set; }

        public string SYN_ROWID { get; set; }

        public DateTime? MODIFY_DATE { get; set; }
        public string MODIFY_BY { get; set; }
        public string CHILD_ELEMENT_WIDTH { get; set; }
        public string REFERENCE_FLAG { get; set; }
        public string REF_TABLE_NAME { get; set; }
        public string NEGATIVE_STOCK_FLAG { get; set; }
        public string FREEZE_MASTER_REF_FLAG { get; set; }
        public string REF_FIX_QUANTITY { get; set; }
        public string REF_FIX_PRICE { get; set; }
        public string HELP_DESCRIPTION { get; set; }
        public string CHARGE_TYPE_FLAG { get; set; }
        public string VALUE_PERCENT_FLAG { get; set; }
        public decimal VALUE_PERCENT_AMOUNT { get; set; }
        public char? ON_ITEM { get; set; }
        public string MANUAL_CALC_CHARGE { get; set; }
        public string DISPLAY_RATE { get; set; }
        public string RATE_SCHEDULE_FIX_PRICE { get; set; }
        public string PRICE_CONTROL_FLAG { get; set; }

    }
    public class DraftFormModel
    {
        public int? SERIAL_NO { get; set; }
        public string FORM_CODE { get; set; }
        public string TABLE_NAME { get; set; }
        public string COLUMN_NAME { get; set; }
        public string COLUMN_VALUE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string TEMPLATE_NO { get; set; }
        public string MODIFY_BY { get; set; }
        public DateTime? MODIFY_DATE { get; set; }

    }
    public class TemplateDraftModel
    {
        public string FORM_CODE { get; set; }
        public string TEMPLATE_CODE { get; set; }
        public string TEMPLATE_EDESC { get; set; }
        public string TEMPLATE_NDESC { get; set; }
    }
    public class TemplateDraftListModel
    {
        public string FORM_CODE { get; set; }
        public string TEMPLATE_CODE { get; set; }
        public string TEMPLATE_EDESC { get; set; }
        public string TEMPLATE_NDESC { get; set; }
        public string FORM_EDESC { get; set; }
        public string MODULE_CODE { get; set; }
        public string FORM_TYPE { get; set; }
    }
    public class Form_Detail_Column
    {
        public string COLUMN_NAME { get; set; }
    }



    public class COMMON_COLUMN
    {
        public decimal? SECOND_QUANTITY { get; set; }
        public decimal? THIRD_QUANTITY { get; set; }
        public decimal? TOTAL_PRICE { get; set; }
        public string IMAGE { get; set; }
        public string CATEGORY { get; set; }
        public int ID { get; set; }
        public string INTERFACE { get; set; }
        public string TYPE { get; set; }
        public string LAMINATION { get; set; }
        public string ITEM_SIZE { get; set; }
        public string THICKNESS { get; set; }
        public string COLOR { get; set; }
        public string GRADE { get; set; }
        public int SIZE_LENGTH { get; set; }
        public int SIZE_WIDTH { get; set; }
        public decimal? MASTER_AMOUNT { get; set; }
        public DateTime? MRR_DATE { get; set; }
        public decimal? MR_NO { get; set; }
        public DateTime? CHALAN_DATE { get; set; }
        public DateTime? DUE_DATE { get; set; }
        public string ACC_CODE { get; set; }
        public string FORM_CODE { get; set; }
        public string BUDGET_FLAG { get; set; }
        public string REMARKS { get; set; }
        public string REASON { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string REGD_OFFICE_EADDRESS { get; set; }
        public string TPIN_VAT_NO { get; set; }
        public string TEL_MOBILE_NO1 { get; set; }
        public string CUSTOMER_NDESC { get; set; }
        public string EMPLOYEE_CODE { get; set; }
        public string EMPLOYEE_EDESC { get; set; }
        public string REQUISITION_NO { get; set; }
        public string MRR_NO { get; set; }
        public string CHALAN_NO { get; set; }
        public decimal? CALC_UNIT_PRICE { get; set; }
        public decimal? CALC_TOTAL_PRICE { get; set; }
        public decimal? EXCHANGE_RATE { get; set; }
        public string PRIORITY_CODE { get; set; }
        public string SHIPPING_ADDRESS { get; set; }
        public string SHIPPING_CONTACT_NO { get; set; }
        public string SALES_TYPE_CODE { get; set; }
        public string AGENT_CODE { get; set; }
        public string AGENT_EDESC { get; set; }
        public int SERIAL_NO { get; set; }
        public string DIVISION_CODE { get; set; }
        public string DIVISION_EDESC { get; set; }
        public string SUPPLIER_MRR_NO { get; set; }
        public string REFERENCE_NO { get; set; }

        public string REFERENCE_FORM_CODE { get; set; }
        public string REFERENCE_PARTY_CODE { get; set; }

        public string DOCUMENT_TYPE_CODE { get; set; }
        public string SUPPLIER_INV_NO { get; set; }
        public DateTime? SUPPLIER_INV_DATE { get; set; }
        public string VOUCHER_NO { get; set; }
        public DateTime? VOUCHER_DATE { get; set; }
        public string TRANSACTION_TYPE { get; set; }
        public decimal? CALC_QUANTITY { get; set; }
        public string ISSUE_NO { get; set; }
        public string INVOICE_NO { get; set; }
        public string SALES_NO { get; set; }
        public string DELIVERY_TERMS { get; set; }
        public DateTime ORDER_DATE { get; set; }
        public DateTime QUOTE_DATE { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string BUDGET_CODE { get; set; }
        public string SPECIFICATION { get; set; }
        public string BRAND_NAME { get; set; }

        public decimal? CREDIT_DAYS { get; set; }
        public string REQUEST_NO { get; set; }
        public string QUOTE_NO { get; set; }

        public decimal? UNIT_PRICE { get; set; }
        public decimal? RANK_VALUE { get; set; }
        public string MASTER_ACC_CODE { get; set; }
        public string ISSUE_TYPE_CODE { get; set; }
        public DateTime? INVOICE_DATE { get; set; }
        public DateTime? SALES_DATE { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_EDESC { get; set; }
        public string ISSUE_SLIP_NO { get; set; }
        public string ACTUAL_QUANTITY { get; set; }
        public string TO_BRANCH_CODE { get; set; }
        public DateTime DELIVERY_DATE { get; set; }
        public DateTime TO_DELIVERED_DATE { get; set; }
        public string ORDER_NO { get; set; }
        public DateTime? REQUISITION_DATE { get; set; }
        public string FROM_LOCATION_CODE { get; set; }
        public string FROM_LOCATION_EDESC { get; set; }
        public string FROM_BUDGET_FLAG { get; set; }
        public decimal? COMPLETED_QUANTITY { get; set; }
        public decimal? REQ_QUANTITY { get; set; }
        public string CURRENCY_CODE { get; set; }
        public DateTime? RETURN_DATE { get; set; }
        public DateTime REQUEST_DATE { get; set; }

        public string BUYERS_NAME { get; set; }
        public string BUYERS_ADDRESS { get; set; }
        public string ADDRESS { get; set; }
        public string CONTACT_PERSON { get; set; }
        public string PHONE_NO { get; set; }
        public string MANUAL_NO { get; set; }
        public decimal? AMOUNT { get; set; }
        public decimal? QUANTITY { get; set; }
        public string LOT_NO { get; set; }
        public int? TERMS_DAY { get; set; }
        public string TO_BUDGET_FLAG { get; set; }
        public string PARTICULARS { get; set; }
        public string MU_CODE { get; set; }
        public string STOCK_BLOCK_FLAG { get; set; }
        public string MASTER_TRANSACTION_TYPE { get; set; }
        public string TO_LOCATION_CODE { get; set; }
        public string TO_LOCATION_EDESC { get; set; }
        public DateTime? ISSUE_DATE { get; set; }
        public string RETURN_NO { get; set; }
        public string PRODUCT_CODE { get; set; }
        public decimal? AREA_CODE { get; set; }
        public string PARTY_TYPE_CODE { get; set; }
        public string PAYMENT_MODE { get; set; }
        public string CHEQUE_NO { get; set; }
        public string MEMBER_SHIP_CARD { get; set; }
        public string BATCH_NO { get; set; }
        public decimal? LINE_ITEM_DISCOUNT { get; set; }
        public string PARTY_TYPE_EDESC { get; set; }

        public string NepaliVOUCHER_DATE { get; set; }
        public decimal? PRODUCTION_QTY { get; set; }

        public List<CUSTOM_TRANSACTION> CUSTOM_TRANSACTION_ENTITY { get; set; }
        public List<DocumentTransaction> IMAGES_LIST { get; set; }
        public List<ChargeOnSales> CHARGE_LIST { get; set; }
        public decimal? GATE_ENTRY_NO { get; set; }

        //        public string DESPATCH THROUGH { get; set; }
        //    public string TERMES OF DELIVERY { get; set; }
        //    public string TRUCK { get; set; }
        //    public string MOBILE NO { get; set; }
        //public string DRIVER MOBILE { get; set; }
        // public string DESTINATION { get; set; }
        //public string DRIVER NAME { get; set; }
        // public string LICENANCE NO. { get; set; }
        // public string TRUCK NO. { get; set; }
        // public string TERMS OF DELIVERY { get; set; }
        //public string LICENSE NO. { get; set; }
        // public string DRIVER MOBILE { get; set; }
        public string PP_NO { get; set; }
        public string PARTY_CODE { get; set; }
        public int SUB_PROJECT_CODE { get; set; }
        public string SUB_PROJECT_NAME { get; set; }
        public DateTime? EST_DELIVERY_DATE { get; set; }
        public DateTime? EFFECTIVE_DATE { get; set; }
        //public decimal BONUS_DISCOUNT { get; set; }
        //public decimal SPECIAL_DISCOUNT { get; set; }
        //public decimal VAT { get; set; }
        //public decimal YEARLY_DISCOUNT { get; set; }
        //public decimal FLAT_DISCOUNT { get; set; }

    }
    public class GuestInfoFromMaterTransaction
    {
        public string Form_CODE { get; set; }
        public string VOUCHER_NO { get; set; }
        public DateTime? VOUCHER_DATE { get; set; }
        public decimal? VOUCHER_AMOUNT { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string CHECKED_BY { get; set; }
        public DateTime? CHECKED_DATE { get; set; }
        public string AUTHORISED_BY { get; set; }
        public DateTime? POSTED_DATE { get; set; }
        public DateTime? MODIFY_DATE { get; set; }
        public string SYN_ROWID { get; set; }
        public String REFERENCE_NO { get; set; }
        public string SESSION_ROWID { get; set; }
        public DateTime? CR_LMT1 { get; set; }
        public DateTime? CR_LMT2 { get; set; }
        public string CR_LMT3 { get; set; }
    }

    public class CompanyInfo
    {
        public string COMPANY_EDESC { get; set; }
        public string ADDRESS { get; set; }
        public string TELEPHONE { get; set; }
        public string EMAIL { get; set; }
        public string TPIN_VAT_NO { get; set; }
        public string ABBR_CODE { get; set; }
        public string FOOTER_LOGO_FILE_NAME { get; set; }
        public string COMPANY_CODE { get; set; }
    }
}
