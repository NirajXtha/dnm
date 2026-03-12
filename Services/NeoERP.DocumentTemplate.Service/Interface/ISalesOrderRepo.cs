using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoERP.DocumentTemplate.Service.Models;


namespace NeoERP.DocumentTemplate.Service.Interface
{
    public interface ISalesOrderRepo
    {
        List<SalesOrderDetailView> GetSalesOrderDetails();

        List<SalesOrderDetail> GetSalesOrderDetailsByOrderNo(string orderno);
    }
}
