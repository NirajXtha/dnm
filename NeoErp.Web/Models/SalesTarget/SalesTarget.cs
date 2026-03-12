using NeoErp.Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Text;

namespace NeoErp.Models.SalesTarget
{
    public class SalesTarget
    {
        public static string GenerateTargetSetup(string Tname, string TType, string BasedOn, string Frequency, string FromDate, string ToDate)
        {
            DateTime TDate = Convert.ToDateTime(ToDate);
            DateTime LastDate = TDate;
            //first nepali date
            string dateFrom = Convert.ToDateTime(FromDate).ToString("dd-MMM-yyyy");
            string toDayDate = "select to_bs('" + dateFrom + "') first_day from dual";
            DataSet dsF = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, toDayDate);
            //last nepali date
            string dateTo = Convert.ToDateTime(ToDate).ToString("dd-MMM-yyyy");
            string lastDayDate = "select to_bs('" + dateTo + "') to_date from dual";
            DataSet dsL = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, lastDayDate);

            //split to from  date for month
            string[] fromMonth = dsF.Tables[0].Rows[0]["first_day"].ToString().Split('/');
            int monthF = Convert.ToInt32(fromMonth[1]);
            //split to last date for month
            string[] toMonth = dsL.Tables[0].Rows[0]["to_date"].ToString().Split('/');
            int monthT = Convert.ToInt32(toMonth[1]);
            int yearT = Convert.ToInt32(toMonth[2]);
            string TargetTable = "";
            if (BasedOn == "Q") {
                if (Frequency == "W")
                {
                    TargetTable = "<table class='table'><tr><th>Periods</th><th>Quantity</th><tr>";
                }
                else
                {
                    TargetTable = "<table class='table'><tr><th>From</th><th>To</th><th>Quantity</th><tr>";
                }
            }
            else if (BasedOn == "V") {
                if (Frequency == "W")
                {
                    TargetTable = "<table class='table'><tr><th>Periods</th><th>Values</th><tr>";
                }
                else
                {
                    TargetTable = "<table class='table'><tr><th>From</th><th>To</th><th>Values</th><tr>";
                }
            }
            
