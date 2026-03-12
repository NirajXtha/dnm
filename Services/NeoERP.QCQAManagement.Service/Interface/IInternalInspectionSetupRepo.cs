using NeoERP.QCQAManagement.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QCQAManagement.Service.Interface
{
    public interface IInternalInspectionSetupRepo
    {
        List<ProductDetails> GetProductDetails();
        List<ItemSetup> GetItemLists();
        bool InsertInternalInspectionDetails(ProductDetails data);
        ProductDetails GetInternalInspectionSetupById(string id);
        bool DeleteInternalInspectionSetupById(string id);
    }
}
