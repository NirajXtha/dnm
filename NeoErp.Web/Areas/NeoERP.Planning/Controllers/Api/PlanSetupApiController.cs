using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using NeoErp.Planning.Service.Interface;
using NeoErp.Planning.Service.Models;
using NeoERP.Planning.Models;
using System.Net;
using WebApi.OutputCache.V2;
using NeoErp.Core.Helpers;

namespace NeoERP.Planning.Controllers.Api
{
    public class PlanSetupApiController : ApiController
    {
        public IPlanSetup _iplanSetup { get; set; }
        public PlanSetupApiController(IPlanSetup iplanSetup)
        {
            this._iplanSetup = iplanSetup;
        }

        public List<ItemModel> getItemByFilter(string filter)
        {
            return _iplanSetup.getItemByFilter(filter);
        }

        public List<string> getPlanType()
        {
            return Enum.GetNames(typeof(EnumPlanType)).ToList();
        }
        public List<PlanModels> GetAllPlanNames(string filter, string startDate, string endDate)
        {
            return _iplanSetup.getPlanList(filter,startDate, endDate);
        }
        public List<MasterSalesPlan> GetAllMasterPlanNames(string filter,string startDate, string endDate)
        {
            return _iplanSetup.getAllMasterPlanNames(filter,startDate, endDate);
        }
        public List<freqNameVlaue> getFrequencyType(string filter)
        {
            var result = _iplanSetup.getFrequency(filter);
            return result;
        }
        [HttpPost]
        public List<PlanSetupTitleModel> getTitleValues(string planCode)
        {
            var result = _iplanSetup.getTitleValues(planCode);
            return result;
        }
        [HttpPost]
        public HttpResponseMessage SaveEmployeeHandover(EmployeeHandoverModel model)
        {
            try
            {
                var result = this._iplanSetup.SaveEmployeeHandover(model);
                if (result!="")
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }

        }
        public HttpResponseMessage getSalesItemDataForReferences(SalesItemForReferenceModel model)
        {
            try
            {
                var result = this._iplanSetup.getSalesItemDataForRefrences(model);
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
        public HttpResponseMessage getSalesItemDataForReference(string ItemList, string startDate, string endDate, string customerCode, string divisionCode, string branchCode, string dateFormat, string frequency,string FiscalYear,string salesFlag)
        {
            try
            {
                var result = this._iplanSetup.getSalesItemDataForRefrence(ItemList, startDate, endDate, customerCode, divisionCode, branchCode, dateFormat, frequency,FiscalYear,salesFlag);
                if (result.Count()>0)
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

        [CacheOutput(ClientTimeSpan =300,ServerTimeSpan =300)]
        public List<ItemModel> getItems()
        {
            var result = _iplanSetup.getItem();
            return result;
        }

        public List<ItemModel> getItemByCode(string planCode, string itemCode)
        {
            var result = _iplanSetup.getItemByCode(planCode, itemCode);
            var itemNodes = new List<ItemModel>();
            foreach (var item in result)
            {
                itemNodes.Add(new ItemModel()
                {
                    Level = item.Level,
                    ITEM_EDESC = item.ITEM_EDESC,
                    ITEM_CODE = item.ITEM_CODE,
                    GROUP_SKU_FLAG = item.GROUP_SKU_FLAG,
                    MASTER_ITEM_CODE = item.MASTER_ITEM_CODE,
                    PRE_ITEM_CODE = item.PRE_ITEM_CODE,
                    IS_CHILD_SELECTED = item.IS_CHILD_SELECTED,
                    hasChildren = item.GROUP_SKU_FLAG == "G" ? true : false
                });
            }
            return itemNodes;
        }

        public List<PlanModels> GetPlanDetailValue(string plancode,string itemcode)
        {
            List<PlanModels> list = this._iplanSetup.GetPlanDetailVale(plancode);
            return list;
        }

        public SalesPlan GetPlanDetailValueByPlanCode(int plancode)
        {
            SalesPlan salesPlanList = new SalesPlan();
            salesPlanList = this._iplanSetup.GetPlanDetailValueByPlanCode(plancode);
            return salesPlanList;
        }
        public SalesPlan GetSalesPlanDetailValueByPlanCode(int plancode)
        {
            SalesPlan salesPlanList = new SalesPlan();
            salesPlanList = this._iplanSetup.GetSalesPlanDetailValueByPlanCode(plancode);
            return salesPlanList;
        }
        public SalesPlan GetMasterPlanDetailValueByMasterPlanCode(int mp_code)
        {
            var list = new SalesPlan();
            list = this._iplanSetup.GetMasterPlanDetailValueByMasterPlanCode(mp_code);
            return list;
        }

        [HttpGet]
        public IHttpActionResult GetItemGoupEntryPreferenceSetup()
        {
            try
            {
                LcPreferenceSetup setup = new LcPreferenceSetup();
                setup = this._iplanSetup.GetItemGoupEntryPreferenceSetup();
                return Ok(setup);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            
        }
        [HttpGet]
        public List<ProductSetupModel> GetItemPriceFromPlanCode(int plancode)
        {
            List<ProductSetupModel> list = this._iplanSetup.GetProductRateByPlanCode(plancode);
            return list;
        }
        [HttpPost]
        public HttpResponseMessage CreateTempSalesPlanReportTable(filterOption model)
        {
            if (model == null)
                model = new filterOption();
            var result = this._iplanSetup.CreateTempSalesPlanReportTable(model.ReportFilters);
            //return result;
            if (result == "TableDropedAndCreated")
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "DELETECREATED", STATUS_CODE = (int)HttpStatusCode.OK });
            }
            else if (result == "TableCreated")
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "CREATED", STATUS_CODE = (int)HttpStatusCode.OK });
            }
            else {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE ="Fail", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

    }
}
