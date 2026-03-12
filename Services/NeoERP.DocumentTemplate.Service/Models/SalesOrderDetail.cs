using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Models
{
    public class SalesOrderDetail
    {
        public string ORDER_NO { get; set; }
        public string VOUCHER_NO { get; set; }
        public DateTime VOUCHER_DATE { get; set; }
        public string ORDER_DATE { get; set; }
        public string MANUAL_NO { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public int SERIAL_NO { get; set; }
        public string REFERENCE_SERIAL_NO { get; set; }
        public string REFERENCE_VOUCHER_NO { get; set; }
        public string ITEM_CODE { get; set; }
        public string ATTRIBUTE_CODE { get; set; }
        public string MAIN_ITEM_CODE { get; set; }
        public string MU_CODE { get; set; }
        public decimal QUANTITY { get; set; }
        public decimal UNIT_PRICE { get; set; }
        public decimal TOTAL_PRICE { get; set; }
        public decimal PRICE { get; set; }
        public decimal? DISCOUNT_CLASS { get; set; }
        public decimal? DISCOUNT_AMOUNT { get; set; }
        public decimal? EXCISE_DUTY_AMOUNT { get; set; }
        public decimal? LUXURY_TAX_AMOUNT { get; set; }
        public decimal? VAT_AMOUNT { get; set; }
        public decimal CALC_QUANTITY { get; set; }
        public decimal CALC_UNIT_PRICE { get; set; }
        public decimal CALC_TOTAL_PRICE { get; set; }
        //public int COMPLETED_QUANTITY { get; set; }
        public string REMARKS { get; set; }
        public string FORM_CODE { get; set; }

        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }

        public string DELIVERY_DATE { get; set; }

        public string CURRENCY_CODE { get; set; }
        public decimal EXCHANGE_RATE { get; set; }
        public string TRACKING_NO { get; set; }
        public string STOCK_BLOCK_FLAG { get; set; }
        public string SESSION_ROWID { get; set; }
        public DateTime? MODIFY_DATE { get; set; }
        public string MODIFY_BY { get; set; }
        public string SYN_ROWID { get; set; }
        public string BATCH_NO { get; set; }
        public string PARTY_TYPE_CODE { get; set; }
        //public int CANCEL_QUANTITY { get; set; }
        //public int ADJUST_QUANTITY { get; set; }
        public string CANCEL_FLAG { get; set; }
        public string CANCEL_BY { get; set; }
        //public DateTime? CANCEL_DATE { get; set; }
        public string CANCEL_REMARKS { get; set; }
        public string PRIORITY_CODE { get; set; }
        public string OPENING_DATA_FLAG { get; set; }
        public string SHIPPING_ADDRESS { get; set; }
        public string SHIPPING_CONTACT_NO { get; set; }
        public string SALES_TYPE_CODE { get; set; }
        public string EMPLOYEE_CODE { get; set; }
        public string MISC_CODE { get; set; }
        public string AGENT_CODE { get; set; }
        public string DIVISION_CODE { get; set; }
        public int? PRINT_COUNT { get; set; }
        public string AREA_CODE { get; set; }
        public decimal? LINE_ITEM_DISCOUNT { get; set; }
        public string SECTOR_CODE { get; set; }
        public decimal? SECOND_QUANTITY { get; set; }
        public decimal? THIRD_QUANTITY { get; set; }

        public Dictionary<string, string> SalesOrderColumn { get; set; }
    }

    public class SalesOrderMasterTransactionDetails
    {
        public string VOUCHER_NO { get; set; }
        public string VOUCHER_AMOUNT { get; set; }
        public string FORM_CODE { get; set; }
        public string CHECKED_BY { get; set; }
        public string AUTHORIZED_BY { get; set; }
        public string POSTED_BY { get; set; }
        public string POSTED_FLAG { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public string REFERENCE_NO { get; set; }
        public string CREATED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string REFERENCE_FORM_CODE { get; set; }
        public string VOUCHER_DATE { get; set; }
        public string COINAGE_AMOUNT { get; set; }
        public string CURRENCY_CODE { get; set; }
        public string EXCHANGE_CODE { get; set; }
        public string SYN_ROWID { get; set; }
        public string PRINT_COUNT { get; set; }
        public string CANCEL_FLAG { get; set; }
        public string SESSION_ROWID { get; set; }
        public string PRINT_FLAG { get; set; }
        public string MODIFY_BY { get; set; }
        public string CHECKED_DATE { get; set; }
        public string AUTHORISED_DATE { get; set; }
        public string POSTED_DATE { get; set; }
        public string OPENING_DATA_FLAG { get; set; }
        public string MODIFY_DATE { get; set; }
        public string PRINTED_TIME { get; set; }
        public string PRINTED_BY { get; set; }
        public string DELETED_BY { get; set; }
        public string DIVISION_CODE { get; set; }
        public string CHEQUE_PRINT_COUNT { get; set; }
        public string REMARKS { get; set; }
        public string IS_SYNC_WITH_IRD { get; set; }
        public string IS_REAL_TIME { get; set; }
        public string CR_LMT1 { get; set; }
        public string CR_LMT2 { get; set; }
        public string CR_LMT3 { get; set; }
        public string CR_LMT4 { get; set; }
        public string CR_LMT5 { get; set; }
        public string CHEQUE_ISSUE_TO { get; set; }
    }

   public class CustomOrderColumn
    {
        public string Order_Mode { get; set; }
        public string Order_By { get; set; }
        public Dictionary<string,string> CustomFieldWithValue { get; set; }
        public string FieldName { get; set; }
        public string FieldValue { get; set; }

    }

   public class SaveFieldForMasterTransaction
    {
        public string GrandTotal { get; set; }
        public string NewOrderNo { get; set; }
        public string FormCode { get; set; }
        public string SaveFlag { get; set; }
        public string VoucherDate { get; set; }
        public string ManualNo { get; set; }
        public string ExchangeRate { get; set; }
        public string CurrencyFormat { get; set; }
        public string NewVoucherNo { get; set; }

    }

   public class SaveFieldForCharge
    {
        public string TableName { get; set; }
        public string NewOrderNo { get; set; }
        public string FormCode { get; set; }
        public string ExchangeRate { get; set; }
        public string ValuePercentFlag { get; set; }
        public string ChargeSerialNo { get; set; }
        public string NewVoucherNo { get; set; }

    }

   public class SalesFieldsForSavingFormData
    {
        public Dictionary<string,string> MasterFields { get; set; }
        public List<Dictionary<string,object>> ChildFields { get; set; }
        public Dictionary<string,string> CustomFields { get; set; }
        public Dictionary<string,string> ChargeFields { get; set; }
        public Dictionary<string,string> InvItemChargeFields { get; set; }
        public Dictionary<string,string> ShippingFields { get; set; }

    }
}
