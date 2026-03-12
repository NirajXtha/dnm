using NeoErp.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Models.Common
{
    public class ReportSettingModel : ReportFiltersModel
    {
        public string reportName { get; set; }
        public string parentReportName { get; set; }
        public string hiddenColumnName { get; set; }
        public string defaultPageSize { get; set; }
        public string itemPerPage { get; set; }
        public string dateFormat { get; set; }
        public string isFavroite { get; set; }
        public string virtualPath { get; set; }
        public string groupField { get; set; }
        public string groupDir { get; set; }
        public string icon { get; set; }
        public string color { get; set; }
        public string parentMenu { get; set; }
        public string description { get; set; }
        public string dateTitle { get; set; }
        public string modelABBR { get; set; }
        public string theme { get; set; }
        public string moduleCode { get; set; }
        public string HideOption { get; set; }
        public string User { get; set; }
        public string Report { get; set; }
    }
    public class FiscalYearDBModel
    {
        public string FiscalYear { get; set; }
        public string DBName { get; set; }
    }

  

}