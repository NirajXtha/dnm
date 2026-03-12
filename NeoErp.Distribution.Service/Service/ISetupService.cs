using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoErp.Core.Domain;
using NeoErp.Core.Models;
using NeoErp.Distribution.Service.Model;

namespace NeoErp.Distribution.Service.Service
{
    public interface ISetupService
    {
        #region freeQty
        List<dynamic> AllCustomer(string searchTerm = "");
        List<dynamic> GetFormSetup();
        List<dynamic> GetItemTreeView();
        dynamic SaveFreeQuantity(List<FreeQuantityModel> data, string customerCode, string formCode, string createdBy);
        List<dynamic> LoadFreeQuantityData(string customerCode, string formCode);
        dynamic ProcessExcelUpload(System.IO.Stream fileStream, string createdBy);
        #endregion freeQty
        IEnumerable<DistributorListModel> GetDistributorList(ReportFiltersModel model, User userInfo);
        IEnumerable<FarmListModel> GetFarmList(ReportFiltersModel model, User userInfo);
        IEnumerable<ResellerListModel> GetResellerList(string Source, ReportFiltersModel model, User userInfo,string status);
        int InactiveResellers(User userInfo);
        IEnumerable<OtherEntity> getAllEntityList(ReportFiltersModel model, User userInfo);
        IEnumerable<RouteListModel> GetRouteList(ReportFiltersModel model, User userInfo);
        IEnumerable<RouteListModel> GetBrandingRouteList(ReportFiltersModel model, User userInfo);
        string AddDistributor(DistributorListModel model, User userInfo);
        string UpdateDistributorOrder(OrderModel model);
        string deleteDistributor(DistributorListModel model,User userInfo);
        string UpdateDistributor(DistributorListModel model, User userInfo);
        string AddReseller(ResellerListModel model, User userInfo);
        string AddOtherEntity(OtherEntity model, User userInfo);
        string UpdateReseller(ResellerListModel model, User userInfo);
        string UpdateEntity(OtherEntity model,User userInfo);
        string deleteReseller(ResellerListModel model, User userInfo);
        string DeleteOtherEntity(OtherEntity model, User userInfo);
        string deleteArea(ResellerListModel model, User userInfo);
        ResellerListModel GetReseller(string Id, User userInfo);
        List<OtherEntity> GetIndividualEntity(string Code, User userInfo);
        //interface for dealer
        IEnumerable<DealerModal> GetDealerList(ReportFiltersModel modal, User userInfo);
        string AddDealer(DealerModal modal, User userInfo);
        string deleteDealer(DealerModal model, User userInfo);
        string UpdateDealer(DealerModal modal, User userInfo);
        List<AreaModel> GetDistrictList();
        List<AreaModel> GetvdcList(string DISTRICT_CODE);
        List<AreaModel> GetZoneList(string DISTRICT_CODE);
        List<AreaModel> GetRegionList(string DISTRICT_CODE);
        List<ResellerGroupModel> GetResellerGroups(User UserInfo);
        List<DistUserEmployeeModel> getLoginEmployee(User UserInfo);
        List<DistUserEmployeeModel> getDistLoginEmployee(User UserInfo);
        string AddResellerGroup(ResellerGroupModel model, User userInfo);
        string deleteGroup(ResellerGroupModel modal, User userInfo);
        

        string AddImageCategory(ImageCategoryModel model, User userInfo);
        string deleteImage(ImageCategoryModel model, User userInfo);
      //  string CreateClosingStock(ClosingStock model, User userInfo);
        string DeleteResellerGroup(int GroupId, User UserInfo);

       // string DeleteCategoryImage(int ImageId, User userInfo);


        List<ImageCategoryModel> GetCategoryImage(User UserInfo);
        List<AreaModel> GetAllAreaList();
        string AddArea(AreaModel modal, User userInfo);
        string UpdateArea(AreaModel modal, User userInfo);
        AreaModel GetAreaSaveObj(AreaModel model, User userInfo);

        string AddRoute(RouteListModel model, User userInfo);
        string UpdateRoute(RouteListModel model, User userInfo);
        string deleteRoute(RouteListModel model, User userInfo);

        List<OutletModel> getAllOutletList();

        string AddOutlet(OutletModel modal, User userInfo);

        string deleteItem(OutletModel modal, User userInfo);

        List<OutletSubtypeModel> getAllSubOutletList(int TYPE_ID, User userInfo);
        List<OutletSubtypeModel> getAllSubOutletList(User userInfo);
        string AddGeneralQuestions(QuestionSetupModel model, User UserInfo);
        string AddTabularQuestions(List<TabularModel> model, User UserInfo);    

        QuestionListModel GetAllQuestions(User UserInfo);

        string SaveSurvey(SurveyModel model, User userInfo);

        List<SurveyModel> GetSurveyList(User userInfo);

        QuestionSetupModel GetGeneralBySetID(string setId, User UserInfo);

