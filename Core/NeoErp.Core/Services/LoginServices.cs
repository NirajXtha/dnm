using NeoErp.Core.Data;
using NeoErp.Core.Domain;
using NeoErp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Core.Services
{
    public class LoginServices:ILoginServices
    {
        private readonly IDbContext _CommonEntity;

        public LoginServices(IDbContext commonEntity)
        {
            this._CommonEntity = commonEntity;

        }
     public   User GetUser(User user)
        {
            var query_string = @"Select user_no User_id,(select branch_code from fa_branch_setup where branch_code='" + user.branch_code + "') Branch,(select initcap(branch_edesc) branch_edesc from fa_branch_setup where branch_code='" + user.branch_code + "') BranchName,company_code as Company,(select initcap(company_edesc) company_edesc from company_setup) CompanyName from sc_application_users where Upper(login_code)=Upper('" + user.UserName + "') and password='" + user.Password + "'";
           return _CommonEntity.SqlQuery<User>(query_string).FirstOrDefault();
            
        }
        public User GetUserByUserId(string userId)
        {
            var query_string = @"Select user_no User_id";
            return _CommonEntity.SqlQuery<User>(query_string).FirstOrDefault();

        }
    }
}