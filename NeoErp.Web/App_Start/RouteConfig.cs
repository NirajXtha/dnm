using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;

namespace NeoErp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
            routes.MapHttpRoute(
                name: "DefaultActionApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );
            routes.MapRoute(
              name: "Default",
              url: "{controller}/{action}/{id}",
              defaults: new { controller = "Home", action = "LogIn", id = UrlParameter.Optional },
              namespaces: new string[]{ "NeoErp.Controllers" }
              //defaults: new { controller = "EmpAccount", action = "AdminLogin", id = UrlParameter.Optional }
          );
          
            //Add Prem Prakash Dhakal for Dashboard
            //routes.MapRoute(
            //    name: "PluginRoute",
            //    url: "plugin/{name}",
            //    defaults: new { controller = "Plugin", action = "Index", name = UrlParameter.Optional }
            //);
        }
    }
}