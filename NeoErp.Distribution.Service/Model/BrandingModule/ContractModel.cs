using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NeoErp.Distribution.Service.Model.BrandingModule
{
   public class ContractModel
    {
        public ContractModel()
        {
            CUSTOMERS = new List<string>();
            AREA_CODE = new List<string>();
            RESELLER_CODE = new List<string>();
            Questions = new List<GeneralQstModel>();
            PRODUCT_ITEMS = new List<ItemUnitModel>();
            ITEM_CODES= new List<string>();
            BRAND_CODE = new List<string>();
        }
        public int MAXID { get; set; }
        public int? CONTRACT_CODE { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_EDESC { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public List<string> CUSTOMERS { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string CONTRACT_EDESC { get; set; }
        public string CONTRACT_NDESC { get; set; }
        public List<string> BRAND_CODE { get; set; }
        public string BRAND_CODE_STRING { get; set; }
        public int? SPROVIDER_CODE { get; set; }
        public string BRANDING_TYPE { get; set; }
        public DateTime START_DATE { get; set; }
        public DateTime END_DATE { get; set; }
        public List<string> AREA_CODE { get; set; }
        public string ITEM_CODE_STRING { get; set; }
        public string CONTRACT_TYPE { get; set; }
        public string AMOUNT_TYPE { get; set; }
        public int? AMOUNT { get; set; }
        public DateTime PAYMENT_DATE { get; set; }
        public int? ADVANCE_AMOUNT { get; set; }
        public string CONTRACTOR_NAME { get; set; }
        public string CONTRACTOR_ADDRESS { get; set; }
        public string CONTRACTOR_EMAIL { get; set; }
        public string CONTRACTOR_PHONE { get; set; }
        public string CONTRACTOR_MOBILE { get; set; }
        public string CONTRACTOR_DESIGNATION { get; set; }
        public string CONTRACTOR_PAN_NO { get; set; }
        public string CONTRACTOR_VAT_NO { get; set; }
        public string OWNER_NAME { get; set; }
        public string OWNER_ADDRESS { get; set; }
        public string OWNER_EMAIL { get; set; }
        public string OWNER_PHONE { get; set; }
        public string OWNER_MOBILE { get; set; }
        public string OWNER_COMPANY_NAME { get; set; }
        public string OWNER_PAN_NO { get; set; }
        public string OWNER_VAT_NO { get; set; }
        public string JOB_ORDER_NO { get; set; }
        public string DESCRIPTION { get; set; }
        public string CONTRACT_FILES { get; set; }
        public string REMARKS { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string LAST_MODIFIED_BY { get; set; }
        public DateTime? LAST_MODIFIED_DATE { get; set; }
        public Char APPROVED_FLAG { get; set; }
        public string APPROVED_BY { get; set; }
        public DateTime? APPROVED_DATE { get; set; }
        public Char DELETED_FLAG { get; set; }
        public string ITEM_CODE { get; set; }
        public List<string> ITEM_CODES { get; set; }
        public string ITEM_EDESC { get; set; }
        public List<string> RESELLER_CODE { get; set; }
        public string RESELLER_CODE_STRING { get; set; }
        public string STATUS { get; set; }
        public int? SET_CODE { get; set; }
        public List<GeneralQstModel> Questions { get; set; }
        public List<ItemUnitModel> PRODUCT_ITEMS { get; set; }
        public string PRODUCT_QUANTITY { get; set; }
        public string MU_CODE { get; set; }
        public string IS_ROUTE_PLAN { get; set; }
        public string HAS_GIFT_NAME { get; set; }

    }
    public class SupplierModel
    {
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_EDESC { get; set; }
    }
    public class CustomerModel
    {
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
    }
    public class AreaModel
    {
        public int? CONTRACT_CODE { get; set; }
        public string AREA_CODE { get; set; }
        public string AREA_NAME { get; set; }
    }
    public class ResellerModel
    {
        public int? CONTRACT_CODE { get; set; }
        public string RESELLER_CODE { get; set; }
        public string RESELLER_NAME { get; set; }
        public string AREA_CODE { get; set; }
    }

    public class BrdItemModel
    {
        public int? CONTRACT_CODE { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
    }
    public class AttachedFile
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public HttpPostedFileBase Attachment { get; set; }
    }

    public class SetQstModel
    {
        public int? SET_CODE { get; set; }
        public string TITLE { get; set; }
    }

    public class BrandTypeModel
    {
        public string BRAND_TYPE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
    }

    public class ItemUnitModel
    {
        public int? CONTRACT_CODE { get; set; }
        public string ITEM_CODE { get; set; }
        public int QUANTITY { get; set; }
        public string MU_CODE { get; set; }
        public string MU_EDESC { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
    }

    public class ContactImportModel
    {
        public string CONTRACT_EDESC { get; set; }
        public string SUPPLIER { get; set; }
        public string BRAND { get; set; }
        public string BRANDING_TYPE { get; set; }
        public DateTime START_DATE { get; set; }
        public DateTime END_DATE { get; set; }
        public string CONTRACT_TYPE { get; set; }
        public string AMOUNT_TYPE { get; set; }
        public int? AMOUNT { get; set; }
        public DateTime? NEXT_PAYMENT_DATE { get; set; }
        public DateTime PAYMENT_DATE { get; set; }
        public int? ADVANCE_AMOUNT { get; set; }
        public string CONTRACTOR_NAME { get; set; }
        public string CONTRACTOR_ADDRESS { get; set; }
        public string CONTRACTOR_EMAIL { get; set; }
        public string CONTRACTOR_PHONE { get; set; }
        public string CONTRACTOR_MOBILE { get; set; }
        public string CONTRACTOR_DESIGNATION { get; set; }
        public string CONTRACTOR_PAN_NO { get; set; }
        public string CONTRACTOR_VAT_NO { get; set; }
        public string OWNER_NAME { get; set; }
        public string OWNER_ADDRESS { get; set; }
        public string OWNER_EMAIL { get; set; }
        public string OWNER_PHONE { get; set; }
        public string OWNER_MOBILE { get; set; }
        public string OWNER_COMPANY_NAME { get; set; }
        public string OWNER_PAN_NO { get; set; }
        public string OWNER_VAT_NO { get; set; }
        public string JOB_ORDER_NO { get; set; }
        public string DESCRIPTION { get; set; }
        public string CONTRACT_FILES { get; set; }
        public string REMARKS { get; set; }
        public string APPROVED_FLAG { get; set; }
        public string APPROVED_BY { get; set; }
        public DateTime? APPROVED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string QUESTION_SET { get; set; }
        public string AREAS { get; set; }
        public string CUSTOMERS { get; set; }
        public string RESELLERS { get; set; }
        public string ITEM { get; set; }
        public string ITEM_CODES { get; set; }
    }
    public class WebQueAnsModel
    {
        public WebQueAnsCommonModel common { get; set; }
        public string webqakey { get; set; }
        public string webqavalue { get; set; }
    }
    public class WebQueAnsCommonModel
    {
        public string customer_code { get; set; }
        public string customer_name { get; set; }
    }
}