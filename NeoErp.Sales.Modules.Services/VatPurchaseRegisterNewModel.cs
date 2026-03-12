using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services
{
  public  class VatPurchaseRegisterNewModel
    {
        public DateTime? INVOICE_DATE { get; set; }
        public string MITI { get; set; }
        public string INVOICE_NO { get; set; }
        public string SUPPLIER_INV_NO { get; set; }
        public DateTime? SUPPLIER_INV_DATE { get; set; }
        public string PP_NO { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_EDESC { get; set; }
        public string TPIN_VAT_NO { get; set; }
        public string P_TYPE { get; set; }
        public string VEHICLE_PER_FLAG { get; set; }
        public string FORM_CODE { get; set; }
        public string ACC_IN_VOUCHER { get; set; }
        public double? INVOICE_TOTAL_PRICE { get; set; }
        public double? VAT_TOTAL_PRICE { get; set; }
        public double? TAXFREE_PURCHASE { get; set; }
        public double? TAXABLE_PURCHASE { get; set; }
        public double? TAXABLE_VAT { get; set; }
        public double? TAXABLE_PURCHASE_IMPORT { get; set; }
        public double? TAXABLE_VAT_IMPORT { get; set; }
        public double? TAX_PUR_IMP_CAP { get; set; }
        public double? TAX_VAT_IMP_CAP { get; set; }
  }
    public class VatPurchaseRegisterNewViewModel
    {
        public VatPurchaseRegisterNewViewModel()
        {
            VatPurchaseNew = new List<VatPurchaseRegisterNewModel>();
            AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public List<VatPurchaseRegisterNewModel> VatPurchaseNew { get; set; }

        public Dictionary<string, AggregationModel> AggregationResult { get; set; }

        public int Total { get; set; }
    }

}
