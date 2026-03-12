using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.AnalysisReport
{
    public class BrandWiseSalesModel
    {
        public string BrandName { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Unit { get; set; }
        public decimal? AverageSalesRate { get; set; }
        public decimal? AverageSalesAmount { get; set; }
        public decimal? Quantity { get; set; }
    }
    public class BrandWiseSalesViewModel
    {
        public List<BrandWiseSalesModel> BrandWiseSalesModel { get; set; }
        public decimal total { get; set; }
        public BrandWiseSalesViewModel()
        {
            BrandWiseSalesModel = new List<BrandWiseSalesModel>();
            this.AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
    }
}
