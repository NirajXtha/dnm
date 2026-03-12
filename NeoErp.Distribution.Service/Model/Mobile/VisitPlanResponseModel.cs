using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.Mobile
{
    public class VisitPlanResponseModel
    {
        public VisitPlanResponseModel()
        {
            entity = new Dictionary<string, VisitEntityModel>();
        }
        public string code { get; set; }
        public Dictionary<string, VisitEntityModel> entity { get; set; }
    }

    public class VisitEntityModel
    {
        public string Employee_Code { get; set; }
        public string Employee_Edesc { get; set; }
        public string Route_Name { get; set; }
        public string Route_Code { get; set; }
        public string Code { get; set; }
        public string Email { get; set; }
        public string Wholeseller { get; set; }
        public string acc_code { get; set; }
        public string address { get; set; }
        public string area_code { get; set; }
        public string area_name { get; set; }
        public string assign_date { get; set; }
        public string default_party_type_code { get; set; }
        public string is_visited { get; set; }
        public string last_visit_by { get; set; }
        public string last_visit_date { get; set; }
        public string last_visit_status { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string name { get; set; }
        public string order_no { get; set; }
        public string p_contact_no { get; set; }
        public string parent_distributor_code { get; set; }
        public string parent_distributor_name { get; set; }
        public string remarks { get; set; }
        public string type { get; set; }
    }

    public class VisitBrdModel
    {
        public string EMPLOYEE_CODE { get; set; }
        public string EMPLOYEE_EDESC { get; set; }
        public string ROUTE_NAME { get; set; }
        public string ROUTE_CODE { get; set; }
        public string CODE { get; set; }
        public string ENTITY_EDESC { get; set; }
        public string CONTACT_NO { get; set; }
        public string AREA_CODE { get; set; }
        public string AREA_NAME { get; set; }
        public string ASSIGN_DATE { get; set; }
        public string ORDER_NO { get; set; }
        public string LATITUDE { get; set; }
        public string LONGITUDE { get; set; }
        public string COMPANY_CODE { get; set; }
    }
}
