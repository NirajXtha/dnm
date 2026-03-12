using NeoErp.LOC.Services.Interfaces;
using NeoErp.LOC.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static NeoERP.LOC.Models.EnumList;

namespace NeoERP.LOC.Controllers.Api
{
    public class LogisticsController : ApiController
    {
        private ILogisticsService _logisticService;
        
        public LogisticsController(ILogisticsService logisticService)
        {
            this._logisticService = logisticService;
        }

        public IHttpActionResult GetAllLocations()
        {
            return Ok(_logisticService.GetAllLocations());
        }

        [HttpGet]
        public List<string> GetShipmentType()
        {
            var shipmentType = Enum.GetNames(typeof(ShipmentType)).ToList();
            return shipmentType;
        }

        public IHttpActionResult GetAllContractor()
        {
            return Ok(_logisticService.GetAllContractor());
        }

        public IHttpActionResult GetAllClearingAgent()
        {
            return Ok(_logisticService.GetAllClearingAgent());
        }
        public IHttpActionResult GetLogisticPlanContainerDetailByShipmentType( string shipmentType,string InvoiceNo,string create_edit,string Logistic_Detail_Code)
        {
            return Ok(_logisticService.GetLogisticPlanContainerDetailByShipmentType(shipmentType, InvoiceNo,create_edit, Logistic_Detail_Code));
        }
     
        public IHttpActionResult GetAllIssuingCarrier()
        {
            return Ok(_logisticService.GetAllIssuingCarrier());
        }
        public IHttpActionResult GetAllContainer()
        {
            return Ok(_logisticService.GetAllContainer());
        }
        public IHttpActionResult GetAllDocuments()
        {
            return Ok(_logisticService.GetAllDocuments());
        }
        [HttpGet]
        public IHttpActionResult  IsAir(string PinvoiceCode)
        {
            return Ok(_logisticService.IsAir(PinvoiceCode));
        }
       
        [HttpGet]
        public List<string> GetDocActionType()
        {
            var docActionType = Enum.GetNames(typeof(DocumentActionType)).ToList();
            return docActionType;
        }

        [HttpPost]
        public IHttpActionResult CreateLogistics(LogisticsModels logisticsdetail)
        {

            try
            {
                string sno = "";
                string cinumber = "";
                _logisticService.CreateLogistics(logisticsdetail, out sno, out cinumber);
                return Content(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, SerialNo = sno, InvoiceNo = cinumber});

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }


        public IHttpActionResult GetLogisticDetailsByLcTrackNo(string lctrackno,string invoiceno)
        {
            try
            {
                return Ok(_logisticService.GetAllLogisticDetails(lctrackno,invoiceno));
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }

        }

        [HttpGet]
        public List<Logistic> getAllLogistic()        
        {
            return _logisticService.GetAllLogistics();
        }
        [HttpGet]
        public Logisticlocationdetail GetLogisticETAByInvLocationCode(string invoiceno, string locationcode)
        {
            return _logisticService.GetLogisticETAByInvLocationCode(invoiceno, locationcode);
        }

        [HttpGet]
        public string CheckLogisticTolocation(string invoiceno, string locationcode)
        {
            return _logisticService.CheckLogisticTolocation(invoiceno, locationcode);
        }
        

          [HttpGet]
        public List<Logistic> getAllLogisticFilter(string invoicenumber)
        {
            return _logisticService.getAllLogisticFilter(invoicenumber);
        }

        [HttpGet]
        public List<LogisticsModels> getAllLogisticShipmentList(string lctrack,string invoice)
        {
            return _logisticService.GetAlLLogisticShipmentLists(lctrack,invoice);
        }
        [HttpGet]
        public IHttpActionResult GetLogisticShipmentDetailsByLCode(string lctrackno, string invoiceno,string logicticcode)
        {
            try
            {
                return Ok(_logisticService.GetAllLogisticShipmentDetailsByLogisticCode(lctrackno, invoiceno, logicticcode));
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }

        }

        public IHttpActionResult RemoveLogisticImage(RemoveLogisticImages imageremovedetails)
        {
            try
            {
                _logisticService.RemoveLogisticImages(imageremovedetails);
                return Ok();
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }
        //
        public IHttpActionResult getAllLogisticsHistoryList(string lctrackno)
        {
            try
            {
                var list = _logisticService.getAllLogisticsHistoryList(lctrackno);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }
        //
        public IHttpActionResult getAllLogisticsDocumentHistoryList(string lctrackno)
        {
            try
            {
                var list = _logisticService.getAllLogisticsDocumentHistoryList(lctrackno);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        public IHttpActionResult getAllLogisticsContainerHistoryList(string lctrackno)
        {
            try
            {
                var list = _logisticService.getAllLogisticsContainerHistoryList(lctrackno);
                return Ok(list);
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
                var result = _logisticService.GetAllLcIpPurchaseOrder(filter);
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
                var result = _logisticService.GetAllLcIpPurchaseOrderfilter(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        public IHttpActionResult getShipmentDetailsByPinvoice(string PinvoiceCode)
        {
            try
            {
                var list = _logisticService.getShipmentlistbyTrackNo(PinvoiceCode);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

    }
}
