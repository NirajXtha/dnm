using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QuotationManagement.Service.Models
{
   public class SummaryReport
    {
        public int ID { get; set; }
        public string TENDER_NO { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public DateTime? VALID_DATE { get; set; }
        public string ITEM_DESC { get; set; }
        public string STATUS { get; set; }
        public string SPECIFICATION { get; set; }
        public int QUANTITY { get; set; }
        public string UNIT { get; set; }
        public string CHECKED_BY { get; set; }
        public string VERIFIED_BY { get; set; }
        public string RECOMMENDED_BY { get; set; }
        public string APPROVED_BY { get; set; }
    }
}
