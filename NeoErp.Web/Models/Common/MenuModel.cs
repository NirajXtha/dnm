using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.OracleClient;
using NeoErp.Core.Models;
using NeoErp.Data;

namespace NeoErp.Models.Common
{
    public class MenuModel : IMenuModel
    {
        private NeoErpCoreEntity _objectEntity;
        private IDbContext _dbContext;
        public MenuModel(NeoErpCoreEntity objectEntity, IDbContext dbContext)
        {
            this._objectEntity = objectEntity;
            this._dbContext = dbContext;
        }
        public string menu_code { get; set; }
        public string desc_eng { get; set; }
        public string desc_loc { get; set; }
        public string icon { get; set; }
        public string module { get; set; }
        public string url { get; set; }
        public string leaf { get; set; }
        public int menu_level { get; set; }
        public int order_no { get; set; }

        public string menu_name { get; set; }

        public string module_code { get; set; }

        public string PrefMenu { get; set; }
        public string GetModule()
        {
            string MenuModule = null;
            string MainMenuModule = null;
            string PrefMenu = null;
            string ModuleSql = "SELECT MODULE_CODE,MODULE_NAME FROM MODULE_MASTER_SETUP WHERE ACTIVE='A' order by Order_no";
            var modules = _dbContext.SqlQuery<Modeuls>(ModuleSql);
            foreach (var module in modules)
            {
                MenuModule = "<li><a href='#'><i class='fa fa-wrench fa-fw'></i>" + module.MODULE_NAME + "<span class='fa arrow'></span></a><ul class='nav nav-second-level'> ";
                string MenuSql = "SELECT MENU_CODE,MENU_NAME,URL FROM MENU_MASTER_SETUP WHERE ACTIVE='A' AND MODULE_CODE = '" + module.MODULE_CODE + "' order by MENU_CODE";
                var menus = _dbContext.SqlQuery<Menus>(MenuSql);
                foreach (var menu in menus)
                {
                    PrefMenu = "<li><li><a href='" + menu.URL + "'><i class='fa'></i>" + menu.MENU_NAME + "</a></li><div class='sidebar-nav'><ul class='nav nav-third-level id='side-menu'> ";
                    string prefsql = null;
                    if (menu.MENU_NAME == "Trial Balance")
                    {
                        prefsql = "SELECT p.pref_code,p.Pref_name,m.menu_code FROM trialbalance_pref_master p, menu_master_setup m where m.menu_code = '" + menu.MENU_CODE + "' and isdefault = 'Y' order by pref_code";
                        var customMenus = _dbContext.SqlQuery<CustomMenus>(prefsql);
                        foreach(var customMenu in customMenus)
                        {
                            PrefMenu = PrefMenu + "<li><a href='" + menu.URL + "?Ah=" + customMenu.pref_code + "'>" + customMenu.Pref_name + "</a></li>";
                        }
                        PrefMenu = PrefMenu + "</ul></div></li>";

                        MenuModule = MenuModule + PrefMenu;
                    }
                    else
                    {
                        MenuModule = MenuModule + "<li><a href='" + menu.URL + "'>" + menu.MENU_NAME + "</a></li>";

                    }
                }
                MenuModule = MenuModule + "</ul></li>";
                MainMenuModule = MainMenuModule + MenuModule;
            }
          
            return MainMenuModule;
        }
        public string GetMenu(string ModuleCode)
        {
            string Menu = "";
            string Query = "select menu_code,menu_name,url from menu_master_setup where active='A' and module_code='" + ModuleCode + "'";
            DataSet ds = OracleHelper.ExecuteDataset(OracleHelper.GetConnection(), CommandType.Text, Query);
            if (ds.Tables[0].Rows.Count != 0)
            {
                Menu = Menu + "<ul>";
            }
            foreach (DataRow ro in ds.Tables[0].Rows)
            {
                Menu = Menu + "<li>";
                if (ro["url"].ToString().Replace(" ", "") != "")
                    Menu = Menu + "<a href='" + ro["url"].ToString() + "'>";
                else
                    Menu = Menu + "<a href='#'>";
                Menu = Menu + ro["menu_name"].ToString();
                Menu = Menu + "</a>";
                Menu = Menu + "</li>";
            }
            if (ds.Tables[0].Rows.Count != 0)
            {
                Menu = Menu + "</ul>";
            }
            return Menu;
        }
        public List<Menus> GetMenuList()
        {
            string MenuSql = "SELECT MENU_CODE,MENU_NAME,URL FROM MENU_MASTER_SETUP WHERE ACTIVE='A' order by MENU_CODE";
            return _dbContext.SqlQuery<Menus>(MenuSql).ToList();
            

        }
    }

    public class Modeuls
    {
        public string MODULE_CODE { get; set; }
        public string MODULE_NAME { get; set; }
        // MODULE_CODE,MODULE_NAME

    }

    public class Menus
    {
        public int MENU_CODE { get; set; }
        public string MENU_NAME { get; set; }
        public string URL { get; set; }

    }
    public class CustomMenus
    {
        public int pref_code { get; set; }
        public string Pref_name { get; set; }
        public int menu_code { get; set;}
      
    }
   
}