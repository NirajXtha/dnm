using NeoErp.Planning.Service.Interface;
using NeoErp.Planning.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoERP.Planning.Controllers
{
    public class MaterialPlanController : Controller
    {
        public IPlanSetup _iPlanSetup { get; set; }
        public IPlan _plan;
        IMaterialPlanRepo _MaterialPlanRepo;
        public MaterialPlanController(IPlanSetup iPlanSetup, IMaterialPlanRepo MaterialPlanRepo, IPlan PlanRepo)
        {
            this._iPlanSetup = iPlanSetup;
            this._MaterialPlanRepo = MaterialPlanRepo;
            this._plan = PlanRepo;
        }
        // GET: MaterialPlan
        public ActionResult Index(string startdate, string enddate)
        {
            ViewBag.startdate = startdate;
            ViewBag.enddate = enddate;
            string employeCode = this._plan.getUsersEmployeeCode();
            ViewBag.EmployeeCode = employeCode;
            return PartialView();
        }
        public ActionResult IndexPI(string startdate, string enddate)
        {
            ViewBag.startdate = startdate;
            ViewBag.enddate = enddate;
            string employeCode = this._plan.getUsersEmployeeCode();
            ViewBag.EmployeeCode = employeCode;
            return PartialView();
        }
        public ActionResult FavMaterialPlanReport()
        {
            return PartialView();
        }

        public ActionResult CreateMaterialPlan(string plancode)
        {
            ViewBag.PlanCode = plancode;
            ViewBag.ProductSelectionLimit = System.Configuration.ConfigurationManager.AppSettings.Get("ProductSelectionLimit");
            string employeCode = this._plan.getUsersEmployeeCode();
            ViewBag.EmployeeCode = employeCode;
            var model = this._iPlanSetup.GetPreferenceSetup("SALES_PLAN");
            return PartialView(model);
        }
        public ActionResult CreateMaterialPlanReference(string plancode)
        {
            ViewBag.PlanCode = plancode;
            ViewBag.ProductSelectionLimit = System.Configuration.ConfigurationManager.AppSettings.Get("ProductSelectionLimit");
            string employeCode = this._plan.getUsersEmployeeCode();
            ViewBag.EmployeeCode = employeCode;
            var model = this._iPlanSetup.GetPreferenceSetup("SALES_PLAN");
            return PartialView(model);
        }
        public ActionResult MaterialFromProduction(string plancode)
        {
            ViewBag.PlanCode = plancode;
            ViewBag.ProductSelectionLimit = System.Configuration.ConfigurationManager.AppSettings.Get("ProductSelectionLimit");
            string employeCode = this._plan.getUsersEmployeeCode();
            ViewBag.EmployeeCode = employeCode;
            var model = this._iPlanSetup.GetPreferenceSetup("SALES_PLAN");
            return PartialView(model);
        }
        public ActionResult MaterialFromProductionPI(string plancode)
        {
            ViewBag.PlanCode = plancode;
            ViewBag.ProductSelectionLimit = System.Configuration.ConfigurationManager.AppSettings.Get("ProductSelectionLimit");
            string employeCode = this._plan.getUsersEmployeeCode();
            ViewBag.EmployeeCode = employeCode;
            var model = this._iPlanSetup.GetPreferenceSetup("SALES_PLAN");
            return PartialView(model);
        }
        public ActionResult MaterialPlanSetup(string planCode)
        {
            ViewBag.planCode = planCode;
            //var FrequencyTitle = _iPlanSetup.GetFrequencyTitle(planCode);
            return View();
        }
        public ActionResult LoadPlanSetupTreeListPartial(string startDate, string endDate, string timeFrameCode, string timeFrameName, string datetype)
        {
            return PartialView("~/Areas/NeoERP.Planning/Views/MaterialPlan/_MaterialPlanSetupTreeList.cshtml");
        }
        public string SaveMaterialPlans(SaveMaterialPlanModel model)
        {
            var result = this._MaterialPlanRepo.SaveMaterialPlans(model);
            return result;
        }
        public string SaveMaterialPlanReference(SaveMaterialPlanReferenceModel model)
        {
            var result = this._MaterialPlanRepo.SaveMaterialPlanReference(model);
            return result;
        }
        public string SaveMaterialPlan(FormCollection fc, MaterialPlanModel param)
        {
            string planCode = "", freqCode = "";
            var result = string.Empty;
            var childArr = new List<MaterialPlanModel>();
            var plan_Code = planCode;
            var time_frame_code = freqCode;


            string totalValue = string.Empty;
            foreach (var key in fc.AllKeys)
            {
                if (key.Contains("materialQty"))
                {
                    if (fc[key] == null || fc[key] == "")
                        fc[key] = "0";
                    var itemCode = getItemCode(key);
                    var obj = new MaterialPlanModel
                    {
                        REQUIRED_QTY = getFielvalue(getFieldName(key, 2)),
                        REMAINING_QTY = getFielvalue(getFieldName(key, 3)),
                        CALC_QTY = getFielvalue(getFieldName(key, 4)),
                        STOCK = Convert.ToDecimal(getFielvalue(getFieldName(key, 5))),
                        PO_PENDING = Convert.ToDecimal(getFielvalue(getFieldName(key, 6))),
                        ITEM_CODE = itemCode,

                    };
                    childArr.Add(obj);
                }
                else if (key.Contains("finishedItemList"))
                {
                    var fil = string.Empty;
                    fil = fc[key];
                    if (!String.IsNullOrEmpty(fil))
                    {
                        string[] arr = new string[] { fil };
                        if (fil.Contains(","))
                        {
                            arr = fil.Split(',');
                        }
                        var obj = new MaterialPlanModel
                        {
                            finishedItemList = arr.ToList(),
                        };
                    }
                }
            }
            MaterialPlan sp = new MaterialPlan()
            {
                PLAN_EDESC = fc["PLAN_EDESC"],
                PLAN_QTY = fc["materialPlanQty"]
            };
            return result = this._MaterialPlanRepo.SaveMaterialPlan(childArr, sp);
        }

        //public string SaveMaterialPlanReference(FormCollection fc)
        //{
        //    string planCode = "", freqCode = "";
        //    var result = string.Empty;
        //    var parentArr = new List<savePlan>();
        //    var childArr = new List<savePlan>();
        //    var otherArr = new List<savePlan>();
        //    var plan_Code = planCode;
        //    var time_frame_code = freqCode;

        //    string totalValue = string.Empty;
        //    var list = new List<string>();
        //    foreach (var key in fc.AllKeys)
        //    {
        //        if (key.Contains("freqItemNum"))
        //        {
        //            if (fc[key] == null || fc[key] == "")
        //                fc[key] = "0";
        //            if (fc[key].Contains('"'))
        //            {
        //                fc[key] = fc[key].ToString().Trim('"');
        //            }
        //            var value = 0M;
        //            decimal.TryParse(fc[key], out value);
        //            if (value == 0)
        //            {
        //                var monthInt = getMonth(key);
        //                list.Add(monthInt);
        //                continue;
        //            }
        //            var itemCode = getItemCode(key);
        //            var obj = new savePlan
        //            {
        //                name = key,
        //                value = fc[key],
        //                itemCode = itemCode,
        //                skipMonth = list
        //            };
        //            childArr.Add(obj);
        //        }
        //    }
        //    //totalValue=totalValue+parentArr[0].value
        //    var calendar_type = (fc["dateFormat"] == "BS") ? "LOC" : "ENG";
        //    SalesPlan sp = new SalesPlan()
        //    {
        //        dateFormat = fc["dateFormat"],
        //        END_DATE = fc["END_DATE"],
        //        PLAN_EDESC = fc["PLAN_EDESC"],
        //        PLAN_CODE = fc["PLAN_CODE"],
        //        REMARKS = fc["REMARKS"],
        //        START_DATE = fc["START_DATE"],
        //        TIME_FRAME_CODE = fc["TIME_FRAME_CODE"],
        //        TIME_FRAME_EDESC = fc["TIME_FRAME_EDESC"],
        //    };
        //    var freqValueSum = 0.0;
        //    foreach (var i in childArr)
        //    {
        //        string freqname = string.Empty;
        //        string freqCount = string.Empty;
        //        getFreqName(i.name, calendar_type, out freqname, out freqCount);
        //        var fre = new freqNameVlaue();
        //        foreach (var oi in otherArr)
        //        {
        //            if (oi.itemCode == i.itemCode)
        //            {
        //                fre.fvalue_amt = oi.value_amt;
        //            }
        //        }
        //        fre.skipMonth = i.skipMonth;
        //        fre.fname = freqCount + "_" + i.name.Split('_')[3];
        //        fre.fvalue = i.value;
        //        freqValueSum += Convert.ToDouble(i.value);
        //        i.frequency.Add(fre);
        //    }
        //    return result = this._MaterialPlanRepo.SaveMaterialPlanReference(childArr, sp);
        //}
        public static string getMonth(string name)
        {
            string[] arr = new string[3];
            arr = name.Split('_');
            return arr[2].ToString();
        }
        public decimal getFielvalue(string name)
        {
            string[] arr = new string[2];
            arr = name.Split('$');
            var val = arr[1].ToString();
            if (val == null || val == "null")
                val = "0";
            return Convert.ToDecimal(val);
        }
        public string getFieldName(string name, int index)
        {
            string[] arr = new string[7];
            arr = name.Split('_');
            return arr[index].ToString();
        }
        public ActionResult CopyPlan(int planCode)
        {
            var model = this._iPlanSetup.GetPreferenceSetups();
            PL_MATERIAL_PLAN result = this._MaterialPlanRepo.GetPlanDetailValueByPlanCode(planCode);
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
            foreach (var item in formData)
            {
                if (item.ToString().Contains("planName_copy"))
                {
                    planName = formData[item.ToString()];
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
                copied = this._MaterialPlanRepo.cloneMaterialPlan(planCode, planName, customers, employees, branchs, divisions, remarks);

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
    }
}