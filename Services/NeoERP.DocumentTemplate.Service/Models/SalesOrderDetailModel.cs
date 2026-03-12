using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Models
{
    public class SalesOrderDetailModel
    {
        public SalesOrderDetailModel()
        {
            CustomOrderTransaction = new List<CustomOrderColumn>();
        }
        public SalesOrderDetail MasterTransaction { get; set; }
        public List<SalesOrderDetail> ChildTransaction { get; set; }
        public List<CustomOrderColumn> CustomOrderTransaction { get; set; }
        public List<string> ReferenceTransaction { get; set; }
        public string InvItemChargeTransaction { get; set; }
        public List<ChargeOnSales> ChargeTransaction { get; set; }
        public ShippingDetails ShippingTransaction { get; set; }
        public List<REF_MODEL_DEFAULT> RefenceModel { get; set; }
        public int TotalChild { get; set; } = 0;
        public List<BATCHTRANSACTIONDATA> BatchTransaction { get; set; }
    }
}
