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
    public class BrandingPlanApiController : ApiController
    {
        IPlan _plan;
        ICOAPlanRepo _COAPlanRepo;
        private IWorkContext _workContext;
        private ICacheManager _cacheManager;
        private IBrandingPlanRepo _BrandingPlanRepo;

        public BrandingPlanApiController(ICOAPlanRepo COAPlanRepo, IBrandingPlanRepo BrandingPlanRepo,ICacheManager cacheManager, IWorkContext workContext,IPlan plan)
        {
            this._COAPlanRepo = COAPlanRepo;
            this._BrandingPlanRepo= BrandingPlanRepo;
            this._cacheManager = cacheManager;
            this._plan = plan;
            this._workContext = workContext;
        }

        public COAPlanSetupDetailViewModal GetPlanDetailValueByPlanCode(int plancode)
       {
            COAPlanSetupDetailViewModal entity = new COAPlanSetupDetailViewModal();
            entity = this._BrandingPlanRepo.GetBudgetPlanDetailValueByPlanCode(plancode);
            return entity;
        }

        [HttpPost]
        public HttpResponseMessage DeletePlane(int planCode)
        {
            try
            {
                var deleteStatus = this._BrandingPlanRepo.deleteBrandingPlan(planCode);
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

        public HttpResponseMessage getSalesItemDataForReference(string ItemList, string startDate, string endDate, string customerCode, string divisionCode, string branchCode, string dateFormat, string frequency)
        {
            try
            {
                var result = this._BrandingPlanRepo.getSalesItemDataForRefrence(ItemList, startDate, endDate, customerCode, divisionCode, branchCode, dateFormat, frequency);
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

        public HttpResponseMessage getPorcumentItemDataForStock(string ItemList, string startDate, string endDate, string customerCode, string divisionCode, string branchCode, string dateFormat, string frequency)
        {
            try
            {
                var result = this._BrandingPlanRepo.getBrandingItemDataForStock(ItemList, startDate, endDate, customerCode, divisionCode, branchCode, dateFormat, frequency);
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
        public List<BrandingPlan> GetSalesPlanList(string customercode, string employeecode, string divisioncode, string branchcode, string startdate, string enddate)
        {
            List<BrandingPlan> model = new List<BrandingPlan>();
            model = _BrandingPlanRepo.getAllBrandingPlans(customercode, employeecode, divisioncode, branchcode, startdate, enddate);
            return model;
        }

        [HttpPost]
        public List<BrandingPlan> GetSalesPlanList(filterOption model)
        {
            return _BrandingPlanRepo.getAllBrandingPlans(model.ReportFilters);

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
            var plantype = Enum.GetNames(typeof(EnumPlanFor)).ToList();
            return plantype;
        }

        [HttpGet]
        //[CacheOutput(ClientTimeSpan =300,ServerTimeSpan =300)]
        public List<ProductTree> GetAllProductsWithChildItem()
        {
            var result = _BrandingPlanRepo.getAllBrandingActivityWithChildItem();
            var productNodes = new List<ProductTree>();
            var productNode = generateProductTree(result, productNodes, null);
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
                    hasProducts = item.GroupFlag == "Y" ? true : false,
                    Items = item.GroupFlag == "Y" ? generateProductTree(model, productNodesChild, item.MasterItemCode) : null,
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
                    productRegister = this._BrandingPlanRepo.ChartOfAccountList();
                    this._cacheManager.Set("SalesRegisterProduct", productRegister, 1);
                }
            }
            else
                productRegister = this._BrandingPlanRepo.ChartOfAccountList();
            return productRegister;
        }
    }


}
