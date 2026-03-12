using NeoErp.Core;
using NeoErp.Core.Helpers;
using NeoErp.Distribution.Service.Model.BrandingModule;
using NeoErp.Distribution.Service.Service.Branding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoErp.Distribution.Controllers.Api
{
    public class BrandingController : ApiController
    {
        public IBrandingService _brandingService;
        private IWorkContext _workContext;

        public BrandingController(IBrandingService setupService, IWorkContext workContext)
        {
            this._brandingService = setupService;
            this._workContext = workContext;
        }

        [HttpPost]
        public string saveBrandingActivity(ActivityModel model)
        {
            var data = _brandingService.saveBrandingActivity(model, _workContext.CurrentUserinformation);
            return data;
        }

        [HttpPost]
        public string UpdateBrandingActivity(ActivityModel model)
        {
            var data = _brandingService.UpdateBrandingActivity(model, _workContext.CurrentUserinformation);
            return data;
        }


        public List<ActivityModel> getBrandingActivityList()
        {
            try
            {
                var data = _brandingService.getBrandingActivityList(_workContext.CurrentUserinformation);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public string deleteActivity(ActivityModel model)
        {
            var result = _brandingService.deleteActivity(model, _workContext.CurrentUserinformation);
            return result;

        }

        public List<ContractModel> getAllContractList(string type)
        {
            var data = _brandingService.getAllContractList(type, _workContext.CurrentUserinformation);
            return data;
        }
        public List<SetQstModel> GetQuestionList()
        {
            var data = _brandingService.GetQuestionList(_workContext.CurrentUserinformation);
            return data;
        }

        public List<SupplierModel> GetSupplier()
        {
            var data = _brandingService.GetSupplierList(_workContext.CurrentUserinformation);
            return data;
        }

        public List<CustomerModel> GetCustomer()
        {
            var data = _brandingService.GetCustomerList(_workContext.CurrentUserinformation);
            return data;
        }

        public List<AreaModel> GetArea()
        {
            var data = _brandingService.GetAreaList(_workContext.CurrentUserinformation);
            return data;
        }

        public List<ResellerModel> GetBrdReseller()
        {
            var data = _brandingService.GetBrdReseller(_workContext.CurrentUserinformation);
            return data;
        }

        public List<BrdItemModel> GetBrdItem()
        {
            var data = _brandingService.GetBrdItem(_workContext.CurrentUserinformation);
            return data;
        }

        public List<ItemUnitModel> GetItemUnit()
        {
            var data = _brandingService.GetItemUnit(_workContext.CurrentUserinformation);
            return data;
        }

        [HttpPost]
        public string saveContract(ContractModel modal)
        {
            var data = _brandingService.saveContract(modal, _workContext.CurrentUserinformation);
            return data;
        }

        [HttpPost]
        public string deleteContract(ContractModel modal)
        {
            var data = _brandingService.deleteContract(modal, _workContext.CurrentUserinformation);
            return data;
        }
        [HttpPost]
        public string updateContract(ContractModel modal)
        {
            var data = _brandingService.updateContract(modal, _workContext.CurrentUserinformation);
            return data;
        }

        public List<ContractModel> getContractSummary()
        {
            var data = _brandingService.getContractSummary(_workContext.CurrentUserinformation);
            return data;
        }

        public List<BrandTypeModel> GetBrandType()
        {
            var data = _brandingService.GetBrandType(_workContext.CurrentUserinformation);
            return data;
        }

        [HttpPost]
        public HttpResponseMessage SaveContractAns(GeneralSaveModel model)
        {
            var result = _brandingService.SaveContractAnswers(model, _workContext.CurrentUserinformation);
            if (result == "200")
                return Request.CreateResponse(HttpStatusCode.OK, new { TYPE = "success", MESSAGE = "Answers saved successfully" });
            else
                return Request.CreateResponse(HttpStatusCode.OK, new { TYPE = "error", MESSAGE = "Error processing request" });
        }
        [HttpGet]
        public HttpResponseMessage GetContractQuesReport()
        {
            try
            {
                var result = _brandingService.ContractQueReport(_workContext.CurrentUserinformation);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { TYPE = "error", MESSAGE = "Error processing request" });
            }
        }
        [HttpPost]
        public List<SchemeReportModel> GetSchemeList(filterOption dateFilter)
        {
            var data = _brandingService.GetAllSchemeList(dateFilter.ReportFilters,_workContext.CurrentUserinformation);
            return data;
        }

    }
}
