using NeoErp.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NeoErp.Models.SalesTarget;

namespace NeoErp.Controllers
{
    public class SalesTargetController : Controller
    {
        private IMenuModel _menuService;
        public SalesTargetController(IMenuModel menuService)
        {
          
            this._menuService = menuService;

        }
        public ActionResult Index()
        {
            ViewData["ModuleMenu"] = _menuService.GetModule();
            return View();
        }

       
        public string GenerateSchedule(string Tname, string TType, string BasedOn, string Frequency, string FromDate, string ToDate)
        {

            string Scheduler = SalesTarget.GenerateTargetSetup(Tname,TType,BasedOn,Frequency,FromDate,ToDate);
            return Scheduler;
        }

        

    }
}
