using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.OracleClient;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using NeoErp.Models.Common;
using System.Data;

namespace NeoErp.Models
{
    public class UsersContext : DbContext
    {
        public UsersContext()
            : base("DefaultConnection")
        {
        }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        [Display(Name = "Input")]
        string Data { get; set; }
        public string btnLogin { get; set; }

        public string Branch { get; set; }

        public string Company { get; set; }

        public string User { get; set; }
        public string company_code { get; set; }
        public string company_name { get; set; }
        public string branch_code { get; set; }
        public string branch_name { get; set; }
        public string User_id
        {
            get;
            set;
        }
        public string GetVelidateUser(string UserName, string Password, string company, string branch)
        {
            string Result = "";
            string SelectQuery = @"Select user_no User_id,(select branch_code from fa_branch_setup where branch_code='" + branch + "') Branch,(select initcap(branch_edesc) branch_edesc from fa_branch_setup where branch_code='" + branch + "') BranchName,company_code as Company,(select initcap(company_edesc) company_edesc from company_setup) CompanyName from sc_application_users where Upper(login_code)=Upper('" + UserName + "') and password='" + Password + "'";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, SelectQuery))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        LoginModel l = new LoginModel();
                        l.Branch = sdr["Branch"].ToString();
                        l.Company = sdr["Company"].ToString();
                       branch_name = sdr["BranchName"].ToString();
                        company_name = sdr["CompanyName"].ToString();
                        Result = sdr["User_id"].ToString();
                    }
                }
            }
            return Result;
        }

        public static IEnumerable<LoginModel> GetCompanyList(string UserName)
        {
            //SC_COMPANY_CONTROL
            // string Query = "select company_code,company_name from company_master_setup where company_code in (select company_code from company_users where userid in (select userid from login_user where user_name='" + UserName + "'))";
            string SelectQuery = @"SELECT company_code, initcap(company_edesc) company_name from company_setup where company_code in(select company_code from sc_company_control where user_no in (select user_no from sc_application_users where Upper(login_code)=Upper('" + UserName + "')))";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, SelectQuery))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        yield return new LoginModel
                        {
                            company_code = sdr["company_code"].ToString(),
                            company_name = sdr["company_name"].ToString(),
                        };
                    }
                }
            }
        }

        public static IEnumerable<LoginModel> GetBranchList(string company_code)
        {
            string SelectQuery = @"SELECT branch_code, initcap(branch_edesc) branch_name from fa_branch_setup 
                                   where company_code='" + company_code + @"' and deleted_flag= 'N' and branch_code<>company_code";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, SelectQuery))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        yield return new LoginModel
                        {
                            branch_code = sdr["branch_code"].ToString(),
                            branch_name = sdr["branch_name"].ToString(),
                        };
                    }
                }
            }
        }

    }

    public class TrialBalanceModeltest
    {
        public string accountName { get; set; }
        public Decimal openingDrAmount { get; set; }
        public Decimal ClosingCrAmount { get; set; }
        public Decimal TransactionDrAmount { get; set; }
        public decimal transactionCrAmount { get; set; }
        public decimal transactionDrAmount { get; set; }
        public TrialBalanceModeltest subaccount { get; set; }
        public ledger ledger { get; set; }
        public TrialBalanceModeltest()
        {
           
            ledger = new ledger();
        }
    }
    public class subaccount
    {
        public string accountName { get; set; }
        public Decimal openingDrAmount { get; set; }
        public Decimal ClosingCrAmount { get; set; }
        public Decimal TransactionDrAmount { get; set; }
        public decimal transactionCrAmount { get; set; }
        public decimal transactionDrAmount { get; set; }
    }
    public class ledger
    {
        public string accountName { get; set; }
        public Decimal openingDrAmount { get; set; }
        public Decimal ClosingCrAmount { get; set; }
        public Decimal TransactionDrAmount { get; set; }
        public decimal transactionCrAmount { get; set; }
        public decimal transactionDrAmount { get; set; }
    }
}
