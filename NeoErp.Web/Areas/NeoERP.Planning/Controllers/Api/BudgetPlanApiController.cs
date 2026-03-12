using NeoErp.Core;
using NeoErp.Core.Caching;
using NeoErp.Core.Helpers;
using NeoErp.Planning.Service.Interface;
using NeoErp.Planning.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoERP.Planning.Controllers.Api
{
    public class BudgetPlanApiController : ApiController
    {
        ICOAPlanRepo _COAPlanRepo;
        private IWorkContext _workContext;
        private ICacheManager _cacheManager;
        public BudgetPlanApiController(ICOAPlanRepo COAPlanRepo, ICacheManager cacheManager, IWorkContext workContext)
        {
            this._COAPlanRepo = COAPlanRepo;
            this._cacheManager = cacheManager;
            this._workContext = workContext;
        }
        [HttpGet]
        public List<ProductTree> GetAllCOANodes()
        {
            var userinfo = this._workContext.CurrentUserinformation;
            //var allProductList = _salesRegister.ProductListAllNodes().ToList();
            var allProductList = _COAPlanRepo.COAListAllNodes(userinfo).ToList();
            var productNodes = new List<ProductTree>();

            foreach (var prod in allProductList)
            {
                productNodes.Add(new ProductTree()
                {
                    Level = prod.LEVEL,
                    ItemName = prod.ItemName,
                    ItemCode = prod.ItemCode,
                    MasterItemCode = prod.MasterItemCode,
                    PreItemCode = prod.PreItemCode,
                    hasProducts = prod.GroupFlag == "N" ? true : false
                });
            }

            return productNodes;
        }

        [HttpGet]
        public List<ProductTree> GetChartOfAccountItems(filterOption filter, bool individual = false)
        {
            List<ProductTree> productRegister = new List<ProductTree>();
            if (individual == false)
            {
                var userinfo = this._workContext.CurrentUserinformation;
                productRegister = this._COAPlanRepo.ChartOfAccountList();
            }
            else
                productRegister = this._COAPlanRepo.ChartOfAccountList();
            return productRegister;
        }

        [HttpGet]
        public List<ProductTree> GetAllCOAByACCId(string prodId, string level, string masterCode)
        {
            var userinfo = this._workContext.CurrentUserinformation;
            //var allProductList = _salesRegister.GetProductsListByProductCode(level, masterCode).ToList();
            var allProductList = _COAPlanRepo.GetCOAListByCOACode(level, masterCode, userinfo).ToList();
            var productNodes = new List<ProductTree>();

            foreach (var prod in allProductList)
            {
                productNodes.Add(new ProductTree()
                {
                    Level = prod.LEVEL,
                    ItemName = prod.ItemName,
                    ItemCode = prod.ItemCode,
                    MasterItemCode = prod.MasterItemCode,
                    PreItemCode = prod.PreItemCode,
                    hasProducts = prod.GroupFlag == "N" ? true : false
                });
            }

            return productNodes;

        }
        [HttpPost]
        public List<COAPlanViewModal> GetCOAPlanList(filterOption model)
        {
            List<COAPlanViewModal> list = _COAPlanRepo.getAllCOAPlans(model.ReportFilters);
            return list;
        }

        public COAPlanSetupDetailViewModal GetPlanDetailValueByPlanCode(int plancode)
        {
            COAPlanSetupDetailViewModal entity = new COAPlanSetupDetailViewModal();
            entity = this._COAPlanRepo.GetBudgetPlanDetailValueByPlanCode(plancode);
            return entity;
        }

        [HttpPost]
        public HttpResponseMessage DeletePlane(int planCode)
        {
            try
            {
                var deleteStatus = this._COAPlanRepo.deleteSalesPlan(planCode);
                if (deleteStatus)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error", STATUS_CODE = (int)HttpStatusCode.OK });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        public HttpResponseMessage getCOADataForReference(string ItemList, string startDate, string endDate, string divisionCode, string branchCode, string dateFormat, string frequency)
        {
            try
            {
                var result = this._COAPlanRepo.getDataForRefrence(ItemList, startDate, endDate, divisionCode, branchCode, dateFormat, frequency);
                if (result.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { DATA = result.ToList(), MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { DATA = result, MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage CreateTempBudgetPlanReportTable(filterOption model)
        {
            if (model == null)
                model = new filterOption();
            var result = this._COAPlanRepo.CreateTemBudgetPlanReportTable(model.ReportFilters);
            //return result;
            if (result == "TableDropedAndCreated")
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "DELETECREATED", STATUS_CODE = (int)HttpStatusCode.OK });
            }
            else if (result == "TableCreated")
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "CREATED", STATUS_CODE = (int)HttpStatusCode.OK });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Fail", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
    }
}
