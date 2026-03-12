using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Xml;
using NeoErp.Models.Common;
using System.Configuration;
using NeoErp.Core.Services.MenuControlService;
using NeoErp.Core;
using NeoErp.Data;
using NeoErp.Services.UserService;
using NeoErp.Core.Models.Log4NetLoggin;
using NeoErp.Core.Services.CommonSetting;
using NeoErp.Core.Plugins;
using NeoErp.Core.Infrastructure;

namespace NeoErp.Controllers
{

    public class FormSetupController : Controller
    {

        private readonly IPluginFinder _pluginFinder;
        private string LoggedInUserId;
        private ISettingService _setting;
        private ILogErp _logErp;
        private IUserService _userService;
        private IDbContext _dbContext;
        private IWorkContext _workContext;
        public FormSetupController(IPluginFinder pluginFinder, ISettingService settingService, IUserService userService, IDbContext dbContext, IWorkContext workContext)
        {
            this._pluginFinder = pluginFinder;
            var workingContent = EngineContext.Current.Resolve<IWorkContext>();
            LoggedInUserId = workingContent.CurrentUserinformation.login_code.ToString();
            this._setting = settingService;
            this._logErp = new LogErp(this);
            _userService = userService;
            _dbContext = dbContext;
            _workContext = workContext;
        }


        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

    }
}