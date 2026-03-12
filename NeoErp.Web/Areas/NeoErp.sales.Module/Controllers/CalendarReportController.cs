using NeoErp.Core.Controllers;
using NeoErp.Core.Models;
using NeoErp.Sales.Modules.Services.Models.CalendarReport;
using NeoErp.Sales.Modules.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace NeoErp.sales.Module.Controllers
{
    public class CalendarReportController : BaseController
    {
        // GET: CalendarReport
        private ICalendarReportService _calendarReportService;
        public CalendarReportController(ICalendarReportService calendarReportService)
        {
            this._calendarReportService = calendarReportService;
        }

        public ActionResult Index()
        {
            return PartialView();
        }

        public ActionResult weekIndex()
        {
            return PartialView();
        }
        public ActionResult FinalWeekIndex()
        {
            return PartialView();
        }
        public ActionResult CalendarReportIndex(int id)
        {
            ViewBag.TitleId = id;
            return PartialView("FinalWeekIndex");
        }
        public ActionResult CalendarDynamicColumnView(CalendarFilterModel model)
        {           
            var dynamicColumns = this._calendarReportService.GenerateColumns(model.FirstHorizontalPeriod, model.SecondHorizontalPeriod, model.formDate);
            return PartialView(dynamicColumns);
        }

        public ActionResult CalendarWeekDynamicColumn(CalendarFilterModel model)
        {
            var dynamicColumns = this._calendarReportService.GenerateColumns(model.FirstHorizontalPeriod, model.SecondHorizontalPeriod, model.formDate);
            

            //If Year and Week chosen, return week partial view
            if (model.FirstHorizontalPeriod == ((char)PeriodFilter.Year).ToString() && model.SecondHorizontalPeriod == ((char)PeriodFilter.Week).ToString())
            {
                ViewData["header"] = RenderPartialViewToString("_CalendarHeader", dynamicColumns.FirstOrDefault());
                return PartialView(dynamicColumns);
            }
            //If Year and Month chosen, return month partial view
            if (model.FirstHorizontalPeriod == ((char)PeriodFilter.Year).ToString() && model.SecondHorizontalPeriod == ((char)PeriodFilter.Month).ToString())
            {
                ViewData["header"] = RenderPartialViewToString("_CalendarMonthHeader", dynamicColumns.FirstOrDefault());
                return PartialView("CalendarMonthDynamicColumn",dynamicColumns);
            }
            //If Year and Quarter chosen, return quarter partial view
            if (model.FirstHorizontalPeriod == ((char)PeriodFilter.Year).ToString() && model.SecondHorizontalPeriod == ((char)PeriodFilter.Quarter).ToString())
            {
                ViewData["header"] = RenderPartialViewToString("_CalendarQuarterHeader", dynamicColumns.FirstOrDefault());
                return PartialView("CalendarQuarterDynamicColumn", dynamicColumns);
            }
            return PartialView(dynamicColumns);
        }

        public ActionResult NewCalendarWeekDynamicColumn(CalendarFilterModel model)
        {
            var dynamicColumns = this._calendarReportService.GenerateColumnsGeneric(model);

            //If Year and Month chosen, return month partial view
            if (model.FirstHorizontalPeriod == ((char)PeriodFilter.Year).ToString() && model.SecondHorizontalPeriod == ((char)PeriodFilter.Month).ToString())
            {
                ViewData["header"] = RenderPartialViewToString("_CalendarMonthHeader", dynamicColumns);
                return PartialView("CalendarMonthDynamicColumn", dynamicColumns);
            }
            //If Year/Month and Week chosen, return default week partial view
            else if ((model.FirstHorizontalPeriod == ((char)PeriodFilter.Year).ToString() || model.FirstHorizontalPeriod == ((char)PeriodFilter.Month).ToString()) && model.SecondHorizontalPeriod == ((char)PeriodFilter.Week).ToString())
            {
                ViewData["header"] = RenderPartialViewToString("_CalendarHeader", dynamicColumns);
                return PartialView(dynamicColumns);
            }
            //If Year/Month/Week as FirstHorizontalPeriod and Day as SecondHorizontalPeriod chosen, return default week partial view
            else if ((model.FirstHorizontalPeriod == ((char)PeriodFilter.Year).ToString() || model.FirstHorizontalPeriod == ((char)PeriodFilter.Month).ToString() || model.FirstHorizontalPeriod == ((char)PeriodFilter.Week).ToString()) && model.SecondHorizontalPeriod == ((char)PeriodFilter.Day).ToString())
            {
                ViewData["header"] = RenderPartialViewToString("_CalendarHeader", dynamicColumns);
                return PartialView(dynamicColumns);
            }

            return PartialView(dynamicColumns);
        }

        public ActionResult GetTreeListFromTitle(int TitleId)
        {
            try
            {
                AdvancedFilterSettingsModel settingFilter = new AdvancedFilterSettingsModel();               
                settingFilter.IsPopUp = false;
                string vertical_table_name = this._calendarReportService.GetVerticalTableNameByTitleId(TitleId);
                switch (vertical_table_name)
                {
                    case "SA_CUSTOMER_SETUP":
                        settingFilter.ShowCustomerFilter = true;
                        return PartialView("Controls/_AdvancedFilter", settingFilter);
                    case "IP_ITEM_MASTER_SETUP":
                        settingFilter.ShowProductFilter = true;
                        return PartialView("Controls/_AdvancedFilter", settingFilter);
                    case "IP_SUPPLIER_SETUP":
                        settingFilter.ShowSupplierFilter = true;
                        return PartialView("Controls/_AdvancedFilter", settingFilter);
                    case "FA_CHART_OF_ACCOUNTS_SETUP":
                        settingFilter.ShowAccountFilter = true;
                        return PartialView("Controls/_AdvancedFilter", settingFilter);
                    default:
                        break;
                }
                return Json("No Record found.", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json("Error occured on processing, ErrorMsg: " + e.Message,JsonRequestBehavior.AllowGet);
            }
        }
    }
}