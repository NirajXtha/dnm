using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NeoErp.Core.Quotation
{
    public class Supplier
    {
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_EDESC { get; set; }
        public string EMAIL { get; set; }
        public string ADDRESS { get; set; }
        public string CONTACT_NO { get; set; }
        public string MASTER_SUPPLIER_CODE { get; set; }
    }
    public class PartyDetailsItems
    {
        public int ID { get; set; }
        public int QUOTATION_NO { get; set; }
        public string ITEM_CODE { get; set; }
        public decimal? RATE { get; set; }
        public decimal? AMOUNT { get; set; }
        public decimal? DISCOUNT { get; set; }
        public decimal? DISCOUNT_AMOUNT { get; set; }
        public decimal? EXCISE { get; set; }
        public decimal? TAXABLE_AMOUNT { get; set; }
        public decimal? VAT_AMOUNT { get; set; }
        public decimal? NET_AMOUNT { get; set; }
    }
    public class PartyDetails
    {
        public string PAN_NO { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string CURRENCY { get; set; }
        public decimal? CURRENCY_RATE { get; set; }
        public DateTime? DELIVERY_DATE { get; set; }
        public string DISCOUNT_TYPE { get; set; }
    }
    public class TermsAndConditions
    {
        public int ID { get; set; }
        public string TENDER_NO { get; set; }
        public int QUOTATION_NO { get; set; }
        public string TERM_CONDITION { get; set; }
    }
    public class CurrencyQuotationModel
    {
        public string CURRENCY_CODE { get; set; }
        public string CURRENCY_EDESC { get; set; }
        public string COUNTRY { get; set; }
        public string CURRENCY_SYMBOL { get; set; }
    }
}