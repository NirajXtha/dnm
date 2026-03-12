using NeoErp.Core.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NeoErp.Data;
using System.Web.Mvc;

namespace NeoErp.Core.Models
{
    public class AppSettingsModel
    {
        public ConCredential SqlConInfo { get; set; }
        public string ClientUniqueID { get; set; }

        public string ExcelReportWebPath = "/Documents/Reports/";
        public string EmpImagePath = "/Documents/EmployeeImages/";
        public string FileUploadUrl = "/Home/UploadFile";

        public string DashboardPage
        {
            get { return dashBoardPage; }
            set { dashBoardPage = value; }
        }
        public static string dashBoardPage = "/Main/DashBoard";
        public static string noPromissionPage = "/Security/Account/NoPermission";
        public static string loginPage = "/Security/Account/Login";
        public static string connStringName = "connString";

    }



    public class FormTreeStructureModel
    {
        public string FORM_CODE { get; set; }
        public string FORM_EDESC { get; set; }
        public string MASTER_FORM_CODE { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string PRE_FORM_CODE { get; set; }
        public string FORM_NDESC { get; set; }
        public string REMARKS { get; set; }
    }



    public class FormSetupResultModel
    {
        public string FORM_CODE { get; set; }
        public string FORM_EDESC { get; set; }
        public string MODULE_CODE { get; set; }
        public string NUMBERING_FORMAT { get; set; }
        public string DATE_FORMAT { get; set; }
        public string START_ID_FLAG { get; set; }
        public string ID_GENERATION_FLAG { get; set; }
        public string CUSTOM_PREFIX_TEXT { get; set; }
        public string CUSTOM_SUFFIX_TEXT { get; set; }
        public string PREFIX_LENGTH { get; set; }
        public string SUFFIX_LENGTH { get; set; }
        public string BODY_LENGTH { get; set; }
        public string START_NO { get; set; }
        public string LAST_NO { get; set; }
        public DateTime? START_DATE { get; set; }
        public DateTime? LAST_DATE { get; set; }
        public string PRINT_REPORT_FLAG { get; set; }
        public string COPY_VALUES_FLAG { get; set; }
        public string QUALITY_CHECK_FLAG { get; set; }
        public string SERIAL_TRACKING_FLAG { get; set; }
        public string BATCH_TRACKING_FLAG { get; set; }
        public string REMARKS { get; set; }

    }

    //SELECT '00' MASTER_ITEM_CODE, '<PRIMARY>' ITEM_EDESC, null as Module_code FROM DUAL
    //                UNION ALL
    //                SELECT master_form_code, form_edesc, module_code
    //                  FROM form_setup
    //                 WHERE DELETED_FLAG = 'N'
    //                       AND COMPANY_CODE = '01'
    //                       AND pre_form_code = '00'
    //                       AND group_sku_flag = 'G'

    public class AllFormModel
    {
        public string MASTER_ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string MODULE_CODE { get; set; }
    }

    public class ModuleCodeAndDetailModel
    {
        public string MODULE_CODE { get; set; }
        public string MODULE_EDESC { get; set; }
    }

    public class BranchCodeDtlModel
    {
        public string BRANCH_CODE { get; set; }
        public string BRANCH_EDESC { get; set; }
        public bool? CHECKCED { get; set; }
    }

    public class FormSetupGroupEntryModel
    {
        public string FORM_CODE { get; set; }
        public string FORM_EDESC { get; set; }
        public string FORM_NDESC { get; set; }
        public string MASTER_FORM_CODE { get; set; }
        public string PRE_FORM_CODE { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string REMARKS { get; set; }
        public string MODULE_CODE { get; set; }
        public bool IS_EDIT { get; set; }
    }


    public class FormSetupFormattingTabModel
    {
        public FormSetupAddOrDuplicateStepModel FormDataInfoModel { get; set; }
        public List<FormattingFormDetailSetupModel> FormattingFormDetailSetupList { get; set; }
        public FormSetupFormatingModel FormFormattingModel { get; set; }
    }



    public class FormSetupAddOrDuplicateStepModel
    {

        public string NEXT_FORM_CODE { get; set; }
        public string ORIGINAL_FORM_CODE { get; set; }
        public string FORM_CODE { get; set; }
        public string FORM_EDESC { get; set; }
        public string FORM_NDESC { get; set; }
        //    public string DR_ACC_CODE { get; set; }
        //    public string CR_ACC_CODE { get; set; }
        public string MASTER_FORM_CODE { get; set; }
        public string PRE_FORM_CODE { get; set; }
        public string MODULE_CODE { get; set; }
        //public string TEMPLATE_CODE { get; set; }

