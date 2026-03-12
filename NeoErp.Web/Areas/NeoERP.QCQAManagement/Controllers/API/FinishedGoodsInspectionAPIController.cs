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
    public class FinishedGoodsInspectionAPIController : ApiController
    {
        // GET: FinishedGoodsInspectionAPI
        // GET: PreDispatchInspectionAPI
        private const string QCQA = "Finished Goods Inspection";
        private IFinishedGoodsInspectionRepo _finishedGoodsInspectionRepo;
        private IDbContext _dbContext;
        private IWorkContext _workContext;
        private ICacheManager _cacheManager;
        private NeoErpCoreEntity _objectEntity;
        private readonly ILogErp _logErp;
        private ISettingService _settingService;
        private DefaultValueForLog _defaultValueForLog;
        public FinishedGoodsInspectionAPIController(IFinishedGoodsInspectionRepo _IFinishedGoodsInspectionRepo, IDbContext dbContext, IWorkContext workContext, ICacheManager cacheManager, NeoErpCoreEntity objectEntity)
        {
            this._finishedGoodsInspectionRepo = _IFinishedGoodsInspectionRepo;
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            this._logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule, _defaultValueForLog.FormCode);
        }
        [HttpGet]
        public List<FinishedGoodsInspection> GetFinishedGoodsInspectionField()
        {
            List<FinishedGoodsInspection> tableLists = _finishedGoodsInspectionRepo.GetFinishedGoodsInspectionField();
            //var dataList = new Items();
            return tableLists;
        }
        [HttpGet]
        public List<ItemSetup> ProductWithCategoryFilter(string ProductType)
        {
            List<ItemSetup> tableLists = _finishedGoodsInspectionRepo.GetProductWithCategoryFilter(ProductType);
            //var dataList = new Items();
            return tableLists;
        }
        public IHttpActionResult saveFinishedGoodsInspection(FinishedGoodsInspection predispatchInspection)
        {
            try
            {
                bool isPosted = _finishedGoodsInspectionRepo.InsertFinishedGoodsInspectionData(predispatchInspection);
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

        public FinishedGoodsInspection GetEditFinishedGoodsInspection(string transactionno)
        {
            FinishedGoodsInspection qc = _finishedGoodsInspectionRepo.GetEditFinishedGoodsInspection(transactionno);
            return qc;
        }
        [HttpGet]
        public FinishedGoodsInspection GetFinishedGoodsInspectionReport(string transactionno)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var SalesOrderDetailFormDetailByFormCodeAndOrderNo = this._finishedGoodsInspectionRepo.GetFinishedGoodsInspectionReport(transactionno);
            return SalesOrderDetailFormDetailByFormCodeAndOrderNo;
        }
    }
}