using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QuotationManagement.Service.Models
{
    public class QuotationNotification
    {
        public string TENDER_NO { get; set; }
        public string MESSAGE { get; set; }
        public DateTime NOTIF_DATE { get; set; }
        public string ACTION { get; set; }
    }
}
