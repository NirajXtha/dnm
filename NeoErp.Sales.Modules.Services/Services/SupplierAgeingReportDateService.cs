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
    public class SupplierAgeingReportDateService:IAgeingReportDataService
    {
        private IDbContext _dbContext;
        private ICacheManager _cacheManager;
        private IWorkContext _workContext;
        public SupplierAgeingReportDateService(IDbContext dbContext, ICacheManager cacheManager, IWorkContext workContext)
        {
            this._workContext = workContext;
            this._dbContext = dbContext;
            this._cacheManager = cacheManager;
        }

        public IEnumerable<AgeingDataModel> GetAgeingDebitAmount(string asOnDate, List<string> masterSupplierIds)
        {
            var user = this._workContext.CurrentUserinformation;
            string query = string.Format(@"SELECT VSL.SUB_CODE as SubCode,VSL.ACC_CODE as AccCode, INITCAP(VSL.SUB_EDESC) Description, VSL.VOUCHER_DATE, SUM(NVL(FN_CONVERT_CURRENCY(NVL(VSL.DR_AMOUNT,0),'NRS',VSL.VOUCHER_DATE),0)) AMOUNT,TO_NUMBER(CS.SUPPLIER_CODE) as Code
                        ,TO_NUMBER(REPLACE(CS.MASTER_SUPPLIER_CODE,'.','')) as MasterCode,TO_NUMBER(REPLACE(CS.PRE_SUPPLIER_CODE,'.',''))  as PreCode
                        FROM V$VIRTUAL_SUB_LEDGER VSL, IP_SUPPLIER_SETUP CS
                        WHERE VSL.DELETED_FLAG='N' 
                        AND VSL.SUB_LEDGER_FLAG='S' 
                        AND VSL.Company_Code='{2}'
                        AND VSL.Branch_Code='{1}'
                        AND TRIM( VSL.SUB_CODE) = TRIM(CS.LINK_SUB_CODE)
                        AND VSL.DR_AMOUNT > 0   
                        AND VSL.POSTED_BY IS NOT NULL
                        AND TO_DATE(TO_CHAR(VSL.VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{0}', 'YYYY-MM-DD')", asOnDate, user != null ? user.Branch : "01.01", user != null ? user.Company : "01" );
                        

            if(masterSupplierIds != null && masterSupplierIds.Count > 0)
            {
                string subQuery = @" AND TRIM(VSL.SUB_CODE) IN(SELECT TRIM(LINK_SUB_CODE)FROM IP_SUPPLIER_SETUP WHERE";
                var count = 1;
                foreach (var subQueryItem in masterSupplierIds)
                {
                    
                    if(count > 1)
                    {
                        subQuery += string.Format(@" OR MASTER_SUPPLIER_CODE LIKE '{0}%'", subQueryItem);
                    }
                    else
                    {
                        subQuery += string.Format(@" MASTER_SUPPLIER_CODE LIKE '{0}%'", subQueryItem);
                    }
                    count++;
                }
                subQuery += @" AND DELETED_FLAG = 'N'
                        AND GROUP_SKU_FLAG = 'I')";

                query += subQuery;
            }

            query += @" GROUP BY VSL.SUB_CODE,VSL.ACC_CODE, INITCAP(VSL.SUB_EDESC), VSL.VOUCHER_DATE ,CS.MASTER_SUPPLIER_CODE, CS.PRE_SUPPLIER_CODE, CS.SUPPLIER_CODE
                        ORDER BY INITCAP(VSL.SUB_EDESC), VSL.VOUCHER_DATE";

            var data = this._dbContext.SqlQuery<AgeingDataModel>(query).ToList();
            return data;
        }

        public IEnumerable<AgeingDataModel> GetAgeingCreditAmount(string asOnDate, List<string> masterSupplierIds)
        {
            var user = this._workContext.CurrentUserinformation;
            string query = string.Format(@"SELECT VSL.SUB_CODE as SubCode,VSL.ACC_CODE as AccCode, INITCAP(VSL.SUB_EDESC) Description, VSL.VOUCHER_DATE, SUM(NVL(FN_CONVERT_CURRENCY(NVL(VSL.CR_AMOUNT,0),'NRS',VSL.VOUCHER_DATE),0)) AMOUNT, TO_NUMBER( CS.SUPPLIER_CODE) as Code
                    ,TO_NUMBER(REPLACE(CS.MASTER_SUPPLIER_CODE,'.','')) as MasterCode,TO_NUMBER(REPLACE(CS.PRE_SUPPLIER_CODE,'.',''))  as PreCode
                    FROM V$VIRTUAL_SUB_LEDGER VSL, IP_SUPPLIER_SETUP CS
                    WHERE VSL.DELETED_FLAG='N' 
                    AND VSL.SUB_LEDGER_FLAG='S'
                    AND VSL.Company_Code='{2}'
                    AND VSL.Branch_Code='{1}'
                    AND TRIM( VSL.SUB_CODE) = TRIM(CS.LINK_SUB_CODE)
                    AND VSL.CR_AMOUNT > 0   
                    AND VSL.POSTED_BY IS NOT NULL
                    AND TO_DATE(TO_CHAR(VSL.VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{0}', 'YYYY-MM-DD')", asOnDate, user != null ? user.Branch : "01.01", user != null ? user.Company : "01");

            if (masterSupplierIds != null && masterSupplierIds.Count > 0)
            {
                string subQuery = @" AND TRIM(VSL.SUB_CODE) IN(SELECT TRIM(LINK_SUB_CODE)FROM IP_SUPPLIER_SETUP WHERE";
                var count = 1;
                foreach (var subQueryItem in masterSupplierIds)
                {
                    

                    if (count > 1)
                    {
                        subQuery += string.Format(@" OR MASTER_SUPPLIER_CODE LIKE '{0}%'", subQueryItem);
                    }
                    else
                    {
                        subQuery += string.Format(@" MASTER_SUPPLIER_CODE LIKE '{0}%'", subQueryItem);
                    }
                    count++;
                }
                subQuery += @" AND DELETED_FLAG = 'N'
                        AND GROUP_SKU_FLAG = 'I')";

                query += subQuery;
            }

            query += @" GROUP BY VSL.SUB_CODE, VSL.ACC_CODE, INITCAP(VSL.SUB_EDESC), VSL.VOUCHER_DATE ,CS.MASTER_SUPPLIER_CODE, CS.PRE_SUPPLIER_CODE,CS.SUPPLIER_CODE
                    ORDER BY INITCAP(VSL.SUB_EDESC), VSL.VOUCHER_DATE";
            var data = this._dbContext.SqlQuery<AgeingDataModel>(query).ToList();
            return data;

        }

        public IEnumerable<AgeingGroupData> GetGroupData()
        {
            return this.GetGroupData(null);
        }

        public IEnumerable<AgeingGroupData> GetGroupData(List<string> branchFilter)
        {
            var user = this._workContext.CurrentUserinformation;
            var query = string.Format(@"SELECT  Supplier_EDESC AS Description,
                                    TO_NUMBER(REPLACE(Master_SUPPLIER_CODE,'.','')) as MasterCode, TO_NUMBER(SUPPLIER_CODE) as Code, TO_NUMBER(REPLACE(PRE_SUPPLIER_CODE,'.','')) as PreCode 
                                    FROM IP_SUPPLIER_SETUP WHERE GROUP_SKU_FLAG = 'G' AND DELETED_FLAG = 'N'");
            if(branchFilter != null && branchFilter.Count> 0)
            {
                query += string.Format(" AND COMPANY_CODE IN ('{0}')", string.Join("','", branchFilter));
            }
            return _dbContext.SqlQuery<AgeingGroupData>(query).OrderByDescending(q => q.PreCode);
        }

        public IEnumerable<AgeingDataModel> GetAgeingDebitAmount(string asOnDate, List<string> masterSupplierIds,List<string> branchFilters, string billwise = null)
        {
            var user = this._workContext.CurrentUserinformation;
            string query = string.Format(@"SELECT VSL.SUB_CODE as SubCode,VSL.ACC_CODE as AccCode, INITCAP(VSL.SUB_EDESC) Description, VSL.VOUCHER_DATE as VoucherDate, SUM(NVL(FN_CONVERT_CURRENCY(NVL(VSL.DR_AMOUNT,0),'NRS',VSL.VOUCHER_DATE),0)) AMOUNT,TO_NUMBER(CS.SUPPLIER_CODE) as Code
                        ,TO_NUMBER(REPLACE(CS.MASTER_SUPPLIER_CODE,'.','')) as MasterCode,TO_NUMBER(REPLACE(CS.PRE_SUPPLIER_CODE,'.',''))  as PreCode
                        FROM V$VIRTUAL_SUB_LEDGER VSL, IP_SUPPLIER_SETUP CS
                        WHERE VSL.DELETED_FLAG='N' 
                        AND VSL.SUB_LEDGER_FLAG='S' 
                      --  AND VSL.Company_Code='{1}'
                         AND VSL.COMPANY_CODE=CS.COMPANY_CODE
                        AND TRIM( VSL.SUB_CODE) = TRIM(CS.LINK_SUB_CODE)
                        AND VSL.DR_AMOUNT > 0   
                        AND VSL.POSTED_BY IS NOT NULL
                        AND TO_DATE(TO_CHAR(VSL.VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{0}', 'YYYY-MM-DD')", asOnDate, user != null ? user.Company : "01");


            if (branchFilters != null && branchFilters.Count > 0)
            {
                query += string.Format(@" AND VSL.BRANCH_CODE IN ('{0}')", string.Join("','", branchFilters));
            }


            if (masterSupplierIds != null && masterSupplierIds.Count > 0)
            {
                string subQuery = @" AND TRIM(VSL.SUB_CODE) IN(SELECT TRIM(LINK_SUB_CODE)FROM IP_SUPPLIER_SETUP WHERE";
                var count = 1;
                foreach (var subQueryItem in masterSupplierIds)
                {

                    if (count > 1)
                    {
                        subQuery += string.Format(@" OR MASTER_SUPPLIER_CODE LIKE '{0}%'", subQueryItem);
                    }
                    else
                    {
                        subQuery += string.Format(@" MASTER_SUPPLIER_CODE LIKE '{0}%'", subQueryItem);
                    }
                    count++;
                }
                subQuery += @" AND DELETED_FLAG = 'N'
                        AND GROUP_SKU_FLAG = 'I')";

                query += subQuery;
            }

            query += @" GROUP BY VSL.SUB_CODE,VSL.ACC_CODE, INITCAP(VSL.SUB_EDESC), VSL.VOUCHER_DATE ,CS.MASTER_SUPPLIER_CODE, CS.PRE_SUPPLIER_CODE, CS.SUPPLIER_CODE
                        ORDER BY INITCAP(VSL.SUB_EDESC), VSL.VOUCHER_DATE";

            var data = this._dbContext.SqlQuery<AgeingDataModel>(query).ToList();
            return data;
        }

        public IEnumerable<AgeingDataModel> GetAgeingCreditAmount(string asOnDate, List<string> masterSupplierIds,List<string> branchFilters,string billwise= null)
        {
            var user = this._workContext.CurrentUserinformation;
            string query = string.Format(@"SELECT VSL.SUB_CODE as SubCode,VSL.ACC_CODE as AccCode, INITCAP(VSL.SUB_EDESC) Description, VSL.VOUCHER_DATE as VoucherDate, SUM(NVL(FN_CONVERT_CURRENCY(NVL(VSL.CR_AMOUNT,0),'NRS',VSL.VOUCHER_DATE),0)) AMOUNT, TO_NUMBER( CS.SUPPLIER_CODE) as Code
                    ,TO_NUMBER(REPLACE(CS.MASTER_SUPPLIER_CODE,'.','')) as MasterCode,TO_NUMBER(REPLACE(CS.PRE_SUPPLIER_CODE,'.',''))  as PreCode
                    FROM V$VIRTUAL_SUB_LEDGER VSL, IP_SUPPLIER_SETUP CS
                    WHERE VSL.DELETED_FLAG='N' 
                    AND VSL.SUB_LEDGER_FLAG='S'
                  --  AND VSL.Company_Code='{1}'
                    AND TRIM( VSL.SUB_CODE) = TRIM(CS.LINK_SUB_CODE)
                    AND VSL.CR_AMOUNT > 0  
                     AND VSL.COMPANY_CODE=CS.COMPANY_CODE 
                    AND VSL.POSTED_BY IS NOT NULL
                    AND TO_DATE(TO_CHAR(VSL.VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{0}', 'YYYY-MM-DD')", asOnDate, user != null ? user.Company : "01");

            if (branchFilters != null && branchFilters.Count > 0)
            {
                query += string.Format(@" AND VSL.BRANCH_CODE IN ('{0}')", string.Join("','", branchFilters));
            }

            if (masterSupplierIds != null && masterSupplierIds.Count > 0)
            {
                string subQuery = @" AND TRIM(VSL.SUB_CODE) IN(SELECT TRIM(LINK_SUB_CODE)FROM IP_SUPPLIER_SETUP WHERE";
                var count = 1;
                foreach (var subQueryItem in masterSupplierIds)
                {


                    if (count > 1)
                    {
                        subQuery += string.Format(@" OR MASTER_SUPPLIER_CODE LIKE '{0}%'", subQueryItem);
                    }
                    else
                    {
                        subQuery += string.Format(@" MASTER_SUPPLIER_CODE LIKE '{0}%'", subQueryItem);
                    }
                    count++;
                }
                subQuery += @" AND DELETED_FLAG = 'N'
                        AND GROUP_SKU_FLAG = 'I')";

                query += subQuery;
            }

            query += @" GROUP BY VSL.SUB_CODE, VSL.ACC_CODE, INITCAP(VSL.SUB_EDESC), VSL.VOUCHER_DATE ,CS.MASTER_SUPPLIER_CODE, CS.PRE_SUPPLIER_CODE,CS.SUPPLIER_CODE
                    ORDER BY INITCAP(VSL.SUB_EDESC), VSL.VOUCHER_DATE";
            var data = this._dbContext.SqlQuery<AgeingDataModel>(query).ToList();
            return data;

        }
        public IEnumerable<AgeingDataModel> GetAgeingChartDebitAmount(string asOnDate, List<string> masterProductIds, List<string> branchFilter)
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
AND SUB_LEDGER_FLAG='S' 
AND DR_AMOUNT > 0  
AND POSTED_BY IS NOT NULL 
AND COMPANY_CODE='01' 
 AND TO_DATE(TO_CHAR(VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{0}', 'YYYY-MM-DD') {1} 
GROUP BY  VOUCHER_DATE 
ORDER BY  VOUCHER_DATE";


            //if (branchFilter != null && branchFilter.Count > 0)
            //{
            //    query += string.Format(@" AND VSL.BRANCH_CODE IN ('{0}')", string.Join("','", branchFilter));
            //}

            if (masterProductIds != null && masterProductIds.Count > 0)
            {

                var count = 1;
                foreach (var subQueryItem in masterProductIds)
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
                if (masterProductIds.Count > 0)
                    subQuery = subQuery + ")";


            }
            var finalQuery = string.Format(query, asOnDate, subQuery);
            var data = this._dbContext.SqlQuery<AgeingDataModel>(finalQuery).ToList();
            return data;

        }

        public IEnumerable<AgeingDataModel> GetAgeingChartCreditAmount(string asOnDate, List<string> masterProductIds, List<string> branchFilter)
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
AND SUB_LEDGER_FLAG='S' 
AND CR_AMOUNT > 0  
AND POSTED_BY IS NOT NULL 
AND COMPANY_CODE='01' 
 AND TO_DATE(TO_CHAR(VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{0}', 'YYYY-MM-DD') {1}
GROUP BY  VOUCHER_DATE 
ORDER BY  VOUCHER_DATE";



            if (masterProductIds != null && masterProductIds.Count > 0)
            {

                var count = 1;
                foreach (var subQueryItem in masterProductIds)
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
                if (masterProductIds.Count > 0)
                    subQuery = subQuery + ")";

            }
            var finalQuery = string.Format(query, asOnDate, subQuery);
            var data = this._dbContext.SqlQuery<AgeingDataModel>(finalQuery).ToList();
            return data;

        }
       public IEnumerable<AgeingDataModel> GetAgeingBillWiseChartDebitAmount(string asOnDate, List<string> masterCustomerIds, List<string> branchFilter)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<AgeingDataModel> GetAgeingBillWiseChartCreditAmount(string asOnDate, List<string> masterCustomerIds, List<string> branchFilter)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AgeingGroupData> GetMobileGroupData(List<string> branchFilter, string customerCode, string companyCode)
        {
            var query = string.Format(@"SELECT  Supplier_EDESC AS Description,
                                    TO_NUMBER(REPLACE(Master_SUPPLIER_CODE,'.','')) as MasterCode, TO_NUMBER(SUPPLIER_CODE) as Code, TO_NUMBER(REPLACE(PRE_SUPPLIER_CODE,'.','')) as PreCode 
                                    FROM IP_SUPPLIER_SETUP WHERE GROUP_SKU_FLAG = 'G' AND DELETED_FLAG = 'N'");
            if(branchFilter != null && branchFilter.Count> 0)
            {
                query += string.Format(" AND COMPANY_CODE IN ('{0}')", string.Join("','", branchFilter));
            }
            return _dbContext.SqlQuery<AgeingGroupData>(query).OrderByDescending(q => q.PreCode);
        }

        public IEnumerable<AgeingDataModel> GetMobileAgeingDebitAmount(string asOnDate, List<string> masterSupplierIds,List<string> branchFilters,string customerCode, string companyCode)
        {
            var user = new User();
            user.Company = companyCode;
            string query = string.Format(@"SELECT VSL.SUB_CODE as SubCode,VSL.ACC_CODE as AccCode, INITCAP(VSL.SUB_EDESC) Description, VSL.VOUCHER_DATE as VoucherDate, SUM(NVL(FN_CONVERT_CURRENCY(NVL(VSL.DR_AMOUNT,0),'NRS',VSL.VOUCHER_DATE),0)) AMOUNT,TO_NUMBER(CS.SUPPLIER_CODE) as Code
                        ,TO_NUMBER(REPLACE(CS.MASTER_SUPPLIER_CODE,'.','')) as MasterCode,TO_NUMBER(REPLACE(CS.PRE_SUPPLIER_CODE,'.',''))  as PreCode
                        FROM V$VIRTUAL_SUB_LEDGER VSL, IP_SUPPLIER_SETUP CS
                        WHERE VSL.DELETED_FLAG='N' 
                        AND VSL.SUB_LEDGER_FLAG='S' 
                      --  AND VSL.Company_Code='{1}'
                         AND VSL.COMPANY_CODE=CS.COMPANY_CODE
                        AND TRIM( VSL.SUB_CODE) = TRIM(CS.LINK_SUB_CODE)
                        AND VSL.DR_AMOUNT > 0   
                        AND VSL.POSTED_BY IS NOT NULL
                        AND TO_DATE(TO_CHAR(VSL.VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{0}', 'YYYY-MM-DD')", asOnDate, user != null ? user.Company : "01");


            if (branchFilters != null && branchFilters.Count > 0)
            {
                query += string.Format(@" AND VSL.BRANCH_CODE IN ('{0}')", string.Join("','", branchFilters));
            }


            if (masterSupplierIds != null && masterSupplierIds.Count > 0)
            {
                string subQuery = @" AND TRIM(VSL.SUB_CODE) IN(SELECT TRIM(LINK_SUB_CODE)FROM IP_SUPPLIER_SETUP WHERE";
                var count = 1;
                foreach (var subQueryItem in masterSupplierIds)
                {

                    if (count > 1)
                    {
                        subQuery += string.Format(@" OR MASTER_SUPPLIER_CODE LIKE '{0}%'", subQueryItem);
                    }
                    else
                    {
                        subQuery += string.Format(@" MASTER_SUPPLIER_CODE LIKE '{0}%'", subQueryItem);
                    }
                    count++;
                }
                subQuery += @" AND DELETED_FLAG = 'N'
                        AND GROUP_SKU_FLAG = 'I')";

                query += subQuery;
            }

            query += @" GROUP BY VSL.SUB_CODE,VSL.ACC_CODE, INITCAP(VSL.SUB_EDESC), VSL.VOUCHER_DATE ,CS.MASTER_SUPPLIER_CODE, CS.PRE_SUPPLIER_CODE, CS.SUPPLIER_CODE
                        ORDER BY INITCAP(VSL.SUB_EDESC), VSL.VOUCHER_DATE";

            var data = this._dbContext.SqlQuery<AgeingDataModel>(query).ToList();
            return data;
        }

        public IEnumerable<AgeingDataModel> GetMobileAgeingCreditAmount(string asOnDate, List<string> masterSupplierIds, List<string> branchFilters, string customerCode, string companyCode)
        {
            var user =new User();
            user.Company = companyCode;
            string query = string.Format(@"SELECT VSL.SUB_CODE as SubCode,VSL.ACC_CODE as AccCode, INITCAP(VSL.SUB_EDESC) Description, VSL.VOUCHER_DATE as VoucherDate, SUM(NVL(FN_CONVERT_CURRENCY(NVL(VSL.CR_AMOUNT,0),'NRS',VSL.VOUCHER_DATE),0)) AMOUNT, TO_NUMBER( CS.SUPPLIER_CODE) as Code
                    ,TO_NUMBER(REPLACE(CS.MASTER_SUPPLIER_CODE,'.','')) as MasterCode,TO_NUMBER(REPLACE(CS.PRE_SUPPLIER_CODE,'.',''))  as PreCode
                    FROM V$VIRTUAL_SUB_LEDGER VSL, IP_SUPPLIER_SETUP CS
                    WHERE VSL.DELETED_FLAG='N' 
                    AND VSL.SUB_LEDGER_FLAG='S'
                  --  AND VSL.Company_Code='{1}'
                    AND TRIM( VSL.SUB_CODE) = TRIM(CS.LINK_SUB_CODE)
                    AND VSL.CR_AMOUNT > 0  
                     AND VSL.COMPANY_CODE=CS.COMPANY_CODE 
                    AND VSL.POSTED_BY IS NOT NULL
                    AND TO_DATE(TO_CHAR(VSL.VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{0}', 'YYYY-MM-DD')", asOnDate, user != null ? user.Company : "01");

            if (branchFilters != null && branchFilters.Count > 0)
            {
                query += string.Format(@" AND VSL.BRANCH_CODE IN ('{0}')", string.Join("','", branchFilters));
            }

            if (masterSupplierIds != null && masterSupplierIds.Count > 0)
            {
                string subQuery = @" AND TRIM(VSL.SUB_CODE) IN(SELECT TRIM(LINK_SUB_CODE)FROM IP_SUPPLIER_SETUP WHERE";
                var count = 1;
                foreach (var subQueryItem in masterSupplierIds)
                {


                    if (count > 1)
                    {
                        subQuery += string.Format(@" OR MASTER_SUPPLIER_CODE LIKE '{0}%'", subQueryItem);
                    }
                    else
                    {
                        subQuery += string.Format(@" MASTER_SUPPLIER_CODE LIKE '{0}%'", subQueryItem);
                    }
                    count++;
                }
                subQuery += @" AND DELETED_FLAG = 'N'
                        AND GROUP_SKU_FLAG = 'I')";

                query += subQuery;
            }

            query += @" GROUP BY VSL.SUB_CODE, VSL.ACC_CODE, INITCAP(VSL.SUB_EDESC), VSL.VOUCHER_DATE ,CS.MASTER_SUPPLIER_CODE, CS.PRE_SUPPLIER_CODE,CS.SUPPLIER_CODE
                    ORDER BY INITCAP(VSL.SUB_EDESC), VSL.VOUCHER_DATE";
            var data = this._dbContext.SqlQuery<AgeingDataModel>(query).ToList();
            return data;

        }
        public IEnumerable<AgeingDataModel> GetMobileAgeingChartDebitAmount(string asOnDate, List<string> masterProductIds, List<string> branchFilter)
        {
            
            var subQuery = string.Empty;
            
            string query = @"SELECT VOUCHER_DATE VoucherDate, SUM(NVL(DR_AMOUNT,0)) Amount 
FROM V$VIRTUAL_SUB_LEDGER 
WHERE DELETED_FLAG='N' 
AND SUB_LEDGER_FLAG='S' 
AND DR_AMOUNT > 0  
AND POSTED_BY IS NOT NULL 
AND COMPANY_CODE='01' 
 AND TO_DATE(TO_CHAR(VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{0}', 'YYYY-MM-DD') {1} 
GROUP BY  VOUCHER_DATE 
ORDER BY  VOUCHER_DATE";


            //if (branchFilter != null && branchFilter.Count > 0)
            //{
            //    query += string.Format(@" AND VSL.BRANCH_CODE IN ('{0}')", string.Join("','", branchFilter));
            //}

            if (masterProductIds != null && masterProductIds.Count > 0)
            {

                var count = 1;
                foreach (var subQueryItem in masterProductIds)
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
                if (masterProductIds.Count > 0)
                    subQuery = subQuery + ")";


            }
            var finalQuery = string.Format(query, asOnDate, subQuery);
            var data = this._dbContext.SqlQuery<AgeingDataModel>(finalQuery).ToList();
            return data;

        }

        public IEnumerable<AgeingDataModel> GetMobileAgeingChartCreditAmount(string asOnDate, List<string> masterProductIds, List<string> branchFilter)
        {
            
            var subQuery = string.Empty;
         
            string query = @"SELECT VOUCHER_DATE VoucherDate, SUM(NVL(CR_AMOUNT,0)) Amount 
                        FROM V$VIRTUAL_SUB_LEDGER 
                        WHERE DELETED_FLAG='N' 
                        AND SUB_LEDGER_FLAG='S' 
                        AND CR_AMOUNT > 0  
                        AND POSTED_BY IS NOT NULL 
                        AND COMPANY_CODE='01' 
                         AND TO_DATE(TO_CHAR(VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{0}', 'YYYY-MM-DD') {1}
                        GROUP BY  VOUCHER_DATE 
                        ORDER BY  VOUCHER_DATE";



            if (masterProductIds != null && masterProductIds.Count > 0)
            {

                var count = 1;
                foreach (var subQueryItem in masterProductIds)
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
                if (masterProductIds.Count > 0)
                    subQuery = subQuery + ")";

            }
            var finalQuery = string.Format(query, asOnDate, subQuery);
            var data = this._dbContext.SqlQuery<AgeingDataModel>(finalQuery).ToList();
            return data;

        }

        public IEnumerable<AgeingDataModel> GetMobileAgeingDealerDebitAmount(string asOnDate, List<string> masterCustomerIds, List<string> branchFilter, string AccountCode = "0", string DealerGropCode = "0")
        {

            throw new NotImplementedException();

        }

        public IEnumerable<AgeingDataModel> GetMobileAgeingDealerCreditAmount(string asOnDate, List<string> masterCustomerIds, List<string> branchFilter, string AccountCode = "0", string DealerGropCode = "0")
        {
            throw new NotImplementedException();

        }

        public IEnumerable<testAgeing> testAgeingData(string asOnDate, List<string> masterCustomerIds, List<string> branchFilter)
        {
            throw new NotImplementedException();
        }
    }
}
