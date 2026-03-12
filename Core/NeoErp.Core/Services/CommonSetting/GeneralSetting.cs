using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Core.Services.CommonSetting
{
    public class GeneralSetting:ISettings
    {
        public string EnableSchedular { get; set; }
        
    }
    public class UserDashboardSetting : ISettings
    {
        public string DefaultPath { get; set; }
        public int UserId { get; set; }

    }
    public class WebPrefrenceSetting : ISettings
    {
        public string Userid { get; set; }
        public string ShowAdvanceSearch { get; set; }
        public string ShowAdvanceAutoComplete { get; set; }
        public string UseSequenceInTransaction { get; set; } = "false";
        public string ShowSYcButtonInView { get; set; } = "false";
        public string IrdUrl { get; set; } = "";
        public string Username { get; set; }
        public string Password { get; set; }
        public string SellerPan { get; set; }
    }
}