using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Transaction.Service.Models
{
    public class BankSetupModel
    {
        public int BankCode { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string SwiftCode { get; set; }
        public string Address { get; set; }
        public string Branch { get; set; }
        public string PhoneNumber { get; set; }
        public string Remarks { get; set; }
        public List<BankContacts> Contacts { get; set; }
        public BankSetupModel()
        {
            Contacts = new List<BankContacts>();
        }
    }
    public class BankContacts
    {
        public int Sn { get; set; }
        public string ContactPerson { get; set; }
        public string MobileNumber { get; set; }
        public string TelephoneNumber { get; set; }
        public string Designation { get; set; }
        public string Remarks { get; set; }
    }
}