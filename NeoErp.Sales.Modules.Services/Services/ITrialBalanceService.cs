using NeoErp.Core.Helpers;
using NeoErp.Sales.Modules.Services.Models.Ledger;
using NeoErp.Sales.Modules.Services.Models.TrialBalance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Services
{
    public interface ITrialBalanceService
    {
        List<TrialBalanceViewModel> GetTrialBalanceMasterGrid(string formDate, string toDate);
        List<TrialBalanceViewModel> GetTrialBalanceChildLedger(string formDate, string toDate, string id);
        //List<TrialBalanceViewModel> GetTrialBalanceChildLedgerTrial(string formDate, string toDate, NeoErp.Core.Domain.User userinfo, int id, string dataGeneric);
        List<TrialBalanceViewModel> GetTrialBalanceChildLedgerTrial(TrialBalancefilterOption model, NeoErp.Core.Domain.User userinfo, int id, string dataGeneric);
        //List<TrialBalanceViewModel> GetTrialBalanceChildLedgerTrial(filterOption model, NeoErp.Core.Domain.User userinfo, int id, string dataGeneric);
        List<TrialBalanceViewModel> GetTrialBalanceAllTree(string formDate, string toDate);

        List<LedgerDailySummaryModel> GetLedgersummaryByAccCode(string formDate, string toDate, string id);
        List<TrialBalanceViewModel> GETSubLedgerTrialBalance(TrialBalancefilterOption model);
        //List<TrialBalanceViewModel> GETSubLedgerTrialBalance(string formDate, string toDate, string AccCode, string id);
        List<LedgerDailySummaryModel> GetSubLedgerDetailBySubCode(string formDate, string toDate, string accountCode, string SubAccCode);
    }
}
