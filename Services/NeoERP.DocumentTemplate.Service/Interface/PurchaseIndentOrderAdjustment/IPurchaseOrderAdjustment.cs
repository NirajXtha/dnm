using NeoERP.DocumentTemplate.Service.Models.PurchaseOrderIndent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Interface.PurchaseIndentOrderAdjustment
{
    public interface IPurchaseOrderAdjustment
    {
        List<OrderAdjustmentDoc> GetDocForOrderAdjustment(string tableName);

        List<OrderAdjustViewModel> GetAllPurchaseOrderAdjustment(IndentSearchParam param);

        string SaveOrderAdjustment(List<OrderAdjustViewModel> modelList);
    }
}
