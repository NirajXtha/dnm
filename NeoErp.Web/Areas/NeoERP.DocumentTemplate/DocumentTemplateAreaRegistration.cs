using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoERP.DocumentTemplate
{
    public class DocumentTemplateAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                //return "DocumentTemplate";
                return "NeoErp.DocumentTemplate";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            //context.MapRoute(
            //    "DocumentTemplate_default",
            //    "DocumentTemplate/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional }
            //);
            context.MapRoute(
                "DocumentTemplate_default",
                "DocumentTemplate/{controller}/{action}/{id}",
                new { action = "Dashboard", controller = "Home", area = AreaName, id = UrlParameter.Optional },
                null,
                new string[] { "NeoErp.DocumentTemplate.Controllers" }
            );
            context.MapRoute("DocumentTemplate", "DocumentTemplate/Home/Dashboard");
        }
    }

}