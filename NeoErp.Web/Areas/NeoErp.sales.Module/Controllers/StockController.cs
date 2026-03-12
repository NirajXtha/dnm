using NeoErp.Core;
using NeoErp.Core.Helpers;
using NeoErp.Sales.Modules.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoErp.sales.Module.Controllers
{
    public class StockController : Controller
    {
       
        public IStockService _stockService { get; set; }
        private IWorkContext _workContext;
        public StockController(IStockService stockService,IWorkContext workContext)
        {
            _stockService = stockService;
            _workContext = workContext;

        }

        public ActionResult LocationWiseStockReport()
        {
            return PartialView();
        }
        public ActionResult LocationVsBranchWiseStockReport()
        {
            filterOption model = new filterOption();
            var locationHeader = _stockService.GetLocationHeader(model.ReportFilters,_workContext.CurrentUserinformation).ToList();
            return PartialView(locationHeader);
        }
    }
}