using NeoErp.Core.Integration;
using NeoErp.Core.Models;
using NeoErp.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace NeoErp.Core.Helpers
{
    public class DBHelper
    {
      
        public static object GetDBData(string ReturnFieldName, string SourceTable, string BaseFieldName, string BaseFieldValue)
        {
            string sql = "Select " + ReturnFieldName + " from " + SourceTable + " where " + BaseFieldName + "='" + BaseFieldValue + "'";
            DbConnBase _con = new DbConnBase();
            var data = _con.ExecuteScalar(sql);
            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">Argument in string type in Pair : FieldName, TableName, ToCheckValue</param>
        /// <returns></returns>
        public static bool IsUsedMaster(params object[] args)
        {
            DbConnBase _con = new DbConnBase();
            StringBuilder str = new StringBuilder();
            str.Clear();

            for (int i = 0; i < args.Length - 1; i++)
            {
                if (i >= 2)
                {
                    str.Append("\n UNION ALL \n");
                }
                str.AppendFormat("Select top 1 {0} from {1} where {0} = '{2}'", args[i], args[i + 1], args[i + 2]);
                i += 2;
            }

            DataTable dt = new DataTable();
           dt= _con.ExecuteDataset(str.ToString()).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public static string FormatFixLen(string FieldName, string FormatChar)
        {
            var ch = FormatChar.Substring(0,1);
            dynamic len = FormatChar.Length;
            return " REPLICATE('" + ch + "' , " + len + " - LEN(" + FieldName + ")) + CAST(" + FieldName + " AS VARCHAR(" + len + " ) ) ";
          
        }

        public static string SqlCstr(string FIeldName)
        {
            return " convert(varchar,(isnull(" + FIeldName + ",''))) ";            
        }

        public static string SqlVal(string FieldName)
        {
            return " convert(float,(isnull(" + FieldName + ",0))) ";            
        }

        public static string SqlConvert(Enums.SqlTypes TypeName, string FieldName)
        {           
            switch (TypeName)
            {
                case Enums.SqlTypes.TypeInt:
                    return " convert(int,(isnull(" + FieldName + ",0))) ";
                case Enums.SqlTypes.TypeFloat:
                    return " convert(float,(isnull(" + FieldName + ",0))) ";
                case Enums.SqlTypes.TypeString:
                    return " convert(varchar,(isnull(" + FieldName + ",''))) ";                  
            }
            return FieldName;
        }

        public static object SqlCondition(string Condition, string TruePart, string FalsePart)
        {
            string Str = "";         
            Str = "(CASE WHEN " + Condition + " THEN " + TruePart + " ELSE " + FalsePart + " END )";            
            return Str;
        }

        public string GetStuffValue(string Seperator, string ColumnName, string TableName, string Condition = "", string OrderBy = "")
        {
            if (string.IsNullOrEmpty(Condition))
                Condition = "1=1";
            if (string.IsNullOrEmpty(OrderBy))
                OrderBy = ColumnName;

            StringBuilder sql = new StringBuilder();
            sql.Remove(0, sql.Length);

            sql.Append("DECLARE @C VARCHAR(MAX)");           
            sql.Append("\n SELECT @C =");
            sql.AppendFormat("\n STUFF ((Select '{0}' +  CONVERT(VARCHAR(MAX), {1}) from  {2} where {3} order by {4}", Seperator, ColumnName, TableName, Condition, OrderBy);           
            sql.Append("\n for xml path('')),1,1,'')");           
            sql.Append("\n SELECT @C ");

            DbConnBase _con = new DbConnBase();
            string RetVal = _con.ExecuteScalar(sql.ToString()).ToString();
            return RetVal;
        }

        public string GetStuffValue(string Seperator, string ColumnName, string SelectionQry)
        {

            StringBuilder sql = new StringBuilder();
            sql.Remove(0, sql.Length);

            sql.Append("DECLARE @C VARCHAR(MAX)");            
            sql.Append("SELECT @C =");
            sql.AppendFormat("STUFF ((Select DISTINCT '{0}' + '['+ CONVERT(VARCHAR(MAX), {1}) +']' from  {2} ", Seperator, ColumnName, SelectionQry);       
            sql.Append(" for xml path('')),1,1,'')");           
            sql.Append("SELECT @C ");

            DbConnBase _con = new DbConnBase();
            string RetVal = _con.ExecuteScalar(sql.ToString()).ToString();
            return RetVal;
        }
        
        public static object GetStuffQuery(string Seperator, string ColumnName, string TableName, string Condition = "", string OrderBy = "", string GroupBy = "")
        {
            if (string.IsNullOrEmpty(Condition))
                Condition = "1=1";
            if (string.IsNullOrEmpty(OrderBy))
                OrderBy = ColumnName;

            StringBuilder sql = new StringBuilder();
            sql.Remove(0, sql.Length);            
            sql.AppendFormat(" STUFF ((Select '{0}' +  {1} from  {2} where {3} {4} order by {5}", Seperator, SqlConvert(Enums.SqlTypes.TypeString, ColumnName), TableName, Condition, (string.IsNullOrEmpty(GroupBy) ? "" : " group by " + GroupBy), OrderBy);
            sql.Append(" for xml path('')),1,1,'')");            
            return sql.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Query"></param>
        /// <param name="ReportColumnName"> column to apply agreegate function</param>
        /// <param name="PivotColumn">Field to be display in column</param>
        /// <param name="StuffValues"></param>
        /// <param name="PivotFunctionName"> name of agregate function</param>
        /// <returns></returns>
        public static string GetPivotQuery(string Query, string ReportColumnName, string PivotColumn, string StuffValues, string PivotFunctionName)
        {
            StringBuilder sql = new StringBuilder();
            sql.Remove(0, sql.Length);
            sql.AppendFormat("Select * FROM ({0}) AS qry1", Query);
            sql.AppendFormat(" Pivot ({0}({1}) for {2}", PivotFunctionName, ReportColumnName, PivotColumn);
            sql.AppendFormat(" in ({0})) as pvt", StuffValues);
            return sql.ToString();
        }

        public bool ValidateSqlQry(string sql)
        {
            DbConnBase _con = new DbConnBase();
            object msg = _con.ExecuteScalar(" Declare @result varchar(30) \n Execute ValidateSqlSyntax '" + sql.Replace("'", "''") + "'," + " @result OUTPUT \n select @result");
            if (msg.ToString() == "Success")
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}