using NeoErp.Core.Helpers;
using NeoErp.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Models.ReportBuilder
{
    public class ReportBuilderModel : filterOption
    {
        public ReportBuilderModel() : base()
        {
            ReportColumnsField = new List<ColumnsField>();
            SettingsReport = new SettingsViewModel();
        }
        public string ReportName { get; set; }
        public string Query { get; set; }
        public string Settings { get; set; }
        public SettingsViewModel SettingsReport { get; set; }
        public List<ColumnsField> ReportColumnsField { get; set; }
    }
    public class SettingsViewModel: AdvancedFilterSettingsModel
    {
        public bool DateFilter { get; set; } = true;
        public bool Email { get; set; } = true;
        public bool showConsolidateFilter { get; set; } = true;
        public bool showReportSetting { get; set; } = true;

    }
    public class ColumnsField
    {
        public string ColumnsName { get; set; }
        public string Type { get; set; }
        public string width { get; set; }
    }

    public class GridOption
    {
        public bool Groupby { get; set; } = true;


    }

}