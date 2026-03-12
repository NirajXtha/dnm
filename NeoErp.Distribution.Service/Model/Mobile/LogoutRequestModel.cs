using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.Mobile
{
    public class LogoutRequestModel:CommonRequestModel
    {
        public string USER_NAME { get; set; }
        public string SP_CODE { get; set; }
    }
}
