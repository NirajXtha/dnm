using NeoERP.DocumentTemplate.Service.Models;
using System;
using System.Collections.Generic;

namespace NeoERP.DocumentTemplate.Service.Interface
{
    public interface IOrderDispatch
    {
        #region DISPATCH ORDER API INTERFACE
        List<DispatchDocument> ListAllDocument();
        List<DispatchDocument> ListDocumentByBranches(string branchCodeList);
        List<OrderDispatchModel> FindAllDataToDispatch(SearchParameter searchParameter);
        string SaveDispatchedOrder(List<OrderDispatchModel> model);
        List<OrderDispatchModel> GetAllDispatchPlannedReport(SearchParameter searchParameter);
        int? GenerateDispatchNo();

        string GetDispatcher();

        #endregion


        #region LOADING SLIP API INTERFACE

        List<VehicleRegistrationModel> GetAllRegisteredVehicleToDispatch(string entryDate = "");

        List<OrderDispatchModel> GetAllDispatchForLoadingSlip(string transactionDate);

        string GenerateLoadingSlip(LoadingSlipModal  modal);

        #endregion

        #region LOADING SLIP PRINTER API INTERFACE

        List<LoadingSlipModalForPrint> GetLoadingSlipPrintList();

        List<LoadingSlipModalForPrint> GetLoadingSlipPrintListByDispatchNo(string number);

        #endregion
    }
}
