using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoErp.Pos
{
    public class PosAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "NeoErp.Pos";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Pos_default",
                "Pos/{controller}/{action}/{id}",
                new { Controller = "Home", action = "Index", area = AreaName, id = UrlParameter.Optional },
                null,
                new string[] { "NeoErp.Pos.Controllers" }
            );
         //   context.MapHttpRoute(
         //    name: "PosDefaultActionApi",
         //    routeTemplate: "Pos/api/{controller}/{action}/{id}",
            
         //);
            context.MapRoute("Pos", "Pos/Home/Index");
        }
    }
}