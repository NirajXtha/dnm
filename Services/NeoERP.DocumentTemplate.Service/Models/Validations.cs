using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Models
{
    public class CreditLimitValidations
    {
        public decimal DR_TOTAL { get; set; }
        public decimal CR_TOTAL { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public int CREDIT_LIMIT { get; set; }
        public decimal BALANCE { get; set; }
        public int EXCEED_LIMIT_PERCENTAGE { get; set; }
    }
}
