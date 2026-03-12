using NeoErp.LOC.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NeoErp.LOC.Services.Models;
using NeoErp.Data;
using NeoErp.Core;

namespace NeoErp.LOC.Services.Services
{
    public class LcLogisticPlanService : ILcLogisticPlanService
    {
        private IDbContext _dbContext;
        private IWorkContext _workcontext;
        public LcLogisticPlanService(IDbContext dbContext, IWorkContext workcontext)
        {
            this._dbContext = dbContext;
            this._workcontext = workcontext;

        }
        public string AddLogisticPlan(LcLogisticPlanModel Model)
        {
            var company_code = _workcontext.CurrentUserinformation.company_code;
            var branch_code = _workcontext.CurrentUserinformation.branch_code;
            var user = _workcontext.CurrentUserinformation.User_id;
            var LOGISTIC_PLAN_CODE = 0;

            var LOT_NO = 0;          
            try
            {
                #region LC_LOGISTIC_PLAN
                var maxLPCCodeQuery = $@"SELECT COALESCE(MAX(LOGISTIC_PLAN_CODE)+1,1) FROM LC_LOGISTIC_PLAN";
                LOGISTIC_PLAN_CODE = _dbContext.SqlQuery<int>(maxLPCCodeQuery).FirstOrDefault();

                var maxSNOQuery = $@"SELECT COALESCE(MAX(SNO)+1,1) FROM LC_LOGISTIC_PLAN WHERE LC_TRACK_NO='{Model.LC_LOGISTIC_PLAN.LC_TRACK_NO}' ";
                var SNO = _dbContext.SqlQuery<int>(maxSNOQuery).FirstOrDefault();

                var maxLOT_NOQuery = $@"SELECT COALESCE(MAX(LOT_NO)+1,1) FROM LC_LOGISTIC_PLAN WHERE LC_TRACK_NO='{Model.LC_LOGISTIC_PLAN.LC_TRACK_NO}' ";
                LOT_NO = _dbContext.SqlQuery<int>(maxLOT_NOQuery).FirstOrDefault();


                var INSERTLC_LOGISTIC_PLAN_QUERY = $@"INSERT INTO LC_LOGISTIC_PLAN(LOGISTIC_PLAN_CODE,LC_TRACK_NO,SNO,CONSIGNEE_NAME,CONSIGNEE_ADDRESS,NOTIFY_APPLICANT_NAME,NOTIFY_APPLICANT_ADDRESS,REMARKS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,APPROVED_BY,APPROVED_DATE,DELETED_FLAG,LOT_NO)
                                     VALUES('{LOGISTIC_PLAN_CODE}','{Model.LC_LOGISTIC_PLAN.LC_TRACK_NO}','{SNO}','{Model.LC_LOGISTIC_PLAN.CONSIGNEE_NAME}','{Model.LC_LOGISTIC_PLAN.CONSIGNEE_ADDRESS}','{Model.LC_LOGISTIC_PLAN.NOTIFY_APPLICANT_NAME}','{Model.LC_LOGISTIC_PLAN.NOTIFY_APPLICANT_ADDRESS}','{Model.LC_LOGISTIC_PLAN.REMARKS}','{company_code}','{branch_code}','{user}',TO_DATE('{DateTime.Now:MM/dd/yyyy/ HH:mm:ss}', 'mm/dd/yyyy hh24:mi:ss'),'{user}',TO_DATE('{DateTime.Now:MM/dd/yyyy/ HH:mm:ss}', 'mm/dd/yyyy hh24:mi:ss'),'N','{LOT_NO}')";
                _dbContext.ExecuteSqlCommand(INSERTLC_LOGISTIC_PLAN_QUERY);
                _dbContext.SaveChanges();
                #endregion

                #region LC_LOGISTIC_PLAN_CONTAINER  
                var CONATAINER_SNO = 0;
                foreach (var item in Model.LC_LOGISTIC_PLAN_CONTAINER)
                {
                    ++CONATAINER_SNO;
                    var PLAN_CONTAINER_CODE_QUERY = $@"SELECT COALESCE(MAX(PLAN_CONTAINER_CODE)+1,1) FROM LC_LOGISTIC_PLAN_CONTAINER";
                    var PLAN_CONTAINER_CODE = _dbContext.SqlQuery<int>(PLAN_CONTAINER_CODE_QUERY).FirstOrDefault();

                    if (item.AIR_PACK == null)
                    {
                        item.AIR_PACK = "0";
                    }
                    //if (item.LOAD_TYPE==null)
                    //{
                    //    item.LOAD_TYPE = "";
                    //}

                    var INSERT_LC_LOGISTIC_PLAN_CONTAINER_QUERY = $@"INSERT INTO LC_LOGISTIC_PLAN_CONTAINER(PLAN_CONTAINER_CODE,LOGISTIC_PLAN_CODE,SNO,LC_TRACK_NO,CONTAINER_CODE,LOAD_TYPE,FROM_LOCATION_CODE,TO_LOCATION_CODE,EST_BOOKING_DATE,EST_LOADING_DATE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,LOT_NO,ROUTE_NO,AIR_PACK,SHIPPING_TYPE,CARRIER_NUMBER)           
                        VALUES('{PLAN_CONTAINER_CODE}','{LOGISTIC_PLAN_CODE}','{CONATAINER_SNO}','{item.LC_TRACK_NO}','{item.CONTAINER_CODE}','{item.LOAD_TYPE}','{item.FROM_LOCATION_CODE}','{item.TO_LOCATION_CODE}',TO_DATE('{item.EST_BOOKING_DATE}', 'mm/dd/yyyy hh24:mi:ss'),TO_DATE('{item.EST_LOADING_DATE}', 'mm/dd/yyyy hh24:mi:ss'),'{company_code}','{branch_code}','{user}',TO_DATE('{DateTime.Now:MM/dd/yyyy/ HH:mm:ss}', 'mm/dd/yyyy hh24:mi:ss'),'N','{LOT_NO}','{item.ROUTE_NO}','{item.AIR_PACK}','{item.SHIPPING_TYPE}','{item.CARRIER_NUMBER}')";
                    _dbContext.ExecuteSqlCommand(INSERT_LC_LOGISTIC_PLAN_CONTAINER_QUERY);
                    _dbContext.SaveChanges();
                }

                #endregion

                #region LC_LOGISTIC_PLAN_ITEM      
                //foreach (var item in Model.LC_LOGISTIC_PLAN_ITEM)
                //{
                //    var LC_LOGISTIC_PLAN_ITEM_CODE_QUERY = $@"SELECT COALESCE(MAX(LC_LOGISTIC_PLAN_ITEM_CODE)+1,1) FROM LC_LOGISTIC_PLAN_ITEM";
                //    var LC_LOGISTIC_PLAN_ITEM_CODE = _dbContext.SqlQuery<int>(LC_LOGISTIC_PLAN_ITEM_CODE_QUERY).FirstOrDefault();



                //    var INSERT_LC_LOGISTIC_PLAN_ITEM_QUERY = $@"INSERT INTO LC_LOGISTIC_PLAN_ITEM(LC_LOGISTIC_PLAN_ITEM_CODE,LOGISTIC_PLAN_CODE,LC_TRACK_NO,ITEM_CODE,QUANTITY,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,APPROVED_BY,APPROVED_DATE,DELETED_FLAG)                             
                //                                                   VALUES({LC_LOGISTIC_PLAN_ITEM_CODE},{LOGISTIC_PLAN_CODE},{item.LC_TRACK_NO},{item.ITEM_CODE},{item.SHIPPMENT_QUANTITY},{company_code},{branch_code},{user},TO_DATE('{DateTime.Now:MM/dd/yyyy/ HH:mm:ss}', 'mm/dd/yyyy hh24:mi:ss'),{user},TO_DATE('{DateTime.Now:MM/dd/yyyy/ HH:mm:ss}', 'mm/dd/yyyy hh24:mi:ss'),'N')";
                //    _dbContext.ExecuteSqlCommand(INSERT_LC_LOGISTIC_PLAN_ITEM_QUERY);
                //    _dbContext.SaveChanges();
                //}
                #endregion

            }
            catch (Exception)
            {
                throw;

            }
            return "Inserted";
        }

