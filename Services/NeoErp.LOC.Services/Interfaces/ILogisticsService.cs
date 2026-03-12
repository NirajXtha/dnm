using NeoErp.LOC.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.LOC.Services.Interfaces
{
    public interface ILogisticsService
    {
        List<LocationModels> GetAllLocations();
        List<LC_ContractorModel> GetAllContractor();
        List<LC_ClearingAgentModel> GetAllClearingAgent();
        List<LcLogisticContainerModel> GetLogisticPlanContainerDetailByShipmentType( string shipmentType,string InvoiceNo,string create_edit, string Logistic_Detail_Code);
        List<DocumentModels> GetAllDocuments();
        void CreateLogistics(LogisticsModels details, out string sno, out string cinumber);
        LogisticsModels GetAllLogisticDetails(string lctrackno, string invoiceno);

        Logisticlocationdetail GetLogisticETAByInvLocationCode(string invoiceno, string locationcode);

        string CheckLogisticTolocation(string invoiceno, string locationcode);
        void UploadFiles(string path, int serialno, string lctrackno, string cinumber);
        List<Logistic> GetAllLogistics();
        List<Logistic> getAllLogisticFilter(string invoicenumber);
        
        List<LogisticsModels> GetAlLLogisticShipmentLists(string filter,string invoice);
        List<IssuingCarrierModels> GetAllIssuingCarrier();
        List<ContainerModels> GetAllContainer();
        LogisticsModels GetAllLogisticShipmentDetailsByLogisticCode(string lctrackno, string invoiceno, string logisticcode);
        void RemoveLogisticImages(RemoveLogisticImages imageremovedetails);
        List<LogisticsHistoryModel> getAllLogisticsHistoryList(string lctrackno);
        List<LogisticsDocumentHistoryModel> getAllLogisticsDocumentHistoryList(string lctrackno);
        List<LogisticsContainerHistoryModel> getAllLogisticsContainerHistoryList(string lctrackno);
        bool IsAir(string PinvoiceCode);

        List<CommercialInvoiceModel> GetAllLcIpPurchaseOrder(string filter);
        List<CommercialInvoiceModel> GetAllLcIpPurchaseOrderfilter(string filter);

        List<LogisticItemModels> getShipmentlistbyTrackNo(string lcnumber);
    }
}
