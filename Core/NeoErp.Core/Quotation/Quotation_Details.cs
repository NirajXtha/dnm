using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Core.Quotation
{
    public class Quotation_Details
    {
        public string ID { get; set; }
        public int QUOTATION_NO { get; set; }
        public string TENDER_NO { get; set; }
        public string PAN_NO { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string PARTY_NAME { get; set; }
        public string ADDRESS { get; set; }
        public string CONTACT_NO { get; set; }
        public string EMAIL { get; set; }
        public string CURRENCY { get; set; }
        public decimal CURRENCY_RATE { get; set; }
        public DateTime? DELIVERY_DATE { get; set; }
        public decimal TOTAL_AMOUNT { get; set; }
        public string BRANCH_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public string COMPANY_CODE { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public decimal TOTAL_DISCOUNT { get; set; }
        public decimal TOTAL_EXCISE { get; set; }
        public decimal TOTAL_TAXABLE_AMOUNT { get; set; }
        public decimal TOTAL_VAT { get; set; }
        public decimal TOTAL_NET_AMOUNT { get; set; }
        public char STATUS { get; set; }
        public string DISCOUNT_TYPE { get; set; }
        public string REVISE { get; set; }
        public string VAT_INCLUDE_FLAG { get; set; }
        public List<Item_details> Item_Detail { get; set; }
        public List<Term_Conditions> TermsCondition { get; set; }
    }
    public class Item_details
    { 
        public int ID { get; set; }
        public int QUOTATION_NO { get; set; }
        public string ITEM_CODE { get; set; }
        public decimal RATE { get; set; }
        public decimal AMOUNT { get; set; }
        public decimal DISCOUNT { get; set; }
        public decimal DISCOUNT_AMOUNT { get; set; }
        public decimal EXCISE { get; set; }
        public decimal TAXABLE_AMOUNT { get; set; }
        public decimal VAT_AMOUNT { get; set; }
        public decimal NET_AMOUNT { get; set; }

    }

    public class Term_Conditions
    {
        public string TERM_CONDITION { get; set; }
    }
}