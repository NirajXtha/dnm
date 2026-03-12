using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoErp.Controllers.Api
{
    public class TestController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage GetContent()
        {


            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, "content");
            return response;
        }
    }
}
