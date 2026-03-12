using System.Web.Mvc;

namespace NeoErp.Areas.NeoERPPlanning
{
    public class PlanningModuleAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "NeoErp.Planning";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Planning_default",
                "Planning/{controller}/{action}/{id}",
                //new { action = "Index", id = UrlParameter.Optional }
                new { action = "Index", controller = "Home", area = AreaName, id = UrlParameter.Optional },
                null,
                new string[] { "NeoErp.Planning.Controllers" }
            );
            context.MapRoute("Planning", "Planning/Home/PlanningDashboard");
        }
    }
}