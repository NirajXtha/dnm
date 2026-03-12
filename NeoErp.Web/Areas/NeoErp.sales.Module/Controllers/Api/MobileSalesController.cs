using NeoErp.Core.Helpers;
using NeoErp.Core.Domain;
using NeoErp.Sales.Modules.Services.Models.SalesDashBoard;
using NeoErp.Sales.Modules.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace NeoErp.sales.Module.Controllers.Api
{
    public class MobileSalesController : ApiController
    {
        private ISalesDashboardService _salesService;
        private ISalesRegister _salesRegister;

        public MobileSalesController(ISalesDashboardService salesService, ISalesRegister salesRegister)
        {
            this._salesService = salesService;
            this._salesRegister = salesRegister;
        }
        [HttpPost]
        public HttpResponseMessage GetSalesMonthlyReport([ModelBinder]filterOption model, string DateFormat, string customerCode , string itemCode="", string categoryCode="", string companyCode="", string branchCode="", string partyTypeCode="", string divisionCode="", string formCode="")
        {
            try
            {
                var user = new User();
                user.company_code = companyCode==null?"01": companyCode;
                user.branch_code = branchCode==null?"01.01" : branchCode;
                
                if (model == null)
                {
                    return Request.CreateResponse(HttpStatusCode.ExpectationFailed, new { STATUS_CODE = (int)HttpStatusCode.ExpectationFailed, MESSAGE = "Required parameter not supplied" });
                }

                var monthlySales = this._salesService.GetSalesMonthSummanryMobile(model.ReportFilters, user, DateFormat, customerCode = null, itemCode = null, categoryCode = null, companyCode = null, branchCode, partyTypeCode = null,divisionCode, formCode = null);
                if (monthlySales.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Monthly sales Successfully retrived", DATA = monthlySales.ToList(), STATUS_CODE = (int)HttpStatusCode.OK });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Monthly Sales not found.", STATUS_CODE = (int)HttpStatusCode.NotFound });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });

            }

        }

    }
}