            int i = 0;
            //print first date
            if (Frequency == "M")
            {
                string datef = Convert.ToDateTime(FromDate).ToString("dd-MMM-yyyy");
                string fromDatef = "select last_day_nepali('" + datef + "') last_dayf from dual";
                DataSet ds3 = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, fromDatef);
                TargetTable += "<tr><td>" + dsF.Tables[0].Rows[0]["first_day"] + "</td><td>" + ds3.Tables[0].Rows[0]["last_dayf"] + "</td><td><input type='text' class='values' name='value[" + i + "]' required></td></tr>";
                i++;
                //concatenate nepali date
                int month = monthF + 1;
                string nepaliDate;
                if (month == 10 || month == 11 || month == 12)
                {
                    nepaliDate = fromMonth[0] + "/" + month + "/" + fromMonth[2];
                }
                else {
                    nepaliDate = fromMonth[0] + "/"+0+ month + "/" + fromMonth[2];
                }
                // convert nepali to english date
                string engDate = "select to_as('" + nepaliDate + "') string_engDate from dual";
                DataSet dsAD = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, engDate);
                DateTime adDate = Convert.ToDateTime(dsAD.Tables[0].Rows[0]["string_engDate"]);
                //loop for from to To date value
                while (adDate < TDate)
                {
                    string datead = Convert.ToDateTime(adDate).ToString("dd-MMM-yyyy");
                    string dateadbs = "select to_bs('" + datead + "') to_dateadbs from dual";
                    DataSet bsad = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, dateadbs);
                    //split to from  date for month
                    string[] fromMonthadbs = bsad.Tables[0].Rows[0]["to_dateadbs"].ToString().Split('/');
                    int monthadbs = Convert.ToInt32(fromMonthadbs[1]);
                    if (monthadbs != monthT)
                    {
                        string date1 = Convert.ToDateTime(adDate).ToString("dd-MMM-yyyy");
                        string fromDate = "select last_day_nepali('" + date1 + "') last_day from dual";
                        DataSet ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, fromDate);

                        string fromDateNepali = "select to_bs('" + date1 + "') string_nepDate from dual";
                        DataSet dsNepali = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, fromDateNepali);
                        string[] firstday = dsNepali.Tables[0].Rows[0]["string_nepDate"].ToString().Split('/');

                        TargetTable += "<tr><td>01/" + firstday[1] + "/" + firstday[2] + "</td><td>" + ds.Tables[0].Rows[0]["last_day"] + "</td><td><input type='text' class='values' name='value[" + i + "]' required></td></tr>";
                        adDate = adDate.AddMonths(1);
                    }
                    else { break; }
                }
                if (monthF != monthT)
                {
                    TargetTable += "<tr><td>01/" + toMonth[1] + "/" + toMonth[2] + "</td><td>" + dsL.Tables[0].Rows[0]["to_date"] + "</td><td><input type='text' class='values' name='value[" + i + "]' required></td></tr>";
                }
                }
            DateTime FrmDate = Convert.ToDateTime(FromDate);
            string dateName;
            if (Frequency == "W")
            {
                
                string date1 = Convert.ToDateTime(FrmDate).ToString("dd-MMM-yyyy");
                string fromDateNepali = "select to_bs('" + date1 + "') string_nepDate from dual";
                DataSet dsNepali = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, fromDateNepali);

                TargetTable += "<tr><td>" + dsNepali.Tables[0].Rows[0]["string_nepDate"] + "</td><td><input type='text' class='values' name='value[" + i + "]' required></td></tr>";

                String dayName = FrmDate.DayOfWeek.ToString();
                switch (dayName)
                {
                    case "Sunday": dateName = Convert.ToDateTime(FrmDate).ToString("dd-MMM-yyyy"); FrmDate = FrmDate.AddDays(7);
                        break;
                    case "Monday": dateName = Convert.ToDateTime(FrmDate).ToString("dd-MMM-yyyy"); FrmDate = FrmDate.AddDays(6);
                        break;
                    case "Tuesday": dateName = Convert.ToDateTime(FrmDate).ToString("dd-MMM-yyyy"); FrmDate = FrmDate.AddDays(5);
                        break;
                    case "Wednesday": dateName = Convert.ToDateTime(FrmDate).ToString("dd-MMM-yyyy"); FrmDate = FrmDate.AddDays(4);
                        break;
                    case "Thursday": dateName = Convert.ToDateTime(FrmDate).ToString("dd-MMM-yyyy"); FrmDate = FrmDate.AddDays(3);
                        break;
                    case "Friday": dateName = Convert.ToDateTime(FrmDate).ToString("dd-MMM-yyyy"); FrmDate = FrmDate.AddDays(2);
                        break;
                    case "Saturday": dateName = Convert.ToDateTime(FrmDate).ToString("dd-MMM-yyyy"); FrmDate = FrmDate.AddDays(1);
                        break;
                }
                //FrmDate = FrmDate.AddDays(7);
                for (DateTime date = FrmDate; date.Date < TDate.Date; date = date.AddDays(7))
                {
                     date1 = Convert.ToDateTime(date).ToString("dd-MMM-yyyy");
                     fromDateNepali = "select to_bs('" + date1 + "') string_nepDate from dual";
                    DataSet dsNepaliS = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, fromDateNepali);

                    TargetTable += "<tr><td>" + dsNepaliS.Tables[0].Rows[0]["string_nepDate"] + "</td><td><input type='text' class='values' name='value[" + i + "]' required></td></tr>";
                    LastDate = date;
                    i++;
                }
                if (LastDate != TDate)
                {
                    i++;
                    TargetTable += "<tr><td>" + dsL.Tables[0].Rows[0]["to_date"] + "</td><td><input type='text' class='values' name='value[" + i + "]' required></td></tr>";
                }
            }
            if (Frequency == "Q")
            {
                string dateNepali = Convert.ToDateTime(FrmDate).ToString("dd-MMM-yyyy");
                string fromDateNepali = "select to_bs('" + dateNepali + "') string_nepDate from dual";
                DataSet dsNepali = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, fromDateNepali);
                string nepaliDate = dsNepali.Tables[0].Rows[0]["string_nepDate"].ToString();
                string qtrTable = "SELECT end_date FROM qtr_end_setup WHERE to_as('" + nepaliDate + "') BETWEEN to_as(qtr_end_setup.st_date) AND to_as(qtr_end_setup.end_date)";
                DataSet dsNepaliQ = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, qtrTable);

                TargetTable += "<tr><td>" + dsNepali.Tables[0].Rows[0]["string_nepDate"] + "</td><td>" + dsNepaliQ.Tables[0].Rows[0]["end_date"] + "</td><td><input type='text' class='values' name='value[" + i + "]' required></td></tr>";
                i++;
                     // convert nepali to english date
                string nepaliQDate = dsNepaliQ.Tables[0].Rows[0]["end_date"].ToString();
                string engDateQ = "select to_as('" + nepaliQDate + "') string_engDate from dual";
                DataSet dsAD = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, engDateQ);
                DateTime adDate = (Convert.ToDateTime(dsAD.Tables[0].Rows[0]["string_engDate"])).AddDays(1);
                while (adDate < TDate)
                {
                    string datead = Convert.ToDateTime(adDate).ToString("dd-MMM-yyyy");
                    string dateadbs = "select to_bs('" + datead + "') to_dateadbs from dual";
                    DataSet bsad = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, dateadbs);
                    //split to from  date for month
                     string[] fromMonthadbs = bsad.Tables[0].Rows[0]["to_dateadbs"].ToString().Split('/');
                    int monthadbs = Convert.ToInt32(fromMonthadbs[1]);
                    int yearadbs = Convert.ToInt32(fromMonthadbs[2]);
                    if (monthadbs > monthT || yearadbs>yearT)
                    {
                        string dateNepaliEndQ = Convert.ToDateTime(adDate).ToString("dd-MMM-yyyy");
                        string toDateNepaliQ = "select to_bs('" + dateNepaliEndQ + "') string_nepDateEndQ from dual";
                        DataSet dsNepaliEndQ = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, toDateNepaliQ);
                        string nepaliDateEndQ = dsNepaliEndQ.Tables[0].Rows[0]["string_nepDateEndQ"].ToString();
                        string qtrTableEndQ = "SELECT st_date,end_date FROM qtr_end_setup WHERE to_as('" + nepaliDateEndQ + "') BETWEEN to_as(qtr_end_setup.st_date) AND to_as(qtr_end_setup.end_date)";
                        DataSet dsNepaliQEndQ = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, qtrTableEndQ);
                        //string Qdate = "select st_date,end_date from qtr_end_setup";
                        //DataSet ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(),CommandType.Text,Qdate);
                        TargetTable += "<tr><td>" + dsNepaliQEndQ.Tables[0].Rows[0]["st_date"] + "</td><td>" + dsNepaliQEndQ.Tables[0].Rows[0]["end_date"] + "</td><td><input type='text' class='values' name='value[" + i + "]' required></td></tr>";
                        string npDate = dsNepaliQEndQ.Tables[0].Rows[0]["end_date"].ToString();
                        string engDateQ1 = "select to_as('" + npDate + "') string_engDate1 from dual";
                        DataSet dsAD1 = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, engDateQ1);
                        adDate = (Convert.ToDateTime(dsAD1.Tables[0].Rows[0]["string_engDate1"])).AddDays(1);
                        i++;
                    }
                    else { break; }

                    
                }
                 string npdate = dsNepaliQ.Tables[0].Rows[0]["end_date"].ToString();
                string engDatenp = "select to_as('" + npdate + "') string_engDatenp from dual";
                DataSet dsADQ = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, engDatenp);
                string adDateQ = dsADQ.Tables[0].Rows[0]["string_engDatenp"].ToString();
                DateTime adQ = Convert.ToDateTime(adDateQ);

                if (TDate > adQ)
                {

                    string dateNepaliEnd = Convert.ToDateTime(TDate).ToString("dd-MMM-yyyy");
                    string toDateNepali = "select to_bs('" + dateNepaliEnd + "') string_nepDateEnd from dual";
                    DataSet dsNepaliEnd = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, toDateNepali);
                    string nepaliDateEnd = dsNepaliEnd.Tables[0].Rows[0]["string_nepDateEnd"].ToString();
                    string qtrTableEnd = "SELECT st_date FROM qtr_end_setup WHERE to_as('" + nepaliDateEnd + "') BETWEEN to_as(qtr_end_setup.st_date) AND to_as(qtr_end_setup.end_date)";
                    DataSet dsNepaliQEnda = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, qtrTableEnd);
                    TargetTable += "<tr><td>" + dsNepaliQEnda.Tables[0].Rows[0]["st_date"] + "</td><td>" + dsL.Tables[0].Rows[0]["to_date"] + "</td><td><input type='text' class='values' name='value[" + i + "]' required></td></tr>";
                    i++;
                }
                
            }
           
            return TargetTable;
        }

    }
}