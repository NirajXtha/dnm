using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.AnalysisReport
{
    public class PartyWiseSalesModel
    {
        public string CustomerGroup { get; set; }
        public string CustomerName { get; set; }
        public decimal? TotalSalesAmount { get; set; }
        public decimal? GroupSalesAmount { get; set; }
    }
    public class PartyWiseSalesViewModel
    {
        public List<PartyWiseSalesModel> PartyWiseSalesModel { get; set; }
        public decimal total { get; set; }
        public PartyWiseSalesViewModel()
        {
            PartyWiseSalesModel = new List<PartyWiseSalesModel>();
            this.AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
    }
}
