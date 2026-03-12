using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoErp.Controllers
{
    public class NotificationBuilderController : Controller
    {
        // GET: NotificationBuilder
        public ActionResult BuildNotification()
        {
            return View();
        }
        public ActionResult NotificationList()
        {
            return View();
        }
    }
}