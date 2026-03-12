using NeoErp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace NeoErp.Core.Services
{
    public class ReportBuilderService:IReportBuilderService
    {
        private IDbContext _dbContext;
        private IWorkContext _workContext;

        public ReportBuilderService(IDbContext dbContext, IWorkContext workContext)
        {
            this._dbContext = dbContext;
            this._workContext = workContext;
        }

       
        public DataTable GetData(string reportName, string queryProvider, string settings="")
        {
            var userInfo = this._workContext.CurrentUserinformation;
            if(userInfo == null)
            {}

            queryProvider = HttpUtility.HtmlDecode(queryProvider);
            string query = queryProvider.Replace("#companyCode#", userInfo.company_code).Replace("#branchCode#", userInfo.branch_code);
            var result = this._dbContext.SqlQuery(query);
            return result;
        }

    }
}