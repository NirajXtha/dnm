using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Core.Models.CustomModels
{
    public class DateFilterModel
    {
        
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string RangeName { get; set; }

        public int SortOrder { get; set; }

        public string StartDateString { get; set; }

        public string EndDateString { get; set; }
    }
}
