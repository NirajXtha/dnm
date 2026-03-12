using NeoErp.Core.Models;
using NeoERP.ProjectManagement.Service.Models;
using NeoERP.ProjectManagement.Service.Services;

using System.Collections.Generic;

namespace NeoERP.ProjectManagement.Service.Interface
{
    public interface ISaveDocTemplateSalesModule
    {
        ResponseMessage SaveSalesOrderFormData(SalesOrderDetailModel salesOrderDetailModel, CommonFieldForSales commonFieldForSales, NeoErpCoreEntity dbcontext = null);

        ResponseMessage SaveSalesChalanFormData(SalesChalanDetailModel salesChalanDetailModel,CommonFieldForSales commonFieldForSales, NeoErpCoreEntity dbcontext = null); 
        ResponseMessage SaveSalesInvoiceFormData(SalesInvoiceDetailModel salesInvoiceDetailModel,CommonFieldForSales commonFieldForSales, NeoErpCoreEntity dbcontext = null);
        ResponseMessage SaveSalesReturnFormData(SalesReturnDetailModel salesReturnDetailModel,CommonFieldForSales commonFieldForSales, NeoErpCoreEntity dbcontext = null);
        ResponseMessage SaveGenericTableFormData(SalesFieldsForSavingFormData salesFields,CommonFieldForSales commonFieldForSales);

        ResponseMessage UpdateSalesOrderFormData(SalesOrderDetailModel salesOrderDetailModel,CommonFieldForSales commonFieldForSales);
        ResponseMessage UpdateSalesChalanFormData(SalesChalanDetailModel salesChalanDetailModel,CommonFieldForSales commonFieldForSales, NeoErpCoreEntity dbcontext = null);
        ResponseMessage UpdateSalesInvoiceFormData(SalesInvoiceDetailModel salesInvoiceDetailModel,CommonFieldForSales commonFieldForSales, NeoErpCoreEntity dbcontext = null);
        ResponseMessage UpdateSalesReturnFormData(SalesReturnDetailModel salesReturnDetailModel,CommonFieldForSales commonFieldForSales, NeoErpCoreEntity dbcontext = null);
        ResponseMessage UpdateGenericTableFormData();
        bool SavePostedTransactionValue(/*SalesOrderDetailModel masterValue,*/ CommonFieldForSales commonFieldForSales, NeoErpCoreEntity dbcontext = null);


         //bool SaveBatchTransactionValues(List<BATCHTRANSACTIONDATA> batchTransaction, CommonFieldForSales commonValue, NeoErpCoreEntity dbcontext = null);




    }
}
