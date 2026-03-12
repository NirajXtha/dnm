using NeoErp.Core;
using NeoErp.Core.Caching;
using NeoErp.Core.Domain;
using NeoErp.Core.Models;
using NeoErp.Core.Models.CustomModels;
using NeoErp.Core.Models.Log4NetLoggin;
using NeoErp.Data;
using NeoERP.DocumentTemplate.Service.Interface;
using NeoERP.DocumentTemplate.Service.Models;
using NeoERP.DocumentTemplate.Service.Models.CustomForm;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;

namespace NeoERP.DocumentTemplate.Controllers.Api
{

    public class SetupApiController : ApiController
    {

        private IDocumentStup _iDocumentSetup;
        private IFormSetupRepo _iFormSetupRepo;
        private IFormTemplateRepo _FormTemplateRepo;
        private IDbContext _dbContext;
        private IWorkContext _workContext;
        private ICacheManager _cacheManager;
        private readonly ILogErp _logErp;
        private DefaultValueForLog _defaultValueForLog;
        public SetupApiController(IDocumentStup _idocumentSetup, IFormSetupRepo _iFormSetupRepo, IDbContext dbContext, IWorkContext workContext, ICacheManager cacheManager, IFormTemplateRepo FormTemplateRepo)
        {
            this._iDocumentSetup = _idocumentSetup;
            this._iFormSetupRepo = _iFormSetupRepo;
            this._FormTemplateRepo = FormTemplateRepo;
            this._dbContext = dbContext;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            this._logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule, _defaultValueForLog.FormCode);
        }

