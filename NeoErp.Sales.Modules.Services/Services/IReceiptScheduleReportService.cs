using NeoErp.Core.Domain;
using NeoErp.Core.Helpers;
using NeoErp.Core.Models;
using NeoErp.Sales.Modules.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Services
{
   public interface IReceiptScheduleReportService
    {
        IList<ReceiptScheduleModel> GetCustomerWiseReceiptSchedule(ReportFiltersModel model, User userinfo);
        IList<PaymentScheduleModel> GetSupplierWisePaymentSchedule(ReportFiltersModel model, User userinfo);
    }
}
