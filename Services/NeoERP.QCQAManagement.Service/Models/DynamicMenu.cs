using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QCQAManagement.Service.Models
{
    public class DynamicMenu
    {
        public string MENU_EDESC { get; set; }

        public string MENU_NO { get; set; }

        public string VIRTUAL_PATH { get; set; }

        public string FULL_PATH { get; set; }

        public string GROUP_SKU_FLAG { get; set; }

        public string ICON_PATH { get; set; }
        public string MODULE_CODE { get; set; }

        public List<DynamicMenu> Items { get; set; }

        public string MODULE_ABBR { get; set; }
        public string COLOR { get; set; }
        public string DESCRIPTION { get; set; }

        public string DASHBOARD_FLAG { get; set; }
        public List<SubMenu> SubMenuList { get; set; }
    }
    public class SubMenu
    {
        public string FORM_CODE { get; set; }
        public string FORM_EDESC { get; set; }
        public string TABLE_NAME { get; set; }
    }
    public class QCQASubMenu
    {
        public string Form_CODE { get; set; }
        public string VOUCHER_NO { get; set; }
        public DateTime? VOUCHER_DATE { get; set; }
        public decimal? VOUCHER_AMOUNT { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string CHECKED_BY { get; set; }
        public DateTime? CHECKED_DATE { get; set; }
        public string AUTHORISED_BY { get; set; }
        public DateTime? POSTED_DATE { get; set; }
        public DateTime? MODIFY_DATE { get; set; }
        public string SYN_ROWID { get; set; }
        public string REFERENCE_NO { get; set; }
        public string SESSION_ROWID { get; set; }
        public string MODULE_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string VEHICLE_NO { get; set; }
        public string PARTY_NAME { get; set; }
        public string ADDRESS { get; set; }
        public string BILL_NO { get; set; }

        //  public String 


    }
}

