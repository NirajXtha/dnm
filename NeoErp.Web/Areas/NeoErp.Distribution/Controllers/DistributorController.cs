using NeoErp.Core;
using NeoErp.Core.Domain;
using NeoErp.Core.Models;
using NeoErp.Core.Services;
using NeoErp.Distribution.Service.DistributorServices;
using NeoErp.Distribution.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using LinqToExcel;
using NeoErp.Distribution.Service.Service;

namespace NeoErp.Distribution.Controllers
{
    public class DistributorController : Controller
    {
        private IDistributorService _service;
        private IWorkContext _workContext;
        private IAuthenticationService _authenticationService;
        private IDashboard _dashboard;
        private NeoErpCoreEntity _objectEntity;
        private ISetupService _setUpObjectEntity;
        public DistributorController(IDistributorService service, NeoErpCoreEntity objectEntity, IWorkContext workContext, IAuthenticationService authenticationService,IDashboard dashboard, ISetupService setUpObjectEntity)
        {
            _service = service;
            _workContext = workContext;
            _authenticationService = authenticationService;
            _dashboard = dashboard;
            _setUpObjectEntity = setUpObjectEntity;
            this._objectEntity = objectEntity;
        }
        // GET: Distributor
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DistributorDashboard()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(User model)
        {
            var user = _service.DistributorLogin(model);
            
            if (user == null)
            {
                ViewBag.Message = "Invalid Username/Password";
                return View();
            }
            user.LoginType = "Distributor";
            Session["UserName"] = user.UserName;
            Session["destributer"] = user.USER_NO;
            Session["userid"] = user.User_id;
            _workContext.CurrentUserinformation = user;
            _authenticationService.SignIn(user, true);
            return RedirectToAction("Index");
        }
        public ActionResult LogOut()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
        public ActionResult ResellerPurchaseOrder()
        {
            return View();
        }

        public ActionResult WholeSellerPurchaseReport()
        {
            return View();
        }
        public ActionResult ResellerPOStatus()
        {
            return PartialView();
        }
        public ActionResult ResellerPOSummaryStatus()
        {
            return PartialView();
        }

        public ActionResult PurchaseOrderSummary()
        {
            return PartialView();
        }

        public ActionResult PurchaseOrder()
        {
            return PartialView();
        }
        public ActionResult DashboardWidgetsDistributor()
        {
            var data = _dashboard.GetDashBoardWidgets(_workContext.CurrentUserinformation);
            return PartialView(data);
        }

        public ActionResult CollectionReport()
        {
            return PartialView();
        }
        public ActionResult AccountStatement()
        {
            return PartialView();
        }
        public ActionResult ClosingReport()
        {
            return PartialView();
        }

        public ActionResult ClosingReportNew()
        {
            return PartialView();
        }

        public ActionResult AgingReport()
        {
            return PartialView();
        }

        public ActionResult BranchWiseSalesVsTarget()
        {

            return PartialView();
        }

        public ActionResult DivisionWiseCreditLismit()
        {
            return PartialView();
        }

        public ActionResult SalesTargetvsAchievementReport()
        {
            return PartialView();
        }
        
        public ActionResult SalesTargetvsCustomerAchievementReport()
        {
            //var columns = _service.GetDynamicFields(_workContext.CurrentUserinformation);
            return PartialView();
        }

        public ActionResult SchemeReport()
        {
            return PartialView();
        }

        public ActionResult DistWidgets()
        {
            var userdata = _workContext.CurrentUserinformation;
            var widgets = _dashboard.GetDistMatrics(true).Where(x => x.IsActive.ToString() == "Y").ToList();
            if (widgets != null)
            {
                foreach (var query in widgets)
                {
                    try
                    {
                        if (query.DISTRIBUTOR_CHECK_TYPE.Trim().ToLower() != "All".Trim().ToLower())
                        {
                            if (query.DISTRIBUTOR_CODE.Trim() != userdata.DistributerNo.Trim())
                                continue;
                            else
                            {
                                if (query.sqlQuery.Contains("#distributor#"))
                                {
                                    query.sqlQuery = query.sqlQuery.Replace("#distributor#", userdata.DistributerNo);
                                }

                                if (query.sqlQuery.ToLower().Trim().Contains("count"))
                                {
                                    var formatedQuery = query.sqlQuery.Replace("DELETED_FLAG=N", "DELETED_FLAG='N'");
                                    var result = _objectEntity.SqlQuery<int?>(formatedQuery).FirstOrDefault();
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
                                    var result = _objectEntity.SqlQuery(query.sqlQuery);

                                    if (result.Rows.Count > 0)
                                    {
                                        var data = result.Rows[0][0].ToString();
                                        Decimal decimalValue = 0;
                                        if (Decimal.TryParse(data, out decimalValue))
                                        {
                                            query.aggValue = String.Format("{0:#,##0.##}", decimalValue);
                                        }
                                        else
                                        {
                                            query.aggValue = data;
                                        }

                                    }
                                    else
                                    {
                                        query.aggValue = "0";
                                    }
                                }
                            }
                        }
                        else
                        {
                            if(query.sqlQuery.Contains("#distributor#"))
                            {
                                query.sqlQuery = query.sqlQuery.Replace("#distributor#",userdata.DistributerNo);
                            }

                            if (query.sqlQuery.ToLower().Trim().Contains("count"))
                            {
                                var formatedQuery = query.sqlQuery.Replace("DELETED_FLAG=N", "DELETED_FLAG='N'");
                                var result = _objectEntity.SqlQuery<int?>(formatedQuery).FirstOrDefault();
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
                                var result = _objectEntity.SqlQuery(query.sqlQuery);

                                if (result.Rows.Count > 0)
                                {
                                    var data = result.Rows[0][0].ToString();
                                    Decimal decimalValue = 0;
                                    if (Decimal.TryParse(data, out decimalValue))
                                    {
                                        query.aggValue = String.Format("{0:#,##0.##}", decimalValue);
                                    }
                                    else
                                    {
                                        query.aggValue = data;
                                    }

                                }
                                else
                                {
                                    query.aggValue = "0";
                                }
                            }

                        }


                    }
                    catch (Exception ex)
                    {
                        continue;
                    }

                    //var result= _objectEntity.SqlQuery<object>(query.sqlQuery).FirstOrDefault();
                    //var resulttest = _objectEntity.SqlQuery<string>(query.sqlQuery).FirstOrDefault();

                }
                return PartialView(widgets);
            }
            return PartialView(widgets);
        }


        [HttpPost]
        public JsonResult ImportDistributor(HttpPostedFileBase file)
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
                        string paths = Server.MapPath("~/DistributionExcel/Distributor/" + file.FileName);
                        string strMappath = "~/DistributionExcel/Distributor/";
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

                        var listItems = (from a in excel.Worksheet<DistributorListModel>(sheetName)
                                         where !String.IsNullOrEmpty(a.DISTRIBUTOR_NAME)
                                         select a).ToList();
                        var result = string.Empty;
                        var inserted = 0;
                        foreach (var item in listItems)
                        {
                            var obj = _setUpObjectEntity.GetDistributorSaveObj(item, _workContext.CurrentUserinformation);
                            result = _setUpObjectEntity.AddDistributor(obj, _workContext.CurrentUserinformation);
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