using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NeoErp.Models;
using NeoErp.Models.Report;
using System.Collections;
using System.Globalization;
using System.Data;
using NeoErp.Models.Common;
using Newtonsoft.Json;

namespace NeoErp.Controllers
{
    public class ReportController : Controller
    {
        private IMenuModel _menuService;
        public ReportController(IMenuModel menuService)
        {
            this._menuService = menuService;

        }
        public ActionResult CustomerDebtor()
        {
            string[] DefaultSetup = Debtor.GetDefaultSetupParam();
            DataTable dt = new DataTable();
            string PreferenceName = DefaultSetup[1];
            string figureAmount = DefaultSetup[2];
            string roundupAmount = DefaultSetup[3];
            string yaxis = DefaultSetup[4];
            string FDATE = DefaultSetup[5];
            int[] Steps = Debtor.GetDefaultDetailParam(DefaultSetup[0]);
            if (yaxis != null)
            {
                if (yaxis == "A")
                {
                    dt = Debtor.getAllCustomerData(Steps, FDATE);
                    ViewData.Model = dt.AsEnumerable();
                    ViewData["ViewType"] = "A";
                }
                if (yaxis == "T")
                {
                    dt = Debtor.getData(Steps, FDATE, figureAmount, roundupAmount);
                    ViewData.Model = dt.AsEnumerable();
                    ViewData["ViewType"] = "T";
                }
            }
            switch (figureAmount)
            {
                case "lakh": ViewData["figureAmount"] = 100000;
                    break;
                case "thousand": ViewData["figureAmount"] = 1000;
                    break;
                case "crore": ViewData["figureAmount"] = 10000000;
                    break;
                case "Actual": ViewData["figureAmount"] = 1;
                    break;
            }
            ViewData["FDATE"] = FDATE;
            ViewData["PrefernceName"] = PreferenceName;
            ViewData["roundupAmount"] = roundupAmount;
            ViewData.Model = dt.AsEnumerable();
            ViewData["ArrayNames"] = Steps;

            ViewData["ModuleMenu"] = _menuService.GetModule();
            return View();
        }


        [HttpPost]
        public ActionResult CustomerDebtor(int[] Steps, string figureAmount, string roundupAmount, string yaxis, string PreferenceName, string ViewPrefName, string SetDefault, string FromDate, string[] FilterCheckTree)
        {
            if (PreferenceName != null)
            {
                ViewData["PrefernceName"] = PreferenceName;
            }
            else
            {
                if (ViewPrefName != "") { ViewData["PrefernceName"] = ViewPrefName; } else { ViewData["PrefernceName"] = "Custom"; }
            }
            if (PreferenceName != null && yaxis[0] != null)
            {
                Debtor.InsertIntoSetup(figureAmount, roundupAmount, yaxis, PreferenceName, Steps, SetDefault, FromDate);

            }
            if (yaxis != null)
            {
                if (yaxis == "A")
                {
                    DataTable dt = new DataTable();
                    dt = Debtor.getAllCustomerData(Steps, FromDate);
                    ViewData.Model = dt.AsEnumerable();
                    ViewData["ViewType"] = "A";
                }
                if (yaxis == "T")
                {
                    DataTable dt = new DataTable();
                    dt = Debtor.getData(Steps, FromDate, figureAmount, roundupAmount);
                    ViewData.Model = dt.AsEnumerable();
                    ViewData["ViewType"] = "T";
                }
            }
            else
            {
                DataTable dt = new DataTable();
                dt = Debtor.getData(Steps, FromDate, figureAmount, roundupAmount);
                ViewData.Model = dt.AsEnumerable();
                ViewData["ViewType"] = "T";
            }
            int[] ArrayNames = Debtor.GetArray();
            ViewData["ArrayNames"] = ArrayNames;
            ViewData["ModuleMenu"] = _menuService.GetModule();
            switch (figureAmount)
            {
                case "lakh": ViewData["figureAmount"] = 100000;
                    break;
                case "thousand": ViewData["figureAmount"] = 1000;
                    break;
                case "crore": ViewData["figureAmount"] = 10000000;
                    break;
                case "Actual": ViewData["figureAmount"] = 1;
                    break;
            }
            ViewData["FDATE"] = FromDate;
            ViewData["roundupAmount"] = roundupAmount;
            return View();
        }


