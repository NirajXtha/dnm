using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Core.Models.CustomModels
{
    public class CRMModel
    {
        public string LEAD_NO { get; set; }

        public string LEAD_DATE { get; set; }

        public string LEAD_TIME { get; set; }

        public string COMPANY_NAME { get; set; }

        public string REQUESTED_BY { get; set; }

        public string PRODUCT_EDESC { get; set; }

        public string DESCRIPTION { get; set; }

        public string AGENT_EDESC { get; set; }

        public string PROCESS_EDESC { get; set; }

        public decimal? DAYS { get; set; }
        public string COMPLETION_DATE { get; set; }
        public decimal? ESTD_COMPLETE { get; set; }
        public decimal? RATING { get; set; }
        
    }
}
