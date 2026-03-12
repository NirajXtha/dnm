using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.Mobile
{
    public class PurchaseOrderResponseModel
    {
        public string CODE { get; set; }
        public string ORDER_NO { get; set; }
        public string ORDER_DATE { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string MU_CODE { get; set; }
        public string QUANTITY { get; set; }
        public string UNIT_PRICE { get; set; }
        public string TOTAL_PRICE { get; set; }
        public string REMARKS { get; set; }
        public string APPROVED_FLAG { get; set; }
        public string DISPATCH_FLAG { get; set; }
        public string ACKNOWLEDGE_FLAG { get; set; }
        public string REJECT_FLAG { get; set; }
        public string SALES_RATE { get; set; }
        public string APPLY_DATE { get; set; }
    }
}
