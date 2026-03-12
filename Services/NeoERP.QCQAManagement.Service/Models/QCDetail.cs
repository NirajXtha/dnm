using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QCQAManagement.Service.Models
{
    public class QCDetail
    {
        public string Parameters { get; set; }
        public string TRANSACTION_NO { get; set; }
        public string FORM_CODE { get; set; }
        public string VOUCHER_NO { get; set; } // GRN NO OR GATE ENTRY NO
        public string ITEM_CODE { get; set; }
        public string QC_CODE { get; set; }
        public string QUANTITY { get; set; }
        public string INVOICE_NO { get; set; }
        public string VENDOR_NAME { get; set; }
        public string MANUAL_NO { get; set; }
        public DateTime QC_DATE { get; set; }
        public string LC_NO { get; set; }
        public DateTime GRN_DATE { get; set; }
        public string NAME { get; set; }
        public string MATERIAL_CODE { get; set; }
        public string Thickness0 { get; set; }
        public string Thickness1 { get; set; }
        public string Thickness2 { get; set; }
        public string Thickness3 { get; set; }
        public string RollDiameter0 { get; set; }
        public string RollDiameter1 { get; set; }
        public string RollDiameter2 { get; set; }
        public string RollDiameter3 { get; set; }
        public string PH0 { get; set; }
        public string PH1 { get; set; }
        public string PH2 { get; set; }
        public string PH3 { get; set; }
        public string UnpleasantSmell0 { get; set; }
        public string UnpleasantSmell1 { get; set; }
        public string UnpleasantSmell2 { get; set; }
        public string UnpleasantSmell3 { get; set; }
        public string DustDirt0 { get; set; }
        public string DustDirt1 { get; set; }
        public string DustDirt2 { get; set; }
        public string DustDirt3 { get; set; }
        public string DamagingMaterial0 { get; set; }
        public string DamagingMaterial1 { get; set; }
        public string DamagingMaterial2 { get; set; }
        public string DamagingMaterial3 { get; set; }
        public string CoreDamaging0 { get; set; }
        public string CoreDamaging1 { get; set; }
        public string CoreDamaging2 { get; set; }
        public string CoreDamaging3 { get; set; }
        public string Width0 { get; set; }
        public string Width1 { get; set; }
        public string Width2 { get; set; }
        public string Width3 { get; set; }
        public string GSM0 { get; set; }
        public string GSM1 { get; set; }
        public string GSM2 { get; set; }
        public string GSM3 { get; set; }
        public string TensileCD0 { get; set; }
        public string TensileCD1 { get; set; }
        public string TensileCD2 { get; set; }
        public string TensileCD3 { get; set; }
        public string TensileMD0 { get; set; }
        public string TensileMD1 { get; set; }
        public string TensileMD2 { get; set; }
        public string TensileMD3 { get; set; }
        public string VisualInspection0 { get; set; }
        public string VisualInspection1 { get; set; }
        public string VisualInspection2 { get; set; }
        public string VisualInspection3 { get; set; }
        public string Remarks { get; set; }
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
        public string SERIAL_NO { get; set; }
    }
    public class Specification
    {
        public string Name { get; set; }
        public object Value0 { get; set; }
        public object Value1 { get; set; }
        public object Value2 { get; set; }
        public object Value3 { get; set; }
        public object Remarks { get; set; }

        public Specification(string name,object value0, object value1, object value2, object value3, object remarks)
        {
            Name = name;
            Value0 = value0;
            Value1 = value1;
            Value2 = value2;
            Value3 = value3;
            Remarks = remarks;
        }
    }
}
