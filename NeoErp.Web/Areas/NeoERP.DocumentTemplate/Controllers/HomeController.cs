using NeoErp.Core.Controllers;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using NeoErp.Core.Models.Log4NetLoggin;
using NeoErp.Core;

namespace NeoERP.DocumentTemplate.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogErp _logErp;
        private DefaultValueForLog _defaultValueForLog;
        private IWorkContext _workContext;
        public HomeController(IWorkContext workContext)
        {
            this._workContext = workContext;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            this._logErp = new LogErp(this,_defaultValueForLog.LogUser,_defaultValueForLog.LogCompany,_defaultValueForLog.LogBranch,_defaultValueForLog.LogTypeCode,_defaultValueForLog.LogModule,_defaultValueForLog.FormCode);
        }
        public ActionResult Index()
        {
            return PartialView();
        }
        public ActionResult Dashboard()
        {
            _logErp.WarnInDB("Action Dashboard Started============");
            _logErp.InfoInFile("Action Dashboard Started============");
            return PartialView();
        }
        public ActionResult ProjectSetup()
        {
            return View();
        }
        public ActionResult DTMainDashboard()
        {
            return PartialView();
        }
        public ActionResult Draft()
        {
            return View();
        }
        public ActionResult DraftSplitter(string module)
        {
            return View();
        }
        public ActionResult ThirdPartyPreferenceSetup()
        {
            return View();
        }
        public ActionResult PreferenceSetup()
        {
            return View();
        }

        public ActionResult PriceSetupCTRL()
        {
            return View();
        }

        public ActionResult LoadingSlipGenerator()
        {
            return View();
        }

        public ActionResult LoadingSlipPrinterPage() 
        {
            return View();
        }
        public ActionResult Upload()
        {
            return PartialView("_Upload");
        }
        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            try
            {
                if (file == null || file.ContentLength == 0)
                {
                    TempData["Message"] = "Please Select File";
                    return RedirectToAction("Upload");
                }
                else
                {

                    if (file.FileName.EndsWith("xml", StringComparison.CurrentCulture))
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        string paths = Server.MapPath("~/Areas/NeoERP.DocumentTemplate/" + "XMLFile" + "/" + "Opera" + "/" + file.FileName);
                        string strMappath = "~/Areas/NeoERP.DocumentTemplate/" + "XMLFile" + "/" + "Opera" + "/";
                        string path = System.IO.Path.Combine(
                                              Server.MapPath(strMappath), file.FileName);
                        if (!Directory.Exists(strMappath))
                        {
                            Directory.CreateDirectory(Server.MapPath(strMappath));
                        }
                        if (System.IO.File.Exists(path))
                            System.IO.File.Delete(path);


                        XDocument xml = XDocument.Load(path);
                        //XDocument xml = XDocument.Load(path);
                        XDocument newXML = new XDocument(
                        new XElement("Students", xml));

                        file.SaveAs(path);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Something Went Wrong : " + ex.StackTrace;
            }
            return RedirectToAction("Upload");
        }


    }
}