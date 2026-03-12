
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoErp.Core.Models.CustomModels.SettingsEntities;

namespace NeoErp.Core.Services.QuickCapSettingService
{
    public interface IQuickCapSetting
    {
        List<QuickCapSettingEntities> GetAllQuickCap();

        List<QuickCapSettingEntities> GetUsers();

        List<QuickCapSettingEntities> GetUserForQuickCapByID(int ID);

        int AddBulkUserForQuickCap(string USERNO, int ID);
    }
}
