using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model
{
  public class AreaWiseDistributorModel
    {
        public string AreaName { get; set; }
        public string AreaCode { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string code { get; set; }
        public string Address { get; set; }
        public String ACTIVE { get; set; }
        public  String CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string full_name { get; set; }
        public string source { get; set; }
        public String WHOLESELLER { get; set; }


    }

    public class DivisionWiseCreditLimitModel
    {
         public string CUSTOMER_EDESC { get; set; }
         public string DIVISION_EDESC { get; set; }
         public string CUSTOMER_CODE  {get;set;}
         public int? CREDIT_LIMIT   {get;set;}
         public string REMARKS        {get;set;}
         public string COMPANY_CODE   {get;set;}
         public string DIVISION_CODE  {get;set;}
         public string CREATED_BY     {get;set;}
         public DateTime? CREATED_DATE   {get;set;}
         public string DELETED_FLAG   {get;set;}
         public string CURRENCY_CODE  {get;set;}
         public int? EXCHANGE_RATE  {get;set;}
         public DateTime? MODIFY_DATE    {get;set;}
         public string MODIFY_BY      {get;set;}
         public string BLOCK_FLAG { get; set; }
        public decimal? UTI { get; set; }
        public decimal? BALANCE { get; set; }
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
        public string SUB_EDESC { get; set; }
        public Decimal? OPEN_AMOUNT { get; set; }
        public string flag { get; set; }
        public string Account { get; set; }
    }

}
