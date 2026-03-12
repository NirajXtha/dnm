using System;
namespace NeoERP.DocumentTemplate.Service.Models
{
    public class AccountInfoDataModel
    {
        public string ACC_CODE { get; set; }
        public string ACC_ID { get; set; }
        public string ACC_EDESC { get; set; }       // Account Name
        public decimal? LIMIT { get; set; }
        public string ACC_NATURE { get; set; }
        public decimal? SHARE_VALUE { get; set; }
        public string ACC_TYPE_FLAG { get; set; }    // "Balance Sheet Account", etc.
        public string TPB_FLAG { get; set; }
        public string TRANSACTION_TYPE { get; set; } // DR or CR
        public string MOBILE_NO { get; set; }
        public string TEL_NO { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string MODIFY_BY { get; set; }
        public DateTime? MODIFY_DATE { get; set; }
        public decimal COA_BUDGET_AMT { get; set; }
        public decimal GENERIC_BL_AMT { get; set; }
        public decimal POSTED_BL_AMT { get; set; }
    }
}