using System;
using System.Collections.Generic;
using System.Web;

namespace NeoErp.Core
{
    public  class DefaultValueForLog
    {
        IWorkContext _workContext;


        public string LogUser { get; set; }
        public string LogCompany { get; set; }
        public string LogBranch { get; set; }
        public string LogTypeCode { get; set; }
        public string LogModule { get; set; }
        public string FormCode { get; set; } = "";

        public Dictionary<string, string> ModuleDictionary
        {
            get {
                return new Dictionary<string, string>
                {
                     { "BusinessIntelligent", "01" },
                     { "DocumentTemplate", "06" }
                };
            }
            set { this.ModuleDictionary=value; }
        }

        public Dictionary<string,string> SubModuleDictionary
        {
            get {
                return new Dictionary<string, string>
                {
                    { "FinancialAccounting", "01" },
                    { "InventoryNPurcurement", "02" },
                    { "ProductionManagement", "03" },
                    { "SalesNRevenue", "04" },
                    { "HumanResourceManagement", "05" }
                };
            }
            set { this.SubModuleDictionary = value; }
        }
        public DefaultValueForLog(IWorkContext workContext)
        {
            this._workContext = workContext;
            LogUser = _workContext.CurrentUserinformation.login_code.ToString();
            LogCompany = _workContext.CurrentUserinformation.company_name;
            LogBranch = _workContext.CurrentUserinformation.branch_name;
            LogTypeCode = SubModuleDictionary["SalesNRevenue"];
            LogModule = ModuleDictionary["DocumentTemplate"];
            FormCode = "";
        }

        //private void SetLogModule()
        //{
        //    ModuleDictionary = new Dictionary<string, string>
        //    {
        //        { "Business Intelligent", "1" },
        //        { "Document Template", "6" }
        //    };
        //}

        //private void SetLogSubModule()
        //{
        //    SubModuleDictionary = new Dictionary<string, string>
        //    {
        //        { "FinancialAccounting", "01" },
        //        { "InventoryNPurcurement", "02" },
        //        { "ProductionManagement", "03" },
        //        { "SalesNRevenue", "04" },
        //        { "HumanResourceManagement", "05" }
        //    };
        //}
    }
}