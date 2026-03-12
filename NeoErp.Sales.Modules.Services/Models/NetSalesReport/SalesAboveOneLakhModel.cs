using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.NetSalesReport
{
    public   class SalesAboveOneLakhModel
    {
        public string PAN { get; set; }
        public string NAME_OF_TAXPAYER { get; set; }
        public string TRADE_NAME_TYPE { get; set; }
        public string PURCHASE_SALES { get; set; }
        public double? EXEMPTED_AMOUNT { get; set; }
        public double? TAXABLE_AMOUNT { get; set; }
        public string REMARKS { get; set; }
    }
    public class SalesAboveOneLakhViewModel
    {
        public SalesAboveOneLakhViewModel()
        {
            SalesAboveOneLakh = new List<SalesAboveOneLakhModel>();
            AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public List<SalesAboveOneLakhModel> SalesAboveOneLakh { get; set; }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
        public int Total { get; set; }
    }
}
