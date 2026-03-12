using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoErp.Controllers.JobScheduling
{
    public class SchedulerController : Controller
    {

        // GET: Scheduler
        public ActionResult Index()
        {
            return View();
        }
        public JavaScriptResult Notification()
        {
            string script = "displayBarNotification('pending order');";
            return JavaScript(script);
        }
    }
}