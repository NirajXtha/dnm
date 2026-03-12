using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.AnalysisReport
{
    public class QuantityBalancePaymentRotationDayModel
    {
        public string account_head { get; set; }

        public int? TreeLevel { get; set; }
        public string PartyTypeCode { get; set; }
        public string PrePartyCode { get; set; }
        public string MasterPartyCode { get; set; }
        public string PartyTypeName { get; set; }
        public string PartyName { get; set; }
        public string Remarks { get; set; }
        public decimal? CreditLimit { get; set; }
        public decimal? Balance { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? PayRotationDays { get; set; }
        public decimal? IsChild { get; set; }
        public decimal? BalanceTotal { get; set; }
        public decimal? QuantityTotal { get; set; }
        public decimal? PayRotationDaysTotal { get; set; }
        public string parentId { get; set; }
        public int? ParentIdInt { get; set; }
        public int? Id { get; set; }
    }
    public class QuantityBalancePaymentRotationDayViewModel
    {
        public List<QuantityBalancePaymentRotationDayModel> QuantityBalancePaymentRotationDayModel { get; set; }
        public decimal total { get; set; }
        public QuantityBalancePaymentRotationDayViewModel()
        {
            QuantityBalancePaymentRotationDayModel = new List<QuantityBalancePaymentRotationDayModel>();
            this.AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
        public decimal? bTotal { get; set; }
        public decimal? qTotal { get; set; }
        public decimal? prdTotal { get; set; }
    }
}
