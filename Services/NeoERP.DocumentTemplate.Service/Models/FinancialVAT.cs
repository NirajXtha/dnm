using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Models
{
   public class FinancialVAT
    {
        public string ACC_CODE { get; set; }
        public string INVOICE_NO { get; set; }
        public string MANUAL_NO { get; set; }
        public string FORM_CODE { get; set; }
        public DateTime? INVOICE_DATE { get; set; }
        public string REMARKS { get; set; }
        public Decimal? TOTAL_VAT_AMOUNT { get; set; }
        public Decimal? PURCHASE_QUANTITY { get; set; }
        public string SESSION_ROWID { get; set; }
        public string PP_NO { get; set; }
        public string PURCHASE_ITEM { get; set; }
        public string PURCHASE_UNIT { get; set; }
        public string CS_CODE { get; set; }
        public string TYPE { get; set; }
        public string DOC_TYPE { get; set; }
        public int? SERIAL_NO { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public Boolean Enable_DirectEntry { get; set; } = false;
        public string VEHICLE_PUR_FLAG { get; set; }
        public string EXEMPTED_FLAG { get; set; }
        public Decimal? VAT_AMOUNT { get; set; }
        public Decimal? TAXABLE_AMOUNT { get; set; }
        public List<FinancialChildVAT> CHILDVAT { get; set; }

    }
    public class FinancialChildVAT
    {

        public int? SERIAL_NO { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string ACC_CODE { get; set; }
        public Decimal? VAT_AMOUNT { get; set; }
        public Decimal? TAXABLE_AMOUNT { get; set; }
       


    }
}
