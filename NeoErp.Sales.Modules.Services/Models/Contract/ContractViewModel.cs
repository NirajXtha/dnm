using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.Contract
{
   public class ContractViewModel
    {
        public DateTime? service_start_date { get; set; }
        public  DateTime? EXPIRY_DATE { get; set; }
        public  string BsStartDate { get; set; }
        public string CONTRACT_NO { get; set; }
        public DateTime? CONTRACT_DATE { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string PRE_CUSTOMER_EDESC { get; set; }
        public string BANDWIDTH { get; set; }
        public string CONTRACT_TYPE { get; set; }
        public string DISTRICT_CODE { get; set; }
        public string DISTRICT_EDESC { get; set; }
        public string CITY_CODE { get; set; }
        public string CITY_EDESC { get; set; }
        public string TECH_CONTACT_PERSON { get; set; }
        public string TECH_CONTACT_ADD { get; set; }
        public string TECH_CONTACT_NO { get; set; }
        public string PAYMENT_LOCATION { get; set; }
        public string HOSTNAME { get; set; }
        public string PAYMENT_BASIS { get; set; }
        public string STATUS_TYPE { get; set; }
        public string REMARKS { get; set; }
        public decimal? RE_PAYMENT_AMOUNT { get; set; }
        public decimal? BUSINESS_SECTOR { get; set; }
        public string BUSINESS_SECTOR_EDESC { get; set; }

        public String CONNECTION_TYPE_EDESC { get; set; }

        public string CUSTOMER_ID { get; set; }

        public decimal Shrawan { get; set; }
        public decimal Bhadra { get; set; }
        public decimal Ashwin { get; set; }
        public decimal Kartik { get; set; }

        public decimal Mangsir { get; set; }
        public decimal Poush { get; set; }

        public decimal Magh { get; set; }
        public decimal Falgun { get; set; }
        public decimal Chaitra { get; set; }
        public decimal Baisakh { get; set; }

        public decimal Jestha { get; set; }
        public decimal Ashadh { get; set; }

        public decimal TotalAmount { get; set; }
 

    }
    public class ContractLandingReportModel
    {
        public List<ContractViewModel> items { get; set; }
        public int total { get; set; }

        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
        public ContractLandingReportModel()
        {
            items = new List<ContractViewModel>();
            this.AggregationResult = new Dictionary<string, AggregationModel>();
        }

    }
}
