using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoERP.QuotationManagement
{
    public class QuotationManagementAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "NeoErp.QuotationManagement";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "QuotationManagement_default",
                "QuotationManagement/{controller}/{action}/{id}",
                new { Controller = "Home", action = "Index", area = AreaName, id = UrlParameter.Optional },
                null,
                new string[] { "NeoErp.QuotationManagement.Controllers" }
            );

            context.MapRoute("QuotationManagement", "QuotationManagement/Home/Dashboard");
        }
    }
}