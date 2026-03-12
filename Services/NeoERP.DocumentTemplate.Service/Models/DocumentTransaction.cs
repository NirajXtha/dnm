using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Models
{
    public class DocumentTransaction
    {
        public string VOUCHER_NO { get; set; }
        public DateTime VOUCHER_DATE { get; set; }
        public int SERIAL_NO { get; set; }
        public string FORM_CODE { get; set; }
        public string SYN_ROWID { get; set; }
        public string SESSION_ID { get; set; }
        public string DOCUMENT_NAME { get; set; }
        public string DOCUMENT_FILE_NAME { get; set; }
    }

    public class CancleSalesOrder
    {
        public decimal? CALCEL_QTY { get; set; }
        public decimal? ADJUST_QUANTITY { get; set; }
        public decimal? SERIAL_NO { get; set; }
    }
}
