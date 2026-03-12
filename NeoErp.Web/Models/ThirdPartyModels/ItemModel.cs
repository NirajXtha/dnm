using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Models.ThirdPartyModels
{
    public class ItemModel
    {
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public string CategoryId { get; set; }
        public string UnitId { get; set; }
    }
}