        TabularModel GetTabularBySetID(string setId, User UserInfo);

        List<QuestionSetModel> GetAllQuestionSets(User userInfo);

        List<UserSetupTreeModel> GetUserSetupTreeList(User UserInfo);
        List<UserRoleModel> GetDistUserRole(User userInfo);
        string SaveUserTree(UserSetupTreeModel model, User userInfo);
        string UpdateUserTree(UserSetupTreeModel model, User userInfo);
        string DeleteUserTree(string code, User userInfo);
        string UpdateUserTreeOrder(UserSetupTreeModel model);
        string CreateClosingStock(ClosingStock model, User userInfo);
        string UpdateClosingStock(ClosingStock model, User userInfo);
        List<ClosingStock> GetClosingStock(User userInfo,string DistId);
        string SaveOpeningStockSetup(OpeningDetailModel model, User userInfo);
        List<OpeningDetailModel> GetOpeningStock(User userInfo);
        List<itemList> GetDistChildItems(string distCode, User userInfo);
        List<ResellerItemRate> GetResellerItems(User userInfo);
        string UpdateResellerItemRate(ResellerItemRate model, User userInfo);
        List<DistQueryBuilderModel> GetDistributorList();
        string SaveWidgets(CreateDistWidgetsModel modal, User userInfo);
        List<DistTableList> GetDistTableList();
        List<DistTableColumn> GetColumnNameList(string tablesName);
        List<NotificationModel> GetAllNotifications(User userInfo);
        string SaveNotification(NotificationModel model, User userInfo);
        int DeleteNotification(string id);
        List<string> GetAllFCMDevices(List<string> sp_codes);
        List<ItemModel> GetAllCompItems(User userInfo);
        string SaveCompItemMap(List<CompItemModel> model, User userInfo);
        List<CompItemModel> GetCompItemMap(User userInfo);

        List<CompetitorItemFields> GetCompFields(User userInfo);
        string SaveCompFields(CompetitorItemFields model, User userInfo);
        string DefaultCompFileds(User userInfo);
        //CompItem
        List<CompItemSetupModel> GetCompItem(User userInfo);
        List<UserSetupModel> GetUserList(User userInfo);
        List<UserAccessModel2> GetUserAcccessList(User userInfo, string id);
        bool setUserAccess(User userInfo, List<UserAccessModel2> models);
        List<string> GetCompCategories(User userInfo);
        string CreateCompItem(CompItemSetupModel model, User userInfo);
        string DeleteCompItem(int Id, User userInfo);
        string SaveGroupMap(List<GroupMapModel> model, User userInfo);
        string SaveDistUserMap(List<UserMapModel> model, User userInfo);
        string deleteUser(UserMapModel model, User userInfo);
        List<GroupMapModel> GetGroupMap(User userInfo);
        List<UserMapModel> GetDistUserMap(User userInfo);
        //EmployeeSetup
        List<CompanyModel> GetCompany(User userInfo);
        List<BranchModel> GetBranch(string COMPANY_CODE, User userInfo);
        List<EmployeeSetupModel> GetEmployee(User userInfo);
        string CreateEmployee(EmployeeSetupModel model, User userInfo);
        string DeleteEmployee(int Id, User userInfo);

        #region ResellerExcelSave
        ResellerListModel GetResellerSaveObj(ResellerListModel item, User userInfo);
        DistributorListModel GetDistributorSaveObj(DistributorListModel item, User userInfo);
        CompItemModel GetCompItemMapSaveObj(CompItemModel item, User userInfo);
        #endregion

        RouteListModel GetRouteSaveObj(RouteGroup model, User userInfo);

        #region Employee Setup

       #endregion
        List<CustomerModel> GetMuCodes();
        List<CategoryModel> GetCategoryCodes();
        List<ItemModel> GetItems(User userInfo);
        List<ItemModel> GetItemsList(User userInfo);
        int SaveItem(ItemModel model, User userInfo);

        List<WEB_QUESTION_ASNWER> GetWebQuestionAnswerList(User userInfo);
        List<ResellerDistributorModel> GetDistributorResellerList(User userInfo );

        List<DDL_TEMPLATE> GetMessageTemplateList(User userInfo);
        List<CustomerModel> GetCustomerByDealer(User userInfo);

        string AddScheme(SchemeModel model, User userInfo);

        List<SchemeModel> GetAllScheme(User userInfo);

        List<ItemModel> GetSchemeItem(string Id, User userInfo);
        List<SchemeGiftModel> GetGiftItem(string Id, User userInfo);
        List<SchemeDetailModel> GetOtherItem(string Id, User userInfo);

        int DeleteScheme(string Id);
        List<AreaModel> GetSchemeArea(string Id);
        List<EmployeeAreaModel> GetEmployeesandRoute(User userInfo);
        bool  ApproveScheme(string Id, string Action);
    }
}