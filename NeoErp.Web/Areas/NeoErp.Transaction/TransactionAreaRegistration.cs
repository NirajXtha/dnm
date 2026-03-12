using System.Web.Mvc;

namespace NeoErp.Transaction
{
    public class TransactionAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "NeoErp.Transaction";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Transaction_default",
                "Transaction/{controller}/{action}/{id}",
                new { action = "Index", controller="Home", area=AreaName, id = UrlParameter.Optional },
                null,
                new string[] { "NeoErp.Transaction.Controllers" }
            );
            context.MapRoute("Transaction", "Transaction/Home/Index");
        }
    }
}