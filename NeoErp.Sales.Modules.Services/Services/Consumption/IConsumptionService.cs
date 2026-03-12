using NeoErp.Core.Domain;
using NeoErp.Core.Models;
using NeoErp.Sales.Modules.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Services.Consumption
{
    public interface IConsumptionService
    {
        IEnumerable<ConsumptionIssueRegisterDetailModel> GetConsumptionIssueRegister(ReportFiltersModel reportFilters, User userInfo, bool liveData);
    }
}
