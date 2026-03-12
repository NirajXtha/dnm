using NeoErp.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Core.Integration
{
    interface INimbleSettings
    {
       // EmailSetting Email { get; set; }
        //CompanyInfoDisplay Client { get; set; }
        LicenseModel License { get; set; }
        AppSettingsModel App { get; set; }
        //Settings setting { get; set; }
        //FiscalYearSettings FYearSettings { get; set; }
        GroupAlises Groups { get; set; }       

        NimbleSettings ReadSettings();
    }
}
