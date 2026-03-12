using NeoErp.Core.Infrastructure.DependencyManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NeoErp.Core.Infrastructure;
using Autofac;
using System.Reflection;
using Autofac.Integration.WebApi;
using NeoErp.Transaction.Service.Services;
using NeoErp.Transaction.Service.Services.BankSetup;

namespace NeoErp.Transaction.Infrastructure
{
    public class TransactionDependency : IDependencyRegistrar
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
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            //builder.RegisterType<Constants>().As<IConstants>().InstancePerLifetimeScope();
            builder.RegisterType<TransactionServiceRepository>().As<ITransactionServiceRepository>().InstancePerLifetimeScope();
            builder.RegisterType<TargetServiceRepositoy>().As<ITargetServiceRepository>().InstancePerLifetimeScope();
            builder.RegisterType<BankSetupService>().As<IBankSetupService>().InstancePerLifetimeScope();
        }
    }
}