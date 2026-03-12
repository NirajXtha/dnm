using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models
{
    public class JournalVoucherDataModel
    {
        public DateTime VOUCHER_DATE { get; set; }
        public string ACC_CODE { get; set; }
        public string ACC_EDESC { get; set; }
        public string FORM_CODE { get; set; }
        public string VOUCHER_NO { get; set; }
        public string MANUAL_NO { get; set; }
        public string IN_MU_CODE { get; set; }
        public string OUT_MU_CODE { get; set; }
        public decimal DR_AMOUNT { get; set; }
        public decimal CR_AMOUNT { get; set; }
        public int SERIAL_NO { get; set; }
        public string PARTICULARS { get; set; }
        public string REMARKS { get; set; }
        public int QUANTITY { get; set; }
        public string MITI { get; set; }
        public int IN_QUANTITY { get; set; }
        public int OUT_QUANTITY { get; set; }
        public string FORM_EDESC { get; set; }
        public string SUB_CODE { get; set; }
        public string SUB_EDESC { get; set; }
        public string TRACKING_NO { get; set; }

    }

    public class GridViewJournalVoucher
    {
        public List<JournalVoucherDataModel> gridJournalVoucherReport { get; set; }
        public int total { get; set; }
        public GridViewJournalVoucher()
        {
            gridJournalVoucherReport = new List<JournalVoucherDataModel>();
            AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
    }
}