        public string CustomerDebtorAjax()
        {
            string AjaxDt = Debtor.GetDataForCustomerDebtorAjax();
            ViewData["AjaxDt"] = AjaxDt;
            return AjaxDt;
        }
        public string CustomerDebtorAjaxComplete(string rowKey)
        {
            string PrefernceSetup = Debtor.GetAllPreferencesVariable(rowKey);
            return PrefernceSetup;
        }
        public void CustomerDebtorAjaxRemoveRow(string rowKey)
        {
            Debtor.RemoveSelectedRow(rowKey);
        }
        public string GetPreferenceNameList()
        {
            string PreferenceList = Debtor.GetPreferenceList();
            return PreferenceList;
        }
        public string GetCustomerTransactions(string CustomerCode)
        {
            string CustomerTransactions = Debtor.GetAllTransaction(CustomerCode);
            return CustomerTransactions;
        }
        public string GetFilterTree()
        {
            string ReturnString = Debtor.GetFilterTree();
            return ReturnString;
        }
        public string getFilteredCustomerList(string values)
        {
            string ReturnSting = Debtor.GetFilteredCustomerList(values);

            return ReturnSting;
        }

        [HttpPost]
        public void InsertSelectedCustomerList(string id)
        {
            Debtor.InsertSelectedCustomers(id);


        }
        public string getTline()
        {

            string tlineData = Debtor.TotalAgingReportInAjax();
            return tlineData;
        }

        public JsonResult getColModals()
        {
            List<object> tlineData = Debtor.GetAllColModals();
            return Json(tlineData, JsonRequestBehavior.AllowGet);
        }

        public string GetCustomerItems(string SalesNo)
        {
            string ItemLists = Debtor.GetAllItems(SalesNo);
            return ItemLists;
        }
        public string getCustomerName(string customer)
        {
            string ItemLists = Debtor.customerName(customer);
            return ItemLists;
        }


        public string GetAllParamsLedger(string Todate)
        {
            string ReturnString = Debtor.GetFromDate(Todate);
            return ReturnString;
        }

        public ActionResult SalesReports()
        {

            ViewData["ModuleMenu"] = _menuService.GetModule();
            return View();
        }

