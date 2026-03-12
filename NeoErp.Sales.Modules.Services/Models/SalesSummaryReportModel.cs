using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models
{
    public class SalesSummaryReportModel
    {
        public string ITEM_EDESC { get; set; }

        public string UNIT { get; set; }

        public string ITEM_CODE { get; set; }

        public decimal? QUANTITY { get; set; }

        public decimal? LANDED_COST { get; set; }

        public decimal? SALES_AMOUNT { get; set; }

        public double? RATE { get; set; }

        public decimal? UNIT_COST { get; set; }

        public decimal? GROSS_PROFIT { get; set; }

        public decimal? GROSS_PROFIT_PERCENTAGE { get; set; }

    }
    public class ProductLevelModel
    {
        public decimal? QUANTITY { get; set; }
        public string PRODUCT { get; set; }
    }

    public class PartyWiseGPAnalysisViewModel
    {
        public List<SalesSummaryReportModel> PartyWiseGPAnalysisModel { get; set; }
        public decimal total { get; set; }
        public PartyWiseGPAnalysisViewModel()
        {
            PartyWiseGPAnalysisModel = new List<SalesSummaryReportModel>();
            this.AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
    }



    public class SalesAnalysisModel
    {
        public string DURATION { get; set; }
        public DateTime SALES_DATE { get; set; }
        public int ITEM_GROUP_CODE { get; set; }
        public string ITEM_GROUP_NAME { get; set; }
        public string ITEM_CODE { get; set; }      
        public decimal QUANTITY { get; set; }
        public decimal AMOUNT { get; set; }
        public decimal DISCOUNT { get; set; }
        public decimal REALIZATION { get; set; }

    }

    public class HighestSellingModel
    {
        public string ITEM_CODE { get; set; }
        public int? QTY { get; set; }
        public string ITEM_EDESC { get; set; }
    }


    public class TopEmployeeListModel
    {
        public string EMPLOYEE_MTD { get; set; }
        public decimal AMOUNT_MTD { get; set; }
        public decimal TARGET_MTD { get; set; }
        public string EMPLOYEE_YTD { get; set; }
        public decimal AMOUNT_YTD { get; set; }
        public decimal TARGET_YTD { get; set; }        
    }

    public class TopDealerListModel
    {
        public string DEALER_MTD { get; set; }
        public decimal QUANTITY_MTD { get; set; }
        public decimal SALES_MTD { get; set; }
        public decimal COLLECTION_MTD { get; set; }
        public decimal BILLCOUNT_MTD { get; set; }
        public decimal CANCLE_ORDER_MTD { get; set; }
        public decimal DUE_BALANCE_MTD { get; set; }
        public decimal DUE_DAYS_MTD { get; set; }
        public decimal CREDIT_LIMIT_MTD { get; set; }
        public decimal EXCEED_MTD { get; set; }

        public string DEALER_YTD { get; set; }
        public decimal QUANTITY_YTD { get; set; }
        public decimal SALES_YTD { get; set; }
        public decimal COLLECTION_YTD { get; set; }
        public decimal BILLCOUNT_YTD { get; set; }
        public decimal CANCLE_ORDER_YTD { get; set; }
        public decimal DUE_BALANCE_YTD { get; set; }
        public decimal DUE_DAYS_YTD { get; set; }
        public decimal CREDIT_LIMIT_YTD { get; set; }
        public decimal EXCEED_YTD { get; set; }
    }

    public class SalesAchieveModel
    {
        public decimal? SALES_ACHIEVE { get; set; }
        public decimal? SALES_UNACHIEVE { get; set; }
        public decimal? TARGET { get; set; }
        public string BRANCH_CODE { get; set; }
        public string BRANCH_EDESC { get; set; }
        public string ITEM_EDESC { get; set; }


        public decimal? SALES_PROJECTED { get; set; }
        public decimal TotalDaysTillDate { get; set; }
        public decimal TotalDaysAll { get; set; }
        public decimal Projected { get; set; }
     
    }
    public class SalesAchieveProjectionModel
    {
        public string BRANCH_EDESC { get; set; }
        public string BRANCH_CODE { get; set; }
        public string DATA_TYPE { get; set; }
        public decimal? QUANTITY { get; set; }
        public decimal? AMOUNT { get; set; }
        public decimal? PROJECTION { get; set; }
        public decimal? PROJECTION_QUANTITY { get; set; }
        public decimal? TARGET { get; set; }
        public decimal? TARGET_QUANTITY { get; set; }
        public string TYPE { get; set; }
    }

    public class SalesAchieveMonthModel
    {
        public decimal? TARGETAMOUNT { get; set; }
        public decimal TARGETQTY { get; set; }
        public decimal? QUANTITY { get; set; }
        public decimal? GROSSAMOUNT { get; set; }
        public string MONTH { get; set; }
        public decimal? SalesAchived { get; set; }
        public decimal? salesUnachived { get; set; }
        public int order { get; set; }
        public string reportType { get; set; }
        public string MONTHINT { get; set; }
        public string YEAR { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? achivedPercentage { get; set; }

    }

    public class PendingOrder
    {
        public decimal ORDER_PLACED { get; set; } = 0;
        public decimal ORDER_DISPATCHED { get; set; } = 0;
        public decimal PENDING_ORDER { get; set; } = 0;
        public string ABBR_CODE { get; set; }   
        public string BRANCH_CODE { get; set; }
    }

    public class TotalValue
    {
        public decimal Sales { get; set; }
        public decimal Collection { get; set; }
    }

    public class SalesTargetViewModel
    {
        public string Branch_name { get; set; }
        public string Branch_code { get; set; }
        public string Month { get; set; }
        public string MonthInt { get; set; }
        public decimal? TargetQty { get; set; }
        public decimal? TargetAmount { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? GrossAmount { get; set; }
        public string DataType { get; set; }
        public string Year { get; set; }


    }
    public class DashboardSalesReport
    {
        public string ITEM_GROUP_NAME { get; set; }
        public int? YTDQUANTITY { get; set; }
        public int? MTDQUANTITY { get; set; }
        public int? TODAYQUANTITY { get; set; }
        public decimal? YTDAMOUNT { get; set; }
        public decimal? MTDAMOUNT { get; set; }
        public decimal? TODAYAMOUNT { get; set; }
        public int? YTDTARGETQUANTITY { get; set; }
        public int? MTDTARGETQUANTITY { get; set; }
        public int? TODAYTARGETQUANTITY { get; set; }
        public decimal? VARIANCE { get; set; }


    }
}
