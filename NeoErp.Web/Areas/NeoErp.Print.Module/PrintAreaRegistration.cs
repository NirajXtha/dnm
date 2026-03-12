using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoErp.Print.Module
{
    public class PrintAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "NeoErp.Print.Module";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Print_default",
                "Print/{controller}/{action}/{id}",
                new { Controller = "PrintHome", action = "Index", area = AreaName, id = UrlParameter.Optional },
                null,
                new string[] { "NeoErp.Print.Module.Controllers" }
            );

            context.MapRoute("Print", "Print/PrintHome/Dashboard");
        }
    }
}