using NeoErp.Sales.Modules.Services.Models.AgeingReport;
using NeoErp.Sales.Modules.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace NeoErp.sales.Module.Controllers.Api
{
    public class MobileAgeingController : ApiController
    {
        private IAgeingReportService _ageingReportService;
        public MobileAgeingController(IAgeingReportService ageingReportService)
        {
            this._ageingReportService = ageingReportService;
        }

        [HttpPost]
        public HttpResponseMessage GetAgeingReport([ModelBinder]AgeingFilterModel model, [ModelBinder]List<string> customerCode, string companyCode)
        {
            if (model == null)
            {
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, new { STATUS_CODE = (int)HttpStatusCode.ExpectationFailed, MESSAGE = "Required parameter not supplied" });
            }
            try
            {
                var ageingReport = this._ageingReportService.GetMobileAgeingChartReport(model, customerCode, companyCode).ToList(); 
                if (ageingReport.Count> 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = string.Format("Ageing Report Successfully retrived "), DATA = ageingReport.ToList(), STATUS_CODE = (int)HttpStatusCode.OK });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = string.Format("Ageing Report not Found "), STATUS_CODE = (int)HttpStatusCode.NotFound });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        public HttpResponseMessage testAgeing([ModelBinder]AgeingFilterModel model, [ModelBinder]List<string> customerCode, string companyCode)
        {
            try
            {
                var ageingReport = this._ageingReportService.testAgeingData(model, customerCode, companyCode).ToList();
                if (ageingReport.Count > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = string.Format("Ageing Report Successfully retrived "), DATA = ageingReport.ToList(), STATUS_CODE = (int)HttpStatusCode.OK });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = string.Format("Ageing Report not Found "), STATUS_CODE = (int)HttpStatusCode.NotFound });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
    }
}
