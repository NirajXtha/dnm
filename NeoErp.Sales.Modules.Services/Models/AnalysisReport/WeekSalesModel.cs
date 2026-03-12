using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.AnalysisReport
{
    public class WeekSalesModel
    {
        public string Model { get; set; }
        public string Category { get; set; }
        public decimal? TotalPurchareQuantity { get; set; }
        public decimal? TotalSalesQuantity { get; set; }
        public decimal? ClosingStockQuantity { get; set; }
        public decimal? One { get; set; }
        public decimal? Two { get; set; }
        public decimal? Three { get; set; }
        public decimal? Four { get; set; }
        public decimal? Five { get; set; }
        public decimal? Six { get; set; }
        public decimal? Seven { get; set; }
        public decimal? Eight { get; set; }
        public decimal? Nine { get; set; }
        public decimal? Ten { get; set; }
        public decimal? Eleven { get; set; }
        public decimal? Twelve { get; set; }
        public decimal? Thirteen { get; set; }
        public decimal? Fourteen { get; set; }
        public decimal? Fifteen { get; set; }
        public decimal? Sixteen { get; set; }
        public decimal? Seventeen { get; set; }
        public decimal? Eighteen { get; set; }
        public decimal? Nineteen { get; set; }
        public decimal? Twenty { get; set; }
        public decimal? TwentyOne { get; set; }
        public decimal? TwentyTwo { get; set; }
        public decimal? TwentyThree { get; set; }
        public decimal? TwentyFour { get; set; }
        public decimal? TwentyFive { get; set; }
        public decimal? TwentySix { get; set; }
        public decimal? TwentySeven { get; set; }
        public decimal? TwentyEight { get; set; }
        public decimal? TwentyNine { get; set; }
        public decimal? Thirty { get; set; }
        public decimal? ThirtyOne { get; set; }
        public decimal? ThirtyTwo { get; set; }
        public decimal? ThirtyThree { get; set; }
        public decimal? ThirtyFour { get; set; }
        public decimal? ThirtyFive { get; set; }
        public decimal? ThirtySix { get; set; }
        public decimal? ThirtySeven { get; set; }
        public decimal? ThirtyEight { get; set; }
        public decimal? ThirtyNine { get; set; }
        public decimal? Fourty { get; set; }
        public decimal? FourtyOne { get; set; }
        public decimal? FourtyTwo { get; set; }
        public decimal? FourtyThree { get; set; }
        public decimal? FourtyFour { get; set; }
        public decimal? FourtyFive { get; set; }
        public decimal? FourtySix { get; set; }
        public decimal? FourtySeven { get; set; }
        public decimal? FourtyEight { get; set; }
        public decimal? FourtyNine { get; set; }
        public decimal? Fifty { get; set; }
        public decimal? FiftyOne { get; set; }
        public decimal? FiftyTwo { get; set; }
        public decimal? FiftyThree { get; set; }
        public decimal? AverageWeeklySales { get; set; }
        public decimal? WOS1 { get; set; }
        public decimal? LastFourMonthSales { get; set; }
        public decimal? AverageLastFourMonthSales { get; set; }
        public decimal? WOS2 { get; set; }
        public string Remarks { get; set; }
    }
    public class WeekSalesViewModel
    {
        public List<WeekSalesModel> WeekSalesModel { get; set; }
        public decimal total { get; set; }
        public WeekSalesViewModel()
        {
            WeekSalesModel = new List<WeekSalesModel>();
            this.AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
    }
}
