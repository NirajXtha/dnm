using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QCQAManagement.Service.Models
{
    public class DailyWastage
    {
        public string DAILYWASTAGE_NO { get; set; }
        public string MANUAL_NO { get; set; }
        public string COMPANY_CODE { get; set; }
        public string FORM_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string MODIFY_DATE { get; set; }
        public string MODIFY_BY { get; set; }
        public string SERIAL_NO { get; set; }
        public string PARAM_CODE { get; set; }
        public string BATCH_NO { get; set; }
        public string REMARKS { get; set; }
        public string PARAMETERS { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string UNIT { get; set; }
        public decimal QTY { get; set; }
        public List<DailyWastage> DailyWastageList { get; set; }
        public List<string> ITEMSETUPS { get; set; }
        public List<PRODUCTSLIST> PRODUCTS { get; set; }
        //public List<DailyWastageItemList> DailyWastageItemLists { get; set; } = new List<DailyWastageItemList>();
        //public List<DailyWastage> DailyWastageDetails { get; set; } = new List<DailyWastage>();
    }
    public class PRODUCTSLIST
    {
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
    }
    //public class DailyWastageItemList
    //{
    //    public string ITEM_CODE { get; set; }
    //    public string ITEM_EDESC { get; set; }
    //    public string DAY { get; set; }
    //    //public List<string> DailyWastageDetails { get; set; } = new List<string>();
    //}
    //public class DailyWastageDetail
    //{
    //    //public DateTime WASTAGE_DATE { get; set; }
    //    public String WASTAGE_VALUE { get; set; }
    //}
}
