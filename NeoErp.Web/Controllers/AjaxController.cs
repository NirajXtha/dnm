using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NeoErp.Models;

namespace NeoErp.Controllers
{
    public class AjaxController : Controller
    {
        //
        // GET: /Ajax/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        private IEnumerable<NeoErp.Models.Common.FindNameModel> GetData(string str)
        {
            IEnumerable<NeoErp.Models.Common.FindNameModel> data = NeoErp.Models.Common.FindNameModel.GetSearchList(str);
            return data;
        }
        [HttpPost]
        public JsonResult GetAutoCompleteSearchList(FormCollection collection)
        {
            IEnumerable<NeoErp.Models.Common.FindNameModel> data = GetData(collection[0].ToString());            
            return Json(data, JsonRequestBehavior.AllowGet);           
        }

    }
}
