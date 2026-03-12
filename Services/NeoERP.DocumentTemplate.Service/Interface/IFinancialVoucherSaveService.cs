using NeoERP.DocumentTemplate.Service.Models;
using System.Collections.Generic;
using NeoErp.Core.Models;

namespace NeoERP.DocumentTemplate.Service.Interface
{
    public interface IFinancialVoucherSaveService
    {

        #region INTERFACE FOR  MAPPING COLUMN VALUE TO OBJECT
        FinancialVoucherDetail MapMasterColumnWithValue(string masterColumn);
        List<FinancialVoucherDetail> MapChildColumnWithValue(string childColumn);
        List<FinancialSubLedger> MapChildSubLedgerColumnWithValue(string childSubLedgerValue);
        List<FinancialVAT> MapVaTColumnWithValue(string vatValue);
        decimal MapDrTotalColumnValue(string drValue);
        decimal MapCrTotalColumnValue(string crValue);
        List<FinancialBudgetTransaction> MapBudgetTransactionColumnValue(string transactionValue);
        List<FinancialTDS> MapTdsColumnValue(string tdsValue);
        List<CHARGETRANSACTION> MapChargeTransactionColumnValue(string chargeTransation);       

        #endregion


        #region INTERFACE FOR SAVING COLUMN VALUE TO DB

        bool SaveChildColumnValue(List<FinancialVoucherDetail> childColumnValue,FinancialVoucherDetail masterColumnValue, CommonFieldsForFinanacialVoucher commonValue, NeoErpCoreEntity dbContext = null);
        bool SaveMasterColumnValue(FinancialVoucherDetail masterColumnValue,CommonFieldsForFinanacialVoucher commonValue, NeoErpCoreEntity dbContext = null);
        bool SaveBudgetTransactionColumnValue(List<FinancialBudgetTransaction> budgetTransaction,CommonFieldsForFinanacialVoucher commonValue, NeoErpCoreEntity dbContext = null);
        bool SaveFinancialTDSColumnValue(List<FinancialTDS> financialTDS, CommonFieldsForFinanacialVoucher commonValue, NeoErpCoreEntity dbContext = null);
        bool SaveFinancialVATValue(List<FinancialVAT> financialVAT, CommonFieldsForFinanacialVoucher commonValue, NeoErpCoreEntity dbContext = null);
        bool SaveFinancialSubLedgerColumnValue(List<FinancialSubLedger> financialSubLedgers, CommonFieldsForFinanacialVoucher commonValue, NeoErpCoreEntity dbContext = null);
        bool SaveChargeTransaction(List<CHARGETRANSACTION> chargetran, CommonFieldsForFinanacialVoucher commonValue, NeoErpCoreEntity dbContext = null);
        #endregion


        #region INTERFACE FOR EDITING FINANCIAL VOUCHER

        List<FinancialVoucherDetail> GetMasterTransactionByVoucherNo(string voucherNumber);

        bool DeleteChildTransaction(CommonFieldsForFinanacialVoucher commonUpdateValue, NeoErpCoreEntity dbContext = null);

        bool UpdateMasterTransaction(CommonFieldsForFinanacialVoucher commonUpdateValue, FinancialVoucherDetail masterColumnValue, NeoErpCoreEntity dbContext = null);

        void DeleteBudgetTransaction(string voucherNo, NeoErpCoreEntity dbContext = null);

        void DeleteSubLedgerTransaction(string voucherNo, NeoErpCoreEntity dbContext = null);

        void DeleteTDSTransaction(string voucherNo, NeoErpCoreEntity dbContext = null);

        void DeleteVatTransaction(string voucherNo, NeoErpCoreEntity dbContext = null);
        void DeleteChargeTransaction(string voucherNo, NeoErpCoreEntity dbContext = null);
        #endregion

    }
}
