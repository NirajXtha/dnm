using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Models.Report
{
    public class TrialBalanceViewModel
    {
        public string account_head { get; set; }
      
        public int TreeLevel { get; set; }
        public string PRE_ACC_CODE { get; set; }
        public string MASTER_ACC_CODE { get; set; }
        public string ACC_CODE { get; set; }
        public decimal? DR_OPENING { get; set; }
        public decimal? CR_OPENING { get; set; }
        public decimal? CR_AMT { get; set; }
        public decimal? DR_AMT { get; set; }
        public decimal? DR_CLOSING { get; set; }
        public decimal? CR_CLOSING{get;set;}
public int CHILD_REC { get; set; }
    }

    public class salesReortViewModel
    {

       public int Year { get; set; }
       public int Month { get; set; }
        public string Item_code { get; set; }
        public int Quantity { get; set; }
        public decimal price { get; set; }
        public string Item_edesc { get; set; }
        public string Customer_code { get; set; }
        public string Customer_edesc { get; set; }
    }
}