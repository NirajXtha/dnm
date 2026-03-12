using NeoErp.Core;
using NeoErp.Distribution.Service.Model;
using NeoErp.Distribution.Service.Service;
using Excel = Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using LinqToExcel;

namespace NeoErp.Distribution.Controllers
{
    public class SetupController : Controller
    {
        public IUserService _objectEntity { get; set; }
        private IWorkContext _workContext;
        private ISetupService _service;

        public SetupController(IUserService userService, IWorkContext workContext, ISetupService service)
        {
            this._objectEntity = userService;
            this._workContext = workContext;
            this._service = service;
        }
        // GET: Setup
        public ActionResult DistributorSetup()
        {
            return PartialView();
        }
        public ActionResult FreeQty()
        {
            return PartialView();
        }

        public ActionResult UserAccessSetup()
        {
            return PartialView();
        }

        public ActionResult CreateDistributor()
        {
            return PartialView("_CreateDistributor");
        }

        public ActionResult ResellerSetup(string status)
        {
            return PartialView(status);
        }
        public ActionResult FarmSetup()
        {
            return PartialView();
        }

        public ActionResult RouteSetup()
        {
            return PartialView();
        }
        public ActionResult RouteSetupBranding()
        {
            return PartialView();
        }

        public ActionResult PreferenceSetup()
        {
            // value object of DIST_PREFERENCE_SETUP
            // Y/N value
            PreferenceSetupModel dps = new PreferenceSetupModel();
            dps = _objectEntity.GetPreferenceSetup(_workContext.CurrentUserinformation);
            return View(dps);
        }

        public ActionResult CustomerSetup()
        {
            return PartialView();
        }

        public ActionResult DealerSetup()
        {
            return PartialView();
        }
        public ActionResult AreaSetup()
        {
            return PartialView();
        }
        public ActionResult QuestionSetup()
        {
            return PartialView();
        }
        public ActionResult QuestionList()
        {
            return PartialView();
        }

        public ActionResult OutLetSetup()
        {
            return PartialView();
        }
        public ActionResult GroupSetup()
        {
            return PartialView();
        }
        public ActionResult ImageCategorySetup()
        {
            return PartialView();
        }

        public ActionResult UserSetupTree()
        {
            return PartialView();
        }

        public ActionResult ClosingStock()
        {
            return PartialView();
        }
        public ActionResult OpeningStockSetup()
        {
            return PartialView();
        }
        public ActionResult MobileRegistration()
        {
            var data = _objectEntity.GetAllDevices(_workContext.CurrentUserinformation);
            return PartialView(data);
        }

        public ActionResult DistQueryBuilder()
        {
            return PartialView();
        }
        public ActionResult NotificationSetup()
        {
            return PartialView();
        }

        public ActionResult OtherEntitySetup()
        {
            return PartialView();
        }


        public ActionResult EmployeeSetup()
        {
            return PartialView();
        }

        public ActionResult CompItemSetup()
        {
            return PartialView();
        }

        public ActionResult CompItemMap()
        {
            return PartialView();
        }

        public ActionResult CompFieldSetup()
        {
            return PartialView();
        }

        public ActionResult GroupMapping()
        {
            return PartialView();
        }
        public ActionResult DistUserMapping()
        {
            return PartialView();
        }

        public ActionResult QuickSetup()
        {
            return PartialView();
        }

        public ActionResult ItemSetup()
        {
            return PartialView();
        }

        public ActionResult OutletItemRate()
        {
            return PartialView();
        }

        public ActionResult SurveySetup()
        {
            return PartialView();
        }

        public ActionResult SchemeSetup()
        {
            return PartialView();
        }

        public ActionResult SchemeApprovalSetup()
        {
            return PartialView();
        }

        public JsonResult AreaImport(HttpPostedFileBase file)
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
                        string paths = Server.MapPath("~/DistributionExcel/Distribution/" + file.FileName);
                        string strMappath = "~/DistributionExcel/Distribution/";
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

