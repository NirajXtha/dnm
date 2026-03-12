using NeoErp.LOC.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.LOC.Services.Interfaces
{
    public interface ILcLogisticPlanService
    {
        string AddLogisticPlan(LcLogisticPlanModel Model);
        List<ItemDetails> GetLCLogisticPlanItemsByLCNumber(string lcnumber);
        LcLogisticPlanModel GetLogisticPlanList(string lcnumber);
        List<ItemDetails> GetLCLogisticPlanItemsByLOT_NO(string LOT_NO,string lcnumber);
        List<LogisticContainerPlan> GetLogisticPlanbyperformainvoice(string PinvoiceCode);
        List<LC_LOGISTIC_PLAN> getAllLcLogisticPlan();
        List<LC_LOGISTIC_PLAN> getAllLcLogisticPlanFilter(string lcnumber);
        List<LC_LOGISTIC_PLANITEMLIST> getAllLcLogisticPlanItemListByTrackNumberAndLogisticPlanCode(string trackNumber, string logisticPlanCode);
        List<LC_LOGISTIC_PLANCONTAINERLIST> getAllLcLogisticPlanContainerListByTrackNumberAndLogisticPlanCode(string trackNumber, string lotNumber);
        // void UpdateQuantity(LC_LOGISTIC_PLANITEMLIST itemdetail);
        string UpdateQuantity(LC_LOGISTIC_PLANITEMLIST itemdetail);
        List<LC_LOGISTIC_PLAN_CONTAINER> GetUpdateShipmentData(string LOGISTIC_PLAN_CODE);
        List<CommercialInvoiceModel> GetAllLcNumbers(string filter);

        List<CommercialInvoiceModel> GetAllLcNumbersfilter(string filter);
        void UpdateLogisticPlan(LC_LOGISTIC_PLANVVIEWMODEL LcLogisticPlanModel);

    }
}
