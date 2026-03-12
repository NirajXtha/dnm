using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model
{
   public class DistributorTragetChartModel
    {
        public string ItemCode { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public decimal? TargetAmount { get; set; }
        public decimal? ActualAmount { get; set; }
        public string NepaliMonth { get; set; }
        public decimal? QUANTITY { get; set; }
        public decimal? ACHIVE_QTY { get; set; }

    }

    public class DashBoardWidgets
    {
        public decimal? Valueamout { get; set; }
        public string Title { get; set; }
    }
    public class LedgerAmount
    {
        public decimal? DRAmount { get; set; }
        public decimal? CRAmount { get; set; }
    }
}
