using Autofac;
using Autofac.Integration.WebApi;
using NeoErp.Core.Infrastructure;
using NeoErp.Core.Infrastructure.DependencyManagement;
using NeoErp.Services.Repository.ThirdPartyApi;
using NeoERP.DocumentTemplate.Service.Interface;
using NeoERP.DocumentTemplate.Service.Interface.CustomForm;
using NeoERP.DocumentTemplate.Service.Interface.ProductionManagement;
using NeoERP.DocumentTemplate.Service.Interface.PurchaseIndentOrderAdjustment;
using NeoERP.DocumentTemplate.Service.Interface.SalesOrderAdjustment;
using NeoERP.DocumentTemplate.Service.Interface.ThirdPartyApi;
using NeoERP.DocumentTemplate.Service.Repository;
using NeoERP.DocumentTemplate.Service.Repository.ThirdPartyApi;
using NeoERP.DocumentTemplate.Service.Services;
using NeoERP.DocumentTemplate.Service.Services.CustomForm;
using NeoERP.DocumentTemplate.Service.Services.ProductionManagement;
using NeoERP.DocumentTemplate.Service.Services.PurchaseOrderIndentAdjustment;
using NeoERP.DocumentTemplate.Service.Services.SalesOrderAdjustment;
using System.Reflection;

namespace NeoERP.DocumentTemplate.Infrastructure
{
    public class DocumentTemplateDependency : IDependencyRegistrar
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
            builder.RegisterType<TestTemplateRepo>().As<ITestTemplateRepo>().InstancePerLifetimeScope();
            builder.RegisterType<FormTemplateRepo>().As<IFormTemplateRepo>().InstancePerLifetimeScope();
            builder.RegisterType<FormSetupRepo>().As<IFormSetupRepo>().InstancePerLifetimeScope();
            builder.RegisterType<SalesOrderRepo>().As<ISalesOrderRepo>().InstancePerLifetimeScope();
            builder.RegisterType<ContraVoucherRepo>().As<IContraVoucher>().InstancePerLifetimeScope();
            builder.RegisterType<InventoryVoucherRepo>().As<IInventoryVoucher>().InstancePerLifetimeScope();
            builder.RegisterType<ValidationRepo>().As<IValidationRepo>().InstancePerLifetimeScope();
            builder.RegisterType<DocumentSetupRepo>().As<IDocumentStup>().InstancePerLifetimeScope();
            builder.RegisterType<ShymphonyService>().As<IShymphonyService>().InstancePerLifetimeScope();
            builder.RegisterType<OSPreferenceSetup>().As<IOSPreferenceSetup>().InstancePerLifetimeScope();
            builder.RegisterType<ShymphonyService>().As<IShymphonyService>().InstancePerLifetimeScope();
            builder.RegisterType<SalesInvoiceService>().As<ISalesInvoice>().InstancePerLifetimeScope();


            //Price Setup Injection

            builder.RegisterType<PriceSetup>().As<IPriceSetup>().InstancePerLifetimeScope();
            builder.RegisterType<SaveDocTemplateService>().As<ISaveDocTemplate>().InstancePerLifetimeScope();
            builder.RegisterType<SaveDocTemplateSalesModule>().As<ISaveDocTemplateSalesModule>().InstancePerLifetimeScope();

            builder.RegisterType<FinancialVoucherSaveService>().As<IFinancialVoucherSaveService>().InstancePerLifetimeScope();
            builder.RegisterType<OrderDispatchService>().As<IOrderDispatch>().InstancePerLifetimeScope();

            // builder.RegisterType<SalesOrderIndentService>().As<ISalesOrderIndent>().InstancePerLifetimeScope();
            builder.RegisterType<PurchaseIndentAdjustmentService>().As<IPurchaseIndentAdjustment>().InstancePerLifetimeScope();
            builder.RegisterType<PurchaseOrderAdjustmentService>().As<IPurchaseOrderAdjustment>().InstancePerLifetimeScope();
            builder.RegisterType<SalesOrderAdjustmentService>().As<ISalesOrderAdjustment>().InstancePerLifetimeScope();

            builder.RegisterType<ProcessSetupBomService>().As<IProcessSetupBom>().InstancePerLifetimeScope();

            builder.RegisterType<SubLedgerMappingSetupService>().As<ISubLegerMappingSetup>().InstancePerLifetimeScope();

            builder.RegisterType<PostDataChequeService>().As<IPostDataCheque>().InstancePerLifetimeScope();

          
            // Production Management
            builder.RegisterType<ProductionManagementService>().As<IProductionManagement>().InstancePerLifetimeScope();

            builder.RegisterType<IRDDataSyncService>().As<IIRDDataSyncService>().InstancePerLifetimeScope();

        }
    }
}