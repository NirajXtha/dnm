
using NeoErp.Core;
using NeoErp.Data;
using NeoErp.LOC.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoErp.LOC.Services.Services
{
    public class LocRegister : ILocRegister
    {
        private IDbContext _objectEntity;
        private IWorkContext _workContext;
        public LocRegister(IDbContext objectEntity, IWorkContext workContext)
        {
            this._objectEntity = objectEntity;
            this._workContext = workContext;
        }
        public List<DynamicMenu> GetDynamicMenu(int userId, int level)
        {
            //= new List<DynamicMenu>();
            var companyCode = _workContext.CurrentUserinformation.company_code;
            //  string query = $@"SELECT MENU_NO,VIRTUAL_PATH,MENU_EDESC,GROUP_SKU_FLAG,ICON_PATH,MODULE_ABBR,COLOR FROM WEB_MENU_MANAGEMENT WHERE MODULE_CODE = '02' AND DELETED_FLAG = 'N' ORDER BY MENU_NO ASC";
            string query = $@"SELECT MENU_NO,VIRTUAL_PATH,MENU_EDESC,GROUP_SKU_FLAG,ICON_PATH,MODULE_ABBR,COLOR FROM WEB_MENU_MANAGEMENT WHERE MODULE_CODE = '02' AND DELETED_FLAG = 'N' ORDER BY MENU_NO ASC";
            //AND MENU_NO IN(SELECT MENU_NO FROM WEB_MENU_CONTROL WHERE ACCESS_FLAG='Y' AND COMPANY_CODE = '{companyCode}' AND USER_NO='{userId}') ORDER BY MENU_NO ASC";
            try
            {
                var dynamicMenu = _objectEntity.SqlQuery<DynamicMenu>(query).ToList();
                var module_code = "02";
                foreach (var item in dynamicMenu)
                {
                    var itemList = new List<DynamicMenu>();
                    itemList = GetChlidMenu(item.MENU_NO);
                    //item.Items.AddRange(itemList);
                    item.Items = itemList;
                }
                return dynamicMenu;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public List<DynamicMenu> GetChlidMenu(string menuNo)
        {
            var dynamicMenu = new List<DynamicMenu>();
            string query = "SELECT MENU_NO, VIRTUAL_PATH, MENU_EDESC, GROUP_SKU_FLAG, ICON_PATH FROM WEB_MENU_MANAGEMENT WHERE PRE_MENU_NO=" + menuNo + " AND DELETED_FLAG = 'N'";
            try
            {
                dynamicMenu = _objectEntity.SqlQuery<DynamicMenu>(query).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
            return dynamicMenu;
        }

        public List<DynamicMenu> getmasterDynamicMenuData()
        {
            var dynamicMenu = new List<DynamicMenu>();
            var companyCode = _workContext.CurrentUserinformation.company_code;
            var userId = _workContext.CurrentUserinformation.User_id;
            string query = $@"SELECT MENU_NO,VIRTUAL_PATH,MENU_EDESC,GROUP_SKU_FLAG,ICON_PATH,MODULE_ABBR,COLOR FROM WEB_MENU_MANAGEMENT WHERE MODULE_CODE = '02' AND DELETED_FLAG = 'N' AND MENU_NO = '15' ORDER BY MENU_NO ASC";
            // string query = $@"SELECT MENU_NO,VIRTUAL_PATH,MENU_EDESC,GROUP_SKU_FLAG,ICON_PATH,MODULE_ABBR,COLOR FROM WEB_MENU_MANAGEMENT WHERE MODULE_CODE = '02' AND DELETED_FLAG = 'N' AND MENU_NO = '15' AND EXISTS(SELECT MENU_NO FROM WEB_MENU_CONTROL WHERE  ACCESS_FLAG='Y' AND COMPANY_CODE = '{companyCode}' AND USER_NO='{userId}'AND MENU_NO = '15') ORDER BY MENU_NO ASC";
            try
            {
                dynamicMenu = _objectEntity.SqlQuery<DynamicMenu>(query).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
            return dynamicMenu;
        }

        public List<DynamicMenu> getlcDynamicMenuData()
        {
            var dynamicMenu = new List<DynamicMenu>();
            var companyCode = _workContext.CurrentUserinformation.company_code;
            var userId = _workContext.CurrentUserinformation.User_id;
            //  string query = $@"SELECT MENU_NO,VIRTUAL_PATH,MENU_EDESC,GROUP_SKU_FLAG,ICON_PATH,MODULE_ABBR,COLOR FROM WEB_MENU_MANAGEMENT WHERE MODULE_CODE = '02' AND DELETED_FLAG = 'N' AND MENU_NO IN('16','17','18','19','20','21','22','23','24') ORDER BY MENU_NO ASC";
            string query = $@"SELECT MENU_NO,VIRTUAL_PATH,MENU_EDESC,GROUP_SKU_FLAG,ICON_PATH,MODULE_ABBR,COLOR FROM WEB_MENU_MANAGEMENT WHERE MODULE_CODE = '02' AND DELETED_FLAG = 'N' AND MENU_NO IN('16','17','18','19','20','21','22','23','24') ORDER BY MENU_NO ASC";
            //string query = $@"SELECT MENU_NO,VIRTUAL_PATH,MENU_EDESC,GROUP_SKU_FLAG,ICON_PATH,MODULE_ABBR,COLOR FROM WEB_MENU_MANAGEMENT WHERE MODULE_CODE = '02' AND DELETED_FLAG = 'N' AND MENU_NO IN('16','17','18','19','20','21','22','23','24') AND MENU_NO IN(SELECT MENU_NO FROM WEB_MENU_CONTROL WHERE ACCESS_FLAG='Y' AND COMPANY_CODE = '{companyCode}' AND USER_NO='{userId}')  ORDER BY MENU_NO ASC";
            //AND MENU_NO IN(SELECT MENU_NO FROM WEB_MENU_CONTROL WHERE ACCESS_FLAG='Y' AND COMPANY_CODE = '{companyCode}' AND USER_NO='{userId}')  ORDER BY MENU_NO ASC";
            try
            {
                dynamicMenu = _objectEntity.SqlQuery<DynamicMenu>(query).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
            return dynamicMenu;
        }

        public List<DynamicMenu> getlcReportsDynamicMenuData()
        {
            var dynamicMenu = new List<DynamicMenu>();
            var companyCode = _workContext.CurrentUserinformation.company_code;
            var userId = _workContext.CurrentUserinformation.User_id;
            string query = $@"SELECT MENU_NO,VIRTUAL_PATH,MENU_EDESC,GROUP_SKU_FLAG,ICON_PATH,MODULE_ABBR,COLOR FROM WEB_MENU_MANAGEMENT WHERE MODULE_CODE = '02' AND DELETED_FLAG = 'N' AND MENU_NO LIKE('%25.%')  ORDER BY MENU_NO ASC";
            //   string query = $@"SELECT MENU_NO,VIRTUAL_PATH,MENU_EDESC,GROUP_SKU_FLAG,ICON_PATH,MODULE_ABBR,COLOR FROM WEB_MENU_MANAGEMENT WHERE MODULE_CODE = '02' AND DELETED_FLAG = 'N' AND  MENU_NO LIKE('%25.%') AND MENU_NO IN(SELECT MENU_NO FROM WEB_MENU_CONTROL WHERE ACCESS_FLAG='Y' AND COMPANY_CODE = '{companyCode}' AND USER_NO='{userId}')  ORDER BY MENU_NO ASC";
            try
            {
                dynamicMenu = _objectEntity.SqlQuery<DynamicMenu>(query).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
            return dynamicMenu;
        }
    }
}