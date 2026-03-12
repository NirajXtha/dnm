using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models
{
   public  class SalesVatWiseSummaryModel
    {
        public string CustomerName { get; set; }
        public string VatNo { get; set; }
        public decimal? GrossAmount { get; set; } = 0;
        public decimal? NetAmount { get; set; } = 0;
        public decimal? InvoiceAmount { get; set; } = 0;
        public List<Charges> charges { get; set; }
        public string CustomerId { get; set; }
        public int? Quantity { get; set; } = 0;
        public SalesVatWiseSummaryModel()
        {
            charges = new List<Charges>();
        }

    }
    public class GridViewSalesVatSummary
    {
        public List<SalesVatWiseSummaryModel> gridSalesVatReport { get; set; }
        public int total { get; set; }
        public GridViewSalesVatSummary()
        {
            gridSalesVatReport = new List<SalesVatWiseSummaryModel>();
            AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public Dictionary<string,AggregationModel> AggregationResult { get; set; }
    }
}
