using NeoErp.Core.Infrastructure.DependencyManagement;
using NeoErp.Core.Infrastructure;
using System.Reflection;
using Autofac.Integration.WebApi;
using NeoERP.ProjectManagement.Service.Interface;
using NeoERP.ProjectManagement.Service.Repository;
using Autofac;

namespace NeoERP.ProjectManagement.Infrastructure
{
    public class ProjectManagementDependency : IDependencyRegistrar
    {
        public int Order
        {
            get
            {
                return 1000;
            }
        }

        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            // Register API controllers
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // Register repositories
            builder.RegisterType<ProjectRepo>().As<IFormProjectRepo>().InstancePerLifetimeScope();
            builder.RegisterType<EntryRepo>().As<IEntryRepo>().InstancePerLifetimeScope();
        }
    }
}
