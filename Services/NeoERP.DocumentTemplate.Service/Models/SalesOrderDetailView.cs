using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Models
{
   public class SalesOrderDetailView
    {
        public string FORM_CODE { get; set; }
        public string FORM_EDESC { get; set; }
        public string ORDER_NO { get; set; }
        public DateTime ORDER_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public DateTime? MODIFY_DATE { get; set; }
        public string MODIFY_BY { get; set; }
    }
}
