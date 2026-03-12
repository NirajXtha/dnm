using NeoErp.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.AgeingReport
{
    public class AgeingFilterWrapperModel: filterOption
    {
        public AgeingFilterModel AgeingFilter { get; set; }
    }

    public class AgeingFilterModel:filterOption
    {
        public AgeingFilterModel()
        {
            Codes = new List<string>();
        }
        public int? FrequencyInDay { get; set; } = (int)FrequencyDayFilter.SevenDay;

        public int? FixedInDay { get; set; } = (int)FixedDayFilter.ThirtyDay;

        public string AsOnDate { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        
        public int? Id { get; set; }

        public string Type { get; set; } = Enum.GetName(typeof(AgeingReportType), AgeingReportType.Customer);

        public List<string> Codes { get; set; }

        public List<string> Area { get; set; }

        public List<string> Branchs { get; set; }

        public string BillWiseOrLedgerWise { get; set; } = "LedgerWise";
        public string ShowGroupWise { get; set; } = "false";

        public string CustomerDealerWise { get; set; } = "Customer";

        public string DealerGroup { get; set; } = "0";

    }

    public class AgeingGroupData
    {
        public String Description { get; set; }

        public int PreCode { get; set; }

        public int MasterCode { get; set; }

        public int Code { get; set; }
        public string MasterCodeWithoutReplace { get; set; }
        public string PreCodeWithoutReplace { get; set; }
    }
    public class AgeingGroupDataNCR
    {
        public String Description { get; set; }

        public string PreCode { get; set; }

        public string MasterCode { get; set; }

        public string Code { get; set; }
        public string MasterCodeWithoutReplace { get; set; }
        public string PreCodeWithoutReplace { get; set; }

        public string group_sku_flag { get; set; }
    }
}
