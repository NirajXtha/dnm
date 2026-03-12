using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.Mobile
{
    public class ClosingStockResponseModel
    {
        public ClosingStockResponseModel()
        {
            item = new Dictionary<string, Dictionary<string, ClosingStockItemModel>>();
            mu_code = new Dictionary<string, Dictionary<string, MuCodeResponseModel>>();
        }
        public Dictionary<string, Dictionary<string, ClosingStockItemModel>> item { get; set; }
        public Dictionary<string, Dictionary<string, MuCodeResponseModel>> mu_code { get; set; }
    }
    public class ClosingStockItemModel
    {
        public string ITEM_EDESC { get; set; }
        public string INDEX_MU_CODE { get; set; }
        public string LVS { get; set; }
        public string BRAND_NAME { get; set; }
        public string ITEM_CODE { get; set; }
    }
}