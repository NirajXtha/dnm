using NeoERP.QCQAManagement.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QCQAManagement.Service.Interface
{
    public interface IGlobalAgroProductsRepo
    {
        GlobalAgroProducts GetGlobalAgroProductLists();
        List<Items> GetGlobalAgroMaterialDetailLists(string VoucherNo);
        List<Items> GetGateEntryDetailsByItemId(string ItemCode);
        List<Items> GetGRNDetailsByItemId(string ItemCode);
        bool InsertGlobalAgroProductsData(GlobalAgroProducts data);
        GlobalAgroProducts GetEditGlobalAgroProductLists(string transactionno);
        GlobalAgroProducts GetGlobalAgroProductsReport(string transactionno);

    }
}
