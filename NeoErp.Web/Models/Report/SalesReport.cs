using NeoErp.Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.OracleClient;
using System.Globalization;
using System.Linq;
using System.Text;

namespace NeoErp.Models.Report
{
    public class SalesReport
    {

        //####MONTHLY REPORT GENERATION############//
        public static DataTable getData(string type, string FromDate, string ToDate, string minAmt, string maxAmt)
        {

            DataTable tblGroup = new DataTable();
            string totalfilterStrQuery = null;
            if (minAmt != "" && maxAmt == "")
            {
                totalfilterStrQuery = " having sum(a.total_price)>=" + minAmt;
            }
            if (maxAmt != "" && minAmt == "")
            {
                totalfilterStrQuery = " having sum(a.total_price)<" + maxAmt;
            }
            if (minAmt != "" && maxAmt != "")
            {
                totalfilterStrQuery = " having (sum(a.total_price)>=" + minAmt + " and sum(a.total_price)<" + maxAmt + ")";
            }


            if (type == "Product,Customer" || type == "Customer,Product")
            {

                string sql = @"SELECT   TO_CHAR (a.sales_date, 'IYYY') AS YEAR,
                                                TO_CHAR (a.sales_date, 'MM') AS MONTH, d.category_edesc, c.pre_item_code,e.customer_code,c.item_code,c.item_edesc, 
                                                (SELECT item_edesc FROM ip_item_master_setup WHERE master_item_code = c.pre_item_code) AS PARENT,
                                                SUM (a.quantity) AS quantity, SUM (a.total_price) AS price,
                                                NVL((SELECT item_edesc FROM ip_item_master_setup WHERE master_item_code = (SELECT pre_item_code FROM ip_item_master_setup WHERE master_item_code = c.pre_item_code)),'Not Defined')
                                                AS grandparent,e.customer_edesc,
                                                (select customer_edesc from sa_customer_setup where master_customer_code = (select pre_customer_code from sa_customer_setup where customer_code=a.customer_code ) and GROUP_SKU_FLAG = 'G') CUSTOMERPARENT,
                                                NVL((SELECT INITCAP(CUSTOMER_EDESC) FROM SA_CUSTOMER_SETUP WHERE MASTER_CUSTOMER_CODE  = (SELECT PRE_CUSTOMER_CODE FROM SA_CUSTOMER_SETUP WHERE MASTER_CUSTOMER_CODE = (select pre_customer_code from sa_customer_setup where master_customer_code = (select pre_customer_code from sa_customer_setup where customer_code=a.customer_code ) and GROUP_SKU_FLAG = 'G') AND GROUP_SKU_FLAG = 'G')),INITCAP('NOT-SPECIFIED')) GRANDPARENTCUSTOMER 
                                                FROM sa_sales_invoice a, ip_item_master_setup c, ip_category_code d,sa_customer_setup e
                                                WHERE (a.sales_date BETWEEN '" + Convert.ToDateTime(FromDate).ToString("dd-MMM-yyyy") + "' AND '" + Convert.ToDateTime(ToDate).ToString("dd-MMM-yyyy") + "')" +
                            @"AND a.item_code = c.item_code
                                                AND c.category_code = d.category_code
                                                AND a.company_code = c.company_code
                                                AND a.customer_code=e.customer_code" + totalfilterStrQuery +
                            @" GROUP BY TO_CHAR (a.sales_date, 'IYYY'),
                                                TO_CHAR (a.sales_date, 'MM'),
                                                a.mu_code,
                                                a.customer_code,
                                                c.pre_item_code,
                                                c.item_code,
                                                c.item_edesc,
                                                d.category_edesc,
                                                e.customer_edesc,
                                                e.customer_code,
                                                e.pre_customer_code
                                                ORDER BY TO_CHAR (A.sales_date, 'IYYY'),
                                                TO_CHAR (A.sales_date, 'MM'),
                                                d.category_edesc
                                                ";

                string sql1 = @"SELECT   TO_CHAR (a.sales_date, 'IYYY') AS YEAR,
TO_CHAR (a.sales_date, 'MM') AS MONTH,e.customer_code,c.item_code,c.item_edesc, 
SUM (a.quantity) AS quantity, SUM (a.total_price) AS price,
e.customer_edesc 
FROM sa_sales_invoice a, ip_item_master_setup c, sa_customer_setup e
WHERE (a.sales_date BETWEEN '" + Convert.ToDateTime(FromDate).ToString("dd-MMM-yyyy") + "' AND '" + Convert.ToDateTime(ToDate).ToString("dd-MMM-yyyy") + "')" +
@"AND a.item_code = c.item_code
AND a.customer_code=e.customer_code " + totalfilterStrQuery +
@"GROUP BY TO_CHAR (a.sales_date, 'IYYY'),
TO_CHAR (a.sales_date, 'MM'),
a.customer_code,
c.item_code,
c.item_edesc,
e.customer_edesc,
e.customer_code
ORDER BY TO_CHAR (A.sales_date, 'IYYY'),
TO_CHAR (A.sales_date, 'MM')";
                DataSet ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, sql1);
                tblGroup = ds.Tables[0];
            }

