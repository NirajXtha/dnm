using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.sales.Module.Models
{
    public class LedgerSetting
    {
        public bool showBreadCrumb { get; set; }
        public bool showDateTime { get; set; }
        //public bool 
    }

    public class LedgerSearch
    {
        public string formDate { get; set; }
        public string toDate { get; set; }
        public string accountCode { get; set; }
        public string GridName { get; set; }
        public string SubCode { get; set; }
        public string BranchCode { get; set; }
        public string DataGeneric { get; set; }
        public string listType { get; set; }
        public string linkSubCode { get; set; }
        public string groupSkuFlag { get; set; }
        public string actionName { get; set; }
        public string MasterCode { get; set; }

        public string CompanyCode { get; set; }
      
    }
}