using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.NetSalesReports
{
    public class BranchItemStockModel
    {
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string MASTER_ITEM_CODE { get; set; }
        public string PRE_ITEM_CODE { get; set; }
        public string INDEX_MU_CODE { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public double? BRANCH_QTY { get; set; }
        public double? BRANCH_VALUE { get; set; }
        public double? BRANCH_FREE { get; set; }
        public string MASTER_CUSTOMER_CODE { get; set; }
        public string PRE_CUSTOMER_CODE { get; set; }
        public int LEVEL { get; set; } // Add this if you're using for hierarchy
        public List<BranchItemStockModel> CHILDREN { get; set; }
        public string CUS_CODE { get; set; }

    }
}
