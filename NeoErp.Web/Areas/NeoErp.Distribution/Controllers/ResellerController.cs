using NeoErp.Core;
using NeoErp.Distribution.Service.Service;
using NeoErp.Distribution.Service.Service.Branding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NeoErp.Distribution.Service.Model.BrandingModule;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using LinqToExcel;
using NeoErp.Distribution.Service.Model;

namespace NeoErp.Distribution.Controllers
{
    public class ResellerController : Controller
    {
        public ISetupService _objectEntity { get; set; }
        private IWorkContext _workContext;
        public ResellerController(ISetupService objectEntity, IWorkContext workContext)
        {
            this._objectEntity = objectEntity;
            this._workContext = workContext;
        }
        // GET: Reseller
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ImportReseller(HttpPostedFileBase file)
        {
            Excel.Application application = new Excel.Application();
            try
            {
                if (file == null || file.ContentLength == 0)
                {

                    return Json(new { TYPE = "error", MESSAGE = "Empty File" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (file.FileName.EndsWith("xls") || file.FileName.EndsWith("xlsx"))
                    {
                        string paths = Server.MapPath("~/DistributionExcel/Reseller/" + file.FileName);
                        string strMappath = "~/DistributionExcel/Reseller/";
                        string path = System.IO.Path.Combine(Server.MapPath(strMappath), file.FileName);
                        if (!Directory.Exists(strMappath))
                        {
                            Directory.CreateDirectory(Server.MapPath(strMappath));
                        }
                        if (System.IO.File.Exists(path))
                            System.IO.File.Delete(path);
                        file.SaveAs(path);
                        string sheetName = "Sheet1";
                        string filepath = paths;
                        var excel = new ExcelQueryFactory(filepath);
                        var worksheetNames = excel.GetWorksheetNames();
                        if (worksheetNames.ElementAt(0) != sheetName)
                        {
                            return Json(new { TYPE = "warning", MESSAGE = "Sheet name mismatched" }, JsonRequestBehavior.AllowGet);
                        }

                        var listItems = (from a in excel.Worksheet<ResellerListModel>(sheetName)
                                         where !string.IsNullOrEmpty(a.Reseller_NAME)
                                         select a).ToList();
                        var result = string.Empty;
                        var inserted = 0;
                        foreach (var item in listItems)
                        {
                            var obj = _objectEntity.GetResellerSaveObj(item, _workContext.CurrentUserinformation);
                            result = _objectEntity.AddReseller(obj, _workContext.CurrentUserinformation);
                            if (result == "200")
                                inserted++;
                        }

                        return Json(new { TYPE = "success", MESSAGE = inserted + " items successfully inserted" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { TYPE = "error", MESSAGE = "File format error" }, JsonRequestBehavior.AllowGet);
                    }

                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                application.Quit();
            }

        }
    }
}