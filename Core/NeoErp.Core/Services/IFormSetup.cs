using NeoErp.Core.Models;
using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Core.Services
{
    public interface IFormSetup
    {
        List<FormTreeStructureModel> GetFormTreeStructureList(string moduleId = "");
        List<FormSetupResultModel> GetFormListByPreFormCode(string preFormCode, string moduleId = "");
        List<AllFormModel> GetMasterFormList();
        int GetNextFormCode();
        List<ModuleCodeAndDetailModel> GetAllModuleList();
        List<BranchCodeDtlModel> GetAllBranchList(string formCode);
        bool InsertFormGroupItem(FormSetupGroupEntryModel model);
        bool UpdateFormGroupItem(FormSetupGroupEntryModel model);
        FormSetupAddOrDuplicateStepModel GetFormSetupClone(string formCode);
        FormSetupAddOrDuplicateStepModel GetForEditFormSetupData(string formCode, bool isDuplicate = false);
        bool UpdateFormSetupGridItem(FormSetupAddOrDuplicateStepModel model);
        List<TransactionTableListModel> FormattingTab_GetTransactionTableList();
        FormSetupFormatingModel FormattingTab_GetEditData(string form_code);
        List<SubLedgerListModel> FormattingTab_GetSubLedgerList(string searchText);
        List<FormattingFormDetailSetupModel> GetFormDetailSetupList(string formCode);
        List<string> FormattingTab_GetUnmappedColumnList(string formCode);
        bool UpdateFormSetupFormattingTabInfo(FormSetupFormattingTabModel model);
        List<CustomerLookupDataModel> GetCustomersList();
        List<ItemMasterLookupModel> GetItemMasterList();
        List<EmployeeLookupDataModel> GetEmployeeList();
        List<CurrencyLookupModel> GetCurrencyList();
        List<MULookupModel> GetMUList();
        List<PriorityLookupModel> GetPriorityList();
        List<SalesTypeLookupModel> GetSalesTypeList();
        string GetCurrencyDesc(string code);
        List<CustomerTreeNode> GetCustomerTreeStructureList();
        List<CustomerDetailWIthIDModel> GetCustomerListByPreCode(string preCustomerCode);
        CustomerDetailModel GetCustomerDetailByCode(string customerCode);
        List<CustomerDetailWIthIDModel> GetCustomerListBySearch(string searchKey);


        List<ItemGroupedTreeNode> GetItemGroupList();
        List<ItemDetailWithIDModel> GetItemListByPreCode(string preItemCode);
        ItemDetailModel GetItemDetailByItemCode(string itemCode);
        List<EmployeeGroupedTreeNode> GetEmployeeGroupedTreeData();
        List<EmployeeDetailWithIDModel> GetEmployeeListByPreCode(string preEmployeeCode);





        FormSetupReferenceTabInfoModel GetReferenceTabData(string formCode);
        List<TransactionTableListModel> GetTransactionTableListWithoutVoucher();
        object GetQuotationFormList(string table_name);
        bool UpdateFormReferenceTabData(FormSetupReferenceTabInfoModel modelData);
        List<BranchListModel> GetBranchListByFormCode(string formCode);
        object GetFormListForInvoiceToBeMatched();
        List<AccountListModel> GetAccountList(string searchText = "");




        FormSetupNumberingTabInfoModel GetNumberingTabData(string formCode);
        bool UpdateFormNumberingTabData(FormSetupNumberingTabInfoModel modelData);
        List<AccountInfoModel> ChargeSetupGetAccountList(string searchText);
        List<ChargeCodeModel> GetChargeCodeList(string form_code);
        ChargeSetupDataModel GetChargeSetupData(string formCode, string chargeCode);
        bool UpdateChargeSetupData(ChargeSetupDataModel model);

        object GetQualityCheckTabData(string form_code = "");

        bool UpdateChargeSetupData(QualityCheckGetDataModel model);
        object GetMiscellaneousTabData(string form_code);
        object GetDocumentReportSetupData(string formCode, string reportSearchText = "");
        bool UpdateMiscellaneousData(MiscellaneousTabDataModel model);

    }
}
