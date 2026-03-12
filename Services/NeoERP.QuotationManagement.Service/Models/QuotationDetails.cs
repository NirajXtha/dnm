using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QuotationManagement.Service.Models
{
    public class QuotationDetails
    {
        public int Quotation_No { get; set; }
        public string Tender_No { get; set; }
        public string Form_Code { get; set; }
        public DateTime Created_Date { get; set; }
        public string SUPPLIER_Code { get; set; }
        public string Address { get; set; }
        public string Contact_No { get; set; }
        public string Item_Code { get; set; }
        public string Specification { get; set; }
        public string Index_Mu_Code { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal Total_Net_Amount { get; set; }
        public string Company_Code { get; set; }
        public string Branch_Code { get; set; }
        public string Currency { get; set; }
        public decimal Currency_Rate { get; set; }
        public DateTime Delivery_Date { get; set; }
        public string Brand_Name { get; set; }
    }

}
