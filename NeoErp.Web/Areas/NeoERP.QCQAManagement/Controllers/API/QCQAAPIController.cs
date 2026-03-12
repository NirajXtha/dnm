using System;
using System.Collections.Generic;
using NeoERP.QCQAManagement.Service.Models;
using System.Linq;
using System.Web;
//using System.Web.Mvc;
using NeoERP.QCQAManagement.Service.Interface;
using NeoErp.Data;
using NeoErp.Core;
using NeoErp.Core.Caching;
using NeoErp.Core.Models;
using NeoErp.Core.Models.Log4NetLoggin;
using NeoErp.Core.Services.CommonSetting;
using System.Web.Http;
using System.Net.Http;
using System.Net;

namespace NeoERP.QCQAManagement.Controllers.API
{
    public class QCQAAPIController : ApiController
    {
        private const string QCQA = "QCQA Management";
        private IQCQARepo _quotRepo;
        private IDbContext _dbContext;
        private IWorkContext _workContext;
        private ICacheManager _cacheManager;
        private NeoErpCoreEntity _objectEntity;
        private readonly ILogErp _logErp;
        private ISettingService _settingService;
        private DefaultValueForLog _defaultValueForLog;
        // GET: QCQAAPI

        public QCQAAPIController(IQCQARepo _IQuotRepo, IDbContext dbContext, IWorkContext workContext, ICacheManager cacheManager, NeoErpCoreEntity objectEntity)
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
        public List<FormDetailSetup> GetQCQADetails(string tableName)
        {
            List<FormDetailSetup> response = new List<FormDetailSetup>();
            try
            {
                response = _quotRepo.GetQCQADetails(tableName);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        // [HttpGet]
        public List<FormDetailSetup> GetFormDetails()
        {
            List<FormDetailSetup> response = new List<FormDetailSetup>();
            try
            {
                List<FormDetailSetup> itemDeatils = new List<FormDetailSetup>();
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        [HttpGet]
        public List<TableList> TableLists()
        {
            List<TableList> tableLists = _quotRepo.GetTableLists();
            return tableLists;
        }
        [HttpGet]
        public List<FormDetailSetup> GetQCQADetailsByTableName(string tableName)
        {
            List<FormDetailSetup> response = _quotRepo.GetQCQADetailsByTableName(tableName);
            return response;
        }

        [HttpPost]
        public HttpResponseMessage AddColumnsToTable(List<FormDetailSetup> modal,string tableName)
        {
            var message = this._quotRepo.AddColumnsToTable(modal,tableName);
            if (message == "success")
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Dealer Updated Successfully", STATUS_CODE = (int)HttpStatusCode.OK });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something Wrong !! Try again Later", STATUS_CODE = (int)HttpStatusCode.BadRequest });
            }

        }

        public IList<FormSetupModel> GetFormCode(string tableName)
        {
            return this._quotRepo.GetFormCode(_workContext.CurrentUserinformation,tableName);

        }
        [HttpGet]
        public List<FormDetailSetup> GetQCFormDetailSetup(string formCode) 
        {
            _logErp.InfoInFile("Get Form Details Setup for : " + formCode + " formcode");
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;

            var response = new List<FormDetailSetup>();
            //if (this._cacheManager.IsSet($"fromdetailsetup_{_workContext.CurrentUserinformation.User_id}_{company_code}_{branch_code}_{formCode}"))
            //{

            //    var data = _cacheManager.Get<List<FormDetailSetup>>($"fromdetailsetup_{_workContext.CurrentUserinformation.User_id}_{company_code}_{branch_code}_{formCode}");
            //    _logErp.InfoInFile(data.Count() + " Form Details setup has been fetched from cached for " + formCode + " formcode");
            //    response = data;
            //}
            //else
            //{
            //    var formDetailList = this._quotRepo.GetQCFormDetailSetup(formCode);
            //    this._cacheManager.Set($"fromdetailsetup_{_workContext.CurrentUserinformation.User_id}_{company_code}_{branch_code}_{formCode}", formDetailList, 20);
            //    _logErp.InfoInFile(formDetailList.Count() + " form details setup has beed fetched for " + formCode + " formcode");
            //    response = formDetailList;
            //}

            var formDetailList = this._quotRepo.GetQCFormDetailSetup(formCode);
            //this._cacheManager.Set($"fromdetailsetup_{_workContext.CurrentUserinformation.User_id}_{company_code}_{branch_code}_{formCode}", formDetailList, 20);
            //_logErp.InfoInFile(formDetailList.Count() + " form details setup has beed fetched for " + formCode + " formcode");
            response = formDetailList;

            return response;
        }
    }
}