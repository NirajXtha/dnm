using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.Division
{
   public class DivisionOutStanding
    {
        public string DivisionName { get; set; }
        public decimal? Amount { get; set; }
        public string DivisonId { get; set; }

    }

    public class LcChartModel
    {
        public decimal? Balance { get; set; }
        public decimal? Target { get; set; }
        public decimal? UsedBalance { get; set; }
        public string Acc_Code { get; set; }
        public string Acc_Edesc { get; set; }
    }

    public class LcChartViewModel
    {
        public string Type { get; set; }
        public decimal? Amount { get; set; }
    }
}
