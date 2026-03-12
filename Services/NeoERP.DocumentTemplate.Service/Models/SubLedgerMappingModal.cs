using System;
using System.Collections.Generic;

namespace NeoERP.DocumentTemplate.Service.Models
{
    public class SubLedgerMappingModal
    {
        public string ACC_CODE { get; set; }
        public string ACC_EDESC { get; set; }
        public string TRANSACTION_TYPE { get; set; }
        public string TPB_FLAG { get; set; }
        public string ACC_TYPE_FLAG { get; set; }
        public string MASTER_ACC_CODE { get; set; }
        public string PRE_ACC_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public bool HAS_BRANCH { get; set; }
        public List<SubLedgerMappingModal> ITEMS { get; set; }

    }

    public class CustomerSubLedgerModal
    {
        public string TYPE_CODE { get; set; }
        public string TYPE_EDESC { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string REGD_OFFICE_EADDRESS { get; set; }
        public string PARTY_TYPE_CODE { get; set; }
        public string LINK_SUB_CODE { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string MASTER_CUSTOMER_CODE { get; set; }
        public string PRE_CUSTOMER_CODE { get; set; }
        public string ACC_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string TPIN_VAT_NO { get; set; }
        public bool HAS_BRANCH { get; set; }
        public List<CustomerSubLedgerModal> ITEMS { get; set; }
    }

    public class EmployeeSubLedgerModal
    {
        public string TYPE_CODE { get; set; }
        public string TYPE_EDESC { get; set; }
        public string EMPLOYEE_CODE { get; set; }
        public string EMPLOYEE_EDESC { get; set; }
        public string MASTER_EMPLOYEE_CODE { get; set; }
        public string PRE_EMPLOYEE_CODE { get; set; }
        public DateTime? JOIN_DATE { get; set; }
        public string ACC_CODE { get; set; }
        public string LINK_SUB_CODE { get; set; }
        public string EMPLOYEE_TYPE_CODE { get; set; }
        public string EMPLOYEE_STATUS { get; set; }
        public string EMPLOYEE_MANUAL_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string LOCK_FLAG { get; set; }
        public string PAN_NO { get; set; }
        public bool HAS_BRANCH { get; set; }
        public List<EmployeeSubLedgerModal> ITEMS { get; set; }

    }

    public class SupplierSubLederModal
    {
        public string TYPE_CODE { get; set; }
        public string TYPE_EDESC { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_EDESC { get; set; }
        public string PARTY_TYPE_CODE { get; set; }
        public string LINK_SUB_CODE { get; set; }
        public string MASTER_SUPPLIER_CODE { get; set; }
        public string PRE_SUPPLIER_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string ACC_CODE { get; set; }
        public string TPIN_VAT_NO { get; set; }
        public string APPROVED_FLAG { get; set; }
        public string CASH_SUPPLIER_FLAG { get; set; }
        public bool HAS_BRANCH { get; set; }
        public List<SupplierSubLederModal> ITEMS { get; set; }

    }

    public class ChartOfItemModel
    {
        public string TYPE_CODE { get; set; }
        public string TYPE_EDESC { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string CATEGORY_CODE { get; set; }
        public string INDEX_MU_CODE { get; set; }
        public string MASTER_ITEM_CODE { get; set; }
        public string PRE_ITEM_CODE { get; set; }
        public string COSTING_METHOD_FLAG { get; set; }
        public string LINK_SUB_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string PRODUCT_CODE { get; set; }
        public bool HAS_BRANCH { get; set; }
        public List<ChartOfItemModel> ITEMS { get; set; }

    }

    public class ModalToSaveSubLedgerMapping
    {
        public string ACCOUNT_INFO_EDESC { get; set; }
        public string ACCOUNT_INFO_CODE { get; set; }
        public string SUB_CODE_INFO { get; set; }
        public string SUB_CODE_TEXT { get; set; }
        public string SELECTED_ACCOUNT_TO_MAP { get; set; }
        public List<ListInfo> LIST_INFO { get; set; }
    }

    public class ListInfo
    {
        public string TEXT { get; set; }
        public string VALUE { get; set; }
        public string LINK_SUB_CODE { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
    }

    public class SubLedgerMapDetail
    {
        public string DESCRIPTION { get; set; }
        public string SUB_CODE { get; set; }
        public string ACC_CODE { get; set; }
        public string ACC_EDESC { get; set; }
        public string COMPANY_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string BRANCH_CODE { get; set; }
        public string SUB_LEDGER_TYPE { get; set; }
    }

    public class CostCenterSubLedgerModal
    {
        public string TYPE_CODE { get; set; }
        public string TYPE_EDESC { get; set; }
        public string PRE_BUDGET_CODE { get; set; }
        public string ACC_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public bool HAS_BRANCH { get; set; }
        public string ACC_EDESC { get; set; }
        public string DESCRIPTION { get; set; }
        public string BUDGET_CODE { get; set; }
        public List<CostCenterSubLedgerModal> ITEMS { get; set; }
    }
}
