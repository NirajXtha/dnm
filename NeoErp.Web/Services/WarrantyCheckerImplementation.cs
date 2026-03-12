using NeoErp.Data;
using NeoErp.Models.WarrantyChecker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Services
{
    public class WarrantyCheckerImplementation : IWarrantyChecker
    {
        private IDbContext _dbContext;
        public WarrantyCheckerImplementation(IDbContext _dbContext)
        {
            this._dbContext = _dbContext;
        }
        public WarrantyChekerModel GetWarrantyInfo(string serialNo)
        {
            var entities = new WarrantyChekerModel();
            try
            {
                var result = new List<WarrantyChekerModel>();
                var Query = @"SELECT INVOICE_NO, TO_CHAR(ACTIVATION_DATE, 'DD-MON-YYYY')ACTIVATION_DATE, TO_CHAR((ACTIVATION_DATE + WARRANTY_DAYS), 'DD-MON-YYYY') as VALID_DATE, ((ACTIVATION_DATE + WARRANTY_DAYS) - TRUNC(SYSDATE) ) as EXPIARY_DAYS , ACTIVATED_NAME, CONTACT_NO, SERVICE_TYPE,ACTIVATE_FLAG, THIEF_FLAG  FROM IP_MAC_SERIAL_TRACK WHERE SERIAL_NO =" + serialNo.Trim()+"";
                Query = Query.Replace("\"", "'");
                result = _dbContext.SqlQuery<WarrantyChekerModel>(Query).ToList();
                if (result.Count > 0)
                {
                    entities.Message = "";
                    entities = result.FirstOrDefault();
                }
                else {
                    entities.Message = "Empty";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return entities;
        }
        public string SaveWarrantyMsgService(DefectMessage param)
        {
            var msg = string.Empty;
            try
            {
                var Query = @"INSERT INTO SERVICE_TRANSACTION (INVOICE_NO, DEFECT_CODE, NOTE, CREATED_DATE) VALUES('" + param.INVOICE_NO + "', '" + param.DEFECT_CODE + "', '" + param.DEFECT_DESC + "', SYSDATE)";

                var result = this._dbContext.ExecuteSqlCommand(Query);
                if (result > 0)
                {
                    msg = "Saved";
                }
            }
            catch (Exception)
            {

                throw;
            }
            return msg;
        }

        public List<DefactModel> GetDefact()
        {
            var entities = new List<DefactModel>();
            try
            {
                var result = new List<DefactModel>();
                var Query = @"SELECT DEFECT_CODE, DEFECT_NAME, DEFECT_DESC FROM DEFECT_SETUP";
                result = _dbContext.SqlQuery<DefactModel>(Query).ToList();
                entities = result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return entities;
        }
    }
}