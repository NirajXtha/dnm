using NeoERP.ProjectManagement.Service.Interface;
using NeoERP.ProjectManagement.Service.Models;
using System;
using NeoERP.DocumentTemplate.Service.Interface;
using NeoERP.DocumentTemplate.Service.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using NeoErp.Core.Models.Log4NetLoggin;
using NeoErp.Core;
//using Excel = Microsoft.Office.Interop.Excel;
using System.Data;
using System.Xml.Linq;
using LinqToExcel;

namespace NeoERP.ProjectManagement.Controllers
{
    public class TemplateController : Controller
    {
        // GET: Template
        private IFormTemplateRepo _formtemplaterepo;
        private IFormSetupRepo _FormSetupRepo;
        private readonly ILogErp _logErp;
        private IWorkContext _workContext;
        private DefaultValueForLog _defaultValueForLog;
        public TemplateController(IFormTemplateRepo formtemplaterepo, IFormSetupRepo FormSetupRepo, IWorkContext workContext)
        {
            _formtemplaterepo = formtemplaterepo;
            _FormSetupRepo = FormSetupRepo;

            this._workContext = workContext;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            //var codeL = _formtemplaterepo.CodeForLog();
            this._logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogModule, _defaultValueForLog.LogModule, _defaultValueForLog.FormCode);
        }
        public ActionResult Index()
        {
            return PartialView();
        }
        public ActionResult Dashboard()
        {
            return PartialView();
        }
        [HttpGet]
        public ActionResult Formtemplate(string formCode)
        {
            return PartialView();
        }
        public ActionResult Setup()
        {
            return PartialView("_SetupPartial");
        }
        public ActionResult SalesOrderDetail()
        {
            return PartialView("SalesOrderDetail");
        }
        [HttpGet]
        public ActionResult FormSetuptemplate(string formCode)
        {
            return PartialView();
        }
        [HttpGet]
        public ActionResult Formtemplates(string formCode, string voucherno)
        {
            return PartialView("Formtemplate");
        }
        //[HttpGet]
        //public ActionResult DocPrinttemplate(string formCode, string orderNo)
        //{
        //    return PartialView("DocPrinttemplate");
        //}

