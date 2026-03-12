using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.Mobile
{
    public class InformationSaveModel:CommonRequestModel
    {
        public string entity_code { get; set; }
        public string entity_type { get; set; }
        public string information { get; set; }
    }
}
