using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;

namespace NeoErp.Core.Helpers
{
    public static class CommonHelper
    {
        public static bool IsNumericType(this object o)
        {
            switch (Type.GetTypeCode(o.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single: 
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsNumericProperty(this Type o)
        {
            switch (Type.GetTypeCode(o))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        public static string ReplaceHtmlTag(this string S)
        {

           var replaceString = Regex.Replace(S, "<.*?>", "#:#");
            replaceString = S.Replace("\n", " ").Replace("&nbsp;", " ");

           var array = Regex.Split(replaceString, "#:#");

            array = array.Where(q => !string.IsNullOrEmpty(q)).ToArray();
            return HttpUtility.HtmlDecode(string.Join(" ", array));
        }
        public static string ConvertDataTableToHTML(this DataTable dt)
        {
            string html = "<table>";
            //add header row
            html += "<tr>";
            for (int i = 0; i < dt.Columns.Count; i++)
                html += "<td>" + dt.Columns[i].ColumnName + "</td>";
            html += "</tr>";
            //add rows
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                html += "<tr>";
                for (int j = 0; j < dt.Columns.Count; j++)
                    html += "<td>" + dt.Rows[i][j].ToString() + "</td>";
                html += "</tr>";
            }
            html += "</table>";
            return html;
        }
        public static string ConvertDataTableToHTMLWithFormat(this DataTable dt)
        {
          
            string html = "<table border='1' class='table table-striped table-hover' style='border:1px'>";
            //add header row
            html += "<thead style='background-color: green;color: white !important;'><tr>";
            for (int i = 0; i < dt.Columns.Count; i++)
                html += "<td>" + dt.Columns[i].ColumnName + "</td>";
            html += "</tr></thead>";
            //add rows
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                html += "<tr>";
                for (int j = 0; j < dt.Columns.Count; j++)
                    html += "<td>" + dt.Rows[i][j].ToString() + "</td>";
                html += "</tr>";
            }
            html += "</table>";
            return html;


        }
        
        
        public static System.IO.MemoryStream ConvertTableIntoExcel(DataTable dt, DataTable dt1)
        {
          
            string html = "<h2>Daily Employee Report</h2><br/><table border='1' class='table table-striped table-hover' style='border:1px'>";
            //add header row
            html += "<thead style='background-color: green;color: white !important;'><tr>";
            for (int i = 0; i < dt.Columns.Count; i++)
                html += "<td>" + dt.Columns[i].ColumnName + "</td>";
            html += "</tr></thead>";
            //add rows
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                html += "<tr>";
                for (int j = 0; j < dt.Columns.Count; j++)
                    html += "<td>" + dt.Rows[i][j].ToString() + "</td>";
                html += "</tr>";
            }
            html += "</table>";


            if(dt1.Rows.Count > 0 )
            {
                 html += "<br/><br/><br/><br/><br/><h2>Daily Sales Report</h2><br/><table border='1' class='table table-striped table-hover' style='border:1px'>";
                html += "<thead style='background-color: green;color: white !important;'><tr>";
                for (int i = 0; i < dt1.Columns.Count; i++)
                    html += "<td>" + dt1.Columns[i].ColumnName + "</td>";
                html += "</tr></thead>";
                //add rows
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    html += "<tr>";
                    for (int j = 0; j < dt1.Columns.Count; j++)
                        html += "<td>" + dt1.Rows[i][j].ToString() + "</td>";
                    html += "</tr>";
                }
                html += "</table>";
            }
            System.IO.MemoryStream s = new MemoryStream();
            System.Text.Encoding Enc = System.Text.Encoding.Default;
            byte[] mBArray = Enc.GetBytes(html);
            s = new MemoryStream(mBArray, false);

            return s;
        }

        
        public static System.IO.MemoryStream DataToExcel(this DataTable dt)
        {
            //StreamWriter sw = new StreamWriter();
            System.IO.StringWriter tw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);
            if (dt.Rows.Count > 0)
            {

                DataGrid dgGrid = new DataGrid();
                dgGrid.DataSource = dt;
                dgGrid.DataBind();
                dgGrid.HeaderStyle.Font.Bold = true;
                //Get the HTML for the control.
                dgGrid.RenderControl(hw);
              

            }
            System.IO.MemoryStream s = new MemoryStream();
            System.Text.Encoding Enc = System.Text.Encoding.Default;
            byte[] mBArray = Enc.GetBytes(tw.ToString());
            s = new MemoryStream(mBArray, false);

            return s;
        }
        public static string GetFiscalYear()
        {
              return ConfigurationManager.AppSettings["FiscalYear"]??string.Empty;
        }
        
    }
}