        public List<ItemDetails> GetLCLogisticPlanItemsByLCNumber(string lcnumber)
        {
            var sqlquery = $@"SELECT LC_TRACK_NO FROM LC_LOC WHERE LC_NUMBER = '{lcnumber}' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
            int? lctracknumber = _dbContext.SqlQuery<int>(sqlquery).FirstOrDefault();

            var sqlquerys = $@"SELECT LC_TRACK_NO, ITEM_CODE, ITEM_EDESC,MU_CODE, TO_CHAR(SUM(INVOICE_QUANTITY))AS SHIPPMENT_QUANTITY, HS_CODE, CALC_QUANTITY, CALC_UNIT_PRICE, COUNTRY_CODE,COUNTRY_EDESC,REMARKS  FROM
            (SELECT  LI.LC_TRACK_NO, LI.HS_CODE, LI.ITEM_CODE, IMS.ITEM_EDESC, LI.MU_CODE, LI.QUANTITY AS CALC_QUANTITY, LI.AMOUNT AS CALC_UNIT_PRICE,
            LI.COUNTRY_OF_ORIGIN AS COUNTRY_CODE, LI.REMARKS, LP.CURRENCY_CODE, CS.COUNTRY_CODE || '-' || CS.COUNTRY_EDESC AS COUNTRY_EDESC, LIN.QUANTITY AS INVOICE_QUANTITY FROM LC_ITEM LI
            LEFT JOIN IP_ITEM_MASTER_SETUP IMS ON LI.ITEM_CODE = IMS.ITEM_CODE AND LI.COMPANY_CODE = IMS.COMPANY_CODE
            LEFT JOIN LC_PURCHASE_ORDER LP ON LI.LC_TRACK_NO = LP.LC_TRACK_NO
            LEFT JOIN COUNTRY_SETUP CS ON LI.COUNTRY_OF_ORIGIN = CS.COUNTRY_CODE AND LI.COMPANY_CODE = CS.COMPANY_CODE
            LEFT JOIN LC_INVOICE LIN ON LI.LC_TRACK_NO = LIN.LC_TRACK_NO  AND  LI.ITEM_CODE = LIN.ITEM_CODE
           WHERE LI.LC_TRACK_NO = '{lctracknumber}'  AND LI.COMPANY_CODE =  '{_workcontext.CurrentUserinformation.company_code}' ) TT
           GROUP BY ITEM_CODE, HS_CODE, LC_TRACK_NO, ITEM_EDESC, MU_CODE, CALC_QUANTITY, CALC_UNIT_PRICE, COUNTRY_CODE,COUNTRY_EDESC,REMARKS";


            var result = _dbContext.SqlQuery<ItemDetails>(sqlquerys).ToList();
            return result;


        }

