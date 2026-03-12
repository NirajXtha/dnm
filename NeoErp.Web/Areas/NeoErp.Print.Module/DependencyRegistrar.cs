using Autofac;
using Autofac.Integration.Mvc;
using NeoErp.Core.Infrastructure;
using NeoErp.Core.Infrastructure.DependencyManagement;
using NeoErp.Print.Service.Services;
using System.Reflection;

namespace NeoErp.Print.Module
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
            // Register MVC controllers
            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            // Register Print Setup Service
            builder.RegisterType<PrintSetupService>().As<IPrintSetupService>().InstancePerLifetimeScope();
        }

        public int Order
        {
            get { return 0; }
        }
    }
}
