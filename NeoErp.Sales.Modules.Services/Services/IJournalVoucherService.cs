using NeoErp.Core.Domain;
using NeoErp.Core.Helpers;
using NeoErp.Core.Models.CustomModels;
using NeoErp.Sales.Modules.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Services
{
    public interface IJournalVoucherService
    {
        List<JournalVoucherDataModel> GetJournalVoucher(filterOption model);
        List<JournalVoucherDataModel> GetJournalVoucher(filterOption model, User userinfo);
        List<JournalVoucherDataModel> GetJournalsUBLEDGERVoucher(filterOption model, User userinfo);
    }
}
