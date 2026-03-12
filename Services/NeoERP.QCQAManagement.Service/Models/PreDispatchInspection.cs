using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QCQAManagement.Service.Models
{
    public class PreDispatchInspection
    {
        public string DISPATCH_NO { get; set; }
        public string CUSTOMER_INVOICE_NO { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public string TRANSPORT_DETAIL { get; set; }
        public string DRIVER_NAME { get; set; }
        public string DRIVER_CONTACT_NO { get; set; }
        public string VEHICLE_NO { get; set; }
        public string PARAMETER_ID { get; set; }
        public string PARAMETERS { get; set; }
        public string MANUAL_NO { get; set; }
        public string PACKING_UNIT { get; set; }
        public string COMPANY_CODE { get; set; }
        public string FORM_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string MODIFY_DATE { get; set; }
        public string MODIFY_BY { get; set; }
        public string DISPATCH_PERSON { get; set; }
        public string QC_INSPECTOR { get; set; }
        public int SERIAL_NO { get; set; }
        public string REMARKS { get; set; }
        //public string ISDUST_VEHICLE { get; set; }
        //public string ISWATERSPILL_VEHICLE { get; set; }
        //public string ISCRACKSHOLES_VEHICLE { get; set; }
        //public string ISNAILS_VEHICLE { get; set; }
        //public string ISLEAKWALL_VEHICLE { get; set; }
        //public string VEHICLE_DUST_REMARKS { get; set; }
        //public string VEHICLE_WATERSPILL_REMARKS { get; set; }
        //public string VEHICLE_CRACKSHOLES_REMARKS { get; set; }
        //public DateTime VEHICLE_NAILS_REMARKS { get; set; }
        //public string VEHICLE_WALL_REMARKS { get; set; }
        //public string ISVISUALDEFECT_PRODUCT { get; set; }
        //public string ISDIMENSIONS_PRODUCT { get; set; }
        //public string ISWEIGHTCHECK_PRODUCT { get; set; }
        //public string PRODUCT_DEFECT_REMARKS { get; set; }
        //public int PRODUCT_DIMENSIONS_REMARKS { get; set; }
        //public string PRODUCT_WEIGHT_REMARKS { get; set; }
        //public string ISCORRECT_PACKAGING { get; set; }
        //public string ISSEALED_PACKAGING { get; set; }
        //public string ISPERBOX_PACKAGING { get; set; }
        //public string ISSTACKING_PACKAGING { get; set; }
        //public DateTime PACKAGING_CORRECT_REMARKS { get; set; }
        //public string PACKAGING_SEALED_REMARKS { get; set; }
        //public string PACKAGING_PERBOX_REMARKS { get; set; }
        //public string PACKAGING_STACKING_REMARKS { get; set; }
        //public string ISINVOICE_DOCUMENTATION { get; set; }
        //public string ISQUALITY_DOCUMENTATION { get; set; }
        //public int ISCOMPLIANCE_DOCUMENTATION { get; set; }
        //public string DOCU_INVOICE_REMARKS { get; set; }
        //public string DOCU_QUALITY_REMARKS { get; set; }
        //public string DOCU_COMP_REMARKS { get; set; }
        public List<PreDispatchInspectionDetails> PreDispatchInspectionDetailsList { get; set; }
    }
    public class PreDispatchInspectionDetails
    {
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string COLUMN_HEADER { get; set; }
        public string ISDUST_VEHICLE { get; set; }
        public string ISWATERSPILL_VEHICLE { get; set; }
        public string ISCRACKSHOLES_VEHICLE { get; set; }
        public string ISNAILS_VEHICLE { get; set; }
        public string ISLEAKWALL_VEHICLE { get; set; }
        public string VEHICLE_DUST_REMARKS { get; set; }
        public string VEHICLE_WATERSPILL_REMARKS { get; set; }
        public string VEHICLE_CRACKSHOLES_REMARKS { get; set; }
        public string VEHICLE_NAILS_REMARKS { get; set; }
        public string VEHICLE_WALL_REMARKS { get; set; }
        public string ISVISUALDEFECT_PRODUCT { get; set; }
        public string ISDIMENSIONS_PRODUCT { get; set; }
        public string ISWEIGHTCHECK_PRODUCT { get; set; }
        public string PRODUCT_DEFECT_REMARKS { get; set; }
        public string PRODUCT_DIMENSIONS_REMARKS { get; set; }
        public string PRODUCT_WEIGHT_REMARKS { get; set; }
        public string ISCORRECT_PACKAGING { get; set; }
        public string ISSEALED_PACKAGING { get; set; }
        public string ISPERBOX_PACKAGING { get; set; }
        public string ISSTACKING_PACKAGING { get; set; }
        public string PACKAGING_CORRECT_REMARKS { get; set; }
        public string PACKAGING_SEALED_REMARKS { get; set; }
        public string PACKAGING_PERBOX_REMARKS { get; set; }
        public string PACKAGING_STACKING_REMARKS { get; set; }
        public string ISINVOICE_DOCUMENTATION { get; set; }
        public string ISQUALITY_DOCUMENTATION { get; set; }
        public string ISCOMPLIANCE_DOCUMENTATION { get; set; }
        public string DOCU_INVOICE_REMARKS { get; set; }
        public string DOCU_QUALITY_REMARKS { get; set; }
        public string DOCU_COMP_REMARKS { get; set; }

        //public string ITEMS { get; set; }
        //public string STATUS { get; set; }
        //public string REMARKS { get; set; }
        //public string COLUMN_HEADER { get; set; }
        //public string TIME_PERIOD { get; set; }
        //public string ITEM_CODE { get; set; }
        //public string ITEM_EDESC { get; set; }
        //public string BATCH_NO { get; set; }
        //public string LOOSE_PRODUCT_SAMPLE { get; set; }
        //public string LOOSE_PRODUCT_DEFECT { get; set; }
        //public string UNSEALED_PACKET_SAMPLE { get; set; }
        //public string UNSEALED_PACKET_DEFECT { get; set; }
        //public string SEALED_PACKET_SAMPLE { get; set; }
        //public string SEALED_PACKET_DEFECT { get; set; }
        //public string BAG_SAMPLE { get; set; }
        //public string BAG_DEFECT { get; set; }
        //public string WRAPPER_SAMPLE { get; set; }
        //public string WRAPPER_DEFECT { get; set; }
        //public string REMARKS { get; set; }
    }
    //public class PACKINGUNIT
    //{
    //    public string MU_CODE { get; set; }
    //    public string MU_EDESC { get; set; }
    //}
}
