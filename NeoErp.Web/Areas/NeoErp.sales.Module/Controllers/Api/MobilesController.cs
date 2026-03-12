using NeoErp.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoErp.sales.Module.Controllers.Api
{
    public class MobilesController : ApiController
    {
        private IMobileService _mobileService;

        public MobilesController(IMobileService mobileService)
        {
            this._mobileService = mobileService;
        }

        // GET api/<controller>
        //public HttpResponseMessage GetVoucher()
        //{
        //    HttpResponseMessage response = new HttpResponseMessage();
        //    try
        //    {
        //        var vouchers = this._mobileService.GetVoucherDetails();
        //        if (vouchers.Count() > 0)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Voucher Successfully retrived", DATA = vouchers.ToList(), STATUS_CODE = (int)HttpStatusCode.OK });
        //        }
        //        return Request.CreateResponse(HttpStatusCode.NotFound, new { MESSAGE = "Voucher not found", statusCode = (int)HttpStatusCode.NotFound });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Error occured while processing the request - " + ex.Message, statusCode = (int)HttpStatusCode.InternalServerError });

        //    }
        //}
    }
}
