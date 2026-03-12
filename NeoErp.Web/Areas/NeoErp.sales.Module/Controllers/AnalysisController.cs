using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;

namespace NeoErp.sales.Module.Controllers
{
    public class AnalysisController : Controller
    {
        // GET: Analysis
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ProductRealizeAreaWise()
        {
            return PartialView();
        }
        public ActionResult ProductRealizationAreaWise()
        {
            return PartialView();
        }
        public ActionResult ProductRealizationSalesManagerAndOfficerWise()
        {
            return PartialView();
        }
        public ActionResult AverageAgingAndPaymentRotationSalesManagerAndOfficerWise()
        {
            return PartialView();
        }
        public ActionResult DealerSalesRange()
        {
            return PartialView();
        }
        public ActionResult QuantityBalanceAndPaymentRotationDay()
        {
            return PartialView();
        }
        public ActionResult QtyBalanceAndPaymentRotationDay()
        {
            return PartialView();
        }
        public ActionResult AreaWiseSalesAndCollection()
        {
            return PartialView();
        }
        #region lc analysis report
        public ActionResult BrandWiseGrossProfit()
        {
            return PartialView();
        }
        public ActionResult BrandWiseSales()
        {
            return PartialView();
        }
        public ActionResult PartyWiseSales()
        {
            return PartialView();
        }
        public ActionResult DebtorAgingPerDay()
        {
            return PartialView();
        }
        public ActionResult DebtorAging()
        {
            return PartialView();
        }
        public ActionResult WeekOfSales()
        {
            return PartialView();
        }
        #endregion
    }
}