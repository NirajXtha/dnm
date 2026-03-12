using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.Mobile
{
    public class CalenderDataModel
    {
        public string MONTH_DAY { get; set; }
        public int EMPLOYEE_ID { get; set; }
        public string ATTENDANCE_DT { get; set; }
        public string IN_TIME { get; set; }
        public string OUT_TIME { get; set; }
        public string ATTENDANCE_STATUS { get; set; }
        public string OVERALL_STATUS { get; set; }
    }
    public class AttendanceCountModel
    {
        public int? PRESENT { get; set; }
        public int? ABSENT { get; set; }
        public int? LATE_IN { get; set; }
        public int? EARLY_OUT { get; set; }
        public int? LEAVE { get; set; }
        public int? TRAVEL { get; set; }
        public int? TRAINING { get; set; }
        public int? WORKON_DAYOFF { get; set; }
        public int? WORKON_HOLIDAY { get; set; }
        public int? LATE_PENALTY { get; set; }
        public int? MISSED_PUNCH { get; set; }
    }
    public class AttendanceStatusModel
    {
        public string ATTENDANCE_STATUS { get; set; }
    }
}
