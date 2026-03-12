using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.BrandingModule
{
    public class ActivityModel
    {
        public int MAXID { get; set; }
        public string ACTIVITY_CODE { get; set; }
        public string ACTIVITY_EDESC { get; set; }
        public string ACTIVITY_NDESC { get; set; }
        public string ACTIVITY_TYPE { get; set; }
        public int PARENT_ACTIVITY_CODE { get; set; }
        public string MASTER_ACTIVITY_CODE { get; set; }
        public string PRE_ACTIVITY_CODE { get; set; }
        public string GROUP_ACTIVITY_FLAG { get; set; }
        public string REMARKS { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set;}
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string LAST_MODIFIED_BY { get; set; }
        public DateTime? LAST_MODIFIED_DATE { get; set; }
        public string APPROVED_FLAG { get; set; }
        public string APPROVED_BY { get; set; }
        public DateTime? APPROVED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string EF1 { get; set; }
        public string EF2 { get; set; }
        public string PATH { get; set; }
        public int? count { get; set; }

    }
}
