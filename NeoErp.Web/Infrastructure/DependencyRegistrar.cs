using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NeoErp.Core.Infrastructure.DependencyManagement;
using Autofac;
using NeoErp.Core.Infrastructure;
using Autofac.Integration.Mvc;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using NeoErp.Data;
using NeoErp.Core.Models;
using NeoErp.Controllers;
using NeoErp.Core.Plugins;
using NeoErp.Core.Caching;
using NeoErp.Core;
using NeoErp.Core.Data;
using NeoErp.Core.Services;
using NeoErp.Framework;
using NeoErp.Models.Common;
using System.Reflection;
using System.Web.Http;
using Autofac.Integration.WebApi;
using NeoErp.Sales.Modules.Services.Services;
using NeoErp.Models.Settings;
using NeoErp.Core.Services.MenuControlService;
using NeoErp.Services;
using Quartz.Impl;
using Quartz;
using NeoErp.Core.Services.Scheduler;
using System.Web.Compilation;
using Quartz.Spi;
using NeoErp.Core.Services.QuickCapSettingService;
using NeoErp.Models.WarrantyChecker;
using NeoErp.Core.Services.CommonSetting;
using NeoErp.Services.ThirdPatryServices;
using NeoErp.Services.MobileWeb;
using NeoErp.Services.MobileWebSetup;
using NeoErp.Services.LogService;
using NeoErp.Core.Models.Log4NetLoggin;
using NeoErp.Core.Services.TemplateMappingServices;
using NeoErp.Services.AccessManager;
using NeoErp.Services.UserService;
//using NeoErp.Services.ThirdPatryServices;

namespace NeoErp.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>

        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            builder.Register(c => c.Resolve<HttpContextBase>().Request)
              .As<HttpRequestBase>()
              .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Response)
                .As<HttpResponseBase>()
                .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Server)
                .As<HttpServerUtilityBase>()
                .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Session)
                .As<HttpSessionStateBase>()
                .InstancePerLifetimeScope();

            builder.RegisterModule(new AutofacWebTypesModule());
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterControllers(typeFinder.GetAssemblies().ToArray());
            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
            builder.RegisterType<WebHelper>().As<IWebHelper>().InstancePerLifetimeScope().PreserveExistingDefaults();
            builder.RegisterType<PluginFinder>().As<IPluginFinder>().InstancePerLifetimeScope();
            builder.RegisterType<MemoryCacheManager>().As<ICacheManager>().Named<ICacheManager>("Neo_cache_static").SingleInstance();
            //builder.RegisterType<PerRequestCacheManager>().As<ICacheManager>().Named<ICacheManager>("Neo_cache_per_request").InstancePerLifetimeScope();
            builder.RegisterType(typeof(NeoErpCoreEntity)).As(typeof(IDbContext)).AsSelf().InstancePerLifetimeScope();
            //builder.Register(c =>
            //{
            //    var selectedDb = ConnectionManager.CurrentSelectedDatabase;
            //    return new NeoErpCoreEntity(selectedDb);
            //}).As(typeof(IDbContext)).AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<FormsAuthenticationService>().As<IAuthenticationService>().InstancePerLifetimeScope();
            builder.RegisterType<WebWorkContext>().As<IWorkContext>().InstancePerLifetimeScope();
            builder.RegisterType<MenuModel>().As<IMenuModel>().InstancePerLifetimeScope();
            builder.RegisterType<SalesRegister>().As<ISalesRegister>().InstancePerLifetimeScope();
            builder.RegisterType<ControlService>().As<IControlService>().InstancePerLifetimeScope();
            builder.RegisterType<MobileService>().As<IMobileService>().InstancePerLifetimeScope();
            builder.RegisterType<MenuSettingsModel>().As<IMenuSettings>().InstancePerLifetimeScope();
            builder.RegisterType<MenuControlService>().As<IMenuControl>().InstancePerLifetimeScope();
            builder.RegisterType<QueryBuilder>().As<IQueryBuilder>().InstancePerLifetimeScope();
            builder.RegisterType<DashBoardMetricService>().As<IDashBoardMetricService>().InstancePerLifetimeScope();
            builder.RegisterType<MessageService>().As<IMessageService>().InstancePerLifetimeScope();
            builder.RegisterType<MailSetting>().As<IMailSetting>().InstancePerLifetimeScope();
            //builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies()).Where(x => typeof(IJob).IsAssignableFrom(x)).AsImplementedInterfaces().InstancePerRequest();
            builder.Register(x => new StdSchedulerFactory().GetScheduler()).As<IScheduler>();
            builder.RegisterType<ReportBuilderService>().As<IReportBuilderService>().InstancePerLifetimeScope();
            builder.RegisterType<QuickCapSettingService>().As<IQuickCapSetting>().InstancePerLifetimeScope();
            builder.RegisterType<WarrantyCheckerImplementation>().As<IWarrantyChecker>().InstancePerLifetimeScope();
            builder.RegisterType<UserChartPermission>().As<IUserChartPermission>().InstancePerLifetimeScope();
            //builder.RegisterType<MailControlService>().As<IMailControl>().InstancePerLifetimeScope();
            builder.RegisterType<SettingService>().As<ISettingService>().InstancePerLifetimeScope();
            builder.RegisterType<MobileNotificationService>().As<IMobileNotificationService>().InstancePerLifetimeScope();
            builder.RegisterType<CRMMobileService>().As<ICRMMobileService>().InstancePerLifetimeScope();
            builder.RegisterType<CrmService>().As<ICrmService>().InstancePerLifetimeScope();
            builder.RegisterType<ReportConfig>().As<IReportConfig>().InstancePerLifetimeScope();
            builder.RegisterType<MobileWebServices>().As<IMobileWeb>().InstancePerLifetimeScope();
            //builder.RegisterType<Core.MongoDBRepository.Repository.MongoRepository>().As<Core.Data.IRepository>().WithParameter(new ).InstancePerLifetimeScope();
            //builder.RegisterType<EmailJob>().As<IJob>();
            //GlobalConfiguration
            
            //Prem Prakash Dhakal
            builder.RegisterType<QuotationServices>().As<IQuotation>().InstancePerLifetimeScope();


            //Register Log Viewer on Home Controller constructor

            builder.RegisterType<LogViewer>().As<ILogViewer>().InstancePerLifetimeScope();
            builder.Register(c => new LogErp("object", "string", "string", "string", "string", "string", "string"))
                .As<ILogErp>().InstancePerLifetimeScope();


            //Register TemplateService and ITemplateMappingSerice For Print Template Implementation
            builder.RegisterType<TemplateMappingService>().As<ITemplateMappingService>().InstancePerLifetimeScope();

            builder.RegisterType<AccessManagerService>().As<IAccessManager>().InstancePerLifetimeScope();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();

            builder.RegisterType<NeoErp.Models.Settings.FormSetupModel>().As<NeoErp.Core.Services.IFormSetup>().InstancePerLifetimeScope();

           

        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order
        {
            get { return 1000; }
        }
    }
}