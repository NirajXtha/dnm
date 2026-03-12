

using NeoErp.Models;
using System.Collections.Generic;

namespace NeoErp.Services.UserService
{
    public interface IUserService
    {
        string ChangePassword(ChangePasswordViewModel model);
        string ChangeUserPassword(ChangePasswordViewModel model);
        string InsertChangeUserPassword(UserViewModel model);
        List<UserViewModel> GetAllUserDetails(UserViewModel model);
        UserViewModel GetUserDetailsByUserNo(int userno);
        List<UserViewModel> GetAllUserType();


        #region User Setup    
        List<AddUserModel> GetUserList();
        List<CompanySetupModel> getAllCompany();
        List<EmployeeModel1> getAllEmployeeList();
        string createUser(AddUserModel model);
        string updateUser(AddUserModel model);
        string DeleteUserFromDb(string usercode);
        #endregion 
    }
}
