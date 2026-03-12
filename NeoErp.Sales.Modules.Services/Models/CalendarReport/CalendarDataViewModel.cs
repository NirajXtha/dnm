using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.CalendarReport
{
    public class CalendarDataViewModel
    {
        public CalendarDataViewModel()
        {
            this.RangeColumnData = new List<CalendarColumnRangeData>();
        }
        public string Description { get; set; }
        public decimal? Total { get; set; }
        public decimal? NetAmount { get; set; }
        public decimal? Quantity { get; set; }
        public string Id { get; set; }
        public int ParentId { get; set; }
        public string MasterCode { get; set; }
        public string PreMasterCode { get; set; }
        public bool HasChildren { get; set; } = false;
        public List<CalendarColumnRangeData> RangeColumnData { get; set; }

        public string week { get; set; }
        public string month { get; set; }
        public string year { get; set; }


        public class CalendarColumnRangeData
        {
            public string ColumnRangeNameFirst { get; set; }
            public string ColumnRangeNameSecond { get; set; }            
            public string ColumnRangeNameThird { get; set; }
            public decimal? NetAmount { get; set; }
            public string SubColumnFirst { get; set; }
            public string SubColumnSecond { get; set; }
        }
    }
}
