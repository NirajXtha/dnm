using NeoErp.Core.Domain;
using NeoErp.Core.Models;
using NeoErp.Sales.Modules.Services.Models.Production;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Services.Production
{
    public interface IProductionService
    {
        List<ProductionRegisterModel> GetProductionRegister(ReportFiltersModel filters, User userInfo);
        IList<ProductionStockInOutSummaryModel> GetProductionStockInOutSummary(ReportFiltersModel filters, User userInfo);
    }
}
