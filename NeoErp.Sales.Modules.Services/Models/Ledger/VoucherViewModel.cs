using NeoErp.Sales.Modules.Services.Models.Voucher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.Ledger
{
   public class VoucherViewModel
    {
        public DateTime voucher_date { get; set; }
        public string PARTICULARS { get; set; }
        public decimal? dr_amount { get; set; }
        public decimal? cr_amount { get; set; }
    }

    public class gridVoucherModel
    {
        public List<VoucherDetailModel> data { get; set; }
        public int total { get; set; }
        public gridVoucherModel()
        {
            data = new List<VoucherDetailModel>();
        }
    }

    public class gridSubVoucherModel
    {
        public List<VoucherDetailModel> data { get; set; }
        public int total { get; set; }
        public gridSubVoucherModel()
        {
            data = new List<VoucherDetailModel>();
        }
    }
}
