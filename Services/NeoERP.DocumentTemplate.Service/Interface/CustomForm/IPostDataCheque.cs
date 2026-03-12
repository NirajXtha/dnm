using NeoERP.DocumentTemplate.Service.Models.CustomForm;
using System.Collections.Generic;

namespace NeoERP.DocumentTemplate.Service.Interface.CustomForm
{
    public interface IPostDataCheque
    {

        #region POST DATA CHEQUE

        List<CustomerModal> GetCustomerSubLedger();
        List<PartyTypeCodeModal> GetAllPartyType();

        List<PartyTypeCodeModal> GetAllDealerType();

        string SaveNewPDCForm(PDCFormSaveModal modal);

        PDCVoucherResponse GeneratePdcOdcVoucher(PDCFormSaveModal modal);

        PDCFormSaveModal EditPDCFormDetail(string pdcId,string pdcStatus);

        string DeletePDCFormDetail(string pdcId);

        List<PDCFormSaveModal> GetAllPDCFormDetail(PDCFilter filter);

        List<PDCFormSaveModal> SearchAllPDCDetail(PDCFilter filter);

        #endregion

        #region COLUMN SETTINGS

        List<FormDetailModal> GetAllFormDetail();

        List<Erp_TableName> GetAllTableName();

        List<FormDetailEditModal> GetAllFormDetailToEdit(string formCode,string tableName);

        string EditAllFormDetail(List<FormDetailEditModal> modal);
        string SaveAllFormDetail(List<FormDetailEditModal> modal);

        List<ColumnNameAsDDL> GetColumnNameForDDL();
        List<TableNameAsDDL> GetTableNameForDDL();
        bool DeleteAllFormDetail(FormDetailEditModal modal);

        List<string> GenerateNewReceipt();
        List<string> GenerateNewPaymentNo();
        List<string> GenerateNewBGVoucher();

        #endregion

        #region CASH BANK SETUP

        List<CashBankSelectedAccountModal> GetSelectedAccountGrid();
        List<CashBankRootDetailModal> GetCashBankRootDetail();
        List<CashBankAccountDetail> GetCashBankAccountDetail(string cb_code);
        string GenerateCBId();
        string SaveCashBankAccountDetail(CashBankAccountDetailSaveModal modal);
        string DeleteCashBankDetail(string cb_code);

        #endregion


        #region BANK RECONCILATION

        List<BankDetailForReconcilation> GetBankDetailForReconcilation();

        List<ReconcilationGridModel> GetDataForReconcilation();

        string UpdateBankReconcilation(string voucherNo);

        BankDetailForReconcilation EditBankReconcilationDetail(string brId);

        string DeleteBankReconcilationDetail(string brId);

        LoggedInInfoModal GetLoggedInInfo();
         
        #endregion


        #region BANK GURANTEE

        List<BankGuranteeModal> GetBankGuranteeList();

        string SaveBankGurantee(BankGuranteeModal modal);

        BankGuranteeModal EditBankGuaranteeDetail(string bgId);

        string DeleteBankGuaranteeDetail(string bgId);
        List<CustomerModal> GetAllCustomer();
        List<SupplierModel> GetAllSupplier();

        #endregion


    }
}
