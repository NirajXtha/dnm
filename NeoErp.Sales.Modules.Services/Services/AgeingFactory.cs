using Autofac.Features.Indexed;
using NeoErp.Core.Caching;
using NeoErp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Services
{
    public class AgeingFactory:IAgeingFactory
    {
        IIndex<AgeingReportType, IAgeingReportDataService> _ageingType;

        public AgeingFactory(IIndex<AgeingReportType, IAgeingReportDataService> ageingType)
        {
            this._ageingType = ageingType;
        }

        public IAgeingReportDataService GetAgeingDataService(AgeingReportType type)
        {
            return this._ageingType[type];
        }
    }
}
