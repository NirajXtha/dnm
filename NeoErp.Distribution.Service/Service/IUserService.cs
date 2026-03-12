using NeoErp.Core;
using NeoErp.Core.Domain;
using NeoErp.Distribution;
using NeoErp.Distribution.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Service
{
   public interface IUserService
    {
        List<UserSetupModel> GetAllUserList(User userInfo);
        string GetSPCodeByCustomerCode(string SP_CODE,string USER_TYPE, int USERID,User userInfo);
        List<EmployeeModel> getEmployeeList();
        List<EmployeeModel> getUserEmployee(User userEmployee);
        List<EmployeeModel> GetSalesPersonList(User userInfo);
        List<DistributorModel> getDistributorList(User userInfo);
        List<NewPartyTypeModel> GetPartyType(User userInfo);
        List<ResellerModel> getResellerList(User userInfo);
        List<SalesPersonModel> getSalesPersonList(User userInfo);
        List<ItemModel> GetItemAndBrand(User userInfo);
        string insertUserDetails(UserSetupModel modal,User userInfo);
        //string UpdateUserAssign(UserSetupModel modal);
        List<UserSetupModel> UpdateUserAssign(UserSetupModel modal, User userInfo);
        List<UserSetupModel> GetAllAssignList(UserSetupModel modal);
        string UpdatePreferenceSetup(PreferenceSetupModel modal, User userInfo);
        PreferenceSetupModel GetPreferenceSetup(User userInfo);

        List<MobileRegModel> GetAllDevices(User userInfo);

        string UpdateDevice(MobileRegModel model, User userInfo);

        string DeleteDevice(MobileRegModel model);
    }
}
