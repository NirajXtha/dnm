using NeoErp.Core.Models;
using NeoErp.Data;
using NeoErp.Sales.Modules.Services.Models.AgeingReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Services
{
    public interface IAgeingReportService
    {
        IEnumerable<AgeingColumnRange> GenerateColumns(int frequency, int fixedDay, string asOnDate);
        IEnumerable<AgeingColumnRange> GenerateColumns(int frequency, int fixedDay, string asOnDate, IFormatProvider provider);
        IEnumerable<AgeingDataViewModel> GetAgeingReport(AgeingFilterModel afModel);
        IEnumerable<AgeingDataViewModel.AgeingColumnRangeData> GetAgeingChartReport(AgeingFilterModel afModel);
        IEnumerable<AgeingDataViewModel.AgeingColumnRangeData> GetAgeingChartReportBranchWise(AgeingFilterModel afModel);
        IEnumerable<AgeingDataViewModel.AgeingColumnRangeData> GetAgeingDivisionChartReport(AgeingFilterModel afModel);

        IEnumerable<AgeingDataViewModel> GetMobileAgeingReport(AgeingFilterModel afModel, string customerCode, string companyCode);
        IEnumerable<AgeingDataViewModel.AgeingColumnRangeData> GetMobileAgeingChartReport(AgeingFilterModel afModel, List<string> customerCode, string companyCode);
        IEnumerable<AgeingDataViewModel.AgeingColumnRangeData> testAgeingData(AgeingFilterModel afModel, List<string> customerCode, string companyCode);
    }
}
