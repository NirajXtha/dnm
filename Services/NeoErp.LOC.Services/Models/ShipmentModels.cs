using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.LOC.Services.Models
{
    public class ShipmentModels
    {
        public ShipmentModels()
        {
            ITEMLIST = new List<ItemListModel>();
        }
        public int LC_TRACK_NO { get; set; }
        public int SNO { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string PINVOICE_NO { get; set; }
        public int? QUANTITY { get; set; }
        public int WEIGHT { get; set; }
        public string MU_CODE { get; set; }
        public int NO_OF_PACK { get; set; }
        public string SHIPMENT_STATUS { get; set; }
        public string FROM_LOCATION { get; set; }
        public string TO_LOCATION { get; set; }
        public string AWB_NO { get; set; }
        public string BL_NO { get; set; }
        public DateTime? BL_DATE { get; set; }
        public string RR_NO { get; set; }
        public DateTime? RR_DATE { get; set; }
        public string LR_NO { get; set; }
        public DateTime? LR_DATE { get; set; }
        public string CONSIGNEE_NAME { get; set; }
        public string CONSIGNEE_ADDRESS { get; set; }
        public string SHIPPER_NAME { get; set; }
        public string SHIPPER_ADDRESS { get; set; }
        public string ISSUING_CARRIER_NAME { get; set; }
        public string ISSUING_CARRIER_ADDRESS { get; set; }
        public string NOTIFY_APPLICANT_NAME { get; set; }
        public string NOTIFY_APPLICANT_ADDRESS { get; set; }
        public string INVOICE_NO { get; set; }
        public string SHIPMENT_TYPE { get; set; }
        public string LOAD_TYPE { get; set; }
        public string CONTAINER_SIZE { get; set; }
        public string CONTAINER_NO { get; set; }
        public int? GROSS_WEIGHT_SEA { get; set; }
        public int? CBM_SEA { get; set; }
        public int? GROSS_WEIGHT_AIR { get; set; }
        public int? CHARGEABLE_WEIGHT_AIR { get; set; }
        public DateTime? AWB_DATE { get; set; }
        public int EST_DAY { get; set; }
        public string VESSEL_NO { get; set; }
        public string TRUCK_NO { get; set; }
        public string TRANSPORTER_NAME { get; set; }
        public string TRANSPORTER_ADDRESS { get; set; }
        public string REMARKS { get; set; }
        public string DETAIL_ENTRY_FLAG { get; set; }
        public string INVOICE_NUMBER { get; set; }
        public int? RECEIVED_QUANTITY { get; set; }
        public IList<ItemListModel> ITEMLIST { get; set; }
    }

    public class ItemListModel
    {
        public int SNO { get; set; }
        public string ITEM_CODE { get; set; }
        public int QUANTITY { get; set; }
        public string MU_CODE { get; set; }
        public int RECEIVED_QUANTITY { get; set; }
        public int EST_DAY { get; set; }
    }
   
    
}