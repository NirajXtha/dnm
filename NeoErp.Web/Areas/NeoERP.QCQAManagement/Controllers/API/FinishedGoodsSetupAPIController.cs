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
    public class FinishedGoodsSetupAPIController : ApiController
    {
        // GET: FinishedGoodsSetupAPI
        private const string QCQA = "Parameter Inspection Setup";
        private IFinishedGoodsSetupRepo _parameterInspectionSetupRepo;
        private IDbContext _dbContext;
        private IWorkContext _workContext;
        private ICacheManager _cacheManager;
        private NeoErpCoreEntity _objectEntity;
        private readonly ILogErp _logErp;
        private ISettingService _settingService;
        private DefaultValueForLog _defaultValueForLog;
        public FinishedGoodsSetupAPIController(IFinishedGoodsSetupRepo _IParameterInspectionSetupRepo, IDbContext dbContext, IWorkContext workContext, ICacheManager cacheManager, NeoErpCoreEntity objectEntity)
        {
            this._parameterInspectionSetupRepo = _IParameterInspectionSetupRepo;
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            this._logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule, _defaultValueForLog.FormCode);
        }
        public List<ParameterInspectionSetup> GetFinishedItemCheckListDetails()
        {
            try
            {
                List<ParameterInspectionSetup> tableList = new List<ParameterInspectionSetup>();
                tableList = _parameterInspectionSetupRepo.GetFinishedItemCheckListDetails();
                return tableList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<ParameterInspectionSetup> GetFinishedItemCheckList()
        {
            try
            {
                List<ParameterInspectionSetup> tableList = new List<ParameterInspectionSetup>();
                tableList = _parameterInspectionSetupRepo.GetFinishedItemCheckList();
                return tableList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<ParameterInspectionSetup> GetFinishedInspectionList()
        {
            try
            {
                var company_code = _workContext.CurrentUserinformation.company_code;
                List<ParameterInspectionSetup> tableList = new List<ParameterInspectionSetup>();
                tableList = _parameterInspectionSetupRepo.GetFinishedInspectionList();
                return tableList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IHttpActionResult saveFinishedGoods(ParameterInspectionSetup parameterInspection)
        {
            try
            {
                bool isPosted = _parameterInspectionSetupRepo.InsertFinishedGoodsSetupData(parameterInspection);
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
        public ParameterInspectionSetup GetFinishedGoodsById(string id)
        {
            ParameterInspectionSetup response = _parameterInspectionSetupRepo.GetFinishedGoodsById(id);
            return response;
        }
    }
}