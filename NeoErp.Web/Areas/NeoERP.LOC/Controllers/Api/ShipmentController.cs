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
    public class ShipmentController : ApiController
    {
        private IShipmentService _shipmentservice;
        public ShipmentController(IShipmentService shipmentservice)
        {
            _shipmentservice = shipmentservice;
        }
        public IHttpActionResult getShipmentDetailsByPinvoice(string PinvoiceCode)
        {
            try
            {
                var list = _shipmentservice.getShipmentlistbyTrackNo(PinvoiceCode);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult CreateShipment(ShipmentModels sidetails)
        {
            try
            {
                _shipmentservice.createShipment(sidetails);
                return Ok();
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        public IHttpActionResult getAllLcIpPurchaseOrder(string filter)
        {
            try
            {
                var result = _shipmentservice.GetAllLcIpPurchaseOrder(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        public IHttpActionResult GetAllLcIpPurchaseOrderfilter(string filter)
        {
            try
            {
                var result = _shipmentservice.GetAllLcIpPurchaseOrderfilter(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        
        public IHttpActionResult GetShipmentDetailsbySno(int sno, string lctrackno)
        {
            try
            {
               var result =  _shipmentservice.getShipmentBySno(sno,lctrackno);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        
    }
}
