using NeoErp.Core.Models.CustomModels;
using NeoErp.Core.Services.MenuControlService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NeoErp.Core.Helpers;
using NeoErp.Core.Models;

namespace NeoErp.Controllers
{
    public class MenuControlController : Controller
    {
        IMenuControl _menuControl;
        public MenuControlController(IMenuControl menuControl)
        {
            this._menuControl = menuControl;
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult BulkControlIndex()
        {
            return View();
        }
       // GET: MenuControl
        public ActionResult GetMenuControl()
        {
            var menuList = _menuControl.GetAllMenuControls();
            var jsonResult = Json(menuList, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        [HttpPost]
        public ActionResult GetAllMenuControl(filterOption model)
        {
            var menuList = _menuControl.GetAllMenuControl(model.ReportFilters).ToList();
            var jsonResult = Json(menuList, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        //GET: Menu by MenuNo
        public ActionResult GetMenuControlByID(string menuNo, string userNo)
        {
            var menuControlByMenuNo = _menuControl.GetMenuControls(menuNo, userNo);
            return Json(menuControlByMenuNo, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetUserWiseBulkMenu(string userNo)
        {
            var menuControlByMenuNo = _menuControl.GetUserWiseBulkMenu(userNo);
            return Json(menuControlByMenuNo, JsonRequestBehavior.AllowGet);
        }

        //Update Menu Control
        public string UpdateMenuControl(MenuControlEntities menu)
        {
            if (menu != null)
            {
                var _menu = _menuControl.UpdateMenuControl(menu);
                if (string.IsNullOrEmpty(_menu))
                {
                    return "Menu Control record updated invalid";
                }
                else
                {
                    return "Menu Control record updated successfully";
                }
               
            }
            else
            {
                return "Invalid Menu record";
            }
        }
        // Add Menu Control
        public string AddMenuControl(MenuControlEntities menu)
        {
            if (menu != null)
            {
                _menuControl.AddNewMenuControl(menu);
                return "Menu Control record added successfully";
            }
            else
            {
                return "Invalid menu record";
            }
        }
        //Add Bulk Menu User
        public string AddBulkMenuUser(MenuControlEntities menu)
        {
            if (menu != null)
            {
                _menuControl.AddBulkMenuUser(menu);
                return "Menu Control record added successfully";
            }
            else
            {
                return "Invalid menu record";
            }
        }

        // Delete Menu Control
        public string DeleteMenuControl(string menuNo, string userNo)
        {

            if (!String.IsNullOrEmpty(menuNo))
            {
                try
                {
                    _menuControl.DeleteMenuControl(menuNo, userNo);
                    return "Selected menu control record deleted sucessfully";
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

        public JsonResult GetMenu()
        {
            var moduleList = _menuControl.GetMenus();
            return Json(moduleList, JsonRequestBehavior.AllowGet);
        }


        //public List<MenuModel> GetAllMenuByFilter(string filter)
        //{
        //    return _menuControl.GetAllMenuByFilter(filter);
        //}

        public JsonResult GetAllModuleByFilter(string filter)
        {
            var moduleList = _menuControl.GetAllModuleByFilter(filter);
            return Json(moduleList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllUserByFilter(string filter)
        {
            var moduleList = _menuControl.GetAllUserByFilter(filter);
            return Json(moduleList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllMenuByFilter(string filter)
        {
            var moduleList = _menuControl.GetAllMenuByFilter(filter);
            return Json(moduleList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetUser()
        {
            var moduleList = _menuControl.GetUsers();
            return Json(moduleList, JsonRequestBehavior.AllowGet);
        }
    }
}