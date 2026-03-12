using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.AnalysisReport
{
    public class DealerSalesRangeModel
    {
        public string DealerSalesRange { get; set; }
        public decimal? Count { get; set; }
        public decimal? DealerCountRatio { get; set; }
        public decimal? TotalSales { get; set; }
        public decimal? SalesRatio { get; set; }
        public decimal? TotalOutStanding { get; set; }
        public decimal? OutStandingRatio { get; set; }
        public decimal? AverageAging { get; set; }
        public decimal? PaymentRotationDay { get; set; }
    }
    public class DealerSalesRangeViewModel
    {
        public List<DealerSalesRangeModel> DealerSalesRangeModel { get; set; }
        public decimal total { get; set; }
        public DealerSalesRangeViewModel()
        {
            DealerSalesRangeModel = new List<DealerSalesRangeModel>();
            this.AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
    }
}
