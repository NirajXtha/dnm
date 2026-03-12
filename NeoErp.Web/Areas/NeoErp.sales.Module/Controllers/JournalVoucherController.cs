using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoErp.sales.Module.Controllers
{
    public class JournalVoucherController : Controller
    {
        // GET: JournalVoucher
        public ActionResult Daybook()
        {
            return PartialView();
        }
        public ActionResult DaySubledgerbook()
        {
            return PartialView();
        }
    }
}