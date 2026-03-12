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
    public class GlobalAgroProductsAPIController : ApiController
    {
        // GET: GlobalAgroProductsAPI
        private const string QCQA = "Global Agro Products";
        private IGlobalAgroProductsRepo _globalAgroProductsRepo;
        private IDbContext _dbContext;
        private IWorkContext _workContext;
        private ICacheManager _cacheManager;
        private NeoErpCoreEntity _objectEntity;
        private readonly ILogErp _logErp;
        private ISettingService _settingService;
        private DefaultValueForLog _defaultValueForLog;
        public GlobalAgroProductsAPIController(IGlobalAgroProductsRepo _IGlobalAgroProductsRepo, IDbContext dbContext, IWorkContext workContext, ICacheManager cacheManager, NeoErpCoreEntity objectEntity)
        {
            this._globalAgroProductsRepo = _IGlobalAgroProductsRepo;
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            this._logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule, _defaultValueForLog.FormCode);
        }
        [HttpGet]
        public GlobalAgroProducts GetGlobalAgroProductLists()
        {
            GlobalAgroProducts tableLists = _globalAgroProductsRepo.GetGlobalAgroProductLists();
            return tableLists;
        }
        [HttpGet]
        public List<Items> GlobalAgroMaterialDetailLists(string VoucherNo)
        {
            List<Items> tableLists = _globalAgroProductsRepo.GetGlobalAgroMaterialDetailLists(VoucherNo);
            var dataList = new Items();
            return tableLists;
        }
        [HttpGet]
        public List<Items> GetGateEntryDetailsByItemId(string ItemCode)
        {
            List<Items> tableLists = _globalAgroProductsRepo.GetGateEntryDetailsByItemId(ItemCode);
            var dataList = new Items();
            return tableLists;
        }
        [HttpGet]
        public List<Items> GetGRNDetailsByItemId(string ItemCode)
        {
            List<Items> tableLists = _globalAgroProductsRepo.GetGRNDetailsByItemId(ItemCode);
            var dataList = new Items();
            return tableLists;
        }
        [HttpPost]
        public IHttpActionResult saveGlobalAgroProducts(GlobalAgroProducts globalAgroProducts)
        {
            try
            {
                bool isPosted = _globalAgroProductsRepo.InsertGlobalAgroProductsData(globalAgroProducts);
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
        public GlobalAgroProducts GetEditGlobalAgroProductLists(string transactionno)
        {
            GlobalAgroProducts qc = _globalAgroProductsRepo.GetEditGlobalAgroProductLists(transactionno);
            return qc;
        }
        [HttpGet]
        public GlobalAgroProducts GetGlobalAgroProductsReport(string transactionno)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            //var response = new List<IncomingMaterial>();
            var SalesOrderDetailFormDetailByFormCodeAndOrderNo = _globalAgroProductsRepo.GetGlobalAgroProductsReport(transactionno);
            
            //ViewBag.TRANSACTION_NO = SalesOrderDetailFormDetailByFormCodeAndOrderNo.TRANSACTION_NO;
            //response = SalesOrderDetailFormDetailByFormCodeAndOrderNo;
            return SalesOrderDetailFormDetailByFormCodeAndOrderNo;
        }

        
    }
}