using NeoErp.Core.Models;
using NeoErp.Sales.Modules.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Services
{
    public interface INewAgeingReport
    {
        dynamic ageingTransactions(TransactionRequestModel model, NeoErpCoreEntity dbContext);
    }
}
