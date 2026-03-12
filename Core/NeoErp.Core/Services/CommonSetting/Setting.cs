using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Core.Services.CommonSetting
{
    public partial class Setting 
    {
        public Setting() { }

        public Setting(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public string Value { get; set; }

       
   

        public override string ToString()
        {
            return Name;
        }
    }

    public class settinglist
    {
       public List<Setting> setting { get; set; }
        public settinglist()
        {
            setting = new List<Setting>();
        }
    }
}