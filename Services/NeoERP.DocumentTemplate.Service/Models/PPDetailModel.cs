using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Models
{
        public class PPDetailModel
        {
            public string REFERENCE_NO { get; set; }
            public string PP_NO { get; set; }
            public string SUPPLIER_INV_NO { get; set; }
            public string INVOICE_DATE { get; set; }
            public string CS_CODE { get; set; }
            public decimal INVOICE_AMOUNT { get; set; }
            public decimal IMPORT_DUTY_AMOUNT { get; set; }
            public decimal VAT_AMOUNT { get; set; }
            public decimal TRANSPORTATION_AMOUNT { get; set; }
            public decimal INSURENCE_AMOUNT { get; set; }
            public decimal FREIGHT_AMOUNT { get; set; }
            public decimal OTHERS_AMOUNT { get; set; }
            public decimal TOTAL_COST { get; set; }
            public decimal ECS_AMOUNT { get; set; }
            public decimal CUSTOM_FEE { get; set; }
            public string REMARKS { get; set; }
            public string FORM_CODE { get; set; }
            public string COMPANY_CODE { get; set; }
            public string BRANCH_CODE { get; set; }
            public string CREATED_BY { get; set; }
            public string CREATED_DATE { get; set; }
            public string DELETED_FLAG { get; set; }
            public string CURRENCY_CODE { get; set; }
            public decimal EXCHANGE_RATE { get; set; }
            public string SESSION_ROWID { get; set; }
            public decimal TAXABLE_AMOUNT { get; set; }
            public string CUSTOM_OFFICE { get; set; }
            public string BILL_OF_EXPORT_NO { get; set; }
            public string C_FORM_NO { get; set; }
            public decimal GREEN_TAX { get; set; }
        }

}
