using NeoErp.Core;
using NeoErp.Distribution.Service.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.IO;

using System.Web.Http;

namespace NeoErp.Distribution.Controllers.Api
{
    public class QuickSetupController : ApiController
    {
        private IWorkContext _workContext;
        private IQuickSetupService _service;
        public QuickSetupController(IWorkContext workContext,IQuickSetupService service)
        {
            _workContext = workContext;
            _service = service;
        }

        [HttpGet]
        public HttpResponseMessage DistributorResellerMapping()
        {
            var result = _service.CreateDistResellerMap(_workContext.CurrentUserinformation);
            if (result == "200")
                return Request.CreateResponse(HttpStatusCode.OK, new { TYPE = "success", MESSAGE = "Successful" });

            return Request.CreateResponse(HttpStatusCode.OK, new { TYPE = "error", MESSAGE = "Something went wrong. Please try again later.", EXCEPTION = result });
        }

        [HttpGet]
        public HttpResponseMessage CreateEntities()
        {
            try
            {
                var Count = _service.createEntities();
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = $"{Math.Abs(Count)} Entities successfully created", TYPE = "success" });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, TYPE = "error" });
            }
        }


        [HttpPost]
        public HttpResponseMessage CreateDefaultValues()
        {
            try
            {
                var Count = _service.CreateDefaultValues();
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Default Values successfully created", TYPE = "success" });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, TYPE = "error" });
            }
        }


        [HttpGet]
        public HttpResponseMessage GetTroubleshoot()
        {
            try
            {
                var data = _service.GetTroubleshoot(_workContext.CurrentUserinformation);
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, TYPE = "error" });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetBrandItem()
        {
            try
            {
                var data = _service.GetBrandItem(_workContext.CurrentUserinformation);
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch(Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error Occured While Procesing the Request-" + ex.Message, TYPE = "Error" });
            }
        }
        [HttpPost]
        public HttpResponseMessage UpdateCreatedByReseller()
        {
            try
            {
                var Count = _service.UpdateCreatedByNameReseller(_workContext.CurrentUserinformation.company_code);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = $"{Count}", TYPE = "success" });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, TYPE = "error" });
            }
        }
        [HttpPost]
        public HttpResponseMessage RemoveDuplicateRoutes()
        {
            try
            {
                var Count = _service.RemoveDuplicateRoutes(_workContext.CurrentUserinformation.company_code);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = $"{Count}", TYPE = "success" });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, TYPE = "error" });
            }
        }
    }
}
