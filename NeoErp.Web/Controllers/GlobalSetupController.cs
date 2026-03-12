using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Xml;
using NeoErp.Models.Common;
using System.Configuration;
using NeoErp.Core.Services.MenuControlService;

namespace NeoErp.Controllers
{
    public class GlobalSetupController : Controller
    {
       
        [HttpPost]
        public void UpdateDb(DbsetupModel model)
        {
            var connStr = ConfigurationManager.ConnectionStrings["NeoErpCoreEntity"].ConnectionString.Split(';');
            var index = Array.FindIndex(connStr, x => x.Contains("USER ID"));
            var Company = connStr[index].Substring(connStr[index].IndexOf("=") + 1).TrimEnd('"');

            string path = Server.MapPath("~/App_Data/FiscalYearDBSetup.xml");
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Indent = true;
            xmlWriterSettings.NewLineOnAttributes = true;

            if (System.IO.File.Exists(path) == false) //if not exist case
            {
                using (XmlWriter writer = XmlWriter.Create(path, xmlWriterSettings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("DBSetup");
                    //generate user node
                    writer.WriteStartElement("Company");
                    writer.WriteAttributeString("ID", Company.ToUpper());
                    writer.WriteAttributeString("Name", Company.ToUpper());


                    writer.WriteStartElement("FiscalYear");
                    writer.WriteAttributeString("ID", model.fiscalYear);
                    writer.WriteAttributeString("Name", model.fiscalYear);


                    if (model.dbName != null)
                    {
                        //Insert date Format
                        writer.WriteElementString("DBName", model.dbName);
                    }

                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    // writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
            else // file exist case
            {

                XDocument xDocument = XDocument.Load(path);
                XElement userReportPresent = xDocument.Descendants("DBSetup")
                 .Descendants("Company").Where(x => (string)x.Attribute("ID") == Company.ToUpper())
                .Descendants("FiscalYear").Where(x => (string)x.Attribute("ID") == model.fiscalYear).FirstOrDefault();

                if (userReportPresent != null)//update old node
                {

                    try
                    {
                        userReportPresent.Attribute("ID").Value = model.fiscalYear;
                    }
                    catch
                    {
                        userReportPresent.Add(new XElement("FiscalYear", model.fiscalYear.ToString()));
                    }
                    try
                    {
                        userReportPresent.Elements("DBName").Single().Value = model.dbName;
                    }
                    catch
                    {
                        userReportPresent.Add(new XElement("DBName", model.dbName));
                    }

                }
                //if(userReportPresent!=null)
                //{

                //    XDocument xdoc = XDocument.Load(path);
                //    var product = xdoc.Descendants("FiscalYear").FirstOrDefault(p => p.Attribute("ID").Value == model.fiscalYear);
                //    product.Remove();
                //    xdoc.Save(path);
                //}
                
                else //generate new node 
                {

                    XElement root = xDocument.Descendants("DBSetup")
                .Descendants("Company").Where(x => (string)x.Attribute("ID") == Company.ToUpper()).FirstOrDefault();
                    if(root == null)
                    {
                        var newroot = xDocument.Descendants("DBSetup").FirstOrDefault();
                        newroot.Add(new XElement("Company",
                            new XAttribute("ID", Company.ToUpper()),
                            new XAttribute("Name", Company.ToUpper())
                        ));
                        root = xDocument.Descendants("DBSetup")
                .Descendants("Company").Where(x => (string)x.Attribute("ID") == Company.ToUpper()).FirstOrDefault();
                    }
                    root.Add(new XElement("FiscalYear",
                        new XAttribute("ID", model.fiscalYear.ToUpper()),
                        new XAttribute("Name", model.fiscalYear.ToUpper())
                        ));

                    XElement rootUserReport = xDocument.Descendants("DBSetup")
                    .Descendants("FiscalYear").Where(x => (string)x.Attribute("ID") == model.fiscalYear).FirstOrDefault();

                    if (model.dbName != null)
                        rootUserReport.Add(new XElement("DBName", model.dbName));
                    
                }

                xDocument.Save(path);
            }

        }

        
        [Authorize]
        public ActionResult index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DeleteDbList(DbsetupModel model)
        {
            string path = Server.MapPath("~/App_Data/FiscalYearDBSetup.xml");
            XDocument xdoc = XDocument.Load(path);
            var product = xdoc.Descendants("FiscalYear").FirstOrDefault(p => p.Attribute("ID").Value == model.fiscalYear);
            product.Remove();
            xdoc.Save(path);
            return View();
        } 
    }
}