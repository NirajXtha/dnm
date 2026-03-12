using NeoErp.Core;
using NeoErp.Core.Controllers;
using NeoErp.Core.Models;
using NeoErp.Core.Models.CustomModels;
using NeoErp.Core.Services;
using NeoErp.sales.Module.Models;
using NeoErp.Sales.Modules.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NeoErp.Core.Helpers;
using NeoErp.Core.Caching;
using NeoErp.sales.Module;
using System.Diagnostics;

namespace NeoErp.sales.Module.Controllers
{
    public class SalesHomeController : BaseController
    {

        //public ISalesRegister _salesRegister { get; set; }
        public ISalesProcessingMoniteringService _salesProcessingMoniterService { get; set; }
        private ISalesDashboardService _salesService { get; set; }
        private IDashBoardMetricService _dashBoardMetricservice { get; set; }
        private NeoErpCoreEntity _objectEntity;
        private IWorkContext _workContext;
        private ICacheManager _cacheManager { get; set; }
        private ISalesRegister _salesRegister { get; set; }
        public SalesHomeController(ISalesRegister salesRegister, ISalesProcessingMoniteringService salesProcessingMoniterService, ISalesDashboardService salesDashboardService, IDashBoardMetricService dashBoardMetricservice, NeoErpCoreEntity objectEntity, IWorkContext workContext, ICacheManager chacheManager)
        {
            this._salesRegister = salesRegister;
            this._salesProcessingMoniterService = salesProcessingMoniterService;
            this._salesService = salesDashboardService;
            this._dashBoardMetricservice = dashBoardMetricservice;
            this._objectEntity = objectEntity;
            this._workContext = workContext;
            this._cacheManager = chacheManager;
        }
        // GET: SalesReprt
        [Authorize]
        public ActionResult DashBoard(string id = null)
        {
            return View();
        }
        public ActionResult Page(string id = null)
        {
            if (id == null)
                return RedirectToAction("Dashboard");
            else
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
            string sortedWidgets = _salesService.GetDashboardWidgets(User.Identity.Name, reportName + "_sorted");
            string allWidgets = _salesService.GetDashboardWidgets(User.Identity.Name, reportName);
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

            if (intpage == 1)
            {
                model.AllchartList = widgetsList.Where(x => x.EndsWith("FullScreen")).ToList();
                if (model.AllchartList.Count <= 0)
                    model.AllchartList = widgetsList.Skip((intpage - 1) * pagesize).Take(pagesize).ToList();
            }
            else
            {
                model.AllchartList = widgetsList.Skip((intpage - 1) * pagesize).Take(pagesize).ToList();
            }


            model.CountTotalChart = widgetsList.Count;
            return View(model);
        }

        public ActionResult GetDashboardPartial(string partialViewName)
        {
            return PartialView(partialViewName);
        }

