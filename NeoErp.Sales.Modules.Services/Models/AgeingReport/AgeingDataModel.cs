using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.AgeingReport
{
    public class AgeingDataModel
    {
        public string SubCode { get; set; }

        public string Description { get; set; }

        public string VoucherDateString { get; set; }

        public DateTime VoucherDate { get; set; }

        public decimal? Amount { get; set; }

        public decimal MasterCode { get; set; }

        public decimal PreCode { get; set; }

        public int Code { get; set; }

        public string AccCode { get; set; }

    }
}
