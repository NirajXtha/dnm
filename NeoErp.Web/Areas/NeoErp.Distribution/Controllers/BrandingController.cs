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
using Newtonsoft.Json;

namespace NeoErp.Distribution.Controllers
{
    public class BrandingController : Controller
    {
        public IBrandingService _objectEntity { get; set; }
        private IWorkContext _workContext;

        public BrandingController(IBrandingService userService, IWorkContext workContext)
        {
            this._objectEntity = userService;
            this._workContext = workContext;
        }
        // GET: Branding
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BrandingActivity()
        {
            return PartialView();
        }
        public ActionResult ContractSetup() //scheme contract
        {
            return PartialView();
        }

        public ActionResult EventContractSetup()
        {
            return PartialView();
        }

        public ActionResult OtherContractSetup()
        {
            return PartialView();
        }

        public ActionResult ContractSummary()
        {
            return PartialView();
        }

        public ActionResult SchemeReport()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult SaveTutorial(AttachedFile uploadFile)
        {
            foreach (string file in Request.Files)
            {
                var fileContent = Request.Files[file];
                if (fileContent != null && fileContent.ContentLength > 0)
                {
                    var inputStream = fileContent.InputStream;
                    var fileName = Path.GetFileName(file);
                    var path = Path.Combine(Server.MapPath("~/App_Data/BrandingAttachment"), fileName);
                    using (var fileStream = System.IO.File.Create(path))
                    {
                        inputStream.CopyTo(fileStream);
                    }
                }
            }
            return Json("Tutorial Saved", JsonRequestBehavior.AllowGet);
        }

        public ActionResult ContractReport()
        {
            return PartialView();
        }
        
        [HttpPost]
        public JsonResult ImportBrdActivity(HttpPostedFileBase file)
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
                        string paths = Server.MapPath("~/DistributionExcel/Branding/" + file.FileName);
                        string strMappath = "~/DistributionExcel/Branding/";
                        string path = System.IO.Path.Combine(Server.MapPath(strMappath), file.FileName);
                        if (!Directory.Exists(strMappath))
                        {
                            Directory.CreateDirectory(Server.MapPath(strMappath));
                        }
                        if (System.IO.File.Exists(path))
                            System.IO.File.Delete(path);
                        file.SaveAs(path);
                        string sheetName = "Sheet 1";
                        string filepath = paths;
                        var excel = new ExcelQueryFactory(filepath);
                        var worksheetNames = excel.GetWorksheetNames();
                        if (worksheetNames.ElementAt(0) != sheetName)
                        {
                            return Json(new { TYPE = "warning", MESSAGE = "Sheet name mismatched" }, JsonRequestBehavior.AllowGet);
                        }

                        var listItems = (from a in excel.Worksheet<ActivityModel>(sheetName)
                                         where !string.IsNullOrEmpty(a.ACTIVITY_EDESC)
                                         select a).ToList();
                        var result = string.Empty;
                        var inserted = 0;
                        foreach (var item in listItems)
                        {
                            result = _objectEntity.saveBrandingActivity(item, _workContext.CurrentUserinformation);
                            if (result == "success")
                                inserted++;
                        }

                        return Json(new { TYPE = "success", MESSAGE = inserted + " items successfully added" }, JsonRequestBehavior.AllowGet);
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

        [HttpPost]
        public JsonResult ImportBrdContract(HttpPostedFileBase file)
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
                        string paths = Server.MapPath("~/DistributionExcel/Branding/" + file.FileName);
                        string strMappath = "~/DistributionExcel/Branding/";
                        string path = System.IO.Path.Combine(Server.MapPath(strMappath), file.FileName);
                        if (!Directory.Exists(strMappath))
                        {
                            Directory.CreateDirectory(Server.MapPath(strMappath));
                        }
                        if (System.IO.File.Exists(path))
                            System.IO.File.Delete(path);
                        file.SaveAs(path);
                        string sheetName = "Sheet 1";
                        string filepath = paths;
                        var excel = new ExcelQueryFactory(filepath);
                        var worksheetNames = excel.GetWorksheetNames();
                        if (worksheetNames.ElementAt(0) != sheetName)
                        {
                            return Json(new { TYPE = "warning", MESSAGE = "Sheet name mismatched" }, JsonRequestBehavior.AllowGet);
                        }

                        var listItems = (from a in excel.Worksheet<ContactImportModel>(sheetName)
                                         where !String.IsNullOrEmpty(a.CONTRACT_EDESC)
                                         select a).ToList();
                        var result = string.Empty;
                        var inserted = 0;
                        var user = _workContext.CurrentUserinformation;
                        foreach (var item in listItems)
                        {
                            var obj = _objectEntity.GetContractSaveObj(item, user);
                            result = _objectEntity.saveContract(obj, user);
                            if (result == "success")
                                inserted++;
                        }

                        return Json(new { TYPE = "success", MESSAGE = inserted + " items successfully added" }, JsonRequestBehavior.AllowGet);
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

        //public string saveUserWebQA(WEB_TEST model)
        //{
        //    keyvalue V= JsonConvert.DeserializeObject<keyvalue>(model.DATA);
        //    return "";
        //}

        public string saveUserWebQA(FormCollection fc)
        {
            var result = string.Empty;
            try
            {
                var list = new List<WebQueAnsModel>();
                foreach (var key in fc.AllKeys)
                {
                    var que = key;
                    var value = fc[key];
                    if (que.Contains("radio*"))
                    {
                        que = fc[key].Split(':')[0];
                        value = fc[key].Split(':')[1];
                    }
                    if (string.IsNullOrWhiteSpace(value) || que=="customer_code" || que=="customer_name")
                        continue;
                    var obj = new WebQueAnsModel
                    {
                        webqakey = que,
                        webqavalue = value
                    };
                    list.Add(obj);
                }
                var commonObj = new WebQueAnsCommonModel
                {
                    customer_code = fc["customer_code"],
                    customer_name = fc["customer_name"],
                };
                if (list.Count>0)
                    result = _objectEntity.saveUserSurveyReportWeb(list,commonObj,_workContext.CurrentUserinformation);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }
    }
}