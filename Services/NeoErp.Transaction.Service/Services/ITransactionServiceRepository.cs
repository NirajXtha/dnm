using NeoErp.Transaction.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Transaction.Service.Services
{
    public interface ITransactionServiceRepository
    {
        //IEnumerable<Consumption> GetConsumptionSetup();
        Consumption GetConsumptionSetup();
        List<Employee> GetAllEmployeeSetup(string filter);
        List<Customers> GetAllCustomerSetup(string filter);
        List<Department> GetAllDepartmentSetup(string filter);
        List<Location> GetAllLocationSetup(string filter);
        List<Products> GetAllProducts(string filter);
        List<CostCenter> GetAllCostCenter(string filter);
        void SaveConsumptionIssue(ConsumptionIssue issue);
    }
}
