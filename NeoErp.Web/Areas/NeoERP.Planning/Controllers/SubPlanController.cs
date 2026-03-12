using NeoErp.Planning.Service.Interface;
using NeoErp.Planning.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoERP.Planning.Controllers
{
    public class SubPlanController : Controller
    {
        private ISubPlanRepo _subplan;
        private IPlanSetup _plansetup;
        public SubPlanController(ISubPlanRepo subplan, IPlanSetup plansetup)
        {
            this._subplan = subplan;
            this._plansetup = plansetup;
        }
        // GET: SubPlan

        [HttpPost]
        public JsonResult SaveSubPlan(FormCollection fc)
        {
            string plancode = fc["\"planName"];
            string subplanName = fc["subplanName"];
            string choosedsubgroup = fc["ChoosedSubGroup"];
            string frequencylabel = fc["FrequencyLabel"];
            string productPlanName = fc["productPlanName"];
            string headertype = fc["headertype"];
            var startEndDate = getStartEndDate(productPlanName);
            var dateRange = _subplan.dateRange(startEndDate[0].ToString(), startEndDate[1].ToString());
            var items = _plansetup.getItemByCode(plancode, null);
            var subplancode = _subplan.SaveSubPlan(Convert.ToInt32(plancode).ToString(), subplanName);
            var timeframes = _subplan.getSubPlanTimeFrame(plancode);

            List<string> abc = new List<string>();
            foreach (var key in fc.AllKeys)
            {
                if (key.Contains("frequency_I"))
                {
                    abc.Add(key);

                }
            }
            List<string> finalvalues = new List<string>();
            foreach (var ab in abc)
            {
                var abcd = ab.Split('_');
                var value = abcd[2];
                if (ab.Contains(value))
                {
                    if (!finalvalues.Any(a => a.Contains(value)))
                        finalvalues.Add(ab);
                }
            }

            foreach (var key in finalvalues)
            {
                var code = getCode(key);
                var preCode = getPreCode(key);
                var masterCode = getMasterCode(key);
                if (string.Equals(headertype.ToLower(), "frequency"))
                {
                    if (frequencylabel == "WEEK") // for week
                    {
                        int i = 0, count = 0, daterangecount = 0;
                        foreach (var ic in items)
                        {
                            var keys = fc.AllKeys.Where(x => x.Contains("frequency" + '_' + "I" + '_' + code + '_')).ToList();
                            int dateIndex = 0;
                            count = 0;
                            daterangecount = 0;
                            foreach (var item in keys)
                            {
                                for (int x = dateIndex; x < dateRange.Count; x++)
                                {
                                    var date = dateRange[x];
                                    decimal finalvalue = 0;

                                    if (fc[item].Contains("\""))
                                    {
                                        finalvalue = Convert.ToDecimal(fc[item].Replace("\"", string.Empty)) / 7;
                                    }
                                    else
                                    {
                                        finalvalue = Convert.ToDecimal(fc[item]) / 7;
                                    }
                                    var preCodes = items.Where(a => a.ITEM_CODE == ic.ITEM_CODE).FirstOrDefault();
                                    _subplan.SaveSubGroupWiseSubPlan(subplancode, ic.ITEM_CODE, preCodes.PRE_ITEM_CODE, preCodes.MASTER_ITEM_CODE, code, masterCode, preCode, finalvalue.ToString(), date.DATE_RANGES, choosedsubgroup);

                                    count++;
                                    daterangecount++;
                                    if (count == 7)
                                    {
                                        count = 0;
                                        dateIndex = dateIndex + 7;
                                        i++;
                                        break;
                                    }
                                    if (daterangecount == dateRange.Count)
                                    {
                                        dateIndex = dateRange.Count;
                                        break;
                                    }
                                }

                            }

                        }
                    }
                    else if (string.Equals(frequencylabel.ToLower(), "month"))
                    { // for month

                        foreach (var ic in items)
                        {
                            var keys = fc.AllKeys.Where(x => x.Contains("frequency" + '_' + "I" + '_' + code + '_')).ToList();

                            foreach (var item in keys)
                            {
                                string monthNow = item.Split('_')[5];
                                string yearNow = item.Split('_')[6];
                                for (int x = 0; x < dateRange.Count; x++)
                                {
                                  var date = dateRange[x];
                                    decimal finalvalue = 0;
                                    if ((Convert.ToInt32(monthNow) == date.DATE_RANGES.Month) && (Convert.ToInt32(yearNow) ==date.DATE_RANGES.Year))
                                    {
                                        //continue;
                                        int daysInMonth = DateTime.DaysInMonth(date.DATE_RANGES.Year, date.DATE_RANGES.Month);
                                        if (fc[item].Contains("\""))
                                        {
                                            finalvalue = Convert.ToDecimal(fc[item].Replace("\"", string.Empty)) / daysInMonth;
                                        }
                                        else
                                        {
                                            finalvalue = Convert.ToDecimal(fc[item]) / daysInMonth;
                                        }
                                        var preCodes = items.Where(a => a.ITEM_CODE == ic.ITEM_CODE).FirstOrDefault();
                                        _subplan.SaveSubGroupWiseSubPlan(subplancode, ic.ITEM_CODE, preCodes.PRE_ITEM_CODE, preCodes.MASTER_ITEM_CODE, code, masterCode, preCode, finalvalue.ToString(), date.DATE_RANGES, choosedsubgroup);
                                    }
                                }
                            }
                        } // end for month insertion
                          //break;
                    }
                }
                else if (string.Equals(headertype.ToLower(), "productname"))
                {
                    string timeframename = timeframes[0].TIME_FRAME;
                    foreach (var ic in items)
                    {
                        var keys = fc.AllKeys.Where(x => x.Contains("frequency" + '_' + "I" + '_' + code + '_')).ToList();
                        
                        foreach (var item in keys)
                        {
                            string itemCode = item.Split('_')[5];
                            if (ic.ITEM_CODE == itemCode)
                            {
                                for (int x = 0; x < dateRange.Count; x++)
                                {
                                    var date = dateRange[x];
                                    decimal finalvalue = 0;

                                    int divideDay = 1;
                                    if (string.Equals(timeframename.ToLower(), "week"))
                                    {
                                        divideDay = 7;
                                    }
                                    else if (string.Equals(timeframename.ToLower(), "month"))
                                    {
                                        divideDay = DateTime.DaysInMonth(date.DATE_RANGES.Year, date.DATE_RANGES.Month);
                                    }


                                    if (fc[item].Contains("\""))
                                    {
                                        finalvalue = Convert.ToDecimal(fc[item].Replace("\"", string.Empty)) / divideDay;
                                    }
                                    else
                                    {
                                        finalvalue = Convert.ToDecimal(fc[item]) / divideDay;
                                    }
                                    var preCodes = items.Where(a => a.ITEM_CODE == ic.ITEM_CODE).FirstOrDefault();
                                    _subplan.SaveSubGroupWiseSubPlan(subplancode, ic.ITEM_CODE, preCodes.PRE_ITEM_CODE, preCodes.MASTER_ITEM_CODE, code, masterCode, preCode, finalvalue.ToString(), date.DATE_RANGES, choosedsubgroup);

                                }
                            }                            
                            
                        }
                    } // end for month insertion

                }

            }

            return Json("Success", JsonRequestBehavior.AllowGet);
        }


        public static string getCode(string name)
        {
            string[] arr = new string[5];
            arr = name.Split('_');
            return arr[2].ToString();
        }
        public static string getPreCode(string name)
        {
            string[] arr = new string[5];
            arr = name.Split('_');
            return arr[3].ToString();
        }
        public static string getMasterCode(string name)
        {
            string[] arr = new string[5];
            arr = name.Split('_');
            return arr[4].ToString();
        }
        public static string getTimeFrame(string name)
        {
            string[] arr = new string[5];
            arr = name.Split('_');
            return arr[5].ToString();
        }
        public static string[] getStartEndDate(string startEndDate)
        {

            string[] arr = new string[2];
            var startDate = startEndDate.Substring(startEndDate.IndexOf("between") + 8, 9);
            var endDate = startEndDate.Substring(startEndDate.IndexOf("to") + 3, 9);
            arr[0] = startDate;
            arr[1] = endDate;
            return arr;
        }
    }
}