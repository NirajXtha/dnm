using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.Production
{
    public class ProductionRegisterModel
    {
        public string MrrNo { get; set; }
        public DateTime MrrDate { get; set; }
        public string Miti { get; set; }
        public string ManualNo { get; set; }
        public string FromLocation { get; set; }
        public string FromBudget { get; set; }
        public string ToLocation { get; set; }
        //public string ToBudget { get; set; }
        public int SerialNo { get; set; }
        public string Item { get; set; }
        public string ItemGroup { get; set; }
        public string ItemSubGroup { get; set; }
        public string MuCode { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? CalcQuantity { get; set; }
        public decimal? CalcUnitPrice { get; set; }
        public decimal? CalcTotalPrice { get; set; }
        public string Remarks { get; set; }
        public string FormCode { get; set; }
        public string CompanyCode { get; set; }
        public string Company { get; set; }
        public string BranchCode { get; set; }
        public string Branch { get; set; }
        public string CurrencyCode { get; set; }
        public decimal? ExchangeRate { get; set; }
        public string TrackingNo { get; set; }
        public string BatchNo { get; set; }
        public string LotNo { get; set; }
        public string RollQty { get; set; }
        public string DivisionCode { get; set; }
        public string ResourceCode { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string TotalHours { get; set; }
        public string ResourceName { get; set; }
    }
    public class ProductionStockInOutSummaryModel
    {
        public string LOCATION { get; set; }
        public string SKU_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string Unit { get; set; }
        public string ITEM_CODE { get; set; }
        public string INDEX_MU_CODE { get; set; }
        public string MASTER_ITEM_CODE { get; set; }
        public string PRE_ITEM_CODE { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string MY_LEVEL { get; set; }
        public string SERVICEiTEM_FLAG { get; set; }
        public decimal OPE_QTY { get; set; }
        public decimal OPE_VAL { get; set; }
        public decimal IN_QTY { get; set; }
        public decimal IN_VAL { get; set; }
        public decimal OUT_QTY { get; set; }
        public decimal OUT_VAL { get; set; }
        public decimal CLO_QTY { get; set; }
        public decimal CLO_VAL { get; set; }
        public decimal OpeningQuantity { get; set; }
        public decimal InQuantity { get; set; }
        public decimal OutQuantity { get; set; }
        public decimal ClosingQuantity { get; set; }

    }

    public class ProductionStockInOutSummaryViewModel
    {
        public List<ProductionStockInOutSummaryModel> ProductionStockInOutSummaryModel { get; set; }
        public decimal total { get; set; }
        public ProductionStockInOutSummaryViewModel()
        {
            ProductionStockInOutSummaryModel = new List<ProductionStockInOutSummaryModel>();
            this.AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
    }
}
