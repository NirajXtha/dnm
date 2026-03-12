using NeoErp.LOC.Services.Interfaces;
using NeoErp.LOC.Services.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoERP.LOC.Controllers
{
    public class CIPaymentSettlementController : Controller
    {

        private ICIPaymentService _service { get; set; }

        public CIPaymentSettlementController(ICIPaymentService service)
        {
            this._service = service;
        }
        // GET: CIPaymentSettlement
        public ActionResult Index()
        {
            return PartialView();
        }

        [HttpPost]
        public JsonResult Upload(IEnumerable<HttpPostedFileBase> inputFiles, FormCollection collection)
        {
            string PSCODE = Convert.ToString(collection["dropzonePSCODE"]);
            string lctrackno = Convert.ToString(collection["doclctrackno"]);

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
                        string ciimages = "CIPaymentSettlement";
                        string strMappath = "~/Pictures/" + LCImages + "/" + ciimages + "/" + lctrackno + "/";
                        string path = System.IO.Path.Combine(
                                               Server.MapPath(strMappath), pic);
                        string ext = Path.GetExtension(path);

                        dbstrMappath = "\\Pictures\\" + LCImages + "\\" + ciimages + "\\" + lctrackno + "\\" + pic;
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

                _service.UpdateImage(lcmodels, PSCODE);
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }

        }
    }
}