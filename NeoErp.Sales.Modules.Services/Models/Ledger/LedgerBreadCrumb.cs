using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.Ledger
{
  public  class LedgerBreadCrumb
    {
       public string AccountCode { get; set; }
        public string MasterAccountCode { get; set; }
        public string ParentAccountCode { get; set; }
        public string AccountName { get; set; }
        public string AccountTypeFlag { get; set; }
    }

    public class LedgerAutosearch
    {
       public string value { get; set; }
        public string text { get; set; }
    }
}
