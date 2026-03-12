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
    public class GRNController : ApiController
    {
        private IGRNService _service { get; set; }
     
        public GRNController(IGRNService service)
        {
            this._service = service;
        }


        public IHttpActionResult getAllGRN()
        {
            try
            {
                var list = _service.getAllGRN();
                return Ok(list);

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult AddUpdateGRN(LcGrnModel lcRNModel)
        {
            var response = "";
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {

                    response = _service.AddUpdateGRN(lcRNModel);
                    scope.Complete();
                    return Ok(response);

                }

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }



        }


        // [HttpGet]
        //public IHttpActionResult LoadCommercialInvoiceInfo(string InvoiceCode,string PPDate)
        //{
        //    try
        //    {
        //        var list = _service.LoadCIInfo(InvoiceCode, PPDate);
        //        return Ok(list);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Content(HttpStatusCode.NotFound, ex.Message);
        //    }
        //}

        [HttpGet]
        public IHttpActionResult LoadCommercialInvoiceInfo(string InvoiceCode)
        {
            try
            {
                var list = _service.LoadCIInfo(InvoiceCode);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }
    }
}
