using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Models
{
    public class IDRSyncModel
    {
    }



    // MaterializeModel replace with  IRDSyncDataModel
    // Related model ; below classes will put in common place : 
    public class IRDSyncDataModel
    {
        public string FISCAL_YEAR { get; set; }
        public string BILL_NO { get; set; }
        public string BILL_DATEAD { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public string CUSTOMER_PAN { get; set; }
        public string BILL_DATE { get; set; }
        public double? AMOUNT { get; set; }
        public double? DISCOUNT { get; set; }
        public double? TAXABLE_AMOUNT { get; set; }
        public double? TAX_AMOUNT { get; set; }
        public double? VAT { get; set; }
        public double? TOTAL_AMOUNT { get; set; }
        public string SYNC_WITH_IRD { get; set; }
        public string IS_BILL_PRINTED { get; set; }
        public string IS_BILL_ACTIVE { get; set; }
        public string PRINTED_TIME { get; set; }
        public string ENTERED_BY { get; set; }
        public string PRINTED_BY { get; set; }
        public string IS_REAL_TIME { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string FORM_CODE { get; set; }
        public string TableName { get; set; }
        public string CUSTOMER_CODE { get; set; }


        // Newly added columns
        public string REQUESTED_JSON { get; set; }
        public string RESPONSE_JSON { get; set; }
        public string REQUESTED_BY { get; set; }
    }


    // Replace of IRD model 
    public class BillReturnViewModel
    {
        public string username { get; set; }
        public string password { get; set; }
        public string seller_pan { get; set; }
        public string buyer_pan { get; set; }
        public string fiscal_year { get; set; }
        public string buyer_name { get; set; }
        public string ref_invoice_number { get; set; }
        public string credit_note_number { get; set; }
        public string credit_note_date { get; set; }
        public string reason_for_return { get; set; }
        public double total_sales { get; set; }
        public double taxable_sales_vat { get; set; }
        public double vat { get; set; }
        public double excisable_amount { get; set; }
        public double excise { get; set; }
        public double taxable_sales_hst { get; set; }
        public double hst { get; set; }
        public double amount_for_esf { get; set; }
        public double esf { get; set; }
        public double export_sales { get; set; }
        public double tax_exempted_sales { get; set; }
        public bool isrealtime { get; set; }
        public DateTime datetimeclient { get; set; }
    }


    public class BillViewModelIRD
    {
        public string username { get; set; }
        public string password { get; set; }
        public string seller_pan { get; set; }
        public string buyer_pan { get; set; }
        public string fiscal_year { get; set; }
        public string buyer_name { get; set; }
        public string invoice_number { get; set; }
        public string invoice_date { get; set; }

        public double total_sales { get; set; }
        public double taxable_sales_vat { get; set; }
        public double vat { get; set; }
        public double excisable_amount { get; set; }
        public double excise { get; set; }
        public double taxable_sales_hst { get; set; }
        public double hst { get; set; }
        public double amount_for_esf { get; set; }
        public double esf { get; set; }
        public double export_sales { get; set; }
        public double tax_exempted_sales { get; set; }

        public bool isrealtime { get; set; }
        public DateTime datetimeclient { get; set; }
    }

    public class CustomerDetails
    {
        public string CUSTOMER_EDESC { get; set; }
        public string TPIN_VAT_NO { get; set; }
    }



    public class ApiSettingModel
    {
        public string USER_NAME { get; set; }         // VARCHAR2(50)
        public string API_PWD { get; set; }           // VARCHAR2(50)
        public string SALES_URL { get; set; }         // VARCHAR2(100)
        public string SALES_RETURN_URL { get; set; }  // VARCHAR2(100)
        public string COMPANY_CODE { get; set; }      // VARCHAR2(30)
        public string CREATED_BY { get; set; }        // VARCHAR2(30)
        public DateTime CREATED_DATE { get; set; }    // DATE
        public string DELETED_FLAG { get; set; }      // CHAR(1)
        public long PAN_NO { get; set; }              // NUMBER
        public string SERVER_NAME { get; set; }       // VARCHAR2(50)
    }


}
