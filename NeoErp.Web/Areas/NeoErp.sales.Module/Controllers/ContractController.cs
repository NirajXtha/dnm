using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoErp.sales.Module.Controllers
{
    public class ContractController : Controller
    {
        // GET: Contract
        public ActionResult ContactIncomeForeCast()
        {
            return  PartialView();
        }
    }
}