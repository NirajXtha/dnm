using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.Ledger
{
  public  class LedgerDailySummaryModel
    {
        public string Miti { get; set; }
        public DateTime VoucherDate { get; set; }
        public string OpeningType { get; set; }
        public Decimal OpeningDr { get; set; }
        public Decimal OpeningCr { get; set; }
        public Decimal DrAmount { get; set; }
        public Decimal CrAmount { get; set; }
        public Decimal ClosingDr { get; set; }
        public Decimal ClosingCr { get; set; }
        public  string ClosingType { get; set; }
    }
}
