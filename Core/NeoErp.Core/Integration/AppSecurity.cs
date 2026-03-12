using NeoErp.Core.Models;
using System.Linq;
using System.Web;

namespace NeoErp.Core.Integration
{

    public class AppSecurity
    {
       
        #region Login Details

        public static LoginDetailsModel loginDetail
        {
            get
            {
                var data = (LoginDetailsModel)_Session.GetSession("loginDetail");
                if (data != null)
                    return data;
                else
                    return new LoginDetailsModel();                
            }
            set
            {
                _Session.AddSession("loginDetail", value);
            }
        }
               
        public static Enums.UserGroupTypes GetGroupTypeID(string GroupType)
        {
            Enums.UserGroupTypes GroupTypeID;
            if (GroupType != null)
            {
                if (GroupType.Trim() == "A")
                    GroupTypeID = Enums.UserGroupTypes.Admin;
                else if (GroupType.Trim() == "S")
                    GroupTypeID = Enums.UserGroupTypes.SuperAdmin;
                else if (GroupType.Trim() == "U")
                    GroupTypeID = Enums.UserGroupTypes.User;
                else if (GroupType.Trim() == "V")
                    GroupTypeID = Enums.UserGroupTypes.Supervisor;
                else if (GroupType.Trim() == "E")
                    GroupTypeID = Enums.UserGroupTypes.Employee;
                else
                    GroupTypeID = Enums.UserGroupTypes.Guest;
            }
            else
            {
                GroupTypeID = Enums.UserGroupTypes.Employee;
            }
            return GroupTypeID;
        }

        public static void LogOut()
        {
            _Session.RemoveSession("loginDetail");
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
        }             
        
        public static bool HasAccount(string userName)
        {
            //NeoCoreEntity db = new NeoCoreEntity();
            var usr = true; /*db.EmpLogin.Where(el => el.LoginID == userName).ToList();*/
            if (usr)
                return true;
            else
                return false;
        }        
        
        #endregion

           

    }

}