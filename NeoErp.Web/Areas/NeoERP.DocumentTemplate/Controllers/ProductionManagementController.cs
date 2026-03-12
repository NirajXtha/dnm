using NeoErp.Core.Controllers;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using NeoErp.Core.Models.Log4NetLoggin;
using NeoErp.Core;

namespace NeoERP.DocumentTemplate.Controllers
{
    public class ProductionManagementController : BaseController
    {
        private readonly ILogErp _logErp;
        private DefaultValueForLog _defaultValueForLog;
        private IWorkContext _workContext;
        public ProductionManagementController(IWorkContext workContext)
        {
            this._workContext = workContext;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            this._logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule, _defaultValueForLog.FormCode);
        }
        public ActionResult ProductionPlanning()
        {
            return View();
        }

    }
}