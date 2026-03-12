using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QuotationManagement.Service.Models
{
   public class Quotation
    {
        public int ID { get; set; }
        public string TENDER_NO { get; set; }
        public DateTime? ISSUE_DATE { get; set; }
        public DateTime? VALID_DATE { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public string NEPALI_DATE { get; set; }
        public string DELIVERY_DT_BS { get; set; }
        public string COMPANY_CODE { get; set; }
        public string REMARKS { get; set; }
        public char STATUS { get; set; }
        public string STATUS_DETAILS { get; set; }
        public string APPROVED_STATUS { get; set; }
        public List<Item_Detail> Items { get; set; }
        public List<PARTY_DETAIL> PartDetails { get; set; }
    }

    public class PARTY_DETAIL
    {
        public int QUOTATION_NO { get; set; }
        public string PARTY_NAME { get; set; }
        public string ITEM_CODE { get; set; }
        public string CHECKED_BY { get; set; }
        public string VERIFY_BY { get; set; }
        public string RECOMMENDED1_BY { get; set; }
        public string RECOMMENDED2_BY { get; set; }
        public string RECOMMENDED3_BY { get; set; }
        public string RECOMMENDED4_BY { get; set; }
        public string APPROVED_BY { get; set; }
        public decimal ACTUAL_PRICE { get; set; }
        public string STATUS { get; set; }
        public string REVISE { get; set; }

    }
    public class Item_Detail
    {
        public int ID { get; set; }
        public string ITEM_DESC { get; set; }
        public string ITEM_CODE { get; set; }
        public string SPECIFICATION { get; set; }
        public string IMAGE { get; set; }
        public string IMAGE_NAME { get; set; }
        public string UNIT { get; set; }
        public int QUANTITY { get; set; }
        public string CATEGORY { get; set; }
        public string BRAND_NAME { get; set; }
        public string INTERFACE { get; set; }
        public string TYPE { get; set; }
        public string LAMINATION { get; set; }
        public string ITEM_SIZE { get; set; }
        public string THICKNESS { get; set; }
        public string COLOR { get; set; }
        public string GRADE { get; set; }
        public int SIZE_LENGTH { get; set; }
        public int SIZE_WIDTH { get; set; }
        public double? LAST_PRICE { get; set; }
        public string LAST_VENDOR { get; set; }

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
        public int CURRENCY_RATE { get; set; }
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
}
