using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Models.QueryBuilder
{
    public class QueryBuilderModel
    {

        public string TableName { get; set; }
        public string TableNamesAlis { get; set; }

    }

    public class TableColumns
    {
        public string tableName { get; set; }
        public string columnsName { get; set; }
        public string dataType { get; set; }
        public decimal dataLength { get; set; }
        public string columnheader { get; set; }
    }

    public class highntQuery
    {
        public string query { get; set; }
    }
    public class CreateMetricwidgets
    {
        public string widgetsName { get; set; }
        public string WidgetsBGColor { get; set; }
        public string sqlQuery { get; set; }
        public string widgetsColor { get; set; }
        public string widgetsTitle { get; set; }
        public string widgetFontIcon { get; set; }
        public string widgetLink { get; set; }

        public bool Isactive { get; set; }
        public string OrderNo { get; set; }
        public string WidgetsId { get; set; }

        public string MidBGColor { get; set; }
        public string MidFontColor { get; set; }
        public bool IsMidBlink { get; set; }

        public string MaxBGColor { get; set; }
        public string MaxFontColor { get; set; }
        public bool IsMaxBlink { get; set; }
        public string MaxValue { get; set; }

        public string MinValue { get; set; }
        public bool IsBlink { get; set; }

        public string ChartType { get; set; }
        public string LABELPOSITION { get; set; }
        public string SPEEDOMETERMAXVALUE { get; set; }
        public string MAXVALUEQUERY { get; set; }
        public string MINVALUEQUERY { get; set; }

        public string ModuleCode { get; set; }

        public string customerCode { get; set; } = "0";

        public string itemsCode { get; set; } = "0";

        public string divisionCode { get; set; } = "0";

        public string suppliersCode { get; set; } = "0";

        public string ledgerCode { get; set; } = "0";

        public string CaculationMethod { get; set; }
        public string SecondaryTitle { get; set; }

        public string TableSize { get; set; } = "col-md-6";

        public string sparklineoption { get; set; } = "line";
        public bool Horizontal { get; set; }
        public string Xaxis { get; set; }
        public string Zaxis { get; set; }
    }

    public class SchedularMail
    {
        public SchedularMail()
        {
            this.UniqueKey = Guid.NewGuid().ToString();
        }
        public string schedularName { get; set; }
        public string email { get; set; }
        public string Subject { get; set; }
        public string sqlQuery { get; set; }
        public string Template { get; set; }
        public string UniqueKey { get; set; }
        public string EMAIL_BCC { get; set; }
        public string EMAIL_CC { get; set; }
        public string ATTACHMENT_FLAG { get; set; } = "N";
        public string MESSAGE_FLAG { get; set; }
        public string sqltype { get; set; } = "Inline";
        public string fequencytype { get; set; } = "daily";
        public string startdate { get; set; } = DateTime.Now.ToString();
        public int Days { get; set; } = 0;
        public bool summaryReport { get; set; } = false;
        public string reportType { get; set; }
        public List<string> employeeCode { get; set; }
        public string reportName { get; set; }
        public bool isTemplate { get; set; }

    }
}