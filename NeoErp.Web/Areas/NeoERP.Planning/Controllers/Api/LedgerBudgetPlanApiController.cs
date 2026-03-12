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
    public class LedgerBudgetPlanApiController : ApiController
    {
        ILBAPlanRepo _LBAPlanRepo;
        private IWorkContext _workContext;
        private ICacheManager _cacheManager;
        public LedgerBudgetPlanApiController(ILBAPlanRepo LBAPlanRepo, ICacheManager cacheManager, IWorkContext workContext)
        {
            this._LBAPlanRepo = LBAPlanRepo;
            this._cacheManager = cacheManager;
            this._workContext = workContext;
        }
        [HttpGet]
        public List<ProductTree> GetAllLBANodes()
        {
            var userinfo = this._workContext.CurrentUserinformation;
            //var allProductList = _salesRegister.ProductListAllNodes().ToList();
            var allProductList = _LBAPlanRepo.LBAListAllNodes(userinfo).ToList();
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
        public List<ProductTree> GetAllProductsWithChildItem(string accCode)
        {
            var result = _LBAPlanRepo.getAllProductsWithChildItem(accCode);
            var productNodes = new List<ProductTree>();
            var productNode = generateProductTree(result, productNodes, "999999");
            return productNode;
        }
        public List<AccountSetup> getAllAccountSetup(string filter)
        {
            var result = _LBAPlanRepo.getAllAccounts();
            return result;
        }

        private List<ProductTree> generateProductTree(List<ProductSetupModel> model, List<ProductTree> productNodes, string preItemCode)
        {
            foreach (var item in model.Where(x => x.PreItemCode == preItemCode))
            {
                var productNodesChild = new List<ProductTree>();
                productNodes.Add(new ProductTree()
                {
                    Level = item.LEVEL,
                    ItemName = item.ItemName,
                    ItemCode = item.ItemCode,
                    GroupSkuFlag = item.GroupFlag,
                    MasterItemCode = item.MasterItemCode,
                    PreItemCode = item.PreItemCode,
                    hasProducts = item.GroupFlag == "G" ? true : false,
                    Items = item.GroupFlag == "G" ? generateProductTree(model, productNodesChild, item.MasterItemCode) : null,
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
                //var productRegister = _salesRegister.SalesRegisterProducts().ToList();

                if (this._cacheManager.IsSet("SalesRegisterProduct"))
                {
                    productRegister = this._cacheManager.Get<List<ProductTree>>("SalesRegisterProduct");
                }
                else
                {
                    productRegister = this._LBAPlanRepo.ChartOfAccountList();
                    this._cacheManager.Set("SalesRegisterProduct", productRegister, 1);
                }
            }
            else
                productRegister = this._LBAPlanRepo.ChartOfAccountList();
            return productRegister;
        }

        [HttpGet]
        public List<ProductTree> GetAllLBAByACCId(string prodId, string level, string masterCode)
        {
            var userinfo = this._workContext.CurrentUserinformation;
            //var allProductList = _salesRegister.GetProductsListByProductCode(level, masterCode).ToList();
            var allProductList = _LBAPlanRepo.GetLBAListByLBACode(level, masterCode, userinfo).ToList();
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
        public List<LBAPlanViewModal> GetLBAPlanList(filterOption model)
        {
            List<LBAPlanViewModal> list = _LBAPlanRepo.getAllLBAPlans(model.ReportFilters);
            return list;
        }

        public LBAPlanSetupDetailViewModal GetPlanDetailValueByPlanCode(int plancode)
        {
            LBAPlanSetupDetailViewModal entity = new LBAPlanSetupDetailViewModal();
            entity = this._LBAPlanRepo.GetLedgerBudgetPlanDetailValueByPlanCode(plancode);
            return entity;
        }

        [HttpPost]
        public HttpResponseMessage DeletePlane(int planCode)
        {
            try
            {
                var deleteStatus = this._LBAPlanRepo.deleteSalesPlan(planCode);
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

        public HttpResponseMessage getLBADataForReference(string plan_code,string ItemList,string subCode, string startDate, string endDate, string divisionCode, string branchCode, string dateFormat, string frequency)
        {
            try
            {
                var result = this._LBAPlanRepo.getDataForRefrence(plan_code,ItemList, subCode, startDate, endDate, divisionCode, branchCode, dateFormat, frequency);
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

        public HttpResponseMessage getTotalReferenceAmount(string acc_code)
        {
            try
            {
                var result = this._LBAPlanRepo.getTotalReferenceAmount(acc_code);
                if (result!=null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { DATA = result, MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
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
        public HttpResponseMessage CreateLBPlanReportTable(filterOption model)
        {
            if (model == null)
                model = new filterOption();
            var result = this._LBAPlanRepo.CreateTemLBPlanReportTable(model.ReportFilters);
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
