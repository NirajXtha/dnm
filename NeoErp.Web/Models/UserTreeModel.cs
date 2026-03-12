using System;
using System.Collections.Generic;

namespace NeoErp.Models
{
    public class UserTreeModel
    {
        public decimal LEVEL { get; set; }
        public string LOGIN_EDESC { get; set; }
        public string LOGIN_CODE { get; set; }
        public decimal USER_NO { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public decimal PARENT_USER_CODE { get; set; }
        public decimal PRE_USER_NO { get; set; }
        public decimal LEVEL_1 { get; set; }
        public string ABBR_CODE { get; set; }
        public bool hasBranch { get; set; }
        public IEnumerable<UserTreeModel> Items { get; set; }
    }

    public class CompanyBranchSaveModal
    {
        // public List<CompanyBranchTree> checkedCompany { get; set; } 
        public List<AvailableControl> checkedControl { get; set; }
        public List<UserTreeModel> checkedUser { get; set; }
    }

    public class UserAccessSaveModel
    {
        public bool isUpdate { get; set; } = false;
        public List<AvailableControl> checkedControl { get; set; }
        public List<UserTreeModel> checkedUser { get; set; }
    }

    public class CompanyBranchTree
    {
        public string branch_edesc { get; set; }
        public string branch_Code { get; set; }
        public string GroupSkuFlag { get; set; }
        public bool hasBranch { get; set; }
        public string MasterItemCode { get; set; }
        public string pre_branch_code { get; set; }
        public string Abbr_Code { get; set; }
        public IEnumerable<CompanyBranchTree> Items { get; set; }
    }

    public class DropdownUsers
    {
        public int USER_NO { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string LOGIN_CODE { get; set; }
        public string LOGIN_EDESC { get; set; }
        public string COMPANY_CODE { get; set; }
        public string USER_TYPE { get; set; }

    }

    public class CompanyModal
    {
        public string COMPANY_CODE { get; set; }
        public string COMPANY_EDESC { get; set; }
        public string PRE_COMPANY_CODE { get; set; }
        public string ABBR_CODE { get; set; }
        public bool HAS_BRANCH { get; set; }
        public List<BranchModal> CompanyBranch { get; set; }

    }

    public class BranchModal
    {
        public string BRANCH_CODE { get; set; }
        public string BRANCH_EDESC { get; set; }
        public string PRE_BRANCH_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string GROUP_SKU_FLAG { get; set; }

    }

    public class AccessedControl
    {
        public string COMPANY_CODE { get; set; }
        public int USER_NO { get; set; }
        public string BRANCH_CODE { get; set; }
        public string BRANCH_EDESC { get; set; }

    }

    public class AppModule
    {
        public string MODULE_CODE { get; set; }
        public string MODULE_EDESC { get; set; }

    }

    public class AvailableModuleModal
    {
        public int LEVEL { get; set; }
        public string FORM_CODE { get; set; }
        public string FORM_EDESC { get; set; }
        public string FORM_NDESC { get; set; }
        public string CUSTOMER_PREFIX { get; set; }
        public string MODULE_CODE { get; set; }
        public string MASTER_FORM_CODE { get; set; }
        public string PRE_FORM_CODE { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string NEW1 { get; set; }
        public string VIEW1 { get; set; }
        public string EDIT1 { get; set; }
        public string CHECK1 { get; set; }
        public string VERIFY1 { get; set; }
        public string POSTPRINT1 { get; set; }
        public string UNPOST1 { get; set; }
        public IEnumerable<AvailableModuleModal> CHILDREN { get; set; }
    }

    public class AvailableModuleModalTree
    {
        public int Level { get; set; }
        public string FORM_CODE { get; set; }
        public string FORM_EDESC { get; set; }
        public string FORM_NDESC { get; set; }
        public string CUSTOMER_PREFIX { get; set; }
        public string MODULE_CODE { get; set; }
        public string MASTER_FORM_CODE { get; set; }
        public string PRE_FORM_CODE { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public bool @checked { get; set; } = false;
        public bool hasSubModule { get; set; }
        public IEnumerable<AvailableModuleModalTree> Items { get; set; }
    }

    public class AvailableControl
    {
        
        public bool SELECT { get; set; } = false;
        public string CONTROL_NAME { get; set; } = string.Empty;
        public string CONTROL_CODE { get; set; } = string.Empty;
        public bool NEW { get; set; } = false;
        public string NEW1 { get; set; } 
        public string REPORTSTO { get; set; } = string.Empty;
        public bool VIEW { get; set; } = false;
        public string VIEW1 { get; set; } 
        public bool EDIT { get; set; } = false;
        public string EDIT1 { get; set; } 
        public bool RECYCLE { get; set; } = false;
        public string RECYCLE1 { get; set; } 
        public bool POSTPRINT { get; set; } = false;
        public string POSTPRINT1 { get; set; }
        public bool UNPOST { get; set; } = false;
        public string UNPOST1 { get; set; }
        public bool CHECK { get; set; } = false;
        public string CHECK1 { get; set; }
        public bool VERIFY { get; set; } = false;
        public string VERIFY1 { get; set; } 
        public string MORE { get; set; } = string.Empty;
        public string CONTROL_HEADING { get; set; } = string.Empty;
    }
}
