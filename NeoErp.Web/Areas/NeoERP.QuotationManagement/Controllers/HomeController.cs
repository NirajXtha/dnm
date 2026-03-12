using NeoErp.Core.Controllers;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using NeoErp.Data;
using NeoErp.Core.Models.Log4NetLoggin;
using NeoErp.Core;
using System.Xml.Linq;
using NeoERP.QuotationManagement.Controllers.Api;
using NeoErp.Core.Caching;
using NeoErp.Core.Models;
using NeoErp.Core.Services.CommonSetting;
using NeoERP.QuotationManagement.Service.Models;
using NeoERP.QuotationManagement.Service.Interface;

namespace NeoERP.QuotationManagement.Controllers
{
    public class HomeController :Controller
    {
        private readonly ILogErp _logErp;
        private DefaultValueForLog _defaultValueForLog;
        private IWorkContext _workContext;
        private IDbContext _dbContext;

        public HomeController(IWorkContext workContext, IDbContext dbContext)
        {
            this._dbContext = dbContext;
            this._workContext = workContext;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            this._logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule, _defaultValueForLog.FormCode);
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult QuotationSetup()
        {
            //List<Company> companyDetail = this.GetCompany();
            //return View(companyDetail);
            return View();
        }
        public ActionResult QuotationApproval()
        {
            return View();
        }
        public ActionResult QuotationLogs()
        {
            return View();
        }
        public ActionResult ValueAssign()
        {
            return View();
        }
        public ActionResult UserPermission()
        {
            return View();
        }
        public ActionResult Dashboard()
        {
            return View();
        }
        public List<Company> GetCompany()
        {
            var company_code = _workContext.CurrentUserinformation.company_code;
            string query = $@"select COMPANY_CODE,COMPANY_EDESC,ADDRESS,EMAIL,LOGO_FILE_NAME from COMPANY_SETUP WHERE COMPANY_CODE='{company_code}'";
            List<Company> company = _dbContext.SqlQuery<Company>(query).ToList();
            return company;
        }
        public ActionResult QuotationDetails()
        {
            return View();
        }
        public ActionResult SummaryReport()
        {
            return View();
        }
        public ActionResult TenderSetup()
        {
            return View();
        }
        public ActionResult QuotationDetail(string id)
        {
            return View();
        }
        public ActionResult QuotationDetailItemwise(string quotation,string tender)
        {
            return View();
        }
        public ActionResult AddQuotation()
        {
            return View();
        }
        public ActionResult ViewQuotation(string id)
        {
            return View();
        }
        public ActionResult EditQuotation(string id)
        {
            return View();
        }
    }
}