        [HttpPost]
        public ActionResult SalesReports(string FromDate, string ToDate, string FromDateN, string ToDateN, string DateStep, string Amount, string Quantity, string figureQuantity, string figureAmount, string roundupQuantity, string roundupAmount, string[] xaxis, string yaxis, string multiple_value, string PreferenceName, string ViewPrefName, string DefaultPreference, string ProductList, string minAmt, string maxAmt)
        {
            multiple_value = multiple_value.Trim(',');
            string a = multiple_value.Trim(',');
            Session["FromDateSales"] = FromDate;
            Session["ToDateSales"] = ToDate;
            Session["DateStep"] = DateStep;
            Session["Amount"] = Amount;
            Session["Quantity"] = Quantity;
            //QuantityCheckboxForSales = Quantity;
            //AmountCheckboxForSales = Amount;
            Session["Xaxis"] = multiple_value;
            Session["roundupQuantity"] = roundupQuantity;
            Session["roundupAmount"] = roundupAmount;
            Session["Yaxis"] = yaxis;
            ViewData["FromDate"] = FromDate;
            ViewData["ToDate"] = ToDate;
            ViewData["FromDateN"] = FromDateN;
            ViewData["ToDateN"] = ToDateN;
            ViewData["DateStep"] = DateStep;
            ViewData["Amount"] = Amount;
            ViewData["Quantity"] = Quantity;
            switch (figureQuantity)
            {
                case "lakh": ViewData["figureQuantity"] = 100000;
                    Session["figureQuantity"] = 100000;
                    break;
                case "thousand": ViewData["figureQuantity"] = 1000;
                    Session["figureQuantity"] = 1000;
                    break;
                case "crore": ViewData["figureQuantity"] = 10000000;
                    Session["figureQuantity"] = 10000000;
                    break;
                case "Actual": ViewData["figureQuantity"] = 1;
                    Session["figureQuantity"] = 1;
                    break;

            }
            switch (figureAmount)
            {
                case "lakh": ViewData["figureAmount"] = 100000;
                    Session["figureAmount"] = 100000;
                    break;
                case "thousand": ViewData["figureAmount"] = 1000;
                    Session["figureAmount"] = 1000;
                    break;
                case "crore": ViewData["figureAmount"] = 10000000;
                    Session["figureAmount"] = 10000000;
                    break;
                case "Actual": ViewData["figureAmount"] = 1;
                    Session["figureAmount"] = 1;
                    break;

            }
            if (PreferenceName != null)
            {
                ViewData["PrefernceName"] = PreferenceName;
            }
            else
            {
                if (ViewPrefName != "") { ViewData["PrefernceName"] = ViewPrefName; } else { ViewData["PrefernceName"] = "Custom"; }
            }
            //if (PreferenceName != null)
            //{
            //    SalesReport.InsertIntoSetup(PreferenceName, Convert.ToDateTime(FromDate).ToString("dd-MMM-yyyy"), ToDate, DateStep, Amount, Quantity, figureAmount, figureQuantity, roundupAmount, roundupQuantity, minAmt, maxAmt, a, yaxis, "on");

            //}
            ViewData["Xaxis"] = multiple_value;
            ViewData["XaxisSelected"] = multiple_value.Split(',');
            ViewData["roundupQuantity"] = roundupQuantity;
            ViewData["roundupAmount"] = roundupAmount;
            ViewData["FromDate"] = Convert.ToDateTime(FromDate).ToString("yyyy-M-dd");
            ViewData["ToDate"] = Convert.ToDateTime(ToDate).ToString("yyyy-M-dd");
            ViewData["Yaxis"] = yaxis;
            //if (PreferenceName != "")
            //{
            //    SalesReport.PreferenceStore(PreferenceName, FromDate, ToDate, DateStep, Amount, Quantity, Convert.ToString(ViewData["figureAmount"]), Convert.ToString(ViewData["figureQuantity"]), roundupAmount, roundupQuantity, multiple_value, yaxis, DefaultPreference);
            //}
            DataTable dt = new DataTable();

            if (a == "CustomerTree" || a == "ProductTree" || a == "CustomerTree,Product" || a == "CustomerTree,Customer" || a == "ProductTree,Product" || a == "ProductTree,Customer")
            {
                //dt = SalesReport.pivotTable(FromDate, ToDate, figureAmount, figureQuantity, roundupAmount, roundupQuantity, a, minAmt, maxAmt, Quantity, Amount);
                //ViewData["avi_treeData"] =  TreeData();
                ViewData["Type"] = "T";
               // return View();
            }
            else
            {
                dt = SalesReport.getData(a, FromDate, ToDate, minAmt, maxAmt);
            }

            if (dt.Rows.Count > 0)
            {

                ViewData.Model = dt.AsEnumerable();
            }
            else
            {
                ViewData.Model = null;
            }
            ViewData["ModuleMenu"] = _menuService.GetModule();
            return View();

        }
        public ActionResult TrySalesReport()
        {
            return View();
        }

        public string getTlineS()
        {

            string tlineData = SalesReport.TotalAgingReportInAjax();
            return tlineData;
        }

        public JsonResult getColModalsS()
        {
            string AmountCheckbox, QuantityCheckbox;

            if (Session["Amount"] == null || Session["Amount"] == "")
            {
                AmountCheckbox = null;
            }
            else
            {
                AmountCheckbox = Session["Amount"].ToString();
            }
            if (Session["Quantity"] == null || Session["Quantity"] == "")
            {
                QuantityCheckbox = null;
            }
            else
            {
                QuantityCheckbox = Session["Quantity"].ToString();
            }
            List<object> tlineData = SalesReport.GetAllColModals(AmountCheckbox, QuantityCheckbox);
            return Json(tlineData, JsonRequestBehavior.AllowGet);
        }




