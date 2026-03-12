using NeoErp.Core;
using NeoErp.Core.Helpers;
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
    [RoutePrefix("api/MobilePO/")]
    public class MobilePOController : ApiController
    {
        public IMobilePOservice _mobilePoservice;
        public MobilePOController(IMobilePOservice mobilePOservice)
        {
            _mobilePoservice = mobilePOservice;
        }

        [HttpPost]
        public List<SalesPersonPoModel> GetSalesPersonlst(string toDate, string fromDate, string salePerson, string orderNo = null,string resellerCode = null)
        {
            List<int> list = new List<int>();
            return _mobilePoservice.GetSalesPersonlst(toDate, fromDate, salePerson, orderNo, resellerCode);
        }

        [HttpPost]
        public HttpResponseMessage ApprovePurchaseOrder(string orderNo = null,string itemCode = null,string Type="")
        {
            string message = string.Empty;
            bool status = false;
            _mobilePoservice.ApprovePurchaseOrder(orderNo, itemCode, out message, out status);
            return Request.CreateResponse(HttpStatusCode.OK, new { message, status });
        }
        [HttpPost]
        public HttpResponseMessage RejectPurchaseOrder(string orderNo = null, string itemCode = null)
        {
            string message = string.Empty;
            bool status = false;
            _mobilePoservice.RejectPurchaseOrder(orderNo, itemCode, out message, out status);
            return Request.CreateResponse(HttpStatusCode.OK, new { message, status });
        }
    }
    
}