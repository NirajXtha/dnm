using NeoErp.LOC.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.LOC.Services.Services
{
    public interface IDashBoardMetricService
    {
        List<MetricWidgetsModel> GetMetricList();
    }
}