        public string GROUP_SKU_FLAG { get; set; }
        //public string NUMBERING_FORMAT { get; set; }
        //public string DATE_FORMAT { get; set; }
        //public string START_ID_FLAG { get; set; }
        //public string ID_GENERATION_FLAG { get; set; }
        //public string CUSTOM_PREFIX_TEXT { get; set; }
        //public string CUSTOM_SUFFIX_TEXT { get; set; }
        //public int? PREFIX_LENGTH { get; set; }
        //public int? SUFFIX_LENGTH { get; set; }
        //public int? BODY_LENGTH { get; set; }
        //public long? START_NO { get; set; }
        //public long? LAST_NO { get; set; }
        //public DateTime? START_DATE { get; set; }
        //public DateTime? LAST_DATE { get; set; }
        //public string REF_COLUMN_NAME { get; set; }
        //public string PRINT_REPORT_FLAG { get; set; }
        //public string PRIMARY_MANUAL_FLAG { get; set; }
        //public string COPY_VALUES_FLAG { get; set; }
        //public string QUALITY_CHECK_FLAG { get; set; }
        //public string SERIAL_TRACKING_FLAG { get; set; }
        //public string BATCH_TRACKING_FLAG { get; set; }
        //public string ACC_CODE { get; set; }
        public string REMARKS { get; set; }
        //public string COMPANY_CODE { get; set; }
        //public string CREATED_BY { get; set; }
        //public DateTime? CREATED_DATE { get; set; }
        //public string DELETED_FLAG { get; set; }
        //public string COINAGE_FLAG { get; set; }
        //public string COINAGE_SUB_CODE { get; set; }
        //public string REFERENCE_FLAG { get; set; }
        //public string REF_TABLE_NAME { get; set; }
        //public string PUBLIC_FLAG { get; set; }
        public string FORM_ACTION_FLAG { get; set; }
        //public string TOTAL_ROUND_FLAG { get; set; }
        //public int? TOTAL_ROUND_INDEX { get; set; }
        //public string REF_FORM_CODE { get; set; }
        public string INTER_BRANCH_FLAG { get; set; }
        //public int? REPORT_NO { get; set; }
        //public string MULTI_UNIT_FLAG { get; set; }
        //public string SALES_INVOICE_CHALAN_FLAG { get; set; }
        //public string REF_FIX_QUANTITY { get; set; }
        //public string REF_FIX_PRICE { get; set; }
        //public int? FREEZE_BACK_DAYS { get; set; }
        //public string AUTO_GL_POST { get; set; }
        //public string RATE_SCHEDULE_FIX_PRICE { get; set; }
        //public string FREEZE_MANUAL_ENTRY_FLAG { get; set; }
        //public string ADVANCE_FLAG { get; set; }
        //public string PURCHASE_EXPENSES_FLAG { get; set; }
        public string FORM_TYPE { get; set; }
        //public string COSTING_FLAG { get; set; }
        //public string NEGATIVE_STOCK_FLAG { get; set; }
        //public string QC_PARAMETER_FLAG { get; set; }
        //public string OTHER_INFO_FLAG { get; set; }
        //public string AFTER_VERIFY_FLAG { get; set; }
        //public string PRICE_CONTROL_FLAG { get; set; }
    }

    public class TransactionTableListModel
    {
        public string TABLE_NAME { get; set; }
        public string TABLE_DESC { get; set; }
    }




    public class FormSetupFormatingModel
    {
        public string COINAGE_FLAG { get; set; }
        public string NUMBERING_FORMAT { get; set; }
        public string DATE_FORMAT { get; set; }
        public string COINAGE_SUB_CODE { get; set; }
        public string TABLE_NAME { get; set; }

    }

    public class SubLedgerListModel
    {
        public string SUB_EDESC { get; set; }
        public string SUB_CODE { get; set; }
    }

