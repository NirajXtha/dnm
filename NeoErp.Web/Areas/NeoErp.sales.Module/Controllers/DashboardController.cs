using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoErp.sales.Module.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Dashboard
        public ActionResult Index(string id = null)
        {
             return RedirectToAction("Dashboard", "SalesHome", new { id = id });          
        }
    }
}