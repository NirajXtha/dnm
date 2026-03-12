using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Transaction.Service.Models
{
    public class Consumption
    {
        public string IssueNo { get; set; }
        public string ManualNo { get; set; }
        public string UserId { get; set; }
        public string DepartmentId { get; set; }
        public string Date { get; set; }
        public string Miti { get; set; }
        public string EmployeeId { get; set; }

        public virtual List<Users> Users { get; set; }
        public virtual List<Department> Departments { get; set; }
        public virtual List<Employee> Employees { get; set; }
    }

    public class ConsumptionIssue
    {
        public string Date { get; set; }
        public string Miti { get; set; }
        public string DepartmentId { get; set; }
        public string EmployeeId { get; set; }
        public string IssueId { get; set; }
        public string MannualNo { get; set; }
        public string UserId { get; set; }
        public string GrandTotal { get; set; }
        public List<Issues> Issues { get; set; }
    }

    public class Issues
    {
        //public Location FromLocation { get; set; }
        //public Products ProductDescription { get; set; }
        public string FromLocation { get; set; }
        public string ProductDescription { get; set; }
        public string Unit { get; set; }
        public string Quantity { get; set; }
        public string Rate { get; set; }
        public string Amount { get; set; }
        //public CostCenter CostCenter { get; set; } 
        public string CostCenter { get; set; }
        public string Remark { get; set; }
        public string SN { get; set; }
    }

    public class Employee
    {
        public string ID { get; set; }
        public string Employee_Name { get; set; }
        public string EMPLOYEE_CODE { get; set; }
        public string EMPLOYEE_EDESC { get; set; }
        public string EMAIL { get; set; }
        public string EPERMANENT_ADDRESS1 { get; set; }
        public string ETEMPORARY_ADDRESS1 { get; set; }
        public string MOBILE { get; set; }
        public string CITIZENSHIP_NO { get; set; }

    }

    public class Department
    {
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
    }

    public class Location
    {
        public string LocationCode { get; set; }
        public string locationName { get; set; }
        public string Auth_Contact_Person { get; set; }
        public string Telephone_Mobile_No { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
    }

    public class Products
    {
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string ItemUnit { get; set; }
    }

    public class CostCenter
    {
        public string BudgetCode { get; set; }
        public string BudgetName { get; set; }
    }

    public class Users
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
    }

    public class Customers
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string REGD_OFFICE_EADDRESS { get; set; }
        public string TEL_MOBILE_NO1 { get; set; }
        public string TPIN_VAT_NO { get; set; }
        public string REGION_CODE { get; set; }
        public string ZONE_CODE { get; set; }
        public string DEALING_PERSON { get; set; }
    }
}