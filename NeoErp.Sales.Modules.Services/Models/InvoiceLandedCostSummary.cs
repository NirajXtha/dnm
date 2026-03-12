using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models
{
   public class InvoiceLandedCostSummary
    {
        public DateTime? InvoiceDate { get; set; }
        public string Miti { get; set; }
        public string InvoiceNo { get; set; }
        public string SupplierName { get; set; }
        public string ItemCode { get; set; }
        public string Unit { get; set; }
        public Decimal? Quantity { get; set; }
        public Decimal? UnitPrice { get; set; }
        public Decimal? TotalPrice { get; set; }
        public string ItemName { get; set; }
        public Decimal? TotalCharges { get; set; }
        public Decimal? AvgLandedCost { get; set; }
        public Decimal? NetAmount { get; set; }
        public IList<PurchaseCharges> charges { get; set; }
        public InvoiceLandedCostSummary()
        {
            charges = new List<PurchaseCharges>();
        }
    }
    public class PurchaseInvoiceLandingReportModel
    {
        public List<InvoiceLandedCostSummary> items { get; set; }
        public int total { get; set; }

        public Dictionary<string,AggregationModel> AggregationResult { get; set; }
        public PurchaseInvoiceLandingReportModel()
        {
            items = new List<InvoiceLandedCostSummary>();
            this.AggregationResult = new Dictionary<string, AggregationModel>();
        }

    }




    public class CustomerWiseProfileAnalysisModel
    {
        public string CustomerName { get; set; }
        public string Customer_Code { get; set; }
        public string ITEM_Code { get; set; }
        public string ITEM_EDESC { get; set; }
        public string Unit { get; set; }
        public decimal? UnitCost { get; set; }
        public int Quantity { get; set; }
        public decimal? Rate { get; set; }
        public decimal SalesAmount { get; set; }
        public decimal Landed_Cost { get; set; }
        public decimal GrossProfit { get; set; }
        public decimal GrossPercent { get; set; }

    }

    public class CustomerWiseProfileAnalysisViewModel
    {
        public List<CustomerWiseProfileAnalysisModel> CustomerWiseProfileAnalysisModel { get; set; }
        public decimal total { get; set; }
        public CustomerWiseProfileAnalysisViewModel()
        {
            CustomerWiseProfileAnalysisModel = new List<CustomerWiseProfileAnalysisModel>();
            this.AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
    }
}
