using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoERP.QuotationManagement.Models
{
    public class Company
    {
        public string COMPANY_CODE { get; set; }
        public string COMPANY_EDESC { get; set; }
        public string ADDRESS { get; set; }
        public string EMAIL { get; set; }
        public string LOGO_FILE_NAME { get; set; }
    }
}