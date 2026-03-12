using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoERP.LOC.Controllers
{
    public class GRNController : Controller
    {
        // GET: GRN
        public ActionResult Index()
        {
            return PartialView();
        }
    }
}