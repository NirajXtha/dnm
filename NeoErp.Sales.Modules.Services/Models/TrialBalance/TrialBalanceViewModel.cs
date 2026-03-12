using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.TrialBalance
{
    public class TrialBalanceViewModel
    {
        public string account_head { get; set; }

        public int? TreeLevel { get; set; }
        public string PRE_ACC_CODE { get; set; }
        public string MASTER_ACC_CODE { get; set; }
        public string ACC_CODE { get; set; }
        public decimal? DR_OPENING { get; set; }
        public decimal? CR_OPENING { get; set; }
        public decimal? CR_AMT { get; set; }
        public decimal? DR_AMT { get; set; }
        public decimal? DR_CLOSING { get; set; }
        public decimal? CR_CLOSING { get; set; }
        public int? CHILD_REC { get; set; } = 0;
        public bool hasChildren { get; set; } = true;
        public string parentId { get; set; }
        public int? ParentIdInt { get; set; }
        public int? Id { get; set; }
        public int? sub_count { get; set; } = 0;
        public string Sub_code { get; set; }
        

    }

    public class PLLedgerViewModel
    {
        public string DESCRIPTION { get; set; }

        public int? parentId { get; set; }
        public int? id { get; set; }
        public string PL_code { get; set; }
        public double? BalanceAmt { get; set; }
        public string Acc_code { get; set; }

        public bool hasChildren { get; set; } = false;

        public string Sign { get; set; } = "+";
        public bool signoperater { get; set; } = true;
    }
}
