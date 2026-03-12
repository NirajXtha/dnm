using NeoErp.Core.Models;
using NeoErp.Sales.Modules.Services.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Services
{
    public class NewAgeingReport : INewAgeingReport
    {
        public dynamic ageingTransactions(TransactionRequestModel model, NeoErpCoreEntity dbContext)
        {

            Console.WriteLine(model);

            var slData = new List<Dictionary<string, object>>();

            var opening = new Dictionary<string, object>();

            List<double> drsArray = new List<double>();

            List<double> crsArray = new List<double>();

            Dictionary<string, float> ageingVals = new Dictionary<string, float>();

            string branchCode = model.BRANCH_CODE;
            string company_code = model.COMPANY_CODE;

            string formatedBranchCodes;
            if (branchCode.Contains("[") && branchCode.Contains("]"))
            {
                formatedBranchCodes = branchCode.Replace("[", "'").Replace("]", "'").Replace(", ", "','");
            }
            else
            {
                formatedBranchCodes = $"'{branchCode}'";
            }

            string formatedCompanyCodes;
            if (company_code.Contains("[") && company_code.Contains("]"))
            {
                formatedCompanyCodes = company_code.Replace("[", "'").Replace("]", "'").Replace(", ", "','");
            }
            else
            {
                formatedCompanyCodes = $"'{company_code}'";
            }

            Debug.WriteLine(formatedBranchCodes);
            var dateData = dbContext.SqlQuery<DateModel>($"select startdate as START_DATE, enddate as END_DATE from v_date_range where rangename='This Year'").ToList();


            //string ss = model.user_id;

            if (string.IsNullOrEmpty(model.from_date))
            {
                model.from_date = dateData[0].START_DATE.ToString("dd-MMM-yyyy");
            }
            if (string.IsNullOrEmpty(model.to_date))
            {
                model.to_date = dateData[0].END_DATE.ToString("dd-MMM-yyyy");
            }
            // First query to fetch transaction details
            string query1 = $@"
        SELECT voucher_date, voucher_no, dr_amount * exchange_rate AS dr_amount, 
               cr_amount * exchange_rate AS cr_amount, particulars, created_by, 
               currency_code, exchange_rate, BS_DATE(voucher_date) AS bs_date,
               remarks, manual_no
        FROM V$VIRTUAL_SUB_LEDGER
        WHERE sub_code = '{model.sub_code}'
          AND company_code in ({formatedCompanyCodes})
          AND branch_code in ({formatedBranchCodes})
          AND trunc(voucher_date) BETWEEN '{model.from_date}' and 
          '{model.to_date}'
          AND deleted_flag = 'N'
          AND form_code != 0
        ORDER BY voucher_date, voucher_no";



            string sConnStr1 = ConfigurationManager.ConnectionStrings["NeoErpCoreEntity"].ToString();
            string[] tokens = sConnStr1.Split('"');

            using (OracleConnection objConn = new OracleConnection(tokens[1]))
            {
                objConn.Open();

                using (OracleCommand cmd = new OracleCommand(query1, objConn))
                {
                    cmd.Parameters.Add(":SubCode", model.sub_code);
                    cmd.Parameters.Add(":CompanyCode", model.COMPANY_CODE);
                    cmd.Parameters.Add(":BranchCode", model.BRANCH_CODE);
                    cmd.Parameters.Add(":FromDate", model.from_date);
                    cmd.Parameters.Add(":ToDate", model.to_date);

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            double debitAmt = reader["dr_amount"] != DBNull.Value ? Convert.ToDouble(reader["dr_amount"]) : 0;
                            double creditAmt = reader["cr_amount"] != DBNull.Value ? Convert.ToDouble(reader["cr_amount"]) : 0;

                            drsArray.Add(debitAmt);
                            crsArray.Add(creditAmt);

                            var vals = new Dictionary<string, object>
                            {
                                ["voucher_date"] = Convert.ToDateTime(reader["voucher_date"]).ToString("dd-MMM-yyyy"),
                                ["voucher_no"] = reader["voucher_no"].ToString(),
                                ["dr_amount"] = debitAmt,
                                ["cr_amount"] = creditAmt,
                                ["particulars"] = reader["particulars"].ToString(),
                                ["created_by"] = reader["created_by"].ToString(),
                                ["currency_code"] = reader["currency_code"].ToString(),
                                ["exchange_rate"] = reader["exchange_rate"] != DBNull.Value ? Convert.ToDouble(reader["exchange_rate"]) : 0,
                                ["miti"] = reader["bs_date"].ToString(),
                                ["remarks"] = reader["remarks"].ToString(),
                                ["manual_no"] = reader["manual_no"].ToString()
                            };

                            slData.Add(vals);
                        }
                    }
                }
            }



            string query = $@"select CAST(NVL(sub, 0) AS NUMBER(20,2)) sub ,voucher_date,
                voucher_no,
                case when amm =1 then closing_balance else  pending_bal end as   pending_bal,
                age,
                closing_balance,
                amm
                from
                (SELECT sub,voucher_date,voucher_no, pending_bal,age, closing_balance,
                            ROW_NUMBER() OVER (PARTITION BY sub ORDER BY voucher_date,sub) amm  FROM
                             (
                             select * from
                            (select to_number(substr(sub_code,2,99)) sub ,voucher_date,voucher_no,
                            case when qw=1 then net_amount else dr_amount end as pending_bal ,
                            trunc(sysdate)-trunc(voucher_date) age,closing_balance from(select sub_code,voucher_no,voucher_date,dr_amount,cr_amount,qw,(dr_amount - nvl(cr_amount,0)) AS net_amount
                            ,SUM(dr_amount - nvl(cr_amount,0)) OVER (PARTITION BY to_number(substr(sub_code,2,99)) ORDER BY voucher_date,voucher_no,serial_no) AS closing_balance
                            from(select sub_code,voucher_no,voucher_date,dr_amount,qw,case when qw=1 then cr_amount else 0 end as cr_amount,serial_no
                            from(select sub_code,voucher_no,voucher_date,dr_amount,cr_amount, ROW_NUMBER() OVER (PARTITION BY to_number(substr(sub_code,2,9999)) ORDER BY to_number(substr(sub_code,2,9999)),voucher_date,voucher_no,serial_no) AS qw,serial_no
                            from (
                            select sub_code,voucher_no,voucher_date,dr_amount, SUM(cr_amount) OVER (PARTITION BY sub_code) cr_amount ,serial_no
                            from (
                            SELECT TO_CHAR(sub_code) sub_code, TO_CHAR(voucher_no) voucher_no, voucher_date, dr_amount, cr_amount ,serial_no
                            FROM V$VIRTUAL_SUB_LEDGER WHERE  
                            1=1 AND company_code in ({formatedCompanyCodes})  and sub_code in (select 'C'||customer_code from DIST_USER_AREAS  where user_id='{model.user_id}')
                            --AND branch_code in ({formatedBranchCodes})
                            and substr(sub_code,1,1)='C' 
                            AND VOUCHER_NO NOT IN(SELECT DISTINCT NVL(BOUNCE_VC_NO,'A000000') FROM FA_PDC_RECEIPTS WHERE (ENCASH_DATE IS NOT NULL OR BOUNCE_DATE IS NOT NULL) AND BOUNCE_FLAG='Y' )
                            and form_code not in (select distinct form_code from form_detail_setup where table_name='FA_PAY_ORDER' AND company_code in ({formatedCompanyCodes})) 
                            )
                            order by to_number(substr(sub_code,2,99)),voucher_date,voucher_no
                            ) where dr_amount !=0)) ) where closing_balance>0 )
                             ) )
                union all
                             select sub,voucher_date,
                voucher_no,
                case when amm =1 then closing_balance *-1 else  pending_bal *-1 end as   pending_bal,
                age,
                closing_balance,
                amm
                from
                (SELECT sub,voucher_date,voucher_no, pending_bal,age, closing_balance ,
                            ROW_NUMBER() OVER (PARTITION BY sub ORDER BY voucher_date,sub) amm  FROM
                             (
                             select * from
                            (select to_number(substr(sub_code,2,99)) sub ,voucher_date,voucher_no,
                            case when qw=1 then net_amount else dr_amount end as pending_bal ,
                            trunc(sysdate)-trunc(voucher_date) age,closing_balance from(select sub_code,voucher_no,voucher_date,dr_amount,cr_amount,qw,(dr_amount - nvl(cr_amount,0)) AS net_amount
                            ,SUM(dr_amount - nvl(cr_amount,0)) OVER (PARTITION BY to_number(substr(sub_code,2,99)) ORDER BY voucher_date,voucher_no) AS closing_balance
                            from(select sub_code,voucher_no,voucher_date,dr_amount,qw,case when qw=1 then cr_amount else 0 end as cr_amount
                            from(select sub_code,voucher_no,voucher_date,dr_amount,cr_amount, ROW_NUMBER() OVER (PARTITION BY to_number(substr(sub_code,2,9999)) ORDER BY to_number(substr(sub_code,2,9999)),voucher_date,voucher_no) AS qw
                            from (
                            select sub_code,voucher_no,voucher_date,dr_amount, SUM(cr_amount) OVER (PARTITION BY sub_code) cr_amount
                            from (
                            SELECT TO_CHAR(sub_code) sub_code, TO_CHAR(voucher_no) voucher_no, voucher_date,cr_amount dr_amount,dr_amount  cr_amount
                            FROM V$VIRTUAL_SUB_LEDGER WHERE  
                            1=1 AND company_code in ({formatedCompanyCodes})  and sub_code in (select 'C'||customer_code from DIST_USER_AREAS  where user_id='{model.user_id}')
                            --AND branch_code in ({formatedBranchCodes}) 
                            and substr(sub_code,1,1)='C'
                            AND VOUCHER_NO NOT IN(SELECT DISTINCT NVL(BOUNCE_VC_NO,'A000000') FROM FA_PDC_RECEIPTS WHERE (ENCASH_DATE IS NOT NULL OR BOUNCE_DATE IS NOT NULL) AND BOUNCE_FLAG='Y' )
                            and form_code not in (select distinct form_code from form_detail_setup where table_name='FA_PAY_ORDER' AND company_code in ({formatedCompanyCodes})) )
                            order by to_number(substr(sub_code,2,99)),voucher_date,voucher_no
                            ) where dr_amount !=0)) ) where closing_balance>0 )
                             ) ) ";



            List<FORAGEING> all_codes = new List<FORAGEING>();

            using (OracleConnection objConn = new OracleConnection(tokens[1]))
            {
                objConn.Open();
                using (OracleCommand objCmd = new OracleCommand(query, objConn))
                {
                    using (OracleDataReader reader = objCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = new FORAGEING
                            {
                                SUB = reader["SUB"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SUB"]),
                                VOUCHER_DATE = reader["VOUCHER_DATE"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["VOUCHER_DATE"]),
                                VOUCHER_NO = reader["VOUCHER_NO"] == DBNull.Value ? "" : reader["VOUCHER_NO"].ToString(),
                                PENDING_BAL = reader["PENDING_BAL"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["PENDING_BAL"]),
                                AGE = reader["AGE"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["AGE"]),
                                CLOSING_BALANCE = reader["CLOSING_BALANCE"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["CLOSING_BALANCE"]),
                                AMM = reader["AMM"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["AMM"])
                            };

                            all_codes.Add(item);
                        }
                    }
                }
            }


            int interval = 30;


            List<Dictionary<string, object>> first030 = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> third3160 = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> fourth6190 = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> sixth91120 = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> seventh120 = new List<Dictionary<string, object>>();

            List<decimal> allBal = new List<decimal>();
            Dictionary<int, Dictionary<string, object>> allData = new Dictionary<int, Dictionary<string, object>>();


            int? subCode = null;
            int x = 0;
            decimal bal = 0;
            string vNo = "";
            int days = 0;

            foreach (var i in all_codes)
            {
                // Assuming i.SUB, i.VOUCHER_DATE, i.VOUCHER_NO, i.PENDING_BAL, i.AGE
                if (i.VOUCHER_DATE != null)
                {
                    int currentSubCode = Convert.ToInt32(i.SUB);
                    //int currentSubCode = Convert.ToInt32(i.SUB);

                    if (x == 0)
                    {
                        subCode = currentSubCode;
                        allBal = new List<decimal>();
                    }
                    else
                    {
                        allBal.Add(bal);

                        var entry = new Dictionary<string, object>
                            {
                                { "amt", bal },
                                { "v_no", vNo },
                                { "days", days }
                            };

                        if (days <= interval)
                            first030.Add(entry);
                        else if (days <= interval * 2)
                            third3160.Add(entry);
                        else if (days <= interval * 3)
                            fourth6190.Add(entry);
                        else if (days <= interval * 4)
                            sixth91120.Add(entry);
                        else
                            seventh120.Add(entry);
                    }

                    if (subCode != currentSubCode)
                    {
                        allData[(int)subCode] = new Dictionary<string, object>
                            {
                                { "sub_code", subCode },
                                { "total", allBal.Sum() },
                                { "0-30", first030.Sum(e => Convert.ToDecimal(e["amt"])) },
                                { "0-31_detail", new List<Dictionary<string, object>>(first030) },
                                { "31-60", third3160.Sum(e => Convert.ToDecimal(e["amt"])) },
                                { "31-60_detail", new List<Dictionary<string, object>>(third3160) },
                                { "61-90", fourth6190.Sum(e => Convert.ToDecimal(e["amt"])) },
                                { "61-90_detail", new List<Dictionary<string, object>>(fourth6190) },
                                { "91-120", sixth91120.Sum(e => Convert.ToDecimal(e["amt"])) },
                                { "91-120_detail", new List<Dictionary<string, object>>(sixth91120) },
                                { "120+", seventh120.Sum(e => Convert.ToDecimal(e["amt"])) },
                                { "120+_detail", new List<Dictionary<string, object>>(seventh120) }
                            };
                        // Reset data
                        allBal.Clear();
                        first030.Clear();
                        third3160.Clear();
                        fourth6190.Clear();
                        sixth91120.Clear();
                        seventh120.Clear();
                    }

                    // Assign current values for next iteration
                    subCode = currentSubCode;
                    bal = Convert.ToDecimal(i.PENDING_BAL);
                    vNo = i.VOUCHER_NO;
                    days = Convert.ToInt32(i.AGE);
                    x++;
                }
            }


            // Add final balance after loop ends

            allBal.Add(bal);


            Console.WriteLine("step2");


            var lastEntry = new Dictionary<string, object>
                {
                    { "amt", bal },
                    { "v_no", vNo },
                    { "days", days }
                };

            if (days <= interval)
                first030.Add(lastEntry);
            else if (days <= interval * 2)
                third3160.Add(lastEntry);
            else if (days <= interval * 3)
                fourth6190.Add(lastEntry);
            else if (days <= interval * 4)
                sixth91120.Add(lastEntry);
            else
                seventh120.Add(lastEntry);

            if (subCode.HasValue)
            {
                allData[(int)subCode] = new Dictionary<string, object>
                    {
                        { "sub_code", subCode },
                        { "total", allBal.Sum() },
                        { "0-30", first030.Sum(e => Convert.ToDecimal(e["amt"])) },
                        { "0-31_detail", new List<Dictionary<string, object>>(first030) },
                        { "31-60", third3160.Sum(e => Convert.ToDecimal(e["amt"])) },
                        { "31-60_detail", new List<Dictionary<string, object>>(third3160) },
                        { "61-90", fourth6190.Sum(e => Convert.ToDecimal(e["amt"])) },
                        { "61-90_detail", new List<Dictionary<string, object>>(fourth6190) },
                        { "91-120", sixth91120.Sum(e => Convert.ToDecimal(e["amt"])) },
                        { "91-120_detail", new List<Dictionary<string, object>>(sixth91120) },
                        { "120+", seventh120.Sum(e => Convert.ToDecimal(e["amt"])) },
                        { "120+_detail", new List<Dictionary<string, object>>(seventh120) }
                    };
            }

            // Fetch customer details

            List<AgeingCustomerModel> customerList = new List<AgeingCustomerModel>();

            string custQuery = $@"
                SELECT 
                    CAST(NVL(customer_code, 0) AS NUMBER(20,2)) customer_code,
                    CUSTOMER_EDESC,
                    GROUP_SKU_FLAG,
                    MASTER_CUSTOMER_CODE,
                    PRE_CUSTOMER_CODE 
                FROM SA_CUSTOMER_SETUP 
                WHERE 1=1 and company_code in ({formatedCompanyCodes})  AND deleted_flag = 'N' 
                GROUP BY CUSTOMER_CODE, CUSTOMER_EDESC, GROUP_SKU_FLAG, MASTER_CUSTOMER_CODE, PRE_CUSTOMER_CODE 
                ORDER BY MASTER_CUSTOMER_CODE";

            using (OracleConnection objConn = new OracleConnection(tokens[1]))
            {
                objConn.Open();
                using (OracleCommand objCmd = new OracleCommand(custQuery, objConn))
                {
                    using (OracleDataReader reader = objCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = new AgeingCustomerModel
                            {
                                CUSTOMER_CODE = reader["CUSTOMER_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CUSTOMER_CODE"]),
                                CUSTOMER_EDESC = reader["CUSTOMER_EDESC"] == DBNull.Value ? "" : reader["CUSTOMER_EDESC"].ToString(),
                                GROUP_SKU_FLAG = reader["GROUP_SKU_FLAG"] == DBNull.Value ? "" : reader["GROUP_SKU_FLAG"].ToString(),
                                MASTER_CUSTOMER_CODE = reader["MASTER_CUSTOMER_CODE"] == DBNull.Value ? "" : reader["MASTER_CUSTOMER_CODE"].ToString(),
                                PRE_CUSTOMER_CODE = reader["PRE_CUSTOMER_CODE"] == DBNull.Value ? "" : reader["PRE_CUSTOMER_CODE"].ToString()
                            };

                            customerList.Add(item);
                        }
                    }
                }
            }



            var resultList = new List<Dictionary<string, object>>();

            foreach (var customer in customerList)
            {
                int customerCode = Convert.ToInt32(customer.CUSTOMER_CODE);
                var totalData = allData.ContainsKey(customerCode) ? allData[customerCode] : new Dictionary<string, object>();

                resultList.Add(new Dictionary<string, object>
                {
                    { "CUSTOMER_CODE", customer.CUSTOMER_CODE },
                    { "CUSTOMER_EDESC", customer.CUSTOMER_EDESC },
                    { "GROUP_SKU_FLAG", customer.GROUP_SKU_FLAG },
                    { "MASTER_CUSTOMER_CODE", customer.MASTER_CUSTOMER_CODE },
                    { "PRE_CUSTOMER_CODE", customer.PRE_CUSTOMER_CODE },
                    { "total", totalData.ContainsKey("total") ? totalData["total"] : 0 },
                    { "0-30", totalData.ContainsKey("0-30") ? totalData["0-30"] : 0 },
                    { "0-31_detail", totalData.ContainsKey("0-31_detail") ? totalData["0-31_detail"] : new List<Dictionary<string, object>>() },
                    { "31-60", totalData.ContainsKey("31-60") ? totalData["31-60"] : 0 },
                    { "31-60_detail", totalData.ContainsKey("31-60_detail") ? totalData["31-60_detail"] : new List<Dictionary<string, object>>() },
                    { "61-90", totalData.ContainsKey("61-90") ? totalData["61-90"] : 0 },
                    { "61-90_detail", totalData.ContainsKey("61-90_detail") ? totalData["61-90_detail"] : new List<Dictionary<string, object>>() },
                    { "91-120", totalData.ContainsKey("91-120") ? totalData["91-120"] : 0 },
                    { "91-120_detail", totalData.ContainsKey("91-120_detail") ? totalData["91-120_detail"] : new List<Dictionary<string, object>>() },
                    { "120+", totalData.ContainsKey("120+") ? totalData["120+"] : 0 },
                    { "120+_detail", totalData.ContainsKey("120+_detail") ? totalData["120+_detail"] : new List<Dictionary<string, object>>() },
                });
            }


            var valsqq = new List<Dictionary<string, object>>();

            decimal total1 = 0;

            var Arrtotal1 = new List<decimal>();
            var Arrtotal2 = new List<decimal>();
            var Arrtotal3 = new List<decimal>();
            var Arrtotal4 = new List<decimal>();
            var Arrtotal5 = new List<decimal>();
            var Arrtotal6 = new List<decimal>();

            Console.WriteLine("step4");


            foreach (var i in resultList)
            {
                string groupFlag = i["GROUP_SKU_FLAG"]?.ToString();
                string masterCode = i["MASTER_CUSTOMER_CODE"]?.ToString() ?? "";
                string preCode = i["PRE_CUSTOMER_CODE"]?.ToString() ?? "";

                if (groupFlag == "G")
                {
                    bool add_on = false;
                    total1 = 0;
                    decimal total2 = 0, total3 = 0, total4 = 0, total5 = 0, total6 = 0;

                    foreach (var z in resultList)
                    {
                        if (z["GROUP_SKU_FLAG"]?.ToString() == "I")
                        {
                            string zMaster = z["MASTER_CUSTOMER_CODE"]?.ToString() ?? "";
                            string match = "";

                            try
                            {
                                match = zMaster.Length >= masterCode.Length ? zMaster.Substring(0, masterCode.Length) : "";
                            }
                            catch { match = ""; }

                            if (masterCode != "" && match.Contains(masterCode))
                            {
                                add_on = true;

                                total1 += Convert.ToDecimal(z["total"]);
                                total2 += Convert.ToDecimal(z["0-30"]);
                                total3 += Convert.ToDecimal(z["31-60"]);
                                total4 += Convert.ToDecimal(z["61-90"]);
                                total5 += Convert.ToDecimal(z["91-120"]);
                                total6 += Convert.ToDecimal(z["120+"]);
                            }
                            else if (add_on)
                            {
                                break;
                            }
                        }
                    }

                    if (total1 > 0)
                    {
                        valsqq.Add(new Dictionary<string, object>
                {
                    { "item", i["CUSTOMER_EDESC"] },
                    { "group_flag", groupFlag },
                    { "total", total1 },
                    { "level", masterCode.Count(c => c == '.') },
                    { "master", masterCode },
                    { "pre", preCode },
                    { "code", i["CUSTOMER_CODE"] },
                    { "0-30", total2 },
                    { "31-60", total3 },
                    { "61-90", total4 },
                    { "91-120", total5 },
                    { "120+", total6 }
                });
                    }
                }

                if (groupFlag == "I" && Convert.ToDecimal(i["total"]) > 0)
                {
                    Arrtotal1.Add(Convert.ToDecimal(i["total"]));
                    Arrtotal2.Add(Convert.ToDecimal(i["0-30"]));
                    Arrtotal3.Add(Convert.ToDecimal(i["31-60"]));
                    Arrtotal4.Add(Convert.ToDecimal(i["61-90"]));
                    Arrtotal5.Add(Convert.ToDecimal(i["91-120"]));
                    Arrtotal6.Add(Convert.ToDecimal(i["120+"]));

                    valsqq.Add(new Dictionary<string, object>
            {
                { "item", i["CUSTOMER_EDESC"] },
                { "group_flag", groupFlag },
                { "total", Convert.ToDecimal(i["total"]) },
                { "level", masterCode.Count(c => c == '.') },
                { "master", masterCode },
                { "pre", preCode },
                { "code", i["CUSTOMER_CODE"] },
                { "0-30", Convert.ToDecimal(i["0-30"]) },
                { "31-60", Convert.ToDecimal(i["31-60"]) },
                { "61-90", Convert.ToDecimal(i["61-90"]) },
                { "91-120", Convert.ToDecimal(i["91-120"]) },
                { "120+", Convert.ToDecimal(i["120+"]) },
                { "0-30_detail", i["0-31_detail"] },
                { "31-60_detail", i["31-60_detail"] },
                { "61-90_detail", i["61-90_detail"] },
                { "91-120_detail", i["91-120_detail"] },
                { "120+_detail", i["120+_detail"] }
            });
                }
            }

            var response = new Dictionary<string, object>
            {
                ["detail"] = valsqq,
                ["total"] = new Dictionary<string, object>
                {
                    ["0-30"] = Arrtotal2.Sum(),
                    ["31-60"] = Arrtotal3.Sum(),
                    ["61-90"] = Arrtotal4.Sum(),
                    ["91-120"] = Arrtotal5.Sum(),
                    ["120+"] = Arrtotal6.Sum(),
                    ["total"] = Arrtotal1.Sum()
                }
            };

            return response;
            //return allData;
        }
    }
}
