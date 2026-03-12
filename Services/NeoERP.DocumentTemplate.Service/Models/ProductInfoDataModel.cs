using System;
using System.Collections.Generic;

namespace NeoERP.DocumentTemplate.Service.Models
{
    public class ProductInfoDataModel
    {
        public ProductBasicInfo ProductInfo { get; set; }
        public List<StockLocationModel> StockByLocation { get; set; }
        public List<SalesRateHistoryModel> SalesHistory { get; set; }
    }

    public class ProductBasicInfo
    {
        public string ProductName { get; set; }
        public string Category { get; set; }
        public string Unit { get; set; }
        public decimal? MinLevel { get; set; }
        public decimal? MaxLevel { get; set; }
        public decimal? ReorderLevel { get; set; }
        public string ProductCode { get; set; }
        public int? LeadTime { get; set; }
        public string SerialFlag { get; set; }
        public string BatchSerialFlag { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string ServiceItemFlag { get; set; }
        public string AltMuCode { get; set; }
        public string HsCode { get; set; }
    }

    public class StockLocationModel
    {
        public string LocationName { get; set; }
        public decimal Stock { get; set; }
    }

    public class SalesRateHistoryModel
    {
        public string CustomerCode { get; set; }
        public decimal SalesPrice { get; set; }
        public DateTime SalesDate { get; set; }
        public string CustomerName { get; set; }
    }
}
