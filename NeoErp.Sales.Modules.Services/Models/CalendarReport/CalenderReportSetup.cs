using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.CalendarReport
{
  public  class CalenderReportSetup
    {
        public string report_edesc { get; set; }
        public string source_data_entity { get; set; }
        public string form_filter_flag { get; set;  }
        public string form_code { get; set; }
        public string calendar_type_flag { get; set; }
        public DateTime? from_date { get; set; }
        public DateTime? to_date { get; set; }
        public string first_hor_period_flag { get; set; }
        public string second_hor_period_flag { get; set; }
        public string vertical_table_name { get; set; }
        public string group_wise_flag { get; set; }
        public string source_data_column { get; set; }
        public string transaction_type { get; set; }
        public string source_data_column2 { get; set; }
        public string column1_heading { get; set; }
        public string column2_heading { get; set; }
        public string MULTI_REPORT { get; set; }
        public string OPEN_CLS_FLAG { get; set; }

    }

    public class DatabaseColumn
    {
        public string COLUMN_NAME { get; set; }
    }
}
