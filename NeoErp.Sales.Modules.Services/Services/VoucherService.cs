using NeoErp.Core.Models;
using NeoErp.Sales.Modules.Services.Models.AgeingReport;
using NeoErp.Sales.Modules.Services.Models.Ledger;
using NeoErp.Sales.Modules.Services.Models.Voucher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoErp.Core.Domain;
using NeoErp.Core;

namespace NeoErp.Sales.Modules.Services.Services
{
    public class VoucherService : IVoucherService
    {
        private NeoErpCoreEntity _objectEntity;
        private IWorkContext _workContext;

        public VoucherService(NeoErpCoreEntity objectEntity,IWorkContext workContext)
        {
            this._objectEntity = objectEntity;
            this._workContext = workContext;
        }
        public List<VoucherDetailModel> GetVoucherDetails(ReportFiltersModel reportFilters,string formDate, string toDate, string AccountCode, NeoErp.Core.Domain.User userinfo,string BranchCode=null, string DataGeneric = "DG")
        {
            //string query = "select TO_DATE ('" + formDate + "','YYYY-MM-DD')   as voucher_date, 'Opening' as PARTICULARS,case when  sum(nvl(dr_amount, 0) - nvl(cr_amount, 0)) >= 0 then sum(nvl(dr_amount, 0) - nvl(cr_amount, 0))" +
            //                "else 0 end as dr_amount,case when sum(nvl(dr_amount, 0) - nvl(cr_amount, 0)) < 0 then abs(sum(nvl(dr_amount, 0) - nvl(cr_amount, 0))) else 0 end as cr_amount from V$VIRTUAL_GENERAL_LEDGER" +
            //                " where acc_code = '" + AccountCode + "' AND VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD') union all select voucher_date, PARTICULARS, dr_amount, cr_amount from V$VIRTUAL_GENERAL_LEDGER" +
            //                " where acc_code = '" + AccountCode + "'  AND VOUCHER_DATE BETWEEN TO_DATE('" + formDate + "', 'YYYY-MM-DD')  AND TO_DATE('" + toDate + "', 'YYYY-MM-DD')";
            if (userinfo == null)
            {
                userinfo = new Core.Domain.User();
                userinfo.company_code = "01";
                userinfo.branch_code = "01.01";

            }
            else if (string.IsNullOrEmpty(userinfo.company_code))
            {
                userinfo.company_code = "01";
                userinfo.branch_code = "01.01";
            }
            else if (string.IsNullOrEmpty(userinfo.branch_code))
            {
                userinfo.company_code = "01";
                userinfo.branch_code = "01.01";
            }
            AccountCode = "'" + AccountCode.Replace(",", "','") + "'";
            var branchCode = string.Empty;
            string Query = string.Empty;
            //if(!string.IsNullOrEmpty(BranchCode))
            //       branchCode = BranchCode.Replace(",", "','");

            // old Query Given By Pramod
            //string Query = "SELECT acc_code,PARTICULARS,manual_no,Voucher_no,voucher_date, bs_date(voucher_date) as Miti, dr_amount, cr_amount,abs(l_csum) as Balance,  (CASE WHEN l_csum >= 0 THEN 'DR' ELSE 'CR' END) BalanceHeader FROM (" +
            //              " SELECT acc_code, PARTICULARS, manual_no, Voucher_no,voucher_date, bs_date(TO_DATE(voucher_date, 'YYYY-MM-DD')) as Miti, dr_amount, cr_amount,  balance, SUM( nvl(dr_amount, 0) - nvl(cr_amount, 0)) OVER(ORDER BY voucher_no,acc_code,voucher_date,manual_no rows between unbounded preceding and current row" +
            //              " ) l_csum FROM(SELECT '0' as acc_code,TO_DATE('" + formDate + "', 'YYYY-MM-DD') AS voucher_date," +
            //                " bs_date(TO_DATE('" + formDate + "', 'YYYY-MM-DD')) as Miti, '0' as Voucher_no, '' as manual_no,'Opening' AS PARTICULARS," +
            //                " CASE WHEN SUM(NVL(dr_amount, 0) - NVL(cr_amount, 0)) >= 0 THEN SUM(NVL(dr_amount, 0) - NVL(cr_amount, 0)) ELSE 0 END AS dr_amount," +
            //                " CASE WHEN SUM(NVL(dr_amount, 0) - NVL(cr_amount, 0)) < 0 THEN ABS(SUM(NVL(dr_amount, 0) - NVL(cr_amount, 0))) ELSE 0 END   AS cr_amount," +
            //                " 0 as balance FROM V$VIRTUAL_GENERAL_LEDGER  WHERE company_code='"+ userinfo.company_code + "' and deleted_flag='N' and branch_code='"+ userinfo.branch_code + "' and acc_code in( " + AccountCode + ") and company_code = '01' AND VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD')" +
            //                " UNION ALL SELECT acc_code,voucher_date, bs_date(TO_DATE(voucher_date, 'YYYY-MM-DD')) as Miti,Voucher_no,  manual_no, PARTICULARS," +
            //                " dr_amount, cr_amount,l_csum balance FROM(SELECT acc_code,voucher_date,  bs_date(TO_DATE(voucher_date, 'YYYY-MM-DD')) as Miti, Voucher_no," +
            //                " manual_no,PARTICULARS, dr_amount, 0, cr_amount, nvl(dr_amount, 0) - nvl(cr_amount, 0) balance, SUM( nvl(dr_amount, 0) - nvl(cr_amount, 0))" +
            //                " OVER(ORDER BY voucher_no,acc_code,voucher_date,manual_no rows between unbounded preceding and current row) l_csum FROM V$VIRTUAL_GENERAL_LEDGER WHERE company_code='"+ userinfo.company_code + "' and deleted_flag='N' and branch_code='"+ userinfo.branch_code + "' and     acc_code IN(" + AccountCode + ")" +
            //                 " AND voucher_date > TO_DATE('" + formDate + "', 'YYYY-MM-DD')   AND voucher_date < TO_DATE('" + toDate + "', 'YYYY-MM-DD') )))";
            //old Query Given end by pramod

            // needtocheckQueryINfo

            //string Query = "SELECT acc_code,PARTICULARS,manual_no,Voucher_no,voucher_date, bs_date(voucher_date) as Miti, dr_amount, cr_amount,abs(l_csum) as Balance,  (CASE WHEN l_csum >= 0 THEN 'DR' ELSE 'CR' END) BalanceHeader FROM (" +
            //             " SELECT acc_code, PARTICULARS, manual_no, Voucher_no,voucher_date, bs_date(TO_DATE(voucher_date, 'YYYY-MM-DD')) as Miti, dr_amount, cr_amount,  balance, SUM( nvl(dr_amount, 0) - nvl(cr_amount, 0)) OVER(ORDER BY voucher_no,acc_code,voucher_date,manual_no rows between unbounded preceding and current row" +
            //             " ) l_csum FROM(SELECT '0' as acc_code,TO_DATE('" + formDate + "', 'YYYY-MM-DD') AS voucher_date," +
            //               " bs_date(TO_DATE('" + formDate + "', 'YYYY-MM-DD')) as Miti, '0' as Voucher_no, '' as manual_no,'Opening' AS PARTICULARS," +
            //               " CASE WHEN SUM(NVL(dr_amount, 0) - NVL(cr_amount, 0)) >= 0 THEN SUM(NVL(dr_amount, 0) - NVL(cr_amount, 0)) ELSE 0 END AS dr_amount," +
            //               " CASE WHEN SUM(NVL(dr_amount, 0) - NVL(cr_amount, 0)) < 0 THEN ABS(SUM(NVL(dr_amount, 0) - NVL(cr_amount, 0))) ELSE 0 END   AS cr_amount," +
            //               " 0 as balance FROM V$VIRTUAL_GENERAL_LEDGER  WHERE company_code='" + userinfo.company_code + "' and deleted_flag='N' and branch_code='" + userinfo.branch_code + "' and acc_code in( " + AccountCode + ") and company_code = '01' AND VOUCHER_DATE <= TO_DATE('" + formDate + "', 'YYYY-MM-DD')" +
            //               " UNION ALL SELECT acc_code,voucher_date, bs_date(TO_DATE(voucher_date, 'YYYY-MM-DD')) as Miti,Voucher_no,  manual_no, PARTICULARS," +
            //               " dr_amount, cr_amount,l_csum balance FROM(SELECT acc_code,voucher_date,  bs_date(TO_DATE(voucher_date, 'YYYY-MM-DD')) as Miti, Voucher_no," +
            //               " manual_no,PARTICULARS, dr_amount, 0, cr_amount, nvl(dr_amount, 0) - nvl(cr_amount, 0) balance, SUM( nvl(dr_amount, 0) - nvl(cr_amount, 0))" +
            //               " OVER(ORDER BY voucher_no,acc_code,voucher_date,manual_no rows between unbounded preceding and current row) l_csum FROM V$VIRTUAL_GENERAL_LEDGER WHERE company_code='" + userinfo.company_code + "' and deleted_flag='N' and branch_code='" + userinfo.branch_code + "' and     acc_code IN(" + AccountCode + ")" +
            //                " AND voucher_date > TO_DATE('" + formDate + "', 'YYYY-MM-DD')   AND voucher_date < TO_DATE('" + toDate + "', 'YYYY-MM-DD')  order by  TO_DATE (voucher_date, 'YYYY-MM-DD') )))";
            string branchcondition = string.IsNullOrEmpty(BranchCode) ? "" : " and branch_code in ("+ BranchCode + ") ";
            string companycondition = reportFilters.CompanyFilter.Count==0 ? " company_code='" + userinfo.company_code + "' " : " company_code in (" + string.Join(",", reportFilters.CompanyFilter) + ") ";
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
                       AND VOUCHER_DATE <=
                              TO_DATE('" + formDate + @"', 'YYYY-MM-DD')
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
                                and form_code<>'0'
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
                       AND VOUCHER_DATE <=
                              TO_DATE('" + formDate + @"', 'YYYY-MM-DD')
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
                                       and form_code<>'0'
                               AND voucher_date >= TO_DATE('" + formDate + @"', 'YYYY-MM-DD')
                               AND voucher_date <= TO_DATE('" + toDate + "', 'YYYY-MM-DD') order by  TO_DATE(voucher_date, 'YYYY-MM-DD'))))";


            }
            return _objectEntity.SqlQuery<VoucherDetailModel>(Query).ToList();


        }
        public List<Accounts> GetAccounts()
        {
            string query = "SELECT ACC_CODE as AccountCode,Acc_edesc as AccountName from fa_chart_of_accounts_setup where deleted_flag='N'";

            return _objectEntity.SqlQuery<Accounts>(query).ToList();
        }
        public List<LedgerBreadCrumb> GetParentAccountByAccountCode(string AccountCode)
        {
            string query = @"select distinct acc_code as AccountCode,Acc_edesc as AccountName,Master_acc_code as MasterAccountCode  ,pre_acc_code as ParentAccountCode,acc_type_flag as AccountTypeFlag
  from fa_chart_of_accounts_setup where deleted_flag='N' and company_code='01' start with master_acc_code = (select master_acc_code from fa_chart_of_accounts_setup where acc_code = '" + AccountCode + "' and company_code='01') connect by prior pre_acc_code = master_acc_code order by master_acc_code asc";
            return _objectEntity.SqlQuery<LedgerBreadCrumb>(query).ToList();

        }

        public List<LedgerAutosearch> GetLedgerAutosearch(string MasterAccountCode, string preAccountCode)
        {
            var length = MasterAccountCode.Length;
            string query = @"select acc_code as value,Acc_edesc as text from fa_chart_of_accounts_setup where length(master_acc_code) =" + length + " and master_acc_code like '" + preAccountCode + "%'";
            return _objectEntity.SqlQuery<LedgerAutosearch>(query).ToList();
        }
        public List<AccountTreeModel> AccountListAllParentNodes(NeoErp.Core.Domain.User userinfo)
        {
            if (userinfo == null)
            {
                userinfo = new Core.Domain.User();
                userinfo.company_code = "01";
                userinfo.branch_code = "01.01";

            }
            else if (string.IsNullOrEmpty(userinfo.company_code))
            {
                userinfo.company_code = "01";
                userinfo.branch_code = "01.01";
            }
            else if (string.IsNullOrEmpty(userinfo.branch_code))
            {
                userinfo.company_code = "01";
                userinfo.branch_code = "01.01";
            }
            string query = @"SELECT LEVEL,INITCAP(CS.Acc_edesc) AS AccountName,CS.acc_code AS AccountCode,
                CS.ACC_TYPE_FLAG as AccountTypeFlag,CS.Master_acc_code as MasterAccCode,CS.pre_acc_code as PreAccCode, CS.BRANCH_CODE as BranchCode,
                  (SELECT COUNT (*)
                              FROM FA_CHART_OF_ACCOUNTS_SETUP
                             WHERE ACC_TYPE_FLAG = 'N' and DELETED_FLAG = 'N' and company_code='"+ userinfo.company_code + "'  and pre_acc_code = CS.master_acc_code)"+
                            "  AS Child_rec FROM fa_chart_of_accounts_setup CS"+
              "  WHERE CS.DELETED_FLAG = 'N' AND CS.COMPANY_CODE = '" + userinfo.company_code + "'"+
                " AND  ACC_TYPE_FLAG = 'N' "+
                " AND LEVEL = 1"+
                " START WITH pre_acc_code = '00'"+
               " CONNECT BY PRIOR Master_acc_code = pre_acc_code"+
               " ORDER SIBLINGS BY Acc_edesc";
            var AccountListNodes = _objectEntity.SqlQuery<AccountTreeModel>(query).ToList();
            return AccountListNodes;
        }
        public List<AccountTreeModel> GetAccountrListByAccountCode(string level, string masterCustomerCode, NeoErp.Core.Domain.User userinfo)
        {
            if (userinfo == null)
            {
                userinfo = new Core.Domain.User();
                userinfo.company_code = "01";
                userinfo.branch_code = "01.01";

            }
            else if (string.IsNullOrEmpty(userinfo.company_code))
            {
                userinfo.company_code = "01";
                userinfo.branch_code = "01.01";
            }
            else if (string.IsNullOrEmpty(userinfo.branch_code))
            {
                userinfo.company_code = "01";
                userinfo.branch_code = "01.01";
            }
            string query = string.Format(@"SELECT INITCAP(CS.Acc_edesc) as AccountName,CS.acc_code as AccountCode,
                CS.ACC_TYPE_FLAG as AccountTypeFlag,CS.Master_acc_code as MasterAccCode,CS.pre_acc_code as PreAccCode, CS.BRANCH_CODE as BranchCode,(SELECT COUNT (*)
                              FROM FA_CHART_OF_ACCOUNTS_SETUP
                             WHERE ACC_TYPE_FLAG = 'N' and DELETED_FLAG = 'N'  and company_code='"+ userinfo.company_code + "'  and pre_acc_code = CS.master_acc_code)"+
                             " AS Child_rec"+
                "  FROM fa_chart_of_accounts_setup CS"+
             "   WHERE CS.DELETED_FLAG = 'N' AND CS.COMPANY_CODE = '"+ userinfo.company_code + "' and cs.branch_code='"+ userinfo.branch_code + "'"+
               " AND  ACC_TYPE_FLAG = 'N'"+
              "    and pre_acc_code =  (select master_acc_code from fa_chart_of_accounts_setup where acc_code='{0}'  and company_code='"+ userinfo.company_code + "')"+ 
               " ORDER  BY Acc_edesc", masterCustomerCode.ToString());
            var AccountListNodes = _objectEntity.SqlQuery<AccountTreeModel>(query).ToList();
            return AccountListNodes;
        }

        public List<AccountTreeModel> GetAccountListHavingChildrenTransaction()
        {
            string query = string.Format(@"SELECT LEVEL,INITCAP(CS.Acc_edesc) AS AccountName,CS.acc_code AS AccountCode,
                CS.ACC_TYPE_FLAG as AccountTypeFlag,CS.Master_acc_code as MasterAccCode,CS.pre_acc_code as PreAccCode, CS.BRANCH_CODE as BranchCode,
                  (SELECT COUNT (*) FROM FA_CHART_OF_ACCOUNTS_SETUP WHERE ACC_TYPE_FLAG = 'T' and DELETED_FLAG = 'N' and pre_acc_code = CS.master_acc_code)
                AS Child_rec FROM fa_chart_of_accounts_setup CS 
                WHERE CS.DELETED_FLAG = 'N' AND CS.COMPANY_CODE = '01'
                AND  ACC_TYPE_FLAG = 'N' 
                START WITH pre_acc_code = '00'
                CONNECT BY PRIOR Master_acc_code = pre_acc_code
                ORDER SIBLINGS BY Acc_edesc");
            var AccountListNodes = _objectEntity.SqlQuery<AccountTreeModel>(query).Where(q=> q.Child_rec > 0).ToList();
            return AccountListNodes;
        }

        public List<LedgerModel> GetLedgerListByAccId(string accountCode, NeoErp.Core.Domain.User userinfo)
        {
            if (userinfo == null)
            {
                userinfo = new Core.Domain.User();
                userinfo.company_code = "01";
                userinfo.branch_code = "01.01";

            }
            else if (string.IsNullOrEmpty(userinfo.company_code))
            {
                userinfo.company_code = "01";
                userinfo.branch_code = "01.01";
            }
            else if (string.IsNullOrEmpty(userinfo.branch_code))
            {
                userinfo.company_code = "01";
                userinfo.branch_code = "01.01";
            }
            accountCode = accountCode = "'" + accountCode.Replace(",", "','") + "'";
            string query = string.Format(@" SELECT INITCAP(CS.Acc_edesc) as LedgerName,CS.acc_code as LedgerCode,
                CS.ACC_TYPE_FLAG as AccountTypeFlag,CS.Master_acc_code as MasterAccCode,CS.pre_acc_code as PreAccCode, CS.BRANCH_CODE as BranchCode
                 FROM fa_chart_of_accounts_setup CS
                WHERE CS.DELETED_FLAG = 'N' AND CS.COMPANY_CODE = '"+ userinfo.company_code + "'"+
               " AND  ACC_TYPE_FLAG = 'T'"+
               "   and pre_acc_code IN ({0})"+
               " ORDER  BY Acc_edesc", accountCode);
            var AccountListNodes = _objectEntity.SqlQuery<LedgerModel>(query).ToList();
            return AccountListNodes;
        }

        public List<AccountTreeModel> AccountListAllGroupNodesAutoComplete()
        {
            string query = @"SELECT INITCAP(CS.Acc_edesc) as AccountName,CS.acc_code as AccountCode,
                CS.ACC_TYPE_FLAG as AccountTypeFlag,CS.Master_acc_code as MasterAccCode,CS.pre_acc_code as PreAccCode, CS.BRANCH_CODE as BranchCode
                 FROM fa_chart_of_accounts_setup CS
                WHERE CS.DELETED_FLAG = 'N' AND CS.COMPANY_CODE = '01'
                AND ACC_TYPE_FLAG = 'T' ";
            var AccountListNodes = _objectEntity.SqlQuery<AccountTreeModel>(query).ToList();
            return AccountListNodes;
        }
        public List<VoucherModel> GetVoucherList(string accountCode, NeoErp.Core.Domain.User userinfo)
        {
            if (userinfo == null)
            {
                userinfo = new Core.Domain.User();
                userinfo.company_code = "01";
                userinfo.branch_code = "01.01";

            }
            else if (string.IsNullOrEmpty(userinfo.company_code))
            {
                userinfo.company_code = "01";
                userinfo.branch_code = "01.01";
            }
            else if (string.IsNullOrEmpty(userinfo.branch_code))
            {
                userinfo.company_code = "01";
                userinfo.branch_code = "01.01";
            }
            string query = @"SELECT DISTINCT SUB_CODE as SubCode,SUB_EDESC as SubEdesc  FROM v$virtual_sub_ledger where acc_code ='" + accountCode + "' and company_code='"+ userinfo.company_code + "'";
            var VoucherList = _objectEntity.SqlQuery<VoucherModel>(query).ToList();
            return VoucherList;
        }
        public List<VoucherDetailModel> GetLedgerDetailBySubCode(string accountCode, string SubAccCode, string formDate, string toDate)
        {
            accountCode = "'" + accountCode.Replace(",", "','") + "'";
            SubAccCode = "'" + SubAccCode.Replace(",", "','") + "'";
            //string Query = "SELECT TO_DATE ('"+ formDate + "', 'YYYY-MM-DD') AS voucher_date, bs_date(TO_DATE ('" + formDate + "', 'YYYY-MM-DD') ) as Miti, '' as Voucher_no,'' as manual_no,'Opening' AS PARTICULARS," +
            //                " CASE WHEN SUM(NVL(dr_amount, 0) - NVL(cr_amount, 0)) >= 0 THEN SUM(NVL(dr_amount, 0) - NVL(cr_amount, 0)) ELSE 0 END AS dr_amount,CASE WHEN SUM(NVL(dr_amount, 0) - NVL(cr_amount, 0)) < 0 THEN ABS(SUM(NVL(dr_amount, 0) - NVL(cr_amount, 0))) ELSE 0 END   AS cr_amount," +
            //                "0 as Balance, '' as BalanceHeader FROM v$virtual_sub_ledger WHERE acc_code = '" + accountCode + "' and sub_code = '" + SubAccCode + "' and company_code = '01'" +
            //                "AND VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD') UNION ALL SELECT voucher_date, bs_date(voucher_date) as Miti, Voucher_no, manual_no, PARTICULARS, dr_amount, cr_amount, CASE WHEN NVL(dr_amount, 0) > 0 THEN ABS(NVL(dr_amount, 0) - NVL(cr_amount, 0))  else ABS(NVL(cr_amount, 0) - NVL(dr_amount, 0)) End as Balance," +
            //                "CASE WHEN NVL(dr_amount, 0) > 0 THEN 'DR' else 'CR' End as BalanceHeader FROM v$virtual_sub_ledger" +
            //                " WHERE acc_code = '"+ accountCode + "' and sub_code = '" + SubAccCode + "' and company_code = '01' AND" +
            //                " VOUCHER_DATE BETWEEN TO_DATE('" + formDate + "', 'YYYY-MM-DD') AND TO_DATE('" + toDate + "', 'YYYY-MM-DD')";
            string Query = @"SELECT sub_code, PARTICULARS, manual_no, Voucher_no,voucher_date,bs_date(voucher_date) as Miti, dr_amount,cr_amount," +
                           " abs(l_csum) as Balance, (CASE WHEN l_csum >= 0 THEN 'DR' ELSE 'CR' END) BalanceHeader FROM ( SELECT sub_code, PARTICULARS, manual_no, Voucher_no," +
                          " voucher_date,bs_date(TO_DATE(voucher_date, 'YYYY-MM-DD')) as Miti,dr_amount,cr_amount, balance,SUM(dr_amount - cr_amount)" +
                          " OVER(ORDER BY sub_code, voucher_date ROWS 5 PRECEDING) l_csum FROM( SELECT '0' as sub_code, '' as sub_edesc,  TO_DATE('" + formDate + "', 'YYYY-MM-DD') AS voucher_date," +
                           " bs_date(TO_DATE('" + formDate + "', 'YYYY-MM-DD')) as Miti, '0' as Voucher_no, '' as manual_no,'Opening' AS PARTICULARS, CASE WHEN SUM(NVL(dr_amount, 0) - NVL(cr_amount, 0)) >= 0 THEN SUM(NVL(dr_amount, 0) - NVL(cr_amount, 0))" +
                          " ELSE 0 END AS dr_amount, CASE WHEN SUM(NVL(dr_amount, 0) - NVL(cr_amount, 0)) < 0 THEN ABS(SUM(NVL(dr_amount, 0) - NVL(cr_amount, 0))) ELSE 0 END   AS cr_amount," +
                          " 0 as balance FROM v$virtual_sub_ledger WHERE acc_code IN (" + accountCode + ") and sub_code IN (" + SubAccCode + ") and company_code = '01' AND VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD')" +
                          " UNION ALL SELECT sub_code, sub_edesc, voucher_date, bs_date(TO_DATE(voucher_date, 'YYYY-MM-DD')) as Miti, Voucher_no,manual_no, PARTICULARS,dr_amount,cr_amount, l_csum balance" +
                          " FROM(SELECT sub_code,sub_edesc, voucher_date, bs_date(TO_DATE(voucher_date, 'YYYY-MM-DD')) as Miti, Voucher_no, manual_no, PARTICULARS,dr_amount," +
                          " cr_amount,dr_amount - cr_amount balance,SUM(dr_amount - cr_amount) OVER(ORDER BY sub_code, sub_edesc, voucher_date ROWS 5 PRECEDING) l_csum FROM v$virtual_sub_ledger" +
                          "  WHERE     acc_code IN (" + accountCode + ") AND sub_code IN (" + SubAccCode + ") AND voucher_date > TO_DATE('"+ formDate + "', 'YYYY-MM-DD')AND voucher_date < TO_DATE('" + toDate + "', 'YYYY-MM-DD') ORDER BY voucher_date)))";
            var subvoucherList = _objectEntity.SqlQuery<VoucherDetailModel>(Query).ToList();
            return subvoucherList;
        }
        public List<VoucherModel> GetSubLedgerListAutoComplete()
        {
            string query = @"SELECT DISTINCT SUB_CODE as SubCode,SUB_EDESC as SubEdesc,ACC_CODE as AccCode FROM v$virtual_sub_ledger where Deleted_flag='N' and company_code='01'";
            var SubLedgerListNodes = _objectEntity.SqlQuery<VoucherModel>(query).ToList();
            return SubLedgerListNodes;
        }

        public List<LedgerModel> GetLedgerListByHierarchy(string accountCode)
        {
            string query = string.Format(@"SELECT INITCAP(CS.Acc_edesc) as LedgerName,CS.acc_code as LedgerCode,
                CS.ACC_TYPE_FLAG as AccountTypeFlag,CS.Master_acc_code as MasterAccCode,CS.pre_acc_code as PreAccCode, CS.BRANCH_CODE as BranchCode
                 FROM fa_chart_of_accounts_setup CS
                WHERE CS.DELETED_FLAG = 'N' AND CS.COMPANY_CODE = '01'
                AND  ACC_TYPE_FLAG = 'T'
                AND  pre_acc_code IN ( 
                 SELECT CS.Master_acc_code  FROM fa_chart_of_accounts_setup CS
                                    WHERE CS.DELETED_FLAG = 'N' AND CS.COMPANY_CODE = '01'
                                    AND  ACC_TYPE_FLAG = 'N' AND Master_acc_code like '{0}%'
                                    START WITH pre_acc_code = '00'
                                    CONNECT BY PRIOR Master_acc_code = pre_acc_code
                                    )
                ORDER  BY Acc_edesc", accountCode);
            var AccountListNodes = _objectEntity.SqlQuery<LedgerModel>(query).ToList();
            return AccountListNodes;
        }

        public List<VoucherDetailReportModal> GetVoucherDetailFromCode(string voucherNo)
        {
            // query to fetch the record.
            // and return 
            //List<VoucherDetailModel> vdm = new List<VoucherDetailModel>();
            //string query = @"select * from V$VIRTUAL_GENERAL_LEDGER where voucher_no='" + voucherNo + "'";
            //var voucherDetailResult = _objectEntity.SqlQuery<VoucherDetailModel>(query).ToList();
            //vdm = voucherDetailResult;

            List<VoucherDetailReportModal> vdm = new List<VoucherDetailReportModal>();
            string query = $@"select rownum as sno, t.* from V$VIRTUAL_GENERAL_LEDGER t where t.voucher_no='" + voucherNo + "' and t.company_code='"+_workContext.CurrentUserinformation.company_code+"'";
            var voucherDetailResult = _objectEntity.SqlQuery<VoucherDetailReportModal>(query).ToList();
            vdm = voucherDetailResult;
            return vdm;
        }
        public IEnumerable<AgeingGroupData> GetAccountData()
        {
            return _objectEntity.SqlQuery<AgeingGroupData>(@"SELECT  ACC_EDESC AS Description,
                                    TO_NUMBER(REPLACE(MASTER_ACC_CODE,'.','')) as MasterCode,MASTER_ACC_CODE as MasterCodeWithoutReplace,PRE_ACC_CODE as PreCodeWithoutReplace, TO_NUMBER(ACC_CODE) as Code, TO_NUMBER(REPLACE(PRE_ACC_CODE,'.','')) as PreCode 
                                    FROM FA_CHART_OF_ACCOUNTS_SETUP WHERE ACC_TYPE_FLAG = 'N' AND DELETED_FLAG = 'N'").OrderByDescending(q => q.PreCode);
        }

        public List<VoucherDetailModel> GetLedgerDetailBySubCode(string accountCode, string SubAccCode, string formDate, string toDate, User userinfo)
        {
            accountCode = "'" + accountCode.Replace(",", "','") + "'";
            SubAccCode = "'" + SubAccCode.Replace(",", "','") + "'";
           
            var companyCode =  $@"'{userinfo.company_code}'";
            //string Query = "SELECT TO_DATE ('"+ formDate + "', 'YYYY-MM-DD') AS voucher_date, bs_date(TO_DATE ('" + formDate + "', 'YYYY-MM-DD') ) as Miti, '' as Voucher_no,'' as manual_no,'Opening' AS PARTICULARS," +
            //                " CASE WHEN SUM(NVL(dr_amount, 0) - NVL(cr_amount, 0)) >= 0 THEN SUM(NVL(dr_amount, 0) - NVL(cr_amount, 0)) ELSE 0 END AS dr_amount,CASE WHEN SUM(NVL(dr_amount, 0) - NVL(cr_amount, 0)) < 0 THEN ABS(SUM(NVL(dr_amount, 0) - NVL(cr_amount, 0))) ELSE 0 END   AS cr_amount," +
            //                "0 as Balance, '' as BalanceHeader FROM v$virtual_sub_ledger WHERE acc_code = '" + accountCode + "' and sub_code = '" + SubAccCode + "' and company_code = '01'" +
            //                "AND VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD') UNION ALL SELECT voucher_date, bs_date(voucher_date) as Miti, Voucher_no, manual_no, PARTICULARS, dr_amount, cr_amount, CASE WHEN NVL(dr_amount, 0) > 0 THEN ABS(NVL(dr_amount, 0) - NVL(cr_amount, 0))  else ABS(NVL(cr_amount, 0) - NVL(dr_amount, 0)) End as Balance," +
            //                "CASE WHEN NVL(dr_amount, 0) > 0 THEN 'DR' else 'CR' End as BalanceHeader FROM v$virtual_sub_ledger" +
            //                " WHERE acc_code = '"+ accountCode + "' and sub_code = '" + SubAccCode + "' and company_code = '01' AND" +
            //                " VOUCHER_DATE BETWEEN TO_DATE('" + formDate + "', 'YYYY-MM-DD') AND TO_DATE('" + toDate + "', 'YYYY-MM-DD')";
            string Query = $@"SELECT sub_code, PARTICULARS, manual_no, Voucher_no,voucher_date,bs_date(voucher_date) as Miti, dr_amount,cr_amount," +
                           " abs(l_csum) as Balance, (CASE WHEN l_csum >= 0 THEN 'DR' ELSE 'CR' END) BalanceHeader FROM ( SELECT sub_code, PARTICULARS, manual_no, Voucher_no," +
                          " voucher_date,bs_date(TO_DATE(voucher_date, 'YYYY-MM-DD')) as Miti,dr_amount,cr_amount, balance,SUM(dr_amount - cr_amount)" +
                          " OVER(ORDER BY sub_code, voucher_date ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) l_csum FROM( SELECT '0' as sub_code, '' as sub_edesc,  TO_DATE('" + formDate + "', 'YYYY-MM-DD') AS voucher_date," +
                           " bs_date(TO_DATE('" + formDate + "', 'YYYY-MM-DD')) as Miti, '0' as Voucher_no, '' as manual_no,'Opening' AS PARTICULARS, CASE WHEN SUM(NVL(dr_amount, 0) - NVL(cr_amount, 0)) >= 0 THEN SUM(NVL(dr_amount, 0) - NVL(cr_amount, 0))" +
                          " ELSE 0 END AS dr_amount, CASE WHEN SUM(NVL(dr_amount, 0) - NVL(cr_amount, 0)) < 0 THEN ABS(SUM(NVL(dr_amount, 0) - NVL(cr_amount, 0))) ELSE 0 END   AS cr_amount," +
                          " 0 as balance FROM v$virtual_sub_ledger WHERE acc_code IN (" + accountCode + ") and sub_code IN (" + SubAccCode + ") and company_code = '"+userinfo.company_code+"' AND (VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD') OR FORM_CODE = '0')" +
                          " UNION ALL SELECT sub_code, sub_edesc, voucher_date, bs_date(TO_DATE(voucher_date, 'YYYY-MM-DD')) as Miti, Voucher_no,manual_no, PARTICULARS,dr_amount,cr_amount, l_csum balance" +
                          " FROM(SELECT sub_code,sub_edesc, voucher_date, bs_date(TO_DATE(voucher_date, 'YYYY-MM-DD')) as Miti, Voucher_no, manual_no, PARTICULARS,dr_amount," +
                          " cr_amount,dr_amount - cr_amount balance,SUM(dr_amount - cr_amount) OVER(ORDER BY sub_code, sub_edesc, voucher_date ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) l_csum FROM v$virtual_sub_ledger" +
                          "  WHERE     acc_code IN (" + accountCode + ") AND sub_code IN (" + SubAccCode + ") AND FORM_CODE <> '0' AND company_code = '" + userinfo.company_code + "' and voucher_date >= TO_DATE('" + formDate + "', 'YYYY-MM-DD')AND voucher_date <= TO_DATE('" + toDate + "', 'YYYY-MM-DD') ORDER BY voucher_date)))";
            var subvoucherList = _objectEntity.SqlQuery<VoucherDetailModel>(Query).ToList();
            return subvoucherList;
        }

       public List<CUSTOMTEMPLATEFORPL> GeneratePLLedger(string formDate,string ToDate,string companyCode,string BranchCode, User userinfo)
        {
            var data = $@"SELECT * FROM FA_PL_TEMPLATE_SETUP";
            var pltemplate = _objectEntity.SqlQuery<CUSTOMTEMPLATEFORPL>(data).ToList();
            foreach(var segment in pltemplate)
            {
                if (segment.END_SEGMENT == "Y")

                    continue;

                if(segment.CL_FLAG=="Y")
                {
                    var query = $@"select item_code ACC_CODE,item_edesc ACC_EDESC,sum(in_price-out_price)  Balance  from (select IT.item_code,IM.ITEM_EDESC, sum(in_quantity*in_unit_price) IN_PRICE ,sum(OUT_QUANTITY*out_unit_price) OUT_PRICE from IP_TEMP_VALUE_LEDGER IT ,IP_ITEM_MASTER_SETUP IM
                                    WHERE IT.ITEM_CODE = IM.ITEM_CODE AND IT.COMPANY_CODE = IM.COMPANY_CODE
                                    AND IM.CATEGORY_CODE NOT IN(SELECT CATEGORY_CODE FROM IP_CATEGORY_CODE WHERE CATEGORY_TYPE = 'FG' AND COMPANY_CODE = '02')
                                    and im.COMPANY_CODE = '02' AND it.VOUCHER_DATE <= TO_DATE('12/01/2022', 'DD-MM-YYYY')
                                    GROUP BY IT.item_code,IM.ITEM_EDESC) group by item_code,item_edesc";
                    var balancedata = _objectEntity.SqlQuery<ChildLedgerSummary>(query);
                    if (balancedata == null)
                    {
                        segment.Balance = 0;
                    }
                    else
                    {
                        segment.Balance = balancedata.Sum(x => x.Balance);
                        segment.ChildSummary = balancedata.ToList();
                    }
                    continue;
                }
                else if(segment.FG_CL_FLAG=="Y")
                {
                    var query = $@"select item_code ACC_CODE,item_edesc ACC_EDESC,sum(in_price-out_price)  Balance  from (select IT.item_code,IM.ITEM_EDESC, sum(in_quantity*in_unit_price) IN_PRICE ,sum(OUT_QUANTITY*out_unit_price) OUT_PRICE from IP_TEMP_VALUE_LEDGER IT ,IP_ITEM_MASTER_SETUP IM
                                    WHERE IT.ITEM_CODE = IM.ITEM_CODE AND IT.COMPANY_CODE = IM.COMPANY_CODE
                                    AND IM.CATEGORY_CODE  IN(SELECT CATEGORY_CODE FROM IP_CATEGORY_CODE WHERE CATEGORY_TYPE = 'FG' AND COMPANY_CODE = '02')
                                    and im.COMPANY_CODE = '02' AND it.VOUCHER_DATE <= TO_DATE('12/01/2022', 'DD-MM-YYYY')
                                    GROUP BY IT.item_code,IM.ITEM_EDESC) group by item_code,item_edesc";
                    var balancedata = _objectEntity.SqlQuery<ChildLedgerSummary>(query);
                    if (balancedata == null)
                    {
                        segment.Balance = 0;
                    }
                    else
                    {
                        segment.Balance = balancedata.Sum(x => x.Balance);
                        segment.ChildSummary = balancedata.ToList();
                    }
                    continue;
                }
                else if(segment.OP_FLAG=="Y")
                {
                    var query = $@"select item_code ACC_CODE,item_edesc ACC_EDESC,sum(in_price-out_price)  Balance  from (select IT.item_code,IM.ITEM_EDESC, sum(in_quantity*in_unit_price) IN_PRICE ,sum(OUT_QUANTITY*out_unit_price) OUT_PRICE from IP_TEMP_VALUE_LEDGER IT ,IP_ITEM_MASTER_SETUP IM
                                    WHERE IT.ITEM_CODE = IM.ITEM_CODE AND IT.COMPANY_CODE = IM.COMPANY_CODE
                                    AND IM.CATEGORY_CODE NOT IN(SELECT CATEGORY_CODE FROM IP_CATEGORY_CODE WHERE CATEGORY_TYPE = 'FG' AND COMPANY_CODE = '02')
                                    and im.COMPANY_CODE = '02' AND it.VOUCHER_DATE < TO_DATE('12/01/2022', 'DD-MM-YYYY')
                                    GROUP BY IT.item_code,IM.ITEM_EDESC) group by item_code,item_edesc";
                    var balancedata = _objectEntity.SqlQuery<ChildLedgerSummary>(query);
                    if (balancedata == null)
                    {
                        segment.Balance = 0;
                    }
                    else
                    {
                        segment.Balance = balancedata.Sum(x => x.Balance);
                        segment.ChildSummary = balancedata.ToList();
                    }
                    continue;
                }
                else if (segment.FG_OP_FLAG == "Y")
                {
                    var query = $@"select item_code ACC_CODE,item_edesc ACC_EDESC,sum(in_price-out_price)  Balance  from (select IT.item_code,IM.ITEM_EDESC, sum(in_quantity*in_unit_price) IN_PRICE ,sum(OUT_QUANTITY*out_unit_price) OUT_PRICE from IP_TEMP_VALUE_LEDGER IT ,IP_ITEM_MASTER_SETUP IM
                                    WHERE IT.ITEM_CODE = IM.ITEM_CODE AND IT.COMPANY_CODE = IM.COMPANY_CODE
                                    AND IM.CATEGORY_CODE  IN(SELECT CATEGORY_CODE FROM IP_CATEGORY_CODE WHERE CATEGORY_TYPE = 'FG' AND COMPANY_CODE = '02')
                                    and im.COMPANY_CODE = '02' AND it.VOUCHER_DATE <= TO_DATE('12/01/2022', 'DD-MM-YYYY')
                                    GROUP BY IT.item_code,IM.ITEM_EDESC) group by item_code,item_edesc";
                    var balancedata = _objectEntity.SqlQuery<ChildLedgerSummary>(query);
                    if (balancedata == null)
                    {
                        segment.Balance = 0;
                    }
                    else
                    {
                        segment.Balance = balancedata.Sum(x => x.Balance);
                        segment.ChildSummary = balancedata.ToList();
                    }
                    continue;
                }

                if (segment.ACCOUNT_FORMULA=="cr-dr".ToUpper())
                {
                    var queryCrDr = $@" SELECT FC.ACC_EDESC,FC.ACC_CODE,SUM(NVL(CR_AMOUNT,0) * NVL(EXCHANGE_RATE,1) - NVL(DR_AMOUNT,0) * NVL(EXCHANGE_RATE,1)) BALANCE
                                         FROM
                                            V$VIRTUAL_GENERAL_LEDGER   FL,FA_CHART_OF_ACCOUNTS_SETUP FC
                                         WHERE 1=1 AND
                                         FL.COMPANY_CODE=FC.COMPANY_CODE AND
                                         FL.ACC_CODE=FC.ACC_CODE
                                           -- branch_code in ('01.01','01.02') 
                                           AND FL.COMPANY_CODE ='02'
                                              and FL.deleted_flag='N'
                                              AND FL.ACC_CODE IN (SELECT ACC_CODE FROM FA_PL_CUSTOM_DETAIL_SETUP  where company_code='02' AND PL_CODE='{segment.PL_CODE}')
                                            AND FL.FORM_CODE <> '0' 
                                            AND trunc(VOUCHER_DATE) BETWEEN trunc(TO_DATE ('2016-Nov-2', 'YYYY-MM-DD'))     AND trunc(TO_DATE ('2020-May-13', 'YYYY-MM-DD'))
                                            GROUP BY  FC.ACC_EDESC,FC.ACC_CODE";
                    var balancedata = _objectEntity.SqlQuery<ChildLedgerSummary>(queryCrDr);
                    if (balancedata == null)
                    {
                        segment.Balance = 0;
                    }
                    else
                    {
                        segment.Balance = balancedata.Sum(x => x.Balance);
                        segment.ChildSummary = balancedata.ToList();
                    }
                    
                        
                   

                }
                else if (segment.ACCOUNT_FORMULA == "DR-CR".ToUpper())
                {
                    var queryDrCr = $@" SELECT FC.ACC_EDESC,FC.ACC_CODE,SUM(NVL(DR_AMOUNT,0) * NVL(EXCHANGE_RATE,1) - NVL(CR_AMOUNT,0) * NVL(EXCHANGE_RATE,1)) BALANCE
                                         FROM
                                            V$VIRTUAL_GENERAL_LEDGER   FL,FA_CHART_OF_ACCOUNTS_SETUP FC
                                         WHERE 1=1 AND
                                         FL.COMPANY_CODE=FC.COMPANY_CODE AND
                                         FL.ACC_CODE=FC.ACC_CODE
                                           -- branch_code in ('01.01','01.02') 
                                           AND FL.COMPANY_CODE ='02'
                                              and FL.deleted_flag='N'
                                              AND FL.ACC_CODE IN (SELECT ACC_CODE FROM FA_PL_CUSTOM_DETAIL_SETUP  where company_code='02' AND PL_CODE='{segment.PL_CODE}')
                                            AND FL.FORM_CODE <> '0' 
                                            AND trunc(VOUCHER_DATE) BETWEEN trunc(TO_DATE ('2016-Nov-2', 'YYYY-MM-DD'))     AND trunc(TO_DATE ('2020-May-13', 'YYYY-MM-DD'))
                                            GROUP BY  FC.ACC_EDESC,FC.ACC_CODE";
                    var balancedataCr = _objectEntity.SqlQuery<ChildLedgerSummary>(queryDrCr);
                    if (balancedataCr == null)
                    {
                        segment.Balance = 0;
                    }
                    else
                    {
                        segment.Balance = balancedataCr.Sum(x => x.Balance);
                        segment.ChildSummary = balancedataCr.ToList();
                    }
                }
                else if (segment.ACCOUNT_FORMULA == "DR".ToUpper())
                {
                    var queryDrCr = $@" SELECT FC.ACC_EDESC,FC.ACC_CODE,SUM(NVL(DR_AMOUNT,0) * NVL(EXCHANGE_RATE,1)) BALANCE
                                         FROM
                                            V$VIRTUAL_GENERAL_LEDGER   FL,FA_CHART_OF_ACCOUNTS_SETUP FC
                                         WHERE 1=1 AND
                                         FL.COMPANY_CODE=FC.COMPANY_CODE AND
                                         FL.ACC_CODE=FC.ACC_CODE
                                           -- branch_code in ('01.01','01.02') 
                                           AND FL.COMPANY_CODE ='02'
                                              and FL.deleted_flag='N'
                                              AND FL.ACC_CODE IN (SELECT ACC_CODE FROM FA_PL_CUSTOM_DETAIL_SETUP  where company_code='02' AND PL_CODE='{segment.PL_CODE}')
                                            AND FL.FORM_CODE <> '0' 
                                            AND trunc(VOUCHER_DATE) BETWEEN trunc(TO_DATE ('2016-Nov-2', 'YYYY-MM-DD'))     AND trunc(TO_DATE ('2020-May-13', 'YYYY-MM-DD'))
                                            GROUP BY  FC.ACC_EDESC,FC.ACC_CODE";
                    var balancedataCr = _objectEntity.SqlQuery<ChildLedgerSummary>(queryDrCr);
                    if (balancedataCr == null)
                    {
                        segment.Balance = 0;
                    }
                    else
                    {
                        segment.Balance = balancedataCr.Sum(x => x.Balance);
                        segment.ChildSummary = balancedataCr.ToList();
                    }
                }
                else if (segment.ACCOUNT_FORMULA == "CR".ToUpper())
                {
                    var queryDrCr = $@" SELECT FC.ACC_EDESC,FC.ACC_CODE,SUM(NVL(CR_AMOUNT,0) * NVL(EXCHANGE_RATE,1)) BALANCE
                                         FROM
                                            V$VIRTUAL_GENERAL_LEDGER   FL,FA_CHART_OF_ACCOUNTS_SETUP FC
                                         WHERE 1=1 AND
                                         FL.COMPANY_CODE=FC.COMPANY_CODE AND
                                         FL.ACC_CODE=FC.ACC_CODE
                                           -- branch_code in ('01.01','01.02') 
                                           AND FL.COMPANY_CODE ='02'
                                              and FL.deleted_flag='N'
                                              AND FL.ACC_CODE IN (SELECT ACC_CODE FROM FA_PL_CUSTOM_DETAIL_SETUP  where company_code='02' AND PL_CODE='{segment.PL_CODE}')
                                            AND FL.FORM_CODE <> '0' 
                                            AND trunc(VOUCHER_DATE) BETWEEN trunc(TO_DATE ('2016-Nov-2', 'YYYY-MM-DD'))     AND trunc(TO_DATE ('2020-May-13', 'YYYY-MM-DD'))
                                            GROUP BY  FC.ACC_EDESC,FC.ACC_CODE";
                    var balancedataCr = _objectEntity.SqlQuery<ChildLedgerSummary>(queryDrCr);
                    if (balancedataCr == null)
                    {
                        segment.Balance = 0;
                    }
                    else
                    {
                        segment.Balance = balancedataCr.Sum(x => x.Balance);
                        segment.ChildSummary = balancedataCr.ToList();
                    }
                }
                //else if (segment.ACCOUNT_FORMULA == "COGS".ToUpper())
                //{
                //    var queryDrCr = $@"select sum(profit_per)  from (SELECT B.ITEM_CODE, B.ITEM_EDESC, B.GROUP_SKU_FLAG, B.INDEX_MU_CODE, B.MASTER_ITEM_CODE, (LENGTH(B.MASTER_ITEM_CODE) - LENGTH(REPLACE(B.MASTER_ITEM_CODE,'.',''))) LEVEL_ROW , AVG_SALES_RATE, AVG_PUR_RATE, SALES_QUANTITY , PURCHASE_QUANTITY, TOTAL_PURCHASE_AMOUNT, TOTAL_SALES_AMOUNT, (AVG_SALES_RATE - AVG_PUR_RATE) * SALES_QUANTITY ITEM_WISE_PROFIT,
                //                                                                                 CASE

                //                                                                                 WHEN TOTAL_SALES_AMOUNT <> 0 THEN

                //                                                                                     CASE

                //                                                                                     WHEN TOTAL_PURCHASE_AMOUNT <> 0 THEN

                //                                                                                          ROUND( ((TOTAL_SALES_AMOUNT- TOTAL_PURCHASE_AMOUNT)/DECODE(TOTAL_PURCHASE_AMOUNT,0,1,TOTAL_PURCHASE_AMOUNT) ) *100,2)

                //                                                                                     Else

                //                                                                                 100

                //                                                                                     End

                //                                                                                 Else

                //                                                                                 0

                //                                                                                 END  PROFIT_PER FROM (

                //                                                                                 SELECT ITEM_CODE, COMPANY_CODE, SUM(SALES_QUANTITY) SALES_QUANTITY, SUM(TOTAL_SALES_AMOUNT) TOTAL_SALES_AMOUNT, SUM(AVG_SALES_RATE) AVG_SALES_RATE, SUM(PURCHASE_QUANTITY) PURCHASE_QUANTITY, SUM(TOTAL_PURCHASE_AMOUNT)  TOTAL_PURCHASE_AMOUNT , SUM(AVG_PUR_RATE) AVG_PUR_RATE FROM (

                //                                                                                 SELECT ITEM_CODE, COMPANY_CODE, SUM(QUANTITY) SALES_QUANTITY,   SUM(TOTAL_AMOUNT) TOTAL_SALES_AMOUNT, SUM(TOTAL_AMOUNT)/DECODE(SUM(QUANTITY) ,0,1,SUM(QUANTITY) ) AVG_SALES_RATE, 0 PURCHASE_QUANTITY, 0 AVG_PUR_RATE , 0 TOTAL_PURCHASE_AMOUNT  FROM V_ITEM_WISE_OUT_VALUES

                //                                                                                 WHERE 1 = 1

                //                                                                                AND COMPANY_CODE='02'

                //                                                                                --AND BRANCH_CODE='01.01'

                //                                                                                 AND VOUCHER_DATE BETWEEN '17-Jul-2016' AND '04-May-2020'

                //                                                                                 AND DELETED_FLAG = 'N'

                //                                                                                 GROUP BY ITEM_CODE, COMPANY_CODE

                //                                                                                 Union All

                //                                                                                 SELECT ITEM_CODE, COMPANY_CODE, 0 SALES_QUANTITY , 0 TOTAL_SALES_AMOUNT ,0 AVG_SALES_RATE ,SUM(QUANTITY) PURCHASE_QUANTITY, SUM(TOTAL_AMOUNT)/DECODE(SUM(QUANTITY),0,1,SUM(QUANTITY))  AVG_PUR_RATE, SUM(TOTAL_AMOUNT) TOTAL_PURCHASE_AMOUNT   FROM (

                //                                                                                 SELECT ITEM_CODE, COMPANY_CODE, NVL(SUM(NVL(IN_QUANTITY,0)) - SUM(NVL(OUT_QUANTITY,0)),0) QUANTITY, NVL(SUM(NVL(IN_QUANTITY,0) * NVL(IN_UNIT_PRICE,0)) - SUM(NVL(OUT_QUANTITY,0) * NVL(OUT_UNIT_PRICE,0)),0) TOTAL_AMOUNT

                //                                                                                 From IP_TEMP_VALUE_LEDGER

                //                                                                                 Where 1 = 1

                //                                                                                AND COMPANY_CODE='02'

                //                                                                                --AND BRANCH_CODE='01.01'

                //                                                                                 AND METHOD = 'FIFO'

                //                                                                                 AND (VOUCHER_DATE < '17-Jul-2016' OR FORM_CODE = '0')

                //                                                                                 AND DELETED_FLAG = 'N'

                //                                                                                 GROUP BY ITEM_CODE , COMPANY_CODE

                //                                                                                 Union All

                //                                                                                 SELECT ITEM_CODE, COMPANY_CODE, SUM(QUANTITY) QUANTITY, SUM(TOTAL_AMOUNT) TOTAL_AMOUNT FROM V_ITEM_WISE_IN_VALUES

                //                                                                                 WHERE 1 = 1

                //                                                                                AND COMPANY_CODE='02'

                //                                                                                --AND BRANCH_CODE='01.01'

                //                                                                                 AND VOUCHER_DATE BETWEEN '17-Jul-2016' AND '04-May-2020'

                //                                                                                 AND DELETED_FLAG = 'N'

                //                                                                                 AND FORM_CODE <> '0'

                //                                                                                 GROUP BY ITEM_CODE  ,COMPANY_CODE

                //                                                                                 )GROUP BY ITEM_CODE  ,COMPANY_CODE

                //                                                                                 )GROUP BY ITEM_CODE  ,COMPANY_CODE

                //                                                                                 )A, IP_ITEM_MASTER_SETUP B

                //                                                                                 Where 1 = 1

                //                                                                                 AND B.ITEM_CODE = A.ITEM_CODE (+)

                //                                                                                 AND B.COMPANY_CODE = A.COMPANY_CODE (+)

                //                                                                                AND B.COMPANY_CODE='02'

                //                                                                                 ORDER BY B.MASTER_ITEM_CODE)";
                //    var balancedataCr = _objectEntity.SqlQuery<double>(queryDrCr).FirstOrDefault();
                //    if (balancedataCr == null)
                //    {
                //        segment.Balance = 0;
                //    }
                //    else
                //    {
                //        segment.Balance = balancedataCr;
                //      //  segment.ChildSummary = balancedataCr.ToList();
                //    }
                //}
                else
                {
                    var queryCrDr = $@" SELECT FC.ACC_EDESC,FC.ACC_CODE,SUM(NVL(CR_AMOUNT,0) * NVL(EXCHANGE_RATE,1) - NVL(DR_AMOUNT,0) * NVL(EXCHANGE_RATE,1)) BALANCE
                                         FROM
                                            V$VIRTUAL_GENERAL_LEDGER   FL,FA_CHART_OF_ACCOUNTS_SETUP FC
                                         WHERE 1=1 AND
                                         FL.COMPANY_CODE=FC.COMPANY_CODE AND
                                         FL.ACC_CODE=FC.ACC_CODE
                                           -- branch_code in ('01.01','01.02') 
                                           AND FL.COMPANY_CODE ='02'
                                              and FL.deleted_flag='N'
                                              AND FL.ACC_CODE IN (SELECT ACC_CODE FROM FA_PL_CUSTOM_DETAIL_SETUP  where company_code='02' AND PL_CODE='{segment.PL_CODE}')
                                            AND FL.FORM_CODE <> '0' 
                                            AND trunc(VOUCHER_DATE) BETWEEN trunc(TO_DATE ('2016-Nov-2', 'YYYY-MM-DD'))     AND trunc(TO_DATE ('2020-May-13', 'YYYY-MM-DD'))
                                            GROUP BY  FC.ACC_EDESC,FC.ACC_CODE";
                    var balancedata = _objectEntity.SqlQuery<ChildLedgerSummary>(queryCrDr);
                    if (balancedata == null)
                    {
                        segment.Balance = 0;
                    }
                    else
                    {
                        segment.Balance = balancedata.Sum(x => x.Balance);
                        segment.ChildSummary = balancedata.ToList();
                    }


                }

            }

            return pltemplate;

        }
    }
}
