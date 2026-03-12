using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NeoErp.Core.Quotation
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