using NeoErp.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Core.Services
{
    public interface IAuthenticationService
    {
        /// <summary>
        /// Sign in
        /// </summary>
        /// <param name="user">UserMode</param>
        /// <param name="createPersistentCookie">A value indicating whether to create a persistent cookie</param>
        void SignIn(User user, bool createPersistentCookie);

        /// <summary>
        /// Sign out
        /// </summary>
        void SignOut();

        /// <summary>
        /// Get authenticated customer
        /// </summary>
        /// <returns>Customer</returns>
        User GetAuthenticatedCustomer();
        IEnumerable<User> GetCompanyList(string UserName);
        IEnumerable<User> GetBranchList( string company_code);
        IEnumerable<BranchInfo> GetCompanyBranchList(string UserName);
        IEnumerable<CompanyBranchModel> GetAllCompanyBranch(string UserName, string Password, string SelectedDatabase);
        IEnumerable<User> SignIn(string UserName, string Password, string company, string branch, string SelectedDatabase);
        void UpdateAuthenticatedCustomer(User user);
        IEnumerable<BranchInfo> GetBranchControlList(string company_code, string username, string SelectedDatabase);

    }
}