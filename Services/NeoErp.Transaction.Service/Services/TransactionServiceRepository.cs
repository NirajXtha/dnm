using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoErp.Transaction.Service.Models;
using NeoErp.Data;
using NeoErp.Core.Models;
using NeoErp.Core;
using NeoErp.Core.Caching;

namespace NeoErp.Transaction.Service.Services
{
    public class TransactionServiceRepository : ITransactionServiceRepository
    {
        private IDbContext _dbContext;
        private IWorkContext _workContext;
        private ICacheManager _cacheManager;
        public TransactionServiceRepository(IDbContext dbContext, IWorkContext workContext, ICacheManager cacheManager)
        {
            this._dbContext = dbContext;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
        }

        public List<CostCenter> GetAllCostCenter(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
                List<CostCenter> CostCenterList = new List<CostCenter>();
                string query = string.Format(@"select COALESCE(Budget_code,' ') as BudgetCode,COALESCE(Budget_EDESC,' ') as BudgetName 
                                from BC_BUDGET_CENTER_SETUP 
                                where deleted_flag='N' and 
                                (budget_code like '%{0}%' or Budget_EDESC like '%{0}%')", 
                                filter.ToUpperInvariant());
                CostCenterList = this._dbContext.SqlQuery<CostCenter>(query).ToList();
                return CostCenterList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Customers> GetAllCustomerSetup(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
                List<Customers> customerList = new List<Customers>(); 
                string key = TransactionCacheConstants.CustomerCacheKey;
                
                if (this._cacheManager.IsSet(key))
                {
                    customerList = this._cacheManager.Get<List<Customers>>(key).Where(a=>a.CustomerName.Contains(filter)
                    || a.CustomerCode.Contains(filter) || a.REGD_OFFICE_EADDRESS.Contains(filter)
                    || a.TEL_MOBILE_NO1.Contains(filter) || a.TPIN_VAT_NO.Contains(filter)
                    || a.REGION_CODE.Contains(filter) || a.ZONE_CODE.Contains(filter)
                    || a.DEALING_PERSON.Contains(filter)).ToList();
                }
                else if(!this._cacheManager.IsSet(key) || string.IsNullOrEmpty(filter))
                {
                    string query = string.Format(@"SELECT
                    INITCAP(CS.CUSTOMER_EDESC) AS CustomerName,
                    CS.CUSTOMER_CODE AS CustomerCode,
                    COALESCE(CS.REGD_OFFICE_EADDRESS,' ') REGD_OFFICE_EADDRESS,
                    COALESCE(CS.TEL_MOBILE_NO1,' ') TEL_MOBILE_NO1,
                    COALESCE(cs.TPIN_VAT_NO,' ') TPIN_VAT_NO,
                    COALESCE(CS.REGION_CODE,' ') REGION_CODE,
                    COALESCE(CS.ZONE_CODE,' ') ZONE_CODE,
                    COALESCE(CS.DEALING_PERSON,' ') DEALING_PERSON
                    FROM SA_CUSTOMER_SETUP CS
                    where CS.DELETED_FLAG='N'
                    and (upper(CS.CUSTOMER_EDESC) like '%{0}%'
                    OR upper(CS.CUSTOMER_CODE) like '%{0}%'
                    OR upper(CS.REGD_OFFICE_EADDRESS) like '%{0}%'
                    OR upper(CS.TEL_MOBILE_NO1) like '%{0}%'
                    OR upper(cs.TPIN_VAT_NO) like '%{0}%'
                    OR upper(CS.REGION_CODE) like '%{0}%'
                    OR upper(CS.ZONE_CODE) like '%{0}%'
                    OR upper(CS.DEALING_PERSON) like '%{0}%')
                    order by Customer_code", filter.ToUpperInvariant());
                    customerList = this._dbContext.SqlQuery<Customers>(query).ToList();
                    this._cacheManager.Set(key, customerList,1);
                }
                return customerList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<Department> GetAllDepartmentSetup(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
                List<Department> DepartmentList = new List<Department>();
                //string query = @"select Department_code as DepartmentCode, Department_edesc as DepartmentName from hr_department_code";
                string query = string.Format(@"Select
                        COALESCE(Department_code,' ') as DepartmentCode, 
                        COALESCE(Department_edesc,' ') as DepartmentName 
                        from hr_department_code
                        where Deleted_flag='N'
                        and(upper(department_edesc) like '%{0}%' or upper(department_code) like '%{0}%')", filter.ToUpperInvariant());
                DepartmentList = this._dbContext.SqlQuery<Department>(query).ToList();
                return DepartmentList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<Employee> GetAllEmployeeSetup(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
                var query = string.Format(@"select 
                             COALESCE(EMPLOYEE_CODE,' ') EMPLOYEE_CODE
                            ,COALESCE(EMPLOYEE_EDESC,' ') EMPLOYEE_EDESC
                            ,COALESCE(EMAIL,' ') EMAIL
                            ,COALESCE(EPERMANENT_ADDRESS1,' ') EPERMANENT_ADDRESS1
                            ,COALESCE(ETEMPORARY_ADDRESS1,' ') ETEMPORARY_ADDRESS1
                            ,COALESCE(MOBILE,' ') MOBILE
                            ,COALESCE(CITIZENSHIP_NO,' ') CITIZENSHIP_NO 
                            from HR_EMPLOYEE_SETUP 
                            where deleted_flag='N' 
                            and Employee_code like '%{0}%' 
                            or upper(employee_edesc) like '%{0}%' 
                            or upper(epermanent_address1) like '%{0}%'
                            OR ETEMPORARY_ADDRESS1 LIKE '%{0}%' 
                            OR CITIZENSHIP_NO LIKE '%{0}%' 
                            OR EMAIL LIKE '%{0}%'  
                            OR MOBILE LIKE '%{0}%'", 
                            filter.ToUpperInvariant());
                var employeeList = this._dbContext.SqlQuery<Employee>(query).ToList();
                return employeeList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<Location> GetAllLocationSetup(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
                List<Location> LocationList = new List<Location>();
                string query = string.Format(@"select  
                        COALESCE(location_code,' ') as LocationCode
                        ,COALESCE(Location_edesc,' ') as LocationName 
                        ,COALESCE(Auth_Contact_Person,' ') as Auth_Contact_Person
                        ,COALESCE(Telephone_Mobile_No,' ') as Telephone_Mobile_No
                        ,COALESCE(Email,' ') as Email
                        ,COALESCE(Fax,' ') as Fax
                        from IP_location_setup 
                        where deleted_flag='N' 
                        and (location_code like '%{0}%' or location_edesc like '%{0}%'
                                or Auth_Contact_Person like '%{0}%'
                                or Telephone_Mobile_No like '%{0}%'
                                or Email like '%{0}%'
                                or Fax like '%{0}%')", filter);
                LocationList = this._dbContext.SqlQuery<Location>(query).ToList();
                return LocationList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<Products> GetAllProducts(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
                List<Products> ProductsList = new List<Products>();
                string query =string.Format(@"select 
                    COALESCE(item_code,' ') as ItemCode
                    ,COALESCE(item_edesc,' ') as ItemDescription
                    ,COALESCE(index_mu_code,' ') as ItemUnit 
                    from ip_item_master_setup
                    where deleted_flag='N'
                    and
                    (
                        upper(item_code) like '%{0}%'
                        or upper(item_edesc) like '%{0}%'
                        or upper(index_mu_code) like '%{0}%'
                    )", filter.ToUpperInvariant());
                ProductsList = this._dbContext.SqlQuery<Products>(query).ToList();
                return ProductsList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Consumption GetConsumptionSetup()
        {
            try
            {
                //select FN_NEW_VOUCHER_NO('01', '357', sysdate-365, 'IP_GOODS_ISSUE') as IssueNo from dual
                string issueNoQuery = @"select FN_NEW_VOUCHER_NO('01', '357', sysdate-365, 'IP_GOODS_ISSUE') as IssueNo from dual";               
                string issueNo = this._dbContext.SqlQuery<string>(issueNoQuery).First();
                Consumption c = new Consumption()
                {
                    IssueNo = issueNo,
                    Date = DateTime.Today.Date.ToString("yyyy-MM-dd"),
                    //Miti = DateTime.Today.Date.ToString("yyyy-MM-dd"),                    
                };

                //cache set for customer
                string key = TransactionCacheConstants.CustomerCacheKey;
                if (this._cacheManager.IsSet(key))
                {
                    string filter = string.Empty;                    
                    List<Customers> customerList = new List<Customers>();
                    string query = string.Format(@"SELECT
                    INITCAP(CS.CUSTOMER_EDESC) AS CustomerName,
                    CS.CUSTOMER_CODE AS CustomerCode,
                    COALESCE(CS.REGD_OFFICE_EADDRESS,' ') REGD_OFFICE_EADDRESS,
                    COALESCE(CS.TEL_MOBILE_NO1,' ') TEL_MOBILE_NO1,
                    COALESCE(cs.TPIN_VAT_NO,' ') TPIN_VAT_NO,
                    COALESCE(CS.REGION_CODE,' ') REGION_CODE,
                    COALESCE(CS.ZONE_CODE,' ') ZONE_CODE,
                    COALESCE(CS.DEALING_PERSON,' ') DEALING_PERSON
                    FROM SA_CUSTOMER_SETUP CS
                    where CS.DELETED_FLAG='N'
                    and (upper(CS.CUSTOMER_EDESC) like '%{0}%'
                    OR upper(CS.CUSTOMER_CODE) like '%{0}%'
                    OR upper(CS.REGD_OFFICE_EADDRESS) like '%{0}%'
                    OR upper(CS.TEL_MOBILE_NO1) like '%{0}%'
                    OR upper(cs.TPIN_VAT_NO) like '%{0}%'
                    OR upper(CS.REGION_CODE) like '%{0}%'
                    OR upper(CS.ZONE_CODE) like '%{0}%'
                    OR upper(CS.DEALING_PERSON) like '%{0}%')
                    order by Customer_code", filter.ToUpperInvariant());
                    customerList = this._dbContext.SqlQuery<Customers>(query).ToList();
                    this._cacheManager.Set(key, customerList, 10);
                }
                return c;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveConsumptionIssue(ConsumptionIssue issue)
        {
            try
            {
                string company_code = this._workContext.CurrentUserinformation.company_code;
                string branch_code = this._workContext.CurrentUserinformation.branch_code;
                string user = this._workContext.CurrentUserinformation.login_code;
                string today = DateTime.Now.ToString("yyyy/MM/dd/ HH.mm.ss");
                //string issueDate = Convert.ToDateTime(issue.Date).ToString("yyyy/mm/dd/ HH.mm.ss");
                string issueDate = issue.Date;

                if (issue.Issues.Count >0)
                {
                    StringBuilder sb = new StringBuilder();
                    
                    foreach(var item in issue.Issues)
                    {
                        sb.AppendLine();

                        string query_insert = string.Format(@"INSERT INTO IP_GOODS_ISSUE(ISSUE_NO,ISSUE_DATE,MANUAL_NO,FROM_LOCATION_CODE,ITEM_CODE,SERIAL_NO,MU_CODE,QUANTITY,UNIT_PRICE,TOTAL_PRICE,REMARKS,ISSUE_TYPE_CODE,FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,CURRENCY_CODE,EXCHANGE_RATE) VALUES('{0}',TO_DATE('{1}','yyyy/mm/dd hh24:mi:ss'),'{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}',TO_DATE('{16}', 'yyyy/mm/dd hh24:mi:ss'),'{17}','{18}','{19}')",
                                                            issue.IssueId, issueDate, issue.MannualNo, item.FromLocation
                                                            , item.ProductDescription, item.SN, item.Unit, item.Quantity,
                                                            item.Rate, item.Amount, item.Remark,"EI", "357", company_code, branch_code, user, today,"N","NRS","1");
                        this._dbContext.ExecuteSqlCommand(query_insert);
                        sb.AppendLine();
                        
                    }
                    foreach(var item in issue.Issues)
                    {
                        sb.AppendLine();
                        string query_insert = string.Format(@"INSERT INTO BUDGET_TRANSACTION(TRANSACTION_NO,FORM_CODE,REFERENCE_NO,SERIAL_NO,COMPANY_CODE,BRANCH_CODE,CREATED_BY,DELETED_FLAG,CURRENCY_CODE,CREATED_DATE,BUDGET_FLAG,BUDGET_CODE,BUDGET_AMOUNT) VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',TO_DATE('{9}','yyyy/mm/dd hh24:mi:ss'),'{10}','{11}','{12}')",
                                        "100", "357",issue.IssueId, item.SN, company_code, branch_code, user, 'N', "NRS",today,'E',item.CostCenter,'0');
                        this._dbContext.ExecuteSqlCommand(query_insert);
                        sb.AppendLine();
                    }

                    foreach(var item in issue.Issues)
                    {
                        sb.AppendLine();
                        string query_insert = string.Format(@"INSERT INTO MASTER_TRANSACTION(VOUCHER_NO,VOUCHER_AMOUNT,FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,DELETED_FLAG,CURRENCY_CODE,CREATED_DATE) VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',TO_DATE('{8}','yyyy/mm/dd hh24:mi:ss'))",
                            issue.IssueId,issue.GrandTotal, "357",company_code, branch_code, user, 'N', "NRS",today);
                        this._dbContext.ExecuteSqlCommand(query_insert);
                        sb.AppendLine();
                    }
                    string query = sb.ToString();
                    //var insertResult = this._dbContext.ExecuteSqlCommand(sb.ToString(),true);
                    this._dbContext.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
