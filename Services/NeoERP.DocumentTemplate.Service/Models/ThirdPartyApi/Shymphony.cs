using System;
using System.Collections.Generic;



namespace NeoERP.DocumentTemplate.Service.Models.ThirdPartyApi
{
    public class Shymphony
    {
        public List<Header> Header { get; set; }
        public List<RejectedItem> RejectedItem { get; set; }
        public List<MenuItem> MenuItem { get; set; }
        public List<Discount> Discount { get; set; }
        public List<ServiceCharge> ServiceCharge { get; set; }
        public List<TenderMedia> TenderMedia { get; set; }
        public string Sales_No { get; set; }
        public Checkxml Checkxml { get; set; }
        public BillViewModel BillViewModel { get; set; }
        public static double totalSales = 0.00;
        public static double taxableSalesVat = 0.00;
        public static double vatAmount = 0.00;
        public static double discountAmount = 0.00;
        public static double serviceTaxAmount = 0.00;
        public static string buyer_pan = "";
        public static string buyer_name = "";
        public static string invoice_date = "";
        public static string invoice_number = "";
        public static string form_code = "";
    }

    public class Checkxml
    {
        public string CheckNum { get; set; }
        public string HarmonyId { get; set; }
        public string Timestamp { get; set; }
        public string WsID { get; set; }
        public string PartnerID { get; set; }
        public string Id { get; set; }
    }
    public class Header
    {
        public string LinkId { get; set; }
        public string CheckNumber { get; set; }
        public string RvcID { get; set; }
        public string RvcName { get; set; }
        public string PropertyID { get; set; }
        public string PropertyName { get; set; }

        public string EmployeeFirstName { get; set; }
        public string CheckSubtotal { get; set; }
        public string CheckDiscount { get; set; }
        public string CheckTax { get; set; }
        public string AutoServiceChargeDescriptor { get; set; }
        public string CheckAutoServiceCharge { get; set; }
        public string CheckOther { get; set; }
        public string WorkstationID { get; set; }
        public string Referencia { get; set; }
        public string ClientId { get; set; }
        public string TotalDue { get; set; }
        public string AutoServiceChargeActive { get; set; }
        public string PrimaryCheckIdentification { get; set; }

