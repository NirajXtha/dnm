using NeoErp.Transaction.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoErp.Transaction.Controllers
{
    public class BankController : Controller
    {
        // GET: Bank
        public ActionResult BankSetup()
        {
            return PartialView();
        }
        public ActionResult BankLimitSetup()
        {
            return PartialView();
        }
        public ActionResult BankLimitList()
        {
            return PartialView();
        }
        public ActionResult LoanCategorySetup()
        {
            return PartialView();
        }
    }
}