using System.Collections.Generic;
using NeoErp.Planning.Services.Models;

namespace NeoErp.Planning.Service.Interface
{
    public interface IDashBoardMetricService
    {
        List<MetricWidgetsModel> GetMetricList();
        List<string> GetDashboard(string reportname);
    }
}
