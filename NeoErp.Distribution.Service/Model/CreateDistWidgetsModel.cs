using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model
{
   public class CreateDistWidgetsModel
    {
        public string widgetsName { get; set; }
        public int? MAXID { get; set; }
        public int ReportId { get; set; }
        public string WidgetsBGColor { get; set; }
        public string DISTRIBUTOR_CODE { get; set; }
        public string sqlQuery { get; set; }
        public string widgetsColor { get; set; }
        public string widgetsTitle { get; set; }
        public string widgetFontIcon { get; set; }

        public bool Isactive { get; set; }
        public bool isDistributorChecked { get; set; }
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

        public string USERPERMISSION { get; set; }
        public int orderIndex { get; set; } = 0;
        public string aggValue { get; set; }
    }

    public class DistTableList
    {
        public string TableName { get; set; }
    }

    public class DistTableColumn
    {
        public string tableName { get; set; }
        public string columnsName { get; set; }
        public string dataType { get; set; }
        public decimal dataLength { get; set; }
        public string columnheader { get; set; }
    }

    public class DistMatrixModel
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

        public string MODULE_CODE { get; set; }

        public string DISTRIBUTOR_CHECK_TYPE { get; set; }
        public string DISTRIBUTOR_CODE { get; set; }
    }

   
}
