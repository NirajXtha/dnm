using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NeoErp.Models.Common;
using System.Xml;
using System.Configuration;
using System.Xml.Linq;
using System.Web.Http;

namespace NeoErp.Controllers.Api
{
    public class GlobalSetupController : ApiController
    {
        // GET: GlobalSetup
        public List<DbsetupModel> DbList()
        {
            try
            {
                var connStr = ConfigurationManager.ConnectionStrings["NeoErpCoreEntity"].ConnectionString.Split(';');
                var index = Array.FindIndex(connStr, x => x.Contains("USER ID"));
                var userName = connStr[index].Substring(connStr[index].IndexOf("=") + 1).TrimEnd('"');

                //string xmlpath = Server.MapPath("~/App_Data/FiscalYearDBSetup.xml");
                string xmlpath = HttpContext.Current.Server.MapPath("~/App_Data/FiscalYearDBSetup.xml");
                var xml = XDocument.Load(xmlpath);
                //var result = xml.Descendants("DBSetup").Descendants("Company").Where(x => (string)x.Attribute("ID") == userName);
                var result = xml.Descendants("DBSetup").Descendants("Company").ToList();

                List<DbsetupModel> model = new List<DbsetupModel>();
                foreach (var item in result.Descendants("FiscalYear"))
                {
                    string fiscalYearName = item.Attributes("Name").FirstOrDefault().Value;
                    var dbName = item.Elements("DBName").FirstOrDefault().Value;
                    model.Add(new DbsetupModel() { dbName = dbName, fiscalYear = fiscalYearName });
                }
                return model;
            }
            catch (Exception e)
            {
                return null;
            }

        }
    }
}