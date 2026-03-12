using ExcelDataReader;
using NeoErp.Core;
using NeoErp.Core.Helpers;
using NeoErp.Data;
using NeoErp.Planning.Service.Interface;
using NeoErp.Planning.Service.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.OracleClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Web;

namespace NeoErp.Planning.Service.Repository
{
    public class DistributionPlaning : IDistributionPlaning
    {
        private IDbContext _dbContext;
        private IWorkContext _workcontext;
        public DistributionPlaning(IDbContext dbContext, IWorkContext _iWorkContext)
        {
            this._workcontext = _iWorkContext;
            this._dbContext = dbContext;
        }

        public string createPlanWiseRoute(RoutePlanModels model)
        {
            try
            {
                string message = "";
                var currentdate = DateTime.Now.ToString("MM/dd/yyyy");
                var currentUserId = _workcontext.CurrentUserinformation.User_id;
                var company_code = _workcontext.CurrentUserinformation.company_code;
                if (model.ROUTE_CODE != "")
                {
                    //var nextValQuery = $@"SELECT PL_PLAN_ROUTES_SEQ.nextval as PL_PLAN_NEXT_CODE FROM DUAL";
                    //var id = _dbContext.SqlQuery<planDetailModel>(nextValQuery).ToList().FirstOrDefault();

                    var insertQuery = $@"INSERT INTO DIST_ROUTE_PLAN(PLAN_EDESC ,START_DATE, END_DATE, TIME_FRAME_CODE , REMARKS ,COMPANY_CODE ,CREATED_BY ,CREATED_DATE ,DELETED_FLAG)
                    VALUES('{model.ROUTE_PLAN_NAME}',TO_DATE('{model.START_DATE}','yyyy/mm/dd'),TO_DATE('{model.END_DATE}','yyyy/mm/dd'),'{model.FREQUENCY_CODE}','{model.REMARKS}','{company_code}','{currentUserId}',TO_DATE('{currentdate}','mm/dd/yyyy'),'N')";
                    var rowCount = _dbContext.ExecuteSqlCommand(insertQuery);
                    _dbContext.SaveChanges();
                    string query_plancode = $@"SELECT PLAN_CODE FROM DIST_ROUTE_PLAN WHERE PLAN_EDESC='{model.ROUTE_PLAN_NAME}' AND START_DATE=TO_DATE('{model.START_DATE}','yyyy/mm/dd') AND END_DATE=TO_DATE('{model.END_DATE}','yyyy/mm/dd') AND TIME_FRAME_CODE='{model.FREQUENCY_CODE}'";
                    string insertedPlancode = this._dbContext.SqlQuery<Int64>(query_plancode).FirstOrDefault().ToString();

                    if (rowCount > 0 && !string.IsNullOrEmpty(insertedPlancode))
                    {
                        var routeCode = model.ROUTE_CODE.Split(',');
                        for (int i = 0; i < routeCode.Length; i++)
                        {
                            var Query = $@"INSERT INTO DIST_ROUTE_PLAN_DETAIL(PLAN_CODE , ROUTE_CODE , COMPANY_CODE ,CREATED_BY ,CREATED_DATE ,DELETED_FLAG)
                            VALUES('{insertedPlancode}','{routeCode[i]}','{company_code}','{currentUserId}',TO_DATE('{currentdate}','MM/DD/YYYY'),'N')";
                            var rowNum = _dbContext.ExecuteSqlCommand(Query);
                            _dbContext.SaveChanges();
                            message = insertedPlancode;
                        }
                    }
                }
                return message;
            }
            catch (Exception)
            {
                return "failed";
            }

        }
        public string AddUpdateRoutes(RouteModels model)
        {
            string message = "";
            if (string.Equals(model.ROUTE_CODE, "0") || string.IsNullOrEmpty(model.ROUTE_CODE))
            {
                var currentdate = DateTime.Now.ToString("MM/dd/yyyy");
                var currentUserId = _workcontext.CurrentUserinformation.User_id;
                var company_code = _workcontext.CurrentUserinformation.company_code;

                //var checkifexists = string.Format(@"SELECT count(*)COUNT from PL_ROUTES where ROUTE_EDESC= '{0}'", model.ROUTE_EDESC);
                var checkifexists = string.Format(@"SELECT count(*)COUNT from DIST_AREA_MASTER where AREA_CODE= '{0}'", model.AREA_CODE);
                var CountRow = _dbContext.SqlQuery<int>(checkifexists).First();
                if (CountRow > 0)
                {
                    //string query = $@"UPDATE PL_ROUTES SET ROUTE_EDESC  = '{model.ROUTE_EDESC}',ROUTE_NDESC= '{model.ROUTE_EDESC}',LAST_MODIFIED_BY = '{ _workcontext.CurrentUserinformation.User_id}',LAST_MODIFIED_DATE = TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}', 'mm/dd/yyyy'),deleted_flag='N' WHERE ROUTE_EDESC='{model.ROUTE_EDESC}'";
                    string query = $@"UPDATE DIST_AREA_MASTER SET AREA_CODE='{model.AREA_CODE}' , AREA_NAME='{model.AREA_NAME}' WHERE AREA_CODE='{model.AREA_CODE}'";
                    var rowCount = _dbContext.ExecuteSqlCommand(query);
                    message = "Success";
                }
                else
                {
                    //var insertQuery = string.Format(@"INSERT INTO PL_ROUTES(ROUTE_CODE ,ROUTE_EDESC ,COMPANY_CODE ,CREATED_BY ,CREATED_DATE ,LAST_MODIFIED_BY ,LAST_MODIFIED_DATE ,APPROVED_BY ,APPROVED_DATE ,DELETED_FLAG)VALUES({0},'{1}','{2}','{3}',TO_DATE('{4}','mm/dd/yyyy'),'{5}',TO_DATE('{6}','mm/dd/yyyy'),'{7}',TO_DATE('{8}','mm/dd/yyyy'),'{9}')", "PL_ROUTE_SEQ.nextval", model.ROUTE_EDESC, company_code, currentUserId, currentdate, currentUserId, currentdate, currentUserId, currentdate, notdeleted);
                    var insertQuery = $@"INSERT INTO DIST_AREA_MASTER(AREA_CODE,AREA_NAME,COMPANY_CODE) VALUES ('{model.AREA_CODE}','{model.AREA_NAME}','{_workcontext.CurrentUserinformation.company_code}')";
                    var rowCount = _dbContext.ExecuteSqlCommand(insertQuery);
                    _dbContext.SaveChanges();
                    message = "Success";
                }

            }
            else
            {
                var checkifexists = string.Format(@"SELECT count(*)COUNT from DIST_AREA_MASTER where AREA_CODE= '{0}'", model.AREA_CODE);
                var CountRow = _dbContext.SqlQuery<int>(checkifexists).First();
                if (CountRow > 0)
                {
                    message = "ExistsButDeleted";

                }
                else
                {
                    //   string query = string.Format(@"UPDATE PL_ROUTES SET ROUTE_EDESC  = '{0}',ROUTE_NDESC= '{1}',LAST_MODIFIED_BY = '{2}',LAST_MODIFIED_DATE = TO_DATE('{3}', 'mm/dd/yyyy'),deleted_flag='N' WHERE ROUTE_CODE IN ({4})",
                    //model.ROUTE_EDESC, model.ROUTE_EDESC, _workcontext.CurrentUserinformation.User_id, DateTime.Now.ToString("MM/dd/yyyy"), model.ROUTE_CODE);
                    string query = $@"UPDATE DIST_AREA_MASTER SET AREA_CODE='{model.AREA_CODE}', AREA_NAME='{model.AREA_NAME}' WHERE AREA_CODE='{model.AREA_CODE}'";
                    var rowCount = _dbContext.ExecuteSqlCommand(query);
                    message = "Success";
                }
            }

            return message;
        }

