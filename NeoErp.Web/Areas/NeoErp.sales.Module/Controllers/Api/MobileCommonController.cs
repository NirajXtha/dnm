using NeoErp.Core;
using NeoErp.Core.Domain;
using NeoErp.Core.Models.CustomModels;
using NeoErp.Core.Services;
using NeoErp.Sales.Modules.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoErp.sales.Module.Controllers.Api
{
    public class MobileCommonController : ApiController
    {
        private IControlService _controlService;
        private ISalesRegister _salesRegister;
        private IWorkContext _workContext;
        private IAuthenticationService _authenticationService;
        public MobileCommonController(IControlService controlService, ISalesRegister salesRegister, IWorkContext workContext, IAuthenticationService authenticationService)
        {
            this._controlService = controlService;
            this._salesRegister = salesRegister;
            this._workContext = workContext;
            this._authenticationService = authenticationService;
        }

        //[HttpPost]
        //public HttpResponseMessage UpdateCompanyCode(string userId, string companyCode, string branchCode)
        //{
        //    User userData = new User();
        //    try
        //    {
        //        userData.User_id = Convert.ToInt32(userId);
        //        userData.company_code = companyCode;
        //        userData.branch_code = branchCode;
        //        _workContext.CurrentUserinformation = userData;
        //        _authenticationService.UpdateAuthenticatedCustomer(userData);
        //        return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Company Code updated successfully.", STATUS_CODE = (int)HttpStatusCode.OK });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });

        //    }
        //}

        [HttpPost]
        public HttpResponseMessage GetDateFilters(string fiscalYear="", string textToAppend = "", bool appendText = false)
        {
            try
            {
                var FincalYear = System.Configuration.ConfigurationManager.AppSettings["FiscalYear"].ToString();
                var result = this._controlService.GetDateFilters(FincalYear, textToAppend, appendText).OrderByDescending(q => q.SortOrder).ToList();
                if (result.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Date type Successfully retrived", DATA = result.ToList(), STATUS_CODE = (int)HttpStatusCode.OK });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Date type not found.", STATUS_CODE = (int)HttpStatusCode.NotFound });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });

            }
        }

        [HttpPost]
        public HttpResponseMessage GetCustomerDetail(string companyCode, string branchCode)
        {
            try
            {
                var result = this._salesRegister.SaleRegisterCustomers(companyCode, branchCode);
                if (result.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Customer detail Successfully retrived", DATA = result.ToList(), STATUS_CODE = (int)HttpStatusCode.OK });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Customer not found.", STATUS_CODE = (int)HttpStatusCode.NotFound });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });

            }
        }

        [HttpPost]
        public HttpResponseMessage GetSupplierDetail(string companyCode, string branchCode)
        {
            try
            {
                var result = this._salesRegister.SaleRegisterSuppliers(companyCode, branchCode);
                if (result.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Suppliers detail Successfully retrived", DATA = result.ToList(), STATUS_CODE = (int)HttpStatusCode.OK });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Supplier not found.", STATUS_CODE = (int)HttpStatusCode.NotFound });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });

            }
        }


    }
}
