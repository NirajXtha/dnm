using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models
{
    public class CalendarReportModel
    {

    }

    public class ReportModel
    {
        public string ReportCode { get; set; }
        public string ReportName { get;  set; }
    }

    public class ReportSetupModel
    {
        public int LEVEL { get; set; }
        public string ReportCode { get; set; }
        public string ReportName { get; set; }
        public string MasterReportCode { get; set; }
        public string PreReportCode { get; set; }
        public string GroupFlag { get; set; }
        public IEnumerable<ReportSetupModel> Items { get; set; }
    }

    public class Tree
    {
        public int Level { get; set; }
        public string ReportCode { get; set; }
        public string ReportName { get; set; }
        public string MasterReportCode { get; set; }
        public string PreReportCode { get; set; }
        public bool hasChildren { get; set; }
    }
}
