using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoERP.LOC.Controllers
{
    public class LcReportController : Controller
    {
        // GET: LcReport
        [HttpGet]
        public ActionResult DueInvoiceReport()
        {
            return PartialView();
        }

        [HttpGet]
        public ActionResult PendingLcReport()
        {
            return PartialView();
        }

        [HttpGet]
        public ActionResult OpenLcReport()
        {
            return PartialView();
        }

        [HttpGet]
        public ActionResult LcStatusReport()
        {
            return PartialView();
        }

        [HttpGet]
        public ActionResult VMovReport()
        {
            return PartialView();
        }
        [HttpGet]
        public ActionResult URVMovReport()
        {
            return PartialView();
        }
        
        [HttpGet]
        public ActionResult LcProductWiseReport()
        {
            return PartialView();
        }

        [HttpGet]
        public ActionResult PendingCommercialInvoiceReport()
        {
            return PartialView();
        }
        [HttpGet]
        public ActionResult PoPendingReport()
        {
            return PartialView();
        }
        [HttpGet]
        public ActionResult PendingCIReport()
        {
            return PartialView();
        }

        [HttpGet]
        public ActionResult LcPendingReport()
        {
            return PartialView();
        }
        [HttpGet]
        public ActionResult ExchangeGainLossReport()
        {
            return PartialView();
        }
        [HttpGet]
        public ActionResult MITReport()
        {
            return PartialView();
        }
    }
}