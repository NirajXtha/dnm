using NeoErp.Core;
using NeoErp.Core.Helpers;
using NeoErp.Sales.Modules.Services.Models;
using NeoErp.Sales.Modules.Services.Models.Production;
using NeoErp.Sales.Modules.Services.Services;
using NeoErp.Sales.Modules.Services.Services.Production;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoErp.sales.Module.Controllers.Api
{
    public class ProductionController : ApiController
    {
        public IProductionService _productionService { get; set; }
        private IWorkContext _workContext;
        private IStockService _stockService;
        public ProductionController(IProductionService productionService, IWorkContext workContext, IStockService stockService)
        {
            this._productionService = productionService;
            this._workContext = workContext;
            this._stockService = stockService;
        }
        [HttpPost]
        public List<ProductionRegisterModel> GetProductionRegister(filterOption model)
        {
            List<ProductionRegisterModel> result = _productionService.GetProductionRegister(model.ReportFilters, _workContext.CurrentUserinformation);
            return result;
        }
        [HttpPost]
        public ProductionStockInOutSummaryViewModel GetProductionStockInOutSummary(filterOption model)
        {
            ProductionStockInOutSummaryViewModel reportData = new ProductionStockInOutSummaryViewModel();
            reportData.ProductionStockInOutSummaryModel = this._productionService.GetProductionStockInOutSummary(model.ReportFilters, _workContext.CurrentUserinformation).ToList();
            reportData.total = reportData.ProductionStockInOutSummaryModel.Count();
            reportData.AggregationResult = KendoGridHelper.GetAggregation(reportData.ProductionStockInOutSummaryModel, model.aggregate);
            return reportData;
        }
    }
}
