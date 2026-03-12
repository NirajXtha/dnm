using NeoErp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace NeoErp.sales.Module.Controllers
{
    public class ConsumptionController : Controller
    {
        private IWorkContext _workContext;
        public ConsumptionController(IWorkContext workContext)
        {            
            _workContext = workContext;
        }


        public ActionResult ConsumptionIssueRegister()
        {
            return PartialView();
        }
    }
}
