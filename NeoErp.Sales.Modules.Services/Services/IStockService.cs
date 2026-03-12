using NeoErp.Core.Domain;
using NeoErp.Core.Helpers;
using NeoErp.Core.Models;
using NeoErp.Sales.Modules.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NeoErp.Sales.Modules.Services.Services
{
    public interface IStockService
    {
        IList<LocationWiseStockModel> GetLocationWiseStockReport(ReportFiltersModel model, User userInfo);
        IList<LocationWiseItemStockModel> GetLocationWiseStock(ReportFiltersModel model, User userInfo);

        IList<LocationsHeader> GetLocationHeader(ReportFiltersModel model,User userinfo);
        

    }
}