        [HttpPost]
        public ActionResult Play(string FromDateV, string ToDateV)
        {
            Session["FromDateP"] = FromDateV;
            Session["ToDateP"] = ToDateV;
            string FromDate = Convert.ToString(Session["FromDate"]);
            string ToDate = Convert.ToString(Session["ToDate"]);

            ViewData["FromDate"] = Convert.ToDateTime(Session["FromDate"]).ToString("yyyy-M-dd");
            ViewData["ToDate"] = Convert.ToDateTime(Session["ToDate"]).ToString("yyyy-M-dd");
            ViewData["FromDateV"] = Convert.ToDateTime(FromDateV).ToString("yyyy-M-dd");
            ViewData["ToDateV"] = Convert.ToDateTime(ToDateV).ToString("yyyy-M-dd");
            ViewData["Amount"] = "on";
            ViewData["Quantity"] = "on";
            ViewData["Xaxis"] = Session["Xaxis"];
            ViewData["XaxisSelected"] = Session["Xaxis"];
            ViewData["roundupQuantity"] = Session["roundupQuantity"];
            ViewData["roundupAmount"] = Session["roundupAmount"];
            ViewData["Yaxis"] = Session["Yaxis"];
            ViewData["figureQuantity"] = Session["figureQuantity"];
            ViewData["figureAmount"] = Session["figureAmount"];





            DataTable dt = new DataTable();
            dt = SalesReport.getData("", FromDateV, ToDateV, "", "");
            if (dt.Rows.Count > 0)
            {
                ViewData.Model = dt.AsEnumerable();
            }
            else
            {
                ViewData.Model = null;
            }
            ViewData["ModuleMenu"] = _menuService.GetModule();
            return View("SalesReports");

        }


        public string Filter(string value)
        {
            string AjaxDt = "";
            if (value == "Category")
            {
                AjaxDt = SalesReport.filterProduct();
            }

            ViewData["AjaxDt"] = AjaxDt;
            return AjaxDt;
        }



        public ActionResult SalesReportsWeekly()
        {

            if (!string.IsNullOrEmpty(Session["FromDate"] as string) && !string.IsNullOrEmpty(Session["ToDate"] as string))
            {
                ViewData["check"] = "true";
                ViewData["DateStep"] = Session["DateStep"];
                string FromDate = Convert.ToDateTime(Session["FromDate"]).ToString("dd-MMM-yyyy");
                string ToDate = Convert.ToDateTime(Session["ToDate"]).ToString("dd-MMM-yyyy");
                ViewData["FromDate"] = Convert.ToDateTime(FromDate).ToString("yyyy-M-dd");
                ViewData["ToDate"] = Convert.ToDateTime(ToDate).ToString("yyyy-M-dd");
                ViewData["FromDateV"] = Convert.ToDateTime(FromDate).ToString("yyyy-M-dd");
                ViewData["ToDateV"] = Convert.ToDateTime(ToDate).ToString("yyyy-M-dd");
                ViewData["Amount"] = "on";
                ViewData["Quantity"] = "on";
                ViewData["Xaxis"] = Session["Xaxis"];
                ViewData["XaxisSelected"] = Session["Xaxis"];
                ViewData["roundupQuantity"] = Session["roundupQuantity"];
                ViewData["roundupAmount"] = Session["roundupAmount"];
                ViewData["Yaxis"] = Session["Yaxis"];
                ViewData["figureQuantity"] = Session["figureQuantity"];
                ViewData["figureAmount"] = Session["figureAmount"];
                DataTable dt = new DataTable();
                DataTable monthlyData = new DataTable();
                dt = SalesReport.getDataWeekly(FromDate, ToDate);
                ViewData.Model = dt.AsEnumerable();
            }
            else
            {
                ViewData["FromDate"] = "2015-01-15";
                ViewData["ToDate"] = "2015-07-15";
                ViewData["FromDateV"] = "2015-01-15";
                ViewData["ToDateV"] = "2015-07-15";
                ViewData["DateStep"] = "LH";
                ViewData["Amount"] = "on";
                ViewData["Quantity"] = "on";
                ViewData["figureQuantity"] = 1;
                ViewData["figureAmount"] = 1;
                ViewData["roundupQuantity"] = "0";
                ViewData["roundupAmount"] = "0.00";
                ViewData["Xaxis"] = "ProductTree";
                ViewData["Yaxis"] = "Timeline";
                ViewData["XaxisSelected"] = "ProductTree".Split(',');

                Session["FromDate"] = "2015-01-15";
                Session["ToDate"] = "2015-07-15";
                Session["DateStep"] = "LH";
                Session["Amount"] = "on";
                Session["Quantity"] = "on";
                Session["figureQuantity"] = 1;
                Session["figureAmount"] = 1;
                Session["roundupQuantity"] = "0";
                Session["roundupAmount"] = "0.00";
                Session["Xaxis"] = "ProductTree";

                Session["Yaxis"] = "Timeline";
                Session["XaxisSelected"] = "ProductTree".Split(',');
                DataTable dt = new DataTable();
                DataTable monthlyData = new DataTable();
                dt = SalesReport.getDataWeekly("15-Jan-2015", "16-Jul-2015");
                ViewData.Model = dt.AsEnumerable();
            }
            ViewData["ModuleMenu"] = _menuService.GetModule();
            return View();
        }

