using NeoErp.Planning.Service.Interface;
using NeoErp.Planning.Service.Models;
using System;
using Excel = Microsoft.Office.Interop.Excel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using LinqToExcel;

namespace NeoERP.Planning.Controllers
{
    public class PlanController : Controller
    {
        public IPlanSetup _iPlanSetup { get; set; }
        IPlan _plan;
        public PlanController(IPlanSetup iPlanSetup, IPlan plan)
        {
            this._iPlanSetup = iPlanSetup;
            this._plan = plan; 
        }

        //public PlanController() : base()
        //{
        //}

        // GET: Planning
        public ActionResult Index(string plancode)
        {

            ViewBag.PlanCode = plancode;
            ViewBag.ProductSelectionLimit = System.Configuration.ConfigurationManager.AppSettings.Get("ProductSelectionLimit");
            string employeCode = this._plan.getUsersEmployeeCode();
            ViewBag.EmployeeCode = employeCode;
            var model = this._iPlanSetup.GetPreferenceSetup("SALES_PLAN");
            //System.Configuration.ConfigurationFileMap fileMap = new System.Configuration.ConfigurationFileMap("~/Areas/NeoERP.Planning/Web.config"); //Path to your config file
            //System.Configuration.Configuration configuration = System.Configuration.ConfigurationManager.OpenMappedMachineConfiguration(fileMap);
            //string applicationType = configuration.AppSettings.Settings["ProductSelectionLimit"].Value;
            return PartialView(model);
        }

        public ActionResult EditPlan(string plancode)
        {
            ViewBag.PlanCode = plancode;
            return PartialView();
        }

        //public ActionResult PlanSetup()
        //{
        //    var FrequencyTitle = _iPlanSetup.GetFrequencyTitle();
        //    return PartialView(FrequencyTitle);
        //}

        //public ActionResult PlanSetup(string planCode)
        //{
        //    //var FrequencyTitle = _iPlanSetup.GetFrequencyTitle(planCode);
        //    return View();
        //}

        //public ActionResult LoadPlanSetupTreeListPartial(string planCode, string itemCode)
        //{
        //    // planCode = "85";
        //    var FrequencyTitle = _iPlanSetup.GetFrequencyTitle(planCode);
        //    //return PartialView("../Views/Shared/_planSetupTreeList", FrequencyTitle);
        //    return PartialView("~/Areas/NeoERP.Planning/Views/Shared/_planSetupTreeList.cshtml", FrequencyTitle);
        //}

