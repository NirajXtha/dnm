using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model
{
    public class SchemeReportModel
    {
        public string SchemeID { get; set; }
        public string SchemeName { get; set; }

         public List<string> AreaCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string AreaName { get; set; }
        public string SP_CODE { get; set; }
        public string Employee_Name { get; set; }
       
        public List<string> CustomerCode { get; set; }
        public List<string> ResellerCode { get; set; }

        public List<string> ItemCode { get; set; }
        public string OfferType { get; set; }
        public string CurrentSlot { get; set; }
        public string SchemeDiscount_Qty { get; set; }
        public string DiscountType { get; set; }
        public string ActualAmount { get; set; }
        public string ActualDiscount { get; set; }
        public string NextScheme { get; set; }
        public string NextSchemeTarget { get; set; }
        public string NextDiscount{ get; set; }
        public string NextDiscountType{ get; set; }



        public int ORDER_NO { get; set; }
        public DateTime ORDER_DATE { get; set; }
        public string MITI { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string RESELLER_NAME { get; set; }
        public string ITEM_EDESC { get; set; }
        public string ORDER_ENTITY { get; set; }
        public string PARTY_TYPE_CODE { get; set; }
        public string PARTY_TYPE_EDESC { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string APPROVED_FLAG { get; set; }
        public string DISPATCH_FLAG { get; set; }
        public string REJECTED_FLAG { get; set; }
        public string EMPLOYEE_EDESC { get; set; }
        public string PO_PARTY_TYPE { get; set; }
        public string PO_CONVERSION_FACTOR { get; set; }
        public string PO_BILLING_NAME { get; set; }
        public int CREDIT_LIMIT { get; set; }
        public decimal? TOTAL_QUANTITY { get; set; }
        public decimal? GRAND_TOTAL_AMOUNT { get; set; }
        public int GRAND_APPROVED_QUENTIITY { get; set; }
        public decimal? TOTAL_APPROVED_AMOUNT { get; set; }
        public string PO_CONVERSION_UNIT { get; set; }
        public decimal? CONVERSION_FACTOR { get; set; }
        public string CONVERSION_MU_CODE { get; set; }
        public int QUANTITY { get; set; }
        public string MU_CODE { get; set; }
        public decimal? UNIT_PRICE { get; set; }
        public decimal? NET_TOTAL { get; set; }
        public decimal? TOTAL_AMOUNT { get; set; }
        public string REMARKS { get; set; }
        public string EntityName { get; set; }
        public string RESELLER_CODE { get; set; }
        public string ITEM_CODE { get; set; }
        public string REJECT_FLAG { get; set; }
    }
}
