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
    public class RawMaterialAPIController : ApiController
    {
        private const string QCQA = "Raw Material Management";
        private IMaterialRepo _materialRepo;
        private IDbContext _dbContext;
        private IWorkContext _workContext;
        private ICacheManager _cacheManager;
        private NeoErpCoreEntity _objectEntity;
        private readonly ILogErp _logErp;
        private ISettingService _settingService;
        private DefaultValueForLog _defaultValueForLog;

        public RawMaterialAPIController(IMaterialRepo _IMaterialRepo, IDbContext dbContext, IWorkContext workContext, ICacheManager cacheManager, NeoErpCoreEntity objectEntity)
        {
            this._materialRepo = _IMaterialRepo;
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            this._logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule, _defaultValueForLog.FormCode);
        }
        // GET: RawMaterialAPI
        [HttpGet]
        public List<FormDetailSetup> GetRawMaterialList()
        {

            //ViewBag.QCNO = _materialRepo.GetQCQAVoucherNo("RawMaterial");
            List<FormDetailSetup> tableLists = _materialRepo.GetRawMaterialDetails("");
            return tableLists;
        }
        
        public List<RawMaterialTree> GetMaterialListsGroupWise()
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var result = _materialRepo.GetMaterialListsGroupWise();           
            return result;
        }
        public IHttpActionResult GetMaterialListsByItemCode(string itemCode, string itemMastercode, string searchText)
        {
            var result = _materialRepo.GetMaterialListsByItemCode(itemCode, itemMastercode, searchText);
            return Ok(result);
        }
        [HttpGet]
        public List<MuCodeModel> GetMuCode()
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<MuCodeModel>();
            if (this._cacheManager.IsSet($"GetMuCode_{userid}_{company_code}_{branch_code}"))
            {
                var data = _cacheManager.Get<List<MuCodeModel>>($"GetMuCode_{userid}_{company_code}_{branch_code}");
                response = data;
            }
            else
            {
                var MuCode = this._materialRepo.GetMuCode();
                this._cacheManager.Set($"GetMuCode_{userid}_{company_code}_{branch_code}", MuCode, 20);
                response = MuCode;
            }
            return response;
        }
        public List<Items> GetAllProductsListByFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter))
                return new List<Items>();

            var Filterdata = new List<Items>();
            if (filter == "!@$")
                return Filterdata;
            if (filter.ToLower().Contains("undef"))
                return Filterdata;

            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var ShowAdvanceAutoComplete = false;
            var setting = _settingService.LoadSetting<WebPrefrenceSetting>(Constants.WebPrefranceSetting);
            if (setting != null)
                bool.TryParse(setting.ShowAdvanceAutoComplete, out ShowAdvanceAutoComplete);
            if (ShowAdvanceAutoComplete == false)
            {
                if (this._cacheManager.IsSet($"GetMaterialLists_{userid}_{company_code}_{branch_code}_{filter}_{ShowAdvanceAutoComplete}"))
                {
                    var data = _cacheManager.Get<List<Items>>($"GetMaterialLists_{userid}_{company_code}_{branch_code}_{filter}_{ShowAdvanceAutoComplete}");
                    return data;
                }
                else
                {
                    var AllFilterIssues = this._materialRepo.GetMaterialLists();
                    this._cacheManager.Set($"GetMaterialLists_{userid}_{company_code}_{branch_code}_{filter}_{ShowAdvanceAutoComplete}", AllFilterIssues, 20);
                    return AllFilterIssues;
                }
            }
            return this._materialRepo.GetMaterialLists();
        }


        [HttpGet]
        public List<RawMaterialDetails> GetMUCodeByRawMaterialId(string productId)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<RawMaterialDetails>();

            var MUCodeByProductId = this._materialRepo.GetRawMaterialDataByItemCode(productId);
            response = MUCodeByProductId;
            return response;
        }
        [HttpGet]
        public List<Items> GetPending_RawMaterialsByItemId(string ItemCode)
        {
            List<Items> tableLists = _materialRepo.GetPending_RawMaterialsByItemId(ItemCode);
            var dataList = new Items();
            return tableLists;
        }
        [HttpGet]
        public List<BatchDetails> GetBatchNoByItemCode(string itemCode)
        {
            var response = this._materialRepo.GetBatchNoByItemCode(itemCode);
            return response;
        }
        [HttpGet]
        public List<BatchDetails> GetBatchNoByTransactionNo(string TransactionNo)
        {
            var response = this._materialRepo.GetBatchNoByTransactionNo(TransactionNo);
            return response;
        }
        public List<RawMaterial> GetMaterialDetailByProductType(string productType)
        {
            List<RawMaterial> tableLists = _materialRepo.GetMaterialDetailByProductType(productType);
            return tableLists;
        }
        [HttpPost]
        public IHttpActionResult saveDailyRawMaterial(RawMaterial rawMateriall)       
        {          
            try
            {
                bool isPosted = _materialRepo.InsertDailyRawMaterialData(rawMateriall);
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
        public RawMaterial GetEditDailyRawMaterial(string transactionno)
        {
            RawMaterial qc = _materialRepo.GetEditDailyRawMaterial(transactionno);
            return qc;
        }
        [HttpGet]
        public RawMaterial GetDailyRawMaterialReport(string transactionno)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var SalesOrderDetailFormDetailByFormCodeAndOrderNo = this._materialRepo.GetDailyRawMaterialReport(transactionno);
            return SalesOrderDetailFormDetailByFormCodeAndOrderNo;
        }

    }
}