        [HttpPost]
        public ActionResult SalesReportsWeekly(string FromDate, string ToDate, string DateStep, string Amount, string Quantity, string figureQuantity, string figureAmount, string roundupQuantity, string roundupAmount, string[] xaxis, string yaxis, string multiple_value, string PreferenceName, string DefaultPreference, string ProductList)
        {
            multiple_value = multiple_value.Trim(',');
            string a = multiple_value.Trim(',');
            Session["FromDate"] = FromDate;
            Session["ToDate"] = ToDate;
            Session["DateStep"] = DateStep;
            Session["Amount"] = Amount;
            Session["Quantity"] = Quantity;
            Session["Xaxis"] = multiple_value;
            Session["roundupQuantity"] = roundupQuantity;
            Session["roundupAmount"] = roundupAmount;
            Session["Yaxis"] = yaxis;
            ViewData["FromDate"] = Convert.ToDateTime(FromDate).ToString("yyyy-M-dd");
            ViewData["ToDate"] = Convert.ToDateTime(ToDate).ToString("yyyy-M-dd");
            ViewData["FromDateV"] = Convert.ToDateTime(FromDate).ToString("yyyy-M-dd");
            ViewData["ToDateV"] = Convert.ToDateTime(ToDate).ToString("yyyy-M-dd");
            ViewData["DateStep"] = DateStep;
            ViewData["Amount"] = Amount;
            ViewData["Quantity"] = Quantity;
            switch (figureQuantity)
            {
                case "lakh": ViewData["figureQuantity"] = 100000;
                    Session["figureQuantity"] = 100000;
                    break;
                case "thousand": ViewData["figureQuantity"] = 1000;
                    Session["figureQuantity"] = 1000;
                    break;
                case "crore": ViewData["figureQuantity"] = 10000000;
                    Session["figureQuantity"] = 10000000;
                    break;
                case "Actual": ViewData["figureQuantity"] = 1;
                    Session["figureQuantity"] = 1;
                    break;

            }
            switch (figureAmount)
            {
                case "lakh": ViewData["figureAmount"] = 100000;
                    Session["figureAmount"] = 100000;
                    break;
                case "thousand": ViewData["figureAmount"] = 1000;
                    Session["figureAmount"] = 1000;
                    break;
                case "crore": ViewData["figureAmount"] = 10000000;
                    Session["figureAmount"] = 10000000;
                    break;
                case "Actual": ViewData["figureAmount"] = 1;
                    Session["figureAmount"] = 1;
                    break;

            }
            //int count = xaxis.Length;
            //string xvalue = "";
            //for (int i = 0; i < count; i++)
            //{
            //    string loopvalue = xaxis[i];
            //    xvalue = xvalue + loopvalue;
            //}
            ViewData["Xaxis"] = multiple_value;
            ViewData["XaxisSelected"] = multiple_value.Split(',');
            ViewData["roundupQuantity"] = roundupQuantity;
            ViewData["roundupAmount"] = roundupAmount;
            ViewData["FromDate"] = Convert.ToDateTime(FromDate).ToString("yyyy-M-dd");
            ViewData["ToDate"] = Convert.ToDateTime(ToDate).ToString("yyyy-M-dd");
            ViewData["Yaxis"] = yaxis;
            //if (PreferenceName != "")
            //{
            //    SalesReport.PreferenceStore(PreferenceName, FromDate, ToDate, DateStep, Amount, Quantity, Convert.ToString(ViewData["figureAmount"]), Convert.ToString(ViewData["figureQuantity"]), roundupAmount, roundupQuantity, multiple_value, yaxis, DefaultPreference);
            //}
            DataTable dt = new DataTable();
            if (ProductList != null)
            {
                dt = SalesReport.getDataFiltered(FromDate, ToDate, ProductList);
            }
            else
            {
                dt = SalesReport.getDataWeekly(FromDate, ToDate);
            }
            if (dt.Rows.Count > 0)
            {
                ViewData.Model = dt.AsEnumerable();
            }
            else
            {
                ViewData.Model = null;
            }
            ViewData["ModuleMenu"] = _menuService.GetModule();
            return View();
        }

