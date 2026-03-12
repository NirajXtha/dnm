using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NeoErp.Core.Models.CustomModels
{
    public class MenuSettingsEntities
    {

       // public int SN { get; set; }

        public string MENU_NO { get; set; }

        public string MENU_EDESC { get; set; }

        public string MENU_NDESC { get; set; }

        public string MODULE_EDESC { get; set; }

        public string MENU_OBJECT_NAME { get; set; }

        public bool IsEdit { get; set; } = true;

        public string MODULE_CODE { get; set; }

        public string FORM_CODE { get; set; }

        public string SALES_FLAG { get; set; }

        public string PRODUCTION_FLAG { get; set; }

        public string INVENTORY_FLAG { get; set; }

        public string FINANCIAL_FLAG { get; set; }

        public string PRE_MENU_NO { get; set; }

        public string GROUP_SKU_FLAG { get; set; }

        public string ICON_PATH { get; set; }

        public string VIRTUAL_PATH { get; set; }
        public string FULL_PATH { get; set; }

        public string REPORT_NO { get; set; }
        public string ANALYSIS_REPORT_FLAG { get; set; }
        public string SYN_ROWID { get; set; }
        public string DELETED_FLAG { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public string COMPANY_CODE { get; set; }

        public string SPOTLIGHT_FLAG { get; set; }
        public string MODIFY_BY { get; set; }
        public string DASHBOARD_FLAG { get; set; }

        public string FIXED_ASSETS_FLAG { get; set; }
        public string CONTROL_FLAG { get; set; }
        public DateTime MODIFY_DATE { get; set; }
        public string MIS_FLAG { get; set; }

        public int LEVEL { get; set; }
        public string MASTER_MENU_CODE { get; set; }
        public bool hasMenus { get; set; }
        public IEnumerable<MenuTree> Items { get; set; }

        public DbSet<MenuSettingsEntities> Menus { get; set; }

        public string MODULE_ABBR { get; set; }
        public string COLOR { get; set; }
        public string DESCRIPTION { get; set; }

    }

    public class MenuTree
    {
        public string menuName { get; set; }
        public string menuNo { get; set; }
        public int Level { get; set; }
        public bool hasMenus { get; set; }
        public string masterMenuCode { get; set; }
        public string preMenuNo { get; set; }
        public IEnumerable<MenuTree> Items { get; set; }
    }

    public class MenuSetupModel
    {
        public int LEVEL { get; set; }
        public string MENU_EDESC { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string MENU_NO { get; set; }
        public string MASTER_MENU_CODE { get; set; }
        public string PRE_MENU_NO { get; set; }
        public string BRANCH_CODE { get; set; }
        public string VIRTUAL_PATH { get; set; }
        public string FULL_PATH { get; set; }
        public string MODULE_CODE { get; set; }
        public int? Childrens { get; set; }
    }
    public class SammyEntity
    {

        public string VIRTUAL_PATH { get; set; }

        public string FULL_PATH { get; set; }
    }
    public class MenuItemModel
    {
        public string MENU_EDESC { get; set; }
        public string MENU_NO { get; set; }
        public string VIRTUAL_PATH { get; set; }
    }
}
