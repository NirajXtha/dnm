using NeoErp.Core;
using NeoErp.Core.Helpers;
using NeoErp.Core.Services;
using NeoErp.Distribution.Service.DistributorServices;
using NeoErp.Distribution.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoErp.Distribution.Controllers.Api
{
    public class DistributorController : ApiController
    {
        private IDistributorService _service;
        private IWorkContext _workContext;
        private IAuthenticationService _authenticationService;
        private IDashboard _dashboard;
        public DistributorController(IDistributorService service, IWorkContext workContext, IAuthenticationService authenticationService, IDashboard dashboard)
        {
            _service = service;
            _workContext = workContext;
            _authenticationService = authenticationService;
            _dashboard = dashboard;
        }
        [HttpPost]
        public string SavePurchaseOrder(DistPurchaseOrder model)
        {
            var result = _service.SavePurchaseOrder(model,_workContext.CurrentUserinformation);
            return result;
        }
        [HttpGet]
        public List<SalesRegisterProductModel> GetDistributorItems(string DistCode = "")
        {
            var item = _service.GetDistributorItems(_workContext.CurrentUserinformation, DistCode);
            return item;
        }

        [HttpPost]
        public PurchaseOrderReportModel GetDistributorMaxOrderNo()
        {
           var data= _service.GetMaxOrderNoFromDistributor();
            if(data!=null)
            {
                data.CUSTOMER_CODE = _workContext.CurrentUserinformation.DistributerNo;
                data.CUSTOMER_EDESC = _workContext.CurrentUserinformation.LOGIN_EDESC;

            }
            return data;
           
        }
        [HttpPost]
        public List<DistributorTragetChartModel> GetDistributortarget(string DistributorCode="0")
        {
            var data = _dashboard.GetDistributorTraget(_workContext.CurrentUserinformation,DistributorCode);
           
            return data;

        }

        [HttpPost]
        public HttpResponseMessage GetDistributorCollections(filterOption model)
        {
            var data = _service.GetCollections(_workContext.CurrentUserinformation);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpPost]
        public HttpResponseMessage SaveDistCOllection(CollectionModel model)
        {
            var status = _service.SaveCollection(model, _workContext.CurrentUserinformation);
            if (status == "ADDED")
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Collection successfully saved", TYPE = "success" });
            else if (status == "UPDATED")
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Collection successfully Updated", TYPE = "success" });
            else
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something went wrong. Please try again.", TYPE = "error" });
        }

        [HttpGet]
        public HttpResponseMessage GetCollectionDetail(string billNo)
        {
            var result = _service.GetCollectionDetail(billNo);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPost]
        public HttpResponseMessage GetAccounStatement(filterOption model)
        {
            try
            {
                var result = _service.GetAccountStatement(model, _workContext.CurrentUserinformation);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch(Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Something went wrong. Please try again.", TYPE = "error" });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetDivisions()
        {
            var result = _service.GetDivisions(_workContext.CurrentUserinformation);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
        [HttpPost]
        public HttpResponseMessage GetClosingReport(filterOption model)
        {
            var result = _service.GetClosingStock(_workContext.CurrentUserinformation, model);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPost]
        public HttpResponseMessage GetClosingReportNew(filterOption model)
        {
            var result = _service.GetClosingStockNew(_workContext.CurrentUserinformation, model);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        public List<SalesVsTargetModel> GetAllSalesVsTarget(filterOption model)
        {
            var result = _service.GetAllSalesVsTarget(_workContext.CurrentUserinformation, model);
            return result;
        }

        public List<DivisionWiseCreditLimitModel> GetDivisionWiseCreditLimitList(filterOption filter)
        {
            var result = _service.GetDivisionWiseCreditLimitList(_workContext.CurrentUserinformation,filter);
            return result;
        }
        public List<VoucherDetailModel> GetLedgeDetail(filterOption filter)
        {
            var result = _service.GetLedgerDetailBySubCode("101");
            return result;
        }

        public List<SalesVsTargetModel> GetAllSalesTargetVsAchievement(filterOption model)
        {
            var result = _service.GetAllSalesTargetVsAchievement(_workContext.CurrentUserinformation, model);
            return result;
        }


        public List<SalesVsCustomerTargetModel> GetAllSalesTargetVsCustomerAchievement(filterOption model)
        {
            var result = _service.GetAllSalesTargetVsCustomerAchievement(_workContext.CurrentUserinformation, model);
            return result;
        }
        
        public List<SchemeModel> GetSchemeData(filterOption model)
        {
            var result = _service.GetSchemeData(_workContext.CurrentUserinformation, model);
            return result;
        }
    }
}
