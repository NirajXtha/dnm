using NeoErp.Core;
using NeoErp.Core.Models;
using NeoErp.Core.Models.Log4NetLoggin;
using NeoERP.DocumentTemplate.Service.Interface;
using NeoERP.DocumentTemplate.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Services
{
    public class SubLedgerMappingSetupService : ISubLegerMappingSetup
    {

        private IWorkContext _workContext;
        private DefaultValueForLog _defaultValueForLog;
        private ILogErp _logErp;
        private NeoErpCoreEntity _objectEntity;

        public SubLedgerMappingSetupService(IWorkContext workContext, NeoErpCoreEntity objectEntity)
        {
            this._workContext = workContext;
            this._objectEntity = objectEntity;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            _logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule, _defaultValueForLog.FormCode);
        }

        public List<SubLedgerMappingModal> GetCharOfAccountTree()
        {
            try
            {

                var accountNodes = new List<SubLedgerMappingModal>();
                var accountTreeQuery = $@"SELECT distinct LEVEL, LPAD('  ',2*(LEVEL - 1)) ||  ACC_EDESC ""ACC_EDESC"",ACC_CODE,MASTER_ACC_CODE,PRE_ACC_CODE,ACC_TYPE_FLAG
                                          FROM FA_CHART_OF_ACCOUNTS_SETUP
                                          WHERE DELETED_FLAG = 'N' and company_code in ('{_workContext.CurrentUserinformation.company_code}')
                                          START WITH PRE_ACC_CODE='00'
                                          CONNECT BY PRIOR MASTER_ACC_CODE = PRE_ACC_CODE
                                         ORDER BY PRE_ACC_CODE";
                var accountTreeData = _objectEntity.SqlQuery<SubLedgerMappingModal>(accountTreeQuery).ToList();
                var accountNode = generateAccountTree(accountTreeData, accountNodes, "00");
                _logErp.InfoInFile("Chart of account data for tree fetch successfully by : " + _workContext.CurrentUserinformation.LOGIN_EDESC);
                return accountNode;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting char of account tree : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }


        private List<SubLedgerMappingModal> generateAccountTree(List<SubLedgerMappingModal> model, List<SubLedgerMappingModal> accNodes, string preaccCode)
        {
            foreach (var account in model.Where(x => x.PRE_ACC_CODE == preaccCode))
            {
                var accountNodesChild = new List<SubLedgerMappingModal>();
                accNodes.Add(new SubLedgerMappingModal()
                {
                    ACC_EDESC = account.ACC_EDESC,
                    ACC_CODE = account.ACC_CODE,
                    MASTER_ACC_CODE = account.MASTER_ACC_CODE,
                    PRE_ACC_CODE = account.PRE_ACC_CODE,
                    ACC_TYPE_FLAG = account.ACC_TYPE_FLAG,
                    HAS_BRANCH = account.ACC_TYPE_FLAG == "N" ? true : false,
                    ITEMS = account.ACC_TYPE_FLAG == "N" ? generateAccountTree(model, accountNodesChild, account.MASTER_ACC_CODE) : null,
                });
            }
            return accNodes;
        }

        public List<ChartOfItemModel> GetChatOfItemSubLedger()
        {
            try
            {
                var chartOfItemSubLegrQuery = $@"SELECT DISTINCT IIMS.ITEM_CODE AS TYPE_CODE,IIMS.ITEM_EDESC AS TYPE_EDESC,IIMS.CATEGORY_CODE,IIMS.INDEX_MU_CODE,IIMS.LINK_SUB_CODE,IIMS.MASTER_ITEM_CODE,IIMS.PRE_ITEM_CODE,
                                             IIMS.COMPANY_CODE,IIMS.CREATED_BY,IIMS.CREATED_DATE,IIMS.PRODUCT_CODE,IIMS.COSTING_METHOD_FLAG
                                            FROM IP_ITEM_MASTER_SETUP IIMS WHERE IIMS.DELETED_FLAG='N' AND IIMS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' 
                                            CONNECT BY PRIOR IIMS.MASTER_ITEM_CODE=IIMS.PRE_ITEM_CODE START WITH IIMS.PRE_ITEM_CODE='00'";
                var chartOfItemSubLegrData = _objectEntity.SqlQuery<ChartOfItemModel>(chartOfItemSubLegrQuery).ToList();
                return chartOfItemSubLegrData;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting char of item sub ledger : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<CustomerSubLedgerModal> GetCustomerSubLedger()
        {
            try
            {
                var customerNodes = new List<CustomerSubLedgerModal>();
                //var customerSubLegrQuery = $@"SELECT DISTINCT SCS.CUSTOMER_CODE AS TYPE_CODE,SCS.CUSTOMER_EDESC AS TYPE_EDESC,SCS.REGD_OFFICE_EADDRESS,SCS.PARTY_TYPE_CODE,SCS.LINK_SUB_CODE,SCS.MASTER_CUSTOMER_CODE,SCS.PRE_CUSTOMER_CODE,
                //                             SCS.COMPANY_CODE,SCS.CREATED_BY,SCS.CREATED_DATE,SCS.ACC_CODE,SCS.TPIN_VAT_NO,SCS.GROUP_SKU_FLAG
                //                            FROM SA_CUSTOMER_SETUP SCS WHERE SCS.DELETED_FLAG='N' AND SCS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' 
                //                            CONNECT BY PRIOR SCS.MASTER_CUSTOMER_CODE=SCS.PRE_CUSTOMER_CODE START WITH SCS.PRE_CUSTOMER_CODE='00'";

                var customerSubLegrQuery = $@" SELECT DISTINCT
                          SCS.CUSTOMER_CODE AS TYPE_CODE,
                          SCS.CUSTOMER_EDESC AS TYPE_EDESC,
                          SCS.REGD_OFFICE_EADDRESS,
                          SCS.PARTY_TYPE_CODE,
                          SCS.LINK_SUB_CODE,
                          SCS.MASTER_CUSTOMER_CODE,
                          SCS.PRE_CUSTOMER_CODE,
                          SCS.COMPANY_CODE,
                          SCS.CREATED_BY,
                          SCS.CREATED_DATE,
                          SCS.ACC_CODE,
                          SCS.TPIN_VAT_NO,
                          SCS.GROUP_SKU_FLAG
                   FROM SA_CUSTOMER_SETUP SCS
                   WHERE SCS.DELETED_FLAG = 'N'
                     AND SCS.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                   CONNECT BY NOCYCLE
                          PRIOR SCS.MASTER_CUSTOMER_CODE = SCS.PRE_CUSTOMER_CODE
                   START WITH SCS.PRE_CUSTOMER_CODE = '00'";

                var customerSubLegrData = _objectEntity.SqlQuery<CustomerSubLedgerModal>(customerSubLegrQuery).ToList();
                var customerSubLeger = generateCustomerTree(customerSubLegrData, customerNodes, "00");
                return customerSubLeger;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting customer sub ledger : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        private List<CustomerSubLedgerModal> generateCustomerTree(List<CustomerSubLedgerModal> model, List<CustomerSubLedgerModal> cusNodes, string precusCode)
        {
            foreach (var customer in model.Where(x => x.PRE_CUSTOMER_CODE == precusCode))
            {
                var customerNodesChild = new List<CustomerSubLedgerModal>();
                cusNodes.Add(new CustomerSubLedgerModal()
                {
                    TYPE_CODE = customer.TYPE_CODE,
                    TYPE_EDESC = customer.TYPE_EDESC,
                    MASTER_CUSTOMER_CODE = customer.MASTER_CUSTOMER_CODE,
                    PRE_CUSTOMER_CODE = customer.PRE_CUSTOMER_CODE,
                    GROUP_SKU_FLAG = customer.GROUP_SKU_FLAG,
                    LINK_SUB_CODE = customer.LINK_SUB_CODE,
                    HAS_BRANCH = customer.GROUP_SKU_FLAG == "G" ? true : false,
                    ITEMS = customer.GROUP_SKU_FLAG == "G" ? generateCustomerTree(model, customerNodesChild, customer.MASTER_CUSTOMER_CODE) : null,
                });
            }
            return cusNodes;
        }

        public List<EmployeeSubLedgerModal> GetEmployeeSubLedger()
        {
            try
            {
                var employeeSubLegrQuery = $@"SELECT DISTINCT HES.EMPLOYEE_CODE AS TYPE_CODE,HES.EMPLOYEE_EDESC AS TYPE_EDESC,HES.MASTER_EMPLOYEE_CODE,HES.PRE_EMPLOYEE_CODE,HES.JOIN_DATE,HES.ACC_CODE,HES.LINK_SUB_CODE,HES.EMPLOYEE_TYPE_CODE,
                                             HES.EMPLOYEE_STATUS,HES.EMPLOYEE_MANUAL_CODE,HES.COMPANY_CODE,HES.CREATED_BY,HES.CREATED_DATE,HES.LOCK_FLAG,HES.PAN_NO
                                            FROM HR_EMPLOYEE_SETUP HES WHERE HES.DELETED_FLAG='N' AND HES.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' 
                                            CONNECT BY PRIOR HES.MASTER_EMPLOYEE_CODE=HES.PRE_EMPLOYEE_CODE START WITH HES.PRE_EMPLOYEE_CODE='00'";
                var employeeSubLegrData = _objectEntity.SqlQuery<EmployeeSubLedgerModal>(employeeSubLegrQuery).ToList();
                return employeeSubLegrData;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting employee sub ledger : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<SubLedgerMapDetail> GetSubLedgerMappingForGrid(string subCode, string masterCode)
        {
            try
            {
                if (subCode == "0") return new List<SubLedgerMapDetail>();
                else
                {
                    //string gridQuery = $@"SELECT SUB_CODE,ACC_CODE,COMPANY_CODE,CREATED_BY,CREATED_DATE FROM FA_SUB_LEDGER_MAP
                    //                      WHERE COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND DELETED_FLAG='N' AND ACC_CODE='{subCode}'";
                    string gridQuery = $@"SELECT distinct FSLM.SUB_CODE,FSLM.ACC_CODE,FSLM.COMPANY_CODE,FSLM.CREATED_BY,FSLM.CREATED_DATE,FCAS.ACC_EDESC,SCS.CUSTOMER_EDESC as DESCRIPTION,
                                            CASE WHEN regexp_replace(FSLM.SUB_CODE,'(^| )([^ ])([^ ])*','\2') = 'C' THEN 'CUSTOMER'  
                                                 WHEN regexp_replace(FSLM.SUB_CODE,'(^| )([^ ])([^ ])*','\2') = 'E' THEN 'EMPLOYEE' 
                                                 WHEN regexp_replace(FSLM.SUB_CODE,'(^| )([^ ])([^ ])*','\2') = 'S' THEN 'SUPPLIER' 
                                                 ELSE 'ITEM' END AS SUB_LEDGER_TYPE 
                                         FROM FA_SUB_LEDGER_MAP FSLM
                                         INNER JOIN FA_CHART_OF_ACCOUNTS_SETUP FCAS ON FCAS.ACC_CODE = FSLM.ACC_CODE AND FCAS.COMPANY_CODE = FSLM.COMPANY_CODE
                                         INNER JOIN SA_CUSTOMER_SETUP SCS on SCS.CUSTOMER_CODE = substr(FSLM.SUB_CODE,2) AND SCS.COMPANY_CODE = FSLM.COMPANY_CODE
                                          WHERE FSLM.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND FSLM.DELETED_FLAG='N' AND FSLM.ACC_CODE='{subCode}'";
                    var gridData = _objectEntity.SqlQuery<SubLedgerMapDetail>(gridQuery).ToList();
                    return gridData;
                }
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting mapping details: " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<SupplierSubLederModal> GetSupplierSubLedger()
        {
            try
            {
                var supplierSubLegrQuery = $@"SELECT DISTINCT ISS.SUPPLIER_CODE AS TYPE_CODE,ISS.SUPPLIER_EDESC AS TYPE_EDESC,ISS.PARTY_TYPE_CODE,ISS.LINK_SUB_CODE,ISS.MASTER_SUPPLIER_CODE,ISS.PRE_SUPPLIER_CODE,
                                             ISS.COMPANY_CODE,ISS.CREATED_BY,ISS.CREATED_DATE,ISS.ACC_CODE,ISS.TPIN_VAT_NO,ISS.APPROVED_FLAG,ISS.CASH_SUPPLIER_FLAG
                                            FROM IP_SUPPLIER_SETUP ISS WHERE ISS.DELETED_FLAG='N' AND ISS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' 
                                            CONNECT BY PRIOR ISS.MASTER_SUPPLIER_CODE=ISS.PRE_SUPPLIER_CODE START WITH ISS.PRE_SUPPLIER_CODE='00'";
                var supplierSubLegrData = _objectEntity.SqlQuery<SupplierSubLederModal>(supplierSubLegrQuery).ToList();
                return supplierSubLegrData;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting supplier sub ledger : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public string SaveSubLedgerMapping(ModalToSaveSubLedgerMapping modal)
        {
            try
            {
                string Info = "";
                var listCount = 0;
                var listInfoCount = modal.LIST_INFO.Count();
                foreach (var ml in modal.LIST_INFO)
                {
                    if (ml.GROUP_SKU_FLAG.Replace("\"","") == "G") continue;
                    if (ml.LINK_SUB_CODE == null || ml.LINK_SUB_CODE == "null") { listCount++; continue; }
                    var mappingSaveQuery = $@"INSERT INTO FA_SUB_LEDGER_MAP(SUB_CODE,ACC_CODE,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,BRANCH_CODE)
                                        VALUES('{ml.LINK_SUB_CODE.Replace("\"", "").TrimEnd()}','{modal.ACCOUNT_INFO_CODE}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N','{_workContext.CurrentUserinformation.branch_code}')";
                    _objectEntity.ExecuteSqlCommand(mappingSaveQuery);
                }
                if (listCount > listInfoCount / 2)
                {
                    _logErp.WarnInDB("Ambigious value for saving subledger, no link sub code found on provided subleger: ");
                    return "Ambigious value for saving subledger, no link sub code found on provided subleger";
                }
                else
                {
                    _logErp.InfoInFile("Sub ledger mapping save successfully by : " + _workContext.CurrentUserinformation.LOGIN_EDESC);
                    return "Successfull";
                }

            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while saving sub ledger mapping : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }
        public List<CostCenterSubLedgerModal> GetCostCenterSubLedger()
        {
            try
            {
                var customerNodes = new List<CostCenterSubLedgerModal>();
                var employeeSubLegrQuery = $@"SELECT DISTINCT HES.BUDGET_CODE AS TYPE_CODE,HES.BUDGET_EDESC AS TYPE_EDESC,HES.PRE_BUDGET_CODE,HES.ACC_CODE,HES.COMPANY_CODE,HES.CREATED_BY,HES.CREATED_DATE,HES.GROUP_SKU_FLAG FROM bc_budget_center_setup HES WHERE HES.DELETED_FLAG='N' AND HES.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
                var employeeSubLegrData = _objectEntity.SqlQuery<CostCenterSubLedgerModal>(employeeSubLegrQuery).ToList();
                var costCenterSubLeger = generateCostCenterTree(employeeSubLegrData, customerNodes, "00");
                return costCenterSubLeger;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting employee sub ledger : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }
        private List<CostCenterSubLedgerModal> generateCostCenterTree(List<CostCenterSubLedgerModal> model, List<CostCenterSubLedgerModal> cusNodes, string precusCode)
        {
            foreach (var customer in model.Where(x => x.PRE_BUDGET_CODE == precusCode))
            {
                var customerNodesChild = new List<CostCenterSubLedgerModal>();
                cusNodes.Add(new CostCenterSubLedgerModal()
                {
                    TYPE_CODE = customer.TYPE_CODE,
                    TYPE_EDESC = customer.TYPE_EDESC,
                    PRE_BUDGET_CODE = customer.PRE_BUDGET_CODE,
                    GROUP_SKU_FLAG = customer.GROUP_SKU_FLAG,
                    HAS_BRANCH = customer.GROUP_SKU_FLAG == "G" ? true : false,
                    ITEMS = customer.GROUP_SKU_FLAG == "G" ? generateCostCenterTree(model, customerNodesChild, customer.TYPE_CODE) : null,
                });
            }
            return cusNodes;
        }
        public string SaveCostCenterMapping(ModalToSaveSubLedgerMapping modal)
        {
            try
            {
                //var listCount = 0;
                //var listInfoCount = modal.LIST_INFO.Count();
                int serialNo = 1;
                foreach (var ml in modal.LIST_INFO)
                {
                    if (ml.GROUP_SKU_FLAG.Replace("\"", "") == "G") continue;
                    var mappingSaveQuery = $@"INSERT INTO BC_BUDGET_CENTER_MAP(BUDGET_CODE,ACC_CODE,SERIAL_NO,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,BRANCH_CODE) VALUES('{ml.VALUE.Replace("\"","").TrimEnd()}','{modal.ACCOUNT_INFO_CODE}',{serialNo},'{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N','{_workContext.CurrentUserinformation.branch_code}')";
                    _objectEntity.ExecuteSqlCommand(mappingSaveQuery);
                    serialNo++;
                }
                //if (listCount > listInfoCount / 2)
                //{
                //    _logErp.WarnInDB("Ambigious value for saving subledger, no link sub code found on provided subleger: ");
                //    return "Ambigious value for saving subledger, no link sub code found on provided subleger";
                //}
                //else
                //{
                    _logErp.InfoInFile("Sub ledger mapping save successfully by : " + _workContext.CurrentUserinformation.LOGIN_EDESC);
                    return "Successfull";
                ///}
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while saving sub ledger mapping : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }
        public List<CostCenterSubLedgerModal> GetCostCenterMappingForGrid(string subCode, string masterCode)
        {
            try
            {
                if (subCode == "0") return new List<CostCenterSubLedgerModal>();
                else
                {
                    string gridQuery = $@"SELECT FSLM.BUDGET_CODE,FSLM.ACC_CODE,FSLM.COMPANY_CODE,FSLM.CREATED_BY,FSLM.CREATED_DATE,FCAS.ACC_EDESC,SCS.BUDGET_EDESC as DESCRIPTION FROM BC_BUDGET_CENTER_MAP FSLM INNER JOIN FA_CHART_OF_ACCOUNTS_SETUP FCAS ON FCAS.ACC_CODE = FSLM.ACC_CODE INNER JOIN BC_BUDGET_CENTER_SETUP SCS on SCS.BUDGET_CODE = FSLM.BUDGET_CODE WHERE FSLM.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND FSLM.DELETED_FLAG='N' AND FSLM.ACC_CODE='{subCode}'";
                    var gridData = _objectEntity.SqlQuery<CostCenterSubLedgerModal>(gridQuery).ToList();
                    return gridData;
                }
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting mapping details: " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }
    }
}
