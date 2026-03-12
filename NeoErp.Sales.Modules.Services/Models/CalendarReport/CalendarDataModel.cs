using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.CalendarReport
{
    public class CalendarDataModel
    {

    }

    public class CalendarCustomerDataModel
    {
        public Int64? id { get; set; }
        public Int64? parentId { get; set; }
        public string PreCodeWithoutConversion { get; set; }
        public string CustomerName { get; set; }

        public string  CustomerCode { get; set; }

        public string MonthName { get; set; }

        public string YearName { get; set; }

        public decimal? Quantity { get; set; }

        public DateTime SalesDate { get; set; }

        public decimal? Price { get; set; }

        public decimal? Total { get; set; }
        public bool hasChildren { get; set; }
        public List<CustomerWeekWise> additionalData { get; set; }
        public CalendarCustomerDataModel()
        {
            additionalData = new List<CustomerWeekWise>();
        }
    }

    public class CustomerWeekWise
    { 
        public string yearname { get; set; }
        public string MonthName { get; set; }
        public string weekName { get; set; }
        public decimal? totalQty { get; set; }
        public decimal? TotalAmount { get; set; }
    }

    public class WeekNumberByMonth
    {
        public string WeekNumber { get; set; }
        public string MonthName { get; set; }
        public string Year { get; set; } 
    }

    public class MonthNumberByYear
    {
        public string MonthNumber { get; set; }
        public string MonthName { get; set; }
        public string Year { get; set; }
    }

    public class CalenderDB
    {
        public string Miti { get; set; }
        public DateTime AdDate { get; set; }
        public string Nepalimonth { get; set; }
        public string month { get; set; }
        public string Nepaliyear { get; set; }
        public string Englishyear { get; set; }
        public Decimal weeks { get; set; }
        public string NepaliMonthInt { get; set; }
        public string MonthInt { get; set; }
    }
}
