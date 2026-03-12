using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Pos.Services.Services
{
    public interface IPosService
    {
        List<ProductViewModel> GetAllProducts();

        CustomerViewModel CreateCustomers(CustomerViewModel customerdetail);
        List<ProductViewModel> GetProductsByValue(string value);

        List<CustomerViewModel> GetAllCustomers(string filter);
    }
}
