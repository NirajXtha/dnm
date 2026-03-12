using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Core.Models.CustomModels
{
    //public class MobileViewVoucherModel
    //{
    //    public MobileViewVoucherModel()
    //    {
    //        this.VOUCHER_DETAIL = new List<MobileViewVoucherDetail>();
    //    }

    //    public string FORM_CODE { get; set; }
    //    public string FORM_DESCRIPTION { get; set; }
    //    public string MODULE_CODE { get; set; }
    //    public IEnumerable<MobileViewVoucherDetail> VOUCHER_DETAIL { get; set; }
    //    public class MobileViewVoucherDetail {
    //        public string VOUCHER_NO { get; set; }
    //        public string VOUCHER_DATE { get; set; }
    //        public string MITI { get; set; }
    //        public decimal VOUCHER_AMOUNT { get; set; }
    //        public string CREATED_BY { get; set; }

    //    }
    //}

    public class MobileViewVoucherModel
    {
        public int SESSION_ROWID { get; set; }
        public string FORM_CODE { get; set; }
        public string FORM_DESCRIPTION { get; set; }
        public string MODULE_CODE { get; set; }
        public string VOUCHER_NO { get; set; }
        public string VOUCHER_DATE { get; set; }
        public string MITI { get; set; }
        public decimal VOUCHER_AMOUNT { get; set; }
        public string CREATED_BY { get; set; }
        public string FORM_TYPE { get; set; }
        public string REMARKS { get; set; }
        public string ACC_CODE { get; set; }
        public string LEDGER_TITLE { get; set; }
        public string BRANCH_DESCRIPTION { get; set; }
    }

    public class MobileViewVoucheDetailModel
    {
        public MobileViewVoucheDetailModel()
        {
            this.LEDGER_DETAIL = new List<MobileViewVoucheDetailModel.LedgerDetails>();
        }
        public string CREATED_BY { get; set; }
        public string VOUCHER_DATE { get; set; }
        public string CREATED_DATE { get; set; }
        public string VOUCHER_NO { get; set; }

        public IEnumerable<ItemDetails> ITEM_DETAIL { get; set; }

        public class ItemDetails
        {
            public decimal? IN_QUANTITY { get; set; }
            public decimal? OUT_QUANTITY { get; set; }
            public string UNIT { get; set; }
            public decimal? UNIT_PRICE { get; set; }
            public decimal? TOTAL_PRICE { get; set; }
            public string ITEM_CODE { get; set; }
            public string DESCRIPTION { get; set; }
            public int SERIAL_NO { get; set; }
        }

        public IEnumerable<MobileViewVoucheDetailModel.LedgerDetails> LEDGER_DETAIL { get; set; }
        public class LedgerDetails {
            public decimal DEBIT_AMOUNT { get; set; }
            public decimal CREDIT_AMOUNT { get; set; }
            public string ACC_CODE { get; set; }
            public string TRANSACTION_TYPE { get; set; }
            public string DESCRIPTION { get; set; }
            public string PARTICULARS { get; set; }
            public bool HAS_SUB_LEDGER { get; set; }
            public int SERIAL_NO { get; set; }
            public SubLedgerDetails SUB_LEDGER_DETAIL { get; set; }
            public CostCenterDetails COST_CENTER_DETAIL { get; set; }
            public CostCenterVat LEDGER_VAT_DETAIL { get; set; }
        }
        public class SubLedgerDetails
        {

            public SubLedgerDetails()
            {
                this.Data = new List<MobileViewVoucheDetailModel.SubLedgerDetails.SubLedgerData>();
            }
            public string TITLE { get; set; }
            public IEnumerable<MobileViewVoucheDetailModel.SubLedgerDetails.SubLedgerData> Data { get; set; }
            public class SubLedgerData
            {
                public int SERIAL_NO { get; set; }
                public string DESCRIPTION { get; set; }
                public string SUB_CODE { get; set; }
                public string TRANSACTION_TYPE { get; set; }
                public decimal DEBIT_AMOUNT { get; set; }
                public decimal CREDIT_AMOUNT { get; set; }
            }   
        }
        public class CostCenterDetails
        {
            public CostCenterDetails()
            {
                this.Data = new List<MobileViewVoucheDetailModel.CostCenterDetails.CostCenterData>();
            }
            public string TITLE { get; set; }
            public IEnumerable<MobileViewVoucheDetailModel.CostCenterDetails.CostCenterData> Data { get; set; }
            public class CostCenterData
            {
                public string VOUCHER_NO { get; set; }
                public string VOUCHER_DATE { get; set; }
                public string PARTICULARS { get; set; }
                public decimal? DR_AMOUNT { get; set; }
                public decimal? CR_AMOUNT { get; set; }
                public int SERIAL_NO { get; set; }
                public string MITI { get; set; }
                public string BUDGET_EDESC { get; set; }
                public string BUDGET_CODE { get; set; }
                public CostCenterCategory COST_CENTER_CATEGORY { get; set; }
                
            }

           public class CostCenterCategory
            {
                public CostCenterCategory()
                {
                    this.CATEGORY_DATA = new List<CostCenterCategoryData>();
                }
                public string TITLE { get; set; }

                public IEnumerable<CostCenterCategoryData> CATEGORY_DATA { get; set; }
            }
            
        }

        public class CostCenterVat
        {
            public string TITLE { get; set; }

            public CostCenterVat()
            {
                this.VAT_DATA = new List<MobileViewVoucheDetailModel.CostCenterVatDetails>();
            }
            public IEnumerable<MobileViewVoucheDetailModel.CostCenterVatDetails> VAT_DATA { get; set; }
        }

        public class CostCenterVatDetails
        {
            public string MANUAL_NO { get; set; }
            public string INVOICE_DATE { get; set; }
            public string CS_CODE { get; set; }
            public string DOC_TYPE { get; set; }
            public decimal? TAXABLE_AMOUNT { get; set; }
            public decimal? VAT_AMOUNT { get; set; }
            public int SERIAL_NO { get; set; }
            public string MITI { get; set; }
            public string P_TYPE { get; set; }
        }
    }

    public class MobileViewLedgerDetailModel
    {
        public MobileViewLedgerDetailModel()
        {
            this.SUBLEDGER_DETAILS = new List<SubLedgerDetails>();
        }
        public string  VOUCHER_NO { get; set; }
        public string ACC_CODE { get; set; }
        public IEnumerable<SubLedgerDetails> SUBLEDGER_DETAILS { get; set; }
        public class SubLedgerDetails {
            public string DESCRIPTION { get; set; }
            public string SUB_CODE { get; set; }
            public string TRANSACTION_TYPE { get; set; }
            public decimal DEBIT_AMOUNT { get; set; }
            public decimal CREDIT_AMOUNT { get; set; }
        }
        
    }
}
