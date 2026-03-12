using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.Mobile
{
    public class CrmModel
    {
        public string LEAD_NO { get; set; }
        public int AGENT_CODE { get; set; }
        public string EMPLOYEE_EDESC { get; set; }
        public string PROCESS_EDESC { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string LEAD_ISSUE { get; set; }
        public string REMARKS { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public DateTime? START_DATE { get; set; }
        public string PROCESS_NO { get; set; }
        public string PROCESS_CODE { get; set; }
        public string Sync_Id { get; set; }
    }
}