        public ActionResult SalesReportsQuarterly()
        {
            if (!string.IsNullOrEmpty(Session["FromDate"] as string) && !string.IsNullOrEmpty(Session["ToDate"] as string))
            {
                ViewData["check"] = "true";
                ViewData["DateStep"] = Session["DateStep"];
                string FromDate = Convert.ToDateTime(Session["FromDate"]).ToString("dd-MMM-yyyy");
                string ToDate = Convert.ToDateTime(Session["ToDate"]).ToString("dd-MMM-yyyy");
                ViewData["FromDate"] = Convert.ToDateTime(FromDate).ToString("yyyy-M-dd");
                ViewData["ToDate"] = Convert.ToDateTime(ToDate).ToString("yyyy-M-dd");
                ViewData["FromDateV"] = Convert.ToDateTime(FromDate).ToString("yyyy-M-dd");
                ViewData["ToDateV"] = Convert.ToDateTime(ToDate).ToString("yyyy-M-dd");
                ViewData["Amount"] = "on";
                ViewData["Quantity"] = "on";
                ViewData["Xaxis"] = Session["Xaxis"];
                ViewData["XaxisSelected"] = Session["Xaxis"];
                ViewData["roundupQuantity"] = Session["roundupQuantity"];
                ViewData["roundupAmount"] = Session["roundupAmount"];
                ViewData["Yaxis"] = Session["Yaxis"];
                ViewData["figureQuantity"] = Session["figureQuantity"];
                ViewData["figureAmount"] = Session["figureAmount"];
                DataTable dt = new DataTable();
                DataTable monthlyData = new DataTable();
                dt = SalesReport.getDataQuarterly(FromDate, ToDate);
                ViewData.Model = dt.AsEnumerable();
            }
            else
            {
                ViewData["FromDate"] = "2015-01-15";
                ViewData["ToDate"] = "2015-07-15";
                ViewData["FromDateV"] = "2015-01-15";
                ViewData["ToDateV"] = "2015-07-15";
                ViewData["DateStep"] = "LH";
                ViewData["Amount"] = "on";
                ViewData["Quantity"] = "on";
                ViewData["figureQuantity"] = 1;
                ViewData["figureAmount"] = 1;
                ViewData["roundupQuantity"] = "0";
                ViewData["roundupAmount"] = "0.00";
                ViewData["Xaxis"] = "ProductTree";
                ViewData["Yaxis"] = "Timeline";
                ViewData["XaxisSelected"] = "ProductTree".Split(',');

                Session["FromDate"] = "2015-01-15";
                Session["ToDate"] = "2015-07-15";
                Session["DateStep"] = "LH";
                Session["Amount"] = "on";
                Session["Quantity"] = "on";
                Session["figureQuantity"] = 1;
                Session["figureAmount"] = 1;
                Session["roundupQuantity"] = "0";
                Session["roundupAmount"] = "0.00";
                Session["Xaxis"] = "ProductTree";

                Session["Yaxis"] = "Timeline";
                Session["XaxisSelected"] = "ProductTree".Split(',');
                DataTable dt = new DataTable();
                DataTable monthlyData = new DataTable();
                dt = SalesReport.getDataQuarterly("15-Jan-2015", "16-Jul-2015");
                ViewData.Model = dt.AsEnumerable();
            }
            ViewData["ModuleMenu"] = _menuService.GetModule();
            return View();
        }

