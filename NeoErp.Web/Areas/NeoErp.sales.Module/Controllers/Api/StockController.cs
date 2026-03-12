using NeoErp.Core;
using NeoErp.Core.Caching;
using NeoErp.Core.Helpers;
using NeoErp.Sales.Modules.Services.Models;
using NeoErp.Sales.Modules.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Linq.Expressions;
using System.Linq.Dynamic;
using NeoErp.Core.Models.CustomModels;

namespace NeoErp.sales.Module.Controllers.Api
{
    public class StockController : ApiController
    {
        private IStockService _stockService;
        private IWorkContext _workContext;


        public StockController(IStockService stockService, IWorkContext workContext)
        {
            this._stockService = stockService;
            this._workContext = workContext;
        }


        [HttpPost]
        public LocationWiseViewStockModel GetLocationWiseStockReport(filterOption model)
        {
            LocationWiseViewStockModel reportData = new LocationWiseViewStockModel();
            reportData.LocationWiseStockModel = this._stockService.GetLocationWiseStockReport(model.ReportFilters, _workContext.CurrentUserinformation).ToList();
            reportData.total = reportData.LocationWiseStockModel.Count();
            reportData.AggregationResult = KendoGridHelper.GetAggregation(reportData.LocationWiseStockModel, model.aggregate);
            return reportData;
        }
        [HttpPost]

        public LocationWiseItemStockViewModel GetLocationWiseReport(filterOption model)
        {
            var CustomerSalesSummary = new LocationWiseItemStockViewModel();
            var totalLocationWise = new List<LocationWiseItemStockModel>();
            var user = _workContext.CurrentUserinformation;
            var customerSales = _stockService.GetLocationWiseStock(model.ReportFilters, user).AsQueryable();
            if (model.filter != null && (model.filter.Filters != null && model.filter.Filters.Count > 0))
            {
                string whereClause = null;
                var parameters = new System.Collections.ArrayList();
                var filters = model.filter.Filters;
                for (var i = 0; i < filters.Count; i++)
                {
                    if (i == 0)
                        whereClause += string.Format("{0}", KendoGridHelper.BuildWhereClause<LocationWiseItemStockModel>(i, model.filter.Logic, filters[i], parameters));
                    else
                        whereClause += string.Format(" {0} {1}", KendoGridHelper.ToLinqOperator(model.filter.Logic), KendoGridHelper.BuildWhereClause<LocationWiseItemStockModel>(i, model.filter.Logic, filters[i], parameters));
                }
                // Where
                totalLocationWise = customerSales.Where(whereClause, parameters.ToArray()).ToList();
                
            }
            else
            {
                totalLocationWise = customerSales.ToList();
               
            }

            var allCharges = new List<LocationsHeader>();
            //var filterCharge = new List<Charges>();
            //if (CustomerSalesSummary.gridSalesVatReport.Count > 0)
            //    allCharges = _salesRegister.GetSumCharges(model.ReportFilters);

            foreach (var a in totalLocationWise)
            {
                var items=new  LocationVsItemsStockModel();
                items.ItemCode = a.Item_Code;
                items.ItemName = a.ITEM_EDESC;
                items.MuCode = a.Mu_code;
                

                if (CustomerSalesSummary.LocationWiseStockModel.Any(x => x.ItemCode==a.Item_Code))
                    continue;
                var itemsLocation = _stockService.GetLocationHeader(model.ReportFilters,user);
                foreach(var location in itemsLocation)
                {
                    var locationsModel = new LocationWiseItemStockModel();
                    locationsModel.LocationCode = location.LocationNo;
                    locationsModel.ITEM_EDESC = location.LocationTitle;
                    var totalvalue = totalLocationWise.Where(x => x.Item_Code == a.Item_Code && a.LocationCode == location.LocationNo).Sum(x => x.AvilableStock);
                    locationsModel.AvilableStock = totalvalue;
                    items.WareHouseList.Add(locationsModel);


                }

                CustomerSalesSummary.LocationWiseStockModel.Add(items);
               
            }
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    // s.Field =  s.Field;
                    CustomerSalesSummary.LocationWiseStockModel = CustomerSalesSummary.LocationWiseStockModel.OrderBy(s.Field + " " + s.Dir).ToList();
                }
            }
            CustomerSalesSummary.total = CustomerSalesSummary.LocationWiseStockModel.Count();

            var aggregationDictionary = KendoGridHelper.GetAggregation(CustomerSalesSummary.LocationWiseStockModel,model.aggregate);

            CustomerSalesSummary.LocationWiseStockModel = CustomerSalesSummary.LocationWiseStockModel.Skip((model.page - 1) * model.pageSize)
                      .Take(model.pageSize).ToList();

            

            
            //var groupData = filterCharge.GroupBy(q => q.CHARGE_CODE).Select(group => new {
            //    Code = group.Key,
            //    Sum = group.Sum(q => q.CHARGE_AMOUNT),
            //    Min = group.Min(q => q.CHARGE_AMOUNT),
            //    Max = group.Max(q => q.CHARGE_AMOUNT),
            //    Average = group.Average(q => q.CHARGE_AMOUNT)
            //});

            //foreach (var item in groupData)
            //{
            //    aggregationDictionary.Add(item.Code, new AggregationModel()
            //    {
            //        average = item.Average ?? 0,
            //        max = item.Max ?? 0,
            //        min = item.Min ?? 0,
            //        sum = item.Sum ?? 0
            //    });
            //}

            foreach (var item in model.aggregate)
            {
                if (aggregationDictionary.ContainsKey(item.field))
                    continue;

                aggregationDictionary.Add(item.field, new AggregationModel()
                {
                    average = 0,
                    max = 0,
                    min = 0,
                    sum = 0
                });
            }

            CustomerSalesSummary.AggregationResult = aggregationDictionary;

            return CustomerSalesSummary;
        }
    }
}
