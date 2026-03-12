using NeoErp.Core.Services.CommonSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Models.Mobiles
{
    public class MobileUserInfo
    {
        public string UserName { get; set; }
        public string deviceId { get; set; }
    }
    public class MobileUsers : ISettings
    {
        public List<MobileUserInfo> User { get; set; }
        public MobileUsers()
        {
            User = new List<MobileUserInfo>();
        }
    }
    public class UserModelForMobile
    {
        public string USER_NO { get; set; }
        public string LOGIN_EDESC { get; set; }
        public string LOGIN_CODE { get; set; }
    }
    public class ItemModelForMobileFilter
    {
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
    }
    public class RMPurchaseModel
    {
        public string PURCHASE_RM { get; set; }
        public string MSG_FORMAT { get; set; }
        public string QUERY { get; set; }
        public List<UserModelForMobile> USER_NO { get; set; }
    }
    public class MobileWebLogTaskModel
    {
        public string TASK_ID { get; set; }
        public string TASK_NAME { get; set; }
        public string MESSAGE_FORMAT { get; set; }
        public string DEVICEID { get; set; }
        public int USER_NO { get; set; }
        public string LOGIN_EDESC { get; set; }
        public string TASK_DATE { get; set; }
    }
    public class MobileWebFilterModel
    {
        public string TYPE { get; set; }
        public string ITEM_CODE { get; set; }
    }

}