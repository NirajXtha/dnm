using NeoErp.LOC.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.LOC.Services.Services
{
    public interface ISettlementInvoiceService
    {
        List<SettlementInvoiceModel> GetAllSettlementInvoices();
        List<SettlementInvoiceModel> GetSettlementInvoicesByID(string id);
        List<SettlementInvocieddlModel> GetInvoice();
        List<SettlementInvocieddlModel> GetInvoiceByFilter(string filter);
        void UpdateFile(LcUploadFileModels lcimagedetail);
        void RemoveFile(LcUploadFileModels lcimagedetail);
        string SaveSettlementInvoices(SettlementInvoiceModel performaInvoices);
        string UpdateSettlementInvoices(SettlementInvoiceModel performaInvoices);
        string DeleteSettlementInvoices(string pfiNumber);
    }
}
