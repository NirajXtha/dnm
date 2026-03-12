using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models
{
    public class GroupwiseStockSummary
    {
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string PRODUCT_CODE { get; set; }
        public string INDEX_MU_CODE { get; set; }
        public string MASTER_ITEM_CODE { get; set; }
        public string PRE_ITEM_CODE { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string SERVICE_ITEM_FLAG { get; set; }
        public int? ROWLEV { get; set; }

        public double? OPEN_QTY { get; set; }
        public double? OPEN_AMT { get; set; }

        public double? PURCHASE_QTY { get; set; }
        public double? PURCHASE_AMT { get; set; }
        public double? PURCHASE_RET_QTY { get; set; }
        public double? PURCHASE_RET_AMT { get; set; }

        public double? SALES_QTY { get; set; }
        public double? SALES_AMT { get; set; }
        public double? SALES_RET_QTY { get; set; }
        public double? SALES_RET_AMT { get; set; }
        public double? SALES_NET_AMT { get; set; }

        public double? SI_AMT { get; set; }
        public double? SR_AMT { get; set; }
        public double? SRE_AMT { get; set; }
        public double? DN_AMT { get; set; }
        public double? CN_AMT { get; set; }

        public double? STK_REC_QTY { get; set; }
        public double? STK_REC_AMT { get; set; }

        public double? STK_TRANS_QTY { get; set; }
        public double? STK_TRANS_AMT { get; set; }
        public double? ST_TRNS_QTY { get; set; }
        public double? ST_TRNS_AMT { get; set; }

        public double? GOODS_ISS_QTY { get; set; }
        public double? GOODS_ISS_AMT { get; set; }

        public double? CIR_QTY { get; set; }
        public double? CIR_AMT { get; set; }

        public double? PRO_ISS_QTY { get; set; }
        public double? PRO_ISS_AMT { get; set; }

        public double? PRO_REC_QTY { get; set; }
        public double? PRO_REC_AMT { get; set; }

        public double? EXP_QTY { get; set; }
        public double? EXP_AMT { get; set; }

        public double? DA_EXP_QTY { get; set; }
        public double? DA_EXP_AMT { get; set; }

        public double? SAM_QTY { get; set; }
        public double? SAM_AMT { get; set; }

        public double? QC_QTY { get; set; }
        public double? QC_AMT { get; set; }

        public double? CLOSING_QTY { get; set; }
        public double? CLOSING_AMT { get; set; }
        public double? PUR_SERIAL_NO { get; set; }
        public string PUR_ITEM_CODE { get; set; }
        public double? PUR_QUANTITY { get; set; }
        public double? PUR_CALC_QUANTITY { get; set; }
    }
    public class GroupwiseStockSummaryViewModel
    {
        public GroupwiseStockSummaryViewModel()
        {
            GroupwiseStock = new List<GroupwiseStockSummary>();
            AggregationResult = new Dictionary<string, AggregationModel>();
        }

        public List<GroupwiseStockSummary> GroupwiseStock { get; set; }

        public Dictionary<string, AggregationModel> AggregationResult { get; set; }

        public int Total { get; set; }
    }


    public class nepaliCalenderdates
    {
        public DateTime STARTDATE { get; set; }
        public DateTime ENDDATE { get; set; }
        public string RANGENAME { get; set; }
    }

    public class Item
    {
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
    }

    public class FY
    {
        public string FISCAL { get; set; }
    }

}
