using NeoErp.Core.Caching;
using NeoErp.Data;
using NeoErp.Sales.Modules.Services.Models.AgeingReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoErp.Core.Domain;
using NeoErp.Core;

namespace NeoErp.Sales.Modules.Services.Services
{
    public class CustomerAgeingReportDataService:IAgeingReportDataService
    {
        private IDbContext _dbContext;
        private ICacheManager _cacheManager;
        private IWorkContext _workContext;
        public CustomerAgeingReportDataService(IDbContext dbContext, ICacheManager cacheManager, IWorkContext workContext)
        {
            this._dbContext = dbContext;
            this._cacheManager = cacheManager;
            this._workContext = workContext;
        }

        public IEnumerable<AgeingDataModel> GetAgeingDebitAmount(string asOnDate, List<string> masterCustomerIds)
        {
           return this.GetAgeingDebitAmount(asOnDate, masterCustomerIds, null);
        }

        public IEnumerable<AgeingDataModel> GetAgeingCreditAmount(string asOnDate, List<string> masterCustomerIds)
        {

            return this.GetAgeingCreditAmount(asOnDate, masterCustomerIds, null);

        }
        public IEnumerable<AgeingGroupData> GetGroupData()
        {
            return this.GetGroupData(null);
        }

        public IEnumerable<AgeingGroupData> GetGroupData(List<string> branchFilter)
        {
            
            var user = this._workContext.CurrentUserinformation;

            string query = string.Format(@"SELECT distinct  CUSTOMER_EDESC AS Description,MASTER_CUSTOMER_CODE as MasterCodeWithoutReplace,PRE_CUSTOMER_CODE as PreCodeWithoutReplace,
                                    TO_NUMBER(REPLACE(MASTER_CUSTOMER_CODE,'.','')) as MasterCode, TO_NUMBER(CUSTOMER_CODE) as Code, TO_NUMBER(REPLACE(PRE_CUSTOMER_CODE,'.','')) as PreCode 
                                    FROM SA_CUSTOMER_SETUP WHERE GROUP_SKU_FLAG = 'G' AND DELETED_FLAG = 'N'");
          
            if(branchFilter != null && branchFilter.Count > 0)
            {
                query += string.Format(" AND COMPANY_CODE IN('{0}') ", string.Join("','", branchFilter) );
            }

            return _dbContext.SqlQuery<AgeingGroupData>(query).OrderByDescending(q => q.PreCode);

        }
        public IEnumerable<AgeingDataModel> GetAgeingDebitAmount(string asOnDate, List<string> masterCustomerIds, List<string> branchFilter,string billwise=null) {
            var user = this._workContext.CurrentUserinformation;

            if (string.IsNullOrEmpty(user.Company))
            {
                user.Company = "01";
                user.Branch = "01.01";
            }

            else if (string.IsNullOrEmpty(user.Branch))
            {
                user.Company = "01";
                user.Branch = "01.01";
            }
            var BranchCode = string.Empty;
            foreach (var company in branchFilter)
            {
                BranchCode += $@"'{company}',";
            }
            
            BranchCode = BranchCode == "" ? $@"'{user.Branch}'" : BranchCode.Remove(BranchCode.Length - 1);
            var tablename = "V$VIRTUAL_SUB_LEDGER";
            if (string.IsNullOrEmpty(billwise))
                tablename = "V$VIRTUAL_SUB_LEDGER";
            tablename = billwise.ToLower() == "LedgerWise".ToLower() ? tablename : "V$CUSTOMER_BILLAGE_LEDGER";
            var Startdate = string.Empty;
            //try
            //{
            //    Startdate = System.Configuration.ConfigurationManager.AppSettings["FiscalYearStartDate"].ToString();
            //}
            //catch(Exception ex)
            //{

            //}
            //if (!string.IsNullOrEmpty(Startdate))
            //    Startdate = $"AND TO_DATE(TO_CHAR(VSL.VOUCHER_DATE, 'DD-MON-YYYY')) >= TO_DATE('{Startdate}', 'YYYY-MM-DD')";
            // billwiseafModel.BillWiseOrLedgerWise.ToLower() == "LedgerWise".ToLower()
            string query = string.Format($@"SELECT VSL.SUB_CODE as SubCode, VSL.ACC_CODE as AccCode, INITCAP(VSL.SUB_EDESC) Description, VSL.VOUCHER_DATE as VoucherDate, NVL(SUM(FN_CONVERT_CURRENCY(NVL(DR_AMOUNT,0) * NVL(EXCHANGE_RATE,1),'NRS', VOUCHER_DATE)),0)  AMOUNT,TO_NUMBER(CS.CUSTOMER_CODE) as Code
                        ,TO_NUMBER(REPLACE(CS.MASTER_CUSTOMER_CODE,'.','')) as MasterCode,TO_NUMBER(REPLACE(CS.PRE_CUSTOMER_CODE,'.',''))  as PreCode
                        FROM {tablename} VSL, SA_CUSTOMER_SETUP CS
                        WHERE VSL.DELETED_FLAG='N'
                     -- AND VSL.Company_Code='{user.Company}'
                        AND VSL.SUB_LEDGER_FLAG='C'
                        AND VSL.COMPANY_CODE=CS.COMPANY_CODE
                        AND VOUCHER_NO NOT  IN(SELECT DISTINCT NVL(VOUCHER_NO,'A000000') FROM FA_PDC_RECEIPTS WHERE CUSTOMER_CODE IN(SELECT CUSTOMER_CODE FROM SA_CUSTOMER_SETUP) AND (ENCASH_DATE IS NOT NULL OR BOUNCE_DATE IS NOT NULL) AND BOUNCE_FLAG='Y')
                        AND TRIM( VSL.SUB_CODE) = TRIM(CS.LINK_SUB_CODE)
                        AND VSL.DR_AMOUNT > 0   
                        AND VSL.POSTED_BY IS NOT NULL
                         {Startdate}
                        AND TO_DATE(TO_CHAR(VSL.VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{asOnDate}', 'YYYY-MM-DD')");


            if(branchFilter != null && branchFilter.Count > 0 )
            {
                query += string.Format(@" AND VSL.BRANCH_CODE IN ({0})", BranchCode);
            }
            var customerFilter = string.Empty;
            if (masterCustomerIds != null && masterCustomerIds.Count > 0)
            {
                customerFilter = @"select  TRIM(LINK_SUB_CODE) from sa_customer_setup where (";
                //IF CUSTOMER_SKU_FLAG = G
                foreach (var item in masterCustomerIds)
                {
                    customerFilter += "master_customer_code like  (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN('" + user.Company + "')) OR ";
                }
                customerFilter = customerFilter.Substring(0, customerFilter.Length - 3);
                var CustomerFilterCode = string.Empty;
                foreach (var company in masterCustomerIds)
                {
                    CustomerFilterCode += $@"'{company}',";
                }
                CustomerFilterCode = CustomerFilterCode == "" ? $@"" : CustomerFilterCode.Remove(CustomerFilterCode.Length - 1);

                //IF CUSTOMER_SKU_FLAG = I                
                customerFilter += $" or (customer_code in ({CustomerFilterCode}) and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN('" + user.Company + "'))) ";


                query += " AND TRIM(VSL.SUB_CODE) IN(" + customerFilter + ")";
            }
            //if (masterCustomerIds != null && masterCustomerIds.Count > 0)
            //{
            //    string subQuery = @" AND TRIM(VSL.SUB_CODE) IN(SELECT TRIM(LINK_SUB_CODE)FROM SA_CUSTOMER_SETUP WHERE";
            //    var count = 1;
            //    foreach (var subQueryItem in masterCustomerIds)
            //    {


            //        if (count > 1)
            //        {
            //            subQuery += string.Format(@" OR MASTER_CUSTOMER_CODE LIKE '{0}%'", subQueryItem);
            //        }
            //        else
            //        {
            //            subQuery += string.Format(@" MASTER_CUSTOMER_CODE LIKE '{0}%'", subQueryItem);
            //        }
            //        count++;
            //    }
            //    subQuery += @" AND DELETED_FLAG = 'N'
            //            AND GROUP_SKU_FLAG = 'I')";

            //    query += subQuery;
            //}
            query += @" GROUP BY VSL.SUB_CODE,VSL.ACC_CODE, INITCAP(VSL.SUB_EDESC), VSL.VOUCHER_DATE ,CS.MASTER_CUSTOMER_CODE, CS.PRE_CUSTOMER_CODE, CS.CUSTOMER_CODE
                        ORDER BY INITCAP(VSL.SUB_EDESC), VSL.VOUCHER_DATE";

            var data = this._dbContext.SqlQuery<AgeingDataModel>(query).ToList();
            return data;

        }
        public IEnumerable<AgeingDataModel> GetAgeingCreditAmount(string asOnDate, List<string> masterCustomerIds, List<string> branchFilter, string billwise = null)
        {
            
            var user = this._workContext.CurrentUserinformation;

            if (string.IsNullOrEmpty(user.Company))
            {
                user.Company = "01";
                user.Branch = "01.01";
            }
            else if (string.IsNullOrEmpty(user.Branch))
            {
                user.Company = "01";
                user.Branch = "01.01";
            }
            var BranchCode = string.Empty;
            foreach (var company in branchFilter)
            {
                BranchCode += $@"'{company}',";
            }

            BranchCode = BranchCode == "" ? $@"'{user.Branch}'" : BranchCode.Remove(BranchCode.Length - 1);
            var tablename = "V$VIRTUAL_SUB_LEDGER";
            if (string.IsNullOrEmpty(billwise))
                tablename = "V$VIRTUAL_SUB_LEDGER";
            tablename = billwise.ToLower() == "LedgerWise".ToLower() ? tablename : "V$CUSTOMER_BILLAGE_LEDGER";
            var Startdate = string.Empty;
            //try
            //{
            //    Startdate = System.Configuration.ConfigurationManager.AppSettings["FiscalYearStartDate"].ToString();
            //}
            //catch (Exception ex)
            //{

            //}
            //if (!string.IsNullOrEmpty(Startdate))
            //    Startdate = $"AND TO_DATE(TO_CHAR(VSL.VOUCHER_DATE, 'DD-MON-YYYY')) >= TO_DATE('{Startdate}', 'YYYY-MM-DD')";
            string query = string.Format($@"SELECT VSL.SUB_CODE as SubCode,VSL.ACC_CODE as AccCode, INITCAP(VSL.SUB_EDESC) Description, VSL.VOUCHER_DATE as VoucherDate, NVL(SUM(FN_CONVERT_CURRENCY(NVL(CR_AMOUNT,0) * NVL(EXCHANGE_RATE,1),'NRS', VOUCHER_DATE)),0)  AMOUNT, TO_NUMBER( CS.CUSTOMER_CODE) as Code
                    ,TO_NUMBER(REPLACE(CS.MASTER_CUSTOMER_CODE,'.','')) as MasterCode,TO_NUMBER(REPLACE(CS.PRE_CUSTOMER_CODE,'.',''))  as PreCode
                    FROM {tablename} VSL, SA_CUSTOMER_SETUP CS
                    WHERE VSL.DELETED_FLAG='N' 
                   -- AND VSL.Company_Code='{user.Company}'
                    AND VSL.SUB_LEDGER_FLAG='C'
                     AND VSL.COMPANY_CODE=CS.COMPANY_CODE
AND VOUCHER_NO NOT  IN(SELECT DISTINCT NVL(VOUCHER_NO,'A000000') FROM FA_PDC_RECEIPTS WHERE CUSTOMER_CODE IN(SELECT CUSTOMER_CODE FROM SA_CUSTOMER_SETUP) AND (ENCASH_DATE IS NOT NULL OR BOUNCE_DATE IS NOT NULL) AND BOUNCE_FLAG='Y')
                    AND TRIM( VSL.SUB_CODE) = TRIM(CS.LINK_SUB_CODE)
                    AND VSL.CR_AMOUNT > 0   
                    AND VSL.POSTED_BY IS NOT NULL
                      {Startdate}
                    AND TO_DATE(TO_CHAR(VSL.VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{asOnDate}', 'YYYY-MM-DD')");


            if (branchFilter != null && branchFilter.Count > 0)
            {
                query += string.Format(@" AND VSL.BRANCH_CODE IN ({0})", BranchCode);
            }

            //if (masterCustomerIds != null && masterCustomerIds.Count > 0)
            //{
            //    string subQuery = @" AND TRIM(VSL.SUB_CODE) IN(SELECT TRIM(LINK_SUB_CODE)FROM SA_CUSTOMER_SETUP WHERE";
            //    var count = 1;
            //    foreach (var subQueryItem in masterCustomerIds)
            //    {

            //        if (count > 1)
            //        {
            //            subQuery += string.Format(@" OR MASTER_CUSTOMER_CODE LIKE '{0}%'", subQueryItem);
            //        }
            //        else
            //        {
            //            subQuery += string.Format(@" MASTER_CUSTOMER_CODE LIKE '{0}%'", subQueryItem);

            //        }
            //        count++;
            //    }
            //    subQuery += @" AND DELETED_FLAG = 'N'
            //            AND GROUP_SKU_FLAG = 'I')";

            //    query += subQuery;
            //}
            var customerFilter = string.Empty;
            if (masterCustomerIds != null && masterCustomerIds.Count > 0)
            {
                customerFilter = @"select  TRIM(LINK_SUB_CODE) from sa_customer_setup where (";
                //IF CUSTOMER_SKU_FLAG = G
                foreach (var item in masterCustomerIds)
                {
                    customerFilter += "master_customer_code like  (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + user.Company + ")) OR ";
                }
                customerFilter = customerFilter.Substring(0, customerFilter.Length - 3);
                //IF CUSTOMER_SKU_FLAG = I                
                var CustomerFilterCode = string.Empty;
                foreach (var company in masterCustomerIds)
                {
                    CustomerFilterCode += $@"'{company}',";
                }
                CustomerFilterCode = CustomerFilterCode == "" ? $@"" : CustomerFilterCode.Remove(CustomerFilterCode.Length - 1);

                //IF CUSTOMER_SKU_FLAG = I                
                customerFilter += $" or (customer_code in ({CustomerFilterCode}) and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN('" + user.Company + "'))) ";



                query += " AND TRIM(VSL.SUB_CODE) IN(" + customerFilter + ")";
            }
            query += @"GROUP BY VSL.SUB_CODE, VSL.ACC_CODE, INITCAP(VSL.SUB_EDESC), VSL.VOUCHER_DATE ,CS.MASTER_CUSTOMER_CODE, CS.PRE_CUSTOMER_CODE,CS.CUSTOMER_CODE
                    ORDER BY INITCAP(VSL.SUB_EDESC), VSL.VOUCHER_DATE";
            var data = this._dbContext.SqlQuery<AgeingDataModel>(query).ToList();
            return data;
        }

        public IEnumerable<AgeingDataModel> GetAgeingChartDebitAmount(string asOnDate, List<string> masterCustomerIds, List<string> branchFilter)
        {
            var user = this._workContext.CurrentUserinformation;
            var subQuery = string.Empty;
            var tablename = "V$VIRTUAL_SUB_LEDGER";
            if (masterCustomerIds.Count > 0)
                tablename = "v$virtual_sub_dealer_ledger";
            if (string.IsNullOrEmpty(user.Company))
            {
                user.Company = "01";
                user.Branch = "01.01";
            }
            else if (string.IsNullOrEmpty(user.Branch))
            {
                user.Company = "01";
                user.Branch = "01.01";
            }
            if(tablename== "v$virtual_sub_dealer_ledger")
            {
                string query1 = $@" SELECT VL.VOUCHER_DATE VoucherDate, SUM(NVL(VL.DR_AMOUNT,0)) Amount 
FROM v$virtual_sub_dealer_ledger VL,IP_PARTY_TYPE_CODE IP
WHERE VL.DELETED_FLAG='N' 
AND IP.COMPANY_CODE=VL.COMPANY_CODE
AND VL.PARTY_TYPE_CODE=IP.PARTY_TYPE_CODE
AND VL.FORM_CODE<>0
AND VL.POSTED_BY IS NOT NULL 
and IP.AREA_CODE in ('{string.Join("','", masterCustomerIds)}')
GROUP BY  VL.VOUCHER_DATE 
ORDER BY  VL.VOUCHER_DATE";
                var data1 = this._dbContext.SqlQuery<AgeingDataModel>(query1).ToList();
                return data1;
            }
            string query = @"SELECT Voucher_Date VoucherDate,
         NVL (
            SUM (
               FN_CONVERT_CURRENCY (
                  NVL (DR_AMOUNT, 0) * NVL (EXCHANGE_RATE, 1),
                  'NRS',
                  VOUCHER_DATE)),
            0)
            Amount
    FROM V$VIRTUAL_SUB_LEDGER
   WHERE     DELETED_FLAG = 'N'
         AND SUB_LEDGER_FLAG = 'C'
         AND DR_AMOUNT > 0
         AND POSTED_BY IS NOT NULL
       {2} 
                       AND TO_DATE (TO_CHAR (VOUCHER_DATE, 'DD-MON-YYYY')) <=
                TO_DATE ('{0}', 'YYYY-MM-DD'){1}
         AND VOUCHER_NO NOT IN
                (SELECT DISTINCT NVL (VOUCHER_NO, 000000)
                   FROM FA_PDC_RECEIPTS 
                       where  (ENCASH_DATE IS NOT NULL OR BOUNCE_DATE IS NOT NULL)
                        AND BOUNCE_FLAG = 'Y')
GROUP BY Voucher_Date order by voucher_date desc";

            var branchQuery = "";
            if (branchFilter != null && branchFilter.Count > 0)
            {
                 branchQuery= string.Format(@" AND BRANCH_CODE IN ('{0}')", string.Join("','", branchFilter));
            }

            if (masterCustomerIds != null && masterCustomerIds.Count > 0)
            {
                
                var count = 1;
                foreach (var subQueryItem in masterCustomerIds)
                {
                    if (count >= 1)
                    {
                        subQuery += string.Format(@" and(trim(SUB_CODE) in (SELECT TRIM(LINK_SUB_CODE) FROM SA_CUSTOMER_SETUP WHERE 
 COMPANY_CODE='01' and master_customer_code like (select master_customer_code from SA_CUSTOMER_SETUP  where company_code='01' and deleted_flag='N' and customer_code='" + subQueryItem+"')||'%' )");
                    }
                    else
                    {
                        subQuery += string.Format(@" or trim(SUB_CODE) in (SELECT TRIM(LINK_SUB_CODE) FROM SA_CUSTOMER_SETUP WHERE 
 COMPANY_CODE='01' and master_customer_code like (select master_customer_code from SA_CUSTOMER_SETUP  where company_code='01' and deleted_flag='N' and customer_code='" + subQueryItem + "')||'%' )");
                    }
                    
                    count++;
                }
                if (masterCustomerIds.Count > 0)
                    subQuery = subQuery + ")";


            }
            var finalQuery = string.Format(query, asOnDate, subQuery,branchQuery);
            var data = this._dbContext.SqlQuery<AgeingDataModel>(finalQuery).ToList();
            return data;

        }
        public IEnumerable<AgeingDataModel> GetAgeingChartCreditAmount(string asOnDate, List<string> masterCustomerIds, List<string> branchFilter)
        {

           var user = this._workContext.CurrentUserinformation;
            var subQuery = string.Empty;
            if (string.IsNullOrEmpty(user.Company))
            {
                user.Company = "01";
                user.Branch = "01.01";
            }
            else if (string.IsNullOrEmpty(user.Branch))
            {
                user.Company = "01";
                user.Branch = "01.01";
            }
            var tablename = "V$VIRTUAL_SUB_LEDGER";
            if (masterCustomerIds.Count > 0)
                tablename = "v$virtual_sub_dealer_ledger";
            if (string.IsNullOrEmpty(user.Company))
            {
                user.Company = "01";
                user.Branch = "01.01";
            }
            else if (string.IsNullOrEmpty(user.Branch))
            {
                user.Company = "01";
                user.Branch = "01.01";
            }
            if (tablename == "v$virtual_sub_dealer_ledger")
            {
                string query1 = $@" SELECT VL.VOUCHER_DATE VoucherDate, SUM(NVL(VL.CR_AMOUNT,0)) Amount 
FROM v$virtual_sub_dealer_ledger VL,IP_PARTY_TYPE_CODE IP
WHERE VL.DELETED_FLAG='N' 
AND IP.COMPANY_CODE=VL.COMPANY_CODE
AND VL.PARTY_TYPE_CODE=IP.PARTY_TYPE_CODE
AND VL.FORM_CODE<>0
AND VL.POSTED_BY IS NOT NULL 
and IP.AREA_CODE in ('{string.Join("','", masterCustomerIds)}')
GROUP BY  VL.VOUCHER_DATE 
ORDER BY  VL.VOUCHER_DATE";
                var data1 = this._dbContext.SqlQuery<AgeingDataModel>(query1).ToList();
                return data1;
            }
            string query = @"SELECT Voucher_Date VoucherDate,
         NVL (
            SUM (
               FN_CONVERT_CURRENCY (
                  NVL (CR_AMOUNT, 0) * NVL (EXCHANGE_RATE, 1),
                  'NRS',
                  VOUCHER_DATE)),
            0)
            Amount
    FROM V$VIRTUAL_SUB_LEDGER
   WHERE     DELETED_FLAG = 'N'
         AND SUB_LEDGER_FLAG = 'C'
         AND CR_AMOUNT > 0
         AND POSTED_BY IS NOT NULL
       {2} 
                       AND TO_DATE (TO_CHAR (VOUCHER_DATE, 'DD-MON-YYYY')) <=
                TO_DATE ('{0}', 'YYYY-MM-DD'){1}
         AND VOUCHER_NO NOT IN
                (SELECT DISTINCT NVL (VOUCHER_NO, 000000)
                   FROM FA_PDC_RECEIPTS 
                       where  (ENCASH_DATE IS NOT NULL OR BOUNCE_DATE IS NOT NULL)
                        AND BOUNCE_FLAG = 'Y')
GROUP BY Voucher_Date";

            var branchQuery = "";
            if (branchFilter != null && branchFilter.Count > 0)
            {
                branchQuery = string.Format(@" AND BRANCH_CODE IN ('{0}')", string.Join("','", branchFilter));
            }

            if (masterCustomerIds != null && masterCustomerIds.Count > 0)
            {

                var count = 1;
                foreach (var subQueryItem in masterCustomerIds)
                {
                    if (count >= 1)
                    {
                        subQuery += string.Format(@" and (trim(SUB_CODE) in (SELECT TRIM(LINK_SUB_CODE) FROM SA_CUSTOMER_SETUP WHERE 
 COMPANY_CODE='01' and master_customer_code like (select master_customer_code from SA_CUSTOMER_SETUP  where company_code='01' and deleted_flag='N' and customer_code='" + subQueryItem + "')||'%' )");
                    }
                    else
                    {
                        subQuery += string.Format(@" or trim(SUB_CODE) in (SELECT TRIM(LINK_SUB_CODE) FROM SA_CUSTOMER_SETUP WHERE 
 COMPANY_CODE='01' and master_customer_code like (select master_customer_code from SA_CUSTOMER_SETUP  where company_code='01' and deleted_flag='N' and customer_code='" + subQueryItem + "')||'%' )");
                    }
                    count++;
                }
                if (masterCustomerIds.Count > 0)
                    subQuery = subQuery + ")";

            }
            var finalQuery = string.Format(query, asOnDate, subQuery,branchQuery);
            var data = this._dbContext.SqlQuery<AgeingDataModel>(finalQuery).ToList();

            return data;
        }

        public IEnumerable<AgeingDataModel> GetAgeingBillWiseChartDebitAmount(string asOnDate, List<string> masterCustomerIds, List<string> branchFilter)
        {
            var user = this._workContext.CurrentUserinformation;
            var subQuery = string.Empty;
            if (string.IsNullOrEmpty(user.Company))
            {
                user.Company = "01";
                user.Branch = "01.01";
            }
            else if (string.IsNullOrEmpty(user.Branch))
            {
                user.Company = "01";
                user.Branch = "01.01";
            }
            string query = @"SELECT VOUCHER_DATE VoucherDate, SUM(NVL(DR_AMOUNT,0)) Amount 
FROM V$CUSTOMER_BILLAGE_LEDGER 
WHERE DELETED_FLAG='N' 
AND SUB_LEDGER_FLAG='C' 
AND DR_AMOUNT > 0  
AND POSTED_BY IS NOT NULL 
{2} 
 AND TO_DATE(TO_CHAR(VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{0}', 'YYYY-MM-DD') {1} 
GROUP BY  VOUCHER_DATE 
ORDER BY  VOUCHER_DATE";


            var branchQuery = "";
            if (branchFilter != null && branchFilter.Count > 0)
            {
                branchQuery = string.Format(@" AND BRANCH_CODE IN ('{0}')", string.Join("','", branchFilter));
            }
            if (masterCustomerIds != null && masterCustomerIds.Count > 0)
            {

                var count = 1;
                foreach (var subQueryItem in masterCustomerIds)
                {
                    if (count >= 1)
                    {
                        subQuery += string.Format(@" and(trim(SUB_CODE) in (SELECT TRIM(LINK_SUB_CODE) FROM SA_CUSTOMER_SETUP WHERE 
 COMPANY_CODE='01' and master_customer_code like (select master_customer_code from SA_CUSTOMER_SETUP  where company_code='01' and deleted_flag='N' and customer_code='" + subQueryItem + "')||'%' )");
                    }
                    else
                    {
                        subQuery += string.Format(@" or trim(SUB_CODE) in (SELECT TRIM(LINK_SUB_CODE) FROM SA_CUSTOMER_SETUP WHERE 
 COMPANY_CODE='01' and master_customer_code like (select master_customer_code from SA_CUSTOMER_SETUP  where company_code='01' and deleted_flag='N' and customer_code='" + subQueryItem + "')||'%' )");
                    }

                    count++;
                }
                if (masterCustomerIds.Count > 0)
                    subQuery = subQuery + ")";


            }
            var finalQuery = string.Format(query, asOnDate, subQuery,branchQuery);
            var data = this._dbContext.SqlQuery<AgeingDataModel>(finalQuery).ToList();
            return data;

        }
        public IEnumerable<AgeingDataModel> GetAgeingBillWiseChartCreditAmount(string asOnDate, List<string> masterCustomerIds, List<string> branchFilter)
        {

            var user = this._workContext.CurrentUserinformation;
            var subQuery = string.Empty;
            if (string.IsNullOrEmpty(user.Company))
            {
                user.Company = "01";
                user.Branch = "01.01";
            }
            else if (string.IsNullOrEmpty(user.Branch))
            {
                user.Company = "01";
                user.Branch = "01.01";
            }
            string query = @"SELECT VOUCHER_DATE VoucherDate, SUM(NVL(CR_AMOUNT,0)) Amount 
FROM V$CUSTOMER_BILLAGE_LEDGER 
WHERE DELETED_FLAG='N' 
AND SUB_LEDGER_FLAG='C' 
AND CR_AMOUNT > 0  
AND POSTED_BY IS NOT NULL 
{2} 
 AND TO_DATE(TO_CHAR(VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{0}', 'YYYY-MM-DD') {1}
GROUP BY  VOUCHER_DATE 
ORDER BY  VOUCHER_DATE";

            var branchQuery = "";
            if (branchFilter != null && branchFilter.Count > 0)
            {
                branchQuery = string.Format(@" AND BRANCH_CODE IN ('{0}')", string.Join("','", branchFilter));
            }

            if (masterCustomerIds != null && masterCustomerIds.Count > 0)
            {

                var count = 1;
                foreach (var subQueryItem in masterCustomerIds)
                {
                    if (count >= 1)
                    {
                        subQuery += string.Format(@" and (trim(SUB_CODE) in (SELECT TRIM(LINK_SUB_CODE) FROM SA_CUSTOMER_SETUP WHERE 
 COMPANY_CODE='01' and master_customer_code like (select master_customer_code from SA_CUSTOMER_SETUP  where company_code='01' and deleted_flag='N' and customer_code='" + subQueryItem + "')||'%' )");
                    }
                    else
                    {
                        subQuery += string.Format(@" or trim(SUB_CODE) in (SELECT TRIM(LINK_SUB_CODE) FROM SA_CUSTOMER_SETUP WHERE 
 COMPANY_CODE='01' and master_customer_code like (select master_customer_code from SA_CUSTOMER_SETUP  where company_code='01' and deleted_flag='N' and customer_code='" + subQueryItem + "')||'%' )");
                    }
                    count++;
                }
                if (masterCustomerIds.Count > 0)
                    subQuery = subQuery + ")";

            }
            var finalQuery = string.Format(query, asOnDate, subQuery,branchQuery);
            var data = this._dbContext.SqlQuery<AgeingDataModel>(finalQuery).ToList();
            return data;
        }

        public IEnumerable<AgeingDataModel> GetAgeingDivisionChartDebitAmount(string asOnDate, List<string> masterCustomerIds, List<string> branchFilter)
        {
            var user = this._workContext.CurrentUserinformation;
            var subQuery = string.Empty;
            if (string.IsNullOrEmpty(user.Company))
            {
                user.Company = "01";
                user.Branch = "01.01";
            }
            else if (string.IsNullOrEmpty(user.Branch))
            {
                user.Company = "01";
                user.Branch = "01.01";
            }
            string query = @"SELECT VOUCHER_DATE VoucherDate, SUM(NVL(DR_AMOUNT,0)) Amount 
FROM V$VIRTUAL_SUB_LEDGER 
WHERE DELETED_FLAG='N' 
AND SUB_LEDGER_FLAG='C' 
AND DR_AMOUNT > 0  
AND POSTED_BY IS NOT NULL 
{2}  
 AND TO_DATE(TO_CHAR(VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{0}', 'YYYY-MM-DD') {1} 
GROUP BY  VOUCHER_DATE 
ORDER BY  VOUCHER_DATE";

            var branchQuery = "";
            if (branchFilter != null && branchFilter.Count > 0)
            {
                branchQuery = string.Format(@" AND BRANCH_CODE IN ('{0}')", string.Join("','", branchFilter));
            }

            if (masterCustomerIds != null && masterCustomerIds.Count > 0)
            {

                var count = 1;
                foreach (var subQueryItem in masterCustomerIds)
                {
                    if (count >= 1)
                    {
                        subQuery += string.Format(@" and(trim(SUB_CODE) in (SELECT TRIM(LINK_SUB_CODE) FROM SA_CUSTOMER_SETUP WHERE 
 COMPANY_CODE='01' and master_customer_code like (select master_customer_code from SA_CUSTOMER_SETUP  where company_code='01' and deleted_flag='N' and customer_code='" + subQueryItem + "')||'%' )");
                    }
                    else
                    {
                        subQuery += string.Format(@" or trim(SUB_CODE) in (SELECT TRIM(LINK_SUB_CODE) FROM SA_CUSTOMER_SETUP WHERE 
 COMPANY_CODE='01' and master_customer_code like (select master_customer_code from SA_CUSTOMER_SETUP  where company_code='01' and deleted_flag='N' and customer_code='" + subQueryItem + "')||'%' )");
                    }

                    count++;
                }
                if (masterCustomerIds.Count > 0)
                    subQuery = subQuery + ")";


            }
            var finalQuery = string.Format(query, asOnDate, subQuery, branchQuery);
            var data = this._dbContext.SqlQuery<AgeingDataModel>(finalQuery).ToList();
            return data;

        }
        public IEnumerable<AgeingDataModel> GetAgeingDivisionChartCreditAmount(string asOnDate, List<string> masterCustomerIds, List<string> branchFilter)
        {

            var user = this._workContext.CurrentUserinformation;
            var subQuery = string.Empty;
            if (string.IsNullOrEmpty(user.Company))
            {
                user.Company = "01";
                user.Branch = "01.01";
            }
            else if (string.IsNullOrEmpty(user.Branch))
            {
                user.Company = "01";
                user.Branch = "01.01";
            }
            string query = @"SELECT VOUCHER_DATE VoucherDate, SUM(NVL(CR_AMOUNT,0)) Amount 
FROM V$VIRTUAL_SUB_LEDGER 
WHERE DELETED_FLAG='N' 
AND SUB_LEDGER_FLAG='C' 
AND CR_AMOUNT > 0  
AND POSTED_BY IS NOT NULL 
{2}  
 AND TO_DATE(TO_CHAR(VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{0}', 'YYYY-MM-DD') {1}
GROUP BY  VOUCHER_DATE 
ORDER BY  VOUCHER_DATE";

            var branchQuery = "";
            if (branchFilter != null && branchFilter.Count > 0)
            {
                branchQuery = string.Format(@" AND BRANCH_CODE IN ('{0}')", string.Join("','", branchFilter));
            }

            if (masterCustomerIds != null && masterCustomerIds.Count > 0)
            {

                var count = 1;
                foreach (var subQueryItem in masterCustomerIds)
                {
                    if (count >= 1)
                    {
                        subQuery += string.Format(@" and (trim(SUB_CODE) in (SELECT TRIM(LINK_SUB_CODE) FROM SA_CUSTOMER_SETUP WHERE 
 COMPANY_CODE='01' and master_customer_code like (select master_customer_code from SA_CUSTOMER_SETUP  where company_code='01' and deleted_flag='N' and customer_code='" + subQueryItem + "')||'%' )");
                    }
                    else
                    {
                        subQuery += string.Format(@" or trim(SUB_CODE) in (SELECT TRIM(LINK_SUB_CODE) FROM SA_CUSTOMER_SETUP WHERE 
 COMPANY_CODE='01' and master_customer_code like (select master_customer_code from SA_CUSTOMER_SETUP  where company_code='01' and deleted_flag='N' and customer_code='" + subQueryItem + "')||'%' )");
                    }
                    count++;
                }
                if (masterCustomerIds.Count > 0)
                    subQuery = subQuery + ")";

            }
            var finalQuery = string.Format(query, asOnDate, subQuery, branchQuery);
            var data = this._dbContext.SqlQuery<AgeingDataModel>(finalQuery).ToList();
            return data;
        }

          public IEnumerable<AgeingGroupData> GetMobileGroupData(List<string> branchFilter, string customerCode, string companyCode)
        {
            string query = string.Format(@"SELECT distinct  CUSTOMER_EDESC AS Description,MASTER_CUSTOMER_CODE as MasterCodeWithoutReplace,PRE_CUSTOMER_CODE as PreCodeWithoutReplace,
                                    TO_NUMBER(REPLACE(MASTER_CUSTOMER_CODE,'.','')) as MasterCode, TO_NUMBER(CUSTOMER_CODE) as Code, TO_NUMBER(REPLACE(PRE_CUSTOMER_CODE,'.','')) as PreCode 
                                    FROM SA_CUSTOMER_SETUP WHERE GROUP_SKU_FLAG = 'G' AND DELETED_FLAG = 'N'");
          
            if(branchFilter != null && branchFilter.Count > 0)
            {
                query += string.Format(" AND COMPANY_CODE IN('{0}') ", string.Join("','", branchFilter) );
            }

            return _dbContext.SqlQuery<AgeingGroupData>(query).OrderByDescending(q => q.PreCode);

        }
        public IEnumerable<AgeingDataModel> GetMobileAgeingDebitAmount(string asOnDate, List<string> masterCustomerIds, List<string> branchFilter, string customerCode, string companyCode) {

            var user = new User();
            user.Company = companyCode;
            string query = string.Format(@"SELECT VSL.SUB_CODE as SubCode, VSL.ACC_CODE as AccCode, INITCAP(VSL.SUB_EDESC) Description, VSL.VOUCHER_DATE as VoucherDate, SUM(NVL(VSL.DR_AMOUNT,0)) AMOUNT,TO_NUMBER(CS.CUSTOMER_CODE) as Code
                        ,TO_NUMBER(REPLACE(CS.MASTER_CUSTOMER_CODE,'.','')) as MasterCode,TO_NUMBER(REPLACE(CS.PRE_CUSTOMER_CODE,'.',''))  as PreCode
                        FROM V$VIRTUAL_SUB_LEDGER VSL, SA_CUSTOMER_SETUP CS
                        WHERE VSL.DELETED_FLAG='N'
                     --   AND VSL.Company_Code='{1}'
                        AND VSL.SUB_LEDGER_FLAG='C'
                        AND VSL.COMPANY_CODE=CS.COMPANY_CODE
                        AND TRIM( VSL.SUB_CODE) = TRIM(CS.LINK_SUB_CODE)
                        AND VSL.DR_AMOUNT > 0   
                        AND VSL.POSTED_BY IS NOT NULL
                        AND CS.CUSTOMER_CODE={2}
                        AND TO_DATE(TO_CHAR(VSL.VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{0}', 'YYYY-MM-DD')", asOnDate, user != null ? user.Company : "01",customerCode);


            if(branchFilter != null && branchFilter.Count > 0 )
            {
                query += string.Format(@" AND VSL.BRANCH_CODE IN ('{0}')", string.Join("','", branchFilter));
            }

            if (masterCustomerIds != null && masterCustomerIds.Count > 0)
            {
                string subQuery = @" AND TRIM(VSL.SUB_CODE) IN(SELECT TRIM(LINK_SUB_CODE)FROM SA_CUSTOMER_SETUP WHERE";
                var count = 1;
                foreach (var subQueryItem in masterCustomerIds)
                {


                    if (count > 1)
                    {
                        subQuery += string.Format(@" OR MASTER_CUSTOMER_CODE LIKE '{0}%'", subQueryItem);
                    }
                    else
                    {
                        subQuery += string.Format(@" MASTER_CUSTOMER_CODE LIKE '{0}%'", subQueryItem);
                    }
                    count++;
                }
                subQuery += @" AND DELETED_FLAG = 'N'
                        AND GROUP_SKU_FLAG = 'I')";

                query += subQuery;
            }
            query += @" GROUP BY VSL.SUB_CODE,VSL.ACC_CODE, INITCAP(VSL.SUB_EDESC), VSL.VOUCHER_DATE ,CS.MASTER_CUSTOMER_CODE, CS.PRE_CUSTOMER_CODE, CS.CUSTOMER_CODE
                        ORDER BY INITCAP(VSL.SUB_EDESC), VSL.VOUCHER_DATE";

            var data = this._dbContext.SqlQuery<AgeingDataModel>(query).ToList();
            return data;

        }
        public IEnumerable<AgeingDataModel> GetMobileAgeingCreditAmount(string asOnDate, List<string> masterCustomerIds, List<string> branchFilter, string customerCode, string companyCode)
        {
            var user = new User();
            user.Company = companyCode;
            
            string query = string.Format(@"SELECT VSL.SUB_CODE as SubCode,VSL.ACC_CODE as AccCode, INITCAP(VSL.SUB_EDESC) Description, VSL.VOUCHER_DATE as VoucherDate, SUM(NVL(VSL.CR_AMOUNT,0)) AMOUNT, TO_NUMBER( CS.CUSTOMER_CODE) as Code
                    ,TO_NUMBER(REPLACE(CS.MASTER_CUSTOMER_CODE,'.','')) as MasterCode,TO_NUMBER(REPLACE(CS.PRE_CUSTOMER_CODE,'.',''))  as PreCode
                    FROM V$VIRTUAL_SUB_LEDGER VSL, SA_CUSTOMER_SETUP CS
                    WHERE VSL.DELETED_FLAG='N' 
                   -- AND VSL.Company_Code='{1}'
                    AND VSL.SUB_LEDGER_FLAG='C'
                     AND VSL.COMPANY_CODE=CS.COMPANY_CODE
                    AND TRIM( VSL.SUB_CODE) = TRIM(CS.LINK_SUB_CODE)
                    AND VSL.CR_AMOUNT > 0   
                    AND VSL.POSTED_BY IS NOT NULL
                    AND CS.CUSTOMER_CODE={2}
                    AND TO_DATE(TO_CHAR(VSL.VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{0}', 'YYYY-MM-DD')", asOnDate, user != null ? user.Company : "01", customerCode);


            if (branchFilter != null && branchFilter.Count > 0)
            {
                query += string.Format(@" AND VSL.BRANCH_CODE IN ('{0}')", string.Join("','", branchFilter));
            }

            if (masterCustomerIds != null && masterCustomerIds.Count > 0)
            {
                string subQuery = @" AND TRIM(VSL.SUB_CODE) IN(SELECT TRIM(LINK_SUB_CODE)FROM SA_CUSTOMER_SETUP WHERE";
                var count = 1;
                foreach (var subQueryItem in masterCustomerIds)
                {

                    if (count > 1)
                    {
                        subQuery += string.Format(@" OR MASTER_CUSTOMER_CODE LIKE '{0}%'", subQueryItem);
                    }
                    else
                    {
                        subQuery += string.Format(@" MASTER_CUSTOMER_CODE LIKE '{0}%'", subQueryItem);

                    }
                    count++;
                }
                subQuery += @" AND DELETED_FLAG = 'N'
                        AND GROUP_SKU_FLAG = 'I')";

                query += subQuery;
            }

            query += @"GROUP BY VSL.SUB_CODE, VSL.ACC_CODE, INITCAP(VSL.SUB_EDESC), VSL.VOUCHER_DATE ,CS.MASTER_CUSTOMER_CODE, CS.PRE_CUSTOMER_CODE,CS.CUSTOMER_CODE
                    ORDER BY INITCAP(VSL.SUB_EDESC), VSL.VOUCHER_DATE";
            var data = this._dbContext.SqlQuery<AgeingDataModel>(query).ToList();
            return data;
        }

        public IEnumerable<AgeingDataModel> GetMobileAgeingChartDebitAmount(string asOnDate, List<string> masterCustomerIds, List<string> branchFilter)
        {
            var subQuery = string.Empty;
           
            string query = @"SELECT VOUCHER_DATE VoucherDate, SUM(NVL(DR_AMOUNT,0)) Amount 
                    FROM V$VIRTUAL_SUB_LEDGER 
                    WHERE DELETED_FLAG='N' 
                    AND SUB_LEDGER_FLAG='C' 
                    AND DR_AMOUNT > 0  
                    AND POSTED_BY IS NOT NULL 
                    {2}  
                     AND TO_DATE(TO_CHAR(VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{0}', 'YYYY-MM-DD') {1} 
                    GROUP BY  VOUCHER_DATE 
                    ORDER BY  VOUCHER_DATE";

            var branchQuery = "";
            if (branchFilter != null && branchFilter.Count > 0)
            {
                 branchQuery= string.Format(@" AND BRANCH_CODE IN ('{0}')", string.Join("','", branchFilter));
            }

            if (masterCustomerIds != null && masterCustomerIds.Count > 0)
            {
                
                var count = 1;
                foreach (var subQueryItem in masterCustomerIds)
                {
                    if (string.IsNullOrEmpty(subQueryItem))
                        continue;
                    if (count >= 1)
                    {
                        subQuery += string.Format(@" and(trim(SUB_CODE) in (SELECT TRIM(LINK_SUB_CODE) FROM SA_CUSTOMER_SETUP WHERE 
 COMPANY_CODE='01' and master_customer_code like (select master_customer_code from SA_CUSTOMER_SETUP  where company_code='01' and deleted_flag='N' and customer_code='" + subQueryItem+"')||'%' )");
                    }
                    else
                    {
                        subQuery += string.Format(@" or trim(SUB_CODE) in (SELECT TRIM(LINK_SUB_CODE) FROM SA_CUSTOMER_SETUP WHERE 
 COMPANY_CODE='01' and master_customer_code like (select master_customer_code from SA_CUSTOMER_SETUP  where company_code='01' and deleted_flag='N' and customer_code='" + subQueryItem + "')||'%' )");
                    }
                    
                    count++;
                }

                if (masterCustomerIds.Count > 0)
                {
                    var nullcheck = masterCustomerIds.FirstOrDefault();
                    if(!string.IsNullOrEmpty(nullcheck))
                    {
                        subQuery = subQuery + ")";
                    }
                   

                }
                    


            }
            var finalQuery = string.Format(query, asOnDate, subQuery,branchQuery);
            var data = this._dbContext.SqlQuery<AgeingDataModel>(finalQuery).ToList();
            return data;

        }
        public IEnumerable<AgeingDataModel> GetMobileAgeingChartCreditAmount(string asOnDate, List<string> masterCustomerIds, List<string> branchFilter)
        {

            
            var subQuery = string.Empty;
          
            string query =@"SELECT VOUCHER_DATE VoucherDate, SUM(NVL(CR_AMOUNT,0)) Amount 
                FROM V$VIRTUAL_SUB_LEDGER 
                WHERE DELETED_FLAG='N' 
                AND SUB_LEDGER_FLAG='C' 
                AND CR_AMOUNT > 0  
                AND POSTED_BY IS NOT NULL 
                {2}  
                 AND TO_DATE(TO_CHAR(VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{0}', 'YYYY-MM-DD') {1}
                GROUP BY  VOUCHER_DATE 
                ORDER BY  VOUCHER_DATE";

            var branchQuery = "";
            if (branchFilter != null && branchFilter.Count > 0)
            {
                branchQuery = string.Format(@" AND BRANCH_CODE IN ('{0}')", string.Join("','", branchFilter));
            }

            if (masterCustomerIds != null && masterCustomerIds.Count > 0)
            {

                var count = 1;
                foreach (var subQueryItem in masterCustomerIds)
                {
                    if (string.IsNullOrEmpty(subQueryItem))
                        continue;
                    if (count >= 1)
                    {
                        subQuery += string.Format(@" and (trim(SUB_CODE) in (SELECT TRIM(LINK_SUB_CODE) FROM SA_CUSTOMER_SETUP WHERE 
 COMPANY_CODE='01' and master_customer_code like (select master_customer_code from SA_CUSTOMER_SETUP  where company_code='01' and deleted_flag='N' and customer_code='" + subQueryItem + "')||'%' )");
                    }
                    else
                    {
                        subQuery += string.Format(@" or trim(SUB_CODE) in (SELECT TRIM(LINK_SUB_CODE) FROM SA_CUSTOMER_SETUP WHERE 
 COMPANY_CODE='01' and master_customer_code like (select master_customer_code from SA_CUSTOMER_SETUP  where company_code='01' and deleted_flag='N' and customer_code='" + subQueryItem + "')||'%' )");
                    }
                    count++;
                }
                if (masterCustomerIds.Count > 0)
                {
                    var nullcheck = masterCustomerIds.FirstOrDefault();
                    if (!string.IsNullOrEmpty(nullcheck))
                    {
                        subQuery = subQuery + ")";
                    }
                   // subQuery = subQuery + ")";
                }
                    

            }
            var finalQuery = string.Format(query, asOnDate, subQuery,branchQuery);
            var data = this._dbContext.SqlQuery<AgeingDataModel>(finalQuery).ToList();
            return data;
        }
        public IEnumerable<AgeingDataModel> GetMobileAgeingDealerDebitAmount(string asOnDate, List<string> masterCustomerIds, List<string> branchFilter, string AccountCode = "0", string DealerGropCode = "0")
        {
            var subQuery = string.Empty;

            string query = @"SELECT VOUCHER_DATE VoucherDate, SUM(NVL(DR_AMOUNT,0)) Amount 
                    FROM V$VIRTUAL_SUB_DEALER_LEDGER
                    WHERE DELETED_FLAG='N' 
                    AND SUB_LEDGER_FLAG='C' 
                    AND DR_AMOUNT > 0  
                    AND POSTED_BY IS NOT NULL 
                    {2}  
                     AND TO_DATE(TO_CHAR(VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{0}', 'YYYY-MM-DD') {1} 
                    GROUP BY  VOUCHER_DATE 
                    ORDER BY  VOUCHER_DATE";

            var branchQuery = "";
            if (branchFilter != null && branchFilter.Count > 0)
            {
                branchQuery = string.Format(@" AND BRANCH_CODE IN ('{0}')", string.Join("','", branchFilter));
            }

            //var subQuery = "";
            if (DealerGropCode != "0")
            {
                subQuery = $@"and upper(dealer_group)=UPPER('{DealerGropCode}')";
            }
            if (masterCustomerIds != null && masterCustomerIds.Count > 0)
            {

                subQuery += string.Format(@"  AND TRIM(PARTY_TYPE_CODE) IN  ('{0}')", string.Join("','", masterCustomerIds));
            }
            var finalQuery = string.Format(query, asOnDate, subQuery, branchQuery);
            var data = this._dbContext.SqlQuery<AgeingDataModel>(finalQuery).ToList();
            return data;

        }

        public IEnumerable<AgeingDataModel> GetMobileAgeingDealerCreditAmount(string asOnDate, List<string> masterCustomerIds, List<string> branchFilter, string AccountCode = "0", string DealerGropCode = "0")
        {


            var subQuery = string.Empty;

            string query = @"SELECT VOUCHER_DATE VoucherDate, SUM(NVL(CR_AMOUNT,0)) Amount 
                FROM V$VIRTUAL_SUB_DEALER_LEDGER
                WHERE DELETED_FLAG='N' 
                AND SUB_LEDGER_FLAG='C' 
                AND CR_AMOUNT > 0  
                AND POSTED_BY IS NOT NULL 
                {2}  
                 AND TO_DATE(TO_CHAR(VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{0}', 'YYYY-MM-DD') {1}
                GROUP BY  VOUCHER_DATE 
                ORDER BY  VOUCHER_DATE";

            var branchQuery = "";
            if (branchFilter != null && branchFilter.Count > 0)
            {
                branchQuery = string.Format(@" AND BRANCH_CODE IN ('{0}')", string.Join("','", branchFilter));
            }

            //var subQuery = "";
            if(DealerGropCode!="0")
            {
                subQuery = $@"and upper(dealer_group)=UPPER('{DealerGropCode}')";
            }
            if (masterCustomerIds != null && masterCustomerIds.Count > 0)
            {
                
                  subQuery += string.Format(@"  AND TRIM(PARTY_TYPE_CODE) IN  ('{0}')", string.Join("','", masterCustomerIds));
            }
            var finalQuery = string.Format(query, asOnDate, subQuery, branchQuery);
            var data = this._dbContext.SqlQuery<AgeingDataModel>(finalQuery).ToList();
            return data;

        }

        public IEnumerable<testAgeing> testAgeingData(string asOnDate, List<string> masterCustomerIds, List<string> branchFilter)
        {
            var To_Date1 = Convert.ToDateTime(asOnDate).ToString("MM /dd/yyyy");
            var From_Date1 = Convert.ToDateTime(To_Date1).AddDays(-30).ToString("MM/dd/yyyy");
            var To_Date2 = Convert.ToDateTime(From_Date1).AddDays(-1).ToString("MM/dd/yyyy");
            var From_Date2 = Convert.ToDateTime(To_Date2).AddDays(-30).ToString("MM/dd/yyyy");
            var To_Date3 = Convert.ToDateTime(From_Date2).AddDays(-1).ToString("MM/dd/yyyy");
            var From_Date3 = Convert.ToDateTime(To_Date3).AddDays(-30).ToString("MM/dd/yyyy");

            var To_Date4 = Convert.ToDateTime(From_Date3).AddDays(-1).ToString("MM/dd/yyyy");
            var From_Date4 = Convert.ToDateTime(To_Date4).AddDays(-30).ToString("MM/dd/yyyy");

            var To_Date5 = Convert.ToDateTime(From_Date4).AddDays(-1).ToString("MM/dd/yyyy");
            var From_Date5 = Convert.ToDateTime(To_Date5).AddDays(-30).ToString("MM/dd/yyyy");

            var subCode = string.Empty;

            foreach (var item in masterCustomerIds)
            {
                subCode = item.ToString();
            }

            var subQuery = string.Empty;

            //string query = $@"SELECT DISTINCT
            //                 A.COMPANY_CODE,
            //                 SUM (
            //                    (SELECT SUM (ROUND (NVL (C.CR_AMOUNT, 0), 2))
            //                       FROM V$CUSTOMER_BILLAGE_LEDGER C
            //                      WHERE     C.COMPANY_CODE = A.COMPANY_CODE
            //                            AND C.BRANCH_CODE = A.BRANCH_CODE
            //                            --AND C.SERIAL_NO = A.SERIAL_NO
            //                            --AND C.SUB_CODE = A.SUB_CODE
            //                            --AND C.ACC_CODE = A.ACC_CODE
            //                            --AND C.FORM_CODE = A.FORM_CODE
            //                            --AND C.VOUCHER_NO = A.VOUCHER_NO
            //                            AND TO_DATE (C.VOUCHER_DATE, 'DD-MM-YYYY') <=
            //                                   TO_DATE ('{asOnDate}','YYYY-MM-DD' )
            //                            AND C.COMPANY_CODE = '01'))
            //                    CR_AMOUNT,
            //                 SUM (
            //                    (SELECT SUM (ROUND (NVL (C.DR_AMOUNT, 0), 2))
            //                       FROM V$CUSTOMER_BILLAGE_LEDGER C
            //                      WHERE     C.COMPANY_CODE = A.COMPANY_CODE
            //                            AND C.BRANCH_CODE = A.BRANCH_CODE
            //                            --AND C.SERIAL_NO = A.SERIAL_NO
            //                            --AND C.SUB_CODE = A.SUB_CODE
            //                            --AND C.ACC_CODE = A.ACC_CODE
            //                            --AND C.FORM_CODE = A.FORM_CODE
            //                            --AND C.VOUCHER_NO = A.VOUCHER_NO
            //                            AND TO_DATE (C.VOUCHER_DATE, 'DD-MM-YYYY') >=
            //                                   TO_DATE ('{From_Date5}', 'YYYY-MM-DD' )
            //                            AND TO_DATE (C.VOUCHER_DATE, 'DD-MM-YYYY') <=
            //                                   TO_DATE ('{To_Date5}', 'YYYY-MM-DD' )
            //                            AND C.COMPANY_CODE = '01'))
            //                    DR_AMOUNT4,
            //                 SUM (
            //                    (SELECT SUM (ROUND (NVL (C.DR_AMOUNT, 0), 2))
            //                       FROM V$CUSTOMER_BILLAGE_LEDGER C
            //                      WHERE     C.COMPANY_CODE = A.COMPANY_CODE
            //                            AND C.BRANCH_CODE = A.BRANCH_CODE
            //                            --AND C.SERIAL_NO = A.SERIAL_NO
            //                            --AND C.SUB_CODE = A.SUB_CODE
            //                            --AND C.ACC_CODE = A.ACC_CODE
            //                            --AND C.FORM_CODE = A.FORM_CODE
            //                            --AND C.VOUCHER_NO = A.VOUCHER_NO
            //                            AND TO_DATE (C.VOUCHER_DATE, 'DD-MM-YYYY') >=
            //                                   TO_DATE ('{From_Date4}', 'YYYY-MM-DD' )
            //                            AND TO_DATE (C.VOUCHER_DATE, 'DD-MM-YYYY') <=
            //                                   TO_DATE ('{To_Date4}', 'YYYY-MM-DD' )
            //                            AND C.COMPANY_CODE = '01'))
            //                    DR_AMOUNT3,
            //                 SUM (
            //                    (SELECT SUM (ROUND (NVL (C.DR_AMOUNT, 0), 2))
            //                       FROM V$CUSTOMER_BILLAGE_LEDGER C
            //                      WHERE     C.COMPANY_CODE = A.COMPANY_CODE
            //                            AND C.BRANCH_CODE = A.BRANCH_CODE
            //                            --AND C.SERIAL_NO = A.SERIAL_NO
            //                            --AND C.SUB_CODE = A.SUB_CODE
            //                            --AND C.ACC_CODE = A.ACC_CODE
            //                            --AND C.FORM_CODE = A.FORM_CODE
            //                            --AND C.VOUCHER_NO = A.VOUCHER_NO
            //                            AND TO_DATE (C.VOUCHER_DATE, 'DD-MM-YYYY') >=
            //                                   TO_DATE ('{From_Date3}', 'YYYY-MM-DD' )
            //                            AND TO_DATE (C.VOUCHER_DATE, 'DD-MM-YYYY') <=
            //                                   TO_DATE ('{To_Date3}', 'YYYY-MM-DD' )
            //                            AND C.COMPANY_CODE = '01'))
            //                    DR_AMOUNT2,
            //                 SUM (
            //                    (SELECT SUM (ROUND (NVL (C.DR_AMOUNT, 0), 2))
            //                       FROM V$CUSTOMER_BILLAGE_LEDGER C
            //                      WHERE     C.COMPANY_CODE = A.COMPANY_CODE
            //                            AND C.BRANCH_CODE = A.BRANCH_CODE
            //                            --AND C.SERIAL_NO = A.SERIAL_NO
            //                            --AND C.SUB_CODE = A.SUB_CODE
            //                            --AND C.ACC_CODE = A.ACC_CODE
            //                            --AND C.FORM_CODE = A.FORM_CODE
            //                            --AND C.VOUCHER_NO = A.VOUCHER_NO
            //                            AND TO_DATE (C.VOUCHER_DATE, 'DD-MM-YYYY') >=
            //                                   TO_DATE ('{From_Date2}', 'YYYY-MM-DD' )
            //                            AND TO_DATE (C.VOUCHER_DATE, 'DD-MM-YYYY') <=
            //                                   TO_DATE ('{To_Date2}', 'YYYY-MM-DD' )
            //                            AND C.COMPANY_CODE = '01'))
            //                    DR_AMOUNT1,
            //                 SUM (
            //                    (SELECT SUM (ROUND (NVL (C.DR_AMOUNT, 0), 2))
            //                       FROM V$CUSTOMER_BILLAGE_LEDGER C
            //                      WHERE     C.COMPANY_CODE = A.COMPANY_CODE
            //                            AND C.BRANCH_CODE = A.BRANCH_CODE
            //                            --AND C.SERIAL_NO = A.SERIAL_NO
            //                            --AND C.SUB_CODE = A.SUB_CODE
            //                            --AND C.ACC_CODE = A.ACC_CODE
            //                            --AND C.FORM_CODE = A.FORM_CODE
            //                            --AND C.VOUCHER_NO = A.VOUCHER_NO
            //                            AND TO_DATE (C.VOUCHER_DATE, 'DD-MM-YYYY') >=
            //                                   TO_DATE ('{From_Date1}', 'YYYY-MM-DD' )
            //                            AND TO_DATE (C.VOUCHER_DATE, 'DD-MM-YYYY') <=
            //                                   TO_DATE ('{To_Date1}', 'YYYY-MM-DD' )
            //                            AND C.COMPANY_CODE = '01'))
            //                    DR_AMOUNT0
            //            FROM V$CUSTOMER_BILLAGE_LEDGER A
            //           WHERE A.DELETED_FLAG = 'N'
            //                 --AND A.SUB_CODE = '{subCode}'
            //                 AND TO_DATE (A.VOUCHER_DATE, 'DD-MM-YYYY') <=
            //                        TO_DATE ('{asOnDate}', 'YYYY-MM-DD' )
            //        GROUP BY A.COMPANY_CODE";

            string query = $@"SELECT DISTINCT
                             A.COMPANY_CODE,
                             SUM (
                                (SELECT SUM (ROUND (NVL (C.CR_AMOUNT, 0), 2))
                                   FROM V$VIRTUAL_SUB_LEDGER C
                                  WHERE     C.COMPANY_CODE = A.COMPANY_CODE
                                        AND C.BRANCH_CODE = A.BRANCH_CODE
                                        AND TO_DATE (C.VOUCHER_DATE) <= 
                                               TO_DATE ('{asOnDate}', 'YYYY-MM-DD' )
                                        AND C.COMPANY_CODE = '01'))
                                CR_AMOUNT,
                             SUM (
                                (SELECT SUM (ROUND (NVL (C.DR_AMOUNT, 0), 2))
                                   FROM V$VIRTUAL_SUB_LEDGER C
                                  WHERE     C.COMPANY_CODE = A.COMPANY_CODE
                                        AND C.BRANCH_CODE = A.BRANCH_CODE
                                        AND TO_DATE (C.VOUCHER_DATE) >=
                                               TO_DATE ('{From_Date5}', 'MM-DD-YYYY' )
                                        AND TO_DATE (C.VOUCHER_DATE) <=
                                               TO_DATE ('{To_Date5}', 'MM-DD-YYYY' )
                                        AND C.COMPANY_CODE = '01'))
                                DR_AMOUNT4,
                             SUM (
                                (SELECT SUM (ROUND (NVL (C.DR_AMOUNT, 0), 2))
                                   FROM V$VIRTUAL_SUB_LEDGER C
                                  WHERE     C.COMPANY_CODE = A.COMPANY_CODE
                                        AND C.BRANCH_CODE = A.BRANCH_CODE
                                        AND TO_DATE (C.VOUCHER_DATE) >=
                                               TO_DATE ('{From_Date4}', 'MM-DD-YYYY' )
                                        AND TO_DATE (C.VOUCHER_DATE) <=
                                               TO_DATE ('{To_Date4}', 'MM-DD-YYYY' )
                                        AND C.COMPANY_CODE = '01'))
                                DR_AMOUNT3,
                             SUM (
                                (SELECT SUM (ROUND (NVL (C.DR_AMOUNT, 0), 2))
                                   FROM V$VIRTUAL_SUB_LEDGER C
                                  WHERE     C.COMPANY_CODE = A.COMPANY_CODE
                                        AND C.BRANCH_CODE = A.BRANCH_CODE
                                        AND TO_DATE (C.VOUCHER_DATE) >=
                                               TO_DATE ('{From_Date3}', 'MM-DD-YYYY' )
                                        AND TO_DATE (C.VOUCHER_DATE) <=
                                               TO_DATE ('{To_Date3}', 'MM-DD-YYYY' )
                                        AND C.COMPANY_CODE = '01'))
                                DR_AMOUNT2,
                             SUM (
                                (SELECT SUM (ROUND (NVL (C.DR_AMOUNT, 0), 2))
                                   FROM V$VIRTUAL_SUB_LEDGER C
                                  WHERE     C.COMPANY_CODE = A.COMPANY_CODE
                                        AND C.BRANCH_CODE = A.BRANCH_CODE
                                        AND TO_DATE (C.VOUCHER_DATE) >=
                                               TO_DATE ('{From_Date2}', 'MM-DD-YYYY' )
                                        AND TO_DATE (C.VOUCHER_DATE) <=
                                               TO_DATE ('{To_Date2}', 'MM-DD-YYYY' )
                                        AND C.COMPANY_CODE = '01'))
                                DR_AMOUNT1,
                             SUM (
                                (SELECT SUM (ROUND (NVL (C.DR_AMOUNT, 0), 2))
                                   FROM V$VIRTUAL_SUB_LEDGER C
                                  WHERE     C.COMPANY_CODE = A.COMPANY_CODE
                                        AND C.BRANCH_CODE = A.BRANCH_CODE
                                        AND TO_DATE (C.VOUCHER_DATE) >=
                                               TO_DATE ('{From_Date1}', 'MM-DD-YYYY' )
                                        AND TO_DATE (C.VOUCHER_DATE) <=
                                               TO_DATE ('{To_Date1}', 'MM-DD-YYYY' )
                                        AND C.COMPANY_CODE = '01'))
                                DR_AMOUNT0
                        FROM V$VIRTUAL_SUB_LEDGER A
                       WHERE A.DELETED_FLAG = 'N'
                             AND TO_DATE (A.VOUCHER_DATE) <=
                                    TO_DATE ('{asOnDate}', 'YYYY-MM-DD' )
                    GROUP BY A.COMPANY_CODE";
            var data = this._dbContext.SqlQuery<testAgeing>(query).ToList();
            return data;
        }

    }
}
