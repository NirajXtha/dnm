using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoErp.Core.Domain;
using NeoErp.Sales.Modules.Services.Models.Ledger;
using NeoErp.Core.Models;
using NeoErp.Sales.Modules.Services.Models;
using NeoErp.Core.Helpers;
using NeoErp.Sales.Modules.Services.Models.Voucher;
using System.Globalization;

namespace NeoErp.Sales.Modules.Services.Services
{
    public class SubsidiaryLedgerService : ISubsidiaryLedger
    {
        private NeoErpCoreEntity _objectEntity;
        public SubsidiaryLedgerService(NeoErpCoreEntity objectEntity)
        {
            this._objectEntity = objectEntity;
        }
        public List<TreeModels> CustomerListAllParentNodes(User userinfo)
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

            string query = $@"SELECT LEVEL,INITCAP(CS.CUSTOMER_EDESC) AS Name,CS.CUSTOMER_CODE AS Code,
                CS.MASTER_CUSTOMER_CODE as MasterCode,CS.PRE_CUSTOMER_CODE as PreCode, CS.BRANCH_CODE as BranchCode,
                  (SELECT COUNT (*)
                              FROM SA_CUSTOMER_SETUP
                             WHERE DELETED_FLAG = 'N' AND COMPANY_CODE=('{userinfo.company_code}') AND PRE_CUSTOMER_CODE = CS.MASTER_CUSTOMER_CODE  AND GROUP_SKU_FLAG='G')  AS Child_rec FROM              SA_CUSTOMER_SETUP CS WHERE CS.DELETED_FLAG = 'N' AND CS.COMPANY_CODE = ('{userinfo.company_code}')  AND LEVEL = 1 
                                START WITH PRE_CUSTOMER_CODE = '00' CONNECT BY NOCYCLE PRIOR MASTER_CUSTOMER_CODE = PRE_CUSTOMER_CODE 
                                 ORDER SIBLINGS BY CUSTOMER_EDESC";
            var customerListNodes = _objectEntity.SqlQuery<TreeModels>(query).ToList();
            return customerListNodes;
        }

        public List<TreeModels> EmployeeListAllParentNodes(User userinfo)
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

