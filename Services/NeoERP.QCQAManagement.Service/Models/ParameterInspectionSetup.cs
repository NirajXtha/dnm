using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QCQAManagement.Service.Models
{
    public class ParameterInspectionSetup
    {
        public string INS_PARAM_DETAIL_NO { get; set; }
        public string INSPECTION_PARAM_NO { get; set; }
        public string PARAMETER_ID { get; set; }
        public string PARAMETERS { get; set; }
        public List<ParameterItemDetails> ParameterItemDetailsList { get; set; }
    }
    public class ParameterItemDetails
    {
        public string ITEMS { get; set; }
    }
}
