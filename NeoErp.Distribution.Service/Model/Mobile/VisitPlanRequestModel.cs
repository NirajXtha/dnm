using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.Mobile
{
    public class VisitPlanRequestModel:CommonRequestModel
    {
        public VisitPlanRequestModel()
        {
            entities = new List<string>();
        }
        public string user_type { get; set; }
        public string spcode { get; set; }
        public string sp_code { get; set; }
        public string date { get; set; }
        public List<string> entities { get; set; }
    }
}
