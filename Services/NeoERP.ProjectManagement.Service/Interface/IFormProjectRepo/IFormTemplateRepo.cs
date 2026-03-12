using NeoErp.Core.Domain;
using NeoERP.ProjectManagement.Service.Models;
using System.Collections.Generic;

namespace NeoERP.ProjectManagement.Service.Interface
{
    public interface IFormTemplateRepo
    {
        List<FormDetailSetup> GetFormDetailSetup(string formCode);
        List<DraftFormModel> GetDraftFormDetailSetup(string formCode);
        List<Customers> GetAllCustomerSetup(string filter);
        List<Suppliers> getALLSupplierListByFlter(string filter);
        List<IssueType> getAllIssueTypeListByFilter(string filter);

        List<Department> GetAllDepartmentSetup(string filter);
        List<Employee> GetAllEmployeeSetup(string filter);
        List<Location> GetAllLocationSetup(string filter);
        List<Products> GetAllProducts(string filter);
        List<CostCenter> GetAllCostCenter(string filter);
        List<Currency> getCurrencyListByFlter(string filter);
        List<Brand> getBrandListByFlter(string filter);
        List<Division> GetAllDivisionSetup(string filter);
        List<Priority> GetAllPrioritySetup(string filter);
        List<SalesType> GetAllSalesTypeSetup(string filter);
        string SaveFormData(string formcode, string columnname, string colunmvalue);
        string NewVoucherNo(string companycode, string formcode, string transactiondate, string tablename);

        List<FormCustomSetup> GetFormCustomSetup(string formCode, string voucherno);

        List<Products> GetProductDataByProductCode(string productcode);

        List<MuCodeModel> GetMuCode();

        List<COMMON_COLUMN> GetSalesOrderFormDetail(string formCode, string orderno);
        List<DraftFormModel> getDraftDataByFormCodeAndTempCode(string formCode, string tempCode);
        decimal GetGrandTotalByVoucherNo(string voucherno, string formcode);

        List<AccountSetup> getALLAccountSetupByFlter(string filter);
        List<CategoryModel> GetAllItemCategoryFilter(string filter);
        List<MuCodeModel> GetAllIndexMuFilter(string filter);

        List<BudgetCenter> GetAllBudgetCenterByFilter(string filter, string accCode);
        List<Branch> GetAllBranchCodeByFilter(string filter);
        List<Employee> GetAllEmployeeCodeByFilter(string filter);
        List<SalesType> GetAllSaleTypeListByFilter(string filter);
        List<BudgetCenter> GetAllBudgetCenterForLocationByFilter(string filter);
        List<PartyType> GetAllPartyTypeByFilter(string filter);
        List<AreaSetup> GetAllAreaSetupByFilter(string filter);
        List<SubLedger> GetAllSubLedgerByFilter(string filter, string subCode);
        List<SubLedger> GetAllSubLedgerByFilterPartyType(string filter, string partyTypeCode);

        List<BudgetCenter> getBudgetCodeByAccCode(string accCode);

        List<BudgetCenter> checkBudgetFlagAccessByLocationCode(string locationCode);

        string getSubledgerCodeByAccCode(string accCode);
        List<SubLedger> GetSubLedgerByAccountCode(string accountcode);
        List<ChargeSetup> GetChargeCodebyFormCode(string formcode);
        List<COMMON_COLUMN> getReferenceGridData(REFERENCE_MODEL model);

        List<COMMON_COLUMN> VoucherDetailByReferenceForTemplate(VoucherRefrence model);

        string NewTransactionNo(string companycode, string formcode, string transactiondate);
        string getBudgetCodeCountByAccCode(string accCode);

        List<MenuModels> GetAllMenuItems();
        List<MenuModels> GetAllInventoryMenuItems();
        List<MenuModels> GetAllSetupMenuItems();
        List<DocumentType> getDocumentTypeListByFlter(string filter);

        List<MenuModels> GetAllProductionMeneItems();


        List<MenuModels> GetAllSalesMenuItems();

        #region Voucher Transaction
        string GetNewSequence();
        #endregion

        List<DocumentSubMenu> GetMaseterTransDetailByFormCode(string formCode, string docVer = "all");
        List<DocumentSubMenu> GetDraftDetails(string formCode);
        List<FormDetailSetup> GetDistFormTransDetailByModuleCode(string modulecode);

