using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.BillwisePurchaseSummary
{
    public class BillwisePurchaseSummaryViewModel
    {
        public long INVOICE_ID { get; set; }
        public DateTime? INVOICE_DATE { get; set; }
        public DateTime? RETURN_DATE { get; set; }
        public string MITI { get; set; }
        public string ORDER_NO { get; set; }
        public string INVOICE_NO { get; set; }
        public string MANUAL_NO { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_EDESC { get; set; }
        public string ADDRESS { get; set; }
        public string TPIN_VAT_NO { get; set; }
        public double? QUANTITY { get; set; }
        public double? TOTAL_PRICE { get; set; }
        public double? EXCISE_DUTY { get; set; }
        public double? DISCOUNT { get; set; }
        public double? TAXABLE_TOTAL_PRICE { get; set; }
        public double? VAT_TOTAL_PRICE { get; set; }
        public double? INVOICE_TOTAL_PRICE { get; set; }
        public double? TOTAL_IN_NRS { get; set; }
        public double? LANDED_IN_NRS { get; set; }
        public string VEHICLE_NO { get; set; }
        public string DESTINATION { get; set; }
        public double? EXCHANGE_RATE { get; set; }
        public string CURRENCY_CODE { get; set; }
        public string SUPPLIER_INV_NO { get; set; }
        public DateTime? SUPPLIER_INV_DATE { get; set; }
        public DateTime? DUE_DATE { get; set; }
        public string PP_NO { get; set; }
        public string FISCAL { get; set; }
        public string RETURN_NO { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string PRODUCT_CODE { get; set; }
        public string UNIT { get; set; }
        public double? UNIT_PRICE { get; set; }
        public string HS_CODE { get; set; }
        public string BRAND_NAME { get; set; }
        //
        public double? D1 { get; set; }
        public double? D2 { get; set; }
        public double? D3 { get; set; }
        public double? D4 { get; set; }
        public double? D5 { get; set; }
        public double? D6 { get; set; }
        public double? D7 { get; set; }
        public double? D8 { get; set; }
        public double? TotalD { get; set; }
    }
    public class BillwisePurchaseSummaryModel
    {
        public BillwisePurchaseSummaryModel()
        {
            BillwisePurchaseSummary = new List<BillwisePurchaseSummaryViewModel>();
            AggregationResult = new Dictionary<string, AggregationModel>();
        }

        public List<BillwisePurchaseSummaryViewModel> BillwisePurchaseSummary { get; set; }

        public Dictionary<string, AggregationModel> AggregationResult { get; set; }

        public int Total { get; set; }
    }

}
