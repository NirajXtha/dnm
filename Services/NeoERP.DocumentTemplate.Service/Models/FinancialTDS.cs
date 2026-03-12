using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Models
{
   public class FinancialTDS
    {
        public string ACC_CODE { get; set; }
        public string VOUCHER_NO { get; set; }
        public string SUBMISSION_NO { get; set; }
        public string MANUAL_NO { get; set; }
        public string FORM_CODE { get; set; }
        public string CS_CODE { get; set; }
        public DateTime? INVOICE_DATE { get; set; }
        public string REMARKS { get; set; }
        public Decimal? TOTAL_TDS_AMOUNT { get; set; }
        public int? SERIAL_NO { get; set; }
        public string SESSION_ROWID { get; set; }
        public string SUPPLIER_CODE { get; set; }
       
        public string TDS_TYPE_CODE { get; set; }
        public string MEETING_CODE { get; set; }
        public Decimal? NET_AMOUNT { get; set; }
        public Decimal? TDS_PERCENTAGE { get; set; }
        public Decimal? TDS_PERCENT { get; set; }
        public Decimal? TDS_AMOUNT { get; set; }
        public Decimal? TAXABLE_AMOUNT { get; set; }
       
        public List<FinancialChildTDS> CHILDTDS { get; set; }


    }
    public class FinancialChildTDS
    {

        public int? SERIAL_NO { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string ACC_CODE { get; set; }
        public string TDS_TYPE_CODE { get; set; }
        public string MEETING_CODE { get; set; }
        public Decimal? NET_AMOUNT { get; set; }
        public Decimal? TDS_PERCENTAGE { get; set; }
        public Decimal? TDS_PERCENT { get; set; }
        public Decimal? TDS_AMOUNT { get; set; }
        public Decimal? TAXABLE_AMOUNT { get; set; }
        public string CS_CODE { get; set; }
        public string VOUCHER_NO { get; set; }
        public string SUBMISSION_NO { get; set; }


    }
}
