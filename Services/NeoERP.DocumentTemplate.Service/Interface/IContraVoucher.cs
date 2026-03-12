using NeoERP.DocumentTemplate.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Interface
{
    public interface IContraVoucher
    {
        List<SalesOrderDetailView> GetAllOptionsList();
        List<FinanceVoucherReference> GetFinanceVoucherReferenceList(string formcode);
        int? GetTotalVoucher(string form_code,string table_name);
        List<FinancialSubLedger> Getsubledgerdetail(string voucherno, string acccode);
        List<FinancialBudgetTransaction> Getbudgetdetail(string voucherno, string acccode);
        string GetPrimaryColumnByTableName(string tablename);
        string InsertFinanceImage(DocumentTransaction documentdetail);
        List<FinancialTDS> Gettdsdetail(string voucherno, string acccode);
        List<FinancialVAT> Getvatdetail(string voucherno, string acccode);
        List<DocumentTemplateMenu> DocumentList();
        List<PurExpSheet> DocumentListDropDown(string tableName, string formCode, string fromdate, string todate, string manualNo = null, string ppNo = null, string supplierCode = null);
        List<PurExpSheet> InvoiceDetails(string invoiceNo);
        List<PurExpSheet> InvoiceDetailsForGrid(string invoiceNo);
        List<IPChargeEdesc> GetIPChargeEdesc(string formCode);
        List<IPChargeCode> GetIPChargeCode(string formCode);
        string GetPurchaseExpensesFlag(string formCode);
        List<PurExpSheet> GetChargeExpList(string voucherNo,string accCode,string accEdesc);

        List<FVPURCHASEEXPSHEETRERERENCE> GetFVReferencePurchaseexpsheet(string voucherno);
        List<IPChargeDtls> GetIPChargedetls(string formCode);
        List<string> GetVoucherNoFrmCharge(string invoiceNo);
        List<PurExpSheet> GetChargeDtlFrmInvoice(string invoiceNo);
        List<PurExpSheet> GetChargeDtlFrmVoucherNo(string orderNo);
        List<PurExpSheet> BindAccountCode(string formCode, string itemEdesc);
    }
}
