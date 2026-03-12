using System;
using System.ComponentModel.DataAnnotations;

namespace NeoErp.Models
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
    public class UserViewModel
    {
        public string SUPER_USER_FLAG { get; set; }
        [DataType(DataType.Password)]
        public string PASSWORD { get; set; }
        public int USER_NO { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string USERNAME { get; set; }
        public string LOGIN_EDESC { get; set; }
        public string LOGIN_NDESC { get; set; }
        public string EMPLOYEE_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string USER_TYPE { get; set; }
        public string FULLNAME { get; set; }
        public string SAVE_FLAG { get; set; }

    }

    public class AddUserModel
    {
        public int USER_NO { get; set; }
        public int PRE_USER_NO { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string LOGIN_CODE { get; set; }
        public string LOGIN_EDESC { get; set; }
        public string PASSWORD { get; set; }
        public string EMPLOYEE_CODE { get; set; }
        public string USER_LOCK_FLAG { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string CREATED_BY { get; set; }
        public string SUPER_USER_FLAG { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string ABBR_CODE { get; set; }
    }


    public class CompanySetupModel
    {
        public string COMPANY_CODE { get; set; }
        public string COMPANY_EDESC { get; set; }
        public string COMPANY_NDESC { get; set; }
        public string ADDRESS { get; set; }
        public string TELEPHONE { get; set; }
        public string EMAIL { get; set; }
        public string FAX { get; set; }
        public string WEB { get; set; }
        public string REMARKS { get; set; }
        public DateTime? VALID_DATE { get; set; }
        public string TPIN_VAT_NO { get; set; }
        public string SMTP_HOST { get; set; }
        public string FOOTER_LOGO_FILE_NAME { get; set; }

        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string SYN_ROWID { get; set; }
        public string CONSOLIDATE_FLAG { get; set; }
        public string ABBR_CODE { get; set; }
        public DateTime? MODIFY_DATE { get; set; }
        public string MODIFY_BY { get; set; }
        public string REGISTRATION_NO { get; set; }
        public string LOGO_FILE_NAME { get; set; }
        public string PRE_COMPANY_CODE { get; set; }


    }

    public class EmployeeModel1
    {
        public string EMPLOYEE_CODE { get; set; }
        public string EMPLOYEE_EDESC { get; set; }
        public string EPERMANENT_ADDRESS1 { get; set; }
        public string COMPANY_EDESC { get; set; }
        public string BRANCH_EDESC { get; set; }
        public string BRANCH_CODES { get; set; } //to retrieve comma seperated
        public string COMPANY_CODES { get; set; }
        public int USER_NO { get; set; }
    }

}