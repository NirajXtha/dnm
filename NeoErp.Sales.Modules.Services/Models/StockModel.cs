using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models
{
   

    public class LocationWiseStockModel
    {
        public string LOCATION { get; set; }
        public string SKU_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string Unit { get; set; }
        public decimal OpeningQuantity { get; set; }
        public decimal InQuantity { get; set; }
        public decimal OutQuantity { get; set; }
        public decimal ClosingQuantity { get; set; }
       
    }

    public class LocationWiseViewStockModel
    {
        public List<LocationWiseStockModel> LocationWiseStockModel { get; set; }
        public decimal total { get; set; }
        public LocationWiseViewStockModel()
        {
            LocationWiseStockModel = new List<LocationWiseStockModel>();
            this.AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
    }


    public class LocationWiseItemStockModel
    {
        public string LOCATION { get; set; }
        public string Mu_code { get; set; }
        public string LocationCode { get; set; }
        public string ITEM_EDESC { get; set; }
        public string Item_Code { get; set; }
        public decimal AvilableStock { get; set; }


    }

    public class LocationVsItemsStockModel
    {
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public string MuCode { get; set; }
        public List<LocationWiseItemStockModel> WareHouseList { get; set; }
        public LocationVsItemsStockModel()
        {
            WareHouseList = new List<LocationWiseItemStockModel>();
        }
    }
    public class LocationWiseItemStockViewModel
    {
        public List<LocationVsItemsStockModel> LocationWiseStockModel { get; set; }
        public decimal total { get; set; }
        public LocationWiseItemStockViewModel()
        {
            LocationWiseStockModel = new List<LocationVsItemsStockModel>();
            this.AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
    }

    public class LocationsHeader
    {
        public string LocationTitle { get; set; }
        public string LocationNo { get; set; }
    }
}
