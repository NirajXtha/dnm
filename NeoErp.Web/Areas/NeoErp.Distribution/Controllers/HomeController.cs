using NeoErp.Core.Models;
using NeoErp.Core.Services;
using NeoErp.Distribution.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LinqToExcel;

namespace NeoErp.Distribution.Controllers
{
    public class HomeController : Controller
    {
        public IDistributionService _service { get; set; }
        private IDashBoardMetricService _dashBoardMetricservice { get; set; }
        private NeoErpCoreEntity _objectEntity;
        public HomeController(IDistributionService service, NeoErpCoreEntity objectEntity, IDashBoardMetricService dashBoardMetricservice)
        {
            _service = service;
            this._dashBoardMetricservice = dashBoardMetricservice;
            this._objectEntity = objectEntity;
        }
        // GET: Home
        public ActionResult Index()
        {
         
            return  PartialView();
        }

        public ActionResult Dashboard()
        {
            return View();
        }


        public ActionResult ChartDashboard()
        {
            return View();
        }

        public ActionResult DashBoardWidgets()
        {
            

            var widgets = _dashBoardMetricservice.GetMetricList(true).Where(x => x.IsActive.Trim() == "Y").OrderBy(x => x.Orderno).ToList();
            if (widgets != null)
            {
                foreach (var query in widgets)
                {
                    try
                    {
                        if (query.sqlQuery.ToLower().Trim().Contains("count"))
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
                    catch (Exception ex)
                    {
                        continue;
                    }

                    //var result= _objectEntity.SqlQuery<object>(query.sqlQuery).FirstOrDefault();
                    //var resulttest = _objectEntity.SqlQuery<string>(query.sqlQuery).FirstOrDefault();

                }
                return View(widgets);
            }

            // ViewBag.widgets = widgets.Split(',').Where(s => !string.IsNullOrEmpty(s)).ToArray();
            return View();
        }


        public ActionResult VisitSummaryReport()
        {
            return PartialView();
        }
        public ActionResult VisitSummaryBrandingReport()
        {
            return PartialView();
        }
        public ActionResult VisitSummaryReportAll()
        {
            return PartialView();
        }
        public ActionResult UserSetup()
        {
            return View();
        }

        public ActionResult DashboardLayout()
        {
            return View();
        }

       public ActionResult PurchaseReport()
        {
            return View();
        }

        


        //public ActionResult DashBoardWidgets()
        //{
        //    var widgets = _dashBoardMetricservice.GetMetricList(true).Where(x => x.IsActive.Trim() == "Y").OrderBy(x => x.Orderno).ToList();
        //    if (widgets != null)
        //    {
        //        foreach (var query in widgets)
        //        {
        //            try
        //            {
        //                if (query.sqlQuery.ToLower().Trim().Contains("count"))
        //                {
        //                    var result = _objectEntity.SqlQuery<int?>(query.sqlQuery).FirstOrDefault();
        //                    if (result != null)
        //                    {
        //                        query.aggValue = String.Format("{0:#,##0.##}", result);
        //                    }
        //                    else
        //                    {
        //                        query.aggValue = "0";
        //                    }
        //                }
        //                else
        //                {
        //                    var result = _objectEntity.SqlQuery<decimal?>(query.sqlQuery).FirstOrDefault();
        //                    if (result != null)
        //                    {
        //                        query.aggValue = String.Format("{0:#,##0.##}", result);
        //                    }
        //                    else
        //                    {
        //                        query.aggValue = "0";
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                continue;
        //            }

        //            //var result= _objectEntity.SqlQuery<object>(query.sqlQuery).FirstOrDefault();
        //            //var resulttest = _objectEntity.SqlQuery<string>(query.sqlQuery).FirstOrDefault();

        //        }
        //        return View(widgets);
        //    }

        //    // ViewBag.widgets = widgets.Split(',').Where(s => !string.IsNullOrEmpty(s)).ToArray();
        //    return View();
        //}
    }
}