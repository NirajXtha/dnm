using NeoErp.Models;
using System.Collections.Generic;

namespace NeoErp.Services.AccessManager
{
    public interface IAccessManager
    {

        List<DropdownUsers> GetDropdownUsers();
        List<AppModule> GetAppModuleDDL();
        List<UserTreeModel> GetAllUserTree();
        List<AvailableControl> GetAvailableControls(string userNo,string selectedControl);
        List<AvailableControl> GetAvailableCompanyControl(string userNo);
        List<AvailableControl> GetAvailableModuleControl(string userNo,string selectedControl);
        List<AvailableControl> GetMasterSetupListControl(string userNo, string selectedControl);
        List<AvailableControl> GetAvailableMasterDefinitionCntrl(string userNo);
        List<AvailableControl> GetAvailableDocumentManagerCntrl(string userNo,string selectedControl);
        string SaveUserAccessControl(UserAccessSaveModel modal);

        //string SaveUserAndCompany(CompanyBranchSaveModal modal);
        //string UpdateUserAndCompany(CompanyBranchSaveModal modal);

        //List<AvailableControl> GetAccessCompanyNBranch(string selectedUser);
        //List<AvailableModuleModalTree> GetAvailableModuleTree();

        //Method For Fetching All Control




        //List<AvailableControl> GetAccessedDocument(string selectedUser);


        //List<AvailableControl> GetAccessedMasterDefinitionCntrl(string selectedUser);


    }
}
