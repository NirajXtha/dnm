using NeoErp.Core;
using NeoErp.Core.Caching;
using NeoErp.Core.Models;
using NeoErp.Core.Models.Log4NetLoggin;
using NeoErp.Core.Services.CommonSetting;
using NeoErp.Data;
using NeoERP.QCQAManagement.Service.Interface;
using NeoERP.QCQAManagement.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace NeoERP.QCQAManagement.Controllers.API
{
    [RoutePrefix("api/OnSiteInspectionAPI")]
    public class OnSiteInspectionAPIController : ApiController
    {
        // GET: OnSiteInspectionAPI
        private const string QCQA = "On Site Inspection";
        private IOnSiteInspectionRepo _onSiteInspectionRepo;
        private IDbContext _dbContext;
        private IWorkContext _workContext;
        private ICacheManager _cacheManager;
        private NeoErpCoreEntity _objectEntity;
        private readonly ILogErp _logErp;
        private ISettingService _settingService;
        private DefaultValueForLog _defaultValueForLog;
        public OnSiteInspectionAPIController(IOnSiteInspectionRepo _IOnSiteInspectionRepo, IDbContext dbContext, IWorkContext workContext, ICacheManager cacheManager, NeoErpCoreEntity objectEntity)
        {
            this._onSiteInspectionRepo = _IOnSiteInspectionRepo;
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            this._logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule, _defaultValueForLog.FormCode);
        }
        [HttpGet]
        public List<FormDetailSetup> GetOnSiteInspectionList()
        {
            List<FormDetailSetup> tableLists = _onSiteInspectionRepo.GetOnSiteInspectionList();
            return tableLists;
        }
        [HttpGet]
        public List<BatchDetails> GetBatchNoByItemCode(string itemCode)
        {
            var response = this._onSiteInspectionRepo.GetBatchNoByItemCode(itemCode);
            return response;
        }
        [HttpGet]
        public List<ParameterDetails> GetParameterDetailsByPlant(string Plant)
        {
            var response = this._onSiteInspectionRepo.GetParameterDetailsByPlant(Plant);
            return response;
        }
        public IHttpActionResult saveOnSiteInspection(OnSiteInspection onSiteInspection)
        {
            try
            {
                bool isPosted = _onSiteInspectionRepo.InsertOnSiteInspectionData(onSiteInspection);
                if (isPosted)
                {
                    return Ok(new { success = true, message = "Data saved successfully." });
                }
                else
                {
                    return Ok(new { success = true, message = "Failed to save." });
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        public OnSiteInspection GetEditOnSiteInspection(string transactionno)
        {
            OnSiteInspection qc = _onSiteInspectionRepo.GetEditOnSiteInspection(transactionno);
            return qc;
        }
        [HttpGet]
        public OnSiteInspection GetOnSiteInspectionReport(string transactionno)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var SalesOrderDetailFormDetailByFormCodeAndOrderNo = this._onSiteInspectionRepo.GetOnSiteInspectionReport(transactionno);
            return SalesOrderDetailFormDetailByFormCodeAndOrderNo;
        }
    }
}