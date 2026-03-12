using NeoERP.DocumentTemplate.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Interface
{
    public interface IIRDDataSyncService
    {
        //string IRDSyncInvoice(string dataString);
        //string IRDSyncInvoiceSales_Bill(IRDSyncDataModel model);
        //string IRDSyncInvoiceSales_Return(IRDSyncDataModel Model);
        string IRDSyncInvoice(FormDetails Model);
        string IRDSyncSalesReturn(FormDetails modelObj);
    }
}
