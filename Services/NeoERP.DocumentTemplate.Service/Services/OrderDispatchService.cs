using NeoErp.Core;
using NeoErp.Core.Models;
using NeoErp.Core.Models.Log4NetLoggin;
using NeoErp.Data;
using NeoERP.DocumentTemplate.Service.Interface;
using NeoERP.DocumentTemplate.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoERP.DocumentTemplate.Service.Services
{
    public class OrderDispatchService : IOrderDispatch
    {
        private IDbContext _dbContext;
        private IWorkContext _workContext;
        private DefaultValueForLog _defaultValueForLog;
        private ILogErp _logErp;
        private NeoErpCoreEntity _objectEntity;

        public OrderDispatchService(IWorkContext workContext, IDbContext dbContext, NeoErpCoreEntity objectEntity)
        {
            this._workContext = workContext;
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            _logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule,_defaultValueForLog.FormCode);
        }


        #region DISPATCH ORDER API IMPLEMENTATION
        public List<OrderDispatchModel> FindAllDataToDispatch(SearchParameter searchParameter)
        {
            List<OrderDispatchModel> dispatchModels = new List<OrderDispatchModel>();
            try
            {
                List<string> compList = new List<string>();
                List<string> branList = new List<string>();
                string dispatchQuery = "";
                if (searchParameter.CompBrnhList.Count > 0)
                {
                    foreach(var cml in searchParameter.CompBrnhList)
                    {
                        compList.Add(cml.Company);
                        branList.Add(cml.Branch);
                    }
                }
                else
                {
                    compList.Add(_workContext.CurrentUserinformation.company_code);
                    branList.Add(_workContext.CurrentUserinformation.branch_code);
                }
                //dispatchQuery = $@"SELECT  to_char(rownum) as DISPATCH_NO, ORDER_NO,TO_CHAR(ORDER_DATE,'DD-MON-YYYY') AS ORDER_DATE, FORM_CODE, ITEM_CODE, QUANTITY, MU_CODE, UNIT_PRICE, SERIAL_NO, MANUAL_NO, CUSTOMER_CODE, BS_DATE(ORDER_DATE) MITI, DUE_QTY, 
                //                          CUSTOMER_EDESC, PARTY_TYPE_CODE, ITEM_EDESC, ADDRESS, AGENT_CODE, PARTY_TYPE_EDESC,company_code FROM V$SALES_ORDER_ANALYSIS 
                //                          WHERE COMPANY_CODE IN('{string.Join(",", compList)}') AND BRANCH_CODE IN('{string.Join(",", branList)}')  
                //                          AND DELETED_FLAG='N' AND (VOUCHER_CANCEL_FLAG='N' OR VOUCHER_CANCEL_FLAG IS NULL) AND FORM_CODE IN('{searchParameter.DocumentCode}')  
                //                          AND TO_DATE(TO_CHAR(ORDER_DATE,'DD-MON-YYYY')) >= TO_DATE('{searchParameter.FromDate}','DD-MON-YYYY') 
                //                          AND TO_DATE(TO_CHAR(ORDER_DATE,'DD-MON-YYYY')) <= TO_DATE('{searchParameter.PlanningDate}','DD-MON-YYYY') 
                //                          AND DUE_QTY > 0 AND AUTHORISED_BY IS NOT NULL 
                //                          ORDER BY FORM_CODE,ORDER_NO,SERIAL_NO,ORDER_DATE desc";

                dispatchQuery = $@"SELECT TO_CHAR(ROWNUM) AS DISPATCH_NO,
                                 SOA.ORDER_NO,
                                 TO_CHAR(SOA.ORDER_DATE, 'DD-MON-YYYY') AS ORDER_DATE,
                                 SOA.FORM_CODE,
                                 (SELECT FORM_EDESC FROM FORM_SETUP WHERE FORM_CODE = SOA.FORM_CODE AND ROWNUM = 1) AS FORM_EDESC,
                                 SOA.ITEM_CODE,
                                 SOA.QUANTITY,
                                 ROUND(SOA.QUANTITY * (IUS.CONVERSION_FACTOR/IUS.FRACTION),6) AS QUANTITY_2ND,
                                 SOA.MU_CODE,
                                 SOA.UNIT_PRICE,
                                 SOA.SERIAL_NO,
                                 SOA.MANUAL_NO,
                                 SOA.CUSTOMER_CODE,
                                 BS_DATE(SOA.ORDER_DATE) MITI,
                                 SOA.DUE_QTY,
                                 SOA.CUSTOMER_EDESC,
                                 SOA.PARTY_TYPE_CODE,
                                 SOA.ITEM_EDESC,
                                 SOA.ADDRESS,
                                 SOA.AGENT_CODE,
                                 SOA.PARTY_TYPE_EDESC,
                                 SOA.company_code,
                                 IUS.CONVERSION_FACTOR,
                                 IUS.FRACTION
                           FROM
                                 V$SALES_ORDER_ANALYSIS SOA
                           LEFT JOIN
                                 IP_ITEM_UNIT_SETUP IUS ON IUS.COMPANY_CODE = SOA.COMPANY_CODE AND IUS.ITEM_CODE = SOA.ITEM_CODE AND IUS.SERIAL_NO = 1
                     WHERE
                         SOA.BRANCH_CODE IN ({string.Join(",", searchParameter.BranchCodeList)})
                         AND SOA.DELETED_FLAG = 'N'
                         AND (SOA.VOUCHER_CANCEL_FLAG = 'N' OR SOA.VOUCHER_CANCEL_FLAG IS NULL)
                         AND SOA.FORM_CODE IN ({searchParameter.DocumentCode})
                         AND SOA.ORDER_DATE >= TO_DATE('{searchParameter.FromDate}', 'DD-MON-YYYY')
                         AND SOA.ORDER_DATE <= TO_DATE('{searchParameter.PlanningDate}', 'DD-MON-YYYY')
                         AND SOA.DUE_QTY > 0
                         AND SOA.AUTHORISED_BY IS NOT NULL
                     ORDER BY
                         SOA.FORM_CODE,
                         SOA.ORDER_NO,
                         SOA.SERIAL_NO,
                         SOA.ORDER_DATE DESC";

                dispatchModels = this._dbContext.SqlQuery<OrderDispatchModel>(dispatchQuery).ToList();
                foreach(var dispatch in dispatchModels)
                {
                    dispatch.VOUCHER_NO = dispatch.ORDER_NO;
                    decimal? chalanQty = 0;
                    decimal? invoiceQty = 0;
                    decimal? totalPlanningQty = 0;
                    double? pendingtoDispatch = 0;
                    if(!string.IsNullOrEmpty(dispatch.VOUCHER_NO))
                    {
                        var salesChalan=$@"SELECT SUM(NVL(QUANTITY,0)) as TotalQty, SUM(NVL(TOTAL_PRICE, 0)) as TotalAmount  FROM SA_SALES_CHALAN 
                                  WHERE COMPANY_CODE='{dispatch.COMPANY_CODE}' AND CHALAN_NO||SERIAL_NO IN ('{ dispatch.VOUCHER_NO}' ) AND FORM_CODE IN ('{dispatch.FORM_CODE}') 
                                  AND ITEM_CODE='{dispatch.ITEM_CODE}' AND CUSTOMER_CODE='{dispatch.CUSTOMER_CODE}'";
                        var chalanRecord = _dbContext.SqlQuery<DispatchSalesMain>(salesChalan).FirstOrDefault();
                        if(chalanRecord!=null)
                        {
                            if(chalanRecord.TotalQty<=0||chalanRecord.TotalQty==null)
                            {
                                var salesinvoice = $@"SELECT SUM(NVL(QUANTITY,0)) as TotalQty, SUM(NVL(TOTAL_PRICE, 0)) as TotalAmount  FROM SA_SALES_INVOICE  
                                  WHERE COMPANY_CODE='{dispatch.COMPANY_CODE}' AND SALES_NO||SERIAL_NO IN ('{ dispatch.VOUCHER_NO}' ) AND FORM_CODE IN ('{dispatch.FORM_CODE}') 
                                  AND ITEM_CODE='{dispatch.ITEM_CODE}' AND CUSTOMER_CODE='{dispatch.CUSTOMER_CODE}'";
                                var invoiceRecord = _dbContext.SqlQuery<DispatchSalesMain>(salesinvoice).FirstOrDefault();
                                if(invoiceRecord!=null)
                                {
                                    chalanRecord.TotalQty = invoiceRecord.TotalQty==null?0:invoiceRecord.TotalQty;
                                    invoiceQty = chalanRecord.TotalQty;
                                    chalanQty = invoiceQty;
                                }
                            }
                            else
                            {
                                chalanQty = chalanRecord.TotalQty;
                            }
                        }

                        var totalDispatch=$@"SELECT SUM(NVL(QUANTITY,0)) TotalQty FROM ORDER_DISPATCH_SCHEDULE 
                                            WHERE COMPANY_CODE='{dispatch.COMPANY_CODE}' AND ORDER_NO = '{dispatch.VOUCHER_NO}' AND ITEM_CODE='{dispatch.ITEM_CODE}' 
                                            AND ACKNOWLEDGE_FLAG IS NULL";
                        var dispatchRecod = _dbContext.SqlQuery<DispatchSalesMain>(totalDispatch).FirstOrDefault();
                        if(dispatchRecod!=null)
                        {
                            totalPlanningQty = dispatchRecod.TotalQty == null ? 0 : dispatchRecod.TotalQty;
                        }
                    }
               
                    pendingtoDispatch =  (totalPlanningQty.HasValue && chalanQty.HasValue) ? Math.Round(Convert.ToDouble(totalPlanningQty.Value) - Convert.ToDouble(chalanQty.Value), 6) : 0.0;
                    dispatch.PendingToDispatch = pendingtoDispatch.HasValue ? Convert.ToDecimal(pendingtoDispatch.Value) : 0;  
                    dispatch.PendingToDispatch2ND = (dispatch.CONVERSION_FACTOR.HasValue && dispatch.FRACTION.HasValue && dispatch.PendingToDispatch.HasValue) ? Math.Round(dispatch.PendingToDispatch.Value * (dispatch.CONVERSION_FACTOR.Value / dispatch.FRACTION.Value), 6) : 0;
                    dispatch.PendingToPlanning = dispatch.QUANTITY- totalPlanningQty;
                    dispatch.PendingToPlanning2ND = (dispatch.CONVERSION_FACTOR.HasValue && dispatch.FRACTION.HasValue && dispatch.PendingToPlanning.HasValue) ? Math.Round(dispatch.PendingToPlanning.Value * (dispatch.CONVERSION_FACTOR.Value / dispatch.FRACTION.Value), 6) : 0;

                    string stockQuery = $@"SELECT SUM(IN_QUANTITY - OUT_QUANTITY) STOCK FROM V$VIRTUAL_STOCK_WIP_LEDGER1  
                                            WHERE COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'  
                                            AND ITEM_CODE = '{dispatch.ITEM_CODE}'  
                                            AND BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' GROUP BY ITEM_CODE, COMPANY_CODE";
                    dispatch.STOCK = _dbContext.SqlQuery<decimal?>(stockQuery).FirstOrDefault();
                    dispatch.STOCK_2ND = (dispatch.CONVERSION_FACTOR.HasValue && dispatch.FRACTION.HasValue && dispatch.STOCK.HasValue) ? Math.Round(dispatch.STOCK.Value * (dispatch.CONVERSION_FACTOR.Value / dispatch.FRACTION.Value), 6) : 0;
                }

             //   var data = dispatchModels.Where(x => Convert.ToDouble(x.PendingToPlanning) > 0).ToList();
                return dispatchModels;
            }catch(Exception ex)
            {
                _logErp.ErrorInDB("Error while getting all dispatch data " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<OrderDispatchModel> GetAllDispatchPlannedReport(SearchParameter searchParameter)
        {
            var companyCode = this._workContext.CurrentUserinformation.company_code;
            var branchCode = this._workContext.CurrentUserinformation.branch_code;
            try
            {
                // var planningQuery = $@"select ORDER_NO,TO_CHAR(VOUCHER_DATE,'DD-MON-YYYY') AS VOUCHER_DATE,FORM_CODE,ITEM_CODE,QUANTITY,UNIT_PRICE,BS_DATE(VOUCHER_DATE) AS MITI,PENDING_DESPATCH_QTY,CUSTOMER_CODE,PARTY_TYPE_CODE,TO_LOCATION from order_dispatch_schedule where form_code='{searchParameter.DocumentCode}' and company_code='{companyCode}' and branch_code='{branchCode}' and deleted_flag='N' and dispatch_flag='Y' and acknowledge_flag='Y'";
                var planningQuery = $@"SELECT sa.ORDER_NO, TO_CHAR(ods.VOUCHER_DATE,'DD-MON-YYYY') AS VOUCHER_DATE, sa.FORM_CODE, sa.ITEM_CODE, sa.QUANTITY, sa.MU_CODE, sa.UNIT_PRICE, sa.SERIAL_NO, sa.MANUAL_NO, sa.CUSTOMER_CODE, BS_DATE(ods.VOUCHER_DATE) MITI, sa.DUE_QTY, 
                                       sa.CUSTOMER_EDESC, sa.PARTY_TYPE_CODE, sa.ITEM_EDESC, sa.ADDRESS, sa.AGENT_CODE, sa.PARTY_TYPE_EDESC FROM V$SALES_ORDER_ANALYSIS sa
                                      INNER JOIN ORDER_DISPATCH_SCHEDULE ods on sa.ORDER_NO=ods.ORDER_NO
                                      where ods.form_code='{searchParameter.DocumentCode}' and ods.company_code='{companyCode}' and ods.branch_code='{branchCode}' and ods.deleted_flag='N' and ods.dispatch_flag='Y' and ods.acknowledge_flag='Y'";
                var plannedReport = this._dbContext.SqlQuery<OrderDispatchModel>(planningQuery).ToList();
                return plannedReport;
            }catch(Exception ex)
            {
                _logErp.ErrorInDB("Error while getting planned report" + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<DispatchDocument> ListAllDocument()
        {
            List<DispatchDocument> docList = new List<DispatchDocument>();
            try
            {
                string docQuery = $@"select FA.FORM_CODE,A.FORM_EDESC from form_DETAIL_setup fA,FORM_SETUP A where FA.FORM_CODE=A.FORM_CODE AND FA.COMPANY_CODE=A.COMPANY_CODE AND  FA.table_name ='SA_SALES_ORDER'  AND a.company_code='01' and a.deleted_flag='N'  GROUP BY FA.FORM_CODE,A.FORM_EDESC";
                docList = this._dbContext.SqlQuery<DispatchDocument>(docQuery).ToList();
                return docList;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting all dispatch document " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<DispatchDocument> ListDocumentByBranches(string branchCodeList)
        {
            List<DispatchDocument> docList = new List<DispatchDocument>();
            try
            {
                string docQuery = $@"select FA.FORM_CODE,A.FORM_EDESC from form_DETAIL_setup fA,FORM_SETUP A 
                                    where FA.FORM_CODE=A.FORM_CODE AND FA.COMPANY_CODE=A.COMPANY_CODE AND  FA.table_name ='SA_SALES_ORDER'  
                                    AND a.company_code='{_workContext.CurrentUserinformation.company_code}' and a.deleted_flag='N'  
                                    AND A.FORM_CODE IN (SELECT FORM_CODE FROM FORM_BRANCH_MAP WHERE BRANCH_CODE IN ({branchCodeList}))
                                    GROUP BY FA.FORM_CODE,A.FORM_EDESC";
                docList = this._dbContext.SqlQuery<DispatchDocument>(docQuery).ToList();
                return docList;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting all dispatch document " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public int? GenerateDispatchNo()
        {
            int? dispatchNo = 0;
            try
            {
                string noGenerationgQuery = $@"select max(to_number(dispatch_no))+1 from ORDER_DISPATCH_SCHEDULE";
                dispatchNo = _dbContext.SqlQuery<int?>(noGenerationgQuery).FirstOrDefault();
                return dispatchNo;

            }catch(Exception ex)
            {
                _logErp.ErrorInDB("Error while generating dispatch no " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public string GetDispatcher()
        {
            try
            {
                return _workContext.CurrentUserinformation.branch_name;
            }catch(Exception ex)
            {
                _logErp.ErrorInDB("Error while getting dispatcher : " + ex.StackTrace);
                throw new Exception(ex.Message);

            }
        }

        public string SaveDispatchedOrder(List<OrderDispatchModel> model)
        {
            try
            {
                string responseMessage = "";
                OrderDispatchModel dispatchModel = new OrderDispatchModel();
                var companyCode = this._workContext.CurrentUserinformation.company_code;
                var branchCode = this._workContext.CurrentUserinformation.branch_code;
                var createdBy = this._workContext.CurrentUserinformation.login_code;
                if(model.Count > 0)
                {
                    foreach(var m in model)
                    {
                        double planningQty = 0;

                        Double.TryParse(m.PLANNING_QTY, out planningQty);

                        if (planningQty<=0)
                        {
                            responseMessage = "MINIMUM_PLANNING_QUANTITY_NOT_MET";
                            continue;
                        }
                        dispatchModel.DISPATCH_NO = m.DISPATCH_NO;
                        dispatchModel.ORDER_DATE = m.ORDER_DATE;
                        dispatchModel.VOUCHER_DATE = string.IsNullOrEmpty(m.VOUCHER_DATE) ? "SYSDATE" : m.VOUCHER_DATE;
                        dispatchModel.VOUCHER_NO = m.VOUCHER_NO;
                        dispatchModel.ORDER_NO = m.ORDER_NO;
                        dispatchModel.CUSTOMER_CODE = m.CUSTOMER_CODE;
                        dispatchModel.FROM_LOCATION = m.FROM_LOCATION;
                        dispatchModel.TO_LOCATION = m.TO_LOCATION;
                        dispatchModel.DISPATCH_FLAG = "Y";
                        dispatchModel.QUANTITY = m.QUANTITY;
                        dispatchModel.COMPANY_CODE = string.IsNullOrEmpty(m.COMPANY_CODE) ? companyCode : m.COMPANY_CODE;
                        dispatchModel.BRANCH_CODE = string.IsNullOrEmpty(m.BRANCH_CODE) ? branchCode : m.BRANCH_CODE;
                        dispatchModel.DELETED_FLAG = "N";
                        dispatchModel.CREATED_BY = string.IsNullOrEmpty(m.CREATED_BY) ? createdBy: m.CREATED_BY;
                        dispatchModel.CREATED_DATE = (m.CREATED_DATE !=null) ? m.CREATED_BY : "SYSDATE";
                        dispatchModel.MODIFY_DATE = (m.MODIFY_DATE !=null) ? m.MODIFY_DATE : "SYSDATE";
                        dispatchModel.PARTY_TYPE_CODE = m.PARTY_TYPE_CODE;
                        dispatchModel.DUE_QTY = m.DUE_QTY;
                        dispatchModel.ACKNOWLEDGE_FLAG = m.ACKNOWLEDGE_FLAG;
                        dispatchModel.UNIT_PRICE = m.UNIT_PRICE;
                        dispatchModel.QUANTITY = m.QUANTITY;
                        dispatchModel.PENDING_TO_DISPATCH = m.PENDING_TO_DISPATCH;
                        dispatchModel.TRANS_NO = m.TRANS_NO;
                        dispatchModel.FORM_CODE = m.FORM_CODE;
                        dispatchModel.ITEM_CODE = m.ITEM_CODE;

                        string dispatchInsertQuery = $@"INSERT INTO ORDER_DISPATCH_SCHEDULE(DISPATCH_NO,VOUCHER_DATE,ORDER_NO,CUSTOMER_CODE,FROM_LOCATION,TO_LOCATION,DISPATCH_FLAG,QUANTITY,COMPANY_CODE,BRANCH_CODE,
                                              DELETED_FLAG,CREATED_BY,CREATED_DATE,MODIFY_DATE,PARTY_TYPE_CODE,PENDING_QTY,ACKNOWLEDGE_FLAG,UNIT_PRICE,ORDER_QTY,PENDING_DESPATCH_QTY,TRANS_NO,FORM_CODE,ITEM_CODE) 
                                             VALUES('{m.MANUAL_NO}',trunc(sysdate),'{dispatchModel.VOUCHER_NO}','{dispatchModel.CUSTOMER_CODE}','{dispatchModel.FROM_LOCATION}','{dispatchModel.TO_LOCATION}',
                                                    '{dispatchModel.DISPATCH_FLAG}',{m.PLANNING_QTY},'{dispatchModel.COMPANY_CODE}','{dispatchModel.BRANCH_CODE}','{dispatchModel.DELETED_FLAG}','{dispatchModel.CREATED_BY}',
                                                    sysdate,sysdate,'{dispatchModel.PARTY_TYPE_CODE}',{dispatchModel.QUANTITY},'{dispatchModel.ACKNOWLEDGE_FLAG}',{dispatchModel.UNIT_PRICE},
                                                    {dispatchModel.QUANTITY},{dispatchModel.PENDING_TO_DISPATCH},{dispatchModel.TRANS_NO},'{dispatchModel.FORM_CODE}','{dispatchModel.ITEM_CODE}')";
                        var result = this._objectEntity.ExecuteSqlCommand(dispatchInsertQuery);
                        responseMessage = "Saved Successfully";
                    }
                }
                return responseMessage;

               
            }catch(Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        public List<DispatchDealer> GetAllDealerType()
        {
            try
            {
                string dealerTypeQuery = $@"SELECT IPTC.PARTY_TYPE_CODE,IPTC.PARTY_TYPE_CODE as id,IPTC.PARTY_TYPE_EDESC,IPTC.PARTY_TYPE_EDESC as label,IPTC.COMPANY_CODE,IPTC.CREATED_BY,IPTC.CREATED_DATE,IPTC.ACC_CODE,IPTC.CREDIT_LIMIT,IPTC.CREDIT_DAYS
                                           FROM IP_PARTY_TYPE_CODE IPTC WHERE IPTC.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND IPTC.DELETED_FLAG='N' AND IPTC.PARTY_TYPE_FLAG='D'";
                var dealerData = _objectEntity.SqlQuery<DispatchDealer>(dealerTypeQuery).ToList();
                return dealerData;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting dealer: " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }


        #endregion


        #region LOADING SLIP API IMPLEMENTATION

        public List<OrderDispatchModel> GetAllDispatchForLoadingSlip(string transactionDate)
        {
            var companyCode = this._workContext.CurrentUserinformation.company_code;
            var branchCode = this._workContext.CurrentUserinformation.branch_code;
            string planningQuery = string.Empty;
            try
            {
                if (transactionDate == null)
                {
                    planningQuery = $@"SELECT 
                                            (T1.QUANTITY - NVL(
                                                (
                                                    SELECT SUM(NVL(QUANTITY, 0))
                                                    FROM SA_LOADING_SLIP_DETAIL
                                                    WHERE REFERENCE_NO = T1.ORDER_NO
                                                        AND COMPANY_CODE = T1.COMPANY_CODE
                                                        AND DISPATCH_NO = T1.DISPATCH_NO
                                                ),
                                                0
                                            )) AS LS_PENDING_QTY,
                                            T1.*
                                        FROM (
                                            SELECT
                                                A.COMPANY_CODE,
                                                A.DISPATCH_NO,
                                                A.ORDER_NO,
                                                A.CUSTOMER_CODE,
                                                NVL(SUM(A.QUANTITY), 0) AS QUANTITY,
                                                A.FORM_CODE,
                                                TO_CHAR(A.VOUCHER_DATE, 'DD-MON-YYYY') AS VOUCHER_DATE,
                                                FN_FETCH_DESC(A.COMPANY_CODE, 'SA_CUSTOMER_SETUP', A.CUSTOMER_CODE) AS CUSTOMER_EDESC,
                                                A.TO_LOCATION,
                                                sa.ITEM_CODE,
                                                sa.MU_CODE,
                                                sa.UNIT_PRICE,
                                                sa.SERIAL_NO,
                                                sa.MANUAL_NO,
                                                BS_DATE(A.VOUCHER_DATE) AS MITI,
                                                sa.DUE_QTY,
                                                sa.PARTY_TYPE_CODE,
                                                sa.ITEM_EDESC,
                                                sa.ADDRESS,
                                                sa.AGENT_CODE,
                                                sa.PARTY_TYPE_EDESC,
                                                A.TO_LOCATION AS DESTINATION
                                            FROM
                                                ORDER_DISPATCH_SCHEDULE A
                                            INNER JOIN
                                                V$SALES_ORDER_ANALYSIS sa ON A.ORDER_NO = sa.ORDER_NO
                                                AND A.COMPANY_CODE = sa.COMPANY_CODE
                                                AND A.CUSTOMER_CODE = sa.CUSTOMER_CODE
                                                AND A.ITEM_CODE = sa.ITEM_CODE
                                            WHERE
                                                A.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                                                AND A.VOUCHER_DATE >= TO_DATE('{DateTime.Now.AddDays(-15).ToString("dd-MM-yyyy")}', 'DD-MM-YYYY')
                                                AND A.VOUCHER_DATE <= TO_DATE('{DateTime.Now.Date.ToString("dd-MM-yyyy")}', 'DD-MM-YYYY')
                                            GROUP BY
                                                A.COMPANY_CODE,
                                                A.DISPATCH_NO,
                                                A.ORDER_NO,
                                                A.CUSTOMER_CODE,
                                                A.FORM_CODE,
                                                A.VOUCHER_DATE,
                                                A.TO_LOCATION,
                                                sa.ITEM_CODE,
                                                sa.MU_CODE,
                                                sa.UNIT_PRICE,
                                                sa.SERIAL_NO,
                                                sa.MANUAL_NO,
                                                BS_DATE(A.VOUCHER_DATE),
                                                sa.DUE_QTY,
                                                sa.PARTY_TYPE_CODE,
                                                sa.ITEM_EDESC,
                                                sa.ADDRESS,
                                                sa.AGENT_CODE,
                                                sa.PARTY_TYPE_EDESC,
                                                A.TO_LOCATION
                                            ORDER BY
                                                A.TO_LOCATION,
                                                A.VOUCHER_DATE DESC,
                                                A.DISPATCH_NO,
                                                A.FORM_CODE,
                                                A.ORDER_NO DESC
                                        ) T1
                                    ";
                }
                else
                {
                    planningQuery = $@"SELECT 
                                            (T1.QUANTITY - NVL(
                                                (
                                                    SELECT SUM(NVL(QUANTITY, 0))
                                                    FROM SA_LOADING_SLIP_DETAIL
                                                    WHERE REFERENCE_NO = T1.ORDER_NO
                                                        AND COMPANY_CODE = T1.COMPANY_CODE
                                                        AND DISPATCH_NO = T1.DISPATCH_NO
                                                ),
                                                0
                                            )) AS LS_PENDING_QTY,
                                            T1.*
                                        FROM (
                                            SELECT
                                                A.COMPANY_CODE,
                                                A.DISPATCH_NO,
                                                A.ORDER_NO,
                                                A.CUSTOMER_CODE,
                                                NVL(SUM(A.QUANTITY), 0) AS QUANTITY,
                                                A.FORM_CODE,
                                                TO_CHAR(A.VOUCHER_DATE, 'DD-MON-YYYY') AS VOUCHER_DATE,
                                                FN_FETCH_DESC(A.COMPANY_CODE, 'SA_CUSTOMER_SETUP', A.CUSTOMER_CODE) AS CUSTOMER_EDESC,
                                                A.TO_LOCATION,
                                                sa.ITEM_CODE,
                                                sa.MU_CODE,
                                                sa.UNIT_PRICE,
                                                sa.SERIAL_NO,
                                                sa.MANUAL_NO,
                                                BS_DATE(A.VOUCHER_DATE) AS MITI,
                                                sa.DUE_QTY,
                                                sa.PARTY_TYPE_CODE,
                                                sa.ITEM_EDESC,
                                                sa.ADDRESS,
                                                sa.AGENT_CODE,
                                                sa.PARTY_TYPE_EDESC,
                                                A.TO_LOCATION AS DESTINATION
                                            FROM
                                                ORDER_DISPATCH_SCHEDULE A
                                            INNER JOIN
                                                V$SALES_ORDER_ANALYSIS sa ON A.ORDER_NO = sa.ORDER_NO
                                                AND A.COMPANY_CODE = sa.COMPANY_CODE
                                                AND A.CUSTOMER_CODE = sa.CUSTOMER_CODE
                                                AND A.ITEM_CODE = sa.ITEM_CODE
                                            WHERE
                                                A.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                                                AND A.VOUCHER_DATE >= TO_DATE('{transactionDate}', 'DD-MON-YYYY')
                                            GROUP BY
                                                A.COMPANY_CODE,
                                                A.DISPATCH_NO,
                                                A.ORDER_NO,
                                                A.CUSTOMER_CODE,
                                                A.FORM_CODE,
                                                A.VOUCHER_DATE,
                                                A.TO_LOCATION,
                                                sa.ITEM_CODE,
                                                sa.MU_CODE,
                                                sa.UNIT_PRICE,
                                                sa.SERIAL_NO,
                                                sa.MANUAL_NO,
                                                BS_DATE(A.VOUCHER_DATE),
                                                sa.DUE_QTY,
                                                sa.PARTY_TYPE_CODE,
                                                sa.ITEM_EDESC,
                                                sa.ADDRESS,
                                                sa.AGENT_CODE,
                                                sa.PARTY_TYPE_EDESC,
                                                A.TO_LOCATION 
                                            ORDER BY
                                                A.TO_LOCATION,
                                                A.VOUCHER_DATE DESC,
                                                A.DISPATCH_NO,
                                                A.FORM_CODE,
                                                A.ORDER_NO DESC
                                        ) T1
                                    ";
                }

                var plannedReport = this._dbContext.SqlQuery<OrderDispatchModel>(planningQuery).ToList();
                return plannedReport;

            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting planned report" + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<VehicleRegistrationModel> GetAllRegisteredVehicleToDispatch(string entryDate = "")
        {
            try
            {
                string formattedEntryDate = string.IsNullOrWhiteSpace(entryDate) ? string.Empty : DateTime.Parse(entryDate).ToString("dd/MMM/yyyy");
                string query = $@"SELECT DISTINCT
                                        IVT.TRANSACTION_NO,
                                        IVC.VEHICLE_EDESC AS VEHICLE_NAME,
                                        IVT.TRANSACTION_DATE,
                                        IVT.REMARKS,
                                        NVL(IVT.ACCESS_FLAG,'N') ACCESS_FLAG,
                                        BS_DATE(IVT.TRANSACTION_DATE) AS miti,
                                        IVT.READ_FLAG,
                                        IVT.REFERENCE_NO,
                                        IVT.DESTINATION,
                                        IVT.VEHICLE_IN_DATE,
                                        IVT.VEHICLE_OUT_DATE,
                                        IVT.DRIVER_NAME,
                                        IVT.DRIVER_LICENCE_NO,
                                        IVT.QUANTITY,
                                        IVT.TRANSPORT_NAME,
                                        IVT.DRIVER_MOBILE_NO,
                                        IVT.IN_TIME,
                                        IVT.OUT_TIME,
                                        IVT.LOAD_IN_TIME,
                                        IVT.LOAD_OUT_TIME,
                                        IVT.VEHICLE_OWNER_NAME,
                                        IVT.VEHICLE_OWNER_NO,
                                        IVT.BROKER_NAME,
                                        IVT.TEAR_WT,
                                        IVT.GROSS_WT,
                                        IVT.NET_WT, 
                                        TOTAL_VEHICLE_HR AS TOTAL_VEHICLE_HR,
                                        (SELECT TO_CHAR(LSD.LS_NO) FROM SA_LOADING_SLIP_DETAIL LSD WHERE LSD.TRANSACTION_NO = IVT.TRANSACTION_NO AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND ROWNUM = 1) WB_SLIP_NO,
                                        C.COMPANY_EDESC AS COMPANY_NAME,
                                        C.ADDRESS AS COMPANY_ADDRESS,
                                        C.TELEPHONE AS COMPANY_TEL_NO
                                    FROM
                                        IP_VEHICLE_TRACK IVT
                                    INNER JOIN
                                        IP_VEHICLE_CODE IVC ON IVT.VEHICLE_NAME = IVC.VEHICLE_EDESC AND IVC.COMPANY_CODE = IVT.COMPANY_CODE
                                    INNER JOIN
                                        COMPANY_SETUP C ON C.COMPANY_CODE = IVT.COMPANY_CODE
                                    WHERE
                                        IVT.DELETED_FLAG = 'N'
                                        AND IVT.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                                        AND IVT.BRANCH_CODE = '{_workContext.CurrentUserinformation.branch_code}'
                                        AND TO_DATE(TO_CHAR(TRANSACTION_DATE,'DD-MON-YYYY')) = TO_DATE('{formattedEntryDate}', 'DD/MON/YYYY', 'NLS_DATE_LANGUAGE=ENGLISH')
                                    ORDER BY
                                        TO_NUMBER(IVT.TRANSACTION_NO)";
                var List = _dbContext.SqlQuery<VehicleRegistrationModel>(query).ToList();

                return List;
            }catch(Exception ex)
            {
                _logErp.ErrorInDB("Error while getting registered vehicle");
                throw new Exception(ex.Message);
            }
        }

        public string GenerateLoadingSlip(LoadingSlipModal modal)
        {
            try
            {
                bool isUpdated = false;
                bool isInserted = false;
                var maxLCNum = "0";
                if(modal !=null)
                {
                    string vehicleUpdateQuery = $@"UPDATE IP_VEHICLE_TRACK SET VEHICLE_NAME = '{modal.VEHICLE_NAME}', REMARKS='{modal.REMARKS}', TRANSACTION_DATE = TRUNC(sysdate), 
                    MODIFY_BY = '{_workContext.CurrentUserinformation.login_code}', ACCESS_FLAG = 'Y', ACCESS_BY='{modal.ACCESS_BY}',
                    READ_FLAG = '{modal.READ_FLAG}', ACCESS_DATE=sysdate, MODIFY_DATE=sysdate,DRIVER_NAME='{modal.DRIVER_NAME}',DRIVER_LICENCE_NO='{modal.DRIVER_LICENSE_NO}',
                    DRIVER_MOBILE_NO = '{modal.DRIVER_MOBILE_NO}', IN_TIME = '{modal.IN_TIME}', OUT_TIME = '{modal.OUT_TIME}', LOAD_IN_TIME = sysdate, 
                    GROSS_WT='{modal.GROSS_WT}',TEAR_WT='{modal.TEAR_WT}',NET_WT='{modal.NET_WT}',QUANTITY='{modal.QUANTITY}', TOTAL_VEHICLE_HR ='{modal.TOTAL_VEHICLE_HR}',
                    DESTINATION='{modal.DESTINATION}',BROKER_NAME='{modal.BROKER_NAME}',TRANSPORT_NAME='{modal.TRANSPORT_NAME}'
                    ,REFERENCE_NO='{modal.REFERENCE_NO}', REFERENCE_FORM_CODE='{modal.REFERENCE_FORM_CODE}' where TRANSACTION_NO='{modal.TRANSACTION_NO}'";
                    var updatedRow = _dbContext.ExecuteSqlCommand(vehicleUpdateQuery);
                    isUpdated = true;

                    var maxRowQuery = $@" SELECT nvl(max(ls_no),0) + 1 as LS_NO from SA_LOADING_SLIP_DETAIL";
                    var maxNum = _objectEntity.SqlQuery<int>(maxRowQuery).FirstOrDefault();
                    var DispatchQuery = $@"SELECT DISPATCH_NO,TO_CHAR(VOUCHER_DATE) VOUCHER_DATE,VOUCHER_DATE VOUCHER_DATE_TYPE,LOADING_SLIP_NO,ORDER_NO,CUSTOMER_CODE,FROM_LOCATION,TO_LOCATION,VEHICLE_NO,QUANTITY,COMPANY_CODE,UNIT_PRICE,FORM_CODE,ITEM_CODE FROM ORDER_DISPATCH_SCHEDULE WHERE DISPATCH_NO='{modal.DISPATCH_NO}' ";
                    var AllDispatchData = _objectEntity.SqlQuery<OrderDispatchModel>(DispatchQuery);
                    var serialNo = 0;
                    foreach (var dispatch in AllDispatchData)
                    {
                        serialNo = serialNo + 1;

                        string slipSaveQuery = $@"INSERT INTO SA_LOADING_SLIP_DETAIL(LS_NO,TRANSACTION_NO,SERIAL_NO,REFERENCE_NO,REFERENCE_FORM_CODE,CUSTOMER_CODE,ITEM_CODE,MU_CODE,QUANTITY,UNIT_PRICE,
                                           VEHICLE_NAME,VOUCHER_DATE,CREATED_BY,CREATED_DATE,COMPANY_CODE,BRANCH_CODE,DELETED_FLAG,ACCESS_FLAG,READ_FLAG,DRIVER_NAME,DRIVER_MOBILE_NO,
                                           DRIVER_LICENCE_NO,IN_TIME,OUT_TIME,LOAD_IN_TIME,LOAD_OUT_TIME,ACCESS_BY,ACCESS_DATE,DESTINATION,BROKER_NAME,MODIFY_DATE,DISPATCH_NO,TO_LOCATION)
                                           VALUES('{maxNum}','{modal.TRANSACTION_NO}','{serialNo}','{modal.REFERENCE_NO}','{modal.REFERENCE_FORM_CODE}','{dispatch.CUSTOMER_CODE}','{dispatch.ITEM_CODE}',
                                           '{modal.MU_CODE}','{dispatch.QUANTITY}','{dispatch.UNIT_PRICE}','{modal.VEHICLE_NAME}',to_date('{modal.VOUCHER_DATE.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'{_workContext.CurrentUserinformation.login_code}',SYSDATE,
                                           '{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}','N','{modal.ACCESS_FLAG}','{modal.READ_FLAG}','{modal.DRIVER_NAME}',
                                           '{modal.DRIVER_MOBILE_NO}','','{modal.IN_TIME}','{modal.OUT_TIME}','{modal.LOAD_IN_TIME}','{modal.LOAD_OUT_TIME}','{modal.ACCESS_BY}',
                                           sysdate,'{modal.DESTINATION}','{modal.BROKER_NAME}',SYSDATE,'{modal.DISPATCH_NO}','{modal.TO_LOCATION}')";
                        var insertedRow = _dbContext.ExecuteSqlCommand(slipSaveQuery);


                    }
                  
                    isInserted = true;
                    if(maxNum<0)
                    {
                        maxLCNum = "Insertbutzore";
                    }
                    else
                    {
                        maxLCNum = maxNum.ToString();
                    }
                    string updateorderDispatch = $@"update ORDER_DISPATCH_SCHEDULE set loading_slip_no='{maxNum}' ,VEHICLE_NO='{modal.VEHICLE_NAME}' ,DRIVER_NAME='{modal.DRIVER_NAME}',TRANSPORTER_CODE='{modal.TRANSACTION_NO}',DRIVER_MOBILE_NO='{modal.DRIVER_MOBILE_NO}', loading_slip_date=trunc(sysdate)  where  dispatch_no='{modal.DISPATCH_NO}'";
                    var insertedRowdispatch = _dbContext.ExecuteSqlCommand(updateorderDispatch);

                }
                if(isInserted && isUpdated)
                {
                    
                    return maxLCNum;
                }
                else
                {
                    return "ERROR";
                }

            }catch(Exception ex)
            {
                _logErp.ErrorInDB("Error while generating loaing slip : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region LOADING SLIP PRINTER API IMPLEMENTATION

        public List<LoadingSlipModalForPrint> GetLoadingSlipPrintList()

        {
            try
            {
                string query = $@"SELECT SLSD.LS_NO as LOADING_SLIP_NO,SLSD.TRANSACTION_NO,SLSD.SERIAL_NO,SLSD.REFERENCE_NO,SLSD.REFERENCE_FORM_CODE,SLSD.CUSTOMER_CODE,SCS.CUSTOMER_EDESC as CUSTOMER_EDESC,SLSD.ITEM_CODE,IIMS.ITEM_EDESC,SLSD.MU_CODE,SLSD.QUANTITY,SLSD.UNIT_PRICE,
                                  SLSD.VEHICLE_NAME,TO_DATE(SLSD.VOUCHER_DATE) as VOUCHER_DATE,SLSD.DRIVER_NAME,SLSD.DRIVER_LICENCE_NO,SLSD.TO_LOCATION,SLSD.LOAD_IN_TIME,SLSD.LOAD_OUT_TIME,SLSD.DESTINATION,SLSD.DISPATCH_NO
                                  FROM SA_LOADING_SLIP_DETAIL SLSD
                                  INNER JOIN IP_ITEM_MASTER_SETUP IIMS on IIMS.ITEM_CODE=SLSD.ITEM_CODE
                                  INNER JOIN SA_CUSTOMER_SETUP SCS on SCS.CUSTOMER_CODE=SLSD.CUSTOMER_CODE
                                  WHERE SLSD.REFERENCE_NO IS NOT NULL AND SLSD.REFERENCE_FORM_CODE IS NOT NULL AND SLSD.DELETED_FLAG='N' AND SLSD.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
                var List = _dbContext.SqlQuery<LoadingSlipModalForPrint>(query).ToList();
                return List;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting loading slip for printing : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<LoadingSlipModalForPrint> GetLoadingSlipPrintListByDispatchNo(string dispatchNo)
        {
            try
            {
                string query = $@"SELECT SLSD.LS_NO as LOADING_SLIP_NO,SLSD.TRANSACTION_NO,SLSD.SERIAL_NO,SLSD.REFERENCE_NO,SLSD.REFERENCE_FORM_CODE,SLSD.CUSTOMER_CODE,SCS.CUSTOMER_EDESC as CUSTOMER_EDESC,SLSD.ITEM_CODE,IIMS.ITEM_EDESC,SLSD.MU_CODE,SLSD.QUANTITY,SLSD.UNIT_PRICE,
                                  SLSD.VEHICLE_NAME,SLSD.VOUCHER_DATE,SLSD.DRIVER_NAME,SLSD.LOAD_IN_TIME,SLSD.LOAD_OUT_TIME,SLSD.DESTINATION,SLSD.DISPATCH_NO
                                  FROM SA_LOADING_SLIP_DETAIL SLSD
                                  INNER JOIN IP_ITEM_MASTER_SETUP IIMS on IIMS.ITEM_CODE=SLSD.ITEM_CODE
                                  INNER JOIN SA_CUSTOMER_SETUP SCS on SCS.CUSTOMER_CODE=SLSD.CUSTOMER_CODE
                                  WHERE SLSD.REFERENCE_NO IS NOT NULL AND SLSD.REFERENCE_FORM_CODE IS NOT NULL AND SLSD.DELETED_FLAG='N' AND SLSD.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND SLSD.DISPATCH_NO='{dispatchNo}'";
                var List = _dbContext.SqlQuery<LoadingSlipModalForPrint>(query).ToList();
                return List;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting loading slip for printing : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        #endregion
    }
}
