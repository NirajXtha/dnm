using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models
{
   public class DateWiseSalesReturnDetailsModel
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
        public string SALES_TYPE { get; set; }
        public string BRAND_NAME { get; set; }
        public string EMPLOYEE_EDESC { get; set; }
        public string FISCAL { get; set; }
        public string PAYMENT_MODE { get; set; }
        public string PRIORITY_CODE { get; set; }
        public string FORM_CODE { get; set; }
        public DateTime? RETURN_DATE { get; set; }
        public string MITI { get; set; }
        public string ORDER_NO { get; set; }
        public string SALES_NO { get; set; }
        public string PARTY_TYPE_CODE { get; set; }
        public string PARTY_TYPE_EDESC { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string ADDRESS { get; set; }
        public string PAN_NO { get; set; }
        public string RETURN_NO { get; set; }
        public int? DIS_PER { get; set; }
    }
    public class DateWiseSalesReturnDetailsViewModel
    {
        public DateWiseSalesReturnDetailsViewModel()
        {
            DateWiseSalesReturnModel = new List<DateWiseSalesReturnDetailsModel>();
            AggregationResult = new Dictionary<string, AggregationModel>();
        }

        public List<DateWiseSalesReturnDetailsModel> DateWiseSalesReturnModel { get; set; }

        public Dictionary<string, AggregationModel> AggregationResult { get; set; }

        public int Total { get; set; }
    }
}