        [HttpPost]
        public ActionResult SalesReportsQuarterly(string FromDate, string ToDate, string DateStep, string Amount, string Quantity, string figureQuantity, string figureAmount, string roundupQuantity, string roundupAmount, string[] xaxis, string yaxis, string multiple_value, string PreferenceName, string DefaultPreference, string ProductList)
        {
            multiple_value = multiple_value.Trim(',');
            string a = multiple_value.Trim(',');
            Session["FromDate"] = FromDate;
            Session["ToDate"] = ToDate;
            Session["DateStep"] = DateStep;
            Session["Amount"] = Amount;
            Session["Quantity"] = Quantity;
            Session["Xaxis"] = multiple_value;
            Session["roundupQuantity"] = roundupQuantity;
            Session["roundupAmount"] = roundupAmount;
            Session["Yaxis"] = yaxis;
            ViewData["FromDate"] = Convert.ToDateTime(FromDate).ToString("yyyy-M-dd");
            ViewData["ToDate"] = Convert.ToDateTime(ToDate).ToString("yyyy-M-dd");
            ViewData["FromDateV"] = Convert.ToDateTime(FromDate).ToString("yyyy-M-dd");
            ViewData["ToDateV"] = Convert.ToDateTime(ToDate).ToString("yyyy-M-dd");
            ViewData["DateStep"] = DateStep;
            ViewData["Amount"] = Amount;
            ViewData["Quantity"] = Quantity;
            switch (figureQuantity)
            {
                case "lakh": ViewData["figureQuantity"] = 100000;
                    Session["figureQuantity"] = 100000;
                    break;
                case "thousand": ViewData["figureQuantity"] = 1000;
                    Session["figureQuantity"] = 1000;
                    break;
                case "crore": ViewData["figureQuantity"] = 10000000;
                    Session["figureQuantity"] = 10000000;
                    break;
                case "Actual": ViewData["figureQuantity"] = 1;
                    Session["figureQuantity"] = 1;
                    break;

            }
            switch (figureAmount)
            {
                case "lakh": ViewData["figureAmount"] = 100000;
                    Session["figureAmount"] = 100000;
                    break;
                case "thousand": ViewData["figureAmount"] = 1000;
                    Session["figureAmount"] = 1000;
                    break;
                case "crore": ViewData["figureAmount"] = 10000000;
                    Session["figureAmount"] = 10000000;
                    break;
                case "Actual": ViewData["figureAmount"] = 1;
                    Session["figureAmount"] = 1;
                    break;

            }
            //int count = xaxis.Length;
            //string xvalue = "";
            //for (int i = 0; i < count; i++)
            //{
            //    string loopvalue = xaxis[i];
            //    xvalue = xvalue + loopvalue;
            //}
            ViewData["Xaxis"] = multiple_value;
            ViewData["XaxisSelected"] = multiple_value.Split(',');
            ViewData["roundupQuantity"] = roundupQuantity;
            ViewData["roundupAmount"] = roundupAmount;
            ViewData["FromDate"] = Convert.ToDateTime(FromDate).ToString("yyyy-M-dd");
            ViewData["ToDate"] = Convert.ToDateTime(ToDate).ToString("yyyy-M-dd");
            ViewData["Yaxis"] = yaxis;
            //if (PreferenceName != "")
            //{
            //    SalesReport.PreferenceStore(PreferenceName, FromDate, ToDate, DateStep, Amount, Quantity, Convert.ToString(ViewData["figureAmount"]), Convert.ToString(ViewData["figureQuantity"]), roundupAmount, roundupQuantity, multiple_value, yaxis, DefaultPreference);
            //}
            DataTable dt = new DataTable();
            if (ProductList != null)
            {
                dt = SalesReport.getDataFiltered(FromDate, ToDate, ProductList);
            }
            else
            {
                dt = SalesReport.getDataQuarterly(FromDate, ToDate);
            }
            if (dt.Rows.Count > 0)
            {
                ViewData.Model = dt.AsEnumerable();
            }
            else
            {
                ViewData.Model = null;
            }
            ViewData["ModuleMenu"] = _menuService.GetModule();
            return View();
        }


        public ActionResult SalesReportsNepali()
        {
            if (!string.IsNullOrEmpty(Session["FromDate"] as string) && !string.IsNullOrEmpty(Session["ToDate"] as string))
            {
                ViewData["check"] = "true";
                ViewData["DateStep"] = Session["DateStep"];
                string FromDate = Session["FromDate"].ToString();
                string ToDate = Session["ToDate"].ToString();
                ViewData["FromDate"] = Convert.ToDateTime(FromDate).ToString("yyyy-M-dd");
                ViewData["ToDate"] = Convert.ToDateTime(ToDate).ToString("yyyy-M-dd");
                DataTable monthlyData = new DataTable();
                monthlyData = SalesReport.getDataMonthlyNepali(FromDate, ToDate);
                ViewData.Model = monthlyData.AsEnumerable();
            }
            ViewData["ModuleMenu"] = _menuService.GetModule();
            return View();
        }

