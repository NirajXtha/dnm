using NeoErp.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NeoERP.DocumentTemplate.Service.Models
{
    public class FormDetailSetup
    {
        public int SERIAL_NO { get; set; }
        public string TABLE_NAME { get; set; }
        public string COLUMN_NAME { get; set; }
        public int COLUMN_WIDTH { get; set; }
        public string COLUMN_HEADER { get; set; }
        public int TOP_POSITION { get; set; }

        public int LEFT_POSITION { get; set; }

        public string DISPLAY_FLAG { get; set; }

        public string DEFA_VALUE { get; set; }

        public string IS_DESC_FLAG { get; set; }

        public string MASTER_CHILD_FLAG { get; set; }

        public string FORM_CODE { get; set; }

        public string FORM_EDESC { get; set; }

        public string FORM_TYPE { get; set; }

        public string COMPANY_CODE { get; set; }

        public string COMPANY_EDESC { get; set; }

        public string TELEPHONE { get; set; }

        public string EMAIL { get; set; }

        public string TPIN_VAT_NO { get; set; }

        public string ADDRESS { get; set; }

        public string CREATED_BY { get; set; }

        public DateTime? CREATED_DATE { get; set; }

        public string DELETED_FLAG { get; set; }

        public string PARTY_TYPE_CODE { get; set; }

        public string FILTER_VALUE { get; set; }

        public string SYN_ROWID { get; set; }

        public DateTime? MODIFY_DATE { get; set; }
        public string MODIFY_BY { get; set; }
        public string CHILD_ELEMENT_WIDTH { get; set; }
        public string REFERENCE_FLAG { get; set; }
        public string REF_TABLE_NAME { get; set; }
        public string NEGATIVE_STOCK_FLAG { get; set; }
        public string FREEZE_MASTER_REF_FLAG { get; set; }
        public string REF_FIX_QUANTITY { get; set; }
        public string REF_FIX_PRICE { get; set; }
        public string HELP_DESCRIPTION { get; set; }
        public string CHARGE_TYPE_FLAG { get; set; }
        public string VALUE_PERCENT_FLAG { get; set; }
        public decimal VALUE_PERCENT_AMOUNT { get; set; }
        public char? ON_ITEM { get; set; }
        public string MANUAL_CALC_CHARGE { get; set; }
        public string DISPLAY_RATE { get; set; }
        public string RATE_SCHEDULE_FIX_PRICE { get; set; }
        public string PRICE_CONTROL_FLAG { get; set; }

        public string NEXT_SCRN_FORM_CODE { get; set; }

    }
    public class DraftFormModel
    {
        public int? SERIAL_NO { get; set; }
        public string FORM_CODE { get; set; }
        public string TABLE_NAME { get; set; }
        public string COLUMN_NAME { get; set; }
        public string COLUMN_VALUE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string TEMPLATE_NO { get; set; }
        public string MODIFY_BY { get; set; }
        public DateTime? MODIFY_DATE { get; set; }

    }
    public class TemplateDraftModel
    {
        public string FORM_CODE { get; set; }
        public string TEMPLATE_CODE { get; set; }
        public string TEMPLATE_EDESC { get; set; }
        public string TEMPLATE_NDESC { get; set; }
    }
    public class TemplateDraftListModel
    {
        public string FORM_CODE { get; set; }
        public string TEMPLATE_CODE { get; set; }
        public string TEMPLATE_EDESC { get; set; }
        public string TEMPLATE_NDESC { get; set; }
        public string FORM_EDESC { get; set; }
        public string MODULE_CODE { get; set; }
        public string FORM_TYPE { get; set; }
    }
    public class Form_Detail_Column
    {
        public string COLUMN_NAME { get; set; }
    }



    public class COMMON_COLUMN
    {
        public decimal? SECOND_QUANTITY { get; set; }
        public decimal? THIRD_QUANTITY { get; set; }
        public decimal? TOTAL_PRICE { get; set; }
        public decimal ED { get; set; }
        public decimal BC { get; set; }
        public decimal SD { get; set; }
        public decimal VT { get; set; }
        public decimal TA { get; set; }
        public decimal NA { get; set; }
        public decimal? MASTER_AMOUNT { get; set; }
        public DateTime? MRR_DATE { get; set; }
        public decimal? MR_NO { get; set; }
        public DateTime? CHALAN_DATE { get; set; }
        public DateTime? DUE_DATE { get; set; }
        public string ACC_CODE { get; set; }
        public string IS_PARENT_ROW { get; set; }
        public string FORM_CODE { get; set; }
        public string BUDGET_FLAG { get; set; }
        public string REMARKS { get; set; }
        public string REASON { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string REGD_OFFICE_EADDRESS { get; set; }
        public string TPIN_VAT_NO { get; set; }
        public string TEL_MOBILE_NO1 { get; set; }
        public string CUSTOMER_NDESC { get; set; }
        public string EMPLOYEE_CODE { get; set; }
        public string EMPLOYEE_EDESC { get; set; }
        public string REQUISITION_NO { get; set; }
        public string MRR_NO { get; set; }
        public string CHALAN_NO { get; set; }
        public decimal? CALC_UNIT_PRICE { get; set; }
        public decimal? CALC_TOTAL_PRICE { get; set; }
        public decimal? EXCHANGE_RATE { get; set; }
        public string PRIORITY_CODE { get; set; }
        public string TNC_CODE { get; set; }
        public string SHIPPING_ADDRESS { get; set; }
        public string SHIPPING_CONTACT_NO { get; set; }
        public string SALES_TYPE_CODE { get; set; }
        public string AGENT_CODE { get; set; }
        public string AGENT_EDESC { get; set; }
        public int SERIAL_NO { get; set; }
        public string DIVISION_CODE { get; set; }
        public string DIVISION_EDESC { get; set; }
        public string SUPPLIER_MRR_NO { get; set; }
        public string REFERENCE_NO { get; set; }

        public string REFERENCE_FORM_CODE { get; set; }
        public string REFERENCE_PARTY_CODE { get; set; }

        public string DOCUMENT_TYPE_CODE { get; set; }
        public string SUPPLIER_INV_NO { get; set; }
        public DateTime? SUPPLIER_INV_DATE { get; set; }
        public string VOUCHER_NO { get; set; }
        public DateTime? VOUCHER_DATE { get; set; }
        public string TRANSACTION_TYPE { get; set; }
        public decimal? CALC_QUANTITY { get; set; }
        public string ISSUE_NO { get; set; }
        public string INVOICE_NO { get; set; }
        public string SALES_NO { get; set; }
        public string ENTRY_NO { get; set; }
        public DateTime? ENTRY_DATE { get; set; }
        public string BILL_NO { get; set; }
        public string QC_ACTION { get; set; }
        public DateTime? RECEIVED_DATE { get; set; }
        public string PRODUCT_CATEGORY { get; set; }
        public DateTime? SAMPLE_DATE { get; set; }
        public DateTime? BILL_DATE { get; set; }
        public string VEHICLE_NO { get; set; }
        public string REF_NO { get; set; }
        public string DELIVERY_TERMS { get; set; }
        public DateTime? ORDER_DATE { get; set; }
        public DateTime? QUOTE_DATE { get; set; }
        public string MAIN_ITEM_CODE { get; set; }
        public string ITEM_CODE { get; set; }
        public string HS_CODE { get; set; }
        public string REFERENCE { get; set; }
        public DateTime? ACTIVITY_DATE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string NON_VAT_FLAG { get; set; }
        public string BUDGET_CODE { get; set; }
        public string SPECIFICATION { get; set; }
        public string BRAND_NAME { get; set; }

        public decimal? CREDIT_DAYS { get; set; }
        public string REQUEST_NO { get; set; }
        public string QUOTE_NO { get; set; }

        public decimal? UNIT_PRICE { get; set; }
        public decimal? RANK_VALUE { get; set; }
        public string MASTER_ACC_CODE { get; set; }
        public string ISSUE_TYPE_CODE { get; set; }
        public DateTime? INVOICE_DATE { get; set; }
        public string GATE_NO { get; set; }
        public string BLANKET_NO { get; set; }
        public DateTime? BLANKET_DATE { get; set; }
        public DateTime? SALES_DATE { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_EDESC { get; set; }
        public string ISSUE_SLIP_NO { get; set; }
        public string ACTUAL_QUANTITY { get; set; }
        public string TO_BRANCH_CODE { get; set; }
        public string P_TYPE { get; set; }
        public DateTime DELIVERY_DATE { get; set; }
        public string ORDER_NO { get; set; }
        public DateTime? REQUISITION_DATE { get; set; }
        public string FROM_LOCATION_CODE { get; set; }
        public string FROM_LOCATION_EDESC { get; set; }
        public string FROM_BUDGET_FLAG { get; set; }
        public decimal? COMPLETED_QUANTITY { get; set; }
        public decimal? REQ_QUANTITY { get; set; }
        public string CURRENCY_CODE { get; set; }
        public DateTime? RETURN_DATE { get; set; }
        public DateTime REQUEST_DATE { get; set; }

        public string BUYERS_NAME { get; set; }
        public string BUYERS_ADDRESS { get; set; }
        public string ADDRESS { get; set; }
        public string CONTACT_PERSON { get; set; }
        public string PHONE_NO { get; set; }
        public string MANUAL_NO { get; set; }
        public decimal? AMOUNT { get; set; }
        public decimal? QUANTITY { get; set; }
        public string LOT_NO { get; set; }
        public int? TERMS_DAY { get; set; }
        public string TO_BUDGET_FLAG { get; set; }
        public string PARTICULARS { get; set; }
        public string MU_CODE { get; set; }
        public string STOCK_BLOCK_FLAG { get; set; }
        public string MASTER_TRANSACTION_TYPE { get; set; }
        public string TO_LOCATION_CODE { get; set; }
        public string TO_LOCATION_EDESC { get; set; }
        public DateTime? ISSUE_DATE { get; set; }
        public string RETURN_NO { get; set; }
        public string PRODUCT_CODE { get; set; }
        public decimal? AREA_CODE { get; set; }
        public string PARTY_TYPE_CODE { get; set; }
        public string PAYMENT_MODE { get; set; }
        public string CHEQUE_NO { get; set; }
        public string MEMBER_SHIP_CARD { get; set; }
        public string BATCH_NO { get; set; }
        public decimal? LINE_ITEM_DISCOUNT { get; set; }
        public string PARTY_TYPE_EDESC { get; set; }

        public string NepaliVOUCHER_DATE { get; set; }
        public decimal? PRODUCTION_QTY { get; set; }

        public decimal? PURITY { get; set; }
        public decimal? LESS_STONE { get; set; }
        public decimal? GROSS_WEIGHT { get; set; }
        public decimal? WASTAGE { get; set; }
        public decimal? NET_WEIGHT { get; set; }
        public decimal? TOTAL_WEIGHT { get; set; }
        public decimal? MAKING { get; set; }
        public decimal? STONE_WEIGHT { get; set; }
        public decimal? STONE_AMOUNT { get; set; }
        public decimal? DIAMOND_CARAT { get; set; }
        public decimal? DIAMOND_AMOUNT { get; set; }
        public decimal? PRICE { get; set; }
        public decimal? EXCISE_AMOUNT { get; set; }
        public decimal? DISCOUNT_CLASS { get; set; }
        public decimal? DISCOUNT_AMOUNT { get; set; }
        public decimal? EXCISE_DUTY_AMOUNT { get; set; }
        public decimal? LUXURY_TAX_AMOUNT { get; set; }
        public decimal? VAT_AMOUNT { get; set; }

        public List<CUSTOM_TRANSACTION> CUSTOM_TRANSACTION_ENTITY { get; set; }
        public List<DocumentTransaction> IMAGES_LIST { get; set; }
        public List<ChargeOnSales> CHARGE_LIST { get; set; }
        public decimal? GATE_ENTRY_NO { get; set; }

        // public string DESPATCH THROUGH { get; set; }
        // public string TERMES OF DELIVERY { get; set; }
        // public string TRUCK { get; set; }
        // public string MOBILE NO { get; set; }
        //public string DRIVER MOBILE { get; set; }
        // public string DESTINATION { get; set; }
        //public string DRIVER NAME { get; set; }
        // public string LICENANCE NO. { get; set; }
        // public string TRUCK NO. { get; set; }
        // public string TERMS OF DELIVERY { get; set; }
        //public string LICENSE NO. { get; set; }
        // public string DRIVER MOBILE { get; set; }
        public string PP_NO { get; set; }
        public string PARTY_CODE { get; set; }
        public int SUB_PROJECT_CODE { get; set; }
        public string SUB_PROJECT_NAME { get; set; }
        public DateTime? EST_DELIVERY_DATE { get; set; }
        public DateTime? EFFECTIVE_DATE { get; set; }
        //public DateTime? ARRIVAL_DATE { get; set; }
        //public DateTime? DEPARTURE_DATE { get; set; }
        public int? PLAN_NO { get; set; }
        public string PLAN_NAME { get; set; }
        public decimal? M_RATE { get; set; }

    }
    public class GuestInfoFromMaterTransaction
    {
        public string Form_CODE { get; set; }
        public string VOUCHER_NO { get; set; }
        public DateTime? VOUCHER_DATE { get; set; }
        public decimal? VOUCHER_AMOUNT { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string CHECKED_BY { get; set; }
        public DateTime? CHECKED_DATE { get; set; }
        public string AUTHORISED_BY { get; set; }
        public DateTime? POSTED_DATE { get; set; }
        public DateTime? MODIFY_DATE { get; set; }
        public string SYN_ROWID { get; set; }
        public String REFERENCE_NO { get; set; }
        public string SESSION_ROWID { get; set; }
        public DateTime? CR_LMT1 { get; set; }
        public DateTime? CR_LMT2 { get; set; }
        public string CR_LMT3 { get; set; }
        //public string ROOMNUMBER { get; set; }
        public string MANUAL_NO { get; set; }
        //public DateTime? ARRIVAL_DATE { get; set; }
        //public DateTime? DEPARTURE_DATE { get; set; }
    }

    public class CompanyInfo
    {
        public string COMPANY_EDESC { get; set; }
        public string ADDRESS { get; set; }
        public string TELEPHONE { get; set; }
        public string EMAIL { get; set; }
        public string TPIN_VAT_NO { get; set; }
        public string ABBR_CODE { get; set; }
        public string FOOTER_LOGO_FILE_NAME { get; set; }
        public string COMPANY_CODE { get; set; }
    }

    public class IncomingMaterial
    {
        public string master_item_code { get; set; }
        public string TRANSACTION_NO { get; set; }
        public string CATEGORY_CODE { get; set; }
        public string item_edesc { get; set; }
        public string ITEM_APPLY_ON { get; set; }
        public string BRAND_NAME { get; set; }
        public string PART_NUMBER { get; set; }
        public string ITEM_SPECIFICATION { get; set; }
        public string INTERFACE { get; set; }
        public string COLOR { get; set; }
        public string LAMINATION { get; set; }
        public string GRADE { get; set; }
        public string TYPE { get; set; }
        public string ITEM_SIZE { get; set; }
        public decimal? SIZE_LENGHT { get; set; }
        public decimal? SIZE_WIDTH { get; set; }
        public decimal? REEM_WEIGHT_KG { get; set; }
        public string REMARKS { get; set; }
        public string pre_item_code { get; set; }
        public string item_code { get; set; }
        public string Voucher_No { get; set; }
        public string invoice_no { get; set; }
        public string QC_NO { get; set; }
        public string MANUAL_NO { get; set; }
        public string supplier_edesc { get; set; }
        public decimal quantity { get; set; }
        public string to_location_code { get; set; }
        //public decimal? thickness { get; set; }
        public string thickness { get; set; }
        public string RollDiameter { get; set; }
        public string PH { get; set; }
        public string UNPLEASANT_SMELL_ODOUR { get; set; }
        public string Dust_Dirt { get; set; }
        public string Damaging_Material { get; set; }
        public string Core_Damaging { get; set; }
        public string GSM { get; set; }
        public string Tensile_CD { get; set; }
        public string Tensile_MD { get; set; }
        public string Visual_Inspection { get; set; }
        public Nullable<System.DateTime> RECEIPT_DATE { get; set; }
        public Nullable<System.DateTime> QC_DATE { get; set; }
        public string MITI { get; set; }
        public string GATE_NO { get; set; }
        public Nullable<System.DateTime> GATE_DATE { get; set; }
        public string GRN_NO { get; set; }
        public Nullable<System.DateTime> GRN_DATE { get; set; }
        public string CreatedBy { get; set; }


        public string Thickness1 { get; set; }
        public string Thickness2 { get; set; }
        public string Thickness3 { get; set; }
        public string RollDiameter1 { get; set; }
        public string RollDiameter2 { get; set; }
        public string RollDiameter3 { get; set; }
        public string PH1 { get; set; }
        public string PH2 { get; set; }
        public string PH3 { get; set; }
        public string UnpleasantSmell1 { get; set; }
        public string UnpleasantSmell2 { get; set; }
        public string UnpleasantSmell3 { get; set; }
        public string DustDirt1 { get; set; }
        public string DustDirt2 { get; set; }
        public string DustDirt3 { get; set; }
        public string DamagingMaterial1 { get; set; }
        public string DamagingMaterial2 { get; set; }
        public string DamagingMaterial3 { get; set; }
        public string CoreDamaging1 { get; set; }
        public string CoreDamaging2 { get; set; }
        public string CoreDamaging3 { get; set; }
        public string Width1 { get; set; }
        public string Width2 { get; set; }
        public string Width3 { get; set; }
        public string GSM1 { get; set; }
        public string GSM2 { get; set; }
        public string GSM3 { get; set; }
        public string TensileCD1 { get; set; }
        public string TensileCD2 { get; set; }
        public string TensileCD3 { get; set; }
        public string TensileMD1 { get; set; }
        public string TensileMD2 { get; set; }
        public string TensileMD3 { get; set; }
        public string VisualInspection1 { get; set; }
        public string VisualInspection2 { get; set; }
        public string VisualInspection3 { get; set; }
        public string Remarks1 { get; set; }
        public string Remarks2 { get; set; }
        public string Remarks3 { get; set; }
        public string Remarks4 { get; set; }
        public string Remarks5 { get; set; }
        public string Remarks6 { get; set; }
        public string Remarks7 { get; set; }
        public string Remarks8 { get; set; }
        public string Remarks9 { get; set; }
        public string Remarks10 { get; set; }
        public string Remarks11 { get; set; }
        public string Remarks12 { get; set; }
    }

    public class GlobalAgroProductsReport
    {
        public string TRANSACTION_NO { get; set; }
        public string REFERENCE_NO { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public int SERIAL_NO { get; set; }
        public string FORM_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string PARTY_NAME { get; set; }
        public string ADDRESS { get; set; }
        public string PHYSICAL_TEST_RAWMATERIAL { get; set; }
        public string WEIGHT { get; set; }
        public string MOISTURE { get; set; }
        public string TEMPERATURE { get; set; }
        public string WET { get; set; }
        public string FUNGUS { get; set; }
        public string DUST { get; set; }
        public string GRADING { get; set; }
        public string SMELL { get; set; }
        public string COLOR { get; set; }
        public string PIECES { get; set; }
        public string IMMATURITY_OF_GRAINS { get; set; }
        public string OTHER_ITEMS { get; set; }
        public string ROTTEN_HOLED { get; set; }
        public string DAMAGED { get; set; }
        public string BROKEN { get; set; }
        public string HUSK { get; set; }
        public string OVERTOASTED { get; set; }
        public string USEABLE { get; set; }
        public string UNUSEABLE { get; set; }
        public string FAT { get; set; }
        public string QUALITY_OF_GOODS { get; set; }
        public string EXCELLENT { get; set; }
        public string GREAT { get; set; }
        public string GOODS_NORMAL { get; set; }
        public string WAREHOUSE { get; set; }
        public string SILO { get; set; }
        public string GHAN { get; set; }
        public string PROTEIN { get; set; }
        public string QUALITY_OF_FIREWOOD { get; set; }
        public string PRODUCT_SIZE { get; set; }
        public string PRODUCT_TYPE { get; set; }
        public string DEDUCT_IN_BAG { get; set; }
        public string NET_WEIGHT { get; set; }
        public string REMARKS { get; set; }
        public string UNLOAD_UNIT { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string MODIFY_DATE { get; set; }
        public string MODIFY_BY { get; set; }
        public string CHECKED_BY { get; set; }
        public string AUTHORISED_BY { get; set; }
        public char ISPLASTIC_BAG { get; set; }
        public char ISJUTE_BAG { get; set; }
        public char ISPLASTIC_WEIGHT { get; set; }
        public char ISJUTE_WEIGHT { get; set; }
        public string VEHICLE_NO { get; set; }
        public string BILL_NO { get; set; }
        public string GATE_OR_GRN_NO { get; set; }
        public List<WEIGHTDETAILS> WEIGHTDETAILSList { get; set; }
        public List<UNLOADEDCHHALLI> UNLOADEDCHHALLIList { get; set; }
        public List<DAKHILADETAILS> DAKHILADETAILSList { get; set; }
    }
    public class WEIGHTDETAILS
    {
        public string FIRST_WEIGHT { get; set; }
        public string SECOND_WEIGHT { get; set; }
        public string NET_WEIGHT { get; set; }
        public string CHALLAN_WEIGHT { get; set; }
        public string WEIGHT_DIFFERENCE { get; set; }
        public string REMARKS { get; set; }
    }
    public class UNLOADEDCHHALLI
    {
        public string FIRST_CHHALLI { get; set; }
        public string SECOND_CHHALLI { get; set; }
        public string THIRD_CHHALLI { get; set; }
        public string FOURTH_CHHALLI { get; set; }
        public string FIFTH_CHHALLI { get; set; }
        public string SIXTH_CHHALLI { get; set; }
        public string SEVENTH_CHHALLI { get; set; }
        public string EIGHTH_CHHALLI { get; set; }
        public string NINETH_CHHALLI { get; set; }
        public string TENTH_CHHALLI { get; set; }
        public string ELEVEN_CHHALLI { get; set; }
        public string TWELVE_CHHALLI { get; set; }
        public string TOTAL { get; set; }
        public string REMARKS { get; set; }
    }
    public class DAKHILADETAILS
    {
        public string ENTRY_NO { get; set; }
        public string BILL_NO { get; set; }
        public string CHALAN_NO { get; set; }
        public string ITEM { get; set; }
        public string TOTAL_BAG { get; set; }
        public string WEIGHT { get; set; }
        public string REMARKS { get; set; }
    }

    public class IRDLogModel
    {
        public string VOUCHER_NO { get; set; }
        public string MESSAGE { get; set; }
        public string FORM_CODE { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string REQUEST_JSON { get; set; }
        public string RESPONSE_JSON { get; set; }
        public double? AMOUNT { get; set; }
        public double? TAX_AMOUNT { get; set; }
        public double? VAT { get; set; }
        public string REQUEST_BY { get; set; }

    }

    public class IRDLogResponseModel
    {
        public List<IRDLogModel> data { get; set; }
        public int total { get; set; }
    }

}
