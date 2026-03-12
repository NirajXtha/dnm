using NeoErp.Core.Models;
using NeoERP.DocumentTemplate.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Interface
{
    public interface IInventoryVoucher
    {
        int? GetTotalVoucher(string form_code, string table_name);
        List<FinancialBudgetTransaction> Getbudgetdetail(string voucherno);
        string InsertInventoryImage(DocumentTransaction documentdetail);
        Inventory MapMasterColumnWithValue(string masterColumn);
        List<Inventory> MapChildColumnWithValue(string childColumn);
        List<CustomOrderColumn> MapCustomTransactionWithValue(string customTransaction);
        decimal MapDrTotalColumnValue(string drValue);
        bool SaveChildColumnValue(List<Inventory> childColumnValue, Inventory masterColumnValue, CommonFieldsForInventory commonValue, FormDetails model, string primarydatecolumn, string primarycolname, out int session_row_id_out_param, NeoErpCoreEntity dbcontext = null);
        //bool SaveChildColumnValue(List<Inventory> childColumnValue, Inventory masterColumnValue, CommonFieldsForInventory commonValue, FormDetails model,string primarydatecolumn,string primarycolname, NeoErpCoreEntity dbcontext = null);
        List<FinancialBudgetTransaction> MapBudgetTransactionColumnValue(string transactionValue);
        void SaveBudgetTransactionColumnValue(List<FinancialBudgetTransaction> budgetTransaction, CommonFieldsForInventory commonValue, NeoErpCoreEntity dbcontext = null);
        void SaveCustomTransaction(List<CustomOrderColumn> customcolumn, CommonFieldsForInventory commonValue, NeoErpCoreEntity dbcontext = null);
        bool SaveMasterColumnValue(Inventory masterColumnValue, CommonFieldsForInventory commonValue, NeoErpCoreEntity dbcontext = null);
        List<Inventory> GetMasterTransactionByVoucherNo(string voucherNumber);
        bool DeleteChildTransaction(CommonFieldsForInventory commonValue, NeoErpCoreEntity dbcontext = null);
        bool UpdateMasterTransaction(CommonFieldsForInventory commonUpdateValue, NeoErpCoreEntity dbcontext = null);
        void DeleteBudgetTransaction(string voucherNo, NeoErpCoreEntity dbcontext = null);
        void DeleteCustomTransaction(string voucherNo, NeoErpCoreEntity dbcontext = null);
        //void GetFormReference(List<Inventory> childColumnValue, Inventory masterColumnValue, CommonFieldsForInventory commonValue, FormDetails model, string primarydatecolumn, string primarycolname, NeoErpCoreEntity dbcontext = null);
        void GetFormReference(CommonFieldsForInventory commonValue, List<REF_MODEL_DEFAULT> REF_MODEL, NeoErpCoreEntity dbcontext = null);
        List<ChargeOnSales> MapChargesColumnWithValue(string charges);
        void SaveChargeColumnValue(List<ChargeOnSales> chargeCol, CommonFieldsForInventory commonField, NeoErpCoreEntity dbcontext = null);

        void DeleteChargeTransaction(string voucherNo, NeoErpCoreEntity coreEntity);

        ShippingDetails MapShippingDetailsColumnValue(string shippingDetails);
        void SaveShippingDetailsColumnValue(ShippingDetails shippingDetails, CommonFieldsForInventory commonFieldForSales, NeoErpCoreEntity Dbcontext = null);
        void UpdateShippingDetailsColumnValue(ShippingDetails shippingDetails, CommonFieldsForInventory commonFieldForSales, NeoErpCoreEntity Dbcontext = null);

        List<BATCHTRANSACTIONDATA> MapBatchTransactionValue(string batchValue);

        void SaveBatchTransactionValues(Inventory masterColumnValue, List<BATCHTRANSACTIONDATA> batchTransaction, CommonFieldsForInventory commonValue, NeoErpCoreEntity dbcontext = null);

        List<BATCHTRANSACTIONDATA> Getbatchdetail(string voucherno);

        List<BATCH_TRANSACTION_DATA> Getbatchtrackingdetail(string voucherno);



        void DeleteBatchTransaction(string voucherNo, NeoErpCoreEntity coreEntity);

        List<BATCH_TRANSACTION_DATA> MapBatchTransValue(string batchTransValue);

        void SaveBatchTransValues(List<Inventory> childColumnValue, Inventory masterColumnValue, List<BATCH_TRANSACTION_DATA> batchTrans, CommonFieldsForInventory commonValue, NeoErpCoreEntity dbcontext = null);

        bool deletevouchernoInv(string tablename, string formcode, string voucherno, string primarycolumnname);

        void SaveResourceDetailsEntryColumnValue(List<ResourceDetail> resourceDetailList, string voucherNo, string process_code, string form_code, string resource_category_type = "", bool isEdit = false, NeoErpCoreEntity Dbcontext = null);

        bool DeleteResourceList(string voucherNo, string resource_category_type, NeoErpCoreEntity coreEntity = null);

        bool DeleteIssueRecord(string voucherno);

        PPDetailModel MapPPDetailColumnValue(string ppDetailValue);
        void SavePPDetailColumnValue(PPDetailModel ppDetailModel, CommonFieldsForInventory commonFieldForInventory, NeoErpCoreEntity Dbcontext = null);
        void UpdatePPDetailColumnValue(PPDetailModel ppDetailModel, CommonFieldsForInventory commonFieldForInventory, NeoErpCoreEntity Dbcontext = null);


    }
}
