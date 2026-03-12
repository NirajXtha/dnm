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
    public class PurchaseIndentAdjustmentService : IPurchaseIndentAdjustment
    {

        private IDbContext _dbContext;
        private IWorkContext _workContext;
        private DefaultValueForLog _defaultValueForLog;
        private ILogErp _logErp;
        private NeoErpCoreEntity _objectEntity;

        public PurchaseIndentAdjustmentService(IWorkContext workContext, IDbContext dbContext, NeoErpCoreEntity objectEntity)
        {
            this._workContext = workContext;
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            _logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule, _defaultValueForLog.FormCode);
        }

        public List<IndentAdjustViewModel> GetAllPurchaseIndentAdjustment(IndentSearchParam param)

        {
            try
            {
               
                string indentDocQuery = $@"SELECT rownum as ROW_NO ,IPR.REQUEST_NO,TO_CHAR(IPR.REQUEST_DATE,'DD-MON-YYYY') as REQUEST_DATE,ILS.LOCATION_EDESC as FROM_LOCATION,ILS.LOCATION_CODE as FROM_LOCATION_CODE,
                IIMS.ITEM_EDESC as ITEM_EDESC,IIMS.ITEM_CODE as ITEM_CODE,IPR.MU_CODE as UNIT,IPR.QUANTITY as QUANTITY,CASE WHEN IPR.CANCEL_QUANTITY IS NULL THEN 0 ELSE IPR.CANCEL_QUANTITY END as CANCEL_QUANTITY,CASE WHEN IPR.ADJUST_QUANTITY IS NULL THEN 0 ELSE IPR.ADJUST_QUANTITY END as ADJUST_QUANTITY,
                IPR.CREATED_BY,TO_CHAR(IPR.CREATED_DATE,'DD-MON-YYYY') as CREATED_DATE FROM IP_PURCHASE_REQUEST IPR
                INNER JOIN IP_LOCATION_SETUP ILS ON IPR.FROM_LOCATION_CODE=ILS.LOCATION_CODE
                INNER JOIN IP_ITEM_MASTER_SETUP IIMS ON IIMS.ITEM_CODE=IPR.ITEM_CODE
                WHERE IPR.FORM_CODE IN('{param.Document}') and IPR.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' 
                AND TO_DATE(TO_CHAR(IPR.REQUEST_DATE,'DD-MON-YYYY')) >= TO_DATE('{param.FromDate}','YYYY-MON-DD') 
                AND TO_DATE(TO_CHAR(IPR.REQUEST_DATE,'DD-MON-YYYY')) <= TO_DATE('{param.ToDate}','YYYY-MON-DD')";

                var indentData = _dbContext.SqlQuery<IndentAdjustViewModel>(indentDocQuery).ToList();
                return indentData;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting purchase indent adjustment : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<IndentAdjustmentDoc> GetDocForIndentAdjustment(string tableName)
        {
            try
            {
                string indentDocQuery = $@"select FS.FORM_EDESC,FS.FORM_CODE from form_setup FS, form_detail_setup FD where 
                FS.COMPANY_CODE=FD.COMPANY_CODE AND FS.FORM_CODE=FD.FORM_CODE AND
                FD.table_name=UPPER('{tableName}') AND FD.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND FS.DELETED_FLAG='N'
                GROUP BY FS.FORM_EDESC,FS.FORM_CODE";

                var indentDocData = _dbContext.SqlQuery<IndentAdjustmentDoc>(indentDocQuery).ToList();
                return indentDocData;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting indent document : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public string SaveIndentAdjustment(List<IndentAdjustViewModel> modelList)
        {
            try
            {
                if(modelList.Count > 0)
                {
                    foreach(var ml in modelList)
                    {
                        string updateIndentAdjustQuery = $@"UPDATE IP_PURCHASE_REQUEST IPR SET IPR.CANCEL_QUANTITY={ml.CANCEL_QUANTITY} , IPR.ADJUST_QUANTITY={ml.ADJUST_QUANTITY} , IPR.CANCEL_FLAG='Y',
                       IPR.CREATED_BY='{_workContext.CurrentUserinformation.login_code}',IPR.CREATED_DATE=SYSDATE WHERE IPR.REQUEST_NO='{ml.REQUEST_NO}'";
                        _dbContext.ExecuteSqlCommand(updateIndentAdjustQuery);
                    }
                    _logErp.InfoInFile("Purchase indent adjustment updated by : " + _workContext.CurrentUserinformation.LOGIN_EDESC);
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
