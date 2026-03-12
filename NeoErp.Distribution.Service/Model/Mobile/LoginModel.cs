using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.Mobile
{
    public class LoginModel : CommonRequestModel
    {
        public string Attendance { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Imei { get; set; }
        public string Device_Name { get; set; }
        public string App_Version { get; set; }
        public string Firebase_key { get; set; }
        public string Installed_Apps { get; set; }
    }

    public class MobileLogModel:CommonRequestModel
    {
        public string SP_CODE { get; set; }
        public string SWITCH_STATUS { get; set; }
        public int BATTERY_PERCENT { get; set; }
    }
}
