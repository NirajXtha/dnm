using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.NetSalesReports
{
    public   class DatewiseNetSalesModels
    {
        public DateTime ITEM_EDESC { get; set; }           
        public string INDEX_MU_CODE { get; set; }                
        public double? SALES_QTY { get; set; }             
        public double? SALES_VALUE { get; set; }            
        public double? SALES_RET_QTY { get; set; }        
        public double? SALES_RET_VALUE { get; set; }      
        public double? DEBIT_VALUE { get; set; }            
        public double? CREDIT_VALUE { get; set; }           
        public double? FREE_QTY { get; set; }               
        public double? NET_SALES_QTY { get; set; }           
        public double? NET_SALES_VALUE { get; set; }
    }
    public class DatewiseNetSalesModelsViewModel
    {
        public DatewiseNetSalesModelsViewModel()
        {
            DateWiseNetSales = new List<DatewiseNetSalesModels>();
            AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public List<DatewiseNetSalesModels> DateWiseNetSales { get; set; }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
        public int Total { get; set; }
    }
}
