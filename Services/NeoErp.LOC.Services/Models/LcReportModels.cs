using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.LOC.Services.Models
{
    public class DueInvoiceReportModels
    {
        public string BANK_NAME { get; set; }
        public string LC_NUMBER { get; set; }
        public string INVOICE_DATE { get; set; }
        public string INVOICE_NUMBER { get; set; }
        public string INVOICE_CURRENCY { get; set; }
        public decimal? AMOUNT { get; set; }
        public int? NPR_VALUE { get; set; }
        public int CREDIT_DAYS { get; set; }
        public string DUE_DATE { get; set; }
        public string PARENT_NAME { get; set; }
        public string BRAND_NAME { get; set; }
        public string SUPPLIER_EDESC { get; set; }
        public string CURRENCY_CODE { get; set; }
        //v 
        public string ORDER_NO { get; set; }
        public string ORDER_DATE { get; set; }
    }

    public class PendingLcReportModels
    {
        //public string PINVOICE_NO { get; set; }
        //public DateTime PINVOICE_DATE { get; set; }
        //public int QUANTITY { get; set; }
        //public int AMOUNT { get; set; }
        //public int TOTAL_AMOUNT { get; set; }
        //public string CURRENCY_CODE { get; set; }
        //public string HS_CODE { get; set; }
        //public string BRAND_NAME { get; set; }
        //public string SUPPLIER_EDESC { get; set; }
        //public int LC_TRACK_NO { get; set; }
        //public string SUPPLIER_CODE { get; set; }
        //public string ITEM_CODE { get; set; }
        //public string LC_NUMBER { get; set; }
        //public string OPEN_DATE { get; set; }
        //public string EXPIRY_DATE { get; set; }
        //public string LAST_SHIPMENT_DATE { get; set; }
        //public string BENIFICARY { get; set; }


        public int LC_TRACK_NO { get; set; }
        public string OPEN_DATE { get; set; }
        public string EXPIRY_DATE { get; set; }
        public string LAST_SHIPMENT_DATE { get; set; }
        public string BENIFICARY { get; set; }
        public string CURRENCY_CODE { get; set; }
        public int QUANTITY { get; set; }
        public decimal AMOUNT { get; set; }
        public decimal TOTAL_AMOUNT { get; set; }
        public string SUPPLIER_EDESC { get; set; }

        public string PINVOICE_NO { get; set; }
        public DateTime PINVOICE_DATE { get; set; }


        public string LcWeekNumber { get; set; }
        public string LcNumber { get; set; }
        public string DocumentOrderNumber { get; set; }
        public string CommercialInvoice { get; set; }
        public string ItemName { get; set; }
        public string OrderQunatity { get; set; }
        public string PendingPurchaseOrder { get; set; }
        public string MIT { get; set; }
        public string Due { get; set; }
        public string ATA { get; set; }
        public string AwbRecievedDate { get; set; }



    }

    public class LcStatusReportModels
    {
        public int LC_TRACK_NO { get; set; }
        public string LC_NUMBER { get; set; }
        public string OPEN_DATE { get; set; }
        public string EXPIRY_DATE { get; set; }
        public string STATUS_EDESC { get; set; }
        public string ITEM_CODE { get; set; }
        public decimal? PFI_AMOUNT { get; set; }
        public decimal? TOTAL_QTY { get; set; }
        public string BRAND_NAME { get; set; }
        public DateTime? INVOICE_DATE { get; set; }
        public string INVOICE_NUMBER { get; set; }
        public decimal? INVOICE_AMOUNT { get; set; }
        public int? QUANTITY { get; set; }
        public string INVOICE_CURRENCY { get; set; }
        public int CREDIT_DAYS { get; set; }
        public DateTime? DUE_DATE { get; set; }
        public string PARENT_NAME { get; set; }
        public string ITEM_EDESC { get; set; }
        public string SUPPLIER_EDESC { get; set; }
        public int LOC_CODE { get; set; }
        public string LAST_SHIPMENT_DATE { get; set; }
        public string TERMS_EDESC { get; set; }
        public decimal? TOTAL_RECEIVED_AMOUNT { get; set; }
        public string LC_CURRENCY { get; set; }
        public decimal? LC_AMOUNT { get; set; }
        public decimal? BALANCE_AMOUNT { get; set; }
    }

    public class VehicleMovementReportModels
    {
        public int LC_TRACK_NO { get; set; }
        public string INVOICE_DATE { get; set; }
        public string INVOICE_NO { get; set; }
        public string LC_NUMBER { get; set; }
        public string CONTRACTER_NAME { get; set; }
        public string CONTRACTER_ADDRESS { get; set; }
        public string LOAD_TYPE { get; set; }
        public string CONTRACT_AMOUNT { get; set; }
        public string SHIPPER_NAME { get; set; }
        public string SHIPPER_ADDRESS { get; set; }
        public string CONTRACT_DATE { get; set; }
        public string FROM_LOCATION_EDESC { get; set; }
        public string TO_LOCATION_EDESC { get; set; }
        public string SHIPMENT_TYPE { get; set; }


        public string SRC_ETA { get; set; }
        public string SRC_ETD { get; set; }
        public string SRC_ATA { get; set; }
        public string SRC_ATD { get; set; }
        public string SRC_ETD_DES { get; set; }
        public string DES_ETA { get; set; }
        public string DES_ETD { get; set; }
        public string DES_ATA { get; set; }
        public string DES_ATD { get; set; }
        public string DES_ETD_NEXT_DES { get; set; }

    }

    public class ItemReportModels
    {
        public int LC_TRACK_NO { get; set; }
        public int SNO { get; set; }
        public string ITEM_CODE { get; set; }
        public string MU_CODE { get; set; }
        public int QUANTITY { get; set; }
        public string CURRENCY_CODE { get; set; }
        public decimal AMOUNT { get; set; }
        public string HS_CODE { get; set; }
        public string COUNTRY_OF_ORIGIN { get; set; }
        public string REMARKS { get; set; }
        public string ITEM_EDESC { get; set; }
        public string COUNTRY_EDESC { get; set; }
        public string BRAND_NAME { get; set; }
    }


    public class LcProductWiseReportModels
    {
        public int LOC_CODE { get; set; }
        public string LC_NUMBER { get; set; }
        public string OPEN_DATE { get; set; }
        public string EXPIRY_DATE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string PRE_DESC { get; set; }
        public string ITEM_CODE { get; set; }
        public int QUANTITY { get; set; }
        public string AMOUNT { get; set; }
        public string CURRENCY_CODE { get; set; }
        public string TOTAL_AMOUNT { get; set; }
    }

    public class PendingCommercialInvoiceReportModels
    {
        public string LC_NUMBER { get; set; }
        public string INVOICE_NUMBER { get; set; }
        public string INVOICE_DATE { get; set; }
        public string TOTAL_AMT { get; set; }
        public string CURRENCY_CODE { get; set; }
        public string PTERMS { get; set; }
        public string CREDIT_DAYS { get; set; }
        public string DUE_DATE { get; set; }
        public string BENIFICARY { get; set; }
        public string ISSUING_BANK { get; set; }

        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string TOTAL_QTY { get; set; }
        public string BALANCE_QTY { get; set; }

        public string BALANCE_AMT { get; set; }


        //public string LC_NUMBER { get; set; }
        //public DateTime? OPEN_DATE { get; set; }
        //public DateTime? EXPIRY_DATE { get; set; }
        //public string ITEM_EDESC { get; set; }
        //public string PRE_DESC { get; set; }
        //public string ITEM_CODE { get; set; }
        //public decimal? AMOUNT { get; set; }
    }

    public class POPendingReportModel
    {
        public string LC_NUMBER { get; set; }
        public string ITEM_DESC { get; set; }
        public string TOTAL_ITEM { get; set; }
        public string QUANTITY { get; set; }
        public string PROCESS_1 { get; set; }
        public string PROCESS_2 { get; set; }
        public string PROCESS_3 { get; set; }
        public string PROCESS_4 { get; set; }
        public string CREATED_DATE { get; set; }

    }
    public class PendingCIReportViewModel
    {
        public string LcNumber { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public decimal? TotalQuantity { get; set; }
        public decimal? BalanceQuantity { get; set; }
        public string CurrencyCode { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? BalanceAmount { get; set; }
    }
    public class ExchangeGainLossReportViewModel
    {
        public DateTime? BillDate { get; set; }
        public string CommercialInvoiceAmount { get; set; }
        public decimal? PaymentRate { get; set; }
        public decimal? SalesRate { get; set; }
        public decimal? ExchangeGainLoss { get; set; }

    }
    public class MITReportViewModel
    {
        public DateTime ORDER_DATE { get; set; }
        public string ORDER_NO { get; set; }
        public int? LC_TRACK_NO { get; set; }
        public string WEEK_LC_NO { get; set; }
        public string DO_NO { get; set; }
        public string CI_NO { get; set; }
        public string ITEM_CODE { get; set; }

        public string ITEM_EDESC { get; set; }

        public decimal? ORDER_QTY { get; set; }
        public decimal? IN_QUANTITY { get; set; }
        public decimal? PENDING_PO_QTY { get; set; }
        public decimal? MIT { get; set; }
        public decimal? AT_PORT { get; set; }
        public decimal? LC_WEEK { get; set; }

        public decimal? CURRENT_WEEK { get; set; }
        public decimal? DUE { get; set; }
        //public decimal? ATA { get; set; }
        //public decimal? AWB_DATE { get; set; }
        //public DateTime? BillDate { get; set; }
        //public string CommercialInvoiceAmount { get; set; }
        //public decimal? PaymentRate { get; set; }
        //public decimal? SalesRate { get; set; }
        //public decimal? ExchangeGainLoss { get; set; }

    }
    public class ExgGainLossReportVModel
    {
        public DateTime? INVOICE_DATE { get; set; }
        public string INVOICE_NUMBER { get; set; }

        public string LC_NUMBER { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public decimal? AMT { get; set; }
        public decimal? QUANTITY { get; set; }
        public string INVOICE_CURRENCY { get; set; }
        public decimal? EXCHANGE_RATE { get; set; }
        public decimal? PAYMENT_EXCHANGE_RATE { get; set; }
        public decimal? SALES_EXG_RATE { get; set; }
        public decimal? SELLING_EXCHANGE_RATE { get; set; }
        public decimal? EXNG_GAIN_LOSS { get; set; }
        public string PAYMENT_DATE { get; set; }

    }

    public class OptimizedPendingLcModel
    {
        public string LcWeekNumber { get; set; }
        public string WeekNumber { get; set; }
        public string LcNumber { get; set; }
        public string DocumentOrderNumber { get; set; }
        public string CommercialInvoice { get; set; }
        public string ItemName { get; set; }
        public decimal? OrderQunatity { get; set; }
        public decimal? PendingPurchaseOrder { get; set; }
        public string MIT { get; set; }
        public decimal? Due { get; set; }
        public string ATPORT { get; set; }
        public string ATA { get; set; }
        public string AwbRecievedDate { get; set; }
    }
    public class OptimizedPendingLcViewModel
    {
        public List<OptimizedPendingLcModel> OptimizedPendingLcModel { get; set; }
        public decimal total { get; set; }
        public OptimizedPendingLcViewModel()
        {
            OptimizedPendingLcModel = new List<OptimizedPendingLcModel>();
            this.AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
    }


}