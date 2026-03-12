using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoERP.Planning.Controllers
{
    public class PlanSetupController : Controller
    {
        // GET: PlanSetup
        public ActionResult PlanSetup(string planCode)
        {
            ViewBag.planCode = planCode;
            //var FrequencyTitle = _iPlanSetup.GetFrequencyTitle(planCode);
            return View();
        }
    }
}