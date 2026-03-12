using NeoErp.Core;
using NeoErp.Core.Caching;
using NeoErp.Core.Infrastructure;
using NeoErp.Core.Models;
using NeoErp.Core.Models.Log4NetLoggin;
using NeoErp.Core.Plugins;
using NeoErp.Core.Services.CommonSetting;
using NeoErp.Data;
using NeoErp.Models;
using NeoErp.Models.Common;
using NeoErp.Sales.Modules.Services.Services;
using NeoErp.Services;
using NeoErp.Services.UserService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using NeoErp.Sales.Modules.Services.Models;
namespace NeoErp.Controllers
{
    public class MainController : Controller
    {
        //
        // GET: /Home/

        private readonly IPluginFinder _pluginFinder;
      //  private readonly IWebHelper _webHelper;
        private string LoggedInUserId;
        public ISalesRegister _salesRegister { get; set; }
        private ICacheManager _cacheManager;
        private ISalesDashboardService _salesService;
        private IUserChartPermission _userChartPermisson;
        private ISettingService _setting;
        private ILogErp _logErp;
        private IUserService _userService;
        private IDbContext _dbContext;
        private IWorkContext _workContext;
        public MainController(IPluginFinder pluginFinder, ISalesDashboardService salesService, IUserChartPermission userChartPermission, ISettingService settingService, IUserService userService, IDbContext dbContext, IWorkContext workContext, ISalesRegister salesRegister)
        {
            this._pluginFinder = pluginFinder;
            //  this._webHelper = webHelper;
            var workingContent = EngineContext.Current.Resolve<IWorkContext>();
            LoggedInUserId = workingContent.CurrentUserinformation.login_code.ToString();
            this._salesService = salesService;
            this._userChartPermisson = userChartPermission;
            this._setting = settingService;
            this._logErp = new LogErp(this);
            _userService = userService;
            _dbContext = dbContext;
            _workContext = workContext;
            _salesRegister = salesRegister;
        }
        public ActionResult Index()
        {
            //var test = _HRMEntities.EmployeeInfo.ToList();
            // var test2 = EngineContext.Current.Resolve<NeoCoreEntity>();
            var defaultaction = ConfigurationManager.AppSettings["DefaultAction"].ToString();
            var defaultpath = ConfigurationManager.AppSettings["DefaultController"].ToString();
            return RedirectToAction(defaultaction, controllerName: defaultpath);
        }


        [HttpGet]
        public ActionResult DashBoard()
        {
            if (GetAvailableUserSettingChart() < 1)
                return RedirectToAction("UserChartSetting");
            else
                return View("ChartDashboard");

        }
        public ActionResult DatabaseEntitiesSetup()
        {
            return PartialView();
        }
        #region Dashboard
        //[HttpGet]
        //public ActionResult GlobalDashboard()
        //{
        //    int chartCount = GetAvailableUserSettingChart();

        //    _logErp.InfoInFile("Redirect to Global dashboard!");
        //    //if (chartCount < 1)
        //    //{
        //    //    //return View();
        //    //    return RedirectToAction("UserChartSetting");
        //    //}
        //    //else
        //    //{
        //    string widgets = _salesService.GetDashboardWidgets(User.Identity.Name.ToUpper(), "Personal");
        //    _logErp.InfoInFile(widgets + $@"Returned using {User.Identity.Name.ToUpper()} and type Personal");


        //    if (widgets != null)
        //        ViewBag.widgets = widgets.Split(',').Where(s => !string.IsNullOrEmpty(s)).ToArray();
        //    //return RedirectToRoute(new { Controller = "sales/SalesHome", action = "Dashboard" });

        //    var pluginDescriptors = _pluginFinder.GetPluginDescriptors(LoadPluginsMode.InstalledOnly, 0).ToList();
        //    _logErp.WarnInDB(pluginDescriptors.Count + "installed plugin found for main app");

        //    //TODO Remove this session storing code if not used then
        //    //  var moduleCollection = SetAllModuelCodeToSession(pluginDescriptors);  

