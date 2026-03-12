using NeoErp.Core.Helpers;
using NeoErp.Sales.Modules.Services.Models.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Services.Contract
{
   public interface IContractService
    {
        List<ContractViewModel> GetAllContractInfo(filterOption model,string FincalYear);
    }
}
