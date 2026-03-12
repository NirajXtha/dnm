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
using System.Web.Http.Results;

namespace NeoERP.QCQAManagement.Controllers.API
{
    [RoutePrefix("api/ProductSetupAPI")]
    public class ProductSetupAPIController : ApiController
    {
        // GET: ProductSetupAPI
        private const string QCQA = "Product Setup";
        private IProductSetupRepo _productSetupRepo;
        private IDbContext _dbContext;
        private IWorkContext _workContext;
        private ICacheManager _cacheManager;
        private NeoErpCoreEntity _objectEntity;
        private readonly ILogErp _logErp;
        private ISettingService _settingService;
        private DefaultValueForLog _defaultValueForLog;
        public ProductSetupAPIController(IProductSetupRepo _IProductSetupRepo, IDbContext dbContext, IWorkContext workContext, ICacheManager cacheManager, NeoErpCoreEntity objectEntity)
        {
            this._productSetupRepo = _IProductSetupRepo;
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            this._logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule, _defaultValueForLog.FormCode);
        }
        [HttpGet]
        public List<ItemDetails> GetItemDetails()
        {
            List<ItemDetails> response = new List<ItemDetails>();
            try
            {
                response = _productSetupRepo.GetItemDetails();
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        [HttpGet]
        public List<ProductSetup> ProductTypeList()
        {
            List<ProductSetup> tableLists = _productSetupRepo.GetProductTypeLists();
            //var dataList = new Items();
            return tableLists;
        }
        [HttpGet]
        public List<ItemSetup> ProductList(string ProductType,string Category_Code)
        {
            List<ItemSetup> tableLists = _productSetupRepo.GetProductLists(ProductType, Category_Code);
            //var dataList = new Items();
            return tableLists;

        }
        [HttpGet]
        public List<ProductDetails> GetParameterList()
        {
            List<ProductDetails> tableLists = _productSetupRepo.GetParameterList();
            return tableLists;
        }
        [HttpPost]
        public IHttpActionResult saveProductDetails(ProductDetails productDetails)
        {
            try
            {
                bool isPosted = _productSetupRepo.InsertProductDetails(productDetails);
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
        public ProductDetails GetProductById(string id)
        {
            ProductDetails response = _productSetupRepo.GetProductById(id);
            return response;
        }
        [HttpPost]
        public HttpResponseMessage DeleteProductSetupById(string id)
        {
            try
            {
                bool isDeleted = _productSetupRepo.DeleteProductSetupById(id);
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