    public class FormattingFormDetailSetupModel
    {
        public int SERIAL_NO { get; set; }                  // NUMBER(5) NOT NULL
        public string TABLE_NAME { get; set; }              // VARCHAR2(30)
        public string COLUMN_NAME { get; set; }             // VARCHAR2(30)
        public string COLUMN_HEADER { get; set; }           // VARCHAR2(30)
        public int COLUMN_WIDTH { get; set; }               // NUMBER(5)
        public int TOP_POSITION { get; set; }               // NUMBER(5)
        public int LEFT_POSITION { get; set; }              // NUMBER(5)
        public string MASTER_CHILD_FLAG { get; set; }       // CHAR(1)
        public string IS_DESC_FLAG { get; set; }            // CHAR(1)
        public string DEFA_VALUE { get; set; }              // VARCHAR2(500)
        public string DISPLAY_FLAG { get; set; }            // CHAR(1)
        public string FILTER_VALUE { get; set; }           // VARCHAR2(100)
        public string DEFA_VALUE_DESC { get; set; }
        public string FILTER_VALUE_DESC { get; set; }
    }


    public class CustomerLookupDataModel
    {
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
    }

    public class ItemMasterLookupModel
    {
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
    }


    public class EmployeeLookupDataModel
    {
        public string EMPLOYEE_CODE { get; set; }
        public string EMPLOYEE_EDESC { get; set; }
    }


    public class CurrencyLookupModel
    {
        public string CURRENCY_CODE { get; set; }
        public string CURRENCY_EDESC { get; set; }
    }

    public class MULookupModel
    {
        public string MU_CODE { get; set; }
        public string MU_EDESC { get; set; }
    }

    public class SalesTypeLookupModel
    {
        public string SALES_TYPE_CODE { get; set; }
        public string SALES_TYPE_EDESC { get; set; }
    }

    public class PriorityLookupModel
    {
        public string PRIORITY_CODE { get; set; }
        public string PRIORITY_EDESC { get; set; }
    }

    public class CustomerLookupModel
    {
        public string MASTER_CUSTOMER_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string PRE_CUSTOMER_CODE { get; set; }
    }

    public class CustomerDetailWIthIDModel : CustomerLookupModel
    {
        public string CUSTOMER_ID { get; set; }
        public string CUSTOMER_CODE { get; set; }
    }


    public class GroupedTreeNode
    {
        public string id { get; set; }
        public string text { get; set; }
        public string parentCode { get; set; }
    }




    public class CustomerTreeNode : GroupedTreeNode
    {

        public List<CustomerTreeNode> items { get; set; } = new List<CustomerTreeNode>();
    }

    public class ItemGroupedTreeNode : GroupedTreeNode
    {
        public List<ItemGroupedTreeNode> items { get; set; } = new List<ItemGroupedTreeNode>();
    }

    public class EmployeeGroupedTreeNode : GroupedTreeNode
    {
        public List<EmployeeGroupedTreeNode> items { get; set; } = new List<EmployeeGroupedTreeNode>();
    }


    public class CustomerDetailModel
    {
        public string CODE { get; set; }
        public string NAME { get; set; }
        public string ADDRESS { get; set; }
        public string TEL1 { get; set; }
        public string TEL2 { get; set; }
        public string EMAIL { get; set; }
        public decimal? CREDIT_LIMIT { get; set; }
        public int? CREDIT_DAYS { get; set; }
        public string PAN { get; set; }
        public string CITY { get; set; }
        public string EXCISE_NO { get; set; }
        public string AREA { get; set; }
    }


    public class ItemGroupLookupModel
    {
        public string MASTER_ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string PRE_ITEM_CODE { get; set; }
    }


    public class ItemDetailWithIDModel
    {
        public string PRE_ITEM_CODE { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string PRODUCT_CODE { get; set; }
    }

    public class ItemDetailModel
    {
        public string CODE { get; set; }
        public string NAME { get; set; }
        public string CATEGORY { get; set; }
        public string UNIT { get; set; }
        public string ALT_UNIT { get; set; }
        public string HS_CODE { get; set; }
    }


    public class EmployeeGroupModel
    {
        public string MASTER_EMPLOYEE_CODE { get; set; }
        public string EMPLOYEE_EDESC { get; set; }
        public string PRE_EMPLOYEE_CODE { get; set; }
    }

    public class EmployeeDetailWithIDModel
    {
        public string PRE_EMPLOYEE_CODE { get; set; }
        public string EMPLOYEE_CODE { get; set; }
        public string EMPLOYEE_EDESC { get; set; }
        public string EMP_ID { get; set; }
    }




