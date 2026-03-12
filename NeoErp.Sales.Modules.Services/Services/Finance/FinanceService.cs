using NeoErp.Core.Domain;
using NeoErp.Core.Models;
using NeoErp.Sales.Modules.Services.Models.FinanceReport;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Services.Finance
{
    public class FinanceService : IFinanceService
    {
        private readonly NeoErpCoreEntity _objectEntity;
        public FinanceService(NeoErpCoreEntity objectEntity)
        {
            _objectEntity = objectEntity;
        }
        public IList<MovementAnalysisModel> GetMovementAnalysis(ReportFiltersModel model, User userInfo)
        {

            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["NeoErpCoreEntity"].ConnectionString;

            using (OracleConnection objConn = new OracleConnection("DATA SOURCE=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = 192.168.1.65)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = VIJAY))); PASSWORD=IMS7778;USER ID=IMS7778"))
            {
                //using (OracleConnection objConn = new OracleConnection(connectionString))
                //{
                //OracleCommand objCmd = new OracleCommand();

                //objCmd.Connection = objConn;
                //objCmd.CommandText = "spRefreshMaterializedView";
                //objCmd.CommandType = CommandType.StoredProcedure;
                //try
                //{
                //    objConn.Open();
                //    var res = objCmd.ExecuteNonQuery();
                //}
                //catch (Exception ex)
                //{
                //    throw ex;
                //}
                //finally
                //{
                //    objConn.Close();
                //}
                OracleCommand objCmd = new OracleCommand();

                objCmd.Connection = objConn;
                objCmd.CommandText = $@"BEGIN 
                                        dbms_mview.refresh('M$V_MOVEMENT_ANALYSIS', '?', '', true, false, 0, 0, 0, false, false);
                                        END; ";
                objCmd.CommandType = CommandType.Text;
                try
                {
                    objConn.Open();
                    var res = objCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    objConn.Close();
                }

            }
            //var movementAnalysisData = new List<MovementAnalysisModel>();
            List<MovementAnalysisModel> movementAnalysisData = null;
            var Query = string.Empty;
            var companyCode = string.Empty;
            var count = 0;

            //try
            //{
            //    ++count;

            //    for (var i = 0; i < 1; i++)
            //    {
            //        var refreshQuery = $@"BEGIN
            //                            dbms_mview.refresh('M$V_MOVEMENT_ANALYSIS', 'C', '', true, false, 0, 0, 0, false, false);
            //                          END;";
            //        var result = this._objectEntity.ExecuteSqlCommand(refreshQuery);
            //    }

            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}

            foreach (var company in model.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }
            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            try
            {


                if (model.FromDate != model.ToDate)
                {
                    movementAnalysisData = new List<MovementAnalysisModel>();

                    Query = @"SELECT CUSTOMER_CODE CustomerCode
								,CUSTOMER_EDESC CustomerName
								,VOUCHER_NO VoucherNumber
								,MANUAL_NO ManaulNumber
								,VOUCHER_DATE VoucherDate
								,CREDIT_LIMIT CreditLimit
								,CREDIT_DAYS CreditDays
								,DUE_DAYS DueDays
								,COMPANY_CODE CompanyCode
								,TO_NUMBER(PENDING_AMT) PendingAmount
								,TO_NUMBER(SALES_AMT) SalesAmount
								,TO_NUMBER(REC_AMT) RecAmount
								,TO_NUMBER(BALANCE) Balance
							FROM M$V_MOVEMENT_ANALYSIS WHERE COMPANY_CODE IN ({0}) 
                            AND CUSTOMER_CODE IN ('{1}') 
                            AND VOUCHER_DATE>=TO_DATE ('{2}','YYYY-MM-DD') AND VOUCHER_DATE<=TO_DATE ('{3}','YYYY-MM-DD')";
                    Query = string.Format(Query, companyCode, string.Join("','", model.CustomerFilter), model.FromDate, model.ToDate);
                }
                else
                {
                    movementAnalysisData = new List<MovementAnalysisModel>();
                    Query = @"SELECT CUSTOMER_CODE CustomerCode
								,CUSTOMER_EDESC CustomerName
								,VOUCHER_NO VoucherNumber
								,MANUAL_NO ManaulNumber
								,VOUCHER_DATE VoucherDate
								,CREDIT_LIMIT CreditLimit
								,CREDIT_DAYS CreditDays
								,DUE_DAYS DueDays
								,COMPANY_CODE CompanyCode
								,TO_NUMBER(PENDING_AMT) PendingAmount
								,TO_NUMBER(SALES_AMT) SalesAmount
								,TO_NUMBER(REC_AMT) RecAmount
								,TO_NUMBER(BALANCE) Balance
							FROM M$V_MOVEMENT_ANALYSIS WHERE COMPANY_CODE IN ({0}) 
                            AND CUSTOMER_CODE IN ('{1}')";

                    Query = string.Format(Query, companyCode, string.Join("','", model.CustomerFilter));

                }

                var movementAnalysisListData = this._objectEntity.SqlQuery<MovementAnalysisModel>(Query).ToList();

                return movementAnalysisListData;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


    }
}
