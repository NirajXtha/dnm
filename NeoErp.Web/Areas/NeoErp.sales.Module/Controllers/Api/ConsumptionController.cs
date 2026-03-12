using NeoErp.Core;
using NeoErp.Core.Helpers;
using NeoErp.Sales.Modules.Services.Models;
using NeoErp.Sales.Modules.Services.Services.Consumption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoErp.sales.Module.Controllers.Api
{
    public class ConsumptionController : ApiController
    {
        
        private IWorkContext _workContext;
        public IConsumptionService _consumptionService { get; set; }

        public ConsumptionController(IWorkContext workContext, IConsumptionService consumptionService)
        {           
            this._workContext = workContext;
            this._consumptionService = consumptionService;
        }



        [HttpPost]
        public ConsumptionIssueRegisterModel ConsumptionIssueRegister(filterOption model, bool liveData = false)
        {
            var reportData = new ConsumptionIssueRegisterModel();
            var data = this._consumptionService.GetConsumptionIssueRegister(model.ReportFilters, _workContext.CurrentUserinformation, liveData);
            //reportData.ConsumptionIssueRegisterDetailModel = data.Skip((model.page - 1) * model.pageSize).Take(model.pageSize).ToList();
            reportData.ConsumptionIssueRegisterDetailModel = data.ToList();
            reportData.total = data.Count();
            return reportData;
        }
    }
}
