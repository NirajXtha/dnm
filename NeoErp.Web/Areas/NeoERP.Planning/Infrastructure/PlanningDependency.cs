using NeoErp.Core.Infrastructure.DependencyManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NeoErp.Core.Infrastructure;
using Autofac;
using System.Reflection;
using Autofac.Integration.WebApi;
using NeoErp.Planning.Service.Repository;
using NeoErp.Planning.Service.Interface;

namespace NeoErp.Planning.Infrastructure
{
    public class PlanningDependency : IDependencyRegistrar
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
            builder.RegisterType<Plan>().As<IPlan>().InstancePerLifetimeScope();
            builder.RegisterType<PlanSetupRepo>().As<IPlanSetup>().InstancePerLifetimeScope();
            builder.RegisterType<SubPlanRepo>().As<ISubPlanRepo>().InstancePerLifetimeScope();
            builder.RegisterType<DistributionPlaning>().As<IDistributionPlaning>().InstancePerLifetimeScope();
            builder.RegisterType<PlanReportRepo>().As<IPlanReport>().InstancePerLifetimeScope();
            builder.RegisterType<COAPlanRepo>().As<ICOAPlanRepo>().InstancePerLifetimeScope();
            builder.RegisterType<LBAPlanRepo>().As<ILBAPlanRepo>().InstancePerLifetimeScope();
            builder.RegisterType<ProcurementPlanRepo>().As<IProcurementPlanRepo>().InstancePerLifetimeScope();
            builder.RegisterType<CollectionPlanRepo>().As<ICollectionPlanRepo>().InstancePerLifetimeScope();
            builder.RegisterType<BrandingPlanRepo>().As<IBrandingPlanRepo>().InstancePerLifetimeScope();
            builder.RegisterType<ProductionPlanRepo>().As<IProductionPlanRepo>().InstancePerLifetimeScope();
            builder.RegisterType<MaterialPlanRepo>().As<IMaterialPlanRepo>().InstancePerLifetimeScope();
            builder.RegisterType<DashBoardMetricService>().As<IDashBoardMetricService>().InstancePerLifetimeScope();
            builder.RegisterType<ChartReportRepo>().As<IChartReport>().InstancePerLifetimeScope();
            builder.RegisterType<TargetExcelUploadRepo>().As<ITargetExcelUploadRepo>().InstancePerLifetimeScope();
            //builder.RegisterType<TargetServiceRepositoy>().As<ITargetServiceRepository>().InstancePerLifetimeScope();
        }
    }
}

