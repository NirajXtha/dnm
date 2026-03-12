using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models
{
    public class FreeQuantityItem
    {
        public string item_code { get; set; }
        public decimal qty { get; set; }
        public string unit { get; set; }
        public decimal free_qty { get; set; }
        public string free_unit { get; set; }
        public int main_serial { get; set; }  // Serial number from IP_ITEM_UNIT_SETUP
        public int free_serial { get; set; }  // Serial number from IP_ITEM_UNIT_SETUP
    }
}
