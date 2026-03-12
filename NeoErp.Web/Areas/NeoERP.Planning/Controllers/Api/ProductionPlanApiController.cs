using NeoErp.Core;
using NeoErp.Core.Caching;
using NeoErp.Core.Helpers;
using NeoErp.Planning.Service.Interface;
using NeoErp.Planning.Service.Models;
using NeoERP.Planning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoERP.Planning.Controllers.Api
{
    public class ProductionPlanApiController : ApiController
    {
        IPlan _plan;
        ICOAPlanRepo _COAPlanRepo;
        private IWorkContext _workContext;
        private ICacheManager _cacheManager;
        private IProductionPlanRepo _ProductionPlanRepo;

        public ProductionPlanApiController(ICOAPlanRepo COAPlanRepo, IProductionPlanRepo ProductionPlanRepo,ICacheManager cacheManager, IWorkContext workContext,IPlan plan)
        {
            this._COAPlanRepo = COAPlanRepo;
            this._ProductionPlanRepo= ProductionPlanRepo;
            this._cacheManager = cacheManager;
            this._plan = plan;
            this._workContext = workContext;
        }

        public PL_PRO_PLAN GetPlanDetailValueByPlanCode(int plancode)
       {
            PL_PRO_PLAN entity = new PL_PRO_PLAN();
            entity = this._ProductionPlanRepo.GetPlanDetailValueByPlanCode(plancode);
            return entity;
        }

        [HttpPost]
        public HttpResponseMessage DeletePlane(int planCode)
        {
            try
            {
                var deleteStatus = this._ProductionPlanRepo.deleteProductionPlan(planCode);
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
        [HttpGet]
        public List<ProductTree> GetAllProductNodes()
        {
            var userinfo = this._workContext.CurrentUserinformation;
            //var allProductList = _salesRegister.ProductListAllNodes().ToList();
            var allProductList = _ProductionPlanRepo.ProductListAllNodes(userinfo).ToList();
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
                    hasProducts = prod.GroupFlag == "G" ? true : false
                });
            }

            return productNodes;
        }
       
        [HttpGet]
        public List<ProductTree> GetAllProductsByProdId(string prodId, string level, string masterCode)
        {
            var userinfo = this._workContext.CurrentUserinformation;
            //var allProductList = _salesRegister.GetProductsListByProductCode(level, masterCode).ToList();
            var allProductList = _ProductionPlanRepo.GetProductsListByProductCode(level, masterCode, userinfo).ToList();
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
                    hasProducts = prod.GroupFlag == "G" ? true : false
                });
            }

            return productNodes;

        }
        [HttpGet]
        //[CacheOutput(ClientTimeSpan =300,ServerTimeSpan =300)]
        public List<ProductTree> GetAllProductsWithChildItem()
        {
            var result = _ProductionPlanRepo.getAllProductsWithChildItem();
            var productNodes = new List<ProductTree>();
            var productNode = generateProductTree(result, productNodes, "00");
            return productNode;
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
        public HttpResponseMessage getSalesItemDataForReference(string ItemList, string startDate, string endDate, string customerCode, string divisionCode, string branchCode, string dateFormat, string frequency)
        {
            try
            {
                var remainingQty = 0M;
                var nextMonth = string.Empty;

                var finalResult = new List<PlanSalesRefrenceModel>();

                var result = this._ProductionPlanRepo.getSalesItemDataForRefrence(ItemList, startDate, endDate, customerCode, divisionCode, branchCode, dateFormat, frequency);
                var stockResult = this._ProductionPlanRepo.getPorcumentItemDataForStock(ItemList, startDate, endDate, customerCode, divisionCode, branchCode, dateFormat, frequency);

                var uncommonList = result.Where(a => !stockResult.Any(b => a.ITEM_CODE == b.ITEM_CODE)).ToList();
                var commonList = result.Where(a => stockResult.Any(b => a.ITEM_CODE == b.ITEM_CODE)).OrderBy(o => o.NEPALI_YEAR).ThenBy(o=>o.ITEM_CODE).ThenBy(o=>o.NEPALI_MONTH).ToList();

                foreach (var stockItem in stockResult)
                {
                    var getSingleItemList = commonList.Where(a => a.ITEM_CODE == stockItem.ITEM_CODE).OrderBy(o=>o.NEPALI_YEAR).ThenBy(o=> o.NEPALI_MONTH).ToList();
                  
                    foreach (var singleItem in getSingleItemList)
                    {
                        if (stockItem.ITEM_CODE == singleItem.ITEM_CODE)
                        {
                            var stockQty = Convert.ToDecimal(stockItem.QTY);
                            if (remainingQty != 0)
                            {
                                stockQty = remainingQty;
                            }
                            var qty = Convert.ToDecimal(singleItem.QTY) - Convert.ToDecimal(stockQty);
                            if (qty < 0)
                            {
                                remainingQty = -qty;
                                qty = 0;
                                commonList = commonList.Where(a => a.ITEM_CODE != singleItem.ITEM_CODE).ToList();
                            }
                            else {
                                remainingQty = 0;
                                stockItem.QTY = "0";
                            }
                            singleItem.QTY = qty.ToString();
                        }
                    }
                    finalResult.AddRange(getSingleItemList);

                }

                finalResult.AddRange(commonList);
                finalResult.AddRange(uncommonList);

                
                if (finalResult.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { DATA = finalResult.ToList(), MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { DATA = finalResult, MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        public void transferQtyToNextMonth() {

        }

        public HttpResponseMessage getPorcumentItemDataForStock(string ItemList, string startDate, string endDate, string customerCode, string divisionCode, string branchCode, string dateFormat, string frequency)
        {
            try
            {
                var result = this._ProductionPlanRepo.getPorcumentItemDataForStock(ItemList, startDate, endDate, customerCode, divisionCode, branchCode, dateFormat, frequency);
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
        

        [HttpGet]
        public List<ProductionPlan> GetSalesPlanList(string customercode, string employeecode, string divisioncode, string branchcode, string startdate, string enddate)
        {
            List<ProductionPlan> model = new List<ProductionPlan>();
            model = _ProductionPlanRepo.getAllProductionPlans(customercode, employeecode, divisioncode, branchcode, startdate, enddate);
            return model;
        }

        [HttpPost]
        public List<ProductionPlan> GetSalesPlanList(filterOption model)
        {
            return _ProductionPlanRepo.getAllProductionPlans(model.ReportFilters);

        }

        [HttpGet]
        public HttpResponseMessage getEmployeeByUserId(string userId)
        {
            var result = this._plan.GetEmployeeByID(userId);
            return Request.CreateResponse(HttpStatusCode.OK, new { DATA = result, MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
        }
        [HttpGet]
        public List<FrequencyModels> GetAllFrequencyByFilters(string filter = null)
        {
            return _plan.getFrequencyByFilter(filter);
        }
        [HttpGet]
        public List<string> GetPlanType()
        {
            var plantype = Enum.GetNames(typeof(EnumPlanType)).ToList();

            return plantype;
        }
        [HttpGet]
        public List<string> GetPlanFor()
        {
            var plantype = Enum.GetNames(typeof(EnumPlanForProduction)).ToList();
            return plantype;
        }
        [HttpPost]
        public HttpResponseMessage CreateTempProductionPlanReportTable(filterOption model)
        {
            if (model == null)
                model = new filterOption();
            var result = this._ProductionPlanRepo.CreateTemProductionPlanReportTable(model.ReportFilters);
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
        #region production plan new subin code
        [HttpGet]
        public List<PorductionDetailsModel> GetPorductionDetails(string startDate,string endDate, string customers)
        {
            var result = _ProductionPlanRepo.GetPorductionDetails(startDate,endDate, customers);
            return result;
        }
        #endregion
    }


}
