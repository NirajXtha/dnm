using NeoErp.Core.Helpers;
using NeoErp.LOC.Services.Models;
using NeoErp.LOC.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoERP.LOC.Controllers.Api
{

    public class PurchaseOrderController : ApiController
    {
        private IPurchaseOrder _purchaseorder { get; set; }
        public PurchaseOrderController(IPurchaseOrder purchaseorder)
        {
            _purchaseorder = purchaseorder;
        }


        public IHttpActionResult getCurrency()
        {
            try
            {
                var list = _purchaseorder.GetAllCurrency();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        public List<ItemDetails> getItemDetailsByTrackOrderNo(string OrderCode)
        {
            return _purchaseorder.GetAllItemsByTrackOrderNo(OrderCode);
        }

        public List<ItemDetail> getAllItemsList(string filter)
        {
            return _purchaseorder.GetAllItemsLists(filter);
        }

        public List<ItemDetail> getAllMuCodeList(string filter, string itemcode)
        {
            return _purchaseorder.GetAllMuCodeLists(filter, itemcode);
        }

        [HttpGet]
        public List<PurchaseOrderModels> getAllPurchaseOrders()
        {
            return _purchaseorder.GetAllPurchaseOrders();
        }

        [HttpGet]
        public List<PurchaseOrderModels> getAllPurchaseOrdersFilter(string purchaseOrder, string beneficiaryname, string orderdate)
        {
            return _purchaseorder.getAllPurchaseOrdersFilter(purchaseOrder, beneficiaryname, orderdate);
        }

     

        public List<Items> getAllPOItemsList(string filter)
        {

            return _purchaseorder.GetAllPOItemsLists(filter);
        }

        public IHttpActionResult getAllPOHistoryItemsList(string lctrackno)
        {
            try
            {
                var list = _purchaseorder.GetAllPOHistoryItemsLists(lctrackno);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        public IHttpActionResult getAllPOHistoryShipmentList(string lctrackno)
        {
            try
            {
                var list = _purchaseorder.GetAllPOHistoryShipmentList(lctrackno);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        public IHttpActionResult getAllPOHistoryDocumentList(string lctrackno)
        {
            try
            {
                var list = _purchaseorder.GetAllPOHistoryDocumentList(lctrackno);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult deletePO(string trackno, string sno)
        {
            try
            {
                _purchaseorder.deletePO(trackno, sno);
                return Ok();
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }


        public List<IpPurchaseOrderModels> getAllIpPurchaseOrder(string filter)
        {

            return _purchaseorder.GetAllIpPurchaseOrders(filter);
        }

        public List<IpPurchaseOrderModels> getAllIpPurchaseOrderfilter(string filter)
        {

            return _purchaseorder.GetAllIpPurchaseOrdersfilter(filter);
        }
        
        public List<ShipmentModels> getAllPOShipmentList(string filter)
        {

            return _purchaseorder.GetAllPOShipmentLists(filter);
        }

        [HttpPost]
        public string RemovePOImage(LcImageModels imageremovedetails)
        {

            var result = _purchaseorder.RemovePoImage(imageremovedetails);
            return result;
        }

        [HttpPost]
        public IHttpActionResult CreatePurchaseOrder(PurchaseOrderModels podetails)
        {
            try
            {
                var ordernumberexist = "";
                var weekNumber =podetails.WEEK_NUMBER;
                if (podetails.LC_TRACK_NO == 0)
                {
                    ordernumberexist = _purchaseorder.OrderNumberExist(podetails.ORDER_NO, "create", podetails.PO_CODE);
                }
                else
                {
                    ordernumberexist = _purchaseorder.OrderNumberExist(podetails.ORDER_NO, "edit", podetails.PO_CODE);
                }
                if (ordernumberexist == null)
                {
                    var list = _purchaseorder.CreatePurchaseOrder(podetails);
                    return Ok(list);
                }
                else
                {
                    return Content(HttpStatusCode.NotModified, "Order Number Already Exist.");
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage CreateItems(List<Items> itemsdetail)
        {
            bool result;
            try
            {
                 result = _purchaseorder.CreateItems(itemsdetail);
                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK});
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }

              
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpPost]
        public HttpResponseMessage UpdateItems(List<Items> itemsdetail)
        {

            bool result;
            try
            {
                result = _purchaseorder.UpdateItems(itemsdetail);
                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }


        public POModel getPurchaseOrderdateandsupplierByOrderCode(string OrderCode)
        {
            return _purchaseorder.getPurchaseOrderdateandsupplierByOrderCode(OrderCode);
        }


        #region Commented Lines
        //public List<ItemDetail> getAllShipmentItemsList(string filter, string lctrack)
        //{
        //    return _purchaseorder.GetAllShipmentItemsLists(filter, lctrack);
        //}
        //[HttpPost]
        //public HttpResponseMessage CreateShipments(ShipmentModels shipmentdetails)
        //{
        //    try
        //    {
        //        var result = _purchaseorder.CreateShipment(shipmentdetails);
        //        return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
        //    }
        //}
        #endregion
    }
}
