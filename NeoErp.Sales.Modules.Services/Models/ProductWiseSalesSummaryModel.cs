using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;

namespace NeoErp.Sales.Modules.Services.Models
{
    public class ProductWiseSalesSummaryModel
    {
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string UNIT { get; set; }
        public int? QUANTITY { get; set; }
        public int? FREE_QTY { get; set; }
        public double? TOTAL_PRICE { get; set; }
        public double? UNIT_PRICE { get; set; } = 0;
        public double? LINE_DISCOUNT { get; set; } = 0;
        public double? DISCOUNT { get; set; }
        public double? TAXABLE_TOTAL_PRICE { get; set; }
        public double? VAT_TOTAL_PRICE { get; set; }
        public double? INVOICE_TOTAL_PRICE { get; set; }
    }

    public class ProductWiseSalesSummaryViewModel
    {
        public ProductWiseSalesSummaryViewModel()
        {
            ProductWiseSalesModel = new List<ProductWiseSalesSummaryModel>();
            AggregationResult = new Dictionary<string, AggregationModel>();
        }

        public List<ProductWiseSalesSummaryModel> ProductWiseSalesModel { get; set; }

        public Dictionary<string, AggregationModel> AggregationResult { get; set; }

        public int Total { get; set; }
    }

    public class ProductSalesSummaryModel
    {
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string UNIT { get; set; }
        public int? QUANTITY { get; set; }
        public int? FREE_QTY { get; set; }
        public double? TOTAL_PRICE { get; set; }
        public double? UNIT_PRICE { get; set; } = 0;
        public double? LINE_DISCOUNT { get; set; } = 0;
        public double? DISCOUNT { get; set; }
        public double? TAXABLE_TOTAL_PRICE { get; set; }
        public double? VAT_TOTAL_PRICE { get; set; }
        public double? INVOICE_TOTAL_PRICE { get; set; }
    }
}


