using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.NetPurchaseModels
{
    public class ProductwiseNetPurchaseViewModel
    {
        public DateTime EDATE { get; set; }
        public string NDATE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string INDEX_MU_CODE { get; set; }
        public double? PURCHASE_QTY { get; set; }
        public double? PURCHASE_VALUE { get; set; }
        public double? PURCHASE_RET_QTY { get; set; }
        public double? PURCHASE_RET_VALUE { get; set; }
        public double? NET_SALES_QTY { get; set; }
        public double? NET_SALES_VALUE { get; set; }

    }

    public class ProductwiseNetPurchaseModel
    {
        public ProductwiseNetPurchaseModel()
        {
            ProductwiseNetPurchase = new List<ProductwiseNetPurchaseViewModel>();
            AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public List<ProductwiseNetPurchaseViewModel> ProductwiseNetPurchase { get; set; }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
        public int Total { get; set; }
    }
}
