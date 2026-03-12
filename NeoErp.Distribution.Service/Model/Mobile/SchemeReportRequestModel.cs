using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.Mobile
{
    public class SchemeReportRequestModel
    {
        public string SP_CODE { get; set; }
        public string ITEM_CODE { get; set; }
        public DateTime DATE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
    }
}
