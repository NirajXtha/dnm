using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QCQAManagement.Service.Models
{
    public class Form_Setup
    {
       
        public int ID { get; set; }
        public string FORM_CODE { get; set; }
        public string FORM_EDESC { get; set; }
        public int FORM_NDESC { get; set; }
        public string MASTER_FORM_CODE { get; set; }
        //public DateTime? CREATED_DATE { get; set; }
        public string TEMPLATE_CODE { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string CUSTOM_PREFIX_TEXT { get; set; }
        public string CUSTOM_SUFFIX_TEXT { get; set; }
        public string PREFIX_LENGTH { get; set; }
        public string SUFFIX_LENGTH { get; set; }
        public string START_NO { get; set; }
        public string LAST_NO { get; set; }

        public string START_DATE { get; set; }
        public string LAST_DATE { get; set; }


        //public int ID { get; set; }
        //public string PREFIX { get; set; }
        //public string SUFFIX { get; set; }
        //public int BODY_LENGTH { get; set; }
        //public string STATUS { get; set; }
        //public DateTime? CREATED_DATE { get; set; }
        //public string CREATED_BY { get; set; }
        //public string COMPANY_CODE { get; set; }

    }
}
