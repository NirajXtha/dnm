using NeoErp.LOC.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.LOC.Services.Services
{
    public interface IPerfomaInvoice
    {
        //
        List<PerformaInvoiceModel> GetAllPerfomaInvoice();
        List<PerformaInvoiceModel> getAllPerfomaInvoiceFilter(string purchaseOrder, string pinvoiceno, string pinvoicedate);
        PerformaInvoiceModel CreatePerfomaInvoice(PerformaInvoiceModel perfomadetail);
        List<PerformaInvoiceModel> GetAllLcIpPurchaseOrder(string filter);
        List<PerformaInvoiceModel> getAllIpPurchaseOrderfilter(string filter);
        List<PerformaInvoiceModel> getAllIpipPurchaseInvoicefilter(string filter);
        
        List<ItemDetails> GetAllItemsByTrackOrderNo(string OrderCode);
        void UpdateImage(LcImageModels imagedetails, string pinvoicecode);
        string RemovePoImage(LcImageModels imagedetails);
        bool CreateItems(List<Items> itemdetails);
        bool UpdateItems(List<Items> itemdetails);
        void deletePI(string lctrack, string sno);
        bool ProformaNumberExist(string pinvoiceno, string action, int pinvoicecode,string orderno, out string Message);
        List<PendingItemModels> PendingItemsList();
    }
}
