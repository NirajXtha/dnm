using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.Stock
{
   public class StockValutionViewModel
    {
        public string CatagoryName { get; set; }
        public string CatagoryCode { get; set; }
        public decimal? Amount { get; set; } = 0M;
        public string Method { get; set; }
    }
}