            string query = $@"SELECT LEVEL,INITCAP(CS.EMPLOYEE_EDESC) AS Name,CS.EMPLOYEE_CODE AS Code,
                CS.MASTER_EMPLOYEE_CODE as MasterCode,CS.PRE_EMPLOYEE_CODE as PreCode, CS.BRANCH_CODE as BranchCode,
                  (SELECT COUNT (*)
                              FROM HR_EMPLOYEE_SETUP
                             WHERE DELETED_FLAG = 'N' AND COMPANY_CODE=('{userinfo.company_code}') AND PRE_EMPLOYEE_CODE = CS.MASTER_EMPLOYEE_CODE AND GROUP_SKU_FLAG='G')  AS Child_rec FROM    HR_EMPLOYEE_SETUP CS WHERE CS.DELETED_FLAG = 'N' AND CS.COMPANY_CODE = ('{userinfo.company_code}')  AND LEVEL = 1 
                                START WITH PRE_EMPLOYEE_CODE = '00' CONNECT BY PRIOR MASTER_EMPLOYEE_CODE = PRE_EMPLOYEE_CODE 
                                 ORDER SIBLINGS BY EMPLOYEE_EDESC";
            var employeeListNodes = _objectEntity.SqlQuery<TreeModels>(query).ToList();
            return employeeListNodes;
        }

        public List<TreeModels> GetCustomerListByCustCode(string level, string masterCustomerCode, User userinfo)
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

            string query = $@"SELECT INITCAP(CS.CUSTOMER_EDESC) as Name,CS.CUSTOMER_CODE as Code,
                CS.MASTER_CUSTOMER_CODE AS MasterCode,CS.PRE_CUSTOMER_CODE AS PreCode, CS.BRANCH_CODE as BranchCode,(SELECT COUNT (*)
                              FROM SA_CUSTOMER_SETUP
                             WHERE DELETED_FLAG = 'N'  AND COMPANY_CODE=('{userinfo.company_code}')  AND PRE_CUSTOMER_CODE= CS.MASTER_CUSTOMER_CODE  AND GROUP_SKU_FLAG='G') AS Child_rec FROM SA_CUSTOMER_SETUP CS WHERE CS.DELETED_FLAG = 'N' AND CS.COMPANY_CODE = ('{userinfo.company_code}')  AND CS.GROUP_SKU_FLAG = 'G' AND PRE_CUSTOMER_CODE =  (SELECT MASTER_CUSTOMER_CODE FROM SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE='{masterCustomerCode}' and company_code=('{userinfo.company_code}')) ORDER  BY CUSTOMER_EDESC";
            var customerListNodes = _objectEntity.SqlQuery<TreeModels>(query).ToList();
            return customerListNodes;
            //and cs.branch_code=('{userinfo.branch_code}')
        }

        public List<TreeModels> GetEmployeeListByEmpCode(string level, string masterCustomerCode, User userinfo)
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

            string query = $@"SELECT INITCAP(CS.EMPLOYEE_EDESC) as Name,CS.EMPLOYEE_CODE as Code,
                CS.MASTER_EMPLOYEE_CODE AS MasterCode,CS.PRE_EMPLOYEE_CODE AS PreCode, CS.BRANCH_CODE as BranchCode,(SELECT COUNT (*)
                              FROM HR_EMPLOYEE_SETUP
                             WHERE DELETED_FLAG = 'N'  AND COMPANY_CODE=('{userinfo.company_code}')  AND PRE_EMPLOYEE_CODE= CS.MASTER_EMPLOYEE_CODE AND GROUP_SKU_FLAG='G') AS Child_rec FROM HR_EMPLOYEE_SETUP CS WHERE CS.DELETED_FLAG = 'N' AND CS.COMPANY_CODE = ('{userinfo.company_code}') and cs.branch_code=('{userinfo.branch_code}') AND CS.GROUP_SKU_FLAG = 'G' AND PRE_EMPLOYEE_CODE =  (SELECT MASTER_EMPLOYEE_CODE FROM HR_EMPLOYEE_SETUP WHERE EMPLOYEE_CODE='{masterCustomerCode}' and company_code=('{userinfo.company_code}')) ORDER  BY EMPLOYEE_EDESC";
            var customerListNodes = _objectEntity.SqlQuery<TreeModels>(query).ToList();
            return customerListNodes;
        }

        public List<TreeModels> GetListByCode(string Code, User userinfo, string listType)
        {
            string query = "";
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

            Code = Code = "'" + Code.Replace(",", "','") + "'";
            if (listType == "Customer")
            {
                query += $@"SELECT INITCAP(CS.CUSTOMER_EDESC) as Name,CS.CUSTOMER_CODE as Code,
               CS.MASTER_CUSTOMER_CODE as MasterCode,CS.PRE_CUSTOMER_CODE as PreCode, CS.BRANCH_CODE as BranchCode
                 FROM SA_CUSTOMER_SETUP CS
                WHERE CS.DELETED_FLAG = 'N' AND CS.COMPANY_CODE = '{userinfo.company_code}' AND PRE_CUSTOMER_CODE IN ({Code}) AND GROUP_SKU_FLAG = 'I' ORDER  BY CUSTOMER_EDESC";
            }
            else if (listType == "Supplier")
            {
                query += $@"SELECT INITCAP(CS.SUPPLIER_EDESC) as Name,CS.SUPPLIER_CODE as Code,
               CS.MASTER_SUPPLIER_CODE as MasterCode,CS.PRE_SUPPLIER_CODE as PreCode, CS.BRANCH_CODE as BranchCode
                 FROM IP_SUPPLIER_SETUP CS
                WHERE CS.DELETED_FLAG = 'N' AND CS.COMPANY_CODE = '{userinfo.company_code}' AND PRE_SUPPLIER_CODE IN ({Code}) AND GROUP_SKU_FLAG = 'I' ORDER  BY SUPPLIER_EDESC";
            }
            else if (listType == "Employee")
            {
                query += $@"SELECT INITCAP(CS.EMPLOYEE_EDESC) as Name,CS.EMPLOYEE_CODE as Code,
               CS.MASTER_EMPLOYEE_CODE as MasterCode,CS.PRE_EMPLOYEE_CODE as PreCode, CS.BRANCH_CODE as BranchCode
                 FROM HR_EMPLOYEE_SETUP CS
                WHERE CS.DELETED_FLAG = 'N' AND CS.COMPANY_CODE = '{userinfo.company_code}' AND PRE_EMPLOYEE_CODE IN ({Code}) AND GROUP_SKU_FLAG = 'I' ORDER  BY EMPLOYEE_EDESC";
            }
            else if (listType == "mSubledger")
            {
                query += $@"SELECT INITCAP(CS.MISC_EDESC) as Name,CS.MISC_CODE as Code,
               CS.MASTER_MISC_CODE as MasterCode,CS.PRE_MISC_CODE as PreCode, CS.BRANCH_CODE as BranchCode
                 FROM FA_MISC_SUBLEDGER_SETUP CS
                WHERE CS.DELETED_FLAG = 'N' AND CS.COMPANY_CODE = '{userinfo.company_code}' AND PRE_MISC_CODE IN ({Code}) AND GROUP_SKU_FLAG = 'I'  ORDER  BY MISC_EDESC";
            }
            else
            {
                query = $@"SELECT INITCAP(CS.CUSTOMER_EDESC) as Name,CS.CUSTOMER_CODE as Code,
               CS.MASTER_CUSTOMER_CODE as MasterCode,CS.PRE_CUSTOMER_CODE as PreCode, CS.BRANCH_CODE as BranchCode
                 FROM SA_CUSTOMER_SETUP CS
                WHERE CS.DELETED_FLAG = 'N' AND CS.COMPANY_CODE = '{userinfo.company_code}' AND PRE_CUSTOMER_CODE IN ({Code}) AND GROUP_SKU_FLAG = 'I' ORDER  BY CUSTOMER_EDESC";
            }
            var AccountListNodes = _objectEntity.SqlQuery<TreeModels>(query).ToList();
            return AccountListNodes;
        }

        public List<MultiSelectModels> GetListByHierarchy(string Code, User userinfo, string listType)
        {
            string query = "";
            if (listType == "Customer")
            {
                query = $@"SELECT INITCAP(CS.CUSTOMER_EDESC) as Name,CS.CUSTOMER_CODE as Code,
               CS.MASTER_CUSTOMER_CODE AS MasterCode,CS.PRE_CUSTOMER_CODE AS PreCode,TRIM(CS.LINK_SUB_CODE) AS LinkSubCode, CS.BRANCH_CODE as BranchCode
                 FROM SA_CUSTOMER_SETUP CS
                WHERE CS.DELETED_FLAG = 'N' AND CS.COMPANY_CODE = '{userinfo.company_code}'
                AND  PRE_CUSTOMER_CODE IN ( 
                 SELECT CS.MASTER_CUSTOMER_CODE  FROM SA_CUSTOMER_SETUP CS
                                    WHERE CS.DELETED_FLAG = 'N' AND CS.COMPANY_CODE = '{userinfo.company_code}'
                                    AND MASTER_CUSTOMER_CODE LIKE '{Code}%'
                                    START WITH PRE_CUSTOMER_CODE = '00'
                                    CONNECT BY PRIOR MASTER_CUSTOMER_CODE = PRE_CUSTOMER_CODE
                                    )
                ORDER  BY CUSTOMER_EDESC";
            }
            else if (listType == "Supplier")
            {
                query = $@"SELECT INITCAP(CS.SUPPLIER_EDESC) as Name,CS.SUPPLIER_CODE as Code,
               CS.MASTER_SUPPLIER_CODE AS MasterCode,CS.PRE_SUPPLIER_CODE AS PreCode,TRIM(CS.LINK_SUB_CODE) AS LinkSubCode, CS.BRANCH_CODE as BranchCode
                 FROM IP_SUPPLIER_SETUP CS
                WHERE CS.DELETED_FLAG = 'N' AND CS.COMPANY_CODE = '{userinfo.company_code}'
                AND  PRE_SUPPLIER_CODE IN ( 
                 SELECT CS.MASTER_SUPPLIER_CODE  FROM IP_SUPPLIER_SETUP CS
                                    WHERE CS.DELETED_FLAG = 'N' AND CS.COMPANY_CODE = '{userinfo.company_code}'
                                    AND MASTER_SUPPLIER_CODE LIKE '{Code}%'
                                    START WITH PRE_SUPPLIER_CODE = '00'
                                    CONNECT BY PRIOR MASTER_SUPPLIER_CODE = PRE_SUPPLIER_CODE
                                    )
                ORDER  BY SUPPLIER_EDESC";
            }
            else if (listType == "Employee")
            {
                query = $@"SELECT INITCAP(CS.EMPLOYEE_EDESC) as Name,CS.EMPLOYEE_CODE as Code,
               CS.MASTER_EMPLOYEE_CODE AS MasterCode,CS.PRE_EMPLOYEE_CODE AS PreCode,TRIM(CS.LINK_SUB_CODE) AS LinkSubCode, CS.BRANCH_CODE as BranchCode
                 FROM HR_EMPLOYEE_SETUP CS
                WHERE CS.DELETED_FLAG = 'N' AND CS.COMPANY_CODE = '{userinfo.company_code}'
                AND  PRE_EMPLOYEE_CODE IN ( 
                 SELECT CS.MASTER_EMPLOYEE_CODE  FROM HR_EMPLOYEE_SETUP CS
                                    WHERE CS.DELETED_FLAG = 'N' AND CS.COMPANY_CODE = '{userinfo.company_code}'
                                    AND MASTER_EMPLOYEE_CODE LIKE '{Code}%'
                                    START WITH PRE_EMPLOYEE_CODE = '00'
                                    CONNECT BY PRIOR MASTER_EMPLOYEE_CODE = PRE_EMPLOYEE_CODE
                                    )
                ORDER  BY EMPLOYEE_EDESC";
            }
            else if (listType == "mSubledger")
            {
                query = $@"SELECT INITCAP(CS.MISC_EDESC) as Name,CS.MISC_CODE as Code,
               CS.MASTER_MISC_CODE AS MasterCode,CS.PRE_MISC_CODE AS PreCode,TRIM(CS.LINK_SUB_CODE) AS LinkSubCode
                 FROM FA_MISC_SUBLEDGER_SETUP CS
                WHERE CS.DELETED_FLAG = 'N' AND CS.COMPANY_CODE = '{userinfo.company_code}'
                AND  PRE_MISC_CODE IN ( 
                 SELECT CS.MASTER_MISC_CODE  FROM FA_MISC_SUBLEDGER_SETUP CS
                                    WHERE CS.DELETED_FLAG = 'N' AND CS.COMPANY_CODE = '{userinfo.company_code}'
                                    AND MASTER_MISC_CODE LIKE '{Code}%'
                                    START WITH PRE_MISC_CODE = '00'
                                    CONNECT BY PRIOR MASTER_MISC_CODE = PRE_MISC_CODE
                                    )
                ORDER  BY MISC_EDESC";
            }

            var customerListNodes = _objectEntity.SqlQuery<MultiSelectModels>(query).ToList();
            return customerListNodes;
        }

        public List<TreeModels> GetMiscSubLedgerBySubCode(string level, string masterCustomerCode, User userinfo)
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

            string query = $@"SELECT INITCAP(CS.MISC_EDESC) as Name,CS.MISC_CODE as Code,
                CS.MASTER_MISC_CODE AS MasterCode,CS.PRE_MISC_CODE AS PreCode,(SELECT COUNT (*)
                              FROM FA_MISC_SUBLEDGER_SETUP
                             WHERE DELETED_FLAG = 'N'  AND COMPANY_CODE=('{userinfo.company_code}')  AND PRE_MISC_CODE= CS.MASTER_MISC_CODE AND GROUP_SKU_FLAG='G') AS Child_rec FROM FA_MISC_SUBLEDGER_SETUP CS WHERE CS.DELETED_FLAG = 'N' AND CS.COMPANY_CODE = ('{userinfo.company_code}') AND CS.GROUP_SKU_FLAG = 'G' AND PRE_MISC_CODE =  (SELECT MASTER_MISC_CODE FROM FA_MISC_SUBLEDGER_SETUP WHERE MISC_CODE='{masterCustomerCode}' and company_code=('{userinfo.company_code}')) ORDER  BY MISC_EDESC";
            var supplierListNodes = _objectEntity.SqlQuery<TreeModels>(query).ToList();
            return supplierListNodes;
        }

        public List<MultiSelectModels> GetParentListByCode(string AccountCode, string listType, User userInfo)
        {
            string query = "";
            if (listType == "Customer")
            {
                query = $@"SELECT DISTINCT CUSTOMER_CODE AS Code,CUSTOMER_EDESC AS Name,MASTER_CUSTOMER_CODE AS MasterCode,PRE_CUSTOMER_CODE AS PreCode
                        FROM SA_CUSTOMER_SETUP WHERE DELETED_FLAG='N' AND COMPANY_CODE='{userInfo.company_code}' START WITH MASTER_CUSTOMER_CODE = (SELECT MASTER_CUSTOMER_CODE FROM SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE= '{AccountCode}' AND COMPANY_CODE='{userInfo.company_code}') CONNECT BY PRIOR PRE_CUSTOMER_CODE = MASTER_CUSTOMER_CODE ORDER BY MASTER_CUSTOMER_CODE ASC";
            }
            else if (listType == "Supplier")
            {
                query = $@"SELECT DISTINCT SUPPLIER_CODE AS Code,SUPPLIER_EDESC AS Name,MASTER_SUPPLIER_CODE AS MasterCode,PRE_SUPPLIER_CODE AS PreCode
                        FROM IP_SUPPLIER_SETUP WHERE DELETED_FLAG='N' AND COMPANY_CODE='{userInfo.company_code}' START WITH MASTER_SUPPLIER_CODE = (SELECT MASTER_SUPPLIER_CODE FROM IP_SUPPLIER_SETUP WHERE SUPPLIER_CODE= '{AccountCode}' AND COMPANY_CODE='{userInfo.company_code}') CONNECT BY PRIOR PRE_SUPPLIER_CODE = MASTER_SUPPLIER_CODE ORDER BY MASTER_SUPPLIER_CODE ASC";
            }
            else if (listType == "Employee")
            {
                query = $@"SELECT DISTINCT EMPLOYEE_CODE AS Code,EMPLOYEE_EDESC AS Name,MASTER_EMPLOYEE_CODE AS MasterCode,PRE_EMPLOYEE_CODE AS PreCode
                        FROM HR_EMPLOYEE_SETUP WHERE DELETED_FLAG='N' AND COMPANY_CODE='{userInfo.company_code}' START WITH MASTER_EMPLOYEE_CODE = (SELECT MASTER_EMPLOYEE_CODE FROM HR_EMPLOYEE_SETUP WHERE EMPLOYEE_CODE= '{AccountCode}' AND COMPANY_CODE='{userInfo.company_code}') CONNECT BY PRIOR PRE_EMPLOYEE_CODE = MASTER_EMPLOYEE_CODE ORDER BY MASTER_EMPLOYEE_CODE ASC";
            }
            else if (listType == "mSubledger")
            {
                query = $@"SELECT DISTINCT MISC_CODE AS Code,MISC_EDESC AS Name,MASTER_MISC_CODE AS MasterCode,PRE_MISC_CODE AS PreCode
                        FROM FA_MISC_SUBLEDGER_SETUP WHERE DELETED_FLAG='N' AND COMPANY_CODE='{userInfo.company_code}' START WITH MASTER_MISC_CODE = (SELECT MASTER_MISC_CODE FROM FA_MISC_SUBLEDGER_SETUP WHERE MISC_CODE= '{AccountCode}' AND COMPANY_CODE='{userInfo.company_code}') CONNECT BY PRIOR PRE_MISC_CODE = MASTER_MISC_CODE ORDER BY MASTER_MISC_CODE ASC";
            }

            return _objectEntity.SqlQuery<MultiSelectModels>(query).ToList();
        }

        public List<MultiSelectModels> GetSubsidiaryCustomers(User userinfo)
        {
            string query = $@"SELECT LEVEL,INITCAP(CS.CUSTOMER_EDESC) AS Name, CS.CUSTOMER_CODE AS Code,CS.GROUP_SKU_FLAG,
                CS.MASTER_CUSTOMER_CODE as MasterCode,CS.PRE_CUSTOMER_CODE as PreCode,CS.LINK_SUB_CODE AS LinkSubCode, CS.BRANCH_CODE as BranchCode,
                  (SELECT COUNT(*) FROM SA_CUSTOMER_SETUP WHERE DELETED_FLAG = 'N' and PRE_CUSTOMER_CODE = CS.MASTER_CUSTOMER_CODE)
                AS Child_rec FROM SA_CUSTOMER_SETUP CS
                WHERE CS.DELETED_FLAG = 'N' AND CS.COMPANY_CODE = ('{userinfo.company_code}')
                START WITH PRE_CUSTOMER_CODE = '00'
                CONNECT BY NOCYCLE PRIOR MASTER_CUSTOMER_CODE = PRE_CUSTOMER_CODE
                ORDER SIBLINGS BY CUSTOMER_EDESC";

            var customerListNodes = _objectEntity.SqlQuery<MultiSelectModels>(query).Where(q => q.Child_rec > 0).ToList();
            return customerListNodes;
        }

        public List<MultiSelectModels> GetSubsidiaryEmployees(User userinfo)
        {

            string query = $@"SELECT LEVEL,INITCAP(CS.EMPLOYEE_EDESC) AS Name, CS.EMPLOYEE_CODE AS Code,CS.GROUP_SKU_FLAG,
                CS.MASTER_EMPLOYEE_CODE as MasterCode,CS.PRE_EMPLOYEE_CODE as PreCode,CS.LINK_SUB_CODE AS LinkSubCode, CS.BRANCH_CODE as BranchCode,
                  (SELECT COUNT(*) FROM HR_EMPLOYEE_SETUP WHERE DELETED_FLAG = 'N' and PRE_EMPLOYEE_CODE = CS.MASTER_EMPLOYEE_CODE)
                AS Child_rec FROM HR_EMPLOYEE_SETUP CS
                WHERE CS.DELETED_FLAG = 'N' AND CS.COMPANY_CODE = ({userinfo.company_code})
                START WITH PRE_EMPLOYEE_CODE = '00'
                CONNECT BY PRIOR MASTER_EMPLOYEE_CODE = PRE_EMPLOYEE_CODE
                ORDER SIBLINGS BY EMPLOYEE_EDESC";

            var customerListNodes = _objectEntity.SqlQuery<MultiSelectModels>(query).Where(q => q.Child_rec > 0).ToList();
            return customerListNodes;
        }

        public List<MultiSelectModels> GetSubsidiaryMSubLedger(User userinfo)
        {
            string query = $@"SELECT LEVEL,INITCAP(CS.MISC_EDESC) AS Name, CS.MISC_CODE AS Code,CS.GROUP_SKU_FLAG,
                CS.MASTER_MISC_CODE as MasterCode,CS.PRE_MISC_CODE as PreCode,CS.LINK_SUB_CODE AS LinkSubCode,
                  (SELECT COUNT(*) FROM FA_MISC_SUBLEDGER_SETUP WHERE DELETED_FLAG = 'N' and PRE_MISC_CODE = CS.MASTER_MISC_CODE)
                AS Child_rec FROM FA_MISC_SUBLEDGER_SETUP CS
                WHERE CS.DELETED_FLAG = 'N' AND CS.COMPANY_CODE = ({userinfo.company_code})
                START WITH PRE_MISC_CODE = '00'
                CONNECT BY PRIOR MASTER_MISC_CODE = PRE_MISC_CODE
                ORDER SIBLINGS BY MISC_EDESC";

            var customerListNodes = _objectEntity.SqlQuery<MultiSelectModels>(query).Where(q => q.Child_rec > 0).ToList();
            return customerListNodes;
        }

        public List<MultiSelectModels> GetSubsidiarySuppliers(User userinfo)
        {

            string query = $@"SELECT LEVEL,INITCAP(CS.SUPPLIER_EDESC) AS Name, CS.SUPPLIER_CODE AS Code,CS.GROUP_SKU_FLAG,
                CS.MASTER_SUPPLIER_CODE as MasterCode,CS.PRE_SUPPLIER_CODE as PreCode,CS.LINK_SUB_CODE AS LinkSubCode, CS.BRANCH_CODE as BranchCode,
                  (SELECT COUNT(*) FROM IP_SUPPLIER_SETUP WHERE DELETED_FLAG = 'N' and PRE_SUPPLIER_CODE = CS.MASTER_SUPPLIER_CODE)
                AS Child_rec FROM IP_SUPPLIER_SETUP CS
                WHERE CS.DELETED_FLAG = 'N' AND CS.COMPANY_CODE = ({ userinfo.company_code})
                START WITH PRE_SUPPLIER_CODE = '00'
                CONNECT BY PRIOR MASTER_SUPPLIER_CODE = PRE_SUPPLIER_CODE
                ORDER SIBLINGS BY SUPPLIER_EDESC";

            var customerListNodes = _objectEntity.SqlQuery<MultiSelectModels>(query).Where(q => q.Child_rec > 0).ToList();
            return customerListNodes;
        }

        public List<TreeModels> GetSupplierListBySupCode(string level, string masterCustomerCode, User userinfo)
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

            string query = $@"SELECT INITCAP(CS.SUPPLIER_EDESC) as Name,CS.SUPPLIER_CODE as Code,
                CS.MASTER_SUPPLIER_CODE AS MasterCode,CS.PRE_SUPPLIER_CODE AS PreCode, CS.BRANCH_CODE as BranchCode,(SELECT COUNT (*)
                              FROM IP_SUPPLIER_SETUP
                             WHERE DELETED_FLAG = 'N'  AND COMPANY_CODE=('{userinfo.company_code}')  AND PRE_SUPPLIER_CODE= CS.MASTER_SUPPLIER_CODE AND GROUP_SKU_FLAG='G') AS Child_rec FROM IP_SUPPLIER_SETUP CS WHERE CS.DELETED_FLAG = 'N' AND CS.COMPANY_CODE = ('{userinfo.company_code}') and cs.branch_code=('{userinfo.branch_code}') AND CS.GROUP_SKU_FLAG = 'G' AND PRE_SUPPLIER_CODE =  (SELECT MASTER_SUPPLIER_CODE FROM IP_SUPPLIER_SETUP WHERE SUPPLIER_CODE='{masterCustomerCode}' and company_code=('{userinfo.company_code}')) ORDER  BY SUPPLIER_EDESC";
            var supplierListNodes = _objectEntity.SqlQuery<TreeModels>(query).ToList();
            return supplierListNodes;
        }

        public List<TreeModels> MisSubLedgerListAllParentNodes(User userinfo)
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

            string query = $@"SELECT LEVEL,INITCAP(CS.MISC_EDESC) AS Name,CS.MISC_CODE AS Code,
                CS.MASTER_MISC_CODE as MasterCode,CS.PRE_MISC_CODE as PreCode, 
                  (SELECT COUNT (*)
                              FROM FA_MISC_SUBLEDGER_SETUP
                             WHERE DELETED_FLAG = 'N' AND COMPANY_CODE=('{userinfo.company_code}') AND PRE_MISC_CODE = CS.MASTER_MISC_CODE AND GROUP_SKU_FLAG='G')  AS Child_rec FROM FA_MISC_SUBLEDGER_SETUP CS WHERE CS.DELETED_FLAG = 'N' AND CS.COMPANY_CODE = ('{userinfo.company_code}')  AND LEVEL = 1 
                                START WITH PRE_MISC_CODE = '00' CONNECT BY PRIOR MASTER_MISC_CODE = PRE_MISC_CODE 
                                 ORDER SIBLINGS BY MISC_EDESC";
            var employeeListNodes = _objectEntity.SqlQuery<TreeModels>(query).ToList();
            return employeeListNodes;
        }

        public List<TreeModels> SupplierListAllParentNodes(User userinfo)
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

            string query = $@"SELECT LEVEL,INITCAP(CS.SUPPLIER_EDESC) AS Name,CS.SUPPLIER_CODE AS Code,
                CS.MASTER_SUPPLIER_CODE as MasterCode,CS.PRE_SUPPLIER_CODE as PreCode, CS.BRANCH_CODE as BranchCode,
                  (SELECT COUNT (*)
                              FROM IP_SUPPLIER_SETUP
                             WHERE DELETED_FLAG = 'N' AND COMPANY_CODE=('{userinfo.company_code}') AND PRE_SUPPLIER_CODE = CS.MASTER_SUPPLIER_CODE AND GROUP_SKU_FLAG='G')  AS Child_rec FROM              IP_SUPPLIER_SETUP CS WHERE CS.DELETED_FLAG = 'N' AND CS.COMPANY_CODE = ('{userinfo.company_code}')  AND LEVEL = 1 
                                START WITH PRE_SUPPLIER_CODE = '00' CONNECT BY PRIOR MASTER_SUPPLIER_CODE = PRE_SUPPLIER_CODE 
                                 ORDER SIBLINGS BY SUPPLIER_EDESC";
            var supplierListNodes = _objectEntity.SqlQuery<TreeModels>(query).ToList();
            return supplierListNodes;
        }

        public List<VoucherDetailModel> GetSubsidiaryVoucherDetails(ReportFiltersModel reportFilters, string formDate, string toDate, string AccountCode, User userinfo, string linkSubCode, string BranchCode = null, string groupSkuFlag = "", string listType = "", string MasterCode = "", string actionName = "")
        {
            string Query = string.Empty;
            string postedGenericQuery = string.Empty;
            string startDate = DateTime.Parse(formDate).ToString("dd-MMM-yyyy");
            string endDate = DateTime.Parse(toDate).ToString("dd-MMM-yyyy");
            var companyCode = string.Empty;
            var branchesCode = string.Empty;
            foreach (var branch in reportFilters.BranchFilter)
            {
                branchesCode += $@"'{branch}',";
            }
            branchesCode = branchesCode == "" ? $@"'{userinfo.branch_code}'" : branchesCode.Remove(branchesCode.Length - 1);
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }
            companyCode = companyCode == "" ? $@"'{userinfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);


            if (reportFilters.PostedGenericFilter.Count > 0)
            {
                if (reportFilters.PostedGenericFilter.FirstOrDefault() == "DP")
                {
                    postedGenericQuery = $@" AND C.POSTED_BY IS NOT NULL";
                }
            }
            var grouplist = new List<VoucherDetailModel>();
            if (actionName == "dblclick")
            {
                if (listType == "Customer")
                {
                    grouplist = GetSubsidiaryDblClickForCustomer(startDate, endDate, companyCode, MasterCode, branchesCode);

                }
                else if (listType == "Supplier")
                {
                    grouplist = GetSubsidiaryDblClickForSupplier(startDate, endDate, companyCode, MasterCode, branchesCode);

                }
                else if (listType == "Employee")
                {
                    grouplist = GetSubsidiaryDblClickForEmployee(startDate, endDate, companyCode, MasterCode, branchesCode);

                }
                else
                {
                    grouplist = GetSubsidiaryDblClickFormSubledger(startDate, endDate, companyCode, MasterCode, branchesCode);

                }
                var individualList = GetSubsidiaryIndividual(reportFilters, postedGenericQuery, companyCode, AccountCode, startDate, endDate, branchesCode, listType);
                grouplist.AddRange(individualList);
                return grouplist;

            }
            else
            {
                AccountCode = "'" + linkSubCode.Replace(",", "','") + "'";
                if (groupSkuFlag == "I")
                {
                    Query = $@" SELECT DISTINCT A.SUB_EDESC,A.ACC_CODE,
                            NVL(SUM((SELECT NVL(SUM(FN_CONVERT_CURRENCY(ROUND(NVL(C.DR_AMOUNT,0),2) * NVL(C.EXCHANGE_RATE,1),'NRS', C.VOUCHER_DATE)),0) - NVL(SUM(FN_CONVERT_CURRENCY(ROUND(NVL(C.CR_AMOUNT,0),2) * NVL(C.EXCHANGE_RATE,1),'NRS', C.VOUCHER_DATE)),0) 
                            FROM V$VIRTUAL_SUB_LEDGER C 
                            WHERE C.COMPANY_CODE = A.COMPANY_CODE AND C.BRANCH_CODE = A.BRANCH_CODE AND C.SERIAL_NO = A.SERIAL_NO AND C.SUB_CODE = A.SUB_CODE 
                            AND C.ACC_CODE = A.ACC_CODE AND C.FORM_CODE = A.FORM_CODE AND C.VOUCHER_NO = A.VOUCHER_NO 
                            AND C.FORM_CODE='0')),0) + NVL(SUM((SELECT NVL(SUM(FN_CONVERT_CURRENCY(ROUND(NVL(C.DR_AMOUNT,0),2) * NVL(C.EXCHANGE_RATE,1),'NRS', C.VOUCHER_DATE)),0) - 
                            NVL(SUM(FN_CONVERT_CURRENCY(ROUND(NVL(C.CR_AMOUNT,0),2) * NVL(C.EXCHANGE_RATE,1),'NRS', C.VOUCHER_DATE)),0) FROM V$VIRTUAL_SUB_LEDGER C 
                            WHERE C.COMPANY_CODE = A.COMPANY_CODE AND C.BRANCH_CODE = A.BRANCH_CODE AND C.SERIAL_NO = A.SERIAL_NO AND C.SUB_CODE = A.SUB_CODE 
                            AND C.ACC_CODE = A.ACC_CODE AND C.FORM_CODE = A.FORM_CODE AND C.VOUCHER_NO = A.VOUCHER_NO 
                            AND TO_DATE(TO_CHAR(C.VOUCHER_DATE,'DD-MON-YYYY')) < TO_DATE('{startDate}') 
                            AND C.FORM_CODE <> '0' {postedGenericQuery} )),0) OPEN_AMOUNT, 
                            SUM((SELECT NVL(SUM(FN_CONVERT_CURRENCY(ROUND(NVL(C.DR_AMOUNT,0),2) * NVL(C.EXCHANGE_RATE,1),'NRS', C.VOUCHER_DATE)),0) FROM V$VIRTUAL_SUB_LEDGER C 
                            WHERE C.COMPANY_CODE = A.COMPANY_CODE AND C.BRANCH_CODE = A.BRANCH_CODE AND C.SERIAL_NO = A.SERIAL_NO AND C.SUB_CODE = A.SUB_CODE 
                            AND C.ACC_CODE = A.ACC_CODE AND C.FORM_CODE = A.FORM_CODE AND C.VOUCHER_NO = A.VOUCHER_NO 
                            AND TO_DATE(TO_CHAR(C.VOUCHER_DATE,'DD-MON-YYYY')) >= TO_DATE('{startDate}') 
                            AND TO_DATE(TO_CHAR(C.VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{endDate}') 
                            AND C.FORM_CODE <> '0' {postedGenericQuery} AND A.COMPANY_CODE=({companyCode})  
                            AND A.BRANCH_CODE IN ({branchesCode})  )) dr_amount, SUM((SELECT NVL(SUM(FN_CONVERT_CURRENCY(ROUND(NVL(C.CR_AMOUNT,0),2) * NVL(C.EXCHANGE_RATE,1),'NRS', C.VOUCHER_DATE)),0) 
                            FROM V$VIRTUAL_SUB_LEDGER C WHERE C.COMPANY_CODE = A.COMPANY_CODE AND C.BRANCH_CODE = A.BRANCH_CODE AND C.SERIAL_NO = A.SERIAL_NO 
                            AND C.SUB_CODE = A.SUB_CODE AND C.ACC_CODE = A.ACC_CODE AND C.FORM_CODE = A.FORM_CODE AND C.VOUCHER_NO = A.VOUCHER_NO 
                            AND TO_DATE(TO_CHAR(C.VOUCHER_DATE,'DD-MON-YYYY')) >= TO_DATE('{startDate}') 
                            AND TO_DATE(TO_CHAR(C.VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{endDate}') 
                            AND C.FORM_CODE <> '0' {postedGenericQuery} AND A.COMPANY_CODE IN ({companyCode})  
                            AND A.BRANCH_CODE IN ({branchesCode})  )) cr_amount FROM V$VIRTUAL_SUB_LEDGER A WHERE A.DELETED_FLAG='N' AND TRIM(A.SUB_CODE) IN({AccountCode})  
                            AND TO_DATE(TO_CHAR(A.VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{endDate}') 
                            AND A.COMPANY_CODE IN ({companyCode})  
                            AND A.BRANCH_CODE IN ({branchesCode})";
                    if (reportFilters.LedgerFilter.Count > 0)
                    {
                        Query = Query + string.Format(@" AND A.ACC_CODE  IN  ('{0}')", string.Join("','", reportFilters.LedgerFilter).ToString());
                    }
                    Query += $@" GROUP BY A.SUB_EDESC,A.ACC_CODE ORDER BY A.SUB_EDESC";
                }

                var list = _objectEntity.SqlQuery<VoucherDetailModel>(Query).ToList();
                var filtered = list.Where(x => x.dr_amount != 0 || x.cr_amount != 0).ToList();
                return filtered;
            }
        }

        #region commented lines
        //        else
        //                {
        //                    var LinkSubCodeStringFormat = "";
        //                    if (listType == "Customer")
        //                    {
        //                        string customQuery = $@" SELECT TRIM(LINK_SUB_CODE) FROM SA_CUSTOMER_SETUP WHERE MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%' from 
        //                            SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = {AccountCode} AND COMPANY_CODE IN('01') AND GROUP_SKU_FLAG='G')";
        //        var linkSubCodes = _objectEntity.SqlQuery<string>(customQuery).ToList();
        //        LinkSubCodeStringFormat = String.Join("','", linkSubCodes.Where(s => !string.IsNullOrEmpty(s)));
        //                    }
        //                    else if (listType == "Supplier")
        //                    {
        //                        string customQuery = $@" SELECT TRIM(LINK_SUB_CODE) FROM IP_SUPPLIER_SETUP WHERE MASTER_SUPPLIER_CODE like (Select DISTINCT(MASTER_SUPPLIER_CODE) || '%' from 
        //                            IP_SUPPLIER_SETUP WHERE SUPPLIER_CODE = {AccountCode} AND COMPANY_CODE IN('01') AND GROUP_SKU_FLAG='G')";
        //    var linkSubCodes = _objectEntity.SqlQuery<string>(customQuery).ToList();
        //    LinkSubCodeStringFormat = String.Join("','", linkSubCodes.Where(s => !string.IsNullOrEmpty(s)));
        //                    }
        //                    else if (listType == "Employee")
        //                    {
        //                        string customQuery = $@" SELECT TRIM(LINK_SUB_CODE) FROM HR_EMPLOYEE_SETUP WHERE MASTER_EMPLOYEE_CODE like (Select DISTINCT(MASTER_EMPLOYEE_CODE) || '%' from 
        //                            HR_EMPLOYEE_SETUP WHERE EMPLOYEE_CODE = {AccountCode} AND COMPANY_CODE IN('01') AND GROUP_SKU_FLAG='G')";
        //var linkSubCodes = _objectEntity.SqlQuery<string>(customQuery).ToList();
        //LinkSubCodeStringFormat = String.Join("','", linkSubCodes.Where(s => !string.IsNullOrEmpty(s)));
        //                    }
        //                    else if (listType == "mSubLedger")
        //                    {
        //                        string customQuery = $@" SELECT TRIM(LINK_SUB_CODE) FROM FA_MISC_SUBLEDGER_SETUP WHERE MASTER_MISC_CODE like (Select DISTINCT(MASTER_MISC_CODE) || '%' from 
        //                            FA_MISC_SUBLEDGER_SETUP WHERE MISC_CODE = {AccountCode} AND COMPANY_CODE IN('01') AND GROUP_SKU_FLAG='G')";
        //var linkSubCodes = _objectEntity.SqlQuery<string>(customQuery).ToList();
        //LinkSubCodeStringFormat = String.Join("','", linkSubCodes.Where(s => !string.IsNullOrEmpty(s)));
        //                    }
        //                    Query = $@" SELECT DISTINCT A.SUB_EDESC,A.ACC_CODE,
        //                            NVL(SUM((SELECT NVL(SUM(FN_CONVERT_CURRENCY(ROUND(NVL(C.DR_AMOUNT,0),2) * NVL(C.EXCHANGE_RATE,1),'NRS', C.VOUCHER_DATE)),0) - NVL(SUM(FN_CONVERT_CURRENCY(ROUND(NVL(C.CR_AMOUNT,0),2) * NVL(C.EXCHANGE_RATE,1),'NRS', C.VOUCHER_DATE)),0) 
        //                            FROM V$VIRTUAL_SUB_LEDGER C 
        //                            WHERE C.COMPANY_CODE = A.COMPANY_CODE AND C.BRANCH_CODE = A.BRANCH_CODE AND C.SERIAL_NO = A.SERIAL_NO AND C.SUB_CODE = A.SUB_CODE 
        //                            AND C.ACC_CODE = A.ACC_CODE AND C.FORM_CODE = A.FORM_CODE AND C.VOUCHER_NO = A.VOUCHER_NO 
        //                            AND C.FORM_CODE='0')),0) + NVL(SUM((SELECT NVL(SUM(FN_CONVERT_CURRENCY(ROUND(NVL(C.DR_AMOUNT,0),2) * NVL(C.EXCHANGE_RATE,1),'NRS', C.VOUCHER_DATE)),0) - 
        //                            NVL(SUM(FN_CONVERT_CURRENCY(ROUND(NVL(C.CR_AMOUNT,0),2) * NVL(C.EXCHANGE_RATE,1),'NRS', C.VOUCHER_DATE)),0) FROM V$VIRTUAL_SUB_LEDGER C 
        //                            WHERE C.COMPANY_CODE = A.COMPANY_CODE AND C.BRANCH_CODE = A.BRANCH_CODE AND C.SERIAL_NO = A.SERIAL_NO AND C.SUB_CODE = A.SUB_CODE 
        //                            AND C.ACC_CODE = A.ACC_CODE AND C.FORM_CODE = A.FORM_CODE AND C.VOUCHER_NO = A.VOUCHER_NO 
        //                            AND TO_DATE(TO_CHAR(C.VOUCHER_DATE,'DD-MON-YYYY')) < TO_DATE('{startDate}') 
        //                            AND C.FORM_CODE <> '0' {postedGenericQuery} )),0) OPEN_AMOUNT, 
        //                            SUM((SELECT NVL(SUM(FN_CONVERT_CURRENCY(ROUND(NVL(C.DR_AMOUNT,0),2) * NVL(C.EXCHANGE_RATE,1),'NRS', C.VOUCHER_DATE)),0) FROM V$VIRTUAL_SUB_LEDGER C 
        //                            WHERE C.COMPANY_CODE = A.COMPANY_CODE AND C.BRANCH_CODE = A.BRANCH_CODE AND C.SERIAL_NO = A.SERIAL_NO AND C.SUB_CODE = A.SUB_CODE 
        //                            AND C.ACC_CODE = A.ACC_CODE AND C.FORM_CODE = A.FORM_CODE AND C.VOUCHER_NO = A.VOUCHER_NO 
        //                             AND TO_DATE(TO_CHAR(C.VOUCHER_DATE,'DD-MON-YYYY')) >= TO_DATE('{startDate}') 
        //                            AND TO_DATE(TO_CHAR(C.VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{endDate}') 
        //                            AND C.FORM_CODE <> '0' {postedGenericQuery} AND A.COMPANY_CODE=({companyCode})  
        //                            AND A.BRANCH_CODE=('{userinfo.branch_code}')  )) dr_amount, SUM((SELECT NVL(SUM(FN_CONVERT_CURRENCY(ROUND(NVL(C.CR_AMOUNT,0),2) * NVL(C.EXCHANGE_RATE,1),'NRS', C.VOUCHER_DATE)),0) 
        //                            FROM V$VIRTUAL_SUB_LEDGER C WHERE C.COMPANY_CODE = A.COMPANY_CODE AND C.BRANCH_CODE = A.BRANCH_CODE AND C.SERIAL_NO = A.SERIAL_NO 
        //                            AND C.SUB_CODE = A.SUB_CODE AND C.ACC_CODE = A.ACC_CODE AND C.FORM_CODE = A.FORM_CODE AND C.VOUCHER_NO = A.VOUCHER_NO 
        //                              AND TO_DATE(TO_CHAR(C.VOUCHER_DATE,'DD-MON-YYYY')) >= TO_DATE('{startDate}') 
        //                            AND TO_DATE(TO_CHAR(C.VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{endDate}') 
        //                            AND C.FORM_CODE <> '0' {postedGenericQuery} AND A.COMPANY_CODE IN ({companyCode})  
        //                            AND A.BRANCH_CODE=('{userinfo.branch_code}')  )) cr_amount FROM V$VIRTUAL_SUB_LEDGER A WHERE A.DELETED_FLAG='N' AND TRIM(A.SUB_CODE) IN('{LinkSubCodeStringFormat}')  
        //                              AND TO_DATE(TO_CHAR(A.VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{endDate}') 
        //                            AND A.COMPANY_CODE IN ({companyCode})  
        //                            AND A.BRANCH_CODE=('{userinfo.branch_code}')";
        //                    if (reportFilters.LedgerFilter.Count > 0)
        //                    {
        //                        Query = Query + string.Format(@" AND A.ACC_CODE  IN  ('{0}')", string.Join("','", reportFilters.LedgerFilter).ToString());
        //                    }
        //                    Query += $@" GROUP BY A.SUB_EDESC,A.ACC_CODE ORDER BY A.SUB_EDESC";
        //                }

        #endregion

        private List<VoucherDetailModel> GetSubsidiaryIndividual(ReportFiltersModel reportFilters, string postedGenericQuery, string companyCode, string AccountCode, string startDate, string endDate, string branch_code, string listType)
        {
            var LinkSubCodeStringFormat = string.Empty;
            var customQuery = string.Empty;
            var Query = string.Empty;
            if (listType == "Customer")
            {
                customQuery = $@"SELECT TRIM(LINK_SUB_CODE)
                                 FROM SA_CUSTOMER_SETUP
                                WHERE DELETED_FLAG = 'N' AND COMPANY_CODE IN {companyCode} AND PRE_CUSTOMER_CODE IN (SELECT MASTER_CUSTOMER_CODE FROM SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = {AccountCode}) AND GROUP_SKU_FLAG = 'I' ORDER  BY CUSTOMER_EDESC";
            }
            else if (listType == "Supplier")
            {
                customQuery = $@"SELECT TRIM(LINK_SUB_CODE)
                                 FROM IP_SUPPLIER_SETUP
                                WHERE DELETED_FLAG = 'N' AND COMPANY_CODE IN {companyCode} AND PRE_SUPPLIER_CODE IN (SELECT MASTER_SUPPLIER_CODE FROM IP_SUPPLIER_SETUP WHERE SUPPLIER_CODE = {AccountCode}) AND GROUP_SKU_FLAG = 'I' ORDER  BY SUPPLIER_EDESC";
            }
            else if (listType == "Employee")
            {
                customQuery = $@"SELECT TRIM(LINK_SUB_CODE)
                                 FROM HR_EMPLOYEE_SETUP
                                WHERE DELETED_FLAG = 'N' AND COMPANY_CODE IN {companyCode} AND PRE_EMPLOYEE_CODE IN (SELECT MASTER_EMPLOYEE_CODE FROM HR_EMPLOYEE_SETUP WHERE EMPLOYEE_CODE= {AccountCode}) AND GROUP_SKU_FLAG = 'I' ORDER  BY EMPLOYEE_EDESC";
            }
            else
            {
                customQuery = $@"SELECT TRIM(LINK_SUB_CODE)
                                 FROM FA_MISC_SUBLEDGER_SETUP
                                WHERE DELETED_FLAG = 'N' AND COMPANY_CODE IN {companyCode} AND PRE_MISC_CODE IN (SELECT MASTER_MISC_CODE FROM FA_MISC_SUBLEDGER_SETUP WHERE MISC_CODE= {AccountCode}) AND GROUP_SKU_FLAG = 'I' ORDER  BY MISC_EDESC";
            }

            var linkSubCodes = _objectEntity.SqlQuery<string>(customQuery).ToList();
            LinkSubCodeStringFormat = String.Join("','", linkSubCodes.Where(s => !string.IsNullOrEmpty(s)));
            Query = $@" SELECT A.SUB_EDESC,
                            NVL(SUM((SELECT NVL(SUM(FN_CONVERT_CURRENCY(ROUND(NVL(C.DR_AMOUNT,0),2) * NVL(C.EXCHANGE_RATE,1),'NRS', C.VOUCHER_DATE)),0) - NVL(SUM(FN_CONVERT_CURRENCY(ROUND(NVL(C.CR_AMOUNT,0),2) * NVL(C.EXCHANGE_RATE,1),'NRS', C.VOUCHER_DATE)),0) 
                            FROM V$VIRTUAL_SUB_LEDGER C 
                            WHERE C.COMPANY_CODE = A.COMPANY_CODE AND C.BRANCH_CODE = A.BRANCH_CODE AND C.SERIAL_NO = A.SERIAL_NO AND C.SUB_CODE = A.SUB_CODE 
                            AND C.ACC_CODE = A.ACC_CODE AND C.FORM_CODE = A.FORM_CODE AND C.VOUCHER_NO = A.VOUCHER_NO 
                            AND C.FORM_CODE='0')),0) + NVL(SUM((SELECT NVL(SUM(FN_CONVERT_CURRENCY(ROUND(NVL(C.DR_AMOUNT,0),2) * NVL(C.EXCHANGE_RATE,1),'NRS', C.VOUCHER_DATE)),0) - 
                            NVL(SUM(FN_CONVERT_CURRENCY(ROUND(NVL(C.CR_AMOUNT,0),2) * NVL(C.EXCHANGE_RATE,1),'NRS', C.VOUCHER_DATE)),0) FROM V$VIRTUAL_SUB_LEDGER C 
                            WHERE C.COMPANY_CODE = A.COMPANY_CODE AND C.BRANCH_CODE = A.BRANCH_CODE AND C.SERIAL_NO = A.SERIAL_NO AND C.SUB_CODE = A.SUB_CODE 
                            AND C.ACC_CODE = A.ACC_CODE AND C.FORM_CODE = A.FORM_CODE AND C.VOUCHER_NO = A.VOUCHER_NO 
                            AND TO_DATE(TO_CHAR(C.VOUCHER_DATE,'DD-MON-YYYY')) < TO_DATE('{startDate}') 
                            AND C.FORM_CODE <> '0' {postedGenericQuery} )),0) OPEN_AMOUNT, 
                            SUM((SELECT NVL(SUM(FN_CONVERT_CURRENCY(ROUND(NVL(C.DR_AMOUNT,0),2) * NVL(C.EXCHANGE_RATE,1),'NRS', C.VOUCHER_DATE)),0) FROM V$VIRTUAL_SUB_LEDGER C 
                            WHERE C.COMPANY_CODE = A.COMPANY_CODE AND C.BRANCH_CODE = A.BRANCH_CODE AND C.SERIAL_NO = A.SERIAL_NO AND C.SUB_CODE = A.SUB_CODE 
                            AND C.ACC_CODE = A.ACC_CODE AND C.FORM_CODE = A.FORM_CODE AND C.VOUCHER_NO = A.VOUCHER_NO 
                             AND TO_DATE(TO_CHAR(C.VOUCHER_DATE,'DD-MON-YYYY')) >= TO_DATE('{startDate}') 
                            AND TO_DATE(TO_CHAR(C.VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{endDate}') 
                            AND C.FORM_CODE <> '0' {postedGenericQuery} AND A.COMPANY_CODE=({companyCode})  
                            AND A.BRANCH_CODE IN ({branch_code})  )) dr_amount, SUM((SELECT NVL(SUM(FN_CONVERT_CURRENCY(ROUND(NVL(C.CR_AMOUNT,0),2) * NVL(C.EXCHANGE_RATE,1),'NRS', C.VOUCHER_DATE)),0) 
                            FROM V$VIRTUAL_SUB_LEDGER C WHERE C.COMPANY_CODE = A.COMPANY_CODE AND C.BRANCH_CODE = A.BRANCH_CODE AND C.SERIAL_NO = A.SERIAL_NO 
                            AND C.SUB_CODE = A.SUB_CODE AND C.ACC_CODE = A.ACC_CODE AND C.FORM_CODE = A.FORM_CODE AND C.VOUCHER_NO = A.VOUCHER_NO 
                              AND TO_DATE(TO_CHAR(C.VOUCHER_DATE,'DD-MON-YYYY')) >= TO_DATE('{startDate}') 
                            AND TO_DATE(TO_CHAR(C.VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{endDate}') 
                            AND C.FORM_CODE <> '0' {postedGenericQuery} AND A.COMPANY_CODE IN ({companyCode})  
                            AND A.BRANCH_CODE IN ({branch_code})  )) cr_amount FROM V$VIRTUAL_SUB_LEDGER A WHERE A.DELETED_FLAG='N' AND TRIM(A.SUB_CODE) IN('{LinkSubCodeStringFormat}')  
                              AND TO_DATE(TO_CHAR(A.VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{endDate}') 
                            AND A.COMPANY_CODE IN ({companyCode})  
                            AND A.BRANCH_CODE IN ({branch_code})";
            if (reportFilters.LedgerFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND A.ACC_CODE  IN  ('{0}')", string.Join("','", reportFilters.LedgerFilter).ToString());
            }
            Query += $@" GROUP BY A.SUB_EDESC ORDER BY A.SUB_EDESC";
            var lists = _objectEntity.SqlQuery<VoucherDetailModel>(Query).ToList();
            lists.Select(c => { c.flag = "I"; return c; }).ToList();
            return lists;
        }

        private List<VoucherDetailModel> GetSubsidiaryDblClickForCustomer(string startDate, string endDate, string companyCode, string MasterCode, string BranchCode)
        {
            string Query = string.Empty;
            Query = $@" SELECT A.CUSTOMER_EDESC AS SUB_EDESC ,A.GROUP_SKU_FLAG,A.MASTER_CUSTOMER_CODE, A.TPIN_VAT_NO, A.PAN_NO,A.LINK_SUB_CODE,NVL(0,0) OPEN_AMOUNT,
                            (SELECT SUM(ROUND(NVL(V.DR_AMOUNT,0),2))
                            FROM V$VIRTUAL_SUB_LEDGER V, SA_CUSTOMER_SETUP X  
                            WHERE  V.SUB_CODE =  TRIM(X.LINK_SUB_CODE) 
                            AND X.DELETED_FLAG='N' 
                            AND X.COMPANY_CODE IN({companyCode})  
                            AND X.PRE_CUSTOMER_CODE LIKE A.MASTER_CUSTOMER_CODE||'%' 
                            AND V.COMPANY_CODE= X.COMPANY_CODE 
                            AND V.BRANCH_CODE IN ({BranchCode})    
                            AND TO_DATE(TO_CHAR(V.VOUCHER_DATE,'DD-MON-YYYY')) >=TO_DATE('{startDate}') 
                            AND TO_DATE(TO_CHAR(V.VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{endDate}') 
                            AND V.FORM_CODE <> '0' ) DR_AMOUNT,
                            (SELECT  SUM(ROUND(NVL(V.CR_AMOUNT,0),2))
                            FROM V$VIRTUAL_SUB_LEDGER V, SA_CUSTOMER_SETUP X  
                            WHERE  V.SUB_CODE =  TRIM(X.LINK_SUB_CODE) 
                            AND X.DELETED_FLAG='N' 
                            AND X.COMPANY_CODE IN({companyCode})  
                            AND X.PRE_CUSTOMER_CODE LIKE A.MASTER_CUSTOMER_CODE||'%' 
                            AND V.COMPANY_CODE= X.COMPANY_CODE 
                            AND V.BRANCH_CODE IN ({BranchCode})        
                            AND TO_DATE(TO_CHAR(V.VOUCHER_DATE,'DD-MON-YYYY')) >=TO_DATE('{startDate}') 
                            AND TO_DATE(TO_CHAR(V.VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{endDate}') 
                            AND V.FORM_CODE <> '0' ) CR_AMOUNT FROM SA_CUSTOMER_SETUP  A
                            WHERE DELETED_FLAG='N' 
                            AND COMPANY_CODE IN({companyCode}) 
                            AND PRE_CUSTOMER_CODE = '{MasterCode}'  
                            AND DELETED_FLAG = 'N'
                            ORDER BY GROUP_SKU_FLAG,CUSTOMER_EDESC
                            ";
            var list = _objectEntity.SqlQuery<VoucherDetailModel>(Query).ToList();
            var filtered = list.Where(x => x.dr_amount != 0 && x.dr_amount != null || x.cr_amount != 0 && x.cr_amount != null || x.OPEN_AMOUNT != 0 && x.OPEN_AMOUNT != null).ToList();



            //var filtered = list.Where(x => x.dr_amount != 0 || x.cr_amount != 0).ToList();
            return filtered;

        }

        private List<VoucherDetailModel> GetSubsidiaryDblClickForSupplier(string startDate, string endDate, string companyCode, string MasterCode, string BranchCode)
        {
            string Query = string.Empty;
            Query = $@" SELECT A.SUPPLIER_EDESC AS SUB_EDESC ,A.GROUP_SKU_FLAG,A.MASTER_SUPPLIER_CODE, A.TPIN_VAT_NO,A.LINK_SUB_CODE,NVL(0,0) OPEN_AMOUNT,
                            (SELECT SUM(ROUND(NVL(V.DR_AMOUNT,0),2))
                            FROM V$VIRTUAL_SUB_LEDGER V, IP_SUPPLIER_SETUP X  
                            WHERE  V.SUB_CODE =  TRIM(X.LINK_SUB_CODE) 
                            AND X.DELETED_FLAG='N' 
                            AND X.COMPANY_CODE IN ({companyCode})  
                            AND X.PRE_SUPPLIER_CODE LIKE A.MASTER_SUPPLIER_CODE||'%' 
                            AND V.COMPANY_CODE= X.COMPANY_CODE 
                            AND V.BRANCH_CODE IN ({BranchCode})        
                            AND TO_DATE(TO_CHAR(V.VOUCHER_DATE,'DD-MON-YYYY')) >=TO_DATE('{startDate}') 
                            AND TO_DATE(TO_CHAR(V.VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{endDate}') 
                            AND V.FORM_CODE <> '0' ) DR_AMOUNT,
                            (SELECT  SUM(ROUND(NVL(V.CR_AMOUNT,0),2))
                            FROM V$VIRTUAL_SUB_LEDGER V, IP_SUPPLIER_SETUP X  
                            WHERE  V.SUB_CODE =  TRIM(X.LINK_SUB_CODE) 
                            AND X.DELETED_FLAG='N' 
                            AND X.COMPANY_CODE IN({companyCode})  
                            AND X.PRE_SUPPLIER_CODE LIKE A.MASTER_SUPPLIER_CODE||'%' 
                            AND V.COMPANY_CODE= X.COMPANY_CODE 
                            AND V.BRANCH_CODE IN ({BranchCode})       
                            AND TO_DATE(TO_CHAR(V.VOUCHER_DATE,'DD-MON-YYYY')) >=TO_DATE('{startDate}') 
                            AND TO_DATE(TO_CHAR(V.VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{endDate}') 
                            AND V.FORM_CODE <> '0' ) CR_AMOUNT FROM IP_SUPPLIER_SETUP  A
                            WHERE DELETED_FLAG='N' 
                            AND COMPANY_CODE IN({companyCode}) 
                            AND PRE_SUPPLIER_CODE = '{MasterCode}'  
                            AND DELETED_FLAG = 'N'
                            ORDER BY GROUP_SKU_FLAG,SUPPLIER_EDESC
                            ";
            var list = _objectEntity.SqlQuery<VoucherDetailModel>(Query).ToList();
            var filtered = list.Where(x => x.dr_amount != 0 && x.dr_amount != null || x.cr_amount != 0 && x.cr_amount != null || x.OPEN_AMOUNT != 0 && x.OPEN_AMOUNT != null).ToList();
            //var filtered = list.Where(x => x.dr_amount != 0 || x.cr_amount != 0).ToList();
            return filtered;
        }

        private List<VoucherDetailModel> GetSubsidiaryDblClickForEmployee(string startDate, string endDate, string companyCode, string MasterCode, string BranchCode)
        {
            string Query = string.Empty;
            Query = $@" SELECT A.EMPLOYEE_EDESC AS SUB_EDESC ,A.GROUP_SKU_FLAG,A.MASTER_EMPLOYEE_CODE,A.PAN_NO,A.LINK_SUB_CODE,NVL(0,0) OPEN_AMOUNT,
                            (SELECT SUM(ROUND(NVL(V.DR_AMOUNT,0),2))
                            FROM V$VIRTUAL_SUB_LEDGER V, HR_EMPLOYEE_SETUP X  
                            WHERE  V.SUB_CODE =  TRIM(X.LINK_SUB_CODE) 
                            AND X.DELETED_FLAG='N' 
                            AND X.COMPANY_CODE IN ({companyCode})  
                            AND X.PRE_EMPLOYEE_CODE LIKE A.MASTER_EMPLOYEE_CODE||'%' 
                            AND V.COMPANY_CODE= X.COMPANY_CODE 
                            AND V.BRANCH_CODE IN ({BranchCode})       
                            AND TO_DATE(TO_CHAR(V.VOUCHER_DATE,'DD-MON-YYYY')) >=TO_DATE('{startDate}') 
                            AND TO_DATE(TO_CHAR(V.VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{endDate}') 
                            AND V.FORM_CODE <> '0' ) DR_AMOUNT,
                            (SELECT  SUM(ROUND(NVL(V.CR_AMOUNT,0),2))
                            FROM V$VIRTUAL_SUB_LEDGER V, HR_EMPLOYEE_SETUP X  
                            WHERE  V.SUB_CODE =  TRIM(X.LINK_SUB_CODE) 
                            AND X.DELETED_FLAG='N' 
                            AND X.COMPANY_CODE IN({companyCode})  
                            AND X.PRE_EMPLOYEE_CODE LIKE A.MASTER_EMPLOYEE_CODE||'%' 
                            AND V.COMPANY_CODE= X.COMPANY_CODE 
                            AND V.BRANCH_CODE IN ({BranchCode})    
                            AND TO_DATE(TO_CHAR(V.VOUCHER_DATE,'DD-MON-YYYY')) >=TO_DATE('{startDate}') 
                            AND TO_DATE(TO_CHAR(V.VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{endDate}') 
                            AND V.FORM_CODE <> '0' ) CR_AMOUNT FROM HR_EMPLOYEE_SETUP  A
                            WHERE DELETED_FLAG='N' 
                            AND COMPANY_CODE IN({companyCode}) 
                            AND PRE_EMPLOYEE_CODE = '{MasterCode}'  
                            AND DELETED_FLAG = 'N'
                            ORDER BY GROUP_SKU_FLAG,EMPLOYEE_EDESC
                            ";
            var list = _objectEntity.SqlQuery<VoucherDetailModel>(Query).ToList();
            //var filtered = list.Where(x => x.dr_amount != 0 || x.cr_amount != 0).ToList();
            var filtered = list.Where(x => x.dr_amount != 0 && x.dr_amount != null || x.cr_amount != 0 && x.cr_amount != null || x.OPEN_AMOUNT != 0 && x.OPEN_AMOUNT != null).ToList();
            return filtered;
        }

        private List<VoucherDetailModel> GetSubsidiaryDblClickFormSubledger(string startDate, string endDate, string companyCode, string MasterCode, string BranchCode)
        {
            string Query = string.Empty;
            Query = $@" SELECT A.MISC_EDESC AS SUB_EDESC ,A.GROUP_SKU_FLAG,A.MASTER_MISC_CODE,A.LINK_SUB_CODE,NVL(0,0) OPEN_AMOUNT,
                            (SELECT SUM(ROUND(NVL(V.DR_AMOUNT,0),2))
                            FROM V$VIRTUAL_SUB_LEDGER V, FA_MISC_SUBLEDGER_SETUP X  
                            WHERE  V.SUB_CODE =  TRIM(X.LINK_SUB_CODE) 
                            AND X.DELETED_FLAG='N' 
                            AND X.COMPANY_CODE IN ({companyCode})  
                            AND X.PRE_MISC_CODE LIKE A.MASTER_MISC_CODE||'%' 
                            AND V.COMPANY_CODE= X.COMPANY_CODE 
                            AND V.BRANCH_CODE IN ({BranchCode})    
                            AND TO_DATE(TO_CHAR(V.VOUCHER_DATE,'DD-MON-YYYY')) >=TO_DATE('{startDate}') 
                            AND TO_DATE(TO_CHAR(V.VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{endDate}') 
                            AND V.FORM_CODE <> '0' ) DR_AMOUNT,
                            (SELECT  SUM(ROUND(NVL(V.CR_AMOUNT,0),2))
                            FROM V$VIRTUAL_SUB_LEDGER V, FA_MISC_SUBLEDGER_SETUP X  
                            WHERE  V.SUB_CODE =  TRIM(X.LINK_SUB_CODE) 
                            AND X.DELETED_FLAG='N' 
                            AND X.COMPANY_CODE IN({companyCode})  
                            AND X.PRE_MISC_CODE LIKE A.MASTER_MISC_CODE||'%' 
                            AND V.COMPANY_CODE= X.COMPANY_CODE 
                            AND V.BRANCH_CODE IN ({BranchCode})        
                            AND TO_DATE(TO_CHAR(V.VOUCHER_DATE,'DD-MON-YYYY')) >=TO_DATE('{startDate}') 
                            AND TO_DATE(TO_CHAR(V.VOUCHER_DATE,'DD-MON-YYYY')) <= TO_DATE('{endDate}') 
                            AND V.FORM_CODE <> '0' ) CR_AMOUNT FROM FA_MISC_SUBLEDGER_SETUP  A
                            WHERE DELETED_FLAG='N' 
                            AND COMPANY_CODE IN({companyCode}) 
                            AND PRE_MISC_CODE = '{MasterCode}'  
                            AND DELETED_FLAG = 'N'
                            ORDER BY GROUP_SKU_FLAG,MISC_EDESC
                            ";
            var list = _objectEntity.SqlQuery<VoucherDetailModel>(Query).ToList();
            //var filtered = list.Where(x => x.dr_amount != 0 || x.cr_amount != 0).ToList();
            var filtered = list.Where(x => x.dr_amount != 0 && x.dr_amount != null || x.cr_amount != 0 && x.cr_amount != null || x.OPEN_AMOUNT != 0 && x.OPEN_AMOUNT != null).ToList();
            return filtered;
        }

        public List<MultiSelectModels> GetLedgerAC(string multiselectCodes)
        {

            string query = $@"SELECT FS.SUB_CODE,FS.ACC_CODE AS Code,FC.ACC_EDESC AS Name FROM FA_SUB_LEDGER_MAP FS JOIN FA_CHART_OF_ACCOUNTS_SETUP FC ON FS.ACC_CODE = FC.ACC_CODE ";
            if (multiselectCodes != null)
            {
                string replacedCodes = "'" + multiselectCodes.Replace(",", "','") + "'";
                query += $@" WHERE TRIM(FS.SUB_CODE) IN ({replacedCodes})";

            }
            var supplierListNodes = _objectEntity.SqlQuery<MultiSelectModels>(query).ToList();
            return supplierListNodes;
        }
    }
}
