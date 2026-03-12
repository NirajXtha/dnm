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
    public class CIPaymentSettlementController : ApiController
    {
        private ICIPaymentService _service { get; set; }
     
        public CIPaymentSettlementController(ICIPaymentService service)
        {
            this._service = service;
        }


        public IHttpActionResult getAllCISettlement()
        {
            try
            {
                var list = _service.getAllCISettlement();
                return Ok(list);

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

      

        [HttpPost]
        public IHttpActionResult AddUpdateCIPaymentSettlement(CIPaymentSettlementModel cIPaymentSettlementModel)
        {
            CIPaymentSettlementModel response = new CIPaymentSettlementModel();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {

                    response = _service.AddUpdateCIPaymentSettlement(cIPaymentSettlementModel);
                    scope.Complete();
                    return Ok(response);

                }

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }



        }

        [HttpPost]
        public IHttpActionResult RemoveCIPSImage(LcImageModels imageremovedetails)
        {
            try
            {
                _service.RemoveCIPSImage(imageremovedetails);
                return Ok();


            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        public IHttpActionResult getAllHistoryCIPaymentSettlementList(string lctrackno)
        {
            try
            {
                var list = _service.getAllHistoryCIPaymentSettlementList(lctrackno);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }


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
