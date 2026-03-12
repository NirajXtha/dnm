using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.Mobile
{
    public class SchemeReportResponseModel
    {

        public string SchemeID { get; set; }
        public string SchemeName { get; set; }
        public string AreaCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string AreaName { get; set; }
        public string SP_CODE { get; set; }
        public List<string> ResellerCode { get; set; }

        public List<ItemDetails> Items { get; set; }
        public string OfferType { get; set; }
       public List<SchemeDetailModel> SchemeDetails { get; set; }
    }
    public class SchemeDetailModel
    {

        public SchemeDetailModel()
        {
            Gift_Items = new List<ItemDetails>();
        }
        public string Rule_ID { get; set; }
        public string Max_Value { get; set; }
        public string Min_Value { get; set; }
        public List<ItemDetails> Gift_Items { get; set; }

        public string Gift_QTY { get; set; }
        public string Discount { get; set; }
        public string DiscountType { get; set; }
    }


    
    public class ItemDetails
    {
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
    }
}
