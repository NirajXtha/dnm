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
    public class SanitationHygieneAPIController : ApiController
    {
        // GET: SanitationHygiene
        private const string QCQA = "Sanitation Hygiene";
        private ISanitationHygieneRepo _sanitationHygieneRepo;
        private IDbContext _dbContext;
        private IWorkContext _workContext;
        private ICacheManager _cacheManager;
        private NeoErpCoreEntity _objectEntity;
        private readonly ILogErp _logErp;
        private ISettingService _settingService;
        private DefaultValueForLog _defaultValueForLog;
        public SanitationHygieneAPIController(ISanitationHygieneRepo _ISanitationHygieneRepo, IDbContext dbContext, IWorkContext workContext, ICacheManager cacheManager, NeoErpCoreEntity objectEntity)
        {
            this._sanitationHygieneRepo = _ISanitationHygieneRepo;
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            this._logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule, _defaultValueForLog.FormCode);
        }
        [HttpGet]
        public List<FormDetailSetup> GetSanitationHygieneList()
        {
            List<FormDetailSetup> tableLists = _sanitationHygieneRepo.GetSanitationHygieneList();
            return tableLists;
        }
        public List<SanitationHygiene> GetMasterSanitationHygiene()
        {
            List<SanitationHygiene> tableLists = _sanitationHygieneRepo.GetMasterSanitationHygiene();
            return tableLists;
        }
        [HttpGet]
        public List<SanitationHygiene> GetAllSanitationHygieneDetails()
        {
            List<SanitationHygiene> tableLists = _sanitationHygieneRepo.GetAllSanitationHygieneDetails();
            return tableLists;
        }

        [HttpGet]
        public List<SanitationHygiene> GetSanitationHygieneDetails(string LocationCode)
        {
            List<SanitationHygiene> tableLists = _sanitationHygieneRepo.GetSanitationHygieneDetails(LocationCode);
            return tableLists;
        }
        public IHttpActionResult saveSanitationHygiene(SanitationHygiene sanitationHygiene)
        {
            try
            {
                bool isPosted = _sanitationHygieneRepo.InsertSanitationHygieneData(sanitationHygiene);
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
        public SanitationHygiene GetEditSanitationHygiene(string transactionno)
        {
            SanitationHygiene qc = _sanitationHygieneRepo.GetEditSanitationHygiene(transactionno);
            return qc;
        }
        [HttpGet]
        public SanitationHygiene GetSanitationHygieneReport(string transactionno)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var SalesOrderDetailFormDetailByFormCodeAndOrderNo = this._sanitationHygieneRepo.GetSanitationHygieneReport(transactionno);
            return SalesOrderDetailFormDetailByFormCodeAndOrderNo;
        }

        #region Report
        [HttpGet]
        public List<ChildModel> GetSanitationHygieneDetailsReport(string frmDate, string toDate)
       {
            List<ChildModel> tableLists = _sanitationHygieneRepo.GetSanitationHygieneDetailsReport(frmDate,toDate);
            return tableLists;
        }
        #endregion

    }
}