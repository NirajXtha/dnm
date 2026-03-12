using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.AnalysisReport
{
    public class AreaWiseSalesAndCollectionModel
    {
        public string AreaCode { get; set; }
        public string AreaName { get; set; }
        public string CompanyCode { get; set; }
        public decimal? DailySales { get; set; }
        public decimal? DailyCollection { get; set; }
        public decimal? MonthlySales { get; set; }
        public decimal? MonthlyCollection { get; set; }
        public decimal? YearlySales { get; set; }
        public decimal? YearlyCollection { get; set; }
    }
    public class AreaWiseSalesAndCollectionViewModel
    {
        public List<AreaWiseSalesAndCollectionModel> AreaWiseSalesAndCollectionModel { get; set; }
        public decimal total { get; set; }
        public AreaWiseSalesAndCollectionViewModel()
        {
            AreaWiseSalesAndCollectionModel = new List<AreaWiseSalesAndCollectionModel>();
            this.AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }

    }
}
