using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoErp.sales.Module.Controllers
{
    public class SalesSummaryReportController : Controller
    {
        // GET: SalesSummaryReport
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PartyWiseGPAnalysisSalesSummary() {
            return PartialView("~/Areas/NeoErp.sales.Module/Views/AnalysisReport/ProductWiseProfitAnalysisReport.cshtml");
        }



        public ActionResult GoodsReceiptNotes()
        {
            return PartialView();
        }


        public ActionResult SalesAnalysis()
        {
            return PartialView();
        }

        public ActionResult TopSalesEmployee()
        {
            return PartialView();
        }

        public ActionResult TopSalesDealer()
        {
            return PartialView();
        }

        public ActionResult RegionProductSales()
        {
            return PartialView();
        }
        //public ActionResult SlowMovingItemReport()
        //{
        //    ViewBag.ReportType = "slowmoving";
        //    return View();
        //}
        
    }
}