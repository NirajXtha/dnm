using NeoErp.Data;
using NeoErp.Planning.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NeoErp.Planning.Service.Models;
using NeoErp.Core.Caching;
using NeoErp.Planning.Services.Models;

using NeoErp.Core;
using NeoErp.Core.Controllers;
using NeoErp.Core.Models;

using NeoErp.Core.Helpers;
using static NeoErp.Planning.Services.Models.MetricWidgetsModel;
using NeoErp.Core.Models.CustomModels;
using NeoErp.Core.Services;

namespace NeoERP.Planning.Controllers
{
    public class HomeController : Controller
    {
        public IPlanSetup _iPlanSetup { get; set; }
        public IPlan _plan;
        private IDbContext _objectEntity;
        private NeoErp.Planning.Service.Interface.IDashBoardMetricService _dashboardMetric { get; set; }
        private ICacheManager _cacheManager { get; set; }
        public HomeController(IPlanSetup iPlanSetup, IPlan plan, NeoErp.Planning.Service.Interface.IDashBoardMetricService dashboardMetric, IDbContext objectEntity, ICacheManager chacheManager)
        {
            this._iPlanSetup = iPlanSetup;
            this._plan = plan;
            this._dashboardMetric = dashboardMetric;
            this._objectEntity = objectEntity;
            this._cacheManager = chacheManager;
        }
        public ActionResult Index()
        {
            return PartialView();
        }
        public ActionResult Organizer()
        {
            return View();
        }
        public ActionResult DashboardChart()
        {
            return View();
        }
        public ActionResult DashBoard()
        {
            var widgets = _dashboardMetric.GetMetricList().Where(x => x.IsActive.Trim() == "Y").OrderBy(x => x.Orderno).ToList();
            if (widgets != null)
            {
                foreach (var query in widgets)
                {

                    if (query.sqlQuery.ToLower().Trim().Contains("count"))
                    {
                        var result = _objectEntity.SqlQuery<string>(query.sqlQuery).ToString();
                        if (result != null)
                        {
                            query.aggValue = String.Format("{0:#,##0.##}", result);
                        }
                        else
                        {
                            query.aggValue = "0";
                        }
                    }
                }
            }
            return PartialView(widgets);
        }
        public ActionResult Setup()
        {
            return PartialView();
        }
        public ActionResult CreateAllPlanTable()
        {
            return PartialView();
        }
        public ActionResult MonthlyWiseSalesPlanReport()
        {
            return PartialView("_MonthlyWiseSalesPlanReport");
        }
        public ActionResult PlanList(string startdate,string enddate)
        {
            ViewBag.startdate = startdate;
            ViewBag.enddate = enddate;
            string employeCode = this._plan.getUsersEmployeeCode();
            ViewBag.EmployeeCode = employeCode;
            return PartialView();
        }
        public ActionResult MasterPlanSetup(string planCode)
        {
            ViewBag.planCode = planCode;
            return View();
        }
        public ActionResult SalesPlanWiseReport(string planCode)
        {
            ViewBag.planCode = planCode;
            return View();
        }

        public ActionResult PlanningDashboard()
        {
              return View();
        }


        public ActionResult MasterPlan(string masterPlanCode)
        {
            ViewBag.masterPlanCode = masterPlanCode;
            return PartialView();
        }

        public ActionResult MasterPlanView(string masterPlanCode)
        {
            ViewBag.masterPlanCode = masterPlanCode;
            return PartialView();
        }

