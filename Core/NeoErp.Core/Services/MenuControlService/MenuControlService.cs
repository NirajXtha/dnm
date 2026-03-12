using NeoErp.Core.Models;
using NeoErp.Core.Models.CustomModels;
using NeoErp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Core.Services.MenuControlService
{
    public class MenuControlService : IMenuControl
    {
        private IDbContext _dbContext;
        private NeoErpCoreEntity _objectEntity;
        private IWorkContext _workContext;

        /// <summary>
        /// Cunstructor use to initialize IDBContext.
        /// </summary>
        /// <param name="dbContext"></param>
        public MenuControlService(IDbContext dbContext, NeoErpCoreEntity objectEntity, IWorkContext workContext)
        {
            _dbContext = dbContext;
            this._objectEntity = objectEntity;
            this._workContext = workContext;
        }

        /// <summary>
        /// Get All Menu Control List.
        /// </summary>
        /// <returns></returns>
        public List<MenuControlEntities> GetAllMenuControls()
        {
            var query = "SELECT C.MENU_NO, to_char(C.USER_NO) as USER_NO, (SELECT LOGIN_EDESC FROM SC_APPLICATION_USERS WHERE ROWNUM <= 1 AND C.USER_NO = USER_NO)LOGIN_EDESC,(SELECT MENU_EDESC FROM WEB_MENU_MANAGEMENT WHERE ROWNUM <= 1 AND C.MENU_NO = MENU_NO)MENU_EDESC ,C.COMPANY_CODE,D.COMPANY_EDESC,C.BRANCH_CODE FROM WEB_MENU_CONTROL C,COMPANY_SETUP D WHERE C.COMPANY_CODE =D.COMPANY_CODE  AND  ROWNUM <= 5";
            List<MenuControlEntities> menuControlList;
            try
            {
                menuControlList = _dbContext.SqlQuery<MenuControlEntities>(query).ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return menuControlList;
        }


        //

        //public List<PendingLcReportModels> GetAllPendingLcReports(ReportFiltersModel reportFilters)
        //{
        //    var sqlquery = $@"SELECT DISTINCT  LPI.PINVOICE_NO, LPI.PINVOICE_DATE,
        //                                        NVL(SUM(LI.QUANTITY),0) AS QUANTITY,
        //                                        NVL(SUM(LI.AMOUNT) ,0)AS AMOUNT,
        //                                       (NVL(SUM(LI.QUANTITY),0)*NVL(SUM(LI.AMOUNT) ,0))AS TOTAL_AMOUNT,
        //                                       LPI.LC_TRACK_NO,
        //                                       LPI.CURRENCY_CODE
        //                                       FROM LC_PERFOMA_INVOICE LPI,
        //                                       LC_ITEM LI,
        //                                       IP_ITEM_MASTER_SETUP IMS
        //                                WHERE LPI.LC_TRACK_NO = LI.LC_TRACK_NO
        //                                       AND IMS.GROUP_SKU_FLAG = 'I'
        //                                       AND IMS.DELETED_FLAG = 'N'
        //                                       AND LI.ITEM_CODE = IMS.ITEM_CODE
        //                                        AND LI.COMPANY_CODE = IMS.COMPANY_CODE
        //                                       AND LPI.LC_TRACK_NO NOT IN (SELECT LC_TRACK_NO FROM LC_LOC) AND LPI.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";

        //    if (!string.IsNullOrEmpty(reportFilters.FromDate))
        //        sqlquery = sqlquery + " AND LPI.PINVOICE_DATE BETWEEN TO_DATE('" + reportFilters.FromDate + "', 'YYYY-MM-DD') AND TO_DATE('" + reportFilters.ToDate + "', 'YYYY-MM-DD')";

        //    //if (reportFilters.SupplierFilter.Count > 0)
        //    //    sqlquery = sqlquery + " AND LPI.SUPPLIER_CODE IN('" + string.Join("','", reportFilters.SupplierFilter).ToString() + "')";

        //    if (reportFilters.CompanyFilter.Count > 0)
        //        sqlquery = sqlquery + " AND LL.COMPANY_CODE IN('" + string.Join("','", reportFilters.CompanyFilter).ToString() + "')";

        //    if (reportFilters.CurrencyFilter.Count > 0)
        //        sqlquery = sqlquery + " AND  LPI.CURRENCY_CODE IN('" + string.Join("','", reportFilters.CurrencyFilter).ToString() + "')";

        //    //if (reportFilters.BrandFilter.Count > 0)
        //    //    sqlquery = sqlquery + " AND ISS.BRAND_NAME IN('" + string.Join("','", reportFilters.BrandFilter).ToString().ToUpper() + "')";

        //    sqlquery += " GROUP BY LPI.PINVOICE_NO, LPI.PINVOICE_DATE,LPI.LC_TRACK_NO,LPI.CURRENCY_CODE";


        //    var lclist = _dbContext.SqlQuery<PendingLcReportModels>(sqlquery).ToList();
        //    return lclist;
        //}
        //public List<MenuControlEntities> GetAllMenuControl(ReportFiltersModel reportfilters)
        //{
        //    List<MenuControlEntities> menuControlList =new List<MenuControlEntities>();
        //    if (reportfilters.MenuNameFilter.Count == 0 && reportfilters.UserNameFilter.Count == 0)
        //    {
        //        return  menuControlList;
        //    }
        //    else
        //    {
        //        var sqlquery = $@"SELECT A.USER_NO,A.MENU_NO,A.LOGIN_EDESC,A.MENU_EDESC,A.COMPANY_CODE,A.COMPANY_EDESC,A.BRANCH_CODE FROM   (SELECT C.MENU_NO,TO_CHAR(C.USER_NO) AS USER_NO,(SELECT LOGIN_EDESC FROM SC_APPLICATION_USERS WHERE ROWNUM <= 1 AND C.USER_NO = USER_NO) LOGIN_EDESC,(SELECT MENU_EDESC FROM WEB_MENU_MANAGEMENT WHERE ROWNUM <= 1 AND C.MENU_NO = MENU_NO) MENU_EDESC,C.COMPANY_CODE,D.COMPANY_EDESC, C.BRANCH_CODE FROM WEB_MENU_CONTROL C, COMPANY_SETUP D WHERE C.COMPANY_CODE = D.COMPANY_CODE)A WHERE A.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
        //        if (reportfilters.MenuNameFilter.Count > 0)
        //            sqlquery = sqlquery + " AND A.MENU_EDESC IN('" + string.Join("','", reportfilters.MenuNameFilter).ToString() + "')";
        //        //if (reportfilters.ModuleNameFilter.Count > 0)
        //        //    sqlquery = sqlquery + " AND A.SUPPLIER_EDESC  IN('" + string.Join("','", reportfilters.ModuleNameFilter).ToString() + "')";
        //        if (reportfilters.UserNameFilter.Count > 0)
        //            sqlquery = sqlquery + " AND A.LOGIN_EDESC  IN('" + string.Join("','", reportfilters.UserNameFilter).ToString() + "')";

        //        try
        //        {
        //            menuControlList = _dbContext.SqlQuery<MenuControlEntities>(sqlquery).ToList();
        //        }
        //        catch (Exception ex)
        //        {

        //            throw ex;
        //        }
        //        return menuControlList;
        //    }

        //}
        //


        public List<MenuControlEntities> GetAllMenuControl(ReportFiltersModel reportfilters)
        {
            List<MenuControlEntities> menuControlList = new List<MenuControlEntities>();
            if (reportfilters.MenuNameFilter.Count == 0 && reportfilters.UserNameFilter.Count == 0 && reportfilters.ModuleNameFilter.Count == 0)
            {
                return menuControlList;
            }
            else
            {
                var sqlquery = $@"Select DISTINCT WMC.MENU_NO
	                            ,TO_CHAR(WMC.USER_NO) AS USER_NO
	                            ,SAU.LOGIN_EDESC
	                            ,WMM.MENU_EDESC || ' (' || NVL(WMS.MODULE_EDESC, 'Master Setup') || ')' AS MENU_EDESC
	                            ,WMC.COMPANY_CODE
	                            ,CS.COMPANY_EDESC
	                            ,WMC.BRANCH_CODE
	                            ,WMM.MODULE_CODE
	                            ,WMS.MODULE_EDESC
                            FROM SC_APPLICATION_USERS SAU
                            INNER JOIN WEB_MENU_CONTROL WMC ON WMC.USER_NO = SAU.USER_NO
                            INNER JOIN WEB_MENU_MANAGEMENT WMM ON WMM.MENU_NO = WMC.MENU_NO
                            LEFT JOIN WEB_MODULE_SETUP WMS ON WMS.MODULE_CODE = WMM.MODULE_CODE
                            INNER JOIN COMPANY_SETUP CS ON CS.COMPANY_CODE = SAU.COMPANY_CODE WHERE SAU.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
                if (reportfilters.MenuNameFilter.Count > 0)
                    sqlquery = sqlquery + " AND WMM.MENU_EDESC IN('" + string.Join("','", reportfilters.MenuNameFilter).ToString() + "')";
                if (reportfilters.ModuleNameFilter.Count > 0)
                    sqlquery = sqlquery + " AND WMS.MODULE_EDESC  IN('" + string.Join("','", reportfilters.ModuleNameFilter).ToString() + "')";
                if (reportfilters.UserNameFilter.Count > 0)
                    sqlquery = sqlquery + " AND SAU.LOGIN_EDESC  IN('" + string.Join("','", reportfilters.UserNameFilter).ToString() + "')";
                try
                {
                    menuControlList = _dbContext.SqlQuery<MenuControlEntities>(sqlquery).ToList();
                }
                catch (Exception ex)
                {

                    throw ex;
                }
                return menuControlList;
            }

        }

        public List<MenuControlEntities> GetMenus()
        {

            var query = $@"SELECT DISTINCT MENU_NO as id, MENU_EDESC|| ' ('||NVL(WMS.MODULE_EDESC,'Master Setup') ||')' as label 
                            FROM WEB_MENU_MANAGEMENT WMM
                            LEFT JOIN WEB_MODULE_SETUP WMS ON WMM.MODULE_CODE = WMS.MODULE_CODE
                            ORDER BY  WMM.MENU_NO";
            List<MenuControlEntities> menuControlList;
            try
            {
                menuControlList = _dbContext.SqlQuery<MenuControlEntities>(query).ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return menuControlList;
        }


        public List<ModuleModel> GetAllModuleByFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }

            var sqlquery = $@"SELECT DISTINCT MODULE_CODE as module_code,MODULE_EDESC as module_name FROM WEB_MODULE_SETUP  ORDER BY MODULE_CODE ";
            var moduleList = _dbContext.SqlQuery<ModuleModel>(sqlquery).ToList();
            return moduleList;
        }
        public List<UserModel> GetAllUserByFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }

            var sqlquery = $@"SELECT DISTINCT TO_CHAR(USER_NO) AS user_id, LOGIN_EDESC AS user_name FROM SC_APPLICATION_USERS WHERE DELETED_FLAG='N'  and company_code='{_workContext.CurrentUserinformation.company_code}' ORDER BY user_name";
            var userList = _dbContext.SqlQuery<UserModel>(sqlquery).ToList();
            return userList;
        }
        public List<MenuModel> GetAllMenuByFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }

            var sqlquery = $@"SELECT WMM.MENU_NO as menu_id, WMM.MENU_EDESC|| ' ('||NVL(WMS.MODULE_EDESC,'Master Setup') ||')' as menu_name 
                                FROM WEB_MENU_MANAGEMENT WMM
                                LEFT JOIN WEB_MODULE_SETUP WMS ON WMM.MODULE_CODE = WMS.MODULE_CODE
                                 ORDER BY menu_name";
            var menuList = _dbContext.SqlQuery<MenuModel>(sqlquery).ToList();
            return menuList;
        }

        public List<MenuControlEntities> GetUsers()
        {
            var query = $@"SELECT TO_CHAR(USER_NO) as id, LOGIN_EDESC as label FROM SC_APPLICATION_USERS where deleted_flag='N' and company_code='{_workContext.CurrentUserinformation.company_code}'  ";
            List<MenuControlEntities> menuControlList;
            try
            {
                menuControlList = _dbContext.SqlQuery<MenuControlEntities>(query).ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return menuControlList;
        }

        public MenuControlEntities GetUserWiseBulkMenu(string userNo)
        {
            var COMPANY_CODE = this._workContext.CurrentUserinformation.company_code;
            string MenuSql = $@"SELECT to_char(USER_NO), LISTAGG(MENU_NO, ',') WITHIN GROUP (ORDER BY USER_NO) AS MENU_NO FROM WEB_MENU_CONTROL WHERE USER_NO = {userNo} AND COMPANY_CODE='{COMPANY_CODE}' GROUP BY USER_NO";
            MenuSql = MenuSql.Replace("\"", "'");
            MenuControlEntities entities;
            try
            {
                //var a = this._dbContext.ExecuteSqlCommand(MenuSql);
                entities = this._dbContext.SqlQuery<MenuControlEntities>(MenuSql).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return entities;

        }
        public MenuControlEntities GetMenuControls(string menuNo, string userNo)
        {
            var COMPANY_CODE = this._workContext.CurrentUserinformation.company_code;
            int userNoInt = 0;
            int.TryParse(userNo.Trim('"'), out userNoInt);
            string MenuSql = $@"SELECT TO_CHAR(USER_NO)AS USER_NO,MENU_NO,ACCESS_FLAG,COMPANY_CODE,CREATED_BY FROM WEB_MENU_CONTROL WHERE MENU_NO = '{menuNo.Trim()}' AND USER_NO ='{userNoInt}' AND COMPANY_CODE='{COMPANY_CODE}'";
            // MenuSql = MenuSql.Replace("\"", "'");
            MenuControlEntities entities;
            try
            {
                //var a = this._dbContext.ExecuteSqlCommand(MenuSql);
                entities = _objectEntity.SqlQuery<MenuControlEntities>(MenuSql).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return entities;

        }
        //      {
        //        var num = 0;
        //        try
        //        {
        //            string MenuSql = "SELECT * FROM WEB_MENU_CONTROL WHERE MENU_NO = " + menuNo + " AND USER_NO =" + userNo + "";
        //    MenuSql = MenuSql.Replace("\"", "'");
        //            num = this._dbContext.ExecuteSqlCommand(MenuSql);
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //        return num.ToString();
        //    }


        //}
        public string UpdateMenuControl(MenuControlEntities menuControl)
        {


            //string MENU_NO ="";
            //string USER_NO="";

            //if(MENU_NO == menuControl.MENU_NO && USER_NO==menuControl.USER_NO)
            //{

            //}
            var COMPANY_CODE = this._workContext.CurrentUserinformation.company_code;
            string MenucountSql = $@"SELECT COUNT(*) FROM WEB_MENU_CONTROL WHERE MENU_NO = '{menuControl.MENU_NO}'AND USER_NO ='{menuControl.USER_NO}' AND COMPANY_CODE='{COMPANY_CODE}'";
            int count = _objectEntity.SqlQuery<int>(MenucountSql).SingleOrDefault();
            if (count == 1)
            {
                return null;
            }

            var num = 0;
            try
            {
                // string MenuSql = "UPDATE WEB_MENU_CONTROL SET MENU_NO ='" + menuControl.MENU_NO + "', USER_NO =" + menuControl.USER_NO + " WHERE MENU_NO='" + menuControl.MENUNO + "'";
                string MenuSql = $@"UPDATE WEB_MENU_CONTROL SET MENU_NO='{menuControl.MENU_NO}'WHERE USER_NO='{menuControl.USER_NO}'AND MENU_NO = '{menuControl.MENUNO}' AND COMPANY_CODE='{COMPANY_CODE}'";
                //string MenuSql = "UPDATE WEB_MENU_CONTROL SET MENU_NO='" + menuControl.MENU_NO + "'WHERE USER_NO="+menuControl.USER_NO+"";
                num = this._dbContext.ExecuteSqlCommand(MenuSql);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return num.ToString();
        }

        public string AddNewMenuControl(MenuControlEntities menuControl)
        {
            var COMPANY_CODE = this._workContext.CurrentUserinformation.company_code;
            var num = 0;
            var menuNo = menuControl.MENU_NO.Split(',');
            foreach (var MENU_NO in menuNo)
            {
                int checkValue = 0;
                string checkSql = $@"SELECT count(*)COUNT FROM WEB_MENU_CONTROL WHERE COMPANY_CODE='{COMPANY_CODE}' AND  MENU_NO = '{MENU_NO.Trim()}' AND USER_NO='{menuControl.USER_NO}'";
                checkValue = _dbContext.SqlQuery<int>(checkSql).First();
                if (checkValue > 0)
                {
                    continue;
                }
                if (string.IsNullOrEmpty(menuControl.ACCESS_FLAG))
                    menuControl.ACCESS_FLAG = "Y";
                string MenuSql = $@"INSERT INTO WEB_MENU_CONTROL (USER_NO, MENU_NO, COMPANY_CODE, ACCESS_FLAG, CREATED_BY, CREATED_DATE)
                                VALUES('{menuControl.USER_NO}','{MENU_NO.Trim()}','{COMPANY_CODE}','{menuControl.ACCESS_FLAG}','{MENU_NO.Trim()}',SYSDATE)";

                try
                {
                    num = this._dbContext.ExecuteSqlCommand(MenuSql);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return num.ToString();
        }

        public string AddBulkMenuUser(MenuControlEntities menuControl)
        {
            var COMPANY_CODE = this._workContext.CurrentUserinformation.company_code;
            var num = 0;

            string deleteSql = $@"DELETE FROM WEB_MENU_CONTROL WHERE USER_NO IN ( {menuControl.USER_NO}) AND MENU_NO IN ({menuControl.MENU_NO}) --AND COMPANY_CODE='{COMPANY_CODE}'";
            num = this._dbContext.ExecuteSqlCommand(deleteSql);

            if (menuControl.MENU_NO == null || menuControl.MENU_NO == "")
            {
                return num.ToString();
            }
            var menuNo = menuControl.MENU_NO.Split(',');
            var userNo = menuControl.USER_NO.Split(',');
            if (string.IsNullOrEmpty(menuControl.ACCESS_FLAG))
                menuControl.ACCESS_FLAG = "Y";
            foreach (var USER_NO in userNo)
            {
                foreach (var MENU_NO in menuNo)
                {
                    string MenuSql = $@"INSERT INTO WEB_MENU_CONTROL (MENU_NO,USER_NO, COMPANY_CODE, ACCESS_FLAG, CREATED_BY, CREATED_DATE)
                                        VALUES('{MENU_NO.Trim()}','{USER_NO.Trim()}','{COMPANY_CODE}','{menuControl.ACCESS_FLAG}','{USER_NO.Trim()}',SYSDATE)";

                    try
                    {
                        num = this._dbContext.ExecuteSqlCommand(MenuSql);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

                //var menuNo = menuControl.MENU_NO.Split(',');
                //foreach (var MENU_NO in menuNo)
                //{
                //    string Menu = "INSERT INTO WEB_MENU_CONTROL (MENU_NO,USER_NO, COMPANY_CODE, ACCESS_FLAG, CREATED_BY, CREATED_DATE)" +
                //                    " VALUES('" + menuControl.USER_NO + "','" + MENU_NO.Trim() + "','01','" + menuControl.ACCESS_FLAG + "','" + MENU_NO.Trim() + "',SYSDATE)";
                //    try
                //    {
                //        num = this._dbContext.ExecuteSqlCommand(Menu);
                //    }
                //    catch (Exception ex)
                //    {
                //        throw ex;
                //    }
                //}

            }




            return num.ToString();
        }
        public string DeleteMenuControl(string menuNo, string userNo)
        {
            var COMPANY_CODE = this._workContext.CurrentUserinformation.company_code;
            var num = 0;
            try
            {
                string MenuSql = $@"DELETE FROM WEB_MENU_CONTROL WHERE MENU_NO= {menuNo} AND USER_NO ={userNo} AND COMPANY_CODE='{COMPANY_CODE}'";
                MenuSql = MenuSql.Replace("\"", "'");
                num = this._dbContext.ExecuteSqlCommand(MenuSql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return num.ToString();
        }


    }
}