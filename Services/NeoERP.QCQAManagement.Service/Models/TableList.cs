using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QCQAManagement.Service.Models
{
    public class TableList
    {
        public string TABLE_NAME { get; set; }
        public string TABLE_DESC { get; set; }
        public string COLUMN_NAME { get; set; }
        public string POST_REQUIRED_FLAG { get; set; }
        public string REMARKS { get; set; }
        public string SYN_ROWID { get; set; }
        public string MODIFY_BY { get; set; }
    }
}
