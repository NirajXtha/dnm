using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Models.ThirdPartyModels
{
    public class SalesInvoiceModel
    {
        public string SalesNumber { get; set; }
        public string SalesDate { get; set; }
        public string ItemId { get; set; }
        public string UnitId { get; set; }
        public int Quantity { get; set; }
        public string Remarks { get; set; }
        public string TransactionRefNo { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
    }
}