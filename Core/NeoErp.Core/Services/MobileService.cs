using NeoErp.Core.Models;
using NeoErp.Core.Models.CustomModels;
using NeoErp.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml.Linq;

namespace NeoErp.Core.Services
{
    public class MobileService : IMobileService
    {
        private IDbContext _dbContext;

        public MobileService(IDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public IEnumerable<MobileViewVoucherModel> GetVoucherDetails(int userId, string moduleCode,string append,int sessionRowId)
        {
            var voucherRawData = this.GetVoucher(userId,moduleCode, append, sessionRowId);

            if (voucherRawData == null || voucherRawData.Count() == 0)
            {
                return new List<MobileViewVoucherModel>();
            }

            return voucherRawData.ToList().Select(q => new MobileViewVoucherModel() {
                FORM_CODE = q.FORM_CODE,
                ACC_CODE = q.ACC_CODE,
                BRANCH_DESCRIPTION = q.BRANCH_DESCRIPTION,
                CREATED_BY = q.CREATED_BY,
                FORM_DESCRIPTION = q.FORM_DESCRIPTION,
                FORM_TYPE = q.FORM_TYPE,
                MITI = !string.IsNullOrEmpty(q.Miti) ? string.Join("-", q.Miti.Split(new char[] { '-' }).Reverse()):string.Empty,
                LEDGER_TITLE = q.LEDGER_TITLE,
                MODULE_CODE = q.MODULE_CODE,
                REMARKS = !string.IsNullOrEmpty(q.REMARKS)?q.REMARKS:string.Empty,
                SESSION_ROWID = q.SESSION_ROWID,
                VOUCHER_AMOUNT = q.VOUCHER_AMOUNT??0,
                VOUCHER_DATE = q.VOUCHER_DATE.ToString("dd-MM-yyyy"),
                VOUCHER_NO = q.VOUCHER_NO
            });

            //return voucherRawData.GroupBy(q => new { q.FORM_CODE, q.MODULE_CODE, q.FORM_DESCRIPTION },
            //    (key, group) => new MobileViewVoucherModel()
            //    {
            //        FORM_CODE = key.FORM_CODE,
            //        FORM_DESCRIPTION = key.FORM_DESCRIPTION,
            //        MODULE_CODE = key.MODULE_CODE,
            //        VOUCHER_DETAIL = voucherRawData.Where(q => q.FORM_CODE == key.FORM_CODE)
            //        .Select(x => new MobileViewVoucherModel.MobileViewVoucherDetail()
            //        {
            //            VOUCHER_NO = x.VOUCHER_NO,
            //            CREATED_BY = x.CREATED_BY,
            //            MITI = string.Join("-", x.Miti.Split(new char[] { '-' }).Reverse()),
            //            VOUCHER_AMOUNT = x.VOUCHER_AMOUNT,
            //            VOUCHER_DATE = x.VOUCHER_DATE.ToString("dd-MM-yyyy"),
            //        })
            //    });
        }

        protected IEnumerable<MobileDataVoucherModel> GetVoucher(int userId, string moduleCode = "",  string append = "top", int sessionRowId = 0)
        {


            if(!string.IsNullOrEmpty(moduleCode) && moduleCode != "00")
            {
                var permission = this.GetModulePermission(userId);

                if (permission.Count() == 0 || !permission.Any(q => q == moduleCode))
                    return new List<MobileDataVoucherModel>();
            }

           string query = string.Format(@"SELECT a.*, rownum r__
                FROM(SELECT  A.VOUCHER_NO , A.VOUCHER_AMOUNT , A.FORM_CODE , A.CREATED_BY , A.VOUCHER_DATE , BS_DATE(A.VOUCHER_DATE) Miti, B.FORM_EDESC FORM_DESCRIPTION,B.FORM_TYPE, C.REMARKS , C.ACC_CODE , D.ACC_EDESC LEDGER_TITLE, E.BRANCH_EDESC BRANCH_DESCRIPTION, TO_NUMBER(C.SESSION_ROWID) SESSION_ROWID, B.MODULE_CODE
                FROM MASTER_TRANSACTION A, FORM_SETUP B, FA_DOUBLE_VOUCHER C, FA_CHART_OF_ACCOUNTS_SETUP D, FA_BRANCH_SETUP E
                WHERE A.FORM_CODE = B.FORM_CODE
               AND A.FORM_CODE IN(SELECT FORM_CODE FROM SC_FORM_CONTROL WHERE USER_NO={0} AND READ_FLAG='Y')
                AND A.VOUCHER_NO = C.VOUCHER_NO
                AND C.SERIAL_NO = 1
                AND C.ACC_CODE = D.ACC_CODE
               AND A.CHECKED_BY IS NULL 
                AND A.BRANCH_CODE = E.BRANCH_CODE
                AND A.BRANCH_CODE = C.BRANCH_CODE
                AND A.COMPANY_CODE = B.COMPANY_CODE
                AND A.COMPANY_CODE = C.COMPANY_CODE
                AND A.COMPANY_CODE = D.COMPANY_CODE
                AND A.COMPANY_CODE = E.COMPANY_CODE", userId);
           
                if(!string.IsNullOrEmpty(moduleCode) && moduleCode !="00")
                {
                    query += string.Format(" AND B.MODULE_CODE = '{0}'", moduleCode);
                }

                if (append == LoadMode.top.ToString())
                {
                    query += string.Format(" AND TO_NUMBER(C.SESSION_ROWID) > {0}", sessionRowId);
                }
                else if (append == LoadMode.bottom.ToString())
                {
                    query += string.Format(" AND TO_NUMBER(C.SESSION_ROWID) < {0}", sessionRowId);
                }
                

                query += " Order By SESSION_ROWID DESC) a WHERE rownum < 50";

                //string query = @"SELECT * FROM (
                //    SELECT ROWNUM, B.MODULE_CODE as ModuleCode, A.VOUCHER_NO as VoucherNo, A.VOUCHER_DATE as VoucherDate, BS_DATE(A.VOUCHER_DATE) Miti, A.FORM_CODE as FormCode, B.FORM_EDESC as FormDescription, A.VOUCHER_AMOUNT as VoucherAmount, A.CREATED_BY as CreatedBy FROM MASTER_TRANSACTION A, FORM_SETUP B
                //    WHERE A.FORM_CODE = B.FORM_CODE
                //    AND A.COMPANY_CODE = B.COMPANY_CODE
                //    AND A.DELETED_FLAG='N'
                //    AND (A.CHECKED_BY = '' OR A.CHECKED_BY IS NULL) AND B.MODULE_CODE='01')
                //    WHERE ROWNUM < 100";
            return this._dbContext.SqlQuery<MobileDataVoucherModel>(query);
        }

        public IEnumerable<MobileDataVoucherModel> GetVoucherWithFlag(int userId, string companyCode,string branchCode, string moduleCode = "", string append = "top", int sessionRowId = 0)
        {


            //if (!string.IsNullOrEmpty(moduleCode) && moduleCode != "00")
            //{
            //    var permission = this.GetModulePermission(userId);

            //    if (permission.Count() == 0 || !permission.Any(q => q == moduleCode))
            //        return new List<MobileDataVoucherModel>();
            //}

            //string query = $@"SELECT a.*, rownum r__
            //    FROM(SELECT DISTINCT A.VOUCHER_NO ,A.CHECKED_BY,A. AUTHORISED_BY,A.POSTED_BY ,SC.CHECK_FLAG,SC.POST_FLAG,SC.VERIFY_FLAG,
            //     A.VOUCHER_AMOUNT , A.FORM_CODE , A.CREATED_BY , A.VOUCHER_DATE , BS_DATE(A.VOUCHER_DATE) Miti, B.FORM_EDESC FORM_DESCRIPTION,B.FORM_TYPE,
            //      C.REMARKS , C.ACC_CODE , D.ACC_EDESC LEDGER_TITLE, E.BRANCH_EDESC BRANCH_DESCRIPTION, TO_NUMBER(C.SESSION_ROWID) SESSION_ROWID, B.MODULE_CODE
            //    FROM MASTER_TRANSACTION A, FORM_SETUP B, FA_DOUBLE_VOUCHER C, FA_CHART_OF_ACCOUNTS_SETUP D, FA_BRANCH_SETUP E,SC_FORM_CONTROL SC
            //    WHERE A.FORM_CODE = B.FORM_CODE
            //         AND A.FORM_CODE=SC.FORM_CODE
            //    AND A.COMPANY_CODE=SC.COMPANY_CODE
            //    AND SC.USER_NO =1032
            //    AND A.FORM_CODE IN(SELECT FORM_CODE FROM SC_FORM_CONTROL WHERE USER_NO=1032 AND (CHECK_FLAG='Y' ))
            //    AND A.VOUCHER_NO = C.VOUCHER_NO
            //    AND C.SERIAL_NO = 1
            //    AND C.ACC_CODE = D.ACC_CODE
            //    AND (A.CHECKED_BY IS NULL) 
            //    AND A.BRANCH_CODE = E.BRANCH_CODE
            //    AND A.BRANCH_CODE = C.BRANCH_CODE
            //    AND A.COMPANY_CODE = B.COMPANY_CODE
            //    AND A.COMPANY_CODE = C.COMPANY_CODE
            //    AND A.COMPANY_CODE = D.COMPANY_CODE
            //    AND A.COMPANY_CODE = E.COMPANY_CODE AND TO_NUMBER(C.SESSION_ROWID) > 0 Order By SESSION_ROWID DESC) a WHERE rownum < 500
            //    UNION ALL
            //    SELECT a.*, rownum r__
            //    FROM(SELECT DISTINCT A.VOUCHER_NO ,A.CHECKED_BY,A. AUTHORISED_BY,A.POSTED_BY ,SC.CHECK_FLAG,SC.POST_FLAG,SC.VERIFY_FLAG,
            //     A.VOUCHER_AMOUNT , A.FORM_CODE , A.CREATED_BY , A.VOUCHER_DATE , BS_DATE(A.VOUCHER_DATE) Miti, B.FORM_EDESC FORM_DESCRIPTION,B.FORM_TYPE,
            //      C.REMARKS , C.ACC_CODE , D.ACC_EDESC LEDGER_TITLE, E.BRANCH_EDESC BRANCH_DESCRIPTION, TO_NUMBER(C.SESSION_ROWID) SESSION_ROWID, B.MODULE_CODE
            //    FROM MASTER_TRANSACTION A, FORM_SETUP B, FA_DOUBLE_VOUCHER C, FA_CHART_OF_ACCOUNTS_SETUP D, FA_BRANCH_SETUP E,SC_FORM_CONTROL SC
            //    WHERE A.FORM_CODE = B.FORM_CODE
            //         AND A.FORM_CODE=SC.FORM_CODE
            //    AND A.COMPANY_CODE=SC.COMPANY_CODE
            //    AND SC.USER_NO =1032
            //    AND A.FORM_CODE IN(SELECT FORM_CODE FROM SC_FORM_CONTROL WHERE USER_NO=1032 AND (VERIFY_FLAG='Y'))
            //    AND A.VOUCHER_NO = C.VOUCHER_NO
            //    AND C.SERIAL_NO = 1
            //    AND C.ACC_CODE = D.ACC_CODE
            //    AND (A.CHECKED_BY IS NOT NULL AND A.AUTHORISED_BY IS NULL) 
            //    AND A.BRANCH_CODE = E.BRANCH_CODE
            //    AND A.BRANCH_CODE = C.BRANCH_CODE
            //    AND A.COMPANY_CODE = B.COMPANY_CODE
            //    AND A.COMPANY_CODE = C.COMPANY_CODE
            //    AND A.COMPANY_CODE = D.COMPANY_CODE
            //    AND A.COMPANY_CODE = E.COMPANY_CODE AND TO_NUMBER(C.SESSION_ROWID) > 0 Order By SESSION_ROWID DESC) a WHERE rownum < 500";


            string query = string.Format(@"SELECT a.*, rownum r__
                    FROM(SELECT DISTINCT A.VOUCHER_NO ,A.CHECKED_BY,A. AUTHORISED_BY,A.POSTED_BY ,SC.CHECK_FLAG,SC.POST_FLAG,SC.VERIFY_FLAG,
                 nvl( A.VOUCHER_AMOUNT,0) VOUCHER_AMOUNT , A.FORM_CODE , A.CREATED_BY , A.VOUCHER_DATE , BS_DATE(A.VOUCHER_DATE) Miti, B.FORM_EDESC FORM_DESCRIPTION,B.FORM_TYPE,
                  C.REMARKS ,C.PARTICULARS, C.ACC_CODE , D.ACC_EDESC LEDGER_TITLE, E.BRANCH_EDESC BRANCH_DESCRIPTION, TO_NUMBER(A.SESSION_ROWID) SESSION_ROWID,
                B.MODULE_CODE, 'FINANCIAL' as TABLE_NAME
                FROM MASTER_TRANSACTION A, FORM_SETUP B, V$VIRTUAL_GENERAL_LEDGER C, FA_CHART_OF_ACCOUNTS_SETUP D, FA_BRANCH_SETUP E,SC_FORM_CONTROL SC
                WHERE A.FORM_CODE = B.FORM_CODE
                     AND A.FORM_CODE=SC.FORM_CODE
                AND A.COMPANY_CODE=SC.COMPANY_CODE
               -- AND SC.USER_NO ={0}
                AND A.FORM_CODE IN(SELECT FORM_CODE FROM SC_FORM_CONTROL WHERE USER_NO={0} AND (CHECK_FLAG='Y' ))
                AND A.VOUCHER_NO = C.VOUCHER_NO
                AND C.SERIAL_NO = 1
                AND C.ACC_CODE = D.ACC_CODE
                AND (A.CHECKED_BY IS NULL) 
                AND A.BRANCH_CODE = E.BRANCH_CODE
                AND A.BRANCH_CODE = C.BRANCH_CODE
                AND A.COMPANY_CODE = B.COMPANY_CODE
                AND A.COMPANY_CODE = C.COMPANY_CODE
                AND A.COMPANY_CODE = D.COMPANY_CODE
                --AND A.COMPANY_CODE = '{1}'
                ", userId, companyCode);

            if (!string.IsNullOrEmpty(branchCode))
            {
                //var b_codes = string.Empty;
                //var bcodes = branchCode.Split(',');
                //foreach (var code in bcodes)
                //{
                //    b_codes += $@"'{code}'";
                //}
                query += string.Format(@" AND A.BRANCH_CODE IN ('{0}')",  branchCode);
            }

            if (!string.IsNullOrEmpty(moduleCode) && moduleCode != "00")
            {
                query += string.Format(" AND B.MODULE_CODE = '{0}'", moduleCode);
            }

            if (append == LoadMode.top.ToString())
            {
                query += string.Format(" AND TO_NUMBER(A.SESSION_ROWID) > {0}", sessionRowId);
            }
            else if (append == LoadMode.bottom.ToString())
            {
                query += string.Format(" AND TO_NUMBER(A.SESSION_ROWID) < {0}", sessionRowId);
            }


            query += " Order By SESSION_ROWID DESC) a WHERE rownum < 50  UNION ALL ";

            query += string.Format(@"SELECT a.*, rownum r__
                     FROM(SELECT DISTINCT A.VOUCHER_NO ,A.CHECKED_BY,A. AUTHORISED_BY,A.POSTED_BY ,SC.CHECK_FLAG,SC.POST_FLAG,SC.VERIFY_FLAG,
                 nvl( A.VOUCHER_AMOUNT,0) VOUCHER_AMOUNT , A.FORM_CODE , A.CREATED_BY , A.VOUCHER_DATE , BS_DATE(A.VOUCHER_DATE) Miti, B.FORM_EDESC FORM_DESCRIPTION,B.FORM_TYPE,
                  C.REMARKS ,C.PARTICULARS, C.ACC_CODE , D.ACC_EDESC LEDGER_TITLE, E.BRANCH_EDESC BRANCH_DESCRIPTION, TO_NUMBER(A.SESSION_ROWID) SESSION_ROWID,
                B.MODULE_CODE, 'FINANCIAL' as TABLE_NAME
                FROM MASTER_TRANSACTION A, FORM_SETUP B, V$VIRTUAL_GENERAL_LEDGER C, FA_CHART_OF_ACCOUNTS_SETUP D, FA_BRANCH_SETUP E,SC_FORM_CONTROL SC
                WHERE A.FORM_CODE = B.FORM_CODE
                     AND A.FORM_CODE=SC.FORM_CODE
                AND A.COMPANY_CODE=SC.COMPANY_CODE
               -- AND SC.USER_NO ={0}
                AND A.FORM_CODE IN(SELECT FORM_CODE FROM SC_FORM_CONTROL WHERE USER_NO={0} AND (VERIFY_FLAG='Y'))
                AND A.VOUCHER_NO = C.VOUCHER_NO
                AND C.SERIAL_NO = 1
                AND C.ACC_CODE = D.ACC_CODE
                AND (A.CHECKED_BY IS NOT NULL AND A.AUTHORISED_BY IS NULL) 
                AND A.BRANCH_CODE = E.BRANCH_CODE
                AND A.BRANCH_CODE = C.BRANCH_CODE
                AND A.COMPANY_CODE = B.COMPANY_CODE
                AND A.COMPANY_CODE = C.COMPANY_CODE
                AND A.COMPANY_CODE = D.COMPANY_CODE
               -- AND A.COMPANY_CODE = '{1}'
                ", userId, companyCode);

            if (!string.IsNullOrEmpty(branchCode))
            {
                query += string.Format(@" AND A.BRANCH_CODE IN ('{0}')", string.Join("','", branchCode));
            }

            if (!string.IsNullOrEmpty(moduleCode) && moduleCode != "00")
            {
                query += string.Format(" AND B.MODULE_CODE = '{0}'", moduleCode);
            }

            if (append == LoadMode.top.ToString())
            {
                query += string.Format(" AND TO_NUMBER(A.SESSION_ROWID) > {0}", sessionRowId);
            }
            else if (append == LoadMode.bottom.ToString())
            {
                query += string.Format(" AND TO_NUMBER(A.SESSION_ROWID) < {0}", sessionRowId);
            }


            query += " Order By SESSION_ROWID DESC) a WHERE rownum < 50 UNION ALL ";

            query += string.Format(@" SELECT a.*, rownum r__
                    FROM(SELECT DISTINCT A.VOUCHER_NO ,A.CHECKED_BY,A. AUTHORISED_BY,A.POSTED_BY ,SC.CHECK_FLAG,SC.POST_FLAG,SC.VERIFY_FLAG,
                 nvl( A.VOUCHER_AMOUNT,0) VOUCHER_AMOUNT , A.FORM_CODE , A.CREATED_BY , A.VOUCHER_DATE , BS_DATE(A.VOUCHER_DATE) Miti, B.FORM_EDESC FORM_DESCRIPTION,B.FORM_TYPE,
                  C.REMARKS ,C.REMARKS PARTICULARS, C.ITEM_CODE ACC_CODE , D.ITEM_EDESC LEDGER_TITLE, E.BRANCH_EDESC BRANCH_DESCRIPTION, TO_NUMBER(A.SESSION_ROWID) SESSION_ROWID,
                B.MODULE_CODE,  C.TABLE_NAME
                FROM MASTER_TRANSACTION A, FORM_SETUP B, V$VIRTUAL_STOCK_WIP_LEDGER C, IP_ITEM_MASTER_SETUP D, FA_BRANCH_SETUP E,SC_FORM_CONTROL SC
                WHERE A.FORM_CODE = B.FORM_CODE
                     AND A.FORM_CODE=SC.FORM_CODE
                AND A.COMPANY_CODE=SC.COMPANY_CODE
                --AND SC.USER_NO ={0}
                AND A.FORM_CODE IN(SELECT FORM_CODE FROM SC_FORM_CONTROL WHERE USER_NO={0} AND (CHECK_FLAG='Y' ))
                AND A.VOUCHER_NO = C.VOUCHER_NO
                AND C.SERIAL_NO = 1
                AND C.ITEM_CODE = D.ITEM_CODE
                AND (A.CHECKED_BY IS NULL) 
                AND A.BRANCH_CODE = E.BRANCH_CODE
                AND A.BRANCH_CODE = C.BRANCH_CODE
                AND A.COMPANY_CODE = B.COMPANY_CODE
                AND A.COMPANY_CODE = C.COMPANY_CODE
                AND A.COMPANY_CODE = D.COMPANY_CODE
                --AND A.COMPANY_CODE = '{1}'
                ", userId, companyCode);

            if (!string.IsNullOrEmpty(branchCode))
            {
                query += string.Format(@" AND A.BRANCH_CODE IN ('{0}')", string.Join("','", branchCode));
            }

            if (!string.IsNullOrEmpty(moduleCode) && moduleCode != "00")
            {
                query += string.Format(" AND B.MODULE_CODE = '{0}'", moduleCode);
            }

            if (append == LoadMode.top.ToString())
            {
                query += string.Format(" AND TO_NUMBER(A.SESSION_ROWID) > {0}", sessionRowId);
            }
            else if (append == LoadMode.bottom.ToString())
            {
                query += string.Format(" AND TO_NUMBER(A.SESSION_ROWID) < {0}", sessionRowId);
            }


            query += " Order By SESSION_ROWID DESC) a WHERE rownum < 50  UNION ALL ";

            query += string.Format(@" SELECT a.*, rownum r__
                     FROM(SELECT DISTINCT A.VOUCHER_NO ,A.CHECKED_BY,A. AUTHORISED_BY,A.POSTED_BY ,SC.CHECK_FLAG,SC.POST_FLAG,SC.VERIFY_FLAG,
                nvl( A.VOUCHER_AMOUNT,0) VOUCHER_AMOUNT , A.FORM_CODE , A.CREATED_BY , A.VOUCHER_DATE , BS_DATE(A.VOUCHER_DATE) Miti, B.FORM_EDESC FORM_DESCRIPTION,B.FORM_TYPE,
                  C.REMARKS ,C.REMARKS PARTICULARS, C.ITEM_CODE ACC_CODE , D.ITEM_EDESC LEDGER_TITLE, E.BRANCH_EDESC BRANCH_DESCRIPTION, TO_NUMBER(A.SESSION_ROWID) SESSION_ROWID,
                B.MODULE_CODE, C.TABLE_NAME
                FROM MASTER_TRANSACTION A, FORM_SETUP B, V$VIRTUAL_STOCK_WIP_LEDGER C, IP_ITEM_MASTER_SETUP D, FA_BRANCH_SETUP E,SC_FORM_CONTROL SC
                WHERE A.FORM_CODE = B.FORM_CODE
                     AND A.FORM_CODE=SC.FORM_CODE
                AND A.COMPANY_CODE=SC.COMPANY_CODE
                --AND SC.USER_NO ={0}
                AND A.FORM_CODE IN(SELECT FORM_CODE FROM SC_FORM_CONTROL WHERE USER_NO={0} AND (VERIFY_FLAG='Y'))
                AND A.VOUCHER_NO = C.VOUCHER_NO
                AND C.SERIAL_NO = 1
                AND C.ITEM_CODE = D.ITEM_CODE
                AND (A.CHECKED_BY IS NOT NULL AND A.AUTHORISED_BY IS NULL) 
                AND A.BRANCH_CODE = E.BRANCH_CODE
                AND A.BRANCH_CODE = C.BRANCH_CODE
                AND A.COMPANY_CODE = B.COMPANY_CODE
                AND A.COMPANY_CODE = C.COMPANY_CODE
                AND A.COMPANY_CODE = D.COMPANY_CODE
                --AND A.COMPANY_CODE = '{1}'
                    ", userId, companyCode);

            if (!string.IsNullOrEmpty(branchCode))
            {
                query += string.Format(@" AND A.BRANCH_CODE IN ('{0}')", string.Join("','", branchCode));
            }

            if (!string.IsNullOrEmpty(moduleCode) && moduleCode != "00")
            {
                query += string.Format(" AND B.MODULE_CODE = '{0}'", moduleCode);
            }

            if (append == LoadMode.top.ToString())
            {
                query += string.Format(" AND TO_NUMBER(A.SESSION_ROWID) > {0}", sessionRowId);
            }
            else if (append == LoadMode.bottom.ToString())
            {
                query += string.Format(" AND TO_NUMBER(A.SESSION_ROWID) < {0}", sessionRowId);
            }


            query += " Order By SESSION_ROWID DESC) a WHERE rownum < 50";
            //string query = @"SELECT * FROM (
            //    SELECT ROWNUM, B.MODULE_CODE as ModuleCode, A.VOUCHER_NO as VoucherNo, A.VOUCHER_DATE as VoucherDate, BS_DATE(A.VOUCHER_DATE) Miti, A.FORM_CODE as FormCode, B.FORM_EDESC as FormDescription, A.VOUCHER_AMOUNT as VoucherAmount, A.CREATED_BY as CreatedBy FROM MASTER_TRANSACTION A, FORM_SETUP B
            //    WHERE A.FORM_CODE = B.FORM_CODE
            //    AND A.COMPANY_CODE = B.COMPANY_CODE
            //    AND A.DELETED_FLAG='N'
            //    AND (A.CHECKED_BY = '' OR A.CHECKED_BY IS NULL) AND B.MODULE_CODE='01')
            //    WHERE ROWNUM < 100";
            return this._dbContext.SqlQuery<MobileDataVoucherModel>(query);
        }

        public MobileViewVoucheDetailModel GetVoucherLedgers(string voucherCode,string tableName, string companyCode, string branchCode)
        {
            var voucherLedgerRawData = this.GetVoucherLedgerDetail(voucherCode, tableName, companyCode, branchCode).ToList();
            if (voucherLedgerRawData.Count() != 0)
            {
                var vouchergroup = voucherLedgerRawData.GroupBy(x => new { x.VoucherNo, x.VoucherDate, x.CreatedDate, x.CreatedBy }).FirstOrDefault();
                var test = new MobileViewVoucheDetailModel();
                test.VOUCHER_NO = vouchergroup.Key.VoucherNo;
                test.VOUCHER_DATE = vouchergroup.Key.VoucherDate.ToString("dd-MM-yyyy");
                test.CREATED_BY = vouchergroup.Key.CreatedBy;
                test.CREATED_DATE = vouchergroup.Key.CreatedDate.ToString("dd-MM-yyyy HH:mm tt");
                if (tableName.ToUpper() != "FINANCIAL")
                {
                    test.ITEM_DETAIL = voucherLedgerRawData.Select(q => new MobileViewVoucheDetailModel.ItemDetails
                    {
                        IN_QUANTITY = q.InQty,
                        OUT_QUANTITY = q.OutQty,
                        ITEM_CODE = q.ItemCode,
                        UNIT = q.Unit,
                        UNIT_PRICE = q.UnitPrice ?? 0,
                        TOTAL_PRICE = q.TotalPrice ?? 0,
                        DESCRIPTION = q.Description,
                        SERIAL_NO = q.SerialNo,
                    }).ToList();
                }
                //else {
                    test.LEDGER_DETAIL = voucherLedgerRawData.Select(q => new MobileViewVoucheDetailModel.LedgerDetails
                    {
                        ACC_CODE = q.AccCode,
                        CREDIT_AMOUNT = q.CreditAmount ?? 0,
                        DEBIT_AMOUNT = q.DebitAmount ?? 0,
                        DESCRIPTION = q.Description,
                        PARTICULARS = q.Particulars,
                        TRANSACTION_TYPE = q.TransactionType,
                        HAS_SUB_LEDGER = q.SubledgerCount > 0 ? true : false,
                        SERIAL_NO = q.SerialNo,
                    }).ToList();
                //}
               
                //var vouchers = voucherLedgerRawData.GroupBy(x => new { x.VoucherNo, x.VoucherDate, x.CreatedDate, x.CreatedBy },
                //(key, group) => new MobileViewVoucheDetailModel
                //{
                //    VOUCHER_NO = key.VoucherNo,
                //    VOUCHER_DATE = key.VoucherDate.ToString("dd-MM-yyyy"),
                //    CREATED_BY = key.CreatedBy,
                //    CREATED_DATE = key.CreatedDate.ToString("dd-MM-yyyy"),
                //    LEDGER_DETAIL = voucherLedgerRawData.Select(q => new MobileViewVoucheDetailModel.LedgerDetails
                //    {
                //        ACC_CODE = q.AccCode,
                //        CREDIT_AMOUNT = q.CreditAmount ?? 0,
                //        DEBIT_AMOUNT = q.DebitAmount ?? 0,
                //        DESCRIPTION = q.Description,
                //        TRANSACTION_TYPE = q.TransactionType,
                //        HAS_SUB_LEDGER = q.SubledgerCount > 0 ? true : false,
                //        SERIAL_NO = q.SerialNo,
                //    }).ToList()

                //}).SingleOrDefault();


                List<MobileViewVoucheDetailModel.LedgerDetails> ledgers = test.LEDGER_DETAIL.ToList();
                foreach (var ledgerItem in ledgers)
                {
                    var subledger = this.GetLedgerSubledgerDetail(voucherCode, ledgerItem.ACC_CODE, ledgerItem.SERIAL_NO);

                    if (subledger != null && subledger.Count() > 0)
                    {
                        var subLedgerDetail = new MobileViewVoucheDetailModel.SubLedgerDetails() { TITLE = "ID/Sub Ledger" };
                        subLedgerDetail.Data = subledger.Select(q => new MobileViewVoucheDetailModel.SubLedgerDetails.SubLedgerData()
                        {
                            CREDIT_AMOUNT = q.CreditAmount ?? 0,
                            DEBIT_AMOUNT = q.DebitAmount ?? 0,
                            DESCRIPTION = q.Description,
                            SERIAL_NO = q.SerialNo,
                            SUB_CODE = q.SubCode,
                            TRANSACTION_TYPE = q.TransactionType
                        });

                        ledgerItem.SUB_LEDGER_DETAIL = subLedgerDetail;
                    }

                    if (subledger == null || subledger.Count() == 0)
                    {
                        var costCenterData = this.GetLedgerCostCenterDetail(voucherCode, ledgerItem.ACC_CODE, ledgerItem.SERIAL_NO);

                        if (costCenterData != null && costCenterData.Count() > 0)
                        {
                            var costCenterDetail = new MobileViewVoucheDetailModel.CostCenterDetails() { TITLE = "Cost Center" };
                            costCenterDetail.Data = costCenterData.Select(q => new MobileViewVoucheDetailModel.CostCenterDetails.CostCenterData()
                            {
                                BUDGET_EDESC = q.BUDGET_EDESC,
                                CR_AMOUNT = q.CR_AMOUNT ?? 0,
                                DR_AMOUNT = q.DR_AMOUNT ?? 0,
                                MITI = string.Join("-", q.MITI.Split(new char[] { '-' }).Reverse()),
                                PARTICULARS = q.PARTICULARS,
                                SERIAL_NO = q.SERIAL_NO,
                                VOUCHER_DATE = q.VOUCHER_DATE.ToString("dd-MM-yyyy"),
                                VOUCHER_NO = q.VOUCHER_NO,
                                BUDGET_CODE = q.BUDGET_CODE
                            });

                            foreach (var costCenterItem in costCenterDetail.Data)
                            {
                                var costCenterCategory = this.GetCostCenterCategory(voucherCode, costCenterItem.BUDGET_CODE, costCenterItem.SERIAL_NO);

                                if (costCenterCategory != null && costCenterCategory.Count() > 0)
                                {
                                    costCenterItem.COST_CENTER_CATEGORY = new MobileViewVoucheDetailModel.CostCenterDetails.CostCenterCategory()
                                    {
                                        TITLE = "Cost Category",
                                        CATEGORY_DATA = costCenterCategory
                                    };
                                }
                            }
                            ledgerItem.COST_CENTER_DETAIL = costCenterDetail;
                        }
                    }

                    var costCenterVatDetails = this.GetCostCenterVatDetail(voucherCode, ledgerItem.SERIAL_NO);
                    if (costCenterVatDetails != null && costCenterVatDetails.Count() > 0)
                    {
                        ledgerItem.LEDGER_VAT_DETAIL = new MobileViewVoucheDetailModel.CostCenterVat()
                        {
                            TITLE = "DC VAT Detail",
                            VAT_DATA = costCenterVatDetails.Select(q => new MobileViewVoucheDetailModel.CostCenterVatDetails()
                            {
                                CS_CODE = q.CS_CODE,
                                DOC_TYPE = q.DOC_TYPE,
                                INVOICE_DATE = q.INVOICE_DATE.ToString("dd-MM-yyyy"),
                                MANUAL_NO = q.MANUAL_NO,
                                MITI = string.Join("-", q.MITI.Split(new char[] { '-' }).Reverse()),
                                P_TYPE = q.P_TYPE,
                                SERIAL_NO = q.SERIAL_NO,
                                TAXABLE_AMOUNT = q.TAXABLE_AMOUNT ?? 0,
                                VAT_AMOUNT = q.VAT_AMOUNT ?? 0
                            })
                        };
                    }
                }
                test.LEDGER_DETAIL = ledgers;
                return test;
            }

            return null;
        }

        protected IEnumerable<MobileVoucherDetailData> GetVoucherLedgerDetail(string voucherCode,string tableName, string companyCode, string branchCode)
        {
            string query = string.Empty;
            if (string.IsNullOrEmpty(voucherCode))
            {
                return new List<MobileVoucherDetailData>();
            }
            

            if (tableName == "FINANCIAL")
            {
                 query = string.Format(@"SELECT MT.CREATED_BY as CreatedBy, MT.VOUCHER_NO as VoucherNo, MT.VOUCHER_DATE as VoucherDate, VGL.PARTICULARS as Particulars,MT.CREATED_DATE as CreatedDate,  VGL.TRANSACTION_TYPE as TransactionType, VGL.ACC_CODE as
                AccCode, VGL.DR_AMOUNT as DebitAmount, VGL.CR_AMOUNT as CreditAmount, VGL.ACC_EDESC as Description, VGL.SERIAL_NO as SerialNo,
                (SELECT COUNT(*) FROM V$VIRTUAL_SUB_LEDGER WHERE VOUCHER_NO='{0}' AND COMPANY_CODE IN ({2}) AND ACC_CODE = VGL.ACC_CODE AND DELETED_FLAG='N') as SubLedgerCount FROM
                V$VIRTUAL_GENERAL_LEDGER VGL INNER JOIN MASTER_TRANSACTION MT ON VGL.VOUCHER_NO = MT.VOUCHER_NO
                WHERE VGL.DELETED_FLAG='N' AND MT.DELETED_FLAG ='N' AND  VGL.VOUCHER_NO='{1}' AND VGL.COMPANY_CODE IN({2})", voucherCode, voucherCode, companyCode);

                if (!string.IsNullOrEmpty(branchCode))
                {
                    query += $@" AND MT.BRANCH_CODE IN ({branchCode})";
                }
            }
            else {
                query = $@"SELECT MT.CREATED_BY AS CreatedBy,
                               MT.VOUCHER_NO AS VoucherNo,
                               MT.VOUCHER_DATE AS VoucherDate,
                               VGL.REMARKS AS Particulars,
                               MT.CREATED_DATE AS CreatedDate,
                              -- VGL.TRANSACTION_TYPE AS TransactionType,
                               VGL.ITEM_CODE AS ItemCode,
                               VGL.CALC_QUANTITY AS InQty,
                               VGL.OUT_CALC_QUANTITY AS OutQty,
                               VGL.ITEM_EDESC AS Description,
                               VGL.SERIAL_NO AS SerialNo,
                               VGL.CALC_UNIT_PRICE as UnitPrice,
                               VGL.CALC_TOTAL_PRICE as TotalPrice,
                               IM.MU_EDESC as Unit,
                               (SELECT COUNT (*)
                                  FROM V$VIRTUAL_STOCK_WIP_LEDGER1
                                 WHERE     VOUCHER_NO = '{voucherCode}'
                                       AND MT.COMPANY_CODE IN ({companyCode})
                                       AND ITEM_CODE = VGL.ITEM_CODE
                                       AND DELETED_FLAG = 'N')
                                  AS SubLedgerCount
                          FROM    V$VIRTUAL_STOCK_WIP_LEDGER1 VGL, MASTER_TRANSACTION MT, IP_MU_CODE IM
                         WHERE     VGL.DELETED_FLAG = 'N'
                         AND VGL.MU_CODE = IM.MU_CODE
                         AND VGL.VOUCHER_NO = MT.VOUCHER_NO
                               AND MT.DELETED_FLAG = 'N'
                                AND MT.COMPANY_CODE IN({companyCode})
                               AND VGL.VOUCHER_NO = '{voucherCode}'";

                if (!string.IsNullOrEmpty(branchCode))
                {
                    query += $@" AND MT.BRANCH_CODE IN ({branchCode})";
                }
            }
            //string query = string.Format(@"SELECT MT.CREATED_BY as CreatedBy, MT.VOUCHER_NO as VoucherNo, MT.VOUCHER_DATE as VoucherDate, VGL.PARTICULARS as Particulars,MT.CREATED_DATE asd CreatedDate, TO_CHAR(MT.CREATED_DATE, 'DD-MON-YYYY hh:mm am') as CreatedDateString, VGL.TRANSACTION_TYPE as TransactionType, VGL.ACC_CODE as
            //    AccCode, VGL.DR_AMOUNT as DebitAmount, VGL.CR_AMOUNT as CreditAmount, VGL.ACC_EDESC as Description, VGL.SERIAL_NO as SerialNo,
            //    (SELECT COUNT(*) FROM V$VIRTUAL_SUB_LEDGER WHERE VOUCHER_NO='{0}' AND ACC_CODE = VGL.ACC_CODE AND DELETED_FLAG='N') as SubLedgerCount FROM
            //    V$VIRTUAL_GENERAL_LEDGER VGL INNER JOIN MASTER_TRANSACTION MT ON VGL.VOUCHER_NO = MT.VOUCHER_NO
            //    WHERE VGL.DELETED_FLAG='N' AND MT.DELETED_FLAG ='N' AND  VGL.VOUCHER_NO='{1}'", voucherCode, voucherCode);
            return this._dbContext.SqlQuery<MobileVoucherDetailData>(query);
        }

        protected IEnumerable<MobileLedgerDetailData> GetLedgerSubledgerDetail(string voucherCode, string accCode, int serialNo)
        {

            if (string.IsNullOrEmpty(voucherCode) || string.IsNullOrEmpty(accCode))
            {
                return new List<MobileLedgerDetailData>();
            }

            string query = string.Format(@"SELECT SERIAL_NO as SerialNo, SUB_EDESC as DESCRIPTION,SUB_CODE as SubCode, ACC_CODE as AccCode,VOUCHER_NO as VoucherNo,DR_AMOUNT as DebitAmount,CR_AMOUNT as CreditAmount, TRANSACTION_TYPE as TransactionType, CREATED_BY as CreatedBy  
                FROM V$VIRTUAL_SUB_LEDGER WHERE VOUCHER_NO='{0}' AND ACC_CODE = '{1}' AND DELETED_FLAG='N' AND SERIAL_NO={2}", voucherCode, accCode, serialNo);

            return this._dbContext.SqlQuery<MobileLedgerDetailData>(query);
        }

        protected IEnumerable<MobileCostCenterDetailData> GetLedgerCostCenterDetail(string voucherCode, string accCode, int serialNo)
        {

            string query = string.Format(@"SELECT VOUCHER_NO, VOUCHER_DATE, BUDGET_CODE, PARTICULARS, DR_AMOUNT, CR_AMOUNT, 
                                SERIAL_NO, BS_DATE(VOUCHER_DATE) MITI, BUDGET_EDESC, BUDGET_CODE
                                FROM VVIRTUAL_ACC_BUDGET_EXPENSES WHERE SERIAL_NO ={0}  
                                AND VOUCHER_NO='{1}' 
                                AND ACC_CODE ='{2}'
                                ORDER BY ROW_ID, SERIAL_NO", serialNo, voucherCode, accCode);

            return this._dbContext.SqlQuery<MobileCostCenterDetailData>(query);
        }

        protected IEnumerable<CostCenterCategoryData> GetCostCenterCategory(string voucherCode, string budgetCode, int serialNo)
        {
            string query = string.Format(@"SELECT A.COST_CATEGORY_CODE, B.COST_CATEGORY_EDESC, B.COST_CATEGORY_STEP 
                        FROM BUDGET_CATEGORY_TRANSACTION A, BC_COST_CATEGORY_SETUP B 
                        WHERE A.COST_CATEGORY_CODE = B.COST_CATEGORY_CODE 
                        AND A.COMPANY_CODE = B.COMPANY_CODE
                        AND A.SERIAL_NO ='{0}' 
                        AND A.BUDGET_CODE='{1}' 
                        AND A.REFERENCE_NO='{2}' 
                        ORDER BY A.SERIAL_NO", serialNo, budgetCode, voucherCode);
            return this._dbContext.SqlQuery<CostCenterCategoryData>(query);
        }

        protected IEnumerable<MobileCostCenterVatDetails> GetCostCenterVatDetail(string voucherNo, int serialNo)
        {
            string query = string.Format(@"SELECT MANUAL_NO, INVOICE_DATE, CS_CODE, DOC_TYPE, TAXABLE_AMOUNT, VAT_AMOUNT, SERIAL_NO,
            BS_DATE(INVOICE_DATE)MITI, P_TYPE  FROM FA_DC_VAT_INVOICE
            WHERE SERIAL_NO = '{0}'
            AND INVOICE_NO = '{1}'
            ORDER BY SERIAL_NO", serialNo, voucherNo);

            return this._dbContext.SqlQuery<MobileCostCenterVatDetails>(query);
        }

        public int ApproveVoucher(string formCode, string voucherCode, string userName, string companyCode,string branchCode, out string message)
        {
            var responseMessage = string.Empty;
            var status = 0;
            if (this.CheckVoucherAlreadyApprove(formCode, voucherCode, companyCode, branchCode))
            {
                message = string.Format("Voucher no {0} already approved", voucherCode);
                return status;
            }

            status = this.ApproveVoucherDetail(formCode, voucherCode, userName, companyCode);
            if (status > 0)
            {
                message = string.Format("Voucher no {0} successfully approved", voucherCode);
            }
            else
            {
                message = string.Format("Voucher no {0} could not be updated");
            }

            return status;
        }

        public int AuthoriseVoucher(string formCode, string voucherCode, string userName, string companyCode, string branchCode, out string message)
        {
            var responseMessage = string.Empty;
            var status = 0;
            if (!this.CheckVoucherAlreadyApprove(formCode, voucherCode, companyCode, branchCode))
            {
                message = string.Format("Voucher no {0} has not been approved yet", voucherCode);
                return status;
            }

            if (this.CheckVoucherAlreadyAuthorized(formCode, voucherCode, companyCode, branchCode))
            {
                message = string.Format("Voucher no {0} has been already authorised", voucherCode);
                return status;
            }

            status = this.AuthorizeVoucherDetail(formCode, voucherCode, userName, companyCode);
            if (status > 0)
            {
                message = string.Format("Voucher no {0} successfully Authorised", voucherCode);
            }
            else
            {
                message = string.Format("Voucher no {0} could not be updated");
            }

            return status;
        }

        public int PostVoucher(string formCode, string voucherCode, string userName, string companyCode, string branchCode, out string message)
        {
            var responseMessage = string.Empty;
            var status = 0;

            if (!this.CheckVoucherAlreadyApprove(formCode, voucherCode, companyCode, branchCode))
            {
                message = string.Format("Voucher no {0} has not been approved yet", voucherCode);
                return status;
            }

            if (!this.CheckVoucherAlreadyAuthorized(formCode, voucherCode, companyCode, branchCode))
            {
                message = string.Format("Voucher no {0} has not been authorised yet", voucherCode);
                return status;
            }

            if (this.CheckVoucherAlreadyPosted(formCode, voucherCode, companyCode))
            {
                message = string.Format("Voucher no {0} has been already posted", voucherCode);
                return status;
            }

            status = this.PostVoucherDetail(formCode, voucherCode, userName, companyCode);
            if (status > 0)
            {
                message = string.Format("Voucher no {0} successfully Posted", voucherCode);
            }
            else
            {
                message = string.Format("Voucher no {0} could not be updated");
            }

            return status;
        }
        protected int ApproveVoucherDetail(string formCode, string voucherCode, string userName, string companyCode)
        {
            string query = string.Format(@"UPDATE MASTER_TRANSACTION SET CHECKED_BY = '{0}', CHECKED_DATE = TO_DATE('{1}', 'yyyy/mm/dd hh24:mi:ss') WHERE FORM_CODE ='{2}' AND VOUCHER_NO = '{3}' AND COMPANY_CODE IN ({4})",
            userName, DateTime.Now.ToString("yyyy/MM/dd/ HH:mm:ss"), formCode, voucherCode, companyCode);

            return this._dbContext.ExecuteSqlCommand(query);
        }

        protected int AuthorizeVoucherDetail(string formCode, string voucherCode, string userName, string companyCode)
        {
            string query = string.Format(@"UPDATE MASTER_TRANSACTION SET AUTHORISED_BY = '{0}', AUTHORISED_DATE = TO_DATE('{1}', 'yyyy/mm/dd hh24:mi:ss') WHERE FORM_CODE ='{2}' AND VOUCHER_NO = '{3}'AND COMPANY_CODE IN ({4})",
            userName, DateTime.Now.ToString("yyyy/MM/dd/ HH:mm:ss"), formCode, voucherCode, companyCode);

            return this._dbContext.ExecuteSqlCommand(query);
        }

        protected int PostVoucherDetail(string formCode, string voucherCode, string userName, string companyCode)
        {
            string query = string.Format(@"UPDATE MASTER_TRANSACTION SET POSTED_BY = '{0}', POSTED_DATE = TO_DATE('{1}', 'yyyy/mm/dd hh24:mi:ss') WHERE FORM_CODE ='{2}' AND VOUCHER_NO = '{3}'AND COMPANY_CODE IN ({4})",
            userName, DateTime.Now.ToString("yyyy/MM/dd/ HH:mm:ss"), formCode, voucherCode, companyCode);

            return this._dbContext.ExecuteSqlCommand(query);
        }

        protected bool CheckVoucherAlreadyApprove(string formCode, string voucherCode, string companyCode, string branchCode)
        {
            string query = string.Format(@"SELECT COUNT(*) FROM MASTER_TRANSACTION A, FORM_SETUP B
                WHERE A.FORM_CODE = B.FORM_CODE
                AND A.COMPANY_CODE = B.COMPANY_CODE
                AND A.DELETED_FLAG='N'
                AND (A.CHECKED_BY = '' OR A.CHECKED_BY IS NULL)  AND A.FORM_CODE='{0}' AND A.VOUCHER_NO = '{1}' AND A.COMPANY_CODE IN ({2})", formCode, voucherCode, companyCode);

            if (!string.IsNullOrEmpty(branchCode))
            {
                query += $@" AND A.BRANCH_CODE IN ({branchCode})";
            }

            var data = this._dbContext.SqlQuery<int>(query).FirstOrDefault();

            if (data > 0)
            {
                return false;
            }

            //if (this._dbContext.SqlQuery<int>(query).FirstOrDefault() > 0)
            //{
            //    return false;
            //}

            return true;
        }

        protected bool CheckVoucherAlreadyAuthorized(string formCode, string voucherCode, string companyCode, string branchCode)
        {
            string query = string.Format(@"SELECT  COUNT(*)  FROM MASTER_TRANSACTION A, FORM_SETUP B
                WHERE A.FORM_CODE = B.FORM_CODE
                AND A.COMPANY_CODE = B.COMPANY_CODE
                AND A.DELETED_FLAG='N'
                AND (A.AUTHORISED_BY = '' OR A.AUTHORISED_BY IS NULL) AND A.FORM_CODE='{0}' AND A.VOUCHER_NO = '{1}' AND A.COMPANY_CODE IN ({2}) ", formCode, voucherCode, companyCode);

            if (!string.IsNullOrEmpty(branchCode))
            {
                query += $@" AND A.BRANCH_CODE IN ({branchCode})";
            }
            var data = this._dbContext.SqlQuery<int>(query).FirstOrDefault();

            if (data > 0)
            {
                return false;
            }

            return true;
        }

        protected bool CheckVoucherAlreadyPosted(string formCode, string voucherCode, string companyCode)
        {
            string query = string.Format(@"SELECT  COUNT(*) FROM MASTER_TRANSACTION A, FORM_SETUP B
                WHERE A.FORM_CODE = B.FORM_CODE
                AND A.COMPANY_CODE = B.COMPANY_CODE
                AND A.DELETED_FLAG='N'
                AND (A.POSTED_BY = '' OR A.POSTED_BY IS NULL) AND B.MODULE_CODE='01' AND A.FORM_CODE='{0}' AND A.VOUCHER_NO = '{1}' AND A.COMPANY_CODE IN ({2}) ", formCode, voucherCode, companyCode);


            var data = this._dbContext.SqlQuery<int>(query).FirstOrDefault();

            if (data > 0)
            {
                return false;
            }
            //if (this._dbContext.SqlQuery<int>(query).FirstOrDefault() > 0)
            //{
            //    return false;
            //}

            return true;
        }

        public IEnumerable<ModuleCount> GetModuelCount()
        {
            string query = @"select b.module_code, count(*) as MODULE_COUNT from master_transaction a, form_setup b
                    where(a.checked_by is null or a.checked_by = '')
                    and a.form_code = b.form_code
                    and a.company_code = b.company_code and a.DELETED_FLAG = 'N'
                    group by b.module_code
                    order by 1";

            return this._dbContext.SqlQuery<ModuleCount>(query);
        }

        public LoginResponseModel Login(string UserName, string Password, out string message, out int statusCode)
        {
            var userInfo = this.CheckUser(UserName, Password);
            if(userInfo.Count() == 1)
            {
                var user = userInfo.FirstOrDefault();
                
                var userPermission = this.GetModulePermission(user.USER_ID);
                if(userPermission.Count() > 0)
                {
                    user.MODULE_PERMISSION = userPermission.ToList();
                }

                message = "Login Sucessfull";
                statusCode = (int)HttpStatusCode.OK;
                return user;
            }
            if (userInfo.Count() > 1)
            {
                message = "Multiple users found";
                statusCode = (int)HttpStatusCode.Ambiguous;
                var user = userInfo.FirstOrDefault();
                return null;
            }

            message = "User doesnot exist";
            statusCode = (int)HttpStatusCode.Unauthorized;
            return null;
        }


        public List<LoginResponseModel> LoginM(string UserName, string Password, out string message, out int statusCode)
        {
            var userInfo = this.CheckUser(UserName, Password);
            if (userInfo.Count() >= 1)
            {
                var user = new List<LoginResponseModel>();
                foreach (var users in userInfo)
                {
                    var userPermission = this.GetModulePermission(users.USER_ID);
                    if (userPermission.Count() > 0)
                    {
                        users.MODULE_PERMISSION = userPermission.ToList();
                    }
                    user.Add(users);
                }
                message = "Multiple users found";
                statusCode = (int)HttpStatusCode.OK;
                return user;
            }
            message = "User doesnot exist";
            statusCode = (int)HttpStatusCode.Unauthorized;
            return null;
        }

        public bool UserExist(int userId, string companyCode)
        {
            var result = this.CheckUserExist(userId, companyCode);

            if (result.Count() >= 1)
                return true;

            return false;
        }

        protected IEnumerable<LoginResponseModel> CheckUser(string userName, string pasword)
        {
            string query = string.Format(@" SELECT SAU.USER_NO as USER_ID, SAU.LOGIN_CODE as USER_NAME, SAU.COMPANY_CODE , SAU.LOGIN_EDESC as FULL_NAME FROM SC_APPLICATION_USERS SAU
                WHERE LOWER(SAU.LOGIN_CODE)=LOWER('{0}') AND (FN_DECRYPT_PASSWORD(PASSWORD) ='{1}' OR PASSWORD = '{1}')
                 AND SAU.DELETED_FLAG='N'", userName,pasword);

            return this._dbContext.SqlQuery<LoginResponseModel>(query);
        }

        protected IEnumerable<LoginResponseModel> CheckUserExist(int userId, string companyCode)
        {
            string query = string.Format(@" SELECT SAU.USER_NO as USER_ID, SAU.LOGIN_CODE as USER_NAME, SAU.COMPANY_CODE , SAU.LOGIN_EDESC as FULL_NAME FROM SC_APPLICATION_USERS SAU
                WHERE SAU.USER_NO = '{0}'
                 AND SAU.COMPANY_CODE IN ({1})
                 AND SAU.DELETED_FLAG='N'", userId, companyCode);

            return this._dbContext.SqlQuery<LoginResponseModel>(query);
        }

        public IEnumerable<string> GetModulePermission(int userId)
        {
            string query = string.Format(@"SELECT MODULE_CODE FROM SC_MODULE_USER_CONTROL WHERE USER_NO = {0} AND DELETED_FLAG='N'",userId);
            return this._dbContext.SqlQuery<string>(query);
        }

        public IEnumerable<string> GetModulePermission(int userId, string companyCode)
        {
            string query = string.Format(@"SELECT MODULE_CODE FROM SC_MODULE_USER_CONTROL WHERE USER_NO = {0} AND COMPANY_CODE IN ({1}) AND DELETED_FLAG='N'", userId, companyCode);
            return this._dbContext.SqlQuery<string>(query);
        }
        
        public List<AccountTreeModelMobile> AccountListAllGroupNodesAutoComplete(string Company_code, string branchCode)
        {
            string query = @"SELECT INITCAP(CS.Acc_edesc) as AccountName,CS.acc_code as AccountCode,
                CS.ACC_TYPE_FLAG as AccountTypeFlag,CS.Master_acc_code as MasterAccCode,CS.pre_acc_code as PreAccCode, CS.BRANCH_CODE as BranchCode
                 FROM fa_chart_of_accounts_setup CS
                WHERE CS.DELETED_FLAG = 'N' AND CS.COMPANY_CODE IN("+ Company_code + @")
                AND ACC_TYPE_FLAG = 'T' ";

            if (!string.IsNullOrEmpty(branchCode))
            {
                query += $@" AND BRANCH_CODE IN ({branchCode})";
            }
            var AccountListNodes = _dbContext.SqlQuery<AccountTreeModelMobile>(query).ToList();
            return AccountListNodes;
        }
        public List<SubLedgerList> SubLedgerList(string Company_code, string branchCode)
        {
            string query = $@"select DISTINCT sub_code as Sub_code ,INITCAP(FN_FETCH_DESC(company_code,'FA_SUB_LEDGER_SETUP',sub_code)) as SubLedgerName from fa_sub_ledger_map where deleted_flag='N' AND COMPANY_CODE IN ('{Company_code}')";

            if (!string.IsNullOrEmpty(branchCode))
            {
                query += $@" AND BRANCH_CODE IN ({branchCode})";
            }

            var LedgerList = _dbContext.SqlQuery<SubLedgerList>(query).ToList();
            return LedgerList;
        }

        //public List<SubLedgerList> DealerSubLedgerList(string Company_code, string branchCode)
        //{
        //    string query = $@"select DISTINCT SM.sub_code as Sub_code ,INITCAP(FN_FETCH_DESC(SM.company_code,'FA_SUB_LEDGER_SETUP',SM.sub_code)) as SubLedgerName 
        //                    from fa_sub_ledger_map SM,FA_SUB_LEDGER_DEALER_MAP SDM where SM.COMPANY_CODE=SDM.COMPANY_CODE
        //                    AND SM.SUB_CODE=SDM.SUB_CODE AND  SM.deleted_flag='N' AND SM.COMPANY_CODE IN ({Company_code})";

        //    if (!string.IsNullOrEmpty(branchCode))
        //    {
        //        query += $@" AND SM.BRANCH_CODE IN ({branchCode})";
        //    }

        //    var LedgerList = _dbContext.SqlQuery<SubLedgerList>(query).ToList();
        //    return LedgerList;
        //}

        public List<DealerPartyTypeModel> DealerSubLedgerList(string Company_code, string branchCode)
        {
            string query = $@"SELECT IP.PARTY_TYPE_CODE,FA.ACC_EDESC,IP.PARTY_TYPE_EDESC
                            FROM IP_PARTY_TYPE_CODE IP,FA_CHART_OF_ACCOUNTS_SETUP FA 
                            WHERE IP.COMPANY_CODE=FA.COMPANY_CODE AND IP.ACC_CODE=FA.ACC_CODE
                                        AND IP.PARTY_TYPE_FLAG='D' AND IP.COMPANY_CODE = '{Company_code}'";

            if (!string.IsNullOrEmpty(branchCode))
            {
                query += $@" AND IP.BRANCH_CODE= '{branchCode}'";
            }

            var LedgerList = _dbContext.SqlQuery<DealerPartyTypeModel>(query).ToList();
            return LedgerList;
        }

        public List<SubLedgerList> DealerCustomerList(string party_type_code)
        {
            string Query = $@"SELECT FLD.SUB_CODE,SCS.CUSTOMER_EDESC SUBLEDGERNAME
                            FROM FA_SUB_LEDGER_DEALER_MAP FLD,SA_CUSTOMER_SETUP SCS
                            WHERE FLD.CUSTOMER_CODE = SCS.CUSTOMER_CODE
                                        AND FLD.COMPANY_CODE = SCS.COMPANY_CODE
                                        AND FLD.PARTY_TYPE_CODE='{party_type_code}'";
            var customers = _dbContext.SqlQuery<SubLedgerList>(Query).ToList();
            return customers;
        }

        public List<AccountModel> AccountSubLedgerList(string Company_code,string subCode)
        {
            string query = $@"select acc_code as AccountCode,acc_code,UPPER(FN_FETCH_DESC(company_code,'FA_CHART_OF_ACCOUNTS_SETUP',ACC_CODE)) as AccountName from fa_sub_ledger_map where deleted_flag='N' AND COMPANY_CODE IN ('{Company_code}')
                               AND SUB_CODE='{subCode}'";
            var LedgerList = _dbContext.SqlQuery<AccountModel>(query).ToList();
            return LedgerList;
        }
        public List<VoucherDetailModelMobile> GetLedgerDetailBySubCode(string accountCode, string SubAccCode, string formDate, string toDate,string company_code, string branchCode)
        {
            if (string.IsNullOrEmpty(accountCode))
                accountCode = "";
            else
            accountCode = "'" + accountCode.Replace(",", "','") + "'";

            SubAccCode = "'" + SubAccCode.Replace(",", "','") + "'";


            string Query = $@"SELECT sub_code,
                   PARTICULARS,
                   manual_no,
                   Voucher_no,
                   voucher_date,
                   bs_date (voucher_date) AS Miti,
                   dr_amount,
                   cr_amount,
                   ABS (l_csum) AS Balance,
                   (CASE WHEN l_csum >= 0 THEN 'DR' ELSE 'CR' END) BalanceHeader
              FROM (SELECT sub_code,
                           PARTICULARS,
                           manual_no,
                           Voucher_no,
                           voucher_date,
                           bs_date (TO_DATE (voucher_date, 'YYYY-MM-DD')) AS Miti,
                           dr_amount,
                           cr_amount,
                           balance,
                           SUM (dr_amount - cr_amount)
                              OVER (ORDER BY voucher_date
                                    ROWS BETWEEN UNBOUNDED PRECEDING
                                         AND     CURRENT ROW)
                              l_csum
                      FROM (SELECT '0' AS sub_code,
                                   '' AS sub_edesc,
                                   TO_DATE ('{formDate }', 'YYYY-MM-DD') AS voucher_date,
                                   bs_date (TO_DATE ('{formDate}', 'YYYY-MM-DD')) AS Miti,
                                   '0' AS Voucher_no,
                                   '' AS manual_no,
                                   'Opening' AS PARTICULARS,
                                   CASE
                                      WHEN SUM (NVL (dr_amount, 0) - NVL (cr_amount, 0)) >=
                                              0
                                      THEN
                                         SUM (NVL (dr_amount, 0) - NVL (cr_amount, 0))
                                      ELSE
                                         0
                                   END
                                      AS dr_amount,
                                   CASE
                                      WHEN SUM (NVL (dr_amount, 0) - NVL (cr_amount, 0)) <
                                              0
                                      THEN
                                         ABS (
                                            SUM (NVL (dr_amount, 0) - NVL (cr_amount, 0)))
                                      ELSE
                                         0
                                   END
                                      AS cr_amount,
                                   0 AS balance
                              FROM v$virtual_sub_ledger
                             WHERE  sub_code IN ({SubAccCode})
                                   AND company_code IN ({company_code})
                                   AND (VOUCHER_DATE <
                                   TO_DATE ('{formDate}', 'YYYY-MM-DD')
                                       OR FORM_CODE='0')";
            if (accountCode != "'00'")
            {
                Query += $@" AND acc_code IN ({accountCode})";
            }

            Query += $@" UNION ALL
                            SELECT sub_code,
                                   sub_edesc,
                                   voucher_date,
                                   bs_date (TO_DATE (voucher_date, 'YYYY-MM-DD')) AS Miti,
                                   Voucher_no,
                                   manual_no,
                                   PARTICULARS,
                                   dr_amount,
                                   cr_amount,
                                   l_csum balance
                              FROM (  SELECT sub_code,
                                             sub_edesc,
                                             voucher_date,
                                             bs_date (TO_DATE (voucher_date, 'YYYY-MM-DD'))
                                                AS Miti,
                                             Voucher_no,
                                             manual_no,
                                             PARTICULARS,
                                             dr_amount,
                                             cr_amount,
                                             dr_amount - cr_amount balance,
                                             SUM (
                                                dr_amount - cr_amount)
                                             OVER (
                                                ORDER BY voucher_date
                                    ROWS BETWEEN UNBOUNDED PRECEDING
                                         AND     CURRENT ROW)
                                                l_csum
                                        FROM v$virtual_sub_ledger
                                       WHERE sub_code IN ({SubAccCode})
                                             AND company_code IN ({company_code})
                                             AND FORM_CODE<>0
                                             AND voucher_date >=
                                                    TO_DATE ('{formDate}', 'YYYY-MM-DD')
                                             AND voucher_date <=
                                                    TO_DATE ('{toDate}', 'YYYY-MM-DD')";
            if (accountCode != "'00'")
            {
                Query += $@" AND acc_code IN ({accountCode})";
            }
            Query += $@" ORDER BY voucher_date)))";


            //string Query = @"SELECT sub_code, PARTICULARS, manual_no, Voucher_no,voucher_date,bs_date(voucher_date) as Miti, dr_amount,cr_amount," +
            //               " abs(l_csum) as Balance, (CASE WHEN l_csum >= 0 THEN 'DR' ELSE 'CR' END) BalanceHeader FROM ( SELECT sub_code, PARTICULARS, manual_no, Voucher_no," +
            //              " voucher_date,bs_date(TO_DATE(voucher_date, 'YYYY-MM-DD')) as Miti,dr_amount,cr_amount, balance,SUM(dr_amount - cr_amount)" +
            //              " OVER(ORDER BY sub_code, voucher_date ROWS 5 PRECEDING) l_csum FROM( SELECT '0' as sub_code, '' as sub_edesc,  TO_DATE('" + formDate + "', 'YYYY-MM-DD') AS voucher_date," +
            //               " bs_date(TO_DATE('" + formDate + "', 'YYYY-MM-DD')) as Miti, '0' as Voucher_no, '' as manual_no,'Opening' AS PARTICULARS, CASE WHEN SUM(NVL(dr_amount, 0) - NVL(cr_amount, 0)) >= 0 THEN SUM(NVL(dr_amount, 0) - NVL(cr_amount, 0))" +
            //              " ELSE 0 END AS dr_amount, CASE WHEN SUM(NVL(dr_amount, 0) - NVL(cr_amount, 0)) < 0 THEN ABS(SUM(NVL(dr_amount, 0) - NVL(cr_amount, 0))) ELSE 0 END   AS cr_amount," +
            //              " 0 as balance FROM v$virtual_sub_ledger WHERE acc_code IN (" + accountCode + ") and sub_code IN (" + SubAccCode + ") and company_code = '01' AND VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD')" +
            //              " UNION ALL SELECT sub_code, sub_edesc, voucher_date, bs_date(TO_DATE(voucher_date, 'YYYY-MM-DD')) as Miti, Voucher_no,manual_no, PARTICULARS,dr_amount,cr_amount, l_csum balance" +
            //              " FROM(SELECT sub_code,sub_edesc, voucher_date, bs_date(TO_DATE(voucher_date, 'YYYY-MM-DD')) as Miti, Voucher_no, manual_no, PARTICULARS,dr_amount," +
            //              " cr_amount,dr_amount - cr_amount balance,SUM(dr_amount - cr_amount) OVER(ORDER BY sub_code, sub_edesc, voucher_date ROWS 5 PRECEDING) l_csum FROM v$virtual_sub_ledger" +
            //              "  WHERE     acc_code IN (" + accountCode + ") AND sub_code IN (" + SubAccCode + ") AND voucher_date > TO_DATE('" + formDate + "', 'YYYY-MM-DD')AND voucher_date < TO_DATE('" + toDate + "', 'YYYY-MM-DD') ORDER BY voucher_date)))";
            var subvoucherList = _dbContext.SqlQuery<VoucherDetailModelMobile>(Query).ToList();
            return subvoucherList;
        }

        public List<VoucherDetailModelMobile> GetDealerLedgerDetail(string accountCode, string SubAccCode, string formDate, string toDate, string company_code, string branchCode,string partyTypeCode)
        {
            if (string.IsNullOrEmpty(accountCode))
                accountCode = "";
            else
                accountCode = "'" + accountCode.Replace(",", "','") + "'";
            if (!string.IsNullOrEmpty(SubAccCode))
                SubAccCode = "'" + SubAccCode.Replace(",", "','") + "'";


            string Query = $@"SELECT sub_code,
                   PARTICULARS,
                   manual_no,
                   Voucher_no,
                   voucher_date,
                   bs_date (voucher_date) AS Miti,
                   dr_amount,
                   cr_amount,
                   ABS (l_csum) AS Balance,
                   (CASE WHEN l_csum >= 0 THEN 'DR' ELSE 'CR' END) BalanceHeader
              FROM (SELECT sub_code,
                           PARTICULARS,
                           manual_no,
                           Voucher_no,
                           voucher_date,
                           bs_date (TO_DATE (voucher_date, 'YYYY-MM-DD')) AS Miti,
                           dr_amount,
                           cr_amount,
                           balance,
                           SUM (dr_amount - cr_amount)
                              OVER (ORDER BY voucher_date
                                    ROWS BETWEEN UNBOUNDED PRECEDING
                                         AND     CURRENT ROW)
                              l_csum
                      FROM (SELECT '0' AS sub_code,
                                   '' AS sub_edesc,
                                   TO_DATE ('{formDate }', 'YYYY-MM-DD') AS voucher_date,
                                   bs_date (TO_DATE ('{formDate}', 'YYYY-MM-DD')) AS Miti,
                                   '0' AS Voucher_no,
                                   '' AS manual_no,
                                   'Opening' AS PARTICULARS,
                                   CASE
                                      WHEN SUM (NVL (dr_amount, 0) - NVL (cr_amount, 0)) >=
                                              0
                                      THEN
                                         SUM (NVL (dr_amount, 0) - NVL (cr_amount, 0))
                                      ELSE
                                         0
                                   END
                                      AS dr_amount,
                                   CASE
                                      WHEN SUM (NVL (dr_amount, 0) - NVL (cr_amount, 0)) <
                                              0
                                      THEN
                                         ABS (
                                            SUM (NVL (dr_amount, 0) - NVL (cr_amount, 0)))
                                      ELSE
                                         0
                                   END
                                      AS cr_amount,
                                   0 AS balance
                              FROM V$VIRTUAL_SUB_DEALER_LEDGER
                             WHERE PARTY_TYPE_CODE='{partyTypeCode}'
                                   AND company_code IN ({company_code})
                                   AND (VOUCHER_DATE <
                                   TO_DATE ('{formDate}', 'YYYY-MM-DD')
                                       OR FORM_CODE='0')";
            if (accountCode != "'00'")
            {
                Query += $@" AND acc_code IN ({accountCode})";
            }
            if (!string.IsNullOrWhiteSpace(SubAccCode))
                Query += $" AND sub_code IN({ SubAccCode})";

            Query += $@" UNION ALL
                            SELECT sub_code,
                                   sub_edesc,
                                   voucher_date,
                                   bs_date (TO_DATE (voucher_date, 'YYYY-MM-DD')) AS Miti,
                                   Voucher_no,
                                   manual_no,
                                   PARTICULARS,
                                   dr_amount,
                                   cr_amount,
                                   l_csum balance
                              FROM (  SELECT sub_code,
                                             sub_edesc,
                                             voucher_date,
                                             bs_date (TO_DATE (voucher_date, 'YYYY-MM-DD'))
                                                AS Miti,
                                             Voucher_no,
                                             manual_no,
                                             PARTICULARS,
                                             dr_amount,
                                             cr_amount,
                                             dr_amount - cr_amount balance,
                                             SUM (
                                                dr_amount - cr_amount)
                                             OVER (
                                                ORDER BY voucher_date
                                    ROWS BETWEEN UNBOUNDED PRECEDING
                                         AND     CURRENT ROW)
                                                l_csum
                                        FROM V$VIRTUAL_SUB_DEALER_LEDGER
                                       WHERE PARTY_TYPE_CODE='{partyTypeCode}'
                                             AND company_code IN ({company_code})
                                             AND FORM_CODE<>0
                                             AND voucher_date >=
                                                    TO_DATE ('{formDate}', 'YYYY-MM-DD')
                                             AND voucher_date <=
                                                    TO_DATE ('{toDate}', 'YYYY-MM-DD')";
            if (accountCode != "'00'")
            {
                Query += $@" AND acc_code IN ({accountCode})";
            }
            if (!string.IsNullOrWhiteSpace(SubAccCode))
                Query += $" AND sub_code IN({ SubAccCode})";

            Query += $@" ORDER BY voucher_date)))";
            var subvoucherList = _dbContext.SqlQuery<VoucherDetailModelMobile>(Query).ToList();
            return subvoucherList;
        }

        public List<VoucherDetailModelMobile> GetVoucherDetailsByAccountCode(ReportFiltersModel reportFilters, string formDate, string toDate, string AccountCode, string BranchCode , string DataGeneric = "DG")
        {

            AccountCode = "'" + AccountCode.Replace(",", "','") + "'";
            string Query = string.Empty;
          
            string branchcondition = string.IsNullOrEmpty(BranchCode) ? "" : " and branch_code in (" + BranchCode + ") ";
            string companycondition = reportFilters.CompanyFilter.Count == 0 ? " company_code='01' " : " company_code in (" + string.Join(",", reportFilters.CompanyFilter) + ") ";
            if (DataGeneric == "DG")
            {
                Query = @"SELECT acc_code,   PARTICULARS,   manual_no,  Voucher_no, voucher_date,  bs_date(voucher_date) AS Miti, dr_amount, cr_amount,
  ABS (l_csum)AS Balance, (CASE WHEN l_csum >= 0 THEN 'DR' ELSE 'CR' END) BalanceHeader
  FROM(SELECT acc_code, PARTICULARS, manual_no, Voucher_no, voucher_date, bs_date(TO_DATE(voucher_date, 'YYYY-MM-DD')) AS Miti, dr_amount, cr_amount, balance,
   SUM(NVL(dr_amount, 0) - NVL(cr_amount, 0))  OVER(ORDER BY    voucher_date  ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW)  l_csum
  FROM(SELECT '0' AS acc_code, TO_DATE('" + formDate + @"', 'YYYY-MM-DD') AS voucher_date, bs_date(TO_DATE('" + formDate + @"', 'YYYY-MM-DD')) AS Miti, '0' AS Voucher_no, '' AS manual_no,
   'Opening' AS PARTICULARS, CASE  WHEN SUM(NVL(dr_amount, 0) - NVL(cr_amount, 0)) >= 0    THEN      SUM(NVL(dr_amount, 0) - NVL(cr_amount, 0))    ELSE    0
   END  AS dr_amount, CASE WHEN SUM(NVL(dr_amount, 0) - NVL(cr_amount, 0)) < 0   THEN        ABS(SUM(NVL(dr_amount, 0) - NVL(cr_amount, 0)))
  ELSE     0   END AS cr_amount, 0 AS balance    FROM V$VIRTUAL_GENERAL_LEDGER    WHERE     " + companycondition + @"     AND deleted_flag = 'N' " + branchcondition +
   "AND acc_code IN(" + AccountCode + @")
                       AND " + companycondition + @"
                       AND (VOUCHER_DATE <=
                              TO_DATE('" + formDate + @"', 'YYYY-MM-DD')
                            and form_code = '0')
                UNION ALL
                SELECT acc_code,
                       voucher_date,
                       bs_date(TO_DATE(voucher_date, 'YYYY-MM-DD')) AS Miti,
                       Voucher_no,
                       manual_no,
                       PARTICULARS,
                       dr_amount,
                       cr_amount,
                       l_csum balance
                  FROM(SELECT acc_code,
                               voucher_date,
                               bs_date(TO_DATE(voucher_date, 'YYYY-MM-DD'))  AS Miti, Voucher_no, manual_no, PARTICULARS, dr_amount, 0, cr_amount,
                               NVL(dr_amount, 0) - NVL(cr_amount, 0)  balance,
                               SUM(NVL(dr_amount, 0) - NVL(cr_amount, 0))
                               OVER(ORDER BY    voucher_date  ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW)    l_csum
                          FROM V$VIRTUAL_GENERAL_LEDGER
                         WHERE     " + companycondition + @"
                               AND deleted_flag = 'N'" +
                                  branchcondition +
                                  "AND acc_code IN(" + AccountCode + @")
                               AND voucher_date >= TO_DATE('" + formDate + @"', 'YYYY-MM-DD')
                               AND voucher_date <= TO_DATE('" + toDate + "', 'YYYY-MM-DD') order by  TO_DATE(voucher_date, 'YYYY-MM-DD'))))";
            }
            else
            {
                Query = @"SELECT acc_code,   PARTICULARS,   manual_no,  Voucher_no, voucher_date,  bs_date(voucher_date) AS Miti, dr_amount, cr_amount,
  ABS (l_csum)AS Balance, (CASE WHEN l_csum >= 0 THEN 'DR' ELSE 'CR' END) BalanceHeader
  FROM(SELECT acc_code, PARTICULARS, manual_no, Voucher_no, voucher_date, bs_date(TO_DATE(voucher_date, 'YYYY-MM-DD')) AS Miti, dr_amount, cr_amount, balance,
   SUM(NVL(dr_amount, 0) - NVL(cr_amount, 0))  OVER(ORDER BY    voucher_date  ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW)  l_csum
  FROM(SELECT '0' AS acc_code, TO_DATE('" + formDate + @"', 'YYYY-MM-DD') AS voucher_date, bs_date(TO_DATE('" + formDate + @"', 'YYYY-MM-DD')) AS Miti, '0' AS Voucher_no, '' AS manual_no,
   'Opening' AS PARTICULARS, CASE  WHEN SUM(NVL(dr_amount, 0) - NVL(cr_amount, 0)) >= 0    THEN      SUM(NVL(dr_amount, 0) - NVL(cr_amount, 0))    ELSE    0
   END  AS dr_amount, CASE WHEN SUM(NVL(dr_amount, 0) - NVL(cr_amount, 0)) < 0   THEN        ABS(SUM(NVL(dr_amount, 0) - NVL(cr_amount, 0)))
  ELSE     0   END AS cr_amount, 0 AS balance    FROM V$VIRTUAL_GENERAL_LEDGER    WHERE     " + companycondition + @"   and posted_by is not null    AND deleted_flag = 'N' " + branchcondition +
  "AND acc_code IN(" + AccountCode + @")
                       AND " + companycondition + @"
                       AND (VOUCHER_DATE <=
                              TO_DATE('" + formDate + @"', 'YYYY-MM-DD')
                             and form_code = '0')
                UNION ALL
                SELECT acc_code,
                       voucher_date,
                       bs_date(TO_DATE(voucher_date, 'YYYY-MM-DD')) AS Miti,
                       Voucher_no,
                       manual_no,
                       PARTICULARS,
                       dr_amount,
                       cr_amount,
                       l_csum balance
                  FROM(SELECT acc_code,
                               voucher_date,
                               bs_date(TO_DATE(voucher_date, 'YYYY-MM-DD'))  AS Miti, Voucher_no, manual_no, PARTICULARS, dr_amount, 0, cr_amount,
                               NVL(dr_amount, 0) - NVL(cr_amount, 0)  balance,
                               SUM(NVL(dr_amount, 0) - NVL(cr_amount, 0))
                               OVER(ORDER BY    voucher_date  ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW)    l_csum
                          FROM V$VIRTUAL_GENERAL_LEDGER
                         WHERE     " + companycondition + @"
                         and posted_by is not null
                               AND deleted_flag = 'N'" +
                                 branchcondition +
                                 "AND acc_code IN(" + AccountCode + @")
                               AND voucher_date >= TO_DATE('" + formDate + @"', 'YYYY-MM-DD')
                               AND voucher_date <= TO_DATE('" + toDate + "', 'YYYY-MM-DD') order by  TO_DATE(voucher_date, 'YYYY-MM-DD'))))";


            }
            return _dbContext.SqlQuery<VoucherDetailModelMobile>(Query).ToList();


        }

        public List<CRMModel> GetCRM()
        {
            string query = $@"SELECT LEAD_NO, LEAD_DATE, LEAD_TIME, COMPANY_NAME, REQUESTED_BY, PRODUCT_EDESC, DESCRIPTION, 
                            NVL(RATING,0)RATING, TO_CHAR(COMPLETION_DATE,'DD-MON-YYYY') COMPLETION_DATE, AGENT_EDESC, PROCESS_EDESC, ESTD_COMPLETE, CEIL(DAYS)DAYS FROM V_TICKET_LIST
                            ORDER BY ROW_ORDER, DAYS, RATING  DESC";
            var crmList = _dbContext.SqlQuery<CRMModel>(query).ToList();
            return crmList;
        }

        public List<PayOrderModel> GetPayorders()
        {
            var Query = @"SELECT FPO.VOUCHER_NO,FPO.VOUCHER_DATE,FPO.ACC_CODE,FCAS.ACC_EDESC,FPO.PARTICULARS,FPO.TRANSACTION_TYPE,
                            FPO.AMOUNT,FPO.COMPANY_CODE,FPO.BRANCH_CODE
                FROM FA_PAY_ORDER FPO
                INNER JOIN FA_CHART_OF_ACCOUNTS_SETUP FCAS ON FPO.ACC_CODE = FCAS.ACC_CODE AND FPO.COMPANY_CODE = FCAS.COMPANY_CODE
                WHERE FPO.CHECKED_REMARKS IS NULL
                            AND FPO.VERIFY_REMARKS IS NULL
                            AND FPO.AUTHORISED_REMARKS IS NULL
                            AND FPO.DELETED_FLAG = 'N'
                            --AND FPO.COMPANY_CODE = '01'
                GROUP BY FPO.VOUCHER_NO,FPO.VOUCHER_DATE,FPO.ACC_CODE,FCAS.ACC_EDESC,FPO.PARTICULARS,FPO.TRANSACTION_TYPE,
                            FPO.AMOUNT,FPO.COMPANY_CODE,FPO.BRANCH_CODE
                ORDER BY FPO.VOUCHER_NO";
            var data = _dbContext.SqlQuery<PayOrderModel>(Query).ToList();
            return data;
        }

        public int UpdatePayOrder(PayOrderModel model)
        {
            if(!string.IsNullOrEmpty(model.CHECKED_REMARKS))
                {
                string Query = $@"UPDATE FA_PAY_ORDER SET CHECKED_REMARKS = '{model.CHECKED_REMARKS}'
                            WHERE VOUCHER_NO = '{model.VOUCHER_NO}' AND COMPANY_CODE = '{model.COMPANY_CODE}'";
                var rows = _dbContext.ExecuteSqlCommand(Query);
                return rows;
            }
            else if(!string.IsNullOrEmpty(model.VERIFY_REMARKS))
            {
                string Query = $@"UPDATE FA_PAY_ORDER SET  VERIFY_REMARKS = '{model.VERIFY_REMARKS}'
                            WHERE VOUCHER_NO = '{model.VOUCHER_NO}' AND COMPANY_CODE = '{model.COMPANY_CODE}'";
                var rows = _dbContext.ExecuteSqlCommand(Query);
                return rows;
            }
            else if (!string.IsNullOrEmpty(model.AUTHORISED_REMARKS))
            {
                string Query = $@"UPDATE FA_PAY_ORDER SET 
                            AUTHORISED_REMARKS = '{model.AUTHORISED_REMARKS}'
                            WHERE VOUCHER_NO = '{model.VOUCHER_NO}' AND COMPANY_CODE = '{model.COMPANY_CODE}'";
                var rows = _dbContext.ExecuteSqlCommand(Query);
                return rows;
            }
            return 0;
        }

        public SalesReportModel GetSalesReport(string company_code)
        {
            string QuerySales = $@"SELECT NVL(YTD_SALES,0) YTD_SALES,NVL(MTD_SALES,0) MTD_SALES,NVL(TODAY_SALES,0) TODAY_SALES,
         NVL(YTD_QSALES,0) YTD_QSALES,NVL(MTD_QSALES,0) MTD_QSALES,NVL(TODAY_QSALES,0) TODAY_QSALES
                    FROM(SELECT
                    (SELECT TO_CHAR(SUM(NET_SALES),'FM9,999,999,999,999')  FROM V$SALES_INVOICE_REPORT3
                    WHERE deleted_flag='N' AND COMPANY_CODE = '{company_code}') YTD_SALES,
                    (SELECT TO_CHAR(SUM(NET_SALES),'FM9,999,999,999,999') FROM V$SALES_INVOICE_REPORT3
                    WHERE SALES_DATE >= (SELECT FN_FIND_BS1STDATE(SYSDATE) FROM DUAL)
                    AND SALES_DATE <= TO_DATE(SYSDATE) AND DELETED_FLAG='N' AND COMPANY_CODE = '{company_code}') MTD_SALES,
                    (SELECT TO_CHAR(SUM(NET_SALES),'FM9,99,99,99,99,999') FROM V$SALES_INVOICE_REPORT3 
                    WHERE SALES_DATE = TO_DATE(SYSDATE) AND DELETED_FLAG='N' AND COMPANY_CODE = '{company_code}') TODAY_SALES,
                    (SELECT TO_CHAR(SUM(QUANTITY),'FM9,999,999,999,999')  FROM V$SALES_INVOICE_REPORT3
                    WHERE deleted_flag='N' AND COMPANY_CODE = '{company_code}') YTD_QSALES,
                    (SELECT TO_CHAR(SUM(QUANTITY),'FM9,999,999,999,999') FROM V$SALES_INVOICE_REPORT3
                    WHERE SALES_DATE >= (SELECT FN_FIND_BS1STDATE(SYSDATE) FROM DUAL)
                    AND SALES_DATE <= TO_DATE(SYSDATE) AND DELETED_FLAG='N' AND COMPANY_CODE = '{company_code}') MTD_QSALES,
                    (SELECT TO_CHAR(SUM(QUANTITY),'FM9,99,99,99,99,999') FROM V$SALES_INVOICE_REPORT3 
                    WHERE SALES_DATE = TO_DATE(SYSDATE) AND DELETED_FLAG='N' AND COMPANY_CODE = '{company_code}') TODAY_QSALES
                    FROM DUAL)";

            string QueryQuarterly = $@"SELECT B.QUATER QUARTER,TO_CHAR(SUM(NET_SALES),'FM9,99,99,99,99,999') AMOUNT,
                        TO_CHAR(SUM(QUANTITY),'FM9,99,99,99,99,999')QUANTITY FROM V$SALES_INVOICE_REPORT3 A, (
                    SELECT 'Q1' QUATER ,'17-Jul-2018' START_DATE, '18-Oct-2018' END_DATE FROM DUAL
                    UNION ALL
                    SELECT 'Q2' QUATER, '19-Oct-2018' START_DATE, '19-Jan-2019' END_DATE FROM DUAL 
                    UNION ALL
                    SELECT 'Q3' QUATER, '15-Jan-2019' START_DATE, '13-Apr-2019' END_DATE FROM DUAL
                    UNION ALL
                    SELECT 'Q4' QUATER, '14-Apr-2019' START_DATE, '16-Jul-2019' END_DATE FROM DUAL)B 
                    WHERE A.SALES_DATE BETWEEN B.START_DATE AND B.END_DATE AND A.DELETED_FLAG='N' AND A.COMPANY_CODE='{company_code}'
                    GROUP BY B.QUATER, B.START_DATE ,B.END_DATE ORDER BY QUATER ASC";
            string PapQuery = $@"SELECT COUNT(*)
                    FROM V$VIRTUAL_PAY_ORDER WHERE DELETED_FLAG='N' AND COMPANY_CODE='{company_code}'
                    AND VOUCHER_NO IN(SELECT VOUCHER_NO FROM MASTER_TRANSACTION WHERE CHECKED_BY IS NULL AND AUTHORISED_BY IS NULL
                    AND DELETED_FLAG='N' AND FORM_CODE IN(SELECT DISTINCT FORM_CODE FROM V$VIRTUAL_PAY_ORDER))";
            string PopQuery = $@"SELECT COUNT(*) FROM
                    (SELECT SUPPLIER_EDESC, ITEM_EDESC, MU_CODE, SUM(NVL(IN_QUANTITY,0)) ORDER_QTY, SUM(NVL(TOTAL_PRICE,0)) AMOUNT, SUM(NVL(TOTAL_PRICE,0))/SUM(NVL(IN_QUANTITY,0)) AVG_RATE
                    FROM V$PURCHASE_ORDER_ANALYSIS
                    WHERE COMPANY_CODE='{company_code}' AND DELETED_FLAG='N' AND DUE_QTY <> 0
                    GROUP BY SUPPLIER_EDESC, ITEM_EDESC, MU_CODE)";
            string PrpQuery = $@"SELECT COUNT(*) FROM(
                    SELECT ITEM_EDESC, MU_CODE, SUM(NVL(QUANTITY,0)) ORDER_QTY, SUM(NVL(TOTAL_PRICE,0)) AMOUNT, SUM(NVL(TOTAL_PRICE,0))/SUM(NVL(QUANTITY,0)) AVG_RATE
                    FROM V$PURCHASE_INDENT_ANALYSIS
                    WHERE COMPANY_CODE='{company_code}' AND DELETED_FLAG='N' AND DUE_QTY <> 0
                    GROUP BY ITEM_EDESC, MU_CODE)";

            var data = _dbContext.SqlQuery<SalesReportModel>(QuerySales).FirstOrDefault();
            data.QUARTER_SALES = _dbContext.SqlQuery<QuarterlySalesModel>(QueryQuarterly).ToList();
            data.PAPCOUNT = _dbContext.SqlQuery<int>(PapQuery).FirstOrDefault();
            data.POPCOUNT = _dbContext.SqlQuery<int>(PopQuery).FirstOrDefault();
            data.PRPCOUNT = _dbContext.SqlQuery<int>(PrpQuery).FirstOrDefault();

            return data;
        }

        public List<TopicSalesModel> GetTopicWiseSales(string companyCode, string topic)
        {
            var filter = string.Empty;
            var filterQry = $@"SELECT ITEM_CODE FROM MOBILE_ITEM_FILTER WHERE FILTER_TYPE='SALES'";
            var itemFilter = _dbContext.SqlQuery<string>(filterQry).FirstOrDefault();
            if (!string.IsNullOrEmpty(itemFilter))
                filter = $@" AND SSI.ITEM_CODE IN ('{itemFilter}') ";

            string Query = string.Empty;
            if (topic.ToUpper() == "ITEM")
                Query = $@"SELECT EDESC,TO_CHAR(AMOUNT,'FM9,999,999,999,999') AMOUNT,TO_CHAR(QUANTITY,'FM9,999,999,999,999.00') QUANTITY FROM(
                            SELECT ITEM_EDESC EDESC,
                                    SUM(OUT_QUANTITY)-SUM(IN_QUANTITY) QUANTITY,
                                    SUM(OUT_CALC_TOTAL_PRICE)-SUM(IN_CALC_TOTAL_PRICE) AMOUNT
                        FROM V$SALES_SALESRETURN_ANALYSIS WHERE COMPANY_CODE = '{companyCode}'
                        GROUP BY ROLLUP(ITEM_EDESC)
                        ORDER BY AMOUNT DESC)";
            else if (topic.ToUpper() == "DIVISION")
                Query = $@"SELECT EDESC,TO_CHAR(AMOUNT,'FM9,999,999,999,999') AMOUNT,TO_CHAR(QUANTITY,'FM9,999,999,999,999') QUANTITY FROM(
                    SELECT FDS.DIVISION_EDESC EDESC, SUM(SSI.NET_SALES) AMOUNT, SUM(SSI.QUANTITY) QUANTITY
                    FROM V$SALES_INVOICE_REPORT3 SSI
                    JOIN FA_DIVISION_SETUP FDS ON FDS.DIVISION_CODE = SSI.DIVISION_CODE AND FDS.COMPANY_CODE = SSI.COMPANY_CODE
                    WHERE SSI.DELETED_FLAG='N' AND SSI.COMPANY_CODE='{companyCode}'
                    GROUP BY ROLLUP(FDS.DIVISION_EDESC)
                    ORDER BY AMOUNT DESC)";
            else if (topic.ToUpper() == "AREA")
                Query = $@"SELECT EDESC,TO_CHAR(AMOUNT,'FM9,999,999,999,999') AMOUNT,TO_CHAR(QUANTITY,'FM9,999,999,999,999') QUANTITY FROM(
                    SELECT ARS.AREA_EDESC EDESC, SUM(SSI.CALC_TOTAL_PRICE) AMOUNT, SUM(SSI.QUANTITY) QUANTITY
                    FROM SA_SALES_INVOICE SSI
                    JOIN AREA_SETUP ARS ON ARS.AREA_CODE = SSI.AREA_CODE AND ARS.COMPANY_CODE = SSI.COMPANY_CODE
                    WHERE ARS.DELETED_FLAG='N' AND SSI.COMPANY_CODE='{companyCode}' {filter}
                    GROUP BY ROLLUP(ARS.AREA_EDESC)
                    ORDER BY AMOUNT DESC)";
            else if (topic.ToUpper() == "CUSTOMER")
                Query = $@"SELECT EDESC,TO_CHAR(AMOUNT,'FM9,999,999,999,999') AMOUNT,TO_CHAR(QUANTITY,'FM9,999,999,999,999') QUANTITY FROM(
                    SELECT SCS.CUSTOMER_EDESC EDESC, SUM(VSI.NET_SALES) AMOUNT, SUM(VSI.QUANTITY) QUANTITY
                    FROM V$SALES_INVOICE_REPORT3 VSI
                    JOIN SA_CUSTOMER_SETUP SCS ON VSI.CUSTOMER_CODE = SCS.CUSTOMER_CODE AND VSI.COMPANY_CODE = SCS.COMPANY_CODE
                    WHERE VSI.DELETED_FLAG='N'  AND VSI.COMPANY_CODE='{companyCode}'
                    GROUP BY ROLLUP(SCS.CUSTOMER_EDESC)
                    ORDER BY AMOUNT DESC)";
            else if (topic.ToUpper() == "DEALER")
                Query = $@"SELECT EDESC,TO_CHAR(AMOUNT,'FM9,999,999,999,999') AMOUNT,TO_CHAR(QUANTITY,'FM9,999,999,999,999') QUANTITY FROM(
                    SELECT IPC.PARTY_TYPE_EDESC EDESC, SUM(VSI.NET_SALES) AMOUNT, SUM(VSI.QUANTITY) QUANTITY
                    FROM V$SALES_INVOICE_REPORT3 VSI
                    JOIN IP_PARTY_TYPE_CODE IPC ON VSI.PARTY_TYPE_CODE = IPC.PARTY_TYPE_CODE AND VSI.COMPANY_CODE = IPC.COMPANY_CODE
                    WHERE VSI.DELETED_FLAG='N'  AND VSI.COMPANY_CODE='{companyCode}'
                    GROUP BY ROLLUP(IPC.PARTY_TYPE_EDESC)
                    ORDER BY AMOUNT DESC)";
            else
                return new List<TopicSalesModel>();

            var data = _dbContext.SqlQuery<TopicSalesModel>(Query).ToList();

            //placing the total row in last position
            var total = data.FirstOrDefault(x => string.IsNullOrWhiteSpace(x.EDESC));
            if(total != null)
            {
                data.Remove(total);
                total.EDESC = "Total";
                data.Add(total);
            }
            return data;
        }

        public List<TopicSalesModel> GetTopicWiseSalesMTD(string companyCode, string topic)
        {
            var filter = string.Empty;
            var filterQry = $@"SELECT ITEM_CODE FROM MOBILE_ITEM_FILTER WHERE FILTER_TYPE='SALES'";
            var itemFilter = _dbContext.SqlQuery<string>(filterQry).FirstOrDefault();
            if (!string.IsNullOrEmpty(itemFilter))
                filter = $@" AND SSI.ITEM_CODE IN ('{itemFilter}') ";

            string Query = string.Empty;
            if (topic.ToUpper() == "ITEM")
                Query = $@"SELECT EDESC,TO_CHAR(AMOUNT,'FM9,999,999,999,999') AMOUNT,TO_CHAR(QUANTITY,'FM9,999,999,999,999.00') QUANTITY FROM(
                    SELECT ITEM_EDESC EDESC,
                                    SUM(OUT_QUANTITY)-SUM(IN_QUANTITY) QUANTITY,
                                    SUM(OUT_CALC_TOTAL_PRICE)-SUM(IN_CALC_TOTAL_PRICE) AMOUNT
                        FROM V$SALES_SALESRETURN_ANALYSIS WHERE COMPANY_CODE = '{companyCode}'
                        AND VOUCHER_DATE >= (SELECT FN_FIND_BS1STDATE(SYSDATE) FROM DUAL)
                        AND VOUCHER_DATE <= TO_DATE(SYSDATE)
                        GROUP BY ROLLUP(ITEM_EDESC)
                        ORDER BY AMOUNT DESC)";
            else if (topic.ToUpper() == "DIVISION")
                Query = $@"SELECT EDESC,TO_CHAR(AMOUNT,'FM9,999,999,999,999') AMOUNT,TO_CHAR(QUANTITY,'FM9,999,999,999,999') QUANTITY FROM(
                    SELECT FDS.DIVISION_EDESC EDESC, SUM(SSI.NET_SALES) AMOUNT, SUM(SSI.QUANTITY) QUANTITY
                    FROM V$SALES_INVOICE_REPORT3 SSI
                    JOIN FA_DIVISION_SETUP FDS ON FDS.DIVISION_CODE = SSI.DIVISION_CODE AND FDS.COMPANY_CODE = SSI.COMPANY_CODE
                    WHERE SSI.DELETED_FLAG='N' AND SSI.COMPANY_CODE='{companyCode}'
                    AND SSI.SALES_DATE >= (SELECT FN_FIND_BS1STDATE(SYSDATE) FROM DUAL)
                    AND SSI.SALES_DATE <= TO_DATE(SYSDATE)
                    GROUP BY ROLLUP(FDS.DIVISION_EDESC)
                    ORDER BY AMOUNT DESC)";
            else if (topic.ToUpper() == "AREA")
                Query = $@"SELECT EDESC,TO_CHAR(AMOUNT,'FM9,999,999,999,999') AMOUNT,TO_CHAR(QUANTITY,'FM9,999,999,999,999') QUANTITY FROM(
                    SELECT ARS.AREA_EDESC EDESC, SUM(SSI.CALC_TOTAL_PRICE) AMOUNT, SUM(SSI.QUANTITY) QUANTITY
                    FROM SA_SALES_INVOICE SSI
                    JOIN AREA_SETUP ARS ON ARS.AREA_CODE = SSI.AREA_CODE AND ARS.COMPANY_CODE = SSI.COMPANY_CODE
                    WHERE ARS.DELETED_FLAG='N' AND SSI.COMPANY_CODE='{companyCode}' {filter}
                    AND SSI.SALES_DATE >= (SELECT FN_FIND_BS1STDATE(SYSDATE) FROM DUAL)
                    AND SSI.SALES_DATE <= TO_DATE(SYSDATE)
                    GROUP BY ROLLUP(ARS.AREA_EDESC)
                    ORDER BY AMOUNT DESC)";
            else if (topic.ToUpper() == "CUSTOMER")
                Query = $@"SELECT EDESC,TO_CHAR(AMOUNT,'FM9,999,999,999,999') AMOUNT,TO_CHAR(QUANTITY,'FM9,999,999,999,999') QUANTITY FROM(
                    SELECT SCS.CUSTOMER_EDESC EDESC, SUM(VSI.NET_SALES) AMOUNT, SUM(VSI.QUANTITY) QUANTITY
                    FROM V$SALES_INVOICE_REPORT3 VSI
                    JOIN SA_CUSTOMER_SETUP SCS ON VSI.CUSTOMER_CODE = SCS.CUSTOMER_CODE AND VSI.COMPANY_CODE = SCS.COMPANY_CODE
                    WHERE VSI.DELETED_FLAG='N'  AND VSI.COMPANY_CODE='{companyCode}'
                    AND VSI.SALES_DATE >= (SELECT FN_FIND_BS1STDATE(SYSDATE) FROM DUAL)
                    AND VSI.SALES_DATE <= TO_DATE(SYSDATE)
                    GROUP BY ROLLUP(SCS.CUSTOMER_EDESC)
                    ORDER BY AMOUNT DESC)";
            else if (topic.ToUpper() == "DEALER")
                Query = $@"SELECT EDESC,TO_CHAR(AMOUNT,'FM9,999,999,999,999') AMOUNT,TO_CHAR(QUANTITY,'FM9,999,999,999,999') QUANTITY FROM(
                    SELECT IPC.PARTY_TYPE_EDESC EDESC, SUM(VSI.NET_SALES) AMOUNT, SUM(VSI.QUANTITY) QUANTITY
                    FROM V$SALES_INVOICE_REPORT3 VSI
                    JOIN IP_PARTY_TYPE_CODE IPC ON VSI.PARTY_TYPE_CODE = IPC.PARTY_TYPE_CODE AND VSI.COMPANY_CODE = IPC.COMPANY_CODE
                    WHERE VSI.DELETED_FLAG='N'  AND VSI.COMPANY_CODE='{companyCode}'
                    AND VSI.SALES_DATE >= (SELECT FN_FIND_BS1STDATE(SYSDATE) FROM DUAL)
                    AND VSI.SALES_DATE <= TO_DATE(SYSDATE)
                    GROUP BY ROLLUP(IPC.PARTY_TYPE_EDESC)
                    ORDER BY AMOUNT DESC)";
            else
                return new List<TopicSalesModel>();

            var data = _dbContext.SqlQuery<TopicSalesModel>(Query).ToList();

            //placing the total row in last position
            var total = data.FirstOrDefault(x => string.IsNullOrWhiteSpace(x.EDESC));
            if (total != null)
            {
                data.Remove(total);
                total.EDESC = "Total";
                data.Add(total);
            }
            return data;
        }

        public List<TopicSalesModel> GetTopicWiseSalesDaily(string companyCode, string topic)
        {
            var filter = string.Empty;
            var filterQry = $@"SELECT ITEM_CODE FROM MOBILE_ITEM_FILTER WHERE FILTER_TYPE='SALES'";
            var itemFilter = _dbContext.SqlQuery<string>(filterQry).FirstOrDefault();
            if (!string.IsNullOrEmpty(itemFilter))
                filter = $@" AND SSI.ITEM_CODE IN ('{itemFilter}') ";

            string Query = string.Empty;
            if (topic.ToUpper() == "ITEM")
                Query = $@"SELECT EDESC,TO_CHAR(AMOUNT,'FM9,999,999,999,999') AMOUNT,TO_CHAR(QUANTITY,'FM9,999,999,999,999.00') QUANTITY FROM(
                        SELECT ITEM_EDESC EDESC,
                                    SUM(OUT_QUANTITY)-SUM(IN_QUANTITY) QUANTITY,
                                    SUM(OUT_CALC_TOTAL_PRICE)-SUM(IN_CALC_TOTAL_PRICE) AMOUNT
                        FROM V$SALES_SALESRETURN_ANALYSIS WHERE COMPANY_CODE = '{companyCode}'
                        AND VOUCHER_DATE = TO_DATE(SYSDATE)
                        GROUP BY ROLLUP(ITEM_EDESC)
                        ORDER BY AMOUNT DESC)";
            else if (topic.ToUpper() == "DIVISION")
                Query = $@"SELECT EDESC,TO_CHAR(AMOUNT,'FM9,999,999,999,999') AMOUNT,TO_CHAR(QUANTITY,'FM9,999,999,999,999') QUANTITY FROM(
                    SELECT FDS.DIVISION_EDESC EDESC, SUM(SSI.NET_SALES) AMOUNT, SUM(SSI.QUANTITY) QUANTITY
                    FROM V$SALES_INVOICE_REPORT3 SSI
                    JOIN FA_DIVISION_SETUP FDS ON FDS.DIVISION_CODE = SSI.DIVISION_CODE AND FDS.COMPANY_CODE = SSI.COMPANY_CODE
                    WHERE SSI.DELETED_FLAG='N' AND SSI.COMPANY_CODE='{companyCode}'
                    AND SSI.SALES_DATE = TO_DATE(SYSDATE)
                    GROUP BY ROLLUP(FDS.DIVISION_EDESC)
                    ORDER BY AMOUNT DESC)";
            else if (topic.ToUpper() == "AREA")
                Query = $@"SELECT EDESC,TO_CHAR(AMOUNT,'FM9,999,999,999,999') AMOUNT,TO_CHAR(QUANTITY,'FM9,999,999,999,999') QUANTITY FROM(
                    SELECT ARS.AREA_EDESC EDESC, SUM(SSI.CALC_TOTAL_PRICE) AMOUNT, SUM(SSI.QUANTITY) QUANTITY
                    FROM SA_SALES_INVOICE SSI
                    JOIN AREA_SETUP ARS ON ARS.AREA_CODE = SSI.AREA_CODE AND ARS.COMPANY_CODE = SSI.COMPANY_CODE
                    WHERE ARS.DELETED_FLAG='N' AND SSI.COMPANY_CODE='{companyCode}' {filter}
                    AND SSI.SALES_DATE = TO_DATE(SYSDATE)
                    GROUP BY ROLLUP(ARS.AREA_EDESC)
                    ORDER BY AMOUNT DESC)";
            else if (topic.ToUpper() == "CUSTOMER")
                Query = $@"SELECT EDESC,TO_CHAR(AMOUNT,'FM9,999,999,999,999') AMOUNT,TO_CHAR(QUANTITY,'FM9,999,999,999,999') QUANTITY FROM(
                    SELECT SCS.CUSTOMER_EDESC EDESC, SUM(VSI.NET_SALES) AMOUNT, SUM(VSI.QUANTITY) QUANTITY
                    FROM V$SALES_INVOICE_REPORT3 VSI
                    JOIN SA_CUSTOMER_SETUP SCS ON VSI.CUSTOMER_CODE = SCS.CUSTOMER_CODE AND VSI.COMPANY_CODE = SCS.COMPANY_CODE
                    WHERE VSI.DELETED_FLAG='N'  AND VSI.COMPANY_CODE='{companyCode}'
                    AND VSI.SALES_DATE = TO_DATE(SYSDATE)
                    GROUP BY ROLLUP(SCS.CUSTOMER_EDESC)
                    ORDER BY AMOUNT DESC)";
            else if (topic.ToUpper() == "DEALER")
                Query = $@"SELECT EDESC,TO_CHAR(AMOUNT,'FM9,999,999,999,999') AMOUNT,TO_CHAR(QUANTITY,'FM9,999,999,999,999') QUANTITY FROM(
                    SELECT IPC.PARTY_TYPE_EDESC EDESC, SUM(VSI.NET_SALES) AMOUNT, SUM(VSI.QUANTITY) QUANTITY
                    FROM V$SALES_INVOICE_REPORT3 VSI
                    JOIN IP_PARTY_TYPE_CODE IPC ON VSI.PARTY_TYPE_CODE = IPC.PARTY_TYPE_CODE AND VSI.COMPANY_CODE = IPC.COMPANY_CODE
                    WHERE VSI.DELETED_FLAG='N'  AND VSI.COMPANY_CODE='{companyCode}'
                    AND VSI.SALES_DATE = TO_DATE(SYSDATE)
                    GROUP BY ROLLUP(IPC.PARTY_TYPE_EDESC)
                    ORDER BY AMOUNT DESC)";
            else
                return new List<TopicSalesModel>();

            var data = _dbContext.SqlQuery<TopicSalesModel>(Query).ToList();

            //placing the total row in last position
            var total = data.FirstOrDefault(x => string.IsNullOrWhiteSpace(x.EDESC));
            if (total != null)
            {
                data.Remove(total);
                total.EDESC = "Total";
                data.Add(total);
            }
            return data;
        }


        public List<TopicSalesModel> GetTopicWiseSalesQuaterly(string companyCode, string topic, string Quater)
        {
            var filter = string.Empty;
            var filterQry = $@"SELECT ITEM_CODE FROM MOBILE_ITEM_FILTER WHERE FILTER_TYPE='SALES'";
            var itemFilter = _dbContext.SqlQuery<string>(filterQry).FirstOrDefault();
            if (!string.IsNullOrEmpty(itemFilter))
                filter = $@" AND SSI.ITEM_CODE IN ('{itemFilter}') ";

            string Query = string.Empty;
            string QuaterQuery = string.Empty;
            if (Quater == "Q1")
                QuaterQuery = " '16-JUL-2017' AND '17-OCT-2017'";
            else if (Quater == "Q2")
                QuaterQuery = " '18-OCT-2017' AND '14-JAN-2018'";
            else if (Quater == "Q3")
                QuaterQuery = " '15-JAN-2018' AND '13-APR-2018'";
            else if (Quater == "Q4")
                QuaterQuery = " '14-APR-2018' AND '16-JUL-2018'";

            if (topic.ToUpper() == "ITEM")
                Query = $@"SELECT EDESC,TO_CHAR(AMOUNT,'FM9,999,999,999,999') AMOUNT,TO_CHAR(QUANTITY,'FM9,999,999,999,999.00') QUANTITY FROM(
                        SELECT ITEM_EDESC EDESC,
                                    SUM(OUT_QUANTITY)-SUM(IN_QUANTITY) QUANTITY,
                                    SUM(OUT_CALC_TOTAL_PRICE)-SUM(IN_CALC_TOTAL_PRICE) AMOUNT
                        FROM V$SALES_SALESRETURN_ANALYSIS WHERE COMPANY_CODE = '{companyCode}'
                        AND VOUCHER_DATE BETWEEN {QuaterQuery}
                        GROUP BY ROLLUP(ITEM_EDESC)
                        ORDER BY AMOUNT DESC)";
            else if (topic.ToUpper() == "DIVISION")
                Query = $@"SELECT EDESC,TO_CHAR(AMOUNT,'FM9,999,999,999,999') AMOUNT,TO_CHAR(QUANTITY,'FM9,999,999,999,999') QUANTITY FROM(
                    SELECT FDS.DIVISION_EDESC EDESC, SUM(SSI.NET_SALES) AMOUNT, SUM(SSI.QUANTITY) QUANTITY
                    FROM V$SALES_INVOICE_REPORT3 SSI
                    JOIN FA_DIVISION_SETUP FDS ON FDS.DIVISION_CODE = SSI.DIVISION_CODE AND FDS.COMPANY_CODE = SSI.COMPANY_CODE
                    WHERE SSI.DELETED_FLAG='N' AND SSI.COMPANY_CODE='{companyCode}'
                    AND SSI.SALES_DATE BETWEEN {QuaterQuery}
                    GROUP BY ROLLUP(FDS.DIVISION_EDESC)
                    ORDER BY AMOUNT DESC)";
            else if (topic.ToUpper() == "AREA")
                Query = $@"SELECT EDESC,TO_CHAR(AMOUNT,'FM9,999,999,999,999') AMOUNT,TO_CHAR(QUANTITY,'FM9,999,999,999,999') QUANTITY FROM(
                    SELECT ARS.AREA_EDESC EDESC, SUM(SSI.CALC_TOTAL_PRICE) AMOUNT, SUM(SSI.QUANTITY) QUANTITY
                    FROM SA_SALES_INVOICE SSI
                    JOIN AREA_SETUP ARS ON ARS.AREA_CODE = SSI.AREA_CODE AND ARS.COMPANY_CODE = SSI.COMPANY_CODE
                    WHERE ARS.DELETED_FLAG='N' AND SSI.COMPANY_CODE='{companyCode}'
                    AND SSI.SALES_DATE BETWEEN {QuaterQuery} {filter}
                    GROUP BY ROLLUP(ARS.AREA_EDESC)
                    ORDER BY AMOUNT DESC)";
            else if (topic.ToUpper() == "CUSTOMER")
                Query = $@"SELECT EDESC,TO_CHAR(AMOUNT,'FM9,999,999,999,999') AMOUNT,TO_CHAR(QUANTITY,'FM9,999,999,999,999') QUANTITY FROM(
                    SELECT SCS.CUSTOMER_EDESC EDESC, SUM(VSI.NET_SALES) AMOUNT, SUM(VSI.QUANTITY) QUANTITY
                    FROM V$SALES_INVOICE_REPORT3 VSI
                    JOIN SA_CUSTOMER_SETUP SCS ON VSI.CUSTOMER_CODE = SCS.CUSTOMER_CODE AND VSI.COMPANY_CODE = SCS.COMPANY_CODE
                    WHERE VSI.DELETED_FLAG='N'  AND VSI.COMPANY_CODE='{companyCode}'
                    AND VSI.SALES_DATE BETWEEN {QuaterQuery}
                    GROUP BY ROLLUP(SCS.CUSTOMER_EDESC)
                    ORDER BY AMOUNT DESC)";
            else if (topic.ToUpper() == "DEALER")
                Query = $@"SELECT EDESC,TO_CHAR(AMOUNT,'FM9,999,999,999,999') AMOUNT,TO_CHAR(QUANTITY,'FM9,999,999,999,999') QUANTITY FROM(
                    SELECT IPC.PARTY_TYPE_EDESC EDESC, SUM(VSI.NET_SALES) AMOUNT, SUM(VSI.QUANTITY) QUANTITY
                    FROM V$SALES_INVOICE_REPORT3 VSI
                    JOIN IP_PARTY_TYPE_CODE IPC ON VSI.PARTY_TYPE_CODE = IPC.PARTY_TYPE_CODE AND VSI.COMPANY_CODE = IPC.COMPANY_CODE
                    WHERE VSI.DELETED_FLAG='N'  AND VSI.COMPANY_CODE='{companyCode}'
                    AND VSI.SALES_DATE  BETWEEN {QuaterQuery}
                    GROUP BY ROLLUP(IPC.PARTY_TYPE_EDESC)
                    ORDER BY AMOUNT DESC)";
            else if (topic.ToUpper() == "MONTH")
                Query = $@"select 
                           EDESC,
                           TO_CHAR(SUM(AMOUNT),'FM9,999,999,999,999') AMOUNT,
                           TO_CHAR(SUM(QUANTITY),'FM9,999,999,999,999') QUANTITY 
                           FROM
                           (
                                   SELECT
                                        TRIM (SUBSTR (FN_CHARTBS_MONTH (SUBSTR (BS_DATE (SI.sales_date), 6, 2)),5,20))EDESC,
                                        NVL(SUM (NVL (SI.total_sales, 0)) / 1, 0) AS AMOUNT,
                                        SUM (NVL(SI.quantity, 0)) / 1 AS QUANTITY
                                    FROM  V$SALES_INVOICE_REPORT3 SI
                                    where SI.deleted_flag='N'
                                    AND SI.COMPANY_CODE ='{companyCode}'
                                    AND SI.SALES_DATE BETWEEN  {QuaterQuery}
                                    group by  FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SI.Sales_date),6,2)),substr(bs_date(SI.sales_date),6,2)
                                    ORDER BY substr(bs_date(SI.sales_date),6,2)
                          ) GROUP BY ROLLUP(EDESC)";
                            else if (topic.ToUpper() == "CATEGORY")
                                Query = $@"SELECT ICC.CATEGORY_EDESC EDESC,
                            TO_CHAR(SUM(SSI.CALC_TOTAL_PRICE),'FM99,999,999,999,999') AMOUNT, 
                            TO_CHAR(SUM(SSI.QUANTITY),'FM99,999,999,999,999') QUANTITY
                FROM SA_SALES_INVOICE SSI
                INNER JOIN IP_ITEM_MASTER_SETUP IMS ON SSI.ITEM_CODE = IMS.ITEM_CODE AND SSI.COMPANY_CODE = IMS.COMPANY_CODE
                INNER JOIN IP_CATEGORY_CODE ICC ON IMS.CATEGORY_CODE = ICC.CATEGORY_CODE AND IMS.COMPANY_CODE = ICC.COMPANY_CODE
                WHERE SSI.DELETED_FLAG='N'
                    AND SSI.COMPANY_CODE='{companyCode}'
                    AND SSI.SALES_DATE BETWEEN  {QuaterQuery} {filter}
                GROUP BY ROLLUP(ICC.CATEGORY_EDESC)";
            else
                return new List<TopicSalesModel>();

            var data = _dbContext.SqlQuery<TopicSalesModel>(Query).ToList();

            //placing the total row in last position
            var total = data.FirstOrDefault(x => string.IsNullOrWhiteSpace(x.EDESC));
            if (total != null)
            {
                data.Remove(total);
                total.EDESC = "Total";
                data.Add(total);
            }
            return data;
        }

        public List<TopicSalesModel> GetTopicWiseQuaterlySales(string companyCode, string topic, string Quater)
        {
            var filter = string.Empty;
            var filterQry = $@"SELECT ITEM_CODE FROM MOBILE_ITEM_FILTER WHERE FILTER_TYPE='SALES'";
            var itemFilter = _dbContext.SqlQuery<string>(filterQry).FirstOrDefault();
            if (!string.IsNullOrEmpty(itemFilter))
                filter = $@" AND SSI.ITEM_CODE IN ('{itemFilter}') ";

            string Query = string.Empty;
            string QuaterQuery = string.Empty;
            if (Quater == "Q1")
                QuaterQuery = " '16-JUL-2017' AND '17-OCT-2017'";
            else if (Quater == "Q2")
                QuaterQuery = " '18-OCT-2017' AND '14-JAN-2018'";
            else if (Quater == "Q3")
                QuaterQuery = " '15-JAN-2018' AND '13-APR-2018'";
            else if (Quater == "Q4")
                QuaterQuery = " '14-APR-2018' AND '16-JUL-2018'";

             if (topic.ToUpper() == "AREA")
                Query = $@"SELECT EDESC,TO_CHAR(AMOUNT,'FM9,999,999,999,999') AMOUNT,TO_CHAR(QUANTITY,'FM9,999,999,999,999') QUANTITY FROM(
                    SELECT ARS.AREA_EDESC EDESC, SUM(SSI.CALC_TOTAL_PRICE) AMOUNT, SUM(SSI.QUANTITY) QUANTITY
                    FROM SA_SALES_INVOICE SSI
                    JOIN AREA_SETUP ARS ON ARS.AREA_CODE = SSI.AREA_CODE AND ARS.COMPANY_CODE = SSI.COMPANY_CODE
                    WHERE ARS.DELETED_FLAG='N' AND SSI.COMPANY_CODE='{companyCode}'
                    AND SSI.SALES_DATE BETWEEN {QuaterQuery} {filter}
                    GROUP BY ROLLUP(ARS.AREA_EDESC)
                    ORDER BY AMOUNT DESC)";
            else if (topic.ToUpper() == "MONTH")
                Query = $@"select 
           EDESC,
           TO_CHAR(AMOUNT,'FM9,999,999,999,999') AMOUNT,
           TO_CHAR(QUANTITY,'FM9,999,999,999,999') QUANTITY 
           FROM
           (
                   SELECT
                        TRIM (SUBSTR (FN_CHARTBS_MONTH (SUBSTR (BS_DATE (SI.sales_date), 6, 2)),5,20))EDESC,
                        NVL(SUM (NVL (SI.total_sales, 0)) / 1, 0) AS AMOUNT,
                        SUM (NVL(SI.quantity, 0)) / 1 AS QUANTITY
                    FROM  V$SALES_INVOICE_REPORT3 SI
                    where SI.deleted_flag='N'
                    AND SI.COMPANY_CODE ='{companyCode}'
                    AND SI.SALES_DATE BETWEEN  {QuaterQuery}
                    group by  FN_CHARTBS_MONTH(SUBSTR(BS_DATE(SI.Sales_date),6,2)),substr(bs_date(SI.sales_date),6,2)
                    ORDER BY substr(bs_date(SI.sales_date),6,2)
          )";
            else if (topic.ToUpper() == "CATEGORY")
                Query = $@"SELECT ICC.CATEGORY_EDESC EDESC,
            TO_CHAR(SUM(SSI.CALC_TOTAL_PRICE),'FM99,999,999,999,999') AMOUNT, 
            TO_CHAR(SUM(SSI.QUANTITY),'FM99,999,999,999,999') QUANTITY
FROM SA_SALES_INVOICE SSI
INNER JOIN IP_ITEM_MASTER_SETUP IMS ON SSI.ITEM_CODE = IMS.ITEM_CODE AND SSI.COMPANY_CODE = IMS.COMPANY_CODE
INNER JOIN IP_CATEGORY_CODE ICC ON IMS.CATEGORY_CODE = ICC.CATEGORY_CODE AND IMS.COMPANY_CODE = ICC.COMPANY_CODE
WHERE SSI.DELETED_FLAG='N'
    AND SSI.COMPANY_CODE='{companyCode}'
    AND SSI.SALES_DATE BETWEEN  {QuaterQuery} {filter}
GROUP BY ICC.CATEGORY_EDESC";
            else
                return new List<TopicSalesModel>();

            var data = _dbContext.SqlQuery<TopicSalesModel>(Query).ToList();

            //placing the total row in last position
            var total = data.FirstOrDefault(x => string.IsNullOrWhiteSpace(x.EDESC));
            if (total != null)
            {
                data.Remove(total);
                total.EDESC = "Total";
                data.Add(total);
            }
            return data;
        }

        public List<PapModel> GetPap(string company_code)
        {
            var query = $@"SELECT FN_FETCH_DESC(COMPANY_CODE,'FA_CHART_OF_ACCOUNTS_SETUP',ACC_CODE) ACC_NAME, SUB_EDESC, CHEQUE_NO, TO_CHAR(DR_AMOUNT,'FM9,999,999,999,999.00') DR_AMOUNT
                FROM V$VIRTUAL_PAY_ORDER WHERE COMPANY_CODE='{company_code}'
                AND VOUCHER_NO IN(SELECT VOUCHER_NO FROM MASTER_TRANSACTION WHERE CHECKED_BY IS NULL AND AUTHORISED_BY IS NULL
                AND FORM_CODE IN(SELECT DISTINCT FORM_CODE FROM V$VIRTUAL_PAY_ORDER))
                UNION ALL
                SELECT 'Total' ACC_NAME,''SUB_EDESC,''CHEQUE_NO,TO_CHAR(SUM(DR_AMOUNT),'FM9,999,999,999,999.00') DR_AMOUNT FROM (
                SELECT FN_FETCH_DESC(COMPANY_CODE,'FA_CHART_OF_ACCOUNTS_SETUP',ACC_CODE) ACC_NAME, SUB_EDESC, CHEQUE_NO, DR_AMOUNT
                                FROM V$VIRTUAL_PAY_ORDER WHERE COMPANY_CODE='{company_code}'
                                AND VOUCHER_NO IN(SELECT VOUCHER_NO FROM MASTER_TRANSACTION WHERE CHECKED_BY IS NULL AND AUTHORISED_BY IS NULL
                                AND FORM_CODE IN(SELECT DISTINCT FORM_CODE FROM V$VIRTUAL_PAY_ORDER)))";

            //var query = $@"SELECT FN_FETCH_DESC(COMPANY_CODE,'FA_CHART_OF_ACCOUNTS_SETUP',ACC_CODE) ACC_NAME, SUB_EDESC, CHEQUE_NO, DR_AMOUNT
            //    FROM V$VIRTUAL_PAY_ORDER WHERE COMPANY_CODE='{company_code}'
            //    AND VOUCHER_NO IN(SELECT VOUCHER_NO FROM MASTER_TRANSACTION WHERE CHECKED_BY IS NULL AND AUTHORISED_BY IS NULL
            //    AND FORM_CODE IN(SELECT DISTINCT FORM_CODE FROM V$VIRTUAL_PAY_ORDER))";
            var data = _dbContext.SqlQuery<PapModel>(query).ToList();
            return data;
        }

        public List<PopModel> GetPop(string company_code)
        {
            //var query = $@"SELECT * FROM (SELECT SUPPLIER_EDESC, ITEM_EDESC, MU_CODE, TO_CHAR(SUM(NVL(IN_QUANTITY,0)),'FM9,999,999,999,999.00') ORDER_QTY,
            //                            TO_CHAR(SUM(NVL(TOTAL_PRICE,0)),'FM9,999,999,999,999.00') AMOUNT,
            //                            TO_CHAR(SUM(NVL(TOTAL_PRICE,0))/SUM(NVL(IN_QUANTITY,0)),'FM9,999,999,999,999.00') AVG_RATE
            //            FROM V$PURCHASE_ORDER_ANALYSIS
            //            WHERE COMPANY_CODE='{company_code}'
            //            AND DELETED_FLAG='N'
            //            AND DUE_QTY <> 0
            //            GROUP BY SUPPLIER_EDESC, ITEM_EDESC, MU_CODE
            //            ORDER BY SUPPLIER_EDESC, ITEM_EDESC)
            //            UNION ALL
            //            SELECT 'Total' SUPPLIER_EDESC,'' ITEM_EDESC,'' MU_CODE,
            //                   '' ORDER_QTY, TO_CHAR(SUM(AMOUNT),'FM9,999,999,999,999.00') AMOUNT,'' AVG_RATE
            //                   FROM(SELECT SUM(NVL(TOTAL_PRICE,0)) AMOUNT
            //                   FROM V$PURCHASE_ORDER_ANALYSIS
            //                   WHERE COMPANY_CODE='{company_code}'
            //                   AND DELETED_FLAG='N'
            //                   AND DUE_QTY <> 0
            //                   GROUP BY SUPPLIER_EDESC, ITEM_EDESC, MU_CODE)";

            var newQuery = $@"SELECT SUPPLIER_EDESC, ITEM_EDESC, MU_CODE, ROUND(SUM(NVL(IN_QUANTITY,0)),3) ORDER_QTY,
                                        ROUND(SUM(NVL(TOTAL_PRICE,0)),3) AMOUNT,
                                        ROUND(SUM(NVL(TOTAL_PRICE,0))/SUM(NVL(IN_QUANTITY,0)),3) AVG_RATE
                        FROM V$PURCHASE_ORDER_ANALYSIS
                        WHERE COMPANY_CODE='{company_code}'
                        AND DELETED_FLAG='N'
                        AND DUE_QTY <> 0
                        GROUP BY SUPPLIER_EDESC, ITEM_EDESC, MU_CODE
                        ORDER BY SUPPLIER_EDESC, ITEM_EDESC";
            var data = _dbContext.SqlQuery<PopModel>(newQuery).ToList();
            return data;
        }

        public List<PrpModel> GetPrp(string company_code)
        {
            var query = $@"SELECT VIA.ITEM_EDESC,VIA.MU_CODE,VIA.FROM_LOCATION_EDESC,VIA.TO_LOCATION_EDESC,
                        TRUNC(SUM(NVL(VIA.QUANTITY,0)),0) ORDER_QTY,
                        TRUNC(SUM(NVL(VIA.TOTAL_PRICE,0)),0) AMOUNT,
                        NVL(IMS.PURCHASE_PRICE,0) PURCHASE_PRICE
                        --TRUNC(SUM(NVL(VIA.TOTAL_PRICE,0))/SUM(NVL(QUANTITY,0)),0) AVG_RATE
            FROM V$PURCHASE_INDENT_ANALYSIS VIA
            INNER JOIN IP_ITEM_MASTER_SETUP IMS ON IMS.ITEM_CODE = VIA.ITEM_CODE AND IMS.COMPANY_CODE = VIA.COMPANY_CODE
            WHERE VIA.COMPANY_CODE='{company_code}'
            AND VIA.DELETED_FLAG='N'
            AND VIA.DUE_QTY <> 0
            GROUP BY VIA.ITEM_EDESC, VIA.MU_CODE,VIA.FROM_LOCATION_EDESC,VIA.TO_LOCATION_EDESC,IMS.PURCHASE_PRICE
            ORDER BY VIA.FROM_LOCATION_EDESC,VIA.TO_LOCATION_EDESC";
            var data = _dbContext.SqlQuery<PrpModel>(query).ToList();
            return data;
        }

        public List<AccountSummaryModel> GetSubLedgerWithFilter(string company_code,string filtername,string formdate,string todate)
        {
            try
            {
                var url = HttpContext.Current.Request.Url.AbsoluteUri;

                string xmlpath = HttpContext.Current.Server.MapPath("~/App_Data/MobileLedgerFilter.xml");
                var xml = XDocument.Load(xmlpath);
                var condition_query = from c in xml.Root.Descendants("Vendor")
                                      where (string)c.Attribute("PlanName") == filtername
                                      select c.Element("ConditionQuery").Value;

                var test = from c in xml.Root.Descendants("Vendor") select (string)c.Attribute("PlanName");
                var result = condition_query.FirstOrDefault();

                    var query = $@"SELECT  SUB_CODE SUBCODE,  SUB_EDESC SUBEDESC, COMPANY_CODE COMPANYCODE,   SUM(NVL(DR_OPENING,0)) DROPENING, SUM(NVL(CR_OPENING,0)) CROPENING, SUM(NVL(DR_AMOUNT,0)) DRAMOUNT,  SUM(NVL(CR_AMOUNT,0)) CRAMOUNT ,
                            CASE
                        WHEN((SUM(NVL(DR_OPENING, 0)) + SUM(NVL(DR_AMOUNT, 0))) - (SUM(NVL(CR_OPENING, 0)) + SUM(NVL(CR_AMOUNT, 0)))) >= 0 THEN
                            ((SUM(NVL(DR_OPENING, 0)) + SUM(NVL(DR_AMOUNT, 0))) - (SUM(NVL(CR_OPENING, 0)) + SUM(NVL(CR_AMOUNT, 0))))
                        ELSE
                            0
                        END
                        AS DRBALANCE,
                        CASE
                          WHEN((SUM(NVL(DR_OPENING, 0)) + SUM(NVL(DR_AMOUNT, 0))) - (SUM(NVL(CR_OPENING, 0)) + SUM(NVL(CR_AMOUNT, 0)))) < 0 THEN
                             ABS(((SUM(NVL(DR_OPENING, 0)) + SUM(NVL(DR_AMOUNT, 0))) - (SUM(NVL(CR_OPENING, 0)) + SUM(NVL(CR_AMOUNT, 0)))))
                          ELSE
                             0
                        END AS CRBALANCE
                        FROM(
                        SELECT SUB_CODE, SUB_EDESC, COMPANY_CODE,
                        CASE
                        WHEN SUM(NVL(DR_AMOUNT * EXCHANGE_RATE, 0) - NVL(CR_AMOUNT * EXCHANGE_RATE, 0)) >= 0 THEN
                            SUM(NVL(DR_AMOUNT * EXCHANGE_RATE, 0) - NVL(CR_AMOUNT * EXCHANGE_RATE, 0))
                        ELSE
                            0
                        END
                        AS DR_OPENING,
                        CASE
                          WHEN SUM(NVL(DR_AMOUNT * EXCHANGE_RATE, 0) - NVL(CR_AMOUNT * EXCHANGE_RATE, 0)) < 0 THEN
                             ABS(SUM(NVL(DR_AMOUNT * EXCHANGE_RATE, 0) - NVL(CR_AMOUNT * EXCHANGE_RATE, 0)))
                          ELSE
                             0
                        END AS CR_OPENING
                        , 0 DR_AMOUNT, 0 CR_AMOUNT
                        FROM V$VIRTUAL_SUB_LEDGER
                        WHERE   COMPANY_CODE = '{company_code}'
                        {result}
                        AND DELETED_FLAG = 'N'
                        AND(VOUCHER_DATE < TO_DATE('{todate}', 'YYYY-MM-DD') OR FORM_CODE = '0')
                        GROUP BY  SUB_CODE, SUB_EDESC, COMPANY_CODE
                        UNION ALL
                        SELECT  SUB_CODE, SUB_EDESC, COMPANY_CODE, 0 DR_OPENING, 0 CR_OPENING, SUM(NVL(DR_AMOUNT * EXCHANGE_RATE, 0)) DR_AMOUNT, SUM(NVL(CR_AMOUNT * EXCHANGE_RATE, 0))  CR_AMOUNT
                        FROM V$VIRTUAL_SUB_LEDGER
                        WHERE COMPANY_CODE = '{company_code}'
                        AND DELETED_FLAG = 'N'
                        {result}
                        AND FORM_CODE <> '0'
                        AND VOUCHER_DATE >= TO_DATE('{formdate}', 'YYYY-MM-DD')
                        AND VOUCHER_DATE <= TO_DATE('{todate}', 'YYYY-MM-DD')
                        GROUP BY  SUB_CODE, SUB_EDESC, COMPANY_CODE)
                        GROUP BY  SUB_CODE,  SUB_EDESC,  COMPANY_CODE order by SUB_EDESC";

                var data = _dbContext.SqlQuery<AccountSummaryModel>(query).ToList();
                return data;

            }
            catch(Exception ex)
            {
                throw new System.ArgumentException(ex.Message, "original");
            }
        }

        public List<TopicSalesModel> GetAccountHeadBalance(string company_code, string branch_code)
        {
            var result = new List<TopicSalesModel>();
            try
            {
                var url = HttpContext.Current.Request.Url.AbsoluteUri;

                string xmlpath = HttpContext.Current.Server.MapPath("~/App_Data/MobileAccountHeadFilter.xml");
                var xml = XDocument.Load(xmlpath);
                var Vendors = from c in xml.Root.Descendants("Vendor") select c;
                foreach(var ven in Vendors)
                {
                    var condition = ven.Element("ConditionQuery").Value;
                    var conditionName = (string)ven.Attribute("ConditionName");
                    var query = string.Empty;
                    if (conditionName.ToUpper().Equals("DR".ToUpper()))
                    {
                         query = $@"SELECT TO_CHAR(SUM(SUM(NVL(DR_AMOUNT,0) - NVL(CR_AMOUNT,0))),'FM9,999,999,999,999.00') BALANCE FROM V$VIRTUAL_GENERAL_LEDGER
                                 WHERE
                                  ACC_TYPE_FLAG = 'T'
                                  {condition}
                                GROUP BY ACC_CODE, ACC_EDESC";
                    }
                    else
                    {
                        query = $@"SELECT TO_CHAR(SUM(SUM(NVL(CR_AMOUNT,0) - NVL(DR_AMOUNT,0))),'FM9,999,999,999,999.00') BALANCE FROM V$VIRTUAL_GENERAL_LEDGER
                                 WHERE
                                  ACC_TYPE_FLAG = 'T'
                                  {condition}
                                GROUP BY ACC_CODE, ACC_EDESC";
                    }
                    var data = _dbContext.SqlQuery<string>(query).FirstOrDefault();
                    result.Add(new TopicSalesModel
                    {
                        EDESC = (string)ven.Attribute("PlanName"),
                        AMOUNT = data
                    });

                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message, "original");
            }
            return result;
        }

        public List<AccountSummaryModel> GetAccountHeadBalanceTopic(string company_code, string filtername, string topic)
        {
            try
            {
                var url = HttpContext.Current.Request.Url.AbsoluteUri;

                string xmlpath = HttpContext.Current.Server.MapPath("~/App_Data/MobileAccountHeadFilter.xml");
                var xml = XDocument.Load(xmlpath);
                var result = xml.Root.Descendants("Vendor").Where(x => (string)x.Attribute("PlanName") == filtername).FirstOrDefault().Element("ConditionQuery").Value;
                var dateFilter = "";
                if (topic.ToLower() == "MTD".ToLower())
                    dateFilter = " AND VOUCHER_DATE >= (SELECT FN_FIND_BS1STDATE(SYSDATE) FROM DUAL) AND VOUCHER_DATE <= TO_DATE(SYSDATE)";

                var query = $@"SELECT  SUB_CODE SUBCODE,  SUB_EDESC SUBEDESC, COMPANY_CODE COMPANYCODE,   SUM(NVL(DR_OPENING,0)) DROPENING, SUM(NVL(CR_OPENING,0)) CROPENING, SUM(NVL(DR_AMOUNT,0)) DRAMOUNT,  SUM(NVL(CR_AMOUNT,0)) CRAMOUNT ,
                            CASE
                        WHEN((SUM(NVL(DR_OPENING, 0)) + SUM(NVL(DR_AMOUNT, 0))) - (SUM(NVL(CR_OPENING, 0)) + SUM(NVL(CR_AMOUNT, 0)))) >= 0 THEN
                            ((SUM(NVL(DR_OPENING, 0)) + SUM(NVL(DR_AMOUNT, 0))) - (SUM(NVL(CR_OPENING, 0)) + SUM(NVL(CR_AMOUNT, 0))))
                        ELSE
                            0
                        END
                        AS DRBALANCE,
                        CASE
                          WHEN((SUM(NVL(DR_OPENING, 0)) + SUM(NVL(DR_AMOUNT, 0))) - (SUM(NVL(CR_OPENING, 0)) + SUM(NVL(CR_AMOUNT, 0)))) < 0 THEN
                             ABS(((SUM(NVL(DR_OPENING, 0)) + SUM(NVL(DR_AMOUNT, 0))) - (SUM(NVL(CR_OPENING, 0)) + SUM(NVL(CR_AMOUNT, 0)))))
                          ELSE
                             0
                        END AS CRBALANCE
                        FROM(
                        SELECT SUB_CODE, SUB_EDESC, COMPANY_CODE,
                        CASE
                        WHEN SUM(NVL(DR_AMOUNT * EXCHANGE_RATE, 0) - NVL(CR_AMOUNT * EXCHANGE_RATE, 0)) >= 0 THEN
                            SUM(NVL(DR_AMOUNT * EXCHANGE_RATE, 0) - NVL(CR_AMOUNT * EXCHANGE_RATE, 0))
                        ELSE
                            0
                        END
                        AS DR_OPENING,
                        CASE
                          WHEN SUM(NVL(DR_AMOUNT * EXCHANGE_RATE, 0) - NVL(CR_AMOUNT * EXCHANGE_RATE, 0)) < 0 THEN
                             ABS(SUM(NVL(DR_AMOUNT * EXCHANGE_RATE, 0) - NVL(CR_AMOUNT * EXCHANGE_RATE, 0)))
                          ELSE
                             0
                        END AS CR_OPENING
                        , 0 DR_AMOUNT, 0 CR_AMOUNT
                        FROM V$VIRTUAL_SUB_LEDGER
                        WHERE   COMPANY_CODE = '{company_code}'
                        {result}
                        AND DELETED_FLAG = 'N'
                        AND(VOUCHER_DATE < TO_DATE(SYSDATE) OR FORM_CODE = '0')
                        GROUP BY  SUB_CODE, SUB_EDESC, COMPANY_CODE
                        UNION ALL
                        SELECT  SUB_CODE, SUB_EDESC, COMPANY_CODE, 0 DR_OPENING, 0 CR_OPENING, SUM(NVL(DR_AMOUNT * EXCHANGE_RATE, 0)) DR_AMOUNT, SUM(NVL(CR_AMOUNT * EXCHANGE_RATE, 0))  CR_AMOUNT
                        FROM V$VIRTUAL_SUB_LEDGER
                        WHERE COMPANY_CODE = '{company_code}'
                        AND DELETED_FLAG = 'N'
                        {result}
                        AND FORM_CODE <> '0'
                        {dateFilter}
                        GROUP BY  SUB_CODE, SUB_EDESC, COMPANY_CODE)
                        GROUP BY  SUB_CODE,  SUB_EDESC,  COMPANY_CODE order by SUB_EDESC";

                var data = _dbContext.SqlQuery<AccountSummaryModel>(query).ToList();
                return data;

            }
            catch (Exception ex)
            {
                throw new System.ArgumentException(ex.Message, "original");
            }
        }

        public List<string> GetLedgerFiler()
        {
            try
            {
                var url = HttpContext.Current.Request.Url.AbsoluteUri;

                string xmlpath = HttpContext.Current.Server.MapPath("~/App_Data/MobileLedgerFilter.xml");
                var xml = XDocument.Load(xmlpath);

                var data = from c in xml.Root.Descendants("Vendor") select (string)c.Attribute("PlanName");
                var result = data.ToList();
                return result;
            }
            catch(Exception ex)
            {
                throw new System.ArgumentException(ex.Message, "original");
            }
            }


        public List<SalesTargetGraphMobile> GetMonthlySalesVsTarget(string company_code,string userCode)
        {
            var filter = string.Empty;
            var filterQry = $@"SELECT ITEM_CODE FROM MOBILE_ITEM_FILTER WHERE FILTER_TYPE='SALES'";
            var itemFilter = _dbContext.SqlQuery<string>(filterQry).FirstOrDefault();
            if (!string.IsNullOrEmpty(itemFilter))
                filter = $@" AND SI.ITEM_CODE IN ('{itemFilter}') ";

            var query = $@"SELECT SUBSTR (bs_date (SI.sales_date), 1, 4) as Year, DS.BRANCH_EDESC as branch_name, DS.BRANCH_CODE as branch_code,
                            fn_bs_month(substr(bs_date(si.sales_date),6,2)) AS Month,
                            SUBSTR(BS_DATE(SI.SALES_DATE),6,2) as MonthInt,
                           0 TargetQty,
                             0 TargetAmount,
                            SUM (nvl(si.calc_Quantity,0))/1 AS Quantity,
                            SUM (nvl(si.calc_total_price,0))/1 AS GrossAmount,
                            'SALES' DATATYPE
                    FROM sa_sales_invoice si, FA_BRANCH_SETUP DS
                    WHERE  1=1
                    AND si.deleted_flag = 'N' AND si.company_code in ('{company_code}') {filter}
                    and SI.BRANCH_CODE = DS.BRANCH_CODE
                    GROUP BY DS.BRANCH_CODE, DS.BRANCH_EDESC, SUBSTR(BS_DATE(SI.SALES_DATE),6,2)
                    ,fn_bs_month(substr(bs_date(si.sales_date),6,2))
                    ,to_number(substr(bs_date(sales_date),6,2)),SUBSTR (bs_date (SI.sales_date), 1, 4)
                    UNION  ALL
                    SELECT SUBSTR (bs_date (SI.PLAN_DATE), 1, 4) as Year,DS.BRANCH_EDESC as branch_name, DS.BRANCH_CODE as branch_code,
                            fn_bs_month(substr(bs_date(si.PLAN_DATE),6,2)) AS Month,
                            SUBSTR(BS_DATE(SI.PLAN_DATE),6,2) as MonthInt,
                            SUM (nvl(si.PER_DAY_QUANTITY,0))/1 AS TargetQty,
                            SUM (nvl(si.PER_DAY_AMOUNT,0))/1 AS TargetAmount,
                            0 Quantity,
                            0 GrossAmount,
                               'TARGET' DATATYPE
                    FROM PL_SALES_PLAN_DTL si, FA_BRANCH_SETUP DS,IP_ITEM_MASTER_SETUP I
                    WHERE 1=1
                    AND si.deleted_flag = 'N' AND si.company_code in ('{company_code}') {filter}
                    AND I.ITEM_CODE=SI.ITEM_CODE
                    AND I.GROUP_SKU_FLAG='I'
                    and SI.BRANCH_CODE = DS.BRANCH_CODE
                       GROUP BY DS.BRANCH_CODE, DS.BRANCH_EDESC, SUBSTR(BS_DATE(SI.PLAN_DATE),6,2)
                    ,fn_bs_month(substr(bs_date(si.PLAN_DATE),6,2))
                    ,to_number(substr(bs_date(PLAN_DATE),6,2)),SUBSTR (bs_date (SI.PLAN_DATE), 1, 4) ";

            var Targetdata = _dbContext.SqlQuery<SalesTargetViewModelMobile>(query).ToList();
            var branchwisesales = new List<SalesTargetGraphMobile>();
            foreach (var data in Targetdata.Where(x => x.DataType == "SALES" || x.DataType == "TARGET").OrderBy(x => x.MonthInt).GroupBy(x => x.MonthInt).Select(x => x.FirstOrDefault()).ToList())
            {
                var branchWise = new SalesTargetGraphMobile();
                branchWise.Month = data.Month;
                branchWise.MonthInt = data.MonthInt;
                branchWise.MonthYear = data.Month;
                branchWise.Sales = Convert.ToDecimal(Targetdata.Where(x => x.DataType == "SALES" && x.MonthInt == data.MonthInt).Sum(X => X.GrossAmount));
                branchWise.Target = Convert.ToDecimal(Targetdata.Where(x => x.DataType != "SALES" && x.MonthInt == data.MonthInt).Sum(X => X.TargetAmount));
                branchWise.Qty_Sales = Convert.ToDecimal(Targetdata.Where(x => x.DataType == "SALES" && x.MonthInt == data.MonthInt).Sum(X => X.Quantity));
                branchWise.Qty_Target = Convert.ToDecimal(Targetdata.Where(x => x.DataType != "SALES" && x.MonthInt == data.MonthInt).Sum(X => X.TargetQty));
                branchwisesales.Add(branchWise);
            }

            return branchwisesales;
        }

        public List<AccountModel> AgingDelarGroup(string Company_code)
        {
            string query = $@"select  dealer_group as AccountName,dealer_group as AccountCode from V$VIRTUAL_SUB_DEALER_LEDGER where company_code='{Company_code}' group by dealer_group";
            var LedgerList = _dbContext.SqlQuery<AccountModel>(query).ToList();
            return LedgerList;
        }
        public List<AccountModel> AgingPartyType(string Company_code,string DealerGroup="All")
        {
            if(DealerGroup=="All")
            {
                string query = $@"select  party_type_code as AccountCode,party_type_edesc as AccountName  from V$VIRTUAL_SUB_DEALER_LEDGER where company_code='{Company_code}' group by party_type_code,party_type_edesc";
                var LedgerList = _dbContext.SqlQuery<AccountModel>(query).ToList();
                return LedgerList;
            }
            else
            {
                string query = $@"select  party_type_code as AccountCode,party_type_edesc as AccountName  from V$VIRTUAL_SUB_DEALER_LEDGER where company_code='{Company_code}' and upper(DEALER_GROUP)=upper('{DealerGroup}') group by party_type_code,party_type_edesc";
                var LedgerList = _dbContext.SqlQuery<AccountModel>(query).ToList();
                return LedgerList;
            }
            var query1 = $@"select  party_type_code as AccountCode,party_type_edesc as AccountName  from V$VIRTUAL_SUB_DEALER_LEDGER where company_code='{Company_code}' group by party_type_code,party_type_edesc";
            var LedgerList1 = _dbContext.SqlQuery<AccountModel>(query1).ToList();
            return LedgerList1;
        }
        //  public List<>

        public List<ItemModel> GetItems(string company)
        {
            var data = _dbContext.SqlQuery<ItemModel>($"SELECT ITEM_CODE CODE,ITEM_EDESC EDESC FROM IP_ITEM_MASTER_SETUP WHERE GROUP_SKU_FLAG = 'I' AND CATEGORY_CODE = 'FG' AND COMPANY_CODE = '{company}'").ToList();
            return data;
        }
        public List<ItemModel> GetDivisions(string company)
        {
            var data = _dbContext.SqlQuery<ItemModel>($"SELECT  DIVISION_CODE CODE,DIVISION_EDESC EDESC FROM FA_DIVISION_SETUP WHERE GROUP_SKU_FLAG = 'I' AND COMPANY_CODE = '{company}'").ToList();
            return data;
        }

        public List<TopEmployeeWithAmountQtyModel> GetTopEmployeesByTheirSalesAmtQty(string amtOrQtyWise, string company_code)
        {
            try
            {
                var filter = string.Empty;
                var filterQry = $@"SELECT ITEM_CODE FROM MOBILE_ITEM_FILTER WHERE FILTER_TYPE='SALES'";
                var itemFilter = _dbContext.SqlQuery<string>(filterQry).FirstOrDefault();
                if (!string.IsNullOrEmpty(itemFilter))
                    filter = $@" AND SI.ITEM_CODE IN ('{itemFilter}') ";

                var query = $@"SELECT * FROM (SELECT ES.EMPLOYEE_CODE, ES.EMPLOYEE_EDESC, 'QUANTITY' QTYORAMT,
                            ROUND(SUM(NVL(QUANTITY,0)),0)QUANTITY,ROUND(SUM(NVL(NET_SALES_RATE,0)*NVL(CALC_QUANTITY,0)),2) AMOUNT FROM SA_SALES_INVOICE SI
                             INNER JOIN HR_EMPLOYEE_SETUP ES ON SI.EMPLOYEE_CODE= ES.EMPLOYEE_CODE AND ES.COMPANY_CODE = SI.COMPANY_CODE
                             WHERE SI.COMPANY_CODE ='{company_code}' AND SI.DELETED_FLAG='N' {filter} 
                             GROUP BY ES.EMPLOYEE_CODE, ES.EMPLOYEE_EDESC ORDER BY QUANTITY DESC) WHERE ROWNUM<=10
                             UNION ALL
                             SELECT * FROM (SELECT ES.EMPLOYEE_CODE, ES.EMPLOYEE_EDESC, 'AMOUNT' QTYORAMT,
                            ROUND(SUM(NVL(QUANTITY,0)),0)QUANTITY,ROUND(SUM(NVL(NET_SALES_RATE,0)*NVL(CALC_QUANTITY,0)),2) AMOUNT FROM SA_SALES_INVOICE SI
                             INNER JOIN HR_EMPLOYEE_SETUP ES ON SI.EMPLOYEE_CODE= ES.EMPLOYEE_CODE AND ES.COMPANY_CODE = SI.COMPANY_CODE
                             WHERE SI.COMPANY_CODE ='{company_code}' AND SI.DELETED_FLAG='N' {filter} 
                             GROUP BY ES.EMPLOYEE_CODE, ES.EMPLOYEE_EDESC ORDER BY AMOUNT DESC) WHERE ROWNUM<=10";
                var result = _dbContext.SqlQuery<TopEmployeeWithAmountQtyModel>(query).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<TopDealerWithAmountQtyModel> GetTopDealerByTheirSalesAmtQty(string amtOrQtyWise, string company_code)
        {
            try
            {
                var filter = string.Empty;
                var filterQry = $@"SELECT ITEM_CODE FROM MOBILE_ITEM_FILTER WHERE FILTER_TYPE='SALES'";
                var itemFilter = _dbContext.SqlQuery<string>(filterQry).FirstOrDefault();
                if (!string.IsNullOrEmpty(itemFilter))
                    filter = $@" AND SI.ITEM_CODE IN ('{itemFilter}') ";

                var query = $@"  SELECT * FROM (SELECT ES.PARTY_TYPE_CODE, ES.PARTY_TYPE_EDESC,'QUANTITY' QTYORAMT,
                             ROUND(SUM(NVL(QUANTITY,0)),0)QUANTITY,ROUND(SUM(NVL(NET_SALES_RATE,0)*NVL(CALC_QUANTITY,0)),2) AMOUNT FROM SA_SALES_INVOICE SI
                             INNER JOIN IP_PARTY_TYPE_CODE ES ON SI.PARTY_TYPE_CODE= ES.PARTY_TYPE_CODE AND ES.COMPANY_CODE = SI.COMPANY_CODE
                             WHERE SI.COMPANY_CODE ='{company_code}' AND SI.DELETED_FLAG='N' {filter}  AND ES.GROUP_SKU_FLAG='I' AND ES.PARTY_TYPE_FLAG='D'
                             GROUP BY ES.PARTY_TYPE_CODE, ES.PARTY_TYPE_EDESC ORDER BY QUANTITY DESC) WHERE ROWNUM<=10
                             UNION ALL
                               SELECT * FROM (SELECT ES.PARTY_TYPE_CODE, ES.PARTY_TYPE_EDESC,'AMOUNT' QTYORAMT,
                             ROUND(SUM(NVL(QUANTITY,0)),0)QUANTITY,ROUND(SUM(NVL(NET_SALES_RATE,0)*NVL(CALC_QUANTITY,0)),2) AMOUNT FROM SA_SALES_INVOICE SI
                             INNER JOIN IP_PARTY_TYPE_CODE ES ON SI.PARTY_TYPE_CODE= ES.PARTY_TYPE_CODE AND ES.COMPANY_CODE = SI.COMPANY_CODE
                             WHERE SI.COMPANY_CODE ='{company_code}' AND SI.DELETED_FLAG='N' {filter} AND ES.GROUP_SKU_FLAG='I' AND ES.PARTY_TYPE_FLAG='D'
                             GROUP BY ES.PARTY_TYPE_CODE, ES.PARTY_TYPE_EDESC ORDER BY AMOUNT DESC) WHERE ROWNUM<=10";
                var result = _dbContext.SqlQuery<TopDealerWithAmountQtyModel>(query).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<TopEmployeeWithAmountQtyModel> GetTopEmployeesByTheirSalesAmtQtyForMonth(string company_code, string branch_code)
        {
            try
            {
                var filter = string.Empty;
                var filterQry = $@"SELECT ITEM_CODE FROM MOBILE_ITEM_FILTER WHERE FILTER_TYPE='SALES'";
                var itemFilter = _dbContext.SqlQuery<string>(filterQry).FirstOrDefault();
                if (!string.IsNullOrEmpty(itemFilter))
                    filter = $@" AND SI.ITEM_CODE IN ('{itemFilter}') ";

                var query = $@"SELECT * FROM (SELECT ES.EMPLOYEE_CODE, ES.EMPLOYEE_EDESC, 'QUANTITY' QTYORAMT,
                            ROUND(SUM(NVL(QUANTITY,0)),0)QUANTITY,ROUND(SUM(NVL(NET_SALES_RATE,0)*NVL(CALC_QUANTITY,0)),2) AMOUNT FROM SA_SALES_INVOICE SI
                             INNER JOIN HR_EMPLOYEE_SETUP ES ON SI.EMPLOYEE_CODE= ES.EMPLOYEE_CODE AND ES.COMPANY_CODE = SI.COMPANY_CODE
                             WHERE SI.COMPANY_CODE ='{company_code}' AND SI.DELETED_FLAG='N' {filter} AND  SALES_DATE >= (SELECT FN_FIND_BS1STDATE(SYSDATE) FROM DUAL)
                            AND SALES_DATE <= TO_DATE(SYSDATE)
                             GROUP BY ES.EMPLOYEE_CODE, ES.EMPLOYEE_EDESC ORDER BY QUANTITY DESC) WHERE ROWNUM<=10
                             UNION ALL
                             SELECT * FROM (SELECT ES.EMPLOYEE_CODE, ES.EMPLOYEE_EDESC, 'AMOUNT' QTYORAMT,
                            ROUND(SUM(NVL(QUANTITY,0)),0)QUANTITY,ROUND(SUM(NVL(NET_SALES_RATE,0)*NVL(CALC_QUANTITY,0)),2) AMOUNT FROM SA_SALES_INVOICE SI
                             INNER JOIN HR_EMPLOYEE_SETUP ES ON SI.EMPLOYEE_CODE= ES.EMPLOYEE_CODE AND ES.COMPANY_CODE = SI.COMPANY_CODE
                             WHERE SI.COMPANY_CODE ='{company_code}' AND SI.DELETED_FLAG='N' {filter} AND  SALES_DATE >= (SELECT FN_FIND_BS1STDATE(SYSDATE) FROM DUAL)
                            AND SALES_DATE <= TO_DATE(SYSDATE)
                             GROUP BY ES.EMPLOYEE_CODE, ES.EMPLOYEE_EDESC ORDER BY AMOUNT DESC) WHERE ROWNUM<=10";
                var result = _dbContext.SqlQuery<TopEmployeeWithAmountQtyModel>(query).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<TopDealerWithAmountQtyModel> GetTopDealerByTheirSalesAmtQtyForMonth(string company_code, string branch_code)
        {
            try
            {
                var filter = string.Empty;
                var filterQry = $@"SELECT ITEM_CODE FROM MOBILE_ITEM_FILTER WHERE FILTER_TYPE='SALES'";
                var itemFilter = _dbContext.SqlQuery<string>(filterQry).FirstOrDefault();
                if (!string.IsNullOrEmpty(itemFilter))
                    filter = $@" AND SI.ITEM_CODE IN ('{itemFilter}') ";

                var query = $@"  SELECT * FROM (SELECT ES.PARTY_TYPE_CODE, ES.PARTY_TYPE_EDESC,'QUANTITY' QTYORAMT,
                             ROUND(SUM(NVL(QUANTITY,0)),0)QUANTITY,ROUND(SUM(NVL(NET_SALES_RATE,0)*NVL(CALC_QUANTITY,0)),2) AMOUNT FROM SA_SALES_INVOICE SI
                             INNER JOIN IP_PARTY_TYPE_CODE ES ON SI.PARTY_TYPE_CODE= ES.PARTY_TYPE_CODE AND ES.COMPANY_CODE = SI.COMPANY_CODE
                             WHERE SI.COMPANY_CODE ='{company_code}' AND SI.DELETED_FLAG='N' {filter} AND  SALES_DATE >= (SELECT FN_FIND_BS1STDATE(SYSDATE) FROM DUAL)
                            AND SALES_DATE <= TO_DATE(SYSDATE) AND ES.GROUP_SKU_FLAG='I' AND ES.PARTY_TYPE_FLAG='D'
                             GROUP BY ES.PARTY_TYPE_CODE, ES.PARTY_TYPE_EDESC ORDER BY QUANTITY DESC) WHERE ROWNUM<=10
                             UNION ALL
                               SELECT * FROM (SELECT ES.PARTY_TYPE_CODE, ES.PARTY_TYPE_EDESC,'AMOUNT' QTYORAMT,
                             ROUND(SUM(NVL(QUANTITY,0)),0)QUANTITY,ROUND(SUM(NVL(NET_SALES_RATE,0)*NVL(CALC_QUANTITY,0)),2) AMOUNT FROM SA_SALES_INVOICE SI
                             INNER JOIN IP_PARTY_TYPE_CODE ES ON SI.PARTY_TYPE_CODE= ES.PARTY_TYPE_CODE AND ES.COMPANY_CODE = SI.COMPANY_CODE
                             WHERE SI.COMPANY_CODE ='{company_code}' AND SI.DELETED_FLAG='N' {filter} AND  SALES_DATE >= (SELECT FN_FIND_BS1STDATE(SYSDATE) FROM DUAL)
                            AND SALES_DATE <= TO_DATE(SYSDATE) AND ES.GROUP_SKU_FLAG='I' AND ES.PARTY_TYPE_FLAG='D'
                             GROUP BY ES.PARTY_TYPE_CODE, ES.PARTY_TYPE_EDESC ORDER BY AMOUNT DESC) WHERE ROWNUM<=10";
                var result = _dbContext.SqlQuery<TopDealerWithAmountQtyModel>(query).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public class count
    {
        public int? countvalue { get; set; }
    }
}