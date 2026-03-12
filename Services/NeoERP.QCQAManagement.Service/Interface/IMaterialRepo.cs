using NeoERP.QCQAManagement.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QCQAManagement.Service.Interface
{
   public interface IMaterialRepo
    {
        List<Items> GetMaterialLists();
        List<Items> GetSupplierLists();
        List<Items> GetMaterialListsByCategory(string CategoryCode);
        List<Items> GetVoucherDetailsByItemId(string ItemCode);
        List<Items> GetPending_VoucherDetailsByItemId(string ItemCode);
        List<Items> GetMaterialDetailLists(string VoucherNo);
        string GetQCQAVoucherNo(string TemplateName);
        bool InsertQCParameterData(QCDetail data);
        bool InsertInComingMaterialSampleData(QCDetail data);
        Items GetEditMaterialDetails(string transactionno);
        Items GetEditMaterialDetailsSample(string transactionno);
        List<FormDetailSetup> GetRawMaterialDetails(string assd);
        List<RawMaterialTree> GetMaterialListsGroupWise();
        List<RawMaterialModels> GetMaterialListsByItemCode(string itemCode, string itemMastercode, string searchText);
        List<MuCodeModel> GetMuCode();
        List<RawMaterialDetails> GetRawMaterialDataByItemCode(string productcode);
        List<Items> GetPending_RawMaterialsByItemId(string ItemCode);
        List<BatchDetails> GetBatchNoByItemCode(string itemCode);
        List<BatchDetails> GetBatchNoByTransactionNo(string TransactionNo);
        bool InsertDailyRawMaterialData(RawMaterial data);
        IncomingMaterial GetIncomingMaterialsDetailReport(string transactionno);
        IncomingMaterial GetIncomingMaterialSampleDetailReport(string transactionno);
        RawMaterial GetEditDailyRawMaterial(string transactionno);
        RawMaterial GetDailyRawMaterialReport(string transactionno);
        List<RawMaterial> GetMaterialDetailByProductType(string productType);

    }
}
