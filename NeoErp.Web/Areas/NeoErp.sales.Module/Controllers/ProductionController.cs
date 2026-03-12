using NeoErp.Core;
using NeoErp.Sales.Modules.Services.Services.Production;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoErp.sales.Module.Controllers
{
    public class ProductionController : Controller
    {
        public IProductionService _productionService { get; set; }
        private IWorkContext _workContext;
        public ProductionController(IProductionService productionService, IWorkContext workContext)
        {
            this._productionService = productionService;
            this._workContext = workContext;
        }
        public ActionResult ProductionRegister()
        {
            return PartialView();
        }
        public ActionResult ProductionStockInOutSummary()
        {
            return PartialView();
        }
    }
}