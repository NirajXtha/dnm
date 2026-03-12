using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QuotationManagement.Service.Models
{
    public class Tender
    {
        public string ID { get; set; }
        public string PREFIX { get; set; }
        public string SUFFIX { get; set; }
        public int? BODY_LENGTH { get; set; }
        public string STATUS { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string CREATED_BY {get;set;}
        public string COMPANY_CODE { get; set; }
        public string FORM_EDESC { get; set; }
        public string FORM_NDESC { get; set; }
        public bool IS_UPDATE { get; set; }
    }
    public class ApprovalRequest
    {
        public string TENDER_NO { get; set; }
        public string REMARKS { get; set; }
        public int? ID { get; set; }
        public string quotationNo { get; set; }
        public string status { get; set; }
        public string type { get; set; }
        public string items { get; set; }
        public string itemId { get; set; }
    }
    public class Template
    {
        public string prefix { get; set; }
        public string Reference { get; set; }
        public string Row { get; set; }
        public string Voucher_no { get; set; }
        public string Item { get; set; }
        public string Name { get; set; }
        public DateTime? toDate { get; set; }
        public DateTime? fromDate { get; set; }
    }
}
