using NeoErp.LOC.Services.Models;
using NeoErp.LOC.Services.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoERP.LOC.Controllers
{
    public class PurchaseOrderController : Controller
    {
        // GET: PurchaseOrder
        private IPurchaseOrder _purchaseorder { get; set; }
        public PurchaseOrderController(IPurchaseOrder purchaseorder)
        {
            _purchaseorder = purchaseorder;
        }
        public ActionResult Index()
        {
            return PartialView();
        }

        public ActionResult PurchaseOrder()
        {
            return PartialView();
        }

        [HttpPost]
        public JsonResult PurchaseOrderUpload(IEnumerable<HttpPostedFileBase> inputFiles, FormCollection collection)
        {
            string purchaseorder = Convert.ToString(collection["purchaseorder"]);
            string lctrackno = Convert.ToString(collection["lctrackno"]);

            string fName = "";
            string dbstrMappath = "";
            string finalpath = "";
            int count = 0;
            LcImageModels lcmodels = new LcImageModels();
            if (inputFiles != null)
            {
                foreach (HttpPostedFileBase fileName in inputFiles)
                {
                    HttpPostedFileBase file = fileName;
                    //Save file content goes here
                    fName = file.FileName;
                    if (file != null && file.ContentLength > 0)
                    {
                        string picture = System.IO.Path.GetFileName(file.FileName);
                        string underscore = "_";
                        string pic = string.Format("{0}{1}", lctrackno, underscore) + picture.PadLeft(0, '0');
                        string LCImages = "LCImages";
                        string PurchaseOrderImages = "PurchaseOrder";
                        string strMappath = "~/Pictures/" + LCImages + "/" + PurchaseOrderImages + "/" + lctrackno + "/";
                        string path = System.IO.Path.Combine(
                                               Server.MapPath(strMappath), pic);
                        string ext = Path.GetExtension(path);

                        dbstrMappath = "\\Pictures\\" + LCImages + "\\" + PurchaseOrderImages + "\\" + lctrackno + "\\" + pic;
                        if (!Directory.Exists(strMappath))
                        {
                            Directory.CreateDirectory(Server.MapPath(strMappath));
                        }

                        file.SaveAs(path);

                    }

                    lcmodels.LocCode = Convert.ToInt32(lctrackno);
                    count++;
                    if (count > 1)
                    {
                        finalpath = finalpath + ":" + dbstrMappath;
                    }
                    else
                    {
                        finalpath = dbstrMappath;
                    }

                }
                lcmodels.Path = finalpath;
                _purchaseorder.UpdateImage(lcmodels, purchaseorder);
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }

        }
    }
}