using NeoERP.DocumentTemplate.Service.Interface;
using NeoERP.DocumentTemplate.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoERP.DocumentTemplate.Controllers
{
    public class PriceSetupController : Controller
    {
        // GET: PriceSetup
        private IPriceSetup _priceSetup;

        public PriceSetupController(IPriceSetup priceSetup)
        {
            this._priceSetup = priceSetup;
        }

       


        public ActionResult SaveUpdatedCell(SaveModelForPriceSetup saveModel)
        {
            try
            {
                var savedResponse = _priceSetup.SaveUpdatedCell(saveModel);
                return Json(savedResponse, JsonRequestBehavior.AllowGet);
            }catch(Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
    }
}