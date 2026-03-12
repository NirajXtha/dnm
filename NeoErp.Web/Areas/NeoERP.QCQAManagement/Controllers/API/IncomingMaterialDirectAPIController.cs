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
    public class IncomingMaterialDirectAPIController : ApiController
    {
        private const string QCQA = "Material Management";
        private IMaterialRepo _materialRepo;
        private IDbContext _dbContext;
        private IWorkContext _workContext;
        private ICacheManager _cacheManager;
        private NeoErpCoreEntity _objectEntity;
        private readonly ILogErp _logErp;
        private ISettingService _settingService;
        private DefaultValueForLog _defaultValueForLog;
        // GET: QCQAAPI

        public IncomingMaterialDirectAPIController(IMaterialRepo _IMaterialRepo, IDbContext dbContext, IWorkContext workContext, ICacheManager cacheManager, NeoErpCoreEntity objectEntity)
        {
            this._materialRepo = _IMaterialRepo;
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            this._logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule, _defaultValueForLog.FormCode);
        }
        // GET: IncomingMaterialAPI
        [HttpGet]
        public List<Items> MaterialLists()
        {
            List<Items> tableLists = _materialRepo.GetMaterialLists();
            var dataList = new Items();
            return tableLists;
        }
        [HttpGet]
        public List<Items> GetSupplierLists()
        {
            List<Items> tableLists = _materialRepo.GetSupplierLists();
            return tableLists;
        }

        [HttpGet]
        public List<Items> MaterialListsByCategory(string CategoryCode)
        {
            List<Items> tableLists = _materialRepo.GetMaterialListsByCategory(CategoryCode);
            var dataList = new Items();
            return tableLists;
        }

        [HttpGet]
        public List<Items> GetVoucherDetailsByItemId(string ItemCode)
        {
            List<Items> tableLists = _materialRepo.GetVoucherDetailsByItemId(ItemCode);
            var dataList = new Items();
            return tableLists;
        }

        [HttpGet]
        public List<Items> GetPending_VoucherDetailsByItemId(string ItemCode)
        {
            List<Items> tableLists = _materialRepo.GetPending_VoucherDetailsByItemId(ItemCode);
            var dataList = new Items();
            return tableLists;
        }

        [HttpGet]
        public string GetQCQAVoucherNo()
        {
            string vouvherNo = _materialRepo.GetQCQAVoucherNo("");
            return vouvherNo;
        }

        [HttpGet]
        public List<Items> MaterialDetailLists(string VoucherNo)
        {
            List<Items> tableLists = _materialRepo.GetMaterialDetailLists(VoucherNo);
            var dataList = new Items();
            return tableLists;
        }

      

        [HttpPost]
        public IHttpActionResult saveInComingMaterialSample(QCDetail qcDetailList)
        {
            try
            {
                bool isPosted = _materialRepo.InsertInComingMaterialSampleData(qcDetailList);
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
        public Items GetEditMaterialDetailsSample(string transactionno)
        {
            Items qc = _materialRepo.GetEditMaterialDetailsSample(transactionno);
            return qc;
        }
        [HttpGet]
        public IncomingMaterial GetIncomingMaterialSampleDetailReport(string transactionno)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var SalesOrderDetailFormDetailByFormCodeAndOrderNo = this._materialRepo.GetIncomingMaterialSampleDetailReport(transactionno);
            return SalesOrderDetailFormDetailByFormCodeAndOrderNo;
        }
    }
}