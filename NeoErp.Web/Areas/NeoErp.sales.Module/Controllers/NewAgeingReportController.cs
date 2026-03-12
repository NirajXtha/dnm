using NeoErp.Core.Models;
using NeoErp.Sales.Modules.Services.Models;
using NeoErp.Sales.Modules.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoErp.sales.Module.Controllers
{
    public class NewAgeingReportController : Controller
    {
        private INewAgeingReport _ageingReportService;
        private NeoErpCoreEntity _context;

        public NewAgeingReportController()
        {
            _ageingReportService = new NewAgeingReport();
            _context = new NeoErpCoreEntity();
        }

        public NewAgeingReportController(INewAgeingReport ageingReportService, NeoErpCoreEntity context)
        {
            _ageingReportService = ageingReportService;
            _context = context;
        }

        // GET: NewAgeingReport
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult AgeingTransactions(TransactionRequestModel model)
        {
            try
            {
                var result = _ageingReportService.ageingTransactions(model, _context);
                return Json(new { success = true, data = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
