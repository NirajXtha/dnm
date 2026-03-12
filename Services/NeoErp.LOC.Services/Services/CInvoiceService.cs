using System;
using NeoErp.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using NeoErp.Core;
using NeoErp.LOC.Services.Models;

namespace NeoErp.LOC.Services.Services
{
    public class CInvoiceService : ICInvoiceService
    {
        private IDbContext _dbContext;
        private IWorkContext _workcontext;
        public CInvoiceService(IDbContext dbContext, IWorkContext iWorkContext)
        {
            this._workcontext = iWorkContext;
            this._dbContext = dbContext;
        }


        public List<CommercialInvoiceModel> LoadCIBylcnumber(string lcnumber)
        {
            var query = $@"select DISTINCT LI.INVOICE_NUMBER,LI.INVOICE_DATE AS INVOICE_DATE , LI.PP_DATE AS PP_DATE, LI.PP_NO AS PP_NO, LI.AWB_NUMBER AS AWB_NUMBER, LI.AWB_DATE AS AWB_DATE, LI.LOT_NO AS LOT_NO from LC_INVOICE  LI 
                           WHERE LC_NUMBER='CCBLLINR0267374 watch' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
            var Record = _dbContext.SqlQuery<CommercialInvoiceModel>(query).ToList();
            return Record;
        }


