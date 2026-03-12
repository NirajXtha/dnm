using NeoErp.Core;
using NeoErp.Core.Models.Log4NetLoggin;
using NeoErp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NeoERP.QCQAManagement.Service.Models;
using NeoERP.QCQAManagement.Service.Interface;

namespace NeoERP.QCQAManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogErp _logErp;
        private DefaultValueForLog _defaultValueForLog;
        private IWorkContext _workContext;
        private IDbContext _dbContext;
        private IParameterRepo _IParameterRepo;
        private IQCQADocumentFinderRepo _IQCQADocumentFinderRepo;
        public HomeController(IParameterRepo IParameterRepo, IQCQADocumentFinderRepo IQCQADocumentFinderRepo, IWorkContext workContext, IDbContext dbContext)
        {
            this._IParameterRepo = IParameterRepo;
            this._IQCQADocumentFinderRepo = IQCQADocumentFinderRepo;
            this._dbContext = dbContext;
            this._workContext = workContext;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            this._logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule, _defaultValueForLog.FormCode);
        }
        public ActionResult Index()
        {
            return View();
        }
        public List<Company> GetCompany()
        {
            var company_code = _workContext.CurrentUserinformation.company_code;
            string query = $@"select COMPANY_CODE,COMPANY_EDESC,ADDRESS,EMAIL,LOGO_FILE_NAME from COMPANY_SETUP WHERE COMPANY_CODE='{company_code}'";
            List<Company> company = _dbContext.SqlQuery<Company>(query).ToList();
            return company;
        }
        public ActionResult QCQAInspectionSetup()
        {
            return View();
        }
        public ActionResult QCQADetail()
        {
            return View();
        }
        public ActionResult Dashboard()
        {
            return View();
        }
        [ChildActionOnly]
        public PartialViewResult QCQAManagementMenu()
        {
            //var companyCode = _workContext.CurrentUserinformation.company_code;

            List<SubMenu> tableLists = _IParameterRepo.GetSubMenuList();

            return PartialView("_QCQAManagementMenu", tableLists);
        }
        [HttpGet]
        public ActionResult GetQCQADetailByFormCode(string formCode, string docVer = "All")
        {
            var response = new List<QCDocumentFinder>();
            var SubMenuDetailList = this._IQCQADocumentFinderRepo.GetDocumentDetails(formCode, docVer);
            return View();
            //return PartialView("QCQADocumentFinderList", SubMenuDetailList);
            //response = SubMenuDetailList;
            //return response;
        }
        [HttpGet]
        public ActionResult QCQADocumentFinderList(string formCode, string docVer = "All")
        {
            var response = new List<QCDocumentFinder>();
            //ViewBag.formCode = formCode;
            var SubMenuDetailList = this._IParameterRepo.GetQCQADetailByFormCode(formCode, docVer);
           return View();

            //List<SubMenu> tableLists = _IParameterRepo.GetSubMenuList();

            //return PartialView("_QCQAManagementMenu", tableLists);
            //return PartialView("_QCQAManagementMenu");
            //return PartialView("QCQADocumentFinderList", SubMenuDetailList);
            //response = SubMenuDetailList;
            //return response;
        }
        #region
        public ActionResult QCQANumberSetupList()
        {
            return View();
        }
        #endregion
        #region
        public ActionResult ParameterSetupList()
        {
            return View();
        }
        #endregion
        #region Product Details
        public ActionResult InternalInspectionSetupList(string voucherno)
        {
            return View();
        }
        #endregion
        #region Incoming Material
        public ActionResult IncomingMaterialList(string voucherno)
        {
            string query_formCode = $@"SELECT form_code   FROM form_setup WHERE FORM_TYPE = 'IN'";
            ViewBag.formCode = this._dbContext.SqlQuery<string>(query_formCode).FirstOrDefault();
            string bodylength = $@"SELECT BODY_LENGTH FROM form_setup WHERE FORM_TYPE = 'RM'";
            ViewBag.bodyLength = this._dbContext.SqlQuery<int>(bodylength).FirstOrDefault();
            string query_reference_no = $@"SELECT LPAD(COALESCE(MAX(TO_NUMBER(REGEXP_SUBSTR(TRANSACTION_NO, '[^/]+', 1, 2))),0) + 1,'{ViewBag.bodyLength}','0') AS REFERENCE_NO FROM QC_PARAMETER_TRANSACTION WHERE form_code = '{ViewBag.formCode}'";
            string reference_no = this._dbContext.SqlQuery<string>(query_reference_no).FirstOrDefault();
            string query_voucherNo = $@"SELECT CONCAT(CONCAT(CUSTOM_PREFIX_TEXT, '{reference_no}'), CUSTOM_SUFFIX_TEXT) AS DSF   FROM form_setup WHERE FORM_TYPE = 'IN'";
            ViewBag.QCNO = this._dbContext.SqlQuery<string>(query_voucherNo).FirstOrDefault();
            return View();           
        }
        #endregion
        #region Incoming Material
        public ActionResult IncomingMaterialDirectList(string voucherno)
        {
            string query_formCode = $@"SELECT form_code   FROM form_setup WHERE FORM_TYPE = 'IS'";
            ViewBag.formCode = this._dbContext.SqlQuery<string>(query_formCode).FirstOrDefault();
            string bodylength = $@"SELECT BODY_LENGTH FROM form_setup WHERE FORM_TYPE = 'RM'";
            ViewBag.bodyLength = this._dbContext.SqlQuery<int>(bodylength).FirstOrDefault();
            string query_reference_no = $@"SELECT LPAD(COALESCE(MAX(TO_NUMBER(REGEXP_SUBSTR(TRANSACTION_NO, '[^/]+', 1, 2))),0) + 1,'{ViewBag.bodyLength}','0') AS REFERENCE_NO FROM QC_PARAMETER_TRANSACTION WHERE form_code = '{ViewBag.formCode}'";
            string reference_no = this._dbContext.SqlQuery<string>(query_reference_no).FirstOrDefault();
            string query_voucherNo = $@"SELECT CONCAT(CONCAT(CUSTOM_PREFIX_TEXT, '{reference_no}'), CUSTOM_SUFFIX_TEXT) AS DSF   FROM form_setup WHERE FORM_TYPE = 'IS'";
            ViewBag.QCNO = this._dbContext.SqlQuery<string>(query_voucherNo).FirstOrDefault();
            return View();
        }
        #endregion
        #region Raw Material
        public ActionResult RawMaterialList(string voucherno)
        {
            string query_formCode = $@"SELECT form_code   FROM form_setup WHERE FORM_TYPE = 'RM'";
            ViewBag.formCode = this._dbContext.SqlQuery<string>(query_formCode).FirstOrDefault();
            string bodylength = $@"SELECT BODY_LENGTH FROM form_setup WHERE FORM_TYPE = 'RM'";
            ViewBag.bodyLength = this._dbContext.SqlQuery<int>(bodylength).FirstOrDefault();
            string query_reference_no = $@"SELECT LPAD(COALESCE(MAX(TO_NUMBER(REGEXP_SUBSTR(TRANSACTION_NO, '[^/]+', 1, 2))),0) + 1,'{ViewBag.bodyLength}','0') AS REFERENCE_NO FROM QC_PARAMETER_TRANSACTION WHERE form_code = '{ViewBag.formCode}'";
            string reference_no = this._dbContext.SqlQuery<string>(query_reference_no).FirstOrDefault();
            string query_voucherNo = $@"SELECT CONCAT(CONCAT(CUSTOM_PREFIX_TEXT, '{reference_no}'), CUSTOM_SUFFIX_TEXT) AS DSF   FROM form_setup WHERE FORM_TYPE = 'RM'";
            ViewBag.QCNO = this._dbContext.SqlQuery<string>(query_voucherNo).FirstOrDefault();
            return View();
        }
        #endregion
        #region Daily Wastage
        public ActionResult DailyWastageList(string voucherno)
        {
            string query_formCode = $@"SELECT form_code   FROM form_setup WHERE FORM_TYPE = 'DW'";
            ViewBag.formCode = this._dbContext.SqlQuery<string>(query_formCode).FirstOrDefault();
            string bodylength = $@"SELECT BODY_LENGTH FROM form_setup WHERE FORM_TYPE = 'RM'";
            ViewBag.bodyLength = this._dbContext.SqlQuery<int>(bodylength).FirstOrDefault();
            string query_reference_no = $@"SELECT LPAD(COALESCE(MAX(TO_NUMBER(REGEXP_SUBSTR(TRANSACTION_NO, '[^/]+', 1, 2))),0) + 1,'{ViewBag.bodyLength}','0') AS REFERENCE_NO FROM QC_PARAMETER_TRANSACTION WHERE form_code = '{ViewBag.formCode}'";
            string reference_no = this._dbContext.SqlQuery<string>(query_reference_no).FirstOrDefault();
            string query_voucherNo = $@"SELECT CONCAT(CONCAT(CUSTOM_PREFIX_TEXT, '{reference_no}'), CUSTOM_SUFFIX_TEXT) AS DSF   FROM form_setup WHERE FORM_TYPE = 'DW'";
            ViewBag.QCNO = this._dbContext.SqlQuery<string>(query_voucherNo).FirstOrDefault();
            ViewBag.FormType = "DW";
            return View();
        }
        #endregion
        #region PreDispatch Inspection
        public ActionResult PreDispatchInspectionList(string voucherno)
        {
            string query_formCode = $@"SELECT form_code   FROM form_setup WHERE FORM_TYPE = 'PD'";
            ViewBag.formCode = this._dbContext.SqlQuery<string>(query_formCode).FirstOrDefault();
            string bodylength = $@"SELECT BODY_LENGTH FROM form_setup WHERE FORM_TYPE = 'RM'";
            ViewBag.bodyLength = this._dbContext.SqlQuery<int>(bodylength).FirstOrDefault();
            string query_reference_no = $@"SELECT LPAD(COALESCE(MAX(TO_NUMBER(REGEXP_SUBSTR(TRANSACTION_NO, '[^/]+', 1, 2))),0) + 1,'{ViewBag.bodyLength}','0') AS REFERENCE_NO FROM QC_PARAMETER_TRANSACTION WHERE form_code = '{ViewBag.formCode}'";
            string reference_no = this._dbContext.SqlQuery<string>(query_reference_no).FirstOrDefault();
            string query_voucherNo = $@"SELECT CONCAT(CONCAT(CUSTOM_PREFIX_TEXT, '{reference_no}'), CUSTOM_SUFFIX_TEXT) AS DSF   FROM form_setup WHERE FORM_TYPE = 'PD'";
            ViewBag.QCNO = this._dbContext.SqlQuery<string>(query_voucherNo).FirstOrDefault();
            return View();
        }
        #endregion

        #region Lab Testing 
        public ActionResult LabTestingList(string voucherno)
        {
            string query_reference_no = $@"SELECT LPAD(COALESCE(MAX(TO_NUMBER(REGEXP_SUBSTR(TRANSACTION_NO, '[^/]+', 1, 2))), 0) + 1,2,'0') AS REFERENCE_NO FROM QC_PARAMETER_TRANSACTION where form_code ='501'";
            string reference_no = this._dbContext.SqlQuery<string>(query_reference_no).FirstOrDefault();
            string query_voucherNo = $@"SELECT CONCAT(CONCAT(CUSTOM_PREFIX_TEXT, '{reference_no}'), CUSTOM_SUFFIX_TEXT) AS DSF   FROM form_setup WHERE FORM_TYPE = 'QC' and form_code ='502'";
            ViewBag.QCNO = this._dbContext.SqlQuery<string>(query_voucherNo).FirstOrDefault();
            return View();
        }
        #endregion

        #region Global Agro Products
        public ActionResult GlobalAgroProductsList(string voucherno)
        {
            string query_reference_no = $@"SELECT LPAD(COALESCE(MAX(TO_NUMBER(REGEXP_SUBSTR(TRANSACTION_NO, '[^/]+', 1, 2))), 0) + 1,2,'0') AS REFERENCE_NO FROM GLOBAL_PRODUCTS_TRANSACTION";
            string reference_no = this._dbContext.SqlQuery<string>(query_reference_no).FirstOrDefault();
            string query_voucherNo = $@"SELECT CONCAT(CONCAT(CUSTOM_PREFIX_TEXT, '{reference_no}'), CUSTOM_SUFFIX_TEXT) AS DSF   FROM form_setup WHERE FORM_TYPE = 'GP'";
            ViewBag.QCNO = this._dbContext.SqlQuery<string>(query_voucherNo).FirstOrDefault();
            string query_formCode = $@"SELECT form_code   FROM form_setup WHERE FORM_TYPE = 'GP'";
            ViewBag.formCode = this._dbContext.SqlQuery<string>(query_formCode).FirstOrDefault();
            return View();
        }
        #endregion
        #region Product Details
        public ActionResult ProductSetupList(string voucherno)
        {
            //string query_reference_no = $@"SELECT LPAD(COALESCE(MAX(TO_NUMBER(REGEXP_SUBSTR(TRANSACTION_NO, '[^/]+', 1, 2))), 0) + 1,2,'0') AS REFERENCE_NO FROM GLOBAL_PRODUCTS_TRANSACTION";
            //string reference_no = this._dbContext.SqlQuery<string>(query_reference_no).FirstOrDefault();
            //string query_voucherNo = $@"SELECT CONCAT(CONCAT(CUSTOM_PREFIX_TEXT, '{reference_no}'), CUSTOM_SUFFIX_TEXT) AS DSF   FROM form_setup WHERE FORM_TYPE = 'GP'";
            //ViewBag.QCNO = this._dbContext.SqlQuery<string>(query_voucherNo).FirstOrDefault();
            //string query_formCode = $@"SELECT form_code   FROM form_setup WHERE FORM_TYPE = 'GP'";
            //ViewBag.formCode = this._dbContext.SqlQuery<string>(query_formCode).FirstOrDefault();
            return View();
        }
        #endregion
        #region OnSite Inspection
        public ActionResult OnSiteInspectionList(string voucherno)
        {
            string query_formCode = $@"SELECT form_code   FROM form_setup WHERE FORM_TYPE = 'OI'";
            ViewBag.formCode = this._dbContext.SqlQuery<string>(query_formCode).FirstOrDefault();
            string bodylength = $@"SELECT BODY_LENGTH FROM form_setup WHERE FORM_TYPE = 'RM'";
            ViewBag.bodyLength = this._dbContext.SqlQuery<int>(bodylength).FirstOrDefault();
            string query_reference_no = $@"SELECT LPAD(COALESCE(MAX(TO_NUMBER(REGEXP_SUBSTR(TRANSACTION_NO, '[^/]+', 1, 2))),0) + 1,'{ViewBag.bodyLength}','0') AS REFERENCE_NO FROM QC_PARAMETER_TRANSACTION WHERE form_code = '{ViewBag.formCode}'";
            string reference_no = this._dbContext.SqlQuery<string>(query_reference_no).FirstOrDefault();
            string query_voucherNo = $@"SELECT CONCAT(CONCAT(CUSTOM_PREFIX_TEXT, '{reference_no}'), CUSTOM_SUFFIX_TEXT) AS DSF   FROM form_setup WHERE FORM_TYPE = 'OI'";
            ViewBag.QCNO = this._dbContext.SqlQuery<string>(query_voucherNo).FirstOrDefault();
            ViewBag.FormType = "OI";
            //string query_formCode = $@"SELECT form_code   FROM form_setup WHERE FORM_TYPE = 'OI'";
            //ViewBag.formCode = this._dbContext.SqlQuery<string>(query_formCode).FirstOrDefault();
            return View();
        }
        #endregion
        #region Internal Inspection
        public ActionResult InternalInspectionList(string voucherno)
        {
            string query_formCode = $@"SELECT form_code   FROM form_setup WHERE FORM_TYPE = 'II'";
            ViewBag.formCode = this._dbContext.SqlQuery<string>(query_formCode).FirstOrDefault();
            string bodylength = $@"SELECT BODY_LENGTH FROM form_setup WHERE FORM_TYPE = 'RM'";
            ViewBag.bodyLength = this._dbContext.SqlQuery<int>(bodylength).FirstOrDefault();
            string query_reference_no = $@"SELECT LPAD(COALESCE(MAX(TO_NUMBER(REGEXP_SUBSTR(TRANSACTION_NO, '[^/]+', 1, 2))),0) + 1,'{ViewBag.bodyLength}','0') AS REFERENCE_NO FROM QC_PARAMETER_TRANSACTION WHERE form_code = '{ViewBag.formCode}'";
            string reference_no = this._dbContext.SqlQuery<string>(query_reference_no).FirstOrDefault();
            string query_voucherNo = $@"SELECT CONCAT(CONCAT(CUSTOM_PREFIX_TEXT, '{reference_no}'), CUSTOM_SUFFIX_TEXT) AS DSF   FROM form_setup WHERE FORM_TYPE = 'II'";
            ViewBag.QCNO = this._dbContext.SqlQuery<string>(query_voucherNo).FirstOrDefault();
            ViewBag.FormType = "II";
            //string query_formCode = $@"SELECT form_code   FROM form_setup WHERE FORM_TYPE = 'OI'";
            //ViewBag.formCode = this._dbContext.SqlQuery<string>(query_formCode).FirstOrDefault();
            return View();
        }
        #endregion
        #region Sanitation Hygiene
        public ActionResult SanitationHygieneList(string voucherno)
        {
            string query_formCode = $@"SELECT form_code   FROM form_setup WHERE FORM_TYPE = 'SH'";
            ViewBag.formCode = this._dbContext.SqlQuery<string>(query_formCode).FirstOrDefault();
            string bodylength = $@"SELECT BODY_LENGTH FROM form_setup WHERE FORM_TYPE = 'RM'";
            ViewBag.bodyLength = this._dbContext.SqlQuery<int>(bodylength).FirstOrDefault();
            string query_reference_no = $@"SELECT LPAD(COALESCE(MAX(TO_NUMBER(REGEXP_SUBSTR(TRANSACTION_NO, '[^/]+', 1, 2))),0) + 1,'{ViewBag.bodyLength}','0') AS REFERENCE_NO FROM QC_PARAMETER_TRANSACTION WHERE form_code = '{ViewBag.formCode}'";
            string reference_no = this._dbContext.SqlQuery<string>(query_reference_no).FirstOrDefault();
            string query_voucherNo = $@"SELECT CONCAT(CONCAT(CUSTOM_PREFIX_TEXT, '{reference_no}'), CUSTOM_SUFFIX_TEXT) AS DSF   FROM form_setup WHERE FORM_TYPE = 'SH'";
            ViewBag.QCNO = this._dbContext.SqlQuery<string>(query_voucherNo).FirstOrDefault();
            return View();
        }
        public ActionResult SanitationHygieneReportList(string voucherno)
        {
            string query_formCode = $@"SELECT form_code   FROM form_setup WHERE FORM_TYPE = 'SH'";
            ViewBag.formCode = this._dbContext.SqlQuery<string>(query_formCode).FirstOrDefault();
            string bodylength = $@"SELECT BODY_LENGTH FROM form_setup WHERE FORM_TYPE = 'RM'";
            ViewBag.bodyLength = this._dbContext.SqlQuery<int>(bodylength).FirstOrDefault();
            string query_reference_no = $@"SELECT LPAD(COALESCE(MAX(TO_NUMBER(REGEXP_SUBSTR(TRANSACTION_NO, '[^/]+', 1, 2))),0) + 1,'{ViewBag.bodyLength}','0') AS REFERENCE_NO FROM QC_PARAMETER_TRANSACTION WHERE form_code = '{ViewBag.formCode}'";
            string reference_no = this._dbContext.SqlQuery<string>(query_reference_no).FirstOrDefault();
            string query_voucherNo = $@"SELECT CONCAT(CONCAT(CUSTOM_PREFIX_TEXT, '{reference_no}'), CUSTOM_SUFFIX_TEXT) AS DSF   FROM form_setup WHERE FORM_TYPE = 'SH'";
            ViewBag.QCNO = this._dbContext.SqlQuery<string>(query_voucherNo).FirstOrDefault();
            return View();
        }
        #endregion
        #region Parameter Inspection Setup
        public ActionResult ParameterInspectionSetupList()
        {
            return View();
        }
        #endregion
        #region Hand Over Inspection
        public ActionResult HandOverInspectionList(string voucherno)
        {
            string query_formCode = $@"SELECT form_code   FROM form_setup WHERE FORM_TYPE = 'HO'";
            ViewBag.formCode = this._dbContext.SqlQuery<string>(query_formCode).FirstOrDefault();
            string bodylength = $@"SELECT BODY_LENGTH FROM form_setup WHERE FORM_TYPE = 'RM'";
            ViewBag.bodyLength = this._dbContext.SqlQuery<int>(bodylength).FirstOrDefault();
            string query_reference_no = $@"SELECT LPAD(COALESCE(MAX(TO_NUMBER(REGEXP_SUBSTR(TRANSACTION_NO, '[^/]+', 1, 2))),0) + 1,'{ViewBag.bodyLength}','0') AS REFERENCE_NO FROM QC_PARAMETER_TRANSACTION WHERE form_code = '{ViewBag.formCode}'";
            string reference_no = this._dbContext.SqlQuery<string>(query_reference_no).FirstOrDefault();
            string query_voucherNo = $@"SELECT CONCAT(CONCAT(CUSTOM_PREFIX_TEXT, '{reference_no}'), CUSTOM_SUFFIX_TEXT) AS DSF   FROM form_setup WHERE FORM_TYPE = 'HO'";
            ViewBag.QCNO = this._dbContext.SqlQuery<string>(query_voucherNo).FirstOrDefault();
            return View();
        }
        #endregion
        #region Finished Goods Setup
        public ActionResult FinishedGoodsSetupList(string voucherno)
        {
            return View();
        }
        #endregion
        #region Finished Goods Inspection
        public ActionResult FinishedGoodsInspectionList(string voucherno)
        {
            string query_formCode = $@"SELECT form_code   FROM form_setup WHERE FORM_TYPE = 'FI'";
            ViewBag.formCode = this._dbContext.SqlQuery<string>(query_formCode).FirstOrDefault();
            string bodylength = $@"SELECT BODY_LENGTH FROM form_setup WHERE FORM_TYPE = 'RM'";
            ViewBag.bodyLength = this._dbContext.SqlQuery<int>(bodylength).FirstOrDefault();
            string query_reference_no = $@"SELECT LPAD(COALESCE(MAX(TO_NUMBER(REGEXP_SUBSTR(TRANSACTION_NO, '[^/]+', 1, 2))),0) + 1,'{ViewBag.bodyLength}','0') AS REFERENCE_NO FROM QC_PARAMETER_TRANSACTION WHERE form_code = '{ViewBag.formCode}'";
            string reference_no = this._dbContext.SqlQuery<string>(query_reference_no).FirstOrDefault();
            string query_voucherNo = $@"SELECT CONCAT(CONCAT(CUSTOM_PREFIX_TEXT, '{reference_no}'), CUSTOM_SUFFIX_TEXT) AS DSF   FROM form_setup WHERE FORM_TYPE = 'FI'";
            ViewBag.QCNO = this._dbContext.SqlQuery<string>(query_voucherNo).FirstOrDefault();
            return View();
        }
        #endregion

        #region Finished Goods Inspection
        public ActionResult QCQADocumentFinderList(string formCode)
        {
            string query_formCode = $@"SELECT form_code   FROM form_setup WHERE FORM_TYPE = 'FI'";
            ViewBag.formCode = this._dbContext.SqlQuery<string>(query_formCode).FirstOrDefault();
            string bodylength = $@"SELECT BODY_LENGTH FROM form_setup WHERE FORM_TYPE = 'RM'";
            ViewBag.bodyLength = this._dbContext.SqlQuery<int>(bodylength).FirstOrDefault();
            string query_reference_no = $@"SELECT LPAD(COALESCE(MAX(TO_NUMBER(REGEXP_SUBSTR(TRANSACTION_NO, '[^/]+', 1, 2))),0) + 1,'{ViewBag.bodyLength}','0') AS REFERENCE_NO FROM QC_PARAMETER_TRANSACTION WHERE form_code = '{ViewBag.formCode}'";
            string reference_no = this._dbContext.SqlQuery<string>(query_reference_no).FirstOrDefault();
            string query_voucherNo = $@"SELECT CONCAT(CONCAT(CUSTOM_PREFIX_TEXT, '{reference_no}'), CUSTOM_SUFFIX_TEXT) AS DSF   FROM form_setup WHERE FORM_TYPE = 'FI'";
            ViewBag.QCNO = this._dbContext.SqlQuery<string>(query_voucherNo).FirstOrDefault();
            return View();
        }
        #endregion

    }
}