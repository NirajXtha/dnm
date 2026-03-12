using NeoERP.QuotationManagement.Service.Models;
using NeoERP.QuotationManagement.Service.Interface;
using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using NeoErp.Core;
using NeoErp.Core.Caching;
using System.Data.SqlClient;
using NeoErp.Core.Models;
using NeoErp.Core.Models.Log4NetLoggin;
using System.Web.Http;
using NeoErp.Data;
using NeoErp.Core.Services.CommonSetting;
using System.Web;
using System.IO;

namespace NeoERP.QuotationManagement.Controllers.Api
{
    public class QuotationApiController : ApiController
    {
        private const string QM = "Quotation Management";
        private IQuotationRepo _quotRepo;
        private IDbContext _dbContext;
        private IWorkContext _workContext;
        private ICacheManager _cacheManager;
        private NeoErpCoreEntity _objectEntity;
        private readonly ILogErp _logErp;
        private ISettingService _settingService;
        private DefaultValueForLog _defaultValueForLog;
        public QuotationApiController(IQuotationRepo _IQuotRepo, IDbContext dbContext, IWorkContext workContext, ICacheManager cacheManager, NeoErpCoreEntity objectEntity)
        {
            this._quotRepo = _IQuotRepo;
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            this._logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule, _defaultValueForLog.FormCode);
        }
        [HttpGet]
        public List<Company> GetCompany()
        {
            List<Company> itemDeatils = _quotRepo.GetCompany();
            return itemDeatils;
        }
        [HttpGet]
        public string getUserType()
        {
            string user_type = _quotRepo.getUserType();
            return user_type;
        }
        [HttpGet]
        public List<Products> ItemDetails()
        {
            List<Products> itemDeatils = _quotRepo.GetAllProducts();
            return itemDeatils;
        }
        [HttpGet]
        public List<QuotationLogModel> GetQuotationLogs()
        {
            return _quotRepo.GetQuotationLogs();
        }
        [HttpPost]
        public HttpResponseMessage CloneQuotation(Quotation_setup model)
        {
            try
            {
                bool cloned = _quotRepo.CloneQuotation(model);
                if (cloned)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = $"Quotation {model.TENDER_NO} Cloned successfully!!", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = $"Failed to clone {model.TENDER_NO}!!", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        public List<Quotation_setup> getTenderNo(string tender)
        {
            List<Quotation_setup> quotations = _quotRepo.GetQuotationId(tender);
            return quotations;
        }
        public List<Quotation_setup> getTenderId(string tenderNo)
        {
            List<Quotation_setup> quotation = _quotRepo.GetTenderId(tenderNo);
            return quotation;
        }
        [HttpPost]
        public HttpResponseMessage deleteQuotationId(ApprovalRequest request)
        {
            try
            {
                bool isDeleted = _quotRepo.deleteQuotationId(request);
                if (isDeleted)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Quotation deleted successfully!!", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    // Handle case where project was not posted successfully
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Failed to delete project!!", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occurred during the operation
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        [HttpPost]
        public IHttpActionResult SaveItemData(Quotation_setup formData)
        {
            try
            {
                bool isPosted = _quotRepo.InsertQuotationData(formData);
                if (isPosted)
                {
                    if (formData.References != null)
                    {
                        bool isReferenced = _quotRepo.AddReference(formData.References, formData.FORM_CODE, formData.TENDER_NO);
                        if (!isReferenced)
                        {
                            return Ok(new { success = false, message = "Quotation data saved successfully." });
                        }
                    }
                    return Ok(new { success = true, message = "Quotation data saved successfully." });
                }
                else
                {
                    return Ok(new { success = false, message = "Failed to save Quotation data." });
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        public List<Quotation_setup> ListAllTenders()
        {
            List<Quotation_setup> response = new List<Quotation_setup>();
            try
            {
                response = _quotRepo.ListAllTenders();
                return response;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        public List<Quotation_setup> ListAllPendingTenders()
        {
            List<Quotation_setup> response = new List<Quotation_setup>();
            try
            {
                response = _quotRepo.ListAllPendingTenders();
                return response;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        public List<Quotation_setup> GetQuotationById(string tenderNo)
        {
            List<Quotation_setup> response = new List<Quotation_setup>();
            try
            {
                response = _quotRepo.GetQuotationById(tenderNo);
                return response;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        [HttpPost]
        public HttpResponseMessage updateItemsById(string tenderNo, string id, string q_no)
        {
            try
            {
                bool isDeleted = _quotRepo.updateItemsById(tenderNo, id, q_no);
                if (isDeleted)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Product deleted successfully!!", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    // Handle case where project was not posted successfully
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Failed to delete product!!", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occurred during the operation
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        public List<Quotation_Details> ListQuotationDetails()
        {
            List<Quotation_Details> response = new List<Quotation_Details>();
            try
            {
                response = _quotRepo.ListQuotationDetails();
                return response;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        [HttpGet]
        public List<CurrencyModel> getCurrency()
        {
            List<CurrencyModel> data = _quotRepo.getCurrency();
            return data;
        }
        [HttpGet]
        public List<Quotation_Details> QuotationDetailsById(string quotationNo, string tenderNo)
        {
            List<Quotation_Details> response = new List<Quotation_Details>();
            try
            {
                response = _quotRepo.QuotationDetailsById(quotationNo, tenderNo);
                return response;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        [HttpGet]
        public List<Quotation_Details> QuotationDetailsId(string quotationNo, string tenderNo)
        {
            List<Quotation_Details> response = new List<Quotation_Details>();
            try
            {
                response = _quotRepo.QuotationDetailsId(quotationNo, tenderNo);
                return response;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        public List<SummaryReport> TendersItemWise(ReportFiltersModel model)
        {
            List<SummaryReport> response = new List<SummaryReport>();
            try
            {
                response = _quotRepo.TendersItemWise(model);
                return response;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        [HttpGet]
        public List<Quotation> ItemDetailsByTender(string tenderNo)
        {
            List<Quotation> response = new List<Quotation>();
            try
            {
                response = _quotRepo.ItemDetailsTenderNo(tenderNo);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        [HttpPost]
        public HttpResponseMessage updateQuotationStatus(ApprovalRequest request)
        {
            try
            {
                bool isDeleted = false;
                if (request.status != "R")
                {
                    isDeleted = _quotRepo.acceptQuotation(request.quotationNo, request.status, request.type, request.items, request.itemId, request.REMARKS);
                }
                else if (request.status == "R")
                {
                    isDeleted = _quotRepo.rejectQuotation(request.quotationNo, request.status, request.REMARKS);
                }
                if (isDeleted)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Quotation !!", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    // Handle case where project was not posted successfully
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Quotation !!", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occurred during the operation
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        [HttpPost]
        public IHttpActionResult saveTender(Tender formData)
        {
            try
            {
                bool isPosted = _quotRepo.InsertTenderData(formData);
                if (isPosted)
                {
                    return Ok(new { success = true, message = "Tender data saved successfully." });
                }
                else
                {
                    return Ok(new { success = true, message = "Failed to save Quotation Tender." });
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpGet]
        public List<Tender> TenderDetails()
        {
            List<Tender> response = new List<Tender>();
            try
            {
                response = _quotRepo.getTenderDetails();
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        [HttpGet]
        public List<Tender> TemplateOptions()
        {
            List<Tender> response = new List<Tender>();
            try
            {
                response = _quotRepo.getTemplateOptions();
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        [HttpGet]
        public List<Tender> getSelectQuotationOptions()
        {
            List<Tender> response = new List<Tender>();
            try
            {
                response = _quotRepo.getSelectQuotationOptions();
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        [HttpPost]
        public HttpResponseMessage deleteTenderId(string tenderNo)
        {
            try
            {
                bool isDeleted = _quotRepo.deleteTenderId(tenderNo);
                if (isDeleted)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Tender No. deleted successfully!!", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    // Handle case where project was not posted successfully
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Failed to delete Tender No.!!", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occurred during the operation
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        [HttpGet]
        public List<Tender> getTenderById(string tenderNo)
        {
            List<Tender> response = new List<Tender>();
            try
            {
                response = _quotRepo.getTenderById(tenderNo);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        public List<QuotationCount> GetQuotationCount()
        {
            List<QuotationCount> response = new List<QuotationCount>();
            try
            {
                response = _quotRepo.GetQuotationCount();
                return response;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        public List<Quotation_Details> QuotationById(string quotationNo, string tenderNo)
        {
            List<Quotation_Details> response = new List<Quotation_Details>();
            try
            {
                response = _quotRepo.QuotationDetailsById(quotationNo, tenderNo);
                return response;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        [HttpGet]
        public List<QuotationNotification> notifications()
        {
            return _quotRepo.GetAllNotification();
        }
        [HttpPost]
        public IHttpActionResult QuotationApproval(ApprovalRequest request)
        {
            try
            {
                bool isApproved = _quotRepo.QuotationApproval(request);
                if (isApproved)
                {
                    return Ok(new { success = true, message = "Quotation Approved successfully." });
                }
                else
                {
                    return Ok(new { success = true, message = "Failed to Approve the Quotation." });
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpGet]
        public IHttpActionResult ApprovalProceeding()
        {
            try
            {
                bool employee = _quotRepo.ApprovalProceeding();
                if (employee)
                {
                    return Ok(new { success = true });
                }
                else
                {
                    return Ok(new { success = false });
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        [HttpGet]
        public List<UserAcess> UserAccess(double amount)
        {
            try
            {
                List<UserAcess> employee = _quotRepo.UserAccess(amount);
                return employee;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        [HttpPost]
        public List<Quotation_setup> getTemplateData(Template template)
        {
            List<Quotation_setup> response = new List<Quotation_setup>();
            try
            {
                response = _quotRepo.getTemplateData(template);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in getTemplateData: " + ex.Message, ex);
            }
        }
        [HttpGet]
        public List<Quotation> VoucherList(string code, string row)
        {
            List<Quotation> response = new List<Quotation>();
            try
            {
                response = _quotRepo.getVoucherList(code, row);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        [HttpGet]
        public List<Employee> getUserValue()
        {
            List<Employee> response = new List<Employee>();
            try
            {
                response = _quotRepo.getUserValue();
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        [HttpPost]
        public IHttpActionResult setUserValue(Employee employee)
        {
            try
            {
                bool setValue = _quotRepo.setUserValue(employee);
                if (setValue)
                {
                    return Ok(new { success = true });
                }
                else
                {
                    return Ok(new { success = false });
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        [HttpPost]
        public IHttpActionResult setUserAccess(UserAcess access)
        {
            try
            {
                bool setValue = _quotRepo.setUserAccess(access);
                if (setValue)
                {
                    return Ok(new { success = true });
                }
                else
                {
                    return Ok(new { success = false });
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        [HttpGet]
        public List<PartyDetails> partyDetails(string id)
        {
            List<PartyDetails> response = new List<PartyDetails>();
            try
            {
                response = _quotRepo.partyDetails(id);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        [HttpGet]
        public List<PartyDetailsItems> partyDetailsItems(string id)
        {
            try
            {
                List<PartyDetailsItems> res = _quotRepo.partyDetailsItems(id);
                return res;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public List<TermsAndConditions> termsAndConditions(string id)
        {
            try
            {
                List<TermsAndConditions> res = _quotRepo.termsAndConditions(id);
                return res;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult UserTypeToSetValue()
        {
            try
            {
                bool res = _quotRepo.UserTypeToSetValue();
                if (res)
                {
                    return Ok(new { success = true });
                }
                else
                {
                    return Ok(new { success = false });
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
