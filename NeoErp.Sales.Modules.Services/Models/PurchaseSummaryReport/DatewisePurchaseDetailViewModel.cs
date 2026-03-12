using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.PurchaseSummaryReport
{
    public class DatewisePurchaseDetailViewModel
    {
        public string INVOICE_NO { get; set; }
        public DateTime? INVOICE_DATE { get; set; }
        public string MITI { get; set; }
        public string ORDER_NO { get; set; }
        public string FORM_CODE { get; set; }
        public string FISCAL { get; set; }
        public string PP_NO { get; set; }
        public string SUPPLIER_INV_NO { get; set; }
        public DateTime? SUPPLIER_INV_DATE { get; set; }
        public DateTime? DUE_DATE { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_EDESC { get; set; }
        public string ADDRESS { get; set; }
        public string TPIN_VAT_NO { get; set; }
        public string CURRENCY_CODE { get; set; }
        public double? EXCHANGE_RATE { get; set; }
        public double? INVOICE_TOTAL_PRICE { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string PRODUCT_CODE { get; set; }
        public string BRAND_NAME { get; set; }
        public string CATEGORY { get; set; }
        public string UPC { get; set; }
        public string HS_CODE { get; set; }
        public string UNIT { get; set; }
        public double? QUANTITY { get; set; }
        public double? UNIT_PRICE { get; set; }
        public double? TOTAL_PRICE { get; set; }
        public double? DISCOUNT { get; set; }
        public double? EXCISE_DUTY { get; set; }
        public double? ADD_CHARGE { get; set; }
        public double? TAXABLE_TOTAL_PRICE { get; set; }
        public double? VAT_TOTAL_PRICE { get; set; }
        public int NON_VAT { get; set; }
        public double? TOTAL_IN_NRS { get; set; }
        public double? LANDED_IN_NRS { get; set; }
        public DateTime? RETURN_DATE { get; set; }
        public string RETURN_NO { get; set; }
        public double? D1 { get; set; }
        public double? D2 { get; set; }
        public double? D3 { get; set; }
        public double? D4 { get; set; }
        public double? D5 { get; set; }
        public double? D6 { get; set; }
        public double? D7 { get; set; }
        public double? D8 { get; set; }
        public double? TOTAL_LANDED_COST { get; set; }
    }

    public class DatewisePurchaseDetailModel
    {
        public DatewisePurchaseDetailModel()
        {
            DateWisePurchase = new List<DatewisePurchaseDetailViewModel>();
            AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public List<DatewisePurchaseDetailViewModel> DateWisePurchase { get; set; }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
        public int Total { get; set; }
    }
    public class FormCodeData
    {
        public string FORM_CODE { get; set; }
        public string FORM_EDESC { get; set; }
    }

    public class AllItem
    {
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string MASTER_ITEM_CODE { get; set; }
        public string PRE_ITEM_CODE { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
    }

    public class AllLocation
    {
        public string LOCATION_CODE { get; set; }
        public string LOCATION_EDESC { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
    }
}
