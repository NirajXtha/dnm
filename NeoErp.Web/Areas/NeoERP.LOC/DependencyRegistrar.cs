using System.Reflection;
using Autofac;
using Autofac.Integration.WebApi;
using NeoErp.Core.Infrastructure;
using NeoErp.Core.Infrastructure.DependencyManagement;
using NeoErp.LOC.Services.Services;
using NeoErp.LOC.Services.Interfaces;

namespace NeoERP.LOC
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
            builder.RegisterType<LocRegister>().As<ILocRegister>().InstancePerLifetimeScope();
            builder.RegisterType<LOCService>().As<ILOCService>().InstancePerLifetimeScope();
            builder.RegisterType<CInvoiceService>().As<ICInvoiceService>().InstancePerLifetimeScope();
            builder.RegisterType<SettlementInvoiceService>().As<ISettlementInvoiceService>().InstancePerLifetimeScope();
            builder.RegisterType<LcSetupService>().As<ILcSetupService>().InstancePerLifetimeScope();
            builder.RegisterType<LcEntryService>().As<ILcEntryService>().InstancePerLifetimeScope();
            builder.RegisterType<LcReportService>().As<ILcReportService>().InstancePerLifetimeScope();
            builder.RegisterType<PurchaseOrder>().As<IPurchaseOrder>().InstancePerLifetimeScope();
            builder.RegisterType<PerformaInvoice>().As<IPerfomaInvoice>().InstancePerLifetimeScope();
            builder.RegisterType<ShipmentService>().As<IShipmentService>().InstancePerLifetimeScope();
            builder.RegisterType<DashBoardMetricService>().As<IDashBoardMetricService>().InstancePerLifetimeScope();
            builder.RegisterType<LogisticsService>().As<ILogisticsService>().InstancePerLifetimeScope();
            builder.RegisterType<LcLogisticPlanService>().As<ILcLogisticPlanService>().InstancePerLifetimeScope();
            builder.RegisterType<CIPaymentService>().As<ICIPaymentService>().InstancePerLifetimeScope();
            builder.RegisterType<GRNService>().As<IGRNService>().InstancePerLifetimeScope();
            
            //GlobalConfiguration
            //GlobalConfiguration

        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order { get; } = 1000;
    }
}