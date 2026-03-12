using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NeoErp.Core.Domain
{
    public class User
    {
        public User()
        {
            UserGuid = Guid.NewGuid();
        }
        public Guid UserGuid { get; set; }
        [Required(ErrorMessage = "Please enter username")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Please enter password")]
        [DataType(DataType.Password)]
        [Display(Name = "password")]
        public string Password { get; set; }
        public string Branch { get; set; }
        public string Company { get; set; }
        public string company_code { get; set; }
        public string company_name { get; set; }
        public string branch_code { get; set; }
        public string branch_name { get; set; }
        public int User_id { get; set; }
        public string login_code { get; set; }
        public DateTime startFiscalYear { get; set; }
        public DateTime endfiscalyear { get; set; }
        public string LOGIN_EDESC { get; set; }
        public int USER_NO { get; set; }
        public string UserType { get; set; }

        public string DistributerNo { get; set; }
        public string LoginType { get; set; } = "Synergy";
        public string EMPLOYEE_CODE { get; set; }
        public string sp_codes { get; set; }
        //public LoginViewModel databaseName { get; set; }
        public string SelectedDatabase { get; set; } = null;
    }

    public class BranchInfo
    {
        public string branch_code { get; set; }
        public string branch_edesc { get; set; }
        public string branch_name { get; set; }
        public string address { get; set; }
        public string telephone_no { get; set; }
        public string email { get; set; }
        public string pre_branch_code { get; set; }
        public string group_sku_flag { get; set; }
        public string company_code { get; set; }
        public string company_edesc { get; set; }
    }

    public class CompanyBranchModel : User
    {
        public string company_edesc { get; set; }
        public string branch_edesc { get; set; }
        public string group_sku_flag { get; set; }

    }
    #region Multiple Database 
    public class DatabaseConfig
    {
        public List<DatabaseInfo> databases { get; set; }
    }

    public class DatabaseInfo
    {
        public string dbUserName { get; set; }
        public string dbPassword { get; set; }
        public string service { get; set; }
        public string host { get; set; }
        public int port { get; set; }
        public string name { get; set; }
        public string active { get; set; }
    }
    //public class LoginViewModel
    //{
    //    public string SelectedDatabase { get; set; }
    //}
    #endregion
}