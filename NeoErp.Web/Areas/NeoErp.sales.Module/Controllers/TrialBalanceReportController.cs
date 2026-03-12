using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoErp.sales.Module.Controllers
{
    public class TrialBalanceReportController : Controller
    {
        // GET: TrialBalance
        public ActionResult Index()
        {
            return  PartialView();
        }
        public ActionResult TreeViewTrialBalance()
        {
            return PartialView();
        }

        public ActionResult TreelistViewTrialBalance()
        {
            return PartialView();
        }
        public ActionResult TreelistViewPlBalance()
        {
            return PartialView();
        }
    }
}