        [HttpPost]
        public ActionResult SalesReportsNepali(string FromDate, string ToDate, string DateStep)
        {
            Session["FromDate"] = FromDate;
            Session["ToDate"] = ToDate;
            Session["DateStep"] = DateStep;
            ViewData["DateStep"] = DateStep;
            ViewData["FromDate"] = Convert.ToDateTime(FromDate).ToString("yyyy-M-dd");
            ViewData["ToDate"] = Convert.ToDateTime(ToDate).ToString("yyyy-M-dd");
            DataTable monthlyData = new DataTable();
            monthlyData = SalesReport.getDataMonthlyNepali(FromDate, ToDate);
            ViewData.Model = monthlyData.AsEnumerable();
            ViewData["ModuleMenu"] = _menuService.GetModule();
            return View();
        }

        public ActionResult Tree()
        {
            DataTable dt = new DataTable();
            //dt = SalesReport.getTree();
            ViewData.Model = dt.AsEnumerable();
            return View();
        }

        //public void check()
        //    {
        //        DataTable dt = SalesReport.pivotTable();
        //    }

        public string sales_report()
        {

            string dt = SalesReport.getTree();
            return dt;
        }


        [HttpPost]
        public void InsertSelectedCustomerListSales(string id)
        {
            SalesReport.InsertSelectedCustomers(id);


        }

        [HttpPost]
        public void InsertSelectedProductListSales(string id)
        {
            SalesReport.InsertSelectedProducts(id);


        }

        public string ProductFilter()
        {
            string ReturnString = SalesReport.getProductFilter();
            return ReturnString;
        }

        public string FilteredItemList(string values)
        {
            string ReturnString = SalesReport.getFilteredItemList(values);
            return ReturnString;
        }

        public string SalesReportAjax()
        {
            string AjaxDt = SalesReport.GetDataForSalesReportAjax();
            ViewData["AjaxDt"] = AjaxDt;
            return AjaxDt;
        }

        public void SalesReportAjaxRemoveRow(string rowKey)
        {
            SalesReport.RemoveSelectedRow(rowKey);
        }

        public string LastDayNepali()
        {

            string Data = SalesReport.LastDayNepali();
            return Data;
        }

        public ActionResult Test()
        {
            ViewData["ModuleMenu"] = _menuService.GetModule();
            return View();
        }

        public string TreeData(string FromDate, string ToDate,string type)
        {
            //string type = "CustomerTree";
            DataTable table = SalesReport.SalesReportTree(FromDate, ToDate,type);
            string JSONString = JsonConvert.SerializeObject(table);
            return JSONString;
        }
        public object getColmodel(string FromDate, string ToDate, string type)
        {
            //string type = "CustomerTree";
            DataTable table = SalesReport.SalesReportTree(FromDate, ToDate, type);
            string[] columnNames = table.Columns.Cast<DataColumn>()
                                 .Select(x => x.ColumnName)
                                 .ToArray();
            string colmodel = "";
            //TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            for (int i = 7; i < columnNames.Length-2;i++ )
           // { colmodel = colmodel + "{name:\"" + columnNames[i] + "\",index:\"" + columnNames[i] + "\",label:\"" + columnNames[i]+"\"},"; }
            { colmodel = colmodel + "{name:\"" + columnNames[i] + "\",index:\"" + columnNames[i] + "\",align:\"right\",label:\"" + Init_cap(columnNames[i].Substring(7, 3)) + "\"},"; }
            colmodel = colmodel.Remove(colmodel.Length - 1);
            colmodel = "[" + colmodel + "]";
            object something = JsonConvert.DeserializeObject(colmodel);
            return something;
        }

        public string Init_cap(string a)
        {
            int l = a.Length;
            string i = a.Substring(0, 1).ToUpper();
            string j = a.Substring(1, l - 1).ToLower();
            string final = i + j+".";
            return final;
        }



        public ActionResult GetDateField()
        {
            return PartialView("~/Views/Shared/Controls/DateField.cshtml", true);
        }
    }

}