        //[OutputCache(Duration = 50)]
        public ActionResult DashBoardWidgets(string reportname = "")
        {
            var MainCancheName = $"neoCache_{reportname}";

            var QueryCacheName = $"neocacheQuery_{reportname}";

            TempData["static"] = false;
            var cacheNamagerKey = $"neoreopder_{reportname}";
            ViewBag.reportname = reportname;
            var Mainqueryvalue = 0M;
            var widgets = _dashBoardMetricservice.GetMetricList(true).Where(x => x.IsActive.Trim() == "Y").OrderBy(x => x.Orderno).ToList();
            var orderWidgets = new List<ReordingViewModel>();

            if (this._cacheManager.IsSet(cacheNamagerKey))
            {
                //orderWidgets = this._cacheManager.Get<List<ReordingViewModel>>(cacheNamagerKey);
                orderWidgets = _objectEntity.SqlQuery<ReordingViewModel>($"select REPORTNAME,WIDGETSID,XAXIS,YAXIS,HEIGHT,WIDTH from Web_widgets_reporder where reportname='{reportname}'").ToList();
                this._cacheManager.Set(cacheNamagerKey, orderWidgets, 120);
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
                    //dashbaordwidgets1 = this._cacheManager.Get<List<MeticorderModel>>(cacheChartPermissionKey);

                    dashbaordwidgets1 = _dashBoardMetricservice.GetDashboard(reportname);
                    this._cacheManager.Set(cacheChartPermissionKey, dashbaordwidgets1, 120);
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
                    //mainWidgets = this._cacheManager.Get<List<MetricWidgetsModel>>(QueryCacheName);
                    //return View(mainWidgets);

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

                                if (query.sqlQuery.Contains("\""))
                                {
                                    query.sqlQuery = query.sqlQuery.Replace("\"", "'");
                                }


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


        public ActionResult Index()
        {
            // var test = _salesRegister.SaleRegisters();
            return PartialView();
        }

        public ActionResult IndexReport()
        {
            var test = _salesRegister.GetChargesTitle();
            Debug.WriteLine(test);
            return PartialView(test);
        }
        public ActionResult VatRegistrationReport()
        {
            return PartialView();
        }

        public ActionResult SalesReport()
        {
            var test = _salesRegister.SaleRegisters();
            foreach (var a in test.GroupBy(x => x.SALES_NO).Select(grp => grp.First())
                   .ToList())
            {
                // var abc = _salesRegister.GetSalesCharges(a.SALES_NO);
                //            var abctest= abc
                //.GroupBy(x => x.Id)
                //.Select(x =>
                //    new
                //    {
                //        Id = x.Key,
                //        Quantity = x.Sum(y => y.
                //    });
                //  a.charges = abc;
            }

            return PartialView(test);
        }
        public ActionResult SalesDetailReport()
        {

            return PartialView();
        }
        public ActionResult FinalSalesReport()
        {

            var chargeTitle = _salesRegister.GetChargesTitle();
            ViewBag.itemCharges = _salesRegister.GetChargesItemTitle();
            return PartialView(chargeTitle);
        }
        public ActionResult SalesSummaryCustomerWise()
        {
            var chargeTitle = _salesRegister.GetChargesTitle();
            return PartialView(chargeTitle);
        }

        public ActionResult SalesRegister()
        {

            return PartialView();
        }
        public ActionResult ProductWiseSalesSummary()
        {
            return PartialView();
        }
        public ActionResult DateWiseSalesDetails()
        {
            return PartialView();
        }
        public ActionResult DateWiseSalesReturnDetails()
        {
            return PartialView();
        }
        public ActionResult BillWiseSalesSummary()
        {
            return PartialView();
        }
        //public ActionResult VatSalesRegisterNew()
        //{
        //    return PartialView();
        //}
        public ActionResult VatSalesRegister()
        {
            return PartialView();
        }
        public ActionResult AuditTrasactionReport()
        {
            return PartialView();
        }
        public ActionResult VatSalesReturnRegisterNew()
        {
            return PartialView();
        }
        public ActionResult VatPurchaseRegisterNew()
        {
            return PartialView();
        }
        public ActionResult VatPurchaseRegister()
        {
            return PartialView();
        }
        public ActionResult VatPurchaseReturnRegisterNew()
        {
            return PartialView();
        }
        public ActionResult BillWiseSalesReturnSummary()
        {
            return PartialView();
        }

        //MonthwiseCustProdNetSales
        public ActionResult MonthwiseCustProdNetSales()
        {
            return PartialView();
        }
        public ActionResult MonthwiseCustomerSalesCollection()
        {
            return PartialView();
        }
        public ActionResult BranchwiseEmployeeNetSales()
        {
            return PartialView();
        }
        public ActionResult BranchwiseProductNetSales()
        {
            return PartialView();
        }

        //Datewise Sales Report for jewellery
        public ActionResult DatewiseSalesDetail()
        {
            return PartialView();
        }
        public ActionResult PurchaseAboveOneLakhReport()
        {
            return PartialView();
        }
        //Grouth wise stock summary
        //public ActionResult GroupwiseStockSummary()
        //{
        //    return PartialView();
        //}
        //Bill wise Purchase Summary 
        public ActionResult BillwisePurchaseSummary()
        {
            return PartialView();
        }
        //Net Sales Reoport 
        public ActionResult NetSalesReports()
        {
            return PartialView();
        }
        //Net purchase Reoport 
        public ActionResult NetPurchaseReports()
        {
            return PartialView();
        }
        //Net purchase Reoport 
        public ActionResult PurchaseSummaryReports()
        {
            return PartialView();
        }
        //Net Sales Summary Reoport 
        public ActionResult NetSalesSummaryReports()
        {
            return PartialView();
        }
        //Order Tracking Report 
        public ActionResult OrderTrackingReports()
        {
            return PartialView();
        }
        public ActionResult SalesProcessingRegisterContainer()
        {
            return PartialView();
        }
        public ActionResult SalesProcessingMoniter(string search = null)
        {
            var model = new SalesMoniterViewModel();
            var date = DateTime.Now.ToString("yyyy-MM-dd");
            if (!string.IsNullOrEmpty(search))
            {
                date = search;
            }

            var companyCode = _workContext.CurrentUserinformation.Company;
            // approve sales order 
            model.approvedSalesOrder = _salesProcessingMoniterService.GetTodayApprovedsalesOrder(date, companyCode);
            model.loadedVechicleOut = _salesProcessingMoniterService.GetTodayLoadedVehicleOut(date, companyCode);
            model.loadingSlipGenerate = _salesProcessingMoniterService.GetTodayLoadingSlipGenerate(date, companyCode);
            model.totalBiil = _salesProcessingMoniterService.GetTodayPendingForDispatch(date, companyCode);
            model.totalSalesOrder = _salesProcessingMoniterService.GetTodaySalesOrder(date, companyCode);
            model.vehiclerRegistration = _salesProcessingMoniterService.GetTodayVehicleRegister(date, companyCode);
            model.distributionPurchaseorder = _salesProcessingMoniterService.GetTodayDistributionPurchaseOrderCount(date, companyCode);
            model.pendingForDispatch = _salesProcessingMoniterService.GetDispatchMangement(date, companyCode);
            model.vechicleIn = _salesProcessingMoniterService.GetVechicalIN(date, companyCode);
            return PartialView("~/Areas/NeoErp.sales.Module/Views/SalesHome/SalesProcessingMoniter.cshtml", model);
        }
        [HttpGet]
        public JsonResult SalesProcessingMoniterGet(string search = null)
        {
            var model = new SalesMoniterViewModel();
            var date = DateTime.Now.ToString("yyyy-MM-dd");
            if (!string.IsNullOrEmpty(search))
            {
                date = search;
            }

            var companyCode = _workContext.CurrentUserinformation.Company;
            // approve sales order 
            model.approvedSalesOrder = _salesProcessingMoniterService.GetTodayApprovedsalesOrder(date, companyCode);
            model.loadedVechicleOut = _salesProcessingMoniterService.GetTodayLoadedVehicleOut(date, companyCode);
            model.loadingSlipGenerate = _salesProcessingMoniterService.GetTodayLoadingSlipGenerate(date, companyCode);
            model.totalBiil = _salesProcessingMoniterService.GetTodayPendingForDispatch(date, companyCode);
            model.totalSalesOrder = _salesProcessingMoniterService.GetTodaySalesOrder(date, companyCode);
            model.vehiclerRegistration = _salesProcessingMoniterService.GetTodayVehicleRegister(date, companyCode);
            model.distributionPurchaseorder = _salesProcessingMoniterService.GetTodayDistributionPurchaseOrderCount(date, companyCode);
            model.pendingForDispatch = _salesProcessingMoniterService.GetDispatchMangement(date, companyCode);
            model.vechicleIn = _salesProcessingMoniterService.GetVechicalIN(date, companyCode);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CustomerWisePriceList()
        {
            return PartialView();
        }

        public ActionResult ProductWisePriceList()
        {
            return PartialView();
        }
        public ActionResult CustomerWiseProfitAnalysisReport()
        {
            return PartialView("~/Areas/NeoErp.sales.Module/Views/AnalysisReport/CustomerWiseProfitAnalysisReport.cshtml");
        }
        public ActionResult ProfitAnalysisChart()
        {
            return PartialView();
        }
        public ActionResult PendingVoucher()
        {
            return PartialView();
        }

        public ActionResult SalesRegisterPrivot()
        {
            return PartialView();
        }

        public ActionResult DailySalesReport()
        {

            return PartialView();
        }

        public ActionResult WeeklyExpensesAnalysisReport()
        {
            return PartialView("~/Areas/NeoErp.sales.Module/Views/AnalysisReport/WeeklyExpensesAnalysisReport.cshtml");
        }


        public ActionResult WeeklyVendorPaymentAnalysisReport()
        {
            return PartialView("~/Areas/NeoErp.sales.Module/Views/AnalysisReport/WeeklyVendorPaymentAnalysisReport.cshtml");
        }

        public ActionResult MaterializeReport()
        {
            return PartialView();
        }
        public ActionResult MaterilizedViewReport()
        {
            //var chargeTitle = _salesRegister.GetChargesTitle();
            //ViewBag.itemCharges = _salesRegister.GetChargesItemTitle();
            return PartialView();
        }
        public ActionResult IRDVatRegistration()
        {
            return PartialView();
        }
        public ActionResult AgentWiseSalesRegister()
        {
            return PartialView();
        }
        public ActionResult PurchaseReturnRegister()
        {
            return PartialView();
        }
        public ActionResult PurchaseRegister()
        {
            return PartialView();
        }
        public ActionResult PurchasePendingReport()
        {
            return PartialView();
        }
        public ActionResult PurchaseOrderReport()
        {
            return PartialView();
        }
        public ActionResult PurchaseVatRegistrationReport()
        {
            return PartialView();
        }
        #region newReports
        public ActionResult SalesExciseRegister()
        {
            return PartialView();
        }
        public ActionResult AuditTrailReport()
        {
            return PartialView();
        }
        //Groupstock Summary Report 
        public ActionResult GroupstockSummaryReport()
        {
            return PartialView();
        }


        #endregion
    }
}