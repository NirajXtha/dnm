using NeoErp.Data;
using NeoErp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Services
{
    public class ReportConfig : IReportConfig
    {
        private IDbContext _dbContext;
        public ReportConfig(IDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
        public List<UserModels> GetAllUserLists()
        {
            string query = $@"SELECT LOGIN_CODE,LOGIN_EDESC FROM SC_APPLICATION_USERS WHERE GROUP_SKU_FLAG = 'I' AND DELETED_FLAG = 'N'";
            var usersList = _dbContext.SqlQuery<UserModels>(query).ToList();
            return usersList;
        }
    }
}