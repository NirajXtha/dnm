using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using static System.Collections.Specialized.BitVector32;

namespace NeoERP.QuotationManagement.Service.Models
{
    public class Quotation_setup
    {
        public int QUOTATION_NO { get; set; }
        public int ID { get; set; }
        public string FORM_CODE { get; set; }
        public string TENDER_NO { get; set; }
        public DateTime? ISSUE_DATE { get; set; }
        public DateTime? VALID_DATE { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public string NEPALI_DATE { get; set; }
        public string DELIVERY_DT_BS { get; set; }
        public string LOG_REMARKS { get; set; }
        public string COMPANY_CODE { get; set; }
        public string REMARKS { get; set; }
        public char STATUS { get; set; }
        public string LOCAL_FLAG { get; set; }
        public string BRANCH_CODE { get; set; }
        public string STATUS_DETAILS { get; set; }
        public string APPROVED_STATUS { get; set; }
        public DateTime? MODIFIED_DATE { get; set; }
        public List<Item> Items { get; set; }
        public string ROW_REFERENCE { get; set; }
        public List<Reference> References { get; set; }
    }

    public class Item
    {
        public int ID { get; set; }
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
        public float PRICE { get; set; }
    }

    public class QuotationItem
    {
        public string QUOTATION_NO { get; set; }
        public int ID { get; set; }
        public int QUANTITY { get; set; }
    }
    public class Reference
    {
        public string FORM_CODE { get; set; }
        public string REF_FORM_CODE { get; set; }
        public string TENDER_NO { get; set; }
        public string REFERENCE_NO { get; set; }
        public string ITEM_CODE { get; set; }
        public string SPECIFICATION { get; set; }
        public string IMAGE { get; set; }
        public string IMAGE_NAME { get; set; }
        public string UNIT { get; set; }
        public int QUANTITY { get; set; }
        public int REF_QTY { get; set; }
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
        public float PRICE { get; set; }
    }

    public class QuotationLogModel
    {
        public int? ID { get; set; }
        public int? QUOTATION_ID { get; set; }
        public int? QUOTATION_NO { get; set; }
        public string TENDER_NO { get; set; }
        public string TYPE { get; set; }
        public string ACTION { get; set; }
        public string ACTION_BY { get; set; }
        public DateTime? ACTION_DATE { get; set; }
        public string CHANGED { get; set; }
        [Column("REMARKS")]
        private string _remarks;

        [NotMapped]
        public string REMARKS
        {
            get {
                return IsUriEncoded(_remarks)
                ? DecodeUriComponent(_remarks)
                : _remarks;
            }
            set { _remarks = EncodeUriComponent(value); }
        }
        [NotMapped]
        public string REMARKS_ENCODED
        {
            get { return _remarks; }
        }
        private static string EncodeUriComponent(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            return Uri.EscapeDataString(value).Replace("'", "%27");
        }
        private static bool IsUriEncoded(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            return Regex.IsMatch(value, @"%[0-9A-Fa-f]{2}");
        }
        private static string DecodeUriComponent(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            string decoded = value;

            while (Regex.IsMatch(decoded, @"%[0-9A-Fa-f]{2}"))
            {
                string temp = Uri.UnescapeDataString(decoded);

                if (temp == decoded)
                    break;

                decoded = temp;
            }

            return decoded;
        }

    }
}
