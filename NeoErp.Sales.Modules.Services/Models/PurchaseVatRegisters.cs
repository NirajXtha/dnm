using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models
{
    public class PurchaseVatRegisters
    {
        public string Miti { get; set; }
        public string InvoiceNo { get; set; }
        public string PartyName { get; set; }
        public string VatNo { get; set; }
        public decimal? GrossPurchase { get; set; }
        public decimal? TaxablePurchase { get; set; }
        public decimal? VatAmount { get; set; }
        public decimal? TotalPurchase { get; set; }
        public string FormCode { get; set; }
        public string Ptype { get; set; }
        public string ManualNo { get; set; }
        //public DateTime InvoiceDate { get; set; }
        public string InvoiceDate { get; set; }
    }

    public class PurchaseVatRegistersView
    {
        //public DateTime InvoiceDate { get; set; }
        public string InvoiceDate { get; set; }
        public string Miti { get; set; }
        public string InvoiceNo { get; set; }
        public string ManualNo { get; set; }
        public string PartyName { get; set; }
        public string VatNo { get; set; }
        public decimal? PurchaseTaxExemp { get; set; } = 0;
        public decimal? localTaxAmount { get; set; } = 0;
        public decimal? localVatAmount { get; set; } = 0;
        public decimal? ImportTaxAmount { get; set; } = 0;
        public decimal? ImportVatAmount { get; set; } = 0;
        public decimal? AdminTaxAmount { get; set; } = 0;
        public decimal? AdminVatAmount { get; set; } = 0;
        public decimal? CaptialTaxAmount { get; set; } = 0;
        public decimal? CapitalVatAmount { get; set; } = 0;
        public Decimal? TotalPurchaseLocalAmount { get; set; } = 0;
        public decimal? TotalPurchaseImport { get; set; } = 0;
        public decimal? TotalVatLocalAmount { get; set; } = 0;
        public decimal? TotalVatImportAmount { get; set; } = 0;
    }
    public class PurchaseVatRegistersViewModel
    {
        public IList<PurchaseVatRegistersView> VatRegisters { get; set; }
        public int total { get; set; }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
        public PurchaseVatRegistersViewModel()
        {
            VatRegisters = new List<PurchaseVatRegistersView>();
            this.AggregationResult = new Dictionary<string, AggregationModel>();
        }
    }
}
