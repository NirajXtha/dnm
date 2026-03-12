using System;
using System.Collections.Generic;
using System.Linq;
using NeoErp.Core;
using NeoErp.Core.Models;
using NeoErp.Data;
using NeoErp.Models.LogServiceModel;

namespace NeoErp.Services.LogService
{
    public class LogViewer : ILogViewer
    {
        private IDbContext _dbContext;
        private IWorkContext _workContext;
        private NeoErpCoreEntity _objectEntity;
        public LogViewer(IDbContext dbContext,IWorkContext workContext, NeoErpCoreEntity objectEntity)
        {
            _dbContext = dbContext;
            _workContext = workContext;
            _objectEntity = objectEntity;
        }

        public List<LogServiceViewModel> GetLogForCurrentUserAndCompany()
        {
            var company = _workContext.CurrentUserinformation.company_code;
            var branch = _workContext.CurrentUserinformation.branch_code;
            var user = _workContext.CurrentUserinformation.User_id;
            string logFetchQuery = $@"SELECT LOG_ID,LOG_DATE,LOG_LEVEL,LOG_THREAD,LOG_LOGGER,LOG_MESSAGE,LOG_USER,LOG_COMPANY,LOG_BRANCH,LOG_TYPECODE,LOG_MODULE FROM LOG_DOC_TEMPLATE WHERE LOG_USER ='{user}' and LOG_COMPANY='{company}' and LOG_BRANCH='{branch}'";
            List<LogServiceViewModel> logEntity = this._dbContext.SqlQuery<LogServiceViewModel>(logFetchQuery).ToList();
            return logEntity;
        }

        public List<LogServiceViewModel> GetAllLog()
        {
            string allLogQuery = $@"SELECT * FROM LOG_DOC_TEMPLATE";
            List<LogServiceViewModel> allLog = this._dbContext.SqlQuery<LogServiceViewModel>(allLogQuery).ToList();
            return allLog;
        }


        public void DeleteLogUsingFilter(DateTime fromDate,DateTime toDate,string module ,string subModule)
        {
            string deleteQuery = $@"DELETE FROM LOG_DOC_TEMPLATE ldt WHERE TRUNC(ldt.LOG_DATE) BETWEEN TO_DATE('{fromDate.ToString("MM/dd/yyyy")}', 'MM/DD/RRRR') and TO_DATE('{toDate.ToString("MM/dd/yyyy")}','MM/DD/RRRR') and ldt.LOG_TYPECODE='{subModule}' and ldt.LOG_MODULE = '{module}'";
            this._objectEntity.ExecuteSqlCommand(deleteQuery);
        }

        public void DeleteAllLog()
        {
            string deleteQuery = $@"DELETE FROM LOG_DOC_TEMPLATE";
            this._objectEntity.ExecuteSqlCommand(deleteQuery);
        }
    }
}