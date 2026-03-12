using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Models.Mobiles
{
    public class NotificationStatusModel
    {
        public string VOUCHER_NO { get; set; }
        public string VOUCHER_AMOUNT { get; set; }
        public string CHECKED_BY { get; set; }
        public string AUTHORISED_BY { get; set; }
        public string SESSION_ROWID { get; set; }
    }

    public class PURCHASE_RM_MODEL
    {
        public string MRR_NO { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public decimal? QTY { get; set; }
        public decimal? BG_AMOUNT { get; set; }
        public int ALERT_PRIOR_DAYS { get; set; }
        public DateTime? END_DATE { get; set; }
        public string ACC_CODE { get; set; }
        public string BANK_GNO { get; set; }
        public string PARTY_NAME { get; set; }
    }
}