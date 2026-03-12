using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QCQAManagement.Service.Models
{
    public class GlobalAgroProducts
    {
        public string TRANSACTION_NO { get; set; }
        public string REFERENCE_NO { get; set; }
        public string ITEM_CODE { get; set; }
        public int SERIAL_NO { get; set; }
        public string FORM_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string PARTY_NAME { get; set; }
        public string ADDRESS { get; set; }
        public string PHYSICAL_TEST_RAWMATERIAL { get; set; }
        public string WEIGHT { get; set; }
        public string MOISTURE { get; set; }
        public string TEMPERATURE { get; set; }
        public string WET { get; set; }
        public string FUNGUS { get; set; }
        public string DUST { get; set; }
        public string GRADING { get; set; }
        public string SMELL { get; set; }
        public string COLOR { get; set; }
        public string PIECES { get; set; }
        public string IMMATURITY_OF_GRAINS { get; set; }
        public string OTHER_ITEMS { get; set; }
        public string ROTTEN_HOLED { get; set; }
        public string DAMAGED { get; set; }
        public string BROKEN { get; set; }
        public string HUSK { get; set; }
        public string OVERTOASTED { get; set; }
        public string USEABLE { get; set; }
        public string UNUSEABLE { get; set; }
        public string FAT { get; set; }
        public string QUALITY_OF_GOODS { get; set; }
        public string EXCELLENT { get; set; }
        public string GREAT { get; set; }
        public string GOODS_NORMAL { get; set; }
        public string WAREHOUSE { get; set; }
        public string SILO { get; set; }
        public string GHAN { get; set; }
        public string PROTEIN { get; set; }
        public string QUALITY_OF_FIREWOOD { get; set; }
        public string PRODUCT_SIZE { get; set; }
        public string PRODUCT_TYPE { get; set; }
        public string DEDUCT_IN_BAG { get; set; }
        public string DEDUCT_IN_PLASTIC { get; set; }
        public string DEDUCT_IN_JUTE { get; set; }
        public string DEDUCT_IN_WT { get; set; }
        public string NET_WEIGHT { get; set; }
        public string PRODUCT_REMARKS { get; set; }
        public string REMARKS { get; set; }
        public string UNLOAD_UNIT { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string CREATED_DATE_STR { get; set; }
        public string MODIFY_DATE { get; set; }
        public string MODIFY_BY { get; set; }
        public string CHECKED_BY { get; set; }
        public string AUTHORISED_BY { get; set; }
        public string ISPLASTIC_BAG { get; set; }
        public string ISJUTE_BAG { get; set; }
        public string ITEM_EDESC { get; set; }
        public string ISPLASTIC_WEIGHT { get; set; }
        public string ISJUTE_WEIGHT { get; set; }
        public string VEHICLE_NO { get; set; }
        public string BILL_NO { get; set; }
        public string GATE_OR_GRN_NO { get; set; }
        public int SLIDER_VALUE { get; set; }
        public string ISUNLOAD { get; set; }
        public string ISPROTEIN { get; set; }
        public string ISPT { get; set; }
        public string ISOUT { get; set; }
        public List<WEIGHTDETAILS> WEIGHTDETAILSList { get; set; }
        public List<UNLOADEDCHHALLI> UNLOADEDCHHALLIList { get; set; }
        public List<DAKHILADETAILS> DAKHILADETAILSList { get; set; }
    }
    public class WEIGHTDETAILS
    {
        public string FIRST_WEIGHT { get; set; }
        public string SECOND_WEIGHT { get; set; }
        public string NET_WEIGHT { get; set; }
        public string CHALLAN_WEIGHT { get; set; }
        public string WEIGHT_DIFFERENCE { get; set; }
        public string REMARKS { get; set; }
    }
    public class UNLOADEDCHHALLI
    {
        public string FIRST_CHHALLI { get; set; }
        public string SECOND_CHHALLI { get; set; }
        public string THIRD_CHHALLI { get; set; }
        public string FOURTH_CHHALLI { get; set; }
        public string FIFTH_CHHALLI { get; set; }
        public string SIXTH_CHHALLI { get; set; }
        public string SEVENTH_CHHALLI { get; set; }
        public string EIGHTH_CHHALLI { get; set; }
        public string NINETH_CHHALLI { get; set; }
        public string TENTH_CHHALLI { get; set; }
        public string ELEVEN_CHHALLI { get; set; }
        public string TWELVE_CHHALLI { get; set; }
        public string TOTAL { get; set; }
        public string REMARKS { get; set; }
    }
    public class DAKHILADETAILS
    {
        public string ENTRY_NO { get; set; }
        public string BILL_NO { get; set; }
        public string CHALAN_NO { get; set; }
        public string ITEM { get; set; }
        public string TOTAL_BAG { get; set; }
        public string WEIGHT { get; set; }
        public string REMARKS { get; set; }
    }
}
