using NeoErp.Core;
using NeoErp.Core.Caching;
using NeoErp.Core.Domain;
using NeoErp.Core.Helpers;
using NeoErp.Core.Models;
using NeoErp.Models;
using NeoErp.Models.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NeoErp.Services
{
    public class UserChartPermission : IUserChartPermission
    {

        private NeoErpCoreEntity _objectEntity;
        private IWorkContext _workContext;
        private ICacheManager _cacheManger;
        public UserChartPermission(NeoErpCoreEntity objectEntity, IWorkContext workContext, ICacheManager cacheManager)
        {
            this._objectEntity = objectEntity;
            this._workContext = workContext;
            this._cacheManger = cacheManager;
        }

        //public bool SaveUserChartPermission(UserChartModel userChartPermission)
        //{
        //    try
        //    {
        //        var cacheChartPermissionKey = $"neochartpersmission_{userChartPermission.ModuleName}";
        //        this._cacheManger.Remove(cacheChartPermissionKey);

        //        //var QueryCacheName = $"neocacheQuery_{reportname}";
        //        //this._cacheManger.Remove(QueryCacheName);
        //        foreach (string user in userChartPermission.Users)
        //        {
        //            var userId = user.ToUpper();
        //            string dashboardWidgetIdQuerey = $@"SELECT TO_NUMBER(TO_CHAR(NVL(MAX(TO_NUMBER(ID)+1),1))) AS id FROM DASHBOARD_WIDGETS";
        //            var dashboardWidgetId = this._objectEntity.SqlQuery<int>(dashboardWidgetIdQuerey).FirstOrDefault();

        //            //First check exist or not
        //            userId = userId.ToUpper();
        //            var test = _objectEntity.SqlQuery<string>("SELECT user_id from dashboard_widgets where user_id='" + userId + "' and module_name='" + userChartPermission.ModuleName + "'");
        //            if (test.Count() > 0)
        //            {
        //                var tst = "update dashboard_widgets set order_no = '" + string.Join(",", userChartPermission.Charts) + "',QuickCap = '" + string.Join(",", userChartPermission.QuickWidgets) + "' where user_id = '" + userId + "' and module_name = '" + userChartPermission.ModuleName + "' and MODIFY_BY = '" + userChartPermission.StaticDasboard + "'";
        //                //update
        //                _objectEntity.ExecuteSqlCommand("update dashboard_widgets set order_no = '" + string.Join(",", userChartPermission.Charts) + "',QuickCap='" + string.Join(",", userChartPermission.QuickWidgets) + "',MODIFY_BY='" + userChartPermission.StaticDasboard + "' where user_id= '" + userId + "' and module_name='" + userChartPermission.ModuleName + "'");
        //            }
        //            else
        //            {
        //                //var abc = "insert into dashboard_widgets(id,module_name,user_id,order_no,QuickCap) values(8,'" + userChartPermission.ModuleName + "','" + userId + "','" + string.Join(",", userChartPermission.Charts) + "','" + string.Join(",", userChartPermission.QuickWidgets) + "')";
        //                //insert
        //                _objectEntity.ExecuteSqlCommand("insert into dashboard_widgets(id,module_name,user_id,order_no,QuickCap,MODIFY_BY) values('"+dashboardWidgetId+"','" + userChartPermission.ModuleName + "','" + userId + "','" + string.Join(",", userChartPermission.Charts) + "','" + string.Join(",", userChartPermission.QuickWidgets) + "','" + userChartPermission.StaticDasboard + "')");
        //            }
        //        }




        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        public bool SaveUserChartPermission(UserChartModel userChartPermission)
        {
            var cacheChartPermissionKey = $"neochartpersmission_{userChartPermission.ModuleName}";
            try
            {
                //var cacheChartPermissionKey = $"neochartpersmission_{userChartPermission.ModuleName}";
                this._cacheManger.Remove(cacheChartPermissionKey);

                //var QueryCacheName = $"neocacheQuery_{reportname}";
                //this._cacheManger.Remove(QueryCacheName);
                foreach (string user in userChartPermission.Users)
                {
                    var userId = user.ToUpper();
                    string dashboardWidgetIdQuerey = $@"SELECT TO_NUMBER(TO_CHAR(NVL(MAX(TO_NUMBER(ID)+1),1))) AS id FROM DASHBOARD_WIDGETS";
                    var dashboardWidgetId = this._objectEntity.SqlQuery<int>(dashboardWidgetIdQuerey).FirstOrDefault();

                    //First check exist or not
                    userId = userId.ToUpper();
                    var test = _objectEntity.SqlQuery<string>("SELECT user_id from dashboard_widgets where user_id='" + userId + "' and module_name='" + userChartPermission.ModuleName + "'");

                    var updateCheckQuery = $@"SELECT Id, USER_ID UserId FROM DASHBOARD_WIDGETS WHERE USER_ID='{userId}' AND module_name='{userChartPermission.ModuleName}'";
                    var updateCheck = this._objectEntity.SqlQuery<UserChartUpdateModel>(updateCheckQuery).FirstOrDefault();
                    if (updateCheck!=null)
                    {
                        var tst = "update dashboard_widgets set order_no = '" + string.Join(",", userChartPermission.Charts) + "',QuickCap = '" + string.Join(",", userChartPermission.QuickWidgets) + "' where user_id = '" + userId + "' and module_name = '" + userChartPermission.ModuleName + "' and MODIFY_BY = '" + userChartPermission.StaticDasboard + "'";
                        //update
                        _objectEntity.ExecuteSqlCommand("update dashboard_widgets set order_no = '" + string.Join(",", userChartPermission.Charts) + "',QuickCap='" + string.Join(",", userChartPermission.QuickWidgets) + "',MODIFY_BY='" + userChartPermission.StaticDasboard + "' where user_id= '" + userId + "' and module_name='" + userChartPermission.ModuleName + "'and id='" + updateCheck.Id + "'");
                    }
                    else
                    {
                        //var abc = "insert into dashboard_widgets(id,module_name,user_id,order_no,QuickCap) values(8,'" + userChartPermission.ModuleName + "','" + userId + "','" + string.Join(",", userChartPermission.Charts) + "','" + string.Join(",", userChartPermission.QuickWidgets) + "')";
                        //insert
                        _objectEntity.ExecuteSqlCommand("insert into dashboard_widgets(id,module_name,user_id,order_no,QuickCap,MODIFY_BY) values('" + dashboardWidgetId + "','" + userChartPermission.ModuleName + "','" + userId + "','" + string.Join(",", userChartPermission.Charts) + "','" + string.Join(",", userChartPermission.QuickWidgets) + "','" + userChartPermission.StaticDasboard + "')");
                    }
                }




                return true;
            }
            catch (Exception ex)
            {
                this._cacheManger.Remove(cacheChartPermissionKey);
                return false;
            }
        }
        public UserChartPermissionModel GetUserChartPermission(string name, string type)
        {
            name = name.ToUpper();
            string Query = @"SELECT order_no from dashboard_widgets where user_id in (" + name + ") and module_name='" + type + "'";
            var charts = _objectEntity.SqlQuery<string>(Query).FirstOrDefault();
            UserChartPermissionModel model = new UserChartPermissionModel()
            {
                User = name,
                Charts = charts.Split(',').ToList(),
                ModuleName = type
            };

            return model;
        }



        public List<UserListModel> GetLoginUserList()
        {
            List<UserListModel> result = new List<UserListModel>();
            try
            {
                string Query = @"SELECT DISTINCT UPPER(LOGIN_CODE) LOGIN_CODE, LOGIN_EDESC,TO_CHAR(USER_NO) ID FROM SC_APPLICATION_USERS WHERE DELETED_FLAG = 'N' ORDER BY LOGIN_EDESC";
                result = _objectEntity.SqlQuery<UserListModel>(Query).ToList();
                return result;
            }
            catch
            {
                return result;
            }

        }

        public List<UserListModel> GetWidgetsList()
        {
            List<UserListModel> result = new List<UserListModel>();
            try
            {
                string Query = @"SELECT DISTINCT UPPER(LOGIN_CODE) LOGIN_CODE, LOGIN_EDESC,TO_CHAR(USER_NO) ID FROM SC_APPLICATION_USERS WHERE DELETED_FLAG = 'N' ORDER BY LOGIN_EDESC";
                result = _objectEntity.SqlQuery<UserListModel>(Query).ToList();
                return result;
            }
            catch
            {
                return result;
            }

        }
        public List<EmployeeSchedular> GetEmployeesListForScheduler()
        {
            var Query = string.Format(@"SELECT es.employee_code as EmployeeCode,
                                            es.employee_edesc as EmployeeName,es.personal_email as PersonalEmail                                            
                                        FROM hr_employee_setup es
                                       WHERE es.deleted_flag = 'N' AND es.company_code = '{0}' AND es.personal_email IS NOT NULL", _workContext.CurrentUserinformation.company_code);

            var employeeList = _objectEntity.SqlQuery<EmployeeSchedular>(Query).ToList();
            return employeeList;
        }
        public List<EmployeeSchedular> GetEmployeesList()
        {
            var Query = string.Format(@"SELECT es.employee_code as EmployeeCode,
                                            es.employee_edesc as EmployeeName,es.personal_email as PersonalEmail                                            
                                        FROM hr_employee_setup es
                                       WHERE es.deleted_flag = 'N' AND es.company_code = '{0}' ", _workContext.CurrentUserinformation.company_code);

            var employeeList = _objectEntity.SqlQuery<EmployeeSchedular>(Query).ToList();
            return employeeList;
        }

        public List<Customers> GetAllCustomers()
        {
            var query = string.Format(@"select Customer_code as CustomerCode,customer_edesc as  CustomerName, EMAIL from SA_CUSTOMER_SETUP where deleted_flag='N' and company_code= '{0}'", _workContext.CurrentUserinformation.company_code);
            var customers = _objectEntity.SqlQuery<Customers>(query).ToList();
            return customers;
        }
        public List<Customers> GetAllCustomersWithOutlet()
        {

            var query = $@"select Customer_code as CustomerCode,customer_edesc as  CustomerName, EMAIL from SA_CUSTOMER_SETUP where deleted_flag='N' and company_code='{_workContext.CurrentUserinformation.company_code}'
                                    UNION
                                    select RESELLER_CODE as CustomerCode,RESELLER_NAME as customer_edesc,EMAIL from DIST_RESELLER_MASTER where  deleted_flag='N' and company_code= '{_workContext.CurrentUserinformation.company_code}'";



            var customers = _objectEntity.SqlQuery<Customers>(query).ToList();
            return customers;
        }

        public List<Dropdownsmodel> GetAllItems()
        {
            var query = "select ITEM_CODE as Code,ITEM_EDESC as Name from ip_item_master_setup where DELETED_FLAG='N' AND COMPANY_CODE='01'";
            var customers = _objectEntity.SqlQuery<Dropdownsmodel>(query).ToList();
            return customers;
        }
        public List<Dropdownsmodel> GetAllSuppliers()
        {
            var query = "select supplier_code as Code,supplier_edesc as Name from IP_SUPPLIER_SETUP where DELETED_FLAG='N' AND COMPANY_CODE='01'";
            var customers = _objectEntity.SqlQuery<Dropdownsmodel>(query).ToList();
            return customers;
        }
        public List<Dropdownsmodel> GetAllDivision()
        {
            var query = "select division_code as Code,division_edesc as Name from FA_DIVISION_SETUP where DELETED_FLAG='N' AND COMPANY_CODE='01'";
            var customers = _objectEntity.SqlQuery<Dropdownsmodel>(query).ToList();
            return customers;
        }
        public List<Dropdownsmodel> GetAllLedgers()
        {
            var query = "select ACC_CODE as Code,ACC_EDESC as Name from FA_CHART_OF_ACCOUNTS_SETUP where DELETED_FLAG='N' AND COMPANY_CODE='01'";
            var customers = _objectEntity.SqlQuery<Dropdownsmodel>(query).ToList();
            return customers;
        }
        public int createEntities(string systemName)
        {
            TextInfo ProperCase = new CultureInfo("en-US", false).TextInfo;
            systemName = ProperCase.ToTitleCase(systemName.ToLower());
            FileInfo file = new FileInfo(System.AppDomain.CurrentDomain.BaseDirectory + $"Areas\\NeoErp.{systemName}\\App_Data\\ENTITES.sql");
            var stream = file.OpenText();
            string script = stream.ReadToEnd();
            stream.Close();
            stream.Dispose();

            var individualStmt = script.Split('#').ToList();
            var count = 0;
            foreach (var stmt in individualStmt)
            {
                try
                {
                    var success = _objectEntity.ExecuteSqlCommand(stmt);
                    count += success;
                }
                catch (Exception ex) { }
            }
            return count;
        }
    }
}