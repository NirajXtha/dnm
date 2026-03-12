using NeoErp.Core.Models.CustomModels;
using NeoErp.Models.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Services
{
    public interface IMobileNotificationService
    {
        List<MobileDataVoucherModel> getNotificationStatus(string userId, string lastNotifyId);
        IEnumerable<MobileDataVoucherModel> GetVoucherWithFlag(int userId, string moduleCode = "", string append = "top", int sessionRowId = 0);
        List<MobileWebLogTaskModel> GetUserMsgFromWeb(string userId,string taskName);
        string GetUserMsgFromWebLog(string userId, string taskName, string taskDate);
        void LogFileInDatabase(string userId, string message, string deviceId, string taskName, string idVal);
        List<PURCHASE_RM_MODEL> GetUniqIdFromQry(string userId, string taskName);
        RMPurchaseModel getMessageByBankID(string userId,string bankId);
    }
}
