using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Models
{
    public class FormCustomSetup
    {
        public string FIELD_NAME { get; set; }
        public string DEFA_FIELD_VALUE { get; set; }
        public string FIELD_VALUE { get; set; }
        public string FORM_CODE { get; set; }

        public string COMPANY_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }

        public string DELETED_FLAG { get; set; }
        public string SYN_ROWID { get; set; }

        public DateTime? MODIFY_DATE { get; set; }

        public string MODIFY_BY { get; set; }

        public int? SERIAL_NO { get; set; }

        public string FIELD_TYPE { get; set; }

        public string FIELD_MANDATORY { get; set; }

        public string HELP_DESCRIPTION { get; set; }

    }
}
