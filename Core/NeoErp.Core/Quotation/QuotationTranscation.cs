using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NeoErp.Core.Quotation
{
    public class QuotationTranscation
    {
        public string TENDER_NO { get; set; }
        public int SERIAL_NO { get; set; }
        public string QUOTATION_NO { get; set; }
        public string SYN_ROWID { get; set; }
        public string SESSION_ID { get; set; }
        public string DOCUMENT_NAME { get; set; }
        public string DOCUMENT_FILE_NAME { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
    }
}