using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoERP.Planning.Controllers.Api
{
    public class EmployeeRoutePlanApiController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage getRoutes(string routecode)
        {
            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = string.Format("Table is Found"), DATA = "", STATUS_CODE = (int)HttpStatusCode.OK });
        }

       
    }
}
