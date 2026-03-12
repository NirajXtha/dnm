
using NeoErp.Core;
using NeoErp.Core.Caching;
using NeoErp.Core.Helpers;
using NeoErp.Sales.Modules.Services.Models;
using NeoErp.Sales.Modules.Services.Models.CalendarReport;
using NeoErp.Sales.Modules.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoErp.sales.Module.Controllers.Api
{
    public class CalendarReportController : ApiController
    {
        public ICalendarReportService _calendarReport;
        private ICacheManager _cacheManager;
        private IWorkContext _workContext;

        public CalendarReportController(ICalendarReportService calendarReport,ICacheManager cacheManager,IWorkContext workContext)
        {
            this._calendarReport = calendarReport;
            this._cacheManager = cacheManager;
            this._workContext = workContext;
        }

        //Get Calendar Report Title List
        [HttpGet]
        public List<ReportModel> GetReportTitle()
        {
            var reportTitleList = _calendarReport.GetReportTitle().ToList();
            return reportTitleList;
        }

        [HttpGet]
        public List<Tree> GetReportTitleNodes()
        {
            var userinfo = this._workContext.CurrentUserinformation;
            //var reportTitleNodeList = _calendarReport.GetReportTitleNodes().ToList();
            var reportTitleNodeList = _calendarReport.GetReportTitleNodes(userinfo).ToList();
            var reportTitleNodes = new List<Tree>();

            foreach(var report in reportTitleNodeList)
            {
                reportTitleNodes.Add(new Tree()
                {
                    Level = report.LEVEL,
                    ReportCode = report.ReportCode,
                    ReportName = report.ReportName,
                    MasterReportCode = report.MasterReportCode,
                    PreReportCode = report.PreReportCode,
                    hasChildren = report.GroupFlag == "G" ? true:false
                });
            }

            return reportTitleNodes;
        }

        
        
        [HttpPost]
        public List<CalendarDataViewModel> GetCalendarReport(CalendarFilterModel model)
        {
            return this._calendarReport.GetCalendarViewReport(model).ToList();
        }

        [HttpPost]
        public List<CalendarCustomerDataModel> GetWeekCalendarReport(CalendarFilterModel model)
        {
            var userinfo = this._workContext.CurrentUserinformation;
            //var modelCalendarReport= this._calendarReport.GetCalenderReport(model);
            var modelCalendarReport = this._calendarReport.GetCalenderReport(model,userinfo);

            if (model.ShowGroup)
            {
                var Report = modelCalendarReport.Where(x => x.parentId == model.Id).ToList();
                return Report;
            }

            return modelCalendarReport;
        }

        [HttpGet]
        public List<ChartOfAccountSetupModel> GetAccountFilterTreeList(GridFilters filter)
        {
            var userinfo = this._workContext.CurrentUserinformation;
            var accountSetup = _calendarReport.ChartOfAccountSelect(userinfo).ToList();// _salesRegister.SaleRegisterCustomers().ToList();
            return accountSetup;
        }

        [HttpGet]
        public List<ChartOfAccountSetupModel> GetAccountFilterTreeListData()
        {
            var userinfo = this._workContext.CurrentUserinformation;
            //var allAccountList = _calendarReport.ChartOfAccountSelect().ToList();
            var allAccountList = _calendarReport.ChartOfAccountSelect(userinfo).ToList();
            var accountNodes = new List<ChartOfAccountSetupModel>();

            foreach (var acc in allAccountList)
            {
                accountNodes.Add(new ChartOfAccountSetupModel()
                {
                    ACC_CODE = acc.ACC_CODE,
                    ACC_EDESC = acc.ACC_EDESC,
                    ACC_TYPE_FLAG = acc.ACC_TYPE_FLAG,
                    MASTER_ACC_CODE = acc.MASTER_ACC_CODE,
                    PRE_ACC_CODE = acc.PRE_ACC_CODE,
                    LEVEL = acc.LEVEL,
                    hasAccount = acc.ACC_TYPE_FLAG == "N" ? true : false
                });
            }

            return accountNodes;
        }

        [HttpGet]
        public List<ChartOfAccountSetupModel> GetAllAccountFilterByACCId(string acc_code, string level, string master_code)
        {
            var userinfo = this._workContext.CurrentUserinformation;
            //var allAccountList = _calendarReport.GetAccountListByAccCode(level, master_code).ToList();
            var allAccountList = _calendarReport.GetAccountListByAccCode(level, master_code,userinfo).ToList();
            var AccountNodes = new List<ChartOfAccountSetupModel>();

            foreach (var acc in allAccountList)
            {
                AccountNodes.Add(new ChartOfAccountSetupModel()
                {
                    ACC_CODE = acc.ACC_CODE,
                    ACC_EDESC = acc.ACC_EDESC,
                    ACC_TYPE_FLAG = acc.ACC_TYPE_FLAG,
                    MASTER_ACC_CODE = acc.MASTER_ACC_CODE,
                    PRE_ACC_CODE = acc.PRE_ACC_CODE,
                    LEVEL = acc.LEVEL,
                    hasAccount = acc.ACC_TYPE_FLAG == "N" ? true : false
                });
            }

            return AccountNodes;
        }
    }
}