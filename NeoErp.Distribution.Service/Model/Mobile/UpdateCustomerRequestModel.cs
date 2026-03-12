using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.Mobile
{
    public class UpdateCustomerRequestModel:CommonRequestModel
    {
        public string code { get; set; }
        public string type { get; set; }
    }
}
