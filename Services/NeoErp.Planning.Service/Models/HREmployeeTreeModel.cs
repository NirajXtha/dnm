using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Planning.Service.Models
{
    public class HREmployeeTreeModel
    {
        public string EMPLOYEE_CODE { get; set; }
        public string EMPLOYEE_EDESC { get; set; }
        public string PARENT_EMPLOYEE_CODE { get; set; }
        public string PARENT_EMPLOYEE_EDESC { get; set; }
        public int LEVEL { get; set; }
    }
}
