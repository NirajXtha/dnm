using Autofac;
using Autofac.Integration.WebApi;
using NeoErp.Core.Caching;
using NeoErp.Core.Infrastructure;
using NeoErp.Core.Infrastructure.DependencyManagement;
using NeoErp.Pos.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace NeoErp.Pos
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

            builder.RegisterType<PosService>().As<IPosService>().InstancePerLifetimeScope();
            builder.RegisterType<ItemImageService>().As<IItemImageService>().InstancePerLifetimeScope();
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