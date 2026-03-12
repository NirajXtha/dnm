using System;

namespace NeoERP.DocumentTemplate.Service.Models
{
    public class FinancialVoucherDetail
    {
        public string VOUCHER_NO { get; set; }
        public string VOUCHER_DATE { get; set; }
        public string MANUAL_NO { get; set; }
        public string SERIAL_NO { get; set; }
        public string ACC_CODE { get; set; }
        public string ACC_ID { get; set; }
        public string PARTICULARS { get; set; }
        public string TRANSACTION_TYPE { get; set; }
        public decimal AMOUNT { get; set; }
        public string PAYMENT_MODE { get; set; }
        public string BUDGET_FLAG { get; set; }
        public string REMARKS { get; set; }
        public string FORM_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string CURRENCY_CODE { get; set; }
        public string EXCHANGE_RATE { get; set; }
        public string SYN_ROWID { get; set; }
        public string OMIT_FLAG { get; set; }
        public string TRACKING_NO { get; set; }
        public string SESSION_ROWID { get; set; }
        public string DIVISION_CODE { get; set; }
        public string CHEQUE_NO { get; set; }
        public string EMPLOYEE_CODE { get; set; }
        public string EFFECTIVE_DATE { get; set; }
        public string MODIFY_DATE { get; set; }
        public string MODIFY_BY { get; set; }
        public string INVOICE_NO { get; set; }
        public string REFERENCE_FLAG { get; set; }
        public string MASTER_ACC_CODE { get; set; }
        public string MASTER_TRANSACTION_TYPE { get; set; }
        public decimal MASTER_AMOUNT { get; set; }
        public string MASTER_BUDGET_FLAG { get; set; }
      
        public string CHECKED_REMARKS { get; set; }
        public string VERIFY_REMARKS { get; set; }
        public string AUTHORISED_REMARKS { get; set; }
        public string CANCEL_FLAG { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_DATE { get; set; }
        public string SUPPLIER_INV_NO { get; set; }
        public string EXP_BUDGET_DATE { get; set; }
        public string MR_ISSUED_BY { get; set; }
        public string MR_NO { get; set; }
        public string SUBMISSION_NO { get; set; }
        public string GATE_ENTRY_NO { get; set; }

    }

    public class CommonFieldsForFinanacialVoucher
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
    }

    public class FinanceSubLedger
    {
        public string SERIAL_NO { get; set; }
        public string SUB_CODE { get; set; }
        public string SUB_EDESC { get; set; }
        public string AMOUNT { get; set; }
        public string PARTICULARS { get; set; }
        public string REFRENCE { get; set; }
        public string PARTY_TYPE_CODE { get; set; }

        public string PARTY_TYPE_EDESC { get; set; }

    }

    public class PurExpSheet
    {
        public string INVOICE_NO { get; set; }
        public string MANUAL_NO { get; set; }
        public string PP_NO { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_EDESC { get; set; }
        public string CURRENCY_CODE { get; set; }
        public decimal EXCHANGE_RATE { get; set; }
        public DateTime? INVOICE_DATE { get; set; }
        public string ITEM_CODE { get; set; }
        public string REFERENCE_NO { get; set; }
        public string ITEM_EDESC { get; set; }
        public string MU_CODE { get; set; }
        public decimal QUANTITY { get; set; }
        public decimal UNIT_PRICE { get; set; }
        public decimal TOTAL_PRICE { get; set; }
        public decimal CALC_QUANTITY { get; set; }
        public decimal CALC_UNIT_PRICE { get; set; }
        public decimal CALC_TOTAL_PRICE { get; set; }
        public decimal CHARGE_AMOUNT { get; set; }
        public string FORM_CODE { get; set; }
        public string CHARGE_CODE { get; set; }
        public string ACC_CODE { get; set; }
        public string ACC_EDESC { get; set; }
       
    }
    public class IPChargeEdesc
    {
        public string CHARGE_CODE { get; set; }
        public string CHARGE_EDESC { get; set; }
        public string CHARGE_TYPE_FLAG { get; set; }
        public string VALUE_PERCENT_FLAG { get; set; }
        public decimal VALUE_PERCENT_AMOUNT { get; set; }
        public string MANUAL_CALC_CHARGE { get; set; }
        public Char? ON_ITEM { get; set; }

    }
    public class IPChargeCode
    {
        //public string CHARGE_CODE { get; set; }
        public string CHARGE_CODE { get; set; }
        //public string CHARGE_TYPE_FLAG { get; set; }
        //public string APPLY_ON { get; set; }

        //public string GL_FLAG { get; set; }

        //public string APPORTION_ON { get; set; }

        //public string IMPACT_ON { get; set; }
    }
    public class IPChargeDtls
    {
        //public string CHARGE_CODE { get; set; }
        public string CHARGE_CODE { get; set; }
        public string CHARGE_TYPE_FLAG { get; set; }
        public string APPLY_ON { get; set; }

        public string GL_FLAG { get; set; }

        public string APPORTION_ON { get; set; }

        public string IMPACT_ON { get; set; }
    }
}
