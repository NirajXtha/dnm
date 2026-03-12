using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.Mobile
{
    public class ProfileDetails
    {
        public string SP_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string START_DATE { get; set; }
        public string END_DATE { get; set; }
    }
    public class PERSONAL_DETAILS
    {
        public string SP_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public int USERID { get; set; }
        public string EMPLOYEE_EDESC { get; set; }
        public string CONTACT_NO { get; set; }
        public string LoginTime { get; set; }
    }
    public class PLAN_VISIT_TARGET
    {
        public int PLAN_TARGET { get; set; }
        public int PLAN_ACHIEVED { get; set; }
    }
    public class NONPLAN_VISIT_TARGET
    {
        public int NONPLAN_TARGET { get; set; }
        public int NONPLAN_ACHIEVED { get; set; }
    }
}
