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
    public class CIPaymentService : ICIPaymentService
    {
        private IDbContext _dbContext;
        private IWorkContext _workcontext;
        public CIPaymentService(IDbContext dbContext, IWorkContext iWorkContext)
        {
            this._workcontext = iWorkContext;
            this._dbContext = dbContext;
        }
        public List<CIPaymentSettlementModel> getAllCISettlement()
        {
            var sqlquerys = $@"select DISTINCT TO_CHAR(LPS.PS_CODE) AS PS_CODE, TO_CHAR(LPS.LC_TRACK_NO) AS LC_TRACK_NO,TO_CHAR(LPS.INVOICE_CODE) AS INVOICE_CODE,TO_CHAR(LI.INVOICE_NUMBER) AS INVOICE_NUMBER,DECODE(LL.PTERMS_CODE,121, 'AT SIGHT',122, 'USANCE') AS PTERMS ,TO_CHAR(COALESCE(LPS.CI_AMOUNT,0.00)) AS CI_AMOUNT, TO_CHAR(LPS.CURRENCY) AS CURRENCY,TO_CHAR(COALESCE(LPS.EXCHANGE_RATE_AT_PAYMENT,0.00))  AS  EXCHANGE_RATE_AT_PAYMENT,TO_CHAR(COALESCE(LPS.DERIVED_TOTAL_AMOUNT,0.00)) AS DERIVED_TOTAL_AMOUNT,TO_CHAR(LPS.SETTLEMENT_DATE,'mm-dd-yyyy') AS  SETTLEMENT_DATE, LPSD.FILE_DETAIL AS FILE_DETAIL  from LC_PAYMENT_SETTLEMENT LPS
                               INNER JOIN LC_INVOICE LI ON LI.INVOICE_CODE=  LPS.INVOICE_CODE
                               INNER JOIN LC_LOC LL ON LI.LC_TRACK_NO = LL.LC_TRACK_NO 
                               LEFT JOIN LC_PAYMENT_SETTLEMENT_DOC LPSD ON LPSD.PS_CODE= LPS.PS_CODE WHERE LPS.BRANCH_CODE ='{_workcontext.CurrentUserinformation.branch_code}' AND LPS.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}' ORDER BY LPS.PS_CODE";
            var result = _dbContext.SqlQuery<CIPaymentSettlementModel>(sqlquerys).ToList();


            List<CIPaymentSettlementModel> newlist = new List<CIPaymentSettlementModel>();
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

        public CIPaymentSettlementModel AddUpdateCIPaymentSettlement(CIPaymentSettlementModel cIPaymentSettlementModel)
        {
            if (!string.IsNullOrEmpty(cIPaymentSettlementModel.PS_CODE))
            {
                //update query
                string UPDATEQUERY = $@"UPDATE LC_PAYMENT_SETTLEMENT 
                                        SET
                                        CI_AMOUNT='{cIPaymentSettlementModel.CI_AMOUNT}',
                                        CURRENCY='{cIPaymentSettlementModel.CURRENCY}',
                                        EXCHANGE_RATE_AT_PAYMENT='{cIPaymentSettlementModel.EXCHANGE_RATE_AT_PAYMENT}',
                                        DERIVED_TOTAL_AMOUNT='{cIPaymentSettlementModel.DERIVED_TOTAL_AMOUNT}',
                                        SETTLEMENT_DATE=TO_DATE('{cIPaymentSettlementModel.SETTLEMENT_DATE}','MM/dd/yyyy')
                                        WHERE PS_CODE='{cIPaymentSettlementModel.PS_CODE}' and BRANCH_CODE ='{_workcontext.CurrentUserinformation.branch_code}' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
                _dbContext.ExecuteSqlCommand(UPDATEQUERY);
                cIPaymentSettlementModel.RESULT = "Updated";

            }
            else
            {
                string countquery = $@"select COUNT(LPS.INVOICE_CODE) from LC_PAYMENT_SETTLEMENT LPS WHERE LPS.INVOICE_CODE='{cIPaymentSettlementModel.INVOICE_CODE}' and LPS.BRANCH_CODE ='{_workcontext.CurrentUserinformation.branch_code}' AND LPS.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
                var rowCount = _dbContext.SqlQuery<int>(countquery).FirstOrDefault();
                if (rowCount > 0)
                {
                    cIPaymentSettlementModel.RESULT = "Existed";
                }

                string lc_track_number_query = $@"select TO_CHAR(I.LC_TRACK_NO) as LC_TRACK_NO from lc_invoice I where I.INVOICE_CODE ='{cIPaymentSettlementModel.INVOICE_CODE}' and I.BRANCH_CODE ='{_workcontext.CurrentUserinformation.branch_code}' AND I.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
                cIPaymentSettlementModel.LC_TRACK_NO = _dbContext.SqlQuery<string>(lc_track_number_query).FirstOrDefault();

                string MAXPSCODE_query = $@"select  TO_CHAR(NVL(MAX(PS_CODE+1),1)) AS PS_CODE from LC_PAYMENT_SETTLEMENT LPS WHERE LPS.BRANCH_CODE ='{_workcontext.CurrentUserinformation.branch_code}' AND LPS.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
                cIPaymentSettlementModel.PS_CODE = _dbContext.SqlQuery<string>(MAXPSCODE_query).FirstOrDefault();

                string insertquery = $@"INSERT INTO LC_PAYMENT_SETTLEMENT(PS_CODE,LC_TRACK_NO,INVOICE_CODE,CI_AMOUNT,CURRENCY,EXCHANGE_RATE_AT_PAYMENT,DERIVED_TOTAL_AMOUNT,SETTLEMENT_DATE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG)
                                        Values('{cIPaymentSettlementModel.PS_CODE}','{ cIPaymentSettlementModel.LC_TRACK_NO}','{cIPaymentSettlementModel.INVOICE_CODE}','{cIPaymentSettlementModel.CI_AMOUNT}','{cIPaymentSettlementModel.CURRENCY}','{cIPaymentSettlementModel.EXCHANGE_RATE_AT_PAYMENT}','{cIPaymentSettlementModel.DERIVED_TOTAL_AMOUNT}',TO_DATE('{cIPaymentSettlementModel.SETTLEMENT_DATE}','MM/dd/yyyy') ,'{_workcontext.CurrentUserinformation.company_code}','{_workcontext.CurrentUserinformation.branch_code}',
                                        '{_workcontext.CurrentUserinformation.User_id}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'{'N'}')";
                _dbContext.ExecuteSqlCommand(insertquery);
                _dbContext.SaveChanges();
                // cIPaymentSettlementModel.PS_CODE = cIPaymentSettlementModel.PS_CODE;
                cIPaymentSettlementModel.RESULT = "Inserted";
            }
            return cIPaymentSettlementModel;

        }

        public void UpdateImage(LcImageModels lcimagedetail, string PSCODE)
        {

            var sqlquery = $@"SELECT FILE_DETAIL FROM LC_PAYMENT_SETTLEMENT_DOC WHERE LC_TRACK_NO  ='{lcimagedetail.LocCode}' AND PS_CODE = '{PSCODE}' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
            var result = _dbContext.SqlQuery<Documents>(sqlquery).FirstOrDefault();
            if (result != null)
            {
                if (result.FILE_DETAIL != null)
                {

                    string dbpath = result.FILE_DETAIL + ":" + lcimagedetail.Path;
                    string query = string.Format(@"UPDATE LC_PAYMENT_SETTLEMENT_DOC SET FILE_DETAIL= '{0}',COMPANY_CODE='{1}',BRANCH_CODE='{2}',LAST_MODIFIED_BY='{3}',LAST_MODIFIED_DATE=TO_DATE('{4}', 'MM/dd/yyyy') WHERE PS_CODE = '{5}'",
                    dbpath, _workcontext.CurrentUserinformation.company_code, _workcontext.CurrentUserinformation.branch_code, _workcontext.CurrentUserinformation.User_id, DateTime.Now.ToString("MM/dd/yyyy"), PSCODE);
                    var rowCount = _dbContext.ExecuteSqlCommand(query);
                }
                else
                {
                    string query = string.Format(@"UPDATE LC_PAYMENT_SETTLEMENT_DOC SET FILE_DETAIL= '{0}',COMPANY_CODE='{1}',BRANCH_CODE='{2}',LAST_MODIFIED_BY='{3}',LAST_MODIFIED_DATE=TO_DATE('{4}', 'MM/dd/yyyy') WHERE PS_CODE = '{5}'",
                    lcimagedetail.Path, _workcontext.CurrentUserinformation.company_code, _workcontext.CurrentUserinformation.branch_code, _workcontext.CurrentUserinformation.User_id, DateTime.Now.ToString("MM/dd/yyyy"), PSCODE);
                    var rowCount = _dbContext.ExecuteSqlCommand(query);
                }
            }
            else
            {
                var maxdocquery = $@"SELECT TO_CHAR(NVL(MAX(PS_DOC_CODE+1),1)) FROM LC_PAYMENT_SETTLEMENT_DOC";
                var maxPS_DOC_CODE = _dbContext.SqlQuery<string>(maxdocquery).FirstOrDefault();
                string query = string.Format(@"INSERT INTO LC_PAYMENT_SETTLEMENT_DOC (PS_DOC_CODE,LC_TRACK_NO,PS_CODE,SNO,FILE_DETAIL,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG)VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',TO_DATE('{8}', 'MM/dd/yyyy'),'{9}')",
                maxPS_DOC_CODE, lcimagedetail.LocCode, PSCODE, maxPS_DOC_CODE, lcimagedetail.Path, _workcontext.CurrentUserinformation.company_code, _workcontext.CurrentUserinformation.branch_code, _workcontext.CurrentUserinformation.User_id, DateTime.Now.ToString("MM/dd/yyyy"), 'N');
                var rowCount = _dbContext.ExecuteSqlCommand(query);

            }
        }

        public void RemoveCIPSImage(LcImageModels imagedetail)
        {
            var sqlquery = $@"select FILE_DETAIL from LC_PAYMENT_SETTLEMENT_DOC where PS_CODE ='{imagedetail.LocCode}' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
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
                    string query = string.Format(@"UPDATE LC_PAYMENT_SETTLEMENT_DOC SET FILE_DETAIL  = '{0}' WHERE PS_CODE IN ({1})",
                    Paths, imagedetail.LocCode);
                    var rowCount = _dbContext.ExecuteSqlCommand(query);
                }
                else
                {
                    string query = string.Format(@"UPDATE LC_PAYMENT_SETTLEMENT_DOC SET FILE_DETAIL  = '{0}' WHERE PS_CODE IN ({1})",
                   "", imagedetail.LocCode);
                    var rowCount = _dbContext.ExecuteSqlCommand(query);
                }
            }
            else
            {
                string query = string.Format(@"UPDATE LC_PAYMENT_SETTLEMENT_DOC SET FILE_DETAIL  = '{0}' WHERE PS_CODE IN ({1})",
               "", imagedetail.LocCode);
                var rowCount = _dbContext.ExecuteSqlCommand(query);
            }
        }

        public List<CIPaymentSettlementHistoryModel> getAllHistoryCIPaymentSettlementList(string lctrackno)
        {
            var sqlquery = $@"SELECT ALPSD.PS_DOC_CODE
	                                ,ALPSD.PS_CODE
	                                ,ALPSD.LC_TRACK_NO
	                                ,ALPSD.FILE_DETAIL
	                                ,ALPSD.REMARKS
	                                ,ALPSD.CREATED_DATE
	                                ,ALPSD.CREATED_BY
	                                ,SAU.LOGIN_EDESC AS CREATED_BY_EDESC
	                                ,ALPSD.LAST_MODIFIED_BY
	                                ,SA.LOGIN_EDESC AS LAST_MODIFIED_BY_EDESC
	                                ,ALPSD.LAST_MODIFIED_DATE AS LAST_MODIFIED_DATE
                                FROM AUD_LC_PAYMENT_SETTLEMENT_DOC ALPSD
                                LEFT JOIN SC_APPLICATION_USERS SAU ON ALPSD.CREATED_BY = SAU.USER_NO
	                                AND ALPSD.COMPANY_CODE = SAU.COMPANY_CODE
                                LEFT JOIN SC_APPLICATION_USERS SA ON ALPSD.LAST_MODIFIED_BY = SA.USER_NO
	                                AND ALPSD.COMPANY_CODE = SA.COMPANY_CODE
                                WHERE ALPSD.LC_TRACK_NO = '{lctrackno}'
                                    AND ALPSD.DELETED_FLAG = 'N'
                                    AND ALPSD.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'
                                ORDER BY ALPSD.LC_TRACK_NO ASC";

            var result = _dbContext.SqlQuery<CIPaymentSettlementHistoryModel>(sqlquery).ToList();
            //result.Any(x => x.LAST_MODIFIED_DATE == null ? "" : x.LAST_MODIFIED_DATE);
            return result;
        }

        //public CIPaymentSettlementModel LoadCIInfo(string InvoiceCode, string SettlementDate)
        //{               
        //    CIPaymentSettlementModel Record = new CIPaymentSettlementModel();
        //    CIPaymentSettlementModel Record2 = new CIPaymentSettlementModel();

        //   var PTERMS_QUERY=$@"select TO_CHAR(LL.PTERMS_CODE) from LC_LOC LL, LC_INVOICE LI  
        //                        WHERE LI.LC_TRACK_NO = LL.LC_TRACK_NO AND LI.INVOICE_CODE =  '{InvoiceCode}'";
        //    var result = _dbContext.SqlQuery<string>(PTERMS_QUERY).FirstOrDefault();
        //    if (result == "121" && !string.IsNullOrEmpty(SettlementDate))
        //    {
        //        var AMOUNT_QUERY = $@"SELECT TO_CHAR(SUM(AMOUNT)) AS CI_AMOUNT FROM LC_INVOICE LI WHERE INVOICE_CODE = '{InvoiceCode}'";
        //        Record2 = _dbContext.SqlQuery<CIPaymentSettlementModel>(AMOUNT_QUERY).FirstOrDefault();


        //        var CURRENCY_QUERY = $@"SELECT TO_CHAR(EXCHANGE_RATE) AS EXCHANGE_RATE_AT_PAYMENT,TO_CHAR(CURRENCY_CODE) AS CURRENCY FROM EXCHANGE_DETAIL_SETUP
        //                        WHERE EXCHANGE_DATE =(SELECT MAX(EXCHANGE_DATE) FROM EXCHANGE_DETAIL_SETUP
        //                        WHERE EXCHANGE_DATE <=TO_DATE('{SettlementDate}','MM-DD-YYYY') 
        //                        AND CURRENCY_CODE IN (SELECT CURRENCY_CODE FROM LC_PERFOMA_INVOICE LPI WHERE LPI.PINVOICE_CODE IN(SELECT PINVOICE_CODE FROM LC_LOC LL, LC_INVOICE LI WHERE LL.LC_TRACK_NO = LI.LC_TRACK_NO AND LI.INVOICE_CODE = '{InvoiceCode}')))
        //                        AND CURRENCY_CODE IN(SELECT CURRENCY_CODE FROM LC_PERFOMA_INVOICE LPI WHERE LPI.PINVOICE_CODE IN(SELECT PINVOICE_CODE FROM LC_LOC LL, LC_INVOICE LI WHERE LL.LC_TRACK_NO = LI.LC_TRACK_NO AND LI.INVOICE_CODE =  '{InvoiceCode}'))";
        //        Record = _dbContext.SqlQuery<CIPaymentSettlementModel>(CURRENCY_QUERY).FirstOrDefault();

        //        Record.CI_AMOUNT = Record2.CI_AMOUNT.ToString();

        //    }
        //    if (result == "121")
        //    {
        //        Record.pterm = "ATSIGHT";
        //    }
        //    if (result == "122")
        //    {

        //        var AMOUNT_QUERY = $@"SELECT TO_CHAR(SUM(AMOUNT)) AS CI_AMOUNT, TO_CHAR(LI.PP_DATE,'MM-DD-YYYY') AS PP_DATE FROM LC_INVOICE LI WHERE INVOICE_CODE = '{InvoiceCode}' GROUP BY LI.PP_DATE";
        //        Record2 = _dbContext.SqlQuery<CIPaymentSettlementModel>(AMOUNT_QUERY).FirstOrDefault();

        //        if (string.IsNullOrEmpty(Record2.PP_DATE))
        //        {
        //            Record.INVALID = "INVALID";
        //            return Record;
        //        }

        //        var CURRENCY_QUERY = $@"SELECT TO_CHAR(EXCHANGE_RATE) AS EXCHANGE_RATE_AT_PAYMENT,TO_CHAR(CURRENCY_CODE) AS CURRENCY FROM EXCHANGE_DETAIL_SETUP
        //                        WHERE EXCHANGE_DATE =(SELECT MAX(EXCHANGE_DATE) FROM EXCHANGE_DETAIL_SETUP
        //                        WHERE EXCHANGE_DATE <=TO_DATE('{Record2.PP_DATE}','MM-DD-YYYY') 
        //                        AND CURRENCY_CODE IN (SELECT CURRENCY_CODE FROM LC_PERFOMA_INVOICE LPI WHERE LPI.PINVOICE_CODE IN(SELECT PINVOICE_CODE FROM LC_LOC LL, LC_INVOICE LI WHERE LL.LC_TRACK_NO = LI.LC_TRACK_NO AND LI.INVOICE_CODE =  '{InvoiceCode}')))
        //                        AND CURRENCY_CODE IN(SELECT CURRENCY_CODE FROM LC_PERFOMA_INVOICE LPI WHERE LPI.PINVOICE_CODE IN(SELECT PINVOICE_CODE FROM LC_LOC LL, LC_INVOICE LI WHERE LL.LC_TRACK_NO = LI.LC_TRACK_NO AND LI.INVOICE_CODE =  '{InvoiceCode}'))";
        //        Record = _dbContext.SqlQuery<CIPaymentSettlementModel>(CURRENCY_QUERY).FirstOrDefault();



        //        Record.pterm = "USANCE";
        //        Record.PP_DATE = Record2.PP_DATE.ToString();
        //        Record.CI_AMOUNT = Record2.CI_AMOUNT.ToString();
        //    }
        //     return Record;

        //}


        public CIPaymentSettlementModel LoadCIInfo(string InvoiceCode)
        {
            CIPaymentSettlementModel Record = new CIPaymentSettlementModel();
            CIPaymentSettlementModel Record2 = new CIPaymentSettlementModel();

            var PTERMS_QUERY = $@"select TO_CHAR(LL.PTERMS_CODE) from LC_LOC LL, LC_INVOICE LI  
                                WHERE LI.LC_TRACK_NO = LL.LC_TRACK_NO AND LI.INVOICE_CODE =  '{InvoiceCode}'";
            var result = _dbContext.SqlQuery<string>(PTERMS_QUERY).FirstOrDefault();
            //if (result == "202")
            //{
            //    var AMOUNT_QUERY = $@"SELECT TO_CHAR(SUM(AMOUNT)) AS CI_AMOUNT FROM LC_INVOICE LI WHERE INVOICE_CODE = '{InvoiceCode}'";
            //    Record2 = _dbContext.SqlQuery<CIPaymentSettlementModel>(AMOUNT_QUERY).FirstOrDefault();


            //    var CURRENCY_QUERY = $@"SELECT DISTINCT LI.CURRENCY_CODE AS CURRENCY  FROM  LC_ITEM LI,LC_LOC LL,LC_INVOICE LINV WHERE  LL.LC_TRACK_NO = LI.LC_TRACK_NO 
            //                            AND LL.LC_TRACK_NO = LINV.LC_TRACK_NO
            //                            AND  LINV.INVOICE_CODE = '{InvoiceCode}'";

            //    Record = _dbContext.SqlQuery<CIPaymentSettlementModel>(CURRENCY_QUERY).FirstOrDefault();

            //    Record.CI_AMOUNT = Record2.CI_AMOUNT.ToString();
            //    Record.pterm = "ATSIGHT";
            //}

            //if (result == "122")
            //{

            //    var AMOUNT_QUERY = $@"SELECT TO_CHAR(SUM(AMOUNT)) AS CI_AMOUNT, TO_CHAR(LI.PP_DATE,'MM-DD-YYYY') AS PP_DATE FROM LC_INVOICE LI WHERE INVOICE_CODE = '{InvoiceCode}' GROUP BY LI.PP_DATE";
            //    Record2 = _dbContext.SqlQuery<CIPaymentSettlementModel>(AMOUNT_QUERY).FirstOrDefault();

            //    if (string.IsNullOrEmpty(Record2.PP_DATE))
            //    {
            //        Record.INVALID = "INVALID";
            //        return Record;
            //    }

            //    var CURRENCY_QUERY = $@"SELECT DISTINCT LI.CURRENCY_CODE AS CURRENCY  FROM  LC_ITEM LI,LC_LOC LL,LC_INVOICE LINV WHERE  LL.LC_TRACK_NO = LI.LC_TRACK_NO 
            //                            AND LL.LC_TRACK_NO = LINV.LC_TRACK_NO
            //                            AND  LINV.INVOICE_CODE = '{InvoiceCode}'";
            //    Record = _dbContext.SqlQuery<CIPaymentSettlementModel>(CURRENCY_QUERY).FirstOrDefault();
            //    Record.pterm = "USANCE";
            //    Record.PP_DATE = Record2.PP_DATE.ToString();
            //    Record.CI_AMOUNT = Record2.CI_AMOUNT.ToString();
            //}
            //return Record;

            if (result == "202" || result == "122")
            {
                var AMOUNT_QUERY = $@"SELECT TO_CHAR(SUM(AMOUNT)) AS CI_AMOUNT FROM LC_INVOICE LI WHERE INVOICE_CODE = '{InvoiceCode}'";
                Record2 = _dbContext.SqlQuery<CIPaymentSettlementModel>(AMOUNT_QUERY).FirstOrDefault();


                var CURRENCY_QUERY = $@"SELECT DISTINCT LI.CURRENCY_CODE AS CURRENCY  FROM  LC_ITEM LI,LC_LOC LL,LC_INVOICE LINV WHERE  LL.LC_TRACK_NO = LI.LC_TRACK_NO 
                                        AND LL.LC_TRACK_NO = LINV.LC_TRACK_NO
                                        AND  LINV.INVOICE_CODE = '{InvoiceCode}'";

                Record = _dbContext.SqlQuery<CIPaymentSettlementModel>(CURRENCY_QUERY).FirstOrDefault();

                Record.CI_AMOUNT = Record2.CI_AMOUNT.ToString();
                Record.pterm = "ATSIGHT";
            }

            if (result == "121" || result == "201")
            {

                var AMOUNT_QUERY = $@"SELECT TO_CHAR(SUM(AMOUNT)) AS CI_AMOUNT, TO_CHAR(LI.PP_DATE,'MM-DD-YYYY') AS PP_DATE FROM LC_INVOICE LI WHERE INVOICE_CODE = '{InvoiceCode}' GROUP BY LI.PP_DATE";
                Record2 = _dbContext.SqlQuery<CIPaymentSettlementModel>(AMOUNT_QUERY).FirstOrDefault();

                if (string.IsNullOrEmpty(Record2.PP_DATE))
                {
                    Record.INVALID = "INVALID";
                    return Record;
                }

                var CURRENCY_QUERY = $@"SELECT DISTINCT LI.CURRENCY_CODE AS CURRENCY  FROM  LC_ITEM LI,LC_LOC LL,LC_INVOICE LINV WHERE  LL.LC_TRACK_NO = LI.LC_TRACK_NO 
                                        AND LL.LC_TRACK_NO = LINV.LC_TRACK_NO
                                        AND  LINV.INVOICE_CODE = '{InvoiceCode}'";
                Record = _dbContext.SqlQuery<CIPaymentSettlementModel>(CURRENCY_QUERY).FirstOrDefault();
                Record.pterm = "USANCE";
                Record.PP_DATE = Record2.PP_DATE.ToString();
                Record.CI_AMOUNT = Record2.CI_AMOUNT.ToString();
            }
            return Record;

        }

        //public CIPaymentSettlementModel LoadCIInfo(string InvoiceCode)
        //{
        //    CIPaymentSettlementModel Record = new CIPaymentSettlementModel();
        //     var PTERMS_QUERY = $@"select TO_CHAR(LL.PTERMS_CODE) from LC_LOC LL, LC_INVOICE LI  
        //                        WHERE LI.LC_TRACK_NO = LL.LC_TRACK_NO AND LI.INVOICE_CODE =  '{InvoiceCode}'";
        //    var result = _dbContext.SqlQuery<string>(PTERMS_QUERY).FirstOrDefault();
        //    if (result == "121")
        //    {
        //        var AMOUNT_QUERY = $@"SELECT TO_CHAR(SUM(AMOUNT)) AS CI_AMOUNT FROM LC_INVOICE LI WHERE INVOICE_CODE = '{InvoiceCode}'";
        //        Record = _dbContext.SqlQuery<CIPaymentSettlementModel>(AMOUNT_QUERY).FirstOrDefault();
        //        //Record.CI_AMOUNT = Record.CI_AMOUNT.ToString();
        //        Record.pterm = "ATSIGHT";
        //    }

        //    if (result == "122")
        //    {

        //        var AMOUNT_QUERY = $@"SELECT TO_CHAR(SUM(AMOUNT)) AS CI_AMOUNT, TO_CHAR(LI.PP_DATE,'MM-DD-YYYY') AS PP_DATE FROM LC_INVOICE LI WHERE INVOICE_CODE = '{InvoiceCode}' GROUP BY LI.PP_DATE";
        //        Record = _dbContext.SqlQuery<CIPaymentSettlementModel>(AMOUNT_QUERY).FirstOrDefault();

        //        if (string.IsNullOrEmpty(Record.PP_DATE))
        //        {
        //            Record.INVALID = "INVALID";
        //            return Record;
        //        }

        //        Record.pterm = "USANCE";
        //        Record.PP_DATE = Record.PP_DATE.ToString();
        //        //Record.CI_AMOUNT = Record.CI_AMOUNT.ToString();
        //    }
        //    return Record;

        //}
    }
}
