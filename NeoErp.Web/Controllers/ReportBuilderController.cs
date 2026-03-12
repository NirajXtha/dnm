using NeoErp.Core.Helpers;
using NeoErp.Models.ReportBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NeoErp.Core.Helpers;

namespace NeoErp.Controllers
{
    public class ReportBuilderController : Controller
    {
        // GET: FormBuilder
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ReportPreview(ReportBuilderModel model)
        {
            if (!string.IsNullOrEmpty(model.Query))
            {
                model.Query =  model.Query.ReplaceHtmlTag();
            }
            
            return PartialView(model);
        }

        public ActionResult ReportIndex()
        {
            return View();
        }
    }
}