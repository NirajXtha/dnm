using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Core.Services
{
    public interface IMenuSettings
    {
        List<MenuSettingsEntities> GetMenuForSettings();

        string getChangeMenuOrder(List<MenuOrderModels> modal);

        MenuSettingsEntities GetMenuForSettings(string menuNo);

        string UpdateMenuForSettings();

        string UpdateMenuForSettings(MenuSettingsEntities menu);

        string CreateMenuForSettings();

        string CreateMenuForSettings(MenuSettingsEntities menu);

        string DeleteMenuForSettings();

        string DeleteMenuForSettings(string MenuNo);

        List<ModuleEntities> GetModuleForMenu();

        List<MenuSettingsEntities> GetPreMenu();

        List<MenuSetupModel> MenuListAllNodes();

        List<MenuSetupModel> GetMenuListByMenuNo(string level, string masterCode);

        List<SammyEntity> GetMenuRouting(string moduleCode);

        List<MenuItemModel> GetMenuList();
    }
}
