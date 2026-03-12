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
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace NeoERP.QCQAManagement.Controllers.API
{
    public class QCQANumberSetupAPIController : ApiController
    {
    private const string QCQA = "QCQA Number Setup";
    private IQCQANumberRepo _QCQANumberRepo;
    private IDbContext _dbContext;
    private IWorkContext _workContext;
    private ICacheManager _cacheManager;
    private NeoErpCoreEntity _objectEntity;
    private readonly ILogErp _logErp;
    private ISettingService _settingService;
    private DefaultValueForLog _defaultValueForLog;
    // GET: QCQAAPI

    public QCQANumberSetupAPIController(IQCQANumberRepo _IQCQANumberRepo, IDbContext dbContext, IWorkContext workContext, ICacheManager cacheManager, NeoErpCoreEntity objectEntity)
    {
        this._QCQANumberRepo = _IQCQANumberRepo;
        this._dbContext = dbContext;
        this._objectEntity = objectEntity;
        this._workContext = workContext;
        this._cacheManager = cacheManager;
        this._defaultValueForLog = new DefaultValueForLog(this._workContext);
        this._logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule, _defaultValueForLog.FormCode);
    }
        // GET: QCQANumberSetupAPI
        [HttpGet]
        public List<FORM_SETUP> QCQANumberDetails()
        {
            List<FORM_SETUP> response = new List<FORM_SETUP>();
            try
            {
                response = _QCQANumberRepo.GetQCNumberDetails();
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        [HttpPost]
        public IHttpActionResult saveQCSetup(FORM_SETUP formData)
        {
            try
            {
                bool isPosted = _QCQANumberRepo.InsertQCData(formData);
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
        public List<FORM_SETUP> GetQCQAById(string formCode)
        {
            List<FORM_SETUP> response = new List<FORM_SETUP>();
            try
            {
                response = _QCQANumberRepo.GetQCQAById(formCode);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        [HttpPost]
        public HttpResponseMessage DeleteQCQAById(string formCode)
        {
            try
            {
                bool isDeleted = _QCQANumberRepo.DeleteQCQAId(formCode);
                if (isDeleted)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Deleted successfully!!", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    // Handle case where project was not posted successfully
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Failed to delete No.!!", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occurred during the operation
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
    }
}