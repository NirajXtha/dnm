using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QCQAManagement.Service.Models
{
    public class SanitationHygiene
    {
        public string SANITATION_NO { get; set; }
        public string DEPARTMENT_CODE { get; set; }
        public string DEPARTMENT_EDESC { get; set; }
        public decimal? STANDARD { get; set; }
        public decimal? ACTUAL { get; set; }
        public decimal? GAP { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string MODIFY_DATE { get; set; }
        public string MODIFY_BY { get; set; }
        public string SYN_ROWID { get; set; }
        public int isExpanded { get; set; }
        public int hasChildren { get; set; }
        public List<SanitationHygiene> SanitationHygieneList { get; set; }
        public List<SanitationHygiene> SanitationHygieneChildList { get; set; }
    }
    public class ChildModel
    {
        public string DEPARTMENT_EDESC { get; set; }
        public Dictionary<int, DayData> Days { get; set; } = new Dictionary<int, DayData>();
    }

    public class DayData
    {
        [Required]
        [StringLength(100, ErrorMessage = "Maximum 100 characters allowed")]
        public decimal? STANDARD { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Maximum 100 characters allowed")]
        public decimal? ACTUAL { get; set; }
        public decimal? GAP { get; set; }
    }
}
