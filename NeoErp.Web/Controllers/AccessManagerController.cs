
using NeoErp.Models;
using NeoErp.Services.AccessManager;
using System.Web.Mvc;

namespace NeoErp.Controllers
{
    [Authorize]
    public class AccessManagerController : Controller
    {
        private IAccessManager _accessManager;

        public AccessManagerController(IAccessManager accessManager)
        {
            this._accessManager = accessManager;
        }

       
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult GetAllUserTree()
        {
            var userTree = _accessManager.GetAllUserTree();
            var jsonResult = Json(userTree, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult GetDropdownUser()
        {
            var ddUser = _accessManager.GetDropdownUsers();
            var jResult = Json(ddUser, JsonRequestBehavior.AllowGet);
            return jResult;
        }

        public ActionResult GetAppModuleDDL()
        {
            var appModule = _accessManager.GetAppModuleDDL();
            var appResult = Json(appModule, JsonRequestBehavior.AllowGet);
            return appResult;
        }

        public ActionResult GetAllAvailableControl(string userNo="0",string selectedControl="0")
        {
            var allControl = _accessManager.GetAvailableControls(userNo,selectedControl);
            var controlRes = Json(allControl, JsonRequestBehavior.AllowGet);
            controlRes.MaxJsonLength = int.MaxValue;
            return controlRes;
        }

        public ActionResult GetAvailableModuleControl(string userNo="0",string selectedControl = "0")
        {
            var allModuleCntrl = _accessManager.GetAvailableModuleControl(userNo, selectedControl);
            var moduleRes = Json(allModuleCntrl, JsonRequestBehavior.AllowGet);
            return moduleRes;
        }

        public ActionResult GetMasterSetupListViewControl(string userNo="0",string selectedControl = "0")
        {
            var masterSetupList = _accessManager.GetMasterSetupListControl(userNo, selectedControl);
            var masterSetupResult = Json(masterSetupList, JsonRequestBehavior.AllowGet);
            return masterSetupResult;
        }

        public ActionResult GetAvailableCompanyCntrl(string userNo="0",string selectedControl="0")
        {
            var company = _accessManager.GetAvailableCompanyControl(userNo);
            var companyRes = Json(company, JsonRequestBehavior.AllowGet);
            return companyRes;
        }

        public ActionResult GetAvailableMasterDefinitionCntrl(string userNo="0",string selectedControl="0")
        {
            var masterDefCntrl = _accessManager.GetAvailableMasterDefinitionCntrl(userNo);
            var masterDefResult = Json(masterDefCntrl, JsonRequestBehavior.AllowGet);
            return masterDefResult;
        }

        public ActionResult GetAvailableDocManagerCntrl(string userNo="0",string selectedControl="0")
        {
            var availableModuleTree = _accessManager.GetAvailableDocumentManagerCntrl(userNo,selectedControl);
            var moduleResult = Json(availableModuleTree, JsonRequestBehavior.AllowGet);
            return moduleResult;
        }


        public ActionResult SaveUserAccessControl(UserAccessSaveModel modal)
        {
            var savedResponse = _accessManager.SaveUserAccessControl(modal);
            var savedResult = Json(savedResponse, JsonRequestBehavior.AllowGet);
            return savedResult;
        }

        //public ActionResult GetAccessedDocument(string selectedUser)
        //{
        //    var acccessDocument = _accessManager.GetAccessedDocument(selectedUser);
        //    var accessDocumentResult = Json(acccessDocument, JsonRequestBehavior.AllowGet);
        //    return accessDocumentResult;
        //}



        //public ActionResult GetAccessedMasterDefinitionCntrl(string selectedUser)
        //{
        //    var accessDef = _accessManager.GetAccessedMasterDefinitionCntrl(selectedUser);
        //    var accessDefRes = Json(accessDef, JsonRequestBehavior.AllowGet);
        //    return accessDefRes;
        //}



        //public ActionResult SaveUserAndCompany(CompanyBranchSaveModal modal)
        //{
        //    var savedResponse = _accessManager.SaveUserAndCompany(modal);
        //    var jsonResult = Json(savedResponse, JsonRequestBehavior.AllowGet);
        //    return jsonResult;
        //}


        //public ActionResult GetAccessedCompany(string selectedUser)
        //{
        //    var accessedResponse = _accessManager.GetAccessCompanyNBranch(selectedUser);
        //    var result = Json(accessedResponse, JsonRequestBehavior.AllowGet);
        //    return result;
        //}
    }
}