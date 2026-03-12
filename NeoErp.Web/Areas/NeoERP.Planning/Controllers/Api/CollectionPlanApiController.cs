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
    public class CollectionPlanApiController : ApiController
    {
        IPlan _plan;
        ICOAPlanRepo _COAPlanRepo;
        private IWorkContext _workContext;
        private ICacheManager _cacheManager;
        private ICollectionPlanRepo _CollectionPlanRepo;
        public CollectionPlanApiController(ICOAPlanRepo COAPlanRepo, ICollectionPlanRepo CollectionPlanRepo, ICacheManager cacheManager, IWorkContext workContext, IPlan plan)
        {
            this._COAPlanRepo = COAPlanRepo;
            this._CollectionPlanRepo = CollectionPlanRepo;
            this._cacheManager = cacheManager;
            this._plan = plan;
            this._workContext = workContext;
        }
        public PL_BRD_CLTN_PLAN GetPlanDetailValueByPlanCode(int plancode)
        {
            PL_BRD_CLTN_PLAN entity = new PL_BRD_CLTN_PLAN();
            entity = this._CollectionPlanRepo.GetPlanDetailValueByPlanCode(plancode);
            return entity;
        }

        [HttpPost]
        public HttpResponseMessage DeletePlane(int planCode)
        {
            try
            {
                var deleteStatus = this._CollectionPlanRepo.deleteCollectionPlan(planCode);
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

        public HttpResponseMessage getSalesItemDataForReference(string ItemList, string startDate, string endDate, string customerCode, string divisionCode, string branchCode, string employeeCode, string dateFormat, string frequency, string MATERIAL_PLAN_CODE = "0", string REFERENCE_FLAG = "")
        {
            try
            {

                var nextMonth = string.Empty;

                var finalResult = new List<PlanSalesRefrenceModel>();
                var result = new List<PlanSalesRefrenceModel>();
                result = this._CollectionPlanRepo.getSalesItemDataForRefrence(ItemList, startDate, endDate, customerCode, divisionCode, branchCode, employeeCode, dateFormat, frequency, MATERIAL_PLAN_CODE, REFERENCE_FLAG);
                var stockResult = this._CollectionPlanRepo.getCollectionItemDataForStock(ItemList, startDate, endDate, customerCode, divisionCode, branchCode, dateFormat, frequency);

                var uncommonList = result.Where(a => !stockResult.Any(b => a.ITEM_CODE == b.ITEM_CODE)).ToList();
                var commonList = result.Where(a => stockResult.Any(b => a.ITEM_CODE == b.ITEM_CODE)).OrderBy(o => o.NEPALI_YEAR).ThenBy(o => o.ITEM_CODE).ThenBy(o => o.NEPALI_MONTH).ToList();

                foreach (var stockItem in stockResult)
                {
                    var remainingQty = 0M;
                    var getSingleItemList = commonList.Where(a => a.ITEM_CODE == stockItem.ITEM_CODE).OrderBy(o => o.NEPALI_YEAR).ThenBy(o => o.NEPALI_MONTH).ToList();
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
                            else
                            {
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

        public void transferQtyToNextMonth()
        {

        }

        public HttpResponseMessage getCollectionItemDataForStock(string ItemList, string startDate, string endDate, string customerCode, string divisionCode, string branchCode, string dateFormat, string frequency)
        {
            try
            {
                var result = this._CollectionPlanRepo.getCollectionItemDataForStock(ItemList, startDate, endDate, customerCode, divisionCode, branchCode, dateFormat, frequency);
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
        public List<CollectionPlan> GetSalesPlanList(string customercode, string employeecode, string divisioncode, string branchcode, string startdate, string enddate)
        {
            List<CollectionPlan> model = new List<CollectionPlan>();
            model = _CollectionPlanRepo.getAllCollectionPlans(customercode, employeecode, divisioncode, branchcode, startdate, enddate);
            return model;
        }

        [HttpPost]
        public List<CollectionPlan> GetSalesPlanList(filterOption model)
        {
            return _CollectionPlanRepo.getAllCollectionPlans(model.ReportFilters);

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
            var plantype = Enum.GetNames(typeof(EnumPlanForCollection)).ToList();
            return plantype;
        }
        [HttpPost]
        public HttpResponseMessage CreateCollectionPlanReportTable(filterOption model)
        {
            if (model == null)
                model = new filterOption();
            var result = this._CollectionPlanRepo.CreateTemCollectionPlanReportTable(model.ReportFilters);
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
        [HttpPost]
        public List<ItemModel> getItemByCode(string planCode)
        {
            return _CollectionPlanRepo.getItemByPlanCode(planCode);
        }
        [HttpGet]
        public List<MaterialPlanModel> GetMaterialPlanList()
        {
            try
            {
                var result = _CollectionPlanRepo.GetMaterialPlanList();
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        [HttpPost]
        public List<CollectionFromMaterialModel> GetAllRawMaterialByMaterialPlanCode(string pCode)
        {
            var result = new List<CollectionFromMaterialModel>();
            try
            {
                result = _CollectionPlanRepo.GetAllRawMaterialByMaterialCode(pCode);
            }
            catch (Exception)
            {

                throw;
            }
            return result;
        }
        [HttpGet]
        public List<ProductTree> GetAllProductsWithChildItem(string pageNameId)
        {
            var userinfo = this._workContext.CurrentUserinformation;
            ////var allProductList = _salesRegister.ProductListAllNodes().ToList();
            //var allProductList = _CollectionPlanRepo.ProductListAllNodes(userinfo).ToList();
            //var productNodes = new List<ProductTree>();

            //foreach (var prod in allProductList)
            //{
            //    productNodes.Add(new ProductTree()
            //    {
            //        Level = prod.LEVEL,
            //        ItemName = prod.ItemName,
            //        ItemCode = prod.ItemCode,
            //        MasterItemCode = prod.MasterItemCode,
            //        PreItemCode = prod.PreItemCode,
            //        hasProducts = prod.GroupFlag == "G" ? true : false
            //    });
            //}

            //return productNodes;
            var result = _CollectionPlanRepo.ProductListAllNodes(userinfo).ToList();
            var productNodes = new List<ProductTree>();
            var productNode = generateProductTree(result, productNodes, "00");
            return productNode;
        }
        [HttpGet]
        public List<ProductTree> GetAllProductsByProdId(string prodId, string level, string masterCode)
        {
            var userinfo = this._workContext.CurrentUserinformation;
            //var allProductList = _salesRegister.GetProductsListByProductCode(level, masterCode).ToList();
            var allProductList = _CollectionPlanRepo.GetProductsListByProductCode(level, masterCode, userinfo).ToList();
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
                    Rate = item.Rate,
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
        public List<PlanRegisterProductModel> GetSalesRegisterProducts(filterOption filter, string pageNameId, bool individual = false)
        {
            var productRegister = new List<PlanRegisterProductModel>();
            productRegister=_CollectionPlanRepo.PlanRegisterProducts(_workContext.CurrentUserinformation).ToList();
            return productRegister;
        }
        #region Collection Plan Report

        [HttpPost]
        public List<PlanReportModel> GetCollectionPlanMonthlyReport(filterOption Model)
        {
            return _CollectionPlanRepo.getMonthlyCollectionPlanChart(Model.ReportFilters);
        }
        [HttpPost]
        public List<PlanReportModel> GetCollectionPlanEmployeeWiseReport(filterOption Model)
        {
            return _CollectionPlanRepo.getEmployeeWiseCollectionPlanChart(Model.ReportFilters);
        }
        [HttpPost]
        public List<PlanReportModel> GetCollectionPlanEmployeeCustomerWiseReport(filterOption Model,string EmpCode)
        {
            return _CollectionPlanRepo.getEmployeeCustomerWiseCollectionPlanChart(Model.ReportFilters,EmpCode);
        }
        [HttpPost]
        public List<CollectionPlanTreeReportModel> TreewiseCustomerCollectionPlanReport(filterOption model)
        {
            var treeWiseCollectionReport = new List<CollectionPlanTreeReportModel>();
            var cachekey = $"treewiseProductCollectionPlan-{model.ReportFilters.FromDate}-{model.ReportFilters.ToDate}";
            if (this._cacheManager.IsSet(cachekey))
            {
                treeWiseCollectionReport = this._cacheManager.Get<List<CollectionPlanTreeReportModel>>(cachekey);
            }
            else
            {
                treeWiseCollectionReport = _CollectionPlanRepo.GetTreewiseCustomerCollectionPlanReport(model.ReportFilters, _workContext.CurrentUserinformation);
                this._cacheManager.Set(cachekey, treeWiseCollectionReport, 10);
            }
            return treeWiseCollectionReport;
        }
        #endregion
    }
}