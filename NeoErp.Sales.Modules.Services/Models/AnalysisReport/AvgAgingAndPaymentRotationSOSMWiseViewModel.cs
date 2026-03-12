using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.AnalysisReport
{
    public class AvgAgingAndPaymentRotationSOSMWiseModel
    {
        public string PartyTypeCode { get; set; }
        public string PrePartyCode { get; set; }
        public string MasterPartyCode { get; set; }
        public string PartyTypeName { get; set; }
        public string PartyName { get; set; }
        public string Remarks { get; set; }
        public string CreditLimit { get; set; }
        public decimal? Balance { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? PayRotationDays { get; set; }
        public decimal? IsChild { get; set; }

    }
    public class AvgAgingAndPaymentRotationSOSMWiseViewModel
    {
        public List<AvgAgingAndPaymentRotationSOSMWiseModel> AvgAgingAndPaymentRotationSOSMWiseModel { get; set; }
        public decimal total { get; set; }
        public AvgAgingAndPaymentRotationSOSMWiseViewModel()
        {
            AvgAgingAndPaymentRotationSOSMWiseModel = new List<AvgAgingAndPaymentRotationSOSMWiseModel>();
            this.AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
    }
}