    public class FormSetupReferenceTabInfoModel
    {
        public string FORM_CODE { get; set; }
        public string REFERENCE_FLAG { get; set; }
        public string REF_TABLE_NAME { get; set; }
        public string REF_COLUMN_NAME { get; set; }
        public string REF_FORM_CODE { get; set; }
        public string FREEZE_MANUAL_ENTRY_FLAG { get; set; }
        public string PUBLIC_FLAG { get; set; }
        public string AFTER_VERIFY_FLAG { get; set; }
        public string ORDER_DISPATCH_FLAG { get; set; }
        public string AFTER_POSTING_FLAG { get; set; }
        public string FREEZE_MASTER_REF_FLAG { get; set; }
        public string ORDER_ACCESS_FAB_FLAG { get; set; }
        public string INVOICE_PJV_FORM_CODE { get; set; }
        public string INVOICE_PJV_FORM_EDESC { get; set; }
        public string RECEIPT_FLAG { get; set; }
        public string RECEIPT_FORM_CODE { get; set; }
        public string RECEIPT_FORM_CODE_EDESC { get; set; }
        public string RECEIPT_CASH_ACC_CODE { get; set; }
        public string RECEIPT_CASH_ACC_CODE_EDESC { get; set; }

        public string DEFAULT_FROM_LOCATION { get; set; }
        public string DEFAULT_SHIPMENT { get; set; }
        public List<BranchListModel> BRANCH_LIST { get; set; }
    }


    public class FormDropdownModel
    {
        public string FORM_CODE { get; set; }
        public string FORM_EDESC { get; set; }
    }

    public class BranchListModel
    {
        public string BRANCH_CODE { get; set; }
        public string BRANCH_EDESC { get; set; }
        public string CHECKCED { get; set; }
    }

    public class AccountListModel
    {
        public string ACC_EDESC { get; set; }
        public string ACC_CODE { get; set; }
    }



    public class FormSetupNumberingTabInfoModel
    {
        public string FORM_CODE { get; set; }
        public string ID_GENERATION_FLAG { get; set; }
        public string CUSTOM_PREFIX_TEXT { get; set; }
        public string CUSTOM_SUFFIX_TEXT { get; set; }

        // TO_CHAR() → always string
        public string PREFIX_LENGTH { get; set; }
        public string SUFFIX_LENGTH { get; set; }
        public string BODY_LENGTH { get; set; }
        public string START_NO { get; set; }
        public string LAST_NO { get; set; }

        public DateTime? START_DATE { get; set; }
        public DateTime? LAST_DATE { get; set; }

        public string BACK_DATE_VNO_SAVE_FLAG { get; set; }
        //                        CUSTOM_MANUAL_PREFIX_TEXT,
        //                        CUSTOM_MANUAL_SUFFIX_TEXT,
        //                        BODY_MANUAL_LENGTH,
        //                        START_MANUAL_NO
        public string CUSTOM_MANUAL_PREFIX_TEXT { get; set; }
        public string CUSTOM_MANUAL_SUFFIX_TEXT { get; set; }
        public int? BODY_MANUAL_LENGTH { get; set; }
        public int? START_MANUAL_NO { get; set; }

    }

    public class AccountInfoModel
    {
        public string ACC_EDESC { get; set; }
        public string ACC_CODE { get; set; }
    }

    public class ChargeCodeModel
    {
        public string CHARGE_CODE { get; set; }
        public string CHARGE_EDESC { get; set; }
    }


