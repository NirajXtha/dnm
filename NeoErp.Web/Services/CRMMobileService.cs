using NeoErp.Core.Models.CustomModels;
using NeoErp.Data;
using NeoErp.Models.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Services
{
    public class CRMMobileService : ICRMMobileService
    {
        private IDbContext _dbContext;

        public CRMMobileService(IDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public List<CRMMobileModel> GetCRMData()
        {
            string query = $@"SELECT LEAD_NO, LEAD_DATE, LEAD_TIME, COMPANY_NAME, REQUESTED_BY, PRODUCT_EDESC, DESCRIPTION, 
                            RATING, TO_CHAR(COMPLETION_DATE,'DD-MON-YYYY') COMPLETION_DATE, AGENT_EDESC, PROCESS_EDESC, ESTD_COMPLETE, CEIL(DAYS)DAYS FROM V_TICKET_LIST
                            ORDER BY ROW_ORDER, DAYS, RATING  DESC";
            var crmList = _dbContext.SqlQuery<CRMMobileModel>(query).ToList();
            return crmList;
        }
    }
}