        //    ViewBag.CompanyCode = _workContext.CurrentUserinformation.company_code.ToString();
        //    ViewBag.BranchCode = _workContext.CurrentUserinformation.branch_code.ToString();
        //    ViewBag.UserId = _workContext.CurrentUserinformation.User_id.ToString();
        //    ViewBag.BaseUrlForDashboard = ConfigurationManager.AppSettings["baseUrlForDashboard"].ToString();
        //    //ViewBag.StartFromDate= _workContext.CurrentUserinformation.startFiscalYear.ToString();
        //    //ViewBag.EndToDate = _workContext.CurrentUserinformation.endfiscalyear.ToString();
        //    ViewBag.StartFromDate = "17-Jul-2024";
        //    ViewBag.EndToDate = "16-Jul-2025";
        //    return View("GlobalDashboardMenu", pluginDescriptors);
        //    // }
        //}

        #endregion

        [HttpGet]
        public ActionResult GlobalDashboard()
        {
            int chartCount = GetAvailableUserSettingChart();

            _logErp.InfoInFile("Redirect to Global dashboard!");
            //if (chartCount < 1)
            //{
            //    //return View();
            //    return RedirectToAction("UserChartSetting");
            //}
            //else
            //{
            string widgets = _salesService.GetDashboardWidgets(User.Identity.Name.ToUpper(), "Personal");
            _logErp.InfoInFile(widgets + $@"Returned using {User.Identity.Name.ToUpper()} and type Personal");


            if (widgets != null)
                ViewBag.widgets = widgets.Split(',').Where(s => !string.IsNullOrEmpty(s)).ToArray();
            //return RedirectToRoute(new { Controller = "sales/SalesHome", action = "Dashboard" });

            var pluginDescriptors = _pluginFinder.GetPluginDescriptors(LoadPluginsMode.InstalledOnly, 0).ToList();
            _logErp.WarnInDB(pluginDescriptors.Count + "installed plugin found for main app");

            //TODO Remove this session storing code if not used then
            //  var moduleCollection = SetAllModuelCodeToSession(pluginDescriptors);  



            return View("GlobalDashboardMenu", pluginDescriptors);
            // }
        }
        [HttpGet]
        public ActionResult NeoGlobalDashboard()
        {
            int chartCount = GetAvailableUserSettingChart();

            _logErp.InfoInFile("Redirect to Global dashboard!");
            //if (chartCount < 1)
            //{
            //    //return View();
            //    return RedirectToAction("UserChartSetting");
            //}
            //else
            //{
            string widgets = _salesService.GetDashboardWidgets(User.Identity.Name.ToUpper(), "Personal");
            _logErp.InfoInFile(widgets + $@"Returned using {User.Identity.Name.ToUpper()} and type Personal");


            if (widgets != null)
                ViewBag.widgets = widgets.Split(',').Where(s => !string.IsNullOrEmpty(s)).ToArray();
            //return RedirectToRoute(new { Controller = "sales/SalesHome", action = "Dashboard" });

            var pluginDescriptors = _pluginFinder.GetPluginDescriptors(LoadPluginsMode.InstalledOnly, 0).ToList();
            _logErp.WarnInDB(pluginDescriptors.Count + "installed plugin found for main app");

            //TODO Remove this session storing code if not used then
            //  var moduleCollection = SetAllModuelCodeToSession(pluginDescriptors);  

            ViewBag.CompanyCode = _workContext.CurrentUserinformation.company_code.ToString();
            ViewBag.BranchCode = _workContext.CurrentUserinformation.branch_code.ToString();
            ViewBag.UserId = _workContext.CurrentUserinformation.User_id.ToString();
            //ViewBag.StartFromDate= _workContext.CurrentUserinformation.startFiscalYear.ToString();
            //ViewBag.EndToDate = _workContext.CurrentUserinformation.endfiscalyear.ToString();
            ViewBag.StartFromDate = "17-Jul-2023";
            ViewBag.EndToDate = "16-Jul-2024";
            return View("NeoGlobalDashboard", pluginDescriptors);
        }
        public static int GetFinancialYearStart(DateTime input)
        {
            return input.Month < 9 ? input.Year - 1 : input.Year;
        }
        //  [Authorize]
        public ActionResult DashBoardPartial(string page = "1")
        {
            if (page == "1")
                Session["pagenumberPersonal"] = null;
            int intpage = 1;

            int pagesize = 2;
            // Slaes Define draggerable save
            var model = new DashboardViewModel();
            if (!string.IsNullOrEmpty(Session["pagenumberPersonal"] as string))
            {
                intpage = Convert.ToInt16(Session["pagenumberPersonal"]) + 1;
                //    if (Session["pagenumber"].ToString() == page)
                //    {
                //        intpage = intpage + 1;

                //    }
                //    else if(Convert.ToInt16(Session["pagenumber"].ToString()) > Convert.ToInt16(page))
                //    {
                //        intpage = intpage + 1;
                //    }
                Session["pagenumberPersonal"] = intpage.ToString();
            }
            else
            {
                Session["pagenumberPersonal"] = intpage.ToString();
            }
            string widgets = _salesService.GetDashboardWidgets(User.Identity.Name.ToUpper(), "Personal");
            if (widgets != null)
            {
                var widgetsListTotal = widgets.Split(',').Where(s => !string.IsNullOrEmpty(s)).ToList();
                model.CountTotalChart = widgetsListTotal.Count;
                model.chartList = widgetsListTotal.Skip((intpage - 1) * pagesize).Take(pagesize).ToList();

            }
            else
            {
                string xmlpath = Server.MapPath("~/App_Data/UserChartSetting.xml");
                var xml = XDocument.Load(xmlpath);
                var query = from c in xml.Root.Descendants("User")
                            where (string)c.Attribute("ID") == LoggedInUserId.ToUpper()
                            select c.Element("ChartName").Value;
                var result = query.ToList();
                model.AllchartList = result.Skip((intpage - 1) * pagesize).Take(pagesize).ToList();

            }


            return View(model);
        }
        private int GetAvailableUserSettingChart()
        {
            List<string> allWidgets = _salesService.GetDashboardWidgets(User.Identity.Name, "Personal_Dashboard")?.Split(',').ToList();
            ViewBag.AvailableChart = allWidgets;
            ViewData["AvailableChart"] = allWidgets;
            TempData["AvailableChart"] = allWidgets;
            return (allWidgets ?? new List<string>()).Count();
        }

