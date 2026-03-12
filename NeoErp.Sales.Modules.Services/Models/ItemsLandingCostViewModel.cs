using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models
{
    public class ItemsLandingCostViewModel
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Unit { get; set; }
        public Decimal? Quantity { get; set; }
        public Decimal? RatePerUnit { get; set; }
        public Decimal? GrossAmount { get; set; }
        public Decimal? NetAmount { get; set; }
        public Decimal? AvgLandingCost { get; set; }
        public Decimal? TotalInCharges { get; set; }
        public IList<PurchaseCharges> charges { get; set; }
        public ItemsLandingCostViewModel()
        {
            charges = new List<PurchaseCharges>();
        }
    }
    public class PurchaseCharges
    {
        public string CHARGE_CODE { get; set; }
        public string CHARGE_TYPE_FLAG { get; set; }
        public decimal? CHARGE_AMOUNT { get; set; }
        public string ITEM_CODE { get; set; }
        public string APPLY_ON { get; set; }
        public string InvoiceNo { get; set; }
    }
    public class PurchaseItemsLandingReportModel
    {
        public List<ItemsLandingCostViewModel> items { get; set; }
        public int total { get; set; }
        public Dictionary<string,AggregationModel> AggregationResult { get; set; }
        public PurchaseItemsLandingReportModel()
        {
            this.AggregationResult = new Dictionary<string, AggregationModel>();
            items = new List<ItemsLandingCostViewModel>();
        }
       
    }
}
