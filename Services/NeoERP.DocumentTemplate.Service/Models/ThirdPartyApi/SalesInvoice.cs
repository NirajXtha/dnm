using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Models.ThirdPartyApi
{
    public class SalesInvoice
    {
        public List<MasterTransaction> Transactions { get; set; }
        public string ResponseMessage { get; set; }
    }
    public class MasterTransaction
    {
        public string salesNumber { get; set; }
        // public string salesAmount { get; set; }
        public string salesDate { get; set; }
        public string currencyCode { get; set; }
        public string exchangeRate { get; set; }
        public string dealer { get; set; }
        public string salesType { get; set; }
        public string area { get; set; }
        public string city { get; set; }
        public string marketingPerson { get; set; }
        public CustomerInformation CustomerInformation { get; set; }
        public List<LineItem> LineItem { get; set; }
        public List<ChargeTransactions> ChargeTransactions { get; set; }
       
    }
    public class CustomerInformation
    {
        public string customerName { get; set; }
        public string address { get; set; }
        public string telMobileNo1 { get; set; }
        public string telMobileNo2 { get; set; }
        public string vatNumber { get; set; }
        public string groupName { get; set; }
    }
    public class LineItem
    {
        public string serialNumber { get; set; }
        public string itemCode { get; set; }
        public string itemName { get; set; }
        public string description { get; set; }
        public string muCode { get; set; }
        public string quantity { get; set; }
        public string unitPrice { get; set; }
        public string totalPrice { get; set; }
        public string remarks { get; set; }
    }

    public class ChargeTransactions
    {
        public string serialNumber { get; set; }
        public string applyOn { get; set; }
        public string chargeCode { get; set; }
        public string chargeTypeFlag { get; set; }
        public string chargeAmount { get; set; }
        public string chargeName { get; set; }
    }

    public class SalesInvoiceApiParameter
    {
        public string salesInvoiceNumber { get; set; }
        public DateTime? dateTime { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
    }
}