        public ActionResult InstalledPluginList()
        {
            var pluginDescriptors = _pluginFinder.GetPluginDescriptors(LoadPluginsMode.InstalledOnly, 0).ToList();

            return PartialView(pluginDescriptors);
        }
        public ActionResult InstalledPluginListForMenu()
        {
            var pluginDescriptors = _pluginFinder.GetPluginDescriptors(LoadPluginsMode.InstalledOnly, 0).ToList();

            return PartialView(pluginDescriptors);
        }

        public JsonResult InstalledPuginListForGlobalMenu()

        {
            List<PluginDescriptor> pluginDescriptors = _pluginFinder.GetPluginDescriptors(LoadPluginsMode.InstalledOnly, 0).ToList();
            return Json(pluginDescriptors, JsonRequestBehavior.AllowGet);
        }

        public ActionResult pluginLists()
        {
            var pluginDescriptors = _pluginFinder.GetPluginDescriptors(LoadPluginsMode.All, 0).ToList();

            return PartialView(pluginDescriptors);
        }

        public ActionResult ClearCache()
        {

            var cacheManager = new MemoryCacheManager();
            cacheManager.Clear();
            var test = new NeoNullCache();
            test.Clear();
            return RedirectToAction("LogIn", "Home");
        }

        public void RestartAppDomain()
        {
            NeoErp.Core.WebHelper helper = new Core.WebHelper(HttpContext);
     //   var test=helper.GetCurrentIpAddress();
            helper.RestartAppDomain(true, "/Home/LogIn");
        }

        public ActionResult DbSetup()
        {
            var conInfo = NeoErp.Data.ConnectionManager.GetConInfo();

            DatabaseSettingModel dbsettings = new DatabaseSettingModel()
            {
                ConType = ConnectionType.SqlServer,
                Database = conInfo.Database,
                Password = conInfo.Password,
                ServerName = conInfo.Server,
                UserName = conInfo.UserName
            };

            return View(dbsettings);

        }

