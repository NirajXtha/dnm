using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Core.Services
{
   public interface IDashBoardMetricService
    {
        List<MetricWidgetsModel> GetMetricList(bool ispermission = false);
        List<MeticorderModel> GetDashboard(string reportname);
        List<MetricWidgetsModel> GetMetricListWithModuleCode(bool ispermission = false, String MetricName = "01");
    }
}
