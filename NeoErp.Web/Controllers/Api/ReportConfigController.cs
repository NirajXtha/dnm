using NeoErp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoErp.Controllers.Api
{
    public class ReportConfigController : ApiController
    {
        IReportConfig _reportConfig;
        public ReportConfigController(IReportConfig reportConfig)
        {
            this._reportConfig = reportConfig;
        }
        [HttpGet]
        public HttpResponseMessage GetUserLists()
        {
            try
            {
                var result = _reportConfig.GetAllUserLists();

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });

            }
        }

        [HttpPost]
        public HttpResponseMessage SaveReportConfig(string multipleuserId,string userName)
        {
            try
            {
                var result = _reportConfig.GetAllUserLists();

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });

            }
        }
    }
}
