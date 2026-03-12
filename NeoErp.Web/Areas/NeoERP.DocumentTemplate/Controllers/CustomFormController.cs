using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoERP.DocumentTemplate.Controllers
{
    public class CustomFormController : Controller
    {
        // GET: CustomForm
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PostDatedCheque() 
        {
            return View();
        }

        public ActionResult ColumnSettingsCshtml()
        {
            return View();
        }

        public ActionResult CashBankSetupHtml()
        {
            return View();
        }

        public ActionResult BankReconcilationHtml()
        {
            return View();
        }

        public ActionResult BankGurantee()
        {
            return View();
        }
    }
}