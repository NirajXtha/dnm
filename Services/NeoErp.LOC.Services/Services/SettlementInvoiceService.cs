using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using NeoErp.Core;
using NeoErp.Data;
using NeoErp.LOC.Services.Models;

namespace NeoErp.LOC.Services.Services
{
    public class SettlementInvoiceService : ISettlementInvoiceService
    {
        private IDbContext _dbContext;
        private IWorkContext _workcontext;
        public SettlementInvoiceService(IDbContext _dbContext, IWorkContext _iWorkContext)
        {
            this._dbContext = _dbContext;
            this._workcontext = _iWorkContext;
        }
        public List<SettlementInvoiceModel> GetAllSettlementInvoices()
        {
            try
            {
                const string sqlquery = @"SELECT lis.SETTLEMENT_CODE,
                                   TO_CHAR(lis.SETTLEMENT_DATE, 'dd/mm/yyyy') AS SETTLEMENT_DATE,
                                  lis.INVOICE_CODE,
                                   li.INVOICE_NUMBER,
                                   lis.SWIFT_COPY,
                                   lis.REMARKS,
                                   lis.ATTACH_DOC
                              FROM LC_INVOICE_SETTLEMENT lis, LC_INVOICE li
                             WHERE lis.DELETED_FLAG != 'Y' AND LI.INVOICE_CODE = lis.INVOICE_CODE
                             ORDER BY lis.SETTLEMENT_CODE";
                var result = _dbContext.SqlQuery<SettlementInvoiceModel>(sqlquery).ToList();
                List<SettlementInvoiceModel> newlist = new List<SettlementInvoiceModel>();
                foreach (var item in result)
                {
                    if (item.ATTACH_DOC != null)
                    {
                        if (item.ATTACH_DOC.ToString().Contains(':'))
                        {
                            item.mylist = item.ATTACH_DOC.Split(':');
                            item.ATTACH_DOC = null;
                        }
                        else
                        {
                            item.ATTACH_DOC = item.ATTACH_DOC;
                        }
                    }
                    newlist.Add(item);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SettlementInvoiceModel> GetSettlementInvoicesByID(string id)
        {
            try
            {
                var sqlquery = "SELECT SETTLEMENT_CODE, TO_CHAR(SETTLEMENT_DATE,'dd/mm/yyyy') as SETTLEMENT_DATE, INVOICE_CODE, SWIFT_COPY, REMARKS, ATTACH_DOC  FROM LC_INVOICE_SETTLEMENT WHERE DELETED_FLAG ='Y' AND INVOICE_CODE ='" + id +"'";
                var result = _dbContext.SqlQuery<SettlementInvoiceModel>(sqlquery).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SettlementInvocieddlModel> GetInvoice()
        {
            try
            {
                const string sqlquery = "SELECT INVOICE_CODE, INVOICE_NUMBER from LC_INVOICE";
                var result = _dbContext.SqlQuery<SettlementInvocieddlModel>(sqlquery).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<SettlementInvocieddlModel> GetInvoiceByFilter(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
                var sqlquery = string.Format(@"SELECT DISTINCT
                                    COALESCE(INVOICE_CODE,0) INVOICE_CODE,
                                    COALESCE(INVOICE_NUMBER,' ') INVOICE_NUMBER
                                    from LC_INVOICE
                                    where deleted_flag='N' AND
                                    INVOICE_CODE like '%{0}%'
                                    or upper(INVOICE_NUMBER)like '%{0}%'", filter.ToUpperInvariant());
                var result = _dbContext.SqlQuery<SettlementInvocieddlModel>(sqlquery).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
public string SaveSettlementInvoices(SettlementInvoiceModel sInvoices)
        {
            try
            {
                var nextValQuery = $@"SELECT LC_INVOICE_SEQ.nextval as SETTLEMENT_NEXT_CODE FROM DUAL";
                var id = _dbContext.SqlQuery<SettlementInvoiceModel>(nextValQuery).ToList().FirstOrDefault();
                var insertQuery =
                    $@"INSERT INTO LC_INVOICE_SETTLEMENT(SETTLEMENT_CODE, INVOICE_CODE, SETTLEMENT_DATE, COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG)
                        VALUES({id.SETTLEMENT_NEXT_CODE}, '{sInvoices
                        .INVOICE_CODE}', TO_DATE('{sInvoices.SETTLEMENT_DATE}', 'dd/mm/yyyy hh24:mi:ss'), '{_workcontext
                        .CurrentUserinformation.company_code}','{_workcontext.CurrentUserinformation.branch_code}','{_workcontext
                        .CurrentUserinformation.User_id}',TO_DATE('{DateTime.Now:MM/dd/yyyy/ HH:mm:ss}', 'mm/dd/yyyy hh24:mi:ss'),'{'N'}')";
                var result = _dbContext.ExecuteSqlCommand(insertQuery);
                return id.SETTLEMENT_NEXT_CODE.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string UpdateSettlementInvoices(SettlementInvoiceModel sInvoices)
        {
            try
            {
                var sqlquery = "UPDATE LC_INVOICE_SETTLEMENT SET SETTLEMENT_DATE =TO_DATE('" + sInvoices.SETTLEMENT_DATE + "','dd/mm/yyyy'), INVOICE_CODE ='" + sInvoices.INVOICE_CODE + "' WHERE SETTLEMENT_CODE='" + sInvoices.SETTLEMENT_CODE + "'";
                var result = _dbContext.ExecuteSqlCommand(sqlquery);
                return result.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string DeleteSettlementInvoices(string pfiCode)
        {
            try
            {
                var sqlquery = "UPDATE LC_INVOICE_SETTLEMENT SET DELETED_FLAG ='Y' WHERE SETTLEMENT_CODE = " + pfiCode ;
                var result = _dbContext.ExecuteSqlCommand(sqlquery);
                return result.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateFile(LcUploadFileModels lcimagedetail)
        {
            throw new NotImplementedException();
        }

        public void RemoveFile(LcUploadFileModels lcimagedetail)
        {
            throw new NotImplementedException();
        }
        //public void UpdateFile(LcUploadFileModels lcimagedetail)
        //{
        //    var sqlquery = "SELECT ATTACH_DOC from LC_INVOICE_SETTLEMENT where SETTLEMENT_CODE ='" + lcimagedetail.Code + "'";
        //    var result = _dbContext.SqlQuery<LcEntryModels>(sqlquery).FirstOrDefault();
        //    if (result.ATTACH_DOC != null)
        //    {
        //        var dbpath = result.ATTACH_DOC;
        //        dbpath = dbpath + ":" + lcimagedetail.Path;
        //        var query =
        //            $@"UPDATE LC_INVOICE_SETTLEMENT SET ATTACH_DOC  = '{dbpath}' WHERE SETTLEMENT_CODE IN ({lcimagedetail.Code})";
        //        var rowCount = _dbContext.ExecuteSqlCommand(query);
        //    }
        //    else
        //    {
        //        var query =
        //            $@"UPDATE LC_INVOICE_SETTLEMENT SET ATTACH_DOC  = '{lcimagedetail.Path}' WHERE SETTLEMENT_CODE IN ({lcimagedetail
        //                .Code})";
        //        var rowCount = _dbContext.ExecuteSqlCommand(query);
        //    }
        //}
        //private static string ConvertStringArrayToString(IEnumerable<string> array)
        //{
        //    var builder = new StringBuilder();
        //    foreach (var value in array)
        //    {
        //        builder.Append(value);
        //        builder.Append(':');
        //    }
        //    return builder.ToString();
        //}
        //public void RemoveFile(LcUploadFileModels lcFile)
        //{
        //    var sqlquery = "select ATTACH_DOC from LC_INVOICE_SETTLEMENT where SETTLEMENT_CODE ='" + lcFile.Code + "'";
        //    var result = _dbContext.SqlQuery<LcEntryModels>(sqlquery).FirstOrDefault();
        //    if (result.ATTACH_DOC != null && result.ATTACH_DOC.ToString().Contains(':'))
        //    {
        //        result.mylist = result.ATTACH_DOC.Split(':');
        //        result.mylist = result.mylist.Where(s => s != lcFile.Path).ToArray();
        //        result.ATTACH_DOC = null;
        //        var paths = SettlementInvoiceService.ConvertStringArrayToString(result.mylist);
        //        paths = paths.Remove(paths.Length - 1);
        //        string query = $@"UPDATE LC_INVOICE_SETTLEMENT SET ATTACH_DOC  = '{paths}' WHERE SETTLEMENT_CODE IN ({lcFile.Code})";
        //        var rowCount = _dbContext.ExecuteSqlCommand(query);
        //    }
        //    else
        //    {
        //        string query = $@"UPDATE LC_INVOICE_SETTLEMENT SET ATTACH_DOC  = '{""}' WHERE SETTLEMENT_CODE IN ({lcFile.Code})";
        //        var rowCount = _dbContext.ExecuteSqlCommand(query);
        //    }
        //}
    }
}