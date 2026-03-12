using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.AnalysisReport
{
    public class DebtorAgingModel
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public decimal? ZeroToFifteen { get; set; }
        public decimal? SisteenToThirty { get; set; }
        public decimal? ThirtyOneToFourtyFive { get; set; }
        public decimal? FourtySixToSixty { get; set; }
        public decimal? SixtyOneToSeventyFive { get; set; }
        public decimal? SeventySixToNinty { get; set; }
        public decimal? NintyPlus { get; set; }
    }
    public class DebtorAgingViewModel
    {
        public List<DebtorAgingModel> DebtorAgingModel { get; set; }
        public decimal total { get; set; }
        public DebtorAgingViewModel()
        {
            DebtorAgingModel = new List<DebtorAgingModel>();
            this.AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
    }
}
