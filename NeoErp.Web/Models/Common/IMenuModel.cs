using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Models.Common
{
    public interface IMenuModel
    {
        string GetModule();
        string GetMenu(string ModuleCode);
        List<Menus> GetMenuList();
    }
}