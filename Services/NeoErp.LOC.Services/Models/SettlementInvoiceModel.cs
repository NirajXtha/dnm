using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.LOC.Services.Models
{
    public class SettlementInvoiceModel
    {
        public int SETTLEMENT_CODE { get; set; }
        public int SETTLEMENT_NEXT_CODE { get; set; }
        public int INVOICE_CODE { get; set; }
        public string INVOICE_NUMBER { get; set; }
        public string SETTLEMENT_DATE { get; set; }
        public string SWIFT_COPY { get; set; }
        public string REMARKS { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_DATE { get; set; }
        public string LAST_MODIFIED_BY { get; set; }
        public string LAST_MODIFIED_DATE { get; set; }
        public string APPROVED_BY { get; set; }
        public string APPROVED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string ATTACH_DOC { get; set; }
        public string[] mylist { get; set; }
    }

    public class SettlementInvocieddlModel
    {
        public int INVOICE_CODE { get; set; }
        public string INVOICE_NUMBER { get; set; }
        
    }
}