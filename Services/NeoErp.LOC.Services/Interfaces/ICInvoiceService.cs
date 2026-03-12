using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoErp.LOC.Services.Models;

namespace NeoErp.LOC.Services.Services
{
    public interface ICInvoiceService
    {
        List<ItemDetails> GetItemsByLCNumber(string lcnumber);
        CommercialInvoiceModel CreateCommercialInvoice(MultiCommercialInvoiceModel multiCommercialInvoiceModel);
        List<CommercialInvoiceModel> GetAllCommInvoice();
        List<CommercialInvoiceModel> GetAllCommInvoiceFilter(string lcnumber);
        void UpdateImage(LcImageModels imagedetails, string purchaseorder);
        List<Items> GetAllCommercialItemsList(string filter);
        CommercialInvoiceModel GetDetailByInvoiceNo(string invoiceno);
        void RemoveCiImage(LcImageModels imagedetail);

        LcLogisticPlanModel GetLogisticPlanList(string lcnumber);
        LcLogisticPlanModel GetLogisticItemsList(string lcnumber);
        //void UpdateQuantity(Items cdetail);
        string UpdateQuantity(Items cdetail);
        string InvoiceNumberExist(string ordernumber, string action, int pocode);
        List<CommercialInvoiceModel> GetAllLcNumbers(string filter);
        List<CommercialInvoiceModel> GetAllLcNumbersfilter(string filter);
        
        List<CommercialInvoiceModel> GetAllInvoiceNumbers(string filter, string lcnumber);
        List<CommercialInvoiceModel> LoadCIBylcnumber(string lcnumber);
        CommercialInvoiceModel EditCommercialInvoice(string lotNumber, string invoiceNumber, string lcNumber);
        List<CommercialInvoicHistoryModel> getAllHistoryCommercialInvoiceList(string lctrackno);
        List<InvoiceNumberModels> GetAllInvoiceNumberByFilter(string filter);

        string CreateCIDONumber(List<CIDOModel> cIDOModel);

       List<CIDOModel> GetCIItemDOByItemCode(string LC_TRACK_NO,string INVOICE_CODE, string ITEM_CODE);
    }
}
