using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QCQAManagement.Service.Models
{
    public class HandOverInspection
    {
        public string DISPATCH_NO { get; set; }
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
        public int SERIAL_NO { get; set; }
        public string REMARKS { get; set; }
        public string OVERALL_REMARKS { get; set; }

        public decimal LooseProduct_SampleTotal { get; set; }
        public decimal LooseProduct_DefectTotal { get; set; }
        public decimal UnsealedPacket_SampleTotal { get; set; }
        public decimal UnsealedPacket_DefectTotal { get; set; }
        public decimal SealedPacket_SampleTotal { get; set; }
        public decimal SealedPacket_DefectTotal { get; set; }
        public decimal CartonBag_SampleTotal { get; set; }
        public decimal CartonBag_DefectTotal { get; set; }
        public decimal WrapperInPrinter_SampleTotal { get; set; }
        public decimal WrapperInPrinter_DefectTotal { get; set; }
        public decimal LooseProductTotal { get; set; }
        public decimal UnsealedPacketTotal { get; set; }
        public decimal SealedPacketTotal { get; set; }
        public decimal CartonBagTotal { get; set; }
        public decimal WrapperInPrinterTotal { get; set; }
        public List<HandOverInspectionDetails> HandOverInspectionDetailsList { get; set; }
    }
    public class HandOverInspectionDetails
    {
        public string TIME_PERIOD { get; set; }
        public string PRODUCT_TYPE { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string BATCH_NO { get; set; }
        public string BATCH_EDESC { get; set; }
        public string LOOSE_PRODUCT_SAMPLE { get; set; }
        public string LOOSE_PRODUCT_DEFECT { get; set; }
        public string UNSEALED_PACKET_SAMPLE { get; set; }
        public string UNSEALED_PACKET_DEFECT { get; set; }
        public string SEALED_PACKET_SAMPLE { get; set; }
        public string SEALED_PACKET_DEFECT { get; set; }
        public string BAG_SAMPLE { get; set; }
        public string BAG_DEFECT { get; set; }
        public string WRAPPER_SAMPLE { get; set; }
        public string WRAPPER_DEFECT { get; set; }
        public string REMARKS { get; set; }
    }
    public class PACKINGUNIT
    {
        public string MU_CODE { get; set; }
        public string MU_EDESC { get; set; }
    }
    //class HandOverInspection
    //{

    //}
}
