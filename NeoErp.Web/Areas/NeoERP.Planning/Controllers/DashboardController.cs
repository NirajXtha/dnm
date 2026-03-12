using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoERP.Planning.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Dashboard
        public ActionResult Index(string id = null)
        {
            return RedirectToAction("DashboardChart", "Home", new { id = id });
        }
        public ActionResult GroupDashboard()
        {
            return View();
        }
    }
}