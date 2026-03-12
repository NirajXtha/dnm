using NeoErp.Core.Domain;
using NeoErp.Core.Models;
using NeoErp.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace NeoErp.Core.Services
{
    public class FormsAuthenticationService : IAuthenticationService
    {
        #region Fields

        private readonly HttpContextBase _httpContext;

        private readonly TimeSpan _expirationTimeSpan;
        private readonly NeoErpCoreEntity _objectEntity;

        private User _cachedUser;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <param name="customerService">Customer service</param>
        /// <param name="customerSettings">Customer settings</param>
        public FormsAuthenticationService(HttpContextBase httpContext, NeoErpCoreEntity objectEntity)
        {
            this._httpContext = httpContext;
            this._expirationTimeSpan = FormsAuthentication.Timeout;
           // this._objectEntity = objectEntity;
            this._objectEntity = !String.IsNullOrWhiteSpace(ConnectionManager.CurrentSelectedDatabase) ? new NeoErpCoreEntity(ConnectionManager.CurrentSelectedDatabase) : objectEntity;

        }
        private ObjectContext GetDatabaseContext(string selectedDatabase = null)
        {
            string databaseToUse = selectedDatabase ?? ConnectionManager.CurrentSelectedDatabase;
            return ConnectionManager.GetDbContext(databaseToUse);
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get authenticated customer
        /// </summary>
        /// <param name="ticket">Ticket</param>
        /// <returns>Customer</returns>
        protected virtual User GetAuthenticatedCustomerFromTicket(FormsAuthenticationTicket ticket)
        {

            if (ticket == null)
                throw new ArgumentNullException("ticket");

            var userString = ticket.UserData;
            var user = userString.Split('#');
            var Currentuser = new User();
            if (user.Length > 0)
            {
                //0: userid
                //1:company_code
                //2:branch_code
                //3: StartFinc
                //4: EndFinc
                //5:BranchName
                //6:CompanyName
                //7:loginName
                //  var Currentuser =new User();

                Currentuser.branch_code = user[2].ToString();
                Currentuser.Branch = user[2].ToString();
                Currentuser.company_code = user[1].ToString();
                Currentuser.Company = user[1].ToString();
                Currentuser.User_id = Convert.ToInt32(user[0]);
                Currentuser.startFiscalYear = Convert.ToDateTime(user[3]);
                Currentuser.endfiscalyear = Convert.ToDateTime(user[4]);
                Currentuser.branch_name = user[5].ToString();
                Currentuser.company_name = user[6].ToString();
                Currentuser.login_code = user[7].ToString();
                Currentuser.DistributerNo = user[8].ToString();
                Currentuser.LOGIN_EDESC = user[9].ToString();
                Currentuser.LoginType = user[10].ToString();

                var spCode = string.Empty;
                var path = HttpContext.Current.Server.MapPath(@"~/App_Data/SpCode_" + Currentuser.User_id + "_" + Currentuser.company_code + ".txt");
                if (System.IO.File.Exists(path))
                {
                    using (System.IO.StreamReader readtext = new System.IO.StreamReader(path))
                    {
                        spCode = readtext.ReadLine();
                    }
                }
                Currentuser.sp_codes = spCode;//user[11].ToString();
            }

            //if (String.IsNullOrWhiteSpace(userId))
            //    return null;

            // string selectquery = (@"SELECT sca.LOGIN_CODE, cs.COMPANY_EDESC, cs.COMPANY_CODE,  (select END_DATE from(select * from hr_fiscal_year_code  order by CREATED_DATE desc )  where ROWNUM <= 1) endfiscalyear,sca.LOGIN_CODE FROM sc_application_users sca inner join sc_company_control  scc on sca.user_no = scc.user_no inner join company_setup cs on SCC.COMPANY_CODE = cs.COMPANY_CODE wHERE sca.user_no = '" + userId + "'");
            //  string selectquery = (@"SELECT sca.LOGIN_CODE, cs.COMPANY_EDESC, cs.COMPANY_CODE, (select END_DATE from(select * from hr_fiscal_year_code  order by CREATED_DATE desc )  where ROWNUM <= 1) endfiscalyear,(select START_DATE from( select * from hr_fiscal_year_code order by CREATED_DATE desc )  where ROWNUM <= 1) startFiscalYear,sca.LOGIN_CODE FROM sc_application_users sca inner join sc_company_control  scc on sca.user_no = scc.user_no inner join company_setup cs on SCC.COMPANY_CODE = cs.COMPANY_CODE wHERE sca.user_no = '" + userId + "'");
            // var userinfo = _objectEntity.SqlQuery<User>(selectquery).FirstOrDefault();
            return Currentuser;



            //var userDb = _objectEntity.SC_APPLICATION_USERS.FirstOrDefault(x => x.USER_NO == userId);
            //User userinfo = new User();
            //if (userDb != null)
            //{

            //    userinfo.company_code = userDb.COMPANY_CODE;
            //    userinfo.UserName = userDb.LOGIN_EDESC;
            //    userinfo.UserId = userDb.USER_NO;
            //    userinfo.company_name = _objectEntity.COMPANY_SETUP.FirstOrDefault(x => x.COMPANY_CODE == userDb.COMPANY_CODE) == null ? "" : _objectEntity.COMPANY_SETUP.FirstOrDefault(x => x.COMPANY_CODE == userDb.COMPANY_CODE).COMPANY_EDESC;
            //    userinfo.UserId = Convert.ToInt16(userId);

            //    var branch = _objectEntity.SC_BRANCH_CONTROL.FirstOrDefault(x => x.USER_NO == userId);
            //    if (branch != null)
            //    {
            //        //  userinfo.branch_name = branch.b;
            //        userinfo.branch_code = branch.BRANCH_CODE;
            //    }




            //}
            // return userinfo;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sign in
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="createPersistentCookie">A value indicating whether to create a persistent cookie</param>
        public virtual void SignIn(User user, bool createPersistentCookie)
        {
            //FormsAuthentication.SignOut();
            var now = DateTime.UtcNow.ToLocalTime();
            //0: userid
            //1:company_code
            //2:branch_code
            //3: StartFinc
            //4: EndFinc
            //5:BranchName
            //6:CompanyName
            //7:loginName
            if (!string.IsNullOrEmpty(user.sp_codes))
            {
                var path = HttpContext.Current.Server.MapPath(@"~/App_Data/SpCode_" + user.User_id + "_" + user.company_code + ".txt");
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                if (!System.IO.File.Exists(path))
                {
                    using (var tw = new System.IO.StreamWriter(path, true))
                    {
                        tw.Flush();
                        tw.WriteLine(user.sp_codes);
                    }
                }
            }
            var userdata = string.Format("{0}#{1}#{2}#{3}#{4}#{5}#{6}#{7}#{8}#{9}#{10}", user.User_id, user.Company, user.Branch,
                user.startFiscalYear.ToString(), user.endfiscalyear.ToString(), user.branch_name, user.company_name, user.UserName, user.DistributerNo, user.LOGIN_EDESC, user.LoginType);
            var ticket = new FormsAuthenticationTicket(
                1 /*version*/,
               user.UserName,
                now,
                now.Add(_expirationTimeSpan),
                createPersistentCookie,
                userdata.ToString(),
                FormsAuthentication.FormsCookiePath);

            var encryptedTicket = FormsAuthentication.Encrypt(ticket);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            cookie.HttpOnly = true;
            if (ticket.IsPersistent)
            {
                cookie.Expires = ticket.Expiration;
            }
            cookie.Secure = FormsAuthentication.RequireSSL;
            cookie.Path = FormsAuthentication.FormsCookiePath;
            if (FormsAuthentication.CookieDomain != null)
            {
                cookie.Domain = FormsAuthentication.CookieDomain;
            }

            _httpContext.Response.Cookies.Add(cookie);
            _cachedUser = user;
        }

        /// <summary>
        /// Sign out
        /// </summary>
        public virtual void SignOut()
        {
            _cachedUser = null;
            FormsAuthentication.SignOut();
        }

        /// <summary>
        /// Get authenticated customer
        /// </summary>
        /// <returns>Customer</returns>
        public virtual User GetAuthenticatedCustomer()
        {
            if (_cachedUser != null)
                return _cachedUser;

            if (_httpContext == null ||
                _httpContext.Request == null ||
                !_httpContext.Request.IsAuthenticated ||
                !(_httpContext.User.Identity is FormsIdentity))
            {
                return null;
            }

            var formsIdentity = (FormsIdentity)_httpContext.User.Identity;
            var customer = GetAuthenticatedCustomerFromTicket(formsIdentity.Ticket);

            _cachedUser = customer;
            return _cachedUser;
        }


        IEnumerable<User> IAuthenticationService.GetCompanyList(string UserName)
        {
            string SelectQuery = @"SELECT company_code, initcap(company_edesc) company_name from company_setup where company_code in(select company_code from sc_company_control where user_no in (select user_no from sc_application_users where Upper(login_code)=Upper('" + UserName + "')))";

            string selectedDb = HttpContext.Current.Session["SelectedDbKey"]?.ToString();
            using (var db = GetDatabaseContext(selectedDb))
            {
                return db.ExecuteStoreQuery<User>(SelectQuery).ToList();
            }
            //var user = this._objectEntity.SqlQuery<User>(SelectQuery);
            //return user;
        }
        IEnumerable<BranchInfo> IAuthenticationService.GetCompanyBranchList(string UserName)
        {
            string SelectQuery = @"select bs.branch_code,bs.branch_edesc,bs.address,bs.telephone_no,bs.email,bs.pre_branch_code,bs.group_sku_flag,c.company_code,c.company_edesc
 from fa_branch_setup bs ,company_setup c where bs.deleted_flag='N' 
and bs.company_code=c.company_code
and bs.company_code in(select company_code from sc_company_control where user_no in (select user_no from sc_application_users where Upper(login_code)=Upper('" + UserName + "')))";

            string selectedDb = HttpContext.Current.Session["SelectedDbKey"]?.ToString();
            using (var db = GetDatabaseContext(selectedDb))
            {
                return db.ExecuteStoreQuery<BranchInfo>(SelectQuery).ToList();
            }
            //var user = this._objectEntity.SqlQuery<BranchInfo>(SelectQuery);
            //return user;
        }

        IEnumerable<CompanyBranchModel> IAuthenticationService.GetAllCompanyBranch(string UserName, string Password, string SelectedDatabase)
        {
            string SelectQuery = $@"select bs.branch_code,bs.branch_edesc,bs.address,bs.telephone_no,bs.email,bs.pre_branch_code,bs.group_sku_flag,c.company_code,c.company_edesc
                from fa_branch_setup bs ,company_setup c where bs.deleted_flag='N' 
                and bs.company_code=c.company_code
                and c.company_code in(select company_code from sc_company_control where user_no in (select user_no from sc_application_users where Upper(login_code)=Upper('{UserName}') and (FN_DECRYPT_PASSWORD(password)='{Password}' or password='{Password}' )))
                and bs.branch_code in (select branch_code from SC_BRANCH_CONTROL where user_no in (select user_no from sc_application_users where Upper(login_code)=Upper('{UserName}') and (FN_DECRYPT_PASSWORD(password)='{Password}' or password='{Password}' ))) order by  bs.branch_code";
            // var result = this._objectEntity.SqlQuery<CompanyBranchModel>(SelectQuery);
            using (var db = ConnectionManager.GetDbContext(SelectedDatabase))
            {
                // var result = db.SqlQuery<CompanyBranchModel>(SelectQuery).ToList();
                var result = db.ExecuteStoreQuery<CompanyBranchModel>(SelectQuery).ToList();
                return result;
            }
            // return result;
        }

        IEnumerable<User> IAuthenticationService.GetBranchList(string company_code)
        {
            string SelectQuery = @"SELECT branch_code, initcap(branch_edesc) branch_name from fa_branch_setup 
                                   where company_code=" + company_code.Replace("\"", " ").Trim() + @" and deleted_flag= 'N' and branch_code<>company_code";

            var user = this._objectEntity.SqlQuery<User>(SelectQuery);
            return user;
        }
        IEnumerable<BranchInfo> IAuthenticationService.GetBranchControlList(string company_code, string username, string SelectedDatabase)
        {
            var stringQuery = @"select bs.branch_code,bs.branch_edesc as branch_name,bs.address,bs.telephone_no,bs.email,bs.pre_branch_code,bs.group_sku_flag,c.company_code,c.company_edesc
 from fa_branch_setup bs ,company_setup c where bs.deleted_flag='N' 
and bs.company_code=c.company_code
and BS.COMPANY_CODE=" + company_code.Replace("\"", " ").Trim() + @"
and  BS.branch_code<>BS.company_code
and bs.branch_code in(select branch_code from SC_BRANCH_CONTROL where user_no in (select user_no from sc_application_users where Upper(login_code)=Upper('" + username + "')))";
            //string SelectQuery = @"SELECT branch_code, initcap(branch_edesc) branch_name from fa_branch_setup 
            //                       where company_code=" + company_code.Replace("\"", " ").Trim() + @" and deleted_flag= 'N' and branch_code<>company_code";

            var user = this._objectEntity.SqlQuery<BranchInfo>(stringQuery);
            return user;
        }
        IEnumerable<User> IAuthenticationService.SignIn(string UserName, string Password, string company, string branch, string SelectedDatabase)
        {
            // string SelectQuery = @"Select user_no User_id,(select branch_code from fa_branch_setup where branch_code='" + branch + "') Branch,(select initcap(branch_edesc) branch_edesc from fa_branch_setup where branch_code='" + branch + "') branch_name,company_code as Company,(select initcap(company_edesc) company_edesc from company_setup) company_name, from sc_application_users where Upper(login_code)=Upper('" + UserName + "') and password='" + Password + "'";
            //string SelectQuery = @"Select user_no User_id,(select branch_code from fa_branch_setup where branch_code='" + branch + "' and company_code='+company+') Branch,(select initcap(branch_edesc) branch_edesc from fa_branch_setup where branch_code='" + branch + "' and company_code='"+company+"') branch_name,company_code as Company,(select initcap(company_edesc) company_edesc from company_setup where company_code='"+company+"') company_name, (select END_DATE from(select * from hr_fiscal_year_code where company_code='"+company+"'  order by CREATED_DATE desc )  where ROWNUM <= 1) endfiscalyear,  (select START_DATE from( select * from hr_fiscal_year_code where  company_code='"+company+"' order by CREATED_DATE desc )  where ROWNUM <= 1) startFiscalYear from sc_application_users where Upper(login_code)=Upper('" + UserName + "') and FN_DECRYPT_PASSWORD(password)='" + Password + "' and  company_code='"+company+"'";
            //string SelectQuery = @"Select user_no User_id,(select branch_code from fa_branch_setup where branch_code='" + branch + "' and company_code='+company+') Branch,(select initcap(branch_edesc) branch_edesc from fa_branch_setup where branch_code='" + branch + "' and company_code='"+company+"') branch_name,company_code as Company,(select initcap(company_edesc) company_edesc from company_setup where company_code='"+company+"') company_name, (select END_DATE from(select * from hr_fiscal_year_code where company_code='"+company+"'  order by CREATED_DATE desc )  where ROWNUM <= 1) endfiscalyear,  (select START_DATE from( select * from hr_fiscal_year_code where  company_code='"+company+"' order by CREATED_DATE desc )  where ROWNUM <= 1) startFiscalYear from sc_application_users where Upper(login_code)=Upper('" + UserName + "') and FN_DECRYPT_PASSWORD(password)='" + Password + "' and  company_code='"+company+"'";

            var selectQuery = $@"select a.user_no User_id,A.EMPLOYEE_CODE,b.branch_code Branch_code,initcap(FN_FETCH_DESC(c.company_code,'COMPANY_SETUP',c.company_code)) company_name,
 initcap(FN_FETCH_DESC(b.company_code,'FA_BRANCH_SETUP',b.branch_code)) branch_name,c.company_code as Company,
  (select END_DATE from(select * from hr_fiscal_year_code where company_code='01'  order by CREATED_DATE desc )  where ROWNUM <= 1) endfiscalyear, 
 (select START_DATE from( select * from hr_fiscal_year_code where  company_code='01' order by CREATED_DATE desc )  where ROWNUM <= 1) startFiscalYear
  from sc_application_users a ,SC_BRANCH_CONTROL b,sc_company_control  c
 where Upper(a.login_code)=Upper('{UserName}') and (FN_DECRYPT_PASSWORD(a.password)='{Password}' or a.password='{Password}' )
 and b.company_code=a.company_code
 and c.company_code=a.company_code
 and a.company_code='{company}'
 and a.user_no=b.user_no
 and a.deleted_flag='N'
 and c.user_no=a.user_no
 and b.branch_code='{branch}'";

            //   string SelectQuery = @"Select user_no User_id,(select branch_code from fa_branch_setup where branch_code='" + branch + "') Branch_code,(select initcap(branch_edesc) branch_edesc from fa_branch_setup where branch_code='" + branch + "' and company_code='" + company + "') branch_name,company_code as Company,(select initcap(company_edesc) company_edesc from company_setup where company_code='" + company + "') company_name, (select END_DATE from(select * from hr_fiscal_year_code where company_code='" + company + "'  order by CREATED_DATE desc )  where ROWNUM <= 1) endfiscalyear,  (select START_DATE from( select * from hr_fiscal_year_code where  company_code='" + company + "' order by CREATED_DATE desc )  where ROWNUM <= 1) startFiscalYear from sc_application_users where Upper(login_code)=Upper('" + UserName + "') and FN_DECRYPT_PASSWORD(password)='" + Password + "' and  company_code='" + company + "'";
            //var user = this._objectEntity.SqlQuery<User>(selectQuery).ToList();
            string selectedDb = HttpContext.Current.Session["SelectedDbKey"]?.ToString();
            List<User> user = null;
            using (var db = ConnectionManager.GetDbContext(selectedDb))
            {
                user = db.ExecuteStoreQuery<User>(selectQuery).ToList();
                var userData = new User();
                if (user.Count > 0)
                {
                    userData = user.First();

                    userData.UserName = UserName;
                }

                //for distribution user Filter
                List<string> sp_codes = new List<string>();
                try
                {
                    // var superFlag = _objectEntity.SqlQuery<string>($"SELECT SUPER_USER_FLAG FROM SC_APPLICATION_USERS WHERE USER_NO='{userData.User_id}' AND COMPANY_CODE = '{company}'").FirstOrDefault();
                    var superFlag = db.ExecuteStoreQuery<string>($"SELECT SUPER_USER_FLAG FROM SC_APPLICATION_USERS WHERE USER_NO='{userData.User_id}' AND COMPANY_CODE = '{company}'").FirstOrDefault();

                    if (superFlag != "Y")
                    {
                        //var pref = _objectEntity.SqlQuery<string>($"SELECT SQL_SP_FILTER FROM DIST_PREFERENCE_SETUP WHERE COMPANY_CODE = '{company}'").FirstOrDefault();
                        var pref = db.ExecuteStoreQuery<string>($"SELECT SQL_SP_FILTER FROM DIST_PREFERENCE_SETUP WHERE COMPANY_CODE = '{company}'").FirstOrDefault();

                        if (pref == "Y")
                        {
                            sp_codes = db.ExecuteStoreQuery<string>($@"SELECT SP_CODE FROM DIST_LOGIN_USER 
                                START WITH PARENT_USERID = (SELECT USERID FROM DIST_LOGIN_USER WHERE SP_CODE = '{userData.EMPLOYEE_CODE}' AND ROWNUM = 1)
                                  CONNECT BY PRIOR USERID = PARENT_USERID    union all
                                SELECT SP_CODE FROM DIST_LOGIN_USER WHERE SP_CODE = '{userData.EMPLOYEE_CODE}' 
                                UNION ALL
                                 SELECT DIST_SP_CODE SP_CODE FROM DIST_SYNERGY_EMPLOYEE_MAP
                                WHERE LOGIN_SP_CODE ='{userData.EMPLOYEE_CODE}'").ToList();
                            if (sp_codes.Count > 0)
                                user[0].sp_codes = "'" + string.Join("','", sp_codes) + "'";

                        }
                    }
                }
                catch (Exception ex)
                {
                    sp_codes = null;
                }

            }
            return user;
        }
        #endregion



        public virtual void UpdateAuthenticatedCustomer(User user)
        {
            var now = DateTime.UtcNow.ToLocalTime();
            //0: userid
            //1:company_code
            //2:branch_code
            //3: StartFinc
            //4: EndFinc
            //5:BranchName
            //6:CompanyName
            //7:loginName
            var userdata = string.Format("{0}#{1}#{2}#{3}#{4}#{5}#{6}#{7}", user.User_id, user.company_code, user.Branch,
                user.startFiscalYear.ToString(), user.endfiscalyear.ToString(), user.branch_name, user.company_name, user.login_code);
            var ticket = new FormsAuthenticationTicket(
                2 /*version*/,
               user.login_code,
                now,
                now.Add(_expirationTimeSpan),
                true,
                userdata.ToString(),
                FormsAuthentication.FormsCookiePath);

            var encryptedTicket = FormsAuthentication.Encrypt(ticket);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            cookie.HttpOnly = true;
            if (ticket.IsPersistent)
            {
                cookie.Expires = ticket.Expiration;
            }
            cookie.Secure = FormsAuthentication.RequireSSL;
            cookie.Path = FormsAuthentication.FormsCookiePath;
            if (FormsAuthentication.CookieDomain != null)
            {
                cookie.Domain = FormsAuthentication.CookieDomain;
            }

            _httpContext.Response.Cookies.Add(cookie);
            _cachedUser = user;
        }
    }
}