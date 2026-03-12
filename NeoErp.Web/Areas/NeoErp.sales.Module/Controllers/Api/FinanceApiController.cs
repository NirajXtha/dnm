using NeoErp.Core;
using NeoErp.Core.Helpers;
using NeoErp.Sales.Modules.Services.Models.FinanceReport;
using NeoErp.Sales.Modules.Services.Services.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoErp.sales.Module.Controllers.Api
{
    public class FinanceApiController : ApiController
    {
        private readonly IFinanceService _financeService;
        private readonly IWorkContext _workContext;
        public FinanceApiController(IWorkContext workContext, IFinanceService financeService)
        {
            _workContext = workContext;
            _financeService = financeService;
        }
        [HttpPost]
        public MovementAnalysisViewModel GetMovementAnalysis(filterOption model)
        {
            MovementAnalysisViewModel reportData = new MovementAnalysisViewModel();
            var totalBalance = 0m;
            var creditLimit = 0m;
            var freeLimit = 0m;

            reportData.MovementAnalysisModel = this._financeService.GetMovementAnalysis(model.ReportFilters, _workContext.CurrentUserinformation).ToList();
            var midRow = reportData.MovementAnalysisModel.Count;
            totalBalance = reportData.MovementAnalysisModel.Select(x => Convert.ToDecimal(x.Balance)).Sum();
            creditLimit = reportData.MovementAnalysisModel.Select(x => Convert.ToDecimal(x.CreditLimit)).FirstOrDefault();
            freeLimit = creditLimit - totalBalance;
            var midIndex = reportData.MovementAnalysisModel.Count;
            if (midIndex % 2 == 0)
            {
                for (var i = 0; i < reportData.MovementAnalysisModel.Count; i++)
                {
                    if (midIndex / 2 == i + 1)
                    {
                        reportData.MovementAnalysisModel[i].CreditLimit = reportData.MovementAnalysisModel[i].CreditLimit;
                        reportData.MovementAnalysisModel[i].FreeLimit = freeLimit.ToString();
                    }

                    else
                    {
                        reportData.MovementAnalysisModel[i].CreditLimit = "--";
                        reportData.MovementAnalysisModel[i].FreeLimit = "--";
                    }

                }
            }
            else
            {
                for (var i = 0; i < reportData.MovementAnalysisModel.Count; i++)
                {
                    if ((midIndex + 1) / 2 == i + 1)
                    {
                        reportData.MovementAnalysisModel[i].CreditLimit = reportData.MovementAnalysisModel[i].CreditLimit;
                        reportData.MovementAnalysisModel[i].FreeLimit = Convert.ToString(freeLimit);

                    }
                    else
                    {
                        reportData.MovementAnalysisModel[i].CreditLimit = "--";
                        reportData.MovementAnalysisModel[i].FreeLimit = "--";
                    }

                }
            }

            for (var i = 0; i < reportData.MovementAnalysisModel.Count - 1; i++)
            {
                reportData.MovementAnalysisModel[i + 1].Balance = (reportData.MovementAnalysisModel[i].Balance + reportData.MovementAnalysisModel[i + 1].Balance) - reportData.MovementAnalysisModel[i + 1].RecAmount;
            }
            reportData.total = reportData.MovementAnalysisModel.Count();
            reportData.AggregationResult = KendoGridHelper.GetAggregation(reportData.MovementAnalysisModel, model.aggregate);
            reportData.AggregationResult.Where(x => x.Key == "Balance").Select(y => y.Value).Select(z => z.sum = totalBalance).FirstOrDefault();
            return reportData;
        }
    }
}
