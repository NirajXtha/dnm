using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models
{
    public class ReceiptScheduleModel
    {
        public string CustomerName { get; set; }
        public string Voucher_No { get; set; }
        public string Manual_No { get; set; }
        public DateTime Voucher_Date { get; set; }
        public int? Credit_Days { get; set; }
        public DateTime DueDate { get; set; }
        public decimal? BillAmount { get; set; }
        public decimal? RecAmount { get; set; }
        public decimal? BalanceAmount { get; set; }
        public int? OverDueDays { get; set; }
    }

    public class ReceiptScheduleViewModel
    {
        public List<ReceiptScheduleModel> ReceiptScheduleModel { get; set; }
        public decimal total { get; set; }       
        public ReceiptScheduleViewModel()
        {
            ReceiptScheduleModel = new List<ReceiptScheduleModel>();
            this.AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
    }



    public class PaymentScheduleModel
    {
        public string SupplierName { get; set; }
        public string Voucher_No { get; set; }
        public string Manual_No { get; set; }
        public DateTime Voucher_Date { get; set; }
        public int Credit_Days { get; set; }
        public DateTime DueDate { get; set; }
        public decimal? BillAmount { get; set; }
        public decimal? RecAmount { get; set; }
        public decimal? BalanceAmount { get; set; }
        public int? OverDueDays { get; set; }
    }

    public class PaymentScheduleViewModel
    {
        public List<PaymentScheduleModel> PaymentScheduleModel { get; set; }
        public decimal total { get; set; }
        public PaymentScheduleViewModel()
        {
            PaymentScheduleModel = new List<PaymentScheduleModel>();
            this.AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
    }
}
