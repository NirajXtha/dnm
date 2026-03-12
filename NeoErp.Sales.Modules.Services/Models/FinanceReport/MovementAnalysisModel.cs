using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.FinanceReport
{
    public class MovementAnalysisModel
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string VoucherNumber { get; set; }
        public string ManaulNumber { get; set; }
        public string VoucherDate { get; set; }
        public string CreditLimit { get; set; }
        public string CreditDays { get; set; }
        public string DueDays { get; set; }
        public string CompanyCode { get; set; }
        public decimal? PendingAmount { get; set; }
        public decimal? SalesAmount { get; set; }
        public decimal? RecAmount { get; set; }
        public decimal? Balance { get; set; }
        public string FreeLimit { get; set; }

    }
    public class MovementAnalysisViewModel
    {
        public List<MovementAnalysisModel> MovementAnalysisModel { get; set; }
        public decimal total { get; set; }
        public MovementAnalysisViewModel()
        {
            MovementAnalysisModel = new List<MovementAnalysisModel>();
            this.AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
    }
}