        List<TemplateDraftModel> GetDraftList(string moduleCode, string formCode);
        string GetPrimaryDateByTableName(string tablename);
        string GetPrimaryColumnByTableName(string tablename);
        string InsertSalesImage(DocumentTransaction documentdetail);
        int? DocumentIfExists(string voucherno, string formcode);
        List<ChargeOnSales> GetChargesData(string formCode, string voucherNo);
        List<ChargeOnSales> GetChargesData(string formCode);
        List<CustomerModels> getAllCustomer();
        List<SupplierModels> getAllSupplier();
        List<ProductsModels> getAllProduct();

        List<LocationModels> getAllLocation();
        List<RegionalModels> getAllRegions();
        List<ResourceModels> getAllResource();
        List<LocationTypeModels> getAllLocationType();
        List<BranchModels> getAllBranch();
        List<DealerModel> getAllDealer();
        List<FormcodeList> getAllfr();
        List<CustomerModels> GetCustomerListByCustomerCode(string customercode, string customerMasterCode, string searchText);
        List<SupplierModels> GetSupplierListBySupplierCode(string suppliercode, string supplierMasterCode, string searchText);
        List<PModels> GetProductListByItemCode(string itemcode, string itemMasterCode, string searchText);
        //List<ExpandoObject> GetSalesOrderFormDetail1(string formCode, string orderno);
        List<LocationModels> GetLocationListByLocationCode(string locationId, string locationCode, string searchText);
        List<BranchModels> GetBranchListByBranchCode(string branchId, string branchCode, string searchText);


        //List<ExpandoObject> GetSalesOrderFormDetail1(string formCode, string orderno);

        //subin changes
        List<AccountCodeModels> getAllAccountCode();
        List<AccountCodeModels> getAllAccountCodeWithChild(string filter);
        List<AccountCodeModels> getAllAccountComboCodeWithChild(string filter);
        //division Under group
        List<DivisionModels> getAllDivisionCode();
        List<ResourceCodeModels> getAllResourceCode();
        List<AreaModels> getAllArea();

        List<AgentModels> getAllAgent();
        List<TransporterModels> getAllTransporter();
        List<AccountCodeModels> GetAccountListByAccountCode(string acccode, string accMasterCode, string searchText);
        List<EmployeeCodeModels> GetEmployeeListByEmployeeCode(string employeecode, string employeeMasterCode, string searchText);
        List<EmployeeCodeModels> getAllEmployee();
        List<DivisionModels> getAllDivision();
        List<DivisionModels> GetdivisionListBydivisionCode(string divisioncode, string divisionMasterCode, string searchText);

        List<FormDetails> getRefrenceOrderNo(string formcode, string filter, string Table_name);

        List<FORM_SETUP_REFERENCE> GetRefrenceFlag(string formcode);
        List<FormSetup> getAllForms();
        List<ModuleModels> getAllModules();
        List<FormSetup> getAllFormsListAccordingToModule(string ModuleCode);


        //List<RefrenceType> getRefrence(string FormCode);
        List<TemplateType> getTemplate(string FormCode);

        List<SubLedger> GetSubLedger();

        List<ChargeCode> GetChargeCode();

        List<CategoryModel> GetCategoryCode();
        List<Customers> getALLSupplierListByFlterForReference(string filter);

        List<BudgetCenterCodeModels> getAllBudgetCenterCode();


        List<ProcessModels> getAllProcess();
        List<Location> GetAllLocation();
        List<PartyType> GetAllPartyType();
        List<PartyRating> GetAllPartyRating();
        List<AccountCodeModels> getAllAccounts();
        //subin changes for dynamic templates name
        List<TemplateType> getTemplates(string FormCode, string tablename);


        List<ApplicationUser> getALLUserListByFlter(string filter);

        List<TemplateDraftModel> GetDraftListFinance();
        List<TemplateDraftModel> GetDraftListSales();
        List<TemplateDraftModel> GetDraftListProduction();
        List<TemplateDraftModel> GetDraftListInventory();


