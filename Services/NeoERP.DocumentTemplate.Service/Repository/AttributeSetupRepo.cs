using NeoERP.DocumentTemplate.Service.Interface;
using System;
using NeoErp.Core;
using NeoErp.Core.Caching;
using NeoErp.Data;
using System.Collections.Generic;
using NeoERP.DocumentTemplate.Service.Models;
using System.Linq;

namespace NeoERP.DocumentTemplate.Service.Repository
{
    public class AttributeSetupRepo : IAttributeSetup
    {
        private IDbContext _dbContext;
        private IWorkContext _workContext;
        private ICacheManager _cacheManager;
        public AttributeSetupRepo(IDbContext dbContext, IWorkContext workContext, ICacheManager cacheManager)
        {
            this._dbContext = dbContext;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
        }

        public string GetAttributeFlagFromCache()
        {
            var cacheKey = $"ATTRIBUTE_FLAG_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}";

            // Check if the value is already cached
            if (this._cacheManager.IsSet(cacheKey))
            {
                return _cacheManager.Get<string>(cacheKey);
            }
            else
            {
                // Query the database for ATTRIBUTE_FLAG
                var attributeFlagQuery = $@"SELECT ATTRIBUTE_FLAG 
                                           FROM PREFERENCE_SETUP 
                                           WHERE COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' 
                                           AND BRANCH_CODE = '{_workContext.CurrentUserinformation.branch_code}'";

                var attributeFlag = this._dbContext.SqlQuery<string>(attributeFlagQuery).FirstOrDefault();

                // Cache the result for 60 minutes (3600 seconds)
                this._cacheManager.Set(cacheKey, attributeFlag ?? "N", 60);

                return attributeFlag;
            }
        }

    }
}
