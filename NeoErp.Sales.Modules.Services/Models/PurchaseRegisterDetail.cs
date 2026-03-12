using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models
{
    public class PurchaseRegisterDetail
    {
        //public DateTime? InvoiceDate { get; set; }
        public string InvoiceDate { get; set; }
        public string Miti { get; set; }
        public string PONo { get; set; }
        public string GRNNo { get; set; }
        public string InvoiceNo { get; set; }
        public string ManualNo { get; set; }
        public string SuppInvNo { get; set; }
        //public DateTime? SuppInvDate { get; set; }
        public string SuppInvDate { get; set; }
        public string SupplierName { get; set; }
        public string StorageLocation { get; set; }
        public string Remarks { get; set; }
        public string ProductName { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; }
        public decimal NetQty { get; set; }
        public decimal? Rate { get; set; }
        public decimal? GrossAmount { get; set; }
    }

    public class PurchaseRegisterViewModel
    {
        public List<PurchaseRegisterDetail> RegisterDetails { get; set; }
        public int total { get; set; }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
        public PurchaseRegisterViewModel()
        {
            this.AggregationResult = new Dictionary<string, AggregationModel>();
            RegisterDetails = new List<PurchaseRegisterDetail>();
        }
    }

    public class PurchasePendingDetailModel
    {
        public string ORDER_NO { get; set; }
        public string ORDER_DATE { get; set; }
        public string MITI { get; set; }
        public string MANUAL_NO { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string ITEM_CODE { get; set; }
        public string MU_CODE { get; set; }
        public decimal? QUANTITY { get; set; }
        public decimal? UNIT_PRICE { get; set; }
        public decimal? TOTAL_PRICE { get; set; }
        public string SUPPLIER_EDESC { get; set; }
        public string ITEM_EDESC { get; set; }
        public string REMARKS { get; set; }
    }
    public class PurchasePendingViewModel
    {
        public List<PurchasePendingDetailModel> SalesRegisters { get; set; }
        public int total { get; set; }
        public PurchasePendingViewModel()
        {
            SalesRegisters = new List<PurchasePendingDetailModel>();
            AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
    }
    public class PurchaseVatRegistrationDetailModel
    {
        public string INVOICE_NO { get; set; }
        public string INVOICE_DATE { get; set; }
        public string MITI { get; set; }
        public string MANUAL_NO { get; set; }
        public string VAT_NO { get; set; }
        //public string ITEM_CODE { get; set; }
        //public string MU_CODE { get; set; }
        public decimal? GROSS_AMOUNT { get; set; }
        public decimal? TAXABLE_AMOUNT { get; set; }
        public decimal? VAT_AMOUNT { get; set; }
        public decimal? TOTAL_AMOUNT { get; set; }
        public string PARTY_NAME { get; set; }
        //public string ITEM_EDESC { get; set; }
        public string REMARKS { get; set; }
    }
    public class PurchaseVatRegistrationViewModel
    {
        public List<PurchaseVatRegistrationDetailModel> SalesRegisters { get; set; }
        public int total { get; set; }
        public PurchaseVatRegistrationViewModel()
        {
            SalesRegisters = new List<PurchaseVatRegistrationDetailModel>();
            AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
    }
}
