using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.LOC.Services.Models
{
    public class PurchaseOrderModels
    {
        public int PO_CODE { get; set; }
        public int PO_NUMBER { get; set; }
        public string ORDER_NO { get; set; }
        public int BNF_BANK_CODE { get; set; }
        public DateTime ORDER_DATE { get; set; }
        public string LEAD_TIME { get; set; }
        public int LC_TRACK_NO { get; set; }
        public string MANUAL_NUMBER { get; set; }
        public int TERMS_CODE { get; set; }
        public int PTERMS_CODE { get; set; }
        public int CREDIT_DAYS { get; set; }
        public DateTime VALIDITY_DATE { get; set; }
        public DateTime EST_DELIVERY_DATE { get; set; }
        public string DELIVERY_PLACE_TYPE { get; set; }
        public string DELIVERY_PLACE { get; set; }
        public string APP_NAME { get; set; }
        public string APP_ADDRESS { get; set; }
        public string BILL_COMPANY_NAME { get; set; }
        public string BILL_COMPANY_ADD { get; set; }
        public string BILL_COMPANY_PHONE { get; set; }
        public string SHIP_COMPANY_NAME { get; set; }
        public string SHIP_COMPANY_ADD { get; set; }
        public string SHIP_COMPANY_PHONE { get; set; }
        public string CONTACT_NAME { get; set; }
        public string CONTACT_PHONE { get; set; }
        public string CONTACT_EMAIL { get; set; }
        public string REMARKS { get; set; }
        public string FROM_LOCATION { get; set; }
        public string TO_LOCATION { get; set; }
        public string SHIPMENT_TYPE { get; set; }
        public string LOAD_TYPE { get; set; }
        public int EST_DAY { get; set; }
        public string TERMS_EDESC { get; set; }
        public string PTERMS_EDESC { get; set; }
        public string BNF_EDESC { get; set; }
        public IList<Items> Itemlist { get; set; }
        public string[] mylist { get; set; }
        public string FILE_DETAIL { get; set; }
        public string TRANSSHIPMENT { get; set; }
        public string CURRENCY_CODE { get; set; }

        public string SUPPLIER_CODE { get; set; }

        public string SUPPLIER_EDESC { get; set; }
        public string WEEK_NUMBER { get; set; }
    }

    public class IpPurchaseOrderModels
    {
        public string ORDER_NO { get; set; }
    }

    public class Items
    {
        public int VERSION_CODE { get; set; }
        public string ITEM_CODE { get; set; }
        public decimal? QUANTITY { get; set; }
        public decimal? AMOUNT { get; set; }
        public string HS_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string TEMP_ITEM_CODE { get; set; }
        public string MU_CODE { get; set; }
        public string COUNTRY_CODE { get; set; }
        public string COUNTRY_OF_ORIGIN { get; set; }
        public string CURRENCY_CODE { get; set; }
        public string COUNTRY_EDESC { get; set; }
        public string REMARKS { get; set; }
        public int LC_TRACK_NO { get; set; }
        public int SNO { get; set; }
        public string Options { get; set; }
        public string EDITED { get; set; }
        public int INPUT_QUANTITY { get; set; }
        public int SHIPPMENT_QUANTITY { get; set; }
        public int INVOICE_CODE { get; set; }
        public string INVOICE_NUMBER { get; set; }
        public decimal? TOTAL_QUANTITY { get; set; }
        public string ADDED { get; set; }
        public int? INVOICE_QUANTITY { get; set; }
        public string AMTEDITED { get; set; }
        public string AMENDMENT { get; set; }
        public string LOT_NO { get; set; }
        public decimal? EXCHANGE_RATE { get; set; }
        public decimal? SALES_EXG_RATE { get; set; }

        public DateTime? PAYMENT_DATE { get; set; }
    }

    public class ItemDetail
    {
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string INDEX_MU_CODE { get; set; }
        public int ITEM_NO { get; set; }

    }

    public class Documents
    {
        public int LC_TRACK_NO { get; set; }
        public int PO_CODE { get; set; }
        public int PINVOICE_CODE { get; set; }
        public int LOC_CODE { get; set; }
        public int INVOICE_CODE { get; set; }
        public int SNO { get; set; }
        public string FILE_DETAIL { get; set; }
        public string REMARKS { get; set; }
        public string[] mylist { get; set; }
    }

    public class Currency
    {
        public string CURRENCY_CODE { get; set; }
        public string CURRENCY_EDESC { get; set; }
    }

    public class ItemHistoryModels
    {
        public int VERSION_CODE { get; set; }
        public int LC_TRACK_NO { get; set; }
        public int SNO { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string MU_CODE { get; set; }
        public int QUANTITY { get; set; }
        public string CURRENCY_CODE { get; set; }
        public int AMOUNT { get; set; }
        public string HS_CODE { get; set; }
        public string COUNTRY_OF_ORIGIN { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string CREATED_BY_EDESC { get; set; }
        public DateTime? LAST_MODIFIED_DATE { get; set; }
        public string LAST_MODIFIED_BY_EDESC { get; set; }
        public string REMARKS { get; set; }
        public string COUNTRY_CODE { get; set; }
    }

    public class ShipmentHistoryModels
    {
        public int VERSION_CODE { get; set; }
        public int LC_TRACK_NO { get; set; }
        public int SNO { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string MU_CODE { get; set; }
        public int QUANTITY { get; set; }
        public string FROM_LOCATION { get; set; }
        public string TO_LOCATION { get; set; }
        public string SHIPMENT_TYPE { get; set; }
        public string LOAD_TYPE { get; set; }
        public int EST_DAY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string CREATED_BY_EDESC { get; set; }
        public DateTime? LAST_MODIFIED_DATE { get; set; }
        public string LAST_MODIFIED_BY_EDESC { get; set; }
    }

    public class DocumentHistoryModels
    {
        public int VERSION_CODE { get; set; }
        public int LC_TRACK_NO { get; set; }
        public int SNO { get; set; }
        public int? PO_CODE { get; set; }
        public string ORDER_NO { get; set; }
        public int? PINVOICE_CODE { get; set; }
        public string[] mylist { get; set; }
        public string PINVOICE_NO { get; set; }
        public int? LOC_CODE { get; set; }
        public string LC_NUMBER { get; set; }
        public int? INVOICE_CODE { get; set; }
        public string FILE_DETAIL { get; set; }
        public string REMARKS { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string CREATED_BY_EDESC { get; set; }
        public DateTime? LAST_MODIFIED_DATE { get; set; }
        public string LAST_MODIFIED_BY_EDESC { get; set; }
        public string  INVOICE_NUMBER { get; set; }
        public string SUPPLIER_EDESC { get; set; }
    }

    public class LCDetailsViewModels
    {
        public int? PO_CODE { get; set; }
        public int? PO_NUMBER { get; set; }
        public string ORDER_NO { get; set; }
        public int? BNF_BANK_CODE { get; set; }
        public DateTime? ORDER_DATE { get; set; }
        public int? LC_TRACK_NO { get; set; }
        public string MANUAL_NUMBER { get; set; }
        public int? TERMS_CODE { get; set; }
        public int? PTERMS_CODE { get; set; }
        public int? CREDIT_DAYS { get; set; }
        public DateTime? VALIDITY_DATE { get; set; }
        public DateTime? EST_DELIVERY_DATE { get; set; }
        public string DELIVERY_PLACE_TYPE { get; set; }
        public string DELIVERY_PLACE { get; set; }
        public string APP_NAME { get; set; }
        public string APP_ADDRESS { get; set; }
        public string BILL_COMPANY_NAME { get; set; }
        public string BILL_COMPANY_ADD { get; set; }
        public string BILL_COMPANY_PHONE { get; set; }
        public string SHIP_COMPANY_NAME { get; set; }
        public string SHIP_COMPANY_ADD { get; set; }
        public string SHIP_COMPANY_PHONE { get; set; }
        public string CONTACT_NAME { get; set; }
        public string CONTACT_PHONE { get; set; }
        public string CONTACT_EMAIL { get; set; }
        public string REMARKS { get; set; }
        public string FROM_LOCATION { get; set; }
        public string TO_LOCATION { get; set; }
        public string SHIPMENT_TYPE { get; set; }
        public string LOAD_TYPE { get; set; }
        public int? EST_DAY { get; set; }
        public string TERMS_EDESC { get; set; }
        public string PTERMS_EDESC { get; set; }
        public string BNF_EDESC { get; set; }
        public string FILE_DETAIL { get; set; }
        public string TRANSSHIPMENT { get; set; }
        public string CURRENCY_CODE { get; set; }  
        public int? PINVOICE_CODE { get; set; }
        public string PINVOICE_NO { get; set; }
        public DateTime? PINVOICE_DATE { get; set; }
        public int? LC_TRACK { get; set; }
        public string INTM_SWIFT_CODE { get; set; }
        public string INTM_BANK_EDESC { get; set; }
        public string SWIFT_CODE { get; set; }
        public int? INTM_BANK_CODE { get; set; }
        public string LC_NUMBER { get; set; }
        public string ADDRESS { get; set; }
        public int? BANK_CODE { get; set; }
        public string BANK_BRANCH { get; set; }
        public string BANK_NAME { get; set; }
        public DateTime? ACCEPTED_DOC_DATE { get; set; }
        public int? LOC_CODE { get; set; }
        public DateTime? OPEN_DATE { get; set; }
        public DateTime? EXPIRY_DATE { get; set; }
        public string EXPIRY_PLACE { get; set; }
        public int? STATUS_CODE { get; set; }
        public string STATUS_EDESC { get; set; }
        public DateTime? LAST_SHIPMENT_DATE { get; set; }
        public int? TOLERANCE_PER { get; set; }
        public int? ADVISING_BANK_CODE { get; set; }
        public string ADVISING_BANK_EDESC { get; set; }
        public string CONFIRM_BANK_EDESC { get; set; }
        public string ISSUING_BANK_EDESC { get; set; }
        public int? CONFIRM_BANK_CODE { get; set; }
        public int? ISSUING_BANK_CODE { get; set; }
        public string PARTIAL_SHIPMENT { get; set; }
        public string CONFIRMATION_REQ { get; set; }
        public string TRANSFERABLE { get; set; }
        public string INSURANCE_FLAG { get; set; }
        public string APP_OUT_CHARGE { get; set; }
        public string BEF_OUT_CHARGE { get; set; }
        public string APP_CONFIRM_CHARGE { get; set; }
        public string BNF_CONFIRM_CHARGE { get; set; }
        public int? DOC_REQ_DAYS { get; set; }
        public string ORIGIN_COUNTRY_CODE { get; set; }
       
        public string LC_TERMS_EDESC { get; set; }
        public string LC_PTERMS_EDESC { get; set; }
    }


    public class POModel {
        public string PODate { get; set; }
        public string POBenificary { get; set; }
    }

}