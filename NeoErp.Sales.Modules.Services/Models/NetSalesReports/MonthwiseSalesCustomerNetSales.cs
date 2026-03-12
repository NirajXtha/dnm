using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.NetSalesReports
{
    public  class MonthwiseSalesCustomerNetSales
    {

        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string MASTER_ITEM_CODE { get; set; }
        public string PRE_ITEM_CODE { get; set; }
        public string GROUP_SKU_FLAG { get; set; }

        public double? S_QTY { get; set; }
        public double? S_VALUE { get; set; }
        public double? B_QTY { get; set; }
        public double? B_VALUE { get; set; }
        public double? A_QTY { get; set; }
        public double? A_VALUE { get; set; }
        public double? K_QTY { get; set; }
        public double? K_VALUE { get; set; }
        public double? M_QTY { get; set; }
        public double? M_VALUE { get; set; }
        public double? P_QTY { get; set; }
        public double? P_VALUE { get; set; }
        public double? Mg_QTY { get; set; }
        public double? Mg_VALUE { get; set; }
        public double? F_QTY { get; set; }
        public double? F_VALUE { get; set; }
        public double? C_QTY { get; set; }
        public double? C_VALUE { get; set; }
        public double? Bh_QTY { get; set; }
        public double? Bh_VALUE { get; set; }
        public double? J_QTY { get; set; }
        public double? J_VALUE { get; set; }
        public double? Aa_QTY { get; set; }
        public double? Aa_VALUE { get; set; }

    }

    public class MonthwiseCustomerSalesCollection
    {

        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string MASTER_CUSTOMER_CODE { get; set; }
        public string PRE_CUSTOMER_CODE { get; set; }
        public string GROUP_SKU_FLAG { get; set; }

        public double? S_COLLECTION { get; set; }
        public double? S_VALUE { get; set; }
        public double? B_COLLECTION { get; set; }
        public double? B_VALUE { get; set; }
        public double? A_COLLECTION { get; set; }
        public double? A_VALUE { get; set; }
        public double? K_COLLECTION { get; set; }
        public double? K_VALUE { get; set; }
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
        public double? J_COLLECTION { get; set; }
        public double? J_VALUE { get; set; }
        public double? Aa_COLLECTION { get; set; }
        public double? Aa_VALUE { get; set; }


    }
    public class MonthwiseCustomerSalesRequest
    {
        public string company_name { get; set; }
        public string fromADdate { get; set; }
        public string toADdate { get; set; }
        public string fromBSdate { get; set; }
        public string toBSdate { get; set; }
        public object branches { get; set; } // define more specific type if possible
    }

}
