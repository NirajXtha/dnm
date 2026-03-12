using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoErp.LOC.Services.Models;

namespace NeoErp.LOC.Services.Services
{
    public interface ILOCService
    {
              
        List<ItemDetails> GetAllItemsByOrderCode(string OrderCode);
        List<CountryModels> GetAllCountry(string filter);
        List<HSModels> GetAllHsCodes(string filter);
        List<BeneficiaryModels> GetAllBeneficiary(string filter);
        List<SupplierModel> GetAllSuppliersByFilter(string filter);
    }
}
