using NeoERP.DocumentTemplate.Service.Models.ThirdPartyApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Interface.ThirdPartyApi
{
   public interface IOSPreferenceSetup
    {

        #region  Preference setup
        string SavePreferenceSetup(OS_PREFERENCE_SETUP model);
        string UpdatePreferenceSetup(OS_PREFERENCE_SETUP model);
        List<OS_PREFERENCE_SETUP> GetPreferenceSetup();
        OS_PREFERENCE_SETUP getPreffitem(string prefId);



        #endregion
    }
}
