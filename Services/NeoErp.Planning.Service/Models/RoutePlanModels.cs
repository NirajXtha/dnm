using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NeoErp.Planning.Service.Models
{
   
    public class RoutePlanModels
    {
        public string ROUTE_PLAN_NAME { get; set; }
        public string ROUTE_CODE { get; set; }
        public string PLAN_CODE { get; set; }
        public string START_DATE { get; set; }
        public string END_DATE { get; set; }
        public string FREQUENCY_CODE { get; set; }
        public string REMARKS { get; set; }
    }

    public class RoutePlanListModels
    {
        public string PLAN_ROUTES_CODE { get; set; }
        public string NAME_EDESC { get; set; }
        public string FREQUENCY_CODE { get; set; }
        public string TIME_FRAME_EDESC { get; set; }
        public string NAME_NDESC { get; set; }
        public DateTime START_DATE { get; set; }
        public DateTime END_DATE { get; set; }
        public string ROUTE_CODE { get; set; }
        public string ROUTE_EDESC { get; set; }
        public string REMARKS { get; set; }
        public string DELETED_FLAG { get; set; }
    }

    public class RoutePlanDateSeries
    {
        public string DATES { get; set; }
        public string YEAR { get; set; }
        public string MONTH { get; set; }
        public string MONTH_NAME { get; set; }
        public string WEEK { get; set; }
        public string DAY { get; set; }
    }

    public class EMP_GROUP
    {
        public int? GROUPID { get; set; }

        public string GROUP_CODE { get; set; }
        public string GROUP_EDESC { get; set; }
    }
    public class DIST_ROUTE_PLAN
    {
        public int? PLAN_CODE { get; set; }
        public string PLAN_EDESC { get; set; }
        public string PLAN_NDESC { get; set; }
        public DateTime? START_DATE { get; set; }
        public DateTime? END_DATE { get; set; }
        public string TARGET_NAME { get; set; }
        public int? TARGET_VALUE { get; set; }
        public int? TIME_FRAME_CODE { get; set; }
        public string REMARKS { get; set; }
        public string TIME_FRAME_EDESC { get; set; }
    }

    public class DIST_ROUTE_DETAIL
    {
        public int PLAN_CODE { get; set; }
        public string ROUTE_CODE { get; set; }
        public string EMP_CODE { get; set; }
        public DateTime ASSIGN_DATE { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime MODIFIED_DATE { get; set; }
        public string MODIFY_BY { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string DELETED_FLAG { get; set; }
    }

    public class DIST_CALENDAR_ROUTE
    {
        public string planCode { get; set; }
        public string addEdit { get; set; }
        public string planName { get; set; }
        public string startDate { get; set; }

        public string endDate { get; set; }
        public string empCode { get; set; }
        public List<ModelData> eventArr { get; set; }
    }

    public class ModelData
    {
        public string routeCode { get; set; }
        public string title { get; set; }
        public string start { get; set; }
        public string end { get; set; }
    }

    public class ExcelRoutePlan {
        
        public string PlanName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string RouteCode{ get; set; }
        public string RouteName { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string AssignDate { get; set; }
        


    }
}
