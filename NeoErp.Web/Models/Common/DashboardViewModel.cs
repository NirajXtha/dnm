using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Models.Common
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
}