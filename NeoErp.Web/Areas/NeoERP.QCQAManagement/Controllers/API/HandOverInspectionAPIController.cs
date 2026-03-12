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
    public class HandOverInspectionAPIController : ApiController
    {
        // GET: PreDispatchInspectionAPI
        private const string QCQA = "Pre Dispatch Inspection Report";
        private IHandOverInspectionRepo _handOverInspectionRepo;
        private IDbContext _dbContext;
        private IWorkContext _workContext;
        private ICacheManager _cacheManager;
        private NeoErpCoreEntity _objectEntity;
        private readonly ILogErp _logErp;
        private ISettingService _settingService;
        private DefaultValueForLog _defaultValueForLog;
        public HandOverInspectionAPIController(IHandOverInspectionRepo _IHandOverInspectionRepo, IDbContext dbContext, IWorkContext workContext, ICacheManager cacheManager, NeoErpCoreEntity objectEntity)
        {
            this._handOverInspectionRepo = _IHandOverInspectionRepo;
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            this._logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule, _defaultValueForLog.FormCode);
        }

        //public PreDispatchInspection GetPreDispatchInspectionList()
        //{

        //    //ViewBag.QCNO = _materialRepo.GetQCQAVoucherNo("RawMaterial");
        //    PreDispatchInspection tableLists = _preDispatchInspectionRepo.GetPreDispatchInspectionList();
        //    return tableLists;
        //}
        [HttpGet]
        public List<FormDetailSetup> GetHandOverInspectionList()
        {

            //ViewBag.QCNO = _materialRepo.GetQCQAVoucherNo("RawMaterial");
            List<FormDetailSetup> tableLists = _handOverInspectionRepo.GetHandOverInspectionList();
            return tableLists;
        }
        [HttpGet]
        public List<PACKINGUNIT> GetPackingUnit()
        {
            List<PACKINGUNIT> tableLists = _handOverInspectionRepo.GetPackingUnit();
            return tableLists;
        }

        [HttpPost]
        public IHttpActionResult saveHandOverInspection(HandOverInspection handoverInspection)
        {
            try
            {
                bool isPosted = _handOverInspectionRepo.InsertHandOverInspectionData(handoverInspection);
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
        public HandOverInspection GetEditHandOverInspection(string transactionno)
        {
            HandOverInspection qc = _handOverInspectionRepo.GetEditHandOverInspection(transactionno);
            return qc;
        }
        [HttpGet]
        public HandOverInspection GetHandOverInspectionReport(string transactionno)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var SalesOrderDetailFormDetailByFormCodeAndOrderNo = this._handOverInspectionRepo.GetHandOverInspectionReport(transactionno);
            return SalesOrderDetailFormDetailByFormCodeAndOrderNo;
        }
    }
}