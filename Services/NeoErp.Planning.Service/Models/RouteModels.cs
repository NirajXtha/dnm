using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Planning.Service.Models
{
    public class RouteModels
    {
        public string ROUTE_CODE { get; set; }
        public string ROUTE_EDESC { get; set; }
        public string ROUTE_NAME { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public int? PLAN_CODE { get; set; }

        public string AREA_CODE { get; set; }
        public string AREA_NAME { get; set; }
    }

    public class Area_Master
    {
        public string AREA_CODE { get; set; }
        public string AREA_NAME { get; set; }
        public string ZONE_CODE { get; set; }
        public string VDC_CODE { get; set; }
        public string GEO_CODE { get; set; }
        public string REG_CODE { get; set; }
    }
    public class RoutePlanModel
    {
        public string ROUTE_CODE { get; set; }
        public string ROUTE_EDESC { get; set; }
        public string ROUTE_NAME { get; set; }
        public string ASSIGN_DATE { get; set; }
        public DateTime? START_DATE { get; set; }
        public DateTime? END_DATE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public int? PLAN_CODE { get; set; }
        public string EMP_CODE { get; set; }
    }
}
