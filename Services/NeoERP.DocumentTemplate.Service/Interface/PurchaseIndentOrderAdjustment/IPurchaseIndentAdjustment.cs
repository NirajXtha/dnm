using NeoERP.DocumentTemplate.Service.Models.PurchaseOrderIndent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Interface.PurchaseIndentOrderAdjustment
{
    public interface IPurchaseIndentAdjustment
    {
        List<IndentAdjustmentDoc> GetDocForIndentAdjustment(string tableName);

        List<IndentAdjustViewModel> GetAllPurchaseIndentAdjustment(IndentSearchParam param);

        string SaveIndentAdjustment(List<IndentAdjustViewModel> modelList);
    }
}
