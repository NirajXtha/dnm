using NeoErp.Core;
using NeoErp.Core.Caching;
using NeoErp.Core.Models;
using NeoErp.Core.Models.Log4NetLoggin;
using NeoErp.Data;
using NeoERP.QCQAManagement.Service.Interface;
using NeoERP.QCQAManagement.Service.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoERP.QCQAManagement.Controllers.API
{
    [RoutePrefix("api/InternalInspectionSetupAPI")]
    public class InternalInspectionSetupAPIController : ApiController
    {
        private const string QCQA = "Internal Inspection Setup";
        private IInternalInspectionSetupRepo _internalInspectionSetupRepo;
        private IDbContext _dbContext;
        private IWorkContext _workContext;
        private ICacheManager _cacheManager;
        private NeoErpCoreEntity _objectEntity;
        private readonly ILogErp _logErp;
        private DefaultValueForLog _defaultValueForLog;
        public InternalInspectionSetupAPIController(IInternalInspectionSetupRepo _IInternalInspectionSetupRepo, IDbContext dbContext, IWorkContext workContext, ICacheManager cacheManager, NeoErpCoreEntity objectEntity)
        {
            this._internalInspectionSetupRepo = _IInternalInspectionSetupRepo;
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            this._logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule, _defaultValueForLog.FormCode);
        }
        [HttpGet]
        public List<ProductDetails> GetProductDetails() // used for Product items data
        {
            List<ProductDetails> response = new List<ProductDetails>();
            try
            {
                response = _internalInspectionSetupRepo.GetProductDetails();
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        [HttpGet]
        public List<ItemSetup> ItemList()
        {
            List<ItemSetup> tableLists = _internalInspectionSetupRepo.GetItemLists();
            return tableLists;
        }
        [HttpPost]
        public IHttpActionResult saveInternalInspectionDetails(ProductDetails productDetails)
        {
            try
            {
                bool isPosted = _internalInspectionSetupRepo.InsertInternalInspectionDetails(productDetails);
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
        public ProductDetails GetInternalInspectionSetupById(string id)
        {
            ProductDetails response = _internalInspectionSetupRepo.GetInternalInspectionSetupById(id);
            return response;
        }
        [HttpPost]
        public HttpResponseMessage DeleteInternalInspectionSetupById(string id)
        {
            try
            {
                bool isDeleted = _internalInspectionSetupRepo.DeleteInternalInspectionSetupById(id);
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