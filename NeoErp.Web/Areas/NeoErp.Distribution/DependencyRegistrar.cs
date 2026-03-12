using Autofac;
using Autofac.Integration.WebApi;
using NeoErp.Core.Infrastructure;
using NeoErp.Core.Infrastructure.DependencyManagement;
using NeoErp.Core.Models;
using NeoErp.Data;
using NeoErp.Distribution.Service;
using NeoErp.Distribution.Service.DistributorServices;
using NeoErp.Distribution.Service.Service;
using NeoErp.Distribution.Service.Service.Branding;
using NeoErp.Distribution.Service.Service.Mobile;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.Linq;
using System.Reflection;
using System.Web;
using NeoErp.Distribution.Service.Service.Scheme;
using NeoErp.Distribution.Service.Service.Scheme.authentication;
using NeoErp.Distribution.Service.Service.Scheme.Master;
using NeoErp.Distribution.Service.Service.Scheme.Points;
using NeoErp.Distribution.Service.Service.Scheme.Scheme;

namespace NeoErp.Distribution
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
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
        //     var connection= ConfigurationManager.ConnectionStrings["DistributionNeoErpCoreEntity"].ConnectionString;
        // var entityconnectioin=    new EntityConnection(connection);
            //builder.RegisterType(typeof(NeoErpCoreEntity)).As(typeof(IDbContext)).WithParameter(new TypedParameter(typeof(EntityConnection), entityconnectioin)).InstancePerLifetimeScope();
            builder.RegisterType<DistributionService>().As<IDistributionService>().InstancePerLifetimeScope();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();
            builder.RegisterType<SetupService>().As<ISetupService>().InstancePerLifetimeScope();
            builder.RegisterType<MobileService>().As<IMobileService>().InstancePerLifetimeScope();
            builder.RegisterType<ActionSelector>().As<IActionSelector>().InstancePerLifetimeScope();
            builder.RegisterType<MobileOfflineService>().As<IMobileOfflineService>().InstancePerLifetimeScope();
            builder.RegisterType<DistributorService>().As<IDistributorService>().InstancePerLifetimeScope();
            builder.RegisterType<Dashboard>().As<IDashboard>().InstancePerLifetimeScope();
            builder.RegisterType<Core.Services.MessageService>().As<Core.Services.IMessageService>().InstancePerLifetimeScope();
            builder.RegisterType<BrandingService>().As<IBrandingService>().InstancePerLifetimeScope();
            builder.RegisterType<QuickSetupService>().As<IQuickSetupService>().InstancePerLifetimeScope();
            builder.RegisterType<MobileResellerService>().As<IMobileResellerService>().InstancePerLifetimeScope();


            /// SCHEME
            builder.RegisterType<SchemeServiceProvider>().As<ISchemeServiceProvider>().InstancePerLifetimeScope();
            builder.RegisterType<AuthenticationService>().As<IAuthenticationService>().InstancePerLifetimeScope();
            builder.RegisterType<MasterService>().As<IMasterService>().InstancePerLifetimeScope();
            builder.RegisterType<PointsService>().As<IPointsService>().InstancePerLifetimeScope();
            builder.RegisterType<Scheme>().As<IScheme>().InstancePerLifetimeScope();
            /// SCHEME END
            /// 

            //GlobalConfiguration
            //GlobalConfiguration

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