                        var listItems = (from a in excel.Worksheet<AreaModel>(sheetName)
                                         where !String.IsNullOrEmpty(a.AREA_NAME)
                                         select a).ToList();
                        var result = string.Empty;
                        var inserted = 0;
                        var user = _workContext.CurrentUserinformation;
                        foreach (var item in listItems)
                        {
                            var saveObj = _service.GetAreaSaveObj(item, user);
                            result = _service.AddArea(item, _workContext.CurrentUserinformation);
                            if (result == "success")
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

        public JsonResult ZoneImport(HttpPostedFileBase file)
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
                        string paths = Server.MapPath("~/DistributionExcel/Distribution/" + file.FileName);
                        string strMappath = "~/DistributionExcel/Distribution/";
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

                        var listItems = (from a in excel.Worksheet<ResellerGroupModel>(sheetName)
                                         where !String.IsNullOrEmpty(a.GROUP_EDESC)
                                         select a).ToList();
                        var result = string.Empty;
                        var inserted = 0;
                        var user = _workContext.CurrentUserinformation;
                        foreach (var item in listItems)
                        {
                            result = _service.AddResellerGroup(item, _workContext.CurrentUserinformation);
                            if (result == "Success")
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
        //public JsonResult ImportRouteData(HttpPostedFileBase file)
        //{
        //    Excel.Application application = new Excel.Application();
        //    try
        //    {
        //        if (file == null || file.ContentLength == 0)
        //        {

        //            return Json(new { TYPE = "error", MESSAGE = "Empty File" }, JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            if (file.FileName.EndsWith("xls") || file.FileName.EndsWith("xlsx"))
        //            {
        //                string paths = Server.MapPath("~/DistributionExcel/Branding/" + file.FileName);
        //                string strMappath = "~/DistributionExcel/Branding/";
        //                string path = System.IO.Path.Combine(Server.MapPath(strMappath), file.FileName);
        //                if (!Directory.Exists(strMappath))
        //                {
        //                    Directory.CreateDirectory(Server.MapPath(strMappath));
        //                }
        //                if (System.IO.File.Exists(path))
        //                    System.IO.File.Delete(path);
        //                file.SaveAs(path);
        //                string sheetName = "Sheet1";
        //                string filepath = paths;
        //                var excel = new ExcelQueryFactory(filepath);
        //                var worksheetNames = excel.GetWorksheetNames();
        //                if (worksheetNames.ElementAt(0) != sheetName)
        //                {
        //                    return Json(new { TYPE = "warning", MESSAGE = "Sheet name mismatched" }, JsonRequestBehavior.AllowGet);
        //                }

        //                var listItems = (from a in excel.Worksheet<RouteImportModel>(sheetName)
        //                                 where !String.IsNullOrEmpty(a.ROUTE_NAME)
        //                                 select a).ToList();
        //                var groupedData = listItems.GroupBy(x => x.ROUTE_NAME);
        //                var result = string.Empty;
        //                var inserted = 0;
        //                foreach (var item in groupedData)
        //                {
        //                    var group = new RouteGroup
        //                    {
        //                        Data = item
        //                    };
        //                    var saveObj = _service.GetRouteSaveObj(group, _workContext.CurrentUserinformation);
        //                    result = _service.AddRoute(saveObj, _workContext.CurrentUserinformation);
        //                    if (result == "200")
        //                        inserted++;
        //                }

        //                return Json(new { TYPE = "success", MESSAGE = inserted + " items successfully inserted" }, JsonRequestBehavior.AllowGet);
        //            }
        //            else
        //            {
        //                return Json(new { TYPE = "error", MESSAGE = "File format error" }, JsonRequestBehavior.AllowGet);
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(ex.Message, JsonRequestBehavior.AllowGet);
        //    }
        //    finally
        //    {
        //        application.Quit();
        //    }

        //}

        public JsonResult ImportRouteData(HttpPostedFileBase file)
        {
            Excel.Application xlApp = null;
            Excel.Workbook xlWorkbook = null;
            try
            {
                if (file == null || file.ContentLength == 0)
                    return Json(new { TYPE = "error", MESSAGE = "Empty file" }, JsonRequestBehavior.AllowGet);

                string fileName = Path.GetFileName(file.FileName);
                string strMappath = "~/DistributionExcel/Branding/";
                string filePath = Path.Combine(Server.MapPath(strMappath), fileName);

                if (!Directory.Exists(Server.MapPath(strMappath)))
                    Directory.CreateDirectory(Server.MapPath(strMappath));

                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                file.SaveAs(filePath);

                xlApp = new Excel.Application();
                xlWorkbook = xlApp.Workbooks.Open(filePath);
                Excel._Worksheet xlWorksheet = xlWorkbook.Sheets["Sheet1"];
                Excel.Range xlRange = xlWorksheet.UsedRange;

                int rowCount = xlRange.Rows.Count;
                int colCount = xlRange.Columns.Count;

                var list = new List<RouteImportModel>();

                for (int i = 2; i <= rowCount; i++) // assuming row 1 is header
                {
                    var routeName = (xlRange.Cells[i, 1] as Excel.Range)?.Text?.ToString();
                    var area = (xlRange.Cells[i, 2] as Excel.Range)?.Text?.ToString();
                    var routeType = (xlRange.Cells[i, 3] as Excel.Range)?.Text?.ToString();
                    var entity = (xlRange.Cells[i, 4] as Excel.Range)?.Text?.ToString();
                    var entityType = (xlRange.Cells[i, 5] as Excel.Range)?.Text?.ToString();

                    if (string.IsNullOrWhiteSpace(routeName)) continue;

                    var model = new RouteImportModel
                    {
                        ROUTE_NAME = routeName,
                        AREA = area,

                        ROUTE_TYPE = routeType,
                        ENTITY = entity,
                        ENTITY_TYPE = entityType
                    };

                    list.Add(model);
                }

                // Group and insert
                int inserted = 0;
                foreach (var group in list.GroupBy(x => x.ROUTE_NAME))
                {
                    var g = new RouteGroup { Data = group };
                    var saveObj = _service.GetRouteSaveObj(g, _workContext.CurrentUserinformation);
                    var result = _service.AddRoute(saveObj, _workContext.CurrentUserinformation);
                    if (result == "200") inserted++;
                }

                return Json(new { TYPE = "success", MESSAGE = inserted + " items inserted" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { TYPE = "error", MESSAGE = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                if (xlWorkbook != null)
                {
                    xlWorkbook.Close(false);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkbook);
                }

                if (xlApp != null)
                {
                    xlApp.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }


        [HttpPost]
        public JsonResult ImportOutletCategoryData(HttpPostedFileBase file)
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
                        string sheetName = "Sheet1";
                        string filepath = paths;
                        var excel = new ExcelQueryFactory(filepath);
                        var worksheetNames = excel.GetWorksheetNames();
                        if (worksheetNames.ElementAt(0) != sheetName)
                        {
                            return Json(new { TYPE = "warning", MESSAGE = "Sheet name mismatched" }, JsonRequestBehavior.AllowGet);
                        }

                        var listItems = (from a in excel.Worksheet<OutletSubtypeModel>(sheetName)
                                         where !String.IsNullOrEmpty(a.TYPE_EDESC)
                                         select a).ToList();
                        var groupedData = listItems.GroupBy(x => x.TYPE_CODE);
                        var result = string.Empty;
                        var inserted = 0;
                        foreach (var item in groupedData)
                        {
                            var subType = (from a in item.ToList()
                                           select new SubtypeArray
                                           {
                                               SUBTYPE_EDESC = a.SUBTYPE_EDESC,
                                               SUBTYPE_CODE = a.SUBTYPE_CODE
                                           }).ToList();
                            var saveObj = new OutletModel
                            {
                                TYPE_EDESC = item.FirstOrDefault().TYPE_EDESC,
                                TYPE_CODE = item.Key,
                                subtypeArr = subType
                            };
                            result = _service.AddOutlet(saveObj, _workContext.CurrentUserinformation);
                            if (result == "success")
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

        public JsonResult ImageCategory(HttpPostedFileBase file)
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
                        string paths = Server.MapPath("~/DistributionExcel/ImageCategory/" + file.FileName);
                        string strMappath = "~/DistributionExcel/ImageCategory/";
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

                        var listItems = (from a in excel.Worksheet<ImageCategoryModel>(sheetName)
                                         where !String.IsNullOrEmpty(a.CATEGORY_EDESC)
                                         select a).ToList();
                        var result = string.Empty;
                        var inserted = 0;
                        var user = _workContext.CurrentUserinformation;
                        foreach (var item in listItems)
                        {
                            result = _service.AddImageCategory(item, _workContext.CurrentUserinformation);
                            if (result == "Success")
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

        public JsonResult ImportCompetitorItemSetup(HttpPostedFileBase file)
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
                        string paths = Server.MapPath("~/DistributionExcel/CompetitorItemSetup/" + file.FileName);
                        string strMappath = "~/DistributionExcel/CompetitorItemSetup/";
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

                        var listItems = (from a in excel.Worksheet<CompItemSetupModel>(sheetName)
                                         where !String.IsNullOrEmpty(a.ITEM_EDESC)
                                         select a).ToList();
                        var result = string.Empty;
                        var inserted = 0;
                        var user = _workContext.CurrentUserinformation;
                        foreach (var item in listItems)
                        {
                            result = _service.CreateCompItem(item, _workContext.CurrentUserinformation);
                            if (result == "200")
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

        public JsonResult ImportCompetitorMapSetup(HttpPostedFileBase file)
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
                        string paths = Server.MapPath("~/DistributionExcel/CompetitorItemMapSetup/" + file.FileName);
                        string strMappath = "~/DistributionExcel/CompetitorItemMapSetup/";
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

                        var listItems = (from a in excel.Worksheet<CompItemModel>(sheetName)
                                         where !String.IsNullOrEmpty(a.ITEM_EDESC)
                                         select a).ToList();
                        var result = string.Empty;
                        var inserted = 0;
                        var user = _workContext.CurrentUserinformation;
                        List<CompItemModel> newCompItemMapList = new List<CompItemModel>();
                        foreach (var item in listItems)
                        {
                            var saveObj = _service.GetCompItemMapSaveObj(item, user);
                            newCompItemMapList.Add(saveObj);
                            inserted++;
                        }
                        result = _service.SaveCompItemMap(newCompItemMapList, _workContext.CurrentUserinformation);

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

       
    }
}