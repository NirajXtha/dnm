using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Core.Models.CustomModels
{
    public class MenuControlEntities
    {
        public string id { get; set; }
        public string MODULE_EDESC { get; set; }
        public string label { get; set; }
        public string USERNO { get; set; }
        public string USER_NO { get; set; }
        public string MENU_NO { get; set; }
        public string MENUNO { get; set; }
        public string MENU_EDESC { get; set; }
        public string LOGIN_EDESC { get; set; }
        public string COMPANY_CODE { get; set; }
        public string COMPANY_EDESC { get; set; }
        public string BRANCH_CODE { get; set; }
        public string ACCESS_FLAG { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }


    }
    public class ModuleModel
    {
        public string module_code { get; set; }
        public string module_name { get; set; }
    }
    public class UserModel
    {
        public string user_id { get; set; }
        public string user_name { get; set; }
    }
    public class MenuModel
    {
        public string menu_id { get; set; }
        public string menu_name { get; set; }
    }
    public class MenuOrderModels
    {
        public string MENU_NO { get; set; }
        public string ORDER { get; set; }
    }
}