using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NeoErp.Core.Models.CustomModels;
using NeoErp.Core.Services;

namespace NeoErp.Controllers.Api
{
    public class MenuSettingsController : ApiController
    {
        IMenuSettings _menuSettings;
        public MenuSettingsController(IMenuSettings _menuSettings) {
            this._menuSettings = _menuSettings;
        }
        [HttpGet]
        //public HttpResponseMessage MenuSettings()
        //{
        //    var menuList = _menuSettings.GetMenuForSettings();
        //    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, menuList);
        //    return response;
        //}
        ////GET: Menu by MenuNo
        //[HttpPost]
        //public HttpResponseMessage GetMenuByMenuNo(string menuNo)
        //{
        //    var menuByID = _menuSettings.GetMenuForSettings(menuNo);
        //    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, menuByID);
        //    return response;
        //}
        ////Update Menu
        //[HttpPost]
        //public HttpResponseMessage UpdateMenu(MenuSettingsEntities menu)
        //{
        //    if (menu != null)
        //    {
        //        var _menu = _menuSettings.UpdateMenuForSettings(menu);
        //        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, _menu);
        //        return response;
        //    }
        //    else
        //    {
        //        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.NoContent);
        //        return response;
        //    }
        //}
        //// Add Menu
        //[HttpPost]
        //public HttpResponseMessage AddMenu(MenuSettingsEntities menu)
        //{
        //    if (menu != null)
        //    {
        //        var menus = _menuSettings.CreateMenuForSettings(menu);
        //        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, menus);
        //        return response;
        //    }
        //    else
        //    {
        //        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.NoContent);
        //        return response;
        //    }
        //}
        //// Delete Menu
        //[HttpPost]
        //public HttpResponseMessage DeleteMenu(string menuNo)
        //{

        //    if (!String.IsNullOrEmpty(menuNo))
        //    {
        //        try
        //        {
        //           var menu = _menuSettings.DeleteMenuForSettings(menuNo);
        //            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, menu);
        //            return response;
        //        }
        //        catch (Exception ex)
        //        {
        //            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.ExpectationFailed, ex);
        //            return response;
        //        }
        //    }
        //    else
        //    {
        //        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.NoContent);
        //        return response;
        //    }
        //}
        //// POST: GetModule
        //[HttpPost]
        //public HttpResponseMessage GetModule()
        //{
        //    var moduleList = _menuSettings.GetModuleForMenu();
        //    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, moduleList);
        //    return response;
        //}




        public List<MenuSettingsEntities> GetMenuForSetting()
        {
            return this._menuSettings.GetMenuForSettings().ToList();
        }
        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        public string ChangeMenuOrder(List<MenuOrderModels> menuList)
        {
            return this._menuSettings.getChangeMenuOrder(menuList);
        }
        
        [HttpGet]
        public string Update()
        {
            return this._menuSettings.UpdateMenuForSettings();
        }
        [HttpGet]
        public string Create(string MENU_NO)
        {
            return this._menuSettings.CreateMenuForSettings();
        }
        [HttpGet]
        public string Delete()
        {
            return this._menuSettings.DeleteMenuForSettings();
        }
        [HttpGet]
        public List<MenuSettingsEntities> GetAllMenusNodes()
        {
            var allMenuList = _menuSettings.MenuListAllNodes().ToList();
            var menuNodes = new List<MenuSettingsEntities>();

            foreach (var menu in allMenuList)
            {
                menuNodes.Add(new MenuSettingsEntities()
                {
                    LEVEL = menu.LEVEL,
                    MENU_EDESC = menu.MENU_EDESC,
                    MENU_NO = menu.MENU_NO,
                    MASTER_MENU_CODE = menu.MASTER_MENU_CODE,
                    PRE_MENU_NO = menu.PRE_MENU_NO,
                    GROUP_SKU_FLAG = menu.GROUP_SKU_FLAG,
                    hasMenus = menu.GROUP_SKU_FLAG == "G" ? true : false,
                    FULL_PATH = menu.FULL_PATH,
                    VIRTUAL_PATH = menu.VIRTUAL_PATH,
                    MODULE_CODE = menu.MODULE_CODE
                });
            }

            return menuNodes;
        }
        [HttpGet]
        public List<MenuSettingsEntities> GetAllMenusByMenuNo(string menuNo, string level, string masterCode)
        {
            var allMenuList = _menuSettings.GetMenuListByMenuNo(level, masterCode).ToList();
            var menuNodes = new List<MenuSettingsEntities>();

            foreach (var menu in allMenuList)
            {
                menuNodes.Add(new MenuSettingsEntities()
                {
                    LEVEL = menu.LEVEL,
                    MENU_EDESC = menu.MENU_EDESC,
                    MENU_NO = menu.MENU_NO,
                    MASTER_MENU_CODE = menu.MASTER_MENU_CODE,
                    PRE_MENU_NO = menu.PRE_MENU_NO,
                    hasMenus = menu.GROUP_SKU_FLAG == "G" ? true : false,
                    GROUP_SKU_FLAG = menu.GROUP_SKU_FLAG,
                    FULL_PATH = menu.FULL_PATH,
                    VIRTUAL_PATH = menu.VIRTUAL_PATH,
                    MODULE_CODE = menu.MODULE_CODE
                });
            }

            return menuNodes;
        }

        public List<SammyEntity> GetMenuRouting(string moduleCode)
        {
            return this._menuSettings.GetMenuRouting(moduleCode);
        }

        [HttpGet]
        public List<MenuItemModel> GetMenuList()
        {
            return this._menuSettings.GetMenuList();
        }
    }
}
