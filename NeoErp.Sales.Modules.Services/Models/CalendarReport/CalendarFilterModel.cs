using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.CalendarReport
{
    public class CalendarFilterModel
    {
        public string formDate { get; set; } = string.Empty;
        public string toDate { get; set; } = string.Empty;

        public int? Id { get; set; }
        public string ReportId { get; set; }

        public string FirstHorizontalPeriod { get; set; } = PeriodFilter.Year.ToString();

        public string SecondHorizontalPeriod { get; set; } = PeriodFilter.Month.ToString();
        // bikalp change 
        public string CalenderTypeValue { get; set; } = CalenderType.Bs.ToString();
        //end Change

        public string ReportName { get; set; }
        public bool ShowGroup { get; set; } = false;

    }
}