    public class ChargeSetupDataModel
    {
        public string CHARGE_CODE { get; set; }            // VARCHAR2(10) NOT NULL
        public string ACC_CODE { get; set; }               // VARCHAR2(30)
        public string ACC_CODE_EDESC { get; set; }
        public string LINK_SUB_CODE { get; set; }          // VARCHAR2(30)
        public string BUDGET_CODE { get; set; }            // VARCHAR2(30)
        public string CHARGE_TYPE_FLAG { get; set; }       // CHAR(1) NOT NULL
        public string VALUE_PERCENT_FLAG { get; set; }     // CHAR(1)
        public decimal? VALUE_PERCENT_AMOUNT { get; set; } // NUMBER(12,2)
        public string GL_FLAG { get; set; }                // CHAR(1)
        public int? PRIORITY_INDEX_NO { get; set; }        // NUMBER(2)
        public string CHARGE_APPLY_ON { get; set; }        // CHAR(1)
        public DateTime? APPLY_FROM_DATE { get; set; }     // DATE
        public DateTime? APPLY_TO_DATE { get; set; }       // DATE
        public string FORM_CODE { get; set; }              // VARCHAR2(10) NOT NULL
        public string COMPANY_CODE { get; set; }           // VARCHAR2(30) NOT NULL
        public string CREATED_BY { get; set; }             // VARCHAR2(30) NOT NULL
        public DateTime CREATED_DATE { get; set; }         // DATE NOT NULL
        public string DELETED_FLAG { get; set; }           // CHAR(1)
        public string APPORTION_ON { get; set; }           // CHAR(1)
        public string IMPACT_ON { get; set; }              // CHAR(1)
        public string APPLY_ON { get; set; }               // CHAR(1)
        public string SYN_ROWID { get; set; }              // VARCHAR2(18)
        public string CHARGE_ACTIVE_FLAG { get; set; }     // CHAR(1)
        public DateTime? MODIFY_DATE { get; set; }         // DATE
        public string NON_GL_FLAG { get; set; }            // CHAR(1)
        public string IMPACT_ON_FLAG { get; set; }         // CHAR(2) DEFAULT 'TP'
        public string MODIFY_BY { get; set; }              // VARCHAR2(30)
        public string CALC_ITEM_BASED_ON { get; set; }     // CHAR(1)
        public string MANUAL_CALC_CHARGE { get; set; }     // CHAR(1)
        public string ON_ITEM { get; set; }                // CHAR(1)
        public string HIDE_PER_FLAG { get; set; }          // CHAR(1) DEFAULT 'N' NOT NULL
    }

    public class QualityCheckGetDataModel
    {
        public string None { get; set; }
        public string SERIAL_TRACKING_FLAG { get; set; }
        public string BATCH_TRACKING_FLAG { get; set; }
        public string batchSerialFlag { get; set; }
        public string FORM_CODE { get; set; }

    }



    public class MiscellaneousTabDataModel
    {
        public string FORM_CODE { get; set; }
        public string PRINT_REPORT_FLAG { get; set; }
        public string PRIMARY_MANUAL_FLAG { get; set; }
        public string ACCESS_BDFSM_FLAG { get; set; }
        public string MULTI_UNIT_FLAG { get; set; }
        public string COPY_VALUES_FLAG { get; set; }               // Copy common values at each new record insert
        public string RATE_SCHEDULE_FIX_PRICE { get; set; }
        public string PRICE_CONTROL_FLAG { get; set; }
        public string NEGATIVE_STOCK_FLAG { get; set; }            // Block Negative Stock
        public string FREEQTY_FLAG { get; set; }                   // Freeze Discount Schedule Rate
        public string DC_VAT_FLAG { get; set; }                    // DC VAT Flag
        public string COMMITMENT_FLAG { get; set; }                // Party Commitment Flag
        public string PURCHASE_MRR_GRNI_FLAG { get; set; }         // Purchase GRNI Account
        public string DISPLAY_RATE { get; set; }                   // Display Rate Schedule
        public string PENDING_INFO_FLAG { get; set; }              // Pending Indent/ Order Info
        public string WM_FLAG { get; set; }                        // Warehouse Management Flag
        public string OTHER_INFO_FLAG { get; set; }                // Other Information Transaction
        public string VNO_AS_DOC_ID_CONTROL { get; set; }          // V No. Generate as Session ID
        public string PUBLIC_FLAG { get; set; }
        public string REF_FIX_QUANTITY { get; set; }
        public string REF_FIX_PRICE { get; set; }
        public string INFO_FLAG { get; set; }
        public string DISCOUNT_SCHEDULE_FLAG { get; set; }

        public string TOTAL_ROUND_FLAG { get; set; }
        public int FREEZE_BACK_DAYS { get; set; }
        public int TOTAL_ROUND_INDEX { get; set; }
        public int DECIMAL_PLACE { get; set; }
        public int MAX_ROWS { get; set; }
        public int? REPORT_NO { get; set; }
        public string REPORT_EDESC { get; set; }


        public string PURCHASE_INVOICE_MRR_FLAG { get; set; }
        public string AUTO_GL_POST { get; set; }
        public string SALES_INVOICE_CHALAN_FLAG { get; set; }
    }


    public class DocumentReportSetupModel
    {
        public string REPORT_EDESC { get; set; }
        public string REPORT_NO { get; set; }
    }


}