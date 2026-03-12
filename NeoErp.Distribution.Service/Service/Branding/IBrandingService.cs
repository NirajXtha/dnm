using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoErp.Distribution.Service.Model.BrandingModule;
using NeoErp.Core.Domain;
using NeoErp.Core.Models;

namespace NeoErp.Distribution.Service.Service.Branding
{
    public interface IBrandingService
    {
        string saveBrandingActivity(ActivityModel model, User userInfo);
        string UpdateBrandingActivity(ActivityModel model, User userInfo);
        List<ActivityModel> getBrandingActivityList(User userInfo);
        string deleteActivity(ActivityModel model, User userInfo);
        List<ContractModel> getAllContractList(string type, User userInfo);
        List<SupplierModel> GetSupplierList(User userInfo);
        List<SetQstModel> GetQuestionList(User userinfo);
        List<CustomerModel> GetCustomerList(User userInfo);
        List<AreaModel> GetAreaList(User userInfo);
        List<ResellerModel> GetBrdReseller(User userInfo);
        List<BrdItemModel> GetBrdItem(User userInfo);
        List<ItemUnitModel> GetItemUnit(User userInfo);
        string saveContract(ContractModel modal, User userInfo);
        string deleteContract(ContractModel modal, User userInfo);
        string updateContract(ContractModel modal, User userInfo);
        List<ContractModel> getContractSummary(User userInfo);
        List<BrandTypeModel> GetBrandType(User usrInfo);
        string SaveContractAnswers(GeneralSaveModel model, User userInfo);
        List<QstReportModel> ContractQueReport(User userInfo);
        List<SchemeReportModel> GetAllSchemeList(ReportFiltersModel model,User UserInfo);
        ContractModel GetContractSaveObj(ContactImportModel model, User userInfo);
        string saveUserSurveyReportWeb(List<WebQueAnsModel> list, WebQueAnsCommonModel common, User userInfo);
    }
}
