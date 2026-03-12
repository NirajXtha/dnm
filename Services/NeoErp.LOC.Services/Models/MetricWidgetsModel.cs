using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.LOC.Services.Models
{
    public class MetricWidgetsModel
    {
        public string widgetsName { get; set; }
        public string WidgetsBGColor { get; set; }
        public string sqlQuery { get; set; }
        public string widgetsColor { get; set; }
        public string widgetsTitle { get; set; }
        public string widgetFontIcon { get; set; }
        public string aggValue { get; set; }
        public int ReportId { get; set; }
        public string IsActive { get; set; }
        public decimal Orderno { get; set; }
        public string QUICKTYPE { get; set; }
        public string MIDBGCOLOR { get; set; }
        public string MIDFONTCOLOR { get; set; }
        public string MIDISBLINK { get; set; }
        public string MAXBGCOLOR { get; set; }
        public string MAXFONTCOLOR { get; set; }
        public string MAXISBLINK { get; set; }
        public decimal MAXVALUES { get; set; }
        public decimal MINVALUES { get; set; }
        public string MINISBLINK { get; set; }

        public string LABELPOSITION { get; set; }
        public decimal? SPEEDOMETERMAXVALUE { get; set; }
        public string MAXVALUEQUERY { get; set; }
        public string MINVALUEQUERY { get; set; }

        public string USERPERMISSION { get; set; }
        public int orderIndex { get; set; } = 0;
        // public string 

    }
}