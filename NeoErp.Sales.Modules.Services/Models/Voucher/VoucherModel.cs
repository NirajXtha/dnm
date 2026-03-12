using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.Voucher
{
    public  class VoucherModel
    {
        public string SubCode { get; set; }
        public string AccCode { get; set; }
        public string SubEdesc { get; set; }
       
    }

    public class VoucherDetailModel
    {
        public DateTime voucher_date { get; set; }
        public string Miti { get; set; }
        public string manual_no { get; set; }
        public decimal? Balance { get; set; }
        public string BalanceHeader { get; set; }
        public string Voucher_no { get; set; }
        public string PARTICULARS { get; set; }
        public decimal? dr_amount { get; set; }
        public decimal? cr_amount { get; set; }
        public string  SUB_EDESC { get; set; }
        public Decimal?  OPEN_AMOUNT { get; set; }
        public string flag { get; set; }
    }

    public class VoucherDetailReportModal
    {
        public decimal sno { get; set; }
        public string uid { get; set; }
        public string VoucherNumber { get; set; }

        //
        public DateTime voucher_date { get; set; }
        public string Miti { get; set; }
        public string manual_no { get; set; }
        public decimal? Balance { get; set; }
        public string BalanceHeader { get; set; }
        public string Voucher_no { get; set; }
        public string PARTICULARS { get; set; }
        public Decimal? dr_amount { get; set; }
        public Decimal? cr_amount { get; set; }
        public string ACC_EDESC { get; set; }
        public string TRANSACTION_TYPE { get; set; }
        public string CHEQUE_NO { get; set; }
    }

    public class CUSTOMTEMPLATEFORPL
    {
        public DateTime voucher_date { get; set; }
        public string Miti { get; set; }
        public string SEGMENT_NAME { get; set; }
        public Double? Balance { get; set; }
        public string OPERATE_TYPE { get; set; }
        public string TEXT_FORMAT { get; set; }
        public string TEXT_COLOR { get; set; }
        public string TEXT_LINE_FORMAT { get; set; }
        public string ACCOUNT_FORMULA { get; set; }
        public decimal? dr_amount { get; set; }
        public decimal? cr_amount { get; set; }
       public string PL_CODE { get; set; }
        public string PL_EDESC { get; set; }
        public string INDEXING { get; set; }
        public string REMARKS { get; set; }
        public string CL_FLAG { get; set; }
        public string FG_CL_FLAG { get; set; }
        public string END_SEGMENT { get; set; } = "N";
        public List<ChildLedgerSummary> ChildSummary { get; set; }

        public string SEGMENT_CODE { get; set; }

        public String OP_FLAG { get; set; }
        public String FG_OP_FLAG { get; set; }

        public CUSTOMTEMPLATEFORPL()
        {
            ChildSummary = new List<ChildLedgerSummary>();
        }

    }

    public class ChildLedgerSummary
    {
        public string ACC_EDESC { get; set; }
        public string ACC_CODE { get; set; }
        public double Balance { get; set; }
        
    }
}