        public bool checkifexists(RouteModels model)
        {
            var checkifexists = string.Format(@"SELECT count(*)COUNT from DIST_AREA_MASTER where AREA_CODE= '{0}' AND AREA_NAME='{0}' ", model.AREA_CODE, model.AREA_NAME);
            var CountRow = _dbContext.SqlQuery<int>(checkifexists).First();
            if (CountRow > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void deleteRoute(int code)
        {
            string query = string.Format(@"UPDATE PL_ROUTES SET DELETED_FLAG  = '{0}' WHERE ROUTE_CODE IN ({1})",
            'Y', code);
            var rowCount = _dbContext.ExecuteSqlCommand(query);
        }

        public List<Area_Master> getAllRoutes()
        {
            // hepl database
            //var sqlquery = $@"select DISTINCT ROUTE_CODE, ROUTE_EDESC FROM PL_ROUTES
            //                WHERE deleted_flag ='N'";

            // global_7374
            var sqlquery = $@"SELECT AREA_CODE,AREA_NAME,ZONE_CODE,VDC_CODE,GEO_CODE,REG_CODE  FROM DIST_AREA_MASTER
                            --WHERE deleted_flag ='N'";
            var route = _dbContext.SqlQuery<Area_Master>(sqlquery).ToList();
            return route;
        }

        public List<EMP_GROUP> getAllEmpGroups()
        {
            var company_code = this._workcontext.CurrentUserinformation.company_code;
            var Query = $@"SELECT GROUPID, GROUP_CODE, GROUP_EDESC FROM DIST_GROUP_MASTER WHERE COMPANY_CODE='{company_code}' AND DELETED_FLAG='N'  ORDER BY TO_NUMBER(GROUPID) ASC";
            var result = _dbContext.SqlQuery<EMP_GROUP>(Query).ToList();
            return result;
        }

        public List<DIST_ROUTE_PLAN> getAllPlanRoutes(string plancode)
        {
            var company_code = this._workcontext.CurrentUserinformation.company_code;
            //var sqlquery = $@"select distinct PLAN_CODE ,PLAN_EDESC,PLAN_NDESC,START_DATE,END_DATE,TARGET_NAME ,TARGET_VALUE,TIME_FRAME_CODE,
            //    REMARKS ,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,APPROVED_BY,APPROVED_DATE,DELETED_FLAG,
            //    (SELECT  TIME_FRAME_EDESC FROM PL_TIME_FRAME WHERE TIME_FRAME_CODE = DIST_ROUTE_PLAN.TIME_FRAME_CODE) AS TIME_FRAME_EDESC 
            //    from DIST_ROUTE_PLAN WHERE DELETED_FLAG='N' ORDER BY PLAN_CODE DESC";
            var sqlquery = $@"SELECT PLAN_CODE ,PLAN_EDESC,PLAN_NDESC,START_DATE,END_DATE,TARGET_NAME ,TARGET_VALUE,TIME_FRAME_CODE,
REMARKS ,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,APPROVED_BY,APPROVED_DATE,DELETED_FLAG,
(SELECT  TIME_FRAME_EDESC FROM PL_TIME_FRAME WHERE TIME_FRAME_CODE = DIST_ROUTE_PLAN.TIME_FRAME_CODE) AS TIME_FRAME_EDESC 
FROM DIST_ROUTE_PLAN WHERE DELETED_FLAG='N' AND COMPANY_CODE = '{company_code}' AND ROUTE_TYPE='D' ORDER BY START_DATE DESC, UPPER(TRIM(PLAN_EDESC)) ASC";
            if (!string.IsNullOrEmpty(plancode))
            {
                //sqlquery = $@"select distinct PLAN_CODE ,PLAN_EDESC,PLAN_NDESC,START_DATE,END_DATE,TARGET_NAME ,TARGET_VALUE,TIME_FRAME_CODE,
                //REMARKS ,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,APPROVED_BY,APPROVED_DATE,DELETED_FLAG,
                //(SELECT  TIME_FRAME_EDESC FROM PL_TIME_FRAME WHERE TIME_FRAME_CODE = DIST_ROUTE_PLAN.TIME_FRAME_CODE) AS TIME_FRAME_EDESC 
                //from DIST_ROUTE_PLAN WHERE DELETED_FLAG='N' AND PLAN_CODE='{plancode}'";
                sqlquery = $@"SELECT PLAN_CODE ,PLAN_EDESC,PLAN_NDESC,START_DATE,END_DATE,TARGET_NAME ,TARGET_VALUE,TIME_FRAME_CODE,
REMARKS ,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,APPROVED_BY,APPROVED_DATE,DELETED_FLAG,
(SELECT  TIME_FRAME_EDESC FROM PL_TIME_FRAME WHERE TIME_FRAME_CODE = DIST_ROUTE_PLAN.TIME_FRAME_CODE) AS TIME_FRAME_EDESC 
FROM DIST_ROUTE_PLAN WHERE  DELETED_FLAG='N' AND COMPANY_CODE = '{company_code}' AND ROUTE_TYPE='D' AND PLAN_CODE='{plancode}' ORDER BY START_DATE DESC, UPPER(TRIM(PLAN_EDESC)) ASC";
            }
            var route = _dbContext.SqlQuery<DIST_ROUTE_PLAN>(sqlquery).ToList();
            return route;
        }

        public List<DIST_ROUTE_PLAN> getAllBrandingPlanRoutes(string plancode)
        {
            var company_code = this._workcontext.CurrentUserinformation.company_code;
            var sqlquery = $@"SELECT PLAN_CODE ,PLAN_EDESC,PLAN_NDESC,START_DATE,END_DATE,TARGET_NAME ,TARGET_VALUE,TIME_FRAME_CODE,
                            REMARKS ,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,APPROVED_BY,APPROVED_DATE,DELETED_FLAG,
                            (SELECT  TIME_FRAME_EDESC FROM PL_TIME_FRAME WHERE TIME_FRAME_CODE = DIST_ROUTE_PLAN.TIME_FRAME_CODE) AS TIME_FRAME_EDESC 
                            FROM DIST_ROUTE_PLAN WHERE DELETED_FLAG='N' AND COMPANY_CODE = '{company_code}' AND  ROUTE_TYPE='B'  ORDER BY START_DATE DESC, UPPER(TRIM(PLAN_EDESC)) ASC";
            if (!string.IsNullOrEmpty(plancode))
            {
                sqlquery = $@"SELECT PLAN_CODE ,PLAN_EDESC,PLAN_NDESC,START_DATE,END_DATE,TARGET_NAME ,TARGET_VALUE,TIME_FRAME_CODE,
                            REMARKS ,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,APPROVED_BY,APPROVED_DATE,DELETED_FLAG,
                            (SELECT  TIME_FRAME_EDESC FROM PL_TIME_FRAME WHERE TIME_FRAME_CODE = DIST_ROUTE_PLAN.TIME_FRAME_CODE) AS TIME_FRAME_EDESC 
                            FROM DIST_ROUTE_PLAN WHERE  DELETED_FLAG='N' AND COMPANY_CODE = '{company_code}' AND PLAN_CODE='{plancode}' AND  ROUTE_TYPE='B' ORDER BY START_DATE DESC, UPPER(TRIM(PLAN_EDESC)) ASC";
            }
            var route = _dbContext.SqlQuery<DIST_ROUTE_PLAN>(sqlquery).ToList();
            return route;
        }


        public List<RouteModels> getAllRoutesByFilter(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
                var sqlquery = $@"select ROUTE_CODE, ROUTE_NAME from DIST_ROUTE_MASTER where DELETED_FLAG='N' and COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}' AND (ROUTE_NAME like '%{filter.ToLowerInvariant()}%' OR ROUTE_CODE like '%{filter.ToString().ToLowerInvariant()}%') ORDER BY ROUTE_NAME";
                var route = _dbContext.SqlQuery<RouteModels>(sqlquery).ToList();
                return route;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<RouteModels> getAllRoutesByFilter(string filter, string empCode)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
                var sqlquery = $@" SELECT RM.ROUTE_CODE,RM.ROUTE_NAME
                                          --AR.AREA_CODE 
                                    FROM DIST_ROUTE_MASTER RM, DIST_ROUTE_AREA AR,DIST_USER_AREAS UA
                                    WHERE AR.COMPANY_CODE=RM.COMPANY_CODE
                                     AND RM.ROUTE_CODE=AR.ROUTE_CODE AND AR.AREA_CODE=UA.AREA_CODE
                                     AND RM.DELETED_FLAG='N'
                                     AND UA.AREA_CODE=AR.AREA_CODE AND RM.COMPANY_CODE=UA.COMPANY_CODE AND UA.SP_CODE='{empCode}'
                                     ORDER BY RM.ROUTE_NAME";
                var route = _dbContext.SqlQuery<RouteModels>(sqlquery).ToList();
                return route;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<RouteModels> getAllRoutesByFilterRouteType(string filter, string empCode, string RouteType = "D")
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
                var sqlquery = $@" SELECT DISTINCT RM.ROUTE_CODE,RM.ROUTE_NAME,AR.AREA_CODE FROM DIST_ROUTE_MASTER RM, DIST_ROUTE_AREA AR,DIST_USER_AREAS UA
                                    WHERE AR.COMPANY_CODE=RM.COMPANY_CODE
                                     AND RM.ROUTE_CODE=AR.ROUTE_CODE AND AR.AREA_CODE=UA.AREA_CODE
                                     AND UA.AREA_CODE=AR.AREA_CODE AND RM.COMPANY_CODE=UA.COMPANY_CODE AND UA.SP_CODE='{empCode}' 
                                     and RM.DELETED_FLAG='N' AND RM.ROUTE_TYPE='{RouteType}'
                                     ORDER BY RM.ROUTE_NAME";
                var route = _dbContext.SqlQuery<RouteModels>(sqlquery).ToList();
                return route;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public List<RouteModels> getAllBrandingRoutesByFilter(string filter, string empCode)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
                var sqlquery = $@" SELECT RM.ROUTE_CODE,RM.ROUTE_NAME,AR.AREA_CODE FROM DIST_ROUTE_MASTER RM, DIST_ROUTE_AREA AR,DIST_USER_AREAS UA
                                    WHERE AR.COMPANY_CODE=RM.COMPANY_CODE
                                     AND RM.ROUTE_CODE=AR.ROUTE_CODE AND AR.AREA_CODE=UA.AREA_CODE
                                     AND RM.DELETED_FLAG='N'
                                     AND UA.AREA_CODE=AR.AREA_CODE AND RM.COMPANY_CODE=UA.COMPANY_CODE and RM.ROUTE_TYPE='B' AND UA.SP_CODE='{empCode}'
                                     ORDER BY RM.ROUTE_NAME";
                var route = _dbContext.SqlQuery<RouteModels>(sqlquery).ToList();
                return route;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<RouteModels> GetRouteByRouteCode(string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code)) { code = string.Empty; }
                var sqlquery = $@"SELECT RPD.PLAN_CODE,RPD.ROUTE_CODE,RM.ROUTE_NAME FROM DIST_ROUTE_PLAN_DETAIL RPD, DIST_ROUTE_MASTER RM
                                WHERE RPD.ROUTE_CODE=RM.ROUTE_CODE
                                AND RPD.DELETED_FLAG='N'
                                AND RPD.PLAN_CODE='{code}' ORDER BY RM.ROUTE_NAME";
                var route = _dbContext.SqlQuery<RouteModels>(sqlquery).ToList();
                return route;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<RoutePlanModel> GetRouteByPlanCode(string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code)) { code = string.Empty; }
                //var sqlquery = $@"select pr.ROUTE_EDESC, pr.ROUTE_CODE from  pl_plan_routes_mapping pprm, PL_ROUTES pr 
                //            WHERE pr.ROUTE_CODE = pprm.ROUTE_CODE AND pprm.PLAN_ROUTES_CODE = {code}";
                var sqlquery = $@"SELECT DISTINCT RPD.ROUTE_CODE,RP.PLAN_CODE, RM.ROUTE_NAME, TO_CHAR(RPD.ASSIGN_DATE,'DD-MM-YYYY')ASSIGN_DATE, RPD.EMP_CODE, RP.START_DATE, RP.END_DATE 
                                           FROM dist_route_detail RPD, DIST_ROUTE_MASTER RM, DIST_ROUTE_PLAN RP
                                WHERE RPD.ROUTE_CODE= RM.ROUTE_CODE
                                AND RPD.PLAN_CODE = RP.PLAN_CODE
                                AND RPD.DELETED_FLAG=RM.DELETED_FLAG
                                AND RPD.DELETED_FLAG=RP.DELETED_FLAG
                                AND RPD.DELETED_FLAG= 'N'
                                AND RPD.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'
                                AND RPD.PLAN_CODE= '{code}' ORDER BY RM.ROUTE_NAME";
                var route = _dbContext.SqlQuery<RoutePlanModel>(sqlquery).ToList();
                return route;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string UpdateRouteExpireEndDate(UpdateExpEndDateModal modal)
        {
            try
            {
                var updateDRP = $@" UPDATE DIST_ROUTE_PLAN RP SET RP.END_DATE =TO_DATE('{modal.EDITED_END_DATE.ToShortDateString()}','mm/dd/yyyy') WHERE RP.PLAN_CODE='{modal.PLAN_CODE}'";
                var rowUpdated = _dbContext.ExecuteSqlCommand(updateDRP);
                if (rowUpdated > 0)
                {
                    return "Updated";
                }
                else
                {
                    return "Error";
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<RoutePlanModel> GetBrandingRouteByPlanCode(string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code)) { code = string.Empty; }
                //var sqlquery = $@"select pr.ROUTE_EDESC, pr.ROUTE_CODE from  pl_plan_routes_mapping pprm, PL_ROUTES pr 
                //            WHERE pr.ROUTE_CODE = pprm.ROUTE_CODE AND pprm.PLAN_ROUTES_CODE = {code}";
                var sqlquery = $@"SELECT RPD.ROUTE_CODE, RM.ROUTE_NAME, TO_CHAR(RPD.ASSIGN_DATE,'DD-MM-YYYY')ASSIGN_DATE, RPD.EMP_CODE, RP.START_DATE, RP.END_DATE FROM DIST_BRANDING_ROUTE_DETAIL RPD, DIST_ROUTE_MASTER RM, DIST_ROUTE_PLAN RP
                                WHERE RPD.ROUTE_CODE= RM.ROUTE_CODE
                                AND RPD.PLAN_CODE = RP.PLAN_CODE
                                AND RPD.DELETED_FLAG= 'N'
                                AND RPD.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'
                                AND RPD.PLAN_CODE= '{code}' and RM.ROUTE_TYPE='B'
                                AND RP.ROUTE_TYPE='B' ORDER BY RM.ROUTE_NAME";
                var route = _dbContext.SqlQuery<RoutePlanModel>(sqlquery).ToList();
                return route;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<EmployeeModels> getEmployees(string filter, string empGroup)
        {
            try
            {
                var condition = string.Empty;
                if (empGroup != "" && empGroup != null)
                    condition = $@" AND LU.GROUPID in ({empGroup})";

                if (!string.IsNullOrWhiteSpace(filter))
                    filter = $@" AND LOWER(TRIM(ES.EMPLOYEE_EDESC)) LIKE '%{filter.ToLower()}%'";

                var spFilter = string.Empty;
                if (!string.IsNullOrWhiteSpace(_workcontext.CurrentUserinformation.sp_codes))
                    spFilter = $@" AND LU.SP_CODE IN ({_workcontext.CurrentUserinformation.sp_codes})";

                string query = $@"SELECT DISTINCT ES.EMPLOYEE_CODE EMPLOYEE_CODE,ES.EMPLOYEE_EDESC || ' ('||ES.EMPLOYEE_CODE||')' EMPLOYEE_EDESC,
                    ES.EMPLOYEE_NDESC EMPLOYEE_NDESC,ES.GROUP_SKU_FLAG GROUP_SKU_FLAG,ES.MASTER_EMPLOYEE_CODE MASTER_EMPLOYEE_CODE,
                    ES.PRE_EMPLOYEE_CODE PRE_EMPLOYEE_CODE  FROM HR_EMPLOYEE_SETUP ES, DIST_LOGIN_USER LU
                    WHERE   ES.DELETED_FLAG='N'   AND LU.SP_CODE=ES.EMPLOYEE_CODE   AND ES.COMPANY_CODE = LU.COMPANY_CODE
                    AND LU.BRANDING='N' AND ES.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}' {condition} {filter} {spFilter}
                    ORDER BY LOWER(TRIM(ES.EMPLOYEE_EDESC || ' ('||ES.EMPLOYEE_CODE||')'))";

                var result = this._dbContext.SqlQuery<EmployeeModels>(query).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<EmployeeModels> getSNGEmployees(string filter, string empGroup)
        {
            try
            {
                var condition = string.Empty;
                if (empGroup != "" && empGroup != null)
                    condition = $@" AND PRE_EMPLOYEE_CODE in ({empGroup})";
                if (!string.IsNullOrWhiteSpace(filter))
                    filter = $@" AND LOWER(TRIM(EMPLOYEE_EDESC)) LIKE '%{filter.ToLower()}%'";

                string query = $@"select * from HR_EMPLOYEE_SETUP where group_sku_flag='I' and deleted_flag='N' and COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}' {filter} {condition}";
                var result = this._dbContext.SqlQuery<EmployeeModels>(query).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<EmployeeModels> getBrandingEmployees(string filter, string empGroup)
        {
            try
            {
                //string query = $@"SELECT EMPLOYEE_CODE,EMPLOYEE_EDESC,EMPLOYEE_NDESC,GROUP_SKU_FLAG,MASTER_EMPLOYEE_CODE,PRE_EMPLOYEE_CODE FROM HR_EMPLOYEE_SETUP WHERE DELETED_FLAG='N'";
                //string query = $@"SELECT DISTINCT SPM.SP_CODE SP_CODE,ES.EMPLOYEE_CODE EMPLOYEE_CODE,ES.EMPLOYEE_EDESC EMPLOYEE_EDESC,
                //                ES.EMPLOYEE_NDESC EMPLOYEE_NDESC,ES.GROUP_SKU_FLAG GROUP_SKU_FLAG,ES.MASTER_EMPLOYEE_CODE MASTER_EMPLOYEE_CODE,
                //                ES.PRE_EMPLOYEE_CODE PRE_EMPLOYEE_CODE
                //                FROM HR_EMPLOYEE_SETUP ES,DIST_SALESPERSON_MASTER SPM
                //                WHERE ES.DELETED_FLAG='N' AND SPM.ACTIVE='Y'
                //                AND SPM.SP_CODE=ES.EMPLOYEE_CODE ORDER BY LOWER(TRIM(ES.EMPLOYEE_EDESC))";
                //string query = $@"SELECT DISTINCT SPM.SP_CODE SP_CODE,ES.EMPLOYEE_CODE EMPLOYEE_CODE,ES.EMPLOYEE_EDESC || ' ('||ES.EMPLOYEE_CODE||')' EMPLOYEE_EDESC,
                //    ES.EMPLOYEE_NDESC EMPLOYEE_NDESC,ES.GROUP_SKU_FLAG GROUP_SKU_FLAG,ES.MASTER_EMPLOYEE_CODE MASTER_EMPLOYEE_CODE,
                //    ES.PRE_EMPLOYEE_CODE PRE_EMPLOYEE_CODE
                //    FROM HR_EMPLOYEE_SETUP ES,DIST_SALESPERSON_MASTER SPM
                //    WHERE ES.DELETED_FLAG='N' AND SPM.ACTIVE='Y'
                //    AND SPM.SP_CODE=ES.EMPLOYEE_CODE 
                //    AND ES.COMPANY_CODE = SPM.COMPANY_CODE
                //    AND SPM.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'
                //    ORDER BY LOWER(TRIM(ES.EMPLOYEE_EDESC || ' ('||ES.EMPLOYEE_CODE||')'))";
                var condition = string.Empty;
                if (empGroup != "" && empGroup != null)
                    condition = $@" AND LU.GROUPID ='{empGroup}'";

                string query = $@"SELECT DISTINCT SPM.SP_CODE SP_CODE,ES.EMPLOYEE_CODE EMPLOYEE_CODE,ES.EMPLOYEE_EDESC || ' ('||ES.EMPLOYEE_CODE||')' EMPLOYEE_EDESC,
                    ES.EMPLOYEE_NDESC EMPLOYEE_NDESC,ES.GROUP_SKU_FLAG GROUP_SKU_FLAG,ES.MASTER_EMPLOYEE_CODE MASTER_EMPLOYEE_CODE,
                    ES.PRE_EMPLOYEE_CODE PRE_EMPLOYEE_CODE
                    FROM HR_EMPLOYEE_SETUP ES,DIST_SALESPERSON_MASTER SPM, DIST_LOGIN_USER LU
                    WHERE SPM.SP_CODE = LU.SP_CODE
                    AND SPM.COMPANY_CODE = LU.COMPANY_CODE
                    AND  ES.DELETED_FLAG='N' AND SPM.ACTIVE='Y'
                    AND SPM.SP_CODE=ES.EMPLOYEE_CODE 
                    AND ES.COMPANY_CODE = SPM.COMPANY_CODE
                    AND LU.BRANDING='Y'
                    AND SPM.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}' {condition}
                    ORDER BY LOWER(TRIM(ES.EMPLOYEE_EDESC || ' ('||ES.EMPLOYEE_CODE||')'))";
                if (!string.IsNullOrEmpty(filter))
                {
                    //query = $@"SELECT EMPLOYEE_CODE,EMPLOYEE_EDESC,EMPLOYEE_NDESC,GROUP_SKU_FLAG,MASTER_EMPLOYEE_CODE,PRE_EMPLOYEE_CODE FROM HR_EMPLOYEE_SETUP WHERE DELETED_FLAG='N' AND LOWER(EMPLOYEE_EDESC) LIKE '%" + filter.ToLower() + "%'";
                    //query = $@"SELECT DISTINCT SPM.SP_CODE SP_CODE,ES.EMPLOYEE_CODE EMPLOYEE_CODE,ES.EMPLOYEE_EDESC EMPLOYEE_EDESC,
                    //            ES.EMPLOYEE_NDESC EMPLOYEE_NDESC,ES.GROUP_SKU_FLAG GROUP_SKU_FLAG,ES.MASTER_EMPLOYEE_CODE MASTER_EMPLOYEE_CODE,
                    //            ES.PRE_EMPLOYEE_CODE PRE_EMPLOYEE_CODE
                    //        FROM HR_EMPLOYEE_SETUP ES,DIST_SALESPERSON_MASTER SPM
                    //         WHERE  ES.DELETED_FLAG='N' AND SPM.ACTIVE='Y'
                    //         AND SPM.SP_CODE=ES.EMPLOYEE_CODE
                    //         AND LOWER (TRIM(ES.EMPLOYEE_EDESC)) LIKE '%" + filter.ToLower() + "%'  ORDER BY LOWER(TRIM(ES.EMPLOYEE_EDESC))";
                    query = $@"SELECT DISTINCT SPM.SP_CODE SP_CODE,ES.EMPLOYEE_CODE EMPLOYEE_CODE,ES.EMPLOYEE_EDESC || ' ('||ES.EMPLOYEE_CODE||')' EMPLOYEE_EDESC,
                        ES.EMPLOYEE_NDESC EMPLOYEE_NDESC,ES.GROUP_SKU_FLAG GROUP_SKU_FLAG,ES.MASTER_EMPLOYEE_CODE MASTER_EMPLOYEE_CODE,
                        ES.PRE_EMPLOYEE_CODE PRE_EMPLOYEE_CODE
                        FROM HR_EMPLOYEE_SETUP ES,DIST_SALESPERSON_MASTER SPM ,  DIST_LOGIN_USER LU
                        WHERE ES.DELETED_FLAG='N' AND SPM.ACTIVE='Y'
                        AND SPM.SP_CODE = LU.SP_CODE
                        AND SPM.COMPANY_CODE = LU.COMPANY_CODE
                        AND SPM.SP_CODE=ES.EMPLOYEE_CODE 
                        AND ES.COMPANY_CODE = SPM.COMPANY_CODE
                         AND LU.BRANDING='Y'
                        AND SPM.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}' {condition}
                        AND LOWER (TRIM(ES.EMPLOYEE_EDESC)) LIKE '%" + filter.ToLower() + "%' ORDER BY LOWER(TRIM(ES.EMPLOYEE_EDESC || ' ('||ES.EMPLOYEE_CODE||')'))";
                }
                var result = this._dbContext.SqlQuery<EmployeeModels>(query).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<RoutePlanDateSeries> getDateSeries(string plancode)
        {
            try
            {
                //string query = $@"SELECT
                //    TO_CHAR((RD.START_DATE) - 1 + ROWNUM) AS DATES,
                //    TO_CHAR(EXTRACT(YEAR FROM ((RD.START_DATE) - 1 + ROWNUM))) AS YEAR, 
                //    TO_CHAR(EXTRACT(MONTH FROM ((RD.START_DATE) - 1 + ROWNUM))) AS MONTH,
                //    TO_CHAR(((RD.START_DATE) - 1 + ROWNUM),'MONTH') AS MONTH_NAME,
                //    TO_CHAR(EXTRACT(DAY FROM ((RD.START_DATE) - 1 + ROWNUM))) AS DAY
                //    FROM ALL_OBJECTS O, DIST_ROUTE_PLAN RD
                //    WHERE TO_DATE(RD.START_DATE) - 1 + ROWNUM <= TO_DATE(RD.END_DATE)
                //    AND RD.PLAN_CODE='{plancode}'
                //    ORDER BY ((RD.START_DATE) - 1 + ROWNUM)";
                string query = $@"SELECT
                    TO_CHAR( BS_DATE((RD.START_DATE) - 1 + ROWNUM)) AS DATES,
                    TO_CHAR(SUBSTR( BS_DATE((RD.START_DATE) - 1 + ROWNUM),0,4)) AS YEAR, 
                    TO_CHAR(SUBSTR( BS_DATE((RD.START_DATE) - 1 + ROWNUM), 6, 2) ) AS MONTH,
                    TO_CHAR(fn_bs_month(SUBSTR( BS_DATE((RD.START_DATE) - 1 + ROWNUM), 6, 2))) AS MONTH_NAME,
                    TO_CHAR (SUBSTR( BS_DATE((RD.START_DATE) - 1 + ROWNUM), 9, 2)) AS DAY
                    FROM ALL_OBJECTS O, DIST_ROUTE_PLAN RD
                    WHERE TO_DATE(RD.START_DATE) - 1 + ROWNUM <= TO_DATE(RD.END_DATE)
                    AND RD.PLAN_CODE='{plancode}'
                    ORDER BY ((RD.START_DATE) - 1 + ROWNUM)";
                List<RoutePlanDateSeries> lst = this._dbContext.SqlQuery<RoutePlanDateSeries>(query).ToList();
                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<FrequencyModels> getFrequencyByFilter(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }

                var sqlquery = string.Format(@"SELECT TIME_FRAME_CODE,TIME_FRAME_EDESC FROM PL_TIME_FRAME
                            where deleted_flag='N' 
                            and (TIME_FRAME_CODE like '%{0}%' 
                            or upper(TIME_FRAME_EDESC) like '%{0}%')",
                            filter.ToUpperInvariant());
                var result = _dbContext.SqlQuery<FrequencyModels>(sqlquery).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool SaveEmployeeRoutePlan(List<DIST_ROUTE_DETAIL> routeDetailList)
        {
            try
            {
                DateTime today = DateTime.Today.Date;

                routeDetailList.ToList().ForEach(a =>
                {
                    a.DELETED_FLAG = "N";
                    a.CREATED_BY = _workcontext.CurrentUserinformation.login_code;
                    a.CREATED_DATE = today;
                    a.MODIFIED_DATE = today;
                    a.COMPANY_CODE = _workcontext.CurrentUserinformation.company_code;
                    a.BRANCH_CODE = _workcontext.CurrentUserinformation.branch_code;
                });

                if (routeDetailList.Count > 0)
                {
                    string deleteQuery = $@"DELETE FROM DIST_ROUTE_DETAIL WHERE PLAN_CODE='{routeDetailList[0].PLAN_CODE}' ";
                    this._dbContext.ExecuteSqlCommand(deleteQuery);
                }

                foreach (var item in routeDetailList)
                {
                    string query = string.Empty;
                    query = $@"INSERT INTO DIST_ROUTE_DETAIL
                            (ROUTE_CODE,EMP_CODE,ASSIGN_DATE,CREATED_DATE,CREATED_BY,MODIFY_DATE,COMPANY_CODE,BRANCH_CODE,PLAN_CODE,DELETED_FLAG)
                            VALUES('{item.ROUTE_CODE}','{item.EMP_CODE}',TO_DATE('{item.ASSIGN_DATE.ToString("MM/dd/yyyy")}','MM/DD/YYYY'),TO_DATE('{item.CREATED_DATE.ToString("MM/dd/yyyy")}','MM/DD/YYYY'),'{item.CREATED_BY}',TO_DATE('{item.MODIFIED_DATE.ToString("MM/dd/yyyy")}','MM/DD/YYYY'),'{item.COMPANY_CODE}','{item.BRANCH_CODE}','{item.PLAN_CODE}','N')";
                    int resultCount = this._dbContext.ExecuteSqlCommand(query.ToString());
                    if (resultCount < 0)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public List<DIST_ROUTE_DETAIL> fetchAssignedEmployeesOfRoute(string plancode)
        {
            List<DIST_ROUTE_DETAIL> list = new List<DIST_ROUTE_DETAIL>();
            string query = $@"SELECT ROUTE_CODE,EMP_CODE,ASSIGN_DATE,CREATED_DATE,MODIFY_DATE,MODIFY_BY,COMPANY_CODE,DELETED_FLAG,PLAN_CODE FROM DIST_ROUTE_DETAIL WHERE PLAN_CODE='{plancode}'";
            list = this._dbContext.SqlQuery<DIST_ROUTE_DETAIL>(query).ToList();
            return list;
        }

        public string removeRouteFromPlan(string plancode, string routecode)
        {
            try
            {
                string query_check_isEmployeeAssigned = $@"select COUNT(*) from DIST_ROUTE_PLAN_DETAIL PD, DIST_ROUTE_DETAIL RD
                        where PD.ROUTE_CODE=RD.ROUTE_CODE
                        and PD.PLAN_CODE=RD.PLAN_CODE
                        and PD.plan_CODE='{plancode}'
                        and RD.ROUTE_CODE='{routecode}'";
                int count_no_employeeAssigned = this._dbContext.SqlQuery<int>(query_check_isEmployeeAssigned).First();
                if (count_no_employeeAssigned > 0)
                {
                    return "Employee has assigned into this route. To delete this route from plan, first remove assigned employee.";
                }
                else
                {
                    string delete_query = $@"DELETE FROM DIST_ROUTE_PLAN_DETAIL WHERE PLAN_CODE='{plancode}' AND ROUTE_CODE='{routecode}'";
                    int delete_count = this._dbContext.ExecuteSqlCommand(delete_query);
                    return "success";
                }
            }
            catch (Exception ex)
            {
                return ex.InnerException.Message;
            }
        }
        public string saveCalendarRoute(DIST_CALENDAR_ROUTE model)
        {
            try
            {
                string insertedPlancode = model.planCode;
                string message = "";
                var rowCount = 0;
                var currentdate = DateTime.Now.ToString("MM/dd/yyyy");
                var currentUserId = _workcontext.CurrentUserinformation.User_id;
                var company_code = _workcontext.CurrentUserinformation.company_code;
                if (model.addEdit == "Edit")
                {
                    var route_plan_query = $@"UPDATE DIST_ROUTE_PLAN SET PLAN_EDESC='{model.planName}',START_DATE=TO_DATE('{model.startDate}','mm/dd/yyyy'), END_DATE=TO_DATE('{model.endDate}','mm/dd/yyyy'),COMPANY_CODE = '{company_code}', CREATED_BY='{currentUserId}',CREATED_DATE=SYSDATE WHERE PLAN_CODE = '{model.planCode}'";
                    rowCount = _dbContext.ExecuteSqlCommand(route_plan_query);
                    var route_plan_detail_query = $@"DELETE FROM DIST_ROUTE_PLAN_DETAIL WHERE PLAN_CODE = '{model.planCode}'";
                    var planDetail = _dbContext.ExecuteSqlCommand(route_plan_detail_query);
                    var route_detail_query = $@"DELETE FROM DIST_ROUTE_DETAIL WHERE PLAN_CODE = '{model.planCode}'";
                    var routeDetail = _dbContext.ExecuteSqlCommand(route_detail_query);

                }
                else
                {
                    var checkPlanQuery = $@"Select Count(*) from DIST_ROUTE_PLAN where PLAN_EDESC='{model.planName}'";
                    int countPlan = this._dbContext.SqlQuery<int>(checkPlanQuery).First();
                    if (countPlan >= 1)
                    {
                        return "Plan Name Already Exists";
                    }
                    var insertQuery = $@"INSERT INTO DIST_ROUTE_PLAN(PLAN_EDESC ,START_DATE, END_DATE, TIME_FRAME_CODE , COMPANY_CODE ,CREATED_BY ,CREATED_DATE ,DELETED_FLAG)
                    VALUES('{model.planName}',TO_DATE('{model.startDate}','mm/dd/yyyy'),TO_DATE('{model.endDate}','mm/dd/yyyy'),'202','{company_code}','{currentUserId}',TO_DATE('{currentdate}','mm/dd/yyyy'),'N')";
                    rowCount = _dbContext.ExecuteSqlCommand(insertQuery);
                    _dbContext.SaveChanges();
                    string query_plancode = $@"SELECT PLAN_CODE FROM DIST_ROUTE_PLAN WHERE PLAN_EDESC='{model.planName}' AND START_DATE=TO_DATE('{model.startDate}','mm/dd/yyyy') AND END_DATE=TO_DATE('{model.endDate}','mm/dd/yyyy') AND TIME_FRAME_CODE='202'";
                    insertedPlancode = this._dbContext.SqlQuery<Int64>(query_plancode).FirstOrDefault().ToString();
                }

                if (rowCount > 0 && !string.IsNullOrEmpty(insertedPlancode))
                {
                    var routeCode = "";
                    var arrayList = model.eventArr;

                    var distinct = arrayList.Select(x => x.routeCode).Distinct();
                    foreach (var item in distinct)
                    {
                        var Query = $@"INSERT INTO DIST_ROUTE_PLAN_DETAIL(PLAN_CODE , ROUTE_CODE , COMPANY_CODE ,CREATED_BY ,CREATED_DATE ,DELETED_FLAG)
                            VALUES('{insertedPlancode}','{item}','{company_code}','{currentUserId}',TO_DATE('{currentdate}','MM/DD/YYYY'),'N')";
                        var rowNum = _dbContext.ExecuteSqlCommand(Query);
                    }
                    foreach (var item in model.eventArr)
                    {
                        //if (routeCode != item.routeCode)
                        //{
                        //    var Query = $@"INSERT INTO DIST_ROUTE_PLAN_DETAIL(PLAN_CODE , ROUTE_CODE , COMPANY_CODE ,CREATED_BY ,CREATED_DATE ,DELETED_FLAG)
                        //    VALUES('{insertedPlancode}','{item.routeCode}','{company_code}','{currentUserId}',TO_DATE('{currentdate}','MM/DD/YYYY'),'N')";
                        //    var rowNum = _dbContext.ExecuteSqlCommand(Query);
                        //}
                        var query = $@"INSERT INTO DIST_ROUTE_DETAIL( ROUTE_CODE ,EMP_CODE, PLAN_CODE, ASSIGN_DATE, COMPANY_CODE ,CREATED_BY ,CREATED_DATE ,DELETED_FLAG)
                            VALUES('{item.routeCode}','{model.empCode}','{insertedPlancode}',TO_DATE('{item.start}','MM/DD/YYYY'),'{company_code}','{currentUserId}',TO_DATE('{currentdate}','MM/DD/YYYY'),'N')";
                        _dbContext.ExecuteSqlCommand(query);

                        _dbContext.SaveChanges();
                        message = "SUCCESS";
                        routeCode = item.routeCode;
                    }
                }
                return message;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public string saveCalendarBrandingRoute(DIST_CALENDAR_ROUTE model)
        {
            try
            {
                string insertedPlancode = model.planCode;
                string message = "";
                var rowCount = 0;
                var currentdate = DateTime.Now.ToString("MM/dd/yyyy");
                var currentUserId = _workcontext.CurrentUserinformation.User_id;
                var company_code = _workcontext.CurrentUserinformation.company_code;
                if (model.addEdit == "Update")
                {
                    var route_plan_query = $@"UPDATE DIST_ROUTE_PLAN SET PLAN_EDESC='{model.planName}',START_DATE=TO_DATE('{model.startDate}','mm/dd/yyyy'), END_DATE=TO_DATE('{model.endDate}','mm/dd/yyyy'),COMPANY_CODE = '{company_code}', CREATED_BY='{currentUserId}',CREATED_DATE=SYSDATE WHERE PLAN_CODE = '{model.planCode}'";
                    rowCount = _dbContext.ExecuteSqlCommand(route_plan_query);
                    var route_plan_detail_query = $@"DELETE FROM DIST_ROUTE_PLAN_DETAIL WHERE PLAN_CODE = '{model.planCode}'";
                    var planDetail = _dbContext.ExecuteSqlCommand(route_plan_detail_query);
                    var route_detail_query = $@"DELETE FROM DIST_BRANDING_ROUTE_DETAIL WHERE PLAN_CODE = '{model.planCode}'";
                    var routeDetail = _dbContext.ExecuteSqlCommand(route_detail_query);

                }
                else
                {
                    var insertQuery = $@"INSERT INTO DIST_ROUTE_PLAN(PLAN_EDESC ,START_DATE, END_DATE, TIME_FRAME_CODE , COMPANY_CODE ,CREATED_BY ,CREATED_DATE ,DELETED_FLAG,ROUTE_TYPE)
                    VALUES('{model.planName}',TO_DATE('{model.startDate}','mm/dd/yyyy'),TO_DATE('{model.endDate}','mm/dd/yyyy'),'202','{company_code}','{currentUserId}',TO_DATE('{currentdate}','mm/dd/yyyy'),'N','B')";
                    rowCount = _dbContext.ExecuteSqlCommand(insertQuery);
                    _dbContext.SaveChanges();
                    string query_plancode = $@"SELECT PLAN_CODE FROM DIST_ROUTE_PLAN WHERE PLAN_EDESC='{model.planName}' AND START_DATE=TO_DATE('{model.startDate}','mm/dd/yyyy') AND END_DATE=TO_DATE('{model.endDate}','mm/dd/yyyy') AND TIME_FRAME_CODE='202' AND ROUTE_TYPE='B' ";
                    insertedPlancode = this._dbContext.SqlQuery<Int64>(query_plancode).FirstOrDefault().ToString();
                }

                if (rowCount > 0 && !string.IsNullOrEmpty(insertedPlancode))
                {
                    var routeCode = "";
                    var arrayList = model.eventArr;

                    var distinct = arrayList.Select(x => x.routeCode).Distinct();
                    foreach (var item in distinct)
                    {
                        var Query = $@"INSERT INTO DIST_ROUTE_PLAN_DETAIL(PLAN_CODE , ROUTE_CODE , COMPANY_CODE ,CREATED_BY ,CREATED_DATE ,DELETED_FLAG,ROUTE_TYPE)
                            VALUES('{insertedPlancode}','{item}','{company_code}','{currentUserId}',TO_DATE('{currentdate}','MM/DD/YYYY'),'N','B')";
                        var rowNum = _dbContext.ExecuteSqlCommand(Query);
                    }
                    foreach (var item in model.eventArr)
                    {
                        //if (routeCode != item.routeCode)
                        //{
                        //    var Query = $@"INSERT INTO DIST_ROUTE_PLAN_DETAIL(PLAN_CODE , ROUTE_CODE , COMPANY_CODE ,CREATED_BY ,CREATED_DATE ,DELETED_FLAG)
                        //    VALUES('{insertedPlancode}','{item.routeCode}','{company_code}','{currentUserId}',TO_DATE('{currentdate}','MM/DD/YYYY'),'N')";
                        //    var rowNum = _dbContext.ExecuteSqlCommand(Query);
                        //}
                        var query = $@"INSERT INTO DIST_BRANDING_ROUTE_DETAIL( ROUTE_CODE ,EMP_CODE, PLAN_CODE, ASSIGN_DATE, COMPANY_CODE ,CREATED_BY ,CREATED_DATE ,DELETED_FLAG)
                            VALUES('{item.routeCode}','{model.empCode}','{insertedPlancode}',TO_DATE('{item.start}','MM/DD/YYYY'),'{company_code}','{currentUserId}',TO_DATE('{currentdate}','MM/DD/YYYY'),'N')";
                        _dbContext.ExecuteSqlCommand(query);

                        _dbContext.SaveChanges();
                        message = "SUCCESS";
                        routeCode = item.routeCode;
                    }
                }
                return message;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public string addRoutesToPlan(string plancode, string routecode)
        {
            try
            {
                DateTime today = DateTime.Today.Date;
                string userid = _workcontext.CurrentUserinformation.login_code,
                    company_code = _workcontext.CurrentUserinformation.company_code,
                    branch_code = _workcontext.CurrentUserinformation.branch_code;

                string message = "success ";
                string[] routeCodes = routecode.Split(',');
                string insertQuery = string.Empty;

                foreach (var item in routeCodes)
                {
                    string checkRoutesInPlanQuery = $@"SELECT COUNT(*) FROM DIST_ROUTE_PLAN_DETAIL WHERE PLAN_CODE='{plancode}' AND ROUTE_CODE='{item}'";
                    int exisRows = this._dbContext.SqlQuery<int>(checkRoutesInPlanQuery).First();
                    if (exisRows > 0)
                    {
                        message += "\nRoute code " + item + " has already assigned.";
                    }
                    else
                    {
                        insertQuery += $@" INTO DIST_ROUTE_PLAN_DETAIL(PLAN_CODE,ROUTE_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG)
                                                VALUES('{plancode}','{item}','{company_code}','{branch_code}','{userid}',TO_DATE('{today.ToShortDateString()}','MM/DD/YYYY'),'N') ";
                    }
                }
                if (!string.IsNullOrEmpty(insertQuery))
                {
                    string insertallquery = "INSERT ALL " + insertQuery + " SELECT * FROM DUAL";
                    var result = this._dbContext.ExecuteSqlCommand(insertallquery);
                }
                return message;
            }
            catch (Exception ex)
            {
                return "Error: " + ex.InnerException.Message;
            }
        }

        public string saveExcelPlan(HttpPostedFile file)
        {
            string response = String.Empty;
            try
            {
                if (file == null || file.ContentLength == 0)
                {

                    return "Empty File";
                }
                else
                {

                    DataSet dsexcelRecords = new DataSet();
                    IExcelDataReader reader = null;
                    HttpPostedFile Inputfile = null;
                    Stream FileStream = null;
                    FileStream = file.InputStream;
                    if (file.FileName.EndsWith(".xls"))
                    {
                        reader = ExcelReaderFactory.CreateBinaryReader(FileStream);
                    }
                    else if (file.FileName.EndsWith(".xlsx"))
                    {
                        reader = ExcelReaderFactory.CreateOpenXmlReader(FileStream);
                    }
                    dsexcelRecords = reader.AsDataSet();
                    reader.Close();

                    if (dsexcelRecords != null && dsexcelRecords.Tables.Count > 0)
                    {
                        DataTable dtStudentRecords = dsexcelRecords.Tables[0];
                        List<ExcelRoutePlan> planlst = new List<ExcelRoutePlan>();
                        for (int i = 1; i < dtStudentRecords.Rows.Count; i++)
                        {
                            //for(int j=1; j < dtStudentRecords.Columns.Count; j++)
                            //{

                            //}
                            DateTime today = DateTime.Parse(dtStudentRecords.Rows[i][1].ToString());
                            var td = today.ToShortDateString();
                            if (String.IsNullOrEmpty(dtStudentRecords.Rows[i][0].ToString()))
                            {
                                return "Plan Name Empty!";
                            }

                            else if (String.IsNullOrEmpty(dtStudentRecords.Rows[i][1].ToString()))
                            {
                                return "English Date  Empty!";
                            }
                            else if (DateTime.Parse(dtStudentRecords.Rows[i][1].ToString()) < DateTime.Today)
                            {
                                return "Invalid Date!";
                            }
                            else if (String.IsNullOrEmpty(dtStudentRecords.Rows[i][6].ToString()))
                            {
                                return "Route Code Empty!";
                            }
                            else if (String.IsNullOrEmpty(dtStudentRecords.Rows[i][8].ToString()))
                            {
                                return "Employee Code Empty!";
                            }
                            else
                            {
                                ExcelRoutePlan plan = new ExcelRoutePlan();
                                plan.PlanName = dtStudentRecords.Rows[i][0].ToString().Trim();
                                plan.AssignDate = dtStudentRecords.Rows[i][1].ToString().Trim();
                                //start date and end date same as assign date as there is no start date and end date in excel
                                plan.StartDate = dtStudentRecords.Rows[i][1].ToString().Trim();
                                plan.EndDate = dtStudentRecords.Rows[i][1].ToString().Trim();
                                plan.RouteName = String.IsNullOrEmpty(dtStudentRecords.Rows[i][7].ToString()) ? "" : dtStudentRecords.Rows[i][7].ToString().Trim();
                                plan.RouteCode = String.IsNullOrEmpty(dtStudentRecords.Rows[i][6].ToString()) ? "" : dtStudentRecords.Rows[i][6].ToString().Trim();
                                plan.EmployeeCode = String.IsNullOrEmpty(dtStudentRecords.Rows[i][8].ToString()) ? "" : dtStudentRecords.Rows[i][8].ToString();
                                plan.EmployeeName = String.IsNullOrEmpty(dtStudentRecords.Rows[i][9].ToString()) ? "" : dtStudentRecords.Rows[i][9].ToString().Trim();

                                planlst.Add(plan);
                            }

                        }
                        var groupedPlan = planlst.GroupBy(x => x.PlanName).Select(y => y.ToList()).ToList();
                        DIST_CALENDAR_ROUTE route = new DIST_CALENDAR_ROUTE();
                        string message = String.Empty;
                        int count = 0;
                        if (groupedPlan.Count > 1)
                        {
                            //List<object> planlist = new List<object>();
                            foreach (var plan in groupedPlan)
                            {
                                //var groupedCode = plan.GroupBy(x => x.RouteCode).Select(y => y.ToList()).ToList();
                                //foreach (var group in groupedCode)
                                //{

                                var groupedEmployee = plan.GroupBy(x => x.EmployeeCode).Select(y => y.ToList()).ToList();
                                route.addEdit = "new";

                                if (groupedEmployee.Count > 1)
                                {
                                    foreach (var empgroup in groupedEmployee)
                                    {
                                        route.planName = empgroup[0].PlanName;
                                        route.startDate = DateTime.Parse(empgroup[0].StartDate).ToString("MM-dd-yyyy");
                                        route.endDate = DateTime.Parse(empgroup[0].EndDate).ToString("MM-dd-yyyy");
                                        route.empCode = empgroup[0].EmployeeCode;
                                        List<ModelData> datalst = new List<ModelData>();
                                        for (int x = 0; x < empgroup.Count; x++)
                                        {
                                            ModelData data = new ModelData();
                                            data.routeCode = empgroup[x].RouteCode;
                                            data.start = DateTime.Parse(empgroup[x].AssignDate).ToString("MM-dd-yyyy");
                                            datalst.Add(data);
                                        }
                                        route.eventArr = datalst;
                                        saveCalendarRoute(route);


                                    }

                                }
                                else
                                {
                                    route.planName = plan[0].PlanName;
                                    route.startDate = DateTime.Parse(plan[0].StartDate).ToString("MM-dd-yyyy");
                                    route.endDate = DateTime.Parse(plan[0].EndDate).ToString("MM-dd-yyyy");
                                    route.empCode = plan[0].EmployeeCode;
                                    List<ModelData> datalst = new List<ModelData>();
                                    for (int i = 0; i < plan.Count; i++)
                                    {
                                        ModelData data = new ModelData();
                                        data.routeCode = plan[i].RouteCode;
                                        data.start = DateTime.Parse(plan[i].AssignDate).ToString("MM-dd-yyyy");
                                        datalst.Add(data);

                                    }
                                    route.eventArr = datalst;
                                    response = saveCalendarRoute(route);

                                }

                                //}
                            }

                        }
                        else
                        {
                            route.planName = planlst[0].PlanName;
                            route.startDate = DateTime.Parse(planlst[0].StartDate).ToString("MM-dd-yyyy");
                            route.endDate = DateTime.Parse(planlst[0].EndDate).ToString("MM-dd-yyyy");
                            route.empCode = planlst[0].EmployeeCode;
                            List<ModelData> datalst = new List<ModelData>();
                            for (int i = 0; i < planlst.Count; i++)
                            {
                                ModelData data = new ModelData();
                                data.routeCode = planlst[i].RouteCode;
                                data.start = DateTime.Parse(planlst[i].AssignDate).ToString("MM-dd-yyyy");
                                datalst.Add(data);

                            }
                            route.eventArr = datalst;
                            response = saveCalendarRoute(route);



                        }

                    }
                    else
                    {
                        return "No data";
                    }
                    return response;

                    //    var file = plan.file;
                    //    try
                    //    {
                    //        if (file == null || file.ContentLength == 0)
                    //        {

                    //            return "Empty File";
                    //        }
                    //        else
                    //        {
                    //            DataSet dsexcelRecords = new DataSet();
                    //            IExcelDataReader reader = null;
                    //            HttpPostedFile Inputfile = null;
                    //            Stream FileStream = null;
                    //            FileStream = file.InputStream;
                    //            DateTime fromDate = DateTime.Parse(plan.frmdate);
                    //            DateTime toDate = DateTime.Parse(plan.todate);

                    //            if (file.FileName.EndsWith(".xls"))
                    //            {
                    //                reader = ExcelReaderFactory.CreateBinaryReader(FileStream);
                    //            }
                    //            else if (file.FileName.EndsWith(".xlsx"))
                    //            {
                    //                reader = ExcelReaderFactory.CreateOpenXmlReader(FileStream);
                    //            }
                    //            else
                    //            {
                    //                //message = "The file format is not supported.";
                    //            }
                    //            dsexcelRecords = reader.AsDataSet();
                    //            reader.Close();
                    //            DIST_CALENDAR_ROUTE route = new DIST_CALENDAR_ROUTE();
                    //            route.empCode = plan.empCode;
                    //            route.planName = plan.PlanName;
                    //            if (dsexcelRecords != null && dsexcelRecords.Tables.Count > 0)
                    //            {
                    //                DataTable dtStudentRecords = dsexcelRecords.Tables[0];
                    //                route.addEdit = "New";
                    //                route.startDate = fromDate.ToString("MM/dd/yyyy");
                    //                route.endDate = toDate.ToString("MM/dd/yyyy");
                    //                List<ModelData> datalst = new List<ModelData>();
                    //                for (int i = 1; i < dtStudentRecords.Rows.Count; i++)
                    //                {
                    //                    if (String.IsNullOrEmpty(dtStudentRecords.Rows[i][1].ToString()))
                    //                    {
                    //                        continue;
                    //                    }
                    //                    else
                    //                    {

                    //                        DateTime startDate = DateTime.Parse(dtStudentRecords.Rows[i][1].ToString());
                    //                        int result = DateTime.Compare(startDate, fromDate);
                    //                        //< 0 − If startDate is earlier than fromDate
                    //                        //0 − If startDate is the same as fromDate
                    //                        //> 0 − If startDate is later than fromDate
                    //                        if (result == -1)
                    //                        {

                    //                            return "Invalid Date";
                    //                        }
                    //                        else
                    //                        {
                    //                            int count = 0;
                    //                            double frequency = double.Parse(dtStudentRecords.Rows[i][2].ToString());
                    //                            while (count != 1)
                    //                            {
                    //                                ModelData data = new ModelData();
                    //                                data.start = startDate.ToString("MM/dd/yyyy");
                    //                                data.routeCode = dtStudentRecords.Rows[i][0].ToString();
                    //                                datalst.Add(data);
                    //                                startDate = startDate.AddDays(frequency);
                    //                                count = DateTime.Compare(startDate, toDate);
                    //                            }

                    //                        }


                    //                    }
                    //                }
                    //                route.eventArr = datalst;                     

                    //            }
                    //            string message = saveCalendarRoute(route);
                    //            return message;
                    //        }


                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        return "Something went wrong!";
                    //    }

                }
            }
            catch (Exception ex)
            {
                return ex.ToString();

            }
        }
        public List<ItemGroupModel> GetItemGroup(string filter)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(filter))
                    filter = $@" AND LOWER(TRIM(item_edesc)) LIKE '%{filter.ToLower()}%'";

                string query = $@"select item_code,item_edesc,MASTER_ITEM_CODE,PRE_ITEM_CODE from ip_item_master_setup where GROUP_SKU_FLAG='G' and  DELETED_FLAG = 'N' AND CATEGORY_CODE in (select category_code from IP_CATEGORY_CODE where category_type='FG') AND COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}' {filter} ORDER BY ITEM_CODE";

                query = $@"select item_code,item_edesc,MASTER_ITEM_CODE,PRE_ITEM_CODE from ip_item_master_setup where GROUP_SKU_FLAG='G' and  DELETED_FLAG = 'N'  AND COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}' {filter} ORDER BY ITEM_CODE";



                List<ItemGroupModel> itemGroup = this._dbContext.SqlQuery<ItemGroupModel>(query).ToList();
                return itemGroup;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<ItemGroupModel> GetItemLists(string filter, string itmGroup)
        {
            try
            {
                var condition = string.Empty;

                if (itmGroup != "" && itmGroup != null)
                    condition = $@" AND PRE_ITEM_CODE in ({itmGroup})";

                if (!string.IsNullOrWhiteSpace(filter))
                    filter = $@" AND LOWER(TRIM(item_edesc)) LIKE '%{filter.ToLower()}%'";

                string query = $@"select item_code,item_edesc,INDEX_MU_CODE as mu_code from ip_item_master_setup where GROUP_SKU_FLAG='I' and  DELETED_FLAG = 'N' AND COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}' {condition} {filter} ORDER BY ITEM_CODE";
                List<ItemGroupModel> itemGroup = this._dbContext.SqlQuery<ItemGroupModel>(query).ToList();
                return itemGroup;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<CustomerGroupModel> GetCustomerLists(string filter, string cusGroup)
        {
            try
            {
                var condition = string.Empty;
                if (cusGroup != "" && cusGroup != null)
                    condition = $@" AND ddm.GROUPID in ({cusGroup})";

                if (!string.IsNullOrWhiteSpace(filter))
                    filter = $@" AND LOWER(TRIM(scs.CUSTOMER_EDESC)) LIKE '%{filter.ToLower()}%'";

                string query = $@"select scs.customer_code,scs.customer_edesc from dist_distributor_master ddm, sa_customer_setup scs where scs.customer_code=ddm.distributor_code and scs.company_code=ddm.company_code and scs.branch_code=ddm.branch_code
                and scs.deleted_flag='N' and ddm.deleted_flag='N' AND ddm.COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}' {condition} {filter} ORDER BY scs.CUSTOMER_CODE ";
                List<CustomerGroupModel> customers = this._dbContext.SqlQuery<CustomerGroupModel>(query).ToList();
                return customers;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<CustomerGroupModel> GetCustomerSNGLists(string filter, string cusGroup)
        {
            try
            {
                var condition = string.Empty;
                if (cusGroup != "" && cusGroup != null)
                    condition = $@" AND PRE_CUSTOMER_CODE in ({cusGroup})";

                if (!string.IsNullOrWhiteSpace(filter))
                    filter = $@" AND LOWER(TRIM(CUSTOMER_EDESC)) LIKE '%{filter.ToLower()}%'";

                string query = $@"select CUSTOMER_CODE,CUSTOMER_EDESC,MASTER_CUSTOMER_CODE,
            case when PRE_CUSTOMER_CODE='00' then null else PRE_CUSTOMER_CODE end as PRE_CUSTOMER_CODE,GROUP_SKU_FLAG
            from sa_customer_setup 
            where GROUP_SKU_FLAG='I' and  DELETED_FLAG = 'N'  
            AND COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}' {condition} {filter} ORDER BY MASTER_CUSTOMER_CODE,pre_customer_code ";
                List<CustomerGroupModel> itemGroup = this._dbContext.SqlQuery<CustomerGroupModel>(query).ToList();
                return itemGroup;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }




        public List<CustomerNode> GetAllSynCustomerGrp(string filter, string cusGroup, string TYPE, string ind)
        {
            try
            {
                var condition = string.Empty;



                if (TYPE == "CUSTOMER")
                {
                    if (cusGroup != "" && cusGroup != null)
                    {
                        condition = $@" AND master_customer_code like  '{cusGroup}%'";
                    }
                    if (!string.IsNullOrWhiteSpace(ind))
                    {
                        filter = $@" AND  customer_code ='{ind}'";
                    }
                }
                else if (TYPE == "ITEM")
                {

                    if (cusGroup != "" && cusGroup != null)
                    {
                        condition = $@" AND master_item_code like  '{cusGroup}%'";
                    }
                    if (!string.IsNullOrWhiteSpace(ind))
                    {
                        filter = $@" AND  item_code ='{ind}'";
                    }

                }








                string grp_res = "00";

                // Ensure cusGroup is not null
                if (!string.IsNullOrEmpty(cusGroup) && cusGroup.Length > 3)
                {
                    grp_res = cusGroup.Substring(0, cusGroup.Length - 3);
                }

                string query = "";

                string pre_master_code = "";




                if (TYPE == "CUSTOMER")
                {
                    if (!string.IsNullOrWhiteSpace(ind))
                    {

                        pre_master_code = $@" '' PRE_CUSTOMER_CODE ";
                    }
                    else
                    {
                        pre_master_code = $@" case when PRE_CUSTOMER_CODE='{grp_res}' then null else PRE_CUSTOMER_CODE end as PRE_CUSTOMER_CODE ";
                    }

                }
                else if (TYPE == "ITEM")
                {
                    if (!string.IsNullOrWhiteSpace(ind))
                    {

                        pre_master_code = $@" '' PRE_item_CODE ";
                    }
                    else
                    {
                        pre_master_code = $@" case when PRE_item_CODE='{grp_res}' then null else PRE_item_CODE end as PRE_CUSTOMER_CODE ";
                    }


                }








                if (TYPE == "CUSTOMER")
                {

                    query = $@"select CUSTOMER_CODE,CUSTOMER_EDESC,MASTER_CUSTOMER_CODE,
                        {pre_master_code},GROUP_SKU_FLAG
                        from sa_customer_setup 
                        where DELETED_FLAG = 'N'   {condition} 
                        AND COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}'  {filter} ORDER BY MASTER_CUSTOMER_CODE,pre_customer_code ";

                }
                else if (TYPE == "ITEM")
                {

                    query = $@"select item_code CUSTOMER_CODE,item_edesc CUSTOMER_EDESC,MASTER_item_CODE MASTER_CUSTOMER_CODE,
                        {pre_master_code},GROUP_SKU_FLAG
                        from ip_item_master_setup
                        where DELETED_FLAG = 'N'   {condition}
                        AND COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}'  {filter} ORDER BY MASTER_item_CODE,pre_item_code ";


                }






                List<CustomerNode> itemGroup = this._dbContext.SqlQuery<CustomerNode>(query).ToList();
                // Return the transformed data
                return itemGroup;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }




        public List<CustomerGroup> GetCustomerGroup(string filter)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(filter))
                    filter = $@" AND LOWER(TRIM(GROUP_EDESC)) LIKE '%{filter.ToLower()}%'";
                string query = $@"select GROUPID as GROUP_ID,GROUP_EDESC,GROUP_CODE from dist_group_master where DELETED_FLAG = 'N'  AND COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}' {filter} ORDER BY GROUP_ID";
                List<CustomerGroup> customerGroup = this._dbContext.SqlQuery<CustomerGroup>(query).ToList();
                return customerGroup;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<CustomerSNGroup> GetCustomerSNGGroup(string filter)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(filter))
                    filter = $@" AND LOWER(TRIM(CUSTOMER_EDESC)) LIKE '%{filter.ToLower()}%'";

                string query = $@"
                select CUSTOMER_CODE as GROUP_ID,CUSTOMER_EDESC as GROUP_EDESC
                ,MASTER_CUSTOMER_CODE,case when PRE_CUSTOMER_CODE='00' then null else PRE_CUSTOMER_CODE end as PRE_CUSTOMER_CODE 
                from sa_customer_setup where GROUP_SKU_FLAG='G' and  DELETED_FLAG = 'N'  
                AND COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}' {filter} ORDER BY CUSTOMER_CODE ";
                List<CustomerSNGroup> cusGroup = this._dbContext.SqlQuery<CustomerSNGroup>(query).ToList();
                return cusGroup;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }




        public List<CustomerSNGroup> GetGroupEmployees(string filter)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(filter))
                    filter = $@" AND LOWER(TRIM(EMPLOYEE_EDESC)) LIKE '%{filter.ToLower()}%'";
                string query = $@"select employee_code as GROUP_ID,employee_edesc as GROUP_EDESC,MASTER_EMPLOYEE_CODE as MASTER_CUSTOMER_CODE,PRE_EMPLOYEE_CODE as PRE_CUSTOMER_CODE from HR_EMPLOYEE_SETUP where group_sku_flag='G' and deleted_flag='N' and COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}' {filter}";
                var result = this._dbContext.SqlQuery<CustomerSNGroup>(query).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<HolidayModel> GetHolidayDetails(string fromDate, string toDate)
        {
            try
            {
                string query = $@"
                    SELECT *
                    FROM (
                        SELECT to_char(start_date + LEVEL - 1) AS HOLIDAY_DATE
                        FROM hris_holiday_master_setup                                
                        CONNECT BY LEVEL <= (end_date - start_date + 1)
                        AND PRIOR start_date = start_date
                        AND PRIOR DBMS_RANDOM.VALUE IS NOT NULL
                        ORDER BY 1
                    ) 
                    WHERE HOLIDAY_DATE BETWEEN TO_DATE('{fromDate}', 'DD-MON-YYYY') 
                           AND TO_DATE('{toDate}', 'DD-MON-YYYY')
                ";


                var result = this._dbContext.SqlQuery<HolidayModel>(query).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public class TreeNodeModel
        {
            public List<string> ParentId { get; set; }
            public string Id { get; set; }       
            public string Name { get; set; }   
            public string Type { get; set; }     
        }

        public class SelectedEmployeeDto
        {
            public string UserId { get; set; }
            public string UserName { get; set; }
        }
        public class EmployeeTreeRequest
        {
            public string Type { get; set; }
            public string SubTarget { get; set; }
            public List<SelectedEmployeeDto> SelectedEmployees { get; set; }
        }

        public List<TreeNodeModel> GetEmployeeTree(EmployeeTreeRequest request)
        {
            if (request == null || request.SelectedEmployees == null)
                return new List<TreeNodeModel>();

            var treeList = new List<TreeNodeModel>();
            foreach (var emp in request.SelectedEmployees)
            {
                treeList.Add(new TreeNodeModel
                {
                    Id = emp.UserId,
                    Name = emp.UserName,
                    Type = "Employee",
                    ParentId = new List<string>()
                });
            }

            return treeList;
        }



        public class CustomersByEmployeeRequest
        {
            public string EmployeeId { get; set; } 
            public List<string> SelectedCustomerCodes { get; set; } = new List<string>();
        }

        public List<TreeNodeModel> GetCustomersByEmployee(List<dynamic> employeeIds, List<dynamic> selectedCustomerCodes)
        {
            var treeList = new List<TreeNodeModel>();

            foreach (var empId in employeeIds)
            {
                var empNode = new TreeNodeModel
                {
                    Id = empId.Id,
                    Type = "Employee",
                    Name = empId.Name,
                    ParentId = new List<string>()
                };

                treeList.Add(empNode);

                foreach (var customerCode in selectedCustomerCodes)
                {
                    var custNode = new TreeNodeModel
                    {
                        Id = customerCode.CUSTOMER_CODE,
                        Type = "Customer",
                        Name = customerCode.CUSTOMER_EDESC,
                        ParentId = new List<string> { (string) empId.Id } 
                    };

                    treeList.Add(custNode);
                }
            }

            return treeList;
        }




        public string saveTargetData(ProfileModel model)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    string message = "";
                    if (model.Employees.Count > 0)
                    {
                        if (model.TargetType == "SAL")
                        {
                            saveData(model);
                        }
                        else
                        {
                            saveColData(model);
                        }
                    }
                    else
                    {
                        if (model.FLAG == "SNG")
                        {
                            model.Employees = getSynergyEmployees(model.EmployeeMasterGroup);
                        }
                        else
                        {
                            model.Employees = getEmployees(model.EmployeeMasterGroup);
                        }
                        if (model.TargetType == "SAL")
                        {
                            saveData(model);
                        }
                        else
                        {
                            saveColData(model);
                        }
                    }
                    transaction.Commit();
                    message = "success";
                    return message;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return ex.ToString();
                }
            }
        }

        public class DateRange
        {
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
        }



        public dynamic saveTargetDataNew(dynamic data)
        {
            try
            {

                string query = @"SELECT NVL(TO_CHAR(MAX(TO_NUMBER(TARGET_ID)) + 1), '1') TARGET_ID FROM ip_target_setup";


                var grp = "";

                if (data["subTargetType"] == "CUS")
                {

                    grp = data["customerGroup"];
                }
                else if (data["subTargetType"] == "ITM")
                {
                    grp = data["itemGroup"];


                }



                var targetId = this._dbContext.SqlQuery<string>(query).FirstOrDefault();


                var employee_code = data["employees"][0];

                query = $@"
                        SELECT TO_CHAR(startdate, 'DD-Mon-YYYY') AS startdate,
                               TO_CHAR(enddate, 'DD-Mon-YYYY') AS enddate
                        FROM v_date_range
                        WHERE rangename='{data["DateFilter"]}'
                ";



                List<DateRange> dateFilter = this._dbContext.SqlQuery<DateRange>(query).ToList();




                //var targetId = data["targetId"];
                var targetName = data["targetName"];

                var master_type = data["subTargetType"];

                List<string> insertValues = new List<string>();

                foreach (var customer in data["gridData"])
                {
                    string masterCode = customer["CUSTOMER_CODE"].ToString();

                    foreach (var key in customer.Properties())
                    {
                        string keyName = key.Name;
                        var value = customer[keyName];



                        if (keyName.StartsWith("qty"))
                        {
                            string vDate = DateTime.ParseExact(keyName.Substring(3), "yyyyMMdd", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                            string targetQuantity = customer[keyName] != null ? customer[keyName].ToString() : "0";
                            string amtkey = keyName;
                            amtkey = amtkey.Replace("qty", "amt");
                            string targetAmount = customer[amtkey] != null ? customer[amtkey].ToString() : "0";

                            if (targetAmount != "0" && targetQuantity != "0")
                            {
                                insertValues.Add($@"INTO ip_target_setup (target_Id ,target_Name,master_code, target_quantity, target_amount, FROM_DATE, END_DATE,ASSIGN_EMPLOYEE,TARGET_TYPE,MASTER_TYPE,FILTER_START_DATE,FILTER_END_DATE,ind,grp) 
                                       VALUES ('{targetId}','{targetName}','{masterCode}', {targetQuantity}, {targetAmount}, TO_DATE('{vDate}', 'YYYY-MM-DD'), TO_DATE('{vDate}', 'YYYY-MM-DD'),'{employee_code}','SAL','{master_type}','{dateFilter[0].StartDate}','{dateFilter[0].EndDate}','{data["ind"]}','{grp}')");
                            }
                        }
                    }
                }

                if (insertValues.Count > 0)
                {
                    string insertQuery = $@"
                        INSERT ALL
                        {string.Join("\n", insertValues)}
                        SELECT * FROM dual";
                    _dbContext.ExecuteSqlCommand(insertQuery);
                    _dbContext.ExecuteSqlCommand("commit");
                }


                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public dynamic SaveDNMTargets(dynamic data)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    // 1️⃣ Get or generate TARGET_ID
                    var targetId = string.Empty;
                    if (data["targetId"] != 0)
                    {
                        targetId = data["targetId"];
                        _dbContext.ExecuteSqlCommand($@"DELETE IP_TARGET_SETUP WHERE TARGET_ID = '{targetId}'");
                    }
                    else
                    {
                        string targetIdQuery = @"SELECT NVL(TO_CHAR(MAX(TO_NUMBER(TARGET_ID)) + 1), '1') TARGET_ID FROM ip_target_setup";
                        targetId = _dbContext.SqlQuery<string>(targetIdQuery).FirstOrDefault();
                    }

                    // 2️⃣ Basic info
                    string targetName = data["targetName"];
                    string targetType = "SAL";
                    string dateFilterFirst = data.DateFilter;
                    var frequency = Frequency[dateFilterFirst.ToLower()];
                    string datefilterred = string.Empty;
                    switch (frequency.ToLower())
                    {
                        case "thisyear": datefilterred = "This Year"; break;
                        case "q1": datefilterred = "First Quarter"; break;
                        case "q2": datefilterred = "Second Quarter"; break;
                        case "q3": datefilterred = "Third Quarter"; break;
                        case "q4": datefilterred = "Forth Quarter"; break;
                        case "monthly": datefilterred = "This Month"; break;
                        case "weekly": datefilterred = "This Week"; break;
                        default: throw new Exception("Invalid frequency type");
                    }

                    // 3️⃣ Date range
                    var dateQuery = $@"
                        SELECT startdate, enddate
                        FROM v_date_range
                        WHERE rangename = '{datefilterred}'";
                    var dateFilter = _dbContext.SqlQuery<DateRange>(dateQuery).FirstOrDefault();
                    if (dateFilter == null) throw new Exception("Invalid date filter.");

                    DateTime startDate = dateFilter.StartDate.Value;
                    DateTime endDate = dateFilter.EndDate.Value;

                    var company_code = _workcontext.CurrentUserinformation.company_code;
                    var branch_code = _workcontext.CurrentUserinformation.branch_code;
                    var login_code = _workcontext.CurrentUserinformation.login_code;

                    string startDateStr = startDate.ToString("yyyy-MM-dd");
                    string endDateStr = endDate.ToString("yyyy-MM-dd");

                    string monthName;
                    bool isAnnual = startDate.Month == 1 && startDate.Day == 1 &&
                                    endDate.Month == 12 && endDate.Day == 31 &&
                                    startDate.Year == endDate.Year;
                    monthName = isAnnual ? "Annual" : GetNepaliMonthFromEnglish(startDate);

                    // 4️⃣ Prepare lists
                    var empGroups = ((IEnumerable<object>)data.employeeGroup ?? Enumerable.Empty<object>()).Select(x => x.ToString()).ToList();
                    var customerGroups = ((IEnumerable<object>)data.customerGroup ?? Enumerable.Empty<object>()).Select(x => x.ToString()).ToList();
                    var employeeIds = ((IEnumerable<object>)data.empId ?? Enumerable.Empty<object>()).Select(x => x.ToString()).ToList();
                    var itemGroups = ((IEnumerable<object>)data.item_Group ?? Enumerable.Empty<object>()).Select(x => x.ToString()).ToList();
                    var individualCustomer = ((IEnumerable<object>)data.individualCusCode ?? Enumerable.Empty<object>()).Select(x => x.ToString()).ToList();

                    decimal targetQuantity = Convert.ToDecimal(data["targetQuantity"] ?? 0);
                    decimal targetAmount = Convert.ToDecimal(data["targetAmount"] ?? 0);

                    if (targetQuantity == 0 && targetAmount == 0)
                        throw new Exception("Target quantity and amount cannot be zero.");

                    // 5️⃣ Bulk insert using System.Data.OracleClient with parameters
                    string connStr = ConfigurationManager.ConnectionStrings["NeoErpCoreEntity"].ToString();
                    string[] tokens = connStr.Split('"'); // adjust if necessary
                    using (var conn = new OracleConnection(tokens[1]))
                    {
                        conn.Open();

                        string insertSql = @"
            INSERT INTO IP_TARGET_SETUP
            (TARGET_ID, TARGET_NAME, TARGET_TYPE, TARGET_QUANTITY, TARGET_AMOUNT, 
             FROM_DATE, END_DATE, ASSIGN_EMPLOYEE, EMPLOYEE_GROUP, ITEM_GROUP, CUSTOMER_GROUP,
             DATE_FILTER, MONTH, MASTER_CODE, COMPANY_CODE, BRANCH_CODE, CREATED_DATE, CREATED_BY, TARGET_SETUP_TYPE,
             INDIVIDUAL_CUSTOMERCODE)
            VALUES
            (:targetId, :targetName, :targetType, :targetQuantity, :targetAmount, 
             TO_DATE(:fromDate, 'YYYY-MM-DD'), TO_DATE(:toDate, 'YYYY-MM-DD'), 
             :empId, :empGroup, :itemGroup, :cusGroup, 
             :dateFilter, :monthName, :masterCode, :companyCode, :branchCode, SYSDATE, :loginCode, :setupType,
             :individualCusCode)";

                        using (var cmd = new OracleCommand())
                        {
                            cmd.Connection = conn;

                            // Hierarchy: EmployeeGroup -> Employee -> CustomerGroup -> Customer -> Selected Items
                            foreach (var empGroup in empGroups.DefaultIfEmpty(""))
                            {
                                foreach (var empId in employeeIds.DefaultIfEmpty(""))
                                {
                                    foreach (var cusGroup in customerGroups.DefaultIfEmpty(""))
                                    {
                                        foreach (var individualCusCode in individualCustomer.DefaultIfEmpty(""))
                                        {
                                            foreach (var item in itemGroups.DefaultIfEmpty(""))
                                            {
                                                cmd.CommandText = insertSql;
                                                cmd.Parameters.Clear();


                                                cmd.Parameters.Add(new OracleParameter("targetId", targetId?.ToString() ?? ""));
                                                cmd.Parameters.Add(new OracleParameter("targetName", targetName?.ToString() ?? ""));
                                                cmd.Parameters.Add(new OracleParameter("targetType", targetType?.ToString() ?? ""));
                                                cmd.Parameters.Add(new OracleParameter("targetQuantity", Convert.ToDecimal(targetQuantity)));
                                                cmd.Parameters.Add(new OracleParameter("targetAmount", Convert.ToDecimal(targetAmount)));
                                                cmd.Parameters.Add(new OracleParameter("fromDate", startDateStr?.ToString() ?? ""));
                                                cmd.Parameters.Add(new OracleParameter("toDate", endDateStr?.ToString() ?? ""));

                                                // Employee info
                                                cmd.Parameters.Add(new OracleParameter("empGroup", empGroup?.ToString() ?? ""));
                                                cmd.Parameters.Add(new OracleParameter("empId", empId?.ToString() ?? ""));

                                                cmd.Parameters.Add(new OracleParameter("cusGroup", cusGroup?.ToString() ?? ""));
                                                cmd.Parameters.Add(new OracleParameter("individualCusCode", individualCusCode?.ToString() ?? ""));

                                                cmd.Parameters.Add(new OracleParameter("itemGroup", item != null ? string.Join(",", item) : ""));

                                                // Misc info
                                                cmd.Parameters.Add(new OracleParameter("dateFilter", datefilterred?.ToString() ?? ""));
                                                cmd.Parameters.Add(new OracleParameter("monthName", monthName?.ToString() ?? ""));
                                                cmd.Parameters.Add(new OracleParameter("masterCode", data.masterCode?.ToString() ?? ""));
                                                cmd.Parameters.Add(new OracleParameter("companyCode", company_code?.ToString() ?? ""));
                                                cmd.Parameters.Add(new OracleParameter("branchCode", branch_code?.ToString() ?? ""));
                                                cmd.Parameters.Add(new OracleParameter("loginCode", login_code?.ToString() ?? ""));
                                                cmd.Parameters.Add(new OracleParameter("setupType", data.setupType?.ToString() ?? ""));


                                                cmd.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    transaction.Commit();
                    return new { success = true, message = "DNM Target saved successfully." };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new { success = false, message = ex.Message };
                }
            }
        }

        public dynamic SaveDNMSalesTarget(dynamic data)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    string targetId;

                    var targetIdStr = Convert.ToString(data.targetId);

                    if (!string.IsNullOrWhiteSpace(targetIdStr) && targetIdStr != "0")
                    {
                        targetId = targetIdStr;
                        _dbContext.ExecuteSqlCommand($@"DELETE IP_TARGET_SETUP WHERE TARGET_ID = '{targetId}'");
                    }
                    else
                    {
                        string targetIdQuery = @"SELECT NVL(TO_CHAR(MAX(TO_NUMBER(TARGET_ID)) + 1), '1') TARGET_ID FROM IP_TARGET_SETUP";
                        targetId = _dbContext.SqlQuery<string>(targetIdQuery).FirstOrDefault();
                    }
                    string targetName = data["targetName"];
                    string dateFilterStr = data.DateFilter?.ToString() ?? "";
                    var frequency = Frequency[dateFilterStr.ToLower()];

                    string datefilterred = string.Empty;
                    switch (frequency.ToLower())
                    {
                        case "thisyear": datefilterred = "This Year"; break;
                        case "q1": datefilterred = "First Quarter"; break;
                        case "q2": datefilterred = "Second Quarter"; break;
                        case "q3": datefilterred = "Third Quarter"; break;
                        case "q4": datefilterred = "Forth Quarter"; break;
                        case "monthly": datefilterred = "This Month"; break;
                        case "weekly": datefilterred = "This Week"; break;
                        default: throw new Exception("Invalid frequency type");
                    }

                    // 3️⃣ Get start & end dates
                    var dateQuery = $@"
                        SELECT startdate, enddate
                        FROM v_date_range
                        WHERE rangename = '{datefilterred}'";
                    var dateFilter = _dbContext.SqlQuery<DateRange>(dateQuery).FirstOrDefault();
                    if (dateFilter == null) throw new Exception("Invalid date filter.");

                    DateTime startDate = dateFilter.StartDate.Value;
                    DateTime endDate = dateFilter.EndDate.Value;

                    var company_code = _workcontext.CurrentUserinformation.company_code;
                    var branch_code = _workcontext.CurrentUserinformation.branch_code;
                    var login_code = _workcontext.CurrentUserinformation.login_code;

                    string startDateStr = startDate.ToString("yyyy-MM-dd");
                    string endDateStr = endDate.ToString("yyyy-MM-dd");

                    string monthName;
                    bool isAnnual = startDate.Month == 1 && startDate.Day == 1 &&
                                    endDate.Month == 12 && endDate.Day == 31 &&
                                    startDate.Year == endDate.Year;
                    monthName = isAnnual ? "Annual" : GetNepaliMonthFromEnglish(startDate);

                    var gridData = ((IEnumerable<object>)data.gridData ?? Enumerable.Empty<object>()).ToList();

                    int totalQuantity = 0;
                    int totalAmount = 0;

                    foreach (var row in gridData)
                    {
                        if (row != null)
                        {
                            dynamic r = row;

                            int q = 0, a = 0;

                            try { q = r.Quantity; } catch { q = 0; }
                            try { a = r.Amount; } catch { a = 0; }

                            totalQuantity += q;
                            totalAmount += a;
                        }
                    }

                    // Optional: add top-level targetQuantity/targetAmount if provided
                    if (data.targetQuantity != null) totalQuantity += Convert.ToInt32(data.targetQuantity);
                    if (data.targetAmount != null) totalAmount += Convert.ToInt32(data.targetAmount);

                    if (totalQuantity <= 0 && totalAmount <= 0)
                        throw new Exception("Target quantity and amount cannot be zero.");

                    List<string> empGroups = new List<string>();
                    if (data.employeeGroup != null)
                    {
                        foreach (var item in data.employeeGroup)
                        {
                            if (item != null) empGroups.Add(item.ToString());
                        }
                    }

                    List<string> employeeIds = new List<string>();
                    if (data.employees != null)
                    {
                        foreach (var item in data.employees)
                        {
                            if (item != null) employeeIds.Add(item.ToString());
                        }
                    }


                    List<dynamic> gridDataJson = new List<dynamic>();
                    if (data.gridData != null)
                    {
                        if (data.gridData is IEnumerable<object> enumerable)
                        {
                            gridDataJson = enumerable.ToList();
                        }
                        else
                        {
                            gridDataJson.Add(data.gridData);
                        }
                    }
                    string subTargetType = data.subTargetType?.ToString() ?? "";

                    // 6️⃣ Insert into IP_TARGET_SETUP
                    string connStr = ConfigurationManager.ConnectionStrings["NeoErpCoreEntity"].ToString();
                    string[] tokens = connStr.Split('"'); // adjust if necessary
                    using (var conn = new OracleConnection(tokens[1]))
                    {
                        conn.Open();

                        using (var cmd = new OracleCommand())
                        {
                            cmd.Connection = conn;

                            foreach (var empGroup in empGroups)
                            {
                                foreach (var empId in employeeIds.DefaultIfEmpty(""))
                                {
                                    cmd.CommandText = $@"
                                        INSERT INTO IP_TARGET_SETUP
                                        (
                                            TARGET_ID,
                                            TARGET_NAME,
                                            TARGET_TYPE,
                                            TARGET_QUANTITY,
                                            TARGET_AMOUNT,
                                            FROM_DATE,
                                            END_DATE,
                                            ASSIGN_EMPLOYEE,
                                            EMPLOYEE_GROUP,
                                            SUB_TARGET_TYPE,
                                            DATE_FILTER,
                                            MONTH,
                                            COMPANY_CODE,
                                            BRANCH_CODE,
                                            CREATED_DATE,
                                            CREATED_BY,
                                            TARGET_SETUP_TYPE
                                        )
                                        VALUES
                                        (
                                            '{targetId}',
                                            '{targetName}',
                                            'SAL',
                                            {totalQuantity},
                                            {totalAmount},
                                            TO_DATE('{startDateStr}', 'YYYY-MM-DD'),
                                            TO_DATE('{endDateStr}', 'YYYY-MM-DD'),
                                            '{empId}',
                                            '{empGroup}',
                                            {(string.IsNullOrEmpty(subTargetType) ? "NULL" : $"'{subTargetType}'")},
                                            '{datefilterred}',
                                            '{monthName}',
                                            '{company_code}',
                                            '{branch_code}',
                                            SYSDATE,
                                            '{login_code}', 'DNM'
                                        )";
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                
                    transaction.Commit();
                    return new { success = true, message = "DNM Sales Target saved successfully." };
                }
        
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new { success = false, message = ex.Message };
                }
            }
        }



        public dynamic UpdateDNMTargets(dynamic data)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    // Get reference TARGET_ID to update
                    string targetId = data["targetId"];
                    if (string.IsNullOrWhiteSpace(targetId))
                        throw new Exception("TARGET_ID is required for update.");

                    string targetName = data["targetName"];
                    string targetType = "SAL";
                    string dateFilterFirst = data.DateFilter;
                    var frequency = Frequency[dateFilterFirst.ToLower()];
                    var datefilterred = string.Empty;

                    switch (frequency.ToLower())
                    {
                        case "thisyear": datefilterred = "This Year"; break;
                        case "q1": datefilterred = "First Quarter"; break;
                        case "q2": datefilterred = "Second Quarter"; break;
                        case "q3": datefilterred = "Third Quarter"; break;
                        case "q4": datefilterred = "Forth Quarter"; break;
                        case "monthly": datefilterred = "This Month"; break;
                        case "weekly": datefilterred = "This Week"; break;
                        default: throw new Exception("Invalid frequency type");
                    }

                    // Date range
                    var dateQuery = $@"
                SELECT startdate, enddate
                FROM v_date_range
                WHERE rangename = '{datefilterred}'";

                    var dateFilter = _dbContext.SqlQuery<DateRange>(dateQuery).FirstOrDefault();
                    if (dateFilter == null) throw new Exception("Invalid date filter.");

                    DateTime startDate = dateFilter.StartDate.Value;
                    DateTime endDate = dateFilter.EndDate.Value;

                    var company_code = _workcontext.CurrentUserinformation.company_code;
                    var branch_code = _workcontext.CurrentUserinformation.branch_code;
                    var login_code = _workcontext.CurrentUserinformation.login_code;

                    string startDateStr = startDate.ToString("yyyy-MM-dd");
                    string endDateStr = endDate.ToString("yyyy-MM-dd");

                    string monthName;
                    bool isAnnual =
                        startDate.Month == 1 &&
                        startDate.Day == 1 &&
                        endDate.Month == 12 &&
                        endDate.Day == 31 &&
                        startDate.Year == endDate.Year;
                    monthName = isAnnual ? "Annual" : GetNepaliMonthFromEnglish(startDate);

                    // Extract arrays
                    var empGroups = ((IEnumerable<object>)data.employeeGroup ?? Enumerable.Empty<object>()).Select(x => x.ToString()).ToList();
                    var customerGroups = ((IEnumerable<object>)data.customerGroup ?? Enumerable.Empty<object>()).Select(x => x.ToString()).ToList();
                    var employeeIds = ((IEnumerable<object>)data.empId ?? Enumerable.Empty<object>()).Select(x => x.ToString()).ToList();
                    var itemGroups = ((IEnumerable<object>)data.item_Group ?? Enumerable.Empty<object>()).Select(x => x.ToString()).ToList();

                    decimal targetQuantity = Convert.ToDecimal(data["targetQuantity"] ?? 0);
                    decimal targetAmount = Convert.ToDecimal(data["targetAmount"] ?? 0);

                    if (targetQuantity == 0 && targetAmount == 0)
                        throw new Exception("Target quantity and amount cannot be zero.");

                    // Build update statements for each combination
                    List<string> updateStatements = new List<string>();
                    foreach (var empId in employeeIds.DefaultIfEmpty(""))
                    {
                        foreach (var empGroup in empGroups.DefaultIfEmpty(""))
                        {
                            foreach (var cusGroup in customerGroups.DefaultIfEmpty(""))
                            {
                                foreach (var item in itemGroups.DefaultIfEmpty(""))
                                {
                                    updateStatements.Add($@"
                                UPDATE IP_TARGET_SETUP
                                SET 
                                    TARGET_NAME = '{targetName}',
                                    TARGET_TYPE = '{targetType}',
                                    TARGET_QUANTITY = {targetQuantity},
                                    TARGET_AMOUNT = {targetAmount},
                                    FROM_DATE = TO_DATE('{startDateStr}', 'YYYY-MM-DD'),
                                    END_DATE = TO_DATE('{endDateStr}', 'YYYY-MM-DD'),
                                    ASSIGN_EMPLOYEE = '{empId}',
                                    EMPLOYEE_GROUP = '{empGroup}',
                                    ITEM_GROUP = '{item}',
                                    CUSTOMER_GROUP = '{cusGroup}',
                                    DATE_FILTER = '{datefilterred}',
                                    MONTH = '{monthName}',
                                    MASTER_CODE = '{data.masterCode ?? ""}',
                                    COMPANY_CODE = '{company_code}',
                                    BRANCH_CODE = '{branch_code}',
                                    UPDATED_DATE = SYSDATE,
                                    UPDATED_BY = '{login_code}',
                                    TARGET_SETUP_TYPE = '{data.setupType}'
                                WHERE TARGET_ID = '{targetId}'
                            ");
                                }
                            }
                        }
                    }

                    if (!updateStatements.Any())
                        throw new Exception("No data to update.");

                    // Execute updates
                    foreach (var sql in updateStatements)
                    {
                        _dbContext.ExecuteSqlCommand(sql);
                    }

                    transaction.Commit();
                    return new { success = true, message = "DNM Target updated successfully." };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new { success = false, message = ex.Message };
                }
            }
        }



        public string GetNepaliMonthFromEnglish(DateTime adDate)
        {
            string adDateStr = adDate.ToString("yyyy-MM-dd");

            // Query to convert AD to BS
            string query = $@"SELECT BS_DATE(TO_DATE('{adDateStr}', 'YYYY-MM-DD')) FROM dual";

            // Example output: "2082-08-02"
            var bsDate = _dbContext.SqlQuery<string>(query).FirstOrDefault();

            if (string.IsNullOrEmpty(bsDate))
                return null;

            // Split and extract BS month
            string[] parts = bsDate.Split('-');
            int bsMonth = Convert.ToInt32(parts[1]);

            string[] nepaliMonths = {
        "Baisakh","Jestha","Ashadh","Shrawan","Bhadra","Ashwin",
        "Kartik","Mangsir","Poush","Magh","Falgun","Chaitra"
    };

            // Return NP month name
            return nepaliMonths[bsMonth - 1];
        }


        private decimal CalculateByFrequency(decimal inputValue, string frequency, DateTime fromDate, DateTime toDate)
        {
            decimal result = 0;

            switch (frequency.ToLower())
            {
                case "annual":
                case "annually":
                    int totalDaysInYear = DateTime.IsLeapYear(fromDate.Year) ? 366 : 365;
                    int actualDays = (toDate - fromDate).Days + 1;
                    result = inputValue * actualDays / totalDaysInYear;
                    break;

                case "quarter":
                case "quarterly":
                    int quarterDays = (toDate - fromDate).Days + 1;
                    result = inputValue * quarterDays / 90; // approximate quarter
                    break;

                case "monthly":
                    int daysInMonth = DateTime.DaysInMonth(fromDate.Year, fromDate.Month);
                    int periodDays = (toDate - fromDate).Days + 1;
                    result = inputValue * periodDays / daysInMonth;
                    break;

                case "weekly":
                    int daysInWeek = 7;
                    int weekDays = (toDate - fromDate).Days + 1;
                    result = inputValue * weekDays / daysInWeek;
                    break;
                case "daily":
                case "1day":
                    result = inputValue;
                    break;

                default:
                    throw new Exception("Invalid frequency type");
            }

            return Math.Round(result, 2);
        }

        public Dictionary<string, string> Frequency = new Dictionary<string, string>
        {
            {"monthly", "monthly"},
            {"thisyear", "yearly"},
            {"q1", "quarterly"},
            {"q2", "quarterly"},
            {"q3", "quarterly"},
            {"q4", "quarterly"},
            {"weekly", "weekly"}
        };







        public string saveTargteNew(dynamic data)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    // Example: Accessing values from the dictionary
                    var targetId = Convert.ToInt32(data["targetId"]);
                    var targetType = data["targetType"].ToString();
                    var dateFilter = data["DateFilter"].ToString();
                    var gridData = data["gridData"] as List<Dictionary<string, object>>;
                    // Process the data as needed...
                    return "Success";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return ex.ToString();
                }
            }
        }
        public string updateTargetData(ProfileModel model)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var Query = $@"DELETE FROM IP_TARGET_SETUP WHERE TARGET_ID='{model.TargetId}' AND COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}'";
                    _dbContext.ExecuteSqlCommand(Query);
                    string message = "";
                    if (model.Employees.Count > 0)
                    {
                        if (model.TargetType == "SAL")
                        {
                            saveData(model);
                        }
                        else
                        {
                            saveColData(model);
                        }
                    }
                    else
                    {
                        if (model.FLAG == "SNG")
                        {
                            model.Employees = getSynergyEmployees(model.EmployeeMasterGroup);
                        }
                        else
                        {
                            model.Employees = getEmployees(model.EmployeeMasterGroup);
                        }
                        if (model.TargetType == "SAL")
                        {
                            saveData(model);
                        }
                        else
                        {
                            saveColData(model);
                        }
                    }
                    transaction.Commit();
                    message = "success";
                    return message;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return ex.ToString();
                }
            }
        }
        public bool saveData(ProfileModel model)
        {
            try
            {
                if (model.TargetId == 0)
                {
                    string query = $@"SELECT COALESCE(MAX(TARGET_ID), 0) + 1 AS TARGET_ID FROM IP_TARGET_SETUP where deleted_flag='N'";
                    model.TargetId = this._dbContext.SqlQuery<int>(query).FirstOrDefault();
                }
                foreach (var employee in model.Employees)
                {
                    int currentMonth = 1;
                    foreach (var data in model.GridData)
                    {
                        string insertQuery = $@"
                                        INSERT INTO ip_target_setup
                                        (
                                            TARGET_ID,DAYS, MASTER_CODE, TARGET_TYPE, SUB_TARGET_TYPE, MU_CODE, 
                                            TARGET_QUANTITY, TARGET_AMOUNT, TARGET_NAME, FROM_DATE, 
                                            END_DATE, COMPANY_CODE, BRANCH_CODE, ASSIGN_EMPLOYEE, 
                                            CREATED_DATE, CREATED_BY, DELETED_FLAG,EMPLOYEE_GROUP,ITEM_GROUP,DATE_FILTER,CUSTOMER_GROUP,FLAG
                                        )
                                        VALUES
                                        ({model.TargetId},'{currentMonth}','{data.ItemCode}','{model.TargetType}','{model.SubTargetType}','{data.muCode}', 
                                            {data.Quantity}, {data.Amount},'{model.TargetName}','{data.Date}', 
                                            '{data.Date}','{_workcontext.CurrentUserinformation.company_code}','{_workcontext.CurrentUserinformation.branch_code}','{employee}',
                                            trunc(sysdate),'{_workcontext.CurrentUserinformation.User_id}','N','{model.EmployeeGroup}','{model.ItemGroup}','{model.DateFilter}','{model.CustomerGroup}','{model.FLAG}')";
                        _dbContext.ExecuteSqlCommand(insertQuery);
                        currentMonth++;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool saveColData(ProfileModel model)
        {
            try
            {
                if (model.TargetId == 0)
                {
                    string query = $@"SELECT COALESCE(MAX(TARGET_ID), 0) + 1 AS TARGET_ID FROM IP_TARGET_SETUP where deleted_flag='N'";
                    model.TargetId = this._dbContext.SqlQuery<int>(query).FirstOrDefault();
                }
                int currentMonth = 1;
                foreach (var data in model.GridData)
                {
                    string insertQuery = $@"
                                        INSERT INTO ip_target_setup
                                        (
                                            TARGET_ID,MONTH, MASTER_CODE, TARGET_TYPE, SUB_TARGET_TYPE, MU_CODE, 
                                            TARGET_QUANTITY, TARGET_AMOUNT, TARGET_NAME, FROM_DATE, 
                                            END_DATE, COMPANY_CODE, BRANCH_CODE, ASSIGN_EMPLOYEE, 
                                            CREATED_DATE, CREATED_BY, DELETED_FLAG,EMPLOYEE_GROUP,ITEM_GROUP,DATE_FILTER,CUSTOMER_GROUP,FLAG
                                        )
                                        VALUES
                                        ({model.TargetId},'{currentMonth}','{data.ItemCode}','{model.TargetType}','{model.SubTargetType}','{data.muCode}', 
                                            {data.Quantity}, {data.Amount},'{model.TargetName}','{data.Date}', 
                                            '{data.Date}','{_workcontext.CurrentUserinformation.company_code}','{_workcontext.CurrentUserinformation.branch_code}','{data.ItemCode}', 
                                            trunc(sysdate),'{_workcontext.CurrentUserinformation.User_id}','N','{model.EmployeeGroup}','{model.ItemGroup}','{model.DateFilter}','{model.CustomerGroup}','{model.FLAG}')";
                    _dbContext.ExecuteSqlCommand(insertQuery);
                    currentMonth++;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public dynamic GetTargetById(string targetId)
        {
            string sConnStr1 = ConfigurationManager.ConnectionStrings["NeoErpCoreEntity"].ToString();
            string[] tokens = sConnStr1.Split('"');
            using (var conn = new OracleConnection(tokens[1]))
            {
                conn.Open();

                // 1️⃣ Fetch main target
                var target = new Dictionary<string, object>();
                string targetSql = @"
                SELECT t.TARGET_ID,
                       t.TARGET_NAME,
                       t.TARGET_TYPE,
                       t.TARGET_QUANTITY,
                       t.TARGET_AMOUNT,
                       t.TARGET_SETUP_TYPE,
                       t.SUB_TARGET_TYPE,
                       t.DATE_FILTER,
                       t.FROM_DATE,
                       t.END_DATE,
                       t.MASTER_CODE,
                       (SELECT CUSTOMER_EDESC FROM SA_CUSTOMER_SETUP c WHERE c.CUSTOMER_CODE = t.MASTER_CODE) AS MASTER_NAME,
                       t.EMPLOYEE_GROUP,
                       t.ASSIGN_EMPLOYEE,
                       t.CUSTOMER_GROUP,
                       t.ITEM_GROUP,
                       t.SUB_TARGET_TYPE
                FROM IP_TARGET_SETUP t
                WHERE t.TARGET_ID = :targetId";

                using (var cmd = new OracleCommand(targetSql, conn))
                {
                    cmd.Parameters.Add(new OracleParameter("targetId", targetId));
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read()) return null;

                        target["TARGET_ID"] = reader["TARGET_ID"];
                        target["TARGET_NAME"] = reader["TARGET_NAME"];
                        target["TARGET_TYPE"] = reader["TARGET_TYPE"];
                        target["TARGET_QUANTITY"] = reader["TARGET_QUANTITY"];
                        target["TARGET_AMOUNT"] = reader["TARGET_AMOUNT"];
                        target["TARGET_SETUP_TYPE"] = reader["TARGET_SETUP_TYPE"];
                        target["SUB_TARGET_TYPE"] = reader["SUB_TARGET_TYPE"];
                        target["DATE_FILTER"] = reader["DATE_FILTER"];
                        target["FROM_DATE"] = reader["FROM_DATE"];
                        target["END_DATE"] = reader["END_DATE"];
                        target["MASTER_CODE"] = reader["MASTER_CODE"];
                        target["MASTER_NAME"] = reader["MASTER_NAME"];
                        target["EMP_GROUP"] = reader["EMPLOYEE_GROUP"]?.ToString().Split(',') ?? new string[] { };
                        target["ASSIGN_EMPLOYEE"] = reader["ASSIGN_EMPLOYEE"]?.ToString().Split(',') ?? new string[] { };
                        target["CUSTOMER_GROUP"] = reader["CUSTOMER_GROUP"]?.ToString().Split(',') ?? new string[] { };
                        target["ITEM_GROUP"] = reader["ITEM_GROUP"]?.ToString().Split(',') ?? new string[] { };
                        target["SUB_TARGET_TYPE"] = reader["SUB_TARGET_TYPE"];
                    }
                }

                // 2️⃣ Fetch ITEMS separately
                string itemsSql = @"SELECT DISTINCT ITEM_GROUP AS ITEM_CODE, MU_CODE FROM IP_TARGET_SETUP WHERE TARGET_ID = :targetId";
                var items = new List<Dictionary<string, object>>();
                using (var cmd = new OracleCommand(itemsSql, conn))
                {
                    cmd.Parameters.Add(new OracleParameter("targetId", targetId));
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new Dictionary<string, object>
                            {
                                ["ITEM_CODE"] = reader["ITEM_CODE"],
                                ["MU_CODE"] = reader["MU_CODE"]
                            });
                        }
                    }
                }

                target["ITEMS"] = items;

                return target;
            }
        }


        public string GetTargetSetupType(string targetId)
        {
            string sConnStr1 = ConfigurationManager.ConnectionStrings["NeoErpCoreEntity"].ToString();
            string[] tokens = sConnStr1.Split('"');

            using (var conn = new OracleConnection(tokens[1]))
            using (var cmd = new OracleCommand())
            {
                conn.Open();
                cmd.Connection = conn;

                cmd.CommandText = @"
            SELECT TARGET_SETUP_TYPE 
            FROM IP_TARGET_SETUP 
            WHERE TARGET_ID = :targetId";

                cmd.Parameters.Add(new OracleParameter("targetId", targetId));

                var result = cmd.ExecuteScalar();
                return result?.ToString() ?? "";
            }
        }


        public dynamic GetDNMTargetById(string targetId, string targetSetupType = "DNM")
        {
            string sConnStr1 = ConfigurationManager.ConnectionStrings["NeoErpCoreEntity"].ToString();
            string[] tokens = sConnStr1.Split('"');

            using (var conn = new OracleConnection(tokens[1]))
            {
                conn.Open();

                var target = new Dictionary<string, object>();

                // 🔥 FULL SELECT including ALL columns saved in SaveDNMTarget
                string targetSql = @"
        SELECT 
            t.TARGET_ID,
            t.TARGET_NAME,
            t.TARGET_TYPE,
            t.TARGET_QUANTITY,
            t.TARGET_AMOUNT,
            t.FROM_DATE,
            t.END_DATE,
            t.DATE_FILTER,
            t.MONTH,
            t.MASTER_CODE,
            (SELECT CUSTOMER_EDESC 
               FROM SA_CUSTOMER_SETUP c 
              WHERE c.CUSTOMER_CODE = t.MASTER_CODE) AS MASTER_NAME,

            t.EMPLOYEE_GROUP,
            t.ASSIGN_EMPLOYEE,
            t.CUSTOMER_GROUP,
            t.INDIVIDUAL_CUSTOMERCODE,
            t.ITEM_GROUP,

            t.TARGET_SETUP_TYPE,
            t.COMPANY_CODE,
            t.BRANCH_CODE,
            t.CREATED_BY,
            t.CREATED_DATE
        FROM IP_TARGET_SETUP t
        WHERE t.TARGET_ID = :targetId
          AND t.TARGET_SETUP_TYPE = :targetSetupType";

                using (var cmd = new OracleCommand(targetSql, conn))
                {
                    cmd.Parameters.Add(new OracleParameter("targetId", targetId));
                    cmd.Parameters.Add(new OracleParameter("targetSetupType", targetSetupType));

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read()) return null;

                        // BASIC INFO
                        target["TARGET_ID"] = reader["TARGET_ID"];
                        target["TARGET_NAME"] = reader["TARGET_NAME"];
                        target["TARGET_TYPE"] = reader["TARGET_TYPE"];
                        target["TARGET_QUANTITY"] = reader["TARGET_QUANTITY"];
                        target["TARGET_AMOUNT"] = reader["TARGET_AMOUNT"];
                        target["DATE_FILTER"] = reader["DATE_FILTER"];
                        target["MONTH"] = reader["MONTH"];
                        target["FROM_DATE"] = reader["FROM_DATE"];
                        target["END_DATE"] = reader["END_DATE"];
                        target["MASTER_CODE"] = reader["MASTER_CODE"];
                        target["MASTER_NAME"] = reader["MASTER_NAME"];

                        // EMPLOYEE DATA
                        target["EMP_GROUP"] = reader["EMPLOYEE_GROUP"]?.ToString().Split(',') ?? new string[] { };
                        target["ASSIGN_EMPLOYEE"] = reader["ASSIGN_EMPLOYEE"]?.ToString().Split(',') ?? new string[] { };

                        // CUSTOMER DATA
                        target["CUSTOMER_GROUP"] = reader["CUSTOMER_GROUP"]?.ToString().Split(',') ?? new string[] { };
                        target["INDIVIDUAL_CUSTOMERCODE"] = reader["INDIVIDUAL_CUSTOMERCODE"]?.ToString().Split(',') ?? new string[] { };

                        // ITEM DATA
                        target["ITEM_GROUP"] = reader["ITEM_GROUP"]?.ToString().Split(',') ?? new string[] { };

                        // META
                        target["TARGET_SETUP_TYPE"] = reader["TARGET_SETUP_TYPE"];
                        target["COMPANY_CODE"] = reader["COMPANY_CODE"];
                        target["BRANCH_CODE"] = reader["BRANCH_CODE"];
                        target["CREATED_BY"] = reader["CREATED_BY"];
                        target["CREATED_DATE"] = reader["CREATED_DATE"];
                    }
                }

                // 🔥 FETCH DISTINCT ITEMS (ITEM_GROUP + MU_CODE)
                string itemsSql = @"
        SELECT DISTINCT 
            ITEM_GROUP AS ITEM_CODE, 
            MU_CODE 
        FROM IP_TARGET_SETUP 
        WHERE TARGET_ID = :targetId";

                var items = new List<Dictionary<string, object>>();

                using (var cmd = new OracleCommand(itemsSql, conn))
                {
                    cmd.Parameters.Add(new OracleParameter("targetId", targetId));

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new Dictionary<string, object>
                            {
                                ["ITEM_CODE"] = reader["ITEM_CODE"],
                                ["MU_CODE"] = reader["MU_CODE"]
                            });
                        }
                    }
                }

                target["ITEMS"] = items;

                return target;
            }
        }




        public List<string> getEmployees(string empGroup)
        {
            try
            {
                var condition = string.Empty;

                // Check if empGroup is not null or empty and format the condition accordingly
                if (!string.IsNullOrEmpty(empGroup))
                {
                    condition = $@" AND LU.GROUPID in ({empGroup})";
                }

                string query = $@"SELECT DISTINCT ES.EMPLOYEE_CODE
                          FROM HR_EMPLOYEE_SETUP ES
                          JOIN DIST_LOGIN_USER LU ON LU.SP_CODE=ES.EMPLOYEE_CODE AND LU.COMPANY_CODE=ES.COMPANY_CODE
                          WHERE ES.DELETED_FLAG='N' 
                          AND LU.BRANDING='N'
                          AND ES.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}' 
                          {condition}
                          ORDER BY LOWER(TRIM(' ('||ES.EMPLOYEE_CODE||')'))";

                var result = this._dbContext.SqlQuery<string>(query).ToList();
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<string> getSynergyEmployees(string empGroup)
        {
            try
            {
                var condition = string.Empty;
                if (empGroup != "" && empGroup != null)
                    condition = $@" AND PRE_EMPLOYEE_CODE in ({empGroup})";

                string query = $@"select distinct EMPLOYEE_CODE from HR_EMPLOYEE_SETUP where group_sku_flag='I' and deleted_flag='N' and COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}' {condition}";
                var result = this._dbContext.SqlQuery<string>(query).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<TARGET_PLAN> getAllTargets()
        {
            var company_code = this._workcontext.CurrentUserinformation.company_code;
            //AND COMPANY_CODE = '{company_code}'
            var sqlquery = $@"SELECT DISTINCT TARGET_ID,TARGET_NAME, MIN(FROM_DATE) AS FROM_DATE,MAX(END_DATE) AS END_DATE
                            FROM IP_TARGET_SETUP WHERE DELETED_FLAG = 'N'  GROUP BY TARGET_ID, TARGET_NAME ORDER BY TARGET_ID DESC";
            var target = _dbContext.SqlQuery<TARGET_PLAN>(sqlquery).ToList();
            return target;
        }
        public string UpdateTarget(string targetId)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                var message = "";
                try
                {
                    var updateQuery = $@"UPDATE IP_TARGET_SETUP SET DELETED_FLAG='Y' WHERE TARGET_ID='{targetId}' AND COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}'";
                    _dbContext.ExecuteSqlCommand(updateQuery);
                    transaction.Commit();
                    message = "success";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    message = "failed";
                }
                return message;
            }
        }

        //public TARGET_DETAILS GetTargetData(string targetId)
        //{
        //    var targetDataQuery = $@"SELECT its.target_id, its.target_name, its.month,its.master_code as code,its.flag,
        //       CASE WHEN its.sub_target_type = 'CUS' AND scs.customer_code IS NOT NULL THEN scs.customer_edesc
        //    WHEN its.sub_target_type = 'ITM' AND itm.item_code IS NOT NULL THEN itm.item_edesc
        //    WHEN its.target_type = 'COL' AND hr.employee_code IS NOT NULL THEN hr.employee_edesc
        //       END AS master_code, its.target_type, its.sub_target_type, its.mu_code, its.target_quantity,its.item_group,its.employee_group,its.customer_group,
        //       its.target_amount, its.from_date, its.end_date, its.company_code, its.branch_code, its.assign_employee, its.created_date, its.created_by, its.deleted_flag
        //    FROM ip_target_setup its
        //    LEFT JOIN hr_employee_setup hr ON hr.employee_code = its.master_code AND hr.company_code = its.company_code
        //    LEFT JOIN ip_item_master_setup itm ON itm.item_code = its.master_code AND itm.company_code = its.company_code
        //    LEFT JOIN sa_customer_setup scs ON scs.customer_code = its.master_code AND scs.company_code = its.company_code
        //    WHERE its.company_code ='{_workcontext.CurrentUserinformation.company_code}' AND its.target_id = {targetId}  AND its.deleted_flag = 'N' order by its.from_date,
        //    CASE 
        //    WHEN its.sub_target_type = 'CUS' AND scs.customer_code IS NOT NULL THEN scs.customer_edesc
        //    WHEN its.sub_target_type = 'ITM' AND itm.item_code IS NOT NULL THEN itm.item_edesc
        //    WHEN its.target_type = 'COL' AND hr.employee_code IS NOT NULL THEN hr.employee_edesc
        //    END";
        //    var targetData = _dbContext.SqlQuery<TargetData>(targetDataQuery).ToList();
        //    // Fetch DateFilter
        //    var dateFilterQuery = $@"SELECT TARGET_ID, MIN(FROM_DATE) AS START_DATE, MAX(END_DATE) AS LAST_DATE,DATE_FILTER
        //                     FROM IP_TARGET_SETUP 
        //                     WHERE DELETED_FLAG = 'N' 
        //                     AND TARGET_ID={targetId} 
        //                     AND COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}' 
        //                     GROUP BY TARGET_ID,DATE_FILTER 
        //                     ORDER BY TARGET_ID DESC";
        //    var dateFilter = _dbContext.SqlQuery<DateFilter>(dateFilterQuery).ToList();

        //    // Create response object
        //    var response = new TARGET_DETAILS
        //    {
        //        TargetData = targetData,
        //        DateFilter = dateFilter
        //    };

        //    return response;
        //}

        public class fetchTarget
        {
            public string MASTER_TYPE { get; set; }
            public string TARGET_ID { get; set; }
            public string TARGET_NAME { get; set; }
            public string MASTER_CODE { get; set; }
            public string TARGET_TYPE { get; set; }
            public string TARGET_QUANTITY { get; set; }
            public string TARGET_AMOUNT { get; set; }
            public string FROM_DATE { get; set; }
            public string ASSIGN_EMPLOYEE { get; set; }
            public string STARTDATE { get; set; }
            public string ENDDATE { get; set; }

            public string FILTERSTARTDATE { get; set; }
            public string FILTERENDDATE { get; set; }

        }

        public class FILTERS
        {
            public string GROUP_ID { get; set; }
            public string GROUP_EDESC { get; set; }
            public string MASTER_CUSTOMER_CODE { get; set; }
            public string PRE_CUSTOMER_CODE { get; set; }
            public string TARGET_ID { get; set; }
            public string TARGET_NAME { get; set; }
            public string TARGET_TYPE { get; set; }
            public string MASTER_TYPE { get; set; }

            public string ASSIGN_EMPLOYEE { get; set; }

            public string EMPLOYEE_EDESC { get; set; }

        }


        public dynamic GetTargetDataNew(string targetId)
        {
            // Use parameterized query to prevent SQL injection
            string targetDataQuery = $@"
            SELECT 
                to_char(MASTER_TYPE) MASTER_TYPE,
                to_char(TARGET_ID) TARGET_ID,
                to_char(TARGET_NAME) TARGET_NAME,        
                to_char(MASTER_CODE) MASTER_CODE,
                to_char(TARGET_TYPE) TARGET_TYPE,
                to_char(TARGET_QUANTITY) TARGET_QUANTITY,
                to_char(TARGET_AMOUNT) TARGET_AMOUNT,
                to_char(FROM_DATE, 'YYYYMMDD') FROM_DATE,
                to_char(ASSIGN_EMPLOYEE) ASSIGN_EMPLOYEE
            FROM ip_target_setup WHERE target_id = '{targetId}'";







            targetDataQuery = $@"
            SELECT 
                to_char(a.MASTER_TYPE) MASTER_TYPE,
                to_char(a.TARGET_ID) TARGET_ID,
                to_char(a.TARGET_NAME) TARGET_NAME,        
                to_char(a.MASTER_CODE) MASTER_CODE,
                to_char(a.TARGET_TYPE) TARGET_TYPE,
                to_char(a.TARGET_QUANTITY) TARGET_QUANTITY,
                to_char(a.TARGET_AMOUNT) TARGET_AMOUNT,
                to_char(a.FROM_DATE, 'YYYYMMDD') FROM_DATE,
                to_char(a.ASSIGN_EMPLOYEE) ASSIGN_EMPLOYEE ,
              TO_CHAR(b.startdate, 'YYYY-Mon-DD') startdate ,
              TO_CHAR(b.enddate, 'YYYY-Mon-DD') enddate,
                TO_CHAR(a.filter_start_date, 'YYYY-Mon-DD') FILTERSTARTDATE ,
                TO_CHAR(a.filter_end_date, 'YYYY-Mon-DD') FILTERENDDATE 
            FROM ip_target_setup a,
            (SELECT target_id,Min(FROM_DATE) startdate,
            Max(FROM_DATE) enddate FROM IP_TARGET_SETUP WHERE TARGET_ID='{targetId}' group by target_id) b
             WHERE a.target_id=b.target_id and a.target_id = '{targetId}'
                ";




            //targetDataQuery = $@"
            //SELECT 
            //    to_char(a.MASTER_TYPE) MASTER_TYPE,
            //    to_char(a.TARGET_ID) TARGET_ID,
            //    to_char(a.TARGET_NAME) TARGET_NAME,        
            //    to_char(a.MASTER_CODE) MASTER_CODE,
            //    to_char(a.TARGET_TYPE) TARGET_TYPE,
            //    to_char(a.TARGET_QUANTITY) TARGET_QUANTITY,
            //    to_char(a.TARGET_AMOUNT) TARGET_AMOUNT,
            //    to_char(a.FROM_DATE, 'YYYYMMDD') FROM_DATE,
            //    to_char(a.ASSIGN_EMPLOYEE) ASSIGN_EMPLOYEE ,
            //    TO_CHAR(a.filter_start_date, 'YYYY-Mon-DD') startdate,
            //    TO_CHAR(a.filter_end_date, 'YYYY-Mon-DD') enddate
            //FROM ip_target_setup a
            // WHERE a.target_id = '{targetId}'
            //    ";


            var targetData = _dbContext.SqlQuery<fetchTarget>(targetDataQuery).ToList();


            System.Diagnostics.Debug.WriteLine(targetData[0].STARTDATE);






            // Grouping data by MASTER_CODE and formatting as required
            var groupedData = targetData
                .GroupBy(x => x.MASTER_CODE)
                .ToDictionary(
                    g => g.Key,
                    g => g.ToDictionary(
                        item => $"qty{item.FROM_DATE}",
                        item => int.Parse(item.TARGET_QUANTITY)
                    )
                );


            // Add amt values to the inner dictionary
            foreach (var g in targetData.GroupBy(x => x.MASTER_CODE))
            {
                var innerDict = groupedData[g.Key];
                foreach (var item in g)
                {
                    string dateKeyAmt = $"amt{item.FROM_DATE}";
                    innerDict[dateKeyAmt] = int.Parse(item.TARGET_AMOUNT);
                }
            }



            targetDataQuery = $@"
                                     SELECT CASE
                                  WHEN a.IND IS NOT NULL AND a.master_type = 'CUS'
                                  THEN
                                     (SELECT customer_code
                                        FROM sa_customer_setup
                                       WHERE master_customer_code =
                                                (SELECT pre_customer_code
                                                   FROM sa_customer_setup
                                                  WHERE company_code = '01' AND customer_code = a.IND)
                                             AND company_code = '01')
                                  WHEN a.IND IS NOT NULL AND a.master_type = 'ITM'
                                  THEN
                                     (SELECT item_code
                                        FROM ip_item_master_setup
                                       WHERE item_code =
                                                (SELECT pre_item_code
                                                   FROM ip_item_master_setup
                                                  WHERE company_code = '01' AND item_code = a.IND)
                                             AND company_code = '01')
                                  WHEN a.master_type = 'CUS'
                                  THEN
                                     (SELECT customer_code
                                        FROM sa_customer_setup
                                       WHERE company_code = '01' AND customer_code = a.grp)
                                  WHEN a.master_type = 'ITM'
                                  THEN
                                     (SELECT ITEM_code
                                        FROM IP_ITEM_MASTER_SETUP
                                       WHERE company_code = '01' AND ITEM_CODE = a.grp)
                                  ELSE
                                     ''
                               END
                                  AS GROUP_ID,
                               CASE
                                  WHEN a.IND IS NOT NULL AND a.master_type = 'CUS'
                                  THEN
                                     (SELECT customer_edesc
                                        FROM sa_customer_setup
                                       WHERE master_customer_code =
                                                (SELECT pre_customer_code
                                                   FROM sa_customer_setup
                                                  WHERE company_code = '01' AND customer_code = a.IND)
                                             AND company_code = '01')
                                  WHEN a.IND IS NOT NULL AND a.master_type = 'ITM'
                                  THEN
                                     (SELECT item_edesc
                                        FROM ip_item_master_setup
                                       WHERE item_code =
                                                (SELECT pre_item_code
                                                   FROM ip_item_master_setup
                                                  WHERE company_code = '01' AND item_code = a.IND)
                                             AND company_code = '01')
                                  WHEN a.master_type = 'CUS'
                                  THEN
                                     (SELECT customer_edesc
                                        FROM sa_customer_setup
                                       WHERE company_code = '01' AND customer_code = a.grp)
                                  WHEN a.master_type = 'ITM'
                                  THEN
                                     (SELECT ITEM_edesc
                                        FROM IP_ITEM_MASTER_SETUP
                                       WHERE company_code = '01' AND ITEM_CODE = a.grp)
                                  ELSE
                                     ''
                               END
                                  AS GROUP_EDESC,
                               CASE
                                  WHEN a.IND IS NOT NULL AND a.master_type = 'CUS'
                                  THEN
                                     (SELECT master_customer_code
                                        FROM sa_customer_setup
                                       WHERE master_customer_code =
                                                (SELECT pre_customer_code
                                                   FROM sa_customer_setup
                                                  WHERE company_code = '01' AND customer_code = a.IND)
                                             AND company_code = '01')
                                  WHEN a.IND IS NOT NULL AND a.master_type = 'ITM'
                                  THEN
                                     (SELECT master_item_code
                                        FROM ip_item_master_setup
                                       WHERE item_code =
                                                (SELECT pre_item_code
                                                   FROM ip_item_master_setup
                                                  WHERE company_code = '01' AND item_code = a.IND)
                                             AND company_code = '01')
                                  WHEN a.master_type = 'CUS'
                                  THEN
                                     (SELECT MASTER_CUSTOMER_CODE
                                        FROM sa_customer_setup
                                       WHERE company_code = '01' AND customer_code = a.grp)
                                  WHEN a.master_type = 'ITM'
                                  THEN
                                     (SELECT MASTER_ITEM_CODE
                                        FROM IP_ITEM_MASTER_SETUP
                                       WHERE company_code = '01' AND ITEM_CODE = a.grp)
                                  ELSE
                                     ''
                               END
                                  AS MASTER_CUSTOMER_CODE,
                               CASE
                                  WHEN a.IND IS NOT NULL AND a.master_type = 'CUS'
                                  THEN
                                     (SELECT pre_customer_code
                                        FROM sa_customer_setup
                                       WHERE master_customer_code =
                                                (SELECT pre_customer_code
                                                   FROM sa_customer_setup
                                                  WHERE company_code = '01' AND customer_code = a.IND)
                                             AND company_code = '01')
                                  WHEN a.IND IS NOT NULL AND a.master_type = 'ITM'
                                  THEN
                                     (SELECT pre_item_code
                                        FROM ip_item_master_setup
                                       WHERE item_code =
                                                (SELECT pre_item_code
                                                   FROM ip_item_master_setup
                                                  WHERE company_code = '01' AND item_code = a.IND)
                                             AND company_code = '01'
                                             )
                                  WHEN a.master_type = 'CUS'
                                  THEN
                                     (SELECT PRE_CUSTOMER_CODE
                                        FROM sa_customer_setup
                                       WHERE company_code = '01' AND customer_code = a.grp)
                                  WHEN a.master_type = 'ITM'
                                  THEN
                                     (SELECT PRE_ITEM_CODE
                                        FROM IP_ITEM_MASTER_SETUP
                                       WHERE company_code = '01' AND ITEM_CODE = a.grp)
                                  ELSE '' END AS PRE_CUSTOMER_CODE,
                               a.grp,
                               TO_CHAR (TARGET_ID) TARGET_ID,
                               TO_CHAR (TARGET_NAME) TARGET_NAME,
                               TO_CHAR (TARGET_TYPE) TARGET_TYPE,
                               TO_CHAR (MASTER_TYPE) MASTER_TYPE,
                               a.ASSIGN_EMPLOYEE, b.employee_edesc 
                          FROM IP_TARGET_SETUP a ,(select * from hr_employee_setup where company_code='01') b
                          where 
                          a.ASSIGN_EMPLOYEE=b.employee_code(+) and   target_id = '{targetId}'
                ";




            //var filters = _dbContext.SqlQuery<FILTERS>(targetDataQuery).ToList();

            List<FILTERS> filters = new List<FILTERS>();

            var fil = new Dictionary<string, string>();

            try
            {
                filters = _dbContext.SqlQuery<FILTERS>(targetDataQuery).ToList();
                if (filters.Count > 0)
                {
                    //var fil = new Dictionary<string, string>();
                    fil["GROUP_ID"] = filters[0].GROUP_ID;
                    fil["GROUP_EDESC"] = filters[0].GROUP_EDESC;
                    fil["MASTER_CUSTOMER_CODE"] = filters[0].MASTER_CUSTOMER_CODE;
                    fil["PRE_CUSTOMER_CODE"] = filters[0].PRE_CUSTOMER_CODE;
                    fil["TARGET_ID"] = filters[0].TARGET_ID;
                    fil["TARGET_NAME"] = filters[0].TARGET_NAME;
                    fil["TARGET_TYPE"] = filters[0].TARGET_TYPE;
                    fil["MASTER_TYPE"] = filters[0].MASTER_TYPE;
                    fil["ASSIGN_EMPLOYEE"] = filters[0].ASSIGN_EMPLOYEE;
                    fil["EMPLOYEE_EDESC"] = filters[0].EMPLOYEE_EDESC;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error executing SQL query: " + ex.Message);
                filters = new List<FILTERS>();
            }

            return new { data = groupedData, startdate = targetData[0].FILTERSTARTDATE, enddate = targetData[0].FILTERENDDATE, filter = fil };
        }


        public List<DistributionGroup> GetDistributionGroups(string filter)
        {
            var company_code = _workcontext.CurrentUserinformation.company_code;
            try
            {
                string query = $@"
                        SELECT 
                            Group_EDESC,
                            GroupId
                        FROM 
                            dist_group_master";
                if (!string.IsNullOrWhiteSpace(filter))
                {
                    query += $" where company_code IN ({filter})";
                }


                List<DistributionGroup> result =
                    _dbContext.SqlQuery<DistributionGroup>(query).ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<EmployeeDistributionGroup> GetEmployeeDistributionGroups(string filter)
        {
            try
            {
                var company_code = _workcontext.CurrentUserinformation.company_code;
                string query = $@"
                    SELECT 
                        e.employee_code,
                        e.employee_edesc,
                        e.master_employee_code,
                        e.pre_employee_code
                    FROM 
                        hr_employee_setup e
                    INNER JOIN 
                        dist_login_user l ON e.employee_code = l.userid
                    INNER JOIN 
                        dist_user_areas ua ON l.userid = ua.user_id
                    WHERE 
                        e.deleted_flag = 'N'
                        AND e.group_sku_flag = 'G'";

                if (!string.IsNullOrWhiteSpace(filter))
                {
                    query += $" AND ua.customer_code IN ({filter})";
                }

                List<EmployeeDistributionGroup> result = _dbContext.SqlQuery<EmployeeDistributionGroup>(query).ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<EmployeeDistribution> GetEmployeeDistribution(string filter)
        {
            try
            {
                var company_code = _workcontext.CurrentUserinformation.company_code;
                string query = @"
                        SELECT USERID, 
                               FULL_NAME 
                        FROM DIST_LOGIN_USER";

                if (!string.IsNullOrWhiteSpace(filter))
                {
                    var validIds = filter.Split(',')
                                         .Where(f => !string.IsNullOrWhiteSpace(f))
                                         .ToList();
                    if (validIds.Count > 0)
                    {
                        query += $" WHERE GROUPID IN ({string.Join(",", validIds.Select(id => id.Trim()))})";
                    }
                }

                List<EmployeeDistribution> result = _dbContext.SqlQuery<EmployeeDistribution>(query).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<CustomerDistribution> GetCustomerDistributionPaged(CustomerGroupRequest request)
        {
            var company_code = _workcontext.CurrentUserinformation.company_code;

           

            var query = $@"
                SELECT *
                FROM (
                    SELECT DISTINCT
                        s.customer_code,
                        s.customer_edesc
                    FROM sa_customer_setup s
                    INNER JOIN dist_user_areas ua ON s.customer_code = ua.customer_code
                    INNER JOIN dist_login_user l ON l.sp_code = ua.sp_code
                    WHERE s.deleted_flag = 'N'
                      AND s.Group_sku_flag = 'I'
                      AND ua.company_code = '{company_code}'";

            if (!string.IsNullOrWhiteSpace(request.CustomerGroupIds) && !request.SelectAll)
            {
                var ids = request.CustomerGroupIds.Split(',')
                           .Where(f => !string.IsNullOrWhiteSpace(f))
                           .Select(f => $"'{f.Trim()}'");
                query += $" AND l.GroupId IN ({string.Join(",", ids)})";
            }

            // Filter by employee/user IDs
            if (!string.IsNullOrWhiteSpace(request.EmployeeIds) && !request.SelectAll)
            {
                var empIds = request.EmployeeIds.Split(',')
                            .Where(e => !string.IsNullOrWhiteSpace(e))
                            .Select(e => $"'{e.Trim()}'");
                query += $" AND l.userId IN ({string.Join(",", empIds)})";
            }

            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                query += $" AND LOWER(s.customer_edesc) LIKE LOWER('%{request.SearchText.Replace("'", "''")}%') ";
            }

            query += @"
        )";
            //WHERE rn > " + request.Skip + " AND rn <= " + (request.Skip + request.Take);

            return _dbContext.SqlQuery<CustomerDistribution>(query).ToList();
        }



        //public List<BrandByCustomer> GetBrandByEmployees(string filter, string searchText)
        //{
        //    try
        //    {
        //        var company_code = _workcontext.CurrentUserinformation.company_code;

        //        string query = $@"
        //    SELECT DISTINCT
        //        i.item_edesc AS BRAND_NAME,
        //        TO_NUMBER(i.Item_code) AS ITEM_CODE
        //    FROM 
        //        ip_item_master_setup i
        //    INNER JOIN 
        //        dist_user_item_mapping uim 
        //        ON i.Item_code = uim.Item_code 
        //        AND i.company_code = uim.company_code
        //    INNER JOIN 
        //        dist_user_areas ua 
        //        ON uim.sp_code = ua.sp_code 
        //        AND uim.company_code = ua.company_code
        //    WHERE 
        //        i.group_sku_flag = 'I'
        //        AND i.deleted_flag = 'N'
        //        AND i.company_code = '{company_code}'";

        //        // ✅ filter by customer list
        //        if (!string.IsNullOrWhiteSpace(filter))
        //        {
        //            var codes = filter.Split(',')
        //                              .Select(c => $"'{c.Trim()}'");
        //            query += $" AND ua.customer_code IN ({string.Join(",", codes)})";
        //        }

        //        // ✅ search filter (THIS FIXES YOUR ISSUE)
        //        if (!string.IsNullOrWhiteSpace(searchText))
        //        {
        //            string safeSearch = searchText.Replace("'", "''").ToUpper();
        //            query += $@" 
        //        AND (
        //            UPPER(i.item_edesc) LIKE '%{safeSearch}%' OR
        //            UPPER(i.item_code) LIKE '%{safeSearch}%'
        //        )";
        //        }

        //        return _dbContext.SqlQuery<BrandByCustomer>(query).ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}



        public List<EmlpoyeeAssociatedItem> GetItemByEmployee(string filter)
        {
            try
            {
                var company_code = _workcontext.CurrentUserinformation.company_code;
                string query = $@"
                        SELECT 
                            i.item_code,
                            i.item_edesc
                        FROM 
                            ip_item_master_setup i
                        INNER JOIN 
                            dist_user_item_mapping d
                            ON i.item_code = d.item_code";

                List<EmlpoyeeAssociatedItem> result = _dbContext.SqlQuery<EmlpoyeeAssociatedItem>(query).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public class SelectedEmployeeGroup
        {
            public int GroupId { get; set; }        
            public string Group_EDESC { get; set; } 
        }

        public List<SelectedEmployeeGroup> GetSelectedEmployeeGroups(int targetId, string companyCode)
        {
            try
            {
                string query = $@"
                    SELECT DISTINCT
                        d.GroupId,
                        d.Group_EDESC
                    FROM dist_group_master d
                    JOIN ip_target_setup t ON t.Target_Id = {targetId}
                    WHERE d.company_code = '{companyCode}'
                      AND INSTR(',' || t.Employee_Group || ',', ',' || TO_CHAR(d.GroupId) || ',') > 0";

                List<SelectedEmployeeGroup> result =
                    _dbContext.SqlQuery<SelectedEmployeeGroup>(query).ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public class SelectedEmployee
        {
            public string User_id { get; set; }
            public string User_name { get; set; }
        }

        public List<SelectedEmployee> GetSelectedEmployees(int targetId)
        {
            try
            {
                string query = $@"
                    SELECT DISTINCT
                        to_char(u.Userid) as User_id,
                        u.User_name
                    FROM dist_login_user u
                    JOIN ip_target_setup t ON t.Target_Id = {targetId}
                    WHERE INSTR(',' || t.assign_employee || ',', ',' || u.Userid || ',') > 0";

                List<SelectedEmployee> result = _dbContext.SqlQuery<SelectedEmployee>(query).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<SelectedEmployee> GetDnmTargetData(int targetId)
        {
            try
            {
                string query = $@"
                    SELECT DISTINCT
                        to_char(u.Userid) as User_id,
                        u.User_name
                    FROM dist_login_user u
                    JOIN ip_target_setup t ON t.Target_Id = {targetId}
                    WHERE INSTR(',' || t.assign_employee || ',', ',' || u.Userid || ',') > 0";

                List<SelectedEmployee> result = _dbContext.SqlQuery<SelectedEmployee>(query).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
