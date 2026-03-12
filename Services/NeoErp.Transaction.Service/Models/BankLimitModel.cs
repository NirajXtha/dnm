using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Transaction.Service.Models
{
    public class BankLimitModel
    {
        public int BankCode { get; set; }
        public string BankName { get; set; }
        public int TransactionNumber { get; set; }
        public string Type { get; set; }
        public List<LoanModel> LoanList { get; set; }

        public BankLimitModel()
        {
            LoanList = new List<LoanModel>();
        }
    }
    public class LoanModel
    {
        public int Id { get; set; }
        public int Sn { get; set; }
        public string LoanCategory { get; set; }
        public decimal LimitAmount { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Remarks { get; set; }
    }
}
