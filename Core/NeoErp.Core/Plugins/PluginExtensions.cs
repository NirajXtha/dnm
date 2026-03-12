using System;
using System.IO;
using System.Linq;

namespace NeoErp.Core.Plugins
{
    public static class PluginExtensions
    {
        public static string GetLogoUrl(this PluginDescriptor pluginDescriptor, IWebHelper webHelper)
        {
            if (pluginDescriptor == null)
                throw new ArgumentNullException("pluginDescriptor");

            if (webHelper == null)
                throw new ArgumentNullException("webHelper");

            if (pluginDescriptor.OriginalAssemblyFile == null || pluginDescriptor.OriginalAssemblyFile.Directory == null)
            {
                return null;
            }

            var pluginDirectory = pluginDescriptor.OriginalAssemblyFile.Directory.Parent;
            var logoLocalPath = Path.Combine(pluginDirectory.FullName, "logo.jpg");
            if (!File.Exists(logoLocalPath))
            {
                return null;
            }

            string logoUrl = string.Format("/Areas/{1}/logo.jpg", "", pluginDirectory.Name);
            return logoUrl;
        }

        public static bool IsPluginInstalled(string moduleName)
        {
         //   NeoErp.Core.Models.NeoCoreEntity _db = new Models.NeoCoreEntity();
            //var module = _db.Modules.Where(wh => wh.ModuleID == moduleName).FirstOrDefault();
            //if (module == null) return false;
            //if (module.IsInstalled == null) return false;
            //return (bool)module.IsInstalled;
            return true;
        }

        public static bool IsPluginLive(string moduleName)
        {
          //  NeoErp.Core.Models.NeoCoreEntity _db = new Models.NeoCoreEntity();
            //var module = _db.Modules.Where(wh => wh.ModuleID == moduleName).FirstOrDefault();
            //if (module == null) return false;
            //if (module.IsInstalled == null) return false;
            //return (bool)module.IsLive;
            return true;
        }
    }
}
