using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Models.CustomForm
{

    #region POST DATA CHEQUE

    public class PartyTypeCodeModal
    {
        public string PARTY_TYPE_CODE { get; set; }
        public string PARTY_TYPE_EDESC { get; set; }
        public string COMPANY_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string ACC_CODE { get; set; }
        public int? CREDIT_LIMIT { get; set; }
        public int? CREDIT_DAYS { get; set; }
        public string id { get; set; }
        public string label { get; set; }
        public string ADDRESS { get; set; }
    }

    public class PDCFormSaveModal
    {
        public string RECEIPT_NO { get; set; }
        public DateTime RECEIPT_DATE { get; set; }
        public DateTime CHEQUE_DATE { get; set; }
        public DateTime? BOUNCE_DATE { get; set; }
        public DateTime? ENCASH_DATE { get; set; }
        public string CUSTOMER { get; set; }
        public string CUSTOMER_CODE_DDL { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string PARTY_TYPE_DDL { get; set; }
        public string DEALER { get; set; }
        public string PARTY_TYPE { get; set; }
        public decimal PDC_AMOUNT { get; set; }
        public string PDC_DETAILS { get; set; }
        public string PARTY_BANK_NAME { get; set; }
        public string BANK_NAME { get; set; }
        public string CHEQUE_NO { get; set; }
        public string REMARKS { get; set; }
        public decimal REMINDER_PRIOR_DAYS { get; set; }
        public string MONEY_RECEIPT_ISSUED_BY { get; set; }
        public string MONEY_RECEIPT_NO { get; set; }
        public string CREATED_BY { get; set; }
        public string BOUNCE_BY { get; set; }
        public string IN_TRANSIT_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public DateTime? IN_TRANSIT_DATE { get; set; }
        public bool CHEQUE_IN_HAND { get; set; } = false;
        public bool CHEQUE_IN_TRANSIT { get; set; } = false;
        public bool DIRECT_BOUNCE { get; set; } = false;
        public bool CHECK_RETURN { get; set; } = false;
        public DateTime? CHECK_RETURN_DATE { get; set; }
        public string SELECTED_ACCOUNT { get; set; }
        public bool ACCOUNT_CONFIRM { get; set; } = false;
        public bool IS_UPDATE { get; set; } = false;
        public string IS_ODC { get; set; }
        public string STATUS { get; set; }
        public string ACC_CODE { get; set; }
        public string MASTER_ACC_CODE { get; set; }
        public string MASTER_TRANSACTION_TYPE { get; set; }
        public string MASTER_AMOUNT { get; set; }
        public string TRANSACTION_TYPE { get; set; }
        public string VOUCHER_NO { get; set; }
        public string MAPPED_ACC_EDESC { get; set; }
    }

    public class CustomerModal
    {
        //property to map multiselect
        public string id { get; set; }
        public string label { get; set; }

        public string TYPE_CODE { get; set; }
        public string TYPE_EDESC { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string REGD_OFFICE_EADDRESS { get; set; }
        public string PARTY_TYPE_CODE { get; set; }
        public string LINK_SUB_CODE { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string MASTER_CUSTOMER_CODE { get; set; }
        public string PRE_CUSTOMER_CODE { get; set; }
        public string ACC_CODE { get; set; }
        public string ACC_EDESC { get; set; }
        public string COMPANY_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string TPIN_VAT_NO { get; set; }
        public bool HAS_BRANCH { get; set; }

    }

    public class SupplierModel
    {
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_EDESC { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string MASTER_SUPPLIER_CODE { get; set; }
        public string PRE_SUPPLIER_CODE { get; set; }
        public int? Childrens { get; set; }
        public int LEVEL { get; set; }
        public string REGD_OFFICE_EADDRESS { get; set; }
        public string TEL_MOBILE_NO1 { get; set; }
        public string PARENT_SUPPLIER_CODE { get; set; }
    }

    public class PDCFilter
    {
        public DateTime DateEnglishFrom { get; set; }
        public DateTime DateEnglishTo { get; set; }
        public DateTime DateNepaliFrom { get; set; }
        public DateTime DateNepaliTo { get; set; }
        public string PdcStatus { get; set; }
        public string Dealer { get; set; }
        public string Customer { get; set; }
        public string PDCType { get; set; }
        public string PDCAmount { get; set; }
    }

    public class PDCVoucherResponse
    {
        public string Status { get; set; }
        public string VoucherNo { get; set; }
        public string Message { get; set; }
    }
    #endregion

    #region COLUMN SETTINGS
    public class FormDetailModal
    {
        public string FORM_CODE { get; set; }
        public string FORM_EDESC { get; set; }
        public string MASTER_FORM_CODE { get; set; }
        public string PRE_FORM_CODE { get; set; }
        public string MODULE_CODE { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string COMPANY_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }

    }

    public class Erp_TableName
    {
        public int TABLE_ID { get; set; }
        public string TABLE_NAME { get; set; }
    }

    public class FormDetailEditModal
    {
        public int ROW_ID { get; set; }
        public int SERIAL_NO { get; set; }
        public int TABLE_ID { get; set; }
        public TableNameAsDDL TABLE_NAME { get; set; } = new TableNameAsDDL();
        public int COLUMN_ID { get; set; }
        public ColumnNameAsDDL COLUMN_NAME { get; set; } = new ColumnNameAsDDL();
        public int COLUMN_WIDTH { get; set; }
        public string COLUMN_HEADER { get; set; }
        public int TOP_POSITION { get; set; }
        public int LEFT_POSITION { get; set; }
        public string FORM_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public bool DISPLAY_FLAG { get; set; } = false;
        public char CDISPLAY_FLAG { get; set; }
        public bool IS_DESC_FLAG { get; set; } = false;
        public char CIS_DESC_FLAG { get; set; }
        public string MASTER_CHILD_FLAG { get; set; }

    }

    public class FormDetailSaveModal
    {
        public int SERIAL_NO { get; set; }
        public string TABLE_NAME { get; set; }
        public int COLUMN_NAME { get; set; }
        public int COLUMN_WIDTH { get; set; }
        public string COLUMN_HEADER { get; set; }
        public int TOP_POSITION { get; set; }
        public int LEFT_POSITION { get; set; }
        public string FORM_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public bool DISPLAY_FLAG { get; set; } = false;
        public bool IS_DESC_FLAG { get; set; } = false;
        public string MASTER_CHILD_FLAG { get; set; }

    }

    public class SaveModal
    {
        List<FormDetailSaveModal> models { get; set; } = new List<FormDetailSaveModal>();
    }

    public class ColumnNameAsDDL
    {
        public int COLUMN_ID { get; set; }
        public string COLUMN_NAME { get; set; }
    }

    public class TableNameAsDDL
    {
        public int TABLE_ID { get; set; }
        public string TABLE_NAME { get; set; }

    }


    public enum ERP_TABLENAME
    {
        SA_SALES_ORDER,
        SA_SALES_INVOICE,
        SA_SALES_CHALAN
    }

    #endregion

    #region CASH BANK SETUP
    public class CashBankSelectedAccountModal
    {
        public string ASSOCIATED_ACCOUNTS_ID { get; set; }
        public string ASSOCIATED_ACCOUNTS { get; set; }
    }

    public class CashBankRootDetailModal
    {
        public string CB_CODE { get; set; }
        public string CB_EDESC { get; set; }
        public string CB_NDESC { get; set; }
        public string COMPANY_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }

    }

    public class BankAccountsModel
    {
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
    }


    public class CashBankAccountDetail
    {
        public string SHORT_CUT { get; set; }
        public string IN_ENGLISH { get; set; }
        public string IN_NEPALI { get; set; }
        public string CB_CODE { get; set; }
        public string TITLE_FLAG { get; set; }
        public string ACC_TYPE_FLAG { get; set; }
        public string ACC_CODE { get; set; }
        public string ACC_EDESC { get; set; }
        public string COMPANY_CODE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public bool IS_UPDATE { get; set; } = false;
    }

    public class AccListInfo
    {
        public string ACC_CODE { get; set; }
        public string ACC_EDESC { get; set; }
        public string ACC_TYPE_FLAG { get; set; }
    }

    public class CashBankAccountDetailSaveModal
    {
        public CashBankAccountDetail CashBankDetail { get; set; }
        public List<AccListInfo> ListInfo = new List<AccListInfo>();
    }
    #endregion


    #region BANK RECONCILATION

    public class BankDetailForReconcilation
    {
        public string ACC_CODE { get; set; }
        public string id { get; set; }
        public string label { get; set; }
        public string ACC_EDESC { get; set; }
        public string TRANSACTION_TYP { get; set; }
        public string TPB_FLAG { get; set; }
        public string ACC_TYPE_FLAG { get; set; }
        public string MASTER_ACC_CODE { get; set; }
        public string PRE_ACC_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string ACC_NATURE { get; set; }

    }

    public class ReconcilationGridModel
    {
        public string TRANSACTION_NO { get; set; }

        public string FORM_CODE { get; set; }

        public string ACC_CODE { get; set; }

        public string TRANSACTION_TYPE { get; set; }

        public string RECONCILATION_FLAG { get; set; }

        public string RECONCILE_FLAG { get; set; }

        public string RECONCILE_BY { get; set; }

        public string COMPANY_CODE { get; set; }

        public decimal? RECONCILATION_AMOUNT { get; set; }

        public DateTime VOUCHER_DATE { get; set; }

        public DateTime MITI { get; set; }

        public string VOUCHER_NO { get; set; }

        public string CHEQUE_NO { get; set; }

        public string PARTICULARS { get; set; }

        public decimal? DR_AMOUNT { get; set; }

        public decimal? CR_AMOUNT { get; set; }

        public decimal? BALANCE_AMOUNT { get; set; }

        public DateTime RECONCILE_DATE { get; set; }

        public DateTime CLEARING_DATE { get; set; }


    }


    #endregion
}
