using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QCQAManagement.Service.Models
{
    public class RawMaterial
    {
        public string QC_NO { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string MANUAL_NO { get; set; }
        public string COMPANY_CODE { get; set; }
        public string FORM_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string MODIFY_DATE { get; set; }
        public string MODIFY_BY { get; set; }
        public int SERIAL_NO { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_EDESC { get; set; }
        public string BATCH_NO { get; set; }
        public string TRANSACTION_NO { get; set; } // THIS IS USED FOR BATCH TRANSACTION TRANSACTION NO
        public string GSM { get; set; }
        public string WIDTH { get; set; }
        public decimal? SIZE_WIDTH { get; set; }      
        public string STRENGTH { get; set; }
        public string STRENGTH_MD { get; set; }
        public string ACTUAL_STRENGTH_MD { get; set; }
        public string ROLL_NO { get; set; }
        public string THICKNESS { get; set; }
        public string ACTUAL_GSM { get; set; }
        public string ACTUAL_WIDTH { get; set; }
        public string ACTUAL_STRENGTH { get; set; }
        public string ACTUAL_THICKNESS { get; set; }
        public string REMARKS { get; set; }
        public List<RawMaterial> RawMaterialList { get; set; }
    }

    public class IncomingMaterial
    {
        public string master_item_code { get; set; }
        public string TRANSACTION_NO { get; set; }
        public string CATEGORY_CODE { get; set; }
        public string item_edesc { get; set; }
        public string ITEM_APPLY_ON { get; set; }
        public string BRAND_NAME { get; set; }
        public string PART_NUMBER { get; set; }
        public string ITEM_SPECIFICATION { get; set; }
        public string INTERFACE { get; set; }
        public string COLOR { get; set; }
        public string LAMINATION { get; set; }
        public string GRADE { get; set; }
        public string TYPE { get; set; }
        public string ITEM_SIZE { get; set; }
        public decimal? SIZE_LENGHT { get; set; }
        public decimal? SIZE_WIDTH { get; set; }
        public decimal? REEM_WEIGHT_KG { get; set; }
        public string REMARKS { get; set; }
        public string pre_item_code { get; set; }
        public string item_code { get; set; }
        public string Voucher_No { get; set; }
        public string invoice_no { get; set; }
        public string QC_NO { get; set; }
        public string MANUAL_NO { get; set; }
        public string supplier_edesc { get; set; }
        public decimal quantity { get; set; }
        public string to_location_code { get; set; }
        //public decimal? thickness { get; set; }
        public string thickness { get; set; }
        public string RollDiameter { get; set; }
        public string PH { get; set; }
        public string UNPLEASANT_SMELL_ODOUR { get; set; }
        public string Dust_Dirt { get; set; }
        public string Damaging_Material { get; set; }
        public string Core_Damaging { get; set; }
        public string GSM { get; set; }
        public string Tensile_CD { get; set; }
        public string Tensile_MD { get; set; }
        public string Visual_Inspection { get; set; }
        public Nullable<System.DateTime> RECEIPT_DATE { get; set; }
        public Nullable<System.DateTime> QC_DATE { get; set; }
        public string MITI { get; set; }
        public string GATE_NO { get; set; }
        public Nullable<System.DateTime> GATE_DATE { get; set; }
        public string GRN_NO { get; set; }
        public Nullable<System.DateTime> GRN_DATE { get; set; }
        public string CreatedBy { get; set; }
        public string Thickness1 { get; set; }
        public string Thickness2 { get; set; }
        public string Thickness3 { get; set; }
        public string RollDiameter1 { get; set; }
        public string RollDiameter2 { get; set; }
        public string RollDiameter3 { get; set; }
        public string PH1 { get; set; }
        public string PH2 { get; set; }
        public string PH3 { get; set; }
        public string UnpleasantSmell1 { get; set; }
        public string UnpleasantSmell2 { get; set; }
        public string UnpleasantSmell3 { get; set; }
        public string DustDirt1 { get; set; }
        public string DustDirt2 { get; set; }
        public string DustDirt3 { get; set; }
        public string DamagingMaterial1 { get; set; }
        public string DamagingMaterial2 { get; set; }
        public string DamagingMaterial3 { get; set; }
        public string CoreDamaging1 { get; set; }
        public string CoreDamaging2 { get; set; }
        public string CoreDamaging3 { get; set; }
        public string Width1 { get; set; }
        public string Width2 { get; set; }
        public string Width3 { get; set; }
        public string GSM1 { get; set; }
        public string GSM2 { get; set; }
        public string GSM3 { get; set; }
        public string TensileCD1 { get; set; }
        public string TensileCD2 { get; set; }
        public string TensileCD3 { get; set; }
        public string TensileMD1 { get; set; }
        public string TensileMD2 { get; set; }
        public string TensileMD3 { get; set; }
        public string VisualInspection1 { get; set; }
        public string VisualInspection2 { get; set; }
        public string VisualInspection3 { get; set; }
        public string Remarks1 { get; set; }
        public string Remarks2 { get; set; }
        public string Remarks3 { get; set; }
        public string Remarks4 { get; set; }
        public string Remarks5 { get; set; }
        public string Remarks6 { get; set; }
        public string Remarks7 { get; set; }
        public string Remarks8 { get; set; }
        public string Remarks9 { get; set; }
        public string Remarks10 { get; set; }
        public string Remarks11 { get; set; }
        public string Remarks12 { get; set; }
        public string MATERIAL_CODE { get; set; }
        public string Thickness0 { get; set; }
        public string RollDiameter0 { get; set; }
    }
}
