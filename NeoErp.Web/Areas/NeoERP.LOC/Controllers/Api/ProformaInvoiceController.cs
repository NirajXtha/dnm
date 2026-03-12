using NeoErp.LOC.Services.Models;
using NeoErp.LOC.Services.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using static NeoERP.LOC.Models.EnumList;

namespace NeoERP.LOC.Controllers.Api
{
    public class ProformaInvoiceController : ApiController
    {
        private ILOCService _locService { get; set; }
        private IPerfomaInvoice _perfomaInvoice { get; set; }

        public ProformaInvoiceController(ILOCService locService, IPerfomaInvoice perfomaInvoice)
        {
            this._locService = locService;
            this._perfomaInvoice = perfomaInvoice;
        }

        [HttpGet]
        public List<PerformaInvoiceModel> getAllPerfomaInvoice()
        {
            return _perfomaInvoice.GetAllPerfomaInvoice();
        }

        [HttpGet]
        public List<PerformaInvoiceModel> getAllPerfomaInvoiceFilter(string purchaseOrder, string pinvoiceno, string pinvoicedate)
        {
            return _perfomaInvoice.getAllPerfomaInvoiceFilter(purchaseOrder, pinvoiceno, pinvoicedate);
        }
     
        public List<ItemDetails> getItemDetailsByOrderCode(string OrderCode)
        {
            return _locService.GetAllItemsByOrderCode(OrderCode);
        }

 

        

        [HttpGet]
        public IHttpActionResult deletePI(string trackno, string sno)
        {
            try
            {
                _perfomaInvoice.deletePI(trackno, sno);
                return Ok();
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult PendingItemList()
        {
            try
            {
                var result = _perfomaInvoice.PendingItemsList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }



        public List<PerformaInvoiceModel> getAllLcIpPurchaseOrder(string filter)
        {
            return _perfomaInvoice.GetAllLcIpPurchaseOrder(filter);
        }
        public List<PerformaInvoiceModel> getAllIpPurchaseOrderfilter(string filter)
        {
            return _perfomaInvoice.getAllIpPurchaseOrderfilter(filter);
        }

        

        public List<PerformaInvoiceModel> getAllIpipPurchaseInvoicefilter(string filter)
        {

            return _perfomaInvoice.getAllIpipPurchaseInvoicefilter(filter);

        }
        public List<ItemDetails> getItemDetailsByTrackOrderNo(string OrderCode)
        {
            return _perfomaInvoice.GetAllItemsByTrackOrderNo(OrderCode);
        }

        public List<CountryModels> getCountryCodes(string filter)
        {
            return _locService.GetAllCountry(filter);
        }

        public List<SupplierModel> GetAllSuppliers(string filter)
        {
            return _locService.GetAllSuppliersByFilter(filter);
        }

        public List<HSModels> getHsCodes(string filter)
        {
            return _locService.GetAllHsCodes(filter);
        }

        public List<BeneficiaryModels> getBeneficiary(string filter)
        {
            return _locService.GetAllBeneficiary(filter);
        }

        [HttpGet]
        public List<string> GetShipmentType()
        {
            var shipmentType = Enum.GetNames(typeof(ShipmentType)).ToList();
            return shipmentType;
        }
       

        [HttpGet]
        public List<string> GetLoadShipmentType()
        {
            var shipmentLoadType = Enum.GetNames(typeof(ShipmentLoadType)).ToList();
            return shipmentLoadType;
        }

        [HttpPost]
        public string RemovePOImage(LcImageModels imageremovedetails)
        {

            var result = _perfomaInvoice.RemovePoImage(imageremovedetails);
            return result;
        }


        [HttpPost]
        public IHttpActionResult CreatePerfomaInvoice(PerformaInvoiceModel perfomadetail)
        {
            try
            {
                string Message = "";
                bool proformanumberexist;
                if (perfomadetail.LC_TRACK_NO == 0)
                {

                    proformanumberexist = _perfomaInvoice.ProformaNumberExist(perfomadetail.PINVOICE_NO, "create", perfomadetail.PINVOICE_CODE, perfomadetail.ORDER_NO, out Message);
                }
                else
                {
                    proformanumberexist = _perfomaInvoice.ProformaNumberExist(perfomadetail.PINVOICE_NO, "edit", perfomadetail.PINVOICE_CODE, perfomadetail.ORDER_NO, out Message);
                }
                if (proformanumberexist == false)
                {
                    var result = _perfomaInvoice.CreatePerfomaInvoice(perfomadetail);
                     return Ok(result);
                }
                else
                {
                    return Ok(Message);
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }


        //[HttpPost]
        //public HttpResponseMessage CreateItems(List<Items> itemsdetail)
        //{

        //    bool result;
        //    try
        //    {
        //        result = _perfomaInvoice.CreateItems(itemsdetail);
        //        if (result)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
        //        }
        //        else
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
        //        }
        //  }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
        //    }
        //}

        //[HttpPost]
        //public HttpResponseMessage UpdateItems(List<Items> itemsdetail)
        //{
        //    bool result;
        //    try
        //    {
        //        result = _perfomaInvoice.UpdateItems(itemsdetail);
        //        if (result)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
        //        }
        //        else
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
        //    }
        //}
    }
}
