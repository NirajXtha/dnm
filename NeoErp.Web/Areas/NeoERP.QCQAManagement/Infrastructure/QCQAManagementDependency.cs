using Autofac;
using Autofac.Integration.WebApi;
using NeoErp.Core.Infrastructure;
using NeoErp.Core.Infrastructure.DependencyManagement;
using NeoERP.QCQAManagement.Service.Interface;
using NeoERP.QCQAManagement.Service.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace NeoERP.QCQAManagement.Infrastructure
{
    public class QCQAManagementDependency : IDependencyRegistrar
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
            builder.RegisterType<QCQASetup>().As<IQCQARepo>().InstancePerLifetimeScope();
            builder.RegisterType<MaterialRepo>().As<IMaterialRepo>().InstancePerLifetimeScope();
            builder.RegisterType<QCQANumberSetup>().As<IQCQANumberRepo>().InstancePerLifetimeScope();
            builder.RegisterType<ParameterSetup>().As<IParameterRepo>().InstancePerLifetimeScope();
            builder.RegisterType<DailyWastageRepo>().As<IDailyWastageRepo>().InstancePerLifetimeScope();
            builder.RegisterType<PreDispatchInspectionRepo>().As<IPreDispatchInspectionRepo>().InstancePerLifetimeScope();
            builder.RegisterType<HandOverInspectionRepo>().As<IHandOverInspectionRepo>().InstancePerLifetimeScope();
            builder.RegisterType<LabTestingRepo>().As<ILabTestingRepo>().InstancePerLifetimeScope();
            builder.RegisterType<GlobalAgroProductsRepo>().As<IGlobalAgroProductsRepo>().InstancePerLifetimeScope();
            builder.RegisterType<ProductSetupRepo>().As<IProductSetupRepo>().InstancePerLifetimeScope();
            builder.RegisterType<ParameterInspectionSetupRepo>().As<IParameterInspectionSetupRepo>().InstancePerLifetimeScope();
            builder.RegisterType<FinishedGoodsSetupRepo>().As<IFinishedGoodsSetupRepo>().InstancePerLifetimeScope();
            builder.RegisterType<InternalInspectionSetupRepo>().As<IInternalInspectionSetupRepo>().InstancePerLifetimeScope();
            builder.RegisterType<OnSiteInspectionRepo>().As<IOnSiteInspectionRepo>().InstancePerLifetimeScope();
            builder.RegisterType<InternalInspectionRepo>().As<IInternalInspectionRepo>().InstancePerLifetimeScope();
            builder.RegisterType<SanitationHygieneRepo>().As<ISanitationHygieneRepo>().InstancePerLifetimeScope();
            builder.RegisterType<FinishedGoodsInspectionRepo>().As<IFinishedGoodsInspectionRepo>().InstancePerLifetimeScope();
            builder.RegisterType<QCQADocumentFinderRepo>().As<IQCQADocumentFinderRepo>().InstancePerLifetimeScope();
        }
    }
}