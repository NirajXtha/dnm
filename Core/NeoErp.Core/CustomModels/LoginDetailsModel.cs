using NeoErp.Core.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Core.Models
{
    public class LoginDetailsModel
    {
        public LoginDetailsModel()
        {
            List<MenuPermissionModel> menu = new List<MenuPermissionModel>();
            IsAuthenticated = false;
            EmpCode = string.Empty;
            EmpName = string.Empty;
            EmpID = string.Empty;

            UserName = string.Empty;
            Email = string.Empty;

            RoleID = 0;
            RoleName = string.Empty;
            ValidIPs = string.Empty;
            LoginSource = 0;

            GroupType = Enums.UserGroupTypes.Guest; //6: Guest
            GroupTypeName = "G";
            PermissionLevel = string.Empty;     
        }
        
        public string LoginID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string EmpID { get; set; }

        public Nullable<System.DateTime> LastLoginTime { get; set; }
        public Nullable<System.DateTime> LastLogoutTime { get; set; }

        
        public bool IsAuthenticated { get; set; }       
        public int LoginSource { get; set; }        
        public string ValidIPs { get; set; }

        /// <summary>
        /// Identify user group type id, integer value for Group Type Name
        /// </summary>
        public Enums.UserGroupTypes GroupType{ get; set; }        

        /// <summary>
        /// Identitfy user group type : ie. S: Super, A: Admin, U: User, V: Suervisor, E: Employee, G: Guest
        /// </summary>
        public string GroupTypeName { get; set; }

        /// <summary>
        /// Permission level to display records, ie. A: All, D: Department, B: Branch, G: Assigned Group from Group Permission Assign.
        /// </summary>
        public string PermissionLevel { get; set; }

        /// <summary>
        /// UserGroupID
        /// </summary>
        public int RoleID { get; set; }

        /// <summary>
        /// UserGroupName
        /// </summary>
        public string RoleName { get; set; }


        /// <summary>
        /// to store allowed menus for currently logged in user.
        /// </summary>
        public  List<MenuPermissionModel> menu { get; set; }

       


    }
}
