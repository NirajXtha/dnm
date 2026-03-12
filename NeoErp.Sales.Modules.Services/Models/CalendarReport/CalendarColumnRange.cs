using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.CalendarReport
{
    public class CalendarColumnRange
    {
        public CalendarColumnRange()
        {
            this.ColumnRangeList = new List<CalendarYearRangeData>();
            FirstColumnlist = new List<FirstHorizontalPeriodModel>();
            SecondColumnlist = new List<SecondHOrizontalPeriodModel>();
        }
        public string FirstHorizontalPeriod { get; set; }
        public string SecondHorizontalPeriod { get; set; }
        public string AsOnDate { get; set; }
        public string Description { get; set; }
        public string SubColumnFirst { get; set; }
        public string SubColumnSecond { get; set; }
        public decimal? Total { get; set; }
        public int Id { get; set; }
        public string ReportName { get; set; }
        public List<FirstHorizontalPeriodModel> FirstColumnlist { get; set; }

        public List<SecondHOrizontalPeriodModel> SecondColumnlist { get; set; }


        //public string FromDateString { get; set; }
        //public string ToDateString { get; set; } = string.Empty;
        //public DateTime? FromDate { get; set; }
        //public DateTime ToDate { get; set; }
        public List<CalendarYearRangeData> ColumnRangeList { get; set; }

        public class CalendarYearRangeData
        {
            public string YearRange { get; set; }
            public List<CalendarQuarterRangeData> QuarterRangeList { get; set; }
        }

        public class CalendarQuarterRangeData
        {
            public string QuarterRange { get; set; }
            public List<CalendarMonthRangeData> MonthRangeList { get; set; }
        }

        public class CalendarMonthRangeData
        {
            public string WeekRange { get; set; }
            public string DayRange { get; set; }
        }
    }

    public class FirstHorizontalPeriodModel
    {
        public string HeaderName { get; set; }
        public List<SecondHOrizontalPeriodModel> secondHorizontalPeriodModel { get; set; }

        public FirstHorizontalPeriodModel()
        {
            secondHorizontalPeriodModel = new List<SecondHOrizontalPeriodModel>();
        }

    }
    public class SecondHOrizontalPeriodModel
    {
        public string HeaderName { get; set; }
        public List<ThirdHorizontalColumnModel> ThirdHorizontalColumn { get; set; }
        public SecondHOrizontalPeriodModel()
        {
            ThirdHorizontalColumn = new List<ThirdHorizontalColumnModel>();
        }
    }
    public class ThirdHorizontalColumnModel
    {
        public string HeaderName { get; set; }
        public List<ForthHorizontalColumnModel> forthHorizontalColumn { get; set; }

        public ThirdHorizontalColumnModel()
            {
            forthHorizontalColumn = new List<ForthHorizontalColumnModel>();
            }
    }
    public class ForthHorizontalColumnModel
    {
        public string HeaderName { get; set; }
        public string HeaderVlaue { get; set; }
       
    }
}
