using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model
{
    public class QuickSetupModel
    {
        public string ROUTE_CODE { get; set; }
        public string EMP_CODE { get; set; }
        public int PLAN_CODE { get; set; }
        public DateTime ASSIGN_DATE { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime APPROVE_DATE { get; set; }
        public string APPROVE_BY { get; set; }
        public DateTime MODIFY_DATE { get; set; }
        public string MODIFY_BY { get; set; }
        public string DELETED_FLAG { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
    }
}
