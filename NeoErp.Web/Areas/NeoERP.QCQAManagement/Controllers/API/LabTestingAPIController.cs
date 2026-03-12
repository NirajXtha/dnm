using NeoErp.Core;
using NeoErp.Core.Caching;
using NeoErp.Core.Models;
using NeoErp.Core.Models.Log4NetLoggin;
using NeoErp.Core.Services.CommonSetting;
using NeoErp.Data;
using NeoERP.QCQAManagement.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace NeoERP.QCQAManagement.Controllers.API
{
    public class LabTestingAPIController : ApiController
    {
        // GET: LabTestingAPI
        private const string QCQA = "Lab Testing Report";
        private ILabTestingRepo _labTestingRepo;
        private IDbContext _dbContext;
        private IWorkContext _workContext;
        private ICacheManager _cacheManager;
        private NeoErpCoreEntity _objectEntity;
        private readonly ILogErp _logErp;
        private ISettingService _settingService;
        private DefaultValueForLog _defaultValueForLog;
        public LabTestingAPIController(ILabTestingRepo _ILabTestingRepo, IDbContext dbContext, IWorkContext workContext, ICacheManager cacheManager, NeoErpCoreEntity objectEntity)
        {
            this._labTestingRepo = _ILabTestingRepo;
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            this._logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule, _defaultValueForLog.FormCode);
        }
    }
}