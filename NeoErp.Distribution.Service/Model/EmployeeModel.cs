using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model
{
  public  class EmployeeModel
    {
        public string EMPLOYEE_CODE { get; set; }
        public string EMPLOYEE_EDESC { get; set; }
        public string EMPLOYEE_NDESC { get; set; }
        public string COMPANY_CODE { get; set; }
    }
    
    public  class EmployeeAreaModel
    {
        public string EMPLOYEE_CODE { get; set; }
        public string EMPLOYEE_EDESC { get; set; }
        public string EMPLOYEE_NDESC { get; set; }
        public List<string> AREA_CODE { get; set; }
    }
}
