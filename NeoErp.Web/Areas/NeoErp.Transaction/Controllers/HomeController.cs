using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoErp.Transaction.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ConsumptionIssue()
        {
            return PartialView();
        }
        public ActionResult LocationIssue()
        {
            return View();
        }
        public ActionResult Dashboard()
        {
            return PartialView();
        }
    }
}