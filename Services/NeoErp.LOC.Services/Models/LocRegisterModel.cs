using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.LOC.Services.Models
{
    public class LocRegisterModel
    {
       
    }
    public class DynamicMenu
    {
        public string MENU_EDESC { get; set; }

        public string MENU_NO { get; set; }

        public string VIRTUAL_PATH { get; set; }

        public string GROUP_SKU_FLAG { get; set; }

        public string ICON_PATH { get; set; }

        public string COLOR { get; set; }

        public string MODULE_ABBR { get; set; }

        public List<DynamicMenu> Items { get; set; }
    }

    public class LcUploadFileModels
    {
        public int Code { get; set; }
        public string Path { get; set; }
        public string FileName { get; set; }
    }
}