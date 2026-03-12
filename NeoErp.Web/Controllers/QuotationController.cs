using System;
using System.Collections.Generic;
using System.Web.Mvc;
using NeoErp.Models;
using NeoErp.Models.Common;
using NeoErp.Services.LogService;
using NeoErp.Core;
using NeoErp.Services;
using NeoErp.Core.Services;
using NeoErp.Core.Domain;
using NeoErp.Core.Caching;
using NeoErp.Core.Plugins;
using System.Xml.Linq;
using System.Xml;
using System.Configuration;
using NeoErp.Data;
using NeoErp.Models.QueryBuilder;
using NeoErp.Core.Services.CommonSetting;
using NeoErp.Core.Models.Log4NetLoggin;
using NeoErp.Core.Quotation;
using NeoErp.Core.Models;
using System.Web;
using System.IO;

namespace NeoErp.Controllers
{
    public class QuotationController : Controller
    {
        private NeoErpCoreEntity _coreEntity;
        private IQuotation _quoTemplate;

        public QuotationController(NeoErpCoreEntity coreEntity, IQuotation quotationTemplate)
        {
            _coreEntity = coreEntity;
            _quoTemplate = quotationTemplate;
        }
        public ActionResult Index(string qo)
        {
            return View();
        }
        public ActionResult Message()
        {
            return View();

        }
        [HttpPost]
        public JsonResult QuotationFileUpload(IEnumerable<HttpPostedFileBase> inputFiles, FormCollection collection)
        {
            var tenderNos = Convert.ToString(collection["TenderNo"]);
            var quotationNos = Convert.ToString(collection["QuotationNo"]);
            var companyCode = Convert.ToString(collection["CompanyCode"]);
            var branchCode = Convert.ToString(collection["BranchCode"]);
            var tenderNo = tenderNos.Split(',')[0];
            var quotationNo = quotationNos.Split(',')[0];
            int serialno = 1;
            int? serialnumber = _quoTemplate.DocumentIfExists(tenderNo, quotationNo, companyCode);
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
                        var replacedvoucherno = tenderNo.Replace("/", "_");
                        string picture = System.IO.Path.GetFileName(file.FileName);
                        string SalesModule = "QuotationModule";
                        string strMappath = "~/Pictures/" + SalesModule + "/" + quotationNo + "/" + replacedvoucherno + "/";
                        string path = System.IO.Path.Combine(
                                               Server.MapPath(strMappath), picture);
                        string ext = Path.GetExtension(path);
                        dbstrMappath = "\\Pictures\\" + SalesModule + "\\" + quotationNo + "\\" + tenderNo + "\\" + picture;
                        if (!Directory.Exists(strMappath))
                        {
                            Directory.CreateDirectory(Server.MapPath(strMappath));
                        }
                        file.SaveAs(path);
                        QuotationTranscation quotationdetail = new QuotationTranscation();
                        quotationdetail.QUOTATION_NO = quotationNo;
                        quotationdetail.TENDER_NO = tenderNo;
                        quotationdetail.DOCUMENT_FILE_NAME = file.FileName;
                        quotationdetail.DOCUMENT_NAME = dbstrMappath;
                        quotationdetail.SERIAL_NO = serialno;
                        quotationdetail.COMPANY_CODE = companyCode;
                        quotationdetail.BRANCH_CODE = branchCode;
                        _quoTemplate.InsertQuotationImage(quotationdetail);
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