using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models
{
    public class SalesRegistersDetail
    {
        //public DateTime? SalesDate { get; set; }
        public string SalesDate { get; set; }
        public string Miti { get; set; }
        public string InvoiceNumber { get; set; }
        public string CustomerName { get; set; }
        public string ItemName { get; set; }
        public string LocationName { get; set; }
        public string ManualNo { get; set; }
        public string REMARKS { get; set; }
        public string Dealer { get; set; }
        public string PartyType { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingContactNo { get; set; }
        public string Unit { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalPrice { get; set; }
        public string AREA_EDESC { get; set; }
        public string AGENT_CODE { get; set; }
        public string AGENT_EDESC { get; set; }
        public string BRAND_EDESC { get; set; }
    }
    public class PurchaseReturnRegistersDetail
    {
        public DateTime? RETURNDATE { get; set; }
        public string MITI { get; set; }
        public string INVOICENUMBER { get; set; }
        public string ITEMNAME { get; set; }
        public string LOCATIONNAME { get; set; }
        public string MANUALNO { get; set; }
        public string REMARKS { get; set; }
        public string SUPPLIERNAME { get; set; }
        public string UNIT { get; set; }
        public decimal? QUANTITY { get; set; }
        public decimal? UNITPRICE { get; set; }
        public decimal? TOTALPRICE { get; set; }
    }

    public class SalesRegisterViewModel//PurchaseReturnRegistersDetail
    {
        public List<SalesRegistersDetail> SalesRegisters { get; set; }
        public int total { get; set; }
        public SalesRegisterViewModel()
        {
            SalesRegisters = new List<SalesRegistersDetail>();
            AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
    }
    public class PurchaseReturnRegisterViewModel//PurchaseReturnRegisterViewModel
    {
        public List<PurchaseReturnRegistersDetail> SalesRegisters { get; set; }
        public int total { get; set; }
        public PurchaseReturnRegisterViewModel()
        {
            SalesRegisters = new List<PurchaseReturnRegistersDetail>();
            AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
    }

    public class PurchaseRegisterPivot
    {
        public List<PurchaseRegisterMoreDetail> PurchaseRegisters { get; set; }
        public int total { get; set; }
        public PurchaseRegisterPivot()
        {
            PurchaseRegisters = new List<PurchaseRegisterMoreDetail>();
            AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
    }

    public class PurchaseRegisterMoreDetail
    {
        public String INVOICE_NO { get; set; }
        public DateTime? INVOICE_DATE { get; set; }
        public string MANUAL_NO { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_NAME { get; set; }
        public string SUPPLIER_INV_NO { get; set; }
        public string SUPPLIER_MRR_NO { get; set; }
        public DateTime? SUPPLIER_INV_DATE { get; set; }
        public string PP_NO { get; set; }
        public DateTime? PP_DATE { get; set; }
        public string REMARKS { get; set; }
        public DateTime? DUE_DATE { get; set; }
        public string CURRENCY_CODE { get; set; }
        public decimal? EXCHANGE_RATE { get; set; }
        public string LOCATION_EDESC { get; set; }
        public string ITEM_EDESC { get; set; }
        public decimal? QUANTITY { get; set; }
        public decimal? UNIT_PRICE { get; set; }
        public decimal? TOTAL_PRICE { get; set; }
        public string FORM_EDESC { get; set; }

        public string ITEM_GROUP_EDESC { get; set; }
        public string ITEM_SUBGROUP_EDESC { get; set; }
        public string CATEGORY_EDESC { get; set; }
        public string COMPANYEDESC { get; set; }
        public string BranchEdesc { get; set; }
    }
}
