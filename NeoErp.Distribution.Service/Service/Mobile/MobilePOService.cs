using NeoErp.Core.Domain;
using NeoErp.Core.Models;
using NeoErp.Distribution.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Service.Mobile
{
    public class MobilePOService : IMobilePOservice
    {
        private NeoErpCoreEntity _objectEntity;
        public MobilePOService(NeoErpCoreEntity objectEntity)
        {
            _objectEntity = objectEntity;
        }
        public List<SalesPersonPoModel> GetSalesPersonlst(string toDate,string fromDate, string salePerson,string orderNO = null,string resellerCode = null)
        {
            User userInfo = new User();
            userInfo.company_code = "06";
            userInfo.sp_codes = salePerson;
            ReportFiltersModel model = new ReportFiltersModel();
            model.ToDate = toDate;
            model.FromDate = fromDate;

            var companyCode = string.Join("','", model.CompanyFilter);
            companyCode = companyCode == "" ? userInfo.company_code : companyCode;

            var salesPersonFilter = string.Empty;
            var resCode = string.Empty;
            var orderNum = string.Empty;
            if (model.ItemBrandFilter.Count > 0)
                salesPersonFilter = $" AND ES.EMPLOYEE_CODE IN  ('{ string.Join("','", model.ItemBrandFilter).ToString()}')";
            else if (!string.IsNullOrWhiteSpace(userInfo.sp_codes))
            {
                salesPersonFilter = $" AND ES.EMPLOYEE_CODE IN ({userInfo.sp_codes})";
            }
            if (!string.IsNullOrWhiteSpace(orderNO))
            {
                orderNum = $" AND DPO1.ORDER_NO IN ({orderNO})";
            }
            if (!string.IsNullOrWhiteSpace(resellerCode))
            {
                resCode = $"AND DPO1.RESELLER_CODE IN ('{resellerCode}')";
            }
            var flagFilter = string.Empty;

            string query = $@"SELECT * FROM (
                                SELECT DPO1.ORDER_NO, DPO1.ORDER_DATE,BS_DATE(TO_CHAR(DPO1.ORDER_DATE)) MITI, DPO1.CUSTOMER_CODE, DPO1.BILLING_NAME CUSTOMER_EDESC, '' RESELLER_NAME, 'D' ORDER_ENTITY, TRIM(IMS.ITEM_EDESC) ITEM_EDESC, DPO1.ITEM_CODE,
                                        DPO1.MU_CODE, DPO1.QUANTITY, DPO1.UNIT_PRICE, DPO1.TOTAL_PRICE NET_TOTAL, IUS.MU_CODE CONVERSION_MU_CODE, IUS.CONVERSION_FACTOR,
                                         DPO1.PARTY_TYPE_CODE,
                                        (CASE WHEN DPO1.PARTY_TYPE_CODE IS NULL
                                          THEN FN_FETCH_DESC (DPO1.COMPANY_CODE,'IP_PARTY_TYPE_CODE',CS.PARTY_TYPE_CODE)
                                          ELSE FN_FETCH_DESC (DPO1.COMPANY_CODE,'IP_PARTY_TYPE_CODE',DPO1.PARTY_TYPE_CODE)
                                        END
                                        ) PARTY_TYPE_EDESC,
                                        DPO1.CREATED_BY, DPO1.CREATED_DATE, DPO1.DELETED_FLAG,
                                        --DPO1.COMPANY_CODE, DPO1.BRANCH_CODE,
                                        DPO1.REMARKS,
                                        DPO1.APPROVED_FLAG, DPO1.DISPATCH_FLAG, DPO1.ACKNOWLEDGE_FLAG, DPO1.REJECT_FLAG,
                                        ES.EMPLOYEE_EDESC,
                                        PS.PO_PARTY_TYPE,
                                        PS.PO_CONVERSION_UNIT,
                                        PS.PO_CONVERSION_FACTOR,
                                        PS.SO_CREDIT_LIMIT_CHK SO_CREDIT_LIMIT_FLAG,
                                        NVL(DPO2.TOTAL_QUANTITY,0) TOTAL_QUANTITY,
                                        DPO2.TOTAL_AMOUNT Grand_Total_Amount,
                                        NVL(DPO2.TOTAL_APPROVE_QTY,0) GRAND_APPROVE_QUENTITY,
                                        NVL(DPO2.TOTAL_APPROVE_AMT,0) TOTAL_APPROVE_AMT,'Distributor' EntityName,
                                        '' RESELLER_CODE
                                FROM DIST_IP_SSD_PURCHASE_ORDER DPO1
                                INNER JOIN IP_ITEM_MASTER_SETUP IMS ON IMS.ITEM_CODE = DPO1.ITEM_CODE AND IMS.COMPANY_CODE = DPO1.COMPANY_CODE AND IMS.CATEGORY_CODE = 'FG' AND IMS.GROUP_SKU_FLAG = 'I'
                                LEFT JOIN SA_CUSTOMER_SETUP CS ON CS.CUSTOMER_CODE = DPO1.CUSTOMER_CODE AND CS.COMPANY_CODE = DPO1.COMPANY_CODE
                                INNER JOIN DIST_LOGIN_USER LU ON LU.USERID = DPO1.CREATED_BY AND LU.ACTIVE = 'Y'
                                INNER JOIN HR_EMPLOYEE_SETUP ES ON ES.EMPLOYEE_CODE = LU.SP_CODE AND ES.COMPANY_CODE = LU.COMPANY_CODE
                                LEFT JOIN IP_ITEM_UNIT_SETUP IUS ON IUS.ITEM_CODE = DPO1.ITEM_CODE AND IUS.COMPANY_CODE = DPO1.COMPANY_CODE
                                INNER JOIN DIST_PREFERENCE_SETUP PS ON PS.COMPANY_CODE = DPO1.COMPANY_CODE
                                INNER JOIN (SELECT POT.ORDER_NO, SUM(POT.NET_QUANTITY) TOTAL_QUANTITY, SUM(POT.NET_PRICE) TOTAL_AMOUNT, SUM(POT.APPROVE_QTY) TOTAL_APPROVE_QTY, SUM(POT.APPROVE_AMT) TOTAL_APPROVE_AMT
                                FROM (SELECT A.ORDER_NO, A.ITEM_CODE, A.MU_CODE, A.QUANTITY, A.TOTAL_PRICE NET_PRICE, A.APPROVE_QTY, A.APPROVE_AMT, C.MU_CODE AS CONVERSION_UNIT, C.CONVERSION_FACTOR,
                                      (CASE
                                        WHEN (C.MU_CODE IS NULL AND C.CONVERSION_FACTOR IS NULL)
                                        THEN A.QUANTITY
                                        ELSE (CASE WHEN A.MU_CODE = C.MU_CODE THEN A.QUANTITY ELSE (A.QUANTITY * C.CONVERSION_FACTOR) END)
                                      END) NET_QUANTITY
                                      FROM DIST_IP_SSD_PURCHASE_ORDER A
                                      LEFT JOIN IP_ITEM_UNIT_SETUP C ON C.ITEM_CODE = A.ITEM_CODE AND C.COMPANY_CODE = A.COMPANY_CODE
                                ) POT
                                GROUP BY POT.ORDER_NO) DPO2 ON DPO2.ORDER_NO = DPO1.ORDER_NO
                                WHERE 1 = 1
                                     -- AND TRUNC(DPO1.ORDER_DATE) BETWEEN TO_DATE('{model.FromDate}','YYYY-MON-DD') AND TO_DATE('{model.ToDate}','YYYY-MON-DD')
                                        AND TRUNC(DPO1.ORDER_DATE) >= TO_DATE('{model.FromDate}','YYYY-MM-DD') AND TRUNC(DPO1.ORDER_DATE) <= TO_DATE('{model.ToDate}','YYYY-MM-DD')
                                      AND DPO1.DELETED_FLAG = 'N'
                                      {flagFilter} {salesPersonFilter}
                                      AND DPO1.COMPANY_CODE IN ('{companyCode}')
                                GROUP BY DPO1.ORDER_NO, DPO1.ORDER_DATE,BS_DATE(TO_CHAR(DPO1.ORDER_DATE)), DPO1.CUSTOMER_CODE, DPO1.BILLING_NAME, '', TRIM(IMS.ITEM_EDESC), DPO1.ITEM_CODE,
                                       DPO1.MU_CODE, DPO1.QUANTITY, DPO1.UNIT_PRICE, DPO1.TOTAL_PRICE, IUS.MU_CODE, IUS.CONVERSION_FACTOR, 
                                       'D', DPO1.PARTY_TYPE_CODE,
                                       (CASE WHEN DPO1.PARTY_TYPE_CODE IS NULL
                                          THEN FN_FETCH_DESC (DPO1.COMPANY_CODE,'IP_PARTY_TYPE_CODE',CS.PARTY_TYPE_CODE)
                                          ELSE FN_FETCH_DESC (DPO1.COMPANY_CODE,'IP_PARTY_TYPE_CODE',DPO1.PARTY_TYPE_CODE)
                                        END
                                       ),
                                       DPO1.CREATED_BY, DPO1.CREATED_DATE, DPO1.DELETED_FLAG,
                                       --DPO1.COMPANY_CODE, DPO1.BRANCH_CODE,
                                       DPO1.REMARKS,
                                       DPO1.APPROVED_FLAG, DPO1.DISPATCH_FLAG, DPO1.ACKNOWLEDGE_FLAG, DPO1.REJECT_FLAG,
                                       ES.EMPLOYEE_EDESC,
                                       PS.PO_PARTY_TYPE,
                                       PS.PO_CONVERSION_UNIT,
                                       PS.PO_CONVERSION_FACTOR,
                                       PS.SO_CREDIT_LIMIT_CHK,
                                       DPO2.TOTAL_QUANTITY,
                                       DPO2.TOTAL_AMOUNT,
                                       DPO2.TOTAL_APPROVE_QTY,
                                       DPO2.TOTAL_APPROVE_AMT
                            union all SELECT DPO1.ORDER_NO,DPO1.ORDER_DATE,BS_DATE(TO_CHAR(DPO1.ORDER_DATE)), DPO1.CUSTOMER_CODE,DPO1.BILLING_NAME CUSTOMER_EDESC, RM.RESELLER_NAME, 'R' ORDER_ENTITY,TRIM(IMS.ITEM_EDESC) ITEM_EDESC,DPO1.ITEM_CODE,
                                   DPO1.MU_CODE, DPO1.QUANTITY, DPO1.UNIT_PRICE, DPO1.TOTAL_PRICE NET_TOTAL, IUS.MU_CODE CONVERSION_MU_CODE, IUS.CONVERSION_FACTOR,
                                         DPO1.PARTY_TYPE_CODE,
                                        (CASE WHEN DPO1.PARTY_TYPE_CODE IS NULL
                                          THEN FN_FETCH_DESC (DPO1.COMPANY_CODE,'IP_PARTY_TYPE_CODE',CS.PARTY_TYPE_CODE)
                                          ELSE FN_FETCH_DESC (DPO1.COMPANY_CODE,'IP_PARTY_TYPE_CODE',DPO1.PARTY_TYPE_CODE)
                                        END
                                        ) PARTY_TYPE_EDESC,
                                        DPO1.CREATED_BY, DPO1.CREATED_DATE, DPO1.DELETED_FLAG,
                                        --DPO1.COMPANY_CODE, DPO1.BRANCH_CODE,
                                        DPO1.REMARKS,
                                        DPO1.APPROVED_FLAG, DPO1.DISPATCH_FLAG, DPO1.ACKNOWLEDGE_FLAG, DPO1.REJECT_FLAG,
                                        ES.EMPLOYEE_EDESC,
                                        PS.PO_PARTY_TYPE,
                                        PS.PO_CONVERSION_UNIT,
                                        PS.PO_CONVERSION_FACTOR,
                                        PS.SO_CREDIT_LIMIT_CHK SO_CREDIT_LIMIT_FLAG,
                                        NVL(DPO2.TOTAL_QUANTITY,0) TOTAL_QUANTITY,
                                        DPO2.TOTAL_AMOUNT Grand_Total_Amount,
                                        NVL(DPO2.TOTAL_APPROVE_QTY,0) GRAND_APPROVE_QUENTITY,
                                        NVL(DPO2.TOTAL_APPROVE_AMT,0) TOTAL_APPROVE_AMT,'Reseller' EntityName,
                                        DPO1.RESELLER_CODE
                            FROM DIST_IP_SSR_PURCHASE_ORDER DPO1
                            INNER JOIN DIST_RESELLER_MASTER RM ON RM.RESELLER_CODE = DPO1.RESELLER_CODE AND RM.IS_CLOSED = 'N'
                            INNER JOIN IP_ITEM_MASTER_SETUP IMS ON IMS.ITEM_CODE = DPO1.ITEM_CODE AND IMS.COMPANY_CODE = DPO1.COMPANY_CODE AND IMS.CATEGORY_CODE = 'FG' AND IMS.GROUP_SKU_FLAG = 'I'
                            LEFT JOIN SA_CUSTOMER_SETUP CS ON CS.CUSTOMER_CODE = DPO1.CUSTOMER_CODE AND CS.COMPANY_CODE = DPO1.COMPANY_CODE
                            INNER JOIN DIST_LOGIN_USER LU ON LU.USERID = DPO1.CREATED_BY AND LU.ACTIVE = 'Y'
                            INNER JOIN HR_EMPLOYEE_SETUP ES ON ES.EMPLOYEE_CODE = LU.SP_CODE AND ES.COMPANY_CODE = LU.COMPANY_CODE
                            LEFT JOIN IP_ITEM_UNIT_SETUP IUS ON IUS.ITEM_CODE = DPO1.ITEM_CODE AND IUS.COMPANY_CODE = DPO1.COMPANY_CODE
                            INNER JOIN DIST_PREFERENCE_SETUP PS ON PS.COMPANY_CODE = DPO1.COMPANY_CODE
                            --LEFT JOIN (SELECT V.SUB_CODE, NVL((SUM (V.DR_AMOUNT) - SUM (V.CR_AMOUNT)),0) BALANCE
                            --  FROM V$VIRTUAL_SUB_LEDGER V
                            --  WHERE 1 = 1
                            --  AND V.COMPANY_CODE IN ('01')
                            --  AND V.SUB_LEDGER_FLAG = 'C'
                            -- GROUP BY V.SUB_CODE) VSL ON TRIM(VSL.SUB_CODE) = TRIM(CS.LINK_SUB_CODE)
                            INNER JOIN (SELECT POT.ORDER_NO, SUM(POT.NET_QUANTITY) TOTAL_QUANTITY, SUM(POT.NET_PRICE) TOTAL_AMOUNT, SUM(POT.APPROVE_QTY) TOTAL_APPROVE_QTY, SUM(POT.APPROVE_AMT) TOTAL_APPROVE_AMT
                                        FROM (SELECT A.ORDER_NO, A.ITEM_CODE, A.MU_CODE, A.QUANTITY, A.TOTAL_PRICE NET_PRICE, A.APPROVE_QTY, A.APPROVE_AMT, C.MU_CODE AS CONVERSION_UNIT, C.CONVERSION_FACTOR,
                                        (CASE
                                          WHEN (C.MU_CODE IS NULL AND C.CONVERSION_FACTOR IS NULL)
                                          THEN A.QUANTITY
                                          ELSE (CASE WHEN A.MU_CODE = C.MU_CODE THEN A.QUANTITY ELSE (A.QUANTITY * C.CONVERSION_FACTOR) END)
                                        END) NET_QUANTITY
                                        FROM DIST_IP_SSR_PURCHASE_ORDER A
                                        LEFT JOIN IP_ITEM_UNIT_SETUP C ON C.ITEM_CODE = A.ITEM_CODE AND C.COMPANY_CODE = A.COMPANY_CODE
                                        WHERE 1=1 
                                        --ORDER BY A.ORDER_NO DESC, A.ITEM_CODE
                            ) POT
                           GROUP BY POT.ORDER_NO) DPO2 ON DPO2.ORDER_NO = DPO1.ORDER_NO
                            WHERE 1 = 1
                              AND TRUNC(DPO1.ORDER_DATE) >= TO_DATE('{model.FromDate}','YYYY-MM-DD') AND TRUNC(DPO1.ORDER_DATE) <= TO_DATE('{model.ToDate}','YYYY-MM-DD')
                              AND DPO1.DELETED_FLAG = 'N'                                
                              AND DPO1.COMPANY_CODE IN ('{companyCode}')  {salesPersonFilter} {orderNum} {resCode}
                            ) ORDER BY EMPLOYEE_EDESC, ITEM_EDESC, ORDER_NO";
            var result = _objectEntity.SqlQuery<SalesPersonPoModel>(query).ToList();
            return result;
        }

        public void ApprovePurchaseOrder(string orderNo, string itemCode,out string message,out bool status)
        {
            try
            {

                if (!string.IsNullOrEmpty(orderNo) && !string.IsNullOrEmpty(itemCode))
                {
                    string query = string.Empty;
                    query = $@"SELECT * FROM DIST_IP_SSD_PURCHASE_ORDER WHERE ORDER_NO ={orderNo} AND ITEM_CODE = {itemCode}";
                    var data = _objectEntity.SqlQuery<ResellerRegistration>(query).ToList();
                    if (data.Count > 0)
                    {
                        query = $@"UPDATE DIST_IP_SSD_PURCHASE_ORDER SET APPROVED_FLAG = 'Y' WHERE ORDER_NO ={orderNo} AND ITEM_CODE = {itemCode}";
                        var result = _objectEntity.ExecuteSqlCommand(query);
                        if (result > 0)
                        {
                            message = "PO Approved.";
                            status = true;
                        }
                        else
                        {
                            message = "PO Not Approved.";
                            status = false;
                        }
                    }
                    else
                    {
                        message = "PO Not Found.";
                        status = false;
                    }
                }
                else
                {
                    message = "Order Number or Item Code Empty.";
                    status = false;
                }
            }
            catch(Exception ex)
            {
                message = ex.Message;
                status = false;
            }
        }
        public void RejectPurchaseOrder(string orderNo, string itemCode, out string message, out bool status)
        {
            try
            {
                if (!string.IsNullOrEmpty(orderNo) && !string.IsNullOrEmpty(itemCode))
                {
                    string query = string.Empty;
                    query = $@"SELECT * FROM DIST_IP_SSD_PURCHASE_ORDER WHERE ORDER_NO ={orderNo} AND ITEM_CODE = {itemCode}";
                    var data = _objectEntity.SqlQuery<ResellerRegistration>(query).ToList();
                    if (data.Count > 0)
                    {
                        query = $@"UPDATE DIST_IP_SSD_PURCHASE_ORDER SET REJECT_FLAG = 'Y' WHERE ORDER_NO ={orderNo} AND ITEM_CODE = {itemCode}";
                        var result = _objectEntity.ExecuteSqlCommand(query);
                        if (result > 0)
                        {
                            message = "PO Rejected.";
                            status = true;
                        }
                        else
                        {
                            message = "PO Not Rejected.";
                            status = false;
                        }
                    }
                    else
                    {
                        message = "PO Not Found.";
                        status = false;
                    }
                }
                else
                {
                    message = "Order Number or Item Code Empty.";
                    status = false;
                }
            }
            catch(Exception ex)
            {
                message = ex.Message;
                status = false;
            }
        }
    }
}
