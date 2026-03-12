using NeoErp.Core;
using NeoErp.Core.Models.CustomModels;
using NeoErp.Data;
using NeoErp.Models.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Services
{
    public class MobileNotificationService : IMobileNotificationService
    {
        private IDbContext  _idbContext { get; set; }
        private IWorkContext _workContext;
        public MobileNotificationService(IDbContext idbContext, IWorkContext workContext)
        {
            this._idbContext = idbContext;
            this._workContext = workContext;
        }

        public IEnumerable<MobileDataVoucherModel> GetVoucherWithFlag(int userId, string moduleCode = "", string append = "top", int sessionRowId = 0)
        {

            string query = string.Format(@"SELECT a.*, rownum r__
                    FROM(SELECT DISTINCT A.VOUCHER_NO ,A.CHECKED_BY,A. AUTHORISED_BY,A.POSTED_BY ,SC.CHECK_FLAG,SC.POST_FLAG,SC.VERIFY_FLAG,
                 A.VOUCHER_AMOUNT , A.FORM_CODE , A.CREATED_BY , A.VOUCHER_DATE , BS_DATE(A.VOUCHER_DATE) Miti, B.FORM_EDESC FORM_DESCRIPTION,B.FORM_TYPE,
                  C.REMARKS ,C.PARTICULARS, C.ACC_CODE , D.ACC_EDESC LEDGER_TITLE, E.BRANCH_EDESC BRANCH_DESCRIPTION, TO_NUMBER(A.SESSION_ROWID) SESSION_ROWID,
                B.MODULE_CODE, 'FINANCIAL' as TABLE_NAME
                FROM MASTER_TRANSACTION A, FORM_SETUP B, FA_DOUBLE_VOUCHER C, FA_CHART_OF_ACCOUNTS_SETUP D, FA_BRANCH_SETUP E,SC_FORM_CONTROL SC
                WHERE A.FORM_CODE = B.FORM_CODE
                     AND A.FORM_CODE=SC.FORM_CODE
                AND A.COMPANY_CODE=SC.COMPANY_CODE
                AND SC.USER_NO ={0}
                AND A.FORM_CODE IN(SELECT FORM_CODE FROM SC_FORM_CONTROL WHERE USER_NO={0} AND (CHECK_FLAG='Y' ))
                AND A.VOUCHER_NO = C.VOUCHER_NO
                AND C.SERIAL_NO = 1
                AND C.ACC_CODE = D.ACC_CODE
                AND (A.CHECKED_BY IS NULL) 
                AND A.BRANCH_CODE = E.BRANCH_CODE
                AND A.BRANCH_CODE = C.BRANCH_CODE
                AND A.COMPANY_CODE = B.COMPANY_CODE
                AND A.COMPANY_CODE = C.COMPANY_CODE
                AND A.COMPANY_CODE = D.COMPANY_CODE", userId);

            if (!string.IsNullOrEmpty(moduleCode) && moduleCode != "00")
            {
                query += string.Format(" AND B.MODULE_CODE = {0}", moduleCode);
            }
            
            query += string.Format(" AND TO_NUMBER(A.SESSION_ROWID) > {0}", sessionRowId);


            query += " Order By SESSION_ROWID DESC) a WHERE rownum < 50  UNION ALL ";

            query += string.Format(@"SELECT a.*, rownum r__
                     FROM(SELECT DISTINCT A.VOUCHER_NO ,A.CHECKED_BY,A. AUTHORISED_BY,A.POSTED_BY ,SC.CHECK_FLAG,SC.POST_FLAG,SC.VERIFY_FLAG,
                 A.VOUCHER_AMOUNT , A.FORM_CODE , A.CREATED_BY , A.VOUCHER_DATE , BS_DATE(A.VOUCHER_DATE) Miti, B.FORM_EDESC FORM_DESCRIPTION,B.FORM_TYPE,
                  C.REMARKS ,C.PARTICULARS, C.ACC_CODE , D.ACC_EDESC LEDGER_TITLE, E.BRANCH_EDESC BRANCH_DESCRIPTION, TO_NUMBER(A.SESSION_ROWID) SESSION_ROWID,
                B.MODULE_CODE, 'FINANCIAL' as TABLE_NAME
                FROM MASTER_TRANSACTION A, FORM_SETUP B, FA_DOUBLE_VOUCHER C, FA_CHART_OF_ACCOUNTS_SETUP D, FA_BRANCH_SETUP E,SC_FORM_CONTROL SC
                WHERE A.FORM_CODE = B.FORM_CODE
                     AND A.FORM_CODE=SC.FORM_CODE
                AND A.COMPANY_CODE=SC.COMPANY_CODE
                AND SC.USER_NO ={0}
                AND A.FORM_CODE IN(SELECT FORM_CODE FROM SC_FORM_CONTROL WHERE USER_NO={0} AND (VERIFY_FLAG='Y'))
                AND A.VOUCHER_NO = C.VOUCHER_NO
                AND C.SERIAL_NO = 1
                AND C.ACC_CODE = D.ACC_CODE
                AND (A.CHECKED_BY IS NOT NULL AND A.AUTHORISED_BY IS NULL) 
                AND A.BRANCH_CODE = E.BRANCH_CODE
                AND A.BRANCH_CODE = C.BRANCH_CODE
                AND A.COMPANY_CODE = B.COMPANY_CODE
                AND A.COMPANY_CODE = C.COMPANY_CODE
                AND A.COMPANY_CODE = D.COMPANY_CODE", userId);

            if (!string.IsNullOrEmpty(moduleCode) && moduleCode != "00")
            {
                query += string.Format(" AND B.MODULE_CODE = {0}", moduleCode);
            }

            query += string.Format(" AND TO_NUMBER(A.SESSION_ROWID) > {0}", sessionRowId);

            query += " Order By SESSION_ROWID DESC) a WHERE rownum < 50 UNION ALL ";

            query += string.Format(@" SELECT a.*, rownum r__
                    FROM(SELECT DISTINCT A.VOUCHER_NO ,A.CHECKED_BY,A. AUTHORISED_BY,A.POSTED_BY ,SC.CHECK_FLAG,SC.POST_FLAG,SC.VERIFY_FLAG,
                 A.VOUCHER_AMOUNT , A.FORM_CODE , A.CREATED_BY , A.VOUCHER_DATE , BS_DATE(A.VOUCHER_DATE) Miti, B.FORM_EDESC FORM_DESCRIPTION,B.FORM_TYPE,
                  C.REMARKS ,C.REMARKS PARTICULARS, C.ITEM_CODE ACC_CODE , D.ITEM_EDESC LEDGER_TITLE, E.BRANCH_EDESC BRANCH_DESCRIPTION, TO_NUMBER(A.SESSION_ROWID) SESSION_ROWID,
                B.MODULE_CODE,  C.TABLE_NAME
                FROM MASTER_TRANSACTION A, FORM_SETUP B, V$VIRTUAL_STOCK_WIP_LEDGER C, IP_ITEM_MASTER_SETUP D, FA_BRANCH_SETUP E,SC_FORM_CONTROL SC
                WHERE A.FORM_CODE = B.FORM_CODE
                     AND A.FORM_CODE=SC.FORM_CODE
                AND A.COMPANY_CODE=SC.COMPANY_CODE
                AND SC.USER_NO ={0}
                AND A.FORM_CODE IN(SELECT FORM_CODE FROM SC_FORM_CONTROL WHERE USER_NO={0} AND (CHECK_FLAG='Y' ))
                AND A.VOUCHER_NO = C.VOUCHER_NO
                AND C.SERIAL_NO = 1
                AND C.ITEM_CODE = D.ITEM_CODE
                AND (A.CHECKED_BY IS NULL) 
                AND A.BRANCH_CODE = E.BRANCH_CODE
                AND A.BRANCH_CODE = C.BRANCH_CODE
                AND A.COMPANY_CODE = B.COMPANY_CODE
                AND A.COMPANY_CODE = C.COMPANY_CODE
                AND A.COMPANY_CODE = D.COMPANY_CODE", userId);

            if (!string.IsNullOrEmpty(moduleCode) && moduleCode != "00")
            {
                query += string.Format(" AND B.MODULE_CODE = {0}", moduleCode);
            }

            query += string.Format(" AND TO_NUMBER(A.SESSION_ROWID) > {0}", sessionRowId);
            

            query += " Order By SESSION_ROWID DESC) a WHERE rownum < 50  UNION ALL ";

            query += string.Format(@" SELECT a.*, rownum r__
                     FROM(SELECT DISTINCT A.VOUCHER_NO ,A.CHECKED_BY,A. AUTHORISED_BY,A.POSTED_BY ,SC.CHECK_FLAG,SC.POST_FLAG,SC.VERIFY_FLAG,
                 A.VOUCHER_AMOUNT , A.FORM_CODE , A.CREATED_BY , A.VOUCHER_DATE , BS_DATE(A.VOUCHER_DATE) Miti, B.FORM_EDESC FORM_DESCRIPTION,B.FORM_TYPE,
                  C.REMARKS ,C.REMARKS PARTICULARS, C.ITEM_CODE ACC_CODE , D.ITEM_EDESC LEDGER_TITLE, E.BRANCH_EDESC BRANCH_DESCRIPTION, TO_NUMBER(A.SESSION_ROWID) SESSION_ROWID,
                B.MODULE_CODE, C.TABLE_NAME
                FROM MASTER_TRANSACTION A, FORM_SETUP B, V$VIRTUAL_STOCK_WIP_LEDGER C, IP_ITEM_MASTER_SETUP D, FA_BRANCH_SETUP E,SC_FORM_CONTROL SC
                WHERE A.FORM_CODE = B.FORM_CODE
                     AND A.FORM_CODE=SC.FORM_CODE
                AND A.COMPANY_CODE=SC.COMPANY_CODE
                AND SC.USER_NO ={0}
                AND A.FORM_CODE IN(SELECT FORM_CODE FROM SC_FORM_CONTROL WHERE USER_NO={0} AND (VERIFY_FLAG='Y'))
                AND A.VOUCHER_NO = C.VOUCHER_NO
                AND C.SERIAL_NO = 1
                AND C.ITEM_CODE = D.ITEM_CODE
                AND (A.CHECKED_BY IS NOT NULL AND A.AUTHORISED_BY IS NULL) 
                AND A.BRANCH_CODE = E.BRANCH_CODE
                AND A.BRANCH_CODE = C.BRANCH_CODE
                AND A.COMPANY_CODE = B.COMPANY_CODE
                AND A.COMPANY_CODE = C.COMPANY_CODE
                AND A.COMPANY_CODE = D.COMPANY_CODE", userId);

            if (!string.IsNullOrEmpty(moduleCode) && moduleCode != "00")
            {
                query += string.Format(" AND B.MODULE_CODE = {0}", moduleCode);
            }

            query += string.Format(" AND TO_NUMBER(A.SESSION_ROWID) > {0}", sessionRowId);
           


            query += " Order By SESSION_ROWID DESC) a WHERE rownum < 50";
            return this._idbContext.SqlQuery<MobileDataVoucherModel>(query);
        }

        public List<MobileDataVoucherModel> getNotificationStatus(string userId, string sessionRowId)
        {
            var moduleCode = "00";
            if (!string.IsNullOrEmpty(moduleCode) && moduleCode != "00")
            {
                var permission = this.GetModulePermission(userId);

                if (permission.Count() == 0 || !permission.Any(q => q == moduleCode))
                    return new List<MobileDataVoucherModel>();
            }

            string query = $@"SELECT  A.VOUCHER_NO , A.VOUCHER_AMOUNT , A.FORM_CODE , A.CREATED_BY , A.VOUCHER_DATE , BS_DATE(A.VOUCHER_DATE) Miti, B.FORM_EDESC FORM_DESCRIPTION,B.FORM_TYPE, C.REMARKS , C.ACC_CODE , D.ACC_EDESC LEDGER_TITLE, E.BRANCH_EDESC BRANCH_DESCRIPTION, TO_NUMBER(C.SESSION_ROWID) SESSION_ROWID, B.MODULE_CODE
                FROM MASTER_TRANSACTION A, FORM_SETUP B, FA_DOUBLE_VOUCHER C, FA_CHART_OF_ACCOUNTS_SETUP D, FA_BRANCH_SETUP E
                WHERE A.FORM_CODE = B.FORM_CODE
                AND A.FORM_CODE IN(SELECT FORM_CODE FROM SC_FORM_CONTROL WHERE USER_NO={userId} AND READ_FLAG='Y')
                AND A.VOUCHER_NO = C.VOUCHER_NO
                AND C.SERIAL_NO = 1
                AND C.ACC_CODE = D.ACC_CODE
               AND A.CHECKED_BY IS NULL 
                AND A.BRANCH_CODE = E.BRANCH_CODE
                AND A.BRANCH_CODE = C.BRANCH_CODE
                AND A.COMPANY_CODE = B.COMPANY_CODE
                AND A.COMPANY_CODE = C.COMPANY_CODE
                AND A.COMPANY_CODE = D.COMPANY_CODE
                AND A.COMPANY_CODE = E.COMPANY_CODE";

            if (!string.IsNullOrEmpty(moduleCode) && moduleCode != "00")
            {
                query += string.Format(" AND B.MODULE_CODE = '{0}'", moduleCode);
            }
            
            query += string.Format(" AND TO_NUMBER(C.SESSION_ROWID) > {0}", sessionRowId);
           

            query += " Order By SESSION_ROWID DESC";

            return this._idbContext.SqlQuery<MobileDataVoucherModel>(query).ToList();
        }

        public IEnumerable<string> GetModulePermission(string userId)
        {
            string query = string.Format(@"SELECT MODULE_CODE FROM SC_MODULE_USER_CONTROL WHERE USER_NO = {0} AND DELETED_FLAG='N'", userId);
            return this._idbContext.SqlQuery<string>(query);
        }

        public List<MobileWebLogTaskModel> GetUserMsgFromWeb(string userId,string taskMsg)
        {
            try
            {
                var condition = string.Empty;
                if (!string.IsNullOrEmpty(taskMsg))
                    condition = $@" AND TASK_NAME = '{taskMsg}'";
                //var query = $@"SELECT MESSAGE_FORMAT FROM (SELECT * FROM  MOBILE_WEB_SETUP WHERE USER_NO ='{userId}' {condition} ORDER BY CREATED_DATE DESC) WHERE ROWNUM=1";
                var query = $@"SELECT DISTINCT TASK_NAME,MESSAGE_FORMAT FROM  MOBILE_WEB_SETUP WHERE TASK_NAME <>'BR' AND USER_NO ='{userId}' {condition} ";
                var response = _idbContext.SqlQuery<MobileWebLogTaskModel>(query).ToList();
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public string GetUserMsgFromWebLog(string userId, string taskMsg, string taskDate)
        {
            try
            {
                var result = string.Empty;
                var condition = string.Empty;
                if (!string.IsNullOrEmpty(taskMsg) && !string.IsNullOrEmpty(taskDate))
                    condition = $@" AND TASK_NAME = '{taskMsg}' AND TASK_DATE='{taskDate}'";
                var query = $@"SELECT MESSAGE_FORMAT FROM (SELECT * FROM  MOBILE_WEB_TASK_LOG WHERE USER_NO ='{userId}' {condition} ORDER BY CREATED_DATE DESC) WHERE ROWNUM=1";
                var response = _idbContext.SqlQuery<string>(query).FirstOrDefault();
                if (!string.IsNullOrEmpty(response))
                    result = response;
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void LogFileInDatabase(string userId, string message, string deviceId,string taskName,string firstVal)
        {
            try
            {
                var result = string.Empty;
                var sysDate = System.DateTime.Today.ToString("MM-dd-yyyy");
                message = message.Replace("'", "''");
                var query = $@"INSERT INTO MOBILE_WEB_TASK_LOG(TASK_ID,TASK_NAME,MESSAGE_FORMAT,USER_NO, DEVICE_ID, TASK_DATE, CREATED_DATE, DELETED_FLAG)
                                VALUES('{firstVal}','{taskName}','{message}', '{Convert.ToInt32(userId)}', '{deviceId}','{sysDate}', SYSDATE,'N')";
                _idbContext.ExecuteSqlCommand(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void LogFileInDatabase(string userId, string message, string deviceId, string taskName)
        {
            throw new NotImplementedException();
        }

        public List<PURCHASE_RM_MODEL> GetUniqIdFromQry(string userId, string taskName)
        {
            try
            {
                var result = new List<PURCHASE_RM_MODEL>();
                var condition = string.Empty;
                if (string.IsNullOrEmpty(taskName))
                    taskName = "BR";
                var query = $@"SELECT QUERY FROM (SELECT * FROM  MOBILE_WEB_SETUP WHERE USER_NO ='{userId}' AND TASK_NAME = '{taskName}' ORDER BY CREATED_DATE DESC) WHERE ROWNUM=1";
                var response = _idbContext.SqlQuery<string>(query).FirstOrDefault();
                if (!string.IsNullOrEmpty(response))
                {
                    var addQuery = $@"SELECT * FROM ({response}) WHERE BANK_GNO NOT IN (SELECT TASK_ID FROM MOBILE_WEB_TASK_LOG)";
                    result = _idbContext.SqlQuery<PURCHASE_RM_MODEL>(addQuery).ToList();
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public RMPurchaseModel getMessageByBankID(string userId,string bankId)
        {
            try
            {
                var result = new RMPurchaseModel();
                var condition = string.Empty;
                var query = $@"SELECT MESSAGE_FORMAT MSG_FORMAT FROM  MOBILE_WEB_SETUP WHERE USER_NO ='{userId}' AND TASK_NAME='BR' AND TASK_ID='{bankId}' ORDER BY CREATED_DATE DESC";
                var response = _idbContext.SqlQuery<RMPurchaseModel>(query).First();
                return result = response ;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}