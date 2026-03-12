using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.BillwisePurchaseSummary
{
    public  class ProductwisePurchaseSummaryViewModel
    {
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string PRODUCT_CODE { get; set; }
        public string UNIT { get; set; }

        public double? QUANTITY { get; set; }
        public double? UNIT_PRICE { get; set; }
        public double? TOTAL_PRICE { get; set; }
        public double? EXCISE_DUTY { get; set; }
        public double? TAXABLE_TOTAL_PRICE { get; set; }
        public double? VAT_TOTAL_PRICE { get; set; }
        public double? INVOICE_TOTAL_PRICE { get; set; }
        public double? DISCOUNT { get; set; }
        public double? LANDED_IN_NRS { get; set; }
    }
    public class ProductwisePurchaseSummaryModel
    {
        public ProductwisePurchaseSummaryModel()
        {
            ProductwisePurchaseSummary = new List<ProductwisePurchaseSummaryViewModel>();
            AggregationResult = new Dictionary<string, AggregationModel>();
        }

        public List<ProductwisePurchaseSummaryViewModel> ProductwisePurchaseSummary { get; set; }

        public Dictionary<string, AggregationModel> AggregationResult { get; set; }

        public int Total { get; set; }
    }
}
