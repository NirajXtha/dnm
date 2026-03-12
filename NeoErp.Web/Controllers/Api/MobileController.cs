using NeoErp.Core.Helpers;
using NeoErp.Core.Models.CustomModels;
using NeoErp.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace NeoErp.Controllers.Api
{
    public class MobileController : ApiController
    {
        private IMobileService _mobileService;
        private IAuthenticationService _authenticationService;
        public MobileController(IMobileService mobileService,IAuthenticationService authenticatioinService)
        {
            this._mobileService = mobileService;
            this._authenticationService = authenticatioinService;
        }

        // GET api/<controller>
        [HttpPost]
        public HttpResponseMessage GetVoucher([ModelBinder]GetVoucherModel model)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    var errors = GetModelStateErrors(ModelState);
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Required value not supplied", Data = errors, STATUS_CODE = (int)HttpStatusCode.ExpectationFailed });
                }
                
                if(!this._mobileService.UserExist(model.UserId, model.companyCode))
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "User Could not be authorized", STATUS_CODE = (int)HttpStatusCode.Unauthorized });

                var vouchers = this._mobileService.GetVoucherDetails(model.UserId, model.moduleCode, model.append, model.sessionRowId);
                if(vouchers.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,new { MESSAGE = "Voucher Successfully retrived", DATA = vouchers.ToList(), STATUS_CODE = (int)HttpStatusCode.OK } );
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Voucher not found", STATUS_CODE = (int)HttpStatusCode.NotFound });
            }
            catch(Exception ex) {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - "+ ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                
            }
        }

        [HttpPost]
        public HttpResponseMessage GetVoucherWithFlag([ModelBinder]GetVoucherModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = GetModelStateErrors(ModelState);
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Required value not supplied", Data = errors, STATUS_CODE = (int)HttpStatusCode.ExpectationFailed });
                }

                if (!this._mobileService.UserExist(model.UserId, model.companyCode))
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "User Could not be authorized", STATUS_CODE = (int)HttpStatusCode.Unauthorized });

                if (!string.IsNullOrEmpty(model.moduleCode) && model.moduleCode != "00")
                {
                    var permission = this._mobileService.GetModulePermission(model.UserId, model.companyCode);

                    if (permission.Count() == 0 || !permission.Any(q => q == model.moduleCode))
                        return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "User not authorized.",  STATUS_CODE = (int)HttpStatusCode.OK });
                }

                var vouchers = this._mobileService.GetVoucherWithFlag(model.UserId, model.companyCode, model.branchCode, model.moduleCode, model.append, model.sessionRowId);
                if (vouchers.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Voucher Successfully retrived", DATA = vouchers.ToList(), STATUS_CODE = (int)HttpStatusCode.OK });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Voucher not found.", STATUS_CODE = (int)HttpStatusCode.NotFound });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });

            }
        }

        [HttpPost]
        public HttpResponseMessage Login([ModelBinder]LoginModel model)
        {
            try
            {

                if(!ModelState.IsValid)
                {
                    var errors = GetModelStateErrors(ModelState);
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Required value not supplied", Data = errors, STATUS_CODE = (int)HttpStatusCode.ExpectationFailed });
                }

                string message = string.Empty;
                int statusCode = 0;

                var user = this._mobileService.LoginM(model.userName, model.password, out message, out statusCode);
                if (user == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = message, STATUS_CODE = statusCode });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = message, DATA=user, STATUS_CODE = statusCode});
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });

            }

        }

        [HttpPost]
        public HttpResponseMessage ListBranch(string UserName)
        {
            if(string.IsNullOrEmpty(UserName))
                  return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "UserName value not supplied", Data = "UserName value not supplied", STATUS_CODE = (int)HttpStatusCode.ExpectationFailed });
            var data = _authenticationService.GetCompanyBranchList(UserName);
            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Branch List Successfully retrived", DATA = data.ToList(), STATUS_CODE = (int)HttpStatusCode.OK });
        }

        //[HttpPost]
        //public HttpResponseMessage ListBranch(string acc_code,string company_code)
        //{
        //    if (string.IsNullOrEmpty(company_code))
        //        company_code = "01";
        //    if (string.IsNullOrEmpty(acc_code))
        //        return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "account value not supplied", Data = "account value not supplied", STATUS_CODE = (int)HttpStatusCode.ExpectationFailed });
        //    var data = _authenticationService.GetCompanyBranchList(UserName);
        //    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Branch List Successfully retrived", DATA = data.ToList(), STATUS_CODE = (int)HttpStatusCode.OK });
        //}
        [HttpPost]
        public HttpResponseMessage GetLedgerList(string CompanyCode, string branchCode)
        {
            if (string.IsNullOrEmpty(CompanyCode))
                CompanyCode = "01";
            var data = _mobileService.AccountListAllGroupNodesAutoComplete(CompanyCode, branchCode);
            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Ledger List Successfully retrived", DATA = data.ToList(), STATUS_CODE = (int)HttpStatusCode.OK });
        }
        [HttpPost]
        public HttpResponseMessage GetSubLedgerList(string CompanyCode, string branchCode)
        {
            if (string.IsNullOrEmpty(CompanyCode))
                CompanyCode = "01";
            var data = _mobileService.SubLedgerList(CompanyCode, branchCode);
            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Sub Ledger List Successfully retrived", DATA = data.ToList(), STATUS_CODE = (int)HttpStatusCode.OK });
        }
        [HttpPost]
        public HttpResponseMessage GetDealerSubLedgerList(string CompanyCode, string branchCode)
        {
            if (string.IsNullOrEmpty(CompanyCode))
                CompanyCode = "01";
            var data = _mobileService.DealerSubLedgerList(CompanyCode, branchCode);
            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Sub Ledger List Successfully retrived", DATA = data.ToList(), STATUS_CODE = (int)HttpStatusCode.OK });
        }
        [HttpPost]
        public HttpResponseMessage GetDealerCustomerList(string PartyType)
        {
            if (string.IsNullOrEmpty(PartyType))
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Empty Party Type", DATA = new List<object>(), STATUS_CODE = (int)HttpStatusCode.BadRequest });
            var data = _mobileService.DealerCustomerList(PartyType);
            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Customer List Successfully retrived", DATA = data.ToList(), STATUS_CODE = (int)HttpStatusCode.OK });
        }
        [HttpPost]
        public HttpResponseMessage GetSubLedgerAccountList(string CompanyCode,string SubCode)
        {
            if (string.IsNullOrEmpty(SubCode))
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "SubCode value not supplied", Data = "SubCode value not supplied", STATUS_CODE = (int)HttpStatusCode.ExpectationFailed });
            if (string.IsNullOrEmpty(CompanyCode))
                CompanyCode = "01";
            var data = _mobileService.AccountSubLedgerList(CompanyCode, SubCode);
            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Sub Ledger List Successfully retrived", DATA = data.ToList(), STATUS_CODE = (int)HttpStatusCode.OK });
        }

        [HttpPost]
        public HttpResponseMessage GetSubLedgerDetail([ModelBinder]GetSubLdegerDetail model)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    var errors = GetModelStateErrors(ModelState);
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Required value not supplied", Data = errors, STATUS_CODE = (int)HttpStatusCode.ExpectationFailed });
                }

                if (!this._mobileService.UserExist(model.UserId, model.CompanyCode))
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "User Could not be authorized", STATUS_CODE = (int)HttpStatusCode.Unauthorized });

                var vouchers = this._mobileService.GetLedgerDetailBySubCode(model.AccountCode, model.SubCode, model.FromDate, model.ToDate, model.CompanyCode, model.BranchCode);
                if (vouchers.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Voucher Successfully retrived", DATA = vouchers.ToList(), STATUS_CODE = (int)HttpStatusCode.OK });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Voucher not found", STATUS_CODE = (int)HttpStatusCode.NotFound });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });

            }
        }

        //  GeneralLedgerDetail
        [HttpPost]
        public HttpResponseMessage GetDealerSubLedgerDetail([ModelBinder]GetSubLdegerDetail model)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    var errors = GetModelStateErrors(ModelState);
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Required value not supplied", Data = errors, STATUS_CODE = (int)HttpStatusCode.ExpectationFailed });
                }

                if (!this._mobileService.UserExist(model.UserId, model.CompanyCode))
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "User Could not be authorized", STATUS_CODE = (int)HttpStatusCode.Unauthorized });

                var vouchers = this._mobileService.GetDealerLedgerDetail(model.AccountCode, model.SubCode, model.FromDate, model.ToDate, model.CompanyCode, model.BranchCode, model.PartyTypeCode);
                if (vouchers.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Voucher Successfully retrived", DATA = vouchers.ToList(), STATUS_CODE = (int)HttpStatusCode.OK });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Voucher not found", STATUS_CODE = (int)HttpStatusCode.NotFound });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });

            }
        }

        [HttpPost]
        public HttpResponseMessage GetGeneralLedgerDetail([ModelBinder]GeneralLedgerDetail model)
        {
            try
            {
                var companyCode = string.Empty;

                if (!ModelState.IsValid)
                {
                    var errors = GetModelStateErrors(ModelState);
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Required value not supplied", Data = errors, STATUS_CODE = (int)HttpStatusCode.ExpectationFailed });
                }

                if (model.Filter.CompanyFilter.Count > 0)
                {
                    companyCode = string.Join(",", model.Filter.CompanyFilter);
                }
                if (!this._mobileService.UserExist(model.UserId, companyCode))
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "User Could not be authorized", STATUS_CODE = (int)HttpStatusCode.Unauthorized });

                var vouchers = this._mobileService.GetVoucherDetailsByAccountCode(model.Filter, model.FromDate, model.ToDate, model.AccountCode, model.BranchCode, model.DataGeneric);
                if (vouchers.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Voucher Successfully retrived", DATA = vouchers.ToList(), STATUS_CODE = (int)HttpStatusCode.OK });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Voucher not found", STATUS_CODE = (int)HttpStatusCode.NotFound });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });

            }
        }


        public HttpResponseMessage GetModuleCount()
        {
            try
            {
                var vouchers = this._mobileService.GetModuelCount();
                if (vouchers.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Approval Count Successfully retrived", DATA = vouchers.ToList(), STATUS_CODE = (int)HttpStatusCode.OK });
                }
                return Request.CreateResponse(HttpStatusCode.NotFound, new { MESSAGE = "Approval not found", STATUS_CODE = (int)HttpStatusCode.NotFound });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });

            }
        }

        [HttpPost]
        public HttpResponseMessage GetVoucherDetail([ModelBinder]GetVoucherDetailModel model)
        {
    
            if(!ModelState.IsValid)
            {
                var errors = GetModelStateErrors(ModelState);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Required value not supplied", Data = errors, STATUS_CODE = (int)HttpStatusCode.ExpectationFailed });

            }

            if(!this._mobileService.UserExist(model.userId, model.companyCode))
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "User Could not be authorized", STATUS_CODE = (int)HttpStatusCode.Unauthorized });

            try
            {
                var ledgerDetails = this._mobileService.GetVoucherLedgers(model.voucherCode, model.tableName, model.companyCode, model.branchCode);
                if(ledgerDetails != null && ledgerDetails.LEDGER_DETAIL.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = string.Format("Ledger Detail Success retrived for voucher {0}", model.voucherCode), DATA = ledgerDetails, STATUS_CODE = (int)HttpStatusCode.OK });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = string.Format("No Ledger Detail Found for {0}", model.voucherCode), STATUS_CODE = (int)HttpStatusCode.NotFound });
            }
            catch(Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }

        
        [HttpPost]
        public HttpResponseMessage ApproveVoucher([ModelBinder]ApproveVoucherModel model)
        {
            if(model == null)
            {
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, new { STATUS_CODE = (int)HttpStatusCode.ExpectationFailed, MESSAGE = "Required parameter not supplied" });
            }

            if( ModelState.IsValid)
            {
                try {
                    var message = string.Empty;
                    var update = this._mobileService.ApproveVoucher(model.FORM_CODE, model.VOUCHER_NO, model.USER_NAME, model.company_code, model.branch_code ,out message);
                    if(update > 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new { STATUS_CODE = (int)HttpStatusCode.OK, MESSAGE = message, model.VOUCHER_NO });
                    }

                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { STATUS_CODE = (int)HttpStatusCode.BadRequest, MESSAGE = message, ERRORS = ModelState });
                }
                catch(Exception ex) {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }
            }

            return Request.CreateResponse(HttpStatusCode.ExpectationFailed, new { STATUS_CODE = (int)HttpStatusCode.ExpectationFailed, MESSAGE = "Expected data not found", ERRORS = ModelState });
        }


        [HttpPost]
        public HttpResponseMessage AuthoriseVoucher([ModelBinder]ApproveVoucherModel model)
        {
            if (model == null)
            {
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, new { STATUS_CODE = (int)HttpStatusCode.ExpectationFailed, MESSAGE = "Required parameter not supplied" });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var message = string.Empty;
                    var update = this._mobileService.AuthoriseVoucher(model.FORM_CODE, model.VOUCHER_NO, model.USER_NAME, model.company_code, model.branch_code, out message);
                    if (update > 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new { STATUS_CODE = (int)HttpStatusCode.OK, MESSAGE = message, model.VOUCHER_NO });
                    }

                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { STATUS_CODE = (int)HttpStatusCode.BadRequest, MESSAGE = message, ERRORS = ModelState });
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }
            }

            return Request.CreateResponse(HttpStatusCode.ExpectationFailed, new { STATUS_CODE = (int)HttpStatusCode.ExpectationFailed, MESSAGE = "Expected data not found", ERRORS = ModelState });
        }

        [HttpPost]
        public HttpResponseMessage PostVoucher([ModelBinder]ApproveVoucherModel model)
        {
            if (model == null)
            {
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, new { STATUS_CODE = (int)HttpStatusCode.ExpectationFailed, MESSAGE = "Required parameter not supplied" });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var message = string.Empty;
                    var update = this._mobileService.PostVoucher(model.FORM_CODE, model.VOUCHER_NO, model.USER_NAME, model.company_code,model.branch_code, out message);
                    if (update > 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new { STATUS_CODE = (int)HttpStatusCode.OK, MESSAGE = message, model.VOUCHER_NO });
                    }

                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { STATUS_CODE = (int)HttpStatusCode.BadRequest, MESSAGE = message, ERRORS = ModelState });
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }
            }

            return Request.CreateResponse(HttpStatusCode.ExpectationFailed, new { STATUS_CODE = (int)HttpStatusCode.ExpectationFailed, MESSAGE = "Expected data not found", ERRORS = ModelState });
        }


        protected Dictionary<string, List<string>> GetModelStateErrors(ModelStateDictionary viewData)
        {
            var errors = new Dictionary<string, List<string>>();

            foreach (var key in viewData.Keys)
            {
                var modelState = viewData[key];
                var list = new List<string>();
                foreach (ModelError error in modelState.Errors)
                {
                    list.Add(error.ErrorMessage);       
                }

                errors.Add(key, list);
            }

            return errors;

        }

        [HttpPost]
        public HttpResponseMessage GetCRMDetail()
        {
            try
            {
                var result = this._mobileService.GetCRM();
                if (result.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "CRM detail Successfully retrived", DATA = result.ToList(), STATUS_CODE = (int)HttpStatusCode.OK });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "CRM not found.", STATUS_CODE = (int)HttpStatusCode.NotFound });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });

            }
        }

        [HttpGet]
        public HttpResponseMessage GetCRMDetails()
        {
            try
            {
                var result = this._mobileService.GetCRM();
                if (result.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "CRM detail Successfully retrived", DATA = result.ToList(), STATUS_CODE = (int)HttpStatusCode.OK });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "CRM not found.", STATUS_CODE = (int)HttpStatusCode.NotFound });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });

            }
        }

        [HttpPost]
        public HttpResponseMessage GetPayorder()
        {
            try
            {
                var result = this._mobileService.GetPayorders();
                if (result.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Pay Orders detail Successfully retrived", DATA = result.ToList(), STATUS_CODE = (int)HttpStatusCode.OK });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Pay Orders not found.", STATUS_CODE = (int)HttpStatusCode.NotFound });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });

            }
        }

        [HttpPost]
        public HttpResponseMessage PostPayOrder(PayOrderModel model)
        {
            if (model == null)
            {
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, new { STATUS_CODE = (int)HttpStatusCode.ExpectationFailed, MESSAGE = "Required parameter not supplied" });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var message = string.Empty;
                    var update = this._mobileService.UpdatePayOrder(model);
                    if (update > 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new { STATUS_CODE = (int)HttpStatusCode.OK, MESSAGE = "Success", model.VOUCHER_NO });
                    }

                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { STATUS_CODE = (int)HttpStatusCode.BadRequest, MESSAGE = message, ERRORS = ModelState });
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }
            }

            return Request.CreateResponse(HttpStatusCode.ExpectationFailed, new { STATUS_CODE = (int)HttpStatusCode.ExpectationFailed, MESSAGE = "Expected data not found", ERRORS = ModelState });
        }

        [HttpGet]
        public HttpResponseMessage GetSalesReport(string company_code)
        {
            try
            {
                var result = this._mobileService.GetSalesReport(company_code);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Sales report Successfully retrived", DATA = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetAccountHeadBalance(string company_code, string branch_code)
        {
            try
            {
                var result = this._mobileService.GetAccountHeadBalance(company_code, branch_code); ;
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Report Successfully retrived", DATA = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetAccountHeadBalanceTopic(string company_code, string filterName, string topic)
        {
            try
            {
                var result = this._mobileService.GetAccountHeadBalanceTopic(company_code, filterName, topic);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Report Successfully retrived", DATA = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetTopicSalesReport(string company_code, string topic)
        {
            try
            {
                var result = this._mobileService.GetTopicWiseSales(company_code, topic);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Sales report Successfully retrived", DATA = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetTopicSalesReportMtd(string company_code, string topic)
        {
            try
            {
                var result = this._mobileService.GetTopicWiseSalesMTD(company_code, topic);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Sales report Successfully retrived", DATA = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetTopicSalesReportDaily(string company_code, string topic)
        {
            try
            {
                var result = this._mobileService.GetTopicWiseSalesDaily(company_code, topic);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Sales report Successfully retrived", DATA = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetTopicSalesReportQuarterly(string company_code, string topic, string quater)
        {
            try
            {
                var result = this._mobileService.GetTopicWiseSalesQuaterly(company_code, topic, quater);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Sales report Successfully retrived", DATA = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
       
        //[HttpGet]
        //public HttpResponseMessage GetTopicWiseQuarterlySalesReport(string company_code, string topic, string quater)
        //{
        //    try
        //    {
        //        var result = this._mobileService.GetTopicWiseQuaterlySales(company_code, topic, quater);
        //        return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Sales report Successfully retrived", DATA = result, STATUS_CODE = (int)HttpStatusCode.OK });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
        //    }
        //}

        [HttpGet]
        public HttpResponseMessage GetPap(string company_code)
        {
            try
            {
                var result = this._mobileService.GetPap(company_code);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Data Successfully retrived", DATA = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetPop(string company_code)
        {
            try
            {
                var result = this._mobileService.GetPop(company_code);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Data Successfully retrived", DATA = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetPrp(string company_code)
        {
            try
            {
                var result = this._mobileService.GetPrp(company_code);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Data Successfully retrived", DATA = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }


        [HttpPost]
        public HttpResponseMessage GetSubledgerWithFilter(string company_code,string filtername,string formdate,string todate)
        {
            try
            {
                var result = this._mobileService.GetSubLedgerWithFilter(company_code,filtername,formdate,todate);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Data Successfully retrived", DATA = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetLedgerFiter()
        {
            try
            {
                var result = this._mobileService.GetLedgerFiler();
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Data Successfully retrived", DATA = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        [HttpGet]
        public HttpResponseMessage GetSalesVsTargetMonthly(string companyCode="01",string UserId="01")
        {
            try
            {
                var result = this._mobileService.GetMonthlySalesVsTarget(companyCode,UserId);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Data Successfully retrived", DATA = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpPost]
        public HttpResponseMessage GetAgingDealerGroup(string CompanyCode)
        {
            if (string.IsNullOrEmpty(CompanyCode))
                CompanyCode = "01";
            var data = _mobileService.AgingDelarGroup(CompanyCode);
            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Dealer Group List", DATA = data.ToList(), STATUS_CODE = (int)HttpStatusCode.OK });
        }
        [HttpPost]
        public HttpResponseMessage GetAgingPartyTypeDealerGroup(string CompanyCode,string DealerGroup= "All")
        {
            if (string.IsNullOrEmpty(CompanyCode))
                CompanyCode = "01";
            var data = _mobileService.AgingPartyType(CompanyCode,DealerGroup);
            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Party Type List", DATA = data.ToList(), STATUS_CODE = (int)HttpStatusCode.OK });
        }
        [HttpGet]
        public HttpResponseMessage GetDivisions(string company_code)
        {
            try
            {
                var result = this._mobileService.GetDivisions(company_code);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Data Successfully retrived", DATA = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetItems(string company_code)
        {
            try
            {
                var result = this._mobileService.GetItems(company_code);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Data Successfully retrived", DATA = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetTopEmployeesByTheirSalesAmtQty(string amtOrQtyWise, string company_code)
        {
            try
            {
                var result = this._mobileService.GetTopEmployeesByTheirSalesAmtQty(amtOrQtyWise,company_code);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Data Successfully retrived", DATA = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        [HttpGet]
        public HttpResponseMessage GetTopDealerByTheirSalesAmtQty(string amtOrQtyWise, string company_code)
        {
            try
            {
                var result = this._mobileService.GetTopDealerByTheirSalesAmtQty(amtOrQtyWise, company_code);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Data Successfully retrived", DATA = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        [HttpGet]
        public HttpResponseMessage GetTopEmployeesByTheirSalesAmtQtyForMonth(string company_code, string branch_code)
        {
            try
            {
                var result = this._mobileService.GetTopEmployeesByTheirSalesAmtQtyForMonth(company_code,branch_code);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Data Successfully retrived", DATA = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        [HttpGet]
        public HttpResponseMessage GetTopDealerByTheirSalesAmtQtyForMonth(string company_code,string branch_code)
        {
            try
            {
                var result = this._mobileService.GetTopDealerByTheirSalesAmtQtyForMonth(company_code,branch_code);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Data Successfully retrived", DATA = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
    }
}