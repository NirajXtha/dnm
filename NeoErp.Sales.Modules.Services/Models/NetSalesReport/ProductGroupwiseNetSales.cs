using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.NetSalesReport
{
    public class ProductGroupwiseNetSales
    {
        public string CUSTOMER_EDESC { get; set; }
        public string ITEM_EDESC { get; set; }
        public string PRODUCT_CODE { get; set; }
        public string INDEX_MU_CODE { get; set; }
        public string ITEM_GROUP_EDESC { get; set; }
        public double? SALES_QTY { get; set; }
        public double? SALES_VALUE { get; set; }
        public double? SALES_RET_QTY { get; set; }
        public double? SALES_RET_VALUE { get; set; }
        public double? DEBIT_VALUE { get; set; }
        public double? CREDIT_VALUE { get; set; }
        public double? FREE_QTY { get; set; }
        public double? NET_SALES_QTY { get; set; }
        public double? NET_SALES_VALUE { get; set; }
    }
}
