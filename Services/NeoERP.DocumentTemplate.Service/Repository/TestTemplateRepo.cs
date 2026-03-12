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
    public class TestTemplateRepo : ITestTemplateRepo
    {
        private IDbContext _dbContext;
        private IWorkContext _workContext;
        private ICacheManager _cacheManager;
        public TestTemplateRepo(IDbContext dbContext, IWorkContext workContext, ICacheManager cacheManager)
        {
            this._dbContext = dbContext;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
        }
        public void testfunction()
        {
            throw new NotImplementedException();
        }
        public List<FormDetailSetup> GetAllFORMDETAILSETUP()
        {
            try
            {

                List<FormDetailSetup> FORMDETAILSETUPList = new List<FormDetailSetup>();
                string query = @"SELECT * FROM FORM_DETAIL_SETUP WHERE FORM_CODE ='158'";
                FORMDETAILSETUPList = this._dbContext.SqlQuery<FormDetailSetup>(query).ToList();
                return FORMDETAILSETUPList;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
