using NeoErp.Core.Domain;
using NeoErp.Core.Models;
using NeoErp.Distribution.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Service.Mobile
{
    public interface IMobilePOservice
    {
        List<SalesPersonPoModel> GetSalesPersonlst(string toDate, string fromDate, string salePerson,string orderNo = null,string resellerCode = null);
        void ApprovePurchaseOrder(string orderNo, string itemCode, out string message, out bool status);
        void RejectPurchaseOrder(string orderNo, string itemCode, out string message, out bool status);
    }
}
