using NeoErp.Models;
using NeoErp.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Services
{
     public interface IUserChartPermission
    {
        bool SaveUserChartPermission(UserChartModel userChartPermission);
        UserChartPermissionModel GetUserChartPermission(string name, string type);
        List<UserListModel> GetLoginUserList();
        List<EmployeeSchedular> GetEmployeesListForScheduler();
        List<EmployeeSchedular> GetEmployeesList();
        List<Customers> GetAllCustomers();
        List<Dropdownsmodel> GetAllItems();
        List<Dropdownsmodel> GetAllSuppliers();
        List<Dropdownsmodel> GetAllDivision();
        List<Dropdownsmodel> GetAllLedgers();
        int createEntities(string systemName);
        List<Customers> GetAllCustomersWithOutlet();
    }
}
