using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoErp.Sales.Modules.Services.Models;

namespace NeoErp.Sales.Modules.Services.Services
{
    public interface IFreeQty
    {
        List<dynamic> AllCustomer(string searchTerm = "");
        List<dynamic> GetFormSetup();
        List<dynamic> GetItemTreeView();
        dynamic SaveFreeQuantity(List<FreeQuantityItem> data, string customerCode, string formCode, string createdBy);
        List<dynamic> LoadFreeQuantityData(string customerCode, string formCode);
        dynamic ProcessExcelUpload(System.IO.Stream fileStream, string createdBy);
    }
}
