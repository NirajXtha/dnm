
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NeoErp.Models;
using Newtonsoft.Json.Linq;

namespace NeoErp.Controllers
{
    public class salesVatController : Controller
    {
        // GET: salesVat

        //sales_report_model sales_data = new sales_report_model("MARRIOTT_7980", "MARRIOTT7980", "", "localhost", "SUJAL");
        sales_report_model sales_data = new sales_report_model("JGITEST", "JGITEST", "1521", "localhost", "orcl");
        [HttpPost]
        public ActionResult production_report_view()
        {
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["NeoErpCoreEntity"].ToString();

                Stream req = Request.InputStream;
                string data = new StreamReader(req).ReadToEnd();
                JObject war = Newtonsoft.Json.Linq.JObject.Parse(data);
                string report_type = "";
                report_type = war["from_date"].ToString();
                var data_res = sales_data.sales_register("01", "01.01", "20-jul-2023", "20-jul-2023");

                var jsonResult = Json(data_res, JsonRequestBehavior.AllowGet);
                //return Json(data_res, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public ActionResult salesVat()
        {   
            return View();
        }
    }
}