        public List<ItemDetails> GetLCLogisticPlanItemsByLOT_NO(string LOT_NO, string lcnumber)
        {
            var sqlquery = $@"SELECT LC_TRACK_NO FROM LC_LOC WHERE LC_NUMBER = '{lcnumber}' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
            int? lctracknumber = _dbContext.SqlQuery<int>(sqlquery).FirstOrDefault();



            var SQL = $@"SELECT LIN.LC_TRACK_NO AS LC_TRACK_NO,TO_CHAR(LIN.ITEM_CODE) AS ITEM_CODE, TO_CHAR(IMS.ITEM_EDESC) AS ITEM_EDESC,TO_CHAR(LI.MU_CODE) AS MU_CODE,TO_CHAR(LIN.QUANTITY) AS SHIPPMENT_QUANTITY , LI.QUANTITY AS CALC_QUANTITY, LI.AMOUNT AS CALC_UNIT_PRICE,TO_CHAR(CS.COUNTRY_CODE) AS COUNTRY_CODE,
            CS.COUNTRY_CODE || '-' || CS.COUNTRY_EDESC AS COUNTRY_EDESC,TO_CHAR(LI.HS_CODE) AS HS_CODE  FROM LC_LOGISTIC_PLAN_ITEM LIN 
            LEFT JOIN  IP_ITEM_MASTER_SETUP IMS ON IMS.ITEM_CODE = LIN.ITEM_CODE
            LEFT JOIN LC_ITEM LI ON LI.LC_TRACK_NO = LIN.LC_TRACK_NO  AND LI.ITEM_CODE = LIN.ITEM_CODE
            LEFT JOIN LC_PURCHASE_ORDER LP ON LI.LC_TRACK_NO = LP.LC_TRACK_NO
            LEFT JOIN COUNTRY_SETUP CS ON LI.COUNTRY_OF_ORIGIN = CS.COUNTRY_CODE AND LI.COMPANY_CODE = CS.COMPANY_CODE
            WHERE LIN.LC_TRACK_NO = '{lctracknumber}' AND LIN.LOGISTIC_PLAN_CODE IN (SELECT DISTINCT LC.LOGISTIC_PLAN_CODE FROM LC_LOGISTIC_PLAN_CONTAINER LC WHERE LC.LOT_NO = '{LOT_NO}')";

            //var sqlquerys = $@"SELECT ITEM_CODE, ITEM_EDESC, SUM(INVOICE_QUANTITY)AS INVOICE_QUANTITY, HS_CODE, LC_TRACK_NO, MU_CODE,TO_CHAR(SHIPPMENT_QUANTITY) AS SHIPPMENT_QUANTITY, CALC_QUANTITY, CALC_UNIT_PRICE, COUNTRY_CODE,COUNTRY_EDESC,REMARKS  FROM
            //(SELECT  LCC.SNO, LI.LC_TRACK_NO, LI.HS_CODE, LI.ITEM_CODE, IMS.ITEM_EDESC, LI.MU_CODE,LIN.QUANTITY AS SHIPPMENT_QUANTITY,LI.QUANTITY AS CALC_QUANTITY, LI.AMOUNT AS CALC_UNIT_PRICE,
            //LI.COUNTRY_OF_ORIGIN AS COUNTRY_CODE, LI.REMARKS, LP.CURRENCY_CODE, CS.COUNTRY_CODE || '-' || CS.COUNTRY_EDESC AS COUNTRY_EDESC, LIN.QUANTITY AS INVOICE_QUANTITY FROM LC_ITEM LI
            //LEFT JOIN IP_ITEM_MASTER_SETUP IMS ON LI.ITEM_CODE = IMS.ITEM_CODE AND LI.COMPANY_CODE = IMS.COMPANY_CODE
            //LEFT JOIN LC_PURCHASE_ORDER LP ON LI.LC_TRACK_NO = LP.LC_TRACK_NO
            //LEFT JOIN COUNTRY_SETUP CS ON LI.COUNTRY_OF_ORIGIN = CS.COUNTRY_CODE AND LI.COMPANY_CODE = CS.COMPANY_CODE
            //LEFT JOIN LC_LOGISTIC_PLAN_CONTAINER LCC ON LI.LC_TRACK_NO = LCC.LC_TRACK_NO
            //LEFT JOIN LC_LOGISTIC_PLAN_ITEM LIN ON LI.LC_TRACK_NO = LIN.LC_TRACK_NO  AND  LI.ITEM_CODE = LIN.ITEM_CODE
            //WHERE LI.LC_TRACK_NO = '{lctracknumber}' AND LI.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}' AND LCC.LOT_NO ='{LOT_NO}' AND ROUTE_NO='1') TT
            //GROUP BY ITEM_CODE, HS_CODE, LC_TRACK_NO, ITEM_EDESC, MU_CODE, CALC_QUANTITY, CALC_UNIT_PRICE, COUNTRY_CODE,COUNTRY_EDESC,REMARKS,SHIPPMENT_QUANTITY";

            var result = _dbContext.SqlQuery<ItemDetails>(SQL).ToList();
            // var result = _dbContext.SqlQuery<ItemDetails>(sqlquerys).ToList();
            return result;


        }
        public LcLogisticPlanModel GetLogisticPlanList(string lcnumber)
        {
            LcLogisticPlanModel Record = new LcLogisticPlanModel();
            var sqlquery = $@"SELECT LC_TRACK_NO FROM LC_LOC WHERE LC_NUMBER = '{lcnumber}' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
            int? lctracknumber = _dbContext.SqlQuery<int>(sqlquery).FirstOrDefault();
            var LCQUERY = $@"SELECT TO_CHAR(IPS.SUPPLIER_EDESC) AS SUPPLIER_EDESC FROM IP_SUPPLIER_SETUP IPS
                           WHERE IPS.SUPPLIER_CODE ='{lcnumber}'";
            var LC_DESC = _dbContext.SqlQuery<string>(LCQUERY).FirstOrDefault();


            var sqlquerys = $@"SELECT TO_CHAR(LC_TRACK_NO)  AS LC_TRACK_NO,TO_CHAR(ROUTE_NO) AS ROUTE_NO ,TO_CHAR(SHIPPING_TYPE) AS SHIPPING_TYPE,TO_CHAR(TO_LOCATION_CODE) AS TO_LOCATION_CODE ,TO_CHAR(FROM_LOCATION_CODE) AS FROM_LOCATION_CODE
                            , TO_CHAR(EST_BOOKING_DATE ,'DD/MM/YYYY') AS EST_BOOKING_DATE, TO_CHAR(EST_LOADING_DATE ,'DD/MM/YYYY') AS EST_LOADING_DATE ,TO_CHAR(LOC.LOCATION_EDESC) AS TO_LOCATION_EDESC,
                            TO_CHAR(LCS.LOCATION_EDESC) AS FROM_LOCATION_EDESC
                            FROM
                            (SELECT C.LC_TRACK_NO  AS LC_TRACK_NO,C.ROUTE_NO AS ROUTE_NO ,C.SHIPPING_TYPE AS SHIPPING_TYPE ,
                             TO_CHAR(C.FROM_LOCATION_CODE) AS FROM_LOCATION_CODE,
                            TO_CHAR(C.TO_LOCATION_CODE) AS TO_LOCATION_CODE,
                            TRUNC(C.EST_BOOKING_DATE)  AS EST_BOOKING_DATE,
                            TRUNC(C.EST_LOADING_DATE)  AS EST_LOADING_DATE
                            FROM LC_LOGISTIC_PLAN_CONTAINER C
                             WHERE C.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}' AND C.LC_TRACK_NO ='{lctracknumber}' AND DELETED_FLAG='N'
                             AND C.LOT_NO NOT IN (SELECT DISTINCT LI.LOT_NO FROM LC_INVOICE  LI 
                             WHERE LI.LOT_NO NOT IN ('0') AND LC_TRACK_NO='{lctracknumber}' AND LC_NUMBER ='{LC_DESC}'
                             )
                            GROUP BY LC_TRACK_NO,ROUTE_NO,SHIPPING_TYPE,FROM_LOCATION_CODE,TO_LOCATION_CODE,EST_BOOKING_DATE,EST_LOADING_DATE)TT
                             LEFT JOIN LC_LOCATION_SETUP LOC ON LOC.LOCATION_CODE=TT.TO_LOCATION_CODE  
                             LEFT JOIN LC_LOCATION_SETUP LCS ON LCS.LOCATION_CODE=TT.FROM_LOCATION_CODE
                             ORDER BY ROUTE_NO";

