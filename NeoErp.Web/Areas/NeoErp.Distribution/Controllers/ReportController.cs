using NeoErp.Core;
using NeoErp.Distribution.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoErp.Distribution.Controllers
{
    public class ReportController : Controller
    {
        public IDistributionService _distributionService;
        private IWorkContext _workContext;

        public ReportController(IDistributionService distributionService, IWorkContext workContext)
        {
            this._distributionService = distributionService;
            this._workContext = workContext;
        }
        public ActionResult SurveyReport()
        {
            return PartialView();
        }
        public ActionResult TargetSetup()
        {
            return PartialView();
        }
        public ActionResult FarmerProblemRecommendation()
        {
            return PartialView();
        }
        public ActionResult SurveyReportAata()
        {
            return PartialView();
        }
        public ActionResult SurveyReportTabularJGI()
        {
            return PartialView();
        }
        public ActionResult SurveyReportAataTabular()
        {
            return PartialView();
        }
        public ActionResult SurveyReportBrandingTabular()
        {
            return PartialView();
        }
        public ActionResult SurveyReportTabularDynamic()
        {
            return PartialView();
        }
        public ActionResult MerchandisingStockReport()
        {
            return PartialView();
        }
        public ActionResult QuestionaireReport()
        {
            return PartialView();
        }
        public ActionResult DistResellerStockReport()
        {
            return PartialView();
        }
        public ActionResult DistResellerBrandItemStockReport()
        {
            return PartialView();
        }
        public ActionResult DistDistributorBrandItemStockReport()
        {
            return PartialView();
        }
        public ActionResult CollectionsReport()
        {
            return PartialView();
        }

        public ActionResult getSalesOrderDetails()
        {
            return PartialView();
        }

        public ActionResult performanceReport()
        {

            return PartialView();
        }

        public ActionResult performanceReport_Global()
        {
            return PartialView();
        }

        public ActionResult ResellerOrderSummaryReport()
        {

            return PartialView();
        }

        public ActionResult SalesPersonPO()
        {
            return PartialView();
        }

        public ActionResult ItemCumulativeReport()
        {
            return PartialView();
        }

        public ActionResult MrVisitReportMap()
        {
            return PartialView();
        }
        
        public ActionResult EOD()
        {
            return PartialView();
        }
        public ActionResult ClaimReport()
        {
            return PartialView();
        }

        public ActionResult CalendarRouteReport()
        {
            return PartialView();
        }

        public ActionResult SalesOrderDetailsDaily()
        {
            return PartialView();
        }

        public ActionResult AttendanceReport()
        {
            return PartialView();
        }
        public ActionResult BrandingAttendanceReport()
        {
            return PartialView();
        }
        public ActionResult MRVisitTrackingMap()
        {
            return PartialView();
        }

        public ActionResult MRMap()
        {
            return PartialView();
        }

        public ActionResult EmployeeWisePerformance()
        {
            return PartialView();
        }

        public ActionResult ItemsMinMax()
        {
            return PartialView();
        }

        public ActionResult EmployeeWisePerformanceNew()
        {
            var columns = _distributionService.GetDynamicFields(_workContext.CurrentUserinformation);
            return PartialView(columns);
        }

        public ActionResult DailyActivityReport()
        {
            return PartialView();
        }

        public ActionResult SalesRegister()
        {
            return PartialView();
        }

        public ActionResult OutletClosingStock()
        {
            return PartialView();
        }
        public ActionResult ResellerClosingStockReport()
        {
            return PartialView();
        }

        public ActionResult VisitImageGallery()
        {
            return PartialView();
        }
        public ActionResult BrandingVisitImageGallery()
        {
            return PartialView();
        }
        public ActionResult VisitImageGalleryCondition()
        {
            return PartialView();
        }
        public ActionResult VisitImageContaractGallery()
        {
            return PartialView();
        }

        public ActionResult CompItemReport()
        {
            var items = _distributionService.CompItems(_workContext.CurrentUserinformation);
            return PartialView(items);
        }

        public ActionResult CompItemReportMonthly()
        {
            var items = _distributionService.CompItems(_workContext.CurrentUserinformation);
            return PartialView(items);
        }

        public ActionResult SPDistanceReport()
        {
            return PartialView();
        }

        public ActionResult SPRouteReport()
        {
            return PartialView();
        }

        public ActionResult ResellerDetailReport()
        {
            return PartialView();
        }
        public ActionResult SalesPersonDetailReport()
        {
            var columns = _distributionService.GetDynamicFields(_workContext.CurrentUserinformation);
            return PartialView(columns);
        }
        public ActionResult SalesPersonDetailIndivisual()
        {
            var columns = _distributionService.GetDynamicFields(_workContext.CurrentUserinformation);
            return PartialView(columns);
        }
        public ActionResult BrandingSalesPersonDetailReport()
        {
            var columns = _distributionService.GetDynamicFields(_workContext.CurrentUserinformation);
            return PartialView(columns);
        }
        
        public ActionResult VisitsummaryTimeReport()
        {
            return PartialView();
        }

        public ActionResult DeviceLog()
        {
            return PartialView();
        }
        public ActionResult AllVisitSummary()
        {
            return PartialView();
        }

        public ActionResult SummaryReport()
        {
            return PartialView();
        }
        public ActionResult EmpAttendanceCalendarReport()
        {
            return PartialView();
        }
        public ActionResult EmployeeActivityReport()
        {
            return PartialView();
        }
        public ActionResult webQuestionnaireReport()
        {
            return PartialView();
        }
        public ActionResult WebSurveyReportDynamic()
        {
            return PartialView();
        }

        public ActionResult AttendanceSummary()
        {
            return PartialView();
        }
        public ActionResult EmployeePerformanceNewDetail()
        {
            var columns = _distributionService.GetDynamicFields(_workContext.CurrentUserinformation);
            return PartialView(columns);
        }
        
        public ActionResult StockSummary()
        {
            return PartialView();
        }

        public ActionResult SchemeReport()
        {
            return PartialView();
        }
        
        public ActionResult SchemeWiseReport()
        {
            return PartialView();
        } 
        public ActionResult SchemeChecker()
        {
            return PartialView();
        }

        public ActionResult LedgerImport()
        {
            return PartialView();
        }
        
        public ActionResult DeliveryReport()
        {
            return PartialView();
        }

        public ActionResult ledgerAccess()
        {
            return PartialView();
        }


    }
}