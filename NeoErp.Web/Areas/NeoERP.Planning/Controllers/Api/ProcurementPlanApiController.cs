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
    public class ProcurementPlanApiController : ApiController
    {
        IPlan _plan;
        ICOAPlanRepo _COAPlanRepo;
        private IWorkContext _workContext;
        private ICacheManager _cacheManager;
        private IProcurementPlanRepo _ProcurementPlanRepo;

        public ProcurementPlanApiController(ICOAPlanRepo COAPlanRepo, IProcurementPlanRepo ProcurementPlanRepo, ICacheManager cacheManager, IWorkContext workContext, IPlan plan)
        {
            this._COAPlanRepo = COAPlanRepo;
            this._ProcurementPlanRepo = ProcurementPlanRepo;
            this._cacheManager = cacheManager;
            this._plan = plan;
            this._workContext = workContext;
        }

        public PL_BRD_PRCMT_PLAN GetPlanDetailValueByPlanCode(int plancode)
        {
            PL_BRD_PRCMT_PLAN entity = new PL_BRD_PRCMT_PLAN();
            entity = this._ProcurementPlanRepo.GetPlanDetailValueByPlanCode(plancode);
            return entity;
        }

        [HttpPost]
        public HttpResponseMessage DeletePlane(int planCode)
        {
            try
            {
                var deleteStatus = this._ProcurementPlanRepo.deleteProcurementPlan(planCode);
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

        public HttpResponseMessage getSalesItemDataForReference(string ItemList, string startDate, string endDate, string customerCode, string divisionCode, string branchCode, string employeeCode, string dateFormat, string frequency, string MATERIAL_PLAN_CODE = "0",string REFERENCE_FLAG = "")
        {
            try
            {
                
                var nextMonth = string.Empty;

                var finalResult = new List<PlanSalesRefrenceModel>();
                var result = new List<PlanSalesRefrenceModel>();
                result = this._ProcurementPlanRepo.getSalesItemDataForRefrence(ItemList, startDate, endDate, customerCode, divisionCode, branchCode, employeeCode, dateFormat, frequency, MATERIAL_PLAN_CODE, REFERENCE_FLAG);
                var stockResult = this._ProcurementPlanRepo.getPorcumentItemDataForStock(ItemList, startDate, endDate, customerCode, divisionCode, branchCode, dateFormat, frequency);

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

        public HttpResponseMessage getPorcumentItemDataForStock(string ItemList, string startDate, string endDate, string customerCode, string divisionCode, string branchCode, string dateFormat, string frequency)
        {
            try
            {
                var result = this._ProcurementPlanRepo.getPorcumentItemDataForStock(ItemList, startDate, endDate, customerCode, divisionCode, branchCode, dateFormat, frequency);
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
        public List<ProcurementPlan> GetSalesPlanList(string customercode, string employeecode, string divisioncode, string branchcode, string startdate, string enddate)
        {
            List<ProcurementPlan> model = new List<ProcurementPlan>();
            model = _ProcurementPlanRepo.getAllProcurementPlans(customercode, employeecode, divisioncode, branchcode, startdate, enddate);
            return model;
        }

        [HttpPost]
        public List<ProcurementPlan> GetSalesPlanList(filterOption model)
        {
            return _ProcurementPlanRepo.getAllProcurementPlans(model.ReportFilters);

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
            var plantype = Enum.GetNames(typeof(EnumPlanForProcurement)).ToList();
            return plantype;
        }
        [HttpPost]
        public HttpResponseMessage CreateTempProcumentPlanReportTable(filterOption model)
        {
            if (model == null)
                model = new filterOption();
            var result = this._ProcurementPlanRepo.CreateTemProcurementPlanReportTable(model.ReportFilters);
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
            return _ProcurementPlanRepo.getItemByPlanCode(planCode);
        }
        [HttpGet]
        public List<MaterialPlanModel> GetMaterialPlanList()
        {
            try
            {
                var result = _ProcurementPlanRepo.GetMaterialPlanList();
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        [HttpPost]
        public List<ProcureFromMaterialModel> GetAllRawMaterialByMaterialPlanCode(string pCode)
        {
            var result = new List<ProcureFromMaterialModel>();
            try
            {
                result = _ProcurementPlanRepo.GetAllRawMaterialByMaterialCode(pCode);
            }
            catch (Exception)
            {

                throw;
            }
            return result;
        }

    }


}
