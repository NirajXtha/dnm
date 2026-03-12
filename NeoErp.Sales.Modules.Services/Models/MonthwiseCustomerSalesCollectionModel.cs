using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models
{
    public  class MonthwiseCustomerSalesCollectionModel
    {
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string MASTER_CUSTOMER_CODE { get; set; }
        public string PRE_CUSTOMER_CODE { get; set; }
        public string GROUP_FLAG { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string INDEX_MU_CODE { get; set; }
        // Customer information

        public double? S_VALUE { get; set; }
        public double? S_COLLECTION { get; set; }
        public double? B_VALUE { get; set; }
        public double? B_COLLECTION { get; set; }
        public double? A_VALUE { get; set; }
        public double? A_COLLECTION { get; set; }
        public double? K_VALUE { get; set; }
        public double? K_COLLECTION { get; set; }
        public double? M_COLLECTION { get; set; }
        public double? M_VALUE { get; set; }
        public double? P_COLLECTION { get; set; }
        public double? P_VALUE { get; set; }
        public double? Mg_COLLECTION { get; set; }
        public double? Mg_VALUE { get; set; }
        public double? F_COLLECTION { get; set; }
        public double? F_VALUE { get; set; }
        public double? C_COLLECTION { get; set; }
        public double? C_VALUE { get; set; }
        public double? Bh_COLLECTION { get; set; }
        public double? Bh_VALUE { get; set; }

    }
    public class MonthwiseCustomerSalesCollectionViewModel
    {
        public MonthwiseCustomerSalesCollectionViewModel()
        {
            CustomerSales = new List<MonthwiseCustomerSalesCollectionModel>();
            AggregationResult = new Dictionary<string, AggregationModel>();
        }

        public List<MonthwiseCustomerSalesCollectionModel> CustomerSales { get; set; }

        public Dictionary<string, AggregationModel> AggregationResult { get; set; }

        public int Total { get; set; }
    }
}