        public CommercialInvoiceModel CreateCommercialInvoice(MultiCommercialInvoiceModel multiCommercialInvoiceModel)
        {
            CommercialInvoiceModel cimodel = new CommercialInvoiceModel();
            var SNO = 0;
            var LC_TRACK_NO = multiCommercialInvoiceModel.LC_TRACK_NO;
            var LC_NUMBER = multiCommercialInvoiceModel.LC_NUMBER;
            var IS_AIR = 'N';
            if (multiCommercialInvoiceModel.IS_AIR)
            {
                IS_AIR = 'Y';
            }

            if (multiCommercialInvoiceModel.CommercialInvoiceData[0].INVOICE_CODE == 0)
            {
                int tempinvoicecode = 0;


                var invoice = $@"SELECT NVL(MAX(INVOICE_CODE +1),1) FROM LC_INVOICE";
                var maxinvoicecode = _dbContext.SqlQuery<int>(invoice).FirstOrDefault();


                var lotquery = $@"SELECT NVL(MAX(LOT_NO +1),1) FROM LC_INVOICE WHERE LC_TRACK_NO = '{LC_TRACK_NO}' AND INVOICE_NUMBER = '{multiCommercialInvoiceModel.CommercialInvoiceData[0].INVOICE_NUMBER}'";
                var LOT_NO = _dbContext.SqlQuery<int>(lotquery).FirstOrDefault();
               
               
                foreach (var CommercialInvoiceData_record in multiCommercialInvoiceModel.CommercialInvoiceData)
                {
                    foreach (var items in multiCommercialInvoiceModel.Itemlist)
                    {

                        ++SNO;

                       


                        var query = $@"SELECT INVOICE_NUMBER,QUANTITY,EXCHANGE_RATE,SALES_EXG_RATE FROM LC_INVOICE WHERE ITEM_CODE = '{items.ITEM_CODE}' AND LC_TRACK_NO = '{LC_TRACK_NO}' AND INVOICE_NUMBER = '{CommercialInvoiceData_record.INVOICE_NUMBER}' AND SNO = '{SNO}' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
                        Items invoicecode = _dbContext.SqlQuery<Items>(query).FirstOrDefault();
                        if (invoicecode != null)
                        {
                            //var dbquantity = invoicecode.QUANTITY;
                            //var totalquantity = items.INPUT_QUANTITY + invoicecode.QUANTITY;
                            var updatetQuantity = $@"UPDATE LC_INVOICE SET QUANTITY='{items.INPUT_QUANTITY}' ,COMPANY_CODE='{_workcontext.CurrentUserinformation
                                     .company_code}',BRANCH_CODE='{_workcontext.CurrentUserinformation.branch_code}',LAST_MODIFIED_BY='{_workcontext
                                     .CurrentUserinformation.User_id}',LAST_MODIFIED_DATE=TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),IS_AIR='{IS_AIR}',EXCHANGE_RATE='{items.EXCHANGE_RATE}',SALES_EXG_RATE='{items.SALES_EXG_RATE}' WHERE  INVOICE_CODE = '{tempinvoicecode}' AND ITEM_CODE = '{items.ITEM_CODE}' AND SNO = '{SNO}' AND LC_TRACK_NO = '{LC_TRACK_NO}'";
                            var updatepinvoice = _dbContext.ExecuteSqlCommand(updatetQuantity);
                        }
                        else
                        {

                            var insertinvoice = $@"INSERT INTO LC_INVOICE(INVOICE_CODE,SNO,INVOICE_DATE,INVOICE_NUMBER,LC_NUMBER,LC_TRACK_NO,INVOICE_CURRENCY,EXCHANGE_RATE,AWB_NUMBER,AWB_DATE,ITEM_CODE,PP_DATE,PP_NO,AMOUNT,QUANTITY,MU_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,LOT_NO,IS_AIR,SALES_EXG_RATE)VALUES('{maxinvoicecode}','{SNO}', TO_DATE('{CommercialInvoiceData_record.INVOICE_DATE}','MM/dd/yyyy HH:MI:SS AM'), '{CommercialInvoiceData_record.INVOICE_NUMBER}','{LC_NUMBER}','{LC_TRACK_NO}','{CommercialInvoiceData_record.CURRENCY_CODE}', '{CommercialInvoiceData_record.EXCHANGE_RATE}','{CommercialInvoiceData_record.AWB_NUMBER}',TO_DATE('{CommercialInvoiceData_record.AWB_DATE}','MM/dd/yyyy HH:MI:SS AM'),'{items.ITEM_CODE}',TO_DATE('{CommercialInvoiceData_record.PP_DATE}','MM/dd/yyyy HH:MI:SS AM'),'{CommercialInvoiceData_record.PP_NO}','{items.AMOUNT}','{items.SHIPPMENT_QUANTITY}','{items.MU_CODE}','{_workcontext.CurrentUserinformation.company_code}','{_workcontext.CurrentUserinformation.branch_code}',
                                                    '{_workcontext.CurrentUserinformation.User_id}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'{'N'}','{LOT_NO}','{IS_AIR}','{items.SALES_EXG_RATE}')";
                            var invoiceinsert = _dbContext.ExecuteSqlCommand(insertinvoice);
                        }





                    }


                }
                #region insert LC_INVOICE_CONTAINER
                var SNO1 = 0;
                foreach (var record in multiCommercialInvoiceModel.ContainerList)
                {
                    ++SNO1;
                    //insert query new table
                    var maxInvoiceContainer_QUERY = $@"SELECT NVL(MAX(INVOICE_CONTAINER +1),1) FROM LC_INVOICE_CONTAINER";

                    var maxInvoiceContainer = _dbContext.SqlQuery<int>(maxInvoiceContainer_QUERY).FirstOrDefault();


                    var insertInvoiceContainerQUERY = $@"INSERT INTO LC_INVOICE_CONTAINER( INVOICE_CONTAINER,INVOICE_CODE,LOGISTIC_PLAN_CODE,PLAN_CONTAINER_CODE,CARRIER_CODE,CARRIER_NUMBER,SNO,LOAD_TYPE,CARRIER_NAME,LC_TRACK_NO,LC_NUMBER,SHIPMENT_TYPE,LOT_NUMBER,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG)
                                     VALUES( '{maxInvoiceContainer}','{maxinvoicecode}','{record.LOGISTIC_PLAN_CODE}','{record.PLAN_CONTAINER_CODE}','{record.CONTAINER_CODE}', '{record.CARRIER_NUMBER}', {SNO1},'{record.LOAD_TYPE}', '{record.CONTAINER_EDESC}', '{LC_TRACK_NO}',
                                     '{LC_NUMBER}',
                                     '{record.SHIPPING_TYPE}',
                                     '{LOT_NO}',
                                     '{_workcontext.CurrentUserinformation.company_code}',
                                     '{_workcontext.CurrentUserinformation.branch_code}',
                                     '{_workcontext.CurrentUserinformation.User_id}',
                                     TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss")}', 'mm/dd/yyyy hh24:mi:ss'),
                                     'N'
                                     ) ";
                   _dbContext.ExecuteSqlCommand(insertInvoiceContainerQUERY);

                }
                #endregion
                cimodel.INVOICE_CODE = maxinvoicecode;
                cimodel.LC_TRACK_NO = LC_TRACK_NO;

            }
            else
            {

                var updatetPI = $@"UPDATE LC_INVOICE SET INVOICE_DATE=TO_DATE('{multiCommercialInvoiceModel.CommercialInvoiceData[0].INVOICE_DATE}','MM/dd/yyyy HH:MI:SS AM'),INVOICE_NUMBER='{multiCommercialInvoiceModel.CommercialInvoiceData[0].INVOICE_NUMBER}',INVOICE_CURRENCY='{multiCommercialInvoiceModel.CommercialInvoiceData[0].CURRENCY_CODE}',EXCHANGE_RATE='{multiCommercialInvoiceModel.CommercialInvoiceData[0].EXCHANGE_RATE}', SALES_EXG_RATE='{multiCommercialInvoiceModel.CommercialInvoiceData[0].SALES_EXG_RATE}',
                           PP_NO='{multiCommercialInvoiceModel.CommercialInvoiceData[0].PP_NO}',AWB_NUMBER='{multiCommercialInvoiceModel.CommercialInvoiceData[0].AWB_NUMBER}',AWB_DATE=TO_DATE('{multiCommercialInvoiceModel.CommercialInvoiceData[0].AWB_DATE}','MM/dd/yyyy HH:MI:SS AM'),PP_DATE = TO_DATE('{multiCommercialInvoiceModel.CommercialInvoiceData[0].PP_DATE}','MM/dd/yyyy HH:MI:SS AM'),COMPANY_CODE='{_workcontext.CurrentUserinformation
                                  .company_code}',BRANCH_CODE='{_workcontext.CurrentUserinformation.branch_code}',LAST_MODIFIED_BY='{_workcontext
                                  .CurrentUserinformation.User_id}',LAST_MODIFIED_DATE=TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),IS_AIR='{IS_AIR}' WHERE  INVOICE_CODE IN '{multiCommercialInvoiceModel.CommercialInvoiceData[0].INVOICE_CODE}'";
                var updatepinvoice = _dbContext.ExecuteSqlCommand(updatetPI);
                CommercialInvoiceModel returnlist = new CommercialInvoiceModel();
                returnlist.LC_TRACK_NO = multiCommercialInvoiceModel.LC_TRACK_NO;
                returnlist.INVOICE_CODE = multiCommercialInvoiceModel.CommercialInvoiceData[0].INVOICE_CODE;
                return returnlist;
            }
            return cimodel;
        }

        //public CommercialInvoiceModel CreateCommercialInvoice(CommercialInvoiceModel cdetails)
        //{
        //    if (cdetails.INVOICE_CODE == 0)
        //    {
        //        int tempinvoicecode = 0;

        //        //if (cdetails.TEMP_INVOICE_CODE == 0)
        //        //{
        //        //    var query = $@"SELECT NVL(MAX(SNO +1),1) FROM LC_INVOICE";
        //        //    var invoicecode = _dbContext.SqlQuery<int>(query).FirstOrDefault();
        //        //    tempinvoicecode = invoicecode;

        //        //}
        //        //else
        //        //{
        //        //    tempinvoicecode = cdetails.TEMP_INVOICE_CODE;
        //        //}


        //        var invoice = $@"SELECT NVL(MAX(INVOICE_CODE +1),1) FROM LC_INVOICE";
        //        var maxinvoicecode = _dbContext.SqlQuery<int>(invoice).FirstOrDefault();
        //        foreach (var items in cdetails.Itemlist)
        //        {
        //            var query = $@"SELECT INVOICE_NUMBER,QUANTITY FROM LC_INVOICE WHERE ITEM_CODE = '{items.ITEM_CODE}' AND LC_TRACK_NO = '{cdetails.LC_TRACK_NO}' AND INVOICE_NUMBER = '{cdetails.INVOICE_NUMBER}' AND SNO = '{items.SNO}' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
        //            Items invoicecode = _dbContext.SqlQuery<Items>(query).FirstOrDefault();
        //            if (invoicecode != null)
        //            {
        //                //var dbquantity = invoicecode.QUANTITY;
        //                //var totalquantity = items.INPUT_QUANTITY + invoicecode.QUANTITY;
        //                var updatetQuantity = $@"UPDATE LC_INVOICE SET QUANTITY='{items.INPUT_QUANTITY}' ,COMPANY_CODE='{_workcontext.CurrentUserinformation
        //                         .company_code}',BRANCH_CODE='{_workcontext.CurrentUserinformation.branch_code}',LAST_MODIFIED_BY='{_workcontext
        //                         .CurrentUserinformation.User_id}',LAST_MODIFIED_DATE=TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy') WHERE  INVOICE_CODE = '{tempinvoicecode}' AND ITEM_CODE = '{items.ITEM_CODE}' AND SNO = '{items.SNO}' AND LC_TRACK_NO = '{cdetails.LC_TRACK_NO}'";
        //                var updatepinvoice = _dbContext.ExecuteSqlCommand(updatetQuantity);
        //            }
        //            else
        //            {

        //                var insertinvoice = $@"INSERT INTO LC_INVOICE(INVOICE_CODE,SNO,INVOICE_DATE,INVOICE_NUMBER,LC_NUMBER,LC_TRACK_NO,INVOICE_CURRENCY,EXCHANGE_RATE,AWB_NUMBER,AWB_DATE,ITEM_CODE,PP_DATE,PP_NO,AMOUNT,QUANTITY,MU_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG)VALUES('{maxinvoicecode}','{items.SNO}', TO_DATE('{cdetails.INVOICE_DATE}','MM/dd/yyyy HH:MI:SS AM'), '{cdetails.INVOICE_NUMBER}','{cdetails.LC_NUMBER}','{cdetails.LC_TRACK_NO}','{cdetails.CURRENCY_CODE}', '{cdetails.EXCHANGE_RATE}','{cdetails.AWB_NUMBER}',TO_DATE('{cdetails.AWB_DATE}','MM/dd/yyyy HH:MI:SS AM'),'{items.ITEM_CODE}',TO_DATE('{cdetails.PP_DATE}','MM/dd/yyyy HH:MI:SS AM'),'{cdetails.PP_NO}','{items.AMOUNT}','{items.SHIPPMENT_QUANTITY}','{items.MU_CODE}','{_workcontext.CurrentUserinformation.company_code}','{_workcontext.CurrentUserinformation.branch_code}','{_workcontext
        //                                       .CurrentUserinformation.User_id}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'{'N'}')";
        //                var invoiceinsert = _dbContext.ExecuteSqlCommand(insertinvoice);
        //            }

        //        }

        //        CommercialInvoiceModel cimodel = new CommercialInvoiceModel();
        //        cimodel.INVOICE_CODE = maxinvoicecode;
        //        cimodel.LC_TRACK_NO = cdetails.LC_TRACK_NO;
        //        return cimodel;
        //    }
        //    else
        //    {
        //        var updatetPI = $@"UPDATE LC_INVOICE SET INVOICE_DATE=TO_DATE('{cdetails.INVOICE_DATE}','MM/dd/yyyy HH:MI:SS AM'),INVOICE_NUMBER='{cdetails.INVOICE_NUMBER}',INVOICE_CURRENCY='{cdetails.CURRENCY_CODE}',EXCHANGE_RATE='{cdetails.EXCHANGE_RATE}', 
        //                   PP_NO='{cdetails.PP_NO}',AWB_NUMBER='{cdetails.AWB_NUMBER}',PP_DATE = TO_DATE('{cdetails.AWB_DATE}','MM/dd/yyyy HH:MI:SS AM'),COMPANY_CODE='{_workcontext.CurrentUserinformation
        //                          .company_code}',BRANCH_CODE='{_workcontext.CurrentUserinformation.branch_code}',LAST_MODIFIED_BY='{_workcontext
        //                          .CurrentUserinformation.User_id}',LAST_MODIFIED_DATE=TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy') WHERE  INVOICE_CODE IN '{cdetails.INVOICE_CODE}'";
        //        var updatepinvoice = _dbContext.ExecuteSqlCommand(updatetPI);
        //        CommercialInvoiceModel returnlist = new CommercialInvoiceModel();
        //        returnlist.LC_TRACK_NO = cdetails.LC_TRACK_NO;
        //        returnlist.INVOICE_CODE = cdetails.INVOICE_CODE;
        //        return returnlist;
        //    }
        //}

        public List<Items> GetAllCommercialItemsList(string filter)
        {
            var sqlquerys = $@"SELECT 
                             LI.LC_TRACK_NO
                            ,LI.INVOICE_CODE
                            ,LI.SNO
                            ,IMS.ITEM_EDESC
                            ,LI.ITEM_CODE
                           ,NVL(CAST(LI.AMOUNT AS DECIMAL(18,2)),0.0)AS AMOUNT
                           ,NVL(CAST(LI.QUANTITY AS DECIMAL(18,2)),0.0) AS QUANTITY
                           ,NVL(CAST(LIT.QUANTITY AS DECIMAL(18,2)) ,0.0)AS TOTAL_QUANTITY
                           ,LI.MU_CODE,LI.EXCHANGE_RATE
                            ,LI.SALES_EXG_RATE,LI.PAYMENT_DATE
                           FROM LC_INVOICE LI
                            LEFT JOIN IP_ITEM_MASTER_SETUP IMS ON LI.ITEM_CODE = IMS.ITEM_CODE
                            AND LI.COMPANY_CODE = IMS.COMPANY_CODE
                            LEFT JOIN LC_ITEM LIT ON LI.LC_TRACK_NO = LIT.LC_TRACK_NO
                            AND LI.ITEM_CODE = LIT.ITEM_CODE                          
                           WHERE LI.INVOICE_NUMBER = '{filter}' AND LI.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
            var result = _dbContext.SqlQuery<Items>(sqlquerys).ToList();
            return result;
            //REMOVED QUERY PART --//AND LI.SNO = LIT.SNO--
        }
        
         public LcLogisticPlanModel GetLogisticItemsList(string lcnumber)
        {
            LcLogisticPlanModel Record = new LcLogisticPlanModel();
            var sqlquery = $@"SELECT LC_TRACK_NO FROM LC_LOC WHERE LC_NUMBER = '{lcnumber}' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
            int? lctracknumber = _dbContext.SqlQuery<int>(sqlquery).FirstOrDefault();
            var LCQUERY = $@"SELECT TO_CHAR(IPS.SUPPLIER_EDESC) AS SUPPLIER_EDESC FROM IP_SUPPLIER_SETUP IPS
                           WHERE IPS.SUPPLIER_CODE ='{lcnumber}'";
            var LC_DESC = _dbContext.SqlQuery<string>(LCQUERY).FirstOrDefault();

            Record.ItemDetails = new List<ItemDetails>();
            var SQL = $@" SELECT LC_TRACK_NO, ITEM_CODE, ITEM_EDESC,MU_CODE, SUM(INVOICE_QUANTITY)AS INVOICE_QUANTITY, HS_CODE, CALC_QUANTITY, CALC_UNIT_PRICE, COUNTRY_CODE,COUNTRY_EDESC,REMARKS  FROM
            (SELECT  LI.LC_TRACK_NO, LI.HS_CODE, LI.ITEM_CODE, IMS.ITEM_EDESC, LI.MU_CODE, LI.QUANTITY AS CALC_QUANTITY, LI.AMOUNT AS CALC_UNIT_PRICE,
            LI.COUNTRY_OF_ORIGIN AS COUNTRY_CODE, LI.REMARKS, LP.CURRENCY_CODE, CS.COUNTRY_CODE || '-' || CS.COUNTRY_EDESC AS COUNTRY_EDESC, LIN.QUANTITY AS INVOICE_QUANTITY FROM LC_ITEM LI
            LEFT JOIN IP_ITEM_MASTER_SETUP IMS ON LI.ITEM_CODE = IMS.ITEM_CODE AND LI.COMPANY_CODE = IMS.COMPANY_CODE
            LEFT JOIN LC_PURCHASE_ORDER LP ON LI.LC_TRACK_NO = LP.LC_TRACK_NO
            LEFT JOIN COUNTRY_SETUP CS ON LI.COUNTRY_OF_ORIGIN = CS.COUNTRY_CODE AND LI.COMPANY_CODE = CS.COMPANY_CODE
            LEFT JOIN LC_INVOICE LIN ON LI.LC_TRACK_NO = LIN.LC_TRACK_NO  AND  LI.ITEM_CODE = LIN.ITEM_CODE
            WHERE LI.LC_TRACK_NO = '{lctracknumber}'  AND LI.COMPANY_CODE =  '{_workcontext.CurrentUserinformation.company_code}' ) TT
            GROUP BY ITEM_CODE, HS_CODE, LC_TRACK_NO, ITEM_EDESC, MU_CODE, CALC_QUANTITY, CALC_UNIT_PRICE, COUNTRY_CODE,COUNTRY_EDESC,REMARKS";
            Record.ItemDetails = _dbContext.SqlQuery<ItemDetails>(SQL).ToList();
          

            return Record;


        }
        public LcLogisticPlanModel GetLogisticPlanList(string lcnumber)
        {
            LcLogisticPlanModel Record = new LcLogisticPlanModel();
            var sqlquery = $@"SELECT LC_TRACK_NO FROM LC_LOC WHERE LC_NUMBER = '{lcnumber}' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
            int? lctracknumber = _dbContext.SqlQuery<int>(sqlquery).FirstOrDefault();
            var LCQUERY = $@"SELECT TO_CHAR(IPS.SUPPLIER_EDESC) AS SUPPLIER_EDESC FROM IP_SUPPLIER_SETUP IPS
                           WHERE IPS.SUPPLIER_CODE ='{lcnumber}'";
            var LC_DESC = _dbContext.SqlQuery<string>(LCQUERY).FirstOrDefault();
            
            var sqlquerys = $@"SELECT TO_CHAR(LOT_NO) AS SNO,LOT_NO AS ROWNUMBER,TO_CHAR(LC_TRACK_NO)  AS LC_TRACK_NO,TO_CHAR(ROUTE_NO) AS ROUTE_NO ,TO_CHAR(SHIPPING_TYPE) AS SHIPPING_TYPE,TO_CHAR(TO_LOCATION_CODE) AS TO_LOCATION_CODE ,TO_CHAR(FROM_LOCATION_CODE) AS FROM_LOCATION_CODE
                            , TO_CHAR(EST_BOOKING_DATE ,'DD/MM/YYYY') AS EST_BOOKING_DATE, TO_CHAR(EST_LOADING_DATE ,'DD/MM/YYYY') AS EST_LOADING_DATE ,TO_CHAR(ACT_BOOKING_DATE ,'DD/MM/YYYY') AS ACT_BOOKING_DATE, TO_CHAR(ACT_LOADING_DATE ,'DD/MM/YYYY') AS ACT_LOADING_DATE ,TO_CHAR(LOC.LOCATION_EDESC) AS TO_LOCATION_EDESC,
                            TO_CHAR(LCS.LOCATION_EDESC) AS FROM_LOCATION_EDESC
                            FROM
                            (SELECT C.LOT_NO AS LOT_NO,C.LC_TRACK_NO  AS LC_TRACK_NO,C.ROUTE_NO AS ROUTE_NO ,C.SHIPPING_TYPE AS SHIPPING_TYPE ,
                             TO_CHAR(C.FROM_LOCATION_CODE) AS FROM_LOCATION_CODE,
                            TO_CHAR(C.TO_LOCATION_CODE) AS TO_LOCATION_CODE,
                            TRUNC(C.EST_BOOKING_DATE)  AS EST_BOOKING_DATE,
                            TRUNC(C.EST_LOADING_DATE)  AS EST_LOADING_DATE,
                            TRUNC(C.ACT_BOOKING_DATE)  AS ACT_BOOKING_DATE,
                            TRUNC(C.ACT_LOADING_DATE)  AS ACT_LOADING_DATE
                            FROM LC_LOGISTIC_PLAN_CONTAINER C
                             WHERE C.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}' AND C.LC_TRACK_NO ='{lctracknumber}' AND DELETED_FLAG='N'
                             GROUP BY LC_TRACK_NO,LOT_NO,ROUTE_NO,SHIPPING_TYPE,FROM_LOCATION_CODE,TO_LOCATION_CODE,EST_BOOKING_DATE,EST_LOADING_DATE,C.ACT_BOOKING_DATE,C.ACT_LOADING_DATE)TT
                             LEFT JOIN LC_LOCATION_SETUP LOC ON LOC.LOCATION_CODE=TT.TO_LOCATION_CODE  
                             LEFT JOIN LC_LOCATION_SETUP LCS ON LCS.LOCATION_CODE=TT.FROM_LOCATION_CODE
                             ORDER BY ROWNUMBER";

            Record.LC_LOGISTIC_PLAN_CONTAINER = _dbContext.SqlQuery<LC_LOGISTIC_PLAN_CONTAINER>(sqlquerys).ToList();

            Record.ItemDetails = new List<ItemDetails>();
            var SQL = $@" SELECT LC_TRACK_NO, ITEM_CODE, ITEM_EDESC,MU_CODE,EXCHANGE_RATE,SALES_EXG_RATE, SUM(INVOICE_QUANTITY)AS INVOICE_QUANTITY, HS_CODE, CALC_QUANTITY, CALC_UNIT_PRICE, COUNTRY_CODE,COUNTRY_EDESC,REMARKS  FROM
            (SELECT  LI.LC_TRACK_NO, LI.HS_CODE, LI.ITEM_CODE, IMS.ITEM_EDESC, LI.MU_CODE, LI.QUANTITY AS CALC_QUANTITY, LI.AMOUNT AS CALC_UNIT_PRICE,
            LI.COUNTRY_OF_ORIGIN AS COUNTRY_CODE, LI.REMARKS, LP.CURRENCY_CODE, CS.COUNTRY_CODE || '-' || CS.COUNTRY_EDESC AS COUNTRY_EDESC, LIN.QUANTITY AS INVOICE_QUANTITY,LIN.EXCHANGE_RATE AS EXCHANGE_RATE,LIN.SALES_EXG_RATE FROM LC_ITEM LI
            LEFT JOIN IP_ITEM_MASTER_SETUP IMS ON LI.ITEM_CODE = IMS.ITEM_CODE AND LI.COMPANY_CODE = IMS.COMPANY_CODE
            LEFT JOIN LC_PURCHASE_ORDER LP ON LI.LC_TRACK_NO = LP.LC_TRACK_NO
            LEFT JOIN COUNTRY_SETUP CS ON LI.COUNTRY_OF_ORIGIN = CS.COUNTRY_CODE AND LI.COMPANY_CODE = CS.COMPANY_CODE
            LEFT JOIN LC_INVOICE LIN ON LI.LC_TRACK_NO = LIN.LC_TRACK_NO  AND  LI.ITEM_CODE = LIN.ITEM_CODE
           WHERE LI.LC_TRACK_NO = '{lctracknumber}'  AND LI.COMPANY_CODE =  '{_workcontext.CurrentUserinformation.company_code}' ) TT
           GROUP BY ITEM_CODE, HS_CODE, LC_TRACK_NO, ITEM_EDESC, MU_CODE,EXCHANGE_RATE,SALES_EXG_RATE, CALC_QUANTITY, CALC_UNIT_PRICE, COUNTRY_CODE,COUNTRY_EDESC,REMARKS";
            Record.ItemDetails = _dbContext.SqlQuery<ItemDetails>(SQL).ToList();

            var PLAN_CONTAINER_QUERY = $@"SELECT  LOT_NO AS ROWNUMBER,PLAN_CONTAINER_CODE AS CONTAINER_ROWNUMBER,TO_CHAR(LPC.SHIPPING_TYPE) AS SHIPPING_TYPE,TO_CHAR(LOGISTIC_PLAN_CODE) AS LOGISTIC_PLAN_CODE,TO_CHAR(LPC.PLAN_CONTAINER_CODE) AS PLAN_CONTAINER_CODE, TO_CHAR(LOT_NO) AS LOT_NO,TO_CHAR(LPC.CONTAINER_CODE) as CONTAINER_CODE, TO_CHAR(LC.CONTAINER_EDESC) AS CONTAINER_EDESC ,TO_CHAR(LPC.LOAD_TYPE) AS LOAD_TYPE,TO_CHAR(LPC.CARRIER_NUMBER) AS CARRIER_NUMBER  from lc_logistic_plan_container lpc
            LEFT JOIN LC_CONTAINER LC ON LC.CONTAINER_CODE = LPC.CONTAINER_CODE WHERE LPC.LC_TRACK_NO = '{lctracknumber}' AND LPC.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}' AND  LPC.PLAN_CONTAINER_CODE  NOT IN (select  LIC.PLAN_CONTAINER_CODE from  LC_INVOICE_CONTAINER  LIC) ORDER BY ROWNUMBER,CONTAINER_ROWNUMBER";
            Record.LC_PLAN_CONTAINER_LIST = _dbContext.SqlQuery<LC_PLAN_CONTAINER_LIST>(PLAN_CONTAINER_QUERY).ToList();


            var LC_LOC_CURRENCY_QUERY = $@"SELECT TO_CHAR(CURRENCY_CODE) AS CURRENCY_CODE, TO_CHAR(COALESCE(EXCHANGE_RATE,0.00)) AS EXCHANGE_RATE FROM
                            (SELECT * FROM EXCHANGE_DETAIL_SETUP ORDER BY CREATED_DATE DESC)TT
                            WHERE TT.CURRENCY_CODE IN (SELECT CURRENCY_CODE FROM LC_PERFOMA_INVOICE LPI WHERE LPI.PINVOICE_CODE IN(SELECT PINVOICE_CODE FROM LC_LOC WHERE LC_NUMBER = '{lcnumber}')
                            ) AND ROWNUM = 1";
            Record.LC_ENTRY_CURRENCY = _dbContext.SqlQuery<LC_ENTRY_CURRENCY>(LC_LOC_CURRENCY_QUERY).FirstOrDefault();
            return Record;


        }
        public List<CommercialInvoiceModel> GetAllCommInvoice()
        {

            var sqlquerys = $@"SELECT DISTINCT 
               TO_CHAR(LI.LOT_NO) AS LOT_NO, LI.INVOICE_CODE,LI.LC_TRACK_NO,LI.INVOICE_DATE,LI.INVOICE_NUMBER,LI.LC_NUMBER,LI.PP_DATE,LI.PP_NO,LD.FILE_DETAIL,LI.INVOICE_CURRENCY,LI.EXCHANGE_RATE,LI.SALES_EXG_RATE,LI.AWB_NUMBER,LI.AWB_DATE,NVL(LI.IS_AIR,'N') AS IS_AIR FROM LC_INVOICE  LI
               LEFT JOIN LC_DOCUMENT LD ON LI.INVOICE_CODE = LD.INVOICE_CODE AND LI.COMPANY_CODE = LD.COMPANY_CODE WHERE LI.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}' ORDER BY LI.LC_TRACK_NO,LOT_NO ASC";
            var result = _dbContext.SqlQuery<CommercialInvoiceModel>(sqlquerys).ToList();
            List<CommercialInvoiceModel> newlist = new List<CommercialInvoiceModel>();
            foreach (var item in result)
            {
                if (item.FILE_DETAIL != null)
                {
                    if (item.FILE_DETAIL.ToString().Contains(':'))
                    {
                        item.mylist = item.FILE_DETAIL.Split(':');
                        item.FILE_DETAIL = null;
                    }
                    else
                    {
                        item.FILE_DETAIL = item.FILE_DETAIL;
                    }
                }
                newlist.Add(item);

            }
            return newlist;
        }

        public List<CommercialInvoiceModel> GetAllCommInvoiceFilter(string lcnumber)
        {
            List<CommercialInvoiceModel> RECORD = new List<CommercialInvoiceModel>();
            if (string.IsNullOrEmpty(lcnumber))
            {
                return RECORD;
            }
            var sqlquerys = $@"SELECT DISTINCT 
               TO_CHAR(LI.LOT_NO) AS LOT_NO, LI.INVOICE_CODE,LI.LC_TRACK_NO,LI.INVOICE_DATE,LI.INVOICE_NUMBER,LI.LC_NUMBER,LI.PP_DATE,LI.PP_NO,LD.FILE_DETAIL,LI.INVOICE_CURRENCY,LI.EXCHANGE_RATE,LI.AWB_NUMBER,LI.AWB_DATE FROM LC_INVOICE  LI
               LEFT JOIN LC_DOCUMENT LD ON LI.INVOICE_CODE = LD.INVOICE_CODE AND LI.COMPANY_CODE = LD.COMPANY_CODE WHERE LI.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}' AND LI.LC_NUMBER LIKE '%{lcnumber}%'  ORDER BY LI.LC_TRACK_NO,LOT_NO ASC";
            var result = _dbContext.SqlQuery<CommercialInvoiceModel>(sqlquerys).ToList();
            List<CommercialInvoiceModel> newlist = new List<CommercialInvoiceModel>();
            foreach (var item in result)
            {
                if (item.FILE_DETAIL != null)
                {
                    if (item.FILE_DETAIL.ToString().Contains(':'))
                    {
                        item.mylist = item.FILE_DETAIL.Split(':');
                        item.FILE_DETAIL = null;
                    }
                    else
                    {
                        item.FILE_DETAIL = item.FILE_DETAIL;
                    }
                }
                newlist.Add(item);

            }
            return newlist;
        }

        public List<CommercialInvoiceModel> GetAllLcNumbers(string filter)
        {
            if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }

            var sqlquery = $@"select LL.LC_NUMBER,LL.LC_TRACK_NO,IPS.SUPPLIER_EDESC,LL.LC_NUMBER AS LC_NUMBER_CODE,LL.CREATED_DATE from LC_LOC LL
                                            LEFT JOIN IP_SUPPLIER_SETUP IPS ON LL.LC_NUMBER = IPS.SUPPLIER_CODE
                                            where UPPER(LL.LC_NUMBER) like '%{filter.ToUpperInvariant()}%' OR UPPER(IPS.SUPPLIER_EDESC) LIKE '%{filter.ToUpperInvariant()}%' AND LL.DELETED_FLAG = 'N'
                                                AND LL.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}' AND LL.LC_TRACK_NO  IN (SELECT DISTINCT LP.LC_TRACK_NO FROM LC_LOGISTIC_PLAN LP) ORDER BY LL.CREATED_DATE DESC";
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
            var sqlquery = $@"SELECT LC_NUMBER,LC_TRACK_NO, SUPPLIER_EDESC, LC_NUMBER AS LC_NUMBER_CODE,CREATED_DATE FROM (select LL.LC_NUMBER,LL.LC_TRACK_NO,IPS.SUPPLIER_EDESC,LL.LC_NUMBER AS LC_NUMBER_CODE,LL.CREATED_DATE from LC_LOC LL
                                            LEFT JOIN IP_SUPPLIER_SETUP IPS ON LL.LC_NUMBER = IPS.SUPPLIER_CODE
                                            where UPPER(LL.LC_NUMBER) like '%{filter.ToUpperInvariant()}%' OR UPPER(IPS.SUPPLIER_EDESC) LIKE '%{filter.ToUpperInvariant()}%' AND LL.DELETED_FLAG = 'N'
                                            AND LL.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}' AND LL.LC_TRACK_NO  IN (SELECT DISTINCT LP.LC_TRACK_NO FROM LC_LOGISTIC_PLAN LP) ORDER BY LL.CREATED_DATE DESC) TT
                                            WHERE TT.SUPPLIER_EDESC IN (select LI.LC_NUMBER from lc_invoice LI )";
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

        public List<CommercialInvoiceModel> GetAllInvoiceNumbers(string filter, string lcnumber)
        {
            if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }

            var sqlquery = $@"SELECT DISTINCT INVOICE_NUMBER,LC_TRACK_NO FROM LC_INVOICE WHERE DELETED_FLAG = 'N' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}' AND LC_NUMBER= '{lcnumber}' AND INVOICE_NUMBER LIKE '%{filter.ToUpperInvariant()}%'";
            var result = _dbContext.SqlQuery<CommercialInvoiceModel>(sqlquery).ToList();
            return result;
        }

        public CommercialInvoiceModel GetDetailByInvoiceNo(string invoiceno)
        {

            var sqlquerys = $@"SELECT LC_NUMBER, INVOICE_CODE,INVOICE_DATE,PP_DATE,PP_NO,INVOICE_CURRENCY,EXCHANGE_RATE,AWB_NUMBER,AWB_DATE FROM LC_INVOICE WHERE INVOICE_NUMBER = '{invoiceno}' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
            var result = _dbContext.SqlQuery<CommercialInvoiceModel>(sqlquerys).FirstOrDefault();
            return result;
        }

        public List<ItemDetails> GetItemsByLCNumber(string lcnumber)
        {
            var sqlquery = $@"SELECT LC_TRACK_NO FROM LC_LOC WHERE LC_NUMBER = '{lcnumber}' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
            int? lctracknumber = _dbContext.SqlQuery<int>(sqlquery).FirstOrDefault();

            var sqlquerys = $@"SELECT  LI.LC_TRACK_NO,LI.HS_CODE,LI.SNO,LI.ITEM_CODE,IMS.ITEM_EDESC,LI.MU_CODE,LI.QUANTITY AS CALC_QUANTITY,LI.AMOUNT AS CALC_UNIT_PRICE,
            LI.COUNTRY_OF_ORIGIN AS COUNTRY_CODE,LI.REMARKS,LP.CURRENCY_CODE,CS.COUNTRY_CODE || '-' || CS.COUNTRY_EDESC AS COUNTRY_EDESC,SUM(LIN.QUANTITY) AS INVOICE_QUANTITY FROM LC_ITEM LI
            LEFT JOIN IP_ITEM_MASTER_SETUP IMS ON LI.ITEM_CODE = IMS.ITEM_CODE AND LI.COMPANY_CODE = IMS.COMPANY_CODE 
            LEFT JOIN LC_PURCHASE_ORDER LP ON LI.LC_TRACK_NO = LP.LC_TRACK_NO 
            LEFT JOIN COUNTRY_SETUP CS ON LI.COUNTRY_OF_ORIGIN = CS.COUNTRY_CODE AND LI.COMPANY_CODE = CS.COMPANY_CODE   
            LEFT JOIN LC_INVOICE LIN ON LI.LC_TRACK_NO = LIN.LC_TRACK_NO  AND  LI.ITEM_CODE = LIN.ITEM_CODE AND LI.SNO = LIN.SNO
               WHERE LI.LC_TRACK_NO = '{lctracknumber}' AND LI.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}' GROUP BY LI.LC_TRACK_NO,LI.HS_CODE,LI.SNO,LI.ITEM_CODE,IMS.ITEM_EDESC,LI.MU_CODE,LI.QUANTITY ,LI.AMOUNT,
            LI.COUNTRY_OF_ORIGIN,LI.REMARKS,LP.CURRENCY_CODE,CS.COUNTRY_CODE ,CS.COUNTRY_EDESC ORDER BY LI.SNO ASC";
            var result = _dbContext.SqlQuery<ItemDetails>(sqlquerys).ToList();
            return result;


        }



        public string InvoiceNumberExist(string ordernumber, string action, int pocode)
        {
            if (action == "create")
            {
                var sqlquery = $@"SELECT INVOICE_NUMBER FROM LC_INVOICE WHERE INVOICE_NUMBER = '{ordernumber}' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}' ";
                var result = _dbContext.SqlQuery<string>(sqlquery).FirstOrDefault();
                return result;
            }
            else
            {
                var sqlquery = $@"SELECT INVOICE_NUMBER FROM LC_INVOICE WHERE INVOICE_NUMBER = '{ordernumber}' AND INVOICE_CODE != '{pocode}' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}' ";
                var result = _dbContext.SqlQuery<string>(sqlquery).FirstOrDefault();
                return result;
            }
        }

        public void RemoveCiImage(LcImageModels imagedetail)
        {
            var sqlquery = $@"select FILE_DETAIL from LC_DOCUMENT where INVOICE_CODE ='{imagedetail.LocCode}' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
            var result = _dbContext.SqlQuery<Documents>(sqlquery).FirstOrDefault();
            string dbpath = result.FILE_DETAIL;
            if (result.FILE_DETAIL != null)
            {
                if (result.FILE_DETAIL.ToString().Contains(':'))
                {
                    result.mylist = result.FILE_DETAIL.Split(':');
                    result.mylist = result.mylist.Where(s => s != imagedetail.Path).ToArray();
                    result.FILE_DETAIL = null;
                    string Paths = LcEntryService.ConvertStringArrayToString(result.mylist);
                    Paths = Paths.Remove(Paths.Length - 1);
                    string query = string.Format(@"UPDATE LC_DOCUMENT SET FILE_DETAIL  = '{0}' WHERE INVOICE_CODE IN ({1})",
                     Paths, imagedetail.LocCode);
                    var rowCount = _dbContext.ExecuteSqlCommand(query);
                }
                else
                {
                    string query = string.Format(@"UPDATE LC_DOCUMENT SET FILE_DETAIL  = '{0}' WHERE INVOICE_CODE IN ({1})",
                   "", imagedetail.LocCode);
                    var rowCount = _dbContext.ExecuteSqlCommand(query);
                }
            }
            else
            {
                string query = string.Format(@"UPDATE LC_DOCUMENT SET FILE_DETAIL  = '{0}' WHERE INVOICE_CODE IN ({1})",
               "", imagedetail.LocCode);
                var rowCount = _dbContext.ExecuteSqlCommand(query);
            }
        }

        public void UpdateImage(LcImageModels lcimagedetail, string purchaseorder)
        {
            var sqlquery = $@"SELECT FILE_DETAIL FROM LC_DOCUMENT WHERE LC_TRACK_NO  ='{lcimagedetail.LocCode}' AND INVOICE_CODE = '{purchaseorder}' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
            var result = _dbContext.SqlQuery<Documents>(sqlquery).FirstOrDefault();

            if (result != null)
            {
                if (result.FILE_DETAIL != null)
                {

                    string dbpath = result.FILE_DETAIL + ":" + lcimagedetail.Path;
                    string query = string.Format(@"UPDATE LC_DOCUMENT SET FILE_DETAIL= '{0}',COMPANY_CODE='{1}',BRANCH_CODE='{2}',LAST_MODIFIED_BY='{3}',LAST_MODIFIED_DATE=TO_DATE('{4}', 'MM/dd/yyyy') WHERE LC_TRACK_NO IN ({5}) AND INVOICE_CODE IN ({6})",
                   dbpath, _workcontext.CurrentUserinformation.company_code, _workcontext.CurrentUserinformation.branch_code, _workcontext.CurrentUserinformation.User_id, DateTime.Now.ToString("MM/dd/yyyy"), lcimagedetail.LocCode, purchaseorder);
                    var rowCount = _dbContext.ExecuteSqlCommand(query);
                }
                else
                {
                    string query = string.Format(@"UPDATE LC_DOCUMENT SET FILE_DETAIL= '{0}',COMPANY_CODE='{1}',BRANCH_CODE='{2}',LAST_MODIFIED_BY='{3}',LAST_MODIFIED_DATE=TO_DATE('{4}', 'MM/dd/yyyy') WHERE LC_TRACK_NO IN ({5}) AND INVOICE_CODE IN ({6})",
                   lcimagedetail.Path, _workcontext.CurrentUserinformation.company_code, _workcontext.CurrentUserinformation.branch_code, _workcontext.CurrentUserinformation.User_id, DateTime.Now.ToString("MM/dd/yyyy"), lcimagedetail.LocCode, purchaseorder);
                    var rowCount = _dbContext.ExecuteSqlCommand(query);
                }
            }
            else
            {
                var lcdocument = $@"SELECT LC_INVOICE_SEQ.NEXTVAL as SNO FROM DUAL";
                var documentno = _dbContext.SqlQuery<Documents>(lcdocument).FirstOrDefault();
                string query = string.Format(@"INSERT INTO LC_DOCUMENT (LC_TRACK_NO,INVOICE_CODE,SNO,FILE_DETAIL,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG)VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}',TO_DATE('{7}', 'MM/dd/yyyy'),'{8}')",
                lcimagedetail.LocCode, purchaseorder, documentno.SNO, lcimagedetail.Path, _workcontext.CurrentUserinformation.company_code, _workcontext.CurrentUserinformation.branch_code, _workcontext.CurrentUserinformation.User_id, DateTime.Now.ToString("MM/dd/yyyy"), 'N');
                var rowCount = _dbContext.ExecuteSqlCommand(query);

            }

        }

        //public void UpdateQuantity(Items cdetail)
        //{
        //    if (cdetail != null)
        //    {
        //        var updatetQuantity = $@"UPDATE LC_INVOICE SET QUANTITY='{cdetail.QUANTITY}' ,COMPANY_CODE='{_workcontext.CurrentUserinformation
        //                           .company_code}',BRANCH_CODE='{_workcontext.CurrentUserinformation.branch_code}',LAST_MODIFIED_BY='{_workcontext
        //                           .CurrentUserinformation.User_id}',LAST_MODIFIED_DATE=TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy') WHERE  INVOICE_CODE = '{cdetail.INVOICE_CODE}' AND ITEM_CODE = '{cdetail.ITEM_CODE}' AND LC_TRACK_NO = '{cdetail.LC_TRACK_NO}' AND SNO = '{cdetail.SNO}'";
        //        var updatepinvoice = _dbContext.ExecuteSqlCommand(updatetQuantity);
        //    }
        //}


        public string UpdateQuantity(Items cdetail)
        {
            if (cdetail != null)
            {
                var query1 = $@"SELECT LI1.QUANTITY AS TOTALQUANTITYBYITEM FROM LC_INVOICE LI1 WHERE LI1.LC_TRACK_NO = '{cdetail.LC_TRACK_NO}' AND LI1.ITEM_CODE='{cdetail.ITEM_CODE}' AND LI1. INVOICE_CODE='{cdetail.INVOICE_CODE}'";
                int previousItemQuantity = _dbContext.SqlQuery<int>(query1).FirstOrDefault();
                var query = $@"SELECT SUM(LI1.QUANTITY) AS TOTALQUANTITYBYITEM FROM LC_INVOICE LI1 WHERE LI1.LC_TRACK_NO = '{cdetail.LC_TRACK_NO}' AND LI1.ITEM_CODE='{cdetail.ITEM_CODE}'";
                int totalQuantityByItem = _dbContext.SqlQuery<int>(query).FirstOrDefault();
                int comparableQuantity = (totalQuantityByItem - (previousItemQuantity - Convert.ToInt32(cdetail.QUANTITY)));

                if (comparableQuantity > Convert.ToInt32(cdetail.TOTAL_QUANTITY))
                {
                    return "Exceeded";
                }
                else
                {
                    string paymentdate = string.Empty;
                    if (cdetail.PAYMENT_DATE.HasValue)
                    {
                        paymentdate = $"TO_DATE('{cdetail.PAYMENT_DATE.Value.AddDays(1).ToString("MM/dd/yyyy")}', 'MM/dd/yyyy')";
                    }
                    else
                    { paymentdate = null;}
                    //var updatetQuantity = $@"UPDATE LC_LOGISTIC_PLAN_ITEM SET QUANTITY='{itemdetail.QUANTITY}' ,COMPANY_CODE='{_workcontext.CurrentUserinformation
                    //                                  .company_code}',BRANCH_CODE='{_workcontext.CurrentUserinformation.branch_code}',LAST_MODIFIED_BY='{_workcontext
                    //                                  .CurrentUserinformation.User_id}',LAST_MODIFIED_DATE=TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy') WHERE LOGISTIC_PLAN_CODE = '{itemdetail.LOGISTIC_PLAN_CODE}' AND  ITEM_CODE = '{itemdetail.ITEM_CODE}' AND LC_TRACK_NO = '{itemdetail.LC_TRACK_NO}' ";
                    //var lcLogisticPlanItem = _dbContext.ExecuteSqlCommand(updatetQuantity);


                    //var updatetQuantity = $@"UPDATE LC_INVOICE SET QUANTITY='{cdetail.QUANTITY}',EXCHANGE_RATE='{cdetail.EXCHANGE_RATE}',SALES_EXG_RATE='{cdetail.SALES_EXG_RATE}' ,COMPANY_CODE='{_workcontext.CurrentUserinformation
                    //               .company_code}',BRANCH_CODE='{_workcontext.CurrentUserinformation.branch_code}',LAST_MODIFIED_BY='{_workcontext
                    //               .CurrentUserinformation.User_id}',LAST_MODIFIED_DATE=TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),PAYMENT_DATE=TO_DATE('{cdetail.PAYMENT_DATE.Value.AddDays(1).ToString("MM/dd/yyyy")}','MM/dd/yyyy') WHERE  INVOICE_CODE = '{cdetail.INVOICE_CODE}' AND ITEM_CODE = '{cdetail.ITEM_CODE}' AND LC_TRACK_NO = '{cdetail.LC_TRACK_NO}' AND SNO = '{cdetail.SNO}'";
                    var updatetQuantity = $@"UPDATE LC_INVOICE SET QUANTITY='{cdetail.QUANTITY}',EXCHANGE_RATE='{cdetail.EXCHANGE_RATE}',SALES_EXG_RATE='{cdetail.SALES_EXG_RATE}' ,COMPANY_CODE='{_workcontext.CurrentUserinformation
                                .company_code}',BRANCH_CODE='{_workcontext.CurrentUserinformation.branch_code}',LAST_MODIFIED_BY='{_workcontext
                                .CurrentUserinformation.User_id}',LAST_MODIFIED_DATE=TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),PAYMENT_DATE={paymentdate} WHERE  INVOICE_CODE = '{cdetail.INVOICE_CODE}' AND ITEM_CODE = '{cdetail.ITEM_CODE}' AND LC_TRACK_NO = '{cdetail.LC_TRACK_NO}' AND SNO = '{cdetail.SNO}'";
                    var updatepinvoice = _dbContext.ExecuteSqlCommand(updatetQuantity);

                    return "Success";
                }


            }
            else { return "fail"; }
        }

        public CommercialInvoiceModel EditCommercialInvoice(string lotNumber, string invoiceNumber, string lcNumber)
        {
            var sqlquerys = $@"SELECT 
                                 DISTINCT LI.INVOICE_CODE
	                            ,LI.LC_TRACK_NO                        
	                            ,LI.INVOICE_NUMBER AS INVOICE_NUMBER
	                            ,LI.INVOICE_DATE AS INVOICE_DATE
	                            ,LI.PP_DATE AS PP_DATE
	                            ,LI.PP_NO AS PP_NO
	                            ,LI.AWB_DATE AS AWB_DATE
	                            ,LI.AWB_NUMBER AS AWB_NUMBER
                            FROM LC_INVOICE LI
                            WHERE LI.INVOICE_NUMBER = '{invoiceNumber}'
	                            AND LI.LC_NUMBER = '{lcNumber}'
	                            AND LI.LOT_NO = '{lotNumber}' 
                                AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
            var result = _dbContext.SqlQuery<CommercialInvoiceModel>(sqlquerys).FirstOrDefault();
            return result;
        }

        public List<CommercialInvoicHistoryModel> getAllHistoryCommercialInvoiceList(string lctrackno)
        {
            var sqlquery = $@"SELECT ALI.INVOICE_CODE
                                    ,ALI.ITEM_CODE
                                    ,IMS.ITEM_EDESC
                                    ,ALI.MU_CODE                                   
                                    ,ALI.QUANTITY
                                    ,ALI.AMOUNT
                                    ,ALI.LC_TRACK_NO
                                    ,ALI.LC_NUMBER
                                    ,ALI.INVOICE_NUMBER
                                    ,ALI.CREATED_DATE
                                    ,ALI.CREATED_BY
                                    ,SAU.LOGIN_EDESC AS CREATED_BY_EDESC
                                    ,ALI.LAST_MODIFIED_BY
                                    ,SA.LOGIN_EDESC AS LAST_MODIFIED_BY_EDESC
                                    ,ALI.LAST_MODIFIED_DATE AS LAST_MODIFIED_DATE
                                FROM AUD_LC_INVOICE ALI
                                LEFT JOIN SC_APPLICATION_USERS SAU ON ALI.CREATED_BY = SAU.USER_NO
                                    AND ALI.COMPANY_CODE = SAU.COMPANY_CODE
                                LEFT JOIN SC_APPLICATION_USERS SA ON ALI.LAST_MODIFIED_BY = SA.USER_NO
                                    AND ALI.COMPANY_CODE = SA.COMPANY_CODE
                                LEFT JOIN IP_ITEM_MASTER_SETUP IMS ON ALI.ITEM_CODE = IMS.ITEM_CODE
                                     AND ALI.COMPANY_CODE = IMS.COMPANY_CODE
                                WHERE ALI.LC_TRACK_NO = '{lctrackno}'
                                    AND ALI.DELETED_FLAG = 'N'
                                    AND ALI.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'
                                ORDER BY ALI.LC_TRACK_NO ASC";

            var result = _dbContext.SqlQuery<CommercialInvoicHistoryModel>(sqlquery).ToList();
            //result.Any(x => x.LAST_MODIFIED_DATE == null ? "" : x.LAST_MODIFIED_DATE);
            return result;
        }

        public List<InvoiceNumberModels> GetAllInvoiceNumberByFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }

            var sqlquery = $@" SELECT LI.INVOICE_CODE AS InvoiceCode 
                                     ,LI.INVOICE_NUMBER AS InvoiceNumber 
                                FROM LC_INVOICE LI 
                                WHERE
                                LI.deleted_flag='N' 
                                AND LI.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}' ORDER BY InvoiceNumber ASC";
            var itemNameList = _dbContext.SqlQuery<InvoiceNumberModels>(sqlquery).ToList();
            return itemNameList;
        }

        public string CreateCIDONumber(List<CIDOModel> cIDOModel)
        {
            var company_code = _workcontext.CurrentUserinformation.company_code;
            var branch_code = _workcontext.CurrentUserinformation.branch_code;
            var userid = _workcontext.CurrentUserinformation.User_id;
            int row = 0;

            foreach (var data in cIDOModel)
            {

                var INVOICE_CODE= data.INVOICE_CODE;
                var ITEM_CODE=data.ITEM_CODE;
                var delete_sqlquery = $@" DELETE FROM LC_DO LD
                                        WHERE LD.INVOICE_CODE='{INVOICE_CODE}' 
                                        AND ITEM_CODE='{ITEM_CODE}'
                                        AND LD.deleted_flag='N' 
                                        AND LD.COMPANY_CODE = '{company_code}'";
                if (!string.IsNullOrEmpty(INVOICE_CODE) && !string.IsNullOrEmpty(ITEM_CODE))
                {
                    row = _dbContext.ExecuteSqlCommand(delete_sqlquery);
                }
          
            }
                 
           foreach (var data in cIDOModel)
            {
                var sqlquery = $@" SELECT TO_CHAR(NVL(MAX(TO_NUMBER(LC_DO_CODE))+1,1)) 
                                FROM LC_DO";
                var max_lc_do_number = _dbContext.SqlQuery<string>(sqlquery).FirstOrDefault();

                if (row > 0)
                {
                    var insertquery = $@"INSERT INTO LC_DO(LC_DO_CODE,LC_TRACK_NO,INVOICE_CODE,ITEM_CODE,DO_NUMBER,QUANTITY,REMARKS,STATUS,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,DELETED_FLAG,COMPANY_CODE)
                                     VALUES('{max_lc_do_number}','{data.LC_TRACK_NO}','{data.INVOICE_CODE}','{data.ITEM_CODE}','{data.DO_NUMBER}','{data.QUANTITY}','{data.REMARKS}','{data.STATUS}','{userid}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'N','{company_code}')";
                    _dbContext.ExecuteSqlCommand(insertquery);
                }
                else
                {
                    var insertquery = $@"INSERT INTO LC_DO(LC_DO_CODE,LC_TRACK_NO,INVOICE_CODE,ITEM_CODE,DO_NUMBER,QUANTITY,REMARKS,STATUS,CREATED_BY,CREATED_DATE,DELETED_FLAG,COMPANY_CODE)
                                     VALUES('{max_lc_do_number}','{data.LC_TRACK_NO}','{data.INVOICE_CODE}','{data.ITEM_CODE}','{data.DO_NUMBER}','{data.QUANTITY}','{data.REMARKS}','{data.STATUS}','{userid}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'N','{company_code}')";
                    _dbContext.ExecuteSqlCommand(insertquery);
                }
               
               
            }
            return "inserted";
        }

        public List<CIDOModel> GetCIItemDOByItemCode(string LC_TRACK_NO, string INVOICE_CODE, string ITEM_CODE)
        {
            List<CIDOModel> Record = new List<CIDOModel>();
            var COMPANY_CODE= _workcontext.CurrentUserinformation.company_code;
            var BRANCH_CODE = _workcontext.CurrentUserinformation.branch_code;
            var sqlquery = $@" SELECT TO_CHAR(LC_DO_CODE) AS LC_DO_CODE, TO_CHAR(DO_NUMBER) AS DO_NUMBER, TO_CHAR(QUANTITY) AS QUANTITY  FROM LC_DO WHERE LC_TRACK_NO='{LC_TRACK_NO}' AND INVOICE_CODE='{INVOICE_CODE}' AND ITEM_CODE='{ITEM_CODE}' AND COMPANY_CODE='{COMPANY_CODE}' ORDER BY LC_DO_CODE ";
            Record = _dbContext.SqlQuery<CIDOModel>(sqlquery).ToList();
            return Record;
        }
    }
}