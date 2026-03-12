using NeoERP.DocumentTemplate.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Interface
{
    public interface ISaveDocTemplate
    {
        List<SalesOrderDetail> GetMasterTransactionByOrderNo(string orderNumber);
      
        SalesOrderDetail MapSalesOrderMasterColumnWithValue(string masterColumn, string primaryDateColumn, string primaryColumn);
        SalesBlanketDetail MapSalesBlanketMasterColumnWithValue(string masterColumn, string primaryDateColumn, string primaryColumn);
        Dictionary<string, string> MapMasterColumnToDic(string masterColumn);

        List<Dictionary<string, object>> MapChildColumnToDict(string childColumn);
        SalesChalanDetail MapSalesChalanMasterColumnWithValue(string masterColumn, string primaryDateColumn, string primaryColumn);
        SalesInvoiceDetail MapSalesInvoiceMasterColumnWithValue(string masterColumn, string primaryDateColumn, string primaryColumn);
        SalesInvoiceJewelleryDetail MapSalesInvoiceJewelleryMasterColumnWithValue(string masterColumn, string primaryDateColumn, string primaryColumn);
        SalesReturnDetail MapSalesReturnMasterColumnWithValue(string masterColumn, string primaryDateColumn, string primaryColumn);
        SalesReturnJewelleryDetail MapSalesReturnJewelleryMasterColumnWithValue(string masterColumn, string primaryDateColumn, string primaryColumn);

        List<SalesOrderDetail> MapOrderChildColumnWithValue(string childColumn);
        List<SalesBlanketDetail> MapBlanketChildColumnWithValue(string childColumn);
        List<SalesChalanDetail> MapChalanChildColumnWithValue(string childColumn);
        List<SalesInvoiceDetail> MapInvoiceChildColumnWithValue(string childColumn);
        List<SalesInvoiceJewelleryDetail> MapInvoiceJewelleryChildColumnWithValue(string childColumn);
        List<SalesReturnDetail> MapReturnChildColumnWithValue(string childColumn);
        List<SalesReturnJewelleryDetail> MapReturnJewelleryChildColumnWithValue(string childColumn);

        List<CustomOrderColumn> MapCustomOrderColumnWithValue(string custom_column_val);
        List<CustomBlanketColumn> MapCustomBlanketColumnWithValue(string custom_column_val);

        List<ChargeOnSales> MapChargesColumnWithValue(string charges);

        ShippingDetails MapShippingDetailsColumnValue(string shippingDetails);

        string GetTemplateNo();
        string AddToFormTemplateSetup(FormDetails model, string templateNo);
        string UpdateFormTemlateSetup(FormDetails model);
        string AddMasterColumnFormSetup(Dictionary<string, string> model, DraftFormModel draftFormModel);
        string AddChildColumnFormSetup(List<Dictionary<string, object>> childColumnWithValue, DraftFormModel draftFormModel);
        void DeleteFromFormTemplateSetupByTemplateNo(string templateNo);
        DraftFormModel GetFormTemplateDetailsByTemplateNo(string templateNo);
        List<REF_MODEL_DEFAULT> MapRefrenceModel(string ReFModel);

        List<BATCHTRANSACTIONDATA> MapBatchTransactionValue(string batchValue);
    }
}