        public ActionResult SubPlanList()
        {
            return PartialView();
        }
        public ActionResult SubPlan()
        {
            return PartialView();
        }
        // Plan Report
        public ActionResult FavSalesPlanReport()
        {
            return PartialView();
        }
        public ActionResult DashBoardWidgets(string reportname = "")
        {
            var cacheNamagerKey = $"neoreopder_{reportname}";
            ViewBag.reportname = reportname;
            var Mainqueryvalue = 0M;
            var widgets = _dashboardMetric.GetMetricList().Where(x => x.IsActive.Trim() == "Y").OrderBy(x => x.Orderno).ToList();
            var orderWidgets = new List<ReordingViewModel>();
            if (this._cacheManager.IsSet(cacheNamagerKey))
            {
                orderWidgets = this._cacheManager.Get<List<ReordingViewModel>>(cacheNamagerKey);
            }
            else
            {
                orderWidgets = _objectEntity.SqlQuery<ReordingViewModel>($"select REPORTNAME,WIDGETSID,XAXIS,YAXIS,HEIGHT,WIDTH from Web_widgets_reporder where reportname='{reportname}'").ToList();
                this._cacheManager.Set(cacheNamagerKey, orderWidgets, 120);
            }
            var dashbaordwidgets = new List<string>();
            if (!string.IsNullOrEmpty(reportname))
            {
                var cacheChartPermissionKey = $"neochartpersmission_{reportname}";
                var dashbaordwidgets1 = new List<string>();
                if (this._cacheManager.IsSet(cacheChartPermissionKey))
                {
                    dashbaordwidgets1 = this._cacheManager.Get<List<string>>(cacheChartPermissionKey);
                }
                else
                {
                    dashbaordwidgets1 = _dashboardMetric.GetDashboard(reportname);
                    this._cacheManager.Set(cacheChartPermissionKey, dashbaordwidgets1, 120);
                }
                foreach (var item in dashbaordwidgets1)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        var data = item.Split(',').ToList();
                        dashbaordwidgets.AddRange(data);
                    }
                }

            }
            var mainWidgets = new List<NeoErp.Planning.Services.Models.MetricWidgetsModel>();
            if (widgets != null)
            {

                foreach (var query in widgets)
                {
                    if (!dashbaordwidgets.Any(x => x == query.widgetsName))
                    {
                        continue;
                    }

                    //   mainWidgets.
                    try
                    {
                        var quicktest = orderWidgets.Where(x => x.WIDGETSID == query.ReportId.ToString()).FirstOrDefault();
                        if (quicktest != null)
                        {
                            query.XAxis = quicktest.XAXIS;
                            query.YAxis = quicktest.YAXIS;
                            query.height = quicktest.HEIGHT;
                            query.Width = quicktest.WIDTH;
                        }

                        if (query.QUICKTYPE == "P")
                        {
                            var quicktest1 = orderWidgets.Where(x => x.WIDGETSID == "sp").FirstOrDefault();
                            if (quicktest1 != null)
                            {
                                query.XAxis = quicktest1.XAXIS;
                                query.YAxis = quicktest1.YAXIS;
                                query.height = quicktest1.HEIGHT;
                                query.Width = quicktest1.WIDTH;
                            }
                            else
                            {
                                query.height = "3";
                                query.Width = "5";
                            }

                        }
                        if (query.QUICKTYPE == "H")
                        {
                            var result = _objectEntity.SqlQuery(query.sqlQuery).ConvertDataTableToHTMLWithFormat();
                            // DataTableListHelper.ConvertTo<List<Spartline>>(result);
                            query.HtmlResult = result;
                            mainWidgets.Add(query);
                            continue;

                        }
                        if (query.QUICKTYPE == "P")
                        {
                            var result = _objectEntity.SqlQuery(query.sqlQuery);
                            var data = DataTableListHelper.ConvertTo<NeoErp.Planning.Services.Models.Spartline>(result).ToList();
                            query.SparkLine.AddRange(data as IEnumerable<NeoErp.Planning.Services.Models.Spartline>);
                            mainWidgets.Add(query);
                            continue;

                        }
                        if (query.QUICKTYPE == "A")
                        {
                            var result = _objectEntity.SqlQuery(query.sqlQuery);
                            var data = DataTableListHelper.ConvertTo<NeoErp.Planning.Services.Models.Spartline>(result).ToList();
                            query.SparkLine.AddRange(data as IEnumerable<NeoErp.Planning.Services.Models.Spartline>);
                            mainWidgets.Add(query);
                            continue;

                        }
                        if (query.sqlQuery.ToLower().Trim().Contains("count"))
                        {
                            try
                            {
                                var result = _objectEntity.SqlQuery<decimal?>(query.sqlQuery).FirstOrDefault();
                                if (result != null)
                                {
                                    Mainqueryvalue = result ?? 0;
                                    query.aggValue = String.Format("{0:#,##0.##}", result);
                                }
                                else
                                {
                                    Mainqueryvalue = result ?? 0;
                                    query.aggValue = "0";
                                }
                            }
                            catch (Exception ex)
                            {
                                var result = _objectEntity.SqlQuery(query.sqlQuery);

                                if (result.Rows.Count > 0)
                                {
                                    var data = result.Rows[0][0].ToString();
                                    Decimal decimalValue = 0;
                                    if (Decimal.TryParse(data, out decimalValue))
                                    {
                                        Mainqueryvalue = decimalValue;
                                        query.aggValue = String.Format("{0:#,##0.##}", decimalValue);
                                    }
                                    else
                                    {
                                        Mainqueryvalue = 0;
                                        query.aggValue = data;
                                    }

                                }
                                else
                                {
                                    Mainqueryvalue = 0;
                                    query.aggValue = "0";
                                }
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
                                    Mainqueryvalue = decimalValue;
                                    query.aggValue = String.Format("{0:#,##0.##}", decimalValue);
                                }
                                else
                                {
                                    Mainqueryvalue = 0;
                                    query.aggValue = data;
                                }

                            }
                            else
                            {
                                Mainqueryvalue = 0;
                                query.aggValue = "0";
                            }
                        }

                        if (query.QUICKTYPE == "C")
                        {
                            if (query.MAXVALUEQUERY.ToLower().Trim().Contains("count"))
                            {
                                try
                                {
                                    var result = _objectEntity.SqlQuery<decimal?>(query.MAXVALUEQUERY).FirstOrDefault();
                                    if (result != null)
                                    {
                                        query.MaxQueryValue = result;
                                    }
                                    else
                                    {
                                        query.MaxQueryValue = 0;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    var result = _objectEntity.SqlQuery(query.MAXVALUEQUERY);

                                    if (result.Rows.Count > 0)
                                    {
                                        var data = result.Rows[0][0].ToString();
                                        Decimal decimalValue = 0;
                                        if (Decimal.TryParse(data, out decimalValue))
                                        {
                                            query.MaxQueryValue = decimalValue;
                                        }
                                        else
                                        {
                                            query.MaxQueryValue = 0;
                                        }

                                    }
                                    else
                                    {
                                        query.MaxQueryValue = 0;
                                    }
                                }
                            }
                            else
                            {
                                var result = _objectEntity.SqlQuery(query.MAXVALUEQUERY);

                                if (result.Rows.Count > 0)
                                {
                                    var data = result.Rows[0][0].ToString();
                                    Decimal decimalValue = 0;
                                    if (Decimal.TryParse(data, out decimalValue))
                                    {
                                        query.MaxQueryValue = decimalValue;
                                    }
                                    else
                                    {
                                        query.MaxQueryValue = 0;
                                    }

                                }
                                else
                                {
                                    query.MaxQueryValue = 0;
                                }
                            }

                            if (query.calculationBase == "P")
                            {
                                var datacalculation = Mainqueryvalue - query.MaxQueryValue;
                                var percentage = Math.Round(Convert.ToDouble((datacalculation / query.MaxQueryValue) * 100), 2);
                                query.SecondaryValue = percentage.ToString();


                            }
                            else if (query.calculationBase == "D")
                            {
                                var datacalculation = Mainqueryvalue - query.MaxQueryValue;
                                // var percentage = (datacalculation / query.MaxQueryValue) * 100;
                                query.SecondaryValue = datacalculation.ToString();

                            }
                            else if (query.calculationBase == "E")
                            {
                                query.SecondaryValue = query.MaxQueryValue.ToString();
                            }

                        }


                        mainWidgets.Add(query);
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }

                    //var result= _objectEntity.SqlQuery<object>(query.sqlQuery).FirstOrDefault();
                    //var resulttest = _objectEntity.SqlQuery<string>(query.sqlQuery).FirstOrDefault();

                }
                return View(mainWidgets);
            }

