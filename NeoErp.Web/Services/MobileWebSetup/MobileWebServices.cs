using NeoErp.Core;
using NeoErp.Data;
using NeoErp.Models.Mobiles;
using NeoErp.Services.MobileWeb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace NeoErp.Services.MobileWebSetup
{
    public class MobileWebServices : IMobileWeb
    {
        private IDbContext _dbContext;
        private IWorkContext _iWorkContext;
        public MobileWebServices(IDbContext dbContext, IWorkContext workContext)
        {
            this._dbContext = dbContext;
            this._iWorkContext = workContext;
        }

        public List<MobileWebLogTaskModel> GetAllMobileWebLog()
        {
            var result = new List<MobileWebLogTaskModel>();
            try
            {
                var logQuery = $@"SELECT DISTINCT MW.TASK_ID, MW.TASK_NAME, SA.USER_NO, MW.TASK_DATE ,SA.LOGIN_EDESC,MW.MESSAGE_FORMAT FROM  MOBILE_WEB_TASK_LOG MW
                                    INNER JOIN SC_APPLICATION_USERS SA ON MW.USER_NO = SA.USER_NO
                                     ORDER BY TASK_DATE DESC";
                result = _dbContext.SqlQuery<MobileWebLogTaskModel>(logQuery).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<UserModelForMobile> getAllUsers(string filter)
        {
            var info = _iWorkContext.CurrentUserinformation;
            try
            {
                string condition = string.Empty;
                if (!String.IsNullOrEmpty(filter))
                {
                    condition = $@"AND ( LOWER(LOGIN_EDESC) LIKE '%{filter.ToLower()}%' OR LOWER(USER_NO) LIKE '%{filter.ToLower()}%')";
                }
                string query = $@"SELECT DISTINCT 
                        INITCAP(LOGIN_EDESC) AS LOGIN_EDESC,
                        LOGIN_CODE ,
                        TO_CHAR(USER_NO)USER_NO
                        FROM SC_APPLICATION_USERS
                        WHERE DELETED_FLAG = 'N' AND COMPANY_CODE='{info.company_code}' {condition}
                        ORDER BY LOGIN_EDESC";
                var userList = _dbContext.SqlQuery<UserModelForMobile>(query).ToList();
                return userList;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<ItemModelForMobileFilter> getAllItems(string filter)
        {
            var info = _iWorkContext.CurrentUserinformation;
            try
            {
                string condition = string.Empty;
                if (!String.IsNullOrEmpty(filter))
                {
                    condition = $@"AND ( LOWER(ITEM_EDESC) LIKE '%{filter.ToLower()}%' OR LOWER(ITEM_CODE) LIKE '%{filter.ToLower()}%')";
                }
                string query = $@"SELECT DISTINCT 
                        INITCAP(ITEM_EDESC) AS ITEM_EDESC,
                        ITEM_CODE 
                        FROM IP_ITEM_MASTER_SETUP
                        WHERE DELETED_FLAG = 'N' AND COMPANY_CODE='{info.company_code}' AND GROUP_SKU_FLAG='I' {condition}
                        ORDER BY ITEM_EDESC";
                var userList = _dbContext.SqlQuery<ItemModelForMobileFilter>(query).ToList();
                return userList;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string saveUserWiseTaskFromat(RMPurchaseModel model)
        {
            var info = _iWorkContext.CurrentUserinformation;
            try
            {
                var result = string.Empty;
                var query = string.Empty;
                var msgFormat = model.MSG_FORMAT;
                var modelQuery = model.QUERY.Replace("''", "\'");
                var RMPurchaseResult = _dbContext.SqlQuery<PURCHASE_RM_MODEL>(modelQuery).ToList();
                if (RMPurchaseResult.Count() > 0)
                {
                    var msg = string.Empty;
                    var taskId = "(SELECT COALESCE(MAX(TASK_ID)+1, MAX(TASK_ID) + 1, 1) FROM MOBILE_WEB_SETUP)";
                    if (model.PURCHASE_RM.ToString() == "BR")
                    {
                        var wordList = msgFormat.Split(':');
                        var firstmsg = wordList[0];
                        var secondMsg = wordList[1];
                        var finalMsg = string.Empty;
                        var count = 0;
                        foreach (var bitem in RMPurchaseResult)
                        {
                            count = count + 1;
                            if (secondMsg.Contains("#PARTY_NAME#"))
                                secondMsg = secondMsg.Replace("#PARTY_NAME#", bitem.PARTY_NAME);
                            if (secondMsg.Contains("#BG_AMOUNT#"))
                                secondMsg = secondMsg.Replace("#BG_AMOUNT#", bitem.BG_AMOUNT.ToString());
                            if (secondMsg.Contains("#ALERT_PRIOR_DAYS#"))
                                secondMsg = secondMsg.Replace("#ALERT_PRIOR_DAYS#", bitem.ALERT_PRIOR_DAYS.ToString());
                            if (secondMsg.Contains("#END_DATE#"))
                                secondMsg = secondMsg.Replace("#END_DATE#", bitem.END_DATE == null ? null : Convert.ToDateTime(bitem.END_DATE).ToShortDateString());
                            if (secondMsg.Contains("#ACC_CODE#"))
                                secondMsg = secondMsg.Replace("#ACC_CODE#", bitem.ACC_CODE);
                            if (secondMsg.Contains("#BANK_GNO#"))
                                secondMsg = secondMsg.Replace("#BANK_GNO#", bitem.BANK_GNO);
                            if (count < RMPurchaseResult.Count())
                                secondMsg += secondMsg + ", ";
                            finalMsg += secondMsg.ToString();
                            secondMsg = wordList[1];
                        }
                        msg = firstmsg + finalMsg;
                        msgFormat = msg;
                    }
                    else
                    {
                        msg = string.Join(",", RMPurchaseResult.Select(p => p.ITEM_EDESC + " - " + p.QTY));
                    }
                    msgFormat = msgFormat.Replace("#message#", msg);
                    msgFormat = msgFormat.Replace("'", "''");
                    var hasMessageQry = $@"SELECT COUNT(*)COUNT FROM MOBILE_WEB_SETUP WHERE TASK_NAME='{model.PURCHASE_RM}' AND USER_NO IN ('{String.Join("','", model.USER_NO.Select(p => p.USER_NO))}')";
                    var hasMessage = _dbContext.SqlQuery<int>(hasMessageQry).First();
                    if (hasMessage > 0)
                    {
                        var delQry = $@"DELETE FROM MOBILE_WEB_SETUP WHERE TASK_NAME='{model.PURCHASE_RM}' AND USER_NO IN ('{String.Join("','", model.USER_NO.Select(p => p.USER_NO))}')";
                        _dbContext.ExecuteSqlCommand(delQry);
                    }
                    foreach (var user in model.USER_NO)
                    {
                        var today = System.DateTime.Today.ToString("MM-dd-yyyy");
                        taskId = $@"'{model.PURCHASE_RM + "_" + user.USER_NO + "_" + today}'";
                        var getQry = $@" SELECT {taskId},'{model.PURCHASE_RM}','{msgFormat}','{user.USER_NO}','{info.company_code}','{info.User_id}','{model.QUERY}',SYSDATE,'N' FROM DUAL UNION ALL ";
                        query += getQry;
                    }
                    if (string.IsNullOrEmpty(query))
                        return result = "Plese Select User";
                    string executeQuery = $@"INSERT INTO MOBILE_WEB_SETUP(TASK_ID,TASK_NAME,MESSAGE_FORMAT,USER_NO,COMPANY_CODE,CREATED_BY, QUERY,CREATED_DATE,DELETED_FLAG) {query}";

                    executeQuery = executeQuery.Substring(0, executeQuery.Length - 10);
                    var response = _dbContext.ExecuteSqlCommand(executeQuery);
                    if (response > 0)
                        result = "Success";
                }
                else
                {
                    result = "There is no data generated by query. So, this message not saved.";
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string saveBRTaskMsgFormat(RMPurchaseModel model)
        {
            var info = _iWorkContext.CurrentUserinformation;
            try
            {
                var result = string.Empty;
                var msgFormat = model.MSG_FORMAT;
                var modelQuery = model.QUERY.Replace("''", "\'");
                var RMPurchaseResult = _dbContext.SqlQuery<PURCHASE_RM_MODEL>(modelQuery).ToList();
                if (RMPurchaseResult.Count() > 0)
                {
                    var taskId = "";
                    if (model.PURCHASE_RM.ToString() == "BR")
                    {
                        var wordList = msgFormat;
                        var secondMsg = wordList;
                        foreach (var bitem in RMPurchaseResult)
                        {
                            var msg = string.Empty;
                            var finalMsg = string.Empty;
                            if (secondMsg.Contains("#PARTY_NAME#"))
                                secondMsg = secondMsg.Replace("#PARTY_NAME#", bitem.PARTY_NAME);
                            if (secondMsg.Contains("#BG_AMOUNT#"))
                                secondMsg = secondMsg.Replace("#BG_AMOUNT#", bitem.BG_AMOUNT.ToString());
                            if (secondMsg.Contains("#ALERT_PRIOR_DAYS#"))
                                secondMsg = secondMsg.Replace("#ALERT_PRIOR_DAYS#", bitem.ALERT_PRIOR_DAYS.ToString());
                            if (secondMsg.Contains("#END_DATE#"))
                                secondMsg = secondMsg.Replace("#END_DATE#", bitem.END_DATE == null ? null : Convert.ToDateTime(bitem.END_DATE).ToShortDateString());
                            if (secondMsg.Contains("#ACC_CODE#"))
                                secondMsg = secondMsg.Replace("#ACC_CODE#", bitem.ACC_CODE);
                            if (secondMsg.Contains("#BANK_GNO#"))
                                secondMsg = secondMsg.Replace("#BANK_GNO#", bitem.BANK_GNO);
                            finalMsg += secondMsg.ToString();
                            secondMsg = wordList;
                            msg =  finalMsg;
                            msgFormat = msg;
                            taskId = bitem.BANK_GNO.ToString();
                            msgFormat = msgFormat.Replace("'", "''");
                            var hasMessageQry = $@"SELECT COUNT(*)COUNT FROM MOBILE_WEB_SETUP WHERE TASK_NAME='{model.PURCHASE_RM}' AND TASK_ID='{taskId}'";
                            var hasMessage = _dbContext.SqlQuery<int>(hasMessageQry).First();
                            if (hasMessage > 0)
                            {
                                var delQry = $@"DELETE FROM MOBILE_WEB_SETUP WHERE TASK_NAME='{model.PURCHASE_RM}' AND TASK_ID='{taskId}'";
                                _dbContext.ExecuteSqlCommand(delQry);
                            }
                            var query = string.Empty;
                            foreach (var user in model.USER_NO)
                            {
                                var getQry = $@" SELECT '{taskId}','{model.PURCHASE_RM}','{msgFormat}','{user.USER_NO}','{info.company_code}','{info.User_id}','{model.QUERY}',SYSDATE,'N' FROM DUAL UNION ALL ";
                                query += getQry;
                            }
                            if (string.IsNullOrEmpty(query))
                                return result = "Plese Select User";
                            string executeQuery = $@"INSERT INTO MOBILE_WEB_SETUP(TASK_ID,TASK_NAME,MESSAGE_FORMAT,USER_NO,COMPANY_CODE,CREATED_BY, QUERY,CREATED_DATE,DELETED_FLAG) {query}";

                            executeQuery = executeQuery.Substring(0, executeQuery.Length - 10);
                            var response = _dbContext.ExecuteSqlCommand(executeQuery);
                            if (response > 0)
                                result = "Success";
                        }
                    }
                }
                else
                {
                    result = "There is no data generated by query. So, this message not saved.";
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string saveFilterDataForMobile(MobileWebFilterModel model)
        {
            var result = string.Empty;
            try
            {
                var hasMessageQry = $@"SELECT COUNT(*)COUNT FROM MOBILE_ITEM_FILTER WHERE FILTER_TYPE='{model.TYPE}'";
                var hasMessage = _dbContext.SqlQuery<int>(hasMessageQry).First();
                if (hasMessage > 0)
                {
                    var delQry = $@"DELETE FROM MOBILE_ITEM_FILTER WHERE FILTER_TYPE='{model.TYPE}'";
                    _dbContext.ExecuteSqlCommand(delQry);
                }
                if (string.IsNullOrEmpty(model.ITEM_CODE))
                    model.ITEM_CODE = "";
                var items = $@"{model.ITEM_CODE.ToString()}";
                items = items.Replace("'", "''");
                string executeQuery = $@"INSERT INTO MOBILE_ITEM_FILTER(FILTER_ID,FILTER_TYPE,ITEM_CODE) VALUES((SELECT COALESCE(MAX(FILTER_ID)+1, MAX(FILTER_ID) + 1, 1) FROM MOBILE_ITEM_FILTER),'{model.TYPE}','{items}')";
                var response = _dbContext.ExecuteSqlCommand(executeQuery);
                if (response > 0)
                    result = "Success";
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<ItemModelForMobileFilter> getAllSavedItems()
        {
            try
            {
                var query = $@"SELECT * FROM MOBILE_ITEM_FILTER WHERE FILTER_TYPE='SALES'";
                return _dbContext.SqlQuery<ItemModelForMobileFilter>(query).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}