using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoErp.LOC.Services.Models;

namespace NeoErp.LOC.Services.Services
{
    public interface ILcEntryService
    {
        List<LcEntryModels> getAllLcList();
        List<LcEntryModels> GetAllLcListFilter(string perfomainvoice, string lcnumber);
        //void UpdateImage(LcImageModels lcimagedetail); GetAllLcListFilter(string perfomainvoice, string lcnumber)
        //void RemoveImage(LcImageModels lcimage);
        List<LCTermModels> getTermsListByFlter(string filter);
        List<LCBankModels> getBanksListByFlter(string filter);
        List<LCPTermModels> getPaymentTermsListByFlter(string filter);
        List<LCStatusModels> getStatusListByFlter(string filter);
        List<PerformaInvoiceCurrencyModel> getCurrencyListByFlter(string filter);
        List<PerformaInvoiceModel> GetAllLcIpPurchaseOrder(string filter);
        LcEntryModels CreateLCEntry(LcEntryModels lcentrydetails);
        void SaveStatus(string status, string lctrack);
        void UpdateImage(LcImageModels imagedetails, string loccode);
        LCItemsModels GetAllItemsByOrderCode(string pinvoiceno);
        string RemoveLCImage(LcImageModels imagedetails);
        LCDetailsViewModels LCDetailList(string lctracknumber);
        List<LCBankModels> getSupplierBanksListByFlter(string filter);
        List<SupplierModels> getSupplierListByFlter(string filter);
        bool LCNumberExist(int pinvoiceno, string action, int loccode, string lcnumber, out string Message);
        List<SupplierModels> GetAllLcSuppliersFromSynergy(string filter);
        List<LCNumberModels> GetAllLCNumberByFilter(string filter);
        List<ItemNameModels> GetAllItemNameByFilter(string filter);
        void insertIntoMasterTransaction(string lcNumber, string trackNumber);



    }
}
