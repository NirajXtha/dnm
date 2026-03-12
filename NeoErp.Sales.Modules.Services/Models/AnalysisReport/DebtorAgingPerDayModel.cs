using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.AnalysisReport
{
    public class DebtorAgingPerDayModel
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public decimal? One { get; set; }
        public decimal? Two { get; set; }
        public decimal? Three { get; set; }
        public decimal? Four { get; set; }
        public decimal? MyProperty { get; set; }
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
        public decimal? TwentyPlus { get; set; }
    }
    public class DebtorAgingPerDayViewModel
    {
        public List<DebtorAgingPerDayModel> DebtorAgingPerDayModel { get; set; }
        public decimal total { get; set; }
        public DebtorAgingPerDayViewModel()
        {
            DebtorAgingPerDayModel = new List<DebtorAgingPerDayModel>();
            this.AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
    }
}
