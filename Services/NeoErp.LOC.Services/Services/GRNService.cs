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
    public class GRNService : IGRNService
    {
        private IDbContext _dbContext;
        private IWorkContext _workcontext;
        public GRNService(IDbContext dbContext, IWorkContext iWorkContext)
        {
            this._workcontext = iWorkContext;
            this._dbContext = dbContext;
        }
        public List<LcGrnModel> getAllGRN()
        {
            var sqlquerys = $@"select DISTINCT TO_CHAR(LG.GRN_CODE) AS GRN_CODE,TO_CHAR(LG.GRN_NO) AS GRN_NO,TO_CHAR(LG.LC_TRACK_NO) AS LC_TRACK_NO,TO_CHAR(LG.INVOICE_CODE) AS INVOICE_CODE,TO_CHAR(LI.INVOICE_NUMBER) AS INVOICE_NUMBER,TO_CHAR(LG.CI_AMOUNT) AS CI_AMOUNT, TO_CHAR(LG.CURRENCY) AS CURRENCY, TO_CHAR(LG.EXCHANGE_RATE) AS  EXCHANGE_RATE,TO_CHAR(LG.DERIVED_TOTAL_AMOUNT) AS DERIVED_TOTAL_AMOUNT,TO_CHAR(LG.GRN_DATE,'mm-dd-yyyy') AS  GRN_DATE,TO_CHAR(LG.PP_RECEIEVE_DATE,'mm-dd-yyyy') AS  PP_RECEIEVE_DATE , TO_CHAR(LG.PP_NO) AS PP_NO  from LC_GRN LG
                               INNER JOIN LC_INVOICE LI ON LI.INVOICE_CODE=  LG.INVOICE_CODE  WHERE LG.BRANCH_CODE ='{_workcontext.CurrentUserinformation.branch_code}' AND LG.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}' ORDER BY LG.GRN_CODE";
            var result = _dbContext.SqlQuery<LcGrnModel>(sqlquerys).ToList();
            return result;
        }

        public string AddUpdateGRN(LcGrnModel lcGrnModel)
        {

            if (!string.IsNullOrEmpty(lcGrnModel.GRN_CODE))
            {
                //update query

                string UPDATEQUERY = $@"UPDATE LC_GRN
                                        SET
                                        GRN_NO='{lcGrnModel.GRN_NO}',
                                        CI_AMOUNT='{lcGrnModel.CI_AMOUNT}',
                                        CURRENCY='{lcGrnModel.CURRENCY}',
                                        EXCHANGE_RATE='{lcGrnModel.EXCHANGE_RATE}',
                                        DERIVED_TOTAL_AMOUNT='{lcGrnModel.DERIVED_TOTAL_AMOUNT}',
                                        GRN_DATE=TO_DATE('{lcGrnModel.GRN_DATE}','MM/dd/yyyy'),
                                        PP_RECEIEVE_DATE=TO_DATE('{lcGrnModel.PP_RECEIEVE_DATE}','MM/dd/yyyy'),
                                        PP_NO='{lcGrnModel.PP_NO}'
                                        WHERE GRN_CODE='{lcGrnModel.GRN_CODE}' and BRANCH_CODE ='{_workcontext.CurrentUserinformation.branch_code}' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
                _dbContext.ExecuteSqlCommand(UPDATEQUERY);

                string updateLcInvoicePP = $@"UPDATE LC_INVOICE
                                        SET 
                                        PP_DATE=TO_DATE('{lcGrnModel.PP_RECEIEVE_DATE}','MM/dd/yyyy'),
                                        PP_NO='{lcGrnModel.PP_NO}'
                                        WHERE INVOICE_CODE='{lcGrnModel.INVOICE_CODE}' and LC_TRACK_NO='{lcGrnModel.LC_TRACK_NO}' and BRANCH_CODE ='{_workcontext.CurrentUserinformation.branch_code}' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
                _dbContext.ExecuteSqlCommand(updateLcInvoicePP);
                return "Updated";

            }
            else
            {
                string countquery = $@"select COUNT(LG.INVOICE_CODE) from LC_GRN LG WHERE LG.INVOICE_CODE='{lcGrnModel.INVOICE_CODE}' and LG.BRANCH_CODE ='{_workcontext.CurrentUserinformation.branch_code}' AND LG.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
                var rowCount = _dbContext.SqlQuery<int>(countquery).FirstOrDefault();
                if (rowCount > 0)
                {
                    return "Existed";
                }
                string lc_track_number_query = $@"select TO_CHAR(I.LC_TRACK_NO) as LC_TRACK_NO from lc_invoice I where I.INVOICE_CODE ='{lcGrnModel.INVOICE_CODE}' and I.BRANCH_CODE ='{_workcontext.CurrentUserinformation.branch_code}' AND I.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
                lcGrnModel.LC_TRACK_NO = _dbContext.SqlQuery<string>(lc_track_number_query).FirstOrDefault();

                string MAXPSCODE_query = $@"select  TO_CHAR(NVL(MAX(GRN_CODE+1),1)) AS GRN_CODE from LC_GRN LG WHERE LG.BRANCH_CODE ='{_workcontext.CurrentUserinformation.branch_code}' AND LG.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
                lcGrnModel.GRN_CODE = _dbContext.SqlQuery<string>(MAXPSCODE_query).FirstOrDefault();


                string insertquery = $@"INSERT INTO LC_GRN(GRN_CODE,LC_TRACK_NO,INVOICE_CODE,GRN_NO,CI_AMOUNT,CURRENCY,EXCHANGE_RATE,DERIVED_TOTAL_AMOUNT,PP_RECEIEVE_DATE,PP_NO,GRN_DATE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG)
                                        Values('{lcGrnModel.GRN_CODE}','{ lcGrnModel.LC_TRACK_NO}','{lcGrnModel.INVOICE_CODE}','{lcGrnModel.GRN_NO}','{lcGrnModel.CI_AMOUNT}','{lcGrnModel.CURRENCY}','{lcGrnModel.EXCHANGE_RATE}','{lcGrnModel.DERIVED_TOTAL_AMOUNT}',TO_DATE('{lcGrnModel.PP_RECEIEVE_DATE}','MM/dd/yyyy'),'{lcGrnModel.PP_NO}',TO_DATE('{lcGrnModel.GRN_DATE}','MM/dd/yyyy')  ,'{_workcontext.CurrentUserinformation.company_code}','{_workcontext.CurrentUserinformation.branch_code}',
                                        '{_workcontext.CurrentUserinformation.User_id}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'{'N'}')";
                _dbContext.ExecuteSqlCommand(insertquery);

                string updateLcInvoicePP = $@"UPDATE LC_INVOICE
                                        SET 
                                        PP_DATE=TO_DATE('{lcGrnModel.PP_RECEIEVE_DATE}','MM/dd/yyyy'),
                                        PP_NO='{lcGrnModel.PP_NO}'
                                        WHERE INVOICE_CODE='{lcGrnModel.INVOICE_CODE}' and LC_TRACK_NO='{lcGrnModel.LC_TRACK_NO}' and BRANCH_CODE ='{_workcontext.CurrentUserinformation.branch_code}' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
                _dbContext.ExecuteSqlCommand(updateLcInvoicePP);
                _dbContext.SaveChanges();
                return "Inserted";
            }

        }


        //public LcGrnModel LoadCIInfo(string InvoiceCode, string PPDate)
        //{
        //    LcGrnModel Record = new LcGrnModel();
        //    LcGrnModel Record2 = new LcGrnModel();
        //    var CURRENCY_QUERY = $@"SELECT TO_CHAR(EXCHANGE_RATE) AS EXCHANGE_RATE,TO_CHAR(CURRENCY_CODE) AS CURRENCY FROM EXCHANGE_DETAIL_SETUP
        //                        WHERE EXCHANGE_DATE =(SELECT MAX(EXCHANGE_DATE) FROM EXCHANGE_DETAIL_SETUP
        //                        WHERE EXCHANGE_DATE <=TO_DATE('{PPDate}','MM-DD-YYYY') 
        //                        AND CURRENCY_CODE IN (SELECT CURRENCY_CODE FROM LC_PERFOMA_INVOICE LPI WHERE LPI.PINVOICE_CODE IN(SELECT PINVOICE_CODE FROM LC_LOC LL, LC_INVOICE LI WHERE LL.LC_TRACK_NO = LI.LC_TRACK_NO AND LI.INVOICE_CODE = '19')))
        //                        AND CURRENCY_CODE IN(SELECT CURRENCY_CODE FROM LC_PERFOMA_INVOICE LPI WHERE LPI.PINVOICE_CODE IN(SELECT PINVOICE_CODE FROM LC_LOC LL, LC_INVOICE LI WHERE LL.LC_TRACK_NO = LI.LC_TRACK_NO AND LI.INVOICE_CODE = '19'))";
        //    Record = _dbContext.SqlQuery<LcGrnModel>(CURRENCY_QUERY).FirstOrDefault();

        //    var AMOUNT_QUERY = $@"SELECT TO_CHAR(SUM(AMOUNT)) AS CI_AMOUNT FROM LC_INVOICE LI WHERE INVOICE_CODE = '{InvoiceCode}'";
        //    Record2 = _dbContext.SqlQuery<LcGrnModel>(AMOUNT_QUERY).FirstOrDefault();
        //    Record.CI_AMOUNT = Record2.CI_AMOUNT.ToString();
        //    return Record;

        //}

        public LcGrnModel LoadCIInfo(string InvoiceCode)
        {
            LcGrnModel Record = new LcGrnModel();
            LcGrnModel Record2 = new LcGrnModel();

            var AMOUNT_QUERY = $@"SELECT TO_CHAR(SUM(AMOUNT)) AS CI_AMOUNT FROM LC_INVOICE LI WHERE INVOICE_CODE = '{InvoiceCode}'";
            Record2 = _dbContext.SqlQuery<LcGrnModel>(AMOUNT_QUERY).FirstOrDefault();
            
            var CURRENCY_QUERY = $@"SELECT TO_CHAR(CURRENCY_CODE) AS CURRENCY FROM EXCHANGE_DETAIL_SETUP
                                    WHERE CURRENCY_CODE IN (SELECT CURRENCY_CODE FROM LC_PERFOMA_INVOICE LPI WHERE LPI.PINVOICE_CODE IN(SELECT PINVOICE_CODE FROM LC_LOC LL, LC_INVOICE LI WHERE LL.LC_TRACK_NO = LI.LC_TRACK_NO AND LI.INVOICE_CODE =  '{InvoiceCode}'))";
                              
            Record = _dbContext.SqlQuery<LcGrnModel>(CURRENCY_QUERY).FirstOrDefault();

           // Record.PP_DATE = Record2.PP_DATE.ToString();
            Record.CI_AMOUNT = Record2.CI_AMOUNT.ToString();

            return Record;

        }
    }
}
