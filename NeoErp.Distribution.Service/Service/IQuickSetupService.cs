using NeoErp.Core.Domain;
using NeoErp.Distribution.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Service
{
    public interface IQuickSetupService
    {
        string CreateDistResellerMap(User userInfo);
        int createEntities();
        int CreateDefaultValues();
        List<TroubleshootModel> GetTroubleshoot(User UserInfo);
        List<BrandItemModel> GetBrandItem(User userInfo);
        string UpdateCreatedByNameReseller(string Company_code);
        string RemoveDuplicateRoutes(string Company_code);

    }
}
