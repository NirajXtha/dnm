using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model
{
  public class AreaModel
    {



        public string DISTRICT_CODE { get; set; }
        public string DISTRICT_NAME { get; set; }
        public string VDC_NAME { get; set; }
        public string VDC_CODE { get; set; }
        public string ZONE_CODE { get; set; }
        public string ZONE_NAME { get; set; }
        public string REG_CODE { get; set; }
        public string REG_NAME { get; set; }
        public string AREA_CODE { get; set; }
        public string AREA_NAME { get; set; }
        public string GEO_CODE { get; set; }
        public int MAXID { get; set; }
        public int? GROUPID { get; set; }
        public string GROUP_EDESC { get; set; }

    }
}
