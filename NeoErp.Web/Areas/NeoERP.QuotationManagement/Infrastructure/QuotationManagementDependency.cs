using NeoErp.Core.Infrastructure.DependencyManagement;
using NeoErp.Core.Infrastructure;
using System.Reflection;
using Autofac.Integration.WebApi;
using Autofac;
using NeoERP.QuotationManagment.Service.Repository;
using NeoERP.QuotationManagement.Service.Interface;

namespace NeoERP.QuotationManagement.Infrastructure
{
    public class QuotationManagementDependency : IDependencyRegistrar
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
            builder.RegisterType<QuotationSetup>().As<IQuotationRepo>().InstancePerLifetimeScope();
        }
    }
}