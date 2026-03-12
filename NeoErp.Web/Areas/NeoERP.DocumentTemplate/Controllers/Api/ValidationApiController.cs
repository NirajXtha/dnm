using NeoERP.DocumentTemplate.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoERP.DocumentTemplate.Controllers.Api
{
    public class ValidationApiController : ApiController
    {
        private IValidationRepo _validaterepo;
        public ValidationApiController(IValidationRepo validaterepo)
        {
            _validaterepo = validaterepo;
        }

        [HttpGet]
        public HttpResponseMessage ValidateCreditLimit(string FormCode = "166", string customercode = "1001", decimal totalamount = 50000)
        {

            decimal DrCrTotal = 0;
            decimal actualbalance = 0;
            string message = "";
            bool creditlimit = _validaterepo.ValidateCreditLimiCustomerWise(FormCode, customercode, totalamount, true, out DrCrTotal, out actualbalance, "01");

            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = message, STATUS_CODE = (int)HttpStatusCode.OK, DrCrTotal = DrCrTotal, CreditLimit = creditlimit });

        }
    }
}