        #region Account
        [HttpPost]
        public HttpResponseMessage DeleteAccountSetupByAccCode(string accCode)
        {
            try
            {
                var result = _iDocumentSetup.DeleteAccountSetupByAccCode(accCode);
                #region CLEAR CACHE
                List<string> keystart = new List<string>();
                keystart.Add("GetChildOfAccountByGroup");
                keystart.Add("getAllAccountMaps");
                keystart.Add("getAccountCode");
                keystart.Add("AllAccountSetupByCode");
                keystart.Add("AllAccountSetupByName");
                keystart.Add("AllFilterAccount");
                keystart.Add("GetAllAccountCode");
                keystart.Add("GetAllChargeAccountSetupByFilter");
                keystart.Add("getAccountCodeWithChild");
                keystart.Add("getAllAccountCodeWithChild");
                keystart.Add("getAllAccountComboCodeWithChild");
                keystart.Add("GetAccountListByAccountCode");
                keystart.Add("GetSubLedgerByAccountCode");
                List<string> Record = new List<string>();
                Record = this._cacheManager.GetAllKeys();
                this._cacheManager.RemoveCacheByKey(keystart, Record);
                #endregion
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        [HttpPost]
        public HttpResponseMessage updateAccountByAccCode(AccountSetupModel model)
        {
            try
            {
                var result = this._iDocumentSetup.udpateAccountSetup(model);
                if (result == "UPDATED")
                {
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("GetChildOfAccountByGroup");
                    keystart.Add("getAllAccountMaps");
                    keystart.Add("getAccountCode");
                    keystart.Add("AllAccountSetupByCode");
                    keystart.Add("AllAccountSetupByName");
                    keystart.Add("AllFilterAccount");
                    keystart.Add("GetAllAccountCode");
                    keystart.Add("GetAllChargeAccountSetupByFilter");
                    keystart.Add("getAccountCodeWithChild");
                    keystart.Add("getAllAccountCodeWithChild");
                    keystart.Add("getAllAccountComboCodeWithChild");
                    keystart.Add("GetAccountListByAccountCode");
                    keystart.Add("GetSubLedgerByAccountCode");
                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "UPDATED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        [HttpPost]
        public HttpResponseMessage createNewAccountHead(AccountSetupModel model)
        {
            try
            {
                var result = this._iDocumentSetup.createNewAccountSetup(model);
                if (result == "INSERTED")
                {
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("GetChildOfAccountByGroup");
                    keystart.Add("getAllAccountMaps");
                    keystart.Add("getAccountCode");
                    keystart.Add("AllAccountSetupByCode");
                    keystart.Add("AllAccountSetupByName");
                    keystart.Add("AllFilterAccount");
                    keystart.Add("GetAllAccountCode");
                    keystart.Add("GetAllChargeAccountSetupByFilter");
                    keystart.Add("getAccountCodeWithChild");
                    keystart.Add("getAllAccountCodeWithChild");
                    keystart.Add("getAllAccountComboCodeWithChild");
                    keystart.Add("GetAccountListByAccountCode");
                    keystart.Add("GetSubLedgerByAccountCode");
                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "INSERTED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else if (result == "INVALIDINSERTED")
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, new
                    {
                        MESSAGE = "INVALIDINSERTED",
                        STATUS_CODE = (int)HttpStatusCode.Conflict
                    });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        [HttpGet]
        public HttpResponseMessage getAccountDetailsByAccCode(string accCode)
        {
            try
            {
                var result = this._iDocumentSetup.GetAccountDataByAccCode(accCode);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        [HttpGet]
        public List<AccountSetupModel> GetChildOfAccountByGroup(string groupId)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<AccountSetupModel>();
            //if (this._cacheManager.IsSet($"GetChildOfAccountByGroup_{userid}_{company_code}_{branch_code}+{groupId}"))
            //{
            //    var data = _cacheManager.Get<List<AccountSetupModel>>($"GetChildOfAccountByGroup_{userid}_{company_code}_{branch_code}+{groupId}");
            //    response = data;
            //}
            //else
            //{
            //    var accountListByGroupCodeList = this._iDocumentSetup.GetAccountListByGroupCode(groupId);
            //    this._cacheManager.Set($"GetChildOfAccountByGroup_{userid}_{company_code}_{branch_code}_{groupId}", accountListByGroupCodeList, 20);
            //    response = accountListByGroupCodeList;
            //}
            //return response;
            var result = this._iDocumentSetup.GetAccountListByGroupCode(groupId);
            return result;
        }
        [HttpGet]
        public List<AccTypeModels> getAllAccountMaps(string filter)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<AccTypeModels>();
            if (this._cacheManager.IsSet($"getAllAccountMaps_{userid}_{company_code}_{branch_code}_{filter}"))
            {
                var data = _cacheManager.Get<List<AccTypeModels>>($"getAllAccountMaps_{userid}_{company_code}_{branch_code}_{filter}");
                response = data;
            }
            else
            {
                var getAllAccountMapsList = this._iDocumentSetup.getAllAccountMaps(filter);
                this._cacheManager.Set($"getAllAccountMaps_{userid}_{company_code}_{branch_code}_{filter}", getAllAccountMapsList, 20);
                response = getAllAccountMapsList;
            }
            return response;
            //var result = this._iDocumentSetup.getAllAccountMaps(filter);
            //return result;
        }
        [HttpGet]
        public List<AccountSetupModel> GetAccountList(string searchtext)
        {
            var result = this._iDocumentSetup.GetAccountList(searchtext);
            return result;
        }
        [HttpGet]
        public string GetNewAccountCode()
        {
            return this._iDocumentSetup.GetNewAccountCode();
        }
        [HttpGet]
        public string GetMaxNewAccountCode()
        {
            return this._iDocumentSetup.GetMaxNewAccountCode();
        }

        [HttpGet]
        public string GetNewAccountName(string searchAccountName)
        {
            return this._iDocumentSetup.GetNewAccountName(searchAccountName);
        }

        [HttpGet]
        public List<BSCustomSetupModel> GetBSCustomSetupList()
        {
            return this._iDocumentSetup.GetBSCustomSetupList();
        }

        [HttpGet]
        public List<PLCustomSetupModel> GetPLCustomSetupList()
        {
            return this._iDocumentSetup.GetPLCustomSetupList();
        }

        [HttpPost]
        public HttpResponseMessage SaveBSCustomSetup(List<BSCustomSetupSaveModel> model)
        {
            try
            {
                var result = this._iDocumentSetup.SaveBSCustomSetup(model);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpPost]
        public HttpResponseMessage SavePLCustomSetup(List<PLCustomSetupSaveModel> model)
        {
            try
            {
                var result = this._iDocumentSetup.SavePLCustomSetup(model);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        #endregion

        #region Customer

        [HttpGet]
        public List<CustomerSetupModel> GetChildOfCustomerByGroup(string groupCode, string wholeSearchText = "nothings")
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<CustomerSetupModel>();
            //if (this._cacheManager.IsSet($"GetChildOfCustomerByGroup_{userid}_{company_code}_{branch_code}_{groupId}"))
            //{
            //    var data = _cacheManager.Get<List<CustomerSetupModel>>($"GetChildOfCustomerByGroup_{userid}_{company_code}_{branch_code}_{groupId}");
            //    response = data;
            //}
            //else
            //{
            //    var accountListByCustomerCodeList = this._iDocumentSetup.GetAccountListByCustomerCode(groupId);
            //    this._cacheManager.Set($"GetChildOfCustomerByGroup_{userid}_{company_code}_{branch_code}_{groupId}", accountListByCustomerCodeList, 20);
            //    response = accountListByCustomerCodeList;
            //}
            //return response;
            var result = this._iDocumentSetup.GetAccountListByCustomerCode(groupCode, wholeSearchText);
            return result;
        }
        [HttpGet]
        public List<CustomerSetupModel> GetAllAccountListByCustomerCode123(string searchText)
        {
            return this._iDocumentSetup.GetAllAccountListByCustomerCode123(searchText);
        }

        [HttpGet]
        public List<CustomerSetupModel> GetCustomerForPDC()
        {
            return this._iDocumentSetup.GetCustomerForPDC();
        }

        [HttpGet]
        public List<PartyTypeModels> GetPartyTypeCodeForCustomer(string customerCode)
        {
            return this._iDocumentSetup.GetPartyTypeCodeForCustomer(customerCode);
        }

        [HttpGet]
        public List<PartyTypeModels> GetPartyTypeCodeForSupplier(string supplierCode)
        {
            return this._iDocumentSetup.GetPartyTypeCodeForSupplier(supplierCode);
        }

        [HttpGet]
        public List<EmployeeCodeModels> GetEmployeeForPDC()
        {
            return this._iDocumentSetup.GetEmployeeForPDC();
        }


        [HttpGet]
        public List<SupplierDTO> GetSupplierForPDC()
        {
            return this._iDocumentSetup.GetSupplierForPDC();
        }


        [HttpGet]
        public List<ChartOfAccountForPDCModel> GetChartOfAccounts()
        {
            return this._iDocumentSetup.GetChartOfAccounts();
        }


        [HttpGet]
        public List<ItemSetupModel> getAllItemsForCustomerStock(string filter)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<ItemSetupModel>();
            if (this._cacheManager.IsSet($"getAllItemsForCustomerStock_{userid}_{company_code}_{branch_code}_{filter}"))
            {
                var data = _cacheManager.Get<List<ItemSetupModel>>($"getAllItemsForCustomerStock_{userid}_{company_code}_{branch_code}_{filter}");
                response = data;
            }
            else
            {
                var getAllItemsForCustomerStockList = this._iDocumentSetup.getAllItemsForCustomerStock(filter);
                this._cacheManager.Set($"getAllItemsForCustomerStock_{userid}_{company_code}_{branch_code}_{filter}", getAllItemsForCustomerStockList, 20);
                response = getAllItemsForCustomerStockList;
            }
            return response;
            //var result = this._iDocumentSetup.getAllItemsForCustomerStock(filter);
            //return result;
        }
        [HttpGet]
        public List<EmployeeCodeModels> getAllComboEmployees(string filter)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<EmployeeCodeModels>();
            if (this._cacheManager.IsSet($"getAllComboEmployees_{userid}_{company_code}_{branch_code}_{filter}"))
            {
                var data = _cacheManager.Get<List<EmployeeCodeModels>>($"getAllComboEmployees_{userid}_{company_code}_{branch_code}_{filter}");
                response = data;
            }
            else
            {
                var getAllComboEmployeesList = this._iDocumentSetup.getAllComboEmployees(filter);
                this._cacheManager.Set($"getAllComboEmployees_{userid}_{company_code}_{branch_code}_{filter}", getAllComboEmployeesList, 20);
                response = getAllComboEmployeesList;
            }
            return response;
            //var result = this._iDocumentSetup.getAllComboEmployees(filter);
            //return result;
        }
        [HttpGet]
        public List<DealerModels> getAllComboDealers(string filter)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<DealerModels>();
            if (this._cacheManager.IsSet($"getAllComboDealers_{userid}_{company_code}_{branch_code}_{filter}"))
            {
                var data = _cacheManager.Get<List<DealerModels>>($"getAllComboDealers_{userid}_{company_code}_{branch_code}_{filter}");
                response = data;
            }
            else
            {
                var getAllComboDealersList = this._iDocumentSetup.getAllComboDealers(filter);
                this._cacheManager.Set($"getAllComboDealers_{userid}_{company_code}_{branch_code}_{filter}", getAllComboDealersList, 20);
                response = getAllComboDealersList;
            }
            return response;
            //var result = this._iDocumentSetup.getAllComboDealers(filter);
            //return result;
        }
        [HttpGet]
        public CustomerModels GetChildCustomerByCustomerCode(string customerCode)
        {
            var result = this._iDocumentSetup.GetChildCustomerByCustomerCode(customerCode);
            return result;
        }
        [HttpGet]
        public int? MaxCustomer()
        {
            var result = this._iDocumentSetup.GetMaxCustomer();
            return result;
        }
        [HttpGet]
        public int? MaxCustomerChild()
        {
            var result = this._iDocumentSetup.GetMaxChildCustomer();
            return result;
        }
        [HttpPost]
        public HttpResponseMessage createNewCustomerGroup(CustomerModels model)
        {
            try
            {
                var result = this._iDocumentSetup.createNewCustomerSetup(model);
                if (result == "INSERTED")
                {
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("GetChildOfCustomerByGroup");
                    keystart.Add("getAllItemsForCustomerStock");
                    keystart.Add("getAllComboEmployees");
                    keystart.Add("getAllComboDealers");
                    keystart.Add("GetCustomers");
                    keystart.Add("AllCustomerSetupByCode");
                    keystart.Add("AllCustomerSetupByName");
                    keystart.Add("AllCustomerSetupByAddress");
                    keystart.Add("AllCustomerSetupByPhoneno");
                    keystart.Add("AllFilterCustomer");
                    keystart.Add("customerDropDownForGroupPopup");
                    keystart.Add("GetCustomerListByCustomerCode");
                    keystart.Add("GetAllCustomerSetupByFilter");


                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "INSERTED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else if (result == "INVALIDINSERTED")
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, new
                    {
                        MESSAGE = "INVALIDINSERTED",
                        STATUS_CODE = (int)HttpStatusCode.Conflict
                    });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpPost]
        public HttpResponseMessage createKYCForm(KYCFORM model)
        {
            try
            {
                var result = this._iDocumentSetup.CreateKYCForm(model);

                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "INSERTED", STATUS_CODE = (int)HttpStatusCode.OK });

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        public HttpResponseMessage getBankAccountsList()
        {
            try
            {
                var result = this._iDocumentSetup.GetBankAccountsList();
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpPost]
        public HttpResponseMessage createChildCustomer(CustomerModels model)
        {
            try
            {
                var result = this._iDocumentSetup.createNewChildCustomerSetup(model);
                if (result == "INSERTED" || result == "UPDATED")
                {
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("GetChildOfCustomerByGroup");
                    keystart.Add("getAllItemsForCustomerStock");
                    keystart.Add("getAllComboEmployees");
                    keystart.Add("getAllComboDealers");
                    keystart.Add("GetCustomers");
                    keystart.Add("AllCustomerSetupByCode");
                    keystart.Add("AllCustomerSetupByName");
                    keystart.Add("AllCustomerSetupByAddress");
                    keystart.Add("AllCustomerSetupByPhoneno");
                    keystart.Add("AllFilterCustomer");
                    keystart.Add("customerDropDownForGroupPopup");
                    keystart.Add("GetCustomerListByCustomerCode");
                    keystart.Add("GetAllCustomerSetupByFilter");
                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else if (result == "INVALIDINSERTED")
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, new
                    {
                        MESSAGE = "Customer name already exist please try another customer name.",
                        STATUS_CODE = (int)HttpStatusCode.Conflict
                    });
                }
                else if (result == "DUPLICATE_CUSTOMER_ID")
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, new
                    {
                        MESSAGE = "Customer ID already exist please try another Customer ID.",
                        STATUS_CODE = (int)HttpStatusCode.Conflict
                    });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        [HttpPost]
        public HttpResponseMessage updateCustomerByCustomerCode(CustomerModels model)
        {
            try
            {
                var result = this._iDocumentSetup.updateCustomerSetup(model);
                if (result == "UPDATED")
                {
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("GetChildOfCustomerByGroup");
                    keystart.Add("getAllItemsForCustomerStock");
                    keystart.Add("getAllComboEmployees");
                    keystart.Add("getAllComboDealers");
                    keystart.Add("GetCustomers");
                    keystart.Add("AllCustomerSetupByCode");
                    keystart.Add("AllCustomerSetupByName");
                    keystart.Add("AllCustomerSetupByAddress");
                    keystart.Add("AllCustomerSetupByPhoneno");
                    keystart.Add("AllFilterCustomer");
                    keystart.Add("customerDropDownForGroupPopup");
                    keystart.Add("GetCustomerListByCustomerCode");
                    keystart.Add("GetAllCustomerSetupByFilter");
                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "UPDATED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }

        [HttpPost]
        public HttpResponseMessage DeleteCustomerTreeByCustomerCode(string customerCode)
        {
            try
            {
                var result = _iDocumentSetup.DeleteCustomerTreeByCustCode(customerCode);
                #region CLEAR CACHE
                List<string> keystart = new List<string>();
                keystart.Add("GetChildOfCustomerByGroup");
                keystart.Add("getAllItemsForCustomerStock");
                keystart.Add("getAllComboEmployees");
                keystart.Add("getAllComboDealers");
                keystart.Add("GetCustomers");
                keystart.Add("AllCustomerSetupByCode");
                keystart.Add("AllCustomerSetupByName");
                keystart.Add("AllCustomerSetupByAddress");
                keystart.Add("AllCustomerSetupByPhoneno");
                keystart.Add("AllFilterCustomer");
                keystart.Add("customerDropDownForGroupPopup");
                keystart.Add("GetCustomerListByCustomerCode");
                keystart.Add("GetAllCustomerSetupByFilter");
                List<string> Record = new List<string>();
                Record = this._cacheManager.GetAllKeys();
                this._cacheManager.RemoveCacheByKey(keystart, Record);
                #endregion
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        [HttpPost]
        public HttpResponseMessage DeleteCustomerByCustomerCode(string customerCode)
        {
            try
            {
                var result = _iDocumentSetup.DeleteCustomerByCustomerCode(customerCode);
                #region CLEAR CACHE
                List<string> keystart = new List<string>();
                keystart.Add("GetChildOfCustomerByGroup");
                keystart.Add("getAllItemsForCustomerStock");
                keystart.Add("getAllComboEmployees");
                keystart.Add("getAllComboDealers");
                keystart.Add("GetCustomers");
                keystart.Add("AllCustomerSetupByCode");
                keystart.Add("AllCustomerSetupByName");
                keystart.Add("AllCustomerSetupByAddress");
                keystart.Add("AllCustomerSetupByPhoneno");
                keystart.Add("AllFilterCustomer");
                keystart.Add("customerDropDownForGroupPopup");
                keystart.Add("GetCustomerListByCustomerCode");
                keystart.Add("GetAllCustomerSetupByFilter");
                List<string> Record = new List<string>();
                Record = this._cacheManager.GetAllKeys();
                this._cacheManager.RemoveCacheByKey(keystart, Record);
                #endregion
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        #endregion

        #region Location
        [HttpPost]
        public HttpResponseMessage DeleteLocationSetupByLocationCode(string locationCode)
        {
            try
            {
                var result = _iDocumentSetup.DeleteLocationSetupByLocationCode(locationCode);
                #region CLEAR CACHE
                List<string> keystart = new List<string>();
                keystart.Add("getAllLocation");
                keystart.Add("GetChildOfLocationByGroup");
                keystart.Add("GetLocation");
                keystart.Add("GetAllLocationListByFilter");
                keystart.Add("GetAllBudgetCenterForLocationByFilter");
                keystart.Add("checkBudgetFlagByLocationCode");
                keystart.Add("GetLocationByGroup");
                keystart.Add("getLocationType");
                keystart.Add("GetLocationListByLocationCode");
                List<string> Record = new List<string>();
                Record = this._cacheManager.GetAllKeys();
                this._cacheManager.RemoveCacheByKey(keystart, Record);
                #endregion
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        [HttpPost]
        public HttpResponseMessage updateLocationByLocationCode(LocationSetupModel model)
        {
            try
            {
                var result = this._iDocumentSetup.udpateLocationSetup(model);
                if (result == "UPDATED")
                {
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("getAllLocation");
                    keystart.Add("GetChildOfLocationByGroup");
                    keystart.Add("GetLocation");
                    keystart.Add("GetAllLocationListByFilter");
                    keystart.Add("GetAllBudgetCenterForLocationByFilter");
                    keystart.Add("checkBudgetFlagByLocationCode");
                    keystart.Add("GetLocationByGroup");
                    keystart.Add("getLocationType");
                    keystart.Add("GetLocationListByLocationCode");
                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "UPDATED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        [HttpPost]
        public HttpResponseMessage createNewLocationHead(LocationSetupModel model)
        {
            try
            {
                var result = this._iDocumentSetup.createNewLocationSetup(model);
                if (result == "INSERTED")
                {
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("getAllLocation");
                    keystart.Add("GetChildOfLocationByGroup");
                    keystart.Add("GetLocation");
                    keystart.Add("GetAllLocationListByFilter");
                    keystart.Add("GetAllBudgetCenterForLocationByFilter");
                    keystart.Add("checkBudgetFlagByLocationCode");
                    keystart.Add("GetLocationByGroup");
                    keystart.Add("getLocationType");
                    keystart.Add("GetLocationListByLocationCode");
                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "INSERTED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else if (result == "INVALIDINSERTED")
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, new
                    {
                        MESSAGE = "INVALIDINSERTED",
                        STATUS_CODE = (int)HttpStatusCode.Conflict
                    });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage getLocationDetailsByLocationCode(string locationCode)
        {
            try
            {
                var result = this._iDocumentSetup.GetLocationDataByLocationCode(locationCode);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        [HttpGet]
        public List<LocationSetupModel> GetChildOfLocationByGroup(string groupId)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<LocationSetupModel>();
            if (this._cacheManager.IsSet($"GetChildOfLocationByGroup_{userid}_{company_code}_{branch_code}_{groupId}"))
            {
                var data = _cacheManager.Get<List<LocationSetupModel>>($"GetChildOfLocationByGroup_{userid}_{company_code}_{branch_code}_{groupId}");
                response = data;
            }
            else
            {
                var locationListByGroupCodeList = this._iDocumentSetup.GetLocationListByGroupCode(groupId);
                this._cacheManager.Set($"GetChildOfLocationByGroup_{userid}_{company_code}_{branch_code}_{groupId}", locationListByGroupCodeList, 20);
                response = locationListByGroupCodeList;
            }
            return response;
            //var result = this._iDocumentSetup.GetLocationListByGroupCode(groupId);
            //return result;
        }
        [HttpGet]
        public List<LocationSetupModel> GetAllLocationList(string searchText)
        {
            return this._iDocumentSetup.GetAllLocationList(searchText);
        }
        [HttpGet]
        public List<LocationModels> getAllLocation(string filter)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<LocationModels>();
            if (this._cacheManager.IsSet($"getAllLocation_{userid}_{company_code}_{branch_code}_{filter}"))
            {
                var data = _cacheManager.Get<List<LocationModels>>($"getAllLocation_{userid}_{company_code}_{branch_code}_{filter}");
                response = data;
            }
            else
            {
                var getAllLocationList = this._iDocumentSetup.getAllLocation(filter);
                this._cacheManager.Set($"getAllLocation_{userid}_{company_code}_{branch_code}_{filter}", getAllLocationList, 20);
                response = getAllLocationList;
            }
            return response;
            //var result = this._iDocumentSetup.getAllLocation(filter);
            //return result;
        }
        #endregion

        #region Attribute
        [HttpPost]
        public HttpResponseMessage DeleteAttributeSetupByAttributeCode(string attributeCode)
        {
            try
            {
                var result = _iDocumentSetup.DeleteAttributeSetupByAttributeCode(attributeCode);
                #region CLEAR CACHE
                List<string> keystart = new List<string>();
                keystart.Add("GetChildOfAttributeByGroup");
                keystart.Add("Getattribute");
                keystart.Add("GetTreeAttribute");
                List<string> Record = new List<string>();
                Record = this._cacheManager.GetAllKeys();
                this._cacheManager.RemoveCacheByKey(keystart, Record);
                #endregion
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        [HttpPost]
        public HttpResponseMessage updateAttributeByAttributeCode(AttributeSetupModel model)
        {
            try
            {
                var result = this._iDocumentSetup.updateAttributeSetup(model);
                if (result == "UPDATED")
                {
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("GetChildOfAttributeByGroup");
                    keystart.Add("Getattribute");
                    keystart.Add("GetTreeAttribute");
                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "UPDATED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        [HttpPost]
        public HttpResponseMessage createNewAttributeHead(AttributeSetupModel model)
        {
            try
            {
                var result = this._iDocumentSetup.createNewAttributeSetup(model);
                if (result == "INSERTED")
                {
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("GetChildOfAttributeByGroup");
                    keystart.Add("Getattribute");
                    keystart.Add("GetTreeAttribute");
                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "INSERTED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        [HttpGet]
        public HttpResponseMessage getAttributeDetailsByAttributeCode(string attributeCode)
        {
            try
            {
                var result = this._iDocumentSetup.GetAttributeDataByAttributeCode(attributeCode);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        [HttpGet]
        public List<AttributeSetupModel> GetChildOfAttributeByGroup(string groupId)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<AttributeSetupModel>();

            if (this._cacheManager.IsSet($"GetChildOfAttributeByGroup_{userid}_{company_code}_{branch_code}_{groupId}"))
            {
                var data = _cacheManager.Get<List<AttributeSetupModel>>($"GetChildOfAttributeByGroup_{userid}_{company_code}_{branch_code}_{groupId}");
                response = data;
            }
            else
            {
                var getAttributeListByGroupCodeList = this._iDocumentSetup.GetAttributeListByGroupCode(groupId);
                this._cacheManager.Set($"GetChildOfAttributeByGroup_{userid}_{company_code}_{branch_code}_{groupId}", getAttributeListByGroupCodeList, 20);
                response = getAttributeListByGroupCodeList;
            }
            return response;
            //var result = this._iDocumentSetup.GetRegionalListByGroupCode(groupId);
            //return result;
        }
        [HttpGet]
        public List<AttributeSetupModel> GetAllAttributeList(string searchText)
        {
            return this._iDocumentSetup.GetAllAttributeList(searchText);
        }

        #endregion

        #region Miscellaneous Sub Ledger
        [HttpGet]
        public string getMaxMiscCode()
        {
            var result = this._iDocumentSetup.GetMaxMiscCode();
            return result;
        }
        [HttpPost]
        public HttpResponseMessage DeleteMiscellaneousSubLedgerSetupByMiscellaneousSubLedgerCode(string MiscellaneousSubLedgerCode)
        {
            try
            {
                var result = _iDocumentSetup.DeleteMiscellaneousSubLedgerSetupByMiscellaneousSubLedgerCode(MiscellaneousSubLedgerCode);
                #region CLEAR CACHE
                List<string> keystart = new List<string>();
                keystart.Add("GetChildOfMiscellaneousSubLedgerByGroup");
                keystart.Add("GetMiscellaneousSubLedger");
                keystart.Add("GetTreeMiscellaneousSubLedger");
                List<string> Record = new List<string>();
                Record = this._cacheManager.GetAllKeys();
                this._cacheManager.RemoveCacheByKey(keystart, Record);
                #endregion
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        [HttpPost]
        public HttpResponseMessage updateMiscellaneousSubLedgerByMiscellaneousSubLedgerCode(MiscellaneousSubLedgerSetupModel model)
        {
            try
            {
                var result = this._iDocumentSetup.udpateMiscellaneousSubLedgerSetup(model);
                if (result == "UPDATED")
                {
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("GetChildOfMiscellaneousSubLedgerByGroup");
                    keystart.Add("GetMiscellaneousSubLedger");
                    keystart.Add("GetTreeMiscellaneousSubLedger");
                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "UPDATED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        [HttpPost]
        public HttpResponseMessage createNewMiscellaneousSubLedgerHead(MiscellaneousSubLedgerSetupModel model)
        {
            try
            {
                var result = this._iDocumentSetup.createNewMiscellaneousSubLedgerSetup(model);
                if (result == "INSERTED")
                {
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("GetChildOfMiscellaneousSubLedgerByGroup");
                    keystart.Add("GetMiscellaneousSubLedger");
                    keystart.Add("GetTreeMiscellaneousSubLedger");
                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "INSERTED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else if (result == "INVALIDINSERTED")
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, new
                    {
                        MESSAGE = "INVALIDINSERTED",
                        STATUS_CODE = (int)HttpStatusCode.Conflict
                    });
                }

                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        [HttpGet]
        public HttpResponseMessage getMiscellaneousSubLedgerDetailsByMiscellaneousSubLedgerCode(string regionCode)
        {
            try
            {
                var result = this._iDocumentSetup.GetMiscellaneousSubLedgerDataByMiscellaneousSubLedgerCode(regionCode);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        [HttpGet]
        public List<MiscellaneousSubLedgerSetupModel> GetChildOfMiscellaneousSubLedgerByGroup(string groupId)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<MiscellaneousSubLedgerSetupModel>();
            if (this._cacheManager.IsSet($"GetChildOfMiscellaneousSubLedgerByGroup_{userid}_{company_code}_{branch_code}_{groupId}"))
            {
                var data = _cacheManager.Get<List<MiscellaneousSubLedgerSetupModel>>($"GetChildOfMiscellaneousSubLedgerByGroup_{userid}_{company_code}_{branch_code}_{groupId}");
                response = data;
            }
            else
            {
                var getMiscellaneousSubLedgerListByGroupCodeList = this._iDocumentSetup.GetMiscellaneousSubLedgerListByGroupCode(groupId);
                this._cacheManager.Set($"GetChildOfMiscellaneousSubLedgerByGroup_{userid}_{company_code}_{branch_code}_{groupId}", getMiscellaneousSubLedgerListByGroupCodeList, 20);
                response = getMiscellaneousSubLedgerListByGroupCodeList;
            }
            return response;
            //var result = this._iDocumentSetup.GetMiscellaneousSubLedgerListByGroupCode(groupId);
            //return result;
        }
        [HttpGet]
        public List<MiscellaneousSubLedgerSetupModel> GetAllMiscellaneousSubLedgerList(string searchText)
        {
            return this._iDocumentSetup.GetAllMiscellaneousSubLedgerList(searchText);
        }
        #endregion

        #region Regional
        [HttpPost]
        public HttpResponseMessage DeleteRegionalSetupByRegionalCode(string regionalCode)
        {
            try
            {
                var result = _iDocumentSetup.DeleteRegionalSetupByRegionalCode(regionalCode);
                #region CLEAR CACHE
                List<string> keystart = new List<string>();
                keystart.Add("GetChildOfRegionalByGroup");
                keystart.Add("Getregional");
                keystart.Add("GetTreeRegional");
                List<string> Record = new List<string>();
                Record = this._cacheManager.GetAllKeys();
                this._cacheManager.RemoveCacheByKey(keystart, Record);
                #endregion
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        [HttpPost]
        public HttpResponseMessage updateRegionalByRegionalCode(RegionalSetupModel model)
        {
            try
            {
                var result = this._iDocumentSetup.udpateRegionalSetup(model);
                if (result == "UPDATED")
                {
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("GetChildOfRegionalByGroup");
                    keystart.Add("Getregional");
                    keystart.Add("GetTreeRegional");
                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "UPDATED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        [HttpPost]
        public HttpResponseMessage createNewRegionalHead(RegionalSetupModel model)
        {
            try
            {
                var result = this._iDocumentSetup.createNewRegionalSetup(model);
                if (result == "INSERTED")
                {
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("GetChildOfRegionalByGroup");
                    keystart.Add("Getregional");
                    keystart.Add("GetTreeRegional");
                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "INSERTED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else if (result == "INVALIDINSERTED")
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, new
                    {
                        MESSAGE = "INVALIDINSERTED",
                        STATUS_CODE = (int)HttpStatusCode.Conflict
                    });
                }

                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        [HttpGet]
        public HttpResponseMessage getRegionalDetailsByRegionalCode(string regionCode)
        {
            try
            {
                var result = this._iDocumentSetup.GetRegionalDataByRegionalCode(regionCode);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }

        [HttpGet]
        public HttpResponseMessage GenerateRegionCode(string regionCode)
        {
            try
            {
                var result = this._iDocumentSetup.GenerateRegionCode(regionCode);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        [HttpGet]
        public List<RegionalSetupModel> GetChildOfRegionalByGroup(string groupId)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<RegionalSetupModel>();
            if (this._cacheManager.IsSet($"GetChildOfRegionalByGroup_{userid}_{company_code}_{branch_code}_{groupId}"))
            {
                var data = _cacheManager.Get<List<RegionalSetupModel>>($"GetChildOfRegionalByGroup_{userid}_{company_code}_{branch_code}_{groupId}");
                response = data;
            }
            else
            {
                var getRegionalListByGroupCodeList = this._iDocumentSetup.GetRegionalListByGroupCode(groupId);
                this._cacheManager.Set($"GetChildOfRegionalByGroup_{userid}_{company_code}_{branch_code}_{groupId}", getRegionalListByGroupCodeList, 20);
                response = getRegionalListByGroupCodeList;
            }
            return response;
            //var result = this._iDocumentSetup.GetRegionalListByGroupCode(groupId);
            //return result;
        }
        [HttpGet]
        public List<RegionalSetupModel> GetAllRegionalList(string searchText)
        {
            return this._iDocumentSetup.GetAllRegionalList(searchText);
        }
        #endregion

        #region Resource
        [HttpPost]
        public HttpResponseMessage DeleteResourceSetupByResourceCode(string resourceCode)
        {
            try
            {
                var result = _iDocumentSetup.DeleteResourceSetupByResourceCode(resourceCode);
                #region CLEAR CACHE
                List<string> keystart = new List<string>();
                keystart.Add("GetChildOfResourceByGroup");
                keystart.Add("GetResource");
                keystart.Add("getResourceCodeWithChild");
                List<string> Record = new List<string>();
                Record = this._cacheManager.GetAllKeys();
                this._cacheManager.RemoveCacheByKey(keystart, Record);
                #endregion
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }

        [HttpPost]
        public HttpResponseMessage updateResourceByResourceCode(ResourceSetupModel model)
        {
            try
            {




                if (model.IS_INDIVIDUAL_OR_SERIAL_ITEM != null && model.IS_INDIVIDUAL_OR_SERIAL_ITEM == true)
                {
                    // Validation for added 
                    if (model.RESOURCE_DETAIL_LIST == null || !model.RESOURCE_DETAIL_LIST.Any())
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "No data provided.");
                    }

                    var validationResults = new List<object>();
                    var details = model.RESOURCE_DETAIL_LIST;

                    for (int i = 0; i < details.Count; i++)
                    {
                        var context = new ValidationContext(details[i], serviceProvider: null, items: null);
                        var results = new List<ValidationResult>();

                        if (!Validator.TryValidateObject(details[i], context, results, true))
                        {
                            validationResults.Add(new
                            {
                                Index = i,
                                Errors = results.Select(r => r.ErrorMessage).ToList()
                            });
                        }
                    }

                    if (validationResults.Any())
                    {
                        var errorObject = new
                        {
                            Message = "Validation failed",
                            Errors = validationResults
                        };

                        return Request.CreateResponse(HttpStatusCode.BadRequest, errorObject);
                    }
                }


                var result = this._iDocumentSetup.udpateResourceSetup(model);
                if (result == "UPDATED")
                {
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("GetChildOfResourceByGroup");
                    keystart.Add("GetResource");
                    keystart.Add("getResourceCodeWithChild");
                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "UPDATED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        [HttpPost]
        public HttpResponseMessage createNewResourceHead(ResourceSetupModel model)
        {
            try
            {



                if (model.IS_INDIVIDUAL_OR_SERIAL_ITEM != null && model.IS_INDIVIDUAL_OR_SERIAL_ITEM == true)
                {
                    if (model.RESOURCE_DETAIL_LIST == null || !model.RESOURCE_DETAIL_LIST.Any())
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "No data provided.");
                    }

                    var validationResults = new List<object>();
                    var details = model.RESOURCE_DETAIL_LIST;

                    for (int i = 0; i < details.Count; i++)
                    {
                        var context = new ValidationContext(details[i], serviceProvider: null, items: null);
                        var results = new List<ValidationResult>();

                        if (!Validator.TryValidateObject(details[i], context, results, true))
                        {
                            validationResults.Add(new
                            {
                                Index = i,
                                Errors = results.Select(r => r.ErrorMessage).ToList()
                            });
                        }
                    }

                    if (validationResults.Any())
                    {
                        var errorObject = new
                        {
                            Message = "Validation failed",
                            Errors = validationResults
                        };

                        return Request.CreateResponse(HttpStatusCode.BadRequest, errorObject);
                    }

                }





                var result = this._iDocumentSetup.createNewResourceSetup(model);
                if (result == "INSERTED")
                {
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("GetChildOfResourceByGroup");
                    keystart.Add("GetResource");
                    keystart.Add("getResourceCodeWithChild");
                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "INSERTED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else if (result == "INVALIDINSERTED")
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, new
                    {
                        MESSAGE = "INVALIDINSERTED",
                        STATUS_CODE = (int)HttpStatusCode.Conflict
                    });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage getResourceDetailsByResourceCode(string resourceCode)
        {
            try
            {
                var result = this._iDocumentSetup.GetResourceDataByResourceCode(resourceCode);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        [HttpGet]
        public List<ResourceSetupModel> GetChildOfResourceByGroup(string groupId)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<ResourceSetupModel>();
            if (this._cacheManager.IsSet($"GetChildOfResourceByGroup_{userid}_{company_code}_{branch_code}_{groupId}"))
            {
                var data = _cacheManager.Get<List<ResourceSetupModel>>($"GetChildOfResourceByGroup_{userid}_{company_code}_{branch_code}_{groupId}");
                response = data;
            }
            else
            {
                var getResourceListByGroupCodeList = this._iDocumentSetup.GetResourceListByGroupCode(groupId);
                this._cacheManager.Set($"GetChildOfResourceByGroup_{userid}_{company_code}_{branch_code}_{groupId}", getResourceListByGroupCodeList, 20);
                response = getResourceListByGroupCodeList;
            }
            return response;
            //var result = this._iDocumentSetup.GetResourceListByGroupCode(groupId);
            //return result;
        }
        public List<ResourceSetupModel> GetAllResourceList(string searchText)
        {
            return this._iDocumentSetup.GetAllResourceList(searchText);
        }
        #endregion

        #region Process
        [HttpPost]
        public HttpResponseMessage DeleteProcessSetupByProcessCode(string processCode)
        {
            try
            {
                var result = _iDocumentSetup.DeleteProcessSetupByProcessCode(processCode);
                //#region CLEAR CACHE
                //List<string> keystart = new List<string>();
                //keystart.Add("GetChildOfProcessByGroup");
                //keystart.Add("getProcessCode");
                //keystart.Add("getProcessCodeWithChild");
                //List<string> Record = new List<string>();
                //Record = this._cacheManager.GetAllKeys();
                //this._cacheManager.RemoveCacheByKey(keystart, Record);
                //#endregion
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        [HttpPost]
        public HttpResponseMessage updateProcessByProcessCode(ProcessSetupModel model)
        {
            try
            {
                var result = this._iDocumentSetup.udpateProcessSetup(model);
                if (result == "UPDATED")
                {
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("GetChildOfProcessByGroup");
                    keystart.Add("getProcessCode");
                    keystart.Add("getProcessCodeWithChild");
                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "UPDATED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        [HttpPost]
        public HttpResponseMessage createNewProcessHead(ProcessSetupModel model)
        {
            try
            {
                var result = this._iDocumentSetup.createNewProcessSetup(model);
                if (result == "INSERTED")
                {
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("GetChildOfProcessByGroup");
                    keystart.Add("getProcessCode");
                    keystart.Add("getProcessCodeWithChild");
                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "INSERTED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        [HttpGet]
        public HttpResponseMessage getProcessDetailsByProcessCode(string processCode)
        {
            try
            {
                var result = this._iDocumentSetup.GetProcessDataByProcessCode(processCode);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        //get Routine Input data
        [HttpGet]
        public HttpResponseMessage GetProcessInputdata()
        {
            try
            {
                var result = this._iDocumentSetup.GetProcessInputGriddata();
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        [HttpGet]
        public List<ProcessSetupModel> GetChildOfProcessByGroup(string groupId)
        {
            //var userid = _workContext.CurrentUserinformation.User_id;
            //var company_code = _workContext.CurrentUserinformation.company_code;
            //var branch_code = _workContext.CurrentUserinformation.branch_code;
            //var response = new List<ProcessSetupModel>();
            //if (this._cacheManager.IsSet($"GetChildOfProcessByGroup_{userid}_{company_code}_{branch_code}_{groupId}"))
            //{
            //    var data = _cacheManager.Get<List<ProcessSetupModel>>($"GetChildOfProcessByGroup_{userid}_{company_code}_{branch_code}_{groupId}");
            //    response = data;
            //}
            //else
            //{
            //    var processListByGroupCodeList = this._iDocumentSetup.GetProcessListByGroupCode(groupId);
            //    this._cacheManager.Set($"GetChildOfProcessByGroup_{userid}_{company_code}_{branch_code}_{groupId}", processListByGroupCodeList, 20);
            //    response = processListByGroupCodeList;
            //}
            //return response;
            var result = this._iDocumentSetup.GetProcessListByGroupCode(groupId);
            return result;
        }
        //[HttpGet]
        //public List<ProcessSetupModel> GetChildOfProcessByGroup(string groupId)
        //{
        //    var result = this._iDocumentSetup.GetProcessListByGroupCode(groupId);
        //    return result;
        //}
        [HttpGet]
        public List<ProcessSetupModel> GetAllProcessList(string searchText)
        {
            return this._iDocumentSetup.GetAllProcessList(searchText);
        }
        #endregion

        #region Item
        [HttpGet]
        public HttpResponseMessage getMaxItemCode(string gFlag)
        {
            try
            {
                var result = this._iDocumentSetup.GetMaxItemCode(gFlag);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        [HttpGet]
        public HttpResponseMessage getitemDetailsByItemCode(string accCode)
        {
            try
            {
                var result = this._iDocumentSetup.GetItemDataByItemCode(accCode);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }


        [HttpGet]
        public List<ItemSetupModel> GetChildOfItemByGroup(string groupId)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<ItemSetupModel>();
            //if (this._cacheManager.IsSet($"GetChildOfItemByGroup_{userid}_{company_code}_{branch_code}_{groupId}"))
            //{
            //    var data = _cacheManager.Get<List<ItemSetupModel>>($"GetChildOfItemByGroup_{userid}_{company_code}_{branch_code}_{groupId}");
            //    response = data;
            //}
            //else
            //{
            var getChildOfItemByGroupList = this._iDocumentSetup.GetItemListByGroupCode(groupId);
            this._cacheManager.Set($"GetChildOfItemByGroup_{userid}_{company_code}_{branch_code}_{groupId}", getChildOfItemByGroupList, 20);
            response = getChildOfItemByGroupList;
            //   }
            return response;
            //var result = this._iDocumentSetup.GetItemListByGroupCode(groupId);
            //return result;
        }
        [HttpGet]
        public List<ItemSetupModel> GetAllItemList(string searchText)
        {
            return this._iDocumentSetup.GetAllItemList(searchText);
        }


        [HttpPost]
        public HttpResponseMessage createNewitem(ItemSetupModalSet model)
        {

            try
            {

                var result = this._iDocumentSetup.createNewItemSetup(model.model);
                if (result == "INSERTED")
                {
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("GetChildOfItemByGroup");
                    keystart.Add("GetProducts");
                    keystart.Add("GetItemChargeDataSavedValueWise");
                    keystart.Add("GetItemChargeDataSavedQuantityWise");
                    keystart.Add("GetInvItemChargesData");
                    keystart.Add("GetMUCodeByProductId");
                    keystart.Add("GetMuCode");
                    keystart.Add("GetProductListByItemCode");
                    keystart.Add("GetAllProductsListByFilter");
                    keystart.Add("getProductCodeWithChild");
                    keystart.Add("GetGroupProducts");
                    keystart.Add("GetItemDataByItemCode");
                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "INSERTED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else if (result == "INVALIDINSERTED")
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, new
                    {
                        MESSAGE = "INVALIDINSERTED",
                        STATUS_CODE = (int)HttpStatusCode.Conflict
                    });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        [HttpPost]
        public HttpResponseMessage updateitemByItemCode(ItemSetupModalSet model)
        {
            try
            {
                var result = this._iDocumentSetup.udpateItemSetup(model.model);
                if (result == "UPDATED")
                {
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("GetChildOfItemByGroup");
                    keystart.Add("GetProducts");
                    keystart.Add("GetItemChargeDataSavedValueWise");
                    keystart.Add("GetItemChargeDataSavedQuantityWise");
                    keystart.Add("GetInvItemChargesData");
                    keystart.Add("GetMUCodeByProductId");
                    keystart.Add("GetMuCode");
                    keystart.Add("GetProductListByItemCode");
                    keystart.Add("GetAllProductsListByFilter");
                    keystart.Add("getProductCodeWithChild");
                    keystart.Add("GetGroupProducts");
                    keystart.Add("GetItemDataByItemCode");
                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "UPDATED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        //[HttpGet]
        //public HttpResponseMessage getitemCodeByitemcode(string itemcode)
        //{
        //    try
        //    {
        //        string result = this._iDocumentSetup.GetItemCodeByItemCode(itemcode);
        //        return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
        //    }
        //    catch (Exception ex)
        //    {

        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
        //    }


        //}
        [HttpPost]
        public HttpResponseMessage DeleteitemsetupByItemcode(string itemcode)
        {
            try
            {
                var result = _iDocumentSetup.DeleteItemSetupByItemCode(itemcode);
                #region CLEAR CACHE
                List<string> keystart = new List<string>();
                keystart.Add("GetChildOfItemByGroup");
                keystart.Add("GetProducts");
                keystart.Add("GetItemChargeDataSavedValueWise");
                keystart.Add("GetItemChargeDataSavedQuantityWise");
                keystart.Add("GetInvItemChargesData");
                keystart.Add("GetMUCodeByProductId");
                keystart.Add("GetMuCode");
                keystart.Add("GetProductListByItemCode");
                keystart.Add("GetAllProductsListByFilter");
                keystart.Add("getProductCodeWithChild");
                keystart.Add("GetGroupProducts");
                List<string> Record = new List<string>();
                Record = this._cacheManager.GetAllKeys();
                this._cacheManager.RemoveCacheByKey(keystart, Record);
                #endregion
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        #endregion

        #region TNC

        [HttpGet]
        public HttpResponseMessage getTNCDetailsByTNCCode(string accCode)
        {
            try
            {
                var result = this._iDocumentSetup.GetTNCDataByTNCCode(accCode);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }

        [HttpGet]
        public List<TNCSetupModel> GetChildOfTNCByGroup(string groupId)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<TNCSetupModel>();
            //if (this._cacheManager.IsSet($"GetChildOfItemByGroup_{userid}_{company_code}_{branch_code}_{groupId}"))
            //{
            //    var data = _cacheManager.Get<List<ItemSetupModel>>($"GetChildOfItemByGroup_{userid}_{company_code}_{branch_code}_{groupId}");
            //    response = data;
            //}
            //else
            //{
            var getChildOfTNCByGroupList = this._iDocumentSetup.GetTNCListByGroupCode(groupId);
            this._cacheManager.Set($"GetChildOfTNCByGroup_{userid}_{company_code}_{branch_code}_{groupId}", getChildOfTNCByGroupList, 20);
            response = getChildOfTNCByGroupList;
            //   }
            return response;
            //var result = this._iDocumentSetup.GetItemListByGroupCode(groupId);
            //return result;
        }

        [HttpPost]
        public HttpResponseMessage createNewTNC(TNCSetupModalSet model)
        {

            try
            {

                var result = this._iDocumentSetup.createNewTNCSetup(model.model);
                if (result == "INSERTED")
                {
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("GetTNC");
                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "INSERTED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else if (result == "INVALIDINSERTED")
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, new
                    {
                        MESSAGE = "INVALIDINSERTED",
                        STATUS_CODE = (int)HttpStatusCode.Conflict
                    });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }


        [HttpPost]
        public HttpResponseMessage updateTNCByTNCCode(TNCSetupModalSet model)
        {
            try
            {
                var result = this._iDocumentSetup.udpateTNCSetup(model.model);
                if (result == "UPDATED")
                {
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("GetTNC");
                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "UPDATED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }


        [HttpPost]
        public HttpResponseMessage DeletetncsetupByTNCcode(string tnccode)
        {
            try
            {
                var result = _iDocumentSetup.DeleteTNCSetupByTNCCode(tnccode);
                #region CLEAR CACHE
                List<string> keystart = new List<string>();
                keystart.Add("GetChildOfTNCByGroup");
                keystart.Add("GetTNC");
                List<string> Record = new List<string>();
                Record = this._cacheManager.GetAllKeys();
                this._cacheManager.RemoveCacheByKey(keystart, Record);
                #endregion
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }

        #endregion

        #region Area
        [HttpPost]
        public HttpResponseMessage deleteAreaSetup(string areaCode)
        {
            try
            {
                var result = _iDocumentSetup.deleteAreaSetup(areaCode);
                #region CLEAR CACHE
                List<string> keystart = new List<string>();
                keystart.Add("GetAllAreaCode");
                keystart.Add("GetAllAreaSetupByFilter");
                keystart.Add("getAreaCodeWithChild");
                List<string> Record = new List<string>();
                Record = this._cacheManager.GetAllKeys();
                this._cacheManager.RemoveCacheByKey(keystart, Record);
                #endregion
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        [HttpPost]
        public HttpResponseMessage createNewAreaSetup(AreaModels model)
        {
            try
            {
                var result = this._iDocumentSetup.createNewAreaSetup(model);
                if (result == "INSERTED")
                {
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("GetAllAreaCode");
                    keystart.Add("GetAllAreaSetupByFilter");
                    keystart.Add("getAreaCodeWithChild");
                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "INSERTED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else if (result == "INVALIDINSERTED")
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, new
                    {
                        MESSAGE = "INVALIDINSERTED",
                        STATUS_CODE = (int)HttpStatusCode.Conflict
                    });
                }

                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpPost]
        public HttpResponseMessage updateAreaSetup(AreaModels model)
        {
            try
            {
                var result = this._iDocumentSetup.updateAreaSetup(model);
                if (result == "UPDATED")
                {
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("GetAllAreaCode");
                    keystart.Add("GetAllAreaSetupByFilter");
                    keystart.Add("getAreaCodeWithChild");
                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        [HttpGet]
        public HttpResponseMessage getAreaDetailsByAreaCode(string areaCode)
        {
            try
            {
                var result = this._iDocumentSetup.GetAccountDataByAccCode(areaCode);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        [HttpGet]
        public HttpResponseMessage getMaxAreaCode()
        {
            try
            {
                var result = this._iDocumentSetup.getMaxAreaCode();
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        [HttpGet]
        public List<AreaModels> GetAllAreaCode()
        {

            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<AreaModels>();
            if (this._cacheManager.IsSet($"GetAllAreaCode_{userid}_{company_code}_{branch_code}"))
            {
                var data = _cacheManager.Get<List<AreaModels>>($"GetAllAreaCode_{userid}_{company_code}_{branch_code}");
                response = data;
            }
            else
            {
                var getAllAreaCodeList = this._iDocumentSetup.getAllAreaCodeDetail();
                this._cacheManager.Set($"GetAllAreaCode_{userid}_{company_code}_{branch_code}", getAllAreaCodeList, 20);
                response = getAllAreaCodeList;
            }
            return response;
            //var result = this._iDocumentSetup.getAllAreaCodeDetail();
            //return result;
        }
        #endregion

        #region Agent
        [HttpPost]
        public HttpResponseMessage deleteAgentSetup(string agentCode)
        {
            try
            {
                var result = _iDocumentSetup.deleteAgentSetup(agentCode);
                #region CLEAR CACHE
                List<string> keystart = new List<string>();
                keystart.Add("getAllAgents");
                keystart.Add("getAgentCodeWithChild");
                List<string> Record = new List<string>();
                Record = this._cacheManager.GetAllKeys();
                this._cacheManager.RemoveCacheByKey(keystart, Record);
                #endregion
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        [HttpPost]
        public HttpResponseMessage createNewAgentSetup(AgentModels model)
        {
            try
            {
                var result = this._iDocumentSetup.createNewAgentSetup(model);
                if (result == "INSERTED")
                {
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("getAllAgents");
                    keystart.Add("getAgentCodeWithChild");
                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "INSERTED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else if (result == "INVALIDINSERTED")
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, new
                    {
                        MESSAGE = "INVALIDINSERTED",
                        STATUS_CODE = (int)HttpStatusCode.Conflict
                    });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpPost]
        public HttpResponseMessage updateAgentSetup(AgentModels model)
        {
            try
            {
                var result = this._iDocumentSetup.updateAgentSetup(model);
                if (result == "UPDATED")
                {
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("getAllAgents");
                    keystart.Add("getAgentCodeWithChild");
                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        [HttpGet]
        public HttpResponseMessage getMaxAgentCode()
        {
            try
            {
                var result = this._iDocumentSetup.getMaxAgentCode();
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        [HttpGet]
        public List<AgentModels> GetAllAgentCode()
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<AgentModels>();
            if (this._cacheManager.IsSet($"GetAllAgentCode_{userid}_{company_code}_{branch_code}"))
            {
                var data = _cacheManager.Get<List<AgentModels>>($"GetAllAgentCode_{userid}_{company_code}_{branch_code}");
                response = data;
            }
            else
            {
                var getAllAgentCodeList = this._iDocumentSetup.getAllAgentCodeDetail();
                this._cacheManager.Set($"GetAllAgentCode_{userid}_{company_code}_{branch_code}", getAllAgentCodeList, 20);
                response = getAllAgentCodeList;
            }
            return response;
            //var result = this._iDocumentSetup.getAllAgentCodeDetail();
            //return result;
        }
        [HttpGet]
        public List<AgentModels> getAllAgents(string filter)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<AgentModels>();
            if (this._cacheManager.IsSet($"getAllAgents_{userid}_{company_code}_{branch_code}_{filter}"))
            {
                var data = _cacheManager.Get<List<AgentModels>>($"getAllAgents_{userid}_{company_code}_{branch_code}_{filter}");
                response = data;
            }
            else
            {
                var getAllAgentsList = this._iDocumentSetup.getAllAgents(filter);
                this._cacheManager.Set($"getAllAgents_{userid}_{company_code}_{branch_code}_{filter}", getAllAgentsList, 20);
                response = getAllAgentsList;
            }
            return response;
            //var result = this._iDocumentSetup.getAllAgents(filter);
            //return result;
        }
        #endregion

        #region Transporter
        [HttpPost]
        public HttpResponseMessage deleteTransporterSetup(string transporterCode)
        {
            try
            {
                var result = _iDocumentSetup.deleteTransporterSetup(transporterCode);
                #region CLEAR CACHE
                List<string> keystart = new List<string>();
                keystart.Add("GetAllTransporterCode");
                keystart.Add("getTransporterCodeWithChild");
                List<string> Record = new List<string>();
                Record = this._cacheManager.GetAllKeys();
                this._cacheManager.RemoveCacheByKey(keystart, Record);
                #endregion
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        [HttpPost]
        public HttpResponseMessage createNewTransporterSetup(TransporterModels model)
        {
            try
            {
                var result = this._iDocumentSetup.createNewTransporterSetup(model);
                if (result == "INSERTED")
                {
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("GetAllTransporterCode");
                    keystart.Add("getTransporterCodeWithChild");
                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "INSERTED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else if (result == "INVALIDINSERTED")
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, new
                    {
                        MESSAGE = "INVALIDINSERTED",
                        STATUS_CODE = (int)HttpStatusCode.Conflict
                    });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpPost]
        public HttpResponseMessage updateTransporterSetup(TransporterModels model)
        {
            try
            {
                var result = this._iDocumentSetup.updateTransporterSetup(model);
                if (result == "UPDATED")
                {
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("GetAllTransporterCode");
                    keystart.Add("getTransporterCodeWithChild");
                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        [HttpGet]
        public HttpResponseMessage getMaxTransporterCode()
        {
            try
            {
                var result = this._iDocumentSetup.getMaxTransporterCode();
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        [HttpGet]
        public List<TransporterModels> GetAllTransporterCode()
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<TransporterModels>();
            if (this._cacheManager.IsSet($"GetAllTransporterCode_{userid}_{company_code}_{branch_code}"))
            {
                var data = _cacheManager.Get<List<TransporterModels>>($"GetAllTransporterCode_{userid}_{company_code}_{branch_code}");
                response = data;
            }
            else
            {
                var getAllTransporterCodeList = this._iDocumentSetup.getAllTransporterCodeDetail();
                this._cacheManager.Set($"GetAllTransporterCode_{userid}_{company_code}_{branch_code}", getAllTransporterCodeList, 20);
                response = getAllTransporterCodeList;
            }
            return response;
            //var result = this._iDocumentSetup.getAllTransporterCodeDetail();
            //return result;
        }
        #endregion

        #region Supplier 
        [HttpGet]
        public HttpResponseMessage getsupplierDetailsBysupplierCode(string suppliercode)
        {
            try
            {
                //var result = this._iDocumentSetup.GetSupplierDataBysupplierCode(suppliercode);
                var result = this._iDocumentSetup.GetSupplierDataBySupplierCode(suppliercode);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        [HttpGet]
        public List<SuplierSetupModel> GetChildOfsupplierByGroup(string groupCode)
        {
            //var userid = _workContext.CurrentUserinformation.User_id;
            //var company_code = _workContext.CurrentUserinformation.company_code;
            //var branch_code = _workContext.CurrentUserinformation.branch_code;
            //var response = new List<SuplierSetupModel>();
            //if (this._cacheManager.IsSet($"GetChildOfsupplierByGroup_{userid}_{company_code}_{branch_code}_{groupId}"))
            //{
            //    var data = _cacheManager.Get<List<SuplierSetupModel>>($"GetChildOfsupplierByGroup_{userid}_{company_code}_{branch_code}_{groupId}");
            //    response = data;
            //}
            //else
            //{
            //    var getSupplyListByGroupCodeList = this._iDocumentSetup.GetSupplyListByGroupCode(groupId);
            //    this._cacheManager.Set($"GetChildOfsupplierByGroup_{userid}_{company_code}_{branch_code}_{groupId}", getSupplyListByGroupCodeList, 20);
            //    response = getSupplyListByGroupCodeList;
            //}
            //return response;
            var result = this._iDocumentSetup.GetSupplyListByGroupCode(groupCode);
            return result;
        }
        [HttpGet]
        public List<SuplierSetupModel> GetAllSupplyList(string searchText)
        {
            return this._iDocumentSetup.GetAllSupplyList(searchText);
        }
        [HttpPost]
        public HttpResponseMessage createNewsupplier(SuplierSetupModalSet suplierSetupModalSet)
        {
            try
            {
                var result = this._iDocumentSetup.createNewSupplierSetup(suplierSetupModalSet);
                if (result == "INSERTED")
                {
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("GetChildOfsupplierByGroup");
                    keystart.Add("GetSuppliers");
                    keystart.Add("AllSupplierForReferenceByCode");
                    keystart.Add("AllSupplierForReferenceByName");
                    keystart.Add("AllSupplierForReferenceByaddress");
                    keystart.Add("AllSupplierForReferenceByphoneno");
                    keystart.Add("AllFilterSupplierForReference");
                    keystart.Add("AllSupplierSetupByCode");
                    keystart.Add("AllSupplierSetupByName");
                    keystart.Add("AllSupplierSetupByAddress");
                    keystart.Add("AllSupplierSetupByPhoneno");
                    keystart.Add("AllFilterSupplier");
                    keystart.Add("getsupplierCodeWithChild");
                    keystart.Add("GetSupplierListBySupplierCode");
                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "INSERTED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else if (result == "INVALIDINSERTED")
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, new
                    {
                        MESSAGE = "INVALIDINSERTED",
                        STATUS_CODE = (int)HttpStatusCode.Conflict
                    });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        [HttpPost]
        public HttpResponseMessage updatesupplierbysupplierCode(SuplierSetupModalSet model)
        {
            try
            {
                var result = this._iDocumentSetup.udpateSupplierSetup(model);
                if (result == "UPDATED")
                {
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("GetChildOfsupplierByGroup");
                    keystart.Add("GetSuppliers");
                    keystart.Add("AllSupplierForReferenceByCode");
                    keystart.Add("AllSupplierForReferenceByName");
                    keystart.Add("AllSupplierForReferenceByaddress");
                    keystart.Add("AllSupplierForReferenceByphoneno");
                    keystart.Add("AllFilterSupplierForReference");
                    keystart.Add("AllSupplierSetupByCode");
                    keystart.Add("AllSupplierSetupByName");
                    keystart.Add("AllSupplierSetupByAddress");
                    keystart.Add("AllSupplierSetupByPhoneno");
                    keystart.Add("AllFilterSupplier");
                    keystart.Add("getsupplierCodeWithChild");
                    keystart.Add("GetSupplierListBySupplierCode");
                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "UPDATED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        [HttpPost]
        public HttpResponseMessage DeletesuppliersetupBysuppliercode(string suppliercode)
        {
            try
            {
                var result = _iDocumentSetup.DeleteSupplierSetupBySupplierCode(suppliercode);
                #region CLEAR CACHE
                List<string> keystart = new List<string>();
                keystart.Add("GetChildOfsupplierByGroup");
                keystart.Add("GetSuppliers");
                keystart.Add("AllSupplierForReferenceByCode");
                keystart.Add("AllSupplierForReferenceByName");
                keystart.Add("AllSupplierForReferenceByaddress");
                keystart.Add("AllSupplierForReferenceByphoneno");
                keystart.Add("AllFilterSupplierForReference");
                keystart.Add("AllSupplierSetupByCode");
                keystart.Add("AllSupplierSetupByName");
                keystart.Add("AllSupplierSetupByAddress");
                keystart.Add("AllSupplierSetupByPhoneno");
                keystart.Add("AllFilterSupplier");
                keystart.Add("getsupplierCodeWithChild");
                keystart.Add("GetSupplierListBySupplierCode");
                List<string> Record = new List<string>();
                Record = this._cacheManager.GetAllKeys();
                this._cacheManager.RemoveCacheByKey(keystart, Record);
                #endregion
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        [HttpGet]
        public HttpResponseMessage getNewSupplierId()
        {
            try
            {
                var result = this._iDocumentSetup.getNewSupplierCode();
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "OK", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }

        [HttpGet]
        public List<SupplierModel> getAllSupplier(string filter)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<SupplierModel>();
            if (this._cacheManager.IsSet($"getAllSupplier_{userid}_{company_code}_{branch_code}_{filter}"))
            {
                var data = _cacheManager.Get<List<SupplierModel>>($"getAllSupplier_{userid}_{company_code}_{branch_code}_{filter}");
                response = data;
            }
            else
            {
                var getAllSupplierList = this._iDocumentSetup.getAllSupplier(filter);
                this._cacheManager.Set($"getAllSupplier_{userid}_{company_code}_{branch_code}_{filter}", getAllSupplierList, 20);
                response = getAllSupplierList;
            }
            return response;
            //var result = this._iDocumentSetup.getAllPartyTypes(filter);
            //return result;
        }

        #endregion

        #region Division Setup
        // Delete Division Setup
        [HttpPost]
        public HttpResponseMessage DeleteDivisionSetupByDivisionCode(string divisionCode)
        {
            try
            {
                var result = _iDocumentSetup.DeleteDivisionCenterByDivisionCode(divisionCode);


                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        //create Division
        [HttpPost]
        public HttpResponseMessage createNewDivisionHead(DivisionSetupModel model)
        {
            try
            {
                var result = this._iDocumentSetup.createNewDivisionSetup(model);
                if (result == "INSERTED")
                {

                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "INSERTED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else if (result == "INVALIDINSERTED")
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, new
                    {
                        MESSAGE = "INVALIDINSERTED",
                        STATUS_CODE = (int)HttpStatusCode.Conflict
                    });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        //Update division

        [HttpPost]
        public HttpResponseMessage updateDivisionCode(DivisionSetupModel model)
        {
            try
            {
                var result = this._iDocumentSetup.udpateDivisionSetup(model);
                if (result == "UPDATED")
                {

                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "UPDATED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }


        //Division
        [HttpGet]
        public HttpResponseMessage getDivisionDetailsByDivisionCode(string divisionCode)
        {
            try
            {
                var result = this._iDocumentSetup.GetDivisionCenterDetailByDivisionCode(divisionCode);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }


        //child grid
        [HttpGet]
        public List<DivisionSetupModel> GetChildOfDivisionByGroup(string groupId)
        {
            var result = this._iDocumentSetup.getAllDivisionschild(groupId);
            return result;
        }
        [HttpGet]
        public List<DivisionSetupModel> getAllDivisionsList(string searchText)
        {
            return this._iDocumentSetup.getAllDivisionsList(searchText);
        }
        #endregion

        #region Branch Setup
        //Delete Branch
        [HttpPost]
        public HttpResponseMessage DeleteBranchCenterSetupByBranchCode(string branchCode)
        {
            try
            {
                var result = _iDocumentSetup.DeleteBranchCenterByBranchCode(branchCode);


                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }

        //Create Branch Setup
        [HttpPost]
        public HttpResponseMessage createNewBranchHead(BranchSetupModel model)
        {
            try
            {
                var result = this._iDocumentSetup.createNewBranchSetup(model);
                if (result == "INSERTED")
                {

                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "INSERTED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else if (result == "INVALIDINSERTED")
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, new
                    {
                        MESSAGE = "INVALIDINSERTED",
                        STATUS_CODE = (int)HttpStatusCode.Conflict
                    });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        //Update Branch setup
        [HttpPost]
        public HttpResponseMessage updateBranchByBranchCode(BranchSetupModel model)
        {
            try
            {
                var result = this._iDocumentSetup.udpateBranchSetup(model);
                if (result == "UPDATED")
                {

                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "UPDATED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }


        [HttpGet]
        public HttpResponseMessage getBranchCenterDetailBybranchCode(string branchCode)
        {
            try
            {
                var result = this._iDocumentSetup.GetBranchCenterDetailBybranchCode(branchCode);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }


        //BranchChild Grid data
        [HttpGet]
        public List<BranchSetupModel> GetChildOfBranchCenterByGroup(string groupId)
        {
            var result = this._iDocumentSetup.GetBranchCenterListByGroupCode(groupId);
            return result;
        }
        [HttpGet]
        public List<BranchSetupModel> GetAllBranchCenterList(string searchText)
        {
            return this._iDocumentSetup.GetAllBranchCenterList(searchText);
        }

        #endregion

        #region Dealer Setup
        //customer
        [HttpGet]
        public List<DealerCustomerMapModel> GetCustomerMappingForDealer(string preCustomerCode = "", string partyTypeCode = "", string filter = "")
        {
            var result = this._iDocumentSetup.GetCustomerMappingForDealer(preCustomerCode, partyTypeCode, filter);
            return result;
        }
        [HttpGet]
        public List<DealerCustomerMapModel> getCustomerMapped(string partyTypeCode)
        {
            var result = this._iDocumentSetup.getCustomerMapped(partyTypeCode);
            return result;
        }
        [HttpGet]
        public List<CustomerSetupModel> GetChildOfCustomerByGroup1(string dealercode = "")
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<CustomerSetupModel>();
            if (string.IsNullOrEmpty(dealercode))
            {
                var results = this._iDocumentSetup.GetAccountListByCustomerCode123();
                return results;
            }
            var result = this._iDocumentSetup.GetAccountListByCustomerCodeByDealerCode(dealercode);
            return result;
        }

        [HttpPost]
        public HttpResponseMessage updateDealerCode(DealerModel model)
        {
            try
            {
                var result = this._iDocumentSetup.udpateDealerSetup(model);
                if (result == "UPDATED")
                {

                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "UPDATED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        [HttpPost]
        public HttpResponseMessage createCustomerMapped(MappedCustomerModel model)
        {
            try
            {
                var result = this._iDocumentSetup.CreateNewCustomerMap(model);
                if (result == "INSERTED")
                {

                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "INSERTED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }


        [HttpPost]
        public HttpResponseMessage createNewDealer(DealerModel model)
        {
            try
            {
                var result = this._iDocumentSetup.createNewDealerSetup(model);
                if (result == "INSERTED")
                {

                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "INSERTED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else if (result == "INVALIDINSERTED")
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, new
                    {
                        MESSAGE = "INVALIDINSERTED",
                        STATUS_CODE = (int)HttpStatusCode.Conflict
                    });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpPost]
        public HttpResponseMessage DeleteDealer(string dealerCode)
        {
            try
            {
                var result = _iDocumentSetup.DeleteDealerCenterByDealerCode(dealerCode);


                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }


        [HttpGet]
        public HttpResponseMessage getDealerData(string dealerCode)
        {
            try
            {
                var result = this._iDocumentSetup.GetDealerDetailBydealerCode(dealerCode);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        [HttpGet]
        public HttpResponseMessage getDealerData()
        {
            try
            {
                var result = this._iDocumentSetup.GetDealerDetailBydealerCode();
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }


        [HttpGet]
        public List<DealerModel> GetChildOfDealerByGroup(string groupCode)
        {

            var result = this._iDocumentSetup.GetDealerListByGroupCode(groupCode);

            return result;


        }
        //Grid data from mapped dealer

        [HttpGet]
        public List<CustSubList> MappedDealerData(string dealerCode)
        {

            var result = this._iDocumentSetup.GetDealerMapped(dealerCode);

            return result;


        }
        #endregion

        #region Party type setup
        public List<AccountCodeModels> getAccountCodPrtyType()
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<AccountCodeModels>();
            if (this._cacheManager.IsSet($"getAccountCodeWithChild_{userid}_{company_code}_{branch_code}"))
            {
                var data = _cacheManager.Get<List<AccountCodeModels>>($"getAccountCodeWithChild_{userid}_{company_code}_{branch_code}");
                response = data;
            }
            else
            {
                var AllAccountCode = _iDocumentSetup.getAllAccountCodeParty();
                this._cacheManager.Set($"getAccountCodeWithChild_{userid}_{company_code}_{branch_code}", AllAccountCode, 20);
                response = AllAccountCode;
            }
            return response;
        }

        [HttpGet]
        public List<PartyTypeModel> partyTypeList()
        {

            var result = this._iDocumentSetup.partyTypeList();

            return result;


        }

        [HttpGet]
        public string GetMaxPartyTypeCode()
        {

            var result = this._iDocumentSetup.GetMaxPartyTypeCode();

            return result;
        }

        [HttpPost]
        public HttpResponseMessage createNewPaetyType(PartyTypeModel model)
        {
            try
            {
                var result = this._iDocumentSetup.createNewPaetyType(model);
                if (result == "INSERTED")
                {

                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "INSERTED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else if (result == "INVALIDINSERTED")
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, new
                    {
                        MESSAGE = "INVALIDINSERTED",
                        STATUS_CODE = (int)HttpStatusCode.Conflict
                    });
                }

                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }


        [HttpPost]
        public HttpResponseMessage updatePartyType(PartyTypeModel model)
        {
            try
            {
                var result = this._iDocumentSetup.updatePartyType(model);
                if (result == "UPDATED")
                {

                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "UPDATED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }

        [HttpPost]
        public HttpResponseMessage deletePartySetup(string partyCode)
        {
            try
            {
                var result = _iDocumentSetup.deletePartySetup(partyCode);

                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        #endregion

        #region Budget Center
        [HttpGet]
        public HttpResponseMessage getMaxBudgetCenterCode(string gFlag)
        {
            try
            {
                var result = this._iDocumentSetup.GetMaxBudgetCenterCode(gFlag);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpPost]
        public HttpResponseMessage DeleteBudgetCenterSetupByBudgetCode(string budgetCode)
        {
            try
            {
                var result = _iDocumentSetup.DeleteBudgetCenterByBudgetCode(budgetCode);
                #region CLEAR CACHE
                List<string> keystart = new List<string>();
                keystart.Add("GetChildOfBudgetCenterByGroup");
                keystart.Add("GetAllBudgetCenterByFilter");
                keystart.Add("GetAllBudgetCenterForLocationByFilter");
                keystart.Add("getBudgetCodeByAccCode");
                keystart.Add("checkBudgetFlagByLocationCode");
                keystart.Add("getBudgetCenterCodeWithChild");
                keystart.Add("getbudgetCenterCode");
                keystart.Add("getAllBudgetCenter");
                List<string> Record = new List<string>();
                Record = this._cacheManager.GetAllKeys();
                this._cacheManager.RemoveCacheByKey(keystart, Record);
                #endregion

                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        [HttpPost]
        public HttpResponseMessage updateBudgetByAccCode(BudgetCenterSetupModel model)
        {
            try
            {
                var result = this._iDocumentSetup.udpateBudgetSetup(model);
                if (result == "UPDATED")
                {
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("GetChildOfBudgetCenterByGroup");
                    keystart.Add("getAllBudgetCenter");
                    keystart.Add("GetAllBudgetCenterByFilter");
                    keystart.Add("GetAllBudgetCenterForLocationByFilter");
                    keystart.Add("getBudgetCodeByAccCode");
                    keystart.Add("checkBudgetFlagByLocationCode");
                    keystart.Add("getBudgetCenterCodeWithChild");
                    keystart.Add("getbudgetCenterCode");
                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "UPDATED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        [HttpPost]
        public HttpResponseMessage createNewBudgetHead(BudgetCenterSetupModel model)
        {
            try
            {
                var result = this._iDocumentSetup.createNewBudgetSetup(model);
                if (result == "INSERTED")
                {
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("GetChildOfBudgetCenterByGroup");
                    keystart.Add("getAllBudgetCenter");
                    keystart.Add("GetAllBudgetCenterByFilter");
                    keystart.Add("GetAllBudgetCenterForLocationByFilter");
                    keystart.Add("getBudgetCodeByAccCode");
                    keystart.Add("checkBudgetFlagByLocationCode");
                    keystart.Add("getBudgetCenterCodeWithChild");
                    keystart.Add("getbudgetCenterCode");
                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "INSERTED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else if (result == "INVALIDINSERTED")
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, new
                    {
                        MESSAGE = "INVALIDINSERTED",
                        STATUS_CODE = (int)HttpStatusCode.Conflict
                    });
                }

                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        [HttpGet]
        public HttpResponseMessage getBudgetCenterDetailBybudgetCode(string budgetCode)
        {
            try
            {
                var result = this._iDocumentSetup.GetBudgetCenterDetailByBudgetCode(budgetCode);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }











        [HttpGet]
        public List<BudgetCenterSetupModel> GetChildOfBudgetCenterByGroup(string groupId)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<BudgetCenterSetupModel>();
            if (this._cacheManager.IsSet($"GetChildOfBudgetCenterByGroup_{userid}_{company_code}_{branch_code}_{groupId}"))
            {
                var data = _cacheManager.Get<List<BudgetCenterSetupModel>>($"GetChildOfBudgetCenterByGroup_{userid}_{company_code}_{branch_code}_{groupId}");
                response = data;
            }
            else
            {
                var budgetCenterListByGroupCodeList = this._iDocumentSetup.GetBudgetCenterListByGroupCode(groupId);
                this._cacheManager.Set($"GetChildOfBudgetCenterByGroup_{userid}_{company_code}_{branch_code}_{groupId}", budgetCenterListByGroupCodeList, 20);
                response = budgetCenterListByGroupCodeList;
            }
            return response;
            //var result = this._iDocumentSetup.GetBudgetCenterListByGroupCode(groupId);
            //return result;
        }
        [HttpGet]
        public List<BudgetCenterSetupModel> GetAllBudgetCenterList(string searchText)
        {
            return this._iDocumentSetup.GetAllBudgetCenterList(searchText);
        }

        [HttpGet]
        public List<BudgetCenter> getAllBudgetCenter(string filter)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<BudgetCenter>();
            if (this._cacheManager.IsSet($"getAllBudgetCenter_{userid}_{company_code}_{branch_code}_{filter}"))
            {
                var data = _cacheManager.Get<List<BudgetCenter>>($"getAllBudgetCenter_{userid}_{company_code}_{branch_code}_{filter}");
                response = data;
            }
            else
            {
                var getAllBudgetCenterList = this._iDocumentSetup.getAllBudgetCenter(filter);
                this._cacheManager.Set($"getAllBudgetCenter_{userid}_{company_code}_{branch_code}_{filter}", getAllBudgetCenterList, 20);
                response = getAllBudgetCenterList;
            }
            return response;
            //var result = this._iDocumentSetup.getAllBudgetCenter(filter);
            //return result;
        }
        #endregion

        #region Company Setup
        //company setup
        [HttpPost]
        public HttpResponseMessage updateCompanyHead(CompanySetupModel model)
        {
            try
            {
                var result = this._iDocumentSetup.updateCompanySetup(model);
                if (result == "UPDATED")
                {

                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "UPDATED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }

        [HttpPost]
        public HttpResponseMessage createNewCompanyHead(CompanySetupModel model)
        {
            try
            {
                var result = this._iDocumentSetup.createNewCompanySetup(model);
                if (result == "INSERTED")
                {
                    var uploadsFolder = HttpContext.Current.Server.MapPath("~/uploads/");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    //var httpRequest = HttpContext.Current.Request;
                    //var companyLogo = httpRequest.Files["companyLogo"];
                    //var footerLogo = httpRequest.Files["footerLogo"];
                    //var data = httpRequest["companyData"];
                    //var companyData = JsonConvert.DeserializeObject<CompanySetupModel>(data);

                    //if (companyLogo != null)
                    //{
                    //    var logoFileName = Path.GetFileNameWithoutExtension(companyLogo.FileName);
                    //    var logoExtension = Path.GetExtension(companyLogo.FileName);
                    //    var logoUniqueName = $"{logoFileName}_{Guid.NewGuid()}{logoExtension}";
                    //    var logoPath = Path.Combine(uploadsFolder, logoUniqueName);
                    //    companyLogo.SaveAs(logoPath);
                    //    companyData.LOGO_FILE_NAME = "/uploads/" + logoUniqueName;
                    //}
                    //if (footerLogo != null)
                    //{
                    //    var footerFileName = Path.GetFileNameWithoutExtension(footerLogo.FileName);
                    //    var footerExtension = Path.GetExtension(footerLogo.FileName);
                    //    var footerUniqueName = $"{footerFileName}_{Guid.NewGuid()}{footerExtension}";
                    //    var footerPath = Path.Combine(uploadsFolder, footerUniqueName);
                    //    footerLogo.SaveAs(footerPath);
                    //    companyData.FOOTER_LOGO_FILE_NAME = "/uploads/" + footerUniqueName;
                    //}
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "INSERTED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else if (result == "INVALIDINSERTED")
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, new
                    {
                        MESSAGE = "INVALIDINSERTED",
                        STATUS_CODE = (int)HttpStatusCode.Conflict
                    });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        //[HttpPost]
        //public HttpResponseMessage createNewCompanyHead()
        //{
        //    try
        //    {
        //        //var httpRequest = HttpContext.Current.Request;
        //        //var companyDataJson = httpRequest["companyData"];
        //        //var companyData = JsonConvert.DeserializeObject<CompanySetupModel>(companyDataJson);
        //        var result = _iDocumentSetup.createNewCompanySetup(companyData);
        //        if (result == "INSERTED")
        //        {
        //            string uploadsFolder = HttpContext.Current.Server.MapPath("~/uploads/");
        //            if (!Directory.Exists(uploadsFolder))
        //                Directory.CreateDirectory(uploadsFolder);

        //            // Handle company logo file
        //            var companyLogo = httpRequest.Files["companyLogo"];
        //            if (companyLogo != null && companyLogo.ContentLength > 0)
        //            {
        //                var logoFileName = $"{Path.GetFileNameWithoutExtension(companyLogo.FileName)}_{Guid.NewGuid()}{Path.GetExtension(companyLogo.FileName)}";
        //                var logoPath = Path.Combine(uploadsFolder, logoFileName);
        //                companyLogo.SaveAs(logoPath);
        //                companyData.LOGO_FILE_NAME = "/uploads/" + logoFileName;
        //            }

        //            // Handle footer logo file
        //            var footerLogo = httpRequest.Files["footerLogo"];
        //            if (footerLogo != null && footerLogo.ContentLength > 0)
        //            {
        //                var footerFileName = $"{Path.GetFileNameWithoutExtension(footerLogo.FileName)}_{Guid.NewGuid()}{Path.GetExtension(footerLogo.FileName)}";
        //                var footerPath = Path.Combine(uploadsFolder, footerFileName);
        //                footerLogo.SaveAs(footerPath);
        //                companyData.FOOTER_LOGO_FILE_NAME = "/uploads/" + footerFileName;
        //            }

        //            // Optional: Re-save or update logos in DB if needed
        //            //_iDocumentSetup.updateCompanyLogos(companyData);

        //            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "INSERTED", STATUS_CODE = (int)HttpStatusCode.OK });
        //        }
        //        else if (result == "INVALIDINSERTED")
        //        {
        //            return Request.CreateResponse(HttpStatusCode.Conflict, new { MESSAGE = "INVALIDINSERTED", STATUS_CODE = (int)HttpStatusCode.Conflict });
        //        }
        //        else
        //        {
        //            return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
        //    }
        //}

        //[HttpPost]
        //public HttpResponseMessage createNewCompanyHead(CompanySetupModel model)
        //{
        //    try
        //    {
        //        var result = this._iDocumentSetup.createNewCompanySetup(model);
        //        if (result == "INSERTED")
        //        {
        //            var uploadsFolder = HttpContext.Current.Server.MapPath("~/uploads/");
        //            if (!Directory.Exists(uploadsFolder))
        //            {
        //                Directory.CreateDirectory(uploadsFolder);
        //            }
        //            var httpRequest = HttpContext.Current.Request;
        //            var companyLogo = httpRequest.Files["companyLogo"];
        //            var footerLogo = httpRequest.Files["footerLogo"];
        //            var data = httpRequest["companyData"];
        //            var companyData = JsonConvert.DeserializeObject<CompanySetupModel>(data);

        //            if (companyLogo != null)
        //            {
        //                var logoFileName = Path.GetFileNameWithoutExtension(companyLogo.FileName);
        //                var logoExtension = Path.GetExtension(companyLogo.FileName);
        //                var logoUniqueName = $"{logoFileName}_{Guid.NewGuid()}{logoExtension}";
        //                var logoPath = Path.Combine(uploadsFolder, logoUniqueName);
        //                companyLogo.SaveAs(logoPath);
        //                companyData.LOGO_FILE_NAME = "/uploads/" + logoUniqueName;
        //            }
        //            if (footerLogo != null)
        //            {
        //                var footerFileName = Path.GetFileNameWithoutExtension(footerLogo.FileName);
        //                var footerExtension = Path.GetExtension(footerLogo.FileName);
        //                var footerUniqueName = $"{footerFileName}_{Guid.NewGuid()}{footerExtension}";
        //                var footerPath = Path.Combine(uploadsFolder, footerUniqueName);
        //                footerLogo.SaveAs(footerPath);
        //                companyData.FOOTER_LOGO_FILE_NAME = "/uploads/" + footerUniqueName;
        //            }
        //            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "INSERTED", STATUS_CODE = (int)HttpStatusCode.OK });
        //        }
        //        else if (result == "INVALIDINSERTED")
        //        {
        //            return Request.CreateResponse(HttpStatusCode.Conflict, new
        //            {
        //                MESSAGE = "INVALIDINSERTED",
        //                STATUS_CODE = (int)HttpStatusCode.Conflict
        //            });
        //        }
        //        else
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
        //        }

        //    }
        //    catch (Exception ex)
        //    {

        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
        //    }
        //}

        [HttpGet]
        public List<CompanySetupModel> GetCompanyGridData()
        {

            var result = this._iDocumentSetup.getAllCompanychild();

            return result;


        }

        [HttpPost]
        public HttpResponseMessage DeleteCompany(string companyCode)
        {
            try
            {
                var result = _iDocumentSetup.DeleteCompanyByCompanyCode(companyCode);


                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        //company
        [HttpGet]
        public HttpResponseMessage getCompanyDetailsByCompanyCode(string cmpanyId)
        {
            try
            {
                var result = this._iDocumentSetup.GetCompnyDetailByCompanyCode(cmpanyId);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        #endregion

        #region Preference Setup



        //preferencesetup 

        [HttpGet]
        public HttpResponseMessage getDatabaseList()
        {
            try
            {
                var result = this._iDocumentSetup.GetDatabaseList();
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }

        //delete  preff
        [HttpPost]
        public HttpResponseMessage DeletePreference(string companyCode)
        {
            try
            {
                var result = _iDocumentSetup.DeletepreffBybranchCode(companyCode);


                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        //update
        [HttpPost]
        public HttpResponseMessage updatePreference(PreferenceModel model)
        {
            try
            {
                var result = this._iDocumentSetup.updatePreferenceSetup(model, _workContext.CurrentUserinformation);
                if (result == "UPDATED")
                {

                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "UPDATED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        //save
        [HttpPost]
        public HttpResponseMessage createPreferenceSetup(PreferenceModel model)
        {
            try
            {
                var result = this._iDocumentSetup.createNewPreferenceSetup(model);
                if (result == "INSERTED")
                {

                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "INSERTED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else if (result == "INVALIDINSERTED")
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, new
                    {
                        MESSAGE = "INVALIDINSERTED",
                        STATUS_CODE = (int)HttpStatusCode.Conflict
                    });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }


        [HttpGet]
        public HttpResponseMessage getpreferenceDetailsByCompanyCode(string cmpanyId)
        {
            try
            {
                var result = this._iDocumentSetup.GetPreferenceDetailByCompanyCode(cmpanyId);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }

        [HttpGet]
        public HttpResponseMessage getPreferenceDocumentSetupOptions()
        {
            try
            {
                var result = this._iDocumentSetup.GetPreferenceDocumentSetupOptions();
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        //preference grid data 
        [HttpGet]
        public List<PreferenceModel> GetPrrffGridData()
        {

            var result = this._iDocumentSetup.getAllPreference();

            return result;


        }


        //Preference form load  
        [HttpGet]
        public List<PreferenceModel> GetPrefffromload(User userInf)
        {

            var result = this._iDocumentSetup.getFormLoad(_workContext.CurrentUserinformation);

            return result;


        }

        [HttpGet]
        public HttpResponseMessage GetCompanyPreff()
        {
            try
            {
                var data = _iDocumentSetup.getCompanyPreff();
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new List<string>());
            }
        }
        //currency
        [HttpGet]
        public List<CurrencymultiModel> currencymultiselect()
        {

            var result = this._iDocumentSetup.getAllCurrencymulti();

            return result;


        }


        ////branch 
        [HttpGet]
        public HttpResponseMessage GetBranchPreff(string COMPANY_CODE = "")
        {
            try
            {
                var data = _iDocumentSetup.getBranchPreff(COMPANY_CODE);
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new List<string>());
            }
        }

        #endregion


        #region Vehicle  Setup
        //VEHOICLE SETUP 
        [HttpGet]
        public List<VehicleSetupModel> getVehicleList()
        {

            var result = this._iDocumentSetup.getAllVehicle();

            return result;


        }

        [HttpGet]
        public string GetVehicleCode(string vehicletype)
        {

            var result = this._iDocumentSetup.GetVehicleCode1(vehicletype);

            return result;


        }


        //CREATE VEHICLE
        [HttpPost]
        public HttpResponseMessage createNewVehicleSetup(VehicleSetupModel model)
        {
            try
            {
                var result = this._iDocumentSetup.createVehicleSetup(model);
                if (result == "INSERTED")
                {

                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "INSERTED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else if (result == "INVALIDINSERTED")
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, new
                    {
                        MESSAGE = "INVALIDINSERTED",
                        STATUS_CODE = (int)HttpStatusCode.Conflict
                    });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        //UPDATE VEHICLE
        [HttpPost]
        public HttpResponseMessage updateNewVehicleSetup(VehicleSetupModel model)
        {
            try
            {
                var result = this._iDocumentSetup.updateVehicleSetup(model);
                if (result == "UPDATED")
                {

                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        //DELETE VEHICLE
        [HttpPost]
        public HttpResponseMessage deleteVehicleSetup(string vehicleCode)
        {
            try
            {
                var result = _iDocumentSetup.deleteVehicleSetups(vehicleCode);

                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }

        #endregion

        #region Vehicle Registration   Setup
        //vehicle Registration

        [HttpGet]
        public HttpResponseMessage getMaxTransactionCode(string gFlag)
        {
            try
            {
                var result = this._iDocumentSetup.getMaxTransactionNo(gFlag);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }


        [HttpGet]

        public List<VehicleRegistrationModel> GetVehicleRegistration(string from = "Others")
        {

            var result = this._iDocumentSetup.GetVehicleReg(from);

            return result;


        }

        //create Vehicle Registration
        [HttpPost]
        public HttpResponseMessage createVehicleRegistration(VehicleRegistrationModel model)
        {
            try
            {
                var result = this._iDocumentSetup.createNewVehicleReg(model);
                if (result == "INSERTED")
                {

                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "INSERTED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else if (result == "INVALIDINSERTED")
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, new
                    {
                        MESSAGE = "INVALIDINSERTED",
                        STATUS_CODE = (int)HttpStatusCode.Conflict
                    });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        //Update Vehicle Restration 

        [HttpPost]
        public HttpResponseMessage updateVehicleRegistration(VehicleRegistrationModel model)
        {
            try
            {
                var result = this._iDocumentSetup.updateVehicleReg(model);
                if (result == "UPDATED")
                {

                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        //Delete Vehicle Registration

        [HttpPost]
        public HttpResponseMessage DeleteVehicle(string vehicleCode)
        {
            try
            {
                var result = _iDocumentSetup.DeleteVehicleRegistration(vehicleCode);


                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }

        //Get vehicle reg
        [HttpGet]
        public HttpResponseMessage getVehicleDetailsByvehicleCode(string transactionCode)
        {
            try
            {
                var result = this._iDocumentSetup.GetVehicleDetailBytrCode(transactionCode);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }


        #endregion


        #region Create Customer and Item Gruop fro Opera Symphony
        //save Group customer for Opera and Symphiny
        [HttpPost]
        public HttpResponseMessage createNewCustomerGroup1(CustomerModels model)
        {
            try
            {
                var result = this._iDocumentSetup.createNewCustomerSetup1(model);
                if (result == "INSERTED")
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "INSERTED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        //Create Group item for Opera and Symphony

        [HttpPost]
        public HttpResponseMessage createNewitem1(ItemSetupModalSet model)
        {

            try
            {

                var result = this._iDocumentSetup.createNewItemSetup1(model.model);
                if (result == "INSERTED")
                {

                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "INSERTED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        #endregion

        #region upload
        [Route("SetupApi/PostUserImage")]
        [AllowAnonymous]
        public HttpResponseMessage PostUserImage(string id)
        {
            String fileSavePath = "";
            if (HttpContext.Current.Request.Files.AllKeys.Any())
            {
                // Get the uploaded image from the Files collection
                var httpPostedFile = HttpContext.Current.Request.Files["UploadedImage"];

                if (httpPostedFile != null)
                {
                    string strMappath = "~/Areas/NeoERP.DocumentTemplate/images/supplier/" + id + "/";
                    // Get the complete file path
                    fileSavePath = HttpContext.Current.Server.MapPath("~/Areas/NeoERP.DocumentTemplate/images/supplier/") + id + "\\" + httpPostedFile.FileName.ToString();

                    if (!Directory.Exists(strMappath))
                    {
                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath(strMappath));
                    }

                    // Save the uploaded file to "UploadedFiles" folder
                    httpPostedFile.SaveAs(fileSavePath);

                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "OK", STATUS_CODE = (int)HttpStatusCode.OK, DATA = fileSavePath });
        }
        [Route("SetupApi/ItemPostUserImage")]
        [AllowAnonymous]
        public HttpResponseMessage ItemPostUserImage(string id)
        {
            String fileSavePath = "";
            if (HttpContext.Current.Request.Files.AllKeys.Any())
            {
                // Get the uploaded image from the Files collection
                var httpPostedFile = HttpContext.Current.Request.Files["UploadedImage"];

                if (httpPostedFile != null)
                {
                    string strMappath = "~/Areas/NeoERP.DocumentTemplate/images/item/" + id + "/";
                    // Get the complete file path
                    fileSavePath = HttpContext.Current.Server.MapPath("~/Areas/NeoERP.DocumentTemplate/images/item/") + id + "\\" + httpPostedFile.FileName.ToString();

                    if (!Directory.Exists(strMappath))
                    {
                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath(strMappath));
                    }

                    // Save the uploaded file to "UploadedFiles" folder
                    httpPostedFile.SaveAs(fileSavePath);

                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "OK", STATUS_CODE = (int)HttpStatusCode.OK, DATA = fileSavePath });
        }
        #endregion

        #region FormControl

        [HttpGet]
        public List<FormControlModels> GetFormControlByFormCode(string formcode)
        {

            _logErp.InfoInFile("Get Form Control by Form code " + formcode + " is started");
            try
            {
                var userid = _workContext.CurrentUserinformation.User_id;
                var company_code = _workContext.CurrentUserinformation.company_code;
                var branch_code = _workContext.CurrentUserinformation.branch_code;
                var response = new List<FormControlModels>();
                if (this._cacheManager.IsSet($"GetFormControlByFormCode_{userid}_{company_code}_{branch_code}_{formcode}"))
                {
                    var data = _cacheManager.Get<List<FormControlModels>>($"GetFormControlByFormCode_{userid}_{company_code}_{branch_code}_{formcode}");
                    _logErp.InfoInFile("Form control by form code is fetched from cache");
                    response = data;
                }
                else
                {
                    var getFormControlByFormCodeList = this._iFormSetupRepo.GetFormControls(formcode);
                    _logErp.InfoInFile(getFormControlByFormCodeList.Count() + " form controls is fetched using : " + formcode);
                    this._cacheManager.Set($"GetFormControlByFormCode_{userid}_{company_code}_{branch_code}_{formcode}", getFormControlByFormCodeList, 20);
                    response = getFormControlByFormCodeList;
                }
                return response;

            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting form control by " + formcode + ex.Message);
                throw new Exception(ex.Message);
            }
            //var result = this._iFormSetupRepo.GetFormControls(formcode);
            //return result;
        }



        [HttpGet]
        public object GetNextScreenFormCode(string formcode, string orderno = "")
        {
            var result = this._iFormSetupRepo.GetNextScreenFormCodeDetails(formcode, orderno);
            return result;
        }

        [HttpGet]
        public object GetIssueScreenFormCode(string orderno = "")
        {
            var result = this._iFormSetupRepo.GetIssueScreenFormCodeDetails(orderno);
            return result;
        }


        #endregion

        #region Quick Setup
        [HttpPost]
        public HttpResponseMessage insertQuickSetup(QuickSetupModel model)
        {
            try
            {

                var result = this._iDocumentSetup.InsertQuickSetup(model);
                if (result == "C_SUCCESS")
                {
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("GetChildOfCustomerByGroup");
                    keystart.Add("getAllItemsForCustomerStock");
                    keystart.Add("getAllComboEmployees");
                    keystart.Add("getAllComboDealers");
                    keystart.Add("GetCustomers");
                    keystart.Add("AllCustomerSetupByCode");
                    keystart.Add("AllCustomerSetupByName");
                    keystart.Add("AllCustomerSetupByAddress");
                    keystart.Add("AllCustomerSetupByPhoneno");
                    keystart.Add("AllFilterCustomer");
                    keystart.Add("customerDropDownForGroupPopup");
                    keystart.Add("GetCustomerListByCustomerCode");
                    keystart.Add("GetAllCustomerSetupByFilter");
                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "C_SUCCESS", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else if (result == "I_SUCCESS")
                {
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("GetChildOfItemByGroup");
                    keystart.Add("GetProducts");
                    keystart.Add("GetItemChargeDataSavedValueWise");
                    keystart.Add("GetItemChargeDataSavedQuantityWise");
                    keystart.Add("GetInvItemChargesData");
                    keystart.Add("GetMUCodeByProductId");
                    keystart.Add("GetMuCode");
                    keystart.Add("GetProductListByItemCode");
                    keystart.Add("GetAllProductsListByFilter");
                    keystart.Add("getProductCodeWithChild");
                    keystart.Add("GetGroupProducts");
                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "I_SUCCESS", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else if (result == "S_SUCCESS")
                {
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("GetChildOfsupplierByGroup");
                    keystart.Add("GetSuppliers");
                    keystart.Add("AllSupplierForReferenceByCode");
                    keystart.Add("AllSupplierForReferenceByName");
                    keystart.Add("AllSupplierForReferenceByaddress");
                    keystart.Add("AllSupplierForReferenceByphoneno");
                    keystart.Add("AllFilterSupplierForReference");
                    keystart.Add("AllSupplierSetupByCode");
                    keystart.Add("AllSupplierSetupByName");
                    keystart.Add("AllSupplierSetupByAddress");
                    keystart.Add("AllSupplierSetupByPhoneno");
                    keystart.Add("AllFilterSupplier");
                    keystart.Add("getsupplierCodeWithChild");
                    keystart.Add("GetSupplierListBySupplierCode");
                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "S_SUCCESS", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        #endregion

        #region Party type
        [HttpGet]
        public List<PartyTypeModels> getAllPartyTypes(string filter)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<PartyTypeModels>();
            if (this._cacheManager.IsSet($"getAllPartyTypes_{userid}_{company_code}_{branch_code}_{filter}"))
            {
                var data = _cacheManager.Get<List<PartyTypeModels>>($"getAllPartyTypes_{userid}_{company_code}_{branch_code}_{filter}");
                response = data;
            }
            else
            {
                var getAllPartyTypesList = this._iDocumentSetup.getAllPartyTypes(filter);
                this._cacheManager.Set($"getAllPartyTypes_{userid}_{company_code}_{branch_code}_{filter}", getAllPartyTypesList, 20);
                response = getAllPartyTypesList;
            }
            return response;
            //var result = this._iDocumentSetup.getAllPartyTypes(filter);
            //return result;
        }
        #endregion

        #region currency
        [HttpGet]
        public List<Currency> getAllCurrency()
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<Currency>();
            if (this._cacheManager.IsSet($"getAllCurrency_{userid}_{company_code}_{branch_code}"))
            {
                var data = _cacheManager.Get<List<Currency>>($"getAllCurrency_{userid}_{company_code}_{branch_code}");
                response = data;
            }
            else
            {
                var ggetAllCurrencyList = this._iDocumentSetup.getAllCurrency();
                this._cacheManager.Set($"getAllCurrency_{userid}_{company_code}_{branch_code}", ggetAllCurrencyList, 20);
                response = ggetAllCurrencyList;
            }
            return response;
            //var result = this._iDocumentSetup.getAllCurrency();
            //return result;
        }
        #endregion

        #region Country/Zone/District
        [HttpGet]
        public List<ZoneModels> getAllZones(string filter)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<ZoneModels>();
            if (this._cacheManager.IsSet($"getAllZones_{userid}_{company_code}_{branch_code}_{filter}"))
            {
                var data = _cacheManager.Get<List<ZoneModels>>($"getAllZones_{userid}_{company_code}_{branch_code}_{filter}");
                response = data;
            }
            else
            {
                var getAllZonesList = this._iDocumentSetup.getAllZones(filter);
                this._cacheManager.Set($"getAllZones_{userid}_{company_code}_{branch_code}_{filter}", getAllZonesList, 20);
                response = getAllZonesList;
            }
            return response;
            //var result = this._iDocumentSetup.getAllZones(filter);
            //return result;
        }
        [HttpGet]
        public List<RegionalModels> getAllRegions(string filter)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<RegionalModels>();
            if (this._cacheManager.IsSet($"getAllRegions_{userid}_{company_code}_{branch_code}_{filter}"))
            {
                var data = _cacheManager.Get<List<RegionalModels>>($"getAllRegions_{userid}_{company_code}_{branch_code}_{filter}");
                response = data;
            }
            else
            {
                var getAllRegionsList = this._iDocumentSetup.getAllRegions(filter);
                this._cacheManager.Set($"getAllRegions_{userid}_{company_code}_{branch_code}_{filter}", getAllRegionsList, 20);
                response = getAllRegionsList;
            }
            return response;
            //var result = this._iDocumentSetup.getAllRegions(filter);
            //return result;
        }
        [HttpGet]
        public List<DistrictModels> getAllDistricts(string filter)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<DistrictModels>();
            if (this._cacheManager.IsSet($"getAllDistricts_{userid} + {company_code}_{branch_code}_{filter}"))
            {
                var data = _cacheManager.Get<List<DistrictModels>>($"getAllDistricts_{userid} + {company_code}_{branch_code}_{filter}");
                response = data;
            }
            else
            {
                var getAllDistrictsList = this._iDocumentSetup.getAllDistricts(filter);
                this._cacheManager.Set($"getAllDistricts_{userid} + {company_code}_{branch_code}_{filter}", getAllDistrictsList, 20);
                response = getAllDistrictsList;
            }
            return response;
            //var result = this._iDocumentSetup.getAllDistricts(filter);
            //return result;
        }
        [HttpGet]
        public List<CityModels> getAllCities(string filter)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<CityModels>();
            if (this._cacheManager.IsSet($"getAllCities_{userid}_{company_code}_{branch_code}_{filter}"))
            {
                var data = _cacheManager.Get<List<CityModels>>($"getAllCities_{userid}_{company_code}_{branch_code}_{filter}");
                response = data;
            }
            else
            {
                var getAllCitiesList = this._iDocumentSetup.getAllCities(filter);
                this._cacheManager.Set($"getAllCities_{userid}_{company_code}_{branch_code}_{filter}", getAllCitiesList, 20);
                response = getAllCitiesList;
            }
            return response;
            //var result = this._iDocumentSetup.getAllCities(filter);
            //return result;
        }
        [HttpGet]
        public List<CountryModels> getAllCountry(string filter)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<CountryModels>();
            if (this._cacheManager.IsSet($"getAllCountry_{userid}_{company_code}_{branch_code}_{filter}"))
            {
                var data = _cacheManager.Get<List<CountryModels>>($"getAllCountry_{userid}_{company_code}_{branch_code}_{filter}");
                response = data;
            }
            else
            {
                var getAllCountryList = this._iDocumentSetup.getAllCountry(filter);
                this._cacheManager.Set($"getAllCountry_{userid}_{company_code}_{branch_code}_{filter}", getAllCountryList, 20);
                response = getAllCountryList;
            }
            return response;
            //var result = this._iDocumentSetup.getAllCountry(filter);
            //return result;
        }
        [HttpGet]
        public List<BranchModels> getAllBranchs(string filter)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<BranchModels>();
            if (this._cacheManager.IsSet($"getAllBranchs_{userid}_{company_code}_{branch_code}_{filter}"))
            {
                var data = _cacheManager.Get<List<BranchModels>>($"getAllBranchs_{userid}_{company_code}_{branch_code}_{filter}");
                response = data;
            }
            else
            {
                var getAllBranchsList = this._iDocumentSetup.getAllBranchs(filter);
                this._cacheManager.Set($"getAllBranchs_{userid}_{company_code}_{branch_code}_{filter}", getAllBranchsList, 20);
                response = getAllBranchsList;
            }
            return response;
            //var result = this._iDocumentSetup.getAllBranchs(filter);
            //return result;
        }


        //price List

        [HttpGet]
        public List<MasterFieldForUpdate> getAllPricelist(string filter)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<MasterFieldForUpdate>();
            if (this._cacheManager.IsSet($"getAllPricelist_{userid}_{company_code}_{branch_code}_{filter}"))
            {
                var data = _cacheManager.Get<List<MasterFieldForUpdate>>($"getAllPricelist_{userid}_{company_code}_{branch_code}_{filter}");
                response = data;
            }
            else
            {
                var getAllPricelist1 = this._iDocumentSetup.getAllPricelist(filter);
                this._cacheManager.Set($"getAllPricelist_{userid}_{company_code}_{branch_code}_{filter}", getAllPricelist1, 20);
                response = getAllPricelist1;
            }
            return response;
            //var result = this._iDocumentSetup.getAllBranchs(filter);
            //return result;
        }
        [HttpGet]
        public int Backdays(string formcode)
        {
            _logErp.InfoInFile("Sales backdays calculator method started========");
            var result = this._iFormSetupRepo.GetBackDaysByFormCode(formcode);
            _logErp.InfoInFile(result + " backdays fetched");
            return result;
        }
        #endregion

        #region WebConfiguration
        [HttpPost]
        public HttpResponseMessage SaveWebPrefrence(WebPrefrence model)
        {
            try
            {

                var result = this._iDocumentSetup.SaveWebPrefrence(model);
                //  if(result.Success)
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result.Success, STATUS_CODE = (int)HttpStatusCode.OK });


                //    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError });


            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        #endregion

        #region Rejectable Item Setup
        [HttpGet]
        public List<RejectableItem> getRejetableItem()
        {

            var result = this._iDocumentSetup.getRejectlbleitems();

            return result;


        }

        [HttpPost]
        public HttpResponseMessage createNewRejectableSetup(RejectableItem model)
        {
            try
            {
                var result = this._iDocumentSetup.createRejectableItemSetup(model);
                if (result == "INSERTED")
                {

                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "INSERTED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else if (result == "INVALIDINSERTED")
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, new
                    {
                        MESSAGE = "INVALIDINSERTED",
                        STATUS_CODE = (int)HttpStatusCode.Conflict
                    });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpPost]
        public HttpResponseMessage updateRejectableSetup(RejectableItem model)
        {
            try
            {
                var result = this._iDocumentSetup.updateRejetableItemSetup(model);
                if (result == "UPDATED")
                {

                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpPost]
        public HttpResponseMessage deleteRejectableItemSetup(string itemId)
        {
            try
            {
                var result = _iDocumentSetup.deleteRejectableSetup(itemId);

                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }

        [HttpGet]
        public KYCFORM GetKfcFormByCustomerCode(string customerCode)
        {
            var result = this._iDocumentSetup.GetKYCFORM(customerCode);
            return result;
        }
        #endregion

        #region ISSUE TYPE SETUP

        [HttpGet]
        public List<IssueType> GetSavedIssueType()
        {
            try
            {
                var typeList = _iDocumentSetup.GetSavedIssueType();
                return typeList;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting issue type : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }


        [HttpPost]
        public HttpResponseMessage SaveIssueType(IssueTypeSetupModel typeModal)
        {
            try
            {
                var message = _iDocumentSetup.SaveIssueType(typeModal);
                if (message == "INSERTED")
                {

                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = message, STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while saving issue type : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage UpdateIssueTypeSetup(IssueTypeSetupModel model)
        {
            try
            {
                var result = this._iDocumentSetup.UpdateIssueTypeSetup(model);
                if (result == "UPDATED")
                {

                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpPost]
        public HttpResponseMessage DeleteIssueTypeSetup(string issueTypeCode)
        {
            try
            {
                var result = _iDocumentSetup.DeleteIssueTypeSetups(issueTypeCode);

                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }

        #endregion

        #region MEASUREMENT UNIT SETUP

        [HttpGet]
        public List<MeasurementUnit> GetAllMeasurementUnit()
        {
            try
            {
                var unitList = _iDocumentSetup.GetAllMeasurementUnit();
                return unitList;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting measurement unit : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }


        [HttpPost]
        public HttpResponseMessage SaveMeasurementUnit(MeasurementUnit unitModel)
        {
            try
            {
                var message = _iDocumentSetup.SaveMeasurementUnit(unitModel);
                if (message == "INSERTED")
                {

                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = message, STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else if (message == "INVALIDINSERTED")
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, new
                    {
                        MESSAGE = "INVALIDINSERTED",
                        STATUS_CODE = (int)HttpStatusCode.Conflict
                    });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while saving measurement unit : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage UpdateMeasurementUnit(MeasurementUnit model)
        {
            try
            {
                var result = this._iDocumentSetup.UpdateMeasurementUnit(model);
                if (result == "UPDATED")
                {

                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpPost]
        public HttpResponseMessage DeleteMeasurementUnit(string unitCode)
        {
            try
            {
                var result = _iDocumentSetup.DeleteMeasurementUnit(unitCode);

                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }

        #endregion

        #region CITY SETUP

        [HttpGet]
        public List<CityModels> GetCities()
        {
            try
            {
                var cityList = _iDocumentSetup.GetCities();
                return cityList;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting city list : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public List<DistrictModels> GetDistricts()
        {
            try
            {
                var districtList = _iDocumentSetup.GetDistricts();
                return districtList;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting city list : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }



        [HttpPost]
        public HttpResponseMessage SaveCitySetup(CityModelForSave cityModal)
        {
            try
            {
                var message = _iDocumentSetup.SaveCitySetup(cityModal);
                if (message == "INSERTED")
                {

                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = message, STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else if (message == "INVALIDINSERTED")
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, new
                    {
                        MESSAGE = "INVALIDINSERTED",
                        STATUS_CODE = (int)HttpStatusCode.Conflict
                    });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while saving city setup : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage UpdateCitySetup(CityModelForSave model)
        {
            try
            {
                var result = this._iDocumentSetup.UpdateCitySetup(model);
                if (result == "UPDATED")
                {

                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpPost]
        public HttpResponseMessage DeleteCitySetup(string cityCode)
        {
            try
            {
                var result = _iDocumentSetup.DeleteCitySetup(cityCode);

                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        #endregion

        #region Currency  Setup
        //Get list
        [HttpGet]
        public List<CurrencySetupModel> getCurrencyList()
        {
            var result = this._iDocumentSetup.getAllCurrencyCode();
            return result;
        }
        //CREATE Currency
        [HttpPost]
        public HttpResponseMessage createNewCurrencySetup(CurrencySetupModel model)
        {
            try
            {
                var result = this._iDocumentSetup.createCurrencySetup(model);
                if (result == "INSERTED")
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "INSERTED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else if (result == "INVALIDINSERTED")
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, new
                    {
                        MESSAGE = "INVALIDINSERTED",
                        STATUS_CODE = (int)HttpStatusCode.Conflict
                    });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        //UPDATE Currency
        [HttpPost]
        public HttpResponseMessage updateNewCurrencySetup(CurrencySetupModel model)
        {
            try
            {
                var result = this._iDocumentSetup.updateCurrencySetup(model);
                if (result == "UPDATED")
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        //DELETE Currency
        [HttpPost]
        public HttpResponseMessage deleteCurrencySetup(string currencyCode)
        {
            try
            {
                var result = _iDocumentSetup.deleteCurrencySetup(currencyCode);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        #endregion

        #region Category Setup
        //Get list
        [HttpGet]
        public List<CategorySetupModel> getCategoryList()
        {
            var result = this._iDocumentSetup.getAllCategoryCode();
            return result;
        }
        [HttpGet]
        public string GetMaxCategoryCode()
        {

            var result = this._iDocumentSetup.GetMaxCategoryCode();

            return result;
        }
        //CREATE Currency
        [HttpPost]
        public HttpResponseMessage createNewCategorySetup(CategorySetupModel model)
        {
            try
            {
                var result = this._iDocumentSetup.createCategorySetup(model);
                if (result == "INSERTED")
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "INSERTED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else if (result == "INVALIDINSERTED")
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, new
                    {
                        MESSAGE = "INVALIDINSERTED",
                        STATUS_CODE = (int)HttpStatusCode.Conflict
                    });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        //UPDATE Currency
        [HttpPost]
        public HttpResponseMessage updateNewCategorySetup(CategorySetupModel model)
        {
            try
            {
                var result = this._iDocumentSetup.updateCategorySetup(model);
                if (result == "UPDATED")
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        //DELETE Currency
        [HttpPost]
        public HttpResponseMessage deleteCategorySetup(string categoryCode)
        {
            try
            {
                var result = _iDocumentSetup.deleteCategorySetup(categoryCode);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        #endregion

        #region CHARGE TYPE SETUP

        [HttpGet]
        public List<ChargeSetupModel> GetCharges()
        {
            try
            {
                var chargeTypeList = _iDocumentSetup.GetCharges();
                return chargeTypeList;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting charge type : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }


        [HttpPost]
        public HttpResponseMessage SaveChargeSetup(ChargeSetupModel chargeModal)
        {
            try
            {
                var message = _iDocumentSetup.SaveChargeType(chargeModal);
                if (message == "INSERTED")
                {

                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = message, STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else if (message == "INVALIDINSERTED")
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, new
                    {
                        MESSAGE = "INVALIDINSERTED",
                        STATUS_CODE = (int)HttpStatusCode.Conflict
                    });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while saving charge type : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage UpdateChargeSetup(ChargeSetupModel model)
        {
            try
            {
                var result = this._iDocumentSetup.UpdateChargeSetup(model);
                if (result == "UPDATED")
                {

                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpPost]
        public HttpResponseMessage DeleteChargeSetup(string chargeCode)
        {
            try
            {
                var result = _iDocumentSetup.DeleteChargeSetup(chargeCode);

                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }

        #endregion


        #region TDS Type  Setup   

        [HttpGet]
        public HttpResponseMessage getMaxTdsCode()
        {
            try
            {
                var result = this._iDocumentSetup.getMaxTdsCode();
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }


        [HttpGet]
        public List<TDSTypeModel> getTDSList()
        {

            var result = this._iDocumentSetup.getAllTDS();

            return result;


        }

        [HttpPost]
        public HttpResponseMessage createTDSSetup(TDSTypeModel model)
        {
            try
            {
                var result = this._iDocumentSetup.createTDSSetup(model);
                if (result == "INSERTED")
                {

                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "INSERTED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else if (result == "INVALIDINSERTED")
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, new
                    {
                        MESSAGE = "INVALIDINSERTED",
                        STATUS_CODE = (int)HttpStatusCode.Conflict
                    });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpPost]
        public HttpResponseMessage updateTdsSetup(TDSTypeModel model)
        {
            try
            {
                var result = this._iDocumentSetup.updatetdsSetup(model);
                if (result == "UPDATED")
                {

                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }


        [HttpPost]
        public HttpResponseMessage deleteTDsSetup(string tdsCode)
        {
            try
            {
                var result = _iDocumentSetup.deleteTDsSetup(tdsCode);

                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }


        #endregion

        #region Priority  Setup
        [HttpGet]
        public string GetMaxPriorityCode()
        {

            var result = this._iDocumentSetup.GetMaxPriorityCode();

            return result;
        }

        [HttpGet]
        public List<PrioritySeupModel> getPriorityList()
        {

            var result = this._iDocumentSetup.getAllPriority();

            return result;


        }

        [HttpPost]
        public HttpResponseMessage createPrioritySetup(PrioritySeupModel model)
        {
            try
            {
                var result = this._iDocumentSetup.createPriority(model);
                if (result == "INSERTED")
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "INSERTED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else if (result == "INVALIDINSERTED")
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, new
                    {
                        MESSAGE = "INVALIDINSERTED",
                        STATUS_CODE = (int)HttpStatusCode.Conflict
                    });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpPost]
        public HttpResponseMessage updateNewPrioritySetup(PrioritySeupModel model)
        {
            try
            {
                var result = this._iDocumentSetup.updatePrioritySetup(model);
                if (result == "UPDATED")
                {

                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpPost]
        public HttpResponseMessage deletePrioritySetup(string priorityCode)
        {
            try
            {
                var result = _iDocumentSetup.deletePrioritySetups(priorityCode);

                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }

        #endregion

        #region Scheme setup
        [HttpGet]
        public HttpResponseMessage getMaxSchemeCode()
        {
            try
            {
                var result = this._iDocumentSetup.getMaxSchemeCode();
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        [HttpPost]
        public HttpResponseMessage createNewSchemeSetup(SchemeModels model)
        {

            try
            {
                var result = this._iDocumentSetup.createNewSchemeSetup(model);
                if (result == "INSERTED")
                {
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("getAllSchemes");
                    keystart.Add("getSchemeCodeWithChild");
                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "INSERTED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        [HttpPost]
        public HttpResponseMessage updateSchemeSetup(SchemeModels model)
        {
            try
            {
                var result = this._iDocumentSetup.updateSchemeSetup(model);
                if (result == "UPDATED")
                {
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("getAllSchemes");
                    keystart.Add("getSchemeCodeWithChild");
                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpPost]
        public HttpResponseMessage deleteschemeSetup(string schemeCode)
        {
            try
            {
                var result = _iDocumentSetup.deleteSchemeSetup(schemeCode);
                #region CLEAR CACHE
                List<string> keystart = new List<string>();
                keystart.Add("getAllSchemes");
                keystart.Add("getSchemeCodeWithChild");
                List<string> Record = new List<string>();
                Record = this._cacheManager.GetAllKeys();
                this._cacheManager.RemoveCacheByKey(keystart, Record);
                #endregion
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        [HttpPost]
        public HttpResponseMessage implementschemeSetup(string schemeCode)
        {
            try
            {
                var result = _iDocumentSetup.ImplementScheme(schemeCode);
                #region CLEAR CACHE
                List<string> keystart = new List<string>();
                keystart.Add("getAllSchemes");
                keystart.Add("getSchemeCodeWithChild");
                List<string> Record = new List<string>();
                Record = this._cacheManager.GetAllKeys();
                this._cacheManager.RemoveCacheByKey(keystart, Record);
                #endregion
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }

        [HttpGet]
        public List<SchemeModels> GetAllSchemeCode()
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<SchemeModels>();
            if (this._cacheManager.IsSet($"GetAllSchemeCode_{userid}_{company_code}_{branch_code}"))
            {
                var data = _cacheManager.Get<List<SchemeModels>>($"GetAllSchemeCode_{userid}_{company_code}_{branch_code}");
                response = data;
            }
            else
            {
                var getAllSchemeCodeList = this._iDocumentSetup.getAllSchemeCodeDetail();
                foreach (var data in getAllSchemeCodeList)
                {
                    StringWriter myWriter = new StringWriter();

                    // Decode the encoded string.
                    HttpUtility.HtmlDecode(data.QUERY_STRING, myWriter);

                    string myDecodedString = myWriter.ToString();
                    data.QUERY_STRING = myDecodedString;
                }
                this._cacheManager.Set($"GetAllSchemeCode_{userid}_{company_code}_{branch_code}", getAllSchemeCodeList, 20);
                response = getAllSchemeCodeList;
            }
            return response;

        }
        [HttpGet]
        public List<SchemeModels> getAllSchemes(string filter)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<SchemeModels>();
            if (this._cacheManager.IsSet($"getAllSchemes_{userid}_{company_code}_{branch_code}_{filter}"))
            {
                var data = _cacheManager.Get<List<SchemeModels>>($"getAllSchemes_{userid}_{company_code}_{branch_code}_{filter}");
                response = data;
            }
            else
            {
                var getAllSchemesList = this._iDocumentSetup.getAllScheme(filter);
                this._cacheManager.Set($"getAllSchemes_{userid}_{company_code}_{branch_code}_{filter}", getAllSchemesList, 20);
                response = getAllSchemesList;
            }
            return response;

        }
        [HttpPost]
        public HttpResponseMessage ImpactVoucherBySchemeAll(ImpactVoucherModel model)
        {
            var schemeImplementVal = JsonConvert.DeserializeObject<List<SchemeImplementModel>>(model.SCHEME_IMPLEMENT_VALUE);
            List<FinancialSubLedger> fa = null;
            if (model.SUB_LEDGER_VALUE != null) fa = JsonConvert.DeserializeObject<List<FinancialSubLedger>>(model.SUB_LEDGER_VALUE);
            var result = "SUCCESS";
            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
        }

        [HttpPost]
        public HttpResponseMessage ImpactVoucherByScheme(List<SchemeImplementModel> models)
        {

            try
            {
                string FormCodeQuery = $@"SELECT FORM_CODE,CHARGE_ACCOUNT_CODE,CHARGE_CODE,TO_CHAR(CHARGE_RATE) AS CHARGE_RATE FROM SCHEME_SETUP WHERE SCHEME_CODE='{models[0].SCHEME_CODE}'";
                var FormCodeData = this._dbContext.SqlQuery<SchemeImplementModel>(FormCodeQuery).FirstOrDefault();
                String[] form_codes = new String[100];
                if (FormCodeData.FORM_CODE != "" && FormCodeData.FORM_CODE != null)
                {
                    if (FormCodeData.FORM_CODE.Contains(','))
                    {
                        form_codes = FormCodeData.FORM_CODE.Split(',');
                    }
                    else
                    {
                        form_codes[0] = FormCodeData.FORM_CODE;
                    }
                    List<string> y = form_codes.ToList<string>();
                    y.RemoveAll(p => string.IsNullOrEmpty(p));
                    form_codes = y.ToArray();
                }
                var result = string.Empty;
                var customerCode = string.Empty;
                var ParTypeCode = string.Empty;

                foreach (var form_code in form_codes)
                {
                    //foreach (var model in models)
                    //{
                    //    if (model.CUSTOMER_CODE != null)
                    //    {
                    //        result = _iDocumentSetup.ImpactSchemeOnVoucherCustomer(model, form_code, FormCodeData.CHARGE_ACCOUNT_CODE, FormCodeData.CHARGE_CODE, FormCodeData.CHARGE_RATE);
                    //        if (result == "NOTMAPPED")
                    //        {
                    //            string customer_name = _iDocumentSetup.GetcustomerNameByCode(model.CUSTOMER_CODE);
                    //            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, CUSTOMER_CODE= model.CUSTOMER_CODE, CUSTOMER_NAME= customer_name, PARTY_TYPE_CODE = "", PARTY_TYPE_NAME = "", STATUS_CODE = (int)HttpStatusCode.OK });
                    //        }

                    //    }
                    //    if (model.PARTY_TYPE_CODE != null)
                    //    {
                    //        result = _iDocumentSetup.ImpactSchemeOnVoucherPartyType(model, form_code, FormCodeData.CHARGE_ACCOUNT_CODE, FormCodeData.CHARGE_CODE, FormCodeData.CHARGE_RATE);
                    //        if(result== "NOTMAPPED")
                    //        {
                    //            string party_type_name = _iDocumentSetup.GetParytTypeNameByCode(model.PARTY_TYPE_CODE);
                    //            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, CUSTOMER_CODE = "", CUSTOMER_NAME = "", PARTY_TYPE_CODE =model.PARTY_TYPE_CODE, PARTY_TYPE_NAME= party_type_name, STATUS_CODE = (int)HttpStatusCode.OK });
                    //        }
                    //    }

                    //}
                    result = _iDocumentSetup.ImpactSchemeOnVoucher(models, form_code, FormCodeData.CHARGE_ACCOUNT_CODE, FormCodeData.CHARGE_CODE, FormCodeData.CHARGE_RATE);

                }



                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }

        #endregion


        #region calculateInterest
        [HttpGet]
        public List<InterestCalculationResultModel> bindCalculateInterestDetails(int RATE, string CUSTOMER_CODE, string GROUP_CODES, DateTime UPTO_DATE, string COMPANY_CODE, string BRANCH_CODE)
        //public List<InterestCalculationResultModel> bindCalculateInterestDetails()
        {
            var BsDateQuery = $@"select to_bs(sysdate) from dual";
            var BsDate = this._dbContext.SqlQuery<string>(BsDateQuery).FirstOrDefault();
            var todaytime = DateTime.Now.ToShortTimeString();
            CompanyBranchInfo branchinfo = new CompanyBranchInfo();
            var branchinfoQuery = $@"select cs.tpin_vat_no as C_TPIN_VAT_NO,bs.address as B_ADDRESS,bs.telephone_no as B_TELEPHONE_NO,bs.email as B_EMAIL from company_setup cs,fa_branch_setup bs where cs.company_code=bs.company_code and bs.branch_code='{BRANCH_CODE}'";

            branchinfo = this._dbContext.SqlQuery<CompanyBranchInfo>(branchinfoQuery).FirstOrDefault();

            InterestCalculationModel model = new InterestCalculationModel();
            model.RATE = Convert.ToDecimal(RATE);
            model.CUSTOMER_CODE = CUSTOMER_CODE;
            model.UPTO_DATE = Convert.ToDateTime(UPTO_DATE);
            model.COMPANY_CODE = COMPANY_CODE;
            model.BRANCH_CODE = BRANCH_CODE;
            model.GROUP_CODES = GROUP_CODES;


            var response = new List<InterestCalculationResultModel>();
            var InterestGridList = this._iDocumentSetup.CalculateInterestByPara(model);

            //var customerDetail = this._FormTemplateRepo.GetCustomerDetail(CUSTOMER_CODE).FirstOrDefault();
            foreach (var InterestGrid in InterestGridList)
            {
                var customerDetail = this._FormTemplateRepo.GetCustomerDetail(InterestGrid.CUSTOMER_CODE).FirstOrDefault();
                InterestGrid.REGD_OFFICE_EADDRESS = customerDetail.REGD_OFFICE_EADDRESS;
                InterestGrid.TEL_MOBILE_NO1 = customerDetail.TEL_MOBILE_NO1;
                InterestGrid.TPIN_VAT_NO = customerDetail.TPIN_VAT_NO;
                InterestGrid.TOTL_INT_PARENT = bindCustomerInterestDetails(RATE, InterestGrid.CUSTOMER_CODE, model.UPTO_DATE, model.COMPANY_CODE, model.BRANCH_CODE).Select(x => x.INTEREST).Sum();
                //InterestGrid.TOTAL_INTEREST = InterestGridList.Select(x => x.INTEREST).Sum();
                //InterestGrid.TOTAL_OUTSTANDING_BEF= InterestGridList.Select(x => x.BALANCE).Sum();
                //InterestGrid.TOTAL_OUTSTANDING_AF = InterestGrid.TOTAL_OUTSTANDING_BEF + InterestGrid.TOTAL_INTEREST;
                InterestGrid.TODAY_DATE = BsDate;
                InterestGrid.EMAIL = customerDetail.EMAIL;
                InterestGrid.TODAY_TIME = todaytime;
            }
            if (branchinfo != null)
            {
                foreach (var IG in InterestGridList)
                {
                    IG.C_TPIN_VAT_NO = branchinfo.C_TPIN_VAT_NO;
                    IG.B_TELEPHONE_NO = branchinfo.B_TELEPHONE_NO;
                    IG.B_ADDRESS = branchinfo.B_ADDRESS;
                    IG.B_EMAIL = branchinfo.B_EMAIL;
                }
            }

            response = InterestGridList;
            return response;
        }

        [HttpGet]
        public List<InterestCalcResDetailModel> bindCustomerInterestDetails(int RATE, string CUSTOMER_CODE, DateTime UPTO_DATE, string COMPANY_CODE, string BRANCH_CODE)
        //public List<InterestCalculationResultModel> bindCalculateInterestDetails()
        {
            var customerDetail = this._FormTemplateRepo.GetCustomerDetail(CUSTOMER_CODE).FirstOrDefault();
            InterestCalculationModel model = new InterestCalculationModel();
            model.RATE = Convert.ToDecimal(RATE);
            model.CUSTOMER_CODE = CUSTOMER_CODE;
            model.UPTO_DATE = Convert.ToDateTime(UPTO_DATE);
            model.COMPANY_CODE = COMPANY_CODE;
            model.BRANCH_CODE = BRANCH_CODE;

            var response = new List<InterestCalcResDetailModel>();
            var InterestGridList = this._iDocumentSetup.CalculateInterestDetailsByPara(model);
            foreach (var InterestGrid in InterestGridList)
            {
                InterestGrid.TOTAL_INTEREST = InterestGridList.Select(x => x.INTEREST).Sum();
                InterestGrid.TOTAL_OUTSTANDING_BEF = InterestGridList.Select(x => x.BALANCE).Sum();
                InterestGrid.TOTAL_OUTSTANDING_AF = InterestGrid.TOTAL_OUTSTANDING_BEF + InterestGrid.TOTAL_INTEREST;
                InterestGrid.REGD_OFFICE_EADDRESS = customerDetail.REGD_OFFICE_EADDRESS;
                InterestGrid.TEL_MOBILE_NO1 = customerDetail.TEL_MOBILE_NO1;
                InterestGrid.TPIN_VAT_NO = customerDetail.TPIN_VAT_NO;
            }
            response = InterestGridList;

            return response;
        }
        [HttpPost]
        public HttpResponseMessage ImpactInterestCalculation(InterestCalcPostModel model)
        {
            var InterestCalculatedVal = JsonConvert.DeserializeObject<List<InterestCalculationResultModel>>(model.INTERESET_DATA);
            var InterestCalcParaVal = JsonConvert.DeserializeObject<InterestCalculcImpacttModel>(model.INTEREST_PARAM_DATA);
            var result = _iDocumentSetup.CreateInterestImpact(InterestCalculatedVal, InterestCalcParaVal);
            if (result == "INSERTED")
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "INSERTED", STATUS_CODE = (int)HttpStatusCode.OK });
            }
            else if (result == "CustomerExist")
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "CustomerExist", STATUS_CODE = (int)HttpStatusCode.OK });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "FAIL", STATUS_CODE = (int)HttpStatusCode.OK });
            }
        }

        [HttpGet]
        public List<InterestCalcLogModel> bindInterestCalcLogDetails()
        {
            var result = _iDocumentSetup.GetInterestCalcLog();
            return result;
        }

        #endregion

        #region Gate Entry
        [HttpPost]
        public HttpResponseMessage GetGateEntryList(GateEntryReqModel model)
        {
            try
            {
                var result = this._iDocumentSetup.GetGateEntryList(model);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }

        [HttpPost]
        public HttpResponseMessage SaveGateEntry(List<GateEntryModel> model)
        {
            try
            {
                var result = this._iDocumentSetup.SaveGateEntry(model);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetGateEntryById(string gateNo)
        {
            try
            {
                var result = this._iDocumentSetup.GetGateEntryById(gateNo);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpDelete]
        public HttpResponseMessage DeleteGateEntry(string gateNo)
        {
            try
            {
                var result = this._iDocumentSetup.DeleteGateEntry(gateNo);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetVehicles(string filter = "")
        {
            try
            {
                var result = this._iDocumentSetup.GetVehicles(filter);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetDriverByVehicleEdesc(string vehicleEdesc)
        {
            try
            {
                var result = this._iDocumentSetup.GetDriverNameByVehicleEdesc(vehicleEdesc);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetTransporters(string filter = "")
        {
            try
            {
                var result = this._iDocumentSetup.GetTransporters(filter);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetVehicleTypes(string filter = "")
        {
            try
            {
                var result = this._iDocumentSetup.GetVehicleTypes(filter);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetItemDiscountsByDate(string itemCodes, string csFlag = "C", string effectiveDate = null)
        {
            try
            {
                if (string.IsNullOrEmpty(itemCodes))
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = new List<object>() });
                }

                DateTime? parsedDate = null;
                if (!string.IsNullOrWhiteSpace(effectiveDate))
                {
                    DateTime dt;
                    if (DateTime.TryParse(effectiveDate, out dt)) parsedDate = dt;
                }

                var codes = itemCodes.Split(',').Select(c => c.Trim()).Where(c => !string.IsNullOrEmpty(c)).ToList();
                var batchResults = this._iDocumentSetup.GetItemDiscountsByDate(codes, csFlag, parsedDate);
                var results = new List<object>();

                foreach (var code in codes)
                {
                    if (batchResults.ContainsKey(code))
                    {
                        var d = batchResults[code];
                        var resultWithCode = new
                        {
                            ITEM_CODE = code,
                            MU_CODE = d.MU_CODE,
                            DISCOUNT_RATE = d.DISCOUNT_RATE,
                            DISCOUNT_PERCENT = d.DISCOUNT_PERCENT,
                            ITEM_DISCOUNT_RATE = d.ITEM_DISCOUNT_RATE,
                            ITEM_DISCOUNT_PERCENT = d.ITEM_DISCOUNT_PERCENT
                        };
                        results.Add(resultWithCode);
                    }
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = results });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetReferences(string filter = "", string gateDate = "")
        {
            try
            {
                DateTime? parsed = null;
                DateTime temp;
                if (DateTime.TryParse(gateDate, out temp)) parsed = temp;
                var result = this._iDocumentSetup.GetGateReferences(filter, parsed);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetLocations()
        {
            try
            {
                var result = this._iDocumentSetup.GetLocations();
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetPartyNameByReference(string referenceNo, string formCode)
        {
            try
            {
                var result = this._iDocumentSetup.GetPartyNameByReference(referenceNo, formCode);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetReferenceItems(string referenceNo, string formCode, string gateDate = "")
        {
            try
            {
                DateTime? parsed = null;
                DateTime temp;
                if (DateTime.TryParse(gateDate, out temp)) parsed = temp;
                var result = this._iDocumentSetup.GetReferenceItems(referenceNo, formCode, parsed);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        [HttpPost]
        public HttpResponseMessage GetPartyNameByReferences([FromBody] List<ReferencePairDto> references)
        {
            try
            {
                if (references == null || references.Count == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { MESSAGE = "Reference list cannot be empty.", STATUS_CODE = (int)HttpStatusCode.BadRequest });
                }

                var result = this._iDocumentSetup.GetPartyNameByReferences(references);

                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpPost]
        public HttpResponseMessage GetMultiReferenceItems(MultiReferenceRequest request)
        {
            try
            {
                if (request == null || request.References == null || request.References.Count == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { MESSAGE = "Reference list cannot be empty.", STATUS_CODE = (int)HttpStatusCode.BadRequest });
                }

                var result = this._iDocumentSetup.GetMultiReferenceItems(request);

                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetMaxGateEntryNo()
        {
            try
            {
                var result = this._iDocumentSetup.GetMaxGateEntryNo();
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetSelectedReference(string orderNo)
        {
            try
            {
                var result = this._iDocumentSetup.GetSelectedReference(orderNo);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        #endregion

        #region Post Date Cheque
        [HttpPost]
        public HttpResponseMessage GetPDCList(PDCReqModel model)
        {
            try
            {
                var result = this._iDocumentSetup.GetPDCList(model);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpPost]
        public HttpResponseMessage GetPaymentPDCList(PDCReqModel model)
        {
            try
            {
                var result = this._iDocumentSetup.GetPaymentPDCList(model);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpPost]
        public HttpResponseMessage SavePDC(PDCModel model)
        {
            try
            {
                var result = this._iDocumentSetup.SavePDC(model);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }




        [HttpPost]
        public HttpResponseMessage CheckDuplicateChequeNo([FromBody] string chequeNo)
        {
            try
            {
                bool exists = _iDocumentSetup.IsChequeNoDuplicate(chequeNo);

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    MESSAGE = exists ? "Duplicate Cheque Number" : "Cheque Number is unique",
                    STATUS_CODE = (int)HttpStatusCode.OK,
                    IS_DUPLICATE = exists
                });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    MESSAGE = ex.Message,
                    STATUS_CODE = (int)HttpStatusCode.InternalServerError
                });
            }
        }



        [HttpPost]
        public HttpResponseMessage SaveNewPDC()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var form = httpRequest.Form;


                HttpPostedFile file = httpRequest.Files["CHEQUE_PHOTO"];
                string savedFileName = "";

                if (file != null)
                {
                    string ext = Path.GetExtension(file.FileName).ToLower();
                    if (!new[] { ".jpg", ".jpeg", ".png" }.Contains(ext))
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest,
                            new { MESSAGE = "Invalid file format.", TYPE = "error" });
                    }

                    savedFileName = Guid.NewGuid() + ext;
                    string folder = HttpContext.Current.Server.MapPath("~/Areas/NeoErp.DocumentTemplate/images/ChequeImages");

                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    file.SaveAs(Path.Combine(folder, savedFileName));
                }


                var model = new PDCPostModel
                {
                    RECEIPT_NO = form["RECEIPT_NO"],
                    RECEIPT_DATE = ParseDate(form["RECEIPT_DATE"]),
                    CHEQUE_TYPE = form["CHEQUE_TYPE"],
                    CHEQUE_DATE = ParseDate(form["CHEQUE_DATE"]),
                    CUSTOMER_CODE = form["CUSTOMER_CODE"],
                    PARTY_TYPE_CODE = form["PARTY_TYPE_CODE"],
                    PDC_AMOUNT = ParseDecimal(form["PDC_AMOUNT"]),
                    PDC_DETAILS = form["PDC_DETAILS"],
                    BANK_NAME = form["BANK_NAME"],
                    CHEQUE_NO = form["CHEQUE_NO"],
                    REMARKS = form["REMARKS"],
                    CHEQUE_PATH = "Areas/NeoErp.DocumentTemplate/images/ChequeImages/" + savedFileName,
                    MR_ISSUED_BY = form["EMPLOYEE_CODE"],
                    PRIOR_DAYS = ParseDecimal(form["PRIOR_DAYS"]),
                    CREATED_BY = form["CREATED_BY"],
                    CREATED_DATE = DateTime.Now,
                    ACC_CODE = form["ACC_CODE"],
                    MR_NO = form["MR_NO"],
                    MANUAL_NO = form["CHEQUE_NO"]
                };

                var result = this._iDocumentSetup.SaveNewPDC(model);

                if (result == "DUPLICATE")
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict,
                        new { MESSAGE = "Cheque Number Already Exists!", STATUS_CODE = (int)HttpStatusCode.Conflict, DATA = result });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }




        [HttpPost]
        public HttpResponseMessage SavePaymentPDC()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var form = httpRequest.Form;


                HttpPostedFile file = httpRequest.Files["CHEQUE_PHOTO"];
                string savedFileName = "";

                if (file != null)
                {
                    string ext = Path.GetExtension(file.FileName).ToLower();
                    if (!new[] { ".jpg", ".jpeg", ".png" }.Contains(ext))
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest,
                            new { MESSAGE = "Invalid file format.", TYPE = "error" });
                    }

                    savedFileName = Guid.NewGuid() + ext;
                    string folder = HttpContext.Current.Server.MapPath("~/Areas/NeoErp.DocumentTemplate/images/ChequeImages");

                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    file.SaveAs(Path.Combine(folder, savedFileName));
                }


                var model = new PDCPaymentModel
                {
                    PAYMENT_NO = form["PAYMENT_NO"],
                    PAYMENT_DATE = ParseDate(form["PAYMENT_DATE"]),
                    CHEQUE_TYPE = form["CHEQUE_TYPE"],
                    CHEQUE_DATE = ParseDate(form["CHEQUE_DATE"]),
                    SUPPLIER_CODE = form["SUPPLIER_CODE"],
                    PARTY_TYPE_CODE = form["PARTY_TYPE_CODE"],
                    PDC_AMOUNT = ParseDecimal(form["PDC_AMOUNT"]),
                    PDC_DETAILS = form["PDC_DETAILS"],
                    BANK_NAME = form["BANK_NAME"],
                    BANK_ACC_CODE = form["BANK_ACC_CODE"],
                    CHEQUE_NO = form["CHEQUE_NO"],
                    REMARKS = form["REMARKS"],
                    CHEQUE_PATH = "Areas/NeoErp.DocumentTemplate/images/ChequeImages/" + savedFileName,
                    PRIOR_DAYS = Convert.ToInt32(form["PRIOR_DAYS"]),
                    ACC_CODE = form["ACC_CODE"]
                };
                var result = this._iDocumentSetup.SavePaymentPDC(model);

                if (result == "DUPLICATE")
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict,
                        new { MESSAGE = "Cheque Number Already Exists!", STATUS_CODE = (int)HttpStatusCode.Conflict, DATA = result });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }




        private DateTime? ParseDate(string dateStr)
        {
            if (String.IsNullOrWhiteSpace(dateStr))
                return null;

            DateTime dt;
            if (DateTime.TryParse(dateStr, out dt))
                return dt;

            return null;
        }

        private decimal? ParseDecimal(string numStr)
        {
            if (String.IsNullOrWhiteSpace(numStr))
                return null;

            decimal val;
            if (Decimal.TryParse(numStr, out val))
                return val;

            return null;
        }



        [HttpPost]
        public HttpResponseMessage UpdatePdcToIntransit(PdcInTransitDTO transitData)
        {

            try
            {
                var result = this._iDocumentSetup.UpdatePdcToIntransit(transitData);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }




        [HttpGet]
        public List<ChartOfAccountForPDCModel> GetChartOfAccountsForPreferences()
        {
            return this._iDocumentSetup.GetChartOfAccountsForPreferences();
        }

        [HttpGet]
        public List<FormSetupDTO> GetFormSetupForPreferences()
        {
            return this._iDocumentSetup.GetFormSetupForPreferences();
        }





        [HttpPost]
        public HttpResponseMessage UpdateEncashData(PdcEncashDTO encashData)
        {

            try
            {
                var result = this._iDocumentSetup.UpdateEncashData(encashData);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }


        [HttpPost]
        public HttpResponseMessage UpdatePaymentPDCEncashData(PdcPaymentEncashDTO encashData)
        {

            try
            {
                var result = this._iDocumentSetup.UpdatePaymentPDCEncashData(encashData);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpPost]
        public HttpResponseMessage SavePreference(PreferenceDataModel data)
        {

            try
            {
                var result = this._iDocumentSetup.SavePreference(data);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }


        [HttpGet]
        public HttpResponseMessage GetPreference()
        {

            try
            {
                var result = this._iDocumentSetup.GetPreference();
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }


        [HttpPost]
        public HttpResponseMessage UpdateBounceData(PdcBounceDTO bounceData)
        {

            try
            {
                var result = this._iDocumentSetup.UpdateBounceData(bounceData);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpPost]
        public HttpResponseMessage UpdateReturnData(PdcReturnDTO returnData)
        {

            try
            {
                var result = this._iDocumentSetup.UpdateReturnData(returnData);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }


        [HttpPost]
        public HttpResponseMessage UpdatePaymentReturnData(PdcPaymentReturnDTO returnData)
        {

            try
            {
                var result = this._iDocumentSetup.UpdatePaymentReturnData(returnData);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }


        [HttpGet]
        public HttpResponseMessage GetPDCById(string receiptNo)
        {
            try
            {
                var result = this._iDocumentSetup.GetPDCById(receiptNo);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        [HttpGet]
        public HttpResponseMessage GetPDCByIdForEdit(string receiptNo)
        {
            try
            {
                var result = this._iDocumentSetup.GetPDCByIdForEdit(receiptNo);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetPaymentPDCByIdForEdit(string paymentNo)
        {
            try
            {
                var result = this._iDocumentSetup.GetPaymentPDCByIdForEdit(paymentNo);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpDelete]
        public HttpResponseMessage DeletePDC(string receiptNo)
        {
            try
            {
                var result = this._iDocumentSetup.DeletePDC(receiptNo);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpDelete]
        public HttpResponseMessage DeletePaymentPDC(string paymentNo)
        {
            try
            {
                var result = this._iDocumentSetup.DeletePaymentPDC(paymentNo);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }



        [HttpPost]
        public HttpResponseMessage GenerateVoucher(string receiptNo)
        {
            try
            {
                var result = this._iDocumentSetup.GenerateVoucher(receiptNo);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }



        [HttpPost]
        public HttpResponseMessage GeneratePaymentVoucher(string paymentNo)
        {
            try
            {
                var result = this._iDocumentSetup.GeneratePaymentVoucher(paymentNo);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }




        #endregion

        #region Consumption Voucher Generate

        [HttpPost]
        public HttpResponseMessage GetConsumptionVoucherCategoryBaseData(ConsumptionVoucherRequestModel model)
        {
            try
            {
                var result = this._iDocumentSetup.GetConsumptionVoucherCategoryBaseData(model);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        #endregion

        #region Rate Schedule
        [HttpGet]
        public HttpResponseMessage GetDefaultCurrency()
        {
            try
            {
                var result = this._iDocumentSetup.GetDefaultCurrency();
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCurrencyName(string currencyCode = "NRS")
        {
            try
            {
                var result = this._iDocumentSetup.GetCurrencyName(currencyCode);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetAreaList()
        {
            try
            {
                var result = this._iDocumentSetup.GetAreaList();
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        [Route("api/SetupApi/GetCurrencyListForRateSchedule")]
        public HttpResponseMessage GetCurrencyListForRateSchedule()
        {
            try
            {
                var result = this._iDocumentSetup.GetCurrencyList();
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCustomerGroups()
        {
            try
            {
                var result = this._iDocumentSetup.GetCustomerGroups();
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        public class PreCodesRequest
        {
            public List<string> MasterCodes { get; set; }
        }

        [HttpPost]
        public HttpResponseMessage GetIndividualsByPreCustomerCodes([FromBody] PreCodesRequest request)
        {
            try
            {
                if (request == null || request.MasterCodes == null || request.MasterCodes.Count == 0)
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = new List<object>() });

                var all = new List<NeoERP.DocumentTemplate.Service.Models.CustomerByGroupModel>();
                foreach (var code in request.MasterCodes.Distinct())
                {
                    var list = this._iDocumentSetup.GetCustomersByGroup(code) ?? new List<NeoERP.DocumentTemplate.Service.Models.CustomerByGroupModel>();
                    all.AddRange(list);
                }
                var distinct = all.GroupBy(x => x.MASTER_CUSTOMER_CODE).Select(g => g.First()).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = distinct });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetItemDiscountsBatch(string itemCodes, string csFlag = "C")
        {
            try
            {
                if (string.IsNullOrEmpty(itemCodes))
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = new List<object>() });
                }

                var codes = itemCodes.Split(',').Select(c => c.Trim()).Where(c => !string.IsNullOrEmpty(c)).ToList();
                var batchResults = this._iDocumentSetup.GetItemDiscountsBatch(codes, csFlag);
                var results = new List<object>();

                foreach (var code in codes)
                {
                    if (batchResults.ContainsKey(code))
                    {
                        var d = batchResults[code];
                        var resultWithCode = new
                        {
                            ITEM_CODE = code,
                            MU_CODE = d.MU_CODE,
                            DISCOUNT_RATE = d.DISCOUNT_RATE,
                            DISCOUNT_PERCENT = d.DISCOUNT_PERCENT,
                            ITEM_DISCOUNT_RATE = d.ITEM_DISCOUNT_RATE,
                            ITEM_DISCOUNT_PERCENT = d.ITEM_DISCOUNT_PERCENT
                        };
                        results.Add(resultWithCode);
                    }
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = results });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpPost]
        public HttpResponseMessage GetIndividualsByPreItemCodes([FromBody] PreCodesRequest request)
        {
            try
            {
                if (request == null || request.MasterCodes == null || request.MasterCodes.Count == 0)
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = new List<object>() });

                var all = new List<NeoERP.DocumentTemplate.Service.Models.ItemByGroupModel>();
                foreach (var code in request.MasterCodes.Distinct())
                {
                    var list = this._iDocumentSetup.GetItemsByGroup(code) ?? new List<NeoERP.DocumentTemplate.Service.Models.ItemByGroupModel>();
                    all.AddRange(list);
                }
                var distinct = all.GroupBy(x => x.ITEM_CODE).Select(g => g.First()).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = distinct });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetItemGroups()
        {
            try
            {
                var result = this._iDocumentSetup.GetItemGroups();
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetMuDescription(string itemCode)
        {
            try
            {
                var result = this._iDocumentSetup.GetMuDescription(itemCode);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetMuDescriptionBatch(string itemCodes)
        {
            try
            {
                if (string.IsNullOrEmpty(itemCodes))
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = new List<object>() });
                }

                var codes = itemCodes.Split(',').Select(c => c.Trim()).Where(c => !string.IsNullOrEmpty(c)).ToList();
                var batchResults = this._iDocumentSetup.GetMuDescriptionBatch(codes);
                var results = new List<object>();

                foreach (var code in codes)
                {
                    if (batchResults.ContainsKey(code))
                    {
                        foreach (var item in batchResults[code])
                        {
                            var resultWithCode = new
                            {
                                ITEM_CODE = code,
                                MU_CODE = item.MU_CODE,
                                MU_EDESC = item.MU_EDESC
                            };
                            results.Add(resultWithCode);
                        }
                    }
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = results });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetItemRates(string itemCode)
        {
            try
            {
                var result = this._iDocumentSetup.GetItemRates(itemCode);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetItemRatesBatch(string itemCodes)
        {
            try
            {
                if (string.IsNullOrEmpty(itemCodes))
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = new List<object>() });
                }

                var codes = itemCodes.Split(',').Select(c => c.Trim()).Where(c => !string.IsNullOrEmpty(c)).ToList();
                var batchResults = this._iDocumentSetup.GetItemRatesBatch(codes);
                var results = new List<object>();

                foreach (var code in codes)
                {
                    if (batchResults.ContainsKey(code))
                    {
                        var result = batchResults[code];
                        var resultWithCode = new
                        {
                            ITEM_CODE = code,
                            STANDARD_RATE = result.STANDARD_RATE,
                            MRP_RATE = result.MRP_RATE,
                            RETAIL_PRICE = result.RETAIL_PRICE,
                            MU_CODE = result.MU_CODE
                        };
                        results.Add(resultWithCode);
                    }
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = results });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetDocumentList()
        {
            try
            {
                var result = this._iDocumentSetup.GetDocumentList();
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCustomersByGroup(string groupCode)
        {
            try
            {
                var result = this._iDocumentSetup.GetCustomersByGroup(groupCode);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetItemsByGroup(string groupCode)
        {
            try
            {
                var result = this._iDocumentSetup.GetItemsByGroup(groupCode);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpPost]
        public HttpResponseMessage SaveRateSchedule(SaveRateScheduleModel rateScheduleData)
        {
            try
            {
                var result = this._iDocumentSetup.SaveRateSchedule(rateScheduleData);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpPost]
        public HttpResponseMessage SaveDiscountSchedule(SaveDiscountScheduleModel discountScheduleData)
        {
            try
            {
                var result = this._iDocumentSetup.SaveDiscountSchedule(discountScheduleData);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetDealerGroups()
        {
            try
            {
                var result = this._iDocumentSetup.GetDealerGroups();
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpPost]
        public HttpResponseMessage GetIndividualsByMasterCodes([FromBody] IndividualsByMasterCodesRequest request)
        {
            try
            {
                List<object> result = new List<object>();

                if (request.PartyType == "customer")
                {
                    result = this._iDocumentSetup.GetCustomerIndividualsByMasterCodes(request.MasterCodes).Cast<object>().ToList();
                }
                else if (request.PartyType == "dealer")
                {
                    result = this._iDocumentSetup.GetDealerIndividualsByMasterCodes(request.MasterCodes).Cast<object>().ToList();
                }
                else if (request.PartyType == "supplier")
                {
                    result = this._iDocumentSetup.GetSupplierIndividualsByMasterCodes(request.MasterCodes).Cast<object>().ToList();
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }



        [HttpPost]
        public HttpResponseMessage GetHierarchicalTreeByMasterCodes(HierarchicalTreeRequest request)
        {
            try
            {
                object result = null;

                switch (request.PartyType?.ToUpper())
                {
                    case "CUSTOMER":
                        result = _iDocumentSetup.GetCustomerHierarchicalTree(request.MasterCodes, request.IndividualCodes, request.SelectMode);
                        break;
                    case "DEALER":
                        result = _iDocumentSetup.GetDealerHierarchicalTree(request.MasterCodes, request.IndividualCodes, request.SelectMode);
                        break;
                    case "SUPPLIER":
                        result = _iDocumentSetup.GetSupplierHierarchicalTree(request.MasterCodes, request.IndividualCodes, request.SelectMode);
                        break;
                    default:
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new { MESSAGE = "Invalid PartyType", STATUS_CODE = (int)HttpStatusCode.BadRequest });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetDealersByGroup(string groupCode)
        {
            try
            {
                var result = this._iDocumentSetup.GetDealersByGroup(groupCode);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetSupplierGroups()
        {
            try
            {
                var result = this._iDocumentSetup.GetSupplierGroups();
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetSuppliersByGroup(string groupCode)
        {
            try
            {
                var result = this._iDocumentSetup.GetSuppliersByGroup(groupCode);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetSupplierPartyTypeAndAccountByMasterCode(string masterSupplierCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(masterSupplierCode))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { MESSAGE = "Invalid masterSupplierCode", STATUS_CODE = (int)HttpStatusCode.BadRequest });
                }

                var supplier = this._iDocumentSetup.GetSupplierDataBySupplierCode(masterSupplierCode);

                var data = new
                {
                    PARTY_TYPE_CODE = supplier != null ? supplier.PARTY_TYPE_CODE : string.Empty,
                    ACC_CODE = supplier != null ? supplier.ACC_CODE : string.Empty
                };

                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = data });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        #endregion

        #region Bank Guarantee
        [HttpPost]
        public HttpResponseMessage GetBankGuaranteeList(BankGuaranteeReqModel model)
        {
            try
            {
                var result = this._iDocumentSetup.GetBankGuaranteeList(model);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCompanyInfo()
        {
            try
            {
                var result = this._iDocumentSetup.GetCompanyInfo();
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCompanyBranches()
        {
            try
            {
                var result = this._iDocumentSetup.GetCompanyBranches();
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        #endregion

        #region Employee Setup
        [HttpGet]
        public HttpResponseMessage GetEmployees()
        {
            try
            {
                var result = this._iDocumentSetup.GetEmployees();
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpPost]
        public HttpResponseMessage DeleteEmployee(string employeeCode)
        {
            try
            {
                var result = this._iDocumentSetup.DeleteEmployee(employeeCode);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        [HttpPost]
        public HttpResponseMessage SaveEmployee(EmployeeDTO emp)
        {
            try
            {
                var result = this._iDocumentSetup.SaveEmployee(emp);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        #endregion

        [HttpGet]
        public string GetMaxCityCode()
        {

            var result = this._iDocumentSetup.GetMaxCityCode();

            return result;
        }
    }
}