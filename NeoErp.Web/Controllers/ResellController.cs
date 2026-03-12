using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoErp.Controllers
{
    public class ResellController : Controller
    {
        // GET: Resell
        public ActionResult Index()
        {
            return View();
        }
        //public ActionResult ResellerList()
        //{
        //    MobileResellerService service = new MobileResellerService();
        //    string message = string.Empty;
        //    bool status = false;
        //    List<ResellerRegistration> objlst = service.GetResellerRegisteredList(null, out message, out status);
        //    return View(objlst);
        //}

        public ActionResult ResellerList()
        {            
            return View();
        }
    }
}