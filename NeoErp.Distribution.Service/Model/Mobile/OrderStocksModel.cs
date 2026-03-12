using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.Mobile
{
    public class OrderStocksModel
    {
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string ITEM_CODE { get; set; }
        public decimal? STOCK_BALANCE {  get; set; }
        public string LOCATION_EDESC { get; set; }
        public string SP_CODE { get; set; }
        public string CODE { get; set; }
        public string ENTITY_TYPE { get; set; } = "D";
    }
}
