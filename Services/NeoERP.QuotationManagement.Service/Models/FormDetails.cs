using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QuotationManagement.Service.Models
{
    public class FormDetails
    {
        public List<REF_MODEL_DEFAULT> REF_MODEL { get; set; }
        public FORM_TEMPLATE_DRAFT FORM_TEMPLATE { get; set; }
        public string Save_Flag { get; set; }
        public bool FROM_REF { get; set; }
        public string Table_Name { get; set; }
        public int ID { get; set; }
        public string Form_Code { get; set; }
        public string Order_No { get; set; }
        public string Grand_Total { get; set; }
        public string Master_COLUMN_VALUE { get; set; }
        public string Child_COLUMN_VALUE { get; set; }
        public string Child_COLUMNS { get; set; }
        public string Custom_COLUMN_VALUE { get; set; }
        public string BUDGET_TRANS_VALUE { get; set; }
        public string TDS_VALUE { get; set; }
        public string VAT_VALUE { get; set; }
        public string PAYMENT_MODE { get; set; }
        public string SUB_LEDGER_VALUE { get; set; }
        public string DR_TOTAL_VALUE { get; set; }
        public string CR_TOTAL_VALUE { get; set; }
        public string PRIMARY_COL_NAME { get; set; }
        public string CHARGES { get; set; }
        public string ORDER_CODE { get; set; }
        public string ORDER_EDESC { get; set; }
        public string Type { get; set; }
        public string TempCode { get; set; }
        public string INV_ITEM_CHARGE_VALUE { get; set; }
        public string SHIPPING_DETAILS_VALUE { get; set; }
        public string charge_tran_value { get; set; }
        public string SERIAL_TRACKING_VALUE { get; set; }
        public string MODULE_CODE { get; set; }
        public string BATCH_TRACKING_VALUE { get; set; }
        public int SUB_PROJECT_CODE { get; set; }
    }

    public class REF_MODEL_DEFAULT
    {
        public string VOUCHER_NO { get; set; }
        public string ITEM_CODE { get; set; }
        public string SERIAL_NO { get; set; }
        public string REF_FORM_CODE { get; set; }
        public string TABLE_NAME { get; set; }
        public decimal Qty { get; set; }
        public decimal? calc_qty { get; set; }
        public decimal cal_unit_price { get; set; }
        public decimal cal_total_price { get; set; }
        public decimal Total_price { get; set; }
        public int SUB_PROJECT_CODE { get; set; } = 0;

    }
    public class FORM_TEMPLATE_DRAFT
    {
        public string TEMPLATE_NO { get; set; }
        public string TEMPLATE_EDESC { get; set; }
        public string TEMPLATE_NDESC { get; set; }
        public string TEMPLATE_ASSIGNEE { get; set; }
        public DateTime? ASSIGNED_DATE { get; set; }
    }

    public class FormDetailRefrence
    {
        //public FormDetailRefrence()
        //{
        //    FormSetupRefrence = new List<FORM_SETUP_REFERENCE>();
        //    FormControlModels = new List<Models.FormControlModels>();
        //}

        //public List<FORM_SETUP_REFERENCE> FormSetupRefrence { get; set; }
        //public List<FormControlModels> FormControlModels { get; set; }
    }

    public class FORM_SETUP_REFERENCE
    {
        public string REFERENCE_FLAG { get; set; }
        public string REF_TABLE_NAME { get; set; }
        public string REF_FORM_CODE { get; set; }
        public string FREEZE_MASTER_REF_FLAG { get; set; }
        public string REF_FIX_QUANTITY { get; set; }
        public string REF_FIX_PRICE { get; set; }
        public decimal FREEZE_BACK_DAYS { get; set; }
        public decimal DECIMAL_PLACE { get; set; }
        public string NEGATIVE_STOCK_FLAG { get; set; }
        public string FREEZE_MANUAL_ENTRY_FLAG { get; set; }
        public string DISCOUNT_SCHEDULE_FLAG { get; set; }
        public string PRICE_CONTROL_FLAG { get; set; }
        public string RATE_DIFF_FLAG { get; set; }
        public string SIM_FLAG { get; set; }
        public String RT_CONTROL_FLAG { get; set; } = "N";
        public string Dealer_system_flag { get; set; } = "N";

        public string SERIAL_TRACKING_FLAG { get; set; }
        public string BATCH_TRACKING_FLAG { get; set; }
    }
    public class DOCUMENT_REFERENCE
    {
        public DOCUMENT_REFERENCE()
        {
            this.referenceModel = new REFERENCE_MODEL();
        }
        public REFERENCE_MODEL referenceModel { get; set; }
    }
    public class DOCUMENT_SCHEME
    {
        public DOCUMENT_SCHEME()
        {
            this.SchemeModel = new SCHEME_MODEL();
        }
        public SCHEME_MODEL SchemeModel { get; set; }
    }
    public class QUERY_COLUMN_MODEL
    {
        public string ORAND { get; set; }
        public string COLUMN_NAME { get; set; }
        public string COLUMN_VALUE { get; set; }
    }
    public class REFERENCE_MODEL
    {
        public string FORM_CODE { get; set; }
        public string TABLE_NAME { get; set; }
        public string DOCUMENT { get; set; }
        public string TEMPLATE { get; set; }
        public string FROM_DATE { get; set; }
        public string TO_DATE { get; set; }
        public string NAME { get; set; }
        public string ITEM_DESC { get; set; }
        public string VOUCHER_NO { get; set; }
        public string ROW { get; set; }
        public string REFERENCE_QUALITY { get; set; }
        public List<QUERY_COLUMN_MODEL> COLUMNS_FILTER { get; set; }
    }
    public class SCHEME_MODEL
    {
        public string SCHEME_CODE { get; set; }
        public string SCHEME_EDESC { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string SCHEME_TYPE { get; set; }
        public string TYPE { get; set; }

        public string ITEM_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string PARTY_TYPE_CODE { get; set; }
        public string AREA_CODE { get; set; }
        public string EFFECTIVE_FROM { get; set; }
        public string EFFECTIVE_TO { get; set; }
    }
    public class REFERENCE_DETAIL_MODEL
    {
        public decimal? REFERENCE_QUANTITY { get; set; }
        public decimal? REFERENCE_UNIT_PRICE { get; set; }
        public decimal? REFERENCE_TOTAL_PRICE { get; set; }
    }
    public class VALIDATION_FLAG_MODEL
    {
        public String BACK_DATE_VNO_SAVE_FLAG { get; set; }
        public String ACCESS_BDFSM_FLAG { get; set; }
        public String BACK_DAYS { get; set; }
        public String FREEZE_BACK_DAYS { get; set; }
        public String VOUCHER_DATE { get; set; }


    }
    public class REFERENCE_DETAILS
    {
        public string REFERENCE_NO { get; set; }
        public string REFERENCE_FORM_CODE { get; set; }
        public string REFERENCE_ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public decimal? REFERENCE_QUANTITY { get; set; }
        public string REFERENCE_MU_CODE { get; set; }
        public decimal? REFERENCE_UNIT_PRICE { get; set; }
        public decimal? REFERENCE_TOTAL_PRICE { get; set; }
        public decimal? REFERENCE_CALC_UNIT_PRICE { get; set; }
        public decimal? REFERENCE_CALC_TOTAL_PRICE { get; set; }
        public string REFERENCE_REMARKS { get; set; }

        //public decimal? REFERENCE_QUANTITY { get; set; }
        //public decimal? REFERENCE_UNIT_PRICE { get; set; }
        //public decimal? REFERENCE_TOTAL_PRICE { get; set; }
    }

}