        List<MenuModels> GetAllPlanningMenuItems();
        List<MenuModels> GetAllPlanningMenuItemsByPreMenuCode(string premenucode);
        List<MenuModels> GetAllMenuItems(string modulecode);
        List<MenuModels> GetAllMenuItemsByPreMenuCode(string premenucode, string modulecode);
        //string GetAccCodeByPTCode(string ptcode);
        //List<DynamicMenu> GetDynamicMenu(string modular_code);
        ////List<DynamicMenuForAllModule> GetDynamicMenu();
        ////List<DynamicMenuForAllModule> GetProjectManagementDynamicMenu();
        //List<DynamicMenu> GetChlidMenu(string modular_code);


        List<WebDesktopFolder> GetFoldertByUserId();
        void UpdateOrderNoByFolderId(List<FOLDER_ORDER> fOLDER_ORDER);
        WebDesktopFolder AddNewFolder(string FOLDER, string FOLDER_COLOR, string ICON);
        List<WebDesktopManagement> AddWebDesktopManagement(WebDesktopManagement webDesktopManagement);
        List<WebDesktopManagement> GetFolderTemplateByUserId();
        bool MenuNoExistsOrNot(string MENU_NO);
        bool FolderExistsOrNot(string FOLDER_NAME);
        bool FormNoExistsOrNot(string FORM_CODE);
        bool DraftTemplateExistsOrNot(string TEMPLATE_CODE);
        void RemoveFolderByFolderId(string FOLDER_ID);
        void ResetWebManagement();
        ChargeOnItem GetInvItemChargesData(string formCode, string itemCode);

        List<ChargeOnSales> GetInvChargesDataSavedvaluewise(string voucherNo, string itemcode);
        List<ChargeOnSales> GetInvChargesDataSavedQuantityWise(string voucherNo, string itemcode);
        List<PaymentMode> getPaymentModeListByFlter(string filter);
        string CheckIsTDSByAccCode(string accCode);
        List<TDSCODE> getALLTDSByFlter(string filter);
        string CheckIsVATByAccCode(string accCode);
        List<VECHILES> GetAllVechDetailsByFilter(string filter);
        List<PartyType> GetAllPartyTypeByFilterAndCustomerCode(string filter, string customercode);
        decimal GetSatanderedRateByFilters(string customercode = "", string fromcode = "", string areacode = "", string itemcode = "");
        List<TRANSPORTER> GetAllTransporterDetailsByFilter(string filter);
        List<CITY> GetAllCityDetailsByFilter(string filter);
        List<ShippingDetailsViewModel> GetShippingData(string formCode, string voucherNo);
        decimal GetStockQuantity(string itemcodecode, string voucherdate, string locationcode);

        List<SubLedger> GetAllSubCodeByFilter(string filter);
        string GetPrintTemplateByFormCode(string formcode);
        List<PriceList> GetAllPriceListByFilterAndCustomerCode(string filter, string customercode);
        decimal GetItemRateByMasterId(string masterid = "", string itemcode = "");
        void UpdatePrintCount(string voucherno, string formcode);
        int GetPrintCountByVoucherNo(string VoucherNo, string formcode, string UpdatePrintCountFlag);
        decimal GetRefGrandTotalByVoucherNo(string voucherno);
        string GetcustomerNameByCode(string code);
        string GetItemNameByCode(string code);
        List<Agent> GetAllAgentSetup(string filter);
        string GetAccNameByCode(string code);
        List<RefrenceType> getRefrence(string FormCode, bool ShowDocumentType = false);
        CodeForLog CodeForLog();
        string DeleteUploadedFile(DropZoneFile model);
        COMMON_COLUMN GetReferenceNoByOrderNo(string orderno);

        string GetParytTypeNameByCode(string code);
        List<REFERENCE_DETAILS> GetReference_Details_For_VoucherNo(string VoucherNo, string formcode);
        List<DocumentSubMenu> GetMaseterTransDetailByFormCodeVer(string formCode, string docVer);
        void UpdateMasterTranasactionForVerification(string orderNo, string formcode, string mode, out string message, out bool status);
        void BulkUpdateMasterTranasactionForVerification(List<string> voucherNo, string formcode, string mode, out string message, out bool status);
        List<Customers> GetCustomerDetail(string filter);
        GuestInfoFromMaterTransaction GetGuestInfoFromMasterTransaction(string formCode, string orderno);
        List<FormDetailSetup> getSalesVerificationFormcodeWise(string moduleCode, string docVer);
        List<string> getSalesVerificationUserWise();
        string GetBudgetNameByCode(string code);
        List<PartyType> GetAllPartyTypes();
        List<AccountSetup> getALLAccountForInvBudgetTrans();
        List<ShippingDetailsViewModel> GetShippingDataByVoucherNo(string voucherNo);
        CompanyInfo GetCompanyInfo();
        bool SaveInvoiceData(List<SalesInvoiceExcel> invData);