        [HttpPost]
        [ActionName("DbSetup")]
        public ActionResult DbSetupSave(DatabaseSettingModel dbsettings)
        {

            //var configuration = WebConfigurationManager.OpenWebConfiguration("~");
            //var section = (ConnectionStringsSection)configuration.GetSection("connectionStrings");
            //section.ConnectionStrings["connString"].ConnectionString = "Data Source=...";
            //configuration.Save();
            return RedirectToAction("DbSetup");
        }


        public ActionResult Uninstall(string systemname)
        {
            try
            {
                //get plugin system name
                string systemName = systemname;
                var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName(systemName, LoadPluginsMode.All);
                if (pluginDescriptor == null)
                    //No plugin found with the specified id
                    return RedirectToAction("DashBoard");

                //check whether plugin is installed
                if (!pluginDescriptor.Installed)
                    return RedirectToAction("DashBoard");
                //uninstall plugin
                pluginDescriptor.Instance().Uninstall();
                //restart application
                NeoErp.Core.WebHelper helper = new Core.WebHelper(HttpContext);
                helper.RestartAppDomain(true, "/Main/DashBoard");
            }
            catch (Exception exc)
            {

            }

            return RedirectToAction("login", "home");
            // return RedirectToAction("DashBoard");
        }

        public ActionResult install(string systemname)
        {
            try
            {
                var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName(systemname, LoadPluginsMode.All);
                if (pluginDescriptor == null)
                    //No plugin found with the specified id
                    return RedirectToAction("DashBoard");

                //check whether plugin is not installed
                if (pluginDescriptor.Installed)
                    return RedirectToAction("DashBoard");

                //install plugin
                pluginDescriptor.Instance().Install();
                NeoErp.Core.WebHelper helper = new Core.WebHelper(HttpContext);
                helper.RestartAppDomain(true, "/Main/DashBoard");

            }
            catch (Exception exc)
            {

            }
            return RedirectToAction("login", "home");
            // return RedirectToAction("DashBoard");
        }

        public ActionResult UserChartSetting()
        {
            List<string> allWidgets = _salesService.GetDashboardWidgetsForPersonalDashboard(User.Identity.Name);
            ViewBag.ListChart = allWidgets;
            int chartCount = GetAvailableUserSettingChart();
            return View(allWidgets);
        }

        [HttpPost]
        public ActionResult UserChartSetting(FormCollection coll)
        {
            var checkedCharts = coll.GetValues("ChartAvailable");
            if (checkedCharts != null)
                _salesService.SaveDashboardWidgets(User.Identity.Name, string.Join(",", checkedCharts), "Personal_Dashboard");
            else
                _salesService.ResetDashboardWidgets(User.Identity.Name, "'Personal_Dashboard'");
            return RedirectToAction("DashBoard");
        }

        public ActionResult PreviewChart(string ChartName)
        {
            //ChartName= Request.QueryString["ChartName"].ToString();
            // get valueName from display name
            string valueName = string.Empty;
            string path = Server.MapPath("~/App_Data/InstalledChart.txt");
            using (StreamReader reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrEmpty(line))
                        continue;
                    string displayNameInFile = line.Split(';')[1].ToString();
                    if (string.Equals(displayNameInFile.Trim(), ChartName.Trim()))
                    {
                        valueName = line.Split(';')[0].ToString();
                    }
                }
            }

