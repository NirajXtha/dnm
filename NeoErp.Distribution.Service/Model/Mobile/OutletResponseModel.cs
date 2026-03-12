using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.Mobile
{
    public class OutletResponseModel
    {
        public OutletResponseModel()
        {
            SIZE = new Dictionary<string, SubTypeModel>();
        }
        public string TYPE_ID { get; set; }
        public string TYPE_CODE { get; set; }
        public string TYPE_EDESC { get; set; }
        public string DELETED_FLAG { get; set; }
        public Dictionary<string,SubTypeModel> SIZE { get; set; }
    }
    public class SubTypeModel
    {
        public string SUBTYPE_ID { get; set; }
        public string SUBTYPE_CODE { get; set; }
        public string SUBTYPE_EDESC { get; set; }
        public string TYPE_ID { get; set; }
    }
    public class OutletTemp
    {
        public string TYPE_ID { get; set; }
        public string TYPE_CODE { get; set; }
        public string TYPE_EDESC { get; set; }
        public string DELETED_FLAG { get; set; }
        public string SUBTYPE_ID { get; set; }
        public string SUBTYPE_CODE { get; set; }
        public string SUBTYPE_EDESC { get; set; }
    }
}
