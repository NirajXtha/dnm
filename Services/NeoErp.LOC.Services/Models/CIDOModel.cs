using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.LOC.Services.Models
{
    public class CIDOModel
    {
        public string LC_DO_CODE { get; set; }
        public string LC_TRACK_NO { get; set; }
        public string INVOICE_CODE { get; set; }
        public string ITEM_CODE { get; set; }
        public string DO_NUMBER { get; set; }
        public string QUANTITY { get; set; }
        public string REMARKS { get; set; }
        public string STATUS { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_DATE { get; set; }
        public string LAST_MODIFIED_BY { get; set; }
        public string LAST_MODIFIED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
    }
}