using NeoErp.Sales.Modules.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoErp.Controllers.Report_Module
{
    public class SalesReprtController : Controller
    {
        public ISalesRegister _salesRegister { get; set; }
        public SalesReprtController(ISalesRegister salesRegister)
        {
            this._salesRegister = salesRegister;
        }
        // GET: SalesReprt
        public ActionResult Index()
        {
            var test = _salesRegister.SaleRegisters();
            return View();
        }
    }
}