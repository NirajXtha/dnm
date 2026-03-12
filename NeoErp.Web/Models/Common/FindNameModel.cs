using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using NeoErp.Models.Common;
using System.Data.OracleClient;

namespace NeoErp.Models.Common
{
    public class FindNameModel
    {
        public string value { get; set; }
        public string label { get; set; }
        public string flag { get; set; }
        //public string name_code { get; set; }
        //public string company_code { get; set; }
        //public string name_index { get; set; }
        //public string report_number { get; set; }

        public static IEnumerable<FindNameModel> GetSearchList(string str)
        {
            string SelectQuery = @"select * from (select 'Trial Balance (Pop Up View)' Name,
                                    'Trial Balance Report Pop Up View Balance Trial trial Blc Consolidated Account Finance Report Rpt' searchname,'PopUp' Code,'TrialBalance' Flag from dual
                                    union all
                                    select 'Trial Balance (New Tab View)', 'Trial Balance Report New Tab View Consolidated Account Finance Report Rpt Blance Blc Bance Sheet Balance Sheet Balance Shit','Index','TrialBalance' from dual
                                    union all
                                    select 'Trial Balance (Tree View)','Trial Balance Report Tree View Consolidate Account Finance Report Rpt Fin','Tree','TrialBalance' from dual
                                    union all
                                    select 'Balance Sheet ','Balance Sheet Report Consolidated Account Finance Report Blc Report Rpt','Index','Home' from dual
                                    union all
                                    select 'Profit & Loss','Profit & Loss Report Consolidated Account Finance Report Profit Loss PL Account','Index','Home' from dual
                                    union all
                                    select 'General Ledger','General Ledger Report Consolidated GL Ledger Gl Account Ledger Account Finance Report','Index','Home' from dual
                                    union all select name,name searchname, code,Flag " +
                                  "from V_FIND_NAME) where lower(searchname) like lower('%" + str + "%') and rowNum <= 100";
            using (OracleDataReader sdr = OracleHelper.ExecuteReader(OracleHelper.GetConnection(), CommandType.Text, SelectQuery))
            {
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    { 
                        yield return new FindNameModel
                        {
                            value = sdr["code"].ToString(),
                            label = sdr["name"].ToString(),
                            flag = sdr["flag"].ToString()
                        };
                    }
                }
            }
        }
    }
}