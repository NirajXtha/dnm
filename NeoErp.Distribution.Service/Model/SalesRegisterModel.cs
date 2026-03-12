using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model
{
    public class SalesRegisterModel
    {
        public DateTime? SalesDate { get; set; }
        public string Miti { get; set; }
        public string InvoiceNumber { get; set; }
        public string CustomerName { get; set; }
        public string ItemName { get; set; }
        public string LocationName { get; set; }
        public string ManualNo { get; set; }
        public string REMARKS { get; set; }
        public string Dealer { get; set; }
        public string PartyType { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingContactNo { get; set; }
        public string Unit { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalPrice { get; set; }
        public string AREA_EDESC { get; set; }
    }

    public class SalesRegisterViewModel
    {
        public List<SalesRegisterModel> SalesRegisters { get; set; }
        public int total { get; set; }
        public SalesRegisterViewModel()
        {
            SalesRegisters = new List<SalesRegisterModel>();
            AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
    }
}
