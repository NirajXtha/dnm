using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models
{
    public  class VatSalesRegisterNewModel
    {
        public string MITI { get; set; }
        public string INVOICE_NO  { get; set; }
        public string PARTY_NAME { get; set; }
        public string VAT_NO { get; set; }
        public decimal? GROSS_SALES { get; set; }
        public decimal? TAXABLE_SALES { get; set; }
        public decimal? VAT { get; set; }
        public decimal? TOTAL_SALES { get; set; }
        public decimal? TAX_EXEMPTED_SALES { get; set; }
        public decimal? NET_SALES { get; set; }

    }
    public class VatSalesRegisterNewViewModel
    {
        public VatSalesRegisterNewViewModel()
        {
            VatSalesRegisterNew = new List<VatSalesRegisterNewModel>();
            AggregationResult = new Dictionary<string, AggregationModel>();
        }

        public List<VatSalesRegisterNewModel> VatSalesRegisterNew { get; set; }

        public Dictionary<string, AggregationModel> AggregationResult { get; set; }

        public int Total { get; set; }
    }
}
