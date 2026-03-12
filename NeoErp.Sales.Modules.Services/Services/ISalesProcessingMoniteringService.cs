using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Services
{
   public interface ISalesProcessingMoniteringService
    {

        int GetTodaySalesOrder(string date, string company_code = "01");
        int GetTodayApprovedsalesOrder(string date, string company_code = "01");
        int GetTodayVehicleRegister(string date, string company_code = "01");
        int GetTodayLoadingSlipGenerate(string date, string company_code = "01");
        int GetTodayLoadedVehicleOut(string date, string company_code = "01");
        int GetTodayPendingForDispatch(string date, string company_code = "01");
        int GetTodayDistributionPurchaseOrderCount(string date, string company_code = "01");
        int GetDispatchMangement(string date, string company_code = "01");
        int GetVechicalIN(string date, string company_code = "01");
    }
}
