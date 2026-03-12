using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model
{
   public class OutletSummaryModel
    {
        public string SP_CODE { get; set; }
        public string EMPLOYEE_EDESC { get; set; }
        public int NOT_VISITED { get; set; }
        public int VISITED { get; set; }
    }
}