            Record.LC_LOGISTIC_PLAN_CONTAINER = _dbContext.SqlQuery<LC_LOGISTIC_PLAN_CONTAINER>(sqlquerys).ToList();
            //foreach (var item in Record.LC_LOGISTIC_PLAN_CONTAINER)
            //{
            //    if (!LOT_NO_List.Contains(item.LOT_NO))
            //    {
            //        LOT_NO_List.Add(item.LOT_NO);
            //    }

            //}

            //foreach (var lot_record in LOT_NO_List)
            //{
            Record.ItemDetails = new List<ItemDetails>();
            var SQL = $@" SELECT LC_TRACK_NO, ITEM_CODE, ITEM_EDESC,MU_CODE, TO_CHAR(SUM(INVOICE_QUANTITY))AS SHIPPMENT_QUANTITY, HS_CODE, CALC_QUANTITY, CALC_UNIT_PRICE, COUNTRY_CODE,COUNTRY_EDESC,REMARKS  FROM
    (SELECT  LI.LC_TRACK_NO, LI.HS_CODE, LI.ITEM_CODE, IMS.ITEM_EDESC, LI.MU_CODE, LI.QUANTITY AS CALC_QUANTITY, LI.AMOUNT AS CALC_UNIT_PRICE,
    LI.COUNTRY_OF_ORIGIN AS COUNTRY_CODE, LI.REMARKS, LP.CURRENCY_CODE, CS.COUNTRY_CODE || '-' || CS.COUNTRY_EDESC AS COUNTRY_EDESC, LIN.QUANTITY AS INVOICE_QUANTITY FROM LC_ITEM LI
    LEFT JOIN IP_ITEM_MASTER_SETUP IMS ON LI.ITEM_CODE = IMS.ITEM_CODE AND LI.COMPANY_CODE = IMS.COMPANY_CODE
    LEFT JOIN LC_PURCHASE_ORDER LP ON LI.LC_TRACK_NO = LP.LC_TRACK_NO
    LEFT JOIN COUNTRY_SETUP CS ON LI.COUNTRY_OF_ORIGIN = CS.COUNTRY_CODE AND LI.COMPANY_CODE = CS.COMPANY_CODE
    LEFT JOIN LC_INVOICE LIN ON LI.LC_TRACK_NO = LIN.LC_TRACK_NO  AND  LI.ITEM_CODE = LIN.ITEM_CODE
   WHERE LI.LC_TRACK_NO = '{lctracknumber}'  AND LI.COMPANY_CODE =  '{_workcontext.CurrentUserinformation.company_code}' ) TT
   GROUP BY ITEM_CODE, HS_CODE, LC_TRACK_NO, ITEM_EDESC, MU_CODE, CALC_QUANTITY, CALC_UNIT_PRICE, COUNTRY_CODE,COUNTRY_EDESC,REMARKS";
            Record.ItemDetails = _dbContext.SqlQuery<ItemDetails>(SQL).ToList();
            //foreach (var item in ItemDetailRecord)
            //{
            //    Record.ItemDetails.Add(item);
            //}

            //}

            return Record;


        }

        public List<LogisticContainerPlan> GetLogisticPlanbyperformainvoice(string PinvoiceCode)
        {
            var sqlquery = $@"SELECT TO_CHAR(LC_NUMBER) AS LC_NUMBER,TO_CHAR(LOT_NO) AS LOT_NO,TO_CHAR(LC_TRACK_NO) AS LC_TRACK_NO FROM LC_INVOICE WHERE INVOICE_CODE = '{PinvoiceCode}' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
            var record = _dbContext.SqlQuery<Logisticplanforinvoice>(sqlquery).FirstOrDefault();

            //var sqlquerys = $@"select  DISTINCT C.LOGISTIC_PLAN_CODE AS LOGISTIC_PLAN_CODE, C.ROUTE_NO,L.LOT_NO AS LOT_NO ,C.SHIPPING_TYPE as SHIPPING_TYPE,TO_CHAR(C.FROM_LOCATION_CODE) AS FROM_LOCATION_CODE,LC.LOCATION_EDESC AS FROM_LOCATION_EDESC ,TO_CHAR(C.TO_LOCATION_CODE) AS TO_LOCATION_CODE,LOC.LOCATION_EDESC AS TO_LOCATION_EDESC , TO_CHAR(C.EST_BOOKING_DATE ,'DD/MM/YYYY') AS EST_BOOKING_DATE,TO_CHAR(C.EST_LOADING_DATE,'DD/MM/YYYY') AS EST_LOADING_DATE 
            //                from LC_LOGISTIC_PLAN_CONTAINER C INNER JOIN
            //                LC_LOGISTIC_PLAN L ON L.LOGISTIC_PLAN_CODE = C.LOGISTIC_PLAN_CODE
            //                LEFT JOIN LC_LOCATION_SETUP LOC ON LOC.LOCATION_CODE=C.TO_LOCATION_CODE  
            //                LEFT JOIN LC_LOCATION_SETUP LC ON LC.LOCATION_CODE=C.FROM_LOCATION_CODE  
            //                WHERE L.LC_TRACK_NO = C.LC_TRACK_NO AND L.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}' AND C.LC_TRACK_NO='{record.LC_TRACK_NO}' AND L.LOT_NO ='{record.LOT_NO}'
            //                ORDER BY C.ROUTE_NO";


            var sqlquerys = $@"SELECT DISTINCT TO_CHAR(C.LOGISTIC_PLAN_CODE) AS LOGISTIC_PLAN_CODE, C.ROUTE_NO,TO_CHAR(L.SNO) AS SNO ,C.SHIPPING_TYPE as SHIPPING_TYPE,TO_CHAR(C.FROM_LOCATION_CODE) AS FROM_LOCATION_CODE, LC.LOCATION_EDESC AS FROM_LOCATION_EDESC ,TO_CHAR(C.TO_LOCATION_CODE) AS TO_LOCATION_CODE, LOC.LOCATION_EDESC AS TO_LOCATION_EDESC , TO_CHAR(C.EST_BOOKING_DATE, 'DD/MM/YYYY') AS EST_BOOKING_DATE, TO_CHAR(C.EST_LOADING_DATE, 'DD/MM/YYYY') AS EST_LOADING_DATE
                            from LC_LOGISTIC_PLAN_CONTAINER C INNER JOIN
                            LC_LOGISTIC_PLAN L ON L.LOGISTIC_PLAN_CODE = C.LOGISTIC_PLAN_CODE
                            LEFT JOIN LC_LOCATION_SETUP LOC ON LOC.LOCATION_CODE = C.TO_LOCATION_CODE
                            LEFT JOIN LC_LOCATION_SETUP LC ON LC.LOCATION_CODE = C.FROM_LOCATION_CODE
                            WHERE C.LOGISTIC_PLAN_CODE IN  (
                            SELECT LOGISTIC_PLAN_CODE FROM LC_INVOICE_CONTAINER  LIC
                            WHERE L.LC_TRACK_NO = C.LC_TRACK_NO AND L.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}' AND C.LC_TRACK_NO='{record.LC_TRACK_NO}' AND LIC.LOT_NUMBER ='{record.LOT_NO}'  AND INVOICE_CODE='{PinvoiceCode}') ORDER BY L.SNO";

            var result = _dbContext.SqlQuery<LogisticContainerPlan>(sqlquerys).ToList();
            return result;


        }

