using NeoERP.DocumentTemplate.Service.Models.SalesOrderIndent;
using System.Collections.Generic;

namespace NeoERP.DocumentTemplate.Service.Interface.SalesOrderAdjustment
{
    public interface ISalesOrderAdjustment
    {
        List<SalesOrderIndentDocument> GetDocForSalesOrderAdjustment(string tableName);

        List<SalesOrderAdjustViewModel> GetAllSalesOrderAdjustment(OrderSearchParams param);

        string SaveSalesOrderAdjustment(List<SalesOrderAdjustViewModel> modelList); 
    }
}
