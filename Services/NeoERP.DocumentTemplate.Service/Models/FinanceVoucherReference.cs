using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Models
{
    public class FinanceVoucherReference
    {
        public string  voucher_no { get; set; }
        public string reference_form_code { get; set; }
        public string FORM_EDESC { get; set; }
        
    }
    public class FVPURCHASEEXPSHEETRERERENCE
    {
     
        public string REFERENCE_NO { get; set; }
        public string FORM_EDESC { get; set; }

    }
}
