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
    public class LogisticsController : Controller
    {
        private ILogisticsService _logisticService;
        public LogisticsController(ILogisticsService logisticService)
        {
            this._logisticService = logisticService;
        }
        // GET: Logistics
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Logistics()
        {
            return View();
        }
        [HttpPost]
        public JsonResult UploadImages(IEnumerable<HttpPostedFileBase> inputFiles, FormCollection collection)
        {
            int SerialNo = Convert.ToInt32(collection["SerialNo"]);
            string lctrackno = Convert.ToString(collection["lctrackno"]);
            string cinumber = Convert.ToString(collection["cinumber"]);
            string fName = "";
            string dbstrMappath = "";
                      
            FileUploadModels lcmodels = new FileUploadModels();
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
                        string Logistics = "Logistics";
                        string strMappath = "~/Pictures/" + LCImages + "/" + Logistics + "/" + lctrackno + "/" + cinumber + "/";
                        string path = System.IO.Path.Combine(
                                               Server.MapPath(strMappath), pic);
                        string ext = Path.GetExtension(path);

                        dbstrMappath = "\\Pictures\\" + LCImages + "\\" + Logistics + "\\" + lctrackno + "\\" + cinumber + "\\" + pic;
                        if (!Directory.Exists(strMappath))
                        {
                            Directory.CreateDirectory(Server.MapPath(strMappath));
                        }
                        file.SaveAs(path);
                        _logisticService.UploadFiles(dbstrMappath, SerialNo, lctrackno, cinumber);
                        SerialNo++;
                    }
                   
                }
               
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }

        }
    }
}