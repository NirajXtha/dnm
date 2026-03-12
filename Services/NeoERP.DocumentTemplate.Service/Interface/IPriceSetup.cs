using NeoERP.DocumentTemplate.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Interface
{
    public interface IPriceSetup
    {
         List<PriceSetupModel> ListAllItemWithName();

        List<PriceSetupModel> GetItemByCompany(string selectdCompany);

        string SaveUpdatedCell(SaveModelForPriceSetup saveModel);

        List<string> GetAllSavedItemsName();

        Tuple<List<PriceSetupModel>,MasterField> GetItemToEdit(string selectedPriceName);

    }
}
