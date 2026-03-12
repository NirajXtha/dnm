using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NeoErp.LOC.Services.Models;
using NeoErp.LOC.Services.Services;

namespace NeoERP.LOC.Controllers
{
    public class SettlementInvoiceController : Controller
    {
        private ISettlementInvoiceService _settlementInvoiceService { get; set; }
        public SettlementInvoiceController(ISettlementInvoiceService settlementInvoiceService)
        {
            this._settlementInvoiceService = settlementInvoiceService;
        }
        // GET: SettlementInvoice
        public ActionResult Index()
        {
            return PartialView();
        }
        [HttpPost]
        public JsonResult FileUpload(IEnumerable<HttpPostedFileBase> inputFiles, FormCollection collection)
        {
            var value = Convert.ToString(collection["SETTLEMENT_CODE"]);
            //var lcFile = Convert.ToString(collection["locEntryForm"]);
            var sendpath = "";
            if (inputFiles != null)
            {
                foreach (var fileName in inputFiles)
                {
                    var file = fileName;
                    //Save file content goes here
                    if (file.ContentLength <= 0) continue;
                    var picture = System.IO.Path.GetFileName(file.FileName);
                    const string underscore = "_";
                    if (picture == null) continue;
                    var pic = $"{value}{underscore}{Guid.NewGuid()}{underscore}" + picture.PadLeft(0, '0');
                    const string lcFile = "LOCSettlementFile";
                    var strMappath = "~/Pictures/" + lcFile + "/" + value + "/";
                    var path = System.IO.Path.Combine(
                        Server.MapPath(strMappath), pic);
                    var ext = Path.GetExtension(path);

                    var dbstrMappath = "\\Pictures\\" + lcFile + "\\" + value + "\\" + pic;
                    if (!Directory.Exists(strMappath))
                    {
                        Directory.CreateDirectory(Server.MapPath(strMappath));
                    }

                    sendpath = dbstrMappath + ':' + sendpath;

                    file.SaveAs(path);
                }
                var finalpath = sendpath.Remove(sendpath.Length - 1);
                var lcmodels = new LcUploadFileModels
                {
                    Path = finalpath,
                    Code = Convert.ToInt32(value)
                };
                _settlementInvoiceService.UpdateFile(lcmodels);

                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }

        }
    }
}