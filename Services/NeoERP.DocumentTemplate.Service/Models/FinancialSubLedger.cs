using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Models
{
 public   class FinancialSubLedger
    {
        public string ACC_CODE { get; set; }
       
        public string TRANSACTION_TYPE { get; set; }
        public int SERIAL_NO { get; set; }
        public decimal? DR_AMOUNT { get; set; }
        public decimal? CR_AMOUNT { get; set; }
        public decimal? REMAINING_AMOUNT { get; set; }
        public decimal? BALANCE_AMOUNT { get; set; }
        public List<FinanceSubLedger> SUBLEDGER { get; set; }
        public string SUB_CODE { get; set; }
        public string SUB_EDESC { get; set; }
        public decimal? AMOUNT { get; set; }
        public string SESSION_ROWID { get; set; }
        public string PARTICULARS { get; set; }
        public string PARTY_TYPE_CODE { get; set; }
        public string PARTY_TYPE_EDESC { get; set; }
    }
}
