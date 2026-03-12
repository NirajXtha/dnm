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
    public class ProductAgeingReportDataService:IAgeingReportDataService
    {
        private IDbContext _dbContext;
        private ICacheManager _cacheManager;
         private IWorkContext _workContext;
        public ProductAgeingReportDataService(IDbContext dbContext, ICacheManager cacheManager,IWorkContext workContext)
        {
            this._dbContext = dbContext;
            this._cacheManager = cacheManager;
            this._workContext = workContext;
        }

        public IEnumerable<AgeingDataModel> GetAgeingDebitAmount(string asOnDate, List<string> masterProductIds)
        {
            string query = string.Format(@"SELECT VSL.SUB_CODE as SubCode, INITCAP(VSL.SUB_EDESC) Description, VSL.VOUCHER_DATE, SUM(NVL(VSL.DR_AMOUNT,0)) AMOUNT,TO_NUMBER(CS.ITEM_CODE) as Code
                        ,TO_NUMBER(REPLACE(CS.MASTER_ITEM_CODE,'.','')) as MasterCode,TO_NUMBER(REPLACE(CS.PRE_ITEM_CODE,'.',''))  as PreCode
                        FROM V$VIRTUAL_SUB_LEDGER VSL, IP_ITEM_MASTER_SETUP CS
                        WHERE VSL.DELETED_FLAG='N' 
                        AND VSL.SUB_LEDGER_FLAG='P'
                        AND TRIM( VSL.SUB_CODE) = TRIM(CS.LINK_SUB_CODE)
                        AND VSL.DR_AMOUNT > 0   
                        AND VSL.POSTED_BY IS NOT NULL
                        AND TO_DATE(TO_CHAR(VSL.VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{0}', 'YYYY-MM-DD')
                        GROUP BY VSL.SUB_CODE, INITCAP(VSL.SUB_EDESC), VSL.VOUCHER_DATE ,CS.MASTER_ITEM_CODE, CS.PRE_ITEM_CODE, CS.ITEM_CODE
                        ORDER BY INITCAP(VSL.SUB_EDESC), VSL.VOUCHER_DATE", asOnDate);
            var data = this._dbContext.SqlQuery<AgeingDataModel>(query).ToList();
            return data;
        }

        public IEnumerable<AgeingDataModel> GetAgeingCreditAmount(string asOnDate, List<string> masterProductIds)
        {
            string query = string.Format(@"SELECT VSL.SUB_CODE as SubCode, INITCAP(VSL.SUB_EDESC) Description, VSL.VOUCHER_DATE, SUM(NVL(VSL.CR_AMOUNT,0)) AMOUNT, TO_NUMBER( CS.ITEM_CODE) as Code
                    ,TO_NUMBER(REPLACE(CS.MASTER_ITEM_CODE,'.','')) as MasterCode,TO_NUMBER(REPLACE(CS.PRE_ITEM_CODE,'.',''))  as PreCode
                    FROM V$VIRTUAL_SUB_LEDGER VSL, IP_ITEM_MASTER_SETUP CS
                    WHERE VSL.DELETED_FLAG='N' 
                    AND VSL.SUB_LEDGER_FLAG='P'
                    AND TRIM( VSL.SUB_CODE) = TRIM(CS.LINK_SUB_CODE)
                    AND VSL.CR_AMOUNT > 0   
                    AND VSL.POSTED_BY IS NOT NULL
                    AND TO_DATE(TO_CHAR(VSL.VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{0}', 'YYYY-MM-DD')
                    GROUP BY VSL.SUB_CODE, INITCAP(VSL.SUB_EDESC), VSL.VOUCHER_DATE ,CS.MASTER_ITEM_CODE, CS.PRE_ITEM_CODE,CS.ITEM_CODE
                    ORDER BY INITCAP(VSL.SUB_EDESC), VSL.VOUCHER_DATE", asOnDate);
            var data = this._dbContext.SqlQuery<AgeingDataModel>(query).ToList();
            return data;

        }

        public IEnumerable<AgeingGroupData> GetGroupData()
        {
            return _dbContext.SqlQuery<AgeingGroupData>(@"SELECT  ITEM_EDESC AS Description,
                                    TO_NUMBER(REPLACE(MASTER_ITEM_CODE,'.','')) as MasterCode,MASTER_ITEM_CODE as MasterCodeWithoutReplace,PRE_ITEM_CODE as PreCodeWithoutReplace, TO_NUMBER(ITEM_CODE) as Code, TO_NUMBER(REPLACE(PRE_ITEM_CODE,'.','')) as PreCode 
                                    FROM IP_ITEM_MASTER_SETUP WHERE GROUP_SKU_FLAG = 'G' AND DELETED_FLAG = 'N'").OrderByDescending(q => q.PreCode); 
        }
        
        public IEnumerable<AgeingGroupData> GetGroupData(List<string> branchFilter)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AgeingDataModel> GetAgeingDebitAmount(string asOnDate, List<string> masterProductIds, List<string> branchFilter, string billwise = null) {
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
            var query=$@"SELECT VSL.ITEM_CODE as SubCode,  INITCAP(CS.ITEM_EDESC) Description, VSL.VOUCHER_DATE as VoucherDate, SUM(NVL(VSL.in_quantity,0)) AMOUNT,TO_NUMBER(VSL.item_code) as Code
                        , TO_NUMBER(REPLACE(CS.MASTER_ITEM_CODE, '.', '')) as MasterCode,TO_NUMBER(REPLACE(CS.PRE_ITEM_CODE, '.', '')) as PreCode
                        FROM v$virtual_stock_wip_ledger VSL, IP_item_master_setup CS
                        WHERE VSL.DELETED_FLAG = 'N'
                        AND VSL.COMPANY_CODE = CS.COMPANY_CODE
                        AND TRIM(VSL.item_code) = TRIM(CS.item_code)
                        AND VSL.in_quantity > 0 ";

            if (branchFilter != null && branchFilter.Count > 0)
            {
                query += string.Format(@" AND VSL.BRANCH_CODE IN ({0})", BranchCode);
            }
            else
            {
                query += string.Format(@" AND VSL.BRANCH_CODE IN ({0})", user.branch_code);

            }
            var customerFilter = string.Empty;
            if (masterProductIds != null && masterProductIds.Count > 0)
            {
                customerFilter = @"select  TRIM(ITEM_CODE) from IP_item_master_setup where (";
                //IF CUSTOMER_SKU_FLAG = G
                foreach (var item in masterProductIds)
                {
                    customerFilter += "master_item_code like  (Select DISTINCT(MASTER_item_CODE) || '%'  from IP_item_master_setup WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN('" + user.Company + "')) OR ";
                }
                customerFilter = customerFilter.Substring(0, customerFilter.Length - 3);
                var CustomerFilterCode = string.Empty;
                foreach (var company in masterProductIds)
                {
                    CustomerFilterCode += $@"'{company}',";
                }
                CustomerFilterCode = CustomerFilterCode == "" ? $@"" : CustomerFilterCode.Remove(CustomerFilterCode.Length - 1);

                //IF CUSTOMER_SKU_FLAG = I                
                customerFilter += $" or (ITEM_CODE in ({CustomerFilterCode}) and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN('" + user.Company + "'))) ";


                query += " AND TRIM(VSL.ITEM_CODE) IN(" + customerFilter + ")";
            }
            query += @" GROUP BY VSL.ITEM_CODE, INITCAP(CS.ITEM_EDESC), VSL.VOUCHER_DATE ,CS.MASTER_ITEM_CODE, CS.PRE_ITEM_CODE
                        ORDER BY INITCAP(CS.ITEM_EDESC), VSL.VOUCHER_DATE";
            var data = _dbContext.SqlQuery<AgeingDataModel>(query);
            return data;

        }

        public IEnumerable<AgeingDataModel> GetAgeingCreditAmount(string asOnDate, List<string> masterProductIds, List<string> branchFilter,string billwise= null)
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
            var query = $@"SELECT VSL.ITEM_CODE as SubCode,  INITCAP(CS.ITEM_EDESC) Description, VSL.VOUCHER_DATE as VoucherDate, SUM(NVL(VSL.out_quantity,0)) AMOUNT,TO_NUMBER(VSL.item_code) as Code
                        , TO_NUMBER(REPLACE(CS.MASTER_ITEM_CODE, '.', '')) as MasterCode,TO_NUMBER(REPLACE(CS.PRE_ITEM_CODE, '.', '')) as PreCode
                        FROM v$virtual_stock_wip_ledger VSL, IP_item_master_setup CS
                        WHERE VSL.DELETED_FLAG = 'N'
                        AND VSL.COMPANY_CODE = CS.COMPANY_CODE
                        AND TRIM(VSL.item_code) = TRIM(CS.item_code)
                        AND VSL.out_quantity > 0 ";
            if (branchFilter != null && branchFilter.Count > 0)
            {
                query += string.Format(@" AND VSL.BRANCH_CODE IN ({0})", BranchCode);
            }
            else
            {
                query += string.Format(@" AND VSL.BRANCH_CODE IN ({0})", user.branch_code);

            }
            var customerFilter = string.Empty;
            if (masterProductIds != null && masterProductIds.Count > 0)
            {
                customerFilter = @"select  TRIM(ITEM_CODE) from IP_item_master_setup where (";
                //IF CUSTOMER_SKU_FLAG = G
                foreach (var item in masterProductIds)
                {
                    customerFilter += "master_item_code like  (Select DISTINCT(MASTER_item_CODE) || '%'  from IP_item_master_setup WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN('" + user.Company + "')) OR ";
                }
                customerFilter = customerFilter.Substring(0, customerFilter.Length - 3);
                var CustomerFilterCode = string.Empty;
                foreach (var company in masterProductIds)
                {
                    CustomerFilterCode += $@"'{company}',";
                }
                CustomerFilterCode = CustomerFilterCode == "" ? $@"" : CustomerFilterCode.Remove(CustomerFilterCode.Length - 1);

                //IF CUSTOMER_SKU_FLAG = I                
                customerFilter += $" or (ITEM_CODE in ({CustomerFilterCode}) and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN('" + user.Company + "'))) ";


                query += " AND TRIM(VSL.ITEM_CODE) IN(" + customerFilter + ")";
            }
            query += @" GROUP BY VSL.ITEM_CODE, INITCAP(CS.ITEM_EDESC), VSL.VOUCHER_DATE ,CS.MASTER_ITEM_CODE, CS.PRE_ITEM_CODE
                        ORDER BY INITCAP(CS.ITEM_EDESC), VSL.VOUCHER_DATE";
            var data = _dbContext.SqlQuery<AgeingDataModel>(query);
            return data;

        }
        public IEnumerable<AgeingDataModel> GetAgeingChartDebitAmount(string asOnDate, List<string> masterProductIds, List<string> branchFilter)
        {

            throw new NotImplementedException();

        }

        public IEnumerable<AgeingDataModel> GetAgeingChartCreditAmount(string asOnDate, List<string> masterProductIds, List<string> branchFilter)
        {

            throw new NotImplementedException();

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
            throw new NotImplementedException();
        }

        public IEnumerable<AgeingDataModel> GetMobileAgeingDebitAmount(string asOnDate, List<string> masterProductIds, List<string> branchFilter, string customerCode, string companyCode)
        {

            throw new NotImplementedException();

        }

        public IEnumerable<AgeingDataModel> GetMobileAgeingCreditAmount(string asOnDate, List<string> masterProductIds, List<string> branchFilter, string customerCode, string companyCode)
        {

            throw new NotImplementedException();

        }
        public IEnumerable<AgeingDataModel> GetMobileAgeingChartDebitAmount(string asOnDate, List<string> masterProductIds, List<string> branchFilter)
        {

            throw new NotImplementedException();

        }

        public IEnumerable<AgeingDataModel> GetMobileAgeingChartCreditAmount(string asOnDate, List<string> masterProductIds, List<string> branchFilter)
        {

            throw new NotImplementedException();

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
