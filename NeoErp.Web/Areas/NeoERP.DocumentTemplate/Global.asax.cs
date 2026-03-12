using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace NeoERP.DocumentTemplate
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            //GlobalConfiguration.Configure(WebApiConfig.Register);
            //FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //RouteConfig.RegisterRoutes(RouteTable.Routes);
            //BundleConfig.RegisterBundles(BundleTable.Bundles);
           // HttpContext.Current.Response.Headers.Add(
           //    "Content-Security-Policy",
           //    "default-src 'self'; script-src 'self' 'unsafe-inline'; child-src 'self' blob: data:; form-action 'self';"
           //);
        }
        //protected void Application_BeginRequest()
        //{
        //    HttpContext.Current.Response.Headers.Add(
        //        "Content-Security-Policy",
        //        "default-src 'self'; script-src 'self' 'unsafe-inline'; child-src 'self' blob: data:; form-action 'self';"
        //    );
        //}
    }
}
