using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.AnalysisReport
{
    public class ProductRealizationAreaWiseModel
    {
        public string AreaName { get; set; }
        public decimal? OPC { get; set; }
        public decimal? PPC { get; set; }
        public decimal? PSC { get; set; }
    }
    public class ProductRealizationAreaWiseViewModel
    {
        public List<ProductRealizationAreaWiseModel> ProductRealizationAreaWiseModel { get; set; }
        public decimal total { get; set; }
        public ProductRealizationAreaWiseViewModel()
        {
            ProductRealizationAreaWiseModel = new List<ProductRealizationAreaWiseModel>();
            this.AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
    }

    //public class LocationWiseStockModel
    //{
    //    public string LOCATION { get; set; }
    //    public string SKU_CODE { get; set; }
    //    public string ITEM_EDESC { get; set; }
    //    public string Unit { get; set; }
    //    public decimal OpeningQuantity { get; set; }
    //    public decimal InQuantity { get; set; }
    //    public decimal OutQuantity { get; set; }
    //    public decimal ClosingQuantity { get; set; }

    //}

    //public class LocationWiseViewStockModel
    //{
    //    public List<LocationWiseStockModel> LocationWiseStockModel { get; set; }
    //    public decimal total { get; set; }
    //    public LocationWiseViewStockModel()
    //    {
    //        LocationWiseStockModel = new List<LocationWiseStockModel>();
    //        this.AggregationResult = new Dictionary<string, AggregationModel>();
    //    }
    //    public Dictionary<string, AggregationModel> AggregationResult { get; set; }
    //}
}