            // ViewBag.widgets = widgets.Split(',').Where(s => !string.IsNullOrEmpty(s)).ToArray();
            return View();
        }

        [Authorize]
        public ActionResult DashBoardPartial(string page = "1", string reportName = null)
        {
            if (Session["oldPagenumber_" + reportName] as string == page)
                return null;

            if (page == "1")
                Session["pagenumber"] = null;

            //return null;  
            int intpage = 1;
            int pagesize = 2;
            // Slaes Define draggerable save
            var model = new DashboardViewModel();
            if (!string.IsNullOrEmpty(Session["pagenumber"] as string))
            {
                intpage = Convert.ToInt16(Session["pagenumber"]) + 1;
                Session["pagenumber"] = intpage.ToString();
            }
            else
            {
                Session["pagenumber"] = intpage.ToString();
            }
            Session["oldPagenumber_" + reportName] = page;



            var widgetsList = new List<string>();
            string sortedWidgets = _plan.GetDashboardWidgets(User.Identity.Name, reportName + "_sorted");
            string allWidgets = _plan.GetDashboardWidgets(User.Identity.Name, reportName);
            if (!string.IsNullOrEmpty(sortedWidgets))
            {
                widgetsList = sortedWidgets.Split(',').Where(s => !string.IsNullOrEmpty(s)).Distinct().ToList();
                var widgetsListTotal = allWidgets.Split(',').Where(s => !string.IsNullOrEmpty(s)).Select(x => x.Substring(0, x.IndexOf('$'))).ToList();
                widgetsList.AddRange(widgetsListTotal.Where(x => !widgetsList.Any(y => y == x)));
            }
            else if (!string.IsNullOrEmpty(allWidgets))
            {
                widgetsList = allWidgets.Split(',').Where(s => !string.IsNullOrEmpty(s)).ToList();
            }


            model.AllchartList = widgetsList.Skip((intpage - 1) * pagesize).Take(pagesize).ToList();
            model.CountTotalChart = widgetsList.Count;
            return View(model);
        }


    }
}