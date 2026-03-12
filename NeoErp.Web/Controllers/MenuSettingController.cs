using NeoErp.Core.Caching;
using NeoErp.Core.Models.CustomModels;
using NeoErp.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoErp.Controllers
{
    public class MenuSettingController : Controller
    {
        public IMenuSettings _menuSettings { get; set; }
        private ICacheManager _cacheManger { get; set; }
        public MenuSettingController(IMenuSettings menuSettings,ICacheManager cacheManager)
        {
            this._menuSettings = menuSettings;
            this._cacheManger = cacheManager;
        }
        // GET: MenuSetting
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult MenuSettings()
        {
            //var menuSettings = _menuSettings.GetMenuForSettings();
            //return PartialView(menuSettings);
            var menuList = _menuSettings.GetMenuForSettings();
            return Json(menuList, JsonRequestBehavior.AllowGet);
            //return View();
        }
        //GET: Menu by MenuNo
        public ActionResult GetMenuByMenuNo(string menuNo)
        {
            var menuByID = _menuSettings.GetMenuForSettings(menuNo);
            //if (menuByID.GROUP_SKU_FLAG == "G")
            //{
            //    menuByID.isGroupSkuFlag = true;
            //}
            //else
            //{
            //    menuByID.isGroupSkuFlag = false;
            //}
            return Json(menuByID, JsonRequestBehavior.AllowGet);
        }
        //Update Menu
        public string UpdateMenu(MenuSettingsEntities menu)
        {
            if (menu != null)
            {
                if (menu.MENU_OBJECT_NAME.Contains("chart"))
                { 
                var QueryCacheName = $"neocacheQuery_{menu.MENU_OBJECT_NAME}";
                this._cacheManger.Remove(QueryCacheName);
            }
                var _menu = _menuSettings.UpdateMenuForSettings(menu);
                //View("~/Views/MenuSetting/index.cshtml");
                return "Menu record updated successfully";
            }
            else
            {
                return "Invalid Menu record";
            }
        }
        // Add Menu
        public string AddMenu(MenuSettingsEntities menu)
        {
            if (menu != null)
            {               
                string menuNo = _menuSettings.CreateMenuForSettings(menu);
                //View("~/Views/MenuSetting/index.cshtml");
                return menuNo;
            }
            else
            {
                return "Invalid menu record";
            }
        }
        // Delete Menu
        public string DeleteMenu(string menuNo)
        {

            if (!String.IsNullOrEmpty(menuNo))
            {
                try
                {
                    var value = _menuSettings.DeleteMenuForSettings(menuNo);
                    if (value == "count")
                    {
                        return "Menu already assgin to the user.";
                    }
                    else {
                        return "Selected menu record deleted sucessfully";
                    }
                }
                catch (Exception)
                {
                    return "Menu details not found";
                }
            }
            else
            {
                return "Invalid operation";
            }
        }
        // POST: GetModule
        public JsonResult GetModule()
        {
            var moduleList = _menuSettings.GetModuleForMenu();
            return Json(moduleList, JsonRequestBehavior.AllowGet);
        }
        // POST: GetPreMenu
        public JsonResult GetPreMenu()
        {
            var preMenuList = _menuSettings.GetPreMenu();
            var a = Json(preMenuList, JsonRequestBehavior.AllowGet);
            return a;
        }

        public ActionResult Create()
        {
            return View();
        }
        //[HttpGet]
        //public List<MenuTree> GetAllMenusNodes()
        //{
        //    var allMenuList = _menuSettings.MenuListAllNodes().ToList();
        //    var menuNodes = new List<MenuTree>();

        //    foreach (var menu in allMenuList)
        //    {
        //        menuNodes.Add(new MenuTree()
        //        {
        //            Level = menu.LEVEL,
        //            MENU_EDESC  = menu.MENU_EDESC,
        //            MENU_NO = menu.MENU_NO,
        //            masterMenuCode = menu.MASTER_MENU_CODE,
        //            preMenuCode = menu.PRE_MENU_NO,
        //            hasMenus = menu.GROUP_SKU_FLAG == "G" ? true : false
        //        });
        //    }

        //    return menuNodes;
        //}
        //[HttpGet]
        //public List<MenuTree> GetAllMenusByMenuNo(string MENU_NO, string level, string masterCode)
        //{
        //    var allMenuList = _menuSettings.GetMenuListByMenuNo(level, masterCode).ToList();
        //    var menuNodes = new List<MenuTree>();

        //    foreach (var menu in allMenuList)
        //    {
        //        menuNodes.Add(new MenuTree()
        //        {
        //            Level = menu.LEVEL,
        //            MENU_EDESC = menu.MENU_EDESC,
        //            MENU_NO = menu.MENU_NO,
        //            masterMenuCode = menu.MASTER_MENU_CODE,
        //            preMenuCode = menu.PRE_MENU_NO,
        //            hasMenus = menu.GROUP_SKU_FLAG == "G" ? true : false
        //        });
        //    }

        //    return menuNodes;
        //}

    }
}