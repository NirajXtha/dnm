using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NeoERP.DocumentTemplate.Service.Models
{
    public class FormSetup
    {
        public string FORM_CODE { get; set; }
        public string FORM_EDESC { get; set; }
        public string FORM_NDESC { get; set; }
        public string DR_ACC_CODE { get; set; }
        public string CR_ACC_CODE { get; set; }
        public string MASTER_FORM_CODE { get; set; }
        public string PRE_FORM_CODE { get; set; }
        public string MODULE_CODE { get; set; }
        public string TEMPLATE_CODE { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string NUMBERING_FORMAT { get; set; }
        public string DATE_FORMAT { get; set; }
        public string START_ID_FLAG { get; set; }
        public string ID_GENERATION_FLAG { get; set; }
        public string CUSTOM_PREFIX_TEXT { get; set; }
        public string CUSTOM_SUFFIX_TEXT { get; set; }
        public int PREFIX_LENGTH { get; set; }
        public int SUFFIX_LENGTH { get; set; }
        public int BODY_LENGTH { get; set; }

        public int START_NO { get; set; }

        public int LAST_NO { get; set; }

        public DateTime? START_DATE { get; set; }
        public DateTime? LAST_DATE { get; set; }

        public string REF_COLUMN_NAME { get; set; }
        public string PRINT_REPORT_FLAG { get; set; }
        public string PRIMARY_MANUAL_FLAG { get; set; }

        public string COPY_VALUES_FLAG { get; set; }

        public string QUALITY_CHECK_FLAG { get; set; }

        public string SERIAL_TRACKING_FLAG { get; set; }

        public string BATCH_TRACKING_FLAG { get; set; }

        public string ACC_CODE { get; set; }

        public string REMARKS { get; set; }

        public string COMPANY_CODE { get; set; }

        public string CREATED_BY { get; set; }

        public DateTime CREATED_DATE { get; set; }

        public string DELETED_FLAG { get; set; }
        public string COINAGE_FLAG { get; set; }
        public string COINAGE_SUB_CODE { get; set; }
        public string REFERENCE_FLAG { get; set; }
        public string REF_TABLE_NAME { get; set; }
        public string PUBLIC_FLAG { get; set; }
        public string FORM_ACTION_FLAG { get; set; }
        public string TOTAL_ROUND_FLAG { get; set; }
        public int TOTAL_ROUND_INDEX { get; set; }
        public string REF_FORM_CODE { get; set; }
        public string INTER_BRANCH_FLAG { get; set; }

        public int REPORT_NO { get; set; }
        public string MULTI_UNIT_FLAG { get; set; }

        public string DELTA_FLAG { get; set; }
        public string SYN_ROWID { get; set; }
        public string SALES_INVOICE_CHALAN_FLAG { get; set; }
        public string REF_FIX_QUANTITY { get; set; }
        public string REF_FIX_PRICE { get; set; }
        public int FREEZE_BACK_DAYS { get; set; }

        public string AUTO_GL_POST { get; set; }

        public string RECEIPT_FLAG { get; set; }
        public string RECEIPT_FORM_CODE { get; set; }
        public string RECEIPT_CASH_ACC_CODE { get; set; }


        public string PURCHASE_INVOICE_MRR_FLAG { get; set; }

        public int DECIMAL_PLACE { get; set; }

        public string PURCHASE_EXPENSES_FLAG { get; set; }
        public string BATCH_FLAG { get; set; }
        public string RATE_SCHEDULE_FIX_PRICE { get; set; }
        public string FREEZE_MANUAL_ENTRY_FLAG { get; set; }
        public string ADVANCE_FLAG { get; set; }
        public string RT_CONTROL_FLAG { get; set; }
        public string FORM_TYPE { get; set; }
        public string COSTING_FLAG { get; set; }
        public string NEGATIVE_STOCK_FLAG { get; set; }
        public string QC_PARAMETER_FLAG { get; set; }
        public DateTime? MODIFY_DATE { get; set; }
        public string OTHER_INFO_FLAG { get; set; }
        public string CUSTOM_MANUAL_PREFIX_TEXT { get; set; }
        public string CUSTOM_MANUAL_SUFFIX_TEXT { get; set; }
        public int BODY_MANUAL_LENGTH { get; set; }
        public int START_MANUAL_NO { get; set; }
        public string ORDER_ACCESS_FAB_FLAG { get; set; }
        public string LOT_GEN_FLAG { get; set; }
        public string AFTER_VERIFY_FLAG { get; set; }
        public string ONLINE_FLAG { get; set; }
        public string ADD_ROW_FLAG { get; set; }
        public string PRICE_CONTROL_FLAG { get; set; }
        public string RATE_DIFF_FLAG { get; set; }
        public string FREEZE_MASTER_REF_FLAG { get; set; }
        public string ACCESS_BDFSM_FLAG { get; set; }
        public string INFO_FLAG { get; set; }
        public string WM_FLAG { get; set; }
        public string FREEQTY_FLAG { get; set; }
        public string DC_VAT_FLAG { get; set; }
        public string COMMITMENT_FLAG { get; set; }
        public string ORDER_DISPATCH_FLAG { get; set; }


        public string PURCHASE_MRR_GRNI_FLAG { get; set; }
        public string PENDING_INFO_FLAG { get; set; }
        public string BACK_DATE_VNO_SAVE_FLAG { get; set; }
        public string AFTER_POSTING_FLAG { get; set; }
        public string DC_TDS_FLAG { get; set; }
        public string PAYMENT_MODE_FLAG { get; set; }
        public string INVOICE_PJV_FORM_CODE { get; set; }
        public string DISCOUNT_SCHEDULE_FLAG { get; set; }
        public string MODIFY_BY { get; set; }
        public string ROLL_TRACKING_FLAG { get; set; }
        public string FORM_URL { get; set; }

    }

    public class MenuModels
    {
        public string FORM_CODE { get; set; }
        public string FORM_EDESC { get; set; }
        public string MENU_DESC { get; set; }
        public string MODULE_CODE { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string PRE_FORM_CODE { get; set; }
        public string MASTER_FORM_CODE { get; set; }
        public string FORM_TYPE { get; set; }
        public string CHILD_FORM_CODE { get; set; }
        public string MENU_NO { get; set; }
        public string MENU_EDESC { get; set; }
        public string FULL_PATH { get; set; }
        public string MODULE_ABBR { get; set; }
        public string ICON_PATH { get; set; }
        public string COLOR { get; set; }
        public string MENU_OBJECT_NAME { get; set; }
        public string PRE_MENU_NO { get; set; }
    }

    //pricelist
    //public class MasterFieldForUpdate
    //{
    //    public int MASTER_ID { get; set; }
    //    public string PRICE_LIST_NAME { get; set; }
    //    public int STATUS { get; set; }
    //    public string COMPANY { get; set; }
    //    public DateTime DATE_ENGLISH { get; set; }
    //    public string CREATED_BY { get; set; }
    //    public DateTime CREATED_DATE { get; set; }
    //    public string MODIFIED_BY { get; set; }
    //    public DateTime MODIFIED_DATE { get; set; }

    //}
    public class CustomerItemMapModel
    {
        public string CUSTOMER_CODE { get; set; }
        public string ITEM_CODE { get; set; }
        public string REMARKS { get; set; }
        public string COMPANY_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public char? DELETED_FLAG { get; set; }
        public DateTime? DELETED_DATE { get; set; }
        public string DELETED_BY { get; set; }
        public string SYN_ROWID { get; set; }
        public DateTime? MODIFY_DATE { get; set; }
        public string MODIFY_BY { get; set; }
    }
    public class CustomerModels
    {
        public CustomerModels()
        {
            ownerInfo = new List<OwnerModels>();
            ownerFiles = new List<CustomerOwnerFileModel>();
            customerInvoiceWiseOpening = new List<CustomerInvoiceWiseOpeningSetup>();
            customerDivision = new List<CustomerDivisionModels>();
            customerInfoList = new List<CustomerInfo>();
            alternativeLocationInfoList = new List<AlternativeLocationInfo>();
            budgetCenterList = new List<CustomerBudgetCenter>();
            sisterConcernsList = new List<CustomerSisterConcern>();
            otherTermsConditionsList = new List<CustomerOtherTermsConditions>();
            customerStockStatusList = new List<CustomerStockStatus>();
            customerItemMapping = new List<int>();
            customerBankMapping = new List<CustomerBankDetailModel>();
        }
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string CUSTOMER_NDESC { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string MASTER_CUSTOMER_CODE { get; set; }
        public string PRE_CUSTOMER_CODE { get; set; }
        public int? Childrens { get; set; }
        public int LEVEL { get; set; }
        public string REGD_OFFICE_EADDRESS { get; set; }
        public string REGD_OFFICE_NADDRESS { get; set; }
        public string TELEPHONE { get; set; }
        public string TEL_MOBILE_NO1 { get; set; }
        public string TEL_MOBILE_NO2 { get; set; }
        public string CUSTOMER_PREFIX { get; set; }
        public int? CUSTOMER_STARTID { get; set; }
        public string REMARKS { get; set; }
        public string CUSTOMER_ACCOUNT { get; set; }
        public string CUSTOMER_TYPE { get; set; }
        public int? maxCustomerCode { get; set; }
        public int? maxCustomerMasterCode { get; set; }
        public string CUSTOMER_FLAG { get; set; }
        public string ACC_CODE { get; set; }
        public string CHILD_AUTOGENERATED { get; set; }
        public string PERMANENT_ADDRESS { get; set; }
        public string TEMPORARY_ADDRESS { get; set; }
        public string FAX { get; set; }
        public string ZONE_CODE { get; set; }
        public string COUNTRY_CODE { get; set; }
        public string REGION_CODE { get; set; }
        public string DISTRICT_CODE { get; set; }
        public string CITY_CODE { get; set; }
        public string TELEPHONE2 { get; set; }
        public string TELEPHONE3 { get; set; }
        public string PAN_VAT { get; set; }
        public string EXCISE { get; set; }
        public string EMAIL { get; set; }
        public string PARTY_TYPE_CODE { get; set; }
        public string ACCOUNTMAP { get; set; }
        public string AGENT_CODE { get; set; }
        public string DEALING_PERSON { get; set; }
        public string BRANCH_CODE { get; set; }
        public string CASH_CUSTOMER_FLAG { get; set; }
        public string ACTIVE_FLAG { get; set; }
        public string AfterSaveCustomerCode { get; set; }
        public decimal? CREDIT_RATE { get; set; }
        public decimal? CREDIT_LIMIT { get; set; }
        public decimal? CUSHION_PERCENT { get; set; }
        public decimal? PRE_CREDIT_LIMIT { get; set; }
        public decimal? DUE_BILL_COUNT { get; set; }
        public decimal? EXCEED_LIMIT_PERCENTAGE { get; set; }
        public decimal? INTEREST_RATE { get; set; }
        public decimal? BANK_GUARANTEE { get; set; }
        public DateTime? OPENING_DATE { get; set; }
        public DateTime? EXPIRY_DATE { get; set; }
        public DateTime? MATURITY_DATE { get; set; }
        public decimal? DISCOUNT_FLAT_RATE { get; set; }
        public int? DISCOUNT_DAYS { get; set; }
        public decimal? DISCOUNT_PERCENT { get; set; }
        public string EXCLUSIVE_FLAG { get; set; }
        public int? CREDIT_DAYS { get; set; }
        public string CREDIT_ACTION_FLAG { get; set; }
        public string TERMS_CONDITIONS { get; set; }
        public string CUSTOMER_GROUP_ID { get; set; }
        public string CUSTOMER_ID { get; set; }
        public string CHILD_CUSTOMER_CODE { get; set; }
        public string CHILD_UPDATE_CUSTOMER_CODE { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public string PARENT_CUSTOMER_CODE { get; set; }
        public string PR_CODE { get; set; }
        public string TPIN_VAT_NO { get; set; }
        public string SYN_ROWID { get; set; }
        public string DELTA_FLAG { get; set; }
        public string APPROVED_FLAG { get; set; }
        public int? MAX_SALES_VALUE { get; set; }
        public string TEL_MOBILE_NO3 { get; set; }
        public int? GROUP_START_NO { get; set; }
        public string PREFIX_TEXT { get; set; }
        public string EXCISE_NO { get; set; }
        public string FAX_NO { get; set; }
        public string LINK_SUB_CODE { get; set; }
        public string PRICE_LIST_ID { get; set; }
        public string EXPORT_FLAG { get; set; }
        public string GST_NO { get; set; }
        public string IEC_NO { get; set; }
        public string TIN { get; set; }
        public string FSSAI_NO { get; set; }
        public string AD_CODE { get; set; }
        public DateTime? MODIFY_DATE { get; set; }
        public string MODIFY_BY { get; set; }
        public List<OwnerModels> ownerInfo { get; set; }
        public List<CustomerOwnerFileModel> ownerFiles { get; set; }
        public List<CustomerInvoiceWiseOpeningSetup> customerInvoiceWiseOpening { get; set; }
        public List<CustomerDivisionModels> customerDivision { get; set; }
        public List<CustomerInfo> customerInfoList { get; set; }
        public List<AlternativeLocationInfo> alternativeLocationInfoList { get; set; }
        public List<CustomerBankDetailModel> customerBankMapping { get; set; }
        public List<CustomerBudgetCenter> budgetCenterList { get; set; }
        public List<CustomerSisterConcern> sisterConcernsList { get; set; }
        public List<CustomerOtherTermsConditions> otherTermsConditionsList { get; set; }
        public List<CustomerStockStatus> customerStockStatusList { get; set; }
        public List<int> customerItemMapping { get; set; }
    }
    public class CustomerBankDetailModel
    {
        public string CUSTOMER_CODE { get; set; }
        public string BANK_NAME { get; set; }
        public string BANK_BRANCH { get; set; }
        public string BANK_ACC_NO { get; set; }
        public string ACC_CODE { get; set; }
        public string ACC_EDESC { get; set; }
        public string COMPANY_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public DateTime? DELETED_DATE { get; set; }
        public string DELETED_BY { get; set; }
        public string SYN_ROWID { get; set; }
        public DateTime? MODIFY_DATE { get; set; }
        public string MODIFY_BY { get; set; }
    }
    public class CustomerOwnerFileModel
    {
        public string COMPANY_CODE { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string OWNER_NAME { get; set; }
        public string FILE_COLUMN_NAME { get; set; }
        public string ORIGINAL_FILENAME { get; set; }
        public string STORED_FILENAME { get; set; }
        public string FILE_URL { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public DateTime? MODIFIED_DATE { get; set; }
        public string DELETED_BY { get; set; }
        public DateTime? DELETED_DATE { get; set; }
        public string DELETED_FLAG { get; set; } = "N";
    }
    public class CustomerOwnerFileUploadModel
    {
        public string CUSTOMER_CODE { get; set; }
        public string OWNER_NAME { get; set; }
        public string FILE_COLUMN_NAME { get; set; }
        public HttpPostedFileBase file { get; set; }
        public string Path { get; set; }
    }

    public class CustomerStockStatus
    {
        public string CUSTOMER_CODE { get; set; }
        public string ITEM_CODE { get; set; }
        public DateTime STOCK_DATE { get; set; }
        public int QUANTITY { get; set; }
        public string REMARKS { get; set; }
        public string COMPANY_CODE { get; set; }

    }


    public class CustomerOtherTermsConditions
    {
        public string CUSTOMER_CODE { get; set; }
        public string FIELD_NAME { get; set; }
        public string FIELD_VALUE { get; set; }
        public string REMARKS { get; set; }
        public string COMPANY_CODE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string SYN_ROWID { get; set; }
    }

    public class CustomerSisterConcern
    {
        public string CUSTOMER_CODE { get; set; }
        public string SISTER_CONCERN_EDESC { get; set; }
        public string SISTER_CONCERN_NDESC { get; set; }
        public string REMARKS { get; set; }
        public string COMPANY_CODE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string SYN_ROWID { get; set; }
    }

    public class CustomerBudgetCenter
    {
        public string CUSTOMER_CODE { get; set; }
        public string BUDGET_CODE { get; set; }
        public string REMARKS { get; set; }
        public string COMPANY_CODE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string SYN_ROWID { get; set; }
    }

    public class AlternativeLocationInfo
    {
        public string SYN_ROWID { get; set; }
        public string LOCATION_CODE { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string OFFICE_EDESC { get; set; }
        public string OFFICE_NDESC { get; set; }
        public string CONTACT_PERSON { get; set; }
        public string ADDRESS { get; set; }
        public string TEL_MOBILE_NO { get; set; }
        public string FAX_NO { get; set; }
        public string EMAIL { get; set; }
        public string REMARKS { get; set; }
    }

    public class CustomerInfo
    {
        public string CUSTOMER_CODE { get; set; }
        public string FIELD_NAME { get; set; }
        public string FIELD_VALUE { get; set; }
        public string REMARKS { get; set; }
        public string COMPANY_CODE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string SYN_ROWID { get; set; }
    }

    public class CustomerInvoiceWiseOpeningSetup
    {
        public string DIVISION_CODE { get; set; }
        public string PARTY_TYPE_CODE { get; set; }
        public string EMPLOYEE_CODE { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string REFERENCE_NO { get; set; }
        public DateTime INVOICE_DATE { get; set; }
        public decimal BALANCE_AMOUNT { get; set; }
        public string REMARKS { get; set; }
        public string DELETED_FLAG { get; set; }
        public string CURRENCY_CODE { get; set; }
        public decimal? EXCHANGE_RATE { get; set; }
        public DateTime? DUE_DATE { get; set; }
        public string TRANSACTION_TYPE { get; set; }
        public string SYN_ROWID { get; set; }
        public string BRANCH_CODE { get; set; }
        public string ACC_CODE { get; set; }
    }

    public class CustomerDivisionModels
    {
        public string CUSTOMER_CODE { get; set; }
        public decimal CREDIT_LIMIT { get; set; }
        public string REMARKS { get; set; }
        public string DIVISION_CODE { get; set; }
        public string BLOCK_FLAG { get; set; }
    }

    public class OwnerModels
    {
        public string ADDRESS { get; set; }
        public string CONTACT_PERSON { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string DESIGNATION { get; set; }
        public string EMAIL { get; set; }
        public string FAX_NO { get; set; }
        public string IMAGE_FILE_CITIZENSHIP { get; set; }
        public string IMAGE_FILE_COMPANY_PAN { get; set; }
        public string IMAGE_FILE_COMPANY_REG { get; set; }
        public string IMAGE_FILE_NAME { get; set; }
        public string OWNER_NAME { get; set; }
        public string REMARKS { get; set; }
        public string TEL_MOBILE_NO { get; set; }

    }

    public class CustomersTree
    {
        public string customerName { get; set; }
        public string customerId { get; set; }
        public bool hasCustomers { get; set; }
        public int Level { get; set; }
        public string preCustomerCode { get; set; }
        public string masterCustomerCode { get; set; }
        public string groupSkuFlag { get; set; }
        public string CUSTOMER_FLAG { get; set; }
        public string PARTY_TYPE_CODE { get; set; }
        public string ACC_CODE { get; set; }
        public int? CUSTOMER_STARTID { get; set; }
        public string CUSTOMER_PREFIX { get; set; }
        public string REMARKS { get; set; }
        public string CUSTOMER_NDESC { get; set; }
        public string PARENT_CUSTOMER_CODE { get; set; }
        public IEnumerable<CustomersTree> Items { get; set; }
    }
    public class CustomersTreeModel
    {
        public int LEVEL { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string CUSTOMER_TYPE_CODE { get; set; }
        public string CUSTOMER_FLAG { get; set; }
        public string PRE_CUSTOMER_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }

        public bool HAS_BRANCH { get; set; }

        public List<CustomersTreeModel> ITEMS { get; set; }

    }
    //delar setup
    public class DealerTree
    {
        public string dealerName { get; set; }
        public string dealerId { get; set; }
        public bool hasDealers { get; set; }
        public int Level { get; set; }
        public string preDealerCode { get; set; }
        public string masterDealerCode { get; set; }
        public string groupSkuFlag { get; set; }
        public string PARENT_DEALER_CODE { get; set; }
        public IEnumerable<DealerTree> Items { get; set; }
        public string PARTY_TYPE_EDESC { get; set; }
        public string PARTY_TYPE_CODE { get; set; }
        public string PRE_PARTY_CODE { get; set; }
        public string MASTER_PARTY_CODE { get; set; }
    }




    public class FormsTree
    {
        public FormsTree()
        {
            Items = new List<FormsTree>();
        }
        public string formName { get; set; }
        public string formId { get; set; }
        public bool hasForms { get; set; }
        public int Level { get; set; }
        public string preFormCode { get; set; }
        public string masterFormCode { get; set; }
        public string groupSkuFlag { get; set; }
        public string moduleCode { get; set; }
        public string urlForSetup { get; set; }
        public string iconPath { get; set; }
        public List<FormsTree> Items { get; set; }
    }
    public class DescriptionMenu
    {
        //public FormsTree()
        //{
        //    Items = new List<FormsTree>();
        //}
        public string formName { get; set; }
        public string formId { get; set; }
        public bool hasForms { get; set; }
        public int Level { get; set; }
        public string preFormCode { get; set; }
        public string masterFormCode { get; set; }
        public string groupSkuFlag { get; set; }
        public string moduleCode { get; set; }
        public string urlForSetup { get; set; }
        public string iconPath { get; set; }
        public List<FormsTree> descItems { get; set; }
    }
    public class SupplierModels
    {
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_EDESC { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string MASTER_SUPPLIER_CODE { get; set; }

        public string ACC_CODE { get; set; }
        public string PARTY_TYPE_CODE { get; set; }
        public string PRE_SUPPLIER_CODE { get; set; }
        public int? Childrens { get; set; }
        public int LEVEL { get; set; }
        public string REGD_OFFICE_EADDRESS { get; set; }
        public string TEL_MOBILE_NO1 { get; set; }
        public string PARENT_SUPPLIER_CODE { get; set; }
    }

    public class SuppliersTree
    {
        public string supplierName { get; set; }
        public string supplierId { get; set; }
        public bool hasSuppliers { get; set; }
        public string ACC_CODE { get; set; }
        public string PARTY_TYPE_CODE { get; set; }
        public int Level { get; set; }
        public string preSupplierCode { get; set; }
        public string masterSupplierCode { get; set; }
        public string groupSkuFlag { get; set; }
        public IEnumerable<SuppliersTree> Items { get; set; }
    }

    public class ProductsModels
    {
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string MASTER_ITEM_CODE { get; set; }
        public string PRE_ITEM_CODE { get; set; }
        public int? Childrens { get; set; }
        public int LEVEL { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public string MU_EDESC { get; set; }
        public string CATEGORY_EDESC { get; set; }
        public decimal? PURCHASE_PRICE { get; set; }
    }

    public class TNCModels
    {
        public string TNC_CODE { get; set; }
        public string TNC_EDESC { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string MASTER_TNC_CODE { get; set; }
        public string PRE_TNC_CODE { get; set; }
    }
    public class PModels
    {
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string MASTER_ITEM_CODE { get; set; }
        public string PRE_ITEM_CODE { get; set; }
        public int? Childrens { get; set; }
        public int LEVEL { get; set; }
        public string CREATED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public string MU_EDESC { get; set; }
        public string CATEGORY_EDESC { get; set; }
        public string PURCHASE_PRICE { get; set; }
        public string ATTRIBUTE_CODE { get; set; }
    }

    public class ProductsTree
    {
        public string itemName { get; set; }
        public string itemCode { get; set; }
        public bool hasItems { get; set; }
        public int Level { get; set; }
        public string preItemCode { get; set; }
        public string masterItemCode { get; set; }
        public string groupSkuFlag { get; set; }
        public IEnumerable<ProductsTree> Items { get; set; }
    }

    public class TNCTree
    {
        public string tncName { get; set; }
        public string tncCode { get; set; }
        public bool hasTNC { get; set; }
        public string preTNCCode { get; set; }
        public string masterTNCCode { get; set; }
        public string groupSkuFlag { get; set; }
        public IEnumerable<TNCTree> TNC { get; set; }
    }
    public class AreaModels
    {
        public string AREA_CODE { get; set; }
        public string AREA_EDESC { get; set; }
        public string COMPANY_CODE { get; set; }
        public string REMARKS { get; set; }
        public string DELETED_FLAG { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string MODIFY_DATE { get; set; }
        public string MODIFY_BY { get; set; }
    }

    public class TransporterModels
    {
        public string TRANSPORTER_CODE { get; set; }
        public string TRANSPORTER_EDESC { get; set; }
        public decimal? PAN_NO { get; set; }
        public decimal? DEPOSIT_AMOUNT { get; set; }
        public string PROPRITER_NAME { get; set; }
        public string PHONE_NO { get; set; }
        public string ADDRESS { get; set; }
        public string REMARKS { get; set; }
        public string COMPANY_CODE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public DateTime? MODIFY_DATE { get; set; }
        public string SYN_ROWID { get; set; }
        public string MODIFY_BY { get; set; }
        public int? PRIORITY { get; set; }
        public string TRANSPORTER_NDESC { get; set; }
    }
    public class RegionalModels
    {
        public string REGION_CODE { get; set; }
        public string REGION_EDESC { get; set; }
        public string REGION_NDESC { get; set; }
        public string REMARKS { get; set; }
        public string PRE_REGION_CODE { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public int? Childrens { get; set; }
        public int LEVEL { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string COMPANY_CODE { get; set; }
        public string SYN_ROWID { get; set; }
        public DateTime? MODIFY_DATE { get; set; }
        public string MODIFY_BY { get; set; }
    }

    public class MiscellaneousSubLedgerModels
    {
        public string MISC_CODE { get; set; }
        public string MISC_EDESC { get; set; }
        public string REGD_OFFICE_EADDRESS { get; set; }
        public string TEL_MOBILE_NO1 { get; set; }
        public string VAT_NO { get; set; }
        public string LINK_SUB_CODE { get; set; }
        public string ACC_CODE { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string MASTER_MISC_CODE { get; set; }
        public string PRE_MISC_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string SYN_ROWID { get; set; }
        public char? DELETED_FLAG { get; set; }
        public DateTime? MODIFY_DATE { get; set; }
        public string MODIFY_BY { get; set; }
        public string MISC_ID { get; set; }
        public int? Childrens { get; set; }
        public int LEVEL { get; set; }
    }

    public class AttributeModels
    {
        public string ATTRIBUTE_CODE { get; set; }
        public string ATTRIBUTE_EDESC { get; set; }
        public string ATTRIBUTE_NDESC { get; set; }
        public string REMARKS { get; set; }
        public string PRE_ATTRIBUTE_CODE { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public int? Childrens { get; set; }
        public int LEVEL { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string COMPANY_CODE { get; set; }
        public string SYN_ROWID { get; set; }
        public DateTime? MODIFY_DATE { get; set; }
        public string MODIFY_BY { get; set; }
    }
    public class AgentModels
    {
        public string AGENT_CODE { get; set; }
        public string AGENT_NDESC { get; set; }
        public string AGENT_EDESC { get; set; }
        public string AGENT_TYPE { get; set; }
        public string REMARKS { get; set; }
        public decimal? CREDIT_LIMIT { get; set; }
        public int? CREDIT_DAYS { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string DELETED_FLAG { get; set; }
        public DateTime? MODIFY_DATE { get; set; }
        public string MODIFY_BY { get; set; }
        public string AGENT_ID { get; set; }
        public string PAN_NO { get; set; }
        public string ADDRESS { get; set; }
    }

    //public class SchemeModels
    //{
    //    public string SCHEME_CODE { get; set; }
    //    public string SCHEME_EDESC { get; set; }
    //    public string FORM_CODE { get; set; }

    //    public string STATUS { get; set; }
    //    public string SCHEME_TYPE { get; set; }
    //    public string TYPE { get; set; }

    //    public DateTime? CALCULATION_DAYS { get; set; }
    //    public string ACCOUNT_CODE { get; set; }

    //    public string CUSTOMER_CODE { get; set; }

    //    public string PARTY_TYPE_CODE { get; set; }

    //    public string ITEM_CODE { get; set; }

    //    public string FOC_ITEM_CODE { get; set; }

    //    public string GROUP_CODE { get; set; }
    //    public string AREA_CODE { get; set; }
    //    public string CHARGE_CODE { get; set; }

    //    public string QUERY_STRING { get; set; }
    //    public decimal? FROM_QUANTITY { get; set; }

    //    public decimal? TO_QUANTITY { get; set; }

    //    public string DISCOUNT_TYPE { get; set; }

    //    public decimal? DISCOUNT { get; set; }

    //    public DateTime? EFFECTIVE_FROM { get; set; }

    //    public DateTime? EFFECTIVE_TO { get; set; }

    //    public string CREATED_BY { get; set; }
    //    public DateTime? CREATED_DATE { get; set; }
    //    public string COMPANY_CODE { get; set; }
    //    public string BRANCH_CODE { get; set; }
    //    public string DELETED_FLAG { get; set; }
    //    public DateTime? MODIFY_DATE { get; set; }
    //    public string MODIFY_BY { get; set; }

    //}
    public class SchemeDetailsModel
    {
        public int SNO { get; set; }
        public string SCHEME_CODE { get; set; }
        public string SCHEME_EDESC { get; set; }
        public string SCHEME_TYPE { get; set; }
        public string TYPE { get; set; }
        public string STATUS { get; set; }
        public string ACCOUNT_CODE { get; set; }
        public string ACC_EDESC { get; set; }
        public string ACC_EDESC_MULTI { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string CUSTOMER_EDESC_MULTI { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string ITEM_EDESC_MULTI { get; set; }
        public string PARTY_TYPE_CODE { get; set; }
        public string PARTY_TYPE_EDESC { get; set; }
        public string PARTY_TYPE_EDESC_MULTI { get; set; }
        public string AREA_CODE { get; set; }

        public string AREA_EDESC { get; set; }
        public string AREA_EDESC_MULTI { get; set; }
        public string BRANCH_CODE { get; set; }

        public string BRANCH_EDESC { get; set; }

        public string BRANCH_EDESC_MULTI { get; set; }
        public string CHARGE_CODE { get; set; }

        public string CHARGE_EDESC { get; set; }
        public string CHARGE_ACCOUNT_CODE { get; set; }
        public decimal? CHARGE_RATE { get; set; }
        public DateTime? EFFECTIVE_FROM { get; set; }
        public DateTime? EFFECTIVE_TO { get; set; }

        public decimal? SALES_SCHEME_VALUE { get; set; }
        public decimal? TOTAL_SALES { get; set; }
        public decimal? SALES_DISCOUNT { get; set; }
    }
    public class SchemeListModel
    {
        public string CUSTOMER_CODE { get; set; }
        public string PARTY_TYPE_CODE { get; set; }
        public decimal? QTY { get; set; }
        public decimal? BONUS_AMT { get; set; }

        public decimal? SALES_AMT { get; set; }
    }
    public class SchemeModels
    {
        public string SCHEME_CODE { get; set; }
        public string SCHEME_EDESC { get; set; }
        public string SCHEME_TYPE { get; set; }
        public string TYPE { get; set; }
        public string STATUS { get; set; }
        public DateTime? CALCULATION_DAYS { get; set; }
        public string FORM_CODE { get; set; }
        public string FORM_EDESC { get; set; }
        public string ACCOUNT_CODE { get; set; }
        public string ACC_EDESC { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string PARTY_TYPE_CODE { get; set; }
        public string PARTY_TYPE_EDESC { get; set; }
        public string AREA_CODE { get; set; }

        public string AREA_EDESC { get; set; }
        public string BRANCH_CODE { get; set; }

        public string BRANCH_EDESC { get; set; }
        public string CHARGE_CODE { get; set; }

        public string CHARGE_EDESC { get; set; }
        public string CHARGE_ACCOUNT_CODE { get; set; }
        public decimal? CHARGE_RATE { get; set; }
        public DateTime? EFFECTIVE_FROM { get; set; }
        public DateTime? EFFECTIVE_TO { get; set; }
        public string QUERY_STRING { get; set; }

        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string COMPANY_CODE { get; set; }

        public string DELETED_FLAG { get; set; }
        public DateTime? MODIFY_DATE { get; set; }
        public string MODIFY_BY { get; set; }
        public string IMPLEMENT_FLAG { get; set; }
        public string REMARKS { get; set; }

    }
    public class ResourceCodeModels
    {
        public DateTime? CREATED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string SYN_ROWID { get; set; }
        public DateTime? MODIFY_DATE { get; set; }
        public string MODIFY_BY { get; set; }
        public string RESOURCE_TYPE { get; set; }
        public string RESOURCE_CODE { get; set; }
        public string RESOURCE_EDESC { get; set; }
        public string RESOURCE_NDESC { get; set; }
        public string RESOURCE_FLAG { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string PRE_RESOURCE_CODE { get; set; }
        public string REMARKS { get; set; }
        public string COMPANY_CODE { get; set; }
        public string CREATED_BY { get; set; }
    }
    public class AccountCodeModels
    {
        public string ACC_CODE { get; set; }
        public string ACC_EDESC { get; set; }
        public string ACC_TYPE_FLAG { get; set; }
        public string MASTER_ACC_CODE { get; set; }
        public string PRE_ACC_CODE { get; set; }
        public int? Childrens { get; set; }
        public int LEVEL { get; set; }
        public string TRANSACTION_TYPE { get; set; }
        public string ACC_NATURE { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_DATE { get; set; }
        //public string REGD_OFFICE_EADDRESS { get; set; }
        //public string TEL_MOBILE_NO1 { get; set; }
    }
    public class BudgetCenterCodeModels
    {
        public string BUDGET_CODE { get; set; }
        public string BUDGET_EDESC { get; set; }
        public string BUDGET_TYPE_FLAG { get; set; }
        public string MASTER_BUDGET_CODE { get; set; }
        public string PRE_BUDGET_CODE { get; set; }
        public int? Childrens { get; set; }
        public int LEVEL { get; set; }
        //public string REGD_OFFICE_EADDRESS { get; set; }
        //public string TEL_MOBILE_NO1 { get; set; }
    }
    public class AccountCodeTree
    {
        public string AccountName { get; set; }
        public string AccountId { get; set; }
        public bool hasAccount { get; set; }
        public int Level { get; set; }
        public string preAccountCode { get; set; }
        public string masterAccountCode { get; set; }
        public string accounttypeflag { get; set; }
        public IEnumerable<AccountCodeTree> Items { get; set; }
    }
    public class BudgetCenterCodeTree
    {
        public string BudgetCenterName { get; set; }
        public string BudgetCenterId { get; set; }
        public bool hasBudgetCenter { get; set; }
        public int Level { get; set; }
        public string preBudgetCenterCode { get; set; }
        public string masterBudgetCenterCode { get; set; }
        public string budgettypeflag { get; set; }
        public IEnumerable<BudgetCenterCodeTree> Items { get; set; }

    }

    //public class BranchCodeTree
    //{
    //    public string BranchName { get; set; }
    //    public string BranchId { get; set; }

    //    public int Level { get; set; }
    //    public string preBranchCode { get; set; }
    //    public string masterBranchCenterCode { get; set; }
    //    public string branchtypeflag { get; set; }
    //    public IEnumerable<BranchCodeTree> Items { get; set; }

    //}
    public class LocationTree
    {
        public string LocationName { get; set; }
        public string LocationId { get; set; }
        public bool hasLocation { get; set; }
        public int Level { get; set; }
        public string preLocationCode { get; set; }
        public string groupSkuFlag { get; set; }
        public IEnumerable<LocationTree> Items { get; set; }
    }
    public class RegionTree
    {
        public string RegionName { get; set; }
        public string RegionId { get; set; }
        public bool hasRegion { get; set; }
        public int Level { get; set; }
        public string preRegionCode { get; set; }
        public string groupSkuFlag { get; set; }
        public IEnumerable<RegionTree> Items { get; set; }
    }

    public class MiscellaneousSubLedgerTree
    {
        public string MISC_EDESC { get; set; }
        public  string MISC_CODE { get; set; }
        public string masterMiscCode { get; set; }
        public string MiscName { get; set; }
        public string MiscId { get; set; }
        public bool hasMisc { get; set; }
        public int Level { get; set; }
        public string preMiscCode { get; set; }
        public string groupSkuFlag { get; set; }
        public IEnumerable<MiscellaneousSubLedgerTree> Items { get; set; }
    }

    public class AttributeTree
    {
        public string AttributeName { get; set; }
        public string AttributeId { get; set; }
        public bool hasAttribute { get; set; }
        public int Level { get; set; }
        public string preAttributeCode { get; set; }
        public string groupSkuFlag { get; set; }
        public IEnumerable<AttributeTree> Items { get; set; }
    }
    public class LocationModels
    {
        public string LOCATION_CODE { get; set; }
        public string LOCATION_EDESC { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string PRE_LOCATION_CODE { get; set; }
        public int? Childrens { get; set; }
        public int LEVEL { get; set; }
        public string ADDRESS { get; set; }
        public string TELEPHONE_MOBILE_NO { get; set; }
        public string EMAIL { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string CREATED_BY { get; set; }
    }
    public class ResourceTree
    {
        public string ResourceName { get; set; }
        public string ResourceId { get; set; }
        public bool hasResource { get; set; }
        public int Level { get; set; }
        public string preResourceCode { get; set; }
        public string groupSkuFlag { get; set; }
        public IEnumerable<ResourceTree> Items { get; set; }
    }
    public class ResourceModels
    {
        public int LEVEL { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string SYN_ROWID { get; set; }
        public DateTime? MODIFY_DATE { get; set; }
        public string MODIFY_BY { get; set; }
        public string RESOURCE_TYPE { get; set; }
        public string RESOURCE_CODE { get; set; }
        public string RESOURCE_EDESC { get; set; }
        public string RESOURCE_NDESC { get; set; }
        public string RESOURCE_FLAG { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string PRE_RESOURCE_CODE { get; set; }
        public string REMARKS { get; set; }
        public string COMPANY_CODE { get; set; }
        public string CREATED_BY { get; set; }
    }
    public class ProcessTree
    {
        public string ProcessName { get; set; }
        public string ProcessId { get; set; }
        public bool hasProcess { get; set; }
        public int Level { get; set; }
        public string preProcessCode { get; set; }
        public string groupSkuFlag { get; set; }
        public IEnumerable<ProcessTree> Items { get; set; }
    }
    public class LocationTypeModels
    {
        public string LOCATION_TYPE_CODE { get; set; }
        public string LOCATION_TYPE_EDESC { get; set; }
        public string LOCATION_TYPE_NDESC { get; set; }
        public string REMARKS { get; set; }
        public string COMPANY_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string SYN_ROWID { get; set; }
        public DateTime? MODIFY_DATE { get; set; }
        public string MODIFY_BY { get; set; }
    }
    public class ProcessModels
    {
        public string PROCESS_CODE { get; set; }
        public string PROCESS_EDESC { get; set; }
        public string PROCESS_FLAG { get; set; }
        public string PRE_PROCESS_CODE { get; set; }
        public int? Childrens { get; set; }
        public int LEVEL { get; set; }
        public string ADDRESS { get; set; }
        public string TELEPHONE_MOBILE_NO { get; set; }
        public string EMAIL { get; set; }
    }
    public class CompanyAndBranchModel
    {
        public string COMPANY_CODE { get; set; }
        public string COMPANY_EDESC { get; set; }
        public string ADDRESS { get; set; }
        public string TELEPHONE { get; set; }
        public string EMAIL { get; set; }
        public string FAX { get; set; }
        public string LOGO_FILE_NAME { get; set; }
        public string TPIN_VAT_NO { get; set; }
        public string BRANCH_CODE { get; set; }
        public string BRANCH_EDESC { get; set; }
        public string BRANCH_ADDRESS { get; set; }
        public string BRANCH_TELEPHONE_NO { get; set; }
        public string BRANCH_EMAIL { get; set; }
    }
    public class BranchModels
    {

        public string BRANCH_CODE { get; set; }
        public string BRANCH_EDESC { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string PRE_BRANCH_CODE { get; set; }
        public int? Childrens { get; set; }
        public int LEVEL { get; set; }
        public string COMPANY_CODE { get; set; }
        public string ADDRESS { get; set; }
        public string TELEPHONE_NO { get; set; }
        public string EMAIL { get; set; }
        public string MASTER_BRANCH_CODE { get; set; }
    }


    public class BranchTree
    {
        public string BRANCH_CODE { get; set; }
        public string BRANCH_EDESC { get; set; }
        public string PRE_BRANCH_CODE { get; set; }
        public string BranchName { get; set; }
        public string BranchId { get; set; }
        public bool hasbranch { get; set; }
        public int Level { get; set; }
        public string preBranchCode { get; set; }
        public string masterBranchCenterCode { get; set; }
        public string groupSkuFlag { get; set; }
        public IEnumerable<BranchTree> Items { get; set; }
    }





    public class EmployeeCodeModels
    {
        public string EMPLOYEE_CODE { get; set; }
        public string EMPLOYEE_EDESC { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string MASTER_EMPLOYEE_CODE { get; set; }
        public string PRE_EMPLOYEE_CODE { get; set; }
        public int? Childrens { get; set; }
        public int LEVEL { get; set; }
        public string EPERMANENT_ADDRESS1 { get; set; }
        public string MOBILE { get; set; }
    }

    public class ChartOfAccountForPDCModel
    {
        public string ACC_CODE { get; set; }
        public string ACC_EDESC { get; set; }
    }

    public class EmployeeCodeTree
    {
        public string employeeName { get; set; }
        public string employeeId { get; set; }
        public bool hasEmployees { get; set; }
        public int Level { get; set; }
        public string preEmployeeCode { get; set; }
        public string masterEmployeeCode { get; set; }
        public string groupSkuFlag { get; set; }
        public IEnumerable<EmployeeCodeTree> Items { get; set; }
    }
    public class DivisionModels
    {
        public string DIVISION_CODE { get; set; }
        public string DIVISION_EDESC { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string ADDRESS { get; set; }
        public string TELEPHONE_NO { get; set; }
        public string EMAIL { get; set; }
        public string COMPANY_CODE { get; set; }
        public string PRE_DIVISION_CODE { get; set; }
        public string MASTER_DIVISION_CODE { get; set; }
        public string REMARKS { get; set; }
        //public string MASTER_SUPPLIER_CODE { get; set; }

        public int? Childrens { get; set; }
        public int LEVEL { get; set; }

    }
    public class DivisionTree
    {
        public string divisionName { get; set; }
        public string divisionId { get; set; }
        public bool hasdivision { get; set; }
        public int Level { get; set; }
        public string predivisionCode { get; set; }
        public string masterDivisionCenterCode { get; set; }
        public string groupSkuFlag { get; set; }
        public IEnumerable<DivisionTree> Items { get; set; }
    }
    public class DealerModels
    {
        public string PARTY_TYPE_CODE { get; set; }
        public string PARTY_TYPE_EDESC { get; set; }

    }
    public class ModuleModels
    {
        public string MODULE_CODE { get; set; }
        public string MODULE_EDESC { get; set; }
    }
    public class FormControlModels
    {

        public string USER_NO { get; set; }
        public string FORM_CODE { get; set; }
        public string CREATE_FLAG { get; set; }
        public string READ_FLAG { get; set; }
        public string UPDATE_FLAG { get; set; }
        public string DELETE_FLAG { get; set; }
        public string POST_FLAG { get; set; }
        public string UNPOST_FLAG { get; set; }
        public string CHECK_FLAG { get; set; }
        public string VERIFY_FLAG { get; set; }
        public string MORE_FLAG { get; set; }
        public string COMPANY_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string UNCHECK_FLAG { get; set; }
        public string UNVERIFY_FLAG { get; set; }
        public string BRANCH_CODE { get; set; }
        public string SYN_ROWID { get; set; }
        public string MODIFY_DATE { get; set; }
        public string MODIFY_BY { get; set; }
        public string CHECK_VALUE { get; set; }
        public string VERIFY_VALUE { get; set; }
        public string POST_VALUE { get; set; }

    }

    public class WebPrefrence
    {
        public string Userid { get; set; }
        public string ShowAdvanceSearch { get; set; }
        public string ShowAdvanceAutoComplete { get; set; }
        public string ShowSYcButtonInView { get; set; } = "false";
        public string IrdUrl { get; set; } = "";
        public string Username { get; set; }
        public string Password { get; set; }
        public string SellerPan { get; set; }
    }

    public class KYCFORM
    {
        public string BirthDate { get; set; }
        public string Gender { get; set; }
        public string MaritalStatus { get; set; }
        public string Religion { get; set; }
        public string Bloadgroup { get; set; }
        public string telephoneNo { get; set; }
        public string MobileNo { get; set; }
        public string Companyname { get; set; }
        public string EmailOffice { get; set; }
        public string Address { get; set; }
        public string PermanentHouseNo { get; set; }
        public string PWARDNO { get; set; }
        public string PSTEETADDRESS { get; set; }
        public string PZONE { get; set; }
        public string PDIStrict { get; set; }
        public string PVDCMunicipality { get; set; }
        public string THouseNo { get; set; }
        public string TWARDNO { get; set; }
        public string TSTEETADDRESS { get; set; }
        public string TZONE { get; set; }
        public string TDIStrict { get; set; }
        public string TVDCMunicipality { get; set; }
        public string EmergencyName { get; set; }
        public string Emergencyrelationship { get; set; }
        public string Emergencyaddress { get; set; }
        public string Emergencyphoneno { get; set; }
        public string FamilyName { get; set; }
        public string FamilyMotherName { get; set; }
        public string FamilyspouseName { get; set; }
        public string weddingDate { get; set; }
        public string Childname { get; set; }
        public string Organizationtype { get; set; }
        public string organizationname { get; set; }
        public string Position { get; set; }
        public string FromDate { get; set; }
        public string KYCCustomerName { get; set; }
        public string CustomerId { get; set; }
        public string BirthDate_bs { get; set; }


    }
    public class SchemeImplementModel
    {
        public string SCHEME_CODE { get; set; }
        public string SCHEMEVOUCHERNO { get; set; }
        public decimal? QAUNTITY { get; set; }
        public decimal? SALES_SCHEME_VALUE { get; set; }
        public decimal? TOTAL_SALES { get; set; }
        public decimal? SALES_DISCOUNT { get; set; }
        public string STATUS { get; set; }
        public string FORM_CODE { get; set; }
        public string ACCOUNT_CODE { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string ITEM_CODE { get; set; }
        public string PARTY_TYPE_CODE { get; set; }
        public string AREA_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string CHARGE_CODE { get; set; }
        public string CHARGE_ACCOUNT_CODE { get; set; }
        public string CHARGE_RATE { get; set; }

        public decimal? CHARGE_RATE_PERCENTAGE { get; set; }
        public string QUERY_STRING { get; set; }

    }



    public class CustomerTree
    {
        public string customerName { get; set; }
        public string customerId { get; set; }
        public bool hasCustomers { get; set; }
        public int Level { get; set; }
        public string masterCustomerCode { get; set; }
        public string preCustomerCode { get; set; }
        public IEnumerable<CustomerTree> Items { get; set; }
    }
    public class CustomerSetupModels
    {
        public int LEVEL { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string MASTER_CUSTOMER_CODE { get; set; }
        public string PRE_CUSTOMER_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public int? Childrens { get; set; }
    }
    public class ImpactVoucherModel
    {
        public string SCHEME_IMPLEMENT_VALUE { get; set; }
        public string SUB_LEDGER_VALUE { get; set; }
    }
    public class InterestCalculationModel
    {
        public decimal RATE { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string GROUP_CODES { get; set; }
        public DateTime UPTO_DATE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
    }
    public class InterestCalculationResultModel
    {
        public string CUSTOMER_CODE { get; set; }

        public string GROUP_CODES { get; set; }

        
        public string CUSTOMER_EDESC { get; set; }
        public string REGD_OFFICE_EADDRESS { get; set; }
        public string TEL_MOBILE_NO1 { get; set; }
        public string TPIN_VAT_NO { get; set; }

        public string C_TPIN_VAT_NO { get; set; }

        public string B_ADDRESS { get; set; }

        public string B_TELEPHONE_NO { get; set; }

        public string B_EMAIL { get; set; }
        public string EMAIL { get; set; }

        public decimal? BALANCE { get; set; }

        public decimal? CREDIT_DAYS { get; set; }
        public decimal? INTEREST { get; set; }
        public decimal? TOTAL_INTEREST { get; set; }

        public decimal? TOTL_INT_PARENT { get; set; }
        public decimal? TOTAL_OUTSTANDING_BEF { get; set; }
        public decimal? TOTAL_OUTSTANDING_AF { get; set; }
        public decimal? RATE { get; set; }
        public DateTime UPTO_DATE { get; set; }

        public string TODAY_DATE { get; set; }
        public string TODAY_TIME { get; set; }
        public List<InterestCalcResDetailModel> DetailList = new List<InterestCalcResDetailModel>();
    }
    public class CompanyBranchInfo
    {
        public string C_TPIN_VAT_NO { get; set; }

        public string B_ADDRESS { get; set; }

        public string B_TELEPHONE_NO { get; set; }

        public string B_EMAIL { get; set; }
    }
 
    public class InterestCalcResDetailModel
    {
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }

        public string VOUCHER_NO { get; set; }

        public DateTime VOUCHER_DATE { get; set; }

        public decimal? CREDIT_DAYS { get; set; }

        public DateTime DUE_DATE { get; set; }
        public decimal? DUE_DAYS { get; set; }
        public decimal? BALANCE { get; set; }


        public decimal? INTEREST { get; set; }
        public decimal? TOTAL_INTEREST { get; set; }
        public decimal? TOTAL_OUTSTANDING_BEF { get; set; }
        public decimal? TOTAL_OUTSTANDING_AF { get; set; }
        public decimal? RATE { get; set; }
        public DateTime UPTO_DATE { get; set; }
        public string REGD_OFFICE_EADDRESS { get; set; }
        public string TEL_MOBILE_NO1 { get; set; }
        public string TPIN_VAT_NO { get; set; }
    }
    public class InterestCalculcImpacttModel
    {
        //public string CUSTOMER_CODE { get; set; }
        //public string CUSTOMER_EDESC { get; set; }
        //public decimal? BALANCE { get; set; }

        //public decimal? CREDIT_DAYS { get; set; }
        //public decimal? INTEREST { get; set; }

        public string FORM_CODE { get; set; }
        public string LEDGER_CODE { get; set; }
        public string ACCOUNT_CODE { get; set; }

        public DateTime? VOUCHER_DATE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string CHARGE_ACCOUNT_CODE { get; set; }
    }
    public class InterestCalcPostModel
    {
        public string INTERESET_DATA { get; set; }
        public string INTEREST_PARAM_DATA { get; set; }
    }

    public class InterestCalcLogModel
    {
        public int INTEREST_LOG_CODE { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public DateTime GENERATED_DATE { get; set; }

        public DateTime CREATED_DATE { get; set; }
        public string VOUCHER_NO { get; set; }

        public DateTime VOUCHER_DATE { get; set; }
        public decimal INTEREST_AMOUNT { get; set; }
    }
}





