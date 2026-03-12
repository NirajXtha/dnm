using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.Mobile
{
    public class TransactionResponseModel
    {
        public string VOUCHER_NO { get; set; }
        public string VOUCHER_DATE { get; set; }
        public string PARTICULARS { get; set; }
        public string DR_AMOUNT { get; set; }
        public string CR_AMOUNT { get; set; }
        public string TRANSACTION_TYPE { get; set; }
    }

    public class MoveTransactionResponseModel
    {
        public string CUSTOMER_EDESC { get; set; }
        public string VOUCHER_NO { get; set; }
        public string MANUAL_NO { get; set; }
        public string VOUCHER_DATE { get; set; }
        public string CREDIT_DAYS { get; set; }
        public string DUE_DAYS { get; set; }
        public string SALES_AMT { get; set; }
        public string REC_AMT { get; set; }
        public string BALANCE { get; set; }
        public string PARTICULARS { get; set; }
        public Decimal? DR_AMOUNT { get; set; }
        public decimal? CR_AMOUNT { get; set; }
        public string TRANSACTION_TYPE { get; set; }
        public string SUB_EDESC { get; set; }
        public string SUB_CODE { get; set; }
        public string CREDIT_LIMIT { get; set; }
        public decimal? OP_BAL { get; set; }
        public decimal? DAILY_DR { get; set; }
        public decimal? DAILY_CR { get; set; }
        public decimal? CL_BAL { get; set; }
    }
}
