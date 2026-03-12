using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.AnalysisReport
{
    public class BrandWiseGrossProfitModel
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string BrandName { get; set; }
        public string GroupSkuFlag { get; set; }
        public string IndexMuCode { get; set; }
        public string MasterItemCode { get; set; }
        public int LevelRow { get; set; }
        public decimal? AverageSalesRate { get; set; }
        public decimal? AveragePurchaseRate { get; set; }
        public decimal? SalesQuantity { get; set; }
        public decimal? PurchaseQuantity { get; set; }
        public decimal? TotalPurchaseAmount { get; set; }
        public decimal? TotalSalesAmount { get; set; }
        public decimal? ClosingValue { get; set; }
        public decimal? ItemWiseProfit { get; set; }
        public decimal? NewItemWiseProfit { get; set; }
        public decimal? ProfitPer { get; set; }
    }
    public class BrandWiseGrossProfitViewModel
    {
        public List<BrandWiseGrossProfitModel> BrandWiseGrossProfitModel { get; set; }
        public decimal total { get; set; }
        public BrandWiseGrossProfitViewModel()
        {
            BrandWiseGrossProfitModel = new List<BrandWiseGrossProfitModel>();
            this.AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
    }
}
