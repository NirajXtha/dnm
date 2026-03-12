using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoErp.Models.Mobiles;

namespace NeoErp.Services.MobileWeb
{
    public interface IMobileWeb
    {
        string saveUserWiseTaskFromat(RMPurchaseModel model);
        string saveBRTaskMsgFormat(RMPurchaseModel model);
        List<UserModelForMobile> getAllUsers(string filter);
        List<MobileWebLogTaskModel> GetAllMobileWebLog();
        string saveFilterDataForMobile(MobileWebFilterModel model);
        List<ItemModelForMobileFilter> getAllItems(string filter);
        List<ItemModelForMobileFilter> getAllSavedItems();
    }
}