            if (type == "Product")
            {
                string sql = @"SELECT   TO_CHAR (a.sales_date, 'IYYY') AS YEAR,
TO_CHAR (a.sales_date, 'MM') AS MONTH,a.item_code,sum(a.quantity) quantity,sum(a.total_price) price,b.item_edesc,'' customer_code, '' customer_edesc
from sa_sales_invoice a,ip_item_master_setup b
WHERE (a.sales_date BETWEEN '" + Convert.ToDateTime(FromDate).ToString("dd-MMM-yyyy") + "' AND '" + Convert.ToDateTime(ToDate).ToString("dd-MMM-yyyy") + "')" +
@"and a.item_code=b.item_code
group by 
TO_CHAR (a.sales_date, 'IYYY'),
TO_CHAR (a.sales_date, 'MM'),
a.item_code,
b.item_edesc" + totalfilterStrQuery;


                DataSet ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, sql);
                tblGroup = ds.Tables[0];
            }
            else if (type == "Customer")
            {
                string sql = @"SELECT   TO_CHAR (a.sales_date, 'IYYY') AS YEAR,
                                TO_CHAR (a.sales_date, 'MM') AS MONTH, d.category_edesc, e.pre_customer_code,e.customer_code,e.customer_edesc ITEM_EDESC,
                                SUM (a.quantity) AS quantity, SUM (a.total_price) AS price,c.item_code,c.item_edesc
                                FROM sa_sales_invoice a, ip_item_master_setup c, ip_category_code d,sa_customer_setup e
                                WHERE (a.sales_date BETWEEN '" + FromDate + "' AND '" + ToDate + "')"
                                + @"AND c.category_code = d.category_code
                                AND a.company_code = c.company_code
                                AND a.customer_code in (select customer_code from sales_cust_code)
                                AND a.customer_code=e.customer_code " + totalfilterStrQuery + " GROUP BY TO_CHAR (a.sales_date, 'IYYY')," +
                               @"TO_CHAR (a.sales_date, 'MM'),
                                a.mu_code,
                                e.customer_code,
                                c.item_code,
                                c.item_edesc,
                                e.pre_customer_code,
                                d.category_edesc,
                                e.customer_edesc";

                string sql1 = @"SELECT   TO_CHAR (a.sales_date, 'IYYY') AS YEAR,
TO_CHAR (a.sales_date, 'MM') AS MONTH,a.customer_code,sum(a.quantity) quantity,sum(a.total_price) price,b.customer_edesc,'' item_code, '' item_edesc
from sa_sales_invoice a,sa_customer_setup b
WHERE (a.sales_date BETWEEN '" + Convert.ToDateTime(FromDate).ToString("dd-MMM-yyyy") + "' AND '" + Convert.ToDateTime(ToDate).ToString("dd-MMM-yyyy") + "')" +
@"and a.customer_code=b.customer_code
group by 
TO_CHAR (a.sales_date, 'IYYY'),
TO_CHAR (a.sales_date, 'MM'),
a.customer_code,
b.customer_edesc" + totalfilterStrQuery + "";


                DataSet ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, sql1);
                tblGroup = ds.Tables[0];
            }


            return tblGroup;

        }



        //############ FUNCTION TO STORE PREFRENCE################////
        internal static void InsertIntoSetup(string PreferenceName, string FromDate, string ToDate, string DateStep, string AmountFlag, string QuantityFlag, string AmountFig, string QuantityFig, string AmountRound, string QuantityRound, string MinAmt, string MaxAmt, string Row, string Grp, string SetDefault)
        {
            string PrefCode = null;
            string AmountCheck = null;
            string QuantityCheck = null;
            string row = null;
            string grp = null;
            string def = null;
            if (SetDefault == "on")
            {
                def = "Y";
            }
            else
            {
                def = "N";
            }
            switch (Row)
            {
                case "Product": row = "P";
                    break;
                case "Customer": row = "C";
                    break;
                case "ProductTree": row = "A";
                    break;
                case "CustomerTree": row = "B";
                    break;
            }
            switch (Grp)
            {
                case "Product": grp = "P";
                    break;
                case "Customer": grp = "C";
                    break;
            }
            if (AmountFlag == "on")
            {
                AmountCheck = "Y";
            }
            else
            {
                AmountCheck = "N";
            }
            if (QuantityFlag == "on")
            {
                QuantityCheck = "Y";
            }
            else
            {
                QuantityCheck = "N";
            }

            string GetPrefName = "SELECT PREF_CODE,PREF_NAME FROM SALES_REPORT_PREF_MASTER";
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
                    string UpdateMasterSql = "UPDATE SALES_REPORT_PREF_MASTER SET FROM_DATE='" + FromDate + "' TO_DATE='" + ToDate + "' DATE_STEP='" + DateStep + "' AMOUNT_FLAG='" + AmountCheck + "' QUANTITY_FLAG='" + QuantityFlag + "' AMOUNT_FIGURE='" + AmountFig + "' QUANTITY_FIGURE='" + QuantityFig + "'AMOUNT_ROUNDUP='" + AmountRound + "' QUANTITY_ROUNDUP='" + QuantityRound + "' MIN_AMOUNT='" + MinAmt + "' MAX_AMOUNT='" + MaxAmt + "' RW='" + row + "' GRP='" + grp + "' ISDEFAULT='" + def + "' WHERE PREF_CODE = '" + PrefCode + "'";
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, UpdateMasterSql);
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, "COMMIT");
                }
                else
                {
                    string UpdateDefaultFlag = "UPDATE SALES_REPORT_PREF_MASTER SET ISDEFAULT = 'N'";
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, UpdateDefaultFlag);
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, "COMMIT");
                    string UpdateMasterSql = "UPDATE SALES_REPORT_PREF_MASTER SET FROM_DATE='" + FromDate + "', TO_DATE='" + ToDate + "', DATE_STEP='" + DateStep + "' ,AMOUNT_FLAG='" + AmountCheck + "', QUANTITY_FLAG='" + QuantityCheck + "', AMOUNT_FIGURE='" + AmountFig + "', QUANTITY_FIGURE='" + QuantityFig + "' ,AMOUNT_ROUNDUP='" + AmountRound + "', QUANTITY_ROUNDUP='" + QuantityRound + "', MIN_AMOUNT='" + MinAmt + "', MAX_AMOUNT='" + MaxAmt + "', RW='" + row + "', GRP='" + grp + "', ISDEFAULT='" + def + "' WHERE PREF_CODE = '" + PrefCode + "'";
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, UpdateMasterSql);
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, "COMMIT");
                }
                //string DeleteDetailSql = "DELETE FROM SALES_REPORT_PREF_MASTER WHERE PREF_CODE = '" + PrefCode + "'";
                //OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, DeleteDetailSql);
                OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, "COMMIT");

            }
            else
            {
                int PREF_CODE = 0;
                string findmaxcode = "SELECT MAX(PREF_CODE) PREF_CODE FROM SALES_REPORT_PREF_MASTER";
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
                    string UpdateDefaultFlag = "UPDATE SALES_REPORT_PREF_MASTER SET ISDEFAULT = 'N'";
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, UpdateDefaultFlag);
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, "COMMIT");
                    inserty = @"INSERT INTO SALES_REPORT_PREF_MASTER(PREF_NAME, PREF_CODE, FROM_DATE, TO_DATE, DATE_STEP, AMOUNT_FLAG, QUANTITY_FLAG, AMOUNT_FIGURE, QUANTITY_FIGURE, AMOUNT_ROUNDUP, QUANTITY_ROUNDUP, MIN_AMOUNT, MAX_AMOUNT, RW, GRP, ISDEFAULT)VALUES
                            ('" + PreferenceName + "','" + PREF_CODE + "','" + FromDate + "','" + ToDate + "','" + DateStep + "','" + AmountCheck + "','" + QuantityCheck + "','" + AmountFig + "','" + QuantityFig + "','" + AmountRound + "','" + QuantityRound + "','" + MinAmt + "','" + MaxAmt + "','" + row + "','" + grp + "','Y')";
                }
                else
                {
                    inserty = @"INSERT INTO SALES_REPORT_PREF_MASTER(PREF_NAME, PREF_CODE, FROM_DATE, TO_DATE, DATE_STEP, AMOUNT_FLAG, QUANTITY_FLAG, AMOUNT_FIGURE, QUANTITY_FIGURE, AMOUNT_ROUNDUP, QUANTITY_ROUNDUP, MIN_AMOUNT, MAX_AMOUNT, RW, GRP, ISDEFAULT)VALUES
                            ('" + PreferenceName + "','" + PrefCode + "','" + FromDate + "','" + ToDate + "','" + DateStep + "','" + AmountCheck + "','" + QuantityCheck + "','" + AmountFig + "','" + QuantityFig + "','" + AmountRound + "','" + QuantityRound + "','" + MinAmt + "','" + MaxAmt + "','" + row + "','" + grp + "','N')";
                }
                OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, inserty);
                OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, "COMMIT");

            }
        }



        //#############FUNCTION FOR FILTER##########//
        public static string filterProduct()
        {

            string sql = @"select category_edesc,category_code from ip_category_code";
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

        //###########FILTERED DATA###################//
        public static DataTable getDataFiltered(string FromDate, string ToDate, string Filter)
        {
            string sql = @"SELECT   TO_CHAR (a.sales_date, 'IYYY') AS YEAR,
TO_CHAR (a.sales_date, 'MM') AS MONTH, d.category_edesc, c.pre_item_code,e.customer_code,c.item_code,c.item_edesc, 
(SELECT item_edesc FROM ip_item_master_setup WHERE master_item_code = c.pre_item_code) AS PARENT,
SUM (a.quantity) AS quantity, SUM (a.total_price) AS price,
NVL((SELECT item_edesc FROM ip_item_master_setup WHERE master_item_code = (SELECT pre_item_code FROM ip_item_master_setup WHERE master_item_code = c.pre_item_code)),'Not Defined')
AS grandparent,e.customer_edesc,
(select customer_edesc from sa_customer_setup where master_customer_code = (select pre_customer_code from sa_customer_setup where customer_code=a.customer_code ) and GROUP_SKU_FLAG = 'G') CUSTOMERPARENT,
NVL((SELECT INITCAP(CUSTOMER_EDESC) FROM SA_CUSTOMER_SETUP WHERE MASTER_CUSTOMER_CODE  = (SELECT PRE_CUSTOMER_CODE FROM SA_CUSTOMER_SETUP WHERE MASTER_CUSTOMER_CODE = (select pre_customer_code from sa_customer_setup where master_customer_code = (select pre_customer_code from sa_customer_setup where customer_code=a.customer_code ) and GROUP_SKU_FLAG = 'G') AND GROUP_SKU_FLAG = 'G')),INITCAP('NOT-SPECIFIED')) GRANDPARENTCUSTOMER 
FROM sa_sales_invoice a, ip_item_master_setup c, ip_category_code d,sa_customer_setup e
WHERE (a.sales_date BETWEEN '" + FromDate + "' AND '" + ToDate + "')" +
@"AND a.item_code = c.item_code
AND c.category_code = d.category_code
AND a.company_code = c.company_code
AND a.customer_code=e.customer_code
AND c.category_code='" + Filter + "'" +
@"GROUP BY TO_CHAR (a.sales_date, 'IYYY'),
TO_CHAR (a.sales_date, 'MM'),
a.mu_code,
a.customer_code,
c.pre_item_code,
c.item_code,
c.item_edesc,
d.category_edesc,
e.customer_edesc,
e.customer_code,
e.pre_customer_code
ORDER BY TO_CHAR (A.sales_date, 'IYYY'),
TO_CHAR (A.sales_date, 'MM'),
d.category_edesc
";
            DataSet ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, sql);
            DataTable tblGroup = ds.Tables[0];
            return tblGroup;
        }


        //######WEEKLY REPORT GENERATION############//
        public static DataTable getDataWeekly(string FromDate, string ToDate)
        {
            string sql = @"SELECT   TO_CHAR (a.sales_date, 'IYYY') AS YEAR,
TO_CHAR (a.sales_date, 'MM') AS MONTH,
TO_CHAR (a.sales_date, 'IW') AS week, d.category_edesc, c.pre_item_code,e.customer_code,c.item_code,c.item_edesc, 
(SELECT item_edesc FROM ip_item_master_setup WHERE master_item_code = c.pre_item_code) AS PARENT,
SUM (a.quantity) AS quantity, SUM (a.total_price) AS price,
NVL((SELECT item_edesc FROM ip_item_master_setup WHERE master_item_code = (SELECT pre_item_code FROM ip_item_master_setup WHERE master_item_code = c.pre_item_code)),'Not Defined')
AS grandparent,e.customer_edesc,
(select customer_edesc from sa_customer_setup where master_customer_code = (select pre_customer_code from sa_customer_setup where customer_code=a.customer_code ) and GROUP_SKU_FLAG = 'G') CUSTOMERPARENT,
NVL((SELECT INITCAP(CUSTOMER_EDESC) FROM SA_CUSTOMER_SETUP WHERE MASTER_CUSTOMER_CODE  = (SELECT PRE_CUSTOMER_CODE FROM SA_CUSTOMER_SETUP WHERE MASTER_CUSTOMER_CODE = (select pre_customer_code from sa_customer_setup where master_customer_code = (select pre_customer_code from sa_customer_setup where customer_code=a.customer_code ) and GROUP_SKU_FLAG = 'G') AND GROUP_SKU_FLAG = 'G')),INITCAP('NOT-SPECIFIED')) GRANDPARENTCUSTOMER 
FROM sa_sales_invoice a, ip_item_master_setup c, ip_category_code d,sa_customer_setup e
WHERE (a.sales_date BETWEEN '" + FromDate + "' AND '" + ToDate + "')" +
@"AND a.item_code = c.item_code
AND c.category_code = d.category_code
AND a.company_code = c.company_code
AND a.customer_code=e.customer_code
GROUP BY TO_CHAR (a.sales_date, 'IYYY'),
TO_CHAR (a.sales_date, 'MM'),
TO_CHAR (a.sales_date, 'IW'),
a.mu_code,
a.customer_code,
c.pre_item_code,
c.item_code,
c.item_edesc,
d.category_edesc,
e.customer_edesc,
e.customer_code,
e.pre_customer_code
ORDER BY TO_CHAR (A.sales_date, 'IYYY'),
TO_CHAR (A.sales_date, 'MM'),
TO_CHAR (A.sales_date, 'IW'),
d.category_edesc";




            DataSet ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, sql);
            DataTable tblGroup = ds.Tables[0];
            return tblGroup;
        }

        //Quarter
        public static DataTable getDataQuarterly(string FromDate, string ToDate)
        {
            string sql = @"SELECT   TO_CHAR (a.sales_date, 'IYYY') AS YEAR,
TO_CHAR (a.sales_date, 'MM') AS MONTH,
to_char(A.sales_date,'Q') as Quarter , d.category_edesc, c.pre_item_code,e.customer_code,c.item_code,c.item_edesc, 
(SELECT item_edesc FROM ip_item_master_setup WHERE master_item_code = c.pre_item_code) AS PARENT,
SUM (a.quantity) AS quantity, SUM (a.total_price) AS price,
NVL((SELECT item_edesc FROM ip_item_master_setup WHERE master_item_code = (SELECT pre_item_code FROM ip_item_master_setup WHERE master_item_code = c.pre_item_code)),'Not Defined')
AS grandparent,e.customer_edesc,
(select customer_edesc from sa_customer_setup where master_customer_code = (select pre_customer_code from sa_customer_setup where customer_code=a.customer_code ) and GROUP_SKU_FLAG = 'G') CUSTOMERPARENT,
NVL((SELECT INITCAP(CUSTOMER_EDESC) FROM SA_CUSTOMER_SETUP WHERE MASTER_CUSTOMER_CODE  = (SELECT PRE_CUSTOMER_CODE FROM SA_CUSTOMER_SETUP WHERE MASTER_CUSTOMER_CODE = (select pre_customer_code from sa_customer_setup where master_customer_code = (select pre_customer_code from sa_customer_setup where customer_code=a.customer_code ) and GROUP_SKU_FLAG = 'G') AND GROUP_SKU_FLAG = 'G')),INITCAP('NOT-SPECIFIED')) GRANDPARENTCUSTOMER 
FROM sa_sales_invoice a, ip_item_master_setup c, ip_category_code d,sa_customer_setup e
WHERE (a.sales_date BETWEEN '" + FromDate + "' AND '" + ToDate + "')" +
@"AND a.item_code = c.item_code
AND c.category_code = d.category_code
AND a.company_code = c.company_code
AND a.customer_code=e.customer_code
GROUP BY TO_CHAR (a.sales_date, 'IYYY'),
TO_CHAR (a.sales_date, 'MM'),
TO_CHAR (a.sales_date, 'Q'),
a.mu_code,
a.customer_code,
c.pre_item_code,
c.item_code,
c.item_edesc,
d.category_edesc,
e.customer_edesc,
e.customer_code,
e.pre_customer_code
ORDER BY TO_CHAR (A.sales_date, 'IYYY'),
TO_CHAR (A.sales_date, 'MM'),
TO_CHAR (A.sales_date, 'Q'),
d.category_edesc";
            DataSet ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, sql);
            DataTable tblGroup = ds.Tables[0];
            return tblGroup;
        }

        //#########MONTHLY REPORT GENERATION IN NEPALI###############//
        public static DataTable getDataMonthlyNepali(string FromDate, string ToDate)
        {


            string sql = @"SELECT   substr(to_bs(a.sales_date),7,4) AS YEAR,
            substr(to_bs (a.sales_date),4,2 ) AS MONTH,
            d.category_edesc,
            c.item_edesc,
            SUM (a.quantity)
            AS quantity, SUM (a.total_price) AS price,(SELECT item_edesc
            FROM ip_item_master_setup
            WHERE master_item_code = c.pre_item_code) AS PARENT,
            (SELECT item_edesc
            FROM ip_item_master_setup
            WHERE master_item_code = (SELECT pre_item_code
            FROM ip_item_master_setup
            WHERE master_item_code = c.pre_item_code))
            AS grandparent
            FROM sa_sales_invoice a, ip_item_master_setup c, ip_category_code d
            WHERE (a.sales_date BETWEEN '15-Jan-2015' AND '16-Jul-2015')
            AND a.item_code = c.item_code
            AND c.category_code = d.category_code
            AND a.company_code = c.company_code
            GROUP BY substr(to_bs(a.sales_date),7,4)   ,
            substr(to_bs (a.sales_date),4,2 )  ,
            a.mu_code,
            c.pre_item_code,
            c.item_edesc,
            d.category_edesc
            ORDER BY substr(to_bs(a.sales_date),7,4)  ,
            substr(to_bs (a.sales_date),4,2 ) ,
            d.category_edesc";
            DataSet ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, sql);
            DataTable tblGroup = ds.Tables[0];
            return tblGroup;
        }




        //#######TREE REPORT###############//
        public static string getTree()
        {
            string sql = @"SELECT  A.CUSTOMER_CODE, initcap(A.CUSTOMER_EDESC) CUSTOMER_EDESC,A.PARRENT_CODE,A.MASTER_CUSTOMER_CODE,PRE_CUSTOMER_CODE,LEVEL,'FALSE' EXPANDED,'TRUE' LOADED,my_is_leaf(A.CUSTOMER_CODE) ISLEAF
                            FROM SA_CUSTOMER_SETUP A where GROUP_SKU_FLAG='G'
                            CONNECT BY PRIOR TRIM(CUSTOMER_CODE)=TRIM(PARRENT_CODE)
                            START WITH PRE_CUSTOMER_CODE='00'
";
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

        public static DataTable PivotTable = new DataTable();
        public static DataColumn column;
        public static DataRow row;


        //########MANUALLY PIVOT TABLE########//
        public static DataTable pivotTable(string FromDate, string ToDate, string figureA, string figureQ, string roundupA, string roundupQ, string type, string minAmt, string maxAmt, string QuantityCheckbox, string AmountCheckbox)
        {
            FromDate = Convert.ToDateTime(FromDate).ToString("dd-MMM-yyyy");
            ToDate = Convert.ToDateTime(ToDate).ToString("dd-MMM-yyyy");
            PivotTable.Clear();
            PivotTable.Columns.Clear();
            PivotTable.Rows.Clear();
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "CustomerCode";
            PivotTable.Columns.Add(column);
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "CustomerName";
            PivotTable.Columns.Add(column);
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "ParentCode";
            PivotTable.Columns.Add(column);
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "Level";
            PivotTable.Columns.Add(column);
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "IsLeaf";
            PivotTable.Columns.Add(column);
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "loaded";
            PivotTable.Columns.Add(column);
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "expanded";
            PivotTable.Columns.Add(column);
            string totalfilterStrQuery = null;
            if (minAmt != "" && maxAmt == "")
            {
                totalfilterStrQuery = " having sum(total_price)>=" + minAmt;
            }
            if (maxAmt != "" && minAmt == "")
            {
                totalfilterStrQuery = " having sum(total_price)<" + maxAmt;
            }
            if (minAmt != "" && maxAmt != "" && minAmt != null && maxAmt != null)
            {
                totalfilterStrQuery = " having (sum(total_price)>=" + minAmt + " and sum(total_price)<" + maxAmt + ")";
            }
            string DateStepSql = @"SELECT distinct TO_CHAR (sales_date, 'MON-IYYY') AS TIMELINE
 from sa_sales_invoice ";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, DateStepSql))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        column = new DataColumn();
                        column.DataType = System.Type.GetType("System.String");
                        column.ColumnName = sdr["TIMELINE"].ToString() + "Quantity";
                        PivotTable.Columns.Add(column);
                        column = new DataColumn();
                        column.DataType = System.Type.GetType("System.String");
                        column.ColumnName = sdr["TIMELINE"].ToString() + "Amount";
                        PivotTable.Columns.Add(column);

                    }
                }
            }
            string TreeSql = "";
            if (type == "CustomerTree")
            {
                TreeSql = @"SELECT  A.CUSTOMER_CODE, initcap(A.CUSTOMER_EDESC) CUSTOMER_EDESC,A.PARRENT_CODE,A.MASTER_CUSTOMER_CODE,PRE_CUSTOMER_CODE,LEVEL,'FALSE' EXPANDED,'TRUE' LOADED,'FALSE' ISLEAF
                                FROM SA_CUSTOMER_SETUP A where GROUP_SKU_FLAG='G'
                                CONNECT BY PRIOR TRIM(CUSTOMER_CODE)=TRIM(PARRENT_CODE)
                                START WITH PRE_CUSTOMER_CODE='00'";
            }
            else if (type == "ProductTree")
            {
                TreeSql = @"SELECT  A.ITEM_CODE CUSTOMER_CODE, initcap(A.ITEM_EDESC) CUSTOMER_EDESC,A.PARRENT_CODE,A.MASTER_ITEM_CODE MASTER_CUSTOMER_CODE,PRE_ITEM_CODE PRE_CUSTOMER_CODE,LEVEL,'FALSE' EXPANDED,'TRUE' LOADED,'FALSE' ISLEAF
                                FROM IP_ITEM_MASTER_SETUP A where GROUP_SKU_FLAG='G'
                                CONNECT BY PRIOR TRIM(ITEM_CODE)=TRIM(PARRENT_CODE)
                                START WITH PRE_ITEM_CODE='00'";
            }
            using (OracleDataReader Treesdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, TreeSql))
            {
                if (Treesdr.HasRows)
                {
                    while (Treesdr.Read())
                    {
                        if (CheckExistance(Treesdr["CUSTOMER_CODE"].ToString()))
                        {
                            row = PivotTable.NewRow();
                            row["CustomerCode"] = Treesdr["CUSTOMER_CODE"].ToString() + "C";
                            row["CustomerName"] = Treesdr["CUSTOMER_EDESC"].ToString();
                            row["ParentCode"] = Treesdr["PARRENT_CODE"].ToString() + "C";
                            row["Level"] = Treesdr["LEVEL"].ToString();
                            row["IsLeaf"] = "false";
                            row["expanded"] = "false";
                            row["loaded"] = "true";
                            string RowStepSql = @"SELECT distinct TO_CHAR (sales_date, 'MON-IYYY') AS TIMELINE
 from sa_sales_invoice where sales_date between '" + FromDate + "' and '" + ToDate + "'";
                            using (OracleDataReader tsdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, RowStepSql))
                            {
                                if (tsdr.HasRows)
                                {
                                    while (tsdr.Read())
                                    {
                                        string GetAmount = @"SELECT SUM(QUANTITY) AMOUNT
                                                            FROM SA_SALES_INVOICE 
                                                            WHERE CUSTOMER_CODE='" + Treesdr["CUSTOMER_CODE"].ToString() + "'" +
                                                            @"AND to_char(SALES_DATE, 'MON-IYYY') = '" + tsdr["TIMELINE"].ToString() + "'" + totalfilterStrQuery;
                                        using (OracleDataReader Fsdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, GetAmount))
                                        {
                                            if (Fsdr.HasRows)
                                            {
                                                while (Fsdr.Read())
                                                {
                                                    if (Fsdr["AMOUNT"].ToString() != "" && Fsdr["AMOUNT"].ToString() != null)
                                                    {

                                                        row[tsdr["TIMELINE"].ToString() + "Quantity"] = Fsdr["AMOUNT"].ToString();
                                                    }
                                                    else
                                                    {
                                                        row[tsdr["TIMELINE"].ToString() + "Quantity"] = null;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            using (OracleDataReader tsdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, RowStepSql))
                            {
                                if (tsdr.HasRows)
                                {
                                    while (tsdr.Read())
                                    {
                                        string GetAmount = @"SELECT SUM(total_price) AMOUNT
                                                            FROM SA_SALES_INVOICE 
                                                            WHERE CUSTOMER_CODE='" + Treesdr["CUSTOMER_CODE"].ToString() + "'" +
                                                            @"AND to_char(SALES_DATE, 'MON-IYYY') = '" + tsdr["TIMELINE"].ToString() + "'" + totalfilterStrQuery;
                                        using (OracleDataReader Fsdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, GetAmount))
                                        {
                                            if (Fsdr.HasRows)
                                            {
                                                while (Fsdr.Read())
                                                {
                                                    if (Fsdr["AMOUNT"].ToString() != "" && Fsdr["AMOUNT"].ToString() != null)
                                                    {

                                                        row[tsdr["TIMELINE"].ToString() + "AMOUNT"] = Fsdr["AMOUNT"].ToString();
                                                    }
                                                    else
                                                    {
                                                        row[tsdr["TIMELINE"].ToString() + "AMOUNT"] = null;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            PivotTable.Rows.Add(row);
                            string Sql = "";
                            if (type == "CustomerTree")
                            {
                                Sql = @"SELECT DISTINCT A.CUSTOMER_CODE,A.CUSTOMER_EDESC,A.PARRENT_CODE,GET_LEVEL_CUST(A.CUSTOMER_CODE) LVL 
                                        FROM SA_CUSTOMER_SETUP A,SA_SALES_INVOICE B 
                                        WHERE A.CUSTOMER_CODE = B.CUSTOMER_CODE 
                                        AND A.PARRENT_CODE ='" + Treesdr["CUSTOMER_CODE"].ToString() + "'";
                            }
                            else if (type == "ProductTree")
                            {
                                Sql = @"SELECT DISTINCT A.ITEM_CODE CUSTOMER_CODE,A.ITEM_EDESC CUSTOMER_EDESC,A.PARRENT_CODE,GET_LEVEL_ITEM(A.ITEM_CODE) LVL 
                                        FROM IP_ITEM_MASTER_SETUP A,SA_SALES_INVOICE B 
                                        WHERE A.ITEM_CODE = B.ITEM_CODE 
                                        AND A.PARRENT_CODE ='" + Treesdr["CUSTOMER_CODE"].ToString() + "'";
                            }
                            using (OracleDataReader Leafsdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, Sql))
                            {
                                if (Leafsdr.HasRows)
                                {
                                    while (Leafsdr.Read())
                                    {
                                        row = PivotTable.NewRow();
                                        row["CustomerCode"] = Leafsdr["CUSTOMER_CODE"].ToString() + "C";
                                        row["CustomerName"] = Leafsdr["CUSTOMER_EDESC"].ToString();
                                        row["ParentCode"] = Leafsdr["PARRENT_CODE"].ToString() + "C";
                                        row["Level"] = Leafsdr["LVL"].ToString();
                                        row["IsLeaf"] = "true";
                                        row["expanded"] = "false";
                                        row["loaded"] = "true";
                                        string RowDateStepSql = @"SELECT distinct TO_CHAR (sales_date, 'MON-IYYY') AS TIMELINE
 from sa_sales_invoice ";

                                        using (OracleDataReader tsdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, RowDateStepSql))
                                        {
                                            if (tsdr.HasRows)
                                            {
                                                while (tsdr.Read())
                                                {
                                                    string GetAmount = "";
                                                    if (type == "CustomerTree")
                                                    {
                                                        GetAmount = @"SELECT SUM(QUANTITY) AMOUNT
                                                                    FROM SA_SALES_INVOICE 
                                                                    WHERE CUSTOMER_CODE='" + Leafsdr["CUSTOMER_CODE"].ToString() + "'" +
                                                                    @"AND to_char(SALES_DATE, 'MON-IYYY') = '" + tsdr["TIMELINE"].ToString() + "'" + totalfilterStrQuery;
                                                    }
                                                    else if (type == "ProductTree")
                                                    {
                                                        GetAmount = @"SELECT SUM(QUANTITY) AMOUNT
                                                                    FROM SA_SALES_INVOICE 
                                                                    WHERE ITEM_CODE='" + Leafsdr["CUSTOMER_CODE"].ToString() + "'" +
                                                                 @"AND to_char(SALES_DATE, 'MON-IYYY') = '" + tsdr["TIMELINE"].ToString() + "'" + totalfilterStrQuery;
                                                    }
                                                    using (OracleDataReader Fsdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, GetAmount))
                                                    {
                                                        if (Fsdr.HasRows)
                                                        {
                                                            while (Fsdr.Read())
                                                            {

                                                                if (Fsdr["AMOUNT"].ToString() != "" && Fsdr["AMOUNT"].ToString() != null)
                                                                {
                                                                    double Amount = Convert.ToDouble(Fsdr["AMOUNT"]);
                                                                    switch (figureQ)
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

                                                                    switch (roundupQ)
                                                                    {
                                                                        case "0": row[tsdr["TIMELINE"].ToString() + "Quantity"] = (Math.Round(Amount, 0)).ToString();
                                                                            break;
                                                                        case "0.0": row[tsdr["TIMELINE"].ToString() + "Quantity"] = (Math.Round(Amount, 1)).ToString();
                                                                            break;
                                                                        case "0.00": row[tsdr["TIMELINE"].ToString() + "Quantity"] = (Math.Round(Amount, 2)).ToString();
                                                                            break;
                                                                        case "0.000": row[tsdr["TIMELINE"].ToString() + "Quantity"] = (Math.Round(Amount, 3)).ToString();
                                                                            break;
                                                                        case "0.0000": row[tsdr["TIMELINE"].ToString() + "Quantity"] = (Math.Round(Amount, 4)).ToString();
                                                                            break;
                                                                    }

                                                                }
                                                                else
                                                                {
                                                                    switch (roundupQ)
                                                                    {
                                                                        case "0": row[tsdr["TIMELINE"].ToString() + "Quantity"] = "0".ToString(); ;
                                                                            break;
                                                                        case "0.0": row[tsdr["TIMELINE"].ToString() + "Quantity"] = "0.0".ToString(); ;
                                                                            break;
                                                                        case "0.00": row[tsdr["TIMELINE"].ToString() + "Quantity"] = "0.00".ToString(); ;
                                                                            break;
                                                                        case "0.000": row[tsdr["TIMELINE"].ToString() + "Quantity"] = "0.000".ToString(); ;
                                                                            break;
                                                                        case "0.0000": row[tsdr["TIMELINE"].ToString() + "Quantity"] = "0.0000".ToString(); ;
                                                                            break;
                                                                    }

                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        using (OracleDataReader tsdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, RowDateStepSql))
                                        {
                                            if (tsdr.HasRows)
                                            {
                                                while (tsdr.Read())
                                                {
                                                    string GetAmount = "";
                                                    if (type == "CustomerTree")
                                                    {
                                                        GetAmount = @"SELECT SUM(total_price) AMOUNT
                                                                        FROM SA_SALES_INVOICE 
                                                                        WHERE CUSTOMER_CODE='" + Leafsdr["CUSTOMER_CODE"].ToString() + "'" +
                                                                 @"AND to_char(SALES_DATE, 'MON-IYYY') = '" + tsdr["TIMELINE"].ToString() + "'" + totalfilterStrQuery;
                                                    }
                                                    else if (type == "ProductTree")
                                                    {
                                                        GetAmount = @"SELECT SUM(total_price) AMOUNT
                                                                        FROM SA_SALES_INVOICE 
                                                                        WHERE ITEM_CODE='" + Leafsdr["CUSTOMER_CODE"].ToString() + "'" +
                                                                     @"AND to_char(SALES_DATE, 'MON-IYYY') = '" + tsdr["TIMELINE"].ToString() + "'" + totalfilterStrQuery;
                                                    }
                                                    using (OracleDataReader Fsdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, GetAmount))
                                                    {
                                                        if (Fsdr.HasRows)
                                                        {
                                                            while (Fsdr.Read())
                                                            {

                                                                if (Fsdr["AMOUNT"].ToString() != "" && Fsdr["AMOUNT"].ToString() != null)
                                                                {
                                                                    double Amount = Convert.ToDouble(Fsdr["AMOUNT"]);
                                                                    switch (figureQ)
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

                                                                    switch (roundupQ)
                                                                    {
                                                                        case "0": row[tsdr["TIMELINE"].ToString() + "AMOUNT"] = (Math.Round(Amount, 0)).ToString();
                                                                            break;
                                                                        case "0.0": row[tsdr["TIMELINE"].ToString() + "AMOUNT"] = (Math.Round(Amount, 1)).ToString();
                                                                            break;
                                                                        case "0.00": row[tsdr["TIMELINE"].ToString() + "AMOUNT"] = (Math.Round(Amount, 2)).ToString();
                                                                            break;
                                                                        case "0.000": row[tsdr["TIMELINE"].ToString() + "AMOUNT"] = (Math.Round(Amount, 3)).ToString();
                                                                            break;
                                                                        case "0.0000": row[tsdr["TIMELINE"].ToString() + "AMOUNT"] = (Math.Round(Amount, 4)).ToString();
                                                                            break;
                                                                    }

                                                                }
                                                                else
                                                                {
                                                                    switch (roundupQ)
                                                                    {
                                                                        case "0": row[tsdr["TIMELINE"].ToString() + "AMOUNT"] = "0".ToString(); ;
                                                                            break;
                                                                        case "0.0": row[tsdr["TIMELINE"].ToString() + "AMOUNT"] = "0.0".ToString(); ;
                                                                            break;
                                                                        case "0.00": row[tsdr["TIMELINE"].ToString() + "AMOUNT"] = "0.00".ToString(); ;
                                                                            break;
                                                                        case "0.000": row[tsdr["TIMELINE"].ToString() + "AMOUNT"] = "0.000".ToString(); ;
                                                                            break;
                                                                        case "0.0000": row[tsdr["TIMELINE"].ToString() + "AMOUNT"] = "0.0000".ToString(); ;
                                                                            break;
                                                                    }

                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        PivotTable.Rows.Add(row);
                                    }
                                }
                            }
                        }
                    }
                }
                return PivotTable;
            }
        }


        private static bool CheckExistance(string CustomerCode)
        {
            string ExistInFilter = "SELECT CUSTOMER CODE FROM SALES_CUST_CODE WHERE CUSTOMER_CODE='" + CustomerCode + "'";


            int Count = 0;


            string GetSql = @"SELECT  DISTINCT A.CUSTOMER_CODE, initcap(A.CUSTOMER_EDESC) CUSTOMER_EDESC,A.PARRENT_CODE 
                            FROM SA_CUSTOMER_SETUP A where GROUP_SKU_FLAG='I'
                            CONNECT BY PRIOR TRIM(CUSTOMER_CODE)=TRIM(PARRENT_CODE)
                            START WITH PARRENT_CODE IN (" + CustomerCode + ")";
            string GetSqlI = @"SELECT  DISTINCT A.ITEM_CODE CUSTOMER_CODE, initcap(A.ITEM_EDESC) CUSTOMER_EDESC,A.PARRENT_CODE 
                            FROM IP_ITEM_MASTER_SETUP A where GROUP_SKU_FLAG='I'
                            CONNECT BY PRIOR TRIM(ITEM_CODE)=TRIM(PARRENT_CODE)
                            START WITH PARRENT_CODE IN (" + CustomerCode + ")";

            using (OracleDataReader tsdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, GetSql))
            using (OracleDataReader ttdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, GetSqlI))
            {
                if (tsdr.HasRows)
                {
                    while (tsdr.Read())
                    {
                        string TheSql = "SELECT COUNT(*) TOTAL FROM SA_SALES_INVOICE WHERE CUSTOMER_CODE='" + tsdr["CUSTOMER_CODE"] + "'";
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
                else if (ttdr.HasRows)
                {
                    while (ttdr.Read())
                    {
                        string TheSql = "SELECT COUNT(*) TOTAL FROM SA_SALES_INVOICE WHERE ITEM_CODE='" + ttdr["CUSTOMER_CODE"] + "'";
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
                    if (Count != 0)
                    {


                        return true;

                    }

                }

            }
            return false;
        }

        public static string TotalAgingReportInAjax()
        {

            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> lrow;
            foreach (DataRow dr in PivotTable.Rows)
            {
                lrow = new Dictionary<string, object>();
                foreach (DataColumn col in PivotTable.Columns)
                {
                    lrow.Add(col.ColumnName, dr[col]);
                }
                rows.Add(lrow);
            }
            return serializer.Serialize(rows);
        }


        internal static List<object> GetAllColModals(string AmountCheckbox, string QuantityCheckbox)
        {
            string[] columnNames = (from dc in PivotTable.Columns.Cast<DataColumn>()
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
                            movies.Add(new { name = ColModal, index = ColModal, hidden = false, key = false, label = "Customer Name" });
                        }
                        else if (ColModal.Contains("Quantity"))
                        {
                            string[] stringSeparators = new string[] { "Quantity" };
                            string[] result = ColModal.Split(stringSeparators,
                          StringSplitOptions.RemoveEmptyEntries);
                            if (QuantityCheckbox == "" || QuantityCheckbox == null)
                            {
                                movies.Add(new { name = ColModal, index = ColModal, hidden = true, key = false, align = "right", summaryType = "sum", label = "Quantity" });
                            }
                            else
                            {
                                movies.Add(new { name = ColModal, index = ColModal, hidden = false, key = false, align = "right", summaryType = "sum", label = "Quantity" });
                            }
                        }
                        else if (ColModal.Contains("Amount"))
                        {
                            string[] stringSeparators = new string[] { "Amount" };
                            string[] result = ColModal.Split(stringSeparators,
                          StringSplitOptions.RemoveEmptyEntries);
                            if (AmountCheckbox == "" || AmountCheckbox == null)
                            {
                                movies.Add(new { name = ColModal, index = ColModal, hidden = true, key = false, align = "right", summaryType = "sum", label = "Amount" });
                            }
                            else
                            {
                                movies.Add(new { name = ColModal, index = ColModal, hidden = false, key = false, align = "right", summaryType = "sum", label = "Amount" });
                            }
                        }
                        else
                        {
                            movies.Add(new { name = ColModal, index = ColModal, hidden = false, key = false, align = "right", summaryType = "sum", label = "Amount" });
                        }
                    }
                }

            }
            return movies;
        }


        internal static void InsertSelectedCustomers(string Ids)
        {
            DataSet CustomerNotSelected;
            try
            {
                int MaxId = 1;
                string TruncSql = "TRUNCATE TABLE SALES_CUST_CODE";
                OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, TruncSql);
                //string NotSelectedQuery = "SELECT CUSTOMER_CODE FROM SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE NOT IN (" + Ids +") AND GROUP_SKU_FLAG='I'";
                //CustomerNotSelected = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, NotSelectedQuery);
                string[] CustomerId = Ids.Split(',');
                foreach (string id in CustomerId)
                {
                    string InsertSql = "INSERT INTO SALES_CUST_CODE VALUES ('" + id + "','" + MaxId + "')";
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, InsertSql);
                    MaxId++;
                }
            }
            catch
            {
                throw new NotImplementedException();
            }
        }

        internal static string getProductFilter()
        {
            string sql = @"SELECT ITEM_CODE,INITCAP(ITEM_EDESC) ITEM_EDESC,PARRENT_CODE,LEVEL,'FALSE' EXPANDED,'TRUE' LOADED,my_item_is_leaf(ITEM_CODE) ISLEAF
                           FROM IP_ITEM_MASTER_SETUP WHERE GROUP_SKU_FLAG = 'G'
                           CONNECT BY PRIOR TRIM(ITEM_CODE)=TRIM(PARRENT_CODE)
                           START WITH PRE_ITEM_CODE = '00'";
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

        internal static string getFilteredItemList(string values)
        {
            string InString = null;
            string[] CustomerCodes = values.Split(',');
            foreach (string CustomerCode in CustomerCodes)
            {
                InString = InString + "'" + CustomerCode + "',";
            }
            InString = InString.Remove(InString.Length - 1);
            string GetSql = @"SELECT  DISTINCT ITEM_CODE, initcap(ITEM_EDESC) ITEM_EDESC,PARRENT_CODE 
                            FROM IP_ITEM_MASTER_SETUP  where GROUP_SKU_FLAG='I'
                            CONNECT BY PRIOR TRIM(ITEM_CODE)=TRIM(PARRENT_CODE)
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

        internal static void InsertSelectedProducts(string Ids)
        {

            try
            {
                int MaxId = 1;
                string TruncSql = "TRUNCATE TABLE SALES_ITEM_CODE";
                OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, TruncSql);
                string QueryToAppend;
                if (Ids == "" || Ids == null)
                {
                    QueryToAppend = "";
                }
                else
                {
                    QueryToAppend = "ITEM_CODE NOT IN (" + Ids + ") AND";
                }
                string NotSelectedQuery = "SELECT ITEM_CODE FROM IP_ITEM_MASTER_SETUP WHERE " + QueryToAppend + " GROUP_SKU_FLAG='I'";
                DataSet CustomerNotSelected = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, NotSelectedQuery);
                int count = CustomerNotSelected.Tables[0].Rows.Count;
                string[] customerid = new string[count];
                for (int i = 0; i < count; i++)
                {
                    customerid[i] = CustomerNotSelected.Tables[0].Rows[i]["ITEM_CODE"].ToString();
                }

                //string[] CustomerId = Ids.Split(',');
                foreach (string id in customerid)
                {
                    string InsertSql = "INSERT INTO SALES_ITEM_CODE VALUES ('" + id + "','" + MaxId + "')";
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, InsertSql);
                    MaxId++;
                }
            }
            catch
            {
                throw new NotImplementedException();
            }
        }



        internal static string GetDataForSalesReportAjax()
        {
            string sql = @"select * from SALES_REPORT_PREF_MASTER";
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




        internal static void RemoveSelectedRow(string rowKey)
        {
            string DeleteMaster = "DELETE FROM SALES_REPORT_PREF_MASTER WHERE PREF_CODE = '" + rowKey + "'";
            OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, DeleteMaster);
            OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, "COMMIT");
        }


        internal static string LastDayNepali()
        {
            string LastDayNepaliQuery = "SELECT LAST_DAY_NEPALI(SYSDATE) LAST_DAY FROM DUAL";

            DataSet ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, LastDayNepaliQuery);
            return ds.Tables[0].Rows[0]["LAST_DAY"].ToString();
        }

        internal static DataTable SalesReportTree(string f, string t, string type)
        {
            string FromDate = f;
            string ToDate = t;
            DateTime FDate = Convert.ToDateTime(FromDate);
            DateTime TDate = Convert.ToDateTime(ToDate);
            int count = (TDate.Year - FDate.Year) * 12 + TDate.Month - FDate.Month;
            string[] QueryToAppendForAmt = null;
            string[] QueryToAppendForQty = null;
            string TreeQuery = "";

            //avi
            switch (type)
            {
                case "CustomerTree":
                    if (FDate.Year == TDate.Year && FDate.Month == TDate.Month)
                    {
                        QueryToAppendForAmt = new string[1];
                        QueryToAppendForQty = new string[1];
                        QueryToAppendForAmt[0] = @",NVL((SELECT SUM(total_price)
                                          FROM sa_sales_invoice a
                                          WHERE a.customer_code=s.customer_code
                                          and a.sales_date between '" + FDate.ToString("dd-MMM-yyyy") + "' and '" +
                                           TDate.ToString("dd-MMM-yyyy");
                        QueryToAppendForQty[0] = @",NVL((SELECT SUM(quantity)
                                          FROM sa_sales_invoice a
                                          WHERE a.customer_code=s.customer_code
                                          and a.sales_date between '" + FDate.ToString("dd-MMM-yyyy") + "' and '" +
                                           TDate.ToString("dd-MMM-yyyy");
                    }
                    else
                    {
                        QueryToAppendForAmt = new string[count + 1];
                        QueryToAppendForQty = new string[count + 1];
                        QueryToAppendForAmt[0] = @",NVL((SELECT SUM(total_price)
                                          FROM sa_sales_invoice a
                                          WHERE a.customer_code=s.customer_code
                                          and a.sales_date between '" + FDate.ToString("dd-MMM-yyyy") + "' and '" + GetLastDay(FDate) + "'),0)" + FDate.ToString("MMMyyyy") + "Amt";
                        QueryToAppendForQty[0] = @",NVL((SELECT SUM(quantity)
                                          FROM sa_sales_invoice a
                                          WHERE a.customer_code=s.customer_code
                                          and a.sales_date between '" + FDate.ToString("dd-MMM-yyyy") + "' and '" + GetLastDay(FDate) + "'),0)" + FDate.ToString("MMMyyyy") + "Qty";
                        FDate = FDate.AddMonths(1);
                        FDate = new DateTime(FDate.Year, FDate.Month, 1);
                        for (int i = 1; i < count; i++)
                        {
                            QueryToAppendForAmt[i] = @",NVL((SELECT SUM(total_price)
                                          FROM sa_sales_invoice a
                                          WHERE a.customer_code=s.customer_code
                    and a.sales_date between first_day_english('" + FDate.ToString("dd-MMM-yyyy") + "') and last_day('" + FDate.ToString("dd-MMM-yyyy") + "')),0) " + FDate.ToString("MMMyyyy") + "Amt";
                            QueryToAppendForQty[i] = @",NVL((SELECT SUM(quantity)
                                          FROM sa_sales_invoice a
                                          WHERE a.customer_code=s.customer_code
                    and a.sales_date between first_day_english('" + FDate.ToString("dd-MMM-yyyy") + "') and last_day('" + FDate.ToString("dd-MMM-yyyy") + "')),0) " + FDate.ToString("MMMyyyy") + "Qty";

                            FDate = FDate.AddMonths(1);

                        }

                        QueryToAppendForAmt[count] = @",NVL((SELECT SUM(total_price)
                                          FROM sa_sales_invoice a
                                          WHERE a.customer_code=s.customer_code
               and a.sales_date between first_day_english('" + FDate.ToString("dd-MMM-yyyy") + "') and '" + TDate.ToString("dd-MMM-yyyy") + "'),0) " + FDate.ToString("MMMyyyy") + "Amt";
                        QueryToAppendForQty[count] = @",NVL((SELECT SUM(quantity)
                                          FROM sa_sales_invoice a
                                          WHERE a.customer_code=s.customer_code
               and a.sales_date between first_day_english('" + FDate.ToString("dd-MMM-yyyy") + "') and '" + TDate.ToString("dd-MMM-yyyy") + "'),0) " + FDate.ToString("MMMyyyy") + "Qty";
                    }
                    string Final = "";
                    for (int j = 0; j < count + 1; j++)
                    {
                        Final = Final + QueryToAppendForAmt[j] + QueryToAppendForQty[j];

                    }
                    TreeQuery = @"SELECT s.customer_code,
  LPAD(' ', 2*level-1)
  ||initcap(s.customer_edesc) customer_edesc,parrent_code,
  level,'FALSE' EXPANDED,'TRUE' LOADED,CONNECT_BY_ISLEAF ISLEAF " + Final + @",s.pre_customer_code,s.master_customer_code
FROM sa_customer_setup s
where s.customer_code in (select customer_code from sa_sales_invoice b where b.sales_date between to_date('" + FromDate + "','yyyy-mm-dd') and to_date('" + ToDate + "','yyyy-mm-dd')) OR " +
       @"s.customer_code in (select customer_code from sa_customer_setup where group_sku_flag='G')
  CONNECT BY PRIOR s.customer_code = s.parrent_code
  START WITH PRE_CUSTOMER_CODE='00'";
                    break;
                case "ProductTree": 
                    if (FDate.Year == TDate.Year && FDate.Month == TDate.Month)
                    {
                        QueryToAppendForAmt = new string[1];
                        QueryToAppendForQty = new string[1];
                        QueryToAppendForAmt[0] = @",NVL((SELECT SUM(total_price)
                                          FROM sa_sales_invoice a
                                          WHERE a.item_code=s.item_code
                                          and a.sales_date between '" + FDate.ToString("dd-MMM-yyyy") + "' and '" +
                                           TDate.ToString("dd-MMM-yyyy");
                        QueryToAppendForQty[0] = @",NVL((SELECT SUM(quantity)
                                          FROM sa_sales_invoice a
                                          WHERE a.item_code=s.item_code
                                          and a.sales_date between '" + FDate.ToString("dd-MMM-yyyy") + "' and '" +
                                           TDate.ToString("dd-MMM-yyyy");
                    }
                    else
                    {
                        QueryToAppendForAmt = new string[count + 1];
                        QueryToAppendForQty = new string[count + 1];
                        QueryToAppendForAmt[0] = @",NVL((SELECT SUM(total_price)
                                          FROM sa_sales_invoice a
                                          WHERE a.item_code=s.item_code
                                          and a.sales_date between '" + FDate.ToString("dd-MMM-yyyy") + "' and '" + GetLastDay(FDate) + "'),0)" + FDate.ToString("MMMyyyy") + "Amt";
                        QueryToAppendForQty[0] = @",NVL((SELECT SUM(quantity)
                                          FROM sa_sales_invoice a
                                          WHERE a.item_code=s.item_code
                                          and a.sales_date between '" + FDate.ToString("dd-MMM-yyyy") + "' and '" + GetLastDay(FDate) + "'),0)" + FDate.ToString("MMMyyyy") + "Qty";
                        FDate = FDate.AddMonths(1);
                        FDate = new DateTime(FDate.Year, FDate.Month, 1);
                        for (int i = 1; i < count; i++)
                        {
                            QueryToAppendForAmt[i] = @",NVL((SELECT SUM(total_price)
                                          FROM sa_sales_invoice a
                                          WHERE a.item_code=s.item_code
                    and a.sales_date between first_day_english('" + FDate.ToString("dd-MMM-yyyy") + "') and last_day('" + FDate.ToString("dd-MMM-yyyy") + "')),0) " + FDate.ToString("MMMyyyy") + "Amt";
                            QueryToAppendForQty[i] = @",NVL((SELECT SUM(quantity)
                                          FROM sa_sales_invoice a
                                          WHERE a.item_code=s.item_code
                    and a.sales_date between first_day_english('" + FDate.ToString("dd-MMM-yyyy") + "') and last_day('" + FDate.ToString("dd-MMM-yyyy") + "')),0) " + FDate.ToString("MMMyyyy") + "Qty";

                            FDate = FDate.AddMonths(1);

                        }

                        QueryToAppendForAmt[count] = @",NVL((SELECT SUM(total_price)
                                          FROM sa_sales_invoice a
                                          WHERE a.item_code=s.item_code
               and a.sales_date between first_day_english('" + FDate.ToString("dd-MMM-yyyy") + "') and '" + TDate.ToString("dd-MMM-yyyy") + "'),0) " + FDate.ToString("MMMyyyy") + "Amt";
                        QueryToAppendForQty[count] = @",NVL((SELECT SUM(quantity)
                                          FROM sa_sales_invoice a
                                          WHERE a.item_code=s.item_code
               and a.sales_date between first_day_english('" + FDate.ToString("dd-MMM-yyyy") + "') and '" + TDate.ToString("dd-MMM-yyyy") + "'),0) " + FDate.ToString("MMMyyyy") + "Qty";
                    }
                     Final = "";
                    for (int j = 0; j < count + 1; j++)
                    {
                        Final = Final + QueryToAppendForAmt[j] + QueryToAppendForQty[j];

                    }
                    TreeQuery = @"SELECT s.item_code customer_code,
  LPAD(' ', 2*level-1)
  ||initcap(s.item_edesc) customer_edesc,parrent_code,
  level,'FALSE' EXPANDED,'TRUE' LOADED,CONNECT_BY_ISLEAF ISLEAF " + Final + @",s.pre_item_code,s.master_item_code
FROM ip_item_master_setup s
where s.item_code in (select item_code from sa_sales_invoice b where b.sales_date between to_date('" + FromDate + "','yyyy-mm-dd') and to_date('" + ToDate + "','yyyy-mm-dd')) OR " +
       @"s.item_code in (select item_code from ip_item_master_setup where group_sku_flag='G')
  CONNECT BY PRIOR s.item_code = s.parrent_code
  START WITH PRE_item_CODE='00'";
                    break;


            }
            DataSet TreeData = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, TreeQuery);
            DataTable FinalTable = TreeData.Tables[0];
            return FinalTable;
        }

        public static string GetLastDay(DateTime Date)
        {
            int Calc = DateTime.DaysInMonth(Date.Year, Date.Month);
            string LastDay = Calc.ToString() + "-" + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(Date.Month) + "-" + Date.Year;
            return LastDay;

        }

    }


}
