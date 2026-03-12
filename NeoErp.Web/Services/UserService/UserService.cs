using NeoErp.Core;
using NeoErp.Core.Models;
using NeoErp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoErp.Services.UserService
{
    public class UserService : IUserService
    {
        private NeoErpCoreEntity _dbContext;
        private IWorkContext _workContext;
        public UserService(NeoErpCoreEntity dbContext, IWorkContext workContext)
        {
            _dbContext = dbContext;
            _workContext = workContext;


        }

        public string ChangePassword(ChangePasswordViewModel model)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;


            //if (model.NewPassword != model.ConfirmPassword)
            //    return "New password and change password does not match";

            //string getUserToChangePassword = $@"select * from sc_application_users  where group_sku_flag='I' and company_code='01' and deleted_flag='N'";
            //var user = this._dbContext.SqlQuery<ChangePasswordModel>(getUserToChangePassword).FirstOrDefault();
            try
            {
                string checkUserPassword = $@"select FN_DECRYPT_PASSWORD(PASSWORD) PASSWORD from sc_application_users  where USER_NO='{userid}' and group_sku_flag='I' and company_code='{company_code}' and deleted_flag='N'";
                var password = this._dbContext.SqlQuery<string>(checkUserPassword).FirstOrDefault();
                if (string.IsNullOrEmpty(password))
                    return "cpempty"; //current password is empty
                else if (password != model.OldPassword)
                    return "cpincorrect"; //current password incorrect
                else if (model.NewPassword != model.ConfirmPassword)
                    return "npcpnotmatch"; //new password and current password does not match
                else
                {
                    string updateUserPassword = $@"UPDATE  sc_application_users SET PASSWORD=FN_ENCRYPT_PASSWORD('{model.NewPassword}')  where USER_NO='{userid}' and company_code='{company_code}' and deleted_flag='N'";
                    var result = _dbContext.ExecuteSqlCommand(updateUserPassword);
                    return "success";
                }

            }
            catch (System.Exception ex)
            {

                return ex.ToString();
            }


        }

        public string ChangeUserPassword(ChangePasswordViewModel model)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;


            //if (model.NewPassword != model.ConfirmPassword)
            //    return "New password and change password does not match";

            //string getUserToChangePassword = $@"select * from sc_application_users  where group_sku_flag='I' and company_code='01' and deleted_flag='N'";
            //var user = this._dbContext.SqlQuery<ChangePasswordModel>(getUserToChangePassword).FirstOrDefault();
            try
            {
                string checkUserPassword = $@"select FN_DECRYPT_PASSWORD(PASSWORD) PASSWORD from sc_application_users  where USER_NO='{userid}' and group_sku_flag='I' and company_code='{company_code}' and deleted_flag='N'";
                var password = this._dbContext.SqlQuery<string>(checkUserPassword).FirstOrDefault();
                if (string.IsNullOrEmpty(password))
                    return "cpempty"; //current password is empty
                else if (password != model.OldPassword)
                    return "cpincorrect"; //current password incorrect
                else if (model.NewPassword != model.ConfirmPassword)
                    return "npcpnotmatch"; //new password and current password does not match
                else
                {
                    string updateUserPassword = $@"UPDATE  sc_application_users SET PASSWORD=FN_ENCRYPT_PASSWORD('{model.NewPassword}')  where USER_NO='{userid}' and company_code='{company_code}' and deleted_flag='N'";
                    var result = _dbContext.ExecuteSqlCommand(updateUserPassword);
                    return "success";
                }

            }
            catch (System.Exception ex)
            {

                return ex.ToString();
            }


        }
        public string InsertChangeUserPassword(UserViewModel model)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var maxUserNumber = 0;
            var insertSuccessFlag = false;

            try
            {
                string query = $@"SELECT NVL(MAX(TO_NUMBER(C.USER_NO )+1),1) AS USER_NO  FROM SC_APPLICATION_USERS C";
                maxUserNumber = this._dbContext.SqlQuery<int>(query).FirstOrDefault();
                if (model.SAVE_FLAG == "Save")
                {
                    string inserPasswordChangedUser = $@"INSERT INTO  SC_APPLICATION_USERS (USER_NO,LOGIN_CODE,LOGIN_EDESC ,PRE_USER_NO,GROUP_SKU_FLAG,ABBR_CODE,PASSWORD,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG) VALUES('{maxUserNumber}','{model.USERNAME}','{model.USERNAME}','{maxUserNumber}','I','FFF',FN_ENCRYPT_PASSWORD('{model.PASSWORD}'),'{company_code}','{userid}',sysdate,'N')";
                    var rowCount = _dbContext.ExecuteSqlCommand(inserPasswordChangedUser);
                    insertSuccessFlag = true;
                    if (insertSuccessFlag == true)
                        return "Success";
                    else
                        return "fail";
                }
                else if (model.SAVE_FLAG == "Update")
                {
                    string updateUserPassword = $@"UPDATE  sc_application_users SET PASSWORD=FN_ENCRYPT_PASSWORD('{model.PASSWORD}'),LOGIN_CODE='{model.USERNAME}' ,LOGIN_EDESC='{model.FULLNAME}',USER_TYPE='{model.USER_TYPE}' where USER_NO={model.USER_NO} and company_code='{company_code}' and deleted_flag='N'";
                    var result = _dbContext.ExecuteSqlCommand(updateUserPassword);
                    return "Updated";
                    //string checkUserPassword = $@"select FN_DECRYPT_PASSWORD(PASSWORD) PASSWORD from sc_application_users  where USER_NO='{userid}' and group_sku_flag='I' and company_code='{company_code}' and deleted_flag='N'";
                    //var password = this._dbContext.SqlQuery<string>(checkUserPassword).FirstOrDefault();
                    //if (string.IsNullOrEmpty(password))
                    //    return "cpempty"; //current password is empty
                    //else if (password != model.PASSWORD)
                    //    return "cpincorrect"; //current password incorrect
                    //else if (model.NewPassword != model.ConfirmPassword)
                    //    return "npcpnotmatch"; //new password and current password does not match
                    //else
                    //{
                    //    string updateUserPassword = $@"UPDATE  sc_application_users SET PASSWORD=FN_ENCRYPT_PASSWORD('{model.PASSWORD}'),LOGIN_CODE='{model.USERNAME}' ,LOGIN_EDESC='{model.FULLNAME}',USER_TYPE='{model.USER_TYPE}' where USER_NO='{userid}' and company_code='{company_code}' and deleted_flag='N'";
                    //    var result = _dbContext.ExecuteSqlCommand(updateUserPassword);
                    //    return "success";
                    //}

                }
                else
                    return "fail";


            }
            catch (System.Exception EX)
            {

                throw EX;
            }

            //if (model.NewPassword != model.ConfirmPassword)
            //    return "New password and change password does not match";

            //string getUserToChangePassword = $@"select * from sc_application_users  where group_sku_flag='I' and company_code='01' and deleted_flag='N'";
            //var user = this._dbContext.SqlQuery<ChangePasswordModel>(getUserToChangePassword).FirstOrDefault();
            //try
            //{
            //    string checkUserPassword = $@"select FN_DECRYPT_PASSWORD(PASSWORD) PASSWORD from sc_application_users  where USER_NO='{userid}' and group_sku_flag='I' and company_code='{company_code}' and deleted_flag='N'";
            //    var password = this._dbContext.SqlQuery<string>(checkUserPassword).FirstOrDefault();
            //    if (string.IsNullOrEmpty(password))
            //        return "cpempty"; //current password is empty
            //    //else if (password != model.OldPassword)
            //    //    return "cpincorrect"; //current password incorrect
            //    //else if (model.NewPassword != model.ConfirmPassword)
            //        return "npcpnotmatch"; //new password and current password does not match
            //    //else
            //    //{
            //    //    string updateUserPassword = $@"UPDATE  sc_application_users SET PASSWORD=FN_ENCRYPT_PASSWORD('{model.NewPassword}')  where USER_NO='{userid}' and company_code='{company_code}' and deleted_flag='N'";
            //    //    var result = _dbContext.ExecuteSqlCommand(updateUserPassword);
            //    //    return "success";
            //    //}
            //    return "";

            //}
            //catch (System.Exception ex)
            //{

            //    return ex.ToString();
            //}


        }

        public List<UserViewModel> GetAllUserDetails(UserViewModel model)
        {
            try
            {
                string userListQuery = $@"SELECT USER_NO, LOGIN_CODE USERNAME,LOGIN_EDESC FULLNAME,USER_TYPE,NVL( FN_DECRYPT_PASSWORD(PASSWORD),  PASSWORD ) PASSWORD FROM SC_APPLICATION_USERS WHERE GROUP_SKU_FLAG='I'";
                var userList = this._dbContext.SqlQuery<UserViewModel>(userListQuery).ToList();
                return userList;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public UserViewModel GetUserDetailsByUserNo(int userno)
        {
            string userByUserNoQuery = $@"SELECT USER_NO, LOGIN_CODE USERNAME,LOGIN_EDESC FULLNAME,USER_TYPE,NVL( FN_DECRYPT_PASSWORD(PASSWORD),  PASSWORD ) PASSWORD FROM SC_APPLICATION_USERS WHERE GROUP_SKU_FLAG='I' and USER_NO={userno}";
            var userByUserNo = this._dbContext.SqlQuery<UserViewModel>(userByUserNoQuery).FirstOrDefault();
            return userByUserNo;
        }
        public List<UserViewModel> GetAllUserType()
        {
            string getAllUserTypeQuery = $@"SELECT USER_NO, USER_TYPE  FROM SC_APPLICATION_USERS WHERE USER_TYPE IS NOT NULL";
            var getAllUserTypeList = this._dbContext.SqlQuery<UserViewModel>(getAllUserTypeQuery).ToList();
            return getAllUserTypeList;
        }


        //User
        #region User Setup 


        public string createUser(AddUserModel modal)
        {
            var message = "";
            var newmaxusercode = string.Empty;
            string selectQuery = @"SELECT NVL(MAX(TO_NUMBER(USER_NO))+1, 1) MAXID FROM SC_APPLICATION_USERS";
            newmaxusercode = this._dbContext.SqlQuery<int>(selectQuery).FirstOrDefault().ToString();
            {
                try
                {
                    var insertQuery = $@"INSERT INTO SC_APPLICATION_USERS(USER_NO,PRE_USER_NO,LOGIN_CODE,LOGIN_EDESC,PASSWORD,EMPLOYEE_CODE,COMPANY_CODE,
                                         USER_LOCK_FLAG,SUPER_USER_FLAG,GROUP_SKU_FLAG,DELETED_FLAG,CREATED_BY,CREATED_DATE,ABBR_CODE) 
                                         VALUES('{newmaxusercode}','0','{modal.LOGIN_CODE}','{modal.LOGIN_CODE}',FN_ENCRYPT_PASSWORD('{modal.PASSWORD}'),'{modal.EMPLOYEE_CODE}','{modal.COMPANY_CODE}',
                                          '{modal.USER_LOCK_FLAG}','{modal.SUPER_USER_FLAG}','I','N','ADMIN',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'AB')";


                    _dbContext.ExecuteSqlCommand(insertQuery);

                    message = "200";
                }
                catch (Exception e)
                {
                    message = "failed";
                }
            }

            return message;
        }



        public string updateUser(AddUserModel modal)
        {
            using (var trans = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var company_code = _workContext.CurrentUserinformation.company_code;
                    var message = string.Empty;
                    string Query = $@"UPDATE SC_APPLICATION_USERS SET LOGIN_CODE='{modal.LOGIN_CODE}',PASSWORD=FN_ENCRYPT_PASSWORD('{modal.PASSWORD}'),EMPLOYEE_CODE='{modal.EMPLOYEE_CODE}',
                                COMPANY_CODE='{modal.COMPANY_CODE}',USER_LOCK_FLAG='{modal.USER_LOCK_FLAG}',SUPER_USER_FLAG='{modal.SUPER_USER_FLAG}',MODIFY_DATE = SYSDATE  WHERE USER_NO = '{modal.USER_NO}'";
                    var entity = this._dbContext.ExecuteSqlCommand(Query);
                    if (entity > 0)
                    {
                        message = "UPDATED";
                    }
                    trans.Commit();
                    return message;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }
        }


        public List<AddUserModel> GetUserList()
        {
            try
            {
                string query = $@"SELECT FN_DECRYPT_PASSWORD(PASSWORD) PASSWORD,USER_NO,LOGIN_CODE,EMPLOYEE_CODE,USER_LOCK_FLAG,COMPANY_CODE,SUPER_USER_FLAG,USER_TYPE,CREATED_DATE,CREATED_BY,USER_TYPE FROM SC_APPLICATION_USERS WHERE DELETED_FLAG = 'N'";
                var result = _dbContext.SqlQuery<AddUserModel>(query).ToList();
                return result;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<CompanySetupModel> getAllCompany()
        {
            try
            {

                string query = $@"SELECT DISTINCT 
                        INITCAP(COMPANY_EDESC) AS COMPANY_EDESC,
                        COMPANY_CODE ,CREATED_BY
                        FROM COMPANY_SETUP
                        WHERE DELETED_FLAG = 'N'";


                var result = _dbContext.SqlQuery<CompanySetupModel>(query).ToList();
                return result;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<EmployeeModel1> getAllEmployeeList()
        {
            try
            {

                string query = $@"SELECT EMPLOYEE_CODE,EMPLOYEE_EDESC,EPERMANENT_ADDRESS1      
                               FROM HR_EMPLOYEE_SETUP 
                               WHERE GROUP_SKU_FLAG = 'I'
                               AND DELETED_FLAG = 'N'";
                var result = _dbContext.SqlQuery<EmployeeModel1>(query).ToList();
                return result;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public string DeleteUserFromDb(string usercode)
        {
            try
            {
                var sqlquery = $@"UPDATE SC_APPLICATION_USERS SET DELETED_FLAG = 'Y' WHERE USER_NO='{usercode}'";
                var result = _dbContext.ExecuteSqlCommand(sqlquery);
                return "DELETED";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion



    }
}