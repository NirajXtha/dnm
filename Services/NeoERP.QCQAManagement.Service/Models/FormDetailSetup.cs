using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QCQAManagement.Service.Models
{
    public class FormDetailSetup
    {
        public decimal SERIAL_NO { get; set; }
        public string COLUMN_NAME { get; set; }
        public int COLUMN_WIDTH { get; set; }
        public string COLUMN_HEADER { get; set; }
        public int TOP_POSITION { get; set; }
        public int LEFT_POSITION { get; set; }
        public string DISPLAY_FLAG { get; set; }
        public string DEFA_VALUE { get; set; }
        public string IS_DESC_FLAG { get; set; }
        public string MASTER_CHILD_FLAG { get; set; }
        public string FILTER_VALUE { get; set; }
        public string FORM_CODE { get; set; }



        public string TABLE_NAME { get; set; }

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
    }
    public class FormSetupModel
    {
        public string FORM_CODE { get; set; }
        public string FORM_EDESC { get; set; }
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

    public class FormDetailsSetup
    {
        public FormDetailsSetup()
        {
            FormSetupRefrence = new List<FormDetailSetup>();
        }
        public List<FormDetailSetup> FormSetupRefrence { get; set; }
    }
}