        public List<LC_LOGISTIC_PLAN> getAllLcLogisticPlan()
        {
            var sqlquerys = $@"SELECT TO_CHAR(T.CARRIER_COUNT) AS CARRIER_COUNT ,T.LC_TRACK_NO, T.LOT_NO, T.LOGISTIC_PLAN_CODE,T.CONSIGNEE_NAME,T.CONSIGNEE_ADDRESS,T.NOTIFY_APPLICANT_NAME,T.NOTIFY_APPLICANT_ADDRESS,T.SUPPLIER_EDESC,T.LC_NUMBER,T.REMARKS,T.SNO,T. SHIPPING_TYPE,T.CREATED_DATE FROM (SELECT 
                                 TO_CHAR(LCLP.LC_TRACK_NO) AS LC_TRACK_NO
                                ,COUNT(LCLPC.PLAN_CONTAINER_CODE) AS CARRIER_COUNT
                                ,TO_CHAR(LCLP.LOT_NO) AS LOT_NO
                                ,TO_CHAR(LCLP.LOGISTIC_PLAN_CODE) AS LOGISTIC_PLAN_CODE
                                ,LCLP.CONSIGNEE_NAME
                                ,LCLP.CONSIGNEE_ADDRESS
                                ,LCLP.NOTIFY_APPLICANT_NAME
                                ,LCLP.NOTIFY_APPLICANT_ADDRESS
                                ,COALESCE(IPS.SUPPLIER_EDESC,LL.LC_NUMBER) AS SUPPLIER_EDESC
                                ,TO_CHAR(LL.LC_NUMBER) AS LC_NUMBER
                                ,LCLP.REMARKS
                                ,TO_CHAR(LCLP.SNO) AS SNO
                                ,TO_CHAR(LCLPC.SHIPPING_TYPE) AS SHIPPING_TYPE
                                ,TO_CHAR(LCLP.CREATED_DATE) AS CREATED_DATE
                                  FROM LC_LOGISTIC_PLAN LCLP
                           INNER JOIN LC_LOGISTIC_PLAN_CONTAINER LCLPC ON LCLPC.LC_TRACK_NO=LCLP.LC_TRACK_NO AND LCLPC.LOT_NO=LCLP.LOT_NO
                            INNER JOIN LC_LOC LL ON LL.LC_TRACK_NO = LCLP.LC_TRACK_NO
                            LEFT JOIN IP_SUPPLIER_SETUP IPS ON LL.LC_NUMBER = IPS.SUPPLIER_CODE                     
                            WHERE LCLP.LOT_NO IS NOT NULL
                            GROUP BY 
                                 LCLP.LC_TRACK_NO,
                                 LCLP.LOT_NO,
                                 LCLP.LOGISTIC_PLAN_CODE
                                ,LCLP.CONSIGNEE_NAME
                                ,LCLP.CONSIGNEE_ADDRESS
                                ,LCLP.NOTIFY_APPLICANT_NAME
                                ,LCLP.NOTIFY_APPLICANT_ADDRESS
                                ,IPS.SUPPLIER_EDESC
                                ,LL.LC_NUMBER
                                ,LCLP.REMARKS
                                ,LCLP.SNO
                                ,LCLPC.SHIPPING_TYPE
                                ,LCLP.CREATED_DATE)T         
                                 ORDER BY T.LC_TRACK_NO ,TO_NUMBER(T.LOT_NO) ASC,T.CREATED_DATE DESC";
            //OLD QUERY
            //var sqlquerys = $@"SELECT 
            //                     TO_CHAR(LCLP.LC_TRACK_NO) AS LC_TRACK_NO
            //                 ,TO_CHAR(LCLP.LOT_NO) AS LOT_NO
            //                 ,TO_CHAR(LCLP.LOGISTIC_PLAN_CODE) AS LOGISTIC_PLAN_CODE
            //                 ,LCLP.CONSIGNEE_NAME
            //                 ,LCLP.CONSIGNEE_ADDRESS
            //                 ,LCLP.NOTIFY_APPLICANT_NAME
            //                 ,LCLP.NOTIFY_APPLICANT_ADDRESS
            //                 ,IPS.SUPPLIER_EDESC
            //                 ,TO_CHAR(LL.LC_NUMBER) AS LC_NUMBER
            //                    ,LCLP.REMARKS
            //                FROM LC_LOGISTIC_PLAN LCLP
            //                INNER JOIN LC_LOC LL ON LL.LC_TRACK_NO = LCLP.LC_TRACK_NO
            //                LEFT JOIN IP_SUPPLIER_SETUP IPS ON LL.LC_NUMBER = IPS.SUPPLIER_CODE
            //                WHERE LCLP.LOT_NO IS NOT NULL
            //                ORDER BY LCLP.LC_TRACK_NO , LCLP.LOT_NO ASC,LCLP.CREATED_DATE DESC";
            var result = _dbContext.SqlQuery<LC_LOGISTIC_PLAN>(sqlquerys).ToList();
            return result;
        }
        //TOTAL_QUANTITY  
        public List<LC_LOGISTIC_PLAN> getAllLcLogisticPlanFilter(string lcnumber)
        {
            List<LC_LOGISTIC_PLAN> record = new List<LC_LOGISTIC_PLAN>();
            if (string.IsNullOrEmpty(lcnumber))
            {
                return record;
            }
            var sqlquerys = $@"SELECT 
                                 TO_CHAR(LCLP.LC_TRACK_NO) AS LC_TRACK_NO
	                            ,TO_CHAR(LCLP.LOT_NO) AS LOT_NO
	                            ,TO_CHAR(LCLP.LOGISTIC_PLAN_CODE) AS LOGISTIC_PLAN_CODE
	                            ,LCLP.CONSIGNEE_NAME
	                            ,LCLP.CONSIGNEE_ADDRESS
	                            ,LCLP.NOTIFY_APPLICANT_NAME
	                            ,LCLP.NOTIFY_APPLICANT_ADDRESS
	                            ,IPS.SUPPLIER_EDESC
	                            ,TO_CHAR(LL.LC_NUMBER) AS LC_NUMBER
                                ,LCLP.REMARKS
                            FROM LC_LOGISTIC_PLAN LCLP
                            INNER JOIN LC_LOC LL ON LL.LC_TRACK_NO = LCLP.LC_TRACK_NO
                            LEFT JOIN IP_SUPPLIER_SETUP IPS ON LL.LC_NUMBER = IPS.SUPPLIER_CODE
                            WHERE IPS.SUPPLIER_EDESC LIKE '%{lcnumber}%'
                            ORDER BY LCLP.LC_TRACK_NO , LCLP.LOT_NO ASC";
            var result = _dbContext.SqlQuery<LC_LOGISTIC_PLAN>(sqlquerys).ToList();
            return result;
        }

        public List<LC_LOGISTIC_PLANITEMLIST> getAllLcLogisticPlanItemListByTrackNumberAndLogisticPlanCode(string trackNumber, string logisticPlanCode)
        {
            var sqlquerys = $@"SELECT 
                                     TO_CHAR(LI.LC_LOGISTIC_PLAN_ITEM_CODE) AS LC_LOGISTIC_PLAN_ITEM_CODE
                                    ,TO_CHAR(LI.LOGISTIC_PLAN_CODE) AS LOGISTIC_PLAN_CODE
                                    ,TO_CHAR(LI.LC_TRACK_NO) AS LC_TRACK_NO
	                                ,TO_CHAR(IM.ITEM_CODE) AS ITEM_CODE
	                                ,IM.ITEM_EDESC AS ITEM_NAME
	                                ,IM.INDEX_MU_CODE AS MU_CODE
	                                ,TO_CHAR(LI.QUANTITY) AS QUANTITY
	                                ,LCI.HS_CODE
	                                ,CS.COUNTRY_CODE || '-' || CS.COUNTRY_EDESC AS COUNTRY_EDESC
                                    ,TO_CHAR(LCI.QUANTITY) AS TOTAL_QUANTITY
                                FROM LC_LOGISTIC_PLAN_ITEM LI
                                INNER JOIN IP_ITEM_MASTER_SETUP IM ON IM.ITEM_CODE = LI.ITEM_CODE
                                INNER JOIN LC_ITEM LCI ON IM.ITEM_CODE = LCI.ITEM_CODE
	                                AND LCI.LC_TRACK_NO = LI.LC_TRACK_NO
                                LEFT JOIN COUNTRY_SETUP CS ON LCI.COUNTRY_OF_ORIGIN = CS.COUNTRY_CODE
	                               AND LCI.COMPANY_CODE = CS.COMPANY_CODE
                                WHERE LI.LC_TRACK_NO = '{trackNumber}'
	                                AND LI.LOGISTIC_PLAN_CODE = '{logisticPlanCode}'";
            var result = _dbContext.SqlQuery<LC_LOGISTIC_PLANITEMLIST>(sqlquerys).ToList();
            return result;
        }

        public List<LC_LOGISTIC_PLANCONTAINERLIST> getAllLcLogisticPlanContainerListByTrackNumberAndLogisticPlanCode(string trackNumber, string lotNumber)
        {
            var sqlquerys = $@"SELECT 
                                     PC.SHIPPING_TYPE
	                                ,TO_CHAR(PC.EST_BOOKING_DATE) AS EST_BOOKING_DATE
	                                ,TO_CHAR(PC.EST_LOADING_DATE) AS EST_LOADING_DATE
                                    ,TO_CHAR(PC.ACT_BOOKING_DATE) AS ACT_BOOKING_DATE
	                                ,TO_CHAR(PC.ACT_LOADING_DATE) AS ACT_LOADING_DATE
	                                ,LC.CONTAINER_EDESC
	                                ,LS.LOCATION_EDESC AS FROM_LOCATION
	                                ,LS1.LOCATION_EDESC AS TO_LOCATION
                                FROM LC_LOGISTIC_PLAN_CONTAINER PC
                                INNER JOIN LC_LOCATION_SETUP LS ON LS.LOCATION_CODE = PC.FROM_LOCATION_CODE
                                INNER JOIN LC_LOCATION_SETUP LS1 ON LS1.LOCATION_CODE = PC.TO_LOCATION_CODE
                                INNER JOIN LC_CONTAINER LC ON LC.CONTAINER_CODE = PC.CONTAINER_CODE
                                WHERE PC.LC_TRACK_NO = '{trackNumber}'
	                                AND PC.LOT_NO = '{lotNumber}'
	                                AND PC.ROUTE_NO = '1'";
            var result = _dbContext.SqlQuery<LC_LOGISTIC_PLANCONTAINERLIST>(sqlquerys).ToList();
            return result;
        }




        public List<LC_LOGISTIC_PLAN_CONTAINER> GetUpdateShipmentData(string LOGISTIC_PLAN_CODE)
        {
            List<LC_LOGISTIC_PLAN_CONTAINER> Record = new List<LC_LOGISTIC_PLAN_CONTAINER>();
            var sqlquery = $@"select  TO_CHAR(PLAN_CONTAINER_CODE) AS PLAN_CONTAINER_CODE,TO_CHAR(C.LOGISTIC_PLAN_CODE) AS LOGISTIC_PLAN_CODE,TO_CHAR(C.SHIPPING_TYPE) AS SHIPPING_TYPE,TO_CHAR(C.EST_BOOKING_DATE,'MM/DD/yyyy') AS EST_BOOKING_DATE,TO_CHAR(C.EST_LOADING_DATE,'MM/DD/yyyy') AS EST_LOADING_DATE,TO_CHAR(C.ACT_BOOKING_DATE,'MM/DD/yyyy') as ACT_BOOKING_DATE,TO_CHAR(C.ACT_LOADING_DATE, 'MM/DD/yyyy') as ACT_LOADING_DATE,TO_CHAR(C.FROM_LOCATION_CODE) AS FROM_LOCATION_CODE,TO_CHAR(C.TO_LOCATION_CODE) AS TO_LOCATION_CODE,TO_CHAR(C.LC_TRACK_NO) AS LC_TRACK_NO,TO_CHAR(C.CONTAINER_CODE) AS CONTAINER_CODE,TO_CHAR(C.LOAD_TYPE) AS LOAD_TYPE,TO_CHAR(C.CARRIER_NUMBER) AS CARRIER_NUMBER FROM LC_LOGISTIC_PLAN_CONTAINER C WHERE C.LOGISTIC_PLAN_CODE='{LOGISTIC_PLAN_CODE}'";
            Record = _dbContext.SqlQuery<LC_LOGISTIC_PLAN_CONTAINER>(sqlquery).ToList();
            return Record;
        }

        public List<CommercialInvoiceModel> GetAllLcNumbers(string filter)
        {
            if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }

            var sqlquery = $@"select LL.LC_NUMBER,LL.LC_TRACK_NO,IPS.SUPPLIER_EDESC,LL.LC_NUMBER AS LC_NUMBER_CODE,LL.CREATED_DATE from LC_LOC LL
                                            LEFT JOIN IP_SUPPLIER_SETUP IPS ON LL.LC_NUMBER = IPS.SUPPLIER_CODE
                                            where UPPER(LL.LC_NUMBER) like '%{filter.ToUpperInvariant()}%' OR UPPER(IPS.SUPPLIER_EDESC) LIKE '%{filter.ToUpperInvariant()}%' AND LL.DELETED_FLAG = 'N'
                                                AND LL.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'ORDER BY LL.CREATED_DATE DESC";
            var result = _dbContext.SqlQuery<CommercialInvoiceModel>(sqlquery).ToList();
            foreach (var item in result)
            {
                if (item.SUPPLIER_EDESC != null)
                {
                    item.LC_NUMBER = item.SUPPLIER_EDESC;
                }
            }

            return result;
        }
        public List<CommercialInvoiceModel> GetAllLcNumbersfilter(string filter)
        {
            if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }

            var sqlquery = $@"select LL.LC_NUMBER,LL.LC_TRACK_NO,IPS.SUPPLIER_EDESC,LL.LC_NUMBER AS LC_NUMBER_CODE,LL.CREATED_DATE from LC_LOC LL
                                            LEFT JOIN IP_SUPPLIER_SETUP IPS ON LL.LC_NUMBER = IPS.SUPPLIER_CODE
                                            where UPPER(LL.LC_NUMBER) like '%{filter.ToUpperInvariant()}%' OR UPPER(IPS.SUPPLIER_EDESC) LIKE '%{filter.ToUpperInvariant()}%' AND LL.DELETED_FLAG = 'N'
                                               AND  LL.LC_TRACK_NO IN (select  DISTINCT I.LC_TRACK_NO from lc_logistic_plan_item I)
                                               AND LL.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'ORDER BY LL.CREATED_DATE DESC";
            var result = _dbContext.SqlQuery<CommercialInvoiceModel>(sqlquery).ToList();
            foreach (var item in result)
            {
                if (item.SUPPLIER_EDESC != null)
                {
                    item.LC_NUMBER = item.SUPPLIER_EDESC;
                }
            }

            return result;
        }
        public bool ABC()
        {
            return true;
        }
        //public void UpdateQuantity(LC_LOGISTIC_PLANITEMLIST itemdetail)
        //{
        //    if (itemdetail != null)
        //    {
        //        var query1= $@"SELECT SUM(LI1.QUANTITY) AS TOTALQUANTITYBYITEM FROM LC_LOGISTIC_PLAN_ITEM LI1 WHERE LI1.LC_TRACK_NO = '{itemdetail.LC_TRACK_NO}' AND LI1.ITEM_CODE='{itemdetail.ITEM_CODE}' AND LI1. LOGISTIC_PLAN_CODE='{itemdetail.LOGISTIC_PLAN_CODE}'";
        //        int previousItemQuantity= _dbContext.SqlQuery<int>(query1).FirstOrDefault();
        //        var query = $@"SELECT SUM(LI1.QUANTITY) AS TOTALQUANTITYBYITEM FROM LC_LOGISTIC_PLAN_ITEM LI1 WHERE LI1.LC_TRACK_NO = '{itemdetail.LC_TRACK_NO}' AND LI1.ITEM_CODE='{itemdetail.ITEM_CODE}'";
        //        int totalQuantityByItem = _dbContext.SqlQuery<int>(query).FirstOrDefault();
        //        int comparableQuantity = (totalQuantityByItem - (previousItemQuantity - int.Parse(itemdetail.QUANTITY)));

        //        if (comparableQuantity > int.Parse(itemdetail.TOTAL_QUANTITY))
        //        {
        //            return;
        //        }
        //        var updatetQuantity = $@"UPDATE LC_LOGISTIC_PLAN_ITEM SET QUANTITY='{itemdetail.QUANTITY}' ,COMPANY_CODE='{_workcontext.CurrentUserinformation
        //                                          .company_code}',BRANCH_CODE='{_workcontext.CurrentUserinformation.branch_code}',LAST_MODIFIED_BY='{_workcontext
        //                                          .CurrentUserinformation.User_id}',LAST_MODIFIED_DATE=TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy') WHERE LOGISTIC_PLAN_CODE = '{itemdetail.LOGISTIC_PLAN_CODE}' AND  ITEM_CODE = '{itemdetail.ITEM_CODE}' AND LC_TRACK_NO = '{itemdetail.LC_TRACK_NO}' ";
        //        var lcLogisticPlanItem = _dbContext.ExecuteSqlCommand(updatetQuantity);



        //    }
        //}

        public string UpdateQuantity(LC_LOGISTIC_PLANITEMLIST itemdetail)
        {
            if (itemdetail != null)
            {
                var query1 = $@"SELECT SUM(LI1.QUANTITY) AS TOTALQUANTITYBYITEM FROM LC_LOGISTIC_PLAN_ITEM LI1 WHERE LI1.LC_TRACK_NO = '{itemdetail.LC_TRACK_NO}' AND LI1.ITEM_CODE='{itemdetail.ITEM_CODE}' AND LI1. LOGISTIC_PLAN_CODE='{itemdetail.LOGISTIC_PLAN_CODE}'";
                int previousItemQuantity = _dbContext.SqlQuery<int>(query1).FirstOrDefault();
                var query = $@"SELECT SUM(LI1.QUANTITY) AS TOTALQUANTITYBYITEM FROM LC_LOGISTIC_PLAN_ITEM LI1 WHERE LI1.LC_TRACK_NO = '{itemdetail.LC_TRACK_NO}' AND LI1.ITEM_CODE='{itemdetail.ITEM_CODE}'";
                int totalQuantityByItem = _dbContext.SqlQuery<int>(query).FirstOrDefault();
                int comparableQuantity = (totalQuantityByItem - (previousItemQuantity - int.Parse(itemdetail.QUANTITY)));

                if (comparableQuantity > int.Parse(itemdetail.TOTAL_QUANTITY))
                {
                    return "Exceeded";
                }
                else
                {
                    var updatetQuantity = $@"UPDATE LC_LOGISTIC_PLAN_ITEM SET QUANTITY='{itemdetail.QUANTITY}' ,COMPANY_CODE='{_workcontext.CurrentUserinformation
                                                      .company_code}',BRANCH_CODE='{_workcontext.CurrentUserinformation.branch_code}',LAST_MODIFIED_BY='{_workcontext
                                                      .CurrentUserinformation.User_id}',LAST_MODIFIED_DATE=TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy') WHERE LOGISTIC_PLAN_CODE = '{itemdetail.LOGISTIC_PLAN_CODE}' AND  ITEM_CODE = '{itemdetail.ITEM_CODE}' AND LC_TRACK_NO = '{itemdetail.LC_TRACK_NO}' ";
                    var lcLogisticPlanItem = _dbContext.ExecuteSqlCommand(updatetQuantity);
                    return "Success";
                }


            }
            else { return "fail"; }
        }
        public void UpdateLogisticPlan(LC_LOGISTIC_PLANVVIEWMODEL LcLogisticPlanModel)
        {
            string updatequery = $@"UPDATE LC_LOGISTIC_PLAN LP SET LP.CONSIGNEE_NAME  = '{LcLogisticPlanModel.CONSIGNEE_NAME}',LP.CONSIGNEE_ADDRESS= '{LcLogisticPlanModel.CONSIGNEE_ADDRESS}', LP.NOTIFY_APPLICANT_NAME='{LcLogisticPlanModel.NOTIFY_APPLICANT_NAME}',LP.NOTIFY_APPLICANT_ADDRESS='{LcLogisticPlanModel.NOTIFY_APPLICANT_ADDRESS}',LP.REMARKS='{LcLogisticPlanModel.REMARKS}',LAST_MODIFIED_BY = '{_workcontext.CurrentUserinformation.User_id}',LAST_MODIFIED_DATE = TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss")}', 'mm/dd/yyyy hh24:mi:ss') WHERE LP.LOGISTIC_PLAN_CODE = '{LcLogisticPlanModel.LOGISTIC_PLAN_CODE}'";
            var rowCount = _dbContext.ExecuteSqlCommand(updatequery);
            _dbContext.SaveChanges();


            foreach (var record in LcLogisticPlanModel.LC_LOGISTIC_PLAN_CONTAINER)
            {
                string updatequery1 = $@"UPDATE LC_LOGISTIC_PLAN_CONTAINER LP
                                        SET 
                                             LP.SHIPPING_TYPE ='{record.SHIPPING_TYPE}',
                                             LP.EST_BOOKING_DATE=TO_DATE('{record.EST_BOOKING_DATE}','MM/dd/yyyy'),
                                             LP.EST_LOADING_DATE=TO_DATE('{record.EST_LOADING_DATE}','MM/dd/yyyy'),
                                             LP.ACT_BOOKING_DATE = TO_DATE('{record.ACT_BOOKING_DATE}','MM/dd/yyyy'),
	                                         LP.ACT_LOADING_DATE = TO_DATE('{record.ACT_LOADING_DATE}','MM/dd/yyyy'),
                                             LP.TO_LOCATION_CODE='{record.TO_LOCATION_CODE}',
                                             LP.FROM_LOCATION_CODE='{record.FROM_LOCATION_CODE}',
                                             LP.CONTAINER_CODE='{record.CONTAINER_CODE}',
                                             LP.LOAD_TYPE='{record.LOAD_TYPE}',
                                             LP.CARRIER_NUMBER='{record.CARRIER_NUMBER}',
	                                         COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}',
	                                         BRANCH_CODE = '{_workcontext.CurrentUserinformation.branch_code}',
	                                         LAST_MODIFIED_BY = '{_workcontext.CurrentUserinformation.User_id}',
	                                         LAST_MODIFIED_DATE = TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss")}', 'mm/dd/yyyy hh24:mi:ss')
                                             WHERE LP.LOGISTIC_PLAN_CODE = '{LcLogisticPlanModel.LOGISTIC_PLAN_CODE}' AND LP.PLAN_CONTAINER_CODE='{record.PLAN_CONTAINER_CODE}'";
                var rowCount1 = _dbContext.ExecuteSqlCommand(updatequery1);
                _dbContext.SaveChanges();
            }

            //string updatequery = $@"UPDATE LC_LOGISTIC_PLAN LP SET LP.CONSIGNEE_NAME  = '{LcLogisticPlanModel.CONSIGNEE_NAME}',LP.CONSIGNEE_ADDRESS= '{LcLogisticPlanModel.CONSIGNEE_ADDRESS}', LP.NOTIFY_APPLICANT_NAME='{LcLogisticPlanModel.NOTIFY_APPLICANT_NAME}',LP.NOTIFY_APPLICANT_ADDRESS='{LcLogisticPlanModel.NOTIFY_APPLICANT_ADDRESS}',LP.REMARKS='{LcLogisticPlanModel.REMARKS}',LAST_MODIFIED_BY = '{_workcontext.CurrentUserinformation.User_id}',LAST_MODIFIED_DATE = TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss")}', 'mm/dd/yyyy hh24:mi:ss') WHERE LP.LOGISTIC_PLAN_CODE = '{LcLogisticPlanModel.LOGISTIC_PLAN_CODE}' AND COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}' AND BRANCH_CODE ='{_workcontext.CurrentUserinformation.branch_code}'";
        }
        private class Logisticplanforinvoice
        {
            public string LC_NUMBER { get; set; }
            public string LOT_NO { get; set; }
            public string LC_TRACK_NO { get; set; }
        }
    }
}