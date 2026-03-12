using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoERP.LOC
{
    public class LOCAreaRegistration : AreaRegistration
    {
        public override string AreaName => "NeoERP.LOC";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                 "LOC_default",
                 "LOC/{controller}/{action}/{id}",
                 new { Controller = "Home", action = "Index", area = AreaName, id = UrlParameter.Optional },
                 null,
                 new string[] { "NeoERP.LOC.Controllers" }
            );
            context.MapRoute("Loc", "Loc/Home/Index");
        }
    }
}