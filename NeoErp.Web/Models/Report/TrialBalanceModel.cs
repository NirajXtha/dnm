using NeoErp.Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Text;

namespace NeoErp.Models.Report
{
    public class TrialBalanceModel
    {
        public string SNo { get; set; }

        public string VoucherNO { get; set; }

        public string VoucherDate { get; set; }

        public string Particulars { get; set; }

        public string ACC_CODE { get; set; }

        public string ACC_EDESC { get; set; }

        public decimal DR_AMOUNT { get; set; }

        public decimal CR_AMOUNT { get; set; }

        public string DRAMOUNT { get; set; }

        public string CRAMOUNT { get; set; }

        public string GROUP_EDESC { get; set; }

        public string SUB_GROUP_EDESC { get; set; }
        public static IEnumerable<TrialBalanceModel> GetTrialBalanceAll()
        {
            string SelectQuery = @"select  (lpad(' ',2*(level-1)) || acc_edesc) account_head,LEVEL,Pre_Acc_Code,Master_ACC_Code,
                                  decode((select sum(dr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                       where Form_Code <> 0 and  pre_acc_code like (am.master_acc_code||'%')), null,(select sum(dr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                      where Form_Code <> 0 and acc_code = am.acc_code),(select sum(dr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                       where Form_Code <> 0 and  pre_acc_code like (am.master_acc_code||'%')))  dr_amt,
                                  decode((select sum(cr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                      where Form_Code <> 0 and pre_acc_code like (am.master_acc_code||'%')) ,null,(select sum(cr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                      where Form_Code <> 0 and acc_code = am.acc_code),(select sum(cr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                      where Form_Code <> 0 and pre_acc_code like (am.master_acc_code||'%'))) cr_amt
                                  from FA_CHART_OF_ACCOUNTS_SETUP am
                                   where deleted_flag = 'N'
                                   and company_code = '01'
                                  connect by prior master_acc_code =pre_acc_codE";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, SelectQuery))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        yield return new TrialBalanceModel
                        {
                            ACC_CODE = sdr["Master_ACC_Code"].ToString(),
                            ACC_EDESC = sdr["account_head"].ToString(),
                            DR_AMOUNT = string.IsNullOrEmpty(sdr["DR_AMT"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["DR_AMT"].ToString()),
                            CR_AMOUNT = string.IsNullOrEmpty(sdr["CR_AMT"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["CR_AMT"].ToString()),
                        };
                    }
                }
            }
        }
        public static IEnumerable<TrialBalanceModel> GetTrialBalanceSubLedgers(string AccountHead)
        {
            decimal CrAmount = 0;
            decimal DrAmount = 0;
            decimal TotalCr = 0;
            decimal TotalDr = 0;
            int Count = 1;
            string SelectQuery = @"select v.Voucher_NO,to_bs(v.Voucher_Date) as Voucher_Date,Particulars Particulars,Particulars as Niration,v.DR_Amount,v.CR_Amount,'GOPAL' POSTED_BY,'HIRA' CREATED_BY  from V_VIRTUAL_GENERAL_LEDGER1 v inner join FA_CHART_OF_ACCOUNTS_SETUP fc on fc.acc_code=v.acc_code Where v.Voucher_NO!='0' and v.Acc_EDESC='" + AccountHead + "' order by v.Voucher_Date";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, SelectQuery))
            {
                if (sdr.HasRows)
                {
                    yield return new TrialBalanceModel
                    {
                        Count = Count,
                        VoucherNO = "",
                        VoucherDate = "01/01/2071",
                        Particulars = "Opening Balance",
                        DR_AMOUNT = 0,
                        CR_AMOUNT = 0
                    };
                    while (sdr.Read())
                    {
                        Count = Count + 1;
                        CrAmount = string.IsNullOrEmpty(sdr["CR_Amount"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["CR_Amount"].ToString());
                        DrAmount = string.IsNullOrEmpty(sdr["DR_Amount"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["DR_Amount"].ToString());
                        TotalCr = TotalCr + CrAmount;
                        TotalDr = TotalDr + DrAmount;
                        string NewParticular = "";
                        NewParticular = sdr["Particulars"].ToString(); // NewParticular + sdr["Particulars"].ToString().Substring(i, Length);
                        yield return new TrialBalanceModel
                        {
                            Count = Count,
                            VoucherNO = sdr["Voucher_NO"].ToString(),
                            VoucherDate = sdr["Voucher_Date"].ToString(),
                            Particulars = "" + NewParticular.ToLower() + "",
                            DR_AMOUNT = DrAmount,
                            CR_AMOUNT = CrAmount
                        };
                    }
                    Count = Count + 1;
                    yield return new TrialBalanceModel
                    {
                        Count = Count,
                        VoucherNO = "",
                        VoucherDate = "31/03/2072",
                        Particulars = "Closing Balance",
                        DR_AMOUNT = decimal.Parse(TotalDr.ToString()),
                        CR_AMOUNT = decimal.Parse(TotalCr.ToString())
                    };
                }
            }
        }
        public static IEnumerable<TrialBalanceModel> GetTrialBalanceGroupBy()
        {
            string SelectQuery = @"select  (lpad(' ',2*(level-1)) || acc_edesc) account_head,LEVEL,Pre_Acc_Code,Master_ACC_Code,
                                  decode((select sum(dr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                       where Form_Code <> 0 and  pre_acc_code like (am.master_acc_code||'%')), null,(select sum(dr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                      where Form_Code <> 0 and acc_code = am.acc_code),(select sum(dr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                       where Form_Code <> 0 and  pre_acc_code like (am.master_acc_code||'%')))  dr_amt,
                                  decode((select sum(cr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                      where Form_Code <> 0 and pre_acc_code like (am.master_acc_code||'%')) ,null,(select sum(cr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                      where Form_Code <> 0 and acc_code = am.acc_code),(select sum(cr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                      where Form_Code <> 0 and pre_acc_code like (am.master_acc_code||'%'))) cr_amt
                                  from FA_CHART_OF_ACCOUNTS_SETUP am
                                   where deleted_flag = 'N'
                                   and company_code = '01'
                                     and Pre_Acc_Code='00'
                                  connect by prior master_acc_code =pre_acc_codE";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, SelectQuery))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        yield return new TrialBalanceModel
                        {
                            ACC_CODE = sdr["Master_ACC_Code"].ToString(),
                            ACC_EDESC = sdr["account_head"].ToString(),
                            DR_AMOUNT = string.IsNullOrEmpty(sdr["DR_AMT"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["DR_AMT"].ToString()),
                            CR_AMOUNT = string.IsNullOrEmpty(sdr["CR_AMT"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["CR_AMT"].ToString()),
                        };
                    }
                }
            }
        }
        public static IEnumerable<TrialBalanceModel> GetTrialBalanceSubGroupBy(string AccountCode)
        {
            decimal DrAmount = 0;
            decimal CrAmount = 0;
            decimal TotalDrAmount = 0;
            decimal TotalCrAmount = 0;
            string SelectQuery = @"select  (lpad(' ',2*(level-1)) || acc_edesc) account_head,LEVEL,Pre_Acc_Code,Master_ACC_Code,
                                  decode((select sum(dr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                       where Form_Code <> 0 and  pre_acc_code like (am.master_acc_code||'%')), null,(select sum(dr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                      where Form_Code <> 0 and acc_code = am.acc_code),(select sum(dr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                       where Form_Code <> 0 and  pre_acc_code like (am.master_acc_code||'%')))  dr_amt,
                                  decode((select sum(cr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                      where Form_Code <> 0 and pre_acc_code like (am.master_acc_code||'%')) ,null,(select sum(cr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                      where Form_Code <> 0 and acc_code = am.acc_code),(select sum(cr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                      where Form_Code <> 0 and pre_acc_code like (am.master_acc_code||'%'))) cr_amt
                                  from FA_CHART_OF_ACCOUNTS_SETUP am
                                   where deleted_flag = 'N'
                                   and company_code = '01'
                                     and Pre_Acc_Code='" + AccountCode + @"' and Level=2
                                  connect by prior master_acc_code =pre_acc_code";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, SelectQuery))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        CrAmount = string.IsNullOrEmpty(sdr["CR_AMT"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["CR_AMT"].ToString());
                        DrAmount = string.IsNullOrEmpty(sdr["DR_AMT"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["DR_AMT"].ToString());
                        TotalCrAmount = TotalCrAmount + CrAmount;
                        TotalDrAmount = TotalDrAmount + DrAmount;
                        yield return new TrialBalanceModel
                        {
                            ACC_CODE = sdr["Master_ACC_Code"].ToString(),
                            ACC_EDESC = sdr["account_head"].ToString(),
                            DR_AMOUNT = DrAmount,
                            CR_AMOUNT = CrAmount
                        };
                    }
                    //yield return new TrialBalanceModel
                    //{
                    //    ACC_CODE = "",
                    //    ACC_EDESC = "Total",
                    //    DR_AMOUNT = TotalDrAmount,
                    //    CR_AMOUNT = TotalCrAmount
                    //};
                }
            }
        }
        public static IEnumerable<TrialBalanceModel> GetTrialBalanceLedger(string AccountCode)
        {
            string SelectQuery = @"select  (lpad(' ',2*(level-1)) || acc_edesc) account_head,LEVEL,Pre_Acc_Code,Master_ACC_Code,
                                    decode((select sum(dr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                         where Form_Code <> 0 and  pre_acc_code like (am.master_acc_code||'%')), null,(select sum(dr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                        where Form_Code <> 0 and acc_code = am.acc_code),(select sum(dr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                         where Form_Code <> 0 and  pre_acc_code like (am.master_acc_code||'%')))  dr_amt,
                                    decode((select sum(cr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                        where Form_Code <> 0 and pre_acc_code like (am.master_acc_code||'%')) ,null,(select sum(cr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                        where Form_Code <> 0 and acc_code = am.acc_code),(select sum(cr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                        where Form_Code <> 0 and pre_acc_code like (am.master_acc_code||'%'))) cr_amt
                                    from FA_CHART_OF_ACCOUNTS_SETUP am
                                     where deleted_flag = 'N'
                                     and company_code = '01'
                                       and Pre_Acc_Code='" + AccountCode + @"' and Level=3
                                    connect by prior master_acc_code =pre_acc_code";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, SelectQuery))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        yield return new TrialBalanceModel
                        {
                            ACC_CODE = sdr["Master_ACC_Code"].ToString(),
                            ACC_EDESC = sdr["account_head"].ToString(),
                            DR_AMOUNT = string.IsNullOrEmpty(sdr["DR_AMT"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["DR_AMT"].ToString()),
                            CR_AMOUNT = string.IsNullOrEmpty(sdr["CR_AMT"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["CR_AMT"].ToString())
                        };
                    }
                }
            }
        }
        public static IEnumerable<TrialBalanceModel> GetTrialBalanceDetails(string AccountCode)
        {
            decimal DrAmount = 0;
            decimal CrAmount = 0;
            decimal TotalDrAmount = 0;
            decimal TotalCrAmount = 0;
            string SelectQuery = @"select  (lpad(' ',2*(level-1)) || acc_edesc) account_head,LEVEL,Pre_Acc_Code,Master_ACC_Code,
                                  decode((select sum(dr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                       where Form_Code <> 0 and  pre_acc_code like (am.master_acc_code||'%')), null,(select sum(dr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                      where Form_Code <> 0 and acc_code = am.acc_code),(select sum(dr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                       where Form_Code <> 0 and  pre_acc_code like (am.master_acc_code||'%')))  dr_amt,
                                  decode((select sum(cr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                      where Form_Code <> 0 and pre_acc_code like (am.master_acc_code||'%')) ,null,(select sum(cr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                      where Form_Code <> 0 and acc_code = am.acc_code),(select sum(cr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                      where Form_Code <> 0 and pre_acc_code like (am.master_acc_code||'%'))) cr_amt
                                  from FA_CHART_OF_ACCOUNTS_SETUP am
                                   where deleted_flag = 'N'
                                   and company_code = '01'
                                     and Pre_Acc_Code='" + AccountCode + @"' and Level=4
                                  connect by prior master_acc_code =pre_acc_code";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, SelectQuery))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        CrAmount = string.IsNullOrEmpty(sdr["CR_AMT"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["CR_AMT"].ToString());
                        DrAmount = string.IsNullOrEmpty(sdr["DR_AMT"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["DR_AMT"].ToString());
                        TotalCrAmount = TotalCrAmount + CrAmount;
                        TotalDrAmount = TotalDrAmount + DrAmount;
                        yield return new TrialBalanceModel
                        {
                            ACC_CODE = sdr["Master_ACC_Code"].ToString(),
                            ACC_EDESC = sdr["account_head"].ToString(),
                            DR_AMOUNT = DrAmount,
                            CR_AMOUNT = CrAmount
                        };
                    }
                    //yield return new TrialBalanceModel
                    //{
                    //    ACC_CODE = "",
                    //    ACC_EDESC = "Total",
                    //    DR_AMOUNT = TotalDrAmount,
                    //    CR_AMOUNT = TotalCrAmount
                    //};
                }
            }
        }
        public static IEnumerable<TrialBalanceModel> GetTrialBalanceVoucher(string VoucherNo)
        {
            decimal DrAmount = 0;
            decimal CrAmount = 0;
            decimal TotalDrAmount = 0;
            decimal TotalCrAmount = 0;
            string SelectQuery = @"select  (lpad(' ',2*(level-1)) || acc_edesc) account_head,LEVEL,Pre_Acc_Code,Master_ACC_Code,
                                  decode((select sum(dr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                       where Form_Code <> 0 and  pre_acc_code like (am.master_acc_code||'%')), null,(select sum(dr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                      where Form_Code <> 0 and acc_code = am.acc_code),(select sum(dr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                       where Form_Code <> 0 and  pre_acc_code like (am.master_acc_code||'%')))  dr_amt,
                                  decode((select sum(cr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                      where Form_Code <> 0 and pre_acc_code like (am.master_acc_code||'%')) ,null,(select sum(cr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                      where Form_Code <> 0 and acc_code = am.acc_code),(select sum(cr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                      where Form_Code <> 0 and pre_acc_code like (am.master_acc_code||'%'))) cr_amt
                                  from FA_CHART_OF_ACCOUNTS_SETUP am
                                   where deleted_flag = 'N'
                                   and company_code = '01'
                                     and Pre_Acc_Code='" + VoucherNo + @"' and Level=5
                                  connect by prior master_acc_code =pre_acc_codE";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, SelectQuery))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        CrAmount = string.IsNullOrEmpty(sdr["CR_AMT"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["CR_AMT"].ToString());
                        DrAmount = string.IsNullOrEmpty(sdr["DR_AMT"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["DR_AMT"].ToString());
                        TotalCrAmount = TotalCrAmount + CrAmount;
                        TotalDrAmount = TotalDrAmount + DrAmount;
                        yield return new TrialBalanceModel
                        {
                            ACC_CODE = sdr["Master_ACC_Code"].ToString(),
                            ACC_EDESC = sdr["account_head"].ToString(),
                            DR_AMOUNT = DrAmount,
                            CR_AMOUNT = CrAmount
                        };
                    }
                    //yield return new TrialBalanceModel
                    //{
                    //    ACC_CODE = "",
                    //    ACC_EDESC = "Total",
                    //    DR_AMOUNT = TotalDrAmount,
                    //    CR_AMOUNT = TotalCrAmount
                    //};
                }
            }
        }
        public static IEnumerable<TrialBalanceModel> GetGroupByTrailBalanceList()
        {
            string SelectQuery = @"SELECT DISTINCT 
                                FN_FETCH_GROUP_ACC_DESC(a.COMPANY_CODE,'FA_CHART_OF_ACCOUNTS_SETUP', a.PRE_ACC_CODE) GROUP_EDESC,
                                FN_FETCH_PRE_DESC(a.COMPANY_CODE,'FA_CHART_OF_ACCOUNTS_SETUP', a.PRE_ACC_CODE) SUB_GROUP_EDESC,
                                A.ACC_CODE, INITCAP(A.ACC_EDESC) AS ACC_EDESC,
                                NVL(SUM(NVL(B.DR_AMOUNT,0) * NVL(B.EXCHANGE_RATE,1)),0) DR_AMOUNT,
                                NVL(SUM(NVL(B.CR_AMOUNT,0) * NVL(B.EXCHANGE_RATE,1)),0) CR_AMOUNT,
                                NVL(SUM(NVL(B.DR_AMOUNT,0) * NVL(B.EXCHANGE_RATE,1)),0) 
                                - NVL(SUM(NVL(B.CR_AMOUNT,0) * NVL(B.EXCHANGE_RATE,1)),0) CLS_AMT 
                                FROM FA_CHART_OF_ACCOUNTS_SETUP A, V$VIRTUAL_GENERAL_LEDGER B
                                WHERE A.ACC_CODE = B.ACC_CODE 
                                AND A.COMPANY_CODE = B.COMPANY_CODE 
                                AND B.VOUCHER_DATE >='15-MAR-2014'
                                AND B.VOUCHER_DATE <='31-MAR-2015' 
                                AND B.COMPANY_CODE IN('01') 
                                AND B.BRANCH_CODE IN('01.01','01.02') 
                                AND B.POSTED_BY IS NOT NULL 
                                HAVING (NVL(SUM(B.DR_AMOUNT),0) <> 0 OR NVL(SUM(B.CR_AMOUNT),0) <> 0) 
                                GROUP BY A.ACC_CODE, INITCAP(A.ACC_EDESC),a.COMPANY_CODE,a.PRE_ACC_CODE 
                                ORDER BY FN_FETCH_GROUP_ACC_DESC(a.COMPANY_CODE,'FA_CHART_OF_ACCOUNTS_SETUP', a.PRE_ACC_CODE),
                                FN_FETCH_PRE_DESC(a.COMPANY_CODE,'FA_CHART_OF_ACCOUNTS_SETUP', a.PRE_ACC_CODE),
                                INITCAP(A.ACC_EDESC)";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, SelectQuery))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        yield return new TrialBalanceModel
                        {
                            ACC_CODE = sdr["ACC_CODE"].ToString(),
                            GROUP_EDESC = sdr["GROUP_EDESC"].ToString(),
                            SUB_GROUP_EDESC = sdr["SUB_GROUP_EDESC"].ToString(),
                            ACC_EDESC = sdr["ACC_EDESC"].ToString(),
                            DR_AMOUNT = Math.Abs(decimal.Parse(sdr["DR_AMOUNT"].ToString())),
                            CR_AMOUNT = Math.Abs(decimal.Parse(sdr["CR_AMOUNT"].ToString()))
                        };
                    }
                }
            }
        }
        public static IEnumerable<TrialBalanceModel> GetTreeTrailBalanceList()
        {
            string SelectQuery = @"SELECT DISTINCT 
                                FN_FETCH_GROUP_ACC_DESC(a.COMPANY_CODE,'FA_CHART_OF_ACCOUNTS_SETUP', a.PRE_ACC_CODE) GROUP_EDESC,
                                FN_FETCH_PRE_DESC(a.COMPANY_CODE,'FA_CHART_OF_ACCOUNTS_SETUP', a.PRE_ACC_CODE) SUB_GROUP_EDESC,
                                A.ACC_CODE, INITCAP(A.ACC_EDESC) AS ACC_EDESC,
                                NVL(SUM(NVL(B.DR_AMOUNT,0) * NVL(B.EXCHANGE_RATE,1)),0) DR_AMOUNT,
                                NVL(SUM(NVL(B.CR_AMOUNT,0) * NVL(B.EXCHANGE_RATE,1)),0) CR_AMOUNT,
                                NVL(SUM(NVL(B.DR_AMOUNT,0) * NVL(B.EXCHANGE_RATE,1)),0) 
                                - NVL(SUM(NVL(B.CR_AMOUNT,0) * NVL(B.EXCHANGE_RATE,1)),0) CLS_AMT 
                                FROM FA_CHART_OF_ACCOUNTS_SETUP A, V$VIRTUAL_GENERAL_LEDGER B
                                WHERE A.ACC_CODE = B.ACC_CODE 
                                AND A.COMPANY_CODE = B.COMPANY_CODE 
                                AND B.VOUCHER_DATE >='15-MAR-2014'
                                AND B.VOUCHER_DATE <='31-MAR-2015' 
                                AND B.COMPANY_CODE IN('01') 
                                AND B.BRANCH_CODE IN('01.01','01.02') 
                                AND B.POSTED_BY IS NOT NULL 
                                HAVING (NVL(SUM(B.DR_AMOUNT),0) <> 0 OR NVL(SUM(B.CR_AMOUNT),0) <> 0) 
                                GROUP BY A.ACC_CODE, INITCAP(A.ACC_EDESC),a.COMPANY_CODE,a.PRE_ACC_CODE 
                                ORDER BY FN_FETCH_GROUP_ACC_DESC(a.COMPANY_CODE,'FA_CHART_OF_ACCOUNTS_SETUP', a.PRE_ACC_CODE),
                                FN_FETCH_PRE_DESC(a.COMPANY_CODE,'FA_CHART_OF_ACCOUNTS_SETUP', a.PRE_ACC_CODE),
                                INITCAP(A.ACC_EDESC)";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, SelectQuery))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        yield return new TrialBalanceModel
                        {
                            ACC_CODE = sdr["ACC_CODE"].ToString(),
                            GROUP_EDESC = sdr["GROUP_EDESC"].ToString(),
                            SUB_GROUP_EDESC = sdr["SUB_GROUP_EDESC"].ToString(),
                            ACC_EDESC = sdr["ACC_EDESC"].ToString(),
                            DR_AMOUNT = Math.Abs(decimal.Parse(sdr["DR_AMOUNT"].ToString())),
                            CR_AMOUNT = Math.Abs(decimal.Parse(sdr["CR_AMOUNT"].ToString()))
                        };
                    }
                }
            }
        }
        public static string GetTrialBalanceTreeReport(DateTime fromDate)
        {
            StringBuilder html = new StringBuilder();
            string GroupQuery = @"select  (lpad(' ',2*(level-1)) || acc_edesc) account_head,LEVEL,Pre_Acc_Code,Master_ACC_Code,
                                  decode((select sum(dr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                       where Form_Code <> 0 and  pre_acc_code like (am.master_acc_code||'%')), null,(select sum(dr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                      where Form_Code <> 0 and acc_code = am.acc_code),(select sum(dr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                       where Form_Code <> 0 and  pre_acc_code like (am.master_acc_code||'%')))  dr_amt,
                                  decode((select sum(cr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                      where Form_Code <> 0 and pre_acc_code like (am.master_acc_code||'%')) ,null,(select sum(cr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                      where Form_Code <> 0 and acc_code = am.acc_code),(select sum(cr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                      where Form_Code <> 0 and pre_acc_code like (am.master_acc_code||'%'))) cr_amt
                                  from FA_CHART_OF_ACCOUNTS_SETUP am
                                   where deleted_flag = 'N'
                                   and company_code = '01'
                                     and Pre_Acc_Code='00'
                                  connect by prior master_acc_code =pre_acc_code";
            DataSet ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, GroupQuery);
            DataTable tblGroup = ds.Tables[0];
            html.Append("<table width='100%' border='1' id='tablebalancesheet' borderColor='#e0e0e0' cellpadding='0' cellspacing='0' style='border-collapse:collapse'><tr class='HeaderAccount' style='line-height:1'><td rowspan='2' style='padding-left: 15px; vertical-align:middle; text-align: left; background:#b7b5b5; color:white; width:1150px; font-weight:bold; font-family:Arial;'>Account Head</td><td colspan='2' style='padding: 7px 10px; vertical-align:middle; text-align: center; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial; min-width:220px;'>Opening Balance</td><td colspan='2' style='padding: 7px 10px; vertical-align:middle; text-align: center; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial; min-width:220px;'>Transaction</td><td colspan='2' style='padding: 7px 10px; vertical-align:middle; text-align: center; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial; min-width:220px;'>Closing Balance</td></tr><tr class='SubHeaderAccount' style='line-height:1'><td style='padding: 7px 10px; vertical-align:middle; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial;'>Dr Amount</td><td style='padding: 7px 10px; vertical-align:middle; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial;'>Cr Amount</td><td style='padding: 7px 10px; vertical-align:middle; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial;'>Dr Amount</td><td style='padding: 7px 10px; vertical-align:middle; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial;'>Cr Amount</td><td style='padding: 7px 10px; vertical-align:middle; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial;'>Dr Amount</td><td style='padding: 7px 10px; vertical-align:middle; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial;'>Cr Amount</td></tr>");
            decimal TotalDr = 0;
            decimal TotalCr = 0;
            foreach (DataRow row in tblGroup.Rows)
            {
                html.Append("<tr class='GroupAccount' style='background-color:#f7f7f7;'>");
                html.Append("<td onclick='Test(" + row["Master_ACC_Code"].ToString() + ")' id='' style='font-weight:bold;padding-left:10px; border:1;float:none;' ><a href='javascript:void(0)' class='fa fa-plus' style='color: #333 !important; padding-right:5px;'></a><span style='cursor: pointer;'>" + row["account_head"].ToString() + "</span></td>");
                html.Append("<td style='font-weight:bold;text-align:right;border:1;float:none;'>" + string.Format("{0:c}", 0).Replace("$", "") + "</td>");
                html.Append("<td style='font-weight:bold;text-align:right;border:1;float:none;'>" + string.Format("{0:c}", 0).Replace("$", "") + "</td>");
                html.Append("<td style='font-weight:bold;text-align:right;border:1;float:none;'>" + string.Format("{0:c}", string.IsNullOrEmpty(row["DR_AMT"].ToString()) ? decimal.Parse("0") : decimal.Parse(row["DR_AMT"].ToString())).Replace("$", "") + "</td>");
                html.Append("<td style='font-weight:bold;text-align:right;border:1;float:none;'>" + string.Format("{0:c}", string.IsNullOrEmpty(row["CR_AMT"].ToString()) ? decimal.Parse("0") : decimal.Parse(row["CR_AMT"].ToString())).Replace("$", "") + "</td>");
                html.Append("<td style='font-weight:bold;text-align:right;border:1;float:none;'>" + string.Format("{0:c}", string.IsNullOrEmpty(row["DR_AMT"].ToString()) ? decimal.Parse("0") : decimal.Parse(row["DR_AMT"].ToString())).Replace("$", "") + "</td>");
                html.Append("<td style='font-weight:bold;text-align:right;border:1;float:none;'>" + string.Format("{0:c}", string.IsNullOrEmpty(row["CR_AMT"].ToString()) ? decimal.Parse("0") : decimal.Parse(row["CR_AMT"].ToString())).Replace("$", "") + "</td>");
                html.Append("</tr>");
                html.Append(GetSubGroup(row["Master_ACC_Code"].ToString()));
                TotalDr = TotalDr + (string.IsNullOrEmpty(row["DR_AMT"].ToString()) ? decimal.Parse("0") : decimal.Parse(row["DR_AMT"].ToString()));
                TotalCr = TotalCr + (string.IsNullOrEmpty(row["CR_AMT"].ToString()) ? decimal.Parse("0") : decimal.Parse(row["CR_AMT"].ToString()));
            }
            if (tblGroup.Rows.Count != 0)
            {
                html.Append("<tr class='GroupAccount' style='background-color:#f7f7f7;'>");
                html.Append("<td style='font-weight:bold;padding-right:15px; border:1;float:none; text-align:right' ><span style='cursor: pointer;'>Grand Total</span></td>");
                html.Append("<td style='font-weight:bold;text-align:right;border:1;float:none;'>" + string.Format("{0:c}", 0).Replace("$", "") + "</td>");
                html.Append("<td style='font-weight:bold;text-align:right;border:1;float:none;'>" + string.Format("{0:c}", 0).Replace("$", "") + "</td>");
                html.Append("<td style='font-weight:bold;text-align:right;border:1;float:none;'>" + string.Format("{0:c}", decimal.Parse(TotalDr.ToString())).Replace("$", "") + "</td>");
                html.Append("<td style='font-weight:bold;text-align:right;border:1;float:none;'>" + string.Format("{0:c}", decimal.Parse(TotalCr.ToString())).Replace("$", "") + "</td>");
                html.Append("<td style='font-weight:bold;text-align:right;border:1;float:none;'>" + string.Format("{0:c}", decimal.Parse(TotalDr.ToString())).Replace("$", "") + "</td>");
                html.Append("<td style='font-weight:bold;text-align:right;border:1;float:none;'>" + string.Format("{0:c}", decimal.Parse(TotalCr.ToString())).Replace("$", "") + "</td>");
                html.Append("</tr>");
            }
            html.Append("</table>");
            string ss = html.ToString();
            ds.Dispose();
            return html.ToString();
        }
        public static string GetSubGroup(string AccountHead)
        {
            string SubGroupQuery = @"select  (lpad(' ',2*(level-1)) || acc_edesc) account_head,LEVEL,Pre_Acc_Code,Master_ACC_Code,
                                      decode((select sum(dr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                           where Form_Code <> 0 and  pre_acc_code like (am.master_acc_code||'%')), null,(select sum(dr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                          where Form_Code <> 0 and acc_code = am.acc_code),(select sum(dr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                           where Form_Code <> 0 and  pre_acc_code like (am.master_acc_code||'%')))  dr_amt,
                                      decode((select sum(cr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                          where Form_Code <> 0 and pre_acc_code like (am.master_acc_code||'%')) ,null,(select sum(cr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                          where Form_Code <> 0 and acc_code = am.acc_code),(select sum(cr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                          where Form_Code <> 0 and pre_acc_code like (am.master_acc_code||'%'))) cr_amt
                                      from FA_CHART_OF_ACCOUNTS_SETUP am
                                       where deleted_flag = 'N'
                                       and company_code = '01'
                                         and Pre_Acc_Code='" + AccountHead + @"' and Level=2
                                      connect by prior master_acc_code =pre_acc_code";
            StringBuilder strHtmlSubGroup = new StringBuilder();
            DataSet ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, SubGroupQuery);
            DataTable tblSubGroup = ds.Tables[0];
            foreach (DataRow row in tblSubGroup.Rows)
            {
                strHtmlSubGroup.Append("<tr class='SubGroupAccount'>");
                strHtmlSubGroup.Append("<td  style='font-weight:bold;padding-left:20px;'><a href='javascript:void(0)' class='fa fa-plus' style='color: #333 !important; padding-right:5px;'></a><span style='cursor: pointer;'>" + row["account_head"] + "</span></td>");
                strHtmlSubGroup.Append("<td style='font-weight:bold;text-align:right;border:1;float:none;'>" + string.Format("{0:c}", 0).Replace("$", "") + "</td>");
                strHtmlSubGroup.Append("<td style='font-weight:bold;text-align:right;border:1;float:none;'>" + string.Format("{0:c}", 0).Replace("$", "") + "</td>");
                strHtmlSubGroup.Append("<td style='font-weight:bold;text-align:right;border:1;float:none;'>" + string.Format("{0:c}", string.IsNullOrEmpty(row["DR_AMT"].ToString()) ? decimal.Parse("0") : decimal.Parse(row["DR_AMT"].ToString())).Replace("$", "") + "</td>");
                strHtmlSubGroup.Append("<td style='font-weight:bold;text-align:right;border:1;float:none;'>" + string.Format("{0:c}", string.IsNullOrEmpty(row["CR_AMT"].ToString()) ? decimal.Parse("0") : decimal.Parse(row["CR_AMT"].ToString())).Replace("$", "") + "</td>");
                strHtmlSubGroup.Append("<td style='font-weight:bold;text-align:right;border:1;float:none;'>" + string.Format("{0:c}", string.IsNullOrEmpty(row["DR_AMT"].ToString()) ? decimal.Parse("0") : decimal.Parse(row["DR_AMT"].ToString())).Replace("$", "") + "</td>");
                strHtmlSubGroup.Append("<td style='font-weight:bold;text-align:right;border:1;float:none;'>" + string.Format("{0:c}", string.IsNullOrEmpty(row["CR_AMT"].ToString()) ? decimal.Parse("0") : decimal.Parse(row["CR_AMT"].ToString())).Replace("$", "") + "</td>");
                strHtmlSubGroup.Append("</tr>");
                strHtmlSubGroup.Append(GetLedger(row["Master_ACC_Code"].ToString()));
            }
            return strHtmlSubGroup.ToString();
        }
        public static string GetLedger(string AccountHead)
        {
            string LedgerQuery = @"select  (lpad(' ',2*(level-1)) || acc_edesc) account_head,LEVEL,Pre_Acc_Code,Master_ACC_Code,
                                  decode((select sum(dr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                       where Form_Code <> 0 and  pre_acc_code like (am.master_acc_code||'%')), null,(select sum(dr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                      where Form_Code <> 0 and acc_code = am.acc_code),(select sum(dr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                       where Form_Code <> 0 and  pre_acc_code like (am.master_acc_code||'%')))  dr_amt,
                                  decode((select sum(cr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                      where Form_Code <> 0 and pre_acc_code like (am.master_acc_code||'%')) ,null,(select sum(cr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                      where Form_Code <> 0 and acc_code = am.acc_code),(select sum(cr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                      where Form_Code <> 0 and pre_acc_code like (am.master_acc_code||'%'))) cr_amt
                                  from FA_CHART_OF_ACCOUNTS_SETUP am
                                   where deleted_flag = 'N'
                                   and company_code = '01'
                                     and Pre_Acc_Code='" + AccountHead + @"' and Level=3
                                  connect by prior master_acc_code =pre_acc_code";
            StringBuilder strHtmlLedger = new StringBuilder();
            DataSet ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, LedgerQuery);
            DataTable tblLedger = ds.Tables[0];
            foreach (DataRow row in tblLedger.Rows)
            {
                strHtmlLedger.Append("<tr class='LedgerAccount'>");
                strHtmlLedger.Append("<td  style='padding-left:40px;'><a href='javascript:void(0)' class='fa fa-plus' style='color: #333 !important; padding-right:5px;'></a><span style='cursor: pointer;'>" + row["account_head"] + "</td>");
                strHtmlLedger.Append("<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", 0).Replace("$", "") + "</td>");
                strHtmlLedger.Append("<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", 0).Replace("$", "") + "</td>");
                strHtmlLedger.Append("<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", string.IsNullOrEmpty(row["DR_AMT"].ToString()) ? decimal.Parse("0") : decimal.Parse(row["DR_AMT"].ToString())).Replace("$", "") + "</td>");
                strHtmlLedger.Append("<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", string.IsNullOrEmpty(row["CR_AMT"].ToString()) ? decimal.Parse("0") : decimal.Parse(row["CR_AMT"].ToString())).Replace("$", "") + "</td>");
                strHtmlLedger.Append("<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", string.IsNullOrEmpty(row["DR_AMT"].ToString()) ? decimal.Parse("0") : decimal.Parse(row["DR_AMT"].ToString())).Replace("$", "") + "</td>");
                strHtmlLedger.Append("<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", string.IsNullOrEmpty(row["CR_AMT"].ToString()) ? decimal.Parse("0") : decimal.Parse(row["CR_AMT"].ToString())).Replace("$", "") + "</td>");
                strHtmlLedger.Append("</tr>");
                strHtmlLedger.Append(GetLedgerDetails(row["Master_ACC_Code"].ToString()));
            }
            return strHtmlLedger.ToString();
        }
        public static string GetLedgerDetails(string AccountHead)
        {
            string LedgerDetailsQuery = @"select  (lpad(' ',2*(level-1)) || acc_edesc) account_head,LEVEL,Pre_Acc_Code,Master_ACC_Code,
                                          decode((select sum(dr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                               where Form_Code <> 0 and  pre_acc_code like (am.master_acc_code||'%')), null,(select sum(dr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                              where Form_Code <> 0 and acc_code = am.acc_code),(select sum(dr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                               where Form_Code <> 0 and  pre_acc_code like (am.master_acc_code||'%')))  dr_amt,
                                          decode((select sum(cr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                              where Form_Code <> 0 and pre_acc_code like (am.master_acc_code||'%')) ,null,(select sum(cr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                              where Form_Code <> 0 and acc_code = am.acc_code),(select sum(cr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                              where Form_Code <> 0 and pre_acc_code like (am.master_acc_code||'%'))) cr_amt
                                          from FA_CHART_OF_ACCOUNTS_SETUP am
                                           where deleted_flag = 'N'
                                           and company_code = '01'
                                             and Pre_Acc_Code='" + AccountHead + @"' and Level=4
                                          connect by prior master_acc_code =pre_acc_code";
            StringBuilder strHtmlLedger = new StringBuilder();
            DataSet ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, LedgerDetailsQuery);
            DataTable tblLedgerDetails = ds.Tables[0];
            foreach (DataRow row in tblLedgerDetails.Rows)
            {
                strHtmlLedger.Append("<tr class='LedgerDetailsAccount'>");
                strHtmlLedger.Append("<td  style='padding-left:70px;'><span style='cursor: pointer;'>" + row["account_head"] + "</span></td>");
                strHtmlLedger.Append("<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", 0).Replace("$", "") + "</td>");
                strHtmlLedger.Append("<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", 0).Replace("$", "") + "</td>");
                strHtmlLedger.Append("<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", string.IsNullOrEmpty(row["DR_AMT"].ToString()) ? decimal.Parse("0") : decimal.Parse(row["DR_AMT"].ToString())).Replace("$", "") + "</td>");
                strHtmlLedger.Append("<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", string.IsNullOrEmpty(row["CR_AMT"].ToString()) ? decimal.Parse("0") : decimal.Parse(row["CR_AMT"].ToString())).Replace("$", "") + "</td>");
                strHtmlLedger.Append("<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", string.IsNullOrEmpty(row["DR_AMT"].ToString()) ? decimal.Parse("0") : decimal.Parse(row["DR_AMT"].ToString())).Replace("$", "") + "</td>");
                strHtmlLedger.Append("<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", string.IsNullOrEmpty(row["CR_AMT"].ToString()) ? decimal.Parse("0") : decimal.Parse(row["CR_AMT"].ToString())).Replace("$", "") + "</td>");
                strHtmlLedger.Append("</tr>");
            }
            return strHtmlLedger.ToString();
        }
        public static IEnumerable<TrialBalanceModel> GetExportToExcelData(string AccountHead)
        {
            string SelectQuery = @"select  (lpad(' ',2*(level-1)) || acc_edesc) account_head,LEVEL,Pre_Acc_Code,Master_ACC_Code,
                                          decode((select sum(dr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                               where Form_Code <> 0 and  pre_acc_code like (am.master_acc_code||'%')), null,(select sum(dr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                              where Form_Code <> 0 and acc_code = am.acc_code),(select sum(dr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                               where Form_Code <> 0 and  pre_acc_code like (am.master_acc_code||'%')))  dr_amt,
                                          decode((select sum(cr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                              where Form_Code <> 0 and pre_acc_code like (am.master_acc_code||'%')) ,null,(select sum(cr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                              where Form_Code <> 0 and acc_code = am.acc_code),(select sum(cr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                              where Form_Code <> 0 and pre_acc_code like (am.master_acc_code||'%'))) cr_amt
                                          from FA_CHART_OF_ACCOUNTS_SETUP am
                                           where deleted_flag = 'N'
                                           and company_code = '01'
                                             and Pre_Acc_Code='" + AccountHead + "'connect by prior master_acc_code =pre_acc_code";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, SelectQuery))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        yield return new TrialBalanceModel
                        {
                            AccountHead = sdr["ACCOUNT_HEAD"].ToString(),
                            CRAMOUNT = sdr["CR_AMT"].ToString(),
                            DRAMOUNT = sdr["DR_AMT"].ToString()
                        };
                    }
                }
            }
        }
        public static string VoucherNo { get; set; }
        public static OracleDataReader DRAmount { get; set; }
        public static OracleDataReader CRAmmount { get; set; }
        public string AccountHead { get; set; }
        public string POSTED_BY { get; set; }
        public string CREATED_BY { get; set; }
        public string Niration { get; set; }
        public int Count { get; set; }
        public string Sub_Code { get; set; }
        public string Sub_EDesc { get; set; }
        public static string GetTreeData(string FromDate, string ToDate, string Company, string Branch)
        {
            StringBuilder Html = new StringBuilder();
            var accountLlist = new List<TrialBalanceModeltest>();
            string Result = GetParrentAccount("00", "0", FromDate, ToDate, Company, Branch);
            Html.Append(Result);
            return Html.ToString();
        }

      
        public static string GetLastLedgerList(string AccountCode, string AccountName, string FromDate, string ToDate, string Company, string Branch)
        {
            string subAccountCode = "";
            string LastAccount = "";
            string Query = "select NVL((select NVL(sum(NVL(DR_AMOUNT,0))-sum(NVL(CR_AMOUNT,0)),0) as TAmount from FA_GENERAL_LEDGER Where Acc_Code='" + AccountCode + "' and company_code = '" + Company + "' and Branch_Code='" + Branch + "' and FORM_CODE=0),0) + NVL((select NVL(sum(NVL(DR_AMOUNT,0))-sum(NVL(CR_AMOUNT,0)),0) as TAmount from FA_GENERAL_LEDGER Where Acc_Code='" + AccountCode + "' and company_code = '" + Company + "' and Branch_Code='" + Branch + "' and To_Date(Voucher_Date) < TO_DATE('" + FromDate + "', 'YYYY-MM-DD') and FORM_CODE!=0 ),0) as OAmount, (select sum(NVL(DR_AMOUNT,0)) DrAmount from FA_GENERAL_LEDGER Where Acc_Code='" + AccountCode + "' and company_code = '" + Company + "' and Branch_Code='" + Branch + "' and To_Date(Voucher_Date) between TO_DATE('" + FromDate + "', 'YYYY-MM-DD') and TO_DATE('" + ToDate + "', 'YYYY-MM-DD') and FORM_CODE!=0) as DrAmount,(select sum(NVL(CR_AMOUNT,0)) as CRmount from FA_GENERAL_LEDGER Where Acc_Code='" + AccountCode + "' and company_code = '" + Company + "' and Branch_Code='" + Branch + "' and To_Date(Voucher_Date) between TO_DATE('" + FromDate + "', 'YYYY-MM-DD') and TO_DATE('" + ToDate + "', 'YYYY-MM-DD') and FORM_CODE!=0) as CrAmount from dual";
            DataSet ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, Query);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                decimal CrAmountO = 0; decimal DrAmountO = 0; decimal CrAmountT = 0; decimal DrAmountT = 0; decimal ClosingDr = 0; decimal ClosingCR = 0;
                decimal OBalance = string.IsNullOrEmpty(row["OAmount"].ToString()) ? decimal.Parse("0") : decimal.Parse(row["OAmount"].ToString());
                CrAmountT = string.IsNullOrEmpty(row["CrAmount"].ToString()) ? decimal.Parse("0") : decimal.Parse(row["CrAmount"].ToString());
                DrAmountT = string.IsNullOrEmpty(row["DrAmount"].ToString()) ? decimal.Parse("0") : decimal.Parse(row["DrAmount"].ToString());

                if (OBalance > 0) { DrAmountO = Math.Abs(OBalance); CrAmountO = 0; }
                else { CrAmountO = Math.Abs(OBalance); DrAmountO = 0; }
                if (((DrAmountO + DrAmountT) - (CrAmountO + CrAmountT)) < 0)
                { ClosingCR = Math.Abs(((DrAmountO + DrAmountT) - (CrAmountO + CrAmountT))); ClosingDr = 0; }
                else { ClosingDr = Math.Abs(((DrAmountO + DrAmountT) - (CrAmountO + CrAmountT))); ClosingCR = 0; }
                string sql = "select count(*) from Fa_Sub_Ledger_Map where Acc_code='" + AccountCode + "'";
                DataSet ds1 = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, sql);
                if (int.Parse(ds1.Tables[0].Rows[0][0].ToString()) != 0)
                {
                    LastAccount = LastAccount + "<tr class='LedgerDetailsAccount sledger'>";
                    LastAccount = LastAccount + "<td  style='padding-left:60px;'><span style='cursor: pointer;' id='" + AccountCode + "'>" + AccountName + " [...]" + "</span></td>";
                    LastAccount = LastAccount + "<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", DrAmountO).Replace("$", "") + "</td>";
                    LastAccount = LastAccount + "<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", CrAmountO).Replace("$", "") + "</td>";
                    LastAccount = LastAccount + "<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", DrAmountT).Replace("$", "") + "</td>";
                    LastAccount = LastAccount + "<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", CrAmountT).Replace("$", "") + "</td>";
                    LastAccount = LastAccount + "<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", (ClosingDr)).Replace("$", "") + "</td>";
                    LastAccount = LastAccount + "<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", (ClosingCR)).Replace("$", "") + "</td>";
                    LastAccount = LastAccount + "</tr>";
                }
                else
                {
                    LastAccount = LastAccount + "<tr class='LedgerDetailsAccount ledger'>";
                    LastAccount = LastAccount + "<td  style='padding-left:60px;'><span style='cursor: pointer;' id='" + AccountCode + "'>" + AccountName + "</span></td>";
                    LastAccount = LastAccount + "<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", DrAmountO).Replace("$", "") + "</td>";
                    LastAccount = LastAccount + "<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", CrAmountO).Replace("$", "") + "</td>";
                    LastAccount = LastAccount + "<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", DrAmountT).Replace("$", "") + "</td>";
                    LastAccount = LastAccount + "<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", CrAmountT).Replace("$", "") + "</td>";
                    LastAccount = LastAccount + "<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", (ClosingDr)).Replace("$", "") + "</td>";
                    LastAccount = LastAccount + "<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", (ClosingCR)).Replace("$", "") + "</td>";
                    LastAccount = LastAccount + "</tr>";
                }
            }
            return LastAccount;
        }
        public static string GetParrentAccount(string MASTER_ACC_CODE, string Level, string FromDate, string ToDate, string Company, string Branch)
        {
            string MainAccount = "";
            string ParrentAccount = "Select ACC_CODE, Initcap(ACC_EDESC) ACC_EDESC, MASTER_ACC_CODE, PRE_ACC_CODE from  FA_CHART_OF_ACCOUNTS_SETUP where PRE_ACC_CODE='" + MASTER_ACC_CODE + "' and  deleted_flag = 'N' and company_code = '" + Company + "' and Branch_Code='" + Branch + "' order by ACC_EDESC";
            DataSet ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, ParrentAccount);
            if (Level == "0")
            {
                MainAccount = MainAccount + "<table width='100%' border='1' id='tablebalancesheet' borderColor='#e0e0e0' cellpadding='0' cellspacing='0' style='border-collapse:collapse'><tr class='HeaderAccount' style='line-height:1'><td rowspan='2' style='padding-left: 15px; vertical-align:middle; text-align: left; background:#b7b5b5; color:white; width:1150px; font-weight:bold; font-family:Arial;'>Account Head</td><td colspan='2' style='padding: 7px 10px; vertical-align:middle; text-align: center; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial; min-width:220px;'>Opening Balance</td><td colspan='2' style='padding: 7px 10px; vertical-align:middle; text-align: center; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial; min-width:220px;'>Transaction</td><td colspan='2' style='padding: 7px 10px; vertical-align:middle; text-align: center; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial; min-width:220px;'>Closing Balance</td></tr><tr class='SubHeaderAccount' style='line-height:1'><td style='padding: 7px 10px; vertical-align:middle; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial;'>Dr Amount</td><td style='padding: 7px 10px; vertical-align:middle; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial;'>Cr Amount</td><td style='padding: 7px 10px; vertical-align:middle; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial;'>Dr Amount</td><td style='padding: 7px 10px; vertical-align:middle; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial;'>Cr Amount</td><td style='padding: 7px 10px; vertical-align:middle; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial;'>Dr Amount</td><td style='padding: 7px 10px; vertical-align:middle; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial;'>Cr Amount</td></tr>";
            }
            Level = (int.Parse(Level) + 1).ToString();
            foreach (DataRow ro in ds.Tables[0].Rows)
            {
                if (Level == "1")
                {
                    MainAccount = MainAccount + "<tr class='GroupAccount' style='background-color:#f7f7f7;'>";
                    MainAccount = MainAccount + "<td style='font-weight:bold;padding-left:10px;'><a href='javascript:void(0)' class='fa fa-plus' style='color: #333 !important; padding-right:5px;'></a><span style='cursor: pointer;'>" + ro["ACC_EDESC"].ToString() + "</span></td>";
                }
                if (Level == "2")
                {
                    MainAccount = MainAccount + "<tr class='SubGroupAccount'>";
                    MainAccount = MainAccount + "<td style='font-weight:bold;padding-left:20px;'><a href='javascript:void(0)' class='fa fa-plus' style='color: #333 !important; padding-right:5px;'></a><span style='cursor: pointer;'>" + ro["ACC_EDESC"].ToString() + "</span></td>";
                }
                if (Level == "3")
                {
                    MainAccount = MainAccount + "<tr class='LedgerAccount'>";
                    MainAccount = MainAccount + "<td style='font-weight:bold; padding-left:40px;'><a href='javascript:void(0)' class='fa fa-plus' style='color: #333 !important; padding-right:5px;'></a><span style='cursor: pointer;'>" + ro["ACC_EDESC"].ToString() + "</span></td>";
                }
                if (int.Parse(Level) < 4)
                {
                    MainAccount = MainAccount + "<td style='font-weight:bold; text-align:right;border:1;float:none;'>" + string.Format("{0:c}", 0).Replace("$", "") + "</td>";
                    MainAccount = MainAccount + "<td style='font-weight:bold; text-align:right;border:1;float:none;'>" + string.Format("{0:c}", 0).Replace("$", "") + "</td>";
                    MainAccount = MainAccount + "<td style='font-weight:bold; text-align:right;border:1;float:none;'>" + string.Format("{0:c}", 0).Replace("$", "") + "</td>";
                    MainAccount = MainAccount + "<td style='font-weight:bold; text-align:right;border:1;float:none;'>" + string.Format("{0:c}", 0).Replace("$", "") + "</td>";
                    MainAccount = MainAccount + "<td style='font-weight:bold; text-align:right;border:1;float:none;'>" + string.Format("{0:c}", 0).Replace("$", "") + "</td>";
                    MainAccount = MainAccount + "<td style='font-weight:bold; text-align:right;border:1;float:none;'>" + string.Format("{0:c}", 0).Replace("$", "") + "</td>";
                    MainAccount = MainAccount + "</tr>";
                }
                string SubAccount = GetParrentAccount(ro["MASTER_ACC_CODE"].ToString(), Level, FromDate, ToDate, Company, Branch);
                if (SubAccount != "")
                {
                    MainAccount = MainAccount + SubAccount;
                }
                else
                {
                    MainAccount = MainAccount + GetLastLedger(ro["ACC_CODE"].ToString(), ro["ACC_EDESC"].ToString(), FromDate, ToDate, Company, Branch);
                }
            }
            if (Level == "1")
            {
                MainAccount = MainAccount + "<tr class='GroupAccounts' style='background-color:#f7f7f7;'>";
                MainAccount = MainAccount + "<td style='font-weight:bold;padding-right:15px; border:1;float:none; text-align:right' ><span style='cursor: pointer;'>Grand Total</span></td>";
                MainAccount = MainAccount + "<td style='font-weight:bold;text-align:right;border:1;float:none;'>" + string.Format("{0:c}", 0).Replace("$", "") + "</td>";
                MainAccount = MainAccount + "<td style='font-weight:bold;text-align:right;border:1;float:none;'>" + string.Format("{0:c}", 0).Replace("$", "") + "</td>";
                MainAccount = MainAccount + "<td style='font-weight:bold;text-align:right;border:1;float:none;'>" + string.Format("{0:c}", 0).Replace("$", "") + "</td>";
                MainAccount = MainAccount + "<td style='font-weight:bold;text-align:right;border:1;float:none;'>" + string.Format("{0:c}", 0).Replace("$", "") + "</td>";
                MainAccount = MainAccount + "<td style='font-weight:bold;text-align:right;border:1;float:none;'>" + string.Format("{0:c}", 0).Replace("$", "") + "</td>";
                MainAccount = MainAccount + "<td style='font-weight:bold;text-align:right;border:1;float:none;'>" + string.Format("{0:c}", 0).Replace("$", "") + "</td>";
                MainAccount = MainAccount + "</tr>";
                MainAccount = MainAccount + "</table>";
            }
            return MainAccount;
        }
        public static string GetLastLedger(string AccountCode, string AccountName, string FromDate, string ToDate, string Company, string Branch)
        {
            string subAccountCode = "";
            string LastAccount = "";
            string Query = "select NVL((select NVL(sum(NVL(DR_AMOUNT,0))-sum(NVL(CR_AMOUNT,0)),0) as TAmount from FA_GENERAL_LEDGER Where Acc_Code='" + AccountCode + "' and company_code = '" + Company + "' and Branch_Code='" + Branch + "' and FORM_CODE=0),0) + NVL((select NVL(sum(NVL(DR_AMOUNT,0))-sum(NVL(CR_AMOUNT,0)),0) as TAmount from FA_GENERAL_LEDGER Where Acc_Code='" + AccountCode + "' and company_code = '" + Company + "' and Branch_Code='" + Branch + "' and To_Date(Voucher_Date) < TO_DATE('" + FromDate + "', 'YYYY-MM-DD') and FORM_CODE!=0 ),0) as OAmount, (select sum(NVL(DR_AMOUNT,0)) DrAmount from FA_GENERAL_LEDGER Where Acc_Code='" + AccountCode + "' and company_code = '" + Company + "' and Branch_Code='" + Branch + "' and To_Date(Voucher_Date) between TO_DATE('" + FromDate + "', 'YYYY-MM-DD') and TO_DATE('" + ToDate + "', 'YYYY-MM-DD') and FORM_CODE!=0) as DrAmount,(select sum(NVL(CR_AMOUNT,0)) as CRmount from FA_GENERAL_LEDGER Where Acc_Code='" + AccountCode + "' and company_code = '" + Company + "' and Branch_Code='" + Branch + "' and To_Date(Voucher_Date) between TO_DATE('" + FromDate + "', 'YYYY-MM-DD') and TO_DATE('" + ToDate + "', 'YYYY-MM-DD') and FORM_CODE!=0) as CrAmount from dual";
            DataSet ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, Query);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                decimal CrAmountO = 0; decimal DrAmountO = 0; decimal CrAmountT = 0; decimal DrAmountT = 0; decimal ClosingDr = 0; decimal ClosingCR = 0;
                decimal OBalance = string.IsNullOrEmpty(row["OAmount"].ToString()) ? decimal.Parse("0") : decimal.Parse(row["OAmount"].ToString());
                CrAmountT = string.IsNullOrEmpty(row["CrAmount"].ToString()) ? decimal.Parse("0") : decimal.Parse(row["CrAmount"].ToString());
                DrAmountT = string.IsNullOrEmpty(row["DrAmount"].ToString()) ? decimal.Parse("0") : decimal.Parse(row["DrAmount"].ToString());

                if (OBalance > 0) { DrAmountO = Math.Abs(OBalance); CrAmountO = 0; }
                else { CrAmountO = Math.Abs(OBalance); DrAmountO = 0; }
                if (((DrAmountO + DrAmountT) - (CrAmountO + CrAmountT)) < 0)
                { ClosingCR = Math.Abs(((DrAmountO + DrAmountT) - (CrAmountO + CrAmountT))); ClosingDr = 0; }
                else { ClosingDr = Math.Abs(((DrAmountO + DrAmountT) - (CrAmountO + CrAmountT))); ClosingCR = 0; }
                string sql = "select count(*) from Fa_Sub_Ledger_Map where Acc_code='" + AccountCode + "'";
                DataSet ds1 = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, sql);
                if (int.Parse(ds1.Tables[0].Rows[0][0].ToString()) != 0)
                {
                    LastAccount = LastAccount + "<tr class='LedgerDetailsAccount sledger'>";
                    LastAccount = LastAccount + "<td  style='padding-left:60px;'><span style='cursor: pointer;' id='" + AccountCode + "'>" + AccountName + " [...]" + "</span></td>";
                    LastAccount = LastAccount + "<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", DrAmountO).Replace("$", "") + "</td>";
                    LastAccount = LastAccount + "<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", CrAmountO).Replace("$", "") + "</td>";
                    LastAccount = LastAccount + "<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", DrAmountT).Replace("$", "") + "</td>";
                    LastAccount = LastAccount + "<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", CrAmountT).Replace("$", "") + "</td>";
                    LastAccount = LastAccount + "<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", (ClosingDr)).Replace("$", "") + "</td>";
                    LastAccount = LastAccount + "<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", (ClosingCR)).Replace("$", "") + "</td>";
                    LastAccount = LastAccount + "</tr>";
                }
                else
                {
                    LastAccount = LastAccount + "<tr class='LedgerDetailsAccount ledger'>";
                    LastAccount = LastAccount + "<td  style='padding-left:60px;'><span style='cursor: pointer;' id='" + AccountCode + "'>" + AccountName + "</span></td>";
                    LastAccount = LastAccount + "<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", DrAmountO).Replace("$", "") + "</td>";
                    LastAccount = LastAccount + "<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", CrAmountO).Replace("$", "") + "</td>";
                    LastAccount = LastAccount + "<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", DrAmountT).Replace("$", "") + "</td>";
                    LastAccount = LastAccount + "<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", CrAmountT).Replace("$", "") + "</td>";
                    LastAccount = LastAccount + "<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", (ClosingDr)).Replace("$", "") + "</td>";
                    LastAccount = LastAccount + "<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", (ClosingCR)).Replace("$", "") + "</td>";
                    LastAccount = LastAccount + "</tr>";
                }
            }
            return LastAccount;
        }
        public static IEnumerable<TrialBalanceModel> GetTrialBalanceDateWise(string AccountCode, string FromDate, string ToDate, string Company, string Branch)
        {
            decimal CrAmount = 0;
            decimal DrAmount = 0;
            decimal TotalCr = 0;
            decimal TotalDr = 0;
            decimal Amount = 0;
            int Count = 0;
            string ToRange = "";
            string SelectQueryOpening = @"select to_bs(TO_DATE('" + FromDate + "', 'YYYY-MM-DD')) FromRange,to_bs(TO_DATE('" + ToDate + "', 'YYYY-MM-DD')) ToRange,nvl((select sum(NVL(Dr_Amount,0)) -Sum(NVL(Cr_Amount,0)) Amount from FA_GENERAL_LEDGER where Acc_Code='" + AccountCode + "' and FORM_CODE=0 and company_code = '" + Company + "' and Branch_Code='" + Branch + "'),0)+nvl((select sum(NVL(Dr_Amount,0))-Sum(NVL(Cr_Amount,0)) Amount from FA_GENERAL_LEDGER where Acc_Code='" + AccountCode + "' and FORM_CODE!=0 and company_code = '" + Company + "' and Branch_Code='" + Branch + "' and To_Date(Voucher_Date) < TO_DATE('" + FromDate + "', 'YYYY-MM-DD')),0) Amount from dual";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, SelectQueryOpening))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        Count = Count + 1;
                        Amount = string.IsNullOrEmpty(sdr["Amount"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["Amount"].ToString());
                        if (Amount < 0)
                        {
                            CrAmount = Math.Abs(Amount);
                            DrAmount = 0;
                        }
                        else
                        {
                            DrAmount = Math.Abs(Amount);
                            CrAmount = 0;
                        }
                        TotalCr = TotalCr + CrAmount;
                        TotalDr = TotalDr + DrAmount;
                        ToRange = sdr["ToRange"].ToString();
                        yield return new TrialBalanceModel
                        {
                            Count = Count,
                            VoucherNO = "",
                            VoucherDate = sdr["FromRange"].ToString(),
                            Particulars = "<b>Opening Balance</b>",
                            DR_AMOUNT = DrAmount,
                            CR_AMOUNT = CrAmount
                        };
                    }
                }
            }
            string SelectQuery = @"select Voucher_No,to_bs(Voucher_Date) Voucher_Date,Acc_Code,Initcap(Particulars) Particulars,sum(NVL(Dr_Amount,0)) Dr_Amount,Sum(NVL(Cr_Amount,0)) Cr_Amount from FA_GENERAL_LEDGER where VOUCHER_NO!='0' and Acc_Code='" + AccountCode + "' and FORM_CODE!=0 and company_code = '" + Company + "' and Branch_Code='" + Branch + "' and FORM_CODE!=0 and To_Date(Voucher_Date) between TO_DATE('" + FromDate + "', 'YYYY-MM-DD') and TO_DATE('" + ToDate + "', 'YYYY-MM-DD') group by Voucher_No,Voucher_Date,Acc_Code,Particulars order by Voucher_Date";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, SelectQuery))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        Count = Count + 1;
                        CrAmount = string.IsNullOrEmpty(sdr["CR_Amount"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["CR_Amount"].ToString());
                        DrAmount = string.IsNullOrEmpty(sdr["DR_Amount"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["DR_Amount"].ToString());
                        TotalCr = TotalCr + CrAmount;
                        TotalDr = TotalDr + DrAmount;
                        string NewParticular = "";
                        int Length = sdr["Particulars"].ToString().Length;
                        int i = 0;
                        while (Length > 74)
                        {
                            NewParticular = NewParticular + sdr["Particulars"].ToString().Substring(i, 73) + "<br />";
                            Length = Length - 73;
                            i = i + 73;
                        }
                        NewParticular = NewParticular + sdr["Particulars"].ToString().Substring(i, Length);
                        yield return new TrialBalanceModel
                        {
                            Count = Count,
                            VoucherNO = sdr["Voucher_NO"].ToString(),
                            VoucherDate = sdr["Voucher_Date"].ToString(),
                            Particulars = "<span style='text-align:justify; text-transform: lowercase !important;'>" + NewParticular.ToLower() + "</span>",
                            DR_AMOUNT = DrAmount,
                            CR_AMOUNT = CrAmount
                        };
                    }
                }
            }
            Count = Count + 1;
            if ((TotalDr - TotalCr) < 0)
            {
                TotalCr = Math.Abs(TotalDr - TotalCr);
                TotalDr = 0;
            }
            else
            {
                TotalDr = Math.Abs(TotalDr - TotalCr);
                TotalCr = 0;
            }
            yield return new TrialBalanceModel
            {
                Count = Count,
                VoucherNO = "",
                VoucherDate = ToRange,
                Particulars = "<b>Closing Balance</b>",
                DR_AMOUNT = decimal.Parse(TotalDr.ToString()),
                CR_AMOUNT = decimal.Parse(TotalCr.ToString())
            };
        }


        public static IEnumerable<TrialBalanceModel> GetTrialBalanceDateWisePref(string AccountCode, string FromDate, string ToDate, string Company, string Branch, string figureAmount, string roundupAmount)
        {
            decimal CrAmount = 0;
            decimal DrAmount = 0;
            decimal TotalCr = 0;
            decimal TotalDr = 0;
            decimal Amount = 0;
            int Count = 0;
            string[] strround = roundupAmount.Split('.');
            int round = strround[1].Length;
            int fA = Convert.ToInt32(figureAmount);
            string ToRange = "";
            string SelectQueryOpening = @"select to_bs(TO_DATE('" + FromDate + "', 'YYYY-MM-DD')) FromRange,to_bs(TO_DATE('" + ToDate + "', 'YYYY-MM-DD')) ToRange,nvl((select sum(NVL(Dr_Amount,0)) -Sum(NVL(Cr_Amount,0)) Amount from FA_GENERAL_LEDGER where Acc_Code='" + AccountCode + "' and FORM_CODE=0 and company_code = '" + Company + "' and Branch_Code='" + Branch + "'),0)+nvl((select sum(NVL(Dr_Amount,0))-Sum(NVL(Cr_Amount,0)) Amount from FA_GENERAL_LEDGER where Acc_Code='" + AccountCode + "' and FORM_CODE!=0 and company_code = '" + Company + "' and Branch_Code='" + Branch + "' and To_Date(Voucher_Date) < TO_DATE('" + FromDate + "', 'YYYY-MM-DD')),0) Amount from dual";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, SelectQueryOpening))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        Count = Count + 1;
                        Amount = string.IsNullOrEmpty(sdr["Amount"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["Amount"].ToString());
                        if (Amount < 0)
                        {
                            CrAmount = Math.Abs(Math.Round(Amount / fA, round));
                            DrAmount = 0;
                        }
                        else
                        {
                            DrAmount = Math.Round(Amount / fA, round);
                            CrAmount = 0;
                        }
                        TotalCr = TotalCr + CrAmount;
                        TotalDr = TotalDr + DrAmount;
                        ToRange = sdr["ToRange"].ToString();
                        yield return new TrialBalanceModel
                        {
                            Count = Count,
                            VoucherNO = "",
                            VoucherDate = sdr["FromRange"].ToString(),
                            Particulars = "<b>Opening Balance</b>",
                            DR_AMOUNT = DrAmount,
                            CR_AMOUNT = CrAmount
                        };
                    }
                }
            }
            string SelectQuery = @"select Voucher_No,to_bs(Voucher_Date) Voucher_Date,Acc_Code,Initcap(Particulars) Particulars,sum(NVL(Dr_Amount,0)) Dr_Amount,Sum(NVL(Cr_Amount,0)) Cr_Amount from FA_GENERAL_LEDGER where VOUCHER_NO!='0' and Acc_Code='" + AccountCode + "' and FORM_CODE!=0 and company_code = '" + Company + "' and Branch_Code='" + Branch + "' and FORM_CODE!=0 and To_Date(Voucher_Date) between TO_DATE('" + FromDate + "', 'YYYY-MM-DD') and TO_DATE('" + ToDate + "', 'YYYY-MM-DD') and rownum<=500 group by Voucher_No,Voucher_Date,Acc_Code,Particulars order by Voucher_Date";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, SelectQuery))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        Count = Count + 1;
                        CrAmount = string.IsNullOrEmpty(sdr["CR_Amount"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["CR_Amount"].ToString());
                        DrAmount = string.IsNullOrEmpty(sdr["DR_Amount"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["DR_Amount"].ToString());
                        decimal cr = Math.Round(CrAmount / fA, round);
                        decimal dr = Math.Round(DrAmount / fA, round);
                        TotalCr = TotalCr + cr;
                        TotalDr = TotalDr + dr;
                        string NewParticular = "";
                        int Length = sdr["Particulars"].ToString().Length;
                        int i = 0;
                        while (Length > 74)
                        {
                            NewParticular = NewParticular + sdr["Particulars"].ToString().Substring(i, 73) + "<br />";
                            Length = Length - 73;
                            i = i + 73;
                        }
                        NewParticular = NewParticular + sdr["Particulars"].ToString().Substring(i, Length);
                        yield return new TrialBalanceModel
                        {
                            Count = Count,
                            VoucherNO = sdr["Voucher_NO"].ToString(),
                            VoucherDate = sdr["Voucher_Date"].ToString(),
                            Particulars = "<span style='text-align:justify; text-transform: lowercase !important;'>" + NewParticular.ToLower() + "</span>",
                            DR_AMOUNT = dr,
                            CR_AMOUNT = cr
                        };
                    }
                }
            }
            Count = Count + 1;
            if ((TotalDr - TotalCr) < 0)
            {
                TotalCr = Math.Abs(Math.Round((TotalDr - TotalCr), round));
                TotalDr = 0;
            }
            else
            {
                TotalDr = Math.Round((TotalDr - TotalCr), round);
                TotalCr = 0;
            }
            yield return new TrialBalanceModel
            {
                Count = Count,
                VoucherNO = "",
                VoucherDate = ToRange,
                Particulars = "<b>Closing Balance</b>",
                DR_AMOUNT = decimal.Parse(TotalDr.ToString()),
                CR_AMOUNT = decimal.Parse(TotalCr.ToString())
            };
        }


        public static IEnumerable<TrialBalanceModel> GetTrialBalanceDateWiseDetails(string Voucher_NO, string Company, string Branch)
        {
            decimal CrAmount = 0;
            decimal DrAmount = 0;
            decimal TotalDr = 0;
            decimal TotalCr = 0;
            string SelectQuery = @"select FC.ACC_CODE,Voucher_NO,Voucher_Date,Initcap(FC.ACC_EDESC) ACC_EDESC,PARTICULARS,DR_AMOUNT,CR_AMOUNT, FA_GENERAL_LEDGER.CREATED_BY,'Hira' POSTED_BY from FA_GENERAL_LEDGER inner join FA_CHART_OF_ACCOUNTS_SETUP FC on FC.ACC_CODE = FA_GENERAL_LEDGER.ACC_CODE where FA_GENERAL_LEDGER.VOUCHER_NO='" + Voucher_NO + "' and FA_GENERAL_LEDGER.company_code = '" + Company + "' and FA_GENERAL_LEDGER.Branch_Code='" + Branch + "' order by SERIAL_NO";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, SelectQuery))
            {
                if (sdr.HasRows)
                {
                    int j = 0;
                    yield return new TrialBalanceModel
                    {
                        SNo = "<b>SNo.</b>",
                        VoucherNO = "0",
                        VoucherDate = "2014-01-01",
                        Particulars = "<b>Particulars</b>",
                        Niration = "",
                        DRAMOUNT = "<b>Debit</b>",
                        CRAMOUNT = "<b>Credit</b>",
                        POSTED_BY = "AA",
                        CREATED_BY = "BB"
                    };
                    while (sdr.Read())
                    {
                        j = j + 1;
                        CrAmount = string.IsNullOrEmpty(sdr["CR_Amount"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["CR_Amount"].ToString());
                        DrAmount = string.IsNullOrEmpty(sdr["DR_Amount"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["DR_Amount"].ToString());

                        TotalCr = TotalCr + CrAmount;
                        TotalDr = TotalDr + DrAmount;

                        string PARTICULARS = "";
                        string pp = sdr["PARTICULARS"].ToString();
                        int Length = sdr["PARTICULARS"].ToString().Length;
                        int i = 0;
                        while (Length > 114)
                        {
                            PARTICULARS = PARTICULARS + sdr["PARTICULARS"].ToString().Substring(i, 113) + "<br />";
                            Length = Length - 113;
                            i = i + 113;
                        }
                        PARTICULARS = PARTICULARS + sdr["PARTICULARS"].ToString().Substring(i, Length);
                        yield return new TrialBalanceModel
                        {
                            SNo = j.ToString() + ".",
                            VoucherNO = sdr["Voucher_NO"].ToString(),
                            VoucherDate = sdr["Voucher_Date"].ToString(),
                            Particulars = sdr["ACC_EDESC"].ToString(),
                            Niration = "<span style='text-align:justify; text-transform: capitalize !important;'>" + PARTICULARS.ToLower() + "</span>",
                            DRAMOUNT = DrAmount.ToString("0.00"),
                            CRAMOUNT = CrAmount.ToString("0.00"),
                            POSTED_BY = sdr["POSTED_BY"].ToString(),
                            CREATED_BY = sdr["CREATED_BY"].ToString()
                        };
                    }
                    yield return new TrialBalanceModel
                    {
                        SNo = "",
                        VoucherNO = "ZZZZZZZ99999",
                        VoucherDate = "2014-01-01",
                        Particulars = "<b>Total</b>",
                        Niration = "",
                        DRAMOUNT = "<b>" + TotalDr.ToString("0.00") + "</b>",
                        CRAMOUNT = "<b>" + TotalCr.ToString("0.00") + "</b>",
                        POSTED_BY = "AA",
                        CREATED_BY = "BB"
                    };
                }
            }
        }
        public static IEnumerable<TrialBalanceModel> GetTrialBalanceDateWiseDetailsPref(string Voucher_NO, string Company, string Branch, string figureAmount, string roundupAmount)
        {
            decimal CrAmount = 0;
            decimal DrAmount = 0;
            decimal TotalDr = 0;
            decimal TotalCr = 0;
            string[] strround = roundupAmount.Split('.');
            int round = strround[1].Length;
            int fA = Convert.ToInt32(figureAmount);
            string SelectQuery = @"select FC.ACC_CODE,Voucher_NO,Voucher_Date,Initcap(FC.ACC_EDESC) ACC_EDESC,PARTICULARS,DR_AMOUNT,CR_AMOUNT, FA_GENERAL_LEDGER.CREATED_BY,'Hira' POSTED_BY from FA_GENERAL_LEDGER inner join FA_CHART_OF_ACCOUNTS_SETUP FC on FC.ACC_CODE = FA_GENERAL_LEDGER.ACC_CODE where FA_GENERAL_LEDGER.VOUCHER_NO='" + Voucher_NO + "' and FA_GENERAL_LEDGER.company_code = '" + Company + "' and FA_GENERAL_LEDGER.Branch_Code='" + Branch + "' order by SERIAL_NO";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, SelectQuery))
            {
                if (sdr.HasRows)
                {
                    int j = 0;
                    yield return new TrialBalanceModel
                    {
                        SNo = "<b>SNo.</b>",
                        VoucherNO = "0",
                        VoucherDate = "2014-01-01",
                        Particulars = "<b>Particulars</b>",
                        Niration = "",
                        DRAMOUNT = "<b>Debit</b>",
                        CRAMOUNT = "<b>Credit</b>",
                        POSTED_BY = "AA",
                        CREATED_BY = "BB"
                    };
                    while (sdr.Read())
                    {
                        j = j + 1;
                        CrAmount = Math.Round((string.IsNullOrEmpty(sdr["CR_Amount"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["CR_Amount"].ToString())) / fA, round);
                        DrAmount = Math.Round((string.IsNullOrEmpty(sdr["DR_Amount"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["DR_Amount"].ToString())) / fA, round);

                        TotalCr = TotalCr + CrAmount;
                        TotalDr = TotalDr + DrAmount;

                        string PARTICULARS = "";
                        string pp = sdr["PARTICULARS"].ToString();
                        int Length = sdr["PARTICULARS"].ToString().Length;
                        int i = 0;
                        while (Length > 114)
                        {
                            PARTICULARS = PARTICULARS + sdr["PARTICULARS"].ToString().Substring(i, 113) + "<br />";
                            Length = Length - 113;
                            i = i + 113;
                        }
                        PARTICULARS = PARTICULARS + sdr["PARTICULARS"].ToString().Substring(i, Length);
                        yield return new TrialBalanceModel
                        {
                            SNo = j.ToString() + ".",
                            VoucherNO = sdr["Voucher_NO"].ToString(),
                            VoucherDate = sdr["Voucher_Date"].ToString(),
                            Particulars = sdr["ACC_EDESC"].ToString(),
                            Niration = "<span style='text-transform: capitalize !important;'>" + PARTICULARS.ToLower() + "</span>",
                            DRAMOUNT = DrAmount.ToString("0.00"),
                            CRAMOUNT = CrAmount.ToString("0.00"),
                            POSTED_BY = sdr["POSTED_BY"].ToString(),
                            CREATED_BY = sdr["CREATED_BY"].ToString()
                        };
                    }
                    yield return new TrialBalanceModel
                    {
                        SNo = "",
                        VoucherNO = "ZZZZZZZ99999",
                        VoucherDate = "2014-01-01",
                        Particulars = "<b>Total</b>",
                        Niration = "",
                        DRAMOUNT = "<b>" + TotalDr.ToString("0.00") + "</b>",
                        CRAMOUNT = "<b>" + TotalCr.ToString("0.00") + "</b>",
                        POSTED_BY = "AA",
                        CREATED_BY = "BB"
                    };
                }
            }
        }
        public static IEnumerable<TrialBalanceModel> getSubLedger()
        {
            string sql = "select  acc_edesc,acc_code from FA_CHART_OF_ACCOUNTS_SETUP am where Acc_Code in (select distinct Acc_Code from Fa_Sub_Ledger_Map) and deleted_flag = 'N' and company_code = '01' and Level=4 connect by prior master_acc_code =pre_acc_code order by acc_edesc";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, sql))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        yield return new TrialBalanceModel
                        {
                            ACC_CODE = sdr["acc_code"].ToString(),
                            ACC_EDESC = sdr["acc_edesc"].ToString()

                        };
                    }
                }
            }
        }
        public static IEnumerable<TrialBalanceModel> getLedger()
        {
            string sql = "select  acc_edesc,acc_code from FA_CHART_OF_ACCOUNTS_SETUP am where Acc_Code in (select Distinct Acc_Code from FA_GENERAL_LEDGER) and deleted_flag = 'N' and company_code = '01' and Level=4 connect by prior master_acc_code =pre_acc_code order by acc_edesc";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, sql))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        yield return new TrialBalanceModel
                        {
                            ACC_CODE = sdr["acc_code"].ToString(),
                            ACC_EDESC = sdr["acc_edesc"].ToString()

                        };
                    }
                }
            }
        }
        public static string GetLedgerName(string AccountCode)
        {
            string Result = "";
            string Query = "select  acc_edesc from FA_CHART_OF_ACCOUNTS_SETUP am where Acc_Code='" + AccountCode + "'";
            DataSet ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, Query);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                Result = row["acc_edesc"].ToString();
            }
            return Result;
        }

        public static IEnumerable<TrialBalanceModel> getfigdata(string prefCode)
        {
            string Query = "select  amount_figure,amount_roundup,fromdate,todate from trialbalance_pref_master where pref_code='" + prefCode + "'";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, Query))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        yield return new TrialBalanceModel
                        {
                            figureAmount = sdr["amount_figure"].ToString(),
                            roundupAmount = sdr["amount_roundup"].ToString(),
                            fromDate = sdr["fromdate"].ToString(),
                            toDate = sdr["todate"].ToString()
                        };
                    }
                }
            }
        }
        public static IEnumerable<TrialBalanceModel> getsubprefdata(string prefCode)
        {
            string Query = "select  amount_figure,amount_roundup,fromdate,todate,subledger from subledger_pref_master where pref_code='" + prefCode + "'";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, Query))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        yield return new TrialBalanceModel
                        {
                            figureAmount = sdr["amount_figure"].ToString(),
                            roundupAmount = sdr["amount_roundup"].ToString(),
                            fromDate = sdr["fromdate"].ToString(),
                            toDate = sdr["todate"].ToString(),
                            subLedger = sdr["subledger"].ToString()
                        };
                    }
                }
            }
        }
        public static IEnumerable<TrialBalanceModel> getledgerprefdata(string prefCode)
        {
            string Query = "select  amount_figure,amount_roundup,fromdate,todate,ledger from ledger_pref_master where pref_code='" + prefCode + "'";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, Query))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        yield return new TrialBalanceModel
                        {
                            figureAmount = sdr["amount_figure"].ToString(),
                            roundupAmount = sdr["amount_roundup"].ToString(),
                            fromDate = sdr["fromdate"].ToString(),
                            toDate = sdr["todate"].ToString(),
                            ledger = sdr["ledger"].ToString()
                        };
                    }
                }
            }
        }
        public static IEnumerable<TrialBalanceModel> GetLedgerStatement(string AccountCode, string FromDate, string ToDate, string Company, string Branch)
        {
            decimal CrAmount = 0;
            decimal DrAmount = 0;
            decimal TotalCr = 0;
            decimal TotalDr = 0;
            decimal Amount = 0;
            int Count = 1;
            string ToRange = "";

            string SelectQueryOpening = @"select to_bs(TO_DATE('" + FromDate + "', 'YYYY-MM-DD')) FromRange,to_bs(TO_DATE('" + ToDate + "', 'YYYY-MM-DD')) ToRange,fs.Sub_Code,Sub_EDesc,sum(NVL(DR_Amount,0))-sum(NVL(CR_Amount,0)) Amount from FA_Sub_Ledger fs inner join FA_SUB_Ledger_Setup fl on fl.Sub_Code=fs.Sub_Code where fs.Sub_Code in (select Sub_Code from FA_Sub_Ledger_MAP where Acc_Code='" + AccountCode + "') and fl.Company_Code='" + Company + "' and fl.Branch_Code='" + Branch + "' and To_Date(Voucher_Date) < TO_DATE('" + FromDate + "', 'YYYY-MM-DD') group by fs.Sub_Code,Sub_EDesc  order by Sub_EDesc";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, SelectQueryOpening))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        Count = Count + 1;
                        Amount = string.IsNullOrEmpty(sdr["Amount"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["Amount"].ToString());
                        if (Amount < 0)
                        {
                            CrAmount = Math.Abs(Amount);
                            DrAmount = 0;
                        }
                        else
                        {
                            DrAmount = Math.Abs(Amount);
                            CrAmount = 0;
                        }
                        TotalCr = TotalCr + CrAmount;
                        TotalDr = TotalDr + DrAmount;
                        ToRange = sdr["ToRange"].ToString();
                        yield return new TrialBalanceModel
                        {
                            Count = Count,
                            Sub_Code = sdr["FromRange"].ToString(),
                            Sub_EDesc = "Opening Balance",
                            DR_AMOUNT = DrAmount,
                            CR_AMOUNT = CrAmount
                        };
                    }
                }
            }
            string SelectQuery = @"select fs.Sub_Code,Sub_EDesc,sum(NVL(DR_Amount,0)) DR_Amount,sum(NVL(CR_Amount,0)) CR_Amount from FA_Sub_Ledger fs inner join FA_SUB_Ledger_Setup fl on fl.Sub_Code=fs.Sub_Code where fs.Sub_Code in (select Sub_Code from FA_Sub_Ledger_MAP where Acc_Code='" + AccountCode + "') and fl.Company_Code='" + Company + "' and fl.Branch_Code='" + Branch + "' and To_Date(Voucher_Date) between TO_DATE('" + FromDate + "', 'YYYY-MM-DD') and TO_DATE('" + ToDate + "', 'YYYY-MM-DD') group by fs.Sub_Code,Sub_EDesc  order by Sub_EDesc";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, SelectQuery))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        Count = Count + 1;
                        CrAmount = string.IsNullOrEmpty(sdr["CR_Amount"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["CR_Amount"].ToString());
                        DrAmount = string.IsNullOrEmpty(sdr["DR_Amount"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["DR_Amount"].ToString());
                        TotalCr = TotalCr + CrAmount;
                        TotalDr = TotalDr + DrAmount;
                        yield return new TrialBalanceModel
                        {
                            Count = Count,
                            Sub_Code = sdr["Sub_Code"].ToString(),
                            Sub_EDesc = sdr["Sub_EDesc"].ToString(),
                            DR_AMOUNT = DrAmount,
                            CR_AMOUNT = CrAmount
                        };
                    }
                }
            }
            Count = Count + 1;
            if ((TotalDr - TotalCr) < 0)
            {
                TotalCr = Math.Abs(TotalDr - TotalCr);
                TotalDr = 0;
            }
            else
            {
                TotalDr = Math.Abs(TotalDr - TotalCr);
                TotalCr = 0;
            }
            yield return new TrialBalanceModel
            {
                Count = Count,
                Sub_Code = ToRange,
                Sub_EDesc = "<b>Closing Balance</b>",
                DR_AMOUNT = decimal.Parse(TotalDr.ToString()),
                CR_AMOUNT = decimal.Parse(TotalCr.ToString())
            };
        }
        public static string GetSubLedger(string AccountCode, string FromDate, string ToDate, string Company, string Branch)
        {
            StringBuilder Html = new StringBuilder();
            string MainAccount = "";
            MainAccount = MainAccount + "<table width='100%' border='1' id='tablebalancesheet' borderColor='#e0e0e0' cellpadding='0' cellspacing='0' style='border-collapse:collapse'><tr class='HeaderAccount' style='line-height:0.5'><td rowspan='2' style='padding-left: 15px; vertical-align:middle; text-align: left; background:#b7b5b5; color:white; width:1150px; font-weight:bold; font-family:Arial;'>Sub Ledger</td><td colspan='2' style='padding: 7px 10px; vertical-align:middle; text-align: center; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial; min-width:220px;'>Opening Balance</td><td colspan='2' style='padding: 7px 10px; vertical-align:middle; text-align: center; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial; min-width:220px;'>Transaction</td><td colspan='2' style='padding: 7px 10px; vertical-align:middle; text-align: center; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial; min-width:220px;'>Closing Balance</td></tr><tr class='SubHeaderAccount' style='line-height:0.5'><td style='padding: 7px 10px; vertical-align:middle; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial;'>Dr Amount</td><td style='padding: 7px 10px; vertical-align:middle; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial;'>Cr Amount</td><td style='padding: 7px 10px; vertical-align:middle; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial;'>Dr Amount</td><td style='padding: 7px 10px; vertical-align:middle; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial;'>Cr Amount</td><td style='padding: 7px 10px; vertical-align:middle; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial;'>Dr Amount</td><td style='padding: 7px 10px; vertical-align:middle; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial;'>Cr Amount</td></tr>";
            string Query = "select fs.Sub_Code,initcap(Sub_EDesc) Sub_EDesc,NVL((select NVL(sum(NVL(DR_Amount,0))-sum(NVL(CR_Amount,0)),0) Amount from FA_Sub_Ledger fl where fl.Sub_Code=fs.Sub_Code and fl.Company_Code='" + Company + "' and fl.Branch_Code='" + Branch + "' and Form_Code='0'),0)+NVL((select NVL(sum(NVL(DR_Amount,0))-sum(NVL(CR_Amount,0)),0) Amount from FA_Sub_Ledger fl where fl.Sub_Code=fs.Sub_Code and fl.Company_Code='" + Company + "' and fl.Branch_Code='" + Branch + "' and Form_Code!='0' and To_Date(Voucher_Date) < TO_DATE('" + FromDate + "', 'YYYY-MM-DD')),0) as Opening,NVL((select sum(NVL(DR_Amount,0)) Amount from FA_Sub_Ledger fl where fl.Sub_Code=fs.Sub_Code and fl.Company_Code='" + Company + "' and fl.Branch_Code='" + Branch + "' and To_Date(Voucher_Date) between TO_DATE('" + FromDate + "', 'YYYY-MM-DD') and TO_DATE('" + ToDate + "', 'YYYY-MM-DD')),0) as TDebit,NVL((select sum(NVL(CR_Amount,0)) Amount from FA_Sub_Ledger fl where fl.Sub_Code=fs.Sub_Code and fl.Company_Code='" + Company + "' and fl.Branch_Code='" + Branch + "' and To_Date(Voucher_Date) between TO_DATE('" + FromDate + "', 'YYYY-MM-DD') and TO_DATE('" + ToDate + "', 'YYYY-MM-DD')),0) as TCredit from FA_SUB_Ledger_Setup fs where fs.Sub_Code in (select Sub_Code from FA_Sub_Ledger_MAP where Acc_Code='" + AccountCode + "') order by fs.Sub_EDesc";
            DataSet ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, Query);
            decimal DrO = 0; decimal CrO = 0; decimal DrT = 0; decimal CrT = 0;
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                decimal CrAmountO = 0; decimal DrAmountO = 0; decimal CrAmountT = 0; decimal DrAmountT = 0; decimal ClosingDr = 0; decimal ClosingCR = 0;
                decimal OBalance = string.IsNullOrEmpty(row["Opening"].ToString()) ? decimal.Parse("0") : decimal.Parse(row["Opening"].ToString());
                CrAmountT = string.IsNullOrEmpty(row["TCredit"].ToString()) ? decimal.Parse("0") : decimal.Parse(row["TCredit"].ToString());
                DrAmountT = string.IsNullOrEmpty(row["TDebit"].ToString()) ? decimal.Parse("0") : decimal.Parse(row["TDebit"].ToString());

                if (OBalance > 0) { DrAmountO = Math.Abs(OBalance); CrAmountO = 0; }
                else { CrAmountO = Math.Abs(OBalance); DrAmountO = 0; }
                if (((DrAmountO + DrAmountT) - (CrAmountO + CrAmountT)) < 0)
                { ClosingCR = Math.Abs(((DrAmountO + DrAmountT) - (CrAmountO + CrAmountT))); ClosingDr = 0; }
                else { ClosingDr = Math.Abs(((DrAmountO + DrAmountT) - (CrAmountO + CrAmountT))); ClosingCR = 0; }

                MainAccount = MainAccount + "<tr class='LedgerDetailsAccount' style='line-height:1.5'>";
                MainAccount = MainAccount + "<td ><span style='cursor: pointer;' id='" + row["Sub_Code"].ToString() + "'>" + row["Sub_EDesc"].ToString() + "</span></td>";
                MainAccount = MainAccount + "<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", DrAmountO).Replace("$", "") + "</td>";
                MainAccount = MainAccount + "<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", CrAmountO).Replace("$", "") + "</td>";
                MainAccount = MainAccount + "<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", DrAmountT).Replace("$", "") + "</td>";
                MainAccount = MainAccount + "<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", CrAmountT).Replace("$", "") + "</td>";
                MainAccount = MainAccount + "<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", (ClosingDr)).Replace("$", "") + "</td>";
                MainAccount = MainAccount + "<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", (ClosingCR)).Replace("$", "") + "</td>";
                MainAccount = MainAccount + "</tr>";
                DrO = DrO + DrAmountO;
                CrO = CrO + CrAmountO;
                DrT = DrT + DrAmountT;
                CrT = CrT + CrAmountT;
            }
            MainAccount = MainAccount + "<tr class='GroupAccounts' style='background-color:#f7f7f7; line-height:1.5'>";
            MainAccount = MainAccount + "<td style='font-weight:bold;padding-right:15px; border:1;float:none; text-align:right' ><span style='cursor: pointer;'>Grand Total</span></td>";
            MainAccount = MainAccount + "<td style='font-weight:bold;text-align:right;border:1;float:none;'>" + string.Format("{0:c}", DrO).Replace("$", "") + "</td>";
            MainAccount = MainAccount + "<td style='font-weight:bold;text-align:right;border:1;float:none;'>" + string.Format("{0:c}", CrO).Replace("$", "") + "</td>";
            MainAccount = MainAccount + "<td style='font-weight:bold;text-align:right;border:1;float:none;'>" + string.Format("{0:c}", DrT).Replace("$", "") + "</td>";
            MainAccount = MainAccount + "<td style='font-weight:bold;text-align:right;border:1;float:none;'>" + string.Format("{0:c}", CrT).Replace("$", "") + "</td>";
            MainAccount = MainAccount + "<td style='font-weight:bold;text-align:right;border:1;float:none;'>" + string.Format("{0:c}", (DrO + DrT)).Replace("$", "") + "</td>";
            MainAccount = MainAccount + "<td style='font-weight:bold;text-align:right;border:1;float:none;'>" + string.Format("{0:c}", (CrO + CrT)).Replace("$", "") + "</td>";
            MainAccount = MainAccount + "</tr>";
            MainAccount = MainAccount + "</table>";
            if (ds.Tables[0].Rows.Count == 0)
                MainAccount = "";
            return Html.Append(MainAccount).ToString();
        }
        public static string GetSubLedgerPref(string AccountCode, string FromDate, string ToDate, string Company, string Branch, string figureAmount, string roundupAmount)
        {
            StringBuilder Html = new StringBuilder();
            string[] strround = roundupAmount.Split('.');
            if (roundupAmount == null && figureAmount == null)
            {
                roundupAmount = "0.0";
                figureAmount = "1";
            }
            int round = strround[1].Length;
            int fA = Convert.ToInt32(figureAmount);
            string MainAccount = "";
            MainAccount = MainAccount + "<table width='100%' border='1' id='tablebalancesheet' borderColor='#e0e0e0' cellpadding='0' cellspacing='0' style='border-collapse:collapse'><tr class='HeaderAccount' style='line-height:0.5'><td rowspan='2' style='padding-left: 15px; vertical-align:middle; text-align: left; background:#b7b5b5; color:white; width:1150px; font-weight:bold; font-family:Arial;'>Sub Ledger</td><td colspan='2' style='padding: 7px 10px; vertical-align:middle; text-align: center; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial; min-width:220px;'>Opening Balance</td><td colspan='2' style='padding: 7px 10px; vertical-align:middle; text-align: center; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial; min-width:220px;'>Transaction</td><td colspan='2' style='padding: 7px 10px; vertical-align:middle; text-align: center; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial; min-width:220px;'>Closing Balance</td></tr><tr class='SubHeaderAccount' style='line-height:0.5'><td style='padding: 7px 10px; vertical-align:middle; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial;'>Dr Amount</td><td style='padding: 7px 10px; vertical-align:middle; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial;'>Cr Amount</td><td style='padding: 7px 10px; vertical-align:middle; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial;'>Dr Amount</td><td style='padding: 7px 10px; vertical-align:middle; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial;'>Cr Amount</td><td style='padding: 7px 10px; vertical-align:middle; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial;'>Dr Amount</td><td style='padding: 7px 10px; vertical-align:middle; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial;'>Cr Amount</td></tr>";
            string Query = "select fs.Sub_Code,initcap(Sub_EDesc) Sub_EDesc,NVL((select NVL(sum(NVL(DR_Amount,0))-sum(NVL(CR_Amount,0)),0) Amount from FA_Sub_Ledger fl where fl.Sub_Code=fs.Sub_Code and fl.Company_Code='" + Company + "' and fl.Branch_Code='" + Branch + "' and Form_Code='0'),0)+NVL((select NVL(sum(NVL(DR_Amount,0))-sum(NVL(CR_Amount,0)),0) Amount from FA_Sub_Ledger fl where fl.Sub_Code=fs.Sub_Code and fl.Company_Code='" + Company + "' and fl.Branch_Code='" + Branch + "' and Form_Code!='0' and To_Date(Voucher_Date) < TO_DATE('" + FromDate + "', 'YYYY-MM-DD')),0) as Opening,NVL((select sum(NVL(DR_Amount,0)) Amount from FA_Sub_Ledger fl where fl.Sub_Code=fs.Sub_Code and fl.Company_Code='" + Company + "' and fl.Branch_Code='" + Branch + "' and To_Date(Voucher_Date) between TO_DATE('" + FromDate + "', 'YYYY-MM-DD') and TO_DATE('" + ToDate + "', 'YYYY-MM-DD')),0) as TDebit,NVL((select sum(NVL(CR_Amount,0)) Amount from FA_Sub_Ledger fl where fl.Sub_Code=fs.Sub_Code and fl.Company_Code='" + Company + "' and fl.Branch_Code='" + Branch + "' and To_Date(Voucher_Date) between TO_DATE('" + FromDate + "', 'YYYY-MM-DD') and TO_DATE('" + ToDate + "', 'YYYY-MM-DD')),0) as TCredit from FA_SUB_Ledger_Setup fs where fs.Sub_Code in (select Sub_Code from FA_Sub_Ledger_MAP where Acc_Code='" + AccountCode + "') order by fs.Sub_EDesc";
            DataSet ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, Query);
            decimal DrO = 0; decimal CrO = 0; decimal DrT = 0; decimal CrT = 0;
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                decimal CrAmountO = 0; decimal DrAmountO = 0; decimal CrAmountT = 0; decimal DrAmountT = 0; decimal ClosingDr = 0; decimal ClosingCR = 0;
                decimal OBalance = string.IsNullOrEmpty(row["Opening"].ToString()) ? decimal.Parse("0") : decimal.Parse(row["Opening"].ToString());
                CrAmountT = Math.Round((string.IsNullOrEmpty(row["TCredit"].ToString()) ? decimal.Parse("0") : decimal.Parse(row["TCredit"].ToString())) / fA, round);
                DrAmountT = Math.Round((string.IsNullOrEmpty(row["TDebit"].ToString()) ? decimal.Parse("0") : decimal.Parse(row["TDebit"].ToString())) / fA, round);

                if (OBalance > 0) { DrAmountO = Math.Round(OBalance / fA, round); CrAmountO = 0; }
                else { CrAmountO = Math.Abs(Math.Round(OBalance / fA, round)); DrAmountO = 0; }
                if (((DrAmountO + DrAmountT) - (CrAmountO + CrAmountT)) < 0)
                { ClosingCR = Math.Abs(((DrAmountO + DrAmountT) - (CrAmountO + CrAmountT))); ClosingDr = 0; }
                else { ClosingDr = Math.Abs(((DrAmountO + DrAmountT) - (CrAmountO + CrAmountT))); ClosingCR = 0; }

                MainAccount = MainAccount + "<tr class='LedgerDetailsAccount' style='line-height:1.5'>";
                MainAccount = MainAccount + "<td ><span style='cursor: pointer;' id='" + row["Sub_Code"].ToString() + "'>" + row["Sub_EDesc"].ToString() + "</span></td>";
                MainAccount = MainAccount + "<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", DrAmountO).Replace("$", "") + "</td>";
                MainAccount = MainAccount + "<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", CrAmountO).Replace("$", "") + "</td>";
                MainAccount = MainAccount + "<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", DrAmountT).Replace("$", "") + "</td>";
                MainAccount = MainAccount + "<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", CrAmountT).Replace("$", "") + "</td>";
                MainAccount = MainAccount + "<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", (ClosingDr)).Replace("$", "") + "</td>";
                MainAccount = MainAccount + "<td style='text-align:right;border:1;float:none;'>" + string.Format("{0:c}", (ClosingCR)).Replace("$", "") + "</td>";
                MainAccount = MainAccount + "</tr>";
                DrO = DrO + DrAmountO;
                CrO = CrO + CrAmountO;
                DrT = DrT + DrAmountT;
                CrT = CrT + CrAmountT;
            }
            MainAccount = MainAccount + "<tr class='GroupAccounts' style='background-color:#f7f7f7; line-height:1.5'>";
            MainAccount = MainAccount + "<td style='font-weight:bold;padding-right:15px; border:1;float:none; text-align:right' ><span style='cursor: pointer;'>Grand Total</span></td>";
            MainAccount = MainAccount + "<td style='font-weight:bold;text-align:right;border:1;float:none;'>" + string.Format("{0:c}", DrO).Replace("$", "") + "</td>";
            MainAccount = MainAccount + "<td style='font-weight:bold;text-align:right;border:1;float:none;'>" + string.Format("{0:c}", CrO).Replace("$", "") + "</td>";
            MainAccount = MainAccount + "<td style='font-weight:bold;text-align:right;border:1;float:none;'>" + string.Format("{0:c}", DrT).Replace("$", "") + "</td>";
            MainAccount = MainAccount + "<td style='font-weight:bold;text-align:right;border:1;float:none;'>" + string.Format("{0:c}", CrT).Replace("$", "") + "</td>";
            MainAccount = MainAccount + "<td style='font-weight:bold;text-align:right;border:1;float:none;'>" + string.Format("{0:c}", (DrO + DrT)).Replace("$", "") + "</td>";
            MainAccount = MainAccount + "<td style='font-weight:bold;text-align:right;border:1;float:none;'>" + string.Format("{0:c}", (CrO + CrT)).Replace("$", "") + "</td>";
            MainAccount = MainAccount + "</tr>";
            MainAccount = MainAccount + "</table>";
            if (ds.Tables[0].Rows.Count == 0)
                MainAccount = "";
            return Html.Append(MainAccount).ToString();
        }
        public static IEnumerable<TrialBalanceModel> GetSubLedgerDateWise(string AccountCode, string FromDate, string ToDate, string Company, string Branch)
        {
            decimal CrAmount = 0;
            decimal DrAmount = 0;
            decimal TotalCr = 0;
            decimal TotalDr = 0;
            decimal Amount = 0;
            int Count = 1;
            string ToRange = "";
            string SelectQueryOpening = @"select to_bs(TO_DATE('" + FromDate + "', 'YYYY-MM-DD')) FromRange,to_bs(TO_DATE('" + ToDate + "', 'YYYY-MM-DD')) ToRange,(select NVL(sum(NVL(Dr_Amount,0)) -Sum(NVL(Cr_Amount,0)),0) Amount from FA_Sub_Ledger where Sub_Code='" + AccountCode + "' and FORM_CODE=0 and company_code = '" + Company + "' and Branch_Code='" + Branch + "')+(select NVL(sum(NVL(Dr_Amount,0))-Sum(NVL(Cr_Amount,0)),0) Amount from FA_Sub_Ledger where Sub_Code='" + AccountCode + "' and FORM_CODE!=0 and company_code = '" + Company + "' and Branch_Code='" + Branch + "' and To_Date(Voucher_Date) < TO_DATE('" + FromDate + "', 'YYYY-MM-DD')) Amount from dual";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, SelectQueryOpening))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        Count = Count + 1;
                        Amount = string.IsNullOrEmpty(sdr["Amount"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["Amount"].ToString());
                        if (Amount < 0)
                        {
                            CrAmount = Math.Abs(Amount);
                            DrAmount = 0;
                        }
                        else
                        {
                            DrAmount = Math.Abs(Amount);
                            CrAmount = 0;
                        }
                        TotalCr = TotalCr + CrAmount;
                        TotalDr = TotalDr + DrAmount;
                        ToRange = sdr["ToRange"].ToString();
                        yield return new TrialBalanceModel
                        {
                            Count = Count,
                            VoucherNO = "",
                            VoucherDate = sdr["FromRange"].ToString(),
                            Particulars = "<b>Opening Balance</b>",
                            DR_AMOUNT = DrAmount,
                            CR_AMOUNT = CrAmount
                        };
                    }
                }
            }
            string SelectQuery = @"select Voucher_No,to_bs(Voucher_Date) Voucher_Date,Sub_Code,Initcap(Particulars) Particulars,sum(NVL(Dr_Amount,0)) Dr_Amount,Sum(NVL(Cr_Amount,0)) Cr_Amount from FA_Sub_Ledger where VOUCHER_NO!='0' and Sub_Code='" + AccountCode + "' and FORM_CODE!=0 and company_code = '" + Company + "' and Branch_Code='" + Branch + "' and FORM_CODE!=0 and To_Date(Voucher_Date) between TO_DATE('" + FromDate + "', 'YYYY-MM-DD') and TO_DATE('" + ToDate + "', 'YYYY-MM-DD') group by Voucher_No,Voucher_Date,Sub_Code,Particulars order by Voucher_Date";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, SelectQuery))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        Count = Count + 1;
                        CrAmount = string.IsNullOrEmpty(sdr["CR_Amount"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["CR_Amount"].ToString());
                        DrAmount = string.IsNullOrEmpty(sdr["DR_Amount"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["DR_Amount"].ToString());
                        TotalCr = TotalCr + CrAmount;
                        TotalDr = TotalDr + DrAmount;
                        string NewParticular = "";
                        int Length = sdr["Particulars"].ToString().Length;
                        int i = 0;
                        while (Length > 74)
                        {
                            NewParticular = NewParticular + sdr["Particulars"].ToString().Substring(i, 73) + "<br />";
                            Length = Length - 73;
                            i = i + 73;
                        }
                        NewParticular = NewParticular + sdr["Particulars"].ToString().Substring(i, Length);
                        yield return new TrialBalanceModel
                        {
                            Count = Count,
                            VoucherNO = sdr["Voucher_NO"].ToString(),
                            VoucherDate = sdr["Voucher_Date"].ToString(),
                            Particulars = "<span style='text-align:justify; text-transform: lowercase !important;'>" + NewParticular.ToLower() + "</span>",
                            DR_AMOUNT = DrAmount,
                            CR_AMOUNT = CrAmount
                        };
                    }
                }
            }
            Count = Count + 1;
            if ((TotalDr - TotalCr) < 0)
            {
                TotalCr = Math.Abs(TotalDr - TotalCr);
                TotalDr = 0;
            }
            else
            {
                TotalDr = Math.Abs(TotalDr - TotalCr);
                TotalCr = 0;
            }
            yield return new TrialBalanceModel
            {
                Count = Count,
                VoucherNO = "",
                VoucherDate = ToRange,
                Particulars = "<b>Closing Balance</b>",
                DR_AMOUNT = decimal.Parse(TotalDr.ToString()),
                CR_AMOUNT = decimal.Parse(TotalCr.ToString())
            };
        }
        public static IEnumerable<TrialBalanceModel> GetSubLedgerDateWisePref(string AccountCode, string FromDate, string ToDate, string Company, string Branch, string figureAmount, string roundupAmount)
        {
            decimal CrAmount = 0;
            decimal DrAmount = 0;
            decimal TotalCr = 0;
            decimal TotalDr = 0;
            decimal Amount = 0;
            int Count = 1;
            if (roundupAmount == null)
            {
                roundupAmount = "0.0";
                figureAmount = "1";
            }
            string[] strround = roundupAmount.Split('.');
            int round = strround[1].Length;
            int fA = Convert.ToInt32(figureAmount);
            string ToRange = "";
            string SelectQueryOpening = @"select to_bs(TO_DATE('" + FromDate + "', 'YYYY-MM-DD')) FromRange,to_bs(TO_DATE('" + ToDate + "', 'YYYY-MM-DD')) ToRange,(select NVL(sum(NVL(Dr_Amount,0)) -Sum(NVL(Cr_Amount,0)),0) Amount from FA_Sub_Ledger where Sub_Code='" + AccountCode + "' and FORM_CODE=0 and company_code = '" + Company + "' and Branch_Code='" + Branch + "')+(select NVL(sum(NVL(Dr_Amount,0))-Sum(NVL(Cr_Amount,0)),0) Amount from FA_Sub_Ledger where Sub_Code='" + AccountCode + "' and FORM_CODE!=0 and company_code = '" + Company + "' and Branch_Code='" + Branch + "' and To_Date(Voucher_Date) < TO_DATE('" + FromDate + "', 'YYYY-MM-DD')) Amount from dual";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, SelectQueryOpening))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        Count = Count + 1;
                        Amount = string.IsNullOrEmpty(sdr["Amount"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["Amount"].ToString());
                        if (Amount < 0)
                        {
                            CrAmount = Math.Abs(Math.Round(Amount / fA, round));
                            DrAmount = 0;
                        }
                        else
                        {
                            DrAmount = Math.Round(Amount / fA, round);
                            CrAmount = 0;
                        }
                        TotalCr = TotalCr + CrAmount;
                        TotalDr = TotalDr + DrAmount;
                        ToRange = sdr["ToRange"].ToString();
                        yield return new TrialBalanceModel
                        {
                            Count = Count,
                            VoucherNO = "",
                            VoucherDate = sdr["FromRange"].ToString(),
                            Particulars = "<b>Opening Balance</b>",
                            DR_AMOUNT = DrAmount,
                            CR_AMOUNT = CrAmount
                        };
                    }
                }
            }
            string SelectQuery = @"select Voucher_No,to_bs(Voucher_Date) Voucher_Date,Sub_Code,Initcap(Particulars) Particulars,sum(NVL(Dr_Amount,0)) Dr_Amount,Sum(NVL(Cr_Amount,0)) Cr_Amount from FA_Sub_Ledger where VOUCHER_NO!='0' and Sub_Code='" + AccountCode + "' and FORM_CODE!=0 and company_code = '" + Company + "' and Branch_Code='" + Branch + "' and FORM_CODE!=0 and To_Date(Voucher_Date) between TO_DATE('" + FromDate + "', 'YYYY-MM-DD') and TO_DATE('" + ToDate + "', 'YYYY-MM-DD') group by Voucher_No,Voucher_Date,Sub_Code,Particulars order by Voucher_Date";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, SelectQuery))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        Count = Count + 1;
                        CrAmount = Math.Round((string.IsNullOrEmpty(sdr["CR_Amount"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["CR_Amount"].ToString())) / fA, round);
                        DrAmount = Math.Round((string.IsNullOrEmpty(sdr["DR_Amount"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["DR_Amount"].ToString())) / fA, round);
                        TotalCr = TotalCr + CrAmount;
                        TotalDr = TotalDr + DrAmount;
                        string NewParticular = "";
                        int Length = sdr["Particulars"].ToString().Length;
                        int i = 0;
                        while (Length > 74)
                        {
                            NewParticular = NewParticular + sdr["Particulars"].ToString().Substring(i, 73) + "<br />";
                            Length = Length - 73;
                            i = i + 73;
                        }
                        NewParticular = NewParticular + sdr["Particulars"].ToString().Substring(i, Length);
                        yield return new TrialBalanceModel
                        {
                            Count = Count,
                            VoucherNO = sdr["Voucher_NO"].ToString(),
                            VoucherDate = sdr["Voucher_Date"].ToString(),
                            Particulars = "<span style='text-align:justify; text-transform: lowercase !important;'>" + NewParticular.ToLower() + "</span>",
                            DR_AMOUNT = DrAmount,
                            CR_AMOUNT = CrAmount
                        };
                    }
                }
            }
            Count = Count + 1;
            if ((TotalDr - TotalCr) < 0)
            {
                TotalCr = Math.Abs(TotalDr - TotalCr);
                TotalDr = 0;
            }
            else
            {
                TotalDr = Math.Abs(TotalDr - TotalCr);
                TotalCr = 0;
            }
            yield return new TrialBalanceModel
            {
                Count = Count,
                VoucherNO = "",
                VoucherDate = ToRange,
                Particulars = "<b>Closing Balance</b>",
                DR_AMOUNT = decimal.Parse(TotalDr.ToString()),
                CR_AMOUNT = decimal.Parse(TotalCr.ToString())
            };
        }
        public static IEnumerable<TrialBalanceModel> GetSubLedgerDateWiseDetails(string Voucher_NO, string Company, string Branch)
        {
            decimal CrAmount = 0;
            decimal DrAmount = 0;
            decimal TotalDr = 0;
            decimal TotalCr = 0;
            string SelectQuery = @"select FC.ACC_CODE,Voucher_NO,Voucher_Date,Initcap(FC.ACC_EDESC) ACC_EDESC,PARTICULARS,DR_AMOUNT,CR_AMOUNT, FA_GENERAL_LEDGER.CREATED_BY,'Hira' POSTED_BY from FA_GENERAL_LEDGER inner join FA_CHART_OF_ACCOUNTS_SETUP FC on FC.ACC_CODE = FA_GENERAL_LEDGER.ACC_CODE where FA_GENERAL_LEDGER.VOUCHER_NO='" + Voucher_NO + "' and FA_GENERAL_LEDGER.company_code = '" + Company + "' and FA_GENERAL_LEDGER.Branch_Code='" + Branch + "' order by SERIAL_NO";
            //string SelectQuery = @"select FC.Sub_Code ACC_CODE,Voucher_NO,Voucher_Date,Initcap(FC.Sub_EDesc) ACC_EDESC,PARTICULARS,DR_AMOUNT,CR_AMOUNT, FA_Sub_Ledger.CREATED_BY,'Hira' POSTED_BY from FA_Sub_Ledger inner join FA_SUB_Ledger_Setup FC on FC.Sub_CODE = FA_Sub_Ledger.Sub_CODE where FA_Sub_Ledger.VOUCHER_NO='" + Voucher_NO + "' and FA_Sub_Ledger.company_code = '" + Company + "' and FA_Sub_Ledger.Branch_Code='" + Branch + "' order by SERIAL_NO";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, SelectQuery))
            {
                if (sdr.HasRows)
                {
                    int j = 0;
                    yield return new TrialBalanceModel
                    {
                        SNo = "<b>SNo.</b>",
                        VoucherNO = "0",
                        VoucherDate = "2014-01-01",
                        Particulars = "<b>Particulars</b>",
                        Niration = "",
                        DRAMOUNT = "<b>Debit</b>",
                        CRAMOUNT = "<b>Credit</b>",
                        POSTED_BY = "AA",
                        CREATED_BY = "BB"
                    };
                    while (sdr.Read())
                    {
                        j = j + 1;
                        CrAmount = string.IsNullOrEmpty(sdr["CR_Amount"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["CR_Amount"].ToString());
                        DrAmount = string.IsNullOrEmpty(sdr["DR_Amount"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["DR_Amount"].ToString());

                        TotalCr = TotalCr + CrAmount;
                        TotalDr = TotalDr + DrAmount;

                        string PARTICULARS = "";
                        string pp = sdr["PARTICULARS"].ToString();
                        int Length = sdr["PARTICULARS"].ToString().Length;
                        int i = 0;
                        while (Length > 114)
                        {
                            PARTICULARS = PARTICULARS + sdr["PARTICULARS"].ToString().Substring(i, 113) + "<br />";
                            Length = Length - 113;
                            i = i + 113;
                        }
                        PARTICULARS = PARTICULARS + sdr["PARTICULARS"].ToString().Substring(i, Length);
                        yield return new TrialBalanceModel
                        {
                            SNo = j.ToString() + ".",
                            VoucherNO = sdr["Voucher_NO"].ToString(),
                            VoucherDate = sdr["Voucher_Date"].ToString(),
                            Particulars = sdr["ACC_EDESC"].ToString(),
                            Niration = "<span style='text-align:justify; text-transform: capitalize !important;'>" + PARTICULARS.ToLower() + "</span>",
                            DRAMOUNT = DrAmount.ToString("0.00"),
                            CRAMOUNT = CrAmount.ToString("0.00"),
                            POSTED_BY = sdr["POSTED_BY"].ToString(),
                            CREATED_BY = sdr["CREATED_BY"].ToString()
                        };
                    }
                    yield return new TrialBalanceModel
                    {
                        SNo = "",
                        VoucherNO = "ZZZZZZZ99999",
                        VoucherDate = "2014-01-01",
                        Particulars = "<b>Total</b>",
                        Niration = "",
                        DRAMOUNT = "<b>" + TotalDr.ToString("0.00") + "</b>",
                        CRAMOUNT = "<b>" + TotalCr.ToString("0.00") + "</b>",
                        POSTED_BY = "AA",
                        CREATED_BY = "BB"
                    };
                }
            }
        }
        public static IEnumerable<TrialBalanceModel> GetSubLedgerDateWiseDetailsPref(string Voucher_NO, string Company, string Branch, string figureAmount, string roundupAmount)
        {
            decimal CrAmount = 0;
            decimal DrAmount = 0;
            decimal TotalDr = 0;
            decimal TotalCr = 0;
            string[] strround = roundupAmount.Split('.');
            int round = strround[1].Length;
            int fA = Convert.ToInt32(figureAmount);
            string SelectQuery = @"select FC.ACC_CODE,Voucher_NO,Voucher_Date,Initcap(FC.ACC_EDESC) ACC_EDESC,PARTICULARS,DR_AMOUNT,CR_AMOUNT, FA_GENERAL_LEDGER.CREATED_BY,'Hira' POSTED_BY from FA_GENERAL_LEDGER inner join FA_CHART_OF_ACCOUNTS_SETUP FC on FC.ACC_CODE = FA_GENERAL_LEDGER.ACC_CODE where FA_GENERAL_LEDGER.VOUCHER_NO='" + Voucher_NO + "' and FA_GENERAL_LEDGER.company_code = '" + Company + "' and FA_GENERAL_LEDGER.Branch_Code='" + Branch + "' order by SERIAL_NO";
            //string SelectQuery = @"select FC.Sub_Code ACC_CODE,Voucher_NO,Voucher_Date,Initcap(FC.Sub_EDesc) ACC_EDESC,PARTICULARS,DR_AMOUNT,CR_AMOUNT, FA_Sub_Ledger.CREATED_BY,'Hira' POSTED_BY from FA_Sub_Ledger inner join FA_SUB_Ledger_Setup FC on FC.Sub_CODE = FA_Sub_Ledger.Sub_CODE where FA_Sub_Ledger.VOUCHER_NO='" + Voucher_NO + "' and FA_Sub_Ledger.company_code = '" + Company + "' and FA_Sub_Ledger.Branch_Code='" + Branch + "' order by SERIAL_NO";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, SelectQuery))
            {
                if (sdr.HasRows)
                {
                    int j = 0;
                    yield return new TrialBalanceModel
                    {
                        SNo = "<b>SNo.</b>",
                        VoucherNO = "0",
                        VoucherDate = "2014-01-01",
                        Particulars = "<b>Particulars</b>",
                        Niration = "",
                        DRAMOUNT = "<b>Debit</b>",
                        CRAMOUNT = "<b>Credit</b>",
                        POSTED_BY = "AA",
                        CREATED_BY = "BB"
                    };
                    while (sdr.Read())
                    {
                        j = j + 1;
                        CrAmount = Math.Round((string.IsNullOrEmpty(sdr["CR_Amount"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["CR_Amount"].ToString())) / fA, round);
                        DrAmount = Math.Round((string.IsNullOrEmpty(sdr["DR_Amount"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["DR_Amount"].ToString())) / fA, round);

                        TotalCr = TotalCr + CrAmount;
                        TotalDr = TotalDr + DrAmount;

                        string PARTICULARS = "";
                        string pp = sdr["PARTICULARS"].ToString();
                        int Length = sdr["PARTICULARS"].ToString().Length;
                        int i = 0;
                        while (Length > 114)
                        {
                            PARTICULARS = PARTICULARS + sdr["PARTICULARS"].ToString().Substring(i, 113) + "<br />";
                            Length = Length - 113;
                            i = i + 113;
                        }
                        PARTICULARS = PARTICULARS + sdr["PARTICULARS"].ToString().Substring(i, Length);
                        yield return new TrialBalanceModel
                        {
                            SNo = j.ToString() + ".",
                            VoucherNO = sdr["Voucher_NO"].ToString(),
                            VoucherDate = sdr["Voucher_Date"].ToString(),
                            Particulars = sdr["ACC_EDESC"].ToString(),
                            Niration = "<span style='text-align:justify; text-transform: capitalize !important;'>" + PARTICULARS.ToLower() + "</span>",
                            DRAMOUNT = DrAmount.ToString("0.00"),
                            CRAMOUNT = CrAmount.ToString("0.00"),
                            POSTED_BY = sdr["POSTED_BY"].ToString(),
                            CREATED_BY = sdr["CREATED_BY"].ToString()
                        };
                    }
                    yield return new TrialBalanceModel
                    {
                        SNo = "",
                        VoucherNO = "ZZZZZZZ99999",
                        VoucherDate = "2014-01-01",
                        Particulars = "<b>Total</b>",
                        Niration = "",
                        DRAMOUNT = "<b>" + TotalDr.ToString("0.00") + "</b>",
                        CRAMOUNT = "<b>" + TotalCr.ToString("0.00") + "</b>",
                        POSTED_BY = "AA",
                        CREATED_BY = "BB"
                    };
                }
            }
        }
        public static string GetDailySummaryStatement(string AccountCode, string FromDate, string ToDate, string Company, string Branch)
        {
            StringBuilder Html = new StringBuilder();
            string MainAccount = "";
            MainAccount = MainAccount + "<table width='100%' border='1' id='tablebalancesheet' borderColor='#e0e0e0' cellpadding='0' cellspacing='0' style='border-collapse:collapse'><tr class='HeaderAccount' style='line-height:0.5'><td rowspan='2' style='padding-left: 15px; vertical-align:middle; text-align: left; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial;'>Miti</td><td rowspan='2' style='padding-left: 15px; vertical-align:middle; text-align: left; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial;'>Date</td><td colspan='2' style='padding: 7px 10px; vertical-align:middle; text-align: center; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial; min-width:220px;'>Opening Balance</td><td colspan='2' style='padding: 7px 10px; vertical-align:middle; text-align: center; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial; min-width:220px;'>Transaction</td><td colspan='2' style='padding: 7px 10px; vertical-align:middle; text-align: center; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial; min-width:220px;'>Closing Balance</td></tr><tr class='SubHeaderAccount' style='line-height:0.5'><td style='padding: 7px 10px; vertical-align:middle; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial;'>Dr Amount</td><td style='padding: 7px 10px; vertical-align:middle; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial;'>Cr Amount</td><td style='padding: 7px 10px; vertical-align:middle; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial;'>Dr Amount</td><td style='padding: 7px 10px; vertical-align:middle; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial;'>Cr Amount</td><td style='padding: 7px 10px; vertical-align:middle; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial;'>Dr Amount</td><td style='padding: 7px 10px; vertical-align:middle; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial;'>Cr Amount</td></tr>";
            string Query = "select to_bs(fs.Voucher_Date) VoucherDate,to_date(fs.Voucher_Date) as EngDate, (NVL((Select sum(NVL(fl.DR_Amount,0))-sum(NVL(fl.CR_Amount,0)) Amount from FA_General_Ledger fl where fl.Acc_Code='" + AccountCode + "' and fl.Branch_Code='" + Branch + "' and Form_Code='0'),0)+NVL((Select sum(NVL(fl.DR_Amount,0))-sum(NVL(fl.CR_Amount,0)) Amount from FA_General_Ledger fl where fl.Acc_Code='" + AccountCode + "' and fl.Branch_Code='" + Branch + "' and Form_Code!='0' and to_date(fl.Voucher_Date)<to_Date(fs.Voucher_Date)),0)) as Opening,NVL((Select sum(NVL(fl.DR_Amount,0)) Amount from FA_General_Ledger fl where fl.Acc_Code='" + AccountCode + "' and fl.Branch_Code='" + Branch + "' and Form_Code!='0' and to_date(fl.Voucher_Date)=to_Date(fs.Voucher_Date)),0) as TDebit,NVL((Select sum(NVL(fl.CR_Amount,0)) Amount from FA_General_Ledger fl where fl.Acc_Code='" + AccountCode + "' and fl.Branch_Code='" + Branch + "' and Form_Code!='0' and to_date(fl.Voucher_Date)=to_Date(fs.Voucher_Date)),0) as TCredit from FA_General_Ledger fs where fs.Company_Code='" + Company + "' and fs.Branch_Code='" + Branch + "' and fs.Acc_Code='" + AccountCode + "' and To_Date(fs.Voucher_Date) between TO_DATE('" + FromDate + "', 'YYYY-MM-DD') and TO_DATE('" + ToDate + "', 'YYYY-MM-DD') group by fs.Voucher_Date  order by Voucher_Date";
            DataSet ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, Query);
            decimal DrO = 0; decimal CrO = 0; decimal DrT = 0; decimal CrT = 0;
            if (ds.Tables[0].Rows.Count == 0)
            {
                if (FromDate != ToDate)
                    Query = "select to_bs(TO_DATE('" + FromDate + "', 'YYYY-MM-DD')) VoucherDate,to_date(TO_DATE('" + FromDate + "', 'YYYY-MM-DD')) as EngDate,(NVL((Select sum(NVL(fl.DR_Amount,0))-sum(NVL(fl.CR_Amount,0)) Amount from FA_General_Ledger fl where fl.Acc_Code='" + AccountCode + "' and fl.Branch_Code='" + Branch + "' and fl.Company_Code='" + Company + "' and Form_Code='0'),0)+NVL((Select sum(NVL(fl.DR_Amount,0))-sum(NVL(fl.CR_Amount,0)) Amount from FA_General_Ledger fl where fl.Company_Code='" + Company + "' and fl.Acc_Code='" + AccountCode + "' and fl.Branch_Code='" + Branch + "' and Form_Code!='0' and to_date(fl.Voucher_Date)<TO_DATE('" + FromDate + "', 'YYYY-MM-DD')),0)) as Opening,NVL((Select sum(NVL(fl.DR_Amount,0)) Amount from FA_General_Ledger fl where fl.Acc_Code='" + AccountCode + "' and fl.Branch_Code='" + Branch + "' and Form_Code!='0' and fl.Company_Code='" + Company + "' and to_date(fl.Voucher_Date)=TO_DATE('" + FromDate + "', 'YYYY-MM-DD')),0) as TDebit,NVL((Select sum(NVL(fl.CR_Amount,0)) Amount from FA_General_Ledger fl where fl.Acc_Code='" + AccountCode + "' and fl.Branch_Code='" + Branch + "' and Form_Code!='0' and fl.Company_Code='" + Company + "' and to_date(fl.Voucher_Date)=TO_DATE('" + FromDate + "', 'YYYY-MM-DD')),0) as TCredit from dual union all select to_bs(TO_DATE('" + ToDate + "', 'YYYY-MM-DD')) VoucherDate,to_date(TO_DATE('" + ToDate + "', 'YYYY-MM-DD')) as EngDate, (NVL((Select sum(NVL(fl.DR_Amount,0))-sum(NVL(fl.CR_Amount,0)) Amount from FA_General_Ledger fl where fl.Acc_Code='" + AccountCode + "' and fl.Branch_Code='" + Branch + "' and fl.Company_Code='" + Company + "' and Form_Code='0'),0)+NVL((Select sum(NVL(fl.DR_Amount,0))-sum(NVL(fl.CR_Amount,0)) Amount from FA_General_Ledger fl where fl.Company_Code='" + Company + "' and fl.Acc_Code='" + AccountCode + "' and fl.Branch_Code='" + Branch + "' and Form_Code!='0' and to_date(fl.Voucher_Date)<TO_DATE('" + ToDate + "', 'YYYY-MM-DD')),0)) as Opening,NVL((Select sum(NVL(fl.DR_Amount,0)) Amount from FA_General_Ledger fl where fl.Acc_Code='" + AccountCode + "' and fl.Branch_Code='" + Branch + "' and Form_Code!='0' and fl.Company_Code='" + Company + "' and to_date(fl.Voucher_Date)=TO_DATE('" + ToDate + "', 'YYYY-MM-DD')),0) as TDebit,NVL((Select sum(NVL(fl.CR_Amount,0)) Amount from FA_General_Ledger fl where fl.Acc_Code='" + AccountCode + "' and fl.Branch_Code='" + Branch + "' and Form_Code!='0' and fl.Company_Code='" + Company + "' and to_date(fl.Voucher_Date)=TO_DATE('" + ToDate + "', 'YYYY-MM-DD')),0) as TCredit from dual";
                else
                    Query = "select to_bs(TO_DATE('" + FromDate + "', 'YYYY-MM-DD')) VoucherDate,to_date(TO_DATE('" + FromDate + "', 'YYYY-MM-DD')) as EngDate,(NVL((Select sum(NVL(fl.DR_Amount,0))-sum(NVL(fl.CR_Amount,0)) Amount from FA_General_Ledger fl where fl.Acc_Code='" + AccountCode + "' and fl.Branch_Code='" + Branch + "' and fl.Company_Code='" + Company + "' and Form_Code='0'),0)+NVL((Select sum(NVL(fl.DR_Amount,0))-sum(NVL(fl.CR_Amount,0)) Amount from FA_General_Ledger fl where fl.Company_Code='" + Company + "' and fl.Acc_Code='" + AccountCode + "' and fl.Branch_Code='" + Branch + "' and Form_Code!='0' and to_date(fl.Voucher_Date)<TO_DATE('" + FromDate + "', 'YYYY-MM-DD')),0)) as Opening,NVL((Select sum(NVL(fl.DR_Amount,0)) Amount from FA_General_Ledger fl where fl.Acc_Code='" + AccountCode + "' and fl.Branch_Code='" + Branch + "' and Form_Code!='0' and fl.Company_Code='" + Company + "' and to_date(fl.Voucher_Date)=TO_DATE('" + FromDate + "', 'YYYY-MM-DD')),0) as TDebit,NVL((Select sum(NVL(fl.CR_Amount,0)) Amount from FA_General_Ledger fl where fl.Acc_Code='" + AccountCode + "' and fl.Branch_Code='" + Branch + "' and Form_Code!='0' and fl.Company_Code='" + Company + "' and to_date(fl.Voucher_Date)=TO_DATE('" + FromDate + "', 'YYYY-MM-DD')),0) as TCredit from dual";
                ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, Query);
            }
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                decimal CrAmountO = 0; decimal DrAmountO = 0; decimal CrAmountT = 0; decimal DrAmountT = 0; decimal ClosingDr = 0; decimal ClosingCR = 0;
                decimal OBalance = string.IsNullOrEmpty(row["Opening"].ToString()) ? decimal.Parse("0") : decimal.Parse(row["Opening"].ToString());
                CrAmountT = string.IsNullOrEmpty(row["TCredit"].ToString()) ? decimal.Parse("0") : decimal.Parse(row["TCredit"].ToString());
                DrAmountT = string.IsNullOrEmpty(row["TDebit"].ToString()) ? decimal.Parse("0") : decimal.Parse(row["TDebit"].ToString());

                if (OBalance > 0) { DrAmountO = Math.Abs(OBalance); CrAmountO = 0; }
                else { CrAmountO = Math.Abs(OBalance); DrAmountO = 0; }
                if (((DrAmountO + DrAmountT) - (CrAmountO + CrAmountT)) < 0)
                { ClosingCR = Math.Abs(((DrAmountO + DrAmountT) - (CrAmountO + CrAmountT))); ClosingDr = 0; }
                else { ClosingDr = Math.Abs(((DrAmountO + DrAmountT) - (CrAmountO + CrAmountT))); ClosingCR = 0; }

                MainAccount = MainAccount + "<tr class='LedgerDetailsAccount' style='line-height:1.5'>";
                MainAccount = MainAccount + "<td style='padding-left: 5px;'><span style='cursor: pointer;' id='" + DateTime.Parse(row["EngDate"].ToString()).ToString("yyyy-MM-dd") + "'>" + row["VoucherDate"].ToString() + "</span></td>";
                MainAccount = MainAccount + "<td style='padding-left: 5px;'>" + DateTime.Parse(row["EngDate"].ToString()).ToShortDateString() + "</td>";
                MainAccount = MainAccount + "<td style='text-align:right;border:1;float:none;padding-right:5px'>" + string.Format("{0:c}", DrAmountO).Replace("$", "") + "</td>";
                MainAccount = MainAccount + "<td style='text-align:right;border:1;float:none;padding-right:5px'>" + string.Format("{0:c}", CrAmountO).Replace("$", "") + "</td>";
                MainAccount = MainAccount + "<td style='text-align:right;border:1;float:none;padding-right:5px'>" + string.Format("{0:c}", DrAmountT).Replace("$", "") + "</td>";
                MainAccount = MainAccount + "<td style='text-align:right;border:1;float:none;padding-right:5px'>" + string.Format("{0:c}", CrAmountT).Replace("$", "") + "</td>";
                MainAccount = MainAccount + "<td style='text-align:right;border:1;float:none;padding-right:5px'>" + string.Format("{0:c}", (ClosingDr)).Replace("$", "") + "</td>";
                MainAccount = MainAccount + "<td style='text-align:right;border:1;float:none;padding-right:5px'>" + string.Format("{0:c}", (ClosingCR)).Replace("$", "") + "</td>";
                MainAccount = MainAccount + "</tr>";
                DrO = DrO + DrAmountO;
                CrO = CrO + CrAmountO;
                DrT = DrT + DrAmountT;
                CrT = CrT + CrAmountT;
            }
            MainAccount = MainAccount + "</table>";
            if (ds.Tables[0].Rows.Count == 0)
                MainAccount = "";
            return Html.Append(MainAccount).ToString();
        }
        public static string GetDailySummaryStatementPref(string AccountCode, string FromDate, string ToDate, string Company, string Branch, string figureAmount, string roundupAmount)
        {
            StringBuilder Html = new StringBuilder();
            string[] strround = roundupAmount.Split('.');
            int round = strround[1].Length;
            int fA = Convert.ToInt32(figureAmount);
            string MainAccount = "";
            MainAccount = MainAccount + "<table width='100%' border='1' id='tablebalancesheet' borderColor='#e0e0e0' cellpadding='0' cellspacing='0' style='border-collapse:collapse'><tr class='HeaderAccount' style='line-height:0.5'><td rowspan='2' style='padding-left: 15px; vertical-align:middle; text-align: left; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial;'>Miti</td><td rowspan='2' style='padding-left: 15px; vertical-align:middle; text-align: left; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial;'>Date</td><td colspan='2' style='padding: 7px 10px; vertical-align:middle; text-align: center; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial; min-width:220px;'>Opening Balance</td><td colspan='2' style='padding: 7px 10px; vertical-align:middle; text-align: center; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial; min-width:220px;'>Transaction</td><td colspan='2' style='padding: 7px 10px; vertical-align:middle; text-align: center; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial; min-width:220px;'>Closing Balance</td></tr><tr class='SubHeaderAccount' style='line-height:0.5'><td style='padding: 7px 10px; vertical-align:middle; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial;'>Dr Amount</td><td style='padding: 7px 10px; vertical-align:middle; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial;'>Cr Amount</td><td style='padding: 7px 10px; vertical-align:middle; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial;'>Dr Amount</td><td style='padding: 7px 10px; vertical-align:middle; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial;'>Cr Amount</td><td style='padding: 7px 10px; vertical-align:middle; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial;'>Dr Amount</td><td style='padding: 7px 10px; vertical-align:middle; background:#b7b5b5; color:white; font-weight:bold; font-family:Arial;'>Cr Amount</td></tr>";
            string Query = "select to_bs(fs.Voucher_Date) VoucherDate,to_date(fs.Voucher_Date) as EngDate, (NVL((Select sum(NVL(fl.DR_Amount,0))-sum(NVL(fl.CR_Amount,0)) Amount from FA_General_Ledger fl where fl.Acc_Code='" + AccountCode + "' and fl.Branch_Code='" + Branch + "' and Form_Code='0'),0)+NVL((Select sum(NVL(fl.DR_Amount,0))-sum(NVL(fl.CR_Amount,0)) Amount from FA_General_Ledger fl where fl.Acc_Code='" + AccountCode + "' and fl.Branch_Code='" + Branch + "' and Form_Code!='0' and to_date(fl.Voucher_Date)<to_Date(fs.Voucher_Date)),0)) as Opening,NVL((Select sum(NVL(fl.DR_Amount,0)) Amount from FA_General_Ledger fl where fl.Acc_Code='" + AccountCode + "' and fl.Branch_Code='" + Branch + "' and Form_Code!='0' and to_date(fl.Voucher_Date)=to_Date(fs.Voucher_Date)),0) as TDebit,NVL((Select sum(NVL(fl.CR_Amount,0)) Amount from FA_General_Ledger fl where fl.Acc_Code='" + AccountCode + "' and fl.Branch_Code='" + Branch + "' and Form_Code!='0' and to_date(fl.Voucher_Date)=to_Date(fs.Voucher_Date)),0) as TCredit from FA_General_Ledger fs where fs.Company_Code='" + Company + "' and fs.Branch_Code='" + Branch + "' and fs.Acc_Code='" + AccountCode + "' and To_Date(fs.Voucher_Date) between TO_DATE('" + FromDate + "', 'YYYY-MM-DD') and TO_DATE('" + ToDate + "', 'YYYY-MM-DD') group by fs.Voucher_Date  order by Voucher_Date";
            DataSet ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, Query);
            decimal DrO = 0; decimal CrO = 0; decimal DrT = 0; decimal CrT = 0;
            if (ds.Tables[0].Rows.Count == 0)
            {
                if (FromDate != ToDate)
                    Query = "select to_bs(TO_DATE('" + FromDate + "', 'YYYY-MM-DD')) VoucherDate,to_date(TO_DATE('" + FromDate + "', 'YYYY-MM-DD')) as EngDate,(NVL((Select sum(NVL(fl.DR_Amount,0))-sum(NVL(fl.CR_Amount,0)) Amount from FA_General_Ledger fl where fl.Acc_Code='" + AccountCode + "' and fl.Branch_Code='" + Branch + "' and fl.Company_Code='" + Company + "' and Form_Code='0'),0)+NVL((Select sum(NVL(fl.DR_Amount,0))-sum(NVL(fl.CR_Amount,0)) Amount from FA_General_Ledger fl where fl.Company_Code='" + Company + "' and fl.Acc_Code='" + AccountCode + "' and fl.Branch_Code='" + Branch + "' and Form_Code!='0' and to_date(fl.Voucher_Date)<TO_DATE('" + FromDate + "', 'YYYY-MM-DD')),0)) as Opening,NVL((Select sum(NVL(fl.DR_Amount,0)) Amount from FA_General_Ledger fl where fl.Acc_Code='" + AccountCode + "' and fl.Branch_Code='" + Branch + "' and Form_Code!='0' and fl.Company_Code='" + Company + "' and to_date(fl.Voucher_Date)=TO_DATE('" + FromDate + "', 'YYYY-MM-DD')),0) as TDebit,NVL((Select sum(NVL(fl.CR_Amount,0)) Amount from FA_General_Ledger fl where fl.Acc_Code='" + AccountCode + "' and fl.Branch_Code='" + Branch + "' and Form_Code!='0' and fl.Company_Code='" + Company + "' and to_date(fl.Voucher_Date)=TO_DATE('" + FromDate + "', 'YYYY-MM-DD')),0) as TCredit from dual union all select to_bs(TO_DATE('" + ToDate + "', 'YYYY-MM-DD')) VoucherDate,to_date(TO_DATE('" + ToDate + "', 'YYYY-MM-DD')) as EngDate, (NVL((Select sum(NVL(fl.DR_Amount,0))-sum(NVL(fl.CR_Amount,0)) Amount from FA_General_Ledger fl where fl.Acc_Code='" + AccountCode + "' and fl.Branch_Code='" + Branch + "' and fl.Company_Code='" + Company + "' and Form_Code='0'),0)+NVL((Select sum(NVL(fl.DR_Amount,0))-sum(NVL(fl.CR_Amount,0)) Amount from FA_General_Ledger fl where fl.Company_Code='" + Company + "' and fl.Acc_Code='" + AccountCode + "' and fl.Branch_Code='" + Branch + "' and Form_Code!='0' and to_date(fl.Voucher_Date)<TO_DATE('" + ToDate + "', 'YYYY-MM-DD')),0)) as Opening,NVL((Select sum(NVL(fl.DR_Amount,0)) Amount from FA_General_Ledger fl where fl.Acc_Code='" + AccountCode + "' and fl.Branch_Code='" + Branch + "' and Form_Code!='0' and fl.Company_Code='" + Company + "' and to_date(fl.Voucher_Date)=TO_DATE('" + ToDate + "', 'YYYY-MM-DD')),0) as TDebit,NVL((Select sum(NVL(fl.CR_Amount,0)) Amount from FA_General_Ledger fl where fl.Acc_Code='" + AccountCode + "' and fl.Branch_Code='" + Branch + "' and Form_Code!='0' and fl.Company_Code='" + Company + "' and to_date(fl.Voucher_Date)=TO_DATE('" + ToDate + "', 'YYYY-MM-DD')),0) as TCredit from dual";
                else
                    Query = "select to_bs(TO_DATE('" + FromDate + "', 'YYYY-MM-DD')) VoucherDate,to_date(TO_DATE('" + FromDate + "', 'YYYY-MM-DD')) as EngDate,(NVL((Select sum(NVL(fl.DR_Amount,0))-sum(NVL(fl.CR_Amount,0)) Amount from FA_General_Ledger fl where fl.Acc_Code='" + AccountCode + "' and fl.Branch_Code='" + Branch + "' and fl.Company_Code='" + Company + "' and Form_Code='0'),0)+NVL((Select sum(NVL(fl.DR_Amount,0))-sum(NVL(fl.CR_Amount,0)) Amount from FA_General_Ledger fl where fl.Company_Code='" + Company + "' and fl.Acc_Code='" + AccountCode + "' and fl.Branch_Code='" + Branch + "' and Form_Code!='0' and to_date(fl.Voucher_Date)<TO_DATE('" + FromDate + "', 'YYYY-MM-DD')),0)) as Opening,NVL((Select sum(NVL(fl.DR_Amount,0)) Amount from FA_General_Ledger fl where fl.Acc_Code='" + AccountCode + "' and fl.Branch_Code='" + Branch + "' and Form_Code!='0' and fl.Company_Code='" + Company + "' and to_date(fl.Voucher_Date)=TO_DATE('" + FromDate + "', 'YYYY-MM-DD')),0) as TDebit,NVL((Select sum(NVL(fl.CR_Amount,0)) Amount from FA_General_Ledger fl where fl.Acc_Code='" + AccountCode + "' and fl.Branch_Code='" + Branch + "' and Form_Code!='0' and fl.Company_Code='" + Company + "' and to_date(fl.Voucher_Date)=TO_DATE('" + FromDate + "', 'YYYY-MM-DD')),0) as TCredit from dual";
                ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, Query);
            }
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                decimal CrAmountO = 0; decimal DrAmountO = 0; decimal CrAmountT = 0; decimal DrAmountT = 0; decimal ClosingDr = 0; decimal ClosingCR = 0;
                decimal OBalance = string.IsNullOrEmpty(row["Opening"].ToString()) ? decimal.Parse("0") : decimal.Parse(row["Opening"].ToString());
                CrAmountT = Math.Round((string.IsNullOrEmpty(row["TCredit"].ToString()) ? decimal.Parse("0") : decimal.Parse(row["TCredit"].ToString())) / fA, round);
                DrAmountT = Math.Round((string.IsNullOrEmpty(row["TDebit"].ToString()) ? decimal.Parse("0") : decimal.Parse(row["TDebit"].ToString())) / fA, round);


                if (OBalance > 0) { DrAmountO = Math.Round(OBalance / fA, round); CrAmountO = 0; }
                else { CrAmountO = Math.Abs(Math.Round(OBalance / fA, round)); DrAmountO = 0; }
                if (((DrAmountO + DrAmountT) - (CrAmountO + CrAmountT)) < 0)
                { ClosingCR = Math.Abs(((DrAmountO + DrAmountT) - (CrAmountO + CrAmountT))); ClosingDr = 0; }
                else { ClosingDr = Math.Abs(((DrAmountO + DrAmountT) - (CrAmountO + CrAmountT))); ClosingCR = 0; }

                MainAccount = MainAccount + "<tr class='LedgerDetailsAccount' style='line-height:1.5'>";
                MainAccount = MainAccount + "<td style='padding-left: 5px;'><span style='cursor: pointer;' id='" + DateTime.Parse(row["EngDate"].ToString()).ToString("yyyy-MM-dd") + "'>" + row["VoucherDate"].ToString() + "</span></td>";
                MainAccount = MainAccount + "<td style='padding-left: 5px;'>" + DateTime.Parse(row["EngDate"].ToString()).ToShortDateString() + "</td>";
                MainAccount = MainAccount + "<td style='text-align:right;border:1;float:none;padding-right:5px'>" + string.Format("{0:c}", DrAmountO).Replace("$", "") + "</td>";
                MainAccount = MainAccount + "<td style='text-align:right;border:1;float:none;padding-right:5px'>" + string.Format("{0:c}", CrAmountO).Replace("$", "") + "</td>";
                MainAccount = MainAccount + "<td style='text-align:right;border:1;float:none;padding-right:5px'>" + string.Format("{0:c}", DrAmountT).Replace("$", "") + "</td>";
                MainAccount = MainAccount + "<td style='text-align:right;border:1;float:none;padding-right:5px'>" + string.Format("{0:c}", CrAmountT).Replace("$", "") + "</td>";
                MainAccount = MainAccount + "<td style='text-align:right;border:1;float:none;padding-right:5px'>" + string.Format("{0:c}", (ClosingDr)).Replace("$", "") + "</td>";
                MainAccount = MainAccount + "<td style='text-align:right;border:1;float:none;padding-right:5px'>" + string.Format("{0:c}", (ClosingCR)).Replace("$", "") + "</td>";
                MainAccount = MainAccount + "</tr>";
                DrO = DrO + DrAmountO;
                CrO = CrO + CrAmountO;
                DrT = DrT + DrAmountT;
                CrT = CrT + CrAmountT;
            }
            MainAccount = MainAccount + "</table>";
            if (ds.Tables[0].Rows.Count == 0)
                MainAccount = "";
            return Html.Append(MainAccount).ToString();
        }
        public static string SavePreference(string prefName, string dateStep, string fromDate, string toDate, string figureAmount, string roundupAmount, string SetDefault)
        {
            //fromDate = Convert.ToDateTime(fromDate).ToString("dd-MMM-yyyy");
            //toDate = Convert.ToDateTime(toDate).ToString("dd-MMM-yyyy");
            if (SetDefault == "True")
            {
                SetDefault = "Y";
            }
            else
            {
                SetDefault = "N";
            }
            string PrefCode = null;
            string GetPrefName = "SELECT PREF_CODE,PREF_NAME FROM trialbalance_pref_master";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, GetPrefName))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        if (prefName == sdr["PREF_NAME"].ToString())
                        {
                            PrefCode = sdr["PREF_CODE"].ToString();
                        }
                    }
                }
            }
            if (PrefCode != null)
            {
                if (SetDefault == null)
                {
                    string UpdateMasterSql = "UPDATE trialbalance_pref_master SET AMOUNT_FIGURE='" + figureAmount + "',AMOUNT_ROUNDUP='" + roundupAmount + "',DateStep='" + dateStep + "',FromDate='" + fromDate + "',Todate='" + toDate + "' WHERE PREF_CODE = '" + PrefCode + "'";
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, UpdateMasterSql);
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, "COMMIT");
                    string DeleteDetailSql = "DELETE FROM trialbalance_pref_master WHERE PREF_CODE = '" + PrefCode + "'";
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, DeleteDetailSql);
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, "COMMIT");
                }
                else
                {
                    string UpdateDefaultFlag = "UPDATE trialbalance_pref_master SET ISDEFAULT = 'N'";
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, UpdateDefaultFlag);
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, "COMMIT");
                    string UpdateMasterSql = "UPDATE trialbalance_pref_master SET AMOUNT_FIGURE='" + figureAmount + "',AMOUNT_ROUNDUP='" + roundupAmount + "',ISDEFAULT='Y',DateStep='" + dateStep + "',FromDate='" + fromDate + "',Todate='" + toDate + "' WHERE PREF_CODE = '" + PrefCode + "'";
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, UpdateMasterSql);
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, "COMMIT");
                }
            }
            else
            {
                int PREF_CODE = 0;
                string findmaxcode = "SELECT MAX(PREF_CODE) PREF_CODE FROM trialbalance_pref_master";
                DataSet ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, findmaxcode);
                string CODE = ds.Tables[0].Rows[0]["PREF_CODE"].ToString();
                if (CODE == "")
                {
                    PREF_CODE = 0;
                }
                else
                {
                    PREF_CODE = Convert.ToInt16(CODE);
                }
                PREF_CODE = PREF_CODE + 1;
                string inserty = null;
                if (SetDefault != null)
                {
                    //string UpdateDefaultFlag = "UPDATE trialbalance_pref_master SET ISDEFAULT = 'N'";
                    //OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, UpdateDefaultFlag);
                    //OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, "COMMIT");
                    inserty = @"INSERT INTO trialbalance_pref_master(
                            PREF_CODE,pref_name,amount_figure,amount_roundup,fromDate,ToDate,DateStep,isdefault)VALUES
                            ('" + PREF_CODE + "','" + prefName + "','" + figureAmount + "','" + roundupAmount + "','" + fromDate + "','" + toDate + "','" + dateStep + "','" + SetDefault + "')";
                }
                else
                {
                    inserty = @"INSERT INTO trialbalance_pref_master(
                            PREF_CODE,pref_name,amount_figure,amount_roundup,fromDate,ToDate,DateStep,isdefault)VALUES
                            ('" + PREF_CODE + "','" + prefName + "','" + figureAmount + "','" + roundupAmount + "','" + fromDate + "','" + toDate + "','" + dateStep + "','" + SetDefault + "')";
                }


                OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, inserty);
                OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, "COMMIT");
            }
            return "Successfull";
        }
        internal static string GetDataForConsolidatedAjax()
        {
            string sql = @"select * from trialbalance_pref_master";
            DataSet ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, sql);
            DataTable tblGroup = ds.Tables[0];

            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in tblGroup.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in tblGroup.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            return serializer.Serialize(rows);
            //return tblGroup;
        }
        internal static string GetAllPreferencesVariable(string rowKey)
        {
            string PreferenceSetup = null;
            string sql = @"select * from trialbalance_pref_master WHERE PREF_CODE = '" + rowKey + "'";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, sql))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        PreferenceSetup = sdr["PREF_CODE"].ToString();
                        PreferenceSetup = PreferenceSetup + "," + sdr["PREF_NAME"].ToString();
                        PreferenceSetup = PreferenceSetup + "," + sdr["AMOUNT_FIGURE"].ToString();
                        PreferenceSetup = PreferenceSetup + "," + sdr["AMOUNT_ROUNDUP"].ToString();
                        PreferenceSetup = PreferenceSetup + "," + sdr["DATESTEP"].ToString();
                        PreferenceSetup = PreferenceSetup + "," + sdr["FROMDATE"].ToString();
                        PreferenceSetup = PreferenceSetup + "," + sdr["TODATE"].ToString();
                    }

                }
            }
            return PreferenceSetup;
        }
        internal static void RemoveSelectedRow(string rowKey)
        {
            string DeleteMaster = "DELETE FROM trialbalance_pref_master WHERE PREF_CODE = '" + rowKey + "'";
            OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, DeleteMaster);
            OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, "COMMIT");
        }
        internal static string GetPreferenceList()
        {
            string PrefNames = null;
            string GetPrefernceSql = "SELECT PREF_NAME FROM trialbalance_pref_master";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, GetPrefernceSql))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        if (PrefNames != null)
                        {
                            PrefNames = PrefNames + "," + sdr["PREF_NAME"].ToString();
                        }
                        else
                        {
                            PrefNames = sdr["PREF_NAME"].ToString();
                        }
                    }

                }
            }
            return PrefNames;
        }
        public static DataTable GetMonthwiseSummary(string from, string to, string AccountHead)
        {
            DataTable tblGroup = new DataTable();
            string SelectQuery = @"select (select Acc_EDesc from Fa_Chart_Of_Accounts_setup where Fa_Chart_Of_Accounts_setup.Acc_Code=FA_GENERAL_LEDGER.Acc_Code) Acc_EDesc,
                        SUBSTR(to_bs(Voucher_Date),4,2)  Voucher_Date,SUBSTR (to_bs(Voucher_Date),7,4) AS YEAR,
                        NVL(NVL(sum(NVL(Dr_Amount,0)),0) -NVL(Sum(NVL(Cr_Amount,0)),0),0) Amount from 
                        FA_GENERAL_LEDGER 
                        where FA_GENERAL_LEDGER.Acc_Code='" + AccountHead + "' and To_Date(Voucher_Date) between TO_DATE('" + from + "', 'YYYY-MM-DD') and TO_DATE('" + to + "', 'YYYY-MM-DD') group by SUBSTR(to_bs(Voucher_Date),4,2),FA_GENERAL_LEDGER.Acc_Code,SUBSTR (to_bs(Voucher_Date),7,4) order by voucher_date";
            DataSet ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, SelectQuery);
            tblGroup = ds.Tables[0];
            return tblGroup;
        }
        public static string SavePreferenceLedgerStatement(string prefName, string dateStep, string fromDate, string toDate, string figureAmount, string roundupAmount, string SetDefault, string ledger)
        {
            //fromDate = Convert.ToDateTime(fromDate).ToString("dd-MMM-yyyy");
            //toDate = Convert.ToDateTime(toDate).ToString("dd-MMM-yyyy");
            string PrefCode = null;
            string GetPrefName = "SELECT PREF_CODE,PREF_NAME FROM ledger_pref_master";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, GetPrefName))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        if (prefName == sdr["PREF_NAME"].ToString())
                        {
                            PrefCode = sdr["PREF_CODE"].ToString();
                        }
                    }
                }
            }
            if (PrefCode != null)
            {
                if (SetDefault == null)
                {
                    string UpdateMasterSql = "UPDATE ledger_pref_master SET AMOUNT_FIGURE='" + figureAmount + "',AMOUNT_ROUNDUP='" + roundupAmount + "',DateStep='" + dateStep + "',FromDate='" + fromDate + "',Todate='" + toDate + "',ledger='" + ledger + "' WHERE PREF_CODE = '" + PrefCode + "'";
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, UpdateMasterSql);
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, "COMMIT");
                    string DeleteDetailSql = "DELETE FROM ledger_pref_master WHERE PREF_CODE = '" + PrefCode + "'";
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, DeleteDetailSql);
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, "COMMIT");
                }
                else
                {
                    string UpdateDefaultFlag = "UPDATE ledger_pref_master SET ISDEFAULT = 'N'";
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, UpdateDefaultFlag);
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, "COMMIT");
                    string UpdateMasterSql = "UPDATE ledger_pref_master SET AMOUNT_FIGURE='" + figureAmount + "',AMOUNT_ROUNDUP='" + roundupAmount + "',ISDEFAULT='Y',DateStep='" + dateStep + "',FromDate='" + fromDate + "',Todate='" + toDate + "',ledger='" + ledger + "' WHERE PREF_CODE = '" + PrefCode + "'";
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, UpdateMasterSql);
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, "COMMIT");
                }
            }
            else
            {
                int PREF_CODE = 0;
                string findmaxcode = "SELECT MAX(PREF_CODE) PREF_CODE FROM ledger_pref_master";
                DataSet ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, findmaxcode);
                string CODE = ds.Tables[0].Rows[0]["PREF_CODE"].ToString();
                if (CODE == "")
                {
                    PREF_CODE = 0;
                }
                else
                {
                    PREF_CODE = Convert.ToInt16(CODE);
                }
                PREF_CODE = PREF_CODE + 1;
                string inserty = null;
                if (SetDefault != null)
                {
                    string UpdateDefaultFlag = "UPDATE ledger_pref_master SET ISDEFAULT = 'N'";
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, UpdateDefaultFlag);
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, "COMMIT");
                    inserty = @"INSERT INTO ledger_pref_master(
                            PREF_CODE,pref_name,amount_figure,amount_roundup,fromDate,ToDate,DateStep,isdefault,ledger)VALUES
                            ('" + PREF_CODE + "','" + prefName + "','" + figureAmount + "','" + roundupAmount + "','" + fromDate + "','" + toDate + "','" + dateStep + "','Y','" + ledger + "')";
                }
                else
                {
                    inserty = @"INSERT INTO ledger_pref_master(
                            PREF_CODE,pref_name,amount_figure,amount_roundup,fromDate,ToDate,DateStep,isdefault,ledger)VALUES
                            ('" + PREF_CODE + "','" + prefName + "','" + figureAmount + "','" + roundupAmount + "','" + fromDate + "','" + toDate + "','" + dateStep + "','N','" + ledger + "')";
                }


                OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, inserty);
                OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, "COMMIT");
            }
            return "Successfull";
        }
        internal static string GetDataForLedgerAjax()
        {
            string sql = @"select * from ledger_pref_master";
            DataSet ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, sql);
            DataTable tblGroup = ds.Tables[0];

            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in tblGroup.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in tblGroup.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            return serializer.Serialize(rows);
            //return tblGroup;
        }
        internal static string GetAllPreferencesVariableLedger(string rowKey)
        {
            string PreferenceSetup = null;
            string sql = @"select * from ledger_pref_master WHERE PREF_CODE = '" + rowKey + "'";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, sql))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        PreferenceSetup = sdr["PREF_CODE"].ToString();
                        PreferenceSetup = PreferenceSetup + "," + sdr["PREF_NAME"].ToString();
                        PreferenceSetup = PreferenceSetup + "," + sdr["AMOUNT_FIGURE"].ToString();
                        PreferenceSetup = PreferenceSetup + "," + sdr["AMOUNT_ROUNDUP"].ToString();
                        PreferenceSetup = PreferenceSetup + "," + sdr["DATESTEP"].ToString();
                        PreferenceSetup = PreferenceSetup + "," + sdr["FROMDATE"].ToString();
                        PreferenceSetup = PreferenceSetup + "," + sdr["TODATE"].ToString();
                        PreferenceSetup = PreferenceSetup + "," + sdr["LEDGER"].ToString();
                    }

                }
            }
            return PreferenceSetup;
        }
        internal static void RemoveSelectedRowLedger(string rowKey)
        {
            string DeleteMaster = "DELETE FROM ledger_pref_master WHERE PREF_CODE = '" + rowKey + "'";
            OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, DeleteMaster);
            OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, "COMMIT");
        }
        internal static string GetPreferenceListLedger()
        {
            string PrefNames = null;
            string GetPrefernceSql = "SELECT PREF_NAME FROM ledger_pref_master";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, GetPrefernceSql))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        if (PrefNames != null)
                        {
                            PrefNames = PrefNames + "," + sdr["PREF_NAME"].ToString();
                        }
                        else
                        {
                            PrefNames = sdr["PREF_NAME"].ToString();
                        }
                    }

                }
            }
            return PrefNames;
        }

        public static string SavePreferenceSubLedgerStatement(string prefName, string dateStep, string fromDate, string toDate, string figureAmount, string roundupAmount, string SetDefault, string subledger)
        {
            //fromDate = Convert.ToDateTime(fromDate).ToString("dd-MMM-yyyy");
            //toDate = Convert.ToDateTime(toDate).ToString("dd-MMM-yyyy");
            string PrefCode = null;
            string GetPrefName = "SELECT PREF_CODE,PREF_NAME FROM subledger_pref_master";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, GetPrefName))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        if (prefName == sdr["PREF_NAME"].ToString())
                        {
                            PrefCode = sdr["PREF_CODE"].ToString();
                        }
                    }
                }
            }
            if (PrefCode != null)
            {
                if (SetDefault == null)
                {
                    string UpdateMasterSql = "UPDATE subledger_pref_master SET AMOUNT_FIGURE='" + figureAmount + "',AMOUNT_ROUNDUP='" + roundupAmount + "',DateStep='" + dateStep + "',FromDate='" + fromDate + "',Todate='" + toDate + "',ledger='" + subledger + "' WHERE PREF_CODE = '" + PrefCode + "'";
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, UpdateMasterSql);
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, "COMMIT");
                    string DeleteDetailSql = "DELETE FROM subledger_pref_master WHERE PREF_CODE = '" + PrefCode + "'";
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, DeleteDetailSql);
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, "COMMIT");
                }
                else
                {
                    string UpdateDefaultFlag = "UPDATE subledger_pref_master SET ISDEFAULT = 'N'";
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, UpdateDefaultFlag);
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, "COMMIT");
                    string UpdateMasterSql = "UPDATE subledger_pref_master SET AMOUNT_FIGURE='" + figureAmount + "',AMOUNT_ROUNDUP='" + roundupAmount + "',ISDEFAULT='Y',DateStep='" + dateStep + "',FromDate='" + fromDate + "',Todate='" + toDate + "',ledger='" + subledger + "' WHERE PREF_CODE = '" + PrefCode + "'";
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, UpdateMasterSql);
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, "COMMIT");
                }
            }
            else
            {
                int PREF_CODE = 0;
                string findmaxcode = "SELECT MAX(PREF_CODE) PREF_CODE FROM subledger_pref_master";
                DataSet ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, findmaxcode);
                string CODE = ds.Tables[0].Rows[0]["PREF_CODE"].ToString();
                if (CODE == "")
                {
                    PREF_CODE = 0;
                }
                else
                {
                    PREF_CODE = Convert.ToInt16(CODE);
                }
                PREF_CODE = PREF_CODE + 1;
                string inserty = null;
                if (SetDefault != null)
                {
                    string UpdateDefaultFlag = "UPDATE subledger_pref_master SET ISDEFAULT = 'N'";
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, UpdateDefaultFlag);
                    OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, "COMMIT");
                    inserty = @"INSERT INTO subledger_pref_master(
                            PREF_CODE,pref_name,amount_figure,amount_roundup,fromDate,ToDate,DateStep,isdefault,subledger)VALUES
                            ('" + PREF_CODE + "','" + prefName + "','" + figureAmount + "','" + roundupAmount + "','" + fromDate + "','" + toDate + "','" + dateStep + "','Y','" + subledger + "')";
                }
                else
                {
                    inserty = @"INSERT INTO subledger_pref_master(
                            PREF_CODE,pref_name,amount_figure,amount_roundup,fromDate,ToDate,DateStep,isdefault,subledger)VALUES
                            ('" + PREF_CODE + "','" + prefName + "','" + figureAmount + "','" + roundupAmount + "','" + fromDate + "','" + toDate + "','" + dateStep + "','N','" + subledger + "')";
                }


                OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, inserty);
                OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, "COMMIT");
            }
            return "Successfull";
        }
        internal static string GetDataForSubLedgerAjax()
        {
            string sql = @"select * from subledger_pref_master";
            DataSet ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, sql);
            DataTable tblGroup = ds.Tables[0];

            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in tblGroup.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in tblGroup.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            return serializer.Serialize(rows);
            //return tblGroup;
        }
        internal static string GetAllPreferencesVariableSubLedger(string rowKey)
        {
            string PreferenceSetup = null;
            string sql = @"select * from subledger_pref_master WHERE PREF_CODE = '" + rowKey + "'";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, sql))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        PreferenceSetup = sdr["PREF_CODE"].ToString();
                        PreferenceSetup = PreferenceSetup + "," + sdr["PREF_NAME"].ToString();
                        PreferenceSetup = PreferenceSetup + "," + sdr["AMOUNT_FIGURE"].ToString();
                        PreferenceSetup = PreferenceSetup + "," + sdr["AMOUNT_ROUNDUP"].ToString();
                        PreferenceSetup = PreferenceSetup + "," + sdr["DATESTEP"].ToString();
                        PreferenceSetup = PreferenceSetup + "," + sdr["FROMDATE"].ToString();
                        PreferenceSetup = PreferenceSetup + "," + sdr["TODATE"].ToString();
                        PreferenceSetup = PreferenceSetup + "," + sdr["SUBLEDGER"].ToString();
                    }

                }
            }
            return PreferenceSetup;
        }
        internal static void RemoveSelectedRowSubLedger(string rowKey)
        {
            string DeleteMaster = "DELETE FROM subledger_pref_master WHERE PREF_CODE = '" + rowKey + "'";
            OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, DeleteMaster);
            OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, "COMMIT");
        }
        internal static string GetPreferenceListSubLedger()
        {
            string PrefNames = null;
            string GetPrefernceSql = "SELECT PREF_NAME FROM subledger_pref_master";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, GetPrefernceSql))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        if (PrefNames != null)
                        {
                            PrefNames = PrefNames + "," + sdr["PREF_NAME"].ToString();
                        }
                        else
                        {
                            PrefNames = sdr["PREF_NAME"].ToString();
                        }
                    }

                }
            }
            return PrefNames;
        }



        public string figureAmount { get; set; }

        public string roundupAmount { get; set; }

        public string fromDate { get; set; }

        public string toDate { get; set; }

        public string subLedger { get; set; }

        public string ledger { get; set; }
    }
    public class TrialBalanceExportModel
    {
        public string ACCOUNTHEAD { get; set; }
        public string CRAMOUNT { get; set; }
        public string DRAMOUNT { get; set; }
        public static IEnumerable<TrialBalanceExportModel> GetExportToExcelData(string AccountHead, string Level)
        {
            decimal CrAmount = 0;
            decimal DrAmount = 0;
            string SelectQuery = @"select  (lpad(' ',2*(level-1)) || acc_edesc) account_head,LEVEL,Pre_Acc_Code,Master_ACC_Code,
                                          decode((select sum(dr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                               where Form_Code <> 0 and  pre_acc_code like (am.master_acc_code||'%')), null,(select sum(dr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                              where Form_Code <> 0 and acc_code = am.acc_code),(select sum(dr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                               where Form_Code <> 0 and  pre_acc_code like (am.master_acc_code||'%')))  dr_amt,
                                          decode((select sum(cr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                              where Form_Code <> 0 and pre_acc_code like (am.master_acc_code||'%')) ,null,(select sum(cr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                              where Form_Code <> 0 and acc_code = am.acc_code),(select sum(cr_aMOUNT) from V_VIRTUAL_GENERAL_LEDGER1
                                              where Form_Code <> 0 and pre_acc_code like (am.master_acc_code||'%'))) cr_amt
                                          from FA_CHART_OF_ACCOUNTS_SETUP am
                                           where deleted_flag = 'N'
                                           and company_code = '01'
                                             and Pre_Acc_Code='" + AccountHead + "'";
            if (Level == "0")
            {
                SelectQuery = SelectQuery + " connect by prior master_acc_code =pre_acc_code";
            }
            else
            {
                SelectQuery = SelectQuery + " and level = '" + Level + "'connect by prior master_acc_code =pre_acc_code";
            }

            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, SelectQuery))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        CrAmount = string.IsNullOrEmpty(sdr["CR_AMT"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["CR_AMT"].ToString());
                        DrAmount = string.IsNullOrEmpty(sdr["DR_AMT"].ToString()) ? decimal.Parse("0") : decimal.Parse(sdr["DR_AMT"].ToString());
                        yield return new TrialBalanceExportModel
                        {
                            ACCOUNTHEAD = sdr["ACCOUNT_HEAD"].ToString(),
                            CRAMOUNT = CrAmount.ToString(),
                            DRAMOUNT = DrAmount.ToString()
                        };
                    }
                }
            }
        }
    }
}