using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.sales.Module.Models
{
    public class DashboardViewModel
    {

        public List<string> chartList { get; set; }
        public List<string> AllchartList { get; set; }
        public int CountTotalChart { get; set; } = 0;
        public bool isMore { get; set; } = true;
        public DashboardViewModel()
        {
            chartList = new List<string>();
            AllchartList = new List<string>();
        }
    }
    public class ReordingViewModel
    {
        public string WIDGETSID { get; set; }
        public string REPORTNAME { get; set; }
        public string XAXIS { get; set; }
        public string YAXIS { get; set; }
        public string HEIGHT { get; set; }
        public string WIDTH { get; set; }

    }
}