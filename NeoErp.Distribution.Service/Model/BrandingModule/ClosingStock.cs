using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.BrandingModule
{
    public class ClosingStock
    {
        public string DistCustomer { get; set; }
        public List<ClosingStockItem> OS_Details { get; set; }
    }
    public class ClosingStockItem
    {
        public int id { get; set; }
        public string selectedItems { get; set; }
        public string mu_code { get; set; }
        public string Quantity { get; set; }
    }
}
