using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.LOC.Services.Models
{
    public class CIPaymentSettlementModel
    {
        public string PS_CODE { get; set; }
        public string LC_TRACK_NO { get; set; }
        public string INVOICE_CODE { get; set; }
        public string INVOICE_NUMBER { get; set; }
        public string CI_AMOUNT { get; set; }
        public string CURRENCY { get; set; }
        public string EXCHANGE_RATE_AT_PAYMENT { get; set; }
        public string DERIVED_TOTAL_AMOUNT { get; set; }
        public string SETTLEMENT_DATE { get; set; }
        public string PP_DATE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public string MODIFIED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string RESULT { get; set; }
        public string FILE_DETAIL { get; set; }
        public IList<Items> Itemlist { get; set; }
        public string[] mylist { get; set; }
        public string pterm { get; set; }
        public string INVALID { get; set; }
        public string PTERMS { get; set; }
    }


    public class lcPaymentSettlementDoc
    {
        public string PS_DOC_CODE { get; set; }
        public string PS_CODE { get; set; }
        public string SNO { get; set; }
        public string LC_TRACK_NO { get; set; }
        public string INVOICE_CODE { get; set; }
        public string DOCUMENT_CODE { get; set; }
        public string DOCUMENT_DATE { get; set; }
        public string FILE_URL { get; set; }
        public string DOC_ACTION { get; set; }
        public string DOC_PREPARE_DATE { get; set; }
        public string EST_RECIEVED_DATE { get; set; }
        public string RECIEVED_DATE { get; set; }
        public string EST_SUBMIT_DATE { get; set; }
        public string SUBMITTED_DATE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_DATE { get; set; }
        public string LAST_MODIFIED_BY { get; set; }
        public string LAST_MODIFIED_DATE { get; set; }
        public string APPROVED_BY { get; set; }
        public string APPROVED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }

    }

    public class CIPaymentSettlementHistoryModel
    {
        public int PS_DOC_CODE { get; set; }
        public int PS_CODE { get; set; }
        public int LC_TRACK_NO { get; set; }
        public int SNO { get; set; }
        public string FILE_DETAIL { get; set; }
        public string REMARKS { get; set; }
        public string CREATED_BY_EDESC { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string LAST_MODIFIED_BY_EDESC { get; set; }
        public DateTime? LAST_MODIFIED_DATE { get; set; }
    }
}