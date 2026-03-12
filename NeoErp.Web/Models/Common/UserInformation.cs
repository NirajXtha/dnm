using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Models.Common
{
    public class UserInformation
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string StartFincalYear { get; set; }
        public string EndFincalYear { get; set; }
    }
    public class EmployeeSchedular
    {
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string PersonalEmail { get; set; }

    }
    public class Customers
    {
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string EMAIL { get; set; }
    }
    public class Suppliers
    {
        public string SuppliersName { get; set; }
        public string SuppliersCode { get; set; }
    }
    public class Dropdownsmodel
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
    public class SummaryReportModel
    {
        public string GROUP_NAME { get; set; }
        public string FULL_NAME { get; set; }
        public string EMAIL { get; set; }
        public decimal? TOTAL_VISIT { get; set; }
        public decimal? TOTAL_OUTLET { get; set; }
        public decimal? TOTAL_COLLECTION { get; set; }
        public decimal? TOTAL_SALES { get; set; }
        public decimal? EFFECTIVECALLS { get; set; }
    }

}