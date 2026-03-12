using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.Mobile
{
    public class LastRateModel
    {
        public string COMPANY_CODE {get; set;}
        public string CUSTOMER_CODE {get; set;}
        public string ITEM_CODE {get; set;}
        public string ITEM_EDESC {get; set;}
        public decimal? LAST_RATE {get; set;}
    }
}
