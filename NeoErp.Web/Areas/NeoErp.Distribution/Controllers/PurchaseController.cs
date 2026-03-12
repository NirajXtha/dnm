using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoErp.Distribution.Controllers
{
    public class PurchaseController : Controller
    {
        // GET: Purchase
        public ActionResult POIndex()
        {            
            return   PartialView();
        }
        

        public ActionResult CancelledSalesOrder()
        {
            return PartialView();
        }


        public ActionResult ApprovedSalesOrder()
        {
            return PartialView();
        }

        public ActionResult DistSalesReturn()
        {
            return PartialView();
        }
        public ActionResult DealerSalesOrder()
        {
            return PartialView();
        }
        public ActionResult VehicleApprovalSetup()
        {
            return PartialView();
        }
    }
}