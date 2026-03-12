using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services
{
    public class MonthwiseCustProdNetSalesModel
    {
        // Basic product/customer information
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string EMPLOYEE_EDESC { get; set; }
        public string EMPLOYEE_CODE { get; set; }
        public string MASTER_CUSTOMER_CODE { get; set; }
        public string PRE_CUSTOMER_CODE { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string INDEX_MU_CODE { get; set; }
        public string MASTER_EMPLOYEE_CODE { get; set; }
        public string PRE_EMPLOYEE_CODE { get; set; }
        public string REGION_EDESC { get; set; }

        // Customer information

        public int ROWLEV { get; set; } //NET_QTY
        public double? S_QTY { get; set; }
        public double? S_NET_QTY { get; set; }
        public double? S_VALUE { get; set; }
        public double? B_QTY { get; set; }
        public double? B_NET_QTY { get; set; }
        public double? B_VALUE { get; set; }
        public double? A_QTY { get; set; }
        public double? A_NET_QTY { get; set; }
        public double? A_VALUE { get; set; }
        public double? K_QTY { get; set; }
        public double? K_NET_QTY { get; set; }
        public double? K_VALUE { get; set; }
        public double? M_QTY { get; set; }
        public double? M_NET_QTY { get; set; }
        public double? M_VALUE { get; set; }
        public double? P_QTY { get; set; }
        public double? P_NET_QTY { get; set; }
        public double? P_VALUE { get; set; }
        public double? Mg_QTY { get; set; }
        public double? Mg_NET_QTY { get; set; }
        public double? Mg_VALUE { get; set; }
        public double? F_QTY { get; set; }
        public double? F_NET_QTY { get; set; }
        public double? F_VALUE { get; set; }
        public double? C_QTY { get; set; }
        public double? C_NET_QTY { get; set; }
        public double? C_VALUE { get; set; }
        public double? Bh_QTY { get; set; }
        public double? Bh_NET_QTY { get; set; }
        public double? Bh_VALUE { get; set; }
        public double? J_QTY { get; set; }
        public double? J_NET_QTY { get; set; }
        public double? J_VALUE { get; set; }
        public double? Aa_QTY { get; set; }
        public double? Aa_NET_QTY { get; set; }
        public double? Aa_VALUE { get; set; }
        public string CUS_CODE { get; set; }
        // Totals
        public double? TOTAL_QTY { get; set; }
        public double? TOTAL_VALUE { get; set; }
        public string SALES_TYPE_CODE { get; set; }

    }



    public class MonthwiseCustProdNetViewModel
    {
        public MonthwiseCustProdNetViewModel()
        {
            MonthwiseNetSales = new List<MonthwiseCustProdNetSalesModel>();
            AggregationResult = new Dictionary<string, AggregationModel>();
        }

        public List<MonthwiseCustProdNetSalesModel> MonthwiseNetSales { get; set; }

        public Dictionary<string, AggregationModel> AggregationResult { get; set; }

        public int Total { get; set; }
    }



    public class CustomerWiseSales
    {
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string MASTER_CUSTOMER_CODE { get; set; }
        public string PRE_CUSTOMER_CODE { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public Double SALES_QTY { get; set; }
        public Double SALES_VALUE { get; set; }
        public Double SALES_RET_QTY { get; set; }
        public Double SALES_RET_VALUE { get; set; }
        public Double NET_SALES_QTY { get; set; }
        public Double NET_SALES_VALUE { get; set; }

    }

    public class Customer
    {
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string GROUP_FLAG { get; set; }
        public string MASTER_CUSTOMER_CODE { get; set; }
    }

    public class Item
    {
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }

    }

    public class EmployeeBrandwiseSales
    {
        // Common properties from your initial SELECT statement
        public string COMPANY_CODE { get; set; }
        public string CUS_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string EMPLOYEE_CODE { get; set; }
        public string EMPLOYEE_EDESC { get; set; }

        // Pivoted properties for Quantity (Q) and Value (V)
        // These match the aliases created by your PIVOT query
        public decimal ADH_Col_Q { get; set; }
        public decimal ADH_Col_V { get; set; }
        public decimal Anti_Bubble_Q { get; set; }
        public decimal Anti_Bubble_V { get; set; }
        public decimal Anti_Bubble_Trading_Q { get; set; }
        public decimal Anti_Bubble_Trading_V { get; set; }
        public decimal Aquamax_Q { get; set; }
        public decimal Aquamax_V { get; set; }
        public decimal Auxiliry_Packing_Materials_Q { get; set; }
        public decimal Auxiliry_Packing_Materials_V { get; set; }
        public decimal Bopp_Tape_Q { get; set; }
        public decimal Bopp_Tape_V { get; set; }
        public decimal DM_Water_Q { get; set; }
        public decimal DM_Water_V { get; set; }
        public decimal Doc_Tape_Q { get; set; }
        public decimal Doc_Tape_V { get; set; }
        public decimal Grip_Well_Q { get; set; }
        public decimal Grip_Well_V { get; set; }
        public decimal Heat_Max_Q { get; set; }
        public decimal Heat_Max_V { get; set; }
        public decimal Heat_Max_MFG_Q { get; set; }
        public decimal Heat_Max_MFG_V { get; set; }
        public decimal Heat_Max_Trading_Q { get; set; }
        public decimal Heat_Max_Trading_V { get; set; }
        public decimal Karpenter_SH_Q { get; set; }
        public decimal Karpenter_SH_V { get; set; }
        public decimal Masking_Tape_Q { get; set; }
        public decimal Masking_Tape_V { get; set; }
        public decimal Membrane_Q { get; set; }
        public decimal Membrane_V { get; set; }
        public decimal Mixed_Bond_Q { get; set; }
        public decimal Mixed_Bond_V { get; set; }
        public decimal PVC_Electric_Tape_Q { get; set; }
        public decimal PVC_Electric_Tape_V { get; set; }
        public decimal Packaging_Materials_Q { get; set; }
        public decimal Packaging_Materials_V { get; set; }
        public decimal Plastic_Jar_Galen_Cutting_Q { get; set; }
        public decimal Plastic_Jar_Galen_Cutting_V { get; set; }
        public decimal Plastick_Jar_Fresh_Q { get; set; }
        public decimal Plastick_Jar_Fresh_V { get; set; }
        public decimal Plate_Gum_Q { get; set; }
        public decimal Plate_Gum_V { get; set; }
        public decimal Rathi_Col_Q { get; set; }
        public decimal Rathi_Col_V { get; set; }
        public decimal Rathi_Col_Speed_Bond_Q { get; set; }
        public decimal Rathi_Col_Speed_Bond_V { get; set; }
        public decimal Rathi_Gum_Q { get; set; }
        public decimal Rathi_Gum_V { get; set; }
        public decimal Raw_Material_Q { get; set; }
        public decimal Raw_Material_V { get; set; }
        public decimal Raw_Materials_Aura_Q { get; set; }
        public decimal Raw_Materials_Aura_V { get; set; }
        public decimal Scrap_Items_Q { get; set; }
        public decimal Scrap_Items_V { get; set; }
        public decimal Side_Pesting_Gum_Q { get; set; }
        public decimal Side_Pesting_Gum_V { get; set; }
        public decimal Speed_Bond_Q { get; set; }
        public decimal Speed_Bond_V { get; set; }
        public decimal Speed_Bond_Trading_Q { get; set; }
        public decimal Speed_Bond_Trading_V { get; set; }
        public decimal Sticker_Gum_Q { get; set; }
        public decimal Sticker_Gum_V { get; set; }
        public decimal Super_Glue_Footwear_Q { get; set; }
        public decimal Super_Glue_Footwear_V { get; set; }
        public decimal Super_Glue_Industry_Q { get; set; }
        public decimal Super_Glue_Industry_V { get; set; }
        public decimal Super_Rathi_Col_Q { get; set; }
        public decimal Super_Rathi_Col_V { get; set; }
        public decimal Super_Stick_Q { get; set; }
        public decimal Super_Stick_V { get; set; }
        public decimal Supreme_Col_Q { get; set; }
        public decimal Supreme_Col_V { get; set; }
        public decimal Vehicles_Q { get; set; }
        public decimal Vehicles_V { get; set; }
        public decimal NA_Q { get; set; }
        public decimal NA_V { get; set; }
    }
}
