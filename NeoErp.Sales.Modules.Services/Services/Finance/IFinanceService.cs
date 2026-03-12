using NeoErp.Core.Domain;
using NeoErp.Core.Models;
using NeoErp.Sales.Modules.Services.Models.FinanceReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Services.Finance
{
    public interface IFinanceService
    {
        IList<MovementAnalysisModel> GetMovementAnalysis(ReportFiltersModel model, User userInfo);
    }
}
