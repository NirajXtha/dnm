using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Web;

namespace NeoErp.LOC.Services.Models
{
  
    public class PerformaInvoiceModel
    {

        public int PO_CODE { get; set; }
        public string REMARKS { get; set; }
        public string ORDER_NO { get; set; }
        public int PINVOICE_CODE { get; set; }
        public string PINVOICE_NO { get; set; }
        public DateTime? PINVOICE_DATE { get; set; }
        public int LC_TRACK_NO { get; set; }
        public int LC_TRACK { get; set; }
        public string  LC_NUMBER { get; set; }
        public string INTM_SWIFT_CODE { get; set; }
        public string INTM_BANK_EDESC { get; set; }
        public string SWIFT_CODE { get; set; }
        public int INTM_BANK_CODE { get; set; }
        public int BNF_CODE { get; set; }
        public string BNF_NAME { get; set; }
        //public string SUPPLIER_CODE { get; set; }
        public string BNF_ADDRESS { get; set; }
        public int BANK_CODE { get; set; }
        public string CURRENCY_CODE { get; set; }
        public string BANK_BRANCH { get; set; }
        public string FILE_DETAIL { get; set; }
        public string BANK_NAME { get; set; }
        public IList<Items> Itemlist { get; set; }
        public string[] mylist { get; set; }
        public DateTime? ACCEPTED_DOC_DATE { get; set; }
        public int TERMS_CODE { get; set; }
        public int PTERMS_CODE { get; set; }
        public int CREDIT_DAYS { get; set; }
        public DateTime? VALIDITY_DATE { get; set; }
        public DateTime? EST_DELIVERY_DATE { get; set; }
        public string SUPPLIER_EDESC { get; set; }
    }
    public class PerformaInvoiceSupplierModel
    {
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_EDESC { get; set; }
    }
    public class PerformaInvoiceBrandModel
    {
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
    }
    public class PerformaInvoiceCurrencyModel
    {
        public string CURRENCY_CODE { get; set; }
        public string CURRENCY_EDESC { get; set; }
    }

    public class ItemDetails
    {
        public int SNO { get; set; }
        public string ITEM_EDESC { get; set; }
        public string ITEM_CODE { get; set; }
        public string COUNTRY_EDESC { get; set; }
        public string CONTAINER_QUANTITY { get; set; }
        public decimal CALC_QUANTITY { get; set; }
        public decimal CALC_UNIT_PRICE { get; set; }
        public decimal CALC_TOTAL_PRICE { get; set; }
        public string MU_CODE { get; set; }
        public string HS_CODE { get; set; }
        public string COUNTRY_CODE { get; set; }
        public string REMARKS { get; set; }
        public int ITEM_NO { get; set; }
        public int LC_TRACK_NO { get; set; }
        public int BNF_BANK_CODE { get; set; }
        public string ADDRESS { get; set; }
        public string CURRENCY_CODE { get; set; }
        public string ADDED { get; set; }
        public string EDITED { get; set; }
        public int? INVOICE_QUANTITY { get; set; }
        public string LOGISTIC_PLAN_CODE { get; set; }
        public string SHIPPMENT_QUANTITY { get; set; }
        public string LOT_NO { get; set; }
        public decimal? EXCHANGE_RATE { get; set; }
        public decimal? SALES_RATE { get; set; }

    }

    public class CommericalInvoiceData {
        public ItemDetails ItemDetails { get; set; }
        public List<string> CONTAINER { get; set; }
    }


    public class CountryModels
    {
        public string COUNTRY_CODE { get; set; }
        public string COUNTRY_EDESC { get; set; }
    }

    public class HSModels
    {
        public string HS_CODE { get; set; }
        public string HS_EDESC { get; set; }
    }

    public class BeneficiaryModels
    {
        public int BNF_CODE { get; set; }
        public string BNF_EDESC { get; set; }
        public string ADDRESS { get; set; }
    }

    public class ItemList
    {
        public int CALC_QUANTITY { get; set; }
        public int CALC_TOTAL_PRICE { get; set; }
        public string HsCode { get; set; }
        public string ITEM_EDESC { get; set; }
        public string MU_CODE { get; set; }
        public string CountryCode { get; set; }

    }

    public class PendingItemModels
    {
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string MU_CODE { get; set; }
        public int? QUANTITY { get; set; }
        public int? INVOICE_QUANTITY { get; set; }
    }

    public class ImportItems
    {
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string MU_CODE { get; set; }
        public int QUANTITY { get; set; }
        public int AMOUNT { get; set; }
        public string HS_CODE { get; set; }
        public string COUNTRY_OF_ORIGIN { get; set; }
    }
    public class FileUpload
    {
        public HttpPostedFileBase file { get; set; }
    }
}
