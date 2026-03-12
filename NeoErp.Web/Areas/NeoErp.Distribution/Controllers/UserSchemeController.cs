using System.Web.Mvc;

namespace NeoErp.Distribution.Controllers
{
    public class UserSchemeController : Controller
    {

        public UserSchemeController()
        {

        }
        #region
        // GET: User Scheme
        public ActionResult SchemeUser()
        {
            return View();
        }
        public ActionResult QrGenerator()
        {
            return View();
        }

        public ActionResult PointsRedeem()
        {
            return View();
        }

        public ActionResult UpdateQr()
        {
            return View();
        }

        #endregion



    }
}