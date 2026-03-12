using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NeoErp.Core.Helpers
{  
    public class DataTableHelper
    {

        public static bool ChangeDataTableColumnDataTypeM(DataTable i_dtTable, string i_sColumnNames, Type i_oNewType)
        {

            string[] ColNames = i_sColumnNames.Split(',');
            foreach (string cols in ColNames)
            {
                ChangeDataTableColumnDataType(i_dtTable, cols, i_oNewType);
            }
            return true;

        }



        /// <summary>
        /// Converts DataType of ADO.NET DataTable to a new type
        /// </summary>
        /// <param name="i_dtTable">DataTable</param>
        /// <param name="i_sColumnNames">Name of the column</param>
        /// <param name="i_oNewType">New Type of the column</param>
        /// <returns>True in case of successfull conversion, False otherwise</returns>
        public static bool ChangeDataTableColumnDataType(DataTable i_dtTable, string i_sColumnNames, Type i_oNewType)
        {

            if (i_dtTable.Columns.Contains(i_sColumnNames) == false)
                return false;

            DataColumn oOldcolumn = i_dtTable.Columns[i_sColumnNames];

            if (object.ReferenceEquals(oOldcolumn.DataType, i_oNewType))
                return true;

            try
            {
                DataColumn oNewColumn = new DataColumn("$temporary^", i_oNewType);

                i_dtTable.Columns.Add(oNewColumn);
                oNewColumn.SetOrdinal(i_dtTable.Columns.IndexOf(oOldcolumn));
                //To make sure column is inserted at the same place

                foreach (DataRow row in i_dtTable.Rows)
                {
                    try
                    {
                        row["$temporary^"] = Convert.ChangeType(row[i_sColumnNames], i_oNewType);
                    }
                    catch
                    {
                    }
                }
                i_dtTable.Columns.Remove(i_sColumnNames);
                oNewColumn.ColumnName = i_sColumnNames;
            }
            catch (Exception generatedExceptionName)
            {
                return false;
            }

            return true;
        }


        public static DataTable RemoveDataTableColumns(DataTable table, string columnList)
        {
            if (table == null)
            {
                return table;
            }

            string[] remCol = columnList.Replace(" ", "").Split(',');

            for (int i = 0; i <= remCol.Length - 1; i++)
            {
                if (table.Columns.Contains(remCol[i]))
                {
                    table.Columns.Remove(remCol[i]);
                }
            }

            return table;
        }
        /// <summary>
        /// This function will remove all columns from datatable except list of include columns
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columnList"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static DataTable IncludeDataTableColumns(DataTable table, string columnList)
        {
            if (table == null)
            {
                return table;
            }

            List<string> inclClm = new List<string>(columnList.Replace(" ", "").Split(','));
            List<string> clmList = new List<string>();
            for (int i = 0; i <= table.Columns.Count - 1; i++)
            {
                clmList.Add(table.Columns[i].ColumnName);
            }

            for (int i = 0; i <= clmList.Count - 1; i++)
            {
                if (inclClm.Contains(clmList[i]))
                    continue;
                if (table.Columns.Contains(clmList[i]))
                {
                    table.Columns.Remove(clmList[i]);
                }
            }

            return table;
        }


        public static object AddSrNoColumn(DataTable tbl, string colName = "SrNo")
        {
            DataColumn col = new DataColumn(colName, typeof(Int64));
            col.AutoIncrement = true;
            col.AutoIncrementSeed = 1;
            col.AutoIncrementStep = 1;
            tbl.Columns.Add(col);
            tbl.Columns[col.ColumnName].SetOrdinal(0);
            return tbl;
        }



        public DataTable merge(DataTable fatherTable, DataTable sonTable, string fatherColumnName, string sonColumnName, string fatherIncludeColList = "", string sonIncludeColList = "")
        {

            DataTable result = getSchemedTable(fatherTable, sonTable, fatherIncludeColList.ToLower(), sonIncludeColList.ToLower());
            string STR = null;

            for (int i = 0; i <= fatherTable.Rows.Count - 1; i++)
            {
                DataRow FatherRow = fatherTable.Rows[i];
                STR = FatherRow[fatherColumnName].ToString();

                for (int j = 0; j <= sonTable.Rows.Count - 1; j++)
                {
                    DataRow sonRow = sonTable.Rows[j];

                    if (STR == sonTable.Rows[j][sonColumnName].ToString())
                    {
                        DataRow RROW = result.NewRow();

                        result.Rows.Add(compinTwoRows(FatherRow, sonRow, RROW, fatherTable, sonTable, result));
                    }
                }
            }

            return result;

        }

        private DataTable getSchemedTable(DataTable main, DataTable branch, string fatherIncludeColList = "", string sonIncludeColList = "")
        {

            DataTable result = new DataTable();

            for (int i = 0; i <= main.Columns.Count - 1; i++)
            {
                if (!string.IsNullOrEmpty(fatherIncludeColList) & fatherIncludeColList.Contains(main.Columns[i].ColumnName.ToLower()) == false)
                {
                    continue;
                }

                result.Columns.Add(main.Columns[i].ColumnName);
            }

            for (int j = 0; j <= branch.Columns.Count - 1; j++)
            {
                if (result.Columns.Contains(branch.Columns[j].ColumnName))
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(sonIncludeColList) & sonIncludeColList.Contains(branch.Columns[j].ColumnName.ToLower()) == false)
                {
                    continue;
                }

                result.Columns.Add(branch.Columns[j].ColumnName);
            }

            return result;

        }

        private DataRow compinTwoRows(DataRow mainRow, DataRow sonRow, DataRow RRow, DataTable Father, DataTable son, DataTable result)
        {

            string mainColumnName = null;

            for (int i = 0; i <= mainRow.ItemArray.Length - 1; i++)
            {
                mainColumnName = Father.Columns[i].ToString();
                if (result.Columns.Contains(mainColumnName) == false)
                    continue;
                RRow[mainColumnName] = mainRow[mainColumnName];
            }

            for (int j = 0; j <= sonRow.ItemArray.Length - 1; j++)
            {
                mainColumnName = son.Columns[j].ToString();
                if (result.Columns.Contains(mainColumnName) == false)
                    continue;
                RRow[mainColumnName] = sonRow[mainColumnName];
            }

            return RRow;

        }

    }


}