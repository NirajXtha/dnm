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
    public class QCQADocumentFinderAPIController : ApiController
    {
        // GET: QCQADocumentFinderAPI
        private const string QCQA = "Document Finder";
        private IQCQADocumentFinderRepo _QCQADocumentFinderRepo;
        private IDbContext _dbContext;
        private IWorkContext _workContext;
        private ICacheManager _cacheManager;
        private NeoErpCoreEntity _objectEntity;
        private readonly ILogErp _logErp;
        private DefaultValueForLog _defaultValueForLog;
        // GET: QCQAAPI

        public QCQADocumentFinderAPIController(IQCQADocumentFinderRepo _IQCQADocumentFinderRepo, IDbContext dbContext, IWorkContext workContext, ICacheManager cacheManager, NeoErpCoreEntity objectEntity)
        {
            this._QCQADocumentFinderRepo = _IQCQADocumentFinderRepo;
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            this._logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule, _defaultValueForLog.FormCode);
        }
        public List<QCDocumentFinder> GetQCQADetailByFormCode(string formCode, string docVer = "all")
        {
            List<QCDocumentFinder> tableLists = _QCQADocumentFinderRepo.GetDocumentDetails(formCode, docVer);
            return tableLists;
        }

        [HttpPost]
        public HttpResponseMessage DeleteQCByTransaction(string transactionNo)
        {
            try
            {
                bool isDeleted = _QCQADocumentFinderRepo.DeleteQCByTransaction(transactionNo);
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