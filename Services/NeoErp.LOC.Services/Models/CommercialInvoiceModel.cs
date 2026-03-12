using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Web;

namespace NeoErp.LOC.Services.Models
{
    public class CommercialInvoiceModel
    {
        public string AWB_NUMBER { get; set; }
        public DateTime? AWB_DATE { get; set; }
        public int INVOICE_CODE { get; set; }
        public DateTime? INVOICE_DATE { get; set; }
        public string INVOICE_NUMBER { get; set; }
        public int AMOUNT { get; set; }
        public int TEMP_INVOICE_CODE { get; set; }
        public int TEMP_LC_TRACK_NO { get; set; }
        public string INVOICE_CURRENCY { get; set; }
        public string CURRENCY_CODE { get; set; }
        public int QUANTITY { get; set; }
        public int LC_TRACK_NO { get; set; }
        public string MU_CODE { get; set; }
        public string CATEGORY_CODE { get; set; }
        public string CATEGORY_EDESC { get; set; }
        public string LC_NUMBER { get; set; }
        public decimal? EXCHANGE_RATE { get; set; }
        public decimal? SALES_EXG_RATE { get; set; }
        public DateTime? PP_DATE { get; set; }
        public string PP_NO { get; set; }
        public IList<Items> Itemlist { get; set; }
        public string FILE_DETAIL { get; set; }
        public string SUPPLIER_EDESC { get; set; }
        public string LC_NUMBER_CODE { get; set; }
        public string[] mylist { get; set; }
        public string LOT_NO { get; set; }
        public string IS_AIR { get; set; }

    }

    public class MultiCommercialInvoiceModel
    {
        public string LC_NUMBER { get; set; }
        public int LC_TRACK_NO { get; set; }
        public string TEMP_INVOICE_CODE { get; set; }
        public string TEMP_LC_TRACK_NO { get; set; }
        public IList<Items> Itemlist { get; set; }
        public IList<CommercialInvoiceData> CommercialInvoiceData { get; set; }
        public List<ContainerList> ContainerList { get; set; }
        public bool IS_AIR { get; set; }
    }


    public class ContainerList
    {
        public string LOGISTIC_PLAN_CODE { get; set; }
        public string PLAN_CONTAINER_CODE { get; set; }
        public string CONTAINER_CODE { get; set; }
        public string LOAD_TYPE { get; set; }
        public string CARRIER_NUMBER { get; set; }
        public string CONTAINER_EDESC { get; set; }
        public string SHIPPING_TYPE { get; set; }

    }
    public class CommercialInvoiceData
    {
        public string AWB_NUMBER { get; set; }
        public DateTime? AWB_DATE { get; set; }
        public int INVOICE_CODE { get; set; }
        public DateTime? INVOICE_DATE { get; set; }
        public string INVOICE_NUMBER { get; set; }
        public int AMOUNT { get; set; }
        public int TEMP_INVOICE_CODE { get; set; }
        public int TEMP_LC_TRACK_NO { get; set; }
        public string INVOICE_CURRENCY { get; set; }
        public string CURRENCY_CODE { get; set; }
        public int QUANTITY { get; set; }
        public int LC_TRACK_NO { get; set; }
        public string MU_CODE { get; set; }
        public string CATEGORY_CODE { get; set; }
        public string CATEGORY_EDESC { get; set; }
        public string LC_NUMBER { get; set; }
        public decimal EXCHANGE_RATE { get; set; }
        public DateTime? PP_DATE { get; set; }
        public string PP_NO { get; set; }
        public string FILE_DETAIL { get; set; }
        public string SUPPLIER_EDESC { get; set; }
        public string LC_NUMBER_CODE { get; set; }
        public string[] mylist { get; set; }
        public string LOT_NO { get; set; }
        public decimal? SALES_EXG_RATE { get; set; }

    }

    public class CommercialInvoiceUnitModel
    {
        public string MU_CODE { get; set; }
        public string MU_EDESC { get; set; }
    }
    public class CommercialInvoiceLOCModel
    {
        public int LOC_CODE { get; set; }
        public string LOC_NUMBER { get; set; }
    }
    public class CommercialInvoiceCategoryModel
    {
        public string CATEGORY_CODE { get; set; }
        public string CATEGORY_EDESC { get; set; }
    }
    public class CommercialInvoiceCurrencyModel
    {
        public string CURRENCY_CODE { get; set; }
        public string CURRENCY_EDESC { get; set; }
    }

    public class CommercialInvoicHistoryModel
    {
        //public int VERSION_CODE { get; set; }
        //public int LC_TRACK_NO { get; set; }
        //public int SNO { get; set; }
        //public string ITEM_CODE { get; set; }
        //public string ITEM_EDESC { get; set; }
        //public string MU_CODE { get; set; }
        //public int QUANTITY { get; set; }
        //public string CURRENCY_CODE { get; set; }
        //public int AMOUNT { get; set; }
        //public string HS_CODE { get; set; }
        //public string COUNTRY_OF_ORIGIN { get; set; }
        //public DateTime? CREATED_DATE { get; set; }
        //public string CREATED_BY_EDESC { get; set; }
        //public DateTime? LAST_MODIFIED_DATE { get; set; }
        //public string LAST_MODIFIED_BY_EDESC { get; set; }
        //public string REMARKS { get; set; }
        //public string COUNTRY_CODE { get; set; }
        public int INVOICE_CODE { get; set; }
        public int SNO { get; set; }
        public DateTime? INVOICE_DATE { get; set; }
        public string INVOICE_NUMBER { get; set; }
        public string LC_NUMBER { get; set; }
        public int LC_TRACK_NO { get; set; }
        public string INVOICE_CURRENCY { get; set; }
        public decimal? EXCHANGE_RATE { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public DateTime? PP_DATE { get; set; }
        public string PP_NO { get; set; }
        public decimal? AMOUNT { get; set; }
        public int QUANTITY { get; set; }
        public string MU_CODE { get; set; }
        public string AWB_NUMBER { get; set; }
        public DateTime? AWB_DATE { get; set; }
        public string BILL_NUMBER { get; set; }
        public DateTime? BILL_DATE { get; set; }
        public string CATEGORY_CODE { get; set; }
        public DateTime? SETTLEMENT_DATE { get; set; }
        public string SWIFT_COPY { get; set; }
        public int LOT_NO { get; set; }
        public string CREATED_BY_EDESC { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string LAST_MODIFIED_BY_EDESC { get; set; }
        public DateTime? LAST_MODIFIED_DATE { get; set; }
        public string APPROVED_BY { get; set; }
        public DateTime? APPROVED_DATE { get; set; }

    }

    public class InvoiceNumberModels
    {
        public int InvoiceCode { get; set; }
        [Required]
        public string InvoiceNumber { get; set; }
    }
}