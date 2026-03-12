using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.ProjectManagement.Service.Models
{
    public class ProjectCount
    {
        public int ID { get; set; }
        public string PROJECT_NAME { get; set; }
        public int SUB_PROJECT_COUNT { get; set; }
        public int COUNT { get; set; }
        public decimal TOTAL_AREA { get; set; }
        public decimal TOTAL_BUDGET { get; set; }
        public DateTime? START_DATE { get; set; }
        public DateTime? END_DATE { get; set; }
        public DateTime? CREATED_DT { get; set; }
        public int LABOUR_COUNT { get; set; }
        public int MATERIAL_COUNT { get; set; }
        public string HEADING { get; set; }
        public string COLOR { get; set; }
        public string ICON { get; set; }
        public int SORTORDER { get; set; }
    }

}
