using NeoErp.Core;
using NeoErp.Core.Domain;
using NeoErp.Sales.Modules.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoErp.sales.Module.Controllers
{
    public class PurchaseController : Controller
    {
        private IWorkContext _workContext;
        public IPurchaseService _purchaseRegister { get; set; }
        public PurchaseController(IPurchaseService purchaseRegister,IWorkContext workContext)
        {
            this._purchaseRegister = purchaseRegister;
            this._workContext = workContext;
        }
        // GET: Purchase
        public ActionResult PurchaseVatRegister()
        {
            return PartialView();
        }
        public ActionResult PurchaseRegister()
        {
            return PartialView();
        }
        public ActionResult PurchaseItemsSummary()
        {
            User userinfo = this._workContext.CurrentUserinformation;
            var chargeTitle = _purchaseRegister.GetChargesTitle(userinfo);
            return PartialView(chargeTitle);

        }
        public ActionResult PurchaseInvoiceSummary()
        {
            User userinfo = this._workContext.CurrentUserinformation;
            var chargeTitle = _purchaseRegister.GetChargesTitle(userinfo);
            return PartialView(chargeTitle);

        }
        public ActionResult purchaseRegisterPrivot()
        {
            return PartialView();
        }

        public ActionResult ItemSetup()
        {
            return View();
        }
    }
}