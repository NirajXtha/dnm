using NeoErp.Core;
using NeoErp.Core.Models;
using NeoErp.Core.Models.Log4NetLoggin;
using NeoERP.DocumentTemplate.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoERP.DocumentTemplate.Service.Services
{
    public class CostCenterMappingSetupService
    {
        private IWorkContext _workContext;
        private DefaultValueForLog _defaultValueForLog;
        private ILogErp _logErp;
        private NeoErpCoreEntity _objectEntity;

        public CostCenterMappingSetupService(IWorkContext workContext, NeoErpCoreEntity objectEntity)
        {
            this._workContext = workContext;
            this._objectEntity = objectEntity;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            _logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule, _defaultValueForLog.FormCode);
        }
        private List<CostCenterMappingModal> generateAccountTree(List<CostCenterMappingModal> model, List<CostCenterMappingModal> accNodes, string preaccCode)
        {
            foreach (var account in model.Where(x => x.PRE_ACC_CODE == preaccCode))
            {
                var accountNodesChild = new List<CostCenterMappingModal>();
                accNodes.Add(new CostCenterMappingModal()
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

        public List<CostCenterMappingModal> GetCharOfAccountTree()
        {
            try
            {
                var accountNodes = new List<CostCenterMappingModal>();
                var accountTreeQuery = $@"SELECT LEVEL, LPAD('  ',2*(LEVEL - 1)) ||  ACC_EDESC ""ACC_EDESC"",ACC_CODE,MASTER_ACC_CODE,PRE_ACC_CODE,ACC_TYPE_FLAG
                                          FROM FA_CHART_OF_ACCOUNTS_SETUP
                                          WHERE DELETED_FLAG = 'N' 
                                          START WITH PRE_ACC_CODE='00'
                                          CONNECT BY PRIOR MASTER_ACC_CODE = PRE_ACC_CODE
                                         ORDER BY PRE_ACC_CODE";
                var accountTreeData = _objectEntity.SqlQuery<CostCenterMappingModal>(accountTreeQuery).ToList();
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
        public List<CostCenterMapDetail> GetCostCenterMappingForGrid(string subCode, string masterCode)
        {
            try
            {
                if (subCode == "0") return new List<CostCenterMapDetail>();
                else
                {
                    string gridQuery = $@"SELECT FSLM.SUB_CODE,FSLM.ACC_CODE,FSLM.COMPANY_CODE,FSLM.CREATED_BY,FSLM.CREATED_DATE,FCAS.ACC_EDESC,SCS.CUSTOMER_EDESC as DESCRIPTION,
                                            CASE WHEN regexp_replace(FSLM.SUB_CODE,'(^| )([^ ])([^ ])*','\2') = 'C' THEN 'CUSTOMER'  
                                                 WHEN regexp_replace(FSLM.SUB_CODE,'(^| )([^ ])([^ ])*','\2') = 'E' THEN 'EMPLOYEE' 
                                                 WHEN regexp_replace(FSLM.SUB_CODE,'(^| )([^ ])([^ ])*','\2') = 'S' THEN 'SUPPLIER' 
                                                 ELSE 'ITEM' END AS SUB_LEDGER_TYPE 
                                         FROM FA_SUB_LEDGER_MAP FSLM
                                         INNER JOIN FA_CHART_OF_ACCOUNTS_SETUP FCAS ON FCAS.ACC_CODE = FSLM.ACC_CODE
                                         INNER JOIN SA_CUSTOMER_SETUP SCS on SCS.CUSTOMER_CODE = substr(FSLM.SUB_CODE,2)
                                          WHERE FSLM.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND FSLM.DELETED_FLAG='N' AND FSLM.ACC_CODE='{subCode}'";
                    var gridData = _objectEntity.SqlQuery<CostCenterMapDetail>(gridQuery).ToList();
                    return gridData;
                }
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting mapping details: " + ex.StackTrace);
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

        public List<CustomerSubLedgerModal> GetCustomerSubLedger()
        {
            try
            {
                var customerNodes = new List<CustomerSubLedgerModal>();
                var customerSubLegrQuery = $@"SELECT DISTINCT SCS.CUSTOMER_CODE AS TYPE_CODE,SCS.CUSTOMER_EDESC AS TYPE_EDESC,SCS.REGD_OFFICE_EADDRESS,SCS.PARTY_TYPE_CODE,SCS.LINK_SUB_CODE,SCS.MASTER_CUSTOMER_CODE,SCS.PRE_CUSTOMER_CODE,
                                             SCS.COMPANY_CODE,SCS.CREATED_BY,SCS.CREATED_DATE,SCS.ACC_CODE,SCS.TPIN_VAT_NO,SCS.GROUP_SKU_FLAG
                                            FROM SA_CUSTOMER_SETUP SCS WHERE SCS.DELETED_FLAG='N' AND SCS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' 
                                            CONNECT BY PRIOR SCS.MASTER_CUSTOMER_CODE=SCS.PRE_CUSTOMER_CODE START WITH SCS.PRE_CUSTOMER_CODE='00'";
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
        public string SaveSubLedgerMapping(ModalToSaveSubLedgerMapping modal)
        {
            try
            {
                string Info = "";
                var listCount = 0;
                var listInfoCount = modal.LIST_INFO.Count();
                foreach (var ml in modal.LIST_INFO)
                {
                    if (ml.GROUP_SKU_FLAG.Replace("\"", "") == "G") continue;
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
    }
}
