using NeoErp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoErp.Controllers.Api
{
    public class CRMMobileController : ApiController
    {
        private ICRMMobileService _icrmmobileService { get; set; }
        public CRMMobileController(ICRMMobileService icrmmobileService)
        {
            this._icrmmobileService = icrmmobileService;
    }
        [HttpPost]
        public HttpResponseMessage GetCRMDetail()
        {
            try
            {
                var result = this._icrmmobileService.GetCRMData();
                if (result.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "CRM detail Successfully retrived", DATA = result.ToList(), STATUS_CODE = (int)HttpStatusCode.OK });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "CRM not found.", STATUS_CODE = (int)HttpStatusCode.NotFound });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });

            }
        }

        [HttpGet]
        public HttpResponseMessage GetCRMDetails()
        {
            try
            {
                var result = this._icrmmobileService.GetCRMData();
                if (result.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "CRM detail Successfully retrived", DATA = result.ToList(), STATUS_CODE = (int)HttpStatusCode.OK });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "CRM not found.", STATUS_CODE = (int)HttpStatusCode.NotFound });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });

            }
        }
    }
}