        public string Table { get; set; }
        public string GuestCount { get; set; }
        //new code change to get customer data
        public string CaptureCustomerData { get; set; }
        //public string CustomerDocumentType { get; set; }
        public string CustomerDocumentType { get; set; }
        public string Document { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CaptureCustomerAddress { get; set; }
        public string Address { get; set; }
        public string Number { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string DocumentType { get; set; }
        public string DocumentId { get; set; }
        public string Complement { get; set; }

        //new added
        public string SimphonyPOSVersion { get; set; }
        public string WorkstationName { get; set; }
        public string CashierID { get; set; }
        public string BusinessDate { get; set; }
        public string BusinessDateTime { get; set; }
        public string CurrentDateTime { get; set; }
        public string TableTextAndGroup { get; set; }
        public string OrderTypeName { get; set; }
        public string OrderTypeNumber { get; set; }
        public string AutoSvcTurnedOff { get; set; }
        public string BegunAsFastTransaction { get; set; }
        public string IsFastTransaction { get; set; }
        public string RoundNumber { get; set; }
        public string Guid { get; set; }
        public string CheckGrossTotal { get; set; }
        public string CheckTotalDue { get; set; }
        public string ChargedTipTotal { get; set; }
        public string CheckServiceChargeTotal { get; set; }
        public string TotalDueByGuest { get; set; }
        public string ChangeDue { get; set; }
        public string CheckPartialPaymentTotal { get; set; }
        public string Training { get; set; }
        public string TransEmployeeFirstName { get; set; }
        public string TransEmployeeLastName { get; set; }
        public string EmployeeId { get; set; }
        public string CheckCurrencySymbol { get; set; }
        public string AddedIn { get; set; }
        public string ChkCancelled { get; set; }
        public string ChkSplitXfer { get; set; }
        public string SplitCheckAdded { get; set; }
        public string SplitFromCheckNum { get; set; }
        public string SplitFromRevCtrID { get; set; }
        public string SplitOut { get; set; }
        public string ReopenedFromCheckNum { get; set; }
        public string ReopenedToCheckNum { get; set; }
        public string MergedCheckNotPrinted { get; set; }
        public string CheckWasReOpened { get; set; }
        public string ClosedCheckReprint { get; set; }
        public string ClosedCheckVoid { get; set; }
        public string ClosedChkAdj { get; set; }
        public string CloseStatus { get; set; }
        public string TransVoidEnabled { get; set; }
        public string TransReturnEnabled { get; set; }
        public string ParserTemplatePath { get; set; }
        public string ParserTemplateHash { get; set; }
        public string PayloadHash { get; set; }
        //new added

    }

    public class MenuItem
    {
        public string Quantity { get; set; }
        public string LinkId { get; set; }
        public string ItemWeight { get; set; }
        public string HasAnyDiscount { get; set; }
        public string ComboMealNumber { get; set; }
        public string ComboMealSideNumber { get; set; }
        public string ComboMealSidePrepCost { get; set; }
        public string InclusiveTax { get; set; }
        public string SalesCount { get; set; }
        public string DecimalSalesCount { get; set; }
        public string Total { get; set; }
        public string VoidLink { get; set; }
        public string DetailLink { get; set; }
        public string Name { get; set; }
        public string Name2 { get; set; }
        public string WeighedItem { get; set; }
        public string Void { get; set; }
        public string LineNumVoid { get; set; }
        public string Shared { get; set; }
        public string FIP_LAD_PAYLOAD_CR_UN_MED_COMERCIAL { get; set; }
        public string FIP_LAD_PAYLOAD_CR_TAX_CODES { get; set; }

        public List<ItemTaxList> ItemTaxList { get; set; }


    }

    public class Discount
    {
        public string LinkId { get; set; }
        public string ObjectNumber { get; set; }
        public string Percentage { get; set; }
        public string SalesCount { get; set; }
        public string Total { get; set; }
        public string VoidLink { get; set; }
        public List<MenuItemDetailLinks> MenuItemDetailLinks { get; set; }
        public string Name { get; set; }
        public string Void { get; set; }
        public string ItemDsc { get; set; }
        public string LineNumVoid { get; set; }
    }
    public class MenuItemDetailLinks
    {
        public string DetailedLinkedId { get; set; }


    }

    public class ServiceCharge
    {
        public string LinkId { get; set; }
        public string ObjectNumber { get; set; }
        public string Percentage { get; set; }
        public string SalesCount { get; set; }
        public string Total { get; set; }
        public string VoidLink { get; set; }
        public string DetailLink { get; set; }
        public string Name { get; set; }
        public string Void { get; set; }
        public string LineNumVoid { get; set; }
    }


    public class TenderMedia
    {
        public string LinkId { get; set; }
        public string ObjectNumber { get; set; }
        public string CurrencyAmount { get; set; }
        public string CurrencyID { get; set; }
        public string SalesCount { get; set; }
        public string Total { get; set; }
        public string VoidLink { get; set; }
        public string Name { get; set; }
        public string Void { get; set; }
        public string LineNumVoid { get; set; }
        public string FIP_LAD_PAYLOAD_CR_TMED_CODES { get; set; }
    }

    public class ItemTaxList
    {
        public string TaxId { get; set; }
        public string TaxName { get; set; }
        public string TaxValue { get; set; }
        public string TaxType { get; set; }
        public string FIP_LAD_PAYLOAD_CR_TAX_CODES { get; set; }
    }
    public class BillViewModel
    {
        public string username { get; set; }
        public string password { get; set; }
        public string seller_pan { get; set; }
        public string buyer_pan { get; set; }
        public string fiscal_year { get; set; }
        public string buyer_name { get; set; }
        public string invoice_number { get; set; }
        public string invoice_date { get; set; }
        public double total_sales { get; set; }
        public double taxable_sales_vat { get; set; }
        public double vat { get; set; }
        public double excisable_amount { get; set; }
        public double excise { get; set; }
        public double taxable_sales_hst { get; set; }
        public double hst { get; set; }
        public double amount_for_esf { get; set; }
        public double esf { get; set; }
        public double export_sales { get; set; }
        public double tax_exempted_sales { get; set; }
        public bool isrealtime { get; set; }
        public DateTime? datetimeClient { get; set; }
    }

    public class RejectedItem
    {
        public string ITEM_ID { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_NAME { get; set; }
        public string REMARKS { get; set; }
        public string DELETED_FLAG { get; set; }
        public string COMPANY_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_DATE { get; set; }
        public string MODIFY_BY { get; set; }
        public string MODIFY_DATE { get; set; }
    }
}