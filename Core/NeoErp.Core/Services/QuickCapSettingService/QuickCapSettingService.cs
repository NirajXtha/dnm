using NeoErp.Core.Models.CustomModels.SettingsEntities;
using NeoErp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Core.Services.QuickCapSettingService
{
    public class QuickCapSettingService : IQuickCapSetting
    {
        private IDbContext _dbContext;
        private IWorkContext _workContext;
        public QuickCapSettingService(IDbContext _dbContext,IWorkContext workContext) {
            this._dbContext = _dbContext;
            this._workContext = workContext;
        }

        public List<QuickCapSettingEntities> GetAllQuickCap()
        {
            var query = "SELECT ID, QUICKCAP_EDESC,QUICKCAP_TITLE ||'  Name: ' ||QUICKCAP_EDESC QUICKCAP_TITLE,Module_code,isactive  FROM WEB_QUICKCAP";
            List<QuickCapSettingEntities> quickCapList;
            try
            {
                quickCapList = _dbContext.SqlQuery<QuickCapSettingEntities>(query).ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return quickCapList;
        }
        
        public int AddBulkUserForQuickCap(string USERNO, int ID)
        {
            string MenuSql = $@"UPDATE WEB_QUICKCAP SET USERPERMISSION ='" + USERNO + "' WHERE ID='" + ID + "' ";

            var num = 0;
            try
            {
                num = this._dbContext.ExecuteSqlCommand(MenuSql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return num;
        }

        public List<QuickCapSettingEntities> GetUserForQuickCapByID(int ID)
        {
            var query = "SELECT USERPERMISSION  FROM WEB_QUICKCAP WHERE ID=" + ID + "";
            List<QuickCapSettingEntities> userList;
            try
            {
                userList = _dbContext.SqlQuery<QuickCapSettingEntities>(query).ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return userList;
        }

        public List<QuickCapSettingEntities> GetUsers()
        {
            var query = "SELECT TO_CHAR(USER_NO) as ID, LOGIN_EDESC as label FROM SC_APPLICATION_USERS";
            List<QuickCapSettingEntities> userList;
            try
            {
                userList = _dbContext.SqlQuery<QuickCapSettingEntities>(query).ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return userList;
        }
    }
}