using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Models
{
    public class CustomerInfoDataModel
    {
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_ID { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public decimal? CREDIT_LIMIT { get; set; }
        public int? CREDIT_DAYS { get; set; }
        public string TPIN_VAT_NO { get; set; }
        public string PAN_NO { get; set; }
        public string REGD_OFFICE_EADDRESS { get; set; }
        public string ACC_CODE { get; set; }
        public string TEL_MOBILE_NO1 { get; set; }
        public string TEL_MOBILE_NO2 { get; set; }
        public string ACC_EDESC { get; set; }
        public string PARTY_TYPE_EDESC { get; set; }
        public decimal? ORDER_NET_QTY { get; set; }
        public decimal? TOTAL_AMOUNT { get; set; }
        public decimal? DISPATCH_QTY { get; set; }
        public decimal? NET_SALES_AMT { get; set; }
        public decimal? CR_AMOUNT { get; set; }
        public decimal? POSTED_CR_AMT { get; set; }
        public decimal? POSTED_BL_AMT { get; set; }
    }
}
