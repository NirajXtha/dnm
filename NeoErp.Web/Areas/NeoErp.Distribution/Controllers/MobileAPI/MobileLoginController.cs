using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoErp.Distribution.Controllers.MobileAPI
{
    [RoutePrefix("api/MobileLogin/")]
    public class MobileLoginController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage login(string username,string password)
        {
            if (username == "admin" && password == "admin@ITNepal")
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "06" });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "InValid Login!" });
        }        
    }
}