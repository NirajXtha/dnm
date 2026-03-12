using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Services
{
    public interface IAgeingFactory
    {
        IAgeingReportDataService GetAgeingDataService(AgeingReportType type);
    }
}
