using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models
{
    public class BranchwiseEmployeeNetSalesModel
    {
        public string EMPLOYEE_CODE { get; set; }           
        public string EMPLOYEE_EDESC { get; set; }            
        public string MASTERA_EMPLOYEE_CODE { get; set; }     
        public string PRE_EMPLOYEE_CODE { get; set; }   
        public string GROUP_FLAG { get; set; }                 
        public double? TOTAL_QTY { get; set; }              
        public double? TOTAL_VALUE { get; set; }                 
    }
    public class BranchwiseEmployeeNetSalesViewModel
    {
        public BranchwiseEmployeeNetSalesViewModel()
        {
            BranchwiseEmployeeSales = new List<BranchwiseEmployeeNetSalesModel>();
            AggregationResult = new Dictionary<string, AggregationModel>();
        }

        public List<BranchwiseEmployeeNetSalesModel> BranchwiseEmployeeSales { get; set; }

        public Dictionary<string, AggregationModel> AggregationResult { get; set; }

        public int Total { get; set; }
    }
}

