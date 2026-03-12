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
    public class DailyWastageAPIController : ApiController
    {
        private const string QCQA = "Daily wastage Report";
        private IDailyWastageRepo _dailyWastageRepo;
        private IDbContext _dbContext;
        private IWorkContext _workContext;
        private ICacheManager _cacheManager;
        private NeoErpCoreEntity _objectEntity;
        private readonly ILogErp _logErp;
        private ISettingService _settingService;
        private DefaultValueForLog _defaultValueForLog;
        public DailyWastageAPIController(IDailyWastageRepo _IDailyWastageRepo, IDbContext dbContext, IWorkContext workContext, ICacheManager cacheManager, NeoErpCoreEntity objectEntity)
        {
            this._dailyWastageRepo = _IDailyWastageRepo;
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            this._logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule, _defaultValueForLog.FormCode);
        }
        //[HttpGet]
        //public DailyWastage GetDailyWastageList()
        //{

        //    //ViewBag.QCNO = _materialRepo.GetQCQAVoucherNo("RawMaterial");
        //    DailyWastage tableLists = _dailyWastageRepo.GetDailyWastageList();
        //    return tableLists;
        //}
        [HttpGet]
        public List<FormDetailSetup> GetDailyWastageList()
        {
            List<FormDetailSetup> tableLists = _dailyWastageRepo.GetDailyWastageList();
            return tableLists;
        }
        public List<Items> GetMaterialGroupLists()
        {
            List<Items> tableLists = _dailyWastageRepo.GetMaterialGroupLists();
            return tableLists;
        }
        [HttpPost]
        public IHttpActionResult saveDailyRawMaterial(DailyWastage rawMateriall)
        {
            try
            {
                bool isPosted = _dailyWastageRepo.InsertDailyWastage(rawMateriall);
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
        public DailyWastage GetEditDailyWastage(string transactionno)
        {
            DailyWastage qc = _dailyWastageRepo.GetEditDailyWastage(transactionno);
            return qc;
        }
        public DailyWastage GetDailyWastageReport(string transactionno)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var SalesOrderDetailFormDetailByFormCodeAndOrderNo = this._dailyWastageRepo.GetDailyWastageReport(transactionno);
            return SalesOrderDetailFormDetailByFormCodeAndOrderNo;
        }
    }
}