using NeoErp.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Core.Repository
{
    interface IModuleService
    {
        string SaveModuleDetail(PluginDescriptor pluginDescription);
        int DeleteModuleByUninstall(PluginDescriptor pluginDescription);
        
    }
}
