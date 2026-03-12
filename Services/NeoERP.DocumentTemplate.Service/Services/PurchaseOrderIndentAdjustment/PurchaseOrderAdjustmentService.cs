using NeoErp.Core;
using NeoErp.Core.Models;
using NeoErp.Core.Models.Log4NetLoggin;
using NeoErp.Data;
using NeoERP.DocumentTemplate.Service.Interface.PurchaseIndentOrderAdjustment;
using NeoERP.DocumentTemplate.Service.Models.PurchaseOrderIndent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Services.PurchaseOrderIndentAdjustment
{
    public class PurchaseOrderAdjustmentService : IPurchaseOrderAdjustment
    {
        private IDbContext _dbContext;
        private IWorkContext _workContext;
        private DefaultValueForLog _defaultValueForLog;
        private ILogErp _logErp;
        private NeoErpCoreEntity _objectEntity;

        public PurchaseOrderAdjustmentService(IWorkContext workContext, IDbContext dbContext, NeoErpCoreEntity objectEntity)
        {
            this._workContext = workContext;
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            _logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule, _defaultValueForLog.FormCode);
        }
        public List<OrderAdjustViewModel> GetAllPurchaseOrderAdjustment(IndentSearchParam param)
        {
            try
            {
                //param.Document = "116";
                string orderDocQuery = $@"SELECT rownum as ROW_NO ,IPO.ORDER_NO,TO_CHAR(IPO.ORDER_DATE,'DD-MON-YYYY') as ORDER_DATE,ISS.SUPPLIER_EDESC as SUPPLIER_EDESC,ISS.SUPPLIER_CODE as SUPPLIER_CODE,
                IIMS.ITEM_EDESC as ITEM_EDESC,IIMS.ITEM_CODE as ITEM_CODE,IPO.MU_CODE as UNIT,IPO.QUANTITY as QUANTITY,CASE WHEN IPO.CANCEL_QUANTITY IS NULL THEN 0 ELSE IPO.CANCEL_QUANTITY END as CANCEL_QUANTITY,CASE WHEN IPO.ADJUST_QUANTITY IS NULL THEN 0 ELSE IPO.ADJUST_QUANTITY END as ADJUST_QUANTITY,
                IPO.CREATED_BY,TO_CHAR(IPO.CREATED_DATE,'DD-MON-YYYY') as CREATED_DATE FROM IP_PURCHASE_ORDER IPO
                INNER JOIN IP_SUPPLIER_SETUP ISS ON IPO.SUPPLIER_CODE=ISS.SUPPLIER_CODE
                INNER JOIN IP_ITEM_MASTER_SETUP IIMS ON IIMS.ITEM_CODE=IPO.ITEM_CODE
                WHERE IPO.FORM_CODE IN('{param.Document}') and IPO.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' 
                AND TO_DATE(TO_CHAR(IPO.ORDER_DATE,'DD-MON-YYYY')) >= TO_DATE('{param.FromDate}','YYYY-MON-DD') 
                AND TO_DATE(TO_CHAR(IPO.ORDER_DATE,'DD-MON-YYYY')) <= TO_DATE('{param.ToDate}','YYYY-MON-DD')";

                var orderData = _dbContext.SqlQuery<OrderAdjustViewModel>(orderDocQuery).ToList();
                return orderData;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting purchase order adjustment : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<OrderAdjustmentDoc> GetDocForOrderAdjustment(string tableName)
        {
            try
            {
                string indentDocQuery = $@"select FS.FORM_EDESC,FS.FORM_CODE from form_setup FS, form_detail_setup FD where 
                FS.COMPANY_CODE=FD.COMPANY_CODE AND FS.FORM_CODE=FD.FORM_CODE AND
                FD.table_name=UPPER('{tableName}') AND FD.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND FS.DELETED_FLAG='N'
                GROUP BY FS.FORM_EDESC,FS.FORM_CODE";

                var orderDocData = _dbContext.SqlQuery<OrderAdjustmentDoc>(indentDocQuery).ToList();
                return orderDocData;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting order document : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public string SaveOrderAdjustment(List<OrderAdjustViewModel> modelList)
        {
            try
            {
                if (modelList.Count > 0)
                {
                    foreach (var ml in modelList)
                    {
                        string updateIndentAdjustQuery = $@"UPDATE IP_PURCHASE_ORDER IPO SET IPO.CANCEL_QUANTITY={ml.CANCEL_QUANTITY} , IPO.ADJUST_QUANTITY={ml.ADJUST_QUANTITY} , IPO.CANCEL_FLAG='Y',
                       IPO.CREATED_BY='{_workContext.CurrentUserinformation.login_code}',IPO.CREATED_DATE=SYSDATE WHERE IPO.ORDER_NO='{ml.ORDER_NO}'";
                        _dbContext.ExecuteSqlCommand(updateIndentAdjustQuery);
                    }
                    _logErp.InfoInFile("Purchase order adjustment updated by : " + _workContext.CurrentUserinformation.LOGIN_EDESC);
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
