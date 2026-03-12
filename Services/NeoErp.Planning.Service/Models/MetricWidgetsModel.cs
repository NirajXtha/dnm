using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Planning.Services.Models
{
    public class MetricWidgetsModel
    {

        public List<Spartline> SparkLine { get; set; }
        public MetricWidgetsModel()
        {
            SparkLine = new List<Spartline>();
        }
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
        public String XAxis { get; set; } = "0";
        public string YAxis { get; set; } = "1";
        public string Width { get; set; } = "3";
        public string height { get; set; } = "1";

        public string xaxislabel { get; set; }
        public string yaxislabel { get; set; }
        public string HtmlResult { get; set; }
        public decimal? MaxQueryValue { get; set; }
        public string calculationBase { get; set; }
        public String secondaryTitle { get; set; }

       
        public string SecondaryValue { get; set; }
    }
    public class Spartline
    {
        public string LABEL { get; set; }
        public decimal? VALUE { get; set; }
        public Decimal? TARGET { get; set; }
    }
}