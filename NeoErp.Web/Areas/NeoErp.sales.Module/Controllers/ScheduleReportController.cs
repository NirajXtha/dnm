using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoErp.sales.Module.Controllers
{
    public class ScheduleReportController : Controller
    {
        // GET: ScheduleReport
        public ActionResult ReceiptSchedule()
        {
            return PartialView();
        }



        public ActionResult PaymentSchedule()
        {
            return PartialView();
        }
    }
}