using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models
{
    public class VatRegisterModel
    {
        public string Miti { get; set; }
        public string InvoiceNo { get; set; }
        public string PartyName { get; set; }
        public string PANNo { get; set; }
        public decimal? NetSales { get; set; }
        public decimal? Discount { get; set; }
        public decimal? TaxExempSales { get; set; } = 0;
        public decimal? ZeroRateExportSales { get; set; } = 0;
        public decimal? TaxableSales { get; set; }
        public decimal? VatAmount { get; set; }
        public string MANUAL_NO { get; set; }
        public decimal? EXCISEABLE_SALES { get; set; } = 0;


    }

    public class VatRegisterViewModel
    {
        public VatRegisterViewModel()
        {
            vatregisterModel = new List<VatRegisterModel>();
            this.AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public List<VatRegisterModel> vatregisterModel { get; set; }

        public Dictionary<string, AggregationModel> AggregationResult { get; set; }

        public int total { get; set; }
    }
}
