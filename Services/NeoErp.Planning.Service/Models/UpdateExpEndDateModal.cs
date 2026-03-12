using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Planning.Service.Models
{
    public class UpdateExpEndDateModal
    {
        public string PLAN_CODE { get; set; }
        public string ROUTE_CODE { get; set; }
        public DateTime END_DATE {get;set;}
        public DateTime EDITED_END_DATE { get; set; }
    }
}
