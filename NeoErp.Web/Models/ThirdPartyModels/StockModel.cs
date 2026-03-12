using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Models.ThirdPartyModels
{
    public class StockModel
    {
        public decimal StockBalQty { get; set; }
        public string ItemNumber { get; set; }
        public string ItemName { get; set; }
        public string CatId { get; set; }
        public string CatName { get; set; }
        public string HubId { get; set; }
        public string HubName { get; set; }
        public string UnitId { get; set; }
        public string UnitName { get; set; }
        public string BranchId { get; set; }
        public string BranchName { get; set; }
    }
}