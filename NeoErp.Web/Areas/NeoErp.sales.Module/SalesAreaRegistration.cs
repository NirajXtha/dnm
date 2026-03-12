using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoErp.sales.Module
{
    public class SalesAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "NeoErp.sales.Module";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Sales_default",
                "Sales/{controller}/{action}/{id}",
                new { Controller = "SalesHome", action = "Index", area = AreaName, id = UrlParameter.Optional },
                null,
                new string[] { "NeoErp.sales.Module.Controllers" }
            );

            context.MapRoute("Sales","Sales/SalesHome/Dashboard");
        }
    }
}