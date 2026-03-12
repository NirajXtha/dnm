using NeoErp.Core;
using NeoErp.Core.Caching;
using NeoErp.Core.Helpers;
using NeoErp.Core.Models;
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
    public class MaterialPlanApiController : ApiController
    {
        IPlan _plan;
        ICOAPlanRepo _COAPlanRepo;
        private IWorkContext _workContext;
        private ICacheManager _cacheManager;
        private IMaterialPlanRepo _MaterialPlanRepo;
        private NeoErpCoreEntity _objectEntity;


        public MaterialPlanApiController(ICOAPlanRepo COAPlanRepo, IMaterialPlanRepo MaterialPlanRepo, ICacheManager cacheManager, IWorkContext workContext, IPlan plan, NeoErpCoreEntity objectEntity)
        {
            this._COAPlanRepo = COAPlanRepo;
            this._MaterialPlanRepo = MaterialPlanRepo;
            this._cacheManager = cacheManager;
            this._plan = plan;
            this._workContext = workContext;
            this._objectEntity = objectEntity;
        }

        public PL_MATERIAL_PLAN GetPlanDetailValueByPlanCode(int plancode)
        {
            PL_MATERIAL_PLAN entity = new PL_MATERIAL_PLAN();
            entity = this._MaterialPlanRepo.GetPlanDetailValueByPlanCode(plancode);
            return entity;
        }

        [HttpPost]
        public HttpResponseMessage DeletePlane(int planCode)
        {
            try
            {
                var deleteStatus = this._MaterialPlanRepo.deleteMaterialPlan(planCode);
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
                var remainingQty = 0M;
                var nextMonth = string.Empty;

                var finalResult = new List<PlanSalesRefrenceModel>();

                var result = this._MaterialPlanRepo.getSalesItemDataForRefrence(ItemList, startDate, endDate, customerCode, divisionCode, branchCode, dateFormat, frequency);
                var stockResult = this._MaterialPlanRepo.getPorcumentItemDataForStock(ItemList, startDate, endDate, customerCode, divisionCode, branchCode, dateFormat, frequency);

                var uncommonList = result.Where(a => !stockResult.Any(b => a.ITEM_CODE == b.ITEM_CODE)).ToList();
                var commonList = result.Where(a => stockResult.Any(b => a.ITEM_CODE == b.ITEM_CODE)).OrderBy(o => o.NEPALI_YEAR).ThenBy(o => o.ITEM_CODE).ThenBy(o => o.NEPALI_MONTH).ToList();

                foreach (var stockItem in stockResult)
                {
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

        public HttpResponseMessage getProductionItemDataForReference(string ItemList, string startDate, string endDate, string branchCode, string dateFormat, string frequency)
        {
            try
            {
                var result = this._MaterialPlanRepo.getProductionItemDataForRefrence(ItemList, startDate, endDate, branchCode, dateFormat, frequency);
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
        public void transferQtyToNextMonth()
        {

        }

        public HttpResponseMessage getPorcumentItemDataForStock(string ItemList, string startDate, string endDate, string customerCode, string divisionCode, string branchCode, string dateFormat, string frequency)
        {
            try
            {
                var result = this._MaterialPlanRepo.getPorcumentItemDataForStock(ItemList, startDate, endDate, customerCode, divisionCode, branchCode, dateFormat, frequency);
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
        public List<MaterialPlan> GetSalesPlanList(string customercode, string employeecode, string divisioncode, string branchcode, string startdate, string enddate)
        {
            List<MaterialPlan> model = new List<MaterialPlan>();
            model = _MaterialPlanRepo.getAllMaterialPlans(customercode, employeecode, divisioncode, branchcode, startdate, enddate);
            return model;
        }
        [HttpPost]
        public List<MaterialPlanModel> GetMaterialPlanDetailByPlanCode(string planCode)
        {
            return this._MaterialPlanRepo.getPlanDetailById(planCode);
        }
        [HttpPost]
        public List<MaterialPlan> GetSalesPlanList(filterOption model)
        {
            return _MaterialPlanRepo.getAllMaterialPlans(model.ReportFilters);

        }
        [HttpPost]
        public List<MaterialPlan> GetGroupMaterialPlanList(filterOption model)
        {
            return _MaterialPlanRepo.getGroupMaterialPlans(model.ReportFilters);

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
        public List<PlanProductTree> GetAllFGProducts()
        {
            var result = _MaterialPlanRepo.getAllFGProducts();
            return result;
        }
        [HttpPost]
        public List<MaterialPlanModel> GetAllRawMaterialByFinishGood(GetMaterialPlanModel model)
        {
            var result = new List<MaterialPlanModel>();
            try
            {
                //result = _MaterialPlanRepo.GetAllRawMaterialByFinishGood(model);
                result = _MaterialPlanRepo.getAllChildItemsForMaterialPlan(model);
            }
            catch (Exception)
            {

                throw;
            }
            return result;
        }

        [HttpGet]
        public List<ProcurementPlan> BindSalesPlanByPlanCode()
        {
            var result = new List<ProcurementPlan>();
            try
            {
                result = _MaterialPlanRepo.BindSalesPlanByPlanCode();
            }
            catch (Exception)
            {

                throw;
            }
            return result;
        }
        [HttpGet]
        public List<ProcurementPlan> BindProductionPlanByPlanCode()
        {
            var result = new List<ProcurementPlan>();
            try
            {
                result = _MaterialPlanRepo.BindProductionPlanByPlanCode();
            }
            catch (Exception)
            {

                throw;
            }
            return result;
        }

        [HttpGet]
        public List<SalesOrderModel> BindSalesOrderCustomer()
        {
            var result = new List<SalesOrderModel>();
            try
            {
                result = _MaterialPlanRepo.BindSalesOrderCustomer();
            }
            catch (Exception)
            {

                throw;
            }
            return result;
        }
        [HttpPost]
        public List<ProcureFromMaterialModel> GetAllSalesPlanItemByPlanCode(string pCode)
        {
            var result = new List<ProcureFromMaterialModel>();
            try
            {
                result = _MaterialPlanRepo.GetAllSalesPlanItemByPlanCode(pCode);
            }
            catch (Exception)
            {

                throw;
            }
            return result;
        }
        [HttpPost]
        public List<ProcureFromMaterialModel> GetAllProductionPlanItemByPlanCode(string pCode)
        {
            var result = new List<ProcureFromMaterialModel>();
            try
            {
                result = _MaterialPlanRepo.GetAllProductionPlanItemByPlanCode(pCode);
            }
            catch (Exception)
            {

                throw;
            }
            return result;
        }
        [HttpPost]
        public List<ProcureFromMaterialModel> GetAllRawMaterialByProductionPlanCode(string pCode)
        {
            var result = new List<ProcureFromMaterialModel>();
            try
            {
                result = _MaterialPlanRepo.GetAllRawMaterialByProductionPlanCode(pCode);
            }
            catch (Exception)
            {

                throw;
            }
            return result;
        }
        [HttpPost]
        public List<ProcureFromMaterialModel> GetAllRawMaterialBySalesPlanCode(string pCode)
        {
            var result = new List<ProcureFromMaterialModel>();
            try
            {
                result = _MaterialPlanRepo.GetAllRawMaterialBySalesPlanCode(pCode);
            }
            catch (Exception)
            {

                throw;
            }
            return result;
        }
        [HttpPost]
        public List<ProcureFromMaterialModel> GetAllRawMaterialBySalesOrderCustomerCode(string pCode, string startDate, string endDate)
        {
            var result = new List<ProcureFromMaterialModel>();
            try
            {
                result = _MaterialPlanRepo.GetAllRawMaterialBySalesOrderCustomerCode(pCode, startDate, endDate);
            }
            catch (Exception)
            {

                throw;
            }
            return result;
        }

        [HttpPost]
        public HttpResponseMessage GeneratePI(MaterialPlanPI model)
        {
            using (var trans = _objectEntity.Database.BeginTransaction())
            {
                try
                {
                    var check = model.checkList.GroupBy(x => x.ITEM_CODE).Select(y => new checklist
                    {
                        ITEM_CODE = y.Key,
                        MU_CODE = y.FirstOrDefault().MU_CODE,
                        QUANTITY = y.Select(z => z.QUANTITY).Sum()
                    });
                    var today = DateTime.Now.ToString("dd-MMM-yyyy");
                    string newVoucherNo = string.Empty;
                    newVoucherNo = _MaterialPlanRepo.NewVoucherNo(_workContext.CurrentUserinformation.company_code, "310", today, "IP_PURCHASE_REQUEST");
                    bool inesertMPPI = _MaterialPlanRepo.InsertMaterialPlanPI(check,model.FROM_DEPARTMENT,model.TO_DEPARTMENT, newVoucherNo);
                    trans.Commit();
                    if (inesertMPPI == true)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "INSERTED", STATUS_CODE = (int)HttpStatusCode.OK,voucherno= newVoucherNo });
                    }
                    else {
                        trans.Rollback();
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                    }
                   
                }
                catch (Exception)
                {

                    trans.Rollback();
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError });

                }
            }
            
            

            //var result = "";
            
        }

    }


}
