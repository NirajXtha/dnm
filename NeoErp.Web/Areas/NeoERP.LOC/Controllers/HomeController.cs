using NeoErp.Data;
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
    public class HomeController : Controller
    {
        private ILcEntryService _lcentryservice { get; set; }
        private IDashBoardMetricService _dashboardentryservice { get; set; }
        private IDbContext _objectEntity;
        public HomeController(ILcEntryService lcentryservice, IDashBoardMetricService dashboardentryservice, IDbContext objectEntity)
        {
            this._lcentryservice = lcentryservice;
            _dashboardentryservice = dashboardentryservice;
            _objectEntity = objectEntity;
        }
        public ActionResult DashBoard()
        {
            return View();
        }

        public ActionResult Preference()
        {
            return View();
        }

        

        public ActionResult Index()
        {
            var widgets = _dashboardentryservice.GetMetricList().Where(x => x.IsActive.Trim() == "Y").OrderBy(x => x.Orderno).ToList();
            if (widgets != null)
            {
                foreach (var query in widgets)
                {
                   
                        if (query.sqlQuery.Trim().Contains("count"))
                        {
                            var result = _objectEntity.SqlQuery<int?>(query.sqlQuery).FirstOrDefault();
                            if (result != null)
                            {
                                query.aggValue = String.Format("{0:#,##0.##}", result);
                            }
                            else
                            {
                                query.aggValue = "0";
                            }
                        }
                        else
                        {
                            //var result = _objectEntity.SqlQuery<decimal?>(query.sqlQuery).FirstOrDefault();
                            //if (result != null)
                            //{
                            //    query.aggValue = String.Format("{0:#,##0.##}", result);
                            //}
                            //else
                            //{
                            //    query.aggValue = "0";
                            //}
                        }
                   
                   

                    //var result= _objectEntity.SqlQuery<object>(query.sqlQuery).FirstOrDefault();
                    //var resulttest = _objectEntity.SqlQuery<string>(query.sqlQuery).FirstOrDefault();

                }
                return PartialView(widgets);
            }

            // ViewBag.widgets = widgets.Split(',').Where(s => !string.IsNullOrEmpty(s)).ToArray();
            return View();
            //var rslt = _dashboardentryservice.GetMetricList();
            //return PartialView(rslt);
        }

        public ActionResult LCSetup()
        {
            return PartialView();
        }

        public ActionResult LcEntry()
        {
            return PartialView();
        }

        [HttpPost]
        public JsonResult LOCImageUpload(IEnumerable<HttpPostedFileBase> inputFiles, FormCollection collection)
        {
            string loccode = Convert.ToString(collection["loccode"]);
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

                        //var guid = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        //var path = string.Format("{0}\\{1}", pathString, guid);
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
                _lcentryservice.UpdateImage(lcmodels, loccode);
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }


    }
}