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
    public class ParameterInspectionSetupAPIController : ApiController
    {
        // GET: ParameterInspectionSetupAPI
        // GET: SanitationHygiene
        private const string QCQA = "Parameter Inspection Setup";
        private IParameterInspectionSetupRepo _parameterInspectionSetupRepo;
        private IDbContext _dbContext;
        private IWorkContext _workContext;
        private ICacheManager _cacheManager;
        private NeoErpCoreEntity _objectEntity;
        private readonly ILogErp _logErp;
        private ISettingService _settingService;
        private DefaultValueForLog _defaultValueForLog;
        public ParameterInspectionSetupAPIController(IParameterInspectionSetupRepo _IParameterInspectionSetupRepo, IDbContext dbContext, IWorkContext workContext, ICacheManager cacheManager, NeoErpCoreEntity objectEntity)
        {
            this._parameterInspectionSetupRepo = _IParameterInspectionSetupRepo;
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            this._logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule, _defaultValueForLog.FormCode);
        }
        public List<ParameterInspectionSetup> GetItemCheckListDetails()
        {
            try
            {
                List<ParameterInspectionSetup> tableList = new List<ParameterInspectionSetup>();
                tableList = _parameterInspectionSetupRepo.GetItemCheckListDetails();
                return tableList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<ParameterInspectionSetup> GetItemCheckList()
        {
            try
            {
                List<ParameterInspectionSetup> tableList = new List<ParameterInspectionSetup>();
                tableList = _parameterInspectionSetupRepo.GetItemCheckList();
                return tableList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<ParameterInspectionSetup> GetParameterInspectionList()
        {
            try
            {
                var company_code = _workContext.CurrentUserinformation.company_code;
                List<ParameterInspectionSetup> tableList = new List<ParameterInspectionSetup>();
                tableList = _parameterInspectionSetupRepo.GetParameterInspectionList();
                return tableList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IHttpActionResult saveParameterInspection(ParameterInspectionSetup parameterInspection)
        {
            try
            {
                bool isPosted = _parameterInspectionSetupRepo.InsertParameterInspectionSetupData(parameterInspection);
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
        [HttpGet]
        public ParameterInspectionSetup GetParameterInspectionById(string id)
        {
            ParameterInspectionSetup response = _parameterInspectionSetupRepo.GetParameterInspectionById(id);
            return response;
        }
    }
}