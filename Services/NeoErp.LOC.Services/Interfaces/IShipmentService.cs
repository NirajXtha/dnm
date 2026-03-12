using NeoErp.LOC.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.LOC.Services.Services
{
    public interface IShipmentService
    {
        void getAllShipmentList();
        List<LogisticItemModels> getShipmentlistbyTrackNo(string lcnumber);
        void createShipment(ShipmentModels sidetails);
        ShipmentModels getShipmentBySno(int sno,string lctrackno);
        List<CommercialInvoiceModel> GetAllLcIpPurchaseOrder(string filter);
        List<CommercialInvoiceModel> GetAllLcIpPurchaseOrderfilter(string filter);
        
    }
}
