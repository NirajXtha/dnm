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
    public class InternalInspectionAPIController : ApiController
    {
        // GET: InternalInspectionAPI
        // GET: OnSiteInspectionAPI
        private const string QCQA = "Internal Inspection";
        private IInternalInspectionRepo _internalInspectionRepo;
        private IDbContext _dbContext;
        private IWorkContext _workContext;
        private ICacheManager _cacheManager;
        private NeoErpCoreEntity _objectEntity;
        private readonly ILogErp _logErp;
        private ISettingService _settingService;
        private DefaultValueForLog _defaultValueForLog;
        public InternalInspectionAPIController(IInternalInspectionRepo _IInternalInspectionRepo, IDbContext dbContext, IWorkContext workContext, ICacheManager cacheManager, NeoErpCoreEntity objectEntity)
        {
            this._internalInspectionRepo = _IInternalInspectionRepo;
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            this._logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule, _defaultValueForLog.FormCode);
        }
        [HttpGet]
        public List<FormDetailSetup> GetInternalInspectionList()
        {
            List<FormDetailSetup> tableLists = _internalInspectionRepo.GetInternalInspectionList();
            return tableLists;
        }
        public List<INTERNALPRODUCTLIST> GetProductsByProductType(string productType)
        {
            List<INTERNALPRODUCTLIST> tableLists = _internalInspectionRepo.GetProductsByProductType(productType);
            return tableLists;
        }
        [HttpGet]
        public List<ItemSetup> VendorDetailsList(string Product)
        {
            List<ItemSetup> tableLists = _internalInspectionRepo.GetVendorDetailsList(Product);
            //var dataList = new Items();
            return tableLists;
        }
        //[HttpGet]
        //public List<ParameterDetails> GetParameterDetailsByItemCode(string productType, string itemCode,string formType)
        //{
        //    List<ParameterDetails> tableLists = _internalInspectionRepo.GetParameterDetailsByItemCode(productType,itemCode, formType);
        //    return tableLists;
        //}
        public List<ParameterDetails> GetParameterDetailsByItemCode(string ProductId)
        {
            List<ParameterDetails> tableLists = _internalInspectionRepo.GetParameterDetailsByItemCode(ProductId);
            return tableLists;
        }
        public IHttpActionResult saveInternalInspection(OnSiteInspection onSiteInspection)
        {
            try
            {
                bool isPosted = _internalInspectionRepo.InsertInternalInspectionData(onSiteInspection);
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
        public OnSiteInspection GetEditInternalInspection(string transactionno)
        {
            OnSiteInspection qc = _internalInspectionRepo.GetEditInternalInspection(transactionno);
            return qc;
        }
        [HttpGet]
        public OnSiteInspection GetInternalInspectionReport(string transactionno)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var SalesOrderDetailFormDetailByFormCodeAndOrderNo = this._internalInspectionRepo.GetInternalInspectionReport(transactionno);
            return SalesOrderDetailFormDetailByFormCodeAndOrderNo;
        }
    }
}