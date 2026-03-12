using NeoErp.Data;
using NeoErp.Models.WarrantyChecker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoErp.Controllers.Api
{
    public class DefectController : ApiController
    {
        private IDbContext _dbContext { get; set; }
        public DefectController(IDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public List<DefactModel> GetDefect()
        {
            var query = "select DEFECT_CODE,DEFECT_NAME,DEFECT_DESC from defect_setup where DELETED_FLAG='N'";
            var data = _dbContext.SqlQuery<DefactModel>(query).ToList();
            return data;
        }
        public HttpResponseMessage CreateDefect(DefactModel model)
        {
            Int16 defectCode = 0;
            Int16.TryParse(model.DEFECT_CODE,out defectCode);
            if (model == null)
                return Request.CreateResponse(HttpStatusCode.NotFound,"Value is not Found");
            if (defectCode == 0)
            {
                var insertQuery = string.Format(@"INSERT INTO defect_setup(DEFECT_CODE,DEFECT_NAME,DEFECT_DESC,COMPANY_CODE,CREATED_DATE,DELETED_FLAG,created_by)VALUES( Defect_sequence.nextval, '" + model.DEFECT_NAME + "', '" + model.DEFECT_DESC + "', '01',sysdate,'N','01')");
                var rowCount = _dbContext.ExecuteSqlCommand(insertQuery);
                _dbContext.SaveChanges();
            }
            else
            {
                string query = string.Format(@"UPDATE defect_setup SET DEFECT_NAME  = '"+model.DEFECT_NAME+ "',DEFECT_DESC= '" + model.DEFECT_DESC + "') WHERE DEFECT_CODE='"+defectCode+"'");
                var rowCount = _dbContext.ExecuteSqlCommand(query);
            }
            return Request.CreateResponse(HttpStatusCode.OK, "success");
        }

        [HttpGet]
        public HttpResponseMessage DeletedDefect(string DefectCode)
        {
            Int16 defectCode = 0;
            Int16.TryParse(DefectCode, out defectCode);
          
            if (defectCode > 0)
            {
                string query = string.Format(@"UPDATE defect_setup SET deleted_flag  = 'Y') WHERE DEFECT_CODE='" + defectCode + "'");
                var rowCount = _dbContext.ExecuteSqlCommand(query);
            }
          
            return Request.CreateResponse(HttpStatusCode.OK, "success");
        }
    }
}