        [HttpGet]
        public ActionResult Formtemplateprint(string formCode, string orderNo, string printstatus)
        {
            return PartialView("Formtemplate");
        }
        //[HttpGet]
        //public ActionResult DraftFormtemplates(string formCode, string tempCode)
        //{
        //    return PartialView("Formtemplate");
        //}
        public ActionResult SplitterIndex(string module)
        {
            return View();
        }
        public ActionResult MenuSplitter(string module)
        {
            return View();
        }
        public ActionResult ReqMenuSplitter(string moduleCode)
        {
            // Your code here
            return View();
        }
        public ActionResult PurMenuSplitter(string moduleCode)
        {
            // Your code here
            return View();
        }
        public ActionResult AuxMenuSplitter(string moduleCode)
        {
            // Your code here
            return View();
        }
        public ActionResult VerifySplitter(string module)
        {
            return View();
        }
        [HttpPost]
        public JsonResult SalesFileUpload(IEnumerable<HttpPostedFileBase> inputFiles, FormCollection collection)
        {
            var vouchernos = Convert.ToString(collection["VoucherNo"]);
            var voucherdates = Convert.ToString(collection["VoucherDate"]);
            var formcodes = Convert.ToString(collection["FormCode"]);
            var voucherno = vouchernos.Split(',')[0];
            var voucherdate = voucherdates.Split(',')[0];
            var formcode = formcodes.Split(',')[0];
            int serialno = 1;
            int? serialnumber = _formtemplaterepo.DocumentIfExists(voucherno, formcode);
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
                        string SalesModule = "SalesModule";
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
                        _formtemplaterepo.InsertSalesImage(documentdetail);
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
        [HttpGet]
        public ActionResult GetFolderTemplateByUserId()
        {
            List<WebDesktopManagement> Record = new List<WebDesktopManagement>();
            Record = this._formtemplaterepo.GetFolderTemplateByUserId();
            return PartialView("~/Views/Shared/GetFolderTemplateByUserId.cshtml", Record);
        }
        //[HttpGet]
        //public ActionResult FolderTemplate()
        //{
        //    WebDesktop Record = new WebDesktop();
        //    Record.WebDesktopFolder = new List<WebDesktopFolder>();
        //    Record.WebDesktopManagement = new List<WebDesktopManagement>();
        //    Record.WebDesktopFolder= this._formtemplaterepo.GetFoldertByUserId();
        //    Record.WebDesktopManagement = this._formtemplaterepo.GetFolderTemplateByUserId();
        //    return PartialView("_FolderTemplate", Record);
        //}



        //dynamic desktop management
        [HttpGet]
        public PartialViewResult FolderTemplate(WebDesktopManagement webDesktopManagement)
        {
            _logErp.WarnInDB("Folder Template Started=====");
            if (!string.IsNullOrEmpty(webDesktopManagement.RESET))
            {
                this._formtemplaterepo.ResetWebManagement();
            }

            if (webDesktopManagement.ICON_PATH != null)
            {
                webDesktopManagement.FORM_CODE = webDesktopManagement.FORM_CODE == null ? "" : webDesktopManagement.FORM_CODE.Trim();
                webDesktopManagement.HREF = webDesktopManagement.HREF == null ? "" : webDesktopManagement.HREF.Trim();
                webDesktopManagement.COLOR = webDesktopManagement.COLOR == null ? "" : webDesktopManagement.COLOR.Trim();
                webDesktopManagement.ICON_PATH = webDesktopManagement.ICON_PATH == null ? "" : webDesktopManagement.ICON_PATH.Trim();
                webDesktopManagement.FORM_TYPE = webDesktopManagement.FORM_TYPE == null ? "" : webDesktopManagement.FORM_TYPE.Trim();
                webDesktopManagement.ABBR = webDesktopManagement.ABBR == null ? "" : webDesktopManagement.ABBR.Trim();
                webDesktopManagement.PREV_FOLDER_NAME = webDesktopManagement.PREV_FOLDER_NAME == null ? "" : webDesktopManagement.PREV_FOLDER_NAME.Trim();
                webDesktopManagement.NEW_FOLDER_NAME = webDesktopManagement.NEW_FOLDER_NAME == null ? "" : webDesktopManagement.NEW_FOLDER_NAME.Trim();
                webDesktopManagement.FUNCTION_LINK = webDesktopManagement.FUNCTION_LINK == null ? "" : webDesktopManagement.FUNCTION_LINK.Trim();
                webDesktopManagement.TEMPLATE_CODE = webDesktopManagement.TEMPLATE_CODE == null ? "" : webDesktopManagement.TEMPLATE_CODE.Trim();
                webDesktopManagement.MENU_EDESC = webDesktopManagement.MENU_EDESC == null ? "" : webDesktopManagement.MENU_EDESC.Trim();
                webDesktopManagement.MENU_DESC = webDesktopManagement.MENU_DESC == null ? "" : webDesktopManagement.MENU_DESC.Trim();
                webDesktopManagement.MENU_NO = webDesktopManagement.MENU_NO == null ? "" : webDesktopManagement.MENU_NO.Trim();
                webDesktopManagement.UNIQUE_ID = webDesktopManagement.UNIQUE_ID.Trim();

                var a = this._formtemplaterepo.AddWebDesktopManagement(webDesktopManagement);
            }
            WebDesktop Record = new WebDesktop();
            Record.WebDesktopFolder = new List<WebDesktopFolder>();
            Record.WebDesktopManagement = new List<WebDesktopManagement>();




            #region INSERT SALES ICON
            var Sales_Record = this._formtemplaterepo.GetAllSalesMenuItems();
            WebDesktopManagement SalesData = new WebDesktopManagement();

            foreach (var icon in Sales_Record)
            {
                SalesData.FORM_CODE = icon.FORM_CODE;
                SalesData.FORM_EDESC = icon.FORM_EDESC;
                SalesData.MODULE_CODE = icon.MODULE_CODE;
                SalesData.HREF = "/ProjectManagement/Home/Index#!DT/formtemplate/" + icon.FORM_CODE;
                SalesData.ABBR = icon.MODULE_ABBR;
                SalesData.COLOR = "#1F9400";
                SalesData.ICON_PATH = "fa fa-calculator";
                SalesData.PREV_FOLDER_NAME = "Document Template(sales)";
                SalesData.NEW_FOLDER_NAME = "Document Template(sales)";
                SalesData.UNIQUE_ID = UniqueId();
                //exist or not

                bool ExistFolder = this._formtemplaterepo.FolderExistsOrNot(SalesData.NEW_FOLDER_NAME);
                if (!ExistFolder)
                {
                    this._formtemplaterepo.AddNewFolder(SalesData.NEW_FOLDER_NAME, "1f9400", "icon-home fontgreen");
                }

                if (!string.IsNullOrEmpty(SalesData.FORM_CODE))
                {
                    bool Exist = this._formtemplaterepo.FormNoExistsOrNot(SalesData.FORM_CODE);
                    if (!Exist)
                    {
                        this._formtemplaterepo.AddWebDesktopManagement(SalesData);

                    }
                }


            }

            #endregion

            #region INSERT FINANCE ICON
            var Finance_Record = this._formtemplaterepo.GetAllMenuItems();
            WebDesktopManagement FianaceData = new WebDesktopManagement();
            foreach (var icon in Finance_Record)
            {
                FianaceData.FORM_CODE = icon.FORM_CODE;
                FianaceData.FORM_EDESC = icon.FORM_EDESC;
                FianaceData.MODULE_CODE = icon.MODULE_CODE;
                FianaceData.HREF = "/ProjectManagement/Home/Index#!DT/FinanceVoucher?formcode=" + icon.FORM_CODE;
                FianaceData.ABBR = icon.MODULE_ABBR;
                FianaceData.COLOR = "#730032";
                FianaceData.ICON_PATH = "fa fa-book";
                FianaceData.PREV_FOLDER_NAME = "Document Template(Finance)";
                FianaceData.NEW_FOLDER_NAME = "Document Template(Finance)";
                FianaceData.UNIQUE_ID = UniqueId();
                //exist or not
                bool ExistFolder = this._formtemplaterepo.FolderExistsOrNot(FianaceData.NEW_FOLDER_NAME);
                if (!ExistFolder)
                {
                    this._formtemplaterepo.AddNewFolder(FianaceData.NEW_FOLDER_NAME, "d6213c", "icon-home fontgreen");
                }

                if (!string.IsNullOrEmpty(FianaceData.FORM_CODE))
                {
                    bool Exist = this._formtemplaterepo.FormNoExistsOrNot(FianaceData.FORM_CODE);
                    if (!Exist)
                    {
                        this._formtemplaterepo.AddWebDesktopManagement(FianaceData);
                    }
                }


            }

            #endregion

            #region INSERT INVENTROY ICON
            var Inventory_Record = this._formtemplaterepo.GetAllInventoryMenuItems();
            WebDesktopManagement InventoryData = new WebDesktopManagement();
            foreach (var icon in Inventory_Record)
            {
                InventoryData.FORM_CODE = icon.FORM_CODE;
                InventoryData.FORM_EDESC = icon.FORM_EDESC;
                InventoryData.MODULE_CODE = icon.MODULE_CODE;
                InventoryData.HREF = "/ProjectManagement/Home/Index#!DT/Inventory?formcode=" + icon.FORM_CODE;
                InventoryData.ABBR = icon.MODULE_ABBR;
                InventoryData.COLOR = "#0E5293";
                InventoryData.ICON_PATH = "fa fa-cart-arrow-down";
                InventoryData.PREV_FOLDER_NAME = "Document Template(Inventory)";
                InventoryData.NEW_FOLDER_NAME = "Document Template(Inventory)";
                InventoryData.UNIQUE_ID = UniqueId();
                //exist or not
                bool ExistFolder = this._formtemplaterepo.FolderExistsOrNot(InventoryData.NEW_FOLDER_NAME);
                if (!ExistFolder)
                {
                    this._formtemplaterepo.AddNewFolder(InventoryData.NEW_FOLDER_NAME, "0e5293", "icon-home fontgreen");
                }

                if (!string.IsNullOrEmpty(InventoryData.FORM_CODE))
                {
                    bool Exist = this._formtemplaterepo.FormNoExistsOrNot(InventoryData.FORM_CODE);
                    if (!Exist)
                    {
                        this._formtemplaterepo.AddWebDesktopManagement(InventoryData);

                    }
                }


            }

            #endregion

            #region INSERT SETUP ICON
            var Setup_Record = this._formtemplaterepo.GetAllSetupMenuItems();

            WebDesktopManagement Data = new WebDesktopManagement();
            foreach (var icon in Setup_Record)
            {
                Data.MENU_NO = icon.MENU_NO;
                Data.MENU_DESC = icon.MENU_DESC;
                Data.MENU_EDESC = icon.MENU_EDESC;
                Data.MODULE_CODE = icon.MODULE_CODE;
                Data.HREF = icon.FULL_PATH;
                Data.ABBR = icon.MODULE_ABBR;
                Data.COLOR = icon.COLOR;
                Data.ICON_PATH = icon.ICON_PATH;
                Data.PREV_FOLDER_NAME = "Master";
                Data.NEW_FOLDER_NAME = "Master";
                Data.UNIQUE_ID = UniqueId();
                //exist or not
                bool ExistFolder = this._formtemplaterepo.FolderExistsOrNot("Master");
                if (!ExistFolder)
                {
                    this._formtemplaterepo.AddNewFolder("Master", "a7a1a1", "icon-home fontgreen");
                }


                if (!string.IsNullOrEmpty(Data.MENU_NO))
                {
                    bool Exist = this._formtemplaterepo.MenuNoExistsOrNot(Data.MENU_NO);
                    if (!Exist)
                    {
                        this._formtemplaterepo.AddWebDesktopManagement(Data);

                    }
                }


            }

            #endregion

            #region INSERT DRAFT ICON
            var Draft_Record = this._FormSetupRepo.GetAllMenuInventoryAssigneeDraftTemplateList();
            WebDesktopManagement DraftData = new WebDesktopManagement();
            foreach (var icon in Draft_Record)
            {


                if (icon.MODULE_CODE == "01")
                {
                    DraftData.FUNCTION_LINK = "GetFVDTData(" + icon.FORM_CODE + "," + icon.TEMPLATE_CODE + ")";
                }
                if (icon.MODULE_CODE == "02")
                {
                    DraftData.FUNCTION_LINK = "GetInvDTData(" + icon.FORM_CODE + "," + icon.TEMPLATE_CODE + ")";
                }
                if (icon.MODULE_CODE == "04")
                {
                    DraftData.FUNCTION_LINK = "GetSalesDTData(" + icon.FORM_CODE + "," + icon.TEMPLATE_CODE + ")";
                }
                DraftData.TEMPLATE_CODE = icon.TEMPLATE_CODE;
                DraftData.FORM_CODE = icon.FORM_CODE;
                DraftData.FORM_EDESC = icon.FORM_EDESC;
                DraftData.MODULE_CODE = icon.MODULE_CODE;
                DraftData.ABBR = icon.FORM_TYPE;
                DraftData.COLOR = "#269BAC";
                DraftData.ICON_PATH = "fa fa-sticky-note";
                DraftData.PREV_FOLDER_NAME = "Draft Template (TIP)";
                DraftData.NEW_FOLDER_NAME = "Draft Template (TIP)";
                DraftData.UNIQUE_ID = UniqueId();
                //exist or not
                bool ExistFolder = this._formtemplaterepo.FolderExistsOrNot(DraftData.NEW_FOLDER_NAME);
                if (!ExistFolder)
                {
                    this._formtemplaterepo.AddNewFolder(DraftData.NEW_FOLDER_NAME, "79559F", "icon-home fontgreen");
                }
                if (!string.IsNullOrEmpty(DraftData.TEMPLATE_CODE))
                {
                    bool Exist = this._formtemplaterepo.DraftTemplateExistsOrNot(DraftData.TEMPLATE_CODE);
                    if (!Exist)
                    {
                        this._formtemplaterepo.AddWebDesktopManagement(DraftData);

                    }
                }


            }

            #endregion

            #region INSERT SAVED DRAFT ICON
            var Saved_Draft_Record = this._FormSetupRepo.GetAllMenuInventoryAssigneeSavedDraftTemplateList();
            WebDesktopManagement SavedDraftData = new WebDesktopManagement();
            foreach (var icon in Saved_Draft_Record)
            {

                SavedDraftData.FUNCTION_LINK = "GetDTData(" + icon.FORM_CODE + "," + icon.TEMPLATE_CODE + ")";
                SavedDraftData.TEMPLATE_CODE = icon.TEMPLATE_CODE;
                SavedDraftData.FORM_CODE = icon.FORM_CODE;
                SavedDraftData.FORM_EDESC = icon.FORM_EDESC;
                SavedDraftData.MODULE_CODE = icon.MODULE_CODE;
                SavedDraftData.ABBR = icon.FORM_TYPE;
                SavedDraftData.COLOR = "#3623AF";
                SavedDraftData.ICON_PATH = "fa fa-sticky-note-o";
                SavedDraftData.PREV_FOLDER_NAME = "Saved Draft Template";
                SavedDraftData.NEW_FOLDER_NAME = "Saved Draft Template";
                SavedDraftData.UNIQUE_ID = UniqueId();
                //exist or not
                bool ExistFolder = this._formtemplaterepo.FolderExistsOrNot(SavedDraftData.NEW_FOLDER_NAME);
                if (!ExistFolder)
                {
                    this._formtemplaterepo.AddNewFolder(SavedDraftData.NEW_FOLDER_NAME, "3623af", "icon-home fontgreen");
                }

                if (!string.IsNullOrEmpty(SavedDraftData.TEMPLATE_CODE))
                {
                    bool Exist = this._formtemplaterepo.DraftTemplateExistsOrNot(SavedDraftData.TEMPLATE_CODE);
                    if (!Exist)
                    {
                        this._formtemplaterepo.AddWebDesktopManagement(SavedDraftData);
                    }
                }


            }

            #endregion

            #region PRODUCTION MGMG

            var productionRecords = this._formtemplaterepo.GetAllProductionMeneItems();
            WebDesktopManagement productionData = new WebDesktopManagement();
            foreach (var icon in productionRecords)
            {
                productionData.FORM_CODE = icon.FORM_CODE;
                productionData.FORM_EDESC = icon.FORM_EDESC;
                productionData.MODULE_CODE = icon.MODULE_CODE;
                productionData.HREF = "/ProjectManagement/Home/Index#!DT/Inventory?formcode=" + icon.FORM_CODE;
                productionData.ABBR = icon.MODULE_ABBR;
                productionData.COLOR = "#F57600";
                productionData.ICON_PATH = "fa fa-cart-arrow-down";
                productionData.PREV_FOLDER_NAME = "Document Template(Production)";
                productionData.NEW_FOLDER_NAME = "Document Template(Production)";
                productionData.UNIQUE_ID = UniqueId();
                //exist or not
                bool ExistFolder = this._formtemplaterepo.FolderExistsOrNot(productionData.NEW_FOLDER_NAME);
                if (!ExistFolder)
                {
                    this._formtemplaterepo.AddNewFolder(productionData.NEW_FOLDER_NAME, "f57600", "icon-home fontgreen");
                }

                if (!string.IsNullOrEmpty(productionData.FORM_CODE))
                {
                    bool Exist = this._formtemplaterepo.FormNoExistsOrNot(productionData.FORM_CODE);
                    if (!Exist)
                    {
                        this._formtemplaterepo.AddWebDesktopManagement(productionData);

                    }
                }


            }


            #endregion


            Record.WebDesktopFolder = this._formtemplaterepo.GetFoldertByUserId();
            _logErp.InfoInFile(Record.WebDesktopFolder.Count() + " folder fetch by user id ");
            _logErp.WarnInDB(Record.WebDesktopFolder.Count() + " folder fetch by user id ");
            Record.WebDesktopManagement = this._formtemplaterepo.GetFolderTemplateByUserId();
            _logErp.InfoInFile(Record.WebDesktopManagement.Count() + " folder template fetch by user id ");
            _logErp.WarnInDB(Record.WebDesktopManagement.Count() + " folder template fetch by user id ");
            foreach (var wd in Record.WebDesktopManagement)
            {
                var form_control_record = this._FormSetupRepo.GetFormControls(wd.FORM_CODE).ToList().FirstOrDefault();
                if (form_control_record != null)
                {
                    wd.CREATE_FLAG = form_control_record.CREATE_FLAG == null ? "" : form_control_record.CREATE_FLAG;
                    wd.UPDATE_FLAG = form_control_record.UPDATE_FLAG == null ? "" : form_control_record.UPDATE_FLAG;
                }

            }
            //ViewData["WebDesktopFolder"] = Record.WebDesktopFolder;
            //ViewData["WebDesktopManagement"] = Record.WebDesktopManagement;
            ViewBag.Record = Record;
            _logErp.InfoInFile("Avaiable record for document template contain : " + Record);
            _logErp.WarnInDB("Avaiable record for document template contain : " + Record);
            return PartialView("~/Areas/NeoErp.ProjectManagement/Views/Template/_FolderTemplate.cshtml");
        }
        [HttpPost]
        public ActionResult OrderNumberTemplate(List<FOLDER_ORDER> fOLDER_ORDER)
        {
            if (fOLDER_ORDER != null)
            {
                this._formtemplaterepo.UpdateOrderNoByFolderId(fOLDER_ORDER);
            }
            return RedirectToAction("FolderTemplate");
        }
        [HttpPost]
        public ActionResult AddMenuFolderTemplate(WebDesktopManagement webDesktopManagement)
        {
            if (webDesktopManagement.ICON_PATH != null)
            {
                webDesktopManagement.FORM_CODE = webDesktopManagement.FORM_CODE == null ? "" : webDesktopManagement.FORM_CODE.Trim();
                webDesktopManagement.HREF = webDesktopManagement.HREF == null ? "" : webDesktopManagement.HREF.Trim();
                webDesktopManagement.COLOR = webDesktopManagement.COLOR == null ? "" : webDesktopManagement.COLOR.Trim();
                webDesktopManagement.ICON_PATH = webDesktopManagement.ICON_PATH == null ? "" : webDesktopManagement.ICON_PATH.Trim();
                webDesktopManagement.FORM_TYPE = webDesktopManagement.FORM_TYPE == null ? "" : webDesktopManagement.FORM_TYPE.Trim();
                webDesktopManagement.ABBR = webDesktopManagement.ABBR == null ? "" : webDesktopManagement.ABBR.Trim();
                webDesktopManagement.PREV_FOLDER_NAME = webDesktopManagement.PREV_FOLDER_NAME == null ? "" : webDesktopManagement.PREV_FOLDER_NAME.Trim();
                webDesktopManagement.NEW_FOLDER_NAME = webDesktopManagement.NEW_FOLDER_NAME == null ? "" : webDesktopManagement.NEW_FOLDER_NAME.Trim();
                webDesktopManagement.FUNCTION_LINK = webDesktopManagement.FUNCTION_LINK == null ? "" : webDesktopManagement.FUNCTION_LINK.Trim();
                webDesktopManagement.TEMPLATE_CODE = webDesktopManagement.TEMPLATE_CODE == null ? "" : webDesktopManagement.TEMPLATE_CODE.Trim();
                webDesktopManagement.MENU_EDESC = webDesktopManagement.MENU_EDESC == null ? "" : webDesktopManagement.MENU_EDESC.Trim();
                webDesktopManagement.MENU_DESC = webDesktopManagement.MENU_DESC == null ? "" : webDesktopManagement.MENU_DESC.Trim();
                webDesktopManagement.MENU_NO = webDesktopManagement.MENU_NO == null ? "" : webDesktopManagement.MENU_NO.Trim();
                webDesktopManagement.UNIQUE_ID = UniqueId();
                if (webDesktopManagement.SideBarMenu)
                {
                    bool ExistFolder = this._formtemplaterepo.FolderExistsOrNot("Menu");
                    if (!ExistFolder)
                    {
                        this._formtemplaterepo.AddNewFolder("Menu", "b09a1b", "icon-home fontgreen");
                    }
                }
                var a = this._formtemplaterepo.AddWebDesktopManagement(webDesktopManagement);
            }

            return RedirectToAction("FolderTemplate");
        }
        [HttpPost]
        public ActionResult RemoveFolderTemplate(string FolderId)
        {
            this._formtemplaterepo.RemoveFolderByFolderId(FolderId);
            return RedirectToAction("FolderTemplate");
        }
        public string RandomIcon()
        {
            string[] IconArray = new string[] {"fa fa-search", "fa fa-envelope-o", "fa fa-heart", "fa fa-star", "fa fa-star-o", "fa fa-user", "fa fa-film", "fa fa-th-large", "fa fa-th",
       "fa fa-th-list", "fa fa-check", "fa fa-times", "fa fa-search-plus", "fa fa-search-minus", "fa fa-power-off", "fa fa-signal", "fa fa-cog", "fa fa-trash-o", "fa fa-home", "fa fa-file-o", "fa fa-clock-o",
       "fa fa-road", "fa fa-download", "fa fa-arrow-circle-o-down", "fa fa-arrow-circle-o-up", "fa fa-inbox", "fa fa-play-circle-o", "fa fa-repeat", "fa fa-refresh", "fa fa-list-alt", "fa fa-lock", "fa fa-flag",
       "fa fa-headphones", "fa fa-volume-off", "fa fa-volume-down", "fa fa-volume-up", "fa fa-qrcode", "fa fa-barcode", "fa fa-tag", "fa fa-tags", "fa fa-book", "fa fa-bookmark", "fa fa-print", "fa fa-camera",
       "fa fa-font", "fa fa-bold", "fa fa-italic", "fa fa-text-height", "fa fa-text-width", "fa fa-align-left", "fa fa-align-center", "fa fa-align-right", "fa fa-align-justify", "fa fa-list", "fa fa-outdent",
       "fa fa-indent", "fa fa-video-camera", "fa fa-picture-o", "fa fa-pencil", "fa fa-map-marker", "fa fa-adjust", "fa fa-tint", "fa fa-pencil-square-o", "fa fa-share-square-o", "fa fa-check-square-o",
       "fa fa-arrows", "fa fa-step-backward", "fa fa-fast-backward", "fa fa-backward", "fa fa-play", "fa fa-pause", "fa fa-stop", "fa fa-forward", "fa fa-fast-forward", "fa fa-step-forward", "fa fa-eject",
       "fa fa-chevron-left", "fa fa-chevron-right", "fa fa-plus-circle", "fa fa-minus-circle", "fa fa-times-circle", "fa fa-check-circle", "fa fa-question-circle", "fa fa-info-circle", "fa fa-crosshairs",
       "fa fa-times-circle-o", "fa fa-check-circle-o", "fa fa-ban", "fa fa-arrow-left", "fa fa-arrow-right", "fa fa-arrow-up", "fa fa-arrow-down", "fa fa-share", "fa fa-expand", "fa fa-compress", "fa fa-plus",
       "fa fa-minus", "fa fa-asterisk", "fa fa-exclamation-circle", "fa fa-gift", "fa fa-leaf", "fa fa-fire", "fa fa-eye", "fa fa-eye-slash", "fa fa-exclamation-triangle", "fa fa-plane", "fa fa-calendar", "fa fa-random",
       "fa fa-comment", "fa fa-magnet", "fa fa-chevron-up", "fa fa-chevron-down", "fa fa-retweet", "fa fa-shopping-cart", "fa fa-folder", "fa fa-folder-open", "fa fa-arrows-v", "fa fa-arrows-h", "fa fa-bar-chart", "fa fa-twitter-square",
       "fa fa-facebook-square", "fa fa-camera-retro", "fa fa-key", "fa fa-cogs", "fa fa-comments", "fa fa-thumbs-o-up", "fa fa-thumbs-o-down", "fa fa-star-half", "fa fa-heart-o", "fa fa-thumb-tack", "fa fa-external-link", "fa fa-sign-in", "fa fa-trophy", "fa fa-github-square", "fa fa-upload", "fa fa-lemon-o",
       "fa fa-phone", "fa fa-square-o", "fa fa-bookmark-o", "fa fa-phone-square", "fa fa-twitter", "fa fa-facebook", "fa fa-github", "fa fa-unlock", "fa fa-credit-card", "fa fa-rss", "fa fa-hdd-o", "fa fa-bullhorn", "fa fa-bell", "fa fa-certificate", "fa fa-hand-o-right", "fa fa-hand-o-left", "fa fa-hand-o-up",
       "fa fa-hand-o-down",
       "fa fa-arrow-circle-left",
       "fa fa-arrow-circle-right",
       "fa fa-arrow-circle-up",
       "fa fa-arrow-circle-down",
       "fa fa-globe",
       "fa fa-wrench",
       "fa fa-tasks",
       "fa fa-filter",
       "fa fa-briefcase",
       "fa fa-arrows-alt",
       "fa fa-users",
       "fa fa-link",
       "fa fa-cloud",
       "fa fa-flask",
       "fa fa-scissors",
       "fa fa-files-o",
       "fa fa-paperclip",
       "fa fa-floppy-o",
       "fa fa-square",
       "fa fa-bars",
       "fa fa-list-ul",
       "fa fa-list-ol",
       "fa fa-strikethrough",
       "fa fa-underline",
       "fa fa-table",
       "fa fa-magic",
       "fa fa-truck",
       "fa fa-pinterest",
       "fa fa-pinterest-square",
       "fa fa-google-plus-square",
       "fa fa-google-plus",
       "fa fa-money",
       "fa fa-caret-down",
       "fa fa-caret-up",
       "fa fa-caret-left",
       "fa fa-caret-right",
       "fa fa-columns",
       "fa fa-sort",
       "fa fa-sort-desc",
       "fa fa-sort-asc",
       "fa fa-envelope",
       "fa fa-linkedin",
       "fa fa-undo",
       "fa fa-gavel",
       "fa fa-tachometer",
       "fa fa-comment-o",
       "fa fa-comments-o",
       "fa fa-bolt",
       "fa fa-sitemap",
       "fa fa-umbrella",
       "fa fa-clipboard",
       "fa fa-lightbulb-o",
       "fa fa-exchange",
       "fa fa-cloud-download",
       "fa fa-cloud-upload",
       "fa fa-user-md",
       "fa fa-stethoscope",
       "fa fa-suitcase",
       "fa fa-bell-o",
       "fa fa-coffee",
       "fa fa-cutlery",
       "fa fa-file-text-o",
       "fa fa-building-o",
       "fa fa-hospital-o",
       "fa fa-ambulance",
       "fa fa-medkit",
       "fa fa-fighter-jet",
       "fa fa-beer",
       "fa fa-h-square",
       "fa fa-plus-square",
       "fa fa-angle-double-left",
       "fa fa-angle-double-right",
       "fa fa-angle-double-up",
       "fa fa-angle-double-down",
       "fa fa-angle-left",
       "fa fa-angle-right",
       "fa fa-angle-up",
       "fa fa-angle-down",
       "fa fa-desktop",
       "fa fa-laptop",
       "fa fa-tablet",
       "fa fa-mobile",
       "fa fa-circle-o",
       "fa fa-quote-left",
       "fa fa-quote-right",
       "fa fa-spinner",
       "fa fa-circle",
       "fa fa-reply",
       "fa fa-github-alt",
       "fa fa-folder-o",
       "fa fa-folder-open-o",
       "fa fa-smile-o",
       "fa fa-frown-o",
       "fa fa-meh-o",
       "fa fa-gamepad",
       "fa fa-keyboard-o",
       "fa fa-flag-o",
       "fa fa-flag-checkered",
       "fa fa-terminal",
       "fa fa-code",
       "fa fa-reply-all",
       "fa fa-star-half-o",
       "fa fa-location-arrow",
       "fa fa-crop",
       "fa fa-code-fork",
       "fa fa-chain-broken",
       "fa fa-question",
       "fa fa-info",
       "fa fa-exclamation",
       "fa fa-superscript",
       "fa fa-subscript",
       "fa fa-eraser",
       "fa fa-puzzle-piece",
       "fa fa-microphone",
       "fa fa-microphone-slash",
       "fa fa-shield",
       "fa fa-calendar-o",
       "fa fa-fire-extinguisher",
       "fa fa-rocket",
       "fa fa-maxcdn",
       "fa fa-chevron-circle-left",
       "fa fa-chevron-circle-right",
       "fa fa-chevron-circle-up",
       "fa fa-chevron-circle-down",
       "fa fa-html5",
       "fa fa-css3",
       "fa fa-anchor",
       "fa fa-unlock-alt",
       "fa fa-bullseye",
       "fa fa-ellipsis-h",
       "fa fa-ellipsis-v",
       "fa fa-rss-square",
       "fa fa-play-circle",
       "fa fa-ticket",
       "fa fa-minus-square",
       "fa fa-minus-square-o",
       "fa fa-level-up",
       "fa fa-level-down",
       "fa fa-check-square",
       "fa fa-pencil-square",
       "fa fa-external-link-square",
       "fa fa-share-square",
       "fa fa-compass",
       "fa fa-caret-square-o-down",
       "fa fa-caret-square-o-up",
       "fa fa-caret-square-o-right",
       "fa fa-eur",
       "fa fa-gbp",
       "fa fa-usd",
       "fa fa-inr",
       "fa fa-jpy",
       "fa fa-rub",
       "fa fa-krw",
       "fa fa-btc",
       "fa fa-file",
       "fa fa-file-text",
       "fa fa-sort-alpha-asc",
       "fa fa-sort-alpha-desc",
       "fa fa-sort-amount-asc",
       "fa fa-sort-amount-desc",
       "fa fa-sort-numeric-asc",
       "fa fa-sort-numeric-desc",
       "fa fa-thumbs-up",
       "fa fa-thumbs-down",
       "fa fa-youtube-square",
       "fa fa-youtube",
       "fa fa-xing",
       "fa fa-xing-square",
       "fa fa-youtube-play",
       "fa fa-dropbox",
       "fa fa-stack-overflow",
       "fa fa-instagram",
       "fa fa-flickr",
       "fa fa-adn",
       "fa fa-bitbucket",
       "fa fa-bitbucket-square",
       "fa fa-tumblr",
       "fa fa-tumblr-square",
       "fa fa-long-arrow-down",
       "fa fa-long-arrow-up",
       "fa fa-long-arrow-left",
       "fa fa-long-arrow-right",
       "fa fa-apple",
       "fa fa-windows",
       "fa fa-android",
       "fa fa-linux",
       "fa fa-dribbble",
       "fa fa-skype",
       "fa fa-foursquare",
       "fa fa-trello",
       "fa fa-female",
       "fa fa-male",
       "fa fa-gratipay",
       "fa fa-sun-o",
       "fa fa-moon-o",
       "fa fa-archive",
       "fa fa-bug",
       "fa fa-vk",
       "fa fa-weibo",
       "fa fa-renren",
       "fa fa-pagelines",
       "fa fa-stack-exchange",
       "fa fa-arrow-circle-o-right",
       "fa fa-arrow-circle-o-left",
       "fa fa-caret-square-o-left",
       "fa fa-dot-circle-o",
       "fa fa-wheelchair",
       "fa fa-vimeo-square",
       "fa fa-try",
       "fa fa-plus-square-o",
       "fa fa-space-shuttle",
       "fa fa-slack",
       "fa fa-envelope-square",
       "fa fa-wordpress",
       "fa fa-openid",
       "fa fa-university",
       "fa fa-graduation-cap",
       "fa fa-yahoo",
       "fa fa-google",
       "fa fa-reddit",
       "fa fa-reddit-square",
       "fa fa-stumbleupon-circle",
       "fa fa-stumbleupon",
       "fa fa-delicious",
       "fa fa-digg",
       "fa fa-pied-piper-pp",
       "fa fa-pied-piper-alt",
       "fa fa-drupal",
       "fa fa-joomla",
       "fa fa-language",
       "fa fa-fax",
       "fa fa-building",
       "fa fa-child",
       "fa fa-paw",
       "fa fa-spoon",
       "fa fa-cube",
       "fa fa-cubes",
       "fa fa-behance",
       "fa fa-behance-square",
       "fa fa-steam",
       "fa fa-steam-square",
       "fa fa-recycle",
       "fa fa-car",
       "fa fa-taxi",
       "fa fa-tree",
       "fa fa-spotify",
       "fa fa-deviantart",
       "fa fa-soundcloud",
       "fa fa-database",
       "fa fa-file-pdf-o",
       "fa fa-file-word-o",
       "fa fa-file-excel-o",
       "fa fa-file-powerpoint-o",
       "fa fa-file-image-o",
       "fa fa-file-archive-o",
       "fa fa-file-audio-o",
       "fa fa-file-video-o",
       "fa fa-file-code-o",
       "fa fa-vine",
       "fa fa-codepen",
       "fa fa-jsfiddle",
       "fa fa-life-ring",
       "fa fa-circle-o-notch",
       "fa fa-rebel",
       "fa fa-empire",
       "fa fa-git-square",
       "fa fa-git",
       "fa fa-hacker-news",
       "fa fa-tencent-weibo",
       "fa fa-qq",
       "fa fa-weixin",
       "fa fa-paper-plane",
       "fa fa-paper-plane-o",
       "fa fa-history",
       "fa fa-circle-thin",
       "fa fa-header",
       "fa fa-paragraph",
       "fa fa-sliders",
       "fa fa-share-alt",
       "fa fa-share-alt-square",
       "fa fa-bomb",
       "fa fa-futbol-o",
       "fa fa-tty",
       "fa fa-binoculars",
       "fa fa-plug",
       "fa fa-slideshare",
       "fa fa-twitch",
       "fa fa-yelp",
       "fa fa-newspaper-o",
       "fa fa-wifi",
       "fa fa-calculator",
       "fa fa-paypal",
       "fa fa-google-wallet",
       "fa fa-cc-visa",
       "fa fa-cc-mastercard",
       "fa fa-cc-discover",
       "fa fa-cc-amex",
       "fa fa-cc-paypal",
       "fa fa-cc-stripe",
       "fa fa-bell-slash",
       "fa fa-bell-slash-o",
       "fa fa-trash",
       "fa fa-copyright",
       "fa fa-at",
       "fa fa-eyedropper",
       "fa fa-paint-brush",
       "fa fa-birthday-cake",
       "fa fa-area-chart",
       "fa fa-pie-chart",
       "fa fa-line-chart",
       "fa fa-lastfm",
       "fa fa-lastfm-square",
       "fa fa-toggle-off",
       "fa fa-toggle-on",
       "fa fa-bicycle",
       "fa fa-bus",
       "fa fa-ioxhost",
       "fa fa-angellist",
       "fa fa-cc",
       "fa fa-ils",
       "fa fa-meanpath",
       "fa fa-buysellads",
       "fa fa-connectdevelop",
       "fa fa-dashcube",
       "fa fa-forumbee",
       "fa fa-leanpub",
       "fa fa-sellsy",
       "fa fa-shirtsinbulk",
       "fa fa-simplybuilt",
       "fa fa-skyatlas",
       "fa fa-cart-plus",
       "fa fa-cart-arrow-down",
       "fa fa-diamond",
       "fa fa-ship",
       "fa fa-user-secret",
       "fa fa-motorcycle",
       "fa fa-street-view",
       "fa fa-heartbeat",
       "fa fa-venus",
       "fa fa-mars", "fa fa-mercury", "fa fa-transgender", "fa fa-transgender-alt", "fa fa-venus-double", "fa fa-mars-double", "fa fa-venus-mars", "fa fa-mars-stroke", "fa fa-mars-stroke-v",
       "fa fa-mars-stroke-h", "fa fa-neuter", "fa fa-genderless", "fa fa-facebook-official", "fa fa-pinterest-p", "fa fa-whatsapp", "fa fa-server",
       "fa fa-user-plus",
       "fa fa-user-times",
       "fa fa-bed",
       "fa fa-viacoin",
       "fa fa-train",
       "fa fa-subway",
       "fa fa-medium",
       "fa fa-y-combinator",
       "fa fa-optin-monster",
       "fa fa-opencart",
       "fa fa-expeditedssl",
       "fa fa-battery-full",
       "fa fa-battery-three-quarters",
       "fa fa-battery-half",
       "fa fa-battery-quarter",
       "fa fa-battery-empty",
       "fa fa-mouse-pointer",
       "fa fa-i-cursor",
       "fa fa-object-group",
       "fa fa-object-ungroup",
       "fa fa-sticky-note",
       "fa fa-sticky-note-o",
       "fa fa-cc-jcb",
       "fa fa-cc-diners-club",
       "fa fa-clone",
       "fa fa-balance-scale",
       "fa fa-hourglass-o",
       "fa fa-hourglass-start",
       "fa fa-hourglass-half",
       "fa fa-hourglass-end",
       "fa fa-hourglass",
       "fa fa-hand-rock-o",
       "fa fa-hand-paper-o",
       "fa fa-hand-scissors-o",
       "fa fa-hand-lizard-o",
       "fa fa-hand-spock-o",
       "fa fa-hand-pointer-o",
       "fa fa-hand-peace-o",
       "fa fa-trademark",
       "fa fa-registered",
       "fa fa-creative-commons",
       "fa fa-gg",
       "fa fa-gg-circle",
       "fa fa-tripadvisor",
       "fa fa-odnoklassniki",
       "fa fa-odnoklassniki-square",
       "fa fa-get-pocket",
       "fa fa-wikipedia-w",
       "fa fa-safari",
       "fa fa-chrome",
       "fa fa-firefox",
       "fa fa-opera",
       "fa fa-internet-explorer",
       "fa fa-television",
       "fa fa-contao",
       "fa fa-500px",
       "fa fa-amazon",
       "fa fa-calendar-plus-o",
       "fa fa-calendar-minus-o",
       "fa fa-calendar-times-o",
       "fa fa-calendar-check-o",
       "fa fa-industry",
       "fa fa-map-pin",
       "fa fa-map-signs",
       "fa fa-map-o",
       "fa fa-map",
       "fa fa-commenting",
       "fa fa-commenting-o",
       "fa fa-houzz",
       "fa fa-vimeo",
       "fa fa-black-tie",
       "fa fa-fonticons",
       "fa fa-reddit-alien",
       "fa fa-edge",
       "fa fa-credit-card-alt",
       "fa fa-codiepie",
       "fa fa-modx",
       "fa fa-fort-awesome",
       "fa fa-usb",
       "fa fa-product-hunt",
       "fa fa-mixcloud",
       "fa fa-scribd",
       "fa fa-pause-circle",
       "fa fa-pause-circle-o",
       "fa fa-stop-circle",
       "fa fa-stop-circle-o",
       "fa fa-shopping-bag",
       "fa fa-shopping-basket",
       "fa fa-hashtag",
       "fa fa-bluetooth",
       "fa fa-bluetooth-b",
       "fa fa-percent",
       "fa fa-gitlab",
       "fa fa-wpbeginner",
       "fa fa-wpforms",
       "fa fa-envira",
       "fa fa-universal-access",
       "fa fa-wheelchair-alt",
       "fa fa-question-circle-o",
       "fa fa-blind",
       "fa fa-audio-description",
       "fa fa-volume-control-phone",
       "fa fa-braille",
       "fa fa-assistive-listening-systems",
       "fa fa-american-sign-language-interpreting",
       "fa fa-deaf",
       "fa fa-glide",
       "fa fa-glide-g",
       "fa fa-sign-language",
       "fa fa-low-vision",
       "fa fa-viadeo",
       "fa fa-viadeo-square",
       "fa fa-snapchat",
       "fa fa-snapchat-ghost",
       "fa fa-snapchat-square",
       "fa fa-pied-piper",
       "fa fa-first-order",
       "fa fa-yoast",
       "fa fa-themeisle",
       "fa fa-google-plus-official",
       "fa fa-font-awesome"};

            var IconNumber = 1;
            return IconArray[IconNumber];
        }
        public string RandomColor()
        {

            Random random = new Random();
            StringBuilder color = new StringBuilder();
            var randomcolor = $"#{random.Next(0x1000000):X6}";
            return randomcolor;
        }
        public string UniqueId()
        {
            Guid g = Guid.NewGuid();
            string GuidString = Convert.ToBase64String(g.ToByteArray());
            GuidString = GuidString.Replace("=", "");
            GuidString = GuidString.Replace("+", "");
            return GuidString;
        }

        #region Import_sales_invoice_from_excel
        [HttpPost]
        public ActionResult ImrortSalesInvoiceFromExcel()

        {
            //Excel.Application application = new Excel.Application();
            string filePath = System.Configuration.ConfigurationManager.AppSettings["ExcelPath"].ToString();//"D:\\Chandra Work Folder\\New Projects\\tets.xlsx";
            string sheetName = "Sheet1";
            var excel = new ExcelQueryFactory(filePath);
            var worksheetNames = excel.GetWorksheetNames();
            var worksheetname = "";
            if (worksheetNames.ElementAt(0) == sheetName)
                worksheetname = sheetName;
            else
                worksheetname = worksheetNames.ElementAt(worksheetNames.Count() - 1);
            var exceldata = from a in excel.Worksheet<SalesInvoiceExcel>(worksheetname) select a;
            var listItems = exceldata.ToList();
            var newItem = new List<SalesInvoiceExcel>();

            int lastinsertNum = GetLastInsertedNum();
            newItem.AddRange(listItems.Skip(lastinsertNum));
            if (newItem.Count > 0)
            {
                //bool result = _formtemplaterepo.SaveInvoiceData(newItem);
                bool result = false;
                if (result) UpdateXml(listItems.Count);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        private void UpdateXml(int count)
        {
            XDocument xmlDoc = XDocument.Load(Server.MapPath("~/App_Data/SalesInvoceExcel.xml"));
            xmlDoc.Element("LastInsert").Element("Number").Value = count.ToString();
            xmlDoc.Save(Server.MapPath("~/App_Data/SalesInvoceExcel.xml"));

        }
        public int GetLastInsertedNum()
        {
            DataSet ds = new DataSet();
            ds.ReadXml(Server.MapPath("~/App_Data/SalesInvoceExcel.xml"));
            DataView dvPrograms;
            dvPrograms = ds.Tables[0].DefaultView;
            int lastnum = 0;
            foreach (DataRowView dr in dvPrograms)
            {
                lastnum = Convert.ToInt32(dr[0]);
            }
            return lastnum;
        }



        [HttpPost]
        public JsonResult ImportChalanData(HttpPostedFileBase file, string form_code, string table_name)
        {
            _logErp.InfoInFile("Import chalan data started");
            //Excel.Application application = new Excel.Application();
            _logErp.InfoInFile("Excel.Application ne Application created");
            try
            {
                if (file == null || file.ContentLength == 0)
                {

                    return Json(new { TYPE = "error", MESSAGE = "Empty File" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (file.FileName.EndsWith("xls") || file.FileName.EndsWith("xlsx"))
                    {

                        var today = DateTime.Now.ToString("MMM d yyyy");
                        _logErp.InfoInFile("Import Chalan Log:====>>>today is :  " + today);
                        string paths = Server.MapPath("~/SalesChalanExcel/" + today + "/" + file.FileName);
                        string strMappath = $"~/SalesChalanExcel/{today}/";
                        string path = System.IO.Path.Combine(Server.MapPath(strMappath), file.FileName);
                        _logErp.InfoInFile("Import Chalan Log:====>>>path to save excel file is  :  " + path);
                        if (!Directory.Exists(strMappath))
                        {
                            Directory.CreateDirectory(Server.MapPath(strMappath));
                        }
                        if (System.IO.File.Exists(path))
                            System.IO.File.Delete(path);
                        file.SaveAs(path);
                        _logErp.InfoInFile("Import Chalan Log:====>>>please check is chalan excel folder is created or not here=>:  " + path);
                        //string sheetName = "Data Format";
                        string filepath = paths;
                        var excel = new ExcelQueryFactory(filepath);
                        var worksheetNames = excel.GetWorksheetNames();
                        _logErp.InfoInFile("Import Chalan Log:====>>>workshet name is :  " + worksheetNames.ElementAt(0));
                        //if (worksheetNames.ElementAt(0) != sheetName)
                        //{
                        //    return Json(new { TYPE = "warning", MESSAGE = "Sheet name mismatched" }, JsonRequestBehavior.AllowGet);
                        //}

                        var chalanFromExcel = (from a in excel.Worksheet<SalesChalanExcelData>(worksheetNames.ElementAt(0))
                                               select a).ToList();


                        _logErp.InfoInFile("Import Chalan Log:====>>>data from excel is :  " + chalanFromExcel);



                        _logErp.InfoInFile("Import Chalan Log:====>>>SaveSalesChalan Method called with Param: " + form_code + "," + table_name);
                        //var chalanSavedResponse = _FormSetupRepo.SaveSalesChalanFromExcel(chalanFromExcel,form_code, table_name);
                        var chalanSavedResponse = _FormSetupRepo.SaveSalesChalanFromExcel(chalanFromExcel, form_code, table_name);
                        _logErp.InfoInFile("Import Chalan Log:====>>>chalan saved response is " + chalanSavedResponse);
                        if (chalanSavedResponse == "Success")
                        {
                            return Json(new { TYPE = "success", MESSAGE = "Chalan successfully imported" }, JsonRequestBehavior.AllowGet);

                        }
                        else if (chalanSavedResponse == "I Think No Row In Excel")
                        {
                            return Json(new { TYPE = "NoRow", MESSAGE = "No Row In Excel!!!", data = chalanSavedResponse }, JsonRequestBehavior.AllowGet);
                        }
                        else if (chalanSavedResponse == "I Think No New Order  In Excel")
                        {
                            return Json(new { TYPE = "NoOrder", MESSAGE = "No New Order  In Excel!!!", data = chalanSavedResponse }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { TYPE = "warning", MESSAGE = "File format error", data = chalanSavedResponse }, JsonRequestBehavior.AllowGet);
                        }

                    }
                    else
                    {
                        return Json(new { TYPE = "error", MESSAGE = "File format error" }, JsonRequestBehavior.AllowGet);
                    }

                }
            }
            catch (Exception ex)
            {
                return Json(new { TYPE = "Exception", MESSAGE = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }







        #endregion

    }
}