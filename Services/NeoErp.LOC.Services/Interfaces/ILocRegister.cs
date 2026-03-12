using NeoErp.LOC.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.LOC.Services.Services
{
    public interface ILocRegister
    {
        List<DynamicMenu> GetDynamicMenu(int userId, int level);
        List<DynamicMenu> GetChlidMenu(string menuNo);
        List<DynamicMenu> getmasterDynamicMenuData();
        List<DynamicMenu> getlcDynamicMenuData();
        List<DynamicMenu> getlcReportsDynamicMenuData();
    }
}
