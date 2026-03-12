using NeoErp.Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Text;

namespace NeoErp.Models.Report
{
    public class Debtor
    {
        public static DataTable CustTable = new DataTable();
        public static DataColumn column;
        public static DataRow row;
        internal static string[] GetDefaultSetupParam()
        {
            string[] DefaultParams = new string[6];
            string GetDefaultParamsSql = "SELECT * FROM CUST_AGE_REPORT_PREF_MASTER WHERE ISDEFAULT = 'Y'";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, GetDefaultParamsSql))
            {
                if (sdr.HasRows)
                {
                    if (sdr.Read())
                    {
                        DefaultParams[0] = sdr["PREF_CODE"].ToString();
                        DefaultParams[1] = sdr["PREF_NAME"].ToString();
                        DefaultParams[2] = sdr["AMOUNT_FIGURE"].ToString();
                        DefaultParams[3] = sdr["AMOUNT_ROUNDUP"].ToString();
                        DefaultParams[4] = sdr["AXIS_VALUE"].ToString();
                        DefaultParams[5] = sdr["FDATE"].ToString();
                    }
                }
            }
            return DefaultParams;
        }

        internal static int[] GetDefaultDetailParam(string PrefCode)
        {
            List<int> list = new List<int>();
            string GetDetailSql = "SELECT X_VALUE FROM CUST_AGE_REPORT_PREF_DETAIL WHERE PREF_CODE = '" + PrefCode + "' ORDER BY X_VALUE";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, GetDetailSql))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        list.Add(Convert.ToInt16(sdr["X_Value"]));
                    }
                }
            }
            int[] DefaultSteps = list.ToArray();
            return DefaultSteps;
        }

        public static DataTable getData()
        {
            DataTable tblGroup = new DataTable();
            string sql = @"SELECT INITCAP(A.CUSTOMER_EDESC) CUSTOMER,B.DATE_STEP STEP,B.BILL_AMOUNT AMOUNT,B.PRE_CODE,B.MASTER_CODE,
                        (SELECT INITCAP(CUSTOMER_EDESC) FROM SA_CUSTOMER_SETUP WHERE MASTER_CUSTOMER_CODE = B.PRE_CODE AND GROUP_SKU_FLAG = 'G')  PARENT,
                        NVL((SELECT INITCAP(CUSTOMER_EDESC) FROM SA_CUSTOMER_SETUP WHERE MASTER_CUSTOMER_CODE  = (SELECT PRE_CUSTOMER_CODE FROM SA_CUSTOMER_SETUP WHERE MASTER_CUSTOMER_CODE = B.PRE_CODE AND GROUP_SKU_FLAG = 'G')),INITCAP('NOT-SPECIFIED')) GRAND_PARENT 
                        FROM SA_CUSTOMER_SETUP A,CUST_AGE B
                        WHERE A.CUSTOMER_CODE = B.MCODE";
            DataSet ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, sql);
            tblGroup = ds.Tables[0];
            return tblGroup;

        }




        public static DataTable getData(int[] Steps, string AsOfDateEnglish, string figure, string round)
        {
            AsOfDateEnglish = Convert.ToDateTime(AsOfDateEnglish).ToString("dd-MMM-yyyy");
            int ArrayLength = Steps.Length;
            string DeleteSql = "DELETE CUST_AGE_TLINE";
            OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, DeleteSql);
            string Commit = "Commit";
            OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, Commit);
            for (int i = 0; i <= ArrayLength - 1; i++)
            {
                string InsertSql = "INSERT INTO CUST_AGE_TLINE VALUES (" + i + "," + Steps[i] + ")";
                OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, InsertSql);
                OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, Commit);
            }
            OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.StoredProcedure, "generate_cust_age('" + AsOfDateEnglish + "')");
            CustTable.Clear();
            CustTable.Columns.Clear();
            CustTable.Rows.Clear();
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "CustomerCode";
            CustTable.Columns.Add(column);
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "CustomerName";
            CustTable.Columns.Add(column);
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "ParentCode";
            CustTable.Columns.Add(column);
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "Level";
            CustTable.Columns.Add(column);
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "IsLeaf";
            CustTable.Columns.Add(column);
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "loaded";
            CustTable.Columns.Add(column);
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "expanded";
            CustTable.Columns.Add(column);
            string DateStepSql = "SELECT DISTINCT DATE_STEP FROM CUST_AGE ORDER BY DATE_STEP";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, DateStepSql))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        column = new DataColumn();
                        column.DataType = System.Type.GetType("System.String");
                        column.ColumnName = sdr["DATE_STEP"].ToString();
                        CustTable.Columns.Add(column);

                    }
                }
            }
            string TreeSql = @"SELECT  A.CUSTOMER_CODE, initcap(A.CUSTOMER_EDESC) CUSTOMER_EDESC,A.PARRENT_CODE,A.MASTER_CUSTOMER_CODE,PRE_CUSTOMER_CODE,LEVEL,'FALSE' EXPANDED,'TRUE' LOADED,'FALSE' ISLEAF
                                FROM SA_CUSTOMER_SETUP A where GROUP_SKU_FLAG='G'
                                CONNECT BY PRIOR TRIM(CUSTOMER_CODE)=TRIM(PARRENT_CODE)
                                START WITH PRE_CUSTOMER_CODE='00'";
            using (OracleDataReader Treesdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, TreeSql))
            {
                if (Treesdr.HasRows)
                {
                    while (Treesdr.Read())
                    {
                        if (CheckExistance(Treesdr["CUSTOMER_CODE"].ToString()))
                        {
                            row = CustTable.NewRow();
                            row["CustomerCode"] = Treesdr["CUSTOMER_CODE"].ToString() + "C";
                            row["CustomerName"] = Treesdr["CUSTOMER_EDESC"].ToString();
                            row["ParentCode"] = Treesdr["PARRENT_CODE"].ToString() + "C";
                            row["Level"] = Treesdr["LEVEL"].ToString();
                            row["IsLeaf"] = "false";
                            row["expanded"] = "true";
                            row["loaded"] = "true";

                            string RowStepSql = "SELECT DISTINCT DATE_STEP FROM CUST_AGE ORDER BY DATE_STEP";
                            using (OracleDataReader tsdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, RowStepSql))
                            {
                                if (tsdr.HasRows)
                                {
                                    while (tsdr.Read())
                                    {
                                        string GetAmount = "SELECT  SUM(nvl(BILL_AMOUNT,0)-nvl(PMT_AMOUNT,0)) AMOUNT FROM CUST_AGE WHERE MCODE='" + Treesdr["CUSTOMER_CODE"].ToString() + "' AND DATE_STEP='" + tsdr["DATE_STEP"].ToString() + "' ";
                                        using (OracleDataReader Fsdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, GetAmount))
                                        {
                                            if (Fsdr.HasRows)
                                            {
                                                while (Fsdr.Read())
                                                {
                                                    if (Fsdr["AMOUNT"].ToString() != "" && Fsdr["AMOUNT"].ToString() != null)
                                                    {

                                                        row[tsdr["DATE_STEP"].ToString()] = Fsdr["AMOUNT"].ToString();
                                                    }
                                                    else
                                                    {
                                                        row[tsdr["DATE_STEP"].ToString()] = row[tsdr["DATE_STEP"].ToString()] = null; ;

                                                        //switch (round)
                                                        //{
                                                        //    case "0": row[tsdr["DATE_STEP"].ToString()] = row[tsdr["DATE_STEP"].ToString()] = "0".ToString(); ;
                                                        //        break;
                                                        //    case "0.0": row[tsdr["DATE_STEP"].ToString()] = row[tsdr["DATE_STEP"].ToString()] = "0.0".ToString(); ;
                                                        //        break;
                                                        //    case "0.00": row[tsdr["DATE_STEP"].ToString()] = row[tsdr["DATE_STEP"].ToString()] = "0.00".ToString(); ;
                                                        //        break;
                                                        //    case "0.000": row[tsdr["DATE_STEP"].ToString()] = row[tsdr["DATE_STEP"].ToString()] = "0.000".ToString(); ;
                                                        //        break;
                                                        //    case "0.0000": row[tsdr["DATE_STEP"].ToString()] = row[tsdr["DATE_STEP"].ToString()] = "0.0000".ToString(); ;
                                                        //        break;
                                                        //}
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            CustTable.Rows.Add(row);
                            string Sql = @"SELECT DISTINCT A.CUSTOMER_CODE,initcap(A.CUSTOMER_EDESC) CUSTOMER_EDESC,A.PARRENT_CODE,GET_LEVEL_CUST(A.CUSTOMER_CODE) LVL 
                                        FROM SA_CUSTOMER_SETUP A,CUST_AGE B 
                                        WHERE A.CUSTOMER_CODE = B.MCODE 
                                        AND A.PARRENT_CODE ='" + Treesdr["CUSTOMER_CODE"].ToString() + "'";
                            using (OracleDataReader Leafsdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, Sql))
                            {
                                if (Leafsdr.HasRows)
                                {
                                    while (Leafsdr.Read())
                                    {
                                        row = CustTable.NewRow();
                                        row["CustomerCode"] = Leafsdr["CUSTOMER_CODE"].ToString() + "C";
                                        row["CustomerName"] = Leafsdr["CUSTOMER_EDESC"].ToString();
                                        row["ParentCode"] = Leafsdr["PARRENT_CODE"].ToString() + "C";
                                        row["Level"] = Leafsdr["LVL"].ToString();
                                        row["IsLeaf"] = "true";
                                        row["expanded"] = "true";
                                        row["loaded"] = "true";
                                        string RowDateStepSql = "SELECT DISTINCT DATE_STEP FROM CUST_AGE ORDER BY DATE_STEP";
                                        using (OracleDataReader tsdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, RowDateStepSql))
                                        {
                                            if (tsdr.HasRows)
                                            {
                                                while (tsdr.Read())
                                                {
                                                    string GetAmount = "SELECT  SUM(nvl(BILL_AMOUNT,0)-nvl(PMT_AMOUNT,0)) AMOUNT FROM CUST_AGE WHERE MCODE='" + Leafsdr["CUSTOMER_CODE"].ToString() + "' AND DATE_STEP='" + tsdr["DATE_STEP"].ToString() + "' ";
                                                    using (OracleDataReader Fsdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, GetAmount))
                                                    {
                                                        if (Fsdr.HasRows)
                                                        {
                                                            while (Fsdr.Read())
                                                            {

                                                                if (Fsdr["AMOUNT"].ToString() != "" && Fsdr["AMOUNT"].ToString() != null)
                                                                {
                                                                    double Amount = Convert.ToDouble(Fsdr["AMOUNT"]);
                                                                    switch (figure)
                                                                    {
                                                                        case "Actual": Amount = Amount / 1;
                                                                            break;
                                                                        case "thousand": Amount = Amount / 1000;
                                                                            break;
                                                                        case "lakh": Amount = Amount / 100000;
                                                                            break;
                                                                        case "crore": Amount = Amount / 100000;
                                                                            break;
                                                                    }

                                                                    switch (round)
                                                                    {
                                                                        case "0": row[tsdr["DATE_STEP"].ToString()] = (Math.Round(Amount, 0)).ToString();
                                                                            break;
                                                                        case "0.0": row[tsdr["DATE_STEP"].ToString()] = (Math.Round(Amount, 1)).ToString();
                                                                            break;
                                                                        case "0.00": row[tsdr["DATE_STEP"].ToString()] = (Math.Round(Amount, 2)).ToString();
                                                                            break;
                                                                        case "0.000": row[tsdr["DATE_STEP"].ToString()] = (Math.Round(Amount, 3)).ToString();
                                                                            break;
                                                                        case "0.0000": row[tsdr["DATE_STEP"].ToString()] = (Math.Round(Amount, 4)).ToString();
                                                                            break;
                                                                    }

                                                                }
                                                                else
                                                                {
                                                                    switch (round)
                                                                    {
                                                                        case "0": row[tsdr["DATE_STEP"].ToString()] = row[tsdr["DATE_STEP"].ToString()] = "0".ToString(); ;
                                                                            break;
                                                                        case "0.0": row[tsdr["DATE_STEP"].ToString()] = row[tsdr["DATE_STEP"].ToString()] = "0.0".ToString(); ;
                                                                            break;
                                                                        case "0.00": row[tsdr["DATE_STEP"].ToString()] = row[tsdr["DATE_STEP"].ToString()] = "0.00".ToString(); ;
                                                                            break;
                                                                        case "0.000": row[tsdr["DATE_STEP"].ToString()] = row[tsdr["DATE_STEP"].ToString()] = "0.000".ToString(); ;
                                                                            break;
                                                                        case "0.0000": row[tsdr["DATE_STEP"].ToString()] = row[tsdr["DATE_STEP"].ToString()] = "0.0000".ToString(); ;
                                                                            break;
                                                                    }

                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        CustTable.Rows.Add(row);
                                    }
                                }
                            }

                        }
                    }
                }
            }
            return CustTable;
        }

        public static bool CheckExistance(string CustomerCode)
        {
            int Count = 0;
            string GetSql = @"SELECT  DISTINCT A.CUSTOMER_CODE, initcap(A.CUSTOMER_EDESC) CUSTOMER_EDESC,A.PARRENT_CODE 
                            FROM SA_CUSTOMER_SETUP A where GROUP_SKU_FLAG='I'
                            CONNECT BY PRIOR TRIM(CUSTOMER_CODE)=TRIM(PARRENT_CODE)
                            START WITH PARRENT_CODE IN (" + CustomerCode + ")";
            using (OracleDataReader tsdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, GetSql))
            {
                if (tsdr.HasRows)
                {
                    while (tsdr.Read())
                    {
                        string TheSql = "SELECT COUNT(*) TOTAL FROM CUST_AGE WHERE MCODE ='" + tsdr["CUSTOMER_CODE"] + "'";
                        using (OracleDataReader Countdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, TheSql))
                        {
                            if (Countdr.HasRows)
                            {
                                while (Countdr.Read())
                                {
                                    int count = Convert.ToInt16(Countdr["TOTAL"]);
                                    if (count != 0)
                                    {
                                        Count++;
                                    }
                                }
                            }
                        }
                    }
                    if (Count != 0) { return true; }

                }
            }
            return false;
        }




        public static void getAllHierarchy(string CustomerCode)
        {
            DataRow[] foundAuthors = CustTable.Select("CustomerCode = '" + CustomerCode + "'");
            if (foundAuthors.Length == 0)
            {
                string TheSql = "SELECT CUSTOMER_CODE,CUSTOMER_EDESC,PARRENT_CODE,GET_LEVEL_CUST(CUSTOMER_CODE) LVL FROM SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + CustomerCode + "'";
                using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, TheSql))
                {
                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            string ParentCode = sdr["PARRENT_CODE"].ToString();
                            getAllHierarchy(ParentCode);
                            row = CustTable.NewRow();
                            row["CustomerCode"] = sdr["CUSTOMER_CODE"].ToString();
                            row["CustomerName"] = sdr["CUSTOMER_EDESC"].ToString();
                            if (ParentCode == null || ParentCode == "")
                            {
                                row["ParentCode"] = "null";
                            }
                            else
                            {
                                row["ParentCode"] = sdr["PARRENT_CODE"].ToString();
                            }
                            row["Level"] = sdr["LVL"].ToString();
                            row["IsLeaf"] = "false".ToString();
                            string RowDateStepSql = "SELECT DISTINCT DATE_STEP FROM CUST_AGE ORDER BY DATE_STEP";
                            using (OracleDataReader tsdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, RowDateStepSql))
                            {
                                if (tsdr.HasRows)
                                {
                                    while (tsdr.Read())
                                    {
                                        row[tsdr["DATE_STEP"].ToString()] = "0".ToString();
                                    }
                                }
                            }
                            CustTable.Rows.Add(row);

                        }
                    }
                }
            }
        }


        internal static int[] GetArray()
        {
            int Count = 0;
            int[] ArrayNames = new int[0];
            string CountSql = "SELECT COUNT(*) AS COUNT FROM CUST_AGE_TLINE";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, CountSql))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        Count = Convert.ToInt16(sdr["COUNT"]);

                    }
                }
                else
                {
                    Count = 0;
                }
            }
            string sql = @"SELECT * FROM CUST_AGE_TLINE order by slno";
            DataSet ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, sql);
            int i = 0;
            ArrayNames = new int[Count];
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, sql))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        ArrayNames[i] = Convert.ToInt16(sdr["TLINE"]);
                        i++;
                    }
                }
            }
            return ArrayNames;
        }

        internal static DataTable getAllCustomerData(int[] Steps, string AsOfDateEnglish)
        {
            AsOfDateEnglish = Convert.ToDateTime(AsOfDateEnglish).ToString("dd-MMM-yyyy");
            int ArrayLength = Steps.Length;
            string DeleteSql = "DELETE CUST_AGE_TLINE";
            OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, DeleteSql);
            string Commit = "Commit";
            for (int i = 0; i <= ArrayLength - 1; i++)
            {
                string InsertSql = "INSERT INTO CUST_AGE_TLINE VALUES (" + i + "," + Steps[i] + ")";
                OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, InsertSql);

            }
            OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, Commit);
            OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.StoredProcedure, "generate_cust_age('" + AsOfDateEnglish + "')");
            string sql = @"SELECT A.DATE_STEP AS STEP,(nvl(A.BILL_AMOUNT,0)-nvl(A.PMT_AMOUNT,0)) AS AMOUNT,INITCAP(B.CUSTOMER_EDESC) AS CUSTOMER,A.MCODE 
                        FROM CUST_AGE A,SA_CUSTOMER_SETUP B
                        WHERE A.MCODE = B.CUSTOMER_CODE";
            DataSet ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, sql);
            DataTable tblGroup = ds.Tables[0];
            return tblGroup;

        }

        internal static void InsertIntoSetup(string figureAmount, string roundupAmount, string yaxis, string PreferenceName, int[] steps, string SetDefault, string FromDate)
        {
            string PrefCode = null;
            string GetPrefName = "SELECT PREF_CODE,PREF_NAME FROM CUST_AGE_REPORT_PREF_MASTER";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, GetPrefName))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        if (PreferenceName == sdr["PREF_NAME"].ToString())
                        {
                            PrefCode = sdr["PREF_CODE"].ToString();
                        }
                    }
                }
            }
            if (PrefCode != null)
            {
                if (SetDefault == null)
                {
                    string UpdateMasterSql = "UPDATE CUST_AGE_REPORT_PREF_MASTER SET AMOUNT_FIGURE='" + figureAmount + "',AMOUNT_ROUNDUP='" + roundupAmount + "',AXIS_VALUE='" + yaxis + "',FDATE ='" + FromDate + "' WHERE PREF_CODE = '" + PrefCode + "'";
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, UpdateMasterSql);
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, "COMMIT");
                }
                else
                {
                    string UpdateDefaultFlag = "UPDATE CUST_AGE_REPORT_PREF_MASTER SET ISDEFAULT = 'N'";
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, UpdateDefaultFlag);
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, "COMMIT");
                    string UpdateMasterSql = "UPDATE CUST_AGE_REPORT_PREF_MASTER SET AMOUNT_FIGURE='" + figureAmount + "',AMOUNT_ROUNDUP='" + roundupAmount + "',AXIS_VALUE='" + yaxis + "',ISDEFAULT='Y',FDATE ='" + FromDate + "' WHERE PREF_CODE = '" + PrefCode + "'";
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, UpdateMasterSql);
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, "COMMIT");
                }
                string DeleteDetailSql = "DELETE FROM CUST_AGE_REPORT_PREF_DETAIL WHERE PREF_CODE = '" + PrefCode + "'";
                OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, DeleteDetailSql);
                OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, "COMMIT");
                for (int i = 0; i <= steps.Length - 1; i++)
                {
                    int step = steps[i];
                    string UpdateDetailSql = @"insert into cust_age_report_pref_detail(pref_code,x_value) values('" + PrefCode + "','" + step + "')";
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, UpdateDetailSql);
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, "COMMIT");
                }
            }
            else
            {
                int PREF_CODE = 0;
                string findmaxcode = "SELECT MAX(PREF_CODE) PREF_CODE FROM CUST_AGE_REPORT_PREF_MASTER";
                DataSet ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, findmaxcode);
                string CODE = ds.Tables[0].Rows[0]["PREF_CODE"].ToString();
                if (CODE == "")
                {
                    PREF_CODE = 0;
                }
                else
                {
                    PREF_CODE = Convert.ToInt16(CODE);
                }
                PREF_CODE = PREF_CODE + 1;
                string inserty = null;
                if (SetDefault != null)
                {
                    string UpdateDefaultFlag = "UPDATE CUST_AGE_REPORT_PREF_MASTER SET ISDEFAULT = 'N'";
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, UpdateDefaultFlag);
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, "COMMIT");
                    inserty = @"INSERT INTO CUST_AGE_REPORT_PREF_MASTER(
                            PREF_CODE,PREF_NAME,AMOUNT_FIGURE,AMOUNT_ROUNDUP,AXIS_VALUE,ISDEFAULT,FDATE)VALUES
                            ('" + PREF_CODE + "','" + PreferenceName + "','" + figureAmount + "','" + roundupAmount + "','" + yaxis + "','Y','" + FromDate + "')";
                }
                else
                {
                    inserty = @"INSERT INTO CUST_AGE_REPORT_PREF_MASTER(
                            PREF_CODE,PREF_NAME,AMOUNT_FIGURE,AMOUNT_ROUNDUP,AXIS_VALUE,ISDEFAULT,FDATE)VALUES
                            ('" + PREF_CODE + "','" + PreferenceName + "','" + figureAmount + "','" + roundupAmount + "','" + yaxis + "','N','" + FromDate + "')";
                }
                OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, inserty);
                OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, "COMMIT");
                for (int i = 0; i <= steps.Length - 1; i++)
                {
                    int step = steps[i];
                    string Query = @"insert into cust_age_report_pref_detail(pref_code,x_value) values('" + PREF_CODE + "','" + step + "')";
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, Query);
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, "COMMIT");
                }
            }
        }
        internal static string GetDataForCustomerDebtorAjax()
        {
            string sql = @"select * from CUST_AGE_REPORT_PREF_MASTER";
            DataSet ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, sql);
            DataTable tblGroup = ds.Tables[0];

            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in tblGroup.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in tblGroup.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            return serializer.Serialize(rows);

        }


        internal static string GetAllPreferencesVariable(string rowKey)
        {
            string PreferenceSetup = null;
            string sql = @"select * from CUST_AGE_REPORT_PREF_MASTER WHERE PREF_CODE = '" + rowKey + "'";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, sql))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        PreferenceSetup = sdr["PREF_CODE"].ToString();
                        PreferenceSetup = PreferenceSetup + "," + sdr["PREF_NAME"].ToString();
                        PreferenceSetup = PreferenceSetup + "," + sdr["AMOUNT_FIGURE"].ToString();
                        PreferenceSetup = PreferenceSetup + "," + sdr["AMOUNT_ROUNDUP"].ToString();
                        PreferenceSetup = PreferenceSetup + "," + sdr["AXIS_VALUE"].ToString();
                        PreferenceSetup = PreferenceSetup + "," + sdr["FDATE"].ToString();
                    }

                }
            }

            string TimelineSql = @"SELECT * FROM CUST_AGE_REPORT_PREF_DETAIL WHERE PREF_CODE = '" + rowKey + "' ORDER BY X_VALUE";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, TimelineSql))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        PreferenceSetup = PreferenceSetup + "," + sdr["X_VALUE"].ToString();
                    }

                }
            }
            return PreferenceSetup;
        }

        internal static void RemoveSelectedRow(string rowKey)
        {
            string DeleteDetail = "DELETE FROM CUST_AGE_REPORT_PREF_DETAIL WHERE PREF_CODE = '" + rowKey + "'";
            OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, DeleteDetail);
            OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, "COMMIT");
            string DeleteMaster = "DELETE FROM CUST_AGE_REPORT_PREF_MASTER WHERE PREF_CODE = '" + rowKey + "'";
            OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, DeleteMaster);
            OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, "COMMIT");
        }

        internal static string GetPreferenceList()
        {
            string PrefNames = null;
            string GetPrefernceSql = "SELECT PREF_NAME FROM CUST_AGE_REPORT_PREF_MASTER";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, GetPrefernceSql))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        if (PrefNames != null)
                        {
                            PrefNames = PrefNames + "," + sdr["PREF_NAME"].ToString();
                        }
                        else
                        {
                            PrefNames = sdr["PREF_NAME"].ToString();
                        }
                    }

                }
            }
            return PrefNames;
        }







        internal static string GetAllTransaction(string CustomerCode)
        {

            CustomerCode = CustomerCode.Replace("C", "");

            string TransactionSql = @"SELECT  DISTINCT to_char(SALES_DATE,'yyyy-MON-dd') SALES_DATE,CUSTOMER_CODE,SALES_NO, GET_SALES_INVOICE_AMOUNT(SALES_NO) AMOUNT
                                    FROM SA_SALES_INVOICE WHERE CUSTOMER_CODE = '" + CustomerCode + "'";
            DataSet ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, TransactionSql);
            DataTable tblGroup = ds.Tables[0];
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in tblGroup.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in tblGroup.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            return serializer.Serialize(rows);
        }

        internal static string GetFilterTree()
        {
            DataTable tblGroup = new DataTable();
            //            string sql = @"SELECT distinct A.CUSTOMER_CODE,A.CUSTOMER_EDESC CUSTOMER_EDESC,A.MASTER_CUSTOMER_CODE,PRE_CUSTOMER_CODE,LEVEL,'FALSE' EXPANDED,'TRUE' LOADED, 
            //                            DECODE(GROUP_SKU_FLAG,'G',0,1) ISLEAF
            //                            FROM SA_CUSTOMER_SETUP A
            //                            where group_sku_flag='G'
            //                            CONNECT BY PRIOR TRIM(MASTER_CUSTOMER_CODE) = TRIM(PRE_CUSTOMER_CODE)
            //                            START WITH PRE_CUSTOMER_CODE = '00'
            //                            ORDER BY MASTER_CUSTOMER_CODE";
            string sql = @"SELECT  A.CUSTOMER_CODE, initcap(A.CUSTOMER_EDESC) CUSTOMER_EDESC,A.PARRENT_CODE,A.MASTER_CUSTOMER_CODE,PRE_CUSTOMER_CODE,LEVEL,'FALSE' EXPANDED,'TRUE' LOADED,my_is_leaf(A.CUSTOMER_CODE) ISLEAF
                            FROM SA_CUSTOMER_SETUP A where GROUP_SKU_FLAG='G'
                            CONNECT BY PRIOR TRIM(CUSTOMER_CODE)=TRIM(PARRENT_CODE)
                            START WITH PRE_CUSTOMER_CODE='00'";

            DataSet ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, sql);
            tblGroup = ds.Tables[0];
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in tblGroup.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in tblGroup.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            return serializer.Serialize(rows);
        }

        internal static string GetFilteredCustomerList(string values)
        {
            string InString = null;
            string[] CustomerCodes = values.Split(',');
            foreach (string CustomerCode in CustomerCodes)
            {
                InString = InString + "'" + CustomerCode + "',";
            }
            InString = InString.Remove(InString.Length - 1);
            string GetSql = @"SELECT  DISTINCT A.CUSTOMER_CODE, initcap(A.CUSTOMER_EDESC) CUSTOMER_EDESC,A.PARRENT_CODE 
                            FROM SA_CUSTOMER_SETUP A where GROUP_SKU_FLAG='I'
                            CONNECT BY PRIOR TRIM(CUSTOMER_CODE)=TRIM(PARRENT_CODE)
                            START WITH PARRENT_CODE IN (" + InString + ")";
            DataSet ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, GetSql);
            DataTable CustumerList = ds.Tables[0];
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> lrow;
            foreach (DataRow dr in CustumerList.Rows)
            {
                lrow = new Dictionary<string, object>();
                foreach (DataColumn col in CustumerList.Columns)
                {
                    lrow.Add(col.ColumnName, dr[col]);
                }
                rows.Add(lrow);
            }
            return serializer.Serialize(rows);
        }

        internal static void InsertSelectedCustomers(string Ids)
        {
            int MaxId = 1;
            string TruncSql = "TRUNCATE TABLE CUST_AGE_CUST_CODE";
            OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, TruncSql);
            string[] CustomerId = Ids.Split(',');
            foreach (string id in CustomerId)
            {
                string InsertSql = "INSERT INTO CUST_AGE_CUST_CODE VALUES ('" + id + "','" + MaxId + "')";
                OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, InsertSql);
                MaxId++;
            }
        }

        internal static string getTlineData()
        {
            try
            {
                string sql = @"SELECT TLINE FROM CUST_AGE_TLINE ORDER BY TLINE";

                DataSet ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, sql);
                DataTable Tline = ds.Tables[0];
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                Dictionary<string, object> lrow;
                foreach (DataRow dr in Tline.Rows)
                {
                    lrow = new Dictionary<string, object>();
                    foreach (DataColumn col in Tline.Columns)
                    {
                        lrow.Add(col.ColumnName, dr[col]);
                    }
                    rows.Add(lrow);
                }
                return serializer.Serialize(rows);
            }
            catch
            {
                throw new NotImplementedException();
            }

        }

        public static string TotalAgingReportInAjax()
        {

            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> lrow;
            foreach (DataRow dr in CustTable.Rows)
            {
                lrow = new Dictionary<string, object>();
                foreach (DataColumn col in CustTable.Columns)
                {
                    lrow.Add(col.ColumnName, dr[col]);
                }
                rows.Add(lrow);
            }
            return serializer.Serialize(rows);
        }


        internal static List<object> GetAllColModals()
        {
            string[] columnNames = (from dc in CustTable.Columns.Cast<DataColumn>()
                                    select dc.ColumnName).ToArray();
            var movies = new List<object>();
            foreach (string ColModal in columnNames)
            {

                if (ColModal != "Level" && ColModal != "IsLeaf" && ColModal != "expanded" && ColModal != "loaded")
                {
                    if (ColModal == "CustomerCode" || ColModal == "ParentCode")
                    {
                        if (ColModal == "CustomerCode")
                        {
                            movies.Add(new { name = ColModal, index = ColModal, hidden = true, key = true });
                        }
                        else
                        {
                            movies.Add(new { name = ColModal, index = ColModal, hidden = true, key = false });
                        }
                    }
                    else
                    {
                        if (ColModal == "CustomerName")
                        {
                            movies.Add(new { name = ColModal, index = ColModal, hidden = false, key = false });
                        }
                        else
                        {
                            movies.Add(new { name = ColModal, index = ColModal, hidden = false, key = false, align = "right", summaryType = "sum" });
                        }
                    }
                }
            }
            return movies;
        }


        internal static string GetAllItems(string SalesNo)
        {
            //try
            //{
            string ItemSql = @"SELECT to_char(A.SALES_DATE,'dd-MON-IYYY') SALES_DATE,A.QUANTITY,A.UNIT_PRICE,A.TOTAL_PRICE,A.REMARKS,A.CREATED_BY,
                                B.ITEM_EDESC,C.MU_EDESC FROM SA_SALES_INVOICE A,IP_ITEM_MASTER_SETUP B,IP_MU_CODE C
                                WHERE A.SALES_NO = '" + SalesNo + "' AND A.ITEM_CODE = B.ITEM_CODE AND A.MU_CODE = C.MU_CODE";

            DataSet ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, ItemSql);
            DataTable tblGroup = ds.Tables[0];
            object SumQuantityObj;
            SumQuantityObj = tblGroup.Compute("Sum(QUANTITY)", "");
            object UnitPriceObj;
            UnitPriceObj = tblGroup.Compute("Sum(UNIT_PRICE)", "");
            object TotalObj;
            TotalObj = tblGroup.Compute("Sum(TOTAL_PRICE)", "");

            DataRow drow = tblGroup.NewRow();
            drow["SALES_DATE"] = "";
            drow["QUANTITY"] = Convert.ToDecimal(SumQuantityObj);
            drow["UNIT_PRICE"] = Convert.ToDecimal(UnitPriceObj);
            drow["TOTAL_PRICE"] = Convert.ToDecimal(TotalObj);
            drow["ITEM_EDESC"] = "<b>Total</b>";
            drow["REMARKS"] = "";
            drow["CREATED_BY"] = "";
            tblGroup.Rows.Add(drow);
            decimal Discount = 0;
            string DiscountSql = "SELECT sum(CHARGE_AMOUNT) CHARGE_AMOUNT FROM CHARGE_TRANSACTION WHERE REFERENCE_NO =  '" + SalesNo + "' and charge_type_flag='D'";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, DiscountSql))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        Discount = string.IsNullOrEmpty(sdr["CHARGE_AMOUNT"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["CHARGE_AMOUNT"].ToString());

                        drow = tblGroup.NewRow();
                        drow["SALES_DATE"] = "";
                        drow["QUANTITY"] = 0;
                        drow["UNIT_PRICE"] = 0;
                        drow["TOTAL_PRICE"] = Discount;
                        drow["ITEM_EDESC"] = "<b>Discount<b>";
                        drow["REMARKS"] = "";
                        drow["CREATED_BY"] = "";
                        tblGroup.Rows.Add(drow);
                    }
                }
            }


            //tblGroup.Rows.Add(drow);
            decimal VatAmount = 0;
            string VatSql = "SELECT sum(CHARGE_AMOUNT) CHARGE_AMOUNT FROM CHARGE_TRANSACTION WHERE REFERENCE_NO =  '" + SalesNo + "' and charge_type_flag='A'";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, VatSql))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        VatAmount = string.IsNullOrEmpty(sdr["CHARGE_AMOUNT"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["CHARGE_AMOUNT"].ToString());

                        drow = tblGroup.NewRow();
                        drow["SALES_DATE"] = "";
                        drow["QUANTITY"] = 0;
                        drow["UNIT_PRICE"] = 0;
                        drow["TOTAL_PRICE"] = VatAmount;
                        drow["ITEM_EDESC"] = "<b>VAT<b>";
                        drow["REMARKS"] = "";
                        drow["CREATED_BY"] = "";
                        tblGroup.Rows.Add(drow);
                    }
                }
            }

            drow = tblGroup.NewRow();
            drow["SALES_DATE"] = "";
            drow["QUANTITY"] = 0;
            drow["UNIT_PRICE"] = 0;
            drow["TOTAL_PRICE"] = Convert.ToDecimal(TotalObj) - Discount + VatAmount;
            drow["ITEM_EDESC"] = "<b>Grand Total<b>";
            drow["REMARKS"] = "";
            drow["CREATED_BY"] = "";
            tblGroup.Rows.Add(drow);

            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in tblGroup.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in tblGroup.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            return serializer.Serialize(rows);
            //}
            //catch
            //{
            //    throw new NotImplementedException();
            //}
        }

        internal static string customerName(string customer)
        {
            try
            {
                string CustomerName = "";
                return "";
            }
            catch
            {
                throw new NotImplementedException();
            }
        }

        internal static string GetFromDate(string Todate)
        {
            DateTime TodateDt = Convert.ToDateTime(Todate);


            int MaxTline = 0;
            string Sql = "select Max(TLINE) MAXTLINE from CUST_AGE_TLINE ";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, Sql))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        MaxTline = Convert.ToInt32(sdr["MAXTLINE"]);
                    }
                }
            }
            string DateDiff = TodateDt.AddDays(-MaxTline).ToString("yyyy-MMM-dd");
            return DateDiff;
        }
    }
}

