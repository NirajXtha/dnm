using NeoErp.Core;
using NeoErp.Core.Caching;
using NeoErp.Data;
using NeoERP.DocumentTemplate.Service.Interface.ThirdPartyApi;
using NeoERP.DocumentTemplate.Service.Models.ThirdPartyApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Transactions;
using System.Web;
using System.Web.Http;
using System.Xml.Linq;

namespace NeoERP.DocumentTemplate.Controllers.ThirdPartyApi
{
    public class ShymphonyApiController : ApiController
    {

        private IWorkContext _workContext;
        private ICacheManager _cacheManager;
        private IDbContext _dbContext;
        private IShymphonyService _hotelogixService;
        public ShymphonyApiController(IDbContext dbContext, IWorkContext workContext, ICacheManager cacheManager, IShymphonyService hotelogixService)
        {
            this._dbContext = dbContext;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
            this._hotelogixService = hotelogixService;
        }


      //  [BasicAuthentication]
        [HttpPost]
        public HttpResponseMessage ReadWrite()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                Shymphony Shymphony = new Shymphony();
                foreach (string file in httpRequest.Files)
                {

                    var postedFile = httpRequest.Files[file];
                    postedFile.SaveAs(HttpContext.Current.Server.MapPath(@"~/Areas/NeoERP.DocumentTemplate/xmlfile/Shymphony/" + postedFile.FileName));
                    var filePath = HttpContext.Current.Server.MapPath(@"~/Areas/NeoERP.DocumentTemplate/xmlfile/Shymphony/" + postedFile.FileName);
                    string xmlFile = filePath;
                    string xmlstring = System.IO.File.ReadAllText(xmlFile);
                    var xml = XDocument.Parse(xmlstring);
                    var Header = xml.Descendants("Check").Descendants("Header").Descendants("FieldList").Descendants("OraPayloadEntityField").ToArray();
                    var MenuItem = xml.Descendants("Check").Descendants("MenuItemList").Descendants("OraPayloadEntityMI").ToList();
                    var Discount = xml.Descendants("Check").Descendants("DiscountList").Descendants("OraPayloadEntityDisc").Descendants("FieldList").ToList();
                    var ServiceCharge = xml.Descendants("Check").Descendants("ServiceChargeList").Descendants("OraPayloadEntitySC").Descendants("FieldList").ToList();
                    var TenderMedia = xml.Descendants("Check").Descendants("TenderMediaList").Descendants("OraPayloadEntityTmed").Descendants("FieldList").ToList();

                    Shymphony.Header = new List<Header>();
                    Shymphony.MenuItem = new List<MenuItem>();
                    Shymphony.Discount = new List<Discount>();
                    Shymphony.ServiceCharge = new List<ServiceCharge>();
                    Shymphony.TenderMedia = new List<TenderMedia>();

                    #region Header

                    Header header = new Header();
                    foreach (var HeaderData in Header)
                    {
                        header.CheckNumber = header.CheckNumber == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "CheckNumber" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.CheckNumber;
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

                    }
                    Shymphony.Header.Add(header);
                    #endregion

                    #region MenuItemList
                    foreach (dynamic ItemRecord in MenuItem)
                    {
                        MenuItem menuItem = new MenuItem();

                        //dynamic ItemField = ItemRecord.Element("FieldList").Descendants("OraPayloadEntityField");
                        var JsonResult = JsonConvert.SerializeObject(ItemRecord);




                        JObject o = JObject.Parse(JsonResult);
                        dynamic FeildList = o.SelectToken("OraPayloadEntityMI").SelectToken("FieldList").SelectToken("OraPayloadEntityField").ToList();
                        // FeildList = JsonConvert.SerializeObject(FeildList);
                        foreach (dynamic ItemData in FeildList)
                        {
                            if (!(ItemData.SelectToken("@field").ToString().Equals("TaxRates")))
                            {
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
                }
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        this._hotelogixService.InsertInvoiceRecord(Shymphony);
                        if (Shymphony.Sales_No == "AE")
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "The invoice number already existed." });
                        }
                        else
                        {
                            scope.Complete();
                            return Request.CreateResponse(HttpStatusCode.Created, Shymphony);

                        }
                    }
                    catch (System.Exception ex)
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                    }



                }

                // return Ok(Shymphony);
            }
            catch (System.Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }

        //[HttpPost]
        //public IHttpActionResult Read(string formCode)
        //{
        //    var httpRequest = HttpContext.Current.Request;
        //    Shymphony Shymphony = new Shymphony();
        //    foreach (string file in httpRequest.Files)
        //    {

        //        var postedFile = httpRequest.Files[file];
        //        postedFile.SaveAs(HttpContext.Current.Server.MapPath(@"~/Areas/NeoERP.DocumentTemplate/xmlfile/Shymphony/" + postedFile.FileName));
        //        var filePath = HttpContext.Current.Server.MapPath(@"~/Areas/NeoERP.DocumentTemplate/xmlfile/Shymphony/" + postedFile.FileName);
        //        string xmlFile = filePath;
        //        string xmlstring = System.IO.File.ReadAllText(xmlFile);
        //        var xml = XDocument.Parse(xmlstring);
        //        var Header = xml.Descendants("Check").Descendants("Header").Descendants("FieldList").Descendants("OraPayloadEntityField").ToArray();
        //        var MenuItem = xml.Descendants("Check").Descendants("MenuItemList").Descendants("OraPayloadEntityMI").ToList();
        //        var Discount = xml.Descendants("Check").Descendants("DiscountList").Descendants("OraPayloadEntityDisc").Descendants("FieldList").ToList();
        //        var ServiceCharge = xml.Descendants("Check").Descendants("ServiceChargeList").Descendants("OraPayloadEntitySC").Descendants("FieldList").ToList();
        //        var TenderMedia = xml.Descendants("Check").Descendants("TenderMediaList").Descendants("OraPayloadEntityTmed").Descendants("FieldList").ToList();

        //        Shymphony.Header = new List<Header>();
        //        Shymphony.MenuItem = new List<MenuItem>();
        //        Shymphony.Discount = new List<Discount>();
        //        Shymphony.ServiceCharge = new List<ServiceCharge>();
        //        Shymphony.TenderMedia = new List<TenderMedia>();

        //        #region Header

        //        Header header = new Header();
        //        foreach (var HeaderData in Header)
        //        {
        //            header.CheckNumber = header.CheckNumber == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "CheckNumber" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.CheckNumber;
        //            header.RvcID = header.RvcID == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "RvcID" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.RvcID;
        //            header.RvcName = header.RvcName == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "RvcName" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.RvcName;
        //            header.PropertyID = header.PropertyID == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "PropertyID" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.PropertyID;
        //            header.PropertyName = header.PropertyName == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "PropertyName" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.PropertyName;
        //            header.CheckSubtotal = header.CheckSubtotal == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "CheckSubtotal" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.CheckSubtotal;
        //            header.CheckDiscount = header.CheckDiscount == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "CheckDiscount" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.CheckDiscount;
        //            header.CheckTax = header.CheckTax == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "CheckTax" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.CheckTax;
        //            header.AutoServiceChargeDescriptor = header.AutoServiceChargeDescriptor == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "AutoServiceChargeDescriptor" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.AutoServiceChargeDescriptor;
        //            header.CheckAutoServiceCharge = header.CheckAutoServiceCharge == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "CheckAutoServiceCharge" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.CheckAutoServiceCharge;
        //            header.CheckOther = header.CheckOther == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "CheckOther" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.CheckOther;
        //            header.WorkstationID = header.WorkstationID == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "WorkstationID" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.WorkstationID;
        //            header.Referencia = header.Referencia == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "Referencia" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.Referencia;
        //            header.ClientId = header.ClientId == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "ClientId" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.ClientId;
        //            header.TotalDue = header.TotalDue == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "TotalDue" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.TotalDue;
        //            header.AutoServiceChargeActive = header.AutoServiceChargeActive == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "AutoServiceChargeActive" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.AutoServiceChargeActive;
        //            header.PrimaryCheckIdentification = header.PrimaryCheckIdentification == null ? (HeaderData.Attributes("field").FirstOrDefault().Value == "PrimaryCheckIdentification" ? HeaderData.Attributes("value").FirstOrDefault().Value : null) : header.PrimaryCheckIdentification;

        //        }
        //        Shymphony.Header.Add(header);
        //        #endregion

        //        #region MenuItemList
        //        foreach (dynamic ItemRecord in MenuItem)
        //        {
        //            MenuItem menuItem = new MenuItem();

        //            //dynamic ItemField = ItemRecord.Element("FieldList").Descendants("OraPayloadEntityField");
        //            var JsonResult = JsonConvert.SerializeObject(ItemRecord);




        //            JObject o = JObject.Parse(JsonResult);
        //            dynamic FeildList = o.SelectToken("OraPayloadEntityMI").SelectToken("FieldList").SelectToken("OraPayloadEntityField").ToList();
        //            // FeildList = JsonConvert.SerializeObject(FeildList);
        //            foreach (dynamic ItemData in FeildList)
        //            {
        //                if (!(ItemData.SelectToken("@field").ToString().Equals("TaxRates")))
        //                {
        //                    menuItem.HasAnyDiscount = menuItem.HasAnyDiscount == null ? (ItemData.SelectToken("@field").ToString() == "HasAnyDiscount" ? ItemData.SelectToken("@value").ToString() : null) : menuItem.HasAnyDiscount;
        //                    menuItem.ComboMealNumber = menuItem.ComboMealNumber == null ? (ItemData.SelectToken("@field").ToString() == "ComboMealNumber" ? ItemData.SelectToken("@value").ToString() : null) : menuItem.ComboMealNumber;
        //                    menuItem.ComboMealSideNumber = menuItem.ComboMealSideNumber == null ? (ItemData.SelectToken("@field").ToString() == "ComboMealSideNumber" ? ItemData.SelectToken("@value").ToString() : null) : menuItem.ComboMealSideNumber;
        //                    menuItem.ComboMealSidePrepCost = menuItem.ComboMealSidePrepCost == null ? (ItemData.SelectToken("@field").ToString() == "ComboMealSidePrepCost" ? ItemData.SelectToken("@value").ToString() : null) : menuItem.ComboMealSidePrepCost;
        //                    menuItem.InclusiveTax = menuItem.InclusiveTax == null ? (ItemData.SelectToken("@field").ToString() == "InclusiveTax" ? ItemData.SelectToken("@value").ToString() : null) : menuItem.InclusiveTax;
        //                    menuItem.SalesCount = menuItem.SalesCount == null ? (ItemData.SelectToken("@field").ToString() == "SalesCount" ? ItemData.SelectToken("@value").ToString() : null) : menuItem.SalesCount;
        //                    menuItem.DecimalSalesCount = menuItem.DecimalSalesCount == null ? (ItemData.SelectToken("@field").ToString() == "DecimalSalesCount" ? ItemData.SelectToken("@value").ToString() : null) : menuItem.DecimalSalesCount;
        //                    menuItem.Total = menuItem.Total == null ? (ItemData.SelectToken("@field").ToString() == "Total" ? ItemData.SelectToken("@value").ToString() : null) : menuItem.Total;
        //                    menuItem.VoidLink = menuItem.VoidLink == null ? (ItemData.SelectToken("@field").ToString() == "VoidLink" ? ItemData.SelectToken("@value").ToString() : null) : menuItem.VoidLink;
        //                    menuItem.DetailLink = menuItem.DetailLink == null ? (ItemData.SelectToken("@field").ToString() == "DetailLink" ? ItemData.SelectToken("@value").ToString() : null) : menuItem.DetailLink;
        //                    menuItem.Name = menuItem.Name == null ? (ItemData.SelectToken("@field").ToString() == "Name" ? ItemData.SelectToken("@value").ToString() : null) : menuItem.Name;
        //                    menuItem.Name2 = menuItem.Name2 == null ? (ItemData.SelectToken("@field").ToString() == "Name2" ? ItemData.SelectToken("@value").ToString() : null) : menuItem.Name2;
        //                    menuItem.WeighedItem = menuItem.WeighedItem == null ? (ItemData.SelectToken("@field").ToString() == "WeighedItem" ? ItemData.SelectToken("@value").ToString() : null) : menuItem.WeighedItem;
        //                    menuItem.Void = menuItem.Void == null ? (ItemData.SelectToken("@field").ToString() == "Void" ? ItemData.SelectToken("@value").ToString() : null) : menuItem.Void;
        //                    menuItem.LineNumVoid = menuItem.LineNumVoid == null ? (ItemData.SelectToken("@field").ToString() == "LineNumVoid" ? ItemData.SelectToken("@value").ToString() : null) : menuItem.LineNumVoid;
        //                    menuItem.Shared = menuItem.Shared == null ? (ItemData.SelectToken("@field").ToString() == "Shared" ? ItemData.SelectToken("@value").ToString() : null) : menuItem.Shared;
        //                    menuItem.FIP_LAD_PAYLOAD_CR_UN_MED_COMERCIAL = menuItem.FIP_LAD_PAYLOAD_CR_UN_MED_COMERCIAL == null ? (ItemData.SelectToken("@field").ToString() == "FIP_LAD_PAYLOAD_CR_UN_MED_COMERCIAL" ? ItemData.SelectToken("@value").ToString() : null) : menuItem.FIP_LAD_PAYLOAD_CR_UN_MED_COMERCIAL;
        //                    menuItem.FIP_LAD_PAYLOAD_CR_TAX_CODES = menuItem.FIP_LAD_PAYLOAD_CR_TAX_CODES == null ? (ItemData.SelectToken("@field").ToString() == "ItemWeight" ? ItemData.SelectToken("@value").ToString() : null) : menuItem.FIP_LAD_PAYLOAD_CR_TAX_CODES;
        //                }
        //                else
        //                {
        //                    var TaxDataList = ItemData.SelectToken("ValueList").SelectToken("GenericParameterList");

        //                    foreach (dynamic TaxitemList in TaxDataList)
        //                    {
        //                        foreach (var Taxitem in TaxitemList)
        //                        {
        //                            menuItem.ItemTaxList = new List<ItemTaxList>();
        //                            ItemTaxList ItemTax = new ItemTaxList();

        //                            foreach (var itemData in Taxitem)
        //                            {
        //                                var Feild = itemData.SelectToken("@field");
        //                                if (Feild != null)
        //                                {
        //                                    Feild = Feild.ToString();
        //                                }

        //                                if (!string.IsNullOrEmpty(Feild))
        //                                {
        //                                    ItemTax.TaxId = ItemTax.TaxId == null ? (itemData.SelectToken("@field").ToString() == "TaxId" ? itemData.SelectToken("@value").ToString() : null) : ItemTax.TaxId;
        //                                    ItemTax.TaxName = ItemTax.TaxName == null ? (itemData.SelectToken("@field").ToString() == "TaxName" ? itemData.SelectToken("@value").ToString() : null) : ItemTax.TaxName;
        //                                    ItemTax.TaxValue = ItemTax.TaxValue == null ? (itemData.SelectToken("@field").ToString() == "TaxValue" ? itemData.SelectToken("@value").ToString() : null) : ItemTax.TaxValue;
        //                                    ItemTax.TaxType = ItemTax.TaxType == null ? (itemData.SelectToken("@field").ToString() == "TaxType" ? itemData.SelectToken("@value").ToString() : null) : ItemTax.TaxType;
        //                                    ItemTax.FIP_LAD_PAYLOAD_CR_TAX_CODES = ItemTax.FIP_LAD_PAYLOAD_CR_TAX_CODES == null ? (itemData.SelectToken("@field").ToString() == "FIP_LAD_PAYLOAD_CR TAX CODES" ? itemData.SelectToken("@value").ToString() : null) : ItemTax.FIP_LAD_PAYLOAD_CR_TAX_CODES;
        //                                }
        //                                else
        //                                {
        //                                    foreach (var TaxitemData in itemData)
        //                                    {
        //                                        ItemTax.TaxId = ItemTax.TaxId == null ? (TaxitemData.SelectToken("@field").ToString() == "TaxId" ? TaxitemData.SelectToken("@value").ToString() : null) : ItemTax.TaxId;
        //                                        ItemTax.TaxName = ItemTax.TaxName == null ? (TaxitemData.SelectToken("@field").ToString() == "TaxName" ? TaxitemData.SelectToken("@value").ToString() : null) : ItemTax.TaxName;
        //                                        ItemTax.TaxValue = ItemTax.TaxValue == null ? (TaxitemData.SelectToken("@field").ToString() == "TaxValue" ? TaxitemData.SelectToken("@value").ToString() : null) : ItemTax.TaxValue;
        //                                        ItemTax.TaxType = ItemTax.TaxType == null ? (TaxitemData.SelectToken("@field").ToString() == "TaxType" ? TaxitemData.SelectToken("@value").ToString() : null) : ItemTax.TaxType;
        //                                        ItemTax.FIP_LAD_PAYLOAD_CR_TAX_CODES = ItemTax.FIP_LAD_PAYLOAD_CR_TAX_CODES == null ? (TaxitemData.SelectToken("@field").ToString() == "FIP_LAD_PAYLOAD_CR TAX CODES" ? TaxitemData.SelectToken("@value").ToString() : null) : ItemTax.FIP_LAD_PAYLOAD_CR_TAX_CODES;



        //                                    }
        //                                    menuItem.ItemTaxList.Add(ItemTax);
        //                                }

        //                                if (ItemTax.FIP_LAD_PAYLOAD_CR_TAX_CODES != null)
        //                                {
        //                                    menuItem.ItemTaxList.Add(ItemTax);
        //                                }
        //                            }



        //                        }





        //                    }
        //                }


        //            }

        //            Shymphony.MenuItem.Add(menuItem);


        //        }
        //        #endregion

        //        #region Binding DiscountList
        //        foreach (var discountdata in Discount)
        //        {
        //            Discount discount = new Discount();
        //            foreach (var discountdata1 in discountdata.Descendants("OraPayloadEntityField"))
        //            {

        //                discount.ObjectNumber = discount.ObjectNumber == null ? (discountdata1.Attributes("field").FirstOrDefault().Value == "ObjectNumber" ? discountdata1.Attributes("value").FirstOrDefault().Value : null) : discount.ObjectNumber;

        //                discount.Percentage = discount.Percentage == null ? (discountdata1.Attributes("field").FirstOrDefault().Value == "Percentage" ? discountdata1.Attributes("value").FirstOrDefault().Value : null) : discount.Percentage;

        //                discount.SalesCount = discount.SalesCount == null ? (discountdata1.Attributes("field").FirstOrDefault().Value == "SalesCount" ? discountdata1.Attributes("value").FirstOrDefault().Value : null) : discount.SalesCount;

        //                discount.Total = discount.Total == null ? (discountdata1.Attributes("field").FirstOrDefault().Value == "Total" ? discountdata1.Attributes("value").FirstOrDefault().Value : null) : discount.Total;

        //                discount.VoidLink = discount.VoidLink == null ? (discountdata1.Attributes("field").FirstOrDefault().Value == "VoidLink" ? discountdata1.Attributes("value").FirstOrDefault().Value : null) : discount.VoidLink;

        //                discount.ItemDsc = discount.ItemDsc == null ? (discountdata1.Attributes("field").FirstOrDefault().Value == "ItemDsc" ? discountdata1.Attributes("value").FirstOrDefault().Value : null) : discount.ItemDsc;

        //                discount.Name = discount.Name == null ? (discountdata1.Attributes("field").FirstOrDefault().Value == "Name" ? discountdata1.Attributes("value").FirstOrDefault().Value : null) : discount.Name;

        //                discount.Void = discount.Void == null ? (discountdata1.Attributes("field").FirstOrDefault().Value == "Void" ? discountdata1.Attributes("value").FirstOrDefault().Value : null) : discount.Void;

        //                discount.LineNumVoid = discount.LineNumVoid == null ? (discountdata1.Attributes("field").FirstOrDefault().Value == "LineNumVoid" ? discountdata.Descendants("OraPayloadEntityField").Attributes("value").FirstOrDefault().Value : null) : discount.LineNumVoid;


        //                //var a = discountdata.Descendants("ValueList").Descendants("GenericParameterList").Descendants("OraPayloadEntityFieldGenericParameter").ToList();

        //                //foreach (var menuItemDetailLinksdata in discountdata.Descendants("ValueList").Descendants("GenericParameterList").Descendants("OraPayloadEntityFieldGenericParameter").ToList())
        //                //{
        //                //    MenuItemDetailLinks menuItemDetailLinks = new MenuItemDetailLinks();
        //                //    menuItemDetailLinks.DetailedLinkedId = menuItemDetailLinksdata.Attributes("value").FirstOrDefault().Value;
        //                //    discount.MenuItemDetailLinks.Add(menuItemDetailLinks);
        //                //}
        //            }




        //            Shymphony.Discount.Add(discount);
        //        }

        //        #endregion

        //        #region Binding ServiceChargeList
        //        var serviceChargeJsonResult = JsonConvert.SerializeObject(ServiceCharge);

        //        foreach (var serviceChargedata in ServiceCharge)
        //        {
        //            ServiceCharge serviceCharge = new ServiceCharge();
        //            foreach (var serviceChargedata1 in serviceChargedata.Descendants("OraPayloadEntityField").ToList())
        //            {
        //                serviceCharge.ObjectNumber = serviceCharge.ObjectNumber == null ? (serviceChargedata1.Attributes("field").FirstOrDefault().Value == "ObjectNumber" ? serviceChargedata1.Attributes("value").FirstOrDefault().Value : null) : serviceCharge.ObjectNumber;

        //                serviceCharge.Percentage = serviceCharge.Percentage == null ? (serviceChargedata1.Attributes("field").FirstOrDefault().Value == "Percentage" ? serviceChargedata1.Attributes("value").FirstOrDefault().Value : null) : serviceCharge.Percentage;

        //                serviceCharge.SalesCount = serviceCharge.SalesCount == null ? (serviceChargedata1.Attributes("field").FirstOrDefault().Value == "SalesCount" ? serviceChargedata1.Attributes("value").FirstOrDefault().Value : null) : serviceCharge.SalesCount;

        //                serviceCharge.Total = serviceCharge.Total == null ? (serviceChargedata1.Attributes("field").FirstOrDefault().Value == "Total" ? serviceChargedata1.Attributes("value").FirstOrDefault().Value : null) : serviceCharge.Total;

        //                serviceCharge.VoidLink = serviceCharge.VoidLink == null ? (serviceChargedata1.Attributes("field").FirstOrDefault().Value == "VoidLink" ? serviceChargedata1.Attributes("value").FirstOrDefault().Value : null) : serviceCharge.VoidLink;

        //                serviceCharge.DetailLink = serviceCharge.DetailLink == null ? (serviceChargedata1.Attributes("field").FirstOrDefault().Value == "DetailLink" ? serviceChargedata1.Attributes("value").FirstOrDefault().Value : null) : serviceCharge.DetailLink;

        //                serviceCharge.Name = serviceCharge.Name == null ? (serviceChargedata1.Attributes("field").FirstOrDefault().Value == "Name" ? serviceChargedata1.Attributes("value").FirstOrDefault().Value : null) : serviceCharge.Name;

        //                serviceCharge.Void = serviceCharge.Void == null ? (serviceChargedata1.Attributes("field").FirstOrDefault().Value == "Void" ? serviceChargedata1.Attributes("value").FirstOrDefault().Value : null) : serviceCharge.Void;

        //                serviceCharge.LineNumVoid = serviceCharge.LineNumVoid == null ? (serviceChargedata1.Attributes("field").FirstOrDefault().Value == "LineNumVoid" ? serviceChargedata1.Attributes("value").FirstOrDefault().Value : null) : serviceCharge.LineNumVoid;
        //            }
        //            Shymphony.ServiceCharge.Add(serviceCharge);
        //        }

        //        #endregion

        //        #region Binding TenderMediaList

        //        foreach (var tenderMediadata in TenderMedia)
        //        {
        //            TenderMedia tenderMedia = new TenderMedia();
        //            foreach (var tenderMediadata1 in tenderMediadata.Descendants("OraPayloadEntityField").ToList())
        //            {
        //                tenderMedia.ObjectNumber = tenderMedia.ObjectNumber == null ? (tenderMediadata1.Attributes("field").FirstOrDefault().Value == "ObjectNumber" ? tenderMediadata1.Attributes("value").FirstOrDefault().Value : null) : tenderMedia.ObjectNumber;

        //                tenderMedia.CurrencyAmount = tenderMedia.CurrencyAmount == null ? (tenderMediadata1.Attributes("field").FirstOrDefault().Value == "CurrencyAmount" ? tenderMediadata1.Attributes("value").FirstOrDefault().Value : null) : tenderMedia.CurrencyAmount;

        //                tenderMedia.SalesCount = tenderMedia.SalesCount == null ? (tenderMediadata1.Attributes("field").FirstOrDefault().Value == "SalesCount" ? tenderMediadata1.Attributes("value").FirstOrDefault().Value : null) : tenderMedia.SalesCount;

        //                tenderMedia.Total = tenderMedia.Total == null ? (tenderMediadata1.Attributes("field").FirstOrDefault().Value == "Total" ? tenderMediadata1.Attributes("value").FirstOrDefault().Value : null) : tenderMedia.Total;

        //                tenderMedia.VoidLink = tenderMedia.VoidLink == null ? (tenderMediadata1.Attributes("field").FirstOrDefault().Value == "VoidLink" ? tenderMediadata1.Attributes("value").FirstOrDefault().Value : null) : tenderMedia.VoidLink;

        //                tenderMedia.CurrencyID = tenderMedia.CurrencyID == null ? (tenderMediadata1.Attributes("field").FirstOrDefault().Value == "CurrencyID" ? tenderMediadata1.Attributes("value").FirstOrDefault().Value : null) : tenderMedia.CurrencyID;

        //                tenderMedia.Name = tenderMedia.Name == null ? (tenderMediadata1.Attributes("field").FirstOrDefault().Value == "Name" ? tenderMediadata1.Attributes("value").FirstOrDefault().Value : null) : tenderMedia.Name;

        //                tenderMedia.Void = tenderMedia.Void == null ? (tenderMediadata1.Attributes("field").FirstOrDefault().Value == "Void" ? tenderMediadata1.Attributes("value").FirstOrDefault().Value : null) : tenderMedia.Void;

        //                tenderMedia.LineNumVoid = tenderMedia.LineNumVoid == null ? (tenderMediadata1.Attributes("field").FirstOrDefault().Value == "LineNumVoid" ? tenderMediadata1.Attributes("value").FirstOrDefault().Value : null) : tenderMedia.LineNumVoid;

        //                tenderMedia.FIP_LAD_PAYLOAD_CR_TMED_CODES = tenderMedia.FIP_LAD_PAYLOAD_CR_TMED_CODES == null ? (tenderMediadata1.Attributes("field").FirstOrDefault().Value == "FIP_LAD_PAYLOAD_CR_TMED_CODES" ? tenderMediadata1.Attributes("value").FirstOrDefault().Value : null) : tenderMedia.FIP_LAD_PAYLOAD_CR_TMED_CODES;
        //            }

        //            Shymphony.TenderMedia.Add(tenderMedia);
        //        }

        //        #endregion
        //    }


        //    using (TransactionScope scope = new TransactionScope())
        //    {

        //        this._hotelogixService.InsertInvoiceRecord(Shymphony, formCode);
        //        scope.Complete();


        //    }



        //    return Ok(Shymphony);


        //}

    }
}