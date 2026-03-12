using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.BillwisePurchaseSummary
{
    public   class CustomerwiseOrderTrackingModel
    {
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public DateTime? ORDER_DATE { get; set; }
        public string MITI { get; set; }
        public string ORDER_NO { get; set; }
        public string EMPLOYEE_CODE { get; set; }
        public string EMPLOYEE_EDESC { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string MU_CODE { get; set; }
        public double? QUANTITY { get; set; }
        public double? SECOND_QUANTITY { get; set; }
        public double? UNIT_PRICE { get; set; }
        public double? TOTAL_PRICE { get; set; }
        public DateTime? CHALAN_DATE { get; set; }
        public string CHALAN_NO { get; set; }
        public double? CHALAN_QTY { get; set; }
        public double? CHALAN_ALT_QTY { get; set; }
        public DateTime? SALES_DATE { get; set; }
        public string SALES_NO { get; set; }
        public double? SALES_QTY { get; set; }
        public double? SALES_ALT_QTY { get; set; }
        public double? SALES_VALUE { get; set; }
        public string REFERENCE_NO { get; set; }
        public string REFERENCE_SERIAL_NO { get; set; }
        public double? ORDER_QTY { get; set; }
        public double? DUE_QTY { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string MASTER_ITEM_CODE { get; set; }
        public string PRE_ITEM_CODE { get; set; }
        public string UNIT { get; set; }
    }
}
