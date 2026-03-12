using NeoErp.Core;
using NeoErp.Core.Models;
using NeoErp.Core.Models.Log4NetLoggin;
using NeoErp.Data;
using NeoERP.DocumentTemplate.Service.Interface.SalesOrderAdjustment;
using NeoERP.DocumentTemplate.Service.Models.SalesOrderIndent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Services.SalesOrderAdjustment
{
    public class SalesOrderAdjustmentService : ISalesOrderAdjustment
    {
        private IDbContext _dbContext;
        private IWorkContext _workContext;
        private DefaultValueForLog _defaultValueForLog;
        private ILogErp _logErp;
        private NeoErpCoreEntity _objectEntity;

        public SalesOrderAdjustmentService(IWorkContext workContext, IDbContext dbContext, NeoErpCoreEntity objectEntity)
        {
            this._workContext = workContext;
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            _logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule, _defaultValueForLog.FormCode);
        }

        public List<SalesOrderAdjustViewModel> GetAllSalesOrderAdjustment(OrderSearchParams param)
        {
            try
            {
                string orderDocQuery = $@"SELECT rownum as ROW_NO ,SSA.ORDER_NO,TO_CHAR(SSA.ORDER_DATE,'DD-MON-YYYY') as ORDER_DATE,SCS.CUSTOMER_EDESC as CUSTOMER_EDESC,SCS.CUSTOMER_CODE as SUPPLIER_CODE,
                IIMS.ITEM_EDESC as ITEM_EDESC,IIMS.ITEM_CODE as ITEM_CODE,SSA.MU_CODE as UNIT,SSA.QUANTITY as QUANTITY,CASE WHEN SSA.CANCEL_QUANTITY IS NULL THEN 0 ELSE SSA.CANCEL_QUANTITY END as CANCEL_QUANTITY,CASE WHEN SSA.ADJUST_QUANTITY IS NULL THEN 0 ELSE SSA.ADJUST_QUANTITY END as ADJUST_QUANTITY,
                SSA.CREATED_BY,TO_CHAR(SSA.CREATED_DATE,'DD-MON-YYYY') as CREATED_DATE FROM SA_SALES_ORDER SSA
                INNER JOIN SA_CUSTOMER_SETUP SCS ON SSA.CUSTOMER_CODE=SCS.CUSTOMER_CODE
                INNER JOIN IP_ITEM_MASTER_SETUP IIMS ON IIMS.ITEM_CODE=SSA.ITEM_CODE
                WHERE SSA.FORM_CODE IN('{param.Document}') and SSA.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' 
                AND TO_DATE(TO_CHAR(SSA.ORDER_DATE,'DD-MON-YYYY')) >= TO_DATE('{param.FromDate}','YYYY-MON-DD') 
                AND TO_DATE(TO_CHAR(SSA.ORDER_DATE,'DD-MON-YYYY')) <= TO_DATE('{param.ToDate}','YYYY-MON-DD')";

                var orderData = _dbContext.SqlQuery<SalesOrderAdjustViewModel>(orderDocQuery).ToList();
                return orderData;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting sales order adjustment : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<SalesOrderIndentDocument> GetDocForSalesOrderAdjustment(string tableName)
        {
            try
            {
                string orderDocQuery = $@"select FS.FORM_EDESC,FS.FORM_CODE from form_setup FS, form_detail_setup FD where 
                FS.COMPANY_CODE=FD.COMPANY_CODE AND FS.FORM_CODE=FD.FORM_CODE AND
                FD.table_name=UPPER('{tableName}') AND FD.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND FS.DELETED_FLAG='N'
                GROUP BY FS.FORM_EDESC,FS.FORM_CODE";

                var orderDocData = _dbContext.SqlQuery<SalesOrderIndentDocument>(orderDocQuery).ToList();
                return orderDocData;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting sales order document : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public string SaveSalesOrderAdjustment(List<SalesOrderAdjustViewModel> modelList)
        {
            try
            {
                if (modelList.Count > 0)
                {
                    foreach (var ml in modelList)
                    {
                        string updateIndentAdjustQuery = $@"UPDATE SA_SALES_ORDER SSA SET SSA.CANCEL_QUANTITY={ml.CANCEL_QUANTITY} , SSA.ADJUST_QUANTITY={ml.ADJUST_QUANTITY} , SSA.CANCEL_FLAG='Y',
                       SSA.CREATED_BY='{_workContext.CurrentUserinformation.login_code}',SSA.CREATED_DATE=SYSDATE WHERE SSA.ORDER_NO='{ml.ORDER_NO}'";
                        _dbContext.ExecuteSqlCommand(updateIndentAdjustQuery);
                    }
                    _logErp.InfoInFile("sales order adjustment updated by : " + _workContext.CurrentUserinformation.LOGIN_EDESC);
                    return "Successfull";
                }
                else
                {
                    return "Unsuccessfull";
                }
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error whlie saving purchase indent adjustment : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }
    }
}
