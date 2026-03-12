using NeoErp.LOC.Services.Interfaces;
using NeoErp.LOC.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Transactions;
using System.Web.Http;

namespace NeoERP.LOC.Controllers.Api
{
    public class LcLogisticPlanController : ApiController
    {
        private ILcLogisticPlanService _lcLogisticPlanService { get; set; }

        public LcLogisticPlanController(ILcLogisticPlanService lcLogisticPlanService)
        {
            this._lcLogisticPlanService = lcLogisticPlanService;

        }


        [HttpPost]
        public IHttpActionResult CreateLogisticPlan(LcLogisticPlanModel LcLogisticPlanModel)
        {
            var response = "";
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {

                    response = _lcLogisticPlanService.AddLogisticPlan(LcLogisticPlanModel);
                    scope.Complete();
                    return Ok();

                }

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }

        }

        public IHttpActionResult GetLCLogisticPlanItemsByLCNumber(string lcnumber)
        {
            try
            {
                var list = _lcLogisticPlanService.GetLCLogisticPlanItemsByLCNumber(lcnumber);
                return Ok(list);

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        public IHttpActionResult GetLogisticPlan(string lcnumber)
        {
            try
            {
                var list = _lcLogisticPlanService.GetLogisticPlanList(lcnumber);
                return Ok(list);

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        public IHttpActionResult GetAllLcIpPurchaseOrder(string filters)
        {
            try
            {
                var list = _lcLogisticPlanService.GetAllLcNumbers(filters);
                return Ok(list);

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        public IHttpActionResult GetAllLcIpPurchaseOrderfilter(string filters)
        {
            try
            {
                var list = _lcLogisticPlanService.GetAllLcNumbersfilter(filters);
                return Ok(list);

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }
        
        public IHttpActionResult getAllLogisticPLanItemsList(string LOT_NO, string lcnumber)
        {
            try
            {
                var list = _lcLogisticPlanService.GetLCLogisticPlanItemsByLOT_NO(LOT_NO, lcnumber);
                return Ok(list);

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        public IHttpActionResult GetLogisticPlanbyperformainvoice(string PinvoiceCode)
        {
            try
            {
                var list = _lcLogisticPlanService.GetLogisticPlanbyperformainvoice(PinvoiceCode);
                return Ok(list);

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        #region lc logistic plan list
        public IHttpActionResult getAllLcLogisticPlan()
        {
            try
            {
                var list = _lcLogisticPlanService.getAllLcLogisticPlan();
                return Ok(list);

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }




        public IHttpActionResult getAllLcLogisticPlanFilter(string lcnumber)
        {
            try
            {
                var list = _lcLogisticPlanService.getAllLcLogisticPlanFilter(lcnumber);
                return Ok(list);

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }
        
        public IHttpActionResult getAllLcLogisticPlanItemListByTrackNumberAndLogisticPlanCode(string trackNumber, string logisticPlanCode)
        {
            try
            {
                var result = _lcLogisticPlanService.getAllLcLogisticPlanItemListByTrackNumberAndLogisticPlanCode(trackNumber, logisticPlanCode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        public IHttpActionResult getAllLcLogisticPlanContainerListByTrackNumberAndLogisticPlanCode(string trackNumber, string lotNumber)
        {
            try
            {
                var result = _lcLogisticPlanService.getAllLcLogisticPlanContainerListByTrackNumberAndLogisticPlanCode(trackNumber, lotNumber);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }
        

        public IHttpActionResult GetUpdateShipmentData(string LOGISTIC_PLAN_CODE)
        {
            try
            {
                var list = _lcLogisticPlanService.GetUpdateShipmentData(LOGISTIC_PLAN_CODE);
                return Ok(list);

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        //public IHttpActionResult UpdateQuantity(LC_LOGISTIC_PLANITEMLIST itemdetail)
        //{
        //    try
        //    {
        //        _lcLogisticPlanService.UpdateQuantity(itemdetail);
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return Content(HttpStatusCode.NotFound, ex.Message);
        //    }
        //}UpdateLogisticPlan

        [HttpPost]
        public HttpResponseMessage UpdateQuantity(LC_LOGISTIC_PLANITEMLIST itemdetail)
        {
            var result = _lcLogisticPlanService.UpdateQuantity(itemdetail);
            //if (result == "Exceeded")
            //{
            //    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "EXCEEDED", STATUS_CODE = (int)HttpStatusCode.OK });
            //}
            //else if(result == "Success")
            //{
            //    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "UPDATED", STATUS_CODE = (int)HttpStatusCode.OK });
            //}
            //else
            //{
            //    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "FAILED", STATUS_CODE = (int)HttpStatusCode.OK });
            //}
            try
            {
                if (result == "Exceeded")
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "EXCEEDED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else if (result == "Success")
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "UPDATED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "FAILED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
            }

            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        [HttpPost]
        public IHttpActionResult UpdateLogisticPlan(LC_LOGISTIC_PLANVVIEWMODEL LcLogisticPlanModel)
        {
            try
            {
                _lcLogisticPlanService.UpdateLogisticPlan(LcLogisticPlanModel);
                return Ok();
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        [HttpGet]
        public List<string> GetShipmentType()
        {
            var shipmentType = Enum.GetNames(typeof(Models.EnumList.LogisticPlan_ShipmentType)).ToList();
            return shipmentType;
        }

        #endregion

    }
}
