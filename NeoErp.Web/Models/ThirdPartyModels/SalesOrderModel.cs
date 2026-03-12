using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Models.ThirdPartyModels
{
    public class SalesOrderModel
    {
        public SalesOrderModel()
        {
            Items = new List<ItemOrderModel>();
        }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string OrderNumber { get; set; }
        public string OrderDate { get; set; }
        public string TransactionRefNo { get; set; }
        public string CustomerCode { get; set; }
        public string UserId { get; set; }  //CREATED_BY
        public string Remarks { get; set; }
        public List<ItemOrderModel> Items { get; set; }
    }
    public class ItemOrderModel
    {
        public int Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public string ItemId { get; set; }  //ITEM_CODE
        public string UnitId { get; set; }  //MU_CODE
    }
}