        bool ItemNoExistsOrNot(string itemcode);

        bool BatchItemNoExistsOrNot(string itemcode);

        List<BATCHTRANSACTIONDATA> GetbatchdetailByItemCodeAndLocCode(string itemcode, string loactioncode);

        List<BATCHTRANSACTIONDATA> GetbatchdetailByItemCodeAndLocCodeforedit(string itemcode, string loactioncode, string voucherno);
        List<COMMON_COLUMN> GetProductionFormDetail(string formCode, string TableName, string RoutingCode, decimal ProductQty);
        bool BatchWiseItemCheck(string itemcode);
        List<BATCHTRANSACTIONDATA> GetbatchTranDataByItemCodeAndLocCode(string itemcode, string loactioncode, string refernceNo = null);

        List<LoadingSlipModalForPrint> GetLoadingSlipListByReferenceoNo(string referenceno);

        string GetLoactionNameByCode(string code);


        List<SchemeModels> getAllScheme();

        List<Document> getAllDocument();
        List<PartyType> GetAllDealer();

        string getusertype();

        List<CustomerModels> getAllSchemeCustomer();

        List<CustomerModels> getSchemeCustomerByCodes(string code);

        List<ProductsModels> getAllProductforScheme();

        List<ProductsModels> getProductforSchemeByCode(string code);

        List<PartyType> GetDealerForSchemeByCode(string code);

        List<SchemeModels> getAllManualScheme(string status, string from, string to);
        List<SchemeDetailsModel> getSchemeDetailGridData(SCHEME_MODEL model);

        List<BranchModels> getAllBranchforscheme();

        List<PartyType> GetAllPartyTypeByFilterAndSubCode(string filter, string Subcode);

        string GetPartyTypeNameByCode(string code);
        List<Document> GetDocumentForSchemeByCode(string code);

        List<BranchModels> GetBranchForSchemeByCode(string code);

        List<AreaSetup> GetAreaForSchemeByCode(string code);
        string getLoginedUser();

        string GetcustomerCodeByName(string name);
        List<AccountCodeModels> getAccountCodeByCode(string acccode);

        List<SchemeDetailsModel> getSchemeDetailFormImpact(SCHEME_MODEL model);

        List<SchemeModels> getAllSchemenotimplemented();

        List<AccountCodeModels> getAllAccountCodeForVs(string filter);

        List<AccountCodeModels> getAllAccountSupp();

        List<CustomersTreeModel> getAllCustomerWithChildren();

        List<CustomerSetupModels> CustomerListAllNodes(User userinfo);

        List<CustomerSetupModels> GetCustomerListByCustomerCode(string level, string masterCustomerCode, User userinfo);

        string CheckVoucherNoReferenced(string voucherno);

        bool deletevouchernoFinance(string tablename, string formcode, string voucherno, string primarycolumnname);

        bool CheckVoucherNoPosted(string voucherno);
        List<CompanyInfo> GetCompanyList();

        List<Document> getDocumentByFilter(string filter);
        List<AccountSetup> getALLAccountGroupForIntrestCalc();

        List<CustomerModels> getAllInterestCalcCustomers(string codes);

        bool InsertProjectData(Project projectData);
        List<Project> ListAllProjects();
        bool DeleteProjects(int id);

        List<Project> GetProjectById(int id);

        List<Employee> GetAllEmployees();
        List<ProjectCount> GetProjectCount();
        List<ProjectCount> CountBarGraph();
        List<SubProjectData> GetCompletedProject();
        List<SubProjectData> GetStartedProject();
        List<SubProjectData> GetSubProjectList();
        List<EntryReport> GetConsumptionTransDetailByFormCode(string formCode, string docVer = "all");
        List<EntryReport> GetRequisitionTransDetailByFormCode(string formCode, string docVer = "all");
        List<EntryReport> GetRequisitionPendingTransByFormCode(string formCode, string docVer = "all");
        List<EntryReport> GetPurchaseTransByFormCode(string formCode, string docVer = "all");

    }
}
