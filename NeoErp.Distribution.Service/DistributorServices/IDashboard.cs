using NeoErp.Core.Domain;
using NeoErp.Distribution.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.DistributorServices
{
    public interface IDashboard
    {
        List<DistributorTragetChartModel> GetDistributorTraget( User userInfo, string customerCode = "0");
        List<DashBoardWidgets> GetDashBoardWidgets(User userInfo);
        List<DistMatrixModel> GetDistMatrics(bool ispermission = false);

    }
}
