using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model
{
   public class SalesPersonModel
    {
        public string SP_CODE { get; set; }
        public string EMPLOYEE_EDESC { get; set; }
        public int? GROUPID { get; set; }
        public string AREA_CODE { get; set; }
        public string GetSalePerson { get; set; }
        public string COMPANY_CODE { get; set; }
    }

    public class SPTraveModel
    {
        public string SP_CODE { get; set; }
        public string EMPLOYEE_EDESC { get; set; }
        public DateTime? UPDATE_DATE { get; set; }
        public double LONGITUDE { get; set; }
        public double LATITUDE { get; set; }
        public string customer_edesc { get; set; }
        public string remarks { get; set; }
        public string UPDATE_DATE1 { get; set; }

    }

    public class SPDistanceModel
    {
        public string SP_CODE { get; set; }
        public string EMPLOYEE_EDESC { get; set; }
        public double DISTANCE { get; set; }
        public int ENTITY_COUNT { get; set; }
    }
}