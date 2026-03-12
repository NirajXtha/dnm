using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Models
{
    public class SalesBlanketDetailModel
    {
        public SalesBlanketDetailModel()
        {
            CustomBlanketTransaction = new List<CustomBlanketColumn>();
        }
        public SalesBlanketDetail MasterBlanketTransaction { get; set; }
        public List<SalesBlanketDetail> ChildBlanketTransaction { get; set; }
        public List<CustomBlanketColumn> CustomBlanketTransaction { get; set; }
        public List<string> ReferenceBlanketTransaction { get; set; }
        public string InvItemChargeTransaction { get; set; }
        public List<ChargeOnSales> ChargeBlanketTransaction { get; set; }
        public ShippingDetails ShippingBlanketTransaction { get; set; }
        public List<REF_MODEL_DEFAULT> RefenceModel { get; set; }
        public int TotalChild { get; set; } = 0;
        public List<BATCHTRANSACTIONDATA> BatchBlanketTransaction { get; set; }
    }
}