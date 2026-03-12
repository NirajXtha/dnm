using NeoErp.Core.Helpers;
using NeoErp.Core.Models;
using NeoErp.Sales.Modules.Services.Models;
using NeoErp.Sales.Modules.Services.Models.Ledger;
using NeoErp.Sales.Modules.Services.Models.TrialBalance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Services
{
    public class TrialBalanceService: ITrialBalanceService
    {
        private NeoErpCoreEntity _objectEntity;

        public TrialBalanceService(NeoErpCoreEntity objectEntity)
        {
            this._objectEntity = objectEntity;
        }
        public List<TrialBalanceViewModel> GetTrialBalanceMasterGrid(string formDate, string toDate)
        {
                        string Query = @"select account_head, TreeLevel, Pre_Acc_Code,Pre_Acc_Code as parentId, Master_ACC_Code , acc_code, DR_OPENING, CR_OPENING, dr_amt, cr_amt , nvl(DR_OPENING,0) +  nvl(dr_amt,0) as DR_Closing, nvl(CR_OPENING,0) + nvl(cr_amt,0) as cr_closing, Child_rec from (select account_head, TreeLevel, Pre_Acc_Code, Master_ACC_Code , acc_code,   " +
            "CASE WHEN dr_amt_opening-cr_amt_opening >= 0  THEN dr_amt_opening-cr_amt_opening  ELSE 0 end as DR_OPENING,CASE WHEN dr_amt_opening-cr_amt_opening < 0  THEN ABS(dr_amt_opening-cr_amt_opening )ELSE 0" +
            "end as CR_OPENING,dr_amt, cr_amt , Child_rec from(select  (lpad(' ',2*(level-1)) || acc_edesc) account_head, LEVEL AS TreeLevel, Pre_Acc_Code, Master_ACC_Code , acc_code, (select count(*) from FA_CHART_OF_ACCOUNTS_SETUP where pre_acc_code = am.master_acc_code) as Child_rec ,decode((select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER" +
            " where  pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD')), null,(select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where  acc_code = am.acc_code  and VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD'))" +
            ",(select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where  pre_acc_code like (am.master_acc_code||'%')  and VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD')))  dr_amt_opening,decode((select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where  pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD')) ,null," +
            "(select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where   acc_code = am.acc_code and VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD')),(select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where  pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD'))" +
            ") cr_amt_opening,decode((select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where  pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE between TO_DATE('" + formDate + "', 'YYYY-MM-DD') and TO_DATE('" + toDate + "', 'YYYY-MM-DD')), null," +
            "(select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where  acc_code = am.acc_code  and VOUCHER_DATE between TO_DATE('" + formDate + "', 'YYYY-MM-DD') and TO_DATE('" + toDate + "', 'YYYY-MM-DD') ),(select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER" +
            " where  pre_acc_code like (am.master_acc_code||'%')  and VOUCHER_DATE between TO_DATE('" + formDate + "', 'YYYY-MM-DD') and TO_DATE('" + toDate + "', 'YYYY-MM-DD') ))  dr_amt ,decode((select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where  pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE between TO_DATE('" + formDate + "', 'YYYY-MM-DD') and TO_DATE('" + toDate + "', 'YYYY-MM-DD')  ) " +
            ",null,(select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where   acc_code = am.acc_code and VOUCHER_DATE between TO_DATE('" + formDate + "', 'YYYY-MM-DD') and TO_DATE('" + toDate + "', 'YYYY-MM-DD') ),(select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER" +
            " where  pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE between TO_DATE('" + formDate + "', 'YYYY-MM-DD') and TO_DATE('" + toDate + "', 'YYYY-MM-DD') )) cr_amt from FA_CHART_OF_ACCOUNTS_SETUP am where deleted_flag = 'N'" +
            "and company_code = '01' and pre_acc_code='00' connect by prior master_acc_code =pre_acc_code))";
                        return _objectEntity.SqlQuery<TrialBalanceViewModel>(Query).ToList();
        }
        public List<TrialBalanceViewModel> GetTrialBalanceChildLedger(string formDate, string toDate, string id)
        {
            if (id != null)
                id = id.Replace("__", "&");
                            string Query = @"select account_head, TreeLevel, Pre_Acc_Code, Master_ACC_Code , acc_code, DR_OPENING, CR_OPENING, dr_amt, cr_amt , nvl(DR_OPENING,0) +  nvl(dr_amt,0) as DR_Closing, nvl(CR_OPENING,0) + nvl(cr_amt,0) as cr_closing, Child_rec from (" +
                "select account_head, TreeLevel, Pre_Acc_Code, Master_ACC_Code , acc_code, CASE WHEN dr_amt_opening-cr_amt_opening >= 0  THEN dr_amt_opening-cr_amt_opening   ELSE 0 end as DR_OPENING," +
                "CASE WHEN dr_amt_opening-cr_amt_opening < 0  THEN ABS(dr_amt_opening-cr_amt_opening )    ELSE 0 end as CR_OPENING,dr_amt, cr_amt , Child_rec from(" +
                "select  (lpad(' ',2*(level-1)) || acc_edesc) account_head, LEVEL AS TreeLevel, Pre_Acc_Code, Master_ACC_Code , acc_code, (select count(*) from FA_CHART_OF_ACCOUNTS_SETUP where pre_acc_code = am.master_acc_code) as Child_rec ,decode((select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where  pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD')" +
                "), null,(select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where  acc_code = am.acc_code  and VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD')),(select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER" +
                " where  pre_acc_code like (am.master_acc_code||'%')  and VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD')))  dr_amt_opening,decode((select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where  pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD')) ,null,(select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER" +
                " where   acc_code = am.acc_code and VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD')),(select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where  pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD'))) cr_amt_opening," +
                "decode((select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where  pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE between TO_DATE('" + formDate + "', 'YYYY-MM-DD') and TO_DATE('" + toDate + "', 'YYYY-MM-DD') ), null," +
                "(select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where  acc_code = am.acc_code  and VOUCHER_DATE between TO_DATE('" + formDate + "', 'YYYY-MM-DD') and TO_DATE('" + toDate + "', 'YYYY-MM-DD') ),(select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER" +
                " where  pre_acc_code like (am.master_acc_code||'%')  and VOUCHER_DATE between TO_DATE('" + formDate + "', 'YYYY-MM-DD') and TO_DATE('" + toDate + "', 'YYYY-MM-DD') ))  dr_amt ," +
                "decode((select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where  pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE between TO_DATE('" + formDate + "', 'YYYY-MM-DD') and TO_DATE('" + toDate + "', 'YYYY-MM-DD')  ) ,null," +
                "(select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where   acc_code = am.acc_code and VOUCHER_DATE between TO_DATE('" + formDate + "', 'YYYY-MM-DD') and TO_DATE('" + toDate + "', 'YYYY-MM-DD') ),(select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER" +
                " where  pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE between TO_DATE('" + formDate + "', 'YYYY-MM-DD') and TO_DATE('" + toDate + "', 'YYYY-MM-DD') )) cr_amt from FA_CHART_OF_ACCOUNTS_SETUP am" +
                " where deleted_flag = 'N' and company_code = '01' and Pre_Acc_Code='" + id + @"' and Level=2 connect by prior master_acc_code =pre_acc_code))";
                 return _objectEntity.SqlQuery<TrialBalanceViewModel>(Query).ToList();
        }


        //    public List<TrialBalanceViewModel> GetTrialBalanceMasterGrid(string formDate, string toDate)
        //    {
        //        string Query = @"select account_head, TreeLevel, Pre_Acc_Code,Pre_Acc_Code as parentId,TO_NUMBER(REPLACE(Pre_Acc_Code,'.','')) as ParentIdInt , TO_NUMBER(REPLACE(Master_ACC_Code,'.','')) as Id, Master_ACC_Code , acc_code, DR_OPENING, CR_OPENING, dr_amt, cr_amt , nvl(DR_OPENING,0) +  nvl(dr_amt,0) as DR_Closing, nvl(CR_OPENING,0) + nvl(cr_amt,0) as cr_closing, Child_rec from (select account_head, TreeLevel, Pre_Acc_Code, Master_ACC_Code , acc_code,   " +
        //"CASE WHEN dr_amt_opening-cr_amt_opening >= 0  THEN dr_amt_opening-cr_amt_opening  ELSE 0 end as DR_OPENING,CASE WHEN dr_amt_opening-cr_amt_opening < 0  THEN ABS(dr_amt_opening-cr_amt_opening )ELSE 0" +
        //"end as CR_OPENING,dr_amt, cr_amt , Child_rec from(select  (lpad(' ',2*(level-1)) || acc_edesc) account_head, LEVEL AS TreeLevel, Pre_Acc_Code, Master_ACC_Code , acc_code, (select count(*) from FA_CHART_OF_ACCOUNTS_SETUP where pre_acc_code = am.master_acc_code) as Child_rec ,decode((select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER" +
        //" where  pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD')), null,(select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where  acc_code = am.acc_code  and VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD'))" +
        //",(select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where  pre_acc_code like (am.master_acc_code||'%')  and VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD')))  dr_amt_opening,decode((select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where  pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD')) ,null," +
        //"(select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where   acc_code = am.acc_code and VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD')),(select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where  pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD'))" +
        //") cr_amt_opening,decode((select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where  pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE between TO_DATE('" + formDate + "', 'YYYY-MM-DD') and TO_DATE('" + toDate + "', 'YYYY-MM-DD')), null," +
        //"(select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where  acc_code = am.acc_code  and VOUCHER_DATE between TO_DATE('" + formDate + "', 'YYYY-MM-DD') and TO_DATE('" + toDate + "', 'YYYY-MM-DD') ),(select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER" +
        //" where  pre_acc_code like (am.master_acc_code||'%')  and VOUCHER_DATE between TO_DATE('" + formDate + "', 'YYYY-MM-DD') and TO_DATE('" + toDate + "', 'YYYY-MM-DD') ))  dr_amt ,decode((select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where  pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE between TO_DATE('" + formDate + "', 'YYYY-MM-DD') and TO_DATE('" + toDate + "', 'YYYY-MM-DD')  ) " +
        //",null,(select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where   acc_code = am.acc_code and VOUCHER_DATE between TO_DATE('" + formDate + "', 'YYYY-MM-DD') and TO_DATE('" + toDate + "', 'YYYY-MM-DD') ),(select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER" +
        //" where  pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE between TO_DATE('" + formDate + "', 'YYYY-MM-DD') and TO_DATE('" + toDate + "', 'YYYY-MM-DD') )) cr_amt from FA_CHART_OF_ACCOUNTS_SETUP am where deleted_flag = 'N'" +
        //"and company_code = '01' and pre_acc_code='00' connect by prior master_acc_code =pre_acc_code))";
        //            return _objectEntity.SqlQuery<TrialBalanceViewModel>(Query).ToList();
        //    }
        public class bCode {
            public string BranchCode { get; set; }
        }
        public List<TrialBalanceViewModel> GetTrialBalanceChildLedgerTrial(TrialBalancefilterOption model, NeoErp.Core.Domain.User userinfo, int id=0,string dataGeneric="DG")
        {
            //if (id != null)
            //    id = id.Replace("__", "&");

            // Old Query that Given by Rajesh 

            //    string Query = string.Format(@"select account_head, TreeLevel, Pre_Acc_Code,Pre_Acc_Code as parentId,TO_NUMBER(REPLACE(Pre_Acc_Code,'.','')) as ParentIdInt,TO_NUMBER(REPLACE(Master_ACC_Code,'.','')) as Id, Master_ACC_Code , acc_code, DR_OPENING, CR_OPENING, dr_amt, cr_amt , nvl(DR_OPENING,0) +  nvl(dr_amt,0) as DR_Closing, nvl(CR_OPENING,0) + nvl(cr_amt,0) as cr_closing, Child_rec,  (select count(*) from fa_sub_ledger_map where company_code='01' and acc_code = final_data.acc_code) as sub_count from (" +
            //"select account_head, TreeLevel, Pre_Acc_Code, Master_ACC_Code , acc_code, CASE WHEN dr_amt_opening-cr_amt_opening >= 0  THEN dr_amt_opening-cr_amt_opening   ELSE 0 end as DR_OPENING," +
            //"CASE WHEN dr_amt_opening-cr_amt_opening < 0  THEN ABS(dr_amt_opening-cr_amt_opening )    ELSE 0 end as CR_OPENING,dr_amt, cr_amt , Child_rec from(" +
            //"select  (lpad(' ',2*(level-1)) || acc_edesc) account_head, LEVEL AS TreeLevel, Pre_Acc_Code, Master_ACC_Code , acc_code, (select count(*) from FA_CHART_OF_ACCOUNTS_SETUP where company_code='01' and  pre_acc_code = am.master_acc_code) as Child_rec ,decode((select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where company_code='01' and pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD')" +
            //"), null,(select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where company_code='01' and acc_code = am.acc_code  and VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD')),(select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER" +
            //" where company_code='01' and  pre_acc_code like (am.master_acc_code||'%')  and VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD')))  dr_amt_opening,decode((select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where company_code='01' and  pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD')) ,null,(select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER" +
            //" where company_code='01' and   acc_code = am.acc_code and VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD')),(select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where company_code='01' and  pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD'))) cr_amt_opening," +
            //"decode((select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where company_code='01' and pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE between TO_DATE('" + formDate + "', 'YYYY-MM-DD') and TO_DATE('" + toDate + "', 'YYYY-MM-DD') ), null," +
            //"(select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where company_code='01' and  acc_code = am.acc_code  and VOUCHER_DATE between TO_DATE('" + formDate + "', 'YYYY-MM-DD') and TO_DATE('" + toDate + "', 'YYYY-MM-DD') ),(select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER" +
            //" where company_code='01' and  pre_acc_code like (am.master_acc_code||'%')  and VOUCHER_DATE between TO_DATE('" + formDate + "', 'YYYY-MM-DD') and TO_DATE('" + toDate + "', 'YYYY-MM-DD') ))  dr_amt ," +
            //"decode((select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where company_code='01' and  pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE between TO_DATE('" + formDate + "', 'YYYY-MM-DD') and TO_DATE('" + toDate + "', 'YYYY-MM-DD')  ) ,null," +
            //"(select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where  company_code='01' and acc_code = am.acc_code and VOUCHER_DATE between TO_DATE('" + formDate + "', 'YYYY-MM-DD') and TO_DATE('" + toDate + "', 'YYYY-MM-DD') ),(select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER" +
            //" where company_code='01' and pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE between TO_DATE('" + formDate + "', 'YYYY-MM-DD') and TO_DATE('" + toDate + "', 'YYYY-MM-DD') )) cr_amt from FA_CHART_OF_ACCOUNTS_SETUP am" +
            //" where deleted_flag = 'N' and company_code = '01'  and TO_NUMBER(REPLACE(Pre_Acc_Code,'.',''))='{0}' and Level={1} connect by prior master_acc_code =pre_acc_code)) final_data", id,1);

            // // End Old Query that Given by Rajesh 
            var branchCode = "";
            var companyCode = "";
            if (userinfo == null)
            {
                userinfo = new Core.Domain.User();
                userinfo.company_code = "01";
                userinfo.branch_code = "01.01";

            }
            else if (string.IsNullOrEmpty(userinfo.company_code))
            {
                userinfo.company_code = "01";
                userinfo.branch_code = userinfo.branch_code;
            }
            else if (string.IsNullOrEmpty(userinfo.branch_code))
            {
                userinfo.company_code = userinfo.company_code;
                userinfo.branch_code = "01.01";
            }

            if (model.ReportFilters.BranchFilter.Count > 0)
            {
                branchCode = string.Format(string.Join("','", model.ReportFilters.BranchFilter).ToString());
            }
            else {
                branchCode= string.Format(string.Join("','", userinfo.branch_code).ToString());
            }
         
            if (dataGeneric == "DG")
            {
                string Query = @"SELECT
                                   account_head,
                                   TreeLevel,
                                   Pre_Acc_Code,
                                   Pre_Acc_Code AS parentId,
                                   TO_NUMBER (REPLACE (Pre_Acc_Code,
                                   '.',
                                   '')) AS ParentIdInt,
                                   TO_NUMBER (REPLACE (Master_ACC_Code,
                                   '.',
                                   '')) AS Id,
                                   Master_ACC_Code,
                                   acc_code,
                                   nvl(dr_amt_opening,
                                   0) as DR_OPENING,
                                   nvl(cr_amt_opening,
                                   0) as CR_OPENING,
                                   nvl(dr_amt,
                                   0) as dr_amt,
                                   nvl(cr_amt,
                                   0) as cr_amt,
                                   CASE 
                                      WHEN (nvl(dr_amt,
                                      0) + nvl(dr_amt_opening,
                                      0)) - (nvl(cr_amt_opening,
                                      0) + nvl(cr_amt,
                                      0)) >= 0  THEN    (nvl(dr_amt,
                                      0) + nvl(dr_amt_opening,
                                      0)) - (nvl(cr_amt_opening,
                                      0) + nvl(cr_amt,
                                      0))  
                                      ELSE    0  
                                   END  AS DR_Closing,
                                   CASE     
                                      WHEN(nvl(dr_amt,
                                      0) + nvl(dr_amt_opening,
                                      0)) - (nvl(cr_amt_opening,
                                      0) + nvl(cr_amt,
                                      0)) < 0    THEN       ABS((nvl(dr_amt,
                                      0) + nvl(dr_amt_opening,
                                      0)) - (nvl(cr_amt_opening,
                                      0) + nvl(cr_amt,
                                      0)))  
                                      ELSE    0      
                                   END   AS cr_closing,
                                   Child_rec,
                                   (SELECT
                                      COUNT(*)   
                                   FROM
                                      fa_sub_ledger_map    
                                   WHERE
                                      branch_code in ('" + branchCode + @"') 
                                          and deleted_flag='N'
                                     -- AND
                                      --company_code = '" + userinfo.company_code + @"' 
                                      AND trim(acc_code) = trim(final_data.acc_code)) AS sub_count    
                                FROM
                                   (SELECT
                                      account_head,
                                      TreeLevel,
                                      Pre_Acc_Code,
                                      Master_ACC_Code,
                                      acc_code,
                                      dr_amt_opening,
                                      cr_amt_opening,
                                      dr_amt,
                                      cr_amt,
                                      Child_rec    
                                   FROM
                                      (SELECT
                                         (LPAD(' ',
                                         2 * (LEVEL - 1)) || acc_edesc)    account_head,
                                         LEVEL AS TreeLevel,
                                         Pre_Acc_Code,
                                         Master_ACC_Code,
                                         acc_code,
                                         (SELECT
                                            COUNT(*)  
                                         FROM
                                            FA_CHART_OF_ACCOUNTS_SETUP  
                                         WHERE
                                           -- branch_code in ('" + branchCode + @"') 
                                          -- AND
                                           company_code = '" + userinfo.company_code + @"'     
                                               and deleted_flag='N'
                                            AND trim(pre_acc_code) = trim(am.master_acc_code))    AS Child_rec,
                                         DECODE((SELECT
                                            SUM(dr_aMOUNT* NVL(EXCHANGE_RATE,1))    
                                         FROM
                                            V$VIRTUAL_GENERAL_LEDGER    
                                         WHERE
                                            branch_code in ('" + branchCode + @"') 
                                               and deleted_flag='N'
                                           -- AND
                                          --  company_code = '" + userinfo.company_code + @"'  
                                            AND trim(pre_acc_code) LIKE(trim(am.master_acc_code) || '%') 
                                            AND (trunc(VOUCHER_DATE) <   trunc(TO_DATE('" + model.ReportFilters.FromDate + @"', 'YYYY-MM-DD')) OR FORM_CODE='0')),
                                         NULL,
                                         (SELECT
                                            SUM(dr_aMOUNT* NVL(EXCHANGE_RATE,1))     
                                         FROM
                                            V$VIRTUAL_GENERAL_LEDGER  
                                         WHERE
                                            branch_code in ('" + branchCode + @"') 
                                               and deleted_flag='N'
                                           -- AND
                                           -- company_code = '" + userinfo.company_code + @"'  
                                            AND trim(acc_code) = trim(am.acc_code)
                                            AND (trunc(VOUCHER_DATE) <   trunc(TO_DATE('" + model.ReportFilters.FromDate + @"', 'YYYY-MM-DD')) OR FORM_CODE='0')),
                                         (SELECT
                                            SUM(dr_aMOUNT* NVL(EXCHANGE_RATE,1))    
                                         FROM
                                            V$VIRTUAL_GENERAL_LEDGER 
                                         WHERE
                                            branch_code in ('" + branchCode + @"') 
                                    and deleted_flag='N'
                                          --  AND
                                          --  company_code = '" + userinfo.company_code + @"'    
                                            AND trim(pre_acc_code) LIKE(trim(am.master_acc_code) || '%')    
                                            AND (trunc(VOUCHER_DATE) <   trunc(TO_DATE('" + model.ReportFilters.FromDate + @"', 'YYYY-MM-DD')) OR FORM_CODE='0')))  dr_amt_opening,
                                         DECODE((SELECT
                                            SUM(cr_aMOUNT* NVL(EXCHANGE_RATE,1)) 
                                         FROM
                                            V$VIRTUAL_GENERAL_LEDGER 
                                         WHERE
                                            branch_code in ('" + branchCode + @"') 
                                             and deleted_flag='N'
                                           -- AND
                                          --  company_code = '" + userinfo.company_code + @"'  
                                            AND trim(pre_acc_code) LIKE(trim(am.master_acc_code) || '%')    
                                            AND (trunc(VOUCHER_DATE) <   trunc(TO_DATE('" + model.ReportFilters.FromDate + @"', 'YYYY-MM-DD')) OR FORM_CODE='0' )),
                                         NULL,
                                         (SELECT
                                            SUM(cr_aMOUNT* NVL(EXCHANGE_RATE,1))  
                                         FROM
                                            V$VIRTUAL_GENERAL_LEDGER  
                                         WHERE
                                            branch_code in ('" + branchCode + @"') 
                                                and deleted_flag='N'
                                          --  AND
                                          --  company_code = '" + userinfo.company_code + @"'   
                                            AND trim(acc_code) = trim(am.acc_code) 
                                            AND (trunc(VOUCHER_DATE) <   trunc(TO_DATE('" + model.ReportFilters.FromDate + @"', 'YYYY-MM-DD')) OR FORM_CODE='0')),
                                         (SELECT
                                            SUM(cr_aMOUNT* NVL(EXCHANGE_RATE,1))  
                                         FROM
                                            V$VIRTUAL_GENERAL_LEDGER 
                                         WHERE
                                            branch_code in ('" + branchCode + @"') 
                                                and deleted_flag='N'
                                            --AND
                                            --company_code = '" + userinfo.company_code + @"'  
                                            AND trim(pre_acc_code) LIKE(trim(am.master_acc_code) || '%')  
                                            AND (trunc(VOUCHER_DATE) <   trunc(TO_DATE('" + model.ReportFilters.FromDate + @"', 'YYYY-MM-DD')) OR FORM_CODE='0' )))  cr_amt_opening,
                                         DECODE((SELECT
                                            SUM(dr_aMOUNT* NVL(EXCHANGE_RATE,1))  
                                         FROM
                                            V$VIRTUAL_GENERAL_LEDGER 
                                         WHERE
                                            branch_code in ('" + branchCode + @"') 
                                               and deleted_flag='N'
                                         --   AND
                                           -- company_code = '" + userinfo.company_code + @"'  
                                            AND trim(pre_acc_code) LIKE        (trim(am.master_acc_code) || '%')    
                                            AND FORM_CODE <> '0'   
                                            AND trunc(VOUCHER_DATE) BETWEEN trunc(TO_DATE('" + model.ReportFilters.FromDate + @"', 'YYYY-MM-DD'))         AND TO_DATE('" + model.ReportFilters.ToDate + @"', 'YYYY-MM-DD')),
                                         NULL,
                                         (SELECT
                                            SUM(dr_aMOUNT* NVL(EXCHANGE_RATE,1)) 
                                         FROM
                                            V$VIRTUAL_GENERAL_LEDGER 
                                         WHERE
                                            branch_code in ('" + branchCode + @"') 
                                               and deleted_flag='N'
                                          --  AND
                                           -- company_code = '" + userinfo.company_code + @"'  
                                            AND trim(acc_code) = trim(am.acc_code)  
                                            AND FORM_CODE <> '0'   
                                            AND trunc(VOUCHER_DATE) BETWEEN trunc(TO_DATE ('" + model.ReportFilters.FromDate + @"', 'YYYY-MM-DD'))        AND trunc(TO_DATE ('" + model.ReportFilters.ToDate + @"', 'YYYY-MM-DD'))),
                                         (SELECT
                                            SUM(dr_aMOUNT* NVL(EXCHANGE_RATE,1)) 
                                         FROM
                                            V$VIRTUAL_GENERAL_LEDGER 
                                         WHERE
                                            branch_code in ('" + branchCode + @"') 
                                               and deleted_flag='N'
                                       --     AND
                                         --   company_code = '" + userinfo.company_code + @"'     
                                            AND FORM_CODE <> '0'     
                                            AND trim(pre_acc_code) LIKE(trim(am.master_acc_code) || '%') 
                                            AND trunc(VOUCHER_DATE) BETWEEN trunc(TO_DATE ('" + model.ReportFilters.FromDate + @"', 'YYYY-MM-DD'))   AND trunc(TO_DATE ('" + model.ReportFilters.ToDate + @"', 'YYYY-MM-DD')))) dr_amt,
                                         DECODE((SELECT
                                            SUM(cr_aMOUNT* NVL(EXCHANGE_RATE,1))   
                                         FROM
                                            V$VIRTUAL_GENERAL_LEDGER   
                                         WHERE
                                            branch_code in ('" + branchCode + @"') 
                                                and deleted_flag='N'
                                         --   AND
                                          --  company_code = '" + userinfo.company_code + @"'   
                                            AND trim(pre_acc_code) LIKE(trim(am.master_acc_code) || '%')   
                                            AND FORM_CODE <> '0'   
                                            AND trunc(VOUCHER_DATE) BETWEEN trunc(TO_DATE('" + model.ReportFilters.FromDate + @"', 'YYYY-MM-DD'))   AND trunc(TO_DATE('" + model.ReportFilters.ToDate + @"', 'YYYY-MM-DD'))),
                                         NULL,
                                         (SELECT
                                            SUM(cr_aMOUNT* NVL(EXCHANGE_RATE,1)) 
                                         FROM
                                            V$VIRTUAL_GENERAL_LEDGER   
                                         WHERE
                                            branch_code in ('" + branchCode + @"') 
                                              and deleted_flag='N'
                                           -- AND
                                         --   company_code = '" + userinfo.company_code + @"'  
                                            AND trim(acc_code) = trim(am.acc_code)
                                            AND FORM_CODE <> '0' 
                                            AND trunc(VOUCHER_DATE) BETWEEN trunc(TO_DATE ('" + model.ReportFilters.FromDate + @"', 'YYYY-MM-DD'))     AND trunc(TO_DATE ('" + model.ReportFilters.ToDate + @"', 'YYYY-MM-DD'))),
                                         (SELECT
                                            SUM(cr_aMOUNT* NVL(EXCHANGE_RATE,1)) 
                                         FROM
                                            V$VIRTUAL_GENERAL_LEDGER   
                                         WHERE
                                            branch_code in ('" + branchCode + @"') 
                                               and deleted_flag='N'
                                          --  AND
                                          --  company_code = '" + userinfo.company_code + @"' 
                                            AND trim(pre_acc_code) LIKE(trim(am.master_acc_code) || '%') 
                                            AND FORM_CODE <> '0'   
                                            AND trunc(VOUCHER_DATE) BETWEEN trunc(TO_DATE ('" + model.ReportFilters.FromDate + @"', 'YYYY-MM-DD')) AND trunc(TO_DATE ('" + model.ReportFilters.ToDate + @"', 'YYYY-MM-DD')))) cr_amt   
                                      FROM
                                         FA_CHART_OF_ACCOUNTS_SETUP am 
                                      WHERE
                                         deleted_flag = 'N' 
                                         AND company_code =  '" + userinfo.company_code + @"' 
                                         AND LEVEL = 1";
               
                //if (model.ReportFilters.BranchFilter.Count > 0)
                //{
                //    Query += string.Format(@" AND BRANCH_CODE IN ('{0}')", string.Join("','", model.ReportFilters.BranchFilter).ToString());
                //}

                Query += "CONNECT BY PRIOR trim(master_acc_code) = trim(pre_acc_code) start with TO_NUMBER(REPLACE(Pre_Acc_Code, '.', '')) = '" + id + "')) final_data";
                return _objectEntity.SqlQuery<TrialBalanceViewModel>(Query).ToList();
            }
            else
            {
                string Query = @"SELECT
                                   account_head,
                                   TreeLevel,
                                   Pre_Acc_Code,
                                   Pre_Acc_Code AS parentId,
                                   TO_NUMBER (REPLACE (Pre_Acc_Code,
                                   '.',
                                   '')) AS ParentIdInt,
                                   TO_NUMBER (REPLACE (Master_ACC_Code,
                                   '.',
                                   '')) AS Id,
                                   Master_ACC_Code,
                                   acc_code,
                                   nvl(dr_amt_opening,
                                   0) as DR_OPENING,
                                   nvl(cr_amt_opening,
                                   0) as CR_OPENING,
                                   nvl(dr_amt,
                                   0) as dr_amt,
                                   nvl(cr_amt,
                                   0) as cr_amt,
                                   CASE 
                                      WHEN (nvl(dr_amt,
                                      0) + nvl(dr_amt_opening,
                                      0)) - (nvl(cr_amt_opening,
                                      0) + nvl(cr_amt,
                                      0)) >= 0  THEN    (nvl(dr_amt,
                                      0) + nvl(dr_amt_opening,
                                      0)) - (nvl(cr_amt_opening,
                                      0) + nvl(cr_amt,
                                      0))  
                                      ELSE    0  
                                   END  AS DR_Closing,
                                   CASE     
                                      WHEN(nvl(dr_amt,
                                      0) + nvl(dr_amt_opening,
                                      0)) - (nvl(cr_amt_opening,
                                      0) + nvl(cr_amt,
                                      0)) < 0    THEN       ABS((nvl(dr_amt,
                                      0) + nvl(dr_amt_opening,
                                      0)) - (nvl(cr_amt_opening,
                                      0) + nvl(cr_amt,
                                      0)))  
                                      ELSE    0      
                                   END   AS cr_closing,
                                   Child_rec,
                                   (SELECT
                                      COUNT(*)   
                                   FROM
                                      fa_sub_ledger_map    
                                   WHERE
                                      branch_code in ('" + branchCode + @"') 
                                         and deleted_flag='N'
                                     -- AND
                                      --company_code = '" + userinfo.company_code + @"' 
                                      AND trim(acc_code) = trim(final_data.acc_code)) AS sub_count    
                                FROM
                                   (SELECT
                                      account_head,
                                      TreeLevel,
                                      Pre_Acc_Code,
                                      Master_ACC_Code,
                                      acc_code,
                                      dr_amt_opening,
                                      cr_amt_opening,
                                      dr_amt,
                                      cr_amt,
                                      Child_rec    
                                   FROM
                                      (SELECT
                                         (LPAD(' ',
                                         2 * (LEVEL - 1)) || acc_edesc)    account_head,
                                         LEVEL AS TreeLevel,
                                         Pre_Acc_Code,
                                         Master_ACC_Code,
                                         acc_code,
                                         (SELECT
                                            COUNT(*)  
                                         FROM
                                            FA_CHART_OF_ACCOUNTS_SETUP  
                                         WHERE
                                           -- branch_code in ('" + branchCode + @"') 
                                          -- AND
                                           company_code = '" + userinfo.company_code + @"'      
                                            AND trim(pre_acc_code) = trim(am.master_acc_code))    AS Child_rec,
                                         DECODE((SELECT
                                            SUM(dr_aMOUNT* NVL(EXCHANGE_RATE,1))    
                                         FROM
                                            V$VIRTUAL_GENERAL_LEDGER    
                                         WHERE
                                            branch_code in ('" + branchCode + @"') 
                                                 and deleted_flag='N'
                                              AND POSTED_BY IS NOT NULL  
                                           -- AND
                                          --  company_code = '" + userinfo.company_code + @"'  
                                            AND trim(pre_acc_code) LIKE(trim(am.master_acc_code) || '%') 
                                            AND (trunc(VOUCHER_DATE) <   trunc(TO_DATE('" + model.ReportFilters.FromDate + @"', 'YYYY-MM-DD')) OR FORM_CODE='0' )),
                                         NULL,
                                         (SELECT
                                            SUM(dr_aMOUNT* NVL(EXCHANGE_RATE,1))     
                                         FROM
                                            V$VIRTUAL_GENERAL_LEDGER  
                                         WHERE
                                            branch_code in ('" + branchCode + @"') 
                                                and deleted_flag='N'
                                             AND POSTED_BY IS NOT NULL  
                                           -- AND
                                           -- company_code = '" + userinfo.company_code + @"'  
                                            AND trim(acc_code) = trim(am.acc_code)
                                            AND (trunc(VOUCHER_DATE) <   trunc(TO_DATE('" + model.ReportFilters.FromDate + @"', 'YYYY-MM-DD')) OR FORM_CODE='0' )),
                                         (SELECT
                                            SUM(dr_aMOUNT* NVL(EXCHANGE_RATE,1))    
                                         FROM
                                            V$VIRTUAL_GENERAL_LEDGER 
                                         WHERE
                                            branch_code in ('" + branchCode + @"') 
                                             and deleted_flag='N'
                                             AND POSTED_BY IS NOT NULL  
                                          --  AND
                                          --  company_code = '" + userinfo.company_code + @"'    
                                            AND trim(pre_acc_code) LIKE(trim(am.master_acc_code) || '%')    
                                            AND (trunc(VOUCHER_DATE) <   trunc(TO_DATE('" + model.ReportFilters.FromDate + @"', 'YYYY-MM-DD')) OR FORM_CODE='0' )))  dr_amt_opening,
                                         DECODE((SELECT
                                            SUM(cr_aMOUNT* NVL(EXCHANGE_RATE,1)) 
                                         FROM
                                            V$VIRTUAL_GENERAL_LEDGER 
                                         WHERE
                                            branch_code in ('" + branchCode + @"') 
                                                and deleted_flag='N'
                                             AND POSTED_BY IS NOT NULL  
                                           -- AND
                                          --  company_code = '" + userinfo.company_code + @"'  
                                            AND trim(pre_acc_code) LIKE(trim(am.master_acc_code) || '%')    
                                            AND (trunc(VOUCHER_DATE) <   trunc(TO_DATE('" + model.ReportFilters.FromDate + @"', 'YYYY-MM-DD')) OR FORM_CODE='0' )),
                                         NULL,
                                         (SELECT
                                            SUM(cr_aMOUNT* NVL(EXCHANGE_RATE,1))  
                                         FROM
                                            V$VIRTUAL_GENERAL_LEDGER  
                                         WHERE
                                            branch_code in ('" + branchCode + @"') 
                                               and deleted_flag='N'
                                             AND POSTED_BY IS NOT NULL  
                                          --  AND
                                          --  company_code = '" + userinfo.company_code + @"'   
                                            AND trim(acc_code) = trim(am.acc_code) 
                                            AND (trunc(VOUCHER_DATE) <   trunc(TO_DATE('" + model.ReportFilters.FromDate + @"', 'YYYY-MM-DD')) OR FORM_CODE='0' )),
                                         (SELECT
                                            SUM(cr_aMOUNT* NVL(EXCHANGE_RATE,1))  
                                         FROM
                                            V$VIRTUAL_GENERAL_LEDGER 
                                         WHERE
                                            branch_code in ('" + branchCode + @"') 
                                               and deleted_flag='N'
                                             AND POSTED_BY IS NOT NULL  
                                            --AND
                                            --company_code = '" + userinfo.company_code + @"'  
                                            AND trim(pre_acc_code) LIKE(trim(am.master_acc_code) || '%')  
                                            AND (trunc(VOUCHER_DATE) <   trunc(TO_DATE('" + model.ReportFilters.FromDate + @"', 'YYYY-MM-DD')) OR FORM_CODE='0' )))  cr_amt_opening,
                                         DECODE((SELECT
                                            SUM(dr_aMOUNT* NVL(EXCHANGE_RATE,1))  
                                         FROM
                                            V$VIRTUAL_GENERAL_LEDGER 
                                         WHERE
                                            branch_code in ('" + branchCode + @"') 
                                               and deleted_flag='N'
                                             AND POSTED_BY IS NOT NULL  
                                         --   AND
                                           -- company_code = '" + userinfo.company_code + @"'  
                                            AND trim(pre_acc_code) LIKE        (trim(am.master_acc_code) || '%')    
                                            AND FORM_CODE <> '0'   
                                            AND trunc(VOUCHER_DATE) BETWEEN trunc(TO_DATE('" + model.ReportFilters.FromDate + @"', 'YYYY-MM-DD'))         AND TO_DATE('" + model.ReportFilters.ToDate + @"', 'YYYY-MM-DD')),
                                         NULL,
                                         (SELECT
                                            SUM(dr_aMOUNT* NVL(EXCHANGE_RATE,1)) 
                                         FROM
                                            V$VIRTUAL_GENERAL_LEDGER 
                                         WHERE
                                            branch_code in ('" + branchCode + @"') 
                                                and deleted_flag='N'
                                             AND POSTED_BY IS NOT NULL  
                                          --  AND
                                           -- company_code = '" + userinfo.company_code + @"'  
                                            AND trim(acc_code) = trim(am.acc_code)  
                                            AND FORM_CODE <> '0'   
                                            AND trunc(VOUCHER_DATE) BETWEEN trunc(TO_DATE ('" + model.ReportFilters.FromDate + @"', 'YYYY-MM-DD'))        AND trunc(TO_DATE ('" + model.ReportFilters.ToDate + @"', 'YYYY-MM-DD'))),
                                         (SELECT
                                            SUM(dr_aMOUNT* NVL(EXCHANGE_RATE,1)) 
                                         FROM
                                            V$VIRTUAL_GENERAL_LEDGER 
                                         WHERE
                                            branch_code in ('" + branchCode + @"') 
                                                 and deleted_flag='N'
                               AND POSTED_BY IS NOT NULL  
                                       --     AND
                                         --   company_code = '" + userinfo.company_code + @"'     
                                            AND FORM_CODE <> '0'     
                                            AND trim(pre_acc_code) LIKE(trim(am.master_acc_code) || '%') 
                                            AND trunc(VOUCHER_DATE) BETWEEN trunc(TO_DATE ('" + model.ReportFilters.FromDate + @"', 'YYYY-MM-DD'))   AND trunc(TO_DATE ('" + model.ReportFilters.ToDate + @"', 'YYYY-MM-DD')))) dr_amt,
                                         DECODE((SELECT
                                            SUM(cr_aMOUNT* NVL(EXCHANGE_RATE,1))   
                                         FROM
                                            V$VIRTUAL_GENERAL_LEDGER   
                                         WHERE
                                            branch_code in ('" + branchCode + @"') 
                                               and deleted_flag='N'
                                               AND POSTED_BY IS NOT NULL  
                                         --   AND
                                          --  company_code = '" + userinfo.company_code + @"'   
                                            AND trim(pre_acc_code) LIKE(trim(am.master_acc_code) || '%')   
                                            AND FORM_CODE <> '0'   
                                            AND trunc(VOUCHER_DATE) BETWEEN trunc(TO_DATE('" + model.ReportFilters.FromDate + @"', 'YYYY-MM-DD'))   AND trunc(TO_DATE('" + model.ReportFilters.ToDate + @"', 'YYYY-MM-DD'))),
                                         NULL,
                                         (SELECT
                                            SUM(cr_aMOUNT* NVL(EXCHANGE_RATE,1)) 
                                         FROM
                                            V$VIRTUAL_GENERAL_LEDGER   
                                         WHERE
                                            branch_code in ('" + branchCode + @"') 
                                                 and deleted_flag='N'
                                              AND POSTED_BY IS NOT NULL  
                                           -- AND
                                         --   company_code = '" + userinfo.company_code + @"'  
                                            AND trim(acc_code) = trim(am.acc_code)
                                            AND FORM_CODE <> '0' 
                                            AND trunc(VOUCHER_DATE) BETWEEN trunc(TO_DATE ('" + model.ReportFilters.FromDate + @"', 'YYYY-MM-DD'))     AND trunc(TO_DATE ('" + model.ReportFilters.ToDate + @"', 'YYYY-MM-DD'))),
                                         (SELECT
                                            SUM(cr_aMOUNT* NVL(EXCHANGE_RATE,1)) 
                                         FROM
                                            V$VIRTUAL_GENERAL_LEDGER   
                                         WHERE
                                            branch_code in ('" + branchCode + @"') 
                                              and deleted_flag='N' AND POSTED_BY IS NOT NULL  
                                          --  AND
                                          --  company_code = '" + userinfo.company_code + @"' 
                                            AND trim(pre_acc_code) LIKE(trim(am.master_acc_code) || '%') 
                                            AND FORM_CODE <> '0'   
                                            AND trunc(VOUCHER_DATE) BETWEEN trunc(TO_DATE ('" + model.ReportFilters.FromDate + @"', 'YYYY-MM-DD')) AND trunc(TO_DATE ('" + model.ReportFilters.ToDate + @"', 'YYYY-MM-DD')))) cr_amt   
                                      FROM
                                         FA_CHART_OF_ACCOUNTS_SETUP am 
                                      WHERE
                                         deleted_flag = 'N' 
                                         AND company_code =  '" + userinfo.company_code + @"' 
                                         AND LEVEL = 1";

                //if (model.ReportFilters.BranchFilter.Count > 0)
                //{
                //    Query += string.Format(@" AND BRANCH_CODE IN ('{0}')", string.Join("','", model.ReportFilters.BranchFilter).ToString());
                //}

                Query += "CONNECT BY PRIOR trim(master_acc_code) = trim(pre_acc_code) start with TO_NUMBER(REPLACE(Pre_Acc_Code, '.', '')) = '" + id + "')) final_data";
                return _objectEntity.SqlQuery<TrialBalanceViewModel>(Query).ToList();
              
            }
          

         
        }
        public List<TrialBalanceViewModel> GetTrialBalanceAllTree(string formDate, string toDate)
        {
            string Query = @"select account_head, TreeLevel, Pre_Acc_Code, Master_ACC_Code , acc_code, DR_OPENING, CR_OPENING, dr_amt, cr_amt , nvl(DR_OPENING,0) +  nvl(dr_amt,0) as DR_Closing, nvl(CR_OPENING,0) + nvl(cr_amt,0) as cr_closing, Child_rec from (select account_head, TreeLevel, Pre_Acc_Code, Master_ACC_Code , acc_code,   " +
"CASE WHEN dr_amt_opening-cr_amt_opening >= 0  THEN dr_amt_opening-cr_amt_opening  ELSE 0 end as DR_OPENING,CASE WHEN dr_amt_opening-cr_amt_opening < 0  THEN ABS(dr_amt_opening-cr_amt_opening )ELSE 0" +
"end as CR_OPENING,dr_amt, cr_amt , Child_rec from(select  (lpad(' ',2*(level-1)) || acc_edesc) account_head, LEVEL AS TreeLevel, Pre_Acc_Code, Master_ACC_Code , acc_code, (select count(*) from FA_CHART_OF_ACCOUNTS_SETUP where pre_acc_code = am.master_acc_code) as Child_rec ,decode((select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER" +
" where  pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD')), null,(select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where  acc_code = am.acc_code  and VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD'))" +
",(select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where  pre_acc_code like (am.master_acc_code||'%')  and VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD')))  dr_amt_opening,decode((select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where  pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD')) ,null," +
"(select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where   acc_code = am.acc_code and VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD')),(select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where  pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD'))" +
") cr_amt_opening,decode((select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where  pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE between TO_DATE('" + formDate + "', 'YYYY-MM-DD') and TO_DATE('" + toDate + "', 'YYYY-MM-DD')), null," +
"(select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where  acc_code = am.acc_code  and VOUCHER_DATE between TO_DATE('" + formDate + "', 'YYYY-MM-DD') and TO_DATE('" + toDate + "', 'YYYY-MM-DD') ),(select sum(dr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER" +
" where  pre_acc_code like (am.master_acc_code||'%')  and VOUCHER_DATE between TO_DATE('" + formDate + "', 'YYYY-MM-DD') and TO_DATE('" + toDate + "', 'YYYY-MM-DD') ))  dr_amt ,decode((select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where  pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE between TO_DATE('" + formDate + "', 'YYYY-MM-DD') and TO_DATE('" + toDate + "', 'YYYY-MM-DD')  ) " +
",null,(select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER where   acc_code = am.acc_code and VOUCHER_DATE between TO_DATE('" + formDate + "', 'YYYY-MM-DD') and TO_DATE('" + toDate + "', 'YYYY-MM-DD') ),(select sum(cr_aMOUNT) from V$VIRTUAL_GENERAL_LEDGER" +
" where  pre_acc_code like (am.master_acc_code||'%') and VOUCHER_DATE between TO_DATE('" + formDate + "', 'YYYY-MM-DD') and TO_DATE('" + toDate + "', 'YYYY-MM-DD') )) cr_amt from FA_CHART_OF_ACCOUNTS_SETUP am where deleted_flag = 'N'" +
"and company_code = '01'  connect by prior master_acc_code =pre_acc_code))";
            return _objectEntity.SqlQuery<TrialBalanceViewModel>(Query).ToList();
        }

       public List<LedgerDailySummaryModel> GetLedgersummaryByAccCode(string formDate, string toDate, string id)
        {
            if (id != null)
                id = id.Replace("__", "&");

            // Old Query given by Pramod

            //            string Query = @"select MiTi as Miti,Voucher_date as VoucherDate,OP_TYPE as OpeningType,(case when op_type='DR'  then opening else 0 end) OpeningDr,
            //        (case when op_type='CR'  then opening else 0 end) OpeningCr,(case when CL_TYPE='DR'  then CLOSING_AMOUNT else 0 end) ClosingDr,
            //            (case when CL_TYPE='CR'  then CLOSING_AMOUNT else 0 end) ClosingCr ,CL_TYPE as ClosingType,DR_AMOUNT as DrAmount, CR_AMOUNT as CrAmount from (SELECT DISTINCT VOUCHER_MITI MITI, VOUCHER_DATE, 
            //            abs( SUM(NVL(OPENING_BALANCE,0))) OPENING, 
            //            DECODE(SIGN(SUM(OPENING_BALANCE)),'-1','CR','DR') OP_TYPE ,
            //abs(SUM(DR_AMOUNT))DR_AMOUNT, abs(SUM(CR_AMOUNT)) CR_AMOUNT, 
            //abs(SUM(CLOSING_BALANCE)) CLOSING_AMOUNT, CL_TYPE
            //FROM V_DAILY_ACC_SUMMARY " +
            //"WHERE ACC_CODE ='"+ id + "'"+
            //" AND VOUCHER_DATE >= TO_DATE ('" + formDate + "', 'YYYY-MM-DD')" +
            //" AND VOUCHER_DATE <= TO_DATE ('" + toDate + "', 'YYYY-MM-DD')" +
            //" AND COMPANY_CODE='01'   AND BRANCH_CODE IN('01.01','01.02')"+
            //" GROUP BY VOUCHER_MITI, VOUCHER_DATE,CL_TYPE"+
            //" ORDER BY VOUCHER_DATE)";

            // Old end Query given by Pramod
            string Query = @"select MiTi as Miti,Voucher_date as VoucherDate,OP_TYPE as OpeningType,(case when op_type='DR'  then opening else 0 end) OpeningDr,
                    (case when op_type='CR'  then opening else 0 end) OpeningCr,(case when CL_TYPE='DR'  then CLOSING_AMOUNT else 0 end) ClosingDr,
                        (case when CL_TYPE='CR'  then CLOSING_AMOUNT else 0 end) ClosingCr ,CL_TYPE as ClosingType,DR_AMOUNT as DrAmount, CR_AMOUNT as CrAmount from (SELECT DISTINCT VOUCHER_MITI MITI, VOUCHER_DATE, 
                        abs( SUM(NVL(OPENING_BALANCE,0))) OPENING, 
                        DECODE(SIGN(SUM(OPENING_BALANCE)),'-1','CR','DR') OP_TYPE ,
            abs(SUM(DR_AMOUNT))DR_AMOUNT, abs(SUM(CR_AMOUNT)) CR_AMOUNT, 
            abs(SUM(CLOSING_BALANCE)) CLOSING_AMOUNT, CL_TYPE
            FROM V_DAILY_ACC_SUMMARY " +
"WHERE ACC_CODE ='" + id + "'" +
" AND VOUCHER_DATE >= TO_DATE ('" + formDate + "', 'YYYY-MM-DD')" +
" AND VOUCHER_DATE <= TO_DATE ('" + toDate + "', 'YYYY-MM-DD')" +
" AND COMPANY_CODE='01'" +
" GROUP BY VOUCHER_MITI, VOUCHER_DATE,CL_TYPE" +
" ORDER BY VOUCHER_DATE)";
            return _objectEntity.SqlQuery<LedgerDailySummaryModel>(Query).ToList();
        }

       public List<LedgerDailySummaryModel> GetSubLedgerDetailBySubCode(string formDate, string toDate, string accountCode, string SubAccCode)
        {
            string Query = @"select MiTi as Miti,Voucher_date as VoucherDate,OP_TYPE as OpeningType,(case when op_type='DR'  then opening else 0 end) OpeningDr,
        (case when op_type='CR'  then opening else 0 end) OpeningCr,(case when CL_TYPE='DR'  then CLOSING_AMOUNT else 0 end) ClosingDr,
            (case when CL_TYPE='CR'  then CLOSING_AMOUNT else 0 end) ClosingCr ,CL_TYPE as ClosingType,DR_AMOUNT as DrAmount, CR_AMOUNT as CrAmount from (SELECT DISTINCT VOUCHER_MITI MITI, VOUCHER_DATE, 
            abs( SUM(NVL(OPENING_BALANCE,0))) OPENING, 
            DECODE(SIGN(SUM(OPENING_BALANCE)),'-1','CR','DR') OP_TYPE ,
abs(SUM(DR_AMOUNT))DR_AMOUNT, abs(SUM(CR_AMOUNT)) CR_AMOUNT, 
abs(SUM(CLOSING_BALANCE)) CLOSING_AMOUNT, CL_TYPE
FROM V_DAILY_SUBACC_SUMMARY"+
" WHERE ACC_CODE ='"+accountCode+"' "+
" AND SUB_CODE='"+SubAccCode+"'"+
" AND VOUCHER_DATE >= TO_DATE ('" + formDate + "', 'YYYY-MM-DD')" +
" AND VOUCHER_DATE <= TO_DATE ('" + toDate + "', 'YYYY-MM-DD')" +
" AND COMPANY_CODE='01'   AND BRANCH_CODE IN('01.01','01.02')" +
" GROUP BY VOUCHER_MITI, VOUCHER_DATE,CL_TYPE" +
" ORDER BY VOUCHER_DATE)";
            return _objectEntity.SqlQuery<LedgerDailySummaryModel>(Query).ToList();
        }

        public List<TrialBalanceViewModel> GETSubLedgerTrialBalance(TrialBalancefilterOption model)
        {
            string Query = "SELECT '" + model.parentId + "' as ACC_CODE," + model.id + " as ParentIdInt,rownum as Id,Sub_code, sub_edesc as account_head,DR_OPENING as DR_OPENING, CR_OPENING as CR_OPENING, dr_amt as DR_AMT, cr_amt as CR_AMT,  NVL (DR_OPENING, 0) + NVL (dr_amt, 0) AS DR_CLOSING,  NVL (CR_OPENING, 0) + NVL (cr_amt, 0) AS CR_CLOSING" +
                            " FROM(SELECT  Sub_code, sub_edesc,CASE WHEN dr_amt_opening - cr_amt_opening >= 0 THEN  dr_amt_opening - cr_amt_opening ELSE 0 END AS DR_OPENING," +
                            " CASE WHEN dr_amt_opening - cr_amt_opening < 0 THEN ABS(dr_amt_opening - cr_amt_opening) ELSE 0 END AS CR_OPENING,  dr_amt,cr_amt FROM( SELECT am.Sub_code, am.SUB_EDESC,  DECODE((SELECT SUM(dr_aMOUNT) FROM v$virtual_sub_ledger" +
                             " where acc_code = '" + model.parentId + "' AND VOUCHER_DATE < TO_DATE('" + model.ReportFilters.FromDate + "', 'YYYY-MM-DD')),  NULL, (SELECT SUM(dr_aMOUNT) FROM v$virtual_sub_ledger" +
                              " WHERE acc_code = '" + model.parentId + "' and sub_code = AM.SUB_CODE  AND VOUCHER_DATE < TO_DATE('" + model.ReportFilters.FromDate + "', 'YYYY-MM-DD'))," +
                              "  (SELECT SUM(dr_aMOUNT)  FROM v$virtual_sub_ledger  where acc_code = '" + model.parentId + "' and sub_code = AM.SUB_CODE AND VOUCHER_DATE < TO_DATE('" + model.ReportFilters.FromDate + "', 'YYYY-MM-DD')))" +
                              " dr_amt_opening, DECODE((SELECT SUM(cr_aMOUNT) FROM v$virtual_sub_ledger  where acc_code = '" + model.parentId + "' and sub_code = AM.SUB_CODE  AND VOUCHER_DATE < TO_DATE('" + model.ReportFilters.FromDate + "', 'YYYY-MM-DD')), NULL, (SELECT SUM(cr_aMOUNT) FROM v$virtual_sub_ledger" +
                               "  WHERE acc_code = '" + model.parentId + "' and sub_code = AM.SUB_CODE   AND VOUCHER_DATE < TO_DATE('" + model.ReportFilters.FromDate + "', 'YYYY-MM-DD'))," +
                               "   (SELECT SUM(cr_aMOUNT) FROM v$virtual_sub_ledger   where acc_code = '" + model.parentId + "' and sub_code = AM.SUB_CODE" +
                                "  AND VOUCHER_DATE < TO_DATE('" + model.ReportFilters.FromDate + "', 'YYYY-MM-DD'))) cr_amt_opening, DECODE((SELECT SUM(dr_aMOUNT) FROM v$virtual_sub_ledger" +
                                 "   where acc_code = '" + model.parentId + "' and sub_code = AM.SUB_CODE  AND VOUCHER_DATE BETWEEN TO_DATE('" + model.ReportFilters.FromDate + "', 'YYYY-MM-DD')  AND TO_DATE('" + model.ReportFilters.ToDate + "', 'YYYY-MM-DD'))," +
                                 "   NULL, (SELECT SUM(dr_aMOUNT) FROM v$virtual_sub_ledger WHERE acc_code = '" + model.parentId + "' and sub_code = AM.SUB_CODE" +
                                  "  AND VOUCHER_DATE BETWEEN TO_DATE ('" + model.ReportFilters.FromDate + "', 'YYYY-MM-DD') AND TO_DATE ('" + model.ReportFilters.ToDate + "', 'YYYY-MM-DD'))," +
                                  "  (SELECT SUM(dr_aMOUNT) FROM v$virtual_sub_ledger where acc_code = '" + model.parentId + "' and sub_code = AM.SUB_CODE" +
                                  "  AND VOUCHER_DATE BETWEEN TO_DATE ('" + model.ReportFilters.FromDate + "', 'YYYY-MM-DD') AND TO_DATE ('" + model.ReportFilters.ToDate + "', 'YYYY-MM-DD'))) " +
                                    " dr_amt, DECODE((SELECT SUM(cr_aMOUNT)  FROM v$virtual_sub_ledger where acc_code = '" + model.parentId + "' and sub_code = AM.SUB_CODE" +
                                  "  AND VOUCHER_DATE BETWEEN TO_DATE('" + model.ReportFilters.FromDate + "', 'YYYY-MM-DD') AND TO_DATE('" + model.ReportFilters.ToDate + "', 'YYYY-MM-DD'))," +
                                  "  NULL,(SELECT SUM(cr_aMOUNT)  FROM v$virtual_sub_ledger WHERE acc_code = '" + model.parentId + "' " +
                                  "  AND VOUCHER_DATE BETWEEN TO_DATE ('" + model.ReportFilters.FromDate + "', 'YYYY-MM-DD')  AND TO_DATE ('" + model.ReportFilters.ToDate + "', 'YYYY-MM-DD'))," +
                                 "   (SELECT SUM(cr_aMOUNT) FROM v$virtual_sub_ledger where acc_code = '" + model.parentId + "' and sub_code = AM.SUB_CODE" +
                                  "  AND VOUCHER_DATE BETWEEN TO_DATE ('" + model.ReportFilters.FromDate + "', 'YYYY-MM-DD') AND TO_DATE ('" + model.ReportFilters.ToDate + "', 'YYYY-MM-DD')))" +
                                  " cr_amt  FROM fa_sub_ledger_setup am inner join FA_sub_ledger_map mp on am.sub_code = MP.SUB_CODE and mp.acc_code = '" + model.parentId + "'" +
                                  " and mp.deleted_flag = 'N' AND mp.company_code = '01'";

            if (model.ReportFilters.BranchFilter.Count > 0)
            {
                Query += string.Format(@" AND mp.BRANCH_CODE IN ('{0}')", string.Join("','", model.ReportFilters.BranchFilter).ToString());
            }

            Query += "))final_data where dr_opening<>0 or  CR_OPENING<>0 or DR_AMT<>0 or  CR_AMT<>0";

            return _objectEntity.SqlQuery<TrialBalanceViewModel>(Query).ToList();


        }

        //public List<TrialBalanceViewModel> GETSubLedgerTrialBalance(string formDate, string toDate, string AccCode,string id)
        //{
        //    string Query = "SELECT '" + AccCode + "' as ACC_CODE," + id + " as ParentIdInt,rownum as Id,Sub_code, sub_edesc as account_head,DR_OPENING as DR_OPENING, CR_OPENING as CR_OPENING, dr_amt as DR_AMT, cr_amt as CR_AMT,  NVL (DR_OPENING, 0) + NVL (dr_amt, 0) AS DR_CLOSING,  NVL (CR_OPENING, 0) + NVL (cr_amt, 0) AS CR_CLOSING" +
        //                    " FROM(SELECT  Sub_code, sub_edesc,CASE WHEN dr_amt_opening - cr_amt_opening >= 0 THEN  dr_amt_opening - cr_amt_opening ELSE 0 END AS DR_OPENING," +
        //                    " CASE WHEN dr_amt_opening - cr_amt_opening < 0 THEN ABS(dr_amt_opening - cr_amt_opening) ELSE 0 END AS CR_OPENING,  dr_amt,cr_amt FROM( SELECT am.Sub_code, am.SUB_EDESC,  DECODE((SELECT SUM(dr_aMOUNT) FROM v$virtual_sub_ledger" +
        //                     " where acc_code = '" + AccCode + "' AND VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD')),  NULL, (SELECT SUM(dr_aMOUNT) FROM v$virtual_sub_ledger" +
        //                      " WHERE acc_code = '" + AccCode + "' and sub_code = AM.SUB_CODE  AND VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD'))," +
        //                      "  (SELECT SUM(dr_aMOUNT)  FROM v$virtual_sub_ledger  where acc_code = '" + AccCode + "' and sub_code = AM.SUB_CODE AND VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD')))" +
        //                      " dr_amt_opening, DECODE((SELECT SUM(cr_aMOUNT) FROM v$virtual_sub_ledger  where acc_code = '" + AccCode + "' and sub_code = AM.SUB_CODE  AND VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD')), NULL, (SELECT SUM(cr_aMOUNT) FROM v$virtual_sub_ledger" +
        //                       "  WHERE acc_code = '" + AccCode + "' and sub_code = AM.SUB_CODE   AND VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD'))," +
        //                       "   (SELECT SUM(cr_aMOUNT) FROM v$virtual_sub_ledger   where acc_code = '" + AccCode + "' and sub_code = AM.SUB_CODE" +
        //                        "  AND VOUCHER_DATE < TO_DATE('" + formDate + "', 'YYYY-MM-DD'))) cr_amt_opening, DECODE((SELECT SUM(dr_aMOUNT) FROM v$virtual_sub_ledger" +
        //                         "   where acc_code = '" + AccCode + "' and sub_code = AM.SUB_CODE  AND VOUCHER_DATE BETWEEN TO_DATE('" + formDate + "', 'YYYY-MM-DD')  AND TO_DATE('" + toDate + "', 'YYYY-MM-DD'))," +
        //                         "   NULL, (SELECT SUM(dr_aMOUNT) FROM v$virtual_sub_ledger WHERE acc_code = '" + AccCode + "' and sub_code = AM.SUB_CODE" +
        //                          "  AND VOUCHER_DATE BETWEEN TO_DATE ('" + formDate + "', 'YYYY-MM-DD') AND TO_DATE ('" + toDate + "', 'YYYY-MM-DD'))," +
        //                          "  (SELECT SUM(dr_aMOUNT) FROM v$virtual_sub_ledger where acc_code = '" + AccCode + "' and sub_code = AM.SUB_CODE" +
        //                          "  AND VOUCHER_DATE BETWEEN TO_DATE ('" + formDate + "', 'YYYY-MM-DD') AND TO_DATE ('" + toDate + "', 'YYYY-MM-DD'))) " +
        //                            " dr_amt, DECODE((SELECT SUM(cr_aMOUNT)  FROM v$virtual_sub_ledger where acc_code = '" + AccCode + "' and sub_code = AM.SUB_CODE" +
        //                          "  AND VOUCHER_DATE BETWEEN TO_DATE('" + formDate + "', 'YYYY-MM-DD') AND TO_DATE('" + toDate + "', 'YYYY-MM-DD'))," +
        //                          "  NULL,(SELECT SUM(cr_aMOUNT)  FROM v$virtual_sub_ledger WHERE acc_code = '" + AccCode + "' " +
        //                          "  AND VOUCHER_DATE BETWEEN TO_DATE ('" + formDate + "', 'YYYY-MM-DD')  AND TO_DATE ('" + toDate + "', 'YYYY-MM-DD'))," +
        //                         "   (SELECT SUM(cr_aMOUNT) FROM v$virtual_sub_ledger where acc_code = '" + AccCode + "' and sub_code = AM.SUB_CODE" +
        //                          "  AND VOUCHER_DATE BETWEEN TO_DATE ('" + formDate + "', 'YYYY-MM-DD') AND TO_DATE ('" + toDate + "', 'YYYY-MM-DD')))" +
        //                          " cr_amt  FROM fa_sub_ledger_setup am inner join FA_sub_ledger_map mp on am.sub_code = MP.SUB_CODE and mp.acc_code = '" + AccCode + "'" +
        //                          " and mp.deleted_flag = 'N' AND mp.company_code = '01'";

        //    if (model.ReportFilters.BranchFilter.Count > 0)
        //    {
        //        Query += string.Format(@" AND mp.BRANCH_CODE IN ('{0}')", string.Join("','", model.ReportFilters.BranchFilter).ToString());
        //    }

        //    Query += "))final_data where dr_opening<>0 or  CR_OPENING<>0 or DR_AMT<>0 or  CR_AMT<>0";

        //    return _objectEntity.SqlQuery<TrialBalanceViewModel>(Query).ToList();


        //}
    }
}
