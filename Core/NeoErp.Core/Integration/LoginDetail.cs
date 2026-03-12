using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nimble.Core.Models;


namespace Nimble.Core.Integration
{
    public class LoginDetail
    {
        public static bool IsAuthenticated
        {
            get
            {
                LoginDetailsModel lm = (LoginDetailsModel)_Session.GetSession("loginDetail");
                if (lm != null)
                    return lm.IsAuthenticated;
                else
                    return false;
            }
        }

        public static string EmpCode
        {
            get
            {
                LoginDetailsModel lm = (LoginDetailsModel)_Session.GetSession("loginDetail");
                if (lm != null)
                    return lm.EmpCode;
                else
                    return "";
            }
        }

        public static string EmpID
        {
            get
            {
                LoginDetailsModel lm = (LoginDetailsModel)_Session.GetSession("loginDetail");
                if (lm != null)
                    return lm.EmpID;
                else
                    return "";
            }
        }

        public static string LoginID
        {
            get
            {
                LoginDetailsModel lm = (LoginDetailsModel)_Session.GetSession("loginDetail");
                if (lm != null)
                    return lm.LoginID;
                else
                    return "";
            }
        }

        public static Nullable<System.DateTime> LastLoginTime
        {
            get
            {
                LoginDetailsModel lm = (LoginDetailsModel)_Session.GetSession("loginDetail");
                if (lm != null)
                    return lm.LastLoginTime;
                else
                    return null;
            }
        }

        public static Enums.UserGroupTypes UserGroupType
        {
            get
            {
                LoginDetailsModel lm = (LoginDetailsModel)_Session.GetSession("loginDetail");
                if (lm != null)
                    return (Enums.UserGroupTypes)lm.UserType;
                else
                    return Enums.UserGroupTypes.Guest;
            }
        }

        public static string EmpName
        {
            get
            {
                LoginDetailsModel lm = (LoginDetailsModel)_Session.GetSession("loginDetail");
                if (lm != null)
                    return lm.EmpName;
                else
                    return "";
            }
        }

        public static string RoleName
        {
            get
            {
                LoginDetailsModel lm = (LoginDetailsModel)_Session.GetSession("loginDetail");
                if (lm != null)
                    return lm.RoleName;
                else
                    return "";
            }
        }

        public static int LoginSouce
        {
            get
            {
                LoginDetailsModel lm = (LoginDetailsModel)_Session.GetSession("loginDetail");
                if (lm != null)
                    return lm.LoginSource;
                else
                    return 0;
            }
        }

        public static int RoleID
        {
            get
            {
                LoginDetailsModel lm = (LoginDetailsModel)_Session.GetSession("loginDetail");
                if (lm != null)
                    return lm.RoleID;
                else
                    return 0;
            }
        }

        public string ValidIPs
        {
            get
            {
                LoginDetailsModel lm = (LoginDetailsModel)_Session.GetSession("loginDetail");
                if (lm != null)
                    return lm.ValidIPs;
                else
                    return "";
            }
        }

        public static DataTable tblUser
        {
            get
            {
                DataTable tbl = new DataTable();
                tbl = (DataTable)_Session.GetSession("tblUser");
                return tbl;
            }
            set
            {
                _Session.AddSession("tblUser", value);
            }
        }

        public static List<MenuPermissionModel> menu
        {
            get
            {                
               var tbl = (List<MenuPermissionModel>)_Session.GetSession("menu");
                return tbl;
            }
            set
            {
                _Session.AddSession("menu", value);
            }
        }

    }

}