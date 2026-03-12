using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Models
{
    public class UserChartModel
    {
        public List<string> Users { get; set; }
        public List<string> Charts { get; set; }
        public string ModuleName { get; set; }
        public string StaticDasboard { get; set; }
        public List<string> QuickWidgets { get; set; } = new List<string>();

    }
    public class UserChartUpdateModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
    }

    
    public class UserChartPermissionModel
    {
        public string User { get; set; }
        public List<string> Charts { get; set; }
        public string ModuleName { get; set; }
    }


    public class UserListModel
    {
        public string id { get; set; }
        public string login_code { get; set; }
        public string login_edesc { get; set; }
    }


    public class UserWiseChartConfigModel
    {
        public string ChartName { get; set; }
        public string ChartType { get; set; }
        public string FieldValue { get; set; }
        public string DateFormat { get; set; }
        public string SalesReturn { get; set; }
        public string ShowLabel { get; set; }
        public string IsStack { get; set; }
        public string FiscalYear { get; set; }
        public string ProductList { get; set; }
        
    }
}