using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models
{
    public class AuditTrailModel
    {
        public string FORM_CODE { get; set; }
        public string LOG_BRANCH { get; set; }
        public string LOG_COMPANY { get; set; }
        public string LOG_DATE { get; set; }
        public int LOG_ID { get; set; }
        public string LOG_LEVEL { get; set; }
        public string LOG_LOGGER { get; set; }
        public string LOG_MESSAGE { get; set; }
        public string LOG_MODULE { get; set; }
        public string LOG_THREAD { get; set; }
        public string LOG_TYPECODE { get; set; }
        public string LOG_USER { get; set; }
    }
    public class AuditTrailViewModel
    {
        public AuditTrailViewModel()
        {
            auditTrialReportModel = new List<AuditTrailModel>();
            this.AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public List<AuditTrailModel> auditTrialReportModel { get; set; }

        public Dictionary<string, AggregationModel> AggregationResult { get; set; }

        public int total { get; set; }
    }
}
