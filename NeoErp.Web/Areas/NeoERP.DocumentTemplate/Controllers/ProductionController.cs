using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoERP.DocumentTemplate.Controllers
{
    public class ProductionController : Controller
    {
        // GET: Production
        [HttpGet]
        public ActionResult Production()
        {

            return PartialView();
        }
    }
}