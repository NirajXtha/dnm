using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model
{
   public  class SchemeModel
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
        public string CheckedStatus { get; set; }
        public string Status { get; set; }

        public List<string> ItemCode { get; set; }          
        public string OfferType { get; set; }
        public string Action { get; set; }
        public List<SchemeDetailModel> SchemeDetails { get; set; }
    }

    public class SchemeDetailModel
    {
        public string Rule_ID { get; set; }
        public string Max_Value { get; set; }
        public string Min_Value { get; set; }
        public List<string> GiftItemCode { get; set; }

        public string GiftQty { get; set; }
        public string Discount { get; set; }
        public string DiscountType { get; set; }
    }


    public class SchemeGiftModel
    {
        public SchemeGiftModel()
        {
            ItemCode = new List<string>();
        }
        public string Rule_ID { get; set; }
        public List<string> ItemCode { get; set; }

        public string ItemName { get; set; }
        public string Max_Value { get; set; }
        public string Min_Value { get; set; }
        public string Gift_QTY { get; set; }
    }
}
