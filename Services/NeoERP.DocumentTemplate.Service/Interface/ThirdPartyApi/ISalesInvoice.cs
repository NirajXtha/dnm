using NeoERP.DocumentTemplate.Service.Models.ThirdPartyApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Interface.ThirdPartyApi
{
    public interface ISalesInvoice
    {
        List<SalesInvoice> GetSalesInvoice(SalesInvoiceApiParameter salesInvoiceApiParameter);
    }
}
