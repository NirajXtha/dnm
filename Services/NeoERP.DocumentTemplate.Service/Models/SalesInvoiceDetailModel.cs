using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Models
{
    public class SalesInvoiceDetailModel
    {
        public SalesInvoiceDetail MasterInvoiceTransaction { get; set; }
        public List<SalesInvoiceDetail> ChildInvoiceTransaction { get; set; }
        public List<CustomOrderColumn> CustomOrderTransaction { get; set; } = new List<CustomOrderColumn>();
        public List<string> ReferenceTransaction { get; set; }
        public string InvItemChargeTransaction { get; set; }
        public List<ChargeOnSales> ChargeTransaction { get; set; }
        public ShippingDetails ShippingTransaction { get; set; }
        public List<REF_MODEL_DEFAULT> RefenceModel { get; set; }
        public List<BATCHTRANSACTIONDATA> BatchTransaction { get; set; }
    }
}
