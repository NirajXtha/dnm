using NeoERP.DocumentTemplate.Service.Interface.ThirdPartyApi;
using NeoERP.DocumentTemplate.Service.Models.ThirdPartyApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Transactions;
using System.Web;
using System.Web.Http;
using System.Xml.Linq;

namespace NeoERP.DocumentTemplate.Controllers.ThirdPartyApi
{
    public class SymphonyApiController : ApiController
    {
        private IShymphonyService _shymphonyService;
        public SymphonyApiController(IShymphonyService shymphonyService)
        {
            this._shymphonyService = shymphonyService;
        }
        Shymphony Shymphony = new Shymphony();
        private string CleanXml(string xml)
        {
            // Trim any leading/trailing white spaces
            xml = xml.Trim();

            // Remove BOM if present
            if (xml.StartsWith("\ufeff"))
            {
                xml = xml.Remove(0, 1);
            }
            return xml;
        }
        [BasicAuthentication]
        [HttpPost]

        public HttpResponseMessage Write()
        {


            var httpRequest = HttpContext.Current.Request;
            try
            {

                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    postedFile.SaveAs(HttpContext.Current.Server.MapPath(@"~/xmlfile/Shymphony/" + postedFile.FileName));
                    var filePath = HttpContext.Current.Server.MapPath(@"~/xmlfile/Shymphony/" + postedFile.FileName);
                    string xmlFile = filePath;
                    string xmlstring = System.IO.File.ReadAllText(xmlFile);
                    //var xmModel = JsonConvert.DeserializeObject<OraPayloadEntityFieldModel>(xmlstring);
                    var xml = XDocument.Parse(xmlstring);
                    var xmlCheck = xml.Descendants("Check").ToList();
                    var Header = xml.Descendants("Check").Descendants("Header").Descendants("FieldList").Descendants("OraPayloadEntityField").ToArray();
                    var MenuItem = xml.Descendants("Check").Descendants("MenuItemList").Descendants("OraPayloadEntityMI").ToList();
                        var Discount = xml.Descendants("Check").Descendants("DiscountList").Descendants("OraPayloadEntityDisc").Descendants("FieldList").ToList();
                    var ServiceCharge = xml.Descendants("Check").Descendants("ServiceChargeList").Descendants("OraPayloadEntitySC").Descendants("FieldList").ToList();
                    var TenderMedia = xml.Descendants("Check").Descendants("TenderMediaList").Descendants("OraPayloadEntityTmed").Descendants("FieldList").ToList();
                    Shymphony.Checkxml = new Checkxml();
                    Shymphony.Header = new List<Header>();
                    Shymphony.MenuItem = new List<MenuItem>();
                    Shymphony.Discount = new List<Discount>();
                    Shymphony.ServiceCharge = new List<ServiceCharge>();
                    Shymphony.TenderMedia = new List<TenderMedia>();
                    // Shymphony.RejectedItem = new List<RejectedItem>();

                    //#region rejected_items
                    //var filePath1 = HttpContext.Current.Server.MapPath(@"~/xmlfile/symphonyConfig/symphony_config.xml");
                    //string xmlFile1 = filePath1;
                    //string xmlstring1 = System.IO.File.ReadAllText(xmlFile1);
                    //var xml1 = XDocument.Parse(xmlstring1);
                    //var rejected_items = xml1.Descendants("symphony_items").Descendants("head").Descendants("rejected_items").Descendants("item").ToList();
                    //if (rejected_items.Count > 0)
                    //{
                    //    foreach (var rejectedItemData in rejected_items)
                    //    {
                    //        Shymphony.RejectedItem.Add(new RejectedItem
                    //        {
                    //            rejected_item_name = !string.IsNullOrEmpty(rejectedItemData.Value) ? Regex.Replace(rejectedItemData.Value, "[^0-9a-zA-Z]+", " ") : ""
                    //        });
                    //    }
                    //}
                    //#endregion

                    #region Header
                    Header header = new Header();
                    foreach (var HeaderData in Header)
                    {
                        header.CheckNumber = header.CheckNumber == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "CheckNumber" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.CheckNumber;
                        header.Table = header.Table == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "Table" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.Table;
                        header.GuestCount = header.GuestCount == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "GuestCount" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.GuestCount;
                        header.EmployeeFirstName = header.EmployeeFirstName == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "EmployeeFirstName" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.EmployeeFirstName;
                        header.RvcID = header.RvcID == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "RvcID" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.RvcID;
                        header.RvcName = header.RvcName == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "RvcName" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.RvcName;
                        header.PropertyID = header.PropertyID == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "PropertyID" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.PropertyID;
                        header.PropertyName = header.PropertyName == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "PropertyName" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.PropertyName;
                        header.CheckSubtotal = header.CheckSubtotal == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "CheckSubtotal" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.CheckSubtotal;
                        header.CheckDiscount = header.CheckDiscount == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "CheckDiscount" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.CheckDiscount;
                        header.CheckTax = header.CheckTax == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "CheckTax" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.CheckTax;
                        header.AutoServiceChargeDescriptor = header.AutoServiceChargeDescriptor == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "AutoServiceChargeDescriptor" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.AutoServiceChargeDescriptor;
                        header.CheckAutoServiceCharge = header.CheckAutoServiceCharge == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "CheckAutoServiceCharge" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.CheckAutoServiceCharge;
                        header.CheckOther = header.CheckOther == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "CheckOther" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.CheckOther;
                        header.WorkstationID = header.WorkstationID == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "WorkstationID" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.WorkstationID;
                        header.Referencia = header.Referencia == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "Referencia" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.Referencia;
                        header.ClientId = header.ClientId == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "ClientId" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.ClientId;
                        header.TotalDue = header.TotalDue == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "TotalDue" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.TotalDue;
                        header.AutoServiceChargeActive = header.AutoServiceChargeActive == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "AutoServiceChargeActive" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.AutoServiceChargeActive;
                        header.PrimaryCheckIdentification = header.PrimaryCheckIdentification == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "PrimaryCheckIdentification" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.PrimaryCheckIdentification;
                        //new code change to get customer data
                        header.CaptureCustomerData = header.CaptureCustomerData == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "CaptureCustomerData" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.CaptureCustomerData;
                        ////header.CustomerDocumentType = header.CustomerDocumentType == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "CustomerDocumentType" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.CustomerDocumentType;
                        ///

                        header.CustomerDocumentType = header.CustomerDocumentType == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "CustomerDocumentType" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.CustomerDocumentType;


                        header.Document = header.Document == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "Document" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.Document;
                        header.Name = header.Name == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "Name" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.Name;

                        header.Email = header.Email == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "Email" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.Email;
                        header.CaptureCustomerAddress = header.CaptureCustomerAddress == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "CaptureCustomerAddress" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.CaptureCustomerAddress;
                        header.Address = header.Address == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "Address" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.Address;
                        header.Number = header.Number == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "Number" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.Number;

                        header.District = header.District == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "District" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.District;
                        header.City = header.City == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "City" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.City;
                        header.State = header.State == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "State" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.State;
                        header.PostalCode = header.PostalCode == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "PostalCode" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.PostalCode;
                        header.DocumentType = header.DocumentType == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "DocumentType" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.DocumentType;
                        header.DocumentId = header.DocumentId == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "DocumentId" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.DocumentId;
                        header.Complement = header.Complement == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "Complement" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.Complement;

                        //new code change to get customer data

                    //new code added..
                    header.SimphonyPOSVersion = header.SimphonyPOSVersion == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "SimphonyPOSVersion" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.SimphonyPOSVersion;
                    header.WorkstationName = header.WorkstationName == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "WorkstationName" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.WorkstationName;
                    header.CashierID = header.CashierID == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "CashierID" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.CashierID;
                    header.BusinessDate = header.BusinessDate == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "BusinessDate" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.BusinessDate;
                    header.BusinessDateTime = header.BusinessDateTime == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "BusinessDateTime" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.BusinessDateTime;
                    header.CurrentDateTime = header.CurrentDateTime == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "CurrentDateTime" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.CurrentDateTime;
                    header.TableTextAndGroup = header.TableTextAndGroup == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "TableTextAndGroup" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.TableTextAndGroup;
                    header.OrderTypeName = header.OrderTypeName == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "OrderTypeName" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.OrderTypeName;
                    header.OrderTypeNumber = header.OrderTypeNumber == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "OrderTypeNumber" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.OrderTypeNumber;
                    header.AutoSvcTurnedOff = header.AutoSvcTurnedOff == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "AutoSvcTurnedOff" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.AutoSvcTurnedOff;
                    header.BegunAsFastTransaction = header.BegunAsFastTransaction == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "BegunAsFastTransaction" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.BegunAsFastTransaction;
                    header.IsFastTransaction = header.IsFastTransaction == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "IsFastTransaction" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.IsFastTransaction;
                    header.RoundNumber = header.RoundNumber == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "RoundNumber" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.RoundNumber;
                    header.Guid = header.Guid == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "Guid" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.Guid;
                    header.CheckGrossTotal = header.CheckGrossTotal == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "CheckGrossTotal" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.CheckGrossTotal;
                    header.CheckTotalDue = header.CheckTotalDue == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "CheckTotalDue" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.CheckTotalDue;
                    header.ChargedTipTotal = header.ChargedTipTotal == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "ChargedTipTotal" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.ChargedTipTotal;
                    header.CheckServiceChargeTotal = header.CheckServiceChargeTotal == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "CheckServiceChargeTotal" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.CheckServiceChargeTotal;
                    header.TotalDueByGuest = header.TotalDueByGuest == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "TotalDueByGuest" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.TotalDueByGuest;
                    header.ChangeDue = header.ChangeDue == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "ChangeDue" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.ChangeDue;
                    header.CheckPartialPaymentTotal = header.CheckPartialPaymentTotal == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "CheckPartialPaymentTotal" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.CheckPartialPaymentTotal;
                    header.Training = header.Training == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "Training" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.Training;
                    header.TransEmployeeFirstName = header.TransEmployeeFirstName == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "TransEmployeeFirstName" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.TransEmployeeFirstName;
                    header.TransEmployeeLastName = header.TransEmployeeLastName == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "TransEmployeeLastName" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.TransEmployeeLastName;
                    header.EmployeeId = header.EmployeeId == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "EmployeeId" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.EmployeeId;
                    header.CheckCurrencySymbol = header.CheckCurrencySymbol == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "CheckCurrencySymbol" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.CheckCurrencySymbol;
                    header.AddedIn = header.AddedIn == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "AddedIn" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.AddedIn;
                    header.ChkCancelled = header.ChkCancelled == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "ChkCancelled" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.ChkCancelled;
                    header.ChkSplitXfer = header.ChkSplitXfer == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "ChkSplitXfer" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.ChkSplitXfer;
                    header.SplitCheckAdded = header.SplitCheckAdded == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "SplitCheckAdded" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.SplitCheckAdded;
                    header.SplitFromCheckNum = header.SplitFromCheckNum == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "SplitFromCheckNum" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.SplitFromCheckNum;
                    header.SplitFromRevCtrID = header.SplitFromRevCtrID == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "SplitFromRevCtrID" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.SplitFromRevCtrID;
                    header.SplitOut = header.SplitOut == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "SplitOut" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.SplitOut;
                    header.ReopenedFromCheckNum = header.ReopenedFromCheckNum == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "ReopenedFromCheckNum" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.ReopenedFromCheckNum;
                    header.ReopenedToCheckNum = header.ReopenedToCheckNum == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "ReopenedToCheckNum" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.ReopenedToCheckNum;
                    header.MergedCheckNotPrinted = header.MergedCheckNotPrinted == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "MergedCheckNotPrinted" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.MergedCheckNotPrinted;
                    header.CheckWasReOpened = header.CheckWasReOpened == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "CheckWasReOpened" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.CheckWasReOpened;
                    header.ClosedCheckReprint = header.ClosedCheckReprint == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "ClosedCheckReprint" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.ClosedCheckReprint;
                    header.ClosedCheckVoid = header.ClosedCheckVoid == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "ClosedCheckVoid" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.ClosedCheckVoid;
                    header.ClosedChkAdj = header.ClosedChkAdj == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "ClosedChkAdj" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.ClosedChkAdj;
                    header.CloseStatus = header.CloseStatus == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "CloseStatus" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.CloseStatus;
                    header.TransVoidEnabled = header.TransVoidEnabled == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "TransVoidEnabled" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.TransVoidEnabled;
                    header.TransReturnEnabled = header.TransReturnEnabled == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "TransReturnEnabled" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.TransReturnEnabled;
                    header.ParserTemplatePath = header.ParserTemplatePath == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "ParserTemplatePath" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.ParserTemplatePath;
                    header.ParserTemplateHash = header.ParserTemplateHash == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "ParserTemplateHash" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.ParserTemplateHash;
                    header.PayloadHash = header.PayloadHash == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "PayloadHash" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.PayloadHash;
                    //new code added..

                    }
                    Shymphony.Header.Add(header);
                    //#endregion

                    #region MenuItemList
                    foreach (dynamic ItemRecord in MenuItem)
                    {
                        MenuItem menuItem = new MenuItem();
                        var JsonResult = JsonConvert.SerializeObject(ItemRecord);
                        JObject o = JObject.Parse(JsonResult);
                        dynamic FeildList = o.SelectToken("OraPayloadEntityMI").SelectToken("FieldList").SelectToken("OraPayloadEntityField").ToList();
                        foreach (dynamic ItemData in FeildList)
                        {
                            if (!(ItemData.SelectToken("@field").ToString().Equals("TaxRates")))
                            {

                                //var TaxDataList = ItemData.SelectToken("GenericParameterList").SelectToken("ValueList").ToList();

                                menuItem.Quantity = menuItem.Quantity == null ? (ItemData.SelectToken("@field").ToString() == "Quantity" ? ItemData.SelectToken("@value").ToString() : null) : menuItem.Quantity;

                                menuItem.HasAnyDiscount = menuItem.HasAnyDiscount == null ? (ItemData.SelectToken("@field").ToString() == "HasAnyDiscount" ? ItemData.SelectToken("@value").ToString() : null) : menuItem.HasAnyDiscount;
                                menuItem.ComboMealNumber = menuItem.ComboMealNumber == null ? (ItemData.SelectToken("@field").ToString() == "ComboMealNumber" ? ItemData.SelectToken("@value").ToString() : null) : menuItem.ComboMealNumber;
                                menuItem.ComboMealSideNumber = menuItem.ComboMealSideNumber == null ? (ItemData.SelectToken("@field").ToString() == "ComboMealSideNumber" ? ItemData.SelectToken("@value").ToString() : null) : menuItem.ComboMealSideNumber;
                                menuItem.ComboMealSidePrepCost = menuItem.ComboMealSidePrepCost == null ? (ItemData.SelectToken("@field").ToString() == "ComboMealSidePrepCost" ? ItemData.SelectToken("@value").ToString() : null) : menuItem.ComboMealSidePrepCost;
                                menuItem.InclusiveTax = menuItem.InclusiveTax == null ? (ItemData.SelectToken("@field").ToString() == "InclusiveTax" ? ItemData.SelectToken("@value").ToString() : null) : menuItem.InclusiveTax;
                                menuItem.SalesCount = menuItem.SalesCount == null ? (ItemData.SelectToken("@field").ToString() == "SalesCount" ? ItemData.SelectToken("@value").ToString() : null) : menuItem.SalesCount;
                                menuItem.DecimalSalesCount = menuItem.DecimalSalesCount == null ? (ItemData.SelectToken("@field").ToString() == "DecimalSalesCount" ? ItemData.SelectToken("@value").ToString() : null) : menuItem.DecimalSalesCount;
                                menuItem.Total = menuItem.Total == null ? (ItemData.SelectToken("@field").ToString() == "Total" ? ItemData.SelectToken("@value").ToString() : null) : menuItem.Total;
                                menuItem.VoidLink = menuItem.VoidLink == null ? (ItemData.SelectToken("@field").ToString() == "VoidLink" ? ItemData.SelectToken("@value").ToString() : null) : menuItem.VoidLink;
                                menuItem.DetailLink = menuItem.DetailLink == null ? (ItemData.SelectToken("@field").ToString() == "DetailLink" ? ItemData.SelectToken("@value").ToString() : null) : menuItem.DetailLink;
                                menuItem.Name = menuItem.Name == null ? (ItemData.SelectToken("@field").ToString() == "Name" ? ItemData.SelectToken("@value").ToString() : null) : menuItem.Name;
                                menuItem.Name2 = menuItem.Name2 == null ? (ItemData.SelectToken("@field").ToString() == "Name2" ? ItemData.SelectToken("@value").ToString() : null) : menuItem.Name2;
                                menuItem.WeighedItem = menuItem.WeighedItem == null ? (ItemData.SelectToken("@field").ToString() == "WeighedItem" ? ItemData.SelectToken("@value").ToString() : null) : menuItem.WeighedItem;
                                menuItem.Void = menuItem.Void == null ? (ItemData.SelectToken("@field").ToString() == "Void" ? ItemData.SelectToken("@value").ToString() : null) : menuItem.Void;
                                menuItem.LineNumVoid = menuItem.LineNumVoid == null ? (ItemData.SelectToken("@field").ToString() == "LineNumVoid" ? ItemData.SelectToken("@value").ToString() : null) : menuItem.LineNumVoid;
                                menuItem.Shared = menuItem.Shared == null ? (ItemData.SelectToken("@field").ToString() == "Shared" ? ItemData.SelectToken("@value").ToString() : null) : menuItem.Shared;
                                menuItem.FIP_LAD_PAYLOAD_CR_UN_MED_COMERCIAL = menuItem.FIP_LAD_PAYLOAD_CR_UN_MED_COMERCIAL == null ? (ItemData.SelectToken("@field").ToString() == "FIP_LAD_PAYLOAD_CR_UN_MED_COMERCIAL" ? ItemData.SelectToken("@value").ToString() : null) : menuItem.FIP_LAD_PAYLOAD_CR_UN_MED_COMERCIAL;
                                menuItem.FIP_LAD_PAYLOAD_CR_TAX_CODES = menuItem.FIP_LAD_PAYLOAD_CR_TAX_CODES == null ? (ItemData.SelectToken("@field").ToString() == "ItemWeight" ? ItemData.SelectToken("@value").ToString() : null) : menuItem.FIP_LAD_PAYLOAD_CR_TAX_CODES;

                            }
                            else
                            {
                                var TaxDataList = ItemData.SelectToken("ValueList").SelectToken("GenericParameterList");

                                foreach (dynamic TaxitemList in TaxDataList)
                                {
                                    foreach (var Taxitem in TaxitemList)
                                    {
                                        menuItem.ItemTaxList = new List<ItemTaxList>();
                                        ItemTaxList ItemTax = new ItemTaxList();

                                        foreach (var itemData in Taxitem)
                                        {
                                            var Feild = itemData.SelectToken("@field");
                                            if (Feild != null)
                                            {
                                                Feild = Feild.ToString();
                                            }

                                            if (!string.IsNullOrEmpty(Feild))
                                            {
                                                ItemTax.TaxId = ItemTax.TaxId == null ? (itemData.SelectToken("@field").ToString() == "TaxId" ? itemData.SelectToken("@value").ToString() : null) : ItemTax.TaxId;
                                                ItemTax.TaxName = ItemTax.TaxName == null ? (itemData.SelectToken("@field").ToString() == "TaxName" ? itemData.SelectToken("@value").ToString() : null) : ItemTax.TaxName;
                                                ItemTax.TaxValue = ItemTax.TaxValue == null ? (itemData.SelectToken("@field").ToString() == "TaxValue" ? itemData.SelectToken("@value").ToString() : null) : ItemTax.TaxValue;
                                                ItemTax.TaxType = ItemTax.TaxType == null ? (itemData.SelectToken("@field").ToString() == "TaxType" ? itemData.SelectToken("@value").ToString() : null) : ItemTax.TaxType;
                                                ItemTax.FIP_LAD_PAYLOAD_CR_TAX_CODES = ItemTax.FIP_LAD_PAYLOAD_CR_TAX_CODES == null ? (itemData.SelectToken("@field").ToString() == "FIP_LAD_PAYLOAD_CR TAX CODES" ? itemData.SelectToken("@value").ToString() : null) : ItemTax.FIP_LAD_PAYLOAD_CR_TAX_CODES;
                                            }
                                            else
                                            {
                                                foreach (var TaxitemData in itemData)
                                                {
                                                    ItemTax.TaxId = ItemTax.TaxId == null ? (TaxitemData.SelectToken("@field").ToString() == "TaxId" ? TaxitemData.SelectToken("@value").ToString() : null) : ItemTax.TaxId;
                                                    ItemTax.TaxName = ItemTax.TaxName == null ? (TaxitemData.SelectToken("@field").ToString() == "TaxName" ? TaxitemData.SelectToken("@value").ToString() : null) : ItemTax.TaxName;
                                                    ItemTax.TaxValue = ItemTax.TaxValue == null ? (TaxitemData.SelectToken("@field").ToString() == "TaxValue" ? TaxitemData.SelectToken("@value").ToString() : null) : ItemTax.TaxValue;
                                                    ItemTax.TaxType = ItemTax.TaxType == null ? (TaxitemData.SelectToken("@field").ToString() == "TaxType" ? TaxitemData.SelectToken("@value").ToString() : null) : ItemTax.TaxType;
                                                    ItemTax.FIP_LAD_PAYLOAD_CR_TAX_CODES = ItemTax.FIP_LAD_PAYLOAD_CR_TAX_CODES == null ? (TaxitemData.SelectToken("@field").ToString() == "FIP_LAD_PAYLOAD_CR TAX CODES" ? TaxitemData.SelectToken("@value").ToString() : null) : ItemTax.FIP_LAD_PAYLOAD_CR_TAX_CODES;
                                                }
                                                menuItem.ItemTaxList.Add(ItemTax);
                                            }
                                            if (ItemTax.FIP_LAD_PAYLOAD_CR_TAX_CODES != null)
                                            {
                                                menuItem.ItemTaxList.Add(ItemTax);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        string[] rvcid = ConfigurationManager.AppSettings["RVCID"].Split(',');
                        
                        foreach (string ID in rvcid)
                        {
                            if (ID == header.RvcID)
                            {
                                ConfigurationManager.AppSettings["IsRetailBusiness"] = true.ToString();
                            }
                            else
                            {
                                ConfigurationManager.AppSettings["IsRetailBusiness"] = false.ToString();

                            }
                        }
                        if (!string.IsNullOrEmpty(menuItem.Total))
                        {
                            if (ConfigurationManager.AppSettings["IsRetailBusiness"] != null)
                            {
                                bool IsRetailBusiness = false;
                                bool.TryParse(ConfigurationManager.AppSettings["IsRetailBusiness"], out IsRetailBusiness);

                                if (IsRetailBusiness)
                                {
                                    decimal DecTotal = 0;
                                    decimal.TryParse(menuItem.Total, out DecTotal);

                                    decimal BeforeTax = DecTotal / 1.13M;
                                    menuItem.Total = BeforeTax.ToString("0.00");
                                }
                            }
                        }
                        Shymphony.MenuItem.Add(menuItem);
                    }
                    #endregion

                    #region Binding DiscountList
                    foreach (var discountdata in Discount)
                    {
                        Discount discount = new Discount();
                        foreach (var discountdata1 in discountdata.Descendants("OraPayloadEntityField"))
                        {

                            discount.ObjectNumber = discount.ObjectNumber == null ? (discountdata1.Attributes("field").FirstOrDefault().Value == "ObjectNumber" ? discountdata1.Attributes("value").FirstOrDefault().Value : null) : discount.ObjectNumber;

                            discount.Percentage = discount.Percentage == null ? (discountdata1.Attributes("field").FirstOrDefault().Value == "Percentage" ? discountdata1.Attributes("value").FirstOrDefault().Value : null) : discount.Percentage;

                            discount.SalesCount = discount.SalesCount == null ? (discountdata1.Attributes("field").FirstOrDefault().Value == "SalesCount" ? discountdata1.Attributes("value").FirstOrDefault().Value : null) : discount.SalesCount;

                            discount.Total = discount.Total == null ? (discountdata1.Attributes("field").FirstOrDefault().Value == "Total" ? discountdata1.Attributes("value").FirstOrDefault().Value : null) : discount.Total;

                            discount.VoidLink = discount.VoidLink == null ? (discountdata1.Attributes("field").FirstOrDefault().Value == "VoidLink" ? discountdata1.Attributes("value").FirstOrDefault().Value : null) : discount.VoidLink;

                            discount.ItemDsc = discount.ItemDsc == null ? (discountdata1.Attributes("field").FirstOrDefault().Value == "ItemDsc" ? discountdata1.Attributes("value").FirstOrDefault().Value : null) : discount.ItemDsc;

                            discount.Name = discount.Name == null ? (discountdata1.Attributes("field").FirstOrDefault().Value == "Name" ? discountdata1.Attributes("value").FirstOrDefault().Value : null) : discount.Name;

                            discount.Void = discount.Void == null ? (discountdata1.Attributes("field").FirstOrDefault().Value == "Void" ? discountdata1.Attributes("value").FirstOrDefault().Value : null) : discount.Void;

                            discount.LineNumVoid = discount.LineNumVoid == null ? (discountdata1.Attributes("field").FirstOrDefault().Value == "LineNumVoid" ? discountdata.Descendants("OraPayloadEntityField").Attributes("value").FirstOrDefault().Value : null) : discount.LineNumVoid;
                            //var a = discountdata.Descendants("ValueList").Descendants("GenericParameterList").Descendants("OraPayloadEntityFieldGenericParameter").ToList();

                            //foreach (var menuItemDetailLinksdata in discountdata.Descendants("ValueList").Descendants("GenericParameterList").Descendants("OraPayloadEntityFieldGenericParameter").ToList())
                            //{
                            //    MenuItemDetailLinks menuItemDetailLinks = new MenuItemDetailLinks();
                            //    menuItemDetailLinks.DetailedLinkedId = menuItemDetailLinksdata.Attributes("value").FirstOrDefault().Value;
                            //    discount.MenuItemDetailLinks.Add(menuItemDetailLinks);
                            //}
                        }
                        Shymphony.Discount.Add(discount);
                    }
                    #endregion

                    #region Binding ServiceChargeList
                    var serviceChargeJsonResult = JsonConvert.SerializeObject(ServiceCharge);

                    foreach (var serviceChargedata in ServiceCharge)
                    {
                        ServiceCharge serviceCharge = new ServiceCharge();
                        foreach (var serviceChargedata1 in serviceChargedata.Descendants("OraPayloadEntityField").ToList())
                        {
                            serviceCharge.ObjectNumber = serviceCharge.ObjectNumber == null ? (serviceChargedata1.Attributes("field").FirstOrDefault().Value == "ObjectNumber" ? serviceChargedata1.Attributes("value").FirstOrDefault().Value : null) : serviceCharge.ObjectNumber;

                            serviceCharge.Percentage = serviceCharge.Percentage == null ? (serviceChargedata1.Attributes("field").FirstOrDefault().Value == "Percentage" ? serviceChargedata1.Attributes("value").FirstOrDefault().Value : null) : serviceCharge.Percentage;

                            serviceCharge.SalesCount = serviceCharge.SalesCount == null ? (serviceChargedata1.Attributes("field").FirstOrDefault().Value == "SalesCount" ? serviceChargedata1.Attributes("value").FirstOrDefault().Value : null) : serviceCharge.SalesCount;

                            serviceCharge.Total = serviceCharge.Total == null ? (serviceChargedata1.Attributes("field").FirstOrDefault().Value == "Total" ? serviceChargedata1.Attributes("value").FirstOrDefault().Value : null) : serviceCharge.Total;

                            serviceCharge.VoidLink = serviceCharge.VoidLink == null ? (serviceChargedata1.Attributes("field").FirstOrDefault().Value == "VoidLink" ? serviceChargedata1.Attributes("value").FirstOrDefault().Value : null) : serviceCharge.VoidLink;

                            serviceCharge.DetailLink = serviceCharge.DetailLink == null ? (serviceChargedata1.Attributes("field").FirstOrDefault().Value == "DetailLink" ? serviceChargedata1.Attributes("value").FirstOrDefault().Value : null) : serviceCharge.DetailLink;

                            serviceCharge.Name = serviceCharge.Name == null ? (serviceChargedata1.Attributes("field").FirstOrDefault().Value == "Name" ? serviceChargedata1.Attributes("value").FirstOrDefault().Value : null) : serviceCharge.Name;

                            serviceCharge.Void = serviceCharge.Void == null ? (serviceChargedata1.Attributes("field").FirstOrDefault().Value == "Void" ? serviceChargedata1.Attributes("value").FirstOrDefault().Value : null) : serviceCharge.Void;

                            serviceCharge.LineNumVoid = serviceCharge.LineNumVoid == null ? (serviceChargedata1.Attributes("field").FirstOrDefault().Value == "LineNumVoid" ? serviceChargedata1.Attributes("value").FirstOrDefault().Value : null) : serviceCharge.LineNumVoid;
                        }
                        Shymphony.ServiceCharge.Add(serviceCharge);
                    }

                    #endregion

                    #region Binding TenderMediaList

                    foreach (var tenderMediadata in TenderMedia)
                    {
                        TenderMedia tenderMedia = new TenderMedia();
                        foreach (var tenderMediadata1 in tenderMediadata.Descendants("OraPayloadEntityField").ToList())
                        {
                            tenderMedia.ObjectNumber = tenderMedia.ObjectNumber == null ? (tenderMediadata1.Attributes("field").FirstOrDefault().Value == "ObjectNumber" ? tenderMediadata1.Attributes("value").FirstOrDefault().Value : null) : tenderMedia.ObjectNumber;

                            tenderMedia.CurrencyAmount = tenderMedia.CurrencyAmount == null ? (tenderMediadata1.Attributes("field").FirstOrDefault().Value == "CurrencyAmount" ? tenderMediadata1.Attributes("value").FirstOrDefault().Value : null) : tenderMedia.CurrencyAmount;

                            tenderMedia.SalesCount = tenderMedia.SalesCount == null ? (tenderMediadata1.Attributes("field").FirstOrDefault().Value == "SalesCount" ? tenderMediadata1.Attributes("value").FirstOrDefault().Value : null) : tenderMedia.SalesCount;

                            tenderMedia.Total = tenderMedia.Total == null ? (tenderMediadata1.Attributes("field").FirstOrDefault().Value == "Total" ? tenderMediadata1.Attributes("value").FirstOrDefault().Value : null) : tenderMedia.Total;

                            tenderMedia.VoidLink = tenderMedia.VoidLink == null ? (tenderMediadata1.Attributes("field").FirstOrDefault().Value == "VoidLink" ? tenderMediadata1.Attributes("value").FirstOrDefault().Value : null) : tenderMedia.VoidLink;

                            tenderMedia.CurrencyID = tenderMedia.CurrencyID == null ? (tenderMediadata1.Attributes("field").FirstOrDefault().Value == "CurrencyID" ? tenderMediadata1.Attributes("value").FirstOrDefault().Value : null) : tenderMedia.CurrencyID;

                            tenderMedia.Name = tenderMedia.Name == null ? (tenderMediadata1.Attributes("field").FirstOrDefault().Value == "Name" ? tenderMediadata1.Attributes("value").FirstOrDefault().Value : null) : tenderMedia.Name;

                            tenderMedia.Void = tenderMedia.Void == null ? (tenderMediadata1.Attributes("field").FirstOrDefault().Value == "Void" ? tenderMediadata1.Attributes("value").FirstOrDefault().Value : null) : tenderMedia.Void;

                            tenderMedia.LineNumVoid = tenderMedia.LineNumVoid == null ? (tenderMediadata1.Attributes("field").FirstOrDefault().Value == "LineNumVoid" ? tenderMediadata1.Attributes("value").FirstOrDefault().Value : null) : tenderMedia.LineNumVoid;

                            tenderMedia.FIP_LAD_PAYLOAD_CR_TMED_CODES = tenderMedia.FIP_LAD_PAYLOAD_CR_TMED_CODES == null ? (tenderMediadata1.Attributes("field").FirstOrDefault().Value == "FIP_LAD_PAYLOAD_CR_TMED_CODES" ? tenderMediadata1.Attributes("value").FirstOrDefault().Value : null) : tenderMedia.FIP_LAD_PAYLOAD_CR_TMED_CODES;
                        }
                        Shymphony.TenderMedia.Add(tenderMedia);
                    }

                    #endregion

                    #region Check
                    foreach (var checkprop in xmlCheck)
                    {
                        Shymphony.Checkxml.HarmonyId = checkprop.Attribute("HarmonyId").Value;
                        Shymphony.Checkxml.CheckNum = checkprop.Attribute("CheckNum").Value;
                        Shymphony.Checkxml.WsID = checkprop.Attribute("WsID").Value;
                        Shymphony.Checkxml.Timestamp = checkprop.Attribute("Timestamp").Value;
                        var hpl = checkprop.Attribute("Id").Value;
                        Shymphony.Checkxml.Id = hpl.Substring(3);
                        // Shymphony.Checkxml.PartnerID = checkprop.Attribute("PartnerID").Value;
                    }

                    #endregion
                }
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        this._shymphonyService.InsertInvoiceRecord(Shymphony);
                        var isSyncToIrd = ConfigurationManager.AppSettings["isSyncToIrd"];

                        if (Shymphony.Sales_No == "EXISTED_INV")
                        {
                            var httpResponse = new HttpResponseMessage();
                            httpResponse.StatusCode = HttpStatusCode.OK;
                            httpResponse.Content = new StringContent(ToExistingInvoiceXML(Shymphony, "The invoice number already existed."), Encoding.UTF8, "application/xml");
                            return httpResponse;
                        }
                        else if (Shymphony.Sales_No == "NO_INV")
                        {
                            var httpResponse = new HttpResponseMessage();
                            httpResponse.StatusCode = HttpStatusCode.OK;
                            httpResponse.Content = new StringContent(ToNoInvoiceXML(Shymphony, "The invoice number could not provided."), Encoding.UTF8, "application/xml");
                            return httpResponse;
                        }
                        else if (Shymphony.Sales_No == "Room Charge")
                        {
                            var httpResponse = new HttpResponseMessage();
                            httpResponse.StatusCode = HttpStatusCode.OK;
                            httpResponse.Content = new StringContent(ToNoInvoiceXML(Shymphony, "The invoice contains Room Charge which cannot be inserted."), Encoding.UTF8, "application/xml");
                            return httpResponse;
                        }
                        else if (Shymphony.Sales_No == "No Tax")
                        {
                            var httpResponse = new HttpResponseMessage();
                            httpResponse.StatusCode = HttpStatusCode.OK;
                            httpResponse.Content = new StringContent(ToNoInvoiceXML(Shymphony, "The invoice not inserted."), Encoding.UTF8, "application/xml");
                            return httpResponse;
                        }
                        else if (Shymphony.Sales_No== "No MenuItem")
                        {
                            var httpResponse = new HttpResponseMessage();
                            httpResponse.StatusCode = HttpStatusCode.OK;
                            httpResponse.Content = new StringContent(ToErrorXML(Shymphony, "Menu Item List is Empty."), Encoding.UTF8, "application/xml");
                            return httpResponse;
                        }
                        else
                        {
                            scope.Complete();
                            if (isSyncToIrd == "true")
                            {
                                ToIRD(Shymphony);
                            }
                            var httpResponse = new HttpResponseMessage();
                            httpResponse.StatusCode = HttpStatusCode.OK;
                            httpResponse.Content = new StringContent(ToSuccessXML(Shymphony), Encoding.UTF8, "application/xml");
                            return httpResponse;

                        }
                    }
                    catch (System.Exception ex)
                    {
                        var httpResponse = new HttpResponseMessage();
                        httpResponse.StatusCode = HttpStatusCode.NotFound;
                        httpResponse.Content = new StringContent(ToNotFoundXML(Shymphony, ex.ToString()), Encoding.UTF8, "application/xml");
                        return httpResponse;

                    }
                }
            }
            catch (System.Exception ex)
            {
                var httpResponse = new HttpResponseMessage();
                httpResponse.StatusCode = HttpStatusCode.InternalServerError;
                httpResponse.Content = new StringContent(ToErrorXML(Shymphony, ex.ToString()), Encoding.UTF8, "application/xml");
                return httpResponse;
            }

        }

        public string ToSuccessXML(Shymphony shymphony)
        {
            using (MemoryStream xmlStream = new MemoryStream())
            {
                XDocument xmlDocument = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XElement("OraPayloadResponseEntity",
                    new XAttribute("clientVersion", "1.0.0.0"),
                        new XElement("Response",
                                new XAttribute("CheckNum", shymphony.Checkxml.CheckNum.ToString()),
                                new XAttribute("Partnerid", "HPL" + shymphony.Checkxml.Id.ToString()),
                                new XAttribute("HarmonyId", shymphony.Checkxml.HarmonyId.ToString()),
                                new XAttribute("WsID", shymphony.Checkxml.WsID.ToString()),
                                 new XAttribute("Timestamp", shymphony.Checkxml.Timestamp.ToString()),
                              new XElement("Header",
                                  new XElement("OraPayloadResponseCode", "1"),
                                   new XElement("OraPayloadResponseRejectReason", "Success")))));
                return xmlDocument.ToString();
            }
        }
        public string ToNotFoundXML(Shymphony shymphony, string ex)
        {
            using (MemoryStream xmlStream = new MemoryStream())
            {
                XDocument xmlDocument = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XElement("OraPayloadResponseEntity",
                    new XAttribute("clientVersion", "1.0.0.0"),
                        new XElement("Response",
                                new XAttribute("CheckNum", shymphony.Checkxml.CheckNum.ToString()),
                                new XAttribute("Partnerid", "HPL" + shymphony.Checkxml.Id.ToString()),
                                new XAttribute("HarmonyId", shymphony.Checkxml.HarmonyId.ToString()),
                                 new XAttribute("Timestamp", shymphony.Checkxml.Timestamp.ToString()),
                              new XElement("Header",
                                  new XElement("OraPayloadResponseCode", "99"),
                                   new XElement("OraPayloadResponseRejectReason", ex)))));
                return xmlDocument.ToString();
            }
        }
        public string ToErrorXML(Shymphony shymphony, string ex)
        {
            using (MemoryStream xmlStream = new MemoryStream())
            {
                XDocument xmlDocument = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XElement("OraPayloadResponseEntity",
                    new XAttribute("clientVersion", "1.0.0.0"),
                        new XElement("Response",
                                new XAttribute("CheckNum", shymphony.Checkxml.CheckNum==null?"Null": shymphony.Checkxml.CheckNum.ToString()),
                                new XAttribute("Partnerid", shymphony.Checkxml.Id == null ? "Null" : shymphony.Checkxml.Id.ToString()),
                                new XAttribute("HarmonyId", shymphony.Checkxml.HarmonyId == null ? "Null" : shymphony.Checkxml.HarmonyId.ToString()),
                                 new XAttribute("Timestamp", shymphony.Checkxml.Timestamp == null ? "Null" :shymphony.Checkxml.Timestamp.ToString()),
                              new XElement("Header",
                                  new XElement("OraPayloadResponseCode", "500"),
                                   new XElement("OraPayloadResponseRejectReason", ex)))));
                return xmlDocument.ToString();
            }
        }
        public string ToExistingInvoiceXML(Shymphony shymphony, string ex)
        {
            using (MemoryStream xmlStream = new MemoryStream())
            {
                XDocument xmlDocument = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XElement("OraPayloadResponseEntity",
                    new XAttribute("clientVersion", "1.0.0.0"),
                        new XElement("Response",
                                new XAttribute("CheckNum", shymphony.Checkxml.CheckNum.ToString()),
                                new XAttribute("Partnerid", shymphony.Checkxml.Id.ToString()),
                                new XAttribute("HarmonyId", shymphony.Checkxml.HarmonyId.ToString()),
                                 new XAttribute("Timestamp", shymphony.Checkxml.Timestamp.ToString()),
                              new XElement("Header",
                                  new XElement("OraPayloadResponseCode", "22"),
                                   new XElement("OraPayloadResponseRejectReason", ex)))));
                return xmlDocument.ToString();
            }
        }
        public string ToNoInvoiceXML(Shymphony shymphony, string ex)
        {
            using (MemoryStream xmlStream = new MemoryStream())
            {
                XDocument xmlDocument = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XElement("OraPayloadResponseEntity",
                    new XAttribute("clientVersion", "1.0.0.0"),
                        new XElement("Response",
                                new XAttribute("CheckNum", shymphony.Checkxml.CheckNum.ToString()),
                                new XAttribute("Partnerid", shymphony.Checkxml.Id.ToString()),
                                new XAttribute("HarmonyId", shymphony.Checkxml.HarmonyId.ToString()),
                                 new XAttribute("Timestamp", shymphony.Checkxml.Timestamp.ToString()),
                              new XElement("Header",
                                  new XElement("OraPayloadResponseCode", "1"),
                                   new XElement("OraPayloadResponseRejectReason", ex)))));
                return xmlDocument.ToString();
            }
        }

      
        public string ToIRD(Shymphony shymphony)
        {
            using (var client = new HttpClient())
            {
                //FiscalYear
                var username = ConfigurationManager.AppSettings["username"];
                var password = ConfigurationManager.AppSettings["password"];
                var seller_pan = ConfigurationManager.AppSettings["seller_pan"];
                var fiscal_year = ConfigurationManager.AppSettings["FiscalYear"];
                var IRDUrl = ConfigurationManager.AppSettings["IRDUrl"];
                var IRDUrlReturn = ConfigurationManager.AppSettings["IRDUrlReturn"];
                var invoice_date = Convert.ToDateTime(Shymphony.invoice_date).ToString("yyyy.MM.dd");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                BillViewModel billViewModel = new BillViewModel
                {
                    username = username,
                    password = password,
                    seller_pan = seller_pan,
                    buyer_pan = Shymphony.buyer_pan,
                    buyer_name = Shymphony.buyer_name,
                    fiscal_year = fiscal_year,
                    invoice_number = shymphony.Sales_No,
                    invoice_date = invoice_date,
                    total_sales = Shymphony.totalSales,
                    taxable_sales_vat = Shymphony.taxableSalesVat,
                    vat = Shymphony.vatAmount,
                    excisable_amount = 0,
                    excise = 0,
                    taxable_sales_hst = 0,
                    hst = 0,
                    amount_for_esf = 0,
                    esf = 0,
                    export_sales = 0,
                    tax_exempted_sales = 0,
                    isrealtime = true,
                    datetimeClient = DateTime.Now
                };
                var response = client.PostAsJsonAsync(IRDUrl, billViewModel).Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync();
                    if (result.Result.ToString() == "100")
                    {
                        this._shymphonyService.ifIsRealTimeFalse(Shymphony);
                        return "API credentials do not match";
                    }
                    else if (result.Result.ToString() == "101")
                    {
                        this._shymphonyService.ifIsRealTimeFalse(Shymphony);
                        return "bill already exists";
                    }
                    else if (result.Result.ToString() == "102")
                    {
                        this._shymphonyService.ifIsRealTimeFalse(Shymphony);
                        return "exception while saving bill details";
                    }
                    else if (result.Result.ToString() == "103")
                    {
                        this._shymphonyService.ifIsRealTimeFalse(Shymphony);
                        return "Unknown exceptions";
                    }
                    else if (result.Result.ToString() == "104")
                    {
                        this._shymphonyService.ifIsRealTimeFalse(Shymphony);
                        return "model invalid";
                    }
                    else
                    {
                        return result.Status.ToString();
                    }

                }
                else
                {
                    var result = response.Content.ReadAsStringAsync();

                    this._shymphonyService.ifIsRealTimeFalse(Shymphony);
                    return "Error";
                }
            }
        }
    }
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
#endregion