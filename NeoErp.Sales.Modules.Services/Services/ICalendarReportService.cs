using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoErp.Core.Models;
using NeoErp.Sales.Modules.Services.Models;
using NeoErp.Sales.Modules.Services.Models.CalendarReport;
using NeoErp.Core.Domain;

namespace NeoErp.Sales.Modules.Services.Services
{
    public interface ICalendarReportService
    {
        List<ReportModel> GetReportTitle();
        List<ReportSetupModel> GetReportTitleNodes();
        List<ReportSetupModel> GetReportTitleNodes(User userinfo);
        IEnumerable<CalendarColumnRange> GenerateColumns(string firstHorizontalPeriod, string secondHorizontalPeriod, string asOnDate);
        IEnumerable<CalendarDataViewModel> GetCalendarViewReport(CalendarFilterModel model);

        List<CalendarCustomerDataModel> GetWeekWiseCalenderReport(string calenderType = "Ad");
        CalendarColumnRange GenerateColumnsGeneric(CalendarFilterModel calenderFilter);
        List<CalendarCustomerDataModel> GetDaysWiseCalenderReport(string calenderType = "A");
        List<CalendarCustomerDataModel> GetCalenderReport(CalendarFilterModel filter);
        List<CalendarCustomerDataModel> GetCalenderReport(CalendarFilterModel filter,User userinfo);
        List<CalenderReportSetup> GetCalenderSetup(int reportid = 0);
        string GetVerticalTableNameByTitleId(int titleId);
        List<ChartOfAccountSetupModel> ChartOfAccountSelect();
        List<ChartOfAccountSetupModel> ChartOfAccountSelect(User userinfo);
        List<ChartOfAccountSetupModel> GetAccountListByAccCode(string level, string master_code);
        List<ChartOfAccountSetupModel> GetAccountListByAccCode(string level, string master_code,User userinfo);
    }
}
