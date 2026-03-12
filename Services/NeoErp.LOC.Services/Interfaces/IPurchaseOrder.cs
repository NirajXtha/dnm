using NeoErp.LOC.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.LOC.Services.Services
{
    public interface IPurchaseOrder
    {
        List<ItemDetail> GetAllItemsLists(string filter);
        List<ItemDetail> GetAllMuCodeLists(string filter,string ItemCode);
        //List<ItemDetail> GetAllShipmentItemsLists(string filter,string lctrack);
        List<Items> GetAllPOItemsLists(string filter);
        List<ItemHistoryModels> GetAllPOHistoryItemsLists(string filter);
        List<ShipmentHistoryModels> GetAllPOHistoryShipmentList(string filter);
        List<DocumentHistoryModels> GetAllPOHistoryDocumentList(string filter);
        List<ShipmentModels> GetAllPOShipmentLists(string filter);
        PurchaseOrderModels CreatePurchaseOrder(PurchaseOrderModels podetails);
        void UpdateImage(LcImageModels imagedetails,string purchaseorder);
        List<PurchaseOrderModels> GetAllPurchaseOrders();
        List<PurchaseOrderModels> getAllPurchaseOrdersFilter(string purchaseOrder, string beneficiaryname, string orderdate);
        bool CreateItems(List<Items> itemdetails);
        bool UpdateItems(List<Items> itemdetails);
        //string CreateShipment(ShipmentModels shipmentdetails);
        List<IpPurchaseOrderModels> GetAllIpPurchaseOrders(string filters);
        List<IpPurchaseOrderModels> GetAllIpPurchaseOrdersfilter(string filters);
        
        string RemovePoImage(LcImageModels imagedetails);
        List<ItemDetails> GetAllItemsByTrackOrderNo(string OrderCode);
        List<Currency> GetAllCurrency();
        void deletePO(string lctrack, string sno);
        string OrderNumberExist(string ordernumber,string action,int pocode);

        POModel getPurchaseOrderdateandsupplierByOrderCode(string OrderCode);
        
    }
}
