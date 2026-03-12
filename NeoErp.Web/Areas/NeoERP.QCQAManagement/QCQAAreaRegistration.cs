using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoERP.QCQAManagement
{
    public class QCQAAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "NeoErp.QCQAManagement";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "QCQAManagement_default",
                "QCQAManagement/{controller}/{action}/{id}",
                new { Controller = "Home", action = "Index", area = AreaName, id = UrlParameter.Optional },
                null,
                new string[] { "NeoErp.QCQAManagement.Controllers" }
            );

            context.MapRoute("QCQAManagement", "QCQAManagement/Home/Dashboard");
        }
    }
}