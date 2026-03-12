using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.Mobile
{
    public class MuCodeResponseModel
    {
        public MuCodeResponseModel()
        {
            CONVERSION_UNIT_FACTOR = new Dictionary<string, string>();
        }
        public string ITEM_CODE { get; set; }
        public string INDEX_MU_CODE { get; set; }
        public string MU_CODE { get; set; }
        public string CONVERSION_FACTOR { get; set; }
        public string COMPANY_CODE { get; set; }
        public Dictionary<string, string> CONVERSION_UNIT_FACTOR { get; set; }
    }
}
