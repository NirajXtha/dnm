using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NeoErp.Core.Quotation
{
    public class Quotation
    {
        public int ID { get; set; }
        public string TENDER_NO { get; set; }
        public DateTime? ISSUE_DATE { get; set; }
        public DateTime? VALID_DATE { get; set; }
        public DateTime? BS_DATE { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public string NEPALI_DATE { get; set; }
        public string DELIVERY_DT_BS { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string REMARKS { get; set; }
        public string LOCAL_FLAG { get; set; }
        public string VAT_INCLUDE_FLAG { get; set; }
        public string IS_POSTED { get; set; }
        public List<Item> Items { get; set; }

    }

    public class Item
    {
        public int ID { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_DESC { get; set; }
        public string SPECIFICATION { get; set; }
        public string IMAGE { get; set; }
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
        public string IMAGE_LINK { get; set; }

    }
}