using NeoErp.Core;
using NeoErp.Core.Models;
using NeoErp.Data;
using NeoERP.QCQAManagement.Service.Interface;
using NeoERP.QCQAManagement.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QCQAManagement.Service.Repository
{

    public class QCQADocumentFinderRepo : IQCQADocumentFinderRepo
    {
        IWorkContext _workContext;
        IDbContext _dbContext;
        private NeoErpCoreEntity _objectEntity;
        public QCQADocumentFinderRepo(IWorkContext workContext, IDbContext dbContext, NeoErpCoreEntity objectEntity)
        {
            this._workContext = workContext;
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
        }

        public List<QCDocumentFinder> GetDocumentDetails(string formCode, string docVer = "all")
        {
            try
            {
                string query = $@"SELECT FORM_CODE, TRANSACTION_NO AS VOUCHER_NO,0 VOUCHER_AMOUNT,TO_CHAR(CREATED_DATE, 'DD-MON-YYYY') AS VOUCHER_DATE
                ,CREATED_BY,CREATED_DATE
                ,'' CHECKED_BY,CHECKED_DATE,'' AUTHORISED_BY,POSTED_DATE
                ,MODIFY_DATE,SYN_ROWID,REFERENCE_NO
                ,SESSION_ROWID,'' ITEM_EDESC,'' VEHICLE_NO,'' PARTY_NAME,'' ADDRESS,'' BILL_NO FROM QC_PARAMETER_TRANSACTION WHERE FORM_CODE ='{formCode.Trim()}' 
                and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND DELETED_FLAG='N' order by TRANSACTION_NO desc";
                var result = _dbContext.SqlQuery<QCDocumentFinder>(query).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool DeleteQCByTransaction(string transactionNo)
        {
            try
            {
                var UPDATE_QUERY = $@"UPDATE QC_PARAMETER_TRANSACTION SET DELETED_FLAG ='Y' WHERE TRANSACTION_NO='{transactionNo}'";
                _dbContext.ExecuteSqlCommand(UPDATE_QUERY);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
