using NeoErp.Distribution.Service.Model;
using NeoErp.Distribution.Service.Service.Mobile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoErp.Distribution.Controllers.MobileAPI
{
    [RoutePrefix("api/MobileReseller/")]
    public class MobileResellerController : ApiController
    {
        public IMobileResellerService _ResellerService;
        public MobileResellerController(IMobileResellerService resellerService)
        {
            _ResellerService = resellerService;
        }
        [HttpPost]
        public HttpResponseMessage RegisterReseller(ResellerRegistration model)
        {
            string message = string.Empty;
            bool status = false;
            if (model != null)
            {               
                _ResellerService.RegisterReseller(model,out message,out status);
                return Request.CreateResponse(HttpStatusCode.OK, new { message,username = model.UserName, status });
            }
            else
            {
                if(model.UserName == null)
                {
                    return Request.CreateResponse(HttpStatusCode.ExpectationFailed, new { message = "Required parameter not supplied.",data = "empty", status });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.ExpectationFailed, new { message = "Required parameter not supplied.", username = model.UserName, status });
                }
            }
        }
        [HttpPost]
        public HttpResponseMessage ValidateLogin(string userName, string password)
        {
            try
            {
                string message = string.Empty;
                bool status = false;
                var user = _ResellerService.ValidateLogin(userName, password, out message, out status);
                if (user == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { message, status });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { message, data = user, status });
            }
            catch(Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Error occured while processing the request - " + ex.Message, status = false });
            }
        }
        public HttpResponseMessage GetResellerRegisteredList(string Id)
        {
           string message = string.Empty;
           bool status = false;
           var result = _ResellerService.GetResellerRegisteredList(Id, out message, out status);
           return Request.CreateResponse(HttpStatusCode.OK, new { message, data = result.ToList(), status });
        }
        public HttpResponseMessage ApproveRegisteredReseller(string Id)
        {
            string message = string.Empty;
            bool status = false;
            _ResellerService.ApproveRegisteredReseller(Id, out message, out status);
            return Request.CreateResponse(HttpStatusCode.OK,new { message, id = Id, status });
        }
        public HttpResponseMessage ChangePassword(string password, string userName)
        {
            string message = string.Empty;
            bool status = false;
            _ResellerService.ChangePassword(password, userName, out message, out status);
            return Request.CreateResponse(HttpStatusCode.OK, new { message, username =userName, status });                
        }
        [HttpGet]
        public HttpResponseMessage DeleteReseller(string id)
        {
            string message = string.Empty;
            bool status = false;
            _ResellerService.DeleteReseller(id, out message, out status);
            return Request.CreateResponse(HttpStatusCode.OK, new { message, id, status });
        }

        public HttpResponseMessage GetResellerImgLst(string resellerCode)
        {
            string message = string.Empty;
            bool status = false;
            var result = _ResellerService.GetResellerImgLst(resellerCode, out message, out status);
            return Request.CreateResponse(HttpStatusCode.OK, new { message, status, data = result.ToList()});
        }
    }
}