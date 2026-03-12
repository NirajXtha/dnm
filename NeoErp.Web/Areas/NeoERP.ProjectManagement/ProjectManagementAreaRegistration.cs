using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoERP.ProjectManagement
{
    public class ProjectManagementAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "NeoErp.ProjectManagement";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ProjectManagement_default",
                "ProjectManagement/{controller}/{action}/{id}",
                new { Controller = "Home", action = "Index", area = AreaName, id = UrlParameter.Optional },
                null,
                new string[] { "NeoErp.ProjectManagement.Controllers" }
            );

            context.MapRoute("ProjectManagement", "ProjectManagement/Home/ProjectDashboard");
        }
    }
}