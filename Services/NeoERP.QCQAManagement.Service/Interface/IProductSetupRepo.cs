using NeoERP.QCQAManagement.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QCQAManagement.Service.Interface
{
    public interface IProductSetupRepo
    {
        List<ItemDetails> GetItemDetails();
        List<ProductSetup> GetProductTypeLists();
        List<ItemSetup> GetProductLists(string ProductType,string Category_Code);
        List<ProductDetails> GetParameterList();
        bool InsertProductDetails(ProductDetails data);
        ProductDetails GetProductById(string id);
        bool DeleteProductSetupById(string id);
    }
}