        public ActionResult LoadPlanSetupTreeListPartial(string startDate, string endDate, string timeFrameCode, string timeFrameName, string datetype)
        {
            //var FrequencyTitle = _iPlanSetup.GetFrequencyTitle(startDate,endDate,timeFrameCode,timeFrameName,datetype);
            //return PartialView("~/Areas/NeoERP.Planning/Views/Shared/_planSetupTreeList.cshtml", FrequencyTitle);
            return PartialView("~/Areas/NeoERP.Planning/Views/Shared/_planSetupTreeList.cshtml");
        }
        public ActionResult LoadSalesPlanSetupTreeListPartial(string startDate, string endDate, string timeFrameCode, string timeFrameName, string datetype)
        {
            //var FrequencyTitle = _iPlanSetup.GetFrequencyTitle(startDate,endDate,timeFrameCode,timeFrameName,datetype);
            //return PartialView("~/Areas/NeoERP.Planning/Views/Shared/_planSetupTreeList.cshtml", FrequencyTitle);
            return PartialView("~/Areas/NeoERP.Planning/Views/Home/_salesPlanReportTreeList.cshtml");
        }
        public ActionResult LoadMasterPlanSetupTreeListPartial(string startDate, string endDate, string timeFrameCode, string timeFrameName, string datetype)
        {
            //var FrequencyTitle = _iPlanSetup.GetFrequencyTitle(startDate,endDate,timeFrameCode,timeFrameName,datetype);
            //return PartialView("~/Areas/NeoERP.Planning/Views/Shared/_planSetupTreeList.cshtml", FrequencyTitle);
            return PartialView("~/Areas/NeoERP.Planning/Views/Home/_masterPlanTreeList.cshtml");
        }
        public JsonResult getFrequencyTitle(string startDate, string endDate, string timeFrameCode, string timeFrameName, string datetype)
        {
            List<MyColumnSettings> list = new List<MyColumnSettings>();
            list = _iPlanSetup.GetFrequencyTitle(startDate, endDate, timeFrameCode, timeFrameName, datetype);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadDynamicCol(string planCode = "85")
        {
            var FrequencyTitle = _iPlanSetup.GetFrequencyTitle(planCode);
            return PartialView(FrequencyTitle);
        }

        public ActionResult SubPlan(string planCode)
        {
            ViewBag.planCode = planCode;
            return PartialView();
        }

        public ActionResult Error()
        {
            return PartialView("Error");
        }

        //public string savePlan(FormCollection fc, string planCode, string freqCode)
        public string savePlan(FormCollection fc)
        {
            string planCode = "", freqCode = "";
            var result = string.Empty;
            var parentArr = new List<savePlan>();
            var childArr = new List<savePlan>();
            var otherArr = new List<savePlan>();
            var plan_Code = planCode;
            var time_frame_code = freqCode;

            //if (string.IsNullOrEmpty(planCode) || string.IsNullOrEmpty(freqCode))
            //    return "";

            string totalValue = string.Empty;
            var list = new List<string>();
            try
            {
                foreach (var key in fc.AllKeys)
                {
                    if (key.Contains("autoFillNum"))
                    {
                        if (fc[key] == null || fc[key] == "")
                            fc[key] = "0";
                        var value = 0M;
                        decimal.TryParse(fc[key], out value);
                        if (value == 0)
                            continue;
                        var itemCode = getItemCode(key);
                        var obj = new savePlan
                        {
                            name = key,
                            value = fc[key],
                            itemCode = itemCode,
                        };
                        parentArr.Add(obj);
                    }
                    else if (key.Contains("freqItemNum"))
                    {
                        if (fc[key] == null || fc[key] == "")
                            fc[key] = "0";
                        if (fc[key].Contains('"'))
                        {
                            fc[key] = fc[key].ToString().Trim('"');
                        }
                        var value = 0M;
                        decimal.TryParse(fc[key], out value);
                        if (value == 0)
                        {
                            var monthInt = getMonth(key);
                            list.Add(monthInt);
                            continue;
                        }
                        var itemCode = getItemCode(key);
                        var obj = new savePlan
                        {
                            name = key,
                            value = fc[key],
                            itemCode = itemCode,
                            skipMonth = list

                        };
                        childArr.Add(obj);
                        //totalValue = totalValue + value;
                    }
                    else if (key.Contains("qtyWiseAmtCalc"))
                    {
                        if (fc[key] == null || fc[key] == "")
                            fc[key] = "0";
                        if (fc[key].Contains('"'))
                        {
                            fc[key] = fc[key].ToString().Trim('"');
                        }
                        var value = 0M;
                        decimal.TryParse(fc[key], out value);
                        if (value == 0)
                            continue;
                        var itemCode = getItemCode(key);
                        var obj = new savePlan
                        {
                            name = key,
                            value_amt = fc[key],
                            itemCode = itemCode,
                        };
                        otherArr.Add(obj);
                    }
                }
                //totalValue=totalValue+parentArr[0].value
                var calendar_type = (fc["dateFormat"] == "BS") ? "LOC" : "ENG";
                if (parentArr.Count == 0) return "No Item";
                totalValue = parentArr[0].value;
                SalesPlan sp = new SalesPlan()
                {
                    customerCode = fc["customerCode"],
                    partytypeCode = fc["partytypeCode"],
                    agentCode = fc["agentCode"],
                    branchCode = fc["branchCode"],
                    employeeCode = fc["employeeCode"],
                    divisionCode = fc["divisionCode"],
                    dateFormat = fc["dateFormat"],
                    END_DATE = fc["END_DATE"],
                    salesRateType = fc["salesRateType"],
                    IsCustomerProduct = fc["IsCustomerProduct"],
                    PLAN_EDESC = fc["PLAN_EDESC"],
                    PLAN_FOR = fc["PLAN_FOR"],
                    PLAN_TYPE = fc["PLAN_TYPE"],
                    PLAN_CODE = fc["PLAN_CODE"],
                    REMARKS = fc["REMARKS"],
                    START_DATE = fc["START_DATE"],
                    TIME_FRAME_CODE = fc["TIME_FRAME_CODE"],
                    TIME_FRAME_EDESC = fc["TIME_FRAME_EDESC"],
                    CALENDAR_TYPE = calendar_type,
                    SALES_AMOUNT = totalValue.ToString(),
                    SALES_QUANTITY = totalValue.ToString()
                };
                foreach (var arr in parentArr)
                {
                    var t = childArr.Where(x => x.itemCode == arr.itemCode).ToList();
                    var freqValueSum = 0.0;
                    foreach (var i in t)
                    {

                        string freqname = string.Empty;
                        string freqCount = string.Empty;
                        getFreqName(i.name, calendar_type, out freqname, out freqCount);
                        var fre = new freqNameVlaue();
                        foreach (var oi in otherArr)
                        {
                            if (oi.itemCode == i.itemCode)
                            {
                                fre.fvalue_amt = oi.value_amt;
                            }
                        }
                        fre.skipMonth = i.skipMonth;
                        fre.fname = freqCount + "_" + i.name.Split('_')[3];
                        fre.fvalue = i.value;
                        freqValueSum += Convert.ToDouble(i.value);
                        arr.frequency.Add(fre);
                    }

                    decimal decimalval = Convert.ToDecimal(arr.value);
                    int arrayValInt = Convert.ToInt32(decimalval);
                    //Int32.TryParse(decimalval, out arrayValInt);

                    if (Convert.ToInt32(freqValueSum) != arrayValInt)
                        continue;
                    //return "error";

                }
                if (parentArr.Count > 0)
                    //return result = _iPlanSetup.SavePlan(parentArr, plan_Code, time_frame_code);
                    return result = _iPlanSetup.SavePlan(parentArr, sp);
                else
                    return "error";

            }
            catch(Exception ex)
            {
                throw;// new Exception( ex.Message,ex);
            }
            
        }

        public ActionResult CopyPlan(int planCode)
        {
            var model = this._iPlanSetup.GetPreferenceSetup("SALES_PLAN");
            SalesPlan result = this._iPlanSetup.GetPlanDetailValueByPlanCode(planCode);
            ViewBag.planCode = planCode;
            return PartialView(model);
        }
        [HttpPost]
        public JsonResult CopyPlan(FormCollection formData)
        {
            string planCode = string.Empty;
            string planName = string.Empty;
            string customers = string.Empty;
            string employees = string.Empty;
            string branchs = string.Empty;
            string divisions = string.Empty;
            string remarks = string.Empty;
            string partyType = string.Empty;
            foreach (var item in formData)
            {
                if (item.ToString().Contains("planName_copy"))
                {
                    planName = formData[item.ToString()];
                }
                else if (item.ToString().Contains("partytype_copy"))
                {
                    partyType = formData[item.ToString()];
                }
                else if (item.ToString().Contains("plancode_copy"))
                {
                    planCode = formData[item.ToString()];
                }
                else if (item.ToString().Contains("customers_copy"))
                {
                    customers = formData[item.ToString()];
                }
                else if (item.ToString().Contains("employees_copy"))
                {
                    employees = formData[item.ToString()];
                }
                else if (item.ToString().Contains("branchs_copy"))
                {
                    branchs = formData[item.ToString()];
                }
                else if (item.ToString().Contains("divisions_copy"))
                {
                    divisions = formData[item.ToString()];
                }
                else if (item.ToString().Contains("remarks"))
                {
                    remarks = formData[item.ToString()];
                }
            }
            bool copied = true;
            if (!string.IsNullOrEmpty(planCode))
            {
                copied = this._plan.cloneSalesPlan(planCode, planName, customers, employees, branchs, divisions, partyType, remarks);
            }
            else
            {
                copied = false;
            }

            if (copied)
            {
                return Json("success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }

        }
        public static void getFreqName(string name, string calander_type, out string freqname, out string freqCount)
        {
            freqCount = "0";
            string[] arr = new string[3];
            arr = name.Split('_');
            freqname = arr[2].ToString();
            if (freqname == "WEEK")
            {
                var count = arr.LastOrDefault();
                if (count != null)
                {
                    freqCount = count;
                }
            }
            else
            {
                var freq = freqname.Split('-');
                switch (freq[0].ToUpper())
                {
                    case "JAN":
                        freqCount = "1";
                        break;
                    case "FEB":
                        freqCount = "2";
                        break;
                    case "MAR":
                        freqCount = "3";
                        break;
                    case "APR":
                        freqCount = "4";
                        break;
                    case "MAY":
                        freqCount = "5";
                        break;
                    case "JUN":
                        freqCount = "6";
                        break;
                    case "JUL":
                        freqCount = "7";
                        break;
                    case "AUG":
                        freqCount = "8";
                        break;
                    case "SEP":
                        freqCount = "9";
                        break;
                    case "OCT":
                        freqCount = "10";
                        break;
                    case "NOV":
                        freqCount = "11";
                        break;
                    case "DEC":
                        freqCount = "12";
                        break;
                    case "BAISAKH":
                        freqCount = "01";
                        break;
                    case "JESTHA":
                        freqCount = "02";
                        break;
                    case "ASHADH":
                        freqCount = "03";
                        break;
                    case "SHRAWAN":
                        freqCount = "04";
                        break;
                    case "BHADRA":
                        freqCount = "05";
                        break;
                    case "ASHOJ":
                        freqCount = "06";
                        break;
                    case "KARTIK":
                        freqCount = "07";
                        break;
                    case "MANGSIR":
                        freqCount = "08";
                        break;
                    case "POUSH":
                        freqCount = "09";
                        break;
                    case "MAGH":
                        freqCount = "10";
                        break;
                    case "FALGUN":
                        freqCount = "11";
                        break;
                    case "CHAITRA":
                        freqCount = "12";
                        break;
                }
            }

        }

        public static string getItemCode(string name)
        {
            string[] arr = new string[3];
            arr = name.Split('_');
            return arr[1].ToString();
        }
        public static string getMonth(string name)
        {
            string[] arr = new string[3];
            arr = name.Split('_');
            return arr[2].ToString();
        }
        #region Master Setup 
        public ActionResult FrequencySetup()
        {
            return PartialView();
        }
        public ActionResult EmployeeTree()
        {
            return PartialView();
        }

        #endregion Master Setup

        [HttpPost]
        public JsonResult Import(FileUpload form_data)
        {
            Excel.Application application = new Excel.Application();
            try
            {
                if (form_data.file == null || form_data.file.ContentLength == 0)
                {

                    return Json("empty", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (form_data.file.FileName.EndsWith("xls") || form_data.file.FileName.EndsWith("xlsx"))
                    {


                        string paths = Server.MapPath("~/PlanExcell/" + "SalesPlan" + "/" + "ImportedExcelFiles" + "/" + form_data.file.FileName);
                        string strMappath = "~/PlanExcell/" + "SalesPlan" + "/" + "ImportedExcelFiles" + "/";
                        string path = System.IO.Path.Combine(
                                              Server.MapPath(strMappath), form_data.file.FileName);
                        if (!Directory.Exists(strMappath))
                        {
                            Directory.CreateDirectory(Server.MapPath(strMappath));
                        }
                        if (System.IO.File.Exists(path))
                            System.IO.File.Delete(path);
                        form_data.file.SaveAs(path);
                        string sheetName = "Sheet1";
                        string filepath = paths;
                        var excel = new ExcelQueryFactory(filepath);
                        var worksheetNames = excel.GetWorksheetNames();
                        //if (worksheetNames.ElementAt(0) != sheetName && worksheetNames.ElementAt(0) != sheetName1)
                        //{
                        //    return Json("sheetmismatch", JsonRequestBehavior.AllowGet);
                        //}

                        var worksheetname = "";
                        if (worksheetNames.ElementAt(0) == sheetName)
                            worksheetname = sheetName;
                        else
                            worksheetname = worksheetNames.ElementAt(worksheetNames.Count() - 1);
                        var exceldata = from a in excel.Worksheet<ImportItems>(worksheetname) where !String.IsNullOrEmpty(a.PLAN_NAME) select a;
                        var listItems = exceldata.ToList();

                        listItems = listItems.Where(a => !String.IsNullOrEmpty(a.PLAN_NAME) && a.ITEM_CODE != null && a.AMOUNT_QUANTITY != null && a.CALENDAR_TYPE != null).ToList();

                        var distinctItems = listItems.GroupBy(p => p.PLAN_NAME).Select(g => g.First()).ToList();
                        var result = string.Empty;
                        //foreach (var a in distinctItems)
                        //{
                        //    var getItemList = listItems.Where(x => x.PLAN_NAME == a.PLAN_NAME).ToList();
                        //    result = _iPlanSetup.SavePlanFromExcell(getItemList);
                        //}
                        result = _iPlanSetup.SavePlanFromExcellAll(listItems, distinctItems);
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("fileincorrect", JsonRequestBehavior.AllowGet);
                    }

                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                application.Quit();
            }

        }

    }
}