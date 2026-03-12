using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model
{
    public class RouteModel
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

    public class DIST_PLAN_ROUTE
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

    public class RouteModelPlan
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

    public class TargetModelPLan
    {
        public string TargetName { get; set; }
        public string Employee_Name { get; set; }
        public string Duration { get; set; }
        public string TargetType { get; set; }
        public int? TargetQuantity { get; set; }
        public int? TargetAmount { get; set; }
        public int? Achieved_Quantity { get; set; }
        public int? Achieved_Amount { get; set; }
        public int? Accomplished_Quantity { get; set; }
        public int? Accomplished_Amount { get; set; }
    }


    public class ModelEmployee
    {
        public string EMPLOYEE_CODE { get; set; }
        public string EMPLOYEE_EDESC { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string MASTER_EMPLOYEE_CODE { get; set; }
        public string PRE_EMPLOYEE_CODE { get; set; }
        public int? Childrens { get; set; }
        public int LEVEL { get; set; }

    }
}
