using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.AgeingReport
{
    public class AgeingColumnRange
    {
        public string ColumnRange { get; set; }
        public int orderBy { get; set; }

        public string FromDateString { get; set; }

        public string ToDateString { get; set; } = string.Empty;

        public DateTime? FromDate { get; set; }

        public DateTime ToDate { get; set; }

    }
}
