using Autofac;
using Autofac.Integration.WebApi;
using AutoMapper;
using NeoErp.Core;
using NeoErp.Core.Caching;
using NeoErp.Core.Infrastructure;
using NeoErp.Core.Infrastructure.DependencyManagement;
using NeoErp.Core.Services;
using NeoErp.Sales.Modules.Services.Services;
using NeoErp.Sales.Modules.Services.Services.Analysis;
using NeoErp.Sales.Modules.Services.Services.Consumption;
using NeoErp.Sales.Modules.Services.Services.Contract;
using NeoErp.Sales.Modules.Services.Services.Finance;
using NeoErp.Sales.Modules.Services.Services.Production;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace NeoErp.sales.Module
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
            builder.RegisterType<FreeQty>().As<IFreeQty>().InstancePerLifetimeScope();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterType<SalesRegister>().As<ISalesRegister>().InstancePerLifetimeScope();
            builder.RegisterType<PurchaseService>().As<IPurchaseService>().InstancePerLifetimeScope();
            builder.RegisterType<TrialBalanceService>().As<ITrialBalanceService>().InstancePerLifetimeScope();
            builder.RegisterType<VoucherService>().As<IVoucherService>().InstancePerLifetimeScope();
            builder.RegisterType<AgeingReportService>().As<IAgeingReportService>().InstancePerLifetimeScope();
            builder.RegisterType<CalendarReportService>().As<ICalendarReportService>().InstancePerLifetimeScope();
            builder.RegisterType<MemoryCacheManager>().As<ICacheManager>().InstancePerLifetimeScope();
            builder.RegisterType<AgeingFactory>().As<IAgeingFactory>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerAgeingReportDataService>().Keyed<IAgeingReportDataService>(AgeingReportType.Customer);
            builder.RegisterType<SupplierAgeingReportDateService>().Keyed<IAgeingReportDataService>(AgeingReportType.Supplier);
            builder.RegisterType<ProductAgeingReportDataService>().Keyed<IAgeingReportDataService>(AgeingReportType.Product);
            builder.RegisterType<DealerAgeingReportDateService>().Keyed<IAgeingReportDataService>(AgeingReportType.Dealer);
            builder.RegisterType<SalesProcessingMoniteringService>().As<ISalesProcessingMoniteringService>().InstancePerLifetimeScope();
            builder.RegisterType<SalesDashboardService>().As<ISalesDashboardService>().InstancePerLifetimeScope();
            builder.RegisterType<DashBoardMetricService>().As<IDashBoardMetricService>().InstancePerLifetimeScope();
            builder.RegisterType<JournalVoucherService>().As<IJournalVoucherService>().InstancePerLifetimeScope();
            builder.RegisterType<ReceiptScheduleReportService>().As<IReceiptScheduleReportService>().InstancePerLifetimeScope();
            builder.RegisterType<StockService>().As<IStockService>().InstancePerLifetimeScope();
            builder.RegisterType<Constants>().As<IConstants>().InstancePerLifetimeScope();
            builder.RegisterType<SalesSummaryReportService>().As<ISalesSummaryReportService>().InstancePerLifetimeScope();
            builder.RegisterType<ContractService>().As<IContractService>().InstancePerLifetimeScope();
            builder.RegisterType<ConsumptionService>().As<IConsumptionService>().InstancePerLifetimeScope();

            builder.RegisterType<ProductionService>().As<IProductionService>().InstancePerLifetimeScope();
            builder.RegisterType<SubsidiaryLedgerService>().As<ISubsidiaryLedger>().InstancePerLifetimeScope();
            builder.RegisterType<AnalysisService>().As<IAnalysisService>().InstancePerLifetimeScope();
            builder.RegisterType<FinanceService>().As<IFinanceService>().InstancePerLifetimeScope();
            #region plan chart registar
            builder.RegisterType<PlanChartReportService>().As<IPlanChartReport>().InstancePerLifetimeScope();
            #endregion

            // autoface
            builder.RegisterAssemblyTypes().AssignableTo(typeof(Profile));

            //register your configuration as a single instance
            builder.Register(c => new MapperConfiguration(cfg =>
            {
                //add your profiles (either resolve from container or however else you acquire them)
                foreach (var profile in c.Resolve<IEnumerable<Profile>>())
                {
                    cfg.AddProfile(profile);
                }
            })).AsSelf().SingleInstance();

            //register your mapper
            builder.Register(c => c.Resolve<MapperConfiguration>().CreateMapper(c.Resolve)).As<IMapper>().InstancePerLifetimeScope();
            // end autoface

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