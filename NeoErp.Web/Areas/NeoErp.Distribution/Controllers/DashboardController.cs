using NeoErp.Core;
using NeoErp.Core.Caching;
using NeoErp.Core.Models;
using NeoErp.Core.Models.CustomModels;
using NeoErp.Core.Services;
using NeoErp.Distribution.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NeoErp.Core.Helpers;

namespace NeoErp.Distribution.Controllers
{
    public class DashboardController : Controller
    {
        private IDashBoardMetricService _dashBoardMetricservice { get; set; }
        private NeoErpCoreEntity _objectEntity;
        private IWorkContext _workContext;
        private ICacheManager _cacheManager { get; set; }
        public DashboardController(IDashBoardMetricService dashBoardMetricservice, NeoErpCoreEntity objectEntity, IWorkContext workContext, ICacheManager chacheManager)
        {
           
            this._dashBoardMetricservice = dashBoardMetricservice;
            this._objectEntity = objectEntity;
            this._workContext = workContext;
            this._cacheManager = chacheManager;
        }
        // GET: Dashboard
        public ActionResult Index(string id = null)
        {
            return RedirectToAction("ChartDashboard", "Home", new { id = id });
        }
        public ActionResult DashBoardWidgets(string reportname = "")
        {
            var MainCancheName = $"neoCache_{reportname}";

            var QueryCacheName = $"neocacheQuery_{reportname}";

            TempData["static"] = false;
            var cacheNamagerKey = $"neoreopder_{reportname}";
            ViewBag.reportname = reportname;
            var Mainqueryvalue = 0M;
            var widgets = _dashBoardMetricservice.GetMetricListWithModuleCode(true,"10").Where(x => x.IsActive.Trim() == "Y").OrderBy(x => x.Orderno).ToList();
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
                var dashbaordwidgets1 = new List<MeticorderModel>();
                if (this._cacheManager.IsSet(cacheChartPermissionKey))
                {
                    dashbaordwidgets1 = this._cacheManager.Get<List<MeticorderModel>>(cacheChartPermissionKey);
                }
                else
                {

                    dashbaordwidgets1 = _dashBoardMetricservice.GetDashboard(reportname);
                    this._cacheManager.Set(cacheChartPermissionKey, dashbaordwidgets1, 120);
                }
                if (dashbaordwidgets1.Any(x => x.MODIFY_BY == "Static"))
                {
                    TempData["static"] = true;
                }

                foreach (var item in dashbaordwidgets1.Select(x => x.QUICKCAP))
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        var data = item.Split(',').ToList();
                        dashbaordwidgets.AddRange(data);
                    }
                }

            }


            if (widgets != null)
            {
                var mainWidgets = new List<MetricWidgetsModel>();
                if (this._cacheManager.IsSet(QueryCacheName))
                {
                    mainWidgets = this._cacheManager.Get<List<MetricWidgetsModel>>(QueryCacheName);
                    return View(mainWidgets);
                }
                else
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
                                var data = DataTableListHelper.ConvertTo<Spartline>(result).ToList();
                                query.SparkLine.AddRange(data as IEnumerable<Spartline>);
                                mainWidgets.Add(query);
                                continue;

                            }
                            if (query.QUICKTYPE == "A")
                            {
                                var result = _objectEntity.SqlQuery(query.sqlQuery);
                                var data = DataTableListHelper.ConvertTo<Spartline>(result).ToList();
                                query.SparkLine.AddRange(data as IEnumerable<Spartline>);
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
                                    if (query.MaxQueryValue == 0)
                                        query.MaxQueryValue = 1;
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
                    this._cacheManager.Set(QueryCacheName, mainWidgets, 120);
                    return View(mainWidgets);
                }


            }

            // ViewBag.widgets = widgets.Split(',').Where(s => !string.IsNullOrEmpty(s)).ToArray();
            return View();
        }
    }
}