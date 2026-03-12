using NeoErp.Core.Models;
using NeoErp.Planning.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Planning.Service.Interface
{
    public interface IChartReport
    {
        List<PlanReportModel> getMonthlySalesPlanChart(ReportFiltersModel filters, string dateFormat);
    }
}
