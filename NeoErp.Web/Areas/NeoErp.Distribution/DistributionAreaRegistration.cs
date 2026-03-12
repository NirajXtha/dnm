using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoErp.Distribution
{
    public class DistributionAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "NeoErp.Distribution";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Distribution_default",
                "Distribution/{controller}/{action}/{id}",
                new { Controller = "Home", action = "Index", area = AreaName, id = UrlParameter.Optional },
                null,
                new string[] { "NeoErp.Distribution.Controllers" }
            );

            context.MapRoute("Distribution", "Distribution/Home/dashboardlayout");
        }
    }
}