using NeoErp.Core;
using NeoErp.Core.Caching;
using NeoErp.LOC.Services.Models;
using NeoErp.LOC.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoERP.LOC.Controllers.Api
{
    public class LocHomeController : ApiController
    {
        public ILocRegister _locRegister { get; set; }
        private IWorkContext _workContext;
        private ICacheManager _cacheManager;
        public LocHomeController(ILocRegister locRegister, ICacheManager cacheManger, IWorkContext workContext)
        {
            this._locRegister = locRegister;
            this._workContext = workContext;
            this._cacheManager = cacheManger;
        }
        [HttpGet]
        public List<DynamicMenu> GetDynamicMenu()
        {
            var menuList = new List<DynamicMenu>();

            var level = 1;
            var userId = _workContext.CurrentUserinformation.User_id;
            menuList = _locRegister.GetDynamicMenu(userId, level);
            foreach (var item in menuList)
            {
                var itemList = new List<DynamicMenu>();
                itemList = _locRegister.GetChlidMenu(item.MENU_NO);
                //item.Items.AddRange(itemList);
                item.Items = itemList;
            }
            return menuList;
        }

        [HttpGet]
        public List<DynamicMenu> getmasterDynamicMenuData()
        {
           var result =  _locRegister.getmasterDynamicMenuData();
            return result;
        }

        [HttpGet]
        public List<DynamicMenu> getlcDynamicMenuData()
        {
            var result = _locRegister.getlcDynamicMenuData();
            return result;
        }

        [HttpGet]
        public List<DynamicMenu> getlcReportsDynamicMenuData()
        {
            var result = _locRegister.getlcReportsDynamicMenuData();
            return result;
        }

    }
}
