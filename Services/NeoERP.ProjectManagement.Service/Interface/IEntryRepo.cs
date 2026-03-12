using NeoErp.Core.Domain;
using NeoErp.Core.Models;
using NeoERP.ProjectManagement.Service.Models;
using NeoERP.DocumentTemplate.Service.Models;
using System.Collections.Generic;
using NeoErp.Core.Models.CustomModels;

namespace NeoERP.ProjectManagement.Service.Interface
{
    public interface IEntryRepo
    {
        SubmitResponse SaveWebPrefrence(WebPrefrence model);
        string updateAreaSetup(AreaModels model);
        List<ChargeOnSales> GetChargesData(string formCode, string voucherNo);
        List<ChargeOnSales> GetChargesData(string formCode);
        List<FormControlModels> GetFormControls(string formcode);
        List<FORM_SETUP_REFERENCE> GetRefrenceFlag(string formcode);
        List<FinancialBudgetTransaction> Getbudgetdetail(string voucherno);
        List<BATCH_TRANSACTION_DATA> Getbatchtrackingdetail(string voucherno);
        List<BATCHTRANSACTIONDATA> Getbatchdetail(string voucherno);
        string GetLoactionNameByCode(string code);
        List<SubProjectData> GetSubProjectList();
        List<CITY> GetAllCityDetailsByFilter(string filter);
        List<VECHILES> GetAllVechDetailsByFilter(string filter);
        List<TRANSPORTER> GetAllTransporterDetailsByFilter(string filter);
        string GetNewSequence();
        //string NewVoucherNo(string companycode, string formcode, string transactiondate, string tablename);
        bool ItemNoExistsOrNot(string itemcode);
        bool BatchItemNoExistsOrNot(string itemcode);
        List<Division> GetAllDivisionSetup(string filter);
        List<Customers> getALLSupplierListByFlterForReference(string filter);
        List<Suppliers> getALLSupplierListByFlter(string filter);
        List<Currency> getCurrencyListByFlter(string filter);
        List<AccountSetup> getALLAccountSetupByFlter(string filter);
        List<AccountSetup> getALLAccountForInvBudgetTrans();
        string getSubledgerCodeByAccCode(string accCode);
        List<BudgetCenter> GetAllBudgetCenterForLocationByFilter(string filter);
        List<SubLedger> GetAllSubLedgerByFilter(string filter, string subCode);
        List<TemplateDraftModel> GetDraftList(string moduleCode, string formCode);
        string InsertQuickSetup(QuickSetupModel model);
        string GetPrimaryDateByTableName(string tablename);
        string GetPrimaryColumnByTableName(string tablename);
        Inventory MapMasterColumnWithValue(string masterColumn);
        List<Inventory> MapChildColumnWithValue(string childColumn);
        List<CustomOrderColumn> MapCustomTransactionWithValue(string customTransaction);
        List<BATCHTRANSACTIONDATA> MapBatchTransactionValue(string batchValue);
        List<BATCH_TRANSACTION_DATA> MapBatchTransValue(string batchTransValue);
        List<ChargeOnSales> MapChargesColumnWithValue(string charges);
        ShippingDetails MapShippingDetailsColumnValue(string shippingDetails);
        bool SaveChildColumnValue(List<Inventory> childColumnValue, Inventory masterColumnValue, CommonFieldsForInventory commonValue, FormDetails model, string primarydatecolumn, string primarycolname, NeoErpCoreEntity dbcontext = null);
        bool SaveMasterColumnValue(Inventory masterColumnValue, CommonFieldsForInventory commonValue, NeoErpCoreEntity dbcontext = null);
        void GetFormReference(CommonFieldsForInventory commonValue, List<REF_MODEL_DEFAULT> REF_MODEL, NeoErpCoreEntity dbcontext = null);
        void SaveBudgetTransactionColumnValue(List<FinancialBudgetTransaction> budgetTransaction, CommonFieldsForInventory commonValue, NeoErpCoreEntity dbcontext = null);
        void SaveBatchTransactionValues(Inventory masterColumnValue, List<BATCHTRANSACTIONDATA> batchTransaction, CommonFieldsForInventory commonValue, NeoErpCoreEntity dbcontext = null);
        void SaveBatchTransValues(List<Inventory> childColumnValue, Inventory masterColumnValue, List<BATCH_TRANSACTION_DATA> batchTrans, CommonFieldsForInventory commonValue, NeoErpCoreEntity dbcontext = null);
        void SaveChargeColumnValue(List<ChargeOnSales> chargeCol, CommonFieldsForInventory commonField, NeoErpCoreEntity dbcontext = null);
        void SaveCustomTransaction(List<CustomOrderColumn> customcolumn, CommonFieldsForInventory commonValue, NeoErpCoreEntity dbcontext = null);
        void SaveShippingDetailsColumnValue(ShippingDetails shippingDetails, CommonFieldsForInventory commonFieldForSales, NeoErpCoreEntity Dbcontext = null);
        List<Inventory> GetMasterTransactionByVoucherNo(string voucherNumber);
        bool DeleteChildTransaction(CommonFieldsForInventory commonValue, NeoErpCoreEntity dbcontext = null);
        bool UpdateMasterTransaction(CommonFieldsForInventory commonUpdateValue, NeoErpCoreEntity dbcontext = null);

        void DeleteBudgetTransaction(string voucherNo, NeoErpCoreEntity dbcontext = null);
        void DeleteBatchTransaction(string voucherNo, NeoErpCoreEntity coreEntity);
        void DeleteChargeTransaction(string voucherNo, NeoErpCoreEntity coreEntity);
        void DeleteCustomTransaction(string voucherNo, NeoErpCoreEntity dbcontext = null);
        void UpdateShippingDetailsColumnValue(ShippingDetails shippingDetails, CommonFieldsForInventory commonFieldForSales, NeoErpCoreEntity Dbcontext = null);
        List<FinancialBudgetTransaction> MapBudgetTransactionColumnValue(string transactionValue);
        List<ShippingDetailsViewModel> GetShippingData(string formCode, string voucherNo);
        List<Products> GetProductDataByProductCode(string productcode);
        List<FormCustomSetup> GetFormCustomSetup(string formCode, string voucherno);
        List<FinanceVoucherReference> GetFinanceVoucherReferenceList(string formcode);
        List<REFERENCE_DETAILS> GetReference_Details_For_VoucherNo(string VoucherNo, string formcode);
        List<PartyType> GetAllPartyTypes();
        List<COMMON_COLUMN> GetProductionFormDetail(string formCode, string TableName, string RoutingCode, decimal ProductQty);
        string GetcustomerNameByCode(string code);
        string GetItemNameByCode(string code);
        bool CheckVoucherNoPosted(string voucherno);
        string CheckVoucherNoReferenced(string voucherno);
        List<FormDetailSetup> GetFormDetailSetup(string formCode);
        bool deletevouchernoInv(string tablename, string formcode, string voucherno, string primarycolumnname);
        List<DraftFormModel> GetDraftFormDetailSetup(string formCode);
        List<FormSetup> GetFormSetupByFormCode(string formCode);
        List<COMMON_COLUMN> GetSalesOrderFormDetail(string formCode, string orderno);
        List<BudgetCenter> getBudgetCodeByAccCode(string accCode);
        List<DraftFormModel> getDraftDataByFormCodeAndTempCode(string formCode, string tempCode);
        int? GetTotalVoucher(string form_code, string table_name);


    }
}
