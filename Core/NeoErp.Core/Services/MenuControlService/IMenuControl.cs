using NeoErp.Core.Models;
using NeoErp.Core.Models.CustomModels;
using System.Collections.Generic;

namespace NeoErp.Core.Services.MenuControlService
{
    public interface IMenuControl
    {   
        /// <summary>
        /// Get All Menu Control list. 
        /// </summary>
        /// <returns></returns>
        List<MenuControlEntities> GetAllMenuControls();

        List<MenuControlEntities> GetAllMenuControl(ReportFiltersModel reportfilters);

        List<MenuControlEntities> GetMenus();
        List<ModuleModel> GetAllModuleByFilter(string filter);
        List<UserModel> GetAllUserByFilter(string filter);
        List<MenuModel> GetAllMenuByFilter(string filter);
        List<MenuControlEntities> GetUsers();
        
        MenuControlEntities GetUserWiseBulkMenu(string UserNo);

        MenuControlEntities GetMenuControls(string menuNo, string UserNo);

        string AddNewMenuControl(MenuControlEntities menuControl);

        string AddBulkMenuUser(MenuControlEntities menuControl);

        string DeleteMenuControl(string menuNo, string userNo);

        string UpdateMenuControl(MenuControlEntities menuControl);
    }
}
