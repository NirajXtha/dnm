using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using System.Threading;
using System.Globalization;
using NeoErp.Core.Integration;
using NeoErp.Core.Helpers;
using NeoErp.Core.Models;
using System.Collections;
using NeoErp.Core.Infrastructure;
using NeoErp.Controllers.Api;
using NeoErp.Core.Services;
using NeoErp.sales.Module.App_Start;
using AutoMapper;

namespace NeoErp
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            //Clear View engines and add RazorViewEngine to avoide search for aspx engine, as it is not using in application.
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());
            ViewEngines.Engines.Add(new MyRazorViewEngine());

            MvcHandler.DisableMvcResponseHeader = true;

            EngineContext.Initialize(false);
            Mapper.Initialize(
            cfg =>
            {
                cfg.AddProfile<AutoMapperProfile>();
            }
        );
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalConfiguration.Configuration.MessageHandlers.Add(new CrossDomain());
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            GlobalConfiguration.Configuration.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);
            //  AuthConfig.RegisterAuth();

            // new HRM.Init(IntegrationHelpers.GetConInfo()).Init();

            //  ModelBinders.Binders.Add(typeof(List<Inventory.Models.Inv_IssueItemsDisplay>), new NeoErp.Models.ListModelBinder<Inventory.Models.Inv_IssueItemsDisplay>());

            //HRM.DataConnection.conString = con.ConnStr;
            //HRM.Init.Init(DataConnection.ConnectionType.SqlServer, true );
            //NeoErp.Modules.NepalDate.Init nepaldate = new NeoErp.Modules.NepalDate.Init();
            // nepaldate.Init();


            //TaskScheduler.Start();
        }

        public void Application_Error(object sender, EventArgs e)
        {
            try
            {


                Exception ex = new Exception();
                ex = Server.GetLastError().GetBaseException();

                //ignore 404 HTTP errors
                var httpException = ex as HttpException;
                if (httpException != null && httpException.GetHttpCode() == 404)
                    return;

                //   ex = Server.GetLastError();

                System.IO.FileStream fs = new System.IO.FileStream(Server.MapPath($"~/Log/errorLOG{DateTime.Now.ToString("ddMMMyyyy")}.txt"), System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                System.IO.StreamWriter s = new System.IO.StreamWriter(fs);
                s.BaseStream.Seek(0, System.IO.SeekOrigin.End);
                s.WriteLine(@"ERROR DATE: {0}
                        ERROR MESSAGE: {1}
                        SOURCE: {2}
                        FORM NAME: {3}
                        QUERYSTRING: {4}
                        TARGETSITE: {5}
                        STACKTRACE: {6}{7}",
                         System.DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture),
                         ex.Message,
                         ex.Source,
                         System.Web.HttpContext.Current.Request.Url.ToString(),
                         Request.QueryString.ToString(),
                         ex.TargetSite.ToString(),
                        ex.StackTrace,
                        System.Diagnostics.EventLogEntryType.Error);
                s.WriteLine("-------------------------------------------------------------------------------------------------------------");
                s.Close();
            }
            catch
            {

            }
        }

        protected void Session_Start()
        {
            Session.Timeout  = 60000;
            if (Session["SelectedDatabase"] == null)
            {
                Session["SelectedDatabase"] = "NeoErpCoreEntity";
            }


        }

        protected void Application_PreRequestHandlerExecute()
        {


            System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-GB");

            //Creating the DateTime Information specific to our application.
            System.Globalization.DateTimeFormatInfo dateTimeInfo = new System.Globalization.DateTimeFormatInfo();
            // Defining various date and time formats.

            dateTimeInfo.DateSeparator = "/";
            dateTimeInfo.LongDatePattern = "dd-MMM-yyyy";
            dateTimeInfo.ShortDatePattern = "dd/MM/yyyy";
            // "dd-MMM-yy"
            dateTimeInfo.LongTimePattern = "hh:mm:ss tt";
            dateTimeInfo.ShortTimePattern = "hh:mm tt";
            // Setting application wide date time format.
            cultureInfo.DateTimeFormat = dateTimeInfo;

            //Assigning our custom Culture to the application.

            //   Thread.CurrentThread.CurrentCulture = cultureInfo;
            //Thread.CurrentThread.CurrentUICulture = cultureInfo;


            //Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-GB");

            CheckSessionTimeOutStatus();
            CheckAuthStatus();

        }

        private void CheckSessionTimeOutStatus()
        {
            HttpContext ctx = HttpContext.Current;
            if (ctx.Session != null)
            {

                // check if a new session id was generated
                if (ctx.Session.IsNewSession)
                {

                    // If it says it is a new session, but an existing cookie exists, then it must
                    // have timed out
                    string sessionCookie = ctx.Request.Headers["Cookie"];
                    if ((null != sessionCookie) && (sessionCookie.IndexOf("ASP.NET_SessionId") >= 0))
                    {
                        string redirectOnSuccess = ctx.Request.Url.PathAndQuery;
                        string redirectUrl = string.Format("?ReturnUrl={0}", redirectOnSuccess);
                        string loginUrl = AppSettingsModel.loginPage; // "~/EmpAccount/Login";
                        if (redirectOnSuccess == loginUrl || redirectOnSuccess == "/") redirectUrl = "";

                        if (ctx.Request.IsAuthenticated)
                        {
                            //new AccountController().LogOff();
                            //FormsAuthentication.SignOut();
                        }
                        //RedirectResult rr = new RedirectResult(loginUrl);
                        //ctx.Result = rr;
                        Session_Start();
                        //if (redirectOnSuccess != "/SelfService/Attendance/QuickAttendance")
                        //{
                        //    ctx.Response.Redirect(loginUrl);
                        //}

                    }
                }
            }

        }

        private void CheckAuthStatus()
        {
            try
            {
                IPublicPageHandler publicpages =  null;
                publicpages= new PublicPageHandler();

                List<string> AllowedPath = new List<string>();
                //   AllowedPath.AddRange(publicpages.AllowedPath.);

                AllowedPath.Add("/Security/Account/Login");
                AllowedPath.Add("/Security/Account/ExternalLogin");
                AllowedPath.Add("/Security/Account/AdminLogin");
                AllowedPath.Add("/Security/Account/ForgotPassword");

                AllowedPath.Add("/Grievance/Lodge/Status");
                AllowedPath.Add("/Grievance/Lodge");
                AllowedPath.Add("/Grievance/Lodge/Index");
                AllowedPath.Add("/Grievance/Lodge/Create");
                AllowedPath.Add("/Grievance/Lodge/Success");
                AllowedPath.Add("/Grievance/Lodge/About");
                AllowedPath.Add("/Grievance/Lodge/Contact");
                AllowedPath.Add("/Grievance/Lodge/faqs");
                AllowedPath.Add("/Grievance/Lodge/Manual");
                AllowedPath.Add("/Grievance/Home/Language");
                AllowedPath.Add("/Main/ChangeLanguage");
                AllowedPath.Add("/Main/DbSetup");
                AllowedPath.Add("/Home/Index");

                AllowedPath = AllowedPath.Select(s => s.ToLower()).ToList();

                //string area = Request.RequestContext.RouteData.Values["area"].ToString();
                //string controller = Request.RequestContext.RouteData.Values["controller"].ToString();
                //string action = Request.RequestContext.RouteData.Values["action"].ToString();
                // string id = Request.RequestContext.RouteData.Values["id"].ToString();

                //string actualUrl = MenuManager.CreateLink(area, controller, action,"").ToLower();


                //if (Session.IsNewSession == false && Session["loginDetail"] == null && AllowedPath.Contains(actualUrl) == false && Request.Url.AbsolutePath != "/")
                //{
                //    Response.Redirect(string.Format("{0}{1}{2}", AppSettingsModel.loginPage , "?returnUrl=" , Server.UrlEncode(Request.Url.AbsolutePath)));
                //}
                //else if (Session.IsNewSession == true && AllowedPath.Contains(actualUrl) == false && Request.Url.AbsolutePath != "/")
                //{
                //    Response.Redirect(string.Format("{0}{1}{2}", AppSettingsModel.loginPage, "?returnUrl=", Server.UrlEncode(Request.Url.AbsolutePath)));
                //}
            }
            catch( Exception ex)
            {
                // Response.Redirect(IntegrationHelpers.loginPage);
            }

        }
    }
}