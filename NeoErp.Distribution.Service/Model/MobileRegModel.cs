using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model
{
    public class MobileRegModel
    {
        public string SP_CODE { get; set; }
        public string FULL_NAME { get; set; }
        public string USER_NAME { get; set; }
        public int USERID { get; set; }
        public string IMEI_NO { get; set; }
        public string DEVICE_NAME { get; set; }
        public string APPROVED_FLAG { get; set; }
        public string ACTIVE { get; set; }
        public string APP_VERSION { get; set; }
        public string CURRENT_LOGIN { get; set; }
    }
}