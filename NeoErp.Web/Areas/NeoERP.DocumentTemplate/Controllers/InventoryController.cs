using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NeoErp.Core.Models;
using System.IO;
using NeoERP.DocumentTemplate.Service.Models;
using NeoERP.DocumentTemplate.Service.Interface;

namespace NeoERP.DocumentTemplate.Controllers
{
    public class InventoryController : Controller
    {
        private NeoErpCoreEntity _coreEntity;
        private IInventoryVoucher _inventoryVoucher;
        private IFormTemplateRepo _formTemplate;
        public InventoryController(NeoErpCoreEntity coreEntity, IInventoryVoucher inventoryVoucher,IFormTemplateRepo formTemplate)
        {
            _coreEntity = coreEntity;
            _inventoryVoucher = inventoryVoucher;
            _formTemplate = formTemplate;
        }
        [HttpGet]
        public ActionResult Inventory()
        {

            return PartialView();
        }

        [HttpPost]
        public JsonResult InventoryFileUpload(IEnumerable<HttpPostedFileBase> inputFiles, FormCollection collection)
        {
            var vouchernos = Convert.ToString(collection["VoucherNo"]);
            var voucherdates = Convert.ToString(collection["VoucherDate"]);
            var formcodes = Convert.ToString(collection["FormCode"]);
            var voucherno = vouchernos.Split(',')[0];
            var voucherdate = voucherdates.Split(',')[0]== "Invalid date" ? DateTime.Now.ToShortDateString() : voucherdates.Split(',')[0];
            var formcode = formcodes.Split(',')[0];
            int serialno = 1;
            int? serialnumber = _formTemplate.DocumentIfExists(voucherno, formcode);
            if (serialnumber != null)
            {
                serialno = Convert.ToInt32(serialnumber) + 1;
            }
            if (inputFiles != null)
            {
                foreach (HttpPostedFileBase fileName in inputFiles)
                {
                    string dbstrMappath = "";
                    HttpPostedFileBase file = fileName;
                    if (file != null && file.ContentLength > 0)
                    {
                        var replacedvoucherno = voucherno.Replace("/", "_");
                        string picture = System.IO.Path.GetFileName(file.FileName);
                        string SalesModule = "InventoryModule";
                        string strMappath = "~/Pictures/" + SalesModule + "/" + formcode + "/" + replacedvoucherno + "/";
                        string path = System.IO.Path.Combine(
                                               Server.MapPath(strMappath), picture);
                        string ext = Path.GetExtension(path);
                        dbstrMappath = "\\Pictures\\" + SalesModule + "\\" + formcode + "\\" + voucherno + "\\" + picture;
                        if (!Directory.Exists(strMappath))
                        {
                            Directory.CreateDirectory(Server.MapPath(strMappath));
                        }
                        file.SaveAs(path);
                        DocumentTransaction documentdetail = new DocumentTransaction();
                        documentdetail.FORM_CODE = formcode;
                        documentdetail.VOUCHER_DATE = Convert.ToDateTime(voucherdate);
                        documentdetail.VOUCHER_NO = voucherno;
                        documentdetail.DOCUMENT_FILE_NAME = file.FileName;
                        documentdetail.DOCUMENT_NAME = dbstrMappath;
                        documentdetail.SERIAL_NO = serialno;
                        _inventoryVoucher.InsertInventoryImage(documentdetail);
                        serialno++;
                    }
                }


                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

    }
}