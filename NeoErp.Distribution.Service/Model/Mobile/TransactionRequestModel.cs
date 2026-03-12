using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.Mobile
{
    public class TransactionRequestModel:CommonRequestModel
    {
        public string from_date { get; set; }
        public string to_date { get; set; }
        public string sub_code { get; set; }
        public string acc_code { get; set; }
    }
}
