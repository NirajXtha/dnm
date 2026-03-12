using NeoErp.Models.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;

namespace NeoErp.Controllers
{
    public class ReportConfigController : Controller
    {
        // GET: ReportConfig
        public ActionResult Index()
        {
            return PartialView();
        }

        public ActionResult GetReportsSettings()
        {
            try
            {
                string username = "";
                string xmlpath = Server.MapPath("~/App_Data/UserReportSetting.xml");
                var xml = XDocument.Load(xmlpath);
                var result = xml.Descendants("UserReportSetup").Descendants("User").ToList();
                var ReportSettingModellist = new List<ReportSettingModel>();
                dynamic dynamicModel = new System.Dynamic.ExpandoObject();
                foreach (var xe in result.ToList())
                {
                    var value = xe.Value;
                    string[] excludeList = { "Amount", "Quantity", "Rate", "Range" };
                    if (xe.Name.LocalName == "User")
                    {
                        if (xe.Name.LocalName.Contains("Filter") && !excludeList.Any(s => xe.Name.LocalName.Contains(s)) && xe.Value != null)
                            ((IDictionary<String, object>)dynamicModel)[xe.Name.LocalName] = xe.Value.Split(',').ToList();
                        else
                            ((IDictionary<String, object>)dynamicModel)[xe.Name.LocalName] = xe.Attribute("ID").Value;
                        username = xe.Attribute("ID").Value;
                    }
                    bool b = xe.Descendants("Report").Any();

                    if (b == true)
                    {
                        var desc = xe.Descendants().ToList();
                        foreach (var report in desc)
                        {
                            if (report.Name.LocalName == "Report")
                            {
                                ((IDictionary<String, object>)dynamicModel)[xe.Name.LocalName] = username;
                                if (report.Name.LocalName.Contains("Filter") && !excludeList.Any(s => report.Name.LocalName.Contains(s)) && report.Value != null)
                                    ((IDictionary<String, object>)dynamicModel)[report.Name.LocalName] = report.Value.Split(',').ToList();
                                else
                                    ((IDictionary<String, object>)dynamicModel)[report.Name.LocalName] = report.Attribute("ID").Value;

                                var reportdesc = report.Descendants().ToList();

                                foreach (var xee in reportdesc)
                                {
                                    var name = xee.Name.LocalName;
                                    if (xee.Name.LocalName.Contains("Filter") && !excludeList.Any(s => xee.Name.LocalName.Contains(s)) && xee.Value != null)
                                        ((IDictionary<String, object>)dynamicModel)[xee.Name.LocalName] = xee.Value.Split(',').ToList();
                                    else
                                        ((IDictionary<String, object>)dynamicModel)[xee.Name.LocalName] = xee.Value;

                                }
                                ReportSettingModel reportSettingModel = GetObject<ReportSettingModel>(dynamicModel);
                                ReportSettingModellist.Add(reportSettingModel);
                                dynamicModel = new System.Dynamic.ExpandoObject();
                            }
                        }
                    }
                }


                return Json(ReportSettingModellist, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ReportSettingModel reportSettingModel = new ReportSettingModel();
                return Json(reportSettingModel, JsonRequestBehavior.AllowGet);
            }

        }

        T GetObject<T>(IDictionary<string, object> dict)
        {
            Type type = typeof(T);
            var obj = Activator.CreateInstance(type);

            foreach (var kv in dict)
            {
                type.GetProperty(kv.Key).SetValue(obj, kv.Value);
            }
            return (T)obj;
        }

        [HttpPost]
        public void SaveReportConfig(string multipleuserId, string userName, string reportName)
        {
            string path = Server.MapPath("~/App_Data/UserReportSetting.xml");
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Indent = true;
            xmlWriterSettings.NewLineOnAttributes = true;
            string[] userarray = multipleuserId.Split(',');
            XDocument xDocument = XDocument.Load(path);
            XElement userReportPresent = xDocument.Descendants("UserReportSetup")
                .Descendants("User").Where(x => (string)x.Attribute("ID") == userName.ToUpper())
                .Descendants("Report").Where(x => (string)x.Attribute("ID") == reportName).FirstOrDefault();
           
            //generate new node 
            XElement root;
            
            foreach (var user in userarray)
            {
                XElement dataAlreadyExists = xDocument.Descendants("UserReportSetup")
                .Descendants("User").Where(x => (string)x.Attribute("ID") == user.ToUpper())
                .Descendants("Report").Where(x => (string)x.Attribute("ID") == reportName).FirstOrDefault();
                if (dataAlreadyExists != null)//delete old node
                {
                    userReportPresent.Ancestors().FirstOrDefault().Remove();
                }
                root = xDocument.Element("UserReportSetup");

                //add user node
                root.Add(new XElement("User",
                             new XAttribute("ID", user.ToUpper()),
                             new XAttribute("Name", user.ToUpper()),
                              userReportPresent
                            ));
            }
            xDocument.Save(path);

        }
    }
}