            if (ChartName.Split(',').Count() > 1)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            return PartialView("~/Areas/NeoErp.sales.Module/Views/SalesHome/_" + valueName + ".cshtml");
        }
        private void SaveUserSetting(List<string> listChart)
        {
            string path = Server.MapPath("~/App_Data/UserChartSetting.xml");
            XDocument xDocument = XDocument.Load(path);
            List<XElement> userChartPresent = xDocument.Descendants("User").Where(x => (string)x.Attribute("ID") == LoggedInUserId.ToUpper()).ToList();
            if (userChartPresent.Count > 0)
            {
                foreach (var item in userChartPresent)
                {
                    item.Remove();
                }
                xDocument.Save(path);
            }
            if (listChart.Count > 0)
            {
                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Indent = true;
                xmlWriterSettings.NewLineOnAttributes = true;

                if (System.IO.File.Exists(path) == false)
                {
                    using (XmlWriter writer = XmlWriter.Create(path, xmlWriterSettings))
                    {
                        writer.WriteStartDocument();
                        writer.WriteStartElement("ChartUserSetup");

                        foreach (var item in listChart)
                        {
                            writer.WriteStartElement("User");
                            writer.WriteAttributeString("ID", LoggedInUserId.ToUpper());
                            writer.WriteAttributeString("Name", item);

                            writer.WriteElementString("ChartName", item);
                            writer.WriteElementString("ChartActive", "1");
                            writer.WriteElementString("ChartType", "Line");

                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                        writer.WriteEndDocument();
                    }
                }
                else // file exist case
                {
                    XElement root = xDocument.Element("ChartUserSetup");
                    foreach (var item in listChart)
                    {
                        root.Add(new XElement("User",
                                new XAttribute("ID", LoggedInUserId.ToUpper()),
                                new XAttribute("Name", item),
                               new XElement("ChartName", item),
                               new XElement("ChartActive", "0"),
                               new XElement("ChartType", "bar")));
                    }
                    xDocument.Save(path);
                }
            }
        }


        [HttpPost]
        public bool SaveUserWiseChartConfig(UserWiseChartConfigModel model)
        {
            string path = Server.MapPath("~/App_Data/UserWiseChartConfig.xml");
            if (model != null)
            {
                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Indent = true;
                xmlWriterSettings.NewLineOnAttributes = true;

                if (System.IO.File.Exists(path) == false) //file not exist case
                {
                    using (XmlWriter writer = XmlWriter.Create(path, xmlWriterSettings))
                    {
                        writer.WriteStartDocument();
                        writer.WriteStartElement("ChartConfig");

                        writer.WriteStartElement("User");
                        writer.WriteAttributeString("ID", LoggedInUserId.ToUpper());
                        writer.WriteAttributeString("Name", LoggedInUserId.ToUpper());

                        writer.WriteStartElement("ChartName");
                        writer.WriteAttributeString("ID", model.ChartName);
                        writer.WriteAttributeString("Name", model.ChartName);

                        writer.WriteElementString("ChartType", model.ChartType);
                        writer.WriteElementString("FieldValue", model.FieldValue);
                        writer.WriteElementString("DateFormat", model.DateFormat);
                        writer.WriteElementString("ShowLabel", model.ShowLabel);
                        writer.WriteElementString("IsStack", model.IsStack);
                        writer.WriteElementString("SalesReturn", model.SalesReturn);
                        writer.WriteElementString("FiscalYear", model.FiscalYear);
                        writer.WriteElementString("ProductList", model.ProductList);

                        writer.WriteEndElement();
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                        writer.WriteEndDocument();

                    }
                }
                else // file exist case
                {
                    XDocument xDocument = XDocument.Load(path);
                    List<XElement> userChartPresent = xDocument.Descendants("User").Where(x => (string)x.Attribute("ID") == LoggedInUserId.ToUpper())
                                                               .Descendants("ChartName").Where(x => (string)x.Attribute("ID") == model.ChartName).ToList();
                    if (userChartPresent.Count > 0)
                    {
                        foreach (var item in userChartPresent)
                        {
                            item.Remove();
                        }
                        xDocument.Save(path);
                    }

                    XElement root = xDocument.Element("ChartConfig")
                                             .Descendants("User").Where(x => (string)x.Attribute("ID") == LoggedInUserId.ToUpper()).FirstOrDefault();
                    if (root == null)
                    {
                        root = xDocument.Element("ChartConfig");
                        root.Add(new XElement("User",
                                                new XAttribute("ID", LoggedInUserId.ToUpper()),
                                                new XAttribute("NAME", LoggedInUserId.ToUpper())));
                        root = xDocument.Element("ChartConfig")
                                             .Descendants("User").Where(x => (string)x.Attribute("ID") == LoggedInUserId.ToUpper()).FirstOrDefault();
                    }
                    root.Add(new XElement("ChartName",
                               new XAttribute("ID", model.ChartName),
                               new XAttribute("Name", model.ChartName),
                              new XElement("ChartType", model.ChartType),
                              new XElement("FieldValue", model.FieldValue),
                              new XElement("DateFormat", model.DateFormat),
                              new XElement("ShowLabel", model.ShowLabel),
                              new XElement("IsStack", model.IsStack),
                              new XElement("IsStack", model.IsStack),
                              new XElement("FiscalYear", model.FiscalYear),
                              new XElement("ProductList", model.ProductList)));
                    xDocument.Save(path);
                }
            }
            return true;
        }

        [HttpPost]
        public JsonResult GetUserWiseChartConfig(string chartName)
        {
            try
            {
                string xmlpath = Server.MapPath("~/App_Data/UserWiseChartConfig.xml");
                var xml = XDocument.Load(xmlpath);
                var result = xml.Descendants("ChartConfig")
                    .Descendants("User").Where(x => (string)x.Attribute("ID") == User.Identity.Name.ToUpper())
                    .Descendants("ChartName").Where(x => (string)x.Attribute("ID") == chartName).FirstOrDefault();


                UserWiseChartConfigModel chartModel = new UserWiseChartConfigModel()
                {
                    ChartName = chartName,
                    ChartType = result.Elements("ChartType").FirstOrDefault()?.Value,
                    FieldValue = result.Elements("FieldValue").FirstOrDefault()?.Value,
                    DateFormat = result.Elements("DateFormat").FirstOrDefault()?.Value,
                    ShowLabel = result.Elements("ShowLabel").FirstOrDefault()?.Value,
                    IsStack = result.Elements("IsStack").FirstOrDefault()?.Value,
                    SalesReturn = result.Elements("SalesReturn").FirstOrDefault()?.Value,
                    FiscalYear = result.Elements("FiscalYear").FirstOrDefault()?.Value,
                    ProductList = result.Elements("ProductList").FirstOrDefault()?.Value,
                };



                return new JsonResult { Data = chartModel };
            }
            catch (Exception)
            {
                return null;
            }

        }

        public ActionResult ReportMailSender()
        {
            return View();
        }



        public ActionResult UserChartPermission(string module)
        {
            string path = Server.MapPath("~/App_Data/InstalledChart.txt");

            List<string> listChart = new List<string>();
            using (StreamReader reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    listChart.Add(line);
                }
            }
            ViewBag.ListChart = listChart.Where(x => !x.ToUpper().Contains("FISCALYEAR")).ToList(); ;
            int chartCount = GetAvailableUserSettingChart();
            return View();
        }

        public ActionResult UserwiseChartSetting()
        {
            string path = Server.MapPath("~/App_Data/InstalledChart.txt");

            List<string> listChart = new List<string>();
            using (StreamReader reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    listChart.Add(line);
                }
            }
            listChart = listChart.Where(x => !x.ToUpper().Contains("FISCALYEAR")).ToList();
            return View(listChart);
        }
        public ActionResult UserwiseChartSettingPartial()
        {
            string path = Server.MapPath("~/App_Data/InstalledChart.txt");

            List<string> listChart = new List<string>();
            using (StreamReader reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    listChart.Add(line);
                }
            }
            listChart = listChart.Where(x => !x.ToUpper().Contains("FISCALYEAR")).ToList();
            return PartialView(listChart);
        }

        public JsonResult GetLoginUserList()
        {
            var userData = _userChartPermisson.GetLoginUserList();
            return Json(userData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetWidgetsList()
        {
            var userData = _userChartPermisson.GetLoginUserList();
            return Json(userData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public bool SaveUserChartPermission(UserChartModel userChartPermission)
        {
            bool flag = _userChartPermisson.SaveUserChartPermission(userChartPermission);
            return flag;
        }

        public ActionResult UserChartPermissionList(string userName, string moduleName)
        {
            userName = "\'ADMINISTRATOR\',\'MOTI\',\'01\'";
            var test = "\'ADMINISTRATOR\',\'MOTI\',\'01\'";
            moduleName = "Sales-Main";
            return View(_userChartPermisson.GetUserChartPermission(userName, moduleName));
        }

        public ActionResult CompareCharts(string mainMenu = "false")
        {
            //string xmlpath = Server.MapPath("~/App_Data/UserChartSetting.xml");
            //var xml = XDocument.Load(xmlpath);
            //var query = from c in xml.Root.Descendants("User")
            //            where (string)c.Attribute("ID") == LoggedInUserId.ToUpper()
            //            select c.Element("ChartName").Value;
            //var result = query.ToArray();

            //ViewBag.widgets = result;
            string path = Server.MapPath("~/App_Data/InstalledChart.txt");
            List<string> listChart = new List<string>();
            using (StreamReader reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    listChart.Add(line);
                }
            }
            listChart = listChart.Where(x => x.ToUpper().Contains("FISCALYEAR")).ToList();
            ViewBag.widgets = listChart;
            ViewBag.showmenu = mainMenu;

            return View();
        }

        public ActionResult CompareChartDivision(string mainMenu = "false")
        {

            return View();
        }

        public ActionResult CompareChartDivisionCollectionWise(string mainMenu = "false")
        {

            return View();
        }

        public ActionResult LoadPartialCharts(string chartName)
        {
            return PartialView(chartName);
        }
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ChangePassword(ChangePasswordViewModel model)
        {
            var res =
            _userService.ChangePassword(model);
            return Json(res);
        }
        [HttpGet]
        public ActionResult ChangeUserPassword()
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            string getSuperuserFlagQuery = $@"SELECT SUPER_USER_FLAG FROM SC_APPLICATION_USERS where USER_NO='{userid}' AND COMPANY_CODE='{company_code}' AND DELETED_FLAG='N'";
            var superUserFlag = this._dbContext.SqlQuery<string>(getSuperuserFlagQuery).FirstOrDefault();
           // superUserFlag = "Y";
            if (superUserFlag == "Y")
                return PartialView("~/Views/Main/ChangeUserPassword.cshtml");
            else
                return PartialView("~/Views/Main/_ChangeCurrentUserPassword.cshtml");
        }

        public ActionResult AddUser()
        {
            return View();
        }


        [HttpGet]
        public JsonResult GetFavroiteMenusForDashboard(string moduleCode = "01")
        {
            try
            {
                string xmlpath = Server.MapPath("~/App_Data/UserReportSetting.xml");
                var xml = XDocument.Load(xmlpath);
                var result = xml.Descendants("UserReportSetup")
                    .Descendants("User").Where(x => (string)x.Attribute("ID") == User.Identity.Name.ToUpper())
                    .Descendants("Report").Where(x => (string)x.Element("isFavroite") == "true" && (string)x.Element("moduleCode") == moduleCode);
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(result.Reverse().ToList(), Newtonsoft.Json.Formatting.Indented).Replace('@', 'R');
                return Json(json, JsonRequestBehavior.AllowGet);
                //return Json(json, "application/json");
            }
            catch (Exception e)
            {

                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }
       
        [HttpGet]
        public List<DynamicMenu> GetDynamicMenuForDashboard(string ModuleCode)
        {
            var menuList = new List<DynamicMenu>();
            try
            {
                var level = 1;
                var userId = _workContext.CurrentUserinformation.User_id;
                if (!ModuleCode.Contains("0"))
                    ModuleCode = ModuleCode.PadLeft(2, '0');

                menuList = _salesRegister.GetDynamicMenu(userId, level, ModuleCode);
                foreach (var item in menuList)
                {
                    var itemList = new List<DynamicMenu>();
                    itemList = _salesRegister.GetChlidMenu(item.MENU_NO, userId, ModuleCode);
                    //item.Items.AddRange(itemList);
                    item.Items = itemList;
                }

            }
            catch (Exception e)
            {
                //throw e;
            }
            return menuList;
            //return Json(menuList, "application/json");
        }
        [HttpPost]
        public int DeleteFavouriteMenu(string menuName)
        {
            string path = Server.MapPath("~/App_Data/UserReportSetting.xml");
            int status = 0;
            if (System.IO.File.Exists(path))
            {
                XDocument xDocument = XDocument.Load(path);
                XElement userReportPresent = xDocument.Descendants("UserReportSetup")
                    .Descendants("User").Where(x => (string)x.Attribute("ID") == User.Identity.Name.ToUpper())
                    .Descendants("Report").Where(x => (string)x.Attribute("ID") == menuName).FirstOrDefault();
                if (userReportPresent != null)
                {
                    userReportPresent.Remove();
                    xDocument.Save(path);
                    status = 200;
                }
                else
                    status = 304; //not modified
            }
            else
                status = 404;

            return status;
        }
    }
}
