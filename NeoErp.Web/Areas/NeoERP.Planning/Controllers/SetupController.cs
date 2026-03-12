using NeoErp.Planning.Service.Interface;
using NeoErp.Planning.Service.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoERP.Planning.Controllers
{
    public class SetupController : Controller
    {
        public IPlanSetup _iPlanSetup { get; set; }
        IPlan _plan;
        public SetupController(IPlanSetup iPlanSetup, IPlan plan)
        {
            this._iPlanSetup = iPlanSetup;
            this._plan = plan;
        }

        // GET: Setup
        public ActionResult PreferenceSetup()
        {
            var model = this._iPlanSetup.GetPreferenceSetups();
            return PartialView("PreferenceSetup", model);
        }
        public ActionResult ConfigSetup()
        {
            return PartialView("_ConfigSetup");
        }
        public ActionResult EmployeeHandover()
        {
            return PartialView("_employeeWiseHandover");
        }
        public ActionResult TempTableSetup()
       {
            //var model = this._iPlanSetup.GetPreferenceSetups();
            //return PartialView("PreferenceSetup", model);
            return PartialView("_TempTableSetupPartial");
        }
        [HttpPost]
        public ActionResult Save(PreferenceSetupModel model)
        {
            var result = this._iPlanSetup.SavePreferenceSetup(model);
            if (result > 0)
                return Json(new { success = true, responseText = "success" }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { success = false, responseText = "fail" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult SaveBasicSetting()
        {
            var result = this._iPlanSetup.SaveAllPreferenceSetup();
            if (result > 0)
                return Json(new { success = true, responseText = "success" }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { success = false, responseText = "fail" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getFrequencyTitleForPlanEdit(string timeFrameName="Month", string datetype="BS")
        {
            var fiscalYear = ConfigurationManager.AppSettings["FiscalYear"].ToString();
            var date = _iPlanSetup.GetFiscalYear(fiscalYear);
            List<MyColumnSettings> list = new List<MyColumnSettings>();
            list = _iPlanSetup.GetFrequencyTitle(date.START_DATE.ToString("MM/dd/yyyy"), date.END_DATE.ToString("MM/dd/yyyy"), "", timeFrameName, datetype);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getPreferenceSetupForPlan(string planName)
        {
            return Json( this._iPlanSetup.GetPreferenceSetup(planName),JsonRequestBehavior.AllowGet);
        }

    }
}