using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model
{
   public class OutletModel
    {
        public int TYPE_ID { get; set; }
        public string TYPE_CODE { get; set; }
        public string TYPE_EDESC { get; set; }
        public string COMPANY_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public DateTime? MODIFY_DATE { get; set; }
        public string MODIFY_BY { get; set; }
        //public <IList> SubtypeArray { get; set; }

         public List<SubtypeArray> subtypeArr { get; set; }


    }

public class SubtypeArray {
        public int SUBTYPE_ID { get; set; }
        public string SUBTYPE_CODE { get; set; }
        public string SUBTYPE_EDESC { get; set; }

    }

    public class OutletSubtypeModel : OutletModel
    {

        public int SUBTYPE_ID { get; set; }
        public string SUBTYPE_CODE { get; set; }
        public string SUBTYPE_EDESC { get; set; }
    }


}
