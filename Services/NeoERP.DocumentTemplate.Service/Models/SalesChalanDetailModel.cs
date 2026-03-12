using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Models
{
    public class SalesChalanDetailModel
    {
        public SalesChalanDetail MasterChalanTransaction { get; set; }
        public List<SalesChalanDetail> ChildChalanTransaction { get; set; }
        public List<CustomOrderColumn> CustomOrderTransaction { get; set; } = new List<CustomOrderColumn>();
        public List<string> ReferenceTransaction { get; set; }
        public string InvItemChargeTransaction { get; set; }
        public List<ChargeOnSales> ChargeTransaction { get; set; }
        public ShippingDetails ShippingTransaction { get; set; }
        public List<REF_MODEL_DEFAULT> RefenceModel { get; set; }

        public List<BATCHTRANSACTIONDATA> BatchTransaction { get